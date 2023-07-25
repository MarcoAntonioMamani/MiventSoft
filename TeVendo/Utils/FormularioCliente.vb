
Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class FormularioCliente
#Region "ATRIBUTOS"
    Public dtBuscador As DataTable
    Public nombreVista As String
    Public posX As Integer
    Public posY As Integer
    Public seleccionado As Boolean
    Public Columna As Integer = -1
    Public filaSelect As Janus.Windows.GridEX.GridEXRow

    Public IdCliente As Integer = 0
    Public NombreCliente As String = ""
    Public NuevoCliente As Boolean = False
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public listEstrucGrilla As List(Of Celda)
#End Region

#Region "METODOS PRIVADOS"
    Public Sub New(ByVal x As Integer, y As Integer, dt1 As DataTable, titulo As String, listEst As List(Of Celda))
        dtBuscador = dt1
        posX = x
        posY = y
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        lbTitulo.Text = titulo

        listEstrucGrilla = listEst

        seleccionado = False


        'grJBuscador.Row = grJBuscador.FilterRow.RowIndex
        'grJBuscador.Col = 1
        Columna = 2
        tbNombre.Focus()
        Me.Width = 1300
    End Sub
    Public Sub _prSeleccionar()
        'If (Columna >= 0) Then
        '    grJBuscador.Select()
        '    ''  grJBuscador.Focus()
        '    grJBuscador.MoveTo(grJBuscador.FilterRow)
        '    grJBuscador.Col = Columna
        'End If
    End Sub


    Private Sub _PMCargarBuscador()

        Dim anchoVentana As Integer = 0
        dtBuscador.Columns.Add("SELECCIONAR", GetType(Byte()))
        grJBuscador.DataSource = dtBuscador
        grJBuscador.RetrieveStructure()


        For i = 0 To dtBuscador.Columns.Count - 2
            With grJBuscador.RootTable.Columns(i)
                If listEstrucGrilla.Item(i).visible = True Then
                    .Caption = listEstrucGrilla.Item(i).titulo
                    .Width = listEstrucGrilla.Item(i).tamano
                    .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
                    .CellStyle.FontSize = 9
                    .MaxLines = 2
                    .WordWrap = True
                    Dim col As DataColumn = dtBuscador.Columns(i)
                    Dim tipo As Type = col.DataType
                    If tipo.ToString = "System.Int32" Or tipo.ToString = "System.Decimal" Then
                        .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                    End If
                    If listEstrucGrilla.Item(i).formato <> String.Empty Then
                        .FormatString = listEstrucGrilla.Item(i).formato
                    End If

                    anchoVentana = anchoVentana + listEstrucGrilla.Item(i).tamano
                Else
                    .Visible = False
                End If
            End With
        Next
        grJBuscador.RootTable.Columns("SELECCIONAR").Width = 150
        'Habilitar Filtradores
        With grJBuscador
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'diseño de la grilla
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
        End With

        CargarIconEstado()
        'adaptar el tamaño de la ventana

    End Sub
    Public Sub CargarIconEstado()
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.seleccionar, grJBuscador.RootTable.Columns("SELECCIONAR").Width - 15, 55)
        img.Save(Bin, Imaging.ImageFormat.Png)
        Dim dt As DataTable = CType(grJBuscador.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1


            CType(grJBuscador.DataSource, DataTable).Rows(i).Item("SELECCIONAR") = Bin.GetBuffer


        Next

    End Sub
#End Region

    Private Sub ModeloAyuda_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress
        e.KeyChar = e.KeyChar.ToString.ToUpper
        If (e.KeyChar = ChrW(Keys.Escape)) Then
            e.Handled = True
            Me.Close()
        End If
    End Sub

    Private Sub grJBuscador_KeyDown(sender As Object, e As KeyEventArgs) Handles grJBuscador.KeyDown

        If e.KeyData = Keys.Escape Then
            Me.Close()
        End If

        If e.KeyData = Keys.Enter Then
            filaSelect = grJBuscador.GetRow()
            seleccionado = True
            Me.Close()
        End If
    End Sub

    Private Sub FormularioAyuda_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        P_Global._prCargarComboGenerico(cbTipoDocumento, L_prLibreriaDetalleGeneral(8), "cnnum", "Codigo", "cndesc1", "TipoDocumento")
        P_Global._prCargarComboGenerico(cbPrecios, L_prListaCategoriasPrecios(), "Id", "Codigo", "Descripcion", "CategoriaPrecio")

        If (CType(cbTipoDocumento.DataSource, DataTable).Rows.Count > 0) Then
            cbTipoDocumento.SelectedIndex = 0
        End If
        If (CType(cbPrecios.DataSource, DataTable).Rows.Count > 0) Then
            cbPrecios.SelectedIndex = 0
        End If
        _PMCargarBuscador()
        tbNombre.Focus()


    End Sub

    Private Sub tbNombre_TextChanged(sender As Object, e As EventArgs) Handles tbNombre.TextChanged
        If (tbNombre.Text = String.Empty) Then
            grJBuscador.RootTable.RemoveFilter()




        Else

            grJBuscador.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grJBuscador.RootTable.Columns("NombreProveedor"), Janus.Windows.GridEX.ConditionOperator.Contains, tbNombre.Text))

        End If
    End Sub

    Private Sub tbNombre_KeyDown(sender As Object, e As KeyEventArgs) Handles tbNombre.KeyDown
        If e.KeyData = Keys.Escape Then
            Me.Close()
        End If

        If (e.KeyData = Keys.Down) Then
            grJBuscador.Focus()
        End If
    End Sub

    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True

        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "

        If tbNombreCliente.Text = String.Empty Then
            Mensaje = Mensaje + " Nombre Cliente"
            _ok = False
        End If

        If cbPrecios.SelectedIndex < 0 Then
            Mensaje = Mensaje + " Precios"
            _ok = False
        End If

        If cbTipoDocumento.SelectedIndex < 0 Then
            Mensaje = Mensaje + " Tipo Documento"
            _ok = False
        End If


        If tbNombreCliente.Text = String.Empty Then
            tbNombreCliente.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If
        If cbPrecios.SelectedIndex < 0 Then
            cbPrecios.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If
        If cbTipoDocumento.SelectedIndex < 0 Then
            cbTipoDocumento.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If
        Return _ok
    End Function

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        If (_PMOValidarCampos()) Then
            Dim dt As DataTable = InsertarClienteFormularioExterno("", tbNombreCliente.Text, cbTipoDocumento.Value, tbNroDocumento.Text, tbNombreCliente.Text, tbNroDocumento.Text, cbPrecios.Value, tbTelefono.Text)
            NuevoCliente = True
            IdCliente = dt.Rows(0).Item("Id")
            NombreCliente = dt.Rows(0).Item("NombreCliente")
            Me.Close()
        End If



    End Sub

    Private Sub FormularioCliente_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyData = Keys.Escape) Then
            Me.Close()

        End If
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Me.Close()
    End Sub

    Private Sub grJBuscador_Click(sender As Object, e As EventArgs) Handles grJBuscador.Click

        Try
            If (grJBuscador.RowCount >= 1 And grJBuscador.Row >= 0) Then
                If (grJBuscador.CurrentColumn.Index = grJBuscador.RootTable.Columns("Seleccionar").Index) Then


                    filaSelect = grJBuscador.GetRow()
                    seleccionado = True
                    Me.Close()
                End If

            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnSi_Paint(sender As Object, e As PaintEventArgs) Handles btnSi.Paint

    End Sub
End Class