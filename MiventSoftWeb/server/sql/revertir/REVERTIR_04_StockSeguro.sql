/* =====================================================================
   SOLO PARA DESHACER 04_StockSeguro.sql. NO es parte de la instalacion: NO ejecutar al instalar.
   Vuelve MAM_Ventas, MAM_Movimientos, sp_go_TC004_appMovil, Trigger_MovimientoInsertar, Trigger_MovimientoDetalleEliminar, Trigger_MovimientosDetalleModificar de DistribucionDistralKCP2023
   a la version anterior (sin validacion de stock). Si StockSeguro no esta instalado, avisa y no hace nada.
   Si el 05 esta instalado, deshacer primero el 05 (no depende de este, pero se deshace en orden inverso).
   ===================================================================== */
SET NOEXEC OFF
GO
IF NOT EXISTS (SELECT 1 FROM (VALUES ('MAM_Ventas'), ('MAM_Movimientos'), ('sp_go_TC004_appMovil'), ('Trigger_MovimientoInsertar'), ('Trigger_MovimientoDetalleEliminar'), ('Trigger_MovimientosDetalleModificar')) AS v(n) WHERE (OBJECT_DEFINITION(OBJECT_ID('dbo.' + v.n)) LIKE '%Sanbit: validacion de stock%' OR OBJECT_DEFINITION(OBJECT_ID('dbo.' + v.n)) LIKE '%Sanbit: saldo en una sola sentencia%'))
BEGIN
	RAISERROR('04 StockSeguro NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
--drop procedure MAM_Ventas 

ALTER PROCEDURE [dbo].[MAM_Ventas](@tipo int,@Id int=-1,@SucursalId int=-1,@FechaVenta date=null,
@PersonalId int =-1,@TipoVenta int=-1,@FechaVencimientoCredito date=null,
@ClienteId int=-1,@MonedaVenta int=-1,@Estado int=-1,
@Glosa nvarchar(450)='',@Descuento decimal(18,2)=0,@TotalVenta decimal(18,2)=0
,@VentaDetalleType VentaDetalleType Readonly,@usuario nvarchar(50)='',@Ventadirecta int=-1,@EstadoPedido int=-1,@FechaEntregar date=null,@Asignacion AsignacionType Readonly,
@ChoferId int=-1,@Anulado int=0,@AsignacionEntregado AsignacionEntregadoType readonly,@AsignacionAnulacion AsignacionAnulacionType readonly
,@PrecioId int=-1,@ProveedorId int =-1,@CategoriaId int =-1,
@FechaI date=null,@FechaF date=null,@Descripcion nvarchar(250)='',@TipoMovimiento int=-1,@VentaDirectaSinConciliacion int=-1,@desde date=null,@hasta date=null,@IDConciliacion int=-1
,@IncrementoVenta decimal(18,2)=0,@IdProducto int =-1)
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

			--==== INCREMENTO (INICIO) - registrar el monto de incremento diario aplicado a ESTA venta ====
			--    Aislado en su propio TRY/CATCH: si por lo que sea falla, NO afecta el registro de la venta real.
			BEGIN TRY
				INSERT INTO VentasIncrementoAplicado (VentaId, Monto, FechaHoraRegistro)
				VALUES (@Id, ISNULL(@IncrementoVenta, 0), SYSDATETIME())
			END TRY
			BEGIN CATCH
				INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
					   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),
					   CONCAT('INCREMENTO TIPO 1 | VentaId: ',@Id,' | Usuario: ',@usuario,' | Error: ',ERROR_MESSAGE()),
					   1,@newFecha,@newHora,@usuario)
			END CATCH
			--==== INCREMENTO (FIN) ====




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
    ) AS DistanciaKm, ISNULL(inc.Monto, 0) AS IncrementoVenta
		from Ventas as a
		inner join Personal as p on p.Id =a.PersonalId
		inner join Clientes as c on c.Id =a.ClienteId
		left join PedidosAsignaciones as asig on asig.VentaId =a.Id
		left join Conciliaciones as con on con.Id =asig.ConciliacionId
		left join Personal as per on per.Id =asig .PersonalId
		inner join PedidosEstados as est on est.Id =a.EstadoPedido
		left join dbo.VentasIncrementoAplicado as inc on inc.VentaId = a.Id
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
					   a.HoraRegistro, ISNULL(inc.Monto, 0) AS IncrementoVenta
FROM     dbo.Ventas AS a INNER JOIN
                  dbo.Clientes AS cli ON cli.Id = a.ClienteId INNER JOIN
                  dbo.VentasDetalles AS detalle ON detalle.VentaId = a.Id INNER JOIN
                  dbo.Productos AS p ON p.Id = detalle.ProductoId INNER JOIN
                  dbo.Empresa AS empresa ON empresa.Id = 1 INNER JOIN
                  dbo.Personal AS per ON per.Id = a.PersonalId LEFT JOIN
                  dbo.VentasIncrementoAplicado AS inc ON inc.VentaId = a.Id
WHERE   a.Id =@Id

group by a.Id ,a.FechaVenta ,cli .NombreCliente ,cli.DireccionCliente,cli.NroDocumento ,cli.Telefono ,p.Id ,p.DescripcionProducto,
empresa.Nombre ,empresa .Direccion,empresa .Ciudad,per.NombrePersonal,a.Glosa,a.TipoVenta,a.FechaVencimientoCredito,a.Descuento,a.HoraRegistro,inc.Monto

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
	    AND (@IdProducto = -1 OR p.Id = @IdProducto)
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
GO
--drop procedure MAM_Movimientos
ALTER PROCEDURE [dbo].[MAM_Movimientos](@tipo int, @id int=-1, @ConceptoId int=-1,@TransaccionId int=-1,@DepositoId int=-1,
                                    @Observacion nvarchar(250)='',@Estado int=-1,@FechaDocumento date=null, 
									@usuario nvarchar(10)='',
									@detalle MovimientoDetalleType Readonly,@ProductoId int=-1,@FechaI date=null,@FechaF date=null,@CategoriaPrecio int=-1,@ProveedorId int=-1)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()
	
	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		BEGIN TRAN ELIMINAR 
		BEGIN TRY 
			DELETE FROM MovimientosDetalle  WHERE MovimientoId =@id
			DELETE FROM Movimientos  WHERE Id =@id
			SELECT @id AS newNumi
			COMMIT TRAN ELIMINAR
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), -1, @newFecha, @newHora, @usuario)
			ROLLBACK TRAN ELMINAR
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY 
			set @id=IIF((select COUNT(id) from Movimientos)= 0, 0, (select MAX(id) from Movimientos))+1

			INSERT INTO Movimientos  VALUES(@id, @ConceptoId , @id , @DepositoId , @Observacion , @Estado , @FechaDocumento ,
									 @newFecha, @newHora, @usuario)
			
			-- INSERTO EL DETALLE 
			INSERT INTO MovimientosDetalle (MovimientoId , ProductoId , Cantidad ,Lote ,FechaVencimiento )
			SELECT @id,td.ProductoId , td.Cantidad , td.Lote ,td.FechaVencimiento 
				FROM @detalle AS td
				WHERE td.ProductoId >0 and td.estado >=0;

			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT @id AS newNumi
			COMMIT TRAN INSERTAR
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 1, @newFecha, @newHora, @usuario)

			ROLLBACK TRAN INSERTAR
		END CATCH
	END
	
	IF @tipo=2--MODIFICACION
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY 

			--MODIFICO EL DETALLE
			DELETE FROM MovimientosDetalle  WHERE MovimientoId =@id;

			UPDATE Movimientos  SET ConceptoId =@ConceptoId , DepositoId =@DepositoId 
			, Observacion =@Observacion , Estado =@Estado , FechaDocumento =@FechaDocumento , 
							 FechaRegistro =@newFecha, HoraRegistro =@newHora, UsuarioRegistro =@usuario
					 Where id=@id;

			INSERT INTO MovimientosDetalle (MovimientoId , ProductoId , Cantidad ,Lote ,FechaVencimiento )
			SELECT @id,td.ProductoId , td.Cantidad , td.Lote ,td.FechaVencimiento 
				FROM @detalle AS td
				WHERE td.ProductoId >0 and td.estado >=0;

	

			--DEVUELVO VALORES DE CONFIRMACION
			select @id as newNumi
			COMMIT TRAN MODIFICACION
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 2, @newFecha, @newHora, @usuario)
			ROLLBACK TRAN MODIFICACION
		END CATCH
	END

	IF @tipo=3 --MOSTRAR TODOS LOS MOVIMIENTOS
	BEGIN
		BEGIN TRY
			SELECT a.Id  as id, a.FechaDocumento  as fdoc, a.ConceptoId  as concep, b.Descripcion  as nconcep, a.Observacion  as obs, 
				   a.Estado  as est, a.DepositoId  as alm ,dep.NombreDeposito 
			FROM Movimientos  a inner join MovimientosTipos  b on a.ConceptoId =b.Id  
			inner join Depositos as dep on dep.id=a.DepositoId 
			where a.ConceptoId not in (1,2)
			ORDER BY a.Id desc


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 3, @newFecha, @newHora, @usuario)
		END CATCH
	END

	IF @tipo=4 --MOSTRAR TODOS DETALLE DE MOVIMIENTO
	BEGIN
		BEGIN TRY
			SELECT a.id , a.MovimientoId , a.ProductoId , b.DescripcionProducto  as Producto,Sum(stock .Cantidad)-(tip.Factor *a.Cantidad) as stock, a.Cantidad 
			, a.Cantidad/p.conversion as CantidadCajas,
			a.Lote ,a.FechaVencimiento ,CAST ('' as image ) as img, 1 as estado ,Sum(stock .Cantidad) as stockFinal,p.conversion
			FROM MovimientosDetalle  a inner join Productos  b on a.ProductoId =b.Id  
			inner join ProductosStock as stock on stock .ProductoId =b.Id and a.FechaVencimiento =stock .FechaVencimiento 
			and a.Lote =stock .Lote 
			inner join Movimientos as mov on mov.Id =a.MovimientoId
			inner join MovimientosTipos as tip on tip.Id=mov.ConceptoId 
			inner join Productos as p on p.id=a.ProductoId
			where a.MovimientoId  =@id
			group by a.id,a.MovimientoId ,a.ProductoId ,b.DescripcionProducto ,a.Cantidad ,a.Lote ,a.FechaVencimiento,tip.Factor,p.Conversion
			ORDER BY a.id asc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 4, @newFecha, @newHora, @usuario)
		END CATCH
	END

		IF @tipo=5 --MOSTRAR TODOS DETALLE DE MOVIMIENTO
	BEGIN
		BEGIN TRY
			select a.Id ,a.Descripcion 
			from MovimientosTipos as a where a.Estado =1 and a.Id not in (1,2,5,6)
			order by a.Id asc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 4, @newFecha, @newHora, @usuario)
		END CATCH
	END

		IF @tipo=6  --Leer configuracion
	BEGIN
		BEGIN TRY
			
			select *
			from Configuracion 
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 4, @newFecha, @newHora, @usuario)
		END CATCH
	END

		IF @tipo=7--MOSTRaR Productos Para el movimiento
	BEGIN
		BEGIN TRY
	

		 select p.Id ,p.CodigoExterno ,p.DescripcionProducto as NombreProducto,p.DescripcionProducto,cat.NombreCategoria  ,Sum(stock .Cantidad ) as stockUnidad
		 ,Sum(stock .Cantidad )/p.Conversion  as stockCaja , 1 as estado,p.Conversion
		 from Productos as p inner join 
		 ProductosStock as stock on stock .ProductoId =p.Id 
		 inner join Categorias as cat on cat.Id =p.CategoriaId 
		 inner join Proveedor as prov on prov.Id=p.ProveedorId 
		 where stock .DepositoId =@DepositoId 
		 group by p.Id ,p.CodigoExterno ,prov.NombreProveedor,p.NombreProducto ,p.DescripcionProducto,cat.NombreCategoria,p.Conversion 
		 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

	IF @tipo=8--MOSTRaR Lotes de producto con stock
	BEGIN
		BEGIN TRY
		
select p.NombreProducto ,a.Lote ,a.FechaVencimiento ,Sum(a.Cantidad ) as stock
from ProductosStock as a
inner join Productos as p on p.Id =a.ProductoId 
and a.Cantidad >0 and a.DepositoId =@DepositoId 
and p.Id=@ProductoId
group by p.NombreProducto ,a.Lote ,a.FechaVencimiento 
order by a.FechaVencimiento asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

IF @tipo=9--Saldo Todos Mayor a Cero
	BEGIN
		BEGIN TRY
		
