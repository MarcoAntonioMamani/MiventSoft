
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Public Class Tec_Despachos
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
                    .TextAlignment = TextAlignment.Center
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
        With JGrM_Buscador.RootTable.Columns("imgConciliacion")
            .TopMargin = 7
            .LeftMargin = 5
            .BottomMargin = 7

        End With
        CargarIconEstado()
    End Sub

    Public Sub CargarIconEstado()
        Dim BinAbierto As New MemoryStream
        Dim imgAbierto As New Bitmap(My.Resources.conciliacionabierto, 150, 25)
        imgAbierto.Save(BinAbierto, Imaging.ImageFormat.Png)

        Dim BinCerrado As New MemoryStream
        Dim imgCerrado As New Bitmap(My.Resources.conciliacioncerrado, 150, 25)
        imgCerrado.Save(BinCerrado, Imaging.ImageFormat.Png)

        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1
            If (dt.Rows(i).Item("EstadoConciliacion") = 1) Then

                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("imgConciliacion") = BinAbierto.GetBuffer
            Else

                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("imgConciliacion") = BinCerrado.GetBuffer
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

        Me.Text = "Despacho De Productos"

        _PMIniciarTodo()
        _prAsignarPermisos()


        _habilitarFocus()

        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date


    End Sub
    Private Sub _prCargarTablaDetalle(id As String)
        Dim dt As New DataTable
        dt = ListaDetalleDespachoProductos(id)


        grDetalle.DataSource = dt
        grDetalle.RetrieveStructure()
        grDetalle.AlternatingColors = True
        'c.Id ,c.ContratoId ,c.ConceptoId,concepto .NombreConcepto ,concepto.Porcentaje as PorcentajeConcepto,1 as estado

        With grDetalle.RootTable.Columns("Id")
            .Width = 90
            .Caption = "Id"
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("DespachoProductosId")
            .Width = 110
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 110
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Caption = "Cod. Producto"
        End With
        With grDetalle.RootTable.Columns("NombreProducto")
            .Width = 300
            .Visible = True
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .Caption = "Producto"
        End With

        With grDetalle.RootTable.Columns("Cantidad")
            .Width = 110
            .Caption = "Cant Unitario"
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grDetalle.RootTable.Columns("CantidadCaja")
            .Width = 110
            .Caption = "Cant Cajas"
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grDetalle.RootTable.Columns("conversion")
            .Width = 70
            .Caption = "Conversion"
            .Visible = True
            .FormatString = "0.00"
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grDetalle.RootTable.Columns("Stock")
            .Width = 110
            .Caption = "Stock"
            .Visible = False
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grDetalle.RootTable.Columns("estado")
            .Width = 150
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("img")
            .Width = 120
            .Visible = False
            .Caption ="Eliminar"
            .LeftMargin = 7
            .TopMargin = 5
            .BottomMargin = 5
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
        CargarIconEliminar()
    End Sub

    Public Sub CargarIconEliminar()
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.rowdelete, 25, 18)
        img.Save(Bin, Imaging.ImageFormat.Png)
        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1


            CType(grDetalle.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer


        Next

    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbPersonal, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSearchPersonal, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbFechaSalida, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDetalle, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
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

        tbFechaSalida.ReadOnly = False
        tbDetalle.ReadOnly = False

        btnSearchPersonal.Visible = True
        btnImprimir.Visible = False


        panelProducto.Visible = True

        grDetalle.RootTable.Columns("img").Visible = True
        grDetalle.RootTable.Columns("stock").Visible = True
    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbFechaSalida.ReadOnly = True
        tbDetalle.ReadOnly = True

        panelProducto.Visible = False
        tbPersonal.ReadOnly = True

        btnSearchPersonal.Visible = False
        btnImprimir.Visible = True

        grDetalle.RootTable.Columns("img").Visible = False
        grDetalle.RootTable.Columns("stock").Visible = False
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbPersonal.Clear()
        tbDetalle.Clear()
        tbFechaSalida.Value = Now.Date
        PersonalId = 0
        btnSearchPersonal.Focus()
        _prCargarTablaDetalle(-1)
    End Sub


    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbPersonal.BackColor = Color.White
        tbFechaSalida.BackColor = Color.White

    End Sub
    Public Sub ReporteVenta(Id As String)
        Dim ef = New Efecto


        ef.tipo = 8
        ef.titulo = "Comprobante de Despacho"
        ef.descripcion = "¿Desea Generar el Comprobante De Despacho  #" + Id + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            ImprimirNotaSalida(Id)


        End If
        ef.Dispose()

    End Sub
    Public Function _PMOGrabarRegistro() As Boolean

        Dim res As Boolean
        Try
            Dim Id As String = ""
            '_Id As String, PersonalId As Integer, ConciliacionId As Integer, SucursalId As Integer, Fecha As String, NroNota As String, Detalle As String, TipoMovimientoID As Integer, dtdetalle As DataTable
            res = InsertarDespachoProductos(Id, PersonalId, ConciliacionID, SucursalId, tbFechaSalida.Value.ToString("yyyy/MM/dd"), tbCodigo.Text, tbDetalle.Text, MovimientoSalidId, CType(grDetalle.DataSource, DataTable))

            If res Then
                ToastNotification.Show(Me, "Codigo de Despacho ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                ReporteVenta(Id)


            Else
                ToastNotification.Show(Me, "Error al guardar el Despacho".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Despacho".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try

            Res = ModificarDespachoProductos(tbCodigo.Text, PersonalId, ConciliacionID, SucursalId, tbFechaSalida.Value.ToString("yyyy/MM/dd"), tbCodigo.Text, tbDetalle.Text, MovimientoSalidId, CType(grDetalle.DataSource, DataTable))
            If Res Then

                ToastNotification.Show(Me, "Codigo de Despacho ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                ReporteVenta(tbCodigo.Text)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el Despacho".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar Despacho".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbPersonal.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Despacho del Personal" + tbPersonal.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_DespachoProductos")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Despacho ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el Despacho".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If
        ef.Dispose()


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
        If (tbFechaSalida.Value.ToString.Trim = String.Empty) Then
            tbFechaSalida.BackColor = Color.Red
            MEP.SetError(tbFechaSalida, "Seleccione Fecha")
            Mensaje = Mensaje + "Fecha Salida"
            _ok = False
        Else
            tbFechaSalida.BackColor = Color.White
            MEP.SetError(tbFechaSalida, "")
        End If


        MHighlighterFocus.UpdateHighlights()
        If (tbFechaSalida.Value.ToString.Trim = String.Empty) Then
            tbFechaSalida.Focus()
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

        Dim dtBuscador As DataTable = L_prListarGeneralFechas("MAM_DespachoProductos", cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        '     c.Id , c.PersonalId, p.NombrePersonal, c.TipoContratoID, TipoContrato.Descripcion as TipoContratoDescripcion
        ',c.Cargo ,Cargo .Descripcion as DescripcionCargo,c.SueldoBase ,c.InicioContrato ,c.FinContrato ,c.Estado ,c.Indefinido 	
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "Cod. Despacho", 60))
        listEstCeldas.Add(New Celda("PersonalId", False))
        listEstCeldas.Add(New Celda("NombrePersonal", True, "Nombre Personal", 300))
        listEstCeldas.Add(New Celda("ConciliacionId", False))
        listEstCeldas.Add(New Celda("SucursalId", False))
        listEstCeldas.Add(New Celda("Fecha", True, "Fecha Despacho", 100, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("NroNota", False))
        listEstCeldas.Add(New Celda("Detalle", True, "Detalle", 200))
        listEstCeldas.Add(New Celda("TipoMovimientoId", False))
        listEstCeldas.Add(New Celda("EstadoConciliacion", False, "Estado Conciliacion", 100))
        listEstCeldas.Add(New Celda("imgConciliacion", True, "Estado Conciliacion", 120))



        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)


        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbPersonal.Text = .GetValue("NombrePersonal").ToString
            PersonalId = .GetValue("PersonalId")
            tbFechaSalida.Value = .GetValue("Fecha")
            tbDetalle.Text = .GetValue("Detalle")
            SucursalId = .GetValue("SucursalId")
            ConciliacionID = .GetValue("ConciliacionId")

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
            If (JGrM_Buscador.GetValue("EstadoConciliacion") = 0) Then '' Si La Conciliacion Esta Cerrada No Puedo Modificar Ni Eliminar
                ToastNotification.Show(Me, "La Conciliacion Ya Esta Cerrada No Es Posible Editar Este Despacho. Primero Debe Revertir El Estado De La Conciliacion", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Else
                TabControlPrincipal.SelectedTabIndex = 0
                btnModificar.PerformClick()
                tbPersonal.Focus()
            End If


        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then
            If (JGrM_Buscador.GetValue("EstadoConciliacion") = 0) Then '' Si La Conciliacion Esta Cerrada No Puedo Modificar Ni Eliminar
                ToastNotification.Show(Me, "La Conciliacion Ya Esta Cerrada No Es Posible Editar Este Despacho. Primero Debe Revertir El Estado De La Conciliacion", img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Else
                btnEliminar.PerformClick()
            End If


        End If
    End Sub










#End Region


    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        TabControlPrincipal.SelectedTabIndex = 0
        btnNuevo.PerformClick()
    End Sub

    Private Sub btnSearchPersonal_Click(sender As Object, e As EventArgs) Handles btnSearchPersonal.Click
        Dim dt As DataTable

        dt = ListarPersonal()
        'a.Id ,a.NombreProveedor ,a.Direccion ,a.Telefono01

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id,", False, "ID", 50))
        listEstCeldas.Add(New Celda("Nombre", True, "NOMBRE", 350))
        listEstCeldas.Add(New Celda("Direccion", True, "DIRECCION", 180))
        listEstCeldas.Add(New Celda("Telefono01", True, "Telefono".ToUpper, 200))
        Dim ef = New Efecto
        ef.tipo = 6
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 150
        ef.ancho = 500
        ef.Context = "Seleccione Personal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            PersonalId = Row.Cells("Id").Value
            tbPersonal.Text = Row.Cells("Nombre").Value
            tbDetalle.Focus()

        End If
        ef.Dispose()

    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Public Function ObtenerPosicion(id As Integer) As Integer

        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("id") = id) Then

                Return i
            End If
        Next
        Return -1
    End Function

    Private Sub btnAgregarProducto_Click(sender As Object, e As EventArgs) Handles btnAgregarProducto.Click
        Dim ef = New Efecto
        ef.tipo = 13
        ef.dtDetalle = CType(grDetalle.DataSource, DataTable)

        ef.ShowDialog()
        ef.Dispose()

    End Sub



    Private Sub grDetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grDetalle.EditingCell
        If (tbDetalle.ReadOnly = True) Then
            e.Cancel = True
            Return
        End If
        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index Or e.Column.Index = grDetalle.RootTable.Columns("CantidadCaja").Index) Then
            e.Cancel = False
        Else
            e.Cancel = True

        End If
    End Sub

    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub
    Private Sub grDetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grDetalle.CellValueChanged
        Dim lin As Integer = grDetalle.GetValue("Id")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin)



        If (e.Column.Index = grDetalle.RootTable.Columns("CantidadCaja").Index) Then

            _fnObtenerFilaDetalle(pos, lin)
            If (Not IsNumeric(grDetalle.GetValue("CantidadCaja")) Or grDetalle.GetValue("CantidadCaja").ToString = String.Empty) Then
                grDetalle.SetValue("CantidadCaja", 0)

            Else



                grDetalle.SetValue("Cantidad", grDetalle.GetValue("CantidadCaja") * grDetalle.GetValue("Conversion"))


                If (grDetalle.GetValue("Cantidad") > grDetalle.GetValue("Stock")) Then
                    ToastNotification.Show(Me, "La Cantidad = " + Str(grDetalle.GetValue("Cantidad")) + " es mayor al Stock del Producto = " + Str(grDetalle.GetValue("Stock")), img, 6000, eToastGlowColor.Red, eToastPosition.TopCenter)

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1

                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    grDetalle.SetValue("Cantidad", 1)
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCaja") = 1 / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion")

                    grDetalle.SetValue("cantidadCaja", (1 / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion")))



                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If
                End If


            End If


        End If





        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("Cantidad")) Or grDetalle.GetValue("Cantidad").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                grDetalle.SetValue("Cantidad", 1)
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCajas") = 1 / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion")
                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grDetalle.GetValue("Cantidad") > 0) Then

                    If (grDetalle.GetValue("Cantidad") <= grDetalle.GetValue("Stock")) Then

                        Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                        If (estado = 1) Then
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                        End If
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCaja") = grDetalle.GetValue("Cantidad") / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("conversion")


                        grDetalle.SetValue("CantidadCaja", (grDetalle.GetValue("Cantidad") / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("conversion")))



                    Else
                        ToastNotification.Show(Me, "La Cantidad = " + Str(grDetalle.GetValue("Cantidad")) + " es mayor al Stock del Producto = " + Str(grDetalle.GetValue("Stock")), img, 6000, eToastGlowColor.Red, eToastPosition.TopCenter)
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCaja") = 1 / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion")
                        Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")
                        grDetalle.SetValue("CantidadCaja", 1 / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion"))

                        grDetalle.SetValue("Cantidad", 1)

                        If (estado = 1) Then
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                        End If

                    End If


                Else

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1

                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("CantidadCaja") = 1 / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion")
                    grDetalle.SetValue("Cantidad", 1)
                    grDetalle.SetValue("CantidadCaja", 1 / CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Conversion"))

                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If
        End If
    End Sub

    Private Sub grDetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grDetalle.MouseClick
        If (tbDetalle.ReadOnly = True) Then
            Return
        End If

        Try
            If (grDetalle.RowCount >= 1) Then
                If (grDetalle.CurrentColumn.Index = grDetalle.RootTable.Columns("img").Index) Then
                    _prEliminarFila()
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub _prEliminarFila()
        If (grDetalle.Row >= 0) Then
            If (grDetalle.RowCount >= 1) Then
                Dim estado As Integer = grDetalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grDetalle.GetValue("Id")
                _fnObtenerFilaDetalle(pos, lin)

                If (estado = 0) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = -2

                End If
                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = -1
                End If

                grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))


            End If
        End If


    End Sub
    Public Sub ImprimirNotaSalida(id As Integer)
        Dim dt As DataTable
        dt = GenerarDatosDespachoReporte(id)
        Dim dtImage As DataTable = ObtenerImagenEmpresa()
        Dim NombreEmpresa As String = dtImage.Rows(0).Item("Nombre")
        Dim Direccion As String = dtImage.Rows(0).Item("Direccion")
        If (dtImage.Rows.Count > 0) Then
            Dim RutaGlobal As String = gs_CarpetaRaiz
            Dim Name As String = dtImage.Rows(0).Item(0)
            If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name)) Then
                Dim im As New Bitmap(New Bitmap(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name))
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(im)
                img.Save(Bin, Imaging.ImageFormat.Png)
                Bin.Dispose()
                For i As Integer = 0 To dt.Rows.Count - 1 Step 1


                    dt.Rows(i).Item("img") = Bin.GetBuffer
                Next
            End If


        End If
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If



        P_Global.Visualizador = New Visualizador

        Dim objrep As New Reporte_Despacho

        objrep.SetDataSource(dt)
        objrep.SetParameterValue("NombreEmpresa", NombreEmpresa)
        objrep.SetParameterValue("Ciudad", Direccion)
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(90)
        P_Global.Visualizador.Show() 'Comentar
        ''P_Global.Visualizador.BringToFront() 'Comentar
    End Sub
    Private Sub ReporteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReporteToolStripMenuItem.Click
        ImprimirNotaSalida(tbCodigo.Text)
    End Sub

    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        If (tbDetalle.ReadOnly = True) Then
            ImprimirNotaSalida(tbCodigo.Text)
        End If
    End Sub

    Private Sub btnConfirmarSalir_Click(sender As Object, e As EventArgs) Handles btnConfirmarSalir.Click
        _PMCargarBuscador()

    End Sub
End Class