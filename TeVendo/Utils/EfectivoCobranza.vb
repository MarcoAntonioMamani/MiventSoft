Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Public Class EfectivoCobranza
    Public MontoBs As Double = 0
    Public MontoDolares As Double = 0
    Public MontoTarjeta As Double = 0
    Public MontoTransferencia As Double = 0
    Public TotalVenta As Double = 0
    Public Bandera As Boolean = False
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)

    Private Sub EfectivoCobranza_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tbMontoBs.Value = MontoBs
        tbMontoDolar.Value = MontoDolares
        tbTarjeta.Value = MontoTarjeta
        tbTransferencia.Value = MontoTransferencia
        tbTotalVenta.Value = TotalVenta

        Calculartotal()
        _habilitarFocus()
        tbMontoBs.Focus()



    End Sub
    Public Sub _habilitarFocus()
        With Highlighter2
            .SetHighlightOnFocus(tbMontoBs, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbMontoDolar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbTarjeta, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbTransferencia, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

            .SetHighlightOnFocus(btnGuardar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub

    Private Sub chkTarjeta_CheckedChanged(sender As Object, e As EventArgs) Handles chkTarjeta.CheckedChanged

        If (chkTarjeta.Checked) Then

            tbTarjeta.Value = tbTotalVenta.Value
        Else
            tbTarjeta.Value = 0

        End If
        Calculartotal()
    End Sub

    Public Sub Calculartotal()
        Dim TotalPagado As Double = 0

        TotalPagado = tbMontoBs.Value + (tbMontoDolar.Value * Global_TipoCambio) + tbTarjeta.Value + tbTransferencia.Value
        tbTotalPagado.Value = TotalPagado

        Dim Cambio As Double = 0

        Cambio = TotalPagado - tbTotalVenta.Value

        If (Cambio > 0) Then
            tbCambio.Value = Cambio
        Else
            tbCambio.Value = 0
        End If




    End Sub

    Private Sub chkTransferencia_CheckedChanged(sender As Object, e As EventArgs) Handles chkTransferencia.CheckedChanged

        If (chkTransferencia.Checked) Then

            tbTransferencia.Value = tbTotalVenta.Value
        Else
            tbTransferencia.Value = 0


        End If
        Calculartotal()
    End Sub

    Private Sub tbTarjeta_ValueChanged(sender As Object, e As EventArgs) Handles tbTarjeta.ValueChanged

        Try
            If (tbTarjeta.Value > 0) Then

                chkTarjeta.CheckValue = True
            Else
                chkTarjeta.CheckValue = False
            End If
        Catch ex As Exception
            chkTarjeta.CheckValue = False
        End Try
        Calculartotal()

    End Sub

    Private Sub tbTransferencia_ValueChanged(sender As Object, e As EventArgs) Handles tbTransferencia.ValueChanged
        Try
            If (tbTransferencia.Value > 0) Then

                chkTransferencia.CheckValue = True
            Else
                chkTransferencia.CheckValue = False
            End If
        Catch ex As Exception
            chkTransferencia.CheckValue = False
        End Try
        Calculartotal()
    End Sub

    Private Sub tbMontoBs_ValueChanged(sender As Object, e As EventArgs) Handles tbMontoBs.ValueChanged

        Calculartotal()
    End Sub

    Private Sub tbMontoDolar_ValueChanged(sender As Object, e As EventArgs) Handles tbMontoDolar.ValueChanged
        Calculartotal()

        If (tbMontoDolar.Value > 0) Then
            lbDolar.Text = Str(tbMontoDolar.Value * Global_TipoCambio) + " Bs"
        Else
            lbDolar.Text = "0 Bs"
        End If
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Bandera = False
        Me.Close()

    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click

        If (tbTotalPagado.Value >= tbTotalVenta.Value) Then


            MontoBs = tbMontoBs.Value
            MontoDolares = tbMontoDolar.Value
            MontoTarjeta = tbTarjeta.Value
            MontoTransferencia = tbTransferencia.Value
            Bandera = True
            Me.Close()


        Else
            ToastNotification.Show(Me, "El Monto Pagado Es Menor Al Monto Total De La Venta", img, 5000, eToastGlowColor.Red, eToastPosition.BottomRight)
            tbMontoBs.Focus()
            Return

        End If

    End Sub

    Private Sub chkMontoBs_CheckedChanged(sender As Object, e As EventArgs) Handles chkMontoBs.CheckedChanged

        If (chkMontoBs.Checked) Then

            tbMontoBs.Value = tbTotalVenta.Value
        Else
            tbMontoBs.Value = 0


        End If
        Calculartotal()
    End Sub

    Private Sub tbMontoBs_KeyDown(sender As Object, e As KeyEventArgs) Handles tbMontoBs.KeyDown, tbMontoDolar.KeyDown, tbTransferencia.KeyDown, tbTarjeta.KeyDown
        If (e.KeyData = Keys.Enter) Then
            btnGuardar.PerformClick()

        End If
    End Sub
End Class