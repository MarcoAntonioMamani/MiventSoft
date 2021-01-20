
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class Tec_Contratos
#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Public FilaSeleccionada As Boolean = False


    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"


    Dim TablaImagenes As DataTable
    Dim TablaInventario As DataTable
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim gs_DirPrograma As String = ""
    Dim gs_RutaImg As String = ""
    Dim PersonalId As Integer = 0




#End Region


#Region "Metodos Overrides"
    Public Sub _PMIniciarTodo()
        _MListEstBuscador = _PMOGetListEstructuraBuscador()
        Me.WindowState = FormWindowState.Maximized

        _PMFiltrar()
        _PMInhabilitar()
    End Sub

    Private Sub _PMCargarBuscador()

        Dim dtBuscador As DataTable = _PMOGetTablaBuscador()

        JGrM_Buscador.DataSource = dtBuscador
        JGrM_Buscador.RetrieveStructure()

        For i = 0 To _MListEstBuscador.Count - 1
            Dim campo As String = _MListEstBuscador.Item(i).campo
            With JGrM_Buscador.RootTable.Columns(campo)
                If _MListEstBuscador.Item(i).visible = True Then
                    .Caption = _MListEstBuscador.Item(i).titulo
                    .Width = _MListEstBuscador.Item(i).tamano
                    .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
                    .MaxLines = 2
                    .WordWrap = True
                    Dim col As DataColumn = dtBuscador.Columns(campo)
                    Dim tipo As Type = col.DataType
                    If tipo.ToString = "System.Int32" Or tipo.ToString = "System.Decimal" Or tipo.ToString = "System.Double" Then
                        .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                    End If
                    If _MListEstBuscador.Item(i).formato <> String.Empty Then
                        .FormatString = _MListEstBuscador.Item(i).formato
                    End If
                Else
                    .Visible = False
                End If
            End With
        Next

        'Habilitar Filtradores
        With JGrM_Buscador
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
        CargarIconEstado()
    End Sub

    Public Sub CargarIconEstado()

        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1
            If (dt.Rows(i).Item("estado") = 1) Then
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.activo, 110, 30)
                img.Save(Bin, Imaging.ImageFormat.Png)
                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer
            Else
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.pasivo, 110, 30)
                img.Save(Bin, Imaging.ImageFormat.Png)
                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer
            End If

        Next

    End Sub

    Public Sub _PMInhabilitar()
        btnNuevo.Visible = True
        btnModificar.Visible = False
        btnEliminar.Visible = False
        btnGrabar.Visible = False
        PanelNavegacion.Enabled = True
        JGrM_Buscador.Enabled = True


        _PMOLimpiarErrores()

        _PMOInhabilitar()
    End Sub

    Private Sub _PMHabilitar()
        JGrM_Buscador.Enabled = False
        _PMOHabilitar()
    End Sub
    Public Sub _PMFiltrar()
        'cargo el buscador
        _PMCargarBuscador()
        If JGrM_Buscador.RowCount > 0 Then
            _MPos = 0
            JGrM_Buscador.Row = _MPos
            _PMOMostrarRegistro(_MPos)
        Else
            _PMOLimpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub

    Public Sub _PMPrimerRegistro()
        If JGrM_Buscador.RowCount > 0 Then
            _MPos = 0

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub _PMAnteriorRegistro()
        If _MPos > 0 And JGrM_Buscador.RowCount > 0 Then
            _MPos = _MPos - 1

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub _PMSiguienteRegistro()
        If _MPos < JGrM_Buscador.RowCount - 1 Then
            _MPos = _MPos + 1

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub _PMUltimoRegistro()
        If JGrM_Buscador.RowCount > 0 Then
            _MPos = JGrM_Buscador.RowCount - 1

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub

    Private Sub _PMNuevo()
        _MNuevo = True
        _MModificar = False

        _PMOLimpiar()
        _PMHabilitar()

        btnNuevo.Visible = False
        btnModificar.Visible = False
        btnEliminar.Visible = False
        btnGrabar.Visible = True
        PanelNavegacion.Enabled = False
        tbPersonal.Focus()


        '_PMOLimpiar()

    End Sub

    Private Sub _PMModificar()
        If JGrM_Buscador.Row >= 0 Then
            _MNuevo = False
            _MModificar = True

            _PMHabilitar()
            btnNuevo.Visible = False
            btnModificar.Visible = False
            btnEliminar.Visible = False
            btnGrabar.Visible = True

            PanelNavegacion.Enabled = False


        End If
    End Sub

    Private Sub _PMEliminar()
        'Dim _Result As MsgBoxResult
        '_Result = MsgBox("¿Esta seguro de Eliminar el Registro?".ToUpper, MsgBoxStyle.YesNo, "Advertencia".ToUpper)
        'If _Result = MsgBoxResult.Yes Then
        '    _PMOEliminarRegistro()
        '    _PMFiltrar()

        'End If
        _PMOEliminarRegistro()
    End Sub

    Private Sub _PMGuardar()

        If _PMOValidarCampos() = False Then
            Exit Sub
        End If

        If _MNuevo Then
            If (VerificarExisteContratoVigente(PersonalId) = False) Then
                If _PMOGrabarRegistro() = True Then
                    'actualizar el grid de buscador
                    _PMCargarBuscador()

                    _PMOLimpiar()
                    _PMSalir()
                Else
                    Exit Sub
                End If

            Else
                ToastNotification.Show(Me, "El Personal " + tbPersonal.Text + "  Ya tiene un Contrato Vigente", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If



        Else

            _PMOModificarRegistro()

            'actualizar el grid de buscador
            _PMCargarBuscador()

            '_PMSalir()
        End If
    End Sub

    Private Sub _PMSalir()
        If btnGrabar.Visible = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()
            TabControlPrincipal.SelectedTabIndex = 1
        Else
            '  Public _modulo As SideNavItem
            '_TabControl.SelectedTab = _modulo
            '_tab.Close()
            'Me.Close()
            TabControlPrincipal.SelectedTabIndex = 1
        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)

        Me.Text = "Gestion De Personal"
        P_Global._prCargarComboGenerico(cbTipoContrato, L_prLibreriaDetalleGeneral(10), "cnnum", "Codigo", "cndesc1", "TipoDocumento")


        P_Global._prCargarComboGenerico(cbCargo, L_prLibreriaDetalleGeneral(11), "cnnum", "Codigo", "cndesc1", "TipoPersonal")
        _PMIniciarTodo()
        _prAsignarPermisos()


        _habilitarFocus()

    End Sub
    Private Sub _prCargarTablaConceptos(id As String)
        Dim dt As New DataTable
        dt = ListaConceptosContratos(id)
        dt.Columns.Add("Eliminar", GetType(Byte()))

        grConceptos.DataSource = dt
        grConceptos.RetrieveStructure()
        grConceptos.AlternatingColors = True
        'c.Id ,c.ContratoId ,c.ConceptoId,concepto .NombreConcepto ,concepto.Porcentaje as PorcentajeConcepto,1 as estado

        With grConceptos.RootTable.Columns("Id")
            .Width = 90
            .Caption = "Id"
            .Visible = True
        End With
        With grConceptos.RootTable.Columns("ContratoId")
            .Width = 110
            .Visible = False
        End With
        With grConceptos.RootTable.Columns("ConceptoId")
            .Width = 110
            .Visible = False
        End With
        With grConceptos.RootTable.Columns("NombreConcepto")
            .Width = 300
            .Visible = True
            .Caption = "Concepto"
        End With

        With grConceptos.RootTable.Columns("PorcentajeConcepto")
            .Width = 110
            .Caption = "Porcentaje"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grConceptos.RootTable.Columns("estado")
            .Width = 150
            .Visible = False
        End With
        With grConceptos.RootTable.Columns("Eliminar")
            .Width = 120
            .Visible = False
        End With
        With grConceptos
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
        CargarIconEliminar()
    End Sub

    Public Sub CargarIconEliminar()
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.eliminar01, grConceptos.RootTable.Columns("Eliminar").Width - 15, 70)
        img.Save(Bin, Imaging.ImageFormat.Png)
        Dim dt As DataTable = CType(grConceptos.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1


            CType(grConceptos.DataSource, DataTable).Rows(i).Item("Eliminar") = Bin.GetBuffer


        Next

    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbPersonal, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSearchPersonal, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbTipoContrato, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnTipoContrato, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbCargo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnCargo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbSalario, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swIndefinido, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbFechaInicio, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbFechaFin, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnAgregarConceptoFijo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        End With
    End Sub

    Private Sub _prAsignarPermisos()

        'Dim dtRolUsu As DataTable = L_prRolDetalleGeneral(gi_userRol, _nameButton)

        'Dim show As Boolean = dtRolUsu.Rows(0).Item("ycshow")
        'Dim add As Boolean = dtRolUsu.Rows(0).Item("ycadd")
        'Dim modif As Boolean = dtRolUsu.Rows(0).Item("ycmod")
        'Dim del As Boolean = dtRolUsu.Rows(0).Item("ycdel")

        'If add = False Then
        '    btnNuevo.Visible = False
        'End If
        'If modif = False Then
        '    btnModificar.Visible = False
        'End If
        'If del = False Then
        '    btnEliminar.Visible = False
        'End If

    End Sub








#End Region

#Region "METODOS SOBRECARGADOS"


    Public Sub _PMOHabilitar()

        cbTipoContrato.ReadOnly = False
        tbSalario.IsInputReadOnly = False
        swIndefinido.IsReadOnly = False
        cbCargo.ReadOnly = False


        tbFechaInicio.ReadOnly = False
        tbFechaFin.ReadOnly = False
        btnCargo.Visible = True
        btnTipoContrato.Visible = True
        btnSearchPersonal.Visible = True


        swIndefinido.IsReadOnly = False
        panelConceptoFijo.Visible = True

        grConceptos.RootTable.Columns("Eliminar").Visible = True
    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        cbTipoContrato.ReadOnly = True
        tbSalario.IsInputReadOnly = True
        swIndefinido.IsReadOnly = True
        cbCargo.ReadOnly = True
        panelConceptoFijo.Visible = False
        tbPersonal.ReadOnly = False
        tbFechaInicio.ReadOnly = True
        tbFechaFin.ReadOnly = True
        btnCargo.Visible = False
        btnTipoContrato.Visible = False
        btnSearchPersonal.Visible = False

        grConceptos.RootTable.Columns("Eliminar").Visible = False

    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbPersonal.Clear()
        tbSalario.Value = 0
        tbFechaInicio.Value = Now.Date
        tbFechaFin.Value = Now.Date

        swIndefinido.Value = False
        seleccionarPrimerItemCombo(cbCargo)
        seleccionarPrimerItemCombo(cbTipoContrato)
        btnSearchPersonal.Focus()
        _prCargarTablaConceptos(-1)
    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()


        tbPersonal.BackColor = Color.White
        cbCargo.BackColor = Color.White
        cbTipoContrato.BackColor = Color.White
        tbSalario.BackColor = Color.White

    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        '_Id As String, PersonalId As Integer, TipoContratoId As Integer, Cargo As Integer, SueldoBase As Double, InicioContrato As String, FinContrato As String, Estado As Integer, Indefinido As Integer, dtdetalle As DataTabl

        'c.Id , c.ContratoId, c.ConceptoId, concepto.NombreConcepto, concepto.Porcentaje As PorcentajeConcepto, 1 as estado
        Dim res As Boolean
        Try
            Dim dt As New DataTable
            dt = CType(grConceptos.DataSource, DataTable).DefaultView.ToTable(False, "Id", "ContratoId", "ConceptoId", "NombreConcepto", "PorcentajeConcepto", "estado")
            Dim FechaFin As Date = Now.Date
            FechaFin = DateAdd(DateInterval.Year, 50, Now.Date)
            res = InsertarContratos(tbCodigo.Text, PersonalId, cbTipoContrato.Value, cbCargo.Value, tbSalario.Value, tbFechaInicio.Value.ToString("dd/MM/yyyy"), IIf(swIndefinido.Value = True, FechaFin.ToString("dd/MM/yyyy"), tbFechaFin.Value.ToString("dd/MM/yyyy")), 1, IIf(swIndefinido.Value = True, 1, 0), dt)

            If res Then


                ToastNotification.Show(Me, "Codigo de Contrato ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar el Contrato".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Contrato".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Dim dt As New DataTable
            dt = CType(grConceptos.DataSource, DataTable).DefaultView.ToTable(False, "Id", "ContratoId", "ConceptoId", "NombreConcepto", "PorcentajeConcepto", "estado")
            Res = ModificarContratos(tbCodigo.Text, PersonalId, cbTipoContrato.Value, cbCargo.Value, tbSalario.Value, tbFechaInicio.Value.ToString("dd/MM/yyyy"), tbFechaFin.Value.ToString("dd/MM/yyyy"), 1, IIf(swIndefinido.Value = True, 1, 0), dt)
            If Res Then

                ToastNotification.Show(Me, "Codigo de Contrato ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el Contrato".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar Contrato".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbPersonal.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Contrato del Personal" + tbPersonal.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_Contratos")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Contrato ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el Contrato".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "

        If PersonalId <= 0 Then
            tbPersonal.BackColor = Color.Red
            MEP.SetError(tbPersonal, "Seleccione Personal")
            Mensaje = Mensaje + "Personal"
            _ok = False
        Else
            tbPersonal.BackColor = Color.White
            MEP.SetError(tbPersonal, "")
        End If
        If (cbTipoContrato.SelectedIndex < 0) Then
            cbTipoContrato.BackColor = Color.Red
            MEP.SetError(cbTipoContrato, "Seleccione un Tipo de Contrato")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Tipo Contrato"
            _ok = False
        Else
            cbTipoContrato.BackColor = Color.White
            MEP.SetError(cbTipoContrato, "")
        End If
        If (cbCargo.SelectedIndex < 0) Then
            cbCargo.BackColor = Color.Red
            MEP.SetError(cbCargo, "Seleccione un Cargo")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Cargo"
            _ok = False
        Else
            cbCargo.BackColor = Color.White
            MEP.SetError(cbCargo, "")
        End If
        If (tbFechaInicio.Value >= tbFechaFin.Value And swIndefinido.Value = False) Then
            tbFechaFin.BackColor = Color.Red
            MEP.SetError(tbFechaFin, "Fecha Finalizacion Debe Ser Menor a la Fecha de Inicio de Vigencia")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Fecha Finalizacion Debe Ser Mayor a la Fecha de Inicio de Vigencia"
            _ok = False
        Else
            tbFechaFin.BackColor = Color.White
            MEP.SetError(tbFechaFin, "")
        End If

        MHighlighterFocus.UpdateHighlights()
        If (tbFechaInicio.Value >= tbFechaFin.Value And swIndefinido.Value = False) Then
            tbFechaFin.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If
        If PersonalId <= 0 Then
            tbPersonal.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If
        If (cbTipoContrato.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbTipoContrato.Focus()
            Return _ok
        End If
        If (cbCargo.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbCargo.Focus()
            Return _ok
        End If

        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Contratos")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        '     c.Id , c.PersonalId, p.NombrePersonal, c.TipoContratoID, TipoContrato.Descripcion as TipoContratoDescripcion
        ',c.Cargo ,Cargo .Descripcion as DescripcionCargo,c.SueldoBase ,c.InicioContrato ,c.FinContrato ,c.Estado ,c.Indefinido 	
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("PersonalId", False))
        listEstCeldas.Add(New Celda("NombrePersonal", True, "Nombre Personal", 200))
        listEstCeldas.Add(New Celda("TipoContratoID", False))
        listEstCeldas.Add(New Celda("TipoContratoDescripcion", True, "Tipo Contrato", 120))
        listEstCeldas.Add(New Celda("img", True, "Estado", 90))
        listEstCeldas.Add(New Celda("Cargo", False))
        listEstCeldas.Add(New Celda("DescripcionCargo", True, "Cargo", 120))
        listEstCeldas.Add(New Celda("Antigüedad", True, "Antigüedad", 150))
        listEstCeldas.Add(New Celda("SueldoBase", True, "Sueldo", 90, "0.00"))
        listEstCeldas.Add(New Celda("InicioContrato", True, "Inicio", 80, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("FinContrato", False, "Fin", 80, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("FechaFin", True, "Fin", 80, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("Estado", False))
        listEstCeldas.Add(New Celda("Indefinido", False))



        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        '     c.Id , c.PersonalId, p.NombrePersonal, c.TipoContratoID, TipoContrato.Descripcion as TipoContratoDescripcion
        ',c.Cargo ,Cargo .Descripcion as DescripcionCargo,c.SueldoBase ,c.InicioContrato ,c.FinContrato ,c.Estado ,c.Indefinido 	
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbPersonal.Text = .GetValue("NombrePersonal").ToString
            PersonalId = .GetValue("PersonalId")
            cbTipoContrato.Value = .GetValue("TipoContratoID")
            cbCargo.Value = .GetValue("Cargo")
            tbSalario.Value = .GetValue("SueldoBase")
            tbFechaInicio.Value = .GetValue("InicioContrato")
            tbFechaFin.Value = .GetValue("FinContrato")


            swIndefinido.Value = .GetValue("Indefinido")


        End With
        _prCargarTablaConceptos(tbCodigo.Text)
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub




    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()
            TabControlPrincipal.SelectedTabIndex = 1
        Else
            '  Public _modulo As SideNavItem
            '_TabControl.SelectedTab = _modulo
            '_tab.Close()
            'Me.Close()
            TabControlPrincipal.SelectedTabIndex = 1
        End If
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _PMNuevo()

    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        _PMModificar()

    End Sub

    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        _PMGuardar()

    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        _PMEliminar()


    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _PMSalir()

    End Sub

    Private Sub Tec_Users_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        _PMPrimerRegistro()
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        _PMAnteriorRegistro()
    End Sub

    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        _PMSiguienteRegistro()
    End Sub

    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        _PMUltimoRegistro()
    End Sub
    Private Sub JGrM_Buscador_SelectionChanged(sender As Object, e As EventArgs) Handles JGrM_Buscador.SelectionChanged
        If (JGrM_Buscador.Row >= 0 And FilaSeleccionada = False) Then
            _MPos = JGrM_Buscador.Row
            _PMOMostrarRegistro(_MPos, True)
        End If
    End Sub

    Private Sub VerToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles VerToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then

            TabControlPrincipal.SelectedTabIndex = 0
        End If
    End Sub

    Private Sub EditarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditarToolStripMenuItem.Click
        If (JGrM_Buscador.Row >= 0) Then
            TabControlPrincipal.SelectedTabIndex = 0
            btnModificar.PerformClick()
            tbPersonal.Focus()
        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then
            btnEliminar.PerformClick()
        End If
    End Sub










#End Region
    Private Sub swEstado_ValueChanged(sender As Object, e As EventArgs) Handles swIndefinido.ValueChanged
        If (swIndefinido.Value = True) Then
            lbFechaFin.Visible = False
            tbFechaFin.Visible = False
        Else

            lbFechaFin.Visible = True
            tbFechaFin.Visible = True
        End If
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        TabControlPrincipal.SelectedTabIndex = 0
        btnNuevo.PerformClick()

    End Sub

    Private Sub btnSearchPersonal_Click(sender As Object, e As EventArgs) Handles btnSearchPersonal.Click
        Dim dt As DataTable

        dt = ListarPersonal()
        'a.Id ,a.NombreProveedor ,a.Direccion ,a.Telefono01

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id,", False, "ID", 50))
        listEstCeldas.Add(New Celda("Nombre", True, "NOMBRE", 350))
        listEstCeldas.Add(New Celda("Direccion", True, "DIRECCION", 180))
        listEstCeldas.Add(New Celda("Telefono01", True, "Telefono".ToUpper, 200))
        Dim ef = New Efecto
        ef.tipo = 6
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 150
        ef.ancho = 500
        ef.Context = "Seleccione Personal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            PersonalId = Row.Cells("Id").Value
            tbPersonal.Text = Row.Cells("Nombre").Value
            cbTipoContrato.Focus()

        End If
    End Sub

    Public Function VerificarExisteContratoVigente(PersonalId As Integer) As Boolean
        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            ''20/01/2021   30/02/2021  
            If (dt.Rows(i).Item("PersonalId") = PersonalId And tbFechaInicio.Value <= dt.Rows(i).Item("FinContrato")) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Sub btnTipoContrato_Click(sender As Object, e As EventArgs) Handles btnTipoContrato.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 10
        ef.titulo = "Crear Nuevo Tipo De Contrato"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbTipoContrato, L_prLibreriaDetalleGeneral(10), "cnnum", "Codigo", "cndesc1", "Tipo Contrato")
            cbTipoContrato.SelectedIndex = CType(cbTipoContrato.DataSource, DataTable).Rows.Count - 1
            cbTipoContrato.Focus()
        End If
    End Sub

    Private Sub btnCargo_Click(sender As Object, e As EventArgs) Handles btnCargo.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 11
        ef.titulo = "Crear Nuevo Cargo"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbCargo, L_prLibreriaDetalleGeneral(11), "cnnum", "Codigo", "cndesc1", "Cargo")
            cbCargo.SelectedIndex = CType(cbTipoContrato.DataSource, DataTable).Rows.Count - 1
            cbCargo.Focus()
        End If
    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Private Sub btnAgregarConceptoFijo_Click(sender As Object, e As EventArgs) Handles btnAgregarConceptoFijo.Click
        Dim dt As DataTable

        dt = ListaConceptosFijosContrato()

        Dim concepto As DataTable = dt.Copy
        concepto.Rows.Clear()
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            Dim Existe As Boolean = False
            Dim ConceptoId As Integer = dt.Rows(i).Item("id")
            For j As Integer = 0 To CType(grConceptos.DataSource, DataTable).Rows.Count - 1 Step 1

                If (dt.Rows(i).Item("id") = CType(grConceptos.DataSource, DataTable).Rows(j).Item("ConceptoId") And CType(grConceptos.DataSource, DataTable).Rows(j).Item("estado") >= 0) Then
                    Existe = True
                End If

            Next
            If (Existe = False) Then
                concepto.ImportRow(dt.Rows(i))
            End If

        Next

        'a.id,a.NombreConcepto ,a.Porcentaje

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("id,", True, "ID", 50))
        listEstCeldas.Add(New Celda("NombreConcepto", True, "CONCEPTO", 350))
        listEstCeldas.Add(New Celda("Porcentaje", True, "PORCENTAJE", 180, "0.00"))
        Dim ef = New Efecto
        ef.tipo = 6
        ef.dt = concepto
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 150
        ef.ancho = 500
        ef.Context = "Seleccione Conceptos Fijos".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
            Dim Bin As New MemoryStream
            Dim img As New Bitmap(My.Resources.eliminar01, grConceptos.RootTable.Columns("Eliminar").Width - 20, 45)
            img.Save(Bin, Imaging.ImageFormat.Png)
            CType(grConceptos.DataSource, DataTable).Rows.Add(_GenerarId() + 1, 0, Row.Cells("id").Value, Row.Cells("NombreConcepto").Value, Row.Cells("Porcentaje").Value, 0, Bin.GetBuffer)


            btnAgregarConceptoFijo.Focus()

        End If
    End Sub
    Public Function _GenerarId()
        Dim dt As DataTable = CType(grConceptos.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(grConceptos.DataSource, DataTable).Rows(i).Item("Id")), 0, CType(grConceptos.DataSource, DataTable).Rows(i).Item("Id"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function
    Public Function ObtenerPosicion(id As Integer) As Integer

        Dim dt As DataTable = CType(grConceptos.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("id") = id) Then

                Return i
            End If
        Next
        Return -1
    End Function
    Private Sub grConceptos_Click(sender As Object, e As EventArgs) Handles grConceptos.Click
        Try
            If (grConceptos.RowCount >= 1 And grConceptos.Row >= 0) Then
                If (grConceptos.CurrentColumn.Index = grConceptos.RootTable.Columns("Eliminar").Index) Then
                    Dim posicion As Integer = -1
                    posicion = ObtenerPosicion(grConceptos.GetValue("id"))
                    CType(grConceptos.DataSource, DataTable).Rows(posicion).Item("estado") = -1
                    grConceptos.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grConceptos.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))

                End If

            End If
        Catch ex As Exception

        End Try

    End Sub
End Class