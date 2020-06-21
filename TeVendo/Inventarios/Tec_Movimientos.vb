Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class Tec_Movimientos
#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public FilaSeleccionada As Boolean = False

    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean
    Dim FilaSelectLote As DataRow = Nothing
    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"


    Dim TablaImagenes As DataTable
    Dim TablaInventario As DataTable
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim gs_DirPrograma As String = ""
    Dim gs_RutaImg As String = ""
    Dim Lote As Boolean = False

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
            .Height = 200
        End With



    End Sub


    Public Sub _PMInhabilitar()
        btnNuevo.Visible = True
        btnModificar.Visible = True
        btnEliminar.Visible = True
        btnGrabar.Visible = False
        PanelNavegacion.Enabled = True
        JGrM_Buscador.Enabled = True


        _PMOLimpiarErrores()

        _PMOInhabilitar()
    End Sub

    Private Sub _PMHabilitar()
        JGrM_Buscador.Enabled = False
        _PMOHabilitar()
        tbNombreProducto.Focus()
    End Sub

    Public Sub ActualizarProductos()

        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            If (CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado") >= 0) Then
                CambiarEstado(CType(grDetalle.DataSource, DataTable).Rows(i).Item("ProductoId"), 0)

            End If


        Next

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
        tbDescripcion.Focus()


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

        Else
            '  Public _modulo As SideNavItem
            '_modulo.Select()
            _tab.Close()
        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()

        LeerConfiguracion()

        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "Gestion De Movimientos"
        P_Global._prCargarComboGenerico(cbDepositos, L_prListarDepositos(), "Id", "Codigo", "NombreDeposito", "NombreDeposito")
        P_Global._prCargarComboGenerico(cbTipoMovimiento, L_prListarTiposMovimientos(), "Id", "Codigo", "Descripcion", "TipoMovimiento")

        _PMIniciarTodo()
        _prAsignarPermisos()


        Dim blah As New Bitmap(New Bitmap(My.Resources.ic_c), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico



    End Sub
    Public Sub LeerConfiguracion()
        Dim dt As DataTable = L_prLeerConfiguracion()
        If (dt.Rows.Count > 0) Then
            Lote = dt.Rows(0).Item("Lote")
        End If
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


    Private Sub _prCargarDetalleVenta(_numi As String)
        Dim dt As New DataTable
        dt = L_prListarDetalleMovimiento(_numi)
        grDetalle.DataSource = dt
        grDetalle.RetrieveStructure()
        grDetalle.AlternatingColors = True
        'a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        '    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado 
        With grDetalle.RootTable.Columns("id")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 50
            .Visible = True
            .Caption = "Cod Producto"
        End With
        With grDetalle.RootTable.Columns("MovimientoId")
            .Width = 90
            .Visible = False
        End With


        With grDetalle.RootTable.Columns("Producto")
            .Width = 150
            .Caption = "Producto"
            .Visible = True
        End With


        With grDetalle.RootTable.Columns("Cantidad")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad".ToUpper
        End With

        With grDetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar".ToUpper
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = False
        End With
        If (Lote = True) Then
            With grDetalle.RootTable.Columns("Lote")
                .Width = 100
                .Caption = "lote".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 100
                .Caption = "FECHA VENC.".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = True
            End With
        Else

            With grDetalle.RootTable.Columns("Lote")
                .Width = 120
                .Caption = "lote".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = False
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 120
                .Caption = "FECHA VENC.".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .FormatString = "yyyy/MM/dd"
                .Visible = False
            End With


        End If
        With grDetalle.RootTable.Columns("stock")
            .Width = 120
            .Caption = "stock".ToUpper
            .Visible = False
        End With
        With grDetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
        End With
        CargarIconEstado()
    End Sub
    Public Sub CargarIconEstado()

        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1

            Dim Bin As New MemoryStream
            Dim img As New Bitmap(My.Resources.rowdelete, 30, 28)
            img.Save(Bin, Imaging.ImageFormat.Png)
                CType(grDetalle.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer


        Next

    End Sub
    Public Sub actualizarSaldoSinLote(ByRef dt As DataTable)
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 

        'a.yfnumi  ,a.yfcdprod1  ,a.yfcdprod2,Sum(b.iccven ) as stock
        'Dim _detalle As DataTable = CType(grdetalle.DataSource, DataTable)
        Dim _dtDetalle As DataTable = CType(grDetalle.DataSource, DataTable)
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim sum As Integer = 0
            Dim codProducto As Integer = dt.Rows(i).Item("Id")
            For j As Integer = 0 To _dtDetalle.Rows.Count - 1 Step 1

                Dim estado As Integer = _dtDetalle.Rows(j).Item("estado")
                If (estado = 0) Then
                    If (codProducto = _dtDetalle.Rows(j).Item("ProductoId")) Then
                        sum = sum + _dtDetalle.Rows(j).Item("Cantidad")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next

    End Sub
    Private Sub _prCargarProductos()
        Dim dt As New DataTable


        If (Lote = True And cbTipoMovimiento.Value = 3) Then '' es Salida Productos
            dt = L_prListarProductosLote(cbDepositos.Value)  ''1=Almacen
            actualizarSaldoSinLote(dt)
        Else
            dt = L_prListarProductosLote(cbDepositos.Value)  ''1=Almacen
        End If
        'p.Id , p.CodigoExterno, p.NombreProducto, p.DescripcionProducto, Sum(stock.Cantidad) as stock 

        grProducto.DataSource = dt
        grproducto.RetrieveStructure()
        grproducto.AlternatingColors = True
        With grProducto.RootTable.Columns("Id")
            .Width = 100
            .Caption = "Id"
            .Visible = True


        End With
        With grProducto.RootTable.Columns("CodigoExterno")
            .Width = 100
            .Caption = "CODIGOP"
            .Visible = False

        End With

        With grProducto.RootTable.Columns("estado")
            .Width = 100
            .Visible = False

        End With
        With grProducto.RootTable.Columns("NombreProducto")
            .Width = 350
            .Caption = "PRODUCTOS"
            .Visible = True

        End With

        With grProducto.RootTable.Columns("DescripcionProducto")
            .Width = 250
            .Visible = True
            .Caption = "DESCRIPCION"
        End With


        With grProducto.RootTable.Columns("stock")
            .Width = 150
            .Visible = True
            .FormatString = "0.00"
            .Caption = "STOCK"
        End With
        With grproducto
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
        _prAplicarCondiccionJanusSinLote()
    End Sub

    Public Sub _prAplicarCondiccionJanusSinLote()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProducto.RootTable.Columns("stock"), ConditionOperator.Equal, 0)
        'fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.Tan
        grProducto.RootTable.FormatConditions.Add(fc)
    End Sub

    Public Sub actualizarSaldo(ByRef dt As DataTable, CodProducto As Integer)
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 

        '      a.tbnumi ,a.tbtv1numi ,a.tbty5prod ,b.yfcdprod1 as producto,a.tbest ,a.tbcmin ,a.tbumin ,Umin .ycdes3 as unidad,a.tbpbas ,a.tbptot ,a.tbobs ,
        'a.tbpcos,a.tblote ,a.tbfechaVenc , a.tbptot2, a.tbfact ,a.tbhact ,a.tbuact,1 as estado,Cast(null as Image) as img,
        'Cast (0 as decimal (18,2)) as stock
        Dim _detalle As DataTable = CType(grDetalle.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim lote As String = dt.Rows(i).Item("Lote")
            Dim FechaVenc As Date = dt.Rows(i).Item("FechaVencimiento")
            Dim sum As Integer = 0
            For j As Integer = 0 To _detalle.Rows.Count - 1
                Dim estado As Integer = _detalle.Rows(j).Item("estado")
                If (estado = 0) Then
                    If (lote = _detalle.Rows(j).Item("Lote") And
                        FechaVenc = _detalle.Rows(j).Item("FechaVencimiento") And CodProducto = _detalle.Rows(j).Item("ProductoId")) Then
                        sum = sum + _detalle.Rows(j).Item("Cantidad")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next

    End Sub

    Private Sub _prCargarLotesDeProductos(CodProducto As Integer, nameProducto As String)
        If (cbDepositos.SelectedIndex < 0) Then
            Return
        End If

        'p.NombreProducto , a.Lote, a.FechaVencimiento, Sum(a.Cantidad) as stock
        Dim dt As New DataTable
        gPanelProductos.Text = nameProducto
        dt = LotesPorProducto(cbDepositos.Value, CodProducto)  ''1=Almacen
        actualizarSaldo(dt, CodProducto)
        grProducto.DataSource = dt
        grProducto.RetrieveStructure()
        grProducto.AlternatingColors = True
        With grProducto.RootTable.Columns("NombreProducto")
            .Width = 150
            .Visible = False

        End With
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 
        With grProducto.RootTable.Columns("Lote")
            .Width = 150
            .Caption = "Lote"
            .Visible = True

        End With
        With grProducto.RootTable.Columns("FechaVencimiento")
            .Width = 160
            .Caption = "Fecha Vencimiento"
            .FormatString = "yyyy/MM/dd"
            .Visible = True

        End With

        With grProducto.RootTable.Columns("stock")
            .Width = 150
            .Visible = True
            .Caption = "Stock"
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With


        With grProducto
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowPosition = TotalRowPosition.BottomFixed
            .VisualStyle = VisualStyle.Office2007
        End With
        _prAplicarCondiccionJanusLote()

    End Sub


    Public Sub _prAplicarCondiccionJanusLote()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProducto.RootTable.Columns("stock"), ConditionOperator.Equal, 0)
        fc.FormatStyle.BackColor = Color.Gold
        fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.White
        grProducto.RootTable.FormatConditions.Add(fc)

        Dim fc2 As GridEXFormatCondition
        fc2 = New GridEXFormatCondition(grProducto.RootTable.Columns("FechaVencimiento"), ConditionOperator.LessThanOrEqualTo, Now.Date)
        fc2.FormatStyle.BackColor = Color.Red
        fc2.FormatStyle.FontBold = TriState.True
        fc2.FormatStyle.ForeColor = Color.White
        grProducto.RootTable.FormatConditions.Add(fc2)

        grProducto.Select()
        grProducto.Col = 1
        grProducto.Row = grProducto.RowCount - 1
    End Sub

    Private Sub _prAddDetalleVenta()
        'a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        '    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado ,Sum(stock .Cantidad )as stock
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.rowdelete, 30, 28)
        img.Save(Bin, Imaging.ImageFormat.Png)
        CType(grDetalle.DataSource, DataTable).Rows.Add(_GenerarId() + 1, 0, 0, "", 0, "20200101", CDate("2020/01/01"), Bin.GetBuffer, 0, 0)
    End Sub
    Public Function _GenerarId()
        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(grDetalle.DataSource, DataTable).Rows(i).Item("Id")), 0, CType(grDetalle.DataSource, DataTable).Rows(i).Item("Id"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function

    Public Function _fnAccesible()
        Return tbDescripcion.ReadOnly = False
    End Function

    Private Sub _HabilitarProductos()
        gPanelProductos.Visible = True
        _prCargarProductos()
        grProducto.Focus()
        grProducto.MoveTo(grProducto.FilterRow)
        grProducto.Col = 1
    End Sub
    Private Sub _DesHabilitarProductos()

        tbNombreProducto.Clear()

        grDetalle.Select()
        grDetalle.Col = 4
        grDetalle.Row = grDetalle.RowCount - 1

        FilaSelectLote = Nothing
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

    Public Function _fnExisteProducto(idprod As Integer) As Boolean
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado")
            If (_idprod = idprod And estado >= 0) Then

                Return True
            End If
        Next
        Return False
    End Function

    Public Sub _prEliminarFila()
        If (grDetalle.Row >= 0) Then
            If (grDetalle.RowCount >= 1) Then
                Dim estado As Integer = grDetalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grDetalle.GetValue("Id")
                _fnObtenerFilaDetalle(pos, lin)

                CambiarEstado(grDetalle.GetValue("ProductoId"), 1)
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

    Public Function _ValidarCampos() As Boolean
        If (cbTipoMovimiento.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Seleccione Tipo Movimiento".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbTipoMovimiento.Focus()
            Return False

        End If
        If (cbDepositos.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Seleccion Deposito".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbDepositos.Focus()
            Return False
        End If

        If (grDetalle.RowCount <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            grDetalle.Focus()

            Return False

        End If
        If (grDetalle.RowCount = 1) Then
            If (CType(grDetalle.DataSource, DataTable).Rows(0).Item("ProductoId") = 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grDetalle.Focus()

                Return False
            End If
        End If
        Return True
    End Function

    Public Sub CambiarEstado(ProductoId As Integer, Estado As Integer)
        If (IsNothing(grProducto.DataSource)) Then
            Return

        End If
        For i As Integer = 0 To CType(grProducto.DataSource, DataTable).Rows.Count - 1 Step 1
            If (CType(grProducto.DataSource, DataTable).Rows(i).Item("Id") = ProductoId) Then
                CType(grProducto.DataSource, DataTable).Rows(i).Item("estado") = Estado
            End If

        Next
        grProducto.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grProducto.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.Equal, 1))
    End Sub


    Public Sub InsertarProductosSinLote(cantidad As Double)
        'a.id , a.MovimientoId, a.ProductoId, b.NombreProducto  As Producto, a.Cantidad,
        '    a.Lote, a.FechaVencimiento, CAST('' as image ) as img, 1 as estado ,Sum(stock .Cantidad )as stock
        Dim pos As Integer = -1
        If (grDetalle.Row < 0) Then
            _prAddDetalleVenta()
        End If



        Dim existe As Boolean = _fnExisteProducto(grProducto.GetValue("Id"))
        If ((Not existe)) Then
            If (grDetalle.GetValue("ProductoId") > 0) Then
                _prAddDetalleVenta()
            End If
            grDetalle.Row = grDetalle.RowCount - 1
            _fnObtenerFilaDetalle(pos, grDetalle.GetValue("Id"))
            If ((pos >= 0)) Then
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = grProducto.GetValue("Id")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Producto") = grProducto.GetValue("NombreProducto")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProducto.GetValue("stock")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = cantidad

                ''    _DesHabilitarProductos()


                CambiarEstado(grProducto.GetValue("Id"), 0)
                'grproducto.RemoveFilters()
                tbNombreProducto.Clear()
                tbNombreProducto.Focus()
            End If




        Else
            If (existe) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grProducto.RemoveFilters()
                tbNombreProducto.Focus()
            End If

        End If
    End Sub

    Public Sub InsertarProductosConLote()

        If (grDetalle.Row < 0) Then
            _prAddDetalleVenta()
        End If

        '      a.Id ,a.icibid ,a.iccprod ,b.yfcdprod1  as producto,a.Cantidad ,
        'a.iclot ,a.icfvenc ,Cast(null as image ) as img,1 as estado,
        '(Sum(inv.iccven )+a.Cantidad  ) as stock

        'a.yfnumi  ,a.yfcdprod1  ,a.yfcdprod2,Sum(b.iccven ) as stock 
        Dim pos As Integer = -1
        grDetalle.Row = grDetalle.RowCount - 1
        _fnObtenerFilaDetalle(pos, grDetalle.GetValue("Id"))
        Dim posProducto As Integer = grProducto.Row
        FilaSelectLote = CType(grProducto.DataSource, DataTable).Rows(posProducto)


        If (grProducto.GetValue("stock") > 0) Then
            _prCargarLotesDeProductos(grProducto.GetValue("Id"), grProducto.GetValue("NombreProducto"))
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "El Producto: ".ToUpper + grProducto.GetValue("NombreProducto") + " NO CUENTA CON STOCK DISPONIBLE", img, 5000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            FilaSelectLote = Nothing
        End If

    End Sub
    Public Function _fnExisteProductoConLote(idprod As Integer, lote As String, fechaVenci As Date) As Boolean
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado")

            Dim _LoteDetalle As String = CType(grDetalle.DataSource, DataTable).Rows(i).Item("Lote")
            Dim _FechaVencDetalle As Date = CType(grDetalle.DataSource, DataTable).Rows(i).Item("FechaVencimiento")
            If (_idprod = idprod And estado >= 0 And lote = _LoteDetalle And fechaVenci = _FechaVencDetalle) Then

                Return True
            End If
        Next
        Return False
    End Function

    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grDetalle.EditingCell
        If (_fnAccesible()) Then

            'Habilitar solo las columnas de Precio, %, Monto y Observación
            If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
                e.Cancel = False
            Else
                If ((e.Column.Index = grDetalle.RootTable.Columns("Lote").Index Or
                    e.Column.Index = grDetalle.RootTable.Columns("FechaVencimiento").Index) And
                Lote = True) Then
                    e.Cancel = False
                Else
                    e.Cancel = True
                End If
            End If


        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub grdetalle_KeyDown(sender As Object, e As KeyEventArgs) Handles grDetalle.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If

        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grDetalle.Col
            f = grDetalle.Row

            If (grDetalle.Col = grDetalle.RootTable.Columns("Cantidad").Index) Then
                If (grDetalle.GetValue("Producto") <> String.Empty) Then

                    tbNombreProducto.Focus()
                Else
                    ToastNotification.Show(Me, "Seleccione Producto", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
            If (grDetalle.Col = grDetalle.RootTable.Columns("Producto").Index) Then
                If (grDetalle.GetValue("Producto") <> String.Empty) Then

                    tbNombreProducto.Focus()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
salirIf:
        End If

        If (e.KeyData = Keys.Control + Keys.Enter And grDetalle.Row >= 0 And
            grDetalle.Col = grDetalle.RootTable.Columns("Producto").Index) Then
            Dim indexfil As Integer = grDetalle.Row
            Dim indexcol As Integer = grDetalle.Col
            tbNombreProducto.Focus()

        End If
        If (e.KeyData = Keys.Escape And grDetalle.Row >= 0) Then

            _prEliminarFila()


        End If



    End Sub

    Private Sub grproducto_KeyDown(sender As Object, e As KeyEventArgs) Handles grProducto.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grProducto.Col
            f = grProducto.Row
            If (f >= 0) Then

                If (IsNothing(FilaSelectLote)) Then
                    ''''''''''''''''''''''''
                    If (Lote = True) Then
                        If (cbTipoMovimiento.Value = 4) Then ''' Aqui pregunto si es con lote y tambien si es una 
                            ''entrada debe solo insertar solo el producto y no seguir con los lotes 
                            Dim ef = New Efecto


                            ef.tipo = 5
                            ef.NombreProducto = grProducto.GetValue("NombreProducto")
                            ef.StockActual = grProducto.GetValue("stock")
                            ef.TipoMovimiento = cbTipoMovimiento.Value
                            ef.ShowDialog()
                            Dim bandera As Boolean = False
                            bandera = ef.band
                            If (bandera = True) Then
                                InsertarProductosSinLote(ef.CantidadTransaccion)

                            End If


                        Else
                            InsertarProductosConLote()
                        End If

                    Else

                        Dim ef = New Efecto


                        ef.tipo = 5
                        ef.NombreProducto = grProducto.GetValue("NombreProducto")
                        ef.StockActual = grProducto.GetValue("stock")
                        ef.TipoMovimiento = cbTipoMovimiento.Value
                        ef.ShowDialog()
                        Dim bandera As Boolean = False
                        bandera = ef.band
                        If (bandera = True) Then
                            InsertarProductosSinLote(ef.CantidadTransaccion)

                        End If
                    End If
                    '''''''''''''''
                Else



                    Dim numiProd As Integer = FilaSelectLote.Item("Id")
                    Dim lote As String = grProducto.GetValue("Lote")
                    Dim FechaVenc As Date = grProducto.GetValue("FechaVencimiento")
                    If (Not _fnExisteProductoConLote(numiProd, lote, FechaVenc)) Then
                        Dim ef = New Efecto


                        ef.tipo = 5
                        ef.NombreProducto = grProducto.GetValue("NombreProducto")
                        ef.StockActual = grProducto.GetValue("stock")
                        ef.TipoMovimiento = cbTipoMovimiento.Value
                        ef.ShowDialog()
                        Dim bandera As Boolean = False
                        bandera = ef.band
                        Dim CantidadVenta As Double = 0
                        If (bandera = True) Then
                            CantidadVenta = ef.CantidadTransaccion

                            If (grDetalle.GetValue("ProductoId") > 0) Then
                                _prAddDetalleVenta()
                            End If
                            Dim pos As Integer = -1
                            grDetalle.Row = grDetalle.RowCount - 1
                            _fnObtenerFilaDetalle(pos, grDetalle.GetValue("Id"))
                            'b.yfcdprod1, a.iclot, a.icfven, a.iccven
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProductoId") = FilaSelectLote.Item("Id")
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Producto") = FilaSelectLote.Item("NombreProducto")
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = CantidadVenta


                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProducto.GetValue("stock")

                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Lote") = grProducto.GetValue("Lote")
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("FechaVencimiento") = grProducto.GetValue("FechaVencimiento")


                            FilaSelectLote = Nothing
                            _DesHabilitarProductos()
                            tbNombreProducto.Focus()
                            _prCargarProductos()


                        End If




                    Else
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "El producto con el lote ya existe modifique su cantidad".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    End If



                End If


            End If
        End If
        If e.KeyData = Keys.Escape Then
            CType(grProducto.DataSource, DataTable).Rows.Clear()

            _DesHabilitarProductos()
        End If
    End Sub

    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grDetalle.CellValueChanged

        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("Cantidad")) Or grDetalle.GetValue("Cantidad").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                Dim lin As Integer = grDetalle.GetValue("Id")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1

                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grDetalle.GetValue("Cantidad") > 0) Then
                    Dim lin As Integer = grDetalle.GetValue("Id")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                Else
                    Dim lin As Integer = grDetalle.GetValue("Id")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If
        End If
    End Sub

    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grDetalle.CellEdited
        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("Cantidad")) Or grDetalle.GetValue("Cantidad").ToString = String.Empty) Then

                grDetalle.SetValue("Cantidad", 1)
            Else
                If (grDetalle.GetValue("Cantidad") > 0) Then
                    Dim stock As Double = grDetalle.GetValue("stock")
                    If (grDetalle.GetValue("Cantidad") > stock And cbTipoMovimiento.Value <> 4) Then
                        Dim lin As Integer = grDetalle.GetValue("Id")
                        Dim pos As Integer = -1
                        _fnObtenerFilaDetalle(pos, lin)
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = stock
                        grDetalle.SetValue("Cantidad", stock)
                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "La cantidad que se quiere sacar es mayor a la que existe en el stock solo puede Sacar : ".ToUpper + Str(stock).Trim,
                          img,
                          5000,
                          eToastGlowColor.Blue,
                          eToastPosition.BottomLeft)
                    End If
                Else

                    grDetalle.SetValue("Cantidad", 1)

                End If
            End If
        End If
    End Sub

    Private Sub grdetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grDetalle.MouseClick
        If (Not _fnAccesible()) Then
            Return
        End If

        If (grDetalle.RowCount >= 1) Then
            If (grDetalle.CurrentColumn.Index = grDetalle.RootTable.Columns("img").Index) Then
                _prEliminarFila()
            End If
        End If


    End Sub

    Private Sub grdetalle_Enter(sender As Object, e As EventArgs) Handles grDetalle.Enter
        If (grDetalle.RowCount > 0) Then
            grDetalle.Select()
            grDetalle.Col = 3
            grDetalle.Row = 0
        End If
    End Sub

    Private Sub cbConcepto_ValueChanged(sender As Object, e As EventArgs) Handles cbTipoMovimiento.ValueChanged



        If (_fnAccesible() And tbCodigo.Text = String.Empty And Not IsNothing(grDetalle.DataSource)) Then

            CType(grDetalle.DataSource, DataTable).Rows.Clear()


        End If

    End Sub

