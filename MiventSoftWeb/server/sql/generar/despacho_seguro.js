// =====================================================================
//  Genera 12_DespachoSeguro.sql y revertir/REVERTIR_12_DespachoSeguro.sql (Distral)
//    node sql/generar/despacho_seguro.js                 (BD de server/.env)
//    node sql/generar/despacho_seguro.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee MAM_DespachoProductos de ESA base. Cambia:
//   @-1 Eliminar despacho: NO si la conciliacion esta cerrada o si alguno de sus pedidos ya se entrego.
//       Si es el ULTIMO despacho de su conciliacion, la conciliacion se elimina con el (antes quedaba abierta
//       y vacia); no se permite si esa conciliacion ya tiene entregas, ventas directas, detalle o cierre de caja.
//       Borra tambien VentasDetallesDespacho (quedaba huerfana y, como el Id del despacho es MAX+1, se pegaba
//       al siguiente despacho). Todo en una transaccion.
//   @1 Nuevo despacho automatico: NO si un pedido ya esta en otro despacho abierto o ya no esta asignado
//       a ese personal (dos usuarios despachando a la vez / pantalla vieja).
//   @2 Modificar: NO si la conciliacion esta cerrada, si es automatico o si se cambia el personal (la
//       conciliacion es de otra persona: eliminar y crear de nuevo).
//  Las validaciones van ANTES de sus TRY: los CATCH originales solo anotan en Bitacora y no avisan al usuario.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'MAM_DespachoProductos';
const MARCA = 'Sanbit: despacho seguro';
const T = (n) => '\t'.repeat(n);
const lista = (select) => `STUFF((${select} FOR XML PATH(''), TYPE).value('.', 'nvarchar(1500)'), 1, 2, '')`;
const error = (n, msg, ...args) => [
    `${T(n)}BEGIN`,
    `${T(n + 1)}RAISERROR('${msg}', 16, 1${args.map((a) => `, ${a}`).join('')})`,
    `${T(n + 1)}RETURN`,
    `${T(n)}END`,
];

