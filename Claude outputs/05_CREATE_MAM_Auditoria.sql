/* ============================================================================
   MAM_Auditoria
   ----------------------------------------------------------------------------
   SP nuevo y dedicado (no toca MAM_Ventas/MAM_Compras/MAM_Movimientos) para
   leer las tablas de auditoria creadas en 01_CREATE_TablasAuditoria.sql.
   Mismo patron que los SP existentes: switch por @tipo, BEGIN TRY/CATCH con
   INSERT a Bitacora en caso de error.

   Como se agrupan los "eventos":
   Cada operacion (Crear/Modificar/Eliminar) escribe 1 o 2 filas en la tabla
   de auditoria del modulo (ANTES y/o DESPUES), usando el MISMO Id, Accion,
   FechaAccion y HoraAccion para ambas filas (se calculan una sola vez al
   principio de cada rama del SP transaccional). Por eso alcanza con
   (Id, Accion, FechaAccion, HoraAccion) para "re-unir" el par Antes/Despues
   de un mismo evento, sin necesitar una columna nueva.

   Uso pensado desde el formulario (ver AccesoLogica.vb):
     1) @tipo=1/4/7  -> Lista de eventos (un renglon por evento) para llenar
        la grilla superior con filtros de fecha/usuario/accion.
     2) @tipo=2/5/8  -> Cabecera Antes/Despues del evento seleccionado
        (0, 1 o 2 filas segun sea Crear/Eliminar o Modificar).
     3) @tipo=3/6/9  -> Detalle Antes/Despues del evento seleccionado.
   ============================================================================ */

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[MAM_Auditoria] (
	@tipo int,
	@Id int = -1,
	@Accion nvarchar(20) = '',
	@FechaAccion date = null,
	@HoraAccion nvarchar(5) = '',
	@Desde date = null,
	@Hasta date = null,
	@UsuarioAccion nvarchar(50) = '',
	@usuario nvarchar(50) = ''
)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora = CONCAT(DATEPART(HOUR, GETDATE()), ':', DATEPART(MINUTE, GETDATE()))
	DECLARE @newFecha date
	set @newFecha = GETDATE()

	-- ========================================================================
	-- VENTAS
	-- ========================================================================
	IF @tipo=1 --Listar eventos de auditoria de Ventas (filtro fecha/usuario/accion)
	BEGIN
		SET ARITHABORT ON
		BEGIN TRY
			SELECT a.Id, a.Accion, a.UsuarioAccion, a.FechaAccion, a.HoraAccion,
				MAX(a.TotalVenta) as TotalVenta, MAX(c.NombreCliente) as NombreCliente
			FROM VentasAuditoria as a
			LEFT JOIN Clientes as c ON c.Id = a.ClienteId
			WHERE a.FechaAccion >= @Desde and a.FechaAccion <= @Hasta
				AND (@UsuarioAccion = '' OR a.UsuarioAccion = @UsuarioAccion)
				AND (@Accion = '' OR a.Accion = @Accion)
			GROUP BY a.Id, a.Accion, a.UsuarioAccion, a.FechaAccion, a.HoraAccion
			ORDER BY a.FechaAccion DESC, a.HoraAccion DESC, a.Id DESC
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END

	IF @tipo=2 --Comparar cabecera Antes/Despues de un evento de Ventas
	BEGIN
		SET ARITHABORT ON
		BEGIN TRY
			SELECT a.*
			FROM VentasAuditoria as a
			WHERE a.Id=@Id AND a.Accion=@Accion AND a.FechaAccion=@FechaAccion AND a.HoraAccion=@HoraAccion
			ORDER BY a.Momento ASC
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END

	IF @tipo=3 --Comparar detalle Antes/Despues de un evento de Ventas
	BEGIN
		SET ARITHABORT ON
		BEGIN TRY
			SELECT enc.Momento, det.*
			FROM VentasDetalleAuditoria as det
			INNER JOIN VentasAuditoria as enc ON enc.AuditoriaId = det.AuditoriaId
			WHERE enc.Id=@Id AND enc.Accion=@Accion AND enc.FechaAccion=@FechaAccion AND enc.HoraAccion=@HoraAccion
			ORDER BY enc.Momento ASC, det.ProductoId ASC
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END

	-- ========================================================================
	-- COMPRAS
	-- ========================================================================
	IF @tipo=4 --Listar eventos de auditoria de Compras
	BEGIN
		SET ARITHABORT ON
		BEGIN TRY
			SELECT a.Id, a.Accion, a.UsuarioAccion, a.FechaAccion, a.HoraAccion,
				MAX(a.TotalCompra) as TotalCompra, MAX(p.NombreProveedor) as NombreProveedor
			FROM ComprasAuditoria as a
			LEFT JOIN Proveedor as p ON p.Id = a.ProveedorId
			WHERE a.FechaAccion >= @Desde and a.FechaAccion <= @Hasta
				AND (@UsuarioAccion = '' OR a.UsuarioAccion = @UsuarioAccion)
				AND (@Accion = '' OR a.Accion = @Accion)
			GROUP BY a.Id, a.Accion, a.UsuarioAccion, a.FechaAccion, a.HoraAccion
			ORDER BY a.FechaAccion DESC, a.HoraAccion DESC, a.Id DESC
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),4,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END

	IF @tipo=5 --Comparar cabecera Antes/Despues de un evento de Compras
	BEGIN
		SET ARITHABORT ON
		BEGIN TRY
			SELECT a.*
			FROM ComprasAuditoria as a
			WHERE a.Id=@Id AND a.Accion=@Accion AND a.FechaAccion=@FechaAccion AND a.HoraAccion=@HoraAccion
			ORDER BY a.Momento ASC
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),5,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END

	IF @tipo=6 --Comparar detalle Antes/Despues de un evento de Compras
	BEGIN
		SET ARITHABORT ON
		BEGIN TRY
			SELECT enc.Momento, det.*
			FROM CompraDetalleAuditoria as det
			INNER JOIN ComprasAuditoria as enc ON enc.AuditoriaId = det.AuditoriaId
			WHERE enc.Id=@Id AND enc.Accion=@Accion AND enc.FechaAccion=@FechaAccion AND enc.HoraAccion=@HoraAccion
			ORDER BY enc.Momento ASC, det.ProductoId ASC
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),6,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END

	-- ========================================================================
	-- MOVIMIENTOS
	-- ========================================================================
	IF @tipo=7 --Listar eventos de auditoria de Movimientos
	BEGIN
		SET ARITHABORT ON
		BEGIN TRY
			SELECT a.Id, a.Accion, a.UsuarioAccion, a.FechaAccion, a.HoraAccion,
				MAX(t.Descripcion) as TipoMovimiento, MAX(d.NombreDeposito) as NombreDeposito
			FROM MovimientosAuditoria as a
			LEFT JOIN MovimientosTipos as t ON t.Id = a.ConceptoId
			LEFT JOIN Depositos as d ON d.id = a.DepositoId
			WHERE a.FechaAccion >= @Desde and a.FechaAccion <= @Hasta
				AND (@UsuarioAccion = '' OR a.UsuarioAccion = @UsuarioAccion)
				AND (@Accion = '' OR a.Accion = @Accion)
			GROUP BY a.Id, a.Accion, a.UsuarioAccion, a.FechaAccion, a.HoraAccion
			ORDER BY a.FechaAccion DESC, a.HoraAccion DESC, a.Id DESC
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),7,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END

	IF @tipo=8 --Comparar cabecera Antes/Despues de un evento de Movimientos
	BEGIN
		SET ARITHABORT ON
		BEGIN TRY
			SELECT a.*
			FROM MovimientosAuditoria as a
			WHERE a.Id=@Id AND a.Accion=@Accion AND a.FechaAccion=@FechaAccion AND a.HoraAccion=@HoraAccion
			ORDER BY a.Momento ASC
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),8,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END

	IF @tipo=9 --Comparar detalle Antes/Despues de un evento de Movimientos
	BEGIN
		SET ARITHABORT ON
		BEGIN TRY
			SELECT enc.Momento, det.*
			FROM MovimientosDetalleAuditoria as det
			INNER JOIN MovimientosAuditoria as enc ON enc.AuditoriaId = det.AuditoriaId
			WHERE enc.Id=@Id AND enc.Accion=@Accion AND enc.FechaAccion=@FechaAccion AND enc.HoraAccion=@HoraAccion
			ORDER BY enc.Momento ASC, det.ProductoId ASC
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact,baregistroid)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),9,@newFecha,@newHora,@usuario,@Id)
		END CATCH
	END
END
GO