#End Region

#Region "METODOS SOBRECARGADOS"


    Public Sub _PMOHabilitar()
        '     (@Id,@CodigoExterno ,@CodigoBarras ,@NombreProducto ,@DescripcionProducto ,
        '@StockMinimo ,@estado ,@CategoriaId ,@EmpresaId ,@ProveedorId ,@MarcaId ,
        '@AttributoId ,@FamiliaId ,
        '@UnidadVentaId ,@UnidadMaximaId ,@Conversion ,@newFecha,@newHora,@usuario )
        tbNombreProducto.Visible = True
        lbNombreProductos.Visible = True
        gPanelProductos.Visible = True


        cbDepositos.ReadOnly = False
        cbTipoMovimiento.ReadOnly = False
        tbDescripcion.ReadOnly = False
        tbFechaTransaccion.IsInputReadOnly = False
        _HabilitarProductos()

    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreProducto.Visible = False
        lbNombreProductos.Visible = False
        gPanelProductos.Visible = False


        cbDepositos.ReadOnly = True
        cbTipoMovimiento.ReadOnly = True
        tbDescripcion.ReadOnly = True
        tbFechaTransaccion.IsInputReadOnly = True

    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbNombreProducto.Text = ""
        tbDescripcion.Text = ""
        tbFechaTransaccion.Value = Now.Date

        seleccionarPrimerItemCombo(cbDepositos)
        seleccionarPrimerItemCombo(cbTipoMovimiento)

        tbDescripcion.Focus()
        _prCargarDetalleVenta(-1)


    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()

        tbDescripcion.BackColor = Color.White
        cbDepositos.BackColor = Color.White
        cbTipoMovimiento.BackColor = Color.White


    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        'ByRef _numi As String, ConceptoId As Integer, DepositoId As Integer,Observacion as String,
        'Estado as integer, FechaDocumento As String, _dtDetalle As DataTable
        Dim res As Boolean
        Try
            res = L_prMovimientoInsertar(tbCodigo.Text, cbTipoMovimiento.Value, cbDepositos.Value, tbDescripcion.Text,
                                         1, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"), CType(grDetalle.DataSource, DataTable))

            If res Then


                ToastNotification.Show(Me, "Codigo de Movimiento ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar el Movimiento".ToUpper, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Movimiento".ToUpper + " " + ex.Message, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = L_prMovimientoActualizar(tbCodigo.Text, cbTipoMovimiento.Value, cbDepositos.Value, tbDescripcion.Text,
                                         1, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"), CType(grDetalle.DataSource, DataTable))


            If Res Then

                ToastNotification.Show(Me, "Codigo de Movimiento ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el Movimiento".ToUpper, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar el Movimiento".ToUpper + " " + ex.Message, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbDescripcion.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Movimiento " + tbDescripcion.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_Movimientos")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Movimiento ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el Movimiento".ToUpper + " " + ex.Message, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "

        If (cbTipoMovimiento.SelectedIndex < 0) Then
            cbTipoMovimiento.BackColor = Color.Red
            MEP.SetError(cbTipoMovimiento, "Seleccione un Tipo Movimiento")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Empresa"
            _ok = False
        Else
            cbTipoMovimiento.BackColor = Color.White
            MEP.SetError(cbTipoMovimiento, "")
        End If

        If (cbDepositos.SelectedIndex < 0) Then
            cbDepositos.BackColor = Color.Red
            MEP.SetError(cbDepositos, "Seleccione un Deposito")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Categoria"
            _ok = False
        Else
            cbDepositos.BackColor = Color.White
            MEP.SetError(cbDepositos, "")
        End If

        If (tbFechaTransaccion.Text.Length <= 0) Then
            tbFechaTransaccion.BackColor = Color.Red
            MEP.SetError(tbFechaTransaccion, "Ingrese una Fecha Valida")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Stock Minimo"
            _ok = False
        Else
            tbFechaTransaccion.BackColor = Color.White
            MEP.SetError(tbFechaTransaccion, "")
        End If

        MHighlighterFocus.UpdateHighlights()

        If (grDetalle.RowCount <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            grDetalle.Focus()

            Return False

        End If
        If (grDetalle.RowCount = 1) Then
            If (CType(grDetalle.DataSource, DataTable).Rows(0).Item("iccprod") = 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grDetalle.Focus()

                Return False
            End If
        End If

        If (cbTipoMovimiento.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbTipoMovimiento.Focus()
            Return _ok
        End If
        If (cbDepositos.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbDepositos.Focus()
            Return _ok
        End If
        If (tbFechaTransaccion.Text.Length <= 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbFechaTransaccion.Focus()
            Return _ok
        End If


        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Movimientos")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'a.Id  as id, a.FechaDocumento  As fdoc, a.ConceptoId  As concep, b.Descripcion  As nconcep, a.Observacion  As obs, 
        '           a.Estado  as est, a.DepositoId  as alm 
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("fdoc", True, "Fecha Transaccion", 100))
        listEstCeldas.Add(New Celda("concep", False))
        listEstCeldas.Add(New Celda("nconcep", True, " Tipo Movimiento", 120))
        listEstCeldas.Add(New Celda("obs", True, " Descripcion Movimiento", 250))

        listEstCeldas.Add(New Celda("est", False, "Estado", 70))
        listEstCeldas.Add(New Celda("alm", False, "Estado", 150))
        listEstCeldas.Add(New Celda("NombreDeposito", True, "Deposito", 150))

        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        'a.Id  as id, a.FechaDocumento  As fdoc, a.ConceptoId  As concep, b.Descripcion  As nconcep, a.Observacion  As obs, 
        '           a.Estado  as est, a.DepositoId  as alm   
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbFechaTransaccion.Value = .GetValue("fdoc")
            cbTipoMovimiento.Value = .GetValue("concep")
            tbDescripcion.Text = .GetValue("obs").ToString
            cbDepositos.Value = .GetValue("alm")


        End With
        _prCargarDetalleVenta(tbCodigo.Text)
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub



    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()

        Else
            '  Public _modulo As SideNavItem
            _modulo.Select()
            _tab.Close()
        End If
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _PMNuevo()

    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        _PMModificar()
        ActualizarProductos()
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
        TabControlPrincipal.SelectedTabIndex = 1
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


    Private Sub Panel1_MouseHover(sender As Object, e As EventArgs) Handles btnSi.MouseHover
        btnSi.BackColor = Color.FromArgb(30, 199, 165)
    End Sub

    Private Sub Panel1_MouseLeave(sender As Object, e As EventArgs) Handles btnSi.MouseLeave
        btnSi.BackColor = Color.FromArgb(26, 179, 148)
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        TabControlPrincipal.SelectedTabIndex = 0
        btnNuevo.PerformClick()

    End Sub

    Private Sub JGrM_Buscador_KeyDown(sender As Object, e As KeyEventArgs) Handles JGrM_Buscador.KeyDown
        If (e.KeyCode = Keys.Enter) Then

            TabControlPrincipal.SelectedTabIndex = 0

        End If
    End Sub

    Private Sub tbNombreProducto_TextChanged(sender As Object, e As EventArgs) Handles tbNombreProducto.TextChanged
        If (tbNombreProducto.Text = String.Empty) Then
            grProducto.RootTable.RemoveFilter()
            grProducto.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grProducto.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.Equal, 1))




        Else
            'grProducto.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grProducto.RootTable.Columns("NombreProducto"), Janus.Windows.GridEX.ConditionOperator.Contains, tbNombreProducto.Text))
            Dim Filter As GridEXFilterCondition
            Filter = New GridEXFilterCondition(grProducto.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.Equal, 1)
            Filter.AddCondition(LogicalOperator.And, New GridEXFilterCondition(grProducto.RootTable.Columns("NombreProducto"), Janus.Windows.GridEX.ConditionOperator.Contains, tbNombreProducto.Text))
            grProducto.RootTable.FilterCondition = Filter
        End If
    End Sub

    Private Sub tbNombreProducto_KeyDown(sender As Object, e As KeyEventArgs) Handles tbNombreProducto.KeyDown
        If grProducto.RowCount <= 0 Then
            Return

        End If

        grProducto.Row = 0
        If (e.KeyData = Keys.Enter) Then

            If (Not _fnAccesible()) Then
                Return
            End If
            If (e.KeyData = Keys.Enter) Then
                Dim f, c As Integer
                c = grProducto.Col
                f = grProducto.Row
                If (f >= 0) Then

                    If (IsNothing(FilaSelectLote)) Then
                        ''''''''''''''''''''''''
                        If (Lote = True) Then
                            If (cbTipoMovimiento.Value = 4) Then ''' Aqui pregunto si es con lote y tambien si es una 
                                ''entrada debe solo insertar solo el producto y no seguir con los lotes 
                                Dim ef = New Efecto


                                ef.tipo = 5
                                ef.NombreProducto = grProducto.GetValue("NombreProducto")
                                ef.StockActual = grProducto.GetValue("stock")
                                ef.TipoMovimiento = cbTipoMovimiento.Value
                                ef.ShowDialog()
                                Dim bandera As Boolean = False
                                bandera = ef.band
                                If (bandera = True) Then
                                    InsertarProductosSinLote(ef.CantidadTransaccion)

                                End If

                            Else
                                InsertarProductosConLote()
                            End If

                        Else

                            Dim ef = New Efecto


                            ef.tipo = 5
                            ef.NombreProducto = grProducto.GetValue("NombreProducto")
                            ef.StockActual = grProducto.GetValue("stock")
                            ef.TipoMovimiento = cbTipoMovimiento.Value
                            ef.ShowDialog()
                            Dim bandera As Boolean = False
                            bandera = ef.band
                            If (bandera = True) Then
                                InsertarProductosSinLote(ef.CantidadTransaccion)

                            End If
                        End If
                        '''''''''''''''



                    End If
                End If
            End If

        End If
        If (e.KeyData = Keys.Down) Then
            grProducto.Focus()
        End If

    End Sub
#End Region
End Class