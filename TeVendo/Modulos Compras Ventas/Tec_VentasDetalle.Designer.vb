<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_VentasDetalle
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
        Me.Paneltop = New System.Windows.Forms.Panel()
        Me.grProducto = New Janus.Windows.GridEX.GridEX()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.LabelX9 = New DevComponents.DotNetBar.LabelX()
        Me.tbProducto = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.grDetalle = New Janus.Windows.GridEX.GridEX()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.MHighlighterFocus = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.MEP = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.btnProductos = New DevComponents.DotNetBar.ButtonX()
        Me.btnConfirmarSalir = New DevComponents.DotNetBar.ButtonX()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.Paneltop.SuspendLayout()
        CType(Me.grProducto, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel5.SuspendLayout()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.grDetalle, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Paneltop
        '
        Me.Paneltop.BackColor = System.Drawing.Color.White
        Me.Paneltop.Controls.Add(Me.grProducto)
        Me.Paneltop.Controls.Add(Me.Panel5)
        Me.Paneltop.Controls.Add(Me.Panel10)
        Me.Paneltop.Dock = System.Windows.Forms.DockStyle.Top
        Me.Paneltop.Location = New System.Drawing.Point(0, 0)
        Me.Paneltop.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Paneltop.Name = "Paneltop"
        Me.Paneltop.Size = New System.Drawing.Size(1095, 583)
        Me.Paneltop.TabIndex = 3
        '
        'grProducto
        '
        Me.grProducto.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grProducto.AlternatingColors = True
        Me.grProducto.BackColor = System.Drawing.Color.White
        Me.grProducto.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grProducto.ColumnAutoResize = True
        Me.grProducto.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grProducto.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grProducto.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grProducto.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProducto.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grProducto.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grProducto.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grProducto.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProducto.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grProducto.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grProducto.HeaderFormatStyle.Alpha = 0
        Me.grProducto.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grProducto.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grProducto.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grProducto.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grProducto.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grProducto.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProducto.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grProducto.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grProducto.Location = New System.Drawing.Point(0, 104)
        Me.grProducto.Margin = New System.Windows.Forms.Padding(4)
        Me.grProducto.Name = "grProducto"
        Me.grProducto.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grProducto.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grProducto.RecordNavigator = True
        Me.grProducto.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProducto.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grProducto.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grProducto.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grProducto.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grProducto.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grProducto.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProducto.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grProducto.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grProducto.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProducto.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grProducto.Size = New System.Drawing.Size(1095, 479)
        Me.grProducto.TabIndex = 4
        Me.grProducto.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grProducto.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grProducto.TableSpacing = 9
        Me.grProducto.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grProducto.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grProducto.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grProducto.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.btnProductos)
        Me.Panel5.Controls.Add(Me.btnConfirmarSalir)
        Me.Panel5.Controls.Add(Me.LabelX9)
        Me.Panel5.Controls.Add(Me.tbProducto)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel5.Location = New System.Drawing.Point(0, 36)
        Me.Panel5.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(1095, 68)
        Me.Panel5.TabIndex = 3
        '
        'LabelX9
        '
        Me.LabelX9.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX9.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX9.Font = New System.Drawing.Font("Georgia", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX9.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX9.Location = New System.Drawing.Point(84, 17)
        Me.LabelX9.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX9.Name = "LabelX9"
        Me.LabelX9.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX9.Size = New System.Drawing.Size(151, 28)
        Me.LabelX9.TabIndex = 374
        Me.LabelX9.Text = "Buscar Producto:"
        '
        'tbProducto
        '
        Me.tbProducto.BackColor = System.Drawing.Color.White
        '
        '
        '
        Me.tbProducto.Border.Class = "TextBoxBorder"
        Me.tbProducto.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbProducto.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbProducto.ForeColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.tbProducto.Location = New System.Drawing.Point(251, 18)
        Me.tbProducto.Margin = New System.Windows.Forms.Padding(4)
        Me.tbProducto.Name = "tbProducto"
        Me.tbProducto.PreventEnterBeep = True
        Me.tbProducto.Size = New System.Drawing.Size(332, 24)
        Me.tbProducto.TabIndex = 1
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
        Me.Panel10.Size = New System.Drawing.Size(1095, 36)
        Me.Panel10.TabIndex = 2
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
        Me.Panel11.Size = New System.Drawing.Size(1093, 34)
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
        Me.Label3.Size = New System.Drawing.Size(253, 34)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Seleccionar Producto"
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
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.White
        Me.Panel2.Controls.Add(Me.grDetalle)
        Me.Panel2.Controls.Add(Me.Panel1)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 583)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1095, 205)
        Me.Panel2.TabIndex = 4
        '
        'grDetalle
        '
        Me.grDetalle.AlternatingColors = True
        Me.grDetalle.BackColor = System.Drawing.Color.White
        Me.grDetalle.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grDetalle.ColumnAutoResize = True
        Me.grDetalle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grDetalle.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grDetalle.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grDetalle.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetalle.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetalle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grDetalle.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grDetalle.HeaderFormatStyle.Alpha = 0
        Me.grDetalle.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grDetalle.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetalle.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grDetalle.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetalle.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetalle.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grDetalle.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grDetalle.Location = New System.Drawing.Point(0, 36)
        Me.grDetalle.Margin = New System.Windows.Forms.Padding(4)
        Me.grDetalle.Name = "grDetalle"
        Me.grDetalle.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grDetalle.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grDetalle.RecordNavigator = True
        Me.grDetalle.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetalle.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grDetalle.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetalle.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetalle.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grDetalle.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grDetalle.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grDetalle.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grDetalle.Size = New System.Drawing.Size(1095, 169)
        Me.grDetalle.TabIndex = 5
        Me.grDetalle.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grDetalle.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetalle.TableSpacing = 9
        Me.grDetalle.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grDetalle.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grDetalle.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grDetalle.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel1.Size = New System.Drawing.Size(1095, 36)
        Me.Panel1.TabIndex = 2
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Controls.Add(Me.Panel4)
        Me.Panel3.Controls.Add(Me.PictureBox1)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(1, 1)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1093, 34)
        Me.Panel3.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Label1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(60, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.Label1.Size = New System.Drawing.Size(253, 34)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Datos De la Venta"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.White
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel4.Location = New System.Drawing.Point(59, 0)
        Me.Panel4.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1, 34)
        Me.Panel4.TabIndex = 1
        '
        'MHighlighterFocus
        '
        Me.MHighlighterFocus.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Orange
        '
        'MEP
        '
        Me.MEP.ContainerControl = Me
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox1.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox1.Size = New System.Drawing.Size(59, 34)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'btnProductos
        '
        Me.btnProductos.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnProductos.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnProductos.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnProductos.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnProductos.ImageFixedSize = New System.Drawing.Size(24, 24)
        Me.btnProductos.Location = New System.Drawing.Point(883, 7)
        Me.btnProductos.Margin = New System.Windows.Forms.Padding(4)
        Me.btnProductos.Name = "btnProductos"
        Me.btnProductos.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnProductos.Size = New System.Drawing.Size(199, 53)
        Me.btnProductos.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnProductos.Symbol = "57410"
        Me.btnProductos.SymbolColor = System.Drawing.Color.DarkOrange
        Me.btnProductos.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnProductos.SymbolSize = 30.0!
        Me.btnProductos.TabIndex = 376
        Me.btnProductos.Text = "Volver a Productos"
        Me.btnProductos.Visible = False
        '
        'btnConfirmarSalir
        '
        Me.btnConfirmarSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnConfirmarSalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnConfirmarSalir.Font = New System.Drawing.Font("Calibri", 9.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnConfirmarSalir.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnConfirmarSalir.ImageFixedSize = New System.Drawing.Size(22, 22)
        Me.btnConfirmarSalir.Location = New System.Drawing.Point(605, 7)
        Me.btnConfirmarSalir.Margin = New System.Windows.Forms.Padding(4)
        Me.btnConfirmarSalir.Name = "btnConfirmarSalir"
        Me.btnConfirmarSalir.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnConfirmarSalir.Size = New System.Drawing.Size(253, 53)
        Me.btnConfirmarSalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnConfirmarSalir.Symbol = "57695"
        Me.btnConfirmarSalir.SymbolColor = System.Drawing.Color.MediumTurquoise
        Me.btnConfirmarSalir.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnConfirmarSalir.SymbolSize = 30.0!
        Me.btnConfirmarSalir.TabIndex = 3
        Me.btnConfirmarSalir.Text = "Confirmar / Salir"
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
        'Tec_VentasDetalle
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1095, 788)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Paneltop)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Name = "Tec_VentasDetalle"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Ventas Detalle"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Paneltop.ResumeLayout(False)
        CType(Me.grProducto, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel5.ResumeLayout(False)
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.grDetalle, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Paneltop As Panel
    Friend WithEvents grProducto As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel5 As Panel
    Friend WithEvents btnConfirmarSalir As DevComponents.DotNetBar.ButtonX
    Friend WithEvents LabelX9 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbProducto As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents Panel10 As Panel
    Friend WithEvents Panel11 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel12 As Panel
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents grDetalle As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel4 As Panel
    Friend WithEvents PictureBox1 As PictureBox
    Protected WithEvents MHighlighterFocus As DevComponents.DotNetBar.Validator.Highlighter
    Protected WithEvents MEP As ErrorProvider
    Friend WithEvents btnProductos As DevComponents.DotNetBar.ButtonX
End Class
