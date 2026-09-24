'============================================================================
' MODIFICAR EN Negocio\AccesoLogica.vb
' Esto REEMPLAZA la funcion VentaInsertar existente (agrega el parametro
' IncrementoVenta y el @IncrementoVenta al llamado del SP). NO se toca
' VentaModificar (el incremento no se re-aplica al modificar una venta ya
' guardada, a proposito - ver comentario en Tec_Ventas.vb).
'
' Buscá la funcion "Public Shared Function VentaInsertar(..." actual y
' reemplazala completa por esta version.
'============================================================================

    Public Shared Function VentaInsertar(ByRef _numi As String, AlmacenId As Integer,
                                           FechaTransacccion As String, PersonalId As Integer, ClienteId As Integer, TipoVenta As Integer,
       FechaVencCredito As String, estado As Integer, glosa As String,
                                           TotalCompra As Double, _dtDetalle As DataTable,
                                           Descuento As Double, EstadoPedido As Integer, FechaEntregar As String, VentaDirecta As Integer, VentaDirectaSinConciliacion As Integer,
                                           IncrementoVenta As Double) As Boolean
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


        _listParam.Add(New Datos.DParametro("@MonedaVenta", 1))
        _listParam.Add(New Datos.DParametro("@Estado", estado))
        _listParam.Add(New Datos.DParametro("@Glosa", glosa))
        _listParam.Add(New Datos.DParametro("@TotalVenta", TotalCompra))
        _listParam.Add(New Datos.DParametro("@Descuento", Descuento))
        _listParam.Add(New Datos.DParametro("@VentaDetalleType", "", _dtDetalle))
        _listParam.Add(New Datos.DParametro("@usuario", L_Usuario))
        _listParam.Add(New Datos.DParametro("@EstadoPedido", EstadoPedido))
        _listParam.Add(New Datos.DParametro("@FechaEntregar", FechaEntregar))
        _listParam.Add(New Datos.DParametro("@Ventadirecta", VentaDirecta))
        _listParam.Add(New Datos.DParametro("@VentaDirectaSinConciliacion", VentaDirectaSinConciliacion))
        _listParam.Add(New Datos.DParametro("@IncrementoVenta", IncrementoVenta))
        _Tabla = D_ProcedimientoConParam("MAM_Ventas", _listParam)

        If _Tabla.Rows.Count > 0 Then
            _numi = _Tabla.Rows(0).Item(0)
            _resultado = True

        Else
            _resultado = False
        End If

        Return _resultado
    End Function
