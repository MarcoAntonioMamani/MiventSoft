Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.Controls
Imports System.Drawing.Printing
Imports CrystalDecisions.Shared

Public Class Visualizador

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Private Sub Visualizador_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim blah As New Bitmap(New Bitmap(My.Resources.printee), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
        Me.Text = "Visualizador Reporte"
    End Sub

    Private Sub btnSeleccionarProducto_Click(sender As Object, e As EventArgs) Handles btnSeleccionarProducto.Click
        Dim pd As New PrintDocument()

        'Dim _Ds3 = L_ObtenerRutaImpresora("2") ' Datos de Impresion de Facturación
        Dim NombreImpresora As String = ""
        pd.PrinterSettings.PrinterName = gs_NombreImpresora

        If (Not pd.PrinterSettings.IsValid) Then
            ToastNotification.Show(Me, "La Impresora  ".ToUpper + gs_NombreImpresora + Chr(13) + "No Existe".ToUpper,
                                      img, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
        Else
            CrGeneral.ReportSource.PrintOptions.PrinterName = gs_NombreImpresora

            CrGeneral.ReportSource.PrintToPrinter(1, True, 0, 0)
        End If

    End Sub
End Class