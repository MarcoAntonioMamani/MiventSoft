/* ============================================================================
   MAM_Ventas - PARCHE DE AUDITORIA (solo se AGREGA codigo, nada se quita)
   Base de datos: DistribucionDistralKCP2023
   Generado con Claude para Marco Mamani (BANCOSOL)
   Base: el ALTER PROCEDURE que Marco pego (Script Date: 21/09/2026 6:52:08)

   Requisito previo: correr primero AuditoriaVentas_CrearTablas.sql (crea
   dbo.VentasAuditoriaEventos y dbo.VentasAuditoriaDetalle). Sin esas dos
   tablas, este SP fallara al ejecutar los tipos -1, 13 y 2 (el error queda
   contenido por el TRY/CATCH de auditoria y cae en Bitacora, pero igual
   hay que crearlas para que la auditoria realmente sirva).

   QUE SE AGREGO (marcado en el cuerpo con --==== AUDITORIA (INICIO/FIN) ====):

   1) @tipo=-1 (Eliminar):
      - Snapshot de Estado/Anulado ANTES del UPDATE.
      - Justo antes del "delete from VentasDetalles" (para poder leerlo
        todavia vivo), inserta 1 evento TipoEvento='ELIMINADO' y el detalle
        COMPLETO de la venta con TipoCambio='VIGENTE' (foto del pedido tal
        como estaba al momento de eliminarlo).

   2) @tipo=13 (Anular Pedidos - proceso por lote via @Asignacion):
      - Antes del UPDATE, guarda en una tabla de memoria el Estado/Anulado
        anterior de CADA venta del lote.
      - Despues del UPDATE, inserta 1 evento TipoEvento='ANULADO' POR CADA
        venta anulada en ese lote (Anulado 0->1, Estado no cambia, tal
        como hace hoy este branch).

   3) @tipo=2 (Modificar):
      - Antes del DELETE FROM VentasDetalles, guarda copia del detalle
        VIGENTE (ProductoId/Cantidad/Precio) y de Estado/Anulado anteriores.
      - Despues de insertar el nuevo detalle, hace un FULL OUTER JOIN entre
        ese "antes" y el detalle nuevo (@VentaDetalleType) y clasifica cada
        producto como AGREGADO / ELIMINADO / CANTIDAD_MODIFICADA / SIN_CAMBIO.
      - SOLO si existe al menos una fila distinta de 'SIN_CAMBIO' (es decir,
        realmente cambio algun producto o cantidad - tal como pediste: NO
        contar como modificado un cambio que no toco cantidad/productos)
        inserta 1 evento TipoEvento='MODIFICADO' y el detalle COMPLETO
        (incluyendo las filas SIN_CAMBIO) para poder ver el pedido entero,
        no solo el delta.

   PATRON DE SEGURIDAD usado en los 3 puntos: cada bloque de auditoria va
   en su PROPIO BEGIN TRY/BEGIN CATCH, separado del TRY/CATCH de la
   operacion real. Si el INSERT de auditoria falla por lo que sea (tabla
   bloqueada, FK violado, etc.), el error se registra en Bitacora con el
   prefijo 'AUDITORIA TIPO X' y el flujo SIGUE - NUNCA hace rollback ni
   bloquea el Eliminar/Anular/Modificar real. Esto es intencional: la
   auditoria no debe poder tumbar una operacion de negocio.

   NO TOCADO: absolutamente ningun otro branch (@tipo=1,3,4,5,6,7,8,9,10,11,
   12,14,15,16,17,18,19,20,21,22,23,30,31) ni una sola linea de los branches
   -1/13/2 fuera de lo listado arriba. Se verifico con diff linea por linea
   contra el texto que pegaste: cero lineas eliminadas o modificadas, solo
   agregadas. Tambien se verifico el balance BEGIN/END/TRY/CATCH/CASE del
   archivo completo.

   PENDIENTE (bug ya reportado, no tocado aqui a proposito): @tipo=18
   "Revertir Anulacion" nunca resetea Estado (solo Anulado y EstadoPedido),
   por lo que revertir una anulacion que vino de sp_go_TC004_appMovil
   (que pone Estado=-1) deja la venta "eliminada" para siempre. Si quieres,
   lo corregimos en un script aparte.

   RECOMENDACION: probar este ALTER en un ambiente de desarrollo/pruebas
   antes de correrlo contra produccion.
   ============================================================================ */

