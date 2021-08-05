
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Public Class AyudaCantidadProductosVentas
    Public respuesta As Boolean = False
    Public NombreProducto As String = ""
    Public Qty As Double = 0
    Public CantidadTotal As Double = 0
    Public CantidadVenta As Double = 0
    Public TipoMovimiento As Integer = 0  ''4= ingreso 3 = egreso
    Public Precio As Double = 0
    Public ProductoId As Integer = 0

#Region "Button Si"


    Public Sub CargarTablaPrecios()
        Dim dt As New DataTable
        dt = L_prListarPrecio(ProductoId)

        grPrecios.DataSource = dt
        grPrecios.RetrieveStructure()

        'dar formato a las columnas
        With grPrecios.RootTable.Columns("Descripcion")
            .Width = 200
            .Visible = True
            .Caption = "Descripcion"

        End With

        With grPrecios.RootTable.Columns("Precio")
            .Caption = "Precio Caja"
            .Width = 100
            .FormatString = "0.00"
        End With

        With grPrecios.RootTable.Columns("PrecioUnitario")
            .Caption = "Precio Unitario"
            .Width = 100
            .FormatString = "0.00"
        End With



        With grPrecios
            .GroupByBoxVisible = False
            'diseño de la grilla
            .AllowAddNew = InheritableBoolean.False
            .RowFormatStyle.BackColor = Color.AliceBlue
            .SelectionMode = SelectionMode.SingleSelection
            '.SelectedFormatStyle.BackColor = Color.LightGreen
            '.RowFormatStyle.BackColor = Color.MediumTurquoise
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
        End With

        grPrecios.Row = 0
    End Sub
    Private Sub Panel1_MouseHover(sender As Object, e As EventArgs) Handles btnSi.MouseHover
        btnSi.BackColor = Color.FromArgb(30, 199, 165)
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
        ValidarStock()

    End Sub
    Private Sub Panel1_MouseLeave(sender As Object, e As EventArgs) Handles btnSi.MouseLeave
        btnSi.BackColor = Color.FromArgb(26, 179, 148)
    End Sub
    Private Sub Formulario_Eliminar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtProducto.Text = NombreProducto
        txtStock.Text = "Cantidad Unitario Disponible = " + Str(CantidadTotal) + Chr(13) + Chr(10) + "Cantidad Cajas Disponible = " + Str(CantidadTotal / Qty) + Chr(13) + Chr(10) + "QTY = " + Str(Qty)
        CargarTablaPrecios()
        _habilitarFocus()

        tbCantidad.Focus()
    End Sub

    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCantidad, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnNo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSi, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        End With
    End Sub
    Private Sub btnNo_MouseHover(sender As Object, e As EventArgs) Handles btnNo.MouseHover
        btnNo.BackColor = Color.FromArgb(170, 170, 170)
    End Sub

    Private Sub btnNo_MouseLeave(sender As Object, e As EventArgs) Handles btnNo.MouseLeave
        btnNo.BackColor = Color.FromArgb(191, 191, 191)
    End Sub



    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        respuesta = False
        Me.Close()
    End Sub

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        respuesta = False
        Me.Close()
    End Sub

    Private Sub btnSi_MouseClick(sender As Object, e As MouseEventArgs) Handles btnSi.MouseClick


        ValidarStock()
    End Sub

    Public Sub ValidarStock()

        If (IsNumeric(tbCantidad.Text)) Then
            Dim CantidadActual As Double = Double.Parse(tbCantidad.Text)
            If (TipoMovimiento = 4) Then

                Precio = grPrecios.GetValue("Precio")
                CantidadVenta = CantidadActual
                respuesta = True
                Me.Close()
            Else
                If (CantidadActual > CantidadTotal) Then

                    tbCantidad.Value = 0
                    tbCajas.Value = 0
                    tbCantidad.Text = Str(CantidadTotal).Trim
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "La cantidad es Superior Al Stock Disponible = " + Str(CantidadTotal), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                    tbCantidad.Focus()
                Else
                    If (CantidadActual > 0) Then
                        Precio = grPrecios.GetValue("Precio")
                        CantidadVenta = CantidadActual
                        respuesta = True
                        Me.Close()
                    Else
                        tbCantidad.Value = 0
                        tbCajas.Value = 0
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "La Cantidad debe ser Mayor o igual a 1", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                        tbCantidad.Focus()
                    End If


                End If
            End If


        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Ingrese Datos Validos", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)


        End If




    End Sub

    Private Sub tbCantidad_KeyDown(sender As Object, e As KeyEventArgs)
        If (e.KeyData = Keys.Enter) Then
            ValidarStock()
        End If
    End Sub

    Private Sub FormularioCantidadProductos_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyData = Keys.Escape) Then
            respuesta = False
            Me.Close()

        End If
    End Sub

    Private Sub tbCajas_ValueChanged(sender As Object, e As EventArgs) Handles tbCajas.ValueChanged

        Dim CantCajasMaximo As Double = CantidadTotal / Qty

        If (tbCajas.Value > CantCajasMaximo And TipoMovimiento <> 4) Then
            tbCantidad.Value = 0
            tbCajas.Value = 0

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "La cantidad Cajas es Superior Al Stock Disponible = " + Str(CantCajasMaximo), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)

            tbCajas.Select()
            tbCajas.Focus()

        Else

            tbCantidad.Value = tbCajas.Value * Qty


        End If

    End Sub

    Private Sub tbCantidad_ValueChanged(sender As Object, e As EventArgs) Handles tbCantidad.ValueChanged


        If (tbCantidad.Value > CantidadTotal And TipoMovimiento <> 4) Then
            tbCantidad.Value = 0
            tbCajas.Value = 0

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "La cantidad Unitaria es Superior Al Stock Disponible = " + Str(CantidadTotal), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbCantidad.Select()

            tbCantidad.Focus()
        Else

            tbCajas.Value = tbCantidad.Value / Qty


        End If
    End Sub

    Private Sub btnSi_Paint(sender As Object, e As PaintEventArgs) Handles btnSi.Paint

    End Sub

    Private Sub tbCajas_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCajas.KeyDown, tbCantidad.KeyDown

        If (e.KeyData = Keys.Enter) Then
            ValidarStock()
        End If
    End Sub
#End Region
End Class