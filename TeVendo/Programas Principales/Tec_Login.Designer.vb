<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_Login
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Tec_Login))
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.tbpassword = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.btnsalir = New DevComponents.DotNetBar.ButtonX()
        Me.btnIngresar = New DevComponents.DotNetBar.ButtonX()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.tbUsuario = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Highlighter1 = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Panel3.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.White
        Me.Panel3.Controls.Add(Me.Panel1)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Padding = New System.Windows.Forms.Padding(2)
        Me.Panel3.Size = New System.Drawing.Size(937, 385)
        Me.Panel3.TabIndex = 33
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.Panel1.Controls.Add(Me.tbpassword)
        Me.Panel1.Controls.Add(Me.btnsalir)
        Me.Panel1.Controls.Add(Me.btnIngresar)
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Controls.Add(Me.tbUsuario)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(2, 2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(933, 381)
        Me.Panel1.TabIndex = 0
        '
        'tbpassword
        '
        '
        '
        '
        Me.tbpassword.Border.BorderColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.tbpassword.Border.BorderColor2 = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.tbpassword.Border.Class = "TextBoxBorder"
        Me.tbpassword.Border.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbpassword.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.tbpassword.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbpassword.Location = New System.Drawing.Point(457, 192)
        Me.tbpassword.Margin = New System.Windows.Forms.Padding(4)
        Me.tbpassword.Name = "tbpassword"
        Me.tbpassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbpassword.PreventEnterBeep = True
        Me.tbpassword.Size = New System.Drawing.Size(385, 37)
        Me.tbpassword.TabIndex = 1
        Me.tbpassword.WatermarkBehavior = DevComponents.DotNetBar.eWatermarkBehavior.HideNonEmpty
        Me.tbpassword.WatermarkColor = System.Drawing.Color.DarkGray
        Me.tbpassword.WatermarkText = "Contraseña"
        '
        'btnsalir
        '
        Me.btnsalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnsalir.BackColor = System.Drawing.Color.Transparent
        Me.btnsalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.btnsalir.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnsalir.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.Color
        Me.btnsalir.Location = New System.Drawing.Point(677, 275)
        Me.btnsalir.Name = "btnsalir"
        Me.btnsalir.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(3)
        Me.btnsalir.Size = New System.Drawing.Size(165, 38)
        Me.btnsalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnsalir.TabIndex = 32
        Me.btnsalir.Text = "Salir"
        Me.btnsalir.TextColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        '
        'btnIngresar
        '
        Me.btnIngresar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnIngresar.BackColor = System.Drawing.Color.Transparent
        Me.btnIngresar.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.btnIngresar.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnIngresar.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.Color
        Me.btnIngresar.Location = New System.Drawing.Point(457, 275)
        Me.btnIngresar.Name = "btnIngresar"
        Me.btnIngresar.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(3)
        Me.btnIngresar.Size = New System.Drawing.Size(165, 38)
        Me.btnIngresar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnIngresar.TabIndex = 2
        Me.btnIngresar.Text = "Ingresar"
        Me.btnIngresar.TextColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(67, 41)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Padding = New System.Windows.Forms.Padding(0, 35, 0, 35)
        Me.PictureBox1.Size = New System.Drawing.Size(294, 272)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 22
        Me.PictureBox1.TabStop = False
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.White
        Me.Panel2.Location = New System.Drawing.Point(367, 41)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(3, 272)
        Me.Panel2.TabIndex = 21
        '
        'tbUsuario
        '
        Me.tbUsuario.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        '
        '
        '
        Me.tbUsuario.Border.BorderColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.tbUsuario.Border.BorderColor2 = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.tbUsuario.Border.Class = "TextBoxBorder"
        Me.tbUsuario.Border.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbUsuario.Border.CornerTypeBottomLeft = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbUsuario.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.tbUsuario.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbUsuario.Location = New System.Drawing.Point(457, 130)
        Me.tbUsuario.Margin = New System.Windows.Forms.Padding(4)
        Me.tbUsuario.Name = "tbUsuario"
        Me.tbUsuario.PreventEnterBeep = True
        Me.tbUsuario.Size = New System.Drawing.Size(385, 37)
        Me.tbUsuario.TabIndex = 0
        Me.tbUsuario.WatermarkBehavior = DevComponents.DotNetBar.eWatermarkBehavior.HideNonEmpty
        Me.tbUsuario.WatermarkColor = System.Drawing.Color.DarkGray
        Me.tbUsuario.WatermarkText = "Nombre de Usuario"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(433, 87)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(446, 21)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Por Favor Acceda Al sistema con sus credenciales autorizados :"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Calibri", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(420, 30)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(466, 49)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Autentificación De Usuario"
        '
        'Highlighter1
        '
        Me.Highlighter1.ContainerControl = Me
        Me.Highlighter1.CustomHighlightColors = New System.Drawing.Color() {System.Drawing.Color.DodgerBlue}
        Me.Highlighter1.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Orange
        '
        'Timer1
        '
        '
        'Tec_Login
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(937, 385)
        Me.Controls.Add(Me.Panel3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "Tec_Login"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Tec_Login"
        Me.Panel3.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents tbUsuario As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents btnIngresar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnsalir As DevComponents.DotNetBar.ButtonX
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Highlighter1 As DevComponents.DotNetBar.Validator.Highlighter
    Friend WithEvents tbpassword As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents Timer1 As Timer
End Class
