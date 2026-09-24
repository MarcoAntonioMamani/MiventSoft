Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar

Public Class Tec_IncrementoVenta

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Tec_IncrementoVenta_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prCargarMontoActual()
    End Sub

    Private Sub _prCargarMontoActual()
        Dim dt As DataTable = L_fnObtenerIncrementoVentaHoy()
        Dim monto As Decimal = 2.0D
        If (dt IsNot Nothing AndAlso dt.Rows.Count > 0) Then
            monto = dt.Rows(0).Item("Monto")
        End If
        tbMonto.Text = monto.ToString("0.00")
        tbMonto.SelectAll()
        tbMonto.Focus()
    End Sub

    ''Solo permite numeros y un unico punto decimal - sin agregar ningun control nuevo
    Private Sub tbMonto_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbMonto.KeyPress
        If (Char.IsControl(e.KeyChar)) Then
            Return
        End If
        If (e.KeyChar = "."c) Then
            If (tbMonto.Text.Contains(".")) Then
                e.Handled = True
            End If
            Return
        End If
        If (Not Char.IsDigit(e.KeyChar)) Then
            e.Handled = True
        End If
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim monto As Decimal

        If (Not Decimal.TryParse(tbMonto.Text, monto) OrElse monto <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Ingrese Un Monto Valido Mayor A Cero".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbMonto.Focus()
            tbMonto.SelectAll()
            Return
        End If

        L_prGuardarIncrementoVentaHoy(monto.ToString("0.00"))

        Dim imgOk As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
        ToastNotification.Show(Me, "Incremento Del Dia Actualizado Correctamente".ToUpper, imgOk, 4000, eToastGlowColor.Green, eToastPosition.TopCenter)

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub lblVerHistorial_Click(sender As Object, e As EventArgs) Handles lblVerHistorial.Click
        Dim frm As New Rep_HistorialIncrementoVenta
        frm.ShowDialog()
    End Sub

End Class
