// =====================================================================
//  Genera 04_StockSeguro.sql y revertir/REVERTIR_04_StockSeguro.sql para la BD conectada (Distral / distribucion)
//    node sql/generar/stockseguro.js                 (BD de server/.env)
//    node sql/generar/stockseguro.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee de ESA base MAM_Ventas, MAM_Movimientos, sp_go_TC004_appMovil (pedidos del celular) y los 3 triggers
//  de MovimientosDetalle, les aplica la validacion de stock y verifica cada punto de cambio: si el codigo de
//  esa base es distinto, se detiene sin generar nada. La BD debe tener la version SIN StockSeguro.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, insertarDespuesDeLinea, escribir } = require('./comun');

const OBJ = ['MAM_Ventas', 'MAM_Movimientos', 'sp_go_TC004_appMovil', 'Trigger_MovimientoInsertar', 'Trigger_MovimientoDetalleEliminar', 'Trigger_MovimientosDetalleModificar'];
const MARCA = 'Sanbit: validacion de stock';
const MARCA_TR = 'Sanbit: saldo en una sola sentencia';
const MARCAS = `(OBJECT_DEFINITION(OBJECT_ID('dbo.' + v.n)) LIKE '%${MARCA}%' OR OBJECT_DEFINITION(OBJECT_ID('dbo.' + v.n)) LIKE '%${MARCA_TR}%')`;
const VALORES = `(VALUES ${OBJ.map((o) => `('${o}')`).join(', ')}) AS v(n)`;

/**
 * Bloque de validacion. efecto = SELECT que devuelve (ProductoId, DepositoId, Delta) [Delta < 0 = descuenta].
 * modo 'propia': va FUERA del TRY, justo despues del BEGIN TRAN: si no alcanza, ROLLBACK + error + RETURN.
 * modo 'catch' : va DENTRO del TRY: solo lanza el error; el CATCH del SP hace el ROLLBACK (y lo reenvia).
 * cond: condicion opcional (ej. el pedido del celular aun no fue sincronizado).
 */
function validacion(X, efecto, I, modo, cond = null) {
    const J = cond ? `${I}\t` : I;
    const cuerpo = [
        `${J}DECLARE @efStock${X} TABLE (ProductoId int, DepositoId int, Delta decimal(18,2))`,
        `${J}INSERT INTO @efStock${X} (ProductoId, DepositoId, Delta)`,
        ...efecto.map((l) => `${J}${l}`),
        `${J}DECLARE @msgStock${X} nvarchar(2000)`,
        `${J}SELECT @msgStock${X} = STUFF((`,
        `${J}    SELECT TOP 10 '; ' + RTRIM(p.DescripcionProducto) + ' (stock actual ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), ISNULL(s.Cantidad, 0)))`,
        `${J}        + ', necesita ' + CONVERT(nvarchar(20), CONVERT(decimal(18,2), -e.Delta)) + ')'`,
        `${J}    FROM (SELECT ProductoId, DepositoId, SUM(Delta) AS Delta FROM @efStock${X} GROUP BY ProductoId, DepositoId) AS e`,
        `${J}    INNER JOIN Productos AS p ON p.Id = e.ProductoId`,
        `${J}    OUTER APPLY (SELECT SUM(x.Cantidad) AS Cantidad FROM ProductosStock AS x WITH (UPDLOCK, ROWLOCK)`,
        `${J}                 WHERE x.ProductoId = e.ProductoId AND x.DepositoId = e.DepositoId) AS s`,
        `${J}    WHERE e.Delta < 0 AND ISNULL(s.Cantidad, 0) + e.Delta < 0`,
        `${J}    ORDER BY p.DescripcionProducto`,
        `${J}    FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, '')`,
        `${J}IF @msgStock${X} IS NOT NULL`,
        `${J}BEGIN`,
        ...(modo === 'propia' ? [`${J}    ROLLBACK TRAN`] : []),
        `${J}    SET @msgStock${X} = 'STOCK INSUFICIENTE: ' + @msgStock${X}`,
        `${J}    RAISERROR('%s', 16, 1, @msgStock${X})`,
        ...(modo === 'propia' ? [`${J}    RETURN`] : []),
        `${J}END`,
    ];
    return [
        `${I}-- ===== ${MARCA} (web, escritorio y celular) =====================================`,
        `${I}-- Lo que esta operacion descuenta por producto y deposito. Las filas de stock quedan BLOQUEADAS (UPDLOCK)`,
        `${I}-- hasta el COMMIT: si otro usuario graba el mismo producto a la vez, espera y despues ve el stock real.`,
        `${I}-- Si no alcanza no se graba nada y se avisa que productos y con que stock actual.`,
        ...(cond ? [`${I}IF ${cond}`, `${I}BEGIN`] : []),
        ...cuerpo,
        ...(cond ? [`${I}END`] : []),
        `${I}-- ==========================================================================================`,
    ].join(NL) + NL;
}
const DEP_SUC = (suc) => `(SELECT a.DepositoId FROM Almacenes AS a WHERE a.Id = ${suc})`;
const FACTOR = '(SELECT c.Factor FROM MovimientosTipos AS c WHERE c.Id = @ConceptoId)';
// lo que el pedido ya tenia vuelve a su deposito (si el pedido no estaba eliminado)
const DEVUELVE = (idVenta) => [
    'UNION ALL -- lo que el pedido ya tenia vuelve a su deposito',
    'SELECT d.ProductoId, a.DepositoId, d.Cantidad',
    'FROM VentasDetalles AS d INNER JOIN Ventas AS v ON v.Id = d.VentaId INNER JOIN Almacenes AS a ON a.Id = v.SucursalId',
    `WHERE d.VentaId = ${idVenta} AND v.Estado <> -1 AND d.ProductoId > 0`,
];
const RELANZA = (I) => `${I}-- ${MARCA}: el aviso de stock se reenvia a quien llamo (web / escritorio)${NL}`
    + `${I}IF ERROR_MESSAGE() LIKE 'STOCK INSUFICIENTE:%'${NL}${I}BEGIN${NL}${I}\tDECLARE @msgStockCatch nvarchar(2048) = ERROR_MESSAGE()${NL}${I}\tRAISERROR('%s', 16, 1, @msgStockCatch)${NL}${I}END${NL}`;

