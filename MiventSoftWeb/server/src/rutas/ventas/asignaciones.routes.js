// =====================================================================
//  Tec_AdministrarAsignacionesPedidos.vb  (programa: btnAsignacionPedidos)  SP: MAM_Ventas
//   Combos: ListarSoloChoferes (MAM_Clientes 12)  ListarConciliacion (MAM_Ventas 22)
//   Pendientes  @tipo 7   -> Asignar (8, @PersonalId)  /  Anular (13)
//   Asignados   @tipo 9   -> Quitar (12) / Anular (13) / Entregar (14, @ChoferId)
//                            Lista de carga: productos (10) / clientes (11)
//   Entregados  @tipo 15  -> Revertir a Asignado (16, bloqueado si la conciliacion esta cerrada)
//   Anulados    @tipo 17  -> Revertir a Pendiente (18)
//   Detalle del pedido (Efecto 17 -> Tec_VisualizarDetallePedido): MAM_Ventas 4
//
//  Igual que VB, cada accion envia la grilla COMPLETA al SP con la columna Asignar marcada.
//  Mejora: el servidor vuelve a leer la lista y marca solo los Id elegidos (no confia en
//  filas del navegador); si un pedido elegido ya cambio de estado, avisa en vez de grabar.
//  Permisos: Asignar = Insertar; Quitar / Entregar / Revertir = Modificar; Anular = Eliminar.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma, accionPermiso } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const PROGRAMA = 'btnAsignacionPedidos';
const router = express.Router();
const ver = requierePrograma(PROGRAMA);
const con = (accion) => [accionPermiso(accion), requierePrograma(PROGRAMA)];

const dos = (n) => String(n).padStart(2, '0');
function fechaTxt(x) {
    if (!(x instanceof Date)) return x;
    const d = `${x.getUTCFullYear()}-${dos(x.getUTCMonth() + 1)}-${dos(x.getUTCDate())}`;
    return x.getUTCHours() || x.getUTCMinutes() ? `${d} ${dos(x.getUTCHours())}:${dos(x.getUTCMinutes())}` : d;
}
// Para el navegador: sin binarios (detalle / img), fechas legibles
const limpiar = (dt) => dt.map((f) => Object.fromEntries(Object.entries(f).filter(([, x]) => !Buffer.isBuffer(x)).map(([k, x]) => [k, fechaTxt(x)])));
const clave = (dt, nombre) => L._columnasDe(dt).find((c) => c.toLowerCase() === nombre.toLowerCase());

const CAMPOS = ['Id', 'FechaPedido', 'NombreCliente', 'NombrePersonal', 'totalPedido'];
const CAMPOS_ENT = [...CAMPOS, 'conciliacion', 'ConciliacionId', 'EstadoConciliacion'];
const CAMPOS_ANU = [...CAMPOS, 'FechaAnulacion'];
const lista = (dt, campos) => limpiar(v.proyectar(dt, campos));

const idChofer = (x) => {
    const n = Number.parseInt(x, 10);
    if (!Number.isFinite(n) || n <= 0) throw new ErrorNegocio('Seleccione un Chofer');
    return n;
};
const idEntero = (x) => { const n = Number.parseInt(x, 10); return Number.isFinite(n) ? n : 0; };

async function choferDe(id) {
    const c = (await L.ListarSoloChoferes()).find((x) => Number(x.Id ?? x.id) === id);
    if (!c) throw new ErrorNegocio('Seleccione un Chofer');
    return String(c.Nombre ?? c.nombre ?? '');
}

/**
 * Arma la tabla para el SP: la lista vigente (columnas en el orden del SELECT) con Asignar
 * = 1 solo en los pedidos elegidos. Devuelve { tabla, elegidas } (filas crudas elegidas).
 */
function marcar(dt, ids) {
    const sel = new Set((Array.isArray(ids) ? ids : []).map((x) => Number.parseInt(x, 10)).filter((x) => x > 0));
    if (!sel.size) throw new ErrorNegocio('No Existe Ningun Pedido Seleccionado');
    if (sel.size > 2000) throw new ErrorNegocio('Demasiados pedidos seleccionados');
    const columnas = L._columnasDe(dt);
    const kId = clave(dt, 'Id');
    const kAsig = clave(dt, 'Asignar');
    if (!dt.length || !kId || !kAsig) throw new ErrorNegocio('Los pedidos seleccionados ya cambiaron de estado. Actualice la lista.');
    const filas = dt.map((f) => ({ ...f, [kAsig]: sel.has(Number(f[kId])) }));
    const elegidas = dt.filter((f) => sel.has(Number(f[kId])));
    if (elegidas.length !== sel.size) throw new ErrorNegocio('Algunos pedidos seleccionados ya cambiaron de estado. Actualice la lista.');
    return { tabla: { columnas, filas }, elegidas };
}
const ids = (b) => (b && b.ids) || [];

