<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class F_AsignacionZona
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
        Me.Paneltop = New System.Windows.Forms.Panel()
        Me.grPersonal = New Janus.Windows.GridEX.GridEX()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnConfirmarSalir = New DevComponents.DotNetBar.ButtonX()
        Me.grzonas = New Janus.Windows.GridEX.GridEX()
        Me.Paneltop.SuspendLayout()
        CType(Me.grPersonal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.grzonas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Paneltop
        '
        Me.Paneltop.BackColor = System.Drawing.Color.White
        Me.Paneltop.Controls.Add(Me.Panel1)
        Me.Paneltop.Controls.Add(Me.Panel10)
        Me.Paneltop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Paneltop.Location = New System.Drawing.Point(0, 0)
        Me.Paneltop.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Paneltop.Name = "Paneltop"
        Me.Paneltop.Size = New System.Drawing.Size(905, 558)
        Me.Paneltop.TabIndex = 3
        '
        'grPersonal
        '
        Me.grPersonal.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grPersonal.AlternatingColors = True
        Me.grPersonal.BackColor = System.Drawing.Color.White
        Me.grPersonal.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grPersonal.ColumnAutoResize = True
        Me.grPersonal.Dock = System.Windows.Forms.DockStyle.Left
        Me.grPersonal.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grPersonal.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grPersonal.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPersonal.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grPersonal.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grPersonal.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grPersonal.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPersonal.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grPersonal.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grPersonal.HeaderFormatStyle.Alpha = 0
        Me.grPersonal.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grPersonal.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grPersonal.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grPersonal.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grPersonal.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grPersonal.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPersonal.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grPersonal.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grPersonal.Location = New System.Drawing.Point(0, 0)
        Me.grPersonal.Margin = New System.Windows.Forms.Padding(4)
        Me.grPersonal.Name = "grPersonal"
        Me.grPersonal.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grPersonal.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grPersonal.RecordNavigator = True
        Me.grPersonal.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPersonal.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grPersonal.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grPersonal.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grPersonal.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grPersonal.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grPersonal.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPersonal.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grPersonal.SelectedFormatStyle.BackColor = System.Drawing.Color.Gold
        Me.grPersonal.SelectedFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grPersonal.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grPersonal.SelectedFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grPersonal.Size = New System.Drawing.Size(630, 491)
        Me.grPersonal.TabIndex = 4
        Me.grPersonal.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grPersonal.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grPersonal.TableSpacing = 9
        Me.grPersonal.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grPersonal.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grPersonal.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grPersonal.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
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
        Me.Panel10.Size = New System.Drawing.Size(905, 67)
        Me.Panel10.TabIndex = 2
        '
        'Panel11
        '
        Me.Panel11.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel11.Controls.Add(Me.btnConfirmarSalir)
        Me.Panel11.Controls.Add(Me.Label3)
        Me.Panel11.Controls.Add(Me.Panel12)
        Me.Panel11.Controls.Add(Me.PictureBox3)
        Me.Panel11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel11.Location = New System.Drawing.Point(1, 1)
        Me.Panel11.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel11.Name = "Panel11"
        Me.Panel11.Size = New System.Drawing.Size(903, 65)
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
        Me.Label3.Size = New System.Drawing.Size(253, 65)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "ASIGNACION ZONA"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel12
        '
        Me.Panel12.BackColor = System.Drawing.Color.White
        Me.Panel12.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel12.Location = New System.Drawing.Point(59, 0)
        Me.Panel12.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel12.Name = "Panel12"
        Me.Panel12.Size = New System.Drawing.Size(1, 65)
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
        Me.PictureBox3.Size = New System.Drawing.Size(59, 65)
        Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox3.TabIndex = 0
        Me.PictureBox3.TabStop = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.grzonas)
        Me.Panel1.Controls.Add(Me.grPersonal)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 67)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(905, 491)
        Me.Panel1.TabIndex = 5
        '
        'btnConfirmarSalir
        '
        Me.btnConfirmarSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnConfirmarSalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnConfirmarSalir.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnConfirmarSalir.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnConfirmarSalir.ImageFixedSize = New System.Drawing.Size(24, 24)
        Me.btnConfirmarSalir.Location = New System.Drawing.Point(744, 8)
        Me.btnConfirmarSalir.Margin = New System.Windows.Forms.Padding(4)
        Me.btnConfirmarSalir.Name = "btnConfirmarSalir"
        Me.btnConfirmarSalir.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnConfirmarSalir.Size = New System.Drawing.Size(166, 53)
        Me.btnConfirmarSalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnConfirmarSalir.Symbol = "57673"
        Me.btnConfirmarSalir.SymbolColor = System.Drawing.Color.MediumTurquoise
        Me.btnConfirmarSalir.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnConfirmarSalir.SymbolSize = 20.0!
        Me.btnConfirmarSalir.TabIndex = 4
        Me.btnConfirmarSalir.Text = "GUARDAR"
        '
        'grzonas
        '
        Me.grzonas.AlternatingColors = True
        Me.grzonas.BackColor = System.Drawing.Color.White
        Me.grzonas.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grzonas.ColumnAutoResize = True
        Me.grzonas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grzonas.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grzonas.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grzonas.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grzonas.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grzonas.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grzonas.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.grzonas.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grzonas.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grzonas.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grzonas.HeaderFormatStyle.Alpha = 0
        Me.grzonas.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grzonas.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grzonas.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grzonas.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grzonas.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grzonas.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grzonas.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grzonas.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.grzonas.Location = New System.Drawing.Point(630, 0)
        Me.grzonas.Margin = New System.Windows.Forms.Padding(4)
        Me.grzonas.Name = "grzonas"
        Me.grzonas.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grzonas.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grzonas.RecordNavigator = True
        Me.grzonas.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grzonas.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grzonas.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grzonas.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grzonas.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grzonas.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grzonas.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grzonas.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grzonas.SelectedFormatStyle.BackColor = System.Drawing.Color.LawnGreen
        Me.grzonas.SelectedFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grzonas.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grzonas.SelectedFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grzonas.Size = New System.Drawing.Size(275, 491)
        Me.grzonas.TabIndex = 5
        Me.grzonas.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grzonas.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grzonas.TableSpacing = 9
        Me.grzonas.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grzonas.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grzonas.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grzonas.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'F_AsignacionZona
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(905, 558)
        Me.Controls.Add(Me.Paneltop)
        Me.Name = "F_AsignacionZona"
        Me.Text = "ASIGNACION ZONA"
        Me.Paneltop.ResumeLayout(False)
        CType(Me.grPersonal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        CType(Me.grzonas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Paneltop As Panel
    Friend WithEvents grPersonal As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel10 As Panel
    Friend WithEvents Panel11 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel12 As Panel
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents btnConfirmarSalir As DevComponents.DotNetBar.ButtonX
    Friend WithEvents grzonas As Janus.Windows.GridEX.GridEX
End Class
