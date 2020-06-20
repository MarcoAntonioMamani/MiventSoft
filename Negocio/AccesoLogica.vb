﻿
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

#Region "VALIDAR ELIMINACION"
    Public Shared Function L_fnbValidarEliminacion(_numi As String, _tablaOri As String, _campoOri As String, ByRef _respuesta As String) As Boolean
        Dim _Tabla As DataTable
        Dim _Where, _campos As String
        _Where = "bbtori='" + _tablaOri + "' and bbtran=1"
        _campos = "bbnumi,bbtran,bbtori,bbcori,bbtdes,bbcdes,bbprog"
        _Tabla = D_Datos_Tabla(_campos, "TB002", _Where)
        _respuesta = "no se puede eliminar el registro: ".ToUpper + _numi + " por que esta siendo usado en los siguientes programas: ".ToUpper + vbCrLf

        Dim result As Boolean = True
        For Each fila As DataRow In _Tabla.Rows
            If L_fnbExisteRegEnTabla(_numi, fila.Item("bbtdes").ToString, fila.Item("bbcdes").ToString) = True Then
                _respuesta = _respuesta + fila.Item("bbprog").ToString + vbCrLf
                result = False
            End If
        Next
        Return result
    End Function

    Private Shared Function L_fnbExisteRegEnTabla(_numiOri As String, _tablaDest As String, _campoDest As String) As Boolean
        Dim _Tabla As DataTable
        Dim _Where, _campos As String
        _Where = _campoDest + "=" + _numiOri
        _campos = _campoDest
        _Tabla = D_Datos_Tabla(_campos, _tablaDest, _Where)
        If _Tabla.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
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
        _Tabla = D_Datos_Tabla("Id,RolId,SucursalId", "Usuarios", "NombreUsuario = '" + _Nom + "' AND Contrasena = '" + _Pass + "'")
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
    Public Shared Function L_fnGeneralZona() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@zauact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TZ001", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarPuntosPorZona(_zanumi As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@Id", _zanumi))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_Zonas", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnGrabarZona(_zanumi As String, _zaciudad As Integer, _zaprovincia As Integer, _zazona As Integer, _zacolor As String, point As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '@zanumi,@zacity ,@zaprovi ,@zazona ,@zacolor   ,@newFecha,@newHora,@zauact
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@zanumi", _zanumi))
        _listParam.Add(New Datos.DParametro("@zacity", _zaciudad))
        _listParam.Add(New Datos.DParametro("@zaprovi", _zaprovincia))
        _listParam.Add(New Datos.DParametro("@zazona", _zazona))
        _listParam.Add(New Datos.DParametro("@zacolor", _zacolor))
        _listParam.Add(New Datos.DParametro("@zauact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@Tz0012", "", point))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TZ001", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _zanumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnModificaZona(_zanumi As String, _zaciudad As Integer, _zaprovincia As Integer, _zazona As Integer, _zacolor As String, point As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@zanumi", _zanumi))
        _listParam.Add(New Datos.DParametro("@zacity", _zaciudad))
        _listParam.Add(New Datos.DParametro("@zaprovi", _zaprovincia))
        _listParam.Add(New Datos.DParametro("@zazona", _zazona))
        _listParam.Add(New Datos.DParametro("@zacolor", _zacolor))
        _listParam.Add(New Datos.DParametro("@zauact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@Tz0012", "", point))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TZ001", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _zanumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnEliminarZona(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        If L_fnbValidarEliminacion(numi, "TZ001", "zanumi", mensaje) = True Then
            Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@zanumi", numi))
            _listParam.Add(New Datos.DParametro("@zauact", L_Usuario))

            _Tabla = D_ProcedimientoConParam("sp_Mam_TZ001", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If
        Else
            _resultado = False
        End If
        Return _resultado
    End Function
#End Region

#Region "TV001 Ventas"
    Public Shared Function L_fnGeneralVenta() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnDetalleVenta(_numi As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@tanumi", _numi))
        _listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnVentaNotaDeVenta(_numi As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 10))
        _listParam.Add(New Datos.DParametro("@tanumi", _numi))
        _listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnVentaFactura(_numi As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 13))
        _listParam.Add(New Datos.DParametro("@tanumi", _numi))
        _listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnListarProductos(_almacen As String, _cliente As String, _detalle As DataTable) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _listParam.Add(New Datos.DParametro("@cliente", _cliente))
        _listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TV0011", "", _detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function
    'funcion para llenar los datos de la grilla en la venta
    Public Shared Function L_fnListarProductosC(_yfcBarra As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 11))
        _listParam.Add(New Datos.DParametro("@yfcbarra", _yfcBarra))
        '_listParam.Add(New Datos.DParametro("@cliente", _cliente))
        '_listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
        '_listParam.Add(New Datos.DParametro("@TV0011", "", _detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TY005", _listParam)



        Return _Tabla

    End Function

    Public Shared Function L_fnListarProductosSinLote(_almacen As String, _cliente As String, _detalle As DataTable) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 9))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _listParam.Add(New Datos.DParametro("@cliente", _cliente))
        _listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TV0011", "", _detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarClientes() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 6))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarProforma() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 11))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarProductoProforma(_panumi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 12))
        _listParam.Add(New Datos.DParametro("@panumi", _panumi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarEmpleado() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

        Return _Tabla
    End Function


    Public Shared Function L_fnGrabarVenta(ByRef _tanumi As String, _taidCorelativo As String, _tafdoc As String, _taven As Integer, _tatven As Integer, _tafvcr As String, _taclpr As Integer,
                                           _tamon As Integer, _taobs As String,
                                           _tadesc As Double, _taice As Double,
                                           _tatotal As Double, detalle As DataTable, _almacen As Integer, _taprforma As Integer) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '    @tanumi ,@taalm,@tafdoc ,@taven  ,@tatven,
        '@tafvcr ,@taclpr,@tamon ,@taest  ,@taobs ,@tadesc ,@newFecha,@newHora,@tauact,@taproforma
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@tanumi", _tanumi))
        _listParam.Add(New Datos.DParametro("@taproforma", _taprforma))
        _listParam.Add(New Datos.DParametro("@taidCore", _taidCorelativo))
        _listParam.Add(New Datos.DParametro("@taalm", _almacen))
        _listParam.Add(New Datos.DParametro("@tafdoc", _tafdoc))
        _listParam.Add(New Datos.DParametro("@taven", _taven))
        _listParam.Add(New Datos.DParametro("@tatven", _tatven))
        _listParam.Add(New Datos.DParametro("@tafvcr", _tafvcr))
        _listParam.Add(New Datos.DParametro("@taclpr", _taclpr))
        _listParam.Add(New Datos.DParametro("@tamon", _tamon))
        _listParam.Add(New Datos.DParametro("@taest", 1))
        _listParam.Add(New Datos.DParametro("@taobs", _taobs))
        _listParam.Add(New Datos.DParametro("@tadesc", _tadesc))
        _listParam.Add(New Datos.DParametro("@taice", _taice))
        _listParam.Add(New Datos.DParametro("@tatotal", _tatotal))
        _listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TV0011", "", detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _tanumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnModificarVenta(_tanumi As String, _tafdoc As String, _taven As Integer, _tatven As Integer, _tafvcr As String, _taclpr As Integer,
                                           _tamon As Integer, _taobs As String,
                                           _tadesc As Double, _taice As Double, _tatotal As Double, detalle As DataTable, _almacen As Integer, _taprforma As Integer) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@tanumi", _tanumi))
        _listParam.Add(New Datos.DParametro("@taalm", _almacen))
        _listParam.Add(New Datos.DParametro("@taproforma", _taprforma))
        _listParam.Add(New Datos.DParametro("@tafdoc", _tafdoc))
        _listParam.Add(New Datos.DParametro("@taven", _taven))
        _listParam.Add(New Datos.DParametro("@tatven", _tatven))
        _listParam.Add(New Datos.DParametro("@tafvcr", _tafvcr))
        _listParam.Add(New Datos.DParametro("@taclpr", _taclpr))
        _listParam.Add(New Datos.DParametro("@tamon", _tamon))
        _listParam.Add(New Datos.DParametro("@taest", 1))
        _listParam.Add(New Datos.DParametro("@taobs", _taobs))
        _listParam.Add(New Datos.DParametro("@tadesc", _tadesc))
        _listParam.Add(New Datos.DParametro("@taice", _taice))
        _listParam.Add(New Datos.DParametro("@tatotal", _tatotal))
        _listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TV0011", "", detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _tanumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnEliminarVenta(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        If L_fnbValidarEliminacion(numi, "TV001", "tanumi", mensaje) = True Then
            Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@tanumi", numi))
            _listParam.Add(New Datos.DParametro("@tauact", L_Usuario))
            _Tabla = D_ProcedimientoConParam("sp_Mam_TV001", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If
        Else
            _resultado = False
        End If
        Return _resultado
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

#Region "TA002 Deposito"
    Public Shared Function L_fnEliminarDeposito(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        If L_fnbValidarEliminacion(numi, "TA002", "abnumi", mensaje) = True Then
            Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@abnumi", numi))
            _listParam.Add(New Datos.DParametro("@abuact", L_Usuario))

            _Tabla = D_ProcedimientoConParam("sp_Mam_TA002", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If
        Else
            _resultado = False
        End If
        Return _resultado
    End Function



    Public Shared Function L_fnGrabarDeposito(ByRef _abnumi As String, _abdesc As String, _abdir As String, _abtelf As String, _ablat As Double, _ablong As Double, _abimg As String, _abest As Integer) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        '@abnumi ,@abdesc ,@abdir ,@abtelf ,@ablat ,@ablong,@abimg ,@abest ,@newFecha,@newHora,@abuact
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@abnumi", _abnumi))
        _listParam.Add(New Datos.DParametro("@abdesc", _abdesc))
        _listParam.Add(New Datos.DParametro("@abdir", _abdir))
        _listParam.Add(New Datos.DParametro("@abtelf", _abtelf))
        _listParam.Add(New Datos.DParametro("@ablat", _ablat))
        _listParam.Add(New Datos.DParametro("@ablong", _ablong))
        _listParam.Add(New Datos.DParametro("@abimg", _abimg))
        _listParam.Add(New Datos.DParametro("@abest", _abest))


        _listParam.Add(New Datos.DParametro("@abuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TA002", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _abnumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnModificarDepositos(ByRef _abnumi As String, _abdesc As String, _abdir As String, _abtelf As String, _ablat As Double, _ablong As Double, _abimg As String, _abest As Integer) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@abnumi", _abnumi))
        _listParam.Add(New Datos.DParametro("@abdesc", _abdesc))
        _listParam.Add(New Datos.DParametro("@abdir", _abdir))
        _listParam.Add(New Datos.DParametro("@abtelf", _abtelf))
        _listParam.Add(New Datos.DParametro("@ablat", _ablat))
        _listParam.Add(New Datos.DParametro("@ablong", _ablong))
        _listParam.Add(New Datos.DParametro("@abimg", _abimg))
        _listParam.Add(New Datos.DParametro("@abest", _abest))

        _listParam.Add(New Datos.DParametro("@abuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TA002", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _abnumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnGeneralDepositos() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@abuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TA002", _listParam)

        Return _Tabla
    End Function



#End Region

#Region "TA001 Almacen"
    Public Shared Function L_fnEliminarAlmacen(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        If L_fnbValidarEliminacion(numi, "TA001", "abnumi", mensaje) = True Then
            Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@aanumi", numi))
            _listParam.Add(New Datos.DParametro("@aauact", L_Usuario))

            _Tabla = D_ProcedimientoConParam("sp_Mam_TA001", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If
        Else
            _resultado = False
        End If
        Return _resultado
    End Function



    Public Shared Function L_fnGrabarAlmacen(ByRef _abnumi As String, _aata2dep As Integer, _abdesc As String, _abdir As String, _abtelf As String, _ablat As Double, _ablong As Double, _abimg As String) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        '@aanumi ,@aata2dep,@aadesc ,@aadir ,@aatelf ,@aalat ,@aalong,@aaimg ,@newFecha,@newHora,@aauact

        'a.aanumi ,a.aabdes ,a.aadir ,a.aatel ,a.aalat ,a.aalong ,a.aaimg,aata2dep ,a.aafact ,a.aahact ,a.aauact
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@aanumi", _abnumi))
        _listParam.Add(New Datos.DParametro("@aata2dep", _aata2dep))
        _listParam.Add(New Datos.DParametro("@aadesc", _abdesc))
        _listParam.Add(New Datos.DParametro("@aadir", _abdir))
        _listParam.Add(New Datos.DParametro("@aatelf", _abtelf))
        _listParam.Add(New Datos.DParametro("@aalat", _ablat))
        _listParam.Add(New Datos.DParametro("@aalong", _ablong))
        _listParam.Add(New Datos.DParametro("@aaimg", _abimg))



        _listParam.Add(New Datos.DParametro("@aauact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TA001", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _abnumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnModificarAlmacen(ByRef _abnumi As String, _aata2dep As Integer, _abdesc As String, _abdir As String, _abtelf As String, _ablat As Double, _ablong As Double, _abimg As String) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@aanumi", _abnumi))
        _listParam.Add(New Datos.DParametro("@aata2dep", _aata2dep))
        _listParam.Add(New Datos.DParametro("@aadesc", _abdesc))
        _listParam.Add(New Datos.DParametro("@aadir", _abdir))
        _listParam.Add(New Datos.DParametro("@aatelf", _abtelf))
        _listParam.Add(New Datos.DParametro("@aalat", _ablat))
        _listParam.Add(New Datos.DParametro("@aalong", _ablong))
        _listParam.Add(New Datos.DParametro("@aaimg", _abimg))

        _listParam.Add(New Datos.DParametro("@aauact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TA001", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _abnumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnGeneralAlmacen() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@aauact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TA001", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarDeposito() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@aauact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TA001", _listParam)

        Return _Tabla
    End Function


#End Region

#Region "TS002 Dosificacion"

    Public Shared Function L_fnEliminarDosificacion(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        If L_fnbValidarEliminacion(numi, "TS002", "sbnumi", mensaje) = True Then
            Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@numi", numi))
            _listParam.Add(New Datos.DParametro("@uact", L_Usuario))

            _Tabla = D_ProcedimientoConParam("sp_go_TS002", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If
        Else
            _resultado = False
        End If
        Return _resultado
    End Function



    Public Shared Function L_fnGrabarDosificacion(ByRef numi As String, cia As Integer, alm As String, sfc As String,
                                                  autoriz As String, nfac As Double, key As String, fdel As String,
                                                  fal As String, nota As String, nota2 As String, est As String) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@numi", numi))
        _listParam.Add(New Datos.DParametro("@cia", cia))
        _listParam.Add(New Datos.DParametro("@alm", alm))
        _listParam.Add(New Datos.DParametro("@sfc", sfc))
        _listParam.Add(New Datos.DParametro("@autoriz", autoriz))
        _listParam.Add(New Datos.DParametro("@nfac", nfac))
        _listParam.Add(New Datos.DParametro("@key", key))
        _listParam.Add(New Datos.DParametro("@fdel", fdel))
        _listParam.Add(New Datos.DParametro("@fal", fal))
        _listParam.Add(New Datos.DParametro("@nota", nota))
        _listParam.Add(New Datos.DParametro("@nota2", nota2))
        _listParam.Add(New Datos.DParametro("@est", est))

        _listParam.Add(New Datos.DParametro("@uact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_go_TS002", _listParam)

        If _Tabla.Rows.Count > 0 Then
            numi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnModificarDosificacion(ByRef numi As String, cia As Integer, alm As String, sfc As String,
                                                     autoriz As String, nfac As Double, key As String, fdel As String,
                                                     fal As String, nota As String, nota2 As String, est As String) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@numi", numi))
        _listParam.Add(New Datos.DParametro("@cia", cia))
        _listParam.Add(New Datos.DParametro("@alm", alm))
        _listParam.Add(New Datos.DParametro("@sfc", sfc))
        _listParam.Add(New Datos.DParametro("@autoriz", autoriz))
        _listParam.Add(New Datos.DParametro("@nfac", nfac))
        _listParam.Add(New Datos.DParametro("@key", key))
        _listParam.Add(New Datos.DParametro("@fdel", fdel))
        _listParam.Add(New Datos.DParametro("@fal", fal))
        _listParam.Add(New Datos.DParametro("@nota", nota))
        _listParam.Add(New Datos.DParametro("@nota2", nota2))
        _listParam.Add(New Datos.DParametro("@est", est))

        _listParam.Add(New Datos.DParametro("@uact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_go_TS002", _listParam)

        If _Tabla.Rows.Count > 0 Then
            numi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnGeneralDosificacion() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@uact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_go_TS002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarCompaniaDosificacion() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@uact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_go_TS002", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnListarAlmacenDosificacion() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@uact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_go_TS002", _listParam)

        Return _Tabla
    End Function

#End Region

#Region "Facturar"

    Public Shared Sub L_Grabar_Factura(_Numi As String, _Fecha As String, _Nfac As String, _NAutoriz As String, _Est As String,
                                       _NitCli As String, _CodCli As String, _DesCli1 As String, _DesCli2 As String,
                                       _A As String, _B As String, _C As String, _D As String, _E As String, _F As String,
                                       _G As String, _H As String, _CodCon As String, _FecLim As String,
                                       _Imgqr As String, _Alm As String, _Numi2 As String)
        Dim Sql As String
        Try
            Sql = "" + _Numi + ", " _
                + "'" + _Fecha + "', " _
                + "" + _Nfac + ", " _
                + "" + _NAutoriz + ", " _
                + "" + _Est + ", " _
                + "'" + _NitCli + "', " _
                + "" + _CodCli + ", " _
                + "'" + _DesCli1 + "', " _
                + "'" + _DesCli2 + "', " _
                + "" + _A + ", " _
                + "" + _B + ", " _
                + "" + _C + ", " _
                + "" + _D + ", " _
                + "" + _E + ", " _
                + "" + _F + ", " _
                + "" + _G + ", " _
                + "" + _H + ", " _
                + "'" + _CodCon + "', " _
                + "'" + _FecLim + "', " _
                + "" + _Imgqr + ", " _
                + "" + _Alm + ", " _
                + "" + _Numi2 + ""

            D_Insertar_Datos("TFV001", Sql)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Shared Sub L_Modificar_Factura(Where As String, Optional _Fecha As String = "",
                                          Optional _Nfact As String = "", Optional _NAutoriz As String = "",
                                          Optional _Est As String = "", Optional _NitCli As String = "",
                                          Optional _CodCli As String = "", Optional _DesCli1 As String = "",
                                          Optional _DesCli2 As String = "", Optional _A As String = "",
                                          Optional _B As String = "", Optional _C As String = "",
                                          Optional _D As String = "", Optional _E As String = "",
                                          Optional _F As String = "", Optional _G As String = "",
                                          Optional _H As String = "", Optional _CodCon As String = "",
                                          Optional _FecLim As String = "", Optional _Imgqr As String = "",
                                          Optional _Alm As String = "", Optional _Numi2 As String = "")
        Dim Sql As String
        Try
            Sql = IIf(_Fecha.Equals(""), "", "fvafec = '" + _Fecha + "', ") + _
              IIf(_Nfact.Equals(""), "", "fvanfac = " + _Nfact + ", ") + _
              IIf(_NAutoriz.Equals(""), "", "fvaautoriz = " + _NAutoriz + ", ") + _
              IIf(_Est.Equals(""), "", "fvaest = " + _Est) + _
              IIf(_NitCli.Equals(""), "", "fvanitcli = '" + _NitCli + "', ") + _
              IIf(_CodCli.Equals(""), "", "fvacodcli = " + _CodCli + ", ") + _
              IIf(_DesCli1.Equals(""), "", "fvadescli1 = '" + _DesCli1 + "', ") + _
              IIf(_DesCli2.Equals(""), "", "fvadescli2 = '" + _DesCli2 + "', ") + _
              IIf(_A.Equals(""), "", "fvastot = " + _A + ", ") + _
              IIf(_B.Equals(""), "", "fvaimpsi = " + _B + ", ") + _
              IIf(_C.Equals(""), "", "fvaimpeo = " + _C + ", ") + _
              IIf(_D.Equals(""), "", "fvaimptc = " + _D + ", ") + _
              IIf(_E.Equals(""), "", "fvasubtotal = " + _E + ", ") + _
              IIf(_F.Equals(""), "", "fvadesc = " + _F + ", ") + _
              IIf(_G.Equals(""), "", "fvatotal = " + _G + ", ") + _
              IIf(_H.Equals(""), "", "fvadebfis = " + _H + ", ") + _
              IIf(_CodCon.Equals(""), "", "fvaccont = '" + _CodCon + "', ") + _
              IIf(_FecLim.Equals(""), "", "fvaflim = '" + _FecLim + "', ") + _
              IIf(_Imgqr.Equals(""), "", "fvaimgqr = '" + _Imgqr + "', ") + _
              IIf(_Alm.Equals(""), "", "fvaalm = " + _Alm + ", ") + _
              IIf(_Numi2.Equals(""), "", "fvanumi2 = " + _Numi2 + ", ")
            Sql = Sql.Trim
            If (Sql.Substring(Sql.Length - 1, 1).Equals(",")) Then
                Sql = Sql.Substring(0, Sql.Length - 1)
            End If

            D_Modificar_Datos("TFV001", Sql, Where)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Public Shared Sub L_Grabar_Factura_Detalle(_Numi As String, _CodProd As String, _DescProd As String, _Cant As String, _Precio As String, _Numi2 As String)
        Dim Sql As String
        Try
            Sql = _Numi + ", '" + _CodProd + "', '" + _DescProd + "', " + _Cant + ", " + _Precio + ", " + _Numi2

            D_Insertar_Datos("TFV0011", Sql)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Shared Function L_Reporte_Factura(_Numi As String, _Numi2 As String) As DataSet
        Dim _Tabla As DataTable
        Dim _Ds As New DataSet
        Dim _Where As String
        _Where = " fvanumi = " + _Numi + " and fvanumi2 = " + _Numi2

        _Tabla = D_Datos_Tabla("*", "VR_GO_Factura", _Where)
        _Ds.Tables.Add(_Tabla)
        Return _Ds
    End Function

    Public Shared Function L_Reporte_Factura_Cia(_Cia As String) As DataSet
        Dim _Tabla As DataTable
        Dim _Ds As New DataSet
        Dim _Where As String
        _Where = " scnumi = " + _Cia

        _Tabla = D_Datos_Tabla("*", "TS003", _Where)
        _Ds.Tables.Add(_Tabla)
        Return _Ds
    End Function

    Public Shared Function L_ObtenerRutaImpresora(_NroImp As String, Optional tImp As String = "") As DataSet
        Dim _Tabla As DataTable
        Dim _Ds As New DataSet
        Dim _Where As String
        If (Not _NroImp.Trim.Equals("")) Then
            _Where = " cbnumi = " + _NroImp + " and cbest = 1 order by cbnumi"
        Else
            _Where = " cbtimp = " + tImp + " and cbest = 1 order by cbnumi"
        End If
        _Tabla = D_Datos_Tabla("*", "TC002", _Where)
        _Ds.Tables.Add(_Tabla)
        Return _Ds
    End Function

    Public Shared Function L_fnGetIVA() As DataTable
        Dim _Tabla As DataTable
        Dim _Where As String
        _Where = "1 = 1"
        _Tabla = D_Datos_Tabla("scdebfis", "TS003", _Where)
        Return _Tabla
    End Function

    Public Shared Function L_fnGetICE() As DataTable
        Dim _Tabla As DataTable
        Dim _Where As String
        _Where = "1 = 1"
        _Tabla = D_Datos_Tabla("scice", "TS003", _Where)
        Return _Tabla
    End Function

    Public Shared Sub L_Grabar_Nit(_Nit As String, _Nom1 As String, _Nom2 As String)
        Dim _Err As Boolean
        Dim _Nom01, _Nom02 As String
        Dim Sql As String
        _Nom01 = ""
        _Nom02 = ""
        L_Validar_Nit(_Nit, _Nom01, _Nom02)

        If _Nom01 = "" Then
            Sql = _Nit + ", '" + _Nom1 + "', '" + _Nom2 + "'"
            _Err = D_Insertar_Datos("TS001", Sql)
        Else
            If (_Nom1 <> _Nom01) Or (_Nom2 <> _Nom02) Then
                Sql = "sanom1 = '" + _Nom1 + "' " + _
                      IIf(_Nom02.ToString.Trim.Equals(""), "", ", sanom2 = '" + _Nom2 + "', ")
                _Err = D_Modificar_Datos("TS001", Sql, "sanit = " + _Nit)
            End If
        End If

    End Sub

    Public Shared Sub L_Validar_Nit(_Nit As String, ByRef _Nom1 As String, ByRef _Nom2 As String)
        Dim _Tabla As DataTable

        _Tabla = D_Datos_Tabla("*", "TS001", "sanit = '" + _Nit + "'")

        If _Tabla.Rows.Count > 0 Then
            _Nom1 = _Tabla.Rows(0).Item(2)
            _Nom2 = IIf(_Tabla.Rows(0).Item(3).ToString.Trim.Equals(""), "", _Tabla.Rows(0).Item(3))
        End If
    End Sub

    Public Shared Function L_Eliminar_Nit(_Nit As String) As Boolean
        Dim res As Boolean = False
        Try
            res = D_Eliminar_Datos("TS001", "sanit = " + _Nit)
        Catch ex As Exception
            res = False
        End Try
        Return res
    End Function

    Public Shared Function L_Dosificacion(_cia As String, _alm As String, _fecha As String) As DataSet
        Dim _Tabla As DataTable
        Dim _Ds As New DataSet
        Dim _Where As String
        _fecha = Now.Date.ToString("yyyy/MM/dd")
        _Where = "sbcia = " + _cia + " AND sbalm = " + _alm + " AND sbfdel <= '" + _fecha + "' AND sbfal >= '" + _fecha + "' AND sbest = 1"

        _Tabla = D_Datos_Tabla("*", "TS002", _Where)
        _Ds.Tables.Add(_Tabla)
        Return _Ds
    End Function

    Public Shared Sub L_Actualiza_Dosificacion(_Numi As String, _NumFac As String, _Numi2 As String)
        Dim _Err As Boolean
        Dim Sql, _where As String
        Sql = "sbnfac = " + _NumFac
        _where = "sbnumi = " + _Numi

        _Err = D_Modificar_Datos("TS002", Sql, _where)
    End Sub

    Public Shared Function L_fnObtenerMaxIdTabla(tabla As String, campo As String, where As String) As Long
        Dim Dt As DataTable = New DataTable
        Dt = D_Maximo(tabla, campo, where)

        If (Dt.Rows.Count > 0) Then
            If (Dt.Rows(0).Item(0).ToString.Equals("")) Then
                Return 0
            Else
                Return CLng(Dt.Rows(0).Item(0).ToString)
            End If
        Else
            Return 0
        End If
    End Function

    Public Shared Function L_fnObtenerTabla(tabla As String, campo As String, where As String) As DataTable
        Dim Dt As DataTable = D_Datos_Tabla(campo, tabla, where)
        Return Dt
    End Function

    Public Shared Function L_fnObtenerDatoTabla(tabla As String, campo As String, where As String) As String
        Dim Dt As DataTable = D_Datos_Tabla(campo, tabla, where)
        If (Dt.Rows.Count > 0) Then
            Return Dt.Rows(0).Item(campo).ToString
        Else
            Return ""
        End If
    End Function

    Public Shared Function L_fnEliminarDatos(Tabla As String, Where As String) As Boolean
        Return D_Eliminar_Datos(Tabla, Where)
    End Function

#End Region

#Region "Anular Factura"

    Public Shared Function L_Obtener_Facturas() As DataSet
        Dim _Tabla1 As New DataTable
        Dim _Tabla2 As New DataTable
        Dim _Ds As New DataSet
        Dim _Where As String
        _Where = " 1 = 1"
        'Cambiar la logica para visualizar las facturas esto en el programa de facturas
        _Tabla1 = D_Datos_Tabla("concat(fvanfac, '_', fvaautoriz) as Archivo, fvanumi as Codigo, fvanfac as [Nro Factura], " _
                                + "fvafec as Fecha, fvacodcli as [Cod Cliente], " _
                                + " fvadescli1 as [Nombre 1], fvadescli2 as [Nombre 2], fvanitcli as Nit, " _
                                + " fvastot as Subtotal, fvadesc as Descuento, fvatotal as Total, " _
                                + " fvaccont as [Cod Control], fvaflim as [Fec Limite], fvaest as Estado",
                                "TFV001", _Where)
        '_Tabla1.Columns(0).ColumnMapping = MappingType.Hidden
        _Ds.Tables.Add(_Tabla1)

        _Tabla2 = D_Datos_Tabla("concat(fvanfac, '_', fvaautoriz) as Archivo, fvbnumi as Codigo, fvbcprod as [Cod Producto], fvbdesprod as Descripcion, " _
                                + " fvbcant as Cantidad, fvbprecio as [Precio Unitario], (fvbcant * fvbprecio) as Precio",
                                "TFV001, TFV0011", "fvanumi = fvbnumi and fvanumi2 = fvbnumi2")
        _Ds.Tables.Add(_Tabla2)
        _Ds.Relations.Add("1", _Tabla1.Columns("Archivo"), _Tabla2.Columns("Archivo"), False)
        Return _Ds
    End Function

    Public Shared Function L_ObtenerDetalleFactura(_CodFact As String) As DataSet 'Modifcar para que solo Traiga los productos Con Stock
        Dim _Tabla As DataTable
        Dim _Ds As New DataSet
        Dim _Where As String
        _Where = "fvbnumi = " + _CodFact
        _Tabla = D_Datos_Tabla("fvbcprod as codP, fvbcant as can, '1' as sto", "TFV0011", _Where)
        _Ds.Tables.Add(_Tabla)
        Return _Ds
    End Function

#End Region

#Region "Libro de ventas"

    Public Shared Function L_fnObtenerLibroVenta(_CodAlm As String, _Mes As String, _Anho As String) As DataTable
        Dim _Tabla As DataTable
        Dim _Where As String = "fvaalm = " + _CodAlm + "and Month(fvafec) = " + _Mes + " and Year(fvafec) = " + _Anho + " ORDER BY fvanfac"
        _Tabla = D_Datos_Tabla("*",
                               "VR_GO_LibroVenta", _Where)
        Return _Tabla
    End Function

    Public Shared Function L_ObtenerAnhoFactura() As DataTable
        Dim _Tabla As DataTable
        Dim _Where As String = "1 = 1 ORDER BY year(fvafec)"
        _Tabla = D_Datos_Tabla("Distinct(year(fvafec)) AS anho",
                               "VR_GO_LibroVenta", _Where)
        Return _Tabla
    End Function

    Public Shared Function L_ObtenerSucursalesFactura() As DataTable
        Dim _Tabla As DataTable
        Dim _Where As String = "1 = 1 ORDER BY a.scneg"
        _Tabla = D_Datos_Tabla("a.scnumi, a.scneg, a.scnit",
                               "TS003 a", _Where)
        Return _Tabla
    End Function

#End Region

#Region "COBROS DE LAS VENTAS"
    Public Shared Function L_fnObtenerLasVentasACredito() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@tduact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnObtenerLosPagos(_numi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@tduact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@credito", _numi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnObtenerLasVentasCreditoPorCliente(_numi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 9))
        '_listParam.Add(New Datos.DParametro("@tduact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tenumi", _numi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnObtenerLasVentasCreditoPorVendedorFecha(_numi As Integer, _fecha As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 11))
        '_listParam.Add(New Datos.DParametro("@tduact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tenumi", _numi))
        _listParam.Add(New Datos.DParametro("@tefdoc", _fecha))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnGrabarPagos(_numi As String, dt As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
               _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@tduact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tdnumi", _numi))
        _listParam.Add(New Datos.DParametro("@TV00121", "", dt))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
    Public Shared Function L_fnEliminarPago(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        If L_fnbValidarEliminacion(numi, "TV00121", "tdnumi", mensaje) = True Then
            Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@tdnumi", numi))
            _listParam.Add(New Datos.DParametro("@tduact", L_Usuario))

            _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If
        Else
            _resultado = False
        End If
        Return _resultado
    End Function
#End Region

#Region "COBROS DE LAS VENTAS CON CHEQUE"
    Public Shared Function L_fnCobranzasGeneral() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnCobranzasObtenerLosPagos(_numi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tdnumi", _numi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnCobranzasObtenerLasVentasACredito() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 6))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnCobranzasDetalle(_numi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tenumi", _numi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnCobranzasReporte(_numi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 8))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tenumi", _numi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnEliminarCobranza(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        If L_fnbValidarEliminacion(numi, "TV0013", "tenumi", mensaje) = True Then
            Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@tenumi", numi))
            _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))

            _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If
        Else
            _resultado = False
        End If
        Return _resultado
    End Function
    '@tenumi ,@tefdoc,@tety4vend ,@teobs ,@newFecha ,@newHora ,@teuact 
    Public Shared Function L_fnGrabarCobranza(_tenumi As String, _tefdoc As String, _tety4vend As Integer, _teobs As String, detalle As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '   @canumi ,@caalm,@cafdoc ,@caty4prov  ,@catven,
        '@cafvcr,@camon ,@caest  ,@caobs ,@cadesc ,@newFecha,@newHora,@cauact
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@tenumi", _tenumi))
        _listParam.Add(New Datos.DParametro("@tefdoc", _tefdoc))
        _listParam.Add(New Datos.DParametro("@tety4vend", _tety4vend))
        _listParam.Add(New Datos.DParametro("@teobs", _teobs))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TV00121", "", detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _tenumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnGrabarCobranza2(_tenumi As String, _tefdoc As String, _tety4vend As Integer, _teobs As String, detalle As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '   @canumi ,@caalm,@cafdoc ,@caty4prov  ,@catven,
        '@cafvcr,@camon ,@caest  ,@caobs ,@cadesc ,@newFecha,@newHora,@cauact
        _listParam.Add(New Datos.DParametro("@tipo", 10))
        _listParam.Add(New Datos.DParametro("@tenumi", _tenumi))
        _listParam.Add(New Datos.DParametro("@tefdoc", _tefdoc))
        _listParam.Add(New Datos.DParametro("@tety4vend", _tety4vend))
        _listParam.Add(New Datos.DParametro("@teobs", _teobs))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TV00121", "", detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _tenumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnModificarCobranza(_tenumi As String, _tefdoc As String, _tety4vend As Integer, _teobs As String, detalle As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '   @canumi ,@caalm,@cafdoc ,@caty4prov  ,@catven,
        '@cafvcr,@camon ,@caest  ,@caobs ,@cadesc ,@newFecha,@newHora,@cauact
        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@tenumi", _tenumi))
        _listParam.Add(New Datos.DParametro("@tefdoc", _tefdoc))
        _listParam.Add(New Datos.DParametro("@tety4vend", _tety4vend))
        _listParam.Add(New Datos.DParametro("@teobs", _teobs))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TV00121", "", detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TV00121Cheque", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _tenumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region

#Region "PAGOS DE LAS COMPRAS CON CHEQUE"
    Public Shared Function L_fnCobranzasGeneralCompra() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarPagosCompras(_credito As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@credito", _credito))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnCobranzasObtenerLosPagosCompra(_numi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tdnumi", _numi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnCobranzasObtenerLasVentasACreditoCompras() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 6))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnCobranzasDetalleCompras(_numi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tenumi", _numi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnCobranzasReporteCompras(_numi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 8))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tenumi", _numi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121Cheque", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnEliminarCobranzaCompras(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        If L_fnbValidarEliminacion(numi, "TC0013", "tenumi", mensaje) = True Then
            Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@tenumi", numi))
            _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))

            _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121Cheque", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If
        Else
            _resultado = False
        End If
        Return _resultado
    End Function
    '@tenumi ,@tefdoc,@tety4vend ,@teobs ,@newFecha ,@newHora ,@teuact 
    Public Shared Function L_fnGrabarCobranzaCompras(_tenumi As String, _tefdoc As String, _tety4vend As Integer, _teobs As String, detalle As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '   @canumi ,@caalm,@cafdoc ,@caty4prov  ,@catven,
        '@cafvcr,@camon ,@caest  ,@caobs ,@cadesc ,@newFecha,@newHora,@cauact
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@tenumi", _tenumi))
        _listParam.Add(New Datos.DParametro("@tefdoc", _tefdoc))
        _listParam.Add(New Datos.DParametro("@tety4vend", _tety4vend))
        _listParam.Add(New Datos.DParametro("@teobs", _teobs))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TC00121", "", detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121Cheque", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _tenumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_fnModificarCobranzaCompras(_tenumi As String, _tefdoc As String, _tety4vend As Integer, _teobs As String, detalle As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        '   @canumi ,@caalm,@cafdoc ,@caty4prov  ,@catven,
        '@cafvcr,@camon ,@caest  ,@caobs ,@cadesc ,@newFecha,@newHora,@cauact
        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@tenumi", _tenumi))
        _listParam.Add(New Datos.DParametro("@tefdoc", _tefdoc))
        _listParam.Add(New Datos.DParametro("@tety4vend", _tety4vend))
        _listParam.Add(New Datos.DParametro("@teobs", _teobs))
        _listParam.Add(New Datos.DParametro("@teuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TC00121", "", detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121Cheque", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _tenumi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
#End Region

#Region "COBROS DE LAS COMPRAS"
    Public Shared Function L_fnObtenerLasComprasACredito() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@tduact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnObtenerLosPagosComprasCreditos(_numi As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@tduact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@credito", _numi))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnGrabarPagosCompraCredito(_numi As String, dt As DataTable) As Boolean
        Dim _Tabla As DataTable
        Dim _resultado As Boolean
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@tduact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@tdnumi", _numi))
        _listParam.Add(New Datos.DParametro("@TC00121", "", dt))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121", _listParam)


        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
    Public Shared Function L_fnEliminarPagoCompraCredito(numi As String, ByRef mensaje As String) As Boolean
        Dim _resultado As Boolean
        If L_fnbValidarEliminacion(numi, "TC00121", "tdnumi", mensaje) = True Then
            Dim _Tabla As DataTable
            Dim _listParam As New List(Of Datos.DParametro)

            _listParam.Add(New Datos.DParametro("@tipo", -1))
            _listParam.Add(New Datos.DParametro("@tdnumi", numi))
            _listParam.Add(New Datos.DParametro("@tduact", L_Usuario))

            _Tabla = D_ProcedimientoConParam("sp_Mam_TC00121", _listParam)

            If _Tabla.Rows.Count > 0 Then
                _resultado = True
            Else
                _resultado = False
            End If
        Else
            _resultado = False
        End If
        Return _resultado
    End Function
#End Region

#Region "TI002 MOVIMIENTOS "
    Public Shared Function L_fnGeneralMovimiento() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function


    Public Shared Function L_fnDetalleMovimiento(_ibid As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@ibid", _ibid))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prMovimientoConcepto() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarSucursalesMovimiento() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TC001", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prMovimientoListarProductos(dt As DataTable, _deposito As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@almacen", _deposito))
        _listParam.Add(New Datos.DParametro("@TI0021", "", dt))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prMovimientoListarProductosConLote(_deposito As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 31))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@almacen", _deposito))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarLotesPorProductoMovimiento(_almacen As Integer, _codproducto As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 32))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _listParam.Add(New Datos.DParametro("@producto", _codproducto))
        _listParam.Add(New Datos.DParametro("ibuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_prMovimientoChoferABMDetalle(numi As String, Type As Integer, detalle As DataTable) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", Type))
        _listParam.Add(New Datos.DParametro("@ibid", numi))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@TI0021", "", detalle))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function


    Public Shared Function L_prMovimientoChoferGrabar(ByRef _ibid As String, _ibfdoc As String, _ibconcep As Integer, _ibobs As String, _almacen As Integer, _depositoDestino As Integer, _ibidOrigen As Integer) As Boolean
        Dim _resultado As Boolean
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@ibid", _ibid))
        _listParam.Add(New Datos.DParametro("@ibfdoc", _ibfdoc))
        _listParam.Add(New Datos.DParametro("@ibconcep", _ibconcep))
        _listParam.Add(New Datos.DParametro("@ibobs", _ibobs))
        _listParam.Add(New Datos.DParametro("@ibest", 1))
        _listParam.Add(New Datos.DParametro("@ibalm", _almacen))
        _listParam.Add(New Datos.DParametro("@ibdepdest", _depositoDestino))
        _listParam.Add(New Datos.DParametro("@ibiddc", 0))
        _listParam.Add(New Datos.DParametro("@ibidOrigen", _ibidOrigen))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)
        If _Tabla.Rows.Count > 0 Then
            _ibid = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function


    Public Shared Function L_prMovimientoModificar(ByRef _ibid As String, _ibfdoc As String, _ibconcep As Integer, _ibobs As String, _almacen As Integer) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@ibid", _ibid))
        _listParam.Add(New Datos.DParametro("@ibfdoc", _ibfdoc))
        _listParam.Add(New Datos.DParametro("@ibconcep", _ibconcep))
        _listParam.Add(New Datos.DParametro("@ibobs", _ibobs))
        _listParam.Add(New Datos.DParametro("@ibest", 1))
        _listParam.Add(New Datos.DParametro("@ibalm", _almacen))
        _listParam.Add(New Datos.DParametro("@ibiddc", 0))

        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _ibid = _Tabla.Rows(0).Item(0)
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function

    Public Shared Function L_prMovimientoEliminar(numi As String) As Boolean
        Dim _resultado As Boolean

        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", -1))
        _listParam.Add(New Datos.DParametro("@ibid", numi))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))

        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _resultado = True
        Else
            _resultado = False
        End If

        Return _resultado
    End Function
    Public Shared Function L_fnListarProductosKardex(_almacen As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 8))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnListarLotesProductos(codproducto As Integer, _almacen As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 28))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@producto", codproducto))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnObtenerSaldoProducto(_almacen As Integer, _codProducto As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 23))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _listParam.Add(New Datos.DParametro("@producto", _codProducto))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnObtenerHistorialProducto(_codProducto As Integer, FechaI As String, FechaF As String, _almacen As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 9))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@producto", _codProducto))
        _listParam.Add(New Datos.DParametro("@fechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@fechaF", FechaF))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnObtenerHistorialProductoporLote(_codProducto As Integer, FechaI As String, FechaF As String, _almacen As Integer, _Lote As String, _FechaVenc As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 30))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@producto", _codProducto))
        _listParam.Add(New Datos.DParametro("@fechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@fechaF", FechaF))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _listParam.Add(New Datos.DParametro("@lote", _Lote))
        _listParam.Add(New Datos.DParametro("@fechaVenc", _FechaVenc))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnObtenerKardexPorProducto(_codProducto As Integer, FechaI As String, FechaF As String, _almacen As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 25))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@producto", _codProducto))
        _listParam.Add(New Datos.DParametro("@fechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@fechaF", FechaF))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnObtenerProductoConMovimiento(FechaI As String, FechaF As String, _almacen As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 26))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@fechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@fechaF", FechaF))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnObtenerKardexGeneralProductos(FechaI As String, FechaF As String, _almacen As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 27))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@fechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@fechaF", FechaF))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnObtenerKardexGeneralProductosporLote(FechaI As String, FechaF As String, _almacen As Integer) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 33))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@fechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@fechaF", FechaF))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnObtenerHistorialProductoGeneral(_codProducto As Integer, FechaI As String, _almacen As Integer) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 20))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@producto", _codProducto))
        _listParam.Add(New Datos.DParametro("@fechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function



    Public Shared Function L_fnObtenerHistorialProductoGeneralPorLote(_codProducto As Integer, FechaI As String, _almacen As Integer, _Lote As String, FechaVenc As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 29))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@producto", _codProducto))
        _listParam.Add(New Datos.DParametro("@fechaI", FechaI))
        _listParam.Add(New Datos.DParametro("@almacen", _almacen))
        _listParam.Add(New Datos.DParametro("@lote", _Lote))
        _listParam.Add(New Datos.DParametro("@fechaVenc", FechaVenc))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnActualizarSaldo(_Almacen As Integer, _CodProducto As String, _Cantidad As Double) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 21))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _listParam.Add(New Datos.DParametro("@producto", _CodProducto))
        _listParam.Add(New Datos.DParametro("@almacen", _Almacen))
        _listParam.Add(New Datos.DParametro("@cantidad", _Cantidad))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_fnStockActual() As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 22))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
    End Function
    Public Shared Function L_fnStockActualLote() As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 34))
        _listParam.Add(New Datos.DParametro("@ibuact", L_Usuario))
        _Tabla = D_ProcedimientoConParam("sp_Mam_TI002", _listParam)

        Return _Tabla
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
 _sucursalId As Integer, _IdEmpresa As Integer) As Boolean
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
 _sucursalId As Integer, _IdEmpresa As Integer) As Boolean
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








End Class