select stock .DepositoId ,dep .NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto,1 as orden ,cla.DescripcionCategoria as Descripcion  ,Sum(stock .Cantidad) as stock 
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join Depositos as dep on dep.id =stock .DepositoId 
group by stock.DepositoId ,dep.NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto ,cla.DescripcionCategoria  
having Sum(stock .Cantidad)>0


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END
IF @tipo=10--Saldo Todos
	BEGIN
		BEGIN TRY
		
select stock .DepositoId ,dep .NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto,1 as orden ,cla.DescripcionCategoria as   Descripcion  ,Sum(stock .Cantidad) as stock 
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join Depositos as dep on dep.id =stock .DepositoId 
group by stock.DepositoId ,dep.NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto  ,cla.DescripcionCategoria  



		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END
IF @tipo=11--Saldo Todos
	BEGIN
		BEGIN TRY
		
select stock .DepositoId ,dep .NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto,1 as orden ,cla.DescripcionCategoria  as Descripcion  ,Sum(stock .Cantidad) as stock 
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join Depositos as dep on dep.id =stock .DepositoId 
where dep.id=@DepositoId 
group by stock.DepositoId ,dep.NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto ,cla.DescripcionCategoria  



		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

IF @tipo=12--Saldo Todos Mayor a Cero
	BEGIN
		BEGIN TRY
		
select stock .DepositoId ,dep .NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto,1 as orden ,cla.DescripcionCategoria  as Descripcion  ,Sum(stock .Cantidad) as stock 
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join Depositos as dep on dep.id =stock .DepositoId 
where dep.id =@DepositoId 
group by stock.DepositoId ,dep.NombreDeposito ,p.Id ,p.NombreProducto ,p.DescripcionProducto ,cla.DescripcionCategoria   
having Sum(stock .Cantidad)>0


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END


	IF @tipo=13--MOSTRaR Productos Para el movimiento
	BEGIN
		BEGIN TRY
	

		 select p.Id ,p.CodigoExterno ,p.NombreProducto as Nombre,p.DescripcionProducto ,Sum(stock .Cantidad ) as stock , 1 as estado
		 from Productos as p inner join 
		 ProductosStock as stock on stock .ProductoId =p.Id 
		 where stock .DepositoId =@DepositoId 
		 group by p.Id ,p.CodigoExterno ,p.NombreProducto ,p.DescripcionProducto 
		 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

	IF @tipo=14--Obtener Movimientos de Productos
	BEGIN
		BEGIN TRY
	SELECT        a.Id  AS id, a.FechaDocumento  AS fdoc, IIF(c.factor = 1, 1, 2) AS concep,Isnull(c.Descripcion  ,salida.Descripcion )  AS descConcep,
						  IIF(c.id =3 or c.id=4, a.Observacion,IIF(c.id=1 or salida.id=2,
						  substring(a.Observacion, charindex('|',a.Observacion) + 1, LEN(a.Observacion)),a.Observacion))
						   AS obs, a.Estado  AS est, a.DepositoId  AS alm, b.Id  AS id2, b.ProductoId  AS cprod, 
						  d.NombreProducto   AS descProd, 
                         Isnull(Cast((b.Cantidad  *c.Factor  )AS decimal(18, 2)),0) AS entrada,
						 Isnull(Cast((b.Cantidad  *salida.Factor  )AS decimal(18, 2)),0) AS salida
						 , 0.0 AS saldo, '' AS nombreCliente, FORMAT(a.FechaDocumento , 'yyyy-MM-dd') AS fechaR
FROM            Movimientos  AS a INNER JOIN
                         MovimientosDetalle  AS b ON a.Id  = b.MovimientoId  Left JOIN
                         MovimientosTipos  AS c ON c.Id  = a.ConceptoId   and c.Factor =1
						 INNER JOIN Productos   AS d ON d .Id   = b.ProductoId  and d.Id  =@ProductoId    ----Codigo Producto
						 and a.FechaDocumento  <@FechaDocumento  
						  and a.DepositoId =@DepositoId  
						  Left join MovimientosTipos  as salida on salida.Id  = a.ConceptoId  and salida .Factor  =-1
						  order by a.id asc
		 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END


	IF @tipo=15
	BEGIN
		BEGIN TRY
		
SELECT        a.Id  , FORMAT(a.FechaDocumento , 'dd/MM/yyyy')   AS fdoc, IIF(c.factor = 1, 1, 2) AS concep,Isnull(c.Descripcion ,salida.Descripcion )  AS descConcep,
						  IIF(c.id =4 or salida.id=3, a.Observacion,
						  IIF(a.conceptoId=1 ,(select concat('Venta # ',ve.Id,' Al Cliente : ',clie.NombreCliente)
						       from Ventas as ve
							   inner join Clientes as clie on clie.id=ve.ClienteId and ve.id=a.transaccionId and a.ConceptoId=1),(select concat('Compra # ',ve.Id,'  Al Proveedor : ',clie.NombreProveedor)
						       from Compras as ve
							   inner join Proveedor as clie on clie.id=ve.ProveedorId and ve.Id=a.transaccionId and a.ConceptoId=2)))
						   AS obs
						   ,b.Lote  as Lote,b.FechaVencimiento  as FechaVenc,
						   a.Estado  AS est, a.DepositoId  AS alm, b.Id  AS id2, b.ProductoId  AS cprod, 
						  d.NombreProducto   AS descProd,  
                         isnull(Cast((b.Cantidad  *c.Factor  )AS decimal(18, 2)),0) AS entrada,
						 isnull(Cast((b.Cantidad *salida.Factor  )AS decimal(18, 2)),0) AS salida
						 , 0.0 AS saldo, FORMAT(a.FechaDocumento , 'yyyy-MM-dd') AS fechaDocumento
FROM            Movimientos  AS a INNER JOIN
                         MovimientosDetalle  AS b ON a.Id  = b.MovimientoId  Left JOIN
                         MovimientosTipos  AS c ON c.Id  = a.ConceptoId  and c.Factor  =1 INNER JOIN
                         Productos   AS d ON d .Id   = b.ProductoId and d.Id  =@ProductoId    ----Codigo Producto
						  and a.FechaDocumento  >=@FechaI   and a.FechaDocumento  <=@fechaF 
						 and a.DepositoId =@DepositoId  
						  Left join MovimientosTipos  as salida on salida.Id  = a.ConceptoId  and salida .Factor  =-1

	order by a.id asc 



		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum, baproc, balinea, bamensaje, batipo, bafact, bahact, bauact)
				   VALUES(ERROR_NUMBER(), ERROR_PROCEDURE(), ERROR_LINE(), ERROR_MESSAGE(), 3, @newFecha, @newHora, @usuario)
		END CATCH
	END

	
	IF @tipo=16--Listar Productos que han tenido movimientos
	BEGIN
		BEGIN TRY
	
select p.Id  ,p.NombreProducto ,presentacion .Descripcion as Presentacion,cast(0 as decimal(18,2)) as SaldoAnterior,
cast(0 as decimal(18,2)) as Entradas,cast(0 as decimal(18,2)) as Salidas,cast(0 as decimal(18,2)) as SaldoFinal
	from Productos  as p inner join ClasificadorDetalle as presentacion
	on presentacion .Id =p.MarcaId  and
	  p.Id  in(select b.ProductoId 
	from
	Movimientos  as a  
	inner join MovimientosDetalle  as b 
	on b.MovimientoId  =a.Id  and  a.FechaDocumento  >= @fechaI   AND a.FechaDocumento  <= @fechaF  
 AND a.DepositoId  = @DepositoId  )
	  order by p.id asc
		 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END
IF @tipo=17--Reporte Movimiento
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
	
SELECT a.Id, format(a.FechaDocumento,'dd/MM/yyyy')as FechaDocumento, tipo.Descripcion AS Movimiento, a.Observacion, dep.NombreDeposito, detalle.ProductoId, p.NombreProducto, cat.NombreCategoria, detalle.Cantidad, CAST('' AS image) AS img
FROM     dbo.Movimientos AS a INNER JOIN
                  dbo.MovimientosDetalle AS detalle ON a.Id = detalle.MovimientoId INNER JOIN
                  dbo.MovimientosTipos AS tipo ON tipo.Id = a.ConceptoId INNER JOIN
                  dbo.Depositos AS dep ON dep.id = a.DepositoId INNER JOIN
                  dbo.Productos AS p ON p.Id = detalle.ProductoId INNER JOIN
                  dbo.Categorias AS cat ON cat.Id = p.CategoriaId
WHERE  (a.Id = @Id)
		 order by p.NombreProducto 

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END

	IF @tipo=18--Consulta Listado 
	BEGIN
	 SET ARITHABORT ON
		BEGIN TRY
	if (@ProveedorId =-1)
	begin
	select p.Id ,p.CodigoExterno ,prov.NombreProveedor ,p.DescripcionProducto as NombreProducto ,p.DescripcionProducto as DescripcionProducto ,industria .Descripcion as industria,cla.NombreCategoria  as Categoria,
(select aa.precio  from Precios as aa where aa.PrecioCategoriaId=@CategoriaPrecio and aa.almacenId=1 and aa.Productoid=p.Id) as precio,unidad .Descripcion as unidad,p.StockMinimo  ,
		  (SELECT ''+ R.NombreDeposito  +' = '+cast(Sum(inv.Cantidad) as nvarchar(10))+'   ' +char(10) FROM Depositos  as R inner join ProductosStock  as inv on  R.id   =inv.DepositoId  and inv.ProductoId  =p.id group by r.NombreDeposito     FOR XML PATH('')) as stock,Sum(stock .Cantidad) as stockGeneral
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join ClasificadorDetalle as industria on industria .Id =p.FamiliaId 
inner join ClasificadorDetalle as unidad on unidad.Id=p.UnidadVentaId 
inner join proveedor as prov on prov.id =p.ProveedorId 
where p.Estado =1
group by p.Id ,p.NombreProducto ,prov.NombreProveedor ,p.DescripcionProducto ,cla.NombreCategoria ,p.StockMinimo,industria .Descripcion,unidad.Descripcion,p.CodigoExterno 

order by p.Id   asc
	end
	else
	begin
	select p.Id ,p.CodigoExterno ,prov.NombreProveedor ,p.DescripcionProducto as NombreProducto ,p.DescripcionProducto as DescripcionProducto ,industria .Descripcion as industria,cla.NombreCategoria  as Categoria,
(select aa.precio  from Precios as aa where aa.PrecioCategoriaId=@CategoriaPrecio and aa.almacenId=1 and aa.Productoid=p.Id) as precio,unidad .Descripcion as unidad,p.StockMinimo  ,
		  (SELECT ''+ R.NombreDeposito  +' = '+cast(Sum(inv.Cantidad) as nvarchar(10))+'   ' +char(10) FROM Depositos  as R inner join ProductosStock  as inv on  R.id   =inv.DepositoId  and inv.ProductoId  =p.id group by r.NombreDeposito     FOR XML PATH('')) as stock,Sum(stock .Cantidad) as stockGeneral
from Productos as p 
inner join ProductosStock as stock on p.Id =stock.ProductoId 
inner join Categorias  as cla on cla.Id =p.CategoriaId 
inner join ClasificadorDetalle as industria on industria .Id =p.FamiliaId 
inner join ClasificadorDetalle as unidad on unidad.Id=p.UnidadVentaId 
inner join proveedor as prov on prov.id =p.ProveedorId 
where p.Estado =1 and prov.id=@ProveedorId
group by p.Id ,p.NombreProducto ,prov.NombreProveedor ,p.DescripcionProducto ,cla.NombreCategoria ,p.StockMinimo,industria .Descripcion,unidad.Descripcion,p.CodigoExterno 

order by p.Id   asc
	end


		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH

END
END



GO
/*this.oanumi = oanumi;
            this.oafdoc = oafdoc;
            this.oahora = oahora;
            this.oaccli = oaccli;
            this.cliente = cliente;
            this.oarepa = oarepa;
            this.oaest = oaest;
            this.oaobs = oaobs;
            this.latitud = latitud;
            this.longitud = longitud;
            this.total = total;
            this.tipocobro = tipocobro;
            this.estado = estado
            this.codigogenerado = codigogenerad*/
