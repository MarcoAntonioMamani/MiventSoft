/* =====================================================================
   12 - DESPACHO SEGURO (eliminar / grabar / modificar despachos sin dejar datos sueltos)
   Cambia SOLO MAM_DespachoProductos:
   - Eliminar (@-1): NO si la conciliacion esta cerrada o si algun pedido del despacho ya se entrego.
     Si es el ULTIMO despacho de su conciliacion, la conciliacion se elimina con el (antes quedaba abierta y
     vacia), salvo que ya tenga entregas, ventas directas, detalle o cierre de caja. Borra tambien
     VentasDetallesDespacho del despacho. Todo en una transaccion.
   - Nuevo automatico (@1): NO si un pedido ya esta en otro despacho abierto o ya no esta asignado a ese personal.
   - Modificar (@2): NO en conciliacion cerrada, NO el automatico, NO cambiar el personal.
   Todo lo demas del SP queda IDENTICO. Vale para web y escritorio.

   Generado desde DistribucionDistralKCP2023 (2026-11-16). PROTECCION: solo se aplica si el SP es esa version (sin contar mayusculas
   ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da error y no modifica nada
   (regenerar con: node sql/generar/despacho_seguro.js). No hace falta reiniciar la web ni la API.
   VOLVER ATRAS: revertir\REVERTIR_12_DespachoSeguro.sql (SOLO para deshacer).
   ===================================================================== */
