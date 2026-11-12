// =====================================================================
//  Genera test/contrato_vb.json desde Negocio/AccesoLogica.vb de ESTA rama:
//  por cada D_ProcedimientoConParam: SP + @tipo -> parametros que envia el escritorio.
//  El simulador (MetodoDatos.mock.js) compara cada llamada de la web contra este archivo.
//  Uso:  node test/generar_contrato.js            (escribe el contrato)
//        node test/generar_contrato.js --comparar (solo muestra lo que cambiaria)
// =====================================================================
const fs = require('node:fs');
const path = require('node:path');

const VB = path.join(__dirname, '..', '..', '..', 'Negocio', 'AccesoLogica.vb');
const SALIDA = path.join(__dirname, 'contrato_vb.json');

function generar(texto) {
    const contrato = {};
    let funcion = null, params = [], tipo = null;
    for (const linea of texto.split(/\r?\n/)) {
        const sinComentario = linea.replace(/'.*$/, (m) => (m.split('"').length % 2 ? '' : m)); // respeta ' dentro de "..."
        const f = sinComentario.match(/^\s*Public\s+Shared\s+(?:Function|Sub)\s+(\w+)/i);
        if (f) { funcion = f[1]; params = []; tipo = null; continue; }
        if (!funcion) continue;
        if (/New\s+List\s*\(\s*Of\s+Datos\.DParametro/i.test(sinComentario)) { params = []; tipo = null; }
        const p = sinComentario.match(/DParametro\(\s*"(@\w+)"\s*,\s*([^,)]*)/i);
        if (p) {
            params.push(p[1].toLowerCase());
            if (p[1].toLowerCase() === '@tipo') tipo = p[2].trim();
        }
        const c = sinComentario.match(/D_ProcedimientoConParam\(\s*("?)(\w+)\1\s*,/i);
        if (c) {
            const sp = c[1] ? c[2] : `*${c[2]}`;
            const clave = `${sp}|${tipo}`;
            const entrada = { funcion, params: [...new Set(params)].sort() };
            const lista = (contrato[clave] = contrato[clave] || []);
            if (!lista.some((x) => x.params.join() === entrada.params.join())) lista.push(entrada);
        }
        if (/^\s*End\s+(Function|Sub)/i.test(sinComentario)) funcion = null;
    }
    return Object.fromEntries(Object.keys(contrato).sort().map((k) => [k, contrato[k]]));
}

const nuevo = generar(fs.readFileSync(VB, 'utf8'));
if (process.argv.includes('--comparar')) {
    const actual = fs.existsSync(SALIDA) ? JSON.parse(fs.readFileSync(SALIDA, 'utf8')) : {};
    const txt = (l) => (l || []).map((x) => `${x.funcion}: ${x.params.join(',')}`).sort().join('\n      ');
    for (const k of [...new Set([...Object.keys(actual), ...Object.keys(nuevo)])].sort()) {
        if (txt(actual[k]) === txt(nuevo[k])) continue;
        console.log(`${k}\n  antes: ${txt(actual[k]) || '-'}\n  ahora: ${txt(nuevo[k]) || '-'}`);
    }
} else {
    fs.writeFileSync(SALIDA, JSON.stringify(nuevo, null, 1));
    console.log(`contrato_vb.json: ${Object.keys(nuevo).length} combinaciones SP|@tipo desde ${VB}`);
}
