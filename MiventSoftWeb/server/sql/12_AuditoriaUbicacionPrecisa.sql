/* =====================================================================
   12 - AUDITORIA: UBICACION CON 18 DECIMALES

   Latitud y Longitud de la auditoria se guardaban con 6 decimales (decimal(9,6)).
   1. AuditoriaRegistros y AuditoriaContexto: Latitud y Longitud pasan a decimal(21,18) (18 decimales).
      Lo ya grabado se conserva igual (con ceros al final). Solo cambia el tipo de columna: la tabla sigue
      siendo de solo lectura (el trigger no se toca).
   2. MAM_Auditoria: guarda el valor tal como lo da el GPS del equipo y al listar devuelve ademas
      LatitudTexto / LongitudTexto (el valor exacto, sin redondear).
   No cambia MAM_Ventas, MAM_Compras ni MAM_Movimientos. Sirve para CUALQUIER BD tal cual (va despues del 11).
   Se puede ejecutar varias veces. VOLVER ATRAS: revertir\REVERTIR_12_AuditoriaUbicacionPrecisa.sql (SOLO para deshacer).
   ===================================================================== */
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET NOEXEC OFF
GO
IF NOT (OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Auditoria')) LIKE '%Kaily: auditoria usuario completo%')
BEGIN
	RAISERROR('12: falta ejecutar primero 11_AuditoriaUsuario.sql. No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Auditoria')) LIKE '%Kaily: auditoria ubicacion precisa%'
	AND COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaRegistros'), 'Latitud', 'Scale') = 18 AND COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaRegistros'), 'Longitud', 'Scale') = 18
	AND COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaContexto'), 'Latitud', 'Scale') = 18 AND COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaContexto'), 'Longitud', 'Scale') = 18
