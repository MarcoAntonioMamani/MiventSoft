<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_CajaGeneral
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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PanelSuperior = New System.Windows.Forms.Panel()
        Me.PanelLEft = New System.Windows.Forms.Panel()
        Me.Panel8 = New System.Windows.Forms.Panel()
        Me.PanelDatos = New System.Windows.Forms.Panel()
        Me.PanelMovimientos = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.grMovimientos = New Janus.Windows.GridEX.GridEX()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.PanelButton = New System.Windows.Forms.Panel()
        Me.PanelNavegacion = New System.Windows.Forms.Panel()
        Me.LabelX5 = New DevComponents.DotNetBar.LabelX()
        Me.tbSaldo = New Janus.Windows.GridEX.EditControls.NumericEditBox()
        Me.cbFechaHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX4 = New DevComponents.DotNetBar.LabelX()
        Me.cbFechaDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.PanelToolBar1 = New System.Windows.Forms.Panel()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.btGenerarKardex = New DevComponents.DotNetBar.ButtonX()
        Me.ButtonX2 = New DevComponents.DotNetBar.ButtonX()
        Me.Panel1.SuspendLayout()
        Me.PanelSuperior.SuspendLayout()
        Me.PanelLEft.SuspendLayout()
        Me.Panel8.SuspendLayout()
        Me.PanelDatos.SuspendLayout()
        Me.PanelMovimientos.SuspendLayout()
        Me.Panel5.SuspendLayout()
        CType(Me.grMovimientos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelButton.SuspendLayout()
        Me.PanelNavegacion.SuspendLayout()
        Me.PanelToolBar1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.PanelSuperior)
        Me.Panel1.Controls.Add(Me.PanelButton)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1345, 515)
        Me.Panel1.TabIndex = 5
        '
        'PanelSuperior
        '
        Me.PanelSuperior.Controls.Add(Me.PanelLEft)
        Me.PanelSuperior.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelSuperior.Location = New System.Drawing.Point(0, 0)
        Me.PanelSuperior.Name = "PanelSuperior"
        Me.PanelSuperior.Size = New System.Drawing.Size(1345, 432)
        Me.PanelSuperior.TabIndex = 1
        '
        'PanelLEft
        '
        Me.PanelLEft.Controls.Add(Me.Panel8)
        Me.PanelLEft.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelLEft.Location = New System.Drawing.Point(0, 0)
        Me.PanelLEft.Name = "PanelLEft"
        Me.PanelLEft.Size = New System.Drawing.Size(1345, 432)
        Me.PanelLEft.TabIndex = 0
        '
        'Panel8
        '
        Me.Panel8.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel8.Controls.Add(Me.PanelDatos)
        Me.Panel8.Controls.Add(Me.Panel10)
        Me.Panel8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel8.Location = New System.Drawing.Point(0, 0)
        Me.Panel8.Name = "Panel8"
        Me.Panel8.Padding = New System.Windows.Forms.Padding(3)
        Me.Panel8.Size = New System.Drawing.Size(1345, 432)
        Me.Panel8.TabIndex = 1
        '
        'PanelDatos
        '
        Me.PanelDatos.BackColor = System.Drawing.Color.White
        Me.PanelDatos.Controls.Add(Me.PanelMovimientos)
        Me.PanelDatos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDatos.Location = New System.Drawing.Point(3, 30)
        Me.PanelDatos.Name = "PanelDatos"
        Me.PanelDatos.Size = New System.Drawing.Size(1339, 399)
        Me.PanelDatos.TabIndex = 2
        '
        'PanelMovimientos
        '
        Me.PanelMovimientos.BackColor = System.Drawing.Color.White
        Me.PanelMovimientos.CanvasColor = System.Drawing.SystemColors.Control
        Me.PanelMovimientos.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.PanelMovimientos.Controls.Add(Me.Panel5)
        Me.PanelMovimientos.DisabledBackColor = System.Drawing.Color.Empty
        Me.PanelMovimientos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelMovimientos.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelMovimientos.Location = New System.Drawing.Point(0, 0)
        Me.PanelMovimientos.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelMovimientos.Name = "PanelMovimientos"
        Me.PanelMovimientos.Size = New System.Drawing.Size(1339, 399)
        '
        '
        '
        Me.PanelMovimientos.Style.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PanelMovimientos.Style.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PanelMovimientos.Style.BackColorGradientAngle = 90
        Me.PanelMovimientos.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.PanelMovimientos.Style.BorderBottomWidth = 1
        Me.PanelMovimientos.Style.BorderColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PanelMovimientos.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.PanelMovimientos.Style.BorderLeftWidth = 1
        Me.PanelMovimientos.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.PanelMovimientos.Style.BorderRightWidth = 1
        Me.PanelMovimientos.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.PanelMovimientos.Style.BorderTopWidth = 1
        Me.PanelMovimientos.Style.CornerDiameter = 4
        Me.PanelMovimientos.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.PanelMovimientos.Style.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelMovimientos.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.PanelMovimientos.Style.TextColor = System.Drawing.Color.White
        Me.PanelMovimientos.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.PanelMovimientos.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.PanelMovimientos.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.PanelMovimientos.TabIndex = 2
        Me.PanelMovimientos.Text = "Listado De Movimientos Caja General"
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.White
        Me.Panel5.Controls.Add(Me.grMovimientos)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel5.Location = New System.Drawing.Point(0, 0)
        Me.Panel5.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(1333, 372)
        Me.Panel5.TabIndex = 0
        '
        'grMovimientos
        '
        Me.grMovimientos.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grMovimientos.AlternatingColors = True
        Me.grMovimientos.BackColor = System.Drawing.Color.White
        Me.grMovimientos.BorderStyle = Janus.Windows.GridEX.BorderStyle.Raised
        Me.grMovimientos.ColumnAutoResize = True
        Me.grMovimientos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grMovimientos.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grMovimientos.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grMovimientos.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grMovimientos.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grMovimientos.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grMovimientos.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grMovimientos.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grMovimientos.HeaderFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.grMovimientos.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.Empty
        Me.grMovimientos.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grMovimientos.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grMovimientos.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grMovimientos.Location = New System.Drawing.Point(0, 0)
        Me.grMovimientos.Margin = New System.Windows.Forms.Padding(4)
        Me.grMovimientos.Name = "grMovimientos"
        Me.grMovimientos.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grMovimientos.Office2007CustomColor = System.Drawing.Color.DimGray
        Me.grMovimientos.RecordNavigator = True
        Me.grMovimientos.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grMovimientos.RowHeaderFormatStyle.BackColor = System.Drawing.Color.DodgerBlue
        Me.grMovimientos.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grMovimientos.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grMovimientos.SelectedFormatStyle.BackColor = System.Drawing.Color.Gold
        Me.grMovimientos.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grMovimientos.SelectedFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grMovimientos.SelectedInactiveFormatStyle.BackColor = System.Drawing.Color.WhiteSmoke
        Me.grMovimientos.Size = New System.Drawing.Size(1333, 372)
        Me.grMovimientos.TabIndex = 4
        Me.grMovimientos.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grMovimientos.TableSpacing = 9
        Me.grMovimientos.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grMovimientos.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grMovimientos.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grMovimientos.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'Panel10
        '
        Me.Panel10.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Panel10.Controls.Add(Me.Panel11)
        Me.Panel10.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel10.Location = New System.Drawing.Point(3, 3)
        Me.Panel10.Name = "Panel10"
        Me.Panel10.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel10.Size = New System.Drawing.Size(1339, 27)
        Me.Panel10.TabIndex = 1
        '
        'Panel11
        '
        Me.Panel11.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel11.Controls.Add(Me.Label3)
        Me.Panel11.Controls.Add(Me.Panel12)
        Me.Panel11.Controls.Add(Me.PictureBox3)
        Me.Panel11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel11.Location = New System.Drawing.Point(1, 1)
        Me.Panel11.Name = "Panel11"
        Me.Panel11.Size = New System.Drawing.Size(1337, 25)
        Me.Panel11.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Label3.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.Label3.Location = New System.Drawing.Point(59, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.Label3.Size = New System.Drawing.Size(254, 25)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Ver Movimientos Caja General"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel12
        '
        Me.Panel12.BackColor = System.Drawing.Color.White
        Me.Panel12.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel12.Location = New System.Drawing.Point(58, 0)
        Me.Panel12.Name = "Panel12"
        Me.Panel12.Size = New System.Drawing.Size(1, 25)
        Me.Panel12.TabIndex = 1
        '
        'PictureBox3
        '
        Me.PictureBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox3.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox3.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox3.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox3.Size = New System.Drawing.Size(58, 25)
        Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox3.TabIndex = 0
        Me.PictureBox3.TabStop = False
        '
        'PanelButton
        '
        Me.PanelButton.AutoScroll = True
        Me.PanelButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PanelButton.Controls.Add(Me.PanelNavegacion)
        Me.PanelButton.Controls.Add(Me.PanelToolBar1)
        Me.PanelButton.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelButton.Location = New System.Drawing.Point(0, 432)
        Me.PanelButton.Name = "PanelButton"
        Me.PanelButton.Size = New System.Drawing.Size(1345, 83)
        Me.PanelButton.TabIndex = 3
        '
        'PanelNavegacion
        '
        Me.PanelNavegacion.Controls.Add(Me.LabelX5)
        Me.PanelNavegacion.Controls.Add(Me.tbSaldo)
        Me.PanelNavegacion.Controls.Add(Me.cbFechaHasta)
        Me.PanelNavegacion.Controls.Add(Me.LabelX4)
        Me.PanelNavegacion.Controls.Add(Me.cbFechaDesde)
        Me.PanelNavegacion.Controls.Add(Me.LabelX2)
        Me.PanelNavegacion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelNavegacion.Location = New System.Drawing.Point(0, 0)
        Me.PanelNavegacion.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelNavegacion.Name = "PanelNavegacion"
        Me.PanelNavegacion.Size = New System.Drawing.Size(924, 83)
        Me.PanelNavegacion.TabIndex = 21
        '
        'LabelX5
        '
        Me.LabelX5.AutoSize = True
        Me.LabelX5.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX5.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX5.ForeColor = System.Drawing.Color.White
        Me.LabelX5.Location = New System.Drawing.Point(285, 9)
        Me.LabelX5.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX5.Name = "LabelX5"
        Me.LabelX5.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX5.Size = New System.Drawing.Size(167, 21)
        Me.LabelX5.TabIndex = 43
        Me.LabelX5.Text = "Saldo Actual Caja General:"
        '
        'tbSaldo
        '
        Me.tbSaldo.Font = New System.Drawing.Font("Calibri", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbSaldo.Location = New System.Drawing.Point(459, 4)
        Me.tbSaldo.Name = "tbSaldo"
        Me.tbSaldo.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.tbSaldo.Office2007CustomColor = System.Drawing.Color.DarkSlateGray
        Me.tbSaldo.ReadOnly = True
        Me.tbSaldo.Size = New System.Drawing.Size(172, 36)
        Me.tbSaldo.TabIndex = 42
        Me.tbSaldo.Text = "0.00"
        Me.tbSaldo.Value = New Decimal(New Integer() {0, 0, 0, 131072})
        Me.tbSaldo.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
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
        Me.cbFechaHasta.Location = New System.Drawing.Point(67, 40)
        Me.cbFechaHasta.Name = "cbFechaHasta"
        Me.cbFechaHasta.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaHasta.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaHasta.SecondIncrement = 10
        Me.cbFechaHasta.Size = New System.Drawing.Size(200, 26)
        Me.cbFechaHasta.TabIndex = 41
        Me.cbFechaHasta.TodayButtonText = "Hoy"
        Me.cbFechaHasta.UseCompatibleTextRendering = False
        Me.cbFechaHasta.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.cbFechaHasta.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.cbFechaHasta.YearIncrement = 10
        '
        'LabelX4
        '
        Me.LabelX4.AutoSize = True
        Me.LabelX4.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX4.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX4.ForeColor = System.Drawing.Color.White
        Me.LabelX4.Location = New System.Drawing.Point(18, 40)
        Me.LabelX4.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX4.Name = "LabelX4"
        Me.LabelX4.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX4.Size = New System.Drawing.Size(42, 21)
        Me.LabelX4.TabIndex = 40
        Me.LabelX4.Text = "Hasta:"
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
        Me.cbFechaDesde.Location = New System.Drawing.Point(67, 8)
        Me.cbFechaDesde.Name = "cbFechaDesde"
        Me.cbFechaDesde.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaDesde.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaDesde.SecondIncrement = 10
        Me.cbFechaDesde.Size = New System.Drawing.Size(200, 26)
        Me.cbFechaDesde.TabIndex = 39
        Me.cbFechaDesde.TodayButtonText = "Hoy"
        Me.cbFechaDesde.UseCompatibleTextRendering = False
        Me.cbFechaDesde.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.cbFechaDesde.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.cbFechaDesde.YearIncrement = 10
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
        Me.LabelX2.ForeColor = System.Drawing.Color.White
        Me.LabelX2.Location = New System.Drawing.Point(15, 8)
        Me.LabelX2.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX2.Size = New System.Drawing.Size(45, 21)
        Me.LabelX2.TabIndex = 37
        Me.LabelX2.Text = "Desde:"
        '
        'PanelToolBar1
        '
        Me.PanelToolBar1.AutoSize = True
        Me.PanelToolBar1.BackColor = System.Drawing.Color.White
        Me.PanelToolBar1.Controls.Add(Me.ButtonX2)
        Me.PanelToolBar1.Controls.Add(Me.ButtonX1)
        Me.PanelToolBar1.Controls.Add(Me.btGenerarKardex)
        Me.PanelToolBar1.Dock = System.Windows.Forms.DockStyle.Right
        Me.PanelToolBar1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PanelToolBar1.Location = New System.Drawing.Point(924, 0)
        Me.PanelToolBar1.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelToolBar1.Name = "PanelToolBar1"
        Me.PanelToolBar1.Size = New System.Drawing.Size(421, 83)
        Me.PanelToolBar1.TabIndex = 7
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.BackColor = System.Drawing.Color.Transparent
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX1.DisabledImagesGrayScale = False
        Me.ButtonX1.Dock = System.Windows.Forms.DockStyle.Right
        Me.ButtonX1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.printee
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.ButtonX1.Location = New System.Drawing.Point(124, 0)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(145, 83)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX1.TabIndex = 1
        Me.ButtonX1.Text = "Imprimir"
        Me.ButtonX1.TextColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.ButtonX1.Visible = False
        '
        'btGenerarKardex
        '
        Me.btGenerarKardex.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btGenerarKardex.BackColor = System.Drawing.Color.Transparent
        Me.btGenerarKardex.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btGenerarKardex.DisabledImagesGrayScale = False
        Me.btGenerarKardex.Dock = System.Windows.Forms.DockStyle.Right
        Me.btGenerarKardex.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btGenerarKardex.Image = Global.TeVendo.My.Resources.Resources.facturacion
        Me.btGenerarKardex.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btGenerarKardex.Location = New System.Drawing.Point(269, 0)
        Me.btGenerarKardex.Margin = New System.Windows.Forms.Padding(4)
        Me.btGenerarKardex.Name = "btGenerarKardex"
        Me.btGenerarKardex.Size = New System.Drawing.Size(152, 83)
        Me.btGenerarKardex.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btGenerarKardex.TabIndex = 0
        Me.btGenerarKardex.Text = "Generar Kardex"
        Me.btGenerarKardex.TextColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        '
        'ButtonX2
        '
        Me.ButtonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX2.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(83, Byte), Integer), CType(CType(79, Byte), Integer))
        Me.ButtonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.ButtonX2.Dock = System.Windows.Forms.DockStyle.Right
        Me.ButtonX2.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX2.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.ButtonX2.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.ButtonX2.Location = New System.Drawing.Point(0, 0)
        Me.ButtonX2.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.ButtonX2.Name = "ButtonX2"
        Me.ButtonX2.Size = New System.Drawing.Size(124, 83)
        Me.ButtonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.ButtonX2.Symbol = ""
        Me.ButtonX2.TabIndex = 70
        Me.ButtonX2.Text = "Salir"
        Me.ButtonX2.TextColor = System.Drawing.Color.White
        '
        'Tec_CajaGeneral
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1345, 515)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Tec_CajaGeneral"
        Me.Text = "Tec_CajaGeneral"
        Me.Panel1.ResumeLayout(False)
        Me.PanelSuperior.ResumeLayout(False)
        Me.PanelLEft.ResumeLayout(False)
        Me.Panel8.ResumeLayout(False)
        Me.PanelDatos.ResumeLayout(False)
        Me.PanelMovimientos.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        CType(Me.grMovimientos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelButton.ResumeLayout(False)
        Me.PanelButton.PerformLayout()
        Me.PanelNavegacion.ResumeLayout(False)
        Me.PanelNavegacion.PerformLayout()
        Me.PanelToolBar1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents PanelSuperior As Panel
    Friend WithEvents PanelLEft As Panel
    Friend WithEvents Panel8 As Panel
    Friend WithEvents PanelDatos As Panel
    Friend WithEvents PanelMovimientos As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Panel5 As Panel
    Friend WithEvents grMovimientos As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel10 As Panel
    Friend WithEvents Panel11 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel12 As Panel
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents PanelButton As Panel
    Protected WithEvents PanelNavegacion As Panel
    Friend WithEvents LabelX5 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbSaldo As Janus.Windows.GridEX.EditControls.NumericEditBox
    Friend WithEvents cbFechaHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX4 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbFechaDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Protected WithEvents PanelToolBar1 As Panel
    Protected WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btGenerarKardex As DevComponents.DotNetBar.ButtonX
    Friend WithEvents ButtonX2 As DevComponents.DotNetBar.ButtonX
End Class
