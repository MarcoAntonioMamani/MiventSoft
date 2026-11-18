// =====================================================================
//  Genera 05_CompraStock.sql y revertir/REVERTIR_05_CompraStock.sql para la BD conectada (Distral)
//    node sql/generar/compra_stock.js               (BD de server/.env)
//    node sql/generar/compra_stock.js --bd=OtraBD   (otra BD del mismo servidor)
//  MAM_Compras @tipo -1 (eliminar) y @tipo 2 (modificar): antes de grabar valida que el stock no quede NEGATIVO
//  (lo que la compra ingreso ya se vendio o salio). No depende del 04.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, insertarDespuesDeLinea, escribir } = require('./comun');

const SP = 'MAM_Compras';
const MARCA = 'Sanbit: stock de la compra';

const inicioLinea = (s, idx) => s.lastIndexOf('\n', idx - 1) + 1;
const finLinea = (s, idx) => { const f = s.indexOf('\n', idx); return f < 0 ? s.length : f + 1; };
const sangria = (s, idx) => /^[ \t]*/.exec(s.slice(inicioLinea(s, idx)))[0];

// Mensaje: "...: Producto (stock actual X, la compra ingreso Y); ..." (la web lo muestra en una tabla; el escritorio tal cual)
const mensaje = (X, I, efecto, prefijo, verbo) => [
    `DECLARE @efStock${X} TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))`,
    `INSERT INTO @efStock${X} (ProductoId, DepositoId, Delta)`,
    ...efecto,
    `DECLARE @msgStock${X} nvarchar(2000)`,
    `SELECT @msgStock${X} = STUFF((`,
    "\tSELECT TOP 10 '; ' + RTRIM(p.DescripcionProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))",
    `\t\t+ ', ${verbo} ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'`,
    `\tFROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStock${X} GROUP BY ProductoId, DepositoId) AS e`,
    '\tINNER JOIN Productos AS p ON p.Id = e.ProductoId',
    '\tOUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)',
    '\t             WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s',
    '\tWHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0',
    '\tORDER BY p.DescripcionProducto',
    "\tFOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')",
    `SET @msgStock${X} = '${prefijo}: ' + @msgStock${X}`,
].map((l) => `${I}${l}\r\n`).join('');

// lo que la compra ingreso (su movimiento concepto 2) sale del stock
const QUITA_COMPRA = [
    'SELECT d.ProductoId, m.DepositoId, -d.Cantidad * c.Factor',
    'FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId',
    'WHERE m.ConceptoId = 2 AND m.TransaccionId = @Id',
];

function eliminar(s) {
    const ini = buscar(s, /IF\s*@tipo\s*=\s*-1(?!\d)/i, 0, s.length, `el bloque @tipo -1 de ${SP}`).idx;
    const finBloque = buscar(s, /IF\s*@tipo\s*=\s*1(?!\d)/i, ini + 1, s.length, `el fin del bloque @tipo -1 de ${SP}`).idx;
    const tr = buscar(s, /BEGIN\s+TRY/i, ini, finBloque, `BEGIN TRY del bloque @tipo -1 de ${SP}`);
    const ca = buscar(s, /END\s+TRY\s*BEGIN\s+CATCH/i, tr.fin, finBloque, `END TRY / BEGIN CATCH del bloque @tipo -1 de ${SP}`);
    const res = buscar(s, /select\s+@Id\s+as\s+newNumi/i, tr.fin, ca.idx, `el resultado (select @Id as newNumi) del bloque @tipo -1 de ${SP}`);
    if (/BEGIN\s+TRAN/i.test(s.slice(ini, finBloque))) falla(`${SP} @-1 ya tiene transaccion en esta base: revisar a mano`);
    const I = sangria(s, tr.idx);
    const J = sangria(s, res.idx);
    const validar = [
        `${I}-- ===== ${MARCA} (web y escritorio) =====================================\r\n`,
        `${I}-- Eliminar la compra quita del stock lo que ingreso. Si eso ya se vendio o salio y el stock quedaria NEGATIVO,\r\n`,
        `${I}-- no se elimina nada y se avisa que productos y con que stock actual. Las filas de stock quedan BLOQUEADAS\r\n`,
        `${I}-- (UPDLOCK) hasta terminar. Eliminar ahora es todo o nada (antes se podia borrar a medias).\r\n`,
        `${I}DECLARE @tcCompraE int = @@TRANCOUNT\r\n`,
        `${I}IF @tcCompraE = 0 BEGIN TRAN ELSE SAVE TRAN CompraEliminar\r\n`,
        mensaje('CE', I, QUITA_COMPRA, 'NO SE PUEDE ELIMINAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO', 'la compra ingreso'),
        `${I}IF @msgStockCE IS NOT NULL\r\n${I}BEGIN\r\n`,
        `${I}\tIF @tcCompraE = 0 ROLLBACK TRAN ELSE ROLLBACK TRAN CompraEliminar\r\n`,
        `${I}\tRAISERROR('%s', 16, 1, @msgStockCE)\r\n${I}\tRETURN\r\n${I}END\r\n`,
        `${I}-- ==========================================================================================\r\n`,
    ].join('');
    const cambios = [
        { pos: inicioLinea(s, tr.idx), texto: validar },
        { pos: finLinea(s, res.idx), texto: `${J}IF @tcCompraE = 0 AND @@TRANCOUNT > 0 COMMIT TRAN /* ${MARCA} */\r\n` },
        // en el CATCH: se deshace todo antes de grabar la Bitacora (antes quedaba borrado a medias)
        { pos: finLinea(s, ca.fin), texto: `${J}IF @tcCompraE = 0 AND @@TRANCOUNT > 0 ROLLBACK TRAN ELSE IF @tcCompraE > 0 AND XACT_STATE() = 1 ROLLBACK TRAN CompraEliminar /* ${MARCA} */\r\n` },
    ].sort((a, b) => b.pos - a.pos);
    for (const x of cambios) s = s.slice(0, x.pos) + x.texto + s.slice(x.pos);
    return s;
}

