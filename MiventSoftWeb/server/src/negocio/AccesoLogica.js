// =====================================================================
//  Negocio/AccesoLogica.js   (equivalente a Negocio/AccesoLogica.vb)
//
//  Mismos nombres de funcion, mismos SP y mismos @parametros que en VB.
//
//  L_Usuario: en VB era "Public Shared L_Usuario" (uno solo para toda la app).
//  En web hay muchos usuarios a la vez, asi que L_Usuario se toma del usuario
//  autenticado en CADA peticion (AsyncLocalStorage). Las funciones lo usan
//  igual que antes: L_Usuario()
//
//  ByRef _numi: en VB se devolvia por referencia. Aqui se devuelve
//  { resultado: true/false, _numi }.
// =====================================================================
const { AsyncLocalStorage } = require('node:async_hooks');
const DParametro = require('../datos/DParametro');
const { D_ProcedimientoConParam, D_ConsultaParam } = require('../datos/AccesoDatos');

const _contexto = new AsyncLocalStorage();

/** Ejecuta fn dentro del contexto del usuario autenticado */
function L_prConUsuario(usuario, fn) {
    return _contexto.run({ usuario }, fn);
}
function L_Usuario() {
    return _contexto.getStore()?.usuario ?? 'DEFAULT';
}

// Patron de VB:  If _Tabla.Rows.Count > 0 Then _numi = _Tabla.Rows(0).Item(0) ...
function _fnResultadoGrabar(_Tabla, _numiActual = '') {
    if (_Tabla.length > 0) {
        const primeraColumna = Object.values(_Tabla[0])[0];
        return { resultado: true, _numi: primeraColumna ?? _numiActual };
    }
    return { resultado: false, _numi: _numiActual };
}

// #Region "LIBRERIAS TEC" ==============================================

async function L_prLibreriaDetalleGeneral(_IdClasificador) {
    const _listParam = [
        new DParametro('@tipo', 3),
        new DParametro('@IdClasificador', _IdClasificador),
        new DParametro('@usuario', L_Usuario()),
    ];
    return D_ProcedimientoConParam('MAM_Clasificadores', _listParam);
}

// #Region "METODOS PARA EL CONTROL DE USUARIOS Y PRIVILEGIOS" ==========

// VB: D_Datos_Tabla("Id,RolId,SucursalId,IdPersonal","Usuarios","NombreUsuario = '" + _Nom + "' ...")
// -> Misma consulta pero PARAMETRIZADA (la original era vulnerable a SQL Injection).
// Kaily: mismas columnas que L_Validar_Usuario de VB (tipo de cambio, permisos de venta, fecha Bolivia, sucursal)
async function L_Validar_Usuario(_Nom, _Pass) {
    return D_ConsultaParam(
        `SELECT Id, RolId, SucursalId, IdPersonal,
                ISNULL((SELECT TOP 1 Monto FROM TipoCambio AS t ORDER BY id DESC), 0) AS Monto,
                ISNULL(ModificarPrecioVenta, 0) AS ModificarPrecioVenta,
                ISNULL(AplicarDescuentoVenta, 0) AS AplicarDescuentoVenta,
                ISNULL(VerTodasVentas, 0) AS VerTodasVentas,
                ISNULL(VerTodosVentasCierre, 0) AS VerTodosVentasCierre,
                CAST(DATEADD(HOUR, -4, GETUTCDATE()) AS DATE) AS FechaBolivia,
                ISNULL(ModificarRegistroAnteriores, 0) AS ModificarRegistroAnteriores,
                ISNULL((SELECT al.Nombrealmacen FROM Almacenes AS al WHERE al.id = SucursalId), 'Todos') AS NombreSucursal
           FROM Usuarios
          WHERE NombreUsuario = @Nom AND Contrasena = @Pass`,
        [new DParametro('@Nom', _Nom), new DParametro('@Pass', _Pass)]
    );
}

// VB: L_RolDetalle_General(_Modo, _idCabecera, _idModulo) con D_Datos_Tabla -> parametrizada
async function L_RolDetalle_General(_Modo, _idCabecera = '', _idModulo = '') {
    const campos = `RolesDetalles.Id, RolesDetalles.RolId, RolesDetalles.Ver, RolesDetalles.Insertar,
                    RolesDetalles.Modificar, RolesDetalles.Eliminar, Programas.IdPrograma,
                    Programas.DescripcionPrograma, Programas.Modulo`;
    if (_Modo === 0) {
        return D_ConsultaParam(
            `SELECT ${campos} FROM RolesDetalles, Programas WHERE RolesDetalles.ProgramaId = Programas.Id`
        );
    }
    const params = [new DParametro('@RolId', _idCabecera)];
    let filtroModulo = '';
    if (_idModulo !== '' && _idModulo !== null && _idModulo !== undefined) {
        filtroModulo = ' AND Programas.Modulo = @Modulo';
        params.push(new DParametro('@Modulo', _idModulo));
    }
    return D_ConsultaParam(
        `SELECT ${campos} FROM RolesDetalles, Programas
          WHERE RolesDetalles.RolId = @RolId${filtroModulo}
            AND RolesDetalles.ProgramaId = Programas.Id`,
        params
    );
}

async function ListarPersonal() {
    return D_ProcedimientoConParam('MAM_Clientes', [new DParametro('@tipo', 9)]);
}

async function ListarPersonalById(id) {
    return D_ProcedimientoConParam('MAM_Clientes', [new DParametro('@tipo', 11), new DParametro('@Id', id)]);
}

// #Region "Producto TecBrinc" (genericos) ==============================

async function L_prListarGeneral(NameSp) {
    return D_ProcedimientoConParam(NameSp, [new DParametro('@tipo', 3), new DParametro('@usuario', L_Usuario())]);
}

async function L_prBorrarRegistro(_numi, sp) {
    const _Tabla = await D_ProcedimientoConParam(sp, [
        new DParametro('@tipo', -1),
        new DParametro('@Id', _numi),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _Tabla.length > 0;
}

// #Region "Empresas TeC" ===============================================
// Nota: en VB InsertarEmpresa/ModificarEmpresa NO envian @usuario; se respeta igual.

function _paramsEmpresa(tipo, e) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', e._Id),
        new DParametro('@Nombre', e.Nombre),
        new DParametro('@Descripcion', e.Descripcion),
        new DParametro('@Latitud', e.Latitud),
        new DParametro('@Longitud', e.Longitud),
        new DParametro('@Imagen', e.Imagen),
        new DParametro('@Propietario', e.Propietario),
        new DParametro('@Telefono', e.Telefono),
        new DParametro('@Nit', e.Nit),
        new DParametro('@Ciudad', e.Ciudad),
        new DParametro('@Direccion', e.Direccion),
    ];
}

async function InsertarEmpresa(e) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Empresas', _paramsEmpresa(1, e));
    return _fnResultadoGrabar(_Tabla, e._Id);
}

async function ModificarEmpresa(e) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Empresas', _paramsEmpresa(2, e));
    return _fnResultadoGrabar(_Tabla, e._Id);
}

// #Region "Categoria TecBrinc" =========================================

async function L_prCategoriaGeneral() {
    return D_ProcedimientoConParam('MAM_Categorias', [new DParametro('@tipo', 3), new DParametro('@usuario', L_Usuario())]);
}

async function L_prCategoriaBorrar(_numi) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Categorias', [
        new DParametro('@tipo', -1),
        new DParametro('@Id', _numi),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _Tabla.length > 0;
}

async function L_prCategoriaInsertar(_numi, _NombreCategoria, _Descripcion, _estado, _Imagen, _VisibleApp, _Empresa) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Categorias', [
        new DParametro('@tipo', 1),
        new DParametro('@Id', _numi),
        new DParametro('@NombreCategoria', _NombreCategoria),
        new DParametro('@DescripcionCategoria', _Descripcion),
        new DParametro('@Estado', _estado),
        new DParametro('@Imagen', _Imagen),
        new DParametro('@VisibleApp', _VisibleApp),
        new DParametro('@EmpresaId', _Empresa),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _fnResultadoGrabar(_Tabla, _numi);
}

async function L_prCategoriaModificar(_numi, _NombreCategoria, _Descripcion, _estado, _Imagen, _VisibleApp, _Empresa) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Categorias', [
        new DParametro('@tipo', 2),
        new DParametro('@Id', _numi),
        new DParametro('@NombreCategoria', _NombreCategoria),
        new DParametro('@DescripcionCategoria', _Descripcion),
        new DParametro('@Estado', _estado),
        new DParametro('@Imagen', _Imagen),
        new DParametro('@VisibleApp', _VisibleApp),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@EmpresaId', _Empresa),
    ]);
    return _fnResultadoGrabar(_Tabla, _numi);
}

// #Region "Usuarios TecBrinc" ==========================================

async function L_prUsuarioGeneral() {
    return D_ProcedimientoConParam('MAM_Usuarios', [new DParametro('@tipo', 3), new DParametro('@usuario', L_Usuario())]);
}

async function L_prListaRolesUsuarios() {
    return D_ProcedimientoConParam('MAM_Usuarios', [new DParametro('@tipo', 5), new DParametro('@usuario', L_Usuario())]);
}

async function L_prListaEmpresasUsuarios() {
    return D_ProcedimientoConParam('MAM_Usuarios', [new DParametro('@tipo', 4), new DParametro('@usuario', L_Usuario())]);
}

async function L_prUsuarioBorrar(_numi) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Usuarios', [
        new DParametro('@tipo', -1),
        new DParametro('@Id', _numi),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _Tabla.length > 0;
}

function _paramsUsuario(tipo, u) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', u._numi),
        new DParametro('@RolId', u._RolId),
        new DParametro('@NombreUsuario', u._NombreUsuario),
        new DParametro('@Contrasena', u._Contrasena),
        new DParametro('@Estado', u._estado),
        new DParametro('@SucursalId', u._sucursalId),
        new DParametro('@IdEmpresa', u._IdEmpresa),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@IdPersonal', u.IdPersonal),
        // Kaily (punto de venta): permisos del usuario, igual que L_prUsuarioInsertar/Modificar de VB
        new DParametro('@VentasTodasVentas', u._VerTodasVentas),
        new DParametro('@VerTodasVentasCierre', u._VerTodosCierres),
        new DParametro('@ModificarRegistroAntiguos', u._ModificarRegistrosAntiguos),
        new DParametro('@ModificarPrecio', u._ModificarPrecio),
        new DParametro('@ModificarDescuento', u._ModificarDescuento),
    ];
}

async function L_prUsuarioInsertar(u) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Usuarios', _paramsUsuario(1, u));
    return _fnResultadoGrabar(_Tabla, u._numi);
}

async function L_prUsuarioModificar(u) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Usuarios', _paramsUsuario(2, u));
    return _fnResultadoGrabar(_Tabla, u._numi);
}

