Imports Logica.AccesoLogica
Imports Modelo.MGlobal
Imports DevComponents.DotNetBar.Controls
Imports DevComponents.DotNetBar.Metro
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Rendering

Public Class Tec_Principal


#Region "Methods Private"


    Sub _prCargarFecha()
        Dim Fecha As String = ""
        Fecha = "     " + Str(Now.Date.Day) + " " + MonthName(Now.Date.Month) + " " + Str(Now.Date.Year)
        btnFecha.Text = Fecha
        btnFecha.ForeColor = Color.White
    End Sub

    Public Sub New()


        ' This call is required by the designer.
        InitializeComponent()
        SuperTabControlMenu.SelectedTab = tab_configuraciones


        _prCargarFecha()
        _prIniciarTodo()
        ' Add any initialization after the InitializeComponent() call.
        _prLogin()
    End Sub
    Private Sub _prIniciarTodo()
        'Leer Archivo de Configuración
        _prLeerArchivoConfig()
        L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.WindowState = FormWindowState.Maximized
    End Sub


    Private Sub _prLeerArchivoConfig()
        Dim Archivo() As String = IO.File.ReadAllLines(Application.StartupPath + "\CONFIG.TXT")
        gs_Ip = Archivo(0).Split("=")(1).Trim
        gs_UsuarioSql = Archivo(1).Split("=")(1).Trim
        gs_ClaveSql = Archivo(2).Split("=")(1).Trim
        gs_NombreBD = Archivo(3).Split("=")(1).Trim
        gs_CarpetaRaiz = Archivo(4).Split("=")(1).Trim
    End Sub

    Private Sub _prLogin()


        _PCargarPrivilegios()
        _prCargarConfiguracionSistema()
        P_prCargarParametros()
        _prValidarMayusculas()
    End Sub
    Public Sub _prValidarMayusculas()
        Dim dt As DataTable = L_fnPorcUtilidad()
        If (dt.Rows.Count > 0) Then
            Modelo.MGlobal.gs_mayusuculas = dt.Rows(0).Item("mayusculas")
        End If
    End Sub

    Private Sub P_prCargarParametros()
        Dim dtConfSistema As DataTable = L_fnConfSistemaGeneral()

        gb_FacturaEmite = dtConfSistema.Rows(0).Item("cccefac")
        gi_FacturaTipo = dtConfSistema.Rows(0).Item("ccctfac")
        gi_FacturaCantidadItems = dtConfSistema.Rows(0).Item("ccccite")
        gb_FacturaIncluirICE = dtConfSistema.Rows(0).Item("ccciice")
        'gi_codeBar = dtConfSistema.Rows(0).Item("ccciice")

    End Sub

    Private Sub _prCargarConfiguracionSistema()
        'Dim dtConf As DataTable = L_prConGlobalGeneral()
        'gd_notaAproTeo = dtConf.Rows(0).Item("gbaproteo")

    End Sub

    Private Sub _PCargarPrivilegios()
        Dim listaTabs As New List(Of DevComponents.DotNetBar.Metro.MetroTilePanel)
        listaTabs.Add(Panel_Configuracion)
        listaTabs.Add(Panel_Ventas)
        listaTabs.Add(Panel_ingresos)
        listaTabs.Add(Panel_Mapa)
        listaTabs.Add(Panel_Almacen)
        listaTabs.Add(Panel_Reportes)
        Dim idRolUsu As String = gi_userRol
        Dim dtModulos As DataTable = L_prLibreriaDetalleGeneral(gi_LibSistema, gi_LibSISModulo)
        Dim listFormsModulo As New List(Of String)

        For i = 0 To dtModulos.Rows.Count - 1
            Dim dtDetRol As DataTable = L_RolDetalle_General(-1, idRolUsu, dtModulos.Rows(i).Item("cnnum"))
            listFormsModulo = New List(Of String)

            If dtDetRol.Rows.Count > 0 Then
                'cargo los nombres de los programas(botones) del modulo
                For Each fila As DataRow In dtDetRol.Rows
                    listFormsModulo.Add(fila.Item("yaprog").ToString.ToUpper)
                Next
                'recorro el modulo(tab) que corresponde
                For Each _item As DevComponents.DotNetBar.BaseItem In listaTabs.Item(i).Items
                    If TypeOf (_item) Is DevComponents.DotNetBar.Metro.MetroTileItem Then 'es un boton del modulo
                        Dim btn As DevComponents.DotNetBar.Metro.MetroTileItem = CType(_item, DevComponents.DotNetBar.Metro.MetroTileItem)
                        If listFormsModulo.Contains(btn.Name.ToUpper) Then 'si el nombre del boton pertenece a la lista de formularios del modulo
                            Dim Texto As String = btn.Text
                            Dim TTexto As String = btn.TitleText
                            Dim f As Integer = listFormsModulo.IndexOf(btn.Name.ToUpper)
                            If Texto = "" Then 'esta usando el Title Text
                                btn.TitleText = dtDetRol.Rows(f).Item("yatit").ToString.ToUpper
                            Else 'esta usando el Text
                                btn.Text = dtDetRol.Rows(f).Item("yatit").ToString.ToUpper
                            End If

                            If dtDetRol.Rows(f).Item("ycshow") = True Or dtDetRol.Rows(f).Item("ycadd") = True Or dtDetRol.Rows(f).Item("ycmod") = True Or dtDetRol.Rows(f).Item("ycdel") = True Then
                                btn.Visible = True
                            Else
                                btn.Visible = False
                            End If
                        Else 'si no pertenece lo oculto
                            btn.Visible = False
                        End If
                    Else 'seria un sub grupo en el modulo
                        'recorro todos los elementos del sub grupo
                        If TypeOf _item Is ItemContainer Then
                            For Each _subItem In _item.SubItems
                                Dim _subBtn As MetroTileItem = CType(_subItem, MetroTileItem)
                                If listFormsModulo.Contains(_subBtn.Name.ToUpper) Then
                                    Dim Texto As String = _subBtn.Text
                                    Dim TTexto As String = _subBtn.TitleText
                                    Dim f As Integer = listFormsModulo.IndexOf(_subBtn.Name.ToUpper)
                                    'If Texto = "" Then 'esta usando el Title Text
                                    '    _subBtn.TitleText = dtDetRol.Rows(f).Item("yatit").ToString.ToUpper
                                    'Else 'esta usando el Text
                                    '    _subBtn.Text = dtDetRol.Rows(f).Item("yatit").ToString.ToUpper
                                    'End If

                                    If dtDetRol.Rows(f).Item("ycshow") = True Or dtDetRol.Rows(f).Item("ycadd") = True Or dtDetRol.Rows(f).Item("ycmod") = True Or dtDetRol.Rows(f).Item("ycdel") = True Then
                                        _subBtn.Visible = True
                                    Else
                                        _subBtn.Visible = False
                                    End If
                                Else
                                    _subBtn.Visible = False
                                End If
                            Next
                        End If

                    End If
                Next
            Else ' no exiten formulario registrados en el modulo pero igual hay que ocultar los botones y los subbotones que tenga
                For Each _item As DevComponents.DotNetBar.BaseItem In listaTabs.Item(i).Items
                    If TypeOf (_item) Is DevComponents.DotNetBar.Metro.MetroTileItem Then 'es un boton del modulo
                        Dim btn As DevComponents.DotNetBar.Metro.MetroTileItem = CType(_item, DevComponents.DotNetBar.Metro.MetroTileItem)
                        btn.Visible = False
                    Else 'seria un sub grupo en el modulo
                        'recorro todos los elementos del sub grupo
                        If TypeOf _item Is ItemContainer Then
                            For Each _subItem In _item.SubItems
                                Dim _subBtn As MetroTileItem = CType(_subItem, MetroTileItem)
                                _subBtn.Visible = False
                            Next
                        End If

                    End If
                Next

            End If

        Next

        'refrescar el formulario
        Me.Refresh()
    End Sub