function modificar(s) {
    const t2 = buscar(s, /IF\s*@tipo\s*=\s*2\s*--\s*MODIFICACION/i, 0, s.length, `el bloque @tipo 2 de ${SP}`).idx;
    const t3 = buscar(s, /IF\s*@tipo\s*=\s*3(?!\d)/i, t2, s.length, `el fin del bloque @tipo 2 de ${SP}`).idx;
    const bt = buscar(s, /BEGIN TRAN MODIFICACION/i, t2, t3, `BEGIN TRAN MODIFICACION de ${SP} @2`);
    const tr = buscar(s, /BEGIN\s+TRY/i, bt.fin, t3, `BEGIN TRY de ${SP} @2`);
    if (tr.idx - bt.fin > 20) falla(`${SP} @2: el BEGIN TRY no esta justo despues del BEGIN TRAN; revisar a mano`);
    const I = sangria(s, bt.idx);
    const texto = [
        `${I}-- ===== ${MARCA} (web y escritorio) =====================================\r\n`,
        `${I}-- Modificar la compra cambia el stock: lo que ya no ingresa (cantidad menor o producto quitado) sale del stock.\r\n`,
        `${I}-- Si eso ya se vendio o salio y el stock quedaria NEGATIVO, no se modifica nada y se avisa.\r\n`,
        mensaje('CM', I, [
            `SELECT td.ProductoId, (SELECT a.DepositoId FROM Almacenes AS a WHERE a.Id = @AlmacenId), td.Cantidad`,
            'FROM @detalle AS td WHERE td.estado >= 0 AND td.ProductoId > 0 AND td.Cantidad > 0',
            'UNION ALL',
            ...QUITA_COMPRA,
        ], 'NO SE PUEDE MODIFICAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO', 'se quitaria'),
        `${I}IF @msgStockCM IS NOT NULL\r\n${I}BEGIN\r\n${I}\tROLLBACK TRAN\r\n${I}\tRAISERROR('%s', 16, 1, @msgStockCM)\r\n${I}\tRETURN\r\n${I}END\r\n`,
        `${I}-- ==========================================================================================\r\n`,
    ].join('');
    return insertarDespuesDeLinea(s, bt.idx, texto);
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    if (def[SP].includes(MARCA)) falla(`${SP} ya tiene "${MARCA}": esta BD ya esta instalada`);
    const nuevo = modificar(eliminar(def[SP]));
    const hoy = new Date().toISOString().slice(0, 10);
    const instalado = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const cab = [
        '/* =====================================================================',
        '   05 - NO ELIMINAR NI MODIFICAR UNA COMPRA SI EL STOCK QUEDA NEGATIVO (web y escritorio usan el mismo SP)',
        '',
        `   ${SP} @tipo -1 (eliminar) y @tipo 2 (modificar): antes de grabar, revisa lo que la compra deja de`,
        '   ingresar. Si eso ya se vendio o salio y el stock quedaria NEGATIVO, no graba nada y avisa:',
        '     "NO SE PUEDE ELIMINAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: Producto (stock actual X, la compra ingreso Y); ..."',
        '     "NO SE PUEDE MODIFICAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: Producto (stock actual X, se quitaria Y); ..."',
        '   Quedar en 0 SI se permite. Eliminar la compra ahora es todo o nada (antes no tenia transaccion).',
        '   Eliminar un ingreso de inventario (Movimientos) lo valida el 04. Todo lo demas del SP queda IDENTICO.',
        '',
        `   Generado desde ${bd} (${hoy}). PROTECCION: solo se aplica si ${SP} es esa version (sin contar`,
        '   mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da error y no modifica nada.',
        '   VOLVER ATRAS: revertir\\REVERTIR_05_CompraStock.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${instalado}`,
        'BEGIN',
        "\tRAISERROR('05 Compra stock YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('05: ${SP} es OTRA version (no la de ${bd} ${hoy}). No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = ['SET NOEXEC OFF', 'GO', `IF ${instalado}`, "\tRAISERROR('05 Compra stock INSTALADO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r1 = escribir(bd, '05_CompraStock.sql', `${cab}${NL}${aAlter(nuevo)}${NL}GO${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        `   SOLO PARA DESHACER 05_CompraStock.sql. NO es parte de la instalacion. Vuelve ${SP} de ${bd} a la version anterior.`,
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT ${instalado}`,
        'BEGIN',
        "\tRAISERROR('05 Compra stock NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        `${aAlter(def[SP])}${NL}GO`,
        'SET NOEXEC OFF', 'GO', "RAISERROR('05 Compra stock DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_05_CompraStock.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