SET NOEXEC OFF
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_DespachoProductos')) LIKE '%Sanbit: despacho seguro%'
BEGIN
	RAISERROR('12 DespachoSeguro YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT
	SET NOEXEC ON
END
ELSE IF ISNULL(HASHBYTES('SHA2_256', REPLACE(REPLACE(REPLACE(REPLACE(UPPER(OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_DespachoProductos'))), CHAR(13), ''), CHAR(10), ''), CHAR(9), ''), ' ', '')), 0x) <> 0x68C695C297EC88B84072480E95FAB807C75D79BC3D9FB835E2669C76E99BBC7E
BEGIN
	RAISERROR('12 DespachoSeguro: MAM_DespachoProductos es OTRA version (no la de DistribucionDistralKCP2023 2026-11-16). No se modifico nada.', 16, 1)
	SET NOEXEC ON
END
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
--drop procedure MAM_DespachoProductos

ALTER PROCEDURE [dbo].[MAM_DespachoProductos](@tipo int,@Id int=-1,@PersonalId int=-1,@ConciliacionId int=-1,
@SucursalId int=-1,@Fecha date=null,@NroNota as nvarchar(100)='',@Detalle nvarchar(450)='',
@TipoMovimientoId int=-1,@Usuario nvarchar(50)='',@DetalleDespacho DespachoProductosDetalleType ReadOnly,
@desde date=null,@hasta date=null,@DespachoAutomatico int =-1,@ReferenciaVenta VentaDespachoAutomaticoType ReadOnly)
AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))

	DECLARE @newFecha date
	set @newFecha=GETDATE()
	
	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		-- Sanbit: despacho seguro (12): validar antes de borrar; si es el ultimo despacho, la conciliacion se va con el
		DECLARE @Elim_Conci int, @Elim_EstadoConci int, @Elim_Lista nvarchar(1500), @Elim_BorrarConci bit = 0, @Elim_Error nvarchar(2048)
		SELECT @Elim_Conci = d.ConciliacionId, @Elim_EstadoConci = c.Estado
		FROM DespachoProductos AS d LEFT JOIN Conciliaciones AS c ON c.Id = d.ConciliacionId WHERE d.Id = @Id
		IF @Elim_Conci IS NULL
		BEGIN
			RAISERROR('NO SE PUEDE ELIMINAR: el despacho %d ya no existe. Actualice la lista.', 16, 1, @Id)
			RETURN
		END
		IF ISNULL(@Elim_EstadoConci, 1) <> 1
		BEGIN
			RAISERROR('NO SE PUEDE ELIMINAR EL DESPACHO %d: la conciliacion %d ya esta cerrada.', 16, 1, @Id, @Elim_Conci)
			RETURN
		END
		-- pedidos de este despacho que el repartidor ya entrego
		SET @Elim_Lista = STUFF((SELECT TOP 10 CONCAT(', #', v.Id) FROM ConciliacionAutomaticoPedido AS cap INNER JOIN Ventas AS v ON v.Id = cap.ventaId WHERE cap.DespachoId = @Id AND v.EstadoPedido IN (3, 4) ORDER BY v.Id FOR XML PATH(''), TYPE).value('.', 'nvarchar(1500)'), 1, 2, '')
		IF @Elim_Lista IS NOT NULL
		BEGIN
			RAISERROR('NO SE PUEDE ELIMINAR EL DESPACHO %d: estos pedidos ya se entregaron: %s. Revierta primero la entrega.', 16, 1, @Id, @Elim_Lista)
			RETURN
		END
		IF NOT EXISTS (SELECT 1 FROM DespachoProductos WHERE ConciliacionId = @Elim_Conci AND Id <> @Id)
		BEGIN
			-- es el ultimo despacho: la conciliacion se elimina con el, salvo que ya tenga movimiento propio
			SET @Elim_Lista = STUFF((SELECT TOP 10 CONCAT(', #', v.Id, IIF(v.VentaDirecta = 1, ' venta directa', ' entregado')) FROM PedidosAsignaciones AS a INNER JOIN Ventas AS v ON v.Id = a.VentaId WHERE a.ConciliacionId = @Elim_Conci AND (v.EstadoPedido IN (3, 4) OR v.VentaDirecta = 1) ORDER BY v.Id FOR XML PATH(''), TYPE).value('.', 'nvarchar(1500)'), 1, 2, '')
			IF @Elim_Lista IS NOT NULL
			BEGIN
				RAISERROR('NO SE PUEDE ELIMINAR EL DESPACHO %d: es el ultimo de la conciliacion %d y esa conciliacion ya tiene movimiento (%s). Cierrela desde Conciliacion.', 16, 1, @Id, @Elim_Conci, @Elim_Lista)
				RETURN
			END
			IF EXISTS (SELECT 1 FROM ConciliacionesDetalle WHERE ConciliacionId = @Elim_Conci) OR EXISTS (SELECT 1 FROM CierreCaja WHERE ConciliacionId = @Elim_Conci)
			BEGIN
				RAISERROR('NO SE PUEDE ELIMINAR EL DESPACHO %d: es el ultimo de la conciliacion %d y esa conciliacion ya tiene detalle o cierre de caja.', 16, 1, @Id, @Elim_Conci)
				RETURN
			END
			SET @Elim_BorrarConci = 1
		END
		BEGIN TRAN
		BEGIN TRY
		    delete from DespachoProductosDetalle  where DespachoProductosId  =@Id 
			delete from ConciliacionAutomaticoPedido where DespachoId =@Id 
			delete from VentasDetallesDespacho where despachoId = @Id
			delete from DespachoProductos    where Id=@Id;
			IF @Elim_BorrarConci = 1
			BEGIN
				-- los pedidos siguen asignados al mismo personal (como antes del despacho), sin conciliacion
				update PedidosAsignaciones set ConciliacionId = 0 where ConciliacionId = @Elim_Conci
				delete from Conciliaciones where Id = @Elim_Conci
			END
			COMMIT TRAN
			SELECT 1 as resp, IIF(@Elim_BorrarConci = 1, @Elim_Conci, 0) as ConciliacionEliminada;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRAN
			SET @Elim_Error = ERROR_MESSAGE()
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@usuario )
			RAISERROR('NO SE PUDO ELIMINAR EL DESPACHO %d: %s', 16, 1, @Id, @Elim_Error)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
		-- Sanbit: despacho seguro (12): en un despacho automatico cada pedido entra una sola vez y solo al de su repartidor
		IF @DespachoAutomatico = 1
		BEGIN
			DECLARE @Nuevo_Lista nvarchar(1500) = STUFF((SELECT TOP 10 CONCAT(', #', r.VentaId, ' (despacho ', cap.DespachoId, ')') FROM @ReferenciaVenta AS r INNER JOIN ConciliacionAutomaticoPedido AS cap ON cap.ventaId = r.VentaId INNER JOIN DespachoProductos AS dp ON dp.Id = cap.DespachoId INNER JOIN Conciliaciones AS cc ON cc.Id = dp.ConciliacionId AND cc.Estado = 1 ORDER BY r.VentaId FOR XML PATH(''), TYPE).value('.', 'nvarchar(1500)'), 1, 2, '')
			IF @Nuevo_Lista IS NOT NULL
			BEGIN
				RAISERROR('NO SE PUEDE GRABAR EL DESPACHO: los pedidos %s ya estan en otro despacho. Actualice la pantalla.', 16, 1, @Nuevo_Lista)
				RETURN
			END
			SET @Nuevo_Lista = STUFF((SELECT TOP 10 CONCAT(', #', r.VentaId) FROM @ReferenciaVenta AS r WHERE NOT EXISTS (SELECT 1 FROM PedidosAsignaciones AS a WHERE a.VentaId = r.VentaId AND a.PersonalId = @PersonalId) ORDER BY r.VentaId FOR XML PATH(''), TYPE).value('.', 'nvarchar(1500)'), 1, 2, '')
			IF @Nuevo_Lista IS NOT NULL
			BEGIN
				RAISERROR('NO SE PUEDE GRABAR EL DESPACHO: los pedidos %s ya no estan asignados a este personal. Actualice la pantalla.', 16, 1, @Nuevo_Lista)
				RETURN
			END
		END
		BEGIN TRAN INSERTAR
		BEGIN TRY 
			Declare @UltimaConciliacionAbierta int
			
			set @Id=IIF((select COUNT(Id) from DespachoProductos)=0,0,(select MAX(Id) from DespachoProductos))+1
			set @UltimaConciliacionAbierta =isnull(( select a.Id   from Conciliaciones as a where a.PersonalID =@PersonalId and a.Estado =1 and a.SucursalId =@SucursalId ),0)

			if (@UltimaConciliacionAbierta =0)
			begin
			declare @IdConci int
			set @IdConci=IIF((select COUNT(Id) from Conciliaciones)=0,0,(select MAX(Id) from Conciliaciones))+1 


			insert into Conciliaciones values (@IdConci,@Fecha,1,@PersonalId ,@SucursalId,'',0,0 )
			set @UltimaConciliacionAbierta=@IdConci 
			end


			--Estado conciliacion 1= abierta y 0 = Cerrada

			INSERT INTO DespachoProductos   VALUES(@Id,@PersonalId,@UltimaConciliacionAbierta ,@SucursalId ,@Fecha ,@NroNota ,@Detalle,@TipoMovimientoId ,
			@newFecha ,@newHora ,@Usuario,@DespachoAutomatico  )
			
				----INSERTO EL DEcaLLE
			INSERT INTO DespachoProductosDetalle (DespachoProductosId ,ProductoId  ,Cantidad  )

			SELECT @Id,td.ProductoId ,td.Cantidad      FROM @DetalleDespacho  AS td
			where td.estado  =0 and  td.ProductoId    >0

			if (@DespachoAutomatico =1)
			begin
			insert into  VentasDetallesDespacho (despachoId,VentaId,ProductoId,Cantidad,Precio,SubTotal,ProcentajeDescuento,MontoDescuento,
			Total,Detalle,PrecioCosto,Lote,FechaVencimiento,FechaRegistro,HoraRegistro,UsuarioRegistro,CantidadUnitaria)
			select @Id,detalle.VentaId,detalle.ProductoId,detalle.Cantidad,detalle.Precio,detalle.SubTotal,detalle.ProcentajeDescuento,detalle.MontoDescuento,
			detalle.Total,detalle.Detalle ,detalle.PrecioCosto,detalle.Lote,detalle.FechaVencimiento,detalle.FechaRegistro,detalle.HoraRegistro,detalle.UsuarioRegistro,detalle.CantidadUnitaria 
			from VentasDetalles as detalle inner join @ReferenciaVenta as ref on ref.VentaId =detalle.VentaId 

			INSERT INTO ConciliacionAutomaticoPedido (ventaId,DespachoId )
			SELECT td.VentaId ,@Id     FROM @ReferenciaVenta  AS td
			end
			------Modifico los Precios Costo------------------
		    

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
	
	IF @tipo=2--MODIFICACION
	BEGIN
		-- Sanbit: despacho seguro (12): no se modifica en conciliacion cerrada, ni el automatico, ni se cambia el personal
		DECLARE @Mod_Conci int, @Mod_EstadoConci int, @Mod_Personal int, @Mod_Auto int
		SELECT @Mod_Conci = d.ConciliacionId, @Mod_EstadoConci = c.Estado, @Mod_Personal = c.PersonalID, @Mod_Auto = ISNULL(d.DespachoAutomatico, 0)
		FROM DespachoProductos AS d LEFT JOIN Conciliaciones AS c ON c.Id = d.ConciliacionId WHERE d.Id = @Id
		IF @Mod_Conci IS NULL
		BEGIN
			RAISERROR('NO SE PUEDE MODIFICAR: el despacho %d ya no existe. Actualice la lista.', 16, 1, @Id)
			RETURN
		END
		IF ISNULL(@Mod_EstadoConci, 1) <> 1
		BEGIN
			RAISERROR('NO SE PUEDE MODIFICAR EL DESPACHO %d: la conciliacion %d ya esta cerrada.', 16, 1, @Id, @Mod_Conci)
			RETURN
		END
		IF @Mod_Auto = 1
		BEGIN
			RAISERROR('NO SE PUEDE MODIFICAR EL DESPACHO %d: es automatico. Eliminelo y vuelva a crearlo.', 16, 1, @Id)
			RETURN
		END
		IF @Mod_Personal IS NOT NULL AND @PersonalId <> @Mod_Personal
		BEGIN
			RAISERROR('NO SE PUEDE CAMBIAR EL PERSONAL DEL DESPACHO %d: pertenece a la conciliacion %d de otro personal. Eliminelo y creelo para el nuevo personal.', 16, 1, @Id, @Mod_Conci)
			RETURN
		END
		BEGIN TRAN MODIFICACION
		BEGIN TRY 
			
			UPDATE DespachoProductos    SET PersonalId=@PersonalId,SucursalId  =@SucursalId  ,Fecha  =@Fecha ,
			NroNota  =@NroNota ,Detalle  =@Detalle  ,FechaRegistro =@newFecha,HoraRegistro =@newHora,UsuarioRegistro   =@usuario 
					 Where Id = @Id 
			
			delete from DespachoProductosDetalle  where DespachoProductosId  =@Id 

				----INSERTO EL DEcaLLE
			INSERT INTO DespachoProductosDetalle (DespachoProductosId ,ProductoId  ,Cantidad)

			SELECT @Id,td.ProductoId ,td.Cantidad      FROM @DetalleDespacho  AS td
			where td.estado  >=0 and  td.ProductoId    >0
			------Modifico los Precios Costo------------------

			--DEVUELVO VALORES DE CONFIRMACION
			select @Id as newNumi
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@usuario )
			ROLLBACK TRAN
		END CATCH
	END

	
	IF @tipo=3--Listar Despacho
	BEGIN
		BEGIN TRY
			
			select despacho.Id,despacho .PersonalId ,p.NombrePersonal ,despacho .ConciliacionId ,
			despacho .SucursalId ,despacho .Fecha ,despacho .NroNota ,despacho .Detalle ,despacho .TipoMovimientoId,
			conci .Estado as EstadoConciliacion,cast('' as image )as imgConciliacion,isnull(despacho.DespachoAutomatico,0)as DespachoAutomatico
			from DespachoProductos as despacho 
			inner join Personal as p on despacho .PersonalId =p.Id 
			inner join Conciliaciones as conci on conci.Id =despacho .ConciliacionId 
			where despacho.Fecha >=@desde and despacho.Fecha <=@hasta 
			order by despacho .Id desc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH
	END

	IF @tipo=4
	BEGIN
		BEGIN TRY
			
			select detalle.Id ,detalle .DespachoProductosId,detalle .ProductoId ,p.NombreProducto,
			detalle .Cantidad ,detalle.Cantidad /p.Conversion as CantidadCaja,(detalle .Cantidad+(select sum(st.Cantidad )  from ProductosStock as st where st.ProductoId=p.Id and st.DepositoId =alm.DepositoId)) as Stock,
			1 as estado,p.conversion,cast('' as image) as img
			from DespachoProductosDetalle as detalle
			inner join Productos as p on detalle .ProductoId =p.Id 
			inner join DespachoProductos as despacho on despacho .Id =detalle .DespachoProductosId 
			inner join Almacenes as alm on alm.id =despacho .SucursalId 
			where detalle .DespachoProductosId =@Id 
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH
	END
	IF @tipo=5
	BEGIN
		BEGIN TRY
		
		select consulta.Id,consulta .CodigoExterno,consulta.NombreProducto,consulta.DescripcionProducto,
		consulta.NombreCategoria,consulta.stock+isnull(sum(detalle.Cantidad),0) as stock,(consulta.stock+isnull(sum(detalle.Cantidad),0))/consulta.Conversion as stockCaja,consulta.estado,
		consulta.Conversion
		from VentasDetalles as detalle 
		inner join Ventas as ven on ven.Id=detalle .VentaId  and ven.EstadoPedido in (1,2)
		right join ( select p.Id ,p.CodigoExterno ,p.NombreProducto,p.DescripcionProducto,cat.NombreCategoria  
		  ,Sum(stock .Cantidad ) as stock, Sum(stock .Cantidad )/p.Conversion 
		  as stockCaja, 1 as estado,p.Conversion 
		 from Productos as p inner join 
		 ProductosStock as stock on stock .ProductoId =p.Id 
		 inner join Categorias as cat on cat.Id =p.CategoriaId 
		 inner join Almacenes as alm on alm.id =@SucursalId 
		 where stock .DepositoId =alm.DepositoId   
		 group by p.Id ,p.CodigoExterno ,p.NombreProducto ,p.DescripcionProducto,cat.NombreCategoria,p.Conversion ) as consulta on consulta.Id =detalle.ProductoId 

		 group by consulta.Id,consulta .CodigoExterno,consulta.NombreProducto,consulta.DescripcionProducto,
		consulta.NombreCategoria,consulta.stockCaja,consulta.estado,
		consulta.Conversion,consulta.stock 
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH
	END

	IF @tipo=6 ---Reporte Salida Productos
	BEGIN
		BEGIN TRY
	SELECT desp.Id, format(desp.Fecha,'dd/MM/yyyy') AS FechaDespacho, desp.ConciliacionId AS NroConciliacion, p.NombrePersonal, desp.Detalle, prod.Id AS ProductoId, prod.NombreProducto, detalle.Cantidad, CAST('' AS image) AS img
FROM     dbo.DespachoProductos AS desp INNER JOIN
                  dbo.DespachoProductosDetalle AS detalle ON detalle.DespachoProductosId = desp.Id INNER JOIN
                  dbo.Personal AS p ON desp.PersonalId = p.Id INNER JOIN
                  dbo.Productos AS prod ON prod.Id = detalle.ProductoId
WHERE  (desp.Id = @Id )
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH
	END

	
	IF @tipo=7 ---Obtener Productos Despacho Automatico
	BEGIN
		BEGIN TRY

select 0 as Id, 0 as DespachoProductosId,detalle.ProductoId,p.DescripcionProducto as NombreProducto,
Sum(detalle .Cantidad) as Cantidad,(Sum(detalle .Cantidad)/p.Conversion ) as CantidadCaja,
			(sum(detalle .Cantidad)+(select sum(st.Cantidad )
			from ProductosStock as st where st.ProductoId=p.Id and st.DepositoId =@SucursalId )) as Stock,
			0 as estado,p.conversion,cast('' as image) as img
from Productos as p
inner join VentasDetalles as detalle
on detalle .ProductoId  =p.Id 
inner join Categorias as cat on cat.Id =p.CategoriaId 
inner join  Ventas as v on v.id=detalle .VentaId 
where v.id in 
(select asig.VentaId 
 from PedidosAsignaciones as asig where asig.PersonalId =@PersonalId ) and  v .EstadoPedido =2  and v.Estado =1 and V.Anulado=0
 and v.Id not in (select ventaid  from ConciliacionAutomaticoPedido 
inner join DespachoProductos as desp on desp.Id =DespachoId
inner join Conciliaciones as conci on conci.id=desp.ConciliacionId  
where conci.Estado =1
 ) 
group by detalle.ProductoId,p.DescripcionProducto,p.Conversion,p.Id

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH
	END

	
	IF @tipo=8 --Obtener Id Venta
	BEGIN
		BEGIN TRY
select ve.Id 
from (select v.Id 
from Ventas as v 
where  v .EstadoPedido =2 and  v.Estado =1 and V.Anulado=0 and v.id in 
(select asig.VentaId 
 from PedidosAsignaciones as asig where asig.PersonalId =@PersonalId  )) as ve
 where ve.Id not in (select ventaid  from ConciliacionAutomaticoPedido inner join DespachoProductos as desp on desp.id=DespachoId 
        inner join Conciliaciones as conci on conci.id=desp.ConciliacionId and conci.Estado =1 )
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@usuario )
		END CATCH
	END

END









GO
SET NOEXEC OFF
GO
IF OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_DespachoProductos')) LIKE '%Sanbit: despacho seguro%'
	RAISERROR('12 DespachoSeguro INSTALADO.', 10, 1) WITH NOWAIT
GO
