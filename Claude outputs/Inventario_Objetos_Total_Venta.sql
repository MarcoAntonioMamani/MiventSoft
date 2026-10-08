/*=============================================================================
 INVENTARIO (SOLO LECTURA) - Objetos que calculan o leen el total de una venta
 Objetivo: saber en que lugares hay que incorporar el incremento guardado en
 dbo.VentasIncrementoAplicado.

 - NO modifica nada. Solo SELECT sobre el catalogo (sys.sql_modules).
 - Devuelve NOMBRES de objetos y banderas, no datos de clientes ni de negocio.
 - Ejecutar en el ambiente de PRUEBA primero.

 Como leer el resultado:
   LeeTotalVenta   = 1  -> usa el total GUARDADO (Ventas.TotalVenta).
                          Las ventas nuevas YA incluyen el incremento -> normalmente OK.
   RecalculaDetalle= 1  -> suma VentaDetalle (SUM(...Total...)) y resta Descuento.
                          NO incluye el incremento -> CANDIDATO A CAMBIAR
                          (agregar LEFT JOIN a VentasIncrementoAplicado).
   YaUsaIncremento = 1  -> ya referencia VentasIncrementoAplicado (los que parcheamos).
=============================================================================*/
SET NOCOUNT ON;

SELECT
    OBJECT_SCHEMA_NAME(m.object_id)                                   AS Esquema,
    OBJECT_NAME(m.object_id)                                          AS Objeto,
    o.type_desc                                                       AS Tipo,
    CASE WHEN m.definition LIKE '%TotalVenta%'                  THEN 1 ELSE 0 END AS LeeTotalVenta,
    CASE WHEN m.definition LIKE '%VentaDetalle%'                THEN 1 ELSE 0 END AS TocaVentaDetalle,
    CASE WHEN m.definition LIKE '%VentaDetalle%'
          AND (m.definition LIKE '%SUM(%')                      THEN 1 ELSE 0 END AS RecalculaDetalle,
    CASE WHEN m.definition LIKE '%Descuento%'                   THEN 1 ELSE 0 END AS UsaDescuento,
    CASE WHEN m.definition LIKE '%VentasIncrementoAplicado%'    THEN 1 ELSE 0 END AS YaUsaIncremento
FROM sys.sql_modules AS m
JOIN sys.objects     AS o ON o.object_id = m.object_id
WHERE o.is_ms_shipped = 0
  AND (   m.definition LIKE '%TotalVenta%'
       OR m.definition LIKE '%VentaDetalle%')
  AND OBJECT_NAME(m.object_id) NOT LIKE 'sp[_]diagram%'
ORDER BY RecalculaDetalle DESC, LeeTotalVenta DESC, Objeto;
