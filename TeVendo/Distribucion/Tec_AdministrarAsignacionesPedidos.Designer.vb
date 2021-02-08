<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_AdministrarAsignacionesPedidos
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
        Dim SuperTabColorTable1 As DevComponents.DotNetBar.Rendering.SuperTabColorTable = New DevComponents.DotNetBar.Rendering.SuperTabColorTable()
        Dim SuperTabLinearGradientColorTable1 As DevComponents.DotNetBar.Rendering.SuperTabLinearGradientColorTable = New DevComponents.DotNetBar.Rendering.SuperTabLinearGradientColorTable()
        Me.SuperTabGeneral = New DevComponents.DotNetBar.SuperTabControl()
        Me.SuperTabItem1 = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel1 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.SuperTabItem2 = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel2 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.grPedidosPendientes = New Janus.Windows.GridEX.GridEX()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel14 = New System.Windows.Forms.Panel()
        Me.Panel15 = New System.Windows.Forms.Panel()
        Me.lbtitle = New System.Windows.Forms.Label()
        Me.Panel16 = New System.Windows.Forms.Panel()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.btnSearchPersonal = New DevComponents.DotNetBar.ButtonX()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.tbChofer = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.ButtonX4 = New DevComponents.DotNetBar.ButtonX()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.ButtonX2 = New DevComponents.DotNetBar.ButtonX()
        CType(Me.SuperTabGeneral, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuperTabGeneral.SuspendLayout()
        Me.SuperTabControlPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.GroupPanel1.SuspendLayout()
        CType(Me.grPedidosPendientes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.Panel14.SuspendLayout()
        Me.Panel15.SuspendLayout()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SuperTabGeneral
        '
        '
        '
        '
        '
        '
        '
        Me.SuperTabGeneral.ControlBox.CloseBox.Name = ""
        '
        '
        '
        Me.SuperTabGeneral.ControlBox.MenuBox.Name = ""
        Me.SuperTabGeneral.ControlBox.Name = ""
        Me.SuperTabGeneral.ControlBox.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.SuperTabGeneral.ControlBox.MenuBox, Me.SuperTabGeneral.ControlBox.CloseBox})
        Me.SuperTabGeneral.Controls.Add(Me.SuperTabControlPanel1)
        Me.SuperTabGeneral.Controls.Add(Me.SuperTabControlPanel2)
        Me.SuperTabGeneral.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabGeneral.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabGeneral.ForeColor = System.Drawing.Color.White
        Me.SuperTabGeneral.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabGeneral.Name = "SuperTabGeneral"
        Me.SuperTabGeneral.ReorderTabsEnabled = True
        Me.SuperTabGeneral.SelectedTabFont = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabGeneral.SelectedTabIndex = 0
        Me.SuperTabGeneral.Size = New System.Drawing.Size(1032, 551)
        Me.SuperTabGeneral.TabFont = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabGeneral.TabIndex = 0
        Me.SuperTabGeneral.Tabs.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.SuperTabItem1, Me.SuperTabItem2})
        SuperTabLinearGradientColorTable1.Colors = New System.Drawing.Color() {System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))}
        SuperTabColorTable1.Background = SuperTabLinearGradientColorTable1
        Me.SuperTabGeneral.TabStripColor = SuperTabColorTable1
        Me.SuperTabGeneral.TabStyle = DevComponents.DotNetBar.eSuperTabStyle.Office2010BackstageBlue
        Me.SuperTabGeneral.Text = "SuperTabGeneral"
        '
        'SuperTabItem1
        '
        Me.SuperTabItem1.AttachedControl = Me.SuperTabControlPanel1
        Me.SuperTabItem1.GlobalItem = False
        Me.SuperTabItem1.Image = Global.TeVendo.My.Resources.Resources.modulocompra
        Me.SuperTabItem1.Name = "SuperTabItem1"
        Me.SuperTabItem1.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Teal
        Me.SuperTabItem1.TabFont = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabItem1.Text = "Asignar Pedidos Pendientes"
        '
        'SuperTabControlPanel1
        '
        Me.SuperTabControlPanel1.Controls.Add(Me.Panel1)
        Me.SuperTabControlPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel1.Location = New System.Drawing.Point(0, 59)
        Me.SuperTabControlPanel1.Name = "SuperTabControlPanel1"
        Me.SuperTabControlPanel1.Size = New System.Drawing.Size(1032, 492)
        Me.SuperTabControlPanel1.TabIndex = 1
        Me.SuperTabControlPanel1.TabItem = Me.SuperTabItem1
        '
        'SuperTabItem2
        '
        Me.SuperTabItem2.AttachedControl = Me.SuperTabControlPanel2
        Me.SuperTabItem2.GlobalItem = False
        Me.SuperTabItem2.Image = Global.TeVendo.My.Resources.Resources.shopping03
        Me.SuperTabItem2.Name = "SuperTabItem2"
        Me.SuperTabItem2.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Teal
        Me.SuperTabItem2.TabFont = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabItem2.Text = "Administrar Asignaciones"
        '
        'SuperTabControlPanel2
        '
        Me.SuperTabControlPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel2.Location = New System.Drawing.Point(0, 59)
        Me.SuperTabControlPanel2.Name = "SuperTabControlPanel2"
        Me.SuperTabControlPanel2.Size = New System.Drawing.Size(917, 492)
        Me.SuperTabControlPanel2.TabIndex = 0
        Me.SuperTabControlPanel2.TabItem = Me.SuperTabItem2
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Controls.Add(Me.Panel14)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1032, 492)
        Me.Panel1.TabIndex = 1
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.GroupPanel1)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(0, 40)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(706, 452)
        Me.Panel3.TabIndex = 7
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.GroupPanel1.Controls.Add(Me.grPedidosPendientes)
        Me.GroupPanel1.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel1.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(706, 452)
        '
        '
        '
        Me.GroupPanel1.Style.BackColor = System.Drawing.Color.DarkCyan
        Me.GroupPanel1.Style.BackColor2 = System.Drawing.Color.DarkCyan
        Me.GroupPanel1.Style.BackColorGradientAngle = 90
        Me.GroupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderBottomWidth = 1
        Me.GroupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderLeftWidth = 1
        Me.GroupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderRightWidth = 1
        Me.GroupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderTopWidth = 1
        Me.GroupPanel1.Style.CornerDiameter = 4
        Me.GroupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far
        Me.GroupPanel1.Style.TextColor = System.Drawing.Color.White
        Me.GroupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.GroupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.GroupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.GroupPanel1.TabIndex = 6
        Me.GroupPanel1.Text = "Listado de Pedidos Pendientes"
        '
        'grPedidosPendientes
        '
        Me.grPedidosPendientes.AlternatingColors = True
        Me.grPedidosPendientes.BackColor = System.Drawing.Color.White
        Me.grPedidosPendientes.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grPedidosPendientes.ColumnAutoResize = True
        Me.grPedidosPendientes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grPedidosPendientes.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grPedidosPendientes.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grPedidosPendientes.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPedidosPendientes.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grPedidosPendientes.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grPedidosPendientes.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grPedidosPendientes.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPedidosPendientes.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grPedidosPendientes.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grPedidosPendientes.HeaderFormatStyle.Alpha = 0
        Me.grPedidosPendientes.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grPedidosPendientes.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grPedidosPendientes.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grPedidosPendientes.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grPedidosPendientes.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grPedidosPendientes.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPedidosPendientes.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grPedidosPendientes.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grPedidosPendientes.Location = New System.Drawing.Point(0, 0)
        Me.grPedidosPendientes.Margin = New System.Windows.Forms.Padding(4)
        Me.grPedidosPendientes.Name = "grPedidosPendientes"
        Me.grPedidosPendientes.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grPedidosPendientes.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grPedidosPendientes.RecordNavigator = True
        Me.grPedidosPendientes.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPedidosPendientes.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grPedidosPendientes.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grPedidosPendientes.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grPedidosPendientes.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grPedidosPendientes.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grPedidosPendientes.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPedidosPendientes.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grPedidosPendientes.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grPedidosPendientes.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPedidosPendientes.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grPedidosPendientes.Size = New System.Drawing.Size(700, 422)
        Me.grPedidosPendientes.TabIndex = 4
        Me.grPedidosPendientes.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grPedidosPendientes.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grPedidosPendientes.TableSpacing = 9
        Me.grPedidosPendientes.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grPedidosPendientes.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grPedidosPendientes.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grPedidosPendientes.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.ButtonX2)
        Me.Panel2.Controls.Add(Me.ButtonX1)
        Me.Panel2.Controls.Add(Me.ButtonX4)
        Me.Panel2.Controls.Add(Me.btnSearchPersonal)
        Me.Panel2.Controls.Add(Me.LabelX1)
        Me.Panel2.Controls.Add(Me.tbChofer)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel2.Location = New System.Drawing.Point(706, 40)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(326, 452)
        Me.Panel2.TabIndex = 6
        '
        'Panel14
        '
        Me.Panel14.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel14.Controls.Add(Me.Panel15)
        Me.Panel14.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel14.Location = New System.Drawing.Point(0, 0)
        Me.Panel14.Name = "Panel14"
        Me.Panel14.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel14.Size = New System.Drawing.Size(1032, 40)
        Me.Panel14.TabIndex = 5
        '
        'Panel15
        '
        Me.Panel15.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel15.Controls.Add(Me.lbtitle)
        Me.Panel15.Controls.Add(Me.Panel16)
        Me.Panel15.Controls.Add(Me.PictureBox4)
        Me.Panel15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel15.Location = New System.Drawing.Point(1, 1)
        Me.Panel15.Name = "Panel15"
        Me.Panel15.Size = New System.Drawing.Size(1030, 38)
        Me.Panel15.TabIndex = 0
        '
        'lbtitle
        '
        Me.lbtitle.BackColor = System.Drawing.Color.Transparent
        Me.lbtitle.Dock = System.Windows.Forms.DockStyle.Left
        Me.lbtitle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbtitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbtitle.Location = New System.Drawing.Point(59, 0)
        Me.lbtitle.Name = "lbtitle"
        Me.lbtitle.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.lbtitle.Size = New System.Drawing.Size(773, 38)
        Me.lbtitle.TabIndex = 2
        Me.lbtitle.Text = "Asignación de Pedidos Pendientes A Distribuidores"
        Me.lbtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel16
        '
        Me.Panel16.BackColor = System.Drawing.Color.White
        Me.Panel16.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel16.Location = New System.Drawing.Point(58, 0)
        Me.Panel16.Name = "Panel16"
        Me.Panel16.Size = New System.Drawing.Size(1, 38)
        Me.Panel16.TabIndex = 1
        '
        'PictureBox4
        '
        Me.PictureBox4.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox4.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox4.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox4.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox4.Name = "PictureBox4"
        Me.PictureBox4.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox4.Size = New System.Drawing.Size(58, 38)
        Me.PictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox4.TabIndex = 0
        Me.PictureBox4.TabStop = False
        '
        'btnSearchPersonal
        '
        Me.btnSearchPersonal.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSearchPersonal.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnSearchPersonal.Font = New System.Drawing.Font("Calibri", 7.8!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSearchPersonal.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnSearchPersonal.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.btnSearchPersonal.Location = New System.Drawing.Point(17, 82)
        Me.btnSearchPersonal.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSearchPersonal.Name = "btnSearchPersonal"
        Me.btnSearchPersonal.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnSearchPersonal.Size = New System.Drawing.Size(232, 34)
        Me.btnSearchPersonal.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSearchPersonal.Symbol = "59500"
        Me.btnSearchPersonal.SymbolColor = System.Drawing.Color.MediumTurquoise
        Me.btnSearchPersonal.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnSearchPersonal.TabIndex = 41
        Me.btnSearchPersonal.Text = "Seleccionar Chofer:"
        '
        'LabelX1
        '
        Me.LabelX1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX1.Location = New System.Drawing.Point(17, 15)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(168, 28)
        Me.LabelX1.TabIndex = 42
        Me.LabelX1.Text = "Asignar A Distribuidor:"
        '
        'tbChofer
        '
        '
        '
        '
        Me.tbChofer.Border.Class = "TextBoxBorder"
        Me.tbChofer.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbChofer.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbChofer.Location = New System.Drawing.Point(17, 51)
        Me.tbChofer.Margin = New System.Windows.Forms.Padding(4)
        Me.tbChofer.Name = "tbChofer"
        Me.tbChofer.PreventEnterBeep = True
        Me.tbChofer.Size = New System.Drawing.Size(255, 26)
        Me.tbChofer.TabIndex = 40
        '
        'ButtonX4
        '
        Me.ButtonX4.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX4.BackColor = System.Drawing.Color.LightSeaGreen
        Me.ButtonX4.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX4.DisabledImagesGrayScale = False
        Me.ButtonX4.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX4.Image = Global.TeVendo.My.Resources.Resources.caja
        Me.ButtonX4.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.ButtonX4.Location = New System.Drawing.Point(17, 137)
        Me.ButtonX4.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX4.Name = "ButtonX4"
        Me.ButtonX4.Size = New System.Drawing.Size(117, 44)
        Me.ButtonX4.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX4.Symbol = ""
        Me.ButtonX4.TabIndex = 43
        Me.ButtonX4.Text = "Asignar"
        Me.ButtonX4.TextColor = System.Drawing.Color.White
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.BackColor = System.Drawing.Color.Orange
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX1.DisabledImagesGrayScale = False
        Me.ButtonX1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.caja
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.ButtonX1.Location = New System.Drawing.Point(142, 137)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(122, 44)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX1.Symbol = ""
        Me.ButtonX1.TabIndex = 44
        Me.ButtonX1.Text = "Actualizar"
        Me.ButtonX1.TextColor = System.Drawing.Color.White
        '
        'ButtonX2
        '
        Me.ButtonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX2.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(83, Byte), Integer), CType(CType(79, Byte), Integer))
        Me.ButtonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.ButtonX2.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX2.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.ButtonX2.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.ButtonX2.Location = New System.Drawing.Point(17, 187)
        Me.ButtonX2.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.ButtonX2.Name = "ButtonX2"
        Me.ButtonX2.Size = New System.Drawing.Size(117, 43)
        Me.ButtonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.ButtonX2.Symbol = ""
        Me.ButtonX2.TabIndex = 70
        Me.ButtonX2.Text = "Salir"
        Me.ButtonX2.TextColor = System.Drawing.Color.White
        '
        'Tec_AdministrarAsignacionesPedidos
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1032, 551)
        Me.Controls.Add(Me.SuperTabGeneral)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "Tec_AdministrarAsignacionesPedidos"
        Me.Text = "Tec_AdministrarAsignacionesPedidos"
        CType(Me.SuperTabGeneral, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SuperTabGeneral.ResumeLayout(False)
        Me.SuperTabControlPanel1.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.GroupPanel1.ResumeLayout(False)
        CType(Me.grPedidosPendientes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel14.ResumeLayout(False)
        Me.Panel15.ResumeLayout(False)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SuperTabGeneral As DevComponents.DotNetBar.SuperTabControl
    Friend WithEvents SuperTabControlPanel1 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents SuperTabItem1 As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents SuperTabControlPanel2 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents SuperTabItem2 As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents grPedidosPendientes As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Panel14 As Panel
    Friend WithEvents Panel15 As Panel
    Friend WithEvents lbtitle As Label
    Friend WithEvents Panel16 As Panel
    Friend WithEvents PictureBox4 As PictureBox
    Friend WithEvents btnSearchPersonal As DevComponents.DotNetBar.ButtonX
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbChofer As DevComponents.DotNetBar.Controls.TextBoxX
    Protected WithEvents ButtonX4 As DevComponents.DotNetBar.ButtonX
    Protected WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents ButtonX2 As DevComponents.DotNetBar.ButtonX
End Class
