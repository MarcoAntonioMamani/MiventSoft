// =====================================================================
//  Genera 08_Auditoria_SPs.sql y revertir/REVERTIR_08_Auditoria_SPs.sql para la BD conectada
//    node sql/generar/auditoria_sps.js                 (BD de server/.env)
//    node sql/generar/auditoria_sps.js --bd=OtraBD     (otra BD del mismo servidor)
//  Agrega a MAM_Ventas, MAM_Compras y MAM_Movimientos (@tipo 2 modificar y @tipo -1 eliminar) la llamada a
//  MAM_Auditoria (07_Auditoria.sql): foto ANTES al empezar y registro al terminar bien. No cambia nada de lo que
//  hacen. Lee los SP de ESA base (con StockSeguro si esta instalado) y verifica cada punto de cambio.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const MARCA = 'Kaily: auditoria';
// Modulo de MAM_Auditoria y nombres de @Id / @usuario tal como estan declarados en cada SP.
// fin = la ultima linea que devuelve el resultado en cada bloque (el registro va justo antes)
const SPS = {
    MAM_Ventas: { modulo: 1, id: '@Id', usuario: '@usuario', finEliminar: /SELECT\s+1\s+as\s+resp/gi, finModificar: /select\s+@Id\s+as\s+newNumi/gi },
    MAM_Compras: { modulo: 2, id: '@Id', usuario: '@Usuario', finEliminar: /select\s+@Id\s+as\s+newNumi/gi, finModificar: /select\s+@Id\s+as\s+newNumi/gi },
    MAM_Movimientos: { modulo: 3, id: '@id', usuario: '@usuario', finEliminar: /SELECT\s+@id\s+AS\s+newNumi/gi, finModificar: /select\s+@id\s+as\s+newNumi/gi },
};
const OBJ = Object.keys(SPS);

const inicioLinea = (s, idx) => s.lastIndexOf('\n', idx - 1) + 1;
const finLinea = (s, idx) => { const f = s.indexOf('\n', idx); return f < 0 ? s.length : f + 1; };
const sangria = (s, idx) => /^[ \t]*/.exec(s.slice(inicioLinea(s, idx)))[0];

/** Ultima coincidencia de re entre desde y hasta */
function ultima(s, re, desde, hasta, nombre) {
    let m; let r = null;
    re.lastIndex = 0;
    const t = s.slice(desde, hasta);
    while ((m = re.exec(t))) r = desde + m.index;
    if (r === null) falla(`No se encontro ${nombre}: el SP de esta base es distinto, hay que revisarlo a mano`);
    return r;
}

