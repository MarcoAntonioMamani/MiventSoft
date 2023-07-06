
Imports System.ComponentModel
Imports System.Text
Imports DevComponents.DotNetBar
Public Class Efecto
    Public _archivo As String
    Public band As Boolean = False
    Public Header As String = ""
    Public tipo As Integer = 0
    Public Context As String = ""
    Public listEstCeldas As List(Of Celda)
    Public listEstCeldasNew As List(Of Celda)
    Public dt As DataTable
    Public alto As Integer
    Public ancho As Integer
    Public Row As Janus.Windows.GridEX.GridEXRow
    Public SeleclCol As Integer = -1
    Public titulo As String
    Public descripcion As String
    Public NombreProducto As String
    Public StockActual As Double
    Public CantidadTransaccion As Double
    Public TipoMovimiento As Integer
    Public NewCliente As Boolean = False
    Public IdCliente As Integer
    Public NombreCliente As String
    Public Lote As String
    Public Lotebool As Boolean
    Public FechaVencimiento As Date
    Public ModuloLibreria As Integer = 0
    Public PlanillaId As Integer = 0
    Public SueldoNeto As Double = 0
    Public dtGeneral As DataTable
    Public Fila As Integer


    Public dtDetalle As DataTable
    Public TipoMovimientoId As Integer
    Public DepositoId As Integer
    Public SucursalId As Integer


    Public CategoriaId As Integer
    Public ProveedorId As Integer


    Public MontoBs As Double = 0
    Public MontoDolares As Double = 0
    Public MontoTarjeta As Double = 0
    Public MontoTransferencia As Double = 0
    Public TotalVenta As Double = 0

    Public TipoPrograma As Integer = 1
    Public Id As Integer = 0


    Public SucursalSelected As Integer = 0
    Public CategoriaPrecioSelected As Integer = 0
    Public TableCategoria As DataTable

    Public TipGrabado As Integer = 0

    Private Sub Efecto_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized

        Select Case tipo
            Case 1
                _prMostrarMensaje()
            Case 2
                _prMostrarMensajeDelete()
            Case 3
                _prMostrarFormAyuda()
            Case 4
                _prLogin()
            Case 5
                _prMostrarFormularioCantidad()
            Case 6
                MostrarFormularioContenido()
            Case 7
                MostrarFormularioClientesContenido()
            Case 8
                ReporteVenta()
            Case 9
                _prMostrarFormularioCantidadLote()

            Case 10
                _prMostrarFormLibreria()
            Case 11
                _prMostrarConceptoFijosPlanilla()
            Case 12
                _prMostrarConceptoVariablesPlanilla()
            Case 13
                _prMostrarAyudaProductosDespachos()
            Case 14
                _prMostrarAyudaProductosMovimientos()
            Case 15
                _prMostrarAyudaProductosCompras()

            Case 16
                _prMostrarAyudaProductosVentas()

            Case 18
                _prMostrarFormCrearCategoria()
            Case 19
                _prMostrarFormCrearProveedor()
            Case 20
                _prMostrarFormCobranza()
            Case 21
                _prMostrarFormCrearTipoMovimiento()
            Case 22
                MostrarFormularioAyudaProductos()
            Case 23
                MostrarFormularioAyudaCatalogo()
        End Select
    End Sub

    Sub _prMostrarFormCrearCategoria()

        Dim frmAyuda As CrearCategoria
        frmAyuda = New CrearCategoria
        frmAyuda.ShowDialog()
        If frmAyuda.Bandera = True Then
            CategoriaId = frmAyuda.IdCategoria
            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If

    End Sub

    Sub _prMostrarFormCrearTipoMovimiento()

        Dim frmAyuda As Crear_TipoMovimientoCaja
        frmAyuda = New Crear_TipoMovimientoCaja
        frmAyuda.ShowDialog()
        If frmAyuda.Bandera = True Then
            Id = frmAyuda.IdTipoMovimiento
            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If

    End Sub

    Sub _prMostrarFormCrearProveedor()

        Dim frmAyuda As CrearProveedor
        frmAyuda = New CrearProveedor
        frmAyuda.ShowDialog()
        If frmAyuda.Bandera = True Then
            ProveedorId = frmAyuda.ProveedorId
            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If

    End Sub

    Sub _prMostrarFormCobranza()

        Dim frmAyuda As EfectivoCobranza
        frmAyuda = New EfectivoCobranza
        frmAyuda.MontoBs = MontoBs
        frmAyuda.MontoDolares = MontoDolares
        frmAyuda.MontoTarjeta = MontoTarjeta
        frmAyuda.MontoTransferencia = MontoTransferencia
        frmAyuda.TotalVenta = TotalVenta
        frmAyuda.ShowDialog()
        If frmAyuda.Bandera = True Then

            MontoBs = frmAyuda.MontoBs
            MontoDolares = frmAyuda.MontoDolares
            MontoTarjeta = frmAyuda.MontoTarjeta
            MontoTransferencia = frmAyuda.MontoTransferencia
            TipGrabado = frmAyuda.TipoGrabado

            band = True
            frmAyuda.Dispose()

            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If

    End Sub
    Public Sub _prLogin()
        Dim Frm As New Tec_Login
        Frm.ShowDialog()
        Frm.Dispose()
        Me.Close()
    End Sub
    Sub _prMostrarFormAyuda()

        'Dim frmAyuda As Modelo.ModeloAyuda
        'frmAyuda = New Modelo.ModeloAyuda(alto, ancho, dt, Context.ToUpper, listEstCeldas)
        'If (SeleclCol >= 0) Then
        '    frmAyuda.Columna = SeleclCol
        '    frmAyuda._prSeleccionar()

        'End If
        'frmAyuda.ShowDialog()
        'If frmAyuda.seleccionado = True Then
        '    Row = frmAyuda.filaSelect
        '    band = True
        '    Me.Close()
        'Else
        '    band = False
        '    Me.Close()
        'End If
        Dim frmAyuda As Formulario_Eliminar
        frmAyuda = New Formulario_Eliminar
        frmAyuda.Titulo = titulo
        frmAyuda.Descripcion = descripcion
        frmAyuda.ShowDialog()
        If frmAyuda.respuesta = True Then

            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If

    End Sub

    Sub _prMostrarFormLibreria()

        Dim frmAyuda As CrearLibreria
        frmAyuda = New CrearLibreria
        frmAyuda.Titulo = titulo
        frmAyuda.Modulo = ModuloLibreria
        frmAyuda.ShowDialog()
        If frmAyuda.Bandera = True Then

            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If

    End Sub

    Sub _prMostrarConceptoFijosPlanilla()

        Dim frmAyuda As Tec_VisualizarConceptoFijosPlanilla
        frmAyuda = New Tec_VisualizarConceptoFijosPlanilla
        frmAyuda.title = titulo
        frmAyuda.PlanillaId = PlanillaId
        frmAyuda.ShowDialog()
        frmAyuda.Dispose()
        Me.Close()


    End Sub

    Sub _prMostrarConceptoVariablesPlanilla()

        Dim frmAyuda As Tec_VisualizarConceptosVariablesPlanilla
        frmAyuda = New Tec_VisualizarConceptosVariablesPlanilla
        frmAyuda.title = titulo
        frmAyuda.PlanillaId = PlanillaId
        frmAyuda.SueldoNeto = SueldoNeto
        frmAyuda.dtGeneral = dtGeneral
        frmAyuda.PosicionGeneral = Fila
        frmAyuda.ShowDialog()
        frmAyuda.Dispose()
        Me.Close()


    End Sub

    Sub _prMostrarAyudaProductosDespachos()

        Dim frmAyuda As Tec_DespachoDetalle
        frmAyuda = New Tec_DespachoDetalle(dtDetalle)

        frmAyuda.ShowDialog()

        frmAyuda.Dispose()
        Me.Close()


    End Sub
    Sub _prMostrarAyudaProductosMovimientos()

        Dim frmAyuda As Tec_MovimientoDetalle
        frmAyuda = New Tec_MovimientoDetalle(dtDetalle)
        frmAyuda.TipoMovimientoId = TipoMovimientoId
        frmAyuda.DepositoId = DepositoId
        frmAyuda.Lote = Lotebool
        frmAyuda.ShowDialog()

        frmAyuda.Dispose 
        Me.Close()


    End Sub

    Sub _prMostrarAyudaProductosCompras()

        Dim frmAyuda As Tec_ComprasDetalle
        frmAyuda = New Tec_ComprasDetalle(dtDetalle)
        frmAyuda.SucursalId = SucursalId
        frmAyuda.Lote = Lotebool
        frmAyuda.ShowDialog()
        frmAyuda.Dispose()

        Me.Close()


    End Sub

    Sub _prMostrarAyudaProductosVentas()

        Dim frmAyuda As Tec_VentasDetalle
        frmAyuda = New Tec_VentasDetalle(dtDetalle)
        frmAyuda.SucursalId = SucursalId
        frmAyuda.Lote = Lotebool
        frmAyuda.IdCliente = IdCliente
        frmAyuda.TipoProgramas = TipoPrograma
        frmAyuda.CategoriaPrecio = CategoriaPrecioSelected
        frmAyuda.ShowDialog()

        CategoriaPrecioSelected = frmAyuda.CategoriaPrecio
        frmAyuda.Dispose()
        Me.Close()


    End Sub
    Public Sub ReporteVenta()
        Dim frmAyuda As Formulario_Reportevb
        frmAyuda = New Formulario_Reportevb
        frmAyuda.Titulo = titulo
        frmAyuda.Descripcion = descripcion
        frmAyuda.ShowDialog()
        If frmAyuda.respuesta = True Then

            band = True
            frmAyuda.Dispose()

            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()

            Me.Close()
        End If
    End Sub

    Sub MostrarFormularioContenido()
        Dim frmAyuda As FormularioAyuda
        frmAyuda = New FormularioAyuda(alto, ancho, dt, Context.ToUpper, listEstCeldasNew)
        If (SeleclCol >= 0) Then
            frmAyuda.Columna = SeleclCol
            frmAyuda._prSeleccionar()

        End If
        frmAyuda.ShowDialog()
        If frmAyuda.seleccionado = True Then
            Row = frmAyuda.filaSelect
            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If
    End Sub

    Sub MostrarFormularioAyudaCatalogo()
        Dim frmAyuda As AyudaMultiCategoria
        frmAyuda = New AyudaMultiCategoria(alto, ancho, dt, Context.ToUpper, listEstCeldasNew)
        If (SeleclCol >= 0) Then
            frmAyuda.Columna = SeleclCol
            frmAyuda._prSeleccionar()

        End If
        frmAyuda.ShowDialog()
        If frmAyuda.seleccionado = True Then
            Row = frmAyuda.filaSelect

            SucursalSelected = frmAyuda.SucursalSelected
            CategoriaPrecioSelected = frmAyuda.CategoriaPrecioSelected
            TableCategoria = frmAyuda.TableCategoria
            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If
    End Sub
    Sub MostrarFormularioAyudaProductos()
        Dim frmAyuda As FormulariiAyudaProductos
        frmAyuda = New FormulariiAyudaProductos(alto, ancho, dt, Context.ToUpper, listEstCeldasNew)
        If (SeleclCol >= 0) Then
            frmAyuda.Columna = SeleclCol
            frmAyuda._prSeleccionar()

        End If
        frmAyuda.ShowDialog()
        If frmAyuda.seleccionado = True Then
            Row = frmAyuda.filaSelect
            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If
    End Sub

    Sub MostrarFormularioClientesContenido()
        Dim frmAyuda As FormularioCliente
        frmAyuda = New FormularioCliente(alto, ancho, dt, Context.ToUpper, listEstCeldasNew)
        If (SeleclCol >= 0) Then
            frmAyuda.Columna = SeleclCol
            frmAyuda._prSeleccionar()

        End If
        frmAyuda.ShowDialog()
        If frmAyuda.seleccionado = True Then
            Row = frmAyuda.filaSelect
            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            NewCliente = frmAyuda.NuevoCliente
            IdCliente = frmAyuda.IdCliente
            NombreCliente = frmAyuda.NombreCliente
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If
    End Sub
    Sub _prMostrarMensaje()
        Dim blah As Bitmap = My.Resources.cuestion
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())

        If (MessageBox.Show(Context, Header, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes) Then
            'Process.Start(_archivo)
            band = True
            Me.Close()
        Else
            band = False
            Me.Close()


        End If
    End Sub
    Sub _prMostrarMensajeDelete()

        Dim info As New TaskDialogInfo(Context, eTaskDialogIcon.Delete, "advertencia".ToUpper, Header, eTaskDialogButton.Yes Or eTaskDialogButton.Cancel, eTaskDialogBackgroundColor.Default)
        Dim result As eTaskDialogResult = TaskDialog.Show(info)
        If result = eTaskDialogResult.Yes Then
            Dim mensajeError As String = ""
            band = True
            Me.Close()

        Else
            band = False

            Me.Close()

        End If
    End Sub

    Sub _prMostrarFormularioCantidad()
        Dim frmAyuda As FormularioCantidadProductos
        frmAyuda = New FormularioCantidadProductos
        frmAyuda.NombreProducto = NombreProducto
        frmAyuda.CantidadTotal = StockActual
        frmAyuda.TipoMovimiento = TipoMovimiento
        frmAyuda.ShowDialog()
        If frmAyuda.respuesta = True Then

            CantidadTransaccion = frmAyuda.CantidadVenta
            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If
    End Sub

    Sub _prMostrarFormularioCantidadLote()
        Dim frmAyuda As Formulario_Cantidad_Lote
        frmAyuda = New Formulario_Cantidad_Lote
        frmAyuda.NombreProducto = NombreProducto
        frmAyuda.CantidadTotal = StockActual
        frmAyuda.ShowDialog()
        If frmAyuda.respuesta = True Then
            Lote = frmAyuda.Lote
            CantidadTransaccion = frmAyuda.CantidadVenta
            FechaVencimiento = frmAyuda.Fecha
            band = True
            frmAyuda.Dispose()
            Me.Close()
        Else
            band = False
            frmAyuda.Dispose()
            Me.Close()
        End If
    End Sub

    Private Sub Efecto_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Me.Dispose()

    End Sub
End Class