ALTER PROCEDURE  [dbo].[sp_go_TC004_appMovil] (@tipo int, @code_id int=-1, @full_name nvarchar(200)='', @business_name nvarchar(200)='',
											  @nit nvarchar(20)='', @mail nvarchar(255)='', @phone nvarchar(50)='',
											  @cell_phone nvarchar(50)='', @address nvarchar(200)='', @reference nvarchar(200)='',
											  @location_lat decimal(18,14)=0, @location_log decimal(18,14)=0, @password_cli nvarchar(255)='',
											  @old_password_cli nvarchar(255)='',@new_password_cli nvarchar(255)='',@observacion nvarchar(200)='',
											  @categoria nvarchar(20)='',@pedido int=-1,@ventas1 xml=null,@json nvarchar(250)='',
											  @fingreso date=null,@credito decimal(18,2)=0,@chofer int=-1,@fechapedido date=null,@idRepartidor int=-1,
											  @oanumi nvarchar(60)='',@oafdoc date=null,@oahora nvarchar(10)='',@oaccli int=-1,
											  @oarepa int=-1,@oaest int=-1,@oaobs nvarchar(200)='',@latitud decimal(18,16)=0,@longitud decimal(18,16)=0,
											  @total decimal(18,4)=0,@tipocobro int=-1,@tenumi nvarchar(30)='',@codigogenerado nvarchar(50)='',@cczona int=-1,@RazonSocial nvarchar(200)='',@Reclamo nvarchar(60)='',@idzona int=-1,@body xml=null,@TV00121 xml=null,
											  @estado int=-1,@IdSincronizacion nvarchar(250)='',@PedidoId int=-1,@ClienteId int=-1,
											  @id int=-1,@Descripcion nvarchar(300)='',  @Fecha date=null,@Hora nvarchar(5)='',@oaap int=-1,@ventaDirecta int=-1,
											  @TipoNegocio int =-1)
AS
BEGIN

	DECLARE @newHora nvarchar(5)
	DECLARE @encargadodeentrega nvarchar(50), @IdProducto nvarchar(10)
	DECLARE @Contenido decimal (18,2), @Unidad nvarchar(20),@Usuario nvarchar(10)
		declare @ofnumi int, @IdDetalle int,@Salida int,@CantidadActual decimal(18,2), @Cantidad DECIMAL (18,2)
		declare @zona int, @IdUnidad int
		declare @concilia int, @MaxIdMovimiento int, @FechaAct date,@MaxIdMovimiento_Detalle int
		declare @Almacen int
		declare @idventa int
		declare @NroConciliacion int
		DECLARE @ValorNuevo NVARCHAR(MAX)
		declare @Personal nvarchar(400)
		DECLARE @ValorAnterior NVARCHAR(MAX)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))
	   DECLARE @ErrMsg nvarchar(4000) ,
                @ErrNum int,
                @ErrLine int


	DECLARE @newFecha date
	set @newFecha=GETDATE()
	set @fechapedido=GETDATE ()  ----AQUIII
	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		BEGIN TRY
			--Eliminar clientesB



			DELETE FROM Clientes  WHERE Id=@code_id
			SELECT @code_id AS newNumi
		END TRY
		BEGIN CATCH
		INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,'Movil')
		END CATCH
	END


		IF @tipo=111 --NUEVO REGISTRO
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY
			set @code_id=IIF((select COUNT(id) from Clientes)= 0, 0, (select MAX(id) from clientes))+1


			if(not exists(select a.* from Clientes  as a where a.CodigoGenerado=@observacion ))
			begin
			INSERT INTO Clientes (Id,ZonaId,PrecioCategoriaId,CodigoExterno,NombreCliente,DireccionCliente,Telefono,Observacion,
			TipoDocumento,NroDocumento,RazonSocial,Nit ,Estado,FechaIngreso,FechaUltimaVenta,ImagenCliente,Latitud,Longitud,FechaRegistro,
			UsuarioRegistro,CodigoGenerado )
				   VALUES (@code_id,@cczona,1,'',@full_name,@address,@cell_phone,@observacion,28,'',@RazonSocial,@nit,1,GETDATE(),Getdate(),'',@location_lat,
				   @location_log,Getdate(),'APP',@observacion )



			select a.Id as code_id,a.NombreCliente as full_name,a.RazonSocial as business_name,a.Nit as nit,'' as mail,a.Telefono as phone,
			a.Telefono as cell_phone,a.DireccionCliente as [address],'' as reference,a.Latitud as location_lat,a.Longitud as location_log,
			'123' as password_cli,'123' as password_cli_copy
			from Clientes as a where a.id=@code_id


			end
			else
				begin
					select a.Id as code_id,a.NombreCliente as full_name,a.RazonSocial as business_name,a.Nit as nit,'' as mail,a.Telefono as phone,
			a.Telefono as cell_phone,a.DireccionCliente as [address],'' as reference,a.Latitud as location_lat,a.Longitud as location_log,
			'123' as password_cli,'123' as password_cli_copy
			from Clientes as a where a.id=-1


			end



			COMMIT TRAN INSERTAR
		END TRY


		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH

	END
		IF @tipo=112 --NUEVO REGISTRO
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY
			set @code_id=IIF((select COUNT(id) from Clientes)= 0, 0, (select MAX(id) from clientes))+1


			if(not exists(select a.* from Clientes  as a where a.CodigoGenerado=@observacion ))
			begin
			INSERT INTO Clientes (Id,ZonaId,PrecioCategoriaId,CodigoExterno,NombreCliente,DireccionCliente,Telefono,Observacion,
			TipoDocumento,NroDocumento,RazonSocial,Nit ,Estado,FechaIngreso,FechaUltimaVenta,ImagenCliente,Latitud,Longitud,FechaRegistro,HoraRegistro ,
			UsuarioRegistro,CodigoGenerado,Referencia,TipoNegocio  )
				   VALUES (@code_id,@cczona,1,'',@full_name,@address,@cell_phone,@observacion,28,'',@RazonSocial,@nit,1,GETDATE(),Getdate(),'',@location_lat,
				   @location_log,Getdate(),@newHora,'APP',@observacion,'',@TipoNegocio )

			set @ValorNuevo = (select a.* from (
			SELECT cl.NombreCliente,cl.DireccionCliente,cl.Telefono,cl.Latitud,cl.Longitud,cl.RazonSocial,cl.Nit,cl.NroDocumento,zo.NombreZona as Zona,clasi.Descripcion as TipoNegocio
                                         FROM Clientes as cl
										 inner join zonas as zo on zo.id=cl.zonaid
										 inner join ClasificadorDetalle as clasi on clasi.id=cl.TipoNegocio
 										 where cl.id=@code_id) as a FOR JSON AUTO)

			INSERT INTO AuditoriaClientes (Accion, Tabla, CampoModificado, ValorAnterior, ValorNuevo, Fecha, Usuario,idPersonal)
    VALUES ('Nuevo Registro', 'Cliente', 'TODOs', '', @ValorNuevo, Getdate(), 'APP',@idRepartidor)

			select a.Id as code_id,a.NombreCliente as full_name,a.RazonSocial as business_name,a.Nit as nit,'' as mail,a.Telefono as phone,
			a.Telefono as cell_phone,a.DireccionCliente as [address],'' as reference,a.Latitud as location_lat,a.Longitud as location_log,
			'123' as password_cli,'123' as password_cli_copy
			from Clientes as a where a.id=@code_id


			end
			else
				begin
					select a.Id as code_id,a.NombreCliente as full_name,a.RazonSocial as business_name,a.Nit as nit,'' as mail,a.Telefono as phone,
			a.Telefono as cell_phone,a.DireccionCliente as [address],'' as reference,a.Latitud as location_lat,a.Longitud as location_log,
			'123' as password_cli,'123' as password_cli_copy
			from Clientes as a where a.CodigoGenerado =@observacion


			end



			COMMIT TRAN INSERTAR
		END TRY
	 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END
	IF @tipo=3 --OBTENER UN CLIENTE
	BEGIN
		BEGIN TRY
			select a.Id as code_id,a.NombreCliente as full_name,a.RazonSocial as business_name,a.Nit as nit,'' as mail,a.Telefono as phone,
			a.Telefono as cell_phone,a.DireccionCliente as [address],'' as reference,a.Latitud as location_lat,a.Longitud as location_log,
			'123' as password_cli,'123' as password_cli_copy
			from Clientes as a where a.id=@code_id
		END TRY
		BEGIN CATCH
	INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,'Movil')
		END CATCH
	END



		IF @tipo=20--Verifico Si Existe el repartidor
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY
--Limpio Conciliacion que esten sin eliminar
--delete from Conciliaciones
--where Conciliaciones.id not in (select des.ConciliacionId   from DespachoProductos as des)

					--DEVUELVO VALORES DE CONFIRMACION
			--select @code_id as newNumi
			---ViewCredito = Para visualizar la opcion de poner el pedido al credito o contado
			--- CantidadProducto= Para limitar la cantidades de productos en un pedido


			select top 1 a.Id   as code_id, a.NombrePersonal  as repartidor,a.NroDocumento  as ci ,1 as zona
			,cast(1 as integer) as mapa,cast(1 as integer) as pedido,cast(1 as integer ) as update_cliente,1 as categoria,
			1 as stock,1 as ViewCredito,5 as CantidadProducto,0 as ValidarZona,0 as precio,
			isnull((SELECT TOP 1 id FROM Conciliaciones as conci WHERE conci.Estado = 1 AND conci.PersonalID =a.Id),0) as idConciliacion,
			 ( SELECT Id, Descripcion FROM ClasificadorDetalle WHERE IdClasificador = 15 FOR JSON PATH) as TipoNegocio,
			 ( SELECT id, NombreCategoria  FROM Categorias  WHERE Estado  = 1 FOR JSON PATH) as categorias
			from Personal  a
			inner join Usuarios as u on u.IdPersonal =a.Id and a.Estado =1
			where u.NombreUsuario   =@mail  and u.Contrasena    =@password_cli  --and zona.lccbnumi =a.cbnumi

			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

		IF @tipo=21--ObtenerClientes
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY

		if (@idzona =-1)
		begin

					select a.Id  as numi,a.CodigoExterno  as codigo,UPPER(dbo.LimpiarString(a.NombreCliente))  as namecliente,a.Nit  as nit ,a.DireccionCliente  as direccion
					,a.Telefono  as telefono,a.Latitud  as latitud,a.Longitud  as longitud,a.FechaIngreso  as fechaingreso,1 as estado,
cast(a.CodigoGenerado  as nvarchar(200)) as codigogenerado,a.PrecioCategoriaId  as cccat,a.ZonaId   as cczona,a.RazonSocial  as razon_social, 1000 as limite,-- cast(1000 as decimal(18,2)) as limite,
cast(0  as decimal(18,2)) as deuda,a.TipoNegocio
from Clientes  as a where a.Estado =1
--and a.cczona  in
--(select zona.lcnumi from  TL0012 as zona where  zona.lccbnumi=@idRepartidor )

order by a.Id  asc
		end
		else
		begin
	select a.Id  as numi,a.CodigoExterno  as codigo,UPPER(dbo.LimpiarString(a.NombreCliente))  as namecliente,a.Nit  as nit ,a.DireccionCliente  as direccion
					,a.Telefono  as telefono,a.Latitud  as latitud,a.Longitud  as longitud,a.FechaIngreso  as fechaingreso,1 as estado,
cast(a.CodigoGenerado  as nvarchar(200)) as codigogenerado,a.PrecioCategoriaId  as cccat,a.ZonaId   as cczona,a.RazonSocial  as razon_social, 1000 as limite,-- cast(1000 as decimal(18,2)) as limite,
cast(0  as decimal(18,2)) as deuda,a.TipoNegocio
from Clientes  as a where a.Estado =1
--and a.cczona =@idzona

order by a.id asc
		end

			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

		IF @tipo=22--ListarProductos
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY

SELECT a.Id  as numi, a.Id  as cod, a.NombreProducto   as producto,pro.Id as idProveedor, pro.NombreProveedor    as Proveedor, a.CategoriaId  as idcategoria
, b.NombreCategoria  as categoria,Cast(0 as decimal(18,2)) as precio,a.FamiliaId  as familia,isnull((select Min(precio.Precio )
from Precios  as precio
inner join PreciosCategorias  as cat on cat.Id  =precio .PrecioCategoriaId
and cat.Tipo  =1 and precio .ProductoId  =a.Id  ),0) as PrecioMinimo,isnull((select Max(precio.Precio )  from Precios  as precio
inner join PreciosCategorias  as cat on cat.Id  =precio .PrecioCategoriaId
and cat.Tipo  =1 and precio .ProductoId  =a.Id  ),0) as PrecioMaximo,a.Conversion as conversion
FROM Productos  a
inner join Categorias  b on a.CategoriaId =b.Id  and a.Estado  =1
inner join Proveedor as pro on pro.Id=a.ProveedorId
				ORDER BY a.Id  ASC