BEGIN
	RAISERROR('12 Auditoria ubicacion precisa YA ESTA INSTALADO. No se modifico nada.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
GO
-- 1. Columnas con 18 decimales -----------------------------------------
IF COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaRegistros'), 'Latitud', 'Scale') <> 18
	ALTER TABLE dbo.AuditoriaRegistros ALTER COLUMN Latitud decimal(21,18) NULL
GO
IF COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaRegistros'), 'Longitud', 'Scale') <> 18
	ALTER TABLE dbo.AuditoriaRegistros ALTER COLUMN Longitud decimal(21,18) NULL
GO
IF COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaContexto'), 'Latitud', 'Scale') <> 18
	ALTER TABLE dbo.AuditoriaContexto ALTER COLUMN Latitud decimal(21,18) NULL
GO
IF COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaContexto'), 'Longitud', 'Scale') <> 18
	ALTER TABLE dbo.AuditoriaContexto ALTER COLUMN Longitud decimal(21,18) NULL
GO
-- 2. SP ----------------------------------------------------------------
ALTER PROCEDURE [dbo].[MAM_Auditoria](@tipo int, @Id int=-1, @Modulo int=-1, @Operacion int=-1, @DocumentoId int=-1,
@Antes xml=NULL, @Foto xml=NULL OUTPUT, @Desde date=NULL, @Hasta date=NULL, @usuario nvarchar(100)='', @UsuarioFiltro nvarchar(100)='')
AS
BEGIN
	SET NOCOUNT ON
	SET ARITHABORT ON
	-- Kaily: auditoria equipo (10): IP, navegador, equipo y ubicacion de quien modifica o elimina
	-- Kaily: auditoria ubicacion precisa (12): latitud y longitud con 18 decimales (decimal(21,18)); LatitudTexto / LongitudTexto = valor exacto
	-- Kaily: auditoria usuario completo (11): MAM_Compras y MAM_Movimientos reciben @usuario de 10 letras; se guarda y se muestra el nombre completo
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

			-- Donde se hizo. Web: lo deja MiventSoftWeb en AuditoriaContexto en el mismo lote que llama al SP (misma sesion: @@SPID).
			-- Escritorio: nombre de la PC (HOST_NAME) e IP de la conexion.
			DECLARE @cIp nvarchar(64), @cNav nvarchar(200), @cEquipo nvarchar(128), @cLat decimal(21,18), @cLon decimal(21,18), @cPrec int, @cUsuario nvarchar(100)
			IF APP_NAME() LIKE '%MiventSoftWeb%'
				SELECT TOP 1 @cIp=c.Ip, @cNav=c.Navegador, @cEquipo=c.Equipo, @cLat=c.Latitud, @cLon=c.Longitud, @cPrec=c.PrecisionMetros, @cUsuario=c.Usuario
				FROM dbo.AuditoriaContexto AS c WHERE c.Spid=@@SPID AND c.Fecha >= DATEADD(MINUTE, -5, GETDATE())
			ELSE
				SELECT @cEquipo=HOST_NAME(), @cIp=CONVERT(nvarchar(64), CONNECTIONPROPERTY('client_net_address')), @cNav='Escritorio (' + APP_NAME() + ')'

			-- Usuario completo. Web: el que deja MiventSoftWeb (si empieza igual que @usuario).
			-- Escritorio: si @usuario vino cortado (10 letras) y hay UN solo usuario (o su personal) cuyo nombre empieza asi, ese nombre.
			DECLARE @uAud nvarchar(100) = @usuario
			IF ISNULL(@cUsuario, '') <> '' AND LEFT(@cUsuario, LEN(@usuario)) = @usuario
				SET @uAud = @cUsuario
			ELSE IF DATALENGTH(@usuario) = 20 AND NOT EXISTS (SELECT 1 FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS k WHERE k.n = @usuario)
				SELECT @uAud = ISNULL((SELECT MIN(k.n) FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS k WHERE LEFT(k.n, 10) = @usuario HAVING COUNT(DISTINCT k.n) = 1), @usuario)

			INSERT INTO AuditoriaRegistros (Fecha, Modulo, Operacion, DocumentoId, FechaDocumento, Descripcion, TotalAntes, TotalDespues,
											Usuario, Equipo, Aplicacion, Antes, Despues, Ip, Navegador, Latitud, Longitud, PrecisionMetros)
			VALUES (GETDATE(), @Modulo, @Operacion, @DocumentoId,
					ISNULL(@Antes.value('(/foto/cab/@Fecha)[1]', 'date'), @Despues.value('(/foto/cab/@Fecha)[1]', 'date')),
					ISNULL(@Despues.value('(/foto/cab/@Descripcion)[1]', 'nvarchar(300)'), @Antes.value('(/foto/cab/@Descripcion)[1]', 'nvarchar(300)')),
					@Antes.value('(/foto/cab/@Total)[1]', 'decimal(18,2)'), @Despues.value('(/foto/cab/@Total)[1]', 'decimal(18,2)'),
					LEFT(@uAud, 100), LEFT(ISNULL(@cEquipo, HOST_NAME()), 128), LEFT(APP_NAME(), 128), @Antes, @Despues,
					LEFT(@cIp, 64), LEFT(@cNav, 200), @cLat, @cLon, @cPrec)
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
				   u.Usuario, a.Equipo, a.Aplicacion, a.Ip, a.Navegador, a.Latitud, a.Longitud, a.PrecisionMetros,
			   CONVERT(varchar(40), a.Latitud) AS LatitudTexto, CONVERT(varchar(40), a.Longitud) AS LongitudTexto
			FROM AuditoriaRegistros AS a
			OUTER APPLY (SELECT Usuario = CASE WHEN DATALENGTH(a.Usuario) = 20 AND NOT EXISTS (SELECT 1 FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS k WHERE k.n = a.Usuario)
				THEN ISNULL((SELECT MIN(k.n) FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS k WHERE LEFT(k.n, 10) = a.Usuario HAVING COUNT(DISTINCT k.n) = 1), a.Usuario) ELSE a.Usuario END) AS u
			WHERE a.Fecha >= @Desde AND a.Fecha < DATEADD(DAY, 1, @Hasta)
			  AND (@Modulo = -1 OR a.Modulo = @Modulo)
			  AND (@Operacion = -1 OR a.Operacion = @Operacion)
			  AND (@DocumentoId = -1 OR a.DocumentoId = @DocumentoId)
			  AND (@UsuarioFiltro = '' OR u.Usuario = @UsuarioFiltro)
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
			   u.Usuario, a.Equipo, a.Aplicacion, a.Ip, a.Navegador, a.Latitud, a.Longitud, a.PrecisionMetros,
			   CONVERT(varchar(40), a.Latitud) AS LatitudTexto, CONVERT(varchar(40), a.Longitud) AS LongitudTexto,
			   CAST(a.Antes AS nvarchar(max)) AS Antes, CAST(a.Despues AS nvarchar(max)) AS Despues
		FROM AuditoriaRegistros AS a
		OUTER APPLY (SELECT Usuario = CASE WHEN DATALENGTH(a.Usuario) = 20 AND NOT EXISTS (SELECT 1 FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS k WHERE k.n = a.Usuario)
			THEN ISNULL((SELECT MIN(k.n) FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS k WHERE LEFT(k.n, 10) = a.Usuario HAVING COUNT(DISTINCT k.n) = 1), a.Usuario) ELSE a.Usuario END) AS u
		WHERE a.Id=@Id
	END

	IF @tipo=5 --USUARIOS que tienen registros (filtro de la pantalla)
	BEGIN
		SELECT DISTINCT u.Usuario FROM AuditoriaRegistros AS a
		OUTER APPLY (SELECT Usuario = CASE WHEN DATALENGTH(a.Usuario) = 20 AND NOT EXISTS (SELECT 1 FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS k WHERE k.n = a.Usuario)
			THEN ISNULL((SELECT MIN(k.n) FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS k WHERE LEFT(k.n, 10) = a.Usuario HAVING COUNT(DISTINCT k.n) = 1), a.Usuario) ELSE a.Usuario END) AS u
		WHERE ISNULL(a.Usuario,'') <> '' ORDER BY u.Usuario
	END
END
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Auditoria')) LIKE '%Kaily: auditoria ubicacion precisa%'
	AND COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaRegistros'), 'Latitud', 'Scale') = 18 AND COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaRegistros'), 'Longitud', 'Scale') = 18
	AND COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaContexto'), 'Latitud', 'Scale') = 18 AND COLUMNPROPERTY(OBJECT_ID('dbo.AuditoriaContexto'), 'Longitud', 'Scale') = 18
	RAISERROR('12 Auditoria ubicacion precisa INSTALADO.', 10, 1) WITH NOWAIT
ELSE
	RAISERROR('12: fallo la instalacion (revisar los mensajes de arriba).', 16, 1)
GO
SET NOEXEC OFF
GO
