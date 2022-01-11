Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Public Class Tec_CierreCaja
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

        Me.Text = "Cierre Caja Distribuidor"

        _PMIniciarTodo()
        _prAsignarPermisos()


        _habilitarFocus()

    End Sub
    Private Sub _prCargarTablaGastos(id As String)
        Dim dt As New DataTable
        dt = ListaDetalleGastosCierreId(id)


        grGastos.DataSource = dt
        grGastos.RetrieveStructure()
        grGastos.AlternatingColors = True
        'ga.Id,ga.CierreCajaId ,ga.ConceptoGastoId,detalle.Descripcion,ga.Detalle,ga.Monto,1 as Estado,cast('' as Image) as Img

        With grGastos.RootTable.Columns("Id")
            .Width = 90
            .Caption = "Id"
            .Visible = False
        End With
        With grGastos.RootTable.Columns("CierreCajaId")
            .Width = 90
            .Caption = "Id"
            .Visible = False
        End With

        With grGastos.RootTable.Columns("ConceptoGastoId")
            .Width = 110
            .Visible = False
        End With

        'ga.Id,ga.CierreCajaId ,ga.ConceptoGastoId,detalle.Descripcion,ga.Detalle,ga.Monto,1 as Estado,cast('' as Image) as Img

        With grGastos.RootTable.Columns("Descripcion")
            .Width = 200
            .Visible = True
            .Caption = "Concepto Gasto"
            .MaxLines = 2
            .WordWrap = True
        End With
        With grGastos.RootTable.Columns("Detalle")
            .Width = 250
            .Visible = True
            .Caption = "Detalle"
            .MaxLines = 3
            .WordWrap = True
        End With
        With grGastos.RootTable.Columns("Monto")
            .Width = 110
            .Caption = "Monto"
            .Visible = True
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With

        With grGastos.RootTable.Columns("estado")
            .Width = 150
            .Visible = False
        End With
        With grGastos.RootTable.Columns("Img")
            .Width = 80
            .Visible = False
            .Caption = "Eliminar"
            .LeftMargin = 7
            .TopMargin = 5
            .BottomMargin = 5
        End With
        With grGastos
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

    Private Sub _prCargarPedidosConciliacion(id As String)
        Dim dt As New DataTable
        dt = ListaDetallePedidosconciliacion(id)


        grVentas.DataSource = dt
        grVentas.RetrieveStructure()
        grVentas.AlternatingColors = True
        '   'ClienteId	NombreCliente	VentaId	NroNotaVenta		Contado	Credito


        With grVentas.RootTable.Columns("ClienteId")
            .Width = 90
            .Caption = "Cod Cliente"
            .Visible = True
            .LeftMargin = 7
            .TopMargin = 5
            .BottomMargin = 5
        End With



        With grVentas.RootTable.Columns("NombreCliente")
            .Width = 300
            .Visible = True
            .Caption = "Nombre Cliente"
            .MaxLines = 2
            .WordWrap = True
        End With

        With grVentas.RootTable.Columns("VentaId")
            .Width = 150
            .Visible = False
        End With

        With grVentas.RootTable.Columns("FechaEntregado")
            .Width = 120
            .Visible = True
            .TextAlignment = TextAlignment.Far
            .Caption = "Fecha Entregado"
            .MaxLines = 3
            .WordWrap = True
        End With
        With grVentas.RootTable.Columns("NroNotaVenta")
            .Width = 120
            .Visible = True
            .TextAlignment = TextAlignment.Far
            .Caption = "Nro Nota Venta"
            .MaxLines = 3
            .WordWrap = True
        End With
        With grVentas.RootTable.Columns("Contado")
            .Width = 150
            .Caption = "Contado"
            .Visible = True
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grVentas.RootTable.Columns("Credito")
            .Width = 150
            .Caption = "Credito"
            .Visible = True
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With



        With grVentas
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .CellToolTipText = "Ventas"
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

    Private Sub _prCargarCobranzasSinCierreCaja(PersonalId As String)
        Dim dt As New DataTable
        dt = ListaCobranzasPorPersonalSinCierre(PersonalId)


        grCobranzas.DataSource = dt
        grCobranzas.RetrieveStructure()
        grCobranzas.AlternatingColors = True
        'TransaccionVentasCreditoId	ClienteId	NombreCliente	NroNotaVenta	Monto

        With grCobranzas.RootTable.Columns("TransaccionVentasCreditoId")
            .Width = 90
            .Visible = False
            .LeftMargin = 7
            .TopMargin = 5
            .BottomMargin = 5
        End With
        With grCobranzas.RootTable.Columns("ClienteId")
            .Width = 90
            .Caption = "Cod Cliente"
            .Visible = True
            .LeftMargin = 7
            .TopMargin = 5
            .BottomMargin = 5
        End With



        With grCobranzas.RootTable.Columns("NombreCliente")
            .Width = 300
            .Visible = True
            .Caption = "Nombre Cliente"
            .MaxLines = 2
            .WordWrap = True
        End With


        With grCobranzas.RootTable.Columns("NroNotaVenta")
            .Width = 120
            .Visible = True
            .TextAlignment = TextAlignment.Far
            .Caption = "Nro Nota Venta"
            .MaxLines = 3
            .WordWrap = True
        End With
        With grCobranzas.RootTable.Columns("Monto")
            .Width = 150
            .Caption = "Monto Cobrado"
            .Visible = True
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With




        With grCobranzas
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .CellToolTipText = "Ventas"
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

    Private Sub _prCargarCobranzasConCierreCaja(CierreCajaId As String)
        Dim dt As New DataTable
        dt = ListaCobranzasPorPersonalconCierre(CierreCajaId)


        grCobranzas.DataSource = dt
        grCobranzas.RetrieveStructure()
        grCobranzas.AlternatingColors = True
        'TransaccionVentasCreditoId	ClienteId	NombreCliente	NroNotaVenta	Monto

        With grCobranzas.RootTable.Columns("TransaccionVentasCreditoId")
            .Width = 90
            .Visible = False
            .LeftMargin = 7
            .TopMargin = 5
            .BottomMargin = 5
        End With
        With grCobranzas.RootTable.Columns("ClienteId")
            .Width = 90
            .Caption = "Cod Cliente"
            .Visible = True
            .LeftMargin = 7
            .TopMargin = 5
            .BottomMargin = 5
        End With



        With grCobranzas.RootTable.Columns("NombreCliente")
            .Width = 300
            .Visible = True
            .Caption = "Nombre Cliente"
            .MaxLines = 2
            .WordWrap = True
        End With


        With grCobranzas.RootTable.Columns("NroNotaVenta")
            .Width = 120
            .Visible = True
            .TextAlignment = TextAlignment.Far
            .Caption = "Nro Nota Venta"
            .MaxLines = 3
            .WordWrap = True
        End With
        With grCobranzas.RootTable.Columns("Monto")
            .Width = 150
            .Caption = "Monto Cobrado"
            .Visible = True
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With




        With grCobranzas
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
            .CellToolTipText = "Ventas"
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

    Public Sub CargarIconEliminar()
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.rowdelete, 25, 18)
        img.Save(Bin, Imaging.ImageFormat.Png)
        Dim dt As DataTable = CType(grGastos.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1


            CType(grGastos.DataSource, DataTable).Rows(i).Item("Img") = Bin.GetBuffer


        Next

    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbPersonal, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnAgregarGastos, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbFechaCierre, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDetalle, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbVentasContado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbVentasCredito, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbGastos, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbContadoCobranzas, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbEfectivoCalculado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbEfectivoRecibido, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDiferencia, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
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

        tbFechaCierre.ReadOnly = False
        tbDetalle.ReadOnly = False

        btnAgregarGastos.Visible = True
        btnImprimir.Visible = False
        btnSearchPersonal.Visible = True
        tbEfectivoRecibido.IsInputReadOnly = False
        panelProducto.Visible = True

        grGastos.RootTable.Columns("img").Visible = True

    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbFechaCierre.ReadOnly = True
        tbDetalle.ReadOnly = True

        tbPersonal.ReadOnly = True

        btnAgregarGastos.Visible = False
        btnSearchPersonal.Visible = False

        btnImprimir.Visible = True
        tbEfectivoRecibido.IsInputReadOnly = True
        grGastos.RootTable.Columns("img").Visible = False
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbPersonal.Clear()
        tbDetalle.Clear()
        tbFechaCierre.Value = Now.Date
        btnSearchPersonal.Focus()
        _prCargarTablaGastos(-1)
        tbVentasContado.Value = 0
        tbVentasCredito.Value = 0
        tbCobranzas.Value = 0
        tbGastos.Value = 0
        tbContadoCobranzas.Value = 0
        tbEfectivoCalculado.Value = 0
        tbEfectivoRecibido.Value = 0
        tbDiferencia.Value = 0

        PersonalId = 0
        ConciliacionID = 0
    End Sub


    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbPersonal.BackColor = Color.White
        tbFechaCierre.BackColor = Color.White

    End Sub

    Public Sub ImprimirNotaSalida(id As Integer)
        Dim dt As DataTable
        dt = DatosreporteCierreCaja(id)
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

        Dim objrep As New ReporteCierreCajaChofer

        objrep.SetDataSource(dt)
        objrep.SetParameterValue("NombreEmpresa", NombreEmpresa)
        objrep.SetParameterValue("Ciudad", Direccion)
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.CrGeneral.Zoom(90)
        P_Global.Visualizador.Show() 'Comentar
        ''P_Global.Visualizador.BringToFront() 'Comentar
    End Sub
    Public Sub ReporteVenta(Id As String)
        Dim ef = New Efecto


        ef.tipo = 8
        ef.titulo = "Comprobante de Cierre"
        ef.descripcion = "¿Desea Generar el Comprobante Cierre Caja  #" + Id + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            ImprimirNotaSalida(Id)


        End If

    End Sub
    Public Function _PMOGrabarRegistro() As Boolean

        Dim res As Boolean
        Try
            Dim Id As String = ""
            '_Id As String, PersonalId As Integer, ConciliacionId As Integer, SucursalId As Integer, Fecha As String, NroNota As String, Detalle As String, TipoMovimientoID As Integer, dtdetalle As DataTable
            res = InsertarCierreCaja(Id, ConciliacionID, PersonalId, tbFechaCierre.Value.ToString("yyyy/MM/dd"), tbGastos.Value, tbVentasCredito.Value, tbVentasContado.Value, tbCobranzas.Value, tbEfectivoCalculado.Value, tbEfectivoRecibido.Value, tbDiferencia.Value, CType(grGastos.DataSource, DataTable), CType(grCobranzas.DataSource, DataTable))

            If res Then
                ToastNotification.Show(Me, "Codigo de Cierre De Caja ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                ReporteVenta(Id)


            Else
                ToastNotification.Show(Me, "Error al guardar el Cierre De Caja".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Cierre De Caja".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try

            Res = ModificarCierreCaja(tbCodigo.Text, ConciliacionID, PersonalId, tbFechaCierre.Value.ToString("yyyy/MM/dd"), tbGastos.Value, tbVentasCredito.Value, tbVentasContado.Value, tbCobranzas.Value, tbEfectivoCalculado.Value, tbEfectivoRecibido.Value, tbDiferencia.Value, CType(grGastos.DataSource, DataTable), CType(grGastos.DataSource, DataTable))
            If Res Then

                ToastNotification.Show(Me, "Codigo de Cierre De Caja ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                ReporteVenta(tbCodigo.Text)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el Cierre De Caja".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

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
        ef.descripcion = "¿Esta Seguro de Eliminar el Cierre De Caja Nro " + tbCodigo.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_CierreCaja")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Cierre De Caja ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el Cierre De Caja".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "

        If PersonalId <= 0 Then
            tbPersonal.BackColor = Color.Red
            MEP.SetError(tbPersonal, "Seleccione Una Conciliacion")
            Mensaje = Mensaje + "Personal"
            _ok = False
        Else
            tbPersonal.BackColor = Color.White
            MEP.SetError(tbPersonal, "")
        End If
        If (tbFechaCierre.Value.ToString.Trim = String.Empty) Then
            tbFechaCierre.BackColor = Color.Red
            MEP.SetError(tbFechaCierre, "Seleccione Fecha")
            Mensaje = Mensaje + "Fecha Salida"
            _ok = False
        Else
            tbFechaCierre.BackColor = Color.White
            MEP.SetError(tbFechaCierre, "")
        End If


        MHighlighterFocus.UpdateHighlights()
        If (tbFechaCierre.Value.ToString.Trim = String.Empty) Then
            tbFechaCierre.Focus()
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

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_CierreCaja")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        '  c.Id ,c.ConciliacionId as NroConciliacion,c.PersonalId,p.NombrePersonal ,c.FechaCierre,c.TotalGastos,c.VentasCreditos ,c.VentasContado,c.TotalCobros ,c.EfectivoCalculado ,c.EfectivoRecibido,c.Diferencia 	
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "Cod. Cierre", 60))
        listEstCeldas.Add(New Celda("NroConciliacion", False))
        listEstCeldas.Add(New Celda("PersonalId", False))

        listEstCeldas.Add(New Celda("NombrePersonal", True, "Nombre Personal", 200))
        listEstCeldas.Add(New Celda("Detalle", True, "Detalle", 200))
        listEstCeldas.Add(New Celda("FechaCierre", True, "Fecha Cierre", 100, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("TotalGastos", False))

        listEstCeldas.Add(New Celda("VentasCreditos", False))
        listEstCeldas.Add(New Celda("VentasContado", False))
        listEstCeldas.Add(New Celda("TotalCobros", False))
        listEstCeldas.Add(New Celda("EfectivoCalculado", False))




        listEstCeldas.Add(New Celda("EfectivoRecibido", True, "Efectivo Recibido", 120, "0.00"))
        listEstCeldas.Add(New Celda("Diferencia", False))



        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        '  c.Id ,c.ConciliacionId as  NroConciliacion,c.PersonalId,p.NombrePersonal ,c.FechaCierre,c.TotalGastos,c.VentasCreditos ,c.VentasContado,c.TotalCobros ,c.EfectivoCalculado ,c.EfectivoRecibido,c.Diferencia 	 Detalle
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbPersonal.Text = .GetValue("NombrePersonal").ToString
            PersonalId = .GetValue("PersonalId")
            ConciliacionID = .GetValue("NroConciliacion")
            tbFechaCierre.Value = .GetValue("FechaCierre")
            tbDetalle.Text = .GetValue("Detalle").ToString
            tbGastos.Value = .GetValue("TotalGastos")
            tbVentasCredito.Value = .GetValue("VentasCreditos")

            tbVentasContado.Value = .GetValue("VentasContado")
            tbCobranzas.Value = .GetValue("TotalCobros")
            tbEfectivoCalculado.Value = .GetValue("EfectivoCalculado")
            tbEfectivoRecibido.Value = .GetValue("EfectivoRecibido")
            tbDiferencia.Value = .GetValue("Diferencia")

            tbContadoCobranzas.Value = .GetValue("VentasContado") + .GetValue("TotalCobros")

        End With
        _prCargarTablaGastos(tbCodigo.Text)
        _prCargarPedidosConciliacion(ConciliacionID)
        _prCargarCobranzasConCierreCaja(tbCodigo.Text)

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

    Private Sub Tec_CierreCaja_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()

    End Sub



    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        Me.Close()

    End Sub

    Private Sub btnSearchPersonal_Click(sender As Object, e As EventArgs) Handles btnSearchPersonal.Click
        Dim dt As DataTable

        dt = ListarConciliacionesCaja()
        'NroConciliacion	Fecha	PersonalID	NombrePersonal

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("NroConciliacion,", True, "Nro Conciliación", 60))
        listEstCeldas.Add(New Celda("Fecha", True, "Fecha Conciliacion", 120, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("PersonalID", False))
        listEstCeldas.Add(New Celda("NombrePersonal", True, "Chofer / Distribuidor", 350))


        Dim ef = New Efecto
        ef.tipo = 6
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 150
        ef.ancho = 500
        ef.Context = "Seleccione Conciliación Cerrada".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            PersonalId = Row.Cells("PersonalID").Value
            tbPersonal.Text = Row.Cells("NombrePersonal").Value
            ConciliacionID = Row.Cells("NroConciliacion").Value
            btnAgregarGastos.Focus()

            _prCargarPedidosConciliacion(ConciliacionID)
            _prCargarCobranzasSinCierreCaja(PersonalId)
            SumarCobranzas()
            SumarVentas()

        End If
    End Sub
    Public Sub SumarCobranzas()

        Dim total As Double = 0
        Dim dt As DataTable = CType(grCobranzas.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            total += dt.Rows(i).Item("Monto")
        Next

        tbCobranzas.Value = total

    End Sub

    Public Sub CalcularTotal()

        Dim Total As Double = 0

        Total = tbVentasContado.Value + tbCobranzas.Value
        Total = Total - tbGastos.Value

        tbEfectivoCalculado.Value = Total
        tbDiferencia.Value = tbEfectivoRecibido.Value - tbEfectivoCalculado.Value

    End Sub

    Public Sub SumarVentas()
        Dim Contado As Double = 0
        Dim Credito As Double = 0
        Dim dt As DataTable = CType(grVentas.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            Contado += dt.Rows(i).Item("Contado")
            Credito += dt.Rows(i).Item("Credito")
        Next

        tbVentasContado.Value = Contado
        tbVentasCredito.Value = Credito

        tbContadoCobranzas.Value = Contado + tbCobranzas.Value
        CalcularTotal()
    End Sub

    Public Function _GenerarId()
        Dim dt As DataTable = CType(grGastos.DataSource, DataTable)
        Dim mayor As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim data As Integer = IIf(IsDBNull(CType(grGastos.DataSource, DataTable).Rows(i).Item("Id")), 0, CType(grGastos.DataSource, DataTable).Rows(i).Item("Id"))
            If (data > mayor) Then
                mayor = data

            End If
        Next
        Return mayor
    End Function
    Private Sub btnAgregarGastos_Click(sender As Object, e As EventArgs) Handles btnAgregarGastos.Click
        Dim ConceptoId As Integer
        Dim Descripcion As String
        Dim Monto As Double
        Dim frmAyuda As Tec_Gastos
        frmAyuda = New Tec_Gastos()
        frmAyuda.ShowDialog()
        If (frmAyuda.Bandera = True) Then

            ConceptoId = frmAyuda.ConceptoId
            Monto = frmAyuda.Monto
            Descripcion = frmAyuda.Descripcion
            Dim Bin As New MemoryStream
            Dim imgDelete As New Bitmap(My.Resources.rowdelete, 25, 23)
            imgDelete.Save(Bin, Imaging.ImageFormat.Png)
            'ga.Id, ga.CierreCajaId, ga.ConceptoGastoId, detalle.Descripcion, ga.Detalle, ga.Monto, 1 As Estado, cast('' as Image) as Img
            CType(grGastos.DataSource, DataTable).Rows.Add(_GenerarId() + 1, 0, ConceptoId, frmAyuda.NombreConcepto, Descripcion, Monto, 0, Bin.GetBuffer())


        End If
        grGastos.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grGastos.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
        CalcularGastos()

    End Sub
    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grGastos.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grGastos.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub
    Public Sub _prEliminarFila()
        If (grGastos.Row >= 0) Then
            If (grGastos.RowCount >= 1) Then
                Dim estado As Integer = grGastos.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grGastos.GetValue("Id")
                _fnObtenerFilaDetalle(pos, lin)


                If (estado = 0) Then
                    CType(grGastos.DataSource, DataTable).Rows(pos).Item("estado") = -2

                End If
                If (estado = 1) Then
                    CType(grGastos.DataSource, DataTable).Rows(pos).Item("estado") = -1
                End If
                If (estado = 2) Then
                    CType(grGastos.DataSource, DataTable).Rows(pos).Item("estado") = -1
                End If
                grGastos.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grGastos.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))


            End If
        End If

        CalcularGastos()
    End Sub
    Private Sub grdetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grGastos.MouseClick

        Try
            If (grGastos.RowCount >= 1) Then
                If (grGastos.CurrentColumn.Index = grGastos.RootTable.Columns("img").Index) Then
                    _prEliminarFila()
                End If
            End If
        Catch ex As Exception

        End Try



    End Sub

    Public Sub CalcularGastos()
        Dim TotalGasto As Double = 0
        Dim dt As DataTable = CType(grGastos.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1

            If (dt.Rows(i).Item("estado") >= 0) Then

                TotalGasto += dt.Rows(i).Item("Monto")

            End If

        Next

        tbGastos.Value = TotalGasto
        CalcularTotal()
    End Sub

    Private Sub tbEfectivoRecibido_ValueChanged(sender As Object, e As EventArgs) Handles tbEfectivoRecibido.ValueChanged


        tbDiferencia.Value = tbEfectivoRecibido.Value - tbEfectivoCalculado.Value

    End Sub

    Private Sub ReporteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReporteToolStripMenuItem.Click
        If (JGrM_Buscador.Row >= 0) Then
            ImprimirNotaSalida(tbCodigo.Text)
        End If
    End Sub
End Class