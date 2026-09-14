/*
	PASO 1 de "Cantidad Unidad / Cantidad Caja" en Compras (+ ajuste posterior).

	Que cambia y por que (mismo patron que ya se aplico en MAM_Movimientos,
	script ALTER_MAM_Movimientos_UnidadCaja.sql):

	- @tipo=5 (lista de productos para comprar) y @tipo=4 (detalle de una compra
	  ya guardada) ahora tambien traen: UnidadVentaId, UnidadVenta (nombre de la
	  Unidad Minima via ClasificadorDetalle), UnidadMaximaId, UnidadMaxima (nombre
	  de la Unidad Maxima / "caja"), y Conversion.
	- Conversion sale con ISNULL(NULLIF(Conversion,0),1): si el producto no tiene
	  Unidad Maxima configurada (Conversion en NULL o en 0), el VB recibe 1 ->
	  1 caja = 1 unidad.
	- No se toca CompraDetalle ni CompraDetalleType: estas 6 columnas son solo de
	  lectura/calculo en pantalla (igual que en Movimientos), no se graban. El
	  Paso 2 (AccesoLogica.vb) arma una proyeccion aparte con exactamente las
	  columnas que espera el TVP antes de invocar el SP, para que esto no rompa
	  ComprasInsertar/ComprasModificar.
	- Ninguna de las dos consultas (@tipo=4 y @tipo=5) tiene GROUP BY, asi que no
	  aplica el problema de Msg 8120 que si tuvimos que corregir en Movimientos.

	AJUSTE (despues del Paso 1): a diferencia de Movimientos -que calcula Cantidad
	Caja en VB despues de cargar el detalle-, aca se agrego CantidadCaja YA
	CALCULADA en el propio @tipo=4 (CantidadCompra / Conversion), para poder
	verificar el valor con una simple consulta SQL sin depender de que el
	.exe este recompilado con el ultimo cambio de VB. El VB igual recalcula este
	valor al cargar (es inofensivo, da el mismo resultado) como respaldo.

	Recomendacion: correr esto primero en un ambiente de pruebas/desarrollo y
	confirmar que el SP recompila sin error antes de seguir con el Paso 2 (VB).
*/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[MAM_Compras] (@tipo int,@Id int=-1,@AlmacenId int =-1,
@FechaTransaccion date=null,@Proveedor int=-1,@TipoVenta int=-1,
@FechaVencimientoCredito date=null,@Moneda int=-1,@Estado int=-1,@Glosa nvarchar(450)='',
@TotalCompra decimal(18,2)=0,@Descuento decimal(18,2)=0,@Usuario nvarchar(10)='',@detalle CompraDetalleType ReadOnly, @CatCosto int=-1,
@EmpresaId int=-1,@desde date=null,@hasta date =null)

AS
BEGIN
	DECLARE @newHora nvarchar(5)
	set @newHora=CONCAT(DATEPART(HOUR,GETDATE()),':',DATEPART(MINUTE,GETDATE()))
	declare @numicat int
	declare @ygmer int
	declare @numicatVenta int
	DECLARE @newFecha date
	Declare @IdCajaIngresoEgreso int
	declare @Prec decimal(18,4)
	set @newFecha=GETDATE()

	IF @tipo=-1 --ELIMINAR REGISTRO
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY
		   delete from CajaIngresoEgreso  where Modulo =2 and IdModulo =@Id and CajaId =1

