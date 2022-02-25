Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class Tec_IngresoEgresoPorTipo
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim IdPersonal As Integer = 0

    Public Sub _prIniciarTodo()
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date

        Me.Text = "REPORTE DE INGRESOS O EGRESOS DETALLADO"


    End Sub


    Private Sub Tec_ReporteIngresosEgresosDetallado_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Public Sub GenerarData(ByRef dt As DataTable)



        dt = ReporteIngresoEgresoDetallado(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))


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

            Dim objrep As New Reporte_IngresosEgresosDetallado
            objrep.SetDataSource(_dt)
            Dim fechaI As String = cbFechaDesde.Value.ToString("dd/MM/yyyy")
            Dim fechaF As String = cbFechaHasta.Value.ToString("dd/MM/yyyy")
            objrep.SetParameterValue("FechaDesde", fechaI)
            objrep.SetParameterValue("FechaHasta", fechaF)
            objrep.SetParameterValue("Usuario", L_Usuario)
            'MReportViewer.ReportSource = objrep
            'MReportViewer.Show()
            'MReportViewer.BringToFront()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar. con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If

    End Sub
End Class