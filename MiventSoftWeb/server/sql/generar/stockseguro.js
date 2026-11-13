// =====================================================================
//  Genera StockSeguro.sql y StockSeguro_REVERTIR.sql para la BD conectada
//    node sql/generar/stockseguro.js                 (BD de server/.env)
//    node sql/generar/stockseguro.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee MAM_Ventas, MAM_Movimientos y los 3 triggers de MovimientosDetalle de ESA base, les aplica
//  los cambios de stock seguro y verifica cada punto de cambio: si el SP de esa base es distinto,
//  se detiene sin generar nada. La BD debe tener la version SIN StockSeguro.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, insertarDespuesDeLinea, escribir } = require('./comun');

const OBJ = ['MAM_Ventas', 'MAM_Movimientos', 'Trigger_MovimientoInsertar', 'Trigger_MovimientoDetalleEliminar', 'Trigger_MovimientosDetalleModificar'];
const MARCAS = "(OBJECT_DEFINITION(OBJECT_ID('dbo.' + v.n)) LIKE '%Kaily: validacion de stock%' OR OBJECT_DEFINITION(OBJECT_ID('dbo.' + v.n)) LIKE '%Kaily: saldo en una sola sentencia%')";
const VALORES = `(VALUES ${OBJ.map((o) => `('${o}')`).join(', ')}) AS v(n)`;

// Bloque de validacion: efecto = SELECT que devuelve (ProductoId, DepositoId, Delta) [Delta < 0 = descuenta]
function validacion(X, efecto, I) {
    return [
        `${I}-- ===== Kaily: validacion de stock (web y escritorio) =====================================`,
        `${I}-- Lo que esta operacion descuenta por producto y deposito. Las filas de stock quedan BLOQUEADAS (UPDLOCK)`,
        `${I}-- hasta el COMMIT: si otro usuario graba el mismo producto a la vez, espera y despues ve el stock real.`,
        `${I}-- Si no alcanza se revierte todo y se avisa que productos y con que stock actual.`,
        `${I}DECLARE @efStock${X} TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))`,
        `${I}INSERT INTO @efStock${X} (ProductoId, DepositoId, Delta)`,
        ...efecto.map((l) => `${I}${l}`),
        `${I}DECLARE @msgStock${X} nvarchar(2000)`,
        `${I}SELECT @msgStock${X} = STUFF((`,
        `${I}    SELECT TOP 10 '; ' + RTRIM(p.NombreProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))`,
        `${I}        + ', necesita ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'`,
        `${I}    FROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStock${X} GROUP BY ProductoId, DepositoId) AS e`,
        `${I}    INNER JOIN Productos AS p ON p.Id = e.ProductoId`,
        `${I}    OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)`,
        `${I}                 WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s`,
        `${I}    WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0`,
        `${I}    ORDER BY p.NombreProducto`,
        `${I}    FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')`,
        `${I}IF @msgStock${X} IS NOT NULL`,
        `${I}BEGIN`,
        `${I}    ROLLBACK TRAN`,
        `${I}    SET @msgStock${X} = 'STOCK INSUFICIENTE: ' + @msgStock${X}`,
        `${I}    RAISERROR('%s', 16, 1, @msgStock${X})`,
        `${I}    RETURN`,
        `${I}END`,
        `${I}-- ==========================================================================================`,
    ].join(NL) + NL;
}
const DEP_VENTA = '(SELECT a.DepositoId FROM Almacenes AS a WHERE a.Id = @SucursalId)';
const FACTOR = '(SELECT c.Factor FROM MovimientosTipos AS c WHERE c.Id = @ConceptoId)';

