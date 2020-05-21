Public Class Rep_SaldoProductos
    Private Sub Rep_SaldoProductos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub
    Private Sub _prCargarComboLibreriaSucursal(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnListarSucursales()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("aanumi").Width = 60
            .DropDownList.Columns("aanumi").Caption = "COD"
            .DropDownList.Columns.Add("aabdes").Width = 300
            .DropDownList.Columns("aabdes").Caption = "SUCURSAL"
            .ValueMember = "aanumi"
            .DisplayMember = "aabdes"
            .DataSource = dt
            .Refresh()
        End With
        If (dt.Rows.Count > 0) Then
            cbAlmacen.SelectedIndex = 0
        End If
    End Sub
    Private Sub _prCargarComboGrupos(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnObtenerGruposLibreria()
        'a.ylcod2,yldes2
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("yccod3").Width = 60
            .DropDownList.Columns("yccod3").Caption = "COD"
            .DropDownList.Columns.Add("yldes2").Width = 250
            .DropDownList.Columns("yldes2").Caption = "GRUPOS"
            .ValueMember = "yccod3"
            .DisplayMember = "yldes2"
            .DataSource = dt
            .Refresh()
        End With
        If (dt.Rows.Count > 0) Then
            cbGrupos.SelectedIndex = 0
        End If
    End Sub
    Private Sub _prCargarComboLibreriaPrecioCosto(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prListarPrecioCosto()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("ygnumi").Width = 60
            .DropDownList.Columns("ygnumi").Caption = "COD"
            .DropDownList.Columns.Add("ygdesc").Width = 500
            .DropDownList.Columns("ygdesc").Caption = "SUCURSAL"
            .ValueMember = "ygnumi"
            .DisplayMember = "ygdesc"
            .DataSource = dt
            .Refresh()
        End With
        If (dt.Rows.Count > 0) Then
            cbGrupos.SelectedIndex = 0
        End If
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click

        _tab.Close()

    End Sub

    Private Sub swTipoVenta_ValueChanged(sender As Object, e As EventArgs)
        If (bandera = False) Then
            Return
        End If
        'If (swTipoVenta.Value = True) Then
        '    _prCargarComboLibreriaPrecioVenta(cbGrupos)
        'Else
        '    _prCargarComboLibreriaPrecioCosto(cbGrupos)

        'End If

    End Sub

    Sub _prInhabilitarAlmacen()
        cbAlmacen.Enabled = False
    End Sub
    Sub _prhabilitarAlmacen()
        cbAlmacen.Enabled = True
    End Sub

    Sub _prInhabilitarGrupos()
        cbGrupos.Enabled = False
    End Sub
    Sub _prhabilitarGrupos()
        cbGrupos.Enabled = True
    End Sub
    Private Sub CheckTodosVendedor_CheckValueChanged(sender As Object, e As EventArgs) Handles CheckTodosAlmacen.CheckValueChanged
        If (CheckTodosAlmacen.Checked) Then
            _prInhabilitarAlmacen()
        Else
            _prhabilitarAlmacen()
        End If

    End Sub
    'grup panel stock mayor a cero o todos


    Private Sub checkTodosGrupos_CheckValueChanged(sender As Object, e As EventArgs) Handles checkTodosGrupos.CheckValueChanged
        If (checkTodosGrupos.Checked) Then
            _prInhabilitarGrupos()
        Else
            _prhabilitarGrupos()
        End If
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        _prCargarReporte()
    End Sub
End Class