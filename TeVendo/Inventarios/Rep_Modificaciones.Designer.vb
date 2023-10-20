<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Rep_Modificaciones
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
        Me.panelDatos = New System.Windows.Forms.Panel()
        Me.PanelTablas = New System.Windows.Forms.Panel()
        Me.grHistorico = New Janus.Windows.GridEX.GridEX()
        Me.PanelMapas = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.ButtonX2 = New DevComponents.DotNetBar.ButtonX()
        Me.Gmc_ClienteActual = New GMap.NET.WindowsForms.GMapControl()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Panel8 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel9 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.PanelImagenes = New System.Windows.Forms.Panel()
        Me.ButtonX4 = New DevComponents.DotNetBar.ButtonX()
        Me.ButtonX3 = New DevComponents.DotNetBar.ButtonX()
        Me.Gmc_ClienteAnterior = New GMap.NET.WindowsForms.GMapControl()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.lbprivilegio = New System.Windows.Forms.Label()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.Panel13 = New System.Windows.Forms.Panel()
        Me.cbFechaHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX4 = New DevComponents.DotNetBar.LabelX()
        Me.cbFechaDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.btnVendedor = New DevComponents.DotNetBar.ButtonX()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.tbVendedor = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.btnConfirmarSalir = New DevComponents.DotNetBar.ButtonX()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.Principal.SuspendLayout()
        Me.panelDatos.SuspendLayout()
        Me.PanelTablas.SuspendLayout()
        CType(Me.grHistorico, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelMapas.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel8.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.PanelImagenes.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel6.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel13.SuspendLayout()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Principal
        '
        Me.Principal.BackColor = System.Drawing.Color.White
        Me.Principal.Controls.Add(Me.panelDatos)
        Me.Principal.Controls.Add(Me.Panel13)
        Me.Principal.Controls.Add(Me.Panel10)
        Me.Principal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Principal.Location = New System.Drawing.Point(0, 0)
        Me.Principal.Name = "Principal"
        Me.Principal.Size = New System.Drawing.Size(1006, 543)
        Me.Principal.TabIndex = 0
        '
        'panelDatos
        '
        Me.panelDatos.Controls.Add(Me.PanelTablas)
        Me.panelDatos.Controls.Add(Me.PanelMapas)
        Me.panelDatos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panelDatos.Location = New System.Drawing.Point(0, 129)
        Me.panelDatos.Name = "panelDatos"
        Me.panelDatos.Size = New System.Drawing.Size(1006, 414)
        Me.panelDatos.TabIndex = 4
        '
        'PanelTablas
        '
        Me.PanelTablas.BackColor = System.Drawing.Color.Salmon
        Me.PanelTablas.Controls.Add(Me.grHistorico)
        Me.PanelTablas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelTablas.Location = New System.Drawing.Point(0, 0)
        Me.PanelTablas.Name = "PanelTablas"
        Me.PanelTablas.Size = New System.Drawing.Size(465, 414)
        Me.PanelTablas.TabIndex = 1
        '
        'grHistorico
        '
        Me.grHistorico.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grHistorico.AlternatingColors = True
        Me.grHistorico.BackColor = System.Drawing.Color.White
        Me.grHistorico.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grHistorico.ColumnAutoResize = True
        Me.grHistorico.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grHistorico.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grHistorico.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grHistorico.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grHistorico.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grHistorico.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grHistorico.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grHistorico.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grHistorico.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grHistorico.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grHistorico.HeaderFormatStyle.Alpha = 0
        Me.grHistorico.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grHistorico.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grHistorico.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grHistorico.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grHistorico.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grHistorico.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grHistorico.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grHistorico.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grHistorico.Location = New System.Drawing.Point(0, 0)
        Me.grHistorico.Margin = New System.Windows.Forms.Padding(4)
        Me.grHistorico.Name = "grHistorico"
        Me.grHistorico.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grHistorico.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grHistorico.RecordNavigator = True
        Me.grHistorico.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grHistorico.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grHistorico.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grHistorico.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grHistorico.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grHistorico.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grHistorico.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grHistorico.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grHistorico.SelectedFormatStyle.BackColor = System.Drawing.Color.Gold
        Me.grHistorico.SelectedFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grHistorico.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grHistorico.SelectedFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grHistorico.Size = New System.Drawing.Size(465, 414)
        Me.grHistorico.TabIndex = 5
        Me.grHistorico.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grHistorico.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grHistorico.TableSpacing = 9
        Me.grHistorico.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grHistorico.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grHistorico.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grHistorico.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'PanelMapas
        '
        Me.PanelMapas.BackColor = System.Drawing.Color.White
        Me.PanelMapas.Controls.Add(Me.TableLayoutPanel1)
        Me.PanelMapas.Dock = System.Windows.Forms.DockStyle.Right
        Me.PanelMapas.Location = New System.Drawing.Point(465, 0)
        Me.PanelMapas.Name = "PanelMapas"
        Me.PanelMapas.Size = New System.Drawing.Size(541, 414)
        Me.PanelMapas.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel2, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(541, 414)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Controls.Add(Me.Panel4)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(3, 210)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Padding = New System.Windows.Forms.Padding(3)
        Me.Panel1.Size = New System.Drawing.Size(535, 201)
        Me.Panel1.TabIndex = 3
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.White
        Me.Panel3.Controls.Add(Me.ButtonX1)
        Me.Panel3.Controls.Add(Me.ButtonX2)
        Me.Panel3.Controls.Add(Me.Gmc_ClienteActual)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(3, 30)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(529, 168)
        Me.Panel3.TabIndex = 2
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.iconacercar
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(25, 25)
        Me.ButtonX1.Location = New System.Drawing.Point(70, 449)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(40, 37)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX1.TabIndex = 21
        '
        'ButtonX2
        '
        Me.ButtonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.ButtonX2.Image = Global.TeVendo.My.Resources.Resources.iconalejar
        Me.ButtonX2.ImageFixedSize = New System.Drawing.Size(25, 25)
        Me.ButtonX2.Location = New System.Drawing.Point(19, 449)
        Me.ButtonX2.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX2.Name = "ButtonX2"
        Me.ButtonX2.Size = New System.Drawing.Size(40, 37)
        Me.ButtonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX2.TabIndex = 20
        '
        'Gmc_ClienteActual
        '
        Me.Gmc_ClienteActual.Bearing = 0!
        Me.Gmc_ClienteActual.CanDragMap = True
        Me.Gmc_ClienteActual.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Gmc_ClienteActual.EmptyTileColor = System.Drawing.Color.Navy
        Me.Gmc_ClienteActual.GrayScaleMode = False
        Me.Gmc_ClienteActual.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow
        Me.Gmc_ClienteActual.LevelsKeepInMemmory = 5
        Me.Gmc_ClienteActual.Location = New System.Drawing.Point(0, 0)
        Me.Gmc_ClienteActual.Margin = New System.Windows.Forms.Padding(4)
        Me.Gmc_ClienteActual.MarkersEnabled = True
        Me.Gmc_ClienteActual.MaxZoom = 2
        Me.Gmc_ClienteActual.MinZoom = 2
        Me.Gmc_ClienteActual.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter
        Me.Gmc_ClienteActual.Name = "Gmc_ClienteActual"
        Me.Gmc_ClienteActual.NegativeMode = False
        Me.Gmc_ClienteActual.PolygonsEnabled = True
        Me.Gmc_ClienteActual.RetryLoadTile = 0
        Me.Gmc_ClienteActual.RoutesEnabled = True
        Me.Gmc_ClienteActual.ScaleMode = GMap.NET.WindowsForms.ScaleModes.[Integer]
        Me.Gmc_ClienteActual.SelectedAreaFillColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(105, Byte), Integer), CType(CType(225, Byte), Integer))
        Me.Gmc_ClienteActual.ShowTileGridLines = False
        Me.Gmc_ClienteActual.Size = New System.Drawing.Size(529, 168)
        Me.Gmc_ClienteActual.TabIndex = 6
        Me.Gmc_ClienteActual.Zoom = 0R
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.Panel4.Controls.Add(Me.Panel8)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(3, 3)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel4.Size = New System.Drawing.Size(529, 27)
        Me.Panel4.TabIndex = 1
        '
        'Panel8
        '
        Me.Panel8.BackColor = System.Drawing.Color.DarkSlateGray
        Me.Panel8.Controls.Add(Me.Label1)
        Me.Panel8.Controls.Add(Me.Panel9)
        Me.Panel8.Controls.Add(Me.PictureBox1)
        Me.Panel8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel8.Location = New System.Drawing.Point(1, 1)
        Me.Panel8.Name = "Panel8"
        Me.Panel8.Size = New System.Drawing.Size(527, 25)
        Me.Panel8.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label1.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(59, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.Label1.Size = New System.Drawing.Size(468, 25)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Ubicacion Actualizada"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel9
        '
        Me.Panel9.BackColor = System.Drawing.Color.White
        Me.Panel9.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel9.Location = New System.Drawing.Point(58, 0)
        Me.Panel9.Name = "Panel9"
        Me.Panel9.Size = New System.Drawing.Size(1, 25)
        Me.Panel9.TabIndex = 1
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox1.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox1.Size = New System.Drawing.Size(58, 25)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel2.Controls.Add(Me.PanelImagenes)
        Me.Panel2.Controls.Add(Me.Panel5)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(3, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Padding = New System.Windows.Forms.Padding(3)
        Me.Panel2.Size = New System.Drawing.Size(535, 201)
        Me.Panel2.TabIndex = 2
        '
        'PanelImagenes
        '
        Me.PanelImagenes.BackColor = System.Drawing.Color.White
        Me.PanelImagenes.Controls.Add(Me.ButtonX4)
        Me.PanelImagenes.Controls.Add(Me.ButtonX3)
        Me.PanelImagenes.Controls.Add(Me.Gmc_ClienteAnterior)
        Me.PanelImagenes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelImagenes.Location = New System.Drawing.Point(3, 30)
        Me.PanelImagenes.Name = "PanelImagenes"
        Me.PanelImagenes.Size = New System.Drawing.Size(529, 168)
        Me.PanelImagenes.TabIndex = 2
        '
        'ButtonX4
        '
        Me.ButtonX4.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX4.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.ButtonX4.Image = Global.TeVendo.My.Resources.Resources.iconacercar
        Me.ButtonX4.ImageFixedSize = New System.Drawing.Size(25, 25)
        Me.ButtonX4.Location = New System.Drawing.Point(70, 449)
        Me.ButtonX4.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX4.Name = "ButtonX4"
        Me.ButtonX4.Size = New System.Drawing.Size(40, 37)
        Me.ButtonX4.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX4.TabIndex = 21
        '
        'ButtonX3
        '
        Me.ButtonX3.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX3.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground
        Me.ButtonX3.Image = Global.TeVendo.My.Resources.Resources.iconalejar
        Me.ButtonX3.ImageFixedSize = New System.Drawing.Size(25, 25)
        Me.ButtonX3.Location = New System.Drawing.Point(19, 449)
        Me.ButtonX3.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX3.Name = "ButtonX3"
        Me.ButtonX3.Size = New System.Drawing.Size(40, 37)
        Me.ButtonX3.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX3.TabIndex = 20
        '
        'Gmc_ClienteAnterior
        '
        Me.Gmc_ClienteAnterior.Bearing = 0!
        Me.Gmc_ClienteAnterior.CanDragMap = True
        Me.Gmc_ClienteAnterior.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Gmc_ClienteAnterior.EmptyTileColor = System.Drawing.Color.Navy
        Me.Gmc_ClienteAnterior.GrayScaleMode = False
        Me.Gmc_ClienteAnterior.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow
        Me.Gmc_ClienteAnterior.LevelsKeepInMemmory = 5
        Me.Gmc_ClienteAnterior.Location = New System.Drawing.Point(0, 0)
        Me.Gmc_ClienteAnterior.Margin = New System.Windows.Forms.Padding(4)
        Me.Gmc_ClienteAnterior.MarkersEnabled = True
        Me.Gmc_ClienteAnterior.MaxZoom = 2
        Me.Gmc_ClienteAnterior.MinZoom = 2
        Me.Gmc_ClienteAnterior.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter
        Me.Gmc_ClienteAnterior.Name = "Gmc_ClienteAnterior"
        Me.Gmc_ClienteAnterior.NegativeMode = False
        Me.Gmc_ClienteAnterior.PolygonsEnabled = True
        Me.Gmc_ClienteAnterior.RetryLoadTile = 0
        Me.Gmc_ClienteAnterior.RoutesEnabled = True
        Me.Gmc_ClienteAnterior.ScaleMode = GMap.NET.WindowsForms.ScaleModes.[Integer]
        Me.Gmc_ClienteAnterior.SelectedAreaFillColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(65, Byte), Integer), CType(CType(105, Byte), Integer), CType(CType(225, Byte), Integer))
        Me.Gmc_ClienteAnterior.ShowTileGridLines = False
        Me.Gmc_ClienteAnterior.Size = New System.Drawing.Size(529, 168)
        Me.Gmc_ClienteAnterior.TabIndex = 6
        Me.Gmc_ClienteAnterior.Zoom = 0R
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.Panel5.Controls.Add(Me.Panel6)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel5.Location = New System.Drawing.Point(3, 3)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel5.Size = New System.Drawing.Size(529, 27)
        Me.Panel5.TabIndex = 1
        '
        'Panel6
        '
        Me.Panel6.BackColor = System.Drawing.Color.DarkSlateGray
        Me.Panel6.Controls.Add(Me.lbprivilegio)
        Me.Panel6.Controls.Add(Me.Panel7)
        Me.Panel6.Controls.Add(Me.PictureBox2)
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel6.ForeColor = System.Drawing.Color.White
        Me.Panel6.Location = New System.Drawing.Point(1, 1)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(527, 25)
        Me.Panel6.TabIndex = 0
        '
        'lbprivilegio
        '
        Me.lbprivilegio.Dock = System.Windows.Forms.DockStyle.Top
        Me.lbprivilegio.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbprivilegio.ForeColor = System.Drawing.Color.White
        Me.lbprivilegio.Location = New System.Drawing.Point(59, 0)
        Me.lbprivilegio.Name = "lbprivilegio"
        Me.lbprivilegio.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.lbprivilegio.Size = New System.Drawing.Size(468, 25)
        Me.lbprivilegio.TabIndex = 2
        Me.lbprivilegio.Text = "Ubicacion Anterior"
        Me.lbprivilegio.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel7
        '
        Me.Panel7.BackColor = System.Drawing.Color.White
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel7.Location = New System.Drawing.Point(58, 0)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(1, 25)
        Me.Panel7.TabIndex = 1
        '
        'PictureBox2
        '
        Me.PictureBox2.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox2.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox2.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox2.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox2.Size = New System.Drawing.Size(58, 25)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 0
        Me.PictureBox2.TabStop = False
        '
        'Panel13
        '
        Me.Panel13.Controls.Add(Me.cbFechaHasta)
        Me.Panel13.Controls.Add(Me.LabelX4)
        Me.Panel13.Controls.Add(Me.cbFechaDesde)
        Me.Panel13.Controls.Add(Me.LabelX2)
        Me.Panel13.Controls.Add(Me.btnVendedor)
        Me.Panel13.Controls.Add(Me.LabelX1)
        Me.Panel13.Controls.Add(Me.tbVendedor)
        Me.Panel13.Controls.Add(Me.btnConfirmarSalir)
        Me.Panel13.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel13.Location = New System.Drawing.Point(0, 36)
        Me.Panel13.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel13.Name = "Panel13"
        Me.Panel13.Size = New System.Drawing.Size(1006, 93)
        Me.Panel13.TabIndex = 5
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
        Me.cbFechaHasta.Location = New System.Drawing.Point(429, 48)
        Me.cbFechaHasta.Name = "cbFechaHasta"
        Me.cbFechaHasta.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaHasta.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaHasta.SecondIncrement = 10
        Me.cbFechaHasta.Size = New System.Drawing.Size(200, 26)
        Me.cbFechaHasta.TabIndex = 382
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
        Me.LabelX4.ForeColor = System.Drawing.Color.Black
        Me.LabelX4.Location = New System.Drawing.Point(380, 48)
        Me.LabelX4.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX4.Name = "LabelX4"
        Me.LabelX4.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX4.Size = New System.Drawing.Size(42, 21)
        Me.LabelX4.TabIndex = 381
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
        Me.cbFechaDesde.Location = New System.Drawing.Point(429, 19)
        Me.cbFechaDesde.Name = "cbFechaDesde"
        Me.cbFechaDesde.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaDesde.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaDesde.SecondIncrement = 10
        Me.cbFechaDesde.Size = New System.Drawing.Size(200, 26)
        Me.cbFechaDesde.TabIndex = 380
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
        Me.LabelX2.ForeColor = System.Drawing.Color.Black
        Me.LabelX2.Location = New System.Drawing.Point(377, 19)
        Me.LabelX2.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX2.Size = New System.Drawing.Size(45, 21)
        Me.LabelX2.TabIndex = 379
        Me.LabelX2.Text = "Desde:"
        '
        'btnVendedor
        '
        Me.btnVendedor.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnVendedor.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnVendedor.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnVendedor.ImageFixedSize = New System.Drawing.Size(20, 20)
        Me.btnVendedor.Location = New System.Drawing.Point(309, 40)
        Me.btnVendedor.Margin = New System.Windows.Forms.Padding(4)
        Me.btnVendedor.Name = "btnVendedor"
        Me.btnVendedor.Size = New System.Drawing.Size(35, 28)
        Me.btnVendedor.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnVendedor.TabIndex = 378
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
        Me.LabelX1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX1.Location = New System.Drawing.Point(24, 19)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(68, 21)
        Me.LabelX1.TabIndex = 377
        Me.LabelX1.Text = "Vendedor:"
        '
        'tbVendedor
        '
        '
        '
        '
        Me.tbVendedor.Border.Class = "TextBoxBorder"
        Me.tbVendedor.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbVendedor.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbVendedor.Location = New System.Drawing.Point(24, 42)
        Me.tbVendedor.Margin = New System.Windows.Forms.Padding(4)
        Me.tbVendedor.Name = "tbVendedor"
        Me.tbVendedor.PreventEnterBeep = True
        Me.tbVendedor.Size = New System.Drawing.Size(278, 26)
        Me.tbVendedor.TabIndex = 376
        '
        'btnConfirmarSalir
        '
        Me.btnConfirmarSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnConfirmarSalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnConfirmarSalir.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnConfirmarSalir.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnConfirmarSalir.ImageFixedSize = New System.Drawing.Size(24, 24)
        Me.btnConfirmarSalir.Location = New System.Drawing.Point(671, 15)
        Me.btnConfirmarSalir.Margin = New System.Windows.Forms.Padding(4)
        Me.btnConfirmarSalir.Name = "btnConfirmarSalir"
        Me.btnConfirmarSalir.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnConfirmarSalir.Size = New System.Drawing.Size(166, 53)
        Me.btnConfirmarSalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnConfirmarSalir.Symbol = "57408"
        Me.btnConfirmarSalir.SymbolColor = System.Drawing.Color.MediumTurquoise
        Me.btnConfirmarSalir.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnConfirmarSalir.SymbolSize = 20.0!
        Me.btnConfirmarSalir.TabIndex = 3
        Me.btnConfirmarSalir.Text = "CARGAR DATOS"
        '
        'Panel10
        '
        Me.Panel10.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Panel10.Controls.Add(Me.Panel11)
        Me.Panel10.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel10.Location = New System.Drawing.Point(0, 0)
        Me.Panel10.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel10.Name = "Panel10"
        Me.Panel10.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel10.Size = New System.Drawing.Size(1006, 36)
        Me.Panel10.TabIndex = 3
        '
        'Panel11
        '
        Me.Panel11.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel11.Controls.Add(Me.Label3)
        Me.Panel11.Controls.Add(Me.Panel12)
        Me.Panel11.Controls.Add(Me.PictureBox3)
        Me.Panel11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel11.Location = New System.Drawing.Point(1, 1)
        Me.Panel11.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel11.Name = "Panel11"
        Me.Panel11.Size = New System.Drawing.Size(1004, 34)
        Me.Panel11.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Label3.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.Label3.Location = New System.Drawing.Point(60, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.Label3.Size = New System.Drawing.Size(416, 34)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "HISTORICO MODIFICACIONES"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel12
        '
        Me.Panel12.BackColor = System.Drawing.Color.White
        Me.Panel12.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel12.Location = New System.Drawing.Point(59, 0)
        Me.Panel12.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel12.Name = "Panel12"
        Me.Panel12.Size = New System.Drawing.Size(1, 34)
        Me.Panel12.TabIndex = 1
        '
        'PictureBox3
        '
        Me.PictureBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox3.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox3.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox3.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox3.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox3.Size = New System.Drawing.Size(59, 34)
        Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox3.TabIndex = 0
        Me.PictureBox3.TabStop = False
        '
        'Rep_Modificaciones
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1006, 543)
        Me.Controls.Add(Me.Principal)
        Me.Name = "Rep_Modificaciones"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "HISTORICO"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Principal.ResumeLayout(False)
        Me.panelDatos.ResumeLayout(False)
        Me.PanelTablas.ResumeLayout(False)
        CType(Me.grHistorico, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelMapas.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel8.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.PanelImagenes.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.Panel6.ResumeLayout(False)
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel13.ResumeLayout(False)
        Me.Panel13.PerformLayout()
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Principal As Panel
    Friend WithEvents Panel10 As Panel
    Friend WithEvents Panel11 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel12 As Panel
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents panelDatos As Panel
    Friend WithEvents PanelTablas As Panel
    Friend WithEvents PanelMapas As Panel
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents ButtonX2 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents Gmc_ClienteActual As GMap.NET.WindowsForms.GMapControl
    Friend WithEvents Panel4 As Panel
    Friend WithEvents Panel8 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel9 As Panel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents PanelImagenes As Panel
    Friend WithEvents ButtonX4 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents ButtonX3 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents Gmc_ClienteAnterior As GMap.NET.WindowsForms.GMapControl
    Friend WithEvents Panel5 As Panel
    Friend WithEvents Panel6 As Panel
    Friend WithEvents lbprivilegio As Label
    Friend WithEvents Panel7 As Panel
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents grHistorico As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel13 As Panel
    Friend WithEvents btnConfirmarSalir As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnVendedor As DevComponents.DotNetBar.ButtonX
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbVendedor As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents cbFechaHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX4 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbFechaDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
End Class
