<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_AuditoriaVentas
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
        Me.components = New System.ComponentModel.Container()
        Me.PanelHeader = New System.Windows.Forms.Panel()
        Me.PanelHeaderInner = New System.Windows.Forms.Panel()
        Me.LblHeader = New System.Windows.Forms.Label()
        Me.PicHeader = New System.Windows.Forms.PictureBox()
        Me.PanelFiltro = New System.Windows.Forms.Panel()
        Me.btnVerComparacion = New DevComponents.DotNetBar.ButtonX()
        Me.btnFiltrar = New DevComponents.DotNetBar.ButtonX()
        Me.cbAccion = New System.Windows.Forms.ComboBox()
        Me.LabelX4 = New DevComponents.DotNetBar.LabelX()
        Me.tbUsuario = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.LabelX3 = New DevComponents.DotNetBar.LabelX()
        Me.tbHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.tbDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.PanelEventos = New System.Windows.Forms.Panel()
        Me.grEventos = New Janus.Windows.GridEX.GridEX()
        Me.PanelComparacion = New System.Windows.Forms.Panel()
        Me.PanelAntes = New System.Windows.Forms.Panel()
        Me.PanelDetAntes = New System.Windows.Forms.Panel()
        Me.grDetAntes = New Janus.Windows.GridEX.GridEX()
        Me.PanelCabAntes = New System.Windows.Forms.Panel()
        Me.grCabAntes = New Janus.Windows.GridEX.GridEX()
        Me.LblAntes = New System.Windows.Forms.Label()
        Me.PanelDespues = New System.Windows.Forms.Panel()
        Me.PanelDetDespues = New System.Windows.Forms.Panel()
        Me.grDetDespues = New Janus.Windows.GridEX.GridEX()
        Me.PanelCabDespues = New System.Windows.Forms.Panel()
        Me.grCabDespues = New Janus.Windows.GridEX.GridEX()
        Me.LblDespues = New System.Windows.Forms.Label()
        Me.PanelHeader.SuspendLayout()
        Me.PanelHeaderInner.SuspendLayout()
        CType(Me.PicHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelFiltro.SuspendLayout()
        Me.PanelEventos.SuspendLayout()
        CType(Me.grEventos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelComparacion.SuspendLayout()
        Me.PanelAntes.SuspendLayout()
        Me.PanelDetAntes.SuspendLayout()
        CType(Me.grDetAntes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelCabAntes.SuspendLayout()
        CType(Me.grCabAntes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelDespues.SuspendLayout()
        Me.PanelDetDespues.SuspendLayout()
        CType(Me.grDetDespues, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelCabDespues.SuspendLayout()
        CType(Me.grCabDespues, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelHeader
        '
        Me.PanelHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.PanelHeader.Controls.Add(Me.PanelHeaderInner)
        Me.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelHeader.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelHeader.Name = "PanelHeader"
        Me.PanelHeader.Padding = New System.Windows.Forms.Padding(1)
        Me.PanelHeader.Size = New System.Drawing.Size(1360, 36)
        Me.PanelHeader.TabIndex = 0
        '
        'PanelHeaderInner
        '
        Me.PanelHeaderInner.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelHeaderInner.Controls.Add(Me.LblHeader)
        Me.PanelHeaderInner.Controls.Add(Me.PicHeader)
        Me.PanelHeaderInner.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelHeaderInner.Location = New System.Drawing.Point(1, 1)
        Me.PanelHeaderInner.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelHeaderInner.Name = "PanelHeaderInner"
        Me.PanelHeaderInner.Size = New System.Drawing.Size(1358, 34)
        Me.PanelHeaderInner.TabIndex = 0
        '
        'PicHeader
        '
        Me.PicHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PicHeader.Dock = System.Windows.Forms.DockStyle.Left
        Me.PicHeader.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PicHeader.Location = New System.Drawing.Point(0, 0)
        Me.PicHeader.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PicHeader.Name = "PicHeader"
        Me.PicHeader.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PicHeader.Size = New System.Drawing.Size(59, 34)
        Me.PicHeader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicHeader.TabIndex = 0
        Me.PicHeader.TabStop = False
        '
        'LblHeader
        '
        Me.LblHeader.Dock = System.Windows.Forms.DockStyle.Left
        Me.LblHeader.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblHeader.ForeColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.LblHeader.Location = New System.Drawing.Point(59, 0)
        Me.LblHeader.Name = "LblHeader"
        Me.LblHeader.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.LblHeader.Size = New System.Drawing.Size(320, 34)
        Me.LblHeader.TabIndex = 1
        Me.LblHeader.Text = "AUDITORIA DE VENTAS"
        Me.LblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelFiltro
        '
        Me.PanelFiltro.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelFiltro.Controls.Add(Me.btnVerComparacion)
        Me.PanelFiltro.Controls.Add(Me.btnFiltrar)
        Me.PanelFiltro.Controls.Add(Me.cbAccion)
        Me.PanelFiltro.Controls.Add(Me.LabelX4)
        Me.PanelFiltro.Controls.Add(Me.tbUsuario)
        Me.PanelFiltro.Controls.Add(Me.LabelX3)
        Me.PanelFiltro.Controls.Add(Me.tbHasta)
        Me.PanelFiltro.Controls.Add(Me.LabelX2)
        Me.PanelFiltro.Controls.Add(Me.tbDesde)
        Me.PanelFiltro.Controls.Add(Me.LabelX1)
        Me.PanelFiltro.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFiltro.Location = New System.Drawing.Point(0, 36)
        Me.PanelFiltro.Name = "PanelFiltro"
        Me.PanelFiltro.Padding = New System.Windows.Forms.Padding(8)
        Me.PanelFiltro.Size = New System.Drawing.Size(1360, 55)
        Me.PanelFiltro.TabIndex = 1
        '
        'LabelX1
        '
        Me.LabelX1.AutoSize = True
        Me.LabelX1.BackColor = System.Drawing.Color.Transparent
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX1.Location = New System.Drawing.Point(8, 18)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(60, 21)
        Me.LabelX1.TabIndex = 100
        Me.LabelX1.Text = "Desde:"
        '
        'tbDesde
        '
        Me.tbDesde.BackColor = System.Drawing.Color.White
        Me.tbDesde.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        Me.tbDesde.DropDownCalendar.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.None
        Me.tbDesde.DropDownCalendar.DayOfWeekAbbreviation = Janus.Windows.CalendarCombo.DayOfWeekAbbreviation.UseAbbreviatedName
        Me.tbDesde.DropDownCalendar.DaysFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbDesde.DropDownCalendar.DaysFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbDesde.DropDownCalendar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbDesde.DropDownCalendar.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbDesde.DropDownCalendar.HeaderAppearance = Janus.Windows.CalendarCombo.ButtonAppearance.PopUp
        Me.tbDesde.DropDownCalendar.HeaderFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbDesde.DropDownCalendar.HeaderFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbDesde.DropDownCalendar.Name = ""
        Me.tbDesde.DropDownCalendar.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbDesde.DropDownCalendar.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbDesde.DropDownCalendar.TodayRectColor = System.Drawing.Color.DarkCyan
        Me.tbDesde.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbDesde.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.tbDesde.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbDesde.Location = New System.Drawing.Point(70, 14)
        Me.tbDesde.Name = "tbDesde"
        Me.tbDesde.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbDesde.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbDesde.SecondIncrement = 10
        Me.tbDesde.Size = New System.Drawing.Size(157, 26)
        Me.tbDesde.TabIndex = 101
        Me.tbDesde.TodayButtonText = "Hoy"
        Me.tbDesde.UseCompatibleTextRendering = False
        Me.tbDesde.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.tbDesde.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbDesde.YearIncrement = 10
        '
        'LabelX2
        '
        Me.LabelX2.AutoSize = True
        Me.LabelX2.BackColor = System.Drawing.Color.Transparent
        Me.LabelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX2.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX2.Location = New System.Drawing.Point(240, 18)
        Me.LabelX2.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX2.Size = New System.Drawing.Size(55, 21)
        Me.LabelX2.TabIndex = 102
        Me.LabelX2.Text = "Hasta:"
        '
        'tbHasta
        '
        Me.tbHasta.BackColor = System.Drawing.Color.White
        Me.tbHasta.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        Me.tbHasta.DropDownCalendar.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.None
        Me.tbHasta.DropDownCalendar.DayOfWeekAbbreviation = Janus.Windows.CalendarCombo.DayOfWeekAbbreviation.UseAbbreviatedName
        Me.tbHasta.DropDownCalendar.DaysFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbHasta.DropDownCalendar.DaysFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbHasta.DropDownCalendar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbHasta.DropDownCalendar.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbHasta.DropDownCalendar.HeaderAppearance = Janus.Windows.CalendarCombo.ButtonAppearance.PopUp
        Me.tbHasta.DropDownCalendar.HeaderFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbHasta.DropDownCalendar.HeaderFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbHasta.DropDownCalendar.Name = ""
        Me.tbHasta.DropDownCalendar.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbHasta.DropDownCalendar.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbHasta.DropDownCalendar.TodayRectColor = System.Drawing.Color.DarkCyan
        Me.tbHasta.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbHasta.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.tbHasta.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbHasta.Location = New System.Drawing.Point(300, 14)
        Me.tbHasta.Name = "tbHasta"
        Me.tbHasta.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbHasta.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbHasta.SecondIncrement = 10
        Me.tbHasta.Size = New System.Drawing.Size(157, 26)
        Me.tbHasta.TabIndex = 103
        Me.tbHasta.TodayButtonText = "Hoy"
        Me.tbHasta.UseCompatibleTextRendering = False
        Me.tbHasta.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.tbHasta.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbHasta.YearIncrement = 10
        '
        'LabelX3
        '
        Me.LabelX3.AutoSize = True
        Me.LabelX3.BackColor = System.Drawing.Color.Transparent
        Me.LabelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX3.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX3.Location = New System.Drawing.Point(470, 18)
        Me.LabelX3.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX3.Name = "LabelX3"
        Me.LabelX3.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX3.Size = New System.Drawing.Size(65, 21)
        Me.LabelX3.TabIndex = 104
        Me.LabelX3.Text = "Usuario:"
        '
        'tbUsuario
        '
        Me.tbUsuario.Border.Class = "TextBoxBorder"
        Me.tbUsuario.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbUsuario.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbUsuario.Location = New System.Drawing.Point(545, 12)
        Me.tbUsuario.Margin = New System.Windows.Forms.Padding(4)
        Me.tbUsuario.Name = "tbUsuario"
        Me.tbUsuario.PreventEnterBeep = True
        Me.tbUsuario.Size = New System.Drawing.Size(160, 26)
        Me.tbUsuario.TabIndex = 105
        '
        'LabelX4
        '
        Me.LabelX4.AutoSize = True
        Me.LabelX4.BackColor = System.Drawing.Color.Transparent
        Me.LabelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX4.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX4.Location = New System.Drawing.Point(720, 18)
        Me.LabelX4.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX4.Name = "LabelX4"
        Me.LabelX4.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX4.Size = New System.Drawing.Size(60, 21)
        Me.LabelX4.TabIndex = 106
        Me.LabelX4.Text = "Accion:"
        '
        'cbAccion
        '
        Me.cbAccion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbAccion.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbAccion.FormattingEnabled = True
        Me.cbAccion.Items.AddRange(New Object() {"", "CREAR", "MODIFICAR", "ELIMINAR"})
        Me.cbAccion.Location = New System.Drawing.Point(790, 14)
        Me.cbAccion.Name = "cbAccion"
        Me.cbAccion.Size = New System.Drawing.Size(140, 24)
        Me.cbAccion.TabIndex = 107
        '
        'btnFiltrar
        '
        Me.btnFiltrar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnFiltrar.BackColor = System.Drawing.Color.DarkOrange
        Me.btnFiltrar.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnFiltrar.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFiltrar.Location = New System.Drawing.Point(950, 10)
        Me.btnFiltrar.Margin = New System.Windows.Forms.Padding(4)
        Me.btnFiltrar.Name = "btnFiltrar"
        Me.btnFiltrar.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnFiltrar.Size = New System.Drawing.Size(140, 34)
        Me.btnFiltrar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnFiltrar.Symbol = ""
        Me.btnFiltrar.SymbolColor = System.Drawing.Color.White
        Me.btnFiltrar.SymbolSize = 15.0!
        Me.btnFiltrar.TabIndex = 108
        Me.btnFiltrar.Text = "Filtrar"
        Me.btnFiltrar.TextColor = System.Drawing.Color.White
        '
        'btnVerComparacion
        '
        Me.btnVerComparacion.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnVerComparacion.BackColor = System.Drawing.Color.SteelBlue
        Me.btnVerComparacion.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnVerComparacion.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnVerComparacion.Location = New System.Drawing.Point(1105, 10)
        Me.btnVerComparacion.Margin = New System.Windows.Forms.Padding(4)
        Me.btnVerComparacion.Name = "btnVerComparacion"
        Me.btnVerComparacion.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnVerComparacion.Size = New System.Drawing.Size(140, 34)
        Me.btnVerComparacion.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnVerComparacion.Symbol = ""
        Me.btnVerComparacion.SymbolColor = System.Drawing.Color.White
        Me.btnVerComparacion.SymbolSize = 15.0!
        Me.btnVerComparacion.TabIndex = 109
        Me.btnVerComparacion.Text = "Ver Comparacion"
        Me.btnVerComparacion.TextColor = System.Drawing.Color.White
        '
        'PanelEventos
        '
        Me.PanelEventos.Controls.Add(Me.grEventos)
        Me.PanelEventos.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelEventos.Location = New System.Drawing.Point(0, 91)
        Me.PanelEventos.Name = "PanelEventos"
        Me.PanelEventos.Padding = New System.Windows.Forms.Padding(5)
        Me.PanelEventos.Size = New System.Drawing.Size(1360, 260)
        Me.PanelEventos.TabIndex = 2
        '
        'grEventos
        '
        Me.grEventos.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grEventos.AlternatingColors = True
        Me.grEventos.BackColor = System.Drawing.Color.White
        Me.grEventos.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grEventos.ColumnAutoResize = True
        Me.grEventos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grEventos.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grEventos.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grEventos.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grEventos.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grEventos.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grEventos.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grEventos.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grEventos.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grEventos.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grEventos.HeaderFormatStyle.Alpha = 0
        Me.grEventos.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grEventos.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grEventos.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grEventos.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grEventos.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grEventos.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grEventos.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grEventos.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grEventos.Location = New System.Drawing.Point(5, 5)
        Me.grEventos.Margin = New System.Windows.Forms.Padding(4)
        Me.grEventos.Name = "grEventos"
        Me.grEventos.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grEventos.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grEventos.RecordNavigator = True
        Me.grEventos.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grEventos.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grEventos.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grEventos.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grEventos.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grEventos.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grEventos.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grEventos.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grEventos.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grEventos.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grEventos.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grEventos.Size = New System.Drawing.Size(1350, 250)
        Me.grEventos.TabIndex = 1
        Me.grEventos.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grEventos.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grEventos.TableSpacing = 9
        Me.grEventos.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grEventos.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grEventos.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grEventos.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'PanelComparacion
        '
        Me.PanelComparacion.Controls.Add(Me.PanelAntes)
        Me.PanelComparacion.Controls.Add(Me.PanelDespues)
        Me.PanelComparacion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelComparacion.Location = New System.Drawing.Point(0, 351)
        Me.PanelComparacion.Name = "PanelComparacion"
        Me.PanelComparacion.Size = New System.Drawing.Size(1360, 313)
        Me.PanelComparacion.TabIndex = 3
        '
        'PanelDespues
        '
        Me.PanelDespues.Controls.Add(Me.PanelDetDespues)
        Me.PanelDespues.Controls.Add(Me.PanelCabDespues)
        Me.PanelDespues.Controls.Add(Me.LblDespues)
        Me.PanelDespues.Dock = System.Windows.Forms.DockStyle.Right
        Me.PanelDespues.Location = New System.Drawing.Point(682, 0)
        Me.PanelDespues.Name = "PanelDespues"
        Me.PanelDespues.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.PanelDespues.Size = New System.Drawing.Size(678, 313)
        Me.PanelDespues.TabIndex = 1
        '
        'LblDespues
        '
        Me.LblDespues.BackColor = System.Drawing.Color.MidnightBlue
        Me.LblDespues.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblDespues.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblDespues.ForeColor = System.Drawing.Color.White
        Me.LblDespues.Location = New System.Drawing.Point(0, 0)
        Me.LblDespues.Name = "LblDespues"
        Me.LblDespues.Size = New System.Drawing.Size(600, 24)
        Me.LblDespues.TabIndex = 0
        Me.LblDespues.Text = "  DESPUES"
        Me.LblDespues.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelCabDespues
        '
        Me.PanelCabDespues.Controls.Add(Me.grCabDespues)
        Me.PanelCabDespues.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelCabDespues.Location = New System.Drawing.Point(0, 24)
        Me.PanelCabDespues.Name = "PanelCabDespues"
        Me.PanelCabDespues.Padding = New System.Windows.Forms.Padding(2)
        Me.PanelCabDespues.Size = New System.Drawing.Size(678, 110)
        Me.PanelCabDespues.TabIndex = 1
        '
        'grCabDespues
        '
        Me.grCabDespues.AlternatingColors = True
        Me.grCabDespues.BackColor = System.Drawing.Color.White
        Me.grCabDespues.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grCabDespues.ColumnAutoResize = True
        Me.grCabDespues.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grCabDespues.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grCabDespues.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grCabDespues.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabDespues.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grCabDespues.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grCabDespues.FocusCellFormatStyle.BackColor = System.Drawing.Color.White
        Me.grCabDespues.FocusCellFormatStyle.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabDespues.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabDespues.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grCabDespues.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grCabDespues.HeaderFormatStyle.Alpha = 0
        Me.grCabDespues.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grCabDespues.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grCabDespues.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grCabDespues.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grCabDespues.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grCabDespues.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabDespues.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grCabDespues.Location = New System.Drawing.Point(2, 2)
        Me.grCabDespues.Margin = New System.Windows.Forms.Padding(4)
        Me.grCabDespues.Name = "grCabDespues"
        Me.grCabDespues.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grCabDespues.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grCabDespues.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabDespues.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grCabDespues.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grCabDespues.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grCabDespues.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grCabDespues.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grCabDespues.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabDespues.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grCabDespues.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grCabDespues.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabDespues.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grCabDespues.Size = New System.Drawing.Size(674, 106)
        Me.grCabDespues.TabIndex = 1
        Me.grCabDespues.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grCabDespues.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grCabDespues.TableSpacing = 9
        Me.grCabDespues.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grCabDespues.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grCabDespues.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grCabDespues.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'PanelDetDespues
        '
        Me.PanelDetDespues.Controls.Add(Me.grDetDespues)
        Me.PanelDetDespues.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetDespues.Location = New System.Drawing.Point(0, 134)
        Me.PanelDetDespues.Name = "PanelDetDespues"
        Me.PanelDetDespues.Padding = New System.Windows.Forms.Padding(2)
        Me.PanelDetDespues.Size = New System.Drawing.Size(678, 179)
        Me.PanelDetDespues.TabIndex = 2
        '
        'grDetDespues
        '
        Me.grDetDespues.AlternatingColors = True
        Me.grDetDespues.BackColor = System.Drawing.Color.White
        Me.grDetDespues.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grDetDespues.ColumnAutoResize = True
        Me.grDetDespues.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grDetDespues.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grDetDespues.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grDetDespues.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetDespues.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetDespues.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetDespues.FocusCellFormatStyle.BackColor = System.Drawing.Color.White
        Me.grDetDespues.FocusCellFormatStyle.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetDespues.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetDespues.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grDetDespues.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grDetDespues.HeaderFormatStyle.Alpha = 0
        Me.grDetDespues.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grDetDespues.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetDespues.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grDetDespues.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetDespues.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetDespues.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetDespues.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grDetDespues.Location = New System.Drawing.Point(2, 2)
        Me.grDetDespues.Margin = New System.Windows.Forms.Padding(4)
        Me.grDetDespues.Name = "grDetDespues"
        Me.grDetDespues.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grDetDespues.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grDetDespues.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetDespues.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetDespues.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grDetDespues.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetDespues.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetDespues.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grDetDespues.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetDespues.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grDetDespues.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grDetDespues.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetDespues.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grDetDespues.Size = New System.Drawing.Size(674, 175)
        Me.grDetDespues.TabIndex = 2
        Me.grDetDespues.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grDetDespues.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetDespues.TableSpacing = 9
        Me.grDetDespues.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grDetDespues.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grDetDespues.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grDetDespues.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'PanelAntes
        '
        Me.PanelAntes.Controls.Add(Me.PanelDetAntes)
        Me.PanelAntes.Controls.Add(Me.PanelCabAntes)
        Me.PanelAntes.Controls.Add(Me.LblAntes)
        Me.PanelAntes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelAntes.Location = New System.Drawing.Point(0, 0)
        Me.PanelAntes.Name = "PanelAntes"
        Me.PanelAntes.Padding = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.PanelAntes.Size = New System.Drawing.Size(682, 313)
        Me.PanelAntes.TabIndex = 0
        '
        'LblAntes
        '
        Me.LblAntes.BackColor = System.Drawing.Color.MidnightBlue
        Me.LblAntes.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblAntes.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblAntes.ForeColor = System.Drawing.Color.White
        Me.LblAntes.Location = New System.Drawing.Point(0, 0)
        Me.LblAntes.Name = "LblAntes"
        Me.LblAntes.Size = New System.Drawing.Size(600, 24)
        Me.LblAntes.TabIndex = 0
        Me.LblAntes.Text = "  ANTES"
        Me.LblAntes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PanelCabAntes
        '
        Me.PanelCabAntes.Controls.Add(Me.grCabAntes)
        Me.PanelCabAntes.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelCabAntes.Location = New System.Drawing.Point(0, 24)
        Me.PanelCabAntes.Name = "PanelCabAntes"
        Me.PanelCabAntes.Padding = New System.Windows.Forms.Padding(2)
        Me.PanelCabAntes.Size = New System.Drawing.Size(682, 110)
        Me.PanelCabAntes.TabIndex = 1
        '
        'grCabAntes
        '
        Me.grCabAntes.AlternatingColors = True
        Me.grCabAntes.BackColor = System.Drawing.Color.White
        Me.grCabAntes.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grCabAntes.ColumnAutoResize = True
        Me.grCabAntes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grCabAntes.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grCabAntes.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grCabAntes.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabAntes.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grCabAntes.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grCabAntes.FocusCellFormatStyle.BackColor = System.Drawing.Color.White
        Me.grCabAntes.FocusCellFormatStyle.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabAntes.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabAntes.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grCabAntes.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grCabAntes.HeaderFormatStyle.Alpha = 0
        Me.grCabAntes.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grCabAntes.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grCabAntes.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grCabAntes.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grCabAntes.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grCabAntes.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabAntes.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grCabAntes.Location = New System.Drawing.Point(2, 2)
        Me.grCabAntes.Margin = New System.Windows.Forms.Padding(4)
        Me.grCabAntes.Name = "grCabAntes"
        Me.grCabAntes.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grCabAntes.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grCabAntes.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabAntes.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grCabAntes.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grCabAntes.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grCabAntes.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grCabAntes.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grCabAntes.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabAntes.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grCabAntes.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grCabAntes.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCabAntes.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grCabAntes.Size = New System.Drawing.Size(678, 106)
        Me.grCabAntes.TabIndex = 1
        Me.grCabAntes.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grCabAntes.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grCabAntes.TableSpacing = 9
        Me.grCabAntes.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grCabAntes.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grCabAntes.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grCabAntes.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'PanelDetAntes
        '
        Me.PanelDetAntes.Controls.Add(Me.grDetAntes)
        Me.PanelDetAntes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetAntes.Location = New System.Drawing.Point(0, 134)
        Me.PanelDetAntes.Name = "PanelDetAntes"
        Me.PanelDetAntes.Padding = New System.Windows.Forms.Padding(2)
        Me.PanelDetAntes.Size = New System.Drawing.Size(682, 179)
        Me.PanelDetAntes.TabIndex = 2
        '
        'grDetAntes
        '
        Me.grDetAntes.AlternatingColors = True
        Me.grDetAntes.BackColor = System.Drawing.Color.White
        Me.grDetAntes.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grDetAntes.ColumnAutoResize = True
        Me.grDetAntes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grDetAntes.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grDetAntes.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grDetAntes.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetAntes.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetAntes.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetAntes.FocusCellFormatStyle.BackColor = System.Drawing.Color.White
        Me.grDetAntes.FocusCellFormatStyle.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetAntes.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetAntes.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grDetAntes.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grDetAntes.HeaderFormatStyle.Alpha = 0
        Me.grDetAntes.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grDetAntes.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetAntes.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grDetAntes.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetAntes.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetAntes.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetAntes.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grDetAntes.Location = New System.Drawing.Point(2, 2)
        Me.grDetAntes.Margin = New System.Windows.Forms.Padding(4)
        Me.grDetAntes.Name = "grDetAntes"
        Me.grDetAntes.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grDetAntes.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grDetAntes.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetAntes.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetAntes.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grDetAntes.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetAntes.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetAntes.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grDetAntes.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetAntes.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grDetAntes.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grDetAntes.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetAntes.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grDetAntes.Size = New System.Drawing.Size(678, 175)
        Me.grDetAntes.TabIndex = 2
        Me.grDetAntes.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grDetAntes.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetAntes.TableSpacing = 9
        Me.grDetAntes.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grDetAntes.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grDetAntes.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grDetAntes.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Tec_AuditoriaVentas
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1360, 664)
        Me.Controls.Add(Me.PanelComparacion)
        Me.Controls.Add(Me.PanelEventos)
        Me.Controls.Add(Me.PanelFiltro)
        Me.Controls.Add(Me.PanelHeader)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Tec_AuditoriaVentas"
        Me.Text = "Auditoria de Ventas"
        Me.PanelHeader.ResumeLayout(False)
        Me.PanelHeaderInner.ResumeLayout(False)
        CType(Me.PicHeader, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelFiltro.ResumeLayout(False)
        Me.PanelFiltro.PerformLayout()
        Me.PanelEventos.ResumeLayout(False)
        CType(Me.grEventos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelComparacion.ResumeLayout(False)
        Me.PanelAntes.ResumeLayout(False)
        Me.PanelDetAntes.ResumeLayout(False)
        CType(Me.grDetAntes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelCabAntes.ResumeLayout(False)
        CType(Me.grCabAntes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelDespues.ResumeLayout(False)
        Me.PanelDetDespues.ResumeLayout(False)
        CType(Me.grDetDespues, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelCabDespues.ResumeLayout(False)
        CType(Me.grCabDespues, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelHeader As Panel
    Friend WithEvents PanelHeaderInner As Panel
    Friend WithEvents LblHeader As Label
    Friend WithEvents PicHeader As PictureBox
    Friend WithEvents PanelFiltro As Panel
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX3 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbUsuario As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX4 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbAccion As System.Windows.Forms.ComboBox
    Friend WithEvents btnFiltrar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnVerComparacion As DevComponents.DotNetBar.ButtonX
    Friend WithEvents PanelEventos As Panel
    Friend WithEvents grEventos As Janus.Windows.GridEX.GridEX
    Friend WithEvents PanelComparacion As Panel
    Friend WithEvents PanelAntes As Panel
    Friend WithEvents LblAntes As Label
    Friend WithEvents PanelCabAntes As Panel
    Friend WithEvents grCabAntes As Janus.Windows.GridEX.GridEX
    Friend WithEvents PanelDetAntes As Panel
    Friend WithEvents grDetAntes As Janus.Windows.GridEX.GridEX
    Friend WithEvents PanelDespues As Panel
    Friend WithEvents LblDespues As Label
    Friend WithEvents PanelCabDespues As Panel
    Friend WithEvents grCabDespues As Janus.Windows.GridEX.GridEX
    Friend WithEvents PanelDetDespues As Panel
    Friend WithEvents grDetDespues As Janus.Windows.GridEX.GridEX
End Class
