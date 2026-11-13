// =====================================================================
//  KAILY LA PAZ: Tec_Proformas.vb (programa: btnProforma)   SP: MAM_Proforma
//  Misma pantalla que Tec_Ventas de Kaily, con estas diferencias del escritorio:
//   * ProformaInsertar / ProformaModificar: sin @VentaPagos ni @Facturado; TipoVenta = 1 y vencimiento = hoy.
//   * Lista: L_prListarVentasGeneralFiltroFecha("MAM_Proforma", ...) sin filtrar por vendedor.
//   * Productos con TipoPrograma = 2: se puede cotizar sin stock (no limita la cantidad).
//   * No exige Caja Chica abierta ni bloquea registros de dias anteriores.
//   * Nota: ListarVentaProforma (MAM_Proforma @tipo 6). Total = Suma - Descuento.
//     Total en dolares con el tipo de cambio VIGENTE: la lista (@tipo 3) no trae tipo de cambio y el de
//     @tipo 6 sale de VentasMonedaCobrada con el Id de la proforma (es el cobro de otra venta) o 6.95 fijo.
//  (La proforma de distribucion, btVentProforma, queda en ventas.routes.js sin usarse en Kaily.)
// =====================================================================
const L = require('../../negocio/AccesoLogica');
const { crearRouterPV } = require('./ventasPuntoVenta.routes');

module.exports = crearRouterPV({
    programa: 'btnProforma',
    nombre: 'Proforma',
    sp: 'MAM_Proforma',
    listaDetalles: (id) => L.ListaProformasDetalles(id),
    insertar: (x, d) => L.ProformaInsertar(x, d),
    modificar: (x, d) => L.ProformaModificar(x, d),
    recibo: (id) => L.ListarVentaProforma(id),
    conPagos: false, exigeCaja: false, validarEditable: false, filtrarPorVendedor: false, sinStock: true, soloContado: true, tcNota: 'vigente',
});
