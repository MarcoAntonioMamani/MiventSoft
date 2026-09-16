Imports DevComponents.DotNetBar
Imports System.IO

''Popup minimalista para pedir UNA cantidad por defecto que se aplica a todos los productos
''que se agregan de un saque a la grilla de detalle de un movimiento de Ingreso
''(boton "Agregar todos los productos" en Tec_Movimientos). No valida contra stock porque,
''a diferencia de FormularioCantidadProductos (pensado para Salida/Venta de UN producto), en
''un Ingreso no hay un limite de "cantidad disponible" que respetar.
Public Class FormularioCantidadDefectoIngreso
    Public respuesta As Boolean = False
    Public CantidadPorDefecto As Double = 0

#Region "Button Si"
    Private Sub Panel1_MouseHover(sender As Object, e As EventArgs) Handles btnSi.MouseHover
        btnSi.BackColor = Color.FromArgb(30, 199, 165)
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
        ValidarCantidad()
    End Sub

    Private Sub Panel1_MouseLeave(sender As Object, e As EventArgs) Handles btnSi.MouseLeave
        btnSi.BackColor = Color.FromArgb(26, 179, 148)
    End Sub

    Private Sub FormularioCantidadDefectoIngreso_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _habilitarFocus()
        tbCantidad.SelectAll()
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
        ValidarCantidad()
    End Sub

    Public Sub ValidarCantidad()
        If (IsNumeric(tbCantidad.Text)) Then
            Dim CantidadActual As Double = Double.Parse(tbCantidad.Text)
            If (CantidadActual > 0) Then
                CantidadPorDefecto = CantidadActual
                respuesta = True
                Me.Close()
            Else
                tbCantidad.Clear()
                tbCantidad.Text = "0"
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "La Cantidad debe ser Mayor a 0", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbCantidad.Focus()
            End If
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Ingrese Datos Validos", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub tbCantidad_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCantidad.KeyDown
        If (e.KeyData = Keys.Enter) Then
            ValidarCantidad()
        End If
    End Sub

    Private Sub FormularioCantidadDefectoIngreso_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyData = Keys.Escape) Then
            respuesta = False
            Me.Close()
        End If
    End Sub
#End Region
End Class
