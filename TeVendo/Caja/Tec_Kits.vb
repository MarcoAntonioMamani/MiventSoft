Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Public Class Tec_Kits

#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Public FilaSeleccionada As Boolean = False


    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"


    Dim TablaImagenes As DataTable
    Dim TablaInventario As DataTable
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim gs_DirPrograma As String = ""
    Dim gs_RutaImg As String = ""
    Dim PersonalId As Integer = 0




#End Region


#Region "Metodos Overrides"
    Public Sub _PMIniciarTodo()
        _MListEstBuscador = _PMOGetListEstructuraBuscador()
        Me.WindowState = FormWindowState.Maximized

        _PMFiltrar()
        _PMInhabilitar()
    End Sub

    Private Sub _PMCargarBuscador()

        Dim dtBuscador As DataTable = _PMOGetTablaBuscador()

        JGrM_Buscador.DataSource = dtBuscador
        JGrM_Buscador.RetrieveStructure()

        For i = 0 To _MListEstBuscador.Count - 1
            Dim campo As String = _MListEstBuscador.Item(i).campo
            With JGrM_Buscador.RootTable.Columns(campo)
                If _MListEstBuscador.Item(i).visible = True Then
                    .Caption = _MListEstBuscador.Item(i).titulo
                    .Width = _MListEstBuscador.Item(i).tamano
                    .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
                    .MaxLines = 2
                    .WordWrap = True
                    Dim col As DataColumn = dtBuscador.Columns(campo)
                    Dim tipo As Type = col.DataType
                    If tipo.ToString = "System.Int32" Or tipo.ToString = "System.Decimal" Or tipo.ToString = "System.Double" Then
                        .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                    End If
                    If _MListEstBuscador.Item(i).formato <> String.Empty Then
                        .FormatString = _MListEstBuscador.Item(i).formato
                    End If
                Else
                    .Visible = False
                End If
            End With
        Next

        'Habilitar Filtradores
        With JGrM_Buscador
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
        'CargarIconEstado()
    End Sub

    Public Sub CargarIconEstado()

        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1
            If (dt.Rows(i).Item("estado") = 1) Then
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.activo, 110, 30)
                img.Save(Bin, Imaging.ImageFormat.Png)
                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("Eliminar") = Bin.GetBuffer
            Else
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.pasivo, 110, 30)
                img.Save(Bin, Imaging.ImageFormat.Png)
                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("Eliminar") = Bin.GetBuffer
            End If

        Next

    End Sub

    Public Sub _PMInhabilitar()
        btnNuevo.Visible = True
        btnModificar.Visible = False
        btnEliminar.Visible = False
        btnGrabar.Visible = False
        PanelNavegacion.Enabled = True
        JGrM_Buscador.Enabled = True


        _PMOLimpiarErrores()

        _PMOInhabilitar()
    End Sub

    Private Sub _PMHabilitar()
        JGrM_Buscador.Enabled = False
        _PMOHabilitar()
    End Sub
    Public Sub _PMFiltrar()
        'cargo el buscador
        _PMCargarBuscador()
        If JGrM_Buscador.RowCount > 0 Then
            _MPos = 0
            JGrM_Buscador.Row = _MPos
            _PMOMostrarRegistro(_MPos)
        Else
            _PMOLimpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub

    Public Sub _PMPrimerRegistro()
        If JGrM_Buscador.RowCount > 0 Then
            _MPos = 0

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub _PMAnteriorRegistro()
        If _MPos > 0 And JGrM_Buscador.RowCount > 0 Then
            _MPos = _MPos - 1

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub _PMSiguienteRegistro()
        If _MPos < JGrM_Buscador.RowCount - 1 Then
            _MPos = _MPos + 1

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub _PMUltimoRegistro()
        If JGrM_Buscador.RowCount > 0 Then
            _MPos = JGrM_Buscador.RowCount - 1

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub

    Private Sub _PMNuevo()
        _MNuevo = True
        _MModificar = False

        _PMOLimpiar()
        _PMHabilitar()

        btnNuevo.Visible = False
        btnModificar.Visible = False
        btnEliminar.Visible = False
        btnGrabar.Visible = True
        PanelNavegacion.Enabled = False
        tbNombreKit.Focus()


        '_PMOLimpiar()

    End Sub

    Private Sub _PMModificar()
        If JGrM_Buscador.Row >= 0 Then
            _MNuevo = False
            _MModificar = True

            _PMHabilitar()
            btnNuevo.Visible = False
            btnModificar.Visible = False
            btnEliminar.Visible = False
            btnGrabar.Visible = True

            PanelNavegacion.Enabled = False


        End If
    End Sub

    Private Sub _PMEliminar()
        'Dim _Result As MsgBoxResult
        '_Result = MsgBox("¿Esta seguro de Eliminar el Registro?".ToUpper, MsgBoxStyle.YesNo, "Advertencia".ToUpper)
        'If _Result = MsgBoxResult.Yes Then
        '    _PMOEliminarRegistro()
        '    _PMFiltrar()

        'End If
        _PMOEliminarRegistro()
    End Sub

    Private Sub _PMGuardar()

        If _PMOValidarCampos() = False Then
            Exit Sub
        End If

        If _MNuevo Then

            If _PMOGrabarRegistro() = True Then
                    'actualizar el grid de buscador
                    _PMCargarBuscador()

                    _PMOLimpiar()
                    _PMSalir()
                Else
                    Exit Sub
                End If





        Else

            _PMOModificarRegistro()

            'actualizar el grid de buscador
            _PMCargarBuscador()

            '_PMSalir()
        End If
    End Sub

    Private Sub _PMSalir()
        If btnGrabar.Visible = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()
            TabControlPrincipal.SelectedTabIndex = 1
        Else
            '  Public _modulo As SideNavItem
            '_TabControl.SelectedTab = _modulo
            '_tab.Close()
            'Me.Close()
            TabControlPrincipal.SelectedTabIndex = 1
        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)

        Me.Text = "Gestion De Kits"

        _PMIniciarTodo()
        _prAsignarPermisos()


        _habilitarFocus()

    End Sub
    Private Sub _prCargarTablProductos(id As String)
        Dim dt As New DataTable
        dt = ListaProductosKits(id)

        grProductos.DataSource = dt
        grProductos.RetrieveStructure()
        grProductos.AlternatingColors = True
        'a.Id,a.KitId,a.ProductoId,p.NombreProducto,a.Precio,a.Cantidad,a.SubTotal,cast('' as image) as Eliminar

        With grProductos.RootTable.Columns("Id")
            .Width = 90
            .Caption = "Cod Kit"
            .Visible = True
        End With
        With grProductos.RootTable.Columns("KitId")
            .Width = 110
            .Visible = False
        End With
        With grProductos.RootTable.Columns("ProductoId")
            .Width = 110
            .Visible = False
        End With
        With grProductos.RootTable.Columns("NombreProducto")
            .Width = 300
            .Visible = True
            .Caption = "Producto"
        End With
        With grProductos.RootTable.Columns("Precio")
            .Width = 100
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Precio"
            .TextAlignment = TextAlignment.Far
        End With
        With grProductos.RootTable.Columns("Cantidad")
            .Width = 100
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad"
            .TextAlignment = TextAlignment.Far

        End With
        With grProductos.RootTable.Columns("SubTotal")
            .Width = 100
            .Visible = True
            .FormatString = "0.00"
            .Caption = "SubTotal"
            .TextAlignment = TextAlignment.Far

        End With
        With grProductos.RootTable.Columns("Estado")
            .Width = 150
            .Visible = False
        End With
        With grProductos.RootTable.Columns("PrecioCosto")
            .Width = 150
            .Visible = False
        End With
        With grProductos.RootTable.Columns("PrecioVentaBackup")
            .Width = 150
            .Visible = False
        End With
        With grProductos.RootTable.Columns("Eliminar")
            .Width = 120
            .Visible = False
        End With
        With grProductos
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .CellToolTipText = "Conceptos"
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed

        End With
        CargarIconEliminar()
    End Sub

    Public Sub CargarIconEliminar()
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.eliminar01, grProductos.RootTable.Columns("Eliminar").Width - 20, 45)
        img.Save(Bin, Imaging.ImageFormat.Png)
        Dim dt As DataTable = CType(grProductos.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1


            CType(grProductos.DataSource, DataTable).Rows(i).Item("Eliminar") = Bin.GetBuffer


        Next

    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNombreKit, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDescripcion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbPrecio, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnAgregarProducto, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        End With
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








#End Region

#Region "METODOS SOBRECARGADOS"


    Public Sub _PMOHabilitar()

        tbNombreKit.ReadOnly = False

        tbDescripcion.ReadOnly = False


        swEstado.IsReadOnly = False
        tbPrecio.IsInputReadOnly = True
        panelConceptoFijo.Visible = True

        grProductos.RootTable.Columns("Eliminar").Visible = True
    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True

        panelConceptoFijo.Visible = False
        tbNombreKit.ReadOnly = True

        tbDescripcion.ReadOnly = True


        swEstado.IsReadOnly = True
        tbPrecio.IsInputReadOnly = True


        grProductos.RootTable.Columns("Eliminar").Visible = False

    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbNombreKit.Clear()
        tbPrecio.Value = 0
        tbDescripcion.Clear()

        swEstado.Value = True

        tbNombreKit.Focus()
        _prCargarTablProductos(-1)
    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()


        tbNombreKit.BackColor = Color.White


    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        '_Id As String, NombreKit As String, DescripcionKit As String, Estado As Integer, Total As Double, dtdetalle As DataTable

        'a.Id,a.KitId,a.ProductoId,p.NombreProducto,a.Precio,a.Cantidad,a.SubTotal,1 as Estado
        Dim res As Boolean
        Try
            Dim dt As New DataTable
            dt = CType(grProductos.DataSource, DataTable).DefaultView.ToTable(False, "Id", "KitId", "ProductoId", "NombreProducto", "Precio", "Cantidad", "SubTotal", "estado", "PrecioCosto", "PrecioVentaBackup")

            res = InsertarKit(tbCodigo.Text, tbNombreKit.Text, tbDescripcion.Text, IIf(swEstado.Value = True, 1, 0), tbPrecio.Value, dt)

            If res Then


                ToastNotification.Show(Me, "Codigo de Kit ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar el Kit".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Kit".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Dim dt As New DataTable
            dt = CType(grProductos.DataSource, DataTable).DefaultView.ToTable(False, "Id", "KitId", "ProductoId", "NombreProducto", "Precio", "Cantidad", "SubTotal", "estado", "PrecioCosto", "PrecioVentaBackup")

            Res = ModificarKit(tbCodigo.Text, tbNombreKit.Text, tbDescripcion.Text, IIf(swEstado.Value = True, 1, 0), tbPrecio.Value, dt)
            If Res Then

                ToastNotification.Show(Me, "Codigo de Kits ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el Kits".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar Kits".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbNombreKit.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Kits " + tbNombreKit.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_Kits")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Kits ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el Kits".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "

        If tbNombreKit.Text.Trim = String.Empty Then
            tbNombreKit.BackColor = Color.Red
            MEP.SetError(tbNombreKit, "Ingrese Nombre Kits")
            Mensaje = Mensaje + "Nombre Kit"
            _ok = False
        Else
            tbNombreKit.BackColor = Color.White
            MEP.SetError(tbNombreKit, "")
        End If


        MHighlighterFocus.UpdateHighlights()
        If tbNombreKit.Text.Trim = String.Empty Then
            tbNombreKit.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If


        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Kits")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        ' a.Id,a.NombreKit,a.DescripcionKit,a.CategoriaId,a.Estado,a.Total
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("NombreKit", True, "Nombre Kit", 200))
        listEstCeldas.Add(New Celda("DescripcionKit", True, "Descripción Kits", 250))
        listEstCeldas.Add(New Celda("CategoriaId", False))
        listEstCeldas.Add(New Celda("Estado", False))
        listEstCeldas.Add(New Celda("Total", True, "Total Kits", 120, "0.00"))




        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        ' a.Id,a.NombreKit,a.DescripcionKit,a.CategoriaId,a.Estado,a.Total	
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbNombreKit.Text = .GetValue("NombreKit").ToString
            tbDescripcion.Text = .GetValue("DescripcionKit").ToString

            tbPrecio.Value = .GetValue("Total")


            swEstado.Value = .GetValue("Estado")


        End With
        _prCargarTablProductos(tbCodigo.Text)
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub




    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()
            TabControlPrincipal.SelectedTabIndex = 1
        Else
            '  Public _modulo As SideNavItem
            '_TabControl.SelectedTab = _modulo
            '_tab.Close()
            'Me.Close()
            TabControlPrincipal.SelectedTabIndex = 1
        End If
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _PMNuevo()

    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        _PMModificar()

    End Sub

    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        _PMGuardar()

    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        _PMEliminar()


    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _PMSalir()

    End Sub

    Private Sub Tec_Users_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        _PMPrimerRegistro()
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        _PMAnteriorRegistro()
    End Sub

    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        _PMSiguienteRegistro()
    End Sub

    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        _PMUltimoRegistro()
    End Sub
    Private Sub JGrM_Buscador_SelectionChanged(sender As Object, e As EventArgs) Handles JGrM_Buscador.SelectionChanged
        If (JGrM_Buscador.Row >= 0 And FilaSeleccionada = False) Then
            _MPos = JGrM_Buscador.Row
            _PMOMostrarRegistro(_MPos, True)
        End If
    End Sub

    Private Sub VerToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles VerToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then

            TabControlPrincipal.SelectedTabIndex = 0
        End If
    End Sub

    Private Sub EditarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditarToolStripMenuItem.Click
        If (JGrM_Buscador.Row >= 0) Then
            TabControlPrincipal.SelectedTabIndex = 0
            btnModificar.PerformClick()
            tbNombreKit.Focus()
        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then
            btnEliminar.PerformClick()
        End If
    End Sub










#End Region


    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        TabControlPrincipal.SelectedTabIndex = 0
        btnNuevo.PerformClick()

    End Sub



    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Private Sub btnAgregarConceptoFijo_Click(sender As Object, e As EventArgs) Handles btnAgregarProducto.Click
        Dim dt As DataTable

        dt = ListarProductosKits()


        'ProductoId	NombreProducto	NombreCategoria	PrecioVenta	PrecioCosto

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("ProductoId,", True, "Cod Producto", 80))
        listEstCeldas.Add(New Celda("NombreProducto", True, "Producto", 300))
        listEstCeldas.Add(New Celda("NombreCategoria", True, "Categoria", 120))
        listEstCeldas.Add(New Celda("PrecioVenta", True, "Precio Venta", 90, "0.00"))
        listEstCeldas.Add(New Celda("PrecioCosto", False, "PrecioCosto", 120))
        Dim ef = New Efecto
        ef.tipo = 22
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 50
        ef.ancho = 350
        ef.Context = "Seleccione Productos".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
            Dim Bin As New MemoryStream
            Dim img As New Bitmap(My.Resources.eliminar01, grProductos.RootTable.Columns("Eliminar").Width - 20, 45)
            img.Save(Bin, Imaging.ImageFormat.Png)
            'a.Id, a.KitId, a.ProductoId, p.NombreProducto, a.Precio, a.Cantidad, a.SubTotal, 1 As Estado, cast('' as image) as Eliminar,
            'a.PrecioCosto, a.PrecioVentaBackup

            CType(grProductos.DataSource, DataTable).Rows.Add(_GenerarId() + 1, 0, Row.Cells("ProductoId").Value, Row.Cells("NombreProducto").Value, Row.Cells("PrecioVenta").Value, 1, Row.Cells("PrecioVenta").Value, 0, Bin.GetBuffer, Row.Cells("PrecioCosto").Value, Row.Cells("PrecioVenta").Value)


            btnAgregarProducto.Focus()

        End If
    End Sub
    Public Function _GenerarId()
        Dim dt As DataTable = CType(grProductos.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(grProductos.DataSource, DataTable).Rows(i).Item("Id")), 0, CType(grProductos.DataSource, DataTable).Rows(i).Item("Id"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function
    Public Function ObtenerPosicion(id As Integer) As Integer

        Dim dt As DataTable = CType(grProductos.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("id") = id) Then

                Return i
            End If
        Next
        Return -1
    End Function
    Private Sub grProductos_Click(sender As Object, e As EventArgs) Handles grProductos.Click
        Try
            If (grProductos.RowCount >= 1 And grProductos.Row >= 0) Then
                If (grProductos.CurrentColumn.Index = grProductos.RootTable.Columns("Eliminar").Index) Then
                    Dim posicion As Integer = -1
                    posicion = ObtenerPosicion(grProductos.GetValue("id"))
                    CType(grProductos.DataSource, DataTable).Rows(posicion).Item("estado") = -1
                    grProductos.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grProductos.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))

                End If

            End If
        Catch ex As Exception

        End Try

    End Sub
    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grProductos.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grProductos.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub

    Private Sub grProductos_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grProductos.CellValueChanged
        Dim lin As Integer = grProductos.GetValue("Id")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin)
        If (e.Column.Index = grProductos.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grProductos.GetValue("Cantidad")) Or grProductos.GetValue("Cantidad").ToString = String.Empty) Then

                'grProductos.GetRow(rowIndex).Cells("cant").Value = 1
                '  grProductos.CurrentRow.Cells.Item("cant").Value = 1

                CType(grProductos.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1

                CType(grProductos.DataSource, DataTable).Rows(pos).Item("SubTotal") = CType(grProductos.DataSource, DataTable).Rows(pos).Item("Precio")
                Dim estado As Integer = CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado")

                grProductos.SetValue("Cantidad", 1)
                grProductos.SetValue("SubTotal", grProductos.GetValue("Precio"))

                If (estado = 1) Then
                    CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grProductos.GetValue("Cantidad") > 0) Then



                    Dim estado As Integer = CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado")
                    Dim rowIndex01 As Integer = grProductos.Row
                    P_PonerTotal(rowIndex01)
                    If (estado = 1) Then
                        CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If




                Else

                    CType(grProductos.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1

                    CType(grProductos.DataSource, DataTable).Rows(pos).Item("Subtotal") = CType(grProductos.DataSource, DataTable).Rows(pos).Item("Precio")
                    Dim estado As Integer = CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado")

                    grProductos.SetValue("Cantidad", 1)

                    grProductos.SetValue("SubTotal", grProductos.GetValue("Precio"))
                    If (estado = 1) Then
                        CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If
        End If

        If (e.Column.Index = grProductos.RootTable.Columns("Precio").Index) Then
            If (Not IsNumeric(grProductos.GetValue("Precio")) Or grProductos.GetValue("Precio").ToString = String.Empty) Then


                CType(grProductos.DataSource, DataTable).Rows(pos).Item("Precio") = grProductos.GetValue("PrecioVentaBackup")
                CType(grProductos.DataSource, DataTable).Rows(pos).Item("SubTotal") = grProductos.GetValue("PrecioVentaBackup") * grProductos.GetValue("Cantidad")

                Dim estado As Integer = CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado")

                grProductos.SetValue("Precio", grProductos.GetValue("PrecioVentaBackup"))
                grProductos.SetValue("SubTotal", (grProductos.GetValue("PrecioVentaBackup") * grProductos.GetValue("Cantidad")))



                If (estado = 1) Then
                    CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else


                Dim estado As Integer = CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado")
                    Dim rowIndex01 As Integer = grProductos.Row
                    P_PonerTotal(rowIndex01)
                    If (estado = 1) Then
                        CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If



            End If
            End If




        Dim rowIndex As Integer = grProductos.Row
        P_PonerTotal(rowIndex)
    End Sub
    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < grProductos.RowCount) Then

            Dim lin As Integer = grProductos.GetValue("Id")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin)
            Dim cant As Double = grProductos.GetValue("Cantidad")
            Dim uni As Double = grProductos.GetValue("Precio")

            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni
                'grProductos.SetValue("lcmdes", montodesc)

                CType(grProductos.DataSource, DataTable).Rows(pos).Item("SubTotal") = TotalUnitario
                grProductos.SetValue("SubTotal", TotalUnitario)
                Dim estado As Integer = CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado")



                If (estado = 1) Then
                    CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            End If
            _prCalcularPrecioTotal()
        End If



    End Sub
    Public Sub _prCalcularPrecioTotal()

        Dim dt As DataTable = CType(grProductos.DataSource, DataTable)
        Dim Total As Double = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            If (dt.Rows(i).Item("estado") >= 0) Then
                Total += dt.Rows(i).Item("Subtotal")
            End If
        Next

        tbPrecio.Value = Total
    End Sub
    Public Function _fnAccesible()
        Return tbNombreKit.ReadOnly = False
    End Function
    Private Sub grProductos_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grProductos.EditingCell
        If (_fnAccesible()) Then
            'a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
            'a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
            '1 As estado, cast('' as image ) as img
            ', 0 as stock
            'Habilitar solo las columnas de Precio, %, Monto y Observación
            If (e.Column.Index = grProductos.RootTable.Columns("Cantidad").Index Or e.Column.Index = grProductos.RootTable.Columns("Precio").Index) Then
                e.Cancel = False
                Return
            Else


                e.Cancel = True


            End If


        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub grProductos_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grProductos.CellEdited
        Dim lin As Integer = grProductos.GetValue("Id")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin)
        If (grProductos.GetValue("Precio") < grProductos.GetValue("PrecioCosto")) Then

            ToastNotification.Show(Me, "El Precio Es Menor Al Precio De Costo Del Producto que Es = " + Str(grProductos.GetValue("PrecioCosto")), img, 6000, eToastGlowColor.Red, eToastPosition.TopCenter)

            CType(grProductos.DataSource, DataTable).Rows(pos).Item("Precio") = grProductos.GetValue("PrecioVentaBackup")
            CType(grProductos.DataSource, DataTable).Rows(pos).Item("SubTotal") = grProductos.GetValue("PrecioVentaBackup") * grProductos.GetValue("Cantidad")

            Dim estado As Integer = CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado")

            grProductos.SetValue("Precio", grProductos.GetValue("PrecioVentaBackup"))
            grProductos.SetValue("SubTotal", (grProductos.GetValue("PrecioVentaBackup") * grProductos.GetValue("Cantidad")))

            Dim rowIndex01 As Integer = grProductos.Row
            P_PonerTotal(rowIndex01)
            If (estado = 1) Then
                CType(grProductos.DataSource, DataTable).Rows(pos).Item("estado") = 2
            End If





        End If
    End Sub
End Class