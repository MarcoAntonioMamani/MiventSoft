Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Public Class Form_ProductosDadoBaja
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim IdPersonal As Integer = 0

    Public Sub _prIniciarTodo()


        Me.Text = "REPORTE PRODUCTOS DADO DE BAJA"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None


    End Sub

    Private Sub Rep_VentasRealizadas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()

    End Sub



    Public Sub GenerarData(ByRef dt As DataTable)


        dt = ReporteProductosDadoBaja()


    End Sub



    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click

        Dim _dt As New DataTable
        GenerarData(_dt)
        If (IsNothing(_dt) Or _dt.Rows.Count = 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            Return

        End If


        If (_dt.Rows.Count > 0) Then

            Dim objrep As New RptProductosBaja
            objrep.SetDataSource(_dt)
            objrep.SetParameterValue("Usuario", L_Usuario)
            MReportViewer.ReportSource = objrep
            MReportViewer.Show()
            MReportViewer.BringToFront()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar. con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If




    End Sub

End Class