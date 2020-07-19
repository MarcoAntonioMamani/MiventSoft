Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid.Style
Imports Janus.Data

Public Class Rep_SaldosValoradosP
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Public Sub _prIniciarTodo()

        Me.Text = "REPORTE SALDOS VALORADOS"
        MReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None


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

    Private Sub Rep_SaldosValoradosP_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click
        Dim _dt As New DataTable
        _dt = ReporteSaldosValorados()
        If (IsNothing(_dt) Or _dt.Rows.Count = 0) Then

            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)

            Return

        End If
        InsertarLogo(_dt)

        If (_dt.Rows.Count > 0) Then

            Dim objrep As New R_SaldosValorados

            objrep.SetDataSource(_dt)
            objrep.SetParameterValue("Usuario", L_Usuario)
            MReportViewer.ReportSource = objrep
            MReportViewer.Show()
            MReportViewer.BringToFront()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "No Existen Datos Para Mostrar. con Los Filtros Seleccionados".ToUpper, img, 5000, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If
    End Sub
End Class