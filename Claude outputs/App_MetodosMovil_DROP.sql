/* ============================================================================
   App_MetodosMovil - BAJA (procedimiento confirmado sin uso)
   Base de datos: DistribucionDistralKCP2023
   Generado con Claude para Marco Mamani (BANCOSOL)

   POR QUE SE DA DE BAJA:
   - Marco confirmo que ya no se usa.
   - Ningun otro objeto de la base lo llama (EXEC App_MetodosMovil no aparece
     en ningun SP/vista del schema exportado).
   - Su rama de insertar pedido (@tipo=26/40) ya ni siquiera es compatible
     con el esquema actual de Ventas: hace "INSERT INTO Ventas VALUES(...)"
     con 24 valores posicionales, y la tabla Ventas hoy tiene 25 columnas
     -> esa rama fallaria con error de "numero de columnas no coincide" si
     alguien la llamara hoy. Es evidencia de que quedo obsoleto cuando el
     esquema evoluciono (probablemente reemplazado por sp_go_TC004_appMovil,
     que si trae exactamente 25 valores y coincide con la tabla viva).
   - Sus ramas @tipo=28 (cambia Ventas.Estado sin ninguna validacion, y con
     un bug: "@observacion=@oaobs" no actualiza ninguna columna real) y
     @tipo=29 (borra y reinserta VentasDetalles) SI seguirian funcionando
     si alguien las llamara, por fuera de la auditoria nueva - por eso se
     recomienda dar de baja el procedimiento completo, no solo dejarlo ahi.

   COMO USAR ESTE SCRIPT:
   1) Corre solo el PASO 1 primero y guarda el resultado (el texto completo
      del procedimiento) en algun lado por si alguna vez hace falta
      consultarlo - es tu respaldo antes de borrar.
   2) Recien despues, si estas de acuerdo, descomenta y corre el PASO 2
      (el DROP esta comentado a proposito, no se ejecuta solo).
   3) Recomendado: probar esto primero en un ambiente de desarrollo/pruebas
      antes de correrlo contra produccion.
   ============================================================================ */

-- ============================================================================
-- PASO 1: respaldo del texto del procedimiento antes de borrarlo
-- ============================================================================
SELECT OBJECT_DEFINITION(OBJECT_ID('dbo.App_MetodosMovil')) AS DefinicionActual;

-- (opcional, si prefieres dejar el respaldo guardado en una tabla en vez de
--  copiar el resultado a mano):
-- IF OBJECT_ID('dbo.SP_Respaldos', 'U') IS NULL
-- BEGIN
--     CREATE TABLE dbo.SP_Respaldos (
--         Id INT IDENTITY(1,1) PRIMARY KEY,
--         NombreObjeto NVARCHAR(200),
--         Definicion NVARCHAR(MAX),
--         FechaRespaldo DATETIME2(3) DEFAULT SYSDATETIME(),
--         Motivo NVARCHAR(400)
--     );
-- END
-- INSERT INTO dbo.SP_Respaldos (NombreObjeto, Definicion, Motivo)
-- SELECT 'App_MetodosMovil', OBJECT_DEFINITION(OBJECT_ID('dbo.App_MetodosMovil')),
--        'Confirmado sin uso por Marco Mamani (21/09/2026). Reemplazado por sp_go_TC004_appMovil.';


-- ============================================================================
-- PASO 2: baja del procedimiento (COMENTADO A PROPOSITO - descomentar cuando
-- estes listo para ejecutarlo)
-- ============================================================================
-- IF OBJECT_ID('dbo.App_MetodosMovil', 'P') IS NOT NULL
-- BEGIN
--     DROP PROCEDURE dbo.App_MetodosMovil;
--     PRINT 'App_MetodosMovil eliminado.';
-- END
-- ELSE
-- BEGIN
--     PRINT 'App_MetodosMovil ya no existe (nada que hacer).';
-- END
