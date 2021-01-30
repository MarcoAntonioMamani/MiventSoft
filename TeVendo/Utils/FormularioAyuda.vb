Imports System.IO
Imports Janus.Windows.GridEX

Public Class FormularioAyuda

#Region "ATRIBUTOS"
    Public dtBuscador As DataTable
    Public nombreVista As String
    Public posX As Integer
    Public posY As Integer
    Public seleccionado As Boolean
    Public Columna As Integer = -1
    Public filaSelect As Janus.Windows.GridEX.GridEXRow

    Public listEstrucGrilla As List(Of Celda)
#End Region

    '''El Tab Index Sirve para establecer cual sera el primer campo de Focus


#Region "METODOS PRIVADOS"
    Public Sub New(ByVal x As Integer, y As Integer, dt1 As DataTable, titulo As String, listEst As List(Of Celda))
        dtBuscador = dt1
        posX = x
        posY = y
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.StartPosition = FormStartPosition.CenterScreen
        'Me.Location = New Point(posX, posY)
        lbTitulo.Text = titulo

        listEstrucGrilla = listEst

        seleccionado = False

        _PMCargarBuscador()
        'grJBuscador.Row = grJBuscador.FilterRow.RowIndex
        'grJBuscador.Col = 1
        Columna = 2
        tbNombre.Focus()
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
                    .MaxLines = 5
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


        ''adaptar el tamaño de la ventana
        'Me.Width = anchoVentana + 50

        CargarIconEstado()


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
        tbNombre.Focus()
    End Sub

    Private Sub tbNombre_TextChanged(sender As Object, e As EventArgs) Handles tbNombre.TextChanged
        If (tbNombre.Text = String.Empty) Then
            grJBuscador.RootTable.RemoveFilter()




        Else
            'grProducto.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grProducto.RootTable.Columns("NombreProducto"), Janus.Windows.GridEX.ConditionOperator.Contains, tbNombreProducto.Text))

            grJBuscador.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grJBuscador.RootTable.Columns("Nombre"), Janus.Windows.GridEX.ConditionOperator.Contains, tbNombre.Text))

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

    Private Sub grJBuscador_DoubleClick(sender As Object, e As EventArgs) Handles grJBuscador.DoubleClick
        If (grJBuscador.RowCount >= 1 And grJBuscador.Row >= 0) Then

            filaSelect = grJBuscador.GetRow()
                seleccionado = True
                Me.Close()


        End If
    End Sub
End Class