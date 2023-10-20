Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips

Public Class Rep_Modificaciones
    Dim _ListPuntos As List(Of PointLatLng)
    Dim _OverlayAnterior As GMapOverlay
    Dim _OverlayNuevo As GMapOverlay
    Dim idPersonal As Integer = 0
    Public Sub IniciarTodod()
        tbVendedor.Focus()
        _prInicarMapa()
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date
        idPersonal = -1
        _prCargarProductos()
        idPersonal = 0
    End Sub
    Public Sub _prInicarMapa()

        '_ListPuntos = New List(Of PointLatLng)
        _OverlayAnterior = New GMapOverlay("points")
        _OverlayNuevo = New GMapOverlay("points")
        Gmc_ClienteActual.Overlays.Add(_OverlayNuevo)
        Gmc_ClienteAnterior.Overlays.Add(_OverlayAnterior)
        P_IniciarMap(Gmc_ClienteActual, 0, 0, _OverlayNuevo)
        P_IniciarMap(Gmc_ClienteAnterior, 0, 0, _OverlayAnterior)
    End Sub
    Private Sub P_IniciarMap(gmc As GMapControl, _latitud As Decimal, _longitud As Decimal, over As GMapOverlay)
        gmc.DragButton = MouseButtons.Left
        gmc.CanDragMap = True
        gmc.MapProvider = GMapProviders.GoogleMap
        If (_latitud <> 0 And _longitud <> 0) Then

            gmc.Position = New PointLatLng(_latitud, _longitud)
        Else

            over.Markers.Clear()
            '' gmc.Position = New PointLatLng(-17.7823605, -63.1822469)
        End If

        gmc.MinZoom = 0
        gmc.MaxZoom = 24
        gmc.Zoom = 15.5
        gmc.AutoScroll = True

        GMapProvider.Language = LanguageType.Spanish
    End Sub
    Public Sub New()
        InitializeComponent()

    End Sub
    Private Sub Tec_DespachoDetalle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarTodod()
    End Sub

    Public Function limpiar(dt As DataTable) As DataTable

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim strValorAnterior As String = If(dt.Rows(i).Item("ValorAnterior") Is DBNull.Value, "", dt.Rows(i).Item("ValorAnterior").ToString())
            Dim strValorNuevo As String = If(dt.Rows(i).Item("ValorNuevo") Is DBNull.Value, "", dt.Rows(i).Item("ValorNuevo").ToString())

            ' Verificar si los valores JSON son nulos o vacíos
            Dim jsonObjectsAnterior As JToken = If(String.IsNullOrEmpty(strValorAnterior), Nothing, JToken.Parse(strValorAnterior))
            Dim jsonObjectsNuevo As JToken = If(String.IsNullOrEmpty(strValorNuevo), Nothing, JToken.Parse(strValorNuevo))

            Dim objAnterior As JObject = If(jsonObjectsAnterior?.FirstOrDefault, New JObject())
            Dim objNuevo As JObject = If(jsonObjectsNuevo?.FirstOrDefault, New JObject())

            Dim resultado As New StringBuilder()

            ' Comparamos los valores de los campos en ambos objetos JSON
            For Each prop In objNuevo.Properties()
                Dim valorAnterior As JToken = objAnterior(prop.Name)
                Dim valorNuevo As JToken = prop.Value

                ' Si el valor anterior no existe o es diferente al nuevo
                If valorAnterior Is Nothing OrElse Not valorAnterior.Equals(valorNuevo) Then
                    ' Verificamos si el valor es un array
                    If valorNuevo.Type = JTokenType.Array Then
                        ' Si es un array, construimos una cadena con cada elemento del array
                        Dim valoresArray As New StringBuilder()
                        For Each item In valorNuevo
                            ' Asumiendo que cada elemento del array es un objeto con un solo campo
                            For Each subProp In item.Children(Of JProperty)()
                                valoresArray.AppendLine(subProp.Name & ": " & subProp.Value.ToString())
                            Next
                        Next
                        resultado.AppendLine(prop.Name & ": " & valoresArray.ToString())
                    Else
                        ' Si no es un array, lo añadimos directamente al StringBuilder
                        resultado.AppendLine(prop.Name & ": " & If(valorAnterior IsNot Nothing, valorAnterior.ToString() & " -> ", "") & valorNuevo.ToString())
                    End If
                End If
            Next
            dt.Rows(i).Item("diferencia") = resultado.ToString()
            Console.WriteLine(resultado.ToString())
        Next
        Return dt
    End Function

    Private Sub _prCargarProductos()
        Dim dt As New DataTable

        If (idPersonal = 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

        dt = L_prListarHistorico(idPersonal, cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))  ''1=Almacen


        'Id Fecha	Accion	ValorAnterior	ValorNuevo	idPersonal	NombrePersonal
        grHistorico.DataSource = limpiar(dt)
        grHistorico.RetrieveStructure()
        grHistorico.AlternatingColors = True
        With grHistorico.RootTable.Columns("Id")
            .Width = 100
            .Caption = "Id"
            .Visible = False


        End With
        With grHistorico.RootTable.Columns("idPersonal")
            .Width = 100
            .Caption = "Id"
            .Visible = False


        End With
        With grHistorico.RootTable.Columns("Fecha")
            .Width = 100
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grHistorico.RootTable.Columns("Accion")
            .Width = 100
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grHistorico.RootTable.Columns("NombrePersonal")
            .Width = 150
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grHistorico.RootTable.Columns("ValorAnterior")
            .Width = 300
            .Caption = "Valor Anterior"
            .Visible = False
            .MaxLines = 20
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With
        With grHistorico.RootTable.Columns("ValorNuevo")
            .Width = 300
            .Caption = "Valor Anterior"
            .Visible = False
            .MaxLines = 20
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With
        With grHistorico.RootTable.Columns("diferencia")
            .Width = 400
            .Caption = "Campos Modificados o Creados"
            .Visible = True
            .MaxLines = 20
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Near
            .WordWrap = True
        End With
        With grHistorico
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub

    Private Sub grHistorico_SelectionChanged(sender As Object, e As EventArgs) Handles grHistorico.SelectionChanged

        If (grHistorico.Row >= 0) Then

            Dim diferencia As String = grHistorico.GetValue("diferencia")

            If diferencia.Contains("Latitud") OrElse diferencia.Contains("Longitud") Then

                Dim jsonObjectsAnterior As JToken = If(String.IsNullOrEmpty(grHistorico.GetValue("ValorAnterior")), Nothing, JToken.Parse(grHistorico.GetValue("ValorAnterior")))
                Dim jsonObjectsNuevo As JToken = If(String.IsNullOrEmpty(grHistorico.GetValue("ValorNuevo")), Nothing, JToken.Parse(grHistorico.GetValue("ValorNuevo")))

                Dim objAnterior As JObject = If(jsonObjectsAnterior?.FirstOrDefault, New JObject())
                Dim objNuevo As JObject = If(jsonObjectsNuevo?.FirstOrDefault, New JObject())

                Dim latitudAnterior As Decimal = 0
                Dim longitudAnterior As Decimal = 0
                Dim latitudNuevo As Decimal = 0
                Dim longitudNuevo As Decimal = 0

                ' Verifica si los campos Latitud y Longitud están presentes y no son nulos en el objeto anterior
                If objAnterior.ContainsKey("Latitud") AndAlso objAnterior("Latitud") IsNot Nothing Then
                    latitudAnterior = Decimal.Parse(objAnterior("Latitud").ToString())
                End If
                If objAnterior.ContainsKey("Longitud") AndAlso objAnterior("Longitud") IsNot Nothing Then
                    longitudAnterior = Decimal.Parse(objAnterior("Longitud").ToString())
                End If

                ' Verifica si los campos Latitud y Longitud están presentes y no son nulos en el nuevo objeto
                If objNuevo.ContainsKey("Latitud") AndAlso objNuevo("Latitud") IsNot Nothing Then
                    latitudNuevo = Decimal.Parse(objNuevo("Latitud").ToString())
                End If
                If objNuevo.ContainsKey("Longitud") AndAlso objNuevo("Longitud") IsNot Nothing Then
                    longitudNuevo = Decimal.Parse(objNuevo("Longitud").ToString())
                End If

                Dim plgAnterio As PointLatLng = New PointLatLng(latitudAnterior, longitudAnterior)
                Dim plgNuevo As PointLatLng = New PointLatLng(latitudNuevo, longitudNuevo)

                _OverlayAnterior.Markers.Clear()
                _OverlayNuevo.Markers.Clear()

                P_AgregarPunto(plgAnterio, _OverlayAnterior, Gmc_ClienteAnterior)
                P_AgregarPunto(plgNuevo, _OverlayNuevo, Gmc_ClienteActual)

            End If



        End If
    End Sub

    Private Sub P_AgregarPunto(pointLatLng As PointLatLng, _Overlay As GMapOverlay, gmc As GMapControl)
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
            gmc.Position = pointLatLng
        End If
    End Sub

    Private Sub btnVendedor_Click(sender As Object, e As EventArgs) Handles btnVendedor.Click



        Dim dt As DataTable

            dt = ListarPersonal()
            'a.Id ,a.NombreProveedor ,a.Direccion ,a.Telefono01

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id", False, "ID", 50))
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

            idPersonal = Row.Cells("Id").Value
            tbVendedor.Text = Row.Cells("Nombre").Value


        End If
            ef.Dispose()

    End Sub

    Private Sub btnConfirmarSalir_Click(sender As Object, e As EventArgs) Handles btnConfirmarSalir.Click
        _prCargarProductos()
    End Sub
End Class