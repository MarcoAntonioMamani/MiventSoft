/* =====================================================================
   MAM_Movimientos - nueva rama @tipo = 19
   Lista, por deposito, cada Producto/Lote/FechaVencimiento con stock <> 0
   para el boton "Resetear Inventario" de Tec_Movimientos.

   COMO APLICAR: abrir el ALTER PROCEDURE de MAM_Movimientos y pegar este
   bloque ANTES del ultimo "END" del procedimiento (despues de @tipo=18).
   Probar primero en una copia de la BD, no en produccion.
   ===================================================================== */

	IF @tipo=19 --Stock por lote distinto de cero (Reseteo de inventario)
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY

			SELECT  p.Id,
			        p.CodigoExterno,
			        p.NombreProducto,
			        stock.Lote,
			        stock.FechaVencimiento,
			        CAST(SUM(stock.Cantidad) AS decimal(18,2)) AS stock
			FROM ProductosStock AS stock
			INNER JOIN Productos AS p ON p.Id = stock.ProductoId
			WHERE stock.DepositoId = @DepositoId
			GROUP BY p.Id, p.CodigoExterno, p.NombreProducto, stock.Lote, stock.FechaVencimiento
			HAVING SUM(stock.Cantidad) <> 0
			ORDER BY p.NombreProducto, stock.FechaVencimiento

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),19,@newFecha,@newHora,@usuario )
		END CATCH
	END


/* =====================================================================
   VERIFICACION PREVIA (ejecutar por separado, NO dentro del SP)
   El reseteo graba cantidades NEGATIVAS en un movimiento de Ingreso (4).
   Hay que confirmar como se actualiza ProductosStock: buscar el trigger.
   ===================================================================== */
-- SELECT t.name AS TriggerName, OBJECT_NAME(t.parent_id) AS Tabla, t.is_disabled
-- FROM sys.triggers t
-- WHERE OBJECT_NAME(t.parent_id) IN ('MovimientosDetalle','Movimientos');
-- EXEC sp_helptext 'NombreDelTrigger';
--
-- Debe sumar  Cantidad * MovimientosTipos.Factor  (sin ABS, sin validar Cantidad > 0).
-- SELECT Id, Descripcion, Factor FROM MovimientosTipos WHERE Id = 4;   -- debe ser Factor = 1
