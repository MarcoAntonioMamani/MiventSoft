Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls

Public Class Tec_Roles
#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem

    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Public _MListEstBuscador As List(Of Celda)
#End Region

#Region "Metodos Overrides"
    Public Sub _PMIniciarTodo()

        Me.WindowState = FormWindowState.Maximized
        _MListEstBuscador = _PMOGetListEstructuraBuscador()

        _PMFiltrar()
        _PMInhabilitar()


        If (CType(grModulos.DataSource, DataTable).Rows.Count > 0) Then

            grModulos.Row = 0
        End If

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

    End Sub
    Private Sub _PMCargarBuscador1()

        Dim dtBuscador As DataTable = _PMOGetTablaBuscador()

        JGrM_Buscador.DataSource = dtBuscador
        JGrM_Buscador.RetrieveStructure()

        For i = 0 To dtBuscador.Columns.Count - 1
            With JGrM_Buscador.RootTable.Columns(i)
                If _MListEstBuscador.Item(i).visible = True Then
                    .Caption = _MListEstBuscador.Item(i).titulo
                    .Width = _MListEstBuscador.Item(i).tamano
                    .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center

                    Dim col As DataColumn = dtBuscador.Columns(i)
                    Dim tipo As Type = col.DataType
                    If tipo.ToString = "System.Int32" Or tipo.ToString = "System.Decimal" Then
                        .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                    End If
                    If _MListEstBuscador.Item(i).formato = String.Empty Then
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

    End Sub


    Public Sub _PMInhabilitar()
        btnNuevo.Visible = True
        btnModificar.Visible = True
        btnEliminar.Visible = True
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
        tbRol.Focus()


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
            If _PMOGrabarRegistro() = True Then
                'actualizar el grid de buscador
                _PMCargarBuscador()

                _PMOLimpiar()
                _PMSalir()
            Else
                Exit Sub
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

        Else
            '  Public _modulo As SideNavItem
            '_modulo.Select()
            _tab.Close()
        End If
    End Sub
#End Region


#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "Gestion De Roles"

        _prCargarGridModulos()
        _PMIniciarTodo()
        _prAsignarPermisos()
        grModulos.Focus()

        tbRol.MaxLength = 30
        If (CType(grModulos.DataSource, DataTable).Rows.Count > 0) Then
            grModulos.Row = 0
        End If
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

    Private Sub _prCargarGridModulos()
        Dim dt As New DataTable
        dt = L_prLibreriaDetalleGeneral(1)

        grModulos.DataSource = dt
        grModulos.RetrieveStructure()

        'dar formato a las columnas
        With grModulos.RootTable.Columns("cnnum")
            .Width = 50
            .Visible = False
        End With

        With grModulos.RootTable.Columns("cndesc1")
            .Caption = "MODULO"
            .Width = 240
        End With



        With grModulos
            .GroupByBoxVisible = False
            'diseño de la grilla
            .AllowAddNew = InheritableBoolean.False
            .RowFormatStyle.BackColor = Color.AliceBlue
            .SelectionMode = SelectionMode.SingleSelection
            '.SelectedFormatStyle.BackColor = Color.LightGreen
            '.RowFormatStyle.BackColor = Color.MediumTurquoise
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
        End With



    End Sub

    Private Sub _prCargarGridDetalle(numi As String)
        Dim dt As New DataTable
        dt = L_prRolDetalleGeneral(numi)

        grDetalle.DataSource = dt
        grDetalle.RetrieveStructure()

        'dar formato a las columnas
        With grDetalle.RootTable.Columns("Id")
            .Width = 50
            .Visible = False
            .EditType = EditType.NoEdit
        End With
        With grDetalle.RootTable.Columns("Modulo")
            .Width = 50
            .Visible = False
            .EditType = EditType.NoEdit
        End With



        With grDetalle.RootTable.Columns("ProgramaId")
            .Caption = "CODIGO"
            .Width = 80
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .EditType = EditType.NoEdit
        End With

        With grDetalle.RootTable.Columns("RolId")

            .Width = 200
            .Visible = False
            .EditType = EditType.NoEdit
        End With
        With grDetalle.RootTable.Columns("DescripcionPrograma")
            .Caption = "PROGRAMAS"
            .Width = 300
            .EditType = EditType.NoEdit
        End With

        With grDetalle.RootTable.Columns("Ver")
            .Caption = "VER"
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
        End With
        With grDetalle.RootTable.Columns("Insertar")
            .Caption = "AGREGAR"
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
        End With
        With grDetalle.RootTable.Columns("Modificar")
            .Caption = "EDITAR"
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
        End With
        With grDetalle.RootTable.Columns("Eliminar")
            .Caption = "ELIMINAR"
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center
        End With

        With grDetalle.RootTable.Columns("estado")
            .Visible = False
        End With

        With grDetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .AllowAddNew = InheritableBoolean.False

        End With

        'Dim fc As GridEXFormatCondition
        'fc = New GridEXFormatCondition(grDetalle.RootTable.Columns("estado"), ConditionOperator.Equal, 0)
        'fc.FormatStyle.BackColor = Color.Gold
        'grDetalle.RootTable.FormatConditions.Add(fc)

        'filtro por la primera fila del modulo
        If dt.Rows.Count > 0 Then
            Dim numiModulo As String = grModulos.GetValue("cnnum")
            Dim desc As String = grModulos.GetValue("cndesc1")
            grDetalle.RemoveFilters()
            grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("Modulo"), Janus.Windows.GridEX.ConditionOperator.Equal, numiModulo))

        End If


    End Sub

    Private Sub _prSeleccionarTodos(columna As String)

        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)
        Dim numiModulo As String = grModulos.GetValue("cnnum")
        Dim filasFiltradas As DataRow() = dt.Select("Modulo=" + numiModulo)
        For Each fila As DataRow In filasFiltradas
            fila.Item(columna) = 1
            If fila.Item("estado") = 1 Then
                fila.Item("estado") = 2
            End If
        Next
    End Sub


