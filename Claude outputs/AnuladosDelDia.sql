/* ============================================================================
   REPORTE: Ventas Anuladas de un Día + verificación de detalle
   Base de datos : DistribucionDistralKCP2023
   Tablas         : Ventas, Clientes, Personal, VentasDetalles, VentasDetallesBackup
   Generado con Claude para Marco Mamani (BANCOSOL) a partir del análisis
   de MAM_Ventas (@tipo=13 "Anular Pedidos" / @tipo=18 "Revertir Anulacion")

   -----------------------------------------------------------------------
   POR QUÉ SOLO SE PUEDE FILTRAR POR DÍA (no por hora)
   -----------------------------------------------------------------------
   El SP MAM_Ventas, en la rama @tipo=13 ("Anular Pedidos"), solo hace:

        UPDATE Ventas SET Anulado = 1, FechaEntrega = @newFecha
        FROM Ventas JOIN @Asignacion ...

   No actualiza UsuarioRegistro ni FechaRegistro, y en el camino exitoso
   tampoco inserta nada en Bitacora (eso solo pasa en el CATCH). Es decir,
   la ÚNICA marca de "cuándo se anuló" que deja el sistema hoy es
   Ventas.FechaEntrega, y esa columna es:

        [FechaEntrega] [date] NULL      -- sin componente de hora

   Conclusión: SÍ se puede filtrar "anulados de tal día" por FechaEntrega.
   NO se puede filtrar por hora, porque ese dato nunca se guarda en
   ningún lado para la anulación con el esquema actual. Si el negocio
   necesita saber la hora exacta, hay que agregar esa columna al UPDATE
   del SP (por ejemplo un HoraAnulacion nvarchar(5) igual que el resto
   del sistema) — hoy el dato simplemente no existe.
   ============================================================================ */

DECLARE @FechaAnulacion DATE = '2026-04-27';   -- <-- cambiar aquí el día a consultar


-- 1) Ventas anuladas ese día -------------------------------------------------
IF OBJECT_ID('tempdb..#VentasAnuladasDelDia') IS NOT NULL DROP TABLE #VentasAnuladasDelDia;

SELECT
    v.Id                AS VentaId,
    v.FechaVenta,
    v.FechaEntrega       AS FechaAnulacion,     -- única marca de tiempo disponible
    v.ClienteId,
    cl.NombreCliente,
    v.PersonalId,
    p.NombrePersonal     AS Vendedor,
    v.TotalVenta,
    v.EstadoPedido,
    v.Anulado
INTO #VentasAnuladasDelDia
FROM Ventas v
INNER JOIN Clientes cl ON cl.Id = v.ClienteId
INNER JOIN Personal p  ON p.Id = v.PersonalId
WHERE v.Estado = 1                 -- descarta eliminadas (Estado = -1, ver conversación anterior)
  AND v.Anulado = 1                -- solo anuladas
  AND v.FechaEntrega = @FechaAnulacion;

SELECT * FROM #VentasAnuladasDelDia
ORDER BY VentaId DESC;


-- 2) Detalle VIGENTE de esas ventas -------------------------------------------
--    "Anular" NUNCA toca VentasDetalles (a diferencia de "Eliminar", que sí
--    la borra). Por lo tanto el detalle real de lo que se vendió sigue
--    intacto en la tabla viva: no hace falta ir al backup para ver qué
--    productos tenía una venta anulada.
SELECT
    vd.*
FROM VentasDetalles vd
WHERE vd.VentaId IN (SELECT VentaId FROM #VentasAnuladasDelDia)
ORDER BY vd.VentaId, vd.Id;


/* ============================================================================
   3) Última "foto" registrada en VentasDetallesBackup por venta
   -----------------------------------------------------------------------
   OJO - esto es un análisis defensivo, no una certeza, porque no tenemos
   el texto del trigger (no vino en el script exportado). Con los 9
   registros que ya revisamos para la venta 37978, el patrón observado es:

     - Al CREAR      -> un solo lote,  Accion = 'NUEVO'
     - Al MODIFICAR  -> siempre un PAR de lotes: 'Modificacion Antes'
                        (estado previo) + 'MODIFICACION DESPUES' (estado
                        nuevo), ambos con el mismo VentaId/FechaRegistro/
                        HoraRegistro dentro de cada lote.
     - No hay ningún registro para "Anular" (confirmado en el punto 2:
       Anular no toca VentasDetalles, así que el trigger nunca dispara).
     - NO sabemos si existe un tercer valor de Accion para cuando la venta
       se ELIMINA (Estado=-1), que sí borra físicamente VentasDetalles.

   Antes de confiar en este bloque para producción, correr:
        SELECT DISTINCT Accion FROM VentasDetallesBackup;
        SELECT name, OBJECT_DEFINITION(object_id)
        FROM sys.triggers WHERE OBJECT_NAME(parent_id) = 'VentasDetalles';
   para confirmar el catálogo real de Accion y la lógica exacta.

   RIESGO conocido de esta agrupación: HoraRegistro se guarda con
   precisión de minuto ('HH:MM', ver patrón @newHora=CONCAT(DATEPART(HOUR..)
   ,':',DATEPART(MINUTE..)) usado en todos los SP). Si la MISMA venta se
   edita dos veces dentro del mismo minuto, este agrupado podría mezclar
   dos lotes distintos. Para uso puntual (una venta a la vez) es
   suficiente; para un reporte masivo automático, conviene primero
   verificar que no haya colisiones.
   ============================================================================ */

;WITH UltimoLote AS (
    SELECT
        VentaId, FechaRegistro, HoraRegistro,
        ROW_NUMBER() OVER (PARTITION BY VentaId ORDER BY Id DESC) AS rn
    FROM VentasDetallesBackup
    WHERE VentaId IN (SELECT VentaId FROM #VentasAnuladasDelDia)
)
SELECT b.*
FROM VentasDetallesBackup b
INNER JOIN UltimoLote u
    ON u.VentaId = b.VentaId
   AND u.FechaRegistro = b.FechaRegistro
   AND u.HoraRegistro  = b.HoraRegistro
   AND u.rn = 1
ORDER BY b.VentaId, b.Id;


-- 3b) Versión simple: literalmente "el último registro insertado" por venta,
--     sin agrupar (por si solo quieres ver la fila con el mayor Id, tal cual
--     se pidió, sin el análisis de lote):
SELECT TOP 1 *
FROM VentasDetallesBackup
WHERE VentaId = 37978
ORDER BY Id DESC;

DROP TABLE #VentasAnuladasDelDia;