// #Region "ROLES TecBrinc" =============================================

// Mismo orden de columnas que: DefaultView.ToTable(True, "Id","RolId","ProgramaId",...)
const COLUMNAS_ROL_DETALLE = ['Id', 'RolId', 'ProgramaId', 'DescripcionPrograma', 'Modulo', 'Ver', 'Insertar', 'Modificar', 'Eliminar', 'estado'];

async function L_prRolGeneral() {
    return D_ProcedimientoConParam('MAM_Roles', [new DParametro('@tipo', 3), new DParametro('@usuario', L_Usuario())]);
}

async function L_prRolDetalleGeneral(_numi) {
    return D_ProcedimientoConParam('MAM_Roles', [
        new DParametro('@tipo', 4),
        new DParametro('@Id', _numi),
        new DParametro('@usuario', L_Usuario()),
    ]);
}

async function L_prRolGrabar(_numi, _rol, _detalleFilas) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Roles', [
        new DParametro('@tipo', 1),
        new DParametro('@Id', _numi),
        new DParametro('@NombreRol', _rol),
        new DParametro('@RolDetalleType', '', { columnas: COLUMNAS_ROL_DETALLE, filas: _detalleFilas }),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _fnResultadoGrabar(_Tabla, _numi);
}

async function L_prRolModificar(_numi, _rol, _detalleFilas) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Roles', [
        new DParametro('@tipo', 2),
        new DParametro('@Id', _numi),
        new DParametro('@NombreRol', _rol),
        new DParametro('@RolDetalleType', '', { columnas: COLUMNAS_ROL_DETALLE, filas: _detalleFilas }),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _fnResultadoGrabar(_Tabla, _numi);
}

async function L_prRolBorrar(_numi) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Roles', [
        new DParametro('@tipo', -1),
        new DParametro('@Id', _numi),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _Tabla.length > 0;
}


// =====================================================================
//  PASO 2: Clientes, Zonas, Productos, Precios
// =====================================================================

/** Columnas de un DataTable en el orden del SELECT (para enviar TVP por posicion, como VB) */
function _columnasDe(dt) {
    if (dt && dt.columns) {
        return Object.values(dt.columns).sort((a, b) => a.index - b.index).map((c) => c.name);
    }
    return Object.keys((dt && dt[0]) || {});
}

// #Region "LIBRERIAS TEC" -> Efecto tipo 10 (crear nuevo item de libreria)
async function L_prClasificadorGrabar(_numi, IdClasificador, _desc1) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Clasificadores', [
        new DParametro('@tipo', 1),
        new DParametro('@Id', _numi),
        new DParametro('@IdClasificador', IdClasificador),
        new DParametro('@descripcion', _desc1),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _fnResultadoGrabar(_Tabla, _numi);
}

// #Region "Clientes Tec" ==============================================
// Nota: igual que VB, InsertarCliente/ModificarCliente NO envian @usuario.
function _paramsCliente(tipo, c) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', c._Id),
        new DParametro('@ZonaId', c.IdZona),
        new DParametro('@PrecioCategoriaId', c.IdPrecio),
        new DParametro('@CodigoExterno', c.CodigoExterno),
        new DParametro('@NombreCliente', c.NombreCliente),
        new DParametro('@DireccionCliente', c.Direccion),
        new DParametro('@Telefono', c.Telefono),
        new DParametro('@estado', c.estado),
        new DParametro('@Observacion', ''),
        new DParametro('@TipoDocumento', c.TipoDocumento),
        new DParametro('@NroDocumento', c.NroDocumento),
        new DParametro('@RazonSocial', c.RazonSocial),
        new DParametro('@nit', c.nit),
        new DParametro('@FechaIngreso', c.FechaIngreso),
        new DParametro('@Latitud', c.Latitud),
        new DParametro('@Longitud', c.Longitud),
    ];
}

async function InsertarCliente(c) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Clientes', _paramsCliente(1, c)), c._Id);
}
async function ModificarCliente(c) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Clientes', _paramsCliente(2, c)), c._Id);
}
async function L_prClienteBorrar(_numi) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Clientes', [
        new DParametro('@tipo', -1),
        new DParametro('@Id', _numi),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _Tabla.length > 0;
}
async function L_prListaCategoriasPrecios() {
    return D_ProcedimientoConParam('MAM_Clientes', [new DParametro('@tipo', 5), new DParametro('@usuario', L_Usuario())]);
}
async function L_prListarZonas() {
    return D_ProcedimientoConParam('MAM_Clientes', [new DParametro('@tipo', 4), new DParametro('@usuario', L_Usuario())]);
}

// #Region "Tec Zonas" =================================================
async function GeneralZona() {
    return D_ProcedimientoConParam('sp_Mam_Zonas', [new DParametro('@tipo', 3), new DParametro('@usuario', L_Usuario())]);
}
async function L_fnListarPuntosPorZona(_zanumi) {
    return D_ProcedimientoConParam('sp_Mam_Zonas', [
        new DParametro('@tipo', 4),
        new DParametro('@Id', _zanumi),
        new DParametro('@usuario', L_Usuario()),
    ]);
}
function _paramsZona(tipo, _Id, _Nombre, _Descripcion, _zacolor, point) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', _Id),
        new DParametro('@NombreZona', _Nombre),
        new DParametro('@DescripcionZona', _Descripcion),
        new DParametro('@Color', _zacolor),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@ZonasPuntos', '', point),
    ];
}
async function InsertarZona(_Id, _Nombre, _Descripcion, _zacolor, point) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('sp_Mam_Zonas', _paramsZona(1, _Id, _Nombre, _Descripcion, _zacolor, point)), _Id);
}
async function ActualizarZona(_Id, _Nombre, _Descripcion, _zacolor, point) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('sp_Mam_Zonas', _paramsZona(2, _Id, _Nombre, _Descripcion, _zacolor, point)), _Id);
}
async function EliminarZona(numi) {
    const _Tabla = await D_ProcedimientoConParam('sp_Mam_Zonas', [
        new DParametro('@tipo', -1),
        new DParametro('@Id', numi),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _Tabla.length > 0;
}

// #Region "Producto TecBrinc" =========================================
async function L_prCargarImagenesRecepcion(ProductoId) {
    return D_ProcedimientoConParam('MAM_Productos', [
        new DParametro('@tipo', 5),
        new DParametro('@Id', ProductoId),
        new DParametro('@usuario', L_Usuario()),
    ]);
}
async function L_prProductoBorrar(_numi) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Productos', [
        new DParametro('@tipo', -1),
        new DParametro('@Id', _numi),
        new DParametro('@usuario', L_Usuario()),
    ]);
    // VB: If Rows.Count > 0 And Rows(0).Item(0) >= 0 -> ok; si no, _mensaje = Rows(0).Item(1)
    if (_Tabla.length > 0) {
        const v = Object.values(_Tabla[0]);
        if (Number(v[0]) >= 0) return { resultado: true, _mensaje: '' };
        return { resultado: false, _mensaje: String(v[1] ?? 'No se pudo eliminar el producto') };
    }
    return { resultado: false, _mensaje: 'No se pudo eliminar el producto' };
}
// Kaily: precios del producto por categoria (grPrecios de Tec_Productos.vb). ProductoId -1 = producto nuevo
async function ListPreciosDetalles(ProductoId) {
    return D_ProcedimientoConParam('MAM_Productos', [
        new DParametro('@tipo', 10),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@id', ProductoId),
    ]);
}
function _paramsProducto(tipo, p, _dtImagenes, dtPrecio) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', p._numi),
        new DParametro('@CodigoExterno', p._CodigoExterno),
        new DParametro('@CodigoBarras', p._CodigoBarra),
        new DParametro('@NombreProducto', p._NombreProducto),
        new DParametro('@DescripcionProducto', p._Descripcion),
        new DParametro('@Estado', p._estado),
        new DParametro('@StockMinimo', p._stockMinimo),
        new DParametro('@CategoriaId', p._CategoriaId),
        new DParametro('@EmpresaId', p._EmpresaId),
        new DParametro('@ProveedorId', p._ProveedorId),
        new DParametro('@MarcaId', p._MarcaId),
        new DParametro('@AttributoId', p._AttributoId),
        new DParametro('@FamiliaId', p._FamiliaId),
        new DParametro('@UnidadVentaId', p._UnidadVentaId),
        new DParametro('@UnidadMaximaId', p._UnidadMaximaId),
        new DParametro('@Conversion', p._conversion),
        new DParametro('@TCL0064', '', _dtImagenes),
        new DParametro('@PrecioT', '', dtPrecio),
        new DParametro('@usuario', L_Usuario()),
    ];
}
async function L_prProductoInsertar(p, _dtImagenes, dtPrecio) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Productos', _paramsProducto(1, p, _dtImagenes, dtPrecio)), p._numi);
}
async function L_prProductoModificar(p, _dtImagenes, dtPrecio) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Productos', _paramsProducto(2, p, _dtImagenes, dtPrecio)), p._numi);
}
async function L_prListaCategorias() {
    return D_ProcedimientoConParam('MAM_Productos', [new DParametro('@tipo', 4), new DParametro('@usuario', L_Usuario())]);
}
async function L_prListarProveedores() {
    return D_ProcedimientoConParam('MAM_Productos', [new DParametro('@tipo', 7), new DParametro('@usuario', L_Usuario())]);
}

// #Region "TY006 Categorias" (sp_Mam_Precios) ==========================
const _sp = (tipo, extra = []) =>
    D_ProcedimientoConParam('sp_Mam_Precios', [new DParametro('@tipo', tipo), ...extra, new DParametro('@usuario', L_Usuario())]);

async function L_fnGeneralCategorias() { return _sp(3); }
async function L_fnGeneralSucursales() { return _sp(4); }
async function L_fnListarProductos() { return _sp(6); }
async function L_fnListarCategorias() { return _sp(7); }
async function L_fnListarProductosConPrecios(_almacen) { return _sp(5, [new DParametro('@almacen', _almacen)]); }
async function L_fnListarCategoriaPrecio() { return _sp(9); }   // FormularioStock: combo Categoria Precio
// FormularioStock (Inventario Productos): MAM_Movimientos @tipo 18, ProveedorId -1 = todos
async function L_prListarProductosTodosInventario(CategoriaPrecio, ProveedorId) {
    return D_ProcedimientoConParam('MAM_Movimientos', [
        new DParametro('@tipo', 18),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@CategoriaPrecio', CategoriaPrecio),
        new DParametro('@ProveedorId', ProveedorId),
    ]);
}