function ventas(s) {
    // @tipo 2 (primero, para no mover los indices de @tipo 1): validacion dentro del TRY y el CATCH reenvia el aviso
    const t2 = buscar(s, /IF\s+@tipo\s*=\s*2\s*--\s*MODIFICACION/i, 0, s.length, 'MAM_Ventas @tipo 2').idx;
    const t3 = buscar(s, /IF\s+@tipo\s*=\s*3\s*--MOSTrar Ventas/i, t2, s.length, 'MAM_Ventas @tipo 3').idx;
    const bloque2 = s.slice(t2, t3);
    const ultCatch = bloque2.toUpperCase().lastIndexOf('END CATCH');
    if (ultCatch < 0) falla('No se encontro el END CATCH de MAM_Ventas @2');
    const posFin = t2 + ultCatch;
    const iniLinea = s.lastIndexOf('\n', posFin - 1) + 1;
    s = s.slice(0, iniLinea) + RELANZA('\t\t\t') + s.slice(iniLinea);
    const bt2 = buscar(s, /begin tran Tr_InsertTI001/i, t2, t3, 'begin tran de MAM_Ventas @2');
    s = insertarDespuesDeLinea(s, bt2.idx, validacion('V2', [
        `SELECT td.ProductoId, ${DEP_SUC('@SucursalId')}, -td.CantidadUnitaria`,
        'FROM @VentaDetalleType AS td WHERE td.estado >= 0 AND td.ProductoId > 0 AND td.CantidadUnitaria > 0',
        ...DEVUELVE('@Id'),
    ], '\t\t\t', 'catch'));
    // @tipo 1: validacion fuera del TRY
    const t1 = buscar(s, /IF\s+@tipo\s*=\s*1\s*--NUEVO REGISTRO/i, 0, s.length, 'MAM_Ventas @tipo 1').idx;
    const t2b = buscar(s, /IF\s+@tipo\s*=\s*2\s*--\s*MODIFICACION/i).idx;
    const bt1 = buscar(s, /BEGIN TRAN INSERTAR/i, t1, t2b, 'BEGIN TRAN INSERTAR de MAM_Ventas @1');
    return insertarDespuesDeLinea(s, bt1.idx, validacion('V1', [
        `SELECT td.ProductoId, ${DEP_SUC('@SucursalId')}, -SUM(td.CantidadUnitaria)`,
        'FROM @VentaDetalleType AS td WHERE td.estado = 0 AND td.ProductoId > 0 AND td.CantidadUnitaria > 0 GROUP BY td.ProductoId',
    ], '\t\t', 'propia'));
}

