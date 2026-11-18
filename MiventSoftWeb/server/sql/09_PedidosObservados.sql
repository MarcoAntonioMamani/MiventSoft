/* =====================================================================
   09_PedidosObservados.sql  -  Sanbit: pedidos del celular que NO pueden entrar (observados)
   ---------------------------------------------------------------------
   Que hace:
   1. Tabla nueva PedidosMovilObservados: un renglon por pedido del celular que el servidor no pudo grabar
      (sin stock, error definitivo, sin productos o fallas repetidas). Guarda el pedido tal como lo mando
      el celular (JSON) para poder corregirlo desde la web.
   2. SP nuevo MAM_PedidosMovilObservados (lo usan la API del celular y la web):
        @tipo 1  registrar / actualizar la observacion (API)
        @tipo 2  estado de varios pedidos para el celular (API): lo que la web corrigio o descarto
        @tipo 3  listado para la web
        @tipo 4  resumen para la notificacion de la web (cantidad de pendientes)
        @tipo 5  la web corrigio y grabo el pedido
        @tipo 6  la web descarto el pedido
        @tipo 7  una observacion (con el pedido JSON) para corregirla en la web
        @tipo 8  stock actual de varios productos (deposito de la sucursal 1, el mismo que valida @26)
      Si el pedido ya entro (Ventas.IdSincronizacion = codigo del celular), la observacion se da por
      resuelta sola (Estado 2), la haya corregido el celular o la web.
   3. Programa web btnPedidosObservados (rol Administrador): pantalla y notificacion de pedidos observados.
   NO modifica sp_go_TC004_appMovil ni ninguna tabla existente.
   Se puede ejecutar mas de una vez.
   VOLVER ATRAS: revertir\REVERTIR_09_PedidosObservados.sql (SOLO para deshacer).
   ===================================================================== */
IF OBJECT_ID('dbo.PedidosMovilObservados') IS NULL
BEGIN
	CREATE TABLE dbo.PedidosMovilObservados (
		Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_PedidosMovilObservados PRIMARY KEY,
		CodigoGenerado nvarchar(150) NOT NULL,   -- codigo del pedido en el celular (= Ventas.IdSincronizacion)
		TipoObservacion int NOT NULL,            -- 1 SIN STOCK, 2 RECHAZADO, 3 SIN PRODUCTOS, 4 NO SE PUDO ENVIAR
		Motivo nvarchar(2000) NULL,
		PersonalId int NULL,                     -- preventista (oarepa)
		ClienteId int NULL,
		Cliente nvarchar(200) NULL,
		Total decimal(18,2) NULL,                -- sin el incremento
		Incremento decimal(18,2) NULL,
		FechaPedido date NULL,
		Pedido nvarchar(max) NULL,               -- el pedido como lo mando el celular (la web lo corrige)
		ModeloCelular nvarchar(120) NULL,
		Intentos int NOT NULL CONSTRAINT DF_PedidosMovilObservados_Intentos DEFAULT 1,
		Estado int NOT NULL CONSTRAINT DF_PedidosMovilObservados_Estado DEFAULT 0, -- 0 PENDIENTE, 1 CORREGIDO EN WEB, 2 ENTRO DESDE EL CELULAR, 3 DESCARTADO EN WEB
		VentaId int NULL,
		FechaRegistro datetime2(0) NOT NULL,
		FechaActualizacion datetime2(0) NOT NULL,
		UsuarioResuelve nvarchar(50) NULL,
		FechaResuelto datetime2(0) NULL,
		ObsResuelve nvarchar(300) NULL)
	CREATE UNIQUE INDEX UX_PedidosMovilObservados_Codigo ON dbo.PedidosMovilObservados (CodigoGenerado)
	CREATE INDEX IX_PedidosMovilObservados_Estado ON dbo.PedidosMovilObservados (Estado, FechaActualizacion)
	RAISERROR('09: tabla PedidosMovilObservados creada.', 10, 1) WITH NOWAIT
END
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
IF OBJECT_ID('dbo.MAM_PedidosMovilObservados') IS NULL EXEC('CREATE PROCEDURE dbo.MAM_PedidosMovilObservados AS RETURN 0')
GO
-- Sanbit: pedidos del celular observados (no pudieron entrar). Ver encabezado de 09_PedidosObservados.sql
ALTER PROCEDURE dbo.MAM_PedidosMovilObservados (@tipo int, @Id int = -1, @CodigoGenerado nvarchar(150) = '',
	@TipoObservacion int = 0, @Motivo nvarchar(2000) = '', @PersonalId int = -1, @ClienteId int = -1,
	@Cliente nvarchar(200) = '', @Total decimal(18,2) = 0, @Incremento decimal(18,2) = -1, @FechaPedido date = NULL,
	@Pedido nvarchar(max) = '', @ModeloCelular nvarchar(120) = '', @Codigos xml = NULL, @Productos xml = NULL,
	@Estado int = -1, @FechaDesde date = NULL, @FechaHasta date = NULL, @VentaId int = -1,
	@ObsResuelve nvarchar(300) = '', @usuario nvarchar(50) = '')
