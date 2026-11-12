import { Navigate, Route, Routes } from 'react-router-dom';
import { useSesion, PermisoContext } from './global/Sesion';
import Tec_Login from './paginas/Tec_Login';
import Tec_Principal from './paginas/Tec_Principal';
import Inicio from './paginas/Inicio';
import PanelModulo from './paginas/PanelModulo';
import TecCategorias from './paginas/configuracion/TecCategorias';
import Tec_Empresas from './paginas/configuracion/Tec_Empresas';
import Tec_Users from './paginas/configuracion/Tec_Users';
import Tec_Roles from './paginas/configuracion/Tec_Roles';
import Tec_Clientes from './paginas/configuracion/Tec_Clientes';
import Tec_Zonas from './paginas/configuracion/Tec_Zonas';
import Tec_Productos from './paginas/configuracion/Tec_Productos';
import Tec_Precios from './paginas/configuracion/Tec_Precios';
import Tec_Kits from './paginas/configuracion/Tec_Kits';
import Tec_Dosificacion from './paginas/configuracion/Tec_Dosificacion';
// Kaily La Paz (punto de venta). Tec_Ventas.jsx (distribucion) lo sigue usando Proforma.jsx
import Tec_Ventas from './paginas/ventas/Tec_VentasPuntoVenta';
import Tec_Personal from './paginas/ventas/Tec_Personal';
import Frm_Proveedor from './paginas/ventas/Frm_Proveedor';
import Tec_CuentasPorCobrar from './paginas/ventas/Tec_CuentasPorCobrar';
import Reporte from './paginas/ventas/Reportes';
import Proforma from './paginas/ventas/Proforma';
import Tec_DespachoVentas from './paginas/ventas/Tec_DespachoVentas';
import Tec_IngresoEgreso, { URL_CAJA } from './paginas/distribucion/Tec_IngresoEgreso';
import Tec_CierreCajero from './paginas/caja/Tec_CierreCajero';
import ReporteCaja from './paginas/caja/ReportesCaja';
import ReporteDistribucion from './paginas/distribucion/ReportesDistribucion';
import Tec_ConceptosFijos from './paginas/planilla/Tec_ConceptosFijos';
import Tec_Contratos from './paginas/planilla/Tec_Contratos';
import Tec_RegistrarPlanilla from './paginas/planilla/Tec_RegistrarPlanilla';
import Tec_Planilla from './paginas/planilla/Tec_Planilla';
import Tec_Compras from './paginas/compras/Tec_Compras';
import Tec_CuentasPorPagar from './paginas/compras/Tec_CuentasPorPagar';
import Tec_Almacenes from './paginas/inventario/Tec_Almacenes';
import Tec_Movimientos from './paginas/inventario/Tec_Movimientos';
import Tec_MapaClientes from './paginas/mapa/Tec_MapaClientes';
import ReporteInventario from './paginas/inventario/ReportesInventario';
import Tec_InventarioProductos from './paginas/inventario/Tec_InventarioProductos';

// Reportes de Gestion Pedidos: [ruta, programa del escritorio, clave]
const REPORTES = [
  ['ventas/reporte-ventas', 'btVentReporteRealizada', 'ventas-realizadas'],
  ['ventas/ventas-vs-costos', 'btVentReporteVentaVsCosto', 'ventas-costos'],
  ['ventas/productos-mas-vendidos', 'btVentProdMasVendidos', 'productos-mas-vendidos'],
  ['ventas/rendimiento-personal', 'btVentRendimiento', 'rendimiento'],
].map(([ruta, prog, clave]) => [ruta, prog, () => <Reporte key={clave} clave={clave} />]);

function Protegido({ programa, children }) {
  const { tienePrograma, permisos } = useSesion();
  return tienePrograma(programa) ? <PermisoContext.Provider value={permisos(programa)}>{children}</PermisoContext.Provider> : (
    <div className="sin-permiso entrada"><b>Sin acceso</b><span>Su rol no tiene privilegios para este programa.</span></div>
  );
}