function ventas(s) {
    const t2 = buscar(s, /IF\s+@tipo\s*=\s*2\s*--MODIFICACION/i, 0, s.length, 'MAM_Ventas @tipo 2').idx;
    const t3 = buscar(s, /IF\s+@tipo\s*=\s*3\s*--MOSTrar Ventas/i, t2, s.length, 'MAM_Ventas @tipo 3').idx;
    // @tipo 2 (de abajo hacia arriba para no mover indices): transaccion + validacion, ROLLBACK en el CATCH, COMMIT al final
    const cat = buscar(s, /BEGIN CATCH[^\n]*\n/i, t2, t3, 'CATCH de MAM_Ventas @2');
    const fcat = buscar(s, /END CATCH[^\n]*\n/i, cat.fin, t3, 'END CATCH de MAM_Ventas @2');
    s = s.slice(0, fcat.fin) + `\t\t-- Kaily: la modificacion ahora es atomica (antes no tenia transaccion: si fallaba se perdia el detalle)${NL}\t\tIF @@TRANCOUNT > 0 COMMIT TRAN${NL}` + s.slice(fcat.fin);
    s = s.slice(0, cat.fin) + `\t\t\tIF @@TRANCOUNT > 0 ROLLBACK TRAN -- Kaily: deshace todo (la bitacora se graba despues y queda)${NL}` + s.slice(cat.fin);
    const ini2 = buscar(s, /IF\s+@tipo\s*=\s*2\s*--MODIFICACION\s*\r?\n\s*BEGIN\s*\r?\n\s*SET ARITHABORT ON[^\n]*\n/i, 0, s.length, 'inicio de MAM_Ventas @2');
    s = s.slice(0, ini2.fin) + `\t\tBEGIN TRAN MODIFICACION${NL}` + validacion('V2', [
        `SELECT td.ProductoId, ${DEP_VENTA}, -td.Cantidad`,
        'FROM @VentaDetalleType AS td WHERE td.estado >= 0 AND td.ProductoId > 0',
        'UNION ALL -- lo que la venta ya tenia vuelve a su deposito',
        'SELECT d.ProductoId, a.DepositoId, d.Cantidad',
        'FROM VentasDetalles AS d INNER JOIN Ventas AS v ON v.Id = d.VentaId INNER JOIN Almacenes AS a ON a.Id = v.SucursalId',
        'WHERE d.VentaId = @Id AND v.Estado <> -1',
    ], '\t\t') + s.slice(ini2.fin);
    // @tipo 1
    const t1 = buscar(s, /IF\s+@tipo\s*=\s*1\s*--NUEVO REGISTRO/i, 0, s.length, 'MAM_Ventas @tipo 1').idx;
    const t2b = buscar(s, /IF\s+@tipo\s*=\s*2\s*--MODIFICACION/i).idx;
    const bt = buscar(s, /BEGIN TRAN INSERTAR/i, t1, t2b, 'BEGIN TRAN INSERTAR de MAM_Ventas @1');
    return insertarDespuesDeLinea(s, bt.idx, validacion('V1', [
        `SELECT td.ProductoId, ${DEP_VENTA}, -SUM(td.Cantidad)`,
        'FROM @VentaDetalleType AS td WHERE td.estado >= 0 AND td.ProductoId > 0 GROUP BY td.ProductoId',
    ], '\t\t'));
}

