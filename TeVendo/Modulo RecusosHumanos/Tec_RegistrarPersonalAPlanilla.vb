Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports System.Globalization

Public Class Tec_RegistrarPersonalAPlanilla
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)

    Dim banderaGrilla = False
    Private Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)

        Me.Text = "Gestion De Contratos"
        P_Global._prCargarComboGenerico(cbAnio, L_prLibreriaDetalleGeneral(13), "cnnum", "Codigo", "cndesc1", "Año")


        P_Global._prCargarComboGenerico(cbMes, L_prLibreriaDetalleGeneral(12), "cnnum", "Codigo", "cndesc1", "Mes")
        cbAnio.SelectedIndex = CType(cbAnio.DataSource, DataTable).Rows.Count - 1
        Dim MesActual As String = MonthName(Month(Now.Date)).ToUpper
        cbMes.SelectedIndex = IIf(ObtenerFilaMes(MesActual) < 0, 0, ObtenerFilaMes(MesActual))
        banderaGrilla = True
        _prCargarTablaPersonalSinPlanilla()

        Me.Text = "Registrar Personal A Planilla"
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

    Private Sub _prCargarTablaPersonalSinPlanilla()
        Dim dt As New DataTable
        If (cbAnio.SelectedIndex < 0 Or cbMes.SelectedIndex < 0) Then
            ToastNotification.Show(Me, "Debe Seleccionar Mes y Anio como Campos Obligatorios ", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

        Dim monthName = cbMes.Text
        Dim monthNumber = DateTime.ParseExact(monthName, "MMMM", CultureInfo.CurrentCulture).Month
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

        If (CType(grContrato.DataSource, DataTable).Rows.Count > 0) Then
            btnRegistrarPlanilla.Visible = True
        Else
            btnRegistrarPlanilla.Visible = False
        End If

    End Sub

    Private Sub Tec_RegistrarPersonalAPlanilla_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
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

    Private Sub cbMes_ValueChanged(sender As Object, e As EventArgs) Handles cbMes.ValueChanged
        If (banderaGrilla = True) Then
            _prCargarTablaPersonalSinPlanilla()
        End If

    End Sub

    Private Sub cbAnio_ValueChanged(sender As Object, e As EventArgs) Handles cbAnio.ValueChanged
        If (banderaGrilla = True) Then
            _prCargarTablaPersonalSinPlanilla()
        End If
    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles btnRegistrarPlanilla.Click
        If (CType(grContrato.DataSource, DataTable).Rows.Count <= 0) Then
            ToastNotification.Show(Me, "No Existe Personal Para Asignar a Planilla", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If
        Dim dt As DataTable = CType(grContrato.DataSource, DataTable)


        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            ''Armando Cabezera de Planilla
            Dim ContratoId As Integer = dt.Rows(i).Item("ContratoId")
            Dim Anio As Integer = CInt(cbAnio.Text)
            Dim monthName As String = cbMes.Text
            Dim Mes As Integer = DateTime.ParseExact(monthName, "MMMM", CultureInfo.CurrentCulture).Month
            Dim Sueldo As Double = dt.Rows(i).Item("SueldoBase")
            Dim SueldoBruto As Double = dt.Rows(i).Item("SueldoBruto")
            Dim SueldoNeto As Double = SueldoBruto
            '' Armando conceptos Fijos
            Dim detalle As DataTable = ListaConceptosPlanilla(-1)
            Dim dtconcepto As DataTable = ListaConceptosContratos(ContratoId)
            For j As Integer = 0 To dtconcepto.Rows.Count - 1 Step 1
                Dim ConceptoId As Integer = dtconcepto.Rows(j).Item("ConceptoId")
                Dim NombreConcepto As String = dtconcepto.Rows(j).Item("NombreConcepto")
                Dim Porcentaje As Double = dtconcepto.Rows(j).Item("PorcentajeConcepto")
                Dim Monto As Double = Sueldo * (Porcentaje / 100)
                Dim TipoConcepto As Integer = 1 ''1=Concepto Fijos  0= Concepto Variable

                detalle.Rows.Add(j, j, ConceptoId, NombreConcepto, Porcentaje, Monto, TipoConcepto, "", Now.Date(), 0)
            Next
            Dim id As String = ""
            'InsertarPlanilla(_Id As String, ContratoId As Integer, Anio As Integer, Mes As Integer, Sueldo As Double, SueldoBruto As Double, SueldoNeto As Double, dtdetalle As DataTable)
            InsertarPlanilla(id, ContratoId, cbAnio.Value, cbMes.Value, Sueldo, SueldoBruto, SueldoNeto, detalle)


        Next
        _prCargarTablaPersonalSinPlanilla()
        ToastNotification.Show(Me, "Trabajadores Asignados a Planilla Correctamente ".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)



    End Sub
End Class