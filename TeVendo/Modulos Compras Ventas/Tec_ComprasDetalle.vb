
Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class Tec_ComprasDetalle

    Dim FilaSelectLote As DataRow = Nothing
    Public dtProductos As DataTable
    Public dtDetalle As DataTable
    Public Lote As Boolean

    Public TipoMovimientoId As Integer
    Public SucursalId As Integer

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public Sub IniciarTodod()
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


        dt = L_prListarProductosCompras(SucursalId)  ''1=Almacen
        dtProductos = dt
        'a.Id , a.NombreProducto, PCosto.Precio As PrecioCosto,
        ''PVenta.Precio as PrecioVenta
        grProducto.DataSource = dtProductos
        grProducto.RetrieveStructure()
        grProducto.AlternatingColors = True
        With grProducto.RootTable.Columns("Id")
            .Width = 100
            .Caption = "Id"
            .Visible = False


        End With
        With grProducto.RootTable.Columns("CodigoExterno")
            .Width = 100
            .Caption = "Cod Externo"
            .Visible = True

        End With

        With grProducto.RootTable.Columns("estado")
            .Width = 100
            .Visible = False

        End With
        With grProducto.RootTable.Columns("NombreProducto")
            .Width = 350
            .Caption = "PRODUCTOS"
            .Visible = False

        End With

        With grProducto.RootTable.Columns("DescripcionProducto")
            .Width = 350
            .Visible = True
            .WordWrap = True
            .MaxLines = 3
            .Caption = "DESCRIPCION"
        End With
        With grProducto.RootTable.Columns("NombreCategoria")
            .Width = 200
            .Visible = True
            .Caption = "CATEGORIA"
        End With
        With grProducto.RootTable.Columns("Marca")
            .Width = 200
            .Visible = True
            .Caption = "Marca"
        End With
        With grProducto.RootTable.Columns("PrecioCosto")
            .Width = 150
            .Visible = False
            .FormatString = "0.00"
        End With
        With grProducto.RootTable.Columns("stock")
            .Width = 150
            .Visible = True
            .Caption = "Stock"
            .FormatString = "0.00"
        End With
        With grProducto.RootTable.Columns("PrecioVenta")
            .Width = 150
            .Visible = False
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
        'd.Id , d.CompraId, d.ProductoId, p.NombreProducto As Producto, d.Cantidad, d.PrecioCosto,
        'd.Lote, d.FechaVencimiento, d.TotalCompra, d.PrecioVenta, 1 As estado, cast('' as image) as img,
        'd.PrecioCosto As costo, d.PrecioVenta  as venta
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.rowdelete, 25, 18)
        img.Save(Bin, Imaging.ImageFormat.Png)
        CType(grDetalle.DataSource, DataTable).Rows.Add(_GenerarId() + 1, 0, 0, "", 0, 0, 0, 0, 0, "20200101", CDate("01/01/2020"), 0, 0, 0, Bin.GetBuffer, 0, 0)
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

        With grDetalle.RootTable.Columns("venta")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
        End With

        With grDetalle.RootTable.Columns("costo")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
        End With

        With grDetalle.RootTable.Columns("PrecioVenta")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "P.Venta"
        End With

        With grDetalle.RootTable.Columns("PrecioCosto")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "P.Costo"
        End With


        With grDetalle.RootTable.Columns("TotalCompra")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "SubTotal"
        End With

        With grDetalle.RootTable.Columns("CompraId")
            .Width = 90
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 50
            .Visible = True
            .Caption = "Cod Producto"
        End With



        With grDetalle.RootTable.Columns("Producto")
            .Width = 150
            .Caption = "Producto"
            .Visible = True
        End With


        With grDetalle.RootTable.Columns("CantidadCompra")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad"
        End With
        With grDetalle.RootTable.Columns("PorcentajeIncremento")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "% Bonif."
        End With
        With grDetalle.RootTable.Columns("CantidadIncremento")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "Cant Bonif."
        End With
        With grDetalle.RootTable.Columns("Cantidad")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "Cant. Total"
        End With
        With grDetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With


        With grDetalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = True
            .LeftMargin = 7
            .TopMargin = 5
            .BottomMargin = 5
        End With




        If (Lote = True) Then
            With grDetalle.RootTable.Columns("Lote")
                .Width = 70
                .Caption = "Lote"
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 70
                .Caption = "Fecha Venc."
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "dd/MM/yyyy"
                .Visible = True
            End With
        Else

            With grDetalle.RootTable.Columns("Lote")
                .Width = 70
                .Caption = "Lote"
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = False
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 70
                .Caption = "Fecha Venc."
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "dd/MM/yyyy"
                .Visible = False
            End With


        End If

        With grDetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
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
            CType(grProducto.DataSource, DataTable).Rows.Clear()

            _DesHabilitarProductos()
        End If
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
    Public Sub InsertarProductosSinLote(cantidad As Double)
        'd.Id , d.CompraId, d.ProductoId, p.NombreProducto As Producto, d.Cantidad, d.PrecioCosto,
        'd.Lote, d.FechaVencimiento, d.TotalCompra, d.PrecioVenta, 1 As estado, cast('' as image) as img,
        'd.PrecioCosto As costo, d.PrecioVenta  as venta
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
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = grProducto.GetValue("PrecioCosto")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("TotalCompra") = grProducto.GetValue("PrecioCosto") * cantidad
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioVenta") = grProducto.GetValue("PrecioVenta")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("costo") = grProducto.GetValue("PrecioCosto")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("venta") = grProducto.GetValue("PrecioVenta")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = cantidad
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra") = cantidad
                ''    _DesHabilitarProductos()


                CambiarEstado(grProducto.GetValue("Id"), 0)
                'grproducto.RemoveFilters()
                '' tbProducto.Clear()
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


            If (Lote = False) Then
                Dim ef = New Efecto


                ef.tipo = 5
                ef.NombreProducto = grProducto.GetValue("NombreProducto")
                ef.StockActual = grProducto.GetValue("stock")
                ef.TipoMovimiento = 4
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    InsertarProductosSinLote(ef.CantidadTransaccion)

                End If

                '''''''''''''''

            Else
                Dim ef = New Efecto


                ef.tipo = 9
                ef.NombreProducto = grProducto.GetValue("NombreProducto")
                ef.StockActual = grProducto.GetValue("stock")
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    InsertarProductosConLote(ef.CantidadTransaccion, ef.Lote, ef.FechaVencimiento)

                End If

                '''''''''''''''
            End If

            '''''''''''''''


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
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("TotalCompra") = grProducto.GetValue("PrecioCosto") * cantidad
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioVenta") = grProducto.GetValue("PrecioVenta")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("costo") = grProducto.GetValue("PrecioCosto")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("venta") = grProducto.GetValue("PrecioVenta")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = cantidad

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra") = cantidad
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
        If (e.Column.Index = grDetalle.RootTable.Columns("CantidadCompra").Index Or
                e.Column.Index = grDetalle.RootTable.Columns("PorcentajeIncremento").Index Or
                e.Column.Index = grDetalle.RootTable.Columns("CantidadIncremento").Index Or
                e.Column.Index = grDetalle.RootTable.Columns("PrecioCosto").Index Or
                 e.Column.Index = grDetalle.RootTable.Columns("PrecioVenta").Index) Then
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
                    " " + dt.Rows(i).Item("CodigoExterno").ToString.ToUpper +
                    " " + dt.Rows(i).Item("DescripcionProducto").ToString.ToUpper +
                    " " + dt.Rows(i).Item("NombreCategoria").ToString.ToUpper +
                    " " + dt.Rows(i).Item("Marca").ToString.ToUpper
                Select Case cant
                    Case 1

                        If (nombre.Trim.Contains(vectoraux(0)) And dt.Rows(i).Item("estado") = 1) Then

                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 2
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 3
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 4
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 5
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 6
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 7

                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 8
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 9
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 10
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 11
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If

                    Case 12
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If


                    Case 13
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 14
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 15
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 16
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 17
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16)) And dt.Rows(i).Item("estado") = 1) Then
                            dtProductoCopy.ImportRow(dt.Rows(i))
                            cont += 1
                        End If
                    Case 18
                        If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16)) And nombre.Trim.Contains(vectoraux(17)) And dt.Rows(i).Item("estado") = 1) Then
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
                If (estado = 2) Then
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
        Dim rowIndex As Integer = grDetalle.Row
        _fnObtenerFilaDetalle(pos, lin)
        If (e.Column.Index = grDetalle.RootTable.Columns("CantidadCompra").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("CantidadCompra")) Or grDetalle.GetValue("CantidadCompra").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra") = 1
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PorcentajeIncremento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadIncremento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1

                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                grDetalle.SetValue("Cantidad", 1)
                grDetalle.SetValue("CantidadIncremento", 0)
                grDetalle.SetValue("PorcentajeIncremento", 0)
                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grDetalle.GetValue("CantidadCompra") > 0) Then


                    Dim PorcentajeIncremento As Double = grDetalle.GetValue("PorcentajeIncremento")
                    Dim MontoIncremento As Double = (grDetalle.GetValue("CantidadCompra") * (PorcentajeIncremento / 100))
                    Dim CantTotal = MontoIncremento + grDetalle.GetValue("CantidadCompra")

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadIncremento") = (grDetalle.GetValue("CantidadCompra") * (PorcentajeIncremento / 100))
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = CantTotal

                    grDetalle.SetValue("Cantidad", CantTotal)
                    grDetalle.SetValue("CantidadIncremento", (grDetalle.GetValue("CantidadCompra") * (PorcentajeIncremento / 100)))

                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    P_PonerTotal(rowIndex)
                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                Else

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra") = 1
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PorcentajeIncremento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadIncremento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")
                    grDetalle.SetValue("Cantidad", 1)
                    grDetalle.SetValue("CantidadIncremento", 0)
                    grDetalle.SetValue("PorcentajeIncremento", 0)
                    P_PonerTotal(rowIndex)
                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If
        End If
        ''''Costo
        If (e.Column.Index = grDetalle.RootTable.Columns("PrecioCosto").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("PrecioCosto")) Or grDetalle.GetValue("PrecioCosto").ToString = String.Empty) Then
                Dim cantidad As Double = grDetalle.GetValue("Cantidad")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("costo")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("TotalCompra") = cantidad * CType(grDetalle.DataSource, DataTable).Rows(pos).Item("costo")

                P_PonerTotal(rowIndex)
            Else
                If (grDetalle.GetValue("PrecioCosto") > 0) Then

                    P_PonerTotal(rowIndex)
                Else

                    Dim cantidad As Double = grDetalle.GetValue("Cantidad")
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto")
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("TotalCompra") = cantidad * CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioCosto")
                End If
            End If
        End If
        ''' Precio Venta
        If (e.Column.Index = grDetalle.RootTable.Columns("PrecioVenta").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("PrecioVenta")) Or grDetalle.GetValue("PrecioVenta").ToString = String.Empty) Then

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioVenta") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("venta")
            Else
                If (grDetalle.GetValue("PrecioVenta") < 0) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PrecioVenta") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("venta")
                End If
            End If
        End If

        ''''''''''Porcentaje Incremento
        If (e.Column.Index = grDetalle.RootTable.Columns("PorcentajeIncremento").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("PorcentajeIncremento")) Or grDetalle.GetValue("CantidadCompra").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1


                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PorcentajeIncremento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadIncremento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra")

                grDetalle.SetValue("Cantidad", CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra"))
                grDetalle.SetValue("CantidadIncremento", 0)
                grDetalle.SetValue("PorcentajeIncremento", 0)
                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grDetalle.GetValue("PorcentajeIncremento") > 0) Then


                    Dim PorcentajeIncremento As Double = grDetalle.GetValue("PorcentajeIncremento")
                    Dim MontoIncremento As Double = (grDetalle.GetValue("CantidadCompra") * (PorcentajeIncremento / 100))

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadIncremento") = MontoIncremento
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = MontoIncremento + grDetalle.GetValue("CantidadCompra")


                    grDetalle.SetValue("Cantidad", MontoIncremento + grDetalle.GetValue("CantidadCompra"))
                    grDetalle.SetValue("CantidadIncremento", MontoIncremento)

                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    P_PonerTotal(rowIndex)
                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                Else


                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PorcentajeIncremento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadIncremento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra")
                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    grDetalle.SetValue("Cantidad", CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra"))
                    grDetalle.SetValue("CantidadIncremento", 0)
                    grDetalle.SetValue("PorcentajeIncremento", 0)

                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If


        End If

        ''''''''''Cantidad Incremento
        If (e.Column.Index = grDetalle.RootTable.Columns("CantidadIncremento").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("CantidadIncremento")) Or grDetalle.GetValue("CantidadIncremento").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1


                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PorcentajeIncremento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadIncremento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra")


                grDetalle.SetValue("Cantidad", CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra"))
                grDetalle.SetValue("CantidadIncremento", 0)
                grDetalle.SetValue("PorcentajeIncremento", 0)

                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grDetalle.GetValue("CantidadIncremento") > 0) Then
                    Dim MontoIncremento As Double = grDetalle.GetValue("CantidadIncremento")

                    Dim PorcentajeIncremento As Double = (grDetalle.GetValue("CantidadIncremento") * 100) / grDetalle.GetValue("CantidadCompra")

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PorcentajeIncremento") = PorcentajeIncremento
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadIncremento") = MontoIncremento
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = MontoIncremento + grDetalle.GetValue("CantidadCompra")


                    grDetalle.SetValue("Cantidad", MontoIncremento + grDetalle.GetValue("CantidadCompra"))
                    grDetalle.SetValue("PorcentajeIncremento", PorcentajeIncremento)

                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    P_PonerTotal(rowIndex)
                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                Else


                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("PorcentajeIncremento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadIncremento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra")

                    grDetalle.SetValue("Cantidad", CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCompra"))
                    grDetalle.SetValue("CantidadIncremento", 0)
                    grDetalle.SetValue("PorcentajeIncremento", 0)

                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If


        End If
        P_PonerTotal(rowIndex)
    End Sub
    Public Sub P_PonerTotal(rowIndex As Integer)

        Try
            If (rowIndex < grDetalle.RowCount) Then

                Dim lin As Integer = grDetalle.GetValue("Id")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                Dim cant As Double = grDetalle.GetValue("CantidadCompra")
                Dim uni As Double = grDetalle.GetValue("PrecioCosto")
                If (pos >= 0) Then
                    Dim TotalUnitario As Double = cant * uni
                    'grDetalle.SetValue("lcmdes", montodesc)

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("TotalCompra") = TotalUnitario
                    grDetalle.SetValue("TotalCompra", TotalUnitario)
                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")
                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If

            End If

        Catch ex As Exception

        End Try



    End Sub
    Private Sub btnConfirmarSalir_Click(sender As Object, e As EventArgs) Handles btnConfirmarSalir.Click
        Me.Close()
    End Sub
End Class