/* =====================================================================
   sp_Mam_Precios @tipo 5 (Tec_Precios: matriz producto x categoria)
   CAMBIA UNA SOLA LINEA (resto del SP identico al de la BD):
     antes:  select b.PrecioCategoriaId  from Precios  as b inner join Productos  as e on b.ProductoId  =c.Id and b.AlmacenId  =@almacen )
     ahora:  select b.PrecioCategoriaId  from Precios  as b where b.ProductoId  =c.Id and b.AlmacenId  =@almacen )
   El "inner join Productos as e" no tenia condicion sobre e: cruzaba cada precio con
   los 2625 productos. Mismo resultado (verificado: 7875 filas, misma huella).

   Medido en KailiIndustrialLaPaz (2026-11-12), plan compilado como lo hace la pantalla
   (primero @tipo 4 sin @almacen -> @almacen = -1):
     original: 43 s (y timeout de 60 s en la web)   corregido: 0.17 s

   PROTECCION: solo se aplica si el SP es EXACTAMENTE la version leida de esta BD
   (otra rama/BD con otro sp_Mam_Precios -> error y no cambia nada).
   VOLVER ATRAS: cambiar la linea "ahora" por la linea "antes" y ejecutar el ALTER.
   ===================================================================== */
IF ISNULL(HASHBYTES('SHA2_256', OBJECT_DEFINITION(OBJECT_ID('dbo.sp_Mam_Precios'))), 0x) <> 0x9769803D13E0DDEB7962499A8D6F00046362F9D0EE09B38CBBFD5B74F73FCE16
BEGIN
	RAISERROR('sp_Mam_Precios no es la version esperada: ya fue corregido o es de otra BD. No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
GO
--drop procedure sp_Mam_TY006
ALTER PROCEDURE [dbo].[sp_Mam_Precios] (@tipo int,@Id int=-1,@Codigo nvarchar(50)='',@Descripcion nvarchar(80)=''
,@TipoCategoria int=-1,@TY007 TY007Type Readonly,
@usuario nvarchar(100)='',@almacen int=-1,@categoria int=-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()
	

	IF @tipo=-1--Eliminar  Categoria
	BEGIN
	  SET ARITHABORT ON
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
	  SET ARITHABORT ON
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
	  SET ARITHABORT ON
		BEGIN TRY 
				 ----------MODIFICO EL DETALLE DE EQUIPO------------
			--INSERTO LOS NUEVOS

			--INSERT INTO Precios(AlmacenId  ,ProductoId   ,PrecioCategoriaId   ,Precio    ,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro )
			--SELECT @almacen ,td.yhprod ,td.yhcatpre ,td.yhprecio 
			--,@newFecha ,@newHora,@usuario   FROM @TY007 AS td
			--where td.estado =3

			----MODIFICO LOS REGISTROS
			--UPDATE Precios  
			--SET Precio  =td.yhprecio 
			--FROM Precios   INNER JOIN @TY007 AS td
			--ON Precios .Id     = td.yhnumi   and td.estado=2;
			--select @Id as newNumi

			-- ===============================================
--  INSERTA NUEVOS REGISTROS (estado = 3)
--  Y LOS GUARDA EN Precios_Log
-- ===============================================

/* =========================
   STAGING para el OUTPUT
   =========================*/
DECLARE @PreciosLog TABLE(
    PreciosId              INT,
    Accion                 CHAR(1),
    AlmacenId_Old          INT NULL, AlmacenId_New INT NULL,
    ProductoId_Old         INT NULL, ProductoId_New INT NULL,
    PrecioCategoriaId_Old  INT NULL, PrecioCategoriaId_New INT NULL,
    Precio_Old             DECIMAL(18,2) NULL, Precio_New DECIMAL(18,2) NULL,
    FechaRegistro_Old      DATE NULL,  FechaRegistro_New DATE NULL,
    HoraRegistro_Old       NVARCHAR(15) NULL, HoraRegistro_New NVARCHAR(15) NULL,
    UsuarioRegistro_Old    NVARCHAR(50) NULL, UsuarioRegistro_New NVARCHAR(50) NULL,
    UsuarioEjecutor        NVARCHAR(128),
    FechaHoraUtc           DATETIME2(3),
    HostName               NVARCHAR(128),
    AppName                NVARCHAR(128),
    Spid                   INT,
    Extra                  NVARCHAR(MAX) NULL
);

/* ========== INSERT ========== */
INSERT INTO dbo.Precios(AlmacenId,ProductoId,PrecioCategoriaId,Precio,FechaRegistro,HoraRegistro,UsuarioRegistro)
OUTPUT
    inserted.Id,
    'I',
    NULL, inserted.AlmacenId,
    NULL, inserted.ProductoId,
    NULL, inserted.PrecioCategoriaId,
    NULL, inserted.Precio,
    NULL, inserted.FechaRegistro,
    NULL, inserted.HoraRegistro,
    NULL, inserted.UsuarioRegistro,
    COALESCE(CONVERT(nvarchar(128), SESSION_CONTEXT(N'app_user')), SUSER_SNAME()),
    SYSUTCDATETIME(),
    HOST_NAME(),
    APP_NAME(),
    @@SPID,
    NULL
INTO @PreciosLog(
    PreciosId, Accion,
    AlmacenId_Old, AlmacenId_New,
    ProductoId_Old, ProductoId_New,
    PrecioCategoriaId_Old, PrecioCategoriaId_New,
    Precio_Old, Precio_New,
    FechaRegistro_Old, FechaRegistro_New,
    HoraRegistro_Old, HoraRegistro_New,
    UsuarioRegistro_Old, UsuarioRegistro_New,
    UsuarioEjecutor, FechaHoraUtc, HostName, AppName, Spid, Extra
)
SELECT @almacen, td.yhprod, td.yhcatpre, td.yhprecio, @newFecha, @newHora, @usuario
FROM @TY007 AS td
WHERE td.estado = 3;

/* ========== UPDATE ========== */
UPDATE p
SET p.Precio = td.yhprecio
OUTPUT
    inserted.Id,
    'U',
    deleted.AlmacenId, inserted.AlmacenId,
    deleted.ProductoId, inserted.ProductoId,
    deleted.PrecioCategoriaId, inserted.PrecioCategoriaId,
    deleted.Precio, inserted.Precio,
    deleted.FechaRegistro, inserted.FechaRegistro,
    deleted.HoraRegistro, inserted.HoraRegistro,
    deleted.UsuarioRegistro, inserted.UsuarioRegistro,
    COALESCE(CONVERT(nvarchar(128), SESSION_CONTEXT(N'app_user')), @usuario),
    DATEADD(HOUR, -4, SYSUTCDATETIME()),
    HOST_NAME(),
    APP_NAME(),
    @@SPID,
    NULL
INTO @PreciosLog(
    PreciosId, Accion,
    AlmacenId_Old, AlmacenId_New,
    ProductoId_Old, ProductoId_New,
    PrecioCategoriaId_Old, PrecioCategoriaId_New,
    Precio_Old, Precio_New,
    FechaRegistro_Old, FechaRegistro_New,
    HoraRegistro_Old, HoraRegistro_New,
    UsuarioRegistro_Old, UsuarioRegistro_New,
    UsuarioEjecutor, FechaHoraUtc, HostName, AppName, Spid, Extra
)
FROM dbo.Precios AS p
JOIN @TY007 AS td
  ON p.Id = td.yhnumi
WHERE td.estado = 2;

/* ========== VOLCADO FINAL A dbo.Precios_Log ==========
   (No enviamos TxGuid; dejar que el DEFAULT NEWSEQUENTIALID() lo genere)
*/
INSERT INTO dbo.Precios_Log(
    PreciosId, Accion,
    AlmacenId_Old, AlmacenId_New,
    ProductoId_Old, ProductoId_New,
    PrecioCategoriaId_Old, PrecioCategoriaId_New,
    Precio_Old, Precio_New,
    FechaRegistro_Old, FechaRegistro_New,
    HoraRegistro_Old, HoraRegistro_New,
    UsuarioRegistro_Old, UsuarioRegistro_New,
    UsuarioEjecutor, FechaHoraUtc, HostName, AppName, Spid, Extra
)
SELECT
    PreciosId, Accion,
    AlmacenId_Old, AlmacenId_New,
    ProductoId_Old, ProductoId_New,
    PrecioCategoriaId_Old, PrecioCategoriaId_New,
    Precio_Old, Precio_New,
    FechaRegistro_Old, FechaRegistro_New,
    HoraRegistro_Old, HoraRegistro_New,
    UsuarioRegistro_Old, UsuarioRegistro_New,
    @usuario, FechaHoraUtc, HostName, AppName, Spid, Extra
FROM @PreciosLog;

SELECT @Id AS newNumi;


				


			
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@usuario)
		END CATCH

	END

	IF @tipo=3 --MOSTRAR TODAS LAS CATEGORIAS
	BEGIN
	  SET ARITHABORT ON
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
	  SET ARITHABORT ON
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
		   SET ARITHABORT ON
		BEGIN TRY
	


	select a.id as yhnumi ,a.ProductoId as yhprod ,a.PrecioCategoriaId  as yhcatpre,c.Codigo  as ygcod   ,a.precio as yhprecio ,1 as estado
from Precios  as a inner join Productos  as b
on b.Id  =a.ProductoId  and a.AlmacenId  =@almacen   --> Almacen
inner join PreciosCategorias as c on c.Id  =a.PrecioCategoriaId  
group by a.Id  ,a.PrecioCategoriaId  ,c.Codigo  ,a.Precio  ,a.ProductoId  
union
select 0 as yhnumi, c.id as yfnumi ,a.id as ygnumi ,a.Codigo  as ygcod  ,isnull((select top 1 pp.Precio   from Precios as pp where pp.ProductoId =c.Id and pp.AlmacenId =@almacen and pp.PrecioCategoriaId =1 ),0) as precio ,3 as estado
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
	   SET ARITHABORT ON
		BEGIN TRY
	
	select a.CodigoExterno  as yfcprod,a.id as yfnumi,a.NombreProducto as yfcdprod1,
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
	   SET ARITHABORT ON
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

IF @tipo=8 --MOSTRAR Precios
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
IF @tipo=9 --MOSTRAR Precios
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
IF @tipo=10 --MOSTRAR Categorias
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
