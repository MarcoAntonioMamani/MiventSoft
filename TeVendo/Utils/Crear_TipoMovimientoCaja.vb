Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Public Class Crear_TipoMovimientoCaja
    Public IdTipoMovimiento As Integer = -1
    Public Bandera As Boolean = False
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public Modulo As Integer = 0
    Private Sub CrearLibreria_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tbTipoMovimiento.Clear()
        _habilitarFocus()
        tbTipoMovimiento.Focus()

    End Sub

    Public Sub _habilitarFocus()
        With Highlighter2
            .SetHighlightOnFocus(tbTipoMovimiento, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swtipo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGuardar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If (tbTipoMovimiento.Text.Length > 3) Then
            Dim id As String = ""
            Bandera = InsertarTipoMovimiento(id, tbTipoMovimiento.Text, IIf(swtipo.Value = True, 1, 0))
            If (Bandera = True) Then

                IdTipoMovimiento = CInt(id.Trim)
                Me.Close()
            Else
                ToastNotification.Show(Me, "Error Al guardar Tipo Movimiento", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbTipoMovimiento.Focus()
            End If


        Else
            ToastNotification.Show(Me, "Debe Ingresar un Texto Valido en Tipo Movimiento", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbTipoMovimiento.Focus()
            Return


        End If
    End Sub

    Private Sub tbdescripcion_KeyDown(sender As Object, e As KeyEventArgs) Handles tbTipoMovimiento.KeyDown
        If (e.KeyData = Keys.Enter) Then
            btnGuardar.Focus()

        End If
    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Bandera = False
        Me.Close()
    End Sub

End Class