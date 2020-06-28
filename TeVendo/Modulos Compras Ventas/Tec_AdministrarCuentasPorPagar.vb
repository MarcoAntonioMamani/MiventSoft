Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Public Class Tec_AdministrarCuentasPorPagar
    Dim dtPendiente As DataTable
    Dim dtPagados As DataTable
    Dim IdPersonal As Integer = 0
    Dim IdCredito As Integer = 0
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
    End Sub
#End Region

#Region "Creditos Pagados"
    Private Sub _prCargarCreditosPagados()
        Dim dt As New DataTable
        dt = L_prListarCreditosPagados()
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

        With grCreditoPagados.RootTable.Columns("Compra")
            .Width = 110
            .Caption = "Compra"
            .Visible = True
        End With
        With grCreditoPagados.RootTable.Columns("NombreProveedor")
            .Width = 150
            .Caption = "Proveedor"
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
        dt = L_prListarPagosPendientes()
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

        With gr_CreditoPendientes.RootTable.Columns("Compra")
            .Width = 110
            .Caption = "Compra"
            .Visible = True
        End With
        With gr_CreditoPendientes.RootTable.Columns("NombreProveedor")
            .Width = 150
            .Caption = "Proveedor"
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

            dtPendiente.Rows.Add(_fil.Cells("Credito").Value, _fil.Cells("Compra").Value, _fil.Cells("NombreProveedor").Value, _fil.Cells("Monto").Value, _fil.Cells("abonado").Value, _fil.Cells("Restante").Value, _fil.Cells("FechaVencimientoCredito").Value, _fil.Cells("DiasMora").Value)

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

        Dim objrep As New Reporte_PagosPendientes

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

            dtPagados.Rows.Add(_fil.Cells("Credito").Value, _fil.Cells("Compra").Value, _fil.Cells("NombreProveedor").Value, _fil.Cells("Monto").Value, _fil.Cells("abonado").Value, _fil.Cells("Restante").Value, _fil.Cells("FechaVencimientoCredito").Value, _fil.Cells("DiasMora").Value)

        Next
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If
        If (dtPagados.Rows.Count <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Generar el Reporte".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return

        End If

        'P_Global.Visualizador = New Visualizador

        'Dim objrep As New Reporte_PagosPendientes

        'objrep.SetDataSource(dtPendiente)
        'objrep.SetParameterValue("Usuario", L_Usuario)
        'P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        'P_Global.Visualizador.CrGeneral.Zoom(110)
        'P_Global.Visualizador.Show() 'Comentar
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

            dt = L_prListarPagosPendientesFiltros()

            'Credito Compra	Nombre	Monto	abonado	Restante	FechaVencimientoCredito	DiasMora

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id", False, "Credito", 50))
            listEstCeldas.Add(New Celda("Credito", True, "Credito", 90))
            listEstCeldas.Add(New Celda("Compra", True, "Compra", 90))
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
            ef.Context = "Seleccione Cuenta Por Pagar".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                IdCredito = Row.Cells("Id").Value
                tbDeuda.Text = Row.Cells("Compra").Value.ToString + " a Proveedor : " + Row.Cells("Nombre").Value.ToString
                tbGlosa.Focus()
                tbMonto.Value = Row.Cells("Monto").Value
                tbSaldo.Value = Row.Cells("Restante").Value
                tbGlosa.Focus()
            End If
        End If
    End Sub

    Private Sub TextBoxX1_KeyDown(sender As Object, e As KeyEventArgs) Handles tbPersonal.KeyDown
        If (e.KeyData = Keys.Control + Keys.Enter) Then
            Dim dt As DataTable

            dt = ListarProveedores()
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

        dt = ListarProveedores()
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

        dt = L_prListarPagosPendientesFiltros()

        'Credito Compra	Nombre	Monto	abonado	Restante	FechaVencimientoCredito	DiasMora

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", False, "Credito", 50))
        listEstCeldas.Add(New Celda("Credito", True, "Credito", 90))
        listEstCeldas.Add(New Celda("Compra", True, "Compra", 90))
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
        ef.Context = "Seleccione Cuenta Por Pagar".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            IdCredito = Row.Cells("Id").Value
            tbDeuda.Text = Row.Cells("Compra").Value.ToString + " a Proveedor : " + Row.Cells("Nombre").Value.ToString
            tbGlosa.Focus()
            tbMonto.Value = Row.Cells("Monto").Value
            tbSaldo.Value = Row.Cells("Restante").Value
            tbGlosa.Focus()
        End If
    End Sub

    Private Sub tab03_Click(sender As Object, e As EventArgs) Handles tab03.Click
        tbDeuda.Focus()
    End Sub
#End Region


End Class