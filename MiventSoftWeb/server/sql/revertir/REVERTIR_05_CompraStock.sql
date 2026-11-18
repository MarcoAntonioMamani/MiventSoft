/* =====================================================================
   SOLO PARA DESHACER 05_CompraStock.sql. NO es parte de la instalacion. Vuelve MAM_Compras de DistribucionDistralKCP2023 a la version anterior.
   ===================================================================== */
SET NOEXEC OFF
GO
IF NOT OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) LIKE '%Sanbit: stock de la compra%'
BEGIN
	RAISERROR('05 Compra stock NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
--drop procedure MAM_Compras

ALTER PROCEDURE [dbo].[MAM_Compras] (@tipo int,@Id int=-1,@AlmacenId int =-1,
@FechaTransaccion date=null,@Proveedor int=-1,@TipoVenta int=-1,
@FechaVencimientoCredito date=null,@Moneda int=-1,@Estado int=-1,@Glosa nvarchar(450)='',
@TotalCompra decimal(18,2)=0,@Descuento decimal(18,2)=0,@Usuario nvarchar(100)='',@detalle CompraDetalleType ReadOnly, @CatCosto int=-1,
@EmpresaId int=-1,@NroNotaCompra nvarchar(50)='',@NroNotaRecepcion nvarchar(50)='',@FechaI date=null,@FechaF date=null)
	
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))
	declare @numicat int
	declare @ygmer int
	declare @numicatVenta int
	DECLARE @newFecha date
	declare @Prec decimal(18,4)
	set @newFecha=GETDATE()

	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		BEGIN TRY 
		
		delete from CreditosCompras  where CompraId  =@Id 
		DELETE FROM CompraDetalle   WHERE CompraId   =@Id
			DELETE from Compras    where Id   =@Id
		
			select @Id as newNumi  --Consulcar que hace newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@Usuario)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
			BEGIN TRAN MODIFICACION
		BEGIN TRY 
        set @Id=IIF((select COUNT(Id) from Compras)=0,0,(select MAX(Id) from Compras))+1
			INSERT INTO Compras VALUES(@Id ,@AlmacenId,@FechaTransaccion ,@Proveedor  ,@TipoVenta,
			@FechaVencimientoCredito,@Moneda ,@Estado  ,@Glosa ,@Descuento  ,@TotalCompra ,@EmpresaId,@NroNotaCompra ,@NroNotaRecepcion ,1,1,@newFecha,@newHora,@Usuario)

			----INSERTO EL DEcaLLE
				INSERT INTO CompraDetalle (CompraId ,ProductoId ,Cantidad  ,PrecioCosto,Lote 
				 ,FechaVencimiento   ,PrecioVenta   ,TotalCompra  
				,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro  )

			SELECT @Id,td.ProductoId ,td.Cantidad  ,td.PrecioCosto  ,
			td.Lote  ,td.FechaVencimiento  ,td.PrecioVenta ,td.TotalCompra  ,
			@newFecha  ,@newHora  ,@Usuario    FROM @detalle AS td
			where td.estado  =0 and  td.ProductoId  >0 and td.Cantidad >0

				INSERT INTO CompraDetalleBackup (CompraId ,ProductoId ,Cantidad  ,PrecioCosto,Lote 
				 ,FechaVencimiento   ,PrecioVenta   ,TotalCompra  
				,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro,Accion,UsuarioModificacion )

			SELECT @Id,td.ProductoId ,td.Cantidad  ,td.PrecioCosto  ,
			td.Lote  ,td.FechaVencimiento  ,td.PrecioVenta ,td.TotalCompra  ,
			@newFecha  ,@newHora  ,@Usuario,'NUEVO',@Usuario    FROM @detalle AS td
			where td.estado  =0 and  td.ProductoId  >0 and td.Cantidad >0
			------Modifico los Precios Costo------------------
			
			

			set @numicat =(select Min(Id ) from PreciosCategorias  where Tipo  =0)

			update Precios  set Precios.Precio  =td.PrecioCosto  
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId  
			and Precios.AlmacenId  =@AlmacenId   and td.estado =0 and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0



			------Modifico los Precios Venta------------------
			set @numicatVenta =(select Min(Id) from PreciosCategorias where Tipo =1)
			update Precios set Precios.Precio  =td.PrecioVenta   
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicatVenta and Precios .ProductoId  =td.ProductoId  
			and Precios.AlmacenId  =@AlmacenId   and td.estado =0 and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0
			select @Id as newNumi

		
		COMMIT TRAN MODIFICACION
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@Usuario)
				   ROLLBACK TRAN MODIFICACION
		END CATCH
	END
	
	IF @tipo=2--MODIFICACION
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY 

		------Antes

				INSERT INTO CompraDetalleBackup (CompraId ,ProductoId ,Cantidad  ,PrecioCosto,Lote 
				 ,FechaVencimiento   ,PrecioVenta   ,TotalCompra  
				,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro,Accion,UsuarioModificacion )

			SELECT td.Id ,td.ProductoId ,td.Cantidad  ,td.PrecioCosto  ,
			td.Lote  ,td.FechaVencimiento  ,td.PrecioVenta ,td.TotalCompra  ,
			@newFecha  ,@newHora  ,@Usuario,'Antes Modificacion',@Usuario    FROM CompraDetalle  AS td
			where   td.CompraId  =@Id 


			--ELIMINO LOS REGISTROS
			DELETE FROM CompraDetalle  WHERE compraid=@Id

			Update Compras set AlmacenId=@AlmacenId ,ProveedorId =@Proveedor ,TipoVenta =@TipoVenta ,
			FechaVencimientoCredito =@FechaVencimientoCredito ,Moneda =@Moneda , Estado =@Estado ,
			Glosa =@Glosa ,TotalCompra =@TotalCompra ,EmpresaId =@EmpresaId ,descuento=@Descuento,
			NroNotaCompra =@NroNotaCompra ,NroNotaRecepcion =@NroNotaRecepcion 
			where Id=@Id 

		

		 ----------MODIFICO EL DEcaLLE DE EQUIPO------------
			--INSERTO LOS NUEVOS

				INSERT INTO CompraDetalle (CompraId ,ProductoId ,Cantidad  ,PrecioCosto,Lote 
				 ,FechaVencimiento   ,PrecioVenta   ,TotalCompra  
				,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro  )

			SELECT @Id,td.ProductoId ,td.Cantidad  ,td.PrecioCosto  ,
			td.Lote  ,td.FechaVencimiento  ,td.PrecioVenta ,td.TotalCompra  ,
			@newFecha  ,@newHora  ,@Usuario    FROM @detalle AS td
			where td.estado>=0 and  td.ProductoId  >0 and td.Cantidad >0


			--------------Insertar despues Modificacion
			INSERT INTO CompraDetalleBackup (CompraId ,ProductoId ,Cantidad  ,PrecioCosto,Lote 
				 ,FechaVencimiento   ,PrecioVenta   ,TotalCompra  
				,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro,Accion,UsuarioModificacion )

			SELECT @Id,td.ProductoId ,td.Cantidad  ,td.PrecioCosto  ,
			td.Lote  ,td.FechaVencimiento  ,td.PrecioVenta ,td.TotalCompra  ,
			@newFecha  ,@newHora  ,@Usuario,'Despues Modificacion',@Usuario    FROM @detalle AS td
			where td.estado  >=0 and  td.ProductoId  >0 and td.Cantidad >0
				------Modifico los Precios Costo------------------
				
			set @numicat =(select Min(Id ) from PreciosCategorias  where Tipo  =0)
				update Precios  set Precios.Precio  =td.PrecioCosto  
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId  
			and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0



			------Modifico los Precios Venta------------------
			set @numicatVenta =(select Min(Id) from PreciosCategorias where Tipo =1)
			update Precios set Precios.Precio  =td.PrecioVenta   
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicatVenta and Precios .ProductoId  =td.ProductoId  
			and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0
			select @Id as newNumi


		

			select @Id as newNumi
		COMMIT TRAN MODIFICACION
		END TRY
		BEGIN CATCH
		print ERROR_MESSAGE()
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@Usuario)
				   ROLLBACK TRAN MODIFICACION
		END CATCH
	END

	IF @tipo=3 --MOSTRaR TODOS
	BEGIN
		BEGIN TRY
		
				
		select a.Id ,a.NroNotaCompra ,a.NroNotaRecepcion ,a.AlmacenId ,a.FechaTransaccion ,a.ProveedorId,p.NombreProveedor,a.TipoVenta ,a.FechaVencimientoCredito ,
		a.Moneda ,IIF(a.Moneda =1,'Boliviano','Dolar')as TituloMoneda,a.Estado ,a.Glosa ,a.TotalCompra ,a.EmpresaId  ,a.descuento,
		IIF(Exists(select t.* from TransaccionComprasCredito as t inner join CreditosCompras as c on c.Id =t.CreditoCompraId and c.CompraId =a.id),1,0) as transaccion
		from Compras as a 
		inner join Proveedor as p on p.id=a.ProveedorId 
		order by a.id desc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=4 --MOSTRaR Decalle
	BEGIN
		BEGIN TRY
		
		

		select d.Id ,d.CompraId ,d.ProductoId ,p.DescripcionProducto as Producto,d.Cantidad ,d.Cantidad /p.Conversion as CantidadCajas,d.PrecioCosto ,
		d.Lote,d.FechaVencimiento ,d.TotalCompra ,d.PrecioVenta,p.Conversion  ,1 as estado,cast('' as image) as img,
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
		BEGIN TRY
	
	
 Declare @CategoriaCompra int,@CategoriaVenta int
	 set @CategoriaCompra =(select Min(k.Id ) from PreciosCategorias  as k where k.Tipo  =0)
	 set @CategoriaVenta =(select Min(k.Id ) from PreciosCategorias  as k where k.Tipo  =1)

	 select a.Id ,a.CodigoExterno ,a.DescripcionProducto as NombreProducto,a.DescripcionProducto,cat.NombreCategoria   ,PCosto .Precio as PrecioCosto,PVenta .Precio as PrecioVenta,1 as estado,
	 (select Sum(st.Cantidad ) from ProductosStock as st inner join
	 Almacenes as alma on  st.ProductoId =a.Id and st.DepositoId=alma .DepositoId and alma.id =@AlmacenId    ) as stock,
	 (select Sum(st.Cantidad ) from ProductosStock as st inner join
	 Almacenes as alma on  st.ProductoId =a.Id and st.DepositoId=alma .DepositoId and alma.id =@AlmacenId    )/a.Conversion  as stockCajas,
	 a.Conversion 
	 from Productos as a 
	 inner join Precios as PCosto on PCosto .PrecioCategoriaId =@CategoriaCompra 
	 and PCosto.ProductoId =a.Id 
	 inner join Precios as PVenta on PVenta .PrecioCategoriaId =@CategoriaVenta 
	 and PVenta .ProductoId =a.Id 
	 and PCosto .AlmacenId =@AlmacenId 
	 and PVenta .AlmacenId =@AlmacenId 
	 inner join Categorias as cat on cat.id=a.CategoriaId 
	 where a.estado=1
	 order by a.id asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=6 --MOSTRaR Proveedores
	BEGIN
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
		BEGIN TRY
	
	
