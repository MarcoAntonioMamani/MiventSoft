// =====================================================================
//  SOLO DESARROLLO / PRUEBAS: simula los SP en memoria (sin SQL Server).
//  Se activa con MIVENT_MOCK=1 o "npm run mock" (nunca en NODE_ENV=production).
//  Usuario de prueba:  ADMIN / 123
//
//  VALIDA EL CONTRATO DEL ESCRITORIO: cada llamada se compara contra
//  test/contrato_vb.json (extraido de Negocio/AccesoLogica.vb). Si la web envia
//  a un SP + @tipo un conjunto de parametros distinto al que envia VB, falla.
// =====================================================================
const path = require('node:path');
const fs = require('node:fs');

const CONTRATO = JSON.parse(fs.readFileSync(path.join(__dirname, '..', '..', 'test', 'contrato_vb.json'), 'utf8'));
// SP propios de la web (server/sql/*.sql), no existen en AccesoLogica.vb: se validan igual (parametros exactos)
const _pc = (funcion, params) => [{ funcion, params }];
Object.assign(CONTRATO, {
    'MAM_PanelControl|1': _pc('L_prPanelHorasVentas', ['@fechaf', '@fechai', '@sucursalid', '@tipo', '@usuario']),
    'MAM_PanelControl|2': _pc('L_prPanelComprasClientes', ['@fechaf', '@fechai', '@sucursalid', '@tipo', '@usuario']),
    'MAM_PanelControl|3': _pc('L_prPanelProductosSinVenta', ['@fechaf', '@fechai', '@tipo', '@usuario']),
    // Auditoria De Registros (server/sql/07_Auditoria.sql)
    'MAM_Auditoria|3': _pc('L_prAuditoriaListar', ['@desde', '@documentoid', '@hasta', '@modulo', '@operacion', '@tipo', '@usuario', '@usuariofiltro']),
    'MAM_Auditoria|4': _pc('L_prAuditoriaDetalle', ['@id', '@tipo', '@usuario']),
    'MAM_Auditoria|5': _pc('L_prAuditoriaUsuarios', ['@tipo', '@usuario']),
});
const llamadas = []; // historial para las pruebas automaticas

// Columnas que tienen los tipos tabla (TVP) en el escritorio
const TVP_COLUMNAS = { cortebilletes: 8, moduloid: 4, roldetalletype: 10, zonaspuntos: 5, tcl0064: 5, preciot: 3, ty007: 6, ventadetalletype: 22, ventapagos: 8, asignaciontype: 5, detallegastos: 8, cobranzas: 5, asignacion: 8, asignacionentregado: 11, asignacionanulacion: 9, detalledespacho: 10, referenciaventa: 2, detalleconciliacion: 5, detalle: 13 };
// Por SP (tiene prioridad): ancho desconocido en el escritorio -> null = no se valida (ver test/verificar.js)
const TVP_SP = { 'MAM_Movimientos:detalle': 10, 'MAM_Compras:detalle': null, 'MAM_Contratos:detallecontratoconcepto': 6, 'MAM_Planilla:planillaconcepto': null };
// Orden del SELECT de MAM_Ventas @tipo 4 de Kaily (simulado: 22 columnas, como _prAddDetalleVenta de Tec_VentasDetalle.vb)
const COLS_VDET = ['Id', 'VentaId', 'ProductoId', 'Producto', 'Cantidad', 'Precio', 'SubTotal', 'ProcentajeDescuento', 'MontoDescuento', 'Total', 'Detalle', 'PrecioCosto',
    'Lote', 'FechaVencimiento', 'estado', 'TipoNombre', 'Tipo', 'KitNombre', 'KitId', 'CantidadKit', 'img', 'stock'];
// MAM_Ventas @tipo 8 (ListaVentasDetallePago): a.Id, a.VentaId, a.MontoBs, a.MontoDolares, a.TarjetaBancaria, a.TransferenciaBancaria, a.TipoCambio, estado
const COLS_PAGO = ['Id', 'VentaId', 'MontoBs', 'MontoDolares', 'TarjetaBancaria', 'TransferenciaBancaria', 'TipoCambio', 'estado'];

function _validarContrato(sp, lista) {
    const tipo = lista.find((p) => p.nombre.toLowerCase() === '@tipo')?.valor;
    const enviados = [...new Set(lista.map((p) => p.nombre.toLowerCase()))].sort();
    const candidatos = CONTRATO[`${sp}|${tipo}`] || CONTRATO[`*NameSp|${tipo}`] || CONTRATO[`*sp|${tipo}`] || [];
    const ok = candidatos.some((c) => c.params.length === enviados.length && c.params.every((p, i) => p === enviados[i]));
    if (!ok) {
        const esperado = candidatos.map((c) => `${c.funcion}: ${c.params.join(',')}`).join(' | ') || '(no existe en AccesoLogica.vb)';
        throw new Error(`[CONTRATO VB] ${sp} @tipo=${tipo} envio [${enviados.join(',')}] y VB envia ${esperado}`);
    }
    for (const p of lista) {
        if (!p.detalle) continue;
        const k = `${sp}:${p.nombre.slice(1).toLowerCase()}`;
        const n = k in TVP_SP ? TVP_SP[k] : TVP_COLUMNAS[p.nombre.slice(1).toLowerCase()];
        const largo = p.detalle.columnas?.length || (Array.isArray(p.detalle.filas?.[0]) ? p.detalle.filas[0].length : n);
        if (n && largo !== n) throw new Error(`[CONTRATO VB] ${p.nombre} debe tener ${n} columnas y tiene ${largo}`);
    }
    llamadas.push({ sp, tipo: Number(tipo), params: Object.fromEntries(lista.map((p) => [p.nombre, p.detalle ? p.detalle : p.valor])) });
}

// recordset con metadatos de columnas (como mssql)
function tabla(filas, nombres) {
    const r = filas.map((f) => ({ ...f }));
    const cols = nombres || Object.keys(filas[0] || {});
    r.columns = Object.fromEntries(cols.map((n, index) => [n, { name: n, index }]));
    return r;
}

const hoy = () => new Date(Date.now() - 4 * 3600 * 1000).toISOString().slice(0, 10); // fecha de Bolivia (UTC-4), igual que el servidor
const sig = (arr, k = 'Id') => arr.reduce((m, r) => Math.max(m, Number(r[k])), 0) + 1;
const nombreDe = (arr, id, campo, k = 'Id') => (arr.find((x) => Number(x[k]) === Number(id)) || {})[campo] || '';

const db = {
    clasif: {
        1: [{ cnnum: 1, cndesc1: 'CONFIGURACION' }, { cnnum: 2, cndesc1: 'VENTAS' }, { cnnum: 3, cndesc1: 'INVENTARIO' }, { cnnum: 4, cndesc1: 'DISTRIBUCION' }, { cnnum: 5, cndesc1: 'MAPA CLIENTES' }, { cnnum: 6, cndesc1: 'COMPRAS' }, { cnnum: 7, cndesc1: 'PLANILLAS' }, { cnnum: 8, cndesc1: 'ALMACEN' }],
        3: [{ cnnum: 1, cndesc1: 'COCA COLA' }, { cnnum: 2, cndesc1: 'PIL' }],
        4: [{ cnnum: 1, cndesc1: 'SIN AZUCAR' }],
        5: [{ cnnum: 1, cndesc1: 'GASEOSAS' }, { cnnum: 2, cndesc1: 'LACTEOS' }],
        6: [{ cnnum: 1, cndesc1: 'UNIDAD' }, { cnnum: 2, cndesc1: 'PAQUETE' }],
        7: [{ cnnum: 1, cndesc1: 'CAJA' }, { cnnum: 2, cndesc1: 'FARDO' }],
        8: [{ cnnum: 1, cndesc1: 'CI' }, { cnnum: 2, cndesc1: 'NIT' }],
        9: [{ cnnum: 1, cndesc1: 'VENDEDOR' }, { cnnum: 2, cndesc1: 'CHOFER' }, { cnnum: 3, cndesc1: 'ADMINISTRATIVO' }],
        13: [{ cnnum: 1, cndesc1: '2025' }, { cnnum: 2, cndesc1: '2026' }],
        14: [{ cnnum: 1, cndesc1: 'COMBUSTIBLE' }, { cnnum: 2, cndesc1: 'ALIMENTACION' }, { cnnum: 3, cndesc1: 'PEAJE' }],
        10: [{ cnnum: 1, cndesc1: 'PLAZO FIJO' }, { cnnum: 2, cndesc1: 'INDEFINIDO' }],
        11: [{ cnnum: 1, cndesc1: 'CHOFER' }, { cnnum: 2, cndesc1: 'VENDEDOR' }, { cnnum: 3, cndesc1: 'ALMACENERO' }],
        12: ['ENERO', 'FEBRERO', 'MARZO', 'ABRIL', 'MAYO', 'JUNIO', 'JULIO', 'AGOSTO', 'SEPTIEMBRE', 'OCTUBRE', 'NOVIEMBRE', 'DICIEMBRE'].map((m, i) => ({ cnnum: i + 1, cndesc1: m })),
        15: [{ cnnum: 1, cndesc1: 'TIENDA' }, { cnnum: 2, cndesc1: 'MERCADO' }, { cnnum: 3, cndesc1: 'MAYORISTA' }],
    },
    programas: [
        ['btConfRoles', 'Roles', 1], ['btConfUsuarios', 'Usuarios', 1], ['btnConfCategoria', 'Categorias', 1], ['btConfEmpresa', 'Empresas', 1],
        ['btConfCliente', 'Clientes', 1], ['btConfProducto', 'Productos', 1], ['btConfPrecio', 'Precios', 1], ['btZona', 'Gestion De Zonas', 5], ['btZonaMapaCliente', 'Ubicacion De Clientes', 5],
        ['btVentVenta', 'Ventas', 2], ['btnPersonal', 'Personal', 7], ['btnProveedores', 'Proveedores', 6], ['btnCredPago', 'Cuentas Por Cobrar A Credito', 2], ['btnProforma', 'Crear Proformas', 2], ['btnGastos', 'Registrar Gastos o Ingresos', 2], ['btnCierreCajero', 'Administrar Cierre Caja', 2], ['btnCajaGeneral', 'Caja General', 2], ['btnIngresoEgresosMensuales', 'Ingresos Egresos Mensuales', 2], ['btnIngresoEgresoDetallado', 'Ingresos Egresos Detallado', 2], ['btnReporteCajaDetallado', 'Reporte Caja Detallado', 2],
        ['btVentReporteRealizada', 'Reporte De Ventas', 2], ['btVentReporteVentaVsCosto', 'Ventas Vs Costos De Productos', 2], ['btVentProdMasVendidos', 'Reporte Productos Mas Vendidos', 2],
        ['btVentRendimiento', 'Rendimiento Personal', 2], ['btnVentaProductos', 'Reporte Ventas Por Productos', 2], ['btnReporteCobranza', 'Reporte Cobranza', 2], ['btnReporteProductoCategoria', 'Ventas Por Categoria', 2], ['btInvMovimiento', 'Ingreso y Salida De Productos', 3], ['btInvAmacen', 'Almacenes', 8], ['btInvUtilidad', 'Saldos Valorados', 3], ['btInvDeposito', 'Deposito', 3], ['btInvKardex', 'Historial De Productos', 3], ['btInvSaldo', 'Reporte De Saldos Productos', 3],
        ['btInvSaldosValorados', 'Saldos Valorados', 3], ['btInvKardexReporte', 'Reporte General Historial', 3], ['btnInvMonitoreo', 'Monitoreo De Productos', 3], ['btnDespacho', 'Despachos', 4], ['btnConciliaciones', 'Conciliaciones', 4], ['btnCierreCaja', 'Cierre De Caja', 4], ['btnCierreReporteGastos', 'Reporte Gastos Chofer', 4],
        ['btnCierreRegistroIngresoEgresoGenerales', 'Registrar Gastos Ingresos Generales', 4], ['btnCierreReporteAnual', 'Reporte Ingresos Egresos Anual', 4],
        ['btnCierreReporteDetallado', 'Reporte Ingresos Egresos Detallado', 4], ['btnAsignacionZona', 'Asignacion De Zonas', 4], ['btnhistoricoCliente', 'Historico Registro Clientes', 4],
        ['btnReporteVentasPorProductos', 'Ventas Productos Por Cliente', 4], ['btnRepAuditoriaVentas', 'Auditoria Ventas', 4], ['btnhistoricoIncremento', 'Historial Incremento Ventas', 4], ['btnAsignacionPedidos', 'Asignacion De Pedidos', 2],
        ['btnCompras', 'Compras', 6], ['btnCompraCredito', 'Cuentas Por Pagar', 6], ['btnHistorico', 'Historico De Compras', 6], ['btnComprasPorProducto', 'Compras Por Producto', 6],
        ['btnConceptoFijos', 'Conceptos Fijos', 7], ['btnContratos', 'Contratos', 7], ['btnRegistrarPlanilla', 'Registrar Personal A Planilla', 7], ['btnPlanilla', 'Planilla', 7],
        // Kaily (al final para no mover las posiciones que usan los roles de prueba)
        ['btnKits', 'Gestion De Kits', 1], ['btConfDosificacion', 'Gestion De Dosificacion', 1], ['btnDespachoVenta', 'Despacho Ventas', 2],
        ['btnAuditoriaRegistros', 'Auditoria De Registros', 1], // solo web (sql/07_Auditoria.sql)
    ].map(([IdPrograma, DescripcionPrograma, Modulo], i) => ({ Id: i + 1, IdPrograma, DescripcionPrograma, Modulo })),
    roles: [
        { Id: 1, NombreRol: 'ADMINISTRADOR', FechaRegistro: hoy(), HoraRegistro: '08:00', UsuarioRegistro: 'SISTEMA' },
        { Id: 2, NombreRol: 'VENDEDOR', FechaRegistro: hoy(), HoraRegistro: '08:00', UsuarioRegistro: 'SISTEMA' },
    ],
    rolesDetalle: [],
    empresas: [
        { Id: 1, Nombre: 'DISTRIBUIDORA CENTRAL', Descripcion: 'Casa matriz', Latitud: -17.3931784, Longitud: -66.1738852, Imagen: 'Default.jpg', Propietario: 'Juan Perez', Telefono: '4441122', Nit: '1023456021', Ciudad: 'Cochabamba', Direccion: 'Av. Heroinas 123' },
        { Id: 2, Nombre: 'SUCURSAL NORTE', Descripcion: 'Zona norte', Latitud: -17.37, Longitud: -66.16, Imagen: 'Default.jpg', Propietario: 'Ana Rojas', Telefono: '4445566', Nit: '2034567012', Ciudad: 'Cochabamba', Direccion: 'Av. America 455' },
    ],
    categorias: [
        { Id: 1, NombreCategoria: 'BEBIDAS', Estado: true, DescripcionCategoria: 'Gaseosas, aguas y jugos', Imagen: 'Default.jpg', EmpresaId: 1, App: true, Empresa: 'DISTRIBUIDORA CENTRAL' },
        { Id: 2, NombreCategoria: 'LACTEOS', Estado: true, DescripcionCategoria: 'Leche, yogurt, quesos', Imagen: 'Default.jpg', EmpresaId: 1, App: true, Empresa: 'DISTRIBUIDORA CENTRAL' },
        { Id: 3, NombreCategoria: 'ABARROTES', Estado: false, DescripcionCategoria: 'Arroz, azucar, fideos', Imagen: 'Default.jpg', EmpresaId: 2, App: false, Empresa: 'SUCURSAL NORTE' },
    ],
    personal: [
        [1, 'MARCO MAMANI', 1], [2, 'LUIS QUISPE', 1], [3, 'ROSA CHOQUE', 1], [4, 'CARLOS VARGAS', 2], [5, 'ANA FLORES', 1],
    ].map(([Id, Nombre, tp]) => ({
        Id, Nombre, NombrePersonal: Nombre, Direccion: `Calle ${Id}`, Telefono01: `7000000${Id}`, TipoDocumento: 1, NroDocumento: String(5000000 + Id),
        TipoPersonal: tp, Tipo: tp === 1 ? 'VENDEDOR' : 'CHOFER', Estado: true, EmpresaId: 1, FechaNacimiento: new Date(Date.UTC(1990, Id, 10)),
    })),
    usuarios: [
        { Id: 1, NombreUsuario: 'ADMIN', Contrasena: '123', estado: true, RolId: 1, NombreRol: 'ADMINISTRADOR', SucursalId: 1, IdEmpresa: 1, Empresa: 'DISTRIBUIDORA CENTRAL', IdPersonal: 1, pedido: false, Personal: 'MARCO MAMANI', EliminarRegistros: 1 },
        { Id: 2, NombreUsuario: 'LQUISPE', Contrasena: 'ABC', estado: true, RolId: 2, NombreRol: 'VENDEDOR', SucursalId: 1, IdEmpresa: 2, Empresa: 'SUCURSAL NORTE', IdPersonal: 2, pedido: true, Personal: 'LUIS QUISPE', EliminarRegistros: 0 },
    ],
    zonas: [
        { Id: 1, NombreZona: 'CENTRO', DescripcionZona: 'Casco viejo', Color: '#2C6AA8' },
        { Id: 2, NombreZona: 'NORTE', DescripcionZona: 'Av. America y alrededores', Color: '#15803D' },
    ],
    puntos: [
        [1, -17.389, -66.162], [1, -17.389, -66.150], [1, -17.398, -66.150], [1, -17.398, -66.162],
        [2, -17.370, -66.165], [2, -17.365, -66.150], [2, -17.378, -66.148],
    ].map(([zona, lat, lng], i) => ({ zbnumi: i + 1, zbzona: zona, zblat: lat, zblongi: lng })),
    preciosCat: [
        { Id: 1, Descripcion: 'PRECIO COSTO' }, { Id: 2, Descripcion: 'PRECIO VENTA' }, { Id: 3, Descripcion: 'MAYORISTA' },
    ],
    clientes: [],
    proveedores: [
        { Id: 1, NombreProveedor: 'EMBOL S.A.', Direccion: 'Parque Industrial', Telefono01: '4401010', Telefono02: '', Descripcion: 'Bebidas', TipoDocumento: 2, NroDocumento: '1020304025', Estado: true },
        { Id: 2, NombreProveedor: 'PIL ANDINA', Direccion: 'Av. Blanco Galindo km 10', Telefono01: '4402020', Telefono02: '70707070', Descripcion: 'Lacteos', TipoDocumento: 2, NroDocumento: '1020304033', Estado: true },
    ],
    productos: [],
    imagenes: [],
    catPrecio: [
        { ygnumi: 1, ygcod: 'A', ygdesc: 'PRECIO COSTO', ygpcv: 0, estado: 'LETRA' },
        { ygnumi: 2, ygcod: 'B', ygdesc: 'PRECIO VENTA', ygpcv: 1, estado: 'LETRA' },
        { ygnumi: 3, ygcod: 'C', ygdesc: 'MAYORISTA', ygpcv: 1, estado: 'LETRA' },
    ],
    almacenes: [{ aanumi: 1, aabdes: 'ALMACEN CENTRAL' }, { aanumi: 2, aabdes: 'ALMACEN NORTE' }],
    precios: [], // { almacen, yhprod, yhcatpre, yhprecio }
};

