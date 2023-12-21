Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports System.Data.OleDb
Public Class Rep_SaldoProductos

    Public Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        '_prCargarComboLibreriaSucursal(cbAlmacen)
        '_prCargarComboGrupos(cbGrupos)
        P_Global._prCargarComboGenerico(cbAlmacen, L_prListarDepositos(), "Id", "Codigo", "NombreDeposito", "NombreDeposito")
        P_Global._prCargarComboGenerico(cbMarca, L_prLibreriaDetalleGeneral(3), "cnnum", "Codigo", "cndesc1", "Marca")

        Me.WindowState = FormWindowState.Maximized

        Me.Text = "REPORTE DE SALDOS"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None

        If (CType(cbAlmacen.DataSource, DataTable).Rows.Count > 0) Then
            cbAlmacen.SelectedIndex = 0
        End If

    End Sub
    Private Sub Rep_SaldoProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub





    Sub _prInhabilitarAlmacen()
        cbAlmacen.Enabled = False
    End Sub
    Sub _prhabilitarAlmacen()
        cbAlmacen.Enabled = True
    End Sub


    Private Sub CheckTodosVendedor_CheckValueChanged(sender As Object, e As EventArgs) Handles CheckTodosAlmacen.CheckValueChanged
        If (CheckTodosAlmacen.Checked) Then
            _prInhabilitarAlmacen()
        Else
            _prhabilitarAlmacen()
        End If

    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        _prCargarReporte()
    End Sub

    Public Sub _prInterpretarDatos(ByRef _dt As DataTable)
        Dim marcaid As Integer = 0

        If (Not chkMarcaTodos.Checked) Then
            marcaid = cbMarca.Value
        End If

        If (CheckTodosAlmacen.Checked And CheckMayorCero.Checked) Then

            _dt = ReporteSaldosTodosAlmacenesMayorA0(marcaid)


        End If
        If (CheckTodosAlmacen.Checked And CheckTodos.Checked) Then

            _dt = ReporteSaldosTodosAlmacenesTodos(marcaid)


        End If
        If (checkUnaAlmacen.Checked And CheckTodos.Checked) Then
            _dt = ReporteSaldosUnAlmacenTodosCantidad(cbAlmacen.Value, marcaid)
        End If
        'un almacen todos mayor a 0
        If (checkUnaAlmacen.Checked And CheckMayorCero.Checked) Then
            _dt = ReporteSaldosUnAlmacenCantidadMayor0(cbAlmacen.Value, marcaid)
        End If


    End Sub
    'grup panel stock mayor a cero o todos
    Private Sub _prCargarReporte()
        Dim _dt As New DataTable
        _prInterpretarDatos(_dt)
        If (_dt.Rows.Count > 0) Then

            Dim objrep As New R_ReporteSaldoFamilia
            objrep.SetDataSource(_dt)

            objrep.SetParameterValue("usuario", L_Usuario)
            MReportViewer.ReportSource = objrep
            MReportViewer.Show()
            MReportViewer.BringToFront()



        Else
            ToastNotification.Show(Me, "NO HAY DATOS PARA LOS PARAMETROS SELECCIONADOS..!!!",
                                       My.Resources.INFORMATION, 2000,
                                       eToastGlowColor.Blue,
                                       eToastPosition.BottomLeft)
            MReportViewer.ReportSource = Nothing
        End If





    End Sub

    Private Sub chkMarcaTodos_CheckValueChanged(sender As Object, e As EventArgs) Handles chkMarcaTodos.CheckValueChanged
        If (chkMarcaTodos.Checked) Then
            _prInhabilitarMarca()
        Else
            _prhabilitarMarca()
        End If
    End Sub
    Sub _prInhabilitarMarca()
        cbMarca.Enabled = False
    End Sub
    Sub _prhabilitarMarca()
        cbMarca.Enabled = True
    End Sub
End Class