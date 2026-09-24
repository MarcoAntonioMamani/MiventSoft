/* ============================================================================
   MAM_VentasIncremento - Incremento diario configurable sobre el total de venta
   Base de datos: DistribucionDistralKCP2023
   Generado con Claude para Marco Mamani (BANCOSOL)

   QUE HACE:
   - Guarda un monto en Bs (por dia) que se puede modificar desde el boton
     nuevo en Tec_Principal.
   - Si un dia nadie lo toca, se sigue usando el ultimo monto configurado
     (no hace falta reconfigurar todos los dias) - "carry forward".
   - Si nunca se configuro nada, el valor por defecto es 2.00 Bs (pedido
     explicito).

   IMPORTANTE - ALCANCE DE ESTA ENTREGA:
   Este script SOLO crea la tabla y el procedimiento para leer/guardar el
   monto del dia (lo que pediste "por ahora"). NO modifica MAM_Ventas ni
   sp_go_TC004_appMovil todavia, o sea el monto que se guarda aqui todavia
   NO se suma automaticamente al total de las ventas nuevas - eso es un
   paso aparte que hacemos cuando me digas, porque toca las mismas dos SPs
   que ya auditamos (justo para no repetir el mismo cuidado que tuvimos ahi).

   Es seguro volver a ejecutar este script: la creacion de tabla esta
   protegida con IF OBJECT_ID... y el procedimiento hace DROP + CREATE.
   ============================================================================ */

IF OBJECT_ID('dbo.VentasIncrementoDiario', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.VentasIncrementoDiario (
        Id                     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Fecha                  DATE NOT NULL,
        Monto                  DECIMAL(18,2) NOT NULL,
        UsuarioModifico        NVARCHAR(150) NULL,
        FechaHoraModificacion  DATETIME2(3) NOT NULL,
        CONSTRAINT UQ_VentasIncrementoDiario_Fecha UNIQUE (Fecha)
    );
END
GO

IF OBJECT_ID('dbo.MAM_VentasIncremento', 'P') IS NOT NULL
    DROP PROCEDURE dbo.MAM_VentasIncremento;
GO

CREATE PROCEDURE dbo.MAM_VentasIncremento
    @tipo     INT,
    @usuario  NVARCHAR(150)  = NULL,
    @Monto    DECIMAL(18,2)  = NULL    -- solo para @tipo=2
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Hoy DATE = CAST(SYSDATETIME() AS DATE);

    IF (@tipo = 1)
    BEGIN
        -- Obtener el monto vigente para hoy
        IF EXISTS (SELECT 1 FROM dbo.VentasIncrementoDiario WHERE Fecha = @Hoy)
        BEGIN
            SELECT Monto, Fecha FROM dbo.VentasIncrementoDiario WHERE Fecha = @Hoy;
        END
        ELSE
        BEGIN
            DECLARE @UltimoMonto DECIMAL(18,2);
            SELECT TOP (1) @UltimoMonto = Monto
            FROM dbo.VentasIncrementoDiario
            WHERE Fecha < @Hoy
            ORDER BY Fecha DESC;

            IF (@UltimoMonto IS NULL)
                SET @UltimoMonto = 2.00;   -- default pedido: arranca en 2 Bs

            SELECT @UltimoMonto AS Monto, @Hoy AS Fecha;
        END
    END

    IF (@tipo = 2)
    BEGIN
        -- Guardar/actualizar el monto de HOY (un registro por dia = historial)
        IF EXISTS (SELECT 1 FROM dbo.VentasIncrementoDiario WHERE Fecha = @Hoy)
        BEGIN
            UPDATE dbo.VentasIncrementoDiario
               SET Monto = @Monto,
                   UsuarioModifico = @usuario,
                   FechaHoraModificacion = SYSDATETIME()
             WHERE Fecha = @Hoy;
        END
        ELSE
        BEGIN
            INSERT INTO dbo.VentasIncrementoDiario (Fecha, Monto, UsuarioModifico, FechaHoraModificacion)
            VALUES (@Hoy, @Monto, @usuario, SYSDATETIME());
        END

        SELECT @Monto AS Monto, @Hoy AS Fecha;
    END
END
GO

/* ============================================================================
   PRUEBA RAPIDA:
   EXEC dbo.MAM_VentasIncremento @tipo = 1, @usuario = 'PRUEBA';
   EXEC dbo.MAM_VentasIncremento @tipo = 2, @usuario = 'PRUEBA', @Monto = 3.50;
   EXEC dbo.MAM_VentasIncremento @tipo = 1, @usuario = 'PRUEBA';  -- deberia devolver 3.50
   SELECT * FROM dbo.VentasIncrementoDiario ORDER BY Fecha DESC;  -- historial por dia
   ============================================================================ */
