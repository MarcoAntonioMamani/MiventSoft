
Imports System.Data
Imports System.Data.SqlClient
Imports Datos.AccesoDatos

Public Class AccesoLogica

    Public Shared L_Usuario As String = "DEFAULT"



#Region "METODOS PRIVADOS"
    Public Shared Sub L_prAbrirConexion(Optional Ip As String = "", Optional UsuarioSql As String = "", Optional ClaveSql As String = "", Optional NombreBD As String = "")
        D_abrirConexion(Ip, UsuarioSql, ClaveSql, NombreBD)
    End Sub

#End Region

#Region "LIBRERIAS TEC"


    Public Shared Function L_prLibreriaDetalleGeneral(_IdClasificador As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@IdClasificador", _IdClasificador))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Clasificadores", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prClasificadorGrabar(ByRef _numi As String, IdClasificador As Integer, _desc1 As String) As Boolean
        Dim _Error As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@IdClasificador", IdClasificador))
        _listParam.Add(New Datos.DParametro("@descripcion", _desc1))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Clasificadores", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _Error = False
        Else
            _Error = True
        End If
        Return Not _Error
    End Function


#End Region


#Region "METODOS PARA EL CONTROL DE USUARIOS Y PRIVILEGIOS"



#Region "Roles"

    Public Shared Function L_RolDetalle_General(_Modo As Integer, Optional _idCabecera As String = "", Optional _idModulo As String = "") As DataTable
        Dim _Tabla As DataTable
        Dim _Where As String
        If _Modo = 0 Then
            _Where = " RolesDetalles.RolId = RolesDetalles.RolId"
        Else
            _Where = " RolesDetalles.RolId=" + _idCabecera + " and Programas.Modulo=" + _idModulo + " and RolesDetalles.ProgramaId=Programas.Id"
        End If
        _Tabla = D_Datos_Tabla("RolesDetalles.Id,RolesDetalles.RolId,RolesDetalles.Ver,RolesDetalles.Insertar,RolesDetalles.Modificar,RolesDetalles.Eliminar,Programas.IdPrograma,Programas.DescripcionPrograma", "RolesDetalles,Programas", _Where)
        Return _Tabla
    End Function




#End Region

#Region "Usuarios"

    Public Shared Function L_Validar_Usuario(_Nom As String, _Pass As String) As DataTable
        Dim _Tabla As DataTable
        _Tabla = D_Datos_Tabla("Id,RolId,SucursalId,IdPersonal", "Usuarios", "NombreUsuario = '" + _Nom + "' AND Contrasena = '" + _Pass + "'")
        Return _Tabla
    End Function
#End Region

#End Region


#Region "TY004 CLIENTES"

    Public Shared Function L_prMapaCLienteGeneral() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 6))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)

        Return _Tabla
    End Function


    Public Shared Function L_prMapaCLienteGeneralPorZona(_zona As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@ZonaId", _zona))
        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)

        Return _Tabla
    End Function



