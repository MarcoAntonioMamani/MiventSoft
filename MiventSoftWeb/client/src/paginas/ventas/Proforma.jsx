// Kaily: Tec_Proformas.vb -> Gestion De Proformas (SP: MAM_Proforma). Misma pantalla que Ventas de Kaily,
// sin cobro, sin tipo de venta ni facturado, y se puede cotizar sin stock.
import { PantallaPV, CFG_PROFORMA_PV } from './Tec_VentasPuntoVenta';

export default function Proforma() {
  return <PantallaPV cfg={CFG_PROFORMA_PV} />;
}
