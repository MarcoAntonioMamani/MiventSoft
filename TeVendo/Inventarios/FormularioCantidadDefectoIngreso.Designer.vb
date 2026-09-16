<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormularioCantidadDefectoIngreso
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
        Me.btnNo = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnSi = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtTitulo = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lblUnidadMin = New System.Windows.Forms.Label()
        Me.tbCantidad = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.MHighlighterFocus = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.btnNo.SuspendLayout()
        Me.btnSi.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnNo
        '
        Me.btnNo.BackColor = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.btnNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.btnNo.Controls.Add(Me.Label4)
        Me.btnNo.Location = New System.Drawing.Point(116, 313)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(156, 50)
        Me.btnNo.TabIndex = 2
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Calibri", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(61, 9)
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
        Me.btnSi.Location = New System.Drawing.Point(330, 313)
        Me.btnSi.Name = "btnSi"
        Me.btnSi.Size = New System.Drawing.Size(156, 50)
        Me.btnSi.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Calibri", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(61, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(30, 29)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Si"
        '
        'txtTitulo
        '
        Me.txtTitulo.AutoSize = True
        Me.txtTitulo.Font = New System.Drawing.Font("Calibri", 20.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtTitulo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer))
        Me.txtTitulo.Location = New System.Drawing.Point(61, 131)
        Me.txtTitulo.Name = "txtTitulo"
        Me.txtTitulo.Size = New System.Drawing.Size(462, 41)
        Me.txtTitulo.TabIndex = 5
        Me.txtTitulo.Text = "Cantidad por Defecto"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.TeVendo.My.Resources.Resources.compra
        Me.PictureBox1.Location = New System.Drawing.Point(227, 33)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(80, 76)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 7
        Me.PictureBox1.TabStop = False
        '
        'lblUnidadMin
        '
        Me.lblUnidadMin.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUnidadMin.ForeColor = System.Drawing.Color.FromArgb(CType(CType(84, Byte), Integer), CType(CType(84, Byte), Integer), CType(CType(84, Byte), Integer))
        Me.lblUnidadMin.Location = New System.Drawing.Point(186, 190)
        Me.lblUnidadMin.Name = "lblUnidadMin"
        Me.lblUnidadMin.Size = New System.Drawing.Size(200, 20)
        Me.lblUnidadMin.TabIndex = 11
        Me.lblUnidadMin.Text = "CANTIDAD A INGRESAR"
        Me.lblUnidadMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'tbCantidad
        '
        '
        '
        '
        Me.tbCantidad.Border.BackColor = System.Drawing.Color.MediumVioletRed
        Me.tbCantidad.Border.BackColor2 = System.Drawing.Color.MediumTurquoise
        Me.tbCantidad.Border.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbCantidad.Border.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbCantidad.Border.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbCantidad.Border.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbCantidad.Border.Class = "TextBoxBorder"
        Me.tbCantidad.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbCantidad.Font = New System.Drawing.Font("Calibri", 25.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCantidad.ForeColor = System.Drawing.Color.Black
        Me.tbCantidad.Location = New System.Drawing.Point(186, 212)
        Me.tbCantidad.Margin = New System.Windows.Forms.Padding(4)
        Me.tbCantidad.Name = "tbCantidad"
        Me.tbCantidad.PreventEnterBeep = True
        Me.tbCantidad.Size = New System.Drawing.Size(200, 60)
        Me.tbCantidad.TabIndex = 0
        Me.tbCantidad.Text = "0"
        Me.tbCantidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'MHighlighterFocus
        '
        Me.MHighlighterFocus.ContainerControl = Me
        Me.MHighlighterFocus.FocusHighlightColor = DevComponents.DotNetBar.Validator.eHighlightColor.Orange
        '
        'FormularioCantidadDefectoIngreso
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(571, 400)
        Me.Controls.Add(Me.lblUnidadMin)
        Me.Controls.Add(Me.tbCantidad)
        Me.Controls.Add(Me.btnNo)
        Me.Controls.Add(Me.btnSi)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.txtTitulo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "FormularioCantidadDefectoIngreso"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FormularioCantidadDefectoIngreso"
        Me.btnNo.ResumeLayout(False)
        Me.btnNo.PerformLayout()
        Me.btnSi.ResumeLayout(False)
        Me.btnSi.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnNo As Panel
    Friend WithEvents Label4 As Label
    Friend WithEvents btnSi As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents txtTitulo As Label
    Friend WithEvents lblUnidadMin As Label
    Friend WithEvents tbCantidad As DevComponents.DotNetBar.Controls.TextBoxX
    Protected WithEvents MHighlighterFocus As DevComponents.DotNetBar.Validator.Highlighter
End Class