--				SELECT a.canumi as numi, a.cacod as cod, a.cadesc as producto, a.cadesc2 as desccorta, a.cacat as idcategoria, b.canombre as categoria,
--Cast(0 as decimal(18,2)) as precio,a.cagr4 as familia,isnull((select Min(precio.chprecio)
--from TC003 as precio
--inner join TC007 as cat on cat.cinumi =precio .chcatcl
--and cat.citcv =1 and precio .chcprod =a.canumi ),0) as PrecioMinimo,isnull((select Max(precio.chprecio)  from TC003 as precio
--inner join TC007 as cat on cat.cinumi =precio .chcatcl
--and cat.citcv =1 and precio .chcprod =a.canumi ),0) as PrecioMaximo
--				FROM TC001 a inner join TC005C b on a.cacat=b.canumi and a.caserie=0 and a.caest =1
--				and a.canumi in (
--				select distinct  bb.iccprod
--from TM001 as aa
--inner join TM0011 as bb on aa.ibid =bb.icibid
--inner join TM0012 as conci on conci .ieid =aa.ibidconcil
--where conci .ieest =1 and aa.ibidchof=@idRepartidor
--				)

--				ORDER BY a.canumi ASC
			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END
		IF @tipo=23--ListarPrecios
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY

select p.id as chnumi,p.ProductoId  as chcprod ,p.PrecioCategoriaId  as chcatcl ,cast(p.Precio/pro.Conversion   as decimal (18,5)) as chprecio
from Precios as p
inner join Productos as pro on pro.id=p.ProductoId and pro.Estado =1
			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

		IF @tipo=24--ListPedidos
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY
		set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@idRepartidor ),0)

		if (@idzona =-1)
		begin

		if (@NroConciliacion>0)
		begin
			select a.Id   as oanumi,DATEADD (DAY ,1,a.FechaVenta ) as oafdoc,a.HoraRegistro  as oahora ,cast(a.ClienteId  as nvarchar(200)) as oaccli
			,cliente .NombreCliente  as cliente,a.RepartidorId  as oarepa ,IIF(a.EstadoPedido=4,3,a.EstadoPedido)  as oaest,a.Glosa  as oaobs,
			isnull(a.Latitud ,0) as latitud,
			isnull(a.Longitud ,0) as longitud,
			(isnull((Select (sum(detalle.Total ))from VentasDetalles  as detalle where detalle.VentaId  =a.Id  ),0)-a.Descuento ) as total,IIF(a.TipoVenta=1,1,2)  as tipocobro,IIF(a.TipoVenta =1,0,((isnull((Select (sum(detalle.Total ))from VentasDetalles  as detalle where detalle.VentaId  =a.Id  ),0)-a.Descuento ))) as totalcredito,1 as estado,
			cast(a.id as nvarchar(200)) as codigogenerado,1 as estadoupdate,'' as reclamo,1 as estadoStock,a.Estado as oaap,a.VentaDirecta
			from Ventas as a
			inner join Clientes  as cliente on 	cliente.id =a.ClienteId   and a.EstadoPedido  in (1,2,3,4)
			and a.Estado  =1
			inner join DespachoProductos as desp on desp.ConciliacionId =@NroConciliacion
			inner join ConciliacionAutomaticoPedido as automa on automa.DespachoId =desp.Id and automa.ventaId =a.Id
			order by a.id asc

		end
		else
		begin --Aqui ingresa cuando no tiene conciliacion abierta solo vera los pedidos de los ultimos 3 dias
			select a.Id   as oanumi,DATEADD (DAY ,1,a.FechaVenta ) as oafdoc,a.HoraRegistro  as oahora ,cast(a.ClienteId  as nvarchar(200)) as oaccli
			,cliente .NombreCliente  as cliente,a.RepartidorId  as oarepa ,IIF(a.EstadoPedido=4,3,a.EstadoPedido)   as oaest,a.Glosa  as oaobs,
			isnull(a.Latitud ,0) as latitud,
			isnull(a.Longitud ,0) as longitud,
			(isnull((Select (sum(detalle.Total ))from VentasDetalles  as detalle where detalle.VentaId  =a.Id  ),0)-a.Descuento ) as total,IIF(a.TipoVenta=1,1,2) as tipocobro,IIF(a.TipoVenta =1,0,((isnull((Select (sum(detalle.Total ))from VentasDetalles  as detalle where detalle.VentaId  =a.Id  ),0)-a.Descuento )))  as totalcredito,1 as estado,
			cast(a.id as nvarchar(200)) as codigogenerado,1 as estadoupdate,'' as reclamo,1 as estadoStock,a.Estado as oaap,a.VentaDirecta
			from Ventas as a,Clientes  as cliente where
			cliente.id =a.ClienteId   and a.EstadoPedido  in (1,2,3)
			and a.Estado  =1 and a.PersonalId =@idRepartidor
			AND (a.FechaVenta >= DATEADD(DAY, -4, CAST(GETDATE() AS DATE)))
			order by a.id asc

		end



		end
		else
		begin

		if (@NroConciliacion>0)
		begin
		select a.Id   as oanumi,DATEADD (DAY ,1,a.FechaVenta ) as oafdoc,a.HoraRegistro  as oahora ,cast(a.ClienteId  as nvarchar(200)) as oaccli
			,cliente .NombreCliente  as cliente,a.RepartidorId  as oarepa ,IIF(a.EstadoPedido=4,3,a.EstadoPedido)   as oaest,a.Glosa  as oaobs,
			isnull(a.Latitud ,0) as latitud,
			isnull(a.Longitud ,0) as longitud,
			(isnull((Select (sum(detalle.Total ))from VentasDetalles  as detalle where detalle.VentaId  =a.Id  ),0)-a.Descuento ) as total,IIF(a.TipoVenta=1,1,2) as tipocobro,IIF(a.TipoVenta =1,0,((isnull((Select (sum(detalle.Total ))from VentasDetalles  as detalle where detalle.VentaId  =a.Id  ),0)-a.Descuento )))  as totalcredito,1 as estado,
			cast(a.id as nvarchar(200)) as codigogenerado,1 as estadoupdate,'' as reclamo,1 as estadoStock,a.Estado as oaap,a.VentaDirecta
			from Ventas as a
			inner join Clientes  as cliente on
			cliente.id =a.ClienteId   and a.EstadoPedido  in (1,2,3,4)
			and a.Estado  =1 and a.VentaDirectaSinConciliacion=0
			inner join DespachoProductos as desp on desp.ConciliacionId =@NroConciliacion
			inner join ConciliacionAutomaticoPedido as automa on automa.DespachoId =desp.Id and automa.ventaId =a.Id

			order by a.id asc

		end
		else
		begin
			select a.Id   as oanumi,DATEADD (DAY ,1,a.FechaVenta ) as oafdoc,a.HoraRegistro  as oahora ,cast(a.ClienteId  as nvarchar(200)) as oaccli
		,cliente .NombreCliente  as cliente,a.RepartidorId  as oarepa ,IIF(a.EstadoPedido=4,3,a.EstadoPedido)   as oaest,a.Glosa  as oaobs,
isnull(a.Latitud ,0) as latitud,
isnull(a.Longitud ,0) as longitud,
(isnull((Select (sum(detalle.Total ))from VentasDetalles  as detalle where detalle.VentaId  =a.Id  ),0)-a.Descuento ) as total,1 as tipocobro,IIF(a.TipoVenta =1,0,((isnull((Select (sum(detalle.Total ))from VentasDetalles  as detalle where detalle.VentaId  =a.Id  ),0)-a.Descuento )))  as totalcredito,1 as estado,
cast(a.id as nvarchar(200)) as codigogenerado,1 as estadoupdate,'' as reclamo,1 as estadoStock,a.Estado as oaap,a.VentaDirecta as ventaDirecta
from Ventas as a,Clientes  as cliente where
cliente.id =a.ClienteId   and a.EstadoPedido  in (1,2,3)
and a.VentaDirectaSinConciliacion=0  and a.PersonalId =@idRepartidor
			AND a.FechaVenta >= DATEADD(DAY, -4, CAST(GETDATE() AS DATE))
		end

		end





			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

	IF @tipo=25--ListarPedidoDetalle
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY
		set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@idRepartidor ),0)
		if (@NroConciliacion>0)
		begin
		select cast(a.id  as nvarchar(70))as obnumi,b.ProductoId  as obcprod,producto .NombreProducto  as cadesc  ,cast(b.Cantidad  as decimal (18,2)) as obpcant ,
cast(b.Precio  as decimal (18,4)) as obpbase ,cast(b.SubTotal as decimal (18,4)) as obptot,cast(b.MontoDescuento   as decimal (18,2)) as descuento,
cast(b.Total   as decimal (18,4)) as total,producto.FamiliaId  as familia,cast(1 as bit) as estado,cast(1 as integer) as obupdate,
b.Cantidad /producto.Conversion as cajas ,producto.Conversion as conversion
from VentasDetalles as b inner join
Ventas as a on a.id =b.VentaId
inner join Productos  as producto on producto .id =b.ProductoId   and a.EstadoPedido  in (1,2,3,4)and a.Estado  =1
inner join DespachoProductos as desp on desp.ConciliacionId =@NroConciliacion and a.VentaDirectaSinConciliacion=0
inner join ConciliacionAutomaticoPedido as automa on automa.DespachoId =desp.Id and automa.ventaId =a.Id
		end
		else
		begin
		select cast(a.id  as nvarchar(70))as obnumi,b.ProductoId  as obcprod,producto .NombreProducto  as cadesc  ,cast(b.Cantidad  as decimal (18,2)) as obpcant ,
