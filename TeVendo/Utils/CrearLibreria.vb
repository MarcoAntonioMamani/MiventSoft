
Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Public Class CrearLibreria

    Public titulo As String = ""
    Public Bandera As Boolean = False
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public Modulo As Integer = 0
    Private Sub CrearLibreria_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lbTitulo.Text = titulo
        tbdescripcion.Clear()
        tbdescripcion.Focus()
        _habilitarFocus()
    End Sub

    Public Sub _habilitarFocus()
        With Highlighter2
            .SetHighlightOnFocus(tbdescripcion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGuardar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If (tbdescripcion.Text.Length > 1) Then
            Bandera = L_prClasificadorGrabar("", Modulo, tbdescripcion.Text)

            Me.Close()

        Else
            ToastNotification.Show(Me, "Debe Ingresar un Texto Valido en la Descripción", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbdescripcion.Focus()
            Return


        End If
    End Sub

    Private Sub tbdescripcion_KeyDown(sender As Object, e As KeyEventArgs) Handles tbdescripcion.KeyDown
        If (e.KeyData = Keys.Enter) Then
            btnGuardar.Focus()

        End If
    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Bandera = False
        Me.Close()
    End Sub
End Class