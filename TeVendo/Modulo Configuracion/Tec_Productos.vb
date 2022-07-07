Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls

Public Class Tec_Productos

#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SuperTabItem
    Public _TabControl As SuperTabControl
    Public FilaSeleccionada As Boolean = False


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
        With JGrM_Buscador.RootTable.Columns("imgEstado")
            .LineAlignment = TextAlignment.Center

        End With

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


        CargarIconEstado()
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
        Me.Text = "Gestion De Productos"
        P_Global._prCargarComboGenerico(cbEmpresa, L_prListaEmpresasUsuarios(), "Id", "Codigo", "Nombre", "Empresa")
        P_Global._prCargarComboGenerico(cbCategoria, L_prListaCategorias(), "Id", "Codigo", "NombreCategoria", "Categoria")

        P_Global._prCargarComboGenerico(cbProveedor, L_prListarProveedores(), "Id", "Codigo", "NombreProveedor", "Proveedor")
        P_Global._prCargarComboGenerico(cbMarca, L_prLibreriaDetalleGeneral(3), "cnnum", "Codigo", "cndesc1", "Marca")
        P_Global._prCargarComboGenerico(cbAtributo, L_prLibreriaDetalleGeneral(4), "cnnum", "Codigo", "cndesc1", "Industria")
        P_Global._prCargarComboGenerico(cbFamilia, L_prLibreriaDetalleGeneral(5), "cnnum", "Codigo", "cndesc1", "Familia")

        P_Global._prCargarComboGenerico(cbUniVenta, L_prLibreriaDetalleGeneral(6), "cnnum", "Codigo", "cndesc1", "Unidad Venta")
        P_Global._prCargarComboGenerico(cbUnidMaxima, L_prLibreriaDetalleGeneral(7), "cnnum", "Codigo", "cndesc1", "Unidad Maxima")

        _PMIniciarTodo()
        _prAsignarPermisos()



        _prEliminarContenidoImage()
        _habilitarFocus()
    End Sub

    Public Sub _habilitarFocus()
        With MHighlighterFocus
            .SetHighlightOnFocus(tbCodigo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbCodigoExterno, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbNombreProducto, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbDescripcion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbEmpresa, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbCategoria, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(swEstado, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbProveedor, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbStockMinimo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbMarca, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbAtributo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbFamilia, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbUniVenta, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(cbUnidMaxima, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbConversion, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnMarca, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnAtributo, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnFamilia, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btUniVenta, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btUniMaxima, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnGrabar, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(btnSalir, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)
            .SetHighlightOnFocus(tbCodigoBarras, DevComponents.DotNetBar.Validator.eHighlightColor.Blue)

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

    Public Sub _prCargarImagen()
        PanelListImagenes.Controls.Clear()

        pbImgProdu.Image = Nothing

        Dim i As Integer = 0
        For Each fila As DataRow In TablaImagenes.Rows
            Dim elemImg As UCLavadero = New UCLavadero
            Dim rutImg = fila.Item("lhima").ToString
            Dim estado As Integer = fila.Item("estado")

            If (estado = 0) Then
                elemImg.pbImg.SizeMode = PictureBoxSizeMode.StretchImage
                Dim bm As Bitmap = Nothing
                Dim by As Byte() = fila.Item("img")
                Dim ms As New MemoryStream(by)
                bm = New Bitmap(ms)


                elemImg.pbImg.Image = bm

                pbImgProdu.SizeMode = PictureBoxSizeMode.StretchImage
                pbImgProdu.Image = bm
                elemImg.pbImg.Tag = i
                elemImg.Dock = DockStyle.Top
                pbImgProdu.Tag = i
                AddHandler elemImg.pbImg.MouseEnter, AddressOf pbImg_MouseEnter

                PanelListImagenes.Controls.Add(elemImg)
                ms.Dispose()

            Else
                If (estado = 1) Then
                    If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos" + rutImg)) Then
                        Dim bm As Bitmap = New Bitmap(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos" + rutImg)
                        elemImg.pbImg.SizeMode = PictureBoxSizeMode.StretchImage
                        elemImg.pbImg.Image = bm
                        pbImgProdu.SizeMode = PictureBoxSizeMode.StretchImage
                        pbImgProdu.Image = bm
                        elemImg.pbImg.Tag = i
                        elemImg.Dock = DockStyle.Top
                        pbImgProdu.Tag = i
                        AddHandler elemImg.pbImg.MouseEnter, AddressOf pbImg_MouseEnter

                        PanelListImagenes.Controls.Add(elemImg)
                    End If

                End If
            End If




            i += 1
        Next

    End Sub
    Private Sub pbImg_MouseEnter(sender As Object, e As EventArgs)
        Dim pb As PictureBox = CType(sender, PictureBox)
        pbImgProdu.Image = pb.Image
        pbImgProdu.Tag = pb.Tag

    End Sub

    Private Function _fnCopiarImagenRutaDefinida() As String
        'copio la imagen en la carpeta del sistema

        Dim file As New OpenFileDialog()
        'file.InitialDirectory = gs_RutaImg
        file.Filter = "Ficheros JPG o JPEG o PNG|*.jpg;*.jpeg;*.png" &
                      "|Ficheros GIF|*.gif" &
                      "|Ficheros BMP|*.bmp" &
                      "|Ficheros PNG|*.png" &
                      "|Ficheros TIFF|*.tif"
        If file.ShowDialog() = DialogResult.OK Then
            Dim ruta As String = file.FileName
            Dim nombre As String = ""

            If file.CheckFileExists = True Then
                Dim img As New Bitmap(New Bitmap(ruta), 1000, 800)
                Dim a As Object = file.GetType.ToString

                Dim da As String = Str(Now.Day).Trim + Str(Now.Month).Trim + Str(Now.Year).Trim + Str(Now.Hour).Trim + Str(Now.Minute).Trim + Str(Now.Second).Trim

                nombre = "\Imagen_" + da + ".jpg".Trim


                If (_fnActionNuevo()) Then
                    Dim mstream = New MemoryStream()

                    img.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)

                    TablaImagenes.Rows.Add(0, 0, nombre, mstream.ToArray(), 0)
                    mstream.Dispose()
                    img.Dispose()

                Else
                    Dim mstream = New MemoryStream()

                    img.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)
                    TablaImagenes.Rows.Add(0, tbCodigo.Text, nombre, mstream.ToArray(), 0)
                    mstream.Dispose()

                End If

                'img.Save(RutaTemporal + nombre, System.Drawing.Imaging.ImageFormat.Jpeg)




            End If
            Return nombre
        End If

        Return "default.jpg"
    End Function

    Public Sub _prGuardarImagenes(_ruta As String)
        PanelListImagenes.Controls.Clear()


        For i As Integer = 0 To TablaImagenes.Rows.Count - 1 Step 1
            Dim estado As Integer = TablaImagenes.Rows(i).Item("estado")
            If (estado = 0) Then

                Dim bm As Bitmap = Nothing
                Dim by As Byte() = TablaImagenes.Rows(i).Item("img")
                Dim ms As New MemoryStream(by)
                bm = New Bitmap(ms)
                Try
                    bm.Save(_ruta + TablaImagenes.Rows(i).Item("lhima"), System.Drawing.Imaging.ImageFormat.Jpeg)
                Catch ex As Exception

                End Try




            End If
            If (estado = -1) Then
                Try
                    Me.pbImgProdu.Image.Dispose()
                    Me.pbImgProdu.Image = Nothing
                    Application.DoEvents()
                    TablaImagenes.Rows(i).Item("img") = Nothing



                    If (File.Exists(_ruta + TablaImagenes.Rows(i).Item("lhima"))) Then
                        My.Computer.FileSystem.DeleteFile(_ruta + TablaImagenes.Rows(i).Item("lhima"))
                    End If

                Catch ex As Exception

                End Try
            End If
        Next
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
        tbCodigoBarras.ReadOnly = False

        cbFamilia.ReadOnly = False
        cbUniVenta.ReadOnly = False
        cbUnidMaxima.ReadOnly = False
        tbConversion.IsInputReadOnly = False
        btnDelete.Visible = True
        btnImagen.Visible = True

        btnMarca.Visible = True
        btnAtributo.Visible = True
        btnFamilia.Visible = True
        btUniVenta.Visible = True
        btUniMaxima.Visible = True

        btnAgregarCategoria.Visible = True
        btnAgregarProveedor.Visible = True
    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreProducto.ReadOnly = True
        tbCodigoExterno.ReadOnly = True
        tbDescripcion.ReadOnly = True
        tbStockMinimo.IsInputReadOnly = True


        tbCodigoBarras.ReadOnly = True

        swEstado.IsReadOnly = True
        cbEmpresa.ReadOnly = True
        cbCategoria.ReadOnly = True
        cbProveedor.ReadOnly = True
        cbMarca.ReadOnly = True
        cbAtributo.ReadOnly = True
        cbFamilia.ReadOnly = True
        cbUniVenta.ReadOnly = True
        cbUnidMaxima.ReadOnly = True
        btnDelete.Visible = False
        btnImagen.Visible = False
        tbConversion.IsInputReadOnly = True

        btnMarca.Visible = False
        btnAtributo.Visible = False
        btnFamilia.Visible = False
        btUniVenta.Visible = False
        btUniMaxima.Visible = False
        btnAgregarCategoria.Visible = False
        btnAgregarProveedor.Visible = False
    End Sub
    Private Sub _prCargarDetallePrecios(ProductoId As String)
        Dim dt As New DataTable
        dt = ListPreciosDetalles(ProductoId)



        If (IsNothing(grPrecios.DataSource)) Then
            grPrecios.DataSource = dt
            grPrecios.RetrieveStructure()
            grPrecios.AlternatingColors = True
            'id  Descripcion	precio
            With grPrecios.RootTable.Columns("id")
                .Width = 100
                .Caption = "CODIGO"
                .Visible = False

            End With



            With grPrecios.RootTable.Columns("Descripcion")
                .Width = 350
                .Caption = "Categoria Precio"
                .Visible = True
            End With

            With grPrecios.RootTable.Columns("precio")
                .Width = 50
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
                .Visible = True
                .FormatString = "0.00"
                .Caption = "precio".ToUpper
            End With


            With grPrecios
                .GroupByBoxVisible = False
                'diseño de la grilla
                .VisualStyle = VisualStyle.Office2007
                .BoundMode = Janus.Data.BoundMode.Bound
                .RowHeaders = InheritableBoolean.True

                .TotalRowPosition = TotalRowPosition.BottomFixed
            End With
        Else
            grPrecios.DataSource = dt

        End If



    End Sub
    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbNombreProducto.Text = ""
        tbDescripcion.Text = ""
        tbCodigoExterno.Text = ""
        tbCodigoBarras.Text = ""
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
        TablaImagenes = L_prCargarImagenesRecepcion(-1)

        _prCargarDetallePrecios(-1)
        _prCargarImagen()
        _prEliminarContenidoImage()
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
            res = L_prProductoInsertar(tbCodigo.Text, tbCodigoExterno.Text, tbCodigoBarras.Text, tbNombreProducto.Text,
                                                 tbDescripcion.Text, tbStockMinimo.Value, IIf(swEstado.Value = True, 1, 0),
                                                 cbCategoria.Value, cbEmpresa.Value, cbProveedor.Value, cbMarca.Value,
                                                 cbAtributo.Value, cbFamilia.Value, cbUniVenta.Value, cbUnidMaxima.Value, tbConversion.Value, TablaImagenes, CType(grPrecios.DataSource, DataTable))

            If res Then

                _prCrearCarpetaImagenes("ProductosTodos")
                _prGuardarImagenes(RutaGlobal + "\Imagenes\Imagenes Productos\" + "ProductosTodos" + "\")

                ToastNotification.Show(Me, "Codigo de Producto ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar el producto".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el producto".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try
        _prEliminarContenidoImage()
        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = L_prProductoModificar(tbCodigo.Text, tbCodigoExterno.Text, tbCodigoBarras.Text, tbNombreProducto.Text,
                                                tbDescripcion.Text, tbStockMinimo.Value, IIf(swEstado.Value = True, 1, 0),
                                                cbCategoria.Value, cbEmpresa.Value, cbProveedor.Value, cbMarca.Value,
                                                cbAtributo.Value, cbFamilia.Value, cbUniVenta.Value, cbUnidMaxima.Value, tbConversion.Value, TablaImagenes, CType(grPrecios.DataSource, DataTable))


            If Res Then
                _prCrearCarpetaImagenes("ProductosTodos")
                _prGuardarImagenes(RutaGlobal + "\Imagenes\Imagenes Productos\" + "ProductosTodos" + "\")

                ToastNotification.Show(Me, "Codigo de Producto ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el producto".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar el producto".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try
        _prEliminarContenidoImage()
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
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el producto".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

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
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If

        If (cbEmpresa.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbEmpresa.Focus()
            Return _ok
        End If
        If (cbCategoria.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbCategoria.Focus()
            Return _ok
        End If
        If (tbStockMinimo.Text.Length <= 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbStockMinimo.Focus()
            Return _ok
        End If

        If (cbProveedor.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbProveedor.Focus()
            Return _ok
        End If
        If (cbMarca.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbMarca.Focus()
            Return _ok
        End If
        If (cbAtributo.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbAtributo.Focus()
            Return _ok
        End If
        If (tbConversion.Text.Length <= 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            tbConversion.Focus()
            Return _ok
        End If
        If (tbCodigoExterno.Text.Trim <> String.Empty) Then

            Dim dt As DataTable = ObtenerCoincidenciasCodigoExterno(tbCodigoExterno.Text.Trim)

            If _MNuevo Then
                If (dt.Rows.Count > 0) Then ''Quiere decir que ya existe un producto con ese codigo
                    Mensaje = Mensaje + Chr(13) + Chr(10) + " Ya existe un Producto con un mismo codigo Externo= " + tbCodigoExterno.Text + " En el Producto " + Str(dt.Rows(0).Item("id")) + "  " + dt.Rows(0).Item("DescripcionProducto")

                    _ok = False

                    ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    tbCodigoExterno.Focus()
                    Return _ok
                End If

            Else
                If (dt.Rows.Count > 1) Then ''Quiere decir que ya existe un producto con ese codigo
                    Mensaje = Mensaje + Chr(13) + Chr(10) + " Ya existe un Producto con un mismo codigo Externo"
                    _ok = False

                    ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    tbCodigoExterno.Focus()
                    Return _ok
                End If
            End If

        End If

        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_Productos")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'Select Case a.Id ,a.CodigoExterno ,a.CodigoBarras ,a.NombreProducto ,a.DescripcionProducto 
        ',a.StockMinimo ,a.Estado ,
        '    a.CategoriaId , cat.NombreCategoria, a.EmpresaId, em.Nombre As Empresa, a.ProveedorId, 
        'a.MarcaId,
        '    a.AttributoId, a.FamiliaId, a.UnidadVentaId, a.UnidadMaximaId, a.Conversion  
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("CodigoExterno", False))
        listEstCeldas.Add(New Celda("CodigoBarras", False))
        listEstCeldas.Add(New Celda("NombreProducto", True, " NombreProducto", 200))
        listEstCeldas.Add(New Celda("DescripcionProducto", True, " Descripcion Producto", 100))
        listEstCeldas.Add(New Celda("StockMinimo", True, "Stock Minimo", 90, "0.00"))
        listEstCeldas.Add(New Celda("estado", False, "Estado", 70))
        listEstCeldas.Add(New Celda("imgEstado", True, "Estado", 150))
        listEstCeldas.Add(New Celda("CategoriaId", False))
        listEstCeldas.Add(New Celda("NombreCategoria", True, "Categoria", 80))
        listEstCeldas.Add(New Celda("EmpresaId", False))
        listEstCeldas.Add(New Celda("Empresa", True, "Empresa", 80))

        listEstCeldas.Add(New Celda("ProveedorId", False))
        listEstCeldas.Add(New Celda("MarcaId", False))
        listEstCeldas.Add(New Celda("AttributoId", False))
        listEstCeldas.Add(New Celda("FamiliaId", False))
        listEstCeldas.Add(New Celda("UnidadVentaId", False))
        listEstCeldas.Add(New Celda("UnidadMaximaId", False))
        listEstCeldas.Add(New Celda("Conversion", False))


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
            tbCodigoBarras.Text = .GetValue("CodigoBarras")
            cbUnidMaxima.Value = .GetValue("UnidadMaximaId")
            tbConversion.Value = .GetValue("Conversion")

            _prCargarDetallePrecios(tbCodigo.Text)
        End With
        TablaImagenes = L_prCargarImagenesRecepcion(tbCodigo.Text)
        _prCargarImagen()
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






    Private Sub btnMarca_Click(sender As Object, e As EventArgs) Handles btnMarca.Click


        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 3
        ef.titulo = "Crear Nueva Marca"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbMarca, L_prLibreriaDetalleGeneral(3), "cnnum", "Codigo", "cndesc1", "Marca")
            cbMarca.SelectedIndex = CType(cbMarca.DataSource, DataTable).Rows.Count - 1
            cbMarca.Focus()
        End If


    End Sub

    Private Sub btnAtributo_Click(sender As Object, e As EventArgs) Handles btnAtributo.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 4
        ef.titulo = "Crear Nueva Industria"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbAtributo, L_prLibreriaDetalleGeneral(4), "cnnum", "Codigo", "cndesc1", "Industria")
            cbAtributo.SelectedIndex = CType(cbAtributo.DataSource, DataTable).Rows.Count - 1
            cbAtributo.Focus()
        End If


    End Sub

    Private Sub btnFamilia_Click(sender As Object, e As EventArgs) Handles btnFamilia.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 5
        ef.titulo = "Crear Nueva Familia"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbFamilia, L_prLibreriaDetalleGeneral(5), "cnnum", "Codigo", "cndesc1", "Familia")
            cbFamilia.SelectedIndex = CType(cbFamilia.DataSource, DataTable).Rows.Count - 1
            cbFamilia.Focus()
        End If



    End Sub

    Private Sub btUniVenta_Click(sender As Object, e As EventArgs) Handles btUniVenta.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 6
        ef.titulo = "Crear Nueva Unidad Venta Minima"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbUniVenta, L_prLibreriaDetalleGeneral(6), "cnnum", "Codigo", "cndesc1", "Uni Minima")
            cbUniVenta.SelectedIndex = CType(cbUniVenta.DataSource, DataTable).Rows.Count - 1
            cbUniVenta.Focus()
        End If


    End Sub

    Private Sub btUniMaxima_Click(sender As Object, e As EventArgs) Handles btUniMaxima.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 10
        ef.ModuloLibreria = 7
        ef.titulo = "Crear Nueva Unidad Maxima"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbUnidMaxima, L_prLibreriaDetalleGeneral(7), "cnnum", "Codigo", "cndesc1", "Uni Maxima")
            cbUnidMaxima.SelectedIndex = CType(cbUnidMaxima.DataSource, DataTable).Rows.Count - 1
            cbUnidMaxima.Focus()
        End If


    End Sub

    Private Sub btnImagen_Click(sender As Object, e As EventArgs) Handles btnImagen.Click
        _fnCopiarImagenRutaDefinida()
        _prCargarImagen()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim pos As Integer = CType(pbImgProdu.Tag, Integer)
        If (IsDBNull(TablaImagenes)) Then
            Return

        End If
        If (pos >= 0 And TablaImagenes.Rows.Count > 0) Then
            TablaImagenes.Rows(pos).Item("estado") = -1
            _prCargarImagen()
        End If
    End Sub
    Public Sub _prEliminarDirectorio(numi As String)

        '_prGuardarImagenes(RutaGlobal + "\Imagenes\Imagenes RecepcionVehiculo\" + "Recepcion_" + tbNumeroOrden.Text.Trim + "\")
        If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos")) Then

            Try
                My.Computer.FileSystem.DeleteDirectory(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos", FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
            Catch ex As Exception

            End Try


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

    Private Sub LabelX5_Click(sender As Object, e As EventArgs) Handles LabelX5.Click

    End Sub

    Private Sub ButtonX2_Click(sender As Object, e As EventArgs) Handles ButtonX2.Click
        _TabControl.SelectedTab = _modulo
        _tab.Close()
        Me.Close()
    End Sub

    Private Sub cbMarca_ValueChanged(sender As Object, e As EventArgs) Handles cbMarca.ValueChanged

    End Sub

    Private Sub btnAgregarCategoria_Click(sender As Object, e As EventArgs) Handles btnAgregarCategoria.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 18
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbCategoria, L_prListaCategorias(), "Id", "Codigo", "NombreCategoria", "Categoria")
            cbCategoria.Value = ef.CategoriaId
            cbCategoria.Focus()
        End If

    End Sub

    Private Sub btnAgregarProveedor_Click(sender As Object, e As EventArgs) Handles btnAgregarProveedor.Click
        Dim numi As String = ""
        Dim ef = New Efecto
        ef.tipo = 19
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_Global._prCargarComboGenerico(cbProveedor, L_prListarProveedores(), "Id", "Codigo", "NombreProveedor", "Proveedor")
            cbProveedor.Value = ef.ProveedorId
            cbProveedor.Focus()
        End If
    End Sub

    Private Sub btnReporteConImagenes_Click(sender As Object, e As EventArgs) Handles btnReporteConImagenes.Click
        Dim dt As DataTable


        dt = L_fnListarCategoriaProductos()
        'a.Id ,a.NombreCliente  as NombreProveedor ,a.DireccionCliente  ,a.Telefono

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("id,", False, "ID", 50))
        listEstCeldas.Add(New Celda("Nombre", True, "CATEGORIA PRECIO", 350))
        listEstCeldas.Add(New Celda("seleccionado", True, "SELECCIONAR", 150))



        Dim ef = New Efecto
        ef.tipo = 23
        ef.dt = dt
        ef.SeleclCol = 0
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 80
        ef.ancho = 800
        ef.Context = "Seleccione Categoria Precio".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            'Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

            Dim CategoriaPrecio As Integer = ef.CategoriaPrecioSelected
            Dim SucursalSelected As Integer = ef.SucursalSelected
            dt = ef.TableCategoria




            GenerarReporte(1, 1, CategoriaPrecio, SucursalSelected, dt)

        End If


    End Sub

    Public Function ExisteCategoria(IdCategoria As Integer, dtCategoria As DataTable)


        For i As Integer = 0 To dtCategoria.Rows.Count - 1 Step 1

            If (dtCategoria.Rows(i).Item("seleccionado") = True And dtCategoria.Rows(i).Item("id") = IdCategoria) Then
                Return True
            End If


        Next
        Return False

    End Function

    Public Sub GenerarReporte(tipo As Integer, PrecioVenta As Integer, CategoriaPrecio As Integer, Sucursal As Integer, dtCategorias As DataTable)
        Dim ancho As Integer = 200
        Dim alto As Integer = 150

        Dim Bin As New MemoryStream
        Dim dt As DataTable
        Dim titulo As String
        If (PrecioVenta = 1) Then
            dt = GenerarReportePrecios(CategoriaPrecio, Sucursal)

            Dim dt2 As DataTable = dt.Copy
            dt2.Rows.Clear()

            For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                If (ExisteCategoria(dt.Rows(i).Item("CategoriaId"), dtCategorias)) Then

                    dt2.ImportRow(dt.Rows(i))
                End If

            Next
            dt = dt2


            titulo = "LISTADO DE PRECIOS DE PRODUCTOS"
        Else
            dt = GenerarReportePrecios(CategoriaPrecio, Sucursal)
            titulo = "LISTADO DE PRECIOS DE PRODUCTOS"
            Dim dt2 As DataTable = dt.Copy
            dt2.Rows.Clear()

            For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                If (ExisteCategoria(dt.Rows(i).Item("CategoriaId"), dtCategorias)) Then

                    dt2.ImportRow(dt.Rows(i))
                End If

            Next
            dt = dt2

        End If

        Dim dtImage As DataTable = ObtenerImagenEmpresa()
        Dim NombreEmpresa As String = dtImage.Rows(0).Item("Nombre")
        Dim Direccion As String = dtImage.Rows(0).Item("Direccion")
        If (dtImage.Rows.Count > 0) Then
            Dim RutaGlobal As String = gs_CarpetaRaiz
            Dim Name As String = dtImage.Rows(0).Item(0)
            If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name)) Then

                Dim im As New Bitmap(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name)

                Dim binEmpresa As MemoryStream
                binEmpresa = New MemoryStream
                Dim img As New Bitmap(im)
                img.Save(binEmpresa, Imaging.ImageFormat.Png)


                For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                    dt.Rows(i).Item("imgEmpresa") = binEmpresa.GetBuffer


                    If (tipo = 1) Then  '' Reporte con Imagenes
                        Dim Rutaimagen As String = dt.Rows(i).Item("Rutaimg")
                        If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos" + Rutaimagen)) Then
                            Dim bm As Bitmap = New Bitmap(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos" + Rutaimagen)
                            Bin = New MemoryStream
                            Dim img02 As New Bitmap(bm, ancho, alto)
                            img02.Save(Bin, Imaging.ImageFormat.Png)
                            dt.Rows(i).Item("img") = Bin.GetBuffer
                            Bin.Dispose()
                            Bin.Close()

                            img02.Dispose()



                        Else
                            Dim bm As Bitmap = New Bitmap(My.Resources.noimage, ancho, alto)
                            Bin = New MemoryStream
                            Dim img02 As New Bitmap(bm)
                            img02.Save(Bin, Imaging.ImageFormat.Png)

                            dt.Rows(i).Item("img") = Bin.GetBuffer
                            Bin.Dispose()
                            Bin.Close()

                            img02.Dispose()
                        End If
                    End If


                Next
                binEmpresa.Dispose()
                binEmpresa.Close()
                img.Dispose()




            Else
                If (tipo = 1) Then
                    For i As Integer = 0 To dt.Rows.Count - 1 Step 1



                        Dim Rutaimagen As String = dt.Rows(i).Item("Rutaimg")
                        If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos" + Rutaimagen)) Then
                            Dim bm As Bitmap = New Bitmap(RutaGlobal + "\Imagenes\Imagenes Productos\ProductosTodos" + Rutaimagen, ancho, alto)
                            Bin = New MemoryStream
                            Dim img02 As New Bitmap(bm)
                            img02.Save(Bin, Imaging.ImageFormat.Png)


                            dt.Rows(i).Item("img") = Bin.GetBuffer
                            Bin.Dispose()
                            Bin.Close()

                            img02.Dispose()
                        Else
                            Dim bm As Bitmap = New Bitmap(My.Resources.noimage, ancho, alto)
                            Bin = New MemoryStream
                            Dim img02 As New Bitmap(bm)
                            img02.Save(Bin, Imaging.ImageFormat.Png)


                            dt.Rows(i).Item("img") = Bin.GetBuffer
                            Bin.Dispose()
                            Bin.Close()

                            img02.Dispose()
                        End If

                    Next
                End If

            End If


        End If
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If



        P_Global.Visualizador = New Visualizador

        If (tipo = 1) Then
            Dim objrep As New Reporte_ProductoImagenes



            objrep.SetDataSource(dt)
            objrep.SetParameterValue("NombreEmpresa", NombreEmpresa)
            objrep.SetParameterValue("Ciudad", Direccion)

            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.CrGeneral.Zoom(90)
            P_Global.Visualizador.Show() 'Comentar
            ''P_Global.Visualizador.BringToFront() 'Comentar

        Else
            Dim objrep As New Reporte_ProductosPrecios



            objrep.SetDataSource(dt)
            objrep.SetParameterValue("NombreEmpresa", NombreEmpresa)
            objrep.SetParameterValue("Ciudad", Direccion)
            objrep.SetParameterValue("Titulo", titulo)
            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.CrGeneral.Zoom(90)
            P_Global.Visualizador.Show() 'Comentar
            ''P_Global.Visualizador.BringToFront() 'Comentar
        End If

    End Sub



    Private Sub ButtonX4_Click(sender As Object, e As EventArgs) Handles ButtonX4.Click
        Dim dt As DataTable


        dt = L_fnListarCategoriaProductos()
        'a.Id ,a.NombreCliente  as NombreProveedor ,a.DireccionCliente  ,a.Telefono

        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("id,", False, "ID", 50))
        listEstCeldas.Add(New Celda("Nombre", True, "CATEGORIA PRECIO", 350))
        listEstCeldas.Add(New Celda("seleccionado", True, "SELECCIONAR", 150))


        Dim ef = New Efecto
        ef.tipo = 23
        ef.dt = dt
        ef.SeleclCol = 0
        ef.listEstCeldasNew = listEstCeldas
        ef.alto = 80
        ef.ancho = 800
        ef.Context = "Seleccione Datos".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then

            Dim CategoriaPrecio As Integer = ef.CategoriaPrecioSelected
            Dim SucursalSelected As Integer = ef.SucursalSelected
            dt = ef.TableCategoria




            GenerarReporte(0, 0, CategoriaPrecio, SucursalSelected, dt)



        End If


    End Sub

    Private Sub grPrecios_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grPrecios.EditingCell
        If (tbNombreProducto.ReadOnly = False) Then
            If (e.Column.Index = grPrecios.RootTable.Columns("precio").Index) Then
                e.Cancel = False
                Return

            End If
        Else
            e.Cancel = True
            Return

        End If
        e.Cancel = True
    End Sub

    Private Sub grPrecios_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grPrecios.CellEdited
        If (e.Column.Index = grPrecios.RootTable.Columns("precio").Index) Then
            If (Not IsNumeric(grPrecios.GetValue("precio")) Or grPrecios.GetValue("precio").ToString = String.Empty) Then

                grPrecios.SetValue("precio", 0)
            Else
                If (grPrecios.GetValue("precio") > 0) Then


                Else

                    grPrecios.SetValue("Cantidad", 0)

                End If
            End If
        End If
    End Sub
#End Region
End Class