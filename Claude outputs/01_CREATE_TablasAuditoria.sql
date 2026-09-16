/* ============================================================================
   Auditoria de Ventas / Compras / Movimientos + mejora de Bitacora
   ----------------------------------------------------------------------------
   Contexto (BancoSol / Kaili Industrial - MiventSoft):
     - Se detecto que MAM_Ventas, MAM_Compras y MAM_Movimientos no hacian
       ROLLBACK en varios @tipo (Eliminar/Nuevo/Modificacion), y que Bitacora
       no guarda el @Id del registro afectado (dificulta el diagnostico).
     - Se detecto que al Eliminar una Venta se pierde el detalle (VentasDetalles)
       sin dejar rastro. En Compras/Movimientos el DELETE fisico de cabecera
       y detalle es intencional (alimenta el historico de movimientos) y NO
       se debe modificar ese comportamiento.
     - Diseno confirmado con el usuario: una tabla de auditoria por modulo
       (cabecera + detalle), con snapshot completo ANTES/DESPUES, escrito
       explicitamente dentro de cada SP (mismo patron que Bitacora), sin
       triggers.

   IMPORTANTE (ASFI / Compliance):
     - Estas tablas van a crecer indefinidamente si no se define una politica
       de retencion/purga. Antes de llevar esto a produccion, validar con
       el area de Compliance/ASFI cuanto tiempo se debe conservar este
       historial y si corresponde algun control adicional (cifrado en reposo,
       respaldo separado, etc.).
     - Recomendado probar primero en un ambiente de desarrollo/test, nunca
       directo en produccion.
   ============================================================================ */

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================================
-- 1) Bitacora: agregamos la columna para poder identificar el registro
--    afectado por el error (hoy Bitacora no lo guarda, es la causa de que
--    el usuario la vea "poco clara" para diagnosticar).
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Bitacora') AND name = 'baregistroid')
BEGIN
	ALTER TABLE dbo.Bitacora ADD baregistroid INT NULL
END
GO

