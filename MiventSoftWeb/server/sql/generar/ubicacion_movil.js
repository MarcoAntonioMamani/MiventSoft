// =====================================================================
//  Genera 07_UbicacionMovil.sql y revertir/REVERTIR_07_UbicacionMovil.sql para la BD conectada (Distral)
//    node sql/generar/ubicacion_movil.js                 (BD de server/.env)
//    node sql/generar/ubicacion_movil.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee sp_go_TC004_appMovil de ESA base (YA con el 06 instalado) y registra, en la tabla nueva
//  VentasRegistroMovil, donde y cuando el celular levanto el pedido (@26) y lo entrego / anulo (@28):
//  GPS, precision, fecha y hora del celular y del servidor, modelo del celular y version de la app.
//  Agrega el SP MAM_VentasRegistroMovil y el programa web btnRegistroCelular.
//  Si el codigo de esa base es distinto, se detiene sin generar nada.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'sp_go_TC004_appMovil';
const MARCA = 'Sanbit: ubicacion del celular';
const MARCA_06 = 'Sanbit: incremento del dia';

/** Reemplaza "viejo" por "nuevo" entre desde..hasta; tiene que aparecer exactamente "veces" veces */
function cambiar(s, viejo, nuevo, desde, hasta, veces, nombre) {
    const tramo = s.slice(desde, hasta);
    const n = tramo.split(viejo).length - 1;
    if (n !== veces) falla(`${nombre}: se esperaba ${veces} vez/veces y aparece ${n}. El SP de esta base es distinto, revisarlo a mano`);
    return s.slice(0, desde) + tramo.split(viejo).join(nuevo) + s.slice(hasta);
}

// Parametros de cada evento: el pedido usa los de siempre (@latitud/@longitud) + los nuevos; la entrega los suyos
const G_PEDIDO = { lat: '@latitud', lng: '@longitud', prec: '@gpsPrecision', fh: '@fechaHoraCelular' };
const G_ENTREGA = { lat: '@latEntrega', lng: '@lngEntrega', prec: '@precisionEntrega', fh: '@fechaHoraEntrega' };
const EVENTO_ENTREGA = "CASE WHEN @oaap = 2 THEN 'ANULADO' ELSE 'ENTREGADO' END";