// Cada programa: /modulo/programa, /nuevo, /:id, /:id/editar
const PROGRAMAS = [
  ['configuracion/categorias', 'btnConfCategoria', TecCategorias],
  ['configuracion/empresas', 'btConfEmpresa', Tec_Empresas],
  ['configuracion/usuarios', 'btConfUsuarios', Tec_Users],
  ['configuracion/roles', 'btConfRoles', Tec_Roles],
  ['configuracion/clientes', 'btConfCliente', Tec_Clientes],
  ['configuracion/zonas', 'btZona', Tec_Zonas],
  ['configuracion/productos', 'btConfProducto', Tec_Productos],
  ['configuracion/precios', 'btConfPrecio', Tec_Precios],
  ['configuracion/kits', 'btnKits', Tec_Kits], // Kaily: MAM_Kits
  ['configuracion/dosificacion', 'btConfDosificacion', Tec_Dosificacion], // Kaily: MAM_Dosificacion
  ['ventas/ventas', 'btVentVenta', Tec_Ventas],
  ['ventas/personal', 'btnPersonal', Tec_Personal],
  ['ventas/proveedores', 'btnProveedores', Frm_Proveedor],
  ['ventas/cuentas-cobrar', 'btnCredPago', Tec_CuentasPorCobrar],
  ['ventas/proformas', 'btnProforma', Proforma], // Kaily: btnProforma (MAM_Proforma)
  ['ventas/despacho', 'btnDespachoVenta', Tec_DespachoVentas], // Kaily: FrmDespacho (MAM_Ventas @tipo 11)
  ['planilla/conceptos-fijos', 'btnConceptoFijos', Tec_ConceptosFijos],
  ['planilla/contratos', 'btnContratos', Tec_Contratos],
  ['planilla/registrar', 'btnRegistrarPlanilla', Tec_RegistrarPlanilla],
  ['planilla/planilla', 'btnPlanilla', Tec_Planilla],
  ['compras/compras', 'btnCompras', Tec_Compras],
  ['compras/cuentas-pagar', 'btnCompraCredito', Tec_CuentasPorPagar],
  ['inventario/almacenes', 'btInvAmacen', Tec_Almacenes],
  ['inventario/movimientos', 'btInvMovimiento', Tec_Movimientos],
  ['inventario/productos', 'FormularioStock', Tec_InventarioProductos],
  ['mapa-clientes/ubicacion', 'btZonaMapaCliente', Tec_MapaClientes],
  ...[['historial-productos', 'btInvKardex', 'kardex'], ['saldos', 'btInvSaldo', 'saldos'], ['saldos-valorados', 'btInvSaldosValorados', 'saldos-valorados'],
    ['reporte-general', 'btInvKardexReporte', 'general'], ['monitoreo', 'btnInvMonitoreo', 'monitoreo']]
    .map(([ruta, prog, clave]) => [`inventario/${ruta}`, prog, () => <ReporteInventario key={clave} clave={clave} />]),
  ['distribucion/ingresos-egresos', 'btnCierreRegistroIngresoEgresoGenerales', Tec_IngresoEgreso],
  // Kaily: Administrar Caja
  ['ventas/gastos-ingresos', 'btnGastos', () => <Tec_IngresoEgreso url={URL_CAJA} />],
  ['ventas/cierre-cajero', 'btnCierreCajero', Tec_CierreCajero],
  ['ventas/ingresos-egresos-mensuales', 'btnIngresoEgresosMensuales', () => <ReporteDistribucion key="ie-anual" clave="ingresos-egresos-anual" />],
  ['ventas/ingresos-egresos-detallado', 'btnIngresoEgresoDetallado', () => <ReporteDistribucion key="ie-det" clave="ingresos-egresos-detallado" />],
  ['ventas/caja-general', 'btnCajaGeneral', () => <ReporteCaja key="caja-general" clave="caja-general" />],
  ['ventas/reporte-caja-detallado', 'btnReporteCajaDetallado', () => <ReporteCaja key="caja-detallado" clave="caja-detallado" />],
  ...REPORTES,
];

// Rutas de los programas: las usa el enrutador y cada pestaña de "programas abiertos" (Tec_Principal)
const RUTAS_PROGRAMAS = PROGRAMAS.map(([ruta, prog, Comp]) => (
  <Route key={ruta} path={`${ruta}/*`} element={<Protegido programa={prog}><Comp /></Protegido>} />
));

export default function App() {
  const { sesion, cargando } = useSesion();
  if (cargando) return <div className="cargando-app"><span className="spinner grande" /></div>;
  if (!sesion) return <Tec_Login />;
  return (
    <Routes>
      <Route element={<Tec_Principal rutas={RUTAS_PROGRAMAS} />}>
        <Route index element={<Inicio />} />
        {RUTAS_PROGRAMAS}
        <Route path=":modulo" element={<PanelModulo />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}
