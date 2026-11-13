// =====================================================================
//  Menu del escritorio (Tec_Principal.Designer.vb): botones de cada panel, en el orden y con el texto del diseno.
//  _PCargarPrivilegios recorre los modulos (L_prLibreriaDetalleGeneral(1)) en orden y, para el modulo i,
//  solo muestra los botones que EXISTEN en el panel i (listaTabs) y tienen algun permiso. Lo que el rol tenga
//  registrado pero no este en el panel, en el escritorio no aparece: la web hace lo mismo.
//  Generado de Tec_Principal.Designer.vb de KAILY LA PAZ (punto de venta):
//  Items/SubItems.AddRange de cada panel y TitleText de cada MetroTileItem.
// =====================================================================
const PANELES = [
    { panel: 'Panel_Configuracion', botones: [
        ['BTCONFROLES', "Roles de Usuario"],
        ['BTCONFUSUARIOS', "Gestion De Usuarios"],
        ['BTCONFCLIENTE', "Gestion De Clientes"],
        ['BTCONFPRODUCTO', "Gestion De Productos"],
        ['BTCONFPRECIO', "Gestion De Precios"],
        ['BTCONFLIBRERIA', "Gestion De Datos"],
        ['BTCONFDOSIFICACION', "Gestion De Dosificacion"],
        ['BTCOMPROVEEDOR', "Gestion De Proveedores"],
        ['BTCOMVENDEDOR', "Gestion De Vendedores"],
        ['BTNCONFCATEGORIA', "Gestion De Categorias"],
        ['BTCONFEMPRESA', "Gestion De Empresas"],
        ['BTNKITS', "Gestion De Kits"],
        // Solo web (no esta en el escritorio): sql/07_Auditoria.sql crea el programa y lo asigna al Administrador
        ['BTNAUDITORIAREGISTROS', "Auditoria De Registros"],
    ] },
    // Panel_Ventas tiene dos grupos: "Ventas" (ItemContainer2) y "Administrar Caja" (ItemContainer8)
    { panel: 'Panel_Ventas', botones: [
        ['BTNPROFORMA', "Crear Proformas"],
        ['BTVENTVENTA', "Gestion De Ventas"],
        ['BTVENTREPORTEREALIZADA', "Reporte De Ventas"],
        ['BTVENTGRAFICA', "Grafica De Ventas"],
        ['BTVENTREPORTEVENTAVSCOSTO', "Ventas Vs Costos de Productos"],
        ['BTINVUTILIDAD', "Ingresos Por Productos"],
        ['BTVENTPRODMASVENDIDOS', "Reporte Productos Mas Vendidos"],
        ['BTVENTRENDIMIENTO', "Rendimiento Personal"],
        ['BTNCREDPAGO', "Cuentas Por Cobrar A Credito"],
        // btnDespachoVenta (FrmDespacho): ultimo del grupo Ventas (ItemContainer2), TitleText "Despacho De Productos"
        ['BTNDESPACHOVENTA', "Despacho De Productos"],
        ['BTNGASTOS', "Registrar Gastos o Ingresos"],
        ['BTNCIERRECAJERO', "Administrar Cierre Caja"],
        ['BTNCAJAGENERAL', "Caja General"],
        ['BTNINGRESOEGRESOSMENSUALES', "Ingresos Egresos Mensuales"],
        ['BTNINGRESOEGRESODETALLADO', "Ingresos Egresos Detallado"],
        ['BTNREPORTECAJADETALLADO', "Reporte Caja Detallado"],
    ] },
    // tab_ingresoproducto.Text = "Ingreso y Salida De Productos" (el modulo se muestra con ese nombre, no con el de la base)
    { panel: 'Panel_ingresos', titulo: 'INGRESO Y SALIDA DE PRODUCTOS', botones: [
        ['BTINVMOVIMIENTO', "Ingreso y Salida De Productos"],
        ['BTINVKARDEX', "Historial De Productos"],
        ['BTINVSALDO', "Reporte De Saldos Productos"],
        ['BTINVSALDOSVALORADOS', "Saldos Valorados"],
        ['BTINVKARDEXREPORTE', "Reporte General Historial"],
        ['BTNINVMONITOREO', "Monitoreo De Productos"],
    ] },
    { panel: 'Panel_Mapa', botones: [
        ['BTZONA', "Gestion De Zonas"],
        ['BTZONAMAPACLIENTE', "Ubicacion De Clientes"],
    ] },
    { panel: 'Panel_Almacen', botones: [
        ['BTINVAMACEN', "Gestion De Sucursales"],
        ['METROTILEITEM30', "Ingresos Por Sucursal"],
    ] },
    { panel: 'Panel_Compras', botones: [
        ['BTNCOMPRAS', "Gestionar Compras"],
        ['BTNCOMPRACREDITO', "Administrar Cuentas Por Pagar"],
        ['BTNPROVEEDORES', "Gestion De Proveedores"],
    ] },
    { panel: 'Panel_Planillas', botones: [
        ['BTNPERSONAL', "Gestion De Personal"],
        ['BTNCONCEPTOFIJOS', "Conceptos Fijos"],
        ['BTNCONTRATOS', "Administrar Contratos"],
        ['BTNREGISTRARPLANILLA', "Asignar Trabajador A Planilla"],
        ['BTNPLANILLA', "Administrar Planilla Salarios"],
    ] },
    // Panel_Distribucion (Despachos, Conciliaciones, Cierre de caja) existe en el diseño pero no corresponde
    // a Kaily (punto de venta): no se migra y su modulo no se muestra en la web.
];

