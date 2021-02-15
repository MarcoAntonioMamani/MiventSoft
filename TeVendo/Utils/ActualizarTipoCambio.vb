
Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Public Class ActualizarTipoCambio
    Public MontoTipoCambio As Double = 0
    Public Bandera As Boolean = False
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Private Sub ActualizarTipoCambio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tbTipoCambio.Value = MontoTipoCambio
        tbTipoCambio.Focus()
        _habilitarFocus()
    End Sub

    Public Sub _habilitarFocus()
        With Highlighter2
            .SetHighlightOnFocus(tbTipoCambio, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGuardar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If (tbTipoCambio.Text.Trim <> String.Empty) Then
            Dim id As String = ""
            Bandera = InsertarTipoCambio(id, tbTipoCambio.Value)
            If (Bandera = True) Then

                MontoTipoCambio = tbTipoCambio.Value
                Bandera = True
                Me.Close()
            Else
                ToastNotification.Show(Me, "Error Al guardar Tipo De Cambio", img, 8000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                tbTipoCambio.Focus()
            End If


        Else
            ToastNotification.Show(Me, "Debe Ingresar un Valor Correcto en Tipo De Cambio", img, 8000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbTipoCambio.Focus()
            Return


        End If

    End Sub
    Private Sub tbdescripcion_KeyDown(sender As Object, e As KeyEventArgs) Handles tbTipoCambio.KeyDown
        If (e.KeyData = Keys.Enter) Then
            btnGuardar.PerformClick()

        End If
    End Sub
    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Bandera = False
        Me.Close()
    End Sub
End Class