const ELIMINAR = [
    '\tIF @tipo=-1 --ELIMINAR REGISTRO',
    '\tBEGIN',
    `\t\t-- ${MARCA} (12): validar antes de borrar; si es el ultimo despacho, la conciliacion se va con el`,
    '\t\tDECLARE @Elim_Conci int, @Elim_EstadoConci int, @Elim_Lista nvarchar(1500), @Elim_BorrarConci bit = 0, @Elim_Error nvarchar(2048)',
    '\t\tSELECT @Elim_Conci = d.ConciliacionId, @Elim_EstadoConci = c.Estado',
    '\t\tFROM DespachoProductos AS d LEFT JOIN Conciliaciones AS c ON c.Id = d.ConciliacionId WHERE d.Id = @Id',
    '\t\tIF @Elim_Conci IS NULL',
    ...error(2, 'NO SE PUEDE ELIMINAR: el despacho %d ya no existe. Actualice la lista.', '@Id'),
    '\t\tIF ISNULL(@Elim_EstadoConci, 1) <> 1',
    ...error(2, 'NO SE PUEDE ELIMINAR EL DESPACHO %d: la conciliacion %d ya esta cerrada.', '@Id', '@Elim_Conci'),
    '\t\t-- pedidos de este despacho que el repartidor ya entrego',
    `\t\tSET @Elim_Lista = ${lista("SELECT TOP 10 CONCAT(', #', v.Id) FROM ConciliacionAutomaticoPedido AS cap INNER JOIN Ventas AS v ON v.Id = cap.ventaId WHERE cap.DespachoId = @Id AND v.EstadoPedido IN (3, 4) ORDER BY v.Id")}`,
    '\t\tIF @Elim_Lista IS NOT NULL',
    ...error(2, 'NO SE PUEDE ELIMINAR EL DESPACHO %d: estos pedidos ya se entregaron: %s. Revierta primero la entrega.', '@Id', '@Elim_Lista'),
    '\t\tIF NOT EXISTS (SELECT 1 FROM DespachoProductos WHERE ConciliacionId = @Elim_Conci AND Id <> @Id)',
    '\t\tBEGIN',
    '\t\t\t-- es el ultimo despacho: la conciliacion se elimina con el, salvo que ya tenga movimiento propio',
    `\t\t\tSET @Elim_Lista = ${lista("SELECT TOP 10 CONCAT(', #', v.Id, IIF(v.VentaDirecta = 1, ' venta directa', ' entregado')) FROM PedidosAsignaciones AS a INNER JOIN Ventas AS v ON v.Id = a.VentaId WHERE a.ConciliacionId = @Elim_Conci AND (v.EstadoPedido IN (3, 4) OR v.VentaDirecta = 1) ORDER BY v.Id")}`,
    '\t\t\tIF @Elim_Lista IS NOT NULL',
    ...error(3, 'NO SE PUEDE ELIMINAR EL DESPACHO %d: es el ultimo de la conciliacion %d y esa conciliacion ya tiene movimiento (%s). Cierrela desde Conciliacion.', '@Id', '@Elim_Conci', '@Elim_Lista'),
    '\t\t\tIF EXISTS (SELECT 1 FROM ConciliacionesDetalle WHERE ConciliacionId = @Elim_Conci) OR EXISTS (SELECT 1 FROM CierreCaja WHERE ConciliacionId = @Elim_Conci)',
    ...error(3, 'NO SE PUEDE ELIMINAR EL DESPACHO %d: es el ultimo de la conciliacion %d y esa conciliacion ya tiene detalle o cierre de caja.', '@Id', '@Elim_Conci'),
    '\t\t\tSET @Elim_BorrarConci = 1',
    '\t\tEND',
    '\t\tBEGIN TRAN',
    '\t\tBEGIN TRY',
    '\t\t    delete from DespachoProductosDetalle  where DespachoProductosId  =@Id ',
    '\t\t\tdelete from ConciliacionAutomaticoPedido where DespachoId =@Id ',
    '\t\t\tdelete from VentasDetallesDespacho where despachoId = @Id',
    '\t\t\tdelete from DespachoProductos    where Id=@Id;',
    '\t\t\tIF @Elim_BorrarConci = 1',
    '\t\t\tBEGIN',
    '\t\t\t\t-- los pedidos siguen asignados al mismo personal (como antes del despacho), sin conciliacion',
    '\t\t\t\tupdate PedidosAsignaciones set ConciliacionId = 0 where ConciliacionId = @Elim_Conci',
    '\t\t\t\tdelete from Conciliaciones where Id = @Elim_Conci',
    '\t\t\tEND',
    '\t\t\tCOMMIT TRAN',
    '\t\t\tSELECT 1 as resp, IIF(@Elim_BorrarConci = 1, @Elim_Conci, 0) as ConciliacionEliminada;',
    '\t\tEND TRY',
    '\t\tBEGIN CATCH',
    '\t\t\tIF @@TRANCOUNT > 0 ROLLBACK TRAN',
    '\t\t\tSET @Elim_Error = ERROR_MESSAGE()',
    '\t\t\tINSERT INTO Bitacora (banum,baproc,balinea,bamensaje,batipo,bafact,bahact,bauact)',
    '\t\t\t\t   VALUES(ERROR_NUMBER(),ERROR_PROCEDURE(),ERROR_LINE(),ERROR_MESSAGE(),-1,@newFecha,@newHora,@usuario )',
    "\t\t\tRAISERROR('NO SE PUDO ELIMINAR EL DESPACHO %d: %s', 16, 1, @Id, @Elim_Error)",
    '\t\tEND CATCH',
    '\tEND',
    '',
    '',
].join(NL);

