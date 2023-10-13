Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class Rep_Historico

    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim IdCliente As Integer = 0

    Public Sub _prIniciarTodo()
        cbFechaDesde.Value = Now.Date.AddYears(-2)
        cbFechaHasta.Value = Now.Date
        tbVendedor.ReadOnly = True
        Me.Text = "REPORTE HISTORICO CLIENTES"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
    End Sub

    Private Sub Rep_VentasRealizadas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub
    Private Sub tbVendedor_KeyDown(sender As Object, e As KeyEventArgs) Handles tbVendedor.KeyDown
        If e.KeyData = Keys.Control + Keys.Enter Then


            Dim dt As DataTable

            dt = ListarCliente()
            'a.Id ,a.NombreCliente  as NombreProveedor ,a.DireccionCliente  ,a.Telefono

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id", False, "ID", 50))
            listEstCeldas.Add(New Celda("NombreProveedor", True, "NOMBRE", 350))
            listEstCeldas.Add(New Celda("DireccionCliente", True, "DIRECCION", 180))
            listEstCeldas.Add(New Celda("Telefono", True, "Telefono".ToUpper, 200))
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

                IdCliente = Row.Cells("Id").Value
                tbVendedor.Text = Row.Cells("NombreProveedor").Value
                cbFechaDesde.Focus()

            End If

        End If
    End Sub

    Private Sub btnVendedor_Click(sender As Object, e As EventArgs) Handles btnVendedor.Click

        Dim dt As DataTable

        dt = ListarCliente()
        'a.Id ,a.NombreCliente  as NombreProveedor ,a.DireccionCliente  ,a.Telefono

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", False, "ID", 50))
        listEstCeldas.Add(New Celda("NombreProveedor", True, "NOMBRE", 350))
        listEstCeldas.Add(New Celda("DireccionCliente", True, "DIRECCION", 180))
        listEstCeldas.Add(New Celda("Telefono", True, "Telefono".ToUpper, 200))
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

            IdCliente = Row.Cells("Id").Value
            tbVendedor.Text = Row.Cells("NombreProveedor").Value
            cbFechaDesde.Focus()

        End If

    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click


        Dim dtContado As DataTable = L_EstructuraVentasContadoCredito(IdCliente, cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), 1)

        Dim dtCredito As DataTable = L_EstructuraVentasContadoCredito(IdCliente, cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), 0)

        Dim TotalDeuda As Double = 0
        Dim TotalPagado As Double = 0
        For i As Integer = 0 To dtCredito.Rows.Count - 1 Step 1

            dtCredito.Rows(i).Item("Haber") = dtCredito.Rows(i).Item("Debe")
            dtContado.ImportRow(dtCredito.Rows(i))

            Dim dtpagos As DataTable = L_EstadoDeCuentasPorCobrarFechas(dtCredito.Rows(i).Item("creditoId"), cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))

            Dim saldo As Double = dtCredito.Rows(i).Item("Debe")

            TotalDeuda = TotalDeuda + dtCredito.Rows(i).Item("Debe")


            For j As Integer = 0 To dtpagos.Rows.Count - 1 Step 1

                saldo = saldo - dtpagos.Rows(j).Item("Monto")
                dtContado.Rows.Add(dtpagos.Rows(j).Item("FechaPago"), dtpagos.Rows(j).Item("FechaPago"),
                                       dtCredito.Rows(i).Item("Cliente"), dtCredito.Rows(i).Item("CodigoTransaccion"),
                                       "Registro De Pago De Credito - Recibo# " + dtpagos.Rows(j).Item("NroRecibo") + " - Del Recibo Venta Manual # " + dtpagos.Rows(j).Item("ReciboManual"), dtpagos.Rows(j).Item("Monto"), dtpagos.Rows(j).Item("Monto"), saldo)

                TotalPagado = TotalPagado + dtpagos.Rows(j).Item("Monto")
            Next

        Next
        For i As Integer = 0 To dtContado.Rows.Count - 1 Step 1
            dtContado.Rows(i).Item("TotalDeuda") = TotalDeuda - TotalPagado
        Next
        If (dtContado.Rows.Count > 0) Then


            dtContado.DefaultView.Sort = "fecha ASC"

            ' Ahora puedes acceder a los datos ordenados utilizando DefaultView
            Dim datosOrdenados As DataView = dtContado.DefaultView

            ' Si deseas asignar los datos ordenados de nuevo a un DataTable
            Dim miTablaOrdenada As New DataTable()
            miTablaOrdenada = datosOrdenados.ToTable()


            Dim objrep As New HistoricoCliente
            objrep.SetDataSource(miTablaOrdenada)
            Dim fechaI As String = cbFechaDesde.Value.ToString("dd/MM/yyyy")
            Dim fechaF As String = cbFechaHasta.Value.ToString("dd/MM/yyyy")
            objrep.SetParameterValue("Usuario", L_Usuario)
            objrep.SetParameterValue("Desde", fechaI)
            objrep.SetParameterValue("Hasta", fechaF)
            MReportViewer.ReportSource = objrep
            MReportViewer.Show()
            MReportViewer.BringToFront()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar. con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If



    End Sub
End Class