#End Region
#Region "TY006 Categorias"


    Public Shared Function L_fnGeneralCategorias() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Precios", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnGeneralSucursales() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Precios", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarProductos() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 6))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Precios", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnListarCategorias() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Precios", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnListarProductosConPrecios(_almacen As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Precios", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnGrabarCategorias(_ygnumi As String, cod As String, desc As String, tipo As Integer) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Codigo", cod))
        _listParam.Add(New Datos.DParametro("@Descripcion", desc))
        _listParam.Add(New Datos.DParametro("@TipoCategoria", tipo))
        _listParam.Add(New Datos.DParametro("@Usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Precios", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _ygnumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnGrabarPrecios(_ygnumi As String, _almacen As Integer, _precio As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _listParam.Add(New Datos.DParametro("@TY007", "", _precio))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Precios", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _ygnumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

#End Region
#Region "TZ001 Zonas"

    Public Shared Function L_fnListarPuntosPorZona(_zanumi As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@Id", _zanumi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Zonas", _listParam)

        Return _Tabla
    End Function


#End Region
#Region "Tec Eliminar Precios"


    Public Shared Function L_fnEliminarCategoria(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@Id", numi))
            _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

            _Tabla = D_ProcedimientoConParam("sp_Mam_Precios", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If

            Return _resultado
    End Function
#End Region
#Region "Personal Tec"

    Public Shared Function InsertarPersonal(_Id As String, NombrePersonal As String, Direccion As String,
                           Telefono01 As String, TipoDocumento As Integer,
                                             NroDocumento As String, TipoPersonal As Integer,
                                             Estado As Integer, EmpresaId As Integer) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '    @Id ,@NombrePersonal ,@DireccionPersonal ,@Telefono01 
        ',@TipoDocumento ,@NroDocumento ,@TipoPersonal ,@estado,@EmpresaId  ,@newFecha ,@newHora ,@usuario
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@NombrePersonal", NombrePersonal))
        _listParam.Add(New Datos.DParametro("@DireccionPersonal", Direccion))
        _listParam.Add(New Datos.DParametro("@Telefono01", Telefono01))
        _listParam.Add(New Datos.DParametro("@TipoDocumento", TipoDocumento))
        _listParam.Add(New Datos.DParametro("@estado", Estado))
        _listParam.Add(New Datos.DParametro("@NroDocumento", NroDocumento))

        _listParam.Add(New Datos.DParametro("@TipoPersonal", TipoPersonal))
        _listParam.Add(New Datos.DParametro("@EmpresaId", EmpresaId))

        _Tabla = D_ProcedimientoConParam("MAM_Personal", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _Id = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function


    Public Shared Function ModificarPersonal(_Id As String, NombrePersonal As String, Direccion As String,
                           Telefono01 As String, TipoDocumento As Integer,
                                             NroDocumento As String, TipoPersonal As Integer,
                                             Estado As Integer, EmpresaId As Integer) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '    @Id ,@NombrePersonal ,@DireccionPersonal ,@Telefono01 
        ',@TipoDocumento ,@NroDocumento ,@TipoPersonal ,@estado,@EmpresaId  ,@newFecha ,@newHora ,@usuario
        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@NombrePersonal", NombrePersonal))
        _listParam.Add(New Datos.DParametro("@DireccionPersonal", Direccion))
        _listParam.Add(New Datos.DParametro("@Telefono01", Telefono01))
        _listParam.Add(New Datos.DParametro("@TipoDocumento", TipoDocumento))
        _listParam.Add(New Datos.DParametro("@estado", Estado))
        _listParam.Add(New Datos.DParametro("@NroDocumento", NroDocumento))

        _listParam.Add(New Datos.DParametro("@TipoPersonal", TipoPersonal))
        _listParam.Add(New Datos.DParametro("@EmpresaId", EmpresaId))

        _Tabla = D_ProcedimientoConParam("MAM_Personal", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _Id = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region
#Region "Proveedor Tec"

    Public Shared Function InsertarProveedor(_Id As String, NombreProveedor As String, Direccion As String,
                           Telefono01 As String, Telefono02 As String, Descripcion As String, TipoDocumento As Integer,
                                             NroDocumento As String, Estado As Integer) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '     @Id ,@NombreProveedor ,@DireccionProveedor ,@Telefono01 ,@Telefono02 ,
        '@Descripcion ,@TipoDocumento ,@NroDocumento ,@estado ,@newFecha ,@newHora ,@usuario
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@NombreProveedor", NombreProveedor))
        _listParam.Add(New Datos.DParametro("@DireccionProveedor", Direccion))
        _listParam.Add(New Datos.DParametro("@Telefono01", Telefono01))
        _listParam.Add(New Datos.DParametro("@Telefono02", Telefono02))
        _listParam.Add(New Datos.DParametro("@Descripcion", Descripcion))
        _listParam.Add(New Datos.DParametro("@TipoDocumento", TipoDocumento))
        _listParam.Add(New Datos.DParametro("@estado", Estado))
        _listParam.Add(New Datos.DParametro("@NroDocumento", NroDocumento))

        _Tabla = D_ProcedimientoConParam("MAM_Proveedores", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _Id = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function ModificarProveedor(_Id As String, NombreProveedor As String, Direccion As String,
                           Telefono01 As String, Telefono02 As String, Descripcion As String, TipoDocumento As Integer,
                                             NroDocumento As String, Estado As Integer) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '     @Id ,@NombreProveedor ,@DireccionProveedor ,@Telefono01 ,@Telefono02 ,
        '@Descripcion ,@TipoDocumento ,@NroDocumento ,@estado ,@newFecha ,@newHora ,@usuario
        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@NombreProveedor", NombreProveedor))
        _listParam.Add(New Datos.DParametro("@DireccionProveedor", Direccion))
        _listParam.Add(New Datos.DParametro("@Telefono01", Telefono01))
        _listParam.Add(New Datos.DParametro("@Telefono02", Telefono02))
        _listParam.Add(New Datos.DParametro("@Descripcion", Descripcion))
        _listParam.Add(New Datos.DParametro("@TipoDocumento", TipoDocumento))
        _listParam.Add(New Datos.DParametro("@estado", Estado))
        _listParam.Add(New Datos.DParametro("@NroDocumento", NroDocumento))

        _Tabla = D_ProcedimientoConParam("MAM_Proveedores", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _Id = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region


#Region "Empresas TeC"



    Public Shared Function InsertarEmpresa(_Id As String, Nombre As String, Descripcion As String, Latitud As Double,
                                           Longitud As Double, Imagen As String, Propietario As String,
         Telefono As String, Nit As String, Ciudad As String, Direccion As String) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        ' @Nombre ,@Descripcion ,@Descripcion ,@Latitud ,@Longitud ,@Imagen ,@Propietario ,@Telefono ,
        '@Nit ,12,12,1,1,'Estoy%20interesado%20en%20sus%20productos..',@Ciudad ,@Direccion 

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@Nombre", Nombre))
        _listParam.Add(New Datos.DParametro("@Descripcion", Descripcion))
        _listParam.Add(New Datos.DParametro("@Latitud", Latitud))
        _listParam.Add(New Datos.DParametro("@Longitud", Longitud))
        _listParam.Add(New Datos.DParametro("@Imagen", Imagen))
        _listParam.Add(New Datos.DParametro("@Propietario", Propietario))
        _listParam.Add(New Datos.DParametro("@Telefono", Telefono))
        _listParam.Add(New Datos.DParametro("@Nit", Nit))
        _listParam.Add(New Datos.DParametro("@Ciudad", Ciudad))
        _listParam.Add(New Datos.DParametro("@Direccion", Direccion))

        _Tabla = D_ProcedimientoConParam("MAM_Empresas", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _Id = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function


    Public Shared Function ModificarEmpresa(_Id As String, Nombre As String, Descripcion As String, Latitud As Double,
                                           Longitud As Double, Imagen As String, Propietario As String,
         Telefono As String, Nit As String, Ciudad As String, Direccion As String) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        ' @Nombre ,@Descripcion ,@Descripcion ,@Latitud ,@Longitud ,@Imagen ,@Propietario ,@Telefono ,
        '@Nit ,12,12,1,1,'Estoy%20interesado%20en%20sus%20productos..',@Ciudad ,@Direccion 

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@Nombre", Nombre))
        _listParam.Add(New Datos.DParametro("@Descripcion", Descripcion))
        _listParam.Add(New Datos.DParametro("@Latitud", Latitud))
        _listParam.Add(New Datos.DParametro("@Longitud", Longitud))
        _listParam.Add(New Datos.DParametro("@Imagen", Imagen))
        _listParam.Add(New Datos.DParametro("@Propietario", Propietario))
        _listParam.Add(New Datos.DParametro("@Telefono", Telefono))
        _listParam.Add(New Datos.DParametro("@Nit", Nit))
        _listParam.Add(New Datos.DParametro("@Ciudad", Ciudad))
        _listParam.Add(New Datos.DParametro("@Direccion", Direccion))

        _Tabla = D_ProcedimientoConParam("MAM_Empresas", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _Id = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region
#Region "Clientes Tec"

    Public Shared Function InsertarCliente(_Id As String, IdZona As Integer, IdPrecio As Integer, CodigoExterno As String, NombreCliente As String,
                                           Direccion As String, Telefono As String, TipoDocumento As Integer, NroDocumento As String,
                                           RazonSocial As String, nit As String, estado As Integer, FechaIngreso As String, Latitud As Double,
                                           Longitud As Double) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '@Id,@ZonaId ,@PrecioCategoriaId ,@CodigoExterno ,@NombreCliente ,@DireccionCliente ,@Telefono ,
        '@Observacion ,@TipoDocumento ,@NroDocumento ,@RazonSocial ,@nit ,@estado ,@FechaIngreso ,@FechaUltimaVenta ,@ImagenCliente ,
        '@Latitud ,@Longitud ,@newFecha ,@newHora ,@usuario
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@ZonaId", IdZona))
        _listParam.Add(New Datos.DParametro("@PrecioCategoriaId", IdPrecio))
        _listParam.Add(New Datos.DParametro("@CodigoExterno", CodigoExterno))
        _listParam.Add(New Datos.DParametro("@NombreCliente", NombreCliente))
        _listParam.Add(New Datos.DParametro("@DireccionCliente", Direccion))
        _listParam.Add(New Datos.DParametro("@Telefono", Telefono))
        _listParam.Add(New Datos.DParametro("@estado", estado))
        _listParam.Add(New Datos.DParametro("@Observacion", ""))
        _listParam.Add(New Datos.DParametro("@TipoDocumento", TipoDocumento))
        _listParam.Add(New Datos.DParametro("@NroDocumento", NroDocumento))
        _listParam.Add(New Datos.DParametro("@RazonSocial", RazonSocial))
        _listParam.Add(New Datos.DParametro("@nit", nit))
        _listParam.Add(New Datos.DParametro("@FechaIngreso", FechaIngreso))
        _listParam.Add(New Datos.DParametro("@Latitud", Latitud))
        _listParam.Add(New Datos.DParametro("@Longitud", Longitud))
        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _Id = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function InsertarClienteFormularioExterno(_Id As String, NombreCliente As String, TipoDocumento As Integer, NroDocumento As String,
                                           RazonSocial As String, nit As String, CategoriaPrecioId As Integer) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 8))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@NombreCliente", NombreCliente))
        _listParam.Add(New Datos.DParametro("@Observacion", ""))
        _listParam.Add(New Datos.DParametro("@TipoDocumento", TipoDocumento))
        _listParam.Add(New Datos.DParametro("@NroDocumento", NroDocumento))
        _listParam.Add(New Datos.DParametro("@RazonSocial", RazonSocial))
        _listParam.Add(New Datos.DParametro("@PrecioCategoriaId", CategoriaPrecioId))
        _listParam.Add(New Datos.DParametro("@nit", nit))
        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)
        Return _Tabla
    End Function

    Public Shared Function L_prClienteBorrar(_numi As String, ByRef _mensaje As String) As Boolean

        Dim _resultado As Boolean

        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", -1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function ModificarCliente(_Id As String, IdZona As Integer, IdPrecio As Integer, CodigoExterno As String, NombreCliente As String,
                                           Direccion As String, Telefono As String, TipoDocumento As Integer, NroDocumento As String,
                                           RazonSocial As String, nit As String, estado As Integer, FechaIngreso As String, Latitud As Double,
                                           Longitud As Double) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '@Id,@ZonaId ,@PrecioCategoriaId ,@CodigoExterno ,@NombreCliente ,@DireccionCliente ,@Telefono ,
        '@Observacion ,@TipoDocumento ,@NroDocumento ,@RazonSocial ,@nit ,@estado ,@FechaIngreso ,@FechaUltimaVenta ,@ImagenCliente ,
        '@Latitud ,@Longitud ,@newFecha ,@newHora ,@usuario
        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@ZonaId", IdZona))
        _listParam.Add(New Datos.DParametro("@PrecioCategoriaId", IdPrecio))
        _listParam.Add(New Datos.DParametro("@CodigoExterno", CodigoExterno))
        _listParam.Add(New Datos.DParametro("@NombreCliente", NombreCliente))
        _listParam.Add(New Datos.DParametro("@DireccionCliente", Direccion))
        _listParam.Add(New Datos.DParametro("@Telefono", Telefono))
        _listParam.Add(New Datos.DParametro("@estado", estado))
        _listParam.Add(New Datos.DParametro("@Observacion", ""))
        _listParam.Add(New Datos.DParametro("@TipoDocumento", TipoDocumento))
        _listParam.Add(New Datos.DParametro("@NroDocumento", NroDocumento))
        _listParam.Add(New Datos.DParametro("@RazonSocial", RazonSocial))
        _listParam.Add(New Datos.DParametro("@nit", nit))
        _listParam.Add(New Datos.DParametro("@FechaIngreso", FechaIngreso))
        _listParam.Add(New Datos.DParametro("@Latitud", Latitud))
        _listParam.Add(New Datos.DParametro("@Longitud", Longitud))
        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _Id = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region
#Region "Tec Reporte Inventario"

    Public Shared Function ReporteSaldosTodosAlmacenesMayorA0() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 9))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnObtenerHistorialProductoGeneral(ProductoId As Integer, FechaI As String, DepositoId As Integer) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 14))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@ProductoId", ProductoId))
        _listParam.Add(New Datos.DParametro("@FechaDocumento", FechaI))
        _listParam.Add(New Datos.DParametro("@DepositoId", DepositoId))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnObtenerHistorialProducto(_codProducto As Integer, FechaI As String, FechaF As String, _almacen As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 15))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@ProductoId", _codProducto))
        _listParam.Add(New Datos.DParametro("@FechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@FechaF", FechaF))
        _listParam.Add(New Datos.DParametro("@DepositoId", _almacen))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function
    Public Shared Function ReporteSaldosTodosAlmacenesTodos() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 10))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function

    Public Shared Function ReporteSaldosUnAlmacenTodosCantidad(depositoId As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 11))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@DepositoId", depositoId))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function

    Public Shared Function ReporteSaldosUnAlmacenCantidadMayor0(depositoId As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 12))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@DepositoId", depositoId))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function
