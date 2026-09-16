Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX

Public Class Tec_AuditoriaVentas

#Region "Atributos"
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
#End Region

#Region "Eventos"
    Private Sub Tec_AuditoriaVentas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized

        tbDesde.Value = New Date(Today.Year, Today.Month, 1)
        tbHasta.Value = Today
        cbAccion.SelectedIndex = 0

        CargarEventos()
    End Sub

    Private Sub btnFiltrar_Click(sender As Object, e As EventArgs) Handles btnFiltrar.Click
        If (tbDesde.Value > tbHasta.Value) Then
            ToastNotification.Show(Me, "La Fecha Desde Debe Ser Menor Que la Fecha Hasta", img, 5000, eToastGlowColor.Red, eToastPosition.BottomRight)
        Else
            CargarEventos()
        End If
    End Sub

    Private Sub btnVerComparacion_Click(sender As Object, e As EventArgs) Handles btnVerComparacion.Click
        CargarComparacion()
    End Sub
#End Region

#Region "Carga de Grillas"
    ''Lista de eventos (Crear/Modificar/Eliminar) de Ventas segun el filtro de fechas/usuario/accion.
    Private Sub CargarEventos()
        Dim dt As DataTable = ListarEventosAuditoriaVentas(Format(tbDesde.Value, "yyyy-MM-dd"), Format(tbHasta.Value, "yyyy-MM-dd"), tbUsuario.Text.Trim, cbAccion.Text)

        grEventos.DataSource = dt
        grEventos.RetrieveStructure()

        With grEventos.RootTable.Columns("Id")
            .Width = 80
            .Caption = "N°"
        End With
        With grEventos.RootTable.Columns("Accion")
            .Width = 100
            .Caption = "ACCION".ToUpper
        End With
        With grEventos.RootTable.Columns("UsuarioAccion")
            .Width = 120
            .Caption = "USUARIO".ToUpper
        End With
        With grEventos.RootTable.Columns("FechaAccion")
            .Width = 100
            .Caption = "FECHA".ToUpper
            .FormatString = "dd/MM/yyyy"
        End With
        With grEventos.RootTable.Columns("HoraAccion")
            .Width = 70
            .Caption = "HORA".ToUpper
        End With
        With grEventos.RootTable.Columns("TotalVenta")
            .Width = 100
            .Caption = "TOTAL".ToUpper
            .FormatString = "0.00"
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
        End With
        With grEventos.RootTable.Columns("NombreCliente")
            .Width = 200
            .Caption = "CLIENTE".ToUpper
        End With

        ''Al recargar la lista se limpia la comparacion anterior para no dejar
        ''en pantalla el Antes/Despues de un evento que ya no esta filtrado.
        grCabAntes.DataSource = Nothing
        grCabDespues.DataSource = Nothing
        grDetAntes.DataSource = Nothing
        grDetDespues.DataSource = Nothing
    End Sub

    ''Trae el Antes/Despues (cabecera y detalle) del evento seleccionado en grEventos
    ''y llena las 4 grillas de comparacion lado a lado.
    Private Sub CargarComparacion()
        If (grEventos.Row < 0) Then
            ToastNotification.Show(Me, "Seleccione un evento de la lista", img, 4000, eToastGlowColor.Red, eToastPosition.BottomRight)
            Return
        End If

        Dim id As Integer = grEventos.GetValue("Id")
        Dim accion As String = grEventos.GetValue("Accion")
        Dim fechaAccion As String = Format(CDate(grEventos.GetValue("FechaAccion")), "yyyy-MM-dd")
        Dim horaAccion As String = grEventos.GetValue("HoraAccion")

        Dim dtCab As DataTable = CompararCabeceraAuditoriaVentas(id, accion, fechaAccion, horaAccion)
        _prCargarGridComparacion(grCabAntes, dtCab, "ANTES")
        _prCargarGridComparacion(grCabDespues, dtCab, "DESPUES")

        Dim dtDet As DataTable = CompararDetalleAuditoriaVentas(id, accion, fechaAccion, horaAccion)
        _prCargarGridComparacion(grDetAntes, dtDet, "ANTES")
        _prCargarGridComparacion(grDetDespues, dtDet, "DESPUES")
    End Sub

    ''El SP siempre devuelve una columna "Momento" (ANTES/DESPUES) para poder separar,
    ''del mismo resultado, la fila que va a la grilla izquierda de la que va a la derecha.
    Private Sub _prCargarGridComparacion(grid As Janus.Windows.GridEX.GridEX, dtOrigen As DataTable, momento As String)
        Dim dv As New DataView(dtOrigen)
        dv.RowFilter = "Momento = '" & momento & "'"

        grid.DataSource = dv.ToTable()
        grid.RetrieveStructure()

        If (grid.RootTable.Columns.Contains("Momento")) Then
            grid.RootTable.Columns("Momento").Visible = False
        End If
        If (grid.RootTable.Columns.Contains("AuditoriaId")) Then
            grid.RootTable.Columns("AuditoriaId").Visible = False
        End If
    End Sub
#End Region

End Class
