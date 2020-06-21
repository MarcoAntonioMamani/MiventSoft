Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls
Public Class Tec_KardexProductos
    Public IdProducto As Integer = 0

    Public Sub IniciarProceso()
        P_Global._prCargarComboGenerico(cbDeposito, L_prListarDepositos(), "Id", "Codigo", "NombreDeposito", "NombreDeposito")
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date


    End Sub

    Private Sub Tec_KardexProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarProceso()

    End Sub

    Private Sub tbProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles tbProducto.KeyDown
        If (cbDeposito.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Seleccione un Deposito".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbDeposito.Focus()
            Return

        End If

        If e.KeyData = Keys.Control + Keys.Enter Then


            Dim dt As DataTable

            dt = L_prListarProductosKardex(cbDeposito.Value)
            'a.Id ,a.NombreProveedor ,a.Direccion ,a.Telefono01

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id,", True, "ID", 50))
            listEstCeldas.Add(New Celda("CodigoExterno,", False))
            listEstCeldas.Add(New Celda("estado,", False))
            listEstCeldas.Add(New Celda("Nombre", True, "Producto", 350))
            listEstCeldas.Add(New Celda("DescripcionProducto", True, "Descripcion", 180))
            listEstCeldas.Add(New Celda("stock", True, "Stock Disponible".ToUpper, 120, "0.00"))
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
                tbProducto.Text = Str(Row.Cells("Id").Value) + "  " + Row.Cells("NombreProveedor").Value
                cbDeposito.Focus()

            End If

        End If
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        If (IdProducto <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Seleccione un Producto".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbProducto.Focus()
        End If



    End Sub

    Private Sub btnProveedor_Click(sender As Object, e As EventArgs) Handles btnProveedor.Click
        If (cbDeposito.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Seleccione un Deposito".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbDeposito.Focus()
            Return

        End If



        Dim dt As DataTable

            dt = L_prListarProductosKardex(cbDeposito.Value)
            'a.Id ,a.NombreProveedor ,a.Direccion ,a.Telefono01

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id,", True, "ID", 50))
            listEstCeldas.Add(New Celda("CodigoExterno,", False))
            listEstCeldas.Add(New Celda("estado,", False))
            listEstCeldas.Add(New Celda("Nombre", True, "Producto", 350))
            listEstCeldas.Add(New Celda("DescripcionProducto", True, "Descripcion", 180))
            listEstCeldas.Add(New Celda("stock", True, "Stock Disponible".ToUpper, 120, "0.00"))
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
                tbProducto.Text = Str(Row.Cells("Id").Value) + "  " + Row.Cells("NombreProveedor").Value
                cbDeposito.Focus()

            End If

    End Sub
End Class