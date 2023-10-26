Imports Negocio.AccesoLogica
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
        P_Global._prCargarComboGenerico(cbChofer, dt, "id", "Id", "Nombre", "Chofer")
        P_Global._prCargarComboGenerico(cbConciliacion, ListarConciliacion(), "id", "Id", "conciliacion", "Conciliacion")
        _prCargarPedidosPendientesAsignacion()
        _prCargarPedidosAnulados()
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
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosPendientes.RootTable.Columns("FechaPedido")
            .Width = 90
            .Visible = True
            .Caption = "Fecha Pedido"
            .FormatString = "dd/MM/yyyy"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosPendientes.RootTable.Columns("detalle")
            .Width = 90
            .Visible = True
            .LeftMargin = 4
            .TopMargin = 4
            .BottomMargin = 4
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Caption = "Detalle Pedido"
        End With
        With grPedidosPendientes.RootTable.Columns("NombrePersonal")
            .Width = 250
            .Caption = "Vendedor"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grPedidosPendientes.RootTable.Columns("NombreCliente")
            .Width = 250
            .Caption = "Cliente"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosPendientes.RootTable.Columns("Asignar")
            .Width = 90
            .Caption = "Seleccionar"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosPendientes.RootTable.Columns("PersonalId")
            .Visible = False
        End With

        With grPedidosPendientes.RootTable.Columns("totalPedido")
            .Width = 70
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
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
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grAsignados.RootTable.Columns("FechaPedido")
            .Width = 90
            .Visible = True
            .Caption = "Fecha Pedido"
            .FormatString = "dd/MM/yyyy"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grAsignados.RootTable.Columns("detalle")
            .Width = 90
            .Visible = True
            .LeftMargin = 4
            .TopMargin = 4
            .BottomMargin = 4
            .Caption = "Detalle Pedido"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grAsignados.RootTable.Columns("NombrePersonal")
            .Width = 250
            .Caption = "Vendedor"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With

        With grAsignados.RootTable.Columns("NombreCliente")
            .Width = 250
            .Caption = "Cliente"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grAsignados.RootTable.Columns("Asignar")
            .Width = 110
            .Caption = "Seleccionar"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grAsignados.RootTable.Columns("PersonalId")
            .Visible = False
        End With

        With grAsignados.RootTable.Columns("totalPedido")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
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

    Private Sub _prCargarPedidosEntregados(ChoferId As Integer)
        Dim dt As New DataTable

        If (ChoferId <= 0) Then
            Return

        End If
        dt = ListaPedidosEntregadosByChofer(ChoferId, cbConciliacion.Value)
        grPedidosEntregados.DataSource = dt
        grPedidosEntregados.RetrieveStructure()
        grPedidosEntregados.AlternatingColors = True
        ' Id	FechaPedido	NombreCliente	NombrePersonal	totalPedido	Asignar	detalle
        With grPedidosEntregados.RootTable.Columns("Id")
            .Width = 60
            .Caption = "Nro Pedido"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosEntregados.RootTable.Columns("FechaPedido")
            .Width = 90
            .Visible = True
            .Caption = "Fecha Pedido"
            .FormatString = "dd/MM/yyyy"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosEntregados.RootTable.Columns("detalle")
            .Width = 90
            .Visible = True
            .LeftMargin = 4
            .TopMargin = 4
            .BottomMargin = 4
            .Caption = "Detalle Pedido"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosEntregados.RootTable.Columns("NombrePersonal")
            .Width = 250
            .Caption = "Vendedor"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosEntregados.RootTable.Columns("conciliacion")
            .Width = 200
            .Caption = "Conciliacion"
            .Visible = True
            .WordWrap = True
            .MaxLines = 3
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosEntregados.RootTable.Columns("NombreCliente")
            .Width = 250
            .Caption = "Cliente"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosEntregados.RootTable.Columns("Asignar")
            .Width = 110
            .Caption = "Seleccionar"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosEntregados.RootTable.Columns("PersonalId")
            .Visible = False
        End With
        With grPedidosEntregados.RootTable.Columns("EstadoConciliacion")
            .Visible = False
        End With
        With grPedidosEntregados.RootTable.Columns("ConciliacionId")
            .Visible = False
        End With
        With grPedidosEntregados.RootTable.Columns("totalPedido")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total".ToUpper
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .AggregateFunction = AggregateFunction.Sum
        End With






        With grPedidosEntregados
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
        CargarIconosEntregado()
    End Sub

    Private Sub _prCargarPedidosAnulados()
        Dim dt As New DataTable

        dt = ListaPedidosAnulados()
        grPedidosAnulados.DataSource = dt
        grPedidosAnulados.RetrieveStructure()
        grPedidosAnulados.AlternatingColors = True
        ' Id	FechaPedido	NombreCliente	NombrePersonal	totalPedido	Asignar	detalle
        With grPedidosAnulados.RootTable.Columns("Id")
            .Width = 60
            .Caption = "Nro Pedido"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosAnulados.RootTable.Columns("FechaPedido")
            .Width = 90
            .Visible = True
            .Caption = "Fecha Pedido"
            .FormatString = "dd/MM/yyyy"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosAnulados.RootTable.Columns("FechaAnulacion")
            .Width = 90
            .Visible = True
            .Caption = "Fecha Anulada"
            .FormatString = "dd/MM/yyyy"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosAnulados.RootTable.Columns("detalle")
            .Width = 90
            .Visible = True
            .LeftMargin = 4
            .TopMargin = 4
            .BottomMargin = 4
            .Caption = "Detalle Pedido"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
        End With
        With grPedidosAnulados.RootTable.Columns("NombrePersonal")
            .Width = 250
            .Caption = "Vendedor"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
        End With

        With grPedidosAnulados.RootTable.Columns("NombreCliente")
            .Width = 250
            .Caption = "Cliente"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
        End With
        With grPedidosAnulados.RootTable.Columns("Asignar")
            .Width = 110
            .Caption = "Seleccionar"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
        End With
        With grPedidosAnulados.RootTable.Columns("PersonalId")
            .Visible = False
        End With


        With grPedidosAnulados.RootTable.Columns("totalPedido")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total".ToUpper
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .AggregateFunction = AggregateFunction.Sum
        End With






        With grPedidosAnulados
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
        CargarIconosAnulados()
    End Sub
    Public Sub CargarIconosAnulados()


        Dim BinCVariable As New MemoryStream
        Dim imgCVariable As New Bitmap(My.Resources.cvariables, 25, 20)
        imgCVariable.Save(BinCVariable, Imaging.ImageFormat.Png)



        Dim dt As DataTable = CType(grPedidosAnulados.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1

            CType(grPedidosAnulados.DataSource, DataTable).Rows(i).Item("detalle") = BinCVariable.GetBuffer

        Next

    End Sub
    Public Sub CargarIconosEntregado()


        Dim BinCVariable As New MemoryStream
        Dim imgCVariable As New Bitmap(My.Resources.cvariables, 25, 20)
        imgCVariable.Save(BinCVariable, Imaging.ImageFormat.Png)



        Dim dt As DataTable = CType(grPedidosEntregados.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1

            CType(grPedidosEntregados.DataSource, DataTable).Rows(i).Item("detalle") = BinCVariable.GetBuffer

        Next

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
        Me.Text = "Administrar Asignaciones Pedidos"
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

    Private Sub grPedidosAnulados_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grPedidosAnulados.EditingCell

        'a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        'a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '1 As estado, cast('' as image ) as img
        ', 0 as stock
        'Habilitar solo las columnas de Precio, %, Monto y Observación
        If (e.Column.Index = grPedidosAnulados.RootTable.Columns("Asignar").Index) Then
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

    Private Sub grEntregados_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grPedidosEntregados.EditingCell

        'a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        'a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '1 As estado, cast('' as image ) as img
        ', 0 as stock
        'Habilitar solo las columnas de Precio, %, Monto y Observación
        If (e.Column.Index = grPedidosEntregados.RootTable.Columns("Asignar").Index) Then
            e.Cancel = False
        Else
            e.Cancel = True

        End If


    End Sub
    Private Sub grAnulados_Click(sender As Object, e As EventArgs) Handles grPedidosAnulados.Click
        Try
            If (grPedidosAnulados.RowCount >= 1 And grPedidosAnulados.Row >= 0) Then
                If (grPedidosAnulados.CurrentColumn.Index = grPedidosAnulados.RootTable.Columns("detalle").Index) Then

                    Dim numi As String = ""
                    Dim ef = New Efecto
                    ef.tipo = 17
                    ef.VentaId = grPedidosAnulados.GetValue("Id")
                    ef.titulo = "Pedido # " + Str(grPedidosAnulados.GetValue("Id")) + "  Cliente = " + grPedidosAnulados.GetValue("NombreCliente")
                    ef.ShowDialog()

                End If

                ''Reporte
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub grEntregados_Click(sender As Object, e As EventArgs) Handles grPedidosEntregados.Click
        Try
            If (grPedidosEntregados.RowCount >= 1 And grPedidosEntregados.Row >= 0) Then
                If (grPedidosEntregados.CurrentColumn.Index = grPedidosEntregados.RootTable.Columns("detalle").Index) Then

                    Dim numi As String = ""
                    Dim ef = New Efecto
                    ef.tipo = 17
                    ef.VentaId = grPedidosEntregados.GetValue("Id")
                    ef.titulo = "Pedido # " + Str(grPedidosEntregados.GetValue("Id")) + "  Cliente = " + grPedidosEntregados.GetValue("NombreCliente")
                    ef.ShowDialog()

                End If

                ''Reporte
            End If
        Catch ex As Exception

        End Try
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

    Public Function ExisteItemSeleccionadosEntregado()

        Dim dt As DataTable = CType(grPedidosEntregados.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("Asignar") = True) Then
                Return True

            End If

        Next



        Return False

    End Function

    Public Function ExisteItemSeleccionadosAnulaciones()

        Dim dt As DataTable = CType(grPedidosAnulados.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("Asignar") = True) Then
                Return True

            End If

        Next



        Return False

    End Function

    Public Function ExisteItemSeleccionadoConciliacionCerrada()

        Dim dt As DataTable = CType(grPedidosEntregados.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("Asignar") = True And dt.Rows(i).Item("EstadoConciliacion") = 0) Then

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
            _prCargarPedidosAsignados(cbPersonalAsignado.Value)
        Else
            ToastNotification.Show(Me, "Error no se Pudo Realizar las Asignaciones", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If


    End Sub

    Private Sub ButtonX6_Click(sender As Object, e As EventArgs) Handles ButtonX6.Click
        If (Not ExisteItemSeleccionadosDesAsignar()) Then
            ToastNotification.Show(Me, "No Existe Ningun Pedido Seleccionado", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

        Dim bandera As Boolean = VentaQuitarAsignaciones(CType(grAsignados.DataSource, DataTable))
        If (bandera = True) Then
            ToastNotification.Show(Me, "Se han Quitado Las Asignaciones Correctamente", My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
            _prCargarPedidosAsignados(cbPersonalAsignado.Value)

            _prCargarPedidosPendientesAsignacion()



        Else
            ToastNotification.Show(Me, "Error no se Pudo completar el proceso", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
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

    Private Sub ButtonX7_Click(sender As Object, e As EventArgs) Handles ButtonX7.Click
        Dim dt As DataTable = ListaProductosAsignadosByChofer(cbPersonalAsignado.Value)


        If (dt.Rows.Count > 0) Then
            Dim dtImage As DataTable = ObtenerImagenEmpresa()
            Dim NombreEmpresa As String = dtImage.Rows(0).Item("Nombre")
            Dim Direccion As String = dtImage.Rows(0).Item("Direccion")
            If (dtImage.Rows.Count > 0) Then
                Dim RutaGlobal As String = gs_CarpetaRaiz
                Dim Name As String = dtImage.Rows(0).Item(0)
                If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name)) Then
                    Dim im As New Bitmap(New Bitmap(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name))
                    Dim Bin As New MemoryStream
                    Dim img As New Bitmap(im)
                    img.Save(Bin, Imaging.ImageFormat.Png)
                    Bin.Dispose()
                    For i As Integer = 0 To dt.Rows.Count - 1 Step 1


                        dt.Rows(i).Item("img") = Bin.GetBuffer
                    Next
                End If


            End If


            If Not IsNothing(P_Global.Visualizador) Then
                P_Global.Visualizador.Close()
            End If



            P_Global.Visualizador = New Visualizador

            Dim objrep As New Reporte_ProductosAsignados

            objrep.SetDataSource(dt)
            objrep.SetParameterValue("Chofer", cbPersonalAsignado.Text)
            objrep.SetParameterValue("NombreEmpresa", NombreEmpresa)
            objrep.SetParameterValue("Ciudad", Direccion)
            objrep.SetParameterValue("Usuario", L_Usuario)
            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.CrGeneral.Zoom(90)
            P_Global.Visualizador.Show() 'Comentar

        Else
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

    End Sub

    Private Sub ButtonX8_Click(sender As Object, e As EventArgs) Handles ButtonX8.Click
        Dim dt As DataTable = ListaClientesAsignadosByChofer(cbPersonalAsignado.Value)


        If (dt.Rows.Count > 0) Then
            Dim dtImage As DataTable = ObtenerImagenEmpresa()
            Dim NombreEmpresa As String = dtImage.Rows(0).Item("Nombre")
            Dim Direccion As String = dtImage.Rows(0).Item("Direccion")
            If (dtImage.Rows.Count > 0) Then
                Dim RutaGlobal As String = gs_CarpetaRaiz
                Dim Name As String = dtImage.Rows(0).Item(0)
                If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name)) Then
                    Dim im As New Bitmap(New Bitmap(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name))
                    Dim Bin As New MemoryStream
                    Dim img As New Bitmap(im)
                    img.Save(Bin, Imaging.ImageFormat.Png)
                    Bin.Dispose()
                    For i As Integer = 0 To dt.Rows.Count - 1 Step 1


                        dt.Rows(i).Item("img") = Bin.GetBuffer
                    Next
                End If


            End If


            If Not IsNothing(P_Global.Visualizador) Then
                P_Global.Visualizador.Close()
            End If



            P_Global.Visualizador = New Visualizador

            Dim objrep As New Reporte_ClientesAsignados

            objrep.SetDataSource(dt)
            objrep.SetParameterValue("Chofer", cbPersonalAsignado.Text)
            objrep.SetParameterValue("NombreEmpresa", NombreEmpresa)
            objrep.SetParameterValue("Ciudad", Direccion)
            objrep.SetParameterValue("Usuario", L_Usuario)
            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.CrGeneral.Zoom(90)
            P_Global.Visualizador.Show() 'Comentar

        Else
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If
    End Sub

    Private Sub ButtonX10_Click(sender As Object, e As EventArgs) Handles ButtonX10.Click
        If (Not ExisteItemSeleccionados()) Then
            ToastNotification.Show(Me, "No Existe Ningun Pedido Seleccionado", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If
        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Anulación"
        ef.descripcion = "¿Esta Seguro de Anular Todos Los Pedidos Seleccionados ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            bandera = VentaAnularPedidos(CType(grPedidosPendientes.DataSource, DataTable))
            If (bandera = True) Then
                ToastNotification.Show(Me, "Los Pedidos Han Sido Anulados Correctamente", My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)


                _prCargarPedidosPendientesAsignacion()
                _prCargarPedidosAnulados()


            Else
                ToastNotification.Show(Me, "Error no se Pudo completar el proceso", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If
        End If

    End Sub

    Private Sub ButtonX9_Click(sender As Object, e As EventArgs) Handles ButtonX9.Click
        If (Not ExisteItemSeleccionadosDesAsignar()) Then
            ToastNotification.Show(Me, "No Existe Ningun Pedido Seleccionado", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Anulación"
        ef.descripcion = "¿Esta Seguro de Anular Todos Los Pedidos Seleccionados ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            bandera = VentaAnularPedidos(CType(grAsignados.DataSource, DataTable))
            If (bandera = True) Then
                ToastNotification.Show(Me, "Los Pedidos Han Sido Anulados Correctamente", My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _prCargarPedidosAsignados(cbPersonalAsignado.Value)

                _prCargarPedidosAnulados()


            Else
                ToastNotification.Show(Me, "Error no se Pudo completar el proceso", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If
        End If

    End Sub

    Private Sub ButtonX11_Click(sender As Object, e As EventArgs) Handles btnEntregar.Click
        If (Not ExisteItemSeleccionadosDesAsignar()) Then
            ToastNotification.Show(Me, "No Existe Ningun Pedido Seleccionado", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

        Dim Mensaje As String = ""
        Dim bandera As Boolean = VentaEntregarPedidos(CType(grAsignados.DataSource, DataTable), cbPersonalAsignado.Value, Mensaje, cbPersonalAsignado.Text)
        If (bandera = True) Then
            ToastNotification.Show(Me, "Los Pedidos Ha Pasado Al Estado Entregado", My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
            _prCargarPedidosAsignados(cbPersonalAsignado.Value)

            _prCargarPedidosEntregados(cbChofer.Value)



        Else
            ToastNotification.Show(Me, Mensaje, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub ButtonX16_Click(sender As Object, e As EventArgs) Handles ButtonX16.Click
        _prCargarPedidosEntregados(cbChofer.Value)

    End Sub

    Private Sub cbChofer_ValueChanged(sender As Object, e As EventArgs) Handles cbChofer.ValueChanged
        Try
            _prCargarPedidosEntregados(cbChofer.Value)
        Catch ex As Exception

        End Try


    End Sub

    Private Sub ButtonX15_Click(sender As Object, e As EventArgs) Handles ButtonX15.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Private Sub ButtonX17_Click(sender As Object, e As EventArgs) Handles btnEntregadoAAsignado.Click
        If (Not ExisteItemSeleccionadosEntregado()) Then
            ToastNotification.Show(Me, "No Existe Ningun Pedido Seleccionado", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

        If (ExisteItemSeleccionadoConciliacionCerrada()) Then  ''Estado ConciliacionCerrada
            ToastNotification.Show(Me, "No Es Posible Revertir Estado Por Que Existen Pedidos Que Pertenece A una Conciliacion Cerrada", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return

        End If
        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Reversión"
        ef.descripcion = "¿Esta Seguro de Revertir El Estado Del Pedido ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            bandera = VentaRevertiEstadoEntregado(CType(grPedidosEntregados.DataSource, DataTable))
            If (bandera = True) Then
                ToastNotification.Show(Me, "Los Pedidos Han Sido Revertido Al Estado Asignado", My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _prCargarPedidosAsignados(cbPersonalAsignado.Value)
                _prCargarPedidosEntregados(cbChofer.Value)



            Else
                ToastNotification.Show(Me, "Error no se Pudo completar el proceso", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If
        End If
    End Sub

    Private Sub ButtonX12_Click(sender As Object, e As EventArgs) Handles ButtonX12.Click
        _prCargarPedidosAnulados()

    End Sub

    Private Sub ButtonX11_Click_1(sender As Object, e As EventArgs) Handles ButtonX11.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Private Sub ButtonX13_Click(sender As Object, e As EventArgs) Handles ButtonX13.Click
        If (Not ExisteItemSeleccionadosAnulaciones()) Then
            ToastNotification.Show(Me, "No Existe Ningun Pedido Seleccionado", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If
        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Reversión"
        ef.descripcion = "¿Esta Seguro de Revertir Estado Del Pedido a Estado Pendiente?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            bandera = VentaRevertirAnulacion(CType(grPedidosAnulados.DataSource, DataTable))
            If (bandera = True) Then
                ToastNotification.Show(Me, "Reversion Exitosa. Los Pedidos Han Pasado Al Estado Pendiente", My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)


                _prCargarPedidosPendientesAsignacion()
                _prCargarPedidosAnulados()


            Else
                ToastNotification.Show(Me, "Error no se Pudo completar el proceso", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If
        End If
    End Sub

    Private Sub cbConciliacion_ValueChanged(sender As Object, e As EventArgs) Handles cbConciliacion.ValueChanged
        Try
            _prCargarPedidosEntregados(cbChofer.Value)
        Catch ex As Exception

        End Try
    End Sub
End Class