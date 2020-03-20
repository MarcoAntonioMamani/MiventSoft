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
Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Public Class Tec_Login


#Region "PrivateMethos"
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
        Dim Archivo() As String = IO.File.ReadAllLines(Application.StartupPath + "\CONFIG.TXT")
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
            ToastNotification.Show(Me, "Error: Debe ingresar un usuario correcto!!!", P_Global.tc_warning, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Exit Sub
        End If
        If tbpassword.Text = "" Then
            ToastNotification.Show(Me, "Error: Debe ingresar una contraseña correctamente..!!!", P_Global.tc_warning, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            Exit Sub
        End If
        Dim dtUsuario As DataTable = L_Validar_Usuario(tbUsuario.Text, tbpassword.Text)
        If dtUsuario.Rows.Count = 0 Then
            ToastNotification.Show(Me, "Error: Los datos de Usuario y Contraseña son incorrectas. Por Favor ingrese con credenciales validos !!!", P_Global.tc_warning, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbUsuario.SelectAll()
            tbUsuario.Focus()
        Else
            gs_user = tbUsuario.Text
            gi_userFuente = dtUsuario.Rows(0).Item("ydfontsize")
            gi_userNumi = dtUsuario.Rows(0).Item("ydnumi")
            gi_userRol = dtUsuario.Rows(0).Item("ydrol")
            gi_userSuc = dtUsuario.Rows(0).Item("ydsuc")
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