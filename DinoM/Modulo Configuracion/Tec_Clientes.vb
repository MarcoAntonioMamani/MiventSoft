Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls

Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips
Public Class Tec_Clientes


#Region "Atributos"
    Dim _Punto As Integer
    Dim _ListPuntos As List(Of PointLatLng)
    Dim _Overlay As GMapOverlay
    Dim _latitud As Double = 0
    Dim _longitud As Double = 0

    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public FilaSeleccionada As Boolean = False

    Public _MListEstBuscador As List(Of Modelo.Celda)
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

    End Sub



    Public Sub _PMInhabilitar()
        btnNuevo.Visible = True
        btnModificar.Visible = True
        btnEliminar.Visible = True
        btnGrabar.Visible = False
        PanelNavegacion.Enabled = True
        JGrM_Buscador.Enabled = True
        btnTipoDocumento.Visible = False

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
        tbNombreCliente.Focus()


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
        _prInicarMapa()
        Me.Text = "Gestion De Clientes"
        P_Global._prCargarComboGenerico(cbTipoDocumento, L_prLibreriaDetalleGeneral(8), "Id", "Codigo", "Nombre", "TipoDocumento")
        P_Global._prCargarComboGenerico(cbPrecios, L_prListaCategoriasPrecios(), "Id", "Codigo", "Descripcion", "CategoriaPrecio")
        P_Global._prCargarComboGenerico(cbZona, L_prListarZonas(), "Id", "Codigo", "NombreZona", "Zonas")

        _PMIniciarTodo()
        _prAsignarPermisos()


        Dim blah As New Bitmap(New Bitmap(My.Resources.ic_c), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico

    End Sub
    Private Sub P_IniciarMap()
        Gmc_Cliente.DragButton = MouseButtons.Left
        Gmc_Cliente.CanDragMap = True
        Gmc_Cliente.MapProvider = GMapProviders.GoogleMap
        If (_latitud <> 0 And _longitud <> 0) Then

            Gmc_Cliente.Position = New PointLatLng(_latitud, _longitud)
        Else

            _Overlay.Markers.Clear()
            Gmc_Cliente.Position = New PointLatLng(-17.3931784, -66.1738852)
        End If

        Gmc_Cliente.MinZoom = 0
        Gmc_Cliente.MaxZoom = 24
        Gmc_Cliente.Zoom = 15.5
        Gmc_Cliente.AutoScroll = True

        GMapProvider.Language = LanguageType.Spanish
    End Sub

    Public Sub _prInicarMapa()
        _Punto = 0
        '_ListPuntos = New List(Of PointLatLng)
        _Overlay = New GMapOverlay("points")
        Gmc_Cliente.Overlays.Add(_Overlay)
        P_IniciarMap()
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

        tbCodigoExterno.ReadOnly = False
        tbNombreCliente.ReadOnly = False
        tbDireccionCliente.ReadOnly = False
        tbTelefono.ReadOnly = False
        tbNroDocumento.ReadOnly = False
        cbTipoDocumento.ReadOnly = False
        tbRazonSocial.ReadOnly = False
        tbnit.ReadOnly = False
        swEstado.IsReadOnly = False



    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbCodigoExterno.ReadOnly = True

        swEstado.IsReadOnly = True
        tbCodigoExterno.ReadOnly = True
        tbNombreCliente.ReadOnly = True
        tbDireccionCliente.ReadOnly = True
        tbTelefono.ReadOnly = True
        tbNroDocumento.ReadOnly = True
        cbTipoDocumento.ReadOnly = True
        tbRazonSocial.ReadOnly = True
        tbnit.ReadOnly = True
        swEstado.IsReadOnly = True
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbCodigoExterno.Text = ""
        swEstado.Value = True
        seleccionarPrimerItemCombo(cbTipoDocumento)
        seleccionarPrimerItemCombo(cbZona)
        seleccionarPrimerItemCombo(cbPrecios)
        _latitud = 0
        _longitud = 0

    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()


        tbNombreCliente.BackColor = Color.White


    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        '_Id As String, IdZona As Integer, IdPrecio As Integer, CodigoExterno As String, NombreCliente As String,
        '                                   Direccion As String, Telefono As String, TipoDocumento As Integer, NroDocumento As String,
        '                                   RazonSocial As String, nit As String, estado As Integer, FechaIngreso As String, Latitud As Double,
        '                                   Longitud As Double
        Dim res As Boolean
        Try
            res = InsertarCliente(tbCodigo.Text, cbZona.Value, cbPrecios.Value, tbCodigoExterno.Text, tbNombreCliente.Text,
                                  tbDireccionCliente.Text, tbTelefono.Text, cbTipoDocumento.Value, tbNroDocumento.Text,
                                  tbRazonSocial.Text, tbnit.Text, IIf(swEstado.Value = True, 1, 0), Now.Date.ToString("dd/MM/yyyy"), _latitud, _longitud)

            If res Then


                ToastNotification.Show(Me, "Codigo de Cliente ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar el Cliente".ToUpper, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Cliente".ToUpper + " " + ex.Message, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = ModificarCliente(tbCodigo.Text, cbZona.Value, cbPrecios.Value, tbCodigoExterno.Text, tbNombreCliente.Text,
                                  tbDireccionCliente.Text, tbTelefono.Text, cbTipoDocumento.Value, tbNroDocumento.Text,
                                  tbRazonSocial.Text, tbnit.Text, IIf(swEstado.Value = True, 1, 0), Now.Date.ToString("dd/MM/yyyy"), _latitud, _longitud)

            If Res Then

                ToastNotification.Show(Me, "Codigo de cliente ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el cliente".ToUpper, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar cliente".ToUpper + " " + ex.Message, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbNombreCliente.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el cliente " + tbNombreCliente.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prProductoBorrar(tbCodigo.Text, mensajeError)
                If res Then

                    ToastNotification.Show(Me, "Codigo de Cliente ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el cliente".ToUpper + " " + ex.Message, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "

        If tbNombreCliente.Text = String.Empty Then
            tbNombreCliente.BackColor = Color.Red
            MEP.SetError(tbNombreCliente, "Ingrese Nombre de cliente")
            Mensaje = Mensaje + " Nombre Cliente"
            _ok = False
        Else
            tbNombreCliente.BackColor = Color.White
            MEP.SetError(tbNombreCliente, "")
        End If

        MHighlighterFocus.UpdateHighlights()

        If tbNombreCliente.Text = String.Empty Then
            tbNombreCliente.Focus()
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If


        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("[MAM_Clientes]")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Modelo.Celda)

        'a.id , a.CodigoExterno, a.NombreCliente, a.DireccionCliente, a.Telefono, a.Observacion, a.TipoDocumento, a.NroDocumento,
        '    a.RazonSocial, a.Nit, a.Estado, a.FechaIngreso, a.FechaUltimaVenta, a.ImagenCliente, a.Latitud, a.Longitud  
        Dim listEstCeldas As New List(Of Modelo.Celda)
        listEstCeldas.Add(New Modelo.Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Modelo.Celda("CodigoExterno", False))
        listEstCeldas.Add(New Modelo.Celda("NombreCliente", True, " NombreCliente", 120))
        listEstCeldas.Add(New Modelo.Celda("DireccionCliente", True, " Direccion", 120))
        listEstCeldas.Add(New Modelo.Celda("Telefono", True, "Telefono", 90))
        listEstCeldas.Add(New Modelo.Celda("TipoDocumento", False))
        listEstCeldas.Add(New Modelo.Celda("NroDocumento", True, " NroDocumento", 120))
        listEstCeldas.Add(New Modelo.Celda("RazonSocial", False, "Razon Social", 80))
        listEstCeldas.Add(New Modelo.Celda("Nit", False, "nit", 70))
        listEstCeldas.Add(New Modelo.Celda("Estado", True, "Estado", 60))
        listEstCeldas.Add(New Modelo.Celda("FechaIngreso", True, "FechaIngreso", 90))
        listEstCeldas.Add(New Modelo.Celda("FechaUltimaVenta", False))
        listEstCeldas.Add(New Modelo.Celda("ImagenCliente", False))
        listEstCeldas.Add(New Modelo.Celda("Latitud", False))
        listEstCeldas.Add(New Modelo.Celda("Longitud", False))


        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        'a.id , a.CodigoExterno, a.NombreCliente, a.DireccionCliente, a.Telefono, a.Observacion, a.TipoDocumento, a.NroDocumento,
        '    a.RazonSocial, a.Nit, a.Estado, a.FechaIngreso, a.FechaUltimaVenta, a.ImagenCliente, a.Latitud, a.Longitud   
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbCodigoExterno.Text = .GetValue("CodigoExterno").ToString
            tbNombreCliente.Text = .GetValue("NombreCliente").ToString
            tbDireccionCliente.Text = .GetValue("DireccionCliente").ToString
            cbTipoDocumento.Value = .GetValue("TipoDocumento")
            tbNroDocumento.Text = .GetValue("NroDocumento")
            tbRazonSocial.Text = .GetValue("RazonSocial").ToString
            tbnit.Text = .GetValue("Nit").ToString
            swEstado.Value = .GetValue("Estado")
            _latitud = .GetValue("Latitud")
            _longitud = .GetValue("Longitud")
        End With
        TablaImagenes = L_prCargarImagenesRecepcion(tbCodigo.Text)
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

        _dibujarUbicacion(JGrM_Buscador.GetValue("NombreCliente").ToString, JGrM_Buscador.GetValue("Id").ToString)

    End Sub
    Public Sub _dibujarUbicacion(_nombre As String, _ci As String)
        If (_latitud <> 0 And _longitud <> 0) Then
            Dim plg As PointLatLng = New PointLatLng(_latitud, _longitud)
            _Overlay.Markers.Clear()
            P_AgregarPunto(plg, _nombre, _ci)
        Else


            _Overlay.Markers.Clear()
            Gmc_Cliente.Position = New PointLatLng(-17.3931784, -66.1738852)
        End If
    End Sub
    Private Sub P_AgregarPunto(pointLatLng As PointLatLng, _nombre As String, _ci As String)
        If (Not IsNothing(_Overlay)) Then
            'añadir puntos
            'Dim markersOverlay As New GMapOverlay("markers")
            Dim marker As New GMarkerGoogle(pointLatLng, My.Resources.markerIcono)
            'añadir tooltip
            Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver
            marker.ToolTip = New GMapBaloonToolTip(marker)
            marker.ToolTipMode = mode
            Dim ToolTipBackColor As New SolidBrush(Color.Blue)
            marker.ToolTip.Fill = ToolTipBackColor
            marker.ToolTip.Foreground = Brushes.White
            'If (Not _nombre.ToString = String.Empty) Then
            '    marker.ToolTipText = "CLIENTE: " + _nombre & vbNewLine & " CI:" + _ci
            'End If
            _Overlay.Markers.Add(marker)
            'mapa.Overlays.Add(markersOverlay)
            Gmc_Cliente.Position = pointLatLng
        End If
    End Sub

    Private Sub ButtonX3_Click(sender As Object, e As EventArgs) Handles ButtonX3.Click
        If (Gmc_Cliente.Zoom >= Gmc_Cliente.MinZoom) Then
            Gmc_Cliente.Zoom = Gmc_Cliente.Zoom - 1
        End If
    End Sub

    Private Sub ButtonX4_Click(sender As Object, e As EventArgs) Handles ButtonX4.Click
        If (Gmc_Cliente.Zoom <= Gmc_Cliente.MaxZoom) Then
            Gmc_Cliente.Zoom = Gmc_Cliente.Zoom + 1
        End If
    End Sub
    Private Sub Gmc_Cliente_DoubleClick(sender As Object, e As EventArgs) Handles Gmc_Cliente.DoubleClick
        If (btnGrabar.Enabled = True) Then


            _Overlay.Markers.Clear()

            Dim gm As GMapControl = CType(sender, GMapControl)
            Dim hj As MouseEventArgs = CType(e, MouseEventArgs)
            Dim plg As PointLatLng = gm.FromLocalToLatLng(hj.X, hj.Y)
            _latitud = plg.Lat
            _longitud = plg.Lng
            ''  MsgBox("latitud:" + Str(plg.Lat) + "   Logitud:" + Str(plg.Lng))

            P_AgregarPunto(plg, "", "")

            '' _ListPuntos.Add(plg)
            'Btnx_ChekGetPoint.Visible = False
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

    Private Sub cbTipoDocumento_ValueChanged(sender As Object, e As EventArgs) Handles cbTipoDocumento.ValueChanged

        If cbTipoDocumento.SelectedIndex < 0 And cbTipoDocumento.Text <> String.Empty Then
            btnTipoDocumento.Visible = True
        Else
            btnTipoDocumento.Visible = False
        End If

    End Sub

    Private Sub btnTipoDocumento_Click(sender As Object, e As EventArgs) Handles btnTipoDocumento.Click
        Dim numi As String = ""

        If L_prClasificadorGrabar(numi, 8, cbTipoDocumento.Text) Then
            P_Global._prCargarComboGenerico(cbTipoDocumento, L_prLibreriaDetalleGeneral(8), "cnnum", "Codigo", "cndesc1", "TipoDocumento")
            cbTipoDocumento.SelectedIndex = CType(cbTipoDocumento.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub









#End Region
End Class