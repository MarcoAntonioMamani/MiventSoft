<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Formulario_Cantidad_Lote
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
        Me.tbCantidad = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.txtStock = New System.Windows.Forms.Label()
        Me.btnNo = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnSi = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.txtProducto = New System.Windows.Forms.Label()
        Me.txtTitulo = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tbLote = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.tbFechaVencimiento = New DevComponents.Editors.DateTimeAdv.DateTimeInput()
        Me.btnNo.SuspendLayout()
        Me.btnSi.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbFechaVencimiento, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
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
        Me.tbCantidad.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCantidad.ForeColor = System.Drawing.Color.Black
        Me.tbCantidad.Location = New System.Drawing.Point(332, 247)
        Me.tbCantidad.Margin = New System.Windows.Forms.Padding(4)
        Me.tbCantidad.Name = "tbCantidad"
        Me.tbCantidad.PreventEnterBeep = True
        Me.tbCantidad.Size = New System.Drawing.Size(206, 40)
        Me.tbCantidad.TabIndex = 11
        Me.tbCantidad.Text = "0"
        Me.tbCantidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtStock
        '
        Me.txtStock.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtStock.ForeColor = System.Drawing.Color.DarkCyan
        Me.txtStock.Location = New System.Drawing.Point(91, 219)
        Me.txtStock.Name = "txtStock"
        Me.txtStock.Size = New System.Drawing.Size(447, 24)
        Me.txtStock.TabIndex = 17
        Me.txtStock.Text = "Cantidad Disponible = 10"
        Me.txtStock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnNo
        '
        Me.btnNo.BackColor = System.Drawing.Color.FromArgb(CType(CType(170, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.btnNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.btnNo.Controls.Add(Me.Label4)
        Me.btnNo.Location = New System.Drawing.Point(130, 429)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(156, 50)
        Me.btnNo.TabIndex = 13
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
        Me.btnSi.Location = New System.Drawing.Point(344, 429)
        Me.btnSi.Name = "btnSi"
        Me.btnSi.Size = New System.Drawing.Size(156, 50)
        Me.btnSi.TabIndex = 12
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Calibri", 13.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(54, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(30, 29)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Si"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.DinoM.My.Resources.Resources.compra
        Me.PictureBox1.Location = New System.Drawing.Point(252, 48)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(80, 76)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 16
        Me.PictureBox1.TabStop = False
        '
        'txtProducto
        '
        Me.txtProducto.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtProducto.ForeColor = System.Drawing.Color.FromArgb(CType(CType(84, Byte), Integer), CType(CType(84, Byte), Integer), CType(CType(84, Byte), Integer))
        Me.txtProducto.Location = New System.Drawing.Point(91, 195)
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
        Me.txtTitulo.Location = New System.Drawing.Point(86, 146)
        Me.txtTitulo.Name = "txtTitulo"
        Me.txtTitulo.Size = New System.Drawing.Size(462, 49)
        Me.txtTitulo.TabIndex = 14
        Me.txtTitulo.Text = "Ingrese Cantidad al Carrito"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(162, 249)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(125, 35)
        Me.Label1.TabIndex = 18
        Me.Label1.Text = "Cantidad:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer))
        Me.Label2.Location = New System.Drawing.Point(162, 297)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(73, 35)
        Me.Label2.TabIndex = 20
        Me.Label2.Text = "Lote:"
        '
        'tbLote
        '
        '
        '
        '
        Me.tbLote.Border.BackColor = System.Drawing.Color.MediumVioletRed
        Me.tbLote.Border.BackColor2 = System.Drawing.Color.MediumTurquoise
        Me.tbLote.Border.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbLote.Border.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbLote.Border.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbLote.Border.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Dot
        Me.tbLote.Border.Class = "TextBoxBorder"
        Me.tbLote.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbLote.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbLote.ForeColor = System.Drawing.Color.Black
        Me.tbLote.Location = New System.Drawing.Point(332, 295)
        Me.tbLote.Margin = New System.Windows.Forms.Padding(4)
        Me.tbLote.Name = "tbLote"
        Me.tbLote.PreventEnterBeep = True
        Me.tbLote.Size = New System.Drawing.Size(206, 40)
        Me.tbLote.TabIndex = 19
        Me.tbLote.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.FromArgb(CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer), CType(CType(89, Byte), Integer))
        Me.Label5.Location = New System.Drawing.Point(165, 352)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(151, 35)
        Me.Label5.TabIndex = 21
        Me.Label5.Text = "Fecha Venc."
        '
        'tbFechaVencimiento
        '
        '
        '
        '
        Me.tbFechaVencimiento.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbFechaVencimiento.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbFechaVencimiento.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown
        Me.tbFechaVencimiento.ButtonDropDown.Visible = True
        Me.tbFechaVencimiento.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbFechaVencimiento.IsPopupCalendarOpen = False
        Me.tbFechaVencimiento.Location = New System.Drawing.Point(332, 350)
        Me.tbFechaVencimiento.Margin = New System.Windows.Forms.Padding(4)
        '
        '
        '
        '
        '
        '
        Me.tbFechaVencimiento.MonthCalendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbFechaVencimiento.MonthCalendar.CalendarDimensions = New System.Drawing.Size(1, 1)
        Me.tbFechaVencimiento.MonthCalendar.ClearButtonVisible = True
        '
        '
        '
        Me.tbFechaVencimiento.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2
        Me.tbFechaVencimiento.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90
        Me.tbFechaVencimiento.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground
        Me.tbFechaVencimiento.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.tbFechaVencimiento.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder
        Me.tbFechaVencimiento.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1
        Me.tbFechaVencimiento.MonthCalendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbFechaVencimiento.MonthCalendar.DisplayMonth = New Date(2017, 2, 1, 0, 0, 0, 0)
        Me.tbFechaVencimiento.MonthCalendar.FirstDayOfWeek = System.DayOfWeek.Monday
        '
        '
        '
        Me.tbFechaVencimiento.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2
        Me.tbFechaVencimiento.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90
        Me.tbFechaVencimiento.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground
        Me.tbFechaVencimiento.MonthCalendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbFechaVencimiento.MonthCalendar.TodayButtonVisible = True
        Me.tbFechaVencimiento.Name = "tbFechaVencimiento"
        Me.tbFechaVencimiento.Size = New System.Drawing.Size(206, 40)
        Me.tbFechaVencimiento.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.tbFechaVencimiento.TabIndex = 22
        '
        'Formulario_Cantidad_Lote
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(671, 556)
        Me.Controls.Add(Me.tbFechaVencimiento)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.tbLote)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tbCantidad)
        Me.Controls.Add(Me.txtStock)
        Me.Controls.Add(Me.btnNo)
        Me.Controls.Add(Me.btnSi)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.txtProducto)
        Me.Controls.Add(Me.txtTitulo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "Formulario_Cantidad_Lote"
        Me.Text = "Formulario_Cantidad_Lote"
        Me.btnNo.ResumeLayout(False)
        Me.btnNo.PerformLayout()
        Me.btnSi.ResumeLayout(False)
        Me.btnSi.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbFechaVencimiento, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tbCantidad As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents txtStock As Label
    Friend WithEvents btnNo As Panel
    Friend WithEvents Label4 As Label
    Friend WithEvents btnSi As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents txtProducto As Label
    Friend WithEvents txtTitulo As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents tbLote As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents Label5 As Label
    Friend WithEvents tbFechaVencimiento As DevComponents.Editors.DateTimeAdv.DateTimeInput
End Class