function aplicarCambios(o, s) {
    if (s.includes(MARCA)) falla(`${o} ya tiene "${MARCA}": esta BD ya esta instalada`);
    const c = SPS[o];
    const foto = (I) => `${I}EXEC MAM_Auditoria @tipo=1, @Modulo=${c.modulo}, @DocumentoId=${c.id}, @Foto=@AudAntes OUTPUT /* ${MARCA}: foto antes */\r\n`;
    const registro = (I, op) => `${I}EXEC MAM_Auditoria @tipo=2, @Modulo=${c.modulo}, @Operacion=${op}, @DocumentoId=${c.id}, @Antes=@AudAntes, @usuario=${c.usuario} /* ${MARCA}: ${op === 1 ? 'modificado' : 'eliminado'} */\r\n`;
    // Puntos de cambio (se aplican de atras para adelante para no mover los indices)
    const cambios = [];
    const bloque = (reIni, reFin, nombre) => {
        const ini = buscar(s, reIni, 0, s.length, `el bloque ${nombre} de ${o}`).idx;
        return { ini, fin: buscar(s, reFin, ini + 1, s.length, `el fin del bloque ${nombre} de ${o}`).idx };
    };
    const el = bloque(/IF\s*@tipo\s*=\s*-1(?!\d)/i, /IF\s*@tipo\s*=\s*1(?!\d)/i, '@tipo -1');
    const mo = bloque(/IF\s*@tipo\s*=\s*2(?!\d)/i, /IF\s*@tipo\s*=\s*3(?!\d)/i, '@tipo 2');
    for (const [b, op, reFin] of [[el, 2, c.finEliminar], [mo, 1, c.finModificar]]) {
        const tr = buscar(s, /BEGIN\s+TRY/i, b.ini, b.fin, `BEGIN TRY del bloque de ${o}`);
        const finTry = ultima(s, /END\s+TRY/gi, b.ini, b.fin, `END TRY del bloque de ${o}`);
        const res = ultima(s, reFin, tr.fin, finTry, `el resultado final del bloque de ${o}`);
        cambios.push({ pos: finLinea(s, tr.fin), texto: foto(sangria(s, res)) });
        cambios.push({ pos: inicioLinea(s, res), texto: registro(sangria(s, res), op) });
    }
    // variable de la foto: despues del primer "set @newFecha=GETDATE()"
    const nf = buscar(s, /set\s+@newFecha\s*=\s*GETDATE\(\)/i, 0, s.length, `set @newFecha de ${o}`);
    cambios.push({ pos: finLinea(s, nf.idx), texto: `${sangria(s, nf.idx)}DECLARE @AudAntes xml /* ${MARCA} */\r\n` });
    cambios.sort((a, b) => b.pos - a.pos);
    for (const x of cambios) s = s.slice(0, x.pos) + x.texto + s.slice(x.pos);
    return s;
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos(OBJ);
    const nuevo = Object.fromEntries(OBJ.map((o) => [o, aplicarCambios(o, def[o])]));
    // SET de cada SP (QUOTED_IDENTIFIER / ANSI_NULLS) para recrearlo igual
    const opciones = {};
    for (const o of OBJ) {
        const r = (await MetodoDatos.EjecutarConsulta(`SELECT OBJECTPROPERTY(OBJECT_ID('dbo.${o}'), 'ExecIsQuotedIdentOn') q, OBJECTPROPERTY(OBJECT_ID('dbo.${o}'), 'ExecIsAnsiNullsOn') n`))[0];
        opciones[o] = `SET QUOTED_IDENTIFIER ${r.q ? 'ON' : 'OFF'}${NL}GO${NL}SET ANSI_NULLS ${r.n ? 'ON' : 'OFF'}${NL}GO`;
    }
    const stock = def.MAM_Ventas.includes('Kaily: validacion de stock');
    const hoy = new Date().toISOString().slice(0, 10);
    const marcas = (n) => OBJ.map((o) => `OBJECT_DEFINITION(OBJECT_ID('dbo.${o}')) ${n ? 'NOT ' : ''}LIKE '%${MARCA}%'`).join(n ? ' OR ' : ' AND ');
    const algunaMarca = OBJ.map((o) => `OBJECT_DEFINITION(OBJECT_ID('dbo.${o}')) LIKE '%${MARCA}%'`).join(' OR ');
    const cab = [
        '/* =====================================================================',
        '   08 - AUDITORIA EN VENTAS, COMPRAS Y MOVIMIENTOS (web y escritorio usan los mismos SP)',
        '',
        '   MAM_Ventas, MAM_Compras y MAM_Movimientos, en @tipo 2 (modificar) y @tipo -1 (eliminar):',
        '     - al empezar: EXEC MAM_Auditoria @tipo=1 (foto ANTES: cabecera y detalle)',
        '     - al terminar bien: EXEC MAM_Auditoria @tipo=2 (guarda antes, despues, usuario, fecha, equipo)',
        '   Lo que hace cada SP queda IGUAL (ventas sigue borrando su detalle al eliminar, etc.).',
        '   Si la auditoria falla, no impide grabar: el error queda en la Bitacora.',
        '',
        `   Generado desde ${bd} (${hoy})${stock ? ', CON StockSeguro (05) instalado' : ''}. PROTECCION: solo se aplica si los 3 SP`,
        '   son esa version (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada.',
        '   Si alguno es otra version, da error y no modifica nada. Los 3 SP van en UNA transaccion.',
        `   REQUISITOS: 07_Auditoria.sql ejecutado antes${stock ? ' y 05_StockSeguro.sql instalado' : ''}.`,
        '   VOLVER ATRAS: revertir\\REVERTIR_08_Auditoria_SPs.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${marcas(false)}`,
        'BEGIN',
        "\tRAISERROR('08 Auditoria en SPs YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        "ELSE IF OBJECT_ID('dbo.MAM_Auditoria', 'P') IS NULL",
        'BEGIN',
        "\tRAISERROR('08: falta ejecutar primero 07_Auditoria.sql (no existe MAM_Auditoria). No se modifico nada.', 16, 1)",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ${OBJ.map((o) => `ISNULL(${HUELLA(o)}, 0x) <> ${hash[o]}`).join(`${NL}   OR `)}`,
        'BEGIN',
        `\tRAISERROR('08: algun SP es OTRA version (no la de ${bd} ${hoy}${stock ? ' con StockSeguro' : ''}). ${stock ? 'Si falta, ejecute antes 05_StockSeguro.sql. ' : ''}No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        'SET XACT_ABORT ON',
        'BEGIN TRAN',
        'GO',
    ].join(NL);
    const fin = [
        'IF @@TRANCOUNT > 0',
        'BEGIN',
        `\tIF ${marcas(false)}`,
        "\tBEGIN COMMIT TRAN; RAISERROR('08 Auditoria en SPs INSTALADO.', 10, 1) WITH NOWAIT END",
        '\tELSE',
        "\tBEGIN ROLLBACK TRAN; RAISERROR('08: fallo la instalacion. Se deshizo todo, no se modifico nada.', 16, 1) END",
        'END',
        `ELSE IF ${marcas(true)}`,
        "\tRAISERROR('08: fallo la instalacion (revisar los mensajes de arriba).', 16, 1)",
        'GO',
        'SET NOEXEC OFF',
        'GO',
    ].join(NL);
    const cuerpo = (defs) => OBJ.map((o) => `${opciones[o]}${NL}${aAlter(defs[o])}${NL}GO`).join(NL);
    const r1 = escribir(bd, '08_Auditoria_SPs.sql', `${cab}${NL}${cuerpo(nuevo)}${NL}${fin}${NL}`);

    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 08_Auditoria_SPs.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve MAM_Ventas, MAM_Compras y MAM_Movimientos de ${bd} a la version sin auditoria.`,
        '   La tabla AuditoriaRegistros y su historial NO se tocan. Si no esta instalado, avisa y no hace nada.',
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${algunaMarca})`,
        'BEGIN',
        "\tRAISERROR('08 Auditoria en SPs NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        'SET XACT_ABORT ON',
        'BEGIN TRAN',
        'GO',
        cuerpo(def),
        'IF @@TRANCOUNT > 0',
        'BEGIN',
        `\tIF NOT (${algunaMarca})`,
        "\tBEGIN COMMIT TRAN; RAISERROR('08 Auditoria en SPs DESHECHO.', 10, 1) WITH NOWAIT END",
        '\tELSE',
        "\tBEGIN ROLLBACK TRAN; RAISERROR('No se pudo deshacer 08. No se modifico nada.', 16, 1) END",
        'END',
        'GO',
        'SET NOEXEC OFF', 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_08_Auditoria_SPs.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