async function L_fnGrabarCategorias(_ygnumi, cod, desc, tipo) {
    // VB usa "@Usuario" con mayuscula aqui; SQL Server no distingue mayusculas en nombres de parametro
    const _Tabla = await D_ProcedimientoConParam('sp_Mam_Precios', [
        new DParametro('@tipo', 1),
        new DParametro('@Codigo', cod),
        new DParametro('@Descripcion', desc),
        new DParametro('@TipoCategoria', tipo),
        new DParametro('@Usuario', L_Usuario()),
    ]);
    return _fnResultadoGrabar(_Tabla, _ygnumi);
}
async function L_fnGrabarPrecios(_ygnumi, _almacen, _precio) {
    const _Tabla = await D_ProcedimientoConParam('sp_Mam_Precios', [
        new DParametro('@tipo', 2),
        new DParametro('@almacen', _almacen),
        new DParametro('@TY007', '', _precio),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _fnResultadoGrabar(_Tabla, _ygnumi);
}
async function L_fnEliminarCategoria(numi) {
    const _Tabla = await _sp(-1, [new DParametro('@Id', numi)]);
    return _Tabla.length > 0;
}

// =====================================================================
//  PANEL PRINCIPAL (solo lectura) - mismos SP que los reportes del escritorio
// =====================================================================

// #Region "Producto TecBrinc" -> L_prListarGeneralFechas (lo usa Tec_Ventas para su buscador)
async function L_prListarGeneralFechas(NameSp, Fechadesde, FechaHasta) {
    return D_ProcedimientoConParam(NameSp, [
        new DParametro('@tipo', 3),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@desde', Fechadesde),
        new DParametro('@hasta', FechaHasta),
    ]);
}
// #Region "Cuentas por Pagar" (Tec_AdministrarCuentasPorPagar / ...PorCobrar)
async function L_prListarPagosPendientes() {
    return D_ProcedimientoConParam('MAM_CreditosCompras', [new DParametro('@tipo', 1), new DParametro('@usuario', L_Usuario())]);
}
async function L_prListarPagosPendientesClientes() {
    return D_ProcedimientoConParam('MAM_CreditosVentas', [new DParametro('@tipo', 1), new DParametro('@usuario', L_Usuario())]);
}
// Reporte_ProductoMasVendido (VB no envia @usuario)
async function ReporteVentasProductosMasVendido(FechaI, FechaF) {
    return D_ProcedimientoConParam('MAM_ReporteVentas', [
        new DParametro('@tipo', 6),
        new DParametro('@FechaI', FechaI),
        new DParametro('@FechaF', FechaF),
    ]);
}
async function L_prListarProductosConStock0() {
    return D_ProcedimientoConParam('MAM_ReporteVentas', [new DParametro('@tipo', 10), new DParametro('@usuario', L_Usuario())]);
}
async function L_prListarProductosConStockMinimo() {
    return D_ProcedimientoConParam('MAM_ReporteVentas', [new DParametro('@tipo', 11), new DParametro('@usuario', L_Usuario())]);
}
async function ListaPedidosPendientesAsignaciones() {
    return D_ProcedimientoConParam('MAM_Ventas', [new DParametro('@tipo', 7), new DParametro('@usuario', L_Usuario())]);
}
// Pedidos de un vendedor en un dia con la ubicacion donde se registro (app movil): MAM_Ventas @tipo 23, ordenado por Id (orden de creacion)
async function ListarVentasVendedorFecha(FechaVenta, PersonalId) {
    return D_ProcedimientoConParam('MAM_Ventas', [
        new DParametro('@tipo', 23),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@FechaVenta', FechaVenta),
        new DParametro('@PersonalId', PersonalId),
    ]);
}

// =====================================================================
//  PASO 3: MODULO VENTAS  (Tec_Personal, Frm_Proveedor, Tec_Ventas, Tec_VentasDetalle)
//  Mismos SP, @tipo y @parametros que AccesoLogica.vb (validado con test/contrato_vb.json)
// =====================================================================

// #Region "Personal Tec"  (VB no envia @usuario)
function _paramsPersonal(tipo, x) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', x._Id),
        new DParametro('@NombrePersonal', x.NombrePersonal),
        new DParametro('@DireccionPersonal', x.Direccion),
        new DParametro('@Telefono01', x.Telefono01),
        new DParametro('@TipoDocumento', x.TipoDocumento),
        new DParametro('@estado', x.Estado),
        new DParametro('@NroDocumento', x.NroDocumento),
        new DParametro('@TipoPersonal', x.TipoPersonal),
        new DParametro('@EmpresaId', x.EmpresaId),
        new DParametro('@FechaNacimiento', x.FechaNacimiento),
    ];
}
async function InsertarPersonal(x) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Personal', _paramsPersonal(1, x)), x._Id);
}
async function ModificarPersonal(x) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Personal', _paramsPersonal(2, x)), x._Id);
}

// #Region "Proveedor Tec"  (VB no envia @usuario)
function _paramsProveedor(tipo, x) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', x._Id),
        new DParametro('@NombreProveedor', x.NombreProveedor),
        new DParametro('@DireccionProveedor', x.Direccion),
        new DParametro('@Telefono01', x.Telefono01),
        new DParametro('@Telefono02', x.Telefono02),
        new DParametro('@Descripcion', x.Descripcion),
        new DParametro('@TipoDocumento', x.TipoDocumento),
        new DParametro('@estado', x.Estado),
        new DParametro('@NroDocumento', x.NroDocumento),
    ];
}
async function InsertarProveedor(x) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Proveedores', _paramsProveedor(1, x)), x._Id);
}
async function ModificarProveedor(x) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Proveedores', _paramsProveedor(2, x)), x._Id);
}

// #Region Ventas
async function L_fnPedidosEstados() {
    return D_ProcedimientoConParam('sp_Mam_Precios', [new DParametro('@tipo', 8), new DParametro('@usuario', L_Usuario())]);
}
async function L_prLeerConfiguracion() {
    return D_ProcedimientoConParam('MAM_Movimientos', [new DParametro('@tipo', 6), new DParametro('@usuario', L_Usuario())]);
}
async function L_fnObtenerIncrementoVentaHoy() {
    return D_ProcedimientoConParam('MAM_VentasIncremento', [new DParametro('@tipo', 1), new DParametro('@usuario', L_Usuario())]);
}
// VB no envia @usuario
async function ListarCliente() {
    return D_ProcedimientoConParam('MAM_Clientes', [new DParametro('@tipo', 10)]);
}
async function ListaVentasDetalles(VentaId) {
    return D_ProcedimientoConParam('MAM_Ventas', [
        new DParametro('@tipo', 4),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@id', VentaId),
    ]);
}
// Kaily: L_prListarProductosVentas(_deposito, _Precio) -> @SucursalId, @CategoriaVentaId (productos Tipo 1 y kits Tipo 2)
async function L_prListarProductosVentas(_deposito, _Precio) {
    return D_ProcedimientoConParam('MAM_Ventas', [
        new DParametro('@tipo', 5),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@SucursalId', _deposito),
        new DParametro('@CategoriaVentaId', _Precio),
    ]);
}
async function L_prListarProductosVentasConciliaciones(_deposito, _ClienteId, ChoferId, PrecioId) {
    return D_ProcedimientoConParam('MAM_Ventas', [
        new DParametro('@tipo', 20),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@SucursalId', _deposito),
        new DParametro('@ClienteId', _ClienteId),
        new DParametro('@ChoferId', ChoferId),
        new DParametro('@PrecioId', PrecioId),
    ]);
}
async function L_prVerificarConcicliacionAbierta(_ChoferId) {
    return D_ProcedimientoConParam('MAM_Ventas', [
        new DParametro('@tipo', 19),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@ChoferId', _ChoferId),
    ]);
}
// VB no envia @usuario
async function ObtenerImagenEmpresa() {
    return D_ProcedimientoConParam('MAM_ReporteVentas', [new DParametro('@tipo', 3)]);
}

/**
 * VentaInsertar / VentaModificar: mismos parametros que VB.
 * _dtDetalle = { columnas: [orden del SELECT de MAM_Ventas @tipo 4], filas: [...] }
 * (igual que el DataTable del grid que VB envia a @VentaDetalleType por posicion)
 */
