
Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid

Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips
Imports System.Drawing
Imports System.Threading
Public Class Tec_Mapaclientes

    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim _Punto As Integer
    Dim _ListPuntos As List(Of PointLatLng)
    Dim _Overlay As GMapOverlay
    Dim _latitud As Double = 0
    Dim _longitud As Double = 0
    Dim TableCliente As DataTable
    Dim Markers As GMapMarker
    Dim cont As Integer = 0



    Private Sub _IniciarTodo()
        L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        _prInicarMapa()
        _prObtenerDatatableClientes()
        _prCargarClientesJanus(TableCliente)

        P_Global._prCargarComboGenerico(cbZona, L_prListarZonas(), "Id", "Codigo", "NombreZona", "Zonas")



        Gmc_Cliente.Zoom = Gmc_Cliente.Zoom - 2
        cbZona.ReadOnly = True
        checkTodos.CheckValue = True
        cbZona.Enabled = False


        Me.Text = "Mapa Clientes"

    End Sub



    Public Sub _prCargarClientesJanus(Cliente As DataTable)

        Dim dt As New DataTable
        dt = Cliente
        ''''janosssssssss''''''
        grCliente.DataSource = dt
        grCliente.RetrieveStructure()

        'a.id , a.NombreCliente, a.Telefono, a.Latitud, a.Longitud 

        With grCliente.RootTable.Columns("id")
            .Width = 60
            .Caption = "CODIGO"

            .Visible = True

        End With

        With grCliente.RootTable.Columns("NombreCliente")
            .Width = 300
            .Caption = "CLIENTE"
            .Visible = True
        End With
        With grCliente.RootTable.Columns("Telefono")
            .Width = 90
            .Caption = "TELEFONO"

            .Visible = True
        End With
        With grCliente.RootTable.Columns("Latitud")
            .Width = 90
            .Visible = False
        End With
        With grCliente.RootTable.Columns("Longitud")
            .Width = 90
            .Visible = False
        End With


        With grCliente
            .GroupByBoxVisible = False

            .VisualStyle = VisualStyle.Office2007
            '.View = View.CardView

            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .RecordNavigator = True
            .RecordNavigatorText = "CLIENTES"
            .RowHeaders = InheritableBoolean.True
        End With



        'grCliente.CardViewGridlines = CardViewGridlines.Vertical


    End Sub
    Public Sub _prObtenerDatatableClientes()
        TableCliente = L_prMapaCLienteGeneral()
        _prDibujarMarketCliente(TableCliente.Rows.Count - 1, TableCliente)
    End Sub
    Public Sub _prDibujarMarketCliente(n As Integer, Cliente As DataTable)
        For i As Integer = 0 To n Step 1
            Dim lat As Double = Cliente.Rows(i).Item("Latitud")
            Dim longitud As Double = Cliente.Rows(i).Item("Longitud")
            If (lat <> 0 And longitud <> 0) Then
                Dim plg As PointLatLng = New PointLatLng(lat, longitud)
                _latitud = plg.Lat
                _longitud = plg.Lng
                P_AgregarPunto(plg, "", "", Cliente.Rows(i))
            End If
        Next
    End Sub
    Private Sub P_IniciarMap()
        Gmc_Cliente.DragButton = MouseButtons.Left
        Gmc_Cliente.CanDragMap = True
        Gmc_Cliente.MapProvider = GMapProviders.GoogleMap
        If (_latitud <> 0 And _longitud <> 0) Then

            Gmc_Cliente.Position = New PointLatLng(_latitud, _longitud)
        Else

            _Overlay.Markers.Clear()
            Gmc_Cliente.Position = New PointLatLng(-17.4120653, -66.1825898)
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
    Private Sub F1_MapaCLientes_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
        _IniciarTodo()

    End Sub



    Private Sub P_AgregarPunto(pointLatLng As PointLatLng, _nombre As String, _ci As String, data As System.Data.DataRow)

        If (Not IsNothing(_Overlay)) Then
            'añadir puntos
            'Dim markersOverlay As New GMapOverlay("markers")
            Dim marker As New GMarkerGoogle(pointLatLng, My.Resources.iconunselected)

            'añadir tooltip
            Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver
            marker.ToolTip = New GMapBaloonToolTip(marker)
            marker.ToolTipMode = mode
            Dim ToolTipBackColor As New SolidBrush(Color.Blue)
            marker.ToolTip.Fill = ToolTipBackColor
            marker.ToolTip.Foreground = Brushes.White
            marker.Tag = data


            _Overlay.Markers.Add(marker)
            'mapa.Overlays.Add(markersOverlay)


        End If



    End Sub

    Private Sub Gmc_Cliente_OnMarkerClick(item As GMapMarker, e As MouseEventArgs) Handles Gmc_Cliente.OnMarkerClick
        If (Not IsNothing(Markers)) Then
            _Overlay.Markers.Remove(Markers)

        End If


        Dim i As DataRow = CType(item.Tag, DataRow)
        Dim lat As String = i.Item("Latitud")
        Dim tags As Object = item.Tag
        Dim longit As String = i.Item("Longitud")



        'Dim markersOverlay As New GMapOverlay("markers")
        Dim marker As New GMarkerGoogle(New PointLatLng(lat, longit), My.Resources.iconselected)

        'añadir tooltip
        Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver
        marker.ToolTip = New GMapBaloonToolTip(marker)
        marker.ToolTipMode = mode
        Dim ToolTipBackColor As New SolidBrush(Color.Blue)
        marker.ToolTip.Fill = ToolTipBackColor
        marker.ToolTip.Foreground = Brushes.White
        marker.Tag = tags

        Markers = marker

        _Overlay.Markers.Add(marker)

        Gmc_Cliente.Position = New PointLatLng(lat, longit)

        Dim posicion As Integer = -1
        Dim k As Integer = 0
        Dim n As Integer = CType(grCliente.DataSource, DataTable).Rows.Count
        While (k < n)
            If (CType(grCliente.DataSource, DataTable).Rows(k).Item("id") = i.Item("Id")) Then
                posicion = k
            End If
            k += 1
        End While



        If (posicion >= 0) Then
            grCliente.Row = posicion
        End If











    End Sub




    Private Sub ButtonX3_Click(sender As Object, e As EventArgs) Handles btnz1.Click
        If (Gmc_Cliente.Zoom <= Gmc_Cliente.MaxZoom) Then
            Gmc_Cliente.Zoom = Gmc_Cliente.Zoom + 1
        End If
    End Sub

    Private Sub ButtonX4_Click(sender As Object, e As EventArgs) Handles btnz2.Click
        If (Gmc_Cliente.Zoom >= Gmc_Cliente.MinZoom) Then
            Gmc_Cliente.Zoom = Gmc_Cliente.Zoom - 1
        End If
    End Sub


    Private Sub grCliente_DoubleClick(sender As Object, e As EventArgs)
        Dim _MPos As Integer
        If grCliente.Row >= 0 Then
            _MPos = grCliente.Row



            Dim longit As String = grCliente.GetValue("Latitud")



            Dim gma As GMapMarker
            _prRecorreMArket(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud"), gma)
            If (Not IsNothing(gma)) Then

                If (Not IsNothing(Markers)) Then
                    _Overlay.Markers.Remove(Markers)

                End If
                Dim marker As New GMarkerGoogle(New PointLatLng(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud")), My.Resources.iconselected)

                'añadir tooltip
                Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver
                marker.ToolTip = New GMapBaloonToolTip(marker)
                marker.ToolTipMode = mode
                Dim ToolTipBackColor As New SolidBrush(Color.Blue)
                marker.ToolTip.Fill = ToolTipBackColor
                marker.ToolTip.Foreground = Brushes.White
                marker.Tag = gma.Tag

                Markers = marker

                _Overlay.Markers.Add(marker)
                Gmc_Cliente.Position = New PointLatLng(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud"))
            Else


                ToastNotification.Show(Me, "NO EXISTE DATOS PARA ESTE CLIENTE", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.MiddleCenter)
            End If

        End If
    End Sub
    Public Sub _prRecorreMArket(latitud As String, longitud As String, ByRef ma As GMapMarker)
        Dim a As List(Of GMapMarker) = _Overlay.Markers.ToList
        For k = 0 To a.Count - 1 Step 1
            Dim mark As GMapMarker = a(k)
            Dim i As DataRow = CType(mark.Tag, DataRow)
            Dim lat As String = i.Item("Latitud")
            Dim longit As String = i.Item("Longitud")
            If (lat.Equals(latitud) And longit.Equals(longitud)) Then
                ma = mark
                Return

            End If
        Next


        '    Dim length As Integer = a.Length


    End Sub
    Private Sub grCliente_KeyDown(sender As Object, e As KeyEventArgs)
        If (e.KeyData = Keys.Enter) Then
            Dim _MPos As Integer
            If grCliente.Row >= 0 Then
                _MPos = grCliente.Row



                Dim longit As String = grCliente.GetValue("Longitud")

                Dim gma As GMapMarker
                _prRecorreMArket(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud"), gma)
                If (Not IsNothing(gma)) Then

                    Dim marker As New GMarkerGoogle(New PointLatLng(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud")), My.Resources.iconselected)

                    'añadir tooltip
                    Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver
                    marker.ToolTip = New GMapBaloonToolTip(marker)
                    marker.ToolTipMode = mode
                    Dim ToolTipBackColor As New SolidBrush(Color.Blue)
                    marker.ToolTip.Fill = ToolTipBackColor
                    marker.ToolTip.Foreground = Brushes.White
                    marker.Tag = gma.Tag

                    Markers = marker

                    _Overlay.Markers.Add(marker)
                    Gmc_Cliente.Position = New PointLatLng(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud"))
                Else


                    ToastNotification.Show(Me, "NO EXISTE DATOS PARA ESTE CLIENTE", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.MiddleCenter)
                End If

            End If
        End If
    End Sub

    Private Sub checkTodos_CheckValueChanged(sender As Object, e As EventArgs) Handles checkTodos.CheckValueChanged
        If (checkTodos.Checked = False) Then
            cbZona.Enabled = True
            cbZona.ReadOnly = False
            If (CType(cbZona.DataSource, DataTable).Rows.Count > 0) Then
                cbZona.SelectedIndex = 0
                Dim dt As DataTable = L_prMapaCLienteGeneralPorZona(cbZona.Value)
                _prCargarClientesJanus(dt)
                _Overlay.Markers.Clear()
                _prDibujarMarketCliente(dt.Rows.Count - 1, dt)
            End If
        End If

        If (checkTodos.Checked = True) Then
            cbZona.Enabled = False
            cbZona.ReadOnly = True

            Dim dt As DataTable = L_prMapaCLienteGeneral()
            _prCargarClientesJanus(dt)
            _Overlay.Markers.Clear()
            _prDibujarMarketCliente(dt.Rows.Count - 1, dt)
        End If
    End Sub

    Private Sub cbZona_ValueChanged(sender As Object, e As EventArgs) Handles cbZona.ValueChanged
        If (cbZona.SelectedIndex >= 0) Then
            Dim dt As DataTable = L_prMapaCLienteGeneralPorZona(cbZona.Value)
            _prCargarClientesJanus(dt)
            _Overlay.Markers.Clear()
            _prDibujarMarketCliente(dt.Rows.Count - 1, dt)
        End If

    End Sub

    Private Sub grCliente_DoubleClick_1(sender As Object, e As EventArgs) Handles grCliente.DoubleClick
        Dim _MPos As Integer
        If grCliente.Row >= 0 Then
            _MPos = grCliente.Row



            Dim longit As String = grCliente.GetValue("Latitud")



            Dim gma As GMapMarker
            _prRecorreMArket(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud"), gma)
            If (Not IsNothing(gma)) Then

                If (Not IsNothing(Markers)) Then
                    _Overlay.Markers.Remove(Markers)

                End If
                Dim marker As New GMarkerGoogle(New PointLatLng(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud")), My.Resources.iconselected)

                'añadir tooltip
                Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver
                marker.ToolTip = New GMapBaloonToolTip(marker)
                marker.ToolTipMode = mode
                Dim ToolTipBackColor As New SolidBrush(Color.Blue)
                marker.ToolTip.Fill = ToolTipBackColor
                marker.ToolTip.Foreground = Brushes.White
                marker.Tag = gma.Tag

                Markers = marker

                _Overlay.Markers.Add(marker)
                Gmc_Cliente.Position = New PointLatLng(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud"))
            Else


                ToastNotification.Show(Me, "NO EXISTE DATOS PARA ESTE CLIENTE", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.MiddleCenter)
            End If

        End If
    End Sub

    Private Sub grCliente_KeyDown_1(sender As Object, e As KeyEventArgs) Handles grCliente.KeyDown
        If (e.KeyData = Keys.Enter) Then
            Dim _MPos As Integer
            If grCliente.Row >= 0 Then
                _MPos = grCliente.Row



                Dim longit As String = grCliente.GetValue("Longitud")

                Dim gma As GMapMarker
                _prRecorreMArket(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud"), gma)
                If (Not IsNothing(gma)) Then

                    Dim marker As New GMarkerGoogle(New PointLatLng(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud")), My.Resources.iconselected)

                    'añadir tooltip
                    Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver
                    marker.ToolTip = New GMapBaloonToolTip(marker)
                    marker.ToolTipMode = mode
                    Dim ToolTipBackColor As New SolidBrush(Color.Blue)
                    marker.ToolTip.Fill = ToolTipBackColor
                    marker.ToolTip.Foreground = Brushes.White
                    marker.Tag = gma.Tag

                    Markers = marker

                    _Overlay.Markers.Add(marker)
                    Gmc_Cliente.Position = New PointLatLng(grCliente.GetValue("Latitud"), grCliente.GetValue("Longitud"))
                Else


                    ToastNotification.Show(Me, "NO EXISTE DATOS PARA ESTE CLIENTE", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.MiddleCenter)
                End If

            End If
        End If
    End Sub
End Class