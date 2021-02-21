
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Public Class Tec_CierreCajaCajero
#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public FilaSeleccionada As Boolean = False
    Public _TabControl As SuperTabControl

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"
    Dim PersonalId As Integer = 0

    Dim TablaImagenes As DataTable
    Dim TablaInventario As DataTable
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim gs_DirPrograma As String = ""
    Dim gs_RutaImg As String = ""

    Dim TipoCambio As Double = 0



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
                    .WordWrap = True
                    .MaxLines = 4
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
        tbMontoInicial.Focus()


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



        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "Cierre Caja Cajero"

        P_Global._prCargarComboGenerico(cbSucursal, L_fnGeneralSucursales(), "aanumi", "Codigo", "aabdes", "Sucursal")

        _PMIniciarTodo()
        _prAsignarPermisos()


        _habilitarFocus()

    End Sub

    Private Sub _prCargarDetalleCobranza(id As Integer)
        Dim dt As New DataTable

        If (id = 0) Then

            dt = L_prListarCobrosCajaPendiente(tbFechaCierre.Value.ToString("yyyy/MM/dd"))
        Else
            dt = L_prListarCobranzaCierresCaja(id)
        End If


        grCobranzas.DataSource = dt
        grCobranzas.RetrieveStructure()
        grCobranzas.AlternatingColors = True
        'id	NroComprobanteVenta	Monto	FechaPago	PersonalId	NombrePersonal
        With grCobranzas.RootTable.Columns("FechaPago")
            .Width = 90
            .Caption = "Fecha Pago"
            .Visible = True
            .FormatString = "dd/MM/yyyy"
        End With
        With grCobranzas.RootTable.Columns("NroComprobanteVenta")
            .Width = 90
            .Caption = "Comprobante Venta"
            .Visible = True
        End With
        With grCobranzas.RootTable.Columns("PersonalId")
            .Width = 110
            .Visible = False
        End With
        With grCobranzas.RootTable.Columns("NombrePersonal")
            .Width = 110
            .Visible = False
        End With
        With grCobranzas.RootTable.Columns("id")
            .Width = 110
            .Visible = False
        End With

        With grCobranzas.RootTable.Columns("Monto")
            .Width = 120
            .Caption = "Monto Pagado"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With


        With grCobranzas
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

    Private Sub _prCargarDetalleEfectivoCortes(id As Integer)
        Dim dt As New DataTable


        dt = L_prListarEfectivoCortesCierre(id)



        grEfectivo.DataSource = dt
        grEfectivo.RetrieveStructure()
        grEfectivo.AlternatingColors = True
        'a.Id,a.CierreCajeroId,a.CorteBs,a.CantidadBs,a.SubTotalBs,a.CorteDolares,a.CantidadDolares,a.SubtotalDolares
        With grEfectivo.RootTable.Columns("id")
            .Width = 110
            .Visible = False
        End With

        With grEfectivo.RootTable.Columns("CierreCajeroId")
            .Width = 110
            .Visible = False
        End With


        With grEfectivo.RootTable.Columns("CorteBs")
            .Width = 100
            .Caption = "Dinero Bs"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grEfectivo.RootTable.Columns("CantidadBs")
            .Width = 100
            .Caption = "Cantidad Bs"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With


        With grEfectivo
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
    Private Sub _prCargarDetalleIngresoEgresos(id As Integer)
        Dim dt As New DataTable

        If (id = 0) Then

            dt = L_prListarMovimientosIngresoEgresoCierrePendiente(cbSucursal.Value, tbFechaCierre.Value.ToString("yyyy/MM/dd"))
        Else
            dt = L_prListarMovimientosIngresoEgresoCierre(id)
        End If


        grIngresosEgresoss.DataSource = dt
        grIngresosEgresoss.RetrieveStructure()
        grIngresosEgresoss.AlternatingColors = True
        'id	Fecha	Descripcion	Monto	IngresoEgreso	Movimiento	CajatipoMovimientoId	NombreTipoMovimiento	PersonalId	NombrePersonal
        With grIngresosEgresoss.RootTable.Columns("id")
            .Width = 110
            .Visible = False
        End With

        With grIngresosEgresoss.RootTable.Columns("CajatipoMovimientoId")
            .Width = 110
            .Visible = False
        End With
        With grIngresosEgresoss.RootTable.Columns("Movimiento")
            .Width = 110
            .Caption = "Movimiento"
            .Visible = True
        End With
        With grIngresosEgresoss.RootTable.Columns("NombreTipoMovimiento")
            .Width = 200
            .Caption = "Tipo Movimiento"
            .Visible = True
        End With

        With grIngresosEgresoss.RootTable.Columns("Fecha")
            .Width = 90
            .Caption = "Fecha"
            .Visible = True
            .FormatString = "dd/MM/yyyy"
        End With
        With grIngresosEgresoss.RootTable.Columns("Descripcion")
            .Width = 200
            .Caption = "Descripcion"
            .Visible = True
        End With


        With grIngresosEgresoss.RootTable.Columns("PersonalId")
            .Width = 110
            .Visible = False
        End With
        With grIngresosEgresoss.RootTable.Columns("NombrePersonal")
            .Width = 110
            .Visible = False
        End With
        With grIngresosEgresoss.RootTable.Columns("IngresoEgreso")
            .Width = 110
            .Visible = False
        End With

        With grIngresosEgresoss.RootTable.Columns("Monto")
            .Width = 120
            .Caption = "Monto"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With


        With grIngresosEgresoss
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

    Private Sub _prCargarDetalleVentas(id As Integer)
        Dim dt As New DataTable

        If (id = 0) Then

            dt = L_prListarVentaCierresCajaPendiente(PersonalId, cbSucursal.Value, tbFechaCierre.Value.ToString("yyyy/MM/dd"))
        Else
            dt = L_prListarVentaCierresCaja(id)
        End If


        grVentas.DataSource = dt
        grVentas.RetrieveStructure()
        grVentas.AlternatingColors = True
        'Id	FechaVenta	contado	contado$	tarjeta	TransferenciaBancaria	tipocambio	credito	totalbs	total$

        With grVentas.RootTable.Columns("Id")
            .Width = 90
            .Caption = "Cod Venta"
            .Visible = True
        End With
        With grVentas.RootTable.Columns("FechaVenta")
            .Width = 110
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tipocambio")
            .Width = 110
            .Visible = False
        End With

        With grVentas.RootTable.Columns("total$")
            .Width = 110
            .Visible = False
        End With
        With grVentas.RootTable.Columns("totalbs")
            .Width = 120
            .Caption = "Total Venta Bs"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With

        With grVentas.RootTable.Columns("credito")
            .Width = 100
            .Caption = "credito"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grVentas.RootTable.Columns("TransferenciaBancaria")
            .Width = 100
            .Caption = "Transferencia Bancaria"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grVentas.RootTable.Columns("tarjeta")
            .Width = 100
            .Caption = "Tarjeta"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With

        With grVentas.RootTable.Columns("contado")
            .Width = 100
            .Caption = "Contado Bs"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grVentas.RootTable.Columns("contado$")
            .Width = 100
            .Caption = "Contado $u$"
            .Visible = True
            .FormatString = "0.00"
            .AggregateFunction = AggregateFunction.Sum
        End With
        With grVentas
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
        With Highlighter2

            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbMontoInicial, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDetalle, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbEfectivoRecibido, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDiferencia, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbTotalCaja, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
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
        '     (@Id,@CodigoExterno ,@CodigoBarras ,@NombreProducto ,@DescripcionProducto ,
        '@StockMinimo ,@estado ,@CategoriaId ,@EmpresaId ,@ProveedorId ,@MarcaId ,
        '@AttributoId ,@FamiliaId ,
        '@UnidadVentaId ,@UnidadMaximaId ,@Conversion ,@newFecha,@newHora,@usuario )

        tbMontoInicial.IsInputReadOnly = False
        tbDetalle.ReadOnly = False


        tbEfectivoRecibido.IsInputReadOnly = False


        btnCargarDatos.Visible = True

        If (Global_Sucursal > 0) Then

            cbSucursal.Value = Global_Sucursal
            cbSucursal.ReadOnly = True
        Else
            cbSucursal.ReadOnly = False

        End If

    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbMontoInicial.IsInputReadOnly = True
        tbDetalle.ReadOnly = True
        btnCargarDatos.Visible = False

        tbEfectivoRecibido.IsInputReadOnly = True

    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""

        tbFechaCierre.Value = Now.Date
        tbPersonal.Clear()
        tbMontoInicial.Value = 0
        tbDetalle.Clear()

        tbMontoInicialTotal.Value = 0
        tbVentasContadoCobranza.Value = 0
        tbTotalPagos.Value = 0
        tbIngresos.Value = 0
        tbGastos.Value = 0
        tbTotalCaja.Value = 0
        tbTotalCortesEfectivo.Value = 0
        TbTotalTransferencia.Value = 0
        tbTotalTarjeta.Value = 0
        tbEfectivoRecibido.Value = 0
        tbDiferencia.Value = 0
        tbMontoInicial.Focus()

    End Sub


    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbEfectivoRecibido.BackColor = Color.White



    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        'ByRef _numi As String, _CodigoExterno As String,
        '                                        _CodigoBarra As String, _NombreProducto As String,
        '_Descripcion As String, _stockMinimo As Decimal, _estado As Integer, _CategoriaId As Integer,
        '_EmpresaId As Integer, _ProveedorId As Integer, _MarcaId As Integer,
        ''_AttributoId As Integer, _FamiliaId As Integer, _UnidadVentaId As Integer,
        '_UnidadMaximaId As Integer,
        ''_conversion As Double
        Dim res As Boolean
        Try
            res = L_prCierreCajeroInsertar(tbCodigo.Text, tbFechaCierre.Value.ToString("yyyy/MM/dd"), PersonalId, cbSucursal.Value, tbMontoInicial.Value, IIf(swEstado.Value = True, 1, 0), TipoCambio, tbVentasContadoCobranza.Value, tbTotalPagos.Value, tbIngresos.Value, tbGastos.Value, tbTotalCaja.Value, tbTotalCortesEfectivo.Value, TbTotalTransferencia.Value, tbTotalTarjeta.Value, tbEfectivoRecibido.Value, tbDiferencia.Value, tbDetalle.Text)

            If res Then



                ToastNotification.Show(Me, "Cierre Caja #".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar Cierre De Caja".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar Cierre De Caja".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = L_prCierreCajeroModificar(tbCodigo.Text, tbFechaCierre.Value.ToString("yyyy/MM/dd"), PersonalId, cbSucursal.Value, tbMontoInicial.Value, IIf(swEstado.Value = True, 1, 0), TipoCambio, tbVentasContadoCobranza.Value, tbTotalPagos.Value, tbIngresos.Value, tbGastos.Value, tbTotalCaja.Value, tbTotalCortesEfectivo.Value, TbTotalTransferencia.Value, tbTotalTarjeta.Value, tbEfectivoRecibido.Value, tbDiferencia.Value, tbDetalle.Text)


            If Res Then


                ToastNotification.Show(Me, "Codigo de Cierre De Caja".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar Cierre Caja".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar el Concepto Fijo".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbDetalle.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar Cierre Caja #" + tbCodigo.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_CierreCajero")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Cierre Caja".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar Cierre De Caja".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "
        'If tbEfectivoRecibido.Value <= 0 Then
        '    tbMonto.BackColor = Color.Red
        '    MEP.SetError(tbMonto, "Ingrese un Monto Valido")
        '    Mensaje = Mensaje + " Monto Valido Mayor a 0"
        '    _ok = False
        'Else
        '    tbMonto.BackColor = Color.White
        '    MEP.SetError(tbMonto, "")
        'End If
        'If (cbMotivoMovimiento.SelectedIndex < 0) Then
        '    cbMotivoMovimiento.BackColor = Color.Red
        '    MEP.SetError(cbMotivoMovimiento, "Seleccione un Motivo Movimiento Valido")
        '    Mensaje = Mensaje + Chr(13) + Chr(10) + " Motivo Movimiento"
        '    _ok = False
        'Else
        '    cbMotivoMovimiento.BackColor = Color.White
        '    MEP.SetError(cbMotivoMovimiento, "")
        'End If
        'If (cbCaja.SelectedIndex < 0) Then
        '    cbCaja.BackColor = Color.Red
        '    MEP.SetError(cbCaja, "Seleccione una Caja ")
        '    Mensaje = Mensaje + Chr(13) + Chr(10) + " Caja"
        '    _ok = False
        'Else
        '    cbCaja.BackColor = Color.White
        '    MEP.SetError(cbCaja, "")
        'End If

        If (cbSucursal.SelectedIndex < 0) Then
            cbSucursal.BackColor = Color.Red
            MEP.SetError(cbSucursal, "Seleccione una Sucursal ")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Sucursal"
            _ok = False
        Else
            cbSucursal.BackColor = Color.White
            MEP.SetError(cbSucursal, "")
        End If



        Highlighter2.UpdateHighlights()

        'If tbMonto.Value <= 0 Then
        '    tbMonto.Focus()
        '    ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
        '    Return _ok
        'End If

        'If (cbMotivoMovimiento.SelectedIndex < 0) Then
        '    ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
        '    cbMotivoMovimiento.Focus()
        '    Return _ok
        'End If
        'If (cbCaja.SelectedIndex < 0) Then

        '    ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
        '    cbCaja.Focus()
        '    Return _ok

        'End If
        If (cbSucursal.SelectedIndex < 0) Then

            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbSucursal.Focus()
            Return _ok

        End If


        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_CierreCajero")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        ''cierre.id,cierre.Fecha,cierre.PersonalId,p.NombrePersonal,cierre.SucursalId,alm.NombreAlmacen as Sucursal,
        'cierre.MontoInicial, cierre.EstadoCaja, IIf(cierre.EstadoCaja = 1,'Abierta','Cerrada') as Caja,cierre.TipoCambio,
        '    cierre.TotalVentas, cierre.TotalCobranza, cierre.TotalIngreso, cierre.TotalEgresos, cierre.TotalCaja, cierre.TotalEfectivo, cierre.TotalTransferencia,
        '    cierre.TotalTarjeta, cierre.TotalEfectivoRecibido, cierre.Diferencia, cierre.Observacion  
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "Movimiento Id", 40))
        listEstCeldas.Add(New Celda("Fecha", True, "Fecha", 70, "dd/MM/yyyy"))
        listEstCeldas.Add(New Celda("PersonalId", False))
        listEstCeldas.Add(New Celda("NombrePersonal", True, "Personal", 150))
        listEstCeldas.Add(New Celda("SucursalId", False))
        listEstCeldas.Add(New Celda("Sucursal", True, "Sucursal", 100))
        listEstCeldas.Add(New Celda("MontoInicial", False))

        listEstCeldas.Add(New Celda("EstadoCaja", False))
        listEstCeldas.Add(New Celda("Caja", True, "Estado Caja", 100))
        listEstCeldas.Add(New Celda("TipoCambio", False))
        listEstCeldas.Add(New Celda("TotalVentas", False))
        listEstCeldas.Add(New Celda("TotalCobranza", False))
        listEstCeldas.Add(New Celda("TotalIngreso", False))

        listEstCeldas.Add(New Celda("TotalEgresos", False))
        listEstCeldas.Add(New Celda("TotalCaja", False))
        listEstCeldas.Add(New Celda("TotalEfectivo", False))
        listEstCeldas.Add(New Celda("TotalTransferencia", False))
        listEstCeldas.Add(New Celda("TotalTarjeta", False))
        listEstCeldas.Add(New Celda("TotalEfectivoRecibido", True, "E. Recibido", 90, "0.00"))
        listEstCeldas.Add(New Celda("Diferencia", False))
        listEstCeldas.Add(New Celda("Observacion", True, "Detalle", 150))


        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        ''cierre.id,cierre.Fecha,cierre.PersonalId,p.NombrePersonal,cierre.SucursalId,alm.NombreAlmacen as Sucursal,
        'cierre.MontoInicial, cierre.EstadoCaja, IIf(cierre.EstadoCaja = 1,'Abierta','Cerrada') as Caja,cierre.TipoCambio,
        '    cierre.TotalVentas, cierre.TotalCobranza, cierre.TotalIngreso, cierre.TotalEgresos, cierre.TotalCaja, cierre.TotalEfectivo, cierre.TotalTransferencia,
        '    cierre.TotalTarjeta, cierre.TotalEfectivoRecibido, cierre.Diferencia, cierre.Observacion
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbFechaCierre.Value = .GetValue("Fecha")
            PersonalId = .GetValue("PersonalId")
            tbPersonal.Text = .GetValue("NombrePersonal").ToString
            cbSucursal.Value = .GetValue("SucursalId")
            tbMontoInicial.Value = .GetValue("MontoInicial")
            tbMontoInicialTotal.Value = .GetValue("MontoInicial")
            swEstado.Value = .GetValue("EstadoCaja")
            TipoCambio = .GetValue("TipoCambio")
            tbVentasContadoCobranza.Value = .GetValue("TotalVentas")
            tbTotalPagos.Value = .GetValue("TotalCobranza")
            tbIngresos.Value = .GetValue("TotalIngreso")
            tbGastos.Value = .GetValue("TotalEgresos")
            tbTotalCaja.Value = .GetValue("TotalCaja")
            tbTotalCortesEfectivo.Value = .GetValue("TotalEfectivo")
            TbTotalTransferencia.Value = .GetValue("TotalTransferencia")
            tbTotalTarjeta.Value = .GetValue("TotalTarjeta")
            tbEfectivoRecibido.Value = .GetValue("TotalEfectivoRecibido")
            tbDiferencia.Value = .GetValue("Diferencia")
            tbDetalle.Text = .GetValue("Observacion").ToString
            _prCargarDetalleVentas(.GetValue("Id"))
            _prCargarDetalleCobranza(.GetValue("Id"))
            _prCargarDetalleIngresoEgresos(.GetValue("Id"))
        End With

        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub

    Private Function P_fnObtenerID() As String
        Dim res As String = ""
        res = res + Now.Hour.ToString("00") + Now.Minute.ToString("00") + Now.Second.ToString("00") + "_" _
            + Now.Day.ToString("00") + Now.Month.ToString("00") + Now.Year.ToString("0000")
        Return res
    End Function

    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()
            TabControlPrincipal.SelectedTabIndex = 1
        Else
            TabControlPrincipal.SelectedTabIndex = 1
            '  Public _modulo As SideNavItem
            '_TabControl.SelectedTab = _modulo
            '_tab.Close()
            'Me.Close()
        End If
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _PMNuevo()
        Dim dt As DataTable = ListarPersonalById(Global_IdPersonal)
        If (dt.Rows.Count > 0) Then

            PersonalId = Global_IdPersonal
            tbPersonal.Text = dt.Rows(0).Item("Nombre")

        End If
        If (Global_Sucursal > 0) Then
            cbSucursal.ReadOnly = True
            cbSucursal.Value = Global_Sucursal
        Else
            cbSucursal.ReadOnly = False

        End If
        swEstado.Value = True
        btnCargarDatos.Enabled = False

    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If (JGrM_Buscador.GetValue("EstadoCaja") = 0) Then

            ToastNotification.Show(Me, "No Se Puede Modificar La Caja Por que Ya Se Encuentra con Estado Cerrada. Elimine la Caja Y Vuelva a Realizar El Cierre", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return

        End If

        _PMModificar()
        cbSucursal.ReadOnly = False
        btnCargarDatos.Enabled = True
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
    Private Sub JGrM_Buscador_DoubleClick(sender As Object, e As EventArgs) Handles JGrM_Buscador.DoubleClick
        If (JGrM_Buscador.Row >= 0) Then
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

            TabControlPrincipal.SelectedTabIndex = 0
            btnModificar.PerformClick()

        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then

            btnEliminar.PerformClick()

        End If
    End Sub



    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Private Sub btnCargarDatos_Click(sender As Object, e As EventArgs) Handles btnCargarDatos.Click
        _prCargarDetalleVentas(0)
        _prCargarDetalleCobranza(0)
        _prCargarDetalleIngresoEgresos(0)
        Calculartotales()




    End Sub

    Public Sub Calculartotales()
        Dim totalVentas As Double = 0
        Dim totalPago As Double = 0
        Dim totalIngresos As Double = 0
        Dim totalEgresos As Double = 0

        Dim totalTarjeta As Double = 0
        Dim Totaltransferencia As Double = 0

        '''Calculo de totales de venta

        Dim dtventas As DataTable = CType(grVentas.DataSource, DataTable)

        For i As Integer = 0 To dtventas.Rows.Count - 1 Step 1
            totalVentas += dtventas.Rows(i).Item("totalbs")
        Next
        '' Calculo total Cobranza
        Dim dtPagos As DataTable = CType(grCobranzas.DataSource, DataTable)
        For i As Integer = 0 To dtPagos.Rows.Count - 1 Step 1
            totalPago += dtPagos.Rows(i).Item("Monto")
        Next
        '''Calculo Ingresos o Egresos

        Dim dtMovimiento As DataTable = CType(grIngresosEgresoss.DataSource, DataTable)
        For i As Integer = 0 To dtMovimiento.Rows.Count - 1 Step 1

            Dim EsIngreso As Integer = dtMovimiento.Rows(i).Item("IngresoEgreso")
            If (EsIngreso = 1) Then
                totalIngresos += dtMovimiento.Rows(i).Item("Monto")
            Else
                totalEgresos += dtMovimiento.Rows(i).Item("Monto")
            End If

        Next


        tbVentasContadoCobranza.Value = totalVentas
        tbTotalPagos.Value = totalPago
        tbIngresos.Value = totalIngresos
        tbGastos.Value = totalEgresos
        tbTotalCaja.Value = (tbMontoInicial.Value + totalVentas + totalPago + totalIngresos) - totalEgresos

    End Sub

#End Region
End Class