Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX

Public Class Rep_HistorialIncrementoVenta

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Rep_HistorialIncrementoVenta_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cbFechaDesde.Value = Now.Date.AddDays(-30)
        cbFechaHasta.Value = Now.Date
        _prCargarHistorial()
    End Sub

    ''Misma ayuda defensiva que usamos en Rep_AuditoriaVentas: configura la columna
    ''solo si existe en el DataTable devuelto, asi no se rompe el formulario.
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

    Private Sub _prCargarHistorial()
        Dim dt As New DataTable
        dt = L_prListarHistorialIncrementoVenta(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))

        grHistorial.DataSource = dt
        grHistorial.RetrieveStructure()
        grHistorial.AlternatingColors = True

        _prConfigColumna(grHistorial, "Id", 70, "COD", False)
        _prConfigColumna(grHistorial, "Fecha", 110, "DIA", True, "dd/MM/yyyy")
        _prConfigColumna(grHistorial, "Monto", 120, "MONTO (Bs)", True, "0.00")
        _prConfigColumna(grHistorial, "Usuario", 220, "MODIFICADO POR", True, "", False, True)
        _prConfigColumna(grHistorial, "FechaHoraModificacion", 180, "FECHA Y HORA DEL CAMBIO", True, "dd/MM/yyyy HH:mm:ss")

        With grHistorial
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
            .ColumnAutoResize = True
            .GridLines = Janus.Windows.GridEX.GridLines.None
            .BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        End With

        lblHistorial.Text = "CAMBIOS REGISTRADOS  (" & dt.Rows.Count.ToString() & ")"

        If (dt.Rows.Count = 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Cambios Registrados En El Rango Seleccionado".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        _prCargarHistorial()
    End Sub

End Class
