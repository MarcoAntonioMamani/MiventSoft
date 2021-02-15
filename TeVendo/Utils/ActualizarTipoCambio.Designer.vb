<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ActualizarTipoCambio
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
        Me.btnGuardar = New DevComponents.DotNetBar.ButtonX()
        Me.btnSalir = New DevComponents.DotNetBar.ButtonX()
        Me.lbNombreProductos = New DevComponents.DotNetBar.LabelX()
        Me.tbTipoCambio = New DevComponents.Editors.DoubleInput()
        Me.Highlighter2 = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.Panel14.SuspendLayout()
        Me.Panel15.SuspendLayout()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbTipoCambio, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.Panel14.Size = New System.Drawing.Size(613, 36)
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
        Me.Panel15.Size = New System.Drawing.Size(611, 34)
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
        Me.lbTitulo.Size = New System.Drawing.Size(552, 34)
        Me.lbTitulo.TabIndex = 10
        Me.lbTitulo.Text = "Actualizar Tipo De Cambio"
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
        'btnGuardar
        '
        Me.btnGuardar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnGuardar.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.btnGuardar.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnGuardar.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGuardar.Image = Global.TeVendo.My.Resources.Resources.iconguardar
        Me.btnGuardar.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnGuardar.Location = New System.Drawing.Point(183, 120)
        Me.btnGuardar.Margin = New System.Windows.Forms.Padding(4)
        Me.btnGuardar.Name = "btnGuardar"
        Me.btnGuardar.Size = New System.Drawing.Size(129, 54)
        Me.btnGuardar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnGuardar.Symbol = "59500"
        Me.btnGuardar.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnGuardar.TabIndex = 2
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
        Me.btnSalir.Location = New System.Drawing.Point(319, 120)
        Me.btnSalir.Name = "btnSalir"
        Me.btnSalir.Size = New System.Drawing.Size(129, 54)
        Me.btnSalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.btnSalir.Symbol = ""
        Me.btnSalir.TabIndex = 3
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
        Me.lbNombreProductos.Location = New System.Drawing.Point(80, 76)
        Me.lbNombreProductos.Margin = New System.Windows.Forms.Padding(4)
        Me.lbNombreProductos.Name = "lbNombreProductos"
        Me.lbNombreProductos.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbNombreProductos.Size = New System.Drawing.Size(96, 23)
        Me.lbNombreProductos.TabIndex = 64
        Me.lbNombreProductos.Text = "Tipo Cambio:"
        '
        'tbTipoCambio
        '
        '
        '
        '
        Me.tbTipoCambio.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbTipoCambio.BackgroundStyle.BorderColor = System.Drawing.Color.DarkBlue
        Me.tbTipoCambio.BackgroundStyle.BorderColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemPressedBorder
        Me.tbTipoCambio.BackgroundStyle.BorderColorLightSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarCaptionInactiveText
        Me.tbTipoCambio.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbTipoCambio.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbTipoCambio.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbTipoCambio.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbTipoCambio.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbTipoCambio.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbTipoCambio.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTipoCambio.Increment = 1.0R
        Me.tbTipoCambio.Location = New System.Drawing.Point(183, 65)
        Me.tbTipoCambio.Name = "tbTipoCambio"
        Me.tbTipoCambio.Size = New System.Drawing.Size(266, 34)
        Me.tbTipoCambio.TabIndex = 1
        '
        'Highlighter2
        '
        Me.Highlighter2.ContainerControl = Me
        Me.Highlighter2.CustomHighlightColors = New System.Drawing.Color() {System.Drawing.Color.Cyan}
        Me.Highlighter2.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Orange
        '
        'ActualizarTipoCambio
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(613, 223)
        Me.Controls.Add(Me.tbTipoCambio)
        Me.Controls.Add(Me.btnGuardar)
        Me.Controls.Add(Me.btnSalir)
        Me.Controls.Add(Me.lbNombreProductos)
        Me.Controls.Add(Me.Panel14)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "ActualizarTipoCambio"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ActualizarTipoCambio"
        Me.Panel14.ResumeLayout(False)
        Me.Panel15.ResumeLayout(False)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbTipoCambio, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel14 As Panel
    Friend WithEvents Panel15 As Panel
    Friend WithEvents lbTitulo As Label
    Friend WithEvents Panel16 As Panel
    Friend WithEvents PictureBox4 As PictureBox
    Protected WithEvents btnGuardar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnSalir As DevComponents.DotNetBar.ButtonX
    Friend WithEvents lbNombreProductos As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbTipoCambio As DevComponents.Editors.DoubleInput
    Public WithEvents Highlighter2 As DevComponents.DotNetBar.Validator.Highlighter
End Class
