/* ============================================================================
   MAM_AuditoriaVentas - Nuevo procedimiento de CONSULTA (solo lectura)
   Base de datos: DistribucionDistralKCP2023
   Generado con Claude para Marco Mamani (BANCOSOL)

   Sigue exactamente tu convencion MAM_<Entidad> + @tipo dispatcher (igual que
   MAM_Ventas, MAM_Clientes, MAM_Clasificadores, etc). No modifica ninguna
   tabla, solo lee de VentasAuditoriaEventos / VentasAuditoriaDetalle (las
   tablas que ya creamos) y de Ventas/Clientes/Productos para mostrar nombres
   en vez de solo Ids.

   @tipo = 1  ->  Grilla MAESTRA: eventos (Anulado/Eliminado/Modificado) de un
                  rango de fechas, opcionalmente filtrados por TipoEvento.
   @tipo = 2  ->  Grilla DETALLE: productos/cambios de UN evento especifico
                  (VentasAuditoriaDetalle), dado su EventoId (el Id de
                  VentasAuditoriaEventos, no el VentaId).

   Por que el filtro de fecha usa >= @Desde AND < @Hasta (y no
   CAST(FechaHora AS DATE) = ...): asi el motor SI puede usar el indice
   IX_VentasAuditoriaEventos_FechaHora_Tipo que ya quedo creado sobre
   (FechaHora, TipoEvento) - es "sargable". Con CAST(...) el indice no se
   usaria y tendria que recorrer toda la tabla.

   Nunca se ejecuto todavia contra la base - recomendado probarlo primero
   en un ambiente de desarrollo/pruebas antes de usarlo en produccion.
   ============================================================================ */

IF OBJECT_ID('dbo.MAM_AuditoriaVentas', 'P') IS NOT NULL
    DROP PROCEDURE dbo.MAM_AuditoriaVentas;
GO

CREATE PROCEDURE dbo.MAM_AuditoriaVentas
    @tipo           INT,
    @usuario        NVARCHAR(150) = NULL,   -- se recibe por convencion (igual que el resto de tus MAM_*), no se usa para filtrar
    @fechaDesde     NVARCHAR(20)  = NULL,   -- formato 'yyyy/MM/dd', igual que L_prListarHistorico
    @fechaHasta     NVARCHAR(20)  = NULL,   -- formato 'yyyy/MM/dd'
    @tipoEvento     NVARCHAR(20)  = NULL,   -- 'TODOS' | 'ANULADO' | 'ELIMINADO' | 'MODIFICADO'
    @EventoId       INT           = NULL    -- Id de VentasAuditoriaEventos (solo para @tipo=2)
AS
BEGIN
    SET NOCOUNT ON;

    IF (@tipo = 1)
    BEGIN
        DECLARE @Desde DATETIME2(3) = CAST(@fechaDesde AS DATE);
        DECLARE @Hasta DATETIME2(3) = DATEADD(DAY, 1, CAST(@fechaHasta AS DATE));

        SELECT
            e.Id,                                    -- EventoId: usar este para pedir el detalle (@tipo=2)
            e.VentaId,
            e.TipoEvento,
            e.Origen,
            e.FechaHora,
            e.Usuario,
            e.EstadoAnterior,
            e.EstadoNuevo,
            e.AnuladoAnterior,
            e.AnuladoNuevo,
            e.Observacion,
            v.ClienteId,
            ISNULL(c.NombreCliente, '') AS Cliente,
            v.TotalVenta,
            v.Glosa
        FROM dbo.VentasAuditoriaEventos AS e
        INNER JOIN dbo.Ventas AS v ON v.Id = e.VentaId
        LEFT JOIN dbo.Clientes AS c ON c.Id = v.ClienteId
        WHERE e.FechaHora >= @Desde
          AND e.FechaHora <  @Hasta
          AND (@tipoEvento IS NULL OR @tipoEvento = 'TODOS' OR e.TipoEvento = @tipoEvento)
        ORDER BY e.FechaHora DESC;
    END

    IF (@tipo = 2)
    BEGIN
        SELECT
            d.Id,
            d.EventoId,
            d.VentaId,
            d.ProductoId,
            ISNULL(p.NombreProducto, '') AS Producto,
            d.TipoCambio,
            d.CantidadAntes,
            d.CantidadDespues,
            d.PrecioAntes,
            d.PrecioDespues
        FROM dbo.VentasAuditoriaDetalle AS d
        LEFT JOIN dbo.Productos AS p ON p.Id = d.ProductoId
        WHERE d.EventoId = @EventoId
        ORDER BY
            CASE d.TipoCambio
                WHEN 'AGREGADO'            THEN 1
                WHEN 'CANTIDAD_MODIFICADA' THEN 2
                WHEN 'ELIMINADO'           THEN 3
                WHEN 'VIGENTE'             THEN 4
                ELSE 5
            END,
            Producto;
    END
END
GO

/* ============================================================================
   PRUEBA RAPIDA (ajusta las fechas a un dia donde ya tengas eventos):

   EXEC dbo.MAM_AuditoriaVentas @tipo = 1, @usuario = 'PRUEBA',
        @fechaDesde = '2026/09/01', @fechaHasta = '2026/09/21', @tipoEvento = 'TODOS';

   -- Con el Id que te devuelva la fila (columna "Id", NO "VentaId"):
   EXEC dbo.MAM_AuditoriaVentas @tipo = 2, @usuario = 'PRUEBA', @EventoId = 1;
   ============================================================================ */