#End Region
#Region "Tec Zonas"
    Public Shared Function InsertarZona(_Id As String,
        _Nombre As String, _Descripcion As String, _zacolor As String, point As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        ''(@Id,@NombreZona  ,@DescripcionZona  ,@Color ,@newFecha,@newHora,@Usuario)
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@NombreZona", _Nombre))
        _listParam.Add(New Datos.DParametro("@DescripcionZona", _Descripcion))
        _listParam.Add(New Datos.DParametro("@Color", _zacolor))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@ZonasPuntos", "", point))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Zonas", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _Id = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
    Public Shared Function GeneralZona() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Zonas", _listParam)

        Return _Tabla
    End Function
    Public Shared Function EliminarZona(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
        _listParam.Add(New Datos.DParametro("@Id", numi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("sp_Mam_Zonas", _listParam)

        If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If

            Return _resultado
    End Function
    Public Shared Function ActualizarZona(_Id As String,
        _Nombre As String, _Descripcion As String, _zacolor As String, point As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        ''(@Id,@NombreZona  ,@DescripcionZona  ,@Color ,@newFecha,@newHora,@Usuario)
        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _Id))
        _listParam.Add(New Datos.DParametro("@NombreZona", _Nombre))
        _listParam.Add(New Datos.DParametro("@DescripcionZona", _Descripcion))
        _listParam.Add(New Datos.DParametro("@Color", _zacolor))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@ZonasPuntos", "", point))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Zonas", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region
#Region "Ventas TecBrinc"
    Public Shared Function ListaVentasDetalles(VentaId As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@id", VentaId))
        _Tabla = D_ProcedimientoConParam("MAM_Ventas", _listParam)

        Return _Tabla
    End Function

    Public Shared Function ListarVentaRecibo(VentaId As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 6))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@id", VentaId))
        _Tabla = D_ProcedimientoConParam("MAM_Ventas", _listParam)

        Return _Tabla
    End Function
    Public Shared Function ReporteCompras(CompraId As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@id", CompraId))
        _Tabla = D_ProcedimientoConParam("MAM_Compras", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prListarProductosVentas(_deposito As Integer, _ClienteId As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@SucursalId", _deposito))
        _listParam.Add(New Datos.DParametro("@ClienteId", _ClienteId))
        _Tabla = D_ProcedimientoConParam("MAM_Ventas", _listParam)

        Return _Tabla
    End Function

    Public Shared Function VentaInsertar(ByRef _numi As String, AlmacenId As Integer,
                                           FechaTransacccion As String, PersonalId As Integer, ClienteId As Integer, TipoVenta As Integer,
       FechaVencCredito As String, Moneda As Integer, estado As Integer, glosa As String,
                                           TotalCompra As Double, _dtDetalle As DataTable,
                                           Descuento As Double) As Boolean
        Dim _resultado As Boolean

        '    @Id ,@SucursalId ,@FechaVenta ,@PersonalId ,@TipoVenta ,
        '@FechaVencimientoCredito ,@ClienteId ,@MonedaVenta ,@Estado ,@Glosa ,
        '@Descuento ,@TotalVenta 

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@SucursalId", AlmacenId))
        _listParam.Add(New Datos.DParametro("@FechaVenta", FechaTransacccion))
        _listParam.Add(New Datos.DParametro("@PersonalId", PersonalId))
        _listParam.Add(New Datos.DParametro("@TipoVenta", TipoVenta))
        _listParam.Add(New Datos.DParametro("@FechaVencimientoCredito", FechaVencCredito))
        _listParam.Add(New Datos.DParametro("@ClienteId", ClienteId))


        _listParam.Add(New Datos.DParametro("@MonedaVenta", Moneda))
        _listParam.Add(New Datos.DParametro("@Estado", estado))
        _listParam.Add(New Datos.DParametro("@Glosa", glosa))
        _listParam.Add(New Datos.DParametro("@TotalVenta", TotalCompra))
        _listParam.Add(New Datos.DParametro("@Descuento", Descuento))
        _listParam.Add(New Datos.DParametro("@VentaDetalleType", "", _dtDetalle))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Ventas", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function VentaModificar(ByRef _numi As String, AlmacenId As Integer,
                                           FechaTransacccion As String, PersonalId As Integer, ClienteId As Integer, TipoVenta As Integer,
       FechaVencCredito As String, Moneda As Integer, estado As Integer, glosa As String,
                                           TotalCompra As Double, _dtDetalle As DataTable,
                                           Descuento As Double) As Boolean
        Dim _resultado As Boolean

        '    @Id ,@SucursalId ,@FechaVenta ,@PersonalId ,@TipoVenta ,
        '@FechaVencimientoCredito ,@ClienteId ,@MonedaVenta ,@Estado ,@Glosa ,
        '@Descuento ,@TotalVenta 

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@SucursalId", AlmacenId))
        _listParam.Add(New Datos.DParametro("@FechaVenta", FechaTransacccion))
        _listParam.Add(New Datos.DParametro("@PersonalId", PersonalId))
        _listParam.Add(New Datos.DParametro("@TipoVenta", TipoVenta))
        _listParam.Add(New Datos.DParametro("@FechaVencimientoCredito", FechaVencCredito))
        _listParam.Add(New Datos.DParametro("@ClienteId", ClienteId))


        _listParam.Add(New Datos.DParametro("@MonedaVenta", Moneda))
        _listParam.Add(New Datos.DParametro("@Estado", estado))
        _listParam.Add(New Datos.DParametro("@Glosa", glosa))
        _listParam.Add(New Datos.DParametro("@TotalVenta", TotalCompra))
        _listParam.Add(New Datos.DParametro("@Descuento", Descuento))
        _listParam.Add(New Datos.DParametro("@VentaDetalleType", "", _dtDetalle))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Ventas", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

#End Region
#Region "Compras TecBrinc"
    Public Shared Function ListaComprasDetalles(CompraId As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@id", CompraId))
        _Tabla = D_ProcedimientoConParam("MAM_Compras", _listParam)

        Return _Tabla
    End Function

    Public Shared Function ListarProveedores() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 6))
        _Tabla = D_ProcedimientoConParam("MAM_Compras", _listParam)

        Return _Tabla
    End Function

    Public Shared Function ListarPersonal() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 9))
        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)

        Return _Tabla
    End Function


    Public Shared Function ListarPersonalById(id As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 11))
        _listParam.Add(New Datos.DParametro("@Id", id))
        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)

        Return _Tabla
    End Function
    Public Shared Function ListarCliente() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 10))
        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prListarProductosCompras(_deposito As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@AlmacenId", _deposito))
        _Tabla = D_ProcedimientoConParam("MAM_Compras", _listParam)

        Return _Tabla
    End Function

    Public Shared Function ComprasInsertar(ByRef _numi As String, AlmacenId As Integer,
                                           FechaTransacccion As String, ProveedorId As Integer, TipoVenta As Integer,
       FechaVencCredito As String, Moneda As Integer, estado As Integer, glosa As String,
                                           TotalCompra As Double, EmpresaId As Integer, _dtDetalle As DataTable,
                                           Descuento As Double) As Boolean
        Dim _resultado As Boolean

        '     @Id ,@AlmacenId,@FechaTransaccion ,@Proveedor  ,@TipoVenta,
        '@FechaVencimientoCredito,@Moneda ,@Estado  ,@Glosa  ,@TotalCompra ,@EmpresaId

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@AlmacenId", AlmacenId))
        _listParam.Add(New Datos.DParametro("@FechaTransaccion", FechaTransacccion))
        _listParam.Add(New Datos.DParametro("@Proveedor", ProveedorId))
        _listParam.Add(New Datos.DParametro("@TipoVenta", TipoVenta))
        _listParam.Add(New Datos.DParametro("@FechaVencimientoCredito", FechaVencCredito))

        _listParam.Add(New Datos.DParametro("@Moneda", Moneda))
        _listParam.Add(New Datos.DParametro("@Estado", estado))
        _listParam.Add(New Datos.DParametro("@Glosa", glosa))
        _listParam.Add(New Datos.DParametro("@TotalCompra", TotalCompra))
        _listParam.Add(New Datos.DParametro("@EmpresaId", EmpresaId))
        _listParam.Add(New Datos.DParametro("@Descuento", Descuento))
        _listParam.Add(New Datos.DParametro("@detalle", "", _dtDetalle))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Compras", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function


    Public Shared Function ComprasModificar(ByRef _numi As String, AlmacenId As Integer,
                                           FechaTransacccion As String, ProveedorId As Integer, TipoVenta As Integer,
       FechaVencCredito As String, Moneda As Integer, estado As Integer, glosa As String,
                                           TotalCompra As Double, EmpresaId As Integer, _dtDetalle As DataTable,
                                            Descuento As Double) As Boolean
        Dim _resultado As Boolean

        '     @Id ,@AlmacenId,@FechaTransaccion ,@Proveedor  ,@TipoVenta,
        '@FechaVencimientoCredito,@Moneda ,@Estado  ,@Glosa  ,@TotalCompra ,@EmpresaId

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@AlmacenId", AlmacenId))
        _listParam.Add(New Datos.DParametro("@FechaTransaccion", FechaTransacccion))
        _listParam.Add(New Datos.DParametro("@Proveedor", ProveedorId))
        _listParam.Add(New Datos.DParametro("@TipoVenta", TipoVenta))
        _listParam.Add(New Datos.DParametro("@FechaVencimientoCredito", FechaVencCredito))
        _listParam.Add(New Datos.DParametro("@Descuento", Descuento))
        _listParam.Add(New Datos.DParametro("@Moneda", Moneda))
        _listParam.Add(New Datos.DParametro("@Estado", estado))
        _listParam.Add(New Datos.DParametro("@Glosa", glosa))
        _listParam.Add(New Datos.DParametro("@TotalCompra", TotalCompra))
        _listParam.Add(New Datos.DParametro("@EmpresaId", EmpresaId))

        _listParam.Add(New Datos.DParametro("@detalle", "", _dtDetalle))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Compras", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region
#Region "Movimientos TecBrinc"

    Public Shared Function L_prMovimientoInsertar(ByRef _numi As String, ConceptoId As Integer, DepositoId As Integer, Observacion As String, Estado As Integer, FechaDocumento As String, _dtDetalle As DataTable) As Boolean
        Dim _resultado As Boolean

        '@id, @ConceptoId , @id , @DepositoId , @Observacion , @Estado , @FechaDocumento ,
        '@newFecha, @newHora, @usuario

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@ConceptoId", ConceptoId))
        _listParam.Add(New Datos.DParametro("@DepositoId", DepositoId))
        _listParam.Add(New Datos.DParametro("@Observacion", Observacion))
        _listParam.Add(New Datos.DParametro("@Estado", Estado))
        _listParam.Add(New Datos.DParametro("@FechaDocumento", FechaDocumento))
        _listParam.Add(New Datos.DParametro("@detalle", "", _dtDetalle))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prMovimientoActualizar(ByRef _numi As String, ConceptoId As Integer, DepositoId As Integer, Observacion As String, Estado As Integer, FechaDocumento As String, _dtDetalle As DataTable) As Boolean
        Dim _resultado As Boolean

        '@id, @ConceptoId , @id , @DepositoId , @Observacion , @Estado , @FechaDocumento ,
        '@newFecha, @newHora, @usuario

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@ConceptoId", ConceptoId))
        _listParam.Add(New Datos.DParametro("@DepositoId", DepositoId))
        _listParam.Add(New Datos.DParametro("@Observacion", Observacion))
        _listParam.Add(New Datos.DParametro("@Estado", Estado))
        _listParam.Add(New Datos.DParametro("@FechaDocumento", FechaDocumento))
        _listParam.Add(New Datos.DParametro("@detalle", "", _dtDetalle))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prBorrarRegistro(_numi As String, ByRef _mensaje As String, sp As String) As Boolean

        Dim _resultado As Boolean

        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", -1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam(sp, _listParam)

        If _Tabla.Rows.Count > 0 Then
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prListarDetalleMovimiento(MovimientoId As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@id", MovimientoId))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prListarProductosLote(_deposito As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@DepositoId", _deposito))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prListarProductosKardex(_deposito As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 13))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@DepositoId", _deposito))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function

    Public Shared Function LotesPorProducto(_almacen As Integer, _codproducto As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 8))
        _listParam.Add(New Datos.DParametro("@DepositoId", _almacen))
        _listParam.Add(New Datos.DParametro("@ProductoId", _codproducto))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarProductosQueTienenMovimientos(FechaI As String, FechaF As String, DepositoId As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 16))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@fechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@fechaF", FechaF))
        _listParam.Add(New Datos.DParametro("@DepositoId", DepositoId))
        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function