// Fila de VentasRegistroMovil (solo la app nueva manda @modeloCelular). 0,0 = el celular no tenia ubicacion.
// Si el celular reenvia lo mismo (reintento sin internet), no se duplica: misma venta, evento y hora del celular.
function insertar(evento, personal, g, condicion) {
    const fh = `TRY_CONVERT(datetime2(0), NULLIF(${g.fh}, ''), 120)`;
    return [
        `\t\tIF (@modeloCelular <> '' AND ${condicion} AND NOT EXISTS (SELECT 1 FROM VentasRegistroMovil AS rm`,
        `\t\t\tWHERE rm.VentaId = @oanumi AND rm.Evento = ${evento} AND rm.FechaHoraCelular = ${fh}))`,
        '\t\t\tINSERT INTO VentasRegistroMovil (VentaId, Evento, EstadoPedido, PersonalId, Latitud, Longitud, PrecisionMetros,',
        '\t\t\t\tFechaHoraCelular, FechaHoraServidor, ModeloCelular, VersionAndroid, VersionApp)',
        `\t\t\tVALUES (@oanumi, ${evento}, @oaest, ${personal},`,
        `\t\t\t\tIIF(${g.lat} = 0 AND ${g.lng} = 0, NULL, ${g.lat}), IIF(${g.lat} = 0 AND ${g.lng} = 0, NULL, ${g.lng}),`,
        `\t\t\t\tIIF(${g.prec} < 0, NULL, ${g.prec}), ${fh}, SYSDATETIME(),`,
        '\t\t\t\tLEFT(@modeloCelular, 120), LEFT(@versionAndroid, 30), LEFT(@versionApp, 30))',
    ].join(NL);
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    const d0 = def[SP];
    if (d0.includes(MARCA)) falla(`${SP} de ${bd} ya tiene el 07: no hace falta generar nada`);
    if (!d0.includes(MARCA_06)) falla(`${SP} de ${bd} NO tiene el 06 (IncrementoMovil): instalar primero 01..06 y generar despues`);
    if (/@modeloCelular\b/i.test(d0)) falla(`${SP} de ${bd} ya tiene un parametro @modeloCelular: revisarlo a mano`);

    let d = d0;
    // 1. Parametros nuevos (opcionales): la app vieja no los manda -> no se registra nada
    const p = buscar(d, /@incremento decimal\(18,2\)=-1,/i, 0, d.length, 'parametro @incremento (06)');
    d = cambiar(d, '@incremento decimal(18,2)=-1,', '@incremento decimal(18,2)=-1,'
        + "@gpsPrecision decimal(10,2)=-1,@fechaHoraCelular nvarchar(30)='',@modeloCelular nvarchar(120)='',"
        + "@versionAndroid nvarchar(30)='',@versionApp nvarchar(30)='',"
        + "@latEntrega decimal(18,16)=0,@lngEntrega decimal(18,16)=0,@precisionEntrega decimal(10,2)=-1,@fechaHoraEntrega nvarchar(30)='',",
        p.idx, p.fin, 1, 'parametros');

    // 2. @26 pedido nuevo: despues de la fila del incremento (06), misma transaccion. Un pedido que ya llego no entra aqui.
    let a = buscar(d, /IF\s+@tipo\s*=\s*26\s*--INSERTAR PEDIDO NUEVO/i, 0, d.length, '@tipo=26').idx;
    let b = buscar(d, /IF\s+@tipo\s*=\s*27\s*--INSERTAR DETALLE PEDIDO NUEVO/i, a, d.length, '@tipo=27').idx;
    const inc = buscar(d, /INSERT INTO VentasIncrementoAplicado \(VentaId, Monto, FechaHoraRegistro\) VALUES \(@oanumi, @incremento, SYSDATETIME\(\)\)\r?\n/i, a, b, '@26 fila del incremento (06)');
    const bloque26 = [
        `\t\t-- ===== ${MARCA} (app nueva): donde y cuando se levanto el pedido =================`,
        insertar("'PEDIDO'", '@oarepa', G_PEDIDO, '1 = 1'),
        '\t\t-- venta directa, o entregado sin internet antes de subir: tambien la fila de la entrega',
        insertar(EVENTO_ENTREGA, '@oarepa', G_ENTREGA, "@fechaHoraEntrega <> ''"),
        '',
    ].join(NL);
    d = d.slice(0, inc.fin) + bloque26 + d.slice(inc.fin);

    // 3. @28 entregar / anular: antes de devolver la confirmacion (solo si la venta existe)
    a = buscar(d, /IF\s*@tipo\s*=\s*28\s*--INSERTAR PEDIDO NUEVO/i, 0, d.length, '@tipo=28').idx;
    b = buscar(d, /IF\s*@tipo\s*=\s*29\s*--\s*MODIFICAR PEDIDO/i, a, d.length, '@tipo=29').idx;
    const conf = buscar(d, /\r?\n[ \t]*SELECT ventas\.id as code_id\s+from ventas where id=@oanumi/i, a, b, '@28 confirmacion');
    const bloque28 = [
        '',
        `\t\t-- ===== ${MARCA} (app nueva): donde y cuando se entrego / anulo =================`,
        insertar(EVENTO_ENTREGA, 'IIF(@idRepartidor > 0, @idRepartidor, NULL)', G_ENTREGA, "@fechaHoraEntrega <> ''"),
    ].join(NL);
    d = d.slice(0, conf.idx) + bloque28 + d.slice(conf.idx);

    const hoy = new Date().toISOString().slice(0, 10);
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const yaEsta = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const TABLA = [
        "IF OBJECT_ID('dbo.VentasRegistroMovil') IS NULL",
        'BEGIN',
        '\tCREATE TABLE dbo.VentasRegistroMovil (',
        '\t\tId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_VentasRegistroMovil PRIMARY KEY,',
        '\t\tVentaId int NOT NULL,',
        '\t\tEvento nvarchar(20) NOT NULL,            -- PEDIDO / ENTREGADO / ANULADO',
        '\t\tEstadoPedido int NULL,                   -- oaest que mando el celular',
        '\t\tPersonalId int NULL,                     -- preventista (PEDIDO) o repartidor (ENTREGADO)',
        '\t\tLatitud decimal(18,16) NULL,             -- NULL = el celular no tenia ubicacion',
        '\t\tLongitud decimal(18,16) NULL,',
        '\t\tPrecisionMetros decimal(10,2) NULL,',
        '\t\tFechaHoraCelular datetime2(0) NULL,      -- reloj del celular (cuando lo hizo, aunque no tuviera internet)',
        '\t\tFechaHoraServidor datetime2(0) NOT NULL, -- cuando llego al servidor',
        '\t\tModeloCelular nvarchar(120) NULL,',
        '\t\tVersionAndroid nvarchar(30) NULL,',
        '\t\tVersionApp nvarchar(30) NULL)',
        '\tCREATE INDEX IX_VentasRegistroMovil_Venta ON dbo.VentasRegistroMovil (VentaId, Evento)',
        '\tCREATE INDEX IX_VentasRegistroMovil_Fecha ON dbo.VentasRegistroMovil (FechaHoraServidor) INCLUDE (VentaId, Evento, PersonalId)',
        'END',
    ].join(NL);
    const SELECT_WEB = [
        '\t\tSELECT r.Id, r.VentaId, r.Evento, r.EstadoPedido, r.PersonalId, RTRIM(p.NombrePersonal) AS NombrePersonal,',
        '\t\t\tRTRIM(c.NombreCliente) AS NombreCliente, v.TotalVenta,',
        '\t\t\tCONVERT(varchar(40), r.Latitud) AS Latitud, CONVERT(varchar(40), r.Longitud) AS Longitud, r.PrecisionMetros,',
        '\t\t\tr.FechaHoraCelular, r.FechaHoraServidor, r.ModeloCelular, r.VersionAndroid, r.VersionApp',
        '\t\tFROM VentasRegistroMovil AS r',
        '\t\tINNER JOIN Ventas AS v ON v.Id = r.VentaId',
        '\t\tLEFT JOIN Clientes AS c ON c.Id = v.ClienteId',
        '\t\tLEFT JOIN Personal AS p ON p.Id = r.PersonalId',
    ].join(NL);
    const SP_WEB = [
        "IF OBJECT_ID('dbo.MAM_VentasRegistroMovil') IS NULL EXEC('CREATE PROCEDURE dbo.MAM_VentasRegistroMovil AS RETURN 0')",
        'GO',
        '-- Sanbit: ubicacion del celular. Lo usa la web (pedidos y entregas registrados desde la app).',
        '--   @tipo 1: registros de una venta (@VentaId)',
        '--   @tipo 2: registros entre dos fechas del servidor (@FechaDesde..@FechaHasta), opcional de un personal (@PersonalId)',
        "ALTER PROCEDURE dbo.MAM_VentasRegistroMovil (@tipo int, @VentaId int = -1, @FechaDesde date = NULL, @FechaHasta date = NULL,",
        "\t@PersonalId int = -1, @usuario nvarchar(50) = '')",
        'AS',
        'BEGIN',
        '\tSET NOCOUNT ON',
        '\tIF @tipo = 1',
        SELECT_WEB,
        '\t\tWHERE r.VentaId = @VentaId',
        '\t\tORDER BY r.Id',
        '\tIF @tipo = 2',
        SELECT_WEB,
        '\t\tWHERE r.FechaHoraServidor >= @FechaDesde AND r.FechaHoraServidor < DATEADD(DAY, 1, @FechaHasta)',
        '\t\t\tAND (@PersonalId <= 0 OR r.PersonalId = @PersonalId)',
        '\t\tORDER BY r.Id',
        'END',
        'GO',
    ].join(NL);
    const PROGRAMA = [
        '-- Programa de la web "Ubicacion de pedidos y entregas" (btnRegistroCelular), en el modulo de los reportes de',
        '-- distribucion, asignado al rol Administrador (1). A otros roles se asigna en Roles de Usuario.',
        "IF NOT EXISTS (SELECT 1 FROM Programas WHERE IdPrograma = 'btnRegistroCelular')",
        'BEGIN',
        '\tDECLARE @IdProg int, @ModuloProg int',
        "\tSET @ModuloProg = ISNULL((SELECT TOP 1 Modulo FROM Programas WHERE IdPrograma = 'btnRepAuditoriaVentas'), 1)",
        "\tIF COLUMNPROPERTY(OBJECT_ID('dbo.Programas'), 'Id', 'IsIdentity') = 1",
        '\tBEGIN',
        "\t\tINSERT INTO Programas (IdPrograma, DescripcionPrograma, Modulo) VALUES ('btnRegistroCelular', 'UBICACION PEDIDOS Y ENTREGAS', @ModuloProg)",
        '\t\tSET @IdProg = SCOPE_IDENTITY()',
        '\tEND',
        '\tELSE',
        '\tBEGIN',
        '\t\tSET @IdProg = ISNULL((SELECT MAX(Id) FROM Programas), 0) + 1',
        "\t\tINSERT INTO Programas (Id, IdPrograma, DescripcionPrograma, Modulo) VALUES (@IdProg, 'btnRegistroCelular', 'UBICACION PEDIDOS Y ENTREGAS', @ModuloProg)",
        '\tEND',
        "\tIF COLUMNPROPERTY(OBJECT_ID('dbo.RolesDetalles'), 'Id', 'IsIdentity') = 1",
        '\t\tINSERT INTO RolesDetalles (RolId, ProgramaId, Ver, Insertar, Modificar, Eliminar)',
        '\t\tSELECT r.Id, @IdProg, IIF(r.Id = 1, 1, 0), 0, 0, 0 FROM Roles AS r',
        '\tELSE',
        '\t\tINSERT INTO RolesDetalles (Id, RolId, ProgramaId, Ver, Insertar, Modificar, Eliminar)',
        '\t\tSELECT ISNULL((SELECT MAX(Id) FROM RolesDetalles), 0) + ROW_NUMBER() OVER (ORDER BY r.Id), r.Id, @IdProg, IIF(r.Id = 1, 1, 0), 0, 0, 0',
        '\t\tFROM Roles AS r',
        "\tRAISERROR('07: programa \"Ubicacion Pedidos Y Entregas\" creado y asignado al rol Administrador.', 10, 1) WITH NOWAIT",
        'END',
        'GO',
    ].join(NL);

    const cab = [
        '/* =====================================================================',
        '   07 - UBICACION, FECHA/HORA Y MODELO DEL CELULAR EN PEDIDOS Y ENTREGAS',
        '   1. Tabla nueva VentasRegistroMovil: una fila cada vez que la app nueva (/apiV4) levanta un pedido (PEDIDO)',
        '      o lo entrega / anula (ENTREGADO / ANULADO): GPS y precision, fecha y hora del celular y del servidor,',
        '      modelo del celular, version de Android y de la app.',
        `   2. ${SP}: parametros nuevos opcionales (@gpsPrecision, @fechaHoraCelular, @modeloCelular,`,
        '      @versionAndroid, @versionApp, @latEntrega, @lngEntrega, @precisionEntrega, @fechaHoraEntrega).',
        '      @26 y @28 graban la fila en la MISMA transaccion. Un reenvio del celular no se registra dos veces.',
        '      La app vieja (/apiV3) no los manda: sus pedidos quedan igual que hoy.',
        '   3. SP nuevo MAM_VentasRegistroMovil y programa web btnRegistroCelular (rol Administrador): reporte con mapa',
        '      de donde y cuando se levanto y se entrego cada pedido.',
        '   Todo lo demas del SP queda IDENTICO.',
        '',
        `   Generado desde ${bd} (${hoy}), con el 06 instalado. PROTECCION: solo se aplica si el SP es esa version`,
        '   (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da',
        '   error y no modifica nada.',
        '   REQUISITO: 06_IncrementoMovil.sql instalado.',
        '   VOLVER ATRAS: revertir\\REVERTIR_07_UbicacionMovil.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${yaEsta}`,
        'BEGIN',
        "\tRAISERROR('07 UbicacionMovil YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('07 UbicacionMovil: ${SP} es OTRA version (no la de ${bd} ${hoy} con el 06). Instalar antes 01..06. No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        TABLA,
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = ['SET NOEXEC OFF', 'GO', `IF ${yaEsta}`, "\tRAISERROR('07 UbicacionMovil INSTALADO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r1 = escribir(bd, '07_UbicacionMovil.sql', `${cab}${NL}${aAlter(d)}${NL}GO${NL}${SP_WEB}${NL}${PROGRAMA}${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 07_UbicacionMovil.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version con el 06 y borra el SP MAM_VentasRegistroMovil.`,
        '   La tabla VentasRegistroMovil y el programa btnRegistroCelular NO se borran (conservan lo registrado).',
        '   ANTES: volver a la app anterior (la nueva manda @modeloCelular, que sin el 07 no existe).',
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${yaEsta})`,
        'BEGIN',
        "\tRAISERROR('07 UbicacionMovil NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        `${aAlter(d0)}${NL}GO`,
        "IF OBJECT_ID('dbo.MAM_VentasRegistroMovil') IS NOT NULL DROP PROCEDURE dbo.MAM_VentasRegistroMovil",
        'GO',
        'SET NOEXEC OFF', 'GO',
        "RAISERROR('07 UbicacionMovil DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_07_UbicacionMovil.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
