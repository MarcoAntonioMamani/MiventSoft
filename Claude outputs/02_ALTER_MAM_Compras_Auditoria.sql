/****** Object:  StoredProcedure [dbo].[MAM_Compras]    Script Date: 16/09/2026 9:45:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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

	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRAN Tr_EliminarCompra
		BEGIN TRY
			--==== AUDITORIA: snapshot ANTES de eliminar (Compras es DELETE fisico de cabecera y detalle) ====
			DECLARE @AuditoriaId_Compras_Del INT

			INSERT INTO ComprasAuditoria (Id,AlmacenId,FechaTransaccion,ProveedorId,TipoVenta,FechaVencimientoCredito,Moneda,Estado,Glosa,Descuento,TotalCompra,EmpresaId,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
			SELECT Id,AlmacenId,FechaTransaccion,ProveedorId,TipoVenta,FechaVencimientoCredito,Moneda,Estado,Glosa,Descuento,TotalCompra,EmpresaId,FechaRegistro,HoraRegistro,UsuarioRegistro,'ELIMINAR','ANTES',@Usuario,@newFecha,@newHora
			FROM Compras WHERE Id=@Id

			SET @AuditoriaId_Compras_Del = SCOPE_IDENTITY()

			INSERT INTO CompraDetalleAuditoria (AuditoriaId,CompraId,Id,ProductoId,CantidadCompra,PorcentajeIncremento,CantidadIncremento,Cantidad,PrecioCosto,Lote,FechaVencimiento,PrecioVenta,TotalCompra,FechaRegistro,HoraRegistro,UsuarioRegistro)
			SELECT @AuditoriaId_Compras_Del,CompraId,Id,ProductoId,CantidadCompra,PorcentajeIncremento,CantidadIncremento,Cantidad,PrecioCosto,Lote,FechaVencimiento,PrecioVenta,TotalCompra,FechaRegistro,HoraRegistro,UsuarioRegistro
			FROM CompraDetalle WHERE CompraId=@Id
			--==== FIN AUDITORIA ====

		   delete from CajaIngresoEgreso  where Modulo =2 and IdModulo =@Id and CajaId =1

update Configuracion set TotalCajaGeneral=(
select Sum((c.monto*a.Operacion)) as Total
from CajaIngresoEgreso as c
inner join CajaTipoMovimiento as a on c.CajatipoMovimientoId =a.Id
 where CajaId =1)


		delete from CreditosCompras  where CompraId  =@Id
		DELETE FROM CompraDetalle   WHERE CompraId   =@Id
			DELETE from Compras    where Id   =@Id

			select @Id as newNumi  --Consulcar que hace newNumi
			COMMIT TRAN Tr_EliminarCompra
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN Tr_EliminarCompra
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@Usuario,@Id)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY
			begin tran Tr_UpdateTI001
        set @Id=IIF((select COUNT(Id) from Compras)=0,0,(select MAX(Id) from Compras))+1
			INSERT INTO Compras VALUES(@Id ,@AlmacenId,@FechaTransaccion ,@Proveedor  ,@TipoVenta,
			@FechaVencimientoCredito,@Moneda ,@Estado  ,@Glosa ,@Descuento  ,@TotalCompra ,@EmpresaId,@newFecha,@newHora,@Usuario)


				------Modifico los Precios Costo------------------



			set @numicat =(select Min(Id ) from PreciosCategorias  where Tipo  =0)

			update Precios  set Precios.Precio  =td.PrecioCosto
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId
			and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

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
			update Precios set Precios.Precio  =td.PrecioVenta
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicatVenta and Precios .ProductoId  =td.ProductoId
			and Precios.AlmacenId  =@AlmacenId   and td.estado =0 and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0
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

			--==== AUDITORIA: snapshot DESPUES de crear ====
			DECLARE @AuditoriaId_Compras_Nueva INT

			INSERT INTO ComprasAuditoria (Id,AlmacenId,FechaTransaccion,ProveedorId,TipoVenta,FechaVencimientoCredito,Moneda,Estado,Glosa,Descuento,TotalCompra,EmpresaId,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
			SELECT Id,AlmacenId,FechaTransaccion,ProveedorId,TipoVenta,FechaVencimientoCredito,Moneda,Estado,Glosa,Descuento,TotalCompra,EmpresaId,FechaRegistro,HoraRegistro,UsuarioRegistro,'CREAR','DESPUES',@Usuario,@newFecha,@newHora
			FROM Compras WHERE Id=@Id

			SET @AuditoriaId_Compras_Nueva = SCOPE_IDENTITY()

			INSERT INTO CompraDetalleAuditoria (AuditoriaId,CompraId,Id,ProductoId,CantidadCompra,PorcentajeIncremento,CantidadIncremento,Cantidad,PrecioCosto,Lote,FechaVencimiento,PrecioVenta,TotalCompra,FechaRegistro,HoraRegistro,UsuarioRegistro)
			SELECT @AuditoriaId_Compras_Nueva,CompraId,Id,ProductoId,CantidadCompra,PorcentajeIncremento,CantidadIncremento,Cantidad,PrecioCosto,Lote,FechaVencimiento,PrecioVenta,TotalCompra,FechaRegistro,HoraRegistro,UsuarioRegistro
			FROM CompraDetalle WHERE CompraId=@Id
			--==== FIN AUDITORIA ====

			commit tran Tr_UpdateTI001

		END TRY
		BEGIN CATCH

		Print ERROR_MESSAGE()
		IF @@TRANCOUNT > 0 rollback tran Tr_UpdateTI001
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@Usuario,@Id)
		END CATCH
	END

	IF @tipo=2--MODIFICACION
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRAN Tr_ModificarCompra
		BEGIN TRY

			--==== AUDITORIA: snapshot ANTES de modificar ====
			DECLARE @AuditoriaId_Compras_ModAntes INT

			INSERT INTO ComprasAuditoria (Id,AlmacenId,FechaTransaccion,ProveedorId,TipoVenta,FechaVencimientoCredito,Moneda,Estado,Glosa,Descuento,TotalCompra,EmpresaId,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
			SELECT Id,AlmacenId,FechaTransaccion,ProveedorId,TipoVenta,FechaVencimientoCredito,Moneda,Estado,Glosa,Descuento,TotalCompra,EmpresaId,FechaRegistro,HoraRegistro,UsuarioRegistro,'MODIFICAR','ANTES',@Usuario,@newFecha,@newHora
			FROM Compras WHERE Id=@Id

			SET @AuditoriaId_Compras_ModAntes = SCOPE_IDENTITY()

			INSERT INTO CompraDetalleAuditoria (AuditoriaId,CompraId,Id,ProductoId,CantidadCompra,PorcentajeIncremento,CantidadIncremento,Cantidad,PrecioCosto,Lote,FechaVencimiento,PrecioVenta,TotalCompra,FechaRegistro,HoraRegistro,UsuarioRegistro)
			SELECT @AuditoriaId_Compras_ModAntes,CompraId,Id,ProductoId,CantidadCompra,PorcentajeIncremento,CantidadIncremento,Cantidad,PrecioCosto,Lote,FechaVencimiento,PrecioVenta,TotalCompra,FechaRegistro,HoraRegistro,UsuarioRegistro
			FROM CompraDetalle WHERE CompraId=@Id
			--==== FIN AUDITORIA ANTES ====

			--ELIMINO LOS REGISTROS
			DELETE FROM CompraDetalle  WHERE CompraId=@Id

			Update Compras set AlmacenId=@AlmacenId ,ProveedorId =@Proveedor ,TipoVenta =@TipoVenta ,
			FechaVencimientoCredito =@FechaVencimientoCredito ,Moneda =@Moneda , Estado =@Estado ,
			Glosa =@Glosa ,TotalCompra =@TotalCompra ,EmpresaId =@EmpresaId ,descuento=@Descuento
			where Id=@Id

			------Modifico los Precios Costo------------------


				---Tipo =1 Venta   0=Compra


			set @numicat =(select Min(Id ) from PreciosCategorias  where Tipo  =0)

			update Precios  set Precios.Precio  =(((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )*Precios.Precio  )+(td.Cantidad *td.PrecioCosto ))/((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )+td.Cantidad )
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId
			and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

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
			update Precios set Precios.Precio  =td.PrecioVenta
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicatVenta and Precios .ProductoId  =td.ProductoId
			and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0
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

			--==== AUDITORIA: snapshot DESPUES de modificar ====
			DECLARE @AuditoriaId_Compras_ModDespues INT

			INSERT INTO ComprasAuditoria (Id,AlmacenId,FechaTransaccion,ProveedorId,TipoVenta,FechaVencimientoCredito,Moneda,Estado,Glosa,Descuento,TotalCompra,EmpresaId,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
			SELECT Id,AlmacenId,FechaTransaccion,ProveedorId,TipoVenta,FechaVencimientoCredito,Moneda,Estado,Glosa,Descuento,TotalCompra,EmpresaId,FechaRegistro,HoraRegistro,UsuarioRegistro,'MODIFICAR','DESPUES',@Usuario,@newFecha,@newHora
			FROM Compras WHERE Id=@Id

			SET @AuditoriaId_Compras_ModDespues = SCOPE_IDENTITY()

			INSERT INTO CompraDetalleAuditoria (AuditoriaId,CompraId,Id,ProductoId,CantidadCompra,PorcentajeIncremento,CantidadIncremento,Cantidad,PrecioCosto,Lote,FechaVencimiento,PrecioVenta,TotalCompra,FechaRegistro,HoraRegistro,UsuarioRegistro)
			SELECT @AuditoriaId_Compras_ModDespues,CompraId,Id,ProductoId,CantidadCompra,PorcentajeIncremento,CantidadIncremento,Cantidad,PrecioCosto,Lote,FechaVencimiento,PrecioVenta,TotalCompra,FechaRegistro,HoraRegistro,UsuarioRegistro
			FROM CompraDetalle WHERE CompraId=@Id
			--==== FIN AUDITORIA DESPUES ====

			select @Id as newNumi
			COMMIT TRAN Tr_ModificarCompra
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN Tr_ModificarCompra
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@Usuario,@Id)
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


		--Se agregan UnidadVentaId/UnidadVenta/UnidadMaximaId/UnidadMaxima/Conversion/CantidadCaja
		--(mismo patron que MAM_Movimientos @tipo=4, mas CantidadCaja ya calculada): solo
		--lectura para el VB, no se graban en CompraDetalle ni forman parte de CompraDetalleType.
		select d.Id ,d.CompraId ,d.ProductoId ,p.NombreProducto as Producto,isnull(d.CantidadCompra,d.Cantidad ) as CantidadCompra ,isnull(d.PorcentajeIncremento,0) as PorcentajeIncremento,isnull(d.CantidadIncremento,0) as CantidadIncremento ,d.Cantidad ,d.PrecioCosto ,
		d.Lote,d.FechaVencimiento ,d.TotalCompra ,d.PrecioVenta ,1 as estado,cast('' as image) as img,
		d.PrecioCosto as costo,d.PrecioVenta  as venta,
		p.UnidadVentaId ,unidadMin.Descripcion as UnidadVenta ,p.UnidadMaximaId ,unidadMax.Descripcion as UnidadMaxima ,ISNULL(NULLIF(p.Conversion,0),1) as Conversion,
		isnull(d.CantidadCompra,d.Cantidad ) / ISNULL(NULLIF(p.Conversion,0),1) as CantidadCaja
		from CompraDetalle as d
		inner join Productos as p on p.Id =d.ProductoId
		left join ClasificadorDetalle as unidadMin on unidadMin.id=p.UnidadVentaId
		left join ClasificadorDetalle as unidadMax on unidadMax.id=p.UnidadMaximaId
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

	 --Se agregan UnidadVentaId/UnidadVenta/UnidadMaximaId/UnidadMaxima/Conversion (mismo
	 --patron que MAM_Movimientos @tipo=7): permite mostrar/calcular Cantidad Caja en el
	 --detalle de la compra al elegir el producto.
	 select a.Id ,a.CodigoExterno ,a.NombreProducto,a.NombreProducto as DescripcionProducto,industria .Descripcion as industria,cat.NombreCategoria   ,PCosto .Precio as PrecioCosto,PVenta .Precio as PrecioVenta,1 as estado,
	 isnull((select Sum(st.Cantidad ) from ProductosStock as st inner join
	 Almacenes as alma on  st.ProductoId =a.Id and st.DepositoId=alma .DepositoId and alma.id =@AlmacenId    ),0) as stock,
	 a.UnidadVentaId ,unidadMin.Descripcion as UnidadVenta ,a.UnidadMaximaId ,unidadMax.Descripcion as UnidadMaxima ,ISNULL(NULLIF(a.Conversion,0),1) as Conversion
	 from Productos as a
	 inner join Precios as PCosto on PCosto .PrecioCategoriaId =@CategoriaCompra
	 and PCosto.ProductoId =a.Id
	 inner join Precios as PVenta on PVenta .PrecioCategoriaId =@CategoriaVenta
	 and PVenta .ProductoId =a.Id
	 and PCosto .AlmacenId =@AlmacenId
	 and PVenta .AlmacenId =@AlmacenId
	 inner join Categorias as cat on cat.id=a.CategoriaId
	 inner join ClasificadorDetalle as industria on industria .id=a.AttributoId
	 left join ClasificadorDetalle as unidadMin on unidadMin.id=a.UnidadVentaId
	 left join ClasificadorDetalle as unidadMax on unidadMax.id=a.UnidadMaximaId
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