USE [DistribucionDistralKCP2023]
GO
/****** Object:  StoredProcedure [dbo].[MAM_Ventas]    Script Date: 21/09/2026 6:52:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




--drop procedure MAM_Ventas 

ALTER  PROCEDURE [dbo].[MAM_Ventas](@tipo int,@Id int=-1,@SucursalId int=-1,@FechaVenta date=null,
@PersonalId int =-1,@TipoVenta int=-1,@FechaVencimientoCredito date=null,
@ClienteId int=-1,@MonedaVenta int=-1,@Estado int=-1,
@Glosa nvarchar(450)='',@Descuento decimal(18,2)=0,@TotalVenta decimal(18,2)=0
,@VentaDetalleType VentaDetalleType Readonly,@usuario nvarchar(50)='',@Ventadirecta int=-1,@EstadoPedido int=-1,@FechaEntregar date=null,@Asignacion AsignacionType Readonly,
@ChoferId int=-1,@Anulado int=0,@AsignacionEntregado AsignacionEntregadoType readonly,@AsignacionAnulacion AsignacionAnulacionType readonly
,@PrecioId int=-1,@ProveedorId int =-1,@CategoriaId int =-1,
@FechaI date=null,@FechaF date=null,@Descripcion nvarchar(250)='',@TipoMovimiento int=-1,@VentaDirectaSinConciliacion int=-1,@desde date=null,@hasta date=null,@IDConciliacion int=-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))
	Declare @NroConciliacion int
	Declare @Personal nvarchar(400)
	DECLARE @newFecha date
	set @newFecha=GETDATE()
	Declare @CategoriaCompra int,@CategoriaVenta int
	Declare @Paso nvarchar(400)
	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		BEGIN TRY
		  SET @Paso =
        'TIPO -1 - Eliminar';
			delete TransaccionVentasCredito where TransaccionVentasCredito .CreditoVentaId in (
			select Id
			from CreditosVentas where VentaId =@Id )
			delete from TransaccionesVentasCreditoDetalle where VentaCreditoId in  (
			select Id
			from CreditosVentas where VentaId =@Id )
			delete CreditosVentas where VentaId =@Id 
		
		    delete from PedidosAsignaciones where VentaId =@Id

			--==== AUDITORIA (INICIO) - snapshot de Estado/Anulado antes de eliminar ====
			DECLARE @Aud_EstadoAnterior_T1 INT, @Aud_AnuladoAnterior_T1 INT
			SELECT @Aud_EstadoAnterior_T1 = Estado, @Aud_AnuladoAnterior_T1 = Anulado FROM Ventas WHERE Id = @Id
			--==== AUDITORIA (FIN) ====

			Update Ventas set Estado =-1,UsuarioRegistro=@usuario,FechaRegistro=@newfecha where Id=@Id 

			--==== AUDITORIA (INICIO) - registrar ELIMINADO con foto completa del detalle vigente ====
			--    (se hace ANTES del delete de VentasDetalles para poder capturar el detalle)
			--    Aislado en su propio TRY/CATCH: si falla, NO afecta la eliminacion real.
			BEGIN TRY
				DECLARE @Aud_EventoId_T1 INT
				INSERT INTO VentasAuditoriaEventos
					(VentaId, TipoEvento, Origen, FechaHora, Usuario, EstadoAnterior, EstadoNuevo, AnuladoAnterior, AnuladoNuevo, Observacion)
				VALUES
					(@Id, 'ELIMINADO', 'DESKTOP', SYSDATETIME(), @usuario, @Aud_EstadoAnterior_T1, -1, @Aud_AnuladoAnterior_T1, @Aud_AnuladoAnterior_T1, NULL)
				SET @Aud_EventoId_T1 = SCOPE_IDENTITY()

				INSERT INTO VentasAuditoriaDetalle (EventoId, VentaId, ProductoId, TipoCambio, CantidadAntes, CantidadDespues, PrecioAntes, PrecioDespues)
				SELECT @Aud_EventoId_T1, @Id, vd.ProductoId, 'VIGENTE', vd.Cantidad, NULL, vd.Precio, NULL
				FROM VentasDetalles AS vd
				WHERE vd.VentaId = @Id
			END TRY
			BEGIN CATCH
				INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),
					   CONCAT('AUDITORIA TIPO -1 | VentaId: ',@Id,' | Usuario: ',@usuario,' | Error: ',ERROR_MESSAGE()),
					   -1,@newFecha,@newHora,@usuario)
			END CATCH
			--==== AUDITORIA (FIN) ====

			delete from VentasDetalles   where VentaId  =@Id;
			SELECT 1 as resp;
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(), CONCAT(
                'PASO: ', @Paso,
                ' | VentaId: ', @Id,
                ' | Usuario: ', @Usuario,
                ' | Error: ', ERROR_MESSAGE()
            ),-1,@newFecha,@newHora,@usuario)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY 
		  SET @Paso =
        'TIPO 1 - Registrar Venta';
		  set @Id=IIF((select COUNT(Id) from Ventas)=0,0,(select MAX(Id) from Ventas))+1
			INSERT INTO Ventas  VALUES(@Id ,@SucursalId ,@FechaVenta ,@PersonalId ,@TipoVenta ,
			@FechaVencimientoCredito ,@ClienteId ,@MonedaVenta ,@Estado ,@Glosa ,
			@Descuento ,@TotalVenta ,@newFecha,@newHora,@Usuario,@EstadoPedido ,0,@FechaEntregar,@Id,'SISTEMA',0,0,@Ventadirecta,@Anulado,@VentaDirectaSinConciliacion )

		

			
			INSERT INTO VentasDetalles (VentaId ,ProductoId ,Cantidad ,Precio ,SubTotal ,ProcentajeDescuento 
			,MontoDescuento, Total ,Detalle ,PrecioCosto ,Lote ,FechaVencimiento ,FechaRegistro ,HoraRegistro ,UsuarioRegistro,CantidadUnitaria)


			SELECT @Id,td.ProductoId ,td.CantidadUnitaria  ,td.Precio ,td.Subtotal ,td.PorcentajeDescuento ,td.MontoDescuento ,
			td.Total ,td.Detalle ,td.PrecioCosto ,td.Lote ,td.FechaVencimiento  ,
			@newFecha  ,@newHora  ,@Usuario,td.CantidadUnitaria    FROM @VentaDetalleType  AS td
			where td.estado  =0 and  td.ProductoId  >0 and td.CantidadUnitaria>0

			----Backup-----------
				set @Personal = (select NombrePersonal  from Personal where Id=@PersonalId )
		INSERT INTO VentasDetallesBackup (VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Programa)
		SELECT VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro,'NUEVO','SISTEMA'
		FROM VentasDetalles where VentaId =@Id 


			if (@Ventadirecta=1)
			begin
			

          set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@PersonalId ),0)

		  INSERT INTO PedidosAsignaciones(VentaId,PersonalId ,ConciliacionId ,Fecha ,Usuario )
		  SELECT @Id ,@PersonalId,@NroConciliacion,
			@newFecha ,@Usuario    
			UPDATE Ventas
			SET	EstadoPedido  = 4,FechaEntrega =Getdate()
			where id=@Id


			end



			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT @Id AS newNumi
			COMMIT TRAN INSERTAR
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),CONCAT(
                'PASO: ', @Paso,
                ' | VentaId: ', @Id,
                ' | Usuario: ', @Usuario,
                ' | Error: ', ERROR_MESSAGE()
            ),1,@newFecha,@newHora,@usuario )

			ROLLBACK TRAN INSERTAR
		END CATCH
	END
	IF @tipo=2--MODIFICACION
	BEGIN
		BEGIN TRY 
			begin tran Tr_InsertTI001
			  SET @Paso =
        'TIPO 2 - Modificar';
			set @Personal = (select NombrePersonal  from Personal where Id=@PersonalId )
		INSERT INTO VentasDetallesBackup (VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Programa)
		SELECT VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,@usuario ,'Modificacion Antes','SISTEMA'
		FROM VentasDetalles where VentaId =@Id 

			--==== AUDITORIA (INICIO) - snapshot del detalle y cabecera ANTES de modificar ====
			--    Se toma aqui porque VentasDetalles todavia no fue borrado.
			DECLARE @Aud_DetalleAntes_T2 TABLE (ProductoId INT, Cantidad DECIMAL(18,5), Precio DECIMAL(18,4))
			INSERT INTO @Aud_DetalleAntes_T2 (ProductoId, Cantidad, Precio)
			SELECT vd.ProductoId, vd.Cantidad, vd.Precio
			FROM VentasDetalles AS vd
			WHERE vd.VentaId = @Id

			DECLARE @Aud_EstadoAnterior_T2 INT, @Aud_AnuladoAnterior_T2 INT
			SELECT @Aud_EstadoAnterior_T2 = Estado, @Aud_AnuladoAnterior_T2 = Anulado FROM Ventas WHERE Id = @Id
			--==== AUDITORIA (FIN) ====

			--ELIMINO LOS REGISTROS
			DELETE FROM VentasDetalles   WHERE ventaid=@Id 

			Update Ventas set SucursalId =@SucursalId ,FechaVenta =@FechaVenta ,PersonalId =@PersonalId ,
			TipoVenta =@TipoVenta ,FechaVencimientoCredito =@FechaVencimientoCredito ,ClienteId =@ClienteId ,
			MonedaVenta =@MonedaVenta ,Estado =@Estado ,Glosa =@Glosa ,Descuento =@Descuento ,TotalVenta =@TotalVenta,
			VentaDirecta =@Ventadirecta,FechaEntrega =@FechaEntregar,Anulado=@Anulado,VentaDirectaSinConciliacion=@VentaDirectaSinConciliacion 
			where Id=@Id 

		 ----------MODIFICO EL DEcaLLE DE EQUIPO------------
			--INSERTO LOS NUEVOS

	

				INSERT INTO VentasDetalles (VentaId ,ProductoId ,Cantidad ,Precio ,SubTotal ,ProcentajeDescuento 
			,MontoDescuento, Total ,Detalle ,PrecioCosto ,Lote ,FechaVencimiento ,FechaRegistro ,HoraRegistro ,UsuarioRegistro,CantidadUnitaria)

			SELECT @Id,td.ProductoId ,td.CantidadUnitaria   ,td.Precio ,td.Subtotal ,td.PorcentajeDescuento ,td.MontoDescuento ,
			td.Total ,td.Detalle ,td.PrecioCosto ,td.Lote ,td.FechaVencimiento  ,
			@newFecha  ,@newHora  ,@Usuario,td.CantidadUnitaria    FROM @VentaDetalleType  AS td
			where td.estado  >=0 and  td.ProductoId  >0 and td.CantidadUnitaria >0

					set @Personal = (select NombrePersonal  from Personal where Id=@PersonalId )
		INSERT INTO VentasDetallesBackup (VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro,CantidadUnitaria,Accion,Programa)
		SELECT @Id,td.ProductoId ,td.CantidadUnitaria   ,td.Precio ,td.Subtotal ,td.PorcentajeDescuento ,td.MontoDescuento ,
			td.Total ,td.Detalle ,td.PrecioCosto ,td.Lote ,td.FechaVencimiento  ,
			@newFecha  ,@newHora  ,@Personal ,td.CantidadUnitaria,'Modificacion DESPUES','SISTEMA'
			FROM @VentaDetalleType  AS td
			where td.estado  >=0 and  td.ProductoId  >0
		

				------Modifico los Precios Costo------------------

			--==== AUDITORIA (INICIO) - diff DESPUES vs ANTES, solo registra si hay cambio real ====
			--    Criterio del negocio: solo cuenta como "modificado" si cambio la cantidad o
			--    se agrego/elimino algun producto - NO si solo cambio el precio u otro dato.
			--    Aislado en su propio TRY/CATCH: si falla, NO afecta la modificacion real.
			BEGIN TRY
				DECLARE @Aud_Diff_T2 TABLE (
					ProductoId INT, TipoCambio NVARCHAR(20),
					CantidadAntes DECIMAL(18,5), CantidadDespues DECIMAL(18,5),
					PrecioAntes DECIMAL(18,4), PrecioDespues DECIMAL(18,4))

				INSERT INTO @Aud_Diff_T2 (ProductoId, TipoCambio, CantidadAntes, CantidadDespues, PrecioAntes, PrecioDespues)
				SELECT
					COALESCE(a.ProductoId, d.ProductoId),
					CASE
						WHEN a.ProductoId IS NULL THEN 'AGREGADO'
						WHEN d.ProductoId IS NULL THEN 'ELIMINADO'
						WHEN a.Cantidad <> d.CantidadUnitaria THEN 'CANTIDAD_MODIFICADA'
						ELSE 'SIN_CAMBIO'
					END,
					a.Cantidad, d.CantidadUnitaria, a.Precio, d.Precio
				FROM @Aud_DetalleAntes_T2 AS a
				FULL OUTER JOIN (
					SELECT ProductoId, CantidadUnitaria, Precio
					FROM @VentaDetalleType
					WHERE estado >= 0 AND ProductoId > 0 AND CantidadUnitaria > 0
				) AS d ON d.ProductoId = a.ProductoId

				IF EXISTS (SELECT 1 FROM @Aud_Diff_T2 WHERE TipoCambio <> 'SIN_CAMBIO')
				BEGIN
					DECLARE @Aud_EventoId_T2 INT
					INSERT INTO VentasAuditoriaEventos
						(VentaId, TipoEvento, Origen, FechaHora, Usuario, EstadoAnterior, EstadoNuevo, AnuladoAnterior, AnuladoNuevo, Observacion)
					VALUES
						(@Id, 'MODIFICADO', 'DESKTOP', SYSDATETIME(), @usuario, @Aud_EstadoAnterior_T2, @Estado, @Aud_AnuladoAnterior_T2, @Anulado, NULL)
					SET @Aud_EventoId_T2 = SCOPE_IDENTITY()

					INSERT INTO VentasAuditoriaDetalle (EventoId, VentaId, ProductoId, TipoCambio, CantidadAntes, CantidadDespues, PrecioAntes, PrecioDespues)
					SELECT @Aud_EventoId_T2, @Id, ProductoId, TipoCambio, CantidadAntes, CantidadDespues, PrecioAntes, PrecioDespues
					FROM @Aud_Diff_T2
				END
			END TRY
			BEGIN CATCH
				INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),
					   CONCAT('AUDITORIA TIPO 2 | VentaId: ',@Id,' | Usuario: ',@Usuario,' | Error: ',ERROR_MESSAGE()),
					   2,@newFecha,@newHora,@Usuario)
			END CATCH
			--==== AUDITORIA (FIN) ====

					if (@Ventadirecta=1)
			begin
			

		  delete from PedidosAsignaciones where VentaId =@Id 

          set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@PersonalId ),0)

		  INSERT INTO PedidosAsignaciones(VentaId,PersonalId ,ConciliacionId ,Fecha ,Usuario )
		  SELECT @Id ,@PersonalId,@NroConciliacion,
			@newFecha ,@Usuario    
			UPDATE Ventas
			SET	EstadoPedido  = 4,FechaEntrega =Getdate()
			where id=@Id


			end
			select @Id as newNumi

			commit tran Tr_InsertTI001
		END TRY
		BEGIN CATCH
		    rollback tran Tr_InsertTI001
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),CONCAT(
                'PASO: ', @Paso,
                ' | VentaId: ', @Id,
                ' | Usuario: ', @Usuario,
                ' | Error: ', ERROR_MESSAGE()
            ),2,@newFecha,@newHora,@Usuario)
		END CATCH
	END

		IF @tipo=3 --MOSTrar Ventas
	BEGIN
		BEGIN TRY
		  SET @Paso =
        'TIPO 3 - Mostrar Ventas';
				
		select a.Id ,a.SucursalId ,a.FechaVenta ,a.PersonalId,p.NombrePersonal as Personal,
		a.TipoVenta,IIF(a.TipoVenta =1,'Contado','Credito')as TVenta,a.FechaVencimientoCredito ,
		a.ClienteId ,c.NombreCliente ,a.MonedaVenta ,a.Estado ,a.Glosa ,a.Descuento ,a.TotalVenta,a.EstadoPedido,est.NombreEstado ,
		a.VentaDirecta,a.FechaEntrega,isnull( per.NombrePersonal, 'No Asignado') as NombrePersonal,a.Anulado,IIF(a.Anulado=1,'Pedido Anulado','Pedido Activo') as EstadoAnulado,
		a.VentaDirectaSinConciliacion,Isnull(con.Estado,1) as EstadoConciliacion,isnull(con.Id,0) as conciliacionId,a.HoraRegistro,
    ROUND(
        geography::Point(a.Latitud, a.Longitud, 4326).STDistance(
            geography::Point(c.Latitud, c.Longitud, 4326)
        ) / 1000, 2
    ) AS DistanciaKm
		from Ventas as a 
		inner join Personal as p on p.Id =a.PersonalId 
		inner join Clientes as c on c.Id =a.ClienteId 
		left join PedidosAsignaciones as asig on asig.VentaId =a.Id
		left join Conciliaciones as con on con.Id =asig.ConciliacionId  
		left join Personal as per on per.Id =asig .PersonalId
		inner join PedidosEstados as est on est.Id =a.EstadoPedido  
		where a.Estado =1 and a.FechaVenta>=@desde and a.FechaVenta <=@hasta 
		order by a.Id desc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),CONCAT(
                'PASO: ', @Paso,
                ' | Usuario: ', @Usuario,
                ' | Error: ', ERROR_MESSAGE()
            ),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=4 --Listar Detalle Venta
	BEGIN
		BEGIN TRY
		  SET @Paso =
        'TIPO 4 - Mostrar Ventas Detalles';
		select a.Id ,a.VentaId ,a.ProductoId ,p.DescripcionProducto as Producto,cast(a.Cantidad as decimal (18,5))/p.Conversion  as Cantidad,cast((a.Cantidad  ) as decimal(18,5)) as CantidadUnitaria,a.Precio ,a.SubTotal ,
		a.ProcentajeDescuento ,a.MontoDescuento ,a.Total ,a.Detalle ,a.PrecioCosto ,a.Lote ,a.FechaVencimiento ,
		1 as estado,cast ('' as image ) as img
		,(select sum(st.Cantidad ) from ProductosStock as st
		 where st.ProductoId =p.Id and st.Lote =a.Lote and  st.FechaVencimiento =a.FechaVencimiento )+a.Cantidad  as stock,p.Conversion 
		from VentasDetalles as a
		inner join Productos as p on p.Id =a.ProductoId 
		where a.VentaId =@Id 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),CONCAT(
                'PASO: ', @Paso,
                ' | VentaId: ', @Id,
                ' | Usuario: ', @Usuario,
                ' | Error: ', ERROR_MESSAGE()
            ),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=5 --MOSTRaR Productos Para Venta
	BEGIN
		BEGIN TRY
	  SET @Paso =
        'TIPO 5 - Mostrar Productos PAra Ventas';
	
	 

	 set @CategoriaCompra =(select Min(k.Id ) from PreciosCategorias  as k where k.Tipo  =0)
	 set @CategoriaVenta =@PrecioId

	 select a.Id ,a.CodigoExterno ,a.DescripcionProducto as NombreProducto,a.DescripcionProducto,cat.NombreCategoria   ,PCosto .Precio as PrecioCosto,PVenta .Precio as PrecioVenta,1 as estado,
	 (select Sum(st.Cantidad ) from ProductosStock as st inner join
	 Almacenes as alma on  st.ProductoId =a.Id and st.DepositoId=alma .DepositoId and alma.id =@SucursalId     ) as stock,(select Sum(st.Cantidad ) from ProductosStock as st inner join
	 Almacenes as alma on  st.ProductoId =a.Id and st.DepositoId=alma .DepositoId and alma.id =@SucursalId     )/a.Conversion  as stockCajas,a.Conversion 
	 from Productos as a 
	 inner join Precios as PCosto on PCosto .PrecioCategoriaId =@CategoriaCompra 
	 and PCosto.ProductoId =a.Id 
	 inner join Precios as PVenta on PVenta .PrecioCategoriaId =@CategoriaVenta 
	 and PVenta .ProductoId =a.Id 
	 inner join Categorias as cat on cat.id=a.CategoriaId 
	 and PCosto .AlmacenId =@SucursalId 
	 and PVenta .AlmacenId =@SucursalId 
	 where a.estado=1
	 order by a.id asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),CONCAT(
                'PASO: ', @Paso,
                ' | Usuario: ', @Usuario,
                ' | Error: ', ERROR_MESSAGE()
            ),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=6 --Recibo de Venta
	BEGIN
		BEGIN TRY
		  SET @Paso =
        'TIPO 6 - Mostrar Recibo';
		SELECT '00000' + CAST(a.Id AS nvarchar(20)) AS nroRecibo,FORMAT (a.FechaVenta , 'dd-MM-yyyy') as FechaVenta, cli.NombreCliente, IIF(cli.DireccionCliente='','S/D', cli.DireccionCliente)as DireccionCliente
		, IIF(cli.NroDocumento='','0',cli.NroDocumento) as NroDocumento, IIF(cli.Telefono='','0',cli.Telefono)as Telefono, p.Id AS ProductoId, p.DescripcionProducto as NombreProducto, Sum(detalle.Cantidad) as Cantidad
		, Sum(detalle.Precio) as Precio, Sum(detalle.SubTotal) as SubTotal, 
                  Sum(detalle.MontoDescuento) as MontoDescuento, Sum(detalle.Total) as Total, empresa.Nombre AS Empresa, empresa.Direccion AS DireccionEmpresa, empresa.Ciudad, CAST('' AS image) AS imageEmpresa,
                      (SELECT TOP (1) Imagen
                       FROM      dbo.Empresa AS Empresa_1) AS rutaImagen, per.NombrePersonal AS vendedor, a.Glosa,
					   IIF(a.TipoVenta=1,'Contado','Credito') as TipoVenta ,IIF(a.TipoVenta=1,'',FORMAT (a.fechaVencimientoCredito , 'dd-MM-yyyy') )as FechaVencimientoCredito,a.Descuento as DescuentoVenta,
					   a.HoraRegistro 
FROM     dbo.Ventas AS a INNER JOIN
                  dbo.Clientes AS cli ON cli.Id = a.ClienteId INNER JOIN
                  dbo.VentasDetalles AS detalle ON detalle.VentaId = a.Id INNER JOIN
                  dbo.Productos AS p ON p.Id = detalle.ProductoId INNER JOIN
                  dbo.Empresa AS empresa ON empresa.Id = 1 INNER JOIN
                  dbo.Personal AS per ON per.Id = a.PersonalId
WHERE   a.Id =@Id 

group by a.Id ,a.FechaVenta ,cli .NombreCliente ,cli.DireccionCliente,cli.NroDocumento ,cli.Telefono ,p.Id ,p.DescripcionProducto,
empresa.Nombre ,empresa .Direccion,empresa .Ciudad,per.NombrePersonal,a.Glosa,a.TipoVenta,a.FechaVencimientoCredito,a.Descuento,a.HoraRegistro  

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),CONCAT(
                'PASO: ', @Paso,
                ' | VentaId: ', @Id,
                ' | Usuario: ', @Usuario,
                ' | Error: ', ERROR_MESSAGE()
            ),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

		IF @tipo=7 --Listar Pedidos Pendientes
	BEGIN
		BEGIN TRY
		
				
	select v.Id ,v.FechaVenta as FechaPedido,cl.NombreCliente,v.PersonalId ,p.NombrePersonal,v.TotalVenta as totalPedido,cast('' as bit ) as Asignar,cast('' as image) as detalle
from ventas as v
inner join Clientes as cl on cl.id=v.ClienteId 
inner join Personal as p on p.id=v.PersonalId 
where v.EstadoPedido in (1,2) and v.Id not in 
(select asig.VentaId 
 from PedidosAsignaciones as asig ) and v.Estado =1 and V.Anulado=0
 order by v.id desc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END
IF @tipo=8 --Registrar Asignaciones
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY 
		
			INSERT INTO PedidosAsignaciones(VentaId,PersonalId ,ConciliacionId ,Fecha ,Usuario )


			SELECT td.Id ,@PersonalId,0,
			@newFecha ,@Usuario    FROM @Asignacion   AS td
			where td.Asignar =1


			UPDATE Ventas
			SET	EstadoPedido  = 2
			FROM Ventas 
			JOIN @Asignacion AS mont ON mont.Asignar = 1 AND Ventas.Id  = mont.Id ;	

			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT @Id AS newNumi
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario )

			ROLLBACK TRAN
		END CATCH
	END

		IF @tipo=9--Listar Pedidos Asignados
	BEGIN
		BEGIN TRY
		
				
		select v.Id ,v.FechaVenta as FechaPedido,cl.NombreCliente,v.PersonalId ,p.NombrePersonal,v.TotalVenta as totalPedido,cast('' as bit ) as Asignar,cast('' as image) as detalle
from ventas as v
inner join Clientes as cl on cl.id=v.ClienteId 
inner join Personal as p on p.id=v.PersonalId 
where v.EstadoPedido  =2 and v.Id  in 
(select asig.VentaId 
 from PedidosAsignaciones as asig where asig.PersonalId =@ChoferId  ) and v.Estado =1 and V.Anulado=0
 --and v.FechaVenta =@FechaVenta 
 order by v.id desc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=10--Listar Reporte  Productos Asignados
	BEGIN
		BEGIN TRY
		
				
select p.Id as CodProducto,p.DescripcionProducto as NombreProducto ,cat.NombreCategoria ,Sum(detalle .Cantidad) as Cantidad,cast('' as image) as img
from Productos as p
inner join VentasDetalles as detalle
on detalle .ProductoId  =p.Id 
inner join Categorias as cat on cat.Id =p.CategoriaId 
inner join  Ventas as v on v.id=detalle .VentaId 
where v.id in 
(select asig.VentaId 
 from PedidosAsignaciones as asig where asig.PersonalId =@ChoferId  ) and v.EstadoPedido =2 and v.Estado =1 and V.Anulado=0
group by p.Id ,p.DescripcionProducto,cat.NombreCategoria

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=11--Listar Reporte  Clientes Asignados
	BEGIN
		BEGIN TRY
		
				
select cl.Id as CodigoCliente,cl.NombreCliente ,cl.Telefono ,cl.DireccionCliente,Sum(v.TotalVenta ) as TotalBs,cast('' as image) as img 
from Clientes as cl
inner join Ventas as v on v.ClienteId =cl.Id 
where v.id in 
(select asig.VentaId 
 from PedidosAsignaciones as asig where asig.PersonalId =@ChoferId  ) and v.EstadoPedido =2 and v.Estado =1 and V.Anulado=0
group by cl.Id ,cl.NombreCliente ,cl.Telefono ,cl.DireccionCliente



		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=12--Eliminar Asignados
	BEGIN
		BEGIN TRY
		
UPDATE Ventas
			SET	EstadoPedido  = 1
			FROM Ventas 
			JOIN @Asignacion AS mont ON mont.Asignar = 1 AND Ventas.Id  = mont.Id ;			

delete from PedidosAsignaciones where VentaId  in (
select a.Id  from @Asignacion as a where a.Asignar =1)


SELECT @Id AS newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=13--Anular Pedidos
	BEGIN
		BEGIN TRY
		
		--==== AUDITORIA (INICIO) - snapshot ANTES de anular (puede ser un lote de varias ventas) ====
		DECLARE @Aud_AntesAnular_T13 TABLE (VentaId INT, EstadoAnterior INT, AnuladoAnterior INT)
		INSERT INTO @Aud_AntesAnular_T13 (VentaId, EstadoAnterior, AnuladoAnterior)
		SELECT v.Id, v.Estado, v.Anulado
		FROM Ventas AS v
		JOIN @Asignacion AS mont ON mont.Asignar = 1 AND v.Id = mont.Id
		--==== AUDITORIA (FIN) ====

UPDATE Ventas
			SET	 Anulado=1,FechaEntrega =@newFecha 
			FROM Ventas 
			JOIN @Asignacion AS mont ON mont.Asignar = 1 AND Ventas.Id  = mont.Id ;			

		--==== AUDITORIA (INICIO) - un evento ANULADO por cada venta del lote ====
		--    Aislado en su propio TRY/CATCH: si falla, NO afecta la anulacion real.
		BEGIN TRY
			INSERT INTO VentasAuditoriaEventos
				(VentaId, TipoEvento, Origen, FechaHora, Usuario, EstadoAnterior, EstadoNuevo, AnuladoAnterior, AnuladoNuevo, Observacion)
			SELECT a.VentaId, 'ANULADO', 'DESKTOP', SYSDATETIME(), @usuario, a.EstadoAnterior, a.EstadoAnterior, a.AnuladoAnterior, 1, NULL
			FROM @Aud_AntesAnular_T13 AS a
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),
				   CONCAT('AUDITORIA TIPO 13 | Usuario: ',@usuario,' | Error: ',ERROR_MESSAGE()),
				   3,@newFecha,@newHora,@Usuario)
		END CATCH
		--==== AUDITORIA (FIN) ====

delete from PedidosAsignaciones where VentaId  in (
select a.Id  from @Asignacion as a where a.Asignar =1)


SELECT @Id AS newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END
IF @tipo=14--EntregarPedidos
	BEGIN
		BEGIN TRY
		


set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@ChoferId ),0)

if (@NroConciliacion >0)
begin
UPDATE Ventas
			SET	EstadoPedido  = 4,FechaEntrega =Getdate()
			FROM Ventas 
			inner JOIN @Asignacion AS mont ON mont.Asignar = 1 AND Ventas.Id  = mont.Id ;

update PedidosAsignaciones set PedidosAsignaciones.ConciliacionId =@NroConciliacion
from PedidosAsignaciones 
inner join @Asignacion as asig on asig .id=PedidosAsignaciones .VentaId and asig.Asignar =1
SELECT 1 AS newNumi
end
else

begin
SELECT -1 AS newNumi
end


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

		IF @tipo=15--Listar Pedidos Entregados
	BEGIN
		BEGIN TRY
		
				
select v.Id,asig.ConciliacionId,'Conciliacion #'+cast(conci.Id as nvarchar(20)) +' '+FORMAT(conci.Fecha,'dd/MM/yyyy') as conciliacion ,v.FechaVenta as FechaPedido,cl.NombreCliente,v.PersonalId ,p.NombrePersonal,v.TotalVenta as totalPedido,cast('' as bit ) as Asignar,cast('' as image) as detalle,
(select con.Estado  from PedidosAsignaciones as aa inner join Conciliaciones as con on con.Id =aa.ConciliacionId and aa.VentaId =v.Id ) as EstadoConciliacion
from ventas as v
inner join Clientes as cl on cl.id=v.ClienteId 
inner join Personal as p on p.id=v.PersonalId 
inner join PedidosAsignaciones as asig on asig.VentaId =v.Id 
and asig.PersonalId =@ChoferId  and v.Estado =1 and V.Anulado=0 and v.EstadoPedido  =4 
inner join Conciliaciones as conci on conci.Id=asig.ConciliacionId 
where conci.Id =@IDConciliacion 
 order by asig.ConciliacionId  desc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=16--Revertir Estado
	BEGIN
		BEGIN TRY
		
UPDATE Ventas
			SET	EstadoPedido  = 2 
			FROM Ventas 
			JOIN @AsignacionEntregado AS mont ON mont.Asignar = 1 AND Ventas.Id  = mont.Id ;			

update PedidosAsignaciones set PedidosAsignaciones.ConciliacionId =0
from PedidosAsignaciones 
inner join @AsignacionEntregado as asig on asig .id=PedidosAsignaciones .VentaId and asig.Asignar =1

SELECT @Id AS newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END
	IF @tipo=17--Listar Pedidos Anulados
	BEGIN
		BEGIN TRY
		
				
select top 100 v.Id ,v.FechaVenta as FechaPedido,v.FechaEntrega as FechaAnulacion,cl.NombreCliente,v.PersonalId ,p.NombrePersonal,v.TotalVenta as totalPedido,cast('' as bit ) as Asignar,cast('' as image) as detalle
from ventas as v
inner join Clientes as cl on cl.id=v.ClienteId 
inner join Personal as p on p.id=v.PersonalId 

where v.Estado =1 and V.Anulado=1
order by v.id desc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=18--Revertir Anulacion
	BEGIN
		BEGIN TRY
		
UPDATE Ventas
			SET	EstadoPedido  = 1, Anulado =0,FechaEntrega =FechaVenta 
			FROM Ventas 
			JOIN @AsignacionAnulacion AS mont ON mont.Asignar = 1 AND Ventas.Id  = mont.Id ;			



SELECT @Id AS newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END


IF @tipo=19--Verificar Si tiene Conciliacion Abierta
	BEGIN
		BEGIN TRY
		
select isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@ChoferId ),0) as Conciliacion



		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=20--Productos para vender del distribuidor
	BEGIN
		BEGIN TRY
		
Declare @ConciliacionId int


set @ConciliacionId =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@ChoferId ),0)


	   SET @Paso =
        'TIPO 20 - Productos PAra El Distribuidor';

	 set @CategoriaCompra =(select Min(k.Id ) from PreciosCategorias  as k where k.Tipo  =0)
	 set @CategoriaVenta =@PrecioId

select a.Id ,a.CodigoExterno ,a.DescripcionProducto as NombreProducto,a.DescripcionProducto,cat.NombreCategoria   ,PCosto .Precio as PrecioCosto,PVenta .Precio as PrecioVenta,1 as estado,
	 (sum(detalle.Cantidad ) -(
	 select isnull(sum(detalleV.Cantidad ),0) from PedidosAsignaciones as asig inner join VentasDetalles as detalleV on detalleV.VentaId=asig.VentaId and detalleV.ProductoId=a.id
	 and (asig.ConciliacionId =@ConciliacionId or asig.ConciliacionId =0) 
	 and asig.PersonalId=(select cc.PersonalID   from Conciliaciones as cc where cc.Id=@ConciliacionId ))) as stock,(sum(detalle.Cantidad ) -(
	 select isnull(sum(detalleV.Cantidad ),0) from PedidosAsignaciones as asig inner join VentasDetalles as detalleV on detalleV.VentaId=asig.VentaId and detalleV.ProductoId=a.id
	 and (asig.ConciliacionId =@ConciliacionId or asig.ConciliacionId =0) 
	 and asig.PersonalId=(select cc.PersonalID   from Conciliaciones as cc where cc.Id=@ConciliacionId )))/a.Conversion  as stockCajas,a.Conversion 
from DespachoProductos as desp 
inner join DespachoProductosDetalle as detalle on desp.ConciliacionId=@ConciliacionId and desp.id=detalle.DespachoProductosId 
inner join Productos as a on  a.id=detalle.ProductoId 
 inner join Precios as PCosto on PCosto .PrecioCategoriaId =@CategoriaCompra 
	 and PCosto.ProductoId =a.Id 
	 inner join Precios as PVenta on PVenta .PrecioCategoriaId =@CategoriaVenta 
	 and PVenta .ProductoId =a.Id 
	 inner join Categorias as cat on cat.id=a.CategoriaId 
	 and PCosto .AlmacenId =1 
	 and PVenta .AlmacenId =1 
	group by a.Id ,a.CodigoExterno ,a.NombreProducto,a.DescripcionProducto,cat.NombreCategoria   ,PCosto .Precio ,PVenta .Precio,a.Conversion 


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),CONCAT(
                'PASO: ', @Paso,
                ' | Usuario: ', @Usuario,
                ' | Error: ', ERROR_MESSAGE()
            ),3,@newFecha,@newHora,@Usuario)
		END CATCH

END


		IF @tipo=21 --Insertar tipo Movimiento
	BEGIN
	SET ARITHABORT ON
		BEGIN TRAN INSERTAR
		BEGIN TRY 
		  set @Id=IIF((select COUNT(Id) from CajaTipoMovimiento)=0,0,(select MAX(Id) from CajaTipoMovimiento))+1
			
		insert into CajaTipoMovimiento values(@Id,@Descripcion,@TipoMovimiento ,IIF(@TipoMovimiento=1,1,-1),1 )
			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT @Id AS newNumi
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@usuario )

			ROLLBACK TRAN
		END CATCH
	END
		IF @tipo=22--Listar Conciliacion
	BEGIN
		BEGIN TRY
		
				
select conci.id,
'Conciliacion #'+cast(conci.Id as nvarchar(20)) +' '+FORMAT(conci.Fecha,'dd/MM/yyyy') as conciliacion
from  Conciliaciones as conci  
 order by conci.id  desc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END
		IF @tipo=23--Listar ventas
	BEGIN
		BEGIN TRY
		
				
select v.id,v.FechaVenta,v.ClienteId,cl.NombreCliente ,v.TotalVenta,v.HoraRegistro,v.Latitud,v.Longitud,
    ROUND(
        geography::Point(v.Latitud, v.Longitud, 4326).STDistance(
            geography::Point(cl.Latitud, cl.Longitud, 4326)
        ) / 1000, 2
    ) AS DistanciaKm
from ventas as v
inner join Clientes as cl on cl.id=v.ClienteId 
where v.FechaVenta =@FechaVenta  and v.PersonalId =@PersonalId 
order by v.id asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=30--Listar Categoria
	BEGIN
		BEGIN TRY
		
				
select a.Id,a.NombreCategoria 
from Categorias as a

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END


	IF @tipo=31 ----Listar ventas por productos 
	BEGIN
		BEGIN TRY
		
				
SELECT  
    ve.ClienteId,
    cl.NombreCliente,
    p.Id AS ProductoId,
    p.CodigoExterno,
    prov.NombreProveedor,
    p.DescripcionProducto AS NombreProducto,
    cla.NombreCategoria AS Categoria,
    unidad.Descripcion AS Unidad,
    SUM(detalle.Cantidad) AS CantidadUni,
    CAST(SUM(detalle.Cantidad) / p.Conversion AS DECIMAL(18,2)) AS CantidadCaja,
    SUM(detalle.Total) AS TotalVenta,
    p.Conversion
FROM Ventas AS ve
INNER JOIN VentasDetalles AS detalle 
    ON ve.Id = detalle.VentaId
INNER JOIN Productos AS p 
    ON detalle.ProductoId = p.Id
INNER JOIN Categorias AS cla 
    ON cla.Id = p.CategoriaId
INNER JOIN ClasificadorDetalle AS unidad 
    ON unidad.Id = p.UnidadVentaId
INNER JOIN Proveedor AS prov 
    ON prov.Id = p.ProveedorId
INNER JOIN Clientes AS cl 
    ON cl.Id = ve.ClienteId
WHERE 
    ve.Estado = 1 
    AND ve.Anulado = 0

    AND (@ProveedorId = -1 OR prov.Id = @ProveedorId)
    AND (@CategoriaId = -1 OR cla.Id = @CategoriaId)
    AND (@ClienteId = -1 OR cl.Id = @ClienteId)
	and ve.FechaVenta >=@FechaI and ve.FechaVenta <=@FechaF
GROUP BY 
    ve.ClienteId,
    cl.NombreCliente,
    p.Id,
    p.CodigoExterno,
    prov.NombreProveedor,
    p.DescripcionProducto,
    cla.NombreCategoria,
    unidad.Descripcion,
    p.Conversion
	order by CantidadUni desc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

END