// Helpers de lectura/validacion del body (equivale a _PMOValidarCampos del lado servidor)
const { ErrorNegocio } = require('../middleware/errores');

function texto(v, max = 4000) {
    const s = v === undefined || v === null ? '' : String(v).trim();
    return s.length > max ? s.slice(0, max) : s;
}

function requerido(v, nombreCampo, max) {
    const s = texto(v, max);
    if (s === '') throw new ErrorNegocio(`Los Siguientes Campos Son Requeridos: ${nombreCampo}`);
    return s;
}

function entero(v, nombreCampo, { requeridoPositivo = false } = {}) {
    const n = Number.parseInt(v, 10);
    if (Number.isNaN(n)) {
        if (requeridoPositivo) throw new ErrorNegocio(`Seleccione ${nombreCampo}`);
        return 0;
    }
    if (requeridoPositivo && n <= 0) throw new ErrorNegocio(`Seleccione ${nombreCampo}`);
    return n;
}

function decimal(v) {
    const n = Number.parseFloat(v);
    return Number.isFinite(n) ? n : 0;
}

// IIf(sw.Value = True, 1, 0)
function bit(v) {
    return v === true || v === 1 || v === '1' || v === 'true' ? 1 : 0;
}

function idRuta(v) {
    const n = Number.parseInt(v, 10);
    if (Number.isNaN(n) || n <= 0) throw new ErrorNegocio('Codigo de registro no valido');
    return n;
}

module.exports = { texto, requerido, entero, decimal, bit, idRuta };

// ---------------------------------------------------------------------
//  proyectar: envia al navegador SOLO las columnas que la pantalla usa.
//  Busca la columna sin distinguir mayusculas (en los SP a veces es "Estado"
//  y a veces "estado") y la entrega con el nombre pedido.
//  campos: ['Id', 'NombreCliente', ...]  o  { alias: 'ColumnaSP' }
// ---------------------------------------------------------------------
// mssql junta columnas repetidas (ej. c.Id y z.Id en un JOIN) en un arreglo: se toma la primera
const unico = (x) => (Array.isArray(x) ? x[0] : x);
// Valor de una columna sin distinguir mayusculas (Id / ID / id)
function campo(f, nombre) {
    const k = Object.keys(f).find((x) => x.toLowerCase() === nombre.toLowerCase());
    return k === undefined ? undefined : unico(f[k]);
}
// Busca la fila cuyo Id (en cualquier mayuscula) es id
const buscarId = (dt, id, nombre = 'Id') => dt.find((f) => Number(campo(f, nombre)) === Number(id));

function proyectar(dt, campos) {
    const pares = Array.isArray(campos) ? campos.map((c) => [c, c]) : Object.entries(campos);
    if (!dt.length) return [];
    const mapa = {};
    for (const k of Object.keys(dt[0])) mapa[k.toLowerCase()] = k;
    const resueltos = pares.map(([alias, col]) => [alias, mapa[col.toLowerCase()]]);
    return dt.map((f) => {
        const o = {};
        for (const [alias, col] of resueltos) o[alias] = col === undefined ? null : unico(f[col]);
        return o;
    });
}

// Fecha a 'yyyy/MM/dd' (formato que usa el escritorio para FechaIngreso)
// - Sin valor: HOY en hora local (Bolivia), como Now.Date en VB.
// - Con valor que viene de SQL Server: mssql entrega DATE/DATETIME como UTC,
//   por eso se leen las partes UTC (si no, en UTC-4 la fecha retrocede un dia).
function fechaVB(v) {
    const dos = (n) => String(n).padStart(2, '0');
    if (!v) {
        const h = new Date();
        return `${h.getFullYear()}/${dos(h.getMonth() + 1)}/${dos(h.getDate())}`;
    }
    const d = new Date(v);
    if (Number.isNaN(d.getTime())) return fechaVB();
    return `${d.getUTCFullYear()}/${dos(d.getUTCMonth() + 1)}/${dos(d.getUTCDate())}`;
}

module.exports.proyectar = proyectar;
module.exports.campo = campo;
module.exports.buscarId = buscarId;
module.exports.fechaVB = fechaVB;
