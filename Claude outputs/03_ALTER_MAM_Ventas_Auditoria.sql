/****** Object:  StoredProcedure [dbo].[MAM_Ventas]    Script Date: 16/09/2026 9:45:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER  PROCEDURE [dbo].[MAM_Ventas](@tipo int,@Id int=-1,@SucursalId int=-1,@FechaVenta date=null,
@PersonalId int =-1,@TipoVenta int=-1,@FechaVencimientoCredito date=null,
@ClienteId int=-1,@MonedaVenta int=-1,@Estado int=-1,
@Glosa nvarchar(450)='',@Descuento decimal(18,2)=0,@TotalVenta decimal(18,2)=0
,@VentaDetalleType VentaDetalle02Type Readonly,@usuario nvarchar(50)='',@TipoCambio decimal(18,2)=0,@VentaPagos VentaMonedatype Readonly,
@Desde date=null,@Hasta date=null,@Descripcion nvarchar(250)='',@TipoMovimiento int=-1,@Facturado int=-1,@CategoriaVentaId int=-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()
	
	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN Tr_EliminarVenta
		BEGIN TRY

		--==== AUDITORIA: snapshot ANTES de eliminar (baja logica cabecera + DELETE fisico detalle) ====
		DECLARE @AuditoriaId_Ventas_Del INT

		INSERT INTO VentasAuditoria (Id,SucursalId,FechaVenta,PersonalId,TipoVenta,FechaVencimientoCredito,ClienteId,MonedaVenta,Estado,Glosa,Descuento,TotalVenta,FechaRegistro,HoraRegistro,UsuarioRegistro,EstadoPedido,RepartidorId,FechaEntrega,IdSincronizacion,Sistema,Latitud,Longitud,Facturado,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
		SELECT Id,SucursalId,FechaVenta,PersonalId,TipoVenta,FechaVencimientoCredito,ClienteId,MonedaVenta,Estado,Glosa,Descuento,TotalVenta,FechaRegistro,HoraRegistro,UsuarioRegistro,EstadoPedido,RepartidorId,FechaEntrega,IdSincronizacion,Sistema,Latitud,Longitud,Facturado,'ELIMINAR','ANTES',@usuario,@newFecha,@newHora
		FROM Ventas WHERE Id=@Id

		SET @AuditoriaId_Ventas_Del = SCOPE_IDENTITY()

		INSERT INTO VentasDetalleAuditoria (AuditoriaId,VentaId,Id,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,Tipo,KitId,CantidadKit,FechaRegistro,HoraRegistro,UsuarioRegistro)
		SELECT @AuditoriaId_Ventas_Del,VentaId,Id,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,Tipo,KitId,CantidadKit,FechaRegistro,HoraRegistro,UsuarioRegistro
		FROM VentasDetalles WHERE VentaId=@Id
		--==== FIN AUDITORIA ====

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
			SELECT 1 as resp;
			COMMIT TRAN Tr_EliminarVenta
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN Tr_EliminarVenta
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN INSERTAR
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

		--==== AUDITORIA: snapshot DESPUES de crear ====
		DECLARE @AuditoriaId_Ventas_Nueva INT

		INSERT INTO VentasAuditoria (Id,SucursalId,FechaVenta,PersonalId,TipoVenta,FechaVencimientoCredito,ClienteId,MonedaVenta,Estado,Glosa,Descuento,TotalVenta,FechaRegistro,HoraRegistro,UsuarioRegistro,EstadoPedido,RepartidorId,FechaEntrega,IdSincronizacion,Sistema,Latitud,Longitud,Facturado,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
		SELECT Id,SucursalId,FechaVenta,PersonalId,TipoVenta,FechaVencimientoCredito,ClienteId,MonedaVenta,Estado,Glosa,Descuento,TotalVenta,FechaRegistro,HoraRegistro,UsuarioRegistro,EstadoPedido,RepartidorId,FechaEntrega,IdSincronizacion,Sistema,Latitud,Longitud,Facturado,'CREAR','DESPUES',@Usuario,@newFecha,@newHora
		FROM Ventas WHERE Id=@Id

		SET @AuditoriaId_Ventas_Nueva = SCOPE_IDENTITY()

		INSERT INTO VentasDetalleAuditoria (AuditoriaId,VentaId,Id,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,Tipo,KitId,CantidadKit,FechaRegistro,HoraRegistro,UsuarioRegistro)
		SELECT @AuditoriaId_Ventas_Nueva,VentaId,Id,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,Tipo,KitId,CantidadKit,FechaRegistro,HoraRegistro,UsuarioRegistro
		FROM VentasDetalles WHERE VentaId=@Id
		--==== FIN AUDITORIA ====
			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT @Id AS newNumi
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario,@Id )
		END CATCH
	END
	IF @tipo=2--MODIFICACION
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN Tr_ModificarVenta
		BEGIN TRY 

		--==== AUDITORIA: snapshot ANTES de modificar ====
		DECLARE @AuditoriaId_Ventas_ModAntes INT

		INSERT INTO VentasAuditoria (Id,SucursalId,FechaVenta,PersonalId,TipoVenta,FechaVencimientoCredito,ClienteId,MonedaVenta,Estado,Glosa,Descuento,TotalVenta,FechaRegistro,HoraRegistro,UsuarioRegistro,EstadoPedido,RepartidorId,FechaEntrega,IdSincronizacion,Sistema,Latitud,Longitud,Facturado,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
		SELECT Id,SucursalId,FechaVenta,PersonalId,TipoVenta,FechaVencimientoCredito,ClienteId,MonedaVenta,Estado,Glosa,Descuento,TotalVenta,FechaRegistro,HoraRegistro,UsuarioRegistro,EstadoPedido,RepartidorId,FechaEntrega,IdSincronizacion,Sistema,Latitud,Longitud,Facturado,'MODIFICAR','ANTES',@Usuario,@newFecha,@newHora
		FROM Ventas WHERE Id=@Id

		SET @AuditoriaId_Ventas_ModAntes = SCOPE_IDENTITY()

		INSERT INTO VentasDetalleAuditoria (AuditoriaId,VentaId,Id,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,Tipo,KitId,CantidadKit,FechaRegistro,HoraRegistro,UsuarioRegistro)
		SELECT @AuditoriaId_Ventas_ModAntes,VentaId,Id,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,Tipo,KitId,CantidadKit,FechaRegistro,HoraRegistro,UsuarioRegistro
		FROM VentasDetalles WHERE VentaId=@Id
		--==== FIN AUDITORIA ANTES ====

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

		--==== AUDITORIA: snapshot DESPUES de modificar ====
		DECLARE @AuditoriaId_Ventas_ModDespues INT

		INSERT INTO VentasAuditoria (Id,SucursalId,FechaVenta,PersonalId,TipoVenta,FechaVencimientoCredito,ClienteId,MonedaVenta,Estado,Glosa,Descuento,TotalVenta,FechaRegistro,HoraRegistro,UsuarioRegistro,EstadoPedido,RepartidorId,FechaEntrega,IdSincronizacion,Sistema,Latitud,Longitud,Facturado,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
		SELECT Id,SucursalId,FechaVenta,PersonalId,TipoVenta,FechaVencimientoCredito,ClienteId,MonedaVenta,Estado,Glosa,Descuento,TotalVenta,FechaRegistro,HoraRegistro,UsuarioRegistro,EstadoPedido,RepartidorId,FechaEntrega,IdSincronizacion,Sistema,Latitud,Longitud,Facturado,'MODIFICAR','DESPUES',@Usuario,@newFecha,@newHora
		FROM Ventas WHERE Id=@Id

		SET @AuditoriaId_Ventas_ModDespues = SCOPE_IDENTITY()

		INSERT INTO VentasDetalleAuditoria (AuditoriaId,VentaId,Id,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,Tipo,KitId,CantidadKit,FechaRegistro,HoraRegistro,UsuarioRegistro)
		SELECT @AuditoriaId_Ventas_ModDespues,VentaId,Id,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,Tipo,KitId,CantidadKit,FechaRegistro,HoraRegistro,UsuarioRegistro
		FROM VentasDetalles WHERE VentaId=@Id
		--==== FIN AUDITORIA DESPUES ====

		select @Id as newNumi
		COMMIT TRAN Tr_ModificarVenta
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN Tr_ModificarVenta
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@Usuario,@Id)
		END CATCH
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
		 isnull((select top 1 cierre .CierreCajeroId   from CierreCajeroReferenciasModulos as cierre where cierre.Modulo =1 and cierre.ModuloId =a.Id ),0)as CierreModulo,a.Facturado,a.HoraRegistro
		from Ventas as a 
		inner join Personal as p on p.Id =a.PersonalId 
		inner join Clientes as c on c.Id =a.ClienteId 
		left join VentasMonedaCobrada as pago on pago.VentaId =a.Id 
		inner join Almacenes as al on al.id =a.SucursalId 
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
		 	 isnull((select top 1 cierre .CierreCajeroId  from CierreCajeroReferenciasModulos as cierre where cierre.Modulo =1 and cierre.ModuloId =a.Id ),0)as CierreModulo,a.Facturado,a.HoraRegistro 
		from Ventas as a 
		inner join Personal as p on p.Id =a.PersonalId 
		inner join Clientes as c on c.Id =a.ClienteId 
		left join VentasMonedaCobrada as pago on pago.VentaId =a.Id 
		inner join Almacenes as al on al.id =a.SucursalId 
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
		
	--Se agregan UnidadVentaId/UnidadVenta/UnidadMaximaId/UnidadMaxima/Conversion/CantidadCaja
	--(mismo patron que MAM_Movimientos/MAM_Compras @tipo=4): solo lectura para el VB, no se
	--graban en VentasDetalles ni forman parte de VentaDetalle02Type. En filas Kit (Tipo=2)
	--estas 6 columnas salen en NULL porque un kit no tiene Unidad Venta/Unidad Maxima propia.
	select a.Id ,a.VentaId ,a.ProductoId ,p.NombreProducto as Producto,a.Cantidad ,a.Precio ,a.SubTotal ,
		a.ProcentajeDescuento ,a.MontoDescuento ,a.Total ,a.Detalle ,a.PrecioCosto ,a.Lote ,a.FechaVencimiento,isnull(a.Tipo,1)as Tipo,
		IIF(a.Tipo=2,'Kits','Productos')as TipoNombre,isnull(a.KitId,0) as KitId,isnull((select k.NombreKit  from Kits as k where k.id=a.KitID),'')as KitNombre,
		isnull(a.CantidadKit,0) as CantidadKit ,1 as estado,cast ('' as image ) as img
		,(select sum(st.Cantidad ) from ProductosStock as st
		 where st.ProductoId =p.Id and st.Lote =a.Lote and  st.FechaVencimiento =a.FechaVencimiento )+a.Cantidad  as stock
		,CASE WHEN isnull(a.Tipo,1)=1 THEN p.UnidadVentaId ELSE NULL END as UnidadVentaId
		,CASE WHEN isnull(a.Tipo,1)=1 THEN unidadMin.Descripcion ELSE NULL END as UnidadVenta
		,CASE WHEN isnull(a.Tipo,1)=1 THEN p.UnidadMaximaId ELSE NULL END as UnidadMaximaId
		,CASE WHEN isnull(a.Tipo,1)=1 THEN unidadMax.Descripcion ELSE NULL END as UnidadMaxima
		,CASE WHEN isnull(a.Tipo,1)=1 THEN ISNULL(NULLIF(p.Conversion,0),1) ELSE NULL END as Conversion
		,CASE WHEN isnull(a.Tipo,1)=1 THEN a.Cantidad / ISNULL(NULLIF(p.Conversion,0),1) ELSE NULL END as CantidadCaja
		from VentasDetalles as a
		inner join Productos as p on p.Id =a.ProductoId
		left join ClasificadorDetalle as unidadMin on unidadMin.id=p.UnidadVentaId
		left join ClasificadorDetalle as unidadMax on unidadMax.id=p.UnidadMaximaId
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
	--Se agregan UnidadVentaId/UnidadVenta/UnidadMaximaId/UnidadMaxima/Conversion (mismo patron
	--que MAM_Compras @tipo=5): solo lectura para el VB. En la rama de Kits salen en NULL, porque
	--un kit no tiene Unidad Venta/Unidad Maxima propia en la tabla Productos. Las columnas
	--nuevas de la rama Productos se agregan tambien al GROUP BY (evita Msg 8120).
  select a.Id ,a.CodigoExterno ,a.NombreProducto,a.NombreProducto as DescripcionProducto,industria .Descripcion as industria,cat.NombreCategoria   ,PCosto .Precio as PrecioCosto,PVenta .Precio as PrecioVenta,1 as estado,
	sum(st.Cantidad ) as stock
	 ,1 as Tipo,'Producto' as NombreTipo
	 ,a.UnidadVentaId
	 ,unidadMin.Descripcion as UnidadVenta
	 ,a.UnidadMaximaId
	 ,unidadMax.Descripcion as UnidadMaxima
	 ,ISNULL(NULLIF(a.Conversion,0),1) as Conversion
	 from Productos as a
	 inner join ProductosStock as st on st.ProductoId =a.Id and st.DepositoId =@SucursalId
	 inner join Precios as PCosto on PCosto .AlmacenId =@SucursalId  and PCosto .PrecioCategoriaId =@CategoriaCompra
	 and PCosto.ProductoId =a.Id
	 inner join Precios as PVenta on PVenta .AlmacenId =@SucursalId  and PVenta .PrecioCategoriaId =@CategoriaVentaId
	 and PVenta .ProductoId =a.Id
	 inner join Categorias as cat on cat.id=a.CategoriaId
	 inner join ClasificadorDetalle as industria on industria .Id =a.AttributoId
	 left join ClasificadorDetalle as unidadMin on unidadMin.id=a.UnidadVentaId
	 left join ClasificadorDetalle as unidadMax on unidadMax.id=a.UnidadMaximaId
	 where a.estado=1
	 group by  a.Id ,a.CodigoExterno ,a.NombreProducto,a.DescripcionProducto,industria .Descripcion,cat.NombreCategoria   ,PCosto .Precio ,PVenta .Precio
	 ,a.UnidadVentaId,unidadMin.Descripcion,a.UnidadMaximaId,unidadMax.Descripcion,a.Conversion

	 	 union

	 select a.id as Id,'' as CodigoExterno,a.NombreKit as NombreProducto,a.DescripcionKit as DescripcionProducto,'' as industria,
	 'KIT' as Categoria,0 as PrecioCosto,a.Total as PrecioVenta,1 as estado,dbo.StockPorKits(a.id,@SucursalId )as stock,
	 2 as Tipo,'Kits' as NombreTipo
	 ,CAST(NULL as int) as UnidadVentaId
	 ,CAST(NULL as nvarchar(200)) as UnidadVenta
	 ,CAST(NULL as int) as UnidadMaximaId
	 ,CAST(NULL as nvarchar(200)) as UnidadMaxima
	 ,CAST(NULL as decimal(18,4)) as Conversion
     from Kits as a


	 order by a.NombreProducto  asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END
	--Restaurado: este bloque (Recibo de Venta / Nota de Venta, usado por
	--ListarVentaRecibo/P_GenerarReporte en Tec_Ventas.vb) no estaba en el texto que me pasaste
	--desde SSMS -- se habia eliminado en algun cambio anterior, ajeno a este trabajo de Unidad
	--Caja. Lo reconstruyo tal cual estaba en el dump de esquema mas viejo que yo tenia, sin
	--ningun cambio de columnas: no necesita CantidadCaja/Conversion/UnidadMaxima porque solo
	--lee directo de VentasDetalles (Cantidad, Precio, SubTotal, MontoDescuento, Total,
	--CantidadKit), que no cambiaron.
	IF @tipo=6 --Recibo de Venta
	BEGIN
	SET ARITHABORT ON
		BEGIN TRY

		SELECT '00000' + CAST(a.Id AS nvarchar(20)) AS nroRecibo,FORMAT (a.FechaVenta , 'dd-MM-yyyy') as FechaVenta, cli.NombreCliente, IIF(cli.DireccionCliente='','S/D', cli.DireccionCliente)as DireccionCliente
		, IIF(cli.NroDocumento='','0',cli.NroDocumento) as NroDocumento, IIF(cli.Telefono='','0',cli.Telefono)as Telefono, p.Id AS ProductoId, p.NombreProducto, cast(Sum(detalle.Cantidad) as int) as Cantidad
		, detalle.Precio, Sum(detalle.SubTotal) as SubTotal,
                  Sum(detalle.MontoDescuento) as MontoDescuento, Sum(detalle.Total) as Total, empresa.Nombre AS Empresa, empresa.Direccion AS DireccionEmpresa, empresa.Ciudad, CAST('' AS varbinary) AS imageEmpresa,
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

group by a.Id ,a.FechaVenta ,cli .NombreCliente ,cli.DireccionCliente,cli.NroDocumento ,cli.Telefono ,p.Id ,p.NombreProducto,
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
END

GO
