/* =====================================================================
   05 - NO ELIMINAR NI MODIFICAR UNA COMPRA SI EL STOCK QUEDA NEGATIVO (web y escritorio usan el mismo SP)

   MAM_Compras @tipo -1 (eliminar) y @tipo 2 (modificar): antes de grabar, revisa lo que la compra deja de
   ingresar. Si eso ya se vendio o salio y el stock quedaria NEGATIVO, no graba nada y avisa:
     "NO SE PUEDE ELIMINAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: Producto (stock actual X, la compra ingreso Y); ..."
     "NO SE PUEDE MODIFICAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: Producto (stock actual X, se quitaria Y); ..."
   Quedar en 0 SI se permite. Eliminar la compra ahora es todo o nada (antes no tenia transaccion).
   Eliminar un ingreso de inventario (Movimientos) lo valida el 04. Todo lo demas del SP queda IDENTICO.

   Generado desde DistribucionDistralKCP2023 (2026-11-15). PROTECCION: solo se aplica si MAM_Compras es esa version (sin contar
   mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da error y no modifica nada.
   VOLVER ATRAS: revertir\REVERTIR_05_CompraStock.sql (SOLO para deshacer).
   ===================================================================== */
SET NOEXEC OFF
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) LIKE '%Sanbit: stock de la compra%'
BEGIN
	RAISERROR('05 Compra stock YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
ELSE IF ISNULL(HASHBYTES('SHA2_256', REPLACE(REPLACE(REPLACE(REPLACE(UPPER(OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras'))), CHAR(13), ''), CHAR(10), ''), CHAR(9), ''), ' ', '')), 0x) <> 0x49195B8F5139BD10324B14982AB27C5D3970DD8813E9A0DBC85BB69A73CD5078
BEGIN
	RAISERROR('05: MAM_Compras es OTRA version (no la de DistribucionDistralKCP2023 2026-11-15). No se modifico nada.', 16, 1)
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
		-- ===== Sanbit: stock de la compra (web y escritorio) =====================================
		-- Eliminar la compra quita del stock lo que ingreso. Si eso ya se vendio o salio y el stock quedaria NEGATIVO,
		-- no se elimina nada y se avisa que productos y con que stock actual. Las filas de stock quedan BLOQUEADAS
		-- (UPDLOCK) hasta terminar. Eliminar ahora es todo o nada (antes se podia borrar a medias).
		DECLARE @tcCompraE int = @@TRANCOUNT
		IF @tcCompraE = 0 BEGIN TRAN ELSE SAVE TRAN CompraEliminar
		DECLARE @efStockCE TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))
		INSERT INTO @efStockCE (ProductoId, DepositoId, Delta)
		SELECT d.ProductoId, m.DepositoId, -d.Cantidad * c.Factor
		FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId
		WHERE m.ConceptoId = 2 AND m.TransaccionId = @Id
		DECLARE @msgStockCE nvarchar(2000)
		SELECT @msgStockCE = STUFF((
			SELECT TOP 10 '; ' + RTRIM(p.DescripcionProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))
				+ ', la compra ingreso ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'
			FROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStockCE GROUP BY ProductoId, DepositoId) AS e
			INNER JOIN Productos AS p ON p.Id = e.ProductoId
			OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)
			             WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s
			WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0
			ORDER BY p.DescripcionProducto
			FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')
		SET @msgStockCE = 'NO SE PUEDE ELIMINAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: ' + @msgStockCE
		IF @msgStockCE IS NOT NULL
		BEGIN
			IF @tcCompraE = 0 ROLLBACK TRAN ELSE ROLLBACK TRAN CompraEliminar
			RAISERROR('%s', 16, 1, @msgStockCE)
			RETURN
		END
		-- ==========================================================================================
		BEGIN TRY 
		
		delete from CreditosCompras  where CompraId  =@Id 
		DELETE FROM CompraDetalle   WHERE CompraId   =@Id
			DELETE from Compras    where Id   =@Id
		
			select @Id as newNumi  --Consulcar que hace newNumi
			IF @tcCompraE = 0 AND @@TRANCOUNT > 0 COMMIT TRAN /* Sanbit: stock de la compra */
		END TRY
		BEGIN CATCH
			IF @tcCompraE = 0 AND @@TRANCOUNT > 0 ROLLBACK TRAN ELSE IF @tcCompraE > 0 AND XACT_STATE() = 1 ROLLBACK TRAN CompraEliminar /* Sanbit: stock de la compra */
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
		-- ===== Sanbit: stock de la compra (web y escritorio) =====================================
		-- Modificar la compra cambia el stock: lo que ya no ingresa (cantidad menor o producto quitado) sale del stock.
		-- Si eso ya se vendio o salio y el stock quedaria NEGATIVO, no se modifica nada y se avisa.
		DECLARE @efStockCM TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))
		INSERT INTO @efStockCM (ProductoId, DepositoId, Delta)
		SELECT td.ProductoId, (SELECT a.DepositoId FROM Almacenes AS a WHERE a.Id = @AlmacenId), td.Cantidad
		FROM @detalle AS td WHERE td.estado >= 0 AND td.ProductoId > 0 AND td.Cantidad > 0
		UNION ALL
		SELECT d.ProductoId, m.DepositoId, -d.Cantidad * c.Factor
		FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId
		WHERE m.ConceptoId = 2 AND m.TransaccionId = @Id
		DECLARE @msgStockCM nvarchar(2000)
		SELECT @msgStockCM = STUFF((
			SELECT TOP 10 '; ' + RTRIM(p.DescripcionProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))
				+ ', se quitaria ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'
			FROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStockCM GROUP BY ProductoId, DepositoId) AS e
			INNER JOIN Productos AS p ON p.Id = e.ProductoId
			OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)
			             WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s
			WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0
			ORDER BY p.DescripcionProducto
			FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')
		SET @msgStockCM = 'NO SE PUEDE MODIFICAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: ' + @msgStockCM
		IF @msgStockCM IS NOT NULL
		BEGIN
			ROLLBACK TRAN
			RAISERROR('%s', 16, 1, @msgStockCM)
			RETURN
		END
		-- ==========================================================================================
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
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) LIKE '%Sanbit: stock de la compra%'
	RAISERROR('05 Compra stock INSTALADO.', 10, 1) WITH NOWAIT
GO
