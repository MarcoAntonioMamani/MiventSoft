Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class Reporte_PTigoMoney
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim IdPersonal As Integer = 0

    Public Sub _prIniciarTodo()
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date
        chkTodos.CheckValue = True


        Dim dt As DataTable = L_prLibreriaDetalleGeneral(15)

        dt.Rows.Add(0, "Todas Las Categorias")

        P_Global._prCargarComboGenerico(cbCategoria, dt, "cnnum", "Codigo", "cndesc1", "Categoria")

        cbCategoria.Value = 0
        tbVendedor.ReadOnly = True
        Me.Text = "REPORTE VENTAS TIGO MONEY"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None


    End Sub

    Private Sub Rep_VentasRealizadas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()

    End Sub

    Private Sub chkTodos_CheckedChanged(sender As Object, e As EventArgs) Handles chkTodos.CheckedChanged
        If (chkTodos.Checked = True) Then
            tbVendedor.Enabled = False
            btnVendedor.Visible = False
            tbVendedor.BackColor = Color.DarkGray
        Else
            tbVendedor.Enabled = True
            btnVendedor.Visible = True
            tbVendedor.BackColor = Color.White
            tbVendedor.Focus()
        End If
    End Sub

    Private Sub tbVendedor_KeyDown(sender As Object, e As KeyEventArgs) Handles tbVendedor.KeyDown
        If e.KeyData = Keys.Control + Keys.Enter Then


            Dim dt As DataTable

            dt = ListarPersonal()
            'a.Id ,a.NombreProveedor ,a.Direccion ,a.Telefono01

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id,", False, "ID", 50))
            listEstCeldas.Add(New Celda("Nombre", True, "NOMBRE", 350))
            listEstCeldas.Add(New Celda("Direccion", True, "DIRECCION", 180))
            listEstCeldas.Add(New Celda("Telefono01", True, "Telefono".ToUpper, 200))
            Dim ef = New Efecto
            ef.tipo = 6
            ef.dt = dt
            ef.SeleclCol = 2
            ef.listEstCeldasNew = listEstCeldas
            ef.alto = 50
            ef.ancho = 350
            ef.Context = "Seleccione Personal".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                IdPersonal = Row.Cells("Id").Value
                tbVendedor.Text = Row.Cells("Nombre").Value
                cbFechaDesde.Focus()

            End If

        End If
    End Sub

    Private Sub btnVendedor_Click(sender As Object, e As EventArgs) Handles btnVendedor.Click
        Dim dt As DataTable

        dt = ListarPersonal()
        'a.Id ,a.NombreProveedor ,a.Direccion ,a.Telefono01

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id,", False, "ID", 50))
        listEstCeldas.Add(New Celda("Nombre", True, "NOMBRE", 350))
        listEstCeldas.Add(New Celda("Direccion", True, "DIRECCION", 180))
        listEstCeldas.Add(New Celda("Telefono01", True, "Telefono".ToUpper, 200))
        Dim ef = New Efecto
        ef.tipo = 6
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 50
        ef.ancho = 350
        ef.Context = "Seleccione Personal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            IdPersonal = Row.Cells("Id").Value
            tbVendedor.Text = Row.Cells("Nombre").Value
            cbFechaDesde.Focus()

        End If
    End Sub

    Public Sub GenerarData(ByRef dt As DataTable)

        If (chkTodos.Checked = True) Then

            dt = ReporteVentasTigoMoneyTodosServicios(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))
        Else
            If (IdPersonal > 0) Then

                dt = ReporteVentasTigoMoneyTodosUnVendedorServicios(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), IdPersonal)
            Else
                tbVendedor.Focus()
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Seleccione un Personal Por Favor".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If
        End If

        '''Filtramos la linea de negocio


        Dim dtfinal As DataTable = dt.Copy

        dtfinal.Rows.Clear()

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            If (cbCategoria.Value = 0) Then
                dtfinal.ImportRow(dt.Rows(i))
            Else
                If (cbCategoria.Value = dt.Rows(i).Item("EstadoPedido")) Then
                    dtfinal.ImportRow(dt.Rows(i))
                End If
            End If
        Next


        dt = dtfinal

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

            Dim objrep As New TigoMoney_Ventas


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