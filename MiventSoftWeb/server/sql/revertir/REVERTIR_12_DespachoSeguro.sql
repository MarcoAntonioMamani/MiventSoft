/* =====================================================================
   SOLO PARA DESHACER 12_DespachoSeguro.sql. NO es parte de la instalacion: NO ejecutar al instalar.
   Vuelve MAM_DespachoProductos de DistribucionDistralKCP2023 a la version original (eliminar despacho sin validar ni borrar la conciliacion).
   ===================================================================== */
SET NOEXEC OFF
GO
IF NOT (OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_DespachoProductos')) LIKE '%Sanbit: despacho seguro%')
BEGIN
	RAISERROR('12 DespachoSeguro NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT
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
		BEGIN TRY
		    delete from DespachoProductosDetalle  where DespachoProductosId  =@Id 
			delete from DespachoProductos    where Id=@Id;

			delete from ConciliacionAutomaticoPedido where DespachoId =@Id 
			SELECT 1 as resp;
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@usuario )
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
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
RAISERROR('12 DespachoSeguro DESHECHO.', 10, 1) WITH NOWAIT
GO
