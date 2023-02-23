Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Imports System.IO

Public Class Tec_AdministrarCuentasPorCobrar
    Dim dtPendiente As DataTable
    Dim dtPagados As DataTable
    Dim IdPersonal As Integer = 0
    Dim IdCredito As Integer = 0
    Dim IdCreditoTodos As Integer = 0
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
#Region "General"
    Private Sub Tec_AdministrarCuentasPorPagar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarProceso()
    End Sub
    Public Sub IniciarProceso()
        _prCargarPagosPendientes()
        _prCargarCreditosPagados()
        Dim dt As DataTable = ListarPersonalById(Global_IdPersonal)
        If (dt.Rows.Count > 0) Then
            IdPersonal = Global_IdPersonal
            tbPersonal.Text = dt.Rows(0).Item("Nombre")



        End If
        Me.Text = "Administrar Cuentas por Cobrar"
        tbFechaTransaccion.Value = Now.Date
    End Sub

    Private Sub _prListaPagos()
        Dim dt As New DataTable
        dt = L_prListarPagosCreditoVenta(IdCredito)

        grPagos.DataSource = dt
        grPagos.RetrieveStructure()
        grPagos.AlternatingColors = True
        'FechaPago	Monto	Glosa	NroComprobante	NombrePersonal	img

        With grPagos.RootTable.Columns("FechaPago")
            .Width = 80
            .Caption = "Fecha De Pago"
            .FormatString = "dd/MM/yyyy"
            .Visible = True
        End With

        With grPagos.RootTable.Columns("CierreModulo")
            .Width = 70
            .Visible = False
        End With
        With grPagos.RootTable.Columns("Monto")
            .Width = 70
            .Visible = True
            .Caption = "Monto Pagado"
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grPagos.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar"
            .Visible = False
        End With


        With grPagos.RootTable.Columns("Glosa")
            .Width = 120
            .Caption = "Glosa"
            .Visible = True
        End With

        With grPagos.RootTable.Columns("NroComprobante")
            .Width = 70
            .Caption = "Comprobante"
            .Visible = True
        End With
        With grPagos.RootTable.Columns("NombrePersonal")
            .Width = 150
            .Caption = "Personal"
            .Visible = True
        End With
        With grPagos.RootTable.Columns("NombreCliente")
            .Width = 150
            .Caption = "Cliente"
            .Visible = False
        End With
        With grPagos.RootTable.Columns("Id")
            .Width = 70
            .Visible = False
        End With



        With grPagos
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed
            .CellToolTipText = "Pagos"
            .GroupByBoxVisible = False

        End With

    End Sub
    Private Sub _prListaPagosAdministracion()
        Dim dt As New DataTable
        dt = L_prListarPagosCreditoVenta(IdCreditoTodos)

        grPagosTodos.DataSource = dt
        grPagosTodos.RetrieveStructure()
        grPagosTodos.AlternatingColors = True
        'FechaPago	Monto	Glosa	NroComprobante	NombrePersonal	img
        With grPagosTodos.RootTable.Columns("CierreModulo")
            .Width = 70
            .Visible = False
        End With
        With grPagosTodos.RootTable.Columns("Id")
            .Width = 70
            .Visible = False
        End With
        With grPagosTodos.RootTable.Columns("FechaPago")
            .Width = 80
            .Caption = "Fecha De Pago"
            .FormatString = "dd/MM/yyyy"
            .Visible = True
        End With


        With grPagosTodos.RootTable.Columns("Monto")
            .Width = 70
            .Visible = True
            .Caption = "Monto Pagado"
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grPagosTodos.RootTable.Columns("img")
            .Width = 70
            .Caption = "Eliminar"
            .Visible = True
        End With


        With grPagosTodos.RootTable.Columns("Glosa")
            .Width = 120
            .Caption = "Glosa"
            .Visible = True
        End With

        With grPagosTodos.RootTable.Columns("NroComprobante")
            .Width = 70
            .Caption = "Comprobante"
            .Visible = True
        End With
        With grPagosTodos.RootTable.Columns("NombrePersonal")
            .Width = 150
            .Caption = "Personal"
            .Visible = True
        End With





        With grPagosTodos
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed
            .CellToolTipText = "Pagos"
            .GroupByBoxVisible = False

        End With
        CargarIconEstado()
    End Sub
    Public Sub CargarIconEstado()

        Dim dt As DataTable = CType(grPagosTodos.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1

            Dim Bin As New MemoryStream
            Dim img As New Bitmap(My.Resources.grilladelete, 40, 35)
            img.Save(Bin, Imaging.ImageFormat.Png)
            CType(grPagosTodos.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer


        Next

    End Sub
#End Region

#Region "Creditos Pagados"
    Private Sub _prCargarCreditosPagados()
        Dim dt As New DataTable
        dt = L_prListarCreditosPagadosCliente()
        dtPagados = dt.Copy
        grCreditoPagados.DataSource = dt
        grCreditoPagados.RetrieveStructure()
        grCreditoPagados.AlternatingColors = True
        'Credito Compra	NombreProveedor	Monto	Pagado		FechaVencimientoCredito	FechaUltimaPago

        With grCreditoPagados.RootTable.Columns("FechaUltimaPago")
            .Width = 110
            .Caption = "Ultimo Pago"
            .Visible = True
        End With
        With grCreditoPagados.RootTable.Columns("FechaVencimientoCredito")
            .Width = 110
            .Caption = "Venc. Credito"
            .Visible = True
        End With

        With grCreditoPagados.RootTable.Columns("Credito")
            .Width = 110
            .Visible = True
            .Caption = "Credito"
        End With

        With grCreditoPagados.RootTable.Columns("venta")
            .Width = 110
            .Caption = "Venta"
            .Visible = True
        End With
        With grCreditoPagados.RootTable.Columns("NombreCliente")
            .Width = 150
            .Caption = "Cliente"
            .Visible = True
        End With
        With grCreditoPagados.RootTable.Columns("Monto")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Monto"
        End With




        With grCreditoPagados.RootTable.Columns("Pagado")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "Pagado"
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With


        With grCreditoPagados
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed
            .CellToolTipText = "Credito"
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False

        End With

    End Sub

#End Region

#Region "Pagos Pendientes"

    Private Sub _prCargarPagosPendientes()
        Dim dt As New DataTable
        dt = L_prListarPagosPendientesClientes()
        dtPendiente = dt.Copy
        gr_CreditoPendientes.DataSource = dt
        gr_CreditoPendientes.RetrieveStructure()
        gr_CreditoPendientes.AlternatingColors = True
        'Credito Compra	NombreProveedor	Monto	abonado	Restante	FechaVencimientoCredito	DiasMora


        With gr_CreditoPendientes.RootTable.Columns("FechaVencimientoCredito")
            .Width = 110
            .Caption = "Venc. Credito"
            .Visible = True
        End With
        With gr_CreditoPendientes.RootTable.Columns("DiasMora")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0"
            .Caption = "Mora"
        End With
        With gr_CreditoPendientes.RootTable.Columns("Credito")
            .Width = 110
            .Visible = True
            .Caption = "Credito"
        End With

        With gr_CreditoPendientes.RootTable.Columns("venta")
            .Width = 110
            .Caption = "Venta"
            .Visible = True
        End With
        With gr_CreditoPendientes.RootTable.Columns("Nombrecliente")
            .Width = 150
            .Caption = "Cliente"
            .Visible = True
        End With
        With gr_CreditoPendientes.RootTable.Columns("Monto")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Monto"
        End With


        With gr_CreditoPendientes.RootTable.Columns("abonado")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "Abonado"
            .FormatString = "0.00"
        End With

        With gr_CreditoPendientes.RootTable.Columns("Restante")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "Restante"
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With


        With gr_CreditoPendientes
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed
            .CellToolTipText = "Credito"
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False

        End With

        _prAplicarCondiccionJanusCreditoPendientes()
    End Sub

    Public Sub _prAplicarCondiccionJanusCreditoPendientes()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(gr_CreditoPendientes.RootTable.Columns("DiasMora"), ConditionOperator.GreaterThan, 0)
        fc.FormatStyle.BackColor = Color.Red
        fc.FormatStyle.ForeColor = Color.White
        fc.FormatStyle.FontBold = TriState.True




        gr_CreditoPendientes.RootTable.FormatConditions.Add(fc)

    End Sub
    Public Sub P_GenerarReportePagosPendientes()
        dtPendiente.Rows.Clear()

        For Each _fil As GridEXRow In gr_CreditoPendientes.GetRows

            dtPendiente.Rows.Add(_fil.Cells("Credito").Value, _fil.Cells("venta").Value, _fil.Cells("Nombrecliente").Value, _fil.Cells("Monto").Value, _fil.Cells("abonado").Value, _fil.Cells("Restante").Value, _fil.Cells("FechaVencimientoCredito").Value, _fil.Cells("DiasMora").Value)

        Next
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If
        If (dtPendiente.Rows.Count <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Generar el Reporte".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return

        End If

        P_Global.Visualizador = New Visualizador

        Dim objrep As New Reporte_CobrosPendientes

        objrep.SetDataSource(dtPendiente)
        objrep.SetParameterValue("Usuario", L_Usuario)
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(110)
        P_Global.Visualizador.Show() 'Comentar
        ''P_Global.Visualizador.BringToFront() 'Comentar
    End Sub

    Public Sub P_GenerarReporteDeudasPagadas()
        dtPagados.Rows.Clear()

        For Each _fil As GridEXRow In grCreditoPagados.GetRows

            dtPagados.Rows.Add(_fil.Cells("Credito").Value, _fil.Cells("venta").Value, _fil.Cells("NombreCliente").Value, _fil.Cells("Monto").Value, _fil.Cells("Pagado").Value, _fil.Cells("FechaVencimientoCredito").Value, _fil.Cells("FechaUltimaPago").Value)

        Next
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If
        If (dtPagados.Rows.Count <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Generar el Reporte".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return

        End If

        P_Global.Visualizador = New Visualizador

        Dim objrep As New Reporte_CreditoVentaPagados
        objrep.SetDataSource(dtPagados)
        objrep.SetParameterValue("Usuario", L_Usuario)
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(110)
        P_Global.Visualizador.Show() 'Comentar
        '''P_Global.Visualizador.BringToFront() 'Comentar
    End Sub
    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click

        P_GenerarReportePagosPendientes()


    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        P_GenerarReporteDeudasPagadas()
    End Sub

    Private Sub tbNombreProveedor_KeyDown(sender As Object, e As KeyEventArgs) Handles tbDeuda.KeyDown
        If (e.KeyData = Keys.Control + Keys.Enter) Then
            Dim dt As DataTable

            dt = L_prListarPagosPendientesFiltrosClientes()

            'Credito Compra	Nombre	Monto	abonado	Restante	FechaVencimientoCredito	DiasMora

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id", False, "Credito", 50))
            listEstCeldas.Add(New Celda("Credito", True, "Credito", 90))
            listEstCeldas.Add(New Celda("Venta", True, "Compra", 90))
            listEstCeldas.Add(New Celda("Nombre", True, "Proveedor", 350))
            listEstCeldas.Add(New Celda("Monto", True, "Monto", 90, "0.00"))
            listEstCeldas.Add(New Celda("abonado", True, "Abonado", 90, "0.00"))
            listEstCeldas.Add(New Celda("Restante", True, "Restante", 90, "0.00"))
            listEstCeldas.Add(New Celda("FechaVencimientoCredito", True, "Venc.Credito".ToUpper, 100, "dd/MM/yyyy"))
            listEstCeldas.Add(New Celda("DiasMora", True, "Mora".ToUpper, 70, "0"))
            Dim ef = New Efecto
            ef.tipo = 6
            ef.dt = dt
            ef.SeleclCol = 2
            ef.listEstCeldasNew = listEstCeldas
            ef.alto = 90
            ef.ancho = 900
            ef.Context = "Seleccione Cuenta Por Cobrar".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                IdCredito = Row.Cells("Id").Value
                tbDeuda.Text = Row.Cells("Venta").Value.ToString + " a Cliente : " + Row.Cells("Nombre").Value.ToString
                tbGlosa.Focus()
                tbMonto.Value = Row.Cells("Monto").Value
                tbSaldo.Value = Row.Cells("Restante").Value
                tbGlosa.Clear()
                tbNroComprobante.Clear()
                tbMontoAPagar.Value = 0
                _prListaPagos()
                tbGlosa.Focus()
            End If
        End If
    End Sub

    Private Sub TextBoxX1_KeyDown(sender As Object, e As KeyEventArgs) Handles tbPersonal.KeyDown
        If (e.KeyData = Keys.Control + Keys.Enter) Then
            Dim dt As DataTable

            dt = ListarPersonalCredito()
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
            ef.Context = "Seleccione Proveedor".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                IdPersonal = Row.Cells("Id").Value
                tbPersonal.Text = Row.Cells("Nombre").Value
                tbGlosa.Focus()

            End If
        End If
    End Sub

    Private Sub btnProveedor_Click(sender As Object, e As EventArgs) Handles btnProveedor.Click
        Dim dt As DataTable

        dt = ListarPersonalCredito()
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
        ef.Context = "Seleccione Proveedor".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            IdPersonal = Row.Cells("Id").Value
            tbPersonal.Text = Row.Cells("Nombre").Value
            tbGlosa.Focus()

        End If
    End Sub

    Private Sub ButtonX3_Click(sender As Object, e As EventArgs) Handles ButtonX3.Click
        Dim dt As DataTable

        dt = L_prListarPagosPendientesFiltrosClientes()

        'Credito Compra	Nombre	Monto	abonado	Restante	FechaVencimientoCredito	DiasMora

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", False, "Credito", 50))
        listEstCeldas.Add(New Celda("Credito", True, "Credito", 90))
        listEstCeldas.Add(New Celda("Venta", True, "Compra", 90))
        listEstCeldas.Add(New Celda("Nombre", True, "Proveedor", 350))
        listEstCeldas.Add(New Celda("Monto", True, "Monto", 90, "0.00"))
        listEstCeldas.Add(New Celda("abonado", True, "Abonado", 90, "0.00"))
        listEstCeldas.Add(New Celda("Restante", True, "Restante", 90, "0.00"))
        listEstCeldas.Add(New Celda("FechaVencimientoCredito", True, "Venc.Credito".ToUpper, 100, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("DiasMora", True, "Mora".ToUpper, 70, "0"))
        Dim ef = New Efecto
        ef.tipo = 6
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 90
        ef.ancho = 800
        ef.Context = "Seleccione Cuenta Por Cobrar".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            IdCredito = Row.Cells("Id").Value
            tbDeuda.Text = Row.Cells("Venta").Value.ToString + " a Cliente : " + Row.Cells("Nombre").Value.ToString
            tbGlosa.Focus()
            tbMonto.Value = Row.Cells("Monto").Value
            tbSaldo.Value = Row.Cells("Restante").Value
            tbGlosa.Clear()
            tbNroComprobante.Clear()
            tbMontoAPagar.Value = 0
            _prListaPagos()
            tbGlosa.Focus()
        End If
    End Sub

    Private Sub tab03_Click(sender As Object, e As EventArgs) Handles tab03.Click
        tbDeuda.Focus()
        If (IdCredito <> 0) Then
            Dim dt As DataTable
            dt = L_prListarPagosPendientesFiltrosClientes()

            For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                If (IdCredito = dt.Rows(i).Item("Id")) Then
                    IdCredito = dt.Rows(i).Item("Id")
                    tbDeuda.Text = dt.Rows(i).Item("Venta") + " a Cliente : " + dt.Rows(i).Item("Nombre")
                    tbGlosa.Focus()
                    tbMonto.Value = dt.Rows(i).Item("Monto")
                    tbSaldo.Value = dt.Rows(i).Item("Restante")
                    tbGlosa.Clear()
                    tbNroComprobante.Clear()
                    tbMontoAPagar.Value = 0
                    _prListaPagos()
                    tbGlosa.Focus()
                End If
            Next


        End If
    End Sub

    Private Sub tbMontoAPagar_ValueChanged(sender As Object, e As EventArgs) Handles tbMontoAPagar.ValueChanged
        If (tbSaldo.Value > 0) Then
            If (tbMontoAPagar.Value > tbSaldo.Value) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El monto Maximo para Pagar es ".ToUpper + Str(tbSaldo.Value), img, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbMontoAPagar.Value = tbSaldo.Value
                tbMontoAPagar.Focus()
                Return
            End If



        End If
    End Sub


    Function ValidarCamposaGrabar() As Boolean
        Dim bandera As Boolean = True
        Dim Mensaje As String = "Los Siguientes Campos Son Requerido : "
        If (IdCredito <= 0) Then
            Mensaje = Mensaje + "  " + " Deuda, "
            bandera = False

        End If
        If (tbMontoAPagar.Value <= 0) Then
            Mensaje = Mensaje + "  " + " El monto a Pagar Debe ser Mayor a 0 "
            bandera = False

        End If
        If (tbFechaTransaccion.ToString = String.Empty) Then
            Mensaje = Mensaje + "  " + " Debe Seleccionar Fecha "
            bandera = False

        End If


        If (bandera = False) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, Mensaje, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If

        If (IdCredito <= 0) Then
            tbDeuda.Focus()
            Return bandera
        End If
        If (tbFechaTransaccion.ToString = String.Empty) Then
            tbFechaTransaccion.Focus()
            Return bandera
        End If
        If (tbMontoAPagar.Value <= 0) Then
            tbMontoAPagar.Focus()
            Return bandera

        End If

        Dim dtCierre As DataTable = L_prListarVentaCierresCajaPendientes(tbFechaTransaccion.Value.ToString("yyyy/MM/dd"), tbFechaTransaccion.Value.ToString("yyyy/MM/dd"))
        Dim fila As DataRow()
        If (Global_Sucursal > 0) Then

            fila = dtCierre.Select("SucursalId=" + Str(Global_Sucursal) + " and EstadoCaja=1 and PersonalId=" + Str(Global_IdPersonal))
        Else
            fila = dtCierre.Select("EstadoCaja=1 and PersonalId=" + Str(Global_IdPersonal))
        End If

        If (Not IsDBNull(fila)) Then
            If (fila.Count <= 0) Then

                ToastNotification.Show(Me, "No Es Posible Hacer La Venta Por que no Existe Caja Chica con Estado Abierta", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                Return False

            End If

        End If



        Return bandera

    End Function
    Public Sub Limpiar()
        tbGlosa.Clear()
        tbNroComprobante.Clear()
        tbSaldo.Value = tbSaldo.Value - tbMontoAPagar.Value
        tbMontoAPagar.Value = 0
        swTipoPago.Value = True
    End Sub

    Private Sub ButtonX4_Click(sender As Object, e As EventArgs) Handles ButtonX4.Click

        If (ValidarCamposaGrabar()) Then


            'Dim dt As DataTable = L_prListarGeneral("MAM_CierreCajero")

            'Dim fila As DataRow() = dt.Select("EstadoCaja=1")
            'If (Not IsDBNull(fila)) Then
            '    If (fila.Count <= 0) Then

            '        ToastNotification.Show(Me, "No Es Posible Hacer EL Cobro Por que no Existe Caja Chica con Estado Abierto Para Esta Fecha =" + tbFechaTransaccion.Value.ToString("dd/MM/yyy"), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            '        tbFechaTransaccion.Focus()
            '        Return
            '    Else
            '        Dim bandera As Boolean = False
            '        For Each item As Object In fila
            '            If (item("Fecha") = tbFechaTransaccion.Value) Then
            '                bandera = True
            '            End If
            '        Next
            '        If (bandera = False) Then
            '            ToastNotification.Show(Me, "No Es Posible Hacer EL Cobro Por que no Existe Caja Chica con Estado Abierto Para Esta Fecha =" + tbFechaTransaccion.Value.ToString("dd/MM/yyy"), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            '            tbFechaTransaccion.Focus()

            '            Return
            '        End If
            '    End If
            'End If

            Dim id As String = ""
            Try
                If (L_prGrabarPagosCreditoVentas(id, IdCredito, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"), IdPersonal, tbGlosa.Text, tbNroComprobante.Text, tbMontoAPagar.Value, IIf(swTipoPago.Value = True, 1, 0))) Then

                    Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                    ToastNotification.Show(Me, "El Pago Ha sido Registrado con Exito", img, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    Limpiar()
                    _prListaPagos()

                End If
            Catch ex As Exception
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Error al Grabar el Pago : " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End Try



        End If



    End Sub
    Private Sub AlertClicked(ByVal alertId As Long)

    End Sub

    Private Sub tbDeudaTodos_KeyDown(sender As Object, e As KeyEventArgs) Handles tbDeudaTodos.KeyDown
        If (e.KeyData = Keys.Control + Keys.Enter) Then
            Dim dt As DataTable

            dt = L_prListarPagosTodosCuentasPorCobrar()

            'Credito Compra	Nombre	Monto	abonado	Restante	FechaVencimientoCredito	DiasMora

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id", False, "Credito", 50))
            listEstCeldas.Add(New Celda("Credito", True, "Credito", 90))
            listEstCeldas.Add(New Celda("Venta", True, "Compra", 90))
            listEstCeldas.Add(New Celda("Nombre", True, "Proveedor", 350))
            listEstCeldas.Add(New Celda("Monto", True, "Monto", 90, "0.00"))
            listEstCeldas.Add(New Celda("abonado", True, "Abonado", 90, "0.00"))
            listEstCeldas.Add(New Celda("Restante", True, "Restante", 90, "0.00"))
            listEstCeldas.Add(New Celda("FechaVencimientoCredito", True, "Venc.Credito".ToUpper, 100, "dd/MM/yyyy"))
            listEstCeldas.Add(New Celda("DiasMora", True, "Mora".ToUpper, 70, "0"))
            Dim ef = New Efecto
            ef.tipo = 6
            ef.dt = dt
            ef.SeleclCol = 2
            ef.listEstCeldasNew = listEstCeldas
            ef.alto = 90
            ef.ancho = 900
            ef.Context = "Seleccione Cuenta Por Cobrar".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                IdCreditoTodos = Row.Cells("Id").Value
                tbDeudaTodos.Text = Row.Cells("Venta").Value.ToString + " a Cliente : " + Row.Cells("Nombre").Value.ToString

                tbtotalCompraTodos.Value = Row.Cells("Monto").Value
                tbSaldoTodos.Value = Row.Cells("Restante").Value
                _prListaPagosAdministracion()

            End If
        End If
    End Sub

    Private Sub btnDeudadTodos_Click(sender As Object, e As EventArgs) Handles btnDeudadTodos.Click
        Dim dt As DataTable

        dt = L_prListarPagosTodosCuentasPorCobrar()

        'Credito Compra	Nombre	Monto	abonado	Restante	FechaVencimientoCredito	DiasMora

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", False, "Credito", 50))
        listEstCeldas.Add(New Celda("Credito", True, "Credito", 90))
        listEstCeldas.Add(New Celda("Venta", True, "Compra", 90))
        listEstCeldas.Add(New Celda("Nombre", True, "Proveedor", 350))
        listEstCeldas.Add(New Celda("Monto", True, "Monto", 90, "0.00"))
        listEstCeldas.Add(New Celda("abonado", True, "Abonado", 90, "0.00"))
        listEstCeldas.Add(New Celda("Restante", True, "Restante", 90, "0.00"))
        listEstCeldas.Add(New Celda("FechaVencimientoCredito", True, "Venc.Credito".ToUpper, 100, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("DiasMora", True, "Mora".ToUpper, 70, "0"))
        Dim ef = New Efecto
        ef.tipo = 6
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 90
        ef.ancho = 900
        ef.Context = "Seleccione Cuenta Por Cobrar".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            IdCreditoTodos = Row.Cells("Id").Value
            tbDeudaTodos.Text = Row.Cells("Venta").Value.ToString + " a Cliente : " + Row.Cells("Nombre").Value.ToString

            tbtotalCompraTodos.Value = Row.Cells("Monto").Value
            tbSaldoTodos.Value = Row.Cells("Restante").Value
            _prListaPagosAdministracion()

        End If
    End Sub

    Private Sub SuperTabItem1_Click(sender As Object, e As EventArgs) Handles SuperTabItem1.Click
        tbDeudaTodos.Focus()
        If (IdCreditoTodos <> 0) Then
            Dim dt As DataTable
            dt = L_prListarPagosTodosCuentasPorCobrar()

            For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                If (dt.Rows(i).Item("Id") = IdCreditoTodos) Then
                    IdCreditoTodos = dt.Rows(i).Item("Id")
                    tbDeudaTodos.Text = dt.Rows(i).Item("Venta") + " a Cliente : " + dt.Rows(i).Item("Nombre")

                    tbtotalCompraTodos.Value = dt.Rows(i).Item("Monto")
                    tbSaldoTodos.Value = dt.Rows(i).Item("Restante")
                    _prListaPagosAdministracion()
                End If


            Next
        End If

    End Sub

    Private Sub grPagosTodos_Click(sender As Object, e As EventArgs) Handles grPagosTodos.Click
        If (grPagosTodos.RowCount >= 1) Then




            If (grPagosTodos.CurrentColumn.Index = grPagosTodos.RootTable.Columns("img").Index) Then

                If (grPagosTodos.GetValue("CierreModulo") > 0) Then
                    ToastNotification.Show(Me, "No Es Posible Eliminar El Cobro por Que ya Pertenece a un Cierre De Caja # " + Str(grPagosTodos.GetValue("CierreModulo")), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                    Return
                End If



                Dim ef = New Efecto


                    ef.tipo = 3
                    ef.titulo = "Eliminación De Pagos"
                    ef.descripcion = "¿Esta Seguro de Eliminar el Pago de Fecha " + grPagosTodos.GetValue("FechaPago") + " Con Monto de " + Str(grPagosTodos.GetValue("Monto")) + " ?"
                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then
                        Dim mensajeError As String = ""
                        Dim res As Boolean
                        Try
                            res = L_prEliminarPagosCuentaPorCobrar(grPagosTodos.GetValue("Id"))
                            If res Then

                                Dim imgOk As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                                ToastNotification.Show(Me, "El pago ha sido Eliminado con Exito".ToUpper, imgOk, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

                                tbSaldoTodos.Value = tbSaldoTodos.Value + grPagosTodos.GetValue("Monto")
                                _prListaPagosAdministracion()
                            Else
                                ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                            End If
                        Catch ex As Exception
                            ToastNotification.Show(Me, "Error al eliminar el Pago".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

                        End Try

                    End If
                End If
            End If
    End Sub

    Private Sub tab_configuraciones_Click(sender As Object, e As EventArgs) Handles tabCreditoPendiente.Click
        _prCargarPagosPendientes()

    End Sub

    Private Sub tab_compraventa_Click(sender As Object, e As EventArgs) Handles tabCreditoPagados.Click
        _prCargarCreditosPagados()
    End Sub

    Private Sub btnImprimirEstadoCuenta_Click(sender As Object, e As EventArgs) Handles btnImprimirEstadoCuenta.Click
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If
        If (grPagosTodos.RowCount <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Generar el Reporte".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return

        End If
        Dim dt As DataTable = L_EstadoDeCuentasPorCobrar(IdCreditoTodos)
        P_Global.Visualizador = New Visualizador

        Dim objrep As New Reporte_VentaEstadoCuentas


        objrep.SetDataSource(dt)
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(110)
        P_Global.Visualizador.Show() 'Comentar
    End Sub

    Private Sub gr_CreditoPendientes_DoubleClick(sender As Object, e As EventArgs) Handles gr_CreditoPendientes.DoubleClick
        If (gr_CreditoPendientes.Row >= 0) Then

            Dim ventasId As Integer = Integer.Parse(gr_CreditoPendientes.GetValue("venta").ToString.Replace("venta000", ""))
            Dim dt As New DataTable
            dt = ListaVentasDetalles(ventasId)
            Dim frm As frm_VerProductosVendidos
            frm = New frm_VerProductosVendidos(dt)
            frm.Show()


        End If
    End Sub
#End Region
End Class