// Kaily (punto de venta): VentaInsertar / VentaModificar de AccesoLogica.vb
//   @VentaPagos = tabla de ListaVentasDetallePago (MAM_Ventas @tipo 8): efectivo Bs, dolares, tarjeta,
//   transferencia y tipo de cambio (vacia si es credito). @Facturado = swFacturado.
function _paramsVentaKaily(tipo, v, _dtDetalle, dtPago) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', v._numi),
        new DParametro('@SucursalId', v.AlmacenId),
        new DParametro('@FechaVenta', v.FechaTransacccion),
        new DParametro('@PersonalId', v.PersonalId),
        new DParametro('@TipoVenta', v.TipoVenta),
        new DParametro('@FechaVencimientoCredito', v.FechaVencCredito),
        new DParametro('@ClienteId', v.ClienteId),
        new DParametro('@MonedaVenta', 1),
        new DParametro('@Estado', 1),
        new DParametro('@Glosa', v.glosa),
        new DParametro('@TotalVenta', v.TotalCompra),
        new DParametro('@Descuento', v.Descuento),
        new DParametro('@VentaDetalleType', '', _dtDetalle),
        new DParametro('@VentaPagos', '', dtPago),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@Facturado', v.Facturado),
    ];
}
async function VentaInsertar(v, _dtDetalle, dtPago) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Ventas', _paramsVentaKaily(1, v, _dtDetalle, dtPago)), v._numi);
}
async function VentaModificar(v, _dtDetalle, dtPago) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Ventas', _paramsVentaKaily(2, v, _dtDetalle, dtPago)), v._numi);
}
// Lista de Tec_Ventas (Kaily): @tipo 3 por rango y sucursal del usuario (Global_Sucursal, -1 = todas)
async function L_prListarVentasGeneralFiltroFecha(NameSp, Desde, Hasta, SucursalId) {
    return D_ProcedimientoConParam(NameSp, [
        new DParametro('@tipo', 3),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@Desde', Desde),
        new DParametro('@Hasta', Hasta),
        new DParametro('@SucursalId', SucursalId),
    ]);
}
// Estructura de @VentaPagos (VentaId -1 = vacia)
async function ListaVentasDetallePago(VentaId) {
    return D_ProcedimientoConParam('MAM_Ventas', [
        new DParametro('@tipo', 8),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@id', VentaId),
    ]);
}
// Productos que componen un kit (Tec_VentasDetalle.InsertarProductosSinLote, Tipo = 2)
async function L_prListarProductosKitsVenta(KitId) {
    return D_ProcedimientoConParam('MAM_Ventas', [
        new DParametro('@tipo', 10),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@Id', KitId),
    ]);
}
// =====================================================================
//  Kaily: Tec_CierreCajaCajero (btnCierreCajero) - MAM_CierreCajero, mismos @tipo y parametros que AccesoLogica.vb
// =====================================================================
const _cc = (tipo, extra = []) => D_ProcedimientoConParam('MAM_CierreCajero', [new DParametro('@tipo', tipo), new DParametro('@usuario', L_Usuario()), ...extra]);
async function L_prListarGeneralCierreCajero(Desde, Hasta) { return _cc(3, [new DParametro('@desde', Desde), new DParametro('@hasta', Hasta)]); }
async function L_prListarVentaCierresCaja(CierreId) { return _cc(4, [new DParametro('@Id', CierreId)]); }
async function L_prListarVentaCierresCajaPendiente(PersonalId, SucursalId, Fecha, CierreTodos) {
    return _cc(5, [new DParametro('@Fecha', Fecha), new DParametro('@PersonalId', PersonalId), new DParametro('@SucursalId', SucursalId), new DParametro('@Global_CierreTodos', CierreTodos)]);
}
async function L_prListarCobranzaCierresCaja(CierreId) { return _cc(6, [new DParametro('@Id', CierreId)]); }
async function L_prListarCobrosCajaPendiente(Fecha, PersonalId, CierreTodos) {
    return _cc(7, [new DParametro('@Fecha', Fecha), new DParametro('@PersonalId', PersonalId), new DParametro('@Global_CierreTodos', CierreTodos)]);
}
async function L_prListarMovimientosIngresoEgresoCierre(CierreId) { return _cc(8, [new DParametro('@Id', CierreId)]); }
async function L_prListarMovimientosIngresoEgresoCierrePendiente(SucursalId, Fecha, PersonalId, CierreTodos) {
    return _cc(9, [new DParametro('@SucursalId', SucursalId), new DParametro('@Fecha', Fecha), new DParametro('@PersonalId', PersonalId), new DParametro('@Global_CierreTodos', CierreTodos)]);
}
async function L_prListarEfectivoCortesCierre(CierreId) { return _cc(10, [new DParametro('@Id', CierreId)]); }
async function L_prListarIdModulosCierre(CierreId) { return _cc(11, [new DParametro('@Id', CierreId)]); }
function _paramsCierreCajero(tipo, c) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', c._numi),
        new DParametro('@Fecha', c.Fecha),
        new DParametro('@PersonalId', c.PersonalId),
        new DParametro('@SucursalId', c.SucursalId),
        new DParametro('@MontoInicial', c.MontoInicial),
        new DParametro('@EstadoCaja', c.EstadoCaja),
        new DParametro('@TipoCambio', c.TipoCambio),
        new DParametro('@TotalVentas', c.TotalVentas),
        new DParametro('@TotalCobranzas', c.TotalCobranzas),
        new DParametro('@TotalIngresos', c.TotalIngresos),
        new DParametro('@TotalEgreso', c.TotalEgresos),
        new DParametro('@TotalCaja', c.TotalCaja),
        new DParametro('@TotalEfectivo', c.TotalEfectivo),
        new DParametro('@Totaltransferencia', c.TotalTransferencia),
        new DParametro('@TotalTarjeta', c.TotalTarjeta),
        new DParametro('@TotalEfectivoRecibido', c.TotalEfectivoRecibido),
        new DParametro('@Diferencia', c.Diferencia),
        new DParametro('@Observacion', c.Observacion),
    ];
}
async function L_prCierreCajeroInsertar(c) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_CierreCajero', [..._paramsCierreCajero(1, c), new DParametro('@usuario', L_Usuario())]), c._numi);
}
async function L_prCierreCajeroModificar(c, dtCortes, dtModulos) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_CierreCajero', [..._paramsCierreCajero(2, c),
        new DParametro('@CorteBilletes', '', dtCortes), new DParametro('@ModuloId', '', dtModulos), new DParametro('@usuario', L_Usuario())]), c._numi);
}

// =====================================================================
//  Kaily: Tec_Kits (btnKits) - MAM_Kits. Lista L_prListarGeneral (@tipo 3), borrar L_prBorrarRegistro (@tipo -1)
// =====================================================================
async function ListaProductosKits(KitId) {
    return D_ProcedimientoConParam('MAM_Kits', [new DParametro('@tipo', 4), new DParametro('@usuario', L_Usuario()), new DParametro('@id', KitId)]);
}
async function ListarProductosKits() {
    return D_ProcedimientoConParam('MAM_Kits', [new DParametro('@tipo', 5), new DParametro('@usuario', L_Usuario())]);
}
// VB: @CategoriaId siempre 1; @Usuario con mayuscula (SQL Server no distingue)
const _paramsKit = (tipo, k, dt) => [
    new DParametro('@tipo', tipo),
    new DParametro('@Id', k._Id),
    new DParametro('@NombreKit', k.NombreKit),
    new DParametro('@DescripcionKit', k.DescripcionKit),
    new DParametro('@CategoriaId', 1),
    new DParametro('@Estado', k.Estado),
    new DParametro('@Total', k.Total),
    new DParametro('@Usuario', L_Usuario()),
    new DParametro('@KitsProductos', '', dt),
];
async function InsertarKit(k, dt) { return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Kits', _paramsKit(1, k, dt)), k._Id); }
async function ModificarKit(k, dt) { return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Kits', _paramsKit(2, k, dt)), k._Id); }

// Kaily: Tec_CajaGeneral (btnCajaGeneral) - saldo anterior (@tipo 6) y movimientos del rango (@tipo 7)
async function L_fnObtenerHistorialCajaGeneral(FechaI) {
    return _cc2(6, [new DParametro('@FechaI', FechaI)]);
}
async function L_fnObtenerHistorialCajaGeneralDesdeHasta(FechaI, FechaF) {
    return _cc2(7, [new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF)]);
}
// Kaily: Tec_ReporteCajaDetallado (btnReporteCajaDetallado) - VB no envia @usuario. CajaId -1 = Todos
async function ReporteIngresoEgresoDetalladoCaja(FechaI, FechaF, CajaId) {
    return D_ProcedimientoConParam('MAM_CajaIngresoEgreso', [new DParametro('@tipo', 13), new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF), new DParametro('@CajaId', CajaId)]);
}
const _cc2 = (tipo, extra) => D_ProcedimientoConParam('MAM_CajaIngresoEgreso', [new DParametro('@tipo', tipo), new DParametro('@usuario', L_Usuario()), ...extra]);
// Kaily: L_ObtenerConfiguraciones (Lote, Mapa, TotalCajaGeneral de la tabla Configuracion)
async function L_ObtenerConfiguraciones() {
    return D_ConsultaParam('SELECT Lote, Mapa, TotalCajaGeneral FROM Configuracion', []);
}

// Global_TipoCambio: VB lo lee al iniciar sesion (L_Validar_Usuario); aqui se lee el vigente al grabar
async function L_ObtenerTipoCambio() {
    const dt = await D_ConsultaParam('SELECT ISNULL((SELECT TOP 1 Monto FROM TipoCambio AS t ORDER BY id DESC), 0) AS Monto', []);
    return dt.length ? Number(dt[0].Monto) || 0 : 0;
}

// =====================================================================
//  Cuentas por Cobrar (Tec_AdministrarCuentasPorCobrar) - MAM_CreditosVentas
// =====================================================================
async function L_prListarCreditosPagadosCliente() {
    return D_ProcedimientoConParam('MAM_CreditosVentas', [new DParametro('@tipo', 2), new DParametro('@usuario', L_Usuario())]);
}
async function L_prListarPagosPendientesFiltrosClientes() {
    return D_ProcedimientoConParam('MAM_CreditosVentas', [new DParametro('@tipo', 3), new DParametro('@usuario', L_Usuario())]);
}
async function L_prGrabarPagosCreditoVentas(TransaccionVentaId, CreditoVentaId, FechaPAgo, PersonalId, Glosa, NroComprobante, Pago) {
    const _Tabla = await D_ProcedimientoConParam('MAM_CreditosVentas', [
        new DParametro('@tipo', 4),
        new DParametro('@CreditoVentaId', CreditoVentaId),
        new DParametro('@FechaPago', FechaPAgo),
        new DParametro('@PersonalId', PersonalId),
        new DParametro('@Glosa', Glosa),
        new DParametro('@NroRecibo', NroComprobante),
        new DParametro('@Monto', Pago),
        new DParametro('@usuario', L_Usuario()),
    ]);
    return _fnResultadoGrabar(_Tabla, TransaccionVentaId);
}
async function L_prListarPagosCreditoVenta(CreditoCompraId) {
    return D_ProcedimientoConParam('MAM_CreditosVentas', [
        new DParametro('@tipo', 5), new DParametro('@usuario', L_Usuario()), new DParametro('@CreditoVentaId', CreditoCompraId),
    ]);
}
async function L_prListarPagosTodosCuentasPorCobrar() {
    return D_ProcedimientoConParam('MAM_CreditosVentas', [new DParametro('@tipo', 6), new DParametro('@usuario', L_Usuario())]);
}
async function L_prEliminarPagosCuentaPorCobrar(TransaccionCompraId) {
    const _Tabla = await D_ProcedimientoConParam('MAM_CreditosVentas', [
        new DParametro('@tipo', 7), new DParametro('@TransaccionVentaId', TransaccionCompraId), new DParametro('@usuario', L_Usuario()),
    ]);
    return _Tabla.length > 0;
}
async function L_EstadoDeCuentasPorCobrar(idCredito) {
    return D_ProcedimientoConParam('MAM_CreditosVentas', [
        new DParametro('@tipo', 8), new DParametro('@usuario', L_Usuario()), new DParametro('@CreditoVentaId', idCredito),
    ]);
}
// VB no envia @usuario
async function ListarPersonalCredito() {
    return D_ProcedimientoConParam('MAM_Compras', [new DParametro('@tipo', 8)]);
}

// =====================================================================
//  Incremento de venta (Tec_IncrementoVenta / Rep_HistorialIncrementoVenta)
// =====================================================================
async function L_prGuardarIncrementoVentaHoy(Monto) {
    return D_ProcedimientoConParam('MAM_VentasIncremento', [
        new DParametro('@tipo', 2), new DParametro('@usuario', L_Usuario()), new DParametro('@Monto', Monto),
    ]);
}
async function L_prListarHistorialIncrementoVenta(FechaDesde, FechaHasta) {
    return D_ProcedimientoConParam('MAM_VentasIncremento', [
        new DParametro('@tipo', 3), new DParametro('@usuario', L_Usuario()), new DParametro('@FechaDesde', FechaDesde), new DParametro('@FechaHasta', FechaHasta),
    ]);
}