cast(b.Precio  as decimal (18,4)) as obpbase ,cast(b.SubTotal as decimal (18,4)) as obptot,cast(b.MontoDescuento   as decimal (18,2)) as descuento,
cast(b.Total   as decimal (18,4)) as total,producto.FamiliaId  as familia,cast(1 as bit) as estado,cast(1 as integer) as obupdate,
b.Cantidad /producto.Conversion as cajas ,producto.Conversion as conversion
from VentasDetalles as b inner join
Ventas as a on a.id =b.VentaId and a.VentaDirectaSinConciliacion=0
inner join Productos  as producto on producto .id =b.ProductoId   and a.EstadoPedido  in (1,2,3)and a.Estado  =1 and a.PersonalId =@idRepartidor
		end




			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

		IF @tipo=26--INSERTAR PEDIDO NUEVO
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY



		set @oanumi=IIF((select COUNT(id) from Ventas)=0,0,(select MAX(id) from Ventas))+1



		declare @oazona2 int
		set @oazona2=IIF(exists(select Clientes.ZonaId from Clientes where Clientes.id=@oaccli)
		,(select Clientes.ZonaId from Clientes where Clientes.Id=@oaccli),0)
		if (not exists(select * from Ventas  where IdSincronizacion =@codigogenerado ))
		begin

		insert into Ventas values(@oanumi,1,GETDATE (),@oarepa,@tipocobro,getdate(),@oaccli,1,1,@oaobs,0,@total,GETDATE (),@oahora,'APP',@oaest,
		0,@oafdoc,@codigogenerado,'Movil',@latitud,@longitud ,@ventaDirecta,0,0)

	   if (@oaest=3)
	   begin
	   update ventas set EstadoPedido =4 where id=@oanumi
	   end


		 if (not exists (select a.* from VentasDetalles  a,Ventas  as b where a.VentaId  =b.id and b.IdSincronizacion   =@codigogenerado))
		begin

		 if (@oaest=2)
	   begin

	declare @estadoAnte02 int

	set @estadoAnte02 = ( select top 1 EstadoPedido from Ventas where id =@oanumi )
	if (@estadoAnte02<=@oaest)
	begin

	   update ventas set EstadoPedido =1 where id=@oanumi
	end

	   end


			set @Personal = (select NombrePersonal  from Personal where Id=@oarepa )
			INSERT INTO VentasDetallesBackup (VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Programa)
		SELECT distinct @oanumi,'obcprod'=x.v.value('obcprod[1]','int'),
						'obpcant'=x.v.value('obpcant[1]','numeric(18,5)'),
						x.v.value('obpbase[1]','numeric(18,4)') as precio,
						(x.v.value('obpbase[1]','numeric(18,4)') *x.v.value('obpcant[1]','numeric(18,4)'))as total,0,
						(x.v.value('descuento[1]','numeric(18,2)') ),
						(x.v.value('total[1]','numeric(18,4)') ),'',
						(select top 1 p.Precio  from Precios as p where p.ProductoId =x.v.value('obcprod[1]','int')
						and p.PrecioCategoriaId=2),20200101,'2020-01-01',GETDATE (),@newHora ,' APP'+' - '+@Personal,'NUEVO','APP'
		FROM @ventas1.nodes('/row/row')x(v)
			where x.v.value('obcprod[1]','int') not in (select detalle.ProductoId    from VentasDetalles  as detalle where detalle .VentaId  =@oanumi )



		INSERT INTO VentasDetalles (VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro  )
		SELECT distinct @oanumi,'obcprod'=x.v.value('obcprod[1]','int'),
						'obpcant'=x.v.value('obpcant[1]','numeric(18,5)'),
						x.v.value('obpbase[1]','numeric(18,4)') as precio,
						(x.v.value('obpbase[1]','numeric(18,4)') *x.v.value('obpcant[1]','numeric(18,4)'))as total,0,
						(x.v.value('descuento[1]','numeric(18,2)') ),
						(x.v.value('total[1]','numeric(18,4)') ),'',(select top 1 p.Precio  from Precios as p where p.ProductoId =x.v.value('obcprod[1]','int') and p.PrecioCategoriaId=2),20200101,'2020-01-01',GETDATE (),@oahora,' APP'+' - '+@Personal
		FROM @ventas1.nodes('/row/row')x(v)
			where x.v.value('obcprod[1]','int') not in (select detalle.ProductoId    from VentasDetalles  as detalle where detalle .VentaId  =@oanumi )





		end


	set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@oarepa ),0)

	if (@NroConciliacion>0)
	begin
	  INSERT INTO PedidosAsignaciones(VentaId,PersonalId ,ConciliacionId ,Fecha ,Usuario )
		  SELECT @oanumi ,@oarepa,@NroConciliacion,
			@newFecha ,'APP'
	end




		----------------
		-- DEVUELVO VALORES DE CONFIRMACION
		SELECT Ventas.id as code_id   from Ventas  where id=@oanumi
		end
		else
		begin
		-- DEVUELVO VALORES DE CONFIRMACION
		SELECT Ventas.id as code_id  from Ventas where IdSincronizacion  =@codigogenerado

		end




			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

			IF @tipo=27--INSERTAR DETALLE PEDIDO NUEVO
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY

	/* this.obnumi = obnumi;
        this.obcprod = obcprod;
        this.cadesc = cadesc;
        this.obpcant = obpcant;
        this.obpbase = obpbase;
        this.obptot = obptot;
        this.estado = estado;*/
		if (not exists (select * from VentasDetalles a where a.VentaId =@oanumi ))
		begin




		INSERT INTO VentasDetalles (VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro  )
		SELECT distinct @oanumi,'obcprod'=x.v.value('obcprod[1]','int'),
						'obpcant'=x.v.value('obpcant[1]','numeric(18,5)'),
						x.v.value('obpbase[1]','numeric(18,4)') as precio,
						(x.v.value('obpbase[1]','numeric(18,4)') *x.v.value('obpcant[1]','numeric(18,5)'))as total,0,
						(x.v.value('descuento[1]','numeric(18,2)') ),
						(x.v.value('total[1]','numeric(18,4)') ),'',(select top 1 p.Precio  from Precios as p where p.ProductoId =x.v.value('obcprod[1]','int') and p.PrecioCategoriaId=2),20200101,'2020-01-01',GETDATE (),@oahora,'APP'
		FROM @ventas1.nodes('/row/row')x(v)
			where x.v.value('obcprod[1]','int') not in (select detalle.ProductoId    from VentasDetalles  as detalle where detalle .VentaId  =@oanumi )

		SELECT Ventas.id as code_id   from Ventas  where id=@oanumi
		end

		else
		begin
						SELECT Ventas.id as code_id   from Ventas  where id=@oanumi
		end




			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

	IF @tipo=28--INSERTAR PEDIDO NUEVO
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY

	/* this.obnumi = obnumi;
        this.obcprod = obcprod;
        this.cadesc = cadesc;
        this.obpcant = obpcant;
        this.obpbase = obpbase;
        this.obptot = obptot;
        this.estado = estado;*/
		if ( exists (select * from Ventas  a where a.id =@oanumi ))
		begin



	if (@oaap=2)
	begin
		set @Personal = (select NombrePersonal  from Personal where Id=3 )

		--==== AUDITORIA (INICIO) - snapshot de Estado/Anulado antes de anular/eliminar ====
		DECLARE @Aud_EstadoAnterior_T28 INT, @Aud_AnuladoAnterior_T28 INT
		SELECT @Aud_EstadoAnterior_T28 = Estado, @Aud_AnuladoAnterior_T28 = Anulado FROM Ventas WHERE Id = @oanumi
		--==== AUDITORIA (FIN) ====

		INSERT INTO VentasDetallesBackup (VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Programa)
		SELECT VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		GETDATE(),@newHora ,UsuarioRegistro,'Anulado Antes','APP'
		FROM VentasDetalles where VentaId =@oanumi

		update Ventas  set EstadoPedido  =@oaest,Estado =-1,TipoVenta =IIF(@tipocobro=2,0,1),Glosa=@oaobs,Anulado=1
		where id = @oanumi

		--==== AUDITORIA (INICIO) - registrar evento con foto completa del detalle vigente ====
		--    OJO: aqui el boton movil se llama "Anular" pero el efecto real es identico
		--    al "Eliminar" del desktop (Estado=-1 + borra VentasDetalles), por eso se
		--    etiqueta TipoEvento='ELIMINADO' (mismo criterio que MAM_Ventas tipo=-1) y
		--    se deja la aclaracion en Observacion. Si prefieres verlo como 'ANULADO'
		--    en el reporte, es cambiar un solo literal aqui abajo.
		--    Se hace ANTES del delete de VentasDetalles para poder capturar el detalle.
		--    Aislado en su propio TRY/CATCH (con XACT_ABORT OFF de forma defensiva,
		--    por si una conexion reciclada del pool quedo con XACT_ABORT ON de una
		--    llamada previa al tipo=29): si falla, NO afecta la anulacion real.
		SET XACT_ABORT OFF
		BEGIN TRY
			DECLARE @Aud_EventoId_T28 INT
			INSERT INTO VentasAuditoriaEventos
				(VentaId, TipoEvento, Origen, FechaHora, Usuario, EstadoAnterior, EstadoNuevo, AnuladoAnterior, AnuladoNuevo, Observacion)
			VALUES
				(@oanumi, 'ELIMINADO', 'MOVIL', SYSDATETIME(), ISNULL(@Personal,'APP'), @Aud_EstadoAnterior_T28, -1, @Aud_AnuladoAnterior_T28, 1,
				 'Boton "Anular" de la app movil (tipo=28, oaap=2): Estado=-1 y detalle eliminado, mismo efecto que un Eliminar de escritorio')
			SET @Aud_EventoId_T28 = SCOPE_IDENTITY()

			INSERT INTO VentasAuditoriaDetalle (EventoId, VentaId, ProductoId, TipoCambio, CantidadAntes, CantidadDespues, PrecioAntes, PrecioDespues)
			SELECT @Aud_EventoId_T28, @oanumi, vd.ProductoId, 'VIGENTE', vd.Cantidad, NULL, vd.Precio, NULL
			FROM VentasDetalles AS vd
			WHERE vd.VentaId = @oanumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),
				   CONCAT('AUDITORIA TIPO 28 (oaap=2) | VentaId: ',@oanumi,' | Error: ',ERROR_MESSAGE()),
				   1,@newFecha,@newHora,'Movil')
		END CATCH
		--==== AUDITORIA (FIN) ====

		delete from VentasDetalles where VentaId =@oanumi

	end
	else
	begin

	declare @estadoAnte int

	set @estadoAnte = ( select top 1 EstadoPedido from Ventas where id =@oanumi )
	if (@estadoAnte<=@oaest)
	begin
	update Ventas  set EstadoPedido  =@oaest,Estado =@oaap,TipoVenta =IIF(@tipocobro=2,0,1),Glosa=@oaobs    where id = @oanumi
	end


	end


	  if (@oaest=3)
	   begin
	   update ventas set EstadoPedido =4 where id=@oanumi
	   end

	   set @oarepa=(select top 1  asig.PersonalId  from PedidosAsignaciones as asig where asig.VentaId =@oanumi )

			set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@oarepa ),0)

	if (@NroConciliacion>0)
	begin
	if (exists(select * from PedidosAsignaciones where VentaId=@oanumi))

	begin
	if (select top 1 ConciliacionId from PedidosAsignaciones where VentaId=@oanumi)<=0
	begin
		update PedidosAsignaciones set ConciliacionId=@NroConciliacion where VentaId =@oanumi
	end



	end
   else
   begin
   	  INSERT INTO PedidosAsignaciones(VentaId,PersonalId ,ConciliacionId ,Fecha ,Usuario )
		  SELECT @oanumi ,@oarepa,@NroConciliacion,
			@newFecha ,'APP'
   end

	end


        SELECT ventas.id as code_id   from ventas where id=@oanumi

	    end
		else
		begin
		select 6666 as oanumi
		end

			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

	IF @tipo = 29 -- MODIFICAR PEDIDO
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @AuditoriaId BIGINT;
    DECLARE @CantidadRecibida INT;
    DECLARE @CantidadInsertada INT;

    DECLARE @ErrProcedure NVARCHAR(200);

    BEGIN TRY

        /*
            1. Guardar el XML antes de comenzar la transacción.
            Así la auditoría no desaparece con el ROLLBACK.
        */
        INSERT INTO AuditoriaPedidoMovil
        (
            VentaId,
            RepartidorId,
            TipoProceso,
            XmlRecibido,
            EstadoProceso,
            UsuarioRegistro,
            Programa
        )
        VALUES
        (
            @oanumi,
            @oarepa,
            @tipo,
            @ventas1,
            'RECIBIDO',
            'APP',
            'MAM_Mobile'
        );

        SET @AuditoriaId = SCOPE_IDENTITY();

        /*
            2. Guardar cada detalle recibido.
        */
        INSERT INTO AuditoriaPedidoMovilDetalle
        (
            AuditoriaId,
            VentaId,
            ProductoId,
            Descripcion,
            Cantidad,
            PrecioBase,
            Descuento,
            SubTotal,
            Total,
            ObUpdate,
            Estado
        )
        SELECT
            @AuditoriaId,
            @oanumi,
            x.v.value('(obcprod/text())[1]', 'int'),
            x.v.value('(cadesc/text())[1]', 'varchar(500)'),
            x.v.value('(obpcant/text())[1]', 'decimal(28,10)'),
            x.v.value('(obpbase/text())[1]', 'decimal(28,10)'),
            x.v.value('(descuento/text())[1]', 'decimal(28,10)'),

            x.v.value('(obpbase/text())[1]', 'decimal(28,10)')
            *
            x.v.value('(obpcant/text())[1]', 'decimal(28,10)'),

            x.v.value('(total/text())[1]', 'decimal(28,10)'),
            x.v.value('(obupdate/text())[1]', 'int'),
            x.v.value('(estado/text())[1]', 'bit')
        FROM @ventas1.nodes('/row/row') AS x(v);

        SET @CantidadRecibida = @@ROWCOUNT;

        UPDATE AuditoriaPedidoMovil
        SET CantidadDetalles = @CantidadRecibida
        WHERE AuditoriaId = @AuditoriaId;

        /*
            3. Validar antes de eliminar los detalles actuales.
        */
        IF ISNULL(@CantidadRecibida, 0) = 0
        BEGIN
            UPDATE AuditoriaPedidoMovil
            SET EstadoProceso = 'SIN DETALLES',
                MensajeError =
                    'El XML no contiene registros en la ruta /row/row.'
            WHERE AuditoriaId = @AuditoriaId;

            RAISERROR(
                'El pedido no contiene detalles validos para modificar.',
                16,
                1
            );
        END;

        /*
            4. Desde aquí comienza la modificación real.
        */
        BEGIN TRANSACTION MODIFICACION;

        IF NOT EXISTS
        (
            SELECT 1
            FROM Ventas WITH (UPDLOCK, HOLDLOCK)
            WHERE Id = @oanumi
        )
        BEGIN
            RAISERROR(
                'La venta indicada no existe.',
                16,
                1
            );
        END;

        SET @Personal =
        (
            SELECT TOP 1 NombrePersonal
            FROM Personal
            WHERE Id = @oarepa
        );

        /*
            5. Respaldar los detalles anteriores.
        */
        INSERT INTO VentasDetallesBackup
        (
            VentaId,
            ProductoId,
            Cantidad,
            Precio,
            SubTotal,
            ProcentajeDescuento,
            MontoDescuento,
            Total,
            Detalle,
            PrecioCosto,
            Lote,
            FechaVencimiento,
            FechaRegistro,
            HoraRegistro,
            UsuarioRegistro,
            Accion,
            Programa
        )
        SELECT
            VentaId,
            ProductoId,
            Cantidad,
            Precio,
            SubTotal,
            ProcentajeDescuento,
            MontoDescuento,
            Total,
            Detalle,
            PrecioCosto,
            Lote,
            FechaVencimiento,
            FechaRegistro,
            @newHora,
            UsuarioRegistro,
            'MODIFICACION ANTES - SP MAM_Mobile Tipo:29',
            'APP'
        FROM VentasDetalles
        WHERE VentaId = @oanumi;

        --==== AUDITORIA (INICIO) - snapshot del detalle y cabecera ANTES de modificar ====
        --    Se toma aqui porque VentasDetalles todavia no fue borrado.
        DECLARE @Aud_DetalleAntes_T29 TABLE (ProductoId INT, Cantidad DECIMAL(18,5), Precio DECIMAL(18,4))
        INSERT INTO @Aud_DetalleAntes_T29 (ProductoId, Cantidad, Precio)
        SELECT vd.ProductoId, vd.Cantidad, vd.Precio
        FROM VentasDetalles AS vd
        WHERE vd.VentaId = @oanumi

        DECLARE @Aud_EstadoAnterior_T29 INT, @Aud_AnuladoAnterior_T29 INT
        SELECT @Aud_EstadoAnterior_T29 = Estado, @Aud_AnuladoAnterior_T29 = Anulado FROM Ventas WHERE Id = @oanumi
        --==== AUDITORIA (FIN) ====

        /*
            6. Eliminar los detalles anteriores.
        */
        DELETE FROM VentasDetalles
        WHERE VentaId = @oanumi;

        /*
            7. Insertar los nuevos detalles desde la auditoría.
        */
        INSERT INTO VentasDetalles
        (
            VentaId,
            ProductoId,
            Cantidad,
            Precio,
            SubTotal,
            ProcentajeDescuento,
            MontoDescuento,
            Total,
            Detalle,
            PrecioCosto,
            Lote,
            FechaVencimiento,
            FechaRegistro,
            HoraRegistro,
            UsuarioRegistro
        )
        SELECT
            @oanumi,
            ad.ProductoId,
            CAST(ad.Cantidad AS DECIMAL(18,5)),
            CAST(ad.PrecioBase AS DECIMAL(18,4)),

            CAST(
                ad.PrecioBase * ad.Cantidad
                AS DECIMAL(18,5)
            ),

            0,
            CAST(ad.Descuento AS DECIMAL(18,2)),
            CAST(ad.Total AS DECIMAL(18,4)),
            '',

            (
                SELECT TOP 1 p.Precio
                FROM Precios AS p
                WHERE p.ProductoId = ad.ProductoId
                  AND p.PrecioCategoriaId = 2
                ORDER BY p.Id DESC
            ),

            20200101,
            CONVERT(DATE, '20200101', 112),
            GETDATE(),
            @oahora,
            'APP'
        FROM AuditoriaPedidoMovilDetalle AS ad
        WHERE ad.AuditoriaId = @AuditoriaId
          AND ISNULL(ad.ObUpdate, 0) >= 0;

        SET @CantidadInsertada = @@ROWCOUNT;

        /*
            8. Si no se insertó nada, RAISERROR envía el flujo al CATCH.
        */
        IF ISNULL(@CantidadInsertada, 0) = 0
        BEGIN
            RAISERROR(
                'No se inserto ningun detalle nuevo en VentasDetalles.',
                16,
                1
            );
        END;

		declare @FechaPedidoModi date

		set @FechaPedidoModi =(select FechaVenta from Ventas where id=@oanumi)

		IF @FechaPedidoModi <> CAST(GETDATE() AS DATE)
