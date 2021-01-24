<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_VisualizarConceptoFijosPlanilla
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
        Me.Panel14 = New System.Windows.Forms.Panel()
        Me.Panel15 = New System.Windows.Forms.Panel()
        Me.lbtitle = New System.Windows.Forms.Label()
        Me.Panel16 = New System.Windows.Forms.Panel()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.grConceptoFijo = New Janus.Windows.GridEX.GridEX()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ButtonX2 = New DevComponents.DotNetBar.ButtonX()
        Me.Panel14.SuspendLayout()
        Me.Panel15.SuspendLayout()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel1.SuspendLayout()
        CType(Me.grConceptoFijo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel14
        '
        Me.Panel14.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel14.Controls.Add(Me.Panel15)
        Me.Panel14.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel14.Location = New System.Drawing.Point(0, 0)
        Me.Panel14.Name = "Panel14"
        Me.Panel14.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel14.Size = New System.Drawing.Size(930, 40)
        Me.Panel14.TabIndex = 4
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
        Me.Panel15.Size = New System.Drawing.Size(928, 38)
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
        Me.lbtitle.Text = "Ver Conceptos Fijos De : Marco Antonio Mamani Chura"
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
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.GroupPanel1.Controls.Add(Me.grConceptoFijo)
        Me.GroupPanel1.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupPanel1.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel1.Location = New System.Drawing.Point(0, 94)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(930, 384)
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
        Me.GroupPanel1.TabIndex = 5
        Me.GroupPanel1.Text = "Listado De Conceptos Fijos Asignados"
        '
        'grConceptoFijo
        '
        Me.grConceptoFijo.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grConceptoFijo.AlternatingColors = True
        Me.grConceptoFijo.BackColor = System.Drawing.Color.White
        Me.grConceptoFijo.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grConceptoFijo.ColumnAutoResize = True
        Me.grConceptoFijo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grConceptoFijo.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grConceptoFijo.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grConceptoFijo.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grConceptoFijo.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grConceptoFijo.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grConceptoFijo.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grConceptoFijo.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grConceptoFijo.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grConceptoFijo.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grConceptoFijo.HeaderFormatStyle.Alpha = 0
        Me.grConceptoFijo.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grConceptoFijo.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grConceptoFijo.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grConceptoFijo.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grConceptoFijo.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grConceptoFijo.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grConceptoFijo.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grConceptoFijo.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grConceptoFijo.Location = New System.Drawing.Point(0, 0)
        Me.grConceptoFijo.Margin = New System.Windows.Forms.Padding(4)
        Me.grConceptoFijo.Name = "grConceptoFijo"
        Me.grConceptoFijo.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grConceptoFijo.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grConceptoFijo.RecordNavigator = True
        Me.grConceptoFijo.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grConceptoFijo.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grConceptoFijo.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grConceptoFijo.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grConceptoFijo.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grConceptoFijo.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grConceptoFijo.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grConceptoFijo.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grConceptoFijo.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grConceptoFijo.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grConceptoFijo.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grConceptoFijo.Size = New System.Drawing.Size(924, 354)
        Me.grConceptoFijo.TabIndex = 4
        Me.grConceptoFijo.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grConceptoFijo.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grConceptoFijo.TableSpacing = 9
        Me.grConceptoFijo.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grConceptoFijo.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grConceptoFijo.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grConceptoFijo.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.White
        Me.Panel1.Controls.Add(Me.ButtonX2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 40)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(930, 54)
        Me.Panel1.TabIndex = 5
        '
        'ButtonX2
        '
        Me.ButtonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX2.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(83, Byte), Integer), CType(CType(79, Byte), Integer))
        Me.ButtonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.ButtonX2.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX2.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.ButtonX2.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.ButtonX2.Location = New System.Drawing.Point(754, 6)
        Me.ButtonX2.Name = "ButtonX2"
        Me.ButtonX2.Size = New System.Drawing.Size(164, 43)
        Me.ButtonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.ButtonX2.Symbol = ""
        Me.ButtonX2.TabIndex = 66
        Me.ButtonX2.Text = "Volver"
        Me.ButtonX2.TextColor = System.Drawing.Color.White
        '
        'Tec_VisualizarConceptoFijosPlanilla
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(930, 478)
        Me.Controls.Add(Me.GroupPanel1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel14)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "Tec_VisualizarConceptoFijosPlanilla"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Visualizar Conceptos Fijos"
        Me.Panel14.ResumeLayout(False)
        Me.Panel15.ResumeLayout(False)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel1.ResumeLayout(False)
        CType(Me.grConceptoFijo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel14 As Panel
    Friend WithEvents Panel15 As Panel
    Friend WithEvents lbtitle As Label
    Friend WithEvents Panel16 As Panel
    Friend WithEvents PictureBox4 As PictureBox
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents grConceptoFijo As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel1 As Panel
    Friend WithEvents ButtonX2 As DevComponents.DotNetBar.ButtonX
End Class
