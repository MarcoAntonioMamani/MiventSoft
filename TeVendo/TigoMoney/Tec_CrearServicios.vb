Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Public Class Tec_CrearServicios

#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public FilaSeleccionada As Boolean = False
    Public _TabControl As SuperTabControl

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Public _MListEstBuscador As List(Of Celda)
    Public _MPos As Integer
    Public _MNuevo As Boolean
    Public _MModificar As Boolean

    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"


    Dim TablaImagenes As DataTable
    Dim TablaInventario As DataTable
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim gs_DirPrograma As String = ""
    Dim gs_RutaImg As String = ""

    Dim ProductoId As Integer = 0

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
                    .WordWrap = True
                    .MaxLines = 4
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
            .Height = 200
        End With



    End Sub
    Public Sub CargarIconEstado()

        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Dim n As Integer = dt.Rows.Count
        For i As Integer = 0 To n - 1 Step 1
            If (dt.Rows(i).Item("estado") = 1) Then
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.activo, 110, 30)
                img.Save(Bin, Imaging.ImageFormat.Png)
                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("imgEstado") = Bin.GetBuffer
            Else
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(My.Resources.pasivo, 110, 30)
                img.Save(Bin, Imaging.ImageFormat.Png)
                CType(JGrM_Buscador.DataSource, DataTable).Rows(i).Item("imgEstado") = Bin.GetBuffer
            End If

        Next

    End Sub


    Public Sub _PMInhabilitar()
        btnNuevo.Visible = False
        btnModificar.Visible = False
        btnEliminar.Visible = False
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
        tbNombreServicio.Focus()


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
            _PMSalir()
            '_PMSalir()
        End If
    End Sub

    Private Sub _PMSalir()
        If btnGrabar.Visible = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()
            TabControlPrincipal.SelectedTabIndex = 1
        Else
            '  Public _modulo As SideNavItem
            '_modulo.Select()
            TabControlPrincipal.SelectedTabIndex = 1
            '_tab.Close()
        End If
    End Sub
#End Region

#Region "METODOS PRIVADOS"

    Private Sub _prIniciarTodo()



        'L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        Me.Text = "ADMINISTRAR SERVICIOS"
        P_Global._prCargarComboGenerico(cbNegocio, L_prLibreriaDetalleGeneral(14), "cnnum", "Codigo", "cndesc1", "Negocio")
        P_Global._prCargarComboGenerico(cbCategoria, L_prLibreriaDetalleGeneral(15), "cnnum", "Codigo", "cndesc1", "Categoria")




        _PMIniciarTodo()
        _prAsignarPermisos()


        _habilitarFocus()
        _prEliminarContenidoImage()
    End Sub

    Public Sub _habilitarFocus()
        With Highlighter2
            .SetHighlightOnFocus(tbNombreServicio, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbMontoServicio, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbNegocio, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbCategoria, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDescripcion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbMontoServicio, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbPorcentajeGanancia, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbMontoGanancia, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbAdicional, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbProducto, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)


        End With
    End Sub


    Private Sub _prCrearCarpetaImagenes(carpetaFinal As String)
        Dim rutaDestino As String = RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal + "\"

        If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal) = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Imagenes") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes")
                If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos")
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal + "\")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Productos") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos")
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal + "\")
                Else
                    If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal) = False Then
                        System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Productos\" + carpetaFinal + "\")
                    End If

                End If
            End If
        End If
    End Sub

    Private Sub _prCrearCarpetaTemporal()

        If System.IO.Directory.Exists(RutaTemporal) = False Then
            System.IO.Directory.CreateDirectory(RutaTemporal)
        Else

            My.Computer.FileSystem.DeleteDirectory(RutaTemporal, FileIO.DeleteDirectoryOption.DeleteAllContents)
            My.Computer.FileSystem.CreateDirectory(RutaTemporal)

        End If

    End Sub
    Sub _prEliminarContenidoImage()
        Try
            My.Computer.FileSystem.DeleteDirectory(gs_RutaImg, FileIO.DeleteDirectoryOption.DeleteAllContents)
        Catch ex As Exception

        End Try

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



    Public Function _fnObtenerNumeroFilasActivas() As Integer
        Dim n As Integer = -1
        For i As Integer = 0 To TablaImagenes.Rows.Count - 1 Step 1
            Dim estado As Integer = TablaImagenes.Rows(i).Item("estado")
            If (estado = 0 Or estado = 1) Then
                n += 1

            End If
        Next
        Return n
    End Function