// Auditoria (MAM_Auditoria): una venta modificada (cantidad, item agregado y quitado) y una compra eliminada
const _ahora = new Date(Date.now() - 4 * 3600 * 1000);
db.auditoria = [
    { Id: 1, Fecha: _ahora, Modulo: 1, Operacion: 1, DocumentoId: 50, FechaDocumento: new Date(hoy() + 'T00:00:00Z'), Descripcion: 'Cliente: TIENDA DON PEPE', TotalAntes: 30, TotalDespues: 27.5,
        Usuario: 'MARCO MAMANI', Equipo: 'CAJA 1 (DESKTOP-49S3GJO)', Aplicacion: 'MiventSoftWeb',
        Ip: '192.168.100.22 | 166.114.162.240', Navegador: 'Edge 131 · Windows 10/11', Latitud: -16.495521, Longitud: -68.133614, PrecisionMetros: 18,
        Antes: `<foto><cab Id="50" Fecha="${hoy()}" Cliente="TIENDA DON PEPE" TipoVenta="Contado" Glosa="A &amp; B" Descuento="0.00" Total="30.00" Facturado="0"/><det ProductoId="1" Producto="COCA COLA 2L" Cantidad="2.00" Precio="10.00" Total="20.00"/><det ProductoId="2" Producto="PIL 1L" Cantidad="1.00" Precio="10.00" Total="10.00"/></foto>`,
        Despues: `<foto><cab Id="50" Fecha="${hoy()}" Cliente="TIENDA DON PEPE" TipoVenta="Contado" Glosa="A &amp; B" Descuento="0.00" Total="27.50" Facturado="0"/><det ProductoId="1" Producto="COCA COLA 2L" Cantidad="1.00" Precio="10.00" Total="10.00"/><det ProductoId="3" Producto="AGUA 600" Cantidad="5.00" Precio="3.50" Total="17.50"/></foto>` },
    { Id: 2, Fecha: _ahora, Modulo: 2, Operacion: 2, DocumentoId: 7, FechaDocumento: new Date(hoy() + 'T00:00:00Z'), Descripcion: 'Proveedor: EMBOL S.A.', TotalAntes: 100, TotalDespues: null,
        Usuario: 'LUIS QUISPE', Equipo: 'PC-ALMACEN', Aplicacion: '.Net SqlClient Data Provider', Ip: '192.168.100.40', Navegador: 'Escritorio (.Net SqlClient Data Provider)',
        Antes: '<foto><cab Id="7" Proveedor="EMBOL S.A." Total="100.00"/><det ProductoId="1" Producto="COCA COLA 2L" Cantidad="10.00" PrecioCosto="10.00" Total="100.00"/></foto>', Despues: null },
];

let _sec = 100;
db.programas.forEach((p) => db.rolesDetalle.push({ Id: _sec++, RolId: 1, ProgramaId: p.Id, Ver: 1, Insertar: 1, Modificar: 1, Eliminar: 1 }));
// VENDEDOR: Clientes con Ver + Modificar (se respetan las acciones marcadas); Productos solo Ver (= todo, como el escritorio)
db.programas.slice(4, 6).forEach((p, i) => db.rolesDetalle.push({ Id: _sec++, RolId: 2, ProgramaId: p.Id, Ver: 1, Insertar: 0, Modificar: i === 0 ? 1 : 0, Eliminar: 0 }));

const nombresCli = ['TIENDA DONA ROSA', 'MINIMARKET EL SOL', 'ABARROTES LA PAZ', 'KIOSKO SAN MARTIN', 'MERCADO CALATAYUD P-12', 'SUPER NORTE', 'DISTRIBUIDORA TUPAC', 'TIENDA JUANITA',
    ...['TIENDA', 'MINIMARKET', 'KIOSKO', 'ABARROTES', 'SURTIDO'].flatMap((t, k) => ['LA ESQUINA', 'EL PROGRESO', 'SANTA ANA', 'LOS PINOS', 'EL CARMEN', 'SAN JOSE'].map((n) => `${t} ${n}${k ? '' : ' 2'}`))];
nombresCli.forEach((n, i) =>
    db.clientes.push({
        Id: i + 1, CodigoExterno: `C-${100 + i}`, NombreCliente: n, DireccionCliente: `Calle ${i + 1} #${100 + i * 7}`, Telefono: `7${String(1000000 + i * 4321).slice(0, 7)}`,
        TipoDocumento: 1, NroDocumento: String(4000000 + i * 1111), RazonSocial: n, Nit: String(1000000 + i), Estado: i !== 3, imgEstado: null,
        FechaIngreso: '2026-01-15T00:00:00.000Z', FechaUltimaVenta: '2026-10-30T00:00:00.000Z', ImagenCliente: 'Default.jpg',
        Latitud: i % 4 === 2 ? 0 : (i % 4 === 3 ? null : -19.58 + i * 0.0012), Longitud: i % 4 === 2 ? 0 : (i % 4 === 3 ? null : -65.757 + i * 0.0009), PrecioCategoriaId: 2, ZonaId: (i % 2) + 1, TipoNegocio: (i % 3) + 1, Referencia: 'Frente a la plaza',
    })
);
[['COCA COLA 2L', 1, 1, 1], ['SPRITE 2L', 1, 1, 1], ['LECHE PIL 1L', 2, 2, 2], ['YOGURT FRUTILLA 1L', 2, 2, 2], ['ARROZ GRANO DE ORO 1KG', 3, 2, 0]].forEach(([n, cat, prov, marca], i) =>
    db.productos.push({
        Id: i + 1, CodigoExterno: `P-${200 + i}`, CodigoBarras: `77${String(1000000000 + i * 7919)}`, NombreProducto: n, DescripcionProducto: '', StockMinimo: 10 + i,
        estado: true, imgEstado: null, CategoriaId: cat, NombreCategoria: nombreDe(db.categorias, cat, 'NombreCategoria'), EmpresaId: 1, Empresa: 'DISTRIBUIDORA CENTRAL',
        ProveedorId: prov, MarcaId: marca, AttributoId: 1, FamiliaId: cat === 2 ? 2 : 1, UnidadVentaId: 1, UnidadMaximaId: 1, Conversion: 6, FechaRegistro: hoy(), UsuarioRegistro: 'SISTEMA',
    })
);
db.productos.forEach((p) => db.catPrecio.forEach((c) => {
    if (c.ygnumi !== 3 || p.Id < 3) db.precios.push({ almacen: 1, yhprod: p.Id, yhcatpre: c.ygnumi, yhprecio: Number((5 + p.Id * 1.5 + c.ygnumi).toFixed(2)) });
}));

// ---- Ventas simuladas (60 dias) para el panel principal ----
const VENDEDORES = [[1, 'MARCO MAMANI'], [2, 'LUIS QUISPE'], [3, 'ROSA CHOQUE'], [4, 'CARLOS VARGAS'], [5, 'ANA FLORES']];
const ESTADOS = ['ENTREGADO', 'ENTREGADO', 'ENTREGADO', 'PENDIENTE', 'ASIGNADO', 'ANULADO'];
db.ventas = [];
(function generarVentas() {
    let semilla = 7;
    const azar = () => { semilla = (semilla * 9301 + 49297) % 233280; return semilla / 233280; };
    const hoy = new Date(Date.now() - 4 * 3600 * 1000);
    let id = 1;
    for (let d = 59; d >= 0; d--) {
        const f = new Date(Date.UTC(hoy.getUTCFullYear(), hoy.getUTCMonth(), hoy.getUTCDate() - d));
        if (f.getUTCDay() === 0) continue; // domingo sin ventas
        const n = 18 + Math.floor(azar() * 22);
        for (let i = 0; i < n; i++) {
            const pico = azar() < 0.5 ? 9 + azar() * 3 : 15 + azar() * 3;
            const hora = Math.min(19, Math.max(7, Math.round(pico + (azar() - 0.5) * 3)));
            const [pid, pnom] = VENDEDORES[Math.floor(azar() * azar() * VENDEDORES.length)];
            const cli = db.clientes[Math.floor(azar() * db.clientes.length)];
            const credito = azar() < 0.3;
            db.ventas.push({ Id: id++, SucursalId: 1, FechaVenta: f, PersonalId: pid, Personal: pnom, TipoVenta: credito ? 0 : 1, NombreCliente: cli.NombreCliente, TVenta: credito ? 'CREDITO' : 'CONTADO',
                ClienteId: cli.Id, Estado: 1, TotalVenta: Math.round((80 + azar() * azar() * 1400) * 100) / 100, EstadoPedido: 1, NombreEstado: d === 0 ? ESTADOS[Math.floor(azar() * ESTADOS.length)] : (azar() < 0.05 ? 'ANULADO' : 'ENTREGADO'),
                HoraRegistro: `${String(hora).padStart(2, '0')}:${String(Math.floor(azar() * 60)).padStart(2, '0')}:00`,
                conciliacionId: d > 7 ? 12 : 0, EstadoConciliacion: d > 7 ? 0 : 1, Glosa: '', Descuento: 0, VentaDirecta: 0, VentaDirectaSinConciliacion: 0,
                FechaVencimientoCredito: f, FechaEntrega: f, NombrePersonal: '', IncrementoVenta: 0 });
        }
    }
})();
const ESTADO_ID = { PENDIENTE: 1, ASIGNADO: 2, ANULADO: 3, ENTREGADO: 4 };
db.ventas.forEach((x) => { x.EstadoPedido = ESTADO_ID[x.NombreEstado] || 1; });
db.estados = [{ id: 1, NombreEstado: 'PENDIENTE' }, { id: 2, NombreEstado: 'ASIGNADO' }, { id: 3, NombreEstado: 'ANULADO' }, { id: 4, NombreEstado: 'ENTREGADO' }];
db.stock = { 1: 600, 2: 300, 3: 120, 4: 48, 5: 0 }; // unidades
db.ventasDet = [];
// Detalle de las ventas de hoy (para ver la nota de venta completa)
db.ventas.filter((vt) => vt.FechaVenta.toISOString().slice(0, 10) === hoy()).forEach((vt) => {
    const lineas = [[1, 3, 24], [3, 15, 13.33], [4, 2, 31.5]];
    lineas.forEach(([pid, cant, pr], k) => {
        const sub = Math.round(cant * pr * 100) / 100;
        db.ventasDet.push({ Id: db.ventasDet.length + 1, VentaId: vt.Id, ProductoId: pid, Cantidad: cant, Precio: pr, SubTotal: sub, ProcentajeDescuento: 0, MontoDescuento: 0, Total: sub, CantidadUnitaria: cant, PrecioCosto: pr * 0.8, Conversion: 6, Tipo: 1, KitId: 0, KitNombre: '', CantidadKit: 0 });
    });
    vt.TotalVenta = Math.round(db.ventasDet.filter((d) => d.VentaId === vt.Id).reduce((x, d) => x + d.Total, 0) * 100) / 100;
    vt.IncrementoVenta = 1; vt.Glosa = 'Entregar por la tarde';
});
db.tipoCambio = 6.96; // Kaily: Global_TipoCambio (tabla TipoCambio)
db.despachoVentas = []; // Kaily: tabla Despacho (FrmDespacho, MAM_Ventas @tipo 11)
// Kaily: MAM_Dosificacion (llave de prueba, no real)
db.dosificaciones = [{ Id: 1, SucursalId: 1, NumeroAutorizacion: '29040011007', UltimoNroFactura: 120, Llave: 'LLAVE-DE-PRUEBA', FechaInicio: new Date(Date.UTC(2026, 0, 1)),
    FechaLimite: new Date(Date.UTC(2026, 11, 31)), Nota01: 'Nota 1', Nota02: '', Estado: true, TipoComputarizado: true }];
db.proformas = [];
db.proformasDet = [];
db.pagosCxc = [];
db.incremento = [{ Id: 1, Fecha: new Date(hoy() + 'T00:00:00Z'), Monto: 2.5, Usuario: 'ADMIN', FechaHoraModificacion: new Date() }];
db.conciliacionAbierta = new Set([1]); // solo MARCO MAMANI tiene conciliacion abierta
db.conciliaciones = [{ id: 12, conciliacion: 'CONC-12 CERRADA', ChoferId: 4, Estado: 0 }, { id: 13, conciliacion: 'CONC-13 ABIERTA', ChoferId: 4, Estado: 1 }];
// pedidos asignados / entregados al chofer CARLOS VARGAS (Id 4) para Asignacion de pedidos
db.ventas.filter((x) => x.NombreEstado === 'ASIGNADO').forEach((x) => { x.ChoferId = 4; });
db.ventas.filter((x) => x.NombreEstado === 'ENTREGADO').slice(-6).forEach((x, i) => { x.ChoferId = 4; x.conciliacionId = i < 3 ? 12 : 13; x.EstadoConciliacion = i < 3 ? 0 : 1; });
db.ventas.filter((x) => x.NombreEstado === 'ANULADO').forEach((x) => { x.FechaAnulacion = x.FechaVenta; });
db.conciliacionAbierta.add(4);
// filas de las grillas de asignacion (orden de columnas del SELECT simulado; detalle = icono binario)
const filaAsig = (x) => ({ Id: x.Id, FechaPedido: x.FechaVenta, NombreCliente: x.NombreCliente, NombrePersonal: x.Personal, totalPedido: x.TotalVenta, Asignar: false, detalle: Buffer.alloc(0), PersonalId: x.PersonalId });
const conc = (id) => db.conciliaciones.find((c) => c.id === id) || {};
// ---- Distribucion: conciliaciones completas y despachos ----
const diaUTC = (n) => { const d = new Date(hoy() + 'T00:00:00Z'); d.setUTCDate(d.getUTCDate() - n); return d; };
Object.assign(db.conciliaciones[0], { Fecha: diaUTC(10), PersonalId: 4, Observacion: 'RUTA NORTE', CierreCaja: 0 });
Object.assign(db.conciliaciones[1], { Fecha: diaUTC(0), PersonalId: 4, Observacion: '', CierreCaja: 0 });
db.conciliaciones.push({ id: 11, conciliacion: 'CONC-11 CON CIERRE', ChoferId: 4, Estado: 0, Fecha: diaUTC(0), PersonalId: 4, Observacion: 'CON CIERRE', CierreCaja: 7 });
db.concDet = { 12: [{ ProductoId: 1, TotalSalida: 24, TotalDevoluciones: 5, TotalEntregado: 18, Diferencia: -1, TotalDevolucionesCalculado: 6 }] };
db.despachos = [
    { Id: 1, PersonalId: 4, ConciliacionId: 13, Fecha: diaUTC(0), Detalle: 'SALIDA MAÑANA', DespachoAutomatico: 0, det: [{ Id: 1, ProductoId: 1, Cantidad: 24 }, { Id: 2, ProductoId: 2, Cantidad: 12 }] },
    { Id: 2, PersonalId: 4, ConciliacionId: 13, Fecha: diaUTC(0), Detalle: 'SALIDA TARDE', DespachoAutomatico: 0, det: [{ Id: 1, ProductoId: 1, Cantidad: 12 }] },
    { Id: 3, PersonalId: 4, ConciliacionId: 12, Fecha: diaUTC(0), Detalle: 'CONCILIACION CERRADA', DespachoAutomatico: 0, det: [{ Id: 1, ProductoId: 1, Cantidad: 24 }] },
];
db.depositos = [{ Id: 1, NombreDeposito: 'ALMACEN CENTRAL' }, { Id: 2, NombreDeposito: 'ALMACEN NORTE' }];
db.sucursales = [{ Id: 1, DepositoId: 1, NombreAlmacen: 'CENTRAL', Direccion: 'AV. AMERICA 100', Descripcion: '', Telefono: '4400000', estado: 1 }, { Id: 2, DepositoId: 2, NombreAlmacen: 'NORTE', Direccion: 'CALLE 5', Descripcion: '', Telefono: '', estado: 1 }];
db.tiposMovInv = [{ Id: 3, Descripcion: 'SALIDA' }, { Id: 4, Descripcion: 'INGRESO' }, { Id: 7, Descripcion: 'TRASPASO INGRESO' }, { Id: 8, Descripcion: 'TRASPASO SALIDA' }];
db.movInv = [{ Id: 1, fdoc: diaUTC(20), concep: 4, obs: 'COMPRA INICIAL', alm: 1, det: [{ Id: 1, ProductoId: 1, Cantidad: 100 }, { Id: 2, ProductoId: 2, Cantidad: 50 }] },
    { Id: 2, fdoc: diaUTC(2), concep: 3, obs: 'MERMA', alm: 1, det: [{ Id: 1, ProductoId: 1, Cantidad: 4 }] }];
