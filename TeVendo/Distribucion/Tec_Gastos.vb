Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO

Public Class Tec_Gastos

    Public ConceptoId As Integer
    Public Descripcion As String = ""
    Public Monto As Double
    Public Bandera As Boolean = False
    Public NombreConcepto As String
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)


    Private Sub Tec_Gastos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        P_Global._prCargarComboGenerico(cbConcepto, L_prLibreriaDetalleGeneral(14), "cnnum", "Codigo", "cndesc1", "Concepto Gasto")

        _habilitarFocus()
        cbConcepto.Focus()
    End Sub
    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(cbConcepto, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbdescripcion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbMonto, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

            .SetHighlightOnFocus(btnGuardar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
        End With
    End Sub
    Private Sub btnTipoContrato_Click(sender As Object, e As EventArgs) Handles btnTipoContrato.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 14
        ef.titulo = "Crear Nuevo Concepto De Gasto"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbConcepto, L_prLibreriaDetalleGeneral(14), "cnnum", "Codigo", "cndesc1", "Concepto Gasto")
            cbConcepto.SelectedIndex = CType(cbConcepto.DataSource, DataTable).Rows.Count - 1
            cbConcepto.Focus()
        End If
        ef.Dispose()

    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click

        Bandera = False
        Me.Close()

    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click

        If (tbMonto.Text.Trim = String.Empty) Then
            ToastNotification.Show(Me, "Debe colocar Un Monto Valido".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbMonto.Focus()
            Return

        End If

        If (tbMonto.Value <= 0) Then
            ToastNotification.Show(Me, "Debe colocar Un Monto Valido".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbMonto.Focus()
            Return

        End If

        If (cbConcepto.SelectedIndex < 0) Then
            ToastNotification.Show(Me, "Debe Seleccionar un Concepto Valido".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbConcepto.Focus()
            Return
        End If

        ConceptoId = cbConcepto.Value
        Descripcion = tbdescripcion.Text
        Monto = tbMonto.Value
        NombreConcepto = cbConcepto.Text
        Bandera = True

        Me.Close()


    End Sub
End Class