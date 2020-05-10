Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls

Public Class Tec_Productos

#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public FilaSeleccionada As Boolean = False

    Public _MListEstBuscador As List(Of Modelo.Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"

#End Region

#Region "Metodos Overrides"
    Public Sub _PMIniciarTodo()
        _MListEstBuscador = _PMOGetListEstructuraBuscador()
        Me.WindowState = FormWindowState.Maximized

        _PMFiltrar()
        _PMInhabilitar()
    End Sub

    Private Sub _PMCargarBuscador()

        Dim dtBuscador As DataTable = _PMOGetTablaBuscador()

        JGrM_Buscador.DataSource = dtBuscador
        JGrM_Buscador.RetrieveStructure()

        For i = 0 To _MListEstBuscador.Count - 1
            Dim campo As String = _MListEstBuscador.Item(i).campo
            With JGrM_Buscador.RootTable.Columns(campo)
                If _MListEstBuscador.Item(i).visible = True Then
                    .Caption = _MListEstBuscador.Item(i).titulo
                    .Width = _MListEstBuscador.Item(i).tamano
                    .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center

                    Dim col As DataColumn = dtBuscador.Columns(campo)
                    Dim tipo As Type = col.DataType
                    If tipo.ToString = "System.Int32" Or tipo.ToString = "System.Decimal" Or tipo.ToString = "System.Double" Then
                        .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                    End If
                    If _MListEstBuscador.Item(i).formato <> String.Empty Then
                        .FormatString = _MListEstBuscador.Item(i).formato
                    End If
                Else
                    .Visible = False
                End If
            End With
        Next

        'Habilitar Filtradores
        With JGrM_Buscador
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With

    End Sub



    Public Sub _PMInhabilitar()
        btnNuevo.Visible = True
        btnModificar.Visible = True
        btnEliminar.Visible = True
        btnGrabar.Visible = False
        PanelNavegacion.Enabled = True
        JGrM_Buscador.Enabled = True


        _PMOLimpiarErrores()

        _PMOInhabilitar()
    End Sub

    Private Sub _PMHabilitar()
        JGrM_Buscador.Enabled = False
        _PMOHabilitar()
    End Sub
    Public Sub _PMFiltrar()
        'cargo el buscador
        _PMCargarBuscador()
        If JGrM_Buscador.RowCount > 0 Then
            _MPos = 0
            JGrM_Buscador.Row = _MPos
            _PMOMostrarRegistro(_MPos)
        Else
            _PMOLimpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub

    Public Sub _PMPrimerRegistro()
        If JGrM_Buscador.RowCount > 0 Then
            _MPos = 0

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub _PMAnteriorRegistro()
        If _MPos > 0 And JGrM_Buscador.RowCount > 0 Then
            _MPos = _MPos - 1

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub _PMSiguienteRegistro()
        If _MPos < JGrM_Buscador.RowCount - 1 Then
            _MPos = _MPos + 1

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub
    Private Sub _PMUltimoRegistro()
        If JGrM_Buscador.RowCount > 0 Then
            _MPos = JGrM_Buscador.RowCount - 1

            _PMOMostrarRegistro(_MPos)
        End If
    End Sub

    Private Sub _PMNuevo()
        _MNuevo = True
        _MModificar = False

        _PMOLimpiar()
        _PMHabilitar()

        btnNuevo.Visible = False
        btnModificar.Visible = False
        btnEliminar.Visible = False
        btnGrabar.Visible = True
        PanelNavegacion.Enabled = False
        tbNombreProducto.Focus()


        '_PMOLimpiar()

    End Sub

    Private Sub _PMModificar()
        If JGrM_Buscador.Row >= 0 Then
            _MNuevo = False
            _MModificar = True

            _PMHabilitar()
            btnNuevo.Visible = False
            btnModificar.Visible = False
            btnEliminar.Visible = False
            btnGrabar.Visible = True

            PanelNavegacion.Enabled = False


        End If
    End Sub

    Private Sub _PMEliminar()
        'Dim _Result As MsgBoxResult
        '_Result = MsgBox("¿Esta seguro de Eliminar el Registro?".ToUpper, MsgBoxStyle.YesNo, "Advertencia".ToUpper)
        'If _Result = MsgBoxResult.Yes Then
        '    _PMOEliminarRegistro()
        '    _PMFiltrar()

        'End If
        _PMOEliminarRegistro()
    End Sub

    Private Sub _PMGuardar()

        If _PMOValidarCampos() = False Then
            Exit Sub
        End If

        If _MNuevo Then
            If _PMOGrabarRegistro() = True Then
                'actualizar el grid de buscador
                _PMCargarBuscador()

                _PMOLimpiar()
                _PMSalir()
            Else
                Exit Sub
            End If

        Else

            _PMOModificarRegistro()

            'actualizar el grid de buscador
            _PMCargarBuscador()

            '_PMSalir()
        End If
    End Sub

    Private Sub _PMSalir()
        If btnGrabar.Visible = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()

        Else
            '  Public _modulo As SideNavItem
            '_modulo.Select()
            _tab.Close()
        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()
        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "Gestion De Productos"
        P_Global._prCargarComboGenerico(cbEmpresa, L_prListaEmpresasUsuarios(), "Id", "Codigo", "Nombre", "Empresa")
        P_Global._prCargarComboGenerico(cbCategoria, L_prListaCategorias(), "Id", "Codigo", "NombreCategoria", "Categoria")

        P_Global._prCargarComboGenerico(cbProveedor, L_prLibreriaDetalleGeneral(2), "cnnum", "Codigo", "cndesc1", "Proveedor")
        P_Global._prCargarComboGenerico(cbMarca, L_prLibreriaDetalleGeneral(3), "cnnum", "Codigo", "cndesc1", "Marca")
        P_Global._prCargarComboGenerico(cbAtributo, L_prLibreriaDetalleGeneral(4), "cnnum", "Codigo", "cndesc1", "Attributo")
        P_Global._prCargarComboGenerico(cbFamilia, L_prLibreriaDetalleGeneral(5), "cnnum", "Codigo", "cndesc1", "Familia")

        P_Global._prCargarComboGenerico(cbUniVenta, L_prLibreriaDetalleGeneral(6), "cnnum", "Codigo", "cndesc1", "Unidad Venta")
        P_Global._prCargarComboGenerico(cbUnidMaxima, L_prLibreriaDetalleGeneral(7), "cnnum", "Codigo", "cndesc1", "Unidad Maxima")

        _PMIniciarTodo()
        _prAsignarPermisos()


        Dim blah As New Bitmap(New Bitmap(My.Resources.ic_c), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico

    End Sub






    Private Sub _prAsignarPermisos()

        'Dim dtRolUsu As DataTable = L_prRolDetalleGeneral(gi_userRol, _nameButton)

        'Dim show As Boolean = dtRolUsu.Rows(0).Item("ycshow")
        'Dim add As Boolean = dtRolUsu.Rows(0).Item("ycadd")
        'Dim modif As Boolean = dtRolUsu.Rows(0).Item("ycmod")
        'Dim del As Boolean = dtRolUsu.Rows(0).Item("ycdel")

        'If add = False Then
        '    btnNuevo.Visible = False
        'End If
        'If modif = False Then
        '    btnModificar.Visible = False
        'End If
        'If del = False Then
        '    btnEliminar.Visible = False
        'End If

    End Sub





#End Region

#Region "METODOS SOBRECARGADOS"


    Public Sub _PMOHabilitar()
        '     (@Id,@CodigoExterno ,@CodigoBarras ,@NombreProducto ,@DescripcionProducto ,
        '@StockMinimo ,@estado ,@CategoriaId ,@EmpresaId ,@ProveedorId ,@MarcaId ,
        '@AttributoId ,@FamiliaId ,
        '@UnidadVentaId ,@UnidadMaximaId ,@Conversion ,@newFecha,@newHora,@usuario )
        tbNombreProducto.ReadOnly = False
        tbCodigoExterno.ReadOnly = False
        tbDescripcion.ReadOnly = False
        tbStockMinimo.IsInputReadOnly = False
        swEstado.IsReadOnly = False
        cbEmpresa.ReadOnly = False
        cbCategoria.ReadOnly = False
        cbProveedor.ReadOnly = False
        cbMarca.ReadOnly = False
        cbAtributo.ReadOnly = False
        cbFamilia.ReadOnly = False
        cbUniVenta.ReadOnly = False
        cbUnidMaxima.ReadOnly = False
        tbConversion.IsInputReadOnly = False


    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreProducto.ReadOnly = True
        tbCodigoExterno.ReadOnly = True
        tbDescripcion.ReadOnly = True
        tbStockMinimo.IsInputReadOnly = True
        swEstado.IsReadOnly = True
        cbEmpresa.ReadOnly = True
        cbCategoria.ReadOnly = True
        cbProveedor.ReadOnly = True
        cbMarca.ReadOnly = True
        cbAtributo.ReadOnly = True
        cbFamilia.ReadOnly = True
        cbUniVenta.ReadOnly = True
        cbUnidMaxima.ReadOnly = True
        tbConversion.IsInputReadOnly = True
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbNombreProducto.Text = ""
        tbDescripcion.Text = ""
        tbCodigoExterno.Text = ""
        tbStockMinimo.Value = 0
        tbConversion.Value = 0
        swEstado.Value = True
        seleccionarPrimerItemCombo(cbEmpresa)
        seleccionarPrimerItemCombo(cbCategoria)
        seleccionarPrimerItemCombo(cbProveedor)
        seleccionarPrimerItemCombo(cbMarca)
        seleccionarPrimerItemCombo(cbAtributo)
        seleccionarPrimerItemCombo(cbFamilia)
        seleccionarPrimerItemCombo(cbUniVenta)
        seleccionarPrimerItemCombo(cbUnidMaxima)
        tbNombreProducto.Focus()


    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbNombreProducto.BackColor = Color.White
        tbDescripcion.BackColor = Color.White
        tbStockMinimo.BackColor = Color.White
        tbConversion.BackColor = Color.White
        cbEmpresa.BackColor = Color.White
        cbCategoria.BackColor = Color.White

        cbProveedor.BackColor = Color.White
        cbMarca.BackColor = Color.White
        cbAtributo.BackColor = Color.White
        cbFamilia.BackColor = Color.White
        cbUniVenta.BackColor = Color.White
        cbUnidMaxima.BackColor = Color.White

    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        'ByRef _numi As String, _CodigoExterno As String,
        '                                        _CodigoBarra As String, _NombreProducto As String,
        '_Descripcion As String, _stockMinimo As Decimal, _estado As Integer, _CategoriaId As Integer,
        '_EmpresaId As Integer, _ProveedorId As Integer, _MarcaId As Integer,
        ''_AttributoId As Integer, _FamiliaId As Integer, _UnidadVentaId As Integer,
        '_UnidadMaximaId As Integer,
        ''_conversion As Double
        Dim res As Boolean
        Try
            res = L_prProductoInsertar(tbCodigo.Text, tbCodigoExterno.Text, "", tbNombreProducto.Text,
                                                 tbDescripcion.Text, tbStockMinimo.Value, IIf(swEstado.Value = True, 1, 0),
                                                 cbCategoria.Value, cbEmpresa.Value, cbProveedor.Value, cbMarca.Value,
                                                 cbAtributo.Value, cbFamilia.Value, cbUniVenta.Value, cbUnidMaxima.Value, tbConversion.Value)

            If res Then


                ToastNotification.Show(Me, "Codigo de Producto ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar el producto".ToUpper, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el producto".ToUpper + " " + ex.Message, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = L_prProductoModificar(tbCodigo.Text, tbCodigoExterno.Text, "", tbNombreProducto.Text,
                                                tbDescripcion.Text, tbStockMinimo.Value, IIf(swEstado.Value = True, 1, 0),
                                                cbCategoria.Value, cbEmpresa.Value, cbProveedor.Value, cbMarca.Value,
                                                cbAtributo.Value, cbFamilia.Value, cbUniVenta.Value, cbUnidMaxima.Value, tbConversion.Value)


            If Res Then

                ToastNotification.Show(Me, "Codigo de Producto ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el producto".ToUpper, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar el producto".ToUpper + " " + ex.Message, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbDescripcion.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Producto " + tbNombreProducto.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prProductoBorrar(tbCodigo.Text, mensajeError)
                If res Then

                    ToastNotification.Show(Me, "Codigo de Categoria ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el producto".ToUpper + " " + ex.Message, My.Resources.WARNING, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "
        If tbNombreProducto.Text = String.Empty Then
            tbNombreProducto.BackColor = Color.Red
            MEP.SetError(tbNombreProducto, "Ingrese Nombre de Producto")
            Mensaje = Mensaje + " Nombre Producto"
            _ok = False
        Else
            tbNombreProducto.BackColor = Color.White
            MEP.SetError(tbNombreProducto, "")
        End If
        If (cbEmpresa.SelectedIndex < 0) Then
            cbEmpresa.BackColor = Color.Red
            MEP.SetError(cbEmpresa, "Seleccione una Empresa")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Empresa"
            _ok = False
        Else
            cbEmpresa.BackColor = Color.White
            MEP.SetError(cbEmpresa, "")
        End If

        If (cbCategoria.SelectedIndex < 0) Then
            cbCategoria.BackColor = Color.Red
            MEP.SetError(cbCategoria, "Seleccione una Categoria")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Categoria"
            _ok = False
        Else
            cbCategoria.BackColor = Color.White
            MEP.SetError(cbCategoria, "")
        End If

        If (tbStockMinimo.Text.Length <= 0) Then
            tbStockMinimo.BackColor = Color.Red
            MEP.SetError(tbStockMinimo, "Ingrese valor Stock Minimo")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Stock Minimo"
            _ok = False
        Else
            tbStockMinimo.BackColor = Color.White
            MEP.SetError(tbStockMinimo, "")
        End If
        If (tbConversion.Text.Length <= 0) Then
            tbConversion.BackColor = Color.Red
            MEP.SetError(tbConversion, "Ingrese Valor en Conversion")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Conversion"
            _ok = False
        Else
            tbConversion.BackColor = Color.White
            MEP.SetError(tbConversion, "")
        End If
        If (cbProveedor.SelectedIndex < 0) Then
            cbProveedor.BackColor = Color.Red
            MEP.SetError(cbProveedor, "Seleccione un Proveedor")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Proveedor"
            _ok = False
        Else
            cbProveedor.BackColor = Color.White
            MEP.SetError(cbProveedor, "")
        End If
        If (cbMarca.SelectedIndex < 0) Then
            cbMarca.BackColor = Color.Red
            MEP.SetError(cbMarca, "Seleccione una Marca")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Marca"
            _ok = False
        Else
            cbMarca.BackColor = Color.White
            MEP.SetError(cbMarca, "")
        End If
        If (cbAtributo.SelectedIndex < 0) Then
            cbAtributo.BackColor = Color.Red
            MEP.SetError(cbAtributo, "Seleccione un Atributo")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Attributo"
            _ok = False
        Else
            cbAtributo.BackColor = Color.White
            MEP.SetError(cbAtributo, "")
        End If
        If (cbFamilia.SelectedIndex < 0) Then
            cbFamilia.BackColor = Color.Red
            MEP.SetError(cbFamilia, "Seleccione una Familia")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Familia"
            _ok = False
        Else
            cbFamilia.BackColor = Color.White
            MEP.SetError(cbFamilia, "")
        End If

        MHighlighterFocus.UpdateHighlights()

        If tbNombreProducto.Text = String.Empty Then
            tbNombreProducto.Focus()
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If

        If (cbEmpresa.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbEmpresa.Focus()
            Return _ok
        End If
        If (cbCategoria.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbCategoria.Focus()
            Return _ok
        End If
        If (tbStockMinimo.Text.Length <= 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbStockMinimo.Focus()
            Return _ok
        End If

        If (cbProveedor.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbProveedor.Focus()
            Return _ok
        End If
        If (cbMarca.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbMarca.Focus()
            Return _ok
        End If
        If (cbAtributo.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbAtributo.Focus()
            Return _ok
        End If
        If (tbConversion.Text.Length <= 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbConversion.Focus()
            Return _ok
        End If
        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Productos")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Modelo.Celda)

        'Select Case a.Id ,a.CodigoExterno ,a.CodigoBarras ,a.NombreProducto ,a.DescripcionProducto 
        ',a.StockMinimo ,a.Estado ,
        '    a.CategoriaId , cat.NombreCategoria, a.EmpresaId, em.Nombre As Empresa, a.ProveedorId, 
        'a.MarcaId,
        '    a.AttributoId, a.FamiliaId, a.UnidadVentaId, a.UnidadMaximaId, a.Conversion  
        Dim listEstCeldas As New List(Of Modelo.Celda)
        listEstCeldas.Add(New Modelo.Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Modelo.Celda("CodigoExterno", False))
        listEstCeldas.Add(New Modelo.Celda("CodigoBarras", False))
        listEstCeldas.Add(New Modelo.Celda("NombreProducto", True, " NombreProducto", 100))
        listEstCeldas.Add(New Modelo.Celda("DescripcionProducto", True, " Descripcion Producto", 100))
        listEstCeldas.Add(New Modelo.Celda("StockMinimo", True, "Stock Minimo", 90))
        listEstCeldas.Add(New Modelo.Celda("estado", True, "Estado", 70))
        listEstCeldas.Add(New Modelo.Celda("CategoriaId", False))
        listEstCeldas.Add(New Modelo.Celda("NombreCategoria", True, "Categoria", 80))
        listEstCeldas.Add(New Modelo.Celda("EmpresaId", False))
        listEstCeldas.Add(New Modelo.Celda("Empresa", True, "Empresa", 80))

        listEstCeldas.Add(New Modelo.Celda("ProveedorId", False))
        listEstCeldas.Add(New Modelo.Celda("MarcaId", False))
        listEstCeldas.Add(New Modelo.Celda("AttributoId", False))
        listEstCeldas.Add(New Modelo.Celda("FamiliaId", False))
        listEstCeldas.Add(New Modelo.Celda("UnidadVentaId", False))
        listEstCeldas.Add(New Modelo.Celda("UnidadMaximaId", False))
        listEstCeldas.Add(New Modelo.Celda("Conversion", False))


        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        'Select Case a.Id ,a.CodigoExterno ,a.CodigoBarras ,a.NombreProducto ,a.DescripcionProducto 
        ',a.StockMinimo ,a.Estado ,
        '    a.CategoriaId , cat.NombreCategoria, a.EmpresaId, em.Nombre As Empresa, a.ProveedorId, 
        'a.MarcaId,
        '    a.AttributoId, a.FamiliaId, a.UnidadVentaId, a.UnidadMaximaId, a.Conversion  
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbCodigoExterno.Text = .GetValue("CodigoExterno").ToString
            tbNombreProducto.Text = .GetValue("NombreProducto").ToString
            tbDescripcion.Text = .GetValue("DescripcionProducto").ToString
            tbStockMinimo.Value = .GetValue("StockMinimo")
            swEstado.Value = .GetValue("estado")
            cbCategoria.Value = .GetValue("CategoriaId")
            cbEmpresa.Value = .GetValue("EmpresaId")
            cbProveedor.Value = .GetValue("ProveedorId")
            cbMarca.Value = .GetValue("MarcaId")
            cbAtributo.Value = .GetValue("AttributoId")
            cbFamilia.Value = .GetValue("FamiliaId")
            cbUniVenta.Value = .GetValue("UnidadVentaId")
            cbUnidMaxima.Value = .GetValue("UnidadMaximaId")
            tbConversion.Value = .GetValue("Conversion")
        End With


        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub

    Private Function P_fnObtenerID() As String
        Dim res As String = ""
        res = res + Now.Hour.ToString("00") + Now.Minute.ToString("00") + Now.Second.ToString("00") + "_" _
            + Now.Day.ToString("00") + Now.Month.ToString("00") + Now.Year.ToString("0000")
        Return res
    End Function

    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()

        Else
            '  Public _modulo As SideNavItem
            _modulo.Select()
            _tab.Close()
        End If
    End Sub

    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _PMNuevo()

    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        _PMModificar()

    End Sub

    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        _PMGuardar()

    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        _PMEliminar()


    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _PMSalir()

    End Sub

    Private Sub Tec_Users_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        _PMPrimerRegistro()
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        _PMAnteriorRegistro()
    End Sub

    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        _PMSiguienteRegistro()
    End Sub

    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        _PMUltimoRegistro()
    End Sub



    Private Sub JGrM_Buscador_SelectionChanged(sender As Object, e As EventArgs) Handles JGrM_Buscador.SelectionChanged
        If (JGrM_Buscador.Row >= 0 And FilaSeleccionada = False) Then
            _MPos = JGrM_Buscador.Row

            _PMOMostrarRegistro(_MPos, True)

        End If
    End Sub

    Private Sub cbProveedor_ValueChanged(sender As Object, e As EventArgs) Handles cbProveedor.ValueChanged
        If cbProveedor.SelectedIndex < 0 And cbProveedor.Text <> String.Empty Then
            btnProveedor.Visible = True
        Else
            btnProveedor.Visible = False
        End If
    End Sub

    Private Sub cbMarca_ValueChanged(sender As Object, e As EventArgs) Handles cbMarca.ValueChanged
        If cbMarca.SelectedIndex < 0 And cbMarca.Text <> String.Empty Then
            btnMarca.Visible = True
        Else
            btnMarca.Visible = False
        End If
    End Sub

    Private Sub cbAtributo_ValueChanged(sender As Object, e As EventArgs) Handles cbAtributo.ValueChanged
        If cbAtributo.SelectedIndex < 0 And cbAtributo.Text <> String.Empty Then
            btnAtributo.Visible = True
        Else
            btnAtributo.Visible = False
        End If
    End Sub

    Private Sub cbFamilia_ValueChanged(sender As Object, e As EventArgs) Handles cbFamilia.ValueChanged
        If cbFamilia.SelectedIndex < 0 And cbFamilia.Text <> String.Empty Then
            btnFamilia.Visible = True
        Else
            btnFamilia.Visible = False
        End If
    End Sub

    Private Sub cbUniVenta_ValueChanged(sender As Object, e As EventArgs) Handles cbUniVenta.ValueChanged
        If cbUniVenta.SelectedIndex < 0 And cbUniVenta.Text <> String.Empty Then
            btUniVenta.Visible = True
        Else
            btUniVenta.Visible = False
        End If
    End Sub

    Private Sub cbUnidMaxima_ValueChanged(sender As Object, e As EventArgs) Handles cbUnidMaxima.ValueChanged
        If cbUnidMaxima.SelectedIndex < 0 And cbUnidMaxima.Text <> String.Empty Then
            btUniMaxima.Visible = True
        Else
            btUniMaxima.Visible = False
        End If
    End Sub

#End Region
End Class