-- ============================================================================
-- 2) VENTAS
-- ============================================================================
IF OBJECT_ID('dbo.VentasAuditoria', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.VentasAuditoria(
		[AuditoriaId] [int] IDENTITY(1,1) NOT NULL,
		[Id] [int] NOT NULL,
		[SucursalId] [int] NULL,
		[FechaVenta] [date] NULL,
		[PersonalId] [int] NULL,
		[TipoVenta] [int] NULL,
		[FechaVencimientoCredito] [date] NULL,
		[ClienteId] [int] NULL,
		[MonedaVenta] [int] NULL,
		[Estado] [int] NULL,
		[Glosa] [nvarchar](450) NULL,
		[Descuento] [decimal](18, 2) NULL,
		[TotalVenta] [decimal](18, 2) NULL,
		[FechaRegistro] [date] NULL,
		[HoraRegistro] [nvarchar](10) NULL,
		[UsuarioRegistro] [nvarchar](50) NULL,
		[EstadoPedido] [int] NULL,
		[RepartidorId] [int] NULL,
		[FechaEntrega] [date] NULL,
		[IdSincronizacion] [nvarchar](150) NULL,
		[Sistema] [nvarchar](50) NULL,
		[Latitud] [decimal](18, 18) NULL,
		[Longitud] [decimal](18, 18) NULL,
		[Facturado] [int] NULL,
		[Accion] [nvarchar](20) NOT NULL,		-- 'CREAR' | 'MODIFICAR' | 'ELIMINAR'
		[Momento] [nvarchar](10) NOT NULL,		-- 'ANTES'  | 'DESPUES'
		[UsuarioAccion] [nvarchar](50) NOT NULL,
		[FechaAccion] [date] NOT NULL,
		[HoraAccion] [nvarchar](5) NOT NULL,
	 CONSTRAINT [PK_VentasAuditoria] PRIMARY KEY CLUSTERED ([AuditoriaId] ASC)
	) ON [PRIMARY]

	CREATE NONCLUSTERED INDEX [IX_VentasAuditoria_Id] ON dbo.VentasAuditoria([Id] ASC)
	CREATE NONCLUSTERED INDEX [IX_VentasAuditoria_FechaAccion] ON dbo.VentasAuditoria([FechaAccion] ASC)
END
GO

IF OBJECT_ID('dbo.VentasDetalleAuditoria', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.VentasDetalleAuditoria(
		[AuditoriaDetalleId] [int] IDENTITY(1,1) NOT NULL,
		[AuditoriaId] [int] NOT NULL,
		[VentaId] [int] NOT NULL,
		[Id] [int] NULL,
		[ProductoId] [int] NULL,
		[Cantidad] [decimal](18, 2) NULL,
		[Precio] [decimal](18, 2) NULL,
		[SubTotal] [decimal](18, 2) NULL,
		[ProcentajeDescuento] [decimal](18, 2) NULL,
		[MontoDescuento] [decimal](18, 2) NULL,
		[Total] [decimal](18, 2) NULL,
		[Detalle] [nvarchar](450) NULL,
		[PrecioCosto] [decimal](18, 2) NULL,
		[Lote] [nvarchar](150) NULL,
		[FechaVencimiento] [date] NULL,
		[Tipo] [int] NULL,
		[KitId] [int] NULL,
		[CantidadKit] [int] NULL,
		[FechaRegistro] [date] NULL,
		[HoraRegistro] [nvarchar](10) NULL,
		[UsuarioRegistro] [nvarchar](50) NULL,
	 CONSTRAINT [PK_VentasDetalleAuditoria] PRIMARY KEY CLUSTERED ([AuditoriaDetalleId] ASC)
	) ON [PRIMARY]

	ALTER TABLE dbo.VentasDetalleAuditoria WITH CHECK ADD CONSTRAINT [FK_VentasDetalleAuditoria_VentasAuditoria]
		FOREIGN KEY([AuditoriaId]) REFERENCES dbo.VentasAuditoria([AuditoriaId])
	ALTER TABLE dbo.VentasDetalleAuditoria CHECK CONSTRAINT [FK_VentasDetalleAuditoria_VentasAuditoria]

	CREATE NONCLUSTERED INDEX [IX_VentasDetalleAuditoria_AuditoriaId] ON dbo.VentasDetalleAuditoria([AuditoriaId] ASC)
	CREATE NONCLUSTERED INDEX [IX_VentasDetalleAuditoria_VentaId] ON dbo.VentasDetalleAuditoria([VentaId] ASC)
END
GO

-- ============================================================================
-- 3) COMPRAS
-- ============================================================================
IF OBJECT_ID('dbo.ComprasAuditoria', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.ComprasAuditoria(
		[AuditoriaId] [int] IDENTITY(1,1) NOT NULL,
		[Id] [int] NOT NULL,
		[AlmacenId] [int] NULL,
		[FechaTransaccion] [date] NULL,
		[ProveedorId] [int] NULL,
		[TipoVenta] [int] NULL,
		[FechaVencimientoCredito] [date] NULL,
		[Moneda] [int] NULL,
		[Estado] [int] NULL,
		[Glosa] [nvarchar](450) NULL,
		[Descuento] [decimal](18, 2) NULL,
		[TotalCompra] [decimal](18, 2) NULL,
		[EmpresaId] [int] NULL,
		[FechaRegistro] [date] NULL,
		[HoraRegistro] [nvarchar](15) NULL,
		[UsuarioRegistro] [nvarchar](50) NULL,
		[Accion] [nvarchar](20) NOT NULL,
		[Momento] [nvarchar](10) NOT NULL,
		[UsuarioAccion] [nvarchar](50) NOT NULL,
		[FechaAccion] [date] NOT NULL,
		[HoraAccion] [nvarchar](5) NOT NULL,
	 CONSTRAINT [PK_ComprasAuditoria] PRIMARY KEY CLUSTERED ([AuditoriaId] ASC)
	) ON [PRIMARY]

	CREATE NONCLUSTERED INDEX [IX_ComprasAuditoria_Id] ON dbo.ComprasAuditoria([Id] ASC)
	CREATE NONCLUSTERED INDEX [IX_ComprasAuditoria_FechaAccion] ON dbo.ComprasAuditoria([FechaAccion] ASC)
END
GO

IF OBJECT_ID('dbo.CompraDetalleAuditoria', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.CompraDetalleAuditoria(
		[AuditoriaDetalleId] [int] IDENTITY(1,1) NOT NULL,
		[AuditoriaId] [int] NOT NULL,
		[CompraId] [int] NOT NULL,
		[Id] [int] NULL,
		[ProductoId] [int] NULL,
		[CantidadCompra] [decimal](18, 2) NULL,
		[PorcentajeIncremento] [decimal](18, 2) NULL,
		[CantidadIncremento] [decimal](18, 2) NULL,
		[Cantidad] [decimal](18, 2) NULL,
		[PrecioCosto] [decimal](18, 2) NULL,
		[Lote] [nvarchar](50) NULL,
		[FechaVencimiento] [date] NULL,
		[PrecioVenta] [decimal](18, 2) NULL,
		[TotalCompra] [decimal](18, 2) NULL,
		[FechaRegistro] [date] NULL,
		[HoraRegistro] [nvarchar](50) NULL,
		[UsuarioRegistro] [nvarchar](50) NULL,
	 CONSTRAINT [PK_CompraDetalleAuditoria] PRIMARY KEY CLUSTERED ([AuditoriaDetalleId] ASC)
	) ON [PRIMARY]

	ALTER TABLE dbo.CompraDetalleAuditoria WITH CHECK ADD CONSTRAINT [FK_CompraDetalleAuditoria_ComprasAuditoria]
		FOREIGN KEY([AuditoriaId]) REFERENCES dbo.ComprasAuditoria([AuditoriaId])
	ALTER TABLE dbo.CompraDetalleAuditoria CHECK CONSTRAINT [FK_CompraDetalleAuditoria_ComprasAuditoria]

	CREATE NONCLUSTERED INDEX [IX_CompraDetalleAuditoria_AuditoriaId] ON dbo.CompraDetalleAuditoria([AuditoriaId] ASC)
	CREATE NONCLUSTERED INDEX [IX_CompraDetalleAuditoria_CompraId] ON dbo.CompraDetalleAuditoria([CompraId] ASC)
END
GO

-- ============================================================================
-- 4) MOVIMIENTOS
-- ============================================================================
IF OBJECT_ID('dbo.MovimientosAuditoria', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.MovimientosAuditoria(
		[AuditoriaId] [int] IDENTITY(1,1) NOT NULL,
		[Id] [int] NOT NULL,
		[ConceptoId] [int] NULL,
		[TransaccionId] [int] NULL,
		[DepositoId] [int] NULL,
		[Observacion] [nvarchar](450) NULL,
		[Estado] [int] NULL,
		[FechaDocumento] [date] NULL,
		[DepositoIdDestino] [int] NULL,
		[IdMovimientoDestino] [int] NULL,
		[FechaRegistro] [date] NULL,
		[HoraRegistro] [nvarchar](15) NULL,
		[UsuarioRegistro] [nvarchar](50) NULL,
		[Accion] [nvarchar](20) NOT NULL,
		[Momento] [nvarchar](10) NOT NULL,
		[UsuarioAccion] [nvarchar](50) NOT NULL,
		[FechaAccion] [date] NOT NULL,
		[HoraAccion] [nvarchar](5) NOT NULL,
	 CONSTRAINT [PK_MovimientosAuditoria] PRIMARY KEY CLUSTERED ([AuditoriaId] ASC)
	) ON [PRIMARY]

	CREATE NONCLUSTERED INDEX [IX_MovimientosAuditoria_Id] ON dbo.MovimientosAuditoria([Id] ASC)
	CREATE NONCLUSTERED INDEX [IX_MovimientosAuditoria_FechaAccion] ON dbo.MovimientosAuditoria([FechaAccion] ASC)
END
GO

IF OBJECT_ID('dbo.MovimientosDetalleAuditoria', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.MovimientosDetalleAuditoria(
		[AuditoriaDetalleId] [int] IDENTITY(1,1) NOT NULL,
		[AuditoriaId] [int] NOT NULL,
		[MovimientoId] [int] NOT NULL,
		[Id] [int] NULL,
		[ProductoId] [int] NULL,
		[Cantidad] [decimal](18, 2) NULL,
		[Lote] [nvarchar](50) NULL,
		[FechaVencimiento] [date] NULL,
	 CONSTRAINT [PK_MovimientosDetalleAuditoria] PRIMARY KEY CLUSTERED ([AuditoriaDetalleId] ASC)
	) ON [PRIMARY]

	ALTER TABLE dbo.MovimientosDetalleAuditoria WITH CHECK ADD CONSTRAINT [FK_MovimientosDetalleAuditoria_MovimientosAuditoria]
		FOREIGN KEY([AuditoriaId]) REFERENCES dbo.MovimientosAuditoria([AuditoriaId])
	ALTER TABLE dbo.MovimientosDetalleAuditoria CHECK CONSTRAINT [FK_MovimientosDetalleAuditoria_MovimientosAuditoria]

	CREATE NONCLUSTERED INDEX [IX_MovimientosDetalleAuditoria_AuditoriaId] ON dbo.MovimientosDetalleAuditoria([AuditoriaId] ASC)
	CREATE NONCLUSTERED INDEX [IX_MovimientosDetalleAuditoria_MovimientoId] ON dbo.MovimientosDetalleAuditoria([MovimientoId] ASC)
END
GO
