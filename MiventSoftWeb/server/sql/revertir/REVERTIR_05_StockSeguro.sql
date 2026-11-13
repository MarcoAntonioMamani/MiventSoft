/* =====================================================================
   SOLO PARA DESHACER 05_StockSeguro.sql. NO es parte de la instalacion: NO ejecutar al instalar.
   Vuelve MAM_Ventas, MAM_Movimientos y los 3 triggers de MovimientosDetalle a la version anterior
   (sin validacion de stock). Si StockSeguro no esta instalado, avisa y no hace nada.
   ===================================================================== */
SET NOEXEC OFF
GO
IF NOT EXISTS (SELECT 1 FROM (VALUES ('MAM_Ventas'), ('MAM_Movimientos'), ('Trigger_MovimientoInsertar'), ('Trigger_MovimientoDetalleEliminar'), ('Trigger_MovimientosDetalleModificar')) AS v(n) WHERE (OBJECT_DEFINITION(OBJECT_ID('dbo.' + v.n)) LIKE '%Kaily: validacion de stock%' OR OBJECT_DEFINITION(OBJECT_ID('dbo.' + v.n)) LIKE '%Kaily: saldo en una sola sentencia%'))
BEGIN
	RAISERROR('StockSeguro NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
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
	
	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
	SET ARITHABORT ON
		BEGIN TRY

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
		BEGIN TRY 

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
			select @Id as newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@Usuario)
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
	
IF @tipo=-1 --ELIMINAR REGISTRO 
BEGIN 

    SET ARITHABORT ON 

    BEGIN TRAN ELIMINAR  

    BEGIN TRY  

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

        ROLLBACK TRAN ELMINAR 

    END CATCH 

END

IF @tipo=1 --NUEVO REGISTRO
BEGIN
    SET ARITHABORT ON

    BEGIN TRAN INSERTAR

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

    BEGIN TRY  


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
ALTER TRIGGER Trigger_MovimientoInsertar ON MovimientosDetalle
AFTER INSERT
AS
BEGIN

	declare @id int, @ibid int, @cprod int, @cantI decimal(18,2),  

			@cantA decimal(18,2), @cantF decimal(18,2), @mov int, @alm int,@lote nvarchar(50)='',@fechaVencimiento date

	declare cSaldo Cursor
		for select a.Id , a.MovimientoId , a.ProductoId , a.Cantidad , b.DepositoId , c.Factor,a.Lote ,a.FechaVencimiento 
			from inserted a inner join Movimientos  b on a.MovimientoId =b.Id 
				 inner join MovimientosTipos  c on b.ConceptoId =c.Id 

	open cSaldo

	fetch cSaldo into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	while (@@FETCH_STATUS = 0)
	begin

		if(exists(select * from ProductosStock  a where a.ProductoId =@cprod and a.DepositoId =@alm
		 and a.Lote =@lote and a.FechaVencimiento =@fechaVencimiento ))
		begin
			begin try
				begin tran tr_UpdateProductosStock
					--Obtener la cantidad actual
					set @cantA=(select Cantidad  from ProductosStock where ProductoId  = @cprod and DepositoId =@alm
					 and Lote =@lote and FechaVencimiento =@fechaVencimiento )
					set @cantF=@cantA + (@cantI*@mov)
					--Actualizar Saldo
					update ProductosStock set Cantidad =@cantF
								 where ProductoId =@cprod and DepositoId =@alm and Lote =@lote and FechaVencimiento =@fechaVencimiento 						
				commit tran tr_UpdateProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end try
			begin catch
				rollback tran tr_UpdateProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end catch
		end
		else
		begin
			begin try
				begin tran tr_InsertProductosStock
					--Insertar Saldo
					Insert into ProductosStock values(@alm,@cprod, (@cantI*@mov),@lote ,@fechaVencimiento )						
				commit tran tr_InsertProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov))
			end try
			begin catch
				rollback tran tr_InsertProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov))
			end catch
		end
		fetch cSaldo into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	end

	close cSaldo
	deallocate cSaldo
END



GO
ALTER TRIGGER [dbo].[Trigger_MovimientoDetalleEliminar] ON [dbo].[MovimientosDetalle]
AFTER DELETE
AS
BEGIN

	declare @id int, @ibid int, @cprod int, @cantI decimal(18,2),  

			@cantA decimal(18,2), @cantF decimal(18,2), @mov int, @alm int,@lote nvarchar(50),@fechaVencimiento date

	declare cSaldo Cursor
		for select a.Id , a.MovimientoId , a.ProductoId , a.Cantidad, b.DepositoId , c.Factor,a.Lote ,a.FechaVencimiento 
			from deleted a inner join Movimientos  b on a.MovimientoId =b.Id 
				 inner join MovimientosTipos  c on b.ConceptoId =c.Id 

	open cSaldo

	fetch cSaldo into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	while (@@FETCH_STATUS = 0)
	begin

		if(exists(select * from ProductosStock  a where a.ProductoId =@cprod and a.DepositoId =@alm 
		and a.Lote =@lote and a.FechaVencimiento =@fechaVencimiento ))
		begin
			begin try
				begin tran tr_UpdateTI001
					--Obtener la cantidad actual
					
					set @cantI=(@cantI*@mov*-1)
					set @cantA=(select Cantidad  from ProductosStock  where ProductoId =@cprod and DepositoId =@alm and 
					Lote =@lote and FechaVencimiento =@fechaVencimiento )
					set @cantF=@cantI+@cantA
					--Actualizar Saldo
					update ProductosStock  set Cantidad=@cantF
								 where ProductoId =@cprod and DepositoId =@alm and Lote =@lote and FechaVencimiento =@fechaVencimiento 						
				commit tran tr_UpdateTI001
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end try
			begin catch
				rollback tran tr_UpdateTI001
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end catch
		end
		else
		begin
			begin try
				begin tran tr_InsertTI001
					--Insertar Saldo
					Insert into ProductosStock  values( @alm,@cprod, (@cantI*@mov*-1),@lote ,@fechaVencimiento )						
				commit tran tr_InsertTI001
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov*-1))
			end try
			begin catch
				rollback tran tr_InsertTI001
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov*-1))
			end catch
		end
		fetch cSaldo into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	end

	close cSaldo
	deallocate cSaldo
