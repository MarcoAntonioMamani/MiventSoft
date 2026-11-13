Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports System.Globalization
Imports System.Text.RegularExpressions

'=====================================================================
'  AUDITORIA DE REGISTROS (programa btnAuditoriaRegistros) - la misma pantalla que la web
'  Lo MODIFICADO y ELIMINADO de ventas, compras y movimientos de inventario, hecho desde la web
'  o desde el escritorio: quien, cuando, desde que equipo y que cambio (antes / despues).
'  SP: MAM_Auditoria (scripts 07 al 12 de MiventSoftWeb\server\sql)
'   @tipo 3 lista, @tipo 4 detalle (fotos antes y despues en XML), @tipo 5 usuarios
'  La cabecera se compara campo por campo y el detalle linea por linea (por producto + lote):
'  igual / modificado / agregado / quitado. Solo lectura: la auditoria no se puede modificar ni borrar.
'  Imprimir: HTML en la carpeta temporal que se abre en el navegador (igual que el reporte de clientes).
'=====================================================================
Public Class Tec_AuditoriaRegistros

#Region "Variables"
    'Campo de la foto que se muestra: nombre en el XML, titulo y tipo (texto | num | fecha | sino)
    Private Class CampoFoto
        Public Campo As String
        Public Titulo As String
        Public Tipo As String
        Public Sub New(_Campo As String, _Titulo As String, Optional _Tipo As String = "texto")
            Campo = _Campo
            Titulo = _Titulo
            Tipo = _Tipo
        End Sub
    End Class

    'Foto del documento guardada por MAM_Auditoria: <foto><cab .../><det .../><det .../></foto>
    Private Class FotoAuditoria
        Public Cab As Dictionary(Of String, String) = Nothing
        Public Det As New List(Of Dictionary(Of String, String))
    End Class

    'Registro ya comparado: lo usan la pantalla y la impresion
    Private Class RegistroAuditoria
        Public Fila As DataRow
        Public Cabecera As DataTable   'Campo, Antes, Despues, Cambio (1 = cambio)
        Public Detalle As DataTable    'Estado, Producto, <campo>_A, <campo>_D, <campo>_C (1 = cambio)
        Public CamposDetalle As List(Of CampoFoto)
        Public Eliminado As Boolean
        Public Agregados As Integer = 0
        Public Quitados As Integer = 0
        Public Modificados As Integer = 0
    End Class

    Const PRECISION_IP As Integer = 25000   'metros: ubicacion aproximada por IP (no GPS), igual que la web
    Const REPORTE_MAX As Integer = 300      'registros como maximo en el reporte
    Const COLOR_CAMBIO As String = "#FFE69A"

    Dim TablaLista As DataTable = Nothing
    Dim IdMostrado As Integer = -1
    Dim RegistroActual As RegistroAuditoria = Nothing
    Dim Cargando As Boolean = False
#End Region

#Region "Inicio"
    Private Sub Tec_AuditoriaRegistros_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    'La lista y el detalle se reparten el ancho de la pantalla (mitad y mitad), sea cual sea la resolucion
    Dim DivisorAjustado As Boolean = False
    Private Sub SplitContainer1_SizeChanged(sender As Object, e As EventArgs) Handles SplitContainer1.SizeChanged
        If (DivisorAjustado OrElse SplitContainer1.Width < SplitContainer1.Panel1MinSize + SplitContainer1.Panel2MinSize + 20) Then
            Return
        End If
        SplitContainer1.SplitterDistance = Math.Max(SplitContainer1.Panel1MinSize, CInt(SplitContainer1.Width * 0.5))
        DivisorAjustado = Me.Visible
    End Sub

    Public Sub _prIniciarTodo()
        Me.Text = "AUDITORIA DE REGISTROS"
        cbFechaDesde.Value = New Date(Now.Year, Now.Month, 1)
        cbFechaHasta.Value = Now.Date

        _prCargarComboModulo()
        _prCargarComboOperacion()
        _prCargarComboUsuario()
        _prMostrarSinSeleccion("Seleccione un registro de la lista para ver quien, cuando, desde donde y que cambio (antes / despues).")

        _prBuscar()
    End Sub

    Private Sub _prCargarComboModulo()
        Dim dt As New DataTable
        dt.Columns.Add("Id", GetType(Integer))
        dt.Columns.Add("Nombre", GetType(String))
        dt.Rows.Add(-1, "Todos")
        dt.Rows.Add(1, "Ventas")
        dt.Rows.Add(2, "Compras")
        dt.Rows.Add(3, "Movimientos de inventario")
        _prCargarCombo(cbModulo, dt, "Modulo", 200)
    End Sub

    Private Sub _prCargarComboOperacion()
        Dim dt As New DataTable
        dt.Columns.Add("Id", GetType(Integer))
        dt.Columns.Add("Nombre", GetType(String))
        dt.Rows.Add(-1, "Todos")
        dt.Rows.Add(1, "Modificados")
        dt.Rows.Add(2, "Eliminados")
        _prCargarCombo(cbOperacion, dt, "Operacion", 150)
    End Sub

    'Usuarios que tienen registros en la auditoria (MAM_Auditoria @tipo 5)
    Private Sub _prCargarComboUsuario()
        Dim dt As New DataTable
        dt.Columns.Add("Id", GetType(String))
        dt.Columns.Add("Nombre", GetType(String))
        dt.Rows.Add("", "Todos")
        Try
            Dim dtUsuarios As DataTable = L_prAuditoriaUsuarios()
            If (dtUsuarios.Columns.Contains("Usuario")) Then
                For Each fila As DataRow In dtUsuarios.Rows
                    Dim nombre As String = _fnTexto(fila.Item("Usuario"))
                    If (nombre <> String.Empty) Then
                        dt.Rows.Add(nombre, nombre)
                    End If
                Next
            End If
        Catch ex As Exception
            'sin la lista de usuarios igual se puede buscar (Todos)
        End Try
        _prCargarCombo(cbUsuario, dt, "Usuario", 250)
    End Sub

    Private Sub _prCargarCombo(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo, dt As DataTable, titulo As String, ancho As Integer)
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("Id").Width = 70
            .DropDownList.Columns("Id").Visible = False
            .DropDownList.Columns.Add("Nombre").Width = ancho
            .DropDownList.Columns("Nombre").Caption = titulo
            .ValueMember = "Id"
            .DisplayMember = "Nombre"
            .DataSource = dt
            .Refresh()
        End With
        mCombo.SelectedIndex = 0
    End Sub
#End Region

