/* =====================================================================
   SOLO PARA DESHACER 01_IX_Indices.sql. NO es parte de la instalacion.
   OJO: 04_StockSeguro usa IX_ProductosStock_Producto_Deposito para bloquear solo los productos
   del pedido; si el 04 esta instalado, ese indice NO se quita.
   ===================================================================== */
SET NOCOUNT ON
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PedidosAsignaciones_VentaId' AND object_id = OBJECT_ID('dbo.PedidosAsignaciones')) DROP INDEX IX_PedidosAsignaciones_VentaId ON dbo.PedidosAsignaciones
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ventas_FechaVenta' AND object_id = OBJECT_ID('dbo.Ventas')) DROP INDEX IX_Ventas_FechaVenta ON dbo.Ventas
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ventas_EstadoPedido' AND object_id = OBJECT_ID('dbo.Ventas')) DROP INDEX IX_Ventas_EstadoPedido ON dbo.Ventas
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Movimientos_Transaccion' AND object_id = OBJECT_ID('dbo.Movimientos')) DROP INDEX IX_Movimientos_Transaccion ON dbo.Movimientos
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MovimientosDetalle_Movimiento' AND object_id = OBJECT_ID('dbo.MovimientosDetalle')) DROP INDEX IX_MovimientosDetalle_Movimiento ON dbo.MovimientosDetalle
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VentasDetalles_Producto' AND object_id = OBJECT_ID('dbo.VentasDetalles')) DROP INDEX IX_VentasDetalles_Producto ON dbo.VentasDetalles
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CompraDetalle_Compra' AND object_id = OBJECT_ID('dbo.CompraDetalle')) DROP INDEX IX_CompraDetalle_Compra ON dbo.CompraDetalle
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CreditosVentas_Venta' AND object_id = OBJECT_ID('dbo.CreditosVentas')) DROP INDEX IX_CreditosVentas_Venta ON dbo.CreditosVentas
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas')) NOT LIKE '%Sanbit: validacion de stock%'
	AND EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductosStock_Producto_Deposito' AND object_id = OBJECT_ID('dbo.ProductosStock'))
	DROP INDEX IX_ProductosStock_Producto_Deposito ON dbo.ProductosStock
GO
RAISERROR('01 Indices QUITADOS.', 10, 1) WITH NOWAIT
GO