AS
BEGIN
	SET NOCOUNT ON

	-- ===== 1: registrar la observacion (la API cuando rechaza el pedido, o el celular cuando lo reporta) =====
	-- Una sola fila por pedido: si vuelve a fallar se actualiza (Intentos + 1). Lo que la web ya corrigio o
	-- descarto no se toca. Si el pedido ya entro no hay nada que observar.
	IF @tipo = 1
	BEGIN
		IF EXISTS (SELECT 1 FROM Ventas AS v WHERE v.IdSincronizacion = @CodigoGenerado)
		BEGIN
			SELECT CAST(0 AS int) AS Id, CAST(2 AS int) AS Estado
			RETURN
		END
		BEGIN TRAN
		UPDATE o WITH (UPDLOCK, HOLDLOCK)
		SET TipoObservacion = @TipoObservacion, Motivo = LEFT(@Motivo, 2000),
			PersonalId = IIF(@PersonalId > 0, @PersonalId, o.PersonalId), ClienteId = IIF(@ClienteId > 0, @ClienteId, o.ClienteId),
			Cliente = IIF(@Cliente <> '', LEFT(@Cliente, 200), o.Cliente), Total = @Total,
			Incremento = IIF(@Incremento >= 0, @Incremento, o.Incremento), FechaPedido = ISNULL(@FechaPedido, o.FechaPedido),
			Pedido = IIF(@Pedido <> '', @Pedido, o.Pedido), ModeloCelular = IIF(@ModeloCelular <> '', LEFT(@ModeloCelular, 120), o.ModeloCelular),
			Intentos = o.Intentos + 1, FechaActualizacion = SYSDATETIME()
		FROM PedidosMovilObservados AS o
		WHERE o.CodigoGenerado = @CodigoGenerado AND o.Estado = 0
		IF @@ROWCOUNT = 0 AND NOT EXISTS (SELECT 1 FROM PedidosMovilObservados AS o WITH (UPDLOCK, HOLDLOCK) WHERE o.CodigoGenerado = @CodigoGenerado)
			INSERT INTO PedidosMovilObservados (CodigoGenerado, TipoObservacion, Motivo, PersonalId, ClienteId, Cliente, Total,
				Incremento, FechaPedido, Pedido, ModeloCelular, Intentos, Estado, FechaRegistro, FechaActualizacion)
			VALUES (@CodigoGenerado, @TipoObservacion, LEFT(@Motivo, 2000), IIF(@PersonalId > 0, @PersonalId, NULL),
				IIF(@ClienteId > 0, @ClienteId, NULL), LEFT(@Cliente, 200), @Total, IIF(@Incremento >= 0, @Incremento, NULL),
				@FechaPedido, NULLIF(@Pedido, ''), LEFT(@ModeloCelular, 120), 1, 0, SYSDATETIME(), SYSDATETIME())
		COMMIT TRAN
		SELECT o.Id, o.Estado FROM PedidosMovilObservados AS o WHERE o.CodigoGenerado = @CodigoGenerado
		RETURN
	END

	-- ===== 2: estado para el celular. @Codigos = <r><c>codigo</c>...</r> =====
	-- Recordset 1: las observaciones ya resueltas (1 corregido en web, 2 entro, 3 descartado).
	-- Recordset 2: el detalle que QUEDO GRABADO en el servidor (el celular reemplaza el suyo con este).
	IF @tipo = 2
	BEGIN
		DECLARE @cod TABLE (CodigoGenerado nvarchar(150) PRIMARY KEY)
		INSERT INTO @cod (CodigoGenerado)
		SELECT DISTINCT LEFT(x.c.value('.', 'nvarchar(200)'), 150) FROM @Codigos.nodes('/r/c') AS x(c)
		WHERE x.c.value('.', 'nvarchar(200)') <> ''

		UPDATE o SET Estado = 2, VentaId = v.Id, FechaResuelto = ISNULL(o.FechaResuelto, SYSDATETIME()),
			UsuarioResuelve = ISNULL(o.UsuarioResuelve, 'CELULAR')
		FROM PedidosMovilObservados AS o
		INNER JOIN @cod AS k ON k.CodigoGenerado = o.CodigoGenerado
		INNER JOIN Ventas AS v ON v.IdSincronizacion = o.CodigoGenerado
		WHERE o.Estado IN (0, 3)

		SELECT o.CodigoGenerado, o.Estado, ISNULL(o.VentaId, 0) AS VentaId, ISNULL(o.UsuarioResuelve, '') AS UsuarioResuelve,
			ISNULL(o.ObsResuelve, '') AS ObsResuelve, ISNULL(v.TotalVenta, 0) AS TotalVenta,
			ISNULL((SELECT TOP 1 ia.Monto FROM VentasIncrementoAplicado AS ia WHERE ia.VentaId = v.Id ORDER BY ia.Id DESC), -1) AS Incremento
		FROM PedidosMovilObservados AS o
		INNER JOIN @cod AS k ON k.CodigoGenerado = o.CodigoGenerado
		LEFT JOIN Ventas AS v ON v.Id = o.VentaId
		WHERE o.Estado IN (1, 2, 3)

		SELECT o.CodigoGenerado, d.ProductoId, RTRIM(ISNULL(p.DescripcionProducto, '')) AS Producto, d.Cantidad, d.Precio,
			d.SubTotal, ISNULL(d.MontoDescuento, 0) AS MontoDescuento, d.Total
		FROM PedidosMovilObservados AS o
		INNER JOIN @cod AS k ON k.CodigoGenerado = o.CodigoGenerado
		INNER JOIN VentasDetalles AS d ON d.VentaId = o.VentaId
		LEFT JOIN Productos AS p ON p.Id = d.ProductoId
		WHERE o.Estado IN (1, 2)
		ORDER BY o.CodigoGenerado, d.Id
		RETURN
	END

	-- Lo que ya entro se da por resuelto (para la web: listado y notificacion)
	IF @tipo IN (3, 4, 7)
		UPDATE o SET Estado = 2, VentaId = v.Id, FechaResuelto = ISNULL(o.FechaResuelto, SYSDATETIME()),
			UsuarioResuelve = ISNULL(o.UsuarioResuelve, 'CELULAR')
		FROM PedidosMovilObservados AS o
		INNER JOIN Ventas AS v ON v.IdSincronizacion = o.CodigoGenerado
		WHERE o.Estado IN (0, 3)

	-- ===== 3: listado web. Pendientes siempre; los resueltos del rango de fechas (por fecha de registro) =====
	IF @tipo = 3
		SELECT o.Id, o.CodigoGenerado, o.TipoObservacion, o.Motivo, o.PersonalId, RTRIM(ISNULL(pe.NombrePersonal, '')) AS NombrePersonal,
			o.ClienteId, RTRIM(ISNULL(c.NombreCliente, o.Cliente)) AS NombreCliente, o.Total, o.Incremento, o.FechaPedido,
			o.Estado, o.VentaId, o.Intentos, o.FechaRegistro, o.FechaActualizacion, o.UsuarioResuelve, o.FechaResuelto,
			o.ObsResuelve, o.ModeloCelular
		FROM PedidosMovilObservados AS o
		LEFT JOIN Personal AS pe ON pe.Id = o.PersonalId
		LEFT JOIN Clientes AS c ON c.Id = o.ClienteId
		WHERE (o.Estado = 0 OR (o.FechaRegistro >= @FechaDesde AND o.FechaRegistro < DATEADD(DAY, 1, @FechaHasta)))
			AND (@Estado < 0 OR o.Estado = @Estado)
		ORDER BY IIF(o.Estado = 0, 0, 1), o.FechaActualizacion DESC

	-- ===== 4: resumen para la notificacion =====
	IF @tipo = 4
		SELECT COUNT(*) AS Pendientes, MAX(o.FechaActualizacion) AS Ultima,
			SUM(IIF(o.TipoObservacion = 1, 1, 0)) AS SinStock, SUM(IIF(o.TipoObservacion <> 1, 1, 0)) AS Otros
		FROM PedidosMovilObservados AS o
		WHERE o.Estado = 0

	-- ===== 5: la web corrigio y grabo el pedido (con sp_go_TC004_appMovil @26, el mismo codigo del celular) =====
	IF @tipo = 5
	BEGIN
		UPDATE o SET Estado = 1, VentaId = @VentaId, Pedido = IIF(@Pedido <> '', @Pedido, o.Pedido),
			UsuarioResuelve = LEFT(@usuario, 50), FechaResuelto = SYSDATETIME(), ObsResuelve = LEFT(@ObsResuelve, 300),
			FechaActualizacion = SYSDATETIME()
		FROM PedidosMovilObservados AS o
		WHERE o.Id = @Id AND o.Estado IN (0, 2)
		SELECT @@ROWCOUNT AS filas
	END

	-- ===== 6: la web descarto el pedido (solo si todavia no entro) =====
	IF @tipo = 6
	BEGIN
		UPDATE o SET Estado = 3, UsuarioResuelve = LEFT(@usuario, 50), FechaResuelto = SYSDATETIME(),
			ObsResuelve = LEFT(@ObsResuelve, 300), FechaActualizacion = SYSDATETIME()
		FROM PedidosMovilObservados AS o
		WHERE o.Id = @Id AND o.Estado = 0
			AND NOT EXISTS (SELECT 1 FROM Ventas AS v WHERE v.IdSincronizacion = o.CodigoGenerado)
		SELECT @@ROWCOUNT AS filas
	END

	-- ===== 7: una observacion con el pedido (JSON) para corregirla =====
	IF @tipo = 7
		SELECT o.Id, o.CodigoGenerado, o.TipoObservacion, o.Motivo, o.PersonalId, RTRIM(ISNULL(pe.NombrePersonal, '')) AS NombrePersonal,
			o.ClienteId, RTRIM(ISNULL(c.NombreCliente, o.Cliente)) AS NombreCliente, o.Total, o.Incremento, o.FechaPedido,
			o.Estado, o.VentaId, o.Intentos, o.FechaRegistro, o.FechaActualizacion, o.UsuarioResuelve, o.FechaResuelto,
			o.ObsResuelve, o.ModeloCelular, o.Pedido
		FROM PedidosMovilObservados AS o
		LEFT JOIN Personal AS pe ON pe.Id = o.PersonalId
		LEFT JOIN Clientes AS c ON c.Id = o.ClienteId
		WHERE o.Id = @Id

	-- ===== 8: stock actual. @Productos = <r><p>id</p>...</r>. Deposito de la sucursal 1 (el que valida @26) =====
	IF @tipo = 8
	BEGIN
		DECLARE @prod TABLE (ProductoId int PRIMARY KEY)
		INSERT INTO @prod (ProductoId)
		SELECT DISTINCT x.p.value('.', 'int') FROM @Productos.nodes('/r/p') AS x(p)
		SELECT k.ProductoId, RTRIM(ISNULL(p.DescripcionProducto, '')) AS Producto,
			ISNULL((SELECT SUM(s.Cantidad) FROM ProductosStock AS s WHERE s.ProductoId = k.ProductoId
				AND s.DepositoId = (SELECT a.DepositoId FROM Almacenes AS a WHERE a.Id = 1)), 0) AS Stock
		FROM @prod AS k
		LEFT JOIN Productos AS p ON p.Id = k.ProductoId
	END
