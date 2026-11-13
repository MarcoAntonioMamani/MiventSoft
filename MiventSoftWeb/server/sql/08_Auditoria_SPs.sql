/* =====================================================================
   08 - AUDITORIA EN VENTAS, COMPRAS Y MOVIMIENTOS (web y escritorio usan los mismos SP)

   MAM_Ventas, MAM_Compras y MAM_Movimientos, en @tipo 2 (modificar) y @tipo -1 (eliminar):
     - al empezar: EXEC MAM_Auditoria @tipo=1 (foto ANTES: cabecera y detalle)
     - al terminar bien: EXEC MAM_Auditoria @tipo=2 (guarda antes, despues, usuario, fecha, equipo)
   Lo que hace cada SP queda IGUAL (ventas sigue borrando su detalle al eliminar, etc.).
   Si la auditoria falla, no impide grabar: el error queda en la Bitacora.

   Generado desde KailiIndustrialLaPaz (2026-11-13), CON StockSeguro (05) instalado. PROTECCION: solo se aplica si los 3 SP
   son esa version (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada.
   Si alguno es otra version, da error y no modifica nada. Los 3 SP van en UNA transaccion.
   REQUISITOS: 07_Auditoria.sql ejecutado antes y 05_StockSeguro.sql instalado.
   VOLVER ATRAS: revertir\REVERTIR_08_Auditoria_SPs.sql (SOLO para deshacer).
   ===================================================================== */
