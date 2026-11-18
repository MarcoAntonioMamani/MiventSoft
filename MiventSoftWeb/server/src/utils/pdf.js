// =====================================================================
//  HTML -> PDF con el Chrome / Edge del servidor (modo invisible, protocolo DevTools por tuberia)
//   - El PDF sale igual que la hoja impresa (@page, cabecera repetida en cada hoja, texto seleccionable).
//   - Seguridad: sin red (proxy muerto + toda peticion se rechaza), sin JavaScript, sin puerto de depuracion
//     (--remote-debugging-pipe), perfil temporal propio. Solo se dibuja el HTML recibido (imagenes en data:).
//   - Un navegador para todos, una hoja a la vez (cola). Se cierra solo tras 5 min sin uso.
//   - PDF_NAVEGADOR en .env: ruta de chrome.exe / msedge.exe si no esta en el lugar de siempre.
// =====================================================================
const { spawn } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');

const TIEMPO_MAX = 45 * 1000;
const INACTIVO = 5 * 60 * 1000;
const COLA_MAX = 8;

class ErrorPdf extends Error {}

function rutaNavegador() {
    const env = (process.env.PDF_NAVEGADOR || '').trim();
    const pf = process.env.ProgramFiles || 'C:\\Program Files';
    const pf86 = process.env['ProgramFiles(x86)'] || 'C:\\Program Files (x86)';
    const local = process.env.LOCALAPPDATA || '';
    const candidatos = [
        env,
        path.join(pf, 'Google', 'Chrome', 'Application', 'chrome.exe'),
        path.join(pf86, 'Google', 'Chrome', 'Application', 'chrome.exe'),
        local && path.join(local, 'Google', 'Chrome', 'Application', 'chrome.exe'),
        path.join(pf86, 'Microsoft', 'Edge', 'Application', 'msedge.exe'),
        path.join(pf, 'Microsoft', 'Edge', 'Application', 'msedge.exe'),
        '/usr/bin/google-chrome', '/usr/bin/chromium', '/usr/bin/chromium-browser', '/usr/bin/microsoft-edge',
    ].filter(Boolean);
    return candidatos.find((r) => { try { return fs.statSync(r).isFile(); } catch { return false; } }) || null;
}

// ---------------- navegador (uno, reutilizado) ----------------
let _nav = null; // { proc, enviar, perfil, cerrar }
let _timerInactivo = null;

function abrirNavegador() {
    const exe = rutaNavegador();
    if (!exe) throw new ErrorPdf('No se encontro Google Chrome ni Microsoft Edge en el servidor para generar el PDF. Instale uno o indique su ruta en PDF_NAVEGADOR (server\\.env).');
    const perfil = fs.mkdtempSync(path.join(os.tmpdir(), 'mivent-pdf-'));
    const proc = spawn(exe, [
        '--headless=new', '--remote-debugging-pipe', `--user-data-dir=${perfil}`,
        '--no-first-run', '--no-default-browser-check', '--disable-extensions', '--disable-sync',
        '--disable-background-networking', '--disable-component-update', '--disable-default-apps',
        '--disable-gpu', '--mute-audio', '--hide-scrollbars',
        '--proxy-server=127.0.0.1:9', '--proxy-bypass-list=<-loopback>', // sin red
        'about:blank',
    ], { stdio: ['ignore', 'ignore', 'ignore', 'pipe', 'pipe'], windowsHide: true });

    const pendientes = new Map();
    const oyentes = new Set();
    let id = 0;
    let buffer = '';
    let vivo = true;

    const morir = (motivo) => {
        if (!vivo) return;
        vivo = false;
        for (const p of pendientes.values()) p.error(new ErrorPdf(motivo));
        pendientes.clear();
        if (_nav && _nav.proc === proc) _nav = null;
        try { proc.kill(); } catch { /* ya cerrado */ }
        setTimeout(() => fs.rm(perfil, { recursive: true, force: true }, () => {}), 2000);
    };
    proc.on('exit', () => morir('El navegador del servidor se cerro mientras generaba el PDF. Intente de nuevo.'));
    proc.on('error', (e) => morir(`No se pudo abrir el navegador para el PDF: ${e.message}`));
    proc.stdio[3].on('error', () => {});
    proc.stdio[4].on('data', (d) => {
        buffer += d.toString('utf8');
        let i;
        while ((i = buffer.indexOf('\0')) >= 0) {
            const txt = buffer.slice(0, i);
            buffer = buffer.slice(i + 1);
            let m;
            try { m = JSON.parse(txt); } catch { continue; }
            if (m.id && pendientes.has(m.id)) {
                const p = pendientes.get(m.id); pendientes.delete(m.id);
                if (m.error) p.error(new Error(m.error.message)); else p.ok(m.result);
            } else if (m.method) oyentes.forEach((f) => f(m));
        }
    });

    const enviar = (method, params = {}, sessionId) => new Promise((ok, error) => {
        if (!vivo) { error(new ErrorPdf('El navegador del servidor no esta disponible.')); return; }
        const n = ++id;
        pendientes.set(n, { ok, error });
        proc.stdio[3].write(`${JSON.stringify({ id: n, method, params, ...(sessionId ? { sessionId } : {}) })}\0`);
    });
    _nav = { proc, enviar, oyentes, cerrar: () => morir('cerrado') };
    return _nav;
}

function navegador() {
    clearTimeout(_timerInactivo);
    return _nav || abrirNavegador();
}

function programarCierre() {
    clearTimeout(_timerInactivo);
    _timerInactivo = setTimeout(() => _nav?.cerrar(), INACTIVO);
    _timerInactivo.unref?.();
}

// ---------------- una hoja ----------------
async function dibujar(html) {
    const nav = navegador();
    const { targetId } = await nav.enviar('Target.createTarget', { url: 'about:blank' });
    let sessionId;
    const quitar = (f) => nav.oyentes.delete(f);
    const rechazar = (m) => {
        if (m.sessionId === sessionId && m.method === 'Fetch.requestPaused') {
            nav.enviar('Fetch.failRequest', { requestId: m.params.requestId, errorReason: 'BlockedByClient' }, sessionId).catch(() => {});
        }
    };
    try {
        ({ sessionId } = await nav.enviar('Target.attachToTarget', { targetId, flatten: true }));
        const s = (m, p) => nav.enviar(m, p, sessionId);
        nav.oyentes.add(rechazar);
        await s('Page.enable');
        await s('Emulation.setScriptExecutionDisabled', { value: true });
        await s('Fetch.enable', { patterns: [{ urlPattern: '*' }] }); // ninguna peticion sale (las data: no pasan por aqui)
        const { frameTree } = await s('Page.getFrameTree');
        await s('Page.setDocumentContent', { frameId: frameTree.frame.id, html });
        await new Promise((r) => setTimeout(r, 250)); // imagenes data: y fuentes
        const { data } = await s('Page.printToPDF', { printBackground: true, preferCSSPageSize: true, displayHeaderFooter: false, marginTop: 0, marginBottom: 0, marginLeft: 0, marginRight: 0 });
        return Buffer.from(data, 'base64');
    } finally {
        quitar(rechazar);
        nav.enviar('Target.closeTarget', { targetId }).catch(() => {});
    }
}

// ---------------- cola: una hoja a la vez ----------------
let _cola = Promise.resolve();
let _enCola = 0;

/** Convierte un documento HTML completo en PDF (Buffer). */
function htmlAPdf(html) {
    if (_enCola >= COLA_MAX) return Promise.reject(new ErrorPdf('El servidor esta generando muchos PDF a la vez. Intente en unos segundos.'));
    _enCola++;
    const trabajo = _cola.then(() => new Promise((ok, error) => {
        const t = setTimeout(() => { _nav?.cerrar(); error(new ErrorPdf('El PDF tardo demasiado en generarse. Intente con menos registros.')); }, TIEMPO_MAX);
        dibujar(html).then(ok, error).finally(() => clearTimeout(t));
    }));
    _cola = trabajo.catch(() => {}).finally(() => { _enCola--; programarCierre(); });
    return trabajo;
}

process.on('exit', () => _nav?.cerrar());

module.exports = { htmlAPdf, ErrorPdf, rutaNavegador };