#Region "Lista"
    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        _prBuscar()
    End Sub

    Private Sub Tec_AuditoriaRegistros_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.F5) Then
            e.Handled = True
            _prBuscar()
        End If
    End Sub

    Private Sub tbDocumento_KeyDown(sender As Object, e As KeyEventArgs) Handles tbDocumento.KeyDown
        If (e.KeyCode = Keys.Enter) Then
            e.Handled = True
            e.SuppressKeyPress = True
            _prBuscar()
        End If
    End Sub

    Private Sub _prBuscar()
        If (cbFechaDesde.Value.Date > cbFechaHasta.Value.Date) Then
            _prMensaje("La fecha Desde no puede ser mayor que la fecha Hasta", False)
            cbFechaDesde.Focus()
            Return
        End If
        Dim documento As Integer = -1
        If (tbDocumento.Text.Trim <> String.Empty) Then
            If (Not Integer.TryParse(tbDocumento.Text.Trim, documento) OrElse documento <= 0) Then
                _prMensaje("El documento debe ser un numero mayor a 0", False)
                tbDocumento.Focus()
                Return
            End If
        End If

        Try
            Me.Cursor = Cursors.WaitCursor
            Dim dt As DataTable = L_prAuditoriaListar(cbFechaDesde.Value.Date, cbFechaHasta.Value.Date,
                                                      _fnValorEntero(cbModulo), _fnValorEntero(cbOperacion),
                                                      _fnTexto(cbUsuario.Value), documento)
            If (Not dt.Columns.Contains("Id")) Then
                'el SP no respondio (ya se mostro el error de SQL): falta instalar los scripts 07 al 12
                _prCargarLista(Nothing)
                _prMensaje("No se pudo leer la auditoria. Verifique que esten instalados los scripts 07 al 12 en la base de datos", False)
                Return
            End If
            _prCargarLista(dt)
        Catch ex As Exception
            _prMensaje("Error al buscar la auditoria: " + ex.Message, False)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    'Arma la tabla de la lista en el orden de las columnas de la pantalla
    Private Sub _prCargarLista(dt As DataTable)
        Dim lista As New DataTable
        lista.Columns.Add("Id", GetType(Integer))
        lista.Columns.Add("Fecha", GetType(DateTime))
        lista.Columns.Add("Operacion", GetType(String))
        lista.Columns.Add("Modulo", GetType(String))
        lista.Columns.Add("DocumentoId", GetType(Integer))
        lista.Columns.Add("Descripcion", GetType(String))
        lista.Columns.Add("Usuario", GetType(String))
        lista.Columns.Add("Origen", GetType(String))
        lista.Columns.Add("Equipo", GetType(String))
        lista.Columns.Add("TotalAntes", GetType(Decimal))
        lista.Columns.Add("TotalDespues", GetType(Decimal))

        Dim modificados As Integer = 0
        Dim eliminados As Integer = 0
        Dim usuarios As New HashSet(Of String)
        If (dt IsNot Nothing) Then
            For Each f As DataRow In dt.Rows
                Dim nueva As DataRow = lista.NewRow()
                nueva.Item("Id") = _fnEntero(f.Item("Id"))
                nueva.Item("Fecha") = f.Item("Fecha")
                nueva.Item("Operacion") = _fnNombreOperacion(_fnEntero(f.Item("Operacion")))
                nueva.Item("Modulo") = _fnNombreModulo(_fnEntero(f.Item("Modulo")))
                nueva.Item("DocumentoId") = _fnEntero(f.Item("DocumentoId"))
                nueva.Item("Descripcion") = _fnTexto(f.Item("Descripcion"))
                nueva.Item("Usuario") = _fnTexto(f.Item("Usuario"))
                nueva.Item("Origen") = _fnOrigen(f.Item("Aplicacion"))
                nueva.Item("Equipo") = _fnTexto(f.Item("Equipo"))
                nueva.Item("TotalAntes") = f.Item("TotalAntes")
                nueva.Item("TotalDespues") = f.Item("TotalDespues")
                lista.Rows.Add(nueva)

                If (_fnEntero(f.Item("Operacion")) = 2) Then
                    eliminados = eliminados + 1
                Else
                    modificados = modificados + 1
                End If
                usuarios.Add(_fnTexto(f.Item("Usuario")))
            Next
        End If

        Cargando = True
        TablaLista = lista
        grAuditoria.DataSource = lista
        grAuditoria.RetrieveStructure()
        _prFormatearLista()
        Cargando = False

        lbResumen.Text = "Registros: " + lista.Rows.Count.ToString +
                         "     Modificados: " + modificados.ToString +
                         "     Eliminados: " + eliminados.ToString +
                         "     Usuarios: " + usuarios.Count.ToString +
                         "     (del " + cbFechaDesde.Value.ToString("dd/MM/yyyy") + " al " + cbFechaHasta.Value.ToString("dd/MM/yyyy") + ")"

        IdMostrado = -1
        RegistroActual = Nothing
        If (lista.Rows.Count = 0) Then
            _prMostrarSinSeleccion("No hay registros modificados ni eliminados con los filtros seleccionados.")
        Else
            _prMostrarDetalleFilaActual()
        End If
    End Sub

    Private Sub _prFormatearLista()
        For Each col As GridEXColumn In grAuditoria.RootTable.Columns
            col.Visible = False
        Next
        _prColumna(grAuditoria, "Fecha", "FECHA Y HORA", 140, "dd/MM/yyyy HH:mm")
        _prColumna(grAuditoria, "Operacion", "OPERACION", 100)
        _prColumna(grAuditoria, "Modulo", "MODULO", 105)
        _prColumna(grAuditoria, "DocumentoId", "NRO.", 60, "", True)
        _prColumna(grAuditoria, "Descripcion", "DOCUMENTO", 220)
        _prColumna(grAuditoria, "Usuario", "USUARIO", 150)
        _prColumna(grAuditoria, "Origen", "DESDE", 80)
        _prColumna(grAuditoria, "Equipo", "EQUIPO", 130)
        _prColumna(grAuditoria, "TotalAntes", "TOTAL ANTES", 95, "#,##0.00", True)
        _prColumna(grAuditoria, "TotalDespues", "TOTAL DESPUES", 95, "#,##0.00", True)

        With grAuditoria
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
            .AllowEdit = InheritableBoolean.False
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .RecordNavigator = True
            .RecordNavigatorText = "REGISTROS"
            .RowHeaders = InheritableBoolean.True
        End With

        'Eliminado en rojo y Modificado en naranja (solo la columna Operacion)
        grAuditoria.RootTable.FormatConditions.Clear()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grAuditoria.RootTable.Columns("Operacion"), ConditionOperator.Equal, "Eliminado")
        fc.FormatStyle.ForeColor = Color.FromArgb(192, 0, 0)
        fc.FormatStyle.FontBold = TriState.True
        fc.TargetColumn = grAuditoria.RootTable.Columns("Operacion")
        grAuditoria.RootTable.FormatConditions.Add(fc)

        fc = New GridEXFormatCondition(grAuditoria.RootTable.Columns("Operacion"), ConditionOperator.Equal, "Modificado")
        fc.FormatStyle.ForeColor = Color.FromArgb(200, 110, 0)
        fc.FormatStyle.FontBold = TriState.True
        fc.TargetColumn = grAuditoria.RootTable.Columns("Operacion")
        grAuditoria.RootTable.FormatConditions.Add(fc)
    End Sub

    Private Sub grAuditoria_SelectionChanged(sender As Object, e As EventArgs) Handles grAuditoria.SelectionChanged
        If (Cargando) Then
            Return
        End If
        _prMostrarDetalleFilaActual()
    End Sub

    Private Sub _prMostrarDetalleFilaActual()
        Dim fila As GridEXRow = grAuditoria.GetRow()
        If (fila Is Nothing OrElse fila.RowType <> RowType.Record) Then
            Return
        End If
        Dim id As Integer = _fnEntero(fila.Cells("Id").Value)
        If (id > 0) Then
            _prMostrarDetalle(id)
        End If
    End Sub