#End Region

#Region "METODOS SOBRECARGADOS"


    Public Sub _PMOHabilitar()

        tbRol.ReadOnly = False

        grModulos.ContextMenuStrip = msModulos
    End Sub

    Public Sub _PMOInhabilitar()
        tbNumi.ReadOnly = True
        tbRol.ReadOnly = True

        grModulos.ContextMenuStrip = Nothing
    End Sub

    Public Sub _PMOLimpiar()
        tbNumi.Text = ""
        tbRol.Text = ""

        'VACIO EL DETALLE
        _prCargarGridDetalle(-1)

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbRol.BackColor = Color.White

    End Sub

    Public Function _PMOGrabarRegistro() As Boolean

        Dim dtDetalle As DataTable = CType(grDetalle.DataSource, DataTable).DefaultView.ToTable(True, "Id", "RolId", "ProgramaId", "DescripcionPrograma", "Modulo", "Ver", "Insertar", "Modificar", "Eliminar", "estado")
        Dim res As Boolean = L_prRolGrabar(tbNumi.Text, tbRol.Text, dtDetalle)
        If res Then
            ToastNotification.Show(Me, "Codigo de Rol ".ToUpper + tbNumi.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
        End If
        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean

        Dim dtDetalle As DataTable = CType(grDetalle.DataSource, DataTable).DefaultView.ToTable(True, "Id", "RolId", "ProgramaId", "DescripcionPrograma", "Modulo", "Ver", "Insertar", "Modificar", "Eliminar", "estado")
        Dim res As Boolean = L_prRolModificar(tbNumi.Text, tbRol.Text, dtDetalle)
        If res Then

            ToastNotification.Show(Me, "Codigo de Rol ".ToUpper + tbNumi.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
            _PSalirRegistro()
        End If
        Return res
    End Function

    Public Sub _PMOEliminarRegistro()
        'Dim info As New TaskDialogInfo("eliminacion".ToUpper, eTaskDialogIcon.Delete, "¿esta seguro de eliminar el registro?".ToUpper, "".ToUpper, eTaskDialogButton.Yes Or eTaskDialogButton.Cancel, eTaskDialogBackgroundColor.Blue)
        'Dim result As eTaskDialogResult = TaskDialog.Show(info)
        'If result = eTaskDialogResult.Yes Then
        '    Dim mensajeError As String = ""
        '    Dim res As Boolean = L_prRolBorrar(tbNumi.Text, mensajeError)
        '    If res Then
        '        ToastNotification.Show(Me, "Codigo de Rol ".ToUpper + tbNumi.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
        '        _PMFiltrar()
        '    Else
        '        ToastNotification.Show(Me, mensajeError, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
        '    End If
        'End If

        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Rol " + tbRol.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_prRolBorrar(tbNumi.Text, mensajeError)
            If res Then
                ToastNotification.Show(Me, "Codigo de Rol ".ToUpper + tbNumi.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PMFiltrar()
            Else
                ToastNotification.Show(Me, mensajeError, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If
        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()

        If tbRol.Text = String.Empty Then
            tbRol.BackColor = Color.Red
            MEP.SetError(tbRol, "ingrese la descripcion del rol!".ToUpper)
            _ok = False
        Else
            tbRol.BackColor = Color.White
            MEP.SetError(tbRol, "")
        End If

        MHighlighterFocus.UpdateHighlights()
        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prRolGeneral()
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 100))
        listEstCeldas.Add(New Celda("NombreRol", True, "ROL", 400))
        listEstCeldas.Add(New Celda("FechaRegistro", False))
        listEstCeldas.Add(New Celda("HoraRegistro", False))
        listEstCeldas.Add(New Celda("UsuarioRegistro", False))
        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer)
        JGrM_Buscador.Row = _MPos

        With JGrM_Buscador
            tbNumi.Text = .GetValue("Id").ToString
            tbRol.Text = .GetValue("NombreRol").ToString
            'CARGAR DETALLE
            _prCargarGridDetalle(tbNumi.Text)
        End With

        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString
        If (Not IsNothing(grDetalle.DataSource)) Then
            Dim numiModulo As String = grModulos.GetValue("cnnum")
            Dim desc As String = grModulos.GetValue("cndesc1")
            grDetalle.RemoveFilters()
            Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)
            grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("Modulo"), Janus.Windows.GridEX.ConditionOperator.Equal, numiModulo))
            lbprivilegio.Text = "privilegios del modulo ".ToUpper + desc
        End If
    End Sub


    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()

        Else
            '  Public _modulo As SideNavItem
            _modulo.Select()
            _tab.Close()
        End If
    End Sub
