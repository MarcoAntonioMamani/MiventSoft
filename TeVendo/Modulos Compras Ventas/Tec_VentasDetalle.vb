
Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO

Public Class Tec_VentasDetalle

    Dim FilaSelectLote As DataRow = Nothing
    Public dtProductos As DataTable
    Public dtDetalle As DataTable
    Public Lote As Boolean
    Public VentaDirecta As Integer = 0
    Public TipoMovimientoId As Integer
    Public SucursalId As Integer
    Public IdCliente As Integer
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public DistribuidorId As Integer = 0



    Public Sub IniciarTodod()

        P_Global._prCargarComboGenerico(cbPrecios, L_prListaCategoriasPrecios(), "Id", "Codigo", "Descripcion", "CategoriaPrecio")
        CargarProductosVentas()

        _prCargarProductos()

        _habilitarFocus()
        ActualizarProductos()
        tbProducto.Focus()


    End Sub
    Private Sub Tec_DespachoDetalle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarTodod()
        tbProducto.Focus()
    End Sub

    Private Sub _prCargarProductos()
        Dim dt As New DataTable
        btnProductos.Visible = False

        If (SucursalId < 0 Or IdCliente = 0) Then
            If (Not IsNothing(grProducto.DataSource)) Then
                CType(grProducto.DataSource, DataTable).Rows.Clear()
            End If
            Return

        End If

        If (VentaDirecta = 1) Then
            dt = L_prListarProductosVentasConciliaciones(SucursalId, IdCliente, DistribuidorId, cbPrecios.Value)  ''1=Almacen
        Else
            dt = L_prListarProductosVentas(SucursalId, IdCliente, cbPrecios.Value)  ''1=Almacen
        End If



        dtProductos = dt.Copy
        'a.Id , a.NombreProducto, PCosto.Precio As PrecioCosto,
        ''PVenta.Precio as PrecioVenta
        grProducto.DataSource = dt
        grProducto.RetrieveStructure()
        grProducto.AlternatingColors = True
        With grProducto.RootTable.Columns("Id")
            .Width = 100
            .Caption = "Id"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center

        End With
        With grProducto.RootTable.Columns("CodigoExterno")
            .Width = 100
            .Caption = "CODIGOP"
            .Visible = False

        End With
        With grProducto.RootTable.Columns("Conversion")
            .Width = 100
            .Visible = True
            .FormatString = "0.00"
        End With

        With grProducto.RootTable.Columns("estado")
            .Width = 100
            .Visible = False

        End With
        With grProducto.RootTable.Columns("NombreProducto")
            .Width = 350
            .Caption = "PRODUCTOS"
            .Visible = True
            .MaxLines = 2
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With

        With grProducto.RootTable.Columns("DescripcionProducto")
            .Width = 250
            .Visible = True
            .MaxLines = 2
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
            .Caption = "DESCRIPCION"
        End With
        With grProducto.RootTable.Columns("NombreCategoria")
            .Width = 150
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .MaxLines = 2
            .WordWrap = True
            .Caption = "CATEGORIA"
        End With


        With grProducto.RootTable.Columns("PrecioCosto")
            .Width = 150
            .Visible = False
            .FormatString = "0.00"
        End With
        With grProducto.RootTable.Columns("stock")
            .Width = 150
            .Visible = True
            .Caption = "Stock Unitario"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .FormatString = "0.00"
        End With
        With grProducto.RootTable.Columns("stockCajas")
            .Width = 150
            .Visible = True
            .Caption = "Stock Cajas"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .FormatString = "0.00"
        End With

        With grProducto.RootTable.Columns("PrecioVenta")
            .Width = 150
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Caption = "Precio Cajas"
            .FormatString = "0.00"
        End With
        With grProducto
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
        fc = New GridEXFormatCondition(grProducto.RootTable.Columns("stock"), ConditionOperator.Equal, 0)
        'fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.White
        fc.FormatStyle.BackColor = Color.Red
        grProducto.RootTable.FormatConditions.Add(fc)
    End Sub
    Public Sub CambiarEstado(ProductoId As Integer, Estado As Integer)
        If (IsNothing(grProducto.DataSource)) Then
            Return

        End If
        For i As Integer = 0 To CType(grProducto.DataSource, DataTable).Rows.Count - 1 Step 1
            If (CType(grProducto.DataSource, DataTable).Rows(i).Item("Id") = ProductoId) Then
                CType(grProducto.DataSource, DataTable).Rows(i).Item("estado") = Estado
            End If

        Next
        grProducto.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grProducto.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.Equal, 1))
    End Sub
    Public Sub ActualizarProductos()

        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            If (CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado") >= 0) Then
                CambiarEstado(CType(grDetalle.DataSource, DataTable).Rows(i).Item("ProductoId"), 0)

            End If


        Next
        grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
    End Sub



    Private Sub _prAddDetalleVenta()
        '     a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        '     a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '     1 As estado, cast('' as image ) as img
        '     , 0 as stock
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.rowdelete, 25, 18)
        img.Save(Bin, Imaging.ImageFormat.Png)
        CType(grDetalle.DataSource, DataTable).Rows.Add(_GenerarId() + 1, 0, 0, "", 0, 0, 0, 0, 0, 0, 0, "", 0, "20200101", CDate("2020/01/01"), 0, Bin.GetBuffer, 0, 0)
    End Sub

    Public Function _GenerarId()
        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(grDetalle.DataSource, DataTable).Rows(i).Item("Id")), 0, CType(grDetalle.DataSource, DataTable).Rows(i).Item("Id"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbProducto, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnConfirmarSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        End With
    End Sub
    Public Sub New(dtv As DataTable)
        InitializeComponent()

        dtDetalle = dtv

    End Sub
    Public Sub CargarProductosVentas()
        Dim bandera As Boolean = False


        grDetalle.DataSource = dtDetalle
        grDetalle.RetrieveStructure()
        grDetalle.AlternatingColors = True
        With grDetalle.RootTable.Columns("Id")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("VentaId")
            .Width = 100
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("Conversion")
            .Width = 100
            .Visible = True
            .FormatString = "0.00"

        End With


        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 30
            .Visible = True
            .Caption = "Cod Producto"
        End With

        With grDetalle.RootTable.Columns("Producto")
            .Width = 150
            .Caption = "Producto"
            .Visible = True
        End With

        With grDetalle.RootTable.Columns("Cantidad")
            .Width = 50
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
            .FormatString = "0.0000"
            .Caption = "Cant.Caja".ToUpper
        End With
        With grDetalle.RootTable.Columns("CantidadUnitaria")
            .Width = 50
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
            .FormatString = "0.0000"
            .Caption = "Cant.Uni".ToUpper
        End With


        With grDetalle.RootTable.Columns("Precio")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "Precio Unitario"
            .FormatString = "0.0000"
        End With

        With grDetalle.RootTable.Columns("SubTotal")
            .Width = 60
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "SubTotal"
            .FormatString = "0.0000"
        End With
        '     a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        '     a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '     1 As estado, cast('' as image ) as img
        '     ,   as stock

        With grDetalle.RootTable.Columns("ProcentajeDescuento")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0"
            .Caption = "P.Descuento".ToUpper
        End With

        With grDetalle.RootTable.Columns("MontoDescuento")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "M.Descuento".ToUpper
        End With


        With grDetalle.RootTable.Columns("Total")
            .Width = 60
            .Visible = True
            .Caption = "Total".ToUpper
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.0000"
            .AggregateFunction = AggregateFunction.Sum
        End With

        With grDetalle.RootTable.Columns("Detalle")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("PrecioCosto")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.0000"
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("stock")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .Visible = False
        End With


        With grDetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("img")
                .Width = 80
                .Caption = "Eliminar".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With


            If (Lote = True) Then
            With grDetalle.RootTable.Columns("Lote")
                .Width = 60
                .Caption = "lote".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 70
                .Caption = "FECHA VENC.".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = True
            End With
        Else

            With grDetalle.RootTable.Columns("Lote")
                .Width = 120
                .Caption = "lote".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = False
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 120
                .Caption = "FECHA VENC.".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = False
            End With


        End If

        With grDetalle
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

        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.rowdelete, 25, 18)
        img.Save(Bin, Imaging.ImageFormat.Png)
        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1
            CType(grDetalle.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer
        Next


    End Sub
    Private Sub grProductos_DoubleClick(sender As Object, e As EventArgs) Handles grProducto.DoubleClick

        Try
            Dim f, c As Integer
            c = grProducto.Col
            f = grProducto.Row
            If (f >= 0) Then

                seleccionarProducto()

            End If
        Catch ex As Exception

        End Try

    End Sub
    Private Sub grProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles grProducto.KeyDown
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grProducto.Col
            f = grProducto.Row
            If (f >= 0) Then

                seleccionarProducto()

            End If
        End If
        If e.KeyData = Keys.Escape Then

            If (IsNothing(FilaSelectLote)) Then
                CType(grProducto.DataSource, DataTable).Rows.Clear()

                _DesHabilitarProductos()
            Else
                btnProductos.Visible = False
                FilaSelectLote = Nothing
                _HabilitarProductos()



            End If


        End If
    End Sub
    Private Sub _HabilitarProductos()

        _prCargarProductos()
        tbProducto.Focus()
    End Sub

    Private Sub _DesHabilitarProductos()
        tbProducto.Clear()
        tbProducto.Focus()
    End Sub
    Public Function _fnExisteProducto(idprod As Integer) As Boolean
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado")
            If (_idprod = idprod And estado >= 0) Then

                Return True
            End If
        Next
        Return False
    End Function

    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub

    Public Sub _fnObtenerFilaProducto(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grProducto.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grProducto.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub

    Public Sub actualizarSaldo(ByRef dt As DataTable, CodProducto As Integer)
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 

        '      a.tbnumi ,a.tbtv1numi ,a.tbty5prod ,b.yfcdprod1 as producto,a.tbest ,a.tbcmin ,a.tbumin ,Umin .ycdes3 as unidad,a.tbpbas ,a.tbptot ,a.tbobs ,
        'a.tbpcos,a.tblote ,a.tbfechaVenc , a.tbptot2, a.tbfact ,a.tbhact ,a.tbuact,1 as estado,Cast(null as Image) as img,
        'Cast (0 as decimal (18,2)) as stock
        Dim _detalle As DataTable = CType(grDetalle.DataSource, DataTable)

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
        If (SucursalId < 0) Then
            Return
        End If
        btnProductos.Visible = True
        'p.NombreProducto , a.Lote, a.FechaVencimiento, Sum(a.Cantidad) as stock
        Dim dt As New DataTable

        dt = LotesPorProducto(SucursalId, CodProducto)  ''1=Almacen
        actualizarSaldo(dt, CodProducto)
        grProducto.DataSource = dt
        grProducto.RetrieveStructure()
        grProducto.AlternatingColors = True
        With grProducto.RootTable.Columns("NombreProducto")
            .Width = 150
            .Visible = False

        End With
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 
        With grProducto.RootTable.Columns("Lote")
            .Width = 150
            .Caption = "Lote"
            .Visible = True

        End With
        With grProducto.RootTable.Columns("FechaVencimiento")
            .Width = 160
            .Caption = "Fecha Vencimiento"
            .FormatString = "yyyy/MM/dd"
            .Visible = True

        End With

        With grProducto.RootTable.Columns("stock")
            .Width = 150
            .Visible = True
            .Caption = "Stock"
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With


        With grProducto
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
    Public Sub _prAplicarCondiccionJanusLote()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProducto.RootTable.Columns("stock"), ConditionOperator.Equal, 0)
        fc.FormatStyle.BackColor = Color.Gold
        fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.White
        grProducto.RootTable.FormatConditions.Add(fc)

        Dim fc2 As GridEXFormatCondition
        fc2 = New GridEXFormatCondition(grProducto.RootTable.Columns("FechaVencimiento"), ConditionOperator.LessThanOrEqualTo, Now.Date)
        fc2.FormatStyle.BackColor = Color.Red
        fc2.FormatStyle.FontBold = TriState.True
        fc2.FormatStyle.ForeColor = Color.White
        grProducto.RootTable.FormatConditions.Add(fc2)

        grProducto.Select()
        grProducto.Col = 1
        grProducto.Row = grProducto.RowCount - 1
    End Sub


    Public Function _fnExisteProductoConLote(idprod As Integer, lote As String, fechaVenci As Date) As Boolean
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado")

            Dim _LoteDetalle As String = CType(grDetalle.DataSource, DataTable).Rows(i).Item("Lote")
            Dim _FechaVencDetalle As Date = CType(grDetalle.DataSource, DataTable).Rows(i).Item("FechaVencimiento")
            If (_idprod = idprod And estado >= 0 And lote = _LoteDetalle And fechaVenci = _FechaVencDetalle) Then

                Return True
            End If
        Next
        Return False
    End Function
    Public Sub InsertarProductosConLote()

        If (grDetalle.Row < 0) Then
            _prAddDetalleVenta()
        End If

        '      a.Id ,a.icibid ,a.iccprod ,b.yfcdprod1  as producto,a.Cantidad ,
        'a.iclot ,a.icfvenc ,Cast(null as image ) as img,1 as estado,
        '(Sum(inv.iccven )+a.Cantidad  ) as stock

        'a.yfnumi  ,a.yfcdprod1  ,a.yfcdprod2,Sum(b.iccven ) as stock 
        Dim pos As Integer = -1
        grDetalle.Row = grDetalle.RowCount - 1
        _fnObtenerFilaDetalle(pos, grDetalle.GetValue("Id"))
        Dim posProducto As Integer = 0
        _fnObtenerFilaProducto(posProducto, grProducto.GetValue("Id"))


        FilaSelectLote = CType(grProducto.DataSource, DataTable).Rows(posProducto)


        If (grProducto.GetValue("stock") > 0) Then
            _prCargarLotesDeProductos(grProducto.GetValue("Id"), grProducto.GetValue("NombreProducto"))
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "El Producto: ".ToUpper + grProducto.GetValue("NombreProducto") + " NO CUENTA CON STOCK DISPONIBLE", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            FilaSelectLote = Nothing
        End If

    End Sub

    Public Sub InsertarProductosSinLote(cantidad As Double)
        'a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        'a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '1 As estado, cast('' as image ) as img
        ', 0 as stock
        Dim pos As Integer = -1
        If (grDetalle.Row < 0) Then
            _prAddDetalleVenta()
        End If
        Dim existe As Boolean = _fnExisteProducto(grProducto.GetValue("Id"))
        If ((Not existe)) Then
            If (grDetalle.GetValue("ProductoId") > 0) Then
                _prAddDetalleVenta()
            End If
            grDetalle.Row = grDetalle.RowCount - 1
            _fnObtenerFilaDetalle(pos, grDetalle.GetValue("Id"))
            If ((pos >= 0)) Then
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = grProducto.GetValue("Id")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Producto") = grProducto.GetValue("NombreProducto")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = cantidad
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadUnitaria") = cantidad * grProducto.GetValue("Conversion")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion") = grProducto.GetValue("Conversion")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio") = grProducto.GetValue("PrecioVenta") / grProducto.GetValue("Conversion")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = grProducto.GetValue("PrecioVenta") * cantidad

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = grProducto.GetValue("PrecioVenta") * cantidad
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = grProducto.GetValue("PrecioCosto") / grProducto.GetValue("Conversion")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProducto.GetValue("Stock")

                CambiarEstado(grProducto.GetValue("Id"), 0)
                'grproducto.RemoveFilters()
                tbProducto.Clear()
                tbProducto.Focus()
            End If
        Else
            If (existe) Then
                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grProducto.RemoveFilters()
                tbProducto.Focus()
            End If
        End If


    End Sub
    Public Sub seleccionarProducto()
        If (IsNothing(FilaSelectLote)) Then


            ''''''''''''''''''''''''
            If (grProducto.GetValue("stock") <= 0) Then

                ToastNotification.Show(Me, "No Existe Stock del Producto Para Proceder con la Venta".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
                Return

            End If

            If (Lote = True) Then
                InsertarProductosConLote()
            Else
                Dim ef = New Efecto
                ef.tipo = 22
                ef.NombreProducto = grProducto.GetValue("NombreProducto")
                ef.StockActual = grProducto.GetValue("stock")
                ef.Conversion = grProducto.GetValue("Conversion")
                ef.TipoMovimiento = 3
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    InsertarProductosSinLote(ef.CantidadTransaccion)

                End If
            End If


        Else

            Dim numiProd As Integer = FilaSelectLote.Item("Id")
            Dim mLote As String = grProducto.GetValue("Lote")
            Dim FechaVenc As Date = grProducto.GetValue("FechaVencimiento")
            If (Not _fnExisteProductoConLote(numiProd, Lote, FechaVenc)) Then
                Dim ef = New Efecto
                ef.tipo = 5
                ef.NombreProducto = grProducto.GetValue("NombreProducto")
                ef.StockActual = grProducto.GetValue("stock")
                ef.TipoMovimiento = 3
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                Dim CantidadVenta As Double = 0
                If (bandera = True) Then
                    CantidadVenta = ef.CantidadTransaccion
                    If (grDetalle.GetValue("ProductoId") > 0) Then
                        _prAddDetalleVenta()
                    End If
                    Dim pos As Integer = -1
                    grDetalle.Row = grDetalle.RowCount - 1
                    _fnObtenerFilaDetalle(pos, grDetalle.GetValue("Id"))

                    If ((pos >= 0)) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = numiProd
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Producto") = FilaSelectLote.Item("NombreProducto")
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = CantidadVenta
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio") = FilaSelectLote.Item("PrecioVenta")
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("conversion") = FilaSelectLote.Item("conversion")
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = FilaSelectLote.Item("PrecioVenta") * CantidadVenta

                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = FilaSelectLote.Item("PrecioVenta") * CantidadVenta
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = FilaSelectLote.Item("PrecioCosto")
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProducto.GetValue("Stock")
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Lote") = mLote
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("FechaVencimiento") = FechaVenc

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

                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El producto con el lote ya existe modifique su cantidad".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If


        End If
    End Sub
    Public Function _fnExisteProductoLote(idprod As Integer, mLote As String, mFecha As Date) As Boolean
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado")
            Dim _Lote As String = CType(grDetalle.DataSource, DataTable).Rows(i).Item("Lote")
            Dim _FechaVenc As Date = CType(grDetalle.DataSource, DataTable).Rows(i).Item("FechaVencimiento")
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
        If (grDetalle.Row < 0) Then
            _prAddDetalleVenta()
        End If



        Dim existe As Boolean = _fnExisteProductoLote(grProducto.GetValue("Id"), mLote, mFechaVen)
        If ((Not existe)) Then
            If (grDetalle.GetValue("ProductoId") > 0) Then
                _prAddDetalleVenta()
            End If
            grDetalle.Row = grDetalle.RowCount - 1
            _fnObtenerFilaDetalle(pos, grDetalle.GetValue("Id"))
            If ((pos >= 0)) Then
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = grProducto.GetValue("Id")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Producto") = grProducto.GetValue("NombreProducto")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = grProducto.GetValue("PrecioCosto")

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioVenta") = grProducto.GetValue("PrecioVenta")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("costo") = grProducto.GetValue("PrecioCosto")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("venta") = grProducto.GetValue("PrecioVenta")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = cantidad
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Lote") = mLote
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("FechaVencimiento") = mFechaVen
                ''    _DesHabilitarProductos()


                CambiarEstado(grProducto.GetValue("Id"), 0)
                'grproducto.RemoveFilters()
                tbProducto.Clear()
                tbProducto.Focus()
            End If




        Else
            If (existe) Then

                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grProducto.RemoveFilters()
                tbProducto.Focus()
            End If

        End If


    End Sub


    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grDetalle.CellEdited
        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("Cantidad")) Or grDetalle.GetValue("Cantidad").ToString = String.Empty) Then

                grDetalle.SetValue("Cantidad", 1)
            Else
                If (grDetalle.GetValue("Cantidad") > 0) Then


                Else

                    grDetalle.SetValue("Cantidad", 1)

                End If
            End If
        End If
    End Sub
    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grDetalle.EditingCell


        'Habilitar solo las columnas de Precio, %, Monto y Observación
        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index Or e.Column.Index = grDetalle.RootTable.Columns("CantidadUnitaria").Index Or
                e.Column.Index = grDetalle.RootTable.Columns("ProcentajeDescuento").Index Or
                 e.Column.Index = grDetalle.RootTable.Columns("MontoDescuento").Index) Then
            e.Cancel = False
        Else
            If ((e.Column.Index = grDetalle.RootTable.Columns("Lote").Index Or
                    e.Column.Index = grDetalle.RootTable.Columns("FechaVencimiento").Index) And
                Lote = True) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If

        End If



    End Sub

    Private Sub grdetalle_KeyDown(sender As Object, e As KeyEventArgs) Handles grDetalle.KeyDown




        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grDetalle.Col
            f = grDetalle.Row

            If (grDetalle.Col = grDetalle.RootTable.Columns("Cantidad").Index) Then
                If (grDetalle.GetValue("Producto") <> String.Empty) Then

                    tbProducto.Focus()
                Else
                    ToastNotification.Show(Me, "Seleccione Producto", img, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
            If (grDetalle.Col = grDetalle.RootTable.Columns("Producto").Index) Then
                If (grDetalle.GetValue("Producto") <> String.Empty) Then

                    tbProducto.Focus()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto", img, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
salirIf:
        End If

        If (e.KeyData = Keys.Control + Keys.Enter And grDetalle.Row >= 0 And
            grDetalle.Col = grDetalle.RootTable.Columns("Producto").Index) Then
            Dim indexfil As Integer = grDetalle.Row
            Dim indexcol As Integer = grDetalle.Col
            tbProducto.Focus()

        End If
        If (e.KeyData = Keys.Escape And grDetalle.Row >= 0) Then

            _prEliminarFila()


        End If




    End Sub
    Private Sub tbProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles tbProducto.KeyDown
        If e.KeyData = Keys.Escape Then
            Me.Close()

        End If
        If e.KeyData = Keys.Down Then
            grProducto.Focus()
        End If
        If e.KeyData = Keys.Enter Then
            If grProducto.RowCount > 0 Then

                grProducto.Row = 0
                seleccionarProducto()
            End If

        End If
    End Sub

    Private Sub tbProducto_TextChanged(sender As Object, e As EventArgs) Handles tbProducto.TextChanged
        Dim dtProductoCopy As DataTable
        dtProductoCopy = dtProductos.Copy
        dtProductoCopy.Rows.Clear()
        Dim dt As DataTable = dtProductos.Copy

        Dim charSequence As String
        charSequence = tbProducto.Text.ToUpper
        If (charSequence.Trim <> String.Empty) Then
            Dim cantidad As Integer = 12
            Dim cont As Integer = 12

            'Split con array de delimitadores
            Dim delimitadores() As String = {" ", ".", ",", ";", "-"}
            Dim vectoraux() As String
            vectoraux = charSequence.Split(delimitadores, StringSplitOptions.None)

            'mostrar resultado
            'For Each item As String In vectoraux


            '    Console.WriteLine("'{0}'", item)
            'Next
            Dim cant As Integer = vectoraux.Length
            'p.Id , p.CodigoExterno, p.NombreProducto, p.DescripcionProducto, Sum(stock.Cantidad) as stock  NombreCategoria
            For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                Dim nombre As String = dt.Rows(i).Item("Id").ToString.ToUpper +
                    " " + dt.Rows(i).Item("NombreProducto").ToString.ToUpper +
                    " " + dt.Rows(i).Item("DescripcionProducto").ToString.ToUpper +
                    " " + dt.Rows(i).Item("NombreCategoria").ToString.ToUpper
                Select Case cant
                    Case 1

                        If (nombre.Trim.Contains(vectoraux(0))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 2
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 3
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 4
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 5
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 6
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 7

                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 8
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 9
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 10
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 11
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 12
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If


                    Case 13
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 14
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 15
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 16
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 17
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 18
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16)) And nombre.Trim.Contains(vectoraux(17))) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If



                End Select

            Next
            grProducto.DataSource = dtProductoCopy.Copy
        Else
            grProducto.DataSource = dtProductos.Copy
        End If



    End Sub
    Public Sub _prEliminarFila()
        If (grDetalle.Row >= 0) Then
            If (grDetalle.RowCount >= 1) Then
                Dim estado As Integer = grDetalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grDetalle.GetValue("Id")
                _fnObtenerFilaDetalle(pos, lin)

                CambiarEstado(grDetalle.GetValue("ProductoId"), 1)
                If (estado = 0) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = -2

                End If
                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = -1
                End If

                grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))


            End If
        End If


    End Sub

    Private Sub grdetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grDetalle.MouseClick

        Try
            If (grDetalle.RowCount >= 1) Then
                If (grDetalle.CurrentColumn.Index = grDetalle.RootTable.Columns("img").Index) Then
                    _prEliminarFila()
                End If
            End If

        Catch ex As Exception

        End Try



    End Sub
    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grDetalle.CellValueChanged
        Dim lin As Integer = grDetalle.GetValue("Id")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin)
        If (e.Column.Index = grDetalle.RootTable.Columns("CantidadUnitaria").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("CantidadUnitaria")) Or grDetalle.GetValue("Cantidad").ToString = String.Empty) Then
                grDetalle.SetValue("CantidadUnitaria", 1)
                grDetalle.SetValue("Cantidad", grDetalle.GetValue("CantidadUnitaria") / grDetalle.GetValue("Conversion"))

            Else
                If (grDetalle.GetValue("CantidadUnitaria") <= grDetalle.GetValue("Stock")) Then
                    grDetalle.SetValue("Cantidad", grDetalle.GetValue("CantidadUnitaria") / grDetalle.GetValue("Conversion"))

                Else
                    ToastNotification.Show(Me, "La Cantidad = " + Str(grDetalle.GetValue("CantidadUnitaria")) + " es mayor al Stock del Producto = " + Str(grDetalle.GetValue("Stock")))
                    grDetalle.SetValue("CantidadUnitaria", 1)
                    grDetalle.SetValue("Cantidad", grDetalle.GetValue("CantidadUnitaria") / grDetalle.GetValue("Conversion"))
                End If


            End If


        End If

        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("Cantidad")) Or grDetalle.GetValue("Cantidad").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadUnitaria") = 1
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1 / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio")
                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                grDetalle.SetValue("CantidadUnitaria", 1)
                grDetalle.SetValue("Cantidad", 1 / grDetalle.GetValue("conversion"))
                'grDetalle.SetValue("CantidadUnitaria", 1 * grDetalle.GetValue("Conversion"))
                grDetalle.SetValue("ProcentajeDescuento", 0)
                grDetalle.SetValue("MontoDescuento", 0)
                grDetalle.SetValue("SubTotal", grDetalle.GetValue("Precio"))
                grDetalle.SetValue("Total", grDetalle.GetValue("Precio"))



                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grDetalle.GetValue("Cantidad") > 0) Then

                    If (grDetalle.GetValue("Cantidad") * grDetalle.GetValue("Conversion") <= grDetalle.GetValue("Stock")) Then

                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadUnitaria") = grDetalle.GetValue("Cantidad") * grDetalle.GetValue("Conversion")

                        grDetalle.SetValue("CantidadUnitaria", grDetalle.GetValue("Cantidad") * grDetalle.GetValue("Conversion"))

                        Dim porcdesc As Double = grDetalle.GetValue("ProcentajeDescuento")
                        Dim montodesc As Double = ((grDetalle.GetValue("Precio") * grDetalle.GetValue("CantidadUnitaria")) * (porcdesc / 100))
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                        grDetalle.SetValue("MontoDescuento", montodesc)
                        Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")
                        Dim rowIndex01 As Integer = grDetalle.Row
                        P_PonerTotal(rowIndex01)
                        If (estado = 1) Then
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                        End If

                    Else


                        ToastNotification.Show(Me, "La Cantidad = " + Str(grDetalle.GetValue("Cantidad") * grDetalle.GetValue("Conversion")) + " es mayor al Stock del Producto = " + Str(grDetalle.GetValue("Stock")), img, 6000, eToastGlowColor.Red, eToastPosition.TopCenter)
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio")
                        Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadUnitaria") = 1 * CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion")

                        grDetalle.SetValue("Cantidad", 1 / grDetalle.GetValue("conversion"))
                        grDetalle.SetValue("CantidadUnitaria", 1)
                        grDetalle.SetValue("ProcentajeDescuento", 0)
                        grDetalle.SetValue("MontoDescuento", 0)
                        grDetalle.SetValue("SubTotal", grDetalle.GetValue("Precio"))
                        grDetalle.SetValue("Total", grDetalle.GetValue("Precio"))



                        If (estado = 1) Then
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                        End If

                    End If


                Else

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadUnitaria") = 1 * CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion")
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio")
                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    grDetalle.SetValue("CantidadUnitaria", 1)
                    grDetalle.SetValue("Cantidad", 1 / grDetalle.GetValue("Conversion"))
                    grDetalle.SetValue("ProcentajeDescuento", 0)
                    grDetalle.SetValue("MontoDescuento", 0)
                    grDetalle.SetValue("SubTotal", grDetalle.GetValue("Precio"))
                    grDetalle.SetValue("Total", grDetalle.GetValue("Precio"))
                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If
        End If

        If (e.Column.Index = grDetalle.RootTable.Columns("ProcentajeDescuento").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("ProcentajeDescuento")) Or grDetalle.GetValue("ProcentajeDescuento").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                lin = grDetalle.GetValue("Id")
                pos = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                'grdetalle.SetValue("tbcmin", 1)
                'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))
            Else
                If (grDetalle.GetValue("ProcentajeDescuento") > 0 And grDetalle.GetValue("ProcentajeDescuento") <= 100) Then

                    Dim porcdesc As Double = grDetalle.GetValue("ProcentajeDescuento")
                    Dim montodesc As Double = (grDetalle.GetValue("SubTotal") * (porcdesc / 100))
                    lin = grDetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                    grDetalle.SetValue("MontoDescuento", montodesc)


                Else
                    lin = grDetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                    grDetalle.SetValue("ProcentajeDescuento", 0)
                    grDetalle.SetValue("MontoDescuento", 0)
                    grDetalle.SetValue("Total", grDetalle.GetValue("SubTotal"))

                    'grdetalle.SetValue("tbcmin", 1)
                    'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))

                End If
            End If
        End If


        '''''''''''''''''''''MONTO DE DESCUENTO '''''''''''''''''''''
        If (e.Column.Index = grDetalle.RootTable.Columns("MontoDescuento").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("MontoDescuento")) Or grDetalle.GetValue("MontoDescuento").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                lin = grDetalle.GetValue("Id")
                pos = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                'grdetalle.SetValue("tbcmin", 1)
                'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))
            Else
                If (grDetalle.GetValue("MontoDescuento") > 0 And grDetalle.GetValue("MontoDescuento") <= grDetalle.GetValue("SubTotal")) Then

                    Dim montodesc As Double = grDetalle.GetValue("MontoDescuento")
                    Dim pordesc As Double = ((montodesc * 100) / grDetalle.GetValue("SubTotal"))

                    lin = grDetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = pordesc

                    grDetalle.SetValue("ProcentajeDescuento", pordesc)


                Else
                    lin = grDetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                    grDetalle.SetValue("ProcentajeDescuento", 0)
                    grDetalle.SetValue("MontoDescuento", 0)
                    grDetalle.SetValue("Total", grDetalle.GetValue("SubTotal"))

                    'grdetalle.SetValue("tbcmin", 1)
                    'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))

                End If
            End If
        End If
        Dim rowIndex As Integer = grDetalle.Row
        P_PonerTotal(rowIndex)
    End Sub
    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < grDetalle.RowCount) Then

            Dim lin As Integer = grDetalle.GetValue("Id")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin)
            Dim cant As Double = grDetalle.GetValue("CantidadUnitaria")
            Dim uni As Double = grDetalle.GetValue("Precio")
            Dim MontoDesc As Double = grDetalle.GetValue("MontoDescuento")
            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni
                'grDetalle.SetValue("lcmdes", montodesc)

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = TotalUnitario
                grDetalle.SetValue("SubTotal", TotalUnitario)
                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")


                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = TotalUnitario - MontoDesc
                grDetalle.SetValue("Total", TotalUnitario - MontoDesc)
                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            End If

        End If




    End Sub
    Private Sub btnConfirmarSalir_Click(sender As Object, e As EventArgs) Handles btnConfirmarSalir.Click
        Me.Close()
    End Sub

    Private Sub btnProductos_Click(sender As Object, e As EventArgs) Handles btnProductos.Click
        btnProductos.Visible = False
        FilaSelectLote = Nothing
        _HabilitarProductos()
    End Sub

    Private Sub cbPrecios_ValueChanged(sender As Object, e As EventArgs) Handles cbPrecios.ValueChanged
        _prCargarProductos()
    End Sub
End Class