#End Region

#Region "Detalle"
    Private Sub _prMostrarDetalle(id As Integer)
        If (id = IdMostrado) Then
            Return
        End If
        Try
            Me.Cursor = Cursors.WaitCursor
            Dim dt As DataTable = L_prAuditoriaDetalle(id)
            If (dt.Rows.Count = 0 OrElse Not dt.Columns.Contains("Antes")) Then
                _prMostrarSinSeleccion("No se encontro el registro de auditoria. Vuelva a buscar.")
                Return
            End If
            RegistroActual = _fnArmarRegistro(dt.Rows(0))
            IdMostrado = id
            _prPintarDetalle(RegistroActual)
        Catch ex As Exception
            IdMostrado = -1
            RegistroActual = Nothing
            _prMostrarSinSeleccion("No se pudo mostrar el registro: " + ex.Message)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub _prMostrarSinSeleccion(texto As String)
        lbSinSeleccion.Text = texto
        PanelDetalle.Visible = False
        lbSinSeleccion.Visible = True
        btnImprimirRegistro.Enabled = False
    End Sub

    Private Sub _prPintarDetalle(r As RegistroAuditoria)
        Dim f As DataRow = r.Fila

        'Titulo: VENTA NRO. 125  -  ELIMINADO
        lbTituloDetalle.Text = _fnNombreModulo(_fnEntero(f.Item("Modulo"))).ToUpper + " NRO. " + _fnEntero(f.Item("DocumentoId")).ToString +
                               "  -  " + _fnNombreOperacion(_fnEntero(f.Item("Operacion"))).ToUpper +
                               If(_fnTexto(f.Item("Descripcion")) <> String.Empty, "   (" + _fnTexto(f.Item("Descripcion")) + ")", "")
        lbTituloDetalle.ForeColor = If(r.Eliminado, Color.FromArgb(192, 0, 0), Color.FromArgb(200, 110, 0))

        lbQuien.Text = _fnLineaQuien(f)
        lbEquipo.Text = _fnLineaEquipo(f)
        lbNavegador.Text = _fnLineaAplicacion(f)
        _prPintarUbicacion(f)

        'Cabecera campo por campo
        grCabecera.DataSource = r.Cabecera
        grCabecera.RetrieveStructure()
        _prColumna(grCabecera, "Campo", "CAMPO", 150)
        _prColumna(grCabecera, "Antes", If(r.Eliminado, "VALOR AL ELIMINAR", "ANTES"), 270)
        _prColumna(grCabecera, "Despues", "DESPUES", 270)
        grCabecera.RootTable.Columns("Despues").Visible = Not r.Eliminado
        grCabecera.RootTable.Columns("Cambio").Visible = False
        _prGrillaSoloLectura(grCabecera)
        grCabecera.RootTable.FormatConditions.Clear()
        Dim fc As New GridEXFormatCondition(grCabecera.RootTable.Columns("Cambio"), ConditionOperator.Equal, 1)
        fc.FormatStyle.BackColor = ColorTranslator.FromHtml(COLOR_CAMBIO)
        fc.FormatStyle.FontBold = TriState.True
        grCabecera.RootTable.FormatConditions.Add(fc)
        lbCabecera.Text = If(r.Eliminado, "CABECERA (como estaba al eliminar)", "CABECERA (en amarillo lo que cambio)")

        'Detalle linea por linea
        grDetalle.DataSource = r.Detalle
        grDetalle.RetrieveStructure()
        For Each col As GridEXColumn In grDetalle.RootTable.Columns
            col.Visible = False
        Next
        _prColumna(grDetalle, "Estado", "ESTADO", 100)
        _prColumna(grDetalle, "Producto", "PRODUCTO", 230)
        For Each c As CampoFoto In r.CamposDetalle
            If (c.Campo = "Producto") Then
                Continue For
            End If
            If (r.Eliminado) Then
                _prColumna(grDetalle, c.Campo + "_A", c.Titulo.ToUpper, 100, "", True)
            Else
                _prColumna(grDetalle, c.Campo + "_A", c.Titulo.ToUpper + " ANTES", 120, "", True)
                _prColumna(grDetalle, c.Campo + "_D", c.Titulo.ToUpper + " AHORA", 120, "", True)
            End If
        Next
        _prGrillaSoloLectura(grDetalle)
        _prCondicionesDetalle(r)

        If (r.Eliminado) Then
            lbDetalleResumen.Text = "DETALLE AL ELIMINAR: " + r.Detalle.Rows.Count.ToString + " linea(s)"
        Else
            lbDetalleResumen.Text = "DETALLE:  " + r.Modificados.ToString + " modificada(s)  -  " + r.Agregados.ToString +
                                    " agregada(s)  -  " + r.Quitados.ToString + " quitada(s)"
        End If

        lbSinSeleccion.Visible = False
        PanelDetalle.Visible = True
        btnImprimirRegistro.Enabled = True
    End Sub

    'Colores del detalle: agregado verde, quitado / eliminado rojo, modificado amarillo y el valor que cambio resaltado
    Private Sub _prCondicionesDetalle(r As RegistroAuditoria)
        Dim tabla As GridEXTable = grDetalle.RootTable
        tabla.FormatConditions.Clear()
        Dim fc As GridEXFormatCondition

        fc = New GridEXFormatCondition(tabla.Columns("Estado"), ConditionOperator.Equal, "Agregado")
        fc.FormatStyle.BackColor = Color.FromArgb(222, 245, 228)
        tabla.FormatConditions.Add(fc)

        fc = New GridEXFormatCondition(tabla.Columns("Estado"), ConditionOperator.Equal, "Quitado")
        fc.FormatStyle.BackColor = Color.FromArgb(253, 226, 226)
        tabla.FormatConditions.Add(fc)

        If (Not r.Eliminado) Then
            fc = New GridEXFormatCondition(tabla.Columns("Estado"), ConditionOperator.Equal, "Modificado")
            fc.FormatStyle.BackColor = Color.FromArgb(255, 248, 220)
            tabla.FormatConditions.Add(fc)

            For Each c As CampoFoto In r.CamposDetalle
                If (c.Campo = "Producto") Then
                    Continue For
                End If
                fc = New GridEXFormatCondition(tabla.Columns(c.Campo + "_C"), ConditionOperator.Equal, 1)
                fc.FormatStyle.BackColor = ColorTranslator.FromHtml(COLOR_CAMBIO)
                fc.FormatStyle.FontBold = TriState.True
                fc.TargetColumn = tabla.Columns(c.Campo + "_D")
                tabla.FormatConditions.Add(fc)
            Next
        End If
    End Sub

    Private Sub _prPintarUbicacion(f As DataRow)
        lnkUbicacion.Tag = Nothing
        Dim ub As String = _fnTextoUbicacion(f)
        If (ub = String.Empty) Then
            lnkUbicacion.Text = "Ubicacion: no registrada"
            lnkUbicacion.LinkArea = New LinkArea(0, 0)
            Return
        End If
        Const VER As String = "Ver en el mapa"
        lnkUbicacion.Text = "Ubicacion: " + ub + "     " + VER
        lnkUbicacion.LinkArea = New LinkArea(lnkUbicacion.Text.Length - VER.Length, VER.Length)
        lnkUbicacion.Tag = _fnUrlMapa(f)
    End Sub

    'El mapa se abre en el navegador solo si el usuario lo pide (la pantalla no carga nada de internet)
    Private Sub lnkUbicacion_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkUbicacion.LinkClicked
        If (lnkUbicacion.Tag Is Nothing) Then
            Return
        End If
        Try
            Process.Start(lnkUbicacion.Tag.ToString)
        Catch ex As Exception
            _prMensaje("No se pudo abrir el mapa: " + ex.Message, False)
        End Try
    End Sub
