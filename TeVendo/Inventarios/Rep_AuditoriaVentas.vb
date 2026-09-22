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

    Private Sub _prIniciarTodo()
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

        With grMaestro.RootTable.Columns("Id")
            .Width = 70
            .Caption = "COD".ToUpper
            .Visible = False
        End With

        With grMaestro.RootTable.Columns("VentaId")
            .Width = 80
            .Caption = "Venta".ToUpper
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grMaestro.RootTable.Columns("TipoEvento")
            .Width = 100
            .Caption = "Accion".ToUpper
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grMaestro.RootTable.Columns("Origen")
            .Width = 90
            .Caption = "Origen".ToUpper
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grMaestro.RootTable.Columns("FechaHora")
            .Width = 140
            .Caption = "Fecha y Hora".ToUpper
            .Visible = True
            .FormatString = "dd/MM/yyyy HH:mm:ss"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grMaestro.RootTable.Columns("Usuario")
            .Width = 110
            .Caption = "Usuario".ToUpper
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grMaestro.RootTable.Columns("Cliente")
            .Width = 200
            .Caption = "Cliente".ToUpper
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Near
        End With

        With grMaestro.RootTable.Columns("TotalVenta")
            .Width = 100
            .Caption = "Total Venta".ToUpper
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grMaestro.RootTable.Columns("Glosa")
            .Width = 150
            .Caption = "Glosa".ToUpper
            .Visible = False
        End With

        With grMaestro.RootTable.Columns("ClienteId")
            .Width = 70
            .Visible = False
        End With

        With grMaestro.RootTable.Columns("EstadoAnterior")
            .Width = 90
            .Caption = "Estado Antes".ToUpper
            .Visible = False
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grMaestro.RootTable.Columns("EstadoNuevo")
            .Width = 90
            .Caption = "Estado Despues".ToUpper
            .Visible = False
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grMaestro.RootTable.Columns("AnuladoAnterior")
            .Width = 90
            .Visible = False
        End With

        With grMaestro.RootTable.Columns("AnuladoNuevo")
            .Width = 90
            .Visible = False
        End With

        With grMaestro.RootTable.Columns("Observacion")
            .Width = 250
            .Caption = "Observacion".ToUpper
            .Visible = True
            .WordWrap = True
            .MaxLines = 3
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Near
        End With

        With grMaestro
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
        End With

        ''limpiar el detalle cada vez que se recarga la maestra
        grDetalle.DataSource = Nothing

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

        With grDetalle.RootTable.Columns("Id")
            .Width = 70
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("EventoId")
            .Width = 70
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("VentaId")
            .Width = 70
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 80
            .Caption = "Cod Producto".ToUpper
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grDetalle.RootTable.Columns("Producto")
            .Width = 220
            .Caption = "Producto".ToUpper
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Near
        End With

        With grDetalle.RootTable.Columns("TipoCambio")
            .Width = 150
            .Caption = "Cambio".ToUpper
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grDetalle.RootTable.Columns("CantidadAntes")
            .Width = 90
            .Caption = "Cant. Antes".ToUpper
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grDetalle.RootTable.Columns("CantidadDespues")
            .Width = 90
            .Caption = "Cant. Despues".ToUpper
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grDetalle.RootTable.Columns("PrecioAntes")
            .Width = 90
            .Caption = "Precio Antes".ToUpper
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grDetalle.RootTable.Columns("PrecioDespues")
            .Width = 90
            .Caption = "Precio Despues".ToUpper
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grDetalle
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
        End With
    End Sub

    Private Sub grMaestro_SelectionChanged(sender As Object, e As EventArgs) Handles grMaestro.SelectionChanged
        If (grMaestro.Row >= 0) Then
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
