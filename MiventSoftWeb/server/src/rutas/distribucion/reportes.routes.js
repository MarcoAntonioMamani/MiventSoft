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
const { rango, col, num, limpiar, fechaIso } = require('./comun');
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
// btnAnio (Efecto 10, libreria 13 "Crear Gestion(Año)"): solo años de 4 digitos, sin repetir
router.post('/ingresos-egresos-anual/anios', requierePrograma('btnCierreReporteAnual'), asyncHandler(async (req, res) => {
    const anio = String(req.body?.descripcion ?? '').trim();
    if (!/^\d{4}$/.test(anio) || +anio < 2000 || +anio > 2100) throw new ErrorNegocio('Ingrese una gestión válida (año de 4 dígitos, ej. 2026)');
    if ((await libreria(13)).some((x) => String(x.cndesc1).trim() === anio)) throw new ErrorNegocio(`La gestión ${anio} ya existe.`);
    const r = await L.L_prClasificadorGrabar('', 13, anio);
    if (!r.resultado) throw new ErrorNegocio('No se pudo registrar la gestión.');
    res.json({ ok: true, id: r._numi, data: await libreria(13), mensaje: `GESTION ${anio} REGISTRADA CON EXITO.` });
}));
// cabecera de la hoja impresa (empresa y logo, mismos SP de solo lectura que los demas reportes)
async function cabeceraImpresa() {
    const [empresas, imagen] = await Promise.all([L.L_prListarGeneral('MAM_Empresas').catch(() => []), L.ObtenerImagenEmpresa().catch(() => [])]);
    return { empresa: v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Ciudad'])[0] || null, imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '' };
}
router.get('/ingresos-egresos-anual', requierePrograma('btnCierreReporteAnual'), asyncHandler(async (req, res) => {
    const anio = Number.parseInt(req.query.anio, 10);
    if (!(anio >= 2000 && anio <= 2100)) throw new ErrorNegocio('Seleccione un Año');
    const [meses, cabecera] = await Promise.all([Promise.all(MESES.map((_, i) => L.ReporteIngresosEgresosMes(anio, i + 1))), cabeceraImpresa()]);
    const suma = (dt, ie) => dt.filter((f) => num(col(f, 'IngresoEgreso')) === ie).reduce((s, f) => s + num(col(f, 'Monto')), 0);
    const ing = meses.map((dt) => suma(dt, 1));
    const egr = meses.map((dt) => Math.abs(suma(dt, 0))); // MAM_CajaIngresoEgreso 9 trae los egresos en negativo (VB: Egreso * -1)
    const fila = (Concepto, vals) => ({ Concepto, ...Object.fromEntries(MESES.map((m, i) => [m, Math.round(vals[i] * 100) / 100])), Total: Math.round(vals.reduce((a, b) => a + b, 0) * 100) / 100 });
    res.json({ ok: true, data: { filas: [fila('INGRESOS', ing), fila('EGRESOS', egr), fila('SALDO', ing.map((x, i) => x - egr[i]))], grafico: MESES.map((m, i) => ({ mes: m, ingreso: ing[i], egreso: egr[i] })), cabecera } });
}));

router.get('/ingresos-egresos-detallado', requierePrograma('btnCierreReporteDetallado'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    // MAM_CajaIngresoEgreso 12: Modulo (tipo de ingreso/egreso), NroComprobante (Id del registro), Fecha, Monto, IngresoEgreso (1/0).
    // Los egresos vienen en negativo: se envian en positivo y el balance es ingresos - egresos.
    // Orden: primero ingresos, luego egresos; dentro, por tipo, fecha y comprobante.
    const [dt, cabecera] = await Promise.all([L.ReporteIngresoEgresoDetallado(d, h), cabeceraImpresa()]);
    const filas = limpiar(dt).map((f) => {
        const ing = num(col(f, 'IngresoEgreso')) === 1;
        return { Grupo: ing ? 'INGRESOS' : 'EGRESOS', Ingreso: ing, Modulo: String(col(f, 'Modulo') ?? '').trim() || '(Sin tipo)', NroComprobante: col(f, 'NroComprobante'), Fecha: col(f, 'Fecha'), Monto: Math.abs(num(col(f, 'Monto'))) };
    });
    const k = (f) => String(f.Fecha ?? '');
    filas.sort((a, b) => (b.Ingreso - a.Ingreso) || a.Modulo.localeCompare(b.Modulo, 'es') || k(a).localeCompare(k(b)) || num(a.NroComprobante) - num(b.NroComprobante));
    res.json({ ok: true, data: { filas, cabecera } });
}));

