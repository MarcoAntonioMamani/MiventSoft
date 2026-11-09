// Utilidades comunes del modulo Distribucion (fechas como VB .ToString("yyyy/MM/dd"))
const { ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const MAX_DIAS = 370;
const dos = (n) => String(n).padStart(2, '0');
const isoDe = (d) => `${d.getUTCFullYear()}-${dos(d.getUTCMonth() + 1)}-${dos(d.getUTCDate())}`;
const aVB = (iso) => iso.replace(/-/g, '/');

function fechaReq(txt, campo) {
    const s = v.texto(txt, 10);
    const m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(s);
    const d = m ? new Date(Date.UTC(+m[1], +m[2] - 1, +m[3])) : null;
    if (!d || Number.isNaN(d.getTime()) || isoDe(d) !== s) throw new ErrorNegocio(`Ingrese una Fecha Valida: ${campo}`);
    return s;
}
function hoyBolivia() { return isoDe(new Date(Date.now() - 4 * 3600 * 1000)); }
function rango(q) {
    const desde = fechaReq(q.desde || hoyBolivia(), 'Desde');
    const hasta = fechaReq(q.hasta || hoyBolivia(), 'Hasta');
    if (desde > hasta) throw new ErrorNegocio('La fecha Desde no puede ser mayor que Hasta');
    if ((Date.parse(hasta) - Date.parse(desde)) / 864e5 > MAX_DIAS) throw new ErrorNegocio('El rango maximo es de un año');
    return [aVB(desde), aVB(hasta)];
}
// valor de una columna sin distinguir mayusculas
const col = (f, nombre) => { const k = Object.keys(f).find((x) => x.toLowerCase() === nombre.toLowerCase()); return k === undefined ? undefined : f[k]; };
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const fechaIso = (x) => (x instanceof Date ? isoDe(x) : (/^(\d{4})[-/](\d{2})[-/](\d{2})/.exec(String(x ?? '')) || []).slice(1, 4).join('-') || null);
// Para el navegador: sin binarios, fechas legibles
const conHora = (d) => (d.getUTCHours() || d.getUTCMinutes() ? `${isoDe(d)} ${dos(d.getUTCHours())}:${dos(d.getUTCMinutes())}` : isoDe(d));
const limpiar = (dt) => dt.map((f) => Object.fromEntries(Object.entries(f).filter(([, x]) => !Buffer.isBuffer(x)).map(([k, x]) => [k, x instanceof Date ? conHora(x) : x])));

module.exports = { fechaReq, rango, aVB, col, num, fechaIso, limpiar, hoyBolivia };
