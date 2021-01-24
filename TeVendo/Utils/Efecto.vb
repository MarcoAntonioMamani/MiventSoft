
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
    Public FechaVencimiento As Date
    Public ModuloLibreria As Integer = 0
    Public PlanillaId As Integer = 0






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

        End Select
    End Sub
    Public Sub _prLogin()
        Dim Frm As New Tec_Login
        Frm.ShowDialog()
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
            Me.Close()
        Else
            band = False
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
            Me.Close()
        Else
            band = False
            Me.Close()
        End If

    End Sub

    Sub _prMostrarConceptoFijosPlanilla()

        Dim frmAyuda As Tec_VisualizarConceptoFijosPlanilla
        frmAyuda = New Tec_VisualizarConceptoFijosPlanilla
        frmAyuda.title = titulo
        frmAyuda.PlanillaId = PlanillaId
        frmAyuda.ShowDialog()
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
            Me.Close()
        Else
            band = False
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
            Me.Close()
        Else
            band = False
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
            Me.Close()
        Else
            NewCliente = frmAyuda.NuevoCliente
            IdCliente = frmAyuda.IdCliente
            NombreCliente = frmAyuda.NombreCliente
            band = False
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
            Me.Close()
        Else
            band = False
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
            Me.Close()
        Else
            band = False
            Me.Close()
        End If
    End Sub
End Class