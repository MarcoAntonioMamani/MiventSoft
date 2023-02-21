Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class frm_VerProductosVendidos


    Dim FilaSelectLote As DataRow = Nothing
    Public dtProductos As DataTable
    Public dtDetalle As DataTable
    Public Lote As Boolean

    Public TipoMovimientoId As Integer
    Public SucursalId As Integer
    Public IdCliente As Integer
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)


    Public TipoProgramas As Integer = 0
    Public Sub IniciarTodod()
        CargarProductosVentas()

    End Sub
    Private Sub Tec_DespachoDetalle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarTodod()

    End Sub


    Public Sub New(dtv As DataTable)
        InitializeComponent()
        dtDetalle = dtv
    End Sub
    Public Sub CargarProductosVentas()
        Dim bandera As Boolean = False


        grDetalle.DataSource = dtDetalle
        grDetalle.RetrieveStructure()
        grDetalle.AlternatingColors = True
        With grDetalle.RootTable.Columns("Id")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("VentaId")
            .Width = 100
            .Visible = False

        End With


        With grDetalle.RootTable.Columns("Tipo")
            .Width = 100
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("TipoNombre")
            .Width = 60
            .Visible = False
            .Caption = "Tipo"
        End With
        With grDetalle.RootTable.Columns("KitId")
            .Width = 100
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("KitNombre")
            .Width = 100
            .Visible = False
            .Caption = "Kit"
            .WordWrap = True
            .MaxLines = 3
        End With

        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 30
            .Visible = True
            .Caption = "Cod Producto"
        End With

        With grDetalle.RootTable.Columns("Producto")
            .Width = 150
            .Caption = "Producto"
            .Visible = True
        End With

        With grDetalle.RootTable.Columns("CantidadKit")
            .Width = 40
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "CantidadKit"
        End With
        With grDetalle.RootTable.Columns("Cantidad")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad".ToUpper
        End With


        With grDetalle.RootTable.Columns("Precio")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "Precio"
            .FormatString = "0.00"
        End With

        With grDetalle.RootTable.Columns("SubTotal")
            .Width = 60
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
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
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0"
            .Caption = "%.Descuento".ToUpper
        End With

        With grDetalle.RootTable.Columns("MontoDescuento")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "M.Descuento".ToUpper
        End With


        With grDetalle.RootTable.Columns("Total")
            .Width = 60
            .Visible = True
            .Caption = "Total".ToUpper
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With

        With grDetalle.RootTable.Columns("Detalle")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("PrecioCosto")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("stock")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .Visible = False
        End With


        With grDetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar".ToUpper
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = False
        End With


        If (Lote = True) Then
            With grDetalle.RootTable.Columns("Lote")
                .Width = 60
                .Caption = "lote".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 70
                .Caption = "FECHA VENC.".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = True
            End With
        Else

            With grDetalle.RootTable.Columns("Lote")
                .Width = 120
                .Caption = "lote".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = False
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 120
                .Caption = "FECHA VENC.".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = False
            End With


        End If

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






    Public Function StringTodouble(Valor As String) As Double
        Try
            Return CDbl(Valor)
        Catch ex As Exception
            Return 0
        End Try

    End Function



End Class