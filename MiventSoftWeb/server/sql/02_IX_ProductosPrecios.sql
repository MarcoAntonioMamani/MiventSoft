/* =====================================================================
   Indices para la carga de productos (Tec_VentasDetalle, Tec_Compras, precios).
   No cambia ningun SP ni resultado.

   Medido en KailiIndustrialLaPaz (2026-11-12), con el plan que arma SQL Server
   cuando la primera llamada al SP llega con los parametros por defecto
   (parameter sniffing: p.ej. el panel llama MAM_Ventas @tipo 3 con -1):
     MAM_Ventas @5  (productos para vender)   sin indices 12.5 s   con indices 0.18 s
     MAM_Compras @5 (productos para comprar)  0.37 s -> 0.18 s
     sp_Mam_Precios @5 (precios)              43 s -> 27 s  (el indice NO alcanza:
                                              ver sp_Mam_Precios_tipo5.sql)
   Sin estos indices cada producto recorre Precios (15.7 mil filas) y
   ProductosStock completos; con ellos es una busqueda directa, sea cual sea el plan.
   ProductosStock: la misma clave la usan los triggers de movimientos al actualizar stock.

   INSTALAR: ejecutar este script completo (solo crea los indices si no existen).
   QUITAR:   DROP INDEX IX_Precios_Producto_Almacen_Categoria ON dbo.Precios
             DROP INDEX IX_ProductosStock_Producto_Deposito ON dbo.ProductosStock
   ===================================================================== */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Precios_Producto_Almacen_Categoria' AND object_id = OBJECT_ID('dbo.Precios'))
	CREATE NONCLUSTERED INDEX IX_Precios_Producto_Almacen_Categoria
	ON dbo.Precios (ProductoId, AlmacenId, PrecioCategoriaId)
	INCLUDE (Precio)
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductosStock_Producto_Deposito' AND object_id = OBJECT_ID('dbo.ProductosStock'))
	CREATE NONCLUSTERED INDEX IX_ProductosStock_Producto_Deposito
	ON dbo.ProductosStock (ProductoId, DepositoId, Lote, FechaVencimiento)
	INCLUDE (Cantidad)
GO
