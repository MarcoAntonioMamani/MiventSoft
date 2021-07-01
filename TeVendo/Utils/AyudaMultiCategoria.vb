Imports System.IO
Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar

Public Class AyudaMultiCategoria

#Region "ATRIBUTOS"
    Public dtBuscador As DataTable
    Public nombreVista As String
    Public posX As Integer
    Public posY As Integer
    Public seleccionado As Boolean
    Public Columna As Integer = -1
    Public filaSelect As Janus.Windows.GridEX.GridEXRow

    Public listEstrucGrilla As List(Of Celda)

    Public SucursalSelected As Integer = 0
    Public CategoriaPrecioSelected As Integer = 0
    Public TableCategoria As DataTable
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



        grJBuscador.DataSource = dtBuscador
        grJBuscador.RetrieveStructure()


        For i = 0 To dtBuscador.Columns.Count - 1
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



        'Habilitar Filtradores
        With grJBuscador
            '.DefaultFilterRowComparison = FilterConditionOperator.Contains
            '.FilterMode = FilterMode.Automatic
            '.FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            'diseño de la grilla
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
        End With


        ''adaptar el tamaño de la ventana
        'Me.Width = anchoVentana + 50




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


        P_Global._prCargarComboGenerico(cbSucursal, L_fnGeneralSucursales(), "aanumi", "Codigo", "aabdes", "Sucursal")

        P_Global._prCargarComboGenerico(cbPrecio, L_fnListarCategoriaPrecioFilter(), "ygnumi", "Codigo", "Nombre", "CategoriaPrecio")

        cbSucursal.SelectedIndex = 0
        cbPrecio.SelectedIndex = 0


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




    Public Function TodosSeleccionados() As Boolean

        Dim dt As DataTable = CType(grJBuscador.DataSource, DataTable)

        Dim bandera As Boolean = True

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("seleccionado") = False) Then
                bandera = False
            End If
        Next
        Return bandera

    End Function

    Public Function AlMenosUnSeleccionado() As Boolean

        Dim dt As DataTable = CType(grJBuscador.DataSource, DataTable)

        Dim bandera As Boolean = False

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("seleccionado") = True) Then
                bandera = True
            End If
        Next
        Return bandera

    End Function

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        Dim dt As DataTable = CType(grJBuscador.DataSource, DataTable)
        If (TodosSeleccionados()) Then





            For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                dt.Rows(i).Item("seleccionado") = False

            Next
        Else
            For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                dt.Rows(i).Item("seleccionado") = True

            Next
        End If
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        If (AlMenosUnSeleccionado()) Then
            SucursalSelected = cbSucursal.Value
            CategoriaPrecioSelected = cbPrecio.Value
            TableCategoria = CType(grJBuscador.DataSource, DataTable)
            seleccionado = True
            Me.Close()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Debe Seleccionar Al Menos una Categoria", img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If
    End Sub

    Private Sub grJBuscador_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grJBuscador.EditingCell
        If (e.Column.Index = grJBuscador.RootTable.Columns("seleccionado").Index) Then
            e.Cancel = False
            Return
        End If

    End Sub
End Class