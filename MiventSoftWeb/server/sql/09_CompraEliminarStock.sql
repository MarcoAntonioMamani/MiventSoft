/* =====================================================================
   09 - NO ELIMINAR UNA COMPRA SI EL STOCK QUEDA NEGATIVO (web y escritorio usan el mismo SP)

   MAM_Compras @tipo -1: antes de borrar, revisa lo que la compra ingreso. Si eso ya se vendio o salio y el
   stock quedaria NEGATIVO, no elimina nada y avisa:
     "NO SE PUEDE ELIMINAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: Producto (stock actual X, la compra ingreso Y); ..."
   Quedar en 0 SI se permite. Eliminar la compra ahora es todo o nada (antes, si fallaba a medias,
   quedaba borrada la caja o el credito pero no el detalle). Eliminar un ingreso de inventario ya lo valida el 05.
   Todo lo demas del SP queda IDENTICO a la version con el 08.

   Generado desde KailiIndustrialLaPaz (2026-11-13). PROTECCION: solo se aplica si MAM_Compras es esa version (con el 08, sin contar
   mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da error y no modifica nada.
   REQUISITO: 08_Auditoria_SPs.sql ejecutado antes.
   VOLVER ATRAS: revertir\REVERTIR_09_CompraEliminarStock.sql (SOLO para deshacer).
   ===================================================================== */
SET NOEXEC OFF
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) LIKE '%Kaily: stock al eliminar compra%'
BEGIN
	RAISERROR('09 Compra eliminar stock YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
ELSE IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) NOT LIKE '%Kaily: auditoria%'
BEGIN
	RAISERROR('09: falta ejecutar primero 08_Auditoria_SPs.sql. No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
ELSE IF ISNULL(HASHBYTES('SHA2_256', REPLACE(REPLACE(REPLACE(REPLACE(UPPER(OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras'))), CHAR(13), ''), CHAR(10), ''), CHAR(9), ''), ' ', '')), 0x) <> 0x060E622AC3E6C9D3118291AA19CC4DD3D166D5A711AFF5635A10576730E18B8D
BEGIN
	RAISERROR('09: MAM_Compras es OTRA version (no la de KailiIndustrialLaPaz 2026-11-13 con el 08). No se modifico nada.', 16, 1)
	SET NOEXEC ON
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
		-- ===== Kaily: stock al eliminar compra (web y escritorio) =====================================
		-- Eliminar la compra quita del stock lo que ingreso (su movimiento concepto 2). Si eso ya se vendio o salio y el
		-- stock quedaria NEGATIVO, no se elimina nada y se avisa que productos y con que stock actual.
		-- Las filas de stock quedan BLOQUEADAS (UPDLOCK) hasta terminar: si otro vende el mismo producto, espera.
		DECLARE @tcCompraE int = @@TRANCOUNT
		IF @tcCompraE = 0 BEGIN TRAN ELSE SAVE TRAN CompraEliminar
		DECLARE @msgStockCE nvarchar(2000)
		SELECT @msgStockCE = STUFF((
			SELECT TOP 10 '; ' + RTRIM(p.NombreProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))
				+ ', la compra ingreso ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'
			FROM (SELECT d.ProductoId, m.DepositoId, SUM(-d.Cantidad * c.Factor) AS Delta
			      FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId
			      WHERE m.ConceptoId = 2 AND m.TransaccionId = @Id
			      GROUP BY d.ProductoId, m.DepositoId) AS e
			INNER JOIN Productos AS p ON p.Id = e.ProductoId
			OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)
			             WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s
			WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0
			ORDER BY p.NombreProducto
			FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')
		IF @msgStockCE IS NOT NULL
		BEGIN
			IF @tcCompraE = 0 ROLLBACK TRAN ELSE ROLLBACK TRAN CompraEliminar
			SET @msgStockCE = 'NO SE PUEDE ELIMINAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: ' + @msgStockCE
			RAISERROR('%s', 16, 1, @msgStockCE)
			RETURN
		END
		-- ==========================================================================================
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
			IF @tcCompraE = 0 AND @@TRANCOUNT > 0 COMMIT TRAN /* Kaily: stock al eliminar compra */
		END TRY
		BEGIN CATCH
			IF @tcCompraE = 0 AND @@TRANCOUNT > 0 ROLLBACK TRAN ELSE IF @tcCompraE > 0 AND XACT_STATE() = 1 ROLLBACK TRAN CompraEliminar /* Kaily: stock al eliminar compra */
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
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) LIKE '%Kaily: stock al eliminar compra%'
	RAISERROR('09 Compra eliminar stock INSTALADO.', 10, 1) WITH NOWAIT
ELSE
	RAISERROR('09: fallo la instalacion (revisar los mensajes de arriba).', 16, 1)
GO
SET NOEXEC OFF
GO
