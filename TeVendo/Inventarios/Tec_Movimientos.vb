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
            _PMSalir()
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
            '_modulo.Select()
            TabControlPrincipal.SelectedTabIndex = 1
            '_tab.Close()
        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()

        LeerConfiguracion()

        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "Gestion De Movimientos"
        Dim dt As DataTable = L_prListarDepositos()
        P_Global._prCargarComboGenerico(cbDepositos, dt, "Id", "Codigo", "NombreDeposito", "NombreDeposito")
        P_Global._prCargarComboGenerico(cbDepositoDestino, dt, "Id", "Codigo", "NombreDeposito", "NombreDeposito")
        P_Global._prCargarComboGenerico(cbTipoMovimiento, L_prListarTiposMovimientos(), "Id", "Codigo", "Descripcion", "TipoMovimiento")

        _PMIniciarTodo()
        _prAsignarPermisos()





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
        With grDetalle.RootTable.Columns("precio")
            .Width = 80
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "precio".ToUpper
        End With
        With grDetalle.RootTable.Columns("Total")
            .Width = 80
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total"
            .AggregateFunction = AggregateFunction.Sum
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
            .TotalRow = InheritableBoolean.True
            .TotalRowFormatStyle.BackColor = Color.Gold
            .TotalRowFormatStyle.ForeColor = Color.Black
            .TotalRowFormatStyle.FontBold = TriState.True
            .TotalRowFormatStyle.FontSize = 11
            .TotalRowPosition = TotalRowPosition.BottomFixed
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




        If (e.KeyData = Keys.Escape And grDetalle.Row >= 0) Then

            _prEliminarFila()

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
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("total") = grDetalle.GetValue("precio")
                grDetalle.SetValue("total", grDetalle.GetValue("precio"))
            Else
                If (grDetalle.GetValue("Cantidad") > 0) Then
                    Dim lin As Integer = grDetalle.GetValue("Id")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("total") = grDetalle.GetValue("Cantidad") * grDetalle.GetValue("precio")
                    grDetalle.SetValue("total", grDetalle.GetValue("Cantidad") * grDetalle.GetValue("precio"))
                Else
                    Dim lin As Integer = grDetalle.GetValue("Id")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("total") = grDetalle.GetValue("precio")
                    grDetalle.SetValue("total", grDetalle.GetValue("precio"))
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

    Private Sub cbConcepto_ValueChanged(sender As Object, e As EventArgs)



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


        btnSeleccionarProducto.Visible = True
        cbDepositos.ReadOnly = False
        cbTipoMovimiento.ReadOnly = False
        tbDescripcion.ReadOnly = False
        tbFechaTransaccion.ReadOnly = False


    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        btnSeleccionarProducto.Visible = False


        cbDepositos.ReadOnly = True
        cbTipoMovimiento.ReadOnly = True
        tbDescripcion.ReadOnly = True
        tbFechaTransaccion.ReadOnly = True

    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""

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
    Function _prGuardarTraspaso() As Boolean
        Dim numi As String = ""
        Dim res As Boolean
        '= L_prMovimientoChoferGrabar(numi, tbFecha.Value.ToString("yyyy/MM/dd"), cbConcepto.Value, tbObservacion.Text, cbAlmacenOrigen.Value, cbDepositoDestino.Value, 0, CType(grDetalle.DataSource, DataTable))
        res = L_prMovimientoInsertar(numi, cbTipoMovimiento.Value, cbDepositos.Value, tbDescripcion.Text,
                                         1, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"), CType(grDetalle.DataSource, DataTable), cbDepositoDestino.Value, 0)

        If res Then

            Dim numDestino As String = ""
            Dim resDestino As Boolean
            '= L_prMovimientoChoferGrabar(numDestino, tbFecha.Value.ToString("yyyy/MM/dd"), 5, tbObservacion.Text, cbDepositoDestino.Value, cbAlmacenOrigen.Value, numi, CType(grDetalle.DataSource, DataTable))
            resDestino = L_prMovimientoInsertar(numDestino, 7, cbDepositoDestino.Value, tbDescripcion.Text,
                                         1, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"), CType(grDetalle.DataSource, DataTable), cbDepositos.Value, numi)

            If resDestino Then

                ReporteVenta(numi)

                ToastNotification.Show(Me, "Codigo de Movimiento ".ToUpper + Str(numi) + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

                Return True
            End If

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "Error al Insertar Movimiento".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

            Return False
        End If
    End Function
    Public Function _PMOGrabarRegistro() As Boolean
        'ByRef _numi As String, ConceptoId As Integer, DepositoId As Integer,Observacion as String,
        'Estado as integer, FechaDocumento As String, _dtDetalle As DataTable

        If (cbTipoMovimiento.Value = 8) Then

            If (cbDepositos.Value = cbDepositoDestino.Value) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El deposito Destino debe ser Diferente al Origen".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
                Return False
            End If
            Return _prGuardarTraspaso()
        End If

        Dim res As Boolean
        Try
            res = L_prMovimientoInsertar(tbCodigo.Text, cbTipoMovimiento.Value, cbDepositos.Value, tbDescripcion.Text,
                                         1, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"), CType(grDetalle.DataSource, DataTable), 0, 0)

            If res Then
                ReporteVenta(tbCodigo.Text)

                ToastNotification.Show(Me, "Codigo de Movimiento ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Error al guardar el Movimiento".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Error al guardar el Movimiento".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = L_prMovimientoActualizar(tbCodigo.Text, cbTipoMovimiento.Value, cbDepositos.Value, tbDescripcion.Text,
                                         1, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"), CType(grDetalle.DataSource, DataTable))


            If Res Then
                ReporteVenta(tbCodigo.Text)
                ToastNotification.Show(Me, "Codigo de Movimiento ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Error al guardar el Movimiento".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Error al modificar el Movimiento".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

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
                res = L_prBorrarRegistroMovimiento(tbCodigo.Text, mensajeError, "MAM_Movimientos", cbTipoMovimiento.Value)
                If res Then

                    ToastNotification.Show(Me, "Codigo de Movimiento ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Error al eliminar el Movimiento".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

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
            If (CType(grDetalle.DataSource, DataTable).Rows(0).Item("ProductoId") = 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grDetalle.Focus()

                Return False
            End If
        End If

        If (cbTipoMovimiento.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.mensaje, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbTipoMovimiento.Focus()
            Return _ok
        End If
        If (cbDepositos.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.mensaje, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbDepositos.Focus()
            Return _ok
        End If
        If (tbFechaTransaccion.Text.Length <= 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.mensaje, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
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




    Private Sub VerToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles VerToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then
            TabControlPrincipal.SelectedTabIndex = 0
        End If

    End Sub

    Private Sub EditarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditarToolStripMenuItem.Click
        If (JGrM_Buscador.Row >= 0) Then
            If (cbTipoMovimiento.Value = 8) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "No Se puede Modificar un Movimiento de Traspaso. Elimine el Movimiento y Vuelva a Crearlo".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Else
                TabControlPrincipal.SelectedTabIndex = 0
                btnModificar.PerformClick()
            End If


        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then

            btnEliminar.PerformClick()

        End If
    End Sub

    Private Sub btnSeleccionarProducto_Click(sender As Object, e As EventArgs) Handles btnSeleccionarProducto.Click
        Dim ef = New Efecto
        ef.tipo = 14
        ef.dtDetalle = CType(grDetalle.DataSource, DataTable)
        ef.TipoMovimientoId = cbTipoMovimiento.Value
        ef.DepositoId = cbDepositos.Value
        ef.Lotebool = Lote
        ef.ShowDialog()
    End Sub

    Private Sub P_GenerarReporte(numi As String)
        Dim dt As DataTable = ListarReporteMovimientoProductos(numi)


        Dim dtImage As DataTable = ObtenerImagenEmpresa()
        If (dtImage.Rows.Count > 0) Then
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

        Dim objrep As New Reporte_Movimiento

        objrep.SetDataSource(dt)

        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(90)
        P_Global.Visualizador.Show() 'Comentar
        ''P_Global.Visualizador.BringToFront() 'Comentar


    End Sub
    Public Sub ReporteVenta(Id As String)
        Dim ef = New Efecto


        ef.tipo = 8
        ef.titulo = "Comprobante de Movimiento"
        ef.descripcion = "¿Desea Generar Reporte Movimiento#" + Id + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_GenerarReporte(Id)


        End If

    End Sub

    Private Sub ReporteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReporteToolStripMenuItem.Click
        P_GenerarReporte(JGrM_Buscador.GetValue("Id"))
    End Sub

    Private Sub cbTipoMovimiento_ValueChanged(sender As Object, e As EventArgs) Handles cbTipoMovimiento.ValueChanged
        If (cbTipoMovimiento.SelectedIndex >= 0) Then
            If (cbTipoMovimiento.Value = 8) Then ''''Movimiento 6=Traspaso Salida
                If (CType(cbDepositos.DataSource, DataTable).Rows.Count > 1) Then
                    lbDepositoDestino.Visible = True
                    cbDepositoDestino.Visible = True
                    lbDepositoOrigen.Text = "Deposito Origen"
                    lbDepositoDestino.Text = "Deposito Destino"
                    cbDepositoDestino.SelectedIndex = 1
                    If (Not _fnAccesible()) Then
                        btnModificar.Enabled = False
                    End If
                Else
                    lbDepositoDestino.Visible = False
                    cbDepositoDestino.Visible = False
                    lbDepositoOrigen.Text = "Deposito:"
                    If (Not _fnAccesible()) Then
                        btnModificar.Enabled = True
                    End If
                End If
            Else
                'btnModificar.Enabled = True
                lbDepositoDestino.Visible = False
                cbDepositoDestino.Visible = False
                lbDepositoOrigen.Text = "Deposito:"

            End If
            If (_fnAccesible() And tbCodigo.Text = String.Empty And Not (IsNothing(grDetalle.DataSource))) Then
                CType(grDetalle.DataSource, DataTable).Rows.Clear()


            End If

        End If
    End Sub
#End Region
End Class