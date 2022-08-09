
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class FReporteCompraProducto
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim IdProducto As Integer = 0


    Public Sub _prIniciarTodo()
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date
        chkTodos.CheckValue = True

        Me.Text = "REPORTE DE COMPRAS POR PRODUCTOS"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None


    End Sub

    Private Sub Rep_VentasRealizadas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()

    End Sub




    Private Sub btnVendedor_Click(sender As Object, e As EventArgs) Handles btnVendedor.Click
        Dim dt As DataTable


        dt = L_prListarProductosKardex(1)
        'a.Id ,a.NombreProveedor ,a.Direccion ,a.Telefono01

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 50))
        listEstCeldas.Add(New Celda("CodigoExterno,", False))
        listEstCeldas.Add(New Celda("Nombre", True, "Producto", 350))
        listEstCeldas.Add(New Celda("DescripcionProducto", False, "Descripcion", 180))
        listEstCeldas.Add(New Celda("stock", True, "Stock Disponible".ToUpper, 120, "0.00"))
        listEstCeldas.Add(New Celda("estado,", False))
        Dim ef = New Efecto
        ef.tipo = 6
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 50
        ef.ancho = 450
        ef.Context = "Seleccione Producto".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then

            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            IdProducto = Row.Cells("Id").Value
            tbProducto.Text = Row.Cells("Nombre").Value
            cbFechaDesde.Focus()

        End If

    End Sub

    Public Sub GenerarData(ByRef dt As DataTable)

        If (chkTodos.Checked = True) Then
            IdProducto = 0

            dt = ReporteComprasPorProductos(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), IdProducto)




        Else
            dt = ReporteComprasPorProductos(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), IdProducto)
        End If
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

            Dim objrep As New ReporteCompraPorProducto
            objrep.SetDataSource(_dt)
            Dim fechaI As String = cbFechaDesde.Value.ToString("yyyy/MM/dd")
            Dim fechaF As String = cbFechaHasta.Value.ToString("yyyy/MM/dd")
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

    Private Sub chkTodos_CheckedChanged(sender As Object, e As EventArgs) Handles chkTodos.CheckedChanged
        If (chkTodos.Checked = True) Then
            tbProducto.Enabled = False
            btnVendedor.Visible = False
            tbProducto.BackColor = Color.DarkGray
        Else
            tbProducto.Enabled = True
            btnVendedor.Visible = True
            tbProducto.BackColor = Color.White
            tbProducto.Focus()
        End If
    End Sub
End Class