// Kaily: a.id, a.MovimientoId, a.ProductoId, Producto, a.Cantidad, a.Lote, a.FechaVencimiento, img, estado, stock (Tec_Movimientos.vb)
const COLS_MDET = ['Id', 'MovimientoId', 'ProductoId', 'Producto', 'Cantidad', 'Lote', 'FechaVencimiento', 'img', 'estado', 'stock'];
const kardexDe = (pid, dep) => db.movInv.filter((m) => m.alm === dep).flatMap((m) => m.det.filter((d) => d.ProductoId === pid).map((d) => ({ id: m.Id, fdoc: m.fdoc, concep: m.concep === 4 ? 1 : 2, descConcep: m.concep === 4 ? 'INGRESO' : 'SALIDA', obs: m.obs, // como el SP: 1 = Factor 1, 2 = Factor -1
    Lote: '', FechaVenc: null, est: 1, alm: dep, id2: 0, cprod: pid, descProd: nombreDe(db.productos, pid, 'NombreProducto'), entrada: m.concep === 4 ? d.Cantidad : 0, salida: m.concep === 4 ? 0 : -d.Cantidad, saldo: 0, fechaDocumento: m.fdoc })));
db.cajas = [{ Id: 1, NombreCaja: 'CAJA GENERAL' }, { Id: 2, NombreCaja: 'CAJA CHICA' }, { Id: 3, NombreCaja: 'BANCO' }];
db.tiposMov = [{ Id: 1, Descripcion: 'VENTA', Tipo: 1 }, { Id: 2, Descripcion: 'PAGO SERVICIOS', Tipo: 0 }, { Id: 9, Descripcion: 'TRASPASO', Tipo: 0 }, { Id: 10, Descripcion: 'INGRESO POR TRASPASO', Tipo: 1 }];
db.cierreCajero = [{ Id: 1, SucursalId: 1, EstadoCaja: 1, Fecha: diaUTC(0) }];
db.movCaja = [
    { Id: 1, CierreCajeroId: 0, Fecha: diaUTC(0), Descripcion: 'VENTA CONTADO', Monto: 500, CajaId: 1, IngresoEgreso: 1, CajatipoMovimientoId: 1, PersonalId: 1, SucursalId: 1, CajaIngresoEgresoIdDestino: 0, CajaIdDestino: 0 },
    { Id: 2, CierreCajeroId: 3, Fecha: diaUTC(40), Descripcion: 'LUZ', Monto: 120, CajaId: 1, IngresoEgreso: 0, CajatipoMovimientoId: 2, PersonalId: 1, SucursalId: 1, CajaIngresoEgresoIdDestino: 0, CajaIdDestino: 0 },
];
db.zonasAsig = { 4: new Set([1]) };
db.cierres = [{ Id: 7, NroConciliacion: 11, PersonalId: 4, Detalle: '', FechaCierre: diaUTC(0), TotalGastos: 50, VentasCreditos: 200, VentasContado: 900, TotalCobros: 100, EfectivoCalculado: 950, EfectivoRecibido: 940, Diferencia: -10,
    gastos: [{ Id: 1, ConceptoGastoId: 1, Detalle: 'COMBUSTIBLE', Descripcion: 'GASOLINA', Monto: 50 }], cobros: [] }];
const filaMov = (m) => ({ ...m, NombreCaja: nombreDe(db.cajas, m.CajaId, 'NombreCaja'), Movimiento: m.IngresoEgreso ? 'INGRESO' : 'EGRESO', NombreTipoMovimiento: nombreDe(db.tiposMov, m.CajatipoMovimientoId, 'Descripcion'),
    NombrePersonal: nombreDe(db.personal, m.PersonalId, 'Nombre'), Modulo: 0, IdModulo: 0, Sucursal: 'CENTRAL' });
const COLS_DDET = ['Id', 'DespachoProductosId', 'ProductoId', 'NombreProducto', 'Cantidad', 'CantidadCaja', 'Stock', 'estado', 'conversion', 'img'];
const convDe = (pid) => (db.productos.find((x) => x.Id === pid) || {}).Conversion || 6;
const filaDDet = (d, desp) => ({ Id: d.Id, DespachoProductosId: desp, ProductoId: d.ProductoId, NombreProducto: nombreDe(db.productos, d.ProductoId, 'NombreProducto'), Cantidad: d.Cantidad,
    CantidadCaja: d.Cantidad / convDe(d.ProductoId), Stock: (db.stock[d.ProductoId] || 0) + d.Cantidad, estado: 1, conversion: convDe(d.ProductoId), img: null });
db.cxc = db.clientes.slice(0, 14).map((c, i) => ({ Id: i + 1, FechaVencimientoCredito: null, DiasMora: [0, 0, 5, 12, 25, 34, 0, 45, 70, 0, 3, 90, 0, 15][i], Credito: i + 1, venta: 100 + i, Nombrecliente: c.NombreCliente, NombrePersonal: VENDEDORES[i % 5][1], Monto: 500 + i * 230, abonado: i * 40, Restante: 500 + i * 190 }));
[15, 16].forEach((Id) => db.cxc.push({ Id, DiasMora: 0, Credito: Id, venta: 100 + Id, Nombrecliente: db.clientes[Id].NombreCliente, NombrePersonal: 'LUIS QUISPE', Monto: 300, abonado: 300, Restante: 0 }));
// mas creditos pagados, de ventas de distintas fechas (para el filtro por fecha de venta)
[60, 220, 410, 640, 820, 990, db.ventas[db.ventas.length - 3].Id].forEach((venta, i) => { const Id = 17 + i; db.cxc.push({ Id, DiasMora: 0, Credito: Id, venta, Nombrecliente: db.clientes[(Id * 3) % db.clientes.length].NombreCliente, NombrePersonal: VENDEDORES[i % 5][1], Monto: 150 + i * 75, abonado: 150 + i * 75, Restante: 0 }); });
// Kaily: los creditos traen PersonalId (filtro por vendedor de Tec_AdministrarCuentasPorCobrar)
db.cxc.forEach((c) => { c.PersonalId = (VENDEDORES.find(([, n]) => n === c.NombrePersonal) || [0])[0]; });
// Compras de Kaily: cabecera + detalle (columnas del SELECT de MAM_Compras @tipo 4, como _prAddDetalleVenta de Tec_ComprasDetalle.vb)
const COLS_CDET = ['Id', 'CompraId', 'ProductoId', 'Producto', 'CantidadCompra', 'PorcentajeIncremento', 'CantidadIncremento', 'Cantidad', 'PrecioCosto', 'Lote', 'FechaVencimiento',
    'TotalCompra', 'PrecioVenta', 'estado', 'img', 'costo', 'venta'];
db.compras = [1, 2, 3].map((i) => ({ Id: i, AlmacenId: 1, ProveedorId: i === 3 ? 2 : 1, FechaTransaccion: diaUTC(i * 3),
    TipoVenta: i === 2 ? 0 : 1, FechaVencimientoCredito: diaUTC(i * 3 - 30), Moneda: 1, TituloMoneda: 'BOLIVIANOS', Estado: 1, Glosa: `COMPRA DE PRUEBA ${i}`, TotalCompra: 0, descuento: 0, EmpresaId: 1, transaccion: i === 2 ? 1 : 0 }));
db.comprasDet = [];
// como la base real: "Credito0001" / "Compra0050"; cada compra a credito con su detalle (MAM_Compras @tipo 4)
db.cxp = db.proveedores.concat([{ Id: 4, NombreProveedor: 'MOLINO ANDINO' }]).map((p, i) => ({ Id: i + 1, DiasMora: [0, 20, 40, 75][i], Credito: `Credito${String(i + 1).padStart(4, '0')}`, Compra: `Compra${String(50 + i).padStart(4, '0')}`, NombreProveedor: p.NombreProveedor, Monto: 9000 - i * 1500, abonado: 1000, Restante: 8000 - i * 1500 }));
db.cxp.forEach((c, i) => (i === 0 ? Array.from({ length: 24 }, (_, k) => k + 1) : [1, 2]).forEach((pid, j) => db.comprasDet.push({ Id: 900 + i * 2 + j, CompraId: 50 + i, ProductoId: pid, Producto: ['COCA COLA 2L', 'SPRITE 2L'][j] || `PRODUCTO DE PRUEBA ${pid}`,
    CantidadCompra: 120 + j * 60, PorcentajeIncremento: 0, CantidadIncremento: 0, Cantidad: 120 + j * 60, Lote: '', FechaVencimiento: null,
    PrecioCosto: 7 + j, PrecioVenta: 8.5 + j, TotalCompra: (120 + j * 60) * (7 + j), img: null, estado: 1, costo: 0, venta: 0 })));
// Planilla
db.conceptosFijos = [{ Id: 1, NombreConcepto: 'AFP', Porcentaje: 12.71, OperacionId: 2, Tipo: 1, Estado: 1 }, { Id: 2, NombreConcepto: 'BONO ANTIGUEDAD', Porcentaje: 5, OperacionId: 1, Tipo: 1, Estado: 1 }, { Id: 3, NombreConcepto: 'ANTICIPO', Porcentaje: 0, OperacionId: 2, Tipo: 0, Estado: 1 }];
db.operacionesCF = [{ Id: 1, NombreOperacion: 'SUMA', Operacion: 1 }, { Id: 2, NombreOperacion: 'RESTA', Operacion: -1 }];
db.contratos = [1, 2, 3].map((i) => ({ Id: i, PersonalId: i, TipoContratoID: i === 3 ? 2 : 1, Cargo: i, SueldoBase: 3000 + i * 500, InicioContrato: diaUTC(400 * i), FinContrato: i === 3 ? new Date(Date.UTC(2075, 0, 1)) : diaUTC(-200), Estado: 1, Indefinido: i === 3 ? 1 : 0 }));
db.contratoConceptos = [{ Id: 1, ContratoId: 1, ConceptoId: 1 }, { Id: 2, ContratoId: 1, ConceptoId: 2 }, { Id: 3, ContratoId: 2, ConceptoId: 1 }];
db.planillas = [];
db.planillaConceptos = [];
db.pagosCxp = [];
db.compras.forEach((c) => [1, 2].forEach((k) => {
    const pid = c.Id + k; const conv = convDe(pid); const cant = conv * (2 + k); const pc = 30 + pid;
    db.comprasDet.push({ Id: db.comprasDet.length + 1, CompraId: c.Id, ProductoId: pid, Producto: nombreDe(db.productos, pid, 'NombreProducto'), Cantidad: cant, CantidadCajas: cant / conv, Lote: '20200101',
        FechaVencimiento: new Date(Date.UTC(2020, 0, 1)), PrecioCosto: pc, PrecioVenta: pc * 1.2, TotalCompra: pc * (cant / conv), img: null, estado: 1, costo: pc, venta: pc * 1.2, conversion: conv });
    c.TotalCompra += pc * (cant / conv);
}));

const P = (lista) => Object.fromEntries(lista.map((p) => [p.nombre.replace('@', '').toLowerCase(), p]));
const COLS_IMG = ['lhnumi', 'lhprod', 'lhima', 'img', 'estado'];
const COLS_PUNTO = ['zbnumi', 'zbzona', 'zblat', 'zblongi', 'estado'];
const COLS_PRECIO = ['yhnumi', 'yhprod', 'yhcatpre', 'yhprecio', 'ygcod', 'estado'];

function detalleRol(rolId) {
    return db.programas.map((p) => {
        const d = db.rolesDetalle.find((x) => x.RolId === rolId && x.ProgramaId === p.Id);
        return {
            Id: d ? d.Id : 0, RolId: rolId, ProgramaId: p.Id, DescripcionPrograma: p.DescripcionPrograma, Modulo: p.Modulo,
            Ver: d ? d.Ver : 0, Insertar: d ? d.Insertar : 0, Modificar: d ? d.Modificar : 0, Eliminar: d ? d.Eliminar : 0, estado: d ? 1 : 0,
        };
    });
}

function matrizPrecios(almacen) {
    const filas = [];
    let n = 1;
    for (const p of db.productos) for (const c of db.catPrecio) {
        const x = db.precios.find((y) => y.almacen === almacen && y.yhprod === p.Id && y.yhcatpre === c.ygnumi);
        filas.push({ yhnumi: n++, yhprod: p.Id, yhcatpre: c.ygnumi, yhprecio: x ? x.yhprecio : 0, ygcod: c.ygcod, estado: x ? 1 : 0 });
    }
    return tabla(filas, COLS_PRECIO);
}

function detalleVenta(ventaId, det = 'ventasDet') {
    return db[det].filter((d) => d.VentaId === ventaId).map((d) => ({
        Id: d.Id, VentaId: d.VentaId, ProductoId: d.ProductoId, Producto: nombreDe(db.productos, d.ProductoId, 'NombreProducto'), Cantidad: d.Cantidad, Precio: d.Precio,
        SubTotal: d.SubTotal, ProcentajeDescuento: d.ProcentajeDescuento, MontoDescuento: d.MontoDescuento, Total: d.Total, Detalle: '',
        PrecioCosto: d.PrecioCosto, Lote: '20200101', FechaVencimiento: new Date(Date.UTC(2020, 0, 1)), estado: 1,
        TipoNombre: Number(d.Tipo) === 2 ? 'Kits' : 'Productos', Tipo: Number(d.Tipo) || 1, KitNombre: d.KitNombre || '', KitId: Number(d.KitId) || 0, CantidadKit: Number(d.CantidadKit) || 0,
        img: null, stock: (db.stock[d.ProductoId] || 0) + (d.CantidadUnitaria ?? d.Cantidad),
    }));
}

// Kaily: MAM_Ventas @tipo 5 (@SucursalId, @CategoriaVentaId). Precio por unidad. Tipo 1 = producto, Tipo 2 = kit
db.kits = [{ Id: 90, NombreProducto: 'COMBO FIESTA', comps: [{ ProductoId: 1, Cantidad: 2, PrecioVenta: 10 }, { ProductoId: 3, Cantidad: 1, PrecioVenta: 5 }] }];
function productosVentaKaily(sucursal, categoria) {
    const pr = (pid, c) => (db.precios.find((y) => y.almacen === 1 && y.yhprod === pid && y.yhcatpre === c) || {}).yhprecio || 0;
    const prods = db.productos.filter((x) => x.estado).map((x) => ({
        Id: x.Id, Tipo: 1, CodigoExterno: x.CodigoExterno, estado: 1, NombreProducto: x.NombreProducto, DescripcionProducto: x.DescripcionProducto, NombreTipo: 'PRODUCTO',
        NombreCategoria: x.NombreCategoria, industria: '', PrecioCosto: pr(x.Id, 1), stock: sucursal === 1 ? (db.stock[x.Id] || 0) : 0, PrecioVenta: pr(x.Id, categoria) || pr(x.Id, 2),
    }));
    const kits = db.kits.map((k) => ({ Id: k.Id, Tipo: 2, CodigoExterno: `K${k.Id}`, estado: 1, NombreProducto: k.NombreProducto, DescripcionProducto: k.NombreProducto, NombreTipo: 'KIT',
        NombreCategoria: 'KITS', industria: '', PrecioCosto: 0, stock: sucursal === 1 ? Math.min(...k.comps.map((c) => Math.floor((db.stock[c.ProductoId] || 0) / c.Cantidad))) : 0,
        PrecioVenta: k.comps.reduce((s, c) => s + c.PrecioVenta * c.Cantidad, 0) }));
    return [...prods, ...kits];
}