const _ids = PANELES.map((p) => new Map(p.botones.map(([id, texto], i) => [id, { texto, orden: i }])));

/**
 * Deja cada modulo como en el escritorio.
 * @param {Array<{cnnum, cndesc1, programas: Array<{IdPrograma, DescripcionPrograma}>}>} modulos en el orden de L_prLibreriaDetalleGeneral(1)
 *  - panel del modulo: el de su misma posicion (como listaTabs.Item(i)); si no comparte ningun boton
 *    (otro orden en la base), el panel con mas botones en comun
 *  - programas: solo los que estan en ese panel, en el orden del panel y con su texto
 *  - si ningun panel coincide el modulo no se muestra (ej. Distribucion, que no corresponde a Kaily)
 */
function comoEscritorio(modulos) {
    return modulos.map((m, i) => {
        const comunes = (k) => m.programas.filter((p) => _ids[k].has(String(p.IdPrograma).toUpperCase())).length;
        let k = i < PANELES.length && comunes(i) > 0 ? i : -1;
        if (k < 0) {
            let mejor = 0;
            PANELES.forEach((_, j) => { const c = comunes(j); if (c > mejor) { mejor = c; k = j; } });
        }
        if (k < 0) return { ...m, programas: [] };
        const panel = _ids[k];
        const programas = m.programas
            .filter((p) => panel.has(String(p.IdPrograma).toUpperCase()))
            .map((p) => ({ ...p, DescripcionPrograma: String(panel.get(String(p.IdPrograma).toUpperCase()).texto || p.DescripcionPrograma).toUpperCase() }))
            .sort((a, b) => panel.get(a.IdPrograma.toUpperCase()).orden - panel.get(b.IdPrograma.toUpperCase()).orden);
        return { ...m, ...(PANELES[k].titulo ? { cndesc1: PANELES[k].titulo } : {}), panel: PANELES[k].panel, programas };
    }).filter((m) => m.programas.length).concat(MODULO_INVENTARIO);
}

// Pestaña fija tab_InvProductos ("Inventario") con FormularioStock: en el escritorio no esta en listMenu,
// asi que no depende de RolesDetalles y la ve todo usuario. Va al final, como en el escritorio.
const MODULO_INVENTARIO = {
    cnnum: 'INVENTARIO', cndesc1: 'INVENTARIO', panel: 'Panel_InvProductos', fijo: true,
    programas: [{ IdPrograma: 'FORMULARIOSTOCK', DescripcionPrograma: 'INVENTARIO PRODUCTOS', I: 0, M: 0, E: 0 }],
};

module.exports = { PANELES, comoEscritorio, MODULO_INVENTARIO };
