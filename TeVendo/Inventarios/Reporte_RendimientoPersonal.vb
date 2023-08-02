Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid.Style
Imports Janus.Data

Public Class Reporte_RendimientoPersonal
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Public Sub _prIniciarTodo()
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date
        chkPersonal.CheckValue = False
        Me.Text = "REPORTE RENDIMIENTO DE PERSONAL"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        _prCargarTablaVendedor()

    End Sub
    Private Sub Reporte_RendimientoPersonal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub _prCargarTablaVendedor()
        Dim dt As New DataTable
        dt = ReporteListarPersonal()
        gr_Personal.DataSource = dt
        gr_Personal.RetrieveStructure()
        gr_Personal.AlternatingColors = True

        'chk Id	NombrePersonal	Cargo
        With gr_Personal.RootTable.Columns("Id")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False

        End With


        With gr_Personal.RootTable.Columns("chk")
            .Width = 40
            .Visible = True
            .Caption = "Seleccionar"
        End With

        With gr_Personal.RootTable.Columns("NombrePersonal")
            .Width = 150
            .Caption = "Personal"
            .Visible = True
        End With
        With gr_Personal.RootTable.Columns("Cargo")
            .Width = 90
            .Caption = "Cargo"
            .Visible = True
        End With




        With gr_Personal
            .GroupByBoxVisible = False
            'diseño de la grilla
            .BoundMode = Janus.Data.BoundMode.Bound
            .RowHeaders = InheritableBoolean.True
        End With

    End Sub

    Private Sub chkPersonal_CheckedChanged(sender As Object, e As EventArgs) Handles chkPersonal.CheckedChanged
        If (chkPersonal.Checked = True) Then
            For i As Integer = 0 To CType(gr_Personal.DataSource, DataTable).Rows.Count - 1 Step 1
                CType(gr_Personal.DataSource, DataTable).Rows(i).Item("chk") = True
            Next
        Else
            For i As Integer = 0 To CType(gr_Personal.DataSource, DataTable).Rows.Count - 1 Step 1
                CType(gr_Personal.DataSource, DataTable).Rows(i).Item("chk") = False
            Next
        End If
    End Sub


    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(gr_Personal.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(gr_Personal.DataSource, DataTable).Rows(i).Item("Id")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub
    Private Sub gr_Personal_MouseClick(sender As Object, e As MouseEventArgs) Handles gr_Personal.MouseClick
        If (gr_Personal.RowCount >= 1) Then
            If (gr_Personal.CurrentColumn.Index = gr_Personal.RootTable.Columns("chk").Index) Then
                Dim estado As Boolean = gr_Personal.GetValue("chk")
                Dim pos As Integer = -1
                Dim lin As Integer = gr_Personal.GetValue("Id")
                _fnObtenerFilaDetalle(pos, lin)
                If (pos >= 0) Then
                    CType(gr_Personal.DataSource, DataTable).Rows(pos).Item("chk") = Not estado
                    gr_Personal.SetValue("chk", Not estado)
                End If



            End If
        End If
    End Sub
    Public Sub InsertarLogo(ByRef dt As DataTable)
        Dim dtImage As DataTable = ObtenerImagenEmpresa(IIf(Global_Sucursal = -1, 1, Global_Sucursal))
        If (dtImage.Rows.Count > 0) Then
            Dim Name As String = dtImage.Rows(0).Item(0)
            If (File.Exists(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name)) Then
                Dim im As New Bitmap(New Bitmap(RutaGlobal + "\Imagenes\Imagenes Empresa" + Name))
                Dim Bin As New MemoryStream
                Dim img As New Bitmap(im)
                img.Save(Bin, Imaging.ImageFormat.Png)
                Bin.Dispose()
                For i As Integer = 0 To dt.Rows.Count - 1 Step 1


                    dt.Rows(i).Item("Img") = Bin.GetBuffer
                Next
            End If


        End If

    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        If (ContarPersonalSeleccionado() = 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Seleccione al menos un Personal Por Favor".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
            Return

        End If
        Dim _dt As New DataTable
        _dt = ReporteVentasRendimientoPersonal(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"), CType(gr_Personal.DataSource, DataTable))
        If (IsNothing(_dt) Or _dt.Rows.Count = 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            Return

        End If
        InsertarLogo(_dt)

        If (_dt.Rows.Count > 0) Then

            Dim objrep As New Reporte_Rendimiento_Personal


            objrep.SetDataSource(_dt)
            Dim fechaI As String = cbFechaDesde.Value.ToString("dd/MM/yyyy")
            Dim fechaF As String = cbFechaHasta.Value.ToString("dd/MM/yyyy")
            objrep.SetParameterValue("FechaDesde", fechaI)
            objrep.SetParameterValue("FechaHasta", fechaF)
            objrep.SetParameterValue("Usuario", L_Usuario)
            MReportViewer.ReportSource = objrep
            MReportViewer.Show()
            MReportViewer.BringToFront()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar. con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If

    End Sub

    Public Function ContarPersonalSeleccionado() As Integer
        Dim dt As DataTable = CType(gr_Personal.DataSource, DataTable)
        Dim cantidad As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            If (dt.Rows(i).Item("chk") = True) Then
                cantidad += 1

            End If
        Next

        Return cantidad

    End Function
End Class