/* =====================================================================
   REVERTIR_09_PedidosObservados.sql  -  SOLO para deshacer 09_PedidosObservados.sql
   Borra el SP MAM_PedidosMovilObservados. La tabla PedidosMovilObservados y el programa btnPedidosObservados
   se CONSERVAN (historial de observaciones; el programa sin el SP no muestra nada).
   Sin el SP la API del celular sigue funcionando: solo deja de registrar observaciones.
   ===================================================================== */
IF OBJECT_ID('dbo.MAM_PedidosMovilObservados') IS NOT NULL
BEGIN
	DROP PROCEDURE dbo.MAM_PedidosMovilObservados
	RAISERROR('REVERTIR 09: SP MAM_PedidosMovilObservados borrado. Se conservan la tabla y el programa.', 10, 1) WITH NOWAIT
END
ELSE
	RAISERROR('REVERTIR 09: el SP MAM_PedidosMovilObservados no existe. No se modifico nada.', 10, 1) WITH NOWAIT
GO
