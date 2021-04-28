Public Class f1_ViewImage

    Public RutaImagen As String
    Public NombreProducto As String
    Private Sub f1_ViewImage_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        lbTitulo.Text = NombreProducto
        Dim bm As Bitmap = New Bitmap(RutaImagen)

        pbImagen.Image = bm
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Me.Close()

    End Sub
    Private Sub PictureBox_MouseWheel(sender As System.Object,
                             e As MouseEventArgs) Handles pbImagen.MouseWheel
        If e.Delta <> 0 Then
            If e.Delta <= 0 Then
                If pbImagen.Width < 500 Then Exit Sub 'minimum 500?
            Else
                If pbImagen.Width > 2000 Then Exit Sub 'maximum 2000?
            End If

            pbImagen.Width += CInt(pbImagen.Width * e.Delta / 1000)
            pbImagen.Height += CInt(pbImagen.Height * e.Delta / 1000)
        End If

    End Sub
End Class