#End Region
#Region "Producto TecBrinc"

    Public Shared Function L_prListarGeneral(NameSp As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam(NameSp, _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prCargarImagenesRecepcion(ProductoId As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@Id", ProductoId))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Productos", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prProductoBorrar(_numi As String, ByRef _mensaje As String) As Boolean

        Dim _resultado As Boolean

        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", -1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Productos", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function
    Public Shared Function L_prProductoInsertar(ByRef _numi As String, _CodigoExterno As String,
                                                _CodigoBarra As String, _NombreProducto As String,
        _Descripcion As String, _stockMinimo As Decimal, _estado As Integer, _CategoriaId As Integer, _EmpresaId As Integer, _ProveedorId As Integer, _MarcaId As Integer,
        _AttributoId As Integer, _FamiliaId As Integer, _UnidadVentaId As Integer, _UnidadMaximaId As Integer,
        _conversion As Double, _dtImagenes As DataTable) As Boolean
        Dim _resultado As Boolean

        '(@Id,@CodigoExterno ,@CodigoBarras ,@NombreProducto ,@DescripcionProducto ,
        '@StockMinimo ,@estado ,@CategoriaId ,@EmpresaId ,@ProveedorId ,@MarcaId ,@AttributoId ,@FamiliaId ,
        '@UnidadVentaId ,@UnidadMaximaId ,@Conversion ,@newFecha,@newHora,@usuario )

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@CodigoExterno", _CodigoExterno))
        _listParam.Add(New Datos.DParametro("@CodigoBarras", _CodigoBarra))
        _listParam.Add(New Datos.DParametro("@NombreProducto", _NombreProducto))
        _listParam.Add(New Datos.DParametro("@DescripcionProducto", _Descripcion))
        _listParam.Add(New Datos.DParametro("@Estado", _estado))
        _listParam.Add(New Datos.DParametro("@StockMinimo", _stockMinimo))
        _listParam.Add(New Datos.DParametro("@CategoriaId", _CategoriaId))
        _listParam.Add(New Datos.DParametro("@EmpresaId", _EmpresaId))
        _listParam.Add(New Datos.DParametro("@ProveedorId", _ProveedorId))
        _listParam.Add(New Datos.DParametro("@MarcaId", _MarcaId))
        _listParam.Add(New Datos.DParametro("@AttributoId", _AttributoId))
        _listParam.Add(New Datos.DParametro("@FamiliaId", _FamiliaId))

        _listParam.Add(New Datos.DParametro("@UnidadVentaId", _UnidadVentaId))
        _listParam.Add(New Datos.DParametro("@UnidadMaximaId", _UnidadMaximaId))
        _listParam.Add(New Datos.DParametro("@Conversion", _conversion))

        _listParam.Add(New Datos.DParametro("@TCL0064", "", _dtImagenes))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))


        _Tabla = D_ProcedimientoConParam("MAM_Productos", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prProductoModificar(ByRef _numi As String, _CodigoExterno As String,
                                                _CodigoBarra As String, _NombreProducto As String,
        _Descripcion As String, _stockMinimo As Decimal, _estado As Integer, _CategoriaId As Integer, _EmpresaId As Integer, _ProveedorId As Integer, _MarcaId As Integer,
        _AttributoId As Integer, _FamiliaId As Integer, _UnidadVentaId As Integer, _UnidadMaximaId As Integer,
        _conversion As Double, _dtImagenes As DataTable) As Boolean
        Dim _resultado As Boolean

        '(@Id,@CodigoExterno ,@CodigoBarras ,@NombreProducto ,@DescripcionProducto ,
        '@StockMinimo ,@estado ,@CategoriaId ,@EmpresaId ,@ProveedorId ,@MarcaId ,@AttributoId ,@FamiliaId ,
        '@UnidadVentaId ,@UnidadMaximaId ,@Conversion ,@newFecha,@newHora,@usuario )

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@CodigoExterno", _CodigoExterno))
        _listParam.Add(New Datos.DParametro("@CodigoBarras", _CodigoBarra))
        _listParam.Add(New Datos.DParametro("@NombreProducto", _NombreProducto))
        _listParam.Add(New Datos.DParametro("@DescripcionProducto", _Descripcion))
        _listParam.Add(New Datos.DParametro("@Estado", _estado))
        _listParam.Add(New Datos.DParametro("@StockMinimo", _stockMinimo))
        _listParam.Add(New Datos.DParametro("@CategoriaId", _CategoriaId))
        _listParam.Add(New Datos.DParametro("@EmpresaId", _EmpresaId))
        _listParam.Add(New Datos.DParametro("@ProveedorId", _ProveedorId))
        _listParam.Add(New Datos.DParametro("@MarcaId", _MarcaId))
        _listParam.Add(New Datos.DParametro("@AttributoId", _AttributoId))
        _listParam.Add(New Datos.DParametro("@FamiliaId", _FamiliaId))

        _listParam.Add(New Datos.DParametro("@UnidadVentaId", _UnidadVentaId))
        _listParam.Add(New Datos.DParametro("@UnidadMaximaId", _UnidadMaximaId))
        _listParam.Add(New Datos.DParametro("@Conversion", _conversion))
        _listParam.Add(New Datos.DParametro("@TCL0064", "", _dtImagenes))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))


        _Tabla = D_ProcedimientoConParam("MAM_Productos", _listParam)

        If _Tabla.Rows.Count > 0 Then

            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region