// =====================================================================
//  Reportes de Gestion Pedidos (VB: ninguno de estos envia @usuario, salvo los de MAM_Productos/MAM_Movimientos)
// =====================================================================
const _rep = (tipo, extra = []) => D_ProcedimientoConParam('MAM_ReporteVentas', [new DParametro('@tipo', tipo), ...extra]);
const _fechas = (FechaI, FechaF) => [new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF)];
async function ReporteVentasAtendidasTodos(FechaI, FechaF) { return _rep(1, _fechas(FechaI, FechaF)); }
async function ReporteVentasAtendidasTodosUnVendedor(FechaI, FechaF, IdPersonal) { return _rep(2, [..._fechas(FechaI, FechaF), new DParametro('@IdPersonal', IdPersonal)]); }
async function ReporteVentasAtendidasTodosUnCliente(FechaI, FechaF, IdCliente) { return _rep(13, [..._fechas(FechaI, FechaF), new DParametro('@IdCliente', IdCliente)]); }
async function ReporteVentasAtendidasTodosUnVendedorUnCliente(FechaI, FechaF, IdPersonal, IdCliente) {
    return _rep(14, [..._fechas(FechaI, FechaF), new DParametro('@IdPersonal', IdPersonal), new DParametro('@IdCliente', IdCliente)]);
}
async function ReporteVentasUtilidadesTodos(FechaI, FechaF) { return _rep(4, _fechas(FechaI, FechaF)); }
async function ReporteVentasUtilidadesTodosUnVendedor(FechaI, FechaF, IdPersonal) { return _rep(5, [..._fechas(FechaI, FechaF), new DParametro('@IdPersonal', IdPersonal)]); }
async function ReporteListarPersonal() { return _rep(7); }
async function ReporteVentasRendimientoPersonal(FechaI, FechaF, dt) { return _rep(8, [..._fechas(FechaI, FechaF), new DParametro('@PersonalType', '', dt)]); }
async function ReporteVentasPorProductos(FechaI, FechaF, ProductoId) { return _rep(15, [..._fechas(FechaI, FechaF), new DParametro('@ProductoId', ProductoId)]); }
async function ReporteHistoricoCobrosTodos(FechaI, FechaF) { return _rep(17, _fechas(FechaI, FechaF)); }
async function ReporteHistoricoCobrosbyVendedor(FechaI, FechaF, IdPersonal) { return _rep(18, [..._fechas(FechaI, FechaF), new DParametro('@IdPersonal', IdPersonal)]); }
async function L_prListarProductosKardex(_deposito) {
    return D_ProcedimientoConParam('MAM_Movimientos', [new DParametro('@tipo', 13), new DParametro('@usuario', L_Usuario()), new DParametro('@DepositoId', _deposito)]);
}
// FrmVentasPorCategoria
async function L_prListaPersonalCB() { return D_ProcedimientoConParam('MAM_Productos', [new DParametro('@tipo', 8), new DParametro('@usuario', L_Usuario())]); }
async function L_prListarSubcategoriaAsignados() { return D_ProcedimientoConParam('MAM_Productos', [new DParametro('@tipo', 11), new DParametro('@usuario', L_Usuario())]); }
async function L_prListarProductosPorCategoria(FechaI, FechaF, PersonalId, CategoriaId, SubCategoriaId) {
    return D_ProcedimientoConParam('MAM_Productos', [new DParametro('@tipo', 9), new DParametro('@usuario', L_Usuario()), ..._fechas(FechaI, FechaF),
        new DParametro('@PersonalId', PersonalId), new DParametro('@CategoriaId', CategoriaId), new DParametro('@SubCategoriaId', SubCategoriaId)]);
}
async function L_prListarProductosPorCategoriaTotalV2PorCategoria(FechaI, FechaF, PersonalId, CategoriaId) {
    return D_ProcedimientoConParam('MAM_Productos', [new DParametro('@tipo', 12), new DParametro('@usuario', L_Usuario()), ..._fechas(FechaI, FechaF),
        new DParametro('@PersonalId', PersonalId), new DParametro('@CategoriaId', CategoriaId)]);
}

// =====================================================================
//  Proforma (Proforma.vb / ProformaDetalle.vb) - MAM_Proforma: mismos parametros que Ventas, sin incremento
// =====================================================================
async function ListaProformaDetalles(VentaId) {
    return D_ProcedimientoConParam('MAM_Proforma', [new DParametro('@tipo', 4), new DParametro('@usuario', L_Usuario()), new DParametro('@id', VentaId)]);
}
async function ListarProformaRecibo(VentaId) {
    return D_ProcedimientoConParam('MAM_Proforma', [new DParametro('@tipo', 6), new DParametro('@usuario', L_Usuario()), new DParametro('@id', VentaId)]);
}
function _paramsProforma(tipo, v, _dtDetalle) {
    return [
        new DParametro('@tipo', tipo),
        new DParametro('@Id', v._numi),
        new DParametro('@SucursalId', v.AlmacenId),
        new DParametro('@FechaVenta', v.FechaTransacccion),
        new DParametro('@PersonalId', v.PersonalId),
        new DParametro('@TipoVenta', v.TipoVenta),
        new DParametro('@FechaVencimientoCredito', v.FechaVencCredito),
        new DParametro('@ClienteId', v.ClienteId),
        // Kaily: ProformaInsertar / ProformaModificar no envian campos de distribucion ni pagos
        new DParametro('@MonedaVenta', 1),
        new DParametro('@Estado', 1),
        new DParametro('@Glosa', v.glosa),
        new DParametro('@TotalVenta', v.TotalCompra),
        new DParametro('@Descuento', v.Descuento),
        new DParametro('@VentaDetalleType', '', _dtDetalle),
        new DParametro('@usuario', L_Usuario()),
    ];
}
async function ProformaInsertar(v, _dtDetalle) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Proforma', _paramsProforma(1, v, _dtDetalle)), v._numi);
}
async function ProformaModificar(v, _dtDetalle) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Proforma', _paramsProforma(2, v, _dtDetalle)), v._numi);
}

// =====================================================================
//  Tec_AdministrarAsignacionesPedidos (btnAsignacionPedidos)
//  Las acciones envian la tabla COMPLETA de la grilla con la columna Asignar marcada
//  (igual que VB: CType(grX.DataSource, DataTable)) -> { columnas, filas }
// =====================================================================
// VB no envia @usuario
async function ListarSoloChoferes() { return D_ProcedimientoConParam('MAM_Clientes', [new DParametro('@tipo', 12)]); }
async function ListarConciliacion() { return D_ProcedimientoConParam('MAM_Ventas', [new DParametro('@tipo', 22)]); }
const _venUsu = (tipo, extra = []) => D_ProcedimientoConParam('MAM_Ventas', [new DParametro('@tipo', tipo), new DParametro('@usuario', L_Usuario()), ...extra]);
async function ListaPedidosAsignadosByChofer(ChoferId) { return _venUsu(9, [new DParametro('@ChoferId', ChoferId)]); }
async function ListaProductosAsignadosByChofer(ChoferId) { return _venUsu(10, [new DParametro('@ChoferId', ChoferId)]); }
async function ListaClientesAsignadosByChofer(ChoferId) { return _venUsu(11, [new DParametro('@ChoferId', ChoferId)]); }
async function ListaPedidosEntregadosByChofer(ChoferId, conciliacionId) {
    return _venUsu(15, [new DParametro('@ChoferId', ChoferId), new DParametro('@IDConciliacion', conciliacionId)]);
}
async function ListaPedidosAnulados() { return _venUsu(17); }

async function VentaInsertarAsignaciones(_dtDetalle, ChoferId) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Ventas', [
        new DParametro('@tipo', 8),
        new DParametro('@Asignacion', '', _dtDetalle),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@PersonalId', ChoferId),
    ]);
    return _Tabla.length > 0;
}
/** Devuelve { resultado, Mensaje } (VB: ByRef Mensaje) */
async function VentaEntregarPedidos(_dtDetalle, ChoferId, NombreDistribuidor) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Ventas', [
        new DParametro('@tipo', 14),
        new DParametro('@Asignacion', '', _dtDetalle),
        new DParametro('@usuario', L_Usuario()),
        new DParametro('@ChoferId', ChoferId),
    ]);
    if (_Tabla.length > 0) {
        const valor = Number(Object.values(_Tabla[0])[0]);
        if (valor > 0) return { resultado: true, Mensaje: '' };
        return { resultado: false, Mensaje: `No se Puede Cambiar A Entregado Por que El Distribuidor: ${NombreDistribuidor}  No Tiene Una Conciliación Con Estado Abierto` };
    }
    return { resultado: false, Mensaje: 'Hubo un error en el Servidor' };
}
async function VentaQuitarAsignaciones(_dtDetalle) { return (await _venUsu(12, [new DParametro('@Asignacion', '', _dtDetalle)])).length > 0; }
async function VentaRevertiEstadoEntregado(_dtDetalle) { return (await _venUsu(16, [new DParametro('@AsignacionEntregado', '', _dtDetalle)])).length > 0; }
async function VentaAnularPedidos(_dtDetalle) { return (await _venUsu(13, [new DParametro('@Asignacion', '', _dtDetalle)])).length > 0; }
async function VentaRevertirAnulacion(_dtDetalle) { return (await _venUsu(18, [new DParametro('@AsignacionAnulacion', '', _dtDetalle)])).length > 0; }