update Configuracion set TotalCajaGeneral=(
select Sum((c.monto*a.Operacion)) as Total
from CajaIngresoEgreso as c
inner join CajaTipoMovimiento as a on c.CajatipoMovimientoId =a.Id
 where CajaId =1)


		delete from CreditosCompras  where CompraId  =@Id
		DELETE FROM CompraDetalle   WHERE CompraId   =@Id
			DELETE from Compras    where Id   =@Id

			select @Id as newNumi  --Consulcar que hace newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@Usuario)
		END CATCH
	END

	IF @tipo=1 --NUEVO REGISTRO
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY
			begin tran Tr_UpdateTI001
        set @Id=IIF((select COUNT(Id) from Compras)=0,0,(select MAX(Id) from Compras))+1
			INSERT INTO Compras VALUES(@Id ,@AlmacenId,@FechaTransaccion ,@Proveedor  ,@TipoVenta,
			@FechaVencimientoCredito,@Moneda ,@Estado  ,@Glosa ,@Descuento  ,@TotalCompra ,@EmpresaId,@newFecha,@newHora,@Usuario)


				------Modifico los Precios Costo------------------



			set @numicat =(select Min(Id ) from PreciosCategorias  where Tipo  =0)

			update Precios  set Precios.Precio  =td.PrecioCosto
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId
			and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

			--	update Precios  set Precios.Precio  =(((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )*Precios.Precio  )+(td.Cantidad *td.PrecioCosto ))/((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )+td.Cantidad )
			--from Precios INNER JOIN @detalle AS td
			--ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId
			--and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

			----INSERTO EL DEcaLLE
				INSERT INTO CompraDetalle (CompraId ,ProductoId ,CantidadCompra,PorcentajeIncremento,CantidadIncremento ,Cantidad  ,PrecioCosto,Lote
				 ,FechaVencimiento   ,PrecioVenta   ,TotalCompra
				,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro  )

			SELECT @Id,td.ProductoId ,td.CantidadCompra,td.PorcentajeIncremento,td.CantidadIncremento ,td.Cantidad  ,td.PrecioCosto  ,
			td.Lote  ,td.FechaVencimiento  ,td.PrecioVenta ,td.TotalCompra  ,
			@newFecha  ,@newHora  ,@Usuario    FROM @detalle AS td
			where td.estado  =0 and  td.ProductoId  >0 and td.Cantidad >0


			------Modifico los Precios Venta------------------
			set @numicatVenta =(select Min(Id) from PreciosCategorias where Tipo =1)
			update Precios set Precios.Precio  =td.PrecioVenta
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicatVenta and Precios .ProductoId  =td.ProductoId
			and Precios.AlmacenId  =@AlmacenId   and td.estado =0 and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0
			select @Id as newNumi




		----Modulo 2= compras
			if (not exists(select a.* from CajaIngresoEgreso as a where a.Modulo =2 and a.IdModulo =@Id and a.CajaId =1) and @TipoVenta =1)
			begin


			set @IdCajaIngresoEgreso=IIF((select COUNT(Id) from CajaIngresoEgreso)=0,0,(select MAX(Id) from CajaIngresoEgreso))+1
			insert into CajaIngresoEgreso values(@IdCajaIngresoEgreso,0,@FechaTransaccion ,'Egreso Por Compras A Proveedor '+(isnull((select top 1 pr.NombreProveedor  from Proveedor as pr where pr.Id =@Proveedor ),'')),
			@TotalCompra ,1,0,20,1,2,@Id,@AlmacenId ,0,0,@Usuario,@newFecha,@newHora )




