// =====================================================================
//  Datos del equipo del cliente para la Auditoria De Registros (sql/10_AuditoriaEquipo.sql)
//  Solo en lo que modifica o elimina (PUT / DELETE). Queda en req.clienteAud y, por el contexto de la
//  peticion, MetodoDatos lo deja en AuditoriaContexto al llamar a MAM_Ventas / MAM_Compras / MAM_Movimientos.
//   ip        : "IP local | IP publica" (app.set('trust proxy', 1)); la publica de un equipo de la red local
//               es la de internet de la oficina (api.ipify.org, una vez por hora)
//   navegador : "Chrome 131 · Windows 10/11" / "App Sanbit · Android 13 · SM-A135F"
//   equipo    : nombre REAL de la PC (la del servidor: os.hostname; otra PC: DNS o NetBIOS) o el modelo del
//               celular; si en "Nombre de este equipo" se le dio un nombre: "CAJA 1 (DESKTOP-ABC)"
//   ubicacion : X-Ubicacion "lat,lng,precision" (GPS de la app; el navegador por http no la entrega)
//  Nada de esto es confiable para seguridad (lo envia el navegador): es informativo, para la auditoria.
// =====================================================================
const dns = require('node:dns').promises;
const os = require('node:os');
const { execFile } = require('node:child_process');

const texto = (v, max) => {
    let s = String(v ?? '');
    try { s = decodeURIComponent(s); } catch { /* tal cual */ }
    return s.replace(/[\u0000-\u001f\u007f<>]/g, '').trim().slice(0, max); // eslint-disable-line no-control-regex
};

function ipCliente(req) {
    const ip = String(req.ip || req.socket?.remoteAddress || '');
    return ip.replace(/^::ffff:/, '') || null;
}

/** Navegador y sistema en pocas palabras, a partir del User-Agent */
function navegadorDe(ua) {
    ua = String(ua || '');
    if (!ua) return null;
    const v = (re) => (re.exec(ua) || [])[1];
    let nav = 'Navegador';
    if (/; wv\)/.test(ua) || /SantitApp|Sanbit/i.test(ua)) nav = 'App Sanbit';
    else if (v(/Edg(?:A|iOS)?\/(\d+)/)) nav = `Edge ${v(/Edg(?:A|iOS)?\/(\d+)/)}`;
    else if (v(/OPR\/(\d+)/)) nav = `Opera ${v(/OPR\/(\d+)/)}`;
    else if (v(/SamsungBrowser\/(\d+)/)) nav = `Samsung Internet ${v(/SamsungBrowser\/(\d+)/)}`;
    else if (v(/Firefox\/(\d+)/)) nav = `Firefox ${v(/Firefox\/(\d+)/)}`;
    else if (v(/(?:Chrome|CriOS)\/(\d+)/)) nav = `Chrome ${v(/(?:Chrome|CriOS)\/(\d+)/)}`;
    else if (v(/Version\/(\d+).*Safari/)) nav = `Safari ${v(/Version\/(\d+).*Safari/)}`;
    let so = '';
    const android = v(/Android ([\d.]+)/);
    if (android) {
        // modelo: "Android 13; SM-A135F Build/..." o "Android 13; K)" (Chrome reducido)
        const modelo = v(/Android [\d.]+; (?:[a-z]{2}-[a-z]{2}; )?([^;)]+?)(?: Build\/|\))/i);
        so = `Android ${android}${modelo && modelo !== 'K' ? ` · ${modelo.trim()}` : ''}`;
    } else if (/iPhone|iPad/.test(ua)) so = `${/iPad/.test(ua) ? 'iPad' : 'iPhone'} iOS ${(v(/OS (\d+[_\d]*)/) || '').replace(/_/g, '.')}`;
    else if (/Windows NT 10/.test(ua)) so = 'Windows 10/11';
    else if (v(/Windows NT ([\d.]+)/)) so = `Windows NT ${v(/Windows NT ([\d.]+)/)}`;
    else if (/Mac OS X/.test(ua)) so = 'macOS';
    else if (/Linux/.test(ua)) so = 'Linux';
    return [nav, so].filter(Boolean).join(' · ').slice(0, 200);
}

const esLocal = (ip) => !ip || ip === '127.0.0.1' || ip === '::1';
const esPrivada = (ip) => esLocal(ip) || /^(10\.|192\.168\.|172\.(1[6-9]|2\d|3[01])\.|169\.254\.|f[cd][0-9a-f]{2}:|fe80:)/i.test(ip);
const conLimite = (p, ms) => Promise.race([p.catch(() => null), new Promise((r) => setTimeout(() => r(null), ms))]);

