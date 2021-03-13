Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class Tec_ReporteCajaDetallado
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim IdPersonal As Integer = 0

    Public Sub _prIniciarTodo()
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date

        Me.Text = "MOVIMIENTO CAJAS DETALLADO"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        _prCargarComboCaja(cbCaja)

    End Sub

    Private Sub _prCargarComboCaja(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prListarCaja()
        dt.Rows.Add(-1, "Todos")
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("Id").Width = 70
            .DropDownList.Columns("Id").Caption = "COD"
            .DropDownList.Columns.Add("NombreCaja").Width = 200
            .DropDownList.Columns("NombreCaja").Caption = "Caja"
            .ValueMember = "Id"
            .DisplayMember = "NombreCaja"
            .DataSource = dt
            .Refresh()
        End With


        mCombo.SelectedIndex = dt.Rows.Count - 1

    End Sub

    Private Sub Tec_ReporteCajaDetallado_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Public Sub GenerarData(ByRef dt As DataTable)



        dt = ReporteIngresoEgresoDetalladoCaja(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), cbCaja.Value)


    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click

        If (cbCaja.SelectedIndex < 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Debe Seleccionar una Caja".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbCaja.Focus()
            Return

        End If

        Dim _dt As New DataTable
        GenerarData(_dt)
        If (IsNothing(_dt) Or _dt.Rows.Count = 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            Return

        End If


        If (_dt.Rows.Count > 0) Then

            Dim objrep As New Reporte_MovimientoCajaDetallado
            objrep.SetDataSource(_dt)
            Dim fechaI As String = cbFechaDesde.Value.ToString("dd/MM/yyyy")
            Dim fechaF As String = cbFechaHasta.Value.ToString("dd/MM/yyyy")
            objrep.SetParameterValue("FechaDesde", fechaI)
            objrep.SetParameterValue("FechaHasta", fechaF)
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