// =====================================================================
//  Genera 13_EntregaSinCambios.sql y revertir/REVERTIR_13_EntregaSinCambios.sql (Distral)
//    node sql/generar/entrega_sin_cambios.js                 (BD de server/.env)
//    node sql/generar/entrega_sin_cambios.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee sp_go_TC004_appMovil de ESA base (con el 10 instalado).
//  Problema: al entregar, la app sube el estado (@28) y despues SIEMPRE reenvia el detalle completo (@29), aunque el
//  repartidor no haya tocado nada. El @29 borra y vuelve a insertar el detalle y al final rechaza los pedidos con
//  FechaVenta distinta de hoy ("No se puede modificar pedido de otra fecha diferente a la actual"): toda entrega de
//  un pedido de preventa de otro dia fallaba y el celular la reintentaba sin fin (queda "por subir").
//  Arreglo: si el detalle recibido es IGUAL al de la venta (mismos productos, cantidades y precios) el @29 no toca
//  nada y responde OK. Un cambio real sigue pasando por todo (stock, fecha, auditoria) como antes.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'sp_go_TC004_appMovil';
const MARCA = 'Sanbit: entrega sin cambios';

const BLOQUE = [
    `        -- ===== ${MARCA} (13) =====================================================================`,
    '        -- Al entregar, la app reenvia el detalle completo aunque no lo haya cambiado. Si es igual al de la venta',
    '        -- (mismos productos, cantidades y precios) no se borra ni se reinserta nada y no se valida fecha ni stock:',
    '        -- solo se confirma. La regla "no modificar pedidos de otra fecha" sigue para los cambios reales.',
    '        IF EXISTS (SELECT 1 FROM Ventas WHERE Id = @oanumi AND Estado <> -1)',
    '           AND NOT EXISTS (',
    '            SELECT 1',
    '            FROM (SELECT vd.ProductoId, SUM(CAST(vd.Cantidad AS decimal(18,5))) AS Cantidad, MAX(CAST(vd.Precio AS decimal(18,4))) AS Precio',
    '                  FROM VentasDetalles AS vd WHERE vd.VentaId = @oanumi GROUP BY vd.ProductoId) AS a',
    '            FULL OUTER JOIN',
    '                 (SELECT ad.ProductoId, SUM(CAST(ad.Cantidad AS decimal(18,5))) AS Cantidad, MAX(CAST(ad.PrecioBase AS decimal(18,4))) AS Precio',
    '                  FROM AuditoriaPedidoMovilDetalle AS ad WHERE ad.AuditoriaId = @AuditoriaId AND ISNULL(ad.ObUpdate, 0) >= 0 GROUP BY ad.ProductoId) AS n',
    '              ON n.ProductoId = a.ProductoId',
    '            WHERE a.ProductoId IS NULL OR n.ProductoId IS NULL OR a.Cantidad <> n.Cantidad OR a.Precio <> n.Precio)',
    '        BEGIN',
    '            COMMIT TRANSACTION MODIFICACION;',
    "            UPDATE AuditoriaPedidoMovil SET EstadoProceso = 'SIN CAMBIOS',",
    "                   MensajeError = 'Detalle igual al de la venta (entrega): no se modifico nada (13).'",
    '            WHERE AuditoriaId = @AuditoriaId;',
    '            SELECT @oanumi AS code_id, @AuditoriaId AS auditoria_id, 0 AS detalles_insertados;',
    '            RETURN;',
    '        END',
    '        -- ==========================================================================================',
    '',
].join(NL);

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    const d0 = def[SP];
    if (d0.includes(MARCA)) falla(`${SP} de ${bd} ya tiene el 13: no hace falta generar nada`);
    if (!d0.includes('Sanbit: rol del celular')) falla(`${SP} de ${bd} NO tiene el 10 (RolMovil): instalar primero 01..12 y generar despues`);

    // dentro del @29: justo despues de abrir la transaccion (antes de la validacion de stock del 04)
    const t = buscar(d0, /IF\s*@tipo\s*=\s*29\s*--\s*MODIFICAR\s+PEDIDO/i, 0, d0.length, '@tipo=29 MODIFICAR PEDIDO');
    const tr = buscar(d0, /BEGIN\s+TRANSACTION\s+MODIFICACION\s*;/i, t.fin, Math.min(d0.length, t.fin + 6000), 'BEGIN TRANSACTION MODIFICACION del @29');
    const ini = d0.indexOf('\n', tr.fin) + 1; // linea siguiente
    if (ini <= 0) falla('No se encontro el fin de linea del BEGIN TRANSACTION');
    const d = d0.slice(0, ini) + BLOQUE + d0.slice(ini);

    const hoy = new Date().toISOString().slice(0, 10);
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const yaEsta = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const cab = [
        '/* =====================================================================',
        '   13 - ENTREGA SIN CAMBIOS: NO RECHAZAR LA ENTREGA DE PEDIDOS DE OTRO DIA',
        `   Cambia SOLO ${SP} @29 (detalle del pedido desde el celular). Al entregar, la app reenvia el detalle`,
        '   completo aunque no lo haya cambiado y el @29 rechazaba los pedidos de otra fecha:',
        '     "No se puede modificar pedido de otra fecha diferente a la actual" -> el celular reintentaba sin fin.',
        '   Ahora, si el detalle es IGUAL al de la venta (productos, cantidades y precios), no toca nada y responde OK.',
        '   Un cambio real de cantidades o productos sigue igual que antes (stock, fecha, auditoria).',
        '   Todo lo demas del SP queda IDENTICO.',
        '',
        `   Generado desde ${bd} (${hoy}), con el 10 instalado. PROTECCION: solo se aplica si el SP es esa version`,
        '   (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da',
        '   error y no modifica nada (regenerar con: node sql/generar/entrega_sin_cambios.js).',
        '   REQUISITO: 10_RolMovil.sql instalado. No hace falta reiniciar la API ni actualizar la app.',
        '   VOLVER ATRAS: revertir\\REVERTIR_13_EntregaSinCambios.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${yaEsta}`,
        'BEGIN',
        "\tRAISERROR('13 EntregaSinCambios YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('13 EntregaSinCambios: ${SP} es OTRA version (no la de ${bd} ${hoy} con el 10). Instalar antes 01..12. No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = ['SET NOEXEC OFF', 'GO', `IF ${yaEsta}`, "\tRAISERROR('13 EntregaSinCambios INSTALADO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r1 = escribir(bd, '13_EntregaSinCambios.sql', `${cab}${NL}${aAlter(d)}${NL}GO${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 13_EntregaSinCambios.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version con el 10 (el @29 vuelve a rechazar las entregas de pedidos de otro dia).`,
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${yaEsta})`,
        'BEGIN',
        "\tRAISERROR('13 EntregaSinCambios NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        `${aAlter(d0)}${NL}GO`,
        'SET NOEXEC OFF', 'GO',
        "RAISERROR('13 EntregaSinCambios DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_13_EntregaSinCambios.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
