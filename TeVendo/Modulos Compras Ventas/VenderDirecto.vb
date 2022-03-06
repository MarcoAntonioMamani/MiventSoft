Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.Controls
Imports System.Drawing.Printing
Imports CrystalDecisions.Shared

Public Class VenderDirecto
    Dim _CodCliente As Integer = 2
    Dim _CodEmpleado As Integer = 0
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Dim FilaSelectLote As DataRow = Nothing
    Dim Table_Producto As DataTable
    Dim G_Lote As Boolean = False '1=igual a mostrar las columnas de lote y fecha de Vencimiento
    Dim Sucursal As Integer = 1
    Dim OcultarFact As Integer = 0
    Dim _codeBar As Integer = 1
    Dim _dias As Integer = 0
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public TotalBs As Double = 0
    Public TotalSus As Double = 0
    Public TotalTarjeta As Double = 0
    Public TipoCambio As Double = 0
    Dim ListImagenes As String()
    Dim contador As Integer = 0

    Dim IdCliente As Integer = 0
    Dim dtDescuentos As DataTable = Nothing
    Public Programa As String
    Dim Lote As Boolean = False

    Dim dtCodigoBarras As DataTable
    Private Sub _IniciarTodo()
        Me.WindowState = FormWindowState.Maximized
        LeerConfiguracion()

        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "Gestion De Ventas"
        Dim blah As New Bitmap(New Bitmap(My.Resources.iconcierre), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico


        dtCodigoBarras = ListProductoCodigoBarraAll()

    End Sub
    Public Sub LeerConfiguracion()
        Dim dt As DataTable = L_prLeerConfiguracion()
        If (dt.Rows.Count > 0) Then
            Lote = dt.Rows(0).Item("Lote")
        End If
    End Sub

    Private Sub _prCargarDetalleVenta(_numi As String)
        Dim dt As New DataTable
        dt = ListaVentasDetalles(_numi)
        grdetalle.DataSource = dt
        grdetalle.RetrieveStructure()
        grdetalle.AlternatingColors = True
        '     a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        '     a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '     1 As estado, cast('' as image ) as img
        '     ,   as stock
        With grdetalle.RootTable.Columns("Id")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False

        End With
        With grdetalle.RootTable.Columns("VentaId")
            .Width = 100
            .Visible = False

        End With


        With grdetalle.RootTable.Columns("ProductoId")
            .Width = 30
            .Visible = True
            .Caption = "Cod Producto"
        End With

        With grdetalle.RootTable.Columns("Producto")
            .Width = 150
            .Caption = "Producto"
            .Visible = True
            .WordWrap = True
            .MaxLines = 3
        End With

        With grdetalle.RootTable.Columns("Cantidad")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad".ToUpper
        End With
        With grdetalle.RootTable.Columns("Tipo")
            .Width = 100
            .Visible = False

        End With
        With grdetalle.RootTable.Columns("TipoNombre")
            .Width = 40
            .Visible = False
            .Caption = "Tipo"
        End With
        With grdetalle.RootTable.Columns("KitId")
            .Width = 100
            .Visible = False

        End With
        With grdetalle.RootTable.Columns("KitNombre")
            .Width = 80
            .Visible = False
            .Caption = "Kit"
            .WordWrap = True
            .MaxLines = 2
        End With

        With grdetalle.RootTable.Columns("Precio")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "Precio"
            .FormatString = "0.00"
        End With

        With grdetalle.RootTable.Columns("SubTotal")
            .Width = 60
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "SubTotal"
            .FormatString = "0.00"
        End With
        '     a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        '     a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '     1 As estado, cast('' as image ) as img
        '     ,   as stock
        With grdetalle.RootTable.Columns("CantidadKit")
            .Width = 40
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "CantidadKit"
        End With
        With grdetalle.RootTable.Columns("ProcentajeDescuento")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0"
            .Caption = "%.Descuento".ToUpper
        End With

        With grdetalle.RootTable.Columns("MontoDescuento")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "M.Descuento".ToUpper
        End With


        With grdetalle.RootTable.Columns("Total")
            .Width = 60
            .Visible = False
            .Caption = "Total".ToUpper
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With

        With grdetalle.RootTable.Columns("Detalle")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grdetalle.RootTable.Columns("PrecioCosto")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("stock")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .Visible = False
        End With


        With grdetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grdetalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar".ToUpper
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = True
        End With



        If (Lote = True) Then
            With grdetalle.RootTable.Columns("Lote")
                .Width = 60
                .Caption = "lote".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
            With grdetalle.RootTable.Columns("FechaVencimiento")
                .Width = 70
                .Caption = "FECHA VENC.".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = True
            End With
        Else

            With grdetalle.RootTable.Columns("Lote")
                .Width = 120
                .Caption = "lote".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = False
            End With
            With grdetalle.RootTable.Columns("FechaVencimiento")
                .Width = 120
                .Caption = "FECHA VENC.".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = False
            End With


        End If

        With grdetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed
        End With
        CargarIconEstado()
    End Sub
    Public Sub CargarIconEstado()


        Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1

            Dim Bin As New MemoryStream
            Dim img As New Bitmap(My.Resources.rowdelete, 30, 28)
            img.Save(Bin, Imaging.ImageFormat.Png)
            CType(grdetalle.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer


        Next



    End Sub
    Private Sub _Limpiar()
        Dim dtclientes As DataTable = L_prListarGeneral("MAM_Clientes")

        Dim fila01 As DataRow() = dtclientes.Select("Id =1")
        If (Not IsDBNull(fila01)) Then
            If (fila01.Count > 0) Then

                IdCliente = fila01(0).Item("Id")
                lbCliente.Text = fila01(0).Item("NombreCliente").ToString


            End If
        End If

        tbProducto.Clear()
        tbTotal.Value = 0
        lbFecha.Text = Now.Date.ToString("dd/MM/yyyy")

        lbNit.Text = "0"


        _prCargarDetalleVenta(-1)

        'txtCambio1.Value = 0
        'txtMontoPagado1.Value = 0



        'If (GPanelProductos.Visible = True) Then
        '    GPanelProductos.Visible = False

        'End If
        With grdetalle.RootTable.Columns("img")
            .Width = 55
            .Caption = "Eliminar"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = False
        End With


        'tbCliente.Focus()


        FilaSelectLote = Nothing

        ' tbCliente.Focus()
        Table_Producto = Nothing
        tbDescripcion.Clear()
        tbPrecio.Text = ""
        tbDescuento.Value = 0

        _prCargarProductos()


    End Sub
    Public Sub _prGuardar()
        If _ValidarCampos() = False Then
            Exit Sub
        End If


        _GuardarNuevo()

    End Sub

    Public Function _GuardarNuevo() As Boolean
        'ByRef _numi As String, ConceptoId As Integer, DepositoId As Integer,Observacion as String,
        'Estado as integer, FechaDocumento As String, _dtDetalle As DataTable
        'tbFechaTransaccion.Value.ToString("yyyy/MM/dd")   CType(grDetalle.DataSource, DataTable)
        Dim res As Boolean
        Dim Id As String = "0"
        Try


            Dim ef = New Efecto
            ef.tipo = 20
            ef.TotalVenta = tbTotal.Value
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then

                Dim dt As DataTable = ListaVentasDetallePago(-1)
                'a.Id , a.VentaId, a.MontoBs, a.MontoDolares, a.TarjetaBancaria, a.TransferenciaBancaria, a.TipoCambio, 1 as estado
                dt.Rows.Add(0, 0, ef.MontoBs, ef.MontoDolares, ef.MontoTarjeta, ef.MontoTransferencia, Global_TipoCambio, 0)


                res = VentaInsertar(Id, 1, Now.Date.ToString("yyyy/MM/dd"),
                                   Global_IdPersonal, IdCliente, 1, Now.Date.ToString("yyyy/MM/dd"),
                                   1, 1, "", tbTotal.Value, CType(grdetalle.DataSource, DataTable), 0, dt, 0)

                If res Then


                    If (ef.TipGrabado = 1) Then
                        P_GenerarReporte(Id)
                    End If

                    ToastNotification.Show(Me, "Codigo de Venta ".ToUpper + Id + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    FilaSelectLote = Nothing

                    _Limpiar()

                Else
                    ToastNotification.Show(Me, "Error al guardar la Venta".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

                End If
            End If




        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar la Venta".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Private Sub P_GenerarReporte(numi As String)

        Try
            Dim dt As DataTable = ListarVentaRecibo(numi)
            Dim RutaGlobal As String = gs_CarpetaRaiz
            Dim total As Decimal = dt.Compute("SUM(Total)", "")
            total = total - dt.Rows(0).Item("DescuentoVenta")
            Dim fechaven As String = dt.Rows(0).Item("FechaVenta")
            Dim dtImage As DataTable = ObtenerImagenEmpresa()
            If (dtImage.Rows.Count > 0) Then
                Dim Name As String = dtImage.Rows(0).Item(0)
                If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name)) Then
                    Dim im As New Bitmap(New Bitmap(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name))
                    Dim Bin As New MemoryStream
                    Dim img As New Bitmap(im)
                    img.Save(Bin, Imaging.ImageFormat.Png)
                    Bin.Dispose()
                    For i As Integer = 0 To dt.Rows.Count - 1 Step 1


                        dt.Rows(i).Item("imageEmpresa") = Bin.GetBuffer
                    Next
                End If


            End If

            Dim _FechaAct As String
            Dim _Fecha() As String
            Dim _FechaPar As String
            Dim _Meses() As String = {"Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"}
            _FechaAct = fechaven
            _Fecha = Split(_FechaAct, "-")
            _FechaPar = "La Paz, " + _Fecha(0).Trim + " De " + _Meses(_Fecha(1) - 1).Trim + " Del " + _Fecha(2).Trim

            If Not IsNothing(P_Global.Visualizador) Then
                P_Global.Visualizador.Close()
            End If
            Dim ParteEntera As Long
            Dim ParteDecimal As Decimal
            ParteEntera = Int(total)
            ParteDecimal = Math.Round(total - ParteEntera, 2)
            Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(ParteEntera)) + " con " +
        IIf(ParteDecimal.ToString.Equals("0"), "00", ParteDecimal.ToString) + "/100 Bolivianos"



            P_Global.Visualizador = New Visualizador

            ''Dim objrep As New Recibo





            Dim objrep As New Recibo07_1000



                objrep.SetDataSource(dt)
                objrep.SetParameterValue("Literal1", li)

            If (gs_ImprimirDirecto.Equals("Si")) Then
                Dim pd As New PrintDocument()

                'Dim _Ds3 = L_ObtenerRutaImpresora("2") ' Datos de Impresion de Facturación
                Dim NombreImpresora As String = ""
                pd.PrinterSettings.PrinterName = gs_NombreImpresora

                If (Not pd.PrinterSettings.IsValid) Then
                    ToastNotification.Show(Me, "La Impresora ".ToUpper + NombreImpresora + Chr(13) + "No Existe".ToUpper,
                                      img, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
                Else
                    objrep.PrintOptions.PrinterName = NombreImpresora

                    objrep.PrintToPrinter(1, True, 0, 0)
                End If


            Else
                P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
                P_Global.Visualizador.CrGeneral.Zoom(130)
                P_Global.Visualizador.Show() 'Comentar
            End If








            ''P_Global.Visualizador.BringToFront() 'Comentar

        Catch ex As Exception
            ToastNotification.Show(Me, "Error al Generar el Reporte " + ex.Message, img, 16000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End Try


    End Sub
    Public Function _ValidarCampos() As Boolean
        Try



            If (grdetalle.RowCount = 1) Then
                grdetalle.Row = grdetalle.RowCount - 1
                If (grdetalle.GetValue("ProductoId") = 0) Then
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "Por Favor Seleccione  un detalle de producto".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.TopCenter)
                    Return False
                End If

            End If

            Dim dt As DataTable = L_prListarGeneral("MAM_CierreCajero")

            Dim fila As DataRow() = dt.Select("SucursalId=" + Str(1) + " and EstadoCaja=1 and PersonalId=" + Str(Global_IdPersonal).Trim)


            If (Not IsDBNull(fila)) Then
                If (fila.Count <= 0) Then

                    ToastNotification.Show(Me, "No Es Posible Hacer EL Movimiento Por que no Existe Caja Chica con Estado Abierto Para Esta Fecha =" + Now.Date.ToString("dd/MM/yyy"), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                    tbProducto.Focus()

                    Return False
                Else
                    Dim bandera As Boolean = False
                    For Each item As Object In fila
                        If (item("Fecha") = Now.Date) Then
                            bandera = True
                        End If
                    Next
                    If (bandera = False) Then
                        ToastNotification.Show(Me, "No Es Posible Hacer EL Movimiento Por que no Existe Caja Chica con Estado Abierto Para Esta Fecha =" + Now.Date.ToString("dd/MM/yyy"), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                        tbProducto.Focus()

                        Return False
                    End If
                End If
            End If

            'Validación para controlar caducidad de Dosificacion


            Return True
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
            Return False
        End Try

    End Function



    Private Sub _prCargarProductos()
        Dim dt As New DataTable



        dt = L_prListarProductosVentas(1, 1)  ''1=Almacen
        'a.Id , a.NombreProducto, PCosto.Precio As PrecioCosto,
        ''PVenta.Precio as PrecioVenta
        grProductos.DataSource = dt
        grProductos.RetrieveStructure()
        grProductos.AlternatingColors = True
        With grProductos.RootTable.Columns("Id")
            .Width = 100
            .Caption = "Cod Producto"
            .Visible = True


        End With
        With grProductos.RootTable.Columns("Tipo")
            .Width = 100
            .Caption = "Tipo"
            .Visible = False


        End With
        With grProductos.RootTable.Columns("CodigoExterno")
            .Width = 100
            .Caption = "Cod Externo"
            .Visible = False

        End With

        With grProductos.RootTable.Columns("estado")
            .Width = 100
            .Visible = False

        End With
        With grProductos.RootTable.Columns("NombreProducto")
            .Width = 300
            .Caption = "PRODUCTOS"
            .Visible = True
            .MaxLines = 3
            .WordWrap = True
        End With

        With grProductos.RootTable.Columns("DescripcionProducto")
            .Width = 250
            .Visible = False
            .MaxLines = 2
            .WordWrap = True
            .Caption = "DESCRIPCION"
        End With
        With grProductos.RootTable.Columns("NombreTipo")
            .Width = 90
            .Visible = False
            .MaxLines = 2
            .WordWrap = True
            .Caption = "Tipo"
        End With
        With grProductos.RootTable.Columns("NombreCategoria")
            .Width = 120
            .Visible = False
            .MaxLines = 2
            .WordWrap = True
            .Caption = "CATEGORIA"
        End With
        With grProductos.RootTable.Columns("industria")
            .Width = 120
            .Visible = False
            .MaxLines = 2
            .WordWrap = False
            .Caption = "Industria"
        End With

        With grProductos.RootTable.Columns("PrecioCosto")
            .Width = 150
            .Visible = False
            .FormatString = "0.00"
        End With
        With grProductos.RootTable.Columns("stock")
            .Width = 150
            .Visible = True
            .Caption = "Stock"
            .FormatString = "0.00"
        End With
        With grProductos.RootTable.Columns("PrecioVenta")
            .Width = 150
            .Visible = True
            .Caption = "Precio"
            .FormatString = "0.00"
        End With
        With grProductos
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
        _prAplicarCondiccionJanusSinLote()
    End Sub
    Public Sub _prAplicarCondiccionJanusSinLote()


        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProductos.RootTable.Columns("stock"), ConditionOperator.Equal, 0)
        'fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.White
            fc.FormatStyle.BackColor = Color.Red
        grProductos.RootTable.FormatConditions.Add(fc)


    End Sub
    Private Sub MostrarMensajeError(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.mensaje,
                               5000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
    Public Sub actualizarSaldo(ByRef dt As DataTable, CodProducto As Integer)
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 

        '      a.tbnumi ,a.tbtv1numi ,a.tbty5prod ,b.yfcdprod1 as producto,a.tbest ,a.tbcmin ,a.tbumin ,Umin .ycdes3 as unidad,a.tbpbas ,a.tbptot ,a.tbobs ,
        'a.tbpcos,a.tblote ,a.tbfechaVenc , a.tbptot2, a.tbfact ,a.tbhact ,a.tbuact,1 as estado,Cast(null as Image) as img,
        'Cast (0 as decimal (18,2)) as stock
        Dim _detalle As DataTable = CType(grdetalle.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim lote As String = dt.Rows(i).Item("Lote")
            Dim FechaVenc As Date = dt.Rows(i).Item("FechaVencimiento")
            Dim sum As Integer = 0
            For j As Integer = 0 To _detalle.Rows.Count - 1
                Dim estado As Integer = _detalle.Rows(j).Item("estado")
                If (estado = 0) Then
                    If (lote = _detalle.Rows(j).Item("Lote") And
                        FechaVenc = _detalle.Rows(j).Item("FechaVencimiento") And CodProducto = _detalle.Rows(j).Item("ProductoId")) Then
                        sum = sum + _detalle.Rows(j).Item("Cantidad")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next

    End Sub
    Private Sub _prCargarLotesDeProductos(CodProducto As Integer, nameProducto As String)

        Dim dt As New DataTable

        dt = LotesPorProducto(1, CodProducto)  ''1=Almacen
        actualizarSaldo(dt, CodProducto)
        grProductos.DataSource = dt
        grProductos.RetrieveStructure()
        grProductos.AlternatingColors = True
        With grProductos.RootTable.Columns("NombreProducto")
            .Width = 150
            .Visible = False

        End With
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 
        With grProductos.RootTable.Columns("Lote")
            .Width = 150
            .Caption = "Lote"
            .Visible = True

        End With
        With grProductos.RootTable.Columns("FechaVencimiento")
            .Width = 160
            .Caption = "Fecha Vencimiento"
            .FormatString = "yyyy/MM/dd"
            .Visible = True

        End With

        With grProductos.RootTable.Columns("stock")
            .Width = 150
            .Visible = True
            .Caption = "Stock"
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With


        With grProductos
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowPosition = TotalRowPosition.BottomFixed
            .VisualStyle = VisualStyle.Office2007
        End With
        _prAplicarCondiccionJanusLote()

    End Sub

    Private Sub _prAddDetalleVenta()
        '     a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        '     a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '     1 As estado, cast('' as image ) as img
        '     , 0 as stock
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.rowdelete, 25, 18)
        img.Save(Bin, Imaging.ImageFormat.Png)
        CType(grdetalle.DataSource, DataTable).Rows.Add(_GenerarId() + 1, 0, 0, "", 0, 0, 0, 0, 0, 0, "", 0, "20200101", CDate("2020/01/01"), 0, "", 0, "", 0, 0, Bin.GetBuffer, 0)
    End Sub
    Public Function _GenerarId()
        Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(grdetalle.DataSource, DataTable).Rows(i).Item("Id")), 0, CType(grdetalle.DataSource, DataTable).Rows(i).Item("Id"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function

    Public Sub _prAplicarCondiccionJanusLote()


        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProductos.RootTable.Columns("stock"), ConditionOperator.Equal, 0)
        fc.FormatStyle.BackColor = Color.Gold
        fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.White
        grProductos.RootTable.FormatConditions.Add(fc)

        Dim fc2 As GridEXFormatCondition
        fc2 = New GridEXFormatCondition(grProductos.RootTable.Columns("FechaVencimiento"), ConditionOperator.LessThanOrEqualTo, Now.Date)
        fc2.FormatStyle.BackColor = Color.Red
        fc2.FormatStyle.FontBold = TriState.True
        fc2.FormatStyle.ForeColor = Color.White
        grProductos.RootTable.FormatConditions.Add(fc2)




    End Sub

    Private Sub _HabilitarProductos()
        GPanelProductos.Visible = True

        _prCargarProductos()
        grProductos.Focus()
        grProductos.MoveTo(grProductos.FilterRow)
        grProductos.Col = 2
    End Sub
    Private Sub _HabilitarFocoDetalle(fila As Integer)
        _prCargarProductos()
        grdetalle.Focus()
        grdetalle.Row = fila
        grdetalle.Col = grdetalle.RootTable.Columns("cantidad").Index
    End Sub
    Private Sub _DesHabilitarProductos()
        'GPanelProductos.Visible = False

        tbProducto.Focus()
        Try
            grdetalle.Select()
            grdetalle.Col = grdetalle.RootTable.Columns("cantidad").Index
            grdetalle.Row = grdetalle.RowCount - 1
        Catch ex As Exception

        End Try




    End Sub
    Public Function _fnExisteProducto(idprod As Integer, Tipo As Integer) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            If (Tipo = 1) Then  '''1=Producto   2=Kits

                If (_idprod = idprod And estado >= 0 And CType(grdetalle.DataSource, DataTable).Rows(i).Item("Tipo") = 1) Then

                    Return True
                End If

            Else
                _idprod = CType(grdetalle.DataSource, DataTable).Rows(i).Item("KitId")
                If (_idprod = idprod And estado >= 0 And CType(grdetalle.DataSource, DataTable).Rows(i).Item("Tipo") = 2) Then

                    Return True
                End If
            End If

        Next
        Return False
    End Function
    Public Function _fnExisteProductoConLote(idprod As Integer, lote As String, fechaVenci As Date, Tipo As Integer) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")

            Dim _LoteDetalle As String = CType(grdetalle.DataSource, DataTable).Rows(i).Item("Lote")
            Dim _FechaVencDetalle As Date = CType(grdetalle.DataSource, DataTable).Rows(i).Item("FechaVencimiento")
            If (Tipo = 1) Then  ''1=productos   2=Kits

                If (_idprod = idprod And estado >= 0 And lote = _LoteDetalle And fechaVenci = _FechaVencDetalle) Then

                    Return True
                End If

            Else
                Dim KitId As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("KitId")
                If (KitId = idprod) Then
                    Return True
                End If


            End If


        Next
        Return False
    End Function
    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer, Tipo As Integer)
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi And CType(grdetalle.DataSource, DataTable).Rows(i).Item("Tipo") = Tipo And CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado") >= 0) Then
                pos = i
                Return
            End If
        Next

    End Sub
    Public Function StringTodouble(Valor As String) As Double
        Try
            Return CDbl(Valor)
        Catch ex As Exception
            Return 0
        End Try

    End Function

    Public Sub _prCalcularPrecioTotal()



        Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)

        If (IsNothing(dt)) Then
            Return

        End If



        Dim montodesc As Double = 0

        Dim total As Double = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            If (dt.Rows(i).Item("estado") >= 0) Then

                total += dt.Rows(i).Item("Total")
            End If
        Next



        Dim pordesc As Double = ((montodesc * 100) / total)

        tbTotal.Value = total - montodesc
        tbDescuento.Value = montodesc
    End Sub
    Public Sub CambiarEstado(ProductoId As Integer, Estado As Integer)
        If (IsNothing(grProductos.DataSource)) Then
            Return

        End If
        For i As Integer = 0 To CType(grProductos.DataSource, DataTable).Rows.Count - 1 Step 1
            If (CType(grProductos.DataSource, DataTable).Rows(i).Item("Id") = ProductoId And CType(grProductos.DataSource, DataTable).Rows(i).Item("Tipo") = 1) Then
                CType(grProductos.DataSource, DataTable).Rows(i).Item("estado") = Estado
            End If

        Next
        'grProductos.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grProductos.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.Equal, 1))
    End Sub
    Public Sub _prEliminarFila()
        If (grdetalle.Row >= 0) Then
            If (grdetalle.RowCount >= 1) Then
                Dim estado As Integer = grdetalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grdetalle.GetValue("Id")
                _fnObtenerFilaDetalle(pos, lin, grdetalle.GetValue("Tipo"))
                Dim TipoKit As Integer = grdetalle.GetValue("Tipo")

                If (TipoKit = 1) Then  ''Productos

                    CambiarEstado(grdetalle.GetValue("ProductoId"), 1)
                    If (estado = 0) Then
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -2

                    End If
                    If (estado = 1) Then
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -1
                    End If

                    grdetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grdetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
                Else ''Kits

                    Dim KitId As Integer = grdetalle.GetValue("KitId")
                    Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)

                    For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                        If (dt.Rows(i).Item("KitId") = KitId) Then
                            If (estado = 0) Then
                                CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado") = -2

                            End If
                            If (estado = 1) Then
                                CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado") = -1
                            End If

                        End If

                    Next







                    grdetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grdetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
                End If



            End If
        End If

        _prCalcularPrecioTotal()

    End Sub

    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < grdetalle.RowCount) Then

            Dim lin As Integer = grdetalle.GetValue("Id")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin, grdetalle.GetValue("Tipo"))
            Dim cant As Double = grdetalle.GetValue("Cantidad")
            Dim uni As Double = grdetalle.GetValue("Precio")
            Dim MontoDesc As Double = StringTodouble(grdetalle.GetValue("MontoDescuento").ToString)
            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni
                'grDetalle.SetValue("lcmdes", montodesc)

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = TotalUnitario
                grdetalle.SetValue("SubTotal", TotalUnitario)
                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")


                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = TotalUnitario - MontoDesc
                grdetalle.SetValue("Total", TotalUnitario - MontoDesc)
                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            End If

        End If




    End Sub
    Private Sub F0_VentasSupermercado_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _IniciarTodo()
        _Limpiar()

        tbProducto.Focus()
    End Sub

    Private Sub tbProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles tbProducto.KeyDown
        If e.KeyData = Keys.Control + Keys.C Then

            Dim dt As DataTable
            'dt = L_fnListarClientes()
            dt = ListarCliente()
            'a.Id ,a.NombreCliente  as NombreProveedor ,a.DireccionCliente  ,a.Telefono

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id, ", False, "ID", 50))
            listEstCeldas.Add(New Celda("CodigoExterno,", True, "Cod Ext.", 70))
            listEstCeldas.Add(New Celda("NombreProveedor", True, "NOMBRE", 350))
            listEstCeldas.Add(New Celda("DireccionCliente", True, "DIRECCION", 180))
            listEstCeldas.Add(New Celda("Telefono", True, "Telefono".ToUpper, 200))


            Dim ef = New Efecto
            ef.tipo = 7
            ef.dt = dt
            ef.SeleclCol = 0
            ef.listEstCeldasNew = listEstCeldas
            ef.alto = 80
            ef.ancho = 800
            ef.Context = "Seleccione Cliente".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                IdCliente = Row.Cells("ID").Value
                lbCliente.Text = Row.Cells("NombreProveedor").Value.ToString

            Else
                If (ef.NewCliente) Then
                    IdCliente = ef.IdCliente
                    lbCliente.Text = ef.NombreCliente

                End If
            End If
            tbProducto.Focus()
        End If
        'If e.KeyData = Keys.Control + Keys.Enter Then

        '    _HabilitarProductos()
        '    Return

        'End If
        If (e.KeyData = Keys.Control + Keys.S) Then
            If _ValidarCampos() = False Then
                Exit Sub
            End If
            _prGuardar()


        End If
        If (e.KeyData = Keys.Enter) Then
            '''''Aqui se inserta por codigo de barras

            If (tbProducto.Text.Trim <> String.Empty) Then
                Dim CodigoB As String = tbProducto.Text.Trim

                Dim fila As DataRow() = dtCodigoBarras.Select("CodigoBarras='" + CodigoB + "'")


            If (Not IsDBNull(fila)) Then
                    If (fila.Count <= 0) Then

                        ToastNotification.Show(Me, "Codigo de Barras No esta Relacionado a ningun producto = " + tbProducto.Text, img, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)

                        tbProducto.Text = ""
                        tbProducto.Focus()
                        Return
                    Else
                        grProductos.RemoveFilters()

                        Dim ProductoId As Integer = fila(0).Item("ProductoId")

                        Dim posicion As Integer = 0
                        Dim dt As DataTable = CType(grProductos.DataSource, DataTable)

                        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                            If (dt.Rows(i).Item("Id") = ProductoId) Then

                                posicion = i
                                Exit For

                            End If

                        Next
                        If (posicion >= 0) Then
                            grProductos.Row = posicion

                            seleccionarProductoCodigoBarra()


                        End If


                    End If

                Else

                    ToastNotification.Show(Me, "Ingrese Codigo de Barras Valido", img, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)

                End If

            End If




            'Else
            '    grdetalle.DataChanged = False
            '    ToastNotification.Show(Me, "El código de barra del producto no existe", img, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If

        If e.KeyData = Keys.Escape Then
            Me.Close()

        End If
        If e.KeyData = Keys.Down Then
            grdetalle.Focus()

        End If

        If e.KeyData = Keys.Control + Keys.B Then
            Dim FRow As GridEXRow = grProductos.FilterRow


            If (Not IsDBNull(FRow)) Then
                grProductos.Focus()
                grProductos.Row = -2
                grProductos.Col = grProductos.RootTable.Columns("NombreProducto").Index
            End If


        End If
    End Sub

    Private Sub grProductos_DoubleClick(sender As Object, e As EventArgs) Handles grProductos.DoubleClick

        Try
            Dim f, c As Integer
            c = grProductos.Col
            f = grProductos.Row
            If (f >= 0) Then

                seleccionarProducto()

            End If
        Catch ex As Exception

        End Try

    End Sub
    Public Function _fnExisteProductoLote(idprod As Integer, mLote As String, mFecha As Date) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            Dim _Lote As String = CType(grdetalle.DataSource, DataTable).Rows(i).Item("Lote")
            Dim _FechaVenc As Date = CType(grdetalle.DataSource, DataTable).Rows(i).Item("FechaVencimiento")
            If (_idprod = idprod And estado >= 0 And _Lote.Trim.Equals(mLote.Trim) And _FechaVenc = mFecha) Then

                Return True
            End If
        Next
        Return False
    End Function
    Public Sub InsertarProductosConLote(cantidad As Double, mLote As String, mFechaVen As Date)
        'd.Id , d.CompraId, d.ProductoId, p.NombreProducto As Producto, d.Cantidad, d.PrecioCosto,
        'd.Lote, d.FechaVencimiento, d.TotalCompra, d.PrecioVenta, 1 As estado, cast('' as image) as img,
        'd.PrecioCosto As costo, d.PrecioVenta  as venta
        Dim pos As Integer = -1
        If (grdetalle.Row < 0) Then
            _prAddDetalleVenta()
        End If



        Dim existe As Boolean = _fnExisteProductoLote(grProductos.GetValue("Id"), mLote, mFechaVen)
        If ((Not existe)) Then
            If (grdetalle.GetValue("ProductoId") > 0) Then
                _prAddDetalleVenta()
            End If
            grdetalle.Row = grdetalle.RowCount - 1
            _fnObtenerFilaDetalle(pos, grdetalle.GetValue("Id"), grdetalle.GetValue("Tipo"))
            If ((pos >= 0)) Then
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = grProductos.GetValue("Id")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Producto") = grProductos.GetValue("NombreProducto")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = grProductos.GetValue("PrecioCosto")

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("PrecioVenta") = grProductos.GetValue("PrecioVenta")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("costo") = grProductos.GetValue("PrecioCosto")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("venta") = grProductos.GetValue("PrecioVenta")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = cantidad
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Lote") = mLote
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("FechaVencimiento") = mFechaVen
                ''    _DesHabilitarProductos()


                CambiarEstado(grProductos.GetValue("Id"), 0)
                'grproducto.RemoveFilters()
                tbProducto.Clear()
                tbProducto.Focus()
            End If




        Else
            If (existe) Then

                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grProductos.RemoveFilters()
                tbProducto.Focus()
            End If

        End If


    End Sub
    Public Sub _fnObtenerFilaProducto(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grProductos.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grProductos.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub
    Public Sub InsertarProductosConLote()

        If (grdetalle.Row < 0) Then
            _prAddDetalleVenta()
        End If

        '      a.Id ,a.icibid ,a.iccprod ,b.yfcdprod1  as producto,a.Cantidad ,
        'a.iclot ,a.icfvenc ,Cast(null as image ) as img,1 as estado,
        '(Sum(inv.iccven )+a.Cantidad  ) as stock

        'a.yfnumi  ,a.yfcdprod1  ,a.yfcdprod2,Sum(b.iccven ) as stock 
        Dim pos As Integer = -1
        grdetalle.Row = grdetalle.RowCount - 1
        _fnObtenerFilaDetalle(pos, grdetalle.GetValue("Id"), grdetalle.GetValue("Tipo"))
        Dim posProducto As Integer = 0
        _fnObtenerFilaProducto(posProducto, grProductos.GetValue("Id"))


        FilaSelectLote = CType(grProductos.DataSource, DataTable).Rows(posProducto)


        If (grProductos.GetValue("stock") > 0) Then
            _prCargarLotesDeProductos(grProductos.GetValue("Id"), grProductos.GetValue("NombreProducto"))
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "El Producto: ".ToUpper + grProductos.GetValue("NombreProducto") + " NO CUENTA CON STOCK DISPONIBLE", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            FilaSelectLote = Nothing
        End If

    End Sub

    Public Sub InsertarProductosSinLote(cantidad As Double)
        'a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        'a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '1 As estado, cast('' as image ) as img
        ', 0 as stock
        Dim pos As Integer = -1
        If (grdetalle.Row < 0) Then
            _prAddDetalleVenta()
        End If
        Dim existe As Boolean = _fnExisteProducto(grProductos.GetValue("Id"), grProductos.GetValue("Tipo"))
        If ((Not existe)) Then

            If (grProductos.GetValue("Tipo") = 1) Then  ''Inserto Productos
                If (grdetalle.GetValue("ProductoId") > 0) Then
                    _prAddDetalleVenta()
                End If
                grdetalle.Row = grdetalle.RowCount - 1
                _fnObtenerFilaDetalle(pos, grdetalle.GetValue("Id"), grdetalle.GetValue("Tipo"))
                If ((pos >= 0)) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = grProductos.GetValue("Id")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Producto") = grProductos.GetValue("NombreProducto")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = cantidad
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Precio") = grProductos.GetValue("PrecioVenta")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = grProductos.GetValue("PrecioVenta") * cantidad

                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = grProductos.GetValue("PrecioVenta") * cantidad
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = grProductos.GetValue("PrecioCosto")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProductos.GetValue("Stock")
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Tipo") = 1
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("TipoNombre") = "Productos"

                    CambiarEstado(grProductos.GetValue("Id"), 0)
                    'grProductos.RemoveFilters()
                    tbProducto.Clear()
                    tbProducto.Focus()

                    tbDescripcion.Text = grProductos.GetValue("NombreProducto")
                    tbPrecio.Text = grProductos.GetValue("PrecioVenta")


                End If

            Else ''Inserto Kits

                Dim dt As DataTable = L_prListarProductosKitsVenta(grProductos.GetValue("Id"))

                For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                    If (grdetalle.GetValue("ProductoId") > 0) Then
                        _prAddDetalleVenta()
                    End If
                    grdetalle.Row = grdetalle.RowCount - 1
                    _fnObtenerFilaDetalle(pos, grdetalle.GetValue("Id"), grdetalle.GetValue("Tipo"))
                    If ((pos >= 0)) Then
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = dt.Rows(i).Item("ProductoId")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Producto") = dt.Rows(i).Item("NombreProducto")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = dt.Rows(i).Item("Cantidad") * cantidad
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Precio") = dt.Rows(i).Item("PrecioVenta")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = (dt.Rows(i).Item("PrecioVenta")) * (dt.Rows(i).Item("Cantidad") * cantidad)

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = (dt.Rows(i).Item("PrecioVenta")) * (dt.Rows(i).Item("Cantidad") * cantidad)
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = dt.Rows(i).Item("PrecioCosto")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = dt.Rows(i).Item("Cantidad")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Tipo") = 2
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("TipoNombre") = "Kits"

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("KitId") = grProductos.GetValue("Id")

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("KitNombre") = grProductos.GetValue("NombreProducto")

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("CantidadKit") = cantidad
                        tbDescripcion.Text = grProductos.GetValue("NombreProducto")
                        tbPrecio.Text = grProductos.GetValue("PrecioVenta")

                        tbProducto.Text = ""
                    End If

                Next
                'CambiarEstado(grProductos.GetValue("Id"), 0)
                'grProductos.RemoveFilters()
                tbProducto.Clear()
                tbProducto.Focus()

            End If

        Else
            If (existe) Then
                If (grProductos.GetValue("Tipo") = 1) Then


                    Dim PosicionP As Integer = -1

                    For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1

                        If (CType(grdetalle.DataSource, DataTable).Rows(i).Item("ProductoId") = grProductos.GetValue("Id") And CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado") >= 0) Then
                            PosicionP = i
                            Exit For

                        End If

                    Next

                    If (PosicionP >= 0) Then

                        CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Cantidad") = CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Cantidad") + 1
                        Dim porcdesc As Double = grdetalle.GetValue("ProcentajeDescuento")
                        Dim montodesc As Double = ((CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Precio") * CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Cantidad")) * (porcdesc / 100))
                        CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("MontoDescuento") = montodesc
                        grdetalle.SetValue("MontoDescuento", montodesc)


                        CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("SubTotal") = CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Precio") * CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Cantidad")


                        CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Total") = CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Precio") * CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Cantidad") - montodesc


                        tbDescripcion.Text = CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Producto")
                        tbPrecio.Text = CType(grdetalle.DataSource, DataTable).Rows(PosicionP).Item("Precio")

                        tbProducto.Text = ""
                    End If

                Else

                    ToastNotification.Show(Me, "El Kit Ya Existe En el Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                End If

                grProductos.RemoveFilters()
                tbProducto.Focus()
            End If
        End If


    End Sub
    Public Sub seleccionarProducto()
        If (IsNothing(FilaSelectLote)) Then


            ''''''''''''''''''''''''



            If (Lote = True) Then
                InsertarProductosConLote()
            Else

                Dim existe As Boolean = _fnExisteProducto(grProductos.GetValue("Id"), grProductos.GetValue("Tipo"))
                If ((Not existe)) Then
                    Dim ef = New Efecto
                    ef.tipo = 5
                    ef.NombreProducto = grProductos.GetValue("NombreProducto")
                    ef.StockActual = grProductos.GetValue("stock")

                    ef.TipoMovimiento = 4


                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then
                        InsertarProductosSinLote(ef.CantidadTransaccion)

                    End If

                Else
                    InsertarProductosSinLote(1)
                End If




            End If


                Else

            Dim numiProd As Integer = FilaSelectLote.Item("Id")
            Dim mLote As String = grProductos.GetValue("Lote")
            Dim FechaVenc As Date = grProductos.GetValue("FechaVencimiento")
            If (Not _fnExisteProductoConLote(numiProd, Lote, FechaVenc, 1)) Then
                Dim ef = New Efecto
                ef.tipo = 5
                ef.NombreProducto = grProductos.GetValue("NombreProducto")
                ef.StockActual = grProductos.GetValue("stock")
                ef.TipoMovimiento = 4
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                Dim CantidadVenta As Double = 0
                If (bandera = True) Then
                    CantidadVenta = ef.CantidadTransaccion
                    If (grdetalle.GetValue("ProductoId") > 0) Then
                        _prAddDetalleVenta()
                    End If
                    Dim pos As Integer = -1
                    grdetalle.Row = grdetalle.RowCount - 1
                    _fnObtenerFilaDetalle(pos, grdetalle.GetValue("Id"), grdetalle.GetValue("Tipo"))

                    If ((pos >= 0)) Then
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = numiProd
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Producto") = FilaSelectLote.Item("NombreProducto")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = CantidadVenta
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Precio") = FilaSelectLote.Item("PrecioVenta")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = FilaSelectLote.Item("PrecioVenta") * CantidadVenta

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = FilaSelectLote.Item("PrecioVenta") * CantidadVenta
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = FilaSelectLote.Item("PrecioCosto")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProductos.GetValue("Stock")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Lote") = mLote
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("FechaVencimiento") = FechaVenc

                        tbProducto.Clear()
                        tbProducto.Focus()

                    End If

                    ''''''''

                    FilaSelectLote = Nothing
                    _DesHabilitarProductos()
                    tbProducto.Focus()
                    _prCargarProductos()
                End If

            Else
                If (grProductos.GetValue("Tipo") = 1) Then
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "El producto con el lote ya existe modifique su cantidad".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Else
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "El producto con el lote ya existe modifique su cantidad".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                End If

            End If


        End If

        _prCalcularPrecioTotal()

    End Sub


    Public Sub seleccionarProductoCodigoBarra()
        If (IsNothing(FilaSelectLote)) Then


            ''''''''''''''''''''''''



            If (Lote = True) Then
                InsertarProductosConLote()
            Else

                InsertarProductosSinLote(1)

            End If


        Else

            Dim numiProd As Integer = FilaSelectLote.Item("Id")
            Dim mLote As String = grProductos.GetValue("Lote")
            Dim FechaVenc As Date = grProductos.GetValue("FechaVencimiento")
            If (Not _fnExisteProductoConLote(numiProd, Lote, FechaVenc, 1)) Then
                Dim ef = New Efecto
                ef.tipo = 5
                ef.NombreProducto = grProductos.GetValue("NombreProducto")
                ef.StockActual = grProductos.GetValue("stock")
                ef.TipoMovimiento = 4
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                Dim CantidadVenta As Double = 0
                If (bandera = True) Then
                    CantidadVenta = ef.CantidadTransaccion
                    If (grdetalle.GetValue("ProductoId") > 0) Then
                        _prAddDetalleVenta()
                    End If
                    Dim pos As Integer = -1
                    grdetalle.Row = grdetalle.RowCount - 1
                    _fnObtenerFilaDetalle(pos, grdetalle.GetValue("Id"), grdetalle.GetValue("Tipo"))

                    If ((pos >= 0)) Then
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = numiProd
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Producto") = FilaSelectLote.Item("NombreProducto")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = CantidadVenta
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Precio") = FilaSelectLote.Item("PrecioVenta")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = FilaSelectLote.Item("PrecioVenta") * CantidadVenta

                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = FilaSelectLote.Item("PrecioVenta") * CantidadVenta
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = FilaSelectLote.Item("PrecioCosto")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProductos.GetValue("Stock")
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Lote") = mLote
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("FechaVencimiento") = FechaVenc

                        tbProducto.Clear()
                        tbProducto.Focus()

                    End If

                    ''''''''

                    FilaSelectLote = Nothing
                    _DesHabilitarProductos()
                    tbProducto.Focus()
                    _prCargarProductos()
                End If

            Else
                If (grProductos.GetValue("Tipo") = 1) Then
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "El producto con el lote ya existe modifique su cantidad".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Else
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "El producto con el lote ya existe modifique su cantidad".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                End If

            End If


        End If

        _prCalcularPrecioTotal()

    End Sub
    Private Sub grProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles grProductos.KeyDown
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grProductos.Col
            f = grProductos.Row
            If (f >= 0) Then

                seleccionarProducto()

            End If
        End If
        If (e.KeyData = Keys.Control + Keys.H) Then



            tbProducto.Focus()
        End If
        If e.KeyData = Keys.Escape Then

            If (IsNothing(FilaSelectLote)) Then
                'CType(grProductos.DataSource, DataTable).Rows.Clear()

                _DesHabilitarProductos()
            Else

                FilaSelectLote = Nothing
                _HabilitarProductos()



            End If


        End If
    End Sub
    Private Sub cargarProductos()
        Dim dt As DataTable
        If (G_Lote = True) Then
            dt = L_fnListarProductos()  ''1=Almacen
            Table_Producto = dt.Copy
        Else
            dt = L_fnListarProductos()  ''1=Almacen
            Table_Producto = dt.Copy
        End If
    End Sub

    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellValueChanged
        Dim lin As Integer = grdetalle.GetValue("Id")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin, grdetalle.GetValue("Tipo"))
        If (e.Column.Index = grdetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("Cantidad")) Or grdetalle.GetValue("Cantidad").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Precio")
                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")

                grdetalle.SetValue("Cantidad", 1)
                grdetalle.SetValue("ProcentajeDescuento", 0)
                grdetalle.SetValue("MontoDescuento", 0)
                grdetalle.SetValue("SubTotal", grdetalle.GetValue("Precio"))
                grdetalle.SetValue("Total", grdetalle.GetValue("Precio"))



                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grdetalle.GetValue("Cantidad") > 0) Then


                    Dim porcdesc As Double = grdetalle.GetValue("ProcentajeDescuento")
                    Dim montodesc As Double = ((grdetalle.GetValue("Precio") * grdetalle.GetValue("Cantidad")) * (porcdesc / 100))
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                    grdetalle.SetValue("MontoDescuento", montodesc)
                    Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")
                    Dim rowIndex01 As Integer = grdetalle.Row
                    P_PonerTotal(rowIndex01)
                    If (estado = 1) Then
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If




                End If
            End If
        End If


        If (e.Column.Index = grdetalle.RootTable.Columns("Precio").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("Precio")) Or grdetalle.GetValue("Precio").ToString = String.Empty) Then

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Precio") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = 0
                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")

                grdetalle.SetValue("Precio", 0)
                grdetalle.SetValue("ProcentajeDescuento", 0)
                grdetalle.SetValue("MontoDescuento", 0)
                grdetalle.SetValue("SubTotal", 0)
                grdetalle.SetValue("Total", 0)



                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else

                Dim porcdesc As Double = grdetalle.GetValue("ProcentajeDescuento")
                Dim montodesc As Double = ((grdetalle.GetValue("Precio") * grdetalle.GetValue("Cantidad")) * (porcdesc / 100))
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Precio") = grdetalle.GetValue("Precio")
                grdetalle.SetValue("MontoDescuento", montodesc)
                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")
                Dim rowIndex01 As Integer = grdetalle.Row
                P_PonerTotal(rowIndex01)
                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If
            End If
        End If
        If (e.Column.Index = grdetalle.RootTable.Columns("ProcentajeDescuento").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("ProcentajeDescuento")) Or grdetalle.GetValue("ProcentajeDescuento").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                lin = grdetalle.GetValue("Id")
                pos = -1
                _fnObtenerFilaDetalle(pos, lin, grdetalle.GetValue("Tipo"))
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")

                grdetalle.SetValue("ProcentajeDescuento", 0)
                grdetalle.SetValue("MontoDescuento", 0)
                'grdetalle.SetValue("tbcmin", 1)
                'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))
            Else
                If (grdetalle.GetValue("ProcentajeDescuento") > 0 And grdetalle.GetValue("ProcentajeDescuento") <= 100) Then

                    Dim porcdesc As Double = grdetalle.GetValue("ProcentajeDescuento")
                    Dim montodesc As Double = (grdetalle.GetValue("SubTotal") * (porcdesc / 100))
                    lin = grdetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin, grdetalle.GetValue("Tipo"))
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                    grdetalle.SetValue("MontoDescuento", montodesc)


                Else
                    lin = grdetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin, grdetalle.GetValue("Tipo"))
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                    grdetalle.SetValue("ProcentajeDescuento", 0)
                    grdetalle.SetValue("MontoDescuento", 0)
                    grdetalle.SetValue("Total", grdetalle.GetValue("SubTotal"))

                    'grdetalle.SetValue("tbcmin", 1)
                    'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))

                End If
            End If
        End If


        '''''''''''''''''''''MONTO DE DESCUENTO '''''''''''''''''''''
        If (e.Column.Index = grdetalle.RootTable.Columns("MontoDescuento").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("MontoDescuento")) Or grdetalle.GetValue("MontoDescuento").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                lin = grdetalle.GetValue("Id")
                pos = -1
                _fnObtenerFilaDetalle(pos, lin, grdetalle.GetValue("Tipo"))
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                grdetalle.SetValue("ProcentajeDescuento", 0)
                grdetalle.SetValue("MontoDescuento", 0)
                'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))
            Else
                If (grdetalle.GetValue("MontoDescuento") > 0 And grdetalle.GetValue("MontoDescuento") <= grdetalle.GetValue("SubTotal")) Then

                    Dim montodesc As Double = grdetalle.GetValue("MontoDescuento")
                    Dim pordesc As Double = ((montodesc * 100) / grdetalle.GetValue("SubTotal"))

                    lin = grdetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin, grdetalle.GetValue("Tipo"))
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = pordesc

                    grdetalle.SetValue("ProcentajeDescuento", pordesc)


                Else
                    lin = grdetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin, grdetalle.GetValue("Tipo"))
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                    grdetalle.SetValue("ProcentajeDescuento", 0)
                    grdetalle.SetValue("MontoDescuento", 0)
                    grdetalle.SetValue("Total", grdetalle.GetValue("SubTotal"))

                    'grdetalle.SetValue("tbcmin", 1)
                    'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))

                End If
            End If
        End If
        Dim rowIndex As Integer = grdetalle.Row
        P_PonerTotal(rowIndex)

        _prCalcularPrecioTotal()

    End Sub

    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grdetalle.EditingCell
        Try
            If (grdetalle.GetValue("Tipo") = 2) Then
                e.Cancel = True
                Return

            End If

            If (e.Column.Index = grdetalle.RootTable.Columns("Cantidad").Index) Then
                e.Cancel = False
                Return
            Else
                If (Global_ModificarDescuento = 1 And (
                    e.Column.Index = grdetalle.RootTable.Columns("ProcentajeDescuento").Index Or
                     e.Column.Index = grdetalle.RootTable.Columns("MontoDescuento").Index)) Then


                    e.Cancel = False
                    Return

                End If
                If (Global_ModificarPrecio = 1 And e.Column.Index = grdetalle.RootTable.Columns("Precio").Index) Then

                    e.Cancel = False
                    Return

                End If

                e.Cancel = True


            End If
        Catch ex As Exception

        End Try







    End Sub

    Private Sub EliminarProductoMenu_Click(sender As Object, e As EventArgs) Handles EliminarProductoMenu.Click
        _prEliminarFila()

    End Sub
End Class