#Region "Categoria TecBrinc"

    Public Shared Function L_prCategoriaGeneral() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Categorias", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prCategoriaBorrar(_numi As String, ByRef _mensaje As String) As Boolean

        Dim _resultado As Boolean

        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", -1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Categorias", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _resultado = True
            'L_prTipoCambioGrabarHistorial(_numi, _fecha, _dolar, _ufv, "TIPO DE CAMBIO", 3)
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
    Public Shared Function L_prCategoriaInsertar(ByRef _numi As String, _NombreCategoria As String,
        _Descripcion As String, _estado As Integer, _Imagen As String, _VisibleApp As Integer,
                                                 _Empresa As Integer) As Boolean
        Dim _resultado As Boolean

        '     INSERT INTO Categorias  VALUES(@Id,@NombreCategoria ,@DescripcionCategoria ,@Estado ,
        '@Imagen ,@VisibleApp ,@newFecha,@newHora,@usuario )

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@NombreCategoria", _NombreCategoria))
        _listParam.Add(New Datos.DParametro("@DescripcionCategoria", _Descripcion))
        _listParam.Add(New Datos.DParametro("@Estado", _estado))
        _listParam.Add(New Datos.DParametro("@Imagen", _Imagen))
        _listParam.Add(New Datos.DParametro("@VisibleApp", _VisibleApp))
        _listParam.Add(New Datos.DParametro("@EmpresaId", _Empresa))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))


        _Tabla = D_ProcedimientoConParam("MAM_Categorias", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prCategoriaModificar(ByRef _numi As String, _NombreCategoria As String,
        _Descripcion As String, _estado As Integer, _Imagen As String, _VisibleApp As Integer, _Empresa As Integer) As Boolean
        Dim _resultado As Boolean

        '     INSERT INTO Categorias  VALUES(@Id,@NombreCategoria ,@DescripcionCategoria ,@Estado ,
        '@Imagen ,@VisibleApp ,@newFecha,@newHora,@usuario )

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@NombreCategoria", _NombreCategoria))
        _listParam.Add(New Datos.DParametro("@DescripcionCategoria", _Descripcion))
        _listParam.Add(New Datos.DParametro("@Estado", _estado))
        _listParam.Add(New Datos.DParametro("@Imagen", _Imagen))
        _listParam.Add(New Datos.DParametro("@VisibleApp", _VisibleApp))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@EmpresaId", _Empresa))

        _Tabla = D_ProcedimientoConParam("MAM_Categorias", _listParam)

        If _Tabla.Rows.Count > 0 Then

            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

#End Region
#Region "Usuarios TecBrinc"
    Public Shared Function L_prUsuarioGeneral() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Usuarios", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prListaRolesUsuarios() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Usuarios", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prListaEmpresasUsuarios() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Usuarios", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prListaCategorias() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Productos", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prListarTiposMovimientos() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prLeerConfiguracion() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 6))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Movimientos", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prListarDepositos() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Almacenes", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prListaCategoriasPrecios() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prListarZonas() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Clientes", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prUsuarioBorrar(_numi As String, ByRef _mensaje As String) As Boolean

        Dim _resultado As Boolean

        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", -1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Usuarios", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _resultado = True
            'L_prTipoCambioGrabarHistorial(_numi, _fecha, _dolar, _ufv, "TIPO DE CAMBIO", 3)
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prUsuarioModificar(_numi As String, _RolId As Integer, _NombreUsuario As String,
                                                _Contrasena As String, _estado As Integer,
 _sucursalId As Integer, _IdEmpresa As Integer, IdPersonal As Integer) As Boolean
        Dim _resultado As Boolean

        'INSERT INTO Usuarios  VALUES(@RolId ,@NombreUsuario ,@Contrasena ,@Estado 
        ',@SucursalId ,@newFecha,@newHora,@usuario,@IdEmpresa  )

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@RolId", _RolId))
        _listParam.Add(New Datos.DParametro("@NombreUsuario", _NombreUsuario))
        _listParam.Add(New Datos.DParametro("@Contrasena", _Contrasena))
        _listParam.Add(New Datos.DParametro("@Estado", _estado))
        _listParam.Add(New Datos.DParametro("@SucursalId", _sucursalId))
        _listParam.Add(New Datos.DParametro("@IdEmpresa", _IdEmpresa))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@IdPersonal", IdPersonal))

        _Tabla = D_ProcedimientoConParam("MAM_Usuarios", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prUsuarioInsertar(ByRef _numi As String, _RolId As Integer, _NombreUsuario As String,
                                                _Contrasena As String, _estado As Integer,
 _sucursalId As Integer, _IdEmpresa As Integer, IdPersonal As Integer) As Boolean
        Dim _resultado As Boolean

        'INSERT INTO Usuarios  VALUES(@RolId ,@NombreUsuario ,@Contrasena ,@Estado 
        ',@SucursalId ,@newFecha,@newHora,@usuario,@IdEmpresa  )

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@RolId", _RolId))
        _listParam.Add(New Datos.DParametro("@NombreUsuario", _NombreUsuario))
        _listParam.Add(New Datos.DParametro("@Contrasena", _Contrasena))
        _listParam.Add(New Datos.DParametro("@Estado", _estado))
        _listParam.Add(New Datos.DParametro("@SucursalId", _sucursalId))
        _listParam.Add(New Datos.DParametro("@IdEmpresa", _IdEmpresa))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@IdPersonal", IdPersonal))

        _Tabla = D_ProcedimientoConParam("MAM_Usuarios", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region
#Region "TecSucursales"
    Public Shared Function L_prSucursalInsertar(ByRef _numi As String, DepositoId As Integer,
           NombreAlmacen As String, Direccion As String, Descripcion As String, telefono As String,
                                                estado As Integer) As Boolean
        Dim _resultado As Boolean

        '   @Id,@DepositoId ,@NombreAlmacen ,@Direccion ,@Descripcion ,
        '@telefono ,@latitud ,@longitud ,@estado ,'',@newFecha,@newHora,@usuario 

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@DepositoId", DepositoId))
        _listParam.Add(New Datos.DParametro("@NombreAlmacen", NombreAlmacen))
        _listParam.Add(New Datos.DParametro("@Direccion", Direccion))
        _listParam.Add(New Datos.DParametro("@estado", estado))
        _listParam.Add(New Datos.DParametro("@Descripcion", Descripcion))
        _listParam.Add(New Datos.DParametro("@telefono", telefono))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))


        _Tabla = D_ProcedimientoConParam("MAM_Almacenes", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prSucursalActualizar(ByRef _numi As String, DepositoId As Integer,
           NombreAlmacen As String, Direccion As String, Descripcion As String, telefono As String, estado As Integer) As Boolean
        Dim _resultado As Boolean

        '   @Id,@DepositoId ,@NombreAlmacen ,@Direccion ,@Descripcion ,
        '@telefono ,@latitud ,@longitud ,@estado ,'',@newFecha,@newHora,@usuario 

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@DepositoId", DepositoId))
        _listParam.Add(New Datos.DParametro("@NombreAlmacen", NombreAlmacen))
        _listParam.Add(New Datos.DParametro("@Direccion", Direccion))
        _listParam.Add(New Datos.DParametro("@estado", estado))
        _listParam.Add(New Datos.DParametro("@Descripcion", Descripcion))
        _listParam.Add(New Datos.DParametro("@telefono", telefono))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))


        _Tabla = D_ProcedimientoConParam("MAM_Almacenes", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prSucursalBorrar(_numi As String, ByRef _mensaje As String) As Boolean

        Dim _resultado As Boolean

        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", -1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Almacenes", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _resultado = True
            'L_prTipoCambioGrabarHistorial(_numi, _fecha, _dolar, _ufv, "TIPO DE CAMBIO", 3)
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region
#Region "ROLES TecBrinc"

    Public Shared Function L_prRolGeneral() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Roles", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prRolDetalleGeneral(_numi As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Roles", _listParam)

        Return _Tabla
    End Function


    Public Shared Function L_prRolGrabar(ByRef _numi As String, _rol As String, _detalle As DataTable) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@NombreRol", _rol))
        _listParam.Add(New Datos.DParametro("@RolDetalleType", "", _detalle))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Roles", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True
            'L_prTipoCambioGrabarHistorial(_numi, _fecha, _dolar, _ufv, "TIPO DE CAMBIO", 1)
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prRolModificar(_numi As String, _rol As String, _detalle As DataTable) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@NombreRol", _rol))
        _listParam.Add(New Datos.DParametro("@RolDetalleType", "", _detalle))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))


        _Tabla = D_ProcedimientoConParam("MAM_Roles", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _resultado = True
            'L_prTipoCambioGrabarHistorial(_numi, _fecha, _dolar, _ufv, "TIPO DE CAMBIO", 2)
        Else
            _resultado = False
        End If

        Return _resultado
    End Function



    Public Shared Function L_prRolBorrar(_numi As String, ByRef _mensaje As String) As Boolean

        Dim _resultado As Boolean

        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
        _listParam.Add(New Datos.DParametro("@Id", _numi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_Roles", _listParam)

        If _Tabla.Rows.Count > 0 Then
                _resultado = True
                'L_prTipoCambioGrabarHistorial(_numi, _fecha, _dolar, _ufv, "TIPO DE CAMBIO", 3)
            Else
                _resultado = False
            End If

            Return _resultado
    End Function



#End Region

#Region "Cuentas por Pagar"


    Public Shared Function L_prListarPagosPendientes() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_CreditosCompras", _listParam)

        Return _Tabla
    End Function
#End Region

End Class
