<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FProductosPorClientes
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
        Dim cbProveedor_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FProductosPorClientes))
        Dim cbCategoriaProducto_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Me.Paneltop = New System.Windows.Forms.Panel()
        Me.grProducto = New Janus.Windows.GridEX.GridEX()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.cbProveedor = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.cbCategoriaProducto = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.btnProductosSinStock = New DevComponents.DotNetBar.ButtonX()
        Me.btnProductos = New DevComponents.DotNetBar.ButtonX()
        Me.btnConfirmarSalir = New DevComponents.DotNetBar.ButtonX()
        Me.LabelX9 = New DevComponents.DotNetBar.LabelX()
        Me.tbProducto = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.LabelX3 = New DevComponents.DotNetBar.LabelX()
        Me.tbNameCliente = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.chkTodosClientes = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.btnClientes = New DevComponents.DotNetBar.ButtonX()
        Me.cbFechaHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX4 = New DevComponents.DotNetBar.LabelX()
        Me.cbFechaDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX5 = New DevComponents.DotNetBar.LabelX()
        Me.MHighlighterFocus = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.c = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.Paneltop.SuspendLayout()
        CType(Me.grProducto, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel5.SuspendLayout()
        CType(Me.cbProveedor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cbCategoriaProducto, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Paneltop
        '
        Me.Paneltop.BackColor = System.Drawing.Color.White
        Me.Paneltop.Controls.Add(Me.grProducto)
        Me.Paneltop.Controls.Add(Me.Panel5)
        Me.Paneltop.Controls.Add(Me.Panel10)
        Me.Paneltop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Paneltop.Location = New System.Drawing.Point(0, 0)
        Me.Paneltop.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Paneltop.Name = "Paneltop"
        Me.Paneltop.Size = New System.Drawing.Size(1444, 532)
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
        Me.grProducto.Location = New System.Drawing.Point(0, 212)
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
        Me.grProducto.SelectedFormatStyle.BackColor = System.Drawing.Color.Gold
        Me.grProducto.SelectedFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grProducto.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grProducto.SelectedFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grProducto.Size = New System.Drawing.Size(1444, 320)
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
        Me.Panel5.Controls.Add(Me.c)
        Me.Panel5.Controls.Add(Me.ButtonX1)
        Me.Panel5.Controls.Add(Me.cbFechaHasta)
        Me.Panel5.Controls.Add(Me.LabelX4)
        Me.Panel5.Controls.Add(Me.cbFechaDesde)
        Me.Panel5.Controls.Add(Me.LabelX5)
        Me.Panel5.Controls.Add(Me.chkTodosClientes)
        Me.Panel5.Controls.Add(Me.btnClientes)
        Me.Panel5.Controls.Add(Me.LabelX3)
        Me.Panel5.Controls.Add(Me.tbNameCliente)
        Me.Panel5.Controls.Add(Me.cbProveedor)
        Me.Panel5.Controls.Add(Me.LabelX2)
        Me.Panel5.Controls.Add(Me.cbCategoriaProducto)
        Me.Panel5.Controls.Add(Me.LabelX1)
        Me.Panel5.Controls.Add(Me.btnProductosSinStock)
        Me.Panel5.Controls.Add(Me.btnProductos)
        Me.Panel5.Controls.Add(Me.btnConfirmarSalir)
        Me.Panel5.Controls.Add(Me.LabelX9)
        Me.Panel5.Controls.Add(Me.tbProducto)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel5.Location = New System.Drawing.Point(0, 36)
        Me.Panel5.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(1444, 176)
        Me.Panel5.TabIndex = 3
        '
        'cbProveedor
        '
        Me.cbProveedor.BackColor = System.Drawing.Color.Azure
        Me.cbProveedor.ColorScheme = ""
        Me.cbProveedor.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbProveedor.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbProveedor.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        cbProveedor_DesignTimeLayout.LayoutString = resources.GetString("cbProveedor_DesignTimeLayout.LayoutString")
        Me.cbProveedor.DesignTimeLayout = cbProveedor_DesignTimeLayout
        Me.cbProveedor.FlatBorderColor = System.Drawing.Color.Black
        Me.cbProveedor.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbProveedor.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbProveedor.HideSelection = False
        Me.cbProveedor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbProveedor.Location = New System.Drawing.Point(209, 59)
        Me.cbProveedor.Margin = New System.Windows.Forms.Padding(4)
        Me.cbProveedor.Name = "cbProveedor"
        Me.cbProveedor.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbProveedor.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbProveedor.SelectedIndex = -1
        Me.cbProveedor.SelectedItem = Nothing
        Me.cbProveedor.Size = New System.Drawing.Size(332, 28)
        Me.cbProveedor.TabIndex = 380
        Me.cbProveedor.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'LabelX2
        '
        Me.LabelX2.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX2.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX2.Location = New System.Drawing.Point(15, 62)
        Me.LabelX2.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX2.Size = New System.Drawing.Size(151, 28)
        Me.LabelX2.TabIndex = 379
        Me.LabelX2.Text = "Proveedor:"
        '
        'cbCategoriaProducto
        '
        Me.cbCategoriaProducto.BackColor = System.Drawing.Color.Azure
        Me.cbCategoriaProducto.ColorScheme = ""
        Me.cbCategoriaProducto.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbCategoriaProducto.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbCategoriaProducto.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        cbCategoriaProducto_DesignTimeLayout.LayoutString = resources.GetString("cbCategoriaProducto_DesignTimeLayout.LayoutString")
        Me.cbCategoriaProducto.DesignTimeLayout = cbCategoriaProducto_DesignTimeLayout
        Me.cbCategoriaProducto.FlatBorderColor = System.Drawing.Color.Black
        Me.cbCategoriaProducto.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbCategoriaProducto.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbCategoriaProducto.HideSelection = False
        Me.cbCategoriaProducto.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbCategoriaProducto.Location = New System.Drawing.Point(209, 95)
        Me.cbCategoriaProducto.Margin = New System.Windows.Forms.Padding(4)
        Me.cbCategoriaProducto.Name = "cbCategoriaProducto"
        Me.cbCategoriaProducto.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbCategoriaProducto.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbCategoriaProducto.SelectedIndex = -1
        Me.cbCategoriaProducto.SelectedItem = Nothing
        Me.cbCategoriaProducto.Size = New System.Drawing.Size(332, 28)
        Me.cbCategoriaProducto.TabIndex = 378
        Me.cbCategoriaProducto.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'LabelX1
        '
        Me.LabelX1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX1.Location = New System.Drawing.Point(15, 93)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(173, 28)
        Me.LabelX1.TabIndex = 377
        Me.LabelX1.Text = "Categoria Producto:"
        '
        'btnProductosSinStock
        '
        Me.btnProductosSinStock.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnProductosSinStock.BackColor = System.Drawing.Color.Honeydew
        Me.btnProductosSinStock.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnProductosSinStock.DisabledImagesGrayScale = False
        Me.btnProductosSinStock.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnProductosSinStock.Image = Global.TeVendo.My.Resources.Resources.sheets
        Me.btnProductosSinStock.ImageFixedSize = New System.Drawing.Size(25, 25)
        Me.btnProductosSinStock.Location = New System.Drawing.Point(1068, 16)
        Me.btnProductosSinStock.Margin = New System.Windows.Forms.Padding(4)
        Me.btnProductosSinStock.Name = "btnProductosSinStock"
        Me.btnProductosSinStock.Size = New System.Drawing.Size(160, 42)
        Me.btnProductosSinStock.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnProductosSinStock.TabIndex = 376
        Me.btnProductosSinStock.Text = "Exportar Excel"
        Me.btnProductosSinStock.TextColor = System.Drawing.Color.Black
        '
        'btnProductos
        '
        Me.btnProductos.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnProductos.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnProductos.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnProductos.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnProductos.ImageFixedSize = New System.Drawing.Size(24, 24)
        Me.btnProductos.Location = New System.Drawing.Point(1073, 68)
        Me.btnProductos.Margin = New System.Windows.Forms.Padding(4)
        Me.btnProductos.Name = "btnProductos"
        Me.btnProductos.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnProductos.Size = New System.Drawing.Size(163, 53)
        Me.btnProductos.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnProductos.Symbol = ""
        Me.btnProductos.SymbolColor = System.Drawing.Color.DarkOrange
        Me.btnProductos.SymbolSize = 20.0!
        Me.btnProductos.TabIndex = 375
        Me.btnProductos.Text = "Exportar Reporte"
        '
        'btnConfirmarSalir
        '
        Me.btnConfirmarSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnConfirmarSalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnConfirmarSalir.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnConfirmarSalir.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnConfirmarSalir.ImageFixedSize = New System.Drawing.Size(24, 24)
        Me.btnConfirmarSalir.Location = New System.Drawing.Point(712, 77)
        Me.btnConfirmarSalir.Margin = New System.Windows.Forms.Padding(4)
        Me.btnConfirmarSalir.Name = "btnConfirmarSalir"
        Me.btnConfirmarSalir.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnConfirmarSalir.Size = New System.Drawing.Size(249, 53)
        Me.btnConfirmarSalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnConfirmarSalir.Symbol = "57408"
        Me.btnConfirmarSalir.SymbolColor = System.Drawing.Color.MediumTurquoise
        Me.btnConfirmarSalir.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnConfirmarSalir.SymbolSize = 20.0!
        Me.btnConfirmarSalir.TabIndex = 3
        Me.btnConfirmarSalir.Text = "Generar"
        '
        'LabelX9
        '
        Me.LabelX9.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX9.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX9.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX9.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX9.Location = New System.Drawing.Point(15, 17)
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
        Me.tbProducto.Location = New System.Drawing.Point(209, 18)
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
        Me.Panel10.Size = New System.Drawing.Size(1444, 36)
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
        Me.Panel11.Size = New System.Drawing.Size(1442, 34)
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
        Me.Label3.Text = "MOVIMIENTOS POR VENTAS"
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
        'LabelX3
        '
        Me.LabelX3.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX3.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX3.Location = New System.Drawing.Point(15, 140)
        Me.LabelX3.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX3.Name = "LabelX3"
        Me.LabelX3.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX3.Size = New System.Drawing.Size(151, 28)
        Me.LabelX3.TabIndex = 382
        Me.LabelX3.Text = "Cliente"
        '
        'tbNameCliente
        '
        Me.tbNameCliente.BackColor = System.Drawing.Color.White
        '
        '
        '
        Me.tbNameCliente.Border.Class = "TextBoxBorder"
        Me.tbNameCliente.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbNameCliente.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbNameCliente.ForeColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.tbNameCliente.Location = New System.Drawing.Point(209, 141)
        Me.tbNameCliente.Margin = New System.Windows.Forms.Padding(4)
        Me.tbNameCliente.Name = "tbNameCliente"
        Me.tbNameCliente.PreventEnterBeep = True
        Me.tbNameCliente.Size = New System.Drawing.Size(332, 24)
        Me.tbNameCliente.TabIndex = 381
        '
        'chkTodosClientes
        '
        '
        '
        '
        Me.chkTodosClientes.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.chkTodosClientes.CheckSignSize = New System.Drawing.Size(16, 16)
        Me.chkTodosClientes.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkTodosClientes.Location = New System.Drawing.Point(605, 139)
        Me.chkTodosClientes.Name = "chkTodosClientes"
        Me.chkTodosClientes.Size = New System.Drawing.Size(85, 23)
        Me.chkTodosClientes.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.chkTodosClientes.TabIndex = 384
        Me.chkTodosClientes.Text = "Todos"
        Me.chkTodosClientes.TextColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'btnClientes
        '
        Me.btnClientes.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnClientes.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnClientes.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnClientes.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.btnClientes.Location = New System.Drawing.Point(549, 132)
        Me.btnClientes.Margin = New System.Windows.Forms.Padding(4)
        Me.btnClientes.Name = "btnClientes"
        Me.btnClientes.Size = New System.Drawing.Size(50, 36)
        Me.btnClientes.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnClientes.TabIndex = 383
        Me.btnClientes.Visible = False
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
        Me.cbFechaHasta.Location = New System.Drawing.Point(761, 48)
        Me.cbFechaHasta.Name = "cbFechaHasta"
        Me.cbFechaHasta.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaHasta.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaHasta.SecondIncrement = 10
        Me.cbFechaHasta.Size = New System.Drawing.Size(200, 26)
        Me.cbFechaHasta.TabIndex = 388
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
        Me.LabelX4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX4.Location = New System.Drawing.Point(712, 48)
        Me.LabelX4.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX4.Name = "LabelX4"
        Me.LabelX4.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX4.Size = New System.Drawing.Size(42, 21)
        Me.LabelX4.TabIndex = 387
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
        Me.cbFechaDesde.Location = New System.Drawing.Point(761, 19)
        Me.cbFechaDesde.Name = "cbFechaDesde"
        Me.cbFechaDesde.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.cbFechaDesde.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.cbFechaDesde.SecondIncrement = 10
        Me.cbFechaDesde.Size = New System.Drawing.Size(200, 26)
        Me.cbFechaDesde.TabIndex = 386
        Me.cbFechaDesde.TodayButtonText = "Hoy"
        Me.cbFechaDesde.UseCompatibleTextRendering = False
        Me.cbFechaDesde.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.cbFechaDesde.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.cbFechaDesde.YearIncrement = 10
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
        Me.LabelX5.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX5.Location = New System.Drawing.Point(709, 19)
        Me.LabelX5.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX5.Name = "LabelX5"
        Me.LabelX5.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX5.Size = New System.Drawing.Size(45, 21)
        Me.LabelX5.TabIndex = 385
        Me.LabelX5.Text = "Desde:"
        '
        'MHighlighterFocus
        '
        Me.MHighlighterFocus.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Orange
        '
        'c
        '
        '
        '
        '
        Me.c.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.c.CheckSignSize = New System.Drawing.Size(16, 16)
        Me.c.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.c.Location = New System.Drawing.Point(605, 16)
        Me.c.Name = "c"
        Me.c.Size = New System.Drawing.Size(85, 23)
        Me.c.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.c.TabIndex = 390
        Me.c.Text = "Todos"
        Me.c.TextColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.search
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.ButtonX1.Location = New System.Drawing.Point(549, 9)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(50, 36)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX1.TabIndex = 389
        Me.ButtonX1.Visible = False
        '
        'FProductosPorClientes
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1444, 532)
        Me.Controls.Add(Me.Paneltop)
        Me.Name = "FProductosPorClientes"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FProductosPorClientes"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Paneltop.ResumeLayout(False)
        CType(Me.grProducto, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        CType(Me.cbProveedor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cbCategoriaProducto, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Paneltop As Panel
    Friend WithEvents grProducto As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel5 As Panel
    Friend WithEvents cbProveedor As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbCategoriaProducto As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Protected WithEvents btnProductosSinStock As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnProductos As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnConfirmarSalir As DevComponents.DotNetBar.ButtonX
    Friend WithEvents LabelX9 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbProducto As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents Panel10 As Panel
    Friend WithEvents Panel11 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel12 As Panel
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents LabelX3 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbNameCliente As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents chkTodosClientes As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents btnClientes As DevComponents.DotNetBar.ButtonX
    Friend WithEvents cbFechaHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX4 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbFechaDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX5 As DevComponents.DotNetBar.LabelX
    Protected WithEvents MHighlighterFocus As DevComponents.DotNetBar.Validator.Highlighter
    Friend WithEvents c As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
End Class
