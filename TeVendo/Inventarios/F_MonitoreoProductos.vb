Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
Imports System.IO
Public Class F_MonitoreoProductos
    Dim dtStock0 As DataTable
    Dim dtStockMinimo As DataTable
    Dim dtProductosVencidos As DataTable
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
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

    Private Sub tabProductosVencidos_Click(sender As Object, e As EventArgs) Handles btnProductosVencidos.Click
        _prCargarProductosStockMinimo()
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

    Private Sub btnProductosSinStock_Click(sender As Object, e As EventArgs) Handles btnProductosSinStock.Click
        _prCrearCarpetaReportes()
        Dim imgOk As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
        If (P_ExportarExcel(RutaGlobal + "\Reporte\Reporte Productos", "ProductosSinStock")) Then
            ToastNotification.Show(Me, "Los Datos Fueron Exportados Correctamente..!!!",
                                       imgOk, 2000,
                                       eToastGlowColor.Green,
                                       eToastPosition.BottomCenter)
        Else
            ToastNotification.Show(Me, "Hubo Problemas Al Exportar Los Datos..!!!",
                                      img, 2000,
                                       eToastGlowColor.Red,
                                       eToastPosition.BottomLeft)
        End If
    End Sub
    Public Function P_ExportarExcel(_ruta As String, Title As String) As Boolean
        Dim _ubicacion As String
        'Dim _directorio As New FolderBrowserDialog

        If (1 = 1) Then 'If(_directorio.ShowDialog = Windows.Forms.DialogResult.OK) Then
            '_ubicacion = _directorio.SelectedPath
            _ubicacion = _ruta
            Try
                Dim _stream As Stream
                Dim _escritor As StreamWriter
                Dim _fila As Integer = gr_ProductosStock0.GetRows.Length
                Dim _columna As Integer = gr_ProductosStock0.RootTable.Columns.Count
                Dim _archivo As String = _ubicacion & "\" + Title + "_" & Now.Date.Day &
                    "." & Now.Date.Month & "." & Now.Date.Year & "_" & Now.Hour & "." & Now.Minute & "." & Now.Second & ".csv"
                Dim _linea As String = ""
                Dim _filadata = 0, columndata As Int32 = 0
                File.Delete(_archivo)
                _stream = File.OpenWrite(_archivo)
                _escritor = New StreamWriter(_stream, System.Text.Encoding.UTF8)

                For Each _col As GridEXColumn In gr_ProductosStock0.RootTable.Columns
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

                For Each _fil As GridEXRow In gr_ProductosStock0.GetRows
                    For Each _col As GridEXColumn In gr_ProductosStock0.RootTable.Columns
                        If (_col.Visible) Then
                            Dim data As String = CStr(_fil.Cells(_col.Key).Value)
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
                    ef.Context = "El Archivo Ha sido Exportado en la Siguiente Ruta: " + _archivo + vbLf + "DESEA ABRIR EL ARCHIVO EXCEL?"
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

    Public Function P_ExportarExcelStockMinimo(_ruta As String, Title As String) As Boolean
        Dim _ubicacion As String
        'Dim _directorio As New FolderBrowserDialog

        If (1 = 1) Then 'If(_directorio.ShowDialog = Windows.Forms.DialogResult.OK) Then
            '_ubicacion = _directorio.SelectedPath
            _ubicacion = _ruta
            Try
                Dim _stream As Stream
                Dim _escritor As StreamWriter
                Dim _fila As Integer = grProductosStockMinimo.GetRows.Length
                Dim _columna As Integer = grProductosStockMinimo.RootTable.Columns.Count
                Dim _archivo As String = _ubicacion & "\" + Title + "_" & Now.Date.Day &
                    "." & Now.Date.Month & "." & Now.Date.Year & "_" & Now.Hour & "." & Now.Minute & "." & Now.Second & ".csv"
                Dim _linea As String = ""
                Dim _filadata = 0, columndata As Int32 = 0
                File.Delete(_archivo)
                _stream = File.OpenWrite(_archivo)
                _escritor = New StreamWriter(_stream, System.Text.Encoding.UTF8)

                For Each _col As GridEXColumn In grProductosStockMinimo.RootTable.Columns
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

                For Each _fil As GridEXRow In grProductosStockMinimo.GetRows
                    For Each _col As GridEXColumn In grProductosStockMinimo.RootTable.Columns
                        If (_col.Visible) Then
                            Dim data As String = CStr(_fil.Cells(_col.Key).Value)
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
                    ef.Context = "El Archivo Ha sido Exportado en la Siguiente Ruta: " + _archivo + vbLf + "DESEA ABRIR EL ARCHIVO EXCEL?"
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

    Public Function P_ExportarExcelProductosVencidos(_ruta As String, Title As String) As Boolean
        Dim _ubicacion As String
        'Dim _directorio As New FolderBrowserDialog

        If (1 = 1) Then 'If(_directorio.ShowDialog = Windows.Forms.DialogResult.OK) Then
            '_ubicacion = _directorio.SelectedPath
            _ubicacion = _ruta
            Try
                Dim _stream As Stream
                Dim _escritor As StreamWriter
                Dim _fila As Integer = gr_ProductosVencidos.GetRows.Length
                Dim _columna As Integer = gr_ProductosVencidos.RootTable.Columns.Count
                Dim _archivo As String = _ubicacion & "\" + Title + "_" & Now.Date.Day &
                    "." & Now.Date.Month & "." & Now.Date.Year & "_" & Now.Hour & "." & Now.Minute & "." & Now.Second & ".csv"
                Dim _linea As String = ""
                Dim _filadata = 0, columndata As Int32 = 0
                File.Delete(_archivo)
                _stream = File.OpenWrite(_archivo)
                _escritor = New StreamWriter(_stream, System.Text.Encoding.UTF8)

                For Each _col As GridEXColumn In gr_ProductosVencidos.RootTable.Columns
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

                For Each _fil As GridEXRow In gr_ProductosVencidos.GetRows
                    For Each _col As GridEXColumn In gr_ProductosVencidos.RootTable.Columns
                        If (_col.Visible) Then
                            Dim data As String = CStr(_fil.Cells(_col.Key).Value)
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
                    ef.Context = "El Archivo Ha sido Exportado en la Siguiente Ruta: " + _archivo + vbLf + "DESEA ABRIR EL ARCHIVO EXCEL?"
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

    Private Sub btnStockMinimo_Click(sender As Object, e As EventArgs) Handles btnStockMinimo.Click
        _prCrearCarpetaReportes()
        Dim imgOk As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
        If (P_ExportarExcelStockMinimo(RutaGlobal + "\Reporte\Reporte Productos", "ProductosStockMinimo")) Then
            ToastNotification.Show(Me, "Los Datos Fueron Exportados Correctamente..!!!",
                                       imgOk, 2000,
                                       eToastGlowColor.Green,
                                       eToastPosition.BottomCenter)
        Else
            ToastNotification.Show(Me, "Hubo Problemas Al Exportar Los Datos..!!!",
                                      img, 2000,
                                       eToastGlowColor.Red,
                                       eToastPosition.BottomLeft)
        End If
    End Sub

    Private Sub ButtonX3_Click(sender As Object, e As EventArgs) Handles ButtonX3.Click
        _prCrearCarpetaReportes()
        Dim imgOk As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
        If (P_ExportarExcelProductosVencidos(RutaGlobal + "\Reporte\Reporte Productos", "ProductosStockMinimo")) Then
            ToastNotification.Show(Me, "Los Datos Fueron Exportados Correctamente..!!!",
                                       imgOk, 2000,
                                       eToastGlowColor.Green,
                                       eToastPosition.BottomCenter)
        Else
            ToastNotification.Show(Me, "Hubo Problemas Al Exportar Los Datos..!!!",
                                      img, 2000,
                                       eToastGlowColor.Red,
                                       eToastPosition.BottomLeft)
        End If
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub
End Class