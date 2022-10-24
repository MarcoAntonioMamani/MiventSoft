Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class frm_comision
    Public dtProductos As DataTable
    Public dtDetalle As DataTable
    Public Lote As Boolean
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Public TipoMovimientoId As Integer
    Public DepositoId As Integer

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)

    Public Sub IniciarTodod()
        P_Global._prCargarComboGenerico(cbPersonal, ListarPersonalCombo(), "id", "Codigo", "Nombre", "Personal")
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date

        _habilitarFocus()

    End Sub


    Public Sub New()
        InitializeComponent()

    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(btnCargarDatos, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        End With
    End Sub
    Private Sub Tec_DespachoDetalle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarTodod()

    End Sub


    Private Sub _prCargarComision()
        Dim dt As New DataTable



        dt = L_prListarVentasComision(cbPersonal.Value, cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))  ''1=Almacen
        dtProductos = dt

        ' AlmacenId   NombreAlmacen	PersonalId	NombrePersonal	ClienteId	NombreCliente	Documento	FechaVenta	TotalVenta	Img	PorcentajeComision	comision

        grProducto.DataSource = dt
        grProducto.RetrieveStructure()
        grProducto.AlternatingColors = True
        With grProducto.RootTable.Columns("AlmacenId")
            .Width = 100
            .Caption = "Id"
            .Visible = False


        End With

        With grProducto.RootTable.Columns("NombreAlmacen")
            .Width = 60
            .Caption = "Sucursal"
            .Visible = True
            .MaxLines = 3
            .WordWrap = True

        End With
        With grProducto.RootTable.Columns("PersonalId")
            .Width = 100
            .Visible = False
        End With

        With grProducto.RootTable.Columns("NombrePersonal")
            .Width = 100
            .Caption = "PERSONAL"
            .Visible = True
            .MaxLines = 2
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("ClienteId")
            .Width = 100
            .Visible = False
        End With
        ' AlmacenId   NombreAlmacen	PersonalId	NombrePersonal	ClienteId	NombreCliente	
        ' Documento	FechaVenta	TotalVenta	Img	PorcentajeComision	comision

        With grProducto.RootTable.Columns("NombreCliente")
            .Width = 100
            .Caption = "CLIENTE"
            .Visible = True
            .MaxLines = 2
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("Documento")
            .Width = 60
            .Caption = "Documento"
            .Visible = True
            .MaxLines = 2
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("FechaVenta")
            .Width = 60
            .Caption = "Fecha"
            .Visible = True
            .MaxLines = 2
            .WordWrap = True
            .FormatString = "dd/MM/yyyy"
        End With
        ''TotalVenta	Img	PorcentajeComision	comision



        With grProducto.RootTable.Columns("TotalVenta")
            .Width = 150
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total"
            .MaxLines = 2
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("Img")
            .Width = 100
            .Visible = False
        End With

        With grProducto.RootTable.Columns("PorcentajeComision")
            .Width = 110
            .Visible = True
            .FormatString = "0.00"
            .Caption = "% Comisión"
            .MaxLines = 2
            .WordWrap = True
        End With
        With grProducto.RootTable.Columns("comision")
            .Width = 150
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Comisión Bs"
            .MaxLines = 2
            .WordWrap = True
            .CellStyle.BackColor = Color.SpringGreen
            .CellStyle.FontBold = TriState.True
            .TextAlignment = TextAlignment.Far
        End With
        With grProducto
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With

    End Sub

    Private Sub btnConfirmarSalir_Click(sender As Object, e As EventArgs) Handles btnCargarDatos.Click
        _prCargarComision()


    End Sub

    Private Sub btnProductos_Click(sender As Object, e As EventArgs) Handles btnProductos.Click
        Dim _dt As New DataTable
        _dt = L_prListarVentasComision(cbPersonal.Value, cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))
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

            Dim objrep As New Reporte_StockGeneral

            objrep.SetDataSource(_dt)
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
        Dim rutaDestino As String = RutaGlobal + "\Reporte\Reporte Comision\"

        If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Comision\") = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Reporte") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte")
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Comision") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Comision")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte Comision") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte Comision")

                End If
            End If
        End If
    End Sub
    Private Sub btnProductosSinStock_Click(sender As Object, e As EventArgs) Handles btnProductosSinStock.Click
        _prCrearCarpetaReportes()
        Dim imgOk As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
        If (P_ExportarExcel(RutaGlobal + "\Reporte\Reporte Comision", "Comision")) Then
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

    Private Sub cbPrecio_ValueChanged(sender As Object, e As EventArgs) Handles cbPersonal.ValueChanged
        _prCargarComision()
    End Sub
End Class