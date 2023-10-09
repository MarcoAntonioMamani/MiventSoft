Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class Rep_Historico

    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim IdCliente As Integer = 0

    Public Sub _prIniciarTodo()
        cbFechaDesde.Value = Now.Date
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
            listEstCeldas.Add(New Celda("NombreCliente", True, "NOMBRE", 350))
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
                tbVendedor.Text = Row.Cells("Nombre").Value
                cbFechaDesde.Focus()

            End If

        End If
    End Sub

End Class