// =====================================================================
//  Genera 09_CompraEliminarStock.sql y revertir/REVERTIR_09_CompraEliminarStock.sql para la BD conectada
//    node sql/generar/compra_eliminar_stock.js               (BD de server/.env)
//    node sql/generar/compra_eliminar_stock.js --bd=OtraBD   (otra BD del mismo servidor)
//  MAM_Compras @tipo -1 (eliminar compra): antes de borrar valida que el stock no quede NEGATIVO
//  (lo que la compra ingreso ya se vendio o salio). Requiere el 08 instalado en esa BD (se genera sobre ese SP).
//  Eliminar un ingreso de inventario ya lo valida el 05 (MAM_Movimientos).
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'MAM_Compras';
const MARCA = 'Kaily: stock al eliminar compra';
const MARCA_08 = 'Kaily: auditoria';

const inicioLinea = (s, idx) => s.lastIndexOf('\n', idx - 1) + 1;
const finLinea = (s, idx) => { const f = s.indexOf('\n', idx); return f < 0 ? s.length : f + 1; };
const sangria = (s, idx) => /^[ \t]*/.exec(s.slice(inicioLinea(s, idx)))[0];

/** Validacion antes del BEGIN TRY. Transaccion propia (o SAVE TRAN si el que llama ya tiene una): todo o nada,
 *  y las filas de stock BLOQUEADAS (UPDLOCK) hasta terminar. Mensaje: "...: Producto (stock actual X, la compra ingreso Y); ..."
 *  (la web lo interpreta para mostrar la tabla; el escritorio lo muestra tal cual) */
const validacion = (I) => [
    `-- ===== ${MARCA} (web y escritorio) =====================================`,
    '-- Eliminar la compra quita del stock lo que ingreso (su movimiento concepto 2). Si eso ya se vendio o salio y el',
    '-- stock quedaria NEGATIVO, no se elimina nada y se avisa que productos y con que stock actual.',
    '-- Las filas de stock quedan BLOQUEADAS (UPDLOCK) hasta terminar: si otro vende el mismo producto, espera.',
    'DECLARE @tcCompraE int = @@TRANCOUNT',
    'IF @tcCompraE = 0 BEGIN TRAN ELSE SAVE TRAN CompraEliminar',
    'DECLARE @msgStockCE nvarchar(2000)',
    'SELECT @msgStockCE = STUFF((',
    "\tSELECT TOP 10 '; ' + RTRIM(p.NombreProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))",
    "\t\t+ ', la compra ingreso ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'",
    '\tFROM (SELECT d.ProductoId, m.DepositoId, SUM(-d.Cantidad * c.Factor) AS Delta',
    '\t      FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId',
    '\t      WHERE m.ConceptoId = 2 AND m.TransaccionId = @Id',
    '\t      GROUP BY d.ProductoId, m.DepositoId) AS e',
    '\tINNER JOIN Productos AS p ON p.Id = e.ProductoId',
    '\tOUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)',
    '\t             WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s',
    '\tWHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0',
    '\tORDER BY p.NombreProducto',
    "\tFOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')",
    'IF @msgStockCE IS NOT NULL',
    'BEGIN',
    '\tIF @tcCompraE = 0 ROLLBACK TRAN ELSE ROLLBACK TRAN CompraEliminar',
    "\tSET @msgStockCE = 'NO SE PUEDE ELIMINAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: ' + @msgStockCE",
    "\tRAISERROR('%s', 16, 1, @msgStockCE)",
    '\tRETURN',
    'END',
    '-- ==========================================================================================',
].map((l) => `${I}${l}\r\n`).join('');

