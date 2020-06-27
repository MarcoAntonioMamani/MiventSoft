Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Public Class Rep_MovimientosProductos
    'gb_FacturaIncluirICE 
    Public _nameButton As String
    Public _tab As SuperTabItem
    Dim Lote As Boolean = False
    Dim Dt1Kardex As DataTable
    Dim Dt2KardexTotal As DataTable
    Public Sub _prIniciarTodo()
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date

        Me.Text = "REPORTE DE MOVIMIENTO DE PRODUCTOS"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        P_Global._prCargarComboGenerico(cbDeposito, L_prListarDepositos(), "Id", "Codigo", "NombreDeposito", "NombreDeposito")

    End Sub

    Public Sub _prObtenerKardexGeneral(ByRef _dt As DataTable)
        Dim dtKardexGeneral As DataTable = L_fnListarProductosQueTienenMovimientos(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), cbDeposito.Value) ''Aqui obtengo todos los productos con movimientos


        '' as SaldoAnterior,
        '' as Entradas,'' as Salidas,'' as SaldoFinal
        For i As Integer = 0 To dtKardexGeneral.Rows.Count - 1 Step 1
            Dim numipro As Integer = dtKardexGeneral.Rows(i).Item("Id")
            Dim saldoAnt As String = ""
            Dim Entrada As String = ""
            Dim Salida As String = ""
            Dim SaldoFinal As String = ""
            Dim dtkardex As DataTable = P_ArmarGrillaDatosGeneral(numipro, cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), cbDeposito.Value, saldoAnt, Entrada, Salida, SaldoFinal)
            dtKardexGeneral.Rows(i).Item("SaldoAnterior") = saldoAnt
            dtKardexGeneral.Rows(i).Item("Entradas") = Entrada
            dtKardexGeneral.Rows(i).Item("Salidas") = Salida
            dtKardexGeneral.Rows(i).Item("SaldoFinal") = SaldoFinal
        Next
        _dt = dtKardexGeneral
    End Sub

    Private Function P_ArmarGrillaDatosGeneral(codprod As Integer, fechaI As String, fechaF As String, almacen As Integer, ByRef SaldoAnt As String,
                                              ByRef Entradas As String, ByRef Salidas As String,
                                              ByRef SaldoFinal As String) As DataTable
        Dim Dt1Kardex = New DataTable
        Dim Dt2KardexTotal = New DataTable

        Dt2KardexTotal = L_fnObtenerHistorialProductoGeneral(codprod, fechaI, almacen)
        Dt1Kardex = L_fnObtenerHistorialProducto(codprod, fechaI, fechaF, almacen)
        If (Dt1Kardex.Rows.Count > 0) Then
            P_CalcularTotalizador(Dt1Kardex, Dt2KardexTotal, codprod,
                          SaldoAnt, Entradas, Salidas, SaldoFinal)

        End If
        Return Dt1Kardex
    End Function

    Private Sub P_CalcularTotalizador(ByRef Dt1Kardex As DataTable, ByRef Dt2KardexTotal As DataTable,
                                  IdProducto As Integer, ByRef SaldoAnt As String,
                                 ByRef Entradas As String, ByRef Salidas As String, ByRef SaldoFinal As String)

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
        ing = IIf(IsDBNull(Dt1Kardex.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 2 or concep=4")), 0, Dt1Kardex.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 2 or concep=4"))
        'Sumar salida de inventario
        sal = IIf(IsDBNull(Dt1Kardex.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 1 or concep=3")), 0, Dt1Kardex.Compute("Sum(entrada)+Sum(salida)", "cprod = " + Str(IdProducto) + " and concep = 1 or concep=3"))
        SaldoAnt = Str(saldo).Trim
        Entradas = Str(ing).Trim
        Salidas = Str(Math.Abs(sal)).Trim
        SaldoFinal = Str(Math.Abs((ing + saldo) + sal)).Trim

    End Sub
    Private Sub Rep_MovimientosProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click

        If (cbDeposito.SelectedIndex < 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Debe Seleccionar un Deposito".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbDeposito.Focus()
            Return

        End If

        Dim _dt As New DataTable
        _prObtenerKardexGeneral(_dt)
        If (_dt.Rows.Count > 0) Then

            Dim objrep As New Reporte_KardexGeneralProductos
            objrep.SetDataSource(_dt)
            Dim fechaI As String = cbFechaDesde.Value.ToString("dd/MM/yyyy")
            Dim fechaF As String = cbFechaHasta.Value.ToString("dd/MM/yyyy")
            objrep.SetParameterValue("Deposito", cbDeposito.Text)
            objrep.SetParameterValue("FechaDesde", fechaI)
            objrep.SetParameterValue("FechaHasta", fechaF)
            MReportViewer.ReportSource = objrep
            MReportViewer.Show()
            MReportViewer.BringToFront()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar. con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If

    End Sub
End Class