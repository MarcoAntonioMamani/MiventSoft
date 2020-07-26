Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls
Public Class Tec_KardexProductos
    Public IdProducto As Integer = 0
    Public Lote As Boolean = False
    Dim Dt1Kardex As DataTable
    Dim Dt2KardexTotal As DataTable

    Public Sub IniciarProceso()
        P_Global._prCargarComboGenerico(cbDeposito, L_prListarDepositos(), "Id", "Codigo", "NombreDeposito", "NombreDeposito")
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date
        tbProducto.Focus()
        Me.Text = "Historial De Productos"
        LeerConfiguracion()
    End Sub
    Public Sub LeerConfiguracion()
        Dim dt As DataTable = L_prLeerConfiguracion()
        If (dt.Rows.Count > 0) Then
            Lote = dt.Rows(0).Item("Lote")
        End If
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
                tbProducto.Text = Str(Row.Cells("Id").Value) + "  " + Row.Cells("Nombre").Value
                tbSaldo.Value = Row.Cells("stock").Value
                cbDeposito.Focus()

                ArmarGrillaMovimientos()
            End If

        End If
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btGenerarKardex.Click
        If (IdProducto <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Seleccione un Producto".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbProducto.Focus()
            Return

        End If

        ArmarGrillaMovimientos()



    End Sub

    Public Sub ArmarGrillaMovimientos()
        PanelMovimientos.Text = "Listado de Movimientos  : " + tbProducto.Text
        Dt1Kardex = New DataTable
        Dt2KardexTotal = New DataTable
        _GenerarHistorial()

        grMovimientos.BoundMode = Janus.Data.BoundMode.Bound
        grMovimientos.DataSource = Dt1Kardex
        grMovimientos.RetrieveStructure()

        'dar formato a las columnas
        With grMovimientos.RootTable.Columns("id")
            .Caption = "ID"
            .Width = 70
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
        End With

        With grMovimientos.RootTable.Columns("fdoc")
            .Caption = "Fecha Transaccion".ToUpper
            .Width = 120
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
        End With

        With grMovimientos.RootTable.Columns("concep")
            .Caption = ""
            .Width = 0
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            '.CellStyle.BackColor = Color.AliceBlue
        End With

        With grMovimientos.RootTable.Columns("descConcep")
            .Caption = "Tipo Movimiento".ToUpper
            .Width = 180
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
        End With
        With grMovimientos.RootTable.Columns("obs")
            .Caption = "Descripcion Movimiento".ToUpper
            .Width = 400
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
        End With

        ''''''''''''''

        With grMovimientos.RootTable.Columns("Lote")
            .Caption = "Lote"
            .Width = 100
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
        End With
        With grMovimientos.RootTable.Columns("FechaVenc")
            .Caption = "F. VENC"
            .Width = 100
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
        End With
        If (Lote = False) Then
            grMovimientos.RootTable.Columns("Lote").Visible = False
            grMovimientos.RootTable.Columns("FechaVenc").Visible = False
        End If
        With grMovimientos.RootTable.Columns("est")
            .Caption = ""
            .Width = 0
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            '.CellStyle.BackColor = Color.AliceBlue
        End With
        With grMovimientos.RootTable.Columns("alm")
            .Caption = ""
            .Width = 0
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            '.CellStyle.BackColor = Color.AliceBlue
        End With
        With grMovimientos.RootTable.Columns("id2")
            .Caption = ""
            .Width = 0
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            '.CellStyle.BackColor = Color.AliceBlue
        End With
        With grMovimientos.RootTable.Columns("cprod")
            .Caption = "Código"
            .Width = 80
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            '.CellStyle.BackColor = Color.AliceBlue
            .FormatString = "0.00"
        End With
        With grMovimientos.RootTable.Columns("descProd")
            .Caption = "Producto"
            .Width = 200
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            '.CellStyle.BackColor = Color.AliceBlue
        End With

        With grMovimientos.RootTable.Columns("entrada")
            .Caption = "Ingresos".ToUpper
            .Key = "entrada"
            .Width = 80
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
            .FormatString = "0.00"
        End With
        With grMovimientos.RootTable.Columns("salida")
            .Caption = "SALIDA".ToUpper
            .Width = 80
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
            .FormatString = "0.00"
        End With
        With grMovimientos.RootTable.Columns("saldo")
            .Caption = "Saldo".ToUpper
            .Width = 80
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
            .FormatString = "0.00"
        End With
        With grMovimientos.RootTable.Columns("fechaDocumento")

            .Width = 300
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            .Position = 2
        End With

        'Habilitar Filtradores
        With grMovimientos
            .GroupByBoxVisible = False
            '.FilterRowFormatStyle.BackColor = Color.Blue
            '.DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .AlternatingColors = True
            .RecordNavigator = True
            .RowHeaders = InheritableBoolean.True
        End With
        _prAplicarCondiccionJanus()
    End Sub
    Public Sub _prAplicarCondiccionJanus()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grMovimientos.RootTable.Columns("saldo"), ConditionOperator.LessThan, 0)
        fc.FormatStyle.BackColor = Color.Red
        fc.FormatStyle.ForeColor = Color.White
        fc.FormatStyle.FontBold = TriState.True




        grMovimientos.RootTable.FormatConditions.Add(fc)

    End Sub
    Public Sub _GenerarHistorial()
        If ((Lote = False Or Lote = True)) Then

            If (IdProducto > 0 And cbDeposito.SelectedIndex >= 0) Then
                Dt2KardexTotal = L_fnObtenerHistorialProductoGeneral(IdProducto, cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbDeposito.Value)
                Dt1Kardex = L_fnObtenerHistorialProducto(IdProducto, cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), cbDeposito.Value)
                If (Dt1Kardex.Rows.Count > 0) Then
                    P_CalcularTotalizador()
                Else

                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "No Existe Movimiento Para Los Filtros Establecidos".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                End If
                'Else
                'Dt1Kardex = L_fnObtenerHistorialProducto("-1", tbFechaI.Value.ToString("yyyy/MM/dd"), tbFechaF.Value.ToString("yyyy/MM/dd"), cbAlmacen.Value)   Por Lote
            End If
            'Else
            '    If ((Lote = True And tblote.Text.Length > 0)) Then
            '        If (tbCodigo.Text.Length > 0 And cbAlmacen.SelectedIndex >= 0 And tblote.Text.Length > 0) Then
            '            Dt2KardexTotal = L_fnObtenerHistorialProductoGeneralPorLote(tbCodigo.Text, tbFechaI.Value.ToString("yyyy/MM/dd"), cbAlmacen.Value,
            '                                tblote.Text, tbFechaVenc.Text)
            '            Dt1Kardex = L_fnObtenerHistorialProductoporLote(tbCodigo.Text, tbFechaI.Value.ToString("yyyy/MM/dd"), tbFechaF.Value.ToString("yyyy/MM/dd"), cbAlmacen.Value, tblote.Text, tbFechaVenc.Text)
            '            If (Dt1Kardex.Rows.Count > 0) Then
            '                P_ArmarKardex()
            '            Else
            '                ToastNotification.Show(Me, "No hay kardex para el rango de fecha".ToUpper,
            '                       My.Resources.INFORMATION,
            '                       _DuracionSms * 1000,
            '                       eToastGlowColor.Blue,
            '                       eToastPosition.BottomLeft)
            '            End If
            '        Else
            '            Dt1Kardex = L_fnObtenerHistorialProductoporLote("-1", tbFechaI.Value.ToString("yyyy/MM/dd"), tbFechaF.Value.ToString("yyyy/MM/dd"), cbAlmacen.Value, tblote.Text, tbFechaVenc.Text)
            '        End If
            '    End If
        End If
    End Sub

    Private Sub P_CalcularTotalizador()

        Dim saldo As Double = 0
        Dim ingT As Double = 0
        Dim salT As Double = 0

        If (Not IsDBNull(Dt2KardexTotal.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 2 or concep=4"))) Then
            ingT = Dt2KardexTotal.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 2 or concep=4")
        End If
        If (Not IsDBNull(Dt2KardexTotal.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 1 or concep=3"))) Then
            salT = Dt2KardexTotal.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 1 or concep=3")
        End If
        saldo = ingT + salT
        Dim ing As Double = 0
        Dim sal As Double = 0
        Dim saldoInicial As Double = 0
        'Sumar ingreso de inventario
        ing = IIf(IsDBNull(Dt1Kardex.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 2 or concep=4")), 0, Dt1Kardex.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 2 or concep=4"))
        'Sumar salida de inventario
        sal = IIf(IsDBNull(Dt1Kardex.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 1 or concep=3")), 0, Dt1Kardex.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 1 or concep=3"))
        'Saldo inicial al partir de la fecha indicada
        saldoInicial = saldo '+ sal + ing
        'Insertamos la primera fila con el saldo Inicial
        Dim f As DataRow
        'Dim st As System.Type
        f = Dt1Kardex.NewRow
        f.Item("Id") = 0
        f.Item("fdoc") = cbFechaDesde.Value.ToShortDateString
        f.Item("concep") = 0
        f.Item("descConcep") = "Saldo Inicial"
        f.Item("obs") = "Saldo  A la Fecha " + cbFechaDesde.Value.ToShortDateString
        f.Item("Lote") = ""
        f.Item("est") = 1
        f.Item("alm") = 1
        f.Item("id2") = 0
        f.Item("cprod") = Str(IdProducto)
        f.Item("descProd") = tbProducto.Text
        f.Item("entrada") = 0
        f.Item("salida") = 0
        f.Item("saldo") = saldoInicial
        f.Item("fechaDocumento") = ""

        Dt1Kardex.Rows.InsertAt(f, 0)

        For Each fil As DataRow In Dt1Kardex.Rows
            Dim s As String = fil.Item("concep").ToString
            If (fil.Item("concep").ToString.Equals("1") Or fil.Item("concep").ToString.Equals("3")) Then '
                saldoInicial = saldoInicial + CDbl(fil.Item("entrada"))
                fil.Item("saldo") = saldoInicial
            ElseIf (fil.Item("concep").ToString.Equals("2") Or fil.Item("concep").ToString.Equals("4")) Then
                saldoInicial = saldoInicial + CDbl(fil.Item("salida"))
                fil.Item("saldo") = saldoInicial
            End If
        Next

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
                tbProducto.Text = Str(Row.Cells("Id").Value) + "  " + Row.Cells("NombreProveedor").Value
            tbSaldo.Value = Row.Cells("stock").Value

            cbDeposito.Focus()

            ArmarGrillaMovimientos()

        End If

    End Sub

    Private Sub cbDeposito_ValueChanged(sender As Object, e As EventArgs) Handles cbDeposito.ValueChanged


    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        If (IsDBNull(Dt1Kardex)) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existe Movimiento Para Generar El Reporte".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return

        End If
        If (IsNothing(Dt1Kardex)) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existe Movimiento Para Generar El Reporte".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return

        End If
        If (Dt1Kardex.Rows.Count <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existe Movimiento Para Generar el Reporte".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Return

        End If
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        P_Global.Visualizador = New Visualizador

        Dim objrep As New R_KardexProducto

        objrep.SetDataSource(Dt1Kardex)
        objrep.SetParameterValue("Producto", tbProducto.Text)
        objrep.SetParameterValue("FechaDesde", cbFechaDesde.Value.ToString("dd/MM/yyyy"))
        objrep.SetParameterValue("FechaHasta", cbFechaHasta.Value.ToString("dd/MM/yyyy"))
        objrep.SetParameterValue("Deposito", cbDeposito.Text)
        objrep.SetParameterValue("Total", Dt1Kardex.Rows(Dt1Kardex.Rows.Count - 1).Item("Saldo"))
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(110)
        P_Global.Visualizador.Show() 'Comentar
        ''P_Global.Visualizador.BringToFront() 'Comentar
    End Sub

    Private Sub cbFechaDesde_ValueChanged(sender As Object, e As EventArgs) Handles cbFechaDesde.ValueChanged
        If (IdProducto <= 0) Then

            Return

        End If

        ArmarGrillaMovimientos()
    End Sub

    Private Sub cbFechaHasta_ValueChanged(sender As Object, e As EventArgs) Handles cbFechaHasta.ValueChanged
        If (IdProducto <= 0) Then

            Return

        End If

        ArmarGrillaMovimientos()
    End Sub
End Class