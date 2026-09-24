<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Rep_HistorialIncrementoVenta
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Principal = New System.Windows.Forms.Panel()
        Me.PanelDatos = New System.Windows.Forms.Panel()
        Me.PanelContenedor = New System.Windows.Forms.Panel()
        Me.grHistorial = New Janus.Windows.GridEX.GridEX()
        Me.lblHistorial = New System.Windows.Forms.Label()
        Me.PanelFiltrosLinea = New System.Windows.Forms.Panel()
        Me.PanelFiltros = New System.Windows.Forms.Panel()
        Me.btnBuscar = New DevComponents.DotNetBar.ButtonX()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.cbFechaHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.cbFechaDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.PanelTitulo = New System.Windows.Forms.Panel()
        Me.lblTitulo = New System.Windows.Forms.Label()
        Me.Principal.SuspendLayout()
        Me.PanelDatos.SuspendLayout()
        Me.PanelContenedor.SuspendLayout()
        CType(Me.grHistorial, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelFiltros.SuspendLayout()
        Me.PanelTitulo.SuspendLayout()
        Me.SuspendLayout()
        '
        'Principal
        '
        Me.Principal.BackColor = System.Drawing.Color.White
        Me.Principal.Controls.Add(Me.PanelDatos)
        Me.Principal.Controls.Add(Me.PanelFiltrosLinea)
        Me.Principal.Controls.Add(Me.PanelFiltros)
        Me.Principal.Controls.Add(Me.PanelTitulo)
        Me.Principal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Principal.Location = New System.Drawing.Point(0, 0)
        Me.Principal.Name = "Principal"
        Me.Principal.Size = New System.Drawing.Size(900, 560)
        Me.Principal.TabIndex = 0
        '
        'PanelDatos
        '
        Me.PanelDatos.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelDatos.Controls.Add(Me.PanelContenedor)
        Me.PanelDatos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDatos.Location = New System.Drawing.Point(0, 107)
        Me.PanelDatos.Name = "PanelDatos"
        Me.PanelDatos.Padding = New System.Windows.Forms.Padding(8)
        Me.PanelDatos.Size = New System.Drawing.Size(900, 453)
        Me.PanelDatos.TabIndex = 2
        '
        'PanelContenedor
        '
        Me.PanelContenedor.BackColor = System.Drawing.Color.White
        Me.PanelContenedor.Controls.Add(Me.grHistorial)
        Me.PanelContenedor.Controls.Add(Me.lblHistorial)
        Me.PanelContenedor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelContenedor.Location = New System.Drawing.Point(8, 8)
        Me.PanelContenedor.Name = "PanelContenedor"
        Me.PanelContenedor.Padding = New System.Windows.Forms.Padding(1)
        Me.PanelContenedor.Size = New System.Drawing.Size(884, 437)
        Me.PanelContenedor.TabIndex = 0
        '
        'grHistorial
        '
        Me.grHistorial.AlternatingColors = True
        Me.grHistorial.BackColor = System.Drawing.Color.White
        Me.grHistorial.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grHistorial.ColumnAutoResize = True
        Me.grHistorial.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grHistorial.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grHistorial.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grHistorial.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grHistorial.HeaderFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grHistorial.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grHistorial.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grHistorial.Location = New System.Drawing.Point(1, 31)
        Me.grHistorial.Name = "grHistorial"
        Me.grHistorial.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grHistorial.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grHistorial.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grHistorial.SelectedFormatStyle.BackColor = System.Drawing.Color.Gold
        Me.grHistorial.SelectedFormatStyle.FontBold = Janus.Windows.GridEX.TriState.[True]
        Me.grHistorial.SelectedFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grHistorial.Size = New System.Drawing.Size(882, 405)
        Me.grHistorial.TabIndex = 1
        Me.grHistorial.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'lblHistorial
        '
        Me.lblHistorial.BackColor = System.Drawing.Color.MidnightBlue
        Me.lblHistorial.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblHistorial.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHistorial.ForeColor = System.Drawing.Color.White
        Me.lblHistorial.Location = New System.Drawing.Point(1, 1)
        Me.lblHistorial.Name = "lblHistorial"
        Me.lblHistorial.Padding = New System.Windows.Forms.Padding(12, 0, 0, 0)
        Me.lblHistorial.Size = New System.Drawing.Size(882, 30)
        Me.lblHistorial.TabIndex = 0
        Me.lblHistorial.Text = "CAMBIOS REGISTRADOS"
        Me.lblHistorial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelFiltrosLinea
        '
        Me.PanelFiltrosLinea.BackColor = System.Drawing.Color.Gainsboro
        Me.PanelFiltrosLinea.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFiltrosLinea.Location = New System.Drawing.Point(0, 106)
        Me.PanelFiltrosLinea.Name = "PanelFiltrosLinea"
        Me.PanelFiltrosLinea.Size = New System.Drawing.Size(900, 1)
        Me.PanelFiltrosLinea.TabIndex = 3
        '
        'PanelFiltros
        '
        Me.PanelFiltros.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelFiltros.Controls.Add(Me.btnBuscar)
        Me.PanelFiltros.Controls.Add(Me.LabelX2)
        Me.PanelFiltros.Controls.Add(Me.cbFechaHasta)
        Me.PanelFiltros.Controls.Add(Me.LabelX1)
        Me.PanelFiltros.Controls.Add(Me.cbFechaDesde)
        Me.PanelFiltros.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFiltros.Location = New System.Drawing.Point(0, 40)
        Me.PanelFiltros.Name = "PanelFiltros"
        Me.PanelFiltros.Size = New System.Drawing.Size(900, 66)
        Me.PanelFiltros.TabIndex = 1
        '
        'btnBuscar
        '
        Me.btnBuscar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnBuscar.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnBuscar.CustomColorName = "49;59;66"
        Me.btnBuscar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBuscar.Location = New System.Drawing.Point(600, 15)
        Me.btnBuscar.Name = "btnBuscar"
        Me.btnBuscar.Size = New System.Drawing.Size(150, 38)
        Me.btnBuscar.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.btnBuscar.TabIndex = 5
        Me.btnBuscar.Text = "BUSCAR"
        Me.btnBuscar.TextColor = System.Drawing.Color.White
        '
        'LabelX2
        '
        Me.LabelX2.AutoSize = True
        Me.LabelX2.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX2.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX2.Location = New System.Drawing.Point(350, 23)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.Size = New System.Drawing.Size(42, 21)
        Me.LabelX2.TabIndex = 2
        Me.LabelX2.Text = "Hasta:"
        '
        'cbFechaHasta
        '
        Me.cbFechaHasta.BackColor = System.Drawing.Color.White
        Me.cbFechaHasta.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
        Me.cbFechaHasta.DropDownCalendar.Name = ""
        Me.cbFechaHasta.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.cbFechaHasta.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.cbFechaHasta.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbFechaHasta.Location = New System.Drawing.Point(400, 20)
        Me.cbFechaHasta.Name = "cbFechaHasta"
        Me.cbFechaHasta.Size = New System.Drawing.Size(180, 26)
        Me.cbFechaHasta.TabIndex = 3
        Me.cbFechaHasta.TodayButtonText = "Hoy"
        Me.cbFechaHasta.UseCompatibleTextRendering = False
        Me.cbFechaHasta.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        '
        'LabelX1
        '
        Me.LabelX1.AutoSize = True
        Me.LabelX1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.Location = New System.Drawing.Point(20, 23)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.Size = New System.Drawing.Size(45, 21)
        Me.LabelX1.TabIndex = 0
        Me.LabelX1.Text = "Desde:"
        '
        'cbFechaDesde
        '
        Me.cbFechaDesde.BackColor = System.Drawing.Color.White
        Me.cbFechaDesde.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
        Me.cbFechaDesde.DropDownCalendar.Name = ""
        Me.cbFechaDesde.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.cbFechaDesde.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.cbFechaDesde.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbFechaDesde.Location = New System.Drawing.Point(90, 20)
        Me.cbFechaDesde.Name = "cbFechaDesde"
        Me.cbFechaDesde.Size = New System.Drawing.Size(180, 26)
        Me.cbFechaDesde.TabIndex = 1
        Me.cbFechaDesde.TodayButtonText = "Hoy"
        Me.cbFechaDesde.UseCompatibleTextRendering = False
        Me.cbFechaDesde.Value = New Date(2026, 8, 23, 0, 0, 0, 0)
        Me.cbFechaDesde.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        '
        'PanelTitulo
        '
        Me.PanelTitulo.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.PanelTitulo.Controls.Add(Me.lblTitulo)
        Me.PanelTitulo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelTitulo.Location = New System.Drawing.Point(0, 0)
        Me.PanelTitulo.Name = "PanelTitulo"
        Me.PanelTitulo.Size = New System.Drawing.Size(900, 40)
        Me.PanelTitulo.TabIndex = 0
        '
        'lblTitulo
        '
        Me.lblTitulo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTitulo.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitulo.ForeColor = System.Drawing.Color.White
        Me.lblTitulo.Location = New System.Drawing.Point(0, 0)
        Me.lblTitulo.Name = "lblTitulo"
        Me.lblTitulo.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.lblTitulo.Size = New System.Drawing.Size(900, 40)
        Me.lblTitulo.TabIndex = 0
        Me.lblTitulo.Text = "HISTORIAL DEL INCREMENTO DIARIO A LA VENTA"
        Me.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Rep_HistorialIncrementoVenta
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 560)
        Me.Controls.Add(Me.Principal)
        Me.Name = "Rep_HistorialIncrementoVenta"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "HISTORIAL DEL INCREMENTO DIARIO A LA VENTA"
        Me.Principal.ResumeLayout(False)
        Me.PanelDatos.ResumeLayout(False)
        Me.PanelContenedor.ResumeLayout(False)
        CType(Me.grHistorial, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelFiltros.ResumeLayout(False)
        Me.PanelFiltros.PerformLayout()
        Me.PanelTitulo.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Principal As Panel
    Friend WithEvents PanelTitulo As Panel
    Friend WithEvents lblTitulo As Label
    Friend WithEvents PanelFiltrosLinea As Panel
    Friend WithEvents PanelFiltros As Panel
    Friend WithEvents btnBuscar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbFechaHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbFechaDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents PanelDatos As Panel
    Friend WithEvents PanelContenedor As Panel
    Friend WithEvents lblHistorial As Label
    Friend WithEvents grHistorial As Janus.Windows.GridEX.GridEX
End Class