BEGIN
           DECLARE @PedidoA VARCHAR(40);

SET @PedidoA = CAST(@oanumi AS VARCHAR(40));

RAISERROR(
    'No se puede modificar pedido de otra fecha diferente a la actual. Pedido: %s. Usuario: %s',
    16,
    1,
    @PedidoA,
    @Personal);
END

        --==== AUDITORIA (INICIO) - diff DESPUES vs ANTES, solo registra si hay cambio real ====
        --    Criterio del negocio: solo cuenta como "modificado" si cambio la cantidad o
        --    se agrego/elimino algun producto - NO si solo cambio el precio u otro dato.
        --    IMPORTANTE: esta rama corre con SET XACT_ABORT ON activo (declarado al inicio
        --    del tipo=29). Bajo XACT_ABORT ON, un error dentro de un TRY/CATCH normal
        --    puede dejar la transaccion "condenada" (doomed) aunque el CATCH lo capture,
        --    lo cual tumbaria la modificacion real solo porque fallo la auditoria - eso
        --    es exactamente lo que NO queremos. Por eso se apaga XACT_ABORT solo para
        --    este bloque aislado y se restaura enseguida.
        SET XACT_ABORT OFF
        BEGIN TRY
            DECLARE @Aud_Diff_T29 TABLE (
                ProductoId INT, TipoCambio NVARCHAR(20),
                CantidadAntes DECIMAL(18,5), CantidadDespues DECIMAL(18,5),
                PrecioAntes DECIMAL(18,4), PrecioDespues DECIMAL(18,4))

            INSERT INTO @Aud_Diff_T29 (ProductoId, TipoCambio, CantidadAntes, CantidadDespues, PrecioAntes, PrecioDespues)
            SELECT
                COALESCE(a.ProductoId, d.ProductoId),
                CASE
                    WHEN a.ProductoId IS NULL THEN 'AGREGADO'
                    WHEN d.ProductoId IS NULL THEN 'ELIMINADO'
                    WHEN a.Cantidad <> d.Cantidad THEN 'CANTIDAD_MODIFICADA'
                    ELSE 'SIN_CAMBIO'
                END,
                a.Cantidad, d.Cantidad, a.Precio, d.PrecioBase
            FROM @Aud_DetalleAntes_T29 AS a
            FULL OUTER JOIN (
                SELECT ad.ProductoId, CAST(ad.Cantidad AS DECIMAL(18,5)) AS Cantidad, CAST(ad.PrecioBase AS DECIMAL(18,4)) AS PrecioBase
                FROM AuditoriaPedidoMovilDetalle AS ad
                WHERE ad.AuditoriaId = @AuditoriaId AND ISNULL(ad.ObUpdate, 0) >= 0
            ) AS d ON d.ProductoId = a.ProductoId

            IF EXISTS (SELECT 1 FROM @Aud_Diff_T29 WHERE TipoCambio <> 'SIN_CAMBIO')
            BEGIN
                DECLARE @Aud_EventoId_T29 INT
                INSERT INTO VentasAuditoriaEventos
                    (VentaId, TipoEvento, Origen, FechaHora, Usuario, EstadoAnterior, EstadoNuevo, AnuladoAnterior, AnuladoNuevo, Observacion)
                VALUES
                    (@oanumi, 'MODIFICADO', 'MOVIL', SYSDATETIME(), ISNULL(@Personal,'APP'), @Aud_EstadoAnterior_T29, @Aud_EstadoAnterior_T29, @Aud_AnuladoAnterior_T29, @Aud_AnuladoAnterior_T29, NULL)
                SET @Aud_EventoId_T29 = SCOPE_IDENTITY()

                INSERT INTO VentasAuditoriaDetalle (EventoId, VentaId, ProductoId, TipoCambio, CantidadAntes, CantidadDespues, PrecioAntes, PrecioDespues)
                SELECT @Aud_EventoId_T29, @oanumi, ProductoId, TipoCambio, CantidadAntes, CantidadDespues, PrecioAntes, PrecioDespues
                FROM @Aud_Diff_T29
            END
        END TRY
        BEGIN CATCH
            INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
                   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),
                   CONCAT('AUDITORIA TIPO 29 | VentaId: ',@oanumi,' | Error: ',ERROR_MESSAGE()),
                   1,@newFecha,@newHora,'Movil')
        END CATCH
        SET XACT_ABORT ON
        --==== AUDITORIA (FIN) ====

        /*
            9. Registrar cómo quedaron los detalles después.
        */
        INSERT INTO VentasDetallesBackup
        (
            VentaId,
            ProductoId,
            Cantidad,
            Precio,
            SubTotal,
            ProcentajeDescuento,
            MontoDescuento,
            Total,
            Detalle,
            PrecioCosto,
            Lote,
            FechaVencimiento,
            FechaRegistro,
            HoraRegistro,
            UsuarioRegistro,
            Accion,
            Programa
        )
        SELECT
            vd.VentaId,
            vd.ProductoId,
            vd.Cantidad,
            vd.Precio,
            vd.SubTotal,
            vd.ProcentajeDescuento,
            vd.MontoDescuento,
            vd.Total,
            vd.Detalle,
            vd.PrecioCosto,
            vd.Lote,
            vd.FechaVencimiento,
            vd.FechaRegistro,
            @newHora,
            'APP - ' + ISNULL(@Personal, ''),
            'MODIFICACION DESPUES - SP MAM_Mobile Tipo:29',
            'APP'
        FROM VentasDetalles AS vd
        WHERE vd.VentaId = @oanumi;

        /*
            10. Actualizar el total.
        */
        UPDATE Ventas
        SET TotalVenta =
        (
            SELECT ISNULL(SUM(vd.Total), 0)
            FROM VentasDetalles AS vd
            WHERE vd.VentaId = @oanumi
        )
        WHERE Id = @oanumi;

        COMMIT TRANSACTION MODIFICACION;

        /*
            Esta actualización está fuera de la transacción confirmada.
        */
        UPDATE AuditoriaPedidoMovil
        SET EstadoProceso = 'PROCESADO',
            MensajeError =
                'Detalles recibidos: '
                + CAST(@CantidadRecibida AS VARCHAR(20))
                + '. Detalles insertados: '
                + CAST(@CantidadInsertada AS VARCHAR(20))
        WHERE AuditoriaId = @AuditoriaId;

        SELECT
            @oanumi AS code_id,
            @AuditoriaId AS auditoria_id,
            @CantidadInsertada AS detalles_insertados;

    END TRY
    BEGIN CATCH

        /*
            Capturar el error antes de ejecutar otras instrucciones.
        */
        SET @ErrMsg = ERROR_MESSAGE();
        SET @ErrNum = ERROR_NUMBER();
        SET @ErrLine = ERROR_LINE();
        SET @ErrProcedure = ERROR_PROCEDURE();

        /*
            Revertir la eliminación y cualquier inserción incompleta.
        */
        IF XACT_STATE() <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        /*
            Actualizar auditoría solamente si alcanzó a crearse.
        */
        IF @AuditoriaId IS NOT NULL
        BEGIN
            UPDATE AuditoriaPedidoMovil
            SET EstadoProceso = 'ERROR',
                MensajeError =
                    'Numero: '
                    + CAST(ISNULL(@ErrNum, 0) AS VARCHAR(20))
                    + ' | Procedimiento: '
                    + ISNULL(@ErrProcedure, '')
                    + ' | Linea: '
                    + CAST(ISNULL(@ErrLine, 0) AS VARCHAR(20))
                    + ' | Mensaje: '
                    + ISNULL(@ErrMsg, '')
            WHERE AuditoriaId = @AuditoriaId;
        END;

        INSERT INTO Bitacora
        (
            banum,
            baproc,
            balinea,
            bamensaje,
            batipo,
            bafact,
            bahact,
            bauact
        )
        VALUES
        (
            @ErrNum,
            @ErrProcedure,
            @ErrLine,
            @ErrMsg,
            1,
            @newFecha,
            @newHora,
            'Movil'
        );

        /*
            Reenviar el error a la aplicación.
            Compatible con versiones antiguas de SQL Server.
        */
        RAISERROR(
            @ErrMsg,
            16,
            1
        );

    END CATCH;