const NUEVO = [
    `\t\t-- ${MARCA} (12): en un despacho automatico cada pedido entra una sola vez y solo al de su repartidor`,
    '\t\tIF @DespachoAutomatico = 1',
    '\t\tBEGIN',
    `\t\t\tDECLARE @Nuevo_Lista nvarchar(1500) = ${lista("SELECT TOP 10 CONCAT(', #', r.VentaId, ' (despacho ', cap.DespachoId, ')') FROM @ReferenciaVenta AS r INNER JOIN ConciliacionAutomaticoPedido AS cap ON cap.ventaId = r.VentaId INNER JOIN DespachoProductos AS dp ON dp.Id = cap.DespachoId INNER JOIN Conciliaciones AS cc ON cc.Id = dp.ConciliacionId AND cc.Estado = 1 ORDER BY r.VentaId")}`,
    '\t\t\tIF @Nuevo_Lista IS NOT NULL',
    ...error(3, 'NO SE PUEDE GRABAR EL DESPACHO: los pedidos %s ya estan en otro despacho. Actualice la pantalla.', '@Nuevo_Lista'),
    `\t\t\tSET @Nuevo_Lista = ${lista("SELECT TOP 10 CONCAT(', #', r.VentaId) FROM @ReferenciaVenta AS r WHERE NOT EXISTS (SELECT 1 FROM PedidosAsignaciones AS a WHERE a.VentaId = r.VentaId AND a.PersonalId = @PersonalId) ORDER BY r.VentaId")}`,
    '\t\t\tIF @Nuevo_Lista IS NOT NULL',
    ...error(3, 'NO SE PUEDE GRABAR EL DESPACHO: los pedidos %s ya no estan asignados a este personal. Actualice la pantalla.', '@Nuevo_Lista'),
    '\t\tEND',
    '',
].join(NL);

const MODIFICAR = [
    `\t\t-- ${MARCA} (12): no se modifica en conciliacion cerrada, ni el automatico, ni se cambia el personal`,
    '\t\tDECLARE @Mod_Conci int, @Mod_EstadoConci int, @Mod_Personal int, @Mod_Auto int',
    '\t\tSELECT @Mod_Conci = d.ConciliacionId, @Mod_EstadoConci = c.Estado, @Mod_Personal = c.PersonalID, @Mod_Auto = ISNULL(d.DespachoAutomatico, 0)',
    '\t\tFROM DespachoProductos AS d LEFT JOIN Conciliaciones AS c ON c.Id = d.ConciliacionId WHERE d.Id = @Id',
    '\t\tIF @Mod_Conci IS NULL',
    ...error(2, 'NO SE PUEDE MODIFICAR: el despacho %d ya no existe. Actualice la lista.', '@Id'),
    '\t\tIF ISNULL(@Mod_EstadoConci, 1) <> 1',
    ...error(2, 'NO SE PUEDE MODIFICAR EL DESPACHO %d: la conciliacion %d ya esta cerrada.', '@Id', '@Mod_Conci'),
    '\t\tIF @Mod_Auto = 1',
    ...error(2, 'NO SE PUEDE MODIFICAR EL DESPACHO %d: es automatico. Eliminelo y vuelva a crearlo.', '@Id'),
    '\t\tIF @Mod_Personal IS NOT NULL AND @PersonalId <> @Mod_Personal',
    ...error(2, 'NO SE PUEDE CAMBIAR EL PERSONAL DEL DESPACHO %d: pertenece a la conciliacion %d de otro personal. Eliminelo y creelo para el nuevo personal.', '@Id', '@Mod_Conci'),
    '',
].join(NL);

