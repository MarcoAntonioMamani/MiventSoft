/* =====================================================================
   SOLO PARA DESHACER 07_Auditoria.sql. NO es parte de la instalacion: NO ejecutar al instalar.
   Ejecutar DESPUES de REVERTIR_08_Auditoria_SPs.sql (si los SP todavia llaman a MAM_Auditoria, no hace nada).
   Quita el SP MAM_Auditoria y el programa "Auditoria De Registros" (btnAuditoriaRegistros) de los roles.
   La tabla AuditoriaRegistros y su historial NO se borran. Para borrarlos (no recomendado), al final:
       DROP TABLE dbo.AuditoriaRegistros
   ===================================================================== */
SET NOEXEC OFF
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas')) LIKE '%Kaily: auditoria%'
   OR OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Compras')) LIKE '%Kaily: auditoria%'
   OR OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Movimientos')) LIKE '%Kaily: auditoria%'
BEGIN
	RAISERROR('Primero ejecute REVERTIR_08_Auditoria_SPs.sql: los SP todavia llaman a MAM_Auditoria. No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
GO
SET XACT_ABORT ON
BEGIN TRAN
IF OBJECT_ID('dbo.MAM_Auditoria', 'P') IS NOT NULL
	DROP PROCEDURE dbo.MAM_Auditoria
DELETE FROM RolesDetalles WHERE ProgramaId IN (SELECT Id FROM Programas WHERE IdPrograma = 'btnAuditoriaRegistros')
DELETE FROM Programas WHERE IdPrograma = 'btnAuditoriaRegistros'
COMMIT TRAN
RAISERROR('07 Auditoria DESHECHO (la tabla AuditoriaRegistros se conserva).', 10, 1) WITH NOWAIT
GO
SET NOEXEC OFF
GO