END
GO
-- Programa de la web "Pedidos observados del celular" (btnPedidosObservados), en el modulo de los reportes de
-- distribucion, asignado al rol Administrador (1). Quien lo tenga recibe la notificacion en la web.
IF NOT EXISTS (SELECT 1 FROM Programas WHERE IdPrograma = 'btnPedidosObservados')
BEGIN
	DECLARE @IdProg int, @ModuloProg int
	SET @ModuloProg = ISNULL((SELECT TOP 1 Modulo FROM Programas WHERE IdPrograma = 'btnRepAuditoriaVentas'), 1)
	IF COLUMNPROPERTY(OBJECT_ID('dbo.Programas'), 'Id', 'IsIdentity') = 1
	BEGIN
		INSERT INTO Programas (IdPrograma, DescripcionPrograma, Modulo) VALUES ('btnPedidosObservados', 'PEDIDOS OBSERVADOS CELULAR', @ModuloProg)
		SET @IdProg = SCOPE_IDENTITY()
	END
	ELSE
	BEGIN
		SET @IdProg = ISNULL((SELECT MAX(Id) FROM Programas), 0) + 1
		INSERT INTO Programas (Id, IdPrograma, DescripcionPrograma, Modulo) VALUES (@IdProg, 'btnPedidosObservados', 'PEDIDOS OBSERVADOS CELULAR', @ModuloProg)
	END
	IF COLUMNPROPERTY(OBJECT_ID('dbo.RolesDetalles'), 'Id', 'IsIdentity') = 1
		INSERT INTO RolesDetalles (RolId, ProgramaId, Ver, Insertar, Modificar, Eliminar)
		SELECT r.Id, @IdProg, IIF(r.Id = 1, 1, 0), IIF(r.Id = 1, 1, 0), IIF(r.Id = 1, 1, 0), 0 FROM Roles AS r
	ELSE
		INSERT INTO RolesDetalles (Id, RolId, ProgramaId, Ver, Insertar, Modificar, Eliminar)
		SELECT ISNULL((SELECT MAX(Id) FROM RolesDetalles), 0) + ROW_NUMBER() OVER (ORDER BY r.Id), r.Id, @IdProg,
			IIF(r.Id = 1, 1, 0), IIF(r.Id = 1, 1, 0), IIF(r.Id = 1, 1, 0), 0
		FROM Roles AS r
	RAISERROR('09: programa "Pedidos Observados Celular" creado y asignado al rol Administrador.', 10, 1) WITH NOWAIT
END
GO
RAISERROR('09 PedidosObservados INSTALADO.', 10, 1) WITH NOWAIT
GO
