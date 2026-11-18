/* =====================================================================
   sp_Mam_Precios @tipo 5 (Tec_Precios: matriz producto x categoria)
   CAMBIA UNA SOLA LINEA (resto del SP identico al de la BD):
     antes:  select b.PrecioCategoriaId  from Precios  as b inner join Productos  as e on b.ProductoId  =c.Id and b.AlmacenId  =@almacen )
     ahora:  select b.PrecioCategoriaId  from Precios  as b where b.ProductoId  =c.Id and b.AlmacenId  =@almacen )
   El "inner join Productos as e" no tenia condicion sobre e: cruzaba cada precio con todos los
   productos. Mismo resultado (en Kaily: 43 s -> 0.2 s cuando el SQL arma un mal plan).

   Generado desde DistribucionDistralKCP2023 (2026-11-15). Si ya esta corregido avisa y no hace nada; si el SP es otra version,
   da error y no modifica nada.
   VOLVER ATRAS: revertir\REVERTIR_03_sp_Mam_Precios_tipo5.sql (SOLO para deshacer).
   ===================================================================== */
SET NOEXEC OFF
GO
IF CHARINDEX(N'select b.PrecioCategoriaId  from Precios  as b where b.ProductoId  =c.Id and b.AlmacenId  =@almacen )', OBJECT_DEFINITION(OBJECT_ID('dbo.sp_Mam_Precios'))) > 0
BEGIN
	RAISERROR('sp_Mam_Precios YA ESTA CORREGIDO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
ELSE IF ISNULL(HASHBYTES('SHA2_256', REPLACE(REPLACE(REPLACE(REPLACE(UPPER(OBJECT_DEFINITION(OBJECT_ID('dbo.sp_Mam_Precios'))), CHAR(13), ''), CHAR(10), ''), CHAR(9), ''), ' ', '')), 0x) <> 0xA337FFF7A2C6F65D124332C3EA537A2D5213830FCD05E8C20D077D8F8E20CA9A
BEGIN
	RAISERROR('sp_Mam_Precios es OTRA version (no la de DistribucionDistralKCP2023 2026-11-15). No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
GO
--drop procedure sp_Mam_TY006
ALTER PROCEDURE [dbo].[sp_Mam_Precios] (@tipo int,@Id int=-1,@Codigo nvarchar(50)='',@Descripcion nvarchar(80)=''
,@TipoCategoria int=-1,@TY007 TY007Type Readonly,
@usuario nvarchar(10)='',@almacen int=-1,@categoria int=-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()
	

	IF @tipo=-1--Eliminar  Categoria
	BEGIN
		BEGIN TRY 
				 ----------MODIFICO EL DETALLE DE EQUIPO------------
			--INSERTO LOS NUEVOS
			delete from Precios  where PrecioCategoriaId  = @Id
			delete from PreciosCategorias  where id =@Id 
				select @Id as newNumi
			
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@usuario)
		END CATCH
		End
	
	IF @tipo=1--Nuevo Registro De Categoria
	BEGIN
		BEGIN TRY 
				 ----------MODIFICO EL DETALLE DE EQUIPO------------
			--INSERTO LOS NUEVOS
			set @Id=IIF((select COUNT(Id) from PreciosCategorias)=0,0,(select MAX(Id) from PreciosCategorias))+1

			INSERT INTO PreciosCategorias  values ( @Id, @Codigo ,@Descripcion ,@TipoCategoria , @newFecha ,@newHora,@usuario )

				select @Id as newNumi


			
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@usuario)
		END CATCH
		End
IF @tipo=2--Nuevo Registro De Precios y modificacion de categorias
	BEGIN
		BEGIN TRY 
				 ----------MODIFICO EL DETALLE DE EQUIPO------------
			--INSERTO LOS NUEVOS

			INSERT INTO Precios(AlmacenId  ,ProductoId   ,PrecioCategoriaId   ,Precio    ,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro )
			SELECT @almacen ,td.yhprod ,td.yhcatpre ,td.yhprecio 
			,@newFecha ,@newHora,@usuario   FROM @TY007 AS td
			where td.estado =3

			--MODIFICO LOS REGISTROS
			UPDATE Precios  
			SET Precio  =td.yhprecio 
			FROM Precios   INNER JOIN @TY007 AS td
			ON Precios .Id     = td.yhnumi   and td.estado=2;
			select @Id as newNumi
					--MODIFICO LOS REGISTROS
		--	UPDATE TY006 
		--	SET ygdesc  =td.ygdesc ,ygpcv =td.ygpcv  
		--	FROM TY006   INNER JOIN @TY006 AS td
		--	ON TY006 .ygnumi      = td.ygnumi   and td.estado=2;

				


			
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@usuario)
		END CATCH

	END

	IF @tipo=3 --MOSTRAR TODAS LAS CATEGORIAS
	BEGIN
		BEGIN TRY
	
	