END



GO
ALTER TRIGGER Trigger_MovimientosDetalleModificar ON MovimientosDetalle
AFTER UPDATE
AS
BEGIN
	--1ro trabajamos sobre las filas eliminadas (E)
	declare @id int, @ibid int, @cprod int, @cantI decimal(18,2),  

			@cantA decimal(18,2), @cantF decimal(18,2), @mov int, @alm int,@lote nvarchar(50),@fechaVencimiento date
	
	declare cSaldoE Cursor
		for select a.Id , a.MovimientoId , a.ProductoId , a.Cantidad , b.DepositoId , c.Factor ,a.Lote ,a.FechaVencimiento 
			from deleted a inner join Movimientos  b on a.MovimientoId =b.Id 
				 inner join MovimientosTipos  c on b.ConceptoId =c.Id 

	open cSaldoE

	fetch cSaldoE into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	while (@@FETCH_STATUS = 0)
	begin

		if(exists(select * from ProductosStock a where a.ProductoId =@cprod and a.DepositoId =@alm and a.Lote =@lote 
		and FechaVencimiento =@fechaVencimiento ))
		begin
			begin try
				begin tran tr_UpdateProductosStock
					--Obtener la cantidad actual
					
					set @cantI=(@cantI*@mov*-1)
					set @cantA=(select Cantidad  from ProductosStock where ProductoId =@cprod and DepositoId =@alm and 
					Lote =@lote and FechaVencimiento =@fechaVencimiento )
					set @cantF=@cantI+@cantA
					--Actualizar Saldo
					update ProductosStock set Cantidad =@cantF
								 where ProductoId =@cprod and DepositoId =@alm and Lote =@lote and FechaVencimiento =@fechaVencimiento 						
				commit tran tr_UpdateProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end try
			begin catch
				rollback tran tr_UpdateProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end catch
		end
		else
		begin
			begin try
				begin tran tr_InsertProductosStock
					--Insertar Saldo
					Insert into ProductosStock values(@alm,@cprod, (@cantI*@mov*-1),@lote ,@fechaVencimiento )						
				commit tran tr_InsertProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov*-1))
			end try
			begin catch
				rollback tran tr_InsertProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov*-1))
			end catch
		end
		fetch cSaldoE into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	end

	close cSaldoE
	deallocate cSaldoE

	--2doo trabajamos sobre las filas insertadas (I)
	declare cSaldoI Cursor
		for select a.Id , a.MovimientoId , a.ProductoId , a.Cantidad , b.DepositoId , c.Factor ,a.Lote ,a.FechaVencimiento 
			from inserted a inner join Movimientos b on a.MovimientoId =b.Id 
				 inner join MovimientosTipos  c on b.ConceptoId =c.Id 

	open cSaldoI

	fetch cSaldoI into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	while (@@FETCH_STATUS = 0)
	begin
		if(exists(select * from ProductosStock a where a.ProductoId =@cprod and a.DepositoId =@alm
		and Lote =@lote and FechaVencimiento =@fechaVencimiento ))
		begin
			begin try
				begin tran tr_UpdateProductosStock
					--Obtener la cantidad actual
					set @cantA=(select Cantidad  from ProductosStock where ProductoId  = @cprod and DepositoId =@alm
					and Lote =@lote and FechaVencimiento =@fechaVencimiento )
					set @cantF=@cantA + (@cantI*@mov)
					--Actualizar Saldo
					update ProductosStock set Cantidad =@cantF
								 where ProductoId =@cprod and DepositoId =@alm and Lote =@lote and FechaVencimiento =@fechaVencimiento 						
				commit tran tr_UpdateProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end try
			begin catch
				rollback tran tr_UpdateProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end catch
		end
		else
		begin
			begin try
				begin tran tr_InsertProductosStock
					--Insertar Saldo
					Insert into ProductosStock values(@alm,@cprod, (@cantI*@mov),@lote ,@fechaVencimiento )						
				commit tran tr_InsertProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov))
			end try
			begin catch
				rollback tran tr_InsertProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov))
			end catch
		end
		fetch cSaldoI into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	end

	close cSaldoI
	deallocate cSaldoI
END




GO
SET NOEXEC OFF
GO