function movimientos(s) {
    // El CATCH de @-1 hacia ROLLBACK de un nombre mal escrito; si en esta base ya esta bien, no se toca
    const typo = s.split('ROLLBACK TRAN ELMINAR').length - 1;
    if (typo > 1) falla('ROLLBACK TRAN ELMINAR aparece mas de una vez');
    if (typo === 1) s = s.replace('ROLLBACK TRAN ELMINAR', 'ROLLBACK TRAN ELIMINAR /* Sanbit: decia ELMINAR y el ROLLBACK fallaba */');
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
    ], '\t\t', 'propia'));
    const t1 = buscar(s, /IF\s+@tipo\s*=\s*1\s*--NUEVO REGISTRO/i, 0, s.length, 'MAM_Movimientos @tipo 1').idx;
    const t2b = buscar(s, /IF\s+@tipo\s*=\s*2\s*--MODIFICACION/i).idx;
    const b1 = buscar(s, /BEGIN TRAN INSERTAR/i, t1, t2b, 'BEGIN TRAN INSERTAR de MAM_Movimientos');
    s = insertarDespuesDeLinea(s, b1.idx, validacion('M1', [
        `SELECT td.ProductoId, @DepositoId, SUM(td.Cantidad) * ${FACTOR}`,
        'FROM @detalle AS td WHERE td.ProductoId > 0 AND td.estado >= 0 GROUP BY td.ProductoId',
    ], '\t\t', 'propia'));
    const tm = buscar(s, /IF\s+@tipo\s*=\s*-1\s*--ELIMINAR REGISTRO/i, 0, s.length, 'MAM_Movimientos @tipo -1').idx;
    const t1c = buscar(s, /IF\s+@tipo\s*=\s*1\s*--NUEVO REGISTRO/i).idx;
    const bm = buscar(s, /BEGIN TRAN ELIMINAR/i, tm, t1c, 'BEGIN TRAN ELIMINAR de MAM_Movimientos');
    return insertarDespuesDeLinea(s, bm.idx, validacion('MB', [
        'SELECT d.ProductoId, m.DepositoId, -d.Cantidad * c.Factor -- eliminar un ingreso descuenta stock',
        'FROM MovimientosDetalle AS d INNER JOIN Movimientos AS m ON m.Id = d.MovimientoId INNER JOIN MovimientosTipos AS c ON c.Id = m.ConceptoId',
        'WHERE d.MovimientoId = @id',
    ], '\t\t', 'propia'));
}

