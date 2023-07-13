Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Public Class CrearProveedor
    Public ProveedorId As Integer = -1
    Public Bandera As Boolean = False
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Bandera = False
        Me.Close()
    End Sub

    Public Sub _habilitarFocus()
        With Highlighter2
            .SetHighlightOnFocus(tbNombreProveedor, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDireccion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbTelefono01, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbTelefono02, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNroDocumento, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbTipoDocumento, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNombreProveedor, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGuardar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub
    Private Sub CrearLibreria_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        P_Global._prCargarComboGenerico(cbTipoDocumento, L_prLibreriaDetalleGeneral(8), "cnnum", "Codigo", "cndesc1", "TipoDocumento")
        tbNombreProveedor.Clear()
        tbNombreProveedor.Focus()
        _habilitarFocus()
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        If (tbNombreProveedor.Text.Length > 1) Then
            Dim id As String = ""
            Bandera = InsertarProveedor(id, tbNombreProveedor.Text, tbDireccion.Text, tbTelefono01.Text, tbTelefono02.Text,
                                    "", cbTipoDocumento.Value, tbNroDocumento.Text, 1)
            If (Bandera = True) Then

                ProveedorId = CInt(id.Trim)
                Me.Close()
            Else
                ToastNotification.Show(Me, "Error Al guardar Proveedor", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbNombreProveedor.Focus()
            End If


        Else
            ToastNotification.Show(Me, "Debe Ingresar un Nombre Valido del Proveedor", img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbNombreProveedor.Focus()
            Return


        End If
    End Sub


End Class