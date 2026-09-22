Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX

Public Class Rep_AuditoriaVentas

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Rep_AuditoriaVentas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    ''La tabla maestra (izquierda) reparte el ancho de forma proporcional con la
    ''de detalle (47% / 53%) en vez de un valor fijo en pixeles - asi se ve bien
    ''tanto maximizado como en una pantalla mas chica.
    Private Sub Rep_AuditoriaVentas_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        _prAjustarAnchoPaneles()
    End Sub

    Private Sub _prAjustarAnchoPaneles()
        If (PanelDatos.ClientSize.Width > 300) Then
            PanelMaestro.Width = CInt(PanelDatos.ClientSize.Width * 0.47)
        End If
    End Sub

    Private Sub _prIniciarTodo()
        _prAjustarAnchoPaneles()
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date

        Dim dtTipos As New DataTable
        dtTipos.Columns.Add("Codigo")
        dtTipos.Columns.Add("Descripcion")
        dtTipos.Rows.Add("TODOS", "TODOS")
        dtTipos.Rows.Add("ANULADO", "ANULADOS")
        dtTipos.Rows.Add("ELIMINADO", "ELIMINADOS")
        dtTipos.Rows.Add("MODIFICADO", "MODIFICADOS")

        P_Global._prCargarComboGenerico(cbTipoEvento, dtTipos, "Codigo", "Codigo", "Descripcion", "Descripcion")

        _prCargarEventos()
    End Sub

    ''Ayuda para no romper el formulario si algun dia una columna cambia o no llega
    ''(por ejemplo si todavia no actualizaste el SP en la base): configura la
    ''columna solo si existe en el DataTable devuelto.
    Private Sub _prConfigColumna(gr As GridEX, nombre As String, ancho As Integer, caption As String, visible As Boolean,
                                  Optional formato As String = "", Optional alinear As Boolean = True, Optional wordWrap As Boolean = False)
        If (Not gr.RootTable.Columns.Contains(nombre)) Then
            Return
        End If
        With gr.RootTable.Columns(nombre)
            .Width = ancho
            .Caption = caption
            .Visible = visible
            If (alinear) Then
                .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
                .TextAlignment = TextAlignment.Center
            End If
            If (formato <> "") Then
                .FormatString = formato
            End If
            If (wordWrap) Then
                .WordWrap = True
                .MaxLines = 2
            End If
        End With
    End Sub

    Private Sub _prCargarEventos()
        Dim dt As New DataTable

        Dim _tipoEvento As String = "TODOS"
        If (cbTipoEvento.Value IsNot Nothing) Then
            _tipoEvento = cbTipoEvento.Value.ToString()
        End If

        dt = L_prListarAuditoriaVentas(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), _tipoEvento)

        grMaestro.DataSource = dt
        grMaestro.RetrieveStructure()
        grMaestro.AlternatingColors = True
        grMaestro.RootTable.FormatConditions.Clear()

        _prConfigColumna(grMaestro, "Id", 70, "COD", False)
        _prConfigColumna(grMaestro, "VentaId", 80, "VENTA", True)
        _prConfigColumna(grMaestro, "TipoEvento", 120, "ACCION", True, "", True, True)
        _prConfigColumna(grMaestro, "Origen", 100, "ORIGEN", True, "", True, True)
        _prConfigColumna(grMaestro, "FechaHora", 150, "FECHA Y HORA", True, "dd/MM/yyyy HH:mm:ss", True, True)
        _prConfigColumna(grMaestro, "Usuario", 130, "USUARIO", True, "", False, True)
        _prConfigColumna(grMaestro, "Personal", 190, "VENDEDOR", True, "", False, True)
        _prConfigColumna(grMaestro, "Cliente", 260, "CLIENTE", True, "", False, True)
        _prConfigColumna(grMaestro, "TotalVenta", 110, "TOTAL VENTA", True, "0.00")
        _prConfigColumna(grMaestro, "Glosa", 150, "GLOSA", False)
        _prConfigColumna(grMaestro, "ClienteId", 70, "", False)
        _prConfigColumna(grMaestro, "EstadoAnterior", 90, "ESTADO ANTES", False)
        _prConfigColumna(grMaestro, "EstadoNuevo", 90, "ESTADO DESPUES", False)
        _prConfigColumna(grMaestro, "AnuladoAnterior", 90, "", False)
        _prConfigColumna(grMaestro, "AnuladoNuevo", 90, "", False)
        ''Observacion se quita de la vista (pedido explicito) - se deja el dato en el
        ''DataTable por si algun dia se quiere volver a mostrar, solo Visible=False
        _prConfigColumna(grMaestro, "Observacion", 250, "OBSERVACION", False, "", False, True)

        ''Resaltar la accion (mismo criterio de color en toda la fila: rojo=Eliminado,
        ''naranja=Anulado, azul=Modificado) - solo estetico, con condiciones nativas de
        ''Janus (GridEXFormatCondition), sin ninguna libreria externa
        If (grMaestro.RootTable.Columns.Contains("TipoEvento")) Then
            Dim fcEliminado As New GridEXFormatCondition(grMaestro.RootTable.Columns("TipoEvento"), ConditionOperator.Equal, "ELIMINADO")
            fcEliminado.FormatStyle.ForeColor = Color.FromArgb(178, 34, 34)
            fcEliminado.FormatStyle.FontBold = TriState.True
            grMaestro.RootTable.FormatConditions.Add(fcEliminado)

            Dim fcAnulado As New GridEXFormatCondition(grMaestro.RootTable.Columns("TipoEvento"), ConditionOperator.Equal, "ANULADO")
            fcAnulado.FormatStyle.ForeColor = Color.FromArgb(184, 108, 0)
            fcAnulado.FormatStyle.FontBold = TriState.True
            grMaestro.RootTable.FormatConditions.Add(fcAnulado)

            Dim fcModificado As New GridEXFormatCondition(grMaestro.RootTable.Columns("TipoEvento"), ConditionOperator.Equal, "MODIFICADO")
            fcModificado.FormatStyle.ForeColor = Color.FromArgb(30, 90, 160)
            fcModificado.FormatStyle.FontBold = TriState.True
            grMaestro.RootTable.FormatConditions.Add(fcModificado)
        End If

        ''Distinguir de un vistazo si la accion vino de la PC (Desktop) o del celular (App Movil)
        If (grMaestro.RootTable.Columns.Contains("Origen")) Then
            Dim fcMovil As New GridEXFormatCondition(grMaestro.RootTable.Columns("Origen"), ConditionOperator.Equal, "MOVIL")
            fcMovil.FormatStyle.ForeColor = Color.FromArgb(96, 60, 160)
            fcMovil.FormatStyle.FontBold = TriState.True
            grMaestro.RootTable.FormatConditions.Add(fcMovil)
        End If

        With grMaestro
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
            .ColumnAutoResize = True
            .GridLines = Janus.Windows.GridEX.GridLines.None
            .BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        End With

        lblMaestro.Text = "EVENTOS DEL DIA  (" & dt.Rows.Count.ToString() & ")"

        ''limpiar el detalle cada vez que se recarga la maestra
        grDetalle.DataSource = Nothing
        lblDetalle.Text = "DETALLE DEL EVENTO SELECCIONADO"

        If (dt.Rows.Count = 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub _prCargarDetalleEvento(EventoId As String)
        Dim dt As New DataTable
        dt = L_prListarAuditoriaVentasDetalle(EventoId)

        grDetalle.DataSource = dt
        grDetalle.RetrieveStructure()
        grDetalle.AlternatingColors = True

        _prConfigColumna(grDetalle, "Id", 70, "", False)
        _prConfigColumna(grDetalle, "EventoId", 70, "", False)
        _prConfigColumna(grDetalle, "VentaId", 70, "", False)
        _prConfigColumna(grDetalle, "ProductoId", 90, "COD PRODUCTO", True)
        _prConfigColumna(grDetalle, "Producto", 260, "PRODUCTO", True, "", False, True)
        _prConfigColumna(grDetalle, "TipoCambio", 150, "CAMBIO", True, "", True, True)
        _prConfigColumna(grDetalle, "CantidadAntes", 90, "CANT. ANTES", True, "0.00")
        _prConfigColumna(grDetalle, "CantidadDespues", 95, "CANT. DESPUES", True, "0.00")
        _prConfigColumna(grDetalle, "PrecioAntes", 90, "PRECIO ANTES", True, "0.00")
        _prConfigColumna(grDetalle, "PrecioDespues", 95, "PRECIO DESPUES", True, "0.00")

        If (grDetalle.RootTable.Columns.Contains("TipoCambio")) Then
            Dim fcAgregado As New GridEXFormatCondition(grDetalle.RootTable.Columns("TipoCambio"), ConditionOperator.Equal, "AGREGADO")
            fcAgregado.FormatStyle.ForeColor = Color.FromArgb(30, 130, 76)
            fcAgregado.FormatStyle.FontBold = TriState.True
            grDetalle.RootTable.FormatConditions.Add(fcAgregado)

            Dim fcEliminado As New GridEXFormatCondition(grDetalle.RootTable.Columns("TipoCambio"), ConditionOperator.Equal, "ELIMINADO")
            fcEliminado.FormatStyle.ForeColor = Color.FromArgb(178, 34, 34)
            fcEliminado.FormatStyle.FontBold = TriState.True
            grDetalle.RootTable.FormatConditions.Add(fcEliminado)

            Dim fcCantMod As New GridEXFormatCondition(grDetalle.RootTable.Columns("TipoCambio"), ConditionOperator.Equal, "CANTIDAD_MODIFICADA")
            fcCantMod.FormatStyle.ForeColor = Color.FromArgb(184, 108, 0)
            fcCantMod.FormatStyle.FontBold = TriState.True
            grDetalle.RootTable.FormatConditions.Add(fcCantMod)
        End If

        With grDetalle
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .ColumnAutoResize = True
            .GridLines = Janus.Windows.GridEX.GridLines.None
            .BorderStyle = Janus.Windows.GridEX.BorderStyle.None
            If (dt.Columns.Contains("CantidadAntes")) Then
                .TotalRow = InheritableBoolean.True
                .TotalRowPosition = TotalRowPosition.BottomFixed
                .TotalRowFormatStyle.BackColor = Color.Gold
                .TotalRowFormatStyle.ForeColor = Color.Black
                .TotalRowFormatStyle.FontBold = TriState.True
                .RootTable.Columns("CantidadAntes").AggregateFunction = AggregateFunction.Sum
                .RootTable.Columns("CantidadDespues").AggregateFunction = AggregateFunction.Sum
            End If
        End With
    End Sub

    Private Sub grMaestro_SelectionChanged(sender As Object, e As EventArgs) Handles grMaestro.SelectionChanged
        If (grMaestro.Row >= 0) Then
            Dim _venta As String = grMaestro.GetValue("VentaId").ToString()
            Dim _accion As String = grMaestro.GetValue("TipoEvento").ToString()
            lblDetalle.Text = "DETALLE - VENTA " & _venta & " (" & _accion & ")"
            _prCargarDetalleEvento(grMaestro.GetValue("Id"))
        End If
    End Sub

    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        _prCargarEventos()
    End Sub

    ''Apenas cambian el tipo de accion (Anulado/Eliminado/Modificado/Todos) se recarga solo,
    ''igual que cbFiltroEstado_ValueChanged en Tec_Ventas.vb
    Private Sub cbTipoEvento_ValueChanged(sender As Object, e As EventArgs) Handles cbTipoEvento.ValueChanged
        _prCargarEventos()
    End Sub

End Class