// =====================================================================
//  DISTRIBUCION: Tec_Despachos (MAM_DespachoProductos) y Tec_Conciliacion (MAM_Conciliacion)
// =====================================================================
const _spUsu = (sp, tipo, extra = []) => D_ProcedimientoConParam(sp, [new DParametro('@tipo', tipo), new DParametro('@usuario', L_Usuario()), ...extra]);
async function ListaDetalleDespachoProductos(Id) { return _spUsu('MAM_DespachoProductos', 4, [new DParametro('@id', Id)]); }
async function ListarProductosSeleccionables(SucursalId) { return _spUsu('MAM_DespachoProductos', 5, [new DParametro('@SucursalId', SucursalId)]); }
async function GenerarDatosDespachoReporte(DespachoId) { return _spUsu('MAM_DespachoProductos', 6, [new DParametro('@Id', DespachoId)]); }
async function CargarProductosAutomatico(PersonalId, SucursalId) {
    return _spUsu('MAM_DespachoProductos', 7, [new DParametro('@PersonalId', PersonalId), new DParametro('@SucursalId', SucursalId)]);
}
async function CargarReferenciaVentas(PersonalId) { return _spUsu('MAM_DespachoProductos', 8, [new DParametro('@PersonalId', PersonalId)]); }
const _paramsDespacho = (tipo, d) => [
    new DParametro('@tipo', tipo),
    new DParametro('@Id', d._Id),
    new DParametro('@PersonalId', d.PersonalId),
    new DParametro('@ConciliacionId', d.ConciliacionId),
    new DParametro('@SucursalId', d.SucursalId),
    new DParametro('@Fecha', d.Fecha),
    new DParametro('@NroNota', d.NroNota),
    new DParametro('@TipoMovimientoID', d.TipoMovimientoID),
    new DParametro('@Detalle', d.Detalle),
    new DParametro('@Usuario', L_Usuario()),
];
/** VB: InsertarDespachoProductos(ByRef _Id, ...) -> { resultado, _numi } */
async function InsertarDespachoProductos(d, dtdetalle, dtReferencia) {
    const _Tabla = await D_ProcedimientoConParam('MAM_DespachoProductos', [
        ..._paramsDespacho(1, d),
        new DParametro('@DespachoAutomatico', d.DespachoAutomatico),
        new DParametro('@DetalleDespacho', '', dtdetalle),
        new DParametro('@ReferenciaVenta', '', dtReferencia),
    ]);
    return _fnResultadoGrabar(_Tabla, d._Id);
}
async function ModificarDespachoProductos(d, dtdetalle) {
    const _Tabla = await D_ProcedimientoConParam('MAM_DespachoProductos', [..._paramsDespacho(2, d), new DParametro('@DetalleDespacho', '', dtdetalle)]);
    return _Tabla.length > 0;
}
async function ListarProductosSalidasConciliacion(Id) { return _spUsu('MAM_Conciliacion', 4, [new DParametro('@id', Id)]); }
async function ListarTodasSalidas(Id) { return _spUsu('MAM_Conciliacion', 5, [new DParametro('@id', Id)]); }
async function ListarProductosEntregadoConciliaciones(Id) { return _spUsu('MAM_Conciliacion', 6, [new DParametro('@id', Id)]); }
async function ListarProductosDetalleConciliacion(Id) { return _spUsu('MAM_Conciliacion', 7, [new DParametro('@id', Id)]); }
async function ModificarConciliacion(_Id, Fecha, Estado, Observacion, detalle) {
    const _Tabla = await D_ProcedimientoConParam('MAM_Conciliacion', [
        new DParametro('@tipo', 2),
        new DParametro('@Id', _Id),
        new DParametro('@Fecha', Fecha),
        new DParametro('@Estado', Estado),
        new DParametro('@Observacion', Observacion),
        new DParametro('@Usuario', L_Usuario()),
        new DParametro('@DetalleConciliacion', '', detalle),
    ]);
    return _Tabla.length > 0;
}

// =====================================================================
//  DISTRIBUCION (2): Ingresos/Egresos de caja, reportes del panel, asignacion de zonas
// =====================================================================
const _spT = (sp, tipo, extra = []) => D_ProcedimientoConParam(sp, [new DParametro('@tipo', tipo), ...extra]);
async function L_prListarCaja() { return _spUsu('MAM_CajaIngresoEgreso', 4); }
async function L_prListarTipoMovimientoCaja() { return _spUsu('MAM_CajaIngresoEgreso', 5); }
/** VB InsertarTipoMovimiento (Crear_TipoMovimientoCaja): MAM_Ventas @tipo 21 -> { resultado, _numi } ; Tipo 1 = ingreso, 0 = egreso */
async function InsertarTipoMovimiento(Descripcion, Tipo) {
    return _fnResultadoGrabar(await _spUsu('MAM_Ventas', 21, [new DParametro('@Id', ''), new DParametro('@Descripcion', Descripcion), new DParametro('@TipoMovimiento', Tipo)]));
}
async function ListarReporteIngresoEgreso(Id) { return _spUsu('MAM_CajaIngresoEgreso', 8, [new DParametro('@id', Id)]); }
const _paramsIE = (tipo, m) => [
    new DParametro('@tipo', tipo),
    new DParametro('@Id', m._numi),
    new DParametro('@CierreCajeroId', 0),
    new DParametro('@Fecha', m.Fecha),
    new DParametro('@Descripcion', m.Descripcion),
    new DParametro('@Monto', m.Monto),
    new DParametro('@CajaId', m.CajaId),
    new DParametro('@IngresoEgreso', m.IngresoEgreso),
    new DParametro('@CajaTipoMovimientoId', m.CajaTipoMovimientoId),
    new DParametro('@PersonalId', m.PersonalId),
    new DParametro('@Modulo', 0),
    new DParametro('@IdModulo', 0),
    new DParametro('@SucursalId', m.SucursalId),
    new DParametro('@CajaIngresoEgresoIdDestino', m.IdIngresoEgreso || 0),
    new DParametro('@CajaIdDestino', m.CajaIdDestino || 0),
    new DParametro('@usuario', L_Usuario()),
];
/** VB L_prIngresoSalidaInsertar(ByRef _numi, ...) -> { resultado, _numi } */
async function L_prIngresoSalidaInsertar(m) { return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_CajaIngresoEgreso', _paramsIE(1, m)), m._numi); }
async function L_prIngresoSalidaModificar(m) { return (await D_ProcedimientoConParam('MAM_CajaIngresoEgreso', _paramsIE(2, { ...m, IdIngresoEgreso: 0 }))).length > 0; }
// VB no envia @usuario en estos reportes
async function ReporteIngresosEgresosMes(Anio, Mes) { return _spT('MAM_CajaIngresoEgreso', 9, [new DParametro('@Mes', Mes), new DParametro('@Anio', Anio)]); }
async function ReporteIngresoEgresoDetallado(FechaI, FechaF) { return _spT('MAM_CajaIngresoEgreso', 12, [new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF)]); }
async function ReporteGastosFecha(FechaI, FechaF) { return _spT('MAM_CierreCaja', 10, [new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF)]); }
async function L_prListarHistorico(idPersonal, fechaI, fechaF) {
    return _spUsu('MAM_Clientes', 13, [new DParametro('@idVendedor', idPersonal), new DParametro('@fechI', fechaI), new DParametro('@fechaF', fechaF)]);
}
async function L_prListarAuditoriaVentas(fechaI, fechaF, tipoEvento) {
    return _spUsu('MAM_AuditoriaVentas', 1, [new DParametro('@fechaDesde', fechaI), new DParametro('@fechaHasta', fechaF), new DParametro('@tipoEvento', tipoEvento)]);
}
async function L_prListarAuditoriaVentasDetalle(EventoId) { return _spUsu('MAM_AuditoriaVentas', 2, [new DParametro('@EventoId', EventoId)]); }
async function ListarProveedoresCombo() { return _spT('MAM_Compras', 12); }
async function ListarCategoriasVentas() { return _spT('MAM_Ventas', 30); }   // VB: ListarCategorias (MAM_Ventas @tipo 30)
async function ListarProductosBuscador() { return _spT('MAM_Clientes', 14); }
async function L_prGenerarReporteProductosPorVentas(ProveedorId, CategoriaID, ClienteId, FechaI, FechaF, IdProducto) {
    return _spUsu('MAM_Ventas', 31, [new DParametro('@ProveedorId', ProveedorId), new DParametro('@CategoriaId', CategoriaID), new DParametro('@ClienteId', ClienteId),
        new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF), new DParametro('@IdProducto', IdProducto)]);
}
async function L_fnListarZonasAsignadas(idRepartidor) {
    return D_ProcedimientoConParam('sp_Mam_Zonas', [new DParametro('@tipo', 5), new DParametro('@idRepartidor', idRepartidor), new DParametro('@usuario', L_Usuario())]);
}
async function L_prRegistrarAsignacion(dt, idRepartidor) {
    const _Tabla = await D_ProcedimientoConParam('sp_Mam_Zonas', [new DParametro('@tipo', 6), new DParametro('@idRepartidor', idRepartidor), new DParametro('@usuario', L_Usuario()), new DParametro('@Asignaciontype', '', dt)]);
    return _Tabla.length > 0;
}

// =====================================================================
//  Tec_CierreCaja (MAM_CierreCaja)
// =====================================================================
async function ListaDetalleGastosCierreId(CierreCajaId) { return _spUsu('MAM_CierreCaja', 4, [new DParametro('@id', CierreCajaId)]); }
async function ListarConciliacionesCaja() { return _spUsu('MAM_CierreCaja', 5); }
async function ListaDetallePedidosconciliacion(Id) { return _spUsu('MAM_CierreCaja', 6, [new DParametro('@id', Id)]); }
async function ListaCobranzasPorPersonalSinCierre(PersonalId, Fecha) { return _spUsu('MAM_CierreCaja', 7, [new DParametro('@PersonalId', PersonalId), new DParametro('@FechaCierre', Fecha)]); }
async function ListaCobranzasPorPersonalconCierre(CierreCajaId) { return _spUsu('MAM_CierreCaja', 8, [new DParametro('@id', CierreCajaId)]); }
async function DatosreporteCierreCaja(CierreCajaId) { return _spUsu('MAM_CierreCaja', 9, [new DParametro('@id', CierreCajaId)]); }
const _paramsCierre = (tipo, c, dtGastos, dtCobranzas) => [
    new DParametro('@tipo', tipo),
    new DParametro('@Id', c._Id),
    new DParametro('@PersonalId', c.PersonalId),
    new DParametro('@ConciliacionId', c.ConciliacionId),
    new DParametro('@FechaCierre', c.FechaCierre),
    new DParametro('@TotalGastos', c.TotalGasto),
    new DParametro('@VentasCredito', c.VentasCredito),
    new DParametro('@VentasContado', c.VentasContado),
    new DParametro('@TotalCobros', c.TotalCobros),
    new DParametro('@EfectivoCalculado', c.EfectivoCalculado),
    new DParametro('@EfectivoRecibido', c.EfectivoRecibido),
    new DParametro('@Diferencia', c.Diferencia),
    new DParametro('@Usuario', L_Usuario()),
    new DParametro('@DetalleGastos', '', dtGastos),
    new DParametro('@Cobranzas', '', dtCobranzas),
];
/** VB InsertarCierreCaja(ByRef _Id, ...) -> { resultado, _numi } */
async function InsertarCierreCaja(c, dtGastos, dtCobranzas) { return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_CierreCaja', _paramsCierre(1, c, dtGastos, dtCobranzas)), c._Id); }
async function ModificarCierreCaja(c, dtGastos, dtCobranzas) { return (await D_ProcedimientoConParam('MAM_CierreCaja', _paramsCierre(2, c, dtGastos, dtCobranzas))).length > 0; }

// =====================================================================
//  INVENTARIO: Tec_Movimientos, Tec_KardexProductos, reportes de saldos, monitoreo
// =====================================================================
async function L_prListarDepositos() { return _spUsu('MAM_Almacenes', 4); }
async function L_prListarTiposMovimientos() { return _spUsu('MAM_Movimientos', 5); }
async function L_prListarDetalleMovimiento(Id) { return _spUsu('MAM_Movimientos', 4, [new DParametro('@id', Id)]); }
async function L_prListarProductosLote(DepositoId) { return _spUsu('MAM_Movimientos', 7, [new DParametro('@DepositoId', DepositoId)]); }
const _paramsMov = (tipo, m, dt) => [
    new DParametro('@tipo', tipo),
    new DParametro('@Id', m._numi),
    new DParametro('@ConceptoId', m.ConceptoId),
    new DParametro('@DepositoId', m.DepositoId),
    new DParametro('@Observacion', m.Observacion),
    new DParametro('@Estado', 1),
    new DParametro('@FechaDocumento', m.FechaDocumento),
    new DParametro('@detalle', '', dt),
    new DParametro('@usuario', L_Usuario()),
];
async function L_prMovimientoInsertar(m, dt) { return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Movimientos', _paramsMov(1, m, dt)), m._numi); }
async function L_prMovimientoActualizar(m, dt) { return (await D_ProcedimientoConParam('MAM_Movimientos', _paramsMov(2, m, dt))).length > 0; }
async function ReporteSaldosTodosAlmacenesMayorA0() { return _spUsu('MAM_Movimientos', 9); }
async function ReporteSaldosTodosAlmacenesTodos() { return _spUsu('MAM_Movimientos', 10); }
async function ReporteSaldosUnAlmacenTodosCantidad(DepositoId) { return _spUsu('MAM_Movimientos', 11, [new DParametro('@DepositoId', DepositoId)]); }
async function ReporteSaldosUnAlmacenCantidadMayor0(DepositoId) { return _spUsu('MAM_Movimientos', 12, [new DParametro('@DepositoId', DepositoId)]); }
async function L_fnObtenerHistorialProductoGeneral(ProductoId, FechaDocumento, DepositoId) {
    return _spUsu('MAM_Movimientos', 14, [new DParametro('@ProductoId', ProductoId), new DParametro('@FechaDocumento', FechaDocumento), new DParametro('@DepositoId', DepositoId)]);
}
async function L_fnObtenerHistorialProducto(ProductoId, FechaI, FechaF, DepositoId) {
    return _spUsu('MAM_Movimientos', 15, [new DParametro('@ProductoId', ProductoId), new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF), new DParametro('@DepositoId', DepositoId)]);
}
async function L_fnListarProductosQueTienenMovimientos(FechaI, FechaF, DepositoId) {
    return _spUsu('MAM_Movimientos', 16, [new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF), new DParametro('@DepositoId', DepositoId)]);
}
async function ReporteSaldosValorados() { return D_ProcedimientoConParam('MAM_ReporteVentas', [new DParametro('@tipo', 9)]); } // VB no envia @usuario
async function L_prListarProductosStockVencidos() { return _spUsu('MAM_ReporteVentas', 12); }

