Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO

Public Class Tec_DespachoDetalle

    Public dtProductos As DataTable
    Public dtDetalle As DataTable
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)

    Public Sub IniciarTodod()
        CargarProductosVentas()
        CargarProductos()
        _habilitarFocus()
        tbProducto.Focus()
    End Sub

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
            .Width = 90
            .Caption = "Id"
            .Visible = True
        End With
        With grDetalle.RootTable.Columns("DespachoProductosId")
            .Width = 110
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 110
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("NombreProducto")
            .Width = 300
            .Visible = True
            .Caption = "Producto"
        End With

        With grDetalle.RootTable.Columns("Cantidad")
            .Width = 110
            .Caption = "Cantidad"
            .Visible = True
            .FormatString = "0.00"
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grDetalle.RootTable.Columns("Stock")
            .Width = 110
            .Caption = "Stock"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grDetalle.RootTable.Columns("estado")
            .Width = 150
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("img")
            .Width = 120
            .Visible = False
            .Caption = "Eliminar"
            .LeftMargin = 7
            .TopMargin = 5
            .BottomMargin = 5
        End With
        With grDetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .CellToolTipText = "Conceptos"
            .GroupByBoxVisible = False
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed

        End With
        grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
    End Sub


    Public Function ExisteProductoDetalle(IdProducto As Integer) As Boolean

        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            If (dt.Rows(i).Item("estado") >= 0 And dt.Rows(i).Item("ProductoId") = IdProducto) Then
                Return True
            End If

        Next
        Return False
    End Function
    Public Function FiltrarProductos(dtSocurce As DataTable) As DataTable
        Dim dt As DataTable = dtSocurce
        Dim dtprod = dt.Copy
        dtprod.Rows.Clear()

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            If (Not ExisteProductoDetalle(dt.Rows(i).Item("Id"))) Then
                dtprod.ImportRow(dt.Rows(i))
            End If

        Next
        Return dtprod

    End Function

    Public Sub CargarProductos()
        dtProductos = ListarProductosSeleccionables(1)

        dtProductos = FiltrarProductos(dtProductos)

        grProductos.DataSource = dtProductos '' 1= Sucursal Principal

        grProductos.RetrieveStructure()
        grProductos.AlternatingColors = True

        '  Id	CodigoExterno	NombreProducto	DescripcionProducto	NombreCategoria	stock	estado
        With grProductos.RootTable.Columns("Id")
            .Width = 40
            .Caption = "Codigo"
            .Visible = True

        End With
        With grProductos.RootTable.Columns("CodigoExterno")
            .Width = 100
            .Caption = "CodigoExterno"
            .Visible = False

        End With


        With grProductos.RootTable.Columns("NombreProducto")
            .Width = 300
            .Caption = "Nombre Producto"
            .MaxLines = 2
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grProductos.RootTable.Columns("DescripcionProducto")
            .Width = 300
            .Caption = "Descripcion Producto"
            .MaxLines = 200
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With
        With grProductos.RootTable.Columns("NombreCategoria")
            .Width = 150
            .Caption = "Nombre Categoria"
            .MaxLines = 150
            .CellStyle.LineAlignment = TextAlignment.Near
            .WordWrap = True
            .Visible = True
        End With

        With grProductos.RootTable.Columns("stock")
            .Width = 100
            .FormatString = "0.00"
            .Visible = True
            .Caption = "Stock"
        End With
        With grProductos.RootTable.Columns("estado")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductos
            '.DefaultFilterRowComparison = FilterConditionOperator.BeginsWith
            '.FilterMode = FilterMode.Automatic
            '.FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With


        _prAplicarCondiccionJanusSinLote()
    End Sub
    Public Sub _prAplicarCondiccionJanusSinLote()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProductos.RootTable.Columns("stock"), ConditionOperator.LessThanOrEqualTo, 0)
        'fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.Red    'Color.Tan
        grProductos.RootTable.FormatConditions.Add(fc)
        Dim fr As GridEXFormatCondition
        fr = New GridEXFormatCondition(grProductos.RootTable.Columns("stock"), ConditionOperator.Equal, -9999)
        fr.FormatStyle.ForeColor = Color.Black
        grProductos.RootTable.FormatConditions.Add(fr)
    End Sub

    Private Sub Tec_DespachoDetalle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarTodod()
        tbProducto.Focus()
    End Sub

    Public Function ExisteProducto(Id As Integer) As Boolean
        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("ProductoId") = Id And dt.Rows(i).Item("estado") >= 0) Then
                Return True
            End If



        Next
        Return False
    End Function

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
            'Id  CodigoExterno	NombreProducto	DescripcionProducto	NombreCategoria	stock	estado
            For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                Dim nombre As String = dt.Rows(i).Item("CodigoExterno").ToString.ToUpper +
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
            grProductos.DataSource = dtProductoCopy.Copy
        Else
            grProductos.DataSource = dtProductos.Copy
        End If



    End Sub


    Private Sub tbProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles tbProducto.KeyDown
        If e.KeyData = Keys.Escape Then
            Me.Close()

        End If
        If e.KeyData = Keys.Down Then
            grProductos.Focus()
        End If
        If e.KeyData = Keys.Enter Then
            If grProductos.RowCount > 0 Then

                grProductos.Row = 0
                seleccionarProducto()
            End If

        End If
    End Sub

    Private Sub btnAgregarProducto_Click(sender As Object, e As EventArgs) Handles btnConfirmarSalir.Click
        Me.Close()
    End Sub

    Private Sub grProductos_KeyDown(sender As Object, e As KeyEventArgs) Handles grProductos.KeyDown
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grProductos.Col
            f = grProductos.Row
            If (f >= 0) Then

                seleccionarProducto()

            End If
        End If
        If e.KeyData = Keys.Escape Then
            Me.Close()

        End If
    End Sub

    Public Sub seleccionarProducto()
        If (Not ExisteProducto(grProductos.GetValue("Id"))) Then
            If (grProductos.GetValue("Stock") <= 0) Then

                ToastNotification.Show(Me, "El Producto no Cuenta con Stock Disponible", img, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)
                Return

            End If
            Dim ef = New Efecto
            ef.tipo = 5
            ef.NombreProducto = grProductos.GetValue("NombreProducto")
            ef.StockActual = grProductos.GetValue("stock")
            ef.TipoMovimiento = 3
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then

                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.rowdelete, 20, 18)
                img.Save(Bin, Imaging.ImageFormat.Png)

                Dim cantidad As Double = ef.CantidadTransaccion


                CType(grDetalle.DataSource, DataTable).Rows.Add(_GenerarId() + 1, 0, grProductos.GetValue("Id"), grProductos.GetValue("NombreProducto"), cantidad, grProductos.GetValue("stock"), 0, Bin.GetBuffer)

                CambiarEstado(grProductos.GetValue("Id"), 2)
                tbProducto.Focus()


            End If

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

            tbProducto.Focus()
        End If
    End Sub

    Public Sub CambiarEstado(ProductoId As Integer, Estado As Integer)
        If (IsNothing(grProductos.DataSource)) Then
            Return

        End If
        For i As Integer = 0 To CType(grProductos.DataSource, DataTable).Rows.Count - 1 Step 1
            If (CType(grProductos.DataSource, DataTable).Rows(i).Item("Id") = ProductoId) Then
                CType(grProductos.DataSource, DataTable).Rows(i).Item("estado") = Estado
            End If

        Next
        grProductos.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grProductos.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.Equal, 1))
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

    Private Sub grDetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grDetalle.EditingCell
        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
            e.Cancel = False
        Else
            e.Cancel = True

        End If

    End Sub
    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub
    Private Sub grDetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grDetalle.CellValueChanged
        Dim lin As Integer = grDetalle.GetValue("Id")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin)
        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("Cantidad")) Or grDetalle.GetValue("Cantidad").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                grDetalle.SetValue("Cantidad", 1)

                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grDetalle.GetValue("Cantidad") > 0) Then

                    If (grDetalle.GetValue("Cantidad") <= grDetalle.GetValue("Stock")) Then

                        Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                        If (estado = 1) Then
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                        End If

                    Else
                        ToastNotification.Show(Me, "La Cantidad = " + Str(grDetalle.GetValue("Cantidad")) + " es mayor al Stock del Producto = " + Str(grDetalle.GetValue("Stock")), img, 6000, eToastGlowColor.Red, eToastPosition.TopCenter)
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1

                        Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                        grDetalle.SetValue("Cantidad", 1)

                        If (estado = 1) Then
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                        End If

                    End If


                Else

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1

                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    grDetalle.SetValue("Cantidad", 1)

                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If
        End If
    End Sub

    Private Sub grProductos_DoubleClick(sender As Object, e As EventArgs) Handles grProductos.DoubleClick
        Dim f, c As Integer
        c = grProductos.Col
        f = grProductos.Row
        If (f >= 0) Then

            seleccionarProducto()

        End If
    End Sub
End Class