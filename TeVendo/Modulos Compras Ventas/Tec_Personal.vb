﻿
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class Tec_Personal

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
        tbNombreProveedor.Focus()


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

        Me.Text = "Gestion De Personal"
        P_Global._prCargarComboGenerico(cbTipoDocumento, L_prLibreriaDetalleGeneral(8), "cnnum", "Codigo", "cndesc1", "TipoDocumento")
        P_Global._prCargarComboGenerico(cbEmpresa, L_prListaEmpresasUsuarios(), "Id", "Codigo", "Nombre", "Empresa")
        P_Global._prCargarComboGenerico(cbTipoPersonal, L_prLibreriaDetalleGeneral(9), "cnnum", "Codigo", "cndesc1", "TipoPersonal")
        _PMIniciarTodo()
        _prAsignarPermisos()
        _habilitarFocus()



    End Sub

    Public Sub _habilitarFocus()
        With Highlighter2
            .SetHighlightOnFocus(tbNombreProveedor, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDireccion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbFechaNacimiento, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbTelefono01, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNroDocumento, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbTipoDocumento, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnTipoDocumento, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

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

        tbNombreProveedor.ReadOnly = False
        tbDireccion.ReadOnly = False
        tbTelefono01.ReadOnly = False
        cbTipoPersonal.ReadOnly = False
        cbEmpresa.ReadOnly = False
        tbFechaNacimiento.ReadOnly = False
        tbNroDocumento.ReadOnly = False
        cbTipoDocumento.ReadOnly = False

        swEstado.IsReadOnly = False
        btnTipoDocumento.Visible = True


    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreProveedor.ReadOnly = True
        tbDireccion.ReadOnly = True
        tbTelefono01.ReadOnly = True
        cbTipoPersonal.ReadOnly = True
        cbEmpresa.ReadOnly = True
        tbFechaNacimiento.ReadOnly = True
        tbNroDocumento.ReadOnly = True
        cbTipoDocumento.ReadOnly = True

        swEstado.IsReadOnly = True
        btnTipoDocumento.Visible = False
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""

        tbNroDocumento.Text = ""
        tbNombreProveedor.Text = ""
        tbCodigo.Text = ""
        tbDireccion.Text = ""
        tbTelefono01.Text = ""
        tbFechaNacimiento.Value = Now.Date


        swEstado.Value = True
        seleccionarPrimerItemCombo(cbTipoDocumento)
        seleccionarPrimerItemCombo(cbTipoPersonal)
        seleccionarPrimerItemCombo(cbEmpresa)

    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()


        tbNombreProveedor.BackColor = Color.White
        cbTipoDocumento.BackColor = Color.White
        cbTipoPersonal.BackColor = Color.White
        cbEmpresa.BackColor = Color.White

    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        '_Id As String, NombrePersonal As String, Direccion As String,
        '                   Telefono01 As String, TipoDocumento As Integer,
        '                                     NroDocumento As String, TipoPersonal As Integer,
        '                                     Estado As Integer, EmpresaId As Integer
        Dim res As Boolean
        Try
            res = InsertarPersonal(tbCodigo.Text, tbNombreProveedor.Text, tbDireccion.Text, tbTelefono01.Text,
                                   cbTipoDocumento.Value, tbNroDocumento.Text, cbTipoPersonal.Value, IIf(swEstado.Value = True, 1, 0), cbEmpresa.Value, tbFechaNacimiento.Value.ToString("yyyy/MM/dd"))

            If res Then


                ToastNotification.Show(Me, "Codigo de Personal ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar el Personal".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Personal".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = ModificarPersonal(tbCodigo.Text, tbNombreProveedor.Text, tbDireccion.Text, tbTelefono01.Text,
                                   cbTipoDocumento.Value, tbNroDocumento.Text, cbTipoPersonal.Value, IIf(swEstado.Value = True, 1, 0), cbEmpresa.Value, tbFechaNacimiento.Value.ToString("yyyy/MM/dd"))
            If Res Then

                ToastNotification.Show(Me, "Codigo de Personal ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el Personal".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar Personal".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbNombreProveedor.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Personal " + tbNombreProveedor.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_Personal")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Personal ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el Personal".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "

        If tbNombreProveedor.Text = String.Empty Then
            tbNombreProveedor.BackColor = Color.Red
            MEP.SetError(tbNombreProveedor, "Ingrese Nombre de Personal")
            Mensaje = Mensaje + " Nombre Personal"
            _ok = False
        Else
            tbNombreProveedor.BackColor = Color.White
            MEP.SetError(tbNombreProveedor, "")
        End If
        If (cbEmpresa.SelectedIndex < 0) Then
            cbEmpresa.BackColor = Color.Red
            MEP.SetError(cbEmpresa, "Seleccione una Empresa")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Empresa"
            _ok = False
        Else
            cbEmpresa.BackColor = Color.White
            MEP.SetError(cbEmpresa, "")
        End If
        'If (cbTipoPersonal.SelectedIndex < 0) Then
        '    cbTipoPersonal.BackColor = Color.Red
        '    MEP.SetError(cbTipoPersonal, "Seleccione un Tipo Personal")
        '    Mensaje = Mensaje + Chr(13) + Chr(10) + " Tipo Personal"
        '    _ok = False
        'Else
        '    cbTipoPersonal.BackColor = Color.White
        '    MEP.SetError(cbTipoPersonal, "")
        'End If

        If (cbTipoDocumento.SelectedIndex < 0) Then
            cbTipoDocumento.BackColor = Color.Red
            MEP.SetError(cbTipoDocumento, "Seleccione un Tipo Documento")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Tipo Documento"
            _ok = False
        Else
            cbTipoDocumento.BackColor = Color.White
            MEP.SetError(cbTipoDocumento, "")
        End If
        Highlighter2.UpdateHighlights()

        If tbNombreProveedor.Text = String.Empty Then
            tbNombreProveedor.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If
        If (cbEmpresa.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbEmpresa.Focus()
            Return _ok
        End If
        'If (cbTipoPersonal.SelectedIndex < 0) Then
        '    ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
        '    cbTipoPersonal.Focus()
        '    Return _ok
        'End If
        If (cbTipoDocumento.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbTipoDocumento.Focus()
            Return _ok
        End If
        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Personal")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'p.id , p.NombrePersonal, p.Direccion, p.Telefono01, p.TipoDocumento, p.NroDocumento,
        '    p.TipoPersonal, p.Estado, p.EmpresaId 
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))

        listEstCeldas.Add(New Celda("NombrePersonal", True, " NombrePersonal", 200))
        listEstCeldas.Add(New Celda("Direccion", False, " Direccion", 120))
        listEstCeldas.Add(New Celda("Telefono01", False, "Telefono01", 90))
        listEstCeldas.Add(New Celda("TipoDocumento", False))
        listEstCeldas.Add(New Celda("TipoPersonal", False))
        listEstCeldas.Add(New Celda("EmpresaId", False))
        listEstCeldas.Add(New Celda("NroDocumento", True, "Nro Documento", 90))
        listEstCeldas.Add(New Celda("Estado", False))
        listEstCeldas.Add(New Celda("img", True, "Estado", 90))
        listEstCeldas.Add(New Celda("Tipo", True, "Cargo", 90))
        listEstCeldas.Add(New Celda("FechaNacimiento", True, "F.Nacimiento", 90))

        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        'p.id , p.NombrePersonal, p.Direccion, p.Telefono01, p.TipoDocumento, p.NroDocumento,
        '    p.TipoPersonal, p.Estado, p.EmpresaId 
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbNombreProveedor.Text = .GetValue("NombrePersonal").ToString
            tbDireccion.Text = .GetValue("Direccion").ToString
            cbTipoDocumento.Value = .GetValue("TipoDocumento")

            tbNroDocumento.Text = .GetValue("NroDocumento")
            tbTelefono01.Text = .GetValue("Telefono01").ToString
            swEstado.Value = .GetValue("Estado")
            cbTipoPersonal.Value = .GetValue("TipoPersonal")
            cbEmpresa.Value = .GetValue("EmpresaId")
            tbFechaNacimiento.Value = .GetValue("FechaNacimiento")
        End With
        TablaImagenes = L_prCargarImagenesRecepcion(tbCodigo.Text)
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







    Private Sub btnTipoDocumento_Click(sender As Object, e As EventArgs) Handles btnTipoDocumento.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 8
        ef.titulo = "Crear Nuevo Tipo De Documento"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbTipoDocumento, L_prLibreriaDetalleGeneral(8), "cnnum", "Codigo", "cndesc1", "TipoDocumento")
            cbTipoDocumento.SelectedIndex = CType(cbTipoDocumento.DataSource, DataTable).Rows.Count - 1
            cbTipoDocumento.Focus()
        End If

    End Sub



    Private Sub JGrM_Buscador_SelectionChanged(sender As Object, e As EventArgs) Handles JGrM_Buscador.SelectionChanged
        If (JGrM_Buscador.Row >= 0 And FilaSeleccionada = False) Then
            _MPos = JGrM_Buscador.Row

            _PMOMostrarRegistro(_MPos, True)

        End If
    End Sub

    Private Sub VerToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles VerToolStripMenuItem1.Click

    End Sub

    Private Sub EditarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditarToolStripMenuItem.Click
        If (JGrM_Buscador.Row >= 0) Then
            btnModificar.PerformClick()
            tbNombreProveedor.Focus()
        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then
            btnEliminar.PerformClick()
        End If
    End Sub

    Private Sub tbNombreProveedor_TextChanged(sender As Object, e As EventArgs) Handles tbNombreProveedor.TextChanged

    End Sub





    Private Sub P_Moverenfoque()
        SendKeys.Send("{TAB}")
    End Sub

    Private Sub tbNombreProveedor_KeyDown(sender As Object, e As KeyEventArgs) Handles tbNombreProveedor.KeyDown, tbDireccion.KeyDown, tbTelefono01.KeyDown, tbCodigo.KeyDown, cbTipoDocumento.KeyDown, tbNroDocumento.KeyDown, swEstado.KeyDown, tbFechaNacimiento.KeyDown
        If (e.KeyCode = Keys.Enter) Then
            e.Handled = True
            P_Moverenfoque()
        End If
    End Sub




#End Region
    Private Sub Efecto_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Me.Dispose()

    End Sub
End Class