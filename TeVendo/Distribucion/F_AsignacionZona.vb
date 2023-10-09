
Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class F_AsignacionZona

    Public Sub New()
        InitializeComponent()
        _prCargarPersonal()
    End Sub
    Private Sub _prCargarPersonal()
        Dim dt As New DataTable



        dt = L_prListarGeneral("MAM_Personal")

        grPersonal.DataSource = dt
        grPersonal.RetrieveStructure()
        grPersonal.AlternatingColors = True
        With grPersonal.RootTable.Columns("Id")
            .Width = 100
            .Caption = "ID"
            .Visible = True
        End With
        With grPersonal.RootTable.Columns("Direccion")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("Telefono01")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("TipoDocumento")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("NroDocumento")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("TipoPersonal")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("Estado")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("EmpresaId")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("img")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("Tipo")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("FechaNacimiento")
            .Width = 100
            .Visible = False
        End With
        With grPersonal.RootTable.Columns("NombrePersonal")
            .Width = 300
            .Caption = "Personal"
            .Visible = True
            .MaxLines = 2
            .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
            .TextAlignment = TextAlignment.Center
            .WordWrap = True
        End With

        With grPersonal
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub

    Private Sub _prCargarZona(idRepartidor As Integer)
        Dim dt As New DataTable


        If (grPersonal.Row >= 0) Then

            dt = L_fnListarZonasAsignadas(idRepartidor)

            grzonas.DataSource = dt
            grzonas.RetrieveStructure()
            grzonas.AlternatingColors = True
            With grzonas.RootTable.Columns("id")
                .Width = 100
                .Caption = "ID"
                .Visible = False
            End With
            With grzonas.RootTable.Columns("idRepartidor")
                .Width = 100
                .Visible = False
            End With
            With grzonas.RootTable.Columns("idZona")
                .Width = 100
                .Visible = False
            End With


            With grzonas.RootTable.Columns("asignada")
                .Width = 200
                .Visible = True
            End With
            With grzonas.RootTable.Columns("NombreZona")
                .Width = 300
                .Caption = "Zona"
                .Visible = True
                .MaxLines = 2
                .HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
                .TextAlignment = TextAlignment.Center
                .WordWrap = True
            End With

            With grzonas
                .DefaultFilterRowComparison = FilterConditionOperator.Contains
                .FilterMode = FilterMode.Automatic
                .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
                .GroupByBoxVisible = False
                .VisualStyle = VisualStyle.Office2007
            End With
        End If
        'id  NombreZona	idRepartidor	idZona	asignada

    End Sub

    Private Sub grPersonal_SelectionChanged(sender As Object, e As EventArgs) Handles grPersonal.SelectionChanged
        If (grPersonal.Row >= 0) Then
            _prCargarZona(grPersonal.GetValue("id"))
        End If
    End Sub

    Private Sub grzonas_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grzonas.EditingCell
        If (e.Column.Index = grzonas.RootTable.Columns("asignada").Index) Then
            e.Cancel = False
        Else
            e.Cancel = True
        End If
    End Sub
    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
    Private Sub btnConfirmarSalir_Click(sender As Object, e As EventArgs) Handles btnConfirmarSalir.Click
        If (grPersonal.Row >= 0) Then
            If (L_prRegistrarAsignacion(CType(grzonas.DataSource, DataTable), grPersonal.GetValue("id"))) Then

                ToastNotification.Show(Me, "Asignacion Registrado con Exito", My.Resources.GRABACION_EXITOSA, 3000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _prCargarZona(grPersonal.GetValue("id"))

            Else
                ToastNotification.Show(Me, "Error al Guardar Asignacion".ToUpper, img, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)

                Return
            End If
        End If
    End Sub
End Class