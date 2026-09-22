'============================================================================
' AGREGAR ESTO a Negocio\AccesoLogica.vb
' No reemplaza nada existente - son 2 funciones nuevas, pegalas donde quieras
' dentro de la Class AccesoLogica (por ejemplo cerca de ListaVentasDetalles,
' linea ~1393, o simplemente antes del "End Class" final del archivo).
' Siguen exactamente el mismo patron que ya usas (List(Of DParametro) +
' D_ProcedimientoConParam), igual que ListaVentasDetalles y L_prListarHistorico.
'============================================================================

    Public Shared Function L_prListarAuditoriaVentas(fechaI As String, fechaF As String, tipoEvento As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 1))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@fechaDesde", fechaI))
        _listParam.Add(New Datos.DParametro("@fechaHasta", fechaF))
        _listParam.Add(New Datos.DParametro("@tipoEvento", tipoEvento))

        _Tabla = D_ProcedimientoConParam("MAM_AuditoriaVentas", _listParam)

        Return _Tabla
    End Function

    Public Shared Function L_prListarAuditoriaVentasDetalle(EventoId As String) As DataTable
        Dim _Tabla As DataTable

        Dim _listParam As New List(Of Datos.DParametro)

        _listParam.Add(New Datos.DParametro("@tipo", 2))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@EventoId", EventoId))

        _Tabla = D_ProcedimientoConParam("MAM_AuditoriaVentas", _listParam)

        Return _Tabla
    End Function
