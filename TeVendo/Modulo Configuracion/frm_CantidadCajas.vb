Imports DevComponents.DotNetBar
Public Class frm_CantidadCajas
    Public respuesta As Boolean = False
    Public NombreProducto As String = ""
    Public CantidadTotal As Double = 0
    Public CantidadVenta As Double = 0
    Public CantidadVentaUnitaria As Double = 0
    Public Conversion As Double = 0
    Public TipoMovimiento As Integer = 0  ''4= ingreso 3 = egreso


    Public BanderaCantidadCaja As Boolean = False
    Public BanderaCantidadUnitaria As Boolean = False

#Region "Button Si"
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
        txtStock.Text = "Cantidad Unitaria= " + Str(CantidadTotal) + "Cantidad Caja= " + Str(Format(CantidadTotal / Conversion, "0.00")) & vbNewLine & "Conversion = " + Str(Conversion)
        tbCantidad.Value = 0
        tbCantidadUnitaria.Value = 0
        _habilitarFocus()
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
            Dim CantidadActualUnitaria As Double = Double.Parse(tbCantidadUnitaria.Text)
            If (TipoMovimiento = 4) Then

                CantidadVenta = CantidadActual
                CantidadVentaUnitaria = CantidadActualUnitaria
                respuesta = True
                Me.Close()
            Else
                If (CantidadActualUnitaria > CantidadTotal) Then

                    tbCantidadUnitaria.Value = 0
                    tbCantidadUnitaria.Text = Str(CantidadTotal).Trim
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "La cantidad es Superior Al Stock Disponible = " + Str(CantidadTotal), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                    tbCantidadUnitaria.Focus()
                Else
                    If (CantidadActual > 0) Then
                        Dim CantidadActual02 As Double = Double.Parse(tbCantidadUnitaria.Text)
                        Dim CantidadConversionCaja As Double = CantidadActual02 / Conversion
                        CantidadVenta = CantidadConversionCaja
                        respuesta = True
                        Me.Close()
                    Else
                        tbCantidad.Value = 0
                        tbCantidad.Text = "0".Trim
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



    Private Sub tbCantidad_ValueChanged(sender As Object, e As EventArgs) Handles tbCantidad.ValueChanged
        If (BanderaCantidadUnitaria = True) Then
            Return

        End If
        BanderaCantidadCaja = True
        If (IsNumeric(tbCantidad.Text)) Then
            Dim CantidadActual As Double = Double.Parse(tbCantidad.Text)
            Dim CantidadConversionUnitaria As Double = CantidadActual * Conversion
            tbCantidadUnitaria.Text = Str(CantidadConversionUnitaria)
            BanderaCantidadCaja = False


        Else
            tbCantidad.Text = 0
            BanderaCantidadCaja = False
        End If
    End Sub

    Private Sub tbCantidadUnitaria_ValueChanged(sender As Object, e As EventArgs) Handles tbCantidadUnitaria.ValueChanged
        If (BanderaCantidadCaja = True) Then
            Return

        End If
        BanderaCantidadUnitaria = True
        If (IsNumeric(tbCantidadUnitaria.Text)) Then
            Dim CantidadActual As Double = Double.Parse(tbCantidadUnitaria.Text)
            Dim CantidadConversionCaja As Double = CantidadActual / Conversion
            tbCantidad.Text = Str(CantidadConversionCaja)
            BanderaCantidadUnitaria = False
        Else
            tbCantidadUnitaria.Text = 0
            BanderaCantidadUnitaria = False
        End If
    End Sub

    Private Sub btnSi_Paint(sender As Object, e As PaintEventArgs) Handles btnSi.Paint

    End Sub

    Private Sub tbCantidad_KeyDown_1(sender As Object, e As KeyEventArgs) Handles tbCantidad.KeyDown, tbCantidadUnitaria.KeyDown
        If (e.KeyData = Keys.Enter) Then
            ValidarStock()
        End If
    End Sub
#End Region
End Class