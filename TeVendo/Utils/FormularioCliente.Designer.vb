<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormularioCliente
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
        Dim cbPrecios_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormularioCliente))
        Dim cbTipoDocumento_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Me.PanelPrincipal = New System.Windows.Forms.Panel()
        Me.PanelBuscador = New System.Windows.Forms.Panel()
        Me.Panel17 = New System.Windows.Forms.Panel()
        Me.grJBuscador = New Janus.Windows.GridEX.GridEX()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.lbNombreProductos = New DevComponents.DotNetBar.LabelX()
        Me.tbNombre = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.Panel14 = New System.Windows.Forms.Panel()
        Me.Panel15 = New System.Windows.Forms.Panel()
        Me.lbTitulo = New System.Windows.Forms.Label()
        Me.Panel16 = New System.Windows.Forms.Panel()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.PanelCrear = New System.Windows.Forms.Panel()
        Me.btnSi = New System.Windows.Forms.Panel()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.LabelX11 = New DevComponents.DotNetBar.LabelX()
        Me.tbNroDocumento = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.LabelX10 = New DevComponents.DotNetBar.LabelX()
        Me.tbTelefono = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.cbPrecios = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX5 = New DevComponents.DotNetBar.LabelX()
        Me.cbTipoDocumento = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX7 = New DevComponents.DotNetBar.LabelX()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.tbNombreCliente = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.PanelCabezeraTitulo = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.PanelPrincipal.SuspendLayout()
        Me.PanelBuscador.SuspendLayout()
        Me.Panel17.SuspendLayout()
        CType(Me.grJBuscador, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel4.SuspendLayout()
        Me.Panel14.SuspendLayout()
        Me.Panel15.SuspendLayout()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelCrear.SuspendLayout()
        Me.btnSi.SuspendLayout()
        CType(Me.cbPrecios, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cbTipoDocumento, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelCabezeraTitulo.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelPrincipal
        '
        Me.PanelPrincipal.BackColor = System.Drawing.Color.MediumSeaGreen
        Me.PanelPrincipal.Controls.Add(Me.PanelBuscador)
        Me.PanelPrincipal.Controls.Add(Me.PanelCrear)
        Me.PanelPrincipal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelPrincipal.Location = New System.Drawing.Point(0, 0)
        Me.PanelPrincipal.Name = "PanelPrincipal"
        Me.PanelPrincipal.Padding = New System.Windows.Forms.Padding(5)
        Me.PanelPrincipal.Size = New System.Drawing.Size(1247, 633)
        Me.PanelPrincipal.TabIndex = 0
        '
        'PanelBuscador
        '
        Me.PanelBuscador.Controls.Add(Me.Panel17)
        Me.PanelBuscador.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelBuscador.Location = New System.Drawing.Point(5, 5)
        Me.PanelBuscador.Name = "PanelBuscador"
        Me.PanelBuscador.Size = New System.Drawing.Size(707, 623)
        Me.PanelBuscador.TabIndex = 0
        '
        'Panel17
        '
        Me.Panel17.Controls.Add(Me.grJBuscador)
        Me.Panel17.Controls.Add(Me.Panel4)
        Me.Panel17.Controls.Add(Me.Panel14)
        Me.Panel17.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel17.Location = New System.Drawing.Point(0, 0)
        Me.Panel17.Name = "Panel17"
        Me.Panel17.Padding = New System.Windows.Forms.Padding(5)
        Me.Panel17.Size = New System.Drawing.Size(707, 623)
        Me.Panel17.TabIndex = 0
        '
        'grJBuscador
        '
        Me.grJBuscador.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
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
        Me.grJBuscador.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grJBuscador.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grJBuscador.HeaderFormatStyle.Alpha = 0
        Me.grJBuscador.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grJBuscador.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grJBuscador.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grJBuscador.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grJBuscador.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grJBuscador.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grJBuscador.Location = New System.Drawing.Point(5, 105)
        Me.grJBuscador.Margin = New System.Windows.Forms.Padding(4)
        Me.grJBuscador.Name = "grJBuscador"
        Me.grJBuscador.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grJBuscador.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grJBuscador.RecordNavigator = True
        Me.grJBuscador.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
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
        Me.grJBuscador.Size = New System.Drawing.Size(697, 513)
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
        Me.Panel4.Controls.Add(Me.lbNombreProductos)
        Me.Panel4.Controls.Add(Me.tbNombre)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(5, 41)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(697, 64)
        Me.Panel4.TabIndex = 0
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
        Me.lbNombreProductos.Location = New System.Drawing.Point(14, 23)
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
        Me.tbNombre.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbNombre.Location = New System.Drawing.Point(229, 21)
        Me.tbNombre.Margin = New System.Windows.Forms.Padding(4)
        Me.tbNombre.Name = "tbNombre"
        Me.tbNombre.PreventEnterBeep = True
        Me.tbNombre.Size = New System.Drawing.Size(270, 32)
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
        Me.Panel14.Size = New System.Drawing.Size(697, 36)
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
        Me.Panel15.Size = New System.Drawing.Size(695, 34)
        Me.Panel15.TabIndex = 0
        '
        'lbTitulo
        '
        Me.lbTitulo.BackColor = System.Drawing.Color.Transparent
        Me.lbTitulo.Dock = System.Windows.Forms.DockStyle.Left
        Me.lbTitulo.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbTitulo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbTitulo.Location = New System.Drawing.Point(59, 0)
        Me.lbTitulo.Name = "lbTitulo"
        Me.lbTitulo.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.lbTitulo.Size = New System.Drawing.Size(254, 34)
        Me.lbTitulo.TabIndex = 2
        Me.lbTitulo.Text = "LISTADO DE CLIENTES"
        Me.lbTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel16
        '
        Me.Panel16.BackColor = System.Drawing.Color.White
        Me.Panel16.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel16.Location = New System.Drawing.Point(58, 0)
        Me.Panel16.Name = "Panel16"
        Me.Panel16.Size = New System.Drawing.Size(1, 34)
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
        Me.PictureBox4.Size = New System.Drawing.Size(58, 34)
        Me.PictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox4.TabIndex = 0
        Me.PictureBox4.TabStop = False
        '
        'PanelCrear
        '
        Me.PanelCrear.BackColor = System.Drawing.Color.White
        Me.PanelCrear.Controls.Add(Me.btnSi)
        Me.PanelCrear.Controls.Add(Me.tbNombreCliente)
        Me.PanelCrear.Controls.Add(Me.LabelX11)
        Me.PanelCrear.Controls.Add(Me.tbNroDocumento)
        Me.PanelCrear.Controls.Add(Me.LabelX10)
        Me.PanelCrear.Controls.Add(Me.tbTelefono)
        Me.PanelCrear.Controls.Add(Me.cbPrecios)
        Me.PanelCrear.Controls.Add(Me.LabelX5)
        Me.PanelCrear.Controls.Add(Me.cbTipoDocumento)
        Me.PanelCrear.Controls.Add(Me.LabelX7)
        Me.PanelCrear.Controls.Add(Me.LabelX1)
        Me.PanelCrear.Controls.Add(Me.PanelCabezeraTitulo)
        Me.PanelCrear.Dock = System.Windows.Forms.DockStyle.Right
        Me.PanelCrear.Location = New System.Drawing.Point(712, 5)
        Me.PanelCrear.Name = "PanelCrear"
        Me.PanelCrear.Padding = New System.Windows.Forms.Padding(5)
        Me.PanelCrear.Size = New System.Drawing.Size(530, 623)
        Me.PanelCrear.TabIndex = 1
        '
        'btnSi
        '
        Me.btnSi.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(179, Byte), Integer), CType(CType(148, Byte), Integer))
        Me.btnSi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.btnSi.Controls.Add(Me.ButtonX1)
        Me.btnSi.Location = New System.Drawing.Point(168, 275)
        Me.btnSi.Name = "btnSi"
        Me.btnSi.Size = New System.Drawing.Size(180, 54)
        Me.btnSi.TabIndex = 224
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.BackColor = System.Drawing.Color.Transparent
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonX1.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.iconguardar
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(40, 40)
        Me.ButtonX1.Location = New System.Drawing.Point(0, 0)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(178, 52)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX1.TabIndex = 8
        Me.ButtonX1.Text = "Guardar Cliente"
        Me.ButtonX1.TextColor = System.Drawing.Color.White
        '
        'LabelX11
        '
        Me.LabelX11.AutoSize = True
        Me.LabelX11.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX11.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX11.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX11.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX11.Location = New System.Drawing.Point(22, 185)
        Me.LabelX11.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX11.Name = "LabelX11"
        Me.LabelX11.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX11.Size = New System.Drawing.Size(120, 23)
        Me.LabelX11.TabIndex = 223
        Me.LabelX11.Text = "Nro Documento:"
        '
        'tbNroDocumento
        '
        '
        '
        '
        Me.tbNroDocumento.Border.Class = "TextBoxBorder"
        Me.tbNroDocumento.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbNroDocumento.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbNroDocumento.Location = New System.Drawing.Point(174, 181)
        Me.tbNroDocumento.Margin = New System.Windows.Forms.Padding(4)
        Me.tbNroDocumento.Name = "tbNroDocumento"
        Me.tbNroDocumento.PreventEnterBeep = True
        Me.tbNroDocumento.Size = New System.Drawing.Size(171, 28)
        Me.tbNroDocumento.TabIndex = 6
        Me.tbNroDocumento.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LabelX10
        '
        Me.LabelX10.AutoSize = True
        Me.LabelX10.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX10.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX10.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX10.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX10.Location = New System.Drawing.Point(22, 118)
        Me.LabelX10.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX10.Name = "LabelX10"
        Me.LabelX10.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX10.Size = New System.Drawing.Size(70, 23)
        Me.LabelX10.TabIndex = 222
        Me.LabelX10.Text = "Telefono:"
        '
        'tbTelefono
        '
        '
        '
        '
        Me.tbTelefono.Border.Class = "TextBoxBorder"
        Me.tbTelefono.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbTelefono.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTelefono.Location = New System.Drawing.Point(174, 113)
        Me.tbTelefono.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTelefono.Name = "tbTelefono"
        Me.tbTelefono.PreventEnterBeep = True
        Me.tbTelefono.Size = New System.Drawing.Size(206, 28)
        Me.tbTelefono.TabIndex = 4
        Me.tbTelefono.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'cbPrecios
        '
        cbPrecios_DesignTimeLayout.LayoutString = resources.GetString("cbPrecios_DesignTimeLayout.LayoutString")
        Me.cbPrecios.DesignTimeLayout = cbPrecios_DesignTimeLayout
        Me.cbPrecios.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbPrecios.Location = New System.Drawing.Point(174, 213)
        Me.cbPrecios.Margin = New System.Windows.Forms.Padding(4)
        Me.cbPrecios.Name = "cbPrecios"
        Me.cbPrecios.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbPrecios.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.cbPrecios.SelectedIndex = -1
        Me.cbPrecios.SelectedItem = Nothing
        Me.cbPrecios.Size = New System.Drawing.Size(219, 26)
        Me.cbPrecios.TabIndex = 7
        Me.cbPrecios.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'LabelX5
        '
        Me.LabelX5.AutoSize = True
        Me.LabelX5.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX5.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX5.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX5.Location = New System.Drawing.Point(22, 221)
        Me.LabelX5.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX5.Name = "LabelX5"
        Me.LabelX5.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX5.Size = New System.Drawing.Size(130, 23)
        Me.LabelX5.TabIndex = 221
        Me.LabelX5.Text = "Categoria Precios:"
        '
        'cbTipoDocumento
        '
        cbTipoDocumento_DesignTimeLayout.LayoutString = resources.GetString("cbTipoDocumento_DesignTimeLayout.LayoutString")
        Me.cbTipoDocumento.DesignTimeLayout = cbTipoDocumento_DesignTimeLayout
        Me.cbTipoDocumento.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbTipoDocumento.Location = New System.Drawing.Point(174, 146)
        Me.cbTipoDocumento.Margin = New System.Windows.Forms.Padding(4)
        Me.cbTipoDocumento.Name = "cbTipoDocumento"
        Me.cbTipoDocumento.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbTipoDocumento.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.cbTipoDocumento.SelectedIndex = -1
        Me.cbTipoDocumento.SelectedItem = Nothing
        Me.cbTipoDocumento.Size = New System.Drawing.Size(206, 28)
        Me.cbTipoDocumento.TabIndex = 5
        Me.cbTipoDocumento.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
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
        Me.LabelX7.Location = New System.Drawing.Point(22, 154)
        Me.LabelX7.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX7.Name = "LabelX7"
        Me.LabelX7.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX7.Size = New System.Drawing.Size(125, 23)
        Me.LabelX7.TabIndex = 220
        Me.LabelX7.Text = "Tipo Documento:"
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
        Me.LabelX1.Location = New System.Drawing.Point(22, 81)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(118, 23)
        Me.LabelX1.TabIndex = 219
        Me.LabelX1.Text = "Nombre Cliente:"
        '
        'tbNombreCliente
        '
        '
        '
        '
        Me.tbNombreCliente.Border.Class = "TextBoxBorder"
        Me.tbNombreCliente.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbNombreCliente.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbNombreCliente.Location = New System.Drawing.Point(175, 76)
        Me.tbNombreCliente.Margin = New System.Windows.Forms.Padding(4)
        Me.tbNombreCliente.Name = "tbNombreCliente"
        Me.tbNombreCliente.PreventEnterBeep = True
        Me.tbNombreCliente.Size = New System.Drawing.Size(324, 28)
        Me.tbNombreCliente.TabIndex = 0
        '
        'PanelCabezeraTitulo
        '
        Me.PanelCabezeraTitulo.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.PanelCabezeraTitulo.Controls.Add(Me.Panel2)
        Me.PanelCabezeraTitulo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelCabezeraTitulo.Location = New System.Drawing.Point(5, 5)
        Me.PanelCabezeraTitulo.Name = "PanelCabezeraTitulo"
        Me.PanelCabezeraTitulo.Padding = New System.Windows.Forms.Padding(1)
        Me.PanelCabezeraTitulo.Size = New System.Drawing.Size(520, 36)
        Me.PanelCabezeraTitulo.TabIndex = 4
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
        Me.Panel2.Size = New System.Drawing.Size(518, 34)
        Me.Panel2.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Label1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(59, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.Label1.Size = New System.Drawing.Size(254, 34)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "DATOS PARA CREAR NUEVO CLIENTE"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.White
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel3.Location = New System.Drawing.Point(58, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1, 34)
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
        Me.PictureBox1.Size = New System.Drawing.Size(58, 34)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'FormularioCliente
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1247, 633)
        Me.Controls.Add(Me.PanelPrincipal)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormularioCliente"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "0"
        Me.PanelPrincipal.ResumeLayout(False)
        Me.PanelBuscador.ResumeLayout(False)
        Me.Panel17.ResumeLayout(False)
        CType(Me.grJBuscador, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel14.ResumeLayout(False)
        Me.Panel15.ResumeLayout(False)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelCrear.ResumeLayout(False)
        Me.PanelCrear.PerformLayout()
        Me.btnSi.ResumeLayout(False)
        CType(Me.cbPrecios, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cbTipoDocumento, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelCabezeraTitulo.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelPrincipal As Panel
    Friend WithEvents PanelCrear As Panel
    Friend WithEvents PanelBuscador As Panel
    Friend WithEvents Panel17 As Panel
    Friend WithEvents grJBuscador As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel4 As Panel
    Friend WithEvents lbNombreProductos As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbNombre As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents Panel14 As Panel
    Friend WithEvents Panel15 As Panel
    Friend WithEvents lbTitulo As Label
    Friend WithEvents Panel16 As Panel
    Friend WithEvents PictureBox4 As PictureBox
    Friend WithEvents PanelCabezeraTitulo As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel3 As Panel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents LabelX11 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbNroDocumento As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX10 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbTelefono As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents cbPrecios As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX5 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbTipoDocumento As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX7 As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbNombreCliente As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents btnSi As Panel
    Protected WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
End Class
