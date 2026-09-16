'' ============================================================================
'' Funciones nuevas para AccesoLogica.vb (pegar antes de "End Class", mismo
'' patron que el resto del archivo: List(Of Datos.DParametro) + D_ProcedimientoConParam).
'' Llaman al SP nuevo MAM_Auditoria (ver 05_CREATE_MAM_Auditoria.sql).
'' ============================================================================

#Region "Auditoria"

    '' ---------------- VENTAS ----------------
    Public Shared Function ListarEventosAuditoriaVentas(desde As String, hasta As String, usuarioAccion As String, accion As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@Desde", desde))
        _listParam.Add(New Datos.DParametro("@Hasta", hasta))
        _listParam.Add(New Datos.DParametro("@UsuarioAccion", usuarioAccion))
        _listParam.Add(New Datos.DParametro("@Accion", accion))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Auditoria", _listParam)
        Return _Tabla
    End Function

    Public Shared Function CompararCabeceraAuditoriaVentas(id As Integer, accion As String, fechaAccion As String, horaAccion As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@Id", id))
        _listParam.Add(New Datos.DParametro("@Accion", accion))
        _listParam.Add(New Datos.DParametro("@FechaAccion", fechaAccion))
        _listParam.Add(New Datos.DParametro("@HoraAccion", horaAccion))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Auditoria", _listParam)
        Return _Tabla
    End Function

    Public Shared Function CompararDetalleAuditoriaVentas(id As Integer, accion As String, fechaAccion As String, horaAccion As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@Id", id))
        _listParam.Add(New Datos.DParametro("@Accion", accion))
        _listParam.Add(New Datos.DParametro("@FechaAccion", fechaAccion))
        _listParam.Add(New Datos.DParametro("@HoraAccion", horaAccion))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Auditoria", _listParam)
        Return _Tabla
    End Function

    '' ---------------- COMPRAS ----------------
    Public Shared Function ListarEventosAuditoriaCompras(desde As String, hasta As String, usuarioAccion As String, accion As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 4))
        _listParam.Add(New Datos.DParametro("@Desde", desde))
        _listParam.Add(New Datos.DParametro("@Hasta", hasta))
        _listParam.Add(New Datos.DParametro("@UsuarioAccion", usuarioAccion))
        _listParam.Add(New Datos.DParametro("@Accion", accion))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Auditoria", _listParam)
        Return _Tabla
    End Function

    Public Shared Function CompararCabeceraAuditoriaCompras(id As Integer, accion As String, fechaAccion As String, horaAccion As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 5))
        _listParam.Add(New Datos.DParametro("@Id", id))
        _listParam.Add(New Datos.DParametro("@Accion", accion))
        _listParam.Add(New Datos.DParametro("@FechaAccion", fechaAccion))
        _listParam.Add(New Datos.DParametro("@HoraAccion", horaAccion))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Auditoria", _listParam)
        Return _Tabla
    End Function

    Public Shared Function CompararDetalleAuditoriaCompras(id As Integer, accion As String, fechaAccion As String, horaAccion As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 6))
        _listParam.Add(New Datos.DParametro("@Id", id))
        _listParam.Add(New Datos.DParametro("@Accion", accion))
        _listParam.Add(New Datos.DParametro("@FechaAccion", fechaAccion))
        _listParam.Add(New Datos.DParametro("@HoraAccion", horaAccion))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Auditoria", _listParam)
        Return _Tabla
    End Function

    '' ---------------- MOVIMIENTOS ----------------
    Public Shared Function ListarEventosAuditoriaMovimientos(desde As String, hasta As String, usuarioAccion As String, accion As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 7))
        _listParam.Add(New Datos.DParametro("@Desde", desde))
        _listParam.Add(New Datos.DParametro("@Hasta", hasta))
        _listParam.Add(New Datos.DParametro("@UsuarioAccion", usuarioAccion))
        _listParam.Add(New Datos.DParametro("@Accion", accion))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Auditoria", _listParam)
        Return _Tabla
    End Function

    Public Shared Function CompararCabeceraAuditoriaMovimientos(id As Integer, accion As String, fechaAccion As String, horaAccion As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 8))
        _listParam.Add(New Datos.DParametro("@Id", id))
        _listParam.Add(New Datos.DParametro("@Accion", accion))
        _listParam.Add(New Datos.DParametro("@FechaAccion", fechaAccion))
        _listParam.Add(New Datos.DParametro("@HoraAccion", horaAccion))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Auditoria", _listParam)
        Return _Tabla
    End Function

    Public Shared Function CompararDetalleAuditoriaMovimientos(id As Integer, accion As String, fechaAccion As String, horaAccion As String) As DataTable
        Dim _Tabla As DataTable
        Dim _listParam As New List(Of Datos.DParametro)
        _listParam.Add(New Datos.DParametro("@tipo", 9))
        _listParam.Add(New Datos.DParametro("@Id", id))
        _listParam.Add(New Datos.DParametro("@Accion", accion))
        _listParam.Add(New Datos.DParametro("@FechaAccion", fechaAccion))
        _listParam.Add(New Datos.DParametro("@HoraAccion", horaAccion))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _Tabla = D_ProcedimientoConParam("MAM_Auditoria", _listParam)
        Return _Tabla
    End Function

#End Region
