Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class Formulario_Cantidad_Lote
    Public respuesta As Boolean = False
    Public NombreProducto As String = ""
    Public CantidadTotal As Double = 0
    Public CantidadVenta As Double = 0
    Public Lote As String = ""
    Public Fecha As Date
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
        txtStock.Text = "Cantidad Disponible = " + Str(CantidadTotal)
        tbLote.Text = "20200101"
        cbFecha.Value = "01/01/2020"
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

        If (IsNumeric(tbCantidad.Text) And tbLote.Text.ToString.Length > 0) Then
            Dim CantidadActual As Double = Double.Parse(tbCantidad.Text)


            CantidadVenta = CantidadActual
            respuesta = True
            Lote = tbLote.Text
            Fecha = cbFecha.Value
            Me.Close()




        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Ingrese Datos Validos", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)


        End If




    End Sub

    Private Sub tbCantidad_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCantidad.KeyDown
        If (e.KeyData = Keys.Enter) Then
            tbLote.Focus()
        End If
    End Sub

    Private Sub FormularioCantidadProductos_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyData = Keys.Escape) Then
            respuesta = False
            Me.Close()

        End If
    End Sub

    Private Sub tbLote_KeyDown(sender As Object, e As KeyEventArgs) Handles tbLote.KeyDown
        If (e.KeyData = Keys.Enter) Then
            cbFecha.Focus()
        End If
    End Sub

    Private Sub cbFecha_KeyDown(sender As Object, e As KeyEventArgs) Handles cbFecha.KeyDown
        If (e.KeyData = Keys.Enter) Then
            ValidarStock()
        End If
        If (e.KeyData = Keys.Tab) Then
            btnSi.Focus()
        End If
    End Sub

    Private Sub btnSi_Paint(sender As Object, e As PaintEventArgs) Handles btnSi.Paint

        'ValidarStock()

    End Sub

    Private Sub btnNo_Paint(sender As Object, e As PaintEventArgs) Handles btnNo.Paint
        'respuesta = False
        'Me.Close()
    End Sub
#End Region
End Class