/* =====================================================================
   Indices para MAM_Ventas @tipo 3 (lista de ventas: Tec_Ventas del escritorio
   y Panel de control / detalle de la web). No cambia ningun SP ni resultado.

   Problema medido (KailiIndustrialLaPaz, 2026-11-12, SQL Server 2017 RTM, compat 110):
     MAM_Ventas @tipo 3, 30 dias  (464 ventas)  ->  3.1 a 4.0 s
     MAM_Ventas @tipo 3, 365 dias (4214 ventas) -> 35 a 42 s
   ~8 ms por venta: para CADA venta se recorre completa
     - VentasMonedaCobrada            (left join por VentaId)        sin indice -> 2.0 s / 464 ventas
     - CierreCajeroReferenciasModulos (select top 1 por ModuloId)    sin indice -> 0.9 s / 464 ventas
   Con un acceso por clave (prueba con HASH JOIN): 22 ms y 8 ms.

   INSTALAR: ejecutar este script completo (solo crea los indices si no existen).
   QUITAR:   DROP INDEX IX_VentasMonedaCobrada_VentaId ON dbo.VentasMonedaCobrada
             DROP INDEX IX_CierreCajeroRefMod_Modulo_ModuloId ON dbo.CierreCajeroReferenciasModulos
   ===================================================================== */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VentasMonedaCobrada_VentaId' AND object_id = OBJECT_ID('dbo.VentasMonedaCobrada'))
	CREATE NONCLUSTERED INDEX IX_VentasMonedaCobrada_VentaId
	ON dbo.VentasMonedaCobrada (VentaId)
	INCLUDE (MontoBs, MontoDolares, TarjetaBancaria, TransferenciaBancaria, TipoCambio)
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CierreCajeroRefMod_Modulo_ModuloId' AND object_id = OBJECT_ID('dbo.CierreCajeroReferenciasModulos'))
	CREATE NONCLUSTERED INDEX IX_CierreCajeroRefMod_Modulo_ModuloId
	ON dbo.CierreCajeroReferenciasModulos (Modulo, ModuloId)
	INCLUDE (CierreCajeroId)
GO
