/* =====================================================================
   07 - AUDITORIA DE REGISTROS (tabla + SP + programa de la web)

   1. Tabla AuditoriaRegistros: quien, cuando, desde que equipo y aplicacion se MODIFICO o ELIMINO una
      venta, compra o movimiento de inventario (ingreso / salida / traspaso), con la foto ANTES y DESPUES
      (cabecera y detalle, en XML). La tabla es de solo lectura: no se puede modificar ni borrar (trigger).
   2. SP MAM_Auditoria: @tipo 1 = foto del documento, @tipo 2 = registrar (lo llaman MAM_Ventas,
      MAM_Compras y MAM_Movimientos desde el script 08), @tipo 3 = listar, @tipo 4 = detalle, @tipo 5 = usuarios.
      Si la auditoria falla, NUNCA impide grabar: el error queda en la Bitacora.
   3. Programa de la web "Auditoria De Registros" (btnAuditoriaRegistros) en el modulo de Gestion de Usuarios,
      asignado al rol 1 (Administrador). A otros roles se asigna en Roles de Usuario.

   Sirve para CUALQUIER BD tal cual (no depende de la version de los SP). Se puede ejecutar varias veces:
   lo que ya existe se conserva (la tabla y su historial nunca se borran). Va ANTES del script 08.
   VOLVER ATRAS: revertir\REVERTIR_07_Auditoria.sql (SOLO para deshacer, despues de REVERTIR_08).
   ===================================================================== */
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- 1. Tabla -------------------------------------------------------------
IF OBJECT_ID('dbo.AuditoriaRegistros', 'U') IS NULL
BEGIN
	CREATE TABLE dbo.AuditoriaRegistros (
		Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditoriaRegistros PRIMARY KEY,
		Fecha datetime NOT NULL CONSTRAINT DF_AuditoriaRegistros_Fecha DEFAULT (GETDATE()),
		Modulo int NOT NULL,            -- 1 Venta, 2 Compra, 3 Movimiento de inventario
		Operacion int NOT NULL,         -- 1 Modificado, 2 Eliminado
		DocumentoId int NOT NULL,       -- Id de la venta / compra / movimiento
		FechaDocumento date NULL,
		Descripcion nvarchar(300) NULL, -- cliente, proveedor o concepto y deposito
		TotalAntes decimal(18,2) NULL,  -- ventas y compras: Bs ; movimientos: cantidad de productos
		TotalDespues decimal(18,2) NULL,
		Usuario nvarchar(100) NULL,     -- @usuario que recibe el SP (el que hizo la operacion)
		Equipo nvarchar(128) NULL,      -- HOST_NAME()
		Aplicacion nvarchar(128) NULL,  -- APP_NAME(): MiventSoftWeb = web ; otro = escritorio
		Antes xml NULL,
		Despues xml NULL,
		CONSTRAINT CK_AuditoriaRegistros_Modulo CHECK (Modulo IN (1, 2, 3)),
		CONSTRAINT CK_AuditoriaRegistros_Operacion CHECK (Operacion IN (1, 2))
	)
	CREATE NONCLUSTERED INDEX IX_AuditoriaRegistros_Fecha ON dbo.AuditoriaRegistros (Fecha) INCLUDE (Modulo, Operacion, DocumentoId, Usuario)
	CREATE NONCLUSTERED INDEX IX_AuditoriaRegistros_Documento ON dbo.AuditoriaRegistros (Modulo, DocumentoId)
	RAISERROR('07: tabla AuditoriaRegistros creada.', 10, 1) WITH NOWAIT
END
ELSE
	RAISERROR('07: la tabla AuditoriaRegistros ya existia: se conserva con su historial.', 10, 1) WITH NOWAIT
