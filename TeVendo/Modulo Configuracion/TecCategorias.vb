
Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class TecCategorias

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
        tbNombreCategoria.Focus()


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
        Me.Text = "Gestion De Categorias"
        _PMIniciarTodo()
        _prAsignarPermisos()

        P_Global._prCargarComboGenerico(cbEmpresa, L_prListaEmpresasUsuarios(), "Id", "Codigo", "Nombre", "Empresa")
        Dim blah As New Bitmap(New Bitmap(My.Resources.ic_c), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico

    End Sub

    Private Sub _prCrearCarpetaTemporal()

        If System.IO.Directory.Exists(RutaTemporal) = False Then
            System.IO.Directory.CreateDirectory(RutaTemporal)
        Else
            Try
                My.Computer.FileSystem.DeleteDirectory(RutaTemporal, FileIO.DeleteDirectoryOption.DeleteAllContents)
                My.Computer.FileSystem.CreateDirectory(RutaTemporal)
                'My.Computer.FileSystem.DeleteDirectory(RutaTemporal, FileIO.UIOption.AllDialogs, FileIO.RecycleOption.SendToRecycleBin)
                'System.IO.Directory.CreateDirectory(RutaTemporal)

            Catch ex As Exception

            End Try

        End If

    End Sub
    Private Sub _prCrearCarpetaImagenes()
        Dim rutaDestino As String = RutaGlobal + "\Imagenes\Imagenes Categoria\"

        If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Categoria\") = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Imagenes") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes")
                If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Categoria") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Categoria")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Imagenes\Imagenes Categoria") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Imagenes\Imagenes Categoria")

                End If
            End If
        End If
    End Sub


    Private Sub _fnMoverImagenRuta(Folder As String, name As String)
        'copio la imagen en la carpeta del sistema
        If (Not name.Equals("Default.jpg") And File.Exists(RutaTemporal + name)) Then

            Dim img As New Bitmap(New Bitmap(RutaTemporal + name), 200, 120)

            UsImg.Image.Dispose()
            UsImg.Image = Nothing
            Try
                My.Computer.FileSystem.CopyFile(RutaTemporal + name,
     Folder + name, overwrite:=True)

            Catch ex As System.IO.IOException
                MsgBox("Error: " + ex.Message)

            End Try



        End If
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

        tbNombreCategoria.ReadOnly = False
        tbDescripcion.ReadOnly = False
        swEstado.IsReadOnly = False
        btnImage.Visible = True
        swApp.IsReadOnly = False
        cbEmpresa.ReadOnly = False
        _prCrearCarpetaImagenes()
        _prCrearCarpetaTemporal()
    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreCategoria.ReadOnly = True
        tbDescripcion.ReadOnly = True
        swEstado.IsReadOnly = True
        btnImage.Visible = False
        swApp.IsReadOnly = True
        cbEmpresa.ReadOnly = True
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbNombreCategoria.Text = ""
        tbDescripcion.Text = ""
        swApp.Value = True
        swEstado.Value = True
        UsImg.Image = My.Resources.pantalla
        tbNombreCategoria.Focus()
        If (CType(cbEmpresa.DataSource, DataTable).Rows.Count > 0) Then
            cbEmpresa.SelectedIndex = 0
        End If

    End Sub


    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbNombreCategoria.BackColor = Color.White
        tbDescripcion.BackColor = Color.White
        cbEmpresa.BackColor = Color.White
    End Sub

    Public Function _PMOGrabarRegistro() As Boolean

        Dim res As Boolean = L_prCategoriaInsertar(tbCodigo.Text, tbNombreCategoria.Text, tbDescripcion.Text,
                                                   IIf(swEstado.Value = True, 1, 0), nameImg, IIf(swApp.Value = True, 1, 0), cbEmpresa.Value)

        If res Then

            Modificado = False
            _fnMoverImagenRuta(RutaGlobal + "\Imagenes\Imagenes Categoria", nameImg)
            nameImg = "Default.jpg"
            ToastNotification.Show(Me, "Codigo de Usuario ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
        End If
        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Dim nameImage As String = JGrM_Buscador.GetValue("Imagen")
        If (Modificado = False) Then
            Res = L_prCategoriaModificar(tbCodigo.Text, tbNombreCategoria.Text, tbDescripcion.Text,
                                                   IIf(swEstado.Value = True, 1, 0), nameImage, IIf(swApp.Value = True, 1, 0), cbEmpresa.Value)
        Else
            Res = L_prCategoriaModificar(tbCodigo.Text, tbNombreCategoria.Text, tbDescripcion.Text,
                                                   IIf(swEstado.Value = True, 1, 0), nameImg, IIf(swApp.Value = True, 1, 0), cbEmpresa.Value)
        End If

        If Res Then
            If (Modificado = True) Then
                _fnMoverImagenRuta(RutaGlobal + "\Imagenes\Imagenes Categoria", nameImg)
                Modificado = False
            End If
            nameImg = "Default.jpg"
            ToastNotification.Show(Me, "Codigo de Usuario ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
            _PSalirRegistro()
        End If
        Return res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbDescripcion.ReadOnly = False
    End Function
    Public Sub _PrEliminarImage()

        If (Not (_fnActionNuevo()) And (File.Exists(RutaGlobal + "\Imagenes\Imagenes Categoria" + nameImg))) Then
            UsImg.Image.Dispose()
            UsImg.Image = Nothing
            Try
                My.Computer.FileSystem.DeleteFile(RutaGlobal + "\Imagenes\Imagenes Categoria" + nameImg)
            Catch ex As Exception

            End Try


        End If
    End Sub

    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar la Categoria " + tbNombreCategoria.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_prCategoriaBorrar(tbCodigo.Text, mensajeError)
            If res Then
                _PrEliminarImage()
                ToastNotification.Show(Me, "Codigo de Categoria ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PMFiltrar()
            Else
                ToastNotification.Show(Me, mensajeError, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            End If
        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "
        If tbNombreCategoria.Text = String.Empty Then
            tbNombreCategoria.BackColor = Color.Red
            MEP.SetError(tbNombreCategoria, "Ingrese Nombre de Usuario")
            Mensaje = Mensaje + " Nombre Usuarios"
            _ok = False
        Else
            tbNombreCategoria.BackColor = Color.White
            MEP.SetError(tbNombreCategoria, "")
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



        MHighlighterFocus.UpdateHighlights()

        If tbNombreCategoria.Text = String.Empty Then
            tbNombreCategoria.Focus()
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If

        If (cbEmpresa.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, My.Resources.WARNING, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbEmpresa.Focus()
            Return _ok
        End If


        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prCategoriaGeneral()
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Modelo.Celda)

        'a.Id , a.NombreCategoria, a.DescripcionCategoria, cast(a.Estado As bit) As Estado 
        '    , a.Imagen, cast(a.VisibleApp As bit) as App  
        Dim listEstCeldas As New List(Of Modelo.Celda)
        listEstCeldas.Add(New Modelo.Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Modelo.Celda("NombreCategoria", True, "Categoria", 90))
        listEstCeldas.Add(New Modelo.Celda("Estado", True, "Estado", 70))
        listEstCeldas.Add(New Modelo.Celda("DescripcionCategoria", True, "Descripcion", 150))
        listEstCeldas.Add(New Modelo.Celda("Imagen", False))
        listEstCeldas.Add(New Modelo.Celda("EmpresaId", False))

        listEstCeldas.Add(New Modelo.Celda("App", True, "App", 90))
        listEstCeldas.Add(New Modelo.Celda("Empresa", True, "Empresa", 100))


        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        'a.Id , a.NombreCategoria, a.DescripcionCategoria, cast(a.Estado As bit) As Estado 
        '    , a.Imagen, cast(a.VisibleApp As bit) as App  
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbNombreCategoria.Text = .GetValue("NombreCategoria").ToString
            tbDescripcion.Text = .GetValue("DescripcionCategoria").ToString
            swEstado.Value = .GetValue("Estado")
            swApp.Value = .GetValue("App")
            cbEmpresa.Value = .GetValue("Empresa")


        End With
        Dim Img As String = JGrM_Buscador.GetValue("Imagen")
        nameImg = Img
        Dim ExisteImagen As Boolean = File.Exists(RutaGlobal + "\Imagenes\Imagenes Categoria" + Img)

        If Img.Equals("Default.jpg") Or Not ExisteImagen Then

            Dim im As New Bitmap(My.Resources.pantalla)
            UsImg.Image = im
        Else
            If (ExisteImagen) Then
                UsImg.SizeMode = PictureBoxSizeMode.StretchImage
                UsImg.Image = Image.FromFile(RutaGlobal + "\Imagenes\Imagenes Categoria" + Img)

            End If
        End If

        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub
    Private Function _fnCopiarImagenRutaDefinida() As String
        'copio la imagen en la carpeta del sistema

        Dim file As New OpenFileDialog()
        file.Filter = "Ficheros JPG o JPEG o PNG|*.jpg;*.jpeg;*.png" &
                      "|Ficheros GIF|*.gif" &
                      "|Ficheros BMP|*.bmp" &
                      "|Ficheros PNG|*.png" &
                      "|Ficheros TIFF|*.tif"
        If file.ShowDialog() = DialogResult.OK Then
            Dim ruta As String = file.FileName


            If file.CheckFileExists = True Then
                Dim img As New Bitmap(New Bitmap(ruta))

                Dim a As Object = file.GetType.ToString
                If (_fnActionNuevo()) Then

                    Dim mayor As Integer
                    mayor = JGrM_Buscador.GetTotal(JGrM_Buscador.RootTable.Columns("Id"), AggregateFunction.Max)
                    nameImg = "\Imagen_" + P_fnObtenerID() + ".jpg"
                    UsImg.SizeMode = PictureBoxSizeMode.StretchImage

                    UsImg.Image = Image.FromFile(ruta)
                    img.Save(RutaTemporal + nameImg, System.Drawing.Imaging.ImageFormat.Jpeg)
                    img.Dispose()
                Else

                    nameImg = "\Imagen_" + P_fnObtenerID() + ".jpg"
                    UsImg.Image = Image.FromFile(ruta)
                    img.Save(RutaTemporal + nameImg, System.Drawing.Imaging.ImageFormat.Jpeg)
                    Modificado = True
                    img.Dispose()

                End If
            End If

            Return nameImg
        End If

        Return "default.jpg"
    End Function
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

    Private Sub btnImage_Click(sender As Object, e As EventArgs) Handles btnImage.Click
        _fnCopiarImagenRutaDefinida()
        btnGrabar.Focus()
    End Sub

    Private Sub JGrM_Buscador_SelectionChanged(sender As Object, e As EventArgs) Handles JGrM_Buscador.SelectionChanged
        If (JGrM_Buscador.Row >= 0 And FilaSeleccionada = False) Then
            _MPos = JGrM_Buscador.Row

            _PMOMostrarRegistro(_MPos, True)

        End If
    End Sub

#End Region

End Class