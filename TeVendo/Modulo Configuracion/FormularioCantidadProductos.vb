Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls

Public Class FormularioCantidadProductos
    Public respuesta As Boolean = False
    Public NombreProducto As String = ""
    Public CantidadTotal As Double = 0
    Public CantidadVenta As Double = 0
    Public TipoMovimiento As Integer = 0  ''4= ingreso 3 = egreso

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

                CantidadVenta = CantidadActual
                respuesta = True
                Me.Close()
            Else
                If (CantidadActual > CantidadTotal) Then

                    tbCantidad.Clear()
                    tbCantidad.Text = Str(CantidadTotal).Trim
                    ToastNotification.Show(Me, "La cantidad es Superior Al Stock Disponible = " + Str(CantidadTotal), My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                    tbCantidad.Focus()
                Else
                    If (CantidadActual > 0) Then

                        CantidadVenta = CantidadActual
                        respuesta = True
                        Me.Close()
                    Else
                        tbCantidad.Clear()
                        tbCantidad.Text = "0".Trim
                        ToastNotification.Show(Me, "La Cantidad debe ser Mayor o igual a 1", My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                        tbCantidad.Focus()
                    End If


                End If
            End If


        Else
            ToastNotification.Show(Me, "Ingrese Datos Validos", My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)


        End If




    End Sub

    Private Sub tbCantidad_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCantidad.KeyDown
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
#End Region
End Class