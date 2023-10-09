
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

        'id  NombrePersonal	Direccion	Telefono01	TipoDocumento	NroDocumento	
        '    TipoPersonal    Estado	EmpresaId	img	Tipo	FechaNacimiento
        grPersonal.DataSource = dt
        grPersonal.RetrieveStructure()
        grPersonal.AlternatingColors = True
        With grPersonal.RootTable.Columns("Id")
            .Width = 100
            .Caption = "ID"
            .Visible = False
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
            .Visible = False
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
End Class