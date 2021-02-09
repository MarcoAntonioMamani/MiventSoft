﻿Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Public Class Tec_AdministrarAsignacionesPedidos
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 40, 40)


    Sub Iniciartodo()


        Dim dt As DataTable = ListarSoloChoferes()
        dt = dt.DefaultView.ToTable(False, "Id", "Nombre")
        P_Global._prCargarComboGenerico(cbPersonal, dt, "id", "Id", "Nombre", "Chofer")
        P_Global._prCargarComboGenerico(cbPersonalAsignado, dt, "id", "Id", "Nombre", "Chofer")

        _prCargarPedidosPendientesAsignacion()

    End Sub

    Private Sub _prCargarPedidosPendientesAsignacion()
        Dim dt As New DataTable
        dt = ListaPedidosPendientesAsignaciones()
        grPedidosPendientes.DataSource = dt
        grPedidosPendientes.RetrieveStructure()
        grPedidosPendientes.AlternatingColors = True
        ' Id	FechaPedido	NombreCliente	NombrePersonal	totalPedido	Asignar	detalle
        With grPedidosPendientes.RootTable.Columns("Id")
            .Width = 60
            .Caption = "Nro Pedido"
            .Visible = True

        End With
        With grPedidosPendientes.RootTable.Columns("FechaPedido")
            .Width = 90
            .Visible = True
            .Caption = "Fecha Pedido"
            .FormatString = "dd/MM/yyyy"

        End With
        With grPedidosPendientes.RootTable.Columns("detalle")
            .Width = 90
            .Visible = True
            .LeftMargin = 4
            .TopMargin = 4
            .BottomMargin = 4
            .Caption = "Detalle Pedido"
        End With
        With grPedidosPendientes.RootTable.Columns("NombrePersonal")
            .Width = 250
            .Caption = "Vendedor"
            .Visible = True
        End With

        With grPedidosPendientes.RootTable.Columns("NombreCliente")
            .Width = 250
            .Caption = "Cliente"
            .Visible = True
        End With
        With grPedidosPendientes.RootTable.Columns("Asignar")
            .Width = 90
            .Caption = "Asignar"
            .Visible = True
        End With
        With grPedidosPendientes.RootTable.Columns("PersonalId")
            .Visible = False
        End With

        With grPedidosPendientes.RootTable.Columns("totalPedido")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total".ToUpper
            .AggregateFunction = AggregateFunction.Sum
        End With






        With grPedidosPendientes
            .GroupByBoxVisible = False
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 9
            .TotalRowPosition = TotalRowPosition.BottomFixed
        End With
        CargarIconos()
    End Sub

    Private Sub _prCargarPedidosAsignados(ChoferId As Integer)
        Dim dt As New DataTable
        dt = ListaPedidosAsignadosByChofer(ChoferId)
        grAsignados.DataSource = dt
        grAsignados.RetrieveStructure()
        grAsignados.AlternatingColors = True
        ' Id	FechaPedido	NombreCliente	NombrePersonal	totalPedido	Asignar	detalle
        With grAsignados.RootTable.Columns("Id")
            .Width = 60
            .Caption = "Nro Pedido"
            .Visible = True

        End With
        With grAsignados.RootTable.Columns("FechaPedido")
            .Width = 90
            .Visible = True
            .Caption = "Fecha Pedido"
            .FormatString = "dd/MM/yyyy"

        End With
        With grAsignados.RootTable.Columns("detalle")
            .Width = 90
            .Visible = True
            .LeftMargin = 4
            .TopMargin = 4
            .BottomMargin = 4
            .Caption = "Detalle Pedido"
        End With
        With grAsignados.RootTable.Columns("NombrePersonal")
            .Width = 250
            .Caption = "Vendedor"
            .Visible = True
        End With

        With grAsignados.RootTable.Columns("NombreCliente")
            .Width = 250
            .Caption = "Cliente"
            .Visible = True
        End With
        With grAsignados.RootTable.Columns("Asignar")
            .Width = 110
            .Caption = "Quitar Asignación"
            .Visible = True
        End With
        With grAsignados.RootTable.Columns("PersonalId")
            .Visible = False
        End With

        With grAsignados.RootTable.Columns("totalPedido")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total".ToUpper
            .AggregateFunction = AggregateFunction.Sum
        End With






        With grAsignados
            .GroupByBoxVisible = False
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 9
            .TotalRowPosition = TotalRowPosition.BottomFixed
        End With
        CargarIconosAsignados()
    End Sub
    Public Sub CargarIconosAsignados()


        Dim BinCVariable As New MemoryStream
        Dim imgCVariable As New Bitmap(My.Resources.cvariables, 25, 20)
        imgCVariable.Save(BinCVariable, Imaging.ImageFormat.Png)



        Dim dt As DataTable = CType(grAsignados.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1

            CType(grAsignados.DataSource, DataTable).Rows(i).Item("detalle") = BinCVariable.GetBuffer

        Next

    End Sub
    Public Sub CargarIconos()


        Dim BinCVariable As New MemoryStream
        Dim imgCVariable As New Bitmap(My.Resources.cvariables, 25, 20)
        imgCVariable.Save(BinCVariable, Imaging.ImageFormat.Png)



        Dim dt As DataTable = CType(grPedidosPendientes.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1

            CType(grPedidosPendientes.DataSource, DataTable).Rows(i).Item("detalle") = BinCVariable.GetBuffer

        Next

    End Sub
    Private Sub Tec_AdministrarAsignacionesPedidos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Iniciartodo()

    End Sub



    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grPedidosPendientes.EditingCell

        'a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        'a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '1 As estado, cast('' as image ) as img
        ', 0 as stock
        'Habilitar solo las columnas de Precio, %, Monto y Observación
        If (e.Column.Index = grPedidosPendientes.RootTable.Columns("Asignar").Index) Then
            e.Cancel = False
        Else
            e.Cancel = True

        End If


    End Sub

    Private Sub grAsignados_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grAsignados.EditingCell

        'a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        'a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '1 As estado, cast('' as image ) as img
        ', 0 as stock
        'Habilitar solo las columnas de Precio, %, Monto y Observación
        If (e.Column.Index = grAsignados.RootTable.Columns("Asignar").Index) Then
            e.Cancel = False
        Else
            e.Cancel = True

        End If


    End Sub
    Private Sub grAsignados_Click(sender As Object, e As EventArgs) Handles grAsignados.Click
        Try
            If (grAsignados.RowCount >= 1 And grAsignados.Row >= 0) Then
                If (grAsignados.CurrentColumn.Index = grAsignados.RootTable.Columns("detalle").Index) Then

                    Dim numi As String = ""
                    Dim ef = New Efecto
                    ef.tipo = 17
                    ef.VentaId = grAsignados.GetValue("Id")
                    ef.titulo = "Pedido # " + Str(grAsignados.GetValue("Id")) + "  Cliente = " + grAsignados.GetValue("NombreCliente")
                    ef.ShowDialog()

                End If

                ''Reporte
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub grplanilla_Click(sender As Object, e As EventArgs) Handles grPedidosPendientes.Click
        Try
            If (grPedidosPendientes.RowCount >= 1 And grPedidosPendientes.Row >= 0) Then
                If (grPedidosPendientes.CurrentColumn.Index = grPedidosPendientes.RootTable.Columns("detalle").Index) Then

                    Dim numi As String = ""
                    Dim ef = New Efecto
                    ef.tipo = 17
                    ef.VentaId = grPedidosPendientes.GetValue("Id")
                    ef.titulo = "Pedido # " + Str(grPedidosPendientes.GetValue("Id")) + "  Cliente = " + grPedidosPendientes.GetValue("NombreCliente")
                    ef.ShowDialog()

                End If

                ''Reporte
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        _prCargarPedidosPendientesAsignacion()
    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub
    Public Function ExisteItemSeleccionadosDesAsignar()

        Dim dt As DataTable = CType(grAsignados.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("Asignar") = True) Then
                Return True

            End If

        Next



        Return False

    End Function
    Public Function ExisteItemSeleccionados()

        Dim dt As DataTable = CType(grPedidosPendientes.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("Asignar") = True) Then
                Return True

            End If

        Next



        Return False

    End Function

    Private Sub ButtonX4_Click(sender As Object, e As EventArgs) Handles ButtonX4.Click

        If (Not ExisteItemSeleccionados()) Then
            ToastNotification.Show(Me, "No Existe Ningun Pedido Seleccionado", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If
        If (cbPersonal.SelectedIndex < 0) Then
            ToastNotification.Show(Me, "Seleccione un Chofer", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If
        Dim bandera As Boolean = VentaInsertarAsignaciones(CType(grPedidosPendientes.DataSource, DataTable), cbPersonal.Value)
        If (bandera = True) Then
            ToastNotification.Show(Me, "Asignacion Realizada Correctamente ", My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
            _prCargarPedidosPendientesAsignacion()

        Else
            ToastNotification.Show(Me, "Error no se Pudo Realizar las Asignaciones", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If


    End Sub

    Private Sub ButtonX6_Click(sender As Object, e As EventArgs) Handles ButtonX6.Click
        If (Not ExisteItemSeleccionadosDesAsignar()) Then
            ToastNotification.Show(Me, "No Existe Ningun Pedido Seleccionado", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If
    End Sub

    Private Sub cbPersonalAsignado_ValueChanged(sender As Object, e As EventArgs) Handles cbPersonalAsignado.ValueChanged
        Try
            _prCargarPedidosAsignados(cbPersonalAsignado.Value)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ButtonX5_Click(sender As Object, e As EventArgs) Handles ButtonX5.Click
        If (cbPersonalAsignado.SelectedIndex >= 0) Then
            _prCargarPedidosAsignados(cbPersonalAsignado.Value)
        End If

    End Sub

    Private Sub ButtonX3_Click(sender As Object, e As EventArgs) Handles ButtonX3.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub
End Class