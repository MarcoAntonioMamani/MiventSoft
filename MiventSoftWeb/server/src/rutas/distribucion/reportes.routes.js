// =====================================================================
//  Reportes del panel Distribucion (cada uno con SU programa, como el menu del escritorio)
//   gastos-chofer           Rep_Gastos                        btnCierreReporteGastos     MAM_CierreCaja 10 (+ libreria 14)
//   ingresos-egresos-anual  Tec_ReporteIngresosEgresosMensuales btnCierreReporteAnual     MAM_CajaIngresoEgreso 9 (x 12 meses, libreria 13 = años)
//   ingresos-egresos-det    Tec_ReporteIngresosEgresosDetallado btnCierreReporteDetallado MAM_CajaIngresoEgreso 12
//   historico-clientes      Rep_Modificaciones                btnhistoricoCliente        MAM_Clientes 13 (JSON antes / despues)
//   productos-por-cliente   FProductosPorClientes             btnReporteVentasPorProductos MAM_Ventas 31 (+ MAM_Compras 12, MAM_Ventas 30, MAM_Clientes 14)
//   auditoria-ventas        Rep_AuditoriaVentas               btnRepAuditoriaVentas      MAM_AuditoriaVentas 1 / 2
//   historial-incremento    Rep_HistorialIncrementoVenta      btnhistoricoIncremento     MAM_VentasIncremento 3
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma, ProgramasPermitidos } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { rango, col, num, limpiar } = require('./comun');
const { libreria } = require('../configuracion/_libreria');

const router = express.Router();
const enviar = (res, dt) => res.json({ ok: true, data: limpiar(dt).map((f) => { const o = { ...f }; delete o.img; return o; }) });
const idOpc = (x) => { const n = Number.parseInt(x, 10); return Number.isFinite(n) && n > 0 ? n : -1; }; // -1 = TODOS (como VB)
const MESES = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];

// combos compartidos: personal (btnVendedor) / clientes / productos -> con cualquier programa de este grupo
const PROGS = ['BTNCIERREREPORTEGASTOS', 'BTNHISTORICOCLIENTE', 'BTNREPORTEVENTASPORPRODUCTOS'];
router.use('/aux', asyncHandler(async (req, res, next) => {
    const p = await ProgramasPermitidos(req.sesion.gi_userRol);
    if (!PROGS.some((x) => p.has(x))) return res.status(403).json({ ok: false, mensaje: 'No tiene privilegios para este programa.' });
    next();
}));
router.get('/aux/personal', asyncHandler(async (req, res) => res.json({ ok: true, data: v.proyectar(await L.ListarPersonal(), ['Id', 'Nombre']) })));

// ---------- Rep_Gastos ----------
router.get('/gastos-chofer/combos', requierePrograma('btnCierreReporteGastos'), asyncHandler(async (req, res) => {
    res.json({ ok: true, data: { tiposGasto: await libreria(14) } });
}));
router.get('/gastos-chofer', requierePrograma('btnCierreReporteGastos'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const gasto = idOpc(req.query.gasto);
    const personal = idOpc(req.query.personal);
    let dt = await L.ReporteGastosFecha(d, h);
    if (gasto > 0) dt = dt.filter((f) => num(col(f, 'TipoGastoId')) === gasto);
    if (personal > 0) dt = dt.filter((f) => num(col(f, 'PersonalId')) === personal);
    enviar(res, dt);
}));

// ---------- Ingresos / Egresos anual: Ingreso y Egreso por mes ----------
router.get('/ingresos-egresos-anual/anios', requierePrograma('btnCierreReporteAnual'), asyncHandler(async (req, res) => {
    res.json({ ok: true, data: await libreria(13) });
}));
router.get('/ingresos-egresos-anual', requierePrograma('btnCierreReporteAnual'), asyncHandler(async (req, res) => {
    const anio = Number.parseInt(req.query.anio, 10);
    if (!(anio >= 2000 && anio <= 2100)) throw new ErrorNegocio('Seleccione un Año');
    const meses = await Promise.all(MESES.map((_, i) => L.ReporteIngresosEgresosMes(anio, i + 1)));
    const suma = (dt, ie) => dt.filter((f) => num(col(f, 'IngresoEgreso')) === ie).reduce((s, f) => s + num(col(f, 'Monto')), 0);
    const ing = meses.map((dt) => suma(dt, 1));
    const egr = meses.map((dt) => suma(dt, 0));
    const fila = (Concepto, vals) => ({ Concepto, ...Object.fromEntries(MESES.map((m, i) => [m, Math.round(vals[i] * 100) / 100])), Total: Math.round(vals.reduce((a, b) => a + b, 0) * 100) / 100 });
    res.json({ ok: true, data: { filas: [fila('INGRESOS', ing), fila('EGRESOS', egr), fila('SALDO', ing.map((x, i) => x - egr[i]))], grafico: MESES.map((m, i) => ({ mes: m, ingreso: ing[i], egreso: egr[i] })) } });
}));

