Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class Reporte_ProductoMasVendido
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim IdPersonal As Integer = 0

    Public Sub _prIniciarTodo()
        Dim dt As DataTable = L_fnGeneralSucursales()
        dt.Rows.Add(-1, "Todos")
        P_Global._prCargarComboGenerico(cbSucursal, dt, "aanumi", "Codigo", "aabdes", "Sucursal")
        cbFechaDesde.Value = Now.Date
        cbFechaHasta.Value = Now.Date
        tbCantidad.Value = 10
        swTipoReporte.Value = True
        Me.Text = "REPORTE DE PRODUCTOS MAS VENDIDOS"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None


        If (Global_Sucursal = -1) Then
            cbSucursal.ReadOnly = False
        Else
            cbSucursal.Value = Global_Sucursal
            cbSucursal.ReadOnly = True
        End If
    End Sub

    Public Sub GenerarData(ByRef dt As DataTable)


        dt = ReporteVentasProductosMasVendido(cbFechaDesde.Value.ToString("yyyy/MM/dd"), cbFechaHasta.Value.ToString("yyyy/MM/dd"))


        If (cbSucursal.Value <> -1) Then
            Dim dt2 As DataTable = dt.Copy
            dt2.Rows.Clear()
            For i As Integer = 0 To dt.Rows.Count - 1 Step 1

                If (cbSucursal.Value = dt.Rows(i).Item("SucursalId")) Then
                    dt2.ImportRow(dt.Rows(i))
                End If

            Next
            dt = dt2
        End If
        InsertarLogo(dt)
    End Sub

    Public Sub InsertarLogo(ByRef dt As DataTable)
        Dim dtImage As DataTable = ObtenerImagenEmpresa()
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
    Private Sub Rep_VentasRealizadas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()

    End Sub
    Private Sub swTipoReporte_ValueChanged(sender As Object, e As EventArgs) Handles swTipoReporte.ValueChanged
        If (swTipoReporte.Value = True) Then
            lbCantidad.Visible = False
            tbCantidad.Visible = False
        Else
            lbCantidad.Visible = True
            tbCantidad.Visible = True

        End If
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        If (swTipoReporte.Value = True) Then ''' Generar Reporte de Datos
            Dim _dt As New DataTable
            GenerarData(_dt)
            If (IsNothing(_dt) Or _dt.Rows.Count = 0) Then

                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

                Return

            End If


            If (_dt.Rows.Count > 0) Then

                Dim objrep As New Reporte_ProductosMasVendidos

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
        Else
            Dim _dt As New DataTable
            GenerarData(_dt)
            If (IsNothing(_dt) Or _dt.Rows.Count = 0) Then

                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

                Return

            End If

            Dim dtData As DataTable = _dt.Copy
            dtData.Rows.Clear()

            If (_dt.Rows.Count > 0 And tbCantidad.Value > 0) Then

                For i As Integer = 0 To _dt.Rows.Count - 1
                    If (i < tbCantidad.Value) Then
                        'IdProducto  NombreProducto	Familia	SucursalId	NombreAlmacen	Cantidad	Total	img
                        dtData.Rows.Add(_dt.Rows(i).Item("IdProducto"), _dt.Rows(i).Item("NombreProducto"), _dt.Rows(i).Item("Familia"), _dt.Rows(i).Item("SucursalId"), _dt.Rows(i).Item("NombreAlmacen"), _dt.Rows(i).Item("Cantidad"), _dt.Rows(i).Item("Total"), _dt.Rows(i).Item("img"))

                    End If

                Next


                Dim objrep As New ReporteGraficoProductosMasVendidos
                objrep.SetDataSource(dtData)
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
        End If
    End Sub
End Class