#End Region

#Region "METODOS SOBRECARGADOS"


    Public Sub _PMOHabilitar()
        '     (@Id,@CodigoExterno ,@CodigoBarras ,@NombreProducto ,@DescripcionProducto ,
        '@StockMinimo ,@estado ,@CategoriaId ,@EmpresaId ,@ProveedorId ,@MarcaId ,
        '@AttributoId ,@FamiliaId ,
        '@UnidadVentaId ,@UnidadMaximaId ,@Conversion ,@newFecha,@newHora,@usuario )
        tbNombreServicio.ReadOnly = False
        tbMontoServicio.IsInputReadOnly = False
        cbNegocio.ReadOnly = False
        swEstado.IsReadOnly = False
        btnSearchProducto.Visible = True
        cbCategoria.ReadOnly = False
        tbMontoServicio.IsInputReadOnly = False
        tbPorcentajeGanancia.IsInputReadOnly = False
        tbMontoGanancia.IsInputReadOnly = False
        tbAdicional.IsInputReadOnly = False


    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreServicio.ReadOnly = True
        tbMontoServicio.IsInputReadOnly = True
        cbNegocio.ReadOnly = True
        swEstado.IsReadOnly = True
        tbProducto.ReadOnly = True
        btnSearchProducto.Visible = False
        cbCategoria.ReadOnly = True
        tbMontoServicio.IsInputReadOnly = True
        tbPorcentajeGanancia.IsInputReadOnly = True
        tbMontoGanancia.IsInputReadOnly = True
        tbAdicional.IsInputReadOnly = True
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbNombreServicio.Text = ""
        tbMontoServicio.Value = 0

        swEstado.Value = True
        seleccionarPrimerItemCombo(cbNegocio)
        seleccionarPrimerItemCombo(cbCategoria)
        tbNombreServicio.Focus()
        tbDescripcion.Clear()
        tbProducto.Clear()
        tbPorcentajeGanancia.Value = 0
        tbMontoGanancia.Value = 0
        tbAdicional.Value = 0
        ProductoId = 0



    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbNombreServicio.BackColor = Color.White
        cbNegocio.BackColor = Color.White
        tbMontoServicio.BackColor = Color.White


    End Sub

    Public Function _PMOGrabarRegistro() As Boolean
        'ByRef _numi As String, _CodigoExterno As String,
        '                                        _CodigoBarra As String, _NombreProducto As String,
        '_Descripcion As String, _stockMinimo As Decimal, _estado As Integer, _CategoriaId As Integer,
        '_EmpresaId As Integer, _ProveedorId As Integer, _MarcaId As Integer,
        ''_AttributoId As Integer, _FamiliaId As Integer, _UnidadVentaId As Integer,
        '_UnidadMaximaId As Integer,
        ''_conversion As Double

        Dim Categoria As Integer = 0

        If (cbCategoria.SelectedIndex > 0) Then
            Categoria = cbCategoria.Value
        End If


        Dim res As Boolean
        Try
            res = L_prServiciosInsertar(tbCodigo.Text, ProductoId, tbNombreServicio.Text, tbDescripcion.Text, tbMontoServicio.Value, tbMontoGanancia.Value, tbPorcentajeGanancia.Value, tbAdicional.Value, cbNegocio.Value, Categoria, IIf(swEstado.Value = True, 1, 0))

            If res Then



                ToastNotification.Show(Me, "Concepto Servicio  ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar el Servicio".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Servicio".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Dim Categoria As Integer = 0

        If (cbCategoria.SelectedIndex > 0) Then
            Categoria = cbCategoria.Value
        End If

        Try
            Res = L_prServiciosModificar(tbCodigo.Text, ProductoId, tbNombreServicio.Text, tbDescripcion.Text, tbMontoServicio.Value, tbMontoGanancia.Value, tbPorcentajeGanancia.Value, tbAdicional.Value, cbNegocio.Value, Categoria, IIf(swEstado.Value = True, 1, 0))


            If Res Then


                ToastNotification.Show(Me, "Codigo de Servicio ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el Servicio".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar el Servicio".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbNombreServicio.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Servicio = " + tbNombreServicio.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_Servicios")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Servicio ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el Servicio".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "
        If tbNombreServicio.Text = String.Empty Then
            tbNombreServicio.BackColor = Color.Red
            MEP.SetError(tbNombreServicio, "Ingrese Nombre de Servicio")
            Mensaje = Mensaje + " Nombre Servicio"
            _ok = False
        Else
            tbNombreServicio.BackColor = Color.White
            MEP.SetError(tbNombreServicio, "")
        End If
        If (cbNegocio.SelectedIndex < 0) Then
            cbNegocio.BackColor = Color.Red
            MEP.SetError(cbNegocio, "Seleccione un Negocio")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Negocio"
            _ok = False
        Else
            cbNegocio.BackColor = Color.White
            MEP.SetError(cbNegocio, "")
        End If







        Highlighter2.UpdateHighlights()

        If tbNombreServicio.Text = String.Empty Then
            tbNombreServicio.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If

        If (cbNegocio.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbNegocio.Focus()
            Return _ok
        End If




        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Servicios")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'a.Id,a.ProductoId,isnull(pr.NombreProducto,'') as NombreProducto,
        'a.NombreServicio , a.DescripcionServicio, a.MontoServicio, a.MontoGanancia, a.PorcentajeGanancia,
        '    a.PagoAdiccional, a.EmpresaBancariaId, emp.Descripcion As EmpresaBancaria, a.CategoriaServicioId, cat.Descripcion As CategoriaServicio,
        '    a.Estado
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("ProductoId", False))
        listEstCeldas.Add(New Celda("NombreProducto", False))
        listEstCeldas.Add(New Celda("NombreServicio", True, " Servicio", 250))
        listEstCeldas.Add(New Celda("DescripcionServicio", False))

        listEstCeldas.Add(New Celda("MontoServicio", True, "Monto Servicio", 100, "0.00"))

        listEstCeldas.Add(New Celda("MontoGanancia", False))
        listEstCeldas.Add(New Celda("PorcentajeGanancia", False))
        listEstCeldas.Add(New Celda("PagoAdiccional", False))
        listEstCeldas.Add(New Celda("EmpresaBancariaId", False))

        listEstCeldas.Add(New Celda("EmpresaBancaria", True, "Negocio", 90, ""))


        listEstCeldas.Add(New Celda("CategoriaServicio", True, "Categoria", 90, ""))
        listEstCeldas.Add(New Celda("Estado", False, "Estado", 70))
        listEstCeldas.Add(New Celda("CategoriaServicioId", False, "Estado", 150))


        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        'a.Id,a.ProductoId,isnull(pr.NombreProducto,'') as NombreProducto,
        'a.NombreServicio , a.DescripcionServicio, a.MontoServicio, a.MontoGanancia, a.PorcentajeGanancia,
        '    a.PagoAdiccional, a.EmpresaBancariaId, emp.Descripcion As EmpresaBancaria, a.CategoriaServicioId, cat.Descripcion As CategoriaServicio,
        '    a.Estado

        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbNombreServicio.Text = .GetValue("NombreServicio").ToString
            ProductoId = .GetValue("ProductoId")
            tbProducto.Text = .GetValue("NombreProducto")
            tbDescripcion.Text = .GetValue("DescripcionServicio")
            tbMontoServicio.Value = .GetValue("MontoServicio")
            tbMontoGanancia.Value = .GetValue("MontoGanancia")
            tbPorcentajeGanancia.Value = .GetValue("PorcentajeGanancia")
            tbAdicional.Value = .GetValue("PagoAdiccional")
            cbNegocio.Value = .GetValue("EmpresaBancariaId")
            cbCategoria.Value = .GetValue("CategoriaServicioId")
            swEstado.Value = .GetValue("Estado")
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
            TabControlPrincipal.SelectedTabIndex = 1
        Else
            TabControlPrincipal.SelectedTabIndex = 1
            '  Public _modulo As SideNavItem
            '_TabControl.SelectedTab = _modulo
            '_tab.Close()
            'Me.Close()
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
        TabControlPrincipal.SelectedTabIndex = 1
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




    Private Sub Panel1_MouseHover(sender As Object, e As EventArgs) Handles btnSi.MouseHover
        btnSi.BackColor = Color.FromArgb(30, 199, 165)
    End Sub

    Private Sub Panel1_MouseLeave(sender As Object, e As EventArgs) Handles btnSi.MouseLeave
        btnSi.BackColor = Color.FromArgb(26, 179, 148)
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        TabControlPrincipal.SelectedTabIndex = 0
        btnNuevo.PerformClick()

    End Sub

    Private Sub JGrM_Buscador_KeyDown(sender As Object, e As KeyEventArgs) Handles JGrM_Buscador.KeyDown
        If (e.KeyCode = Keys.Enter) Then

            TabControlPrincipal.SelectedTabIndex = 0

        End If
    End Sub
    Private Sub JGrM_Buscador_DoubleClick(sender As Object, e As EventArgs) Handles JGrM_Buscador.DoubleClick
        If (JGrM_Buscador.Row >= 0) Then
            TabControlPrincipal.SelectedTabIndex = 0
        End If
    End Sub
    Private Sub VerToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles VerToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then

            TabControlPrincipal.SelectedTabIndex = 0
        End If
    End Sub

    Private Sub EditarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditarToolStripMenuItem.Click
        If (JGrM_Buscador.Row >= 0) Then

            TabControlPrincipal.SelectedTabIndex = 0
            btnModificar.PerformClick()

        End If
    End Sub

    Private Sub EliminarToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles EliminarToolStripMenuItem1.Click
        If (JGrM_Buscador.Row >= 0) Then

            btnEliminar.PerformClick()

        End If
    End Sub

    Private Sub tbNombreConcepto_TextChanged(sender As Object, e As EventArgs) Handles tbNombreServicio.TextChanged

    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Private Sub btnSearchProducto_Click(sender As Object, e As EventArgs) Handles btnSearchProducto.Click
        Dim dt As DataTable

        dt = ListarProductosKits()


        'ProductoId	NombreProducto	NombreCategoria	PrecioVenta	PrecioCosto

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("ProductoId,", True, "Cod Producto", 80))
        listEstCeldas.Add(New Celda("NombreProducto", True, "Producto", 300))
        listEstCeldas.Add(New Celda("NombreCategoria", True, "Categoria", 120))
        listEstCeldas.Add(New Celda("PrecioVenta", False, "Precio Venta", 90, "0.00"))
        listEstCeldas.Add(New Celda("PrecioCosto", False, "PrecioCosto", 120))
        Dim ef = New Efecto
        ef.tipo = 22
        ef.dt = dt
        ef.SeleclCol = 2
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 50
        ef.ancho = 350
        ef.Context = "Seleccione Productos".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
            tbProducto.Text = Row.Cells("NombreProducto").Value
            ProductoId = Row.Cells("ProductoId").Value
            cbNegocio.Focus()
        End If
    End Sub

    Private Sub tbPorcentajeGanancia_ValueChanged(sender As Object, e As EventArgs) Handles tbPorcentajeGanancia.ValueChanged
        If (tbPorcentajeGanancia.Focused) Then
            If (Not tbPorcentajeGanancia.Text = String.Empty And Not tbMontoServicio.Text = String.Empty) Then
                If (tbPorcentajeGanancia.Value = 0 Or tbPorcentajeGanancia.Value > 100) Then
                    tbPorcentajeGanancia.Value = 0
                    tbMontoGanancia.Value = 0



                Else

                    Dim porcdesc As Double = tbPorcentajeGanancia.Value
                    Dim montodesc As Double = tbMontoServicio.Value * (porcdesc / 100)
                    tbMontoGanancia.Value = montodesc

                End If


            End If
            If (tbPorcentajeGanancia.Text = String.Empty) Then
                tbMontoGanancia.Value = 0

            End If
        End If
    End Sub

    Private Sub tbMontoGanancia_ValueChanged(sender As Object, e As EventArgs) Handles tbMontoGanancia.ValueChanged
        If (tbMontoGanancia.Focused) Then

            Dim total As Double = tbMontoServicio.Value
            If (Not tbMontoGanancia.Text = String.Empty And Not tbMontoGanancia.Text = String.Empty) Then
                If (tbMontoGanancia.Value = 0 Or tbMontoGanancia.Value > total) Then
                    tbMontoGanancia.Value = 0
                    tbPorcentajeGanancia.Value = 0

                Else
                    Dim montodesc As Double = tbMontoGanancia.Value
                    Dim pordesc As Double = ((montodesc * 100) / tbMontoServicio.Value)
                    tbPorcentajeGanancia.Value = pordesc


                End If

            End If

            If (tbMontoGanancia.Text = String.Empty) Then
                tbMontoGanancia.Value = 0

            End If
        End If

    End Sub

    Private Sub cbNegocio_ValueChanged(sender As Object, e As EventArgs) Handles cbNegocio.ValueChanged
        If (cbNegocio.SelectedIndex <> 0) Then
            lbCategoria.Visible = False
            cbCategoria.Visible = False
        Else
            lbCategoria.Visible = True
            cbCategoria.Visible = True
        End If
    End Sub
#End Region
End Class