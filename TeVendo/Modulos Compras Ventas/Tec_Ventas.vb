Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Imports Facturacion

Public Class Tec_Ventas
#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public FilaSeleccionada As Boolean = False

    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean
    Dim FilaSelectLote As DataRow = Nothing
    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"
    Dim Lote As Boolean = False
    Dim IdVendedor As Integer = 0
    Dim IdCliente As Integer = 0

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
            '_tab.Close()
            TabControlPrincipal.SelectedTabIndex = 1
            'Me.Close()

        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()

        LeerConfiguracion()

        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "Gestion De Ventas"
        P_Global._prCargarComboGenerico(cbSucursal, L_fnGeneralSucursales(), "aanumi", "Codigo", "aabdes", "Sucursal")

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
        dt = ListaVentasDetalles(_numi)
        grDetalle.DataSource = dt
        grDetalle.RetrieveStructure()
        grDetalle.AlternatingColors = True
        '     a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        '     a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '     1 As estado, cast('' as image ) as img
        '     ,   as stock
        With grDetalle.RootTable.Columns("Id")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("VentaId")
            .Width = 100
            .Visible = False

        End With


        With grDetalle.RootTable.Columns("ProductoId")
            .Width = 30
            .Visible = True
            .Caption = "Cod Producto"
        End With

        With grDetalle.RootTable.Columns("Producto")
            .Width = 150
            .Caption = "Producto"
            .Visible = True
        End With

        With grDetalle.RootTable.Columns("Cantidad")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad".ToUpper
        End With
        With grDetalle.RootTable.Columns("Tipo")
            .Width = 100
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("TipoNombre")
            .Width = 40
            .Visible = True
            .Caption = "Tipo"
        End With
        With grDetalle.RootTable.Columns("KitId")
            .Width = 100
            .Visible = False

        End With
        With grDetalle.RootTable.Columns("KitNombre")
            .Width = 80
            .Visible = True
            .Caption = "Kit"
            .WordWrap = True
            .MaxLines = 2
        End With

        With grDetalle.RootTable.Columns("Precio")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "Precio"
            .FormatString = "0.00"
        End With

        With grDetalle.RootTable.Columns("SubTotal")
            .Width = 60
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "SubTotal"
            .FormatString = "0.00"
        End With
        '     a.Id , a.VentaId, a.ProductoId, p.NombreProducto As Producto, a.Cantidad, a.Precio, a.SubTotal,
        '     a.ProcentajeDescuento, a.MontoDescuento, a.Total, a.Detalle, a.PrecioCosto, a.Lote, a.FechaVencimiento,
        '     1 As estado, cast('' as image ) as img
        '     ,   as stock
        With grDetalle.RootTable.Columns("CantidadKit")
            .Width = 40
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "CantidadKit"
        End With
        With grDetalle.RootTable.Columns("ProcentajeDescuento")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0"
            .Caption = "%.Descuento".ToUpper
        End With

        With grDetalle.RootTable.Columns("MontoDescuento")
            .Width = 70
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "M.Descuento".ToUpper
        End With


        With grDetalle.RootTable.Columns("Total")
            .Width = 60
            .Visible = True
            .Caption = "Total".ToUpper
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With

        With grDetalle.RootTable.Columns("Detalle")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grDetalle.RootTable.Columns("PrecioCosto")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .Visible = False
        End With
        With grDetalle.RootTable.Columns("stock")
            .Width = 90
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .Visible = False
        End With


        With grDetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        If (tbGlosa.ReadOnly = False) Then
            With grDetalle.RootTable.Columns("img")
                .Width = 80
                .Caption = "Eliminar".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
        Else
            With grDetalle.RootTable.Columns("img")
                .Width = 80
                .Caption = "Eliminar".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = False
            End With

        End If


        If (Lote = True) Then
            With grDetalle.RootTable.Columns("Lote")
                .Width = 60
                .Caption = "lote".ToUpper
                .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
                .Visible = True
            End With
            With grDetalle.RootTable.Columns("FechaVencimiento")
                .Width = 70
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
        Return tbGlosa.ReadOnly = False
    End Function



    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer, Tipo As Integer)
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi And CType(grDetalle.DataSource, DataTable).Rows(i).Item("Tipo") = Tipo) Then
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
                _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
                Dim TipoKit As Integer = grDetalle.GetValue("Tipo")

                If (TipoKit = 1) Then  ''Productos


                    If (estado = 0) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = -2

                    End If
                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = -1
                    End If

                    grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
                Else ''Kits

                    Dim KitId As Integer = grDetalle.GetValue("KitId")
                    Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)

                    For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                        If (dt.Rows(i).Item("KitId") = KitId) Then
                            If (estado = 0) Then
                                CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado") = -2

                            End If
                            If (estado = 1) Then
                                CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado") = -1
                            End If

                        End If

                    Next







                    grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
                End If



            End If
        End If
        _prCalcularPrecioTotal()

    End Sub

    Public Function _ValidarCampos() As Boolean
        If (IdVendedor <= 0) Then
            ToastNotification.Show(Me, "Seleccione Vendedor".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbVendedor.Focus()
            Return False

        End If
        If (IdCliente <= 0) Then
            ToastNotification.Show(Me, "Seleccione Cliente".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbCliente.Focus()
            Return False

        End If
        If (cbSucursal.SelectedIndex < 0) Then
            ToastNotification.Show(Me, "Seleccion Sucursal".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbSucursal.Focus()
            Return False
        End If

        If (grDetalle.RowCount <= 0) Then
            ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            grDetalle.Focus()

            Return False

        End If
        If (grDetalle.RowCount = 1) Then
            If (CType(grDetalle.DataSource, DataTable).Rows(0).Item("ProductoId") = 0) Then

                ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grDetalle.Focus()

                Return False
            End If
        End If
        Return True
    End Function






    Public Function _fnExisteProductoConLote(idprod As Integer, lote As String, fechaVenci As Date) As Boolean
        For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("ProductoId")
            Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(i).Item("estado")
            _prCalcularPrecioTotal()
            Dim _LoteDetalle As String = CType(grDetalle.DataSource, DataTable).Rows(i).Item("Lote")
            Dim _FechaVencDetalle As Date = CType(grDetalle.DataSource, DataTable).Rows(i).Item("FechaVencimiento")
            If (_idprod = idprod And estado >= 0 And lote = _LoteDetalle And fechaVenci = _FechaVencDetalle) Then

                Return True
            End If
        Next
        Return False
    End Function
    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grDetalle.EditingCell
        Try
            If (tbGlosa.ReadOnly = True) Then
                e.Cancel = True
                Return
            End If

            If (grDetalle.GetValue("Tipo") = 2) Then
                e.Cancel = True
                Return

            End If

            If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
                e.Cancel = False
                Return
            Else
                If (Global_ModificarDescuento = 1 And (
                    e.Column.Index = grDetalle.RootTable.Columns("ProcentajeDescuento").Index Or
                     e.Column.Index = grDetalle.RootTable.Columns("MontoDescuento").Index)) Then


                    e.Cancel = False
                    Return

                End If
                If (Global_ModificarPrecio = 1 And e.Column.Index = grDetalle.RootTable.Columns("Precio").Index) Then

                    e.Cancel = False
                    Return

                End If

                e.Cancel = True


            End If
        Catch ex As Exception

        End Try







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

                    btnSeleccionarProducto.Focus()
                Else
                    ToastNotification.Show(Me, "Seleccione Producto", img, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
            If (grDetalle.Col = grDetalle.RootTable.Columns("Producto").Index) Then
                If (grDetalle.GetValue("Producto") <> String.Empty) Then

                    btnSeleccionarProducto.Focus()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto", img, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
salirIf:
        End If

        If (e.KeyData = Keys.Control + Keys.Enter And grDetalle.Row >= 0 And
            grDetalle.Col = grDetalle.RootTable.Columns("Producto").Index) Then
            Dim indexfil As Integer = grDetalle.Row
            Dim indexcol As Integer = grDetalle.Col
            btnSeleccionarProducto.Focus()

        End If
        If (e.KeyData = Keys.Escape And grDetalle.Row >= 0) Then

            _prEliminarFila()


        End If



    End Sub






    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grDetalle.CellValueChanged
        Dim lin As Integer = grDetalle.GetValue("Id")
        Dim pos As Integer = -1
        _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("Cantidad")) Or grDetalle.GetValue("Cantidad").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio")
                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                grDetalle.SetValue("Cantidad", 1)
                grDetalle.SetValue("ProcentajeDescuento", 0)
                grDetalle.SetValue("MontoDescuento", 0)
                grDetalle.SetValue("SubTotal", grDetalle.GetValue("Precio"))
                grDetalle.SetValue("Total", grDetalle.GetValue("Precio"))



                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else
                If (grDetalle.GetValue("Cantidad") > 0) Then

                    If (grDetalle.GetValue("Cantidad") <= grDetalle.GetValue("Stock")) Then
                        Dim porcdesc As Double = grDetalle.GetValue("ProcentajeDescuento")
                        Dim montodesc As Double = ((grDetalle.GetValue("Precio") * grDetalle.GetValue("Cantidad")) * (porcdesc / 100))
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                        grDetalle.SetValue("MontoDescuento", montodesc)
                        Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")
                        Dim rowIndex01 As Integer = grDetalle.Row
                        P_PonerTotal(rowIndex01)
                        If (estado = 1) Then
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                        End If

                    Else
                        ToastNotification.Show(Me, "La Cantidad = " + Str(grDetalle.GetValue("Cantidad")) + " es mayor al Stock del Producto = " + Str(grDetalle.GetValue("Stock")), img, 6000, eToastGlowColor.Red, eToastPosition.TopCenter)
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio")
                        Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                        grDetalle.SetValue("Cantidad", 1)
                        grDetalle.SetValue("ProcentajeDescuento", 0)
                        grDetalle.SetValue("MontoDescuento", 0)
                        grDetalle.SetValue("SubTotal", grDetalle.GetValue("Precio"))
                        grDetalle.SetValue("Total", grDetalle.GetValue("Precio"))



                        If (estado = 1) Then
                            CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                        End If

                    End If


                Else

                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Cantidad") = 1
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio")
                    Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                    grDetalle.SetValue("Cantidad", 1)
                    grDetalle.SetValue("ProcentajeDescuento", 0)
                    grDetalle.SetValue("MontoDescuento", 0)
                    grDetalle.SetValue("SubTotal", grDetalle.GetValue("Precio"))
                    grDetalle.SetValue("Total", grDetalle.GetValue("Precio"))
                    If (estado = 1) Then
                        CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                    End If

                End If
            End If
        End If

        If (e.Column.Index = grDetalle.RootTable.Columns("Precio").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("Precio")) Or grDetalle.GetValue("Precio").ToString = String.Empty) Then

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = 0
                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                grDetalle.SetValue("Precio", 0)
                grDetalle.SetValue("ProcentajeDescuento", 0)
                grDetalle.SetValue("MontoDescuento", 0)
                grDetalle.SetValue("SubTotal", 0)
                grDetalle.SetValue("Total", 0)



                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            Else

                Dim porcdesc As Double = grDetalle.GetValue("ProcentajeDescuento")
                Dim montodesc As Double = ((grDetalle.GetValue("Precio") * grDetalle.GetValue("Cantidad")) * (porcdesc / 100))
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio") = grDetalle.GetValue("Precio")
                grDetalle.SetValue("MontoDescuento", montodesc)
                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")
                Dim rowIndex01 As Integer = grDetalle.Row
                P_PonerTotal(rowIndex01)
                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If
            End If
        End If


        If (e.Column.Index = grDetalle.RootTable.Columns("ProcentajeDescuento").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("ProcentajeDescuento")) Or grDetalle.GetValue("ProcentajeDescuento").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                lin = grDetalle.GetValue("Id")
                pos = -1
                _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                grDetalle.SetValue("ProcentajeDescuento", 0)
                grDetalle.SetValue("MontoDescuento", 0)
                'grdetalle.SetValue("tbcmin", 1)
                'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))
            Else
                If (grDetalle.GetValue("ProcentajeDescuento") > 0 And grDetalle.GetValue("ProcentajeDescuento") <= 100) Then

                    Dim porcdesc As Double = grDetalle.GetValue("ProcentajeDescuento")
                    Dim montodesc As Double = (grDetalle.GetValue("SubTotal") * (porcdesc / 100))
                    lin = grDetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                    grDetalle.SetValue("MontoDescuento", montodesc)


                Else
                    lin = grDetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                    grDetalle.SetValue("ProcentajeDescuento", 0)
                    grDetalle.SetValue("MontoDescuento", 0)
                    grDetalle.SetValue("Total", grDetalle.GetValue("SubTotal"))
                    _prCalcularPrecioTotal()

                    'grdetalle.SetValue("tbcmin", 1)
                    'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))

                End If
            End If
        End If


        '''''''''''''''''''''MONTO DE DESCUENTO '''''''''''''''''''''
        If (e.Column.Index = grDetalle.RootTable.Columns("MontoDescuento").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("MontoDescuento")) Or grDetalle.GetValue("MontoDescuento").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                lin = grDetalle.GetValue("Id")
                pos = -1
                _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")

                grDetalle.SetValue("ProcentajeDescuento", 0)
                grDetalle.SetValue("MontoDescuento", 0)
                'grdetalle.SetValue("tbcmin", 1)
                'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))
            Else
                If (grDetalle.GetValue("MontoDescuento") > 0 And grDetalle.GetValue("MontoDescuento") <= grDetalle.GetValue("SubTotal")) Then

                    Dim montodesc As Double = grDetalle.GetValue("MontoDescuento")
                    Dim pordesc As Double = ((montodesc * 100) / grDetalle.GetValue("SubTotal"))

                    lin = grDetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = montodesc
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = pordesc

                    grDetalle.SetValue("ProcentajeDescuento", pordesc)


                Else
                    lin = grDetalle.GetValue("Id")
                    pos = -1
                    _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("ProcentajeDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("MontoDescuento") = 0
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal")
                    grDetalle.SetValue("ProcentajeDescuento", 0)
                    grDetalle.SetValue("MontoDescuento", 0)
                    grDetalle.SetValue("Total", grDetalle.GetValue("SubTotal"))
                    _prCalcularPrecioTotal()
                    'grdetalle.SetValue("tbcmin", 1)
                    'grdetalle.SetValue("SubTotal", grdetalle.GetValue("tbpbas"))

                End If
            End If
        End If
        Dim rowIndex As Integer = grDetalle.Row
        P_PonerTotal(rowIndex)
        _prCalcularPrecioTotal()
    End Sub
    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < grDetalle.RowCount) Then

            Dim lin As Integer = grDetalle.GetValue("Id")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
            Dim cant As Double = grDetalle.GetValue("Cantidad")
            Dim uni As Double = grDetalle.GetValue("Precio")
            Dim MontoDesc As Double = grDetalle.GetValue("MontoDescuento")
            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni
                'grDetalle.SetValue("lcmdes", montodesc)

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = TotalUnitario
                grDetalle.SetValue("SubTotal", TotalUnitario)
                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")


                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Total") = TotalUnitario - MontoDesc
                grDetalle.SetValue("Total", TotalUnitario - MontoDesc)
                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If

            End If
            _prCalcularPrecioTotal()
        End If



    End Sub
    Public Sub _prCalcularPrecioTotal()
        If (tbMdesc.Text.ToString = String.Empty) Then
            tbMdesc.Value = 0
        End If

        Dim montodesc As Double = tbMdesc.Value

        Dim total As Double = 0
        Dim dt As DataTable = CType(grDetalle.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            If (dt.Rows(i).Item("estado") >= 0) Then

                total += dt.Rows(i).Item("Total")
            End If
        Next



        Dim pordesc As Double = ((montodesc * 100) / total)
        tbPdesc.Value = pordesc





        tbTotal.Value = total - montodesc
    End Sub
    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grDetalle.CellEdited
        If (e.Column.Index = grDetalle.RootTable.Columns("Cantidad").Index) Then
            If (Not IsNumeric(grDetalle.GetValue("Cantidad")) Or grDetalle.GetValue("Cantidad").ToString = String.Empty) Then

                grDetalle.SetValue("Cantidad", 1)
            Else
                If (grDetalle.GetValue("Cantidad") > 0) Then


                Else

                    grDetalle.SetValue("Cantidad", 1)

                End If
            End If
        End If

        If (e.Column.Index = grDetalle.RootTable.Columns("Precio").Index) Then
            Dim lin As Integer = grDetalle.GetValue("Id")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin, grDetalle.GetValue("Tipo"))
            If (grDetalle.GetValue("Precio") < grDetalle.GetValue("PrecioCosto")) Then

                ToastNotification.Show(Me, "El Precio Es Menor Al Precio De Costo Del Producto que Es = " + Str(grDetalle.GetValue("PrecioCosto")), img, 6000, eToastGlowColor.Red, eToastPosition.TopCenter)

                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("Precio") = grDetalle.GetValue("PrecioCosto")
                CType(grDetalle.DataSource, DataTable).Rows(pos).Item("SubTotal") = grDetalle.GetValue("PrecioCosto") * grDetalle.GetValue("Cantidad")

                Dim estado As Integer = CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado")

                grDetalle.SetValue("Precio", grDetalle.GetValue("PrecioCosto"))
                grDetalle.SetValue("SubTotal", (grDetalle.GetValue("PrecioCosto") * grDetalle.GetValue("Cantidad")))


                Dim rowIndex As Integer = grDetalle.Row
                P_PonerTotal(rowIndex)
                If (estado = 1) Then
                    CType(grDetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If
            End If

        End If
        _prCalcularPrecioTotal()
    End Sub


    Private Sub grdetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grDetalle.MouseClick
        If (Not _fnAccesible()) Then
            Return
        End If
        Try
            If (grDetalle.RowCount >= 1) Then
                If (grDetalle.Row >= 0) Then
                    If (grDetalle.CurrentColumn.Index = grDetalle.RootTable.Columns("img").Index) Then
                        _prEliminarFila()
                    End If
                End If

            End If
        Catch ex As Exception

        End Try



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

        Else
            If (_fnAccesible() And tbCodigo.Text <> String.Empty And Not IsNothing(grDetalle.DataSource)) Then

                For i As Integer = 0 To CType(grDetalle.DataSource, DataTable).Rows.Count - 1 Step 1
                    If (CType(grDetalle.DataSource, DataTable).Rows(i).Item("Estado") > 0) Then
                        CType(grDetalle.DataSource, DataTable).Rows(i).Item("Estado") = -1
                    Else
                        CType(grDetalle.DataSource, DataTable).Rows(i).Item("Estado") = -2
                    End If


                Next

            End If

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

        tbGlosa.ReadOnly = False
        If (Global_Sucursal >= 0) Then

            cbSucursal.ReadOnly = True
        Else

            cbSucursal.ReadOnly = False
        End If

        swTipoVenta.IsReadOnly = False
        tbFechaVencimientoCredito.ReadOnly = False
        tbFechaTransaccion.ReadOnly = False

        If (Global_ModificarDescuento = 1) Then
            tbMdesc.IsInputReadOnly = False
            tbPdesc.IsInputReadOnly = False
        Else
            tbMdesc.IsInputReadOnly = True
            tbPdesc.IsInputReadOnly = True
        End If


        btnVendedor.Visible = False
        btnCliente.Visible = True
        BtnImprimir.Visible = False
        tab_Cobro.Visible = False

        If (_MModificar = True) Then
            swEntrega.IsReadOnly = True
        Else
            swEntrega.IsReadOnly = False
        End If


    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        btnSeleccionarProducto.Visible = False
        tbVendedor.ReadOnly = True
        tbCliente.ReadOnly = True
        tbGlosa.ReadOnly = True
        swEntrega.IsReadOnly = True
        cbSucursal.ReadOnly = True
        swTipoVenta.IsReadOnly = True
        tbFechaVencimientoCredito.ReadOnly = True
        tbFechaTransaccion.ReadOnly = True
        tbTotal.IsInputReadOnly = True
        tbMdesc.IsInputReadOnly = True
        tbPdesc.IsInputReadOnly = True
        grDetalle.RootTable.Columns("img").Visible = False
        btnVendedor.Visible = False
        btnCliente.Visible = False
        BtnImprimir.Visible = True

        tab_Cobro.Visible = True

        swFacturado.IsReadOnly = True
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""

        tbGlosa.Text = ""
        tbCliente.Text = ""
        tbVendedor.Text = ""
        IdVendedor = 0
        IdCliente = 0
        tbFechaTransaccion.Value = Now.Date
        tbFechaVencimientoCredito.Value = Now.Date
        swTipoVenta.Value = True


        IdCliente = 0

        If (Global_Sucursal >= 0) Then

            cbSucursal.Value = Global_Sucursal
        Else
            seleccionarPrimerItemCombo(cbSucursal)
        End If


        tbMdesc.Value = 0
        tbPdesc.Value = 0
        tbTotal.Value = 0
        _prCargarDetalleVenta(-1)


        tbMontoBs.Value = 0
        tbMontoDolar.Value = 0
        tbTransferencia.Value = 0
        tbTarjeta.Value = 0

        tbTotalPagado.Value = 0
        tbCambio.Value = 0
        swFacturado.Value = False

    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()

        tbVendedor.BackColor = Color.White
        cbSucursal.BackColor = Color.White
        tbCliente.BackColor = Color.White

    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        'ByRef _numi As String, ConceptoId As Integer, DepositoId As Integer,Observacion as String,
        'Estado as integer, FechaDocumento As String, _dtDetalle As DataTable
        'tbFechaTransaccion.Value.ToString("yyyy/MM/dd")   CType(grDetalle.DataSource, DataTable)
        Dim res As Boolean
        Dim Id As String = "0"
        Try

            If (swTipoVenta.Value = True) Then

                Dim ef = New Efecto
                ef.tipo = 20
                ef.TotalVenta = tbTotal.Value
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then

                    Dim dt As DataTable = ListaVentasDetallePago(-1)
                    'a.Id , a.VentaId, a.MontoBs, a.MontoDolares, a.TarjetaBancaria, a.TransferenciaBancaria, a.TipoCambio, 1 as estado
                    dt.Rows.Add(0, 0, ef.MontoBs, ef.MontoDolares, ef.MontoTarjeta, ef.MontoTransferencia, Global_TipoCambio, 0)


                    res = VentaInsertar(Id, cbSucursal.Value, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"),
                                   IdVendedor, IdCliente, IIf(swTipoVenta.Value = True, 1, 0), tbFechaVencimientoCredito.Value.ToString("yyyy/MM/dd"),
                                   1, 1, tbGlosa.Text, tbTotal.Value, CType(grDetalle.DataSource, DataTable), tbMdesc.Value, dt, IIf(swFacturado.Value = True, 1, 0), IIf(swEntrega.Value = True, 1, 0))

                    If res Then

                        ReporteVenta(Id)
                        ToastNotification.Show(Me, "Codigo de Venta ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                        FilaSelectLote = Nothing

                    Else
                        ToastNotification.Show(Me, "Error al guardar la Venta".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

                    End If
                End If
            Else

                Dim dt As DataTable = ListaVentasDetallePago(-1)
                'a.Id , a.VentaId, a.MontoBs, a.MontoDolares, a.TarjetaBancaria, a.TransferenciaBancaria, a.TipoCambio, 1 as estado



                res = VentaInsertar(Id, cbSucursal.Value, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"),
                               IdVendedor, IdCliente, IIf(swTipoVenta.Value = True, 1, 0), tbFechaVencimientoCredito.Value.ToString("yyyy/MM/dd"),
                               1, 1, tbGlosa.Text, tbTotal.Value, CType(grDetalle.DataSource, DataTable), tbMdesc.Value, dt, IIf(swFacturado.Value = True, 1, 0), IIf(swEntrega.Value = True, 1, 0))

                If res Then

                    ReporteVenta(Id)
                    ToastNotification.Show(Me, "Codigo de Venta ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    FilaSelectLote = Nothing

                Else
                    ToastNotification.Show(Me, "Error al guardar la Venta".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

                End If
            End If





        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar la Venta".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try

            If (swTipoVenta.Value = True) Then

                Dim ef = New Efecto
                ef.tipo = 20
                ef.TotalVenta = tbTotal.Value
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then

                    Dim dt As DataTable = ListaVentasDetallePago(-1)
                    'a.Id , a.VentaId, a.MontoBs, a.MontoDolares, a.TarjetaBancaria, a.TransferenciaBancaria, a.TipoCambio, 1 as estado
                    dt.Rows.Add(0, 0, ef.MontoBs, ef.MontoDolares, ef.MontoTarjeta, ef.MontoTransferencia, Global_TipoCambio, 0)


                    Res = VentaModificar(tbCodigo.Text, cbSucursal.Value, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"),
                                   IdVendedor, IdCliente, IIf(swTipoVenta.Value = True, 1, 0), tbFechaVencimientoCredito.Value.ToString("yyyy/MM/dd"),
                                   1, 1, tbGlosa.Text, tbTotal.Value, CType(grDetalle.DataSource, DataTable), tbMdesc.Value, dt, IIf(swFacturado.Value = True, 1, 0))

                    If Res Then

                        ReporteVenta(tbCodigo.Text)
                        ToastNotification.Show(Me, "Codigo de Venta ".ToUpper + tbCodigo.Text + " Modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                        FilaSelectLote = Nothing

                    Else
                        ToastNotification.Show(Me, "Error al Modificar la Venta".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

                    End If
                End If
            Else

                Dim dt As DataTable = ListaVentasDetallePago(-1)
                'a.Id , a.VentaId, a.MontoBs, a.MontoDolares, a.TarjetaBancaria, a.TransferenciaBancaria, a.TipoCambio, 1 as estado



                Res = VentaModificar(tbCodigo.Text, cbSucursal.Value, tbFechaTransaccion.Value.ToString("yyyy/MM/dd"),
                               IdVendedor, IdCliente, IIf(swTipoVenta.Value = True, 1, 0), tbFechaVencimientoCredito.Value.ToString("yyyy/MM/dd"),
                               1, 1, tbGlosa.Text, tbTotal.Value, CType(grDetalle.DataSource, DataTable), tbMdesc.Value, dt, IIf(swFacturado.Value = True, 1, 0))

                If Res Then

                    ReporteVenta(tbCodigo.Text)
                    ToastNotification.Show(Me, "Codigo de Venta ".ToUpper + tbCodigo.Text + " Modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    FilaSelectLote = Nothing

                Else
                    ToastNotification.Show(Me, "Error al Modificar la Venta".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

                End If
            End If



        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar La Venta".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbGlosa.ReadOnly = False
    End Function
    Public Sub ReporteVenta(Id As String)
        Dim ef = New Efecto


        ef.tipo = 8
        ef.titulo = "Comprobante de Venta"
        ef.descripcion = "¿Desea Generar el Reporte de la Venta #" + Id + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_GenerarReporte(Id)


        End If

    End Sub

    Public Sub _PMOEliminarRegistro()
        FilaSelectLote = Nothing

        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar la Venta " + tbCodigo.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_Ventas")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Venta ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar la Venta".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "

        If (cbSucursal.SelectedIndex < 0) Then
            cbSucursal.BackColor = Color.Red
            MEP.SetError(cbSucursal, "Seleccione una Sucursal")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Empresa"
            _ok = False
        Else
            cbSucursal.BackColor = Color.White
            MEP.SetError(cbSucursal, "")
        End If
        If (IdVendedor <= 0) Then
            tbVendedor.BackColor = Color.Red
            MEP.SetError(tbVendedor, "Seleccione un Personal")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Personal"
            _ok = False
        Else
            tbVendedor.BackColor = Color.White
            MEP.SetError(tbVendedor, "")
        End If
        If (IdCliente <= 0) Then
            tbCliente.BackColor = Color.Red
            MEP.SetError(tbCliente, "Seleccione un Cliente")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Cliente"
            _ok = False
        Else
            tbCliente.BackColor = Color.White
            MEP.SetError(tbCliente, "")
        End If

        If (tbFechaTransaccion.Text.Length <= 0) Then
            tbFechaTransaccion.BackColor = Color.Red
            MEP.SetError(tbFechaTransaccion, "Ingrese una Fecha Valida")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Fecha Venta"
            _ok = False
        Else
            tbFechaTransaccion.BackColor = Color.White
            MEP.SetError(tbFechaTransaccion, "")
        End If

        MHighlighterFocus.UpdateHighlights()

        If (grDetalle.RowCount <= 0) Then
            ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            grDetalle.Focus()

            Return False

        End If
        If (grDetalle.RowCount = 1) Then
            If (CType(grDetalle.DataSource, DataTable).Rows(0).Item("ProductoId") = 0) Then

                ToastNotification.Show(Me, "Por Favor Inserte un Detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                grDetalle.Focus()

                Return False
            End If
        End If

        If (cbSucursal.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbSucursal.Focus()
            Return _ok
        End If

        If (tbFechaTransaccion.Text.Length <= 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbFechaTransaccion.Focus()
            Return _ok
        End If
        If (IdVendedor <= 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbVendedor.Focus()
            Return _ok
        End If
        If (IdCliente <= 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbCliente.Focus()
            Return _ok
        End If

        Dim dt As DataTable = L_prListarGeneral("MAM_CierreCajero")

        Dim fila As DataRow() = dt.Select("SucursalId=" + Str(cbSucursal.Value) + " and EstadoCaja=1")
        If (Not IsDBNull(fila)) Then
            If (fila.Count <= 0) Then

                ToastNotification.Show(Me, "No Es Posible Hacer EL Movimiento Por que no Existe Caja Chica con Estado Abierto Para Esta Fecha =" + tbFechaTransaccion.Value.ToString("dd/MM/yyy"), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbFechaTransaccion.Focus()
                _ok = False
                Return _ok
            Else
                Dim bandera As Boolean = False
                For Each item As Object In fila
                    If (item("Fecha") = tbFechaTransaccion.Value) Then
                        bandera = True
                    End If
                Next
                If (bandera = False) Then
                    ToastNotification.Show(Me, "No Es Posible Hacer EL Movimiento Por que no Existe Caja Chica con Estado Abierto Para Esta Fecha =" + tbFechaTransaccion.Value.ToString("dd/MM/yyy"), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                    tbFechaTransaccion.Focus()
                    _ok = False
                    Return _ok
                End If
            End If
        End If

        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable

        If gi_userRol = 1 Then
            dtBuscador = L_prListarVentasGeneralFiltroFecha("MAM_Ventas", tbDesde.Value.ToString("yyyy/MM/dd"), tbHasta.Value.ToString("yyyy/MM/dd"), Global_Sucursal)

        Else
            dtBuscador = L_prListarVentasGeneralFiltroFecha("MAM_Ventas", tbDesde.Value.ToString("yyyy/MM/dd"), tbHasta.Value.ToString("yyyy/MM/dd"), Global_Sucursal)

            Dim dt = dtBuscador.Copy

            dt.Rows.Clear()

            For i As Integer = 0 To dtBuscador.Rows.Count - 1 Step 1

                If (dtBuscador.Rows(i).Item("PersonalId") = Global_IdPersonal) Then
                    dt.ImportRow(dtBuscador.Rows(i))
                End If
            Next

            dtBuscador = dt

        End If


        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'a.Id , a.SucursalId, a.FechaVenta, a.PersonalId, p.NombrePersonal As Personal,
        'a.TipoVenta, IIf(a.TipoVenta = 1,'Contado','Credito')as TVenta,a.FechaVencimientoCredito ,
        'a.ClienteId, c.NombreCliente, a.MonedaVenta, a.Estado, a.Glosa, a.Descuento, a.TotalVenta 

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("SucursalId", False))
        listEstCeldas.Add(New Celda("FechaVenta", True, "Fecha Venta", 100))
        listEstCeldas.Add(New Celda("PersonalId", False, "Estado", 150))
        listEstCeldas.Add(New Celda("Personal", True, "Personal", 150))
        listEstCeldas.Add(New Celda("TipoVenta", False, "", 70))
        listEstCeldas.Add(New Celda("NombreCliente", True, "Cliente", 150))
        listEstCeldas.Add(New Celda("TVenta", True, "Tipo Venta", 70))
        listEstCeldas.Add(New Celda("FechaVencimientoCredito", False, "Estado", 70))
        listEstCeldas.Add(New Celda("ClienteId", False))

        listEstCeldas.Add(New Celda("MonedaVenta", False))
        listEstCeldas.Add(New Celda("Estado", False))
        listEstCeldas.Add(New Celda("Facturado", False))

        listEstCeldas.Add(New Celda("Glosa", True, " Glosa", 200))
        listEstCeldas.Add(New Celda("NombreAlmacen", True, " Sucursal", 120))
        listEstCeldas.Add(New Celda("TotalVenta", True, "Total Venta", 120, "0.00"))
        listEstCeldas.Add(New Celda("Descuento", False))
        listEstCeldas.Add(New Celda("MontoBs", False))
        listEstCeldas.Add(New Celda("MontoDolares", False))
        listEstCeldas.Add(New Celda("TarjetaBancaria", False))
        listEstCeldas.Add(New Celda("TransferenciaBancaria", False))
        listEstCeldas.Add(New Celda("TipoCambio", False))
        listEstCeldas.Add(New Celda("PendienteEntrega", False))
        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        'a.Id , a.SucursalId, a.FechaVenta, a.PersonalId, p.NombrePersonal As Personal,
        'a.TipoVenta, IIf(a.TipoVenta = 1,'Contado','Credito')as TVenta,a.FechaVencimientoCredito ,
        'a.ClienteId, c.NombreCliente, a.MonedaVenta, a.Estado, a.Glosa, a.Descuento, a.TotalVenta 
        Dim TipoCambio As Double = 0
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            cbSucursal.Value = .GetValue("SucursalId")
            tbFechaTransaccion.Value = .GetValue("FechaVenta")
            IdVendedor = .GetValue("PersonalId")
            tbVendedor.Text = .GetValue("Personal").ToString
            swTipoVenta.Value = .GetValue("TipoVenta")
            IdCliente = .GetValue("ClienteId")
            swFacturado.Value = .GetValue("Facturado")
            tbFechaVencimientoCredito.Value = .GetValue("FechaVencimientoCredito")
            tbCliente.Text = .GetValue("NombreCliente").ToString
            swEntrega.Value = .GetValue("PendienteEntrega")
            tbGlosa.Text = .GetValue("Glosa").ToString
            tbMdesc.Value = .GetValue("descuento")

            tbMontoBs.Value = .GetValue("MontoBs")
            tbMontoDolar.Value = .GetValue("MontoDolares")
            tbTarjeta.Value = .GetValue("TarjetaBancaria")
            tbTransferencia.Value = .GetValue("TransferenciaBancaria")
            TipoCambio = .GetValue("TipoCambio")
            tbTotalPagado.Value = tbMontoBs.Value + (tbMontoDolar.Value * TipoCambio) + tbTransferencia.Value + tbTarjeta.Value
            lbTipoCambio.Text = "Tipo Cambio = " + Str(TipoCambio)

        End With

        _prCargarDetalleVenta(tbCodigo.Text)
        tbMdesc.Value = JGrM_Buscador.GetValue("Descuento")
        _prCalcularPrecioTotal()
        tbCambio.Value = tbTotalPagado.Value - tbTotal.Value
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub



    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()

        Else
            '  Public _modulo As SideNavItem
            TabControlPrincipal.SelectedTabIndex = 1
        End If
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click


        _PMNuevo()
                Dim dt As DataTable = ListarPersonalById(Global_IdPersonal)
                If (dt.Rows.Count > 0) Then
                    IdVendedor = Global_IdPersonal
                    tbVendedor.Text = dt.Rows(0).Item("Nombre")
                    tbCliente.Focus()

                Else
                    tbVendedor.Focus()
                End If
                Dim dtclientes As DataTable = L_prListarGeneral("MAM_Clientes")

                Dim fila01 As DataRow() = dtclientes.Select("Id =1")
                If (Not IsDBNull(fila01)) Then
                    If (fila01.Count > 0) Then

                        IdCliente = fila01(0).Item("Id")
                        tbCliente.Text = fila01(0).Item("NombreCliente").ToString
                btnSeleccionarProducto.Focus()
            End If
                End If









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
        tbDesde.Value = Now.Date
        tbHasta.Value = Now.Date

        _prIniciarTodo()
        TabControlPrincipal.SelectedTabIndex = 1
        Me.Width = 1920
        Me.Height = 1080
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
        Dim dtCierre As DataTable = L_prListarVentaCierresCajaPendientes(tbDesde.Value.ToString("yyyy/MM/dd"), tbHasta.Value.ToString("yyyy/MM/dd"))
        Dim fila As DataRow()
        If (Global_Sucursal > 0) Then

            fila = dtCierre.Select("SucursalId=" + Str(Global_Sucursal) + " and EstadoCaja=1 and PersonalId=" + Str(Global_IdPersonal))
        Else
            fila = dtCierre.Select("EstadoCaja=1 and PersonalId=" + Str(Global_IdPersonal))
        End If

        If (Not IsDBNull(fila)) Then
            If (fila.Count <= 0) Then

                ToastNotification.Show(Me, "No Es Posible Hacer La Venta Por que no Existe Caja Chica con Estado Abierta", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                Return
            Else
                TabControlPrincipal.SelectedTabIndex = 0
                btnNuevo.PerformClick()
            End If

        End If

    End Sub

    Private Sub JGrM_Buscador_KeyDown(sender As Object, e As KeyEventArgs) Handles JGrM_Buscador.KeyDown
        If (e.KeyCode = Keys.Enter) Then

            TabControlPrincipal.SelectedTabIndex = 0

        End If
    End Sub




    Private Sub tbProveedor_KeyDown(sender As Object, e As KeyEventArgs) Handles tbVendedor.KeyDown
        If (_fnAccesible()) Then
            'If e.KeyData = Keys.Control + Keys.Enter Then


            '    Dim dt As DataTable

            '    dt = ListarPersonal()
            '    'a.Id ,a.NombreProveedor ,a.Direccion ,a.Telefono01

            '    Dim listEstCeldas As New List(Of Celda)
            '    listEstCeldas.Add(New Celda("Id,", False, "ID", 50))
            '    listEstCeldas.Add(New Celda("Nombre", True, "NOMBRE", 350))
            '    listEstCeldas.Add(New Celda("Direccion", True, "DIRECCION", 180))
            '    listEstCeldas.Add(New Celda("Telefono01", True, "Telefono".ToUpper, 200))
            '    Dim ef = New Efecto
            '    ef.tipo = 6
            '    ef.dt = dt
            '    ef.SeleclCol = 2
            '    ef.listEstCeldasNew = listEstCeldas
            '    ef.alto = 50
            '    ef.ancho = 350
            '    ef.Context = "Seleccione Personal".ToUpper
            '    ef.ShowDialog()
            '    Dim bandera As Boolean = False
            '    bandera = ef.band
            '    If (bandera = True) Then
            '        Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            '        IdVendedor = Row.Cells("Id").Value
            '        tbVendedor.Text = Row.Cells("Nombre").Value
            '        tbCliente.Focus()

            '    End If

            'End If

        End If


    End Sub

    Private Sub btnProveedor_Click(sender As Object, e As EventArgs) Handles btnVendedor.Click
        If (_fnAccesible()) Then


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
            ef.alto = 50
            ef.ancho = 350
            ef.Context = "Seleccione Personal".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                IdVendedor = Row.Cells("Id").Value
                tbVendedor.Text = Row.Cells("Nombre").Value
                tbCliente.Focus()

            End If

        End If


    End Sub

    Private Sub swTipoVenta_ValueChanged(sender As Object, e As EventArgs) Handles swTipoVenta.ValueChanged
        If (swTipoVenta.Value = True) Then
            tbFechaVencimientoCredito.Visible = False
            lbcredito.Visible = False
        Else
            lbcredito.Visible = True
            tbFechaVencimientoCredito.Visible = True

        End If
    End Sub

    Private Sub tbPdesc_ValueChanged(sender As Object, e As EventArgs) Handles tbPdesc.ValueChanged
        If (tbPdesc.Focused) Then
            If (Not tbPdesc.Text = String.Empty And Not tbTotal.Text = String.Empty) Then
                If (tbPdesc.Value = 0 Or tbPdesc.Value > 100) Then
                    tbPdesc.Value = 0
                    tbMdesc.Value = 0

                    _prCalcularPrecioTotal()

                Else

                    Dim porcdesc As Double = tbPdesc.Value
                    Dim montodesc As Double = (grDetalle.GetTotal(grDetalle.RootTable.Columns("Total"), AggregateFunction.Sum) * (porcdesc / 100))
                    tbMdesc.Value = montodesc
                    tbTotal.Value = grDetalle.GetTotal(grDetalle.RootTable.Columns("Total"), AggregateFunction.Sum) - montodesc
                End If


            End If
            If (tbPdesc.Text = String.Empty) Then
                tbMdesc.Value = 0

            End If
        End If
    End Sub

    Private Sub tbMdesc_ValueChanged(sender As Object, e As EventArgs) Handles tbMdesc.ValueChanged
        If (tbMdesc.Focused) Then

            Dim total As Double = tbTotal.Value
            If (Not tbMdesc.Text = String.Empty And Not tbMdesc.Text = String.Empty) Then
                If (tbMdesc.Value = 0 Or tbMdesc.Value > total) Then
                    tbMdesc.Value = 0
                    tbPdesc.Value = 0
                    _prCalcularPrecioTotal()
                Else
                    Dim montodesc As Double = tbMdesc.Value
                    Dim pordesc As Double = ((montodesc * 100) / grDetalle.GetTotal(grDetalle.RootTable.Columns("Total"), AggregateFunction.Sum))
                    tbPdesc.Value = pordesc
                    tbTotal.Value = grDetalle.GetTotal(grDetalle.RootTable.Columns("Total"), AggregateFunction.Sum) - montodesc

                End If

            End If

            If (tbMdesc.Text = String.Empty) Then
                tbMdesc.Value = 0

            End If
        End If
    End Sub

    Private Sub tbCliente_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCliente.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If
        If (e.KeyData = Keys.Control + Keys.Enter) Then '

            Dim dt As DataTable

            dt = ListarCliente()
            'a.Id ,a.NombreCliente  as NombreProveedor ,a.DireccionCliente  ,a.Telefono

            Dim listEstCeldas As New List(Of Celda)
            listEstCeldas.Add(New Celda("Id,", False, "ID", 50))
            listEstCeldas.Add(New Celda("NombreCliente", True, "NOMBRE", 350))
            listEstCeldas.Add(New Celda("DireccionCliente", True, "DIRECCION", 180))
            listEstCeldas.Add(New Celda("Telefono", True, "Telefono".ToUpper, 200))


            Dim ef = New Efecto
            ef.tipo = 7
            ef.dt = dt
            ef.SeleclCol = 0
            ef.listEstCeldasNew = listEstCeldas
            ef.alto = 80
            ef.ancho = 800
            ef.Context = "Seleccione Cliente".ToUpper
            ef.ShowDialog()
            Dim bandera As Boolean = False
            bandera = ef.band
            If (bandera = True) Then
                Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                IdCliente = Row.Cells("ID").Value
                tbCliente.Text = Row.Cells("NombreProveedor").Value.ToString

                tbGlosa.Focus()
            Else
                If (ef.NewCliente) Then
                    IdCliente = ef.IdCliente
                    tbCliente.Text = ef.NombreCliente

                    tbGlosa.Focus()
                End If
            End If


        End If

    End Sub

    Private Sub btnCliente_Click(sender As Object, e As EventArgs) Handles btnCliente.Click
        If (Not _fnAccesible()) Then
            Return
        End If


        Dim dt As DataTable


        dt = ListarCliente()
        'a.Id ,a.NombreCliente  as NombreProveedor ,a.DireccionCliente  ,a.Telefono

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id,", False, "ID", 50))
        listEstCeldas.Add(New Celda("NombreCliente", True, "NOMBRE", 350))
        listEstCeldas.Add(New Celda("DireccionCliente", True, "DIRECCION", 180))
        listEstCeldas.Add(New Celda("Telefono", True, "Telefono".ToUpper, 200))


        Dim ef = New Efecto
        ef.tipo = 7
        ef.dt = dt
        ef.SeleclCol = 0
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 80
        ef.ancho = 800
        ef.Context = "Seleccione Cliente".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            IdCliente = Row.Cells("ID").Value
            tbCliente.Text = Row.Cells("NombreProveedor").Value.ToString

            btnSeleccionarProducto.Focus()
        Else
            If (ef.NewCliente) Then
                IdCliente = ef.IdCliente
                tbCliente.Text = ef.NombreCliente

                btnSeleccionarProducto.Focus()
            End If
        End If

    End Sub
    Private Sub P_GenerarReporte(numi As String)

        Try
            Dim dt As DataTable = ListarVentaRecibo(numi)

            Dim total As Decimal = dt.Compute("SUM(Total)", "")
            total = total - dt.Rows(0).Item("DescuentoVenta")
            Dim fechaven As String = dt.Rows(0).Item("FechaVenta")
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


                        dt.Rows(i).Item("imageEmpresa") = Bin.GetBuffer
                    Next
                End If


            End If

            Dim _FechaAct As String
            Dim _Fecha() As String
            Dim _FechaPar As String
            Dim _Meses() As String = {"Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"}
            _FechaAct = fechaven
            _Fecha = Split(_FechaAct, "-")
            _FechaPar = "Villazón, " + _Fecha(0).Trim + " De " + _Meses(_Fecha(1) - 1).Trim + " Del " + _Fecha(2).Trim

            If Not IsNothing(P_Global.Visualizador) Then
                P_Global.Visualizador.Close()
            End If
            Dim ParteEntera As Long
            Dim ParteDecimal As Decimal
            ParteEntera = Int(total)
            ParteDecimal = Math.Round(total - ParteEntera, 2)
            Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(ParteEntera)) + " con " +
        IIf(ParteDecimal.ToString.Equals("0"), "00", ParteDecimal.ToString) + "/100 Bolivianos"



            P_Global.Visualizador = New Visualizador

            Dim objrep As New Recibo
            'objrep.SetDataSource(dt)
            'objrep.Subreports.Item("Recibo.rpt").SetDataSource(dt)
            'objrep.Subreports.Item("Recibo.rpt - 01").SetDataSource(dt)
            'objrep.SetParameterValue("MontoA", li)
            'objrep.SetParameterValue("FechaA", _FechaPar)
            'objrep.SetParameterValue("TotalA", Str(total))
            'objrep.SetParameterValue("TipoReporteA", "NOTA DE VENTA")




            objrep.SetDataSource(dt)


            objrep.SetParameterValue("Monto", li)
            objrep.SetParameterValue("Fecha", _FechaPar)
            objrep.SetParameterValue("Total", Str(total))
            objrep.SetParameterValue("TipoReporte", "NOTA DE VENTA")
            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.CrGeneral.Zoom(130)
            P_Global.Visualizador.Show() 'Comentar
            ''P_Global.Visualizador.BringToFront() 'Comentar

        Catch ex As Exception
            ToastNotification.Show(Me, "Error al Generar el Reporte " + ex.Message, img, 16000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End Try


    End Sub

    Private Sub P_GenerarReporteMasCopia(numi As String)
        Dim dt As DataTable = ListarVentaRecibo(numi)

        Dim total As Decimal = dt.Compute("SUM(Total)", "")
        total = total - dt.Rows(0).Item("DescuentoVenta")
        Dim fechaven As String = dt.Rows(0).Item("FechaVenta")
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


                    dt.Rows(i).Item("imageEmpresa") = Bin.GetBuffer
                Next
            End If


        End If

        Dim _FechaAct As String
        Dim _Fecha() As String
        Dim _FechaPar As String
        Dim _Meses() As String = {"Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"}
        _FechaAct = fechaven
        _Fecha = Split(_FechaAct, "-")
        _FechaPar = "Villazón, " + _Fecha(0).Trim + " De " + _Meses(_Fecha(1) - 1).Trim + " Del " + _Fecha(2).Trim

        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If
        Dim ParteEntera As Long
        Dim ParteDecimal As Decimal
        ParteEntera = Int(total)
        ParteDecimal = Math.Round(total - ParteEntera, 2)
        Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(ParteEntera)) + " con " +
        IIf(ParteDecimal.ToString.Equals("0"), "00", ParteDecimal.ToString) + "/100 Bolivianos"



        P_Global.Visualizador = New Visualizador

        Dim objrep As New ReporteVentaMasCopia
        objrep.SetDataSource(dt)
        objrep.Subreports.Item("Recibo.rpt").SetDataSource(dt)
        objrep.Subreports.Item("Recibo.rpt - 01").SetDataSource(dt)
        objrep.SetParameterValue("MontoA", li)
        objrep.SetParameterValue("FechaA", _FechaPar)
        objrep.SetParameterValue("TotalA", Str(total))
        objrep.SetParameterValue("TipoReporteA", "NOTA DE VENTA")




        'objrep.SetDataSource(dt)


        'objrep.SetParameterValue("Monto", li)
        'objrep.SetParameterValue("Fecha", _FechaPar)
        'objrep.SetParameterValue("Total", Str(total))
        'objrep.SetParameterValue("TipoReporte", "NOTA DE VENTA")
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(130)
        P_Global.Visualizador.Show() 'Comentar
        ''P_Global.Visualizador.BringToFront() 'Comentar


    End Sub
    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles BtnImprimir.Click
        If (Not _fnAccesible() And tbCodigo.Text <> String.Empty) Then
            P_GenerarReporte(tbCodigo.Text)

        End If
    End Sub

    Private Sub ButtonX2_Click_1(sender As Object, e As EventArgs) Handles ButtonX2.Click
        If (Not _fnAccesible() And tbCodigo.Text <> String.Empty) Then
            P_GenerarReporte(tbCodigo.Text)

        End If
    End Sub

    Private Sub VerToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles VerToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then
            TabControlPrincipal.SelectedTabIndex = 0

        End If
    End Sub

    Private Sub EditarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditarToolStripMenuItem.Click

        If (JGrM_Buscador.GetValue("CierreModulo") > 0) Then
            ToastNotification.Show(Me, "No Es Posible Modificar La Venta Ya que Pertenece A un cierre De Caja Cerrado # " + Str(JGrM_Buscador.GetValue("CierreModulo")), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

        If (JGrM_Buscador.Row >= 0) Then
            TabControlPrincipal.SelectedTabIndex = 0
            btnModificar.PerformClick()

        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.GetValue("CierreModulo") > 0) Then
            ToastNotification.Show(Me, "No Es Posible Eliminar La Venta Ya que Pertenece A un cierre De Caja Cerrado # " + Str(JGrM_Buscador.GetValue("CierreModulo")), img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return
        End If

        If (JGrM_Buscador.Row >= 0) Then
            btnEliminar.PerformClick()


        End If
    End Sub

    Private Sub LabelX3_Click(sender As Object, e As EventArgs) Handles LabelX3.Click

    End Sub

    Private Sub btnSeleccionarProducto_Click(sender As Object, e As EventArgs) Handles btnSeleccionarProducto.Click
        Dim ef = New Efecto
        ef.tipo = 16
        ef.dtDetalle = CType(grDetalle.DataSource, DataTable)

        ef.SucursalId = cbSucursal.Value
        ef.Lotebool = Lote
        ef.TipoPrograma = 1
        ef.IdCliente = IdCliente
        ef.ShowDialog()
        grDetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grDetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
        _prCalcularPrecioTotal()
    End Sub

    Private Sub LabelX8_Click(sender As Object, e As EventArgs) Handles LabelX8.Click

    End Sub

    Private Sub btnConfirmarSalir_Click(sender As Object, e As EventArgs) Handles btnFiltrarVentas.Click

        If (tbDesde.Value > tbHasta.Value) Then

            ToastNotification.Show(Me, "La Fecha Desde Debe Ser Menor Que la Fecha Hasta", img, 5000, eToastGlowColor.Red, eToastPosition.BottomRight)
        Else
            _PMCargarBuscador()
        End If


    End Sub

    Private Sub btnImprimirCopia_Click(sender As Object, e As EventArgs) Handles btnImprimirCopia.Click
        If (Not _fnAccesible() And tbCodigo.Text <> String.Empty) Then
            P_GenerarReporteMasCopia(tbCodigo.Text)

        End If
    End Sub

    Private Sub ButtonX3_Click(sender As Object, e As EventArgs) Handles ButtonX3.Click
        If (Not _fnAccesible() And tbCodigo.Text <> String.Empty) Then
            P_GenerarReporteMasCopia(tbCodigo.Text)

        End If
    End Sub

    Private Sub LabelX19_Click(sender As Object, e As EventArgs) Handles LabelX19.Click

    End Sub

    Private Sub swFacturado_ValueChanged(sender As Object, e As EventArgs) Handles swFacturado.ValueChanged
        If (swFacturado.Value = False) Then
            GpanelFacturado.Visible = False
            TabFacturado.Visible = False
        Else
            GpanelFacturado.Visible = True
            TabFacturado.Visible = True
        End If
    End Sub
#End Region
End Class