#End Region

#Region "Comparacion antes / despues (igual que la web: server\src\rutas\configuracion\auditoria.routes.js)"
    'Campos que se muestran de cada modulo
    Private Function _fnCampos(modulo As Integer, cabecera As Boolean) As List(Of CampoFoto)
        Dim l As New List(Of CampoFoto)
        Select Case modulo
            Case 1 'Venta
                If (cabecera) Then
                    l.Add(New CampoFoto("Fecha", "Fecha", "fecha"))
                    l.Add(New CampoFoto("Sucursal", "Sucursal"))
                    l.Add(New CampoFoto("Cliente", "Cliente"))
                    l.Add(New CampoFoto("Personal", "Vendedor"))
                    l.Add(New CampoFoto("TipoVenta", "Tipo de venta"))
                    l.Add(New CampoFoto("VencimientoCredito", "Vence credito", "fecha"))
                    l.Add(New CampoFoto("Glosa", "Glosa"))
                    l.Add(New CampoFoto("Facturado", "Facturado", "sino"))
                    l.Add(New CampoFoto("Descuento", "Descuento", "num"))
                    l.Add(New CampoFoto("Total", "Total", "num"))
                Else
                    l.Add(New CampoFoto("Producto", "Producto"))
                    l.Add(New CampoFoto("Cantidad", "Cantidad", "num"))
                    l.Add(New CampoFoto("Precio", "Precio", "num"))
                    l.Add(New CampoFoto("Descuento", "Descuento", "num"))
                    l.Add(New CampoFoto("Total", "Total", "num"))
                End If
            Case 2 'Compra
                If (cabecera) Then
                    l.Add(New CampoFoto("Fecha", "Fecha", "fecha"))
                    l.Add(New CampoFoto("Almacen", "Almacen"))
                    l.Add(New CampoFoto("Proveedor", "Proveedor"))
                    l.Add(New CampoFoto("TipoCompra", "Tipo de compra"))
                    l.Add(New CampoFoto("VencimientoCredito", "Vence credito", "fecha"))
                    l.Add(New CampoFoto("Glosa", "Glosa"))
                    l.Add(New CampoFoto("Descuento", "Descuento", "num"))
                    l.Add(New CampoFoto("Total", "Total", "num"))
                Else
                    l.Add(New CampoFoto("Producto", "Producto"))
                    l.Add(New CampoFoto("Cantidad", "Cantidad", "num"))
                    l.Add(New CampoFoto("PrecioCosto", "P. Costo", "num"))
                    l.Add(New CampoFoto("PrecioVenta", "P. Venta", "num"))
                    l.Add(New CampoFoto("Total", "Total", "num"))
                End If
            Case 3 'Movimiento de inventario
                If (cabecera) Then
                    l.Add(New CampoFoto("Fecha", "Fecha", "fecha"))
                    l.Add(New CampoFoto("Concepto", "Concepto"))
                    l.Add(New CampoFoto("Deposito", "Deposito"))
                    l.Add(New CampoFoto("DepositoDestino", "Deposito destino"))
                    l.Add(New CampoFoto("IdMovimientoDestino", "Movimiento destino"))
                    l.Add(New CampoFoto("Observacion", "Observacion"))
                    l.Add(New CampoFoto("Total", "Cantidad total", "num"))
                Else
                    l.Add(New CampoFoto("Producto", "Producto"))
                    l.Add(New CampoFoto("Cantidad", "Cantidad", "num"))
                End If
        End Select
        Return l
    End Function

    'Fila de MAM_Auditoria @tipo 4 -> cabecera y detalle comparados
    Private Function _fnArmarRegistro(fila As DataRow) As RegistroAuditoria
        Dim r As New RegistroAuditoria
        r.Fila = fila
        Dim modulo As Integer = _fnEntero(fila.Item("Modulo"))
        Dim camposCab As List(Of CampoFoto) = _fnCampos(modulo, True)
        r.CamposDetalle = _fnCampos(modulo, False)
        If (camposCab.Count = 0) Then
            Throw New Exception("Modulo de auditoria desconocido (" + modulo.ToString + ")")
        End If

        Dim antes As FotoAuditoria = _fnLeerFoto(_fnTexto(fila.Item("Antes")))
        Dim despues As FotoAuditoria = _fnLeerFoto(_fnTexto(fila.Item("Despues")))
        r.Eliminado = (despues Is Nothing)

        'Cabecera: un renglon por campo (sin los campos vacios en los dos lados)
        r.Cabecera = New DataTable
        r.Cabecera.Columns.Add("Campo", GetType(String))
        r.Cabecera.Columns.Add("Antes", GetType(String))
        r.Cabecera.Columns.Add("Despues", GetType(String))
        r.Cabecera.Columns.Add("Cambio", GetType(Integer))
        Dim cabAntes As Dictionary(Of String, String) = If(antes Is Nothing, Nothing, antes.Cab)
        Dim cabDespues As Dictionary(Of String, String) = If(despues Is Nothing, Nothing, despues.Cab)
        For Each c As CampoFoto In camposCab
            Dim va As String = _fnNormal(_fnAtributo(cabAntes, c.Campo), c.Tipo)
            Dim vd As String = _fnNormal(_fnAtributo(cabDespues, c.Campo), c.Tipo)
            If (va = String.Empty And vd = String.Empty) Then
                Continue For
            End If
            Dim cambio As Boolean = (cabAntes IsNot Nothing AndAlso cabDespues IsNot Nothing AndAlso va <> vd)
            r.Cabecera.Rows.Add(c.Titulo, _fnMostrar(va, c.Tipo), _fnMostrar(vd, c.Tipo), If(cambio, 1, 0))
        Next

        'Detalle: se empareja por producto + lote (en orden)
        r.Detalle = New DataTable
        r.Detalle.Columns.Add("Estado", GetType(String))
        r.Detalle.Columns.Add("Producto", GetType(String))
        For Each c As CampoFoto In r.CamposDetalle
            If (c.Campo = "Producto") Then
                Continue For
            End If
            r.Detalle.Columns.Add(c.Campo + "_A", GetType(String))
            r.Detalle.Columns.Add(c.Campo + "_D", GetType(String))
            r.Detalle.Columns.Add(c.Campo + "_C", GetType(Integer))
        Next

        Dim lineasAntes As List(Of Dictionary(Of String, String)) = If(antes Is Nothing, New List(Of Dictionary(Of String, String)), antes.Det)
        If (Not r.Eliminado) Then
            Dim usadas(lineasAntes.Count) As Boolean
            For Each d As Dictionary(Of String, String) In despues.Det
                Dim ia As Integer = -1
                For i As Integer = 0 To lineasAntes.Count - 1 Step 1
                    If (Not usadas(i) AndAlso _fnClaveLinea(lineasAntes(i)) = _fnClaveLinea(d)) Then
                        ia = i
                        Exit For
                    End If
                Next
                If (ia >= 0) Then
                    usadas(ia) = True
                    _prAgregarLinea(r, "", lineasAntes(ia), d)
                Else
                    _prAgregarLinea(r, "Agregado", Nothing, d)
                End If
            Next
            For i As Integer = 0 To lineasAntes.Count - 1 Step 1
                If (Not usadas(i)) Then
                    _prAgregarLinea(r, "Quitado", lineasAntes(i), Nothing)
                End If
            Next
        Else
            For Each a As Dictionary(Of String, String) In lineasAntes
                _prAgregarLinea(r, "Eliminado", a, Nothing)
            Next
        End If
        Return r
    End Function

    'estado "" = linea que esta antes y despues: queda "Modificado" o "Igual" segun los cambios
    Private Sub _prAgregarLinea(r As RegistroAuditoria, estado As String, a As Dictionary(Of String, String), d As Dictionary(Of String, String))
        Dim fila As DataRow = r.Detalle.NewRow()
        Dim cambios As Integer = 0
        For Each c As CampoFoto In r.CamposDetalle
            Dim va As String = _fnNormal(_fnAtributo(a, c.Campo), c.Tipo)
            Dim vd As String = _fnNormal(_fnAtributo(d, c.Campo), c.Tipo)
            If (c.Campo = "Producto") Then
                fila.Item("Producto") = If(d IsNot Nothing, vd, va)
                Continue For
            End If
            Dim cambio As Boolean = (a IsNot Nothing AndAlso d IsNot Nothing AndAlso va <> vd)
            If (cambio) Then
                cambios = cambios + 1
            End If
            fila.Item(c.Campo + "_A") = _fnMostrar(va, c.Tipo)
            fila.Item(c.Campo + "_D") = _fnMostrar(vd, c.Tipo)
            fila.Item(c.Campo + "_C") = If(cambio, 1, 0)
        Next
        If (estado = String.Empty) Then
            estado = If(cambios > 0, "Modificado", "Igual")
        End If
        fila.Item("Estado") = estado
        r.Detalle.Rows.Add(fila)

        Select Case estado
            Case "Agregado"
                r.Agregados = r.Agregados + 1
            Case "Quitado"
                r.Quitados = r.Quitados + 1
            Case "Modificado"
                r.Modificados = r.Modificados + 1
        End Select
    End Sub

    Private Function _fnClaveLinea(linea As Dictionary(Of String, String)) As String
        Return _fnAtributo(linea, "ProductoId") + "|" + _fnAtributo(linea, "Lote")
    End Function

    'Lee la foto XML (FOR XML RAW: los atributos ya vienen decodificados por XElement)
    Private Function _fnLeerFoto(xml As String) As FotoAuditoria
        If (String.IsNullOrWhiteSpace(xml)) Then
            Return Nothing
        End If
        Dim foto As New FotoAuditoria
        Dim raiz As XElement = XElement.Parse("<r>" + xml + "</r>")
        For Each el As XElement In raiz.Descendants()
            If (el.Name.LocalName = "cab" OrElse el.Name.LocalName = "det") Then
                Dim valores As New Dictionary(Of String, String)
                For Each at As XAttribute In el.Attributes()
                    valores(at.Name.LocalName) = at.Value
                Next
                If (el.Name.LocalName = "cab") Then
                    foto.Cab = valores
                Else
                    foto.Det.Add(valores)
                End If
            End If
        Next
        Return foto
    End Function

    Private Function _fnAtributo(valores As Dictionary(Of String, String), campo As String) As String
        If (valores Is Nothing OrElse Not valores.ContainsKey(campo)) Then
            Return ""
        End If
        Return valores(campo)
    End Function

    'Valor comparable segun el tipo (los decimales vienen como "12.50" o "12.5" segun la columna)
    Private Function _fnNormal(valor As String, tipo As String) As String
        Dim t As String = If(valor Is Nothing, "", valor).ToString.Trim
        If (t = String.Empty) Then
            Return ""
        End If
        Select Case tipo
            Case "num"
                Dim d As Decimal
                If (Decimal.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, d)) Then
                    Return d.ToString("0.############", CultureInfo.InvariantCulture)
                End If
            Case "fecha"
                Dim f As DateTime
                If (DateTime.TryParse(t, CultureInfo.InvariantCulture, DateTimeStyles.None, f)) Then
                    Return f.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                End If
            Case "sino"
                Return If(t = "1" OrElse t.ToLower = "true", "Si", "No")
        End Select
        Return t
    End Function

    'Valor para mostrar (el normalizado se usa solo para comparar)
    Private Function _fnMostrar(normal As String, tipo As String) As String
        If (normal = String.Empty) Then
            Return ""
        End If
        Select Case tipo
            Case "num"
                Dim d As Decimal
                If (Decimal.TryParse(normal, NumberStyles.Float, CultureInfo.InvariantCulture, d)) Then
                    Return d.ToString("#,##0.00##", CultureInfo.InvariantCulture)
                End If
            Case "fecha"
                Dim f As DateTime
                If (DateTime.TryParseExact(normal, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, f)) Then
                    Return f.ToString("dd/MM/yyyy")
                End If
        End Select
        Return normal
    End Function
