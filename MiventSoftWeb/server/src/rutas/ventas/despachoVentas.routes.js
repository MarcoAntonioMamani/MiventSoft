// =====================================================================
//  KAILY LA PAZ: FrmDespacho.vb (programa: btnDespachoVenta, modulo Ventas) -> "Despacho Ventas"
//   Lista       la de Ventas: L_prListarVentasGeneralFiltroFecha (MAM_Ventas @tipo 3) con
//               EstadoDespacho, FechaDespacho, Despachante, PersonalDespachadorId, ObservacionDespacho.
//               Si el rol no es 1 y el usuario no tiene VerTodasVentas, solo sus ventas.
//   Detalle     ListaVentasDetalles (@tipo 4)      Despachador  ListarPersonal (MAM_Clientes @tipo 9)
//   Grabar      VentaInsertarDespacho (MAM_Ventas @tipo 11): Entregado = inserta el despacho si la venta
//               no tiene; No entregado = lo borra. Si ya estaba entregado el SP no cambia despachador ni observacion.
//   Nota        P_GenerarReporte "NOTA DE DESPACHO" (ListarVentaRecibo @tipo 6): solo si esta entregado
//  Reglas del escritorio: sin Nuevo ni Eliminar (botones ocultos); "Editar" no se permite en ventas de dias
//  anteriores sin ModificarRegistroAnteriores; "Debe Seleccionar un Despachador".
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const ventas = require('./ventasPuntoVenta.routes');

const I = ventas._interno; // misma lista, busqueda y nota que Tec_Ventas (MAM_Ventas)
const router = express.Router();
router.use(requierePrograma('btnDespachoVenta'));

const MAX_DIAS_LISTA = 370;
const MAX_OBSERVACION = 400; // @ObservacionDespacho nvarchar(400)

// isnull(desp.FechaDespacho,'') llega como 1900-01-01 cuando no hay despacho
const _fila = (f) => {
    const entregado = Number(f.EstadoDespacho) === 1;
    return {
        ...f,
        EstadoDespacho: entregado ? 1 : 0,
        FechaDespacho: entregado ? I.isoDe(f.FechaDespacho) : null,
        PersonalDespachadorId: Number(f.PersonalDespachadorId) || 0,
        Despachante: f.Despachante ?? '',
        ObservacionDespacho: f.ObservacionDespacho ?? '',
    };
};

// _PMOGetTablaBuscador
router.get('/', asyncHandler(async (req, res) => {
    const desde = I.fechaReq(req.query.desde || I.hoyBolivia(), 'Desde');
    const hasta = I.fechaReq(req.query.hasta || I.hoyBolivia(), 'Hasta');
    if (desde > hasta) throw new ErrorNegocio('La Fecha Desde Debe Ser Menor Que la Fecha Hasta');
    if ((Date.parse(hasta) - Date.parse(desde)) / 86400000 > MAX_DIAS_LISTA) throw new ErrorNegocio('El rango maximo es de un año');
    res.json({ ok: true, data: (await I._lista(I.globales(req.sesion), desde, hasta)).map(_fila) });
}));

// permisos que usa la pantalla (EditarToolStripMenuItem)
router.get('/inicio', asyncHandler(async (req, res) => {
    const g = I.globales(req.sesion);
    res.json({ ok: true, data: { hoy: I.hoyBolivia(), modificarAnteriores: g.modificarAnteriores } });
}));

// ButtonX1: "Seleccione Despachador" (ListarPersonal)
router.get('/personal', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.ListarPersonal(), ['Id', 'Nombre', 'Direccion', 'Telefono01']) });
}));

const CAMPOS_DETALLE = ['Id', 'ProductoId', 'Producto', 'Cantidad', 'Precio', 'SubTotal', 'ProcentajeDescuento', 'MontoDescuento', 'Total', 'Tipo', 'TipoNombre', 'KitId', 'KitNombre', 'CantidadKit'];
router.get('/:id/detalle', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    res.json({ ok: true, data: v.proyectar(await L.ListaVentasDetalles(id), CAMPOS_DETALLE) });
}));

// btnModificarEntregado_Click
router.put('/:id', asyncHandler(async (req, res) => {
    const g = I.globales(req.sesion);
    const id = v.idRuta(req.params.id);
    const b = req.body || {};
    const venta = _fila(await I._buscarVenta(g, id, I.fechaReq(b.fecha, 'Fecha')));
    if (!g.modificarAnteriores && venta.FechaVenta < I.hoyBolivia()) {
        throw new ErrorNegocio('No puede Modificar Despachos Anteriores a la fecha de Hoy, Consulte con su Administrador'.toUpperCase());
    }
    const PersonalId = Number.parseInt(b.PersonalId, 10);
    if (!(PersonalId > 0)) throw new ErrorNegocio('Debe Seleccionar un Despachador'.toUpperCase());
    if (!v.proyectar(await L.ListarPersonal(), ['Id']).some((p) => Number(p.Id) === PersonalId)) {
        throw new ErrorNegocio('El Despachador no existe. Actualice la lista e intente de nuevo.');
    }
    const Despachado = v.bit(b.Despachado);
    const r = await L.VentaInsertarDespacho(id, v.texto(b.Observacion, MAX_OBSERVACION), PersonalId, Despachado);
    if (!r.resultado) throw new ErrorNegocio('Hubo un Error al guardar el despacho'.toUpperCase());
    res.json({ ok: true, id, fecha: venta.FechaVenta, mensaje: 'Despacho Grabado con Exito.'.toUpperCase() });
}));

// BtnImprimir: NOTA DE DESPACHO, solo con el despacho entregado
router.get('/:id/recibo', asyncHandler(async (req, res) => {
    const g = I.globales(req.sesion);
    const id = v.idRuta(req.params.id);
    const venta = _fila(await I._buscarVenta(g, id, I.fechaReq(req.query.fecha, 'Fecha')));
    if (venta.EstadoDespacho !== 1) throw new ErrorNegocio('No se puede imprimir por que el despacho no esta entregado'.toUpperCase());
    res.json({ ok: true, data: await I._recibo(id, venta) });
}));

module.exports = router;