#End Region
    Private Sub SELECCIONARTODOSSHOWToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SELECCIONARTODOSSHOWToolStripMenuItem.Click
        _prSeleccionarTodos("Ver")
    End Sub

    Private Sub SELECCIONARTODOSADDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SELECCIONARTODOSADDToolStripMenuItem.Click
        _prSeleccionarTodos("Insertar")
    End Sub

    Private Sub SELECCIONARTODOSEDITToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SELECCIONARTODOSEDITToolStripMenuItem.Click
        _prSeleccionarTodos("Modificar")
    End Sub

    Private Sub SELECCIONARTODOSDELToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SELECCIONARTODOSDELToolStripMenuItem.Click
        _prSeleccionarTodos("Eliminar")
    End Sub



    Private Sub JGrM_Buscador_SelectionChanged(sender As Object, e As EventArgs) Handles JGrM_Buscador.SelectionChanged
        If JGrM_Buscador.Row >= 0 Then
            _MPos = JGrM_Buscador.Row
            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub grDetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grDetalle.CellEdited
        Dim estado As Integer = grDetalle.GetValue("estado")
        If estado = 1 Then
            grDetalle.SetValue("estado", 2)
        End If

    End Sub
    Private Sub grDetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grDetalle.EditingCell
        If btnGrabar.Enabled Then
            If e.Column.Key <> "Ver" And e.Column.Key <> "Insertar" And e.Column.Key <> "Modificar" And e.Column.Key <> "Eliminar" Then
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If


    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _PMNuevo()
    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        _PMModificar()
    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        _PMEliminar()
    End Sub

    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        _PMGuardar()
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _PMSalir()
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

    Private Sub Tec_Roles_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub lbprivilegio_Click(sender As Object, e As EventArgs) Handles lbprivilegio.Click

    End Sub

    Private Sub grModulos_SelectionChanged(sender As Object, e As EventArgs) Handles grModulos.SelectionChanged
        If (Not IsNothing(grDetalle.DataSource)) Then
            Dim numiModulo As String = grModulos.GetValue("cnnum")
            Dim desc As String = grModulos.GetValue("cndesc1")
            grDetalle.RemoveFilters()
            Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)
            grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("Modulo"), Janus.Windows.GridEX.ConditionOperator.Equal, numiModulo))
            lbprivilegio.Text = "privilegios del modulo ".ToUpper + desc
        End If

    End Sub
End Class