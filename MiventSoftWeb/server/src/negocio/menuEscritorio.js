// =====================================================================
//  Menu del escritorio (Tec_Principal.Designer.vb): botones de cada panel, en el orden y con el texto del diseno.
//  _PCargarPrivilegios recorre los modulos (L_prLibreriaDetalleGeneral(1)) en orden y, para el modulo i,
//  solo muestra los botones que EXISTEN en el panel i (listaTabs) y tienen algun permiso. Lo que el rol tenga
//  registrado pero no este en el panel, en el escritorio no aparece: la web hace lo mismo.
//  Generado de Tec_Principal.Designer.vb (Items/SubItems.AddRange y TitleText de cada MetroTileItem).
// =====================================================================
const PANELES = [
    { panel: 'Panel_Configuracion', botones: [
        ['BTCONFROLES', "Roles de Usuario"],
        ['BTCONFUSUARIOS', "Registrar Usuarios"],
        ['BTCONFCLIENTE', "Gestion De Clientes"],
        ['BTCONFPRODUCTO', "Gestion De Productos"],
        ['BTCONFPRECIO', "Modificar Precios"],
        ['BTCONFLIBRERIA', "Gestion De Datos"],
        ['BTCONFDOSIFICACION', "Gestion De Dosificacion"],
        ['BTCOMPROVEEDOR', "Gestion De Proveedores"],
        ['BTCOMVENDEDOR', "Gestion De Vendedores"],
        ['BTNCONFCATEGORIA', "Gestion De Categorias"],
        ['BTCONFEMPRESA', "Gestion De Empresas"],
    ] },
    { panel: 'Panel_Ventas', botones: [
        ['BTVENTVENTA', "Crear Nuevos Pedidos"],
        ['BTNASIGNACIONPEDIDOS', "Asignación De Pedidos"],
        ['BTVENTGRAFICA', "Grafica De Ventas"],
        ['BTVENTREPORTEREALIZADA', "Reporte De Ventas"],
        ['BTVENTREPORTEVENTAVSCOSTO', "Ventas Vs Costos de Productos"],
        ['BTINVUTILIDAD', "Ingresos Por Productos"],
        ['BTVENTPRODMASVENDIDOS', "Reporte Productos Mas Vendidos"],
        ['BTVENTRENDIMIENTO', "Rendimiento Personal"],
        ['BTNCREDPAGO', "Cuentas Por Cobrar A Credito"],
        ['BTNVENTAPRODUCTOS', "Reporte Ventas Por Productos"],
        ['BTNREPORTECOBRANZA', "REPORTE COBRANZA"],
        ['BTVENTPROFORMA', "Crear Proformas"],
        ['BTNREPORTEPRODUCTOCATEGORIA', "Ventas Por Categoria"],
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
        ['BTNCOMPRAS', "Realizar Compras"],
        ['BTNCOMPRACREDITO', "Administrar Cuentas Por Pagar"],
        ['BTNPROVEEDORES', "Gestion De Proveedores"],
        ['BTNHISTORICO', "Reporte Historico Compras"],
        ['BTNCOMPRASPORPRODUCTO', "Reporte Compras Por Productos"],
    ] },
    { panel: 'Panel_Planillas', botones: [
        ['BTNPERSONAL', "Gestion De Personal"],
        ['BTNCONCEPTOFIJOS', "Conceptos Fijos"],
        ['BTNCONTRATOS', "Administrar Contratos"],
        ['BTNREGISTRARPLANILLA', "Asignar Trabajador A Planilla"],
        ['BTNPLANILLA', "Administrar Planilla Salarios"],
    ] },
    { panel: 'Panel_Distribucion', botones: [
        ['BTNDESPACHO', "Despachar Productos"],
        ['BTNCONCILIACIONES', "Conciliar Productos"],
        ['BTNCIERRECAJA', "Cierre De Caja"],
        ['METROTILEITEM4', "Asignar Trabajador A Planilla"],
        ['METROTILEITEM5', "Administrar Planilla Salarios"],
        ['BTNCIERREREPORTEGASTOS', "Reporte Gastos Chofer"],
        ['BTNCIERREREGISTROINGRESOEGRESOGENERALES', "Registrar Gastos & Ingresos Generales"],
        ['BTNCIERREREPORTEANUAL', "Reporte Ingresos & Egresos Anual"],
        ['BTNCIERREREPORTEDETALLADO', "Reporte Ingresos & Egresos Detallado"],
        ['BTNASIGNACIONZONA', "Asignacion De Zonas"],
        ['BTNHISTORICOCLIENTE', "Historico Registro Clientes"],
        ['BTNREPORTEVENTASPORPRODUCTOS', "Ventas Productos Por Cliente"],
        ['BTNREPAUDITORIAVENTAS', "Auditoria Ventas"],
        ['BTNHISTORICOINCREMENTO', "Historial Incremento Ventas"],
    ] }
];

const _ids = PANELES.map((p) => new Map(p.botones.map(([id, texto], i) => [id, { texto, orden: i }])));

/**
 * Deja cada modulo como en el escritorio.
 * @param {Array<{cnnum, cndesc1, programas: Array<{IdPrograma, DescripcionPrograma}>}>} modulos en el orden de L_prLibreriaDetalleGeneral(1)
 *  - panel del modulo: el de su misma posicion (como listaTabs.Item(i)); si no comparte ningun boton
 *    (otro orden en la base), el panel con mas botones en comun
 *  - programas: solo los que estan en ese panel, en el orden del panel y con su texto
 *  - si ningun panel coincide se deja como viene (no se oculta nada que no se pueda ubicar)
 */
function comoEscritorio(modulos) {
    return modulos.map((m, i) => {
        const comunes = (k) => m.programas.filter((p) => _ids[k].has(String(p.IdPrograma).toUpperCase())).length;
        let k = i < PANELES.length && comunes(i) > 0 ? i : -1;
        if (k < 0) {
            let mejor = 0;
            PANELES.forEach((_, j) => { const c = comunes(j); if (c > mejor) { mejor = c; k = j; } });
        }
        if (k < 0) return m;
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
