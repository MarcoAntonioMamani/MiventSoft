<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_IncrementoVenta
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
        Me.Principal = New System.Windows.Forms.Panel()
        Me.PanelBody = New System.Windows.Forms.Panel()
        Me.PanelBotones = New System.Windows.Forms.Panel()
        Me.btnCancelar = New DevComponents.DotNetBar.ButtonX()
        Me.btnGuardar = New DevComponents.DotNetBar.ButtonX()
        Me.lblBs = New DevComponents.DotNetBar.LabelX()
        Me.tbMonto = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.lblMontoCaption = New DevComponents.DotNetBar.LabelX()
        Me.lblInfo = New DevComponents.DotNetBar.LabelX()
        Me.PanelTitulo = New System.Windows.Forms.Panel()
        Me.lblTitulo = New System.Windows.Forms.Label()
        Me.lblVerHistorial = New System.Windows.Forms.Label()
        Me.Principal.SuspendLayout()
        Me.PanelBody.SuspendLayout()
        Me.PanelBotones.SuspendLayout()
        Me.PanelTitulo.SuspendLayout()
        Me.SuspendLayout()
        '
        'Principal
        '
        Me.Principal.BackColor = System.Drawing.Color.White
        Me.Principal.Controls.Add(Me.PanelBody)
        Me.Principal.Controls.Add(Me.PanelTitulo)
        Me.Principal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Principal.Location = New System.Drawing.Point(0, 0)
        Me.Principal.Name = "Principal"
        Me.Principal.Size = New System.Drawing.Size(420, 260)
        Me.Principal.TabIndex = 0
        '
        'PanelBody
        '
        Me.PanelBody.Controls.Add(Me.PanelBotones)
        Me.PanelBody.Controls.Add(Me.lblBs)
        Me.PanelBody.Controls.Add(Me.tbMonto)
        Me.PanelBody.Controls.Add(Me.lblMontoCaption)
        Me.PanelBody.Controls.Add(Me.lblInfo)
        Me.PanelBody.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelBody.Location = New System.Drawing.Point(0, 45)
        Me.PanelBody.Name = "PanelBody"
        Me.PanelBody.Padding = New System.Windows.Forms.Padding(20, 15, 20, 15)
        Me.PanelBody.Size = New System.Drawing.Size(420, 215)
        Me.PanelBody.TabIndex = 1
        '
        'PanelBotones
        '
        Me.PanelBotones.Controls.Add(Me.btnCancelar)
        Me.PanelBotones.Controls.Add(Me.btnGuardar)
        Me.PanelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBotones.Location = New System.Drawing.Point(20, 145)
        Me.PanelBotones.Name = "PanelBotones"
        Me.PanelBotones.Size = New System.Drawing.Size(380, 55)
        Me.PanelBotones.TabIndex = 4
        '
        'btnCancelar
        '
        Me.btnCancelar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnCancelar.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnCancelar.CustomColorName = "150;150;150"
        Me.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancelar.Font = New System.Drawing.Font("Calibri", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancelar.Location = New System.Drawing.Point(198, 8)
        Me.btnCancelar.Name = "btnCancelar"
        Me.btnCancelar.Size = New System.Drawing.Size(170, 42)
        Me.btnCancelar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2000
        Me.btnCancelar.TabIndex = 1
        Me.btnCancelar.Text = "CANCELAR"
        Me.btnCancelar.TextColor = System.Drawing.Color.DarkRed
        '
        'btnGuardar
        '
        Me.btnGuardar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnGuardar.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnGuardar.CustomColorName = "25;170;141"
        Me.btnGuardar.Font = New System.Drawing.Font("Calibri", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGuardar.Location = New System.Drawing.Point(10, 8)
        Me.btnGuardar.Name = "btnGuardar"
        Me.btnGuardar.Size = New System.Drawing.Size(170, 42)
        Me.btnGuardar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2000
        Me.btnGuardar.TabIndex = 0
        Me.btnGuardar.Text = "GUARDAR"
        Me.btnGuardar.TextColor = System.Drawing.Color.Black
        '
        'lblBs
        '
        Me.lblBs.AutoSize = True
        Me.lblBs.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lblBs.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lblBs.Font = New System.Drawing.Font("Calibri", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBs.ForeColor = System.Drawing.Color.Gray
        Me.lblBs.Location = New System.Drawing.Point(296, 112)
        Me.lblBs.Name = "lblBs"
        Me.lblBs.Size = New System.Drawing.Size(22, 22)
        Me.lblBs.TabIndex = 5
        Me.lblBs.Text = "Bs."
        '
        'tbMonto
        '
        '
        '
        '
        Me.tbMonto.Border.Class = "TextBoxBorder"
        Me.tbMonto.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbMonto.Font = New System.Drawing.Font("Calibri", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMonto.ForeColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.tbMonto.Location = New System.Drawing.Point(140, 102)
        Me.tbMonto.Name = "tbMonto"
        Me.tbMonto.PreventEnterBeep = True
        Me.tbMonto.Size = New System.Drawing.Size(150, 40)
        Me.tbMonto.TabIndex = 2
        '
        'lblMontoCaption
        '
        Me.lblMontoCaption.AutoSize = True
        Me.lblMontoCaption.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lblMontoCaption.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lblMontoCaption.Font = New System.Drawing.Font("Calibri", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMontoCaption.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lblMontoCaption.Location = New System.Drawing.Point(20, 78)
        Me.lblMontoCaption.Name = "lblMontoCaption"
        Me.lblMontoCaption.Size = New System.Drawing.Size(130, 22)
        Me.lblMontoCaption.TabIndex = 3
        Me.lblMontoCaption.Text = "Monto de Hoy (Bs):"
        '
        'lblInfo
        '
        Me.lblInfo.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lblInfo.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lblInfo.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblInfo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer), CType(CType(90, Byte), Integer))
        Me.lblInfo.Location = New System.Drawing.Point(20, 15)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(380, 55)
        Me.lblInfo.TabIndex = 0
        Me.lblInfo.Text = "Monto que se suma automaticamente al total de cada venta. Se guarda para el dia d" &
    "e hoy y afecta a las ventas nuevas que se registren."
        Me.lblInfo.WordWrap = True
        '
        'PanelTitulo
        '
        Me.PanelTitulo.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.PanelTitulo.Controls.Add(Me.lblTitulo)
        Me.PanelTitulo.Controls.Add(Me.lblVerHistorial)
        Me.PanelTitulo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelTitulo.Location = New System.Drawing.Point(0, 0)
        Me.PanelTitulo.Name = "PanelTitulo"
        Me.PanelTitulo.Size = New System.Drawing.Size(420, 45)
        Me.PanelTitulo.TabIndex = 0
        '
        'lblTitulo
        '
        Me.lblTitulo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTitulo.Font = New System.Drawing.Font("Calibri", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitulo.ForeColor = System.Drawing.Color.White
        Me.lblTitulo.Location = New System.Drawing.Point(0, 0)
        Me.lblTitulo.Name = "lblTitulo"
        Me.lblTitulo.Padding = New System.Windows.Forms.Padding(18, 0, 0, 0)
        Me.lblTitulo.Size = New System.Drawing.Size(280, 45)
        Me.lblTitulo.TabIndex = 0
        Me.lblTitulo.Text = "INCREMENTO DIARIO A LA VENTA"
        Me.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblVerHistorial
        '
        Me.lblVerHistorial.Cursor = System.Windows.Forms.Cursors.Hand
        Me.lblVerHistorial.Dock = System.Windows.Forms.DockStyle.Right
        Me.lblVerHistorial.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVerHistorial.ForeColor = System.Drawing.Color.White
        Me.lblVerHistorial.Location = New System.Drawing.Point(280, 0)
        Me.lblVerHistorial.Name = "lblVerHistorial"
        Me.lblVerHistorial.Padding = New System.Windows.Forms.Padding(0, 0, 15, 0)
        Me.lblVerHistorial.Size = New System.Drawing.Size(140, 45)
        Me.lblVerHistorial.TabIndex = 1
        Me.lblVerHistorial.Text = "Ver Historial"
        Me.lblVerHistorial.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Tec_IncrementoVenta
        '
        Me.AcceptButton = Me.btnGuardar
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancelar
        Me.ClientSize = New System.Drawing.Size(420, 260)
        Me.Controls.Add(Me.Principal)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Tec_IncrementoVenta"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Incremento Diario a la Venta"
        Me.Principal.ResumeLayout(False)
        Me.PanelBody.ResumeLayout(False)
        Me.PanelBody.PerformLayout()
        Me.PanelBotones.ResumeLayout(False)
        Me.PanelTitulo.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Principal As Panel
    Friend WithEvents PanelTitulo As Panel
    Friend WithEvents lblTitulo As Label
    Friend WithEvents lblVerHistorial As Label
    Friend WithEvents PanelBody As Panel
    Friend WithEvents lblInfo As DevComponents.DotNetBar.LabelX
    Friend WithEvents lblMontoCaption As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbMonto As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents lblBs As DevComponents.DotNetBar.LabelX
    Friend WithEvents PanelBotones As Panel
    Friend WithEvents btnGuardar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnCancelar As DevComponents.DotNetBar.ButtonX
End Class