GO
-- 2. Solo lectura: nadie modifica ni borra la auditoria desde el sistema ----
IF OBJECT_ID('dbo.TR_AuditoriaRegistros_SoloLectura', 'TR') IS NULL
	EXEC('CREATE TRIGGER dbo.TR_AuditoriaRegistros_SoloLectura ON dbo.AuditoriaRegistros INSTEAD OF UPDATE, DELETE
AS
BEGIN
	RAISERROR(''La auditoria de registros no se puede modificar ni borrar.'', 16, 1)
END')
GO
-- 3. SP ----------------------------------------------------------------
IF OBJECT_ID('dbo.MAM_Auditoria', 'P') IS NULL
	EXEC('CREATE PROCEDURE dbo.MAM_Auditoria AS RETURN')
GO
-- si ya tiene el 10 (equipo, IP, ubicacion) no se vuelve a esta version
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Auditoria')) LIKE '%Kaily: auditoria equipo%'
BEGIN
	RAISERROR('07: MAM_Auditoria ya tiene el 10 (equipo y ubicacion): se conserva.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
GO
ALTER PROCEDURE [dbo].[MAM_Auditoria](@tipo int, @Id int=-1, @Modulo int=-1, @Operacion int=-1, @DocumentoId int=-1,
@Antes xml=NULL, @Foto xml=NULL OUTPUT, @Desde date=NULL, @Hasta date=NULL, @usuario nvarchar(100)='', @UsuarioFiltro nvarchar(100)='')
AS
BEGIN
	SET NOCOUNT ON
	SET ARITHABORT ON
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()

	IF @tipo=1 --FOTO DEL DOCUMENTO (cabecera + detalle). Devuelve @Foto, no devuelve filas
	BEGIN
		-- El detalle va ordenado por producto: si se graba sin cambiar nada, el antes y el despues son iguales
		IF @Modulo=1 --Venta
			SET @Foto=(SELECT
				(SELECT v.Id, v.FechaVenta AS Fecha, v.SucursalId, al.NombreAlmacen AS Sucursal, v.ClienteId, c.NombreCliente AS Cliente,
						v.PersonalId, p.NombrePersonal AS Personal, IIF(v.TipoVenta=1,'Contado','Credito') AS TipoVenta,
						v.FechaVencimientoCredito AS VencimientoCredito, v.Glosa, v.Descuento, v.TotalVenta AS Total, v.Facturado, v.Estado,
						'Cliente: ' + ISNULL(c.NombreCliente,'') AS Descripcion
				   FROM Ventas AS v LEFT JOIN Almacenes AS al ON al.id=v.SucursalId
				   LEFT JOIN Clientes AS c ON c.Id=v.ClienteId LEFT JOIN Personal AS p ON p.Id=v.PersonalId
				  WHERE v.Id=@DocumentoId
				  FOR XML RAW('cab'), TYPE),
				(SELECT d.ProductoId, pr.NombreProducto AS Producto, d.Cantidad, d.Precio, d.MontoDescuento AS Descuento, d.Total,
						d.Lote, d.FechaVencimiento
				   FROM VentasDetalles AS d LEFT JOIN Productos AS pr ON pr.Id=d.ProductoId
				  WHERE d.VentaId=@DocumentoId
				  ORDER BY d.ProductoId, d.Lote, d.Cantidad, d.Precio
				  FOR XML RAW('det'), TYPE)
			FOR XML PATH('foto'), TYPE)

		IF @Modulo=2 --Compra
			SET @Foto=(SELECT
				(SELECT co.Id, co.FechaTransaccion AS Fecha, co.AlmacenId, al.NombreAlmacen AS Almacen, co.ProveedorId, pv.NombreProveedor AS Proveedor,
						IIF(co.TipoVenta=1,'Contado','Credito') AS TipoCompra, co.FechaVencimientoCredito AS VencimientoCredito,
						co.Glosa, co.Descuento, co.TotalCompra AS Total, co.Estado,
						'Proveedor: ' + ISNULL(pv.NombreProveedor,'') AS Descripcion
				   FROM Compras AS co LEFT JOIN Almacenes AS al ON al.id=co.AlmacenId LEFT JOIN Proveedor AS pv ON pv.Id=co.ProveedorId
				  WHERE co.Id=@DocumentoId
				  FOR XML RAW('cab'), TYPE),
				(SELECT d.ProductoId, pr.NombreProducto AS Producto, d.Cantidad, d.PrecioCosto, d.PrecioVenta, d.TotalCompra AS Total,
						d.Lote, d.FechaVencimiento
				   FROM CompraDetalle AS d LEFT JOIN Productos AS pr ON pr.Id=d.ProductoId
				  WHERE d.CompraId=@DocumentoId
				  ORDER BY d.ProductoId, d.Lote, d.Cantidad, d.PrecioCosto
				  FOR XML RAW('det'), TYPE)
			FOR XML PATH('foto'), TYPE)

		IF @Modulo=3 --Movimiento de inventario (ingreso, salida, traspaso)
			SET @Foto=(SELECT
				(SELECT m.Id, m.FechaDocumento AS Fecha, m.ConceptoId, t.Descripcion AS Concepto, m.DepositoId, de.NombreDeposito AS Deposito,
						NULLIF(m.DepositoIdDestino, 0) AS DepositoIdDestino, dd.NombreDeposito AS DepositoDestino,
						NULLIF(m.IdMovimientoDestino, 0) AS IdMovimientoDestino, m.Observacion, m.Estado,
						(SELECT SUM(x.Cantidad) FROM MovimientosDetalle AS x WHERE x.MovimientoId=m.Id) AS Total,
						ISNULL(t.Descripcion,'') + ' - ' + ISNULL(de.NombreDeposito,'') + ISNULL(' -> ' + dd.NombreDeposito,'') AS Descripcion
				   FROM Movimientos AS m LEFT JOIN MovimientosTipos AS t ON t.Id=m.ConceptoId
				   LEFT JOIN Depositos AS de ON de.id=m.DepositoId LEFT JOIN Depositos AS dd ON dd.id=m.DepositoIdDestino
				  WHERE m.Id=@DocumentoId
				  FOR XML RAW('cab'), TYPE),
				(SELECT d.ProductoId, pr.NombreProducto AS Producto, d.Cantidad, d.Lote, d.FechaVencimiento
				   FROM MovimientosDetalle AS d LEFT JOIN Productos AS pr ON pr.Id=d.ProductoId
				  WHERE d.MovimientoId=@DocumentoId
				  ORDER BY d.ProductoId, d.Lote, d.Cantidad
				  FOR XML RAW('det'), TYPE)
			FOR XML PATH('foto'), TYPE)
	END

	IF @tipo=2 --REGISTRAR modificacion (@Operacion 1) o eliminacion (@Operacion 2). No devuelve filas
	BEGIN
		BEGIN TRY
			DECLARE @Despues xml
			IF @Operacion=1
				EXEC MAM_Auditoria @tipo=1, @Modulo=@Modulo, @DocumentoId=@DocumentoId, @Foto=@Despues OUTPUT
			-- se grabo sin cambiar nada: no se registra
			IF @Operacion=1 AND ISNULL(CAST(@Antes AS nvarchar(max)), '') = ISNULL(CAST(@Despues AS nvarchar(max)), '')
				RETURN

			INSERT INTO AuditoriaRegistros (Fecha, Modulo, Operacion, DocumentoId, FechaDocumento, Descripcion, TotalAntes, TotalDespues,
											Usuario, Equipo, Aplicacion, Antes, Despues)
			VALUES (GETDATE(), @Modulo, @Operacion, @DocumentoId,
					ISNULL(@Antes.value('(/foto/cab/@Fecha)[1]', 'date'), @Despues.value('(/foto/cab/@Fecha)[1]', 'date')),
					ISNULL(@Despues.value('(/foto/cab/@Descripcion)[1]', 'nvarchar(300)'), @Antes.value('(/foto/cab/@Descripcion)[1]', 'nvarchar(300)')),
					@Antes.value('(/foto/cab/@Total)[1]', 'decimal(18,2)'), @Despues.value('(/foto/cab/@Total)[1]', 'decimal(18,2)'),
					LEFT(@usuario, 100), LEFT(HOST_NAME(), 128), LEFT(APP_NAME(), 128), @Antes, @Despues)
		END TRY
		BEGIN CATCH
			-- la auditoria nunca impide grabar la venta / compra / movimiento
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),LEFT(ERROR_MESSAGE(),400),2,@newFecha,@newHora,LEFT(@usuario,100))
		END CATCH
	END

	IF @tipo=3 --LISTAR por fecha de la operacion. @Modulo, @Operacion, @DocumentoId = -1: todos ; @UsuarioFiltro = '': todos
	BEGIN
		BEGIN TRY
			SELECT a.Id, a.Fecha, a.Modulo, a.Operacion, a.DocumentoId, a.FechaDocumento, a.Descripcion, a.TotalAntes, a.TotalDespues,
				   a.Usuario, a.Equipo, a.Aplicacion
			FROM AuditoriaRegistros AS a
			WHERE a.Fecha >= @Desde AND a.Fecha < DATEADD(DAY, 1, @Hasta)
			  AND (@Modulo = -1 OR a.Modulo = @Modulo)
			  AND (@Operacion = -1 OR a.Operacion = @Operacion)
			  AND (@DocumentoId = -1 OR a.DocumentoId = @DocumentoId)
			  AND (@UsuarioFiltro = '' OR a.Usuario = @UsuarioFiltro)
			ORDER BY a.Fecha DESC, a.Id DESC
			OPTION (RECOMPILE)
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),LEFT(ERROR_MESSAGE(),400),3,@newFecha,@newHora,LEFT(@usuario,100))
		END CATCH
	END

	IF @tipo=4 --DETALLE: fotos antes y despues de un registro
	BEGIN
		SELECT a.Id, a.Fecha, a.Modulo, a.Operacion, a.DocumentoId, a.FechaDocumento, a.Descripcion, a.TotalAntes, a.TotalDespues,
			   a.Usuario, a.Equipo, a.Aplicacion,
			   CAST(a.Antes AS nvarchar(max)) AS Antes, CAST(a.Despues AS nvarchar(max)) AS Despues
		FROM AuditoriaRegistros AS a
		WHERE a.Id=@Id
	END

	IF @tipo=5 --USUARIOS que tienen registros (filtro de la pantalla)
	BEGIN
		SELECT DISTINCT a.Usuario FROM AuditoriaRegistros AS a WHERE ISNULL(a.Usuario,'') <> '' ORDER BY a.Usuario
	END