select a.id as ygnumi,a.Codigo as ygcod,a.Descripcion as ygdesc,a.Tipo as ygpcv,IIF (a.Tipo =1 ,'Venta','Compra') as estado
from PreciosCategorias as a 
order by a.Codigo 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario)
		END CATCH

END

	IF @tipo=4 --MOSTRAR TODAS LAS SUCURSALES
	BEGIN
		BEGIN TRY
	
	select a.id as aanumi ,a.NombreAlmacen  as aabdes 
	from Almacenes  as a order by a.id asc
		
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario)
		END CATCH

END

	IF @tipo=5 --MOSTRAR TODOS LOS PRECIOS POR PRODUCTOS
	BEGIN
		BEGIN TRY
	


	select a.id as yhnumi ,a.ProductoId as yhprod ,a.PrecioCategoriaId  as yhcatpre,c.Codigo  as ygcod   ,a.precio as yhprecio ,1 as estado
from Precios  as a inner join Productos  as b
on b.Id  =a.ProductoId  and a.AlmacenId  =@almacen   --> Almacen
inner join PreciosCategorias as c on c.Id  =a.PrecioCategoriaId  
group by a.Id  ,a.PrecioCategoriaId  ,c.Codigo  ,a.Precio  ,a.ProductoId  
union
select 0 as yhnumi, c.id as yfnumi ,a.id as ygnumi ,a.Codigo  as ygcod  ,cast(0 as decimal(18,2)) as precio ,3 as estado
from PreciosCategorias  as a ,Productos  as c where a.Id  not in(
select b.PrecioCategoriaId  from Precios  as b where b.ProductoId  =c.Id and b.AlmacenId  =@almacen )
group by c.Id  ,a.id ,a.Codigo 
		
		order by yhprod 
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario)
		END CATCH

END
	
	IF @tipo=6 --MOSTRAR TODos los productos
	BEGIN
		BEGIN TRY
	
	select a.id as yfcprod,a.id as yfnumi,a.NombreProducto as yfcdprod1,
	c.NombreCategoria ,Proveedor .NombreProveedor as proveedor
	from Productos as a
	inner join Categorias as c on c.Id =a.CategoriaId
	inner join Proveedor on  Proveedor.id=a.ProveedorId 

	order by a.Id asc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario)
		END CATCH

END

	IF @tipo=7 --MOSTRAR TODAS LAS Categorias
	BEGIN
		BEGIN TRY
	


		select a.Id as ygnumi,a.Codigo as ygcod,a.Descripcion as ygdesc
		from PreciosCategorias as a
		order by a.Codigo asc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario)
		END CATCH

END
IF @tipo=8 --Listar Estado de Pedidos
	BEGIN
		BEGIN TRY
	


	select  *
	from PedidosEstados
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario)
		END CATCH

END


IF @tipo=9 --MOSTRAR Precios
	BEGIN
	   SET ARITHABORT ON
		BEGIN TRY
	
	select a.Id as ygnumi,a.Codigo as ygcod,a.Descripcion as ygdesc
		from PreciosCategorias as a where a.Tipo=1
		order by a.Codigo asc
		
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario)
		END CATCH

END
IF @tipo=10 --MOSTRAR Precios
	BEGIN
	  SET ARITHABORT ON
		BEGIN TRY
	
	select a.Id as ygnumi,a.Descripcion as Nombre
		from PreciosCategorias as a
		order by a.Codigo asc
		
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario)
		END CATCH

END
IF @tipo=11 --MOSTRAR Categorias
	BEGIN
	  SET ARITHABORT ON
		BEGIN TRY
	

select a.Id,a.NombreCategoria as Nombre,cast(1 as bit ) seleccionado
 from Categorias as a where a.Estado=1
 order by a.NombreCategoria asc
		
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario)
		END CATCH

END
End



GO
SET NOEXEC OFF
GO
IF CHARINDEX(N'select b.PrecioCategoriaId  from Precios  as b where b.ProductoId  =c.Id and b.AlmacenId  =@almacen )', OBJECT_DEFINITION(OBJECT_ID('dbo.sp_Mam_Precios'))) > 0
	RAISERROR('03 sp_Mam_Precios CORREGIDO.', 10, 1) WITH NOWAIT
GO
