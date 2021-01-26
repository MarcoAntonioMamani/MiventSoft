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
            .LeftMargin = 10
            .TopMargin = 10
            .BottomMargin = 10
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

        CargarIconos()

    End Sub

    Public Sub CargarIconos()
        Dim BinCFijos As New MemoryStream
        Dim imgCFijos As New Bitmap(My.Resources.cfijos, 40, 35)
        imgCFijos.Save(BinCFijos, Imaging.ImageFormat.Png)

        Dim BinCVariable As New MemoryStream
        Dim imgCVariable As New Bitmap(My.Resources.cvariables, 40, 35)
        imgCVariable.Save(BinCVariable, Imaging.ImageFormat.Png)

        Dim BinReporte As New MemoryStream
        Dim imgReporte As New Bitmap(My.Resources.printerplanilla, 40, 35)
        imgReporte.Save(BinReporte, Imaging.ImageFormat.Png)

        Dim dt As DataTable = CType(grplanilla.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1


            CType(grplanilla.DataSource, DataTable).Rows(i).Item("ConceptoFijos") = BinCFijos.GetBuffer
            CType(grplanilla.DataSource, DataTable).Rows(i).Item("ConceptoVariable") = BinCVariable.GetBuffer
            CType(grplanilla.DataSource, DataTable).Rows(i).Item("Reporte") = BinReporte.GetBuffer
        Next

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
            If (banderaGrilla = True) Then
                _prCargarTablaPlanilla()
            End If
        Else
            cbAnio.Enabled = True
            cbMes.Enabled = True
            If (banderaGrilla = True) Then
                _prCargarTablaPlanilla()
            End If
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

    Public Function ObtenerFila(id As Integer) As Integer

        Dim dt As DataTable = CType(grplanilla.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("PlanillaId") = id) Then

                Return i
            End If

        Next
        Return -1
    End Function


    Private Sub grplanilla_Click(sender As Object, e As EventArgs) Handles grplanilla.Click
        Try
            If (grplanilla.RowCount >= 1 And grplanilla.Row >= 0) Then
                If (grplanilla.CurrentColumn.Index = grplanilla.RootTable.Columns("ConceptoFijos").Index) Then

                    Dim numi As String = ""
                    Dim ef = New Efecto
                    ef.tipo = 11
                    ef.PlanillaId = grplanilla.GetValue("PlanillaId")
                    ef.titulo = "Ver Conceptos Fijos De: " + grplanilla.GetValue("Trabajador")
                    ef.ShowDialog()

                End If
                If (grplanilla.CurrentColumn.Index = grplanilla.RootTable.Columns("ConceptoVariable").Index) Then
                    Dim Posicion As Integer = ObtenerFila(grplanilla.GetValue("PlanillaId"))
                    Dim numi As String = ""
                    Dim ef = New Efecto
                    ef.tipo = 12
                    ef.PlanillaId = grplanilla.GetValue("PlanillaId")
                    ef.SueldoNeto = grplanilla.GetValue("SueldoNeto")
                    ef.titulo = grplanilla.GetValue("Trabajador")
                    ef.Fila = Posicion
                    ef.dtGeneral = CType(grplanilla.DataSource, DataTable)
                    ef.ShowDialog()

                End If
                If (grplanilla.CurrentColumn.Index = grplanilla.RootTable.Columns("Reporte").Index) Then
                    GenerarReporteIndividual(grplanilla.GetValue("PlanillaId"))
                End If
                ''Reporte
            End If
        Catch ex As Exception

        End Try
    End Sub


    Public Sub GenerarReporteIndividual(id As Integer)
        Dim dt As DataTable = ReporteBoletaSalarioIndividual(id)

        Dim dtImage As DataTable = ObtenerImagenEmpresa()
        Dim NombreEmpresa As String = dtImage.Rows(0).Item("Nombre")
        Dim Direccion As String = dtImage.Rows(0).Item("Direccion")
        If (dtImage.Rows.Count > 0) Then
            Dim RutaGlobal As String = gs_CarpetaRaiz
            Dim Name As String = dtImage.Rows(0).Item(0)
            If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name)) Then
                Dim im As New Bitmap(New Bitmap(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name))
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(im)
                img.Save(Bin, Imaging.ImageFormat.Png)
                Bin.Dispose()
                For i As Integer = 0 To dt.Rows.Count - 1 Step 1


                    dt.Rows(i).Item("img") = Bin.GetBuffer
                Next
            End If


        End If


        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If



        P_Global.Visualizador = New Visualizador

        Dim objrep As New Reporte_BoletaSueldo

        objrep.SetDataSource(dt)
        objrep.SetParameterValue("NombreEmpresa", NombreEmpresa)
        objrep.SetParameterValue("Ciudad", Direccion)
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(120)
        P_Global.Visualizador.Show() 'Comentar
        ''P_Global.Visualizador.BringToFront() 'Comentar


    End Sub

    Private Sub btnReportePlanilla_Click(sender As Object, e As EventArgs) Handles btnReportePlanilla.Click
        Dim dt As DataTable
        If (CType(grplanilla.DataSource, DataTable).Rows.Count <= 0) Then
            ToastNotification.Show(Me, "No Existen Datos En Planilla Para Generar Reporte", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return

        End If

        If (chktodos.Checked = True) Then
            dt = ListarPlanillaReporte(cbMes.Value, cbAnio.Value, 1)

        Else
            dt = ListarPlanillaReporte(cbMes.Value, cbAnio.Value, 0)
        End If
        Dim dtImage As DataTable = ObtenerImagenEmpresa()
        Dim NombreEmpresa As String = dtImage.Rows(0).Item("Nombre")
        Dim Direccion As String = dtImage.Rows(0).Item("Direccion")
        If (dtImage.Rows.Count > 0) Then
            Dim RutaGlobal As String = gs_CarpetaRaiz
            Dim Name As String = dtImage.Rows(0).Item(0)
            If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name)) Then
                Dim im As New Bitmap(New Bitmap(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name))
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(im)
                img.Save(Bin, Imaging.ImageFormat.Png)
                Bin.Dispose()
                For i As Integer = 0 To dt.Rows.Count - 1 Step 1


                    dt.Rows(i).Item("img") = Bin.GetBuffer
                Next
            End If


        End If


        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If



        P_Global.Visualizador = New Visualizador

        Dim objrep As New Reporte_Planilla

        objrep.SetDataSource(dt)
        objrep.SetParameterValue("NombreEmpresa", NombreEmpresa)
        objrep.SetParameterValue("Ciudad", Direccion)
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(120)
        P_Global.Visualizador.Show() 'Comentar
        ''P_Global.Visualizador.BringToFront() 'Comentar
    End Sub
End Class