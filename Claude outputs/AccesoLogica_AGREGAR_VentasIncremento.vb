'============================================================================
' AGREGAR ESTO a Negocio\AccesoLogica.vb
' No reemplaza nada existente - son 3 funciones nuevas, pegalas donde quieras
' dentro de la Class AccesoLogica (por ejemplo junto a las funciones que
' agregamos para MAM_AuditoriaVentas, o antes del "End Class" final del
' archivo). Mismo patron de siempre (List(Of DParametro) + D_ProcedimientoConParam).
' NOTA: la 3ra funcion (L_prListarHistorialIncrementoVenta) requiere haber
' corrido MAM_VentasIncremento_v2_Historial.sql (agrega el @tipo=3 al SP).
'============================================================================

    Public Shared Function L_fnObtenerIncrementoVentaHoy() As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))

        _Tabla = D_ProcedimientoConParam("MAM_VentasIncremento", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prGuardarIncrementoVentaHoy(Monto As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@Monto", Monto))

        _Tabla = D_ProcedimientoConParam("MAM_VentasIncremento", _listParam)

        Return _Tabla
    End Function

    ''Historial completo (quien / cuanto / que dia) para Rep_HistorialIncrementoVenta.
    ''FechaDesde/FechaHasta en formato "yyyy/MM/dd" (mismo criterio que L_prListarAuditoriaVentas).
    Public Shared Function L_prListarHistorialIncrementoVenta(FechaDesde As String, FechaHasta As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 3))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@fechaDesde", FechaDesde))
        _listParam.Add(New Datos.DParametro("@fechaHasta", FechaHasta))

        _Tabla = D_ProcedimientoConParam("MAM_VentasIncremento", _listParam)

        Return _Tabla
    End Function