#End Region

#Region "Datos del registro (quien, equipo, ubicacion)"
    Private Function _fnNombreModulo(modulo As Integer) As String
        Select Case modulo
            Case 1
                Return "Venta"
            Case 2
                Return "Compra"
            Case 3
                Return "Movimiento"
        End Select
        Return ""
    End Function

    Private Function _fnNombreOperacion(operacion As Integer) As String
        Return If(operacion = 2, "Eliminado", "Modificado")
    End Function

    'APP_NAME() de la conexion: la web se conecta como MiventSoftWeb
    Private Function _fnOrigen(aplicacion As Object) As String
        Return If(_fnTexto(aplicacion).ToUpper.Contains("MIVENTSOFTWEB"), "Web", "Escritorio")
    End Function

    Private Function _fnLineaQuien(f As DataRow) As String
        Dim texto As String = "Usuario: " + If(_fnTexto(f.Item("Usuario")) = String.Empty, "-", _fnTexto(f.Item("Usuario"))) +
                              "     Fecha: " + _fnFecha(f.Item("Fecha"), "dd/MM/yyyy HH:mm:ss") +
                              "     Desde: " + _fnOrigen(f.Item("Aplicacion"))
        Return texto
    End Function

    Private Function _fnLineaEquipo(f As DataRow) As String
        Dim ipLocal As String = ""
        Dim ipPublica As String = ""
        _prSepararIp(_fnTexto(f.Item("Ip")), ipLocal, ipPublica)
        Return "Equipo: " + If(_fnTexto(f.Item("Equipo")) = String.Empty, "(no registrado)", _fnTexto(f.Item("Equipo"))) +
               "     IP local: " + If(ipLocal = String.Empty, "-", ipLocal) +
               "     IP publica: " + If(ipPublica = String.Empty, "-", ipPublica)
    End Function

    Private Function _fnLineaAplicacion(f As DataRow) As String
        Dim texto As String
        If (_fnTexto(f.Item("Navegador")) <> String.Empty) Then
            texto = "Navegador: " + _fnTexto(f.Item("Navegador"))
        Else
            texto = "Aplicacion: " + If(_fnTexto(f.Item("Aplicacion")) = String.Empty, "-", _fnTexto(f.Item("Aplicacion")))
        End If
        If (_fnFecha(f.Item("FechaDocumento"), "dd/MM/yyyy") <> String.Empty) Then
            texto = texto + "     Fecha del documento: " + _fnFecha(f.Item("FechaDocumento"), "dd/MM/yyyy")
        End If
        Return texto
    End Function

    'Ip guardada como "local | publica" (o una sola)
    Private Sub _prSepararIp(valor As String, ByRef ipLocal As String, ByRef ipPublica As String)
        Dim partes() As String = valor.Split("|"c)
        Dim a As String = partes(0).Trim
        Dim b As String = If(partes.Length > 1, partes(partes.Length - 1), "").ToString.Trim
        If (b <> String.Empty) Then
            ipLocal = a
            ipPublica = b
        ElseIf (_fnEsIpPrivada(a)) Then
            ipLocal = a
        Else
            ipPublica = a
        End If
    End Sub

    Private Function _fnEsIpPrivada(ip As String) As Boolean
        Return Regex.IsMatch(ip, "^(10\.|192\.168\.|172\.(1[6-9]|2\d|3[01])\.|127\.)")
    End Function

    Private Function _fnHayUbicacion(f As DataRow) As Boolean
        Return Not IsDBNull(f.Item("Latitud")) AndAlso Not IsDBNull(f.Item("Longitud"))
    End Function

    'Coordenada con todos los decimales guardados (script 12: LatitudTexto / LongitudTexto)
    Private Function _fnCoordenada(f As DataRow, columnaTexto As String, columnaNumero As String) As String
        Dim t As String = ""
        If (f.Table.Columns.Contains(columnaTexto)) Then
            t = _fnTexto(f.Item(columnaTexto))
        End If
        If (t = String.Empty) Then
            t = Convert.ToDecimal(f.Item(columnaNumero)).ToString(CultureInfo.InvariantCulture)
        End If
        If (t.Contains(".")) Then
            t = t.TrimEnd("0"c).TrimEnd("."c)
        End If
        Return t
    End Function

    Private Function _fnTextoUbicacion(f As DataRow) As String
        If (Not _fnHayUbicacion(f)) Then
            Return ""
        End If
        Dim texto As String = _fnCoordenada(f, "LatitudTexto", "Latitud") + ", " + _fnCoordenada(f, "LongitudTexto", "Longitud")
        If (Not IsDBNull(f.Item("PrecisionMetros"))) Then
            Dim precision As Decimal = Convert.ToDecimal(f.Item("PrecisionMetros"))
            If (precision >= PRECISION_IP) Then
                texto = texto + "  (aproximada por IP, puede errar por kilometros)"
            Else
                texto = texto + "  (GPS +/- " + Math.Round(precision).ToString + " m)"
            End If
        End If
        Return texto
    End Function

    Private Function _fnUrlMapa(f As DataRow) As String
        Dim lat As String = _fnCoordenada(f, "LatitudTexto", "Latitud")
        Dim lng As String = _fnCoordenada(f, "LongitudTexto", "Longitud")
        Dim zoom As String = "18"
        If (Not IsDBNull(f.Item("PrecisionMetros")) AndAlso Convert.ToDecimal(f.Item("PrecisionMetros")) >= PRECISION_IP) Then
            zoom = "12"
        End If
        Return "https://www.openstreetmap.org/?mlat=" + lat + "&mlon=" + lng + "#map=" + zoom + "/" + lat + "/" + lng
    End Function
