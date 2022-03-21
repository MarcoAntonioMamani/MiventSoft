Public Class Formulario_Reportevb
    Public respuesta As Boolean = False
    Public Titulo As String = ""
    Public Descripcion As String = ""

#Region "Button Si"
    Private Sub Panel1_MouseHover(sender As Object, e As EventArgs) Handles btnSi.MouseHover
        btnSi.BackColor = Color.FromArgb(30, 199, 165)
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
        respuesta = True
        Me.Close()

    End Sub
    Private Sub Panel1_MouseLeave(sender As Object, e As EventArgs) Handles btnSi.MouseLeave
        btnSi.BackColor = Color.FromArgb(26, 179, 148)
    End Sub
    Private Sub Formulario_Eliminar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtTitulo.Text = Titulo
        txtDescripcion.Text = Descripcion
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
        respuesta = True
        Me.Close()
    End Sub

    Private Sub Formulario_Reportevb_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyData = Keys.S) Then

            respuesta = True
            Me.Close()
        End If

        If (e.KeyData = Keys.N) Then
            respuesta = False
            Me.Close()

        End If
    End Sub
#End Region


End Class