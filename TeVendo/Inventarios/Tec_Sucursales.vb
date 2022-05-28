Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls

Public Class Tec_Sucursales
#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem

    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Public Selected As Boolean = False

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
        tbNombreSucursal.Focus()


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

        Me.Text = "Gestion De Sucursales"
        _PMIniciarTodo()
        _prAsignarPermisos()


        P_Global._prCargarComboGenerico(cbDeposito, L_prListarDepositos(), "Id", "Codigo", "NombreDeposito", "NombreDeposito")

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
        tbNombreSucursal.ReadOnly = False
        tbDescripcion.ReadOnly = False
        cbDeposito.ReadOnly = False
        tbDireccion.ReadOnly = False
        tbTelefono.ReadOnly = False
        swEstado.IsReadOnly = False
    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreSucursal.ReadOnly = True
        tbDescripcion.ReadOnly = True
        cbDeposito.ReadOnly = True
        tbDireccion.ReadOnly = True
        tbTelefono.ReadOnly = True
        swEstado.IsReadOnly = True
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbNombreSucursal.Text = ""
        tbDescripcion.Text = ""
        tbDireccion.Text = ""
        tbTelefono.Text = ""
        swEstado.Value = True
        seleccionarPrimerItemCombo(cbDeposito)
        tbNombreSucursal.Focus()
    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub


    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbNombreSucursal.BackColor = Color.White
        cbDeposito.BackColor = Color.White
    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        'ByRef _numi As String, DepositoId As Integer,
        '   NombreAlmacen As String, Direccion As String, Descripcion As String, telefono As String, 
        '                                        estado As Integer
        Dim res As Boolean = L_prSucursalInsertar(tbCodigo.Text, cbDeposito.Value, tbNombreSucursal.Text,
                                                  tbDireccion.Text, tbDescripcion.Text,
                                                  tbTelefono.Text, IIf(swEstado.Value = True, 1, 0))
        If res Then
            ToastNotification.Show(Me, "Codigo de Sucursal ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
        End If
        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean

        Dim res As Boolean = L_prSucursalActualizar(tbCodigo.Text, cbDeposito.Value, tbNombreSucursal.Text,
                                                  tbDireccion.Text, tbDescripcion.Text,
                                                  tbTelefono.Text, IIf(swEstado.Value = True, 1, 0))
        If res Then

            ToastNotification.Show(Me, "Codigo de Sucursal ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
            _PSalirRegistro()
        End If
        Return res
    End Function

    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar la Sucursal " + tbNombreSucursal.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_prSucursalBorrar(tbCodigo.Text, mensajeError)
            If res Then
                ToastNotification.Show(Me, "Codigo de Sucursal ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PMFiltrar()
            Else
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If
        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()

        If tbNombreSucursal.Text = String.Empty Then
            tbNombreSucursal.BackColor = Color.Red
            MEP.SetError(tbNombreSucursal, "Ingrese Nombre de Sucursal")
            _ok = False
        Else
            tbNombreSucursal.BackColor = Color.White
            MEP.SetError(tbNombreSucursal, "")
        End If


        If cbDeposito.SelectedIndex < 0 Then
            cbDeposito.BackColor = Color.Red
            MEP.SetError(cbDeposito, "Seleccione Un Deposito")
            _ok = False
        Else
            cbDeposito.BackColor = Color.White
            MEP.SetError(cbDeposito, "")
        End If


        MHighlighterFocus.UpdateHighlights()

        If tbNombreSucursal.Text = String.Empty Then
            tbNombreSucursal.Focus()
            Return _ok
        End If


        If cbDeposito.SelectedIndex < 0 Then
            cbDeposito.Focus()
            Return _ok
        End If

        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Almacenes")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'a.id , a.DepositoId, b.NombreDeposito, a.NombreAlmacen, a.Direccion, isnull(a.Descripcion,'') as Descripcion ,a.Telefono ,
        'a.Latitud, a.Longitud, a.Estado 
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("DepositoId", False))
        listEstCeldas.Add(New Celda("NombreAlmacen", True, "Almacen", 120))
        listEstCeldas.Add(New Celda("NombreDeposito", True, "Depositos", 90))
        listEstCeldas.Add(New Celda("Direccion", True, "Direccion", 90))
        listEstCeldas.Add(New Celda("Descripcion", False))
        listEstCeldas.Add(New Celda("Telefono", False))
        listEstCeldas.Add(New Celda("Longitud", False))
        listEstCeldas.Add(New Celda("Latitud", False))

        listEstCeldas.Add(New Celda("estado", True, "Estado", 60))

        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional bandera As Boolean = False)

        If (bandera = False) Then
            Selected = True
            JGrM_Buscador.Row = _MPos
            Selected = False
        End If

        'a.id , a.DepositoId, b.NombreDeposito, a.NombreAlmacen, a.Direccion,
        'isnull(a.Descripcion,'') as Descripcion ,a.Telefono ,
        'a.Latitud, a.Longitud, a.Estado 
        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbNombreSucursal.Text = .GetValue("NombreAlmacen").ToString
            cbDeposito.Value = .GetValue("DepositoId")
            tbDireccion.Text = .GetValue("Direccion").ToString
            tbDescripcion.Text = .GetValue("Descripcion").ToString
            tbTelefono.Text = .GetValue("Telefono".ToString)

            swEstado.Value = .GetValue("estado")



        End With

        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

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

    Private Sub VerToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles VerToolStripMenuItem1.Click

    End Sub

    Private Sub EditarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditarToolStripMenuItem.Click
        If (JGrM_Buscador.Row >= 0) Then
            btnModificar.PerformClick()

        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 1) Then
            btnEliminar.PerformClick()

        End If
        If (JGrM_Buscador.Row = 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No se Puede Eliminar Todas Las Sucursales", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub

#End Region
End Class