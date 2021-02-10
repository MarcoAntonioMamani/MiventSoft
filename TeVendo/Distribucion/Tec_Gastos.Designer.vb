<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_Gastos
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
        Dim cbConcepto_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Tec_Gastos))
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.lbNombreProductos = New DevComponents.DotNetBar.LabelX()
        Me.tbdescripcion = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.btnGuardar = New DevComponents.DotNetBar.ButtonX()
        Me.btnSalir = New DevComponents.DotNetBar.ButtonX()
        Me.cbConcepto = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.tbMonto = New DevComponents.Editors.DoubleInput()
        Me.btnTipoContrato = New DevComponents.DotNetBar.ButtonX()
        Me.MHighlighterFocus = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cbConcepto, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbMonto, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel10
        '
        Me.Panel10.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Panel10.Controls.Add(Me.Panel11)
        Me.Panel10.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel10.Location = New System.Drawing.Point(0, 0)
        Me.Panel10.Name = "Panel10"
        Me.Panel10.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel10.Size = New System.Drawing.Size(770, 27)
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
        Me.Panel11.Name = "Panel11"
        Me.Panel11.Size = New System.Drawing.Size(768, 25)
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
        Me.Label3.Text = "Datos Del Gasto"
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
        Me.PictureBox3.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox3.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox3.Size = New System.Drawing.Size(58, 25)
        Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox3.TabIndex = 0
        Me.PictureBox3.TabStop = False
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
        Me.lbNombreProductos.Location = New System.Drawing.Point(75, 110)
        Me.lbNombreProductos.Margin = New System.Windows.Forms.Padding(4)
        Me.lbNombreProductos.Name = "lbNombreProductos"
        Me.lbNombreProductos.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbNombreProductos.Size = New System.Drawing.Size(130, 23)
        Me.lbNombreProductos.TabIndex = 62
        Me.lbNombreProductos.Text = "Detalle Del Gasto:"
        '
        'tbdescripcion
        '
        '
        '
        '
        Me.tbdescripcion.Border.BackColor = System.Drawing.Color.MidnightBlue
        Me.tbdescripcion.Border.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbdescripcion.Border.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbdescripcion.Border.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbdescripcion.Border.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbdescripcion.Border.Class = "TextBoxBorder"
        Me.tbdescripcion.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbdescripcion.FocusHighlightEnabled = True
        Me.tbdescripcion.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbdescripcion.Location = New System.Drawing.Point(234, 105)
        Me.tbdescripcion.Margin = New System.Windows.Forms.Padding(4)
        Me.tbdescripcion.MaxLength = 200
        Me.tbdescripcion.Multiline = True
        Me.tbdescripcion.Name = "tbdescripcion"
        Me.tbdescripcion.PreventEnterBeep = True
        Me.tbdescripcion.Size = New System.Drawing.Size(360, 95)
        Me.tbdescripcion.TabIndex = 2
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
        Me.LabelX1.Location = New System.Drawing.Point(59, 58)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(146, 23)
        Me.LabelX1.TabIndex = 63
        Me.LabelX1.Text = "Concepto Del Gasto:"
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
        Me.LabelX2.Location = New System.Drawing.Point(150, 213)
        Me.LabelX2.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX2.Size = New System.Drawing.Size(55, 23)
        Me.LabelX2.TabIndex = 64
        Me.LabelX2.Text = "Monto:"
        '
        'btnGuardar
        '
        Me.btnGuardar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnGuardar.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.btnGuardar.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnGuardar.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGuardar.Image = Global.TeVendo.My.Resources.Resources.iconguardar
        Me.btnGuardar.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnGuardar.Location = New System.Drawing.Point(233, 268)
        Me.btnGuardar.Margin = New System.Windows.Forms.Padding(4)
        Me.btnGuardar.Name = "btnGuardar"
        Me.btnGuardar.Size = New System.Drawing.Size(129, 54)
        Me.btnGuardar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnGuardar.Symbol = "59500"
        Me.btnGuardar.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnGuardar.TabIndex = 4
        Me.btnGuardar.Text = "Guardar"
        Me.btnGuardar.TextColor = System.Drawing.Color.White
        '
        'btnSalir
        '
        Me.btnSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSalir.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(83, Byte), Integer), CType(CType(79, Byte), Integer))
        Me.btnSalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnSalir.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSalir.Image = Global.TeVendo.My.Resources.Resources.atras
        Me.btnSalir.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.btnSalir.Location = New System.Drawing.Point(369, 268)
        Me.btnSalir.Name = "btnSalir"
        Me.btnSalir.Size = New System.Drawing.Size(129, 54)
        Me.btnSalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.btnSalir.Symbol = ""
        Me.btnSalir.TabIndex = 5
        Me.btnSalir.Text = "Salir"
        Me.btnSalir.TextColor = System.Drawing.Color.White
        '
        'cbConcepto
        '
        Me.cbConcepto.BackColor = System.Drawing.Color.Azure
        Me.cbConcepto.ColorScheme = ""
        Me.cbConcepto.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbConcepto.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbConcepto.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        cbConcepto_DesignTimeLayout.LayoutString = resources.GetString("cbConcepto_DesignTimeLayout.LayoutString")
        Me.cbConcepto.DesignTimeLayout = cbConcepto_DesignTimeLayout
        Me.cbConcepto.FlatBorderColor = System.Drawing.Color.Black
        Me.cbConcepto.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbConcepto.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbConcepto.HideSelection = False
        Me.cbConcepto.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbConcepto.Location = New System.Drawing.Point(234, 58)
        Me.cbConcepto.Margin = New System.Windows.Forms.Padding(4)
        Me.cbConcepto.Name = "cbConcepto"
        Me.cbConcepto.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbConcepto.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbConcepto.SelectedIndex = -1
        Me.cbConcepto.SelectedItem = Nothing
        Me.cbConcepto.Size = New System.Drawing.Size(360, 29)
        Me.cbConcepto.TabIndex = 1
        Me.cbConcepto.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'tbMonto
        '
        '
        '
        '
        Me.tbMonto.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbMonto.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbMonto.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbMonto.Font = New System.Drawing.Font("Microsoft Sans Serif", 19.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMonto.Increment = 1.0R
        Me.tbMonto.Location = New System.Drawing.Point(234, 207)
        Me.tbMonto.Name = "tbMonto"
        Me.tbMonto.Size = New System.Drawing.Size(210, 45)
        Me.tbMonto.TabIndex = 3
        '
        'btnTipoContrato
        '
        Me.btnTipoContrato.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnTipoContrato.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnTipoContrato.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTipoContrato.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.btnTipoContrato.Location = New System.Drawing.Point(602, 34)
        Me.btnTipoContrato.Margin = New System.Windows.Forms.Padding(4)
        Me.btnTipoContrato.Name = "btnTipoContrato"
        Me.btnTipoContrato.Size = New System.Drawing.Size(120, 67)
        Me.btnTipoContrato.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnTipoContrato.Symbol = ""
        Me.btnTipoContrato.SymbolColor = System.Drawing.Color.MediumTurquoise
        Me.btnTipoContrato.TabIndex = 223
        Me.btnTipoContrato.Text = "Nuevo Concepto"
        '
        'MHighlighterFocus
        '
        Me.MHighlighterFocus.ContainerControl = Me
        Me.MHighlighterFocus.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Orange
        '
        'Tec_Gastos
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.ClientSize = New System.Drawing.Size(770, 398)
        Me.Controls.Add(Me.btnTipoContrato)
        Me.Controls.Add(Me.tbMonto)
        Me.Controls.Add(Me.cbConcepto)
        Me.Controls.Add(Me.btnSalir)
        Me.Controls.Add(Me.btnGuardar)
        Me.Controls.Add(Me.LabelX2)
        Me.Controls.Add(Me.LabelX1)
        Me.Controls.Add(Me.lbNombreProductos)
        Me.Controls.Add(Me.tbdescripcion)
        Me.Controls.Add(Me.Panel10)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "Tec_Gastos"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Tec_Gastos"
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cbConcepto, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbMonto, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel10 As Panel
    Friend WithEvents Panel11 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel12 As Panel
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents lbNombreProductos As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbdescripcion As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Protected WithEvents btnGuardar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnSalir As DevComponents.DotNetBar.ButtonX
    Friend WithEvents cbConcepto As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents tbMonto As DevComponents.Editors.DoubleInput
    Friend WithEvents btnTipoContrato As DevComponents.DotNetBar.ButtonX
    Protected WithEvents MHighlighterFocus As DevComponents.DotNetBar.Validator.Highlighter
End Class