END;

	IF @tipo=30 --Modificar Cliente
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY

			if(exists(select a.* from Clientes  as a where a.id  =@code_id ))
			begin
		set @ValorAnterior = (select a.* from (
			SELECT cl.NombreCliente,cl.DireccionCliente,cl.Telefono,cl.Latitud,cl.Longitud,cl.RazonSocial,cl.Nit,cl.NroDocumento,zo.NombreZona as Zona,clasi.Descripcion as TipoNegocio
                                         FROM Clientes as cl
										 inner join zonas as zo on zo.id=cl.zonaid
										 inner join ClasificadorDetalle as clasi on clasi.id=cl.TipoNegocio
 										 where cl.id=@code_id) as a FOR JSON AUTO)

		update Clientes  set NombreCliente  =@full_name ,DireccionCliente  =@address ,Telefono  =@phone ,
		nit =@nit ,Latitud  =@location_lat ,Longitud  =@location_log,RazonSocial  =@RazonSocial,TipoNegocio=@TipoNegocio,ZonaId=@cczona   where id =@code_id


		set @ValorNuevo = (select a.* from (
			SELECT cl.NombreCliente,cl.DireccionCliente,cl.Telefono,cl.Latitud,cl.Longitud,cl.RazonSocial,cl.Nit,cl.NroDocumento,zo.NombreZona as Zona,clasi.Descripcion as TipoNegocio
                                         FROM Clientes as cl
										 inner join zonas as zo on zo.id=cl.zonaid
										 inner join ClasificadorDetalle as clasi on clasi.id=cl.TipoNegocio
 										 where cl.id=@code_id) as a FOR JSON AUTO)

			INSERT INTO AuditoriaClientes (Accion, Tabla, CampoModificado, ValorAnterior, ValorNuevo, Fecha, Usuario,idPersonal)
    VALUES ('Modificar Registro', 'Cliente', 'TODOs', @ValorAnterior, @ValorNuevo, Getdate(), 'APP',@idRepartidor)

			select a.Id as code_id,a.NombreCliente as full_name,a.RazonSocial as business_name,a.Nit as nit,'' as mail,a.Telefono as phone,
			a.Telefono as cell_phone,a.DireccionCliente as [address],'' as reference,a.Latitud as location_lat,a.Longitud as location_log,
			'123' as password_cli,'123' as password_cli_copy
			from Clientes as a where a.id=@code_id



			-- DEVUELVO VALORES DE CONFIRMACION
			--SELECT @code_id AS newNumi

			select a.id as code_id, a.NombreCliente  as full_name
			from clientes a
			where a.id=@code_id


			end
			else
				begin
				select 1
			end



			COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

	IF @tipo=31 --MObtener Stock
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY

		--consultar stock

Declare @ConciliacionId int;
SELECT @ConciliacionId = ISNULL((SELECT TOP 1 id FROM Conciliaciones WHERE Estado = 1 AND PersonalID = @idRepartidor ), 0);
WITH Asignaciones AS (
    SELECT
        detalleV.ProductoId,
        ISNULL(SUM(detalleV.Cantidad), 0) AS Cantidad
    FROM
        PedidosAsignaciones AS asig
    INNER JOIN
        VentasDetalles AS detalleV
    ON
        detalleV.VentaId = asig.VentaId
    WHERE
        (asig.ConciliacionId = @ConciliacionId OR asig.ConciliacionId = 0)
        AND asig.PersonalId = (SELECT cc.PersonalID FROM Conciliaciones AS cc WHERE cc.Id = @ConciliacionId)
    GROUP BY
        detalleV.ProductoId
)
		select a.id as codigoProducto,ti.Cantidad  as cantidad,-1 as almacen
		 from Productos as a
		 inner join ProductosStock  as ti on ti.ProductoId  =a.id
		 and ti.DepositoId  =1 where a.Estado =1
		 union
SELECT
    a.Id AS codigoProducto,
    SUM(detalle.Cantidad) - COALESCE(Asignaciones.Cantidad, 0) AS cantidad,
    1 AS almacen
FROM
    DespachoProductos AS desp
INNER JOIN
    DespachoProductosDetalle AS detalle
ON
    desp.ConciliacionId = @ConciliacionId AND desp.id = detalle.DespachoProductosId
INNER JOIN
    Productos AS a
ON
    a.id = detalle.ProductoId
LEFT JOIN
    Asignaciones
ON
    a.id = Asignaciones.ProductoId
GROUP BY
    a.Id, a.CodigoExterno, a.NombreProducto, a.DescripcionProducto, a.Conversion, Asignaciones.Cantidad;



       COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

	IF @tipo=40--INSERTAR PEDIDO NUEVO
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY

		set @oanumi=IIF((select COUNT(id) from ventas)=0,0,(select MAX(id) from ventas))+1


		set @oazona2=IIF(exists(select clientes.Zonaid  from clientes where clientes.id=@oaccli),(select clientes.zonaId from clientes where clientes.id=@oaccli),0)
		if (not exists(select * from ventas where IdSincronizacion  =@codigogenerado ))
		begin

insert into Ventas values(@oanumi,1,GETDATE (),@oarepa,1,getdate(),@oaccli,1,1,@oaobs,0,0,GETDATE (),@oahora,'APP',@oaest,
		0,@oafdoc,@codigogenerado,'Movil',@latitud,@longitud ,0,0,0)



		 if (not exists (select a.* from VentasDetalles a,ventas as b where a.VentaId  =b.id and b.IdSincronizacion   =@codigogenerado))
		begin

				set @Personal = (select NombrePersonal  from Personal where Id=@oarepa )
			INSERT INTO VentasDetallesBackup (VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro,Accion,Programa)
		SELECT distinct @oanumi,'obcprod'=x.v.value('obcprod[1]','int'),
						'obpcant'=x.v.value('obpcant[1]','numeric(18,5)'),
						x.v.value('obpbase[1]','numeric(18,4)') as precio,
						(x.v.value('obpbase[1]','numeric(18,4)') *x.v.value('obpcant[1]','numeric(18,4)'))as total,0,
						(x.v.value('descuento[1]','numeric(18,2)') ),
						(x.v.value('total[1]','numeric(18,4)') ),'',
						(select top 1 p.Precio  from Precios as p where p.ProductoId =x.v.value('obcprod[1]','int')
						and p.PrecioCategoriaId=2),20200101,'2020-01-01',GETDATE (),@oahora,' APP'+' - '+@Personal,'NUEVO','APP'
		FROM @ventas1.nodes('/row/row')x(v)
			where x.v.value('obcprod[1]','int') not in (select detalle.ProductoId    from VentasDetalles  as detalle where detalle .VentaId  =@oanumi )


			INSERT INTO VentasDetalles (VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,Total,Detalle,PrecioCosto,Lote,FechaVencimiento,
		FechaRegistro,HoraRegistro,UsuarioRegistro  )
		SELECT distinct @oanumi,'obcprod'=x.v.value('obcprod[1]','int'),
						'obpcant'=x.v.value('obpcant[1]','numeric(18,5)'),
						x.v.value('obpbase[1]','numeric(18,4)') as precio,
						(x.v.value('obpbase[1]','numeric(18,4)') *x.v.value('obpcant[1]','numeric(18,5)'))as total,0,
						(x.v.value('descuento[1]','numeric(18,4)') ),
						(x.v.value('total[1]','numeric(18,4)') ),'',(select top 1 p.Precio  from Precios as p where p.ProductoId =x.v.value('obcprod[1]','int') and p.PrecioCategoriaId=2),20200101,'2020-01-01',GETDATE (),@oahora,'APP'
		FROM @ventas1.nodes('/row/row')x(v)
			where x.v.value('obcprod[1]','int') not in (select detalle.ProductoId    from VentasDetalles  as detalle where detalle .VentaId  =@oanumi )
			and (x.v.value('obupdate[1]','int'))>=0


		end
		set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@oarepa ),0)

	if (@NroConciliacion>0)
	begin
	  INSERT INTO PedidosAsignaciones(VentaId,PersonalId ,ConciliacionId ,Fecha ,Usuario )
		  SELECT @oanumi ,@oarepa,@NroConciliacion,
			@newFecha ,'APP'
	end

		----------------
		-- DEVUELVO VALORES DE CONFIRMACION
		SELECT ventas.id as code_id   from ventas where id=@oanumi
		end
		else
		begin
		-- DEVUELVO VALORES DE CONFIRMACION
		SELECT ventas.id as code_id  from ventas where IdSincronizacion  =@codigogenerado

		end




			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

		IF @tipo=41--Obtener Zonas
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY


select a.id as lanumi ,a.NombreZona   as zona ,@idRepartidor  as idRepartidor
from   zonas as a
order by a.id asc

			COMMIT TRAN MODIFICACION
		END TRY
		BEGIN CATCH
		INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,'Movil')
			ROLLBACK TRAN MODIFICACION
		END CATCH
	END
		IF @tipo=42 --MObtener tabla Descuentos
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY

select 1 as id,1 as idProducto, 0as cantidad1,0 as cantidad2,GETDATE() as fechaInicio,
GETDATE() as fechaFin,0 as precio

					COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END
			IF @tipo=43 --MObtener tabla Descuentos
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY

select venta.Id  as PedidoId,cliente.Id as ClienteId,cliente .Nombrecliente as cliente,cliente.DireccionCliente as direccion,cliente.Telefono,
10000 as limitecliente,venta.personalId,per.NombrePersonal as vendedor,venta.FechaVenta as FechaPedido,credito.TotalCredito as totalfactura,
(credito .TotalCredito -isnull((select  Sum(tr.Monto)
from TransaccionesVentasCreditoDetalle as tr where tr.ventaCreditoId =credito.Id ),0)) as pendiente,1 as estado,
cast(IIF(DATEDIFF (DAY ,credito.FechaVencimientoCredito,GETDATE ())<=0,1,0) as bit) as EstadoCredito,
IIF(DATEDIFF (DAY ,credito.FechaVencimientoCredito,GETDATE ())<=0,0,DATEDIFF (DAY ,credito.FechaVencimientoCredito,GETDATE ())) as Mora,0 as Factura
 from CreditosVentas  as credito
 inner join Ventas  as venta on venta.Id =credito.ventaId
 inner join Clientes  as cliente on cliente .Id =venta.clienteId
 inner join Personal as per on per.id=venta.PersonalId
 group by venta.Id ,cliente.Id,cliente.DireccionCliente,cliente.Telefono,venta.PersonalId ,cliente .Nombrecliente,per.NombrePersonal,venta.fechaventa ,credito.TotalCredito ,credito .FechaVencimientoCredito ,
 credito .Id
 having (credito .TotalCredito -isnull((select  Sum(tr.Monto)
from TransaccionesVentasCreditoDetalle as tr where tr.ventaCreditoId =credito.Id ),0))>0


					COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

			IF @tipo=44 --MObtener tabla
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY



		select a.id as tenumi,a.FechaPago as fecha,a.PersonalId as IdPersonal,a.Glosa as observacion,1 as estado
from TransaccionVentasCredito as  a

					COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END


			IF @tipo=45 --MObtener tabla
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY

select detalle.id as tdnumi,credito.VentaId as pedidoId,a.id as cobranzaId,a.FechaPago ,detalle.Monto as montoAPagar,1 as estado,
cl.NombreCliente as cliente,a.Glosa as oaobs
from TransaccionVentasCredito as  a
inner join TransaccionesVentasCreditoDetalle as detalle on
a.id=detalle.transaccionVentaId
inner join CreditosVentas as credito on credito.Id =a.CreditoVentaId
inner join Clientes as cl on cl.Id =a.ClienteId
					COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END


	IF @tipo=47 --MObtener Almacen
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY

		--consultar stock
			set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@oarepa ),0)

		select a.Id  as ProductoId,a.NombreProducto  as Producto,getdate() as fecha,0 as inicial,0 as ingreso,0 as venta,
		0 as saldo,0 as fisico,0 as diferencia,0 as totalbs
		from Productos as a
		inner join VentasDetalles as detalle on detalle.ProductoId =a.Id
		inner join PedidosAsignaciones as asig on asig.VentaId =detalle.VentaId
		where asig.ConciliacionId in (0,@NroConciliacion )
       COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

		IF @tipo=50--ListarProducto View Stock
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY
		set @NroConciliacion =isnull((select top 1 id from Conciliaciones where Estado =1 and PersonalID =@oarepa ),0)
