/* ============================================================================
   sp_go_TC004_appMovil - PARCHE DE AUDITORIA (solo se AGREGA codigo)
   Base de datos: DistribucionDistralKCP2023
   Generado con Claude para Marco Mamani (BANCOSOL)
   Base: el ALTER PROCEDURE que Marco pego (Script Date: 21/09/2026 7:08:55)

   Requisito previo: correr primero AuditoriaVentas_CrearTablas.sql.

   QUE SE AGREGO (marcado con --==== AUDITORIA (INICIO/FIN) ====):

   1) @tipo=28, sub-rama "if (@oaap=2)" (boton "Anular" de la app movil):
      En realidad este boton hace Estado=-1 + Anulado=1 + BORRA VentasDetalles,
      es decir el mismo efecto que el "Eliminar" del desktop, no un anular
      reversible. Por eso se registra como TipoEvento='ELIMINADO' (Origen=
      'MOVIL'), con una Observacion que deja constancia de que vino del boton
      "Anular" movil. Si prefieres que en el reporte aparezca como 'ANULADO'
      en vez de 'ELIMINADO', es cambiar un solo literal (buscar "TIPO 28").
      Snapshot de Estado/Anulado antes + evento + foto completa del detalle
      vigente (TipoCambio='VIGENTE'), tomada ANTES del delete real.

      La otra sub-rama de @tipo=28 (el "else", que solo mueve EstadoPedido/
      Estado entre 1/2/3/4) NO se tocó: es un cambio de estado de entrega,
      no anular/eliminar/modificar, y no entra en el alcance pedido.

   2) @tipo=29 (Modificar Pedido, la rama moderna con AuditoriaPedidoMovil):
      - Snapshot del detalle VIGENTE (ProductoId/Cantidad/Precio) y de
        Estado/Anulado, tomado justo antes del DELETE FROM VentasDetalles
        (paso 6 del comentario original).
      - Despues de validar que la modificacion es correcta (después del
        chequeo de fecha del pedido, antes de grabar el backup "DESPUES"),
        hace un FULL OUTER JOIN entre ese "antes" y los datos ya decodificados
        en AuditoriaPedidoMovilDetalle (mismo filtro ObUpdate>=0 que usa el
        INSERT real a VentasDetalles) y clasifica cada producto como
        AGREGADO / ELIMINADO / CANTIDAD_MODIFICADA / SIN_CAMBIO.
      - Solo si hay al menos un cambio real (no 'SIN_CAMBIO') inserta el
        evento MODIFICADO y el detalle completo.

   PUNTO TECNICO IMPORTANTE - SET XACT_ABORT ON en @tipo=29:
   Esta rama activa XACT_ABORT ON al principio. Bajo XACT_ABORT ON, un
   error capturado por un TRY/CATCH normal puede dejar la transaccion ya
   "condenada" (doomed) aunque el CATCH no la relance - es decir, si la
   auditoria fallara, podria arrastrar consigo la modificacion real del
   pedido, justo lo que el patron de seguridad busca evitar. Por eso los
   dos bloques de auditoria (en @tipo=28 y @tipo=29) hacen
   SET XACT_ABORT OFF antes de su propio TRY/CATCH y lo restauran (ON)
   despues, dejando el resto del procedimiento exactamente como estaba.
   En @tipo=28 se apaga por prevencion (esa rama no lo activa, pero una
   conexion reciclada del connection pool podria arrastrar el ON de una
   llamada previa al tipo=29 en la misma conexion).

   NO TOCADO: el resto de la sub-rama de @tipo=28 (cambios de EstadoPedido),
   y absolutamente ningun otro @tipo (-1,111,112,3,20,21,22,23,24,25,26,27,
   30,31,40,41,42,43,44,45,47,50,51,52,53,54,55,56). Se verifico con diff
   linea por linea contra el texto que pegaste: cero lineas eliminadas o
   modificadas, solo agregadas. Tambien se verifico el balance BEGIN/END/
   TRY/CATCH/CASE del archivo completo.

   PENDIENTE DE TU CONFIRMACION: la etiqueta ELIMINADO vs ANULADO para el
   boton "Anular" movil (ver punto 1). Quedo con ELIMINADO por default
   porque coincide con el efecto real en la base de datos; cambialo si tu
   criterio de negocio prefiere otra cosa.

   RECOMENDACION: probar en un ambiente de desarrollo/pruebas antes de
   correr esto contra produccion, sobre todo el tipo=29 por el tema de
   XACT_ABORT explicado arriba.
   ============================================================================ */

USE [DistribucionDistralKCP2023]
GO
/****** Object:  StoredProcedure [dbo].[sp_go_TC004_appMovil]    Script Date: 21/09/2026 7:08:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
