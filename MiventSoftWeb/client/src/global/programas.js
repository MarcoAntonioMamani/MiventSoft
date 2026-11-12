// =====================================================================
//  Registro de programas web.  Clave = Programas.IdPrograma (nombre del boton
//  en Tec_Principal del escritorio). Lo que no este aqui aparece como "Proximamente".
//  Rutas:  /<modulo>  -> panel del modulo,   /<modulo>/<programa>[/...]  -> programa
// =====================================================================
export const PROGRAMAS_WEB = {
  BTCONFROLES: { ruta: '/configuracion/roles', icono: 'shield' },
  BTCONFUSUARIOS: { ruta: '/configuracion/usuarios', icono: 'users' },
  BTNCONFCATEGORIA: { ruta: '/configuracion/categorias', icono: 'tag' },
  BTCONFEMPRESA: { ruta: '/configuracion/empresas', icono: 'building' },
  BTCONFCLIENTE: { ruta: '/configuracion/clientes', icono: 'store' },
  BTCONFPRODUCTO: { ruta: '/configuracion/productos', icono: 'box' },
  BTCONFPRECIO: { ruta: '/configuracion/precios', icono: 'dollar' },
  BTNKITS: { ruta: '/configuracion/kits', icono: 'box' },
  // Kaily: Tec_Dosificacion (MAM_Dosificacion)
  BTCONFDOSIFICACION: { ruta: '/configuracion/dosificacion', icono: 'receipt' },
  BTZONA: { ruta: '/configuracion/zonas', icono: 'map' },
  BTZONAMAPACLIENTE: { ruta: '/mapa-clientes/ubicacion', icono: 'globe' },
  // Paso 3: modulo Ventas
  BTVENTVENTA: { ruta: '/ventas/ventas', icono: 'cart' },
  BTNPERSONAL: { ruta: '/ventas/personal', icono: 'users' },
  BTNPROVEEDORES: { ruta: '/ventas/proveedores', icono: 'bag' },
  BTNCREDPAGO: { ruta: '/ventas/cuentas-cobrar', icono: 'wallet' },
  // Kaily: btnProforma (Tec_Proformas, MAM_Proforma)
  BTNPROFORMA: { ruta: '/ventas/proformas', icono: 'receipt' },
  // Kaily: Administrar Caja
  BTNGASTOS: { ruta: '/ventas/gastos-ingresos', icono: 'wallet' },
  BTNCIERRECAJERO: { ruta: '/ventas/cierre-cajero', icono: 'calc' },
  BTNCAJAGENERAL: { ruta: '/ventas/caja-general', icono: 'wallet' },
  BTNINGRESOEGRESOSMENSUALES: { ruta: '/ventas/ingresos-egresos-mensuales', icono: 'chart' },
  BTNINGRESOEGRESODETALLADO: { ruta: '/ventas/ingresos-egresos-detallado', icono: 'bars' },
  BTNREPORTECAJADETALLADO: { ruta: '/ventas/reporte-caja-detallado', icono: 'history' },
  // Ingreso y salida de productos (Inventario)
  BTINVAMACEN: { ruta: '/inventario/almacenes', icono: 'building' },
  BTINVMOVIMIENTO: { ruta: '/inventario/movimientos', icono: 'truck' },
  BTINVKARDEX: { ruta: '/inventario/historial-productos', icono: 'bars' },
  BTINVSALDO: { ruta: '/inventario/saldos', icono: 'chart' },
  BTINVSALDOSVALORADOS: { ruta: '/inventario/saldos-valorados', icono: 'pie' },
  BTINVKARDEXREPORTE: { ruta: '/inventario/reporte-general', icono: 'area' },
  BTNINVMONITOREO: { ruta: '/inventario/monitoreo', icono: 'target' },
  // Pestaña fija "Inventario" del escritorio (FormularioStock), la ve todo usuario
  FORMULARIOSTOCK: { ruta: '/inventario/productos', icono: 'box' },
  // Compras
  BTNCOMPRAS: { ruta: '/compras/compras', icono: 'bag' },
  BTNCOMPRACREDITO: { ruta: '/compras/cuentas-pagar', icono: 'wallet' },
  // Kaily no tiene Historico de compras ni Compras por producto
  // Planilla
  BTNCONCEPTOFIJOS: { ruta: '/planilla/conceptos-fijos', icono: 'clipboard' },
  BTNCONTRATOS: { ruta: '/planilla/contratos', icono: 'clipboard' },
  BTNREGISTRARPLANILLA: { ruta: '/planilla/registrar', icono: 'users' },
  BTNPLANILLA: { ruta: '/planilla/planilla', icono: 'wallet' },
  // Panel_Distribucion (Despachos, Conciliaciones, Cierre de caja) no corresponde a Kaily (punto de venta): sin programas web.
  BTNCIERREREGISTROINGRESOEGRESOGENERALES: { ruta: '/distribucion/ingresos-egresos', icono: 'wallet' },
  BTVENTREPORTEREALIZADA: { ruta: '/ventas/reporte-ventas', icono: 'chart' },
  BTVENTREPORTEVENTAVSCOSTO: { ruta: '/ventas/ventas-vs-costos', icono: 'dollar' },
  BTVENTPRODMASVENDIDOS: { ruta: '/ventas/productos-mas-vendidos', icono: 'bars' },
  BTVENTRENDIMIENTO: { ruta: '/ventas/rendimiento-personal', icono: 'target' },
  // Kaily no tiene Ventas por productos, Reporte de cobranza ni Ventas por categoria
};

