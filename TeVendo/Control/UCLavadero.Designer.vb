﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UCLavadero
    Inherits System.Windows.Forms.UserControl

    'UserControl reemplaza a Dispose para limpiar la lista de componentes.
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
        Me.pbSombra = New System.Windows.Forms.Panel()
        Me.pbImg = New System.Windows.Forms.PictureBox()
        Me.pbSombra.SuspendLayout()
        CType(Me.pbImg, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbSombra
        '
        Me.pbSombra.BackColor = System.Drawing.Color.White
        Me.pbSombra.Controls.Add(Me.pbImg)
        Me.pbSombra.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbSombra.Location = New System.Drawing.Point(0, 0)
        Me.pbSombra.Margin = New System.Windows.Forms.Padding(4)
        Me.pbSombra.Name = "pbSombra"
        Me.pbSombra.Padding = New System.Windows.Forms.Padding(7, 6, 7, 6)
        Me.pbSombra.Size = New System.Drawing.Size(158, 146)
        Me.pbSombra.TabIndex = 3
        '
        'pbImg
        '
        Me.pbImg.BackColor = System.Drawing.Color.White
        Me.pbImg.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbImg.Image = Global.TeVendo.My.Resources.Resources.camera
        Me.pbImg.Location = New System.Drawing.Point(7, 6)
        Me.pbImg.Margin = New System.Windows.Forms.Padding(4)
        Me.pbImg.Name = "pbImg"
        Me.pbImg.Padding = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.pbImg.Size = New System.Drawing.Size(144, 134)
        Me.pbImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbImg.TabIndex = 0
        Me.pbImg.TabStop = False
        '
        'UCLavadero
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pbSombra)
        Me.Name = "UCLavadero"
        Me.Size = New System.Drawing.Size(158, 146)
        Me.pbSombra.ResumeLayout(False)
        CType(Me.pbImg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pbSombra As Panel
    Friend WithEvents pbImg As PictureBox
End Class
