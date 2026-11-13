/* =====================================================================
   SOLO PARA DESHACER 10_AuditoriaEquipo.sql. NO es parte de la instalacion: NO ejecutar al instalar.
   Vuelve MAM_Auditoria a la version del 07 y borra la tabla AuditoriaContexto.
   Las columnas Ip, Navegador, Latitud, Longitud y PrecisionMetros de AuditoriaRegistros SE CONSERVAN (con su historial).
   Antes volver a la version anterior de la web (la nueva escribe en AuditoriaContexto solo si existe).
   ===================================================================== */
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET NOEXEC OFF
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Auditoria')) LIKE '%Kaily: auditoria usuario completo%'
BEGIN
	RAISERROR('Primero deshacer el 11 (revertir\REVERTIR_11_AuditoriaUsuario.sql). No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
GO
IF NOT (OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Auditoria')) LIKE '%Kaily: auditoria equipo%') AND OBJECT_ID('dbo.AuditoriaContexto', 'U') IS NULL
BEGIN
	RAISERROR('10 Auditoria equipo NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT
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
IF OBJECT_ID('dbo.AuditoriaContexto', 'U') IS NOT NULL
	DROP TABLE dbo.AuditoriaContexto
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Auditoria')) LIKE '%Kaily: auditoria usuario completo%'
BEGIN
	RAISERROR('Primero deshacer el 11 (revertir\REVERTIR_11_AuditoriaUsuario.sql). No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
GO
IF NOT (OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Auditoria')) LIKE '%Kaily: auditoria equipo%') AND OBJECT_ID('dbo.AuditoriaContexto', 'U') IS NULL
	RAISERROR('10 Auditoria equipo DESHECHO.', 10, 1) WITH NOWAIT
ELSE
	RAISERROR('No se pudo deshacer 10.', 16, 1)
GO
SET NOEXEC OFF
GO