SET NOEXEC OFF
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas')) LIKE '%Kaily: auditoria%' AND OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) LIKE '%Kaily: auditoria%' AND OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Movimientos')) LIKE '%Kaily: auditoria%'
BEGIN
	RAISERROR('08 Auditoria en SPs YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
ELSE IF OBJECT_ID('dbo.MAM_Auditoria', 'P') IS NULL
BEGIN
	RAISERROR('08: falta ejecutar primero 07_Auditoria.sql (no existe MAM_Auditoria). No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
ELSE IF ISNULL(HASHBYTES('SHA2_256', REPLACE(REPLACE(REPLACE(REPLACE(UPPER(OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas'))), CHAR(13), ''), CHAR(10), ''), CHAR(9), ''), ' ', '')), 0x) <> 0x84B81D73681B60E5601CE0A5E1D5397FB19B30B743E159EA7A3209B394E7469F
   OR ISNULL(HASHBYTES('SHA2_256', REPLACE(REPLACE(REPLACE(REPLACE(UPPER(OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras'))), CHAR(13), ''), CHAR(10), ''), CHAR(9), ''), ' ', '')), 0x) <> 0x2277EEF4E5A567F3DD65FF8C52FF79ADB92A43C38A335F1D93646A686FAEF8EA
   OR ISNULL(HASHBYTES('SHA2_256', REPLACE(REPLACE(REPLACE(REPLACE(UPPER(OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Movimientos'))), CHAR(13), ''), CHAR(10), ''), CHAR(9), ''), ' ', '')), 0x) <> 0xA3D68311C8DEFF3D314B8ABDC2741B8AB08F8B15DF30A3F1F86622B365566209
BEGIN
	RAISERROR('08: algun SP es OTRA version (no la de KailiIndustrialLaPaz 2026-11-13 con StockSeguro). Si falta, ejecute antes 05_StockSeguro.sql. No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
GO
SET XACT_ABORT ON
BEGIN TRAN
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
--drop procedure MAM_Ventas 


ALTER PROCEDURE [dbo].[MAM_Ventas](@tipo int,@Id int=-1,@SucursalId int=-1,@FechaVenta date=null,
@PersonalId int =-1,@TipoVenta int=-1,@FechaVencimientoCredito date=null,
@ClienteId int=-1,@MonedaVenta int=-1,@Estado int=-1,
@Glosa nvarchar(450)='',@Descuento decimal(18,2)=0,@TotalVenta decimal(18,2)=0
,@VentaDetalleType VentaDetalle02Type Readonly,@usuario nvarchar(50)='',@TipoCambio decimal(18,2)=0,@VentaPagos VentaMonedatype Readonly,
@Desde date=null,@Hasta date=null,@ObservacionDespacho nvarchar(400)='',@VentaId int=-1,@Descripcion nvarchar(250)='',@TipoMovimiento int=-1,@Facturado int=-1,@CategoriaVentaId int=-1,
@Despachado int =-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()
	DECLARE @AudAntes xml /* Kaily: auditoria */
	
	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
	SET ARITHABORT ON
		BEGIN TRY
			EXEC MAM_Auditoria @tipo=1, @Modulo=1, @DocumentoId=@Id, @Foto=@AudAntes OUTPUT /* Kaily: auditoria: foto antes */

		    delete VentasMonedaCobrada where VentaId =@Id 
			delete TransaccionVentasCredito where TransaccionVentasCredito .CreditoVentaId in (
			select Id
			from CreditosVentas where VentaId =@Id )
			delete from TransaccionesVentasCreditoDetalle where VentaCreditoId in  (
			select Id
			from CreditosVentas where VentaId =@Id )
			delete CreditosVentas where VentaId =@Id 
		
			Update Ventas set Estado =-1 where Id=@Id 
			delete from VentasDetalles   where VentaId  =@Id;
			EXEC MAM_Auditoria @tipo=2, @Modulo=1, @Operacion=2, @DocumentoId=@Id, @Antes=@AudAntes, @usuario=@usuario /* Kaily: auditoria: eliminado */
			SELECT 1 as resp;
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@usuario)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN INSERTAR
		-- ===== Kaily: validacion de stock (web y escritorio) =====================================
		-- Lo que esta operacion descuenta por producto y deposito. Las filas de stock quedan BLOQUEADAS (UPDLOCK)
		-- hasta el COMMIT: si otro usuario graba el mismo producto a la vez, espera y despues ve el stock real.
		-- Si no alcanza se revierte todo y se avisa que productos y con que stock actual.
		DECLARE @efStockV1 TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))
		INSERT INTO @efStockV1 (ProductoId, DepositoId, Delta)
		SELECT td.ProductoId, (SELECT a.DepositoId FROM Almacenes AS a WHERE a.Id = @SucursalId), -SUM(td.Cantidad)
		FROM @VentaDetalleType AS td WHERE td.estado >= 0 AND td.ProductoId > 0 GROUP BY td.ProductoId
		DECLARE @msgStockV1 nvarchar(2000)
		SELECT @msgStockV1 = STUFF((
		    SELECT TOP 10 '; ' + RTRIM(p.NombreProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))
		        + ', necesita ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'
		    FROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStockV1 GROUP BY ProductoId, DepositoId) AS e
		    INNER JOIN Productos AS p ON p.Id = e.ProductoId
		    OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)
		                 WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s
		    WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0
		    ORDER BY p.NombreProducto
		    FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')
		IF @msgStockV1 IS NOT NULL
		BEGIN
		    ROLLBACK TRAN
		    SET @msgStockV1 = 'STOCK INSUFICIENTE: ' + @msgStockV1
		    RAISERROR('%s', 16, 1, @msgStockV1)
		    RETURN
		END
		-- ==========================================================================================
		BEGIN TRY 
		  set @Id=IIF((select COUNT(Id) from Ventas)=0,0,(select MAX(Id) from Ventas))+1
			INSERT INTO Ventas  VALUES(@Id ,@SucursalId ,@FechaVenta ,@PersonalId ,@TipoVenta ,
			@FechaVencimientoCredito ,@ClienteId ,@MonedaVenta ,@Estado ,@Glosa ,
			@Descuento ,@TotalVenta ,@newFecha,@newHora,@Usuario,4,0,@FechaVenta,@Id,'SISTEMA',0,0,@Facturado )

			
			INSERT INTO VentasDetalles (VentaId ,ProductoId ,Cantidad ,Precio ,SubTotal ,ProcentajeDescuento 
			,MontoDescuento, Total ,Detalle ,PrecioCosto ,Lote ,FechaVencimiento,Tipo,KitId,CantidadKit  ,FechaRegistro ,HoraRegistro ,UsuarioRegistro)


			SELECT @Id,td.ProductoId ,td.Cantidad  ,td.Precio ,td.Subtotal ,td.PorcentajeDescuento ,td.MontoDescuento ,
			td.Total ,td.Detalle ,td.PrecioCosto ,td.Lote ,td.FechaVencimiento,td.Tipo,td.KitId,td.CantidadKit  ,
			@newFecha  ,@newHora  ,@Usuario    FROM @VentaDetalleType  AS td
			where td.estado  >=0 and  td.ProductoId  >0

			-------------Backup

				INSERT INTO VentasDetallesBackup (VentaId ,ProductoId ,Cantidad ,Precio ,SubTotal ,ProcentajeDescuento 
			,MontoDescuento, Total ,Detalle ,PrecioCosto ,Lote ,FechaVencimiento,Tipo,KitId,CantidadKit  ,FechaRegistro ,HoraRegistro ,UsuarioRegistro,estado)


			SELECT @Id,td.ProductoId ,td.Cantidad  ,td.Precio ,td.Subtotal ,td.PorcentajeDescuento ,td.MontoDescuento ,
			td.Total ,td.Detalle ,td.PrecioCosto ,td.Lote ,td.FechaVencimiento,td.Tipo,td.KitId,td.CantidadKit  ,
			@newFecha  ,@newHora  ,@Usuario,td.estado     FROM @VentaDetalleType  AS td
			where td.estado  >=0 and  td.ProductoId  >0

			--------------


			INSERT INTO VentasMonedaCobrada (VentaId ,MontoBs ,MontoDolares ,TarjetaBancaria ,TransferenciaBancaria,TipoCambio )


			SELECT @Id,td.MontoBs ,td.MontoDolares ,td.TarjetaBancaria ,td.TransferenciaBancaria,td.TipoCambio    FROM @VentaPagos   AS td
			where td.estado  =0 
			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT @Id AS newNumi
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario )

			ROLLBACK TRAN
		END CATCH
	END
	IF @tipo=2--MODIFICACION
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN MODIFICACION
		-- ===== Kaily: validacion de stock (web y escritorio) =====================================
		-- Lo que esta operacion descuenta por producto y deposito. Las filas de stock quedan BLOQUEADAS (UPDLOCK)
		-- hasta el COMMIT: si otro usuario graba el mismo producto a la vez, espera y despues ve el stock real.
		-- Si no alcanza se revierte todo y se avisa que productos y con que stock actual.
		DECLARE @efStockV2 TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))
		INSERT INTO @efStockV2 (ProductoId, DepositoId, Delta)
		SELECT td.ProductoId, (SELECT a.DepositoId FROM Almacenes AS a WHERE a.Id = @SucursalId), -td.Cantidad
		FROM @VentaDetalleType AS td WHERE td.estado >= 0 AND td.ProductoId > 0
		UNION ALL -- lo que la venta ya tenia vuelve a su deposito
		SELECT d.ProductoId, a.DepositoId, d.Cantidad
		FROM VentasDetalles AS d INNER JOIN Ventas AS v ON v.Id = d.VentaId INNER JOIN Almacenes AS a ON a.Id = v.SucursalId
		WHERE d.VentaId = @Id AND v.Estado <> -1
		DECLARE @msgStockV2 nvarchar(2000)
		SELECT @msgStockV2 = STUFF((
		    SELECT TOP 10 '; ' + RTRIM(p.NombreProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))
		        + ', necesita ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'
		    FROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStockV2 GROUP BY ProductoId, DepositoId) AS e
		    INNER JOIN Productos AS p ON p.Id = e.ProductoId
		    OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)
		                 WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s
		    WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0
		    ORDER BY p.NombreProducto
		    FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')
		IF @msgStockV2 IS NOT NULL
		BEGIN
		    ROLLBACK TRAN
		    SET @msgStockV2 = 'STOCK INSUFICIENTE: ' + @msgStockV2
		    RAISERROR('%s', 16, 1, @msgStockV2)
		    RETURN
		END
		-- ==========================================================================================
		BEGIN TRY 
			EXEC MAM_Auditoria @tipo=1, @Modulo=1, @DocumentoId=@Id, @Foto=@AudAntes OUTPUT /* Kaily: auditoria: foto antes */

	------------
			--INSERTO LOS Detalle antes de Eliminar

				INSERT INTO VentasDetallesBackup (VentaId ,ProductoId ,Cantidad ,Precio ,SubTotal ,ProcentajeDescuento 
			,MontoDescuento, Total ,Detalle ,PrecioCosto ,Lote ,FechaVencimiento,Tipo,KitId,CantidadKit  ,FechaRegistro ,HoraRegistro ,UsuarioRegistro,estado)


			SELECT @Id,td.ProductoId ,td.Cantidad  ,td.Precio ,td.Subtotal ,td.ProcentajeDescuento  ,td.MontoDescuento ,
			td.Total ,td.Detalle ,td.PrecioCosto ,td.Lote ,td.FechaVencimiento,td.Tipo,td.KitId,td.CantidadKit  ,
			@newFecha  ,@newHora  ,'Eliminado',1    FROM VentasDetalles  AS td
			where td.VentaId=@ID


			-------------Backup

			--ELIMINO LOS REGISTROS
			DELETE FROM VentasDetalles   WHERE VentaId =@Id 
			delete from VentasMonedaCobrada where VentaId =@Id 


			Update Ventas set SucursalId =@SucursalId ,FechaVenta =@FechaVenta ,PersonalId =@PersonalId ,
			TipoVenta =@TipoVenta ,FechaVencimientoCredito =@FechaVencimientoCredito ,ClienteId =@ClienteId ,
			MonedaVenta =@MonedaVenta ,Estado =@Estado ,Glosa =@Glosa ,Descuento =@Descuento ,TotalVenta =@TotalVenta,Facturado=@Facturado 
			where Id=@Id 

		 ----------MODIFICO EL DEcaLLE DE EQUIPO------------
			--INSERTO LOS NUEVOS

			INSERT INTO VentasDetalles (VentaId ,ProductoId ,Cantidad ,Precio ,SubTotal ,ProcentajeDescuento 
			,MontoDescuento, Total ,Detalle ,PrecioCosto ,Lote ,FechaVencimiento,Tipo,KitId,CantidadKit  ,FechaRegistro ,HoraRegistro ,UsuarioRegistro)


			SELECT @Id,td.ProductoId ,td.Cantidad  ,td.Precio ,td.Subtotal ,td.PorcentajeDescuento ,td.MontoDescuento ,
			td.Total ,td.Detalle ,td.PrecioCosto ,td.Lote ,td.FechaVencimiento,td.Tipo,td.KitId,td.CantidadKit  ,
			@newFecha  ,@newHora  ,@Usuario    FROM @VentaDetalleType  AS td
			where td.estado  >=0 and  td.ProductoId  >0


			-------------Backup

			INSERT INTO VentasDetallesBackup (VentaId ,ProductoId ,Cantidad ,Precio ,SubTotal ,ProcentajeDescuento 
			,MontoDescuento, Total ,Detalle ,PrecioCosto ,Lote ,FechaVencimiento,Tipo,KitId,CantidadKit  ,FechaRegistro ,HoraRegistro ,UsuarioRegistro,estado)


			SELECT @Id,td.ProductoId ,td.Cantidad  ,td.Precio ,td.Subtotal ,td.PorcentajeDescuento ,td.MontoDescuento ,
			td.Total ,td.Detalle ,td.PrecioCosto ,td.Lote ,td.FechaVencimiento,td.Tipo,td.KitId,td.CantidadKit  ,
			@newFecha  ,@newHora  ,@Usuario,td.estado     FROM @VentaDetalleType  AS td
			where td.estado  >=0 and  td.ProductoId  >0

			--------------

			--------Inserto Detalle Pago
			INSERT INTO VentasMonedaCobrada (VentaId ,MontoBs ,MontoDolares ,TarjetaBancaria ,TransferenciaBancaria,TipoCambio )


			SELECT @Id,td.MontoBs ,td.MontoDolares ,td.TarjetaBancaria ,td.TransferenciaBancaria,td.TipoCambio    FROM @VentaPagos   AS td
			where td.estado  >=0 
				------Modifico los Precios Costo------------------
			EXEC MAM_Auditoria @tipo=2, @Modulo=1, @Operacion=1, @DocumentoId=@Id, @Antes=@AudAntes, @usuario=@usuario /* Kaily: auditoria: modificado */
			select @Id as newNumi
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN -- Kaily: deshace todo (la bitacora se graba despues y queda)
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@Usuario)
		END CATCH
		-- Kaily: la modificacion ahora es atomica (antes no tenia transaccion: si fallaba se perdia el detalle)
		IF @@TRANCOUNT > 0 COMMIT TRAN
	END

		IF @tipo=3 --MOSTrar Ventas
	BEGIN
	SET ARITHABORT ON
		BEGIN TRY
		
		if (@SucursalId >=0)

		begin
		
		select a.Id ,a.SucursalId ,a.FechaVenta ,a.PersonalId,p.NombrePersonal as Personal,
		a.TipoVenta,IIF(a.TipoVenta =1,'Contado','Credito')as TVenta,a.FechaVencimientoCredito ,
		a.ClienteId ,c.NombreCliente ,a.MonedaVenta ,a.Estado ,a.Glosa ,a.Descuento ,a.TotalVenta,
		isnull(pago.MontoBs,0) as MontoBs ,isnull(pago.MontoDolares,0)as MontoDolares ,isnull(pago.TarjetaBancaria,0)as TarjetaBancaria
		 ,isnull(pago.TransferenciaBancaria,0)as TransferenciaBancaria,isnull(pago.TipoCambio,0) as TipoCambio,al.NombreAlmacen,
		 isnull((select top 1 cierre .CierreCajeroId   from CierreCajeroReferenciasModulos as cierre where cierre.Modulo =1 and cierre.ModuloId =a.Id ),0)as CierreModulo,a.Facturado,
	 isnull(IIF(desp.Id>0,1,0),0) as  EstadoDespacho,	 isnull(IIF(desp.Id>0,'DESPACHADO','NO DESPACHADO'),'NO DESPACHADO') as  TDespachado,isnull(desp.FechaDespacho,'') as FechaDespacho,isnull(despachante.NombrePersonal,'') as Despachante,
	 isnull(desp.PersonalId,0) as PersonalDespachadorId,isnull(desp .Observacion,'') as ObservacionDespacho
		from Ventas as a 
		inner join Personal as p on p.Id =a.PersonalId 
		inner join Clientes as c on c.Id =a.ClienteId 
		left join VentasMonedaCobrada as pago on pago.VentaId =a.Id 
		inner join Almacenes as al on al.id =a.SucursalId 
		left join despacho as desp on desp.ventaid=a.id
		left join personal as despachante on despachante.id=desp.PersonalId
		where a.Estado =1 and a.FechaVenta >=@Desde and a.FechaVenta <=@Hasta and a.SucursalId =@SucursalId 
		order by a.Id desc

		end
		else
		begin
		
		select a.Id ,a.SucursalId ,a.FechaVenta ,a.PersonalId,p.NombrePersonal as Personal,
		a.TipoVenta,IIF(a.TipoVenta =1,'Contado','Credito')as TVenta,a.FechaVencimientoCredito ,
		a.ClienteId ,c.NombreCliente ,a.MonedaVenta ,a.Estado ,a.Glosa ,a.Descuento ,a.TotalVenta,
		isnull(pago.MontoBs,0) as MontoBs ,isnull(pago.MontoDolares,0)as MontoDolares ,isnull(pago.TarjetaBancaria,0)as TarjetaBancaria
		 ,isnull(pago.TransferenciaBancaria,0)as TransferenciaBancaria,isnull(pago.TipoCambio,0) as TipoCambio,al.NombreAlmacen,
		 	 isnull((select top 1 cierre .CierreCajeroId  from CierreCajeroReferenciasModulos as cierre where cierre.Modulo =1 and cierre.ModuloId =a.Id ),0)as CierreModulo,a.Facturado  
		,	 isnull(IIF(desp.Id>0,1,0),0) as  EstadoDespacho,	 isnull(IIF(desp.Id>0,'DESPACHADO','NO DESPACHADO'),'NO DESPACHADO') as  TDespachado,isnull(desp.FechaDespacho,'') as FechaDespacho,isnull(despachante.NombrePersonal,'') as Despachante,
			 isnull(desp.PersonalId,0) as PersonalDespachadorId,isnull(desp .Observacion,'') as ObservacionDespacho
		from Ventas as a 
		inner join Personal as p on p.Id =a.PersonalId 
		inner join Clientes as c on c.Id =a.ClienteId 
		left join VentasMonedaCobrada as pago on pago.VentaId =a.Id 
		inner join Almacenes as al on al.id =a.SucursalId 
		left join despacho as desp on desp.ventaid=a.id
		left join personal as despachante on despachante.id=desp.PersonalId
		where a.Estado =1 and a.FechaVenta >=@Desde and a.FechaVenta <=@Hasta 
		order by a.Id desc
		end



		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=4 --Listar Detalle Venta
	BEGIN
	SET ARITHABORT ON
		BEGIN TRY
		
	select a.Id ,a.VentaId ,a.ProductoId ,p.NombreProducto as Producto,a.Cantidad ,a.Precio ,a.SubTotal ,
		a.ProcentajeDescuento ,a.MontoDescuento ,a.Total ,a.Detalle ,a.PrecioCosto ,a.Lote ,a.FechaVencimiento,isnull(a.Tipo,1)as Tipo,
		IIF(a.Tipo=2,'Kits','Productos')as TipoNombre,isnull(a.KitId,0) as KitId,isnull((select k.NombreKit  from Kits as k where k.id=a.KitID),'')as KitNombre,
		isnull(a.CantidadKit,0) as CantidadKit ,1 as estado,cast ('' as image ) as img
		,(select sum(st.Cantidad ) from ProductosStock as st
		 where st.ProductoId =p.Id and st.Lote =a.Lote and  st.FechaVencimiento =a.FechaVencimiento )+a.Cantidad  as stock
		from VentasDetalles as a
		inner join Productos as p on p.Id =a.ProductoId 
		where a.VentaId =@Id 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=5 --MOSTRaR Productos Para Venta
	BEGIN
	SET ARITHABORT ON
		BEGIN TRY
	
	
     Declare @CategoriaCompra int
	 
	

	 set @CategoriaCompra =(select Min(k.Id ) from PreciosCategorias  as k where k.Tipo  =0)
  select a.Id ,a.CodigoExterno ,a.NombreProducto,a.NombreProducto as DescripcionProducto,industria .Descripcion as industria,cat.NombreCategoria   ,PCosto .Precio as PrecioCosto,PVenta .Precio as PrecioVenta,1 as estado,
	sum(st.Cantidad ) as stock
	 ,1 as Tipo,'Producto' as NombreTipo
	 from Productos as a 
	 inner join ProductosStock as st on st.ProductoId =a.Id and st.DepositoId =@SucursalId 
	 inner join Precios as PCosto on PCosto .AlmacenId =1  and PCosto .PrecioCategoriaId =@CategoriaCompra 
	 and PCosto.ProductoId =a.Id 
	 inner join Precios as PVenta on PVenta .AlmacenId =1  and PVenta .PrecioCategoriaId =@CategoriaVentaId 
	 and PVenta .ProductoId =a.Id 
	 inner join Categorias as cat on cat.id=a.CategoriaId 
	 inner join ClasificadorDetalle as industria on industria .Id =a.AttributoId 

	 group by  a.Id ,a.CodigoExterno ,a.NombreProducto,a.DescripcionProducto,industria .Descripcion,cat.NombreCategoria   ,PCosto .Precio ,PVenta .Precio 

	 	 union

	 select a.id as Id,'' as CodigoExterno,a.NombreKit as NombreProducto,a.DescripcionKit as DescripcionProducto,'' as industria,
	 'KIT' as Categoria,0 as PrecioCosto,a.Total as PrecioVenta,1 as estado,dbo.StockPorKits(a.id,@SucursalId )as stock,
	 2 as Tipo,'Kits' as NombreTipo
     from Kits as a


	 order by a.NombreProducto  asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=6 --Recibo de Venta
	BEGIN
	SET ARITHABORT ON
		BEGIN TRY
		
		
SELECT '00000' + CAST(a.Id AS nvarchar(20)) AS nroRecibo,FORMAT (a.FechaVenta , 'dd-MM-yyyy') as FechaVenta, cli.NombreCliente, IIF(cli.DireccionCliente='','S/D', cli.DireccionCliente)as DireccionCliente
		, IIF(cli.NroDocumento='','0',cli.NroDocumento) as NroDocumento, IIF(cli.Telefono='','0',cli.Telefono)as Telefono, p.Id AS ProductoId, p.NombreProducto, cast(Sum(detalle.Cantidad) as int) as Cantidad
		, detalle.Precio, Sum(detalle.SubTotal) as SubTotal, 
                  Sum(detalle.MontoDescuento) as MontoDescuento, Sum(detalle.Total) as Total, empresa.Nombre AS Empresa,IIF(a.SucursalId=1, empresa.Direccion,'Z.Santiago 1,calle 7 #101,Av 1 y 2 Cel/WHATSAPP:65432226') AS DireccionEmpresa, empresa.Ciudad, CAST('' AS varbinary) AS imageEmpresa,
                      (SELECT TOP (1) Imagen
                       FROM      dbo.Empresa AS Empresa_1) AS rutaImagen, per.NombrePersonal AS vendedor, a.Glosa,
					   IIF(a.TipoVenta=1,'Contado','Credito') as TipoVenta ,IIF(a.TipoVenta=1,'',FORMAT (a.fechaVencimientoCredito , 'dd-MM-yyyy') )as FechaVencimientoCredito,a.Descuento as DescuentoVenta,
					   Unidad .Descripcion as Unidad
				  ,isnull((select top 1 aa.TipoCambio  from VentasMonedaCobrada as aa where aa.VentaId =a.Id ),6.95) as TipoCambio
FROM     dbo.Ventas AS a INNER JOIN
                  dbo.Clientes AS cli ON cli.Id = a.ClienteId INNER JOIN
                  dbo.VentasDetalles AS detalle ON detalle.VentaId = a.Id INNER JOIN
                  dbo.Productos AS p ON p.Id = detalle.ProductoId INNER JOIN
                  dbo.Empresa AS empresa ON empresa.Id = 1 INNER JOIN
                  dbo.Personal AS per ON per.Id = a.PersonalId
				    inner join ClasificadorDetalle as Unidad on Unidad .Id =p.UnidadVentaId 

WHERE   a.Id =@Id  and detalle.Tipo =1

group by a.Id ,a.FechaVenta ,cli .NombreCliente ,cli.DireccionCliente,cli.NroDocumento,a.SucursalId  ,cli.Telefono ,p.Id ,p.NombreProducto,
empresa.Nombre,detalle.precio ,empresa .Direccion,empresa .Ciudad,per.NombrePersonal,a.Glosa,a.TipoVenta,a.FechaVencimientoCredito,a.Descuento,Unidad .Descripcion  


union 

		SELECT distinct '00000' + CAST(a.Id AS nvarchar(20)) AS nroRecibo,FORMAT (a.FechaVenta , 'dd-MM-yyyy') as FechaVenta, cli.NombreCliente, IIF(cli.DireccionCliente='','S/D', cli.DireccionCliente)as DireccionCliente
		, IIF(cli.NroDocumento='','0',cli.NroDocumento) as NroDocumento, IIF(cli.Telefono='','0',cli.Telefono)as Telefono, k.id AS ProductoId,  k.NombreKit as NombreProducto, cast(detalle.CantidadKit as int)  as Cantidad
		,(select Sum(det.Total)  from VentasDetalles as det where det.VentaId =a.Id and det.KitId =detalle.KitId )/detalle.CantidadKit   as Precio,(select Sum(det.Total)  from VentasDetalles as det where det.VentaId =a.Id and det.KitId =detalle.KitId ) as SubTotal, 
                  Sum(detalle.MontoDescuento) as MontoDescuento, (select Sum(det.Total)  from VentasDetalles as det where det.VentaId =a.Id and det.KitId =detalle.KitId ) as Total, empresa.Nombre AS Empresa, empresa.Direccion AS DireccionEmpresa, empresa.Ciudad, CAST('' AS varbinary) AS imageEmpresa,
                      (SELECT TOP (1) Imagen
                       FROM      dbo.Empresa AS Empresa_1) AS rutaImagen, per.NombrePersonal AS vendedor, a.Glosa,
					   IIF(a.TipoVenta=1,'Contado','Credito') as TipoVenta ,IIF(a.TipoVenta=1,'',FORMAT (a.fechaVencimientoCredito , 'dd-MM-yyyy') )as FechaVencimientoCredito,a.Descuento as DescuentoVenta,
					   'KIT' as Unidad
				  ,isnull((select top 1 aa.TipoCambio  from VentasMonedaCobrada as aa where aa.VentaId =a.Id ),6.95) as TipoCambio
FROM     dbo.Ventas AS a INNER JOIN
                  dbo.Clientes AS cli ON cli.Id = a.ClienteId INNER JOIN
                  dbo.VentasDetalles AS detalle ON detalle.VentaId = a.Id INNER JOIN
                  dbo.Empresa AS empresa ON empresa.Id = 1 INNER JOIN
                  dbo.Personal AS per ON per.Id = a.PersonalId
					inner join Kits as k on k.id=detalle .KitId 
WHERE   a.Id =@Id  and detalle.Tipo =2

group by a.Id ,a.FechaVenta ,cli .NombreCliente ,cli.DireccionCliente,cli.NroDocumento ,cli.Telefono ,
empresa.Nombre,detalle.precio ,empresa .Direccion,empresa .Ciudad,per.NombrePersonal,a.Glosa,a.TipoVenta,a.FechaVencimientoCredito,a.Descuento,k.id,k.NombreKit,detalle.KitId,detalle.CantidadKit 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END
	IF @tipo=7 --Insertar tipo De Cambio
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN INSERTAR
		BEGIN TRY 
		  set @Id=IIF((select COUNT(Id) from TipoCambio)=0,0,(select MAX(Id) from TipoCambio))+1
			
		insert into TipoCambio values(@Id,@TipoCambio,GETDATE (),@usuario )
			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT @Id AS newNumi
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario )

			ROLLBACK TRAN
		END CATCH
	END

		IF @tipo=8 --MontoCobrado
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN INSERTAR
		BEGIN TRY 
		 
		 select a.Id ,a.VentaId ,a.MontoBs ,a.MontoDolares,a.TarjetaBancaria  ,a.TransferenciaBancaria,a.TipoCambio,1 as estado
		 from VentasMonedaCobrada as a where a.VentaId =@Id 
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario )

			ROLLBACK TRAN
		END CATCH
	END

		IF @tipo=9 --Insertar tipo Movimiento
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN INSERTAR
		BEGIN TRY 
		  set @Id=IIF((select COUNT(Id) from CajaTipoMovimiento)=0,0,(select MAX(Id) from CajaTipoMovimiento))+1
			
		insert into CajaTipoMovimiento values(@Id,@Descripcion,@TipoMovimiento ,IIF(@TipoMovimiento=1,1,-1),1 )
			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT @Id AS newNumi
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario )

			ROLLBACK TRAN
		END CATCH
	END
	IF @tipo=10 --ProductosKit
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN INSERTAR
		BEGIN TRY 
		select ki.ProductoId,p.NombreProducto,ki.Cantidad,pre.Precio as PrecioCosto,ki.Precio as PrecioVenta 
from KitsProductos as ki
inner join Productos as p on ki.ProductoId =p.Id 
inner join Precios as pre on pre.ProductoId =p.id and pre.AlmacenId =1 and pre.PrecioCategoriaId =2

where ki.KitId =@Id 
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario )

			ROLLBACK TRAN
		END CATCH
	END

	
	IF @tipo=11 --Insertar Despacho
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN INSERTAR
		BEGIN TRY 
		  set @Id=IIF((select COUNT(Id) from Despacho)=0,0,(select MAX(Id) from Despacho))+1
		  if (@Despachado=1)
		  begin
		  	if (not exists(select * from despacho where ventaId=@VentaId))
			begin
			insert into Despacho values(@VentaId,@newFecha,@newHora,@usuario,@ObservacionDespacho,'',@PersonalId)
			end
		  end
		   if (@Despachado=0)
		  begin
		  if ( exists(select * from despacho where ventaId=@VentaId))
			begin
			delete from Despacho where ventaid=@VentaId
			end
		  
		  end

			SELECT @Id AS newNumi
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario )

			ROLLBACK TRAN
		END CATCH
	END
END





GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
--Drop Procedure [MAM_Compras]
ALTER PROCEDURE [dbo].[MAM_Compras] (@tipo int,@Id int=-1,@AlmacenId int =-1,
@FechaTransaccion date=null,@Proveedor int=-1,@TipoVenta int=-1,
@FechaVencimientoCredito date=null,@Moneda int=-1,@Estado int=-1,@Glosa nvarchar(450)='',
@TotalCompra decimal(18,2)=0,@Descuento decimal(18,2)=0,@Usuario nvarchar(10)='',@detalle CompraDetalleType ReadOnly, @CatCosto int=-1,
@EmpresaId int=-1,@desde date=null,@hasta date =null)
	
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))
	declare @numicat int
	declare @ygmer int
	declare @numicatVenta int
	DECLARE @newFecha date
	Declare @IdCajaIngresoEgreso int
	declare @Prec decimal(18,4)
	set @newFecha=GETDATE()
	DECLARE @AudAntes xml /* Kaily: auditoria */

	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY 
			EXEC MAM_Auditoria @tipo=1, @Modulo=2, @DocumentoId=@Id, @Foto=@AudAntes OUTPUT /* Kaily: auditoria: foto antes */
		   delete from CajaIngresoEgreso  where Modulo =2 and IdModulo =@Id and CajaId =1
		   
update Configuracion set TotalCajaGeneral=(
select Sum((c.monto*a.Operacion)) as Total
from CajaIngresoEgreso as c
inner join CajaTipoMovimiento as a on c.CajatipoMovimientoId =a.Id 
 where CajaId =1)


		delete from CreditosCompras  where CompraId  =@Id 
		DELETE FROM CompraDetalle   WHERE CompraId   =@Id
			DELETE from Compras    where Id   =@Id
		
			EXEC MAM_Auditoria @tipo=2, @Modulo=2, @Operacion=2, @DocumentoId=@Id, @Antes=@AudAntes, @usuario=@Usuario /* Kaily: auditoria: eliminado */
			select @Id as newNumi  --Consulcar que hace newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@Usuario)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY 
        set @Id=IIF((select COUNT(Id) from Compras)=0,0,(select MAX(Id) from Compras))+1
			INSERT INTO Compras VALUES(@Id ,@AlmacenId,@FechaTransaccion ,@Proveedor  ,@TipoVenta,
			@FechaVencimientoCredito,@Moneda ,@Estado  ,@Glosa ,@Descuento  ,@TotalCompra ,@EmpresaId,@newFecha,@newHora,@Usuario)


				------Modifico los Precios Costo------------------
			
			

			set @numicat =(select Min(Id ) from PreciosCategorias  where Tipo  =0)

			--update Precios  set Precios.Precio  =td.PrecioCosto  
			--from Precios INNER JOIN @detalle AS td
			--ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId  
			--and Precios.AlmacenId  =@AlmacenId   and td.estado =0 and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

			--	update Precios  set Precios.Precio  =(((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )*Precios.Precio  )+(td.Cantidad *td.PrecioCosto ))/((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )+td.Cantidad )
			--from Precios INNER JOIN @detalle AS td
			--ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId  
			--and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

			----INSERTO EL DEcaLLE
				INSERT INTO CompraDetalle (CompraId ,ProductoId ,CantidadCompra,PorcentajeIncremento,CantidadIncremento ,Cantidad  ,PrecioCosto,Lote 
				 ,FechaVencimiento   ,PrecioVenta   ,TotalCompra  
				,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro  )

			SELECT @Id,td.ProductoId ,td.CantidadCompra,td.PorcentajeIncremento,td.CantidadIncremento ,td.Cantidad  ,td.PrecioCosto  ,
			td.Lote  ,td.FechaVencimiento  ,td.PrecioVenta ,td.TotalCompra  ,
			@newFecha  ,@newHora  ,@Usuario    FROM @detalle AS td
			where td.estado  =0 and  td.ProductoId  >0 and td.Cantidad >0
		

			------Modifico los Precios Venta------------------
			set @numicatVenta =(select Min(Id) from PreciosCategorias where Tipo =1)
			--update Precios set Precios.Precio  =td.PrecioVenta   
			--from Precios INNER JOIN @detalle AS td
			--ON  Precios .PrecioCategoriaId  =@numicatVenta and Precios .ProductoId  =td.ProductoId  
			--and Precios.AlmacenId  =@AlmacenId   and td.estado =0 and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0
			select @Id as newNumi

		


		----Modulo 2= compras
			if (not exists(select a.* from CajaIngresoEgreso as a where a.Modulo =2 and a.IdModulo =@Id and a.CajaId =1) and @TipoVenta =1)
			begin 


			set @IdCajaIngresoEgreso=IIF((select COUNT(Id) from CajaIngresoEgreso)=0,0,(select MAX(Id) from CajaIngresoEgreso))+1
			insert into CajaIngresoEgreso values(@IdCajaIngresoEgreso,0,@FechaTransaccion ,'Egreso Por Compras A Proveedor '+(isnull((select top 1 pr.NombreProveedor  from Proveedor as pr where pr.Id =@Proveedor ),'')),
			@TotalCompra ,1,0,20,1,2,@Id,@AlmacenId ,0,0,@Usuario,@newFecha,@newHora )



			
update Configuracion set TotalCajaGeneral=(
select Sum((c.monto*a.Operacion)) as Total
from CajaIngresoEgreso as c
inner join CajaTipoMovimiento as a on c.CajatipoMovimientoId =a.Id 
 where CajaId =1)

			end


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@Usuario)
		END CATCH
	END
	
	IF @tipo=2--MODIFICACION
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY 
			EXEC MAM_Auditoria @tipo=1, @Modulo=2, @DocumentoId=@Id, @Foto=@AudAntes OUTPUT /* Kaily: auditoria: foto antes */


			--ELIMINO LOS REGISTROS
			DELETE FROM CompraDetalle  WHERE CompraId=@Id

			Update Compras set AlmacenId=@AlmacenId ,ProveedorId =@Proveedor ,TipoVenta =@TipoVenta ,
			FechaVencimientoCredito =@FechaVencimientoCredito ,Moneda =@Moneda , Estado =@Estado ,
			Glosa =@Glosa ,TotalCompra =@TotalCompra ,EmpresaId =@EmpresaId ,descuento=@Descuento 
			where Id=@Id 

			------Modifico los Precios Costo------------------
				

				---Tipo =1 Venta   0=Compra


			set @numicat =(select Min(Id ) from PreciosCategorias  where Tipo  =0)

			--update Precios  set Precios.Precio  =(((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )*Precios.Precio  )+(td.Cantidad *td.PrecioCosto ))/((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )+td.Cantidad )
			--from Precios INNER JOIN @detalle AS td
			--ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId  
			--and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

			--update Precios  set Precios.Precio  =td.PrecioCosto  
			--from Precios INNER JOIN @detalle AS td
			--ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId  
			--and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

		 ----------MODIFICO EL DEcaLLE DE EQUIPO------------
			--INSERTO LOS NUEVOS


			
					INSERT INTO CompraDetalle (CompraId ,ProductoId ,CantidadCompra,PorcentajeIncremento,CantidadIncremento ,Cantidad  ,PrecioCosto,Lote 
				 ,FechaVencimiento   ,PrecioVenta   ,TotalCompra  
				,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro  )

			SELECT @Id,td.ProductoId ,td.CantidadCompra,td.PorcentajeIncremento,td.CantidadIncremento ,td.Cantidad  ,td.PrecioCosto  ,
			td.Lote  ,td.FechaVencimiento  ,td.PrecioVenta ,td.TotalCompra  ,
			@newFecha  ,@newHora  ,@Usuario    FROM @detalle AS td
			where td.estado  >=0 and  td.ProductoId  >0 and td.Cantidad >0


			------Modifico los Precios Venta------------------
			set @numicatVenta =(select Min(Id) from PreciosCategorias where Tipo =1)
			--update Precios set Precios.Precio  =td.PrecioVenta   
			--from Precios INNER JOIN @detalle AS td
			--ON  Precios .PrecioCategoriaId  =@numicatVenta and Precios .ProductoId  =td.ProductoId  
			--and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0
			select @Id as newNumi

			-----Elimino  Caja
		   delete from CajaIngresoEgreso  where Modulo =2 and IdModulo =@Id and CajaId =1

		----Modulo 2= compras
			if (not exists(select a.* from CajaIngresoEgreso as a where a.Modulo =2 and a.IdModulo =@Id and a.CajaId =1) and @TipoVenta =1)
			begin 

	
			set @IdCajaIngresoEgreso=IIF((select COUNT(Id) from CajaIngresoEgreso)=0,0,(select MAX(Id) from CajaIngresoEgreso))+1
			insert into CajaIngresoEgreso values(@IdCajaIngresoEgreso,0,@FechaTransaccion ,'Egreso Por Compras A Proveedor '+(isnull((select top 1 pr.NombreProveedor  from Proveedor as pr where pr.Id =@Proveedor ),'')),
			@TotalCompra ,1,0,20,1,2,@Id,@AlmacenId,0,0,@Usuario,@newFecha,@newHora )
			
update Configuracion set TotalCajaGeneral=(
select Sum((c.monto*a.Operacion)) as Total
from CajaIngresoEgreso as c
inner join CajaTipoMovimiento as a on c.CajatipoMovimientoId =a.Id 
 where CajaId =1)

			end



			EXEC MAM_Auditoria @tipo=2, @Modulo=2, @Operacion=1, @DocumentoId=@Id, @Antes=@AudAntes, @usuario=@Usuario /* Kaily: auditoria: modificado */
			select @Id as newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@Usuario)
		END CATCH
	END

	IF @tipo=3 --MOSTRaR TODOS
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY
		
				
		select a.Id ,a.AlmacenId ,a.FechaTransaccion ,a.ProveedorId,p.NombreProveedor,a.TipoVenta ,a.FechaVencimientoCredito ,
		a.Moneda ,IIF(a.Moneda =1,'Boliviano','Dolar')as TituloMoneda,a.Estado ,a.Glosa ,a.TotalCompra ,a.EmpresaId  ,a.descuento,
		IIF(Exists(select t.* from TransaccionComprasCredito as t inner join CreditosCompras as c on c.Id =t.CreditoCompraId and c.CompraId =a.id),1,0) as transaccion
		from Compras as a 
		inner join Proveedor as p on p.id=a.ProveedorId 
		where a.FechaTransaccion >=@desde and a.FechaTransaccion <=@hasta 
		order by a.id desc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=4 --MOSTRaR Decalle
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY
		
		

		select d.Id ,d.CompraId ,d.ProductoId ,p.NombreProducto as Producto,isnull(d.CantidadCompra,d.Cantidad ) as CantidadCompra ,isnull(d.PorcentajeIncremento,0) as PorcentajeIncremento,isnull(d.CantidadIncremento,0) as CantidadIncremento ,d.Cantidad ,d.PrecioCosto ,
		d.Lote,d.FechaVencimiento ,d.TotalCompra ,d.PrecioVenta ,1 as estado,cast('' as image) as img,
		d.PrecioCosto as costo,d.PrecioVenta  as venta
		from CompraDetalle as d
		inner join Productos as p on p.Id =d.ProductoId 
		where d.CompraId  =@Id 
		order by d.Id asc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=5 --MOSTRaR Productos Para Comprar
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY
	
	
 Declare @CategoriaCompra int,@CategoriaVenta int
	 set @CategoriaCompra =(select Min(k.Id ) from PreciosCategorias  as k where k.Tipo  =0)
	 set @CategoriaVenta =(select Min(k.Id ) from PreciosCategorias  as k where k.Tipo  =1)

	 select a.Id ,a.CodigoExterno ,a.NombreProducto,a.NombreProducto as DescripcionProducto,industria .Descripcion as industria,cat.NombreCategoria   ,PCosto .Precio as PrecioCosto,PVenta .Precio as PrecioVenta,1 as estado,
	 isnull((select Sum(st.Cantidad ) from ProductosStock as st inner join
	 Almacenes as alma on  st.ProductoId =a.Id and st.DepositoId=alma .DepositoId and alma.id =@AlmacenId    ),0) as stock
	 from Productos as a 
	 inner join Precios as PCosto on PCosto .PrecioCategoriaId =@CategoriaCompra 
	 and PCosto.ProductoId =a.Id 
	 inner join Precios as PVenta on PVenta .PrecioCategoriaId =@CategoriaVenta 
	 and PVenta .ProductoId =a.Id 
	 and PCosto .AlmacenId =@AlmacenId 
	 and PVenta .AlmacenId =@AlmacenId 
	 inner join Categorias as cat on cat.id=a.CategoriaId 
	 inner join ClasificadorDetalle as industria on industria .id=a.AttributoId 
	 order by a.id asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=6 --MOSTRaR Proveedores
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY
	
		select a.Id ,a.NombreProveedor as Nombre ,a.Direccion ,a.Telefono01  
		from Proveedor as a
		order by a.Id asc
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=7 --Reporte de Compras
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY
	
	
select a.Id ,sucursal .NombreAlmacen ,FORMAT (a.FechaTransaccion , 'dd-MM-yyyy')  as FechaTransaccion,IIF(a.TipoVenta=1,'Contado','Credito') as TipoCompra ,
IIF(a.TipoVenta=1,'',FORMAT (a.FechaVencimientoCredito , 'dd-MM-yyyy') ) as FechaVencimientoCredito,prov .NombreProveedor 
,a.Glosa ,pro.Id as ProductoId,pro.NombreProducto ,detalle .PrecioCosto ,detalle .Cantidad ,detalle .TotalCompra 
from Compras as a inner join
CompraDetalle as detalle on detalle .CompraId =a.Id 
inner join Almacenes as sucursal on sucursal .id =a.AlmacenId 
inner join Proveedor as prov on prov .Id =a.ProveedorId 
inner join Productos as pro on pro.Id =detalle .ProductoId
where a.Id =@Id 
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=8 --MOSTRaR Personal
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY
	
		select a.Id ,a.NombrePersonal  as Nombre ,a.Direccion ,a.Telefono01  
		from Personal  as a
		order by a.Id asc
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END
	
End





GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
--drop procedure MAM_Movimientos
ALTER PROCEDURE [dbo].[MAM_Movimientos](@tipo int, @id int=-1, @ConceptoId int=-1,@TransaccionId int=-1,@DepositoId int=-1,
                                    @Observacion nvarchar(250)='',@Estado int=-1,@FechaDocumento date=null, 
									@usuario nvarchar(10)='',
									@detalle MovimientoDetalleType Readonly,@ProductoId int=-1,@FechaI date=null,@FechaF date=null,@DepositoIdDestino int=-1,
									@IdMovimientoDestino int=-1,@CategoriaPrecio int =-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()
	DECLARE @AudAntes xml /* Kaily: auditoria */
	
IF @tipo=-1 --ELIMINAR REGISTRO 
BEGIN 

    SET ARITHABORT ON 

    BEGIN TRAN ELIMINAR  
    -- ===== Kaily: validacion de stock (web y escritorio) =====================================
    -- Lo que esta operacion descuenta por producto y deposito. Las filas de stock quedan BLOQUEADAS (UPDLOCK)
    -- hasta el COMMIT: si otro usuario graba el mismo producto a la vez, espera y despues ve el stock real.
    -- Si no alcanza se revierte todo y se avisa que productos y con que stock actual.
    DECLARE @efStockMB TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))
    INSERT INTO @efStockMB (ProductoId, DepositoId, Delta)
    SELECT d.ProductoId, m.DepositoId, -d.Cantidad * c.Factor -- eliminar un ingreso descuenta stock
    FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId
    WHERE d.MovimientoId = @id OR (@ConceptoId = 8 AND d.MovimientoId IN (SELECT a.Id FROM Movimientos AS a WHERE a.IdMovimientoDestino = @id))
    DECLARE @msgStockMB nvarchar(2000)
    SELECT @msgStockMB = STUFF((
        SELECT TOP 10 '; ' + RTRIM(p.NombreProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))
            + ', necesita ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'
        FROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStockMB GROUP BY ProductoId, DepositoId) AS e
        INNER JOIN Productos AS p ON p.Id = e.ProductoId
        OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)
                     WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s
        WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0
        ORDER BY p.NombreProducto
        FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')
    IF @msgStockMB IS NOT NULL
    BEGIN
        ROLLBACK TRAN
        SET @msgStockMB = 'STOCK INSUFICIENTE: ' + @msgStockMB
        RAISERROR('%s', 16, 1, @msgStockMB)
        RETURN
    END
    -- ==========================================================================================

    BEGIN TRY  
        EXEC MAM_Auditoria @tipo=1, @Modulo=3, @DocumentoId=@id, @Foto=@AudAntes OUTPUT /* Kaily: auditoria: foto antes */

        if(@ConceptoId =8) 
        begin 

            declare @ibid2 int,@MovimientoDestino int 


            set @ibid2  =(Select a.Id  from Movimientos  as a where a.IdMovimientoDestino=@id) 


            ---------------------------------------------------
            -- LOG DETALLE DEL MOVIMIENTO DESTINO
            -- ANTES DE ELIMINAR
            ---------------------------------------------------
            INSERT INTO MovimientosDetalleLog
            (
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                'DELETE',
                GETDATE(),
                @usuario
            FROM MovimientosDetalle
            WHERE MovimientoId=@ibid2;


            ---------------------------------------------------
            -- LOG MOVIMIENTO DESTINO
            -- ANTES DE ELIMINAR
            ---------------------------------------------------
            INSERT INTO MovimientosLog
            (
                MovimientoId,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                Id,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                'DELETE',
                GETDATE(),
                @usuario
            FROM Movimientos
            WHERE Id=@ibid2;


            DELETE FROM MovimientosDetalle   
            WHERE MovimientoId=@ibid2   

            DELETE from Movimientos   
            where id=@ibid2  


            ---------------------------------------------------
            -- LOG DETALLE DEL MOVIMIENTO ORIGEN
            -- ANTES DE ELIMINAR
            ---------------------------------------------------
            INSERT INTO MovimientosDetalleLog
            (
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                'DELETE',
                GETDATE(),
                @usuario
            FROM MovimientosDetalle
            WHERE MovimientoId=@id;


            ---------------------------------------------------
            -- LOG MOVIMIENTO ORIGEN
            -- ANTES DE ELIMINAR
            ---------------------------------------------------
            INSERT INTO MovimientosLog
            (
                MovimientoId,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                Id,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                'DELETE',
                GETDATE(),
                @usuario
            FROM Movimientos
            WHERE Id=@id;


            DELETE FROM MovimientosDetalle    
            WHERE MovimientoId=@id  

            DELETE from Movimientos    
            where id=@id  


            select @id as newNumi  --Consulibr que hace newNumi 

        end 


        if (@ConceptoId <>8) 
        begin 


            ---------------------------------------------------
            -- LOG DETALLE
            -- ANTES DE ELIMINAR
            ---------------------------------------------------
            INSERT INTO MovimientosDetalleLog
            (
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                'DELETE',
                GETDATE(),
                @usuario
            FROM MovimientosDetalle
            WHERE MovimientoId=@id;


            ---------------------------------------------------
            -- LOG MOVIMIENTO
            -- ANTES DE ELIMINAR
            ---------------------------------------------------
            INSERT INTO MovimientosLog
            (
                MovimientoId,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                Id,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                'DELETE',
                GETDATE(),
                @usuario
            FROM Movimientos
            WHERE Id=@id;


            DELETE FROM MovimientosDetalle  
            WHERE MovimientoId=@id 

            DELETE FROM Movimientos  
            WHERE Id=@id 

        end 


        EXEC MAM_Auditoria @tipo=2, @Modulo=3, @Operacion=2, @DocumentoId=@id, @Antes=@AudAntes, @usuario=@usuario /* Kaily: auditoria: eliminado */
        SELECT @id AS newNumi 

        COMMIT TRAN ELIMINAR 

    END TRY 

    BEGIN CATCH 

        INSERT INTO Bitacora
        (
            banum, 
            baproc, 
            balinea, 
            bamensaje, 
            batipo, 
            bafact, 
            bahact, 
            bauact
        ) 
        VALUES
        (
            ERROR_NUMBER(), 
            ERROR_PROCEDURE(), 
            ERROR_LINE(), 
            ERROR_MESSAGE(), 
            -1, 
            @newFecha, 
            @newHora, 
            @usuario
        ) 

        ROLLBACK TRAN ELIMINAR /* Kaily: decia ELMINAR y el ROLLBACK fallaba */ 

    END CATCH 

END

IF @tipo=1 --NUEVO REGISTRO
BEGIN
    SET ARITHABORT ON

    BEGIN TRAN INSERTAR
    -- ===== Kaily: validacion de stock (web y escritorio) =====================================
    -- Lo que esta operacion descuenta por producto y deposito. Las filas de stock quedan BLOQUEADAS (UPDLOCK)
    -- hasta el COMMIT: si otro usuario graba el mismo producto a la vez, espera y despues ve el stock real.
    -- Si no alcanza se revierte todo y se avisa que productos y con que stock actual.
    DECLARE @efStockM1 TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))
    INSERT INTO @efStockM1 (ProductoId, DepositoId, Delta)
    SELECT td.ProductoId, @DepositoId, SUM(td.Cantidad) * (SELECT c.Factor FROM MovimientosTipos AS c WHERE c.Id = @ConceptoId)
    FROM @detalle AS td WHERE td.ProductoId > 0 AND td.estado >= 0 GROUP BY td.ProductoId
    DECLARE @msgStockM1 nvarchar(2000)
    SELECT @msgStockM1 = STUFF((
        SELECT TOP 10 '; ' + RTRIM(p.NombreProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))
            + ', necesita ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'
        FROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStockM1 GROUP BY ProductoId, DepositoId) AS e
        INNER JOIN Productos AS p ON p.Id = e.ProductoId
        OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)
                     WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s
        WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0
        ORDER BY p.NombreProducto
        FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')
    IF @msgStockM1 IS NOT NULL
    BEGIN
        ROLLBACK TRAN
        SET @msgStockM1 = 'STOCK INSUFICIENTE: ' + @msgStockM1
        RAISERROR('%s', 16, 1, @msgStockM1)
        RETURN
    END
    -- ==========================================================================================

    BEGIN TRY 

        set @id=IIF((select COUNT(id) from Movimientos)= 0, 0, (select MAX(id) from Movimientos))+1

        if(@ConceptoId =7) --Traspaso Ingreso
        begin

            INSERT INTO Movimientos  
            VALUES(
                @id, 
                @ConceptoId, 
                @id, 
                @DepositoId, 
                concat(
                    @Observacion,
                    ' ORIGEN: ',
                    @DepositoIdDestino,
                    ' - ',
                    (select top 1 NombreDeposito from Depositos where Id=@DepositoIdDestino)
                ),
                @Estado,
                @FechaDocumento,
                @DepositoIdDestino,
                @IdMovimientoDestino,
                @newFecha,
                @newHora,
                @usuario
            )


            ---------------------------------------------------
            -- LOG INSERT MOVIMIENTO
            ---------------------------------------------------
            INSERT INTO MovimientosLog
            (
                MovimientoId,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                Id,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                'INSERT',
                GETDATE(),
                @usuario
            FROM Movimientos
            WHERE Id=@id


            update Movimientos 
            set Movimientos.IdMovimientoDestino=@id
            ,Observacion = concat(
                Observacion,
                ' DESTINO: ',
                @DepositoId,
                ' - ',
                (select top 1 NombreDeposito from Depositos where Id=@DepositoId)
            )
            where Id=@IdMovimientoDestino 


            ---------------------------------------------------
            -- LOG UPDATE MOVIMIENTO ORIGEN
            ---------------------------------------------------
            INSERT INTO MovimientosLog
            (
                MovimientoId,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                Id,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                'UPDATE',
                GETDATE(),
                @usuario
            FROM Movimientos
            WHERE Id=@IdMovimientoDestino


            INSERT INTO MovimientosDetalle
            (
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento
            )
            SELECT 
                @id,
                td.ProductoId,
                td.Cantidad,
                td.Lote,
                td.FechaVencimiento 
            FROM @detalle AS td
            WHERE td.ProductoId >0 
            and td.estado >=0;


            ---------------------------------------------------
            -- LOG INSERT DETALLE
            ---------------------------------------------------
            INSERT INTO MovimientosDetalleLog
            (
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                'INSERT',
                GETDATE(),
                @usuario
            FROM MovimientosDetalle
            WHERE MovimientoId=@id

        end


        if(@ConceptoId =8) --Traspaso Salida
        begin

            INSERT INTO Movimientos  
            VALUES(
                @id,
                @ConceptoId,
                @id,
                @DepositoId,
                @Observacion,
                @Estado,
                @FechaDocumento,
                @DepositoIdDestino,
                @IdMovimientoDestino,
                @newFecha,
                @newHora,
                @usuario
            )


            ---------------------------------------------------
            -- LOG INSERT MOVIMIENTO
            ---------------------------------------------------
            INSERT INTO MovimientosLog
            (
                MovimientoId,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                Id,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                'INSERT',
                GETDATE(),
                @usuario
            FROM Movimientos
            WHERE Id=@id


            INSERT INTO MovimientosDetalle
            (
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento
            )
            SELECT 
                @id,
                td.ProductoId,
                td.Cantidad,
                td.Lote,
                td.FechaVencimiento 
            FROM @detalle AS td
            WHERE td.ProductoId >0 
            and td.estado >=0;


            ---------------------------------------------------
            -- LOG INSERT DETALLE
            ---------------------------------------------------
            INSERT INTO MovimientosDetalleLog
            (
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                'INSERT',
                GETDATE(),
                @usuario
            FROM MovimientosDetalle
            WHERE MovimientoId=@id

        end


        if(@ConceptoId <>7 and @ConceptoId <>8)
        begin

            INSERT INTO Movimientos  
            VALUES(
                @id,
                @ConceptoId,
                @id,
                @DepositoId,
                @Observacion,
                @Estado,
                @FechaDocumento,
                @DepositoIdDestino,
                @IdMovimientoDestino,
                @newFecha,
                @newHora,
                @usuario
            )


            ---------------------------------------------------
            -- LOG INSERT MOVIMIENTO
            ---------------------------------------------------
            INSERT INTO MovimientosLog
            (
                MovimientoId,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                Id,
                ConceptoId,
                DepositoId,
                Observacion,
                Estado,
                FechaDocumento,
                DepositoIdDestino,
                IdMovimientoDestino,
                'INSERT',
                GETDATE(),
                @usuario
            FROM Movimientos
            WHERE Id=@id


            -- INSERTO EL DETALLE
            INSERT INTO MovimientosDetalle
            (
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento
            )
            SELECT 
                @id,
                td.ProductoId,
                td.Cantidad,
                td.Lote,
                td.FechaVencimiento 
            FROM @detalle AS td
            WHERE td.ProductoId >0 
            and td.estado >=0;


            ---------------------------------------------------
            -- LOG INSERT DETALLE
            ---------------------------------------------------
            INSERT INTO MovimientosDetalleLog
            (
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                Accion,
                FechaLog,
                Usuario
            )
            SELECT
                MovimientoId,
                ProductoId,
                Cantidad,
                Lote,
                FechaVencimiento,
                'INSERT',
                GETDATE(),
                @usuario
            FROM MovimientosDetalle
            WHERE MovimientoId=@id

        end


        -- DEVUELVO VALORES DE CONFIRMACION
        SELECT @id AS newNumi

        COMMIT TRAN INSERTAR

    END TRY

    BEGIN CATCH

        INSERT INTO Bitacora
        (
            banum,
            baproc,
            balinea,
            bamensaje,
            batipo,
            bafact,
            bahact,
            bauact
        )
        VALUES
        (
            ERROR_NUMBER(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            1,
            @newFecha,
            @newHora,
            @usuario
        )

        ROLLBACK TRAN INSERTAR

    END CATCH

END
	
IF @tipo=2--MODIFICACION 
BEGIN 
    SET ARITHABORT ON 

    BEGIN TRAN MODIFICACION 
    -- ===== Kaily: validacion de stock (web y escritorio) =====================================
    -- Lo que esta operacion descuenta por producto y deposito. Las filas de stock quedan BLOQUEADAS (UPDLOCK)
    -- hasta el COMMIT: si otro usuario graba el mismo producto a la vez, espera y despues ve el stock real.
    -- Si no alcanza se revierte todo y se avisa que productos y con que stock actual.
    DECLARE @efStockM2 TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))
    INSERT INTO @efStockM2 (ProductoId, DepositoId, Delta)
    SELECT td.ProductoId, @DepositoId, td.Cantidad * (SELECT c.Factor FROM MovimientosTipos AS c WHERE c.Id = @ConceptoId)
    FROM @detalle AS td WHERE td.ProductoId > 0 AND td.estado >= 0
    UNION ALL -- lo que el movimiento tenia se revierte en su deposito
    SELECT d.ProductoId, m.DepositoId, -d.Cantidad * c.Factor
    FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId
    WHERE d.MovimientoId = @id
    DECLARE @msgStockM2 nvarchar(2000)
    SELECT @msgStockM2 = STUFF((
        SELECT TOP 10 '; ' + RTRIM(p.NombreProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))
            + ', necesita ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'
        FROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStockM2 GROUP BY ProductoId, DepositoId) AS e
        INNER JOIN Productos AS p ON p.Id = e.ProductoId
        OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)
                     WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s
        WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0
        ORDER BY p.NombreProducto
        FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')
    IF @msgStockM2 IS NOT NULL
    BEGIN
        ROLLBACK TRAN
        SET @msgStockM2 = 'STOCK INSUFICIENTE: ' + @msgStockM2
        RAISERROR('%s', 16, 1, @msgStockM2)
        RETURN
    END
    -- ==========================================================================================

    BEGIN TRY  
        EXEC MAM_Auditoria @tipo=1, @Modulo=3, @DocumentoId=@id, @Foto=@AudAntes OUTPUT /* Kaily: auditoria: foto antes */


        ---------------------------------------------------
        -- LOG DETALLE ANTES DE ELIMINAR
        ---------------------------------------------------
        INSERT INTO MovimientosDetalleLog
        (
            MovimientoId,
            ProductoId,
            Cantidad,
            Lote,
            FechaVencimiento,
            Accion,
            FechaLog,
            Usuario
        )
        SELECT
            MovimientoId,
            ProductoId,
            Cantidad,
            Lote,
            FechaVencimiento,
            'DELETE',
            GETDATE(),
            @usuario
        FROM MovimientosDetalle
        WHERE MovimientoId = @id;


        --MODIFICO EL DETALLE 
        DELETE FROM MovimientosDetalle  
        WHERE MovimientoId =@id; 


        UPDATE Movimientos  
        SET ConceptoId =@ConceptoId , 
            DepositoId =@DepositoId,
            Observacion =@Observacion , 
            Estado =@Estado , 
            FechaDocumento =@FechaDocumento ,  
            FechaRegistro =@newFecha, 
            HoraRegistro =@newHora, 
            UsuarioRegistro =@usuario 
        Where id=@id; 


        ---------------------------------------------------
        -- LOG UPDATE MOVIMIENTO
        ---------------------------------------------------
        INSERT INTO MovimientosLog
        (
            MovimientoId,
            ConceptoId,
            DepositoId,
            Observacion,
            Estado,
            FechaDocumento,
            DepositoIdDestino,
            IdMovimientoDestino,
            Accion,
            FechaLog,
            Usuario
        )
        SELECT
            Id,
            ConceptoId,
            DepositoId,
            Observacion,
            Estado,
            FechaDocumento,
            DepositoIdDestino,
            IdMovimientoDestino,
            'UPDATE',
            GETDATE(),
            @usuario
        FROM Movimientos
        WHERE Id=@id;


        INSERT INTO MovimientosDetalle 
        (
            MovimientoId,
            ProductoId,
            Cantidad,
            Lote,
            FechaVencimiento
        ) 
        SELECT 
            @id,
            td.ProductoId,
            td.Cantidad,
            td.Lote,
            td.FechaVencimiento  
        FROM @detalle AS td 
        WHERE td.ProductoId >0 
        and td.estado >=0; 


        ---------------------------------------------------
        -- LOG NUEVO DETALLE
        ---------------------------------------------------
        INSERT INTO MovimientosDetalleLog
        (
            MovimientoId,
            ProductoId,
            Cantidad,
            Lote,
            FechaVencimiento,
            Accion,
            FechaLog,
            Usuario
        )
        SELECT
            MovimientoId,
            ProductoId,
            Cantidad,
            Lote,
            FechaVencimiento,
            'INSERT',
            GETDATE(),
            @usuario
        FROM MovimientosDetalle
        WHERE MovimientoId=@id;


        --DEVUELVO VALORES DE CONFIRMACION 
        EXEC MAM_Auditoria @tipo=2, @Modulo=3, @Operacion=1, @DocumentoId=@id, @Antes=@AudAntes, @usuario=@usuario /* Kaily: auditoria: modificado */
        select @id as newNumi 

        COMMIT TRAN MODIFICACION 

    END TRY 

    BEGIN CATCH 

        INSERT INTO Bitacora 
        (
            banum, 
            baproc, 
            balinea, 
            bamensaje, 
            batipo, 
            bafact, 
            bahact, 
            bauact
        ) 
        VALUES
        (
            ERROR_NUMBER(), 
            ERROR_PROCEDURE(), 
            ERROR_LINE(), 
            ERROR_MESSAGE(), 
            2, 
            @newFecha, 
            @newHora, 
            @usuario
        ) 

        ROLLBACK TRAN MODIFICACION 

    END CATCH 

END

	IF @tipo=3 --MOSTRAR TODOS LOS MOVIMIENTOS
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
			SELECT a.Id  as id, a.FechaDocumento  as fdoc, a.ConceptoId  as concep, b.Descripcion  as nconcep, a.Observacion  as obs, 
				   a.Estado  as est, a.DepositoId  as alm ,dep.NombreDeposito 
			FROM Movimientos  a inner join MovimientosTipos  b on a.ConceptoId =b.Id  
			inner join Depositos as dep on dep.id=a.DepositoId 
			where a.ConceptoId not in (1,2,7)
			ORDER BY a.Id desc


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 3, @newFecha, @newHora, @usuario)
		END CATCH
	END

	IF @tipo=4 --MOSTRAR TODOS DETALLE DE MOVIMIENTO
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
			SELECT a.id , a.MovimientoId , a.ProductoId , b.NombreProducto  as Producto, a.Cantidad ,
			a.Lote ,a.FechaVencimiento ,CAST ('' as image ) as img, 1 as estado ,Sum(stock .Cantidad )as stock
			FROM MovimientosDetalle  a inner join Productos  b on a.ProductoId =b.Id  
			inner join ProductosStock as stock on stock .ProductoId =b.Id and a.FechaVencimiento =stock .FechaVencimiento 
			and a.Lote =stock .Lote 
			where a.MovimientoId  =@id
			group by a.id,a.MovimientoId ,a.ProductoId ,b.NombreProducto ,a.Cantidad ,a.Lote ,a.FechaVencimiento 
			ORDER BY a.id asc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 4, @newFecha, @newHora, @usuario)
		END CATCH
	END

		IF @tipo=5 --MOSTRAR TODOS DETALLE DE MOVIMIENTO
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
			select a.Id ,a.Descripcion 
			from MovimientosTipos as a where a.Estado =1 and a.Id not in (1,2,5,6,7)
			order by a.Id asc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 4, @newFecha, @newHora, @usuario)
		END CATCH
	END

		IF @tipo=6  --Leer configuracion
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
			
			select *
			from Configuracion 
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 4, @newFecha, @newHora, @usuario)
		END CATCH
	END

		IF @tipo=7--MOSTRaR Productos Para el movimiento
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
	

		 select p.Id ,p.CodigoExterno ,p.NombreProducto,
		 p.NombreProducto as DescripcionProducto,industria.Descripcion as industria,cat.NombreCategoria  ,Sum(stock .Cantidad ) as stock , 1 as estado
		 from Productos as p inner join 
		 ProductosStock as stock on stock .ProductoId =p.Id 
		 inner join Categorias as cat on cat.Id =p.CategoriaId 
		 inner join ClasificadorDetalle as industria on industria .id=p.AttributoId 
		 where stock .DepositoId =@DepositoId 
		 group by p.Id ,p.CodigoExterno ,p.NombreProducto ,p.DescripcionProducto,cat.NombreCategoria,industria .Descripcion 
		 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

	IF @tipo=8--MOSTRaR Lotes de producto con stock
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
		
select p.NombreProducto ,a.Lote ,a.FechaVencimiento ,Sum(a.Cantidad ) as stock
from ProductosStock as a
inner join Productos as p on p.Id =a.ProductoId 
and a.Cantidad >0 and a.DepositoId =@DepositoId 
and p.Id=@ProductoId
group by p.NombreProducto ,a.Lote ,a.FechaVencimiento 
order by a.FechaVencimiento asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

IF @tipo=9--Saldo Todos Mayor a Cero
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
		
select stock .DepositoId ,dep .NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto,1 as orden ,cla.DescripcionCategoria as Descripcion  ,Sum(stock .Cantidad) as stock 
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join Depositos as dep on dep.id =stock .DepositoId 
group by stock.DepositoId ,dep.NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto ,cla.DescripcionCategoria  
having Sum(stock .Cantidad)>0


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END
IF @tipo=10--Saldo Todos
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
		
select stock .DepositoId ,dep .NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto,1 as orden ,cla.DescripcionCategoria as   Descripcion  ,Sum(stock .Cantidad) as stock 
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join Depositos as dep on dep.id =stock .DepositoId 
group by stock.DepositoId ,dep.NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto  ,cla.DescripcionCategoria  



		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END
IF @tipo=11--Saldo Todos
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
		
select stock .DepositoId ,dep .NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto,1 as orden ,cla.DescripcionCategoria  as Descripcion  ,Sum(stock .Cantidad) as stock 
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join Depositos as dep on dep.id =stock .DepositoId 
where dep.id=@DepositoId 
group by stock.DepositoId ,dep.NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto ,cla.DescripcionCategoria  



		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

IF @tipo=12--Saldo Todos Mayor a Cero
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
		
select stock .DepositoId ,dep .NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto,1 as orden ,cla.DescripcionCategoria  as Descripcion  ,Sum(stock .Cantidad) as stock 
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join Depositos as dep on dep.id =stock .DepositoId 
where dep.id =@DepositoId 
group by stock.DepositoId ,dep.NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto ,cla.DescripcionCategoria   
having Sum(stock .Cantidad)>0


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END


	IF @tipo=13--MOSTRaR Productos Para el movimiento
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
	

		 select p.Id ,p.CodigoExterno ,p.NombreProducto as Nombre,p.DescripcionProducto ,Sum(stock .Cantidad ) as stock , 1 as estado
		 from Productos as p inner join 
		 ProductosStock as stock on stock .ProductoId =p.Id 
		 where stock .DepositoId =@DepositoId 
		 group by p.Id ,p.CodigoExterno ,p.NombreProducto ,p.DescripcionProducto 
		 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

	IF @tipo=14--Obtener Movimientos de Productos
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
	SELECT        a.Id  AS id, a.FechaDocumento  AS fdoc, IIF(c.factor = 1, 1, 2) AS concep,Isnull(c.Descripcion  ,salida.Descripcion )  AS descConcep,
						  IIF(c.id =3 or c.id=4, a.Observacion,IIF(c.id=1 or salida.id=2,
						  substring(a.Observacion, charindex('|',a.Observacion) + 1, LEN(a.Observacion)),a.Observacion))
						   AS obs, a.Estado  AS est, a.DepositoId  AS alm, b.Id  AS id2, b.ProductoId  AS cprod, 
						  d.NombreProducto   AS descProd, 
                         Isnull(Cast((b.Cantidad  *c.Factor  )AS decimal(18, 2)),0) AS entrada,
						 Isnull(Cast((b.Cantidad  *salida.Factor  )AS decimal(18, 2)),0) AS salida
						 , 0.0 AS saldo, '' AS nombreCliente, FORMAT(a.FechaDocumento , 'yyyy-MM-dd') AS fechaR
FROM            Movimientos  AS a INNER JOIN
                         MovimientosDetalle  AS b ON a.Id  = b.MovimientoId  Left JOIN
                         MovimientosTipos  AS c ON c.Id  = a.ConceptoId   and c.Factor =1
						 INNER JOIN Productos   AS d ON d .Id   = b.ProductoId  and d.Id  =@ProductoId    ----Codigo Producto
						 and a.FechaDocumento  <@FechaDocumento  
						  and a.DepositoId =@DepositoId  
						  Left join MovimientosTipos  as salida on salida.Id  = a.ConceptoId  and salida .Factor  =-1
						  order by a.id asc
		 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END


	IF @tipo=15
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
		
SELECT        a.Id  , FORMAT(a.FechaDocumento , 'dd/MM/yyyy')   AS fdoc, IIF(c.factor = 1, 1, 2) AS concep,Isnull(c.Descripcion ,salida.Descripcion )  AS descConcep,
						  IIF(c.id =4 or salida.id=3, a.Observacion,
						  IIF(a.conceptoId=1 ,(select concat('Venta # ',ve.Id,' Al Cliente : ',clie.NombreCliente)
						       from Ventas as ve
							   inner join Clientes as clie on clie.id=ve.ClienteId and ve.id=a.transaccionId and a.ConceptoId=1),(select concat('Compra # ',ve.Id,'  Al Proveedor : ',clie.NombreProveedor)
						       from Compras as ve
							   inner join Proveedor as clie on clie.id=ve.ProveedorId and ve.Id=a.transaccionId and a.ConceptoId=2)))
						   AS obs
						   ,b.Lote  as Lote,b.FechaVencimiento  as FechaVenc,
						   a.Estado  AS est, a.DepositoId  AS alm, b.Id  AS id2, b.ProductoId  AS cprod, 
						  d.NombreProducto   AS descProd,  
                         isnull(Cast((b.Cantidad  *c.Factor  )AS decimal(18, 2)),0) AS entrada,
						 isnull(Cast((b.Cantidad *salida.Factor  )AS decimal(18, 2)),0) AS salida
						 , 0.0 AS saldo, FORMAT(a.FechaDocumento , 'yyyy-MM-dd') AS fechaDocumento
FROM            Movimientos  AS a INNER JOIN
                         MovimientosDetalle  AS b ON a.Id  = b.MovimientoId  Left JOIN
                         MovimientosTipos  AS c ON c.Id  = a.ConceptoId  and c.Factor  =1 INNER JOIN
                         Productos   AS d ON d .Id   = b.ProductoId and d.Id  =@ProductoId    ----Codigo Producto
						  and a.FechaDocumento  >=@FechaI   and a.FechaDocumento  <=@fechaF 
						 and a.DepositoId =@DepositoId  
						  Left join MovimientosTipos  as salida on salida.Id  = a.ConceptoId  and salida .Factor  =-1

	order by a.id asc 



		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 3, @newFecha, @newHora, @usuario)
		END CATCH
	END

	
	IF @tipo=16--Listar Productos que han tenido movimientos
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
	
select p.Id  ,p.NombreProducto ,presentacion .Descripcion as Presentacion,cast(0 as decimal(18,2)) as SaldoAnterior,
cast(0 as decimal(18,2)) as Entradas,cast(0 as decimal(18,2)) as Salidas,cast(0 as decimal(18,2)) as SaldoFinal
	from Productos  as p inner join ClasificadorDetalle as presentacion
	on presentacion .Id =p.MarcaId  and
	  p.Id  in(select b.ProductoId 
	from
	Movimientos  as a  
	inner join MovimientosDetalle  as b 
	on b.MovimientoId  =a.Id  and  a.FechaDocumento  >= @fechaI   AND a.FechaDocumento  <= @fechaF  
 AND a.DepositoId  = @DepositoId  )
	  order by p.id asc
		 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END
	IF @tipo=17--Reporte Movimiento
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
	
SELECT a.Id, format(a.FechaDocumento,'dd/MM/yyyy')as FechaDocumento, tipo.Descripcion AS Movimiento, a.Observacion, dep.NombreDeposito, detalle.ProductoId, p.NombreProducto, cat.NombreCategoria, detalle.Cantidad, CAST('' AS image) AS img
FROM     dbo.Movimientos AS a INNER JOIN
                  dbo.MovimientosDetalle AS detalle ON a.Id = detalle.MovimientoId INNER JOIN
                  dbo.MovimientosTipos AS tipo ON tipo.Id = a.ConceptoId INNER JOIN
                  dbo.Depositos AS dep ON dep.id = a.DepositoId INNER JOIN
                  dbo.Productos AS p ON p.Id = detalle.ProductoId INNER JOIN
                  dbo.Categorias AS cat ON cat.Id = p.CategoriaId
WHERE  (a.Id = @Id)
		 order by p.NombreProducto 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

	IF @tipo=18--Consulta Listado 
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
	
select p.Id ,p.CodigoExterno ,p.NombreProducto ,p.NombreProducto as DescripcionProducto ,industria .Descripcion as industria,cla.NombreCategoria  as Categoria,
(select aa.precio  from Precios as aa where aa.PrecioCategoriaId=@CategoriaPrecio and aa.almacenId=1 and aa.Productoid=p.Id) as precio,unidad .Descripcion as unidad,p.StockMinimo  ,
		  (SELECT ''+ R.NombreDeposito  +' = '+cast(Sum(inv.Cantidad) as nvarchar(10))+'   ' +char(10) FROM Depositos  as R inner join ProductosStock  as inv on  R.id   =inv.DepositoId  and inv.ProductoId  =p.id group by r.NombreDeposito     FOR XML PATH('')) as stock,Sum(stock .Cantidad) as stockGeneral
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join ClasificadorDetalle as industria on industria .Id =p.FamiliaId 
inner join ClasificadorDetalle as unidad on unidad.Id=p.UnidadVentaId 
where p.Estado =1
group by p.Id ,p.NombreProducto ,p.DescripcionProducto ,cla.NombreCategoria ,p.StockMinimo,industria .Descripcion,unidad.Descripcion,p.CodigoExterno 

order by p.Id   asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END
END





GO
IF @@TRANCOUNT > 0
BEGIN
	IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas')) LIKE '%Kaily: auditoria%' AND OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) LIKE '%Kaily: auditoria%' AND OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Movimientos')) LIKE '%Kaily: auditoria%'
	BEGIN COMMIT TRAN; RAISERROR('08 Auditoria en SPs INSTALADO.', 10, 1) WITH NOWAIT END
	ELSE
	BEGIN ROLLBACK TRAN; RAISERROR('08: fallo la instalacion. Se deshizo todo, no se modifico nada.', 16, 1) END
END
ELSE IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas')) NOT LIKE '%Kaily: auditoria%' OR OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) NOT LIKE '%Kaily: auditoria%' OR OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Movimientos')) NOT LIKE '%Kaily: auditoria%'
	RAISERROR('08: fallo la instalacion (revisar los mensajes de arriba).', 16, 1)
GO
SET NOEXEC OFF
GO
