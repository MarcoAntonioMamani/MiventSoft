Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid.Style
Imports Janus.Data
Public Class Tec_ReporteIngresosEgresosMensuales
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public Sub _prIniciarTodo()

        Me.Text = "REPORTE INGRESOS/EGRESOS MENSUALES"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        P_Global._prCargarComboGenerico(cbAnio, L_prLibreriaDetalleGeneral(13), "cnnum", "Codigo", "cndesc1", "Año")

        cbAnio.SelectedIndex = CType(cbAnio.DataSource, DataTable).Rows.Count - 1
    End Sub

    Private Sub Tec_ReporteIngresosEgresosMensuales_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub


    Public Sub ArmarTabla(ByRef dt As DataTable, ByRef dtGrafico As DataTable)

        Dim Anio As Integer = CInt(cbAnio.Text)

        For i As Integer = 1 To 12 Step 1
            Dim dtIngresoegreso As DataTable = ReporteIngresosEgresosMes(Anio, i)
            Dim totalIngreso As Double = IIf(IsDBNull(dtIngresoegreso.Compute("Sum(Monto)", "IngresoEgreso=1")), 0, dtIngresoegreso.Compute("Sum(Monto)", "IngresoEgreso=1"))
            Dim totalEgreso As Double = IIf(IsDBNull(dtIngresoegreso.Compute("Sum(Monto)", "IngresoEgreso=0")), 0, dtIngresoegreso.Compute("Sum(Monto)", "IngresoEgreso=0"))
            dtGrafico.Rows(i - 1).Item("Ingreso") = totalIngreso
            dtGrafico.Rows(i - 1).Item("Egreso") = totalEgreso * -1
            If (i = 1) Then

                dt.Rows(0).Item("Enero") = totalIngreso
                dt.Rows(1).Item("Enero") = totalEgreso

            End If
            If (i = 2) Then

                dt.Rows(0).Item("Febrero") = totalIngreso
                dt.Rows(1).Item("Febrero") = totalEgreso

            End If
            If (i = 3) Then

                dt.Rows(0).Item("Marzo") = totalIngreso
                dt.Rows(1).Item("Marzo") = totalEgreso
            End If
            If (i = 4) Then

                dt.Rows(0).Item("Abril") = totalIngreso
                dt.Rows(1).Item("Abril") = totalEgreso
            End If
            If (i = 5) Then

                dt.Rows(0).Item("Mayo") = totalIngreso
                dt.Rows(1).Item("Mayo") = totalEgreso
            End If
            If (i = 6) Then

                dt.Rows(0).Item("Junio") = totalIngreso
                dt.Rows(1).Item("Junio") = totalEgreso
            End If
            If (i = 7) Then

                dt.Rows(0).Item("Julio") = totalIngreso
                dt.Rows(1).Item("Julio") = totalEgreso
            End If
            If (i = 8) Then

                dt.Rows(0).Item("Agosto") = totalIngreso
                dt.Rows(1).Item("Agosto") = totalEgreso
            End If
            If (i = 9) Then

                dt.Rows(0).Item("Septiembre") = totalIngreso
                dt.Rows(1).Item("Septiembre") = totalEgreso
            End If
            If (i = 10) Then

                dt.Rows(0).Item("Octubre") = totalIngreso
                dt.Rows(1).Item("Octubre") = totalEgreso
            End If
            If (i = 11) Then

                dt.Rows(0).Item("Noviembre") = totalIngreso
                dt.Rows(1).Item("Noviembre") = totalEgreso
            End If
            If (i = 12) Then

                dt.Rows(0).Item("Diciembre") = totalIngreso
                dt.Rows(1).Item("Diciembre") = totalEgreso

            End If

        Next

    End Sub
    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        Dim _dt As New DataTable
        _dt = ReporteListarMesesIngresosEgresos()
        Dim dtGrafico As DataTable = ReporteListarMesesIngresosEgresosGrafico()
        ArmarTabla(_dt, dtGrafico)

        If (IsNothing(_dt) Or _dt.Rows.Count = 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            Return

        End If


        If (_dt.Rows.Count > 0) Then

            Dim objrep As New Reporte_ResumenIngresoEgresos


            objrep.SetDataSource(_dt)

            objrep.Subreports.Item("ReporteGraficoIngresoEgreso.rpt").SetDataSource(dtGrafico)
            objrep.SetParameterValue("Anio", cbAnio.Text)
            objrep.SetParameterValue("Usuario", L_Usuario)
            MReportViewer.ReportSource = objrep
            MReportViewer.Show()
            MReportViewer.BringToFront()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar. con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub btnAnio_Click(sender As Object, e As EventArgs) Handles btnAnio.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 13
        ef.titulo = "Crear Gestion(Año)"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbAnio, L_prLibreriaDetalleGeneral(13), "cnnum", "Codigo", "cndesc1", "Año")
            cbAnio.SelectedIndex = CType(cbAnio.DataSource, DataTable).Rows.Count - 1
            cbAnio.Focus()
        End If
    End Sub
End Class