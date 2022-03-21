<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Visualizador
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.CrGeneral = New CrystalDecisions.Windows.Forms.CrystalReportViewer()
        Me.btnSeleccionarProducto = New DevComponents.DotNetBar.ButtonX()
        Me.SuspendLayout()
        '
        'CrGeneral
        '
        Me.CrGeneral.ActiveViewIndex = -1
        Me.CrGeneral.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CrGeneral.Cursor = System.Windows.Forms.Cursors.Default
        Me.CrGeneral.Dock = System.Windows.Forms.DockStyle.Fill
        Me.CrGeneral.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CrGeneral.Location = New System.Drawing.Point(0, 0)
        Me.CrGeneral.Name = "CrGeneral"
        Me.CrGeneral.Size = New System.Drawing.Size(868, 501)
        Me.CrGeneral.TabIndex = 0
        Me.CrGeneral.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None
        '
        'btnSeleccionarProducto
        '
        Me.btnSeleccionarProducto.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSeleccionarProducto.BackColor = System.Drawing.Color.MediumSeaGreen
        Me.btnSeleccionarProducto.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnSeleccionarProducto.Font = New System.Drawing.Font("Calibri", 10.2!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSeleccionarProducto.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnSeleccionarProducto.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.btnSeleccionarProducto.Location = New System.Drawing.Point(13, 82)
        Me.btnSeleccionarProducto.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSeleccionarProducto.Name = "btnSeleccionarProducto"
        Me.btnSeleccionarProducto.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnSeleccionarProducto.Size = New System.Drawing.Size(219, 58)
        Me.btnSeleccionarProducto.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSeleccionarProducto.Symbol = ""
        Me.btnSeleccionarProducto.SymbolColor = System.Drawing.Color.White
        Me.btnSeleccionarProducto.SymbolSize = 24.0!
        Me.btnSeleccionarProducto.TabIndex = 215
        Me.btnSeleccionarProducto.Text = "IMPRIMIR"
        Me.btnSeleccionarProducto.TextColor = System.Drawing.Color.White
        '
        'Visualizador
        '
        Me.ClientSize = New System.Drawing.Size(868, 501)
        Me.Controls.Add(Me.btnSeleccionarProducto)
        Me.Controls.Add(Me.CrGeneral)
        Me.Name = "Visualizador"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CRV1 As CrystalDecisions.Windows.Forms.CrystalReportViewer
    Friend WithEvents Stc_Visualizador As DevComponents.DotNetBar.SuperTabControl
    Friend WithEvents SuperTabControlPanel1 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents Sti_Reporte As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents StBtn_Volver As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents StBtn_Max As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents CrGeneral As CrystalDecisions.Windows.Forms.CrystalReportViewer
    Friend WithEvents btnSeleccionarProducto As DevComponents.DotNetBar.ButtonX
End Class