function movimientos(s) {
    // El CATCH de @-1 hacia ROLLBACK de un nombre mal escrito; si en esta base ya esta bien, no se toca
    const typo = s.split('ROLLBACK TRAN ELMINAR').length - 1;
    if (typo > 1) falla('ROLLBACK TRAN ELMINAR aparece mas de una vez');
    if (typo === 1) s = s.replace('ROLLBACK TRAN ELMINAR', 'ROLLBACK TRAN ELIMINAR /* Kaily: decia ELMINAR y el ROLLBACK fallaba */');
    const t2 = buscar(s, /IF\s+@tipo\s*=\s*2\s*--MODIFICACION/i, 0, s.length, 'MAM_Movimientos @tipo 2').idx;
    const t3 = buscar(s, /IF\s+@tipo\s*=\s*3\s*--MOSTRAR TODOS/i, t2, s.length, 'MAM_Movimientos @tipo 3').idx;
    const b2 = buscar(s, /BEGIN TRAN MODIFICACION/i, t2, t3, 'BEGIN TRAN MODIFICACION de MAM_Movimientos');
    s = insertarDespuesDeLinea(s, b2.idx, validacion('M2', [
        `SELECT td.ProductoId, @DepositoId, td.Cantidad * ${FACTOR}`,
        'FROM @detalle AS td WHERE td.ProductoId > 0 AND td.estado >= 0',
        'UNION ALL -- lo que el movimiento tenia se revierte en su deposito',
        'SELECT d.ProductoId, m.DepositoId, -d.Cantidad * c.Factor',
        'FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId',
        'WHERE d.MovimientoId = @id',
    ], '    '));
    const t1 = buscar(s, /IF\s+@tipo\s*=\s*1\s*--NUEVO REGISTRO/i, 0, s.length, 'MAM_Movimientos @tipo 1').idx;
    const t2b = buscar(s, /IF\s+@tipo\s*=\s*2\s*--MODIFICACION/i).idx;
    const b1 = buscar(s, /BEGIN TRAN INSERTAR/i, t1, t2b, 'BEGIN TRAN INSERTAR de MAM_Movimientos');
    s = insertarDespuesDeLinea(s, b1.idx, validacion('M1', [
        `SELECT td.ProductoId, @DepositoId, SUM(td.Cantidad) * ${FACTOR}`,
        'FROM @detalle AS td WHERE td.ProductoId > 0 AND td.estado >= 0 GROUP BY td.ProductoId',
    ], '    '));
    const tm = buscar(s, /IF\s+@tipo\s*=\s*-1\s*--ELIMINAR REGISTRO/i, 0, s.length, 'MAM_Movimientos @tipo -1').idx;
    const t1c = buscar(s, /IF\s+@tipo\s*=\s*1\s*--NUEVO REGISTRO/i).idx;
    const bm = buscar(s, /BEGIN TRAN ELIMINAR/i, tm, t1c, 'BEGIN TRAN ELIMINAR de MAM_Movimientos');
    return insertarDespuesDeLinea(s, bm.idx, validacion('MB', [
        'SELECT d.ProductoId, m.DepositoId, -d.Cantidad * c.Factor -- eliminar un ingreso descuenta stock',
        'FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId',
        'WHERE d.MovimientoId = @id OR (@ConceptoId = 8 AND d.MovimientoId IN (SELECT a.Id FROM Movimientos AS a WHERE a.IdMovimientoDestino = @id))',
    ], '    '));
}

