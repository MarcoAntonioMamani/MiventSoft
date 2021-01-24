Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports System.Globalization

Public Class Tec_Listarplanilla
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)

    Dim banderaGrilla = False
    Private Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)


        P_Global._prCargarComboGenerico(cbAnio, L_prLibreriaDetalleGeneral(13), "cnnum", "Codigo", "cndesc1", "Año")


        P_Global._prCargarComboGenerico(cbMes, L_prLibreriaDetalleGeneral(12), "cnnum", "Codigo", "cndesc1", "Mes")
        cbAnio.SelectedIndex = CType(cbAnio.DataSource, DataTable).Rows.Count - 1
        Dim MesActual As String = MonthName(Month(Now.Date)).ToUpper
        cbMes.SelectedIndex = IIf(ObtenerFilaMes(MesActual) < 0, 0, ObtenerFilaMes(MesActual))
        banderaGrilla = True
        _prCargarTablaPlanilla()


    End Sub

    Private Sub _prCargarTablaPlanilla()
        Dim dt As New DataTable
        If (cbAnio.SelectedIndex < 0 Or cbMes.SelectedIndex < 0) Then
            ToastNotification.Show(Me, "Debe Seleccionar Mes y Anio como Campos Obligatorios ", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

        Dim monthName = cbMes.Text
        Dim monthNumber = DateTime.ParseExact(monthName, "MMMM", CultureInfo.CurrentCulture).Month
        If (chktodos.Checked = True) Then
            dt = ListarPlanilla(cbMes.Value, cbAnio.Value, 1)

        Else
            dt = ListarPlanilla(cbMes.Value, cbAnio.Value, 0)
        End If



        grplanilla.DataSource = dt
        grplanilla.RetrieveStructure()
        grplanilla.AlternatingColors = True
        'fila	PlanillaId	Anio	Mes	Trabajador	InicioContrato	FinContrato	Cargo	Sueldo	ConceptoFijos	SueldoBruto	ConceptoVariable	SueldoNeto	Reporte

        With grplanilla.RootTable.Columns("fila")
            .Width = 30
            .Visible = True
            .Caption = "Nº"
        End With
        With grplanilla.RootTable.Columns("PlanillaId")
            .Width = 90
            .Visible = False
        End With


        With grplanilla.RootTable.Columns("Anio")
            .Width = 50
            .Visible = True
            .Caption = "Año"
        End With
        With grplanilla.RootTable.Columns("Mes")
            .Width = 50
            .Visible = True
            .Caption = "Mes"
        End With
        With grplanilla.RootTable.Columns("Trabajador")
            .Width = 220
            .Visible = True
            .Caption = "Trabajador"
        End With
        'fila	PlanillaId	Anio	Mes	Trabajador	InicioContrato	FinContrato	Cargo	Sueldo	ConceptoFijos	SueldoBruto	ConceptoVariable	SueldoNeto	Reporte

        With grplanilla.RootTable.Columns("InicioContrato")
            .Width = 100
            .Visible = True
            .Caption = "Inicio Contrato"
            .FormatString = "dd/MM/yyyy"
        End With
        With grplanilla.RootTable.Columns("FinContrato")
            .Width = 90
            .Visible = True
            .Caption = "Fin Contrato"
            .FormatString = "dd/MM/yyyy"
        End With
        With grplanilla.RootTable.Columns("Cargo")
            .Width = 120
            .Visible = True
            .Caption = "Cargo"
        End With
        'fila	PlanillaId	Anio	Mes	Trabajador	InicioContrato	FinContrato	Cargo	Sueldo	ConceptoFijos	SueldoBruto	ConceptoVariable	SueldoNeto	Reporte
        With grplanilla.RootTable.Columns("Sueldo")
            .Width = 80
            .Caption = "Sueldo"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grplanilla.RootTable.Columns("ConceptoFijos")
            .Width = 90
            .Visible = True
            .Caption = "C.Fijos"
        End With
        With grplanilla.RootTable.Columns("SueldoBruto")
            .Width = 100
            .Caption = "Sueldo Bruto"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grplanilla.RootTable.Columns("ConceptoVariable")
            .Width = 90
            .Visible = True
            .Caption = "C.Variable"
        End With
        With grplanilla.RootTable.Columns("SueldoNeto")
            .Width = 100
            .Caption = "Sueldo Neto"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grplanilla.RootTable.Columns("Reporte")
            .Width = 90
            .Visible = True
            .Caption = "Reporte"
        End With
        With grplanilla
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
    Public Function ObtenerFilaMes(Mes As String)

        Dim dt As DataTable = CType(cbMes.DataSource, DataTable)
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (Mes.Contains(dt.Rows(i).Item("cndesc1"))) Then
                Return i
            End If

        Next
        Return -1
    End Function
    Private Sub Tec_Listarplanilla_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Private Sub chktodos_CheckedChanged(sender As Object, e As EventArgs) Handles chktodos.CheckedChanged
        If (chktodos.Checked = True) Then
            cbAnio.Enabled = False
            cbMes.Enabled = False
        Else
            cbAnio.Enabled = True
            cbMes.Enabled = True
        End If
    End Sub

    Private Sub cbAnio_ValueChanged(sender As Object, e As EventArgs) Handles cbAnio.ValueChanged
        If (banderaGrilla = True) Then
            _prCargarTablaPlanilla()
        End If
    End Sub

    Private Sub cbMes_ValueChanged(sender As Object, e As EventArgs) Handles cbMes.ValueChanged
        If (banderaGrilla = True) Then
            _prCargarTablaPlanilla()
        End If
    End Sub
End Class