function productosVenta(sucursal, precioId) {
    return db.productos.filter((x) => x.estado).map((x) => {
        const pr = (c) => (db.precios.find((y) => y.almacen === 1 && y.yhprod === x.Id && y.yhcatpre === c) || {}).yhprecio || 0;
        const st = sucursal === 1 ? (db.stock[x.Id] || 0) : 0;
        return { Id: x.Id, CodigoExterno: x.CodigoExterno, NombreProducto: x.NombreProducto, DescripcionProducto: x.DescripcionProducto, NombreCategoria: x.NombreCategoria,
            Conversion: x.Conversion, PrecioCosto: pr(1) * x.Conversion, stock: st, stockCajas: st / x.Conversion, PrecioVenta: pr(precioId) * x.Conversion, estado: 1 };
    });
}

// Simula el SP: aplica el detalle segun "estado" (0 inserta, 2 modifica, -1 elimina, 1 y -2 se ignoran)
function grabarVenta(tipo, id, v, detalle, dest = { cab: 'ventas', det: 'ventasDet', stock: true }, detPagos = null) {
    const col = (f, n) => f[detalle.columnas.indexOf(n)];
    const filas = detalle.filas.map((f) => (Array.isArray(f) ? f : detalle.columnas.map((c) => f[c])));
    let Id = id;
    const cli = db.clientes.find((c) => c.Id === Number(v('ClienteId')));
    const cab = {
        SucursalId: Number(v('SucursalId')), FechaVenta: new Date(String(v('FechaVenta')).replace(/\//g, '-') + 'T00:00:00Z'), PersonalId: Number(v('PersonalId')),
        Personal: nombreDe(db.personal, v('PersonalId'), 'Nombre'), TipoVenta: Number(v('TipoVenta')), TVenta: Number(v('TipoVenta')) === 1 ? 'CONTADO' : 'CREDITO',
        FechaVencimientoCredito: new Date(String(v('FechaVencimientoCredito')).replace(/\//g, '-') + 'T00:00:00Z'), ClienteId: Number(v('ClienteId')), NombreCliente: cli ? cli.NombreCliente : '',
        Glosa: v('Glosa'), TotalVenta: Number(v('TotalVenta')), Descuento: Number(v('Descuento')), EstadoPedido: Number(v('EstadoPedido')) || 4,
        NombreEstado: (db.estados.find((e) => e.id === (Number(v('EstadoPedido')) || 4)) || {}).NombreEstado || '', VentaDirecta: Number(v('Ventadirecta')) || 0,
        VentaDirectaSinConciliacion: Number(v('VentaDirectaSinConciliacion')) || 0,
        FechaEntrega: new Date(String(v('FechaEntregar') ?? v('FechaVenta')).replace(/\//g, '-') + 'T00:00:00Z'),
        // Kaily: @Facturado y @VentaPagos (1 fila posicional en contado)
        Facturado: Number(v('Facturado')) || 0,
    };
    const pg = detPagos?.filas?.[0];
    Object.assign(cab, pg ? { MontoBs: Number(pg[2]), MontoDolares: Number(pg[3]), TarjetaBancaria: Number(pg[4]), TransferenciaBancaria: Number(pg[5]), TipoCambio: Number(pg[6]) }
        : { MontoBs: 0, MontoDolares: 0, TarjetaBancaria: 0, TransferenciaBancaria: 0, TipoCambio: 0 });
    if (tipo === 1) {
        Id = sig(db[dest.cab]);
        const h = new Date(Date.now() - 4 * 3600 * 1000);
        db[dest.cab].push({ Id, ...cab, Estado: 1, MonedaVenta: 1, HoraRegistro: `${String(h.getUTCHours()).padStart(2, '0')}:${String(h.getUTCMinutes()).padStart(2, '0')}:00`,
            NombrePersonal: '', EstadoConciliacion: 1, conciliacionId: 0, IncrementoVenta: Number(v('IncrementoVenta')) });
    } else {
        Object.assign(db[dest.cab].find((x) => x.Id === Id), cab);
    }
    for (const f of filas) {
        const est = Number(col(f, 'estado'));
        // Kaily no tiene CantidadUnitaria: la Cantidad ya es en unidades
        const unid = detalle.columnas.includes('CantidadUnitaria') ? Number(col(f, 'CantidadUnitaria')) : Number(col(f, 'Cantidad'));
        const reg = { ProductoId: Number(col(f, 'ProductoId')), Cantidad: Number(col(f, 'Cantidad')), CantidadUnitaria: unid, Precio: Number(col(f, 'Precio')),
            SubTotal: Number(col(f, 'SubTotal')), ProcentajeDescuento: Number(col(f, 'ProcentajeDescuento')), MontoDescuento: Number(col(f, 'MontoDescuento')), Total: Number(col(f, 'Total')),
            PrecioCosto: Number(col(f, 'PrecioCosto')), Conversion: Number(col(f, 'Conversion')) || 1,
            Tipo: Number(col(f, 'Tipo')) || 1, KitId: Number(col(f, 'KitId')) || 0, KitNombre: col(f, 'KitNombre') || '', CantidadKit: Number(col(f, 'CantidadKit')) || 0 };
        if (est === 0) { db[dest.det].push({ Id: sig(db[dest.det]), VentaId: Id, ...reg }); if (dest.stock) db.stock[reg.ProductoId] = (db.stock[reg.ProductoId] || 0) - reg.CantidadUnitaria; }
        const ant = db[dest.det].find((d) => d.Id === Number(col(f, 'Id')) && d.VentaId === Id);
        if (est === 2 && ant) { if (dest.stock) db.stock[ant.ProductoId] += ant.CantidadUnitaria - reg.CantidadUnitaria; Object.assign(ant, reg); }
        if (est === -1 && ant) { if (dest.stock) db.stock[ant.ProductoId] += ant.CantidadUnitaria; db[dest.det] = db[dest.det].filter((d) => d !== ant); }
    }
    return tabla([{ Id }]);
}

async function EjecutarProcedimiento(nombre, lista) {
    _validarContrato(nombre, lista);
    const p = P(lista);
    const tipo = Number(p.tipo?.valor);
    const v = (k) => p[k.toLowerCase()]?.valor;
    const id = Number(v('Id'));
    switch (nombre) {
        case 'MAM_Clasificadores': {
            const idc = Number(v('IdClasificador'));
            db.clasif[idc] = db.clasif[idc] || [];
            if (tipo === 3) return tabla(db.clasif[idc], ['cnnum', 'cndesc1']);
            if (tipo === 1) { const cnnum = sig(db.clasif[idc], 'cnnum'); db.clasif[idc].push({ cnnum, cndesc1: v('descripcion') }); return tabla([{ cnnum }]); }
            return tabla([]);
        }
        case 'MAM_Clientes': {
            if (tipo === 9) return tabla(db.personal);
            if (tipo === 6 || tipo === 7) return tabla(db.clientes.filter((c) => tipo === 6 || Number(c.ZonaId ?? c.Id % 3 + 1) === Number(v('ZonaId'))).map((c, i) => ({ Id: c.Id, NombreCliente: c.NombreCliente, Telefono: c.Telefono, DireccionCliente: c.DireccionCliente,
                Latitud: Number(c.Latitud) || (i % 7 === 6 ? 0 : -17.39 + (i % 10) * 0.004), Longitud: Number(c.Longitud) || (i % 7 === 6 ? 0 : -66.16 - Math.floor(i / 10) * 0.004) })));
            if (tipo === 11) return tabla(db.personal.filter((x) => x.Id === id));
            if (tipo === 3) return tabla(db.clientes);
            if (tipo === 4) return tabla(db.zonas.map((z) => ({ Id: z.Id, NombreZona: z.NombreZona })));
            if (tipo === 5) return tabla(db.preciosCat);
            if (tipo === 10) return tabla(db.clientes.map((c) => ({ Id: c.Id, NombreCliente: c.NombreCliente, DireccionCliente: c.DireccionCliente, Telefono: c.Telefono })));
            // Kaily InsertarClienteFormularioExterno (FormularioCliente): devuelve Id, NombreCliente
            if (tipo === 8) {
                const Id = sig(db.clientes);
                db.clientes.push({ Id, CodigoExterno: '', NombreCliente: v('NombreCliente'), DireccionCliente: '', Telefono: v('Telefono'), TipoDocumento: v('TipoDocumento'), NroDocumento: v('NroDocumento'),
                    RazonSocial: v('RazonSocial'), Nit: v('nit'), Estado: true, FechaIngreso: hoy(), Latitud: 0, Longitud: 0, PrecioCategoriaId: v('PrecioCategoriaId'), ZonaId: 0,
                    FechaUltimaVenta: null, ImagenCliente: 'Default.jpg', imgEstado: null });
                return tabla([{ Id, NombreCliente: v('NombreCliente') }]);
            }
            if (tipo === -1) { db.clientes = db.clientes.filter((x) => x.Id !== id); return tabla([{ Id: id }]); }
            const reg = {
                CodigoExterno: v('CodigoExterno'), NombreCliente: v('NombreCliente'), DireccionCliente: v('DireccionCliente'), Telefono: v('Telefono'),
                TipoDocumento: v('TipoDocumento'), NroDocumento: v('NroDocumento'), RazonSocial: v('RazonSocial'), Nit: v('nit'), Estado: !!v('estado'),
                FechaIngreso: String(v('FechaIngreso')).replace(/\//g, '-'), Latitud: v('Latitud'), Longitud: v('Longitud'), PrecioCategoriaId: v('PrecioCategoriaId'),
                ZonaId: v('ZonaId'), TipoNegocio: v('TipoNegocio'), Referencia: v('Referencia'),
            };
            if (tipo === 1) { const Id = sig(db.clientes); db.clientes.push({ Id, ...reg, FechaUltimaVenta: null, ImagenCliente: 'Default.jpg', imgEstado: null }); return tabla([{ Id }]); }
            if (tipo === 2) { Object.assign(db.clientes.find((x) => x.Id === id), reg); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'sp_Mam_Zonas': {
            if (tipo === 3) return tabla(db.zonas);
            if (tipo === 4) return tabla(db.puntos.filter((x) => x.zbzona === id).map((x) => ({ ...x, estado: 1 })), COLS_PUNTO);
            if (tipo === -1) { db.zonas = db.zonas.filter((x) => x.Id !== id); db.puntos = db.puntos.filter((x) => x.zbzona !== id); return tabla([{ Id: id }]); }
            let Id = id;
            const reg = { NombreZona: v('NombreZona'), DescripcionZona: v('DescripcionZona'), Color: v('Color') };
            if (tipo === 1) { Id = sig(db.zonas); db.zonas.push({ Id, ...reg }); }
            if (tipo === 2) Object.assign(db.zonas.find((x) => x.Id === Id), reg);
            // estado: 0 nuevo -> insertar; -1 -> borrar existente; 1 -> conservar; -2 -> ignorar
            for (const f of p.zonaspuntos.detalle.filas) {
                const est = Number(f[4]);
                if (est === 0) db.puntos.push({ zbnumi: sig(db.puntos, 'zbnumi'), zbzona: Id, zblat: f[2], zblongi: f[3] });
                if (est === -1) db.puntos = db.puntos.filter((x) => x.zbnumi !== f[0]);
            }
            return tabla([{ Id }]);
        }
        case 'MAM_Productos': {
            if (tipo === 3) return tabla(db.productos);
            // Kaily ListPreciosDetalles: id (categoria de precio), Descripcion, precio
            if (tipo === 10) {
                db.preciosProducto = db.preciosProducto || {};
                return tabla([{ id: 1, Descripcion: 'PRECIO COSTO' }, { id: 2, Descripcion: 'PRECIO VENTA' }]
                    .map((c) => ({ ...c, precio: (db.preciosProducto[id] || {})[c.id] ?? 0 })), ['id', 'Descripcion', 'precio']);
            }
            if (tipo === 4) return tabla(db.categorias.map((c) => ({ Id: c.Id, NombreCategoria: c.NombreCategoria })));
            if (tipo === 7) return tabla(db.proveedores);
            // Kaily GenerarReportePrecios (@tipo 8, @CategoriaPrecio, @SucursalId): Vista_ProductoPrecios
            if (tipo === 8 && v('CategoriaPrecio') !== undefined) {
                const cp = Number(v('CategoriaPrecio'));
                return tabla(db.productos.map((x) => ({ Id: x.Id, CodigoExterno: x.CodigoExterno, NombreProducto: x.NombreProducto, DescripcionProducto: x.DescripcionProducto || x.NombreProducto,
                    industria: 'BOLIVIA', unidad: 'UNIDAD', NombreCategoria: nombreDe(db.categorias, x.CategoriaId, 'NombreCategoria'), CategoriaId: x.CategoriaId, Rutaimg: `\\producto_${x.Id}.jpg`,
                    Precio: (db.precios.find((y) => y.yhprod === x.Id && y.yhcatpre === cp) || {}).yhprecio ?? 0, img: null, imgEmpresa: null })));
            }
            // Kaily ObtenerCoincidenciasCodigoExterno (@tipo 11, @CodigoExterno)
            if (tipo === 11 && v('CodigoExterno') !== undefined) {
                return tabla(db.productos.filter((x) => String(x.CodigoExterno || '').trim() === String(v('CodigoExterno'))).map((x) => ({ id: x.Id, DescripcionProducto: x.DescripcionProducto || x.NombreProducto })));
            }
            if (tipo === 8) return tabla([{ Id: -1, NombrePersonal: 'TODOS' }, ...db.personal.map((x) => ({ Id: x.Id, NombrePersonal: x.Nombre }))]);
            if (tipo === 11) return tabla([{ Id: -1, NombreSubCategoria: 'TODOS' }, { Id: 1, NombreSubCategoria: 'GASEOSAS' }, { Id: 2, NombreSubCategoria: 'LACTEOS' }]);
            if (tipo === 9 || tipo === 12) {
                const cat = Number(v('CategoriaId'));
                const prods = db.productos.filter((x) => cat === -1 || x.CategoriaId === cat);
                // detalle por linea de venta (como @tipo 9) y totalizado por vendedor + categoria (@tipo 12)
                const vend = Number(v('PersonalId'));
                const pers = db.personal.filter((x) => vend === -1 || x.Id === vend).slice(0, 3);
                const det = [];
                pers.forEach((pe, a) => prods.forEach((x, i) => [0, 1].forEach((d) => {
                    const cant = 6 * ((i + d + a) % 4 + 1); const precio = 1.5 + i * 2.25;
                    det.push({ NombrePersonal: pe.Nombre, NombreProducto: x.NombreProducto, Subcategoria: x.NombreCategoria, NombreCategoria: x.NombreCategoria,
                        FechaVenta: new Date(Date.UTC(2026, 9, 26 + d)), VentaId: 47949 + a * 40 + i * 3 + d, ProductoId: x.Id, Cantidad: cant, MontoDescuento: d && i === 1 ? 2 : 0,
                        Total: cant * precio - (d && i === 1 ? 2 : 0), Precio: precio });
                })));
                if (tipo === 12) {
                    const m = new Map();
                    det.forEach((r) => { const k = `${r.NombrePersonal}|${r.NombreCategoria}`; const x = m.get(k) || { NombrePersonal: r.NombrePersonal, NombreCategoria: r.NombreCategoria, Cantidad: 0, total: 0 }; x.Cantidad += r.Cantidad; x.total += r.Total; m.set(k, x); });
                    return tabla([...m.values()]);
                }
                return tabla(det);
            }
            if (tipo === 5) return tabla(db.imagenes.filter((x) => x.lhprod === id).map((x) => ({ ...x, img: null, estado: 1 })), COLS_IMG);
            if (tipo === -1) {
                if (id === 1) return tabla([{ res: -1, mensaje: 'EL PRODUCTO TIENE MOVIMIENTOS, NO SE PUEDE ELIMINAR' }]);
                db.productos = db.productos.filter((x) => x.Id !== id);
                return tabla([{ res: id, mensaje: '' }]);
            }
            const reg = {
                CodigoExterno: v('CodigoExterno'), CodigoBarras: v('CodigoBarras'), NombreProducto: v('NombreProducto'), DescripcionProducto: v('DescripcionProducto'),
                StockMinimo: v('StockMinimo'), estado: !!v('Estado'), CategoriaId: v('CategoriaId'), NombreCategoria: nombreDe(db.categorias, v('CategoriaId'), 'NombreCategoria'),
                EmpresaId: v('EmpresaId'), Empresa: nombreDe(db.empresas, v('EmpresaId'), 'Nombre'), ProveedorId: v('ProveedorId'), MarcaId: v('MarcaId'),
                AttributoId: v('AttributoId'), FamiliaId: v('FamiliaId'), UnidadVentaId: v('UnidadVentaId'), UnidadMaximaId: v('UnidadMaximaId'), Conversion: v('Conversion'),
            };
            let Id = id;
            if (tipo === 1) { Id = sig(db.productos); db.productos.push({ Id, ...reg, imgEstado: null, FechaRegistro: hoy(), UsuarioRegistro: '' }); }
            if (tipo === 2) Object.assign(db.productos.find((x) => x.Id === Id), reg);
            for (const f of p.tcl0064.detalle.filas) {
                const est = Number(f[4]);
                if (est === 0) db.imagenes.push({ lhnumi: sig(db.imagenes, 'lhnumi'), lhprod: Id, lhima: f[2] });
                if (est === -1) db.imagenes = db.imagenes.filter((x) => x.lhnumi !== f[0]);
            }
            // @PrecioT (Kaily): filas posicionales [id, Descripcion, precio]
            db.preciosProducto = db.preciosProducto || {};
            db.preciosProducto[Id] = Object.fromEntries((p.preciot?.detalle.filas || []).map((f) => [Number(f[0]), Number(f[2])]));
            return tabla([{ Id }]);
        }
        case 'MAM_Dosificacion': {
            // Kaily: Tec_Dosificacion (@tipo 3 lista, 1/2 grabar, -1 eliminar)
            if (tipo === 3) return tabla(db.dosificaciones.map((d) => ({ ...d, NombreAlmacen: d.SucursalId === 1 ? 'CENTRAL' : 'SUCURSAL 2' })));
            if (tipo === -1) { db.dosificaciones = db.dosificaciones.filter((x) => x.Id !== id); return tabla([{ Id: id }]); }
            const reg = { SucursalId: Number(v('SucursalId')), NumeroAutorizacion: v('NumeroAutorizacion'), UltimoNroFactura: Number(v('UltimoNroFactura')), Llave: v('Llave'),
                FechaInicio: new Date(`${String(v('FechaInicio')).replace(/\//g, '-')}T00:00:00Z`), FechaLimite: new Date(`${String(v('FechaLimite')).replace(/\//g, '-')}T00:00:00Z`),
                Nota01: v('Nota01'), Nota02: v('Nota02'), Estado: !!Number(v('Estado')), TipoComputarizado: !!Number(v('TipoComputarizado')) };
            if (tipo === 1) { const Id = sig(db.dosificaciones); db.dosificaciones.push({ Id, ...reg }); return tabla([{ Id }]); }
            if (tipo === 2) { Object.assign(db.dosificaciones.find((x) => x.Id === id), reg); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'MAM_Ventas': {
            // Kaily: InsertarTipoCambio (ActualizarTipoCambio)
            if (tipo === 7) { db.tipoCambio = Number(v('TipoCambio')); return tabla([{ Id: 1 }]); }
            if (tipo === 6) { const vr = db.ventas.find((x) => x.Id === id); const cl = vr ? db.clientes.find((c) => c.Id === vr.ClienteId) || {} : {};
                return tabla(vr ? (detalleVenta(vr.Id).length ? detalleVenta(vr.Id) : [{}]).map((d) => ({ Id: vr.Id, NombreCliente: vr.NombreCliente, Nit: cl.Nit || 'SN', Telefono: cl.Telefono || '', Direccion: cl.Direccion || '', HoraRegistro: vr.HoraRegistro || '19:18', ProductoId: d.ProductoId, CodigoExterno: d.ProductoId ? 100 + d.ProductoId : null, Producto: d.Producto, Total: d.Total, DescuentoVenta: vr.Descuento || 0, IncrementoVenta: 1, imageEmpresa: null })) : []); }
            if (tipo === 3) {
                const d = new Date(String(v('desde')).replace(/\//g, '-') + 'T00:00:00Z');
                const h = new Date(String(v('hasta')).replace(/\//g, '-') + 'T00:00:00Z');
                // Kaily: @SucursalId (Global_Sucursal, -1 = todas) y columnas de pago / cierre
                const suc = v('SucursalId') === undefined ? -1 : Number(v('SucursalId'));
                // Kaily: left join despacho (FrmDespacho). Sin despacho: isnull(...,'') -> 1900-01-01
                const desp = (x) => {
                    const dp = db.despachoVentas.find((y) => y.VentaId === x.Id);
                    return dp ? { EstadoDespacho: 1, TDespachado: 'DESPACHADO', FechaDespacho: dp.FechaDespacho, Despachante: nombreDe(db.personal, dp.PersonalId, 'Nombre'), PersonalDespachadorId: dp.PersonalId, ObservacionDespacho: dp.Observacion }
                        : { EstadoDespacho: 0, TDespachado: 'NO DESPACHADO', FechaDespacho: new Date(Date.UTC(1900, 0, 1)), Despachante: '', PersonalDespachadorId: 0, ObservacionDespacho: '' };
                };
                // como el SP real de Kaily: sin NombreEstado / EstadoPedido (no hay estados de pedido) ni HoraRegistro
                return tabla(db.ventas.filter((x) => x.FechaVenta >= d && x.FechaVenta <= h && (suc <= 0 || x.SucursalId === suc))
                    .map(({ NombreEstado, EstadoPedido, HoraRegistro, ...x }) => ({ NombreAlmacen: 'CENTRAL', Facturado: 0, MontoBs: 0, MontoDolares: 0, TarjetaBancaria: 0, TransferenciaBancaria: 0, TipoCambio: 0, CierreModulo: 0, ...x, ...desp(x) })));
            }
            // Kaily: VentaInsertarDespacho (FrmDespacho). 1 = inserta si no existe; 0 = borra
            if (tipo === 11) {
                const vid = Number(v('VentaId'));
                const existe = db.despachoVentas.some((y) => y.VentaId === vid);
                if (Number(v('Despachado')) === 1 && !existe) {
                    db.despachoVentas.push({ VentaId: vid, PersonalId: Number(v('PersonalId')), Observacion: String(v('ObservacionDespacho') ?? ''), FechaDespacho: diaUTC(0) });
                }
                if (Number(v('Despachado')) === 0) db.despachoVentas = db.despachoVentas.filter((y) => y.VentaId !== vid);
                return tabla([{ newNumi: db.despachoVentas.length }]);
            }
            // Kaily: estructura de @VentaPagos (ListaVentasDetallePago)
            if (tipo === 8) return tabla([], COLS_PAGO);
            // Kaily: productos de un kit (L_prListarProductosKitsVenta)
            if (tipo === 10) {
                const k = db.kits.find((x) => x.Id === id);
                return tabla((k ? k.comps : []).map((c) => ({ ProductoId: c.ProductoId, NombreProducto: nombreDe(db.productos, c.ProductoId, 'NombreProducto'), Cantidad: c.Cantidad, PrecioVenta: c.PrecioVenta, PrecioCosto: c.PrecioVenta * 0.7 })));
            }
            if (tipo === 5 && v('CategoriaVentaId') !== undefined) return tabla(productosVentaKaily(Number(v('SucursalId')), Number(v('CategoriaVentaId'))));
            if (tipo === 4) return tabla(detalleVenta(id), COLS_VDET);
            if (tipo === 5) return tabla(productosVenta(Number(v('SucursalId')), Number(v('PrecioId'))));
            if (tipo === -1) {
                const x = db.ventas.find((y) => y.Id === id);
                if (!x) return tabla([]);
                db.ventasDet.filter((d) => d.VentaId === id).forEach((d) => { db.stock[d.ProductoId] = (db.stock[d.ProductoId] || 0) + d.CantidadUnitaria; });
                db.ventasDet = db.ventasDet.filter((d) => d.VentaId !== id);
                db.ventas = db.ventas.filter((y) => y.Id !== id);
                return tabla([{ Id: id }]);
            }
            if (tipo === 1 || tipo === 2) return grabarVenta(tipo, id, v, p.ventadetalletype.detalle, undefined, p.ventapagos?.detalle);
            return tabla([]);
        }
        case 'MAM_Personal':
        case 'MAM_Proveedores': {
            const esPer = nombre === 'MAM_Personal';
            const arr = esPer ? db.personal : db.proveedores;
            if (tipo === 3) return tabla(arr);
            if (tipo === -1) {
                if (esPer && db.ventas.some((x) => x.PersonalId === id)) return tabla([]); // tiene ventas: no se borra
                const i = arr.findIndex((x) => x.Id === id); if (i >= 0) arr.splice(i, 1);
                return tabla([{ Id: id }]);
            }
            const reg = esPer
                ? { NombrePersonal: v('NombrePersonal'), Nombre: v('NombrePersonal'), Direccion: v('DireccionPersonal'), Telefono01: v('Telefono01'), TipoDocumento: Number(v('TipoDocumento')), NroDocumento: v('NroDocumento'),
                    TipoPersonal: Number(v('TipoPersonal')), Tipo: nombreDe(db.clasif[9], v('TipoPersonal'), 'cndesc1', 'cnnum'), Estado: !!v('estado'), EmpresaId: Number(v('EmpresaId')),
                    FechaNacimiento: new Date(String(v('FechaNacimiento')).replace(/\//g, '-') + 'T00:00:00Z') }
                : { NombreProveedor: v('NombreProveedor'), Direccion: v('DireccionProveedor'), Telefono01: v('Telefono01'), Telefono02: v('Telefono02'), Descripcion: v('Descripcion'),
                    TipoDocumento: Number(v('TipoDocumento')), NroDocumento: v('NroDocumento'), Estado: !!v('estado') };
            if (tipo === 1) { const Id = sig(arr); arr.push({ Id, ...reg }); return tabla([{ Id }]); }
            if (tipo === 2) { Object.assign(arr.find((x) => x.Id === id), reg); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'MAM_Almacenes': {
            if (tipo === 4) return tabla(db.depositos);
            if (tipo === 3) return tabla(db.sucursales.map((a) => ({ ...a, NombreDeposito: nombreDe(db.depositos, a.DepositoId, 'NombreDeposito') })));
            if (tipo === -1) { db.sucursales = db.sucursales.filter((a) => a.Id !== id); return tabla([{ Id: id }]); }
            const reg = { DepositoId: Number(v('DepositoId')), NombreAlmacen: v('NombreAlmacen'), Direccion: v('Direccion'), Descripcion: v('Descripcion'), Telefono: v('Telefono'), estado: Number(v('estado')) };
            if (tipo === 1) { const Id = sig(db.sucursales); db.sucursales.push({ Id, ...reg }); return tabla([{ Id }]); }
            if (tipo === 2) { Object.assign(db.sucursales.find((a) => a.Id === id), reg); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'MAM_Movimientos': {
            const fd = (k) => new Date(String(v(k)).replace(/\//g, '-') + 'T00:00:00Z');
            if (tipo === 13) return tabla(db.productos.map((x) => ({ Id: x.Id, CodigoExterno: x.CodigoExterno, Nombre: x.NombreProducto, DescripcionProducto: x.NombreProducto, stock: db.stock[x.Id] || 0, estado: 1 })));
            if (tipo === 3) return tabla(db.movInv.map((m) => ({ Id: m.Id, fdoc: m.fdoc, concep: m.concep, nconcep: nombreDe(db.tiposMovInv, m.concep, 'Descripcion'), obs: m.obs, est: 1, alm: m.alm, NombreDeposito: nombreDe(db.depositos, m.alm, 'NombreDeposito') })));
            if (tipo === 4) { const m = db.movInv.find((x) => x.Id === id); return tabla(m ? m.det.map((d) => ({ Id: d.Id, MovimientoId: m.Id, ProductoId: d.ProductoId, Producto: nombreDe(db.productos, d.ProductoId, 'NombreProducto'), Cantidad: d.Cantidad, Lote: '20200101', FechaVencimiento: new Date(Date.UTC(2020, 0, 1)), img: null, estado: 1, stock: db.stock[d.ProductoId] || 0 })) : [], COLS_MDET); }
            // Kaily: comprobante Reporte_Movimiento
            if (tipo === 17) { const m = db.movInv.find((x) => x.Id === id); return tabla(m ? m.det.map((d) => ({ Id: m.Id, FechaDocumento: m.fdoc, TipoMovimiento: nombreDe(db.tiposMovInv, m.concep, 'Descripcion'), NombreDeposito: nombreDe(db.depositos, m.alm, 'NombreDeposito'), Observacion: m.obs, CodigoExterno: nombreDe(db.productos, d.ProductoId, 'CodigoExterno'), Producto: nombreDe(db.productos, d.ProductoId, 'NombreProducto'), Cantidad: d.Cantidad, img: null })) : []); }
            if (tipo === 5) return tabla(db.tiposMovInv);
            if (tipo === 7) return tabla(productosVenta(1, 1).map((x) => ({ Id: x.Id, CodigoExterno: x.CodigoExterno, NombreProducto: x.NombreProducto, DescripcionProducto: x.NombreProducto, NombreCategoria: x.NombreCategoria, stockUnidad: Number(v('DepositoId')) === 1 ? x.stock : 0, conversion: x.Conversion })));
            if (tipo === 9 || tipo === 10 || tipo === 11 || tipo === 12) return tabla(db.productos.map((x) => ({ Id: x.Id, NombreProducto: x.NombreProducto, Categoria: x.NombreCategoria, Deposito: 'ALMACEN CENTRAL', Cantidad: db.stock[x.Id] || 0 })).filter((r) => (tipo === 9 || tipo === 12 ? r.Cantidad > 0 : true)));
            if (tipo === 14) return tabla(kardexDe(Number(v('ProductoId')), Number(v('DepositoId'))).filter((r) => r.fdoc < fd('FechaDocumento')));
            if (tipo === 15) return tabla(kardexDe(Number(v('ProductoId')), Number(v('DepositoId'))).filter((r) => r.fdoc >= fd('FechaI') && r.fdoc <= fd('FechaF')));
            if (tipo === 16) { const ids = [...new Set(db.movInv.filter((m) => m.alm === Number(v('DepositoId')) && m.fdoc >= fd('fechaI') && m.fdoc <= fd('fechaF')).flatMap((m) => m.det.map((d) => d.ProductoId)))]; return tabla(ids.map((i) => ({ Id: i, NombreProducto: nombreDe(db.productos, i, 'NombreProducto'), SaldoAnterior: '', Entradas: '', Salidas: '', SaldoFinal: '' }))); }
            if (tipo === -1) { db.movInv = db.movInv.filter((m) => m.Id !== id && !(Number(v('ConceptoId')) === 8 && m.destinoDe === id)); return tabla([{ Id: id }]); }
            if (tipo === 1 || tipo === 2) {
                const reg = { fdoc: fd('FechaDocumento'), concep: Number(v('ConceptoId')), obs: v('Observacion'), alm: Number(v('DepositoId')) };
                let m;
                if (tipo === 1) { m = { Id: sig(db.movInv), ...reg, depDestino: Number(v('DepositoIdDestino')), destinoDe: Number(v('IdMovimientoDestino')), det: [] }; db.movInv.push(m); } else { m = db.movInv.find((x) => x.Id === id); Object.assign(m, reg); }
                for (const f of p.detalle.detalle.filas) {
                    const e = Number(f.estado); const ant = m.det.find((d) => d.Id === Number(f.Id));
                    if (e === 0) m.det.push({ Id: sig(m.det), ProductoId: Number(f.ProductoId), Cantidad: Number(f.Cantidad) });
                    if (e === 2 && ant) ant.Cantidad = Number(f.Cantidad);
                    if (e === -1 && ant) m.det = m.det.filter((d) => d !== ant);
                }
                return tabla([{ Id: m.Id }]);
            }
            if (tipo === 13) return tabla(db.productos.map((x) => ({ Id: x.Id, Nombre: x.NombreProducto, Stock: db.stock[x.Id] || 0 })));
            if (tipo === 18) {
                const cat = Number(v('CategoriaPrecio'));
                return tabla(db.productos.map((x) => ({
                    Id: x.Id, CodigoExterno: x.CodigoExterno, NombreProducto: x.NombreProducto,
                    Categoria: x.NombreCategoria, industria: '', unidad: 'UNIDAD', DescripcionProducto: x.DescripcionProducto || x.NombreProducto, stockMinimo: x.StockMinimo,
                    precio: (db.precios.find((y) => y.almacen === 1 && y.yhprod === x.Id && y.yhcatpre === cat) || {}).yhprecio || 0, stock: db.stock[x.Id] || 0, stockGeneral: db.stock[x.Id] || 0 })));
            }
            return tipo === 6 ? tabla([{ Lote: false }]) : tabla([]);
        }
        case 'MAM_Proforma': {
            if (tipo === 3) {
                const d = new Date(String(v('desde')).replace(/\//g, '-') + 'T00:00:00Z');
                const h = new Date(String(v('hasta')).replace(/\//g, '-') + 'T00:00:00Z');
                // como el SP real: sin columnas de cobro ni TipoCambio
                return tabla(db.proformas.filter((x) => x.FechaVenta >= d && x.FechaVenta <= h)
                    .map(({ IncrementoVenta, MontoBs, MontoDolares, TarjetaBancaria, TransferenciaBancaria, TipoCambio, ...x }) => x));
            }
            if (tipo === 4) return tabla(detalleVenta(id, 'proformasDet'), COLS_VDET);
            if (tipo === 6) { const vr = db.proformas.find((x) => x.Id === id); const cl = vr ? db.clientes.find((c) => c.Id === vr.ClienteId) || {} : {};
                return tabla(vr ? detalleVenta(vr.Id, 'proformasDet').map((d) => ({ Id: vr.Id, NombreCliente: vr.NombreCliente, Nit: cl.Nit || 'SN', Telefono: cl.Telefono || '', Direccion: cl.Direccion || '', HoraRegistro: vr.HoraRegistro || '10:05', ProductoId: d.ProductoId, CodigoExterno: 100 + d.ProductoId, Producto: d.Producto, Total: d.Total, DescuentoVenta: vr.Descuento || 0, imageEmpresa: null })) : []); }
            if (tipo === -1) { db.proformasDet = db.proformasDet.filter((d) => d.VentaId !== id); const n = db.proformas.length; db.proformas = db.proformas.filter((y) => y.Id !== id); return tabla(n === db.proformas.length ? [] : [{ Id: id }]); }
            if (tipo === 1 || tipo === 2) return grabarVenta(tipo, id, v, p.ventadetalletype.detalle, { cab: 'proformas', det: 'proformasDet', stock: false });
            return tabla([]);
        }
        case 'MAM_CreditosVentas': {
            const vivo = (c) => Math.round((c.Monto - c.abonado) * 100) / 100;
            db.cxc.forEach((c) => { c.Restante = vivo(c); });
            // Kaily: sin NombrePersonal en @tipo 3 / 6; PersonalId en 1 / 2 / 3 / 6; CierreModulo en los pagos (@tipo 5)
            const deuda = (c) => ({ Id: c.Id, Credito: c.Credito, Venta: String(c.venta), Nombre: c.Nombrecliente, Monto: c.Monto,
                abonado: c.abonado, Restante: c.Restante, FechaVencimientoCredito: new Date(Date.UTC(2026, 9, 1 + c.Id)), DiasMora: c.DiasMora, PersonalId: c.PersonalId });
            if (tipo === 1) return tabla(db.cxc.filter((c) => c.Restante > 0));
            if (tipo === 3) return tabla(db.cxc.filter((c) => c.Restante > 0).map(deuda));
            if (tipo === 6) return tabla(db.cxc.map(deuda));
            if (tipo === 2) return tabla(db.cxc.filter((c) => c.Restante <= 0).map((c) => ({ Id: c.Id, Credito: `Credito${String(c.Credito).padStart(7, '0')}`, venta: `venta${String(c.venta).padStart(8, '0')}`, NombreCliente: c.Nombrecliente, Monto: c.Monto, Pagado: c.abonado, FechaVencimientoCredito: new Date(Date.UTC(2026, 9, 1)), FechaUltimaPago: new Date(), PersonalId: c.PersonalId })));
            if (tipo === 5) return tabla(db.pagosCxc.filter((x) => x.Credito === Number(v('CreditoVentaId'))).map(({ Credito, ...x }) => ({ CierreModulo: 0, ...x })), ['Id', 'FechaPago', 'Monto', 'Glosa', 'NroComprobante', 'NombrePersonal', 'NombreCliente', 'CierreModulo', 'img']);
            if (tipo === 4) {
                const c = db.cxc.find((x) => x.Id === Number(v('CreditoVentaId')));
                const Id = sig(db.pagosCxc);
                db.pagosCxc.push({ Id, Credito: c.Id, FechaPago: new Date(String(v('FechaPago')).replace(/\//g, '-') + 'T00:00:00Z'), Monto: Number(v('Monto')), Glosa: v('Glosa'), NroComprobante: v('NroRecibo'),
                    NombrePersonal: nombreDe(db.personal, v('PersonalId'), 'Nombre'), NombreCliente: c.Nombrecliente, img: null });
                c.abonado += Number(v('Monto'));
                return tabla([{ Id }]);
            }
            if (tipo === 7) {
                const pg = db.pagosCxc.find((x) => x.Id === Number(v('TransaccionVentaId')));
                if (!pg) return tabla([]);
                db.cxc.find((x) => x.Id === pg.Credito).abonado -= pg.Monto;
                db.pagosCxc = db.pagosCxc.filter((x) => x !== pg);
                return tabla([{ Id: pg.Id }]);
            }
            if (tipo === 8) {
                const c = db.cxc.find((x) => x.Id === Number(v('CreditoVentaId')));
                return tabla(db.pagosCxc.filter((x) => x.Credito === c.Id).map((x) => ({ Cliente: c.Nombrecliente, Venta: c.venta, MontoCredito: c.Monto, FechaPago: x.FechaPago, Monto: x.Monto, Glosa: x.Glosa, Saldo: c.Monto - c.abonado, imagen: Buffer.from('x') })));
            }
            return tabla([]);
        }
        case 'MAM_CajaIngresoEgreso': {
            if (tipo === 3) return tabla(db.movCaja.map(filaMov));
            if (tipo === 4) return tabla(db.cajas);
            if (tipo === 5) return tabla(db.tiposMov);
            if (tipo === 8) return tabla(db.movCaja.filter((m) => m.Id === id).map((m) => ({ ...filaMov(m), img: null })));
            if (tipo === 9) return tabla(db.movCaja.filter((m) => m.Fecha.getUTCFullYear() === Number(v('Anio')) && m.Fecha.getUTCMonth() + 1 === Number(v('Mes'))).map((m) => ({ Monto: m.Monto, IngresoEgreso: m.IngresoEgreso })));
            if (tipo === 12) return tabla(db.movCaja.map((m) => { const f = filaMov(m); return { Fecha: f.Fecha, Caja: f.NombreCaja, Movimiento: f.Movimiento, Motivo: f.NombreTipoMovimiento, Descripcion: f.Descripcion, Monto: f.Monto }; }));
            // Kaily Caja general: @tipo 6 = movimientos anteriores a FechaI ; @tipo 7 = del rango. Egreso en negativo
            if (tipo === 6 || tipo === 7) {
                const fi = new Date(String(v('FechaI')).replace(/\//g, '-') + 'T00:00:00Z');
                const ff = tipo === 7 ? new Date(String(v('FechaF')).replace(/\//g, '-') + 'T00:00:00Z') : null;
                return tabla(db.movCaja.filter((m) => (tipo === 6 ? m.Fecha < fi : m.Fecha >= fi && m.Fecha <= ff)).map((m) => ({ Id: m.Id, IngresoEgreso: m.IngresoEgreso, Fecha: m.Fecha,
                    CajatipoMovimientoId: m.CajatipoMovimientoId, TipoMovimiento: nombreDe(db.tiposMov, m.CajatipoMovimientoId, 'Descripcion'), Descripcion: m.Descripcion,
                    Ingreso: m.IngresoEgreso ? m.Monto : 0, Egreso: m.IngresoEgreso ? 0 : -m.Monto, Saldo: 0 })));
            }
            if (tipo === 13) return tabla(db.movCaja.filter((m) => Number(v('CajaId')) === -1 || m.CajaId === Number(v('CajaId'))).map((m) => { const f = filaMov(m); return { Fecha: f.Fecha, Caja: f.NombreCaja, Movimiento: f.Movimiento, Descripcion: f.Descripcion, Monto: f.Monto }; }));
            if (tipo === -1) { db.movCaja = db.movCaja.filter((m) => m.Id !== id); return tabla([{ Id: id }]); }
            if (tipo === 1 || tipo === 2) {
                const reg = { CierreCajeroId: 0, Fecha: new Date(String(v('Fecha')).replace(/\//g, '-') + 'T00:00:00Z'), Descripcion: v('Descripcion'), Monto: Number(v('Monto')), CajaId: Number(v('CajaId')), IngresoEgreso: Number(v('IngresoEgreso')),
                    CajatipoMovimientoId: Number(v('CajaTipoMovimientoId')), PersonalId: Number(v('PersonalId')), SucursalId: Number(v('SucursalId')), CajaIngresoEgresoIdDestino: Number(v('CajaIngresoEgresoIdDestino')), CajaIdDestino: Number(v('CajaIdDestino')) };
                if (tipo === 1) { const Id = sig(db.movCaja); db.movCaja.push({ Id, ...reg }); return tabla([{ Id }]); }
                Object.assign(db.movCaja.find((m) => m.Id === id), reg); return tabla([{ Id: id }]);
            }
            return tabla([]);
        }
        case 'MAM_CierreCajero': {
            // Kaily: Tec_CierreCajaCajero
            const COLS_CORTE = ['Id', 'CierreCajeroId', 'CorteBs', 'CantidadBs', 'SubTotalBs', 'CorteDolares', 'CantidadDolares', 'SubtotalDolares'];
            const COLS_MOD = ['Id', 'CierreCajeroId', 'Modulo', 'IdModulo'];
            const filaCC = (c) => ({ MontoInicial: 0, TipoCambio: 6.96, TotalVentas: 0, TotalCobranza: 0, TotalIngreso: 0, TotalEgresos: 0, TotalCaja: 0, TotalEfectivo: 0,
                TotalTransferencia: 0, TotalTarjeta: 0, TotalEfectivoRecibido: 0, Diferencia: 0, Observacion: '', PersonalId: 1, ...c,
                NombrePersonal: nombreDe(db.personal, c.PersonalId ?? 1, 'Nombre'), Sucursal: 'CENTRAL', Caja: Number(c.EstadoCaja) === 1 ? 'Abierta' : 'Cerrada' });
            db.cortesCC = db.cortesCC || {};
            if (tipo === 3) return tabla(db.cierreCajero.map(filaCC));
            if (tipo === 5) return tabla([{ Id: 501, FechaVenta: diaUTC(0), contado: 100, 'contado$': 0, tarjeta: 20, TransferenciaBancaria: 30, tipocambio: 6.96, credito: 0, totalbs: 150, 'total$': 0 }]);
            if (tipo === 7) return tabla([{ id: 21, NroComprobanteVenta: 'venta0000021', Monto: 40, FechaPago: diaUTC(0), PersonalId: 1, NombrePersonal: 'MARCO MAMANI' }]);
            if (tipo === 9) return tabla([{ id: 1, Fecha: diaUTC(0), Descripcion: 'GASTO', Monto: 10, IngresoEgreso: 0, Movimiento: 'EGRESO', CajatipoMovimientoId: 2, NombreTipoMovimiento: 'PAGO', PersonalId: 1, NombrePersonal: 'MARCO MAMANI' }]);
            if (tipo === 4 || tipo === 6 || tipo === 8) return tabla([]);
            if (tipo === 10) return tabla(db.cortesCC[id] || [], COLS_CORTE);
            if (tipo === 11) return tabla([], COLS_MOD);
            if (tipo === -1) { db.cierreCajero = db.cierreCajero.filter((x) => x.Id !== id); return tabla([{ Id: id }]); }
            if (tipo === 1 || tipo === 2) {
                const reg = { Fecha: new Date(String(v('Fecha')).replace(/\//g, '-') + 'T00:00:00Z'), PersonalId: Number(v('PersonalId')), SucursalId: Number(v('SucursalId')),
                    MontoInicial: Number(v('MontoInicial')), EstadoCaja: Number(v('EstadoCaja')), TipoCambio: Number(v('TipoCambio')), TotalVentas: Number(v('TotalVentas')),
                    TotalCobranza: Number(v('TotalCobranzas')), TotalIngreso: Number(v('TotalIngresos')), TotalEgresos: Number(v('TotalEgreso')), TotalCaja: Number(v('TotalCaja')),
                    TotalEfectivo: Number(v('TotalEfectivo')), TotalTransferencia: Number(v('Totaltransferencia')), TotalTarjeta: Number(v('TotalTarjeta')),
                    TotalEfectivoRecibido: Number(v('TotalEfectivoRecibido')), Diferencia: Number(v('Diferencia')), Observacion: v('Observacion') };
                if (tipo === 1) { const Id = sig(db.cierreCajero); db.cierreCajero.push({ Id, ...reg }); return tabla([{ Id }]); }
                Object.assign(db.cierreCajero.find((x) => x.Id === id), reg);
                db.cortesCC[id] = p.cortebilletes.detalle.filas.map((f) => Object.fromEntries(COLS_CORTE.map((c, i) => [c, f[i]])));
                return tabla([{ Id: id }]);
            }
            return tabla([]);
        }
        case 'MAM_Compras': {
            if (tipo === 8) return tabla(db.personal.map(({ Id, Nombre, Direccion, Telefono01 }) => ({ Id, Nombre, Direccion, Telefono01 })));
            if (tipo === 6) return tabla(db.proveedores.map((p) => ({ Id: p.Id, Nombre: p.NombreProveedor, Direccion: p.Direccion, Telefono01: p.Telefono01 })));
            if (tipo === 3) { // Kaily: L_prListarGeneralComprasFiltro (@desde / @hasta)
                const d = new Date(String(v('desde')).replace(/\//g, '-') + 'T00:00:00Z');
                const h = new Date(String(v('hasta')).replace(/\//g, '-') + 'T00:00:00Z');
                return tabla(db.compras.filter((c) => c.FechaTransaccion >= d && c.FechaTransaccion <= h).map((c) => ({ ...c, NombreProveedor: nombreDe(db.proveedores, c.ProveedorId, 'NombreProveedor') })));
            }
            if (tipo === 4) return tabla(db.comprasDet.filter((d) => d.CompraId === Number(v('id'))), COLS_CDET);
            if (tipo === 5) return tabla(db.productos.map((x) => ({ Id: x.Id, CodigoExterno: x.CodigoExterno, NombreProducto: x.NombreProducto, DescripcionProducto: x.DescripcionProducto || x.NombreProducto,
                NombreCategoria: nombreDe(db.categorias, x.CategoriaId, 'NombreCategoria'), industria: 'BOLIVIA', PrecioCosto: 30 + x.Id, PrecioVenta: (30 + x.Id) * 1.2, stock: db.stock[x.Id] || 0, estado: 1 })));
            if (tipo === 7) return tabla(db.comprasDet.filter((d) => d.CompraId === Number(v('id'))).map((d) => ({ Producto: d.Producto, Cantidad: d.Cantidad, PrecioCosto: d.PrecioCosto, TotalCompra: d.TotalCompra, Logo: Buffer.from('x') })));
            if (tipo === 1 || tipo === 2) {
                const det = p.detalle.detalle;
                const fila = (f) => (Array.isArray(f) ? Object.fromEntries(det.columnas.map((c, i) => [c, f[i]])) : f);
                const cab = { AlmacenId: Number(v('AlmacenId')), ProveedorId: Number(v('Proveedor')), FechaTransaccion: new Date(String(v('FechaTransaccion')).replace(/\//g, '-') + 'T00:00:00Z'),
                    TipoVenta: Number(v('TipoVenta')), FechaVencimientoCredito: new Date(String(v('FechaVencimientoCredito')).replace(/\//g, '-') + 'T00:00:00Z'), Moneda: 1, TituloMoneda: 'BOLIVIANOS', Estado: 1, Glosa: v('Glosa'), TotalCompra: Number(v('TotalCompra')), descuento: Number(v('Descuento')), EmpresaId: 1 };
                const Id = tipo === 1 ? sig(db.compras) : id;
                if (tipo === 1) db.compras.push({ Id, ...cab, transaccion: 0 }); else Object.assign(db.compras.find((c) => c.Id === id), cab);
                for (const f of det.filas.map(fila)) {
                    const e = Number(f.estado);
                    if (e === 0) db.comprasDet.push({ ...f, Id: sig(db.comprasDet), CompraId: Id, estado: 1 });
                    else if (e === 2) Object.assign(db.comprasDet.find((d) => d.Id === Number(f.Id)), { ...f, estado: 1 });
                    else if (e === -1) db.comprasDet = db.comprasDet.filter((d) => d.Id !== Number(f.Id));
                }
                return tabla([{ Id }]);
            }
            if (tipo === -1) { db.compras = db.compras.filter((c) => c.Id !== id); db.comprasDet = db.comprasDet.filter((d) => d.CompraId !== id); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'MAM_ConceptosFijos': {
            const op = (id) => nombreDe(db.operacionesCF, id, 'NombreOperacion');
            if (tipo === 3) return tabla(db.conceptosFijos.map((c) => ({ ...c, NombreOperacion: op(c.OperacionId), TipoConcepto: c.Tipo ? 'FIJO' : 'VARIABLE', imgEstado: null })));
            if (tipo === 4) return tabla(db.operacionesCF.map(({ Id, NombreOperacion }) => ({ Id, NombreOperacion })));
            const reg = { NombreConcepto: v('NombreConcepto'), Porcentaje: Number(v('Porcentaje')), OperacionId: Number(v('OperacionId')), Tipo: Number(v('TipoConcepto')), Estado: Number(v('Estado')) };
            if (tipo === 1) { const Id = sig(db.conceptosFijos); db.conceptosFijos.push({ Id, ...reg }); return tabla([{ Id }]); }
            if (tipo === 2) { Object.assign(db.conceptosFijos.find((c) => c.Id === id), reg); return tabla([{ Id: id }]); }
            if (tipo === -1) { db.conceptosFijos = db.conceptosFijos.filter((c) => c.Id !== id); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'MAM_Contratos': {
            const cf = (cid) => db.conceptosFijos.find((c) => c.Id === cid) || {};
            if (tipo === 3) return tabla(db.contratos.map((c) => ({ Id: c.Id, PersonalId: c.PersonalId, NombrePersonal: nombreDe(db.personal, c.PersonalId, 'Nombre'), TipoContratoID: c.TipoContratoID,
                TipoContratoDescripcion: nombreDe(db.clasif[10], c.TipoContratoID, 'cndesc1', 'cnnum'), img: null, Cargo: c.Cargo, DescripcionCargo: nombreDe(db.clasif[11], c.Cargo, 'cndesc1', 'cnnum'),
                'Antigüedad': '1 AÑO', SueldoBase: c.SueldoBase, InicioContrato: c.InicioContrato, FinContrato: c.FinContrato, FechaFin: c.Indefinido ? 'INDEFINIDO' : c.FinContrato, Estado: 1, Indefinido: c.Indefinido })));
            if (tipo === 4) return tabla(db.contratoConceptos.filter((x) => x.ContratoId === id).map((x) => ({ Id: x.Id, ContratoId: x.ContratoId, ConceptoId: x.ConceptoId, NombreConcepto: cf(x.ConceptoId).NombreConcepto, PorcentajeConcepto: cf(x.ConceptoId).Porcentaje, estado: 1 })),
                ['Id', 'ContratoId', 'ConceptoId', 'NombreConcepto', 'PorcentajeConcepto', 'estado']);
            if (tipo === 5) return tabla(db.conceptosFijos.filter((c) => c.Tipo === 1 && c.Estado === 1).map((c) => ({ id: c.Id, NombreConcepto: c.NombreConcepto, Porcentaje: c.Porcentaje })));
            if (tipo === 1 || tipo === 2) {
                const fd = (k) => new Date(String(v(k)).replace(/\//g, '-') + 'T00:00:00Z');
                const reg = { PersonalId: Number(v('PersonalId')), TipoContratoID: Number(v('TipoContratoId')), Cargo: Number(v('Cargo')), SueldoBase: Number(v('SueldoBase')), InicioContrato: fd('InicioContrato'), FinContrato: fd('FinContrato'), Estado: 1, Indefinido: Number(v('Indefinido')) };
                const Id = tipo === 1 ? sig(db.contratos) : id;
                if (tipo === 1) db.contratos.push({ Id, ...reg }); else Object.assign(db.contratos.find((c) => c.Id === id), reg);
                for (const f of p.detallecontratoconcepto.detalle.filas) {
                    if (Number(f.estado) === 0) db.contratoConceptos.push({ Id: sig(db.contratoConceptos), ContratoId: Id, ConceptoId: Number(f.ConceptoId) });
                    if (Number(f.estado) === -1) db.contratoConceptos = db.contratoConceptos.filter((x) => x.Id !== Number(f.Id));
                }
                return tabla([{ Id }]);
            }
            if (tipo === -1) { db.contratos = db.contratos.filter((c) => c.Id !== id); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'MAM_Planilla': {
            const COLS_PC = ['Id', 'PlanillaId', 'ConceptoId', 'NombreConcepto', 'Porcentaje', 'Monto', 'TipoConcepto', 'Descripcion', 'Fecha', 'estado', 'eliminar'];
            const ctr = (cid) => db.contratos.find((c) => c.Id === cid);
            const neto = (pl) => Math.round((pl.SueldoBruto + db.planillaConceptos.filter((x) => x.PlanillaId === pl.Id && x.TipoConcepto === 0).reduce((a, x) => a + x.Monto, 0)) * 100) / 100;
            const filaPl = (pl, i) => ({ fila: i + 1, PlanillaId: pl.Id, Anio: nombreDe(db.clasif[13], pl.Anio, 'cndesc1', 'cnnum'), Mes: nombreDe(db.clasif[12], pl.Mes, 'cndesc1', 'cnnum'),
                Trabajador: nombreDe(db.personal, ctr(pl.ContratoId).PersonalId, 'Nombre'), InicioContrato: ctr(pl.ContratoId).InicioContrato, FinContrato: ctr(pl.ContratoId).FinContrato,
                Cargo: nombreDe(db.clasif[11], ctr(pl.ContratoId).Cargo, 'cndesc1', 'cnnum'), Sueldo: pl.Sueldo, ConceptoFijos: null, SueldoBruto: pl.SueldoBruto, ConceptoVariable: null, SueldoNeto: neto(pl), Reporte: null });
            const delPeriodo = () => db.planillas.filter((pl) => Number(v('Todos')) === 1 || (pl.Mes === Number(v('Mes')) && pl.Anio === Number(v('Anio'))));
            if (tipo === 5) return tabla(db.contratos.filter((c) => !db.planillas.some((pl) => pl.ContratoId === c.Id && pl.Mes === Number(v('Mes')) && pl.Anio === Number(v('Anio')))).map((c) => ({
                IdPersonal: c.PersonalId, Personal: nombreDe(db.personal, c.PersonalId, 'Nombre'), ContratoId: c.Id, InicioContrato: c.InicioContrato, FinContrato: c.FinContrato, Cargo: nombreDe(db.clasif[11], c.Cargo, 'cndesc1', 'cnnum'),
                SueldoBase: c.SueldoBase, SueldoBruto: Math.round(c.SueldoBase * 1.05 * 100) / 100 })));
            if (tipo === 4) return tabla(db.planillaConceptos.filter((x) => x.PlanillaId === id && (Number(v('TipoConcepto')) === -1 || x.TipoConcepto === Number(v('TipoConcepto')))).map((x) => ({ ...x, estado: 1, eliminar: null })), COLS_PC);
            if (tipo === 1) {
                const Id = sig(db.planillas);
                db.planillas.push({ Id, ContratoId: Number(v('ContratoId')), Anio: Number(v('Anio')), Mes: Number(v('Mes')), Sueldo: Number(v('Sueldo')), SueldoBruto: Number(v('SueldoBruto')) });
                for (const f of p.planillaconcepto.detalle.filas) db.planillaConceptos.push({ ...f, Id: sig(db.planillaConceptos), PlanillaId: Id });
                return tabla([{ Id }]);
            }
            if (tipo === 7) {
                for (const f of p.planillaconcepto.detalle.filas) {
                    if (Number(f.estado) === 0) db.planillaConceptos.push({ ...f, Id: sig(db.planillaConceptos), PlanillaId: id, eliminar: undefined });
                    if (Number(f.estado) === -1) db.planillaConceptos = db.planillaConceptos.filter((x) => x.Id !== Number(f.Id));
                }
                return tabla([{ Id: id }]);
            }
            if (tipo === 6) return tabla(db.conceptosFijos.filter((c) => c.Tipo === 0).map((c) => ({ id: c.Id, NombreConcepto: c.NombreConcepto, Operacion: nombreDe(db.operacionesCF, c.OperacionId, 'Operacion') })));
            if (tipo === 3) return tabla(delPeriodo().map(filaPl));
            if (tipo === 9) return tabla(delPeriodo().map((pl, i) => { const { ConceptoFijos, ConceptoVariable, Reporte, ...x } = filaPl(pl, i); return { ...x, img: Buffer.from('x') }; }));
            if (tipo === 8) { // mismas columnas que el SP (LEFT JOIN: sin conceptos sale 1 fila con NombreConcepto NULL)
                const pl = db.planillas.find((x) => x.Id === id);
                if (!pl) return tabla([]);
                const c = ctr(pl.ContratoId); const fp = filaPl(pl, 0);
                const per = db.personal.find((x) => x.Id === c.PersonalId) || {};
                const conc = db.planillaConceptos.filter((x) => x.PlanillaId === id);
                const suma = (t) => conc.filter((x) => Number(x.TipoConcepto) === t).reduce((a, x) => a + Number(x.Monto), 0);
                const ddmm = (d) => new Date(d).toISOString().slice(0, 10).split('-').reverse().join('/');
                const base = { PlanillaId: id, NombrePersonal: fp.Trabajador, Cargo: fp.Cargo, Mes: `${fp.Mes} - ${fp.Anio}`, HaberBasicoBase: c.SueldoBase, NroDocumento: per.NroDocumento ?? per.Ci ?? '',
                    Regional: 'Santa Cruz', FechaIngreso: ddmm(c.InicioContrato), 'Antigüedad': '1 Año; 1 Meses; 4 Dias', HaberBasico: pl.Sueldo };
                const tot = { SueldoBruto: pl.SueldoBruto, SueldoNeto: neto(pl), img: Buffer.from('x'), SumaConceptoFijo: suma(1), SumaConceptoVariable: suma(0) };
                if (!conc.length) return tabla([{ ...base, NombreConcepto: null, TipoConcepto: 'Variable', asNombreOperacion: '---', Porcentaje: '---', Monto: 0, ...tot }]);
                return tabla(conc.map((x) => ({ ...base, NombreConcepto: `${x.NombreConcepto}

Fecha: ${ddmm(x.Fecha || new Date())}`, TipoConcepto: Number(x.TipoConcepto) === 1 ? 'Fijo' : 'Variable',
                    asNombreOperacion: Number(x.Monto) < 0 ? 'Resta' : 'Suma', Porcentaje: Number(x.TipoConcepto) === 1 ? String(Math.abs(Number(x.Porcentaje))) : '---', Monto: x.Monto, ...tot })));
            }
            return tabla([]);
        }
        case 'MAM_CreditosCompras': {
            const deuda = (c) => ({ Id: c.Id, Credito: c.Credito, Compra: String(c.Compra), NombrePersonal: 'Null', Nombre: c.NombreProveedor, Monto: c.Monto, abonado: c.abonado, Restante: Math.round((c.Monto - c.abonado) * 100) / 100,
                FechaVencimientoCredito: diaUTC(c.DiasMora), DiasMora: c.DiasMora });
            db.cxp.forEach((c) => { c.Restante = Math.round((c.Monto - c.abonado) * 100) / 100; });
            if (tipo === 1) return tabla(db.cxp.filter((c) => c.Restante > 0));
            if (tipo === 3) return tabla(db.cxp.filter((c) => c.Restante > 0).map(deuda));
            if (tipo === 6) return tabla(db.cxp.map(deuda));
            if (tipo === 2) return tabla(db.cxp.filter((c) => c.Restante <= 0).map((c) => ({ Id: c.Id, Credito: c.Credito, Compra: c.Compra, NombreProveedor: c.NombreProveedor, Monto: c.Monto, Pagado: c.abonado, FechaVencimientoCredito: diaUTC(3), FechaUltimaPago: new Date() })));
            if (tipo === 5) return tabla(db.pagosCxp.filter((x) => x.Credito === Number(v('CreditoCompraId'))).map(({ Credito, ...x }) => x), ['Id', 'FechaPago', 'Monto', 'Glosa', 'NroComprobante', 'NombrePersonal', 'img']);
            if (tipo === 4) {
                const c = db.cxp.find((x) => x.Id === Number(v('CreditoCompraId')));
                const Id = sig(db.pagosCxp);
                db.pagosCxp.push({ Id, Credito: c.Id, FechaPago: new Date(String(v('FechaPago')).replace(/\//g, '-') + 'T00:00:00Z'), Monto: Number(v('Monto')), Glosa: v('Glosa'), NroComprobante: v('NroRecibo'), NombrePersonal: nombreDe(db.personal, v('PersonalId'), 'Nombre'), img: null });
                c.abonado += Number(v('Monto'));
                return tabla([{ Id }]);
            }
            if (tipo === 7) {
                const pg = db.pagosCxp.find((x) => x.Id === Number(v('TransaccionCompraId')));
                if (!pg) return tabla([]);
                db.cxp.find((x) => x.Id === pg.Credito).abonado -= pg.Monto;
                db.pagosCxp = db.pagosCxp.filter((x) => x !== pg);
                return tabla([{ Id: pg.Id }]);
            }
            if (tipo === 8) {
                const c = db.cxp.find((x) => x.Id === Number(v('CreditoCompraId')));
                return tabla(db.pagosCxp.filter((x) => x.Credito === c.Id).map((x) => ({ Proveedor: c.NombreProveedor, Compra: c.Compra, MontoCredito: c.Monto, FechaPago: x.FechaPago, Monto: x.Monto, Glosa: x.Glosa, Saldo: c.Monto - c.abonado, imagen: Buffer.from('x') })));
            }
            return tabla([]);
        }
        case 'MAM_ReporteVentas': {
            if (tipo === 3) return tabla([{ Imagen: 'Default.jpg', Nombre: 'DISTRAL REGIONAL- POTOSI', Direccion: 'CALLE ARGOTE 168' }]);
            if (tipo === 9) return tabla(db.productos.map((x) => ({ Id: x.Id, NombreProducto: x.NombreProducto, Stock: db.stock[x.Id] || 0, PrecioCosto: 5 + x.Id, Total: (db.stock[x.Id] || 0) * (5 + x.Id), img: null })));
            if (tipo === 12) return tabla([{ Id: 3, NombreProducto: nombreDe(db.productos, 3, 'NombreProducto'), lote: 'L-2025', FechaVencimiento: diaUTC(15), cantidad: 12 }]);
            const fi = () => new Date(String(v('FechaI')).replace(/\//g, '-') + 'T00:00:00Z');
            const ff = () => new Date(String(v('FechaF')).replace(/\//g, '-') + 'T00:00:00Z');
            const enRango = () => db.ventas.filter((x) => x.FechaVenta >= fi() && x.FechaVenta <= ff() && x.NombreEstado !== 'ANULADO');
            const fila = (x) => ({ SucursalId: 1, NombreSucursal: 'SUCURSAL PRINCIPAL', Id: x.Id, FechaVenta: x.FechaVenta, PersonalId: x.PersonalId, Personal: x.Personal, ClienteId: x.ClienteId, NombreCliente: x.NombreCliente, TipoVenta: x.TVenta, TotalVenta: x.TotalVenta, imageEmpresa: Buffer.from('x') });
            if ([1, 2, 13, 14].includes(tipo)) return tabla(enRango().filter((x) => (![2, 14].includes(tipo) || x.PersonalId === Number(v('IdPersonal'))) && (![13, 14].includes(tipo) || x.ClienteId === Number(v('IdCliente')))).map(fila));
            if (tipo === 4 || tipo === 5) return tabla(enRango().filter((x) => tipo === 4 || x.PersonalId === Number(v('IdPersonal'))).map((x) => ({ ...fila(x), Costo: Math.round(x.TotalVenta * 0.72 * 100) / 100, Utilidad: Math.round(x.TotalVenta * 0.28 * 100) / 100 })));
            if (tipo === 7) return tabla(db.personal.map((x) => ({ chk: false, Id: x.Id, NombrePersonal: x.Nombre, Cargo: x.Tipo })), ['chk', 'Id', 'NombrePersonal', 'Cargo']);
            if (tipo === 8) {
                const sel = p.personaltype.detalle.filas.filter((f) => f.chk).map((f) => f.Id);
                // meses anteriores a las ventas del simulador: valores sinteticos (estacionalidad por vendedor) para probar graficos
                const primera = db.ventas.reduce((a, y) => (y.FechaVenta < a ? y.FechaVenta : a), new Date(8.64e15));
                const sint = (x) => { if (ff() >= primera) return null; const mes = fi().getUTCMonth(); const dias = Math.round((ff() - fi()) / 864e5) + 1;
                    const f = 1 + 0.35 * Math.sin((mes + x.Id * 2) / 1.9) + (x.Id % 3 === 0 ? mes * 0.04 : -mes * 0.02); const ped = Math.max(0, Math.round((8 + x.Id * 3) * f * dias / 30));
                    return { Personal: x.Nombre, Pedidos: ped, Total: Math.round(ped * (320 + x.Id * 25) * 100) / 100 }; };
                return tabla(db.personal.filter((x) => sel.includes(x.Id)).map((x) => { const s0 = sint(x); if (s0) return s0; const vs = enRango().filter((y) => y.PersonalId === x.Id); return { Personal: x.Nombre, Pedidos: vs.length, Total: Math.round(vs.reduce((s2, y) => s2 + y.TotalVenta, 0) * 100) / 100 }; }));
            }
            if (tipo === 15) return tabla(db.productos.filter((x) => !Number(v('ProductoId')) || x.Id === Number(v('ProductoId'))).map((x, i) => ({ Producto: x.NombreProducto, Cantidad: 40 - i * 5, Total: (40 - i * 5) * 9.5 })));
            if (tipo === 17 || tipo === 18) {
                // cobros de ventas a credito: pagadas 2 dias despues de la venta (el SP real trae el monto de la venta y el del pago)
                const GLOSAS = ['CANCELADO EN EFECTIVO', 'CANCELADO MEDIANTE QR', 'Cancelado', 'abono a cuenta', 'CANCELADO POR QR'];
                const cobros = db.ventas.filter((x) => x.TipoVenta === 0 && x.NombreEstado !== 'ANULADO').map((x, i) => ({
                    VentaId: x.Id, FechaVenta: x.FechaVenta, FechaPago: new Date(x.FechaVenta.getTime() + 2 * 864e5), PersonalId: x.PersonalId, NombrePersonal: x.Personal,
                    NombreCliente: x.NombreCliente, Glosa: GLOSAS[i % GLOSAS.length], MontoVenta: x.TotalVenta, Monto: i % 4 === 3 ? Math.round(x.TotalVenta * 50) / 100 : x.TotalVenta }));
                return tabla(cobros.filter((x) => x.FechaPago >= fi() && x.FechaPago <= ff() && (tipo === 17 || x.PersonalId === Number(v('IdPersonal')))));
            }
            if (tipo === 6) {
                const extra = ['CARISSA SH/AC SACHET BEBE 48X50+15ML', 'GALLITO DETERGENTE POLVO FLORAL 60X140 G.', 'LURI CARISSA SHAMPOO (CARISSA) (DYP) ALOE VERA (12X800ml)',
                    'LURI CARISSA SHAMPOO (CARISSA) (DYP) BEBE (12X800ml)', 'LURI TODO BRILLO LAVANDINA (12X1LI)', 'LURI TODO BRILLO SACA GRASA C/GATILLO (12X830ml)',
                    'TODO BRILLO 3A LAVAVAJILLAS LIMON 12X750ML', 'TB PLUS LIMPIA PISOS MARINO 12X900ML', 'LURI FOUR LOKO FOUR LOKO (BLACK) (12X473ml)'];
                const prods = [...db.productos.map((x) => ({ Id: x.Id, Nombre: x.NombreProducto })), ...extra.map((n, i) => ({ Id: 900 + i, Nombre: n }))];
                return tabla(prods.map((x, i) => ({ IdProducto: x.Id, NombreProducto: x.Nombre, Familia: ['BEBIDAS', 'LACTEOS', 'LIMPIEZA'][i % 3], SucursalId: 1, NombreAlmacen: 'Sucursal Principal',
                    Cantidad: Math.max(4, 240 - ((i * 37) % 200)), Total: Math.round((900 + ((i * 523) % 2400)) * 100) / 100 })).sort((x, y) => y.Cantidad - x.Cantidad), ['IdProducto', 'NombreProducto', 'Familia', 'SucursalId', 'NombreAlmacen', 'Cantidad', 'Total', 'img']);
            }
            if (tipo === 16) return tabla(db.comprasDet.filter((d) => !Number(v('ProductoId')) || d.ProductoId === Number(v('ProductoId'))).map((d) => ({ Fecha: (db.compras.find((c) => c.Id === d.CompraId) || {}).FechaTransaccion, Compra: d.CompraId, Producto: d.Producto, Cantidad: d.Cantidad, PrecioCosto: d.PrecioCosto, Total: d.TotalCompra, imageEmpresa: Buffer.from('x') })));
            if (tipo === 10) return tabla(db.productos.slice(4).map((p) => ({ Id: p.Id, NombreProducto: p.NombreProducto })));
            if (tipo === 11) return tabla(db.productos.slice(2).map((p) => ({ Id: p.Id, NombreProducto: p.NombreProducto })));
            return tabla([]);
        }
        case 'sp_Mam_Precios': {
            if (tipo === 3) return tabla(db.catPrecio);
            if (tipo === 4) return tabla(db.almacenes);
            if (tipo === 6) return tabla(db.productos.map((x) => ({ yfnumi: x.Id, yfcprod: x.CodigoExterno, yfcdprod1: x.NombreProducto, NombreCategoria: x.NombreCategoria, proveedor: nombreDe(db.proveedores, x.ProveedorId, 'NombreProveedor') })));
            // Kaily: @tipo 9 = L_fnListarCategoriaPrecioFilter (ygnumi, Nombre) · @tipo 10 = L_fnListarCategoriaProductos (id, Nombre, seleccionado)
            if (tipo === 9) return tabla(db.catPrecio.map(({ ygnumi, ygcod, ygdesc }) => ({ ygnumi, ygcod, Nombre: ygdesc, ygdesc })));
            if (tipo === 10) return tabla(db.categorias.map((c) => ({ id: c.Id, Nombre: c.NombreCategoria, seleccionado: false })));
            if (tipo === 7 || tipo === 8) return tabla(db.catPrecio.map(({ ygnumi, ygcod, ygdesc }) => ({ ygnumi, ygcod, ygdesc })));
            if (tipo === 5) return matrizPrecios(Number(v('almacen')));
            if (tipo === 1) { const ygnumi = sig(db.catPrecio, 'ygnumi'); db.catPrecio.push({ ygnumi, ygcod: v('Codigo'), ygdesc: v('Descripcion'), ygpcv: 1, estado: Number(v('TipoCategoria')) ? 'LETRA' : 'NUMERO' }); return tabla([{ ygnumi }]); }
            if (tipo === -1) { db.catPrecio = db.catPrecio.filter((x) => x.ygnumi !== id); return tabla([{ ygnumi: id }]); }
            if (tipo === 2) {
                const alm = Number(v('almacen'));
                for (const f of p.ty007.detalle.filas) {
                    const [, prod, cat, precio, , est] = f;
                    if (Number(est) === 2) db.precios.find((y) => y.almacen === alm && y.yhprod === prod && y.yhcatpre === cat).yhprecio = precio;
                    if (Number(est) === 3) db.precios.push({ almacen: alm, yhprod: prod, yhcatpre: cat, yhprecio: precio });
                }
                return tabla([{ ok: 1 }]);
            }
            return tabla([]);
        }
        case 'MAM_Empresas': {
            if (tipo === 3) return tabla(db.empresas);
            if (tipo === -1) { db.empresas = db.empresas.filter((x) => x.Id !== id); return tabla([{ Id: id }]); }
            const reg = { Nombre: v('Nombre'), Descripcion: v('Descripcion'), Latitud: v('Latitud'), Longitud: v('Longitud'), Imagen: v('Imagen'), Propietario: v('Propietario'), Telefono: v('Telefono'), Nit: v('Nit'), Ciudad: v('Ciudad'), Direccion: v('Direccion') };
            if (tipo === 1) { const Id = sig(db.empresas); db.empresas.push({ Id, ...reg }); return tabla([{ Id }]); }
            if (tipo === 2) { Object.assign(db.empresas.find((x) => x.Id === id), reg); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'MAM_Categorias': {
            if (tipo === 3) return tabla(db.categorias);
            if (tipo === -1) { db.categorias = db.categorias.filter((x) => x.Id !== id); return tabla([{ Id: id }]); }
            const reg = { NombreCategoria: v('NombreCategoria'), DescripcionCategoria: v('DescripcionCategoria'), Estado: !!v('Estado'), Imagen: v('Imagen'), App: !!v('VisibleApp'), EmpresaId: Number(v('EmpresaId')), Empresa: nombreDe(db.empresas, v('EmpresaId'), 'Nombre') };
            if (tipo === 1) { const Id = sig(db.categorias); db.categorias.push({ Id, ...reg }); return tabla([{ Id }]); }
            if (tipo === 2) { Object.assign(db.categorias.find((x) => x.Id === id), reg); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'MAM_Usuarios': {
            if (tipo === 3) return tabla(db.usuarios);
            if (tipo === 4) return tabla(db.empresas.map((e) => ({ Id: e.Id, Codigo: e.Id, Nombre: e.Nombre })));
            if (tipo === 5) return tabla(db.roles.map((r) => ({ Id: r.Id, NombreRol: r.NombreRol })));
            if (tipo === -1) { db.usuarios = db.usuarios.filter((x) => x.Id !== id); return tabla([{ Id: id }]); }
            const reg = { NombreUsuario: v('NombreUsuario'), Contrasena: v('Contrasena'), estado: !!v('Estado'), RolId: Number(v('RolId')), NombreRol: nombreDe(db.roles, v('RolId'), 'NombreRol'), SucursalId: v('SucursalId'), IdEmpresa: Number(v('IdEmpresa')), Empresa: nombreDe(db.empresas, v('IdEmpresa'), 'Nombre'), IdPersonal: Number(v('IdPersonal')), Personal: nombreDe(db.personal, v('IdPersonal'), 'Nombre'),
                ModificarPrecioVenta: Number(v('ModificarPrecio')), AplicarDescuentoVenta: Number(v('ModificarDescuento')), VerTodasVentas: Number(v('VentasTodasVentas')),
                VerTodosVentasCierre: Number(v('VerTodasVentasCierre')), ModificarRegistroAnteriores: Number(v('ModificarRegistroAntiguos')), EliminarRegistros: Number(v('EliminarRegistros')) };
            if (tipo === 1) { const Id = sig(db.usuarios); db.usuarios.push({ Id, ...reg }); return tabla([{ Id }]); }
            if (tipo === 2) { Object.assign(db.usuarios.find((x) => x.Id === id), reg); return tabla([{ Id: id }]); }
            return tabla([]);
        }
        case 'MAM_Roles': {
            if (tipo === 3) return tabla(db.roles);
            if (tipo === 4) return tabla(detalleRol(id));
            if (tipo === -1) { db.roles = db.roles.filter((x) => x.Id !== id); return tabla([{ Id: id }]); }
            let Id = id;
            if (tipo === 1) { Id = sig(db.roles); db.roles.push({ Id, NombreRol: v('NombreRol'), FechaRegistro: hoy(), HoraRegistro: '', UsuarioRegistro: '' }); }
            if (tipo === 2) db.roles.find((x) => x.Id === Id).NombreRol = v('NombreRol');
            db.rolesDetalle = db.rolesDetalle.filter((x) => x.RolId !== Id);
            p.roldetalletype.detalle.filas.forEach((f) => db.rolesDetalle.push({ Id: _sec++, RolId: Id, ProgramaId: f.ProgramaId, Ver: f.Ver, Insertar: f.Insertar, Modificar: f.Modificar, Eliminar: f.Eliminar }));
            return tabla([{ Id }]);
        }
        // Auditoria De Registros (server/sql/07_Auditoria.sql): lo que registran MAM_Ventas / MAM_Compras / MAM_Movimientos
        case 'MAM_Auditoria': {
            if (tipo === 3) {
                const d = new Date(String(v('Desde')).split('/').join('-') + 'T00:00:00Z'); const h = new Date(String(v('Hasta')).split('/').join('-') + 'T23:59:59Z');
                const m = Number(v('Modulo')); const o = Number(v('Operacion')); const doc = Number(v('DocumentoId')); const u = String(v('UsuarioFiltro') || '');
                return tabla(db.auditoria.filter((a) => a.Fecha >= d && a.Fecha <= h && (m === -1 || a.Modulo === m) && (o === -1 || a.Operacion === o)
                    && (doc === -1 || a.DocumentoId === doc) && (!u || a.Usuario === u)).map(({ Antes, Despues, ...a }) => a));
            }
            if (tipo === 4) return tabla(db.auditoria.filter((a) => a.Id === id));
            if (tipo === 5) return tabla([...new Set(db.auditoria.map((a) => a.Usuario))].map((Usuario) => ({ Usuario })));
            return tabla([]);
        }
        // SP propio del panel (server/sql/03_MAM_PanelControl.sql)
        case 'MAM_PanelControl': {
            const fd = (k) => new Date(String(v(k)).replace(/\//g, '-') + 'T00:00:00Z');
            const suc = v('SucursalId') === undefined ? -1 : Number(v('SucursalId'));
            const ventas = db.ventas.filter((x) => x.Estado === 1 && x.FechaVenta >= fd('FechaI') && x.FechaVenta <= fd('FechaF') && (suc < 0 || x.SucursalId === suc));
            // como lo graba el escritorio: 'H:m' sin ceros ('9:5', '23:0')
            if (tipo === 1) return tabla(ventas.map((x) => { const [hh, mm] = String(x.HoraRegistro || '').split(':'); return { Id: x.Id, HoraRegistro: hh ? `${Number(hh)}:${Number(mm)}` : null }; }), ['Id', 'HoraRegistro']);
            if (tipo === 2) {
                const g = new Map();
                for (const x of ventas) {
                    const k = `${x.ClienteId}|${x.FechaVenta.toISOString().slice(0, 10)}`;
                    const r = g.get(k) || { ClienteId: x.ClienteId, NombreCliente: x.NombreCliente, FechaVenta: x.FechaVenta, Compras: 0, Monto: 0 };
                    r.Compras++; r.Monto = Math.round((r.Monto + x.TotalVenta) * 100) / 100;
                    g.set(k, r);
                }
                return tabla([...g.values()].sort((a, b) => a.ClienteId - b.ClienteId || a.FechaVenta - b.FechaVenta), ['ClienteId', 'NombreCliente', 'FechaVenta', 'Compras', 'Monto']);
            }
            if (tipo === 3) {
                const ultima = (pid) => db.ventasDet.filter((d) => d.ProductoId === pid).map((d) => db.ventas.find((x) => x.Id === d.VentaId))
                    .filter((x) => x && x.Estado === 1 && x.FechaVenta <= fd('FechaF')).reduce((a, x) => (!a || x.FechaVenta > a ? x.FechaVenta : a), null);
                return tabla(db.productos.filter((x) => (db.stock[x.Id] || 0) > 0).map((x) => {
                    const precio = (db.precios.find((y) => y.almacen === 1 && y.yhprod === x.Id && y.yhcatpre === 1) || {}).yhprecio || 0;
                    return { Id: x.Id, CodigoExterno: x.CodigoExterno, NombreProducto: x.NombreProducto, Categoria: x.NombreCategoria, Stock: db.stock[x.Id], PrecioVenta: precio, Valor: db.stock[x.Id] * precio, UltimaVenta: ultima(x.Id) };
                }).filter((x) => !x.UltimaVenta || x.UltimaVenta < fd('FechaI')).sort((a, b) => b.Valor - a.Valor),
                ['Id', 'CodigoExterno', 'NombreProducto', 'Categoria', 'Stock', 'PrecioVenta', 'Valor', 'UltimaVenta']);
            }
            return tabla([]);
        }
        default:
            return tabla([]);
    }
}

async function EjecutarConsulta(sqlTexto, lista) {
    const p = P(lista);
    if (/FROM\s+Usuarios/i.test(sqlTexto)) {
        return tabla(db.usuarios
            .filter((u) => u.NombreUsuario === p.nom.valor && u.Contrasena === p.pass.valor && u.estado)
            .map((u) => ({ Id: u.Id, RolId: u.RolId, SucursalId: u.SucursalId, IdPersonal: u.IdPersonal, Monto: db.tipoCambio,
                ModificarPrecioVenta: u.ModificarPrecioVenta ?? 1, AplicarDescuentoVenta: u.AplicarDescuentoVenta ?? 1, VerTodasVentas: u.VerTodasVentas ?? 1,
                VerTodosVentasCierre: u.VerTodosVentasCierre ?? 1, FechaBolivia: new Date(hoy() + 'T00:00:00Z'), ModificarRegistroAnteriores: u.ModificarRegistroAnteriores ?? 0,
                NombreSucursal: 'CENTRAL' })));
    }
    if (/FROM\s+TipoCambio/i.test(sqlTexto)) return tabla([{ Monto: db.tipoCambio }]);
    if (/FROM\s+Configuracion/i.test(sqlTexto)) return tabla([{ Lote: false, Mapa: true, TotalCajaGeneral: 1250.5 }]);
    if (/RolesDetalles/i.test(sqlTexto)) {
        const rol = Number(p.rolid?.valor);
        const mod = p.modulo ? Number(p.modulo.valor) : null;
        return tabla(db.rolesDetalle
            .filter((d) => d.RolId === rol)
            .map((d) => {
                const pr = db.programas.find((x) => x.Id === d.ProgramaId);
                return { ...d, IdPrograma: pr.IdPrograma, DescripcionPrograma: pr.DescripcionPrograma, Modulo: pr.Modulo };
            })
            .filter((d) => mod === null || d.Modulo === mod));
    }
    return tabla([]);
}

module.exports = { CrearConexion: async () => null, EjecutarProcedimiento, EjecutarConsulta, CerrarConexion: async () => null, __llamadas: llamadas, __db: db };