// Triggers: el saldo se actualiza en UNA sentencia (antes: leer y luego pisar -> dos grabaciones a la vez perdian una)
function trigger(nombre, s, esperadas) {
    let n = 0;
    s = s.replace(/set @cantA\s*=\s*\(select Cantidad\s+from ProductosStock[^)]*\)\s*set @cantF\s*=\s*(@cantA\s*\+\s*\(@cantI\*@mov\)|@cantI\s*\+\s*@cantA)(\s*--Actualizar Saldo\s*)update ProductosStock\s+set Cantidad\s*=\s*@cantF/gi,
        (m, expr, medio) => {
            n += 1;
            const delta = expr.startsWith('@cantA') ? '(@cantI*@mov)' : '@cantI';
            return `--Kaily: saldo en una sola sentencia (antes se leia y luego se pisaba: dos grabaciones a la vez perdian una)${medio}update ProductosStock set @cantF = Cantidad = Cantidad + ${delta}`;
        });
    if (n !== esperadas) falla(`${nombre}: se encontraron ${n} actualizaciones de saldo (se esperaban ${esperadas}); el trigger de esta base es distinto`);
    return s;
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos(OBJ);
    for (const o of OBJ) if (/Kaily: validacion de stock|Kaily: saldo en una sola sentencia/.test(def[o])) falla(`${o} ya tiene StockSeguro en ${bd}: no hace falta generar nada`);
    const nuevo = {
        MAM_Ventas: ventas(def.MAM_Ventas), MAM_Movimientos: movimientos(def.MAM_Movimientos),
        Trigger_MovimientoInsertar: trigger('Trigger_MovimientoInsertar', def.Trigger_MovimientoInsertar, 1),
        Trigger_MovimientoDetalleEliminar: trigger('Trigger_MovimientoDetalleEliminar', def.Trigger_MovimientoDetalleEliminar, 1),
        Trigger_MovimientosDetalleModificar: trigger('Trigger_MovimientosDetalleModificar', def.Trigger_MovimientosDetalleModificar, 2),
    };
    const hoy = new Date().toISOString().slice(0, 10);
    const cab = [
        '/* =====================================================================',
        '   STOCK SEGURO (web y escritorio usan los mismos SP y triggers)',
        '',
        '   1. MAM_Ventas @1 / @2 y MAM_Movimientos @1 / @2 / @-1: antes de grabar calculan lo que la',
        '      operacion descuenta, BLOQUEAN esas filas de stock (UPDLOCK) y si no alcanza revierten y',
        '      avisan: "STOCK INSUFICIENTE: Producto (stock actual X, necesita Y); ..."',
        '      -> si dos maquinas venden el mismo producto a la vez, la segunda espera y ve el stock real.',
        '      Compras no se valida (es ingreso). Eliminar un ingreso o traspaso que ya se vendio SI se valida.',
        '   2. MAM_Ventas @2 (modificar venta) no tenia transaccion: si fallaba a medias se perdia el detalle.',
        '      Ahora es atomica (todo o nada).',
        '   3. MAM_Movimientos @-1: el CATCH hacia "ROLLBACK TRAN ELMINAR" (nombre mal escrito) y fallaba.',
        '   4. Triggers de MovimientosDetalle: el saldo se actualiza en UNA sentencia',
        '      (antes leian el saldo y despues lo pisaban: dos grabaciones a la vez perdian una).',
        '   Todo lo demas de cada objeto queda IDENTICO a la version de la BD.',
        '',
        `   Generado desde ${bd} (${hoy}). PROTECCION: solo se aplica si el codigo de los 5 objetos es esa`,
        '   version (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si alguno es',
        '   otra version (otra BD / otra rama), da error y no modifica nada.',
        '   VOLVER ATRAS: revertir\\REVERTIR_05_StockSeguro.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF EXISTS (SELECT 1 FROM ${VALORES} WHERE ${MARCAS})`,
        'BEGIN',
        "\tRAISERROR('StockSeguro YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ${OBJ.map((o) => `ISNULL(${HUELLA(o)}, 0x) <> ${hash[o]}`).join(`${NL}   OR `)}`,
        'BEGIN',
        `\tRAISERROR('StockSeguro: algun SP o trigger es OTRA version (no la de ${bd} ${hoy}). No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        '-- Necesario para que el bloqueo sea solo de los productos de la operacion (busqueda por indice)',
        "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductosStock_Producto_Deposito' AND object_id = OBJECT_ID('dbo.ProductosStock'))",
        '\tCREATE NONCLUSTERED INDEX IX_ProductosStock_Producto_Deposito ON dbo.ProductosStock (ProductoId, DepositoId, Lote, FechaVencimiento) INCLUDE (Cantidad)',
        'GO',
    ].join(NL);
    const r1 = escribir(bd, '05_StockSeguro.sql', `${cab}${NL}${OBJ.map((o) => `${aAlter(nuevo[o])}${NL}GO`).join(NL)}${NL}SET NOEXEC OFF${NL}GO${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 05_StockSeguro.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve MAM_Ventas, MAM_Movimientos y los 3 triggers de MovimientosDetalle de ${bd} a la version`,
        '   anterior (sin validacion de stock). Si StockSeguro no esta instalado, avisa y no hace nada.',
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT EXISTS (SELECT 1 FROM ${VALORES} WHERE ${MARCAS})`,
        'BEGIN',
        "\tRAISERROR('StockSeguro NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        ...OBJ.map((o) => `${aAlter(def[o])}${NL}GO`),
        'SET NOEXEC OFF', 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_05_StockSeguro.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