#End Region
    Private Sub SuperTabControl1_SelectedTabChanged(sender As Object, e As DevComponents.DotNetBar.SuperTabStripSelectedTabChangedEventArgs) Handles SuperTabControlMenu.SelectedTabChanged

    End Sub

    Private Sub SuperTabControlPanel1_Click(sender As Object, e As EventArgs) Handles SuperTabControlPanel1.Click

    End Sub

    Private Sub MetroTileItem2_Click(sender As Object, e As EventArgs) Handles btConfCliente.Click

    End Sub

    Private Sub MetroTileItem2_Click_1(sender As Object, e As EventArgs)

    End Sub

    Private Sub MetroTileItem5_Click(sender As Object, e As EventArgs) Handles btComProveedor.Click

    End Sub

    Private Sub btnFecha_Click(sender As Object, e As EventArgs) Handles btnFecha.Click

    End Sub

    Private Sub MetroTileItem34_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub MetroTileItem29_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub MetroTileItem29_Click_1(sender As Object, e As EventArgs) Handles btZonaMapaCliente.Click

    End Sub

    Private Sub btInvSaldo_Click(sender As Object, e As EventArgs) Handles btInvSaldo.Click

    End Sub

    Private Sub Tec_Principal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        imguser.Width = tab_compraventa.Size.Width
        Dim blah As New Bitmap(New Bitmap(My.Resources.compra), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Me.Close()

    End Sub

    Private Sub btConfRoles_Click(sender As Object, e As EventArgs) Handles btConfRoles.Click
        SuperTabControlMenu.SelectedTab = tab_ventana
        'Dim frm As New F0_Roles
        Dim frm As New Tec_Roles
        frm._nameButton = btConfRoles.Name
        'frm._modulo = FP_Configuracion
        Dim tab3 As SuperTabItem = superTabControl3.CreateTab(frm.Text)
        tab3.RecalcSize()
        tab3.ThemeAware = True
        tab3.ShowSubItems = True
        tab3.UpdateBindings()
        frm._tab = tab3
        Dim panel As Panel = P_Global._fnCrearPanelVentanas(frm)
        superTabControl3.SelectedTabIndex = superTabControl3.Tabs.Count - 1
        tab3.AttachedControl.Controls.Add(panel)
        frm.Show()
        tab3.Text = frm.Text
        tab3.Icon = frm.Icon
    End Sub
End Class