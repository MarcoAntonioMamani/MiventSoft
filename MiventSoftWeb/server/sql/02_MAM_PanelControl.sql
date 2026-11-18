/* =====================================================================
   02 - MAM_PanelControl: SP propio del Panel de control de la web (Inicio)
   El escritorio no lo usa. Solo lectura (SELECT); si falla, registra en Bitacora como los demas SP.

   @tipo=2  Pedidos validos por cliente y por dia (ClienteId, NombreCliente, FechaVenta, Compras, Monto)
            -> "Clientes que dejaron de comprar" y "Clientes rentables".
   @tipo=3  Productos activos con stock y SIN pedidos desde @FechaI hasta @FechaF, con su ultima
            venta (null = nunca se vendio) y su precio de venta (primera categoria de venta,
            PreciosCategorias.Tipo = 1, almacen 1, la misma que actualiza MAM_Compras).
   Pedidos validos: Estado = 1 y Anulado = 0.

   INSTALAR: ejecutar este script completo (crea o reemplaza SOLO este SP). Se puede repetir.
   QUITAR:   revertir\REVERTIR_02_MAM_PanelControl.sql
   ===================================================================== */
IF OBJECT_ID('dbo.MAM_PanelControl', 'P') IS NOT NULL
	DROP PROCEDURE [dbo].[MAM_PanelControl]
GO

CREATE PROCEDURE [dbo].[MAM_PanelControl] (@tipo int,@usuario nvarchar(50)='',@FechaI date=null,@FechaF date=null)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()


	IF @tipo=2 --Pedidos por cliente y por dia (clientes que dejaron de comprar / clientes rentables)
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY

			select a.ClienteId ,c.NombreCliente ,a.FechaVenta ,count(a.Id) as Compras ,sum(a.TotalVenta) as Monto
			from Ventas as a
			inner join Clientes as c on c.Id =a.ClienteId
			where a.Estado =1 and a.Anulado =0 and a.FechaVenta >=@FechaI and a.FechaVenta <=@FechaF
			group by a.ClienteId ,c.NombreCliente ,a.FechaVenta
			order by a.ClienteId ,a.FechaVenta

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@usuario )
		END CATCH
	END


	IF @tipo=3 --Productos con stock y sin pedidos desde @FechaI, con su ultima venta
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY

			declare @CategoriaVenta int
			set @CategoriaVenta =(select Min(k.Id) from PreciosCategorias as k where k.Tipo =1)

			select p.Id ,p.CodigoExterno ,p.DescripcionProducto as NombreProducto ,cat.NombreCategoria as Categoria ,stock.Stock
			,isnull(pre.Precio,0) as PrecioVenta ,stock.Stock *isnull(pre.Precio,0) as Valor ,ult.UltimaVenta
			from Productos as p
			inner join Categorias as cat on cat.Id =p.CategoriaId
			inner join (select st.ProductoId ,sum(st.Cantidad) as Stock
			            from ProductosStock as st
			            group by st.ProductoId) as stock on stock.ProductoId =p.Id
			outer apply (select top 1 aa.Precio from Precios as aa
			             where aa.ProductoId =p.Id and aa.PrecioCategoriaId =@CategoriaVenta and aa.AlmacenId =1) as pre
			left join (select de.ProductoId ,max(v.FechaVenta) as UltimaVenta
			           from VentasDetalles as de
			           inner join Ventas as v on v.Id =de.VentaId
			           where v.Estado =1 and v.Anulado =0 and v.FechaVenta <=@FechaF
			           group by de.ProductoId) as ult on ult.ProductoId =p.Id
			where p.Estado =1 and stock.Stock >0
			and (ult.UltimaVenta is null or ult.UltimaVenta <@FechaI)
			order by Valor desc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH
	END

END
GO
RAISERROR('02 MAM_PanelControl INSTALADO.', 10, 1) WITH NOWAIT
GO
