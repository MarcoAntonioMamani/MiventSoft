Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Public Class Tec_AdministrarCuentasPorPagar

#Region "General"
    Private Sub Tec_AdministrarCuentasPorPagar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarProceso()
    End Sub
    Public Sub IniciarProceso()
        _prCargarPagosPendientes()

    End Sub
#End Region

#Region "Pagos Pendientes"

    Private Sub _prCargarPagosPendientes()
        Dim dt As New DataTable
        dt = L_prListarPagosPendientes()
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

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click

    End Sub
#End Region


End Class