#End Region

#Region "Imprimir (HTML que se abre en el navegador: desde ahi se imprime o se guarda como PDF)"
    Private Sub btnImprimirRegistro_Click(sender As Object, e As EventArgs) Handles btnImprimirRegistro.Click
        If (RegistroActual Is Nothing) Then
            _prMensaje("Seleccione un registro de la lista", False)
            Return
        End If
        Try
            Dim f As DataRow = RegistroActual.Fila
            Dim titulo As String = "AUDITORIA - " + _fnNombreModulo(_fnEntero(f.Item("Modulo"))).ToUpper + " NRO. " + _fnEntero(f.Item("DocumentoId")).ToString
            Dim cuerpo As New System.Text.StringBuilder
            _prHtmlRegistro(cuerpo, RegistroActual)
            _prAbrirHtml(_fnHtmlPagina(titulo, "Registro de auditoria nro. " + _fnEntero(f.Item("Id")).ToString, cuerpo.ToString))
        Catch ex As Exception
            _prMensaje("Error al generar la impresion: " + ex.Message, False)
        End Try
    End Sub

    'Reporte: los registros que quedaron en la lista (con los filtros de la grilla) con todo su detalle
    Private Sub btnReporte_Click(sender As Object, e As EventArgs) Handles btnReporte.Click
        Dim ids As New List(Of Integer)
        For Each fila As GridEXRow In grAuditoria.GetRows()
            If (fila.RowType = RowType.Record) Then
                ids.Add(_fnEntero(fila.Cells("Id").Value))
            End If
        Next
        If (ids.Count = 0) Then
            _prMensaje("No hay registros para el reporte con los filtros actuales", False)
            Return
        End If
        If (ids.Count > REPORTE_MAX) Then
            _prMensaje("El reporte tiene " + ids.Count.ToString + " registros; el maximo es " + REPORTE_MAX.ToString + ". Acorte el rango de fechas o use los filtros", False)
            Return
        End If

        Try
            Me.Cursor = Cursors.WaitCursor
            Dim cuerpo As New System.Text.StringBuilder
            Dim incluidos As Integer = 0
            For Each id As Integer In ids
                Dim dt As DataTable = L_prAuditoriaDetalle(id)
                If (dt.Rows.Count > 0 AndAlso dt.Columns.Contains("Antes")) Then
                    _prHtmlRegistro(cuerpo, _fnArmarRegistro(dt.Rows(0)))
                    incluidos = incluidos + 1
                End If
            Next
            Dim subtitulo As String = "Del " + cbFechaDesde.Value.ToString("dd/MM/yyyy") + " al " + cbFechaHasta.Value.ToString("dd/MM/yyyy") +
                                      " &middot; Modulo: " + _fnHtml(cbModulo.Text) + " &middot; Operacion: " + _fnHtml(cbOperacion.Text) +
                                      " &middot; Usuario: " + _fnHtml(cbUsuario.Text) + " &middot; " + incluidos.ToString + " registro(s)"
            _prAbrirHtml(_fnHtmlPagina("REPORTE DE AUDITORIA DE REGISTROS", subtitulo, cuerpo.ToString))
        Catch ex As Exception
            _prMensaje("Error al generar el reporte: " + ex.Message, False)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub _prHtmlRegistro(sb As System.Text.StringBuilder, r As RegistroAuditoria)
        Dim f As DataRow = r.Fila
        sb.AppendLine("<div class='reg'>")
        sb.AppendLine("<div class='titreg " + If(r.Eliminado, "eli", "mod") + "'>" +
                      _fnHtml(_fnNombreModulo(_fnEntero(f.Item("Modulo"))).ToUpper + " NRO. " + _fnEntero(f.Item("DocumentoId")).ToString +
                      " - " + _fnNombreOperacion(_fnEntero(f.Item("Operacion"))).ToUpper) +
                      If(_fnTexto(f.Item("Descripcion")) <> String.Empty, " <span>(" + _fnHtml(f.Item("Descripcion")) + ")</span>", "") + "</div>")
        sb.AppendLine("<div class='dato'>" + _fnHtmlLinea(_fnLineaQuien(f)) + "</div>")
        sb.AppendLine("<div class='dato'>" + _fnHtmlLinea(_fnLineaEquipo(f)) + "</div>")
        sb.AppendLine("<div class='dato'>" + _fnHtmlLinea(_fnLineaAplicacion(f)) + "</div>")
        Dim ub As String = _fnTextoUbicacion(f)
        sb.AppendLine("<div class='dato'>Ubicacion: " + If(ub = String.Empty, "no registrada", _fnHtml(ub)) + "</div>")

        'Cabecera
        sb.AppendLine("<h3>" + If(r.Eliminado, "Cabecera (como estaba al eliminar)", "Cabecera") + "</h3>")
        sb.AppendLine("<table class='cabe'><thead><tr><th>Campo</th><th>" + If(r.Eliminado, "Valor al eliminar", "Antes") + "</th>" +
                      If(r.Eliminado, "", "<th>Despues</th>") + "</tr></thead><tbody>")
        For Each c As DataRow In r.Cabecera.Rows
            Dim cambio As Boolean = (_fnEntero(c.Item("Cambio")) = 1)
            sb.Append("<tr" + If(cambio, " class='cam'", "") + "><td>" + _fnHtml(c.Item("Campo")) + "</td><td>" + _fnHtml(c.Item("Antes")) + "</td>")
            If (Not r.Eliminado) Then
                sb.Append("<td>" + _fnHtml(c.Item("Despues")) + "</td>")
            End If
            sb.AppendLine("</tr>")
        Next
        sb.AppendLine("</tbody></table>")

        'Detalle
        If (r.Eliminado) Then
            sb.AppendLine("<h3>Detalle al eliminar (" + r.Detalle.Rows.Count.ToString + " lineas)</h3>")
        Else
            sb.AppendLine("<h3>Detalle: " + r.Modificados.ToString + " modificada(s), " + r.Agregados.ToString + " agregada(s), " + r.Quitados.ToString + " quitada(s)</h3>")
        End If
        sb.Append("<table><thead><tr><th>Estado</th><th>Producto</th>")
        For Each c As CampoFoto In r.CamposDetalle
            If (c.Campo = "Producto") Then
                Continue For
            End If
            If (r.Eliminado) Then
                sb.Append("<th class='n'>" + _fnHtml(c.Titulo) + "</th>")
            Else
                sb.Append("<th class='n'>" + _fnHtml(c.Titulo) + " antes</th><th class='n'>" + _fnHtml(c.Titulo) + " ahora</th>")
            End If
        Next
        sb.AppendLine("</tr></thead><tbody>")
        For Each d As DataRow In r.Detalle.Rows
            Dim estado As String = _fnTexto(d.Item("Estado"))
            sb.Append("<tr class='" + estado.ToLower + "'><td>" + _fnHtml(estado) + "</td><td>" + _fnHtml(d.Item("Producto")) + "</td>")
            For Each c As CampoFoto In r.CamposDetalle
                If (c.Campo = "Producto") Then
                    Continue For
                End If
                sb.Append("<td class='n'>" + _fnHtml(d.Item(c.Campo + "_A")) + "</td>")
                If (Not r.Eliminado) Then
                    Dim cambio As Boolean = (_fnEntero(d.Item(c.Campo + "_C")) = 1)
                    sb.Append("<td class='n" + If(cambio, " cam", "") + "'>" + _fnHtml(d.Item(c.Campo + "_D")) + "</td>")
                End If
            Next
            sb.AppendLine("</tr>")
        Next
        If (r.Detalle.Rows.Count = 0) Then
            sb.AppendLine("<tr><td colspan='20'>Sin lineas</td></tr>")
        End If
        sb.AppendLine("</tbody></table>")
        sb.AppendLine("</div>")
    End Sub

    Private Function _fnHtmlPagina(titulo As String, subtitulo As String, cuerpo As String) As String
        Dim sb As New System.Text.StringBuilder
        sb.AppendLine("<!DOCTYPE html>")
        sb.AppendLine("<html lang='es'><head><meta charset='utf-8'>")
        sb.AppendLine("<title>" + _fnHtml(titulo) + "</title>")
        sb.AppendLine("<style>")
        sb.AppendLine("body{font-family:Calibri,Arial,sans-serif;margin:24px;color:#222}")
        sb.AppendLine(".cab{display:flex;align-items:center;gap:16px;border-bottom:2px solid #2874AA;padding-bottom:8px}")
        sb.AppendLine(".cab img{max-height:70px}")
        sb.AppendLine("h1{margin:0;font-size:22px;color:#2874AA}")
        sb.AppendLine(".info{font-size:12px;color:#555}")
        sb.AppendLine(".barra{margin:12px 0}")
        sb.AppendLine(".barra button{padding:6px 14px;font-size:14px;background:#1AB394;color:#fff;border:0;cursor:pointer}")
        sb.AppendLine(".reg{border:1px solid #cfd8e3;border-radius:4px;padding:10px 12px;margin:14px 0;page-break-inside:avoid}")
        sb.AppendLine(".titreg{font-size:16px;font-weight:bold;margin-bottom:4px}.titreg span{font-weight:normal;color:#555;font-size:13px}")
        sb.AppendLine(".titreg.eli{color:#C00000}.titreg.mod{color:#C86E00}")
        sb.AppendLine(".dato{font-size:12px;color:#333;margin:1px 0}")
        sb.AppendLine("h3{font-size:13px;color:#1AB394;margin:10px 0 4px}")
        sb.AppendLine("table{width:100%;border-collapse:collapse;font-size:12px}")
        sb.AppendLine("th{background:#2874AA;color:#fff;text-align:left;padding:4px 5px}")
        sb.AppendLine("td{padding:3px 5px;border-bottom:1px solid #ddd}")
        sb.AppendLine(".n{text-align:right}table.cabe td:first-child{width:22%}")
        sb.AppendLine("tr.agregado td{background:#DEF5E4}tr.quitado td{background:#FDE2E2}tr.modificado td{background:#FFF8DC}")
        sb.AppendLine("tr.cam td,td.cam{background:" + COLOR_CAMBIO + " !important;font-weight:bold}")
        sb.AppendLine("@media print{.barra{display:none} th,td,tr td{-webkit-print-color-adjust:exact;print-color-adjust:exact}}")
        sb.AppendLine("</style></head><body>")

        sb.AppendLine("<div class='cab'>")
        Dim logo As String = _fnLogoEmpresaBase64()
        If (logo <> String.Empty) Then
            sb.AppendLine("<img src='data:image/png;base64," + logo + "'>")
        End If
        sb.AppendLine("<div><h1>" + _fnHtml(titulo) + "</h1>")
        sb.AppendLine("<div class='info'>" + subtitulo + "</div>")
        sb.AppendLine("<div class='info'>Generado: " + Now.ToString("dd/MM/yyyy HH:mm") + " &middot; Usuario: " + _fnHtml(gs_user) + "</div>")
        sb.AppendLine("</div></div>")
        sb.AppendLine("<div class='barra'><button onclick='window.print()'>Imprimir / Guardar PDF</button></div>")
        sb.AppendLine(cuerpo)
        sb.AppendLine("</body></html>")
        Return sb.ToString
    End Function

    'Tiene datos de usuarios, IP y ubicacion: se borran los anteriores de la carpeta temporal
    Private Sub _prAbrirHtml(html As String)
        Try
            For Each archivo As String In Directory.GetFiles(Path.GetTempPath(), "AuditoriaRegistros_*.html")
                Try
                    File.Delete(archivo)
                Catch ex As Exception
                    'puede estar abierto en el navegador, se ignora
                End Try
            Next
        Catch ex As Exception
        End Try
        Dim ruta As String = Path.Combine(Path.GetTempPath(), "AuditoriaRegistros_" + Now.ToString("yyyyMMdd_HHmmss") + ".html")
        File.WriteAllText(ruta, html, System.Text.Encoding.UTF8)
        Process.Start(ruta)
    End Sub

    'Logo de la empresa en base64 para incrustarlo en el HTML (igual que el reporte de clientes)
    Private Function _fnLogoEmpresaBase64() As String
        Try
            Dim dtImage As DataTable = ObtenerImagenEmpresa()
            If (dtImage.Rows.Count > 0) Then
                Dim Name As String = dtImage.Rows(0).Item(0).ToString
                Dim ruta As String = gs_CarpetaRaiz + "\Imagenes\Imagenes Empresa\" + Name.TrimStart("\"c)
                If (File.Exists(ruta)) Then
                    Using Bin As New MemoryStream
                        Using im As New Bitmap(ruta)
                            im.Save(Bin, Imaging.ImageFormat.Png)
                        End Using
                        Return Convert.ToBase64String(Bin.ToArray())
                    End Using
                End If
            End If
        Catch ex As Exception
            'sin logo no pasa nada, el reporte se genera igual
        End Try
        Return ""
    End Function

    'Linea de datos: los espacios que separan cada dato se muestran con un punto medio
    Private Function _fnHtmlLinea(texto As String) As String
        Return _fnHtml(texto).Replace("     ", " &nbsp;&middot;&nbsp; ")
    End Function

    Private Function _fnHtml(valor As Object) As String
        If (IsDBNull(valor) Or IsNothing(valor)) Then
            Return ""
        End If
        Return System.Net.WebUtility.HtmlEncode(valor.ToString)
    End Function
