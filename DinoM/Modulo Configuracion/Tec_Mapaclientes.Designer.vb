<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Tec_Mapaclientes
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim cbZona_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Tec_Mapaclientes))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PanelSuperior = New System.Windows.Forms.Panel()
        Me.PanelLEft = New System.Windows.Forms.Panel()
        Me.Panel8 = New System.Windows.Forms.Panel()
        Me.PanelDatos = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.PanelZona = New System.Windows.Forms.Panel()
        Me.Zonas = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.PanelTable = New System.Windows.Forms.Panel()
        Me.grCliente = New Janus.Windows.GridEX.GridEX()
        Me.GroupPanel3 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.checkTodos = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.cbZona = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.PanelMapa = New System.Windows.Forms.Panel()
        Me.btnz2 = New DevComponents.DotNetBar.ButtonX()
        Me.btnz1 = New DevComponents.DotNetBar.ButtonX()
        Me.Gmc_Cliente = New GMap.NET.WindowsForms.GMapControl()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.PanelButton = New System.Windows.Forms.Panel()
        Me.Panel1.SuspendLayout()
        Me.PanelSuperior.SuspendLayout()
        Me.PanelLEft.SuspendLayout()
        Me.Panel8.SuspendLayout()
        Me.PanelDatos.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.PanelZona.SuspendLayout()
        Me.Zonas.SuspendLayout()
        Me.PanelTable.SuspendLayout()
        CType(Me.grCliente, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        CType(Me.cbZona, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelMapa.SuspendLayout()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.PanelSuperior)
        Me.Panel1.Controls.Add(Me.PanelButton)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1319, 689)
        Me.Panel1.TabIndex = 4
        '
        'PanelSuperior
        '
        Me.PanelSuperior.Controls.Add(Me.PanelLEft)
        Me.PanelSuperior.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelSuperior.Location = New System.Drawing.Point(0, 0)
        Me.PanelSuperior.Name = "PanelSuperior"
        Me.PanelSuperior.Size = New System.Drawing.Size(1319, 666)
        Me.PanelSuperior.TabIndex = 1
        '
        'PanelLEft
        '
        Me.PanelLEft.Controls.Add(Me.Panel8)
        Me.PanelLEft.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelLEft.Location = New System.Drawing.Point(0, 0)
        Me.PanelLEft.Name = "PanelLEft"
        Me.PanelLEft.Size = New System.Drawing.Size(1319, 666)
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
        Me.Panel8.Size = New System.Drawing.Size(1319, 666)
        Me.Panel8.TabIndex = 1
        '
        'PanelDatos
        '
        Me.PanelDatos.BackColor = System.Drawing.Color.White
        Me.PanelDatos.Controls.Add(Me.TableLayoutPanel1)
        Me.PanelDatos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDatos.Location = New System.Drawing.Point(3, 30)
        Me.PanelDatos.Name = "PanelDatos"
        Me.PanelDatos.Size = New System.Drawing.Size(1313, 633)
        Me.PanelDatos.TabIndex = 2
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.PanelZona, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.PanelMapa, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1313, 633)
        Me.TableLayoutPanel1.TabIndex = 23
        '
        'PanelZona
        '
        Me.PanelZona.Controls.Add(Me.Zonas)
        Me.PanelZona.Controls.Add(Me.GroupPanel3)
        Me.PanelZona.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelZona.Location = New System.Drawing.Point(4, 4)
        Me.PanelZona.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelZona.Name = "PanelZona"
        Me.PanelZona.Size = New System.Drawing.Size(517, 625)
        Me.PanelZona.TabIndex = 20
        '
        'Zonas
        '
        Me.Zonas.CanvasColor = System.Drawing.SystemColors.Control
        Me.Zonas.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.Zonas.Controls.Add(Me.PanelTable)
        Me.Zonas.DisabledBackColor = System.Drawing.Color.Empty
        Me.Zonas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Zonas.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Zonas.Location = New System.Drawing.Point(0, 77)
        Me.Zonas.Margin = New System.Windows.Forms.Padding(4)
        Me.Zonas.Name = "Zonas"
        Me.Zonas.Size = New System.Drawing.Size(517, 548)
        '
        '
        '
        Me.Zonas.Style.BackColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.Zonas.Style.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.Zonas.Style.BackColorGradientAngle = 90
        Me.Zonas.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.Zonas.Style.BorderBottomWidth = 1
        Me.Zonas.Style.BorderColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.Zonas.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.Zonas.Style.BorderLeftWidth = 1
        Me.Zonas.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.Zonas.Style.BorderRightWidth = 1
        Me.Zonas.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.Zonas.Style.BorderTopWidth = 1
        Me.Zonas.Style.CornerDiameter = 4
        Me.Zonas.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.Zonas.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.Zonas.Style.TextColor = System.Drawing.Color.White
        Me.Zonas.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.Zonas.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.Zonas.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.Zonas.TabIndex = 2
        Me.Zonas.Text = "ZONAS"
        '
        'PanelTable
        '
        Me.PanelTable.AutoScroll = True
        Me.PanelTable.BackColor = System.Drawing.Color.White
        Me.PanelTable.Controls.Add(Me.grCliente)
        Me.PanelTable.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelTable.Location = New System.Drawing.Point(0, 0)
        Me.PanelTable.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelTable.Name = "PanelTable"
        Me.PanelTable.Size = New System.Drawing.Size(511, 519)
        Me.PanelTable.TabIndex = 0
        '
        'grCliente
        '
        Me.grCliente.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grCliente.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grCliente.FlatBorderColor = System.Drawing.Color.DodgerBlue
        Me.grCliente.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grCliente.FocusStyle = Janus.Windows.GridEX.FocusStyle.Solid
        Me.grCliente.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCliente.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grCliente.GroupRowVisualStyle = Janus.Windows.GridEX.GroupRowVisualStyle.UseRowStyle
        Me.grCliente.HeaderFormatStyle.Font = New System.Drawing.Font("Georgia", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grCliente.HeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.grCliente.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grCliente.Location = New System.Drawing.Point(0, 0)
        Me.grCliente.Margin = New System.Windows.Forms.Padding(4)
        Me.grCliente.Name = "grCliente"
        Me.grCliente.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grCliente.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grCliente.SelectedFormatStyle.BackColor = System.Drawing.Color.Gold
        Me.grCliente.Size = New System.Drawing.Size(511, 519)
        Me.grCliente.TabIndex = 0
        Me.grCliente.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'GroupPanel3
        '
        Me.GroupPanel3.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel3.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel3.Controls.Add(Me.Panel4)
        Me.GroupPanel3.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupPanel3.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel3.Location = New System.Drawing.Point(0, 0)
        Me.GroupPanel3.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupPanel3.Name = "GroupPanel3"
        Me.GroupPanel3.Size = New System.Drawing.Size(517, 77)
        '
        '
        '
        Me.GroupPanel3.Style.BackColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.GroupPanel3.Style.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.GroupPanel3.Style.BackColorGradientAngle = 90
        Me.GroupPanel3.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderBottomWidth = 1
        Me.GroupPanel3.Style.BorderColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.GroupPanel3.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderLeftWidth = 1
        Me.GroupPanel3.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderRightWidth = 1
        Me.GroupPanel3.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel3.Style.BorderTopWidth = 1
        Me.GroupPanel3.Style.CornerDiameter = 4
        Me.GroupPanel3.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel3.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel3.Style.TextColor = System.Drawing.Color.White
        Me.GroupPanel3.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.GroupPanel3.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.GroupPanel3.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.GroupPanel3.TabIndex = 1
        Me.GroupPanel3.Text = "Filtro Zonas"
        '
        'Panel4
        '
        Me.Panel4.AutoScroll = True
        Me.Panel4.BackColor = System.Drawing.Color.White
        Me.Panel4.Controls.Add(Me.checkTodos)
        Me.Panel4.Controls.Add(Me.cbZona)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(0, 0)
        Me.Panel4.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(511, 48)
        Me.Panel4.TabIndex = 0
        '
        'checkTodos
        '
        '
        '
        '
        Me.checkTodos.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.checkTodos.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.checkTodos.Location = New System.Drawing.Point(49, 10)
        Me.checkTodos.Margin = New System.Windows.Forms.Padding(4)
        Me.checkTodos.Name = "checkTodos"
        Me.checkTodos.Size = New System.Drawing.Size(87, 28)
        Me.checkTodos.TabIndex = 4
        Me.checkTodos.Text = "TODOS"
        Me.checkTodos.TextColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        '
        'cbZona
        '
        Me.cbZona.BorderStyle = Janus.Windows.GridEX.BorderStyle.Flat
        Me.cbZona.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        cbZona_DesignTimeLayout.LayoutString = resources.GetString("cbZona_DesignTimeLayout.LayoutString")
        Me.cbZona.DesignTimeLayout = cbZona_DesignTimeLayout
        Me.cbZona.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbZona.Location = New System.Drawing.Point(168, 10)
        Me.cbZona.Margin = New System.Windows.Forms.Padding(4)
        Me.cbZona.Name = "cbZona"
        Me.cbZona.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbZona.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.cbZona.SelectedIndex = -1
        Me.cbZona.SelectedItem = Nothing
        Me.cbZona.Size = New System.Drawing.Size(293, 26)
        Me.cbZona.TabIndex = 3
        Me.cbZona.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'PanelMapa
        '
        Me.PanelMapa.BackColor = System.Drawing.Color.Transparent
        Me.PanelMapa.Controls.Add(Me.btnz2)
        Me.PanelMapa.Controls.Add(Me.btnz1)
        Me.PanelMapa.Controls.Add(Me.Gmc_Cliente)
        Me.PanelMapa.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelMapa.Location = New System.Drawing.Point(529, 4)
        Me.PanelMapa.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelMapa.Name = "PanelMapa"
        Me.PanelMapa.Size = New System.Drawing.Size(780, 625)
        Me.PanelMapa.TabIndex = 21
        '
        'btnz2
        '
        Me.btnz2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnz2.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.btnz2.Image = Global.DinoM.My.Resources.Resources.iconalejar
        Me.btnz2.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnz2.Location = New System.Drawing.Point(9, 57)
        Me.btnz2.Margin = New System.Windows.Forms.Padding(4)
        Me.btnz2.Name = "btnz2"
        Me.btnz2.Size = New System.Drawing.Size(53, 49)
        Me.btnz2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnz2.TabIndex = 7
        '
        'btnz1
        '
        Me.btnz1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnz1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.btnz1.Image = Global.DinoM.My.Resources.Resources.iconacercar
        Me.btnz1.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnz1.Location = New System.Drawing.Point(9, 6)
        Me.btnz1.Margin = New System.Windows.Forms.Padding(4)
        Me.btnz1.Name = "btnz1"
        Me.btnz1.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnz1.Size = New System.Drawing.Size(53, 49)
        Me.btnz1.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.btnz1.TabIndex = 6
        '
        'Gmc_Cliente
        '
        Me.Gmc_Cliente.AutoScroll = True
        Me.Gmc_Cliente.AutoSize = True
        Me.Gmc_Cliente.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Gmc_Cliente.Bearing = 0!
        Me.Gmc_Cliente.CanDragMap = True
        Me.Gmc_Cliente.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Gmc_Cliente.EmptyTileColor = System.Drawing.Color.Navy
        Me.Gmc_Cliente.GrayScaleMode = False
        Me.Gmc_Cliente.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow
        Me.Gmc_Cliente.LevelsKeepInMemmory = 5
        Me.Gmc_Cliente.Location = New System.Drawing.Point(0, 0)
        Me.Gmc_Cliente.Margin = New System.Windows.Forms.Padding(4)
        Me.Gmc_Cliente.MarkersEnabled = True
        Me.Gmc_Cliente.MaxZoom = 2
        Me.Gmc_Cliente.MinZoom = 2
        Me.Gmc_Cliente.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter
        Me.Gmc_Cliente.Name = "Gmc_Cliente"
        Me.Gmc_Cliente.NegativeMode = False
        Me.Gmc_Cliente.PolygonsEnabled = True
        Me.Gmc_Cliente.RetryLoadTile = 0
        Me.Gmc_Cliente.RoutesEnabled = True
        Me.Gmc_Cliente.ScaleMode = GMap.NET.WindowsForms.ScaleModes.[Integer]
        Me.Gmc_Cliente.SelectedAreaFillColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(105, Byte), Integer), CType(CType(225, Byte), Integer))
        Me.Gmc_Cliente.ShowTileGridLines = False
        Me.Gmc_Cliente.Size = New System.Drawing.Size(780, 625)
        Me.Gmc_Cliente.TabIndex = 0
        Me.Gmc_Cliente.Zoom = 0R
        '
        'Panel10
        '
        Me.Panel10.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Panel10.Controls.Add(Me.Panel11)
        Me.Panel10.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel10.Location = New System.Drawing.Point(3, 3)
        Me.Panel10.Name = "Panel10"
        Me.Panel10.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel10.Size = New System.Drawing.Size(1313, 27)
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
        Me.Panel11.Size = New System.Drawing.Size(1311, 25)
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
        Me.Label3.Text = "Vistas de Clientes"
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
        Me.PictureBox3.Image = Global.DinoM.My.Resources.Resources.tec_triangulo_blanco
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
        Me.PanelButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PanelButton.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelButton.Location = New System.Drawing.Point(0, 666)
        Me.PanelButton.Name = "PanelButton"
        Me.PanelButton.Size = New System.Drawing.Size(1319, 23)
        Me.PanelButton.TabIndex = 3
        '
        'Tec_Mapaclientes
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1319, 689)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Tec_Mapaclientes"
        Me.Text = "Tec_Mapaclientes"
        Me.Panel1.ResumeLayout(False)
        Me.PanelSuperior.ResumeLayout(False)
        Me.PanelLEft.ResumeLayout(False)
        Me.Panel8.ResumeLayout(False)
        Me.PanelDatos.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.PanelZona.ResumeLayout(False)
        Me.Zonas.ResumeLayout(False)
        Me.PanelTable.ResumeLayout(False)
        CType(Me.grCliente, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel3.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        CType(Me.cbZona, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelMapa.ResumeLayout(False)
        Me.PanelMapa.PerformLayout()
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents PanelSuperior As Panel
    Friend WithEvents PanelLEft As Panel
    Friend WithEvents Panel8 As Panel
    Friend WithEvents PanelDatos As Panel
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents PanelZona As Panel
    Friend WithEvents Zonas As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents PanelTable As Panel
    Friend WithEvents grCliente As Janus.Windows.GridEX.GridEX
    Friend WithEvents GroupPanel3 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Panel4 As Panel
    Friend WithEvents PanelMapa As Panel
    Friend WithEvents btnz2 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnz1 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents Gmc_Cliente As GMap.NET.WindowsForms.GMapControl
    Friend WithEvents Panel10 As Panel
    Friend WithEvents Panel11 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel12 As Panel
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents PanelButton As Panel
    Friend WithEvents checkTodos As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents cbZona As Janus.Windows.GridEX.EditControls.MultiColumnCombo
End Class
