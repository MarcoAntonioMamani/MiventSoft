/* ============================================================================
   MAM_VentasIncremento - v2: Historial completo (quien / cuanto / que dia)
   Base de datos: DistribucionDistralKCP2023
   Generado con Claude para Marco Mamani (BANCOSOL)

   QUE CAMBIA respecto de la v1 (MAM_VentasIncremento.sql):
   - Antes: si alguien tocaba el monto dos veces el mismo dia, la segunda
     pisaba (UPDATE) la primera y se perdia el rastro de quien/cuando.
   - Ahora: CADA guardado inserta una fila NUEVA (nunca se pisa nada), asi
     queda un historial completo. El monto "vigente" de hoy sigue
     calculandose con el mismo criterio de "carry forward" (si hoy nadie
     lo toco, se usa el ultimo monto guardado; si nunca se configuro nada,
     el default sigue siendo 2.00 Bs).
   - Se agrega @tipo=3 para listar el historial (usado por la pantalla
     nueva Rep_HistorialIncrementoVenta).

   IMPORTANTE: esto se ejecuta DESPUES de MAM_VentasIncremento.sql (v1).
   No perdes los datos que ya se guardaron - la tabla no se recrea, solo se
   le quita una restriccion, y el procedimiento se reemplaza (DROP + CREATE,
   como siempre). Es seguro volver a ejecutar este script las veces que haga
   falta.
   ============================================================================ */

-- 1) Sacar el UNIQUE(Fecha): a partir de ahora puede haber varias filas para
--    el mismo dia (una por cada vez que alguien cambio el monto ese dia).
IF EXISTS (
    SELECT 1 FROM sys.key_constraints
    WHERE name = 'UQ_VentasIncrementoDiario_Fecha'
      AND parent_object_id = OBJECT_ID('dbo.VentasIncrementoDiario')
)
BEGIN
    ALTER TABLE dbo.VentasIncrementoDiario DROP CONSTRAINT UQ_VentasIncrementoDiario_Fecha;
END
GO

IF OBJECT_ID('dbo.MAM_VentasIncremento', 'P') IS NOT NULL
    DROP PROCEDURE dbo.MAM_VentasIncremento;
GO

CREATE PROCEDURE dbo.MAM_VentasIncremento
    @tipo         INT,
    @usuario      NVARCHAR(150)  = NULL,
    @Monto        DECIMAL(18,2)  = NULL,   -- solo para @tipo=2
    @fechaDesde   NVARCHAR(20)   = NULL,   -- formato 'yyyy/MM/dd' - solo @tipo=3
    @fechaHasta   NVARCHAR(20)   = NULL    -- formato 'yyyy/MM/dd' - solo @tipo=3
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Hoy DATE = CAST(SYSDATETIME() AS DATE);

    IF (@tipo = 1)
    BEGIN
        -- Monto vigente para hoy: el ULTIMO cambio guardado (el mas reciente
        -- por fecha/hora), sin importar si fue hoy o un dia anterior
        -- (carry forward). Si todavia nadie configuro nada, arranca en 2 Bs.
        DECLARE @UltimoMonto DECIMAL(18,2);
        SELECT TOP (1) @UltimoMonto = Monto
        FROM dbo.VentasIncrementoDiario
        WHERE Fecha <= @Hoy
        ORDER BY FechaHoraModificacion DESC;

        IF (@UltimoMonto IS NULL)
            SET @UltimoMonto = 2.00;   -- default pedido: arranca en 2 Bs

        SELECT @UltimoMonto AS Monto, @Hoy AS Fecha;
    END

    IF (@tipo = 2)
    BEGIN
        -- Cada cambio es una fila NUEVA (nunca se pisa una fila existente),
        -- asi queda el historial completo de quien cambio cuanto y cuando.
        INSERT INTO dbo.VentasIncrementoDiario (Fecha, Monto, UsuarioModifico, FechaHoraModificacion)
        VALUES (@Hoy, @Monto, @usuario, SYSDATETIME());

        SELECT @Monto AS Monto, @Hoy AS Fecha;
    END

    IF (@tipo = 3)
    BEGIN
        -- Listado de historial para la pantalla Rep_HistorialIncrementoVenta,
        -- filtrado por cuando se hizo el cambio (mismo criterio de rango de
        -- fechas que ya usamos en MAM_AuditoriaVentas).
        DECLARE @Desde DATETIME2(3) = CAST(@fechaDesde AS DATE);
        DECLARE @Hasta DATETIME2(3) = DATEADD(DAY, 1, CAST(@fechaHasta AS DATE));

        SELECT
            Id,
            Fecha,
            Monto,
            ISNULL(UsuarioModifico, '') AS Usuario,
            FechaHoraModificacion
        FROM dbo.VentasIncrementoDiario
        WHERE FechaHoraModificacion >= @Desde
          AND FechaHoraModificacion < @Hasta
        ORDER BY FechaHoraModificacion DESC;
    END
END
GO

/* ============================================================================
   PRUEBA RAPIDA:
   EXEC dbo.MAM_VentasIncremento @tipo = 1, @usuario = 'PRUEBA';
   EXEC dbo.MAM_VentasIncremento @tipo = 2, @usuario = 'PRUEBA', @Monto = 3.50;
   EXEC dbo.MAM_VentasIncremento @tipo = 2, @usuario = 'PRUEBA', @Monto = 4.00;  -- mismo dia, no pisa la anterior
   EXEC dbo.MAM_VentasIncremento @tipo = 1, @usuario = 'PRUEBA';  -- deberia devolver 4.00 (la ultima)
   EXEC dbo.MAM_VentasIncremento @tipo = 3, @usuario = 'PRUEBA',
        @fechaDesde = '2026/08/01', @fechaHasta = '2026/09/22';  -- historial completo
   ============================================================================ */
