// Proforma.vb -> Gestion De Proformas (SP: MAM_Proforma). Misma pantalla que Tec_Ventas, sin incremento.
import { PantallaVentas, CFG_PROFORMA } from './Tec_Ventas';

export default function Proforma() {
  return <PantallaVentas cfg={CFG_PROFORMA} />;
}