END
GO
SET NOEXEC OFF
GO
-- 4. Programa de la web (modulo de Gestion De Usuarios), asignado al rol Administrador (1) --------------
IF NOT EXISTS (SELECT 1 FROM Programas WHERE IdPrograma = 'btnAuditoriaRegistros')
BEGIN
	DECLARE @IdProg int, @ModuloProg int
	SET @ModuloProg = ISNULL((SELECT TOP 1 Modulo FROM Programas WHERE IdPrograma = 'btConfUsuarios'), 1)
	IF COLUMNPROPERTY(OBJECT_ID('dbo.Programas'), 'Id', 'IsIdentity') = 1
	BEGIN
		INSERT INTO Programas (IdPrograma, DescripcionPrograma, Modulo) VALUES ('btnAuditoriaRegistros', 'AUDITORIA DE REGISTROS', @ModuloProg)
		SET @IdProg = SCOPE_IDENTITY()
	END
	ELSE
	BEGIN
		SET @IdProg = ISNULL((SELECT MAX(Id) FROM Programas), 0) + 1
		INSERT INTO Programas (Id, IdPrograma, DescripcionPrograma, Modulo) VALUES (@IdProg, 'btnAuditoriaRegistros', 'AUDITORIA DE REGISTROS', @ModuloProg)
	END
	-- una fila por rol (asi aparece en Roles de Usuario); solo el Administrador lo ve
	IF COLUMNPROPERTY(OBJECT_ID('dbo.RolesDetalles'), 'Id', 'IsIdentity') = 1
		INSERT INTO RolesDetalles (RolId, ProgramaId, Ver, Insertar, Modificar, Eliminar)
		SELECT r.Id, @IdProg, IIF(r.Id = 1, 1, 0), 0, 0, 0 FROM Roles AS r
	ELSE
		INSERT INTO RolesDetalles (Id, RolId, ProgramaId, Ver, Insertar, Modificar, Eliminar)
		SELECT ISNULL((SELECT MAX(Id) FROM RolesDetalles), 0) + ROW_NUMBER() OVER (ORDER BY r.Id), r.Id, @IdProg, IIF(r.Id = 1, 1, 0), 0, 0, 0
		FROM Roles AS r
	RAISERROR('07: programa "Auditoria De Registros" creado y asignado al rol Administrador.', 10, 1) WITH NOWAIT
END
ELSE
	RAISERROR('07: el programa "Auditoria De Registros" ya existia.', 10, 1) WITH NOWAIT
GO
RAISERROR('07_Auditoria.sql TERMINADO. Siguiente: 08_Auditoria_SPs.sql', 10, 1) WITH NOWAIT
GO
