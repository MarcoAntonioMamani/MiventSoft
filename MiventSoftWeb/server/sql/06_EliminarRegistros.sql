/* =====================================================================
   06 - PUEDE ELIMINAR REGISTROS (permiso por usuario; web y escritorio usan el mismo SP)

   1. Usuarios.EliminarRegistros (int): 1 = puede eliminar ventas, compras, movimientos de inventario
      (ingresos / salidas / traspasos) y pagos de cobranzas y de cuentas por pagar.
   2. Los usuarios que YA existen quedan con 0 (SIN el permiso). Despues, en Usuarios,
      se le habilita a quien si debe eliminar. Los usuarios nuevos tambien empiezan SIN el permiso.
   3. MAM_Usuarios: @EliminarRegistros (-1 = no enviado). Al modificar desde un escritorio que todavia no
      tiene la casilla, conserva el valor actual (no lo borra). El INSERT ahora lleva lista de columnas.
   Todo lo demas del SP queda IDENTICO a la version de la BD.

   Generado desde KailiIndustrialLaPaz (2026-11-13). PROTECCION: solo se aplica si MAM_Usuarios es esa version (sin contar
   mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da error
   y no modifica nada. Columna y SP van en UNA transaccion: o queda todo o no queda nada.
   VOLVER ATRAS: revertir\REVERTIR_06_EliminarRegistros.sql (SOLO para deshacer).
   ===================================================================== */
SET NOEXEC OFF
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Usuarios')) LIKE '%Kaily: eliminar registros%'
BEGIN
	RAISERROR('EliminarRegistros YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
ELSE IF ISNULL(HASHBYTES('SHA2_256', REPLACE(REPLACE(REPLACE(REPLACE(UPPER(OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Usuarios'))), CHAR(13), ''), CHAR(10), ''), CHAR(9), ''), ' ', '')), 0x) <> 0x65FC7EAFF60FF5947C4FF971B69510E14E3C6CCD83B097E73207064FF0BB9703
BEGIN
	RAISERROR('EliminarRegistros: MAM_Usuarios es OTRA version (no la de KailiIndustrialLaPaz 2026-11-13). No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
GO
SET XACT_ABORT ON
BEGIN TRAN
IF COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NULL
	ALTER TABLE dbo.Usuarios ADD EliminarRegistros int NULL
GO
-- los que ya existen quedan SIN el permiso (se habilita a mano en Usuarios)
EXEC('UPDATE dbo.Usuarios SET EliminarRegistros = 0 WHERE EliminarRegistros IS NULL')
GO
ALTER PROCEDURE [dbo].[MAM_Usuarios](@tipo int,@Id int=-1,@RolId int=0,@NombreUsuario nvarchar(200)='',
@Contrasena nvarchar(300)='',@Estado int=0,@SucursalId int=-1,@IdEmpresa int=-1,@usuario nvarchar(50)
,@IdPersonal int=0,@ModificarPrecio int=-1,@ModificarDescuento int=-1,@VentasTodasVentas int =-1,@VerTodasVentasCierre int=-1, @ModificarRegistroAntiguos int=-1, @EliminarRegistros int=-1 /* Kaily: eliminar registros */)
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
		
			
			-- Kaily: eliminar registros: lista de columnas; si no se envia, no puede eliminar
			INSERT INTO Usuarios (RolId, NombreUsuario, Contrasena, Estado, SucursalId, FechaRegistro, HoraRegistro, UsuarioRegistro, IdEmpresa, IdPersonal, ModificarPrecioVenta, AplicarDescuentoVenta, VerTodasVentas, VerTodosVentasCierre, ModificarRegistroAnteriores, EliminarRegistros)
			VALUES(@RolId ,@NombreUsuario ,@Contrasena ,@Estado ,@SucursalId ,@newFecha
			,@newHora,@usuario,@IdEmpresa ,@IdPersonal,@ModificarPrecio,@ModificarDescuento,@VentasTodasVentas,@VerTodasVentasCierre,@ModificarRegistroAntiguos, IIF(@EliminarRegistros = 1, 1, 0))
			
			

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
							 VerTodosVentasCierre=@VerTodasVentasCierre,ModificarRegistroAnteriores=@ModificarRegistroAntiguos,
							 EliminarRegistros = IIF(@EliminarRegistros = -1, EliminarRegistros, @EliminarRegistros) /* Kaily: eliminar registros */
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
		isnull(u.ModificarRegistroAnteriores,0) as ModificarRegistroAnteriores,
		isnull(u.EliminarRegistros,0) as EliminarRegistros /* Kaily: eliminar registros */
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
	IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Usuarios')) LIKE '%Kaily: eliminar registros%' AND COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NOT NULL
	BEGIN COMMIT TRAN; RAISERROR('EliminarRegistros INSTALADO.', 10, 1) WITH NOWAIT END
	ELSE
	BEGIN ROLLBACK TRAN; RAISERROR('EliminarRegistros: fallo la instalacion. Se deshizo todo, no se modifico nada.', 16, 1) END
END
ELSE IF NOT (OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Usuarios')) LIKE '%Kaily: eliminar registros%') OR COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NULL
	RAISERROR('EliminarRegistros: fallo la instalacion (revisar los mensajes de arriba).', 16, 1)
GO
SET NOEXEC OFF
GO
