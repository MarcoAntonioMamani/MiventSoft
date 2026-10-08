Imports Negocio.AccesoLogica
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
Imports System.Text.RegularExpressions

Public Class Tec_Clientes


#Region "Atributos"
    Dim _Punto As Integer
    Dim _ListPuntos As List(Of PointLatLng)
    Dim _Overlay As GMapOverlay
    Dim _latitud As Double = 0
    Dim _longitud As Double = 0
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Public FilaSeleccionada As Boolean = False

    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"
    Dim Mapa As Integer = 0

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
                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("imgEstado") = Bin.GetBuffer
            Else
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.pasivo, 110, 30)
                img.Save(Bin, Imaging.ImageFormat.Png)
                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("imgEstado") = Bin.GetBuffer
            End If

        Next

    End Sub

    Public Sub _PMInhabilitar()
        btnNuevo.Visible = False
        btnModificar.Visible = False
        btnEliminar.Visible = False
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
            '_tab.Close()
            TabControlPrincipal.SelectedTabIndex = 1
        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()
        L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        LeerConfiguracion()

        Me.Text = "Gestion De Clientes"
        P_Global._prCargarComboGenerico(cbTipoDocumento, L_prLibreriaDetalleGeneral(8), "cnnum", "Codigo", "cndesc1", "TipoDocumento")
        P_Global._prCargarComboGenerico(cbPrecios, L_prListaCategoriasPrecios(), "Id", "Codigo", "Descripcion", "CategoriaPrecio")
        P_Global._prCargarComboGenerico(cbZona, L_prListarZonas(), "Id", "Codigo", "NombreZona", "Zonas")
        If (Mapa = 1) Then
            PanelRight.Visible = True

            _prInicarMapa()
        Else
            PanelRight.Visible = False
            PanelLEft.Dock = DockStyle.Fill
            Panel13.Visible = False
        End If

        _PMIniciarTodo()
        _prAsignarPermisos()

        _habilitarFocus()

    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

            .SetHighlightOnFocus(tbCodigoExterno, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNombreCliente, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDireccionCliente, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbTelefono, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbTipoDocumento, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNroDocumento, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbRazonSocial, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbnit, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbPrecios, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbZona, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub
    Public Sub LeerConfiguracion()
        Dim dt As DataTable = L_prLeerConfiguracion()
        If (dt.Rows.Count > 0) Then
            Mapa = dt.Rows(0).Item("Mapa")
        End If
    End Sub
    Private Sub P_IniciarMap()
        Gmc_Cliente.DragButton = MouseButtons.Left
        Gmc_Cliente.CanDragMap = True
        Gmc_Cliente.MapProvider = GMapProviders.GoogleMap
        If (_latitud <> 0 And _longitud <> 0) Then

            Gmc_Cliente.Position = New PointLatLng(_latitud, _longitud)
        Else

            _Overlay.Markers.Clear()
            Gmc_Cliente.Position = New PointLatLng(-14.8325109, -64.9159598)
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
        cbZona.ReadOnly = False
        cbPrecios.ReadOnly = False
        btnTipoDocumento.Visible = True
        tbNombreCliente.Focus()
    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbCodigoExterno.ReadOnly = True
        btnTipoDocumento.Visible = False
        swEstado.IsReadOnly = True
        tbCodigoExterno.ReadOnly = True
        tbNombreCliente.ReadOnly = True
        tbDireccionCliente.ReadOnly = True
        tbTelefono.ReadOnly = True
        tbNroDocumento.ReadOnly = True
        cbTipoDocumento.ReadOnly = True
        cbZona.ReadOnly = True
        cbPrecios.ReadOnly = True
        tbRazonSocial.ReadOnly = True
        tbnit.ReadOnly = True
        swEstado.IsReadOnly = True
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbCodigoExterno.Text = ""
        tbNombreCliente.Text = ""
        tbDireccionCliente.Text = ""
        tbTelefono.Text = ""
        tbNroDocumento.Text = ""
        tbRazonSocial.Text = ""
        tbnit.Text = ""

        swEstado.Value = True
        seleccionarPrimerItemCombo(cbTipoDocumento)
        seleccionarPrimerItemCombo(cbZona)
        seleccionarPrimerItemCombo(cbPrecios)
        _latitud = 0
        _longitud = 0

        If (Mapa = 1) Then
            _Overlay.Markers.Clear()
        End If


        tbNombreCliente.Focus()
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
                                  tbRazonSocial.Text, tbnit.Text, IIf(swEstado.Value = True, 1, 0), Now.Date.ToString("yyyy/MM/dd"), _latitud, _longitud)

            If res Then


                ToastNotification.Show(Me, "Codigo de Cliente ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else

                ToastNotification.Show(Me, "Error al guardar el Cliente".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Cliente".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = ModificarCliente(tbCodigo.Text, cbZona.Value, cbPrecios.Value, tbCodigoExterno.Text, tbNombreCliente.Text,
                                  tbDireccionCliente.Text, tbTelefono.Text, cbTipoDocumento.Value, tbNroDocumento.Text,
                                  tbRazonSocial.Text, tbnit.Text, IIf(swEstado.Value = True, 1, 0), Now.Date.ToString("yyyy/MM/dd"), _latitud, _longitud)

            If Res Then

                ToastNotification.Show(Me, "Codigo de cliente ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el cliente".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar cliente".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

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
                res = L_prClienteBorrar(tbCodigo.Text, mensajeError)
                If res Then

                    ToastNotification.Show(Me, "Codigo de Cliente ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el cliente".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

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
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If


        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Clientes")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'a.id , a.CodigoExterno, a.NombreCliente, a.DireccionCliente, a.Telefono, a.Observacion, a.TipoDocumento, a.NroDocumento,
        '    a.RazonSocial, a.Nit, a.Estado, a.FechaIngreso, a.FechaUltimaVenta, a.ImagenCliente, a.Latitud, a.Longitud  
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("CodigoExterno", False))
        listEstCeldas.Add(New Celda("NombreCliente", True, " NombreCliente", 200))
        listEstCeldas.Add(New Celda("DireccionCliente", True, " Direccion", 120))
        listEstCeldas.Add(New Celda("Telefono", True, "Telefono", 90))
        listEstCeldas.Add(New Celda("TipoDocumento", False))
        listEstCeldas.Add(New Celda("NroDocumento", True, " NroDocumento", 120))
        listEstCeldas.Add(New Celda("RazonSocial", False, "Razon Social", 80))
        listEstCeldas.Add(New Celda("Nit", False, "nit", 70))
        listEstCeldas.Add(New Celda("Estado", False, "Estado", 60))
        listEstCeldas.Add(New Celda("imgEstado", True, "Estado", 80))
        listEstCeldas.Add(New Celda("FechaIngreso", True, "FechaIngreso", 90))
        listEstCeldas.Add(New Celda("FechaUltimaVenta", False))
        listEstCeldas.Add(New Celda("ImagenCliente", False))
        listEstCeldas.Add(New Celda("Latitud", False))
        listEstCeldas.Add(New Celda("Longitud", False))
        listEstCeldas.Add(New Celda("PrecioCategoriaId", False))
        listEstCeldas.Add(New Celda("ZonaId", False))


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
            cbPrecios.Value = .GetValue("PrecioCategoriaId")
            cbZona.Value = .GetValue("ZonaId")
            tbNroDocumento.Text = .GetValue("NroDocumento")
            tbTelefono.Text = .GetValue("Telefono")
            tbRazonSocial.Text = .GetValue("RazonSocial").ToString
            tbnit.Text = .GetValue("Nit").ToString
            swEstado.Value = .GetValue("estado")
            _latitud = .GetValue("Latitud")
            _longitud = .GetValue("Longitud")
        End With
        TablaImagenes = L_prCargarImagenesRecepcion(tbCodigo.Text)
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString
        If (Mapa = 1) Then
            _dibujarUbicacion(JGrM_Buscador.GetValue("NombreCliente").ToString, JGrM_Buscador.GetValue("Id").ToString)
        End If


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
            Dim marker As New GMarkerGoogle(pointLatLng, My.Resources.iconmarker)
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
            TabControlPrincipal.SelectedTabIndex = 1
        Else
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





    Private Sub cbTipoDocumento_ValueChanged(sender As Object, e As EventArgs)

        If cbTipoDocumento.SelectedIndex < 0 And cbTipoDocumento.Text <> String.Empty Then
            btnTipoDocumento.Visible = True
        Else
            btnTipoDocumento.Visible = False
        End If

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

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        TabControlPrincipal.SelectedTabIndex = 0
        btnNuevo.PerformClick()
        tbNombreCliente.Focus()
    End Sub

    Private Sub JGrM_Buscador_KeyDown(sender As Object, e As KeyEventArgs) Handles JGrM_Buscador.KeyDown
        If (e.KeyCode = Keys.Enter) Then

            TabControlPrincipal.SelectedTabIndex = 0

        End If
    End Sub

    Private Sub JGrM_Buscador_SelectionChanged(sender As Object, e As EventArgs) Handles JGrM_Buscador.SelectionChanged
        If (JGrM_Buscador.Row >= 0 And FilaSeleccionada = False) Then
            _MPos = JGrM_Buscador.Row

            _PMOMostrarRegistro(_MPos, True)

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
    Private Sub NavigateWebURL(ByVal URL As String, Optional browser As String = "default")

        If Not (browser = "default") Then
            Try
                '// try set browser if there was an error (browser not installed)
                Process.Start(browser, URL)
            Catch ex As Exception
                '// use default browser
                Process.Start(URL)
            End Try

        Else
            '// use default browser
            Process.Start(URL)

        End If

    End Sub
    Public Function ValidarTelefono(strNumber As String)
        Dim regex As Regex = New Regex("\A[0-9]{8,8}\z")
        Dim match As Match = regex.Match(strNumber)

        If (match.Success) Then

            Return True

        Else
            Return False
        End If

    End Function
    Private Sub LinkWhatsappToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LinkWhatsappToolStripMenuItem.Click
        Dim Url As String = "https://wa.me/+591".ToString + tbTelefono.Text + "/?text="
        If (ValidarTelefono(tbTelefono.Text)) Then
            NavigateWebURL(Url)

        Else
            ToastNotification.Show(Me, "Numero Telefono Invalido".ToUpper, img, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub








#End Region

    Private Sub Tec_Precios_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Me.Dispose()
    End Sub
#Region "Reporte de Clientes"

    'Boton y opcion del menu llaman al mismo procedimiento
    Private Sub btnReporteClientes_Click(sender As Object, e As EventArgs) Handles btnReporteClientes.Click
        P_GenerarReporteClientes()
    End Sub

    Private Sub ReporteClientesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReporteClientesToolStripMenuItem.Click
        P_GenerarReporteClientes()
    End Sub

    'Reporte en HTML (sin Crystal): se arma una pagina con todos los clientes en orden alfabetico,
    'se guarda en la carpeta temporal de Windows y se abre en el navegador (desde ahi se imprime o se guarda como PDF)
    Private Sub P_GenerarReporteClientes()
        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
        Try
            'Misma consulta del buscador (MAM_Clientes @tipo=3), ordenada en codigo por nombre
            Dim dt As DataTable = L_prListarGeneral("MAM_Clientes")
            If (dt.Rows.Count = 0) Then
                ToastNotification.Show(Me, "No hay clientes para mostrar".ToUpper, img, 3000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Return
            End If
            'Se ordena por el nombre normalizado (sin espacios de mas, mayusculas, sin tildes)
            'asi los repetidos quedan uno debajo del otro aunque esten escritos distinto
            Dim dtOrden As DataTable = dt.Copy()
            dtOrden.Columns.Add("NombreOrden", GetType(String))
            For i As Integer = 0 To dtOrden.Rows.Count - 1 Step 1
                dtOrden.Rows(i).Item("NombreOrden") = _fnNormalizarNombre(dtOrden.Rows(i).Item("NombreCliente"))
            Next
            Dim vista As New DataView(dtOrden)
            vista.Sort = "NombreOrden ASC"

            Dim html As String = _fnArmarHtmlClientes(vista)

            P_BorrarReportesClientesAnteriores()
            Dim ruta As String = Path.Combine(Path.GetTempPath(), "ReporteClientes_" + Now.ToString("yyyyMMdd_HHmmss") + ".html")
            File.WriteAllText(ruta, html, System.Text.Encoding.UTF8)
            Process.Start(ruta)

        Catch ex As Exception
            ToastNotification.Show(Me, "Error al generar el reporte de clientes: ".ToUpper + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End Try
    End Sub

    Public Function _fnArmarHtmlClientes(vista As DataView) As String
        Dim sb As New System.Text.StringBuilder

        sb.AppendLine("<!DOCTYPE html>")
        sb.AppendLine("<html lang='es'><head><meta charset='utf-8'>")
        sb.AppendLine("<title>Reporte de Clientes</title>")
        sb.AppendLine("<style>")
        sb.AppendLine("body{font-family:Calibri,Arial,sans-serif;margin:24px;color:#222}")
        sb.AppendLine(".cab{display:flex;align-items:center;gap:16px;border-bottom:2px solid #2874AA;padding-bottom:8px}")
        sb.AppendLine(".cab img{max-height:70px}")
        sb.AppendLine("h1{margin:0;font-size:22px;color:#2874AA}")
        sb.AppendLine(".info{font-size:12px;color:#555}")
        sb.AppendLine(".barra{margin:12px 0;display:flex;gap:8px}")
        sb.AppendLine("#buscar{flex:1;padding:6px;font-size:14px}")
        sb.AppendLine("#soloRep{width:18px;height:18px;cursor:pointer}")
        sb.AppendLine(".barra button{padding:6px 14px;font-size:14px;background:#1AB394;color:#fff;border:0;cursor:pointer}")
        sb.AppendLine("table{width:100%;border-collapse:collapse;font-size:12px}")
        sb.AppendLine("th{background:#2874AA;color:#fff;text-align:left;padding:5px}")
        sb.AppendLine("td{padding:4px 5px;border-bottom:1px solid #ddd}")
        sb.AppendLine("tr:nth-child(even) td{background:#f4f8fb}")
        sb.AppendLine("tr.rep td{background:#FFF176 !important}")
        sb.AppendLine(".leyenda{display:inline-block;background:#FFF176;padding:1px 8px;border:1px solid #d4c200}")
        sb.AppendLine(".barra label{display:flex;align-items:center;gap:4px;font-size:14px;white-space:nowrap;cursor:pointer}")
        sb.AppendLine("@media print{.barra{display:none} th,tr.rep td{-webkit-print-color-adjust:exact;print-color-adjust:exact}}")
        sb.AppendLine("</style></head><body>")

        'Contar cuantas veces aparece cada nombre (normalizado) para marcar los repetidos
        Dim conteoNombres As New Dictionary(Of String, Integer)
        For i As Integer = 0 To vista.Count - 1 Step 1
            Dim clave As String = _fnNormalizarNombre(vista(i).Item("NombreCliente"))
            If (clave = String.Empty) Then
                Continue For
            End If
            If (conteoNombres.ContainsKey(clave)) Then
                conteoNombres(clave) = conteoNombres(clave) + 1
            Else
                conteoNombres.Add(clave, 1)
            End If
        Next
        Dim totalRepetidos As Integer = 0
        For i As Integer = 0 To vista.Count - 1 Step 1
            Dim clave As String = _fnNormalizarNombre(vista(i).Item("NombreCliente"))
            If (clave <> String.Empty AndAlso conteoNombres(clave) > 1) Then
                totalRepetidos = totalRepetidos + 1
            End If
        Next

        'Cabecera con logo de la empresa (si existe)
        sb.AppendLine("<div class='cab'>")
        Dim logo As String = _fnLogoEmpresaBase64()
        If (logo <> String.Empty) Then
            sb.AppendLine("<img src='data:image/png;base64," + logo + "'>")
        End If
        sb.AppendLine("<div><h1>REPORTE DE CLIENTES</h1>")
        sb.AppendLine("<div class='info'>Orden alfabetico &middot; Total: " + vista.Count.ToString + " clientes &middot; Generado: " +
                      Now.ToString("dd/MM/yyyy HH:mm") + " &middot; Usuario: " + _fnHtml(gs_user) + "</div>")
        If (totalRepetidos > 0) Then
            sb.AppendLine("<div class='info'><span class='leyenda'>Amarillo</span> = nombre repetido (" + totalRepetidos.ToString + " clientes)</div>")
        End If
        sb.AppendLine("</div>")
        sb.AppendLine("</div>")

        sb.AppendLine("<div class='barra'><input id='buscar' placeholder='Buscar cliente, telefono, documento...' oninput='filtrarDespues()' onkeyup='filtrarDespues()'>")
        If (totalRepetidos > 0) Then
            sb.AppendLine("<label for='soloRep'><input type='checkbox' id='soloRep' onclick='filtrar()'> Solo repetidos</label>")
        End If
        sb.AppendLine("<button onclick='window.print()'>Imprimir / PDF</button></div>")

        sb.AppendLine("<table id='tabla'><thead><tr>")
        sb.AppendLine("<th>#</th><th>Codigo</th><th>Nombre</th><th>Direccion</th><th>Telefono</th><th>Nro Documento</th><th>Razon Social</th><th>NIT</th><th>Veces</th>")
        sb.AppendLine("</tr></thead><tbody>")

        For i As Integer = 0 To vista.Count - 1 Step 1
            Dim fila As DataRowView = vista(i)
            Dim claveFila As String = _fnNormalizarNombre(fila.Item("NombreCliente"))
            If (claveFila <> String.Empty AndAlso conteoNombres(claveFila) > 1) Then
                sb.Append("<tr class='rep'>")
            Else
                sb.Append("<tr>")
            End If
            sb.Append("<td>" + (i + 1).ToString + "</td>")
            sb.Append("<td>" + _fnHtml(fila.Item("Id")) + "</td>")
            sb.Append("<td>" + _fnHtml(fila.Item("NombreCliente")) + "</td>")
            sb.Append("<td>" + _fnHtml(fila.Item("DireccionCliente")) + "</td>")
            sb.Append("<td>" + _fnHtml(fila.Item("Telefono")) + "</td>")
            sb.Append("<td>" + _fnHtml(fila.Item("NroDocumento")) + "</td>")
            sb.Append("<td>" + _fnHtml(fila.Item("RazonSocial")) + "</td>")
            sb.Append("<td>" + _fnHtml(fila.Item("Nit")) + "</td>")
            If (claveFila <> String.Empty AndAlso conteoNombres(claveFila) > 1) Then
                sb.Append("<td><b>" + conteoNombres(claveFila).ToString + "</b></td>")
            Else
                sb.Append("<td></td>")
            End If
            sb.AppendLine("</tr>")
        Next

        sb.AppendLine("</tbody></table>")

        'Buscador dentro de la pagina
        sb.AppendLine("<script>")
        'Filtro rapido: el texto de cada fila se lee UNA sola vez (antes se leia innerText de cada fila
        'en cada filtrado, lo que obliga al navegador a recalcular la pagina por cada fila y la colgaba).
        'Durante el filtrado la tabla se oculta para que el navegador dibuje una sola vez al final.
        sb.AppendLine("var filas=null,textos=[],reps=[],espera=null;")
        sb.AppendLine("function iniciar(){var tb=document.getElementById('tabla').tBodies[0];filas=tb.rows;")
        sb.AppendLine("for(var i=0;i<filas.length;i++){var tx=filas[i].textContent||filas[i].innerText||'';")
        sb.AppendLine("textos.push(tx.toLowerCase());reps.push(filas[i].className.indexOf('rep')>=0);}}")
        sb.AppendLine("function filtrar(){if(filas===null){iniciar();}")
        sb.AppendLine("var t=document.getElementById('buscar').value.toLowerCase();")
        sb.AppendLine("var c=document.getElementById('soloRep');var sr=c?c.checked:false;")
        sb.AppendLine("var tb=document.getElementById('tabla').tBodies[0];tb.style.display='none';")
        sb.AppendLine("for(var i=0;i<filas.length;i++){var ok=(t==='' || textos[i].indexOf(t)>=0);")
        sb.AppendLine("if(sr && !reps[i]){ok=false;}filas[i].style.display=ok?'':'none';}")
        sb.AppendLine("tb.style.display='';}")
        sb.AppendLine("function filtrarDespues(){if(espera){clearTimeout(espera);}espera=setTimeout(filtrar,250);}")
        sb.AppendLine("</script>")
        sb.AppendLine("</body></html>")

        Return sb.ToString
    End Function

    'Nombre para comparar repetidos: sin espacios de mas, en mayusculas y sin tildes
    '"Juan  Pérez " y "JUAN PEREZ" se consideran el mismo nombre
    Public Function _fnNormalizarNombre(valor As Object) As String
        If (IsDBNull(valor) Or IsNothing(valor)) Then
            Return ""
        End If
        Dim texto As String = valor.ToString.Trim.ToUpper
        texto = Regex.Replace(texto, "\s+", " ")
        Dim sinTildes As New System.Text.StringBuilder
        For Each c As Char In texto.Normalize(System.Text.NormalizationForm.FormD)
            If (Globalization.CharUnicodeInfo.GetUnicodeCategory(c) <> Globalization.UnicodeCategory.NonSpacingMark) Then
                sinTildes.Append(c)
            End If
        Next
        Return sinTildes.ToString.Normalize(System.Text.NormalizationForm.FormC)
    End Function

    'Convierte el valor a texto seguro para HTML (evita que un nombre con < o & rompa la pagina)
    Public Function _fnHtml(valor As Object) As String
        If (IsDBNull(valor) Or IsNothing(valor)) Then
            Return ""
        End If
        Return System.Net.WebUtility.HtmlEncode(valor.ToString)
    End Function

    'Logo de la empresa en base64 para incrustarlo en el HTML
    Public Function _fnLogoEmpresaBase64() As String
        Try
            Dim dtImage As DataTable = ObtenerImagenEmpresa()
            If (dtImage.Rows.Count > 0) Then
                Dim Name As String = dtImage.Rows(0).Item(0).ToString
                Dim ruta As String = gs_CarpetaRaiz + "\Imagenes\Imagenes Empresa\" + Name.TrimStart("\"c)
                If (File.Exists(ruta)) Then
                    Dim Bin As New MemoryStream
                    Dim im As New Bitmap(ruta)
                    im.Save(Bin, Imaging.ImageFormat.Png)
                    im.Dispose()
                    Dim base64 As String = Convert.ToBase64String(Bin.ToArray())
                    Bin.Dispose()
                    Return base64
                End If
            End If
        Catch ex As Exception
            'sin logo no pasa nada, el reporte se genera igual
        End Try
        Return ""
    End Function

    'El HTML tiene datos de clientes: se borran los reportes anteriores de la carpeta temporal
    Private Sub P_BorrarReportesClientesAnteriores()
        Try
            For Each archivo As String In Directory.GetFiles(Path.GetTempPath(), "ReporteClientes_*.html")
                Try
                    File.Delete(archivo)
                Catch ex As Exception
                    'puede estar abierto en el navegador, se ignora
                End Try
            Next
        Catch ex As Exception
        End Try
    End Sub

#End Region
End Class