select a.Id ,sucursal .NombreAlmacen ,FORMAT (a.FechaTransaccion , 'dd-MM-yyyy')  as FechaTransaccion,IIF(a.TipoVenta=1,'Contado','Credito') as TipoCompra ,
IIF(a.TipoVenta=1,'',FORMAT (a.FechaVencimientoCredito , 'dd-MM-yyyy') ) as FechaVencimientoCredito,prov .NombreProveedor 
,a.Glosa ,pro.Id as ProductoId,pro.DescripcionProducto as NombreProducto ,detalle .PrecioCosto ,detalle .Cantidad/pro.Conversion   as Cantidad ,detalle .TotalCompra 
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
		IF @tipo=9 --MOSTRaR Lista de Compras
	BEGIN
		BEGIN TRY
	
		select a.Id ,a.FechaTransaccion as FechaCompra,p.NombreProveedor ,isnull(a.NroNotaCompra,'')as NroNotaCompra
,isnull(a.NroNotaRecepcion,'') as NroNotaRecepcion
from Compras as a
inner join Proveedor as p on a.ProveedorId =p.Id 
where a.FechaTransaccion >=@FechaI and a.FechaTransaccion <=@FechaF 
order by a.Id desc
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

		IF @tipo=10 --ObtenerTotalesCompra
	BEGIN
		BEGIN TRY
	
select a.Id ,(select Sum(d.Cantidad )  from CompraDetalle as d where d.CompraId =a.Id ) as Cantidad,
a.TotalCompra 
from Compras as a
inner join Proveedor as p on a.ProveedorId =p.Id 
where a.FechaTransaccion >=@FechaI and a.FechaTransaccion <=@FechaF 
order by a.Id desc
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END
		IF @tipo=11 --ObtenerTotalesCompra
	BEGIN
		BEGIN TRY
	
select c.id,d.ProductoId,p.NombreProducto  ,d.Cantidad,d.PrecioCosto 
from Compras as c
inner join CompraDetalle as d on c.Id =CompraId 
inner join Productos as p on d.ProductoId =p.id
where c.FechaTransaccion >=@FechaI and c.FechaTransaccion <=@FechaF 
order by c.Id desc
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=12 --MOSTRaR Proveedores
	BEGIN
		BEGIN TRY
	
		select a.Id ,a.NombreProveedor as Proveedor
		from Proveedor as a where a.estado=1
		order by a.Id asc
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END
End





GO
SET NOEXEC OFF
GO
RAISERROR('05 Compra stock DESHECHO.', 10, 1) WITH NOWAIT
GO