// ---------------- lecturas ----------------
router.get('/inicio', ver, asyncHandler(async (req, res) => {
    const [ch, co] = await Promise.all([L.ListarSoloChoferes(), L.ListarConciliacion()]);
    res.json({ ok: true, data: { choferes: v.proyectar(ch, ['Id', 'Nombre']), conciliaciones: v.proyectar(co, ['id', 'conciliacion']) } });
}));
router.get('/pendientes', ver, asyncHandler(async (req, res) => {
    res.json({ ok: true, data: lista(await L.ListaPedidosPendientesAsignaciones(), CAMPOS) });
}));
router.get('/asignados/:chofer', ver, asyncHandler(async (req, res) => {
    res.json({ ok: true, data: lista(await L.ListaPedidosAsignadosByChofer(idChofer(req.params.chofer)), CAMPOS) });
}));
router.get('/entregados/:chofer', ver, asyncHandler(async (req, res) => {
    res.json({ ok: true, data: lista(await L.ListaPedidosEntregadosByChofer(idChofer(req.params.chofer), idEntero(req.query.conciliacion)), CAMPOS_ENT) });
}));
router.get('/anulados', ver, asyncHandler(async (req, res) => {
    res.json({ ok: true, data: lista(await L.ListaPedidosAnulados(), CAMPOS_ANU) });
}));
// Lista de carga del chofer (Reporte_ProductosAsignados / clientes asignados)
router.get('/carga/:chofer', ver, asyncHandler(async (req, res) => {
    const chofer = idChofer(req.params.chofer);
    const fn = req.query.de === 'clientes' ? L.ListaClientesAsignadosByChofer : L.ListaProductosAsignadosByChofer;
    const [dt, emp, nombre] = await Promise.all([fn(chofer), L.ObtenerImagenEmpresa(), choferDe(chofer)]);
    const e = emp[0] || {};
    res.json({ ok: true, data: { filas: limpiar(dt).map((f) => { const o = { ...f }; delete o.img; return o; }), chofer: nombre, empresa: { nombre: e.Nombre ?? '', direccion: e.Direccion ?? '' } } });
}));
// Detalle del pedido (sin costo)
router.get('/pedido/:id/detalle', ver, asyncHandler(async (req, res) => {
    const dt = await L.ListaVentasDetalles(v.idRuta(req.params.id));
    res.json({ ok: true, data: v.proyectar(dt, ['Id', 'ProductoId', 'Producto', 'Cantidad', 'Precio', 'SubTotal', 'ProcentajeDescuento', 'MontoDescuento', 'Total']) });
}));

// ---------------- acciones ----------------
const fallo = (res, m = 'Error no se Pudo completar el proceso') => res.status(400).json({ ok: false, mensaje: m });

router.post('/asignar', ...con('insertar'), asyncHandler(async (req, res) => {
    const { tabla } = marcar(await L.ListaPedidosPendientesAsignaciones(), ids(req.body));
    const chofer = idChofer(req.body.chofer);
    await choferDe(chofer);
    if (!(await L.VentaInsertarAsignaciones(tabla, chofer))) return fallo(res, 'Error no se Pudo Realizar las Asignaciones');
    res.json({ ok: true, mensaje: 'Asignacion Realizada Correctamente' });
}));

router.post('/quitar', ...con('modificar'), asyncHandler(async (req, res) => {
    const { tabla } = marcar(await L.ListaPedidosAsignadosByChofer(idChofer(req.body.chofer)), ids(req.body));
    if (!(await L.VentaQuitarAsignaciones(tabla))) return fallo(res);
    res.json({ ok: true, mensaje: 'Se han Quitado Las Asignaciones Correctamente' });
}));

router.post('/entregar', ...con('modificar'), asyncHandler(async (req, res) => {
    const chofer = idChofer(req.body.chofer);
    const { tabla } = marcar(await L.ListaPedidosAsignadosByChofer(chofer), ids(req.body));
    const r = await L.VentaEntregarPedidos(tabla, chofer, await choferDe(chofer));
    if (!r.resultado) return fallo(res, r.Mensaje);
    res.json({ ok: true, mensaje: 'Los Pedidos Ha Pasado Al Estado Entregado' });
}));

// Anular: desde Pendientes (ButtonX10) o desde Asignados (ButtonX9)
router.post('/anular', ...con('eliminar'), asyncHandler(async (req, res) => {
    const dt = req.body.desde === 'asignados'
        ? await L.ListaPedidosAsignadosByChofer(idChofer(req.body.chofer))
        : await L.ListaPedidosPendientesAsignaciones();
    const { tabla } = marcar(dt, ids(req.body));
    if (!(await L.VentaAnularPedidos(tabla))) return fallo(res);
    res.json({ ok: true, mensaje: 'Los Pedidos Han Sido Anulados Correctamente' });
}));

router.post('/revertir-entrega', ...con('modificar'), asyncHandler(async (req, res) => {
    const dt = await L.ListaPedidosEntregadosByChofer(idChofer(req.body.chofer), idEntero(req.body.conciliacion));
    const { tabla, elegidas } = marcar(dt, ids(req.body));
    const kEst = clave(dt, 'EstadoConciliacion');
    if (kEst && elegidas.some((f) => Number(f[kEst]) === 0)) {
        throw new ErrorNegocio('No Es Posible Revertir Estado Por Que Existen Pedidos Que Pertenece A una Conciliacion Cerrada');
    }
    if (!(await L.VentaRevertiEstadoEntregado(tabla))) return fallo(res);
    res.json({ ok: true, mensaje: 'Los Pedidos Han Sido Revertido Al Estado Asignado' });
}));

router.post('/revertir-anulacion', ...con('modificar'), asyncHandler(async (req, res) => {
    const { tabla } = marcar(await L.ListaPedidosAnulados(), ids(req.body));
    if (!(await L.VentaRevertirAnulacion(tabla))) return fallo(res);
    res.json({ ok: true, mensaje: 'Reversion Exitosa. Los Pedidos Han Pasado Al Estado Pendiente' });
}));

module.exports = router;
module.exports._interno = { marcar };
