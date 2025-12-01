
Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class FrmVentasPorCategoria
    Public dtProductos As DataTable
    Public dtDetalle As DataTable
    Public Lote As Boolean

    Dim RutaGlobal As String = gs_CarpetaRaiz
    Public TipoMovimientoId As Integer
    Public DepositoId As Integer

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)

    Dim Inicial As Integer = 0
    Public Sub IniciarTodod()

        Dim dtCategorias As DataTable = L_prListaCategorias()
        dtCategorias.Rows.Add(-1, "TODOS")
        P_Global._prCargarComboGenerico(cbCategoria, dtCategorias, "Id", "Codigo", "NombreCategoria", "Categoria")
        Dim dt As DataTable = L_prListaPersonalCB()
        dt.Rows.Add(-1, "TODOS")
        P_Global._prCargarComboGenerico(cbVendedor, dt, "Id", "Codigo", "NombrePersonal", "NombrePersonal")

        Dim dtSubcategoria As DataTable = L_prListarSubcategoriaAsignados()
        dtSubcategoria.Rows.Add(-1, "TODOS")
        P_Global._prCargarComboGenerico(cbSubCategoria, dtSubcategoria, "Id", "Codigo", "Descripcion", "Descripcion")



        cbVendedor.Value = -1
        cbCategoria.Value = -1
        cbSubCategoria.Value = -1
        _habilitarFocus()

        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date

        Inicial = 1
        _prCargarProductos(cbCategoria.Value)
        cbCategoria.Focus()
    End Sub


    Public Sub New()
        InitializeComponent()

    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(cbCategoria, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnConfirmarSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        End With
    End Sub
    Private Sub Tec_DespachoDetalle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarTodod()
        cbCategoria.Focus()
    End Sub
    Private Sub tbProducto_TextChanged(sender As Object, e As EventArgs)

        Try

            Dim dtProductoCopy As DataTable
            dtProductoCopy = dtProductos.Copy
            dtProductoCopy.Rows.Clear()
            Dim dt As DataTable = dtProductos.Copy

            Dim charSequence As String
            charSequence = ""
            If (charSequence.Trim <> String.Empty) Then
                Dim cantidad As Integer = 12
                Dim cont As Integer = 12

                'Split con array de delimitadores
                Dim delimitadores() As String = {" ", ".", ",", ";", "-"}
                Dim vectoraux() As String
                vectoraux = charSequence.Split(delimitadores, StringSplitOptions.None)

                'mostrar resultado
                'For Each item As String In vectoraux


                '    Console.WriteLine("'{0}'", item)
                'Next
                Dim cant As Integer = vectoraux.Length
                'p.Id , p.CodigoExterno, p.NombreProducto, p.DescripcionProducto, Sum(stock.Cantidad) as stock 
                For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                    Dim nombre As String = dt.Rows(i).Item("NombrePersonal").ToString.ToUpper +
                    " " + dt.Rows(i).Item("NombreProducto").ToString.ToUpper +
                    " " + dt.Rows(i).Item("NombreCategoria").ToString.ToUpper +
                    " " + dt.Rows(i).Item("VentaId").ToString.ToUpper
                    Select Case cant
                        Case 1

                            If (nombre.Trim.Contains(vectoraux(0))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If

                        Case 2
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 3
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 4
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 5
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 6
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If

                        Case 7

                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 8
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 9
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 10
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If

                        Case 11
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If

                        Case 12
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If


                        Case 13
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 14
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 15
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 16
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 17
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If
                        Case 18
                            If (nombre.Trim.Contains(vectoraux(0)) And nombre.Trim.Contains(vectoraux(1)) And nombre.Trim.Contains(vectoraux(2)) And nombre.Trim.Contains(vectoraux(3)) And nombre.Trim.Contains(vectoraux(4)) And nombre.Trim.Contains(vectoraux(5)) And nombre.Trim.Contains(vectoraux(6)) And nombre.Trim.Contains(vectoraux(7)) And nombre.Trim.Contains(vectoraux(8)) And nombre.Trim.Contains(vectoraux(9)) And nombre.Trim.Contains(vectoraux(10)) And nombre.Trim.Contains(vectoraux(11)) And nombre.Trim.Contains(vectoraux(12)) And nombre.Trim.Contains(vectoraux(13)) And nombre.Trim.Contains(vectoraux(14)) And nombre.Trim.Contains(vectoraux(15)) And nombre.Trim.Contains(vectoraux(16)) And nombre.Trim.Contains(vectoraux(17))) Then
                                dtProductoCopy.ImportRow(dt.Rows(i))
                                cont += 1
                            End If



                    End Select

                Next
                grProducto.DataSource = dtProductoCopy.Copy
            Else
                grProducto.DataSource = dtProductos.Copy
            End If

        Catch ex As Exception

        End Try



    End Sub

    Private Sub tbProducto_KeyDown(sender As Object, e As KeyEventArgs)

        If e.KeyData = Keys.Down Then
            grProducto.Focus()
        End If
        If e.KeyData = Keys.Enter Then
            grProducto.Focus()

        End If
    End Sub

    Private Sub _prCargarProductos(CategoriaPrecio As Integer)
        Dim dt As New DataTable



        dt = L_prListarProductosPorCategoria(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"),
                                             cbVendedor.Value, cbCategoria.Value, cbSubCategoria.Value)  ''1=Almacen
        dtProductos = dt

        'p.Id , p.CodigoExterno, p.NombreProducto, p.DescripcionProducto, Sum(stock.Cantidad) as stock 

        grProducto.DataSource = dt
        grProducto.RetrieveStructure()
        grProducto.AlternatingColors = True

        With grProducto.RootTable.Columns("NombrePersonal")
            .Width = 200
            .Caption = "Personal"
            .Visible = True
            .MaxLines = 2
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("NombreProducto")
            .Width = 300
            .Caption = "PRODUCTOS"
            .Visible = False
            .MaxLines = 2
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("Subcategoria")
            .Width = 100
            .Caption = "Subcategoria"
            .Visible = True
            .MaxLines = 2
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("NombreCategoria")
            .Width = 150
            .Caption = "CATEGORIA"
            .Visible = True
            .MaxLines = 2
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With

        With grProducto.RootTable.Columns("FechaVenta")
            .Width = 100
            .Caption = "FechaVenta"
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With
        ''NombreCategoria



        With grProducto.RootTable.Columns("VentaId")
            .Width = 70
            .Visible = True
            .FormatString = "0"
            .Caption = "Cod. Venta"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("ProductoId")
            .Width = 70
            .Visible = False
            .FormatString = "0"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Caption = "Cod. Producto"
            .MaxLines = 2
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("Cantidad")
            .Width = 100
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Caption = "Cantidad"
            .AggregateFunction = AggregateFunction.Sum
            .MaxLines = 2
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("MontoDescuento")
            .Width = 100
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Caption = "MontoDescuento"
            .MaxLines = 2
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("Total")
            .Width = 100
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .CellStyle.BackColor = Color.SpringGreen
            .CellStyle.FontBold = TriState.True
            .Caption = "Total"
            .AggregateFunction = AggregateFunction.Sum
            .MaxLines = 2
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("Precio")
            .Width = 100
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Precio"
            .MaxLines = 2
            .WordWrap = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .TextAlignment = TextAlignment.Far
            .CellStyle.FontSize = 11
        End With
        With grProducto
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            .TotalRow = InheritableBoolean.True
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub

    Private Sub btnConfirmarSalir_Click(sender As Object, e As EventArgs) Handles btnConfirmarSalir.Click
        _prCargarProductos(cbCategoria.Value)


    End Sub

    Private Sub btnProductos_Click(sender As Object, e As EventArgs) Handles btnProductos.Click
        Dim _dt As New DataTable
        _dt = L_prListarProductosPorCategoria(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"),
                                             cbVendedor.Value, cbCategoria.Value, cbSubCategoria.Value)
        If (IsNothing(_dt) Or _dt.Rows.Count = 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            Return

        End If


        If (_dt.Rows.Count > 0) Then


            If Not IsNothing(P_Global.Visualizador) Then
                P_Global.Visualizador.Close()
            End If


            P_Global.Visualizador = New Visualizador

            Dim objrep As New ProductosXCategoriaRep

            objrep.SetDataSource(_dt)
            Dim fechaI As String = cbFechaDesde.Value.ToString("yyyy/MM/dd")
            Dim fechaF As String = cbFechaHasta.Value.ToString("yyyy/MM/dd")
            objrep.SetParameterValue("FechaDesde", fechaI)
            objrep.SetParameterValue("FechaHasta", fechaF)
            objrep.SetParameterValue("Usuario", L_Usuario)
            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.CrGeneral.Zoom(90)
            P_Global.Visualizador.Show() 'Comentar
            ''P_Global.Visualizador.BringToFront() 'Comentar





        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar. con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
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
        If (P_ExportarExcel(RutaGlobal + "\Reporte\Reporte Productos", "ProductosStockGeneral")) Then
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
                Dim _fila As Integer = grProducto.GetRows.Length
                Dim _columna As Integer = grProducto.RootTable.Columns.Count
                Dim _archivo As String = _ubicacion & "\" + Title + "_" & Now.Date.Day &
                    "." & Now.Date.Month & "." & Now.Date.Year & "_" & Now.Hour & "." & Now.Minute & "." & Now.Second & ".csv"
                Dim _linea As String = ""
                Dim _filadata = 0, columndata As Int32 = 0
                File.Delete(_archivo)
                _stream = File.OpenWrite(_archivo)
                _escritor = New StreamWriter(_stream, System.Text.Encoding.UTF8)

                For Each _col As GridEXColumn In grProducto.RootTable.Columns
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

                For Each _fil As GridEXRow In grProducto.GetRows
                    For Each _col As GridEXColumn In grProducto.RootTable.Columns
                        If (_col.Visible) Then
                            ' Utiliza Convert.ToString para manejar correctamente DBNull.
                            Dim data As String = Convert.ToString(_fil.Cells(_col.Key).Value)
                            data = data.Replace(";", ",") ' Reemplazar punto y coma para mantener el formato CSV.
                            data = data.Replace(vbCr, "").Replace(vbLf, "") ' Eliminar caracteres de salto de línea.
                            _linea = _linea & data & ";"
                        End If
                    Next
                    _linea = Mid(CStr(_linea), 1, _linea.Length - 1)
                    _escritor.WriteLine(_linea)
                    _linea = Nothing
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

    Private Sub cbPrecio_ValueChanged(sender As Object, e As EventArgs) Handles cbCategoria.ValueChanged
        ''_prCargarProductos(cbCategoria.Value)
        If (Inicial <> 0) Then
            _prCargarProductos(cbCategoria.Value)
            '' tbProducto.Text = cbCategoria.Text
        End If
    End Sub

    Private Sub cbVendedor_ValueChanged(sender As Object, e As EventArgs) Handles cbVendedor.ValueChanged
        ''_prCargarProductos(cbVendedor.Value)
        If (Inicial <> 0) Then
            _prCargarProductos(cbVendedor.Value)
            '' tbProducto.Text = cbVendedor.Text
        End If

    End Sub

    Private Sub cbFechaDesde_ValueChanged(sender As Object, e As EventArgs) Handles cbFechaDesde.ValueChanged
        If (Inicial <> 0) Then
            _prCargarProductos(cbCategoria.Value)
        End If

    End Sub

    Private Sub cbFechaHasta_ValueChanged(sender As Object, e As EventArgs) Handles cbFechaHasta.ValueChanged
        If (Inicial <> 0) Then
            _prCargarProductos(cbCategoria.Value)
        End If
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        Dim _dt As New DataTable
        _dt = L_prListarProductosPorCategoriaTotal(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"),
                                             cbVendedor.Value, cbCategoria.Value)
        If (IsNothing(_dt) Or _dt.Rows.Count = 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            Return

        End If


        If (_dt.Rows.Count > 0) Then


            If Not IsNothing(P_Global.Visualizador) Then
                P_Global.Visualizador.Close()
            End If


            P_Global.Visualizador = New Visualizador

            Dim objrep As New ReporteGeneralPorCategoria

            objrep.SetDataSource(_dt)
            Dim fechaI As String = cbFechaDesde.Value.ToString("yyyy/MM/dd")
            Dim fechaF As String = cbFechaHasta.Value.ToString("yyyy/MM/dd")
            objrep.SetParameterValue("FechaDesde", fechaI)
            objrep.SetParameterValue("FechaHasta", fechaF)
            objrep.SetParameterValue("Usuario", L_Usuario)
            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.CrGeneral.Zoom(90)
            P_Global.Visualizador.Show() 'Comentar
            ''P_Global.Visualizador.BringToFront() 'Comentar





        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar. con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub

    Private Sub cbSubCategoria_ValueChanged(sender As Object, e As EventArgs) Handles cbSubCategoria.ValueChanged
        If (Inicial <> 0) Then
            _prCargarProductos(cbCategoria.Value)
            '' tbProducto.Text = cbCategoria.Text
        End If
    End Sub
End Class