// Tec_Mapaclientes (btZonaMapaCliente)
async function L_prMapaCLienteGeneral() { return _spUsu('MAM_Clientes', 6); }
async function L_prMapaCLienteGeneralPorZona(ZonaId) { return _spUsu('MAM_Clientes', 7, [new DParametro('@ZonaId', ZonaId)]); }

// Tec_Sucursales (btInvAmacen)  SP: MAM_Almacenes
const _paramsAlm = (tipo, a) => [
    new DParametro('@tipo', tipo), new DParametro('@Id', a._numi), new DParametro('@DepositoId', a.DepositoId), new DParametro('@NombreAlmacen', a.NombreAlmacen),
    new DParametro('@Direccion', a.Direccion), new DParametro('@estado', a.estado), new DParametro('@Descripcion', a.Descripcion), new DParametro('@Telefono', a.Telefono),
    new DParametro('@usuario', L_Usuario()),
];
async function L_prSucursalInsertar(a) { return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Almacenes', _paramsAlm(1, a)), a._numi); }
async function L_prSucursalActualizar(a) { return (await D_ProcedimientoConParam('MAM_Almacenes', _paramsAlm(2, a))).length > 0; }
async function L_prSucursalBorrar(id) { return (await D_ProcedimientoConParam('MAM_Almacenes', [new DParametro('@tipo', -1), new DParametro('@Id', id), new DParametro('@usuario', L_Usuario())])).length > 0; }

// Tec_Compras / Tec_ComprasDetalle (btnCompras)  SP: MAM_Compras
async function ListaComprasDetalles(CompraId) { return _spUsu('MAM_Compras', 4, [new DParametro('@id', CompraId)]); }
async function L_prListarProductosCompras(_deposito) { return _spUsu('MAM_Compras', 5, [new DParametro('@AlmacenId', _deposito)]); }
async function ListarProveedores() { return _spT('MAM_Compras', 6); } // VB no envia @usuario
async function ReporteCompras(CompraId) { return _spUsu('MAM_Compras', 7, [new DParametro('@id', CompraId)]); }
const _paramsCompra = (tipo, c, dt) => [
    new DParametro('@tipo', tipo), new DParametro('@Id', c._numi), new DParametro('@AlmacenId', c.AlmacenId), new DParametro('@FechaTransaccion', c.FechaTransaccion),
    new DParametro('@Proveedor', c.ProveedorId), new DParametro('@TipoVenta', c.TipoVenta), new DParametro('@FechaVencimientoCredito', c.FechaVencimientoCredito),
    new DParametro('@NroNotaCompra', c.NroNotaCompra), new DParametro('@NroNotaRecepcion', c.NroNotaRecepcion), new DParametro('@Moneda', 1), new DParametro('@Estado', 1),
    new DParametro('@Glosa', c.Glosa), new DParametro('@TotalCompra', c.TotalCompra), new DParametro('@EmpresaId', 1), new DParametro('@Descuento', c.Descuento),
    new DParametro('@detalle', '', dt), new DParametro('@usuario', L_Usuario()),
];
async function ComprasInsertar(c, dt) { return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Compras', _paramsCompra(1, c, dt)), c._numi); }
async function ComprasModificar(c, dt) { return (await D_ProcedimientoConParam('MAM_Compras', _paramsCompra(2, c, dt))).length > 0; }
// Tec_HistoricoPreciosCompra (btnHistorico)
const _rangoCompras = (tipo, FechaI, FechaF) => _spUsu('MAM_Compras', tipo, [new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF)]);
async function ListaComprasHistorico(FechaI, FechaF) { return _rangoCompras(9, FechaI, FechaF); }
async function ListaComprasTotalesHistorico(FechaI, FechaF) { return _rangoCompras(10, FechaI, FechaF); }
async function ListaComprasProductosHistorico(FechaI, FechaF) { return _rangoCompras(11, FechaI, FechaF); }
// FReporteCompraProducto (btnComprasPorProducto)  VB no envia @usuario
async function ReporteComprasPorProductos(FechaI, FechaF, ProductoId) {
    return _spT('MAM_ReporteVentas', 16, [new DParametro('@FechaI', FechaI), new DParametro('@FechaF', FechaF), new DParametro('@ProductoId', ProductoId)]);
}
// Tec_AdministrarCuentasPorPagar (btnCompraCredito)  SP: MAM_CreditosCompras
async function L_prListarCreditosPagados() { return _spUsu('MAM_CreditosCompras', 2); }
async function L_prListarPagosPendientesFiltros() { return _spUsu('MAM_CreditosCompras', 3); }
async function L_prGrabarPagosCreditoCompras(TransaccionCompraId, CreditoCompraId, FechaPAgo, PersonalId, Glosa, NroComprobante, Pago) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_CreditosCompras', [
        new DParametro('@tipo', 4), new DParametro('@CreditoCompraId', CreditoCompraId), new DParametro('@FechaPago', FechaPAgo), new DParametro('@PersonalId', PersonalId),
        new DParametro('@Glosa', Glosa), new DParametro('@NroRecibo', NroComprobante), new DParametro('@Monto', Pago), new DParametro('@usuario', L_Usuario()),
    ]), TransaccionCompraId);
}
async function L_prListarPagosCreditoCompra(id) { return _spUsu('MAM_CreditosCompras', 5, [new DParametro('@CreditoCompraId', id)]); }
async function L_prListarPagosTodosCuentasPorPagar() { return _spUsu('MAM_CreditosCompras', 6); }
async function L_prEliminarPagosCuentaPorPagar(id) { return (await _spUsu('MAM_CreditosCompras', 7, [new DParametro('@TransaccionCompraId', id)])).length > 0; }
async function L_EstadoDeCuentasPorPagar(id) { return _spUsu('MAM_CreditosCompras', 8, [new DParametro('@CreditoCompraId', id)]); }

