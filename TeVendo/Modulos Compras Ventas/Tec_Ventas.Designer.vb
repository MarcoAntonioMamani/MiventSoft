<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Tec_Ventas
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Tec_Ventas))
        Me.TabControlPrincipal = New DevComponents.DotNetBar.SuperTabControl()
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
        Me.SuperTabControl1 = New DevComponents.DotNetBar.SuperTabControl()
        Me.SuperTabControlPanel3 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.cbSucursal = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.tbFechaVencimientoCredito = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.tbFechaTransaccion = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.btnSeleccionarProducto = New DevComponents.DotNetBar.ButtonX()
        Me.btnCliente = New DevComponents.DotNetBar.ButtonX()
        Me.LabelX11 = New DevComponents.DotNetBar.LabelX()
        Me.tbCliente = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.btnVendedor = New DevComponents.DotNetBar.ButtonX()
        Me.lbcredito = New DevComponents.DotNetBar.LabelX()
        Me.LabelX5 = New DevComponents.DotNetBar.LabelX()
        Me.swTipoVenta = New DevComponents.DotNetBar.Controls.SwitchButton()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.tbVendedor = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.tbCodigo = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.LabelX3 = New DevComponents.DotNetBar.LabelX()
        Me.LabelX2 = New DevComponents.DotNetBar.LabelX()
        Me.LabelX4 = New DevComponents.DotNetBar.LabelX()
        Me.tbGlosa = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.LabelX7 = New DevComponents.DotNetBar.LabelX()
        Me.SuperTabItem3 = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel4 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel20 = New System.Windows.Forms.Panel()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.tbCambio = New DevComponents.Editors.DoubleInput()
        Me.LabelX17 = New DevComponents.DotNetBar.LabelX()
        Me.tbTotalPagado = New DevComponents.Editors.DoubleInput()
        Me.LabelX18 = New DevComponents.DotNetBar.LabelX()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lbTipoCambio = New DevComponents.DotNetBar.LabelX()
        Me.tbTransferencia = New DevComponents.Editors.DoubleInput()
        Me.LabelX15 = New DevComponents.DotNetBar.LabelX()
        Me.tbTarjeta = New DevComponents.Editors.DoubleInput()
        Me.LabelX14 = New DevComponents.DotNetBar.LabelX()
        Me.tbMontoBs = New DevComponents.Editors.DoubleInput()
        Me.LabelX12 = New DevComponents.DotNetBar.LabelX()
        Me.tbMontoDolar = New DevComponents.Editors.DoubleInput()
        Me.LabelX13 = New DevComponents.DotNetBar.LabelX()
        Me.Panel18 = New System.Windows.Forms.Panel()
        Me.tab_Cobro = New DevComponents.DotNetBar.SuperTabItem()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Panel11 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel12 = New System.Windows.Forms.Panel()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.PanelButton = New System.Windows.Forms.Panel()
        Me.PanelToolBar1 = New System.Windows.Forms.Panel()
        Me.ButtonX3 = New DevComponents.DotNetBar.ButtonX()
        Me.BtnImprimir = New DevComponents.DotNetBar.ButtonX()
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
        Me.SuperTabControlPanel2 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.Panel17 = New System.Windows.Forms.Panel()
        Me.JGrM_Buscador = New Janus.Windows.GridEX.GridEX()
        Me.MeuOpciones = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.VerToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EliminarToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnImprimirCopia = New DevComponents.DotNetBar.ButtonX()
        Me.Panel21 = New System.Windows.Forms.Panel()
        Me.btnFiltrarVentas = New DevComponents.DotNetBar.ButtonX()
        Me.tbHasta = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX16 = New DevComponents.DotNetBar.LabelX()
        Me.tbDesde = New Janus.Windows.CalendarCombo.CalendarCombo()
        Me.LabelX6 = New DevComponents.DotNetBar.LabelX()
        Me.Panel19 = New System.Windows.Forms.Panel()
        Me.ButtonX2 = New DevComponents.DotNetBar.ButtonX()
        Me.btnSi = New System.Windows.Forms.Panel()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.Panel14 = New System.Windows.Forms.Panel()
        Me.Panel15 = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Panel16 = New System.Windows.Forms.Panel()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.SuperTabItem2 = New DevComponents.DotNetBar.SuperTabItem()
        Me.MHighlighterFocus = New DevComponents.DotNetBar.Validator.Highlighter()
        Me.MEP = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.GroupPanel1 = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel22 = New System.Windows.Forms.Panel()
        Me.LabelX19 = New DevComponents.DotNetBar.LabelX()
        Me.tbnit = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.LabelX20 = New DevComponents.DotNetBar.LabelX()
        Me.TextBoxX1 = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.SuperTabItem4 = New DevComponents.DotNetBar.SuperTabItem()
        Me.SuperTabControlPanel5 = New DevComponents.DotNetBar.SuperTabControlPanel()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.LabelX22 = New DevComponents.DotNetBar.LabelX()
        CType(Me.TabControlPrincipal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControlPrincipal.SuspendLayout()
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
        CType(Me.SuperTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuperTabControl1.SuspendLayout()
        Me.SuperTabControlPanel3.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.cbSucursal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuperTabControlPanel4.SuspendLayout()
        Me.Panel20.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.tbCambio, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbTotalPagado, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.tbTransferencia, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbTarjeta, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbMontoBs, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbMontoDolar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel10.SuspendLayout()
        Me.Panel11.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelButton.SuspendLayout()
        Me.PanelToolBar1.SuspendLayout()
        Me.PanelNavegacion.SuspendLayout()
        Me.SuperTabControlPanel2.SuspendLayout()
        Me.Panel17.SuspendLayout()
        CType(Me.JGrM_Buscador, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MeuOpciones.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.Panel21.SuspendLayout()
        Me.Panel19.SuspendLayout()
        Me.btnSi.SuspendLayout()
        Me.Panel14.SuspendLayout()
        Me.Panel15.SuspendLayout()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupPanel1.SuspendLayout()
        Me.Panel22.SuspendLayout()
        Me.SuperTabControlPanel5.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
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
        Me.TabControlPrincipal.Controls.Add(Me.SuperTabControlPanel1)
        Me.TabControlPrincipal.Controls.Add(Me.SuperTabControlPanel2)
        Me.TabControlPrincipal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlPrincipal.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControlPrincipal.Location = New System.Drawing.Point(0, 0)
        Me.TabControlPrincipal.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.TabControlPrincipal.Name = "TabControlPrincipal"
        Me.TabControlPrincipal.ReorderTabsEnabled = True
        Me.TabControlPrincipal.SelectedTabFont = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControlPrincipal.SelectedTabIndex = 1
        Me.TabControlPrincipal.Size = New System.Drawing.Size(1371, 625)
        Me.TabControlPrincipal.TabAlignment = DevComponents.DotNetBar.eTabStripAlignment.Bottom
        Me.TabControlPrincipal.TabFont = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControlPrincipal.TabIndex = 3
        Me.TabControlPrincipal.TabLayoutType = DevComponents.DotNetBar.eSuperTabLayoutType.MultiLineFit
        Me.TabControlPrincipal.Tabs.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.SuperTabItem1, Me.SuperTabItem2})
        Me.TabControlPrincipal.TabStyle = DevComponents.DotNetBar.eSuperTabStyle.WinMediaPlayer12
        Me.TabControlPrincipal.Text = "Datos"
        '
        'SuperTabControlPanel1
        '
        Me.SuperTabControlPanel1.CanvasColor = System.Drawing.Color.Silver
        Me.SuperTabControlPanel1.Controls.Add(Me.Panel1)
        Me.SuperTabControlPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel1.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabControlPanel1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.SuperTabControlPanel1.Name = "SuperTabControlPanel1"
        Me.SuperTabControlPanel1.Size = New System.Drawing.Size(1371, 596)
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
        Me.Panel1.Size = New System.Drawing.Size(1371, 596)
        Me.Panel1.TabIndex = 0
        '
        'PanelSuperior
        '
        Me.PanelSuperior.Controls.Add(Me.PanelLEft)
        Me.PanelSuperior.Controls.Add(Me.PanelRight)
        Me.PanelSuperior.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelSuperior.Location = New System.Drawing.Point(0, 0)
        Me.PanelSuperior.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelSuperior.Name = "PanelSuperior"
        Me.PanelSuperior.Size = New System.Drawing.Size(1371, 546)
        Me.PanelSuperior.TabIndex = 1
        '
        'PanelRight
        '
        Me.PanelRight.Controls.Add(Me.Panel2)
        Me.PanelRight.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelRight.Location = New System.Drawing.Point(0, 0)
        Me.PanelRight.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelRight.Name = "PanelRight"
        Me.PanelRight.Size = New System.Drawing.Size(1371, 357)
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
        Me.Panel2.Size = New System.Drawing.Size(1371, 357)
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
        Me.PanelImagenes.Size = New System.Drawing.Size(1365, 326)
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
        Me.PanelVerImagen.Size = New System.Drawing.Size(1365, 326)
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
        Me.grDetalle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.GridLines = Janus.Windows.GridEX.GridLines.None
        Me.grDetalle.GridLineStyle = Janus.Windows.GridEX.GridLineStyle.Solid
        Me.grDetalle.HeaderFormatStyle.Alpha = 0
        Me.grDetalle.HeaderFormatStyle.Appearance = Janus.Windows.GridEX.Appearance.Flat
        Me.grDetalle.HeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetalle.HeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.UseAlpha
        Me.grDetalle.HeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetalle.HeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetalle.HeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.HeaderFormatStyle.ForeColor = System.Drawing.Color.Black
        Me.grDetalle.Location = New System.Drawing.Point(0, 0)
        Me.grDetalle.Margin = New System.Windows.Forms.Padding(4)
        Me.grDetalle.Name = "grDetalle"
        Me.grDetalle.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grDetalle.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grDetalle.RowFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.RowHeaderFormatStyle.BackColor = System.Drawing.Color.MidnightBlue
        Me.grDetalle.RowHeaderFormatStyle.BackColorAlphaMode = Janus.Windows.GridEX.AlphaMode.Transparent
        Me.grDetalle.RowHeaderFormatStyle.BackColorGradient = System.Drawing.Color.MidnightBlue
        Me.grDetalle.RowHeaderFormatStyle.BackgroundGradientMode = Janus.Windows.GridEX.BackgroundGradientMode.DiagonalBackwards
        Me.grDetalle.RowHeaderFormatStyle.BackgroundImageDrawMode = Janus.Windows.GridEX.BackgroundImageDrawMode.Center
        Me.grDetalle.RowHeaderFormatStyle.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.RowHeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(94, Byte), Integer))
        Me.grDetalle.SelectedFormatStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(92, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.grDetalle.SelectedFormatStyle.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grDetalle.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.grDetalle.Size = New System.Drawing.Size(1156, 326)
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
        Me.PanelTotal.Location = New System.Drawing.Point(1156, 0)
        Me.PanelTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelTotal.Name = "PanelTotal"
        Me.PanelTotal.Size = New System.Drawing.Size(209, 326)
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
        Me.Panel13.Size = New System.Drawing.Size(209, 326)
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
        Me.LabelX10.Location = New System.Drawing.Point(8, 118)
        Me.LabelX10.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX10.Name = "LabelX10"
        Me.LabelX10.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX10.Size = New System.Drawing.Size(79, 21)
        Me.LabelX10.TabIndex = 53
        Me.LabelX10.Text = "Total Venta:"
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
        Me.tbTotal.Font = New System.Drawing.Font("Calibri", 10.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTotal.Increment = 1.0R
        Me.tbTotal.Location = New System.Drawing.Point(9, 143)
        Me.tbTotal.LockUpdateChecked = False
        Me.tbTotal.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTotal.MinValue = 0R
        Me.tbTotal.Name = "tbTotal"
        Me.tbTotal.Size = New System.Drawing.Size(173, 29)
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
        Me.LabelX9.Location = New System.Drawing.Point(8, 63)
        Me.LabelX9.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX9.Name = "LabelX9"
        Me.LabelX9.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX9.Size = New System.Drawing.Size(119, 21)
        Me.LabelX9.TabIndex = 51
        Me.LabelX9.Text = "Monto Descuento:"
        '
        'tbMdesc
        '
        Me.tbMdesc.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
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
        Me.tbMdesc.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMdesc.Increment = 1.0R
        Me.tbMdesc.Location = New System.Drawing.Point(75, 85)
        Me.tbMdesc.LockUpdateChecked = False
        Me.tbMdesc.Margin = New System.Windows.Forms.Padding(4)
        Me.tbMdesc.MinValue = 0R
        Me.tbMdesc.Name = "tbMdesc"
        Me.tbMdesc.Size = New System.Drawing.Size(108, 28)
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
        Me.LabelX8.Location = New System.Drawing.Point(8, 7)
        Me.LabelX8.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX8.Name = "LabelX8"
        Me.LabelX8.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX8.Size = New System.Drawing.Size(143, 21)
        Me.LabelX8.TabIndex = 49
        Me.LabelX8.Text = "Porcentaje Descuento:"
        '
        'tbPdesc
        '
        Me.tbPdesc.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
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
        Me.tbPdesc.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbPdesc.Increment = 1.0R
        Me.tbPdesc.Location = New System.Drawing.Point(75, 31)
        Me.tbPdesc.LockUpdateChecked = False
        Me.tbPdesc.Margin = New System.Windows.Forms.Padding(4)
        Me.tbPdesc.MinValue = 0R
        Me.tbPdesc.Name = "tbPdesc"
        Me.tbPdesc.Size = New System.Drawing.Size(108, 28)
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
        Me.Panel5.Size = New System.Drawing.Size(1365, 27)
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
        Me.Panel6.Size = New System.Drawing.Size(1363, 25)
        Me.Panel6.TabIndex = 0
        '
        'lbprivilegio
        '
        Me.lbprivilegio.Dock = System.Windows.Forms.DockStyle.Left
        Me.lbprivilegio.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
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
        Me.PanelLEft.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelLEft.Location = New System.Drawing.Point(0, 357)
        Me.PanelLEft.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelLEft.Name = "PanelLEft"
        Me.PanelLEft.Size = New System.Drawing.Size(1371, 189)
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
        Me.Panel8.Size = New System.Drawing.Size(1371, 189)
        Me.Panel8.TabIndex = 1
        '
        'Panel9
        '
        Me.Panel9.BackColor = System.Drawing.Color.White
        Me.Panel9.Controls.Add(Me.SuperTabControl1)
        Me.Panel9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel9.Location = New System.Drawing.Point(3, 2)
        Me.Panel9.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel9.Name = "Panel9"
        Me.Panel9.Size = New System.Drawing.Size(1365, 185)
        Me.Panel9.TabIndex = 2
        '
        'SuperTabControl1
        '
        '
        '
        '
        '
        '
        '
        Me.SuperTabControl1.ControlBox.CloseBox.Name = ""
        '
        '
        '
        Me.SuperTabControl1.ControlBox.MenuBox.Name = ""
        Me.SuperTabControl1.ControlBox.Name = ""
        Me.SuperTabControl1.ControlBox.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.SuperTabControl1.ControlBox.MenuBox, Me.SuperTabControl1.ControlBox.CloseBox})
        Me.SuperTabControl1.Controls.Add(Me.SuperTabControlPanel5)
        Me.SuperTabControl1.Controls.Add(Me.SuperTabControlPanel4)
        Me.SuperTabControl1.Controls.Add(Me.SuperTabControlPanel3)
        Me.SuperTabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControl1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabControl1.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabControl1.Name = "SuperTabControl1"
        Me.SuperTabControl1.ReorderTabsEnabled = True
        Me.SuperTabControl1.SelectedTabFont = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabControl1.SelectedTabIndex = 0
        Me.SuperTabControl1.Size = New System.Drawing.Size(1365, 185)
        Me.SuperTabControl1.TabAlignment = DevComponents.DotNetBar.eTabStripAlignment.Right
        Me.SuperTabControl1.TabFont = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabControl1.TabIndex = 218
        Me.SuperTabControl1.Tabs.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.SuperTabItem3, Me.tab_Cobro, Me.SuperTabItem4})
        Me.SuperTabControl1.Text = "SuperTabControl1"
        '
        'SuperTabControlPanel3
        '
        Me.SuperTabControlPanel3.Controls.Add(Me.Panel3)
        Me.SuperTabControlPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel3.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabControlPanel3.Name = "SuperTabControlPanel3"
        Me.SuperTabControlPanel3.Size = New System.Drawing.Size(1231, 185)
        Me.SuperTabControlPanel3.TabIndex = 1
        Me.SuperTabControlPanel3.TabItem = Me.SuperTabItem3
        '
        'Panel3
        '
        Me.Panel3.AutoScroll = True
        Me.Panel3.Controls.Add(Me.GroupPanel1)
        Me.Panel3.Controls.Add(Me.cbSucursal)
        Me.Panel3.Controls.Add(Me.tbFechaVencimientoCredito)
        Me.Panel3.Controls.Add(Me.tbFechaTransaccion)
        Me.Panel3.Controls.Add(Me.btnSeleccionarProducto)
        Me.Panel3.Controls.Add(Me.btnCliente)
        Me.Panel3.Controls.Add(Me.LabelX11)
        Me.Panel3.Controls.Add(Me.tbCliente)
        Me.Panel3.Controls.Add(Me.btnVendedor)
        Me.Panel3.Controls.Add(Me.lbcredito)
        Me.Panel3.Controls.Add(Me.LabelX5)
        Me.Panel3.Controls.Add(Me.swTipoVenta)
        Me.Panel3.Controls.Add(Me.LabelX1)
        Me.Panel3.Controls.Add(Me.tbVendedor)
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
        Me.Panel3.Size = New System.Drawing.Size(1231, 185)
        Me.Panel3.TabIndex = 49
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
        Me.cbSucursal.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbSucursal.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.cbSucursal.HideSelection = False
        Me.cbSucursal.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cbSucursal.Location = New System.Drawing.Point(708, 25)
        Me.cbSucursal.Margin = New System.Windows.Forms.Padding(4)
        Me.cbSucursal.Name = "cbSucursal"
        Me.cbSucursal.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbSucursal.Office2007CustomColor = System.Drawing.Color.MediumTurquoise
        Me.cbSucursal.SelectedIndex = -1
        Me.cbSucursal.SelectedItem = Nothing
        Me.cbSucursal.Size = New System.Drawing.Size(219, 28)
        Me.cbSucursal.TabIndex = 217
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
        Me.tbFechaVencimientoCredito.Location = New System.Drawing.Point(443, 132)
        Me.tbFechaVencimientoCredito.Name = "tbFechaVencimientoCredito"
        Me.tbFechaVencimientoCredito.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbFechaVencimientoCredito.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbFechaVencimientoCredito.SecondIncrement = 10
        Me.tbFechaVencimientoCredito.Size = New System.Drawing.Size(211, 26)
        Me.tbFechaVencimientoCredito.TabIndex = 216
        Me.tbFechaVencimientoCredito.TodayButtonText = "Hoy"
        Me.tbFechaVencimientoCredito.UseCompatibleTextRendering = False
        Me.tbFechaVencimientoCredito.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
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
        Me.tbFechaTransaccion.Location = New System.Drawing.Point(157, 25)
        Me.tbFechaTransaccion.Name = "tbFechaTransaccion"
        Me.tbFechaTransaccion.Office2007ColorScheme = Janus.Windows.CalendarCombo.Office2007ColorScheme.Custom
        Me.tbFechaTransaccion.Office2007CustomColor = System.Drawing.Color.Turquoise
        Me.tbFechaTransaccion.SecondIncrement = 10
        Me.tbFechaTransaccion.Size = New System.Drawing.Size(138, 26)
        Me.tbFechaTransaccion.TabIndex = 215
        Me.tbFechaTransaccion.TodayButtonText = "Hoy"
        Me.tbFechaTransaccion.UseCompatibleTextRendering = False
        Me.tbFechaTransaccion.Value = New Date(2020, 6, 21, 0, 0, 0, 0)
        Me.tbFechaTransaccion.VisualStyle = Janus.Windows.CalendarCombo.VisualStyle.Office2007
        Me.tbFechaTransaccion.YearIncrement = 10
        '
        'btnSeleccionarProducto
        '
        Me.btnSeleccionarProducto.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSeleccionarProducto.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnSeleccionarProducto.Font = New System.Drawing.Font("Calibri", 10.2!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSeleccionarProducto.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnSeleccionarProducto.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.btnSeleccionarProducto.Location = New System.Drawing.Point(951, 10)
        Me.btnSeleccionarProducto.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSeleccionarProducto.Name = "btnSeleccionarProducto"
        Me.btnSeleccionarProducto.Shape = New DevComponents.DotNetBar.RoundRectangleShapeDescriptor(2)
        Me.btnSeleccionarProducto.Size = New System.Drawing.Size(205, 43)
        Me.btnSeleccionarProducto.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnSeleccionarProducto.Symbol = "57670"
        Me.btnSeleccionarProducto.SymbolColor = System.Drawing.Color.MediumTurquoise
        Me.btnSeleccionarProducto.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material
        Me.btnSeleccionarProducto.SymbolSize = 24.0!
        Me.btnSeleccionarProducto.TabIndex = 214
        Me.btnSeleccionarProducto.Text = "Agregar Productos"
        '
        'btnCliente
        '
        Me.btnCliente.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnCliente.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnCliente.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnCliente.ImageFixedSize = New System.Drawing.Size(20, 20)
        Me.btnCliente.Location = New System.Drawing.Point(301, 128)
        Me.btnCliente.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCliente.Name = "btnCliente"
        Me.btnCliente.Size = New System.Drawing.Size(35, 28)
        Me.btnCliente.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnCliente.TabIndex = 213
        Me.btnCliente.Visible = False
        '
        'LabelX11
        '
        Me.LabelX11.AutoSize = True
        Me.LabelX11.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX11.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX11.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX11.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX11.Location = New System.Drawing.Point(16, 106)
        Me.LabelX11.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX11.Name = "LabelX11"
        Me.LabelX11.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX11.Size = New System.Drawing.Size(50, 21)
        Me.LabelX11.TabIndex = 212
        Me.LabelX11.Text = "Cliente:"
        '
        'tbCliente
        '
        '
        '
        '
        Me.tbCliente.Border.Class = "TextBoxBorder"
        Me.tbCliente.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbCliente.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCliente.Location = New System.Drawing.Point(17, 130)
        Me.tbCliente.Margin = New System.Windows.Forms.Padding(4)
        Me.tbCliente.Name = "tbCliente"
        Me.tbCliente.PreventEnterBeep = True
        Me.tbCliente.Size = New System.Drawing.Size(277, 26)
        Me.tbCliente.TabIndex = 211
        '
        'btnVendedor
        '
        Me.btnVendedor.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnVendedor.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnVendedor.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnVendedor.ImageFixedSize = New System.Drawing.Size(20, 20)
        Me.btnVendedor.Location = New System.Drawing.Point(301, 74)
        Me.btnVendedor.Margin = New System.Windows.Forms.Padding(4)
        Me.btnVendedor.Name = "btnVendedor"
        Me.btnVendedor.Size = New System.Drawing.Size(35, 28)
        Me.btnVendedor.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnVendedor.TabIndex = 210
        Me.btnVendedor.Visible = False
        '
        'lbcredito
        '
        Me.lbcredito.AutoSize = True
        Me.lbcredito.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbcredito.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbcredito.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbcredito.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lbcredito.Location = New System.Drawing.Point(345, 134)
        Me.lbcredito.Margin = New System.Windows.Forms.Padding(4)
        Me.lbcredito.Name = "lbcredito"
        Me.lbcredito.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbcredito.Size = New System.Drawing.Size(91, 21)
        Me.lbcredito.TabIndex = 56
        Me.lbcredito.Text = "Venc. Credito:"
        '
        'LabelX5
        '
        Me.LabelX5.AutoSize = True
        Me.LabelX5.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX5.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX5.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX5.Location = New System.Drawing.Point(345, 81)
        Me.LabelX5.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX5.Name = "LabelX5"
        Me.LabelX5.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX5.Size = New System.Drawing.Size(74, 21)
        Me.LabelX5.TabIndex = 54
        Me.LabelX5.Text = "Tipo Venta:"
        '
        'swTipoVenta
        '
        '
        '
        '
        Me.swTipoVenta.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.swTipoVenta.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.swTipoVenta.Location = New System.Drawing.Point(345, 103)
        Me.swTipoVenta.Margin = New System.Windows.Forms.Padding(4)
        Me.swTipoVenta.Name = "swTipoVenta"
        Me.swTipoVenta.OffBackColor = System.Drawing.Color.LawnGreen
        Me.swTipoVenta.OffText = "CREDITO"
        Me.swTipoVenta.OnBackColor = System.Drawing.Color.Gold
        Me.swTipoVenta.OnText = "CONTADO"
        Me.swTipoVenta.Size = New System.Drawing.Size(309, 22)
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
        Me.LabelX1.Location = New System.Drawing.Point(16, 53)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(68, 21)
        Me.LabelX1.TabIndex = 52
        Me.LabelX1.Text = "Vendedor:"
        '
        'tbVendedor
        '
        '
        '
        '
        Me.tbVendedor.Border.Class = "TextBoxBorder"
        Me.tbVendedor.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbVendedor.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbVendedor.Location = New System.Drawing.Point(16, 76)
        Me.tbVendedor.Margin = New System.Windows.Forms.Padding(4)
        Me.tbVendedor.Name = "tbVendedor"
        Me.tbVendedor.PreventEnterBeep = True
        Me.tbVendedor.Size = New System.Drawing.Size(279, 26)
        Me.tbVendedor.TabIndex = 0
        '
        'tbCodigo
        '
        '
        '
        '
        Me.tbCodigo.Border.Class = "TextBoxBorder"
        Me.tbCodigo.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbCodigo.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCodigo.Location = New System.Drawing.Point(17, 25)
        Me.tbCodigo.Margin = New System.Windows.Forms.Padding(4)
        Me.tbCodigo.Name = "tbCodigo"
        Me.tbCodigo.PreventEnterBeep = True
        Me.tbCodigo.Size = New System.Drawing.Size(133, 26)
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
        Me.LabelX3.Location = New System.Drawing.Point(157, 2)
        Me.LabelX3.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX3.Name = "LabelX3"
        Me.LabelX3.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX3.Size = New System.Drawing.Size(120, 21)
        Me.LabelX3.TabIndex = 48
        Me.LabelX3.Text = "Fecha Transaccion:"
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
        Me.LabelX2.Location = New System.Drawing.Point(16, 0)
        Me.LabelX2.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX2.Name = "LabelX2"
        Me.LabelX2.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX2.Size = New System.Drawing.Size(73, 28)
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
        Me.LabelX4.Location = New System.Drawing.Point(345, 1)
        Me.LabelX4.Margin = New System.Windows.Forms.Padding(4)
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
        Me.tbGlosa.Font = New System.Drawing.Font("Calibri", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbGlosa.Location = New System.Drawing.Point(345, 25)
        Me.tbGlosa.Margin = New System.Windows.Forms.Padding(4)
        Me.tbGlosa.Multiline = True
        Me.tbGlosa.Name = "tbGlosa"
        Me.tbGlosa.PreventEnterBeep = True
        Me.tbGlosa.Size = New System.Drawing.Size(309, 52)
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
        Me.LabelX7.Location = New System.Drawing.Point(708, 2)
        Me.LabelX7.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX7.Name = "LabelX7"
        Me.LabelX7.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX7.Size = New System.Drawing.Size(59, 21)
        Me.LabelX7.TabIndex = 44
        Me.LabelX7.Text = "Sucursal:"
        '
        'SuperTabItem3
        '
        Me.SuperTabItem3.AttachedControl = Me.SuperTabControlPanel3
        Me.SuperTabItem3.GlobalItem = False
        Me.SuperTabItem3.Name = "SuperTabItem3"
        Me.SuperTabItem3.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Blue
        Me.SuperTabItem3.SelectedTabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabItem3.Symbol = ""
        Me.SuperTabItem3.SymbolSize = 10.0!
        Me.SuperTabItem3.TabFont = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabItem3.Text = "Datos Ventas"
        '
        'SuperTabControlPanel4
        '
        Me.SuperTabControlPanel4.Controls.Add(Me.Panel20)
        Me.SuperTabControlPanel4.Controls.Add(Me.Panel18)
        Me.SuperTabControlPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel4.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabControlPanel4.Name = "SuperTabControlPanel4"
        Me.SuperTabControlPanel4.Size = New System.Drawing.Size(1173, 185)
        Me.SuperTabControlPanel4.TabIndex = 0
        Me.SuperTabControlPanel4.TabItem = Me.tab_Cobro
        '
        'Panel20
        '
        Me.Panel20.Controls.Add(Me.GroupBox2)
        Me.Panel20.Controls.Add(Me.GroupBox1)
        Me.Panel20.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel20.Location = New System.Drawing.Point(0, 0)
        Me.Panel20.Name = "Panel20"
        Me.Panel20.Size = New System.Drawing.Size(1173, 185)
        Me.Panel20.TabIndex = 1
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.GroupBox2.Controls.Add(Me.tbCambio)
        Me.GroupBox2.Controls.Add(Me.LabelX17)
        Me.GroupBox2.Controls.Add(Me.tbTotalPagado)
        Me.GroupBox2.Controls.Add(Me.LabelX18)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox2.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.ForeColor = System.Drawing.Color.Cornsilk
        Me.GroupBox2.Location = New System.Drawing.Point(656, 0)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox2.Size = New System.Drawing.Size(517, 185)
        Me.GroupBox2.TabIndex = 363
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Efectivo Pagado / Cambio:"
        '
        'tbCambio
        '
        '
        '
        '
        Me.tbCambio.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbCambio.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbCambio.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbCambio.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCambio.ForeColor = System.Drawing.Color.Black
        Me.tbCambio.Increment = 1.0R
        Me.tbCambio.Location = New System.Drawing.Point(130, 81)
        Me.tbCambio.Margin = New System.Windows.Forms.Padding(4)
        Me.tbCambio.MinValue = 0R
        Me.tbCambio.Name = "tbCambio"
        Me.tbCambio.Size = New System.Drawing.Size(160, 27)
        Me.tbCambio.TabIndex = 68
        Me.tbCambio.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX17
        '
        Me.LabelX17.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX17.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX17.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX17.ForeColor = System.Drawing.Color.White
        Me.LabelX17.Location = New System.Drawing.Point(10, 81)
        Me.LabelX17.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX17.Name = "LabelX17"
        Me.LabelX17.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX17.Size = New System.Drawing.Size(113, 22)
        Me.LabelX17.TabIndex = 67
        Me.LabelX17.Text = "Cambio:"
        '
        'tbTotalPagado
        '
        '
        '
        '
        Me.tbTotalPagado.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbTotalPagado.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbTotalPagado.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbTotalPagado.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTotalPagado.ForeColor = System.Drawing.Color.Black
        Me.tbTotalPagado.Increment = 1.0R
        Me.tbTotalPagado.Location = New System.Drawing.Point(130, 37)
        Me.tbTotalPagado.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTotalPagado.MinValue = 0R
        Me.tbTotalPagado.Name = "tbTotalPagado"
        Me.tbTotalPagado.Size = New System.Drawing.Size(160, 27)
        Me.tbTotalPagado.TabIndex = 3
        Me.tbTotalPagado.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX18
        '
        Me.LabelX18.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX18.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX18.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX18.ForeColor = System.Drawing.Color.White
        Me.LabelX18.Location = New System.Drawing.Point(10, 41)
        Me.LabelX18.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX18.Name = "LabelX18"
        Me.LabelX18.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX18.Size = New System.Drawing.Size(113, 22)
        Me.LabelX18.TabIndex = 64
        Me.LabelX18.Text = "Total Pagado:"
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.GroupBox1.Controls.Add(Me.lbTipoCambio)
        Me.GroupBox1.Controls.Add(Me.tbTransferencia)
        Me.GroupBox1.Controls.Add(Me.LabelX15)
        Me.GroupBox1.Controls.Add(Me.tbTarjeta)
        Me.GroupBox1.Controls.Add(Me.LabelX14)
        Me.GroupBox1.Controls.Add(Me.tbMontoBs)
        Me.GroupBox1.Controls.Add(Me.LabelX12)
        Me.GroupBox1.Controls.Add(Me.tbMontoDolar)
        Me.GroupBox1.Controls.Add(Me.LabelX13)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox1.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Cornsilk
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Size = New System.Drawing.Size(656, 185)
        Me.GroupBox1.TabIndex = 362
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Efectivo Recibido:"
        '
        'lbTipoCambio
        '
        Me.lbTipoCambio.AutoSize = True
        Me.lbTipoCambio.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbTipoCambio.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbTipoCambio.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbTipoCambio.ForeColor = System.Drawing.Color.White
        Me.lbTipoCambio.Location = New System.Drawing.Point(479, 11)
        Me.lbTipoCambio.Margin = New System.Windows.Forms.Padding(4)
        Me.lbTipoCambio.Name = "lbTipoCambio"
        Me.lbTipoCambio.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbTipoCambio.Size = New System.Drawing.Size(76, 21)
        Me.lbTipoCambio.TabIndex = 71
        Me.lbTipoCambio.Text = "Monto $u$:"
        '
        'tbTransferencia
        '
        '
        '
        '
        Me.tbTransferencia.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbTransferencia.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbTransferencia.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbTransferencia.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTransferencia.ForeColor = System.Drawing.Color.Black
        Me.tbTransferencia.Increment = 1.0R
        Me.tbTransferencia.Location = New System.Drawing.Point(479, 77)
        Me.tbTransferencia.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTransferencia.MinValue = 0R
        Me.tbTransferencia.Name = "tbTransferencia"
        Me.tbTransferencia.Size = New System.Drawing.Size(147, 27)
        Me.tbTransferencia.TabIndex = 70
        Me.tbTransferencia.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX15
        '
        Me.LabelX15.AutoSize = True
        Me.LabelX15.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX15.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX15.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX15.ForeColor = System.Drawing.Color.White
        Me.LabelX15.Location = New System.Drawing.Point(301, 82)
        Me.LabelX15.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX15.Name = "LabelX15"
        Me.LabelX15.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX15.Size = New System.Drawing.Size(168, 23)
        Me.LabelX15.TabIndex = 69
        Me.LabelX15.Text = "Transferencia Bancaria:"
        Me.LabelX15.VerticalTextTopUp = False
        '
        'tbTarjeta
        '
        '
        '
        '
        Me.tbTarjeta.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbTarjeta.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbTarjeta.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbTarjeta.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbTarjeta.ForeColor = System.Drawing.Color.Black
        Me.tbTarjeta.Increment = 1.0R
        Me.tbTarjeta.Location = New System.Drawing.Point(129, 82)
        Me.tbTarjeta.Margin = New System.Windows.Forms.Padding(4)
        Me.tbTarjeta.MinValue = 0R
        Me.tbTarjeta.Name = "tbTarjeta"
        Me.tbTarjeta.Size = New System.Drawing.Size(160, 27)
        Me.tbTarjeta.TabIndex = 68
        Me.tbTarjeta.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX14
        '
        Me.LabelX14.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX14.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX14.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX14.ForeColor = System.Drawing.Color.White
        Me.LabelX14.Location = New System.Drawing.Point(9, 82)
        Me.LabelX14.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX14.Name = "LabelX14"
        Me.LabelX14.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX14.Size = New System.Drawing.Size(113, 22)
        Me.LabelX14.TabIndex = 67
        Me.LabelX14.Text = "Tarjeta:"
        '
        'tbMontoBs
        '
        '
        '
        '
        Me.tbMontoBs.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbMontoBs.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbMontoBs.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbMontoBs.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMontoBs.ForeColor = System.Drawing.Color.Black
        Me.tbMontoBs.Increment = 1.0R
        Me.tbMontoBs.Location = New System.Drawing.Point(129, 38)
        Me.tbMontoBs.Margin = New System.Windows.Forms.Padding(4)
        Me.tbMontoBs.MinValue = 0R
        Me.tbMontoBs.Name = "tbMontoBs"
        Me.tbMontoBs.Size = New System.Drawing.Size(160, 27)
        Me.tbMontoBs.TabIndex = 3
        Me.tbMontoBs.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX12
        '
        Me.LabelX12.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX12.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX12.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX12.ForeColor = System.Drawing.Color.White
        Me.LabelX12.Location = New System.Drawing.Point(9, 42)
        Me.LabelX12.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX12.Name = "LabelX12"
        Me.LabelX12.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX12.Size = New System.Drawing.Size(113, 22)
        Me.LabelX12.TabIndex = 64
        Me.LabelX12.Text = "Monto Bs:"
        '
        'tbMontoDolar
        '
        '
        '
        '
        Me.tbMontoDolar.BackgroundStyle.Class = "DateTimeInputBackground"
        Me.tbMontoDolar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbMontoDolar.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2
        Me.tbMontoDolar.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbMontoDolar.ForeColor = System.Drawing.Color.Black
        Me.tbMontoDolar.Increment = 1.0R
        Me.tbMontoDolar.Location = New System.Drawing.Point(479, 38)
        Me.tbMontoDolar.Margin = New System.Windows.Forms.Padding(4)
        Me.tbMontoDolar.MinValue = 0R
        Me.tbMontoDolar.Name = "tbMontoDolar"
        Me.tbMontoDolar.Size = New System.Drawing.Size(147, 27)
        Me.tbMontoDolar.TabIndex = 4
        Me.tbMontoDolar.WatermarkAlignment = DevComponents.Editors.eTextAlignment.Right
        '
        'LabelX13
        '
        Me.LabelX13.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX13.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX13.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX13.ForeColor = System.Drawing.Color.White
        Me.LabelX13.Location = New System.Drawing.Point(301, 42)
        Me.LabelX13.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX13.Name = "LabelX13"
        Me.LabelX13.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX13.Size = New System.Drawing.Size(93, 22)
        Me.LabelX13.TabIndex = 66
        Me.LabelX13.Text = "Monto $u$:"
        '
        'Panel18
        '
        Me.Panel18.Location = New System.Drawing.Point(-31, -59)
        Me.Panel18.Name = "Panel18"
        Me.Panel18.Size = New System.Drawing.Size(200, 100)
        Me.Panel18.TabIndex = 0
        '
        'tab_Cobro
        '
        Me.tab_Cobro.AttachedControl = Me.SuperTabControlPanel4
        Me.tab_Cobro.GlobalItem = False
        Me.tab_Cobro.Name = "tab_Cobro"
        Me.tab_Cobro.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Blue
        Me.tab_Cobro.SelectedTabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tab_Cobro.Symbol = ""
        Me.tab_Cobro.SymbolSize = 10.0!
        Me.tab_Cobro.TabFont = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tab_Cobro.Text = "Detalle Cobro"
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
        Me.Panel10.Size = New System.Drawing.Size(1365, 0)
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
        Me.Panel11.Size = New System.Drawing.Size(1363, 0)
        Me.Panel11.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Label3.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.Label3.Location = New System.Drawing.Point(60, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.Label3.Size = New System.Drawing.Size(253, 0)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Datos De Ventas"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel12
        '
        Me.Panel12.BackColor = System.Drawing.Color.White
        Me.Panel12.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel12.Location = New System.Drawing.Point(59, 0)
        Me.Panel12.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel12.Name = "Panel12"
        Me.Panel12.Size = New System.Drawing.Size(1, 0)
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
        Me.PictureBox3.Size = New System.Drawing.Size(59, 0)
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
        Me.PanelButton.Location = New System.Drawing.Point(0, 546)
        Me.PanelButton.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PanelButton.Name = "PanelButton"
        Me.PanelButton.Size = New System.Drawing.Size(1371, 50)
        Me.PanelButton.TabIndex = 3
        '
        'PanelToolBar1
        '
        Me.PanelToolBar1.Controls.Add(Me.ButtonX3)
        Me.PanelToolBar1.Controls.Add(Me.BtnImprimir)
        Me.PanelToolBar1.Controls.Add(Me.btnSalir)
        Me.PanelToolBar1.Controls.Add(Me.btnEliminar)
        Me.PanelToolBar1.Controls.Add(Me.btnGrabar)
        Me.PanelToolBar1.Controls.Add(Me.btnModificar)
        Me.PanelToolBar1.Controls.Add(Me.btnNuevo)
        Me.PanelToolBar1.Dock = System.Windows.Forms.DockStyle.Left
        Me.PanelToolBar1.Location = New System.Drawing.Point(0, 0)
        Me.PanelToolBar1.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelToolBar1.Name = "PanelToolBar1"
        Me.PanelToolBar1.Size = New System.Drawing.Size(819, 50)
        Me.PanelToolBar1.TabIndex = 7
        '
        'ButtonX3
        '
        Me.ButtonX3.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX3.ColorTable = DevComponents.DotNetBar.eButtonColor.Magenta
        Me.ButtonX3.Dock = System.Windows.Forms.DockStyle.Left
        Me.ButtonX3.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX3.Image = Global.TeVendo.My.Resources.Resources.tec_print
        Me.ButtonX3.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.ButtonX3.Location = New System.Drawing.Point(739, 0)
        Me.ButtonX3.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX3.Name = "ButtonX3"
        Me.ButtonX3.Size = New System.Drawing.Size(115, 50)
        Me.ButtonX3.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX3.TabIndex = 12
        Me.ButtonX3.Text = "IMPRIMIR + COPIA"
        Me.ButtonX3.TextColor = System.Drawing.Color.White
        '
        'BtnImprimir
        '
        Me.BtnImprimir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.BtnImprimir.ColorTable = DevComponents.DotNetBar.eButtonColor.Magenta
        Me.BtnImprimir.Dock = System.Windows.Forms.DockStyle.Left
        Me.BtnImprimir.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnImprimir.Image = Global.TeVendo.My.Resources.Resources.tec_print
        Me.BtnImprimir.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.BtnImprimir.Location = New System.Drawing.Point(624, 0)
        Me.BtnImprimir.Margin = New System.Windows.Forms.Padding(4)
        Me.BtnImprimir.Name = "BtnImprimir"
        Me.BtnImprimir.Size = New System.Drawing.Size(115, 50)
        Me.BtnImprimir.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.BtnImprimir.TabIndex = 11
        Me.BtnImprimir.Text = "IMPRIMIR"
        Me.BtnImprimir.TextColor = System.Drawing.Color.White
        '
        'btnSalir
        '
        Me.btnSalir.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnSalir.ColorTable = DevComponents.DotNetBar.eButtonColor.Magenta
        Me.btnSalir.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnSalir.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSalir.Image = Global.TeVendo.My.Resources.Resources.iconatras
        Me.btnSalir.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnSalir.Location = New System.Drawing.Point(524, 0)
        Me.btnSalir.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSalir.Name = "btnSalir"
        Me.btnSalir.Size = New System.Drawing.Size(100, 50)
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
        Me.btnEliminar.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEliminar.Image = Global.TeVendo.My.Resources.Resources.iconeliminar
        Me.btnEliminar.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnEliminar.Location = New System.Drawing.Point(391, 0)
        Me.btnEliminar.Margin = New System.Windows.Forms.Padding(4)
        Me.btnEliminar.Name = "btnEliminar"
        Me.btnEliminar.Size = New System.Drawing.Size(133, 50)
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
        Me.btnGrabar.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGrabar.Image = Global.TeVendo.My.Resources.Resources.iconguardar
        Me.btnGrabar.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnGrabar.Location = New System.Drawing.Point(252, 0)
        Me.btnGrabar.Margin = New System.Windows.Forms.Padding(4)
        Me.btnGrabar.Name = "btnGrabar"
        Me.btnGrabar.Size = New System.Drawing.Size(139, 50)
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
        Me.btnModificar.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnModificar.Image = Global.TeVendo.My.Resources.Resources.iconeditar
        Me.btnModificar.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnModificar.Location = New System.Drawing.Point(112, 0)
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
        Me.btnNuevo.Font = New System.Drawing.Font("Arial", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNuevo.Image = Global.TeVendo.My.Resources.Resources.iconadd
        Me.btnNuevo.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.btnNuevo.Location = New System.Drawing.Point(0, 0)
        Me.btnNuevo.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNuevo.Name = "btnNuevo"
        Me.btnNuevo.Size = New System.Drawing.Size(112, 50)
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
        Me.PanelNavegacion.Location = New System.Drawing.Point(904, 0)
        Me.PanelNavegacion.Margin = New System.Windows.Forms.Padding(4)
        Me.PanelNavegacion.Name = "PanelNavegacion"
        Me.PanelNavegacion.Size = New System.Drawing.Size(467, 50)
        Me.PanelNavegacion.TabIndex = 21
        '
        'LblPaginacion
        '
        Me.LblPaginacion.BackColor = System.Drawing.Color.White
        Me.LblPaginacion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LblPaginacion.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
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
        Me.SuperTabItem1.TabFont = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabItem1.Text = "Detalle De Datos"
        Me.SuperTabItem1.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Center
        '
        'SuperTabControlPanel2
        '
        Me.SuperTabControlPanel2.Controls.Add(Me.Panel17)
        Me.SuperTabControlPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel2.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabControlPanel2.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.SuperTabControlPanel2.Name = "SuperTabControlPanel2"
        Me.SuperTabControlPanel2.Size = New System.Drawing.Size(1371, 589)
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
        Me.Panel17.Size = New System.Drawing.Size(1371, 589)
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
        Me.JGrM_Buscador.GridLines = Janus.Windows.GridEX.GridLines.None
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
        Me.JGrM_Buscador.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
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
        Me.JGrM_Buscador.Size = New System.Drawing.Size(1361, 479)
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
        Me.Panel4.Size = New System.Drawing.Size(1361, 64)
        Me.Panel4.TabIndex = 4
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.46511!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.60465!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.325581!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.60465!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnImprimirCopia, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel21, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel19, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnSi, 3, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.Padding = New System.Windows.Forms.Padding(5, 2, 5, 5)
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1361, 64)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'btnImprimirCopia
        '
        Me.btnImprimirCopia.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnImprimirCopia.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(179, Byte), Integer), CType(CType(148, Byte), Integer))
        Me.btnImprimirCopia.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.btnImprimirCopia.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnImprimirCopia.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnImprimirCopia.Image = Global.TeVendo.My.Resources.Resources.tec_print
        Me.btnImprimirCopia.ImageFixedSize = New System.Drawing.Size(25, 25)
        Me.btnImprimirCopia.Location = New System.Drawing.Point(1076, 6)
        Me.btnImprimirCopia.Margin = New System.Windows.Forms.Padding(4)
        Me.btnImprimirCopia.Name = "btnImprimirCopia"
        Me.btnImprimirCopia.Size = New System.Drawing.Size(23, 49)
        Me.btnImprimirCopia.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnImprimirCopia.TabIndex = 8
        Me.btnImprimirCopia.Text = "Imprimir Nota" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "+ Copia"
        Me.btnImprimirCopia.TextColor = System.Drawing.Color.White
        Me.btnImprimirCopia.Visible = False
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
        Me.Panel21.Location = New System.Drawing.Point(8, 5)
        Me.Panel21.Name = "Panel21"
        Me.Panel21.Size = New System.Drawing.Size(810, 51)
        Me.Panel21.TabIndex = 5
        '
        'btnFiltrarVentas
        '
        Me.btnFiltrarVentas.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnFiltrarVentas.BackColor = System.Drawing.Color.DarkOrange
        Me.btnFiltrarVentas.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnFiltrarVentas.Font = New System.Drawing.Font("Calibri", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFiltrarVentas.Image = Global.TeVendo.My.Resources.Resources.search
        Me.btnFiltrarVentas.ImageFixedSize = New System.Drawing.Size(28, 28)
        Me.btnFiltrarVentas.Location = New System.Drawing.Point(532, 9)
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
        Me.tbHasta.Location = New System.Drawing.Point(354, 10)
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
        Me.LabelX16.Location = New System.Drawing.Point(261, 14)
        Me.LabelX16.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX16.Name = "LabelX16"
        Me.LabelX16.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX16.Size = New System.Drawing.Size(83, 21)
        Me.LabelX16.TabIndex = 221
        Me.LabelX16.Text = "Venta Hasta:"
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
        Me.tbDesde.Location = New System.Drawing.Point(97, 11)
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
        Me.LabelX6.Location = New System.Drawing.Point(4, 15)
        Me.LabelX6.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX6.Name = "LabelX6"
        Me.LabelX6.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX6.Size = New System.Drawing.Size(86, 21)
        Me.LabelX6.TabIndex = 219
        Me.LabelX6.Text = "Venta Desde:"
        '
        'Panel19
        '
        Me.Panel19.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(179, Byte), Integer), CType(CType(148, Byte), Integer))
        Me.Panel19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel19.Controls.Add(Me.ButtonX2)
        Me.Panel19.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel19.Location = New System.Drawing.Point(859, 4)
        Me.Panel19.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel19.Name = "Panel19"
        Me.Panel19.Size = New System.Drawing.Size(210, 53)
        Me.Panel19.TabIndex = 5
        '
        'ButtonX2
        '
        Me.ButtonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX2.BackColor = System.Drawing.Color.Transparent
        Me.ButtonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonX2.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX2.Image = Global.TeVendo.My.Resources.Resources.tec_print
        Me.ButtonX2.ImageFixedSize = New System.Drawing.Size(25, 25)
        Me.ButtonX2.Location = New System.Drawing.Point(0, 0)
        Me.ButtonX2.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX2.Name = "ButtonX2"
        Me.ButtonX2.Size = New System.Drawing.Size(208, 51)
        Me.ButtonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX2.TabIndex = 7
        Me.ButtonX2.Text = "Imprimir Nota de Venta"
        Me.ButtonX2.TextColor = System.Drawing.Color.White
        '
        'btnSi
        '
        Me.btnSi.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(179, Byte), Integer), CType(CType(148, Byte), Integer))
        Me.btnSi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.btnSi.Controls.Add(Me.ButtonX1)
        Me.btnSi.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnSi.Location = New System.Drawing.Point(1106, 4)
        Me.btnSi.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnSi.Name = "btnSi"
        Me.btnSi.Size = New System.Drawing.Size(247, 53)
        Me.btnSi.TabIndex = 4
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.BackColor = System.Drawing.Color.Transparent
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue
        Me.ButtonX1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonX1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.TeVendo.My.Resources.Resources.iconadd
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(25, 25)
        Me.ButtonX1.Location = New System.Drawing.Point(0, 0)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(4)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(245, 51)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.ButtonX1.TabIndex = 7
        Me.ButtonX1.Text = "Nueva Venta"
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
        Me.Panel14.Size = New System.Drawing.Size(1361, 36)
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
        Me.Panel15.Size = New System.Drawing.Size(1359, 34)
        Me.Panel15.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Left
        Me.Label2.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.Label2.Location = New System.Drawing.Point(60, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.Label2.Size = New System.Drawing.Size(253, 34)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "LISTADO DE VENTAS"
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
        Me.SuperTabItem2.TabFont = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabItem2.Text = "Lista De Datos"
        Me.SuperTabItem2.TextAlignment = DevComponents.DotNetBar.eItemAlignment.Center
        '
        'MEP
        '
        Me.MEP.ContainerControl = Me
        '
        'GroupPanel1
        '
        Me.GroupPanel1.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007
        Me.GroupPanel1.Controls.Add(Me.Panel22)
        Me.GroupPanel1.DisabledBackColor = System.Drawing.Color.Empty
        Me.GroupPanel1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanel1.Location = New System.Drawing.Point(708, 60)
        Me.GroupPanel1.Name = "GroupPanel1"
        Me.GroupPanel1.Size = New System.Drawing.Size(469, 121)
        '
        '
        '
        Me.GroupPanel1.Style.BackColor = System.Drawing.Color.DarkCyan
        Me.GroupPanel1.Style.BackColor2 = System.Drawing.Color.DarkCyan
        Me.GroupPanel1.Style.BackColorGradientAngle = 90
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
        Me.GroupPanel1.Text = "Facturar A:"
        '
        'Panel22
        '
        Me.Panel22.Controls.Add(Me.LabelX20)
        Me.Panel22.Controls.Add(Me.TextBoxX1)
        Me.Panel22.Controls.Add(Me.LabelX19)
        Me.Panel22.Controls.Add(Me.tbnit)
        Me.Panel22.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel22.Location = New System.Drawing.Point(0, 0)
        Me.Panel22.Name = "Panel22"
        Me.Panel22.Size = New System.Drawing.Size(463, 94)
        Me.Panel22.TabIndex = 0
        '
        'LabelX19
        '
        Me.LabelX19.AutoSize = True
        Me.LabelX19.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX19.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX19.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX19.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX19.Location = New System.Drawing.Point(4, 9)
        Me.LabelX19.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX19.Name = "LabelX19"
        Me.LabelX19.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX19.Size = New System.Drawing.Size(25, 21)
        Me.LabelX19.TabIndex = 214
        Me.LabelX19.Text = "Nit:"
        '
        'tbnit
        '
        '
        '
        '
        Me.tbnit.Border.Class = "TextBoxBorder"
        Me.tbnit.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.tbnit.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbnit.Location = New System.Drawing.Point(96, 9)
        Me.tbnit.Margin = New System.Windows.Forms.Padding(4)
        Me.tbnit.Name = "tbnit"
        Me.tbnit.PreventEnterBeep = True
        Me.tbnit.Size = New System.Drawing.Size(349, 26)
        Me.tbnit.TabIndex = 213
        '
        'LabelX20
        '
        Me.LabelX20.AutoSize = True
        Me.LabelX20.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX20.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX20.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX20.ForeColor = System.Drawing.Color.FromArgb(CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LabelX20.Location = New System.Drawing.Point(4, 44)
        Me.LabelX20.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX20.Name = "LabelX20"
        Me.LabelX20.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX20.Size = New System.Drawing.Size(84, 21)
        Me.LabelX20.TabIndex = 216
        Me.LabelX20.Text = "Razon Social:"
        '
        'TextBoxX1
        '
        '
        '
        '
        Me.TextBoxX1.Border.Class = "TextBoxBorder"
        Me.TextBoxX1.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.TextBoxX1.Font = New System.Drawing.Font("Calibri", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxX1.Location = New System.Drawing.Point(96, 39)
        Me.TextBoxX1.Margin = New System.Windows.Forms.Padding(4)
        Me.TextBoxX1.Multiline = True
        Me.TextBoxX1.Name = "TextBoxX1"
        Me.TextBoxX1.PreventEnterBeep = True
        Me.TextBoxX1.Size = New System.Drawing.Size(349, 51)
        Me.TextBoxX1.TabIndex = 215
        '
        'SuperTabItem4
        '
        Me.SuperTabItem4.AttachedControl = Me.SuperTabControlPanel5
        Me.SuperTabItem4.GlobalItem = False
        Me.SuperTabItem4.Name = "SuperTabItem4"
        Me.SuperTabItem4.PredefinedColor = DevComponents.DotNetBar.eTabItemColor.OfficeMobile2014Blue
        Me.SuperTabItem4.SelectedTabFont = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SuperTabItem4.Symbol = ""
        Me.SuperTabItem4.SymbolSize = 10.0!
        Me.SuperTabItem4.Text = "Datos Facturación"
        '
        'SuperTabControlPanel5
        '
        Me.SuperTabControlPanel5.Controls.Add(Me.GroupBox3)
        Me.SuperTabControlPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SuperTabControlPanel5.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabControlPanel5.Name = "SuperTabControlPanel5"
        Me.SuperTabControlPanel5.Size = New System.Drawing.Size(1196, 185)
        Me.SuperTabControlPanel5.TabIndex = 0
        Me.SuperTabControlPanel5.TabItem = Me.SuperTabItem4
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.Color.FromArgb(CType(CType(25, Byte), Integer), CType(CType(170, Byte), Integer), CType(CType(141, Byte), Integer))
        Me.GroupBox3.Controls.Add(Me.LabelX22)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox3.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.ForeColor = System.Drawing.Color.Cornsilk
        Me.GroupBox3.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox3.Size = New System.Drawing.Size(1196, 185)
        Me.GroupBox3.TabIndex = 364
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Datos De Facturación:"
        '
        'LabelX22
        '
        Me.LabelX22.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX22.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX22.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX22.ForeColor = System.Drawing.Color.White
        Me.LabelX22.Location = New System.Drawing.Point(10, 41)
        Me.LabelX22.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX22.Name = "LabelX22"
        Me.LabelX22.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX22.Size = New System.Drawing.Size(113, 22)
        Me.LabelX22.TabIndex = 64
        Me.LabelX22.Text = "Total Pagado:"
        '
        'Tec_Ventas
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1371, 625)
        Me.Controls.Add(Me.TabControlPrincipal)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Name = "Tec_Ventas"
        Me.Text = "Gestion Ventas"
        CType(Me.TabControlPrincipal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControlPrincipal.ResumeLayout(False)
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
        CType(Me.SuperTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SuperTabControl1.ResumeLayout(False)
        Me.SuperTabControlPanel3.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        CType(Me.cbSucursal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SuperTabControlPanel4.ResumeLayout(False)
        Me.Panel20.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        CType(Me.tbCambio, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbTotalPagado, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.tbTransferencia, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbTarjeta, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbMontoBs, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbMontoDolar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel10.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelButton.ResumeLayout(False)
        Me.PanelToolBar1.ResumeLayout(False)
        Me.PanelNavegacion.ResumeLayout(False)
        Me.SuperTabControlPanel2.ResumeLayout(False)
        Me.Panel17.ResumeLayout(False)
        CType(Me.JGrM_Buscador, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MeuOpciones.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.Panel21.ResumeLayout(False)
        Me.Panel21.PerformLayout()
        Me.Panel19.ResumeLayout(False)
        Me.btnSi.ResumeLayout(False)
        Me.Panel14.ResumeLayout(False)
        Me.Panel15.ResumeLayout(False)
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupPanel1.ResumeLayout(False)
        Me.Panel22.ResumeLayout(False)
        Me.Panel22.PerformLayout()
        Me.SuperTabControlPanel5.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
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
    Friend WithEvents PanelTotal As Panel
    Friend WithEvents Panel13 As Panel
    Friend WithEvents LabelX10 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbTotal As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX9 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbMdesc As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX8 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbPdesc As DevComponents.Editors.DoubleInput
    Friend WithEvents Panel5 As Panel
    Friend WithEvents Panel6 As Panel
    Friend WithEvents lbprivilegio As Label
    Friend WithEvents Panel7 As Panel
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents PanelLEft As Panel
    Friend WithEvents Panel8 As Panel
    Friend WithEvents Panel9 As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents btnVendedor As DevComponents.DotNetBar.ButtonX
    Friend WithEvents lbcredito As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX5 As DevComponents.DotNetBar.LabelX
    Friend WithEvents swTipoVenta As DevComponents.DotNetBar.Controls.SwitchButton
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbVendedor As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents tbCodigo As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX3 As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX2 As DevComponents.DotNetBar.LabelX
    Friend WithEvents LabelX4 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbGlosa As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX7 As DevComponents.DotNetBar.LabelX
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
    Friend WithEvents btnCliente As DevComponents.DotNetBar.ButtonX
    Friend WithEvents LabelX11 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbCliente As DevComponents.DotNetBar.Controls.TextBoxX
    Protected WithEvents MHighlighterFocus As DevComponents.DotNetBar.Validator.Highlighter
    Protected WithEvents MEP As ErrorProvider
    Protected WithEvents BtnImprimir As DevComponents.DotNetBar.ButtonX
    Friend WithEvents Panel19 As Panel
    Protected WithEvents ButtonX2 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents MeuOpciones As ContextMenuStrip
    Friend WithEvents VerToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents EditarToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EliminarToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents btnSeleccionarProducto As DevComponents.DotNetBar.ButtonX
    Friend WithEvents tbFechaTransaccion As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents tbFechaVencimientoCredito As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents cbSucursal As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents SuperTabControl1 As DevComponents.DotNetBar.SuperTabControl
    Friend WithEvents SuperTabControlPanel4 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents Panel20 As Panel
    Friend WithEvents Panel18 As Panel
    Friend WithEvents tab_Cobro As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents SuperTabControlPanel3 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents SuperTabItem3 As DevComponents.DotNetBar.SuperTabItem
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents LabelX14 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbMontoBs As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX12 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbMontoDolar As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX13 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbTarjeta As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX15 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbTransferencia As DevComponents.Editors.DoubleInput
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents tbCambio As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX17 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbTotalPagado As DevComponents.Editors.DoubleInput
    Friend WithEvents LabelX18 As DevComponents.DotNetBar.LabelX
    Friend WithEvents lbTipoCambio As DevComponents.DotNetBar.LabelX
    Friend WithEvents Panel21 As Panel
    Friend WithEvents tbDesde As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX6 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbHasta As Janus.Windows.CalendarCombo.CalendarCombo
    Friend WithEvents LabelX16 As DevComponents.DotNetBar.LabelX
    Friend WithEvents btnFiltrarVentas As DevComponents.DotNetBar.ButtonX
    Protected WithEvents btnImprimirCopia As DevComponents.DotNetBar.ButtonX
    Protected WithEvents ButtonX3 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents GroupPanel1 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents Panel22 As Panel
    Friend WithEvents LabelX19 As DevComponents.DotNetBar.LabelX
    Friend WithEvents tbnit As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX20 As DevComponents.DotNetBar.LabelX
    Friend WithEvents TextBoxX1 As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents SuperTabControlPanel5 As DevComponents.DotNetBar.SuperTabControlPanel
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents LabelX22 As DevComponents.DotNetBar.LabelX
    Friend WithEvents SuperTabItem4 As DevComponents.DotNetBar.SuperTabItem
End Class
