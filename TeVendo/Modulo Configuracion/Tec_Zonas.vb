Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips
Imports DevComponents.DotNetBar.Controls

Public Class Tec_Zonas

#Region "Variables Locales"
#Region "MApas"
    Dim _Punto As Integer
    Dim _ListPuntos As List(Of PointLatLng) = New List(Of PointLatLng)

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Dim _Overlay As GMapOverlay
    Dim _latitud As Double = 0
    Dim _longitud As Double = 0
    Public FilaSeleccionada As Boolean = False
#End Region

    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Dim _TablePoint As DataTable
    Dim _color As String = ""

    Dim _AddPoint As Boolean = False

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

        _Overlay.Polygons.Clear()

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
#Region "Metodos Privados"
    Private Sub _prIniciarTodo()
        Me.Text = "Gestionar Zonas"
        _prInicarMapa()
        '' L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        _prMaxLength()
        _prAsignarPermisos()
        _PMIniciarTodo()
        _prCargarZonasTable()
        tbNombreZona.Focus()
        Dim blah As New Bitmap(New Bitmap(My.Resources.zona), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
        _habilitarFocus()
    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbColor, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNombreZona, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDescripcionZona, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub
    Public Sub _prCargarZonasTable()
        grZonas.BoundMode = Janus.Data.BoundMode.Bound
        grZonas.DataSource = JGrM_Buscador.DataSource
        grZonas.RetrieveStructure()
        'id, NombreZona, DescripcionZona, Color 
        With grZonas.RootTable.Columns("id")
            .Caption = "Id"
            .Width = 40
            .Visible = True
            .FormatString = "0"
        End With
        With grZonas.RootTable.Columns("NombreZona")
            .Caption = "Nombre Zona"
            .Width = 150
            .Visible = True
            .FormatString = ""
        End With
        With grZonas.RootTable.Columns("DescripcionZona")
            .Caption = "Descripcion"
            .Width = 150
            .Visible = True

        End With
        With grZonas.RootTable.Columns("Color")
            .Width = 150
            .Visible = False
            .FormatString = "0"
        End With


        With grZonas
            .GroupByBoxVisible = False
            '.FilterRowFormatStyle.BackColor = Color.Blue
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'Diseño de la tabla
            .VisualStyle = VisualStyle.Office2007
            .SelectionMode = SelectionMode.SingleSelection
            .AlternatingColors = True
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

    Public Sub _prMaxLength()
        tbNombreZona.MaxLength = 200
        tbDescripcionZona.MaxLength = 400

    End Sub

    Private Sub P_IniciarMap()
        Gmc_Cliente.DragButton = MouseButtons.Left
        Gmc_Cliente.CanDragMap = True
        Gmc_Cliente.MapProvider = GMapProviders.GoogleMap
        If (_latitud <> 0 And _longitud <> 0) Then

            Gmc_Cliente.Position = New PointLatLng(_latitud, _longitud)
        Else

            _Overlay.Markers.Clear()
            Gmc_Cliente.Position = New PointLatLng(-19.566845, -65.7667348)
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


#End Region

#Region "METODOS SOBRECARGADOS"

    Public Sub _PMOHabilitar()

        tbNombreZona.ReadOnly = False
        tbDescripcionZona.ReadOnly = False
        BtAdicionar.Visible = True
        btLimpiar.Visible = True
        btnLimpiarUltimo.Visible = True
        grZonas.Enabled = False

    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreZona.ReadOnly = True
        tbDescripcionZona.ReadOnly = True
        tbColor.ReadOnly = True
        BtAdicionar.Visible = False
        btLimpiar.Visible = False
        btnLimpiarUltimo.Visible = False
        _prCargarZonasTable()
        grZonas.Enabled = True
        If (grZonas.RowCount > 0) Then
            grZonas.Row = 0
        End If
        grZonas.Focus()
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Clear()
        tbNombreZona.Text = ""
        tbColor.Text = ""
        tbDescripcionZona.Text = ""

        _Overlay.Markers.Clear()
        _latitud = 0
        _longitud = 0
        _TablePoint = L_fnListarPuntosPorZona(-1)
        _Overlay.Polygons.Clear()
        _AddPoint = False
        tbColor.Clear()
        _ListPuntos.Clear()

        _prCargarZonasTable()
        tbNombreZona.Focus()
    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbNombreZona.BackColor = Color.White
    End Sub

    Public Function _PMOGrabarRegistro() As Boolean


        Dim res As Boolean = InsertarZona(tbCodigo.Text, tbNombreZona.Text, tbDescripcionZona.Text, tbColor.Text, _TablePoint)


        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Zona ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )


            ''cbProvincia.Focus()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Zona no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean

        Dim res As Boolean = ActualizarZona(tbCodigo.Text, tbNombreZona.Text, tbDescripcionZona.Text, tbColor.Text, _TablePoint)
        If res Then
            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Zona ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter)

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Zona no pudo ser modificado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
        _PMFiltrar()
        _prCargarZonasTable()
        _PMInhabilitar()
        _PMPrimerRegistro()
        Return res

    End Function

    Public Sub _prSalir()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()

        Else
            _TabControl.SelectedTab = _modulo
            _tab.Close()
            Me.Close()
        End If
    End Sub

    Public Function _fnAccesible() As Boolean
        Return tbNombreZona.ReadOnly = False
    End Function

    Public Sub _PMOEliminarRegistro()
        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar la Zona " + tbNombreZona.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = EliminarZona(tbCodigo.Text, mensajeError)
                If res Then

                    ToastNotification.Show(Me, "Codigo de Zona ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else

                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar la Zona".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If




    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()

        If tbNombreZona.Text.Length <= 0 Then
            tbNombreZona.BackColor = Color.Red
            MEP.SetError(tbNombreZona, "Ingrese un Nombre de Zona!".ToUpper)
            _ok = False
        Else
            tbNombreZona.BackColor = Color.White
            MEP.SetError(tbNombreZona, "")
        End If



        MHighlighterFocus.UpdateHighlights()
        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable
        Dim dtBuscador As DataTable = GeneralZona()
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)
        Dim listEstCeldas As New List(Of Celda)
        'id, NombreZona, DescripcionZona, Color 
        listEstCeldas.Add(New Celda("Id", True, "Id".ToUpper, 40))
        listEstCeldas.Add(New Celda("Color", False))
        listEstCeldas.Add(New Celda("NombreZona", True, "NombreZona".ToUpper, 250))
        listEstCeldas.Add(New Celda("DescripcionZona", True, "Descripcion".ToUpper, 350))


        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        'id, NombreZona, DescripcionZona, Color 
        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Try
            tbCodigo.Text = JGrM_Buscador.GetValue("Id").ToString
        Catch ex As Exception
            Exit Sub
        End Try
        With JGrM_Buscador
            tbNombreZona.Text = .GetValue("NombreZona").ToString
            tbColor.Text = .GetValue("Color").ToString
            tbDescripcionZona.Text = .GetValue("DescripcionZona").ToString


        End With
        _TablePoint = L_fnListarPuntosPorZona(tbCodigo.Text)
        _prdibujarPoligonoRegistrado(tbColor.Text)
        '_dibujarUbicacion(JGrM_Buscador.GetValue("yddesc").ToString, JGrM_Buscador.GetValue("yddctnum").ToString)
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub

    Public Sub _prdibujarPoligonoRegistrado(_color As String)
        Try

            Dim lati, longi As Double
            _ListPuntos.Clear()
            For i = 0 To _TablePoint.Rows.Count - 1
                lati = _TablePoint.Rows(i).Item("zblat")
                longi = _TablePoint.Rows(i).Item("zblongi")
                Dim plg As PointLatLng = New PointLatLng(lati, longi)
                _ListPuntos.Add(plg)


            Next
            If _TablePoint.Rows.Count > 0 Then
                'posicionar en la zona

                Dim latiZona, longiZona As Double
                latiZona = _TablePoint.Rows(0).Item("zblat")
                longiZona = _TablePoint.Rows(0).Item("zblongi")
                Gmc_Cliente.Position = New PointLatLng(latiZona, longiZona)

                ColorDialog1.Color = ColorTranslator.FromHtml(tbColor.Text)

                tbColor.BackColor = ColorDialog1.Color
                tbColor.Refresh()
                Dim polygon As New GMapPolygon(_ListPuntos, "mypolygon")
                'agregar color
                polygon.Fill = New SolidBrush(Color.FromArgb(50, ColorDialog1.Color))
                polygon.Stroke = New Pen(Color.FromArgb(211, 28, 112), 1.5)
                _Overlay.Polygons.Clear()
                _Overlay.Polygons.Add(polygon)
            Else
                _Overlay.Polygons.Clear()
            End If
        Catch ex As Exception

        End Try


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
    Public Sub _PMOHabilitarFocus()

        With MHighlighterFocus
            .SetHighlightOnFocus(tbNombreZona, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)


        End With
    End Sub

