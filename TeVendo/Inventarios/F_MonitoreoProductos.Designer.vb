<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class F_MonitoreoProductos
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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.SuperTabControlMenu = New DevComponents.DotNetBar.SuperTabControl()
        Me.SuperTabControlPanel3 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel17 = New System.Windows.Forms.Panel()
        Me.gr_ProductosStock0 = New Janus.Windows.GridEX.GridEX()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.btnProductosSinStock = New DevComponents.DotNetBar.ButtonX()
        Me.tabProductosSinStock = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel6 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.PanelDatos = New System.Windows.Forms.Panel()
        Me.Panel8 = New System.Windows.Forms.Panel()
        Me.gr_ProductosVencidos = New Janus.Windows.GridEX.GridEX()
        Me.Panel9 = New System.Windows.Forms.Panel()
        Me.ButtonX3 = New DevComponents.DotNetBar.ButtonX()
        Me.GroupPanel4 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.grPagos = New Janus.Windows.GridEX.GridEX()
        Me.btnProductosVencidos = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel5 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.grProductosStockMinimo = New Janus.Windows.GridEX.GridEX()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.btnStockMinimo = New DevComponents.DotNetBar.ButtonX()
        Me.tabProductosStockMinimo = New DevComponents.DotNetBar.SuperTabItem()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Title = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.btnProductosSinVentas = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel1 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.grProductoSinVentas = New Janus.Windows.GridEX.GridEX()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.btnExportarSinVentas = New DevComponents.DotNetBar.ButtonX()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel13 = New System.Windows.Forms.Panel()
        Me.GridEX2 = New Janus.Windows.GridEX.GridEX()
        Me.Panel21 = New System.Windows.Forms.Panel()
        Me.btnSinVentas = New DevComponents.DotNetBar.ButtonX()
        Me.tbHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX16 = New DevComponents.DotNetBar.LabelX()
        Me.tbDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX6 = New DevComponents.DotNetBar.LabelX()
        Me.Panel1.SuspendLayout()
        CType(Me.SuperTabControlMenu, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuperTabControlMenu.SuspendLayout()
        Me.SuperTabControlPanel3.SuspendLayout()
        Me.Panel17.SuspendLayout()
        CType(Me.gr_ProductosStock0, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.SuspendLayout()
        Me.SuperTabControlPanel6.SuspendLayout()
        Me.PanelDatos.SuspendLayout()
        Me.Panel8.SuspendLayout()
        CType(Me.gr_ProductosVencidos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel9.SuspendLayout()
        Me.GroupPanel4.SuspendLayout()
        Me.Panel7.SuspendLayout()
        CType(Me.grPagos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuperTabControlPanel5.SuspendLayout()
        Me.Panel5.SuspendLayout()
        CType(Me.grProductosStockMinimo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel6.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Title.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuperTabControlPanel1.SuspendLayout()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        CType(Me.grProductoSinVentas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel12.SuspendLayout()
        Me.GroupPanel1.SuspendLayout()
        Me.Panel13.SuspendLayout()
        CType(Me.GridEX2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel21.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.SuperTabControlMenu)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(988, 517)
        Me.Panel1.TabIndex = 2
        '
        'SuperTabControlMenu
        '
        Me.SuperTabControlMenu.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        '
        '
        '
        '
        '
        '
        Me.SuperTabControlMenu.ControlBox.CloseBox.Name = ""
        '
        '
        '
        Me.SuperTabControlMenu.ControlBox.MenuBox.Name = ""
        Me.SuperTabControlMenu.ControlBox.Name = ""
        Me.SuperTabControlMenu.ControlBox.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.SuperTabControlMenu.ControlBox.MenuBox, Me.SuperTabControlMenu.ControlBox.CloseBox})
        Me.SuperTabControlMenu.Controls.Add(Me.SuperTabControlPanel1)
        Me.SuperTabControlMenu.Controls.Add(Me.SuperTabControlPanel6)
        Me.SuperTabControlMenu.Controls.Add(Me.SuperTabControlPanel5)
        Me.SuperTabControlMenu.Controls.Add(Me.SuperTabControlPanel3)
        Me.SuperTabControlMenu.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlMenu.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabControlMenu.ForeColor = System.Drawing.Color.Black
        Me.SuperTabControlMenu.Location = New System.Drawing.Point(0, 41)
        Me.SuperTabControlMenu.Name = "SuperTabControlMenu"
        Me.SuperTabControlMenu.ReorderTabsEnabled = True
        Me.SuperTabControlMenu.SelectedTabFont = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabControlMenu.SelectedTabIndex = 0
        Me.SuperTabControlMenu.Size = New System.Drawing.Size(988, 476)
        Me.SuperTabControlMenu.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabControlMenu.TabIndex = 0
        Me.SuperTabControlMenu.TabLayoutType = DevComponents.DotNetBar.eSuperTabLayoutType.MultiLineFit
        Me.SuperTabControlMenu.Tabs.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.tabProductosSinStock, Me.tabProductosStockMinimo, Me.btnProductosSinVentas, Me.btnProductosVencidos})
        SuperTabLinearGradientColorTable1.Colors = New System.Drawing.Color() {System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))}
        SuperTabColorTable1.Background = SuperTabLinearGradientColorTable1
        Me.SuperTabControlMenu.TabStripColor = SuperTabColorTable1
        Me.SuperTabControlMenu.TabStyle = DevComponents.DotNetBar.eSuperTabStyle.Office2010BackstageBlue
        Me.SuperTabControlMenu.Text = "SuperTabControl1"
        '
        'SuperTabControlPanel3
        '
        Me.SuperTabControlPanel3.Controls.Add(Me.Panel17)
        Me.SuperTabControlPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel3.Location = New System.Drawing.Point(0, 58)
        Me.SuperTabControlPanel3.Name = "SuperTabControlPanel3"
        Me.SuperTabControlPanel3.Size = New System.Drawing.Size(988, 418)
        Me.SuperTabControlPanel3.TabIndex = 3
        Me.SuperTabControlPanel3.TabItem = Me.tabProductosSinStock
        '
        'Panel17
        '
        Me.Panel17.Controls.Add(Me.gr_ProductosStock0)
        Me.Panel17.Controls.Add(Me.Panel4)
        Me.Panel17.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel17.Location = New System.Drawing.Point(0, 0)
        Me.Panel17.Name = "Panel17"
        Me.Panel17.Padding = New System.Windows.Forms.Padding(5)
        Me.Panel17.Size = New System.Drawing.Size(988, 418)
        Me.Panel17.TabIndex = 5
        '
        'gr_ProductosStock0
        '
        Me.gr_ProductosStock0.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.gr_ProductosStock0.AlternatingColors = True
        Me.gr_ProductosStock0.BackColor = System.Drawing.Color.WhiteSmoke
        Me.gr_ProductosStock0.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.gr_ProductosStock0.ColumnAutoResize = True
        Me.gr_ProductosStock0.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gr_ProductosStock0.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.gr_ProductosStock0.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.gr_ProductosStock0.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosStock0.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.gr_ProductosStock0.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.gr_ProductosStock0.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.gr_ProductosStock0.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosStock0.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.gr_ProductosStock0.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.gr_ProductosStock0.HeaderFormatStyle.Alpha = 0
        Me.gr_ProductosStock0.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.gr_ProductosStock0.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.gr_ProductosStock0.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.gr_ProductosStock0.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.gr_ProductosStock0.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.gr_ProductosStock0.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosStock0.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.gr_ProductosStock0.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.gr_ProductosStock0.Location = New System.Drawing.Point(5, 66)
        Me.gr_ProductosStock0.Margin = New System.Windows.Forms.Padding(4)
        Me.gr_ProductosStock0.Name = "gr_ProductosStock0"
        Me.gr_ProductosStock0.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.gr_ProductosStock0.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.gr_ProductosStock0.RecordNavigator = True
        Me.gr_ProductosStock0.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosStock0.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.gr_ProductosStock0.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.gr_ProductosStock0.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.gr_ProductosStock0.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.gr_ProductosStock0.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.gr_ProductosStock0.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosStock0.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.gr_ProductosStock0.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.gr_ProductosStock0.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosStock0.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.gr_ProductosStock0.Size = New System.Drawing.Size(978, 347)
        Me.gr_ProductosStock0.TabIndex = 2
        Me.gr_ProductosStock0.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.gr_ProductosStock0.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.gr_ProductosStock0.TableSpacing = 9
        Me.gr_ProductosStock0.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.gr_ProductosStock0.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.gr_ProductosStock0.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.gr_ProductosStock0.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.White
        Me.Panel4.Controls.Add(Me.btnProductosSinStock)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(5, 5)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(978, 61)
        Me.Panel4.TabIndex = 4
        '
        'btnProductosSinStock
        '
        Me.btnProductosSinStock.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnProductosSinStock.BackColor = System.Drawing.Color.Honeydew
        Me.btnProductosSinStock.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnProductosSinStock.DisabledImagesGrayScale = False
        Me.btnProductosSinStock.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnProductosSinStock.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnProductosSinStock.Image = Global.TeVendo.My.Resources.Resources.sheets
        Me.btnProductosSinStock.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnProductosSinStock.Location = New System.Drawing.Point(751, 0)
        Me.btnProductosSinStock.Margin = New System.Windows.Forms.Padding(4)
        Me.btnProductosSinStock.Name = "btnProductosSinStock"
        Me.btnProductosSinStock.Size = New System.Drawing.Size(227, 61)
        Me.btnProductosSinStock.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnProductosSinStock.TabIndex = 2
        Me.btnProductosSinStock.Text = "Exportar Excel"
        Me.btnProductosSinStock.TextColor = System.Drawing.Color.Black
        '
        'tabProductosSinStock
        '
        Me.tabProductosSinStock.AttachedControl = Me.SuperTabControlPanel3
        Me.tabProductosSinStock.GlobalItem = False
        Me.tabProductosSinStock.Image = Global.TeVendo.My.Resources.Resources.stock01
        Me.tabProductosSinStock.Name = "tabProductosSinStock"
        Me.tabProductosSinStock.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Teal
        Me.tabProductosSinStock.SelectedTabFont = New System.Drawing.Font("Calibri", 12.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tabProductosSinStock.SymbolColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.tabProductosSinStock.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tabProductosSinStock.Text = "Productos Sin Stock"
        Me.tabProductosSinStock.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Near
        '
        'SuperTabControlPanel6
        '
        Me.SuperTabControlPanel6.Controls.Add(Me.PanelDatos)
        Me.SuperTabControlPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel6.Location = New System.Drawing.Point(0, 58)
        Me.SuperTabControlPanel6.Name = "SuperTabControlPanel6"
        Me.SuperTabControlPanel6.Size = New System.Drawing.Size(988, 418)
        Me.SuperTabControlPanel6.TabIndex = 6
        Me.SuperTabControlPanel6.TabItem = Me.btnProductosVencidos
        '
        'PanelDatos
        '
        Me.PanelDatos.BackColor = System.Drawing.Color.White
        Me.PanelDatos.Controls.Add(Me.Panel8)
        Me.PanelDatos.Controls.Add(Me.GroupPanel4)
        Me.PanelDatos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDatos.Location = New System.Drawing.Point(0, 0)
        Me.PanelDatos.Name = "PanelDatos"
        Me.PanelDatos.Size = New System.Drawing.Size(988, 418)
        Me.PanelDatos.TabIndex = 3
        '
        'Panel8
        '
        Me.Panel8.Controls.Add(Me.gr_ProductosVencidos)
        Me.Panel8.Controls.Add(Me.Panel9)
        Me.Panel8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel8.Location = New System.Drawing.Point(0, 0)
        Me.Panel8.Name = "Panel8"
        Me.Panel8.Padding = New System.Windows.Forms.Padding(5)
        Me.Panel8.Size = New System.Drawing.Size(988, 418)
        Me.Panel8.TabIndex = 7
        '
        'gr_ProductosVencidos
        '
        Me.gr_ProductosVencidos.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.gr_ProductosVencidos.AlternatingColors = True
        Me.gr_ProductosVencidos.BackColor = System.Drawing.Color.WhiteSmoke
        Me.gr_ProductosVencidos.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.gr_ProductosVencidos.ColumnAutoResize = True
        Me.gr_ProductosVencidos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gr_ProductosVencidos.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.gr_ProductosVencidos.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.gr_ProductosVencidos.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosVencidos.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.gr_ProductosVencidos.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.gr_ProductosVencidos.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.gr_ProductosVencidos.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosVencidos.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.gr_ProductosVencidos.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.gr_ProductosVencidos.HeaderFormatStyle.Alpha = 0
        Me.gr_ProductosVencidos.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.gr_ProductosVencidos.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.gr_ProductosVencidos.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.gr_ProductosVencidos.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.gr_ProductosVencidos.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.gr_ProductosVencidos.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosVencidos.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.gr_ProductosVencidos.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.gr_ProductosVencidos.Location = New System.Drawing.Point(5, 66)
        Me.gr_ProductosVencidos.Margin = New System.Windows.Forms.Padding(4)
        Me.gr_ProductosVencidos.Name = "gr_ProductosVencidos"
        Me.gr_ProductosVencidos.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.gr_ProductosVencidos.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.gr_ProductosVencidos.RecordNavigator = True
        Me.gr_ProductosVencidos.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosVencidos.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.gr_ProductosVencidos.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.gr_ProductosVencidos.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.gr_ProductosVencidos.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.gr_ProductosVencidos.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.gr_ProductosVencidos.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosVencidos.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.gr_ProductosVencidos.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.gr_ProductosVencidos.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_ProductosVencidos.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.gr_ProductosVencidos.Size = New System.Drawing.Size(978, 347)
        Me.gr_ProductosVencidos.TabIndex = 2
        Me.gr_ProductosVencidos.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.gr_ProductosVencidos.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.gr_ProductosVencidos.TableSpacing = 9
        Me.gr_ProductosVencidos.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.gr_ProductosVencidos.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.gr_ProductosVencidos.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.gr_ProductosVencidos.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel9
        '
        Me.Panel9.BackColor = System.Drawing.Color.White
        Me.Panel9.Controls.Add(Me.ButtonX3)
        Me.Panel9.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel9.Location = New System.Drawing.Point(5, 5)
        Me.Panel9.Name = "Panel9"
        Me.Panel9.Size = New System.Drawing.Size(978, 61)
        Me.Panel9.TabIndex = 4
        '
        'ButtonX3
        '
        Me.ButtonX3.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX3.BackColor = System.Drawing.Color.Honeydew
        Me.ButtonX3.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX3.DisabledImagesGrayScale = False
        Me.ButtonX3.Dock = System.Windows.Forms.DockStyle.Right
        Me.ButtonX3.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX3.Image = Global.TeVendo.My.Resources.Resources.sheets
        Me.ButtonX3.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.ButtonX3.Location = New System.Drawing.Point(751, 0)
        Me.ButtonX3.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX3.Name = "ButtonX3"
        Me.ButtonX3.Size = New System.Drawing.Size(227, 61)
        Me.ButtonX3.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX3.TabIndex = 3
        Me.ButtonX3.Text = "Exportar Excel"
        Me.ButtonX3.TextColor = System.Drawing.Color.Black
        '
        'GroupPanel4
        '
        Me.GroupPanel4.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel4.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel4.Controls.Add(Me.Panel7)
        Me.GroupPanel4.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel4.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel4.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel4.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupPanel4.Name = "GroupPanel4"
        Me.GroupPanel4.Size = New System.Drawing.Size(988, 418)
        '
        '
        '
        Me.GroupPanel4.Style.BackColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.GroupPanel4.Style.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.GroupPanel4.Style.BackColorGradientAngle = 90
        Me.GroupPanel4.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderBottomWidth = 1
        Me.GroupPanel4.Style.BorderColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.GroupPanel4.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderLeftWidth = 1
        Me.GroupPanel4.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderRightWidth = 1
        Me.GroupPanel4.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel4.Style.BorderTopWidth = 1
        Me.GroupPanel4.Style.CornerDiameter = 4
        Me.GroupPanel4.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel4.Style.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel4.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel4.Style.TextColor = System.Drawing.Color.White
        Me.GroupPanel4.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.GroupPanel4.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.GroupPanel4.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.GroupPanel4.TabIndex = 2
        Me.GroupPanel4.Text = "Listado De Pagos"
        Me.GroupPanel4.Visible = False
        '
        'Panel7
        '
        Me.Panel7.BackColor = System.Drawing.Color.White
        Me.Panel7.Controls.Add(Me.grPagos)
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel7.Location = New System.Drawing.Point(0, 0)
        Me.Panel7.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(982, 391)
        Me.Panel7.TabIndex = 0
        '
        'grPagos
        '
        Me.grPagos.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grPagos.AlternatingColors = True
        Me.grPagos.BackColor = System.Drawing.Color.White
        Me.grPagos.BorderStyle = Janus.Windows.GridEX.BorderStyle.Raised
        Me.grPagos.ColumnAutoResize = True
        Me.grPagos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grPagos.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grPagos.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPagos.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grPagos.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grPagos.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grPagos.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPagos.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grPagos.HeaderFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grPagos.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.Empty
        Me.grPagos.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPagos.HeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grPagos.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grPagos.Location = New System.Drawing.Point(0, 0)
        Me.grPagos.Margin = New System.Windows.Forms.Padding(4)
        Me.grPagos.Name = "grPagos"
        Me.grPagos.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grPagos.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grPagos.RecordNavigator = True
        Me.grPagos.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPagos.RowHeaderFormatStyle.BackColor = System.Drawing.Color.DodgerBlue
        Me.grPagos.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPagos.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grPagos.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grPagos.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPagos.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grPagos.SelectedInactiveFormatStyle.BackColor = System.Drawing.Color.WhiteSmoke
        Me.grPagos.Size = New System.Drawing.Size(982, 391)
        Me.grPagos.TabIndex = 3
        Me.grPagos.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grPagos.TableSpacing = 9
        Me.grPagos.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grPagos.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grPagos.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grPagos.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'btnProductosVencidos
        '
        Me.btnProductosVencidos.AttachedControl = Me.SuperTabControlPanel6
        Me.btnProductosVencidos.GlobalItem = False
        Me.btnProductosVencidos.Image = Global.TeVendo.My.Resources.Resources.stock03
        Me.btnProductosVencidos.Name = "btnProductosVencidos"
        Me.btnProductosVencidos.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Teal
        Me.btnProductosVencidos.SelectedTabFont = New System.Drawing.Font("Calibri", 12.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnProductosVencidos.SymbolColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.btnProductosVencidos.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnProductosVencidos.Text = "Productos Vencidos"
        Me.btnProductosVencidos.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Near
        '
        'SuperTabControlPanel5
        '
        Me.SuperTabControlPanel5.Controls.Add(Me.Panel5)
        Me.SuperTabControlPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel5.Location = New System.Drawing.Point(0, 114)
        Me.SuperTabControlPanel5.Name = "SuperTabControlPanel5"
        Me.SuperTabControlPanel5.Size = New System.Drawing.Size(988, 362)
        Me.SuperTabControlPanel5.TabIndex = 5
        Me.SuperTabControlPanel5.TabItem = Me.tabProductosStockMinimo
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.grProductosStockMinimo)
        Me.Panel5.Controls.Add(Me.Panel6)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel5.Location = New System.Drawing.Point(0, 0)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Padding = New System.Windows.Forms.Padding(5)
        Me.Panel5.Size = New System.Drawing.Size(988, 362)
        Me.Panel5.TabIndex = 6
        '
        'grProductosStockMinimo
        '
        Me.grProductosStockMinimo.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grProductosStockMinimo.AlternatingColors = True
        Me.grProductosStockMinimo.BackColor = System.Drawing.Color.WhiteSmoke
        Me.grProductosStockMinimo.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grProductosStockMinimo.ColumnAutoResize = True
        Me.grProductosStockMinimo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grProductosStockMinimo.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grProductosStockMinimo.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grProductosStockMinimo.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductosStockMinimo.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grProductosStockMinimo.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grProductosStockMinimo.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grProductosStockMinimo.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductosStockMinimo.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grProductosStockMinimo.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grProductosStockMinimo.HeaderFormatStyle.Alpha = 0
        Me.grProductosStockMinimo.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grProductosStockMinimo.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grProductosStockMinimo.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grProductosStockMinimo.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grProductosStockMinimo.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grProductosStockMinimo.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductosStockMinimo.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grProductosStockMinimo.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grProductosStockMinimo.Location = New System.Drawing.Point(5, 66)
        Me.grProductosStockMinimo.Margin = New System.Windows.Forms.Padding(4)
        Me.grProductosStockMinimo.Name = "grProductosStockMinimo"
        Me.grProductosStockMinimo.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grProductosStockMinimo.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grProductosStockMinimo.RecordNavigator = True
        Me.grProductosStockMinimo.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductosStockMinimo.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grProductosStockMinimo.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grProductosStockMinimo.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grProductosStockMinimo.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grProductosStockMinimo.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grProductosStockMinimo.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductosStockMinimo.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grProductosStockMinimo.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grProductosStockMinimo.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductosStockMinimo.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grProductosStockMinimo.Size = New System.Drawing.Size(978, 291)
        Me.grProductosStockMinimo.TabIndex = 2
        Me.grProductosStockMinimo.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grProductosStockMinimo.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grProductosStockMinimo.TableSpacing = 9
        Me.grProductosStockMinimo.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grProductosStockMinimo.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grProductosStockMinimo.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grProductosStockMinimo.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel6
        '
        Me.Panel6.BackColor = System.Drawing.Color.White
        Me.Panel6.Controls.Add(Me.btnStockMinimo)
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel6.Location = New System.Drawing.Point(5, 5)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(978, 61)
        Me.Panel6.TabIndex = 4
        '
        'btnStockMinimo
        '
        Me.btnStockMinimo.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnStockMinimo.BackColor = System.Drawing.Color.Honeydew
        Me.btnStockMinimo.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnStockMinimo.DisabledImagesGrayScale = False
        Me.btnStockMinimo.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnStockMinimo.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStockMinimo.Image = Global.TeVendo.My.Resources.Resources.sheets
        Me.btnStockMinimo.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnStockMinimo.Location = New System.Drawing.Point(751, 0)
        Me.btnStockMinimo.Margin = New System.Windows.Forms.Padding(4)
        Me.btnStockMinimo.Name = "btnStockMinimo"
        Me.btnStockMinimo.Size = New System.Drawing.Size(227, 61)
        Me.btnStockMinimo.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnStockMinimo.TabIndex = 3
        Me.btnStockMinimo.Text = "Exportar Excel"
        Me.btnStockMinimo.TextColor = System.Drawing.Color.Black
        '
        'tabProductosStockMinimo
        '
        Me.tabProductosStockMinimo.AttachedControl = Me.SuperTabControlPanel5
        Me.tabProductosStockMinimo.GlobalItem = False
        Me.tabProductosStockMinimo.Image = Global.TeVendo.My.Resources.Resources.stock02
        Me.tabProductosStockMinimo.Name = "tabProductosStockMinimo"
        Me.tabProductosStockMinimo.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Teal
        Me.tabProductosStockMinimo.SelectedTabFont = New System.Drawing.Font("Calibri", 12.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tabProductosStockMinimo.SymbolColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.tabProductosStockMinimo.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tabProductosStockMinimo.Text = "Productos Menor Al Stock Minimo"
        Me.tabProductosStockMinimo.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Near
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Title)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(988, 41)
        Me.Panel2.TabIndex = 0
        '
        'Title
        '
        Me.Title.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Title.Controls.Add(Me.Label1)
        Me.Title.Controls.Add(Me.Panel3)
        Me.Title.Controls.Add(Me.PictureBox1)
        Me.Title.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Title.Location = New System.Drawing.Point(0, 0)
        Me.Title.Name = "Title"
        Me.Title.Size = New System.Drawing.Size(988, 41)
        Me.Title.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Label1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(59, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(20, 5, 0, 0)
        Me.Label1.Size = New System.Drawing.Size(267, 41)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Monitorear Productos"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.White
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel3.Location = New System.Drawing.Point(58, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1, 41)
        Me.Panel3.TabIndex = 1
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox1.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox1.Size = New System.Drawing.Size(58, 41)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'btnProductosSinVentas
        '
        Me.btnProductosSinVentas.AttachedControl = Me.SuperTabControlPanel1
        Me.btnProductosSinVentas.GlobalItem = False
        Me.btnProductosSinVentas.Image = Global.TeVendo.My.Resources.Resources.creditospagados
        Me.btnProductosSinVentas.Name = "btnProductosSinVentas"
        Me.btnProductosSinVentas.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Teal
        Me.btnProductosSinVentas.SelectedTabFont = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnProductosSinVentas.Text = "Productos Sin Ventas"
        '
        'SuperTabControlPanel1
        '
        Me.SuperTabControlPanel1.Controls.Add(Me.Panel10)
        Me.SuperTabControlPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel1.Location = New System.Drawing.Point(0, 58)
        Me.SuperTabControlPanel1.Name = "SuperTabControlPanel1"
        Me.SuperTabControlPanel1.Size = New System.Drawing.Size(988, 418)
        Me.SuperTabControlPanel1.TabIndex = 0
        Me.SuperTabControlPanel1.TabItem = Me.btnProductosSinVentas
        '
        'Panel10
        '
        Me.Panel10.BackColor = System.Drawing.Color.White
        Me.Panel10.Controls.Add(Me.Panel11)
        Me.Panel10.Controls.Add(Me.GroupPanel1)
        Me.Panel10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel10.Location = New System.Drawing.Point(0, 0)
        Me.Panel10.Name = "Panel10"
        Me.Panel10.Size = New System.Drawing.Size(988, 418)
        Me.Panel10.TabIndex = 4
        '
        'Panel11
        '
        Me.Panel11.Controls.Add(Me.grProductoSinVentas)
        Me.Panel11.Controls.Add(Me.Panel12)
        Me.Panel11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel11.Location = New System.Drawing.Point(0, 0)
        Me.Panel11.Name = "Panel11"
        Me.Panel11.Padding = New System.Windows.Forms.Padding(5)
        Me.Panel11.Size = New System.Drawing.Size(988, 418)
        Me.Panel11.TabIndex = 7
        '
        'grProductoSinVentas
        '
        Me.grProductoSinVentas.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grProductoSinVentas.AlternatingColors = True
        Me.grProductoSinVentas.BackColor = System.Drawing.Color.WhiteSmoke
        Me.grProductoSinVentas.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grProductoSinVentas.ColumnAutoResize = True
        Me.grProductoSinVentas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grProductoSinVentas.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grProductoSinVentas.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grProductoSinVentas.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductoSinVentas.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grProductoSinVentas.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grProductoSinVentas.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grProductoSinVentas.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductoSinVentas.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grProductoSinVentas.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grProductoSinVentas.HeaderFormatStyle.Alpha = 0
        Me.grProductoSinVentas.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grProductoSinVentas.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grProductoSinVentas.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grProductoSinVentas.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grProductoSinVentas.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grProductoSinVentas.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductoSinVentas.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grProductoSinVentas.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grProductoSinVentas.Location = New System.Drawing.Point(5, 86)
        Me.grProductoSinVentas.Margin = New System.Windows.Forms.Padding(4)
        Me.grProductoSinVentas.Name = "grProductoSinVentas"
        Me.grProductoSinVentas.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grProductoSinVentas.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grProductoSinVentas.RecordNavigator = True
        Me.grProductoSinVentas.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductoSinVentas.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grProductoSinVentas.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grProductoSinVentas.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grProductoSinVentas.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grProductoSinVentas.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grProductoSinVentas.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductoSinVentas.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grProductoSinVentas.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grProductoSinVentas.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProductoSinVentas.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grProductoSinVentas.Size = New System.Drawing.Size(978, 327)
        Me.grProductoSinVentas.TabIndex = 2
        Me.grProductoSinVentas.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grProductoSinVentas.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grProductoSinVentas.TableSpacing = 9
        Me.grProductoSinVentas.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grProductoSinVentas.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grProductoSinVentas.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grProductoSinVentas.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel12
        '
        Me.Panel12.BackColor = System.Drawing.Color.White
        Me.Panel12.Controls.Add(Me.Panel21)
        Me.Panel12.Controls.Add(Me.btnExportarSinVentas)
        Me.Panel12.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel12.Location = New System.Drawing.Point(5, 5)
        Me.Panel12.Name = "Panel12"
        Me.Panel12.Size = New System.Drawing.Size(978, 81)
        Me.Panel12.TabIndex = 4
        '
        'btnExportarSinVentas
        '
        Me.btnExportarSinVentas.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnExportarSinVentas.BackColor = System.Drawing.Color.Honeydew
        Me.btnExportarSinVentas.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnExportarSinVentas.DisabledImagesGrayScale = False
        Me.btnExportarSinVentas.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnExportarSinVentas.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnExportarSinVentas.Image = Global.TeVendo.My.Resources.Resources.sheets
        Me.btnExportarSinVentas.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnExportarSinVentas.Location = New System.Drawing.Point(751, 0)
        Me.btnExportarSinVentas.Margin = New System.Windows.Forms.Padding(4)
        Me.btnExportarSinVentas.Name = "btnExportarSinVentas"
        Me.btnExportarSinVentas.Size = New System.Drawing.Size(227, 81)
        Me.btnExportarSinVentas.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnExportarSinVentas.TabIndex = 3
        Me.btnExportarSinVentas.Text = "Exportar Excel"
        Me.btnExportarSinVentas.TextColor = System.Drawing.Color.Black
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.Panel13)
        Me.GroupPanel1.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel1.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(988, 418)
        '
        '
        '
        Me.GroupPanel1.Style.BackColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.GroupPanel1.Style.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.GroupPanel1.Style.BackColorGradientAngle = 90
        Me.GroupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderBottomWidth = 1
        Me.GroupPanel1.Style.BorderColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.GroupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderLeftWidth = 1
        Me.GroupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderRightWidth = 1
        Me.GroupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel1.Style.BorderTopWidth = 1
        Me.GroupPanel1.Style.CornerDiameter = 4
        Me.GroupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel1.Style.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
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
        Me.GroupPanel1.TabIndex = 2
        Me.GroupPanel1.Text = "Listado De Pagos"
        Me.GroupPanel1.Visible = False
        '
        'Panel13
        '
        Me.Panel13.BackColor = System.Drawing.Color.White
        Me.Panel13.Controls.Add(Me.GridEX2)
        Me.Panel13.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel13.Location = New System.Drawing.Point(0, 0)
        Me.Panel13.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel13.Name = "Panel13"
        Me.Panel13.Size = New System.Drawing.Size(982, 391)
        Me.Panel13.TabIndex = 0
        '
        'GridEX2
        '
        Me.GridEX2.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.GridEX2.AlternatingColors = True
        Me.GridEX2.BackColor = System.Drawing.Color.White
        Me.GridEX2.BorderStyle = Janus.Windows.GridEX.BorderStyle.Raised
        Me.GridEX2.ColumnAutoResize = True
        Me.GridEX2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridEX2.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.GridEX2.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GridEX2.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.GridEX2.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.GridEX2.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.GridEX2.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GridEX2.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.GridEX2.HeaderFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.GridEX2.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.Empty
        Me.GridEX2.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GridEX2.HeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.GridEX2.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.GridEX2.Location = New System.Drawing.Point(0, 0)
        Me.GridEX2.Margin = New System.Windows.Forms.Padding(4)
        Me.GridEX2.Name = "GridEX2"
        Me.GridEX2.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.GridEX2.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.GridEX2.RecordNavigator = True
        Me.GridEX2.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GridEX2.RowHeaderFormatStyle.BackColor = System.Drawing.Color.DodgerBlue
        Me.GridEX2.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GridEX2.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.GridEX2.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.GridEX2.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GridEX2.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.GridEX2.SelectedInactiveFormatStyle.BackColor = System.Drawing.Color.WhiteSmoke
        Me.GridEX2.Size = New System.Drawing.Size(982, 391)
        Me.GridEX2.TabIndex = 3
        Me.GridEX2.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.GridEX2.TableSpacing = 9
        Me.GridEX2.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.GridEX2.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.GridEX2.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.GridEX2.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'Panel21
        '
        Me.Panel21.AutoScroll = True
        Me.Panel21.Controls.Add(Me.btnSinVentas)
        Me.Panel21.Controls.Add(Me.tbHasta)
        Me.Panel21.Controls.Add(Me.LabelX16)
        Me.Panel21.Controls.Add(Me.tbDesde)
        Me.Panel21.Controls.Add(Me.LabelX6)
        Me.Panel21.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel21.Location = New System.Drawing.Point(0, 0)
        Me.Panel21.Name = "Panel21"
        Me.Panel21.Size = New System.Drawing.Size(751, 81)
        Me.Panel21.TabIndex = 6
        '
        'btnSinVentas
        '
        Me.btnSinVentas.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSinVentas.BackColor = System.Drawing.Color.DarkOrange
        Me.btnSinVentas.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnSinVentas.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSinVentas.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnSinVentas.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.btnSinVentas.Location = New System.Drawing.Point(532, 18)
        Me.btnSinVentas.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSinVentas.Name = "btnSinVentas"
        Me.btnSinVentas.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnSinVentas.Size = New System.Drawing.Size(160, 34)
        Me.btnSinVentas.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSinVentas.Symbol = ""
        Me.btnSinVentas.SymbolColor = System.Drawing.Color.White
        Me.btnSinVentas.SymbolSize = 15.0!
        Me.btnSinVentas.TabIndex = 222
        Me.btnSinVentas.Text = "Filtrar"
        Me.btnSinVentas.TextColor = System.Drawing.Color.White
        '
        'tbHasta
        '
        Me.tbHasta.BackColor = System.Drawing.Color.White
        Me.tbHasta.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
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
        Me.tbHasta.Location = New System.Drawing.Point(322, 20)
        Me.tbHasta.Name = "tbHasta"
        Me.tbHasta.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbHasta.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbHasta.SecondIncrement = 10
        Me.tbHasta.Size = New System.Drawing.Size(157, 26)
        Me.tbHasta.TabIndex = 220
        Me.tbHasta.TodayButtonText = "Hoy"
        Me.tbHasta.UseCompatibleTextRendering = False
        Me.tbHasta.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.tbHasta.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbHasta.YearIncrement = 10
        '
        'LabelX16
        '
        Me.LabelX16.AutoSize = True
        Me.LabelX16.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX16.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX16.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX16.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX16.Location = New System.Drawing.Point(261, 23)
        Me.LabelX16.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX16.Name = "LabelX16"
        Me.LabelX16.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX16.Size = New System.Drawing.Size(42, 21)
        Me.LabelX16.TabIndex = 221
        Me.LabelX16.Text = "Hasta:"
        '
        'tbDesde
        '
        Me.tbDesde.BackColor = System.Drawing.Color.White
        Me.tbDesde.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
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
        Me.tbDesde.Location = New System.Drawing.Point(97, 20)
        Me.tbDesde.Name = "tbDesde"
        Me.tbDesde.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbDesde.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbDesde.SecondIncrement = 10
        Me.tbDesde.Size = New System.Drawing.Size(157, 26)
        Me.tbDesde.TabIndex = 218
        Me.tbDesde.TodayButtonText = "Hoy"
        Me.tbDesde.UseCompatibleTextRendering = False
        Me.tbDesde.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.tbDesde.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbDesde.YearIncrement = 10
        '
        'LabelX6
        '
        Me.LabelX6.AutoSize = True
        Me.LabelX6.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX6.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX6.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX6.Location = New System.Drawing.Point(24, 24)
        Me.LabelX6.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX6.Name = "LabelX6"
        Me.LabelX6.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX6.Size = New System.Drawing.Size(45, 21)
        Me.LabelX6.TabIndex = 219
        Me.LabelX6.Text = "Desde:"
        '
        'F_MonitoreoProductos
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(988, 517)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "F_MonitoreoProductos"
        Me.Text = "Monitoreo de Productos"
        Me.Panel1.ResumeLayout(False)
        CType(Me.SuperTabControlMenu, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SuperTabControlMenu.ResumeLayout(False)
        Me.SuperTabControlPanel3.ResumeLayout(False)
        Me.Panel17.ResumeLayout(False)
        CType(Me.gr_ProductosStock0, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ResumeLayout(False)
        Me.SuperTabControlPanel6.ResumeLayout(False)
        Me.PanelDatos.ResumeLayout(False)
        Me.Panel8.ResumeLayout(False)
        CType(Me.gr_ProductosVencidos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel9.ResumeLayout(False)
        Me.GroupPanel4.ResumeLayout(False)
        Me.Panel7.ResumeLayout(False)
        CType(Me.grPagos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SuperTabControlPanel5.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        CType(Me.grProductosStockMinimo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel6.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Title.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SuperTabControlPanel1.ResumeLayout(False)
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        CType(Me.grProductoSinVentas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel12.ResumeLayout(False)
        Me.GroupPanel1.ResumeLayout(False)
        Me.Panel13.ResumeLayout(False)
        CType(Me.GridEX2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel21.ResumeLayout(False)
        Me.Panel21.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents SuperTabControlMenu As DevComponents.DotNetBar.SuperTabControl
    Friend WithEvents SuperTabControlPanel3 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents Panel17 As Panel
    Friend WithEvents gr_ProductosStock0 As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel4 As Panel
    Protected WithEvents btnProductosSinStock As DevComponents.DotNetBar.ButtonX
    Friend WithEvents tabProductosSinStock As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents SuperTabControlPanel6 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents PanelDatos As Panel
    Friend WithEvents GroupPanel4 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Panel7 As Panel
    Friend WithEvents grPagos As Janus.Windows.GridEX.GridEX
    Friend WithEvents btnProductosVencidos As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents SuperTabControlPanel5 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents Panel5 As Panel
    Friend WithEvents grProductosStockMinimo As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel6 As Panel
    Friend WithEvents tabProductosStockMinimo As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Title As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Panel8 As Panel
    Friend WithEvents gr_ProductosVencidos As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel9 As Panel
    Protected WithEvents ButtonX3 As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnStockMinimo As DevComponents.DotNetBar.ButtonX
    Friend WithEvents SuperTabControlPanel1 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents Panel10 As Panel
    Friend WithEvents Panel11 As Panel
    Friend WithEvents grProductoSinVentas As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel12 As Panel
    Protected WithEvents btnExportarSinVentas As DevComponents.DotNetBar.ButtonX
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Panel13 As Panel
    Friend WithEvents GridEX2 As Janus.Windows.GridEX.GridEX
    Friend WithEvents btnProductosSinVentas As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents Panel21 As Panel
    Friend WithEvents btnSinVentas As DevComponents.DotNetBar.ButtonX
    Friend WithEvents tbHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX16 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX6 As DevComponents.DotNetBar.LabelX
End Class