// Tec_ConceptosFijos (btnConceptoFijos)  SP: MAM_ConceptosFijos
async function L_prListarOperacionesConceptosFijos() { return _spUsu('MAM_ConceptosFijos', 4); }
const _paramsCF = (tipo, c) => [
    new DParametro('@tipo', tipo), new DParametro('@Id', c._numi), new DParametro('@NombreConcepto', c.NombreConcepto), new DParametro('@Porcentaje', c.Porcentaje),
    new DParametro('@OperacionId', c.OperacionId), new DParametro('@TipoConcepto', c.TipoConcepto), new DParametro('@Estado', c.Estado), new DParametro('@usuario', L_Usuario()),
];
async function L_prConceptoFijoInsertar(c) { return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_ConceptosFijos', _paramsCF(1, c)), c._numi); }
async function L_prConceptoFijoModificar(c) { return (await D_ProcedimientoConParam('MAM_ConceptosFijos', _paramsCF(2, c))).length > 0; }
// Tec_Contratos (btnContratos)  SP: MAM_Contratos
async function ListaConceptosContratos(ContratoId) { return _spUsu('MAM_Contratos', 4, [new DParametro('@id', ContratoId)]); }
async function ListaConceptosFijosContrato() { return _spUsu('MAM_Contratos', 5); }
const _paramsContrato = (tipo, c, dt) => [
    new DParametro('@tipo', tipo), new DParametro('@Id', c._numi), new DParametro('@PersonalId', c.PersonalId), new DParametro('@TipoContratoId', c.TipoContratoId),
    new DParametro('@Cargo', c.Cargo), new DParametro('@SueldoBase', c.SueldoBase), new DParametro('@InicioContrato', c.InicioContrato), new DParametro('@FinContrato', c.FinContrato),
    new DParametro('@Usuario', L_Usuario()), new DParametro('@estado', 1), new DParametro('@Indefinido', c.Indefinido), new DParametro('@DetalleContratoConcepto', '', dt),
];
async function InsertarContratos(c, dt) { return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Contratos', _paramsContrato(1, c, dt)), c._numi); }
async function ModificarContratos(c, dt) { return (await D_ProcedimientoConParam('MAM_Contratos', _paramsContrato(2, c, dt))).length > 0; }
// Tec_RegistrarPersonalAPlanilla / Tec_Listarplanilla (btnRegistrarPlanilla / btnPlanilla)  SP: MAM_Planilla
async function InsertarPlanilla(p, dt) {
    return _fnResultadoGrabar(await D_ProcedimientoConParam('MAM_Planilla', [
        new DParametro('@tipo', 1), new DParametro('@Id', p._numi), new DParametro('@ContratoId', p.ContratoId), new DParametro('@Anio', p.Anio), new DParametro('@Mes', p.Mes),
        new DParametro('@Sueldo', p.Sueldo), new DParametro('@SueldoBruto', p.SueldoBruto), new DParametro('@SueldoNeto', p.SueldoNeto), new DParametro('@Usuario', L_Usuario()),
        new DParametro('@PlanillaConcepto', '', dt),
    ]), p._numi);
}
async function ModificarPlanillaSoloDetalle(Id, dt) {
    return (await D_ProcedimientoConParam('MAM_Planilla', [new DParametro('@tipo', 7), new DParametro('@Id', Id), new DParametro('@Usuario', L_Usuario()), new DParametro('@PlanillaConcepto', '', dt)])).length > 0;
}
async function ListaConceptosPlanilla(PlanillaId, TipoConcepto) { return _spUsu('MAM_Planilla', 4, [new DParametro('@Id', PlanillaId), new DParametro('@TipoConcepto', TipoConcepto)]); }
async function ListarPersonalSinPlanilla(Mes, Anio) { return _spUsu('MAM_Planilla', 5, [new DParametro('@Mes', Mes), new DParametro('@Anio', Anio)]); }
async function ListarConceptoVariablePlanilla() { return _spUsu('MAM_Planilla', 6); }
async function ListarPlanilla(Mes, Anio, Todos) { return _spUsu('MAM_Planilla', 3, [new DParametro('@Mes', Mes), new DParametro('@Anio', Anio), new DParametro('@Todos', Todos)]); }
async function ListarPlanillaReporte(Mes, Anio, Todos) { return _spUsu('MAM_Planilla', 9, [new DParametro('@Mes', Mes), new DParametro('@Anio', Anio), new DParametro('@Todos', Todos)]); }
async function ReporteBoletaSalarioIndividual(id) { return _spUsu('MAM_Planilla', 8, [new DParametro('@id', id)]); }

// P_GenerarReporte (Nota de venta del escritorio)
async function ListarVentaRecibo(VentaId) { return _spUsu('MAM_Ventas', 6, [new DParametro('@id', VentaId)]); }

module.exports = {
    ListarVentaRecibo,
    L_prListarOperacionesConceptosFijos, L_prConceptoFijoInsertar, L_prConceptoFijoModificar, ListaConceptosContratos, ListaConceptosFijosContrato,
    InsertarContratos, ModificarContratos, InsertarPlanilla, ModificarPlanillaSoloDetalle, ListaConceptosPlanilla, ListarPersonalSinPlanilla,
    ListarConceptoVariablePlanilla, ListarPlanilla, ListarPlanillaReporte, ReporteBoletaSalarioIndividual,
    ListaComprasDetalles, L_prListarProductosCompras, ListarProveedores, ReporteCompras, ComprasInsertar, ComprasModificar,
    ListaComprasHistorico, ListaComprasTotalesHistorico, ListaComprasProductosHistorico, ReporteComprasPorProductos,
    L_prListarCreditosPagados, L_prListarPagosPendientesFiltros, L_prGrabarPagosCreditoCompras, L_prListarPagosCreditoCompra,
    L_prListarPagosTodosCuentasPorPagar, L_prEliminarPagosCuentaPorPagar, L_EstadoDeCuentasPorPagar,
    L_prSucursalInsertar, L_prSucursalActualizar, L_prSucursalBorrar,
    L_prMapaCLienteGeneral, L_prMapaCLienteGeneralPorZona,
    L_prListarDepositos, L_prListarTiposMovimientos, L_prListarDetalleMovimiento, L_prListarProductosLote, L_prMovimientoInsertar, L_prMovimientoActualizar,
    ReporteSaldosTodosAlmacenesMayorA0, ReporteSaldosTodosAlmacenesTodos, ReporteSaldosUnAlmacenTodosCantidad, ReporteSaldosUnAlmacenCantidadMayor0,
    L_fnObtenerHistorialProductoGeneral, L_fnObtenerHistorialProducto, L_fnListarProductosQueTienenMovimientos, ReporteSaldosValorados, L_prListarProductosStockVencidos,
    ListaDetalleGastosCierreId, ListarConciliacionesCaja, ListaDetallePedidosconciliacion, ListaCobranzasPorPersonalSinCierre, ListaCobranzasPorPersonalconCierre,
    DatosreporteCierreCaja, InsertarCierreCaja, ModificarCierreCaja,
    L_prListarCaja, L_prListarTipoMovimientoCaja, InsertarTipoMovimiento, ListarReporteIngresoEgreso, L_prIngresoSalidaInsertar, L_prIngresoSalidaModificar,
    ReporteIngresosEgresosMes, ReporteIngresoEgresoDetallado, ReporteGastosFecha, L_prListarHistorico, L_prListarAuditoriaVentas, L_prListarAuditoriaVentasDetalle,
    ListarProveedoresCombo, ListarCategoriasVentas, ListarProductosBuscador, L_prGenerarReporteProductosPorVentas, L_fnListarZonasAsignadas, L_prRegistrarAsignacion,
    ListaDetalleDespachoProductos, ListarProductosSeleccionables, GenerarDatosDespachoReporte, CargarProductosAutomatico, CargarReferenciaVentas,
    InsertarDespachoProductos, ModificarDespachoProductos,
    ListarProductosSalidasConciliacion, ListarTodasSalidas, ListarProductosEntregadoConciliaciones, ListarProductosDetalleConciliacion, ModificarConciliacion,
    ListarSoloChoferes, ListarConciliacion, ListaPedidosAsignadosByChofer, ListaProductosAsignadosByChofer, ListaClientesAsignadosByChofer,
    ListaPedidosEntregadosByChofer, ListaPedidosAnulados, VentaInsertarAsignaciones, VentaEntregarPedidos, VentaQuitarAsignaciones,
    VentaRevertiEstadoEntregado, VentaAnularPedidos, VentaRevertirAnulacion,
    ListaProformaDetalles, ListarProformaRecibo, ProformaInsertar, ProformaModificar,
    ReporteVentasAtendidasTodos, ReporteVentasAtendidasTodosUnVendedor, ReporteVentasAtendidasTodosUnCliente, ReporteVentasAtendidasTodosUnVendedorUnCliente,
    ReporteVentasUtilidadesTodos, ReporteVentasUtilidadesTodosUnVendedor, ReporteListarPersonal, ReporteVentasRendimientoPersonal, ReporteVentasPorProductos,
    ReporteHistoricoCobrosTodos, ReporteHistoricoCobrosbyVendedor, L_prListarProductosKardex,
    L_prListaPersonalCB, L_prListarSubcategoriaAsignados, L_prListarProductosPorCategoria, L_prListarProductosPorCategoriaTotalV2PorCategoria,
    L_prListarCreditosPagadosCliente, L_prListarPagosPendientesFiltrosClientes, L_prGrabarPagosCreditoVentas, L_prListarPagosCreditoVenta,
    L_prListarPagosTodosCuentasPorCobrar, L_prEliminarPagosCuentaPorCobrar, L_EstadoDeCuentasPorCobrar, ListarPersonalCredito,
    L_prGuardarIncrementoVentaHoy, L_prListarHistorialIncrementoVenta,
    InsertarPersonal, ModificarPersonal, InsertarProveedor, ModificarProveedor,
    L_fnPedidosEstados, L_prLeerConfiguracion, L_fnObtenerIncrementoVentaHoy, ListarCliente, ListaVentasDetalles,
    L_prListarProductosVentas, L_prListarProductosVentasConciliaciones, L_prVerificarConcicliacionAbierta, ObtenerImagenEmpresa,
    VentaInsertar, VentaModificar, L_prListarVentasGeneralFiltroFecha, ListaVentasDetallePago, L_prListarProductosKitsVenta, L_ObtenerTipoCambio,
    L_prListarGeneralCierreCajero, L_prListarVentaCierresCaja, L_prListarVentaCierresCajaPendiente, L_prListarCobranzaCierresCaja, L_prListarCobrosCajaPendiente,
    L_prListarMovimientosIngresoEgresoCierre, L_prListarMovimientosIngresoEgresoCierrePendiente, L_prListarEfectivoCortesCierre, L_prListarIdModulosCierre,
    L_prCierreCajeroInsertar, L_prCierreCajeroModificar,
    L_fnObtenerHistorialCajaGeneral, L_fnObtenerHistorialCajaGeneralDesdeHasta, ReporteIngresoEgresoDetalladoCaja, L_ObtenerConfiguraciones,
    ListaProductosKits, ListarProductosKits, InsertarKit, ModificarKit,
    L_prListarGeneralFechas, L_prListarPagosPendientes, L_prListarPagosPendientesClientes, ReporteVentasProductosMasVendido,
    L_prListarProductosConStock0, L_prListarProductosConStockMinimo, ListaPedidosPendientesAsignaciones, ListarVentasVendedorFecha,
    L_fnListarCategoriaPrecio, L_prListarProductosTodosInventario,
    _columnasDe,
    L_prClasificadorGrabar,
    InsertarCliente, ModificarCliente, L_prClienteBorrar, L_prListaCategoriasPrecios, L_prListarZonas,
    GeneralZona, L_fnListarPuntosPorZona, InsertarZona, ActualizarZona, EliminarZona,
    L_prCargarImagenesRecepcion, L_prProductoBorrar, L_prProductoInsertar, L_prProductoModificar, L_prListaCategorias, L_prListarProveedores, ListPreciosDetalles,
    L_fnGeneralCategorias, L_fnGeneralSucursales, L_fnListarProductos, L_fnListarCategorias, L_fnListarProductosConPrecios,
    L_fnGrabarCategorias, L_fnGrabarPrecios, L_fnEliminarCategoria,
    L_prConUsuario,
    L_Usuario,
    L_prLibreriaDetalleGeneral,
    L_Validar_Usuario,
    L_RolDetalle_General,
    ListarPersonal,
    ListarPersonalById,
    L_prListarGeneral,
    L_prBorrarRegistro,
    InsertarEmpresa,
    ModificarEmpresa,
    L_prCategoriaGeneral,
    L_prCategoriaBorrar,
    L_prCategoriaInsertar,
    L_prCategoriaModificar,
    L_prUsuarioGeneral,
    L_prListaRolesUsuarios,
    L_prListaEmpresasUsuarios,
    L_prUsuarioBorrar,
    L_prUsuarioInsertar,
    L_prUsuarioModificar,
    L_prRolGeneral,
    L_prRolDetalleGeneral,
    L_prRolGrabar,
    L_prRolModificar,
    L_prRolBorrar,
    COLUMNAS_ROL_DETALLE,
};
