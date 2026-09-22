<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Rep_AuditoriaVentas
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
        Dim cbTipoEvento_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Rep_AuditoriaVentas))
        Me.Principal = New System.Windows.Forms.Panel()
        Me.PanelDatos = New System.Windows.Forms.Panel()
        Me.PanelDetalle = New System.Windows.Forms.Panel()
        Me.grDetalle = New Janus.Windows.GridEX.GridEX()
        Me.lblDetalle = New System.Windows.Forms.Label()
        Me.PanelSeparador = New System.Windows.Forms.Panel()
        Me.PanelMaestro = New System.Windows.Forms.Panel()
        Me.grMaestro = New Janus.Windows.GridEX.GridEX()
        Me.lblMaestro = New System.Windows.Forms.Label()
        Me.PanelFiltrosLinea = New System.Windows.Forms.Panel()
        Me.PanelFiltros = New System.Windows.Forms.Panel()
        Me.btnBuscar = New DevComponents.DotNetBar.ButtonX()
        Me.cbTipoEvento = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX3 = New DevComponents.DotNetBar.LabelX()
        Me.cbFechaHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.cbFechaDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.PanelTitulo = New System.Windows.Forms.Panel()
        Me.lblTitulo = New System.Windows.Forms.Label()
        Me.Principal.SuspendLayout()
        Me.PanelDatos.SuspendLayout()
        Me.PanelDetalle.SuspendLayout()
        CType(Me.grDetalle, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelMaestro.SuspendLayout()
        CType(Me.grMaestro, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelFiltros.SuspendLayout()
        CType(Me.cbTipoEvento, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.Principal.Size = New System.Drawing.Size(1200, 650)
        Me.Principal.TabIndex = 0
        '
        'PanelDatos
        '
        Me.PanelDatos.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelDatos.Controls.Add(Me.PanelDetalle)
        Me.PanelDatos.Controls.Add(Me.PanelSeparador)
        Me.PanelDatos.Controls.Add(Me.PanelMaestro)
        Me.PanelDatos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDatos.Location = New System.Drawing.Point(0, 107)
        Me.PanelDatos.Name = "PanelDatos"
        Me.PanelDatos.Padding = New System.Windows.Forms.Padding(8)
        Me.PanelDatos.Size = New System.Drawing.Size(1200, 543)
        Me.PanelDatos.TabIndex = 2
        '
        'PanelDetalle
        '
        Me.PanelDetalle.BackColor = System.Drawing.Color.White
        Me.PanelDetalle.Controls.Add(Me.grDetalle)
        Me.PanelDetalle.Controls.Add(Me.lblDetalle)
        Me.PanelDetalle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetalle.Location = New System.Drawing.Point(590, 8)
        Me.PanelDetalle.Name = "PanelDetalle"
        Me.PanelDetalle.Padding = New System.Windows.Forms.Padding(1)
        Me.PanelDetalle.Size = New System.Drawing.Size(602, 527)
        Me.PanelDetalle.TabIndex = 1
        '
        'grDetalle
        '
        Me.grDetalle.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grDetalle.AlternatingColors = True
        Me.grDetalle.BackColor = System.Drawing.Color.White
        Me.grDetalle.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grDetalle.ColumnAutoResize = True
        Me.grDetalle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grDetalle.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grDetalle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grDetalle.HeaderFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grDetalle.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grDetalle.Location = New System.Drawing.Point(1, 31)
        Me.grDetalle.Name = "grDetalle"
        Me.grDetalle.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grDetalle.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grDetalle.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.SelectedFormatStyle.BackColor = System.Drawing.Color.Gold
        Me.grDetalle.SelectedFormatStyle.FontBold = Janus.Windows.GridEX.TriState.[True]
        Me.grDetalle.SelectedFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grDetalle.Size = New System.Drawing.Size(600, 495)
        Me.grDetalle.TabIndex = 1
        Me.grDetalle.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'lblDetalle
        '
        Me.lblDetalle.BackColor = System.Drawing.Color.MidnightBlue
        Me.lblDetalle.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblDetalle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDetalle.ForeColor = System.Drawing.Color.White
        Me.lblDetalle.Location = New System.Drawing.Point(1, 1)
        Me.lblDetalle.Name = "lblDetalle"
        Me.lblDetalle.Padding = New System.Windows.Forms.Padding(12, 0, 0, 0)
        Me.lblDetalle.Size = New System.Drawing.Size(600, 30)
        Me.lblDetalle.TabIndex = 0
        Me.lblDetalle.Text = "DETALLE DEL EVENTO SELECCIONADO"
        Me.lblDetalle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelSeparador
        '
        Me.PanelSeparador.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelSeparador.Dock = System.Windows.Forms.DockStyle.Left
        Me.PanelSeparador.Location = New System.Drawing.Point(580, 8)
        Me.PanelSeparador.Name = "PanelSeparador"
        Me.PanelSeparador.Size = New System.Drawing.Size(10, 527)
        Me.PanelSeparador.TabIndex = 2
        '
        'PanelMaestro
        '
        Me.PanelMaestro.BackColor = System.Drawing.Color.White
        Me.PanelMaestro.Controls.Add(Me.grMaestro)
        Me.PanelMaestro.Controls.Add(Me.lblMaestro)
        Me.PanelMaestro.Dock = System.Windows.Forms.DockStyle.Left
        Me.PanelMaestro.Location = New System.Drawing.Point(8, 8)
        Me.PanelMaestro.Name = "PanelMaestro"
        Me.PanelMaestro.Padding = New System.Windows.Forms.Padding(1)
        Me.PanelMaestro.Size = New System.Drawing.Size(572, 527)
        Me.PanelMaestro.TabIndex = 0
        '
        'grMaestro
        '
        Me.grMaestro.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grMaestro.AlternatingColors = True
        Me.grMaestro.BackColor = System.Drawing.Color.White
        Me.grMaestro.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grMaestro.ColumnAutoResize = True
        Me.grMaestro.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grMaestro.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grMaestro.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grMaestro.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grMaestro.HeaderFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grMaestro.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grMaestro.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grMaestro.Location = New System.Drawing.Point(1, 31)
        Me.grMaestro.Name = "grMaestro"
        Me.grMaestro.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grMaestro.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grMaestro.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grMaestro.SelectedFormatStyle.BackColor = System.Drawing.Color.Gold
        Me.grMaestro.SelectedFormatStyle.FontBold = Janus.Windows.GridEX.TriState.[True]
        Me.grMaestro.SelectedFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grMaestro.Size = New System.Drawing.Size(570, 495)
        Me.grMaestro.TabIndex = 0
        Me.grMaestro.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'lblMaestro
        '
        Me.lblMaestro.BackColor = System.Drawing.Color.MidnightBlue
        Me.lblMaestro.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblMaestro.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMaestro.ForeColor = System.Drawing.Color.White
        Me.lblMaestro.Location = New System.Drawing.Point(1, 1)
        Me.lblMaestro.Name = "lblMaestro"
        Me.lblMaestro.Padding = New System.Windows.Forms.Padding(12, 0, 0, 0)
        Me.lblMaestro.Size = New System.Drawing.Size(570, 30)
        Me.lblMaestro.TabIndex = 1
        Me.lblMaestro.Text = "EVENTOS DEL DIA"
        Me.lblMaestro.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelFiltrosLinea
        '
        Me.PanelFiltrosLinea.BackColor = System.Drawing.Color.Gainsboro
        Me.PanelFiltrosLinea.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFiltrosLinea.Location = New System.Drawing.Point(0, 106)
        Me.PanelFiltrosLinea.Name = "PanelFiltrosLinea"
        Me.PanelFiltrosLinea.Size = New System.Drawing.Size(1200, 1)
        Me.PanelFiltrosLinea.TabIndex = 3
        '
        'PanelFiltros
        '
        Me.PanelFiltros.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelFiltros.Controls.Add(Me.btnBuscar)
        Me.PanelFiltros.Controls.Add(Me.cbTipoEvento)
        Me.PanelFiltros.Controls.Add(Me.LabelX3)
        Me.PanelFiltros.Controls.Add(Me.cbFechaHasta)
        Me.PanelFiltros.Controls.Add(Me.LabelX2)
        Me.PanelFiltros.Controls.Add(Me.cbFechaDesde)
        Me.PanelFiltros.Controls.Add(Me.LabelX1)
        Me.PanelFiltros.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFiltros.Location = New System.Drawing.Point(0, 40)
        Me.PanelFiltros.Name = "PanelFiltros"
        Me.PanelFiltros.Size = New System.Drawing.Size(1200, 66)
        Me.PanelFiltros.TabIndex = 1
        '
        'btnBuscar
        '
        Me.btnBuscar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnBuscar.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnBuscar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBuscar.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnBuscar.ImageFixedSize = New System.Drawing.Size(20, 20)
        Me.btnBuscar.Location = New System.Drawing.Point(870, 15)
        Me.btnBuscar.Name = "btnBuscar"
        Me.btnBuscar.Size = New System.Drawing.Size(150, 38)
        Me.btnBuscar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnBuscar.TabIndex = 6
        Me.btnBuscar.Text = "CARGAR DATOS"
        '
        'cbTipoEvento
        '
        cbTipoEvento_DesignTimeLayout.LayoutString = resources.GetString("cbTipoEvento_DesignTimeLayout.LayoutString")
        Me.cbTipoEvento.DesignTimeLayout = cbTipoEvento_DesignTimeLayout
        Me.cbTipoEvento.Location = New System.Drawing.Point(650, 20)
        Me.cbTipoEvento.Name = "cbTipoEvento"
        Me.cbTipoEvento.SelectedIndex = -1
        Me.cbTipoEvento.SelectedItem = Nothing
        Me.cbTipoEvento.Size = New System.Drawing.Size(180, 22)
        Me.cbTipoEvento.TabIndex = 5
        '
        'LabelX3
        '
        Me.LabelX3.AutoSize = True
        Me.LabelX3.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX3.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX3.Location = New System.Drawing.Point(600, 23)
        Me.LabelX3.Name = "LabelX3"
        Me.LabelX3.Size = New System.Drawing.Size(34, 21)
        Me.LabelX3.TabIndex = 4
        Me.LabelX3.Text = "Tipo:"
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
        Me.cbFechaDesde.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
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
        'PanelTitulo
        '
        Me.PanelTitulo.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.PanelTitulo.Controls.Add(Me.lblTitulo)
        Me.PanelTitulo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelTitulo.Location = New System.Drawing.Point(0, 0)
        Me.PanelTitulo.Name = "PanelTitulo"
        Me.PanelTitulo.Size = New System.Drawing.Size(1200, 40)
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
        Me.lblTitulo.Size = New System.Drawing.Size(1200, 40)
        Me.lblTitulo.TabIndex = 0
        Me.lblTitulo.Text = "AUDITORIA DE VENTAS - ANULADOS / ELIMINADOS / MODIFICADOS"
        Me.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Rep_AuditoriaVentas
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1200, 650)
        Me.Controls.Add(Me.Principal)
        Me.Name = "Rep_AuditoriaVentas"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AUDITORIA DE VENTAS"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Principal.ResumeLayout(False)
        Me.PanelDatos.ResumeLayout(False)
        Me.PanelDetalle.ResumeLayout(False)
        CType(Me.grDetalle, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelMaestro.ResumeLayout(False)
        CType(Me.grMaestro, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelFiltros.ResumeLayout(False)
        Me.PanelFiltros.PerformLayout()
        CType(Me.cbTipoEvento, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelTitulo.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Principal As Panel
    Friend WithEvents PanelTitulo As Panel
    Friend WithEvents lblTitulo As Label
    Friend WithEvents PanelFiltrosLinea As Panel
    Friend WithEvents PanelFiltros As Panel
    Friend WithEvents btnBuscar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents cbTipoEvento As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX3 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbFechaHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbFechaDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents PanelDatos As Panel
    Friend WithEvents PanelMaestro As Panel
    Friend WithEvents lblMaestro As Label
    Friend WithEvents grMaestro As Janus.Windows.GridEX.GridEX
    Friend WithEvents PanelSeparador As Panel
    Friend WithEvents PanelDetalle As Panel
    Friend WithEvents lblDetalle As Label
    Friend WithEvents grDetalle As Janus.Windows.GridEX.GridEX
End Class