#End Region

#Region "Utilitarios"
    Private Sub _prColumna(grilla As GridEX, campo As String, titulo As String, ancho As Integer, Optional formato As String = "", Optional derecha As Boolean = False)
        'el titulo de la columna se ve completo (el ancho minimo es el del texto)
        ancho = Math.Max(ancho, TextRenderer.MeasureText(titulo, grilla.Font).Width + 26)
        With grilla.RootTable.Columns(campo)
            .Caption = titulo
            .Width = ancho
            .Visible = True
            If (formato <> String.Empty) Then
                .FormatString = formato
            End If
            If (derecha) Then
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            End If
        End With
    End Sub

    Private Sub _prGrillaSoloLectura(grilla As GridEX)
        With grilla
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
            .AllowEdit = InheritableBoolean.False
            .FilterMode = FilterMode.None
            .RecordNavigator = False
            .RowHeaders = InheritableBoolean.False
        End With
    End Sub

    Private Sub _prMensaje(texto As String, exito As Boolean)
        Dim img As Bitmap = New Bitmap(If(exito, My.Resources.OK, My.Resources.mensaje), 50, 50)
        ToastNotification.Show(Me, texto.ToUpper, img, 5000, If(exito, eToastGlowColor.Green, eToastGlowColor.Red), eToastPosition.TopCenter)
    End Sub

    Private Function _fnValorEntero(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo) As Integer
        If (mCombo.SelectedIndex < 0 OrElse IsNothing(mCombo.Value) OrElse IsDBNull(mCombo.Value)) Then
            Return -1
        End If
        Return _fnEntero(mCombo.Value)
    End Function

    Private Function _fnEntero(valor As Object) As Integer
        If (IsDBNull(valor) OrElse IsNothing(valor)) Then
            Return 0
        End If
        Dim n As Integer
        If (Integer.TryParse(valor.ToString, n)) Then
            Return n
        End If
        Return 0
    End Function

    Private Function _fnTexto(valor As Object) As String
        If (IsDBNull(valor) OrElse IsNothing(valor)) Then
            Return ""
        End If
        Return valor.ToString.Trim
    End Function

    Private Function _fnFecha(valor As Object, formato As String) As String
        If (IsDBNull(valor) OrElse IsNothing(valor)) Then
            Return ""
        End If
        Return Convert.ToDateTime(valor).ToString(formato)
    End Function
#End Region

End Class
