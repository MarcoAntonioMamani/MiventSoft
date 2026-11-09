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
import Tec_Ventas from './paginas/ventas/Tec_Ventas';
import Tec_Personal from './paginas/ventas/Tec_Personal';
import Frm_Proveedor from './paginas/ventas/Frm_Proveedor';
import Tec_CuentasPorCobrar from './paginas/ventas/Tec_CuentasPorCobrar';
import Reporte from './paginas/ventas/Reportes';
import Proforma from './paginas/ventas/Proforma';
import Tec_AsignacionPedidos from './paginas/ventas/Tec_AsignacionPedidos';
import Tec_Despachos from './paginas/distribucion/Tec_Despachos';
import Tec_Conciliacion from './paginas/distribucion/Tec_Conciliacion';
import Tec_IngresoEgreso from './paginas/distribucion/Tec_IngresoEgreso';
import Tec_CierreCaja from './paginas/distribucion/Tec_CierreCaja';
import Tec_AsignacionZona from './paginas/distribucion/Tec_AsignacionZona';
import ReporteDistribucion from './paginas/distribucion/ReportesDistribucion';
import Tec_ConceptosFijos from './paginas/planilla/Tec_ConceptosFijos';
import Tec_Contratos from './paginas/planilla/Tec_Contratos';
import Tec_RegistrarPlanilla from './paginas/planilla/Tec_RegistrarPlanilla';
import Tec_Planilla from './paginas/planilla/Tec_Planilla';
import Tec_Compras from './paginas/compras/Tec_Compras';
import Tec_CuentasPorPagar from './paginas/compras/Tec_CuentasPorPagar';
import ReporteCompras from './paginas/compras/ReportesCompras';
import Tec_Almacenes from './paginas/inventario/Tec_Almacenes';
import Tec_Movimientos from './paginas/inventario/Tec_Movimientos';
import Tec_MapaClientes from './paginas/mapa/Tec_MapaClientes';
import ReporteInventario from './paginas/inventario/ReportesInventario';

// Reportes de Gestion Pedidos: [ruta, programa del escritorio, clave]
const REPORTES = [
  ['ventas/reporte-ventas', 'btVentReporteRealizada', 'ventas-realizadas'],
  ['ventas/ventas-vs-costos', 'btVentReporteVentaVsCosto', 'ventas-costos'],
  ['ventas/productos-mas-vendidos', 'btVentProdMasVendidos', 'productos-mas-vendidos'],
  ['ventas/rendimiento-personal', 'btVentRendimiento', 'rendimiento'],
  ['ventas/ventas-por-productos', 'btnVentaProductos', 'ventas-productos'],
  ['ventas/reporte-cobranza', 'btnReporteCobranza', 'historico-cobros'],
  ['ventas/ventas-por-categoria', 'btnReporteProductoCategoria', 'ventas-categoria'],
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
  ['ventas/ventas', 'btVentVenta', Tec_Ventas],
  ['ventas/personal', 'btnPersonal', Tec_Personal],
  ['ventas/proveedores', 'btnProveedores', Frm_Proveedor],
  ['ventas/cuentas-cobrar', 'btnCredPago', Tec_CuentasPorCobrar],
  ['ventas/proformas', 'btVentProforma', Proforma],
  ['ventas/asignacion-pedidos', 'btnAsignacionPedidos', Tec_AsignacionPedidos],
  ['planilla/conceptos-fijos', 'btnConceptoFijos', Tec_ConceptosFijos],
  ['planilla/contratos', 'btnContratos', Tec_Contratos],
  ['planilla/registrar', 'btnRegistrarPlanilla', Tec_RegistrarPlanilla],
  ['planilla/planilla', 'btnPlanilla', Tec_Planilla],
  ['compras/compras', 'btnCompras', Tec_Compras],
  ['compras/cuentas-pagar', 'btnCompraCredito', Tec_CuentasPorPagar],
  ['compras/historico', 'btnHistorico', () => <ReporteCompras key="historico" clave="historico" />],
  ['compras/compras-por-producto', 'btnComprasPorProducto', () => <ReporteCompras key="cpp" clave="compras-por-producto" />],
  ['inventario/almacenes', 'btInvAmacen', Tec_Almacenes],
  ['inventario/movimientos', 'btInvMovimiento', Tec_Movimientos],
  ['mapa-clientes/ubicacion', 'btZonaMapaCliente', Tec_MapaClientes],
  ...[['historial-productos', 'btInvKardex', 'kardex'], ['saldos', 'btInvSaldo', 'saldos'], ['saldos-valorados', 'btInvSaldosValorados', 'saldos-valorados'],
    ['reporte-general', 'btInvKardexReporte', 'general'], ['monitoreo', 'btnInvMonitoreo', 'monitoreo']]
    .map(([ruta, prog, clave]) => [`inventario/${ruta}`, prog, () => <ReporteInventario key={clave} clave={clave} />]),
  ['distribucion/despachos', 'btnDespacho', Tec_Despachos],
  ['distribucion/conciliaciones', 'btnConciliaciones', Tec_Conciliacion],
  ['distribucion/cierre-caja', 'btnCierreCaja', Tec_CierreCaja],
  ['distribucion/ingresos-egresos', 'btnCierreRegistroIngresoEgresoGenerales', Tec_IngresoEgreso],
  ['distribucion/asignacion-zonas', 'btnAsignacionZona', Tec_AsignacionZona],
  ...[
    ['gastos-chofer', 'btnCierreReporteGastos'], ['ingresos-egresos-anual', 'btnCierreReporteAnual'], ['ingresos-egresos-detallado', 'btnCierreReporteDetallado'],
    ['historico-clientes', 'btnhistoricoCliente'], ['productos-por-cliente', 'btnReporteVentasPorProductos'], ['auditoria-ventas', 'btnRepAuditoriaVentas'],
    ['historial-incremento', 'btnhistoricoIncremento'],
  ].map(([clave, prog]) => [`distribucion/${clave}`, prog, () => <ReporteDistribucion key={clave} clave={clave} />]),
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