// NetBIOS (redes Windows sin DNS interno): "nbtstat -A 192.168.1.20" -> nombre de la PC (<00> UNIQUE)
function nombreNetbios(ip) {
    if (process.platform !== 'win32' || !/^\d{1,3}(\.\d{1,3}){3}$/.test(ip)) return Promise.resolve(null);
    return new Promise((r) => {
        execFile('nbtstat', ['-A', ip], { timeout: 2500, windowsHide: true }, (err, salida) => {
            if (!salida) return r(null); // nbtstat puede terminar con codigo de error aunque haya respondido
            const m = /^\s*([^\s<]{1,15})\s*<00>\s+(?!GR)/im.exec(salida); // la primera <00> que no es GRUPO
            r(m ? m[1].toUpperCase() : null);
        });
    });
}

// Nombre REAL del equipo: la misma PC del servidor -> su nombre; otra PC -> DNS de la red y si no, NetBIOS.
// Cache 1 h. Se averigua apenas el equipo usa el sistema (ver middleware), asi al modificar ya esta listo.
const _nombres = new Map();
function nombrePc(ip) {
    if (esLocal(ip)) return Promise.resolve(os.hostname().toUpperCase());
    if (!esPrivada(ip)) return Promise.resolve(null); // por internet el nombre DNS es del proveedor, no de la PC
    const c = _nombres.get(ip);
    if (c && c.vence > Date.now()) return c.promesa;
    // getnameinfo del sistema: en Windows prueba DNS, LLMNR y NetBIOS (como "ping -a"); si no, nbtstat
    const limpio = (n) => (n && !/^[\d.:a-f]+$/i.test(n) ? n.split('.')[0].toUpperCase() : null);
    const promesa = (async () => limpio(await conLimite(dns.lookupService(ip, 445).then((r) => r.hostname), 2500))
        || limpio(await conLimite(dns.reverse(ip).then((n) => n[0]), 800))
        || (await conLimite(nombreNetbios(ip), 3000)))();
    _nombres.set(ip, { promesa, vence: Date.now() + 3600 * 1000 });
    if (_nombres.size > 500) _nombres.delete(_nombres.keys().next().value);
    return promesa;
}

// IP publica: si el equipo esta en la red local, la de internet es la de la oficina (la misma del servidor).
// Se consulta una vez por hora a api.ipify.org (solo devuelve la IP; no se envia ningun dato).
// Sin internet o con AUDITORIA_IP_PUBLICA=0 queda vacia.
let _publica = { ip: null, vence: 0, promesa: null };
function ipPublicaServidor() {
    if (process.env.AUDITORIA_IP_PUBLICA === '0') return Promise.resolve(null);
    if (_publica.vence > Date.now()) return Promise.resolve(_publica.ip);
    if (!_publica.promesa) {
        _publica.promesa = conLimite(fetch('https://api.ipify.org', { signal: AbortSignal.timeout(3000) }).then((r) => (r.ok ? r.text() : null)), 3500)
            .then((t) => {
                const ip = /^[\d.:a-f]{3,45}$/i.test(String(t || '').trim()) ? String(t).trim() : null;
                _publica = { ip, vence: Date.now() + (ip ? 3600 : 300) * 1000, promesa: null };
                return ip;
            });
    }
    return _publica.promesa;
}

// Ubicacion APROXIMADA por IP publica (respaldo cuando no hay GPS): ciudad / zona, con km de error.
// ipapi.co (https, sin clave). Cache 1 dia por IP. Se envia SOLO la IP publica. AUDITORIA_UBICACION_IP=0 la desactiva.
// Se guarda con PrecisionMetros = 25000 (la pantalla la muestra como "aproximada por IP").
const PRECISION_IP = 25000;
const _geoIp = new Map();
function ubicacionPorIp(ip) {
    if (!ip || esPrivada(ip) || process.env.AUDITORIA_UBICACION_IP === '0') return Promise.resolve(null);
    const c = _geoIp.get(ip);
    if (c && c.vence > Date.now()) return c.promesa;
    const promesa = conLimite(fetch(`https://ipapi.co/${encodeURIComponent(ip)}/json/`, { signal: AbortSignal.timeout(3000), headers: { 'User-Agent': 'MiventSoftWeb' } })
        .then((r) => (r.ok ? r.json() : null))
        .then((j) => (j && Number.isFinite(j.latitude) && Number.isFinite(j.longitude) && !j.error
            ? { latitud: coord(j.latitude), longitud: coord(j.longitude), precision: PRECISION_IP } : null)), 3500);
    _geoIp.set(ip, { promesa, vence: Date.now() + 24 * 3600 * 1000 });
    promesa.then((v) => { if (!v) _geoIp.set(ip, { promesa, vence: Date.now() + 600 * 1000 }); }); // si fallo, reintentar en 10 min
    if (_geoIp.size > 500) _geoIp.delete(_geoIp.keys().next().value);
    return promesa;
}

