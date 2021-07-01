<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AyudaMultiCategoria
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
        Dim cbPrecio_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AyudaMultiCategoria))
        Dim cbSucursal_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Me.Panel17 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.grJBuscador = New Janus.Windows.GridEX.GridEX()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.ButtonX2 = New DevComponents.DotNetBar.ButtonX()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.cbPrecio = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.cbSucursal = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.btnSalir = New DevComponents.DotNetBar.ButtonX()
        Me.lbNombreProductos = New DevComponents.DotNetBar.LabelX()
        Me.tbNombre = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.Panel14 = New System.Windows.Forms.Panel()
        Me.Panel15 = New System.Windows.Forms.Panel()
        Me.lbTitulo = New System.Windows.Forms.Label()
        Me.Panel16 = New System.Windows.Forms.Panel()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.Panel17.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.grJBuscador, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.SuspendLayout()
        CType(Me.cbPrecio, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cbSucursal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel14.SuspendLayout()
        Me.Panel15.SuspendLayout()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel17
        '
        Me.Panel17.Controls.Add(Me.Panel1)
        Me.Panel17.Controls.Add(Me.Panel4)
        Me.Panel17.Controls.Add(Me.Panel14)
        Me.Panel17.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel17.Location = New System.Drawing.Point(0, 0)
        Me.Panel17.Name = "Panel17"
        Me.Panel17.Padding = New System.Windows.Forms.Padding(5)
        Me.Panel17.Size = New System.Drawing.Size(1222, 644)
        Me.Panel17.TabIndex = 1
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.grJBuscador)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(5, 202)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1212, 437)
        Me.Panel1.TabIndex = 2
        '
        'grJBuscador
        '
        Me.grJBuscador.AlternatingColors = True
        Me.grJBuscador.BackColor = System.Drawing.Color.White
        Me.grJBuscador.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grJBuscador.ColumnAutoResize = True
        Me.grJBuscador.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grJBuscador.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grJBuscador.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grJBuscador.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grJBuscador.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grJBuscador.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grJBuscador.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grJBuscador.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grJBuscador.HeaderFormatStyle.Alpha = 0
        Me.grJBuscador.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grJBuscador.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grJBuscador.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grJBuscador.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grJBuscador.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grJBuscador.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grJBuscador.Location = New System.Drawing.Point(0, 0)
        Me.grJBuscador.Margin = New System.Windows.Forms.Padding(4)
        Me.grJBuscador.Name = "grJBuscador"
        Me.grJBuscador.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grJBuscador.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grJBuscador.RecordNavigator = True
        Me.grJBuscador.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grJBuscador.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grJBuscador.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grJBuscador.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grJBuscador.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grJBuscador.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grJBuscador.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grJBuscador.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grJBuscador.Size = New System.Drawing.Size(1212, 437)
        Me.grJBuscador.TabIndex = 1
        Me.grJBuscador.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grJBuscador.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grJBuscador.TableSpacing = 9
        Me.grJBuscador.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grJBuscador.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grJBuscador.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grJBuscador.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.White
        Me.Panel4.Controls.Add(Me.ButtonX2)
        Me.Panel4.Controls.Add(Me.ButtonX1)
        Me.Panel4.Controls.Add(Me.LabelX2)
        Me.Panel4.Controls.Add(Me.cbPrecio)
        Me.Panel4.Controls.Add(Me.LabelX1)
        Me.Panel4.Controls.Add(Me.cbSucursal)
        Me.Panel4.Controls.Add(Me.btnSalir)
        Me.Panel4.Controls.Add(Me.lbNombreProductos)
        Me.Panel4.Controls.Add(Me.tbNombre)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(5, 41)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1212, 161)
        Me.Panel4.TabIndex = 1
        '
        'ButtonX2
        '
        Me.ButtonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX2.BackColor = System.Drawing.Color.Black
        Me.ButtonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.ButtonX2.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX2.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.ButtonX2.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.ButtonX2.Location = New System.Drawing.Point(646, 91)
        Me.ButtonX2.Name = "ButtonX2"
        Me.ButtonX2.Size = New System.Drawing.Size(184, 40)
        Me.ButtonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.ButtonX2.Symbol = ""
        Me.ButtonX2.TabIndex = 224
        Me.ButtonX2.Text = "Seleccionar Todos"
        Me.ButtonX2.TextColor = System.Drawing.Color.White
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.BackColor = System.Drawing.Color.DarkTurquoise
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.ButtonX1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.ButtonX1.Location = New System.Drawing.Point(836, 91)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(129, 40)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.ButtonX1.Symbol = ""
        Me.ButtonX1.SymbolColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ButtonX1.TabIndex = 223
        Me.ButtonX1.Text = "Continuar"
        Me.ButtonX1.TextColor = System.Drawing.Color.White
        '
        'LabelX2
        '
        Me.LabelX2.AutoSize = True
        Me.LabelX2.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX2.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX2.Location = New System.Drawing.Point(76, 63)
        Me.LabelX2.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX2.Size = New System.Drawing.Size(123, 23)
        Me.LabelX2.TabIndex = 222
        Me.LabelX2.Text = "Categoria Precio:"
        '
        'cbPrecio
        '
        Me.cbPrecio.BackColor = System.Drawing.Color.Azure
        Me.cbPrecio.ColorScheme = ""
        Me.cbPrecio.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbPrecio.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbPrecio.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        cbPrecio_DesignTimeLayout.LayoutString = resources.GetString("cbPrecio_DesignTimeLayout.LayoutString")
        Me.cbPrecio.DesignTimeLayout = cbPrecio_DesignTimeLayout
        Me.cbPrecio.FlatBorderColor = System.Drawing.Color.Black
        Me.cbPrecio.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbPrecio.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbPrecio.HideSelection = False
        Me.cbPrecio.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbPrecio.Location = New System.Drawing.Point(225, 55)
        Me.cbPrecio.Margin = New System.Windows.Forms.Padding(4)
        Me.cbPrecio.Name = "cbPrecio"
        Me.cbPrecio.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbPrecio.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbPrecio.SelectedIndex = -1
        Me.cbPrecio.SelectedItem = Nothing
        Me.cbPrecio.Size = New System.Drawing.Size(360, 32)
        Me.cbPrecio.TabIndex = 221
        Me.cbPrecio.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
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
        Me.LabelX1.Location = New System.Drawing.Point(133, 16)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(66, 23)
        Me.LabelX1.TabIndex = 220
        Me.LabelX1.Text = "Sucursal:"
        '
        'cbSucursal
        '
        Me.cbSucursal.BackColor = System.Drawing.Color.Azure
        Me.cbSucursal.ColorScheme = ""
        Me.cbSucursal.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbSucursal.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbSucursal.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        cbSucursal_DesignTimeLayout.LayoutString = resources.GetString("cbSucursal_DesignTimeLayout.LayoutString")
        Me.cbSucursal.DesignTimeLayout = cbSucursal_DesignTimeLayout
        Me.cbSucursal.FlatBorderColor = System.Drawing.Color.Black
        Me.cbSucursal.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbSucursal.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbSucursal.HideSelection = False
        Me.cbSucursal.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbSucursal.Location = New System.Drawing.Point(225, 11)
        Me.cbSucursal.Margin = New System.Windows.Forms.Padding(4)
        Me.cbSucursal.Name = "cbSucursal"
        Me.cbSucursal.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbSucursal.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbSucursal.SelectedIndex = -1
        Me.cbSucursal.SelectedItem = Nothing
        Me.cbSucursal.Size = New System.Drawing.Size(360, 32)
        Me.cbSucursal.TabIndex = 219
        Me.cbSucursal.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'btnSalir
        '
        Me.btnSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSalir.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(83, Byte), Integer), CType(CType(79, Byte), Integer))
        Me.btnSalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnSalir.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSalir.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.btnSalir.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.btnSalir.Location = New System.Drawing.Point(972, 91)
        Me.btnSalir.Name = "btnSalir"
        Me.btnSalir.Size = New System.Drawing.Size(93, 40)
        Me.btnSalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.btnSalir.Symbol = ""
        Me.btnSalir.TabIndex = 61
        Me.btnSalir.Text = "Salir"
        Me.btnSalir.TextColor = System.Drawing.Color.White
        '
        'lbNombreProductos
        '
        Me.lbNombreProductos.AutoSize = True
        Me.lbNombreProductos.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbNombreProductos.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbNombreProductos.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbNombreProductos.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbNombreProductos.Location = New System.Drawing.Point(14, 110)
        Me.lbNombreProductos.Margin = New System.Windows.Forms.Padding(4)
        Me.lbNombreProductos.Name = "lbNombreProductos"
        Me.lbNombreProductos.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbNombreProductos.Size = New System.Drawing.Size(185, 23)
        Me.lbNombreProductos.TabIndex = 60
        Me.lbNombreProductos.Text = "Introducir Datos a Buscar:"
        '
        'tbNombre
        '
        '
        '
        '
        Me.tbNombre.Border.BackColor = System.Drawing.Color.MidnightBlue
        Me.tbNombre.Border.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbNombre.Border.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbNombre.Border.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbNombre.Border.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbNombre.Border.Class = "TextBoxBorder"
        Me.tbNombre.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbNombre.FocusHighlightEnabled = True
        Me.tbNombre.Font = New System.Drawing.Font("Calibri", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbNombre.Location = New System.Drawing.Point(225, 104)
        Me.tbNombre.Margin = New System.Windows.Forms.Padding(4)
        Me.tbNombre.Name = "tbNombre"
        Me.tbNombre.PreventEnterBeep = True
        Me.tbNombre.Size = New System.Drawing.Size(360, 36)
        Me.tbNombre.TabIndex = 1
        '
        'Panel14
        '
        Me.Panel14.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel14.Controls.Add(Me.Panel15)
        Me.Panel14.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel14.Location = New System.Drawing.Point(5, 5)
        Me.Panel14.Name = "Panel14"
        Me.Panel14.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel14.Size = New System.Drawing.Size(1212, 36)
        Me.Panel14.TabIndex = 3
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
        Me.Panel15.Size = New System.Drawing.Size(1210, 34)
        Me.Panel15.TabIndex = 4
        '
        'lbTitulo
        '
        Me.lbTitulo.BackColor = System.Drawing.Color.Transparent
        Me.lbTitulo.Dock = System.Windows.Forms.DockStyle.Left
        Me.lbTitulo.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbTitulo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbTitulo.Location = New System.Drawing.Point(59, 0)
        Me.lbTitulo.Name = "lbTitulo"
        Me.lbTitulo.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.lbTitulo.Size = New System.Drawing.Size(625, 34)
        Me.lbTitulo.TabIndex = 10
        Me.lbTitulo.Text = "LISTADO DE MOVIMIENTOS"
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
        'AyudaMultiCategoria
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1222, 644)
        Me.Controls.Add(Me.Panel17)
        Me.Name = "AyudaMultiCategoria"
        Me.Text = "AyudaMultiCategoria"
        Me.Panel17.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.grJBuscador, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        CType(Me.cbPrecio, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cbSucursal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel14.ResumeLayout(False)
        Me.Panel15.ResumeLayout(False)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel17 As Panel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents grJBuscador As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel4 As Panel
    Friend WithEvents btnSalir As DevComponents.DotNetBar.ButtonX
    Friend WithEvents lbNombreProductos As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbNombre As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents Panel14 As Panel
    Friend WithEvents Panel15 As Panel
    Friend WithEvents lbTitulo As Label
    Friend WithEvents Panel16 As Panel
    Friend WithEvents PictureBox4 As PictureBox
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbPrecio As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbSucursal As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents ButtonX2 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
End Class
