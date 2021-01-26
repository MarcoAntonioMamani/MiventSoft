<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_Listarplanilla
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
        Dim cbMes_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Tec_Listarplanilla))
        Dim cbAnio_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Me.PanelCabecera = New System.Windows.Forms.Panel()
        Me.chktodos = New System.Windows.Forms.CheckBox()
        Me.ButtonX2 = New DevComponents.DotNetBar.ButtonX()
        Me.cbMes = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.cbAnio = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX7 = New DevComponents.DotNetBar.LabelX()
        Me.Panel14 = New System.Windows.Forms.Panel()
        Me.Panel15 = New System.Windows.Forms.Panel()
        Me.lbTitulo = New System.Windows.Forms.Label()
        Me.Panel16 = New System.Windows.Forms.Panel()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.PanelDetalle = New System.Windows.Forms.Panel()
        Me.grplanilla = New Janus.Windows.GridEX.GridEX()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.btnReportePlanilla = New DevComponents.DotNetBar.ButtonX()
        Me.PanelCabecera.SuspendLayout()
        CType(Me.cbMes, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cbAnio, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel14.SuspendLayout()
        Me.Panel15.SuspendLayout()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelDetalle.SuspendLayout()
        CType(Me.grplanilla, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelCabecera
        '
        Me.PanelCabecera.BackColor = System.Drawing.Color.White
        Me.PanelCabecera.Controls.Add(Me.btnReportePlanilla)
        Me.PanelCabecera.Controls.Add(Me.chktodos)
        Me.PanelCabecera.Controls.Add(Me.ButtonX2)
        Me.PanelCabecera.Controls.Add(Me.cbMes)
        Me.PanelCabecera.Controls.Add(Me.LabelX1)
        Me.PanelCabecera.Controls.Add(Me.cbAnio)
        Me.PanelCabecera.Controls.Add(Me.LabelX7)
        Me.PanelCabecera.Controls.Add(Me.Panel14)
        Me.PanelCabecera.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelCabecera.Location = New System.Drawing.Point(0, 0)
        Me.PanelCabecera.Name = "PanelCabecera"
        Me.PanelCabecera.Size = New System.Drawing.Size(1388, 120)
        Me.PanelCabecera.TabIndex = 0
        '
        'chktodos
        '
        Me.chktodos.AutoSize = True
        Me.chktodos.BackColor = System.Drawing.Color.White
        Me.chktodos.FlatAppearance.BorderColor = System.Drawing.Color.DarkCyan
        Me.chktodos.FlatAppearance.BorderSize = 3
        Me.chktodos.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chktodos.Location = New System.Drawing.Point(586, 82)
        Me.chktodos.Name = "chktodos"
        Me.chktodos.Size = New System.Drawing.Size(101, 25)
        Me.chktodos.TabIndex = 70
        Me.chktodos.Text = "Ver Todos"
        Me.chktodos.UseVisualStyleBackColor = True
        '
        'ButtonX2
        '
        Me.ButtonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX2.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(83, Byte), Integer), CType(CType(79, Byte), Integer))
        Me.ButtonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.ButtonX2.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX2.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.ButtonX2.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.ButtonX2.Location = New System.Drawing.Point(693, 67)
        Me.ButtonX2.Name = "ButtonX2"
        Me.ButtonX2.Size = New System.Drawing.Size(164, 43)
        Me.ButtonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.ButtonX2.Symbol = ""
        Me.ButtonX2.TabIndex = 69
        Me.ButtonX2.Text = "Salir"
        Me.ButtonX2.TextColor = System.Drawing.Color.White
        '
        'cbMes
        '
        Me.cbMes.BackColor = System.Drawing.Color.Azure
        Me.cbMes.ColorScheme = ""
        Me.cbMes.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbMes.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbMes.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        cbMes_DesignTimeLayout.LayoutString = resources.GetString("cbMes_DesignTimeLayout.LayoutString")
        Me.cbMes.DesignTimeLayout = cbMes_DesignTimeLayout
        Me.cbMes.FlatBorderColor = System.Drawing.Color.Black
        Me.cbMes.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbMes.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbMes.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbMes.Location = New System.Drawing.Point(295, 82)
        Me.cbMes.Margin = New System.Windows.Forms.Padding(4)
        Me.cbMes.Name = "cbMes"
        Me.cbMes.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbMes.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbMes.SelectedIndex = -1
        Me.cbMes.SelectedItem = Nothing
        Me.cbMes.Size = New System.Drawing.Size(262, 28)
        Me.cbMes.TabIndex = 67
        Me.cbMes.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'LabelX1
        '
        Me.LabelX1.AutoSize = True
        Me.LabelX1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX1.Location = New System.Drawing.Point(295, 52)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(37, 23)
        Me.LabelX1.TabIndex = 68
        Me.LabelX1.Text = "Mes:"
        '
        'cbAnio
        '
        Me.cbAnio.BackColor = System.Drawing.Color.Azure
        Me.cbAnio.ColorScheme = ""
        Me.cbAnio.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbAnio.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbAnio.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        cbAnio_DesignTimeLayout.LayoutString = resources.GetString("cbAnio_DesignTimeLayout.LayoutString")
        Me.cbAnio.DesignTimeLayout = cbAnio_DesignTimeLayout
        Me.cbAnio.FlatBorderColor = System.Drawing.Color.Black
        Me.cbAnio.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbAnio.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbAnio.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbAnio.Location = New System.Drawing.Point(25, 82)
        Me.cbAnio.Margin = New System.Windows.Forms.Padding(4)
        Me.cbAnio.Name = "cbAnio"
        Me.cbAnio.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbAnio.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbAnio.SelectedIndex = -1
        Me.cbAnio.SelectedItem = Nothing
        Me.cbAnio.Size = New System.Drawing.Size(262, 28)
        Me.cbAnio.TabIndex = 65
        Me.cbAnio.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'LabelX7
        '
        Me.LabelX7.AutoSize = True
        Me.LabelX7.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX7.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX7.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX7.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX7.Location = New System.Drawing.Point(25, 52)
        Me.LabelX7.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX7.Name = "LabelX7"
        Me.LabelX7.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX7.Size = New System.Drawing.Size(36, 23)
        Me.LabelX7.TabIndex = 66
        Me.LabelX7.Text = "Año:"
        '
        'Panel14
        '
        Me.Panel14.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel14.Controls.Add(Me.Panel15)
        Me.Panel14.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel14.Location = New System.Drawing.Point(0, 0)
        Me.Panel14.Name = "Panel14"
        Me.Panel14.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel14.Size = New System.Drawing.Size(1388, 36)
        Me.Panel14.TabIndex = 5
        '
        'Panel15
        '
        Me.Panel15.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel15.Controls.Add(Me.lbTitulo)
        Me.Panel15.Controls.Add(Me.Panel16)
        Me.Panel15.Controls.Add(Me.PictureBox4)
        Me.Panel15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel15.Location = New System.Drawing.Point(1, 1)
        Me.Panel15.Name = "Panel15"
        Me.Panel15.Size = New System.Drawing.Size(1386, 34)
        Me.Panel15.TabIndex = 4
        '
        'lbTitulo
        '
        Me.lbTitulo.BackColor = System.Drawing.Color.Transparent
        Me.lbTitulo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbTitulo.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbTitulo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbTitulo.Location = New System.Drawing.Point(59, 0)
        Me.lbTitulo.Name = "lbTitulo"
        Me.lbTitulo.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.lbTitulo.Size = New System.Drawing.Size(1327, 34)
        Me.lbTitulo.TabIndex = 10
        Me.lbTitulo.Text = "BUSCAR PLANILLA"
        Me.lbTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel16
        '
        Me.Panel16.BackColor = System.Drawing.Color.White
        Me.Panel16.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel16.Location = New System.Drawing.Point(58, 0)
        Me.Panel16.Name = "Panel16"
        Me.Panel16.Size = New System.Drawing.Size(1, 34)
        Me.Panel16.TabIndex = 11
        '
        'PictureBox4
        '
        Me.PictureBox4.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox4.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox4.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox4.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox4.Name = "PictureBox4"
        Me.PictureBox4.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox4.Size = New System.Drawing.Size(58, 34)
        Me.PictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox4.TabIndex = 0
        Me.PictureBox4.TabStop = False
        '
        'PanelDetalle
        '
        Me.PanelDetalle.BackColor = System.Drawing.Color.White
        Me.PanelDetalle.Controls.Add(Me.grplanilla)
        Me.PanelDetalle.Controls.Add(Me.Panel1)
        Me.PanelDetalle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetalle.Location = New System.Drawing.Point(0, 120)
        Me.PanelDetalle.Name = "PanelDetalle"
        Me.PanelDetalle.Size = New System.Drawing.Size(1388, 321)
        Me.PanelDetalle.TabIndex = 1
        '
        'grplanilla
        '
        Me.grplanilla.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grplanilla.AlternatingColors = True
        Me.grplanilla.BackColor = System.Drawing.Color.White
        Me.grplanilla.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grplanilla.ColumnAutoResize = True
        Me.grplanilla.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grplanilla.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grplanilla.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grplanilla.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grplanilla.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grplanilla.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grplanilla.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grplanilla.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grplanilla.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grplanilla.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grplanilla.HeaderFormatStyle.Alpha = 0
        Me.grplanilla.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grplanilla.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grplanilla.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grplanilla.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grplanilla.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grplanilla.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grplanilla.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grplanilla.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grplanilla.Location = New System.Drawing.Point(0, 36)
        Me.grplanilla.Margin = New System.Windows.Forms.Padding(4)
        Me.grplanilla.Name = "grplanilla"
        Me.grplanilla.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grplanilla.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grplanilla.RecordNavigator = True
        Me.grplanilla.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grplanilla.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grplanilla.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grplanilla.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grplanilla.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grplanilla.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grplanilla.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grplanilla.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grplanilla.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grplanilla.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grplanilla.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grplanilla.Size = New System.Drawing.Size(1388, 285)
        Me.grplanilla.TabIndex = 6
        Me.grplanilla.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grplanilla.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grplanilla.TableSpacing = 9
        Me.grplanilla.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grplanilla.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grplanilla.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grplanilla.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel1.Size = New System.Drawing.Size(1388, 36)
        Me.Panel1.TabIndex = 5
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Controls.Add(Me.Panel3)
        Me.Panel2.Controls.Add(Me.PictureBox1)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(1, 1)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1386, 34)
        Me.Panel2.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(59, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.Label1.Size = New System.Drawing.Size(1327, 34)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "LISTADO CONTRATOS PLANILLA"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.White
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel3.Location = New System.Drawing.Point(58, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1, 34)
        Me.Panel3.TabIndex = 11
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox1.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox1.Size = New System.Drawing.Size(58, 34)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'btnReportePlanilla
        '
        Me.btnReportePlanilla.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnReportePlanilla.BackColor = System.Drawing.Color.LightSeaGreen
        Me.btnReportePlanilla.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnReportePlanilla.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnReportePlanilla.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.btnReportePlanilla.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.btnReportePlanilla.Location = New System.Drawing.Point(874, 67)
        Me.btnReportePlanilla.Name = "btnReportePlanilla"
        Me.btnReportePlanilla.Size = New System.Drawing.Size(206, 43)
        Me.btnReportePlanilla.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.btnReportePlanilla.Symbol = "59565"
        Me.btnReportePlanilla.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnReportePlanilla.TabIndex = 71
        Me.btnReportePlanilla.Text = "Reporte Planilla"
        Me.btnReportePlanilla.TextColor = System.Drawing.Color.White
        '
        'Tec_Listarplanilla
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1388, 441)
        Me.Controls.Add(Me.PanelDetalle)
        Me.Controls.Add(Me.PanelCabecera)
        Me.Name = "Tec_Listarplanilla"
        Me.Text = "Planilla De Contratos"
        Me.PanelCabecera.ResumeLayout(False)
        Me.PanelCabecera.PerformLayout()
        CType(Me.cbMes, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cbAnio, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel14.ResumeLayout(False)
        Me.Panel15.ResumeLayout(False)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelDetalle.ResumeLayout(False)
        CType(Me.grplanilla, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelCabecera As Panel
    Friend WithEvents Panel14 As Panel
    Friend WithEvents Panel15 As Panel
    Friend WithEvents lbTitulo As Label
    Friend WithEvents Panel16 As Panel
    Friend WithEvents PictureBox4 As PictureBox
    Friend WithEvents PanelDetalle As Panel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents cbMes As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbAnio As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX7 As DevComponents.DotNetBar.LabelX
    Friend WithEvents ButtonX2 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents chktodos As CheckBox
    Friend WithEvents grplanilla As Janus.Windows.GridEX.GridEX
    Friend WithEvents btnReportePlanilla As DevComponents.DotNetBar.ButtonX
End Class
