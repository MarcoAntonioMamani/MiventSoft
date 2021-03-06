﻿Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls

Public Class Tec_Precios

#Region "Variables Globales"
    Dim precio As DataTable
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
#End Region
#Region "MEtodos Privados"
    Private Sub _IniciarTodo()

        Me.WindowState = FormWindowState.Maximized
        _prCargarTablaCategorias()
        _prCargarComboLibreria(cbAlmacen)

        _prAsignarPermisos()
        Me.Text = "GESTIONAR PRECIOS"
        Dim blah As New Bitmap(New Bitmap(My.Resources.precio), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
    End Sub
    Private Sub _prAsignarPermisos()

        'Dim dtRolUsu As DataTable = L_prRolDetalleGeneral(gi_userRol, _nameButton)

        'Dim show As Boolean = dtRolUsu.Rows(0).Item("ycshow")
        'Dim add As Boolean = dtRolUsu.Rows(0).Item("ycadd")
        'Dim modif As Boolean = dtRolUsu.Rows(0).Item("ycmod")
        'Dim del As Boolean = dtRolUsu.Rows(0).Item("ycdel")

        'If add = False Then
        '    btnNuevo.Visible = False
        'End If
        'If modif = False Then
        '    btnModificar.Visible = False
        'End If
        'If del = False Then
        '    btnEliminar.Visible = False
        'End If
    End Sub
    Public Sub _prCargarPrecios()
        precio = L_fnListarProductosConPrecios(cbAlmacen.Value)
    End Sub
    Public Sub _prCargarTablaPrecios(bandera As Boolean) ''Bandera = true si es que haiq cargar denuevo la tabla de Precio Bandera =false si solo cargar datos al Janus con el precio antepuesto
        If (cbAlmacen.SelectedIndex >= 0) Then
            Dim productos As DataTable = L_fnListarProductos()
            If (bandera = True) Then
                _prCargarPrecios()
            End If
            Dim categorias As DataTable = L_fnListarCategorias()

            For Each fila As DataRow In categorias.Rows
                productos.Columns.Add("estado_" + fila.Item("ygcod").ToString.Trim)
                productos.Columns.Add(fila.Item("ygnumi").ToString.Trim)
            Next
            For j As Integer = 0 To productos.Rows.Count - 1 Step 1
                Dim idprod As Integer = productos.Rows(j).Item("yfnumi")
                Dim result As DataRow() = precio.Select("yhprod=" + Str(idprod))
                For i As Integer = 0 To result.Length - 1 Step 1
                    Dim rowIndex As Integer = precio.Rows.IndexOf(result(i))
                    Dim columnprecio As String = result(i).Item("yhcatpre")
                    Dim columnestado As String = "estado_" + result(i).Item("ygcod")
                    productos.Rows(j).Item(columnprecio) = result(i).Item("yhprecio")
                    productos.Rows(j).Item(columnestado) = Str(result(i).Item("estado")) + "_" + Str(rowIndex).Trim

                Next
            Next

            grprecio.BoundMode = Janus.Data.BoundMode.Bound
            grprecio.DataSource = productos
            grprecio.RetrieveStructure()
            For Each fc As DataRow In categorias.Rows

                Dim columnprecio As String = fc.Item("ygnumi").ToString.Trim
                Dim columnestado As String = "estado_" + fc.Item("ygcod").ToString.Trim
                With grprecio.RootTable.Columns(columnestado)
                    .Caption = ""
                    .Width = 150
                    .Visible = False
                    .FormatString = "0"
                End With

                With grprecio.RootTable.Columns(columnprecio)
                    .Caption = fc.Item("ygdesc").ToString
                    .Width = 90
                    .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
                    .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                    .Visible = True
                    .FormatString = "0.00"
                End With

            Next

            'a.yfcprod ,a.yfnumi ,a.yfcdprod1,gr3.ycdes3 as Laboratorio,gr4.ycdes3 as Presentacion 
            With grprecio.RootTable.Columns("yfcprod")
                .Caption = "Cod"
                .Width = 40
                .Visible = True
            End With
            With grprecio.RootTable.Columns("yfnumi")
                .Caption = "Cod P"
                .Width = 70
                .Visible = False
            End With
            With grprecio.RootTable.Columns("yfcdprod1")
                .Caption = "Producto"
                .Width = 200
                .Visible = True
            End With
            With grprecio.RootTable.Columns("NombreCategoria")
                .Caption = "Categoria"
                .Width = 120
                .Visible = True
            End With
            With grprecio.RootTable.Columns("proveedor")
                .Caption = "Proveedor"
                .Width = 120
                .Visible = True
            End With
            'Habilitar Filtradores
            With grprecio
                .GroupByBoxVisible = False
                '.FilterRowFormatStyle.BackColor = Color.Blue
                .DefaultFilterRowComparison = FilterConditionOperator.Contains
                '.FilterMode = FilterMode.Automatic
                .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
                .FilterMode = FilterMode.Automatic
                'Diseño de la tabla
                .VisualStyle = VisualStyle.Office2007
                .SelectionMode = SelectionMode.SingleSelection
                .AlternatingColors = True
            End With
        End If
    End Sub
    Private Sub _prCargarComboLibreria(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnGeneralSucursales()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("aanumi").Width = 70
            .DropDownList.Columns("aanumi").Caption = "COD"
            .DropDownList.Columns.Add("aabdes").Width = mCombo.Size.Width
            .DropDownList.Columns("aabdes").Caption = "DESCRIPCION"
            .ValueMember = "aanumi"
            .DisplayMember = "aabdes"
            .DataSource = dt
            .Refresh()
        End With
        If (CType(cbAlmacen.DataSource, DataTable).Rows.Count > 0) Then
            cbAlmacen.SelectedIndex = 0
        End If
    End Sub
    Private Sub _prInhabiliitar()

        GPanelAddCategoria.Visible = False
        btnModificar.Visible = True
        btnGrabar.Visible = False
        btnCategoria.Visible = True
        PanelCategoria.Visible = False
        _prCargarTablaPrecios(True)

        grcategoria.ContextMenuStrip = Nothing
    End Sub
    Private Sub _prhabilitar()
        grcategoria.ContextMenuStrip = msModulos

        tbDescripcion.Focus()
        btnGrabar.Visible = True
    End Sub

    Private Sub _prCargarTablaCategorias()
        Dim dt As New DataTable
        dt = L_fnGeneralCategorias()
        grcategoria.DataSource = dt
        grcategoria.RetrieveStructure()
        grcategoria.AlternatingColors = True

        'dar formato a las columnas
        'a.ygnumi, a.ygcod, a.ygdesc, a.ygpcv, a.ygfact, a.yghact, a.yguact
        With grcategoria.RootTable.Columns("ygnumi")
            .Width = 70
            .Caption = "Cod"
            .Visible = False

        End With

        With grcategoria.RootTable.Columns("ygcod")
            .Width = 40
            .Visible = True
            .Caption = "Cod"
        End With

        With grcategoria.RootTable.Columns("ygdesc")
            .Caption = "Descripcion"
            .Width = 150
            .Visible = True


        End With
        With grcategoria.RootTable.Columns("ygpcv")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False



        End With

        With grcategoria.RootTable.Columns("estado")
            .Width = 80
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "Tipo"

        End With




        With grcategoria
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007

        End With


    End Sub

    Public Function _fnAccesible()
        Return btnGrabar.Visible = True Or PanelCategoria.Visible = True
    End Function

    Private Function _FSiguienteLetra(palabra As String) As String
        Dim alfabeto As New List(Of String)
        alfabeto.Add("A")
        alfabeto.Add("B")
        alfabeto.Add("C")
        alfabeto.Add("D")
        alfabeto.Add("E")
        alfabeto.Add("F")
        alfabeto.Add("G")
        alfabeto.Add("H")
        alfabeto.Add("I")
        alfabeto.Add("J")
        alfabeto.Add("K")
        alfabeto.Add("L")
        alfabeto.Add("M")
        alfabeto.Add("N")
        alfabeto.Add("O")
        alfabeto.Add("P")
        alfabeto.Add("Q")
        alfabeto.Add("R")
        alfabeto.Add("S")
        alfabeto.Add("T")
        alfabeto.Add("U")
        alfabeto.Add("V")
        alfabeto.Add("W")
        alfabeto.Add("X")
        alfabeto.Add("Y")
        alfabeto.Add("Z")
        Dim letra As String
        If palabra.Length = 1 Then
            letra = palabra(0)
            '26 letras en el alphabeto
            If alfabeto.IndexOf(letra) = 25 Then
                palabra = "AA"
            Else
                palabra = alfabeto(alfabeto.IndexOf(letra) + 1)
            End If
        Else
            letra = palabra(1)
            If alfabeto.IndexOf(letra) = 25 Then
                palabra = ""
            Else
                palabra = palabra(0) + alfabeto(alfabeto.IndexOf(letra) + 1)
            End If
        End If
        Return palabra
    End Function


    Public Sub _prLimpiar()
        tbDescripcion.Clear()
        swEstado.Value = True
        tbDescripcion.Focus()
    End Sub
    Public Function _prValidarCategoria() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()

        If tbDescripcion.Text = String.Empty Then
            tbDescripcion.BackColor = Color.Red
            MEP.SetError(tbDescripcion, "Ingrese una Descripcion!")
            _ok = False
            AddHandler tbDescripcion.KeyDown, AddressOf TextBox_KeyDown
        Else
            tbDescripcion.BackColor = Color.White
            MEP.SetError(tbDescripcion, "")
        End If



        MHighlighterFocus.UpdateHighlights()
        Return _ok
    End Function

    Private Sub _prGrabarCategorias()
        If (_prValidarCategoria()) Then
            Dim letra As String
            If (swEstado.Value = True) Then
                letra = _fnObtenerSiguienteString()
            Else
                letra = _fnObtenerSiguienteInt()

            End If

            Dim grabar As Boolean = L_fnGrabarCategorias("", letra, tbDescripcion.Text, IIf(swEstado.Value = True, 1, 0))
            If (grabar) Then
                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                ToastNotification.Show(Me, "Categoria Grabado con Exito.",
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter
                                          )
                _prLimpiar()

                _prCargarTablaCategorias()

                _prCargarDatosTablaPrecios()


            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, "La categoria no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If

    End Sub

    Public Sub _prCargarDatosTablaPrecios()
        Dim result() As DataRow = precio.Select("estado > 1")
        _prCargarPrecios()
        For i As Integer = 0 To result.Length - 1 Step 1
            Dim r As DataRow = result.GetValue(i)
            Dim dr() As DataRow
            dr = precio.Select("yhprod=" + Str(r.Item("yhprod")) + "and yhcatpre=" + Str(r.Item("yhcatpre")))
            If dr Is Nothing Then
                'No se encontró la fila. Crear nueva fila

            Else
                'Fila encontrada
                If (dr.Length > 0) Then
                    dr.GetValue(0).Item("yhprecio") = r.Item("yhprecio")
                    dr.GetValue(0).Item("estado") = r.Item("estado")
                End If


            End If

        Next
        _prCargarTablaPrecios(False)
    End Sub

    Public Function _fnObtenerSiguienteString() As String

        If (grcategoria.RowCount > 0) Then
            Dim b As Boolean = False
            Dim length As Integer = grcategoria.RowCount - 1
            While (length >= 0 And b = False)
                Dim data As String = CType(grcategoria.DataSource, DataTable).Rows(length).Item("ygcod")
                If (Not IsNumeric(data)) Then
                    b = True
                Else
                    length -= 1
                End If
            End While
            If (b = False) Then
                Return _FSiguienteLetra(" ")
            Else
                Return _FSiguienteLetra(CType(grcategoria.DataSource, DataTable).Rows(length).Item("ygcod"))

            End If
        Else
            Return _FSiguienteLetra(" ")

        End If

    End Function

    Public Function _fnObtenerSiguienteInt() As String
        If (grcategoria.RowCount > 0) Then
            Dim b As Boolean = False
            Dim length As Integer = grcategoria.RowCount - 1
            While (length >= 0 And b = False)
                Dim data As String = CType(grcategoria.DataSource, DataTable).Rows(length).Item("ygcod")
                If (IsNumeric(data)) Then
                    b = True
                Else
                    length -= 1
                End If
            End While
            If (b = False) Then
                Return _fnSiguienteNumero(0)
            Else
                Return _fnSiguienteNumero(CType(grcategoria.DataSource, DataTable).Rows(length).Item("ygcod"))

            End If
        Else
            Return _fnSiguienteNumero(0)

        End If

    End Function
    Public Function _fnSiguienteNumero(num As Integer)
        Return num + 1
    End Function

#End Region


#Region "MEtodoso Formulario"
    Private Sub F0_Precios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _IniciarTodo()
        _prInhabiliitar()
    End Sub
    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        PanelCategoria.Visible = False
        _prhabilitar()
        btnCategoria.Visible = False
        btnModificar.Visible = False

    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        If (_fnAccesible()) Then
            _prInhabiliitar()
        Else
            _TabControl.SelectedTab = _modulo
            _tab.Close()
            Me.Close()
        End If
    End Sub
    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        _prGrabarCategorias()
        btnGrabar.PerformClick()


    End Sub
    Private Sub TextBox_KeyDown(sender As Object, e As KeyEventArgs)
        Dim tb As TextBoxX = CType(sender, TextBoxX)
        If tb.Text = String.Empty Then

        Else
            tb.BackColor = Color.White
            MEP.SetError(tb, "")
        End If
    End Sub
    Private Sub grprecio_CellEdited(sender As Object, e As ColumnActionEventArgs)
        If (_fnAccesible()) Then
            'Habilitar solo las columnas de Precio, %, Monto y Observación
            If (e.Column.Index > 1) Then
                Dim data As String = grprecio.GetValue(e.Column.Index - 1).ToString.Trim 'En esta columna obtengo un protocolo que me indica el estado del precio 0= no insertado 1= ya insertado , a la ves con un '-' me indica la posicion de ese dato en el Datatable que envio para grabarlo que esta en 'precio' Ejemplo:1-15 -> estado=1 posicion=15
                Dim estado As String = data.Substring(0, 1).Trim
                Dim pos As String = data.Substring(2, data.Length - 2)
                If (estado = 1 Or estado = 2) Then
                    precio.Rows(pos).Item("estado") = 2
                    precio.Rows(pos).Item("yhprecio") = grprecio.GetValue(e.Column.Index)
                Else
                    If (estado = 0 Or estado = 3) Then
                        precio.Rows(pos).Item("estado") = 3
                        precio.Rows(pos).Item("yhprecio") = grprecio.GetValue(e.Column.Index)
                    End If
                End If


            End If

        End If
    End Sub

    Private Sub grprecio_EditingCell(sender As Object, e As EditingCellEventArgs)

        If btnGrabar.Visible = False Then
            Return
        End If
        If (_fnAccesible() And IsNothing(grprecio.DataSource) = False) Then
            'Deshabilitar la columna de Productos y solo habilitar la de los precios
            If (e.Column.Index = grprecio.RootTable.Columns("yfcdprod1").Index) Then 'Or e.Column.Index = grprecio.RootTable.Columns("73").Index
                e.Cancel = True
            Else
                e.Cancel = False
            End If
        Else
            e.Cancel = True
        End If
    End Sub
    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click

        Dim grabar As Boolean = L_fnGrabarPrecios("", cbAlmacen.Value, precio)
        If (grabar) Then
            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "categoria Grabado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            _prLimpiar()

            _prCargarTablaPrecios(True)
            _prInhabiliitar()

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La categoria no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If

    End Sub

    Private Sub cbAlmacen_ValueChanged(sender As Object, e As EventArgs) Handles cbAlmacen.ValueChanged

        _prCargarTablaPrecios(True) ''Si el selecciona otra sucursal cambia sus precio por sucursales
    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub SELECCIONARTODOSDELToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SELECCIONARTODOSDELToolStripMenuItem.Click
        Dim img As Bitmap
        If (grcategoria.RowCount > 0) Then
            If (grcategoria.Row > 1) Then
                Dim pos As Integer = grcategoria.Row
                Dim dt As DataTable = CType(grcategoria.DataSource, DataTable)
                Dim mensajeError As String = ""
                Dim res As Boolean = L_fnEliminarCategoria(dt.Rows(pos).Item("ygnumi"), mensajeError)
                If res Then


                    img = New Bitmap(My.Resources.checked, 50, 50)

                    ToastNotification.Show(Me, "Código de Categoria ".ToUpper + Str(dt.Rows(pos).Item("ygnumi")) + " eliminado con Exito.".ToUpper,
                                              img, 2000,
                                              eToastGlowColor.Green,
                                              eToastPosition.TopCenter)

                    _prLimpiar()

                    _prCargarTablaCategorias()

                    _prCargarDatosTablaPrecios()


                Else
                    img = New Bitmap(My.Resources.cancel, 50, 50)
                    ToastNotification.Show(Me, mensajeError, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                End If

            Else
                img = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, "CODIGO DE CATEGORIA DEL SISTEMA NO PUEDE SER ELIMINADA", img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If

    End Sub

    Private Sub grprecio_EditingCell_1(sender As Object, e As EditingCellEventArgs) Handles grprecio.EditingCell
        If btnGrabar.Visible = False Then
            e.Cancel = True
            Return
        End If
        If (_fnAccesible() And IsNothing(grprecio.DataSource) = False) Then
            'Deshabilitar la columna de Productos y solo habilitar la de los precios
            If (e.Column.Index = grprecio.RootTable.Columns("yfcdprod1").Index Or
                e.Column.Index = grprecio.RootTable.Columns("NombreCategoria").Index Or
                e.Column.Index = grprecio.RootTable.Columns("proveedor").Index Or
                e.Column.Index = grprecio.RootTable.Columns("yfcprod").Index Or
                e.Column.Index = grprecio.RootTable.Columns("yfnumi").Index) Then 'Or e.Column.Index = grprecio.RootTable.Columns("73").Index
                e.Cancel = True
            Else
                e.Cancel = False
            End If
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub grprecio_CellEdited_1(sender As Object, e As ColumnActionEventArgs) Handles grprecio.CellEdited
        If (_fnAccesible()) Then
            'Habilitar solo las columnas de Precio, %, Monto y Observación
            If (e.Column.Index > 1) Then
                Dim data As String = grprecio.GetValue(e.Column.Index - 1).ToString.Trim 'En esta columna obtengo un protocolo que me indica el estado del precio 0= no insertado 1= ya insertado , a la ves con un '-' me indica la posicion de ese dato en el Datatable que envio para grabarlo que esta en 'precio' Ejemplo:1-15 -> estado=1 posicion=15
                Dim estado As String = data.Substring(0, 1).Trim
                Dim pos As String = data.Substring(2, data.Length - 2)
                If (estado = 1 Or estado = 2) Then
                    precio.Rows(pos).Item("estado") = 2
                    precio.Rows(pos).Item("yhprecio") = grprecio.GetValue(e.Column.Index)
                Else
                    If (estado = 0 Or estado = 3) Then
                        If (IsNumeric(grprecio.GetValue(e.Column.Index))) Then
                            precio.Rows(pos).Item("estado") = 3
                            precio.Rows(pos).Item("yhprecio") = grprecio.GetValue(e.Column.Index)
                        Else
                            precio.Rows(pos).Item("estado") = 3
                            precio.Rows(pos).Item("yhprecio") = 0
                            grprecio.SetValue(e.Column.Index, 0)


                        End If
                    End If
                End If


            End If

        End If
    End Sub

    Private Sub btnCategoria_Click(sender As Object, e As EventArgs) Handles btnCategoria.Click
        btnCategoria.Visible = False
        btnModificar.Visible = False
        btnGrabar.Visible = False
        PanelCategoria.Visible = True
        GPanelAddCategoria.Visible = True
    End Sub
#End Region



End Class