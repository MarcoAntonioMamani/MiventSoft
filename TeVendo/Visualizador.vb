Public Class Visualizador
    Private Sub Visualizador_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim blah As New Bitmap(New Bitmap(My.Resources.printee), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
        Me.Text = "Visualizador Reporte"
    End Sub
End Class