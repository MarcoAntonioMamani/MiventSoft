/* =====================================================================
   01 - INDICES (velocidad de ventas/pedidos, panel, precios, stock y grabacion)
   Solo agrega indices (no cambia datos ni SP). Se puede ejecutar mas de una vez:
   el que ya existe se salta.

   Medido en DistribucionDistralKCP2023 (copia local, 2026-11-15):
     - Lista de pedidos MAM_Ventas @3 (30 dias, web y escritorio): 11 s -> 0,15 s
     - Pedidos pendientes MAM_Ventas @7, productos para vender @5: mas rapidos y estables
     - Al grabar/eliminar un pedido, compra o movimiento: los triggers buscan Movimientos por
       TransaccionId y MovimientosDetalle por MovimientoId (antes recorrian 46 mil y 190 mil filas)
     - ProductosStock (ProductoId, DepositoId, ...): lo usa la validacion de stock (04) para
       bloquear SOLO los productos del pedido.
   Recomendado: ejecutar fuera del horario de atencion (crear indices en Ventas toma segundos).
   QUITAR: revertir\REVERTIR_01_IX_Indices.sql
   ===================================================================== */
SET NOCOUNT ON
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PedidosAsignaciones_VentaId' AND object_id = OBJECT_ID('dbo.PedidosAsignaciones'))
	CREATE NONCLUSTERED INDEX IX_PedidosAsignaciones_VentaId ON dbo.PedidosAsignaciones (VentaId) INCLUDE (ConciliacionId, PersonalId)
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ventas_FechaVenta' AND object_id = OBJECT_ID('dbo.Ventas'))
	CREATE NONCLUSTERED INDEX IX_Ventas_FechaVenta ON dbo.Ventas (FechaVenta, Estado)
	INCLUDE (ClienteId, PersonalId, EstadoPedido, TotalVenta, TipoVenta, Anulado, SucursalId)
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ventas_EstadoPedido' AND object_id = OBJECT_ID('dbo.Ventas'))
	CREATE NONCLUSTERED INDEX IX_Ventas_EstadoPedido ON dbo.Ventas (EstadoPedido, Estado, Anulado)
	INCLUDE (FechaVenta, ClienteId, PersonalId, TotalVenta)
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductosStock_Producto_Deposito' AND object_id = OBJECT_ID('dbo.ProductosStock'))
	CREATE NONCLUSTERED INDEX IX_ProductosStock_Producto_Deposito ON dbo.ProductosStock (ProductoId, DepositoId, Lote, FechaVencimiento) INCLUDE (Cantidad)
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Movimientos_Transaccion' AND object_id = OBJECT_ID('dbo.Movimientos'))
	CREATE NONCLUSTERED INDEX IX_Movimientos_Transaccion ON dbo.Movimientos (TransaccionId, ConceptoId)
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MovimientosDetalle_Movimiento' AND object_id = OBJECT_ID('dbo.MovimientosDetalle'))
	CREATE NONCLUSTERED INDEX IX_MovimientosDetalle_Movimiento ON dbo.MovimientosDetalle (MovimientoId) INCLUDE (ProductoId, Cantidad)
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VentasDetalles_Producto' AND object_id = OBJECT_ID('dbo.VentasDetalles'))
	CREATE NONCLUSTERED INDEX IX_VentasDetalles_Producto ON dbo.VentasDetalles (ProductoId) INCLUDE (VentaId, Cantidad)
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CompraDetalle_Compra' AND object_id = OBJECT_ID('dbo.CompraDetalle'))
	CREATE NONCLUSTERED INDEX IX_CompraDetalle_Compra ON dbo.CompraDetalle (CompraId)
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CreditosVentas_Venta' AND object_id = OBJECT_ID('dbo.CreditosVentas'))
	CREATE NONCLUSTERED INDEX IX_CreditosVentas_Venta ON dbo.CreditosVentas (VentaId)
GO
RAISERROR('01 Indices INSTALADOS (los que ya existian se saltaron).', 10, 1) WITH NOWAIT
GO
