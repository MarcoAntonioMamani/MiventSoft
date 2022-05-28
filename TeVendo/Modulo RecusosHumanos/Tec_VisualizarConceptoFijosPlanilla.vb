Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports System.Globalization
Public Class Tec_VisualizarConceptoFijosPlanilla

    Public title As String = ""
    Public PlanillaId As Integer = 0



    Public Sub CargarTableConceptoPlanilla()
        Dim detalle As DataTable = ListaConceptosPlanilla(PlanillaId, 1) '' TipoConcepto=1  Concepto Fijo
        grConceptoFijo.DataSource = detalle
        grConceptoFijo.RetrieveStructure()
        grConceptoFijo.AlternatingColors = True
        'Id	PlanillaId	ConceptoId	NombreConcepto	Porcentaje	Monto	TipoConcepto	Descripcion	Fecha	estado	eliminar

        With grConceptoFijo.RootTable.Columns("Id")
            .Width = 90
            .Visible = False
        End With
        With grConceptoFijo.RootTable.Columns("PlanillaId")
            .Width = 90
            .Visible = False
        End With
        With grConceptoFijo.RootTable.Columns("ConceptoId")
            .Width = 90
            .Visible = False
        End With

        With grConceptoFijo.RootTable.Columns("NombreConcepto")
            .Width = 250
            .Visible = True
            .Caption = "CONCEPTO FIJO"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grConceptoFijo.RootTable.Columns("Porcentaje")
            .Width = 90
            .Visible = True
            .FormatString = "0.00"
            .Caption = "PORCENTAJE %"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .AggregateFunction = AggregateFunction.Sum
        End With

        'Id	PlanillaId	ConceptoId	NombreConcepto	Porcentaje	Monto	TipoConcepto	Descripcion	Fecha	estado	eliminar


        With grConceptoFijo.RootTable.Columns("Monto")
            .Width = 110
            .Caption = "Monto"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grConceptoFijo.RootTable.Columns("TipoConcepto")
            .Width = 90
            .Visible = False
        End With
        With grConceptoFijo.RootTable.Columns("Descripcion")
            .Width = 90
            .Visible = False
        End With
        With grConceptoFijo.RootTable.Columns("Fecha")
            .Width = 90
            .Visible = False
        End With
        With grConceptoFijo.RootTable.Columns("estado")
            .Width = 90
            .Visible = False
        End With
        With grConceptoFijo.RootTable.Columns("eliminar")
            .Width = 90
            .Visible = False
        End With
        With grConceptoFijo
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .CellToolTipText = "Conceptos"
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed

        End With


    End Sub
    Private Sub Tec_VisualizarConceptoFijosPlanilla_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lbtitle.Text = title
        CargarTableConceptoPlanilla()
    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        Me.Close()

    End Sub


End Class