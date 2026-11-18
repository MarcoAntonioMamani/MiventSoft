/* SOLO PARA DESHACER 02_MAM_PanelControl.sql (el panel de Inicio deja de mostrar el analisis de clientes y productos). */
IF OBJECT_ID('dbo.MAM_PanelControl', 'P') IS NOT NULL
	DROP PROCEDURE [dbo].[MAM_PanelControl]
GO
RAISERROR('02 MAM_PanelControl QUITADO.', 10, 1) WITH NOWAIT
GO
