
Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Public Class F1_Impresion
    Public Impresora As String = 0
    Public Bandera As Boolean = False
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Private Sub ActualizarTipoCambio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tbImpresora.Text = Impresora
        tbImpresora.Focus()
        _habilitarFocus()
    End Sub

    Public Sub _habilitarFocus()
        With Highlighter2
            .SetHighlightOnFocus(tbImpresora, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGuardar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If (tbImpresora.Text.Trim <> String.Empty) Then
            Dim id As String = ""
            Bandera = ActualizarImpresora(tbImpresora.Text.Trim)
            If (Bandera = True) Then

                Impresora = tbImpresora.Text.Trim

                gs_NombreImpresora = tbImpresora.Text.Trim
                Bandera = True
                Me.Close()
            Else
                ToastNotification.Show(Me, "Error Al guardar Nombre Impresora", img, 8000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                tbImpresora.Focus()
            End If


        Else
            ToastNotification.Show(Me, "Debe Ingresar Nombre Impresora", img, 8000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbImpresora.Focus()
            Return


        End If

    End Sub
    Private Sub tbdescripcion_KeyDown(sender As Object, e As KeyEventArgs) Handles tbImpresora.KeyDown
        If (e.KeyData = Keys.Enter) Then
            btnGuardar.PerformClick()

        End If
    End Sub
    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Bandera = False
        Me.Close()
    End Sub
End Class