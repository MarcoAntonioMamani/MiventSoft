
Imports Negocio.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports DevComponents.DotNetBar.Controls
Public Class Tec_ConceptosFijos
#Region "Atributos"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
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
        tbNombreConcepto.Focus()


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
        Me.Text = "Conceptos Fijos"

        _prCargarComboLibreria(cbOperacion)



        _PMIniciarTodo()
        _prAsignarPermisos()



        _prEliminarContenidoImage()
    End Sub
    Private Sub _prCargarComboLibreria(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_prListarOperacionesConceptosFijos()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("Id").Width = 70
            .DropDownList.Columns("Id").Caption = "COD"
            .DropDownList.Columns.Add("NombreOperacion").Width = 200
            .DropDownList.Columns("NombreOperacion").Caption = "Operaciones"
            .ValueMember = "Id"
            .DisplayMember = "NombreOperacion"
            .DataSource = dt
            .Refresh()
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
        tbNombreConcepto.ReadOnly = False
        tbPorcentaje.IsInputReadOnly = False
        cbOperacion.ReadOnly = False
        swEstado.IsReadOnly = False
        swTipoConcepto.IsReadOnly = False
    End Sub

    Public Sub _PMOInhabilitar()
        tbCodigo.ReadOnly = True
        tbNombreConcepto.ReadOnly = True
        tbPorcentaje.IsInputReadOnly = True
        cbOperacion.ReadOnly = True
        swEstado.IsReadOnly = True
        swTipoConcepto.IsReadOnly = True
    End Sub

    Public Sub _PMOLimpiar()
        tbCodigo.Text = ""
        tbNombreConcepto.Text = ""
        tbPorcentaje.Value = 0

        swEstado.Value = True
        seleccionarPrimerItemCombo(cbOperacion)

        tbNombreConcepto.Focus()
        swTipoConcepto.Value = True
    End Sub
    Public Sub seleccionarPrimerItemCombo(cb As EditControls.MultiColumnCombo)
        If (CType(cb.DataSource, DataTable).Rows.Count > 0) Then
            cb.SelectedIndex = 0
        End If

    End Sub

    Public Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbNombreConcepto.BackColor = Color.White
        cbOperacion.BackColor = Color.White
        tbPorcentaje.BackColor = Color.White


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
            res = L_prConceptoFijoInsertar(tbCodigo.Text, tbNombreConcepto.Text, tbPorcentaje.Value, cbOperacion.Value, IIf(swEstado.Value = True, 1, 0), IIf(swTipoConcepto.Value = True, 1, 0))

            If res Then



                ToastNotification.Show(Me, "Concepto Fijo  ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

            Else
                ToastNotification.Show(Me, "Error al guardar el Concepto fijo".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al guardar el Concepto fijo".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return res

    End Function

    Public Function _PMOModificarRegistro() As Boolean
        Dim Res As Boolean
        Try
            Res = L_prConceptoFijoModificar(tbCodigo.Text, tbNombreConcepto.Text, tbPorcentaje.Value, cbOperacion.Value, IIf(swEstado.Value = True, 1, 0), IIf(swTipoConcepto.Value = True, 1, 0))


            If Res Then


                ToastNotification.Show(Me, "Codigo de Concepto Fijo ".ToUpper + tbCodigo.Text + " modificado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PSalirRegistro()
            Else
                ToastNotification.Show(Me, "Error al guardar el Concepto Fijo".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End If
        Catch ex As Exception
            ToastNotification.Show(Me, "Error al modificar el Concepto Fijo".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

        End Try

        Return Res
    End Function
    Public Function _fnActionNuevo() As Boolean
        Return tbCodigo.Text = String.Empty And tbNombreConcepto.ReadOnly = False
    End Function


    Public Sub _PMOEliminarRegistro()


        Dim ef = New Efecto


        ef.tipo = 3
        ef.titulo = "Confirmación de Eliminación"
        ef.descripcion = "¿Esta Seguro de Eliminar el Concepto Fijo " + tbNombreConcepto.Text + " ?"
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean
            Try
                res = L_prBorrarRegistro(tbCodigo.Text, mensajeError, "MAM_ConceptosFijos")
                If res Then

                    ToastNotification.Show(Me, "Codigo de Concepto Fijo ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                    _PMFiltrar()
                Else
                    ToastNotification.Show(Me, mensajeError, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            Catch ex As Exception
                ToastNotification.Show(Me, "Error al eliminar el Concepto Fijo".ToUpper + " " + ex.Message, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            End Try

        End If


    End Sub
    Public Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()
        Dim Mensaje As String = "Los Siguientes Campos Son Requeridos: "
        If tbNombreConcepto.Text = String.Empty Then
            tbNombreConcepto.BackColor = Color.Red
            MEP.SetError(tbNombreConcepto, "Ingrese Nombre de Concepto")
            Mensaje = Mensaje + " Nombre Concepto"
            _ok = False
        Else
            tbNombreConcepto.BackColor = Color.White
            MEP.SetError(tbNombreConcepto, "")
        End If
        If (cbOperacion.SelectedIndex < 0) Then
            cbOperacion.BackColor = Color.Red
            MEP.SetError(cbOperacion, "Seleccione una Operación")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Operación"
            _ok = False
        Else
            cbOperacion.BackColor = Color.White
            MEP.SetError(cbOperacion, "")
        End If


        If (tbPorcentaje.Text <> String.Empty) Then
            If (swTipoConcepto.Value = True) Then
                If (tbPorcentaje.Value <= 0) Then
                    tbPorcentaje.BackColor = Color.Red
                    MEP.SetError(tbPorcentaje, "Ingrese valor Porcentaje Valido")
                    Mensaje = Mensaje + Chr(13) + Chr(10) + " Porcentaje"
                    _ok = False
                Else
                    tbPorcentaje.BackColor = Color.White
                    MEP.SetError(tbPorcentaje, "")
                End If
            End If


        Else
            tbPorcentaje.BackColor = Color.Red
            MEP.SetError(tbPorcentaje, "Ingrese valor Porcentaje Valido")
            Mensaje = Mensaje + Chr(13) + Chr(10) + " Porcentaje"
            _ok = False
        End If




        MHighlighterFocus.UpdateHighlights()

        If tbNombreConcepto.Text = String.Empty Then
            tbNombreConcepto.Focus()
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return _ok
        End If

        If (cbOperacion.SelectedIndex < 0) Then
            ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
            cbOperacion.Focus()
            Return _ok
        End If
        If (swTipoConcepto.Value = True) Then
            If (tbPorcentaje.Text.Length <= 0) Then
                ToastNotification.Show(Me, Mensaje, img, 8000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbPorcentaje.Focus()
                Return _ok
            End If
        End If



        Return _ok
    End Function

    Public Function _PMOGetTablaBuscador() As DataTable

        Dim dtBuscador As DataTable = L_prListarGeneral("MAM_ConceptosFijos")
        Return dtBuscador
    End Function

    Public Function _PMOGetListEstructuraBuscador() As List(Of Celda)

        'a.Id ,a.NombreConcepto ,a.Porcentaje ,a.OperacionId,b.NombreOperacion ,cast('' as image) imgEstado ,a.Estado 
        Dim listEstCeldas As New List(Of Celda)
        listEstCeldas.Add(New Celda("Id", True, "ID", 40))
        listEstCeldas.Add(New Celda("NombreConcepto", True, " Nombre Concepto", 250))
        listEstCeldas.Add(New Celda("Porcentaje", True, "Porcentaje", 100, "0.00"))
        listEstCeldas.Add(New Celda("OperacionId", False))
        listEstCeldas.Add(New Celda("Tipo", False))
        listEstCeldas.Add(New Celda("NombreOperacion", True, "Operación", 90, ""))
        listEstCeldas.Add(New Celda("TipoConcepto", True, "Concepto", 90, ""))
        listEstCeldas.Add(New Celda("Estado", False, "Estado", 70))
        listEstCeldas.Add(New Celda("imgEstado", True, "Estado", 150))


        Return listEstCeldas
    End Function

    Public Sub _PMOMostrarRegistro(_N As Integer, Optional selected As Boolean = False)

        'a.Id , a.NombreConcepto, a.Porcentaje, a.OperacionId, b.NombreOperacion, cast('' as image) imgEstado ,a.Estado  
        If (selected = False) Then
            FilaSeleccionada = True
            JGrM_Buscador.Row = _MPos
            FilaSeleccionada = False
        End If

        With JGrM_Buscador
            tbCodigo.Text = .GetValue("Id").ToString
            tbNombreConcepto.Text = .GetValue("NombreConcepto").ToString
            swTipoConcepto.Value = .GetValue("Tipo")
            tbPorcentaje.Value = .GetValue("Porcentaje")
            cbOperacion.Value = .GetValue("OperacionId")
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

    Private Sub tbNombreConcepto_TextChanged(sender As Object, e As EventArgs) Handles tbNombreConcepto.TextChanged

    End Sub
#End Region
End Class