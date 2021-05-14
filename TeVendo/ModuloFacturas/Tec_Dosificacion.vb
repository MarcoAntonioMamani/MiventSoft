
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class Tec_Dosificacion
#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public FilaSeleccionada As Boolean = False
    Public _TabControl As SuperTabControl

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"
    Dim PersonalId As Integer = 0

    Dim TablaImagenes As DataTable
    Dim TablaInventario As DataTable
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim gs_DirPrograma As String = ""
    Dim gs_RutaImg As String = ""



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
                    .WordWrap = True
                    .MaxLines = 4
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
            .Height = 200
        End With



    End Sub



    Public Sub _PMInhabilitar()
        btnNuevo.Visible = False
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
        tbNroAutorizacion.Focus()


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
            _PMSalir()
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
            '_modulo.Select()
            TabControlPrincipal.SelectedTabIndex = 1
            '_tab.Close()
        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()



        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "Dosificación"
        P_Global._prCargarComboGenerico(cbSucursal, L_fnGeneralSucursales(), "aanumi", "Codigo", "aabdes", "Sucursal")



        _PMIniciarTodo()
        _prAsignarPermisos()


        _habilitarFocus()

    End Sub

    Public Sub _habilitarFocus()
        With Highlighter2
            .SetHighlightOnFocus(cbSucursal, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNroAutorizacion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbLlave, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNota01, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNota02, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbFechaInicio, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbFechaLimite, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swTipo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNumeroFactura, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)


        End With
    End Sub


    Private Sub _prCargarComboTipoMovimiento(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prListarTipoMovimientoCaja()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("Id").Width = 70
            .DropDownList.Columns("Id").Caption = "COD"
            .DropDownList.Columns.Add("Descripcion").Width = 200
            .DropDownList.Columns("Descripcion").Caption = "Motivo Movimiento"
            .ValueMember = "Id"
            .DisplayMember = "Descripcion"
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
        '     (@Id,@CodigoExterno ,@CodigoBarras ,@NombreProducto ,@DescripcionProducto ,
        '@StockMinimo ,@estado ,@CategoriaId ,@EmpresaId ,@ProveedorId ,@MarcaId ,
        '@AttributoId ,@FamiliaId ,
        '@UnidadVentaId ,@UnidadMaximaId ,@Conversion ,@newFecha,@newHora,@usuario )


        cbSucursal.ReadOnly = False
        tbNroAutorizacion.ReadOnly = False
        tbLlave.ReadOnly = False
        tbNota01.ReadOnly = False
        tbNota02.ReadOnly = False
        tbFechaInicio.ReadOnly = False
        tbFechaLimite.ReadOnly = False
        tbNumeroFactura.IsInputReadOnly = False
        swEstado.IsReadOnly = False
        swTipo.IsReadOnly = False


    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        cbSucursal.ReadOnly = True
        tbNroAutorizacion.ReadOnly = True
        tbLlave.ReadOnly = True
        tbNota01.ReadOnly = True
        tbNota02.ReadOnly = True
        tbFechaInicio.ReadOnly = True
        tbFechaLimite.ReadOnly = True
        tbNumeroFactura.IsInputReadOnly = True
        swEstado.IsReadOnly = True
        swTipo.IsReadOnly = True
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbFechaInicio.Value = Now.Date
        tbFechaLimite.Value = Now.Date

        tbNroAutorizacion.Clear()
        tbLlave.Clear()
        tbNota01.Clear()
        tbNota02.Clear()

        tbNumeroFactura.Value = 0
        swEstado.Value = True
        swTipo.Value = True
        seleccionarPrimerItemCombo(cbSucursal)



    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbNroAutorizacion.BackColor = Color.White
        tbLlave.BackColor = Color.White
        tbNota01.BackColor = Color.White
        tbNota02.BackColor = Color.White
        tbFechaInicio.BackColor = Color.White
        tbFechaLimite.BackColor = Color.White


    End Sub

    Public Function _PMOGrabarRegistro() As Boolean

        Dim res As Boolean
        Try

            'SucursalId As Integer, NumeroAutorizacion As String, UltimoNroFactura As Integer, Llave As String, FechaInicio As String, FechaLimite As String, Nota01 As String, Nota02 As String, Estado As Integer, TipoComputarizado As Integer
            res = L_prInsertarDosificacion(tbCodigo.Text, cbSucursal.Value, tbNroAutorizacion.Text, tbNumeroFactura.Value, tbLlave.Text, tbFechaInicio.Value.ToString("yyyy/MM/dd"), tbFechaLimite.Value.ToString("yyyy/MM/dd"), tbNota01.Text, tbNota02.Text, IIf(swEstado.Value = True, 1, 0), IIf(swTipo.Value = True, 1, 0))

            If res Then


                ToastNotification.Show(Me, "Dosificación #".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar Dosificación".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar Dosificación".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = L_prModificarDosificacion(tbCodigo.Text, cbSucursal.Value, tbNroAutorizacion.Text, tbNumeroFactura.Value, tbLlave.Text, tbFechaInicio.Value.ToString("yyyy/MM/dd"), tbFechaLimite.Value.ToString("yyyy/MM/dd"), tbNota01.Text, tbNota02.Text, IIf(swEstado.Value = True, 1, 0), IIf(swTipo.Value = True, 1, 0))


            If Res Then


                ToastNotification.Show(Me, "Codigo de Dosificación".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar Dosificación".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar la Dosificación".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbLlave.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar La Dosificación " + tbCodigo.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_Dosificacion")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Dosificación".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar Dosificación".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "
        If tbNumeroFactura.Value < 0 Then
            tbNumeroFactura.BackColor = Color.Red
            MEP.SetError(tbNumeroFactura, "Ingrese un Monto Valido")
            Mensaje = Mensaje + " Monto Valido Mayor a 0"
            _ok = False
        Else
            tbNumeroFactura.BackColor = Color.White
            MEP.SetError(tbNumeroFactura, "")
        End If


        If (cbSucursal.SelectedIndex < 0) Then
            cbSucursal.BackColor = Color.Red
            MEP.SetError(cbSucursal, "Seleccione una Sucursal ")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Sucursal"
            _ok = False
        Else
            cbSucursal.BackColor = Color.White
            MEP.SetError(cbSucursal, "")
        End If



        Highlighter2.UpdateHighlights()

        If tbNumeroFactura.Value < 0 Then
            tbNumeroFactura.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If

        If (cbSucursal.SelectedIndex < 0) Then

            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbSucursal.Focus()
            Return _ok

        End If




        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Dosificacion")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'a.Id,a.SucursalId,suc.NombreAlmacen,a.NumeroAutorizacion,a.UltimoNroFactura,a.Llave,
        'a.FechaInicio, a.FechaLimite, a.Nota01, a.Nota02, a.Estado, a.TipoComputarizado 
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "Movimiento Id", 40))
        listEstCeldas.Add(New Celda("SucursalId", False))
        listEstCeldas.Add(New Celda("FechaInicio", True, "Fecha Inicio", 70, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("FechaLimite", True, "Fecha Inicio", 70, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("NombreAlmacen", True, "Sucursal", 120))
        listEstCeldas.Add(New Celda("UltimoNroFactura", True, "Ult. Nro fact.", 90, "0.00"))
        listEstCeldas.Add(New Celda("NumeroAutorizacion", True, "Nro Autorización", 100, ""))
        listEstCeldas.Add(New Celda("Llave", False))
        listEstCeldas.Add(New Celda("Nota01", False))
        listEstCeldas.Add(New Celda("Nota02", False))

        listEstCeldas.Add(New Celda("Estado", False))
        listEstCeldas.Add(New Celda("TipoComputarizado", False))

        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        'a.Id,a.SucursalId,suc.NombreAlmacen,a.NumeroAutorizacion,a.UltimoNroFactura,a.Llave,
        'a.FechaInicio, a.FechaLimite, a.Nota01, a.Nota02, a.Estado, a.TipoComputarizado 
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            cbSucursal.Value = .GetValue("SucursalId")
            tbNroAutorizacion.Text = .GetValue("NumeroAutorizacion").ToString
            tbNumeroFactura.Value = .GetValue("UltimoNroFactura")
            tbLlave.Text = .GetValue("Llave")
            tbFechaInicio.Value = .GetValue("FechaInicio")
            tbFechaLimite.Value = .GetValue("FechaLimite")
            tbNota01.Text = .GetValue("Nota01")
            tbNota02.Text = .GetValue("Nota02")
            swEstado.Value = .GetValue("Estado")
            swTipo.Value = .GetValue("TipoComputarizado")
        End With

        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub

    Private Function P_fnObtenerID() As String
        Dim res As String = ""
        res = res + Now.Hour.ToString("00") + Now.Minute.ToString("00") + Now.Second.ToString("00") + "_" _
            + Now.Day.ToString("00") + Now.Month.ToString("00") + Now.Year.ToString("0000")
        Return res
    End Function

    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()
            TabControlPrincipal.SelectedTabIndex = 1
        Else
            TabControlPrincipal.SelectedTabIndex = 1
            '  Public _modulo As SideNavItem
            '_TabControl.SelectedTab = _modulo
            '_tab.Close()
            'Me.Close()
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
        TabControlPrincipal.SelectedTabIndex = 1
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




    Private Sub Panel1_MouseHover(sender As Object, e As EventArgs) Handles btnSi.MouseHover
        btnSi.BackColor = Color.FromArgb(30, 199, 165)
    End Sub

    Private Sub Panel1_MouseLeave(sender As Object, e As EventArgs) Handles btnSi.MouseLeave
        btnSi.BackColor = Color.FromArgb(26, 179, 148)
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        TabControlPrincipal.SelectedTabIndex = 0
        btnNuevo.PerformClick()

    End Sub

    Private Sub JGrM_Buscador_KeyDown(sender As Object, e As KeyEventArgs) Handles JGrM_Buscador.KeyDown
        If (e.KeyCode = Keys.Enter) Then

            TabControlPrincipal.SelectedTabIndex = 0

        End If
    End Sub
    Private Sub JGrM_Buscador_DoubleClick(sender As Object, e As EventArgs) Handles JGrM_Buscador.DoubleClick
        If (JGrM_Buscador.Row >= 0) Then
            TabControlPrincipal.SelectedTabIndex = 0
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

        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then


            btnEliminar.PerformClick()

        End If
    End Sub



    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub









#End Region
End Class