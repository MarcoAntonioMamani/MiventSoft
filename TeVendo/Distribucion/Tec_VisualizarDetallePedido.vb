Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX

Public Class Tec_VisualizarDetallePedido

    Public title As String = ""
    Public VentaId As Integer = 0
    Private Sub Tec_VisualizarDetallePedido_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lbtitle.Text = title
        _prCargarDetalleVenta(VentaId)
    End Sub

    Private Sub _prCargarDetalleVenta(_numi As String)
        Dim dt As New DataTable
        dt = ListaVentasDetalles(_numi)
        grDetalle.DataSource = dt
        grDetalle.RetrieveStructure()
        grDetalle.AlternatingColors = True
        '     a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        '     a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '     1 As estado, cast('' as image ) as img
        '     ,   as stock
        With grDetalle.RootTable.Columns("Id")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("VentaId")
            .Width = 100
            .Visible = False

        End With


        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 30
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Caption = "Cod Producto"
        End With

        With grDetalle.RootTable.Columns("Producto")
            .Width = 150
            .Caption = "Producto"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
        End With

        With grDetalle.RootTable.Columns("Cantidad")
            .Width = 50
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad".ToUpper
        End With


        With grDetalle.RootTable.Columns("Precio")
            .Width = 50
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
            .Caption = "Precio"
            .FormatString = "0.00"
        End With

        With grDetalle.RootTable.Columns("SubTotal")
            .Width = 60
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
            .Caption = "SubTotal"
            .FormatString = "0.00"
        End With
        '     a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        '     a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '     1 As estado, cast('' as image ) as img
        '     ,   as stock

        With grDetalle.RootTable.Columns("ProcentajeDescuento")
            .Width = 70
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
            .FormatString = "0"
            .Caption = "P.Descuento".ToUpper
        End With

        With grDetalle.RootTable.Columns("MontoDescuento")
            .Width = 70
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = True
            .FormatString = "0.00"
            .Caption = "M.Descuento".ToUpper
        End With


        With grDetalle.RootTable.Columns("Total")
            .Width = 60
            .Visible = True
            .Caption = "Total".ToUpper
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With

        With grDetalle.RootTable.Columns("Detalle")
            .Width = 50
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("PrecioCosto")
            .Width = 90
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .FormatString = "0.00"
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("stock")
            .Width = 90
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .FormatString = "0.00"
            .Visible = False
        End With


        With grDetalle.RootTable.Columns("estado")
            .Width = 50
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("img")
                .Width = 80
                .Caption = "Eliminar".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = False
            End With





        With grDetalle.RootTable.Columns("Lote")
                .Width = 120
                .Caption = "lote".ToUpper

                .Visible = False
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 120
                .Caption = "FECHA VENC.".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = False
            End With




        With grDetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed
        End With

    End Sub
    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        Me.Close()
    End Sub


End Class