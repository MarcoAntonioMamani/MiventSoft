<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EfectivoCobranza
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
        Me.lbTitulo = New System.Windows.Forms.Label()
        Me.Panel16 = New System.Windows.Forms.Panel()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel20 = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnGuardar = New DevComponents.DotNetBar.ButtonX()
        Me.btnSalir = New DevComponents.DotNetBar.ButtonX()
        Me.tbCambio = New DevComponents.Editors.DoubleInput()
        Me.LabelX17 = New DevComponents.DotNetBar.LabelX()
        Me.tbTotalPagado = New DevComponents.Editors.DoubleInput()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.chkMontoBs = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.lbDolar = New DevComponents.DotNetBar.LabelX()
        Me.chkTransferencia = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.chkTarjeta = New DevComponents.DotNetBar.Controls.CheckBoxX()
        Me.tbTransferencia = New DevComponents.Editors.DoubleInput()
        Me.tbTarjeta = New DevComponents.Editors.DoubleInput()
        Me.tbMontoBs = New DevComponents.Editors.DoubleInput()
        Me.tbMontoDolar = New DevComponents.Editors.DoubleInput()
        Me.LabelX13 = New DevComponents.DotNetBar.LabelX()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.tbTotalVenta = New DevComponents.Editors.DoubleInput()
        Me.LabelX18 = New DevComponents.DotNetBar.LabelX()
        Me.Highlighter2 = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.Panel14.SuspendLayout()
        Me.Panel15.SuspendLayout()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel20.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.tbCambio, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbTotalPagado, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox3.SuspendLayout()
        CType(Me.tbTransferencia, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbTarjeta, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbMontoBs, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbMontoDolar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        CType(Me.tbTotalVenta, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.Panel14.Size = New System.Drawing.Size(1002, 36)
        Me.Panel14.TabIndex = 6
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
        Me.Panel15.Size = New System.Drawing.Size(1000, 34)
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
        Me.lbTitulo.Size = New System.Drawing.Size(941, 34)
        Me.lbTitulo.TabIndex = 10
        Me.lbTitulo.Text = "Registrar Monto A Pagar"
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
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Controls.Add(Me.GroupBox2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 36)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1002, 317)
        Me.Panel1.TabIndex = 7
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Panel20)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 80)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1002, 237)
        Me.Panel2.TabIndex = 1
        '
        'Panel20
        '
        Me.Panel20.Controls.Add(Me.GroupBox1)
        Me.Panel20.Controls.Add(Me.GroupBox3)
        Me.Panel20.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel20.Location = New System.Drawing.Point(0, 0)
        Me.Panel20.Name = "Panel20"
        Me.Panel20.Size = New System.Drawing.Size(1002, 237)
        Me.Panel20.TabIndex = 2
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.GroupBox1.Controls.Add(Me.ButtonX1)
        Me.GroupBox1.Controls.Add(Me.btnGuardar)
        Me.GroupBox1.Controls.Add(Me.btnSalir)
        Me.GroupBox1.Controls.Add(Me.tbCambio)
        Me.GroupBox1.Controls.Add(Me.LabelX17)
        Me.GroupBox1.Controls.Add(Me.tbTotalPagado)
        Me.GroupBox1.Controls.Add(Me.LabelX1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Cornsilk
        Me.GroupBox1.Location = New System.Drawing.Point(528, 0)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Size = New System.Drawing.Size(474, 237)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Efectivo Pagado / Cambio:"
        '
        'btnGuardar
        '
        Me.btnGuardar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnGuardar.BackColor = System.Drawing.Color.Lime
        Me.btnGuardar.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnGuardar.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGuardar.Image = Global.TeVendo.My.Resources.Resources.iconguardar
        Me.btnGuardar.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnGuardar.Location = New System.Drawing.Point(10, 142)
        Me.btnGuardar.Margin = New System.Windows.Forms.Padding(4)
        Me.btnGuardar.Name = "btnGuardar"
        Me.btnGuardar.Size = New System.Drawing.Size(129, 54)
        Me.btnGuardar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnGuardar.Symbol = "59500"
        Me.btnGuardar.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnGuardar.TabIndex = 6
        Me.btnGuardar.Text = "Grabar" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Venta"
        Me.btnGuardar.TextColor = System.Drawing.Color.Black
        '
        'btnSalir
        '
        Me.btnSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSalir.BackColor = System.Drawing.Color.OrangeRed
        Me.btnSalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnSalir.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSalir.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.btnSalir.ImageFixedSize = New System.Drawing.Size(20, 20)
        Me.btnSalir.Location = New System.Drawing.Point(283, 142)
        Me.btnSalir.Name = "btnSalir"
        Me.btnSalir.Size = New System.Drawing.Size(129, 54)
        Me.btnSalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.btnSalir.Symbol = ""
        Me.btnSalir.TabIndex = 7
        Me.btnSalir.Text = "Salir"
        Me.btnSalir.TextColor = System.Drawing.Color.White
        '
        'tbCambio
        '
        '
        '
        '
        Me.tbCambio.BackgroundStyle.BackColor = System.Drawing.Color.Gold
        Me.tbCambio.BackgroundStyle.BackColor2 = System.Drawing.Color.Maroon
        Me.tbCambio.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbCambio.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbCambio.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbCambio.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCambio.ForeColor = System.Drawing.Color.White
        Me.tbCambio.Increment = 1.0R
        Me.tbCambio.IsInputReadOnly = True
        Me.tbCambio.Location = New System.Drawing.Point(130, 81)
        Me.tbCambio.Margin = New System.Windows.Forms.Padding(4)
        Me.tbCambio.MinValue = 0R
        Me.tbCambio.Name = "tbCambio"
        Me.tbCambio.Size = New System.Drawing.Size(160, 38)
        Me.tbCambio.TabIndex = 68
        Me.tbCambio.Value = 5.25R
        Me.tbCambio.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX17
        '
        Me.LabelX17.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX17.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX17.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX17.ForeColor = System.Drawing.Color.White
        Me.LabelX17.Location = New System.Drawing.Point(10, 81)
        Me.LabelX17.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX17.Name = "LabelX17"
        Me.LabelX17.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX17.Size = New System.Drawing.Size(113, 22)
        Me.LabelX17.TabIndex = 67
        Me.LabelX17.Text = "Cambio:"
        '
        'tbTotalPagado
        '
        '
        '
        '
        Me.tbTotalPagado.BackgroundStyle.BackColor = System.Drawing.Color.LawnGreen
        Me.tbTotalPagado.BackgroundStyle.BackColor2 = System.Drawing.Color.Green
        Me.tbTotalPagado.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbTotalPagado.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbTotalPagado.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbTotalPagado.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTotalPagado.ForeColor = System.Drawing.Color.White
        Me.tbTotalPagado.Increment = 1.0R
        Me.tbTotalPagado.IsInputReadOnly = True
        Me.tbTotalPagado.Location = New System.Drawing.Point(130, 37)
        Me.tbTotalPagado.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTotalPagado.MinValue = 0R
        Me.tbTotalPagado.Name = "tbTotalPagado"
        Me.tbTotalPagado.Size = New System.Drawing.Size(160, 38)
        Me.tbTotalPagado.TabIndex = 52
        Me.tbTotalPagado.Value = 1520.0R
        Me.tbTotalPagado.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX1
        '
        Me.LabelX1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.ForeColor = System.Drawing.Color.White
        Me.LabelX1.Location = New System.Drawing.Point(10, 41)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(113, 22)
        Me.LabelX1.TabIndex = 64
        Me.LabelX1.Text = "Total Pagado:"
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.GroupBox3.Controls.Add(Me.chkMontoBs)
        Me.GroupBox3.Controls.Add(Me.lbDolar)
        Me.GroupBox3.Controls.Add(Me.chkTransferencia)
        Me.GroupBox3.Controls.Add(Me.chkTarjeta)
        Me.GroupBox3.Controls.Add(Me.tbTransferencia)
        Me.GroupBox3.Controls.Add(Me.tbTarjeta)
        Me.GroupBox3.Controls.Add(Me.tbMontoBs)
        Me.GroupBox3.Controls.Add(Me.tbMontoDolar)
        Me.GroupBox3.Controls.Add(Me.LabelX13)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox3.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.Cornsilk
        Me.GroupBox3.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox3.Size = New System.Drawing.Size(528, 237)
        Me.GroupBox3.TabIndex = 1
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Efectivo Recibido:"
        '
        'chkMontoBs
        '
        Me.chkMontoBs.AutoSize = True
        '
        '
        '
        Me.chkMontoBs.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkMontoBs.BackgroundStyle.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.MenuBarBackground2
        Me.chkMontoBs.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkMontoBs.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkMontoBs.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkMontoBs.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.chkMontoBs.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkMontoBs.Location = New System.Drawing.Point(7, 36)
        Me.chkMontoBs.Name = "chkMontoBs"
        Me.chkMontoBs.Size = New System.Drawing.Size(91, 23)
        Me.chkMontoBs.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.chkMontoBs.TabIndex = 74
        Me.chkMontoBs.Text = "Monto Bs"
        Me.chkMontoBs.TextColor = System.Drawing.Color.White
        '
        'lbDolar
        '
        Me.lbDolar.AutoSize = True
        Me.lbDolar.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbDolar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbDolar.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbDolar.ForeColor = System.Drawing.Color.White
        Me.lbDolar.Location = New System.Drawing.Point(410, 67)
        Me.lbDolar.Margin = New System.Windows.Forms.Padding(4)
        Me.lbDolar.Name = "lbDolar"
        Me.lbDolar.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbDolar.Size = New System.Drawing.Size(31, 23)
        Me.lbDolar.TabIndex = 73
        Me.lbDolar.Text = "0 Bs"
        '
        'chkTransferencia
        '
        Me.chkTransferencia.AutoSize = True
        '
        '
        '
        Me.chkTransferencia.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkTransferencia.BackgroundStyle.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.MenuBarBackground2
        Me.chkTransferencia.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkTransferencia.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkTransferencia.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkTransferencia.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.chkTransferencia.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkTransferencia.Location = New System.Drawing.Point(209, 102)
        Me.chkTransferencia.Name = "chkTransferencia"
        Me.chkTransferencia.Size = New System.Drawing.Size(188, 23)
        Me.chkTransferencia.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.chkTransferencia.TabIndex = 72
        Me.chkTransferencia.Text = "Transferencia Bancaria:"
        Me.chkTransferencia.TextColor = System.Drawing.Color.White
        '
        'chkTarjeta
        '
        Me.chkTarjeta.AutoSize = True
        '
        '
        '
        Me.chkTarjeta.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkTarjeta.BackgroundStyle.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.MenuBarBackground2
        Me.chkTarjeta.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkTarjeta.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkTarjeta.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.DashDot
        Me.chkTarjeta.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.chkTarjeta.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkTarjeta.Location = New System.Drawing.Point(9, 102)
        Me.chkTarjeta.Name = "chkTarjeta"
        Me.chkTarjeta.Size = New System.Drawing.Size(73, 23)
        Me.chkTarjeta.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.chkTarjeta.TabIndex = 71
        Me.chkTarjeta.Text = "Tarjeta"
        Me.chkTarjeta.TextColor = System.Drawing.Color.White
        '
        'tbTransferencia
        '
        '
        '
        '
        Me.tbTransferencia.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbTransferencia.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbTransferencia.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbTransferencia.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTransferencia.ForeColor = System.Drawing.Color.Black
        Me.tbTransferencia.Increment = 1.0R
        Me.tbTransferencia.Location = New System.Drawing.Point(209, 132)
        Me.tbTransferencia.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTransferencia.MinValue = 0R
        Me.tbTransferencia.Name = "tbTransferencia"
        Me.tbTransferencia.Size = New System.Drawing.Size(193, 27)
        Me.tbTransferencia.TabIndex = 4
        Me.tbTransferencia.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'tbTarjeta
        '
        '
        '
        '
        Me.tbTarjeta.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbTarjeta.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbTarjeta.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbTarjeta.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTarjeta.ForeColor = System.Drawing.Color.Black
        Me.tbTarjeta.Increment = 1.0R
        Me.tbTarjeta.Location = New System.Drawing.Point(9, 132)
        Me.tbTarjeta.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTarjeta.MinValue = 0R
        Me.tbTarjeta.Name = "tbTarjeta"
        Me.tbTarjeta.Size = New System.Drawing.Size(180, 27)
        Me.tbTarjeta.TabIndex = 3
        Me.tbTarjeta.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'tbMontoBs
        '
        '
        '
        '
        Me.tbMontoBs.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbMontoBs.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbMontoBs.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbMontoBs.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMontoBs.ForeColor = System.Drawing.Color.Black
        Me.tbMontoBs.Increment = 1.0R
        Me.tbMontoBs.Location = New System.Drawing.Point(9, 67)
        Me.tbMontoBs.Margin = New System.Windows.Forms.Padding(4)
        Me.tbMontoBs.MinValue = 0R
        Me.tbMontoBs.Name = "tbMontoBs"
        Me.tbMontoBs.Size = New System.Drawing.Size(180, 27)
        Me.tbMontoBs.TabIndex = 1
        Me.tbMontoBs.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'tbMontoDolar
        '
        '
        '
        '
        Me.tbMontoDolar.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbMontoDolar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbMontoDolar.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbMontoDolar.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMontoDolar.ForeColor = System.Drawing.Color.Black
        Me.tbMontoDolar.Increment = 1.0R
        Me.tbMontoDolar.Location = New System.Drawing.Point(209, 67)
        Me.tbMontoDolar.Margin = New System.Windows.Forms.Padding(4)
        Me.tbMontoDolar.MinValue = 0R
        Me.tbMontoDolar.Name = "tbMontoDolar"
        Me.tbMontoDolar.Size = New System.Drawing.Size(193, 27)
        Me.tbMontoDolar.TabIndex = 2
        Me.tbMontoDolar.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX13
        '
        Me.LabelX13.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX13.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX13.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX13.ForeColor = System.Drawing.Color.White
        Me.LabelX13.Location = New System.Drawing.Point(209, 37)
        Me.LabelX13.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX13.Name = "LabelX13"
        Me.LabelX13.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX13.Size = New System.Drawing.Size(93, 22)
        Me.LabelX13.TabIndex = 66
        Me.LabelX13.Text = "Monto $u$:"
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.DarkSlateGray
        Me.GroupBox2.Controls.Add(Me.tbTotalVenta)
        Me.GroupBox2.Controls.Add(Me.LabelX18)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox2.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.ForeColor = System.Drawing.Color.Cornsilk
        Me.GroupBox2.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox2.Size = New System.Drawing.Size(1002, 80)
        Me.GroupBox2.TabIndex = 50
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Total Venta A Pagar:"
        '
        'tbTotalVenta
        '
        '
        '
        '
        Me.tbTotalVenta.BackgroundStyle.BackColor = System.Drawing.Color.LawnGreen
        Me.tbTotalVenta.BackgroundStyle.BackColor2 = System.Drawing.Color.LawnGreen
        Me.tbTotalVenta.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbTotalVenta.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbTotalVenta.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbTotalVenta.Font = New System.Drawing.Font("Arial Black", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTotalVenta.ForeColor = System.Drawing.Color.Black
        Me.tbTotalVenta.Increment = 1.0R
        Me.tbTotalVenta.IsInputReadOnly = True
        Me.tbTotalVenta.Location = New System.Drawing.Point(350, 19)
        Me.tbTotalVenta.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTotalVenta.MinValue = 0R
        Me.tbTotalVenta.Name = "tbTotalVenta"
        Me.tbTotalVenta.Size = New System.Drawing.Size(246, 46)
        Me.tbTotalVenta.TabIndex = 65
        Me.tbTotalVenta.Value = 1520.0R
        Me.tbTotalVenta.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX18
        '
        Me.LabelX18.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX18.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX18.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX18.ForeColor = System.Drawing.Color.White
        Me.LabelX18.Location = New System.Drawing.Point(193, 26)
        Me.LabelX18.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX18.Name = "LabelX18"
        Me.LabelX18.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX18.Size = New System.Drawing.Size(149, 39)
        Me.LabelX18.TabIndex = 64
        Me.LabelX18.Text = "Total Venta:"
        '
        'Highlighter2
        '
        Me.Highlighter2.ContainerControl = Me
        Me.Highlighter2.CustomHighlightColors = New System.Drawing.Color() {System.Drawing.Color.Cyan}
        Me.Highlighter2.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Orange
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.BackColor = System.Drawing.Color.Cyan
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX1.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.iconguardar
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.ButtonX1.Location = New System.Drawing.Point(147, 142)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(129, 54)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX1.Symbol = ""
        Me.ButtonX1.TabIndex = 69
        Me.ButtonX1.Text = "Grabar" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Imprimir"
        Me.ButtonX1.TextColor = System.Drawing.Color.Black
        '
        'EfectivoCobranza
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1002, 353)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel14)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "EfectivoCobranza"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "EfectivoCobranza"
        Me.Panel14.ResumeLayout(False)
        Me.Panel15.ResumeLayout(False)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel20.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.tbCambio, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbTotalPagado, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.tbTransferencia, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbTarjeta, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbMontoBs, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbMontoDolar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        CType(Me.tbTotalVenta, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel14 As Panel
    Friend WithEvents Panel15 As Panel
    Friend WithEvents lbTitulo As Label
    Friend WithEvents Panel16 As Panel
    Friend WithEvents PictureBox4 As PictureBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents LabelX18 As DevComponents.DotNetBar.LabelX
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Panel20 As Panel
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents tbCambio As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX17 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbTotalPagado As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents tbTransferencia As DevComponents.Editors.DoubleInput
    Friend WithEvents tbTarjeta As DevComponents.Editors.DoubleInput
    Friend WithEvents tbMontoBs As DevComponents.Editors.DoubleInput
    Friend WithEvents tbMontoDolar As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX13 As DevComponents.DotNetBar.LabelX
    Friend WithEvents chkTransferencia As DevComponents.DotNetBar.Controls.CheckBoxX
    Friend WithEvents chkTarjeta As DevComponents.DotNetBar.Controls.CheckBoxX
    Protected WithEvents btnGuardar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnSalir As DevComponents.DotNetBar.ButtonX
    Public WithEvents Highlighter2 As DevComponents.DotNetBar.Validator.Highlighter
    Friend WithEvents tbTotalVenta As DevComponents.Editors.DoubleInput
    Friend WithEvents lbDolar As DevComponents.DotNetBar.LabelX
    Friend WithEvents chkMontoBs As DevComponents.DotNetBar.Controls.CheckBoxX
    Protected WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
End Class
