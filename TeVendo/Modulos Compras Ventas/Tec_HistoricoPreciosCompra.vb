
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.Controls
Public Class Tec_HistoricoPreciosCompra

    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Dim DtProducto As DataTable
    Dim dtCompras As DataTable
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 40, 40)

    Sub Iniciartodo()
        tbDesde.Value = Now.Date
        tbHasta.Value = Now.Date

    End Sub

    Public Function ObtenerProductos() As DataTable

        Dim dt As DataTable = DtProducto.Copy
        dt.Rows.Clear()

        For i As Integer = 0 To DtProducto.Rows.Count - 1 Step 1

            Dim ProductoId As Integer = DtProducto.Rows(i).Item("ProductoId")
            Dim Bandera As Boolean = False

            For j As Integer = 0 To dt.Rows.Count - 1 Step 1
                If (ProductoId = dt.Rows(j).Item("ProductoId")) Then
                    Bandera = True
                End If

            Next
            If (Bandera = False) Then
                dt.ImportRow(DtProducto.Rows(i))
            End If

        Next
        Return dt
    End Function
    Public Function ArmarHistorico() As DataTable
        '''Armando Columnas
        dtCompras = ListaComprasHistorico(tbDesde.Value.ToString("yyyy/MM/dd"), tbHasta.Value.ToString("yyyy/MM/dd"))

        If (dtCompras.Rows.Count <= 0) Then
            dtCompras.Columns.Add("TotalesCantidad")
            dtCompras.Columns.Add("TotalesBs")
            ToastNotification.Show(Me, "No Existe Compras Dentro De las Fechas Seleccionadas", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)


            Return dtCompras

        End If

        DtProducto = ListaComprasProductosHistorico(tbDesde.Value.ToString("yyyy/MM/dd"), tbHasta.Value.ToString("yyyy/MM/dd"))

        Dim dtTotales As DataTable = ListaComprasTotalesHistorico(tbDesde.Value.ToString("yyyy/MM/dd"), tbHasta.Value.ToString("yyyy/MM/dd"))

        Dim DtProductoUnico As DataTable = ObtenerProductos()

        For i As Integer = 0 To DtProductoUnico.Rows.Count - 1 Step 1

            dtCompras.Columns.Add(Str(DtProductoUnico.Rows(i).Item("ProductoId")).Trim + "Cantidad".Trim)

        Next
        dtCompras.Columns.Add("TotalesCantidad")
        For i As Integer = 0 To DtProductoUnico.Rows.Count - 1 Step 1

            dtCompras.Columns.Add(Str(DtProductoUnico.Rows(i).Item("ProductoId")).Trim + "Precio".Trim)

        Next
        dtCompras.Columns.Add("TotalesBs")
        ''''''''''''''''''''''''''

        '''''Rellenando Datos
        For i As Integer = 0 To dtCompras.Rows.Count - 1 Step 1
            Dim CompraId As Integer = dtCompras.Rows(i).Item("Id")
            Dim dtProductoFiltrado() As DataRow = DtProducto.Select("Id=" + Str(CompraId))
            If (dtProductoFiltrado.Count >= 1) Then
                For Each dr As DataRow In dtProductoFiltrado
                    Dim ProdId As Integer = dr.Item("ProductoId")
                    dtCompras(i).Item(Str(ProdId).Trim + "Cantidad".Trim) = dr.Item("Cantidad")
                    dtCompras(i).Item(Str(ProdId).Trim + "Precio".Trim) = dr.Item("PrecioCosto")



                Next
            End If

            Dim dtTotalesFiltrado() As DataRow = dtTotales.Select("Id=" + Str(CompraId))
            If (dtTotalesFiltrado.Count >= 1) Then
                For Each dr As DataRow In dtTotalesFiltrado
                    dtCompras(i).Item("TotalesCantidad") = dr.Item("Cantidad")
                    dtCompras(i).Item("TotalesBs") = dr.Item("TotalCompra")

                Next
            End If

        Next

        Return dtCompras
    End Function
    Private Sub _prCargarHistorico()



        Dim dt As DataTable = ArmarHistorico()
        grCompras.DataSource = dt
        grCompras.RetrieveStructure()
        grCompras.AlternatingColors = True

        With grCompras.RootTable.Columns("Id")
            .Width = 60
            .Caption = "Codigo"
            .Visible = True

        End With
        With grCompras.RootTable.Columns("FechaCompra")
            .Width = 90
            .Visible = True
            .Caption = "Fecha Compra"
            .FormatString = "dd/MM/yyyy"

        End With
        With grCompras.RootTable.Columns("NombreProveedor")
            .Width = 200
            .Visible = True
            .Caption = "Proveedor"
        End With
        With grCompras.RootTable.Columns("NroNotaCompra")
            .Width = 100
            .Visible = True
            .Caption = "Nota Compra"
        End With
        With grCompras.RootTable.Columns("NroNotaRecepcion")
            .Width = 100
            .Visible = True
            .Caption = "Nota Recepción"
        End With
        With grCompras.RootTable.Columns("TotalesCantidad")
            .Width = 100
            .FormatString = "0.00"
            .Caption = "Total Cantidad"
            .Visible = True
            .CellStyle.FontBold = TriState.True
            .CellStyle.BackColor = Color.Lime
            .AggregateFunction = AggregateFunction.Sum
        End With

        With grCompras.RootTable.Columns("TotalesBs")
            .Width = 100
            .FormatString = "0.00"
            .Caption = "Total Bs"
            .CellStyle.FontBold = TriState.True
            .CellStyle.BackColor = Color.Lime
            .Visible = True
            .AggregateFunction = AggregateFunction.Sum
        End With
        For i As Integer = 0 To dtCompras.Rows.Count - 1 Step 1
            Dim CompraId As Integer = dtCompras.Rows(i).Item("Id")
            Dim dtProductoFiltrado() As DataRow = DtProducto.Select("Id=" + Str(CompraId))
            If (dtProductoFiltrado.Count >= 1) Then
                For Each dr As DataRow In dtProductoFiltrado
                    Dim ProdId As Integer = dr.Item("ProductoId")
                    Dim NameColumnCantidad As String = Str(ProdId).Trim + "Cantidad".Trim
                    Dim NameColumnPrecio As String = Str(ProdId).Trim + "Precio".Trim


                    With grCompras.RootTable.Columns(NameColumnCantidad)
                        .Width = 120
                        .Visible = True
                        .Caption = dr.Item("NombreProducto")
                    End With
                    With grCompras.RootTable.Columns(NameColumnPrecio)
                        .Width = 120
                        .Visible = True
                        .Caption = dr.Item("NombreProducto")
                    End With

                Next
            End If


        Next

        ''TotalesCantidad Tota TotalesBs




        With grCompras
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

    End Sub
    Private Sub Tec_AdministrarAsignacionesPedidos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Iniciartodo()
        Me.Text = "Historico De Compras"
    End Sub

    Private Sub ButtonX5_Click(sender As Object, e As EventArgs) Handles ButtonX5.Click
        _prCargarHistorico()
    End Sub
    Private Sub _prCrearCarpetaReportes()
        Dim rutaDestino As String = RutaGlobal + "\Reporte\Reporte Productos\"

        If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Productos\") = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Reporte") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte")
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Productos")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Productos")

                End If
            End If
        End If
    End Sub

    Public Function P_ExportarExcel(_ruta As String) As Boolean
        Dim _ubicacion As String
        'Dim _directorio As New FolderBrowserDialog

        If (1 = 1) Then 'If(_directorio.ShowDialog = Windows.Forms.DialogResult.OK) Then
            '_ubicacion = _directorio.SelectedPath
            _ubicacion = _ruta
            Try
                Dim _stream As Stream
                Dim _escritor As StreamWriter
                Dim _fila As Integer = grCompras.GetRows.Length
                Dim _columna As Integer = grCompras.RootTable.Columns.Count
                Dim _archivo As String = _ubicacion & "\HistoricoPrecios_" & Now.Date.Day &
                    "." & Now.Date.Month & "." & Now.Date.Year & "_" & Now.Hour & "." & Now.Minute & "." & Now.Second & ".csv"
                Dim _linea As String = ""
                Dim _filadata = 0, columndata As Int32 = 0
                File.Delete(_archivo)
                _stream = File.OpenWrite(_archivo)
                _escritor = New StreamWriter(_stream, System.Text.Encoding.UTF8)

                For Each _col As GridEXColumn In grCompras.RootTable.Columns
                    If (_col.Visible) Then
                        _linea = _linea & _col.Caption & ";"
                    End If
                Next
                _linea = Mid(CStr(_linea), 1, _linea.Length - 1)
                _escritor.WriteLine(_linea)
                _linea = Nothing

                'Pbx_Precios.Visible = True
                'Pbx_Precios.Minimum = 1
                'Pbx_Precios.Maximum = Dgv_Precios.RowCount
                'Pbx_Precios.Value = 1

                For Each _fil As GridEXRow In grCompras.GetRows
                    For Each _col As GridEXColumn In grCompras.RootTable.Columns
                        If (_col.Visible) Then
                            Dim data As String = CStr(IIf(IsDBNull(_fil.Cells(_col.Key).Value), " ", _fil.Cells(_col.Key).Value))
                            data = data.Replace(";", ",")
                            _linea = _linea & data & ";"
                        End If
                    Next
                    _linea = Mid(CStr(_linea), 1, _linea.Length - 1)
                    _escritor.WriteLine(_linea)
                    _linea = Nothing
                    'Pbx_Precios.Value += 1
                Next
                _escritor.Close()
                'Pbx_Precios.Visible = False
                Try
                    Dim ef = New Efecto
                    ef._archivo = _archivo

                    ef.tipo = 1
                    ef.Context = "Su archivo ha sido Guardado en la ruta: " + _archivo + vbLf + "DESEA ABRIR EL ARCHIVO?"
                    ef.Header = "PREGUNTA"
                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then
                        Process.Start(_archivo)
                    End If

                    'If (MessageBox.Show("Su archivo ha sido Guardado en la ruta: " + _archivo + vbLf + "DESEA ABRIR EL ARCHIVO?", "PREGUNTA", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes) Then
                    '    Process.Start(_archivo)
                    'End If
                    Return True
                Catch ex As Exception
                    MsgBox(ex.Message)
                    Return False
                End Try
            Catch ex As Exception
                MsgBox(ex.Message)
                Return False
            End Try
        End If
        Return False
    End Function
    Private Sub ButtonX6_Click(sender As Object, e As EventArgs) Handles ButtonX6.Click
        If (Not IsNothing(grCompras) And Not IsNothing(grCompras.DataSource)) Then

            _prCrearCarpetaReportes()
            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            If (P_ExportarExcel(RutaGlobal + "\Reporte\Reporte Productos")) Then
                ToastNotification.Show(Me, "EXPORTACIÓN DE REPORTE HISTORICO DE COMPRAS EXITOSA..!!!",
                                           img, 2000,
                                           eToastGlowColor.Green,
                                           eToastPosition.BottomCenter)
            Else
                ToastNotification.Show(Me, "FALLO AL EXPORTACIÓN DE LISTA DE PRODUCTOS..!!!",
                                           img, 2000,
                                           eToastGlowColor.Red,
                                           eToastPosition.BottomLeft)
            End If
        Else
            ToastNotification.Show(Me, "NO HAY DATOS PARA EXPORTAR..!!!",
                                   My.Resources.INFORMATION, 2000,
                                   eToastGlowColor.Blue,
                                   eToastPosition.TopCenter)

        End If
    End Sub
End Class