update Configuracion set TotalCajaGeneral=(
select Sum((c.monto*a.Operacion)) as Total
from CajaIngresoEgreso as c
inner join CajaTipoMovimiento as a on c.CajatipoMovimientoId =a.Id
 where CajaId =1)

			end
			commit tran Tr_UpdateTI001

		END TRY
		BEGIN CATCH

		Print ERROR_MESSAGE()
		rollback tran Tr_UpdateTI001
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),1,@newFecha,@newHora,@Usuario)
		END CATCH
	END

	IF @tipo=2--MODIFICACION
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY


			--ELIMINO LOS REGISTROS
			DELETE FROM CompraDetalle  WHERE CompraId=@Id

			Update Compras set AlmacenId=@AlmacenId ,ProveedorId =@Proveedor ,TipoVenta =@TipoVenta ,
			FechaVencimientoCredito =@FechaVencimientoCredito ,Moneda =@Moneda , Estado =@Estado ,
			Glosa =@Glosa ,TotalCompra =@TotalCompra ,EmpresaId =@EmpresaId ,descuento=@Descuento
			where Id=@Id

			------Modifico los Precios Costo------------------


				---Tipo =1 Venta   0=Compra


			set @numicat =(select Min(Id ) from PreciosCategorias  where Tipo  =0)

			update Precios  set Precios.Precio  =(((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )*Precios.Precio  )+(td.Cantidad *td.PrecioCosto ))/((select Sum(a.Cantidad) from ProductosStock as a where a.ProductoId =td.ProductoId )+td.Cantidad )
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId
			and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

			--update Precios  set Precios.Precio  =td.PrecioCosto
			--from Precios INNER JOIN @detalle AS td
			--ON  Precios .PrecioCategoriaId  =@numicat and Precios .ProductoId  =td.ProductoId
			--and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0

		 ----------MODIFICO EL DEcaLLE DE EQUIPO------------
			--INSERTO LOS NUEVOS



					INSERT INTO CompraDetalle (CompraId ,ProductoId ,CantidadCompra,PorcentajeIncremento,CantidadIncremento ,Cantidad  ,PrecioCosto,Lote
				 ,FechaVencimiento   ,PrecioVenta   ,TotalCompra
				,FechaRegistro  ,HoraRegistro  ,UsuarioRegistro  )

			SELECT @Id,td.ProductoId ,td.CantidadCompra,td.PorcentajeIncremento,td.CantidadIncremento ,td.Cantidad  ,td.PrecioCosto  ,
			td.Lote  ,td.FechaVencimiento  ,td.PrecioVenta ,td.TotalCompra  ,
			@newFecha  ,@newHora  ,@Usuario    FROM @detalle AS td
			where td.estado  >=0 and  td.ProductoId  >0 and td.Cantidad >0


			------Modifico los Precios Venta------------------
			set @numicatVenta =(select Min(Id) from PreciosCategorias where Tipo =1)
			update Precios set Precios.Precio  =td.PrecioVenta
			from Precios INNER JOIN @detalle AS td
			ON  Precios .PrecioCategoriaId  =@numicatVenta and Precios .ProductoId  =td.ProductoId
			and Precios.AlmacenId  =@AlmacenId   and td.estado in (0,2) and  td.ProductoId  >0 and @estado >0 and td.Cantidad >0
			select @Id as newNumi

			-----Elimino  Caja
		   delete from CajaIngresoEgreso  where Modulo =2 and IdModulo =@Id and CajaId =1

		----Modulo 2= compras
			if (not exists(select a.* from CajaIngresoEgreso as a where a.Modulo =2 and a.IdModulo =@Id and a.CajaId =1) and @TipoVenta =1)
			begin


			set @IdCajaIngresoEgreso=IIF((select COUNT(Id) from CajaIngresoEgreso)=0,0,(select MAX(Id) from CajaIngresoEgreso))+1
			insert into CajaIngresoEgreso values(@IdCajaIngresoEgreso,0,@FechaTransaccion ,'Egreso Por Compras A Proveedor '+(isnull((select top 1 pr.NombreProveedor  from Proveedor as pr where pr.Id =@Proveedor ),'')),
			@TotalCompra ,1,0,20,1,2,@Id,@AlmacenId,0,0,@Usuario,@newFecha,@newHora )

