
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO

Public Class Tec_Conciliacion
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

    Dim ConciliacionID As Integer = 0
    Dim SucursalId As Integer = 1
    Dim MovimientoSalidId As Integer = 5





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
        With JGrM_Buscador.RootTable.Columns("imgEstado")
            .TopMargin = 7
            .LeftMargin = 5
            .BottomMargin = 7

        End With
        CargarIconEstado()
    End Sub

    Public Sub CargarIconEstado()
        Dim BinAbierto As New MemoryStream
        Dim imgAbierto As New Bitmap(My.Resources.conciliacionabierto, 150, 20)
        imgAbierto.Save(BinAbierto, Imaging.ImageFormat.Png)

        Dim BinCerrado As New MemoryStream
        Dim imgCerrado As New Bitmap(My.Resources.pasivo, 110, 30)
        imgCerrado.Save(BinCerrado, Imaging.ImageFormat.Png)

        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1
            If (dt.Rows(i).Item("Estado") = 1) Then

                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("imgEstado") = BinAbierto.GetBuffer
            Else

                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("imgEstado") = BinCerrado.GetBuffer
            End If

        Next

    End Sub

    Public Sub _PMInhabilitar()
        btnNuevo.Visible = False
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
        tbPersonal.Focus()


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

        Me.Text = "Conciliaciones De Productos"

        _PMIniciarTodo()
        _prAsignarPermisos()


        _habilitarFocus()

    End Sub


    Public Function GenerarIdSalidas(dtSal As DataTable) As DataTable
        Dim dt As DataTable = New DataTable
        ''Aqui obtenemos los id de las salidas
        dt.Columns.Add("IdSalida", GetType(Integer))

        For i As Integer = 0 To dtSal.Rows.Count - 1 Step 1

            Dim result As DataRow() = dt.Select("IdSalida=" + Str(dtSal.Rows(i).Item("DespachoProductosId")))
            If (result.Length <= 0) Then

                dt.Rows.Add(dtSal.Rows(i).Item("DespachoProductosId"))
            End If

        Next
        Return dt

    End Function




    Private Sub _prCargarTablaDetalle(id As String)
        Dim dt As New DataTable
        dt = ListarProductosSalidasConciliacion(id)  ''Aqui obtenemos solo los productos de las salidas
        Dim dtSalidas = ListarTodasSalidas(id)
        Dim dtIdSalidas = GenerarIdSalidas(dtSalidas)
        Dim n As Integer = 0
        If (dtSalidas.Rows.Count > 0) Then

            dtIdSalidas.DefaultView.Sort = "IdSalida asc"
            n = dtSalidas.Rows(0).Item("CantidadSalidas")

            For i As Integer = 0 To n - 1 Step 1
                '' Aqui estamos agregando la columna salida
                Dim idSalida As Integer = dtIdSalidas.Rows(i).Item("IdSalida")
                dt.Columns.Add(Str(dtIdSalidas.Rows(i).Item("IdSalida")))

                For j As Integer = 0 To dt.Rows.Count - 1 Step 1   ''Aqui recorremos el Datatable Principal

                    Dim dtSalProducto() As DataRow = dtSalidas.Select("DespachoProductosId=" + Str(idSalida) + " and ProductoId=" + Str(dt.Rows(j).Item("ProductoId")))


                    If (dtSalProducto.Count = 1) Then

                        dt.Rows(j).Item(Str(idSalida)) = dtSalProducto(0).Item("Cantidad")
                    Else
                        dt.Rows(j).Item(Str(idSalida)) = Nothing
                    End If

                Next

            Next
        End If

        dt.Columns.Add("TotalSalida".Trim)
        dt.Columns.Add("TotalDevoluciones".Trim)
        dt.Columns.Add("TotalEntregado".Trim)
        dt.Columns.Add("Diferencia".Trim)

        ''Aqui rellenaremos los vamores de totales

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim idProducto As Integer = dt.Rows(i).Item("ProductoId")
            Dim totalProductoSalida As Double = 0
            Dim dtSalProducto() As DataRow = dtSalidas.Select("ProductoId=" + Str(idProducto))
            If (dtSalProducto.Count >= 1) Then

                For Each dr As DataRow In dtSalProducto

                    totalProductoSalida += dr.Item("Cantidad")

                Next
            End If
            dt.Rows(i).Item("TotalSalida") = totalProductoSalida
            dt.Rows(i).Item("TotalDevoluciones") = 0
            dt.Rows(i).Item("TotalEntregado") = 0
            dt.Rows(i).Item("Diferencia") = 0
        Next



        grDetalle.DataSource = dt
        grDetalle.RetrieveStructure()
        grDetalle.AlternatingColors = True


        With grDetalle.RootTable.Columns("TotalSalida")
            .Width = 70
            .Caption = "Tot Salidas."
            .Visible = True
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
        End With
        With grDetalle.RootTable.Columns("TotalDevoluciones")
            .Width = 70
            .Caption = "Devoluciones."
            .Visible = True
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
        End With

        With grDetalle.RootTable.Columns("TotalEntregado")
            .Width = 70
            .Caption = "Entregado."
            .Visible = True
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
        End With
        With grDetalle.RootTable.Columns("Diferencia")
            .Width = 70
            .Caption = "Diferencia"
            .Visible = True
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
        End With
        'c.Id ,c.ContratoId ,c.ConceptoId,concepto .NombreConcepto ,concepto.Porcentaje as PorcentajeConcepto,1 as estado
        For i As Integer = 0 To n - 1 Step 1
            Dim idSalida As Integer = dtIdSalidas.Rows(i).Item("IdSalida")
            With grDetalle.RootTable.Columns(Str(idSalida))
                .Width = 70
                .Caption = "Sal.#0".Trim + Str(i + 1).Trim
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
            End With

        Next
        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 40
            .Caption = "Item"
            .Visible = True
        End With
        With grDetalle.RootTable.Columns("NombreProducto")
            .Width = 250
            .MaxLines = 2
            .WordWrap = True
            .Visible = True
            .Caption = "Producto"
        End With

        With grDetalle
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

    End Sub


    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbPersonal, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbFechaConciliacion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDetalle, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

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

        tbFechaConciliacion.ReadOnly = False
        tbDetalle.ReadOnly = False
        btnImprimir.Visible = False
        swEstado.IsReadOnly = False

    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbFechaConciliacion.ReadOnly = True
        tbDetalle.ReadOnly = True

        swEstado.IsReadOnly = True
        tbPersonal.ReadOnly = True
        btnImprimir.Visible = True

    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbPersonal.Clear()
        tbDetalle.Clear()
        tbFechaConciliacion.Value = Now.Date


        _prCargarTablaDetalle(-1)
    End Sub


    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbPersonal.BackColor = Color.White
        tbFechaConciliacion.BackColor = Color.White
        tbDetalle.BackColor = Color.White
    End Sub

    Public Function _PMOGrabarRegistro() As Boolean


        Return True

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try

            Res = ModificarDespachoProductos(tbCodigo.Text, PersonalId, ConciliacionID, SucursalId, tbFechaConciliacion.Value.ToString("dd/MM/yyyy"), tbCodigo.Text, tbDetalle.Text, MovimientoSalidId, CType(grDetalle.DataSource, DataTable))
            If Res Then

                ToastNotification.Show(Me, "Conciliación # ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar la conciliación".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar Conciliación".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbPersonal.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()
    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "

        If PersonalId <= 0 Then
            tbPersonal.BackColor = Color.Red
            MEP.SetError(tbPersonal, "Seleccione Personal")
            Mensaje = Mensaje + "Personal"
            _ok = False
        Else
            tbPersonal.BackColor = Color.White
            MEP.SetError(tbPersonal, "")
        End If
        If (tbFechaConciliacion.Value.ToString.Trim = String.Empty) Then
            tbFechaConciliacion.BackColor = Color.Red
            MEP.SetError(tbFechaConciliacion, "Seleccione Fecha")
            Mensaje = Mensaje + "Fecha Salida"
            _ok = False
        Else
            tbFechaConciliacion.BackColor = Color.White
            MEP.SetError(tbFechaConciliacion, "")
        End If


        MHighlighterFocus.UpdateHighlights()
        If (tbFechaConciliacion.Value.ToString.Trim = String.Empty) Then
            tbFechaConciliacion.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If
        If PersonalId <= 0 Then
            tbPersonal.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If


        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Conciliacion")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        '  Id	Fecha	Estado	PersonalID	NombrePersonal	SucursalId	Observacion	imgEstado	
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "Cod. Conciliación", 60))
        listEstCeldas.Add(New Celda("PersonalId", False))
        listEstCeldas.Add(New Celda("Fecha", True, "Fecha Conciliación", 100, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("Estado", False))
        listEstCeldas.Add(New Celda("NombrePersonal", True, "Nombre Personal", 300))
        listEstCeldas.Add(New Celda("SucursalId", False))
        listEstCeldas.Add(New Celda("Observacion", True, "Detalle Conciliación", 250))
        listEstCeldas.Add(New Celda("imgEstado", True, "Estado Conciliacion", 120))
        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        ''Id    Fecha	Estado	PersonalID	NombrePersonal	SucursalId	Observacion	imgEstado	
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbPersonal.Text = .GetValue("NombrePersonal").ToString
            PersonalId = .GetValue("PersonalId")
            tbFechaConciliacion.Value = .GetValue("Fecha")
            tbDetalle.Text = .GetValue("Observacion")
            SucursalId = .GetValue("SucursalId")
            ConciliacionID = .GetValue("Id")
            swEstado.Value = .GetValue("Estado")

        End With
        _prCargarTablaDetalle(tbCodigo.Text)
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
            tbPersonal.Focus()
        End If
    End Sub











#End Region


End Class