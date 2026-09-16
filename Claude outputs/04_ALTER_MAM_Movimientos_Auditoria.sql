/****** Object:  StoredProcedure [dbo].[MAM_Movimientos]    Script Date: 16/09/2026 9:45:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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

			--==== AUDITORIA: snapshot ANTES de eliminar el PAR de movimientos del traspaso ====
			DECLARE @AuditoriaId_Mov_Del1 INT, @AuditoriaId_Mov_Del2 INT

			INSERT INTO MovimientosAuditoria (Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
			SELECT Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,'ELIMINAR','ANTES',@usuario,@newFecha,@newHora
			FROM Movimientos WHERE Id=@ibid2

			SET @AuditoriaId_Mov_Del1 = SCOPE_IDENTITY()

			INSERT INTO MovimientosDetalleAuditoria (AuditoriaId,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento)
			SELECT @AuditoriaId_Mov_Del1,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento
			FROM MovimientosDetalle WHERE MovimientoId=@ibid2

			INSERT INTO MovimientosAuditoria (Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
			SELECT Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,'ELIMINAR','ANTES',@usuario,@newFecha,@newHora
			FROM Movimientos WHERE Id=@id

			SET @AuditoriaId_Mov_Del2 = SCOPE_IDENTITY()

			INSERT INTO MovimientosDetalleAuditoria (AuditoriaId,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento)
			SELECT @AuditoriaId_Mov_Del2,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento
			FROM MovimientosDetalle WHERE MovimientoId=@id
			--==== FIN AUDITORIA ====
			--set @depositoDestino =(Select a.ibalm    from TI002  as a,TA002 as b,TA001 as c where c.aata2dep =b.abnumi
   --          and a.ibid  =@ibid2   and a.ibalm  =c.aanumi )
   						DELETE FROM MovimientosDetalle   WHERE MovimientoId   =@ibid2
				DELETE from Movimientos   where id   =@ibid2
						DELETE FROM MovimientosDetalle    WHERE MovimientoId    =@id
				DELETE from Movimientos    where id   =@id


			select @id as newNumi  --Consulibr que hace newNumi



				end

				if (@ConceptoId <>8)
				begin

				--==== AUDITORIA: snapshot ANTES de eliminar ====
				DECLARE @AuditoriaId_Mov_DelSimple INT

				INSERT INTO MovimientosAuditoria (Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
				SELECT Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,'ELIMINAR','ANTES',@usuario,@newFecha,@newHora
				FROM Movimientos WHERE Id=@id

				SET @AuditoriaId_Mov_DelSimple = SCOPE_IDENTITY()

				INSERT INTO MovimientosDetalleAuditoria (AuditoriaId,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento)
				SELECT @AuditoriaId_Mov_DelSimple,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento
				FROM MovimientosDetalle WHERE MovimientoId=@id
				--==== FIN AUDITORIA ====

				DELETE FROM MovimientosDetalle  WHERE MovimientoId =@id
			DELETE FROM Movimientos  WHERE Id =@id
				end



			SELECT @id AS newNumi
			COMMIT TRAN ELIMINAR
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN ELIMINAR
			INSERT INTO Bitacora(banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact, baregistroid)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), -1, @newFecha, @newHora, @usuario, @id)
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

			--	  INSERT INTO TI002 VALUES(@ibid ,@ibfdoc,@ibconcep ,Concat(@ibobs  ,' ORIGEN: ',@ibdepdest,'-',(select TA002.abdesc from  TA002 where TA002.abnumi =@ibdepdest)),@ibest,
			--@ibalm ,@ibdepdest,@ibidOrigen,@ibiddc,@newFecha,@newHora,@ibuact)

			INSERT INTO Movimientos  VALUES(@id, @ConceptoId , @id , @DepositoId , concat(@Observacion,' ORIGEN: ',@DepositoIdDestino,' - ',(select top 1 NombreDeposito  from Depositos where Id=@DepositoIdDestino)) , @Estado , @FechaDocumento ,
									 @DepositoIdDestino,@IdMovimientoDestino ,@newFecha, @newHora, @usuario)

				 --update TI002
				 --set TI002.ididdestino=@ibid
				 --,TI002.ibobs =Concat(TI002.ibobs  ,' DESTINO: ',@ibalm ,'-',(select TA002.abdesc from  TA002 where TA002.abnumi =@ibalm))
				 --where TI002.ibid =@ibidOrigen

				--==== AUDITORIA: snapshot ANTES de modificar el movimiento destino del traspaso ====
				DECLARE @AuditoriaId_Mov_TraspasoDestino INT

				INSERT INTO MovimientosAuditoria (Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
				SELECT Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,'MODIFICAR','ANTES',@usuario,@newFecha,@newHora
				FROM Movimientos WHERE Id=@IdMovimientoDestino
				--==== FIN AUDITORIA ====

				 update Movimientos
				 set Movimientos.IdMovimientoDestino=@id
				 ,Observacion = concat(Observacion ,' DESTINO: ',@DepositoId,' - ',(select top 1 NombreDeposito  from Depositos where Id=@DepositoId))
				 where Id=@IdMovimientoDestino

				--==== AUDITORIA: snapshot DESPUES del movimiento destino modificado ====
				INSERT INTO MovimientosAuditoria (Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
				SELECT Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,'MODIFICAR','DESPUES',@usuario,@newFecha,@newHora
				FROM Movimientos WHERE Id=@IdMovimientoDestino
				--==== FIN AUDITORIA ====

			INSERT INTO MovimientosDetalle (MovimientoId , ProductoId , Cantidad ,Lote ,FechaVencimiento )
			SELECT @id,td.ProductoId , td.Cantidad , td.Lote ,td.FechaVencimiento
				FROM @detalle AS td
				WHERE td.ProductoId >0 and td.estado >=0;
				end
				if(@ConceptoId =8) --Traspaso Salida
				begin

INSERT INTO Movimientos  VALUES(@id, @ConceptoId , @id , @DepositoId , @Observacion , @Estado , @FechaDocumento ,
									 @DepositoIdDestino,@IdMovimientoDestino ,@newFecha, @newHora, @usuario)

			--	  INSERT INTO TI002 VALUES(@ibid ,@ibfdoc,@ibconcep ,@ibobs  ,@ibest,
			--@ibalm ,@ibdepdest,@ibidOrigen ,@ibiddc,@newFecha,@newHora,@ibuact)


		INSERT INTO MovimientosDetalle (MovimientoId , ProductoId , Cantidad ,Lote ,FechaVencimiento )
			SELECT @id,td.ProductoId , td.Cantidad , td.Lote ,td.FechaVencimiento
				FROM @detalle AS td
				WHERE td.ProductoId >0 and td.estado >=0;
				end
			if(@ConceptoId <>7 and @ConceptoId <>8)
				begin

					INSERT INTO Movimientos  VALUES(@id, @ConceptoId , @id , @DepositoId , @Observacion , @Estado , @FechaDocumento ,
									 @DepositoIdDestino,@IdMovimientoDestino ,@newFecha, @newHora, @usuario)

			-- INSERTO EL DETALLE
			INSERT INTO MovimientosDetalle (MovimientoId , ProductoId , Cantidad ,Lote ,FechaVencimiento )
			SELECT @id,td.ProductoId , td.Cantidad , td.Lote ,td.FechaVencimiento
				FROM @detalle AS td
				WHERE td.ProductoId >0 and td.estado >=0;

				end

			--==== AUDITORIA: snapshot DESPUES de crear ====
			DECLARE @AuditoriaId_Mov_Nuevo INT

			INSERT INTO MovimientosAuditoria (Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
			SELECT Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,'CREAR','DESPUES',@usuario,@newFecha,@newHora
			FROM Movimientos WHERE Id=@id

			SET @AuditoriaId_Mov_Nuevo = SCOPE_IDENTITY()

			INSERT INTO MovimientosDetalleAuditoria (AuditoriaId,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento)
			SELECT @AuditoriaId_Mov_Nuevo,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento
			FROM MovimientosDetalle WHERE MovimientoId=@id
			--==== FIN AUDITORIA ====

			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT @id AS newNumi
			COMMIT TRAN INSERTAR
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN INSERTAR
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact, baregistroid)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 1, @newFecha, @newHora, @usuario, @id)
		END CATCH
	END

	IF @tipo=2--MODIFICACION
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRAN MODIFICACION
		BEGIN TRY

			--==== AUDITORIA: snapshot ANTES de modificar ====
			DECLARE @AuditoriaId_Mov_ModAntes INT

			INSERT INTO MovimientosAuditoria (Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
			SELECT Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,'MODIFICAR','ANTES',@usuario,@newFecha,@newHora
			FROM Movimientos WHERE Id=@id

			SET @AuditoriaId_Mov_ModAntes = SCOPE_IDENTITY()

			INSERT INTO MovimientosDetalleAuditoria (AuditoriaId,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento)
			SELECT @AuditoriaId_Mov_ModAntes,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento
			FROM MovimientosDetalle WHERE MovimientoId=@id
			--==== FIN AUDITORIA ANTES ====

			--MODIFICO EL DETALLE
			DELETE FROM MovimientosDetalle  WHERE MovimientoId =@id;

			UPDATE Movimientos  SET ConceptoId =@ConceptoId , DepositoId =@DepositoId
			, Observacion =@Observacion , Estado =@Estado , FechaDocumento =@FechaDocumento ,
							 FechaRegistro =@newFecha, HoraRegistro =@newHora, UsuarioRegistro =@usuario
					 Where id=@id;

			INSERT INTO MovimientosDetalle (MovimientoId , ProductoId , Cantidad ,Lote ,FechaVencimiento )
			SELECT @id,td.ProductoId , td.Cantidad , td.Lote ,td.FechaVencimiento
				FROM @detalle AS td
				WHERE td.ProductoId >0 and td.estado >=0;

			--==== AUDITORIA: snapshot DESPUES de modificar ====
			DECLARE @AuditoriaId_Mov_ModDespues INT

			INSERT INTO MovimientosAuditoria (Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Momento,UsuarioAccion,FechaAccion,HoraAccion)
			SELECT Id,ConceptoId,TransaccionId,DepositoId,Observacion,Estado,FechaDocumento,DepositoIdDestino,IdMovimientoDestino,FechaRegistro,HoraRegistro,UsuarioRegistro,'MODIFICAR','DESPUES',@usuario,@newFecha,@newHora
			FROM Movimientos WHERE Id=@id

			SET @AuditoriaId_Mov_ModDespues = SCOPE_IDENTITY()

			INSERT INTO MovimientosDetalleAuditoria (AuditoriaId,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento)
			SELECT @AuditoriaId_Mov_ModDespues,MovimientoId,Id,ProductoId,Cantidad,Lote,FechaVencimiento
			FROM MovimientosDetalle WHERE MovimientoId=@id
			--==== FIN AUDITORIA DESPUES ====

			--DEVUELVO VALORES DE CONFIRMACION
			select @id as newNumi
			COMMIT TRAN MODIFICACION
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN MODIFICACION
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact, baregistroid)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 2, @newFecha, @newHora, @usuario, @id)
		END CATCH
	END

	IF @tipo=3 --MOSTRAR TODOS LOS MOVIMIENTOS
	BEGIN
	 SET ARITHABORT ON
		DECLARE @CategoriaCompraBuscador int
		SET @CategoriaCompraBuscador = (select Min(Id) from PreciosCategorias where Tipo=0)
		BEGIN TRY
			SELECT a.Id  as id, a.FechaDocumento  as fdoc, a.ConceptoId  as concep, b.Descripcion  as nconcep, a.Observacion  as obs,
				   a.Estado  as est, a.DepositoId  as alm ,dep.NombreDeposito,
				   ISNULL(totMov.TotalMovimiento,0) as TotalMovimiento
			FROM Movimientos  a inner join MovimientosTipos  b on a.ConceptoId =b.Id
			inner join Depositos as dep on dep.id=a.DepositoId
			OUTER APPLY (
				select sum(d.Cantidad * ISNULL(costoDet.Precio,0)) as TotalMovimiento
				from MovimientosDetalle as d
				OUTER APPLY (
					select top 1 costo.Precio
					from Precios as costo
					where costo.ProductoId=d.ProductoId and costo.PrecioCategoriaId=@CategoriaCompraBuscador and costo.AlmacenId=1
				) as costoDet
				where d.MovimientoId=a.Id
			) as totMov
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
		DECLARE @CategoriaCompraMov int
		SET @CategoriaCompraMov = (select Min(Id) from PreciosCategorias where Tipo=0)
		BEGIN TRY
			SELECT a.id , a.MovimientoId , a.ProductoId , b.NombreProducto  as Producto, a.Cantidad ,
			a.Lote ,a.FechaVencimiento ,CAST ('' as image ) as img, 1 as estado ,Sum(stock .Cantidad )as stock ,
			b.CodigoBarras ,b.UnidadVentaId ,unidadMin.Descripcion as UnidadVenta ,b.UnidadMaximaId ,unidadMax.Descripcion as UnidadMaxima ,ISNULL(NULLIF(b.Conversion,0),1) as Conversion ,
			ISNULL((select top 1 costo.Precio from Precios as costo where costo.ProductoId=b.Id and costo.PrecioCategoriaId=@CategoriaCompraMov and costo.AlmacenId=1),0) as precio,
			a.Cantidad * ISNULL((select top 1 costo.Precio from Precios as costo where costo.ProductoId=b.Id and costo.PrecioCategoriaId=@CategoriaCompraMov and costo.AlmacenId=1),0) as Total
			FROM MovimientosDetalle  a inner join Productos  b on a.ProductoId =b.Id
			inner join ProductosStock as stock on stock .ProductoId =b.Id and a.FechaVencimiento =stock .FechaVencimiento
			and a.Lote =stock .Lote
			left join ClasificadorDetalle as unidadMin on unidadMin.id=b.UnidadVentaId
			left join ClasificadorDetalle as unidadMax on unidadMax.id=b.UnidadMaximaId
			where a.MovimientoId  =@id
			group by a.id,a.MovimientoId ,a.ProductoId ,b.Id ,b.NombreProducto ,a.Cantidad ,a.Lote ,a.FechaVencimiento,
			b.CodigoBarras ,b.UnidadVentaId ,unidadMin.Descripcion ,b.UnidadMaximaId ,unidadMax.Descripcion ,b.Conversion
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
		DECLARE @CategoriaCompraMovPick int
		SET @CategoriaCompraMovPick = (select Min(Id) from PreciosCategorias where Tipo=0)
		BEGIN TRY


		 select p.Id ,p.CodigoExterno ,p.CodigoBarras ,p.NombreProducto,p.NombreProducto as DescripcionProducto,industria.Descripcion as industria,cat.NombreCategoria  ,
		 p.UnidadVentaId ,unidadMin.Descripcion as UnidadVenta ,p.UnidadMaximaId ,unidadMax.Descripcion as UnidadMaxima ,ISNULL(NULLIF(p.Conversion,0),1) as Conversion ,
		 ISNULL((select top 1 costo.Precio from Precios as costo where costo.ProductoId=p.Id and costo.PrecioCategoriaId=@CategoriaCompraMovPick and costo.AlmacenId=1),0) as precio,
		 Sum(stock .Cantidad ) as stock , 1 as estado
		 from Productos as p inner join
		 ProductosStock as stock on stock .ProductoId =p.Id
		 inner join Categorias as cat on cat.Id =p.CategoriaId
		 inner join ClasificadorDetalle as industria on industria .id=p.AttributoId
		 left join ClasificadorDetalle as unidadMin on unidadMin.id=p.UnidadVentaId
		 left join ClasificadorDetalle as unidadMax on unidadMax.id=p.UnidadMaximaId
		 where stock .DepositoId =@DepositoId
		 group by p.Id ,p.CodigoExterno ,p.CodigoBarras ,p.NombreProducto ,p.DescripcionProducto,cat.NombreCategoria,industria .Descripcion,
		 p.UnidadVentaId ,unidadMin.Descripcion ,p.UnidadMaximaId ,unidadMax.Descripcion ,p.Conversion


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