/** Inserta el bloque al inicio de la linea donde empieza `re` (buscado dentro del @tipo) */
function antesDe(s, reTipo, re, nombre, bloque) {
    const t = buscar(s, reTipo, 0, s.length, nombre);
    const x = buscar(s, re, t.fin, Math.min(s.length, t.fin + 400), `${re} de ${nombre}`);
    const ini = s.lastIndexOf('\n', x.idx) + 1;
    return s.slice(0, ini) + bloque + s.slice(ini);
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    const d0 = def[SP];
    if (d0.includes(MARCA)) falla(`${SP} de ${bd} ya tiene el 12: no hace falta generar nada`);

    // @-1 completo (desde su IF hasta el IF @tipo=1) se reemplaza
    const a = buscar(d0, /IF\s*@tipo\s*=\s*-1\s*--\s*ELIMINAR\s+REGISTRO/i, 0, d0.length, '@tipo=-1');
    const b = buscar(d0, /IF\s*@tipo\s*=\s*1\s*--\s*NUEVO\s+REGISTRO/i, a.fin, d0.length, '@tipo=1');
    const viejo = d0.slice(a.idx, b.idx);
    for (const x of ['delete from DespachoProductosDetalle', 'delete from DespachoProductos ', 'delete from ConciliacionAutomaticoPedido']) {
        if (!viejo.includes(x)) falla(`@-1 de ${SP} no es el esperado (falta "${x}"): revisarlo a mano`);
    }
    if (/Conciliaciones|VentasDetallesDespacho|BEGIN\s+TRAN/i.test(viejo)) falla(`@-1 de ${SP} ya hace mas cosas que la version conocida: revisarlo a mano`);
    // de linea completa a linea completa: el IF @tipo=1 conserva su sangria original
    let d = d0.slice(0, d0.lastIndexOf('\n', a.idx) + 1) + ELIMINAR + d0.slice(d0.lastIndexOf('\n', b.idx) + 1);

    d = antesDe(d, /IF\s*@tipo\s*=\s*2\s*--\s*MODIFICACION/i, /BEGIN\s+TRAN\s+MODIFICACION/i, '@tipo=2', MODIFICAR);
    d = antesDe(d, /IF\s*@tipo\s*=\s*1\s*--\s*NUEVO\s+REGISTRO/i, /BEGIN\s+TRAN\s+INSERTAR/i, '@tipo=1', NUEVO);
    if ((d.match(new RegExp(MARCA, 'g')) || []).length !== 3) falla('No se pudieron ubicar los 3 lugares (@-1, @1, @2)');

    const hoy = new Date().toISOString().slice(0, 10);
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const yaEsta = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const cab = [
        '/* =====================================================================',
        '   12 - DESPACHO SEGURO (eliminar / grabar / modificar despachos sin dejar datos sueltos)',
        `   Cambia SOLO ${SP}:`,
        '   - Eliminar (@-1): NO si la conciliacion esta cerrada o si algun pedido del despacho ya se entrego.',
        '     Si es el ULTIMO despacho de su conciliacion, la conciliacion se elimina con el (antes quedaba abierta y',
        '     vacia), salvo que ya tenga entregas, ventas directas, detalle o cierre de caja. Borra tambien',
        '     VentasDetallesDespacho del despacho. Todo en una transaccion.',
        '   - Nuevo automatico (@1): NO si un pedido ya esta en otro despacho abierto o ya no esta asignado a ese personal.',
        '   - Modificar (@2): NO en conciliacion cerrada, NO el automatico, NO cambiar el personal.',
        '   Todo lo demas del SP queda IDENTICO. Vale para web y escritorio.',
        '',
        `   Generado desde ${bd} (${hoy}). PROTECCION: solo se aplica si el SP es esa version (sin contar mayusculas`,
        '   ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da error y no modifica nada',
        '   (regenerar con: node sql/generar/despacho_seguro.js). No hace falta reiniciar la web ni la API.',
        '   VOLVER ATRAS: revertir\\REVERTIR_12_DespachoSeguro.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${yaEsta}`,
        'BEGIN',
        "\tRAISERROR('12 DespachoSeguro YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('12 DespachoSeguro: ${SP} es OTRA version (no la de ${bd} ${hoy}). No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = ['SET NOEXEC OFF', 'GO', `IF ${yaEsta}`, "\tRAISERROR('12 DespachoSeguro INSTALADO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r1 = escribir(bd, '12_DespachoSeguro.sql', `${cab}${NL}${aAlter(d)}${NL}GO${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 12_DespachoSeguro.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version original (eliminar despacho sin validar ni borrar la conciliacion).`,
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${yaEsta})`,
        'BEGIN',
        "\tRAISERROR('12 DespachoSeguro NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        `${aAlter(d0)}${NL}GO`,
        'SET NOEXEC OFF', 'GO',
        "RAISERROR('12 DespachoSeguro DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_12_DespachoSeguro.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
