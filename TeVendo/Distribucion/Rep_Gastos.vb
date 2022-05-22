﻿
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Public Class Rep_Gastos
    'gb_FacturaIncluirICE 
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Dim Lote As Boolean = False
    Dim Dt1Kardex As DataTable
    Dim Dt2KardexTotal As DataTable
    Public Sub _prIniciarTodo()
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date

        Me.Text = "REPORTE DE GASTOS"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        Dim dt As DataTable = L_prLibreriaDetalleGeneral(14)
        dt.Rows.Add(-1, "Todos")

        P_Global._prCargarComboGenerico(cbGastos, dt, "cnnum", "Codigo", "cndesc1", "Concepto Gasto")
        cbGastos.Value = -1

    End Sub



    Private Sub Rep_MovimientosProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub


    Sub InterpretarDatos(ByRef dt As DataTable)

        Dim dtDatos As DataTable = ReporteGastosFecha(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))

        If (cbGastos.Value = -1) Then

            dt = dtDatos

        Else
            dt = dtDatos.Copy
            dt.Rows.Clear()
            For i As Integer = 0 To dtDatos.Rows.Count - 1 Step 1

                If (dtDatos.Rows(i).Item("TipoGastoId") = cbGastos.Value) Then
                    dt.ImportRow(dtDatos.Rows(i))
                End If


            Next



        End If
    End Sub
    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click

        If (cbGastos.SelectedIndex < 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Debe Seleccionar un Tipo de Gasto".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbGastos.Focus()
            Return

        End If

        Dim _dt As New DataTable

        InterpretarDatos(_dt)
        If (_dt.Rows.Count > 0) Then

            Dim objrep As New R_Gastos
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