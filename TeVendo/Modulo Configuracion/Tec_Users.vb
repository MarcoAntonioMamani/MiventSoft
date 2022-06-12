Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Public Class Tec_Users


#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Public Selected As Boolean = False
    Public IdPersonal As Integer = 0


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
                    .TextAlignment = TextAlignment.Center

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
        tbVendedor.Focus()


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
            _TabControl.SelectedTab = _modulo
            _tab.Close()
            Me.Close()

        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "Gestion De Usuarios"
        _prCargarComboLibreriaRoles(cbRol)
        _prCargarComboLibreriaEmpresa(cbEmpresa)
        _PMIniciarTodo()
        _prAsignarPermisos()

        _habilitarFocus()

    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

            .SetHighlightOnFocus(tbNombreUsuario, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbVendedor, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbContrasena, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbEmpresa, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbRol, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swVentasDirectas, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub
    Private Sub _prCargarComboLibreriaRoles(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prListaRolesUsuarios()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("Id").Width = 60
            .DropDownList.Columns("Id").Caption = "Codigo"
            .DropDownList.Columns.Add("NombreRol").Width = 500
            .DropDownList.Columns("NombreRol").Caption = "Roles"
            .ValueMember = "Id"
            .DisplayMember = "NombreRol"
            .DataSource = dt
            .Refresh()
        End With
    End Sub

    Private Sub _prCargarComboLibreriaEmpresa(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prListaEmpresasUsuarios()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("Id").Width = 60
            .DropDownList.Columns("Id").Caption = "Codigo"
            .DropDownList.Columns.Add("Nombre").Width = 500
            .DropDownList.Columns("Nombre").Caption = "Empresa"
            .ValueMember = "Id"
            .DisplayMember = "Nombre"
            .DataSource = dt
            .Refresh()
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

        tbNombreUsuario.ReadOnly = False
        tbContrasena.ReadOnly = False
        cbEmpresa.ReadOnly = False
        cbRol.ReadOnly = False
        swEstado.IsReadOnly = False
        btnVendedor.Visible = True

        swVentasDirectas.IsReadOnly = False
    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreUsuario.ReadOnly = True
        tbContrasena.ReadOnly = True
        cbEmpresa.ReadOnly = True
        cbRol.ReadOnly = True
        swEstado.IsReadOnly = True
        tbVendedor.ReadOnly = True
        swVentasDirectas.IsReadOnly = True
        btnVendedor.Visible = False
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbNombreUsuario.Text = ""
        tbContrasena.Text = ""
        swEstado.Value = True
        swVentasDirectas.Value = False
        tbVendedor.Clear()
        If (ObtenerLongitudCombo(cbRol) > 0) Then
            cbRol.SelectedIndex = 0
        End If
        If (ObtenerLongitudCombo(cbEmpresa) > 0) Then
            cbEmpresa.SelectedIndex = 0
        End If
        tbVendedor.Focus()
        IdPersonal = 0
    End Sub
    Public Function ObtenerLongitudCombo(cb As EditControls.MultiColumnCombo) As Integer
        Return CType(cb.DataSource, DataTable).Rows.Count

    End Function

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbNombreUsuario.BackColor = Color.White
        tbContrasena.BackColor = Color.White
        cbEmpresa.BackColor = Color.White
        cbRol.BackColor = Color.White
        tbVendedor.BackColor = Color.White
    End Sub

    Public Function _PMOGrabarRegistro() As Boolean

        Dim res As Boolean = L_prUsuarioInsertar(tbCodigo.Text, cbRol.Value, tbNombreUsuario.Text,
                                                  tbContrasena.Text, IIf(swEstado.Value = True, 1, 0), 1, cbEmpresa.Value, IdPersonal, IIf(swEstado.Value = True, 1, 0))
        If res Then
            ToastNotification.Show(Me, "Codigo de Usuario ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
        End If
        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean

        Dim res As Boolean = L_prUsuarioModificar(tbCodigo.Text, cbRol.Value, tbNombreUsuario.Text,
                                                  tbContrasena.Text, IIf(swEstado.Value = True, 1, 0), 1, cbEmpresa.Value, IdPersonal, IIf(swEstado.Value = True, 1, 0))
        If res Then

            ToastNotification.Show(Me, "Codigo de Usuario ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
            _PSalirRegistro()
        End If
        Return res
    End Function

    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Usuario " + tbNombreUsuario.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_prUsuarioBorrar(tbCodigo.Text, mensajeError)
            If res Then
                ToastNotification.Show(Me, "Codigo de Usuario ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PMFiltrar()
            Else
                ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If
        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()

        If tbNombreUsuario.Text = String.Empty Then
            tbNombreUsuario.BackColor = Color.Red
            MEP.SetError(tbNombreUsuario, "Ingrese Nombre de Usuario")
            _ok = False
        Else
            tbNombreUsuario.BackColor = Color.White
            MEP.SetError(tbNombreUsuario, "")
        End If

        If tbContrasena.Text = String.Empty Then
            tbContrasena.BackColor = Color.Red
            MEP.SetError(tbContrasena, "Ingrese una Contraseña")
            _ok = False
        Else
            tbContrasena.BackColor = Color.White
            MEP.SetError(tbContrasena, "")
        End If
        If cbEmpresa.SelectedIndex < 0 Then
            cbEmpresa.BackColor = Color.Red
            MEP.SetError(cbEmpresa, "Seleccione Una Empresa")
            _ok = False
        Else
            cbEmpresa.BackColor = Color.White
            MEP.SetError(cbEmpresa, "")
        End If
        If cbRol.SelectedIndex < 0 Then
            cbRol.BackColor = Color.Red
            MEP.SetError(cbRol, "Seleccione Una Rol")
            _ok = False
        Else
            cbRol.BackColor = Color.White
            MEP.SetError(cbRol, "")
        End If
        If IdPersonal <= 0 Then
            tbVendedor.BackColor = Color.Red
            MEP.SetError(tbVendedor, "Seleccione Un Personal")
            _ok = False
        Else
            tbVendedor.BackColor = Color.White
            MEP.SetError(tbVendedor, "")
        End If
        MHighlighterFocus.UpdateHighlights()

        If IdPersonal <= 0 Then
            tbVendedor.Focus()
            Return _ok
        End If
        If tbNombreUsuario.Text = String.Empty Then
            tbNombreUsuario.Focus()
            Return _ok
        End If

        If tbContrasena.Text = String.Empty Then
            tbContrasena.Focus()
            Return _ok
        End If
        If cbEmpresa.SelectedIndex < 0 Then
            cbEmpresa.Focus()
            Return _ok
        End If
        If cbRol.SelectedIndex < 0 Then
            cbRol.Focus()
            Return _ok
        End If

        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prUsuarioGeneral()
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'u.Id, u.NombreUsuario, u.Contrasena, cast(u.Estado As bit)  As estado, u.RolId,
        'r.NombreRol, u.SucursalId, u.IdEmpresa, em.Nombre as Empresa
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("NombreUsuario", True, "ROL", 100))
        listEstCeldas.Add(New Celda("Contrasena", False))
        listEstCeldas.Add(New Celda("estado", True, "Estado", 60))
        listEstCeldas.Add(New Celda("RolId", False))

        listEstCeldas.Add(New Celda("NombreRol", True, "Rol", 90))
        listEstCeldas.Add(New Celda("SucursalId", False))
        listEstCeldas.Add(New Celda("IdEmpresa", False))
        listEstCeldas.Add(New Celda("Empresa", True, "Empresa", 80))
        listEstCeldas.Add(New Celda("IdPersonal", False))
        listEstCeldas.Add(New Celda("pedido", False))
        listEstCeldas.Add(New Celda("Personal", True, "Personal", 120))
        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional bandera As Boolean = False)

        If (bandera = False) Then
            Selected = True
            JGrM_Buscador.Row = _MPos
            Selected = False
        End If

        'u.Id, u.NombreUsuario, u.Contrasena, cast(u.Estado As bit)  As estado, u.RolId,
        'r.NombreRol, u.SucursalId, u.IdEmpresa, em.Nombre as Empresa
        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbNombreUsuario.Text = .GetValue("NombreUsuario").ToString
            tbContrasena.Text = .GetValue("Contrasena").ToString
            swEstado.Value = .GetValue("estado")
            cbRol.Value = .GetValue("RolId")
            cbEmpresa.Value = .GetValue("IdEmpresa")
            IdPersonal = .GetValue("IdPersonal")
            swVentasDirectas.Value = .GetValue("pedido")
            tbVendedor.Text = .GetValue("Personal").ToString
        End With

        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub


    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()

        Else
            '  Public _modulo As SideNavItem
            _TabControl.SelectedTab = _modulo
            _tab.Close()
            Me.Close()

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
        If (JGrM_Buscador.Row >= 0 And Selected = False) Then
            _MPos = JGrM_Buscador.Row
            _PMOMostrarRegistro(JGrM_Buscador.Row, True)
        End If

    End Sub
    Function _fnAccesible() As Boolean
        Return tbNombreUsuario.ReadOnly = False
    End Function
    Private Sub tbVendedor_KeyDown(sender As Object, e As KeyEventArgs) Handles tbVendedor.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Control + Keys.Enter Then


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
                ef.alto = 50
                ef.ancho = 350
                ef.Context = "Seleccione Personal".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                    IdPersonal = Row.Cells("Id").Value
                    tbVendedor.Text = Row.Cells("Nombre").Value
                    tbNombreUsuario.Focus()

                End If

            End If

        End If

    End Sub

    Private Sub btnVendedor_Click(sender As Object, e As EventArgs) Handles btnVendedor.Click
        If (_fnAccesible()) Then
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
            ef.alto = 50
            ef.ancho = 350
            ef.Context = "Seleccione Personal".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                IdPersonal = Row.Cells("Id").Value
                tbVendedor.Text = Row.Cells("Nombre").Value
                tbNombreUsuario.Focus()

            End If
        End If
    End Sub

    Private Sub JGrM_Buscador_FormattingRow(sender As Object, e As RowLoadEventArgs) Handles JGrM_Buscador.FormattingRow

    End Sub

#End Region
End Class