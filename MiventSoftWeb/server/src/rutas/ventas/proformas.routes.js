// =====================================================================
//  Proforma.vb + ProformaDetalle.vb  (programa: btVentProforma)   SP: MAM_Proforma
//  Mismo formulario que Tec_Ventas con estas diferencias del escritorio:
//   * Sin incremento: Total = Suma(detalle) - Descuento
//   * Productos siempre de L_prListarProductosVentas (@tipo 5): TODOS, tambien los sin stock
//     (el escritorio los pinta en rojo y deja elegirlos: frm_CantidadCajas con TipoMovimiento=4 no limita stock)
//   * Nota: ListarProformaRecibo (MAM_Proforma @tipo 6), misma hoja que la nota de venta
//   * La unica forma de crear en el escritorio es btnVentaSinConciliacion:
//     VentaDirecta=1, SinConciliacion=1 -> se graba @Ventadirecta=0, @VentaDirectaSinConciliacion=1, EstadoPedido=4.
//     La web graba EXACTAMENTE esos valores, pero permite crear segun el permiso Insertar del rol
//     (en el escritorio solo lo veian usuarios con Usuarios.pedido = 1).
// =====================================================================
const L = require('../../negocio/AccesoLogica');
const { crearRouter } = require('./ventas.routes');

module.exports = crearRouter({
    programa: 'btVentProforma',
    nombre: 'Proforma',
    sp: 'MAM_Proforma',
    listaDetalles: (id) => L.ListaProformaDetalles(id),
    insertar: (v, d) => L.ProformaInsertar(v, d),
    modificar: (v, d) => L.ProformaModificar(v, d),
    recibo: (id) => L.ListarProformaRecibo(id),
    sinStock: true,
    conIncremento: false,
    exigePermisoSinConciliacion: false,
    modos: { proforma: { VentaDirecta: 1, SinConciliacion: 1, EstadoPedido: 4 } },
});
