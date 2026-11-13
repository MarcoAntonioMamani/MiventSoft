<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_AuditoriaRegistros
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
        Me.PanelTitulo = New System.Windows.Forms.Panel()
        Me.PanelTitulo2 = New System.Windows.Forms.Panel()
        Me.lbTitulo = New System.Windows.Forms.Label()
        Me.PanelSeparador = New System.Windows.Forms.Panel()
        Me.PictureBoxTitulo = New System.Windows.Forms.PictureBox()
        Me.PanelFiltros = New System.Windows.Forms.Panel()
        Me.tbDocumento = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.lbDocumento = New DevComponents.DotNetBar.LabelX()
        Me.cbOperacion = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.lbOperacion = New DevComponents.DotNetBar.LabelX()
        Me.cbModulo = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.lbModulo = New DevComponents.DotNetBar.LabelX()
        Me.cbUsuario = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.lbUsuario = New DevComponents.DotNetBar.LabelX()
        Me.cbFechaHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.lbHasta = New DevComponents.DotNetBar.LabelX()
        Me.cbFechaDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.lbDesde = New DevComponents.DotNetBar.LabelX()
        Me.PanelBotones = New System.Windows.Forms.Panel()
        Me.btnBuscar = New DevComponents.DotNetBar.ButtonX()
        Me.btnReporte = New DevComponents.DotNetBar.ButtonX()
        Me.PanelResumen = New System.Windows.Forms.Panel()
        Me.lbResumen = New System.Windows.Forms.Label()
        Me.PanelPrincipal = New System.Windows.Forms.Panel()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.grAuditoria = New Janus.Windows.GridEX.GridEX()
        Me.PanelDetalle = New System.Windows.Forms.Panel()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.grDetalle = New Janus.Windows.GridEX.GridEX()
        Me.lbDetalleResumen = New System.Windows.Forms.Label()
        Me.grCabecera = New Janus.Windows.GridEX.GridEX()
        Me.lbCabecera = New System.Windows.Forms.Label()
        Me.PanelInfo = New System.Windows.Forms.Panel()
        Me.lnkUbicacion = New System.Windows.Forms.LinkLabel()
        Me.lbNavegador = New System.Windows.Forms.Label()
        Me.lbEquipo = New System.Windows.Forms.Label()
        Me.lbQuien = New System.Windows.Forms.Label()
        Me.lbTituloDetalle = New System.Windows.Forms.Label()
        Me.btnImprimirRegistro = New DevComponents.DotNetBar.ButtonX()
        Me.lbSinSeleccion = New System.Windows.Forms.Label()
        Me.PanelTitulo.SuspendLayout()
        Me.PanelTitulo2.SuspendLayout()
        CType(Me.PictureBoxTitulo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelFiltros.SuspendLayout()
        CType(Me.cbOperacion, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cbModulo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cbUsuario, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelBotones.SuspendLayout()
        Me.PanelResumen.SuspendLayout()
        Me.PanelPrincipal.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.grAuditoria, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelDetalle.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.grDetalle, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.grCabecera, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelInfo.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelTitulo
        '
        Me.PanelTitulo.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.PanelTitulo.Controls.Add(Me.PanelTitulo2)
        Me.PanelTitulo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelTitulo.Location = New System.Drawing.Point(0, 0)
        Me.PanelTitulo.Name = "PanelTitulo"
        Me.PanelTitulo.Padding = New System.Windows.Forms.Padding(1)
        Me.PanelTitulo.Size = New System.Drawing.Size(1280, 27)
        Me.PanelTitulo.TabIndex = 0
        '
        'PanelTitulo2
        '
        Me.PanelTitulo2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelTitulo2.Controls.Add(Me.lbTitulo)
        Me.PanelTitulo2.Controls.Add(Me.PanelSeparador)
        Me.PanelTitulo2.Controls.Add(Me.PictureBoxTitulo)
        Me.PanelTitulo2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelTitulo2.Location = New System.Drawing.Point(1, 1)
        Me.PanelTitulo2.Name = "PanelTitulo2"
        Me.PanelTitulo2.Size = New System.Drawing.Size(1278, 25)
        Me.PanelTitulo2.TabIndex = 0
        '
        'lbTitulo
        '
        Me.lbTitulo.BackColor = System.Drawing.Color.Transparent
        Me.lbTitulo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbTitulo.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbTitulo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbTitulo.Location = New System.Drawing.Point(59, 0)
        Me.lbTitulo.Name = "lbTitulo"
        Me.lbTitulo.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.lbTitulo.Size = New System.Drawing.Size(1219, 25)
        Me.lbTitulo.TabIndex = 2
        Me.lbTitulo.Text = "AUDITORIA DE REGISTROS  (lo modificado y eliminado en ventas, compras y movimientos de inventario)"
        Me.lbTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelSeparador
        '
        Me.PanelSeparador.BackColor = System.Drawing.Color.White
        Me.PanelSeparador.Dock = System.Windows.Forms.DockStyle.Left
        Me.PanelSeparador.Location = New System.Drawing.Point(58, 0)
        Me.PanelSeparador.Name = "PanelSeparador"
        Me.PanelSeparador.Size = New System.Drawing.Size(1, 25)
        Me.PanelSeparador.TabIndex = 1
        '
        'PictureBoxTitulo
        '
        Me.PictureBoxTitulo.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBoxTitulo.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBoxTitulo.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBoxTitulo.Location = New System.Drawing.Point(0, 0)
        Me.PictureBoxTitulo.Name = "PictureBoxTitulo"
        Me.PictureBoxTitulo.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBoxTitulo.Size = New System.Drawing.Size(58, 25)
        Me.PictureBoxTitulo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBoxTitulo.TabIndex = 0
        Me.PictureBoxTitulo.TabStop = False
        '
        'PanelFiltros
        '
        Me.PanelFiltros.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PanelFiltros.Controls.Add(Me.tbDocumento)
        Me.PanelFiltros.Controls.Add(Me.lbDocumento)
        Me.PanelFiltros.Controls.Add(Me.cbOperacion)
        Me.PanelFiltros.Controls.Add(Me.lbOperacion)
        Me.PanelFiltros.Controls.Add(Me.cbModulo)
        Me.PanelFiltros.Controls.Add(Me.lbModulo)
        Me.PanelFiltros.Controls.Add(Me.cbUsuario)
        Me.PanelFiltros.Controls.Add(Me.lbUsuario)
        Me.PanelFiltros.Controls.Add(Me.cbFechaHasta)
        Me.PanelFiltros.Controls.Add(Me.lbHasta)
        Me.PanelFiltros.Controls.Add(Me.cbFechaDesde)
        Me.PanelFiltros.Controls.Add(Me.lbDesde)
        Me.PanelFiltros.Controls.Add(Me.PanelBotones)
        Me.PanelFiltros.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFiltros.Location = New System.Drawing.Point(0, 27)
        Me.PanelFiltros.Name = "PanelFiltros"
        Me.PanelFiltros.Size = New System.Drawing.Size(1280, 80)
        Me.PanelFiltros.TabIndex = 1
        '
        'lbDesde
        '
        Me.lbDesde.AutoSize = True
        Me.lbDesde.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbDesde.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbDesde.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbDesde.ForeColor = System.Drawing.Color.White
        Me.lbDesde.Location = New System.Drawing.Point(14, 13)
        Me.lbDesde.Name = "lbDesde"
        Me.lbDesde.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbDesde.Size = New System.Drawing.Size(45, 21)
        Me.lbDesde.TabIndex = 0
        Me.lbDesde.Text = "Desde:"
        '
        'cbFechaDesde
        '
        Me.cbFechaDesde.BackColor = System.Drawing.Color.White
        Me.cbFechaDesde.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
        Me.cbFechaDesde.DropDownCalendar.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.None
        Me.cbFechaDesde.DropDownCalendar.DayOfWeekAbbreviation = Janus.Windows.CalendarCombo.DayOfWeekAbbreviation.UseAbbreviatedName
        Me.cbFechaDesde.DropDownCalendar.DaysFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.cbFechaDesde.DropDownCalendar.DaysFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.cbFechaDesde.DropDownCalendar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbFechaDesde.DropDownCalendar.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.cbFechaDesde.DropDownCalendar.HeaderAppearance = Janus.Windows.CalendarCombo.ButtonAppearance.PopUp
        Me.cbFechaDesde.DropDownCalendar.HeaderFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.cbFechaDesde.DropDownCalendar.HeaderFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.cbFechaDesde.DropDownCalendar.Name = ""
        Me.cbFechaDesde.DropDownCalendar.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaDesde.DropDownCalendar.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaDesde.DropDownCalendar.TodayRectColor = System.Drawing.Color.DarkCyan
        Me.cbFechaDesde.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.cbFechaDesde.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.cbFechaDesde.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbFechaDesde.Location = New System.Drawing.Point(64, 11)
        Me.cbFechaDesde.Name = "cbFechaDesde"
        Me.cbFechaDesde.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaDesde.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaDesde.SecondIncrement = 10
        Me.cbFechaDesde.Size = New System.Drawing.Size(140, 26)
        Me.cbFechaDesde.TabIndex = 1
        Me.cbFechaDesde.TodayButtonText = "Hoy"
        Me.cbFechaDesde.UseCompatibleTextRendering = False
        Me.cbFechaDesde.Value = New Date(2026, 1, 1, 0, 0, 0, 0)
        Me.cbFechaDesde.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.cbFechaDesde.YearIncrement = 10
        '
        'lbHasta
        '
        Me.lbHasta.AutoSize = True
        Me.lbHasta.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbHasta.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbHasta.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbHasta.ForeColor = System.Drawing.Color.White
        Me.lbHasta.Location = New System.Drawing.Point(14, 46)
        Me.lbHasta.Name = "lbHasta"
        Me.lbHasta.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbHasta.Size = New System.Drawing.Size(42, 21)
        Me.lbHasta.TabIndex = 2
        Me.lbHasta.Text = "Hasta:"
        '
        'cbFechaHasta
        '
        Me.cbFechaHasta.BackColor = System.Drawing.Color.White
        Me.cbFechaHasta.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
        Me.cbFechaHasta.DropDownCalendar.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.None
        Me.cbFechaHasta.DropDownCalendar.DayOfWeekAbbreviation = Janus.Windows.CalendarCombo.DayOfWeekAbbreviation.UseAbbreviatedName
        Me.cbFechaHasta.DropDownCalendar.DaysFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.cbFechaHasta.DropDownCalendar.DaysFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.cbFechaHasta.DropDownCalendar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbFechaHasta.DropDownCalendar.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.cbFechaHasta.DropDownCalendar.HeaderAppearance = Janus.Windows.CalendarCombo.ButtonAppearance.PopUp
        Me.cbFechaHasta.DropDownCalendar.HeaderFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.cbFechaHasta.DropDownCalendar.HeaderFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.cbFechaHasta.DropDownCalendar.Name = ""
        Me.cbFechaHasta.DropDownCalendar.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaHasta.DropDownCalendar.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaHasta.DropDownCalendar.TodayRectColor = System.Drawing.Color.DarkCyan
        Me.cbFechaHasta.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.cbFechaHasta.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.cbFechaHasta.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbFechaHasta.Location = New System.Drawing.Point(64, 44)
        Me.cbFechaHasta.Name = "cbFechaHasta"
        Me.cbFechaHasta.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaHasta.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaHasta.SecondIncrement = 10
        Me.cbFechaHasta.Size = New System.Drawing.Size(140, 26)
        Me.cbFechaHasta.TabIndex = 3
        Me.cbFechaHasta.TodayButtonText = "Hoy"
        Me.cbFechaHasta.UseCompatibleTextRendering = False
        Me.cbFechaHasta.Value = New Date(2026, 1, 1, 0, 0, 0, 0)
        Me.cbFechaHasta.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.cbFechaHasta.YearIncrement = 10
        '
        'lbModulo
        '
        Me.lbModulo.AutoSize = True
        Me.lbModulo.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbModulo.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbModulo.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbModulo.ForeColor = System.Drawing.Color.White
        Me.lbModulo.Location = New System.Drawing.Point(216, 13)
        Me.lbModulo.Name = "lbModulo"
        Me.lbModulo.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbModulo.Size = New System.Drawing.Size(50, 21)
        Me.lbModulo.TabIndex = 4
        Me.lbModulo.Text = "Modulo:"
        '
        'cbModulo
        '
        Me.cbModulo.BackColor = System.Drawing.Color.Azure
        Me.cbModulo.ColorScheme = ""
        Me.cbModulo.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbModulo.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbModulo.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        Me.cbModulo.FlatBorderColor = System.Drawing.Color.Black
        Me.cbModulo.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbModulo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbModulo.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbModulo.Location = New System.Drawing.Point(294, 11)
        Me.cbModulo.Name = "cbModulo"
        Me.cbModulo.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbModulo.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbModulo.SelectedIndex = -1
        Me.cbModulo.SelectedItem = Nothing
        Me.cbModulo.Size = New System.Drawing.Size(150, 26)
        Me.cbModulo.TabIndex = 5
        Me.cbModulo.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'lbOperacion
        '
        Me.lbOperacion.AutoSize = True
        Me.lbOperacion.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbOperacion.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbOperacion.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbOperacion.ForeColor = System.Drawing.Color.White
        Me.lbOperacion.Location = New System.Drawing.Point(216, 46)
        Me.lbOperacion.Name = "lbOperacion"
        Me.lbOperacion.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbOperacion.Size = New System.Drawing.Size(64, 21)
        Me.lbOperacion.TabIndex = 6
        Me.lbOperacion.Text = "Operacion:"
        '
        'cbOperacion
        '
        Me.cbOperacion.BackColor = System.Drawing.Color.Azure
        Me.cbOperacion.ColorScheme = ""
        Me.cbOperacion.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbOperacion.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbOperacion.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        Me.cbOperacion.FlatBorderColor = System.Drawing.Color.Black
        Me.cbOperacion.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbOperacion.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbOperacion.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbOperacion.Location = New System.Drawing.Point(294, 44)
        Me.cbOperacion.Name = "cbOperacion"
        Me.cbOperacion.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbOperacion.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbOperacion.SelectedIndex = -1
        Me.cbOperacion.SelectedItem = Nothing
        Me.cbOperacion.Size = New System.Drawing.Size(150, 26)
        Me.cbOperacion.TabIndex = 7
        Me.cbOperacion.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'lbUsuario
        '
        Me.lbUsuario.AutoSize = True
        Me.lbUsuario.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbUsuario.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbUsuario.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbUsuario.ForeColor = System.Drawing.Color.White
        Me.lbUsuario.Location = New System.Drawing.Point(460, 13)
        Me.lbUsuario.Name = "lbUsuario"
        Me.lbUsuario.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbUsuario.Size = New System.Drawing.Size(50, 21)
        Me.lbUsuario.TabIndex = 8
        Me.lbUsuario.Text = "Usuario:"
        '
        'cbUsuario
        '
        Me.cbUsuario.BackColor = System.Drawing.Color.Azure
        Me.cbUsuario.ColorScheme = ""
        Me.cbUsuario.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbUsuario.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbUsuario.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        Me.cbUsuario.FlatBorderColor = System.Drawing.Color.Black
        Me.cbUsuario.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbUsuario.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbUsuario.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbUsuario.Location = New System.Drawing.Point(546, 11)
        Me.cbUsuario.Name = "cbUsuario"
        Me.cbUsuario.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbUsuario.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbUsuario.SelectedIndex = -1
        Me.cbUsuario.SelectedItem = Nothing
        Me.cbUsuario.Size = New System.Drawing.Size(210, 26)
        Me.cbUsuario.TabIndex = 9
        Me.cbUsuario.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'lbDocumento
        '
        Me.lbDocumento.AutoSize = True
        Me.lbDocumento.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbDocumento.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbDocumento.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbDocumento.ForeColor = System.Drawing.Color.White
        Me.lbDocumento.Location = New System.Drawing.Point(460, 46)
        Me.lbDocumento.Name = "lbDocumento"
        Me.lbDocumento.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbDocumento.Size = New System.Drawing.Size(73, 21)
        Me.lbDocumento.TabIndex = 10
        Me.lbDocumento.Text = "Documento:"
        '
        'tbDocumento
        '
        Me.tbDocumento.BackColor = System.Drawing.Color.White
        '
        '
        '
        Me.tbDocumento.Border.Class = "TextBoxBorder"
        Me.tbDocumento.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbDocumento.DisabledBackColor = System.Drawing.Color.White
        Me.tbDocumento.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbDocumento.ForeColor = System.Drawing.Color.Black
        Me.tbDocumento.Location = New System.Drawing.Point(546, 46)
        Me.tbDocumento.MaxLength = 9
        Me.tbDocumento.Name = "tbDocumento"
        Me.tbDocumento.PreventEnterBeep = True
        Me.tbDocumento.Size = New System.Drawing.Size(120, 23)
        Me.tbDocumento.TabIndex = 11
        Me.tbDocumento.WatermarkText = "Nro. (opcional)"
        '
        'PanelBotones
        '
        Me.PanelBotones.BackColor = System.Drawing.Color.White
        Me.PanelBotones.Controls.Add(Me.btnBuscar)
        Me.PanelBotones.Controls.Add(Me.btnReporte)
        Me.PanelBotones.Dock = System.Windows.Forms.DockStyle.Right
        Me.PanelBotones.Location = New System.Drawing.Point(1040, 0)
        Me.PanelBotones.Name = "PanelBotones"
        Me.PanelBotones.Size = New System.Drawing.Size(240, 80)
        Me.PanelBotones.TabIndex = 12
        '
        'btnBuscar
        '
        Me.btnBuscar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnBuscar.BackColor = System.Drawing.Color.Transparent
        Me.btnBuscar.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnBuscar.DisabledImagesGrayScale = False
        Me.btnBuscar.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnBuscar.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBuscar.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnBuscar.ImageFixedSize = New System.Drawing.Size(32, 32)
        Me.btnBuscar.Location = New System.Drawing.Point(0, 0)
        Me.btnBuscar.Name = "btnBuscar"
        Me.btnBuscar.Size = New System.Drawing.Size(120, 80)
        Me.btnBuscar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnBuscar.TabIndex = 0
        Me.btnBuscar.Text = "Buscar (F5)"
        Me.btnBuscar.TextColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        '
        'btnReporte
        '
        Me.btnReporte.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnReporte.BackColor = System.Drawing.Color.Transparent
        Me.btnReporte.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnReporte.DisabledImagesGrayScale = False
        Me.btnReporte.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnReporte.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnReporte.Image = Global.TeVendo.My.Resources.Resources.printee
        Me.btnReporte.ImageFixedSize = New System.Drawing.Size(32, 32)
        Me.btnReporte.Location = New System.Drawing.Point(120, 0)
        Me.btnReporte.Name = "btnReporte"
        Me.btnReporte.Size = New System.Drawing.Size(120, 80)
        Me.btnReporte.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnReporte.TabIndex = 1
        Me.btnReporte.Text = "Reporte"
        Me.btnReporte.TextColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        '
        'PanelResumen
        '
        Me.PanelResumen.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelResumen.Controls.Add(Me.lbResumen)
        Me.PanelResumen.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelResumen.Location = New System.Drawing.Point(0, 107)
        Me.PanelResumen.Name = "PanelResumen"
        Me.PanelResumen.Padding = New System.Windows.Forms.Padding(10, 0, 10, 0)
        Me.PanelResumen.Size = New System.Drawing.Size(1280, 30)
        Me.PanelResumen.TabIndex = 2
        '
        'lbResumen
        '
        Me.lbResumen.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbResumen.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbResumen.ForeColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.lbResumen.Location = New System.Drawing.Point(10, 0)
        Me.lbResumen.Name = "lbResumen"
        Me.lbResumen.Size = New System.Drawing.Size(1260, 30)
        Me.lbResumen.TabIndex = 0
        Me.lbResumen.Text = "Elija las fechas y presione Buscar."
        Me.lbResumen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelPrincipal
        '
        Me.PanelPrincipal.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.PanelPrincipal.Controls.Add(Me.SplitContainer1)
        Me.PanelPrincipal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelPrincipal.Location = New System.Drawing.Point(0, 137)
        Me.PanelPrincipal.Name = "PanelPrincipal"
        Me.PanelPrincipal.Padding = New System.Windows.Forms.Padding(3)
        Me.PanelPrincipal.Size = New System.Drawing.Size(1280, 583)
        Me.PanelPrincipal.TabIndex = 3
        '
        'SplitContainer1
        '
        Me.SplitContainer1.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.BackColor = System.Drawing.Color.White
        Me.SplitContainer1.Panel1.Controls.Add(Me.grAuditoria)
        Me.SplitContainer1.Panel1MinSize = 300
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.Color.White
        Me.SplitContainer1.Panel2.Controls.Add(Me.PanelDetalle)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lbSinSeleccion)
        Me.SplitContainer1.Panel2MinSize = 300
        Me.SplitContainer1.Size = New System.Drawing.Size(1274, 577)
        Me.SplitContainer1.SplitterDistance = 620
        Me.SplitContainer1.SplitterWidth = 5
        Me.SplitContainer1.TabIndex = 0
        '
        'grAuditoria
        '
        Me.grAuditoria.BackColor = System.Drawing.Color.White
        Me.grAuditoria.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grAuditoria.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grAuditoria.GroupByBoxVisible = False
        Me.grAuditoria.Location = New System.Drawing.Point(0, 0)
        Me.grAuditoria.Name = "grAuditoria"
        Me.grAuditoria.Size = New System.Drawing.Size(620, 577)
        Me.grAuditoria.TabIndex = 0
        Me.grAuditoria.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'PanelDetalle
        '
        Me.PanelDetalle.BackColor = System.Drawing.Color.White
        Me.PanelDetalle.Controls.Add(Me.SplitContainer2)
        Me.PanelDetalle.Controls.Add(Me.PanelInfo)
        Me.PanelDetalle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetalle.Location = New System.Drawing.Point(0, 0)
        Me.PanelDetalle.Name = "PanelDetalle"
        Me.PanelDetalle.Padding = New System.Windows.Forms.Padding(6)
        Me.PanelDetalle.Size = New System.Drawing.Size(649, 577)
        Me.PanelDetalle.TabIndex = 0
        Me.PanelDetalle.Visible = False
        '
        'PanelInfo
        '
        Me.PanelInfo.BackColor = System.Drawing.Color.FromArgb(CType(CType(244, Byte), Integer), CType(CType(248, Byte), Integer), CType(CType(251, Byte), Integer))
        Me.PanelInfo.Controls.Add(Me.lnkUbicacion)
        Me.PanelInfo.Controls.Add(Me.lbNavegador)
        Me.PanelInfo.Controls.Add(Me.lbEquipo)
        Me.PanelInfo.Controls.Add(Me.lbQuien)
        Me.PanelInfo.Controls.Add(Me.lbTituloDetalle)
        Me.PanelInfo.Controls.Add(Me.btnImprimirRegistro)
        Me.PanelInfo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelInfo.Location = New System.Drawing.Point(6, 6)
        Me.PanelInfo.Name = "PanelInfo"
        Me.PanelInfo.Padding = New System.Windows.Forms.Padding(8, 4, 4, 4)
        Me.PanelInfo.Size = New System.Drawing.Size(637, 124)
        Me.PanelInfo.TabIndex = 0
        '
        'lbTituloDetalle
        '
        Me.lbTituloDetalle.AutoEllipsis = True
        Me.lbTituloDetalle.Dock = System.Windows.Forms.DockStyle.Top
        Me.lbTituloDetalle.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbTituloDetalle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.lbTituloDetalle.Location = New System.Drawing.Point(8, 4)
        Me.lbTituloDetalle.Name = "lbTituloDetalle"
        Me.lbTituloDetalle.Size = New System.Drawing.Size(485, 28)
        Me.lbTituloDetalle.TabIndex = 0
        Me.lbTituloDetalle.Text = "-"
        Me.lbTituloDetalle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbQuien
        '
        Me.lbQuien.AutoEllipsis = True
        Me.lbQuien.Dock = System.Windows.Forms.DockStyle.Top
        Me.lbQuien.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbQuien.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbQuien.Location = New System.Drawing.Point(8, 32)
        Me.lbQuien.Name = "lbQuien"
        Me.lbQuien.Size = New System.Drawing.Size(485, 21)
        Me.lbQuien.TabIndex = 1
        Me.lbQuien.Text = "-"
        Me.lbQuien.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbEquipo
        '
        Me.lbEquipo.AutoEllipsis = True
        Me.lbEquipo.Dock = System.Windows.Forms.DockStyle.Top
        Me.lbEquipo.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbEquipo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbEquipo.Location = New System.Drawing.Point(8, 53)
        Me.lbEquipo.Name = "lbEquipo"
        Me.lbEquipo.Size = New System.Drawing.Size(485, 21)
        Me.lbEquipo.TabIndex = 2
        Me.lbEquipo.Text = "-"
        Me.lbEquipo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbNavegador
        '
        Me.lbNavegador.AutoEllipsis = True
        Me.lbNavegador.Dock = System.Windows.Forms.DockStyle.Top
        Me.lbNavegador.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbNavegador.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbNavegador.Location = New System.Drawing.Point(8, 74)
        Me.lbNavegador.Name = "lbNavegador"
        Me.lbNavegador.Size = New System.Drawing.Size(485, 21)
        Me.lbNavegador.TabIndex = 3
        Me.lbNavegador.Text = "-"
        Me.lbNavegador.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lnkUbicacion
        '
        Me.lnkUbicacion.Dock = System.Windows.Forms.DockStyle.Top
        Me.lnkUbicacion.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lnkUbicacion.LinkColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.lnkUbicacion.Location = New System.Drawing.Point(8, 95)
        Me.lnkUbicacion.Name = "lnkUbicacion"
        Me.lnkUbicacion.Size = New System.Drawing.Size(485, 21)
        Me.lnkUbicacion.TabIndex = 4
        Me.lnkUbicacion.Text = "-"
        Me.lnkUbicacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnImprimirRegistro
        '
        Me.btnImprimirRegistro.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnImprimirRegistro.BackColor = System.Drawing.Color.Transparent
        Me.btnImprimirRegistro.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnImprimirRegistro.DisabledImagesGrayScale = False
        Me.btnImprimirRegistro.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnImprimirRegistro.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnImprimirRegistro.Image = Global.TeVendo.My.Resources.Resources.printee
        Me.btnImprimirRegistro.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.btnImprimirRegistro.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.btnImprimirRegistro.Location = New System.Drawing.Point(493, 4)
        Me.btnImprimirRegistro.Name = "btnImprimirRegistro"
        Me.btnImprimirRegistro.Size = New System.Drawing.Size(140, 116)
        Me.btnImprimirRegistro.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnImprimirRegistro.TabIndex = 5
        Me.btnImprimirRegistro.Text = "Imprimir registro"
        Me.btnImprimirRegistro.TextColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        '
        'lbCabecera
        '
        Me.lbCabecera.Dock = System.Windows.Forms.DockStyle.Top
        Me.lbCabecera.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbCabecera.ForeColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.lbCabecera.Location = New System.Drawing.Point(6, 130)
        Me.lbCabecera.Name = "lbCabecera"
        Me.lbCabecera.Size = New System.Drawing.Size(637, 26)
        Me.lbCabecera.TabIndex = 1
        Me.lbCabecera.Text = "CABECERA"
        Me.lbCabecera.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'grCabecera
        '
        Me.grCabecera.BackColor = System.Drawing.Color.White
        Me.grCabecera.Dock = System.Windows.Forms.DockStyle.Top
        Me.grCabecera.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabecera.GroupByBoxVisible = False
        Me.grCabecera.Location = New System.Drawing.Point(6, 156)
        Me.grCabecera.Name = "grCabecera"
        Me.grCabecera.Size = New System.Drawing.Size(637, 250)
        Me.grCabecera.TabIndex = 2
        Me.grCabecera.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'lbDetalleResumen
        '
        Me.lbDetalleResumen.Dock = System.Windows.Forms.DockStyle.Top
        Me.lbDetalleResumen.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbDetalleResumen.ForeColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.lbDetalleResumen.Location = New System.Drawing.Point(6, 346)
        Me.lbDetalleResumen.Name = "lbDetalleResumen"
        Me.lbDetalleResumen.Size = New System.Drawing.Size(637, 26)
        Me.lbDetalleResumen.TabIndex = 3
        Me.lbDetalleResumen.Text = "DETALLE"
        Me.lbDetalleResumen.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'grDetalle
        '
        Me.grDetalle.BackColor = System.Drawing.Color.White
        Me.grDetalle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grDetalle.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.GroupByBoxVisible = False
        Me.grDetalle.Location = New System.Drawing.Point(6, 372)
        Me.grDetalle.Name = "grDetalle"
        Me.grDetalle.Size = New System.Drawing.Size(637, 199)
        Me.grDetalle.TabIndex = 4
        Me.grDetalle.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'lbSinSeleccion
        '
        Me.lbSinSeleccion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbSinSeleccion.Font = New System.Drawing.Font("Calibri", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbSinSeleccion.ForeColor = System.Drawing.Color.Gray
        Me.lbSinSeleccion.Location = New System.Drawing.Point(0, 0)
        Me.lbSinSeleccion.Name = "lbSinSeleccion"
        Me.lbSinSeleccion.Size = New System.Drawing.Size(649, 577)
        Me.lbSinSeleccion.TabIndex = 1
        Me.lbSinSeleccion.Text = "Seleccione un registro de la lista para ver quien, cuando, desde donde y que cambio (antes / despues)."
        Me.lbSinSeleccion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Tec_AuditoriaRegistros
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1280, 720)
        Me.Controls.Add(Me.PanelPrincipal)
        Me.Controls.Add(Me.PanelResumen)
        Me.Controls.Add(Me.PanelFiltros)
        Me.Controls.Add(Me.PanelTitulo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "Tec_AuditoriaRegistros"
        Me.Text = "AUDITORIA DE REGISTROS"
        Me.PanelTitulo.ResumeLayout(False)
        Me.PanelTitulo2.ResumeLayout(False)
        CType(Me.PictureBoxTitulo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelFiltros.ResumeLayout(False)
        Me.PanelFiltros.PerformLayout()
        CType(Me.cbOperacion, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cbModulo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cbUsuario, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelBotones.ResumeLayout(False)
        Me.PanelResumen.ResumeLayout(False)
        Me.PanelPrincipal.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.grAuditoria, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelDetalle.ResumeLayout(False)
        CType(Me.grDetalle, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.grCabecera, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelInfo.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelTitulo As Panel
    Friend WithEvents PanelTitulo2 As Panel
    Friend WithEvents lbTitulo As Label
    Friend WithEvents PanelSeparador As Panel
    Friend WithEvents PictureBoxTitulo As PictureBox
    Friend WithEvents PanelFiltros As Panel
    Friend WithEvents lbDesde As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbFechaDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents lbHasta As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbFechaHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents lbModulo As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbModulo As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents lbOperacion As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbOperacion As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents lbUsuario As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbUsuario As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents lbDocumento As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbDocumento As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents PanelBotones As Panel
    Friend WithEvents btnBuscar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnReporte As DevComponents.DotNetBar.ButtonX
    Friend WithEvents PanelResumen As Panel
    Friend WithEvents lbResumen As Label
    Friend WithEvents PanelPrincipal As Panel
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents grAuditoria As Janus.Windows.GridEX.GridEX
    Friend WithEvents PanelDetalle As Panel
    Friend WithEvents PanelInfo As Panel
    Friend WithEvents lbTituloDetalle As Label
    Friend WithEvents lbQuien As Label
    Friend WithEvents lbEquipo As Label
    Friend WithEvents lbNavegador As Label
    Friend WithEvents lnkUbicacion As LinkLabel
    Friend WithEvents btnImprimirRegistro As DevComponents.DotNetBar.ButtonX
    Friend WithEvents lbCabecera As Label
    Friend WithEvents grCabecera As Janus.Windows.GridEX.GridEX
    Friend WithEvents lbDetalleResumen As Label
    Friend WithEvents grDetalle As Janus.Windows.GridEX.GridEX
    Friend WithEvents lbSinSeleccion As Label
End Class