router.get('/ingresos-egresos-detallado', requierePrograma('btnCierreReporteDetallado'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    enviar(res, await L.ReporteIngresoEgresoDetallado(d, h));
}));

// ---------- Rep_Modificaciones: que campos cambiaron (ValorAnterior / ValorNuevo en JSON) ----------
function primerObjeto(txt) {
    if (txt === null || txt === undefined || String(txt).trim() === '') return {};
    try { const j = JSON.parse(String(txt)); const o = Array.isArray(j) ? j[0] : j; return o && typeof o === 'object' ? o : {}; } catch { return {}; }
}
const textoValor = (x) => (Array.isArray(x) ? x.map((i) => (i && typeof i === 'object' ? Object.entries(i).map(([k, y]) => `${k}: ${y}`).join(', ') : String(i))).join(' | ') : x && typeof x === 'object' ? JSON.stringify(x) : String(x ?? ''));
router.get('/historico-clientes', requierePrograma('btnhistoricoCliente'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const dt = await L.L_prListarHistorico(idOpc(req.query.personal), d, h);
    const filas = limpiar(dt).map((f) => {
        const ant = primerObjeto(col(f, 'ValorAnterior'));
        const nue = primerObjeto(col(f, 'ValorNuevo'));
        const cambios = Object.keys(nue).filter((k) => JSON.stringify(ant[k]) !== JSON.stringify(nue[k]))
            .map((k) => ({ campo: k, antes: k in ant ? textoValor(ant[k]) : '', despues: textoValor(nue[k]) }));
        const o = { Id: col(f, 'Id'), Fecha: col(f, 'Fecha'), Accion: col(f, 'Accion'), NombrePersonal: col(f, 'NombrePersonal'), diferencia: cambios.map((c) => `${c.campo}: ${c.despues}`).join('; '), cambios };
        const lat = nue.Latitud ?? nue.latitud; const lon = nue.Longitud ?? nue.longitud;
        if (lat !== undefined && lon !== undefined) o.ubicacion = { lat: Number(lat), lon: Number(lon) };
        return o;
    });
    res.json({ ok: true, data: filas });
}));

// ---------- FProductosPorClientes ----------
router.get('/productos-por-cliente/combos', requierePrograma('btnReporteVentasPorProductos'), asyncHandler(async (req, res) => {
    const [prov, cat, prod, cli] = await Promise.all([L.ListarProveedoresCombo(), L.ListarCategoriasVentas(), L.ListarProductosBuscador(), L.ListarCliente()]);
    const dos = (dt) => { const ks = L._columnasDe(dt); return dt.map((f) => ({ Id: f[ks[0]], Nombre: f[ks[1]] })); };
    res.json({ ok: true, data: { proveedores: dos(prov), categorias: dos(cat), productos: dos(prod), clientes: v.proyectar(cli, ['Id', 'NombreCliente']) } });
}));
router.get('/productos-por-cliente', requierePrograma('btnReporteVentasPorProductos'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const q = req.query;
    enviar(res, await L.L_prGenerarReporteProductosPorVentas(idOpc(q.proveedor), idOpc(q.categoria), idOpc(q.cliente), d, h, idOpc(q.producto)));
}));

// ---------- Rep_AuditoriaVentas ----------
const EVENTOS = ['TODOS', 'ANULADO', 'ELIMINADO', 'MODIFICADO'];
router.get('/auditoria-ventas', requierePrograma('btnRepAuditoriaVentas'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const ev = EVENTOS.includes(String(req.query.evento)) ? String(req.query.evento) : 'TODOS';
    enviar(res, await L.L_prListarAuditoriaVentas(d, h, ev));
}));
router.get('/auditoria-ventas/:id/detalle', requierePrograma('btnRepAuditoriaVentas'), asyncHandler(async (req, res) => {
    enviar(res, await L.L_prListarAuditoriaVentasDetalle(v.idRuta(req.params.id)));
}));

// ---------- Rep_HistorialIncrementoVenta ----------
router.get('/historial-incremento', requierePrograma('btnhistoricoIncremento'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    enviar(res, v.proyectar(await L.L_prListarHistorialIncrementoVenta(d, h), ['Id', 'Fecha', 'Monto', 'Usuario', 'FechaHoraModificacion']));
}));

module.exports = router;