// ---------- Rep_Modificaciones: que campos cambiaron (ValorAnterior / ValorNuevo en JSON) ----------
function primerObjeto(txt) {
    if (txt === null || txt === undefined || String(txt).trim() === '') return {};
    try { const j = JSON.parse(String(txt)); const o = Array.isArray(j) ? j[0] : j; return o && typeof o === 'object' ? o : {}; } catch { return {}; }
}
const textoValor = (x) => (Array.isArray(x) ? x.map((i) => (i && typeof i === 'object' ? Object.entries(i).map(([k, y]) => `${k}: ${y}`).join(', ') : String(i))).join(' | ') : x && typeof x === 'object' ? JSON.stringify(x) : String(x ?? ''));
// Latitud / Longitud validas (numeros, en rango y distintas de 0,0) -> { lat, lon }; si no, null
function punto(o) {
    const lat = Number(col(o, 'Latitud')); const lon = Number(col(o, 'Longitud'));
    if (!Number.isFinite(lat) || !Number.isFinite(lon) || (lat === 0 && lon === 0) || Math.abs(lat) > 90 || Math.abs(lon) > 180) return null;
    if (col(o, 'Latitud') === null || col(o, 'Latitud') === '' || col(o, 'Longitud') === null || col(o, 'Longitud') === '') return null;
    return { lat, lon };
}
router.get('/historico-clientes', requierePrograma('btnhistoricoCliente'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    // MAM_Clientes 13 filtra "idPersonal = @idVendedor" (sin opcion TODOS) y @fechaF es date contra Fecha datetime
    // (excluye el dia Hasta). Sin cambiar el SP: TODOS = una llamada por personal, y se pide hasta el dia siguiente
    // para luego quedarse solo con lo que cae hasta el final del dia Hasta.
    const sig = new Date(`${h.replace(/\//g, '-')}T00:00:00Z`); sig.setUTCDate(sig.getUTCDate() + 1);
    const hSig = sig.toISOString().slice(0, 10).replace(/-/g, '/');
    const hIso = h.replace(/\//g, '-');
    const sel = idOpc(req.query.personal);
    const ids = sel > 0 ? [sel] : [...new Set((await L.ListarPersonal()).map((p) => num(col(p, 'Id'))).filter((x) => x > 0))];
    const dt = [];
    for (let i = 0; i < ids.length; i += 6) { // de a 6 llamadas en paralelo para no saturar la conexion
        (await Promise.all(ids.slice(i, i + 6).map((id) => L.L_prListarHistorico(id, d, hSig)))).forEach((x) => dt.push(...x));
    }
    const enRango = dt.filter((f) => (fechaIso(col(f, 'Fecha')) || '') <= hIso);
    enRango.sort((a, b) => (new Date(col(a, 'Fecha')) - new Date(col(b, 'Fecha'))) || num(col(a, 'Id')) - num(col(b, 'Id')));
    const filas = limpiar(enRango).map((f) => {
        const ant = primerObjeto(col(f, 'ValorAnterior'));
        const nue = primerObjeto(col(f, 'ValorNuevo'));
        const cambios = Object.keys(nue).filter((k) => JSON.stringify(ant[k]) !== JSON.stringify(nue[k]))
            .map((k) => ({ campo: k, antes: k in ant ? textoValor(ant[k]) : '', despues: textoValor(nue[k]) }));
        const o = { Id: col(f, 'Id'), Fecha: col(f, 'Fecha'), Accion: col(f, 'Accion'), NombrePersonal: col(f, 'NombrePersonal'), diferencia: cambios.map((c) => `${c.campo}: ${c.despues}`).join('; '), cambios };
        // Gmc_ClienteAnterior / Gmc_ClienteActual: ubicacion antes y despues, solo si la latitud y longitud son validas
        // (nuevo: no hay "antes"; modificacion: donde estaba y donde esta hoy)
        o.nuevo = Object.keys(ant).length === 0; // sin ValorAnterior: registro nuevo
        o.ubicacionAntes = punto(ant);
        o.ubicacion = punto(nue);
        return o;
    });
    res.json({ ok: true, data: filas });
}));

// ---------- FProductosPorClientes ----------
router.get('/productos-por-cliente/combos', requierePrograma('btnReporteVentasPorProductos'), asyncHandler(async (req, res) => {
    const [prov, cat, prod, cli] = await Promise.all([L.ListarProveedoresCombo(), L.ListarCategoriasVentas(), L.ListarProductosBuscador(), L.ListarCliente()]);
    const dos = (dt) => { const ks = L._columnasDe(dt); return dt.map((f) => ({ Id: f[ks[0]], Nombre: f[ks[1]] })); };
    // MAM_Clientes 14: Id, CodigoExterno, NombreProducto, NombreCategoria (VB btnProducto_Click)
    const productos = prod.map((f) => {
        const cod = String(col(f, 'CodigoExterno') ?? '').trim();
        const nom = String(col(f, 'NombreProducto') ?? '').trim();
        return { Id: col(f, 'Id'), Nombre: cod ? `${nom} (${cod})` : nom };
    });
    // MAM_Clientes 10: el nombre del cliente viene en la columna NombreProveedor (VB btnClientes_Click)
    const clientes = cli.map((f) => ({ Id: col(f, 'Id'), NombreCliente: col(f, 'NombreCliente') ?? col(f, 'NombreProveedor') }));
    res.json({ ok: true, data: { proveedores: dos(prov), categorias: dos(cat), productos, clientes } });
}));
router.get('/productos-por-cliente', requierePrograma('btnReporteVentasPorProductos'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const q = req.query;
    const [dt, cabecera] = await Promise.all([L.L_prGenerarReporteProductosPorVentas(idOpc(q.proveedor), idOpc(q.categoria), idOpc(q.cliente), d, h, idOpc(q.producto)), cabeceraImpresa()]);
    // Cliente > Categoria > Producto (el cliente agrupa y ordena; aqui solo se envian las filas del SP)
    res.json({ ok: true, data: { filas: limpiar(dt), cabecera } });
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
