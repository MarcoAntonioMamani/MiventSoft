/* =====================================================================
   SOLO PARA DESHACER 06_EliminarRegistros.sql. NO es parte de la instalacion: NO ejecutar al instalar.
   Vuelve MAM_Usuarios de KailiIndustrialLaPaz a la version anterior y QUITA la columna Usuarios.EliminarRegistros
   (el SP anterior inserta sin lista de columnas y no funciona con la columna de mas).
   Antes: volver a la version anterior de la web y del escritorio. Si no esta instalado, avisa y no hace nada.
   ===================================================================== */
SET NOEXEC OFF
GO
IF NOT (OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Usuarios')) LIKE '%Kaily: eliminar registros%')
BEGIN
	RAISERROR('EliminarRegistros NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
GO
SET XACT_ABORT ON
BEGIN TRAN
IF COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NOT NULL
	EXEC('ALTER TABLE dbo.Usuarios DROP COLUMN EliminarRegistros')
GO
ALTER PROCEDURE [dbo].[MAM_Usuarios](@tipo int,@Id int=-1,@RolId int=0,@NombreUsuario nvarchar(200)='',
@Contrasena nvarchar(300)='',@Estado int=0,@SucursalId int=-1,@IdEmpresa int=-1,@usuario nvarchar(50)
,@IdPersonal int=0,@ModificarPrecio int=-1,@ModificarDescuento int=-1,@VentasTodasVentas int =-1,@VerTodasVentasCierre int=-1, @ModificarRegistroAntiguos int=-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()
	
	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		BEGIN TRY
			delete from Usuarios   where Id=@Id;
			SELECT 1 as resp;
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@usuario)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY 
		
			
			INSERT INTO Usuarios  VALUES(@RolId ,@NombreUsuario ,@Contrasena ,@Estado ,@SucursalId ,@newFecha
			,@newHora,@usuario,@IdEmpresa ,@IdPersonal,@ModificarPrecio,@ModificarDescuento,@VentasTodasVentas,@VerTodasVentasCierre,@ModificarRegistroAntiguos )
			
			

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
		BEGIN TRAN MODIFICACION
		BEGIN TRY 
			
			UPDATE Usuarios  SET RolId  =@RolId ,NombreUsuario =@NombreUsuario ,Contrasena =@Contrasena ,Estado =@Estado ,SucursalId =@SucursalId  ,
							 FechaRegistro =@newFecha,HoraRegistro =@newHora,UsuarioRegistro =@usuario ,IdEmpresa =@IdEmpresa ,IdPersonal =@IdPersonal,
							 ModificarPrecioVenta =@ModificarPrecio,AplicarDescuentoVenta =@ModificarDescuento,VerTodasVentas=@VentasTodasVentas,
							 VerTodosVentasCierre=@VerTodasVentasCierre,ModificarRegistroAnteriores=@ModificarRegistroAntiguos
					 Where Id = @Id 
		
			--DEVUELVO VALORES DE CONFIRMACION
			select @Id as newNumi
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@usuario )
			ROLLBACK TRAN
		END CATCH
	END

	
	IF @tipo=3--traer cabecera
	BEGIN
		BEGIN TRY
		
		select u.Id,u.NombreUsuario ,u.Contrasena ,cast(u.Estado as bit)  as estado,u.RolId,r.NombreRol ,u.SucursalId,isnull(al.NombreAlmacen,'Todos') as NombreAlmacen ,u.IdEmpresa,em.Nombre as Empresa,
		u.IdPersonal, per.NombrePersonal as Personal,isnull(u.ModificarPrecioVenta,0)as ModificarPrecioVenta,
		isnull(u.AplicarDescuentoVenta,0) as AplicarDescuentoVenta,isnull(u.VerTodasVentas,0) as VerTodasVentas,isnull(u.VerTodosVentasCierre,0) as VerTodosVentasCierre,
		isnull(u.ModificarRegistroAnteriores,0) as ModificarRegistroAnteriores
		from Usuarios as u inner join Roles as r on u.RolId =r.Id 
		inner join Empresa as em on em.Id =u.IdEmpresa 
		inner join Personal as per on per.Id =u.IdPersonal 
		left join Almacenes as al on al.id =u.SucursalId 
		order by u.id desc
			

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH
	END

	
	IF @tipo=4--Listar Empresas
	BEGIN
		BEGIN TRY
		 
		  select Id ,Nombre 
		   from Empresa 
		   order by id asc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH
	END
		IF @tipo=5--Listar Roles
	BEGIN
		BEGIN TRY
		 
		  select Id ,NombreRol  
		   from Roles  
		   order by id asc
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
	IF NOT (OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Usuarios')) LIKE '%Kaily: eliminar registros%') AND COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NULL
	BEGIN COMMIT TRAN; RAISERROR('EliminarRegistros DESHECHO.', 10, 1) WITH NOWAIT END
	ELSE
	BEGIN ROLLBACK TRAN; RAISERROR('No se pudo deshacer EliminarRegistros. No se modifico nada.', 16, 1) END
END
GO
SET NOEXEC OFF
GO
