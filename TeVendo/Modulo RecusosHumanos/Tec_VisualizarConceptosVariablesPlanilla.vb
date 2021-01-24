Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports System.Globalization
Public Class Tec_VisualizarConceptosVariablesPlanilla

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public title As String = ""
    Public PlanillaId As Integer = 0

    Dim dtConceptoVariable As DataTable = ListarConceptoVariablePlanilla()
    Public SueldoNeto As Double = 0

    Public dtGeneral As DataTable
    Public PosicionGeneral As Integer

    Public Sub IniciarTodo()

        Dim dt = dtConceptoVariable.DefaultView.ToTable(False, "id", "NombreConcepto")
        P_Global._prCargarComboGenerico(cbConceptoVariable, dt, "id", "Id", "NombreConcepto", "Concepto Variable")
        _habilitarFocus()

    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(cbConceptoVariable, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbMonto, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDescripcion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnRegistrarConceptoVariable, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(grConceptoVariable, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub

    Public Sub CargarTableConceptoPlanilla()
        Dim detalle As DataTable = ListaConceptosPlanilla(PlanillaId, 0) '' TipoConcepto=1  Concepto Fijo
        grConceptoVariable.DataSource = detalle
        grConceptoVariable.RetrieveStructure()
        grConceptoVariable.AlternatingColors = True
        'Id	PlanillaId	ConceptoId	NombreConcepto	Porcentaje	Monto	TipoConcepto	Descripcion	Fecha	estado	eliminar

        With grConceptoVariable.RootTable.Columns("Id")
            .Width = 90
            .Visible = False
        End With
        With grConceptoVariable.RootTable.Columns("PlanillaId")
            .Width = 90
            .Visible = False
        End With
        With grConceptoVariable.RootTable.Columns("ConceptoId")
            .Width = 90
            .Visible = False
        End With

        With grConceptoVariable.RootTable.Columns("NombreConcepto")
            .Width = 250
            .Visible = True
            .Caption = "CONCEPTO VARIABLE"
            .MaxLines = 2
            .WordWrap = True
        End With

        With grConceptoVariable.RootTable.Columns("Porcentaje")
            .Width = 90
            .Visible = False
            .FormatString = "0.00"
            .Caption = "PORCENTAJE %"

        End With

        'Id	PlanillaId	ConceptoId	NombreConcepto	Porcentaje	Monto	TipoConcepto	Descripcion	Fecha	estado	eliminar


        With grConceptoVariable.RootTable.Columns("Monto")
            .Width = 110
            .Caption = "Monto"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grConceptoVariable.RootTable.Columns("TipoConcepto")
            .Width = 90
            .Visible = False
        End With
        With grConceptoVariable.RootTable.Columns("Descripcion")
            .Width = 300
            .Visible = True
            .MaxLines = 2
            .WordWrap = True
            .Caption = "Descripcion"
        End With
        With grConceptoVariable.RootTable.Columns("Fecha")
            .Width = 90
            .Visible = True
            .Caption = "Fecha Registro"
            .FormatString = "dd/MM/yyyy"
        End With
        With grConceptoVariable.RootTable.Columns("estado")
            .Width = 90
            .Visible = False
        End With
        With grConceptoVariable.RootTable.Columns("eliminar")
            .Width = 90
            .Visible = True
            .Caption = "Eliminar"
            .TopMargin = 5
            .LeftMargin = 7
            .BottomMargin = 5
        End With
        With grConceptoVariable
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

        CargarIcono()
    End Sub

    Public Sub CargarIcono()
        Dim dt As DataTable = CType(grConceptoVariable.DataSource, DataTable)
        Dim Bin As New MemoryStream
        Dim imgDelete As New Bitmap(My.Resources.rowdelete, 30, 28)
        imgDelete.Save(Bin, Imaging.ImageFormat.Png)
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            CType(grConceptoVariable.DataSource, DataTable).Rows(i).Item("eliminar") = Bin.GetBuffer()

        Next

    End Sub
    Private Sub Tec_VisualizarConceptoFijosPlanilla_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lbtitle.Text = "Ver Conceptos Variables De: " + title + "  Sueldo Neto = " + Str(SueldoNeto)
        CargarTableConceptoPlanilla()
        IniciarTodo()
    End Sub


    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Me.Close()

    End Sub

    Private Sub LabelX1_Click(sender As Object, e As EventArgs) Handles LabelX1.Click

    End Sub

    Private Sub btnRegistrarConceptoVariable_Click(sender As Object, e As EventArgs) Handles btnRegistrarConceptoVariable.Click
        If (tbMonto.Text.Trim = String.Empty) Then
            ToastNotification.Show(Me, "Ingrese un Monto Valido", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbMonto.Focus()
            Return
        Else
            If (tbMonto.Value <= 0) Then
                ToastNotification.Show(Me, "Ingrese un Monto Valido Mayor a 0", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                tbMonto.Focus()
                Return
            End If
        End If
        If (cbConceptoVariable.SelectedIndex < 0) Then
            ToastNotification.Show(Me, "Seleccione un Concepto Variable a Registrar", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbConceptoVariable.Focus()
            Return
        End If

        Dim OperacionConcepto As Integer = ObtenerOperacionConcepto(cbConceptoVariable.Value)
        If (OperacionConcepto = -1) Then

            If (tbMonto.Value > SueldoNeto) Then
                ToastNotification.Show(Me, "No Puede Excederse Del Sueldo Neto = " + Str(SueldoNeto), img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                tbMonto.Focus()
                Return
            End If


        End If


        'Id  PlanillaId	ConceptoId	NombreConcepto	Porcentaje	Monto	TipoConcepto	Descripcion	Fecha	estado	eliminar

        Dim Bin As New MemoryStream
        Dim imgDelete As New Bitmap(My.Resources.rowdelete, 30, 28)
        imgDelete.Save(Bin, Imaging.ImageFormat.Png)
        CType(grConceptoVariable.DataSource, DataTable).Rows.Add(_GenerarId() + 1, PlanillaId, cbConceptoVariable.Value, cbConceptoVariable.Text, 0, tbMonto.Value * OperacionConcepto, 0, tbDescripcion.Text, Now.Date(), 0, Bin.GetBuffer())

        Dim bandera As Boolean = ModificarPlanillaSoloDetalle(PlanillaId, CType(grConceptoVariable.DataSource, DataTable))
        If (bandera = True) Then

            CargarTableConceptoPlanilla()

            SueldoNeto = SueldoNeto + (tbMonto.Value * OperacionConcepto)
            tbMonto.Value = 0
            lbtitle.Text = "Ver Conceptos Variables De: " + title + "  Sueldo Neto = " + Str(SueldoNeto)
            ToastNotification.Show(Me, "Concepto Variable Asignado Correctamente".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            dtGeneral.Rows(PosicionGeneral).Item("SueldoNeto") = SueldoNeto
        Else
            ToastNotification.Show(Me, "error al Guardar El Concepto Variable", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If

    End Sub

    Public Function _GenerarId()
        Dim dt As DataTable = CType(grConceptoVariable.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(grConceptoVariable.DataSource, DataTable).Rows(i).Item("Id")), 0, CType(grConceptoVariable.DataSource, DataTable).Rows(i).Item("Id"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function

    Public Function ObtenerOperacionConcepto(ConceptoId As Integer) As Integer

        For i As Integer = 0 To dtConceptoVariable.Rows.Count - 1 Step 1

            If (dtConceptoVariable.Rows(i).Item("id") = ConceptoId) Then
                Return dtConceptoVariable.Rows(i).Item("Operacion")

            End If
        Next
        Return 0

    End Function

    Private Sub grConceptoVariable_Click(sender As Object, e As EventArgs) Handles grConceptoVariable.Click
        Try
            If (grConceptoVariable.RowCount >= 1 And grConceptoVariable.Row >= 0) Then
                If (grConceptoVariable.CurrentColumn.Index = grConceptoVariable.RootTable.Columns("eliminar").Index) Then

                    Dim ef = New Efecto


                    ef.tipo = 3
                    ef.titulo = "Confirmación de Eliminación"
                    ef.descripcion = "¿Esta Seguro de Eliminar el concepto Variable " + grConceptoVariable.GetValue("NombreConcepto") + " Con Monto = " + Str(grConceptoVariable.GetValue("Monto")) + " ?"
                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then
                        Dim mensajeError As String = ""
                        Dim res As Boolean
                        Try
                            Dim posicion As Integer = PosicionItem(grConceptoVariable.GetValue("id"))
                            CType(grConceptoVariable.DataSource, DataTable).Rows(posicion).Item("estado") = -1
                            res = ModificarPlanillaSoloDetalle(PlanillaId, CType(grConceptoVariable.DataSource, DataTable))
                            If res Then



                                SueldoNeto = SueldoNeto - (grConceptoVariable.GetValue("Monto"))
                                dtGeneral.Rows(PosicionGeneral).Item("SueldoNeto") = SueldoNeto
                                lbtitle.Text = "Ver Conceptos Variables De: " + title + "  Sueldo Neto = " + Str(SueldoNeto)
                                CargarTableConceptoPlanilla()
                                ToastNotification.Show(Me, "Concepto Variable Asignado Correctamente".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                            Else
                                ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                            End If
                        Catch ex As Exception
                            ToastNotification.Show(Me, "Error al eliminar el Contrato".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

                        End Try

                    End If


                End If

            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Function PosicionItem(id As Integer) As Integer
        Dim dt As DataTable = CType(grConceptoVariable.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("Id") = id) Then
                Return i
            End If

        Next
        Return -1
    End Function
End Class