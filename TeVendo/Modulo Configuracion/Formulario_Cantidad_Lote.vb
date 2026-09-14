Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class Formulario_Cantidad_Lote
    Public respuesta As Boolean = False
    Public NombreProducto As String = ""
    Public CantidadTotal As Double = 0
    Public CantidadVenta As Double = 0
    Public Lote As String = ""
    Public Fecha As Date

    ''Cantidad Unidad / Cantidad Caja (Unidad Maxima), misma idea que FormularioCantidadProductos.
    Public Conversion As Double = 1 ''cuantas unidades minimas entran en 1 unidad maxima
    Public UnidadMinNombre As String = "UNIDAD"
    Public UnidadMaxNombre As String = "CAJA"
    Private _sincronizando As Boolean = False
#Region "Button Si"
    Private Sub Panel1_MouseHover(sender As Object, e As EventArgs) Handles btnSi.MouseHover
        btnSi.BackColor = Color.FromArgb(30, 199, 165)
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
        ValidarStock()

    End Sub
    Private Sub Panel1_MouseLeave(sender As Object, e As EventArgs) Handles btnSi.MouseLeave
        btnSi.BackColor = Color.FromArgb(26, 179, 148)
    End Sub
    Private Sub Formulario_Eliminar_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtProducto.Text = NombreProducto
        txtStock.Text = "Cantidad Disponible = " + Str(CantidadTotal)
        tbLote.Text = "20200101"
        cbFecha.Value = "01/01/2020"

        ''Cantidad Unidad / Cantidad Caja en un solo paso, junto con Lote y Fecha de
        ''Vencimiento (misma idea que FormularioCantidadProductos, pero todo en esta pantalla).
        Label1.Text = UnidadMinNombre.ToUpper & ":"
        LabelCaja.Text = UnidadMaxNombre.ToUpper & ":"
        _prCalcularCaja()

        _habilitarFocus()
    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCantidad, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbCantidadCaja, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbLote, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbFecha, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnNo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSi, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        End With
    End Sub

    ''Cantidad Unidad manda: la Cantidad Caja es solo referencial (se recalcula sola), misma
    ''logica que FormularioCantidadProductos._prCalcularCaja/_prCalcularUnidad.
    Private Sub _prCalcularCaja()
        If (_sincronizando) Then
            Return
        End If
        _sincronizando = True
        If (IsNumeric(tbCantidad.Text) And Conversion > 0) Then
            tbCantidadCaja.Text = Format(Double.Parse(tbCantidad.Text) / Conversion, "0.##")
        Else
            tbCantidadCaja.Text = "0"
        End If
        _sincronizando = False
    End Sub

    Private Sub _prCalcularUnidad()
        If (_sincronizando) Then
            Return
        End If
        _sincronizando = True
        If (IsNumeric(tbCantidadCaja.Text)) Then
            tbCantidad.Text = Format(Double.Parse(tbCantidadCaja.Text) * Conversion, "0.##")
        Else
            tbCantidad.Text = "0"
        End If
        _sincronizando = False
    End Sub

    Private Sub tbCantidad_TextChanged(sender As Object, e As EventArgs) Handles tbCantidad.TextChanged
        _prCalcularCaja()
    End Sub

    Private Sub tbCantidadCaja_TextChanged(sender As Object, e As EventArgs) Handles tbCantidadCaja.TextChanged
        _prCalcularUnidad()
    End Sub
    Private Sub btnNo_MouseHover(sender As Object, e As EventArgs) Handles btnNo.MouseHover
        btnNo.BackColor = Color.FromArgb(170, 170, 170)
    End Sub

    Private Sub btnNo_MouseLeave(sender As Object, e As EventArgs) Handles btnNo.MouseLeave
        btnNo.BackColor = Color.FromArgb(191, 191, 191)
    End Sub



    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        respuesta = False
        Me.Close()
    End Sub

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        respuesta = False
        Me.Close()
    End Sub

    Private Sub btnSi_MouseClick(sender As Object, e As MouseEventArgs) Handles btnSi.MouseClick


        ValidarStock()
    End Sub

    Public Sub ValidarStock()

        If (IsNumeric(tbCantidad.Text)) Then
            Dim CantidadActual As Double = Double.Parse(tbCantidad.Text)
            If (CantidadActual > 0) Then
                If (tbLote.Text.ToString.Length > 0) Then

                    respuesta = True
                    CantidadVenta = CantidadActual
                    Lote = tbLote.Text
                    Fecha = cbFecha.Value
                    Me.Close()
                Else
                    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                    ToastNotification.Show(Me, "Ingrese Datos Validos", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                    tbLote.Focus()
                End If
            Else
                tbCantidad.Clear()
                tbCantidad.Text = "0".Trim
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "La Cantidad debe ser Mayor o igual a 1", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbCantidad.Focus()
            End If
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Ingrese Datos Validos", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End If




    End Sub

    Private Sub tbCantidad_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCantidad.KeyDown
        If (e.KeyData = Keys.Enter) Then
            tbCantidadCaja.Focus()
        End If
    End Sub

    Private Sub tbCantidadCaja_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCantidadCaja.KeyDown
        If (e.KeyData = Keys.Enter) Then
            tbLote.Focus()
        End If
    End Sub

    Private Sub FormularioCantidadProductos_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyData = Keys.Escape) Then
            respuesta = False
            Me.Close()

        End If
    End Sub

    Private Sub tbLote_KeyDown(sender As Object, e As KeyEventArgs) Handles tbLote.KeyDown
        If (e.KeyData = Keys.Enter) Then
            cbFecha.Focus()
        End If
    End Sub

    Private Sub cbFecha_KeyDown(sender As Object, e As KeyEventArgs) Handles cbFecha.KeyDown
        If (e.KeyData = Keys.Enter) Then
            ValidarStock()
        End If
        If (e.KeyData = Keys.Tab) Then
            btnSi.Focus()
        End If
    End Sub

    Private Sub btnSi_Paint(sender As Object, e As PaintEventArgs) Handles btnSi.Paint

        'ValidarStock()

    End Sub

    Private Sub btnNo_Paint(sender As Object, e As PaintEventArgs) Handles btnNo.Paint
        'respuesta = False
        'Me.Close()
    End Sub
#End Region
End Class