

Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls
Public Class Tec_CajaGeneral
    Dim Dt1Kardex As DataTable
    Dim Dt2KardexTotal As DataTable
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Public FilaSeleccionada As Boolean = False

    Dim Bandera As Boolean = True
    Public Sub IniciarProceso()

        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date
        ArmarGrillaMovimientos()
        Me.Text = "Historial De Movimientos Caja General"

    End Sub
    Private Sub Tec_KardexProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarProceso()

    End Sub
    Public Sub ArmarGrillaMovimientos()

        Dt1Kardex = New DataTable
        Dt2KardexTotal = New DataTable
        _GenerarHistorial()

        grMovimientos.BoundMode = Janus.Data.BoundMode.Bound
        grMovimientos.DataSource = Dt1Kardex
        grMovimientos.RetrieveStructure()

        'Id	IngresoEgreso	Fecha	CajatipoMovimientoId	TipoMovimiento	Descripcion	Ingreso	Egreso	Saldo
        With grMovimientos.RootTable.Columns("Id")
            .Caption = "ID"
            .Width = 70
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
        End With
        With grMovimientos.RootTable.Columns("IngresoEgreso")
            .Caption = ""
            .Width = 0
            .Visible = False
            '.CellStyle.BackColor = Color.AliceBlue
        End With

        With grMovimientos.RootTable.Columns("CajatipoMovimientoId")
            .Width = 100
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            '.CellStyle.BackColor = Color.AliceBlue
        End With
        With grMovimientos.RootTable.Columns("Fecha")
            .Caption = "Fecha Transaccion".ToUpper
            .Width = 120
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            '.CellStyle.BackColor = Color.AliceBlue
        End With



        With grMovimientos.RootTable.Columns("TipoMovimiento")
            .Caption = "Tipo Movimiento".ToUpper
            .Width = 180
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .WordWrap = True
            .MaxLines = 2
            '.CellStyle.BackColor = Color.AliceBlue
        End With


        With grMovimientos.RootTable.Columns("Descripcion")
            .Caption = "Descripcion Movimiento".ToUpper
            .Width = 300
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .CellStyle.FontSize = 9
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .WordWrap = True
            .MaxLines = 2
            '.CellStyle.BackColor = Color.AliceBlue
        End With

        ''''''''''''''


        With grMovimientos.RootTable.Columns("Ingreso")
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
        With grMovimientos.RootTable.Columns("Egreso")
            .Caption = "Egresos".ToUpper
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
        Dim dt As DataTable = L_ObtenerConfiguraciones()

        If (dt.Rows.Count > 0) Then
            tbSaldo.Value = dt.Rows(0).Item("TotalCajaGeneral")
        End If
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

        Dt2KardexTotal = L_fnObtenerHistorialCajaGeneral(cbFechaDesde.Value.ToString("yyyy/MM/dd"))
        Dt1Kardex = L_fnObtenerHistorialCajaGeneralDesdeHasta(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))
        If (Dt1Kardex.Rows.Count > 0) Then
            P_CalcularTotalizador()
        Else
            If (Bandera = False) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "No Existe Movimiento Para Los Filtros Establecidos".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Else
                Bandera = False
            End If

        End If
        'Else
        'Dt1Kardex = L_fnObtenerHistorialProducto("-1", tbFechaI.Value.ToString("yyyy/MM/dd"), tbFechaF.Value.ToString("yyyy/MM/dd"), cbAlmacen.Value)   Por Lote


    End Sub

    Private Sub P_CalcularTotalizador()

        Dim saldo As Double = 0
        Dim ingT As Double = 0
        Dim salT As Double = 0

        If (Not IsDBNull(Dt2KardexTotal.Compute("Sum(Ingreso)", "1=1"))) Then
            ingT = Dt2KardexTotal.Compute("Sum(Ingreso)", "1=1")
        End If
        If (Not IsDBNull(Dt2KardexTotal.Compute("Sum(Egreso)", "1=1"))) Then
            salT = Dt2KardexTotal.Compute("Sum(Egreso)", "1=1")
        End If
        saldo = ingT + salT
        Dim ing As Double = 0
        Dim sal As Double = 0
        Dim saldoInicial As Double = 0
        'Sumar ingreso de inventario
        ing = IIf(IsDBNull(Dt1Kardex.Compute("Sum(Ingreso)", "1=1")), 0, Dt1Kardex.Compute("Sum(Ingreso)", "1=1"))
        'Sumar salida de inventario
        sal = IIf(IsDBNull(Dt1Kardex.Compute("Sum(Egreso)", "1=1")), 0, Dt1Kardex.Compute("Sum(Egreso)", "1=1"))
        'Saldo inicial al partir de la fecha indicada
        saldoInicial = saldo '+ sal + ing
        'Insertamos la primera fila con el saldo Inicial
        Dim f As DataRow
        'Dim st As System.Type
        f = Dt1Kardex.NewRow
        f.Item("Id") = 0
        f.Item("Fecha") = cbFechaDesde.Value.ToShortDateString
        f.Item("CajatipoMovimientoId") = 0
        f.Item("TipoMovimiento") = "Saldo Inicial"
        f.Item("Descripcion") = "Saldo  A la Fecha " + cbFechaDesde.Value.ToShortDateString
        f.Item("Ingreso") = 0
        f.Item("IngresoEgreso") = 1
        f.Item("Egreso") = 0
        f.Item("Saldo") = saldoInicial

        Dt1Kardex.Rows.InsertAt(f, 0)

        For Each fil As DataRow In Dt1Kardex.Rows

            If (fil.Item("IngresoEgreso") = 0) Then '
                saldoInicial = saldoInicial + CDbl(fil.Item("Egreso"))
                fil.Item("Saldo") = saldoInicial
            ElseIf (fil.Item("IngresoEgreso") = 1) Then
                saldoInicial = saldoInicial + CDbl(fil.Item("Ingreso"))
                fil.Item("Saldo") = saldoInicial
            End If
        Next

    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Private Sub btGenerarKardex_Click(sender As Object, e As EventArgs) Handles btGenerarKardex.Click
        ArmarGrillaMovimientos()
    End Sub
End Class