<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AyudaCantidadProductosVentas
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
        Me.txtStock = New System.Windows.Forms.Label()
        Me.btnNo = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnSi = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtProducto = New System.Windows.Forms.Label()
        Me.txtTitulo = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.MHighlighterFocus = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.tbCajas = New DevComponents.Editors.DoubleInput()
        Me.tbCantidad = New DevComponents.Editors.DoubleInput()
        Me.LabelX8 = New DevComponents.DotNetBar.LabelX()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.grPrecios = New Janus.Windows.GridEX.GridEX()
        Me.btnNo.SuspendLayout()
        Me.btnSi.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbCajas, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbCantidad, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel1.SuspendLayout()
        CType(Me.grPrecios, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtStock
        '
        Me.txtStock.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtStock.ForeColor = System.Drawing.Color.DarkCyan
        Me.txtStock.Location = New System.Drawing.Point(81, 200)
        Me.txtStock.Name = "txtStock"
        Me.txtStock.Size = New System.Drawing.Size(447, 69)
        Me.txtStock.TabIndex = 17
        Me.txtStock.Text = "Cantidad Unitario disp. = 10"
        Me.txtStock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnNo
        '
        Me.btnNo.BackColor = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.btnNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.btnNo.Controls.Add(Me.Label4)
        Me.btnNo.Location = New System.Drawing.Point(80, 405)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(208, 50)
        Me.btnNo.TabIndex = 3
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Calibri", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(81, 9)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(42, 29)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "No"
        '
        'btnSi
        '
        Me.btnSi.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(179, Byte), Integer), CType(CType(148, Byte), Integer))
        Me.btnSi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.btnSi.Controls.Add(Me.Label3)
        Me.btnSi.Location = New System.Drawing.Point(312, 405)
        Me.btnSi.Name = "btnSi"
        Me.btnSi.Size = New System.Drawing.Size(208, 50)
        Me.btnSi.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Calibri", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(84, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(30, 29)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Si"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtProducto
        '
        Me.txtProducto.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtProducto.ForeColor = System.Drawing.Color.FromArgb(CType(CType(84, Byte), Integer), CType(CType(84, Byte), Integer), CType(CType(84, Byte), Integer))
        Me.txtProducto.Location = New System.Drawing.Point(81, 176)
        Me.txtProducto.Name = "txtProducto"
        Me.txtProducto.Size = New System.Drawing.Size(447, 24)
        Me.txtProducto.TabIndex = 15
        Me.txtProducto.Text = "Paracetamol"
        Me.txtProducto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtTitulo
        '
        Me.txtTitulo.AutoSize = True
        Me.txtTitulo.Font = New System.Drawing.Font("Calibri", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtTitulo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer))
        Me.txtTitulo.Location = New System.Drawing.Point(76, 123)
        Me.txtTitulo.Name = "txtTitulo"
        Me.txtTitulo.Size = New System.Drawing.Size(462, 49)
        Me.txtTitulo.TabIndex = 14
        Me.txtTitulo.Text = "Ingrese Cantidad al Carrito"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.TeVendo.My.Resources.Resources.compra
        Me.PictureBox1.Location = New System.Drawing.Point(246, 44)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(80, 76)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 16
        Me.PictureBox1.TabStop = False
        '
        'MHighlighterFocus
        '
        Me.MHighlighterFocus.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Orange
        '
        'tbCajas
        '
        '
        '
        '
        Me.tbCajas.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbCajas.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbCajas.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbCajas.Font = New System.Drawing.Font("Calibri", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCajas.Increment = 1.0R
        Me.tbCajas.Location = New System.Drawing.Point(312, 323)
        Me.tbCajas.Name = "tbCajas"
        Me.tbCajas.Size = New System.Drawing.Size(208, 56)
        Me.tbCajas.TabIndex = 2
        '
        'tbCantidad
        '
        Me.tbCantidad.AutoOverwrite = True
        '
        '
        '
        Me.tbCantidad.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbCantidad.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbCantidad.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbCantidad.Font = New System.Drawing.Font("Calibri", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCantidad.Increment = 1.0R
        Me.tbCantidad.Location = New System.Drawing.Point(80, 323)
        Me.tbCantidad.Name = "tbCantidad"
        Me.tbCantidad.Size = New System.Drawing.Size(208, 56)
        Me.tbCantidad.TabIndex = 1
        '
        'LabelX8
        '
        Me.LabelX8.AutoSize = True
        Me.LabelX8.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX8.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX8.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX8.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX8.Location = New System.Drawing.Point(312, 293)
        Me.LabelX8.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX8.Name = "LabelX8"
        Me.LabelX8.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX8.Size = New System.Drawing.Size(106, 23)
        Me.LabelX8.TabIndex = 216
        Me.LabelX8.Text = "Cantidad Cajas"
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
        Me.LabelX1.Location = New System.Drawing.Point(80, 293)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(128, 23)
        Me.LabelX1.TabIndex = 217
        Me.LabelX1.Text = "Cantidad Unitario"
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.grPrecios)
        Me.GroupPanel1.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel1.Dock = System.Windows.Forms.DockStyle.Right
        Me.GroupPanel1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel1.Location = New System.Drawing.Point(611, 0)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(485, 541)
        '
        '
        '
        Me.GroupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemDesignTimeBorder
        Me.GroupPanel1.Style.BackColorGradientAngle = 90
        Me.GroupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.CustomizeText
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
        Me.GroupPanel1.TabIndex = 218
        Me.GroupPanel1.Text = "Seleccione Precio"
        '
        'grPrecios
        '
        Me.grPrecios.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grPrecios.BackColor = System.Drawing.Color.WhiteSmoke
        Me.grPrecios.ColumnAutoResize = True
        Me.grPrecios.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grPrecios.FlatBorderColor = System.Drawing.Color.DodgerBlue
        Me.grPrecios.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grPrecios.FocusCellFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grPrecios.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPrecios.GridLineColor = System.Drawing.Color.DodgerBlue
        Me.grPrecios.HeaderFormatStyle.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPrecios.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grPrecios.Hierarchical = True
        Me.grPrecios.Location = New System.Drawing.Point(0, 0)
        Me.grPrecios.Margin = New System.Windows.Forms.Padding(4)
        Me.grPrecios.Name = "grPrecios"
        Me.grPrecios.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grPrecios.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grPrecios.RowFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.grPrecios.SelectedFormatStyle.BackColor = System.Drawing.Color.Gold
        Me.grPrecios.SelectedFormatStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPrecios.SelectedFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grPrecios.SelectedInactiveFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.grPrecios.Size = New System.Drawing.Size(479, 512)
        Me.grPrecios.TabIndex = 3
        Me.grPrecios.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'AyudaCantidadProductosVentas
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1096, 541)
        Me.Controls.Add(Me.GroupPanel1)
        Me.Controls.Add(Me.LabelX1)
        Me.Controls.Add(Me.LabelX8)
        Me.Controls.Add(Me.tbCantidad)
        Me.Controls.Add(Me.tbCajas)
        Me.Controls.Add(Me.txtStock)
        Me.Controls.Add(Me.btnNo)
        Me.Controls.Add(Me.btnSi)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.txtProducto)
        Me.Controls.Add(Me.txtTitulo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "AyudaCantidadProductosVentas"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AyudaCantidadProductosVentas"
        Me.btnNo.ResumeLayout(False)
        Me.btnNo.PerformLayout()
        Me.btnSi.ResumeLayout(False)
        Me.btnSi.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbCajas, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbCantidad, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel1.ResumeLayout(False)
        CType(Me.grPrecios, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtStock As Label
    Friend WithEvents btnNo As Panel
    Friend WithEvents Label4 As Label
    Friend WithEvents btnSi As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents txtProducto As Label
    Friend WithEvents txtTitulo As Label
    Protected WithEvents MHighlighterFocus As DevComponents.DotNetBar.Validator.Highlighter
    Friend WithEvents tbCajas As DevComponents.Editors.DoubleInput
    Friend WithEvents tbCantidad As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX8 As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents grPrecios As Janus.Windows.GridEX.GridEX
End Class
