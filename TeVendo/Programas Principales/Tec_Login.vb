Imports DevComponents.DotNetBar

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports Negocio.AccesoLogica
Imports Janus.Windows.GridEX
Public Class Tec_Login


#Region "PrivateMethos"

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public Sub _habilitarFocus()
        With Highlighter1
            .SetHighlightOnFocus(tbUsuario, DevComponents.DotNetBar.Validator.eHighlightColor.Red)
            .SetHighlightOnFocus(tbpassword, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

        End With
    End Sub
    Private Sub _prDesvenecerPantalla()
        Dim a, b As Decimal
        For a = 20 To 0 Step -5
            b = a / 100
            Me.Opacity = b
            Me.Refresh()
        Next
    End Sub
    Private Sub _prLeerArchivoConfig()
        Dim Archivo() As String = IO.File.ReadAllLines(Application.StartupPath + "\dbConnect.TXT")
        gs_Ip = Archivo(0).Split("=")(1).Trim
        gs_UsuarioSql = Archivo(1).Split("=")(1).Trim
        gs_ClaveSql = Archivo(2).Split("=")(1).Trim
        gs_NombreBD = Archivo(3).Split("=")(1).Trim
        gs_CarpetaRaiz = Archivo(4).Split("=")(1).Trim
    End Sub
#End Region
    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub Tec_Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _habilitarFocus()
        Me.Text = "L O G I N"
        Me.Opacity = 0.01
        Timer1.Interval = 10
        Timer1.Enabled = True
        tbUsuario.Focus()

        Dim blah As New Bitmap(New Bitmap(My.Resources.user), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
        _prLeerArchivoConfig()

        L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If Me.Opacity < 1 Then
            Me.Opacity += 0.05
        Else
            Timer1.Enabled = False
        End If



    End Sub

    Private Sub btnIngresar_Click(sender As Object, e As EventArgs) Handles btnIngresar.Click

        If tbUsuario.Text = "" Then
            ToastNotification.Show(Me, "Error: Debe ingresar un usuario correcto!!!", P_Global.mensaje, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Exit Sub
        End If
        If tbpassword.Text = "" Then
            ToastNotification.Show(Me, "Error: Debe ingresar una contraseña correctamente..!!!", P_Global.mensaje, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Exit Sub
        End If
        Dim dtUsuario As DataTable = L_Validar_Usuario(tbUsuario.Text.ToUpper(), tbpassword.Text.ToUpper())
        If dtUsuario.Rows.Count = 0 Then
            ToastNotification.Show(Me, "Error: Los datos de Usuario y Contraseña son incorrectas. Por Favor ingrese con credenciales validos !!!", P_Global.mensaje, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbUsuario.SelectAll()
            tbUsuario.Focus()
        Else

            Dim idPersonal As Integer = dtUsuario.Rows(0).Item("idPersonal")
            Global_IdPersonal = idPersonal
            Dim dt As DataTable = ListarPersonalById(idPersonal)
            If (dt.Rows.Count > 0) Then
                L_Usuario = dt.Rows(0).Item("Nombre")
                gs_user = dt.Rows(0).Item("Nombre")
            Else
                L_Usuario = tbUsuario.Text
                gs_user = tbUsuario.Text
            End If

            gi_userFuente = 13
            gi_userNumi = dtUsuario.Rows(0).Item("Id")
            gi_userRol = dtUsuario.Rows(0).Item("RolId")
            gi_userSuc = dtUsuario.Rows(0).Item("SucursalId")
            'gb_userTodasSuc = IIf(dtUsuario.Rows(0).Item("ydall") = 1, True, False)

            _prDesvenecerPantalla()


            Tec_Principal.Show()
            Me.Close()



        End If
    End Sub
    Private Sub Login_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress, tbpassword.KeyPress, tbUsuario.KeyPress
        If (e.KeyChar = ChrW(Keys.Escape)) Then
            _prDesvenecerPantalla()
            Me.Close()
        End If
    End Sub

    Private Sub btnsalir_Click(sender As Object, e As EventArgs) Handles btnsalir.Click

        _prDesvenecerPantalla()
            Me.Close()

    End Sub

    Private Sub tbUsuario_KeyDown(sender As Object, e As KeyEventArgs) Handles tbUsuario.KeyDown
        If (e.KeyData = Keys.Enter) Then
            tbpassword.SelectAll()
            tbpassword.Focus()

        End If
    End Sub

    Private Sub tbpassword_KeyDown(sender As Object, e As KeyEventArgs) Handles tbpassword.KeyDown
        If (e.KeyData = Keys.Enter) Then
            btnIngresar.Focus()

        End If
    End Sub
End Class