// Coordenada como TEXTO exacto, hasta 18 decimales (sql/12: decimal(21,18)); no pasa por Number para no perder digitos
const coord = (v) => {
    const t = typeof v === 'number' ? (Number.isFinite(v) ? (/e/i.test(String(v)) ? v.toFixed(18) : String(v)) : '') : String(v || '');
    const m = /^(-?)(\d{1,3})(?:\.(\d+))?$/.exec(t.trim());
    if (!m) return null;
    const dec = (m[3] || '').slice(0, 18).replace(/0+$/, '');
    return `${m[1]}${Number(m[2])}${dec ? `.${dec}` : ''}`.replace(/^-0$/, '0');
};

function ubicacionDe(v) {
    const m = /^\s*(-?\d{1,3}(?:\.\d+)?)\s*,\s*(-?\d{1,3}(?:\.\d+)?)(?:\s*,\s*(\d+(?:\.\d+)?))?\s*$/.exec(String(v || ''));
    if (!m) return {};
    if (Math.abs(Number(m[1])) > 90 || Math.abs(Number(m[2])) > 180) return {};
    return { latitud: coord(m[1]), longitud: coord(m[2]), precision: m[3] ? Math.min(Math.round(Number(m[3])), 2e9) : null };
}

async function clienteAuditoria(req, res, next) {
    const ipLocal = ipCliente(req);
    if (req.method !== 'PUT' && req.method !== 'DELETE') {
        // cualquier pedido: se averigua en segundo plano el nombre del equipo y la IP publica (listos al modificar)
        if (ipLocal) {
            nombrePc(ipLocal).catch(() => {});
            (esPrivada(ipLocal) ? ipPublicaServidor() : Promise.resolve(ipLocal)).then(ubicacionPorIp).catch(() => {});
        }
        return next();
    }
    try {
        const ip = esLocal(ipLocal) ? '127.0.0.1' : ipLocal;
        const [pc, publica] = await Promise.all([conLimite(nombrePc(ipLocal), 3000), esPrivada(ipLocal) ? ipPublicaServidor() : Promise.resolve(null)]);
        const nav = navegadorDe(req.get('User-Agent'));
        const modelo = (/Android [\d.]+ · (.+)$/.exec(nav || '') || [])[1]; // celular: su modelo
        const etiqueta = texto(req.get('X-Equipo-Nombre'), 60); // nombre opcional que se le dio en "Nombre de este equipo"
        const real = pc || modelo || null;
        const id = texto(req.get('X-Equipo-Id'), 12).replace(/[^A-Z0-9]/gi, '').toUpperCase();
        // "DESKTOP-49S3GJO" ; con nombre dado: "CAJA 1 (DESKTOP-49S3GJO)" ; sin nada: "Equipo 6CF254"
        const equipo = real ? (etiqueta && etiqueta !== real ? `${etiqueta} (${real})` : real) : (etiqueta || (id ? `Equipo ${id}` : null));
        // IP: "local | publica" (la publica si el equipo esta en la red local); si ya es publica, solo esa
        const ips = publica && publica !== ip ? `${ip} | ${publica}` : ip;
        // GPS del equipo; si no hay, la aproximada por la IP publica
        let ubic = ubicacionDe(req.get('X-Ubicacion'));
        if (ubic.latitud == null) ubic = (await ubicacionPorIp(publica || (esPrivada(ip) ? null : ip))) || {};
        req.clienteAud = { ip: ips ? ips.slice(0, 64) : null, navegador: nav, equipo: equipo ? equipo.slice(0, 128) : null, ...ubic };
    } catch {
        req.clienteAud = null; // la auditoria nunca impide la operacion
    }
    next();
}

module.exports = { clienteAuditoria, navegadorDe, ubicacionDe, PRECISION_IP };