// Pedidos del celular (preventa). Los productos llegan en el XML @ventas1 (/row/row: obcprod, obpcant, ...)
const XML_PEDIDO = [
    'FROM (SELECT DISTINCT x.v.value(\'obcprod[1]\',\'int\') AS ProductoId, x.v.value(\'obpcant[1]\',\'numeric(18,5)\') AS Cantidad,',
    '      x.v.value(\'obpbase[1]\',\'numeric(18,4)\') AS Precio, x.v.value(\'descuento[1]\',\'numeric(18,2)\') AS Descuento,',
    '      x.v.value(\'total[1]\',\'numeric(18,4)\') AS Total',
    '      FROM @ventas1.nodes(\'/row/row\') x(v)) AS px',
    'WHERE px.ProductoId > 0 AND px.Cantidad > 0 GROUP BY px.ProductoId',
];
function movil(s) {
    // @tipo 29 (modificar pedido): dentro del TRY; el CATCH del SP hace ROLLBACK y reenvia el mensaje a la app
    const t29 = buscar(s, /IF\s+@tipo\s*=\s*29\s*--\s*MODIFICAR PEDIDO/i, 0, s.length, 'sp_go_TC004_appMovil @tipo 29').idx;
    const bt29 = buscar(s, /BEGIN TRANSACTION MODIFICACION;/i, t29, s.length, 'BEGIN TRANSACTION de @29');
    s = insertarDespuesDeLinea(s, bt29.idx, validacion('A29', [
        `SELECT ad.ProductoId, ${DEP_SUC('(SELECT v.SucursalId FROM Ventas AS v WHERE v.Id = @oanumi)')}, -CAST(ad.Cantidad AS decimal(18,2))`,
        'FROM AuditoriaPedidoMovilDetalle AS ad WHERE ad.AuditoriaId = @AuditoriaId AND ISNULL(ad.ObUpdate, 0) >= 0 AND ad.ProductoId > 0',
        ...DEVUELVE('@oanumi'),
    ], '        ', 'catch'));
    // @tipo 27 (detalle de un pedido que aun no tiene detalle)
    const t27 = buscar(s, /IF\s+@tipo\s*=\s*27\s*--INSERTAR DETALLE PEDIDO NUEVO/i, 0, s.length, 'sp_go_TC004_appMovil @tipo 27').idx;
    const t28 = buscar(s, /IF\s+@tipo\s*=\s*28\s*--/i, t27, s.length, 'sp_go_TC004_appMovil @tipo 28').idx;
    const bt27 = buscar(s, /BEGIN TRAN MODIFICACION/i, t27, t28, 'BEGIN TRAN de @27');
    s = insertarDespuesDeLinea(s, bt27.idx, validacion('A27', [
        `SELECT px.ProductoId, ${DEP_SUC('(SELECT v.SucursalId FROM Ventas AS v WHERE v.Id = @oanumi)')}, -SUM(px.Cantidad)`,
        ...XML_PEDIDO,
    ], '\t\t', 'propia', 'NOT EXISTS (SELECT 1 FROM VentasDetalles AS vd WHERE vd.VentaId = @oanumi)'));
    // @tipo 26 (pedido nuevo): solo si el pedido aun no fue sincronizado (si ya llego, se devuelve el mismo sin validar)
    const t26 = buscar(s, /IF\s+@tipo\s*=\s*26\s*--INSERTAR PEDIDO NUEVO/i, 0, s.length, 'sp_go_TC004_appMovil @tipo 26').idx;
    const t27b = buscar(s, /IF\s+@tipo\s*=\s*27\s*--/i, t26, s.length).idx;
    const bt26 = buscar(s, /BEGIN TRAN MODIFICACION/i, t26, t27b, 'BEGIN TRAN de @26');
    if (!/insert into Ventas values\(@oanumi,1,/i.test(s.slice(bt26.idx, t27b))) falla('@26: el pedido ya no se graba en la sucursal 1; revisar a mano');
    return insertarDespuesDeLinea(s, bt26.idx, validacion('A26', [
        `SELECT px.ProductoId, ${DEP_SUC('1')}, -SUM(px.Cantidad) -- @26 graba el pedido en la sucursal 1`,
        ...XML_PEDIDO,
    ], '\t\t', 'propia', 'NOT EXISTS (SELECT 1 FROM Ventas AS vs WHERE vs.IdSincronizacion = @codigogenerado)'));
}

// Triggers: el saldo se actualiza en UNA sentencia (antes: leer y luego pisar -> dos grabaciones a la vez perdian una)
function trigger(nombre, s, esperadas) {
    let n = 0;
    s = s.replace(/set @cantA\s*=\s*\(select Cantidad\s+from ProductosStock[^)]*\)\s*set @cantF\s*=\s*(@cantA\s*\+\s*\(@cantI\*@mov\)|@cantI\s*\+\s*@cantA)(\s*--Actualizar Saldo\s*)update ProductosStock\s+set Cantidad\s*=\s*@cantF/gi,
        (m, expr, medio) => {
            n += 1;
            const delta = expr.startsWith('@cantA') ? '(@cantI*@mov)' : '@cantI';
            return `--${MARCA_TR} (antes se leia y luego se pisaba: dos grabaciones a la vez perdian una)${medio}update ProductosStock set @cantF = Cantidad = Cantidad + ${delta}`;
        });
    if (n !== esperadas) falla(`${nombre}: se encontraron ${n} actualizaciones de saldo (se esperaban ${esperadas}); el trigger de esta base es distinto`);
    return s;
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos(OBJ);
    for (const o of OBJ) if (def[o].includes(MARCA) || def[o].includes(MARCA_TR)) falla(`${o} ya tiene StockSeguro en ${bd}: no hace falta generar nada`);
    const nuevo = {
        MAM_Ventas: ventas(def.MAM_Ventas), MAM_Movimientos: movimientos(def.MAM_Movimientos), sp_go_TC004_appMovil: movil(def.sp_go_TC004_appMovil),
        Trigger_MovimientoInsertar: trigger('Trigger_MovimientoInsertar', def.Trigger_MovimientoInsertar, 1),
        Trigger_MovimientoDetalleEliminar: trigger('Trigger_MovimientoDetalleEliminar', def.Trigger_MovimientoDetalleEliminar, 1),
        Trigger_MovimientosDetalleModificar: trigger('Trigger_MovimientosDetalleModificar', def.Trigger_MovimientosDetalleModificar, 2),
    };
    const hoy = new Date().toISOString().slice(0, 10);
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const cab = [
        '/* =====================================================================',
        '   04 - STOCK SEGURO: NO SE VENDE NI SE SACA MAS DE LO QUE HAY',
        '   (la web, el escritorio y la app del celular usan estos mismos SP y triggers)',
        '',
        '   1. Pedidos: MAM_Ventas @1 (nuevo) / @2 (modificar) y sp_go_TC004_appMovil @26 (pedido nuevo del',
        '      celular), @27 (detalle del pedido) y @29 (modificar pedido): antes de grabar calculan lo que el',
        '      pedido descuenta, BLOQUEAN esas filas de stock (UPDLOCK) y si no alcanza no graban nada y avisan:',
        '        "STOCK INSUFICIENTE: Producto (stock actual X, necesita Y); ..."',
        '      -> si dos preventistas piden el mismo producto a la vez, el segundo espera y ve el stock real.',
        '      Un pedido del celular que YA se sincronizo (mismo codigo) se devuelve igual, sin volver a validar.',
        '   2. Movimientos de inventario MAM_Movimientos @1 / @2 / @-1: una salida no puede dejar stock negativo y',
        '      ELIMINAR UN INGRESO que ya se vendio o salio tampoco (se avisa con los productos).',
        '      El CATCH de @-1 hacia "ROLLBACK TRAN ELMINAR" (nombre mal escrito) y fallaba: corregido.',
        '   3. Triggers de MovimientosDetalle: el saldo se actualiza en UNA sentencia',
        '      (antes leian el saldo y despues lo pisaban: dos grabaciones a la vez perdian una).',
        '   Compras no se valida al grabar (es ingreso); eliminar una compra lo valida el 05.',
        '   Todo lo demas de cada objeto queda IDENTICO a la version de la BD.',
        '',
        `   Generado desde ${bd} (${hoy}). PROTECCION: solo se aplica si el codigo de los ${OBJ.length} objetos es esa`,
        '   version (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si alguno es',
        '   otra version (otra BD / otra rama), da error y no modifica nada.',
        '   REQUISITO: 01_IX_Indices.sql (igual se crea aqui el indice de ProductosStock si falta).',
        '   VOLVER ATRAS: revertir\\REVERTIR_04_StockSeguro.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF EXISTS (SELECT 1 FROM ${VALORES} WHERE ${MARCAS})`,
        'BEGIN',
        "\tRAISERROR('04 StockSeguro YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ${OBJ.map((o) => `ISNULL(${HUELLA(o)}, 0x) <> ${hash[o]}`).join(`${NL}   OR `)}`,
        'BEGIN',
        `\tRAISERROR('04 StockSeguro: algun SP o trigger es OTRA version (no la de ${bd} ${hoy}). No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        '-- Necesario para que el bloqueo sea solo de los productos del pedido (busqueda por indice)',
        "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductosStock_Producto_Deposito' AND object_id = OBJECT_ID('dbo.ProductosStock'))",
        '\tCREATE NONCLUSTERED INDEX IX_ProductosStock_Producto_Deposito ON dbo.ProductosStock (ProductoId, DepositoId, Lote, FechaVencimiento) INCLUDE (Cantidad)',
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = [
        'SET NOEXEC OFF', 'GO',
        `IF (SELECT COUNT(*) FROM ${VALORES} WHERE ${MARCAS}) = ${OBJ.length}`,
        "\tRAISERROR('04 StockSeguro INSTALADO.', 10, 1) WITH NOWAIT",
        'GO',
    ].join(NL);
    const r1 = escribir(bd, '04_StockSeguro.sql', `${cab}${NL}${OBJ.map((o) => `${aAlter(nuevo[o])}${NL}GO`).join(NL)}${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 04_StockSeguro.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${OBJ.join(', ')} de ${bd}`,
        '   a la version anterior (sin validacion de stock). Si StockSeguro no esta instalado, avisa y no hace nada.',
        '   Si el 05 esta instalado, deshacer primero el 05 (no depende de este, pero se deshace en orden inverso).',
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT EXISTS (SELECT 1 FROM ${VALORES} WHERE ${MARCAS})`,
        'BEGIN',
        "\tRAISERROR('04 StockSeguro NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        ...OBJ.map((o) => `${aAlter(def[o])}${NL}GO`),
        'SET NOEXEC OFF', 'GO',
        "RAISERROR('04 StockSeguro DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_04_StockSeguro.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
