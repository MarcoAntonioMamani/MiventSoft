/* =====================================================================
   MAM_PanelControl - SP propio del Panel de control de la web (MiventSoftWeb)
   El escritorio no lo usa. Solo lectura (SELECT); si falla, registra en
   Bitacora como los demas SP.

   @tipo=1  Hora de las ventas (Id, HoraRegistro)
            MAM_Ventas @tipo 3 no trae la hora: el panel la une por Id.
   @tipo=2  Compras por cliente y por dia (ClienteId, NombreCliente, FechaVenta, Compras, Monto)
            Para "Clientes que dejaron de comprar" y "Clientes rentables".
   @tipo=3  Productos con stock y SIN ventas desde @FechaI hasta @FechaF,
            con su ultima venta (null = nunca se vendio) y su precio de venta
            (categoria 1 "Precio Facturado", almacen 1, como MAM_Movimientos @tipo 18).

   Parametros: @FechaI / @FechaF como los demas reportes,
               @SucursalId = -1 todas las sucursales (igual que MAM_Ventas @tipo 3).
   Ventas validas: Estado = 1 y TotalVenta (igual que MAM_Ventas @tipo 3).

   INSTALAR: ejecutar este script completo en la BD (crea o reemplaza SOLO este SP).
   QUITAR:   DROP PROCEDURE [dbo].[MAM_PanelControl]
   ===================================================================== */
IF OBJECT_ID('dbo.MAM_PanelControl', 'P') IS NOT NULL
	DROP PROCEDURE [dbo].[MAM_PanelControl]
GO

CREATE PROCEDURE [dbo].[MAM_PanelControl] (@tipo int,@usuario nvarchar(50)='',@FechaI date=null,@FechaF date=null,@SucursalId int=-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()


	IF @tipo=1 --Hora de las ventas (grafico Ventas por hora)
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY

			select a.Id ,a.HoraRegistro
			from Ventas as a
			where a.Estado =1 and a.FechaVenta >=@FechaI and a.FechaVenta <=@FechaF
			and (@SucursalId <0 or a.SucursalId =@SucursalId)

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario )
		END CATCH
	END


	IF @tipo=2 --Compras por cliente y por dia (clientes que dejaron de comprar / clientes rentables)
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY

			select a.ClienteId ,c.NombreCliente ,a.FechaVenta ,count(a.Id) as Compras ,sum(a.TotalVenta) as Monto
			from Ventas as a
			inner join Clientes as c on c.Id =a.ClienteId
			where a.Estado =1 and a.FechaVenta >=@FechaI and a.FechaVenta <=@FechaF
			and (@SucursalId <0 or a.SucursalId =@SucursalId)
			group by a.ClienteId ,c.NombreCliente ,a.FechaVenta
			order by a.ClienteId ,a.FechaVenta

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@usuario )
		END CATCH
	END


	IF @tipo=3 --Productos con stock y sin ventas desde @FechaI, con su ultima venta
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY

			select p.Id ,p.CodigoExterno ,p.NombreProducto ,cat.NombreCategoria as Categoria ,stock.Stock
			,isnull(pre.Precio,0) as PrecioVenta ,stock.Stock *isnull(pre.Precio,0) as Valor ,ult.UltimaVenta
			from Productos as p
			inner join Categorias as cat on cat.Id =p.CategoriaId
			inner join (select st.ProductoId ,sum(st.Cantidad) as Stock
			            from ProductosStock as st
			            group by st.ProductoId) as stock on stock.ProductoId =p.Id
			outer apply (select top 1 aa.Precio from Precios as aa
			             where aa.ProductoId =p.Id and aa.PrecioCategoriaId =1 and aa.AlmacenId =1) as pre
			left join (select de.ProductoId ,max(v.FechaVenta) as UltimaVenta
			           from VentasDetalles as de
			           inner join Ventas as v on v.Id =de.VentaId
			           where v.Estado =1 and v.FechaVenta <=@FechaF
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
