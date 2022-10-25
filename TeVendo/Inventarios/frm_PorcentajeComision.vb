Imports Janus.Windows.GridEX
Imports Negocio.AccesoLogica

Imports DevComponents.DotNetBar
Imports System.IO
Public Class frm_PorcentajeComision
    Public dtProductos As DataTable
    Public dtDetalle As DataTable
    Public Lote As Boolean
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Public TipoMovimientoId As Integer
    Public DepositoId As Integer

    Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)

    Public Sub IniciarTodod()
        P_Global._prCargarComboGenerico(cbPersonal, ListarPersonalCombo(), "id", "Codigo", "Nombre", "Personal")


    End Sub


    Public Sub New()
        InitializeComponent()

    End Sub

    Private Sub Tec_DespachoDetalle_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciarTodod()

    End Sub


    Private Sub _prCargarComision()
        Dim dt As New DataTable



        dt = L_prListarAlmacenesComision(cbPersonal.Value)  ''1=Almacen
        dtProductos = dt

        ' 0 as Id,@Id as PersonalId,a.id as SucursalId,a.NombreAlmacen ,cast(0 as decimal(18,2)) as PorcentajeComision

        grAlmacenes.DataSource = dt
        grAlmacenes.RetrieveStructure()
        grAlmacenes.AlternatingColors = True
        With grAlmacenes.RootTable.Columns("Id")
            .Width = 100
            .Visible = False


        End With
        With grAlmacenes.RootTable.Columns("PersonalId")
            .Width = 100
            .Visible = False


        End With
        With grAlmacenes.RootTable.Columns("SucursalId")
            .Width = 100
            .Visible = False


        End With
        With grAlmacenes.RootTable.Columns("NombreAlmacen")
            .Width = 60
            .Caption = "Sucursal"
            .Visible = True
            .MaxLines = 3
            .WordWrap = True

        End With

        With grAlmacenes.RootTable.Columns("PorcentajeComision")
            .Width = 150
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Porcentaje Comision"
            .MaxLines = 2
            .WordWrap = True
        End With
        With grAlmacenes
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With

    End Sub
    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grAlmacenes.EditingCell

        If (grAlmacenes.GetValue("PorcentajeComision") = 2) Then
            e.Cancel = False
            Return

        End If
        e.Cancel = True
    End Sub

    Private Sub cbPersonal_ValueChanged(sender As Object, e As EventArgs) Handles cbPersonal.ValueChanged
        _prCargarComision()
    End Sub
End Class