export const ICONO_MODULO = {
  'INGRESO Y SALIDA': 'truck',
  CONFIGURACION: 'settings',
  VENTAS: 'cart',
  INVENTARIO: 'box',
  ALMACEN: 'box',
  DISTRIBUCION: 'truck',
  COMPRAS: 'bag',
  MAPA: 'globe',
  PLANILLAS: 'wallet',
  INGRESO: 'wallet',
};

export function iconoModulo(nombre = '') {
  const n = nombre.toUpperCase();
  const k = Object.keys(ICONO_MODULO).find((x) => n.includes(x));
  return k ? ICONO_MODULO[k] : 'grid';
}

// "DISTRIBUCIÓN Y RUTAS" -> "distribucion-y-rutas"
export function slug(texto = '') {
  return texto.normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase().trim().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
}

// "CONFIGURACION" -> "Configuración" (para mostrar)
const ACENTOS = { CONFIGURACION: 'Configuración', DISTRIBUCION: 'Distribución', CATEGORIAS: 'Categorías', ALMACEN: 'Almacén' };
export function titulo(texto = '') {
  const t = String(texto).trim();
  if (ACENTOS[t.toUpperCase()]) return ACENTOS[t.toUpperCase()];
  return t.toLowerCase().replace(/(^|\s)\S/g, (c) => c.toUpperCase());
}

/** Busca modulo y programa activos segun la URL */
export function ubicar(modulos = [], pathname = '/') {
  const [m1, m2] = pathname.split('/').filter(Boolean);
  if (!m1) return {};
  const ruta = m2 && `/${m1}/${m2}`;
  // el programa puede estar en otro modulo que el de su ruta (como en el escritorio: Personal en Planillas)
  const duenio = ruta && modulos.find((m) => m.programas.some((p) => PROGRAMAS_WEB[p.IdPrograma]?.ruta === ruta));
  const modulo = duenio || modulos.find((m) => slug(m.cndesc1) === m1)
    || modulos.find((m) => m.programas.some((p) => PROGRAMAS_WEB[p.IdPrograma]?.ruta.split('/')[1] === m1));
  const programa = ruta && modulo?.programas.find((p) => PROGRAMAS_WEB[p.IdPrograma]?.ruta === ruta);
  return { modulo, programa };
}

// Iconos a color del escritorio (Tec_Principal.resx / Resources), en client/public/iconos/<IdPrograma>.png
const ICONOS_IMG = new Set(['BTCOMPROVEEDOR','BTCOMVENDEDOR','BTCONFCLIENTE','BTCONFDOSIFICACION','BTCONFEMPRESA','BTCONFLIBRERIA','BTCONFPRECIO','BTCONFPRODUCTO','BTCONFROLES','BTCONFUSUARIOS','BTINVAMACEN','BTINVKARDEX','BTINVKARDEXREPORTE','BTINVMOVIMIENTO','BTINVSALDO','BTINVSALDOSVALORADOS','BTINVUTILIDAD','BTNASIGNACIONPEDIDOS','BTNASIGNACIONZONA','BTNCIERRECAJA','BTNCIERREREGISTROINGRESOEGRESOGENERALES','BTNCIERREREPORTEANUAL','BTNCIERREREPORTEDETALLADO','BTNCIERREREPORTEGASTOS','BTNCOMPRACREDITO','BTNCOMPRAS','BTNCOMPRASPORPRODUCTO','BTNCONCEPTOFIJOS','BTNCONCILIACIONES','BTNCONFCATEGORIA','BTNCONTRATOS','BTNCREDPAGO','BTNDESPACHO','BTNHISTORICO','BTNHISTORICOCLIENTE','BTNHISTORICOINCREMENTO','BTNINVMONITOREO','BTNPERSONAL','BTNPLANILLA','BTNPROVEEDORES','BTNREGISTRARPLANILLA','BTNREPAUDITORIAVENTAS','BTNREPORTECOBRANZA','BTNREPORTEPRODUCTOCATEGORIA','BTNREPORTEVENTASPORPRODUCTOS','BTNVENTAPRODUCTOS','BTVENTGRAFICA','BTVENTPRODMASVENDIDOS','BTVENTPROFORMA','BTVENTRENDIMIENTO','BTVENTREPORTEREALIZADA','BTVENTREPORTEVENTAVSCOSTO','BTVENTVENTA','BTZONA','BTZONAMAPACLIENTE']);
export const imgPrograma = (id) => (ICONOS_IMG.has(String(id).toUpperCase()) ? `/iconos/${String(id).toUpperCase()}.png` : null);