update Configuracion set TotalCajaGeneral=(
select Sum((c.monto*a.Operacion)) as Total
from CajaIngresoEgreso as c
inner join CajaTipoMovimiento as a on c.CajatipoMovimientoId =a.Id
 where CajaId =1)

			end



			select @Id as newNumi
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora(banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),2,@newFecha,@newHora,@Usuario)
		END CATCH
	END

	IF @tipo=3 --MOSTRaR TODOS
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY


		select a.Id ,a.AlmacenId ,a.FechaTransaccion ,a.ProveedorId,p.NombreProveedor,a.TipoVenta ,a.FechaVencimientoCredito ,
		a.Moneda ,IIF(a.Moneda =1,'Boliviano','Dolar')as TituloMoneda,a.Estado ,a.Glosa ,a.TotalCompra ,a.EmpresaId  ,a.descuento,
		IIF(Exists(select t.* from TransaccionComprasCredito as t inner join CreditosCompras as c on c.Id =t.CreditoCompraId and c.CompraId =a.id),1,0) as transaccion
		from Compras as a
		inner join Proveedor as p on p.id=a.ProveedorId
		where a.FechaTransaccion >=@desde and a.FechaTransaccion <=@hasta
		order by a.id desc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=4 --MOSTRaR Decalle
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY


		--Se agregan UnidadVentaId/UnidadVenta/UnidadMaximaId/UnidadMaxima/Conversion/CantidadCaja
		--(mismo patron que MAM_Movimientos @tipo=4, mas CantidadCaja ya calculada): solo
		--lectura para el VB, no se graban en CompraDetalle ni forman parte de CompraDetalleType.
		select d.Id ,d.CompraId ,d.ProductoId ,p.NombreProducto as Producto,isnull(d.CantidadCompra,d.Cantidad ) as CantidadCompra ,isnull(d.PorcentajeIncremento,0) as PorcentajeIncremento,isnull(d.CantidadIncremento,0) as CantidadIncremento ,d.Cantidad ,d.PrecioCosto ,
		d.Lote,d.FechaVencimiento ,d.TotalCompra ,d.PrecioVenta ,1 as estado,cast('' as image) as img,
		d.PrecioCosto as costo,d.PrecioVenta  as venta,
		p.UnidadVentaId ,unidadMin.Descripcion as UnidadVenta ,p.UnidadMaximaId ,unidadMax.Descripcion as UnidadMaxima ,ISNULL(NULLIF(p.Conversion,0),1) as Conversion,
		isnull(d.CantidadCompra,d.Cantidad ) / ISNULL(NULLIF(p.Conversion,0),1) as CantidadCaja
		from CompraDetalle as d
		inner join Productos as p on p.Id =d.ProductoId
		left join ClasificadorDetalle as unidadMin on unidadMin.id=p.UnidadVentaId
		left join ClasificadorDetalle as unidadMax on unidadMax.id=p.UnidadMaximaId
		where d.CompraId  =@Id
		order by d.Id asc
		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=5 --MOSTRaR Productos Para Comprar
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY


 Declare @CategoriaCompra int,@CategoriaVenta int
	 set @CategoriaCompra =(select Min(k.Id ) from PreciosCategorias  as k where k.Tipo  =0)
	 set @CategoriaVenta =(select Min(k.Id ) from PreciosCategorias  as k where k.Tipo  =1)

	 --Se agregan UnidadVentaId/UnidadVenta/UnidadMaximaId/UnidadMaxima/Conversion (mismo
	 --patron que MAM_Movimientos @tipo=7): permite mostrar/calcular Cantidad Caja en el
	 --detalle de la compra al elegir el producto.
	 select a.Id ,a.CodigoExterno ,a.NombreProducto,a.NombreProducto as DescripcionProducto,industria .Descripcion as industria,cat.NombreCategoria   ,PCosto .Precio as PrecioCosto,PVenta .Precio as PrecioVenta,1 as estado,
	 isnull((select Sum(st.Cantidad ) from ProductosStock as st inner join
	 Almacenes as alma on  st.ProductoId =a.Id and st.DepositoId=alma .DepositoId and alma.id =@AlmacenId    ),0) as stock,
	 a.UnidadVentaId ,unidadMin.Descripcion as UnidadVenta ,a.UnidadMaximaId ,unidadMax.Descripcion as UnidadMaxima ,ISNULL(NULLIF(a.Conversion,0),1) as Conversion
	 from Productos as a
	 inner join Precios as PCosto on PCosto .PrecioCategoriaId =@CategoriaCompra
	 and PCosto.ProductoId =a.Id
	 inner join Precios as PVenta on PVenta .PrecioCategoriaId =@CategoriaVenta
	 and PVenta .ProductoId =a.Id
	 and PCosto .AlmacenId =@AlmacenId
	 and PVenta .AlmacenId =@AlmacenId
	 inner join Categorias as cat on cat.id=a.CategoriaId
	 inner join ClasificadorDetalle as industria on industria .id=a.AttributoId
	 left join ClasificadorDetalle as unidadMin on unidadMin.id=a.UnidadVentaId
	 left join ClasificadorDetalle as unidadMax on unidadMax.id=a.UnidadMaximaId
	 order by a.id asc

		END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=6 --MOSTRaR Proveedores
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY

		select a.Id ,a.NombreProveedor as Nombre ,a.Direccion ,a.Telefono01
		from Proveedor as a
		order by a.Id asc
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

	IF @tipo=7 --Reporte de Compras
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY


select a.Id ,sucursal .NombreAlmacen ,FORMAT (a.FechaTransaccion , 'dd-MM-yyyy')  as FechaTransaccion,IIF(a.TipoVenta=1,'Contado','Credito') as TipoCompra ,
IIF(a.TipoVenta=1,'',FORMAT (a.FechaVencimientoCredito , 'dd-MM-yyyy') ) as FechaVencimientoCredito,prov .NombreProveedor
,a.Glosa ,pro.Id as ProductoId,pro.NombreProducto ,detalle .PrecioCosto ,detalle .Cantidad ,detalle .TotalCompra
from Compras as a inner join
CompraDetalle as detalle on detalle .CompraId =a.Id
inner join Almacenes as sucursal on sucursal .id =a.AlmacenId
inner join Proveedor as prov on prov .Id =a.ProveedorId
inner join Productos as pro on pro.Id =detalle .ProductoId
where a.Id =@Id
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

IF @tipo=8 --MOSTRaR Personal
	BEGIN
		 SET ARITHABORT ON
		BEGIN TRY

		select a.Id ,a.NombrePersonal  as Nombre ,a.Direccion ,a.Telefono01
		from Personal  as a
		order by a.Id asc
				END TRY
		BEGIN CATCH
			INSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)
				   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),3,@newFecha,@newHora,@Usuario)
		END CATCH

END

End
GO