function aplicarCambios(s) {
    if (s.includes(MARCA)) falla(`${SP} ya tiene "${MARCA}": esta BD ya esta instalada`);
    if (!s.includes(MARCA_08)) falla(`${SP} no tiene el 08 (auditoria): primero ejecute 08_Auditoria_SPs.sql en esta BD y despues genere el 09`);
    const ini = buscar(s, /IF\s*@tipo\s*=\s*-1(?!\d)/i, 0, s.length, `el bloque @tipo -1 de ${SP}`).idx;
    const finBloque = buscar(s, /IF\s*@tipo\s*=\s*1(?!\d)/i, ini + 1, s.length, `el fin del bloque @tipo -1 de ${SP}`).idx;
    const tr = buscar(s, /BEGIN\s+TRY/i, ini, finBloque, `BEGIN TRY del bloque @tipo -1 de ${SP}`);
    const ca = buscar(s, /END\s+TRY\s*BEGIN\s+CATCH/i, tr.fin, finBloque, `END TRY / BEGIN CATCH del bloque @tipo -1 de ${SP}`);
    const res = buscar(s, /select\s+@Id\s+as\s+newNumi/i, tr.fin, ca.idx, `el resultado (select @Id as newNumi) del bloque @tipo -1 de ${SP}`);
    const I = sangria(s, res.idx);
    const cambios = [
        { pos: inicioLinea(s, tr.idx), texto: validacion(sangria(s, tr.idx)) },
        { pos: finLinea(s, res.idx), texto: `${I}IF @tcCompraE = 0 AND @@TRANCOUNT > 0 COMMIT TRAN /* ${MARCA} */\r\n` },
        // en el CATCH: se deshace todo antes de grabar la Bitacora (antes quedaba borrado a medias)
        { pos: finLinea(s, ca.fin), texto: `${I}IF @tcCompraE = 0 AND @@TRANCOUNT > 0 ROLLBACK TRAN ELSE IF @tcCompraE > 0 AND XACT_STATE() = 1 ROLLBACK TRAN CompraEliminar /* ${MARCA} */\r\n` },
    ].sort((a, b) => b.pos - a.pos);
    for (const x of cambios) s = s.slice(0, x.pos) + x.texto + s.slice(x.pos);
    return s;
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    const nuevo = aplicarCambios(def[SP]);
    const r = (await MetodoDatos.EjecutarConsulta(`SELECT OBJECTPROPERTY(OBJECT_ID('dbo.${SP}'), 'ExecIsQuotedIdentOn') q, OBJECTPROPERTY(OBJECT_ID('dbo.${SP}'), 'ExecIsAnsiNullsOn') n`))[0];
    const opciones = `SET QUOTED_IDENTIFIER ${r.q ? 'ON' : 'OFF'}${NL}GO${NL}SET ANSI_NULLS ${r.n ? 'ON' : 'OFF'}${NL}GO`;
    const hoy = new Date().toISOString().slice(0, 10);
    const instalado = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const cab = [
        '/* =====================================================================',
        '   09 - NO ELIMINAR UNA COMPRA SI EL STOCK QUEDA NEGATIVO (web y escritorio usan el mismo SP)',
        '',
        `   ${SP} @tipo -1: antes de borrar, revisa lo que la compra ingreso. Si eso ya se vendio o salio y el`,
        '   stock quedaria NEGATIVO, no elimina nada y avisa:',
        '     "NO SE PUEDE ELIMINAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: Producto (stock actual X, la compra ingreso Y); ..."',
        '   Quedar en 0 SI se permite. Eliminar la compra ahora es todo o nada (antes, si fallaba a medias,',
        '   quedaba borrada la caja o el credito pero no el detalle). Eliminar un ingreso de inventario ya lo valida el 05.',
        `   Todo lo demas del SP queda IDENTICO a la version con el 08.`,
        '',
        `   Generado desde ${bd} (${hoy}). PROTECCION: solo se aplica si ${SP} es esa version (con el 08, sin contar`,
        '   mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da error y no modifica nada.',
        '   REQUISITO: 08_Auditoria_SPs.sql ejecutado antes.',
        '   VOLVER ATRAS: revertir\\REVERTIR_09_CompraEliminarStock.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${instalado}`,
        'BEGIN',
        "\tRAISERROR('09 Compra eliminar stock YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) NOT LIKE '%${MARCA_08}%'`,
        'BEGIN',
        "\tRAISERROR('09: falta ejecutar primero 08_Auditoria_SPs.sql. No se modifico nada.', 16, 1)",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('09: ${SP} es OTRA version (no la de ${bd} ${hoy} con el 08). No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
    ].join(NL);
    const fin = [
        `IF ${instalado}`,
        "\tRAISERROR('09 Compra eliminar stock INSTALADO.', 10, 1) WITH NOWAIT",
        'ELSE',
        "\tRAISERROR('09: fallo la instalacion (revisar los mensajes de arriba).', 16, 1)",
        'GO',
        'SET NOEXEC OFF',
        'GO',
    ].join(NL);
    const r1 = escribir(bd, '09_CompraEliminarStock.sql', `${cab}${NL}${opciones}${NL}${aAlter(nuevo)}${NL}GO${NL}${fin}${NL}`);

    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 09_CompraEliminarStock.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version del 08 (con auditoria, sin la validacion de stock al eliminar).`,
        '   Si no esta instalado, avisa y no hace nada. Para deshacer tambien el 08, despues ejecute REVERTIR_08.',
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${instalado})`,
        'BEGIN',
        "\tRAISERROR('09 Compra eliminar stock NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        opciones,
        aAlter(def[SP]),
        'GO',
        `IF NOT (${instalado})`,
        "\tRAISERROR('09 Compra eliminar stock DESHECHO.', 10, 1) WITH NOWAIT",
        'ELSE',
        "\tRAISERROR('No se pudo deshacer 09.', 16, 1)",
        'GO',
        'SET NOEXEC OFF', 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_09_CompraEliminarStock.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
