Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Imports System.IO
Public Class F_MonitoreoProductos
    Dim dtStock0 As DataTable
    Dim dtStockMinimo As DataTable
    Dim dtProductosVencidos As DataTable
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public Sub IniciarProceso()

        _prCargarProductosSinStock()
        _prCargarProductosStockMinimo()
        _prCargarProductosVencidos()
    End Sub
    Private Sub _prCargarProductosSinStock()
        Dim dt As New DataTable
        dt = L_prListarProductosConStock0()
        dtStock0 = dt.Copy
        gr_ProductosStock0.DataSource = dt
        gr_ProductosStock0.RetrieveStructure()
        gr_ProductosStock0.AlternatingColors = True
        'Id  NombreProducto	NombreCategoria	cantidad


        With gr_ProductosStock0.RootTable.Columns("Id")
            .Width = 90
            .Visible = True
            .Caption = "Id Producto"
        End With

        With gr_ProductosStock0.RootTable.Columns("NombreCategoria")
            .Width = 150
            .Caption = "Categoria"
            .Visible = True
        End With
        With gr_ProductosStock0.RootTable.Columns("NombreProducto")
            .Width = 250
            .Caption = "Producto"
            .Visible = True
        End With
        With gr_ProductosStock0.RootTable.Columns("cantidad")
            .Width = 110
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad Disponible"
        End With







        With gr_ProductosStock0
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True

            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False

        End With

    End Sub

    Private Sub _prCargarProductosStockMinimo()
        Dim dt As New DataTable
        dt = L_prListarProductosConStockMinimo()
        dtStockMinimo = dt.Copy
        grProductosStockMinimo.DataSource = dt
        grProductosStockMinimo.RetrieveStructure()
        grProductosStockMinimo.AlternatingColors = True
        'Id  NombreProducto	NombreCategoria	cantidad


        With grProductosStockMinimo.RootTable.Columns("Id")
            .Width = 90
            .Visible = True
            .Caption = "Id Producto"
        End With

        With grProductosStockMinimo.RootTable.Columns("NombreCategoria")
            .Width = 150
            .Caption = "Categoria"
            .Visible = True
        End With
        With grProductosStockMinimo.RootTable.Columns("NombreProducto")
            .Width = 250
            .Caption = "Producto"
            .Visible = True
        End With
        With grProductosStockMinimo.RootTable.Columns("StockMinimo")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Stock Minimo"
        End With

        With grProductosStockMinimo.RootTable.Columns("cantidadActual")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Stock Actual"
        End With






        With grProductosStockMinimo
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True

            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False

        End With

    End Sub

    Private Sub _prCargarProductosVencidos()
        Dim dt As New DataTable
        dt = L_prListarProductosStockVencidos()
        dtProductosVencidos = dt.Copy
        gr_ProductosVencidos.DataSource = dt
        gr_ProductosVencidos.RetrieveStructure()
        gr_ProductosVencidos.AlternatingColors = True
        'Id	NombreProducto	lote	FechaVencimiento	cantidad


        With gr_ProductosVencidos.RootTable.Columns("Id")
            .Width = 90
            .Visible = True
            .Caption = "Id Producto"
        End With

        With gr_ProductosVencidos.RootTable.Columns("lote")
            .Width = 70
            .Caption = "Lote"
            .Visible = True
        End With
        With gr_ProductosVencidos.RootTable.Columns("FechaVencimiento")
            .Width = 100
            .FormatString = "dd/MM/yyyy"
            .Caption = "Fecha Vencimiento"
            .Visible = True
        End With
        With gr_ProductosVencidos.RootTable.Columns("NombreProducto")
            .Width = 250
            .Caption = "Producto"
            .Visible = True
        End With
        With gr_ProductosVencidos.RootTable.Columns("cantidad")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Stock Actual"
        End With







        With gr_ProductosVencidos
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True

            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False

        End With

    End Sub
    Private Sub F_MonitoreoProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarProceso()
    End Sub

    Private Sub tabCreditoPendiente_Click(sender As Object, e As EventArgs) Handles tabProductosSinStock.Click
        _prCargarProductosSinStock()
    End Sub

    Private Sub tabProductosStockMinimo_Click(sender As Object, e As EventArgs) Handles tabProductosStockMinimo.Click
        _prCargarProductosStockMinimo()
    End Sub

    Private Sub tabProductosVencidos_Click(sender As Object, e As EventArgs) Handles tabProductosVencidos.Click
        _prCargarProductosStockMinimo()
    End Sub
End Class