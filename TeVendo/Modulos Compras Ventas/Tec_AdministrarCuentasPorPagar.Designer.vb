<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_AdministrarCuentasPorPagar
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Tec_AdministrarCuentasPorPagar))
        Dim SuperTabColorTable1 As DevComponents.DotNetBar.Rendering.SuperTabColorTable = New DevComponents.DotNetBar.Rendering.SuperTabColorTable()
        Dim SuperTabLinearGradientColorTable1 As DevComponents.DotNetBar.Rendering.SuperTabLinearGradientColorTable = New DevComponents.DotNetBar.Rendering.SuperTabLinearGradientColorTable()
        Me.btnCompras = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.btnCompraCredito = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.btnProveedores = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.btInvAmacen = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.MetroTileItem30 = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.btZona = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.btZonaMapaCliente = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.SuperTabControlMenu = New DevComponents.DotNetBar.SuperTabControl()
        Me.SuperTabControlPanel3 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel17 = New System.Windows.Forms.Panel()
        Me.gr_CreditoPendientes = New Janus.Windows.GridEX.GridEX()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.tab_configuraciones = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel6 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.tab03 = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel5 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.tab_compraventa = New DevComponents.DotNetBar.SuperTabItem()
        Me.MetroTileItem1 = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.MetroTileItem2 = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.MetroTileItem3 = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.MetroTileItem4 = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.MetroTileItem5 = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.MetroTileItem6 = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.MetroTileItem7 = New DevComponents.DotNetBar.Metro.MetroTileItem()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Title = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.SuperTabControlMenu, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuperTabControlMenu.SuspendLayout()
        Me.SuperTabControlPanel3.SuspendLayout()
        Me.Panel17.SuspendLayout()
        CType(Me.gr_CreditoPendientes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Title.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnCompras
        '
        Me.btnCompras.Image = CType(resources.GetObject("btnCompras.Image"), System.Drawing.Image)
        Me.btnCompras.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.btnCompras.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnCompras.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.btnCompras.Name = "btnCompras"
        Me.btnCompras.SymbolColor = System.Drawing.Color.Black
        Me.btnCompras.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.btnCompras.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.btnCompras.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(159, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btnCompras.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(168, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.btnCompras.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.btnCompras.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCompras.TileStyle.PaddingRight = 30
        Me.btnCompras.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.btnCompras.TitleText = "Gestionar " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Compras"
        Me.btnCompras.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.btnCompras.TitleTextColor = System.Drawing.Color.White
        '
        'btnCompraCredito
        '
        Me.btnCompraCredito.Image = Global.TeVendo.My.Resources.Resources.credito
        Me.btnCompraCredito.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.btnCompraCredito.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnCompraCredito.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.btnCompraCredito.Name = "btnCompraCredito"
        Me.btnCompraCredito.SymbolColor = System.Drawing.Color.Black
        Me.btnCompraCredito.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.btnCompraCredito.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.btnCompraCredito.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(189, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnCompraCredito.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(132, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(132, Byte), Integer))
        Me.btnCompraCredito.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.btnCompraCredito.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCompraCredito.TileStyle.PaddingRight = 10
        Me.btnCompraCredito.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.btnCompraCredito.TitleText = "Administrar" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Cuentas Por" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Pagar" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.btnCompraCredito.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.btnCompraCredito.TitleTextColor = System.Drawing.Color.White
        '
        'btnProveedores
        '
        Me.btnProveedores.Image = CType(resources.GetObject("btnProveedores.Image"), System.Drawing.Image)
        Me.btnProveedores.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.btnProveedores.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnProveedores.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.btnProveedores.Name = "btnProveedores"
        Me.btnProveedores.SymbolColor = System.Drawing.Color.Black
        Me.btnProveedores.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.btnProveedores.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.btnProveedores.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(159, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btnProveedores.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(168, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.btnProveedores.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.btnProveedores.TileStyle.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnProveedores.TileStyle.PaddingRight = 20
        Me.btnProveedores.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.btnProveedores.TitleText = "Gestion De" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & " Proveedores"
        Me.btnProveedores.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.btnProveedores.TitleTextColor = System.Drawing.Color.White
        '
        'btInvAmacen
        '
        Me.btInvAmacen.Image = CType(resources.GetObject("btInvAmacen.Image"), System.Drawing.Image)
        Me.btInvAmacen.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.btInvAmacen.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.btInvAmacen.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.btInvAmacen.Name = "btInvAmacen"
        Me.btInvAmacen.SymbolColor = System.Drawing.Color.Black
        Me.btInvAmacen.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.btInvAmacen.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.btInvAmacen.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(189, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btInvAmacen.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(132, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(132, Byte), Integer))
        Me.btInvAmacen.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.btInvAmacen.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btInvAmacen.TileStyle.PaddingRight = 20
        Me.btInvAmacen.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.btInvAmacen.TitleText = "Gestion De" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & " Sucursales"
        Me.btInvAmacen.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.btInvAmacen.TitleTextColor = System.Drawing.Color.White
        '
        'MetroTileItem30
        '
        Me.MetroTileItem30.Image = CType(resources.GetObject("MetroTileItem30.Image"), System.Drawing.Image)
        Me.MetroTileItem30.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.MetroTileItem30.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.MetroTileItem30.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.MetroTileItem30.Name = "MetroTileItem30"
        Me.MetroTileItem30.SymbolColor = System.Drawing.Color.Black
        Me.MetroTileItem30.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.MetroTileItem30.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.MetroTileItem30.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(159, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.MetroTileItem30.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(168, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.MetroTileItem30.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.MetroTileItem30.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MetroTileItem30.TileStyle.PaddingRight = 20
        Me.MetroTileItem30.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.MetroTileItem30.TitleText = "Ingresos " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Por Sucursal"
        Me.MetroTileItem30.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.MetroTileItem30.TitleTextColor = System.Drawing.Color.White
        '
        'btZona
        '
        Me.btZona.Image = CType(resources.GetObject("btZona.Image"), System.Drawing.Image)
        Me.btZona.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.btZona.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.btZona.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.btZona.Name = "btZona"
        Me.btZona.SymbolColor = System.Drawing.Color.Black
        Me.btZona.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.btZona.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.btZona.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(159, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.btZona.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(168, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.btZona.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.btZona.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btZona.TileStyle.PaddingRight = 20
        Me.btZona.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.btZona.TitleText = "Gestion De" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Zonas"
        Me.btZona.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.btZona.TitleTextColor = System.Drawing.Color.White
        '
        'btZonaMapaCliente
        '
        Me.btZonaMapaCliente.Image = CType(resources.GetObject("btZonaMapaCliente.Image"), System.Drawing.Image)
        Me.btZonaMapaCliente.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.btZonaMapaCliente.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.btZonaMapaCliente.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.btZonaMapaCliente.Name = "btZonaMapaCliente"
        Me.btZonaMapaCliente.SymbolColor = System.Drawing.Color.Black
        Me.btZonaMapaCliente.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.btZonaMapaCliente.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.btZonaMapaCliente.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(189, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btZonaMapaCliente.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(132, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(132, Byte), Integer))
        Me.btZonaMapaCliente.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.btZonaMapaCliente.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btZonaMapaCliente.TileStyle.PaddingRight = 20
        Me.btZonaMapaCliente.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.btZonaMapaCliente.TitleText = "Ubicacion De" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Clientes"
        Me.btZonaMapaCliente.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.btZonaMapaCliente.TitleTextColor = System.Drawing.Color.White
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
        Me.SuperTabControlMenu.Controls.Add(Me.SuperTabControlPanel3)
        Me.SuperTabControlMenu.Controls.Add(Me.SuperTabControlPanel5)
        Me.SuperTabControlMenu.Controls.Add(Me.SuperTabControlPanel6)
        Me.SuperTabControlMenu.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlMenu.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabControlMenu.ForeColor = System.Drawing.Color.Black
        Me.SuperTabControlMenu.Location = New System.Drawing.Point(0, 41)
        Me.SuperTabControlMenu.Name = "SuperTabControlMenu"
        Me.SuperTabControlMenu.ReorderTabsEnabled = True
        Me.SuperTabControlMenu.SelectedTabFont = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabControlMenu.SelectedTabIndex = 0
        Me.SuperTabControlMenu.Size = New System.Drawing.Size(1064, 503)
        Me.SuperTabControlMenu.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabControlMenu.TabIndex = 0
        Me.SuperTabControlMenu.TabLayoutType = DevComponents.DotNetBar.eSuperTabLayoutType.MultiLineFit
        Me.SuperTabControlMenu.Tabs.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.tab_configuraciones, Me.tab_compraventa, Me.tab03})
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
        Me.SuperTabControlPanel3.Size = New System.Drawing.Size(1064, 445)
        Me.SuperTabControlPanel3.TabIndex = 3
        Me.SuperTabControlPanel3.TabItem = Me.tab_configuraciones
        '
        'Panel17
        '
        Me.Panel17.Controls.Add(Me.gr_CreditoPendientes)
        Me.Panel17.Controls.Add(Me.Panel4)
        Me.Panel17.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel17.Location = New System.Drawing.Point(0, 0)
        Me.Panel17.Name = "Panel17"
        Me.Panel17.Padding = New System.Windows.Forms.Padding(5)
        Me.Panel17.Size = New System.Drawing.Size(1064, 445)
        Me.Panel17.TabIndex = 5
        '
        'gr_CreditoPendientes
        '
        Me.gr_CreditoPendientes.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.gr_CreditoPendientes.AlternatingColors = True
        Me.gr_CreditoPendientes.BackColor = System.Drawing.Color.WhiteSmoke
        Me.gr_CreditoPendientes.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.gr_CreditoPendientes.ColumnAutoResize = True
        Me.gr_CreditoPendientes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gr_CreditoPendientes.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.gr_CreditoPendientes.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.gr_CreditoPendientes.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_CreditoPendientes.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.gr_CreditoPendientes.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.gr_CreditoPendientes.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.gr_CreditoPendientes.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_CreditoPendientes.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.gr_CreditoPendientes.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.gr_CreditoPendientes.HeaderFormatStyle.Alpha = 0
        Me.gr_CreditoPendientes.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.gr_CreditoPendientes.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.gr_CreditoPendientes.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.gr_CreditoPendientes.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.gr_CreditoPendientes.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.gr_CreditoPendientes.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_CreditoPendientes.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.gr_CreditoPendientes.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.gr_CreditoPendientes.Location = New System.Drawing.Point(5, 66)
        Me.gr_CreditoPendientes.Margin = New System.Windows.Forms.Padding(4)
        Me.gr_CreditoPendientes.Name = "gr_CreditoPendientes"
        Me.gr_CreditoPendientes.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.gr_CreditoPendientes.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.gr_CreditoPendientes.RecordNavigator = True
        Me.gr_CreditoPendientes.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_CreditoPendientes.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.gr_CreditoPendientes.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.gr_CreditoPendientes.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.gr_CreditoPendientes.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.gr_CreditoPendientes.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.gr_CreditoPendientes.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_CreditoPendientes.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.gr_CreditoPendientes.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.gr_CreditoPendientes.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gr_CreditoPendientes.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.gr_CreditoPendientes.Size = New System.Drawing.Size(1054, 374)
        Me.gr_CreditoPendientes.TabIndex = 2
        Me.gr_CreditoPendientes.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.gr_CreditoPendientes.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.gr_CreditoPendientes.TableSpacing = 9
        Me.gr_CreditoPendientes.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.gr_CreditoPendientes.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.gr_CreditoPendientes.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.gr_CreditoPendientes.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.White
        Me.Panel4.Controls.Add(Me.ButtonX1)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(5, 5)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1054, 61)
        Me.Panel4.TabIndex = 4
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX1.DisabledImagesGrayScale = False
        Me.ButtonX1.Dock = System.Windows.Forms.DockStyle.Right
        Me.ButtonX1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.printee
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.ButtonX1.Location = New System.Drawing.Point(827, 0)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(227, 61)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX1.TabIndex = 2
        Me.ButtonX1.Text = "Imprimir Reporte"
        Me.ButtonX1.TextColor = System.Drawing.Color.White
        '
        'tab_configuraciones
        '
        Me.tab_configuraciones.AttachedControl = Me.SuperTabControlPanel3
        Me.tab_configuraciones.GlobalItem = False
        Me.tab_configuraciones.Image = Global.TeVendo.My.Resources.Resources.creditopagados
        Me.tab_configuraciones.Name = "tab_configuraciones"
        Me.tab_configuraciones.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Teal
        Me.tab_configuraciones.SelectedTabFont = New System.Drawing.Font("Calibri", 12.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tab_configuraciones.SymbolColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.tab_configuraciones.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tab_configuraciones.Text = "Creditos Pendientes"
        Me.tab_configuraciones.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Near
        '
        'SuperTabControlPanel6
        '
        Me.SuperTabControlPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel6.Location = New System.Drawing.Point(0, 58)
        Me.SuperTabControlPanel6.Name = "SuperTabControlPanel6"
        Me.SuperTabControlPanel6.Size = New System.Drawing.Size(1064, 445)
        Me.SuperTabControlPanel6.TabIndex = 6
        Me.SuperTabControlPanel6.TabItem = Me.tab03
        '
        'tab03
        '
        Me.tab03.AttachedControl = Me.SuperTabControlPanel6
        Me.tab03.GlobalItem = False
        Me.tab03.Image = Global.TeVendo.My.Resources.Resources.pagos
        Me.tab03.Name = "tab03"
        Me.tab03.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Teal
        Me.tab03.SelectedTabFont = New System.Drawing.Font("Calibri", 12.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tab03.SymbolColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.tab03.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tab03.Text = "Pagar Credito"
        Me.tab03.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Near
        '
        'SuperTabControlPanel5
        '
        Me.SuperTabControlPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel5.Location = New System.Drawing.Point(0, 58)
        Me.SuperTabControlPanel5.Name = "SuperTabControlPanel5"
        Me.SuperTabControlPanel5.Size = New System.Drawing.Size(1064, 445)
        Me.SuperTabControlPanel5.TabIndex = 5
        Me.SuperTabControlPanel5.TabItem = Me.tab_compraventa
        '
        'tab_compraventa
        '
        Me.tab_compraventa.AttachedControl = Me.SuperTabControlPanel5
        Me.tab_compraventa.GlobalItem = False
        Me.tab_compraventa.Image = Global.TeVendo.My.Resources.Resources.creditospagados
        Me.tab_compraventa.Name = "tab_compraventa"
        Me.tab_compraventa.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Teal
        Me.tab_compraventa.SelectedTabFont = New System.Drawing.Font("Calibri", 12.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tab_compraventa.SymbolColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.tab_compraventa.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tab_compraventa.Text = "Creditos Pagados"
        Me.tab_compraventa.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Near
        '
        'MetroTileItem1
        '
        Me.MetroTileItem1.Image = CType(resources.GetObject("MetroTileItem1.Image"), System.Drawing.Image)
        Me.MetroTileItem1.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.MetroTileItem1.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.MetroTileItem1.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.MetroTileItem1.Name = "MetroTileItem1"
        Me.MetroTileItem1.SymbolColor = System.Drawing.Color.Black
        Me.MetroTileItem1.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.MetroTileItem1.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.MetroTileItem1.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(159, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.MetroTileItem1.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(168, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.MetroTileItem1.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.MetroTileItem1.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MetroTileItem1.TileStyle.PaddingRight = 30
        Me.MetroTileItem1.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.MetroTileItem1.TitleText = "Gestionar " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Compras"
        Me.MetroTileItem1.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.MetroTileItem1.TitleTextColor = System.Drawing.Color.White
        '
        'MetroTileItem2
        '
        Me.MetroTileItem2.Image = Global.TeVendo.My.Resources.Resources.credito
        Me.MetroTileItem2.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.MetroTileItem2.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.MetroTileItem2.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.MetroTileItem2.Name = "MetroTileItem2"
        Me.MetroTileItem2.SymbolColor = System.Drawing.Color.Black
        Me.MetroTileItem2.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.MetroTileItem2.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.MetroTileItem2.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(189, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.MetroTileItem2.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(132, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(132, Byte), Integer))
        Me.MetroTileItem2.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.MetroTileItem2.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MetroTileItem2.TileStyle.PaddingRight = 10
        Me.MetroTileItem2.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.MetroTileItem2.TitleText = "Administrar" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Cuentas Por" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Pagar" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.MetroTileItem2.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.MetroTileItem2.TitleTextColor = System.Drawing.Color.White
        '
        'MetroTileItem3
        '
        Me.MetroTileItem3.Image = CType(resources.GetObject("MetroTileItem3.Image"), System.Drawing.Image)
        Me.MetroTileItem3.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.MetroTileItem3.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.MetroTileItem3.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.MetroTileItem3.Name = "MetroTileItem3"
        Me.MetroTileItem3.SymbolColor = System.Drawing.Color.Black
        Me.MetroTileItem3.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.MetroTileItem3.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.MetroTileItem3.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(159, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.MetroTileItem3.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(168, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.MetroTileItem3.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.MetroTileItem3.TileStyle.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MetroTileItem3.TileStyle.PaddingRight = 20
        Me.MetroTileItem3.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.MetroTileItem3.TitleText = "Gestion De" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & " Proveedores"
        Me.MetroTileItem3.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.MetroTileItem3.TitleTextColor = System.Drawing.Color.White
        '
        'MetroTileItem4
        '
        Me.MetroTileItem4.Image = CType(resources.GetObject("MetroTileItem4.Image"), System.Drawing.Image)
        Me.MetroTileItem4.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.MetroTileItem4.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.MetroTileItem4.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.MetroTileItem4.Name = "MetroTileItem4"
        Me.MetroTileItem4.SymbolColor = System.Drawing.Color.Black
        Me.MetroTileItem4.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.MetroTileItem4.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.MetroTileItem4.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(189, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.MetroTileItem4.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(132, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(132, Byte), Integer))
        Me.MetroTileItem4.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.MetroTileItem4.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MetroTileItem4.TileStyle.PaddingRight = 20
        Me.MetroTileItem4.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.MetroTileItem4.TitleText = "Gestion De" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & " Sucursales"
        Me.MetroTileItem4.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.MetroTileItem4.TitleTextColor = System.Drawing.Color.White
        '
        'MetroTileItem5
        '
        Me.MetroTileItem5.Image = CType(resources.GetObject("MetroTileItem5.Image"), System.Drawing.Image)
        Me.MetroTileItem5.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.MetroTileItem5.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.MetroTileItem5.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.MetroTileItem5.Name = "MetroTileItem5"
        Me.MetroTileItem5.SymbolColor = System.Drawing.Color.Black
        Me.MetroTileItem5.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.MetroTileItem5.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.MetroTileItem5.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(159, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.MetroTileItem5.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(168, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.MetroTileItem5.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.MetroTileItem5.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MetroTileItem5.TileStyle.PaddingRight = 20
        Me.MetroTileItem5.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.MetroTileItem5.TitleText = "Ingresos " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Por Sucursal"
        Me.MetroTileItem5.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.MetroTileItem5.TitleTextColor = System.Drawing.Color.White
        '
        'MetroTileItem6
        '
        Me.MetroTileItem6.Image = CType(resources.GetObject("MetroTileItem6.Image"), System.Drawing.Image)
        Me.MetroTileItem6.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.MetroTileItem6.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.MetroTileItem6.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.MetroTileItem6.Name = "MetroTileItem6"
        Me.MetroTileItem6.SymbolColor = System.Drawing.Color.Black
        Me.MetroTileItem6.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.MetroTileItem6.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.MetroTileItem6.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(159, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.MetroTileItem6.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(168, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.MetroTileItem6.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.MetroTileItem6.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MetroTileItem6.TileStyle.PaddingRight = 20
        Me.MetroTileItem6.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.MetroTileItem6.TitleText = "Gestion De" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Zonas"
        Me.MetroTileItem6.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.MetroTileItem6.TitleTextColor = System.Drawing.Color.White
        '
        'MetroTileItem7
        '
        Me.MetroTileItem7.Image = CType(resources.GetObject("MetroTileItem7.Image"), System.Drawing.Image)
        Me.MetroTileItem7.ImageIndent = New System.Drawing.Point(-50, 0)
        Me.MetroTileItem7.ImageTextAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.MetroTileItem7.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far
        Me.MetroTileItem7.Name = "MetroTileItem7"
        Me.MetroTileItem7.SymbolColor = System.Drawing.Color.Black
        Me.MetroTileItem7.TileColor = DevComponents.DotNetBar.Metro.eMetroTileColor.Azure
        Me.MetroTileItem7.TileSize = New System.Drawing.Size(250, 135)
        '
        '
        '
        Me.MetroTileItem7.TileStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(189, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.MetroTileItem7.TileStyle.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(132, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(132, Byte), Integer))
        Me.MetroTileItem7.TileStyle.CornerType = DevComponents.DotNetBar.eCornerType.Diagonal
        Me.MetroTileItem7.TileStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MetroTileItem7.TileStyle.PaddingRight = 20
        Me.MetroTileItem7.TileStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.MetroTileItem7.TitleText = "Ubicacion De" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Clientes"
        Me.MetroTileItem7.TitleTextAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.MetroTileItem7.TitleTextColor = System.Drawing.Color.White
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.SuperTabControlMenu)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1064, 544)
        Me.Panel1.TabIndex = 0
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Title)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1064, 41)
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
        Me.Title.Size = New System.Drawing.Size(1064, 41)
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
        Me.Label1.Text = "Administrar Cuentas por Pagar"
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
        'Tec_AdministrarCuentasPorPagar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1064, 544)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Tec_AdministrarCuentasPorPagar"
        Me.Text = "Cuentas Por Pagar"
        CType(Me.SuperTabControlMenu, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SuperTabControlMenu.ResumeLayout(False)
        Me.SuperTabControlPanel3.ResumeLayout(False)
        Me.Panel17.ResumeLayout(False)
        CType(Me.gr_CreditoPendientes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Title.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnCompras As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents btnCompraCredito As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents btnProveedores As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents btInvAmacen As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents MetroTileItem30 As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents btZona As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents btZonaMapaCliente As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents SuperTabControlMenu As DevComponents.DotNetBar.SuperTabControl
    Friend WithEvents SuperTabControlPanel3 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents tab_configuraciones As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents SuperTabControlPanel5 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents tab_compraventa As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents SuperTabControlPanel6 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents tab03 As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents MetroTileItem1 As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents MetroTileItem2 As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents MetroTileItem3 As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents MetroTileItem4 As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents MetroTileItem5 As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents MetroTileItem6 As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents MetroTileItem7 As DevComponents.DotNetBar.Metro.MetroTileItem
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Title As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Panel17 As Panel
    Friend WithEvents gr_CreditoPendientes As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel4 As Panel
    Protected WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
End Class
