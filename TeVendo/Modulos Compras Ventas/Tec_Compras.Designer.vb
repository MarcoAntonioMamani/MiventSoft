﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_Compras
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
        Dim cbSucursal_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Tec_Compras))
        Me.TabControlPrincipal = New DevComponents.DotNetBar.SuperTabControl()
        Me.SuperTabControlPanel2 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel17 = New System.Windows.Forms.Panel()
        Me.JGrM_Buscador = New Janus.Windows.GridEX.GridEX()
        Me.MeuOpciones = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.VerToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EliminarToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel21 = New System.Windows.Forms.Panel()
        Me.btnFiltrarVentas = New DevComponents.DotNetBar.ButtonX()
        Me.tbHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX16 = New DevComponents.DotNetBar.LabelX()
        Me.tbDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX6 = New DevComponents.DotNetBar.LabelX()
        Me.btnSi = New System.Windows.Forms.Panel()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.Panel14 = New System.Windows.Forms.Panel()
        Me.Panel15 = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Panel16 = New System.Windows.Forms.Panel()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.SuperTabItem2 = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel1 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PanelSuperior = New System.Windows.Forms.Panel()
        Me.PanelRight = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.PanelImagenes = New System.Windows.Forms.Panel()
        Me.PanelVerImagen = New System.Windows.Forms.Panel()
        Me.grDetalle = New Janus.Windows.GridEX.GridEX()
        Me.PanelTotal = New System.Windows.Forms.Panel()
        Me.Panel13 = New System.Windows.Forms.Panel()
        Me.LabelX10 = New DevComponents.DotNetBar.LabelX()
        Me.tbTotal = New DevComponents.Editors.DoubleInput()
        Me.LabelX9 = New DevComponents.DotNetBar.LabelX()
        Me.tbMdesc = New DevComponents.Editors.DoubleInput()
        Me.LabelX8 = New DevComponents.DotNetBar.LabelX()
        Me.tbPdesc = New DevComponents.Editors.DoubleInput()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.lbprivilegio = New System.Windows.Forms.Label()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.PanelLEft = New System.Windows.Forms.Panel()
        Me.Panel8 = New System.Windows.Forms.Panel()
        Me.Panel9 = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.btnSeleccionarProducto = New DevComponents.DotNetBar.ButtonX()
        Me.cbSucursal = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.tbFechaVencimientoCredito = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.tbFechaTransaccion = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.btnProveedor = New DevComponents.DotNetBar.ButtonX()
        Me.lbFechaVencimientoCredito = New DevComponents.DotNetBar.LabelX()
        Me.lbtext = New DevComponents.DotNetBar.LabelX()
        Me.swTipoVenta = New DevComponents.DotNetBar.Controls.SwitchButton()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.tbProveedor = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.tbCodigo = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.LabelX3 = New DevComponents.DotNetBar.LabelX()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.LabelX4 = New DevComponents.DotNetBar.LabelX()
        Me.tbGlosa = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.LabelX7 = New DevComponents.DotNetBar.LabelX()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.PanelButton = New System.Windows.Forms.Panel()
        Me.PanelToolBar1 = New System.Windows.Forms.Panel()
        Me.btnCompra = New DevComponents.DotNetBar.ButtonX()
        Me.btnSalir = New DevComponents.DotNetBar.ButtonX()
        Me.btnEliminar = New DevComponents.DotNetBar.ButtonX()
        Me.btnGrabar = New DevComponents.DotNetBar.ButtonX()
        Me.btnModificar = New DevComponents.DotNetBar.ButtonX()
        Me.btnNuevo = New DevComponents.DotNetBar.ButtonX()
        Me.PanelNavegacion = New System.Windows.Forms.Panel()
        Me.LblPaginacion = New System.Windows.Forms.Label()
        Me.btnUltimo = New DevComponents.DotNetBar.ButtonX()
        Me.btnSiguiente = New DevComponents.DotNetBar.ButtonX()
        Me.btnAnterior = New DevComponents.DotNetBar.ButtonX()
        Me.btnPrimero = New DevComponents.DotNetBar.ButtonX()
        Me.SuperTabItem1 = New DevComponents.DotNetBar.SuperTabItem()
        Me.MEP = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.MHighlighterFocus = New DevComponents.DotNetBar.Validator.Highlighter()
        CType(Me.TabControlPrincipal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControlPrincipal.SuspendLayout()
        Me.SuperTabControlPanel2.SuspendLayout()
        Me.Panel17.SuspendLayout()
        CType(Me.JGrM_Buscador, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MeuOpciones.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Panel21.SuspendLayout()
        Me.btnSi.SuspendLayout()
        Me.Panel14.SuspendLayout()
        Me.Panel15.SuspendLayout()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuperTabControlPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.PanelSuperior.SuspendLayout()
        Me.PanelRight.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.PanelImagenes.SuspendLayout()
        Me.PanelVerImagen.SuspendLayout()
        CType(Me.grDetalle, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelTotal.SuspendLayout()
        Me.Panel13.SuspendLayout()
        CType(Me.tbTotal, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbMdesc, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbPdesc, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel5.SuspendLayout()
        Me.Panel6.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelLEft.SuspendLayout()
        Me.Panel8.SuspendLayout()
        Me.Panel9.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.cbSucursal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelButton.SuspendLayout()
        Me.PanelToolBar1.SuspendLayout()
        Me.PanelNavegacion.SuspendLayout()
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TabControlPrincipal
        '
        '
        '
        '
        '
        '
        '
        Me.TabControlPrincipal.ControlBox.CloseBox.Name = ""
        '
        '
        '
        Me.TabControlPrincipal.ControlBox.MenuBox.Name = ""
        Me.TabControlPrincipal.ControlBox.Name = ""
        Me.TabControlPrincipal.ControlBox.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.TabControlPrincipal.ControlBox.MenuBox, Me.TabControlPrincipal.ControlBox.CloseBox})
        Me.TabControlPrincipal.Controls.Add(Me.SuperTabControlPanel2)
        Me.TabControlPrincipal.Controls.Add(Me.SuperTabControlPanel1)
        Me.TabControlPrincipal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlPrincipal.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControlPrincipal.Location = New System.Drawing.Point(0, 0)
        Me.TabControlPrincipal.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.TabControlPrincipal.Name = "TabControlPrincipal"
        Me.TabControlPrincipal.ReorderTabsEnabled = True
        Me.TabControlPrincipal.SelectedTabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControlPrincipal.SelectedTabIndex = 1
        Me.TabControlPrincipal.Size = New System.Drawing.Size(1493, 796)
        Me.TabControlPrincipal.TabAlignment = DevComponents.DotNetBar.eTabStripAlignment.Bottom
        Me.TabControlPrincipal.TabFont = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControlPrincipal.TabIndex = 2
        Me.TabControlPrincipal.TabLayoutType = DevComponents.DotNetBar.eSuperTabLayoutType.MultiLineFit
        Me.TabControlPrincipal.Tabs.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.SuperTabItem1, Me.SuperTabItem2})
        Me.TabControlPrincipal.TabStyle = DevComponents.DotNetBar.eSuperTabStyle.WinMediaPlayer12
        Me.TabControlPrincipal.Text = "Datos"
        '
        'SuperTabControlPanel2
        '
        Me.SuperTabControlPanel2.Controls.Add(Me.Panel17)
        Me.SuperTabControlPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel2.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabControlPanel2.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.SuperTabControlPanel2.Name = "SuperTabControlPanel2"
        Me.SuperTabControlPanel2.Size = New System.Drawing.Size(1493, 765)
        Me.SuperTabControlPanel2.TabIndex = 2
        Me.SuperTabControlPanel2.TabItem = Me.SuperTabItem2
        '
        'Panel17
        '
        Me.Panel17.Controls.Add(Me.JGrM_Buscador)
        Me.Panel17.Controls.Add(Me.Panel4)
        Me.Panel17.Controls.Add(Me.Panel14)
        Me.Panel17.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel17.Location = New System.Drawing.Point(0, 0)
        Me.Panel17.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel17.Name = "Panel17"
        Me.Panel17.Padding = New System.Windows.Forms.Padding(5)
        Me.Panel17.Size = New System.Drawing.Size(1493, 765)
        Me.Panel17.TabIndex = 4
        '
        'JGrM_Buscador
        '
        Me.JGrM_Buscador.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.JGrM_Buscador.AlternatingColors = True
        Me.JGrM_Buscador.BackColor = System.Drawing.Color.White
        Me.JGrM_Buscador.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.JGrM_Buscador.ColumnAutoResize = True
        Me.JGrM_Buscador.ContextMenuStrip = Me.MeuOpciones
        Me.JGrM_Buscador.Dock = System.Windows.Forms.DockStyle.Fill
        Me.JGrM_Buscador.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.JGrM_Buscador.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.JGrM_Buscador.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.JGrM_Buscador.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.JGrM_Buscador.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.JGrM_Buscador.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle
        Me.JGrM_Buscador.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.JGrM_Buscador.GridLineColor = System.Drawing.Color.Black
        Me.JGrM_Buscador.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.JGrM_Buscador.HeaderFormatStyle.Alpha = 0
        Me.JGrM_Buscador.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.JGrM_Buscador.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.JGrM_Buscador.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.JGrM_Buscador.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.JGrM_Buscador.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.JGrM_Buscador.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.JGrM_Buscador.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.JGrM_Buscador.HideSelection = Janus.Windows.GridEX.HideSelection.Highlight
        Me.JGrM_Buscador.Location = New System.Drawing.Point(5, 105)
        Me.JGrM_Buscador.Margin = New System.Windows.Forms.Padding(4)
        Me.JGrM_Buscador.Name = "JGrM_Buscador"
        Me.JGrM_Buscador.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Black
        Me.JGrM_Buscador.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.JGrM_Buscador.RecordNavigator = True
        Me.JGrM_Buscador.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.JGrM_Buscador.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.JGrM_Buscador.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.JGrM_Buscador.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.JGrM_Buscador.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.JGrM_Buscador.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.JGrM_Buscador.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.JGrM_Buscador.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.JGrM_Buscador.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.JGrM_Buscador.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.JGrM_Buscador.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.JGrM_Buscador.Size = New System.Drawing.Size(1483, 655)
        Me.JGrM_Buscador.TabIndex = 2
        Me.JGrM_Buscador.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.JGrM_Buscador.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.JGrM_Buscador.TableSpacing = 9
        Me.JGrM_Buscador.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.JGrM_Buscador.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.JGrM_Buscador.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.JGrM_Buscador.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'MeuOpciones
        '
        Me.MeuOpciones.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MeuOpciones.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.VerToolStripMenuItem1, Me.EditarToolStripMenuItem, Me.EliminarToolStripMenuItem1})
        Me.MeuOpciones.Name = "MeuOpciones"
        Me.MeuOpciones.Size = New System.Drawing.Size(146, 112)
        Me.MeuOpciones.Text = "Opciones"
        '
        'VerToolStripMenuItem1
        '
        Me.VerToolStripMenuItem1.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.VerToolStripMenuItem1.Image = Global.TeVendo.My.Resources.Resources.verRegistros2
        Me.VerToolStripMenuItem1.Name = "VerToolStripMenuItem1"
        Me.VerToolStripMenuItem1.Padding = New System.Windows.Forms.Padding(5)
        Me.VerToolStripMenuItem1.Size = New System.Drawing.Size(155, 36)
        Me.VerToolStripMenuItem1.Text = "Ver"
        '
        'EditarToolStripMenuItem
        '
        Me.EditarToolStripMenuItem.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.EditarToolStripMenuItem.Image = Global.TeVendo.My.Resources.Resources.edit
        Me.EditarToolStripMenuItem.Name = "EditarToolStripMenuItem"
        Me.EditarToolStripMenuItem.Padding = New System.Windows.Forms.Padding(5)
        Me.EditarToolStripMenuItem.Size = New System.Drawing.Size(155, 36)
        Me.EditarToolStripMenuItem.Text = "Editar"
        '
        'EliminarToolStripMenuItem1
        '
        Me.EliminarToolStripMenuItem1.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.EliminarToolStripMenuItem1.Image = Global.TeVendo.My.Resources.Resources.trash2
        Me.EliminarToolStripMenuItem1.Name = "EliminarToolStripMenuItem1"
        Me.EliminarToolStripMenuItem1.Padding = New System.Windows.Forms.Padding(5)
        Me.EliminarToolStripMenuItem1.Size = New System.Drawing.Size(155, 36)
        Me.EliminarToolStripMenuItem1.Text = "Eliminar"
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.White
        Me.Panel4.Controls.Add(Me.TableLayoutPanel1)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(5, 41)
        Me.Panel4.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1483, 64)
        Me.Panel4.TabIndex = 4
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Panel21, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnSi, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.Padding = New System.Windows.Forms.Padding(2, 2, 5, 5)
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1483, 64)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'Panel21
        '
        Me.Panel21.AutoScroll = True
        Me.Panel21.Controls.Add(Me.btnFiltrarVentas)
        Me.Panel21.Controls.Add(Me.tbHasta)
        Me.Panel21.Controls.Add(Me.LabelX16)
        Me.Panel21.Controls.Add(Me.tbDesde)
        Me.Panel21.Controls.Add(Me.LabelX6)
        Me.Panel21.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel21.Location = New System.Drawing.Point(5, 5)
        Me.Panel21.Name = "Panel21"
        Me.Panel21.Size = New System.Drawing.Size(1233, 51)
        Me.Panel21.TabIndex = 6
        '
        'btnFiltrarVentas
        '
        Me.btnFiltrarVentas.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnFiltrarVentas.BackColor = System.Drawing.Color.DarkOrange
        Me.btnFiltrarVentas.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnFiltrarVentas.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFiltrarVentas.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnFiltrarVentas.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.btnFiltrarVentas.Location = New System.Drawing.Point(587, 9)
        Me.btnFiltrarVentas.Margin = New System.Windows.Forms.Padding(4)
        Me.btnFiltrarVentas.Name = "btnFiltrarVentas"
        Me.btnFiltrarVentas.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnFiltrarVentas.Size = New System.Drawing.Size(121, 34)
        Me.btnFiltrarVentas.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnFiltrarVentas.Symbol = ""
        Me.btnFiltrarVentas.SymbolColor = System.Drawing.Color.White
        Me.btnFiltrarVentas.SymbolSize = 15.0!
        Me.btnFiltrarVentas.TabIndex = 222
        Me.btnFiltrarVentas.Text = "Filtrar"
        Me.btnFiltrarVentas.TextColor = System.Drawing.Color.White
        '
        'tbHasta
        '
        Me.tbHasta.BackColor = System.Drawing.Color.White
        Me.tbHasta.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
        Me.tbHasta.DropDownCalendar.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.None
        Me.tbHasta.DropDownCalendar.DayOfWeekAbbreviation = Janus.Windows.CalendarCombo.DayOfWeekAbbreviation.UseAbbreviatedName
        Me.tbHasta.DropDownCalendar.DaysFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbHasta.DropDownCalendar.DaysFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbHasta.DropDownCalendar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbHasta.DropDownCalendar.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbHasta.DropDownCalendar.HeaderAppearance = Janus.Windows.CalendarCombo.ButtonAppearance.PopUp
        Me.tbHasta.DropDownCalendar.HeaderFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbHasta.DropDownCalendar.HeaderFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbHasta.DropDownCalendar.Name = ""
        Me.tbHasta.DropDownCalendar.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbHasta.DropDownCalendar.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbHasta.DropDownCalendar.TodayRectColor = System.Drawing.Color.DarkCyan
        Me.tbHasta.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbHasta.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.tbHasta.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbHasta.Location = New System.Drawing.Point(415, 12)
        Me.tbHasta.Name = "tbHasta"
        Me.tbHasta.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbHasta.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbHasta.SecondIncrement = 10
        Me.tbHasta.Size = New System.Drawing.Size(157, 26)
        Me.tbHasta.TabIndex = 220
        Me.tbHasta.TodayButtonText = "Hoy"
        Me.tbHasta.UseCompatibleTextRendering = False
        Me.tbHasta.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.tbHasta.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbHasta.YearIncrement = 10
        '
        'LabelX16
        '
        Me.LabelX16.AutoSize = True
        Me.LabelX16.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX16.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX16.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX16.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX16.Location = New System.Drawing.Point(316, 16)
        Me.LabelX16.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX16.Name = "LabelX16"
        Me.LabelX16.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX16.Size = New System.Drawing.Size(94, 21)
        Me.LabelX16.TabIndex = 221
        Me.LabelX16.Text = "Compra Hasta:"
        '
        'tbDesde
        '
        Me.tbDesde.BackColor = System.Drawing.Color.White
        Me.tbDesde.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
        Me.tbDesde.DropDownCalendar.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.None
        Me.tbDesde.DropDownCalendar.DayOfWeekAbbreviation = Janus.Windows.CalendarCombo.DayOfWeekAbbreviation.UseAbbreviatedName
        Me.tbDesde.DropDownCalendar.DaysFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbDesde.DropDownCalendar.DaysFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbDesde.DropDownCalendar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbDesde.DropDownCalendar.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbDesde.DropDownCalendar.HeaderAppearance = Janus.Windows.CalendarCombo.ButtonAppearance.PopUp
        Me.tbDesde.DropDownCalendar.HeaderFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbDesde.DropDownCalendar.HeaderFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbDesde.DropDownCalendar.Name = ""
        Me.tbDesde.DropDownCalendar.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbDesde.DropDownCalendar.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbDesde.DropDownCalendar.TodayRectColor = System.Drawing.Color.DarkCyan
        Me.tbDesde.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbDesde.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.tbDesde.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbDesde.Location = New System.Drawing.Point(152, 13)
        Me.tbDesde.Name = "tbDesde"
        Me.tbDesde.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbDesde.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbDesde.SecondIncrement = 10
        Me.tbDesde.Size = New System.Drawing.Size(157, 26)
        Me.tbDesde.TabIndex = 218
        Me.tbDesde.TodayButtonText = "Hoy"
        Me.tbDesde.UseCompatibleTextRendering = False
        Me.tbDesde.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.tbDesde.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbDesde.YearIncrement = 10
        '
        'LabelX6
        '
        Me.LabelX6.AutoSize = True
        Me.LabelX6.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX6.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX6.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX6.Location = New System.Drawing.Point(47, 17)
        Me.LabelX6.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX6.Name = "LabelX6"
        Me.LabelX6.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX6.Size = New System.Drawing.Size(98, 21)
        Me.LabelX6.TabIndex = 219
        Me.LabelX6.Text = "Compra Desde:"
        '
        'btnSi
        '
        Me.btnSi.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(179, Byte), Integer), CType(CType(148, Byte), Integer))
        Me.btnSi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.btnSi.Controls.Add(Me.ButtonX1)
        Me.btnSi.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnSi.Location = New System.Drawing.Point(1244, 4)
        Me.btnSi.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnSi.Name = "btnSi"
        Me.btnSi.Size = New System.Drawing.Size(231, 53)
        Me.btnSi.TabIndex = 4
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.BackColor = System.Drawing.Color.Transparent
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonX1.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.iconadd
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.ButtonX1.Location = New System.Drawing.Point(0, 0)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(229, 51)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX1.TabIndex = 7
        Me.ButtonX1.Text = "Nueva Compra"
        Me.ButtonX1.TextColor = System.Drawing.Color.White
        '
        'Panel14
        '
        Me.Panel14.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel14.Controls.Add(Me.Panel15)
        Me.Panel14.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel14.Location = New System.Drawing.Point(5, 5)
        Me.Panel14.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel14.Name = "Panel14"
        Me.Panel14.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel14.Size = New System.Drawing.Size(1483, 36)
        Me.Panel14.TabIndex = 3
        '
        'Panel15
        '
        Me.Panel15.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel15.Controls.Add(Me.Label2)
        Me.Panel15.Controls.Add(Me.Panel16)
        Me.Panel15.Controls.Add(Me.PictureBox4)
        Me.Panel15.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel15.Location = New System.Drawing.Point(1, 1)
        Me.Panel15.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel15.Name = "Panel15"
        Me.Panel15.Size = New System.Drawing.Size(1481, 34)
        Me.Panel15.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Left
        Me.Label2.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.Label2.Location = New System.Drawing.Point(60, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.Label2.Size = New System.Drawing.Size(253, 34)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "LISTADO DE COMPRAS"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel16
        '
        Me.Panel16.BackColor = System.Drawing.Color.White
        Me.Panel16.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel16.Location = New System.Drawing.Point(59, 0)
        Me.Panel16.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel16.Name = "Panel16"
        Me.Panel16.Size = New System.Drawing.Size(1, 34)
        Me.Panel16.TabIndex = 1
        '
        'PictureBox4
        '
        Me.PictureBox4.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox4.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox4.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox4.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox4.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PictureBox4.Name = "PictureBox4"
        Me.PictureBox4.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox4.Size = New System.Drawing.Size(59, 34)
        Me.PictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox4.TabIndex = 0
        Me.PictureBox4.TabStop = False
        '
        'SuperTabItem2
        '
        Me.SuperTabItem2.AttachedControl = Me.SuperTabControlPanel2
        Me.SuperTabItem2.GlobalItem = False
        Me.SuperTabItem2.Name = "SuperTabItem2"
        Me.SuperTabItem2.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Gold
        Me.SuperTabItem2.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabItem2.Text = "Lista De Datos"
        Me.SuperTabItem2.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Center
        '
        'SuperTabControlPanel1
        '
        Me.SuperTabControlPanel1.CanvasColor = System.Drawing.Color.Silver
        Me.SuperTabControlPanel1.Controls.Add(Me.Panel1)
        Me.SuperTabControlPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel1.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabControlPanel1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.SuperTabControlPanel1.Name = "SuperTabControlPanel1"
        Me.SuperTabControlPanel1.Size = New System.Drawing.Size(1493, 765)
        Me.SuperTabControlPanel1.TabIndex = 1
        Me.SuperTabControlPanel1.TabItem = Me.SuperTabItem1
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.PanelSuperior)
        Me.Panel1.Controls.Add(Me.PanelButton)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1493, 765)
        Me.Panel1.TabIndex = 0
        '
        'PanelSuperior
        '
        Me.PanelSuperior.Controls.Add(Me.PanelRight)
        Me.PanelSuperior.Controls.Add(Me.PanelLEft)
        Me.PanelSuperior.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelSuperior.Location = New System.Drawing.Point(0, 0)
        Me.PanelSuperior.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelSuperior.Name = "PanelSuperior"
        Me.PanelSuperior.Size = New System.Drawing.Size(1493, 715)
        Me.PanelSuperior.TabIndex = 1
        '
        'PanelRight
        '
        Me.PanelRight.Controls.Add(Me.Panel2)
        Me.PanelRight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelRight.Location = New System.Drawing.Point(0, 247)
        Me.PanelRight.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelRight.Name = "PanelRight"
        Me.PanelRight.Size = New System.Drawing.Size(1493, 468)
        Me.PanelRight.TabIndex = 1
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel2.Controls.Add(Me.PanelImagenes)
        Me.Panel2.Controls.Add(Me.Panel5)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Padding = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel2.Size = New System.Drawing.Size(1493, 468)
        Me.Panel2.TabIndex = 1
        '
        'PanelImagenes
        '
        Me.PanelImagenes.BackColor = System.Drawing.Color.White
        Me.PanelImagenes.Controls.Add(Me.PanelVerImagen)
        Me.PanelImagenes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelImagenes.Location = New System.Drawing.Point(3, 29)
        Me.PanelImagenes.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelImagenes.Name = "PanelImagenes"
        Me.PanelImagenes.Size = New System.Drawing.Size(1487, 437)
        Me.PanelImagenes.TabIndex = 2
        '
        'PanelVerImagen
        '
        Me.PanelVerImagen.BackColor = System.Drawing.Color.Transparent
        Me.PanelVerImagen.Controls.Add(Me.grDetalle)
        Me.PanelVerImagen.Controls.Add(Me.PanelTotal)
        Me.PanelVerImagen.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelVerImagen.Location = New System.Drawing.Point(0, 0)
        Me.PanelVerImagen.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelVerImagen.Name = "PanelVerImagen"
        Me.PanelVerImagen.Size = New System.Drawing.Size(1487, 437)
        Me.PanelVerImagen.TabIndex = 2
        '
        'grDetalle
        '
        Me.grDetalle.AlternatingColors = True
        Me.grDetalle.BackColor = System.Drawing.Color.White
        Me.grDetalle.BorderStyle = Janus.Windows.GridEX.BorderStyle.None
        Me.grDetalle.ColumnAutoResize = True
        Me.grDetalle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grDetalle.EnterKeyBehavior = Janus.Windows.GridEX.EnterKeyBehavior.None
        Me.grDetalle.FilterRowFormatStyle.BackColor = System.Drawing.Color.White
        Me.grDetalle.FilterRowFormatStyle.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.FilterRowFormatStyle.LineAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetalle.FilterRowFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetalle.FocusCellFormatStyle.BackColor = System.Drawing.Color.White
        Me.grDetalle.FocusCellFormatStyle.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.GridLineColor = System.Drawing.Color.Black
        Me.grDetalle.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grDetalle.HeaderFormatStyle.Alpha = 0
        Me.grDetalle.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grDetalle.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetalle.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grDetalle.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetalle.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetalle.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grDetalle.Location = New System.Drawing.Point(0, 0)
        Me.grDetalle.Margin = New System.Windows.Forms.Padding(4)
        Me.grDetalle.Name = "grDetalle"
        Me.grDetalle.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Black
        Me.grDetalle.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grDetalle.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetalle.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grDetalle.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetalle.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetalle.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grDetalle.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grDetalle.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grDetalle.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grDetalle.Size = New System.Drawing.Size(1278, 437)
        Me.grDetalle.TabIndex = 0
        Me.grDetalle.TabKeyBehavior = Janus.Windows.GridEX.TabKeyBehavior.ControlNavigation
        Me.grDetalle.TableHeaderFormatStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
        Me.grDetalle.TableSpacing = 9
        Me.grDetalle.TreeLineColor = System.Drawing.SystemColors.ControlDarkDark
        Me.grDetalle.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grDetalle.VisualStyleAreas.ControlBorderStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        Me.grDetalle.VisualStyleAreas.HeadersStyle = Janus.Windows.GridEX.VisualStyle.VS2005
        '
        'PanelTotal
        '
        Me.PanelTotal.AutoScroll = True
        Me.PanelTotal.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PanelTotal.Controls.Add(Me.Panel13)
        Me.PanelTotal.Dock = System.Windows.Forms.DockStyle.Right
        Me.PanelTotal.Location = New System.Drawing.Point(1278, 0)
        Me.PanelTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelTotal.Name = "PanelTotal"
        Me.PanelTotal.Size = New System.Drawing.Size(209, 437)
        Me.PanelTotal.TabIndex = 4
        '
        'Panel13
        '
        Me.Panel13.AutoScroll = True
        Me.Panel13.BackColor = System.Drawing.Color.Transparent
        Me.Panel13.Controls.Add(Me.LabelX10)
        Me.Panel13.Controls.Add(Me.tbTotal)
        Me.Panel13.Controls.Add(Me.LabelX9)
        Me.Panel13.Controls.Add(Me.tbMdesc)
        Me.Panel13.Controls.Add(Me.LabelX8)
        Me.Panel13.Controls.Add(Me.tbPdesc)
        Me.Panel13.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel13.Location = New System.Drawing.Point(0, 0)
        Me.Panel13.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel13.Name = "Panel13"
        Me.Panel13.Size = New System.Drawing.Size(209, 437)
        Me.Panel13.TabIndex = 39
        '
        'LabelX10
        '
        Me.LabelX10.AutoSize = True
        Me.LabelX10.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX10.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX10.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX10.ForeColor = System.Drawing.Color.White
        Me.LabelX10.Location = New System.Drawing.Point(8, 124)
        Me.LabelX10.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX10.Name = "LabelX10"
        Me.LabelX10.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX10.Size = New System.Drawing.Size(91, 21)
        Me.LabelX10.TabIndex = 53
        Me.LabelX10.Text = "Total Compra:"
        '
        'tbTotal
        '
        '
        '
        '
        Me.tbTotal.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.[Double]
        Me.tbTotal.BackgroundStyle.BorderBottomColor = System.Drawing.Color.Red
        Me.tbTotal.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.[Double]
        Me.tbTotal.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.[Double]
        Me.tbTotal.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.[Double]
        Me.tbTotal.BackgroundStyle.BorderTopColor = System.Drawing.Color.Gold
        Me.tbTotal.BackgroundStyle.BorderTopWidth = 1
        Me.tbTotal.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbTotal.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbTotal.BackgroundStyle.CornerTypeBottomLeft = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbTotal.BackgroundStyle.CornerTypeBottomRight = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbTotal.BackgroundStyle.CornerTypeTopLeft = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbTotal.BackgroundStyle.CornerTypeTopRight = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbTotal.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbTotal.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTotal.Increment = 1.0R
        Me.tbTotal.Location = New System.Drawing.Point(8, 154)
        Me.tbTotal.LockUpdateChecked = False
        Me.tbTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTotal.MinValue = 0R
        Me.tbTotal.Name = "tbTotal"
        Me.tbTotal.Size = New System.Drawing.Size(173, 32)
        Me.tbTotal.TabIndex = 52
        Me.tbTotal.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX9
        '
        Me.LabelX9.AutoSize = True
        Me.LabelX9.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX9.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX9.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX9.ForeColor = System.Drawing.Color.White
        Me.LabelX9.Location = New System.Drawing.Point(8, 60)
        Me.LabelX9.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX9.Name = "LabelX9"
        Me.LabelX9.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX9.Size = New System.Drawing.Size(119, 21)
        Me.LabelX9.TabIndex = 51
        Me.LabelX9.Text = "Monto Descuento:"
        '
        'tbMdesc
        '
        '
        '
        '
        Me.tbMdesc.BackgroundStyle.BorderBottomColor = System.Drawing.Color.Red
        Me.tbMdesc.BackgroundStyle.BorderTopColor = System.Drawing.Color.Gold
        Me.tbMdesc.BackgroundStyle.BorderTopWidth = 1
        Me.tbMdesc.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbMdesc.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbMdesc.BackgroundStyle.CornerTypeBottomLeft = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbMdesc.BackgroundStyle.CornerTypeBottomRight = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbMdesc.BackgroundStyle.CornerTypeTopLeft = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbMdesc.BackgroundStyle.CornerTypeTopRight = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbMdesc.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbMdesc.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMdesc.Increment = 1.0R
        Me.tbMdesc.Location = New System.Drawing.Point(75, 85)
        Me.tbMdesc.LockUpdateChecked = False
        Me.tbMdesc.Margin = New System.Windows.Forms.Padding(4)
        Me.tbMdesc.MinValue = 0R
        Me.tbMdesc.Name = "tbMdesc"
        Me.tbMdesc.Size = New System.Drawing.Size(108, 30)
        Me.tbMdesc.TabIndex = 50
        Me.tbMdesc.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX8
        '
        Me.LabelX8.AutoSize = True
        Me.LabelX8.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX8.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX8.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX8.ForeColor = System.Drawing.Color.White
        Me.LabelX8.Location = New System.Drawing.Point(8, 5)
        Me.LabelX8.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX8.Name = "LabelX8"
        Me.LabelX8.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX8.Size = New System.Drawing.Size(143, 21)
        Me.LabelX8.TabIndex = 49
        Me.LabelX8.Text = "Porcentaje Descuento:"
        '
        'tbPdesc
        '
        '
        '
        '
        Me.tbPdesc.BackgroundStyle.BorderBottomColor = System.Drawing.Color.Red
        Me.tbPdesc.BackgroundStyle.BorderTopColor = System.Drawing.Color.Gold
        Me.tbPdesc.BackgroundStyle.BorderTopWidth = 1
        Me.tbPdesc.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbPdesc.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbPdesc.BackgroundStyle.CornerTypeBottomLeft = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbPdesc.BackgroundStyle.CornerTypeBottomRight = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbPdesc.BackgroundStyle.CornerTypeTopLeft = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbPdesc.BackgroundStyle.CornerTypeTopRight = DevComponents.DotNetBar.eCornerType.Rounded
        Me.tbPdesc.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbPdesc.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbPdesc.Increment = 1.0R
        Me.tbPdesc.Location = New System.Drawing.Point(75, 27)
        Me.tbPdesc.LockUpdateChecked = False
        Me.tbPdesc.Margin = New System.Windows.Forms.Padding(4)
        Me.tbPdesc.MinValue = 0R
        Me.tbPdesc.Name = "tbPdesc"
        Me.tbPdesc.Size = New System.Drawing.Size(108, 30)
        Me.tbPdesc.TabIndex = 33
        Me.tbPdesc.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.Panel5.Controls.Add(Me.Panel6)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel5.Location = New System.Drawing.Point(3, 2)
        Me.Panel5.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel5.Size = New System.Drawing.Size(1487, 27)
        Me.Panel5.TabIndex = 1
        '
        'Panel6
        '
        Me.Panel6.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel6.Controls.Add(Me.lbprivilegio)
        Me.Panel6.Controls.Add(Me.Panel7)
        Me.Panel6.Controls.Add(Me.PictureBox2)
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel6.Location = New System.Drawing.Point(1, 1)
        Me.Panel6.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(1485, 25)
        Me.Panel6.TabIndex = 0
        '
        'lbprivilegio
        '
        Me.lbprivilegio.Dock = System.Windows.Forms.DockStyle.Left
        Me.lbprivilegio.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbprivilegio.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbprivilegio.Location = New System.Drawing.Point(60, 0)
        Me.lbprivilegio.Name = "lbprivilegio"
        Me.lbprivilegio.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.lbprivilegio.Size = New System.Drawing.Size(665, 25)
        Me.lbprivilegio.TabIndex = 2
        Me.lbprivilegio.Text = "Detalle Productos"
        Me.lbprivilegio.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel7
        '
        Me.Panel7.BackColor = System.Drawing.Color.White
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel7.Location = New System.Drawing.Point(59, 0)
        Me.Panel7.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(1, 25)
        Me.Panel7.TabIndex = 1
        '
        'PictureBox2
        '
        Me.PictureBox2.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PictureBox2.Dock = System.Windows.Forms.DockStyle.Left
        Me.PictureBox2.Image = Global.TeVendo.My.Resources.Resources.tec_triangulo_blanco
        Me.PictureBox2.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox2.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox2.Size = New System.Drawing.Size(59, 25)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 0
        Me.PictureBox2.TabStop = False
        '
        'PanelLEft
        '
        Me.PanelLEft.Controls.Add(Me.Panel8)
        Me.PanelLEft.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelLEft.Location = New System.Drawing.Point(0, 0)
        Me.PanelLEft.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelLEft.Name = "PanelLEft"
        Me.PanelLEft.Size = New System.Drawing.Size(1493, 247)
        Me.PanelLEft.TabIndex = 0
        '
        'Panel8
        '
        Me.Panel8.BackColor = System.Drawing.Color.FromArgb(CType(CType(40, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(170, Byte), Integer))
        Me.Panel8.Controls.Add(Me.Panel9)
        Me.Panel8.Controls.Add(Me.Panel10)
        Me.Panel8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel8.Location = New System.Drawing.Point(0, 0)
        Me.Panel8.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel8.Name = "Panel8"
        Me.Panel8.Padding = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel8.Size = New System.Drawing.Size(1493, 247)
        Me.Panel8.TabIndex = 1
        '
        'Panel9
        '
        Me.Panel9.BackColor = System.Drawing.Color.White
        Me.Panel9.Controls.Add(Me.Panel3)
        Me.Panel9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel9.Location = New System.Drawing.Point(3, 29)
        Me.Panel9.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel9.Name = "Panel9"
        Me.Panel9.Size = New System.Drawing.Size(1487, 216)
        Me.Panel9.TabIndex = 2
        '
        'Panel3
        '
        Me.Panel3.AutoScroll = True
        Me.Panel3.Controls.Add(Me.btnSeleccionarProducto)
        Me.Panel3.Controls.Add(Me.cbSucursal)
        Me.Panel3.Controls.Add(Me.tbFechaVencimientoCredito)
        Me.Panel3.Controls.Add(Me.tbFechaTransaccion)
        Me.Panel3.Controls.Add(Me.btnProveedor)
        Me.Panel3.Controls.Add(Me.lbFechaVencimientoCredito)
        Me.Panel3.Controls.Add(Me.lbtext)
        Me.Panel3.Controls.Add(Me.swTipoVenta)
        Me.Panel3.Controls.Add(Me.LabelX1)
        Me.Panel3.Controls.Add(Me.tbProveedor)
        Me.Panel3.Controls.Add(Me.tbCodigo)
        Me.Panel3.Controls.Add(Me.LabelX3)
        Me.Panel3.Controls.Add(Me.LabelX2)
        Me.Panel3.Controls.Add(Me.LabelX4)
        Me.Panel3.Controls.Add(Me.tbGlosa)
        Me.Panel3.Controls.Add(Me.LabelX7)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(1487, 216)
        Me.Panel3.TabIndex = 49
        '
        'btnSeleccionarProducto
        '
        Me.btnSeleccionarProducto.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSeleccionarProducto.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnSeleccionarProducto.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSeleccionarProducto.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnSeleccionarProducto.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.btnSeleccionarProducto.Location = New System.Drawing.Point(376, 164)
        Me.btnSeleccionarProducto.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSeleccionarProducto.Name = "btnSeleccionarProducto"
        Me.btnSeleccionarProducto.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnSeleccionarProducto.Size = New System.Drawing.Size(205, 46)
        Me.btnSeleccionarProducto.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSeleccionarProducto.Symbol = "57670"
        Me.btnSeleccionarProducto.SymbolColor = System.Drawing.Color.MediumTurquoise
        Me.btnSeleccionarProducto.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnSeleccionarProducto.SymbolSize = 22.0!
        Me.btnSeleccionarProducto.TabIndex = 211
        Me.btnSeleccionarProducto.Text = "Agregar Productos"
        '
        'cbSucursal
        '
        Me.cbSucursal.BackColor = System.Drawing.Color.Azure
        Me.cbSucursal.ColorScheme = ""
        Me.cbSucursal.ComboStyle = Janus.Windows.GridEX.ComboStyle.DropDownList
        Me.cbSucursal.ControlStyle.ButtonAppearance = Janus.Windows.GridEX.ButtonAppearance.PopUp
        Me.cbSucursal.ControlThemedAreas = Janus.Windows.GridEX.ControlThemedAreas.Button
        cbSucursal_DesignTimeLayout.LayoutString = resources.GetString("cbSucursal_DesignTimeLayout.LayoutString")
        Me.cbSucursal.DesignTimeLayout = cbSucursal_DesignTimeLayout
        Me.cbSucursal.FlatBorderColor = System.Drawing.Color.Black
        Me.cbSucursal.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbSucursal.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbSucursal.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbSucursal.Location = New System.Drawing.Point(377, 31)
        Me.cbSucursal.Margin = New System.Windows.Forms.Padding(4)
        Me.cbSucursal.Name = "cbSucursal"
        Me.cbSucursal.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbSucursal.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbSucursal.SelectedIndex = -1
        Me.cbSucursal.SelectedItem = Nothing
        Me.cbSucursal.Size = New System.Drawing.Size(316, 26)
        Me.cbSucursal.TabIndex = 2
        Me.cbSucursal.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'tbFechaVencimientoCredito
        '
        Me.tbFechaVencimientoCredito.BackColor = System.Drawing.Color.White
        Me.tbFechaVencimientoCredito.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
        Me.tbFechaVencimientoCredito.DropDownCalendar.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.None
        Me.tbFechaVencimientoCredito.DropDownCalendar.DayOfWeekAbbreviation = Janus.Windows.CalendarCombo.DayOfWeekAbbreviation.UseAbbreviatedName
        Me.tbFechaVencimientoCredito.DropDownCalendar.DaysFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbFechaVencimientoCredito.DropDownCalendar.DaysFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbFechaVencimientoCredito.DropDownCalendar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbFechaVencimientoCredito.DropDownCalendar.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbFechaVencimientoCredito.DropDownCalendar.HeaderAppearance = Janus.Windows.CalendarCombo.ButtonAppearance.PopUp
        Me.tbFechaVencimientoCredito.DropDownCalendar.HeaderFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbFechaVencimientoCredito.DropDownCalendar.HeaderFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbFechaVencimientoCredito.DropDownCalendar.Name = ""
        Me.tbFechaVencimientoCredito.DropDownCalendar.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbFechaVencimientoCredito.DropDownCalendar.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbFechaVencimientoCredito.DropDownCalendar.TodayRectColor = System.Drawing.Color.DarkCyan
        Me.tbFechaVencimientoCredito.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbFechaVencimientoCredito.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.tbFechaVencimientoCredito.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbFechaVencimientoCredito.Location = New System.Drawing.Point(376, 135)
        Me.tbFechaVencimientoCredito.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.tbFechaVencimientoCredito.Name = "tbFechaVencimientoCredito"
        Me.tbFechaVencimientoCredito.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbFechaVencimientoCredito.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbFechaVencimientoCredito.SecondIncrement = 10
        Me.tbFechaVencimientoCredito.Size = New System.Drawing.Size(317, 26)
        Me.tbFechaVencimientoCredito.TabIndex = 5
        Me.tbFechaVencimientoCredito.TodayButtonText = "Hoy"
        Me.tbFechaVencimientoCredito.UseCompatibleTextRendering = False
        Me.tbFechaVencimientoCredito.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.tbFechaVencimientoCredito.Visible = False
        Me.tbFechaVencimientoCredito.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbFechaVencimientoCredito.YearIncrement = 10
        '
        'tbFechaTransaccion
        '
        Me.tbFechaTransaccion.BackColor = System.Drawing.Color.White
        Me.tbFechaTransaccion.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.Flat
        '
        '
        '
        Me.tbFechaTransaccion.DropDownCalendar.BorderStyle = Janus.Windows.CalendarCombo.BorderStyle.None
        Me.tbFechaTransaccion.DropDownCalendar.DayOfWeekAbbreviation = Janus.Windows.CalendarCombo.DayOfWeekAbbreviation.UseAbbreviatedName
        Me.tbFechaTransaccion.DropDownCalendar.DaysFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbFechaTransaccion.DropDownCalendar.DaysFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbFechaTransaccion.DropDownCalendar.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbFechaTransaccion.DropDownCalendar.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbFechaTransaccion.DropDownCalendar.HeaderAppearance = Janus.Windows.CalendarCombo.ButtonAppearance.PopUp
        Me.tbFechaTransaccion.DropDownCalendar.HeaderFormatStyle.FontBold = Janus.Windows.CalendarCombo.TriState.[True]
        Me.tbFechaTransaccion.DropDownCalendar.HeaderFormatStyle.ForeColor = System.Drawing.Color.DarkSlateGray
        Me.tbFechaTransaccion.DropDownCalendar.Name = ""
        Me.tbFechaTransaccion.DropDownCalendar.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbFechaTransaccion.DropDownCalendar.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbFechaTransaccion.DropDownCalendar.TodayRectColor = System.Drawing.Color.DarkCyan
        Me.tbFechaTransaccion.DropDownCalendar.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbFechaTransaccion.EditStyle = Janus.Windows.CalendarCombo.EditStyle.Free
        Me.tbFechaTransaccion.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbFechaTransaccion.Location = New System.Drawing.Point(136, 31)
        Me.tbFechaTransaccion.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.tbFechaTransaccion.Name = "tbFechaTransaccion"
        Me.tbFechaTransaccion.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbFechaTransaccion.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbFechaTransaccion.SecondIncrement = 10
        Me.tbFechaTransaccion.Size = New System.Drawing.Size(200, 26)
        Me.tbFechaTransaccion.TabIndex = 3
        Me.tbFechaTransaccion.TodayButtonText = "Hoy"
        Me.tbFechaTransaccion.UseCompatibleTextRendering = False
        Me.tbFechaTransaccion.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.tbFechaTransaccion.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbFechaTransaccion.YearIncrement = 10
        '
        'btnProveedor
        '
        Me.btnProveedor.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnProveedor.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnProveedor.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnProveedor.ImageFixedSize = New System.Drawing.Size(22, 22)
        Me.btnProveedor.Location = New System.Drawing.Point(319, 79)
        Me.btnProveedor.Margin = New System.Windows.Forms.Padding(4)
        Me.btnProveedor.Name = "btnProveedor"
        Me.btnProveedor.Size = New System.Drawing.Size(35, 31)
        Me.btnProveedor.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnProveedor.TabIndex = 210
        Me.btnProveedor.Visible = False
        '
        'lbFechaVencimientoCredito
        '
        Me.lbFechaVencimientoCredito.AutoSize = True
        Me.lbFechaVencimientoCredito.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbFechaVencimientoCredito.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbFechaVencimientoCredito.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbFechaVencimientoCredito.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbFechaVencimientoCredito.Location = New System.Drawing.Point(377, 116)
        Me.lbFechaVencimientoCredito.Margin = New System.Windows.Forms.Padding(4)
        Me.lbFechaVencimientoCredito.Name = "lbFechaVencimientoCredito"
        Me.lbFechaVencimientoCredito.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbFechaVencimientoCredito.Size = New System.Drawing.Size(91, 21)
        Me.lbFechaVencimientoCredito.TabIndex = 56
        Me.lbFechaVencimientoCredito.Text = "Venc. Credito:"
        Me.lbFechaVencimientoCredito.Visible = False
        '
        'lbtext
        '
        Me.lbtext.AutoSize = True
        Me.lbtext.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbtext.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbtext.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbtext.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbtext.Location = New System.Drawing.Point(376, 60)
        Me.lbtext.Margin = New System.Windows.Forms.Padding(4)
        Me.lbtext.Name = "lbtext"
        Me.lbtext.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbtext.Size = New System.Drawing.Size(86, 21)
        Me.lbtext.TabIndex = 54
        Me.lbtext.Text = "Tipo Compra:"
        '
        'swTipoVenta
        '
        '
        '
        '
        Me.swTipoVenta.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.swTipoVenta.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.swTipoVenta.Location = New System.Drawing.Point(376, 82)
        Me.swTipoVenta.Margin = New System.Windows.Forms.Padding(4)
        Me.swTipoVenta.Name = "swTipoVenta"
        Me.swTipoVenta.OffBackColor = System.Drawing.Color.LawnGreen
        Me.swTipoVenta.OffText = "CREDITO"
        Me.swTipoVenta.OnBackColor = System.Drawing.Color.Gold
        Me.swTipoVenta.OnText = "CONTADO"
        Me.swTipoVenta.Size = New System.Drawing.Size(180, 27)
        Me.swTipoVenta.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.swTipoVenta.TabIndex = 4
        Me.swTipoVenta.Value = True
        Me.swTipoVenta.ValueObject = "Y"
        '
        'LabelX1
        '
        Me.LabelX1.AutoSize = True
        Me.LabelX1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX1.Location = New System.Drawing.Point(19, 59)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(72, 21)
        Me.LabelX1.TabIndex = 52
        Me.LabelX1.Text = "Proveedor:"
        '
        'tbProveedor
        '
        '
        '
        '
        Me.tbProveedor.Border.Class = "TextBoxBorder"
        Me.tbProveedor.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbProveedor.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbProveedor.Location = New System.Drawing.Point(19, 82)
        Me.tbProveedor.Margin = New System.Windows.Forms.Padding(4)
        Me.tbProveedor.Name = "tbProveedor"
        Me.tbProveedor.PreventEnterBeep = True
        Me.tbProveedor.Size = New System.Drawing.Size(293, 26)
        Me.tbProveedor.TabIndex = 0
        '
        'tbCodigo
        '
        '
        '
        '
        Me.tbCodigo.Border.Class = "TextBoxBorder"
        Me.tbCodigo.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbCodigo.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCodigo.Location = New System.Drawing.Point(19, 31)
        Me.tbCodigo.Margin = New System.Windows.Forms.Padding(4)
        Me.tbCodigo.Name = "tbCodigo"
        Me.tbCodigo.PreventEnterBeep = True
        Me.tbCodigo.Size = New System.Drawing.Size(109, 26)
        Me.tbCodigo.TabIndex = 35
        Me.tbCodigo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'LabelX3
        '
        Me.LabelX3.AutoSize = True
        Me.LabelX3.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX3.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX3.Location = New System.Drawing.Point(136, 9)
        Me.LabelX3.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX3.Name = "LabelX3"
        Me.LabelX3.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX3.Size = New System.Drawing.Size(95, 21)
        Me.LabelX3.TabIndex = 48
        Me.LabelX3.Text = "Fecha Compra:"
        '
        'LabelX2
        '
        Me.LabelX2.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX2.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX2.Location = New System.Drawing.Point(19, 7)
        Me.LabelX2.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX2.Size = New System.Drawing.Size(99, 28)
        Me.LabelX2.TabIndex = 37
        Me.LabelX2.Text = "Codigo:"
        '
        'LabelX4
        '
        Me.LabelX4.AutoSize = True
        Me.LabelX4.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX4.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX4.Location = New System.Drawing.Point(19, 116)
        Me.LabelX4.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelX4.Name = "LabelX4"
        Me.LabelX4.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX4.Size = New System.Drawing.Size(41, 21)
        Me.LabelX4.TabIndex = 41
        Me.LabelX4.Text = "Glosa:"
        '
        'tbGlosa
        '
        '
        '
        '
        Me.tbGlosa.Border.Class = "TextBoxBorder"
        Me.tbGlosa.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbGlosa.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbGlosa.Location = New System.Drawing.Point(19, 139)
        Me.tbGlosa.Margin = New System.Windows.Forms.Padding(4)
        Me.tbGlosa.Multiline = True
        Me.tbGlosa.Name = "tbGlosa"
        Me.tbGlosa.PreventEnterBeep = True
        Me.tbGlosa.Size = New System.Drawing.Size(317, 71)
        Me.tbGlosa.TabIndex = 1
        '
        'LabelX7
        '
        Me.LabelX7.AutoSize = True
        Me.LabelX7.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX7.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX7.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX7.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX7.Location = New System.Drawing.Point(376, 9)
        Me.LabelX7.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX7.Name = "LabelX7"
        Me.LabelX7.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX7.Size = New System.Drawing.Size(59, 21)
        Me.LabelX7.TabIndex = 44
        Me.LabelX7.Text = "Sucursal:"
        '
        'Panel10
        '
        Me.Panel10.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Panel10.Controls.Add(Me.Panel11)
        Me.Panel10.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel10.Location = New System.Drawing.Point(3, 2)
        Me.Panel10.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel10.Name = "Panel10"
        Me.Panel10.Padding = New System.Windows.Forms.Padding(1)
        Me.Panel10.Size = New System.Drawing.Size(1487, 27)
        Me.Panel10.TabIndex = 1
        '
        'Panel11
        '
        Me.Panel11.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Panel11.Controls.Add(Me.Label3)
        Me.Panel11.Controls.Add(Me.Panel12)
        Me.Panel11.Controls.Add(Me.PictureBox3)
        Me.Panel11.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel11.Location = New System.Drawing.Point(1, 1)
        Me.Panel11.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel11.Name = "Panel11"
        Me.Panel11.Size = New System.Drawing.Size(1485, 25)
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
        Me.Label3.Size = New System.Drawing.Size(253, 25)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Datos De La Compra"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel12
        '
        Me.Panel12.BackColor = System.Drawing.Color.White
        Me.Panel12.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel12.Location = New System.Drawing.Point(59, 0)
        Me.Panel12.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
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
        Me.PictureBox3.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Padding = New System.Windows.Forms.Padding(15, 5, 15, 5)
        Me.PictureBox3.Size = New System.Drawing.Size(59, 25)
        Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox3.TabIndex = 0
        Me.PictureBox3.TabStop = False
        '
        'PanelButton
        '
        Me.PanelButton.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.PanelButton.Controls.Add(Me.PanelToolBar1)
        Me.PanelButton.Controls.Add(Me.PanelNavegacion)
        Me.PanelButton.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelButton.Location = New System.Drawing.Point(0, 715)
        Me.PanelButton.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelButton.Name = "PanelButton"
        Me.PanelButton.Size = New System.Drawing.Size(1493, 50)
        Me.PanelButton.TabIndex = 3
        '
        'PanelToolBar1
        '
        Me.PanelToolBar1.Controls.Add(Me.btnCompra)
        Me.PanelToolBar1.Controls.Add(Me.btnSalir)
        Me.PanelToolBar1.Controls.Add(Me.btnEliminar)
        Me.PanelToolBar1.Controls.Add(Me.btnGrabar)
        Me.PanelToolBar1.Controls.Add(Me.btnModificar)
        Me.PanelToolBar1.Controls.Add(Me.btnNuevo)
        Me.PanelToolBar1.Dock = System.Windows.Forms.DockStyle.Left
        Me.PanelToolBar1.Location = New System.Drawing.Point(0, 0)
        Me.PanelToolBar1.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelToolBar1.Name = "PanelToolBar1"
        Me.PanelToolBar1.Size = New System.Drawing.Size(753, 50)
        Me.PanelToolBar1.TabIndex = 7
        '
        'btnCompra
        '
        Me.btnCompra.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnCompra.BackColor = System.Drawing.Color.Transparent
        Me.btnCompra.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnCompra.DisabledImagesGrayScale = False
        Me.btnCompra.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnCompra.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCompra.Image = Global.TeVendo.My.Resources.Resources.printee
        Me.btnCompra.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnCompra.Location = New System.Drawing.Point(641, 0)
        Me.btnCompra.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCompra.Name = "btnCompra"
        Me.btnCompra.Size = New System.Drawing.Size(112, 50)
        Me.btnCompra.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnCompra.TabIndex = 11
        Me.btnCompra.Text = "Imprimir"
        Me.btnCompra.TextColor = System.Drawing.Color.White
        '
        'btnSalir
        '
        Me.btnSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Magenta
        Me.btnSalir.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnSalir.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSalir.Image = Global.TeVendo.My.Resources.Resources.iconatras
        Me.btnSalir.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnSalir.Location = New System.Drawing.Point(522, 0)
        Me.btnSalir.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSalir.Name = "btnSalir"
        Me.btnSalir.Size = New System.Drawing.Size(93, 50)
        Me.btnSalir.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSalir.TabIndex = 10
        Me.btnSalir.Text = "SALIR"
        Me.btnSalir.TextColor = System.Drawing.Color.White
        '
        'btnEliminar
        '
        Me.btnEliminar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnEliminar.ColorTable = DevComponents.DotNetBar.eButtonColor.Magenta
        Me.btnEliminar.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnEliminar.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEliminar.Image = Global.TeVendo.My.Resources.Resources.iconeliminar
        Me.btnEliminar.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnEliminar.Location = New System.Drawing.Point(391, 0)
        Me.btnEliminar.Margin = New System.Windows.Forms.Padding(4)
        Me.btnEliminar.Name = "btnEliminar"
        Me.btnEliminar.Size = New System.Drawing.Size(131, 50)
        Me.btnEliminar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnEliminar.TabIndex = 8
        Me.btnEliminar.Text = "ELIMINAR"
        Me.btnEliminar.TextColor = System.Drawing.Color.White
        '
        'btnGrabar
        '
        Me.btnGrabar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnGrabar.ColorTable = DevComponents.DotNetBar.eButtonColor.Magenta
        Me.btnGrabar.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnGrabar.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGrabar.Image = Global.TeVendo.My.Resources.Resources.iconguardar
        Me.btnGrabar.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnGrabar.Location = New System.Drawing.Point(260, 0)
        Me.btnGrabar.Margin = New System.Windows.Forms.Padding(4)
        Me.btnGrabar.Name = "btnGrabar"
        Me.btnGrabar.Size = New System.Drawing.Size(131, 50)
        Me.btnGrabar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnGrabar.TabIndex = 9
        Me.btnGrabar.Text = "GRABAR"
        Me.btnGrabar.TextColor = System.Drawing.Color.White
        '
        'btnModificar
        '
        Me.btnModificar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnModificar.ColorTable = DevComponents.DotNetBar.eButtonColor.Magenta
        Me.btnModificar.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnModificar.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnModificar.Image = Global.TeVendo.My.Resources.Resources.iconeditar
        Me.btnModificar.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnModificar.Location = New System.Drawing.Point(120, 0)
        Me.btnModificar.Margin = New System.Windows.Forms.Padding(4)
        Me.btnModificar.Name = "btnModificar"
        Me.btnModificar.Size = New System.Drawing.Size(140, 50)
        Me.btnModificar.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnModificar.TabIndex = 7
        Me.btnModificar.Text = "MODIFICAR"
        Me.btnModificar.TextColor = System.Drawing.Color.White
        '
        'btnNuevo
        '
        Me.btnNuevo.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnNuevo.BackColor = System.Drawing.Color.Transparent
        Me.btnNuevo.ColorTable = DevComponents.DotNetBar.eButtonColor.Magenta
        Me.btnNuevo.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnNuevo.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNuevo.Image = Global.TeVendo.My.Resources.Resources.iconadd
        Me.btnNuevo.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnNuevo.Location = New System.Drawing.Point(0, 0)
        Me.btnNuevo.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNuevo.Name = "btnNuevo"
        Me.btnNuevo.Size = New System.Drawing.Size(120, 50)
        Me.btnNuevo.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnNuevo.TabIndex = 6
        Me.btnNuevo.Text = "NUEVO"
        Me.btnNuevo.TextColor = System.Drawing.Color.White
        '
        'PanelNavegacion
        '
        Me.PanelNavegacion.Controls.Add(Me.LblPaginacion)
        Me.PanelNavegacion.Controls.Add(Me.btnUltimo)
        Me.PanelNavegacion.Controls.Add(Me.btnSiguiente)
        Me.PanelNavegacion.Controls.Add(Me.btnAnterior)
        Me.PanelNavegacion.Controls.Add(Me.btnPrimero)
        Me.PanelNavegacion.Dock = System.Windows.Forms.DockStyle.Right
        Me.PanelNavegacion.Location = New System.Drawing.Point(1026, 0)
        Me.PanelNavegacion.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelNavegacion.Name = "PanelNavegacion"
        Me.PanelNavegacion.Size = New System.Drawing.Size(467, 50)
        Me.PanelNavegacion.TabIndex = 21
        '
        'LblPaginacion
        '
        Me.LblPaginacion.BackColor = System.Drawing.Color.White
        Me.LblPaginacion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblPaginacion.Font = New System.Drawing.Font("Arial", 22.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblPaginacion.ForeColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.LblPaginacion.Location = New System.Drawing.Point(276, 0)
        Me.LblPaginacion.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LblPaginacion.Name = "LblPaginacion"
        Me.LblPaginacion.Size = New System.Drawing.Size(191, 50)
        Me.LblPaginacion.TabIndex = 22
        Me.LblPaginacion.Text = "0/0"
        Me.LblPaginacion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnUltimo
        '
        Me.btnUltimo.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnUltimo.ColorTable = DevComponents.DotNetBar.eButtonColor.Orange
        Me.btnUltimo.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnUltimo.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnUltimo.Image = Global.TeVendo.My.Resources.Resources.derechaDoble
        Me.btnUltimo.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnUltimo.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.btnUltimo.Location = New System.Drawing.Point(207, 0)
        Me.btnUltimo.Margin = New System.Windows.Forms.Padding(4)
        Me.btnUltimo.Name = "btnUltimo"
        Me.btnUltimo.Size = New System.Drawing.Size(69, 50)
        Me.btnUltimo.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnUltimo.TabIndex = 14
        '
        'btnSiguiente
        '
        Me.btnSiguiente.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSiguiente.ColorTable = DevComponents.DotNetBar.eButtonColor.Orange
        Me.btnSiguiente.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnSiguiente.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSiguiente.Image = Global.TeVendo.My.Resources.Resources.derechaLine
        Me.btnSiguiente.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnSiguiente.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.btnSiguiente.Location = New System.Drawing.Point(138, 0)
        Me.btnSiguiente.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSiguiente.Name = "btnSiguiente"
        Me.btnSiguiente.Size = New System.Drawing.Size(69, 50)
        Me.btnSiguiente.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSiguiente.TabIndex = 13
        '
        'btnAnterior
        '
        Me.btnAnterior.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnAnterior.ColorTable = DevComponents.DotNetBar.eButtonColor.Orange
        Me.btnAnterior.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnAnterior.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAnterior.Image = Global.TeVendo.My.Resources.Resources.back_1
        Me.btnAnterior.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnAnterior.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.btnAnterior.Location = New System.Drawing.Point(69, 0)
        Me.btnAnterior.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAnterior.Name = "btnAnterior"
        Me.btnAnterior.Size = New System.Drawing.Size(69, 50)
        Me.btnAnterior.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnAnterior.TabIndex = 12
        '
        'btnPrimero
        '
        Me.btnPrimero.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnPrimero.ColorTable = DevComponents.DotNetBar.eButtonColor.Orange
        Me.btnPrimero.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnPrimero.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrimero.Image = Global.TeVendo.My.Resources.Resources.izquierda2
        Me.btnPrimero.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnPrimero.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.btnPrimero.Location = New System.Drawing.Point(0, 0)
        Me.btnPrimero.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPrimero.Name = "btnPrimero"
        Me.btnPrimero.Size = New System.Drawing.Size(69, 50)
        Me.btnPrimero.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnPrimero.TabIndex = 11
        '
        'SuperTabItem1
        '
        Me.SuperTabItem1.AttachedControl = Me.SuperTabControlPanel1
        Me.SuperTabItem1.GlobalItem = False
        Me.SuperTabItem1.Name = "SuperTabItem1"
        Me.SuperTabItem1.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Gold
        Me.SuperTabItem1.TabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabItem1.Text = "Detalle De Datos"
        Me.SuperTabItem1.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Center
        '
        'MEP
        '
        Me.MEP.ContainerControl = Me
        '
        'Tec_Compras
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1493, 796)
        Me.Controls.Add(Me.TabControlPrincipal)
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Name = "Tec_Compras"
        Me.Text = "Tec_Compras"
        CType(Me.TabControlPrincipal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControlPrincipal.ResumeLayout(False)
        Me.SuperTabControlPanel2.ResumeLayout(False)
        Me.Panel17.ResumeLayout(False)
        CType(Me.JGrM_Buscador, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MeuOpciones.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.Panel21.ResumeLayout(False)
        Me.Panel21.PerformLayout()
        Me.btnSi.ResumeLayout(False)
        Me.Panel14.ResumeLayout(False)
        Me.Panel15.ResumeLayout(False)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SuperTabControlPanel1.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.PanelSuperior.ResumeLayout(False)
        Me.PanelRight.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.PanelImagenes.ResumeLayout(False)
        Me.PanelVerImagen.ResumeLayout(False)
        CType(Me.grDetalle, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelTotal.ResumeLayout(False)
        Me.Panel13.ResumeLayout(False)
        Me.Panel13.PerformLayout()
        CType(Me.tbTotal, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbMdesc, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbPdesc, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel5.ResumeLayout(False)
        Me.Panel6.ResumeLayout(False)
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelLEft.ResumeLayout(False)
        Me.Panel8.ResumeLayout(False)
        Me.Panel9.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        CType(Me.cbSucursal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelButton.ResumeLayout(False)
        Me.PanelToolBar1.ResumeLayout(False)
        Me.PanelNavegacion.ResumeLayout(False)
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabControlPrincipal As DevComponents.DotNetBar.SuperTabControl
    Friend WithEvents SuperTabControlPanel1 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents Panel1 As Panel
    Friend WithEvents PanelSuperior As Panel
    Friend WithEvents PanelRight As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents PanelImagenes As Panel
    Friend WithEvents PanelVerImagen As Panel
    Friend WithEvents grDetalle As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel5 As Panel
    Friend WithEvents Panel6 As Panel
    Friend WithEvents lbprivilegio As Label
    Friend WithEvents Panel7 As Panel
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents PanelLEft As Panel
    Friend WithEvents Panel8 As Panel
    Friend WithEvents Panel9 As Panel
    Friend WithEvents Panel10 As Panel
    Friend WithEvents Panel11 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Panel12 As Panel
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents PanelButton As Panel
    Protected WithEvents PanelToolBar1 As Panel
    Protected WithEvents btnSalir As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnEliminar As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnGrabar As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnModificar As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnNuevo As DevComponents.DotNetBar.ButtonX
    Protected WithEvents PanelNavegacion As Panel
    Protected WithEvents LblPaginacion As Label
    Protected WithEvents btnUltimo As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnSiguiente As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnAnterior As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnPrimero As DevComponents.DotNetBar.ButtonX
    Friend WithEvents SuperTabItem1 As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents SuperTabControlPanel2 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents Panel17 As Panel
    Friend WithEvents JGrM_Buscador As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel4 As Panel
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents btnSi As Panel
    Protected WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents Panel14 As Panel
    Friend WithEvents Panel15 As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents Panel16 As Panel
    Friend WithEvents PictureBox4 As PictureBox
    Friend WithEvents SuperTabItem2 As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents Panel3 As Panel
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbProveedor As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents tbCodigo As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX3 As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX4 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbGlosa As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX7 As DevComponents.DotNetBar.LabelX
    Friend WithEvents swTipoVenta As DevComponents.DotNetBar.Controls.SwitchButton
    Friend WithEvents lbtext As DevComponents.DotNetBar.LabelX
    Friend WithEvents lbFechaVencimientoCredito As DevComponents.DotNetBar.LabelX
    Friend WithEvents PanelTotal As Panel
    Friend WithEvents Panel13 As Panel
    Friend WithEvents tbPdesc As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX8 As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX10 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbTotal As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX9 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbMdesc As DevComponents.Editors.DoubleInput
    Protected WithEvents MEP As ErrorProvider
    Protected WithEvents MHighlighterFocus As DevComponents.DotNetBar.Validator.Highlighter
    Friend WithEvents btnProveedor As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnCompra As DevComponents.DotNetBar.ButtonX
    Friend WithEvents tbFechaVencimientoCredito As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents tbFechaTransaccion As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents cbSucursal As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents MeuOpciones As ContextMenuStrip
    Friend WithEvents VerToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents EditarToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EliminarToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents btnSeleccionarProducto As DevComponents.DotNetBar.ButtonX
    Friend WithEvents Panel21 As Panel
    Friend WithEvents btnFiltrarVentas As DevComponents.DotNetBar.ButtonX
    Friend WithEvents tbHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX16 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX6 As DevComponents.DotNetBar.LabelX
End Class