SELECT a.Id  as productoId,a.NombreProducto  as nombreProducto,cast(0 as decimal(18,2))as cantInicial,cast(0 as decimal(18,2)) as cantFinal,
0 as precio,0 as stock,
0 as entrada,0 as xentrada,0 as rebote,0 as aut,0 as saldo
from Productos as a
		inner join VentasDetalles as detalle on detalle.ProductoId =a.Id
		inner join PedidosAsignaciones as asig on asig.VentaId =detalle.VentaId
		where asig.ConciliacionId in (0,@NroConciliacion )

			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END
				IF @tipo=51 --Crear Visita
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY
			set @code_id=IIF((select COUNT(id) from Visita)= 0, 0, (select MAX(id) from Visita))+1
			set @id =(select Id  from Clientes where CodigoGenerado =@ClienteId )
			if(not exists(select a.* from Visita as a where a.IdSincronizacion =@IdSincronizacion))
			begin


			--INSERT INTO Visita (Id,RepartidorId,PedidoId,ClienteId,Descripcion,Estado,Latitud,Longitud,Fecha,Hora,IdSincronizacion )
			--	   VALUES (@code_id,@IdRepartidor,@PedidoId,@id,@Descripcion,@Estado,@Latitud,@Longitud,@Fecha,@Hora,@IdSincronizacion)

			-- DEVUELVO VALORES DE CONFIRMACION
			SELECT 1 AS newNumi

			--select a.id,a.RepartidorId,a.PedidoId,a.ClienteId,cliente.NombreCliente  as NombreCliente,
			--cliente.DireccionCliente  as Direccion,cliente.Telefono  as Telefono,a.Descripcion,a.Estado,
			--a.Latitud,a.Longitud,1 as Sincronizado,cast(a.id as nvarchar(250)) as IdSincronizacion,
			--a.Fecha,a.Hora
			--from Visita a
			--inner join clientes as cliente on cliente.id=a.clienteId
			--where a.id=@code_id

			end
			else
				begin

			--		select a.id,a.RepartidorId,a.PedidoId,a.ClienteId,cliente.NombreCliente  as NombreCliente,
			--cliente.DireccionCliente  as Direccion,cliente.Telefono  as Telefono,a.Descripcion,a.Estado,
			--a.Latitud,a.Longitud,1 as Sincronizado,a.IdSincronizacion as IdSincronizacion,
			--a.Fecha,a.Hora
			--from Visita a
			--inner join clientes as cliente on cliente.id=a.clienteId
			--where a.IdSincronizacion =@observacion

			SELECT 1 AS newNumi

			end



			COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

	IF @tipo=52 --Modificar Visitas
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY

				if( exists(select a.* from Visita as a where a.Id =@id))
			begin


		update Visita set Descripcion  =@Descripcion where id =@id


			select a.Id , '' as full_name
			from Visita a
			where a.Id=@id


			end
			else
				begin
				select 1
			end



			COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END
	IF @tipo=53 --Listar Visitas
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY


			select top 10 a.id,a.RepartidorId,a.PedidoId,a.ClienteId,cliente.NombreCliente  as NombreCliente,
			cliente.DireccionCliente  as Direccion,cliente.Telefono  as Telefono,a.Descripcion,a.Estado,
			a.Latitud,a.Longitud,1 as Sincronizado,cast(a.id as nvarchar(250)) as IdSincronizacion,
			a.fecha,a.Hora
			from Visita a
			inner join clientes as cliente on cliente.id=a.clienteId
			where a.RepartidorId=@IdRepartidor			order by id desc

			COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

	IF @tipo=54 --Listar Puntos
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY


select  a.id as idzona,detalle.Latitud  as latitud,detalle.Longitud  as longitud
from  ZonasPuntos  as detalle
inner join Zonas as a on a.id =detalle.ZonaId


			COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END
	IF @tipo=55 --Get Categoria Precio
	BEGIN
		BEGIN TRAN INSERTAR
		BEGIN TRY


select a.Id,a.Descripcion
from PreciosCategorias as a where a.Tipo =1


			COMMIT TRAN INSERTAR
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END

			IF @tipo=56--Obtener Tipo Negocios
	BEGIN
		BEGIN TRAN MODIFICACION
		BEGIN TRY
SELECT (
    SELECT id, Descripcion
    FROM ClasificadorDetalle
    WHERE IdClasificador = 15
    FOR JSON PATH
) AS valor;

			COMMIT TRAN MODIFICACION
		END TRY
		 BEGIN CATCH
        set @ErrMsg  = ERROR_MESSAGE();
           set     @ErrNum  = ERROR_NUMBER();
           set     @ErrLine = ERROR_LINE();

        INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
            VALUES(@ErrNum,ERROR_PROCEDURE(),@ErrLine,@ErrMsg,1,@newFecha,@newHora,'Movil');

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;
    END CATCH
	END
END
GO
ALTER TRIGGER [dbo].[Trigger_MovimientoInsertar] ON [dbo].[MovimientosDetalle]
AFTER INSERT
AS
BEGIN

	declare @id int, @ibid int, @cprod int, @cantI decimal(18,2),  

			@cantA decimal(18,2), @cantF decimal(18,2), @mov int, @alm int,@lote nvarchar(100)='',@fechaVencimiento date

	declare cSaldo Cursor
		for select a.Id , a.MovimientoId , a.ProductoId , a.Cantidad , b.DepositoId , c.Factor,a.Lote ,a.FechaVencimiento 
			from inserted a inner join Movimientos  b on a.MovimientoId =b.Id 
				 inner join MovimientosTipos  c on b.ConceptoId =c.Id 

	open cSaldo

	fetch cSaldo into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	while (@@FETCH_STATUS = 0)
	begin

		if(exists(select * from ProductosStock  a where a.ProductoId =@cprod and a.DepositoId =@alm
		 and a.Lote =@lote and a.FechaVencimiento =@fechaVencimiento ))
		begin
			begin try
				begin tran tr_UpdateProductosStock
					--Obtener la cantidad actual
					set @cantA=(select Cantidad  from ProductosStock where ProductoId  = @cprod and DepositoId =@alm
					 and Lote =@lote and FechaVencimiento =@fechaVencimiento )
					set @cantF=@cantA + (@cantI*@mov)
					--Actualizar Saldo
					update ProductosStock set Cantidad =@cantF
								 where ProductoId =@cprod and DepositoId =@alm and Lote =@lote and FechaVencimiento =@fechaVencimiento 						
				commit tran tr_UpdateProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end try
			begin catch
				rollback tran tr_UpdateProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end catch
		end
		else
		begin
			begin try
				begin tran tr_InsertProductosStock
					--Insertar Saldo
					Insert into ProductosStock values(@alm,@cprod, (@cantI*@mov),@lote ,@fechaVencimiento )						
				commit tran tr_InsertProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov))
			end try
			begin catch
				rollback tran tr_InsertProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov))
			end catch
		end
		fetch cSaldo into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	end

	close cSaldo
	deallocate cSaldo
END


GO
ALTER TRIGGER [dbo].[Trigger_MovimientoDetalleEliminar] ON [dbo].[MovimientosDetalle]
AFTER DELETE
AS
BEGIN

	declare @id int, @ibid int, @cprod int, @cantI decimal(18,2),  

			@cantA decimal(18,2), @cantF decimal(18,2), @mov int, @alm int,@lote nvarchar(50),@fechaVencimiento date

	declare cSaldo Cursor
		for select a.Id , a.MovimientoId , a.ProductoId , a.Cantidad, b.DepositoId , c.Factor,a.Lote ,a.FechaVencimiento 
			from deleted a inner join Movimientos  b on a.MovimientoId =b.Id 
				 inner join MovimientosTipos  c on b.ConceptoId =c.Id 

	open cSaldo

	fetch cSaldo into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	while (@@FETCH_STATUS = 0)
	begin

		if(exists(select * from ProductosStock  a where a.ProductoId =@cprod and a.DepositoId =@alm 
		and a.Lote =@lote and a.FechaVencimiento =@fechaVencimiento ))
		begin
			begin try
				begin tran tr_UpdateTI001
					--Obtener la cantidad actual
					
					set @cantI=(@cantI*@mov*-1)
					set @cantA=(select Cantidad  from ProductosStock  where ProductoId =@cprod and DepositoId =@alm and 
					Lote =@lote and FechaVencimiento =@fechaVencimiento )
					set @cantF=@cantI+@cantA
					--Actualizar Saldo
					update ProductosStock  set Cantidad=@cantF
								 where ProductoId =@cprod and DepositoId =@alm and Lote =@lote and FechaVencimiento =@fechaVencimiento 						
				commit tran tr_UpdateTI001
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end try
			begin catch
				rollback tran tr_UpdateTI001
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end catch
		end
		else
		begin
			begin try
				begin tran tr_InsertTI001
					--Insertar Saldo
					Insert into ProductosStock  values( @alm,@cprod, (@cantI*@mov*-1),@lote ,@fechaVencimiento )						
				commit tran tr_InsertTI001
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov*-1))
			end try
			begin catch
				rollback tran tr_InsertTI001
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov*-1))
			end catch
		end
		fetch cSaldo into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	end

	close cSaldo
	deallocate cSaldo
END


GO
ALTER TRIGGER [dbo].[Trigger_MovimientosDetalleModificar] ON [dbo].[MovimientosDetalle]
AFTER UPDATE
AS
BEGIN
	--1ro trabajamos sobre las filas eliminadas (E)
	declare @id int, @ibid int, @cprod int, @cantI decimal(18,2),  

			@cantA decimal(18,2), @cantF decimal(18,2), @mov int, @alm int,@lote nvarchar(100),@fechaVencimiento date
	
	declare cSaldoE Cursor
		for select a.Id , a.MovimientoId , a.ProductoId , a.Cantidad , b.DepositoId , c.Factor ,a.Lote ,a.FechaVencimiento 
			from deleted a inner join Movimientos  b on a.MovimientoId =b.Id 
				 inner join MovimientosTipos  c on b.ConceptoId =c.Id 

	open cSaldoE

	fetch cSaldoE into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	while (@@FETCH_STATUS = 0)
	begin

		if(exists(select * from ProductosStock a where a.ProductoId =@cprod and a.DepositoId =@alm and a.Lote =@lote 
		and FechaVencimiento =@fechaVencimiento ))
		begin
			begin try
				begin tran tr_UpdateProductosStock
					--Obtener la cantidad actual
					
					set @cantI=(@cantI*@mov*-1)
					set @cantA=(select Cantidad  from ProductosStock where ProductoId =@cprod and DepositoId =@alm and 
					Lote =@lote and FechaVencimiento =@fechaVencimiento )
					set @cantF=@cantI+@cantA
					--Actualizar Saldo
					update ProductosStock set Cantidad =@cantF
								 where ProductoId =@cprod and DepositoId =@alm and Lote =@lote and FechaVencimiento =@fechaVencimiento 						
				commit tran tr_UpdateProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end try
			begin catch
				rollback tran tr_UpdateProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end catch
		end
		else
		begin
			begin try
				begin tran tr_InsertProductosStock
					--Insertar Saldo
					Insert into ProductosStock values(@alm,@cprod, (@cantI*@mov*-1),@lote ,@fechaVencimiento )						
				commit tran tr_InsertProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov*-1))
			end try
			begin catch
				rollback tran tr_InsertProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov*-1))
			end catch
		end
		fetch cSaldoE into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	end

	close cSaldoE
	deallocate cSaldoE

	--2doo trabajamos sobre las filas insertadas (I)
	declare cSaldoI Cursor
		for select a.Id , a.MovimientoId , a.ProductoId , a.Cantidad , b.DepositoId , c.Factor ,a.Lote ,a.FechaVencimiento 
			from inserted a inner join Movimientos b on a.MovimientoId =b.Id 
				 inner join MovimientosTipos  c on b.ConceptoId =c.Id 

	open cSaldoI

	fetch cSaldoI into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	while (@@FETCH_STATUS = 0)
	begin
		if(exists(select * from ProductosStock a where a.ProductoId =@cprod and a.DepositoId =@alm
		and Lote =@lote and FechaVencimiento =@fechaVencimiento ))
		begin
			begin try
				begin tran tr_UpdateProductosStock
					--Obtener la cantidad actual
					set @cantA=(select Cantidad  from ProductosStock where ProductoId  = @cprod and DepositoId =@alm
					and Lote =@lote and FechaVencimiento =@fechaVencimiento )
					set @cantF=@cantA + (@cantI*@mov)
					--Actualizar Saldo
					update ProductosStock set Cantidad =@cantF
								 where ProductoId =@cprod and DepositoId =@alm and Lote =@lote and FechaVencimiento =@fechaVencimiento 						
				commit tran tr_UpdateProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end try
			begin catch
				rollback tran tr_UpdateProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', @cantF)
			end catch
		end
		else
		begin
			begin try
				begin tran tr_InsertProductosStock
					--Insertar Saldo
					Insert into ProductosStock values(@alm,@cprod, (@cantI*@mov),@lote ,@fechaVencimiento )						
				commit tran tr_InsertProductosStock
				print concat('OK -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov))
			end try
			begin catch
				rollback tran tr_InsertProductosStock
				print concat('ERROR -> cprod: ', @cprod, ' Saldo: ', (@cantI*@mov))
			end catch
		end
		fetch cSaldoI into @id, @ibid, @cprod, @cantI, @alm, @mov,@lote,@fechaVencimiento
	end

	close cSaldoI
	deallocate cSaldoI
END



GO
SET NOEXEC OFF
GO
RAISERROR('04 StockSeguro DESHECHO.', 10, 1) WITH NOWAIT
GO
