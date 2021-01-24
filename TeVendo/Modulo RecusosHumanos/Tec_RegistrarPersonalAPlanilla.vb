Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO

Public Class Tec_RegistrarPersonalAPlanilla

    Private Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)

        Me.Text = "Gestion De Contratos"
        P_Global._prCargarComboGenerico(cbAnio, L_prLibreriaDetalleGeneral(13), "cnnum", "Codigo", "cndesc1", "Año")


        P_Global._prCargarComboGenerico(cbMes, L_prLibreriaDetalleGeneral(12), "cnnum", "Codigo", "cndesc1", "Mes")
        _prCargarTablaPersonalSinPlanilla()

    End Sub

    Private Sub _prCargarTablaPersonalSinPlanilla()
        Dim dt As New DataTable
        If (cbAnio.SelectedIndex < 0 Or cbMes.SelectedIndex < 0) Then
            ToastNotification.Show(Me, "Error al guardar el Contrato".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If

        dt = ListarPersonalSinPlanilla(cbMes.Value, cbAnio.Value)


        grContrato.DataSource = dt
        grContrato.RetrieveStructure()
        grContrato.AlternatingColors = True
        'IdPersonal	Personal	ContratoId	InicioContrato	FinContrato	Cargo	SueldoBase	SueldoBruto

        With grContrato.RootTable.Columns("IdPersonal")
            .Width = 90
            .Visible = False
        End With

        With grContrato.RootTable.Columns("Personal")
            .Width = 300
            .Visible = True
            .Caption = "TRABAJADOR"
        End With
        With grContrato.RootTable.Columns("ContratoId")
            .Width = 90
            .Visible = False
        End With
        With grContrato.RootTable.Columns("InicioContrato")
            .Width = 90
            .Visible = True
            .Caption = "Inicio Contrato"
            .FormatString = "dd/MM/yyyy"
        End With
        With grContrato.RootTable.Columns("FinContrato")
            .Width = 90
            .Visible = True
            .Caption = "Fin Contrato"
            .FormatString = "dd/MM/yyyy"
        End With
        With grContrato.RootTable.Columns("Cargo")
            .Width = 120
            .Visible = True
            .Caption = "Cargo"
        End With
        With grContrato.RootTable.Columns("SueldoBase")
            .Width = 110
            .Caption = "Sueldo Base"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grContrato.RootTable.Columns("SueldoBruto")
            .Width = 110
            .Caption = "Sueldo Bruto"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grContrato
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

    Private Sub Tec_RegistrarPersonalAPlanilla_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub btnAnio_Click(sender As Object, e As EventArgs) Handles btnAnio.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 13
        ef.titulo = "Crear Gestion(Año)"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbAnio, L_prLibreriaDetalleGeneral(13), "cnnum", "Codigo", "cndesc1", "Año")
            cbAnio.SelectedIndex = CType(cbAnio.DataSource, DataTable).Rows.Count - 1
            cbAnio.Focus()
        End If
    End Sub
End Class