#End Region


    Private Sub Gmc_Cliente_MouseClick(sender As Object, e As MouseEventArgs) Handles Gmc_Cliente.MouseClick
        If _AddPoint = True Then
            Dim gm As GMapControl = CType(sender, GMapControl)
            Dim hj As MouseEventArgs = CType(e, MouseEventArgs)
            Dim plg As PointLatLng = gm.FromLocalToLatLng(hj.X, hj.Y)

            _AgregarPunto(plg)
            _ListPuntos.Add(plg)
            _TablePoint.Rows.Add(1, 1, plg.Lat, plg.Lng, 0)
        End If

    End Sub
    Private Sub _AgregarPunto(pointLatLng As PointLatLng, Optional etiqueta As String = "")
        Dim marker As GMarkerGoogle
        ''Dim markersOverlay As New GMapOverlay("markers")
        If (IsNothing(_ListPuntos)) Then

            marker = New GMarkerGoogle(pointLatLng, My.Resources.puntofinal)
        Else
            If (_ListPuntos.Count < 2 And _ListPuntos.Count >= 0) Then
                marker = New GMarkerGoogle(pointLatLng, My.Resources.puntofinal)
            End If
            If (_ListPuntos.Count >= 2) Then
                Dim markerA As New GMarkerGoogle(_ListPuntos(_ListPuntos.Count - 1), My.Resources.puntoinicial)
                Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver

                markerA.ToolTip = New GMapBaloonToolTip(markerA)

                _Overlay.Markers.Add(markerA)
            End If
            marker = New GMarkerGoogle(pointLatLng, My.Resources.puntofinal)
        End If



        'añadir tooltip
        If etiqueta <> "" Then
            Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver

            marker.ToolTip = New GMapBaloonToolTip(marker)

            marker.ToolTipMode = mode
            Dim ToolTipBackColor As New SolidBrush(Color.Red)
            marker.ToolTip.Fill = ToolTipBackColor
            marker.ToolTipText = etiqueta
        End If
        _Overlay.Markers.Add(marker)
        'mapa.Overlays.Add(markersOverlay)
    End Sub

    Public Function _fnEsNuevo()
        Return tbCodigo.Text = String.Empty And tbNombreZona.ReadOnly = False
    End Function
    Public Function _fnModificar()
        Return tbCodigo.Text <> String.Empty And tbNombreZona.ReadOnly = False
    End Function
    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles btLimpiar.Click
        If (_fnEsNuevo()) Then
            _ListPuntos.Clear()
            _Overlay.Markers.Clear()
            _TablePoint.Clear()
        Else
            If (_fnModificar()) Then
                _ListPuntos.Clear()
                _Overlay.Markers.Clear()
                _prCambiarEstadoPuntos(1, -1)
                _prCambiarEstadoPuntos(0, -2)
            End If
        End If

    End Sub
    Public Sub _prCambiarEstadoPuntos(estado As String, valor As Integer)
        Dim result As DataRow() = _TablePoint.Select("estado=" + estado)
        For i As Integer = 0 To result.Length - 1 Step 1
            Dim rowIndex As Integer = _TablePoint.Rows.IndexOf(result(i))
            _TablePoint.Rows(rowIndex).Item("estado") = valor
        Next
    End Sub

    Private Sub BtAdicionar_Click(sender As Object, e As EventArgs) Handles BtAdicionar.Click
        If (_fnEsNuevo()) Then
            If (_AddPoint = False) Then
                _AddPoint = True
                _ListPuntos.Clear()
                _Overlay.Markers.Clear()
                BtAdicionar.Text = "Dibujar Marcado"
            Else
                _AddPoint = False
                BtAdicionar.Text = "Ingresar Puntos"
                _DibujarPoligono()
            End If
        Else
            If (_AddPoint = False) Then
                _AddPoint = True
                _ListPuntos.Clear()
                _Overlay.Markers.Clear()
                _prCambiarEstadoPuntos(1, -1)
                _prCambiarEstadoPuntos(0, -2)
                BtAdicionar.Text = "Dibujar Marcado"
            Else
                _AddPoint = False
                BtAdicionar.Text = "Ingresar Puntos"
                _DibujarPoligono()
            End If
        End If

    End Sub
    Public Sub _DibujarPoligono()
        Dim polygon As New GMapPolygon(_ListPuntos, "mypolygon")
        'agregar color
        If ColorDialog1.ShowDialog() = DialogResult.OK Then
            polygon.Fill = New SolidBrush(Color.FromArgb(50, ColorDialog1.Color))
            Dim hexa As String = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialog1.Color.R, ColorDialog1.Color.G, ColorDialog1.Color.B)
            tbColor.Text = hexa
            tbColor.BackColor = ColorDialog1.Color
        Else
            polygon.Fill = New SolidBrush(Color.FromArgb(50, Color.Blue))
            Dim hexa As String = String.Format("#{0:X2}{1:X2}{2:X2}", ColorDialog1.Color.R, ColorDialog1.Color.G, ColorDialog1.Color.B)
            tbColor.Text = hexa
            tbColor.BackColor = ColorDialog1.Color
        End If

        'polygon.Fill = New SolidBrush(Color.FromArgb(50, Color.Black))
        polygon.Stroke = New Pen(Color.FromArgb(211, 28, 112), 1.5)
        _Overlay.Polygons.Clear()
        _Overlay.Polygons.Add(polygon)
        'añadir tooltip a poligono

        ''mapa.Overlays.Add(polyOverlay)

        _Overlay.Markers.Clear()

    End Sub

    Private Sub ButtonX3_Click(sender As Object, e As EventArgs) Handles ButtonX3.Click
        If (Gmc_Cliente.Zoom <= Gmc_Cliente.MaxZoom) Then
            Gmc_Cliente.Zoom = Gmc_Cliente.Zoom + 1
        End If
    End Sub

    Private Sub ButtonX4_Click(sender As Object, e As EventArgs) Handles ButtonX4.Click
        If (Gmc_Cliente.Zoom >= Gmc_Cliente.MinZoom) Then
            Gmc_Cliente.Zoom = Gmc_Cliente.Zoom - 1
        End If
    End Sub

    Private Sub grZonas_TabIndexChanged(sender As Object, e As EventArgs)
        Dim i As Integer = 0
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

    Private Sub grZonas_SelectionChanged_1(sender As Object, e As EventArgs) Handles grZonas.SelectionChanged
        Try
            If (Not IsDBNull(grZonas)) Then
                If (grZonas.Row >= 0) Then
                    _MPos = grZonas.Row
                    JGrM_Buscador.Row = _MPos
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnLimpiarUltimo_Click(sender As Object, e As EventArgs) Handles btnLimpiarUltimo.Click

        If (_fnEsNuevo() Or _fnModificar()) Then

            Dim result As DataRow() = _TablePoint.Select("estado>=0")
            If (result.Length > 0) Then  '' Pregunto si hay activos para eliminar

                _Overlay.Markers.Clear()
                _TablePoint.Rows.RemoveAt(ObtenerUltimaPosicionValida())

                _ListPuntos.RemoveAt(_ListPuntos.Count - 1)

                result = _TablePoint.Select("estado>=0")
                Dim lati, longi As Double
                For i As Integer = 0 To result.Length - 1 Step 1
                    Dim rowIndex As Integer = _TablePoint.Rows.IndexOf(result(i))
                    lati = _TablePoint.Rows(rowIndex).Item("zblat")
                    longi = _TablePoint.Rows(rowIndex).Item("zblongi")
                    Dim plg As PointLatLng = New PointLatLng(lati, longi)

                    _AgregarPunto(plg)
                Next



            End If


        End If

    End Sub

    Public Function ObtenerUltimaPosicionValida() As Integer


        Dim result As DataRow() = _TablePoint.Select("estado>=0")

        Dim rowIndex As Integer = _TablePoint.Rows.IndexOf(result(result.Length - 1))
        Return rowIndex

    End Function
End Class