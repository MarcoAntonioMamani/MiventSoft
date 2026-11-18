// =====================================================================
//  Genera 08_PedidoConDetalle.sql y revertir/REVERTIR_08_PedidoConDetalle.sql para la BD conectada (Distral)
//    node sql/generar/pedido_detalle.js                 (BD de server/.env)
//    node sql/generar/pedido_detalle.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee sp_go_TC004_appMovil de ESA base (YA con el 07 instalado). En @26 (pedido nuevo del celular), antes del
//  COMMIT, verifica que TODOS los productos que mando el celular quedaron en VentasDetalles. Si el pedido llega
//  sin productos o falta alguno: ROLLBACK y error (no queda la cabecera sola en Ventas).
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'sp_go_TC004_appMovil';
const MARCA = 'Sanbit: pedido con detalle';
const MARCA_07 = 'Sanbit: ubicacion del celular';

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    const d0 = def[SP];
    if (d0.includes(MARCA)) falla(`${SP} de ${bd} ya tiene el 08: no hace falta generar nada`);
    if (!d0.includes(MARCA_07)) falla(`${SP} de ${bd} NO tiene el 07 (UbicacionMovil): instalar primero 01..07 y generar despues`);

    // @26: antes de "set @NroConciliacion" (sigue dentro de la rama "pedido nuevo" y de la transaccion)
    const a = buscar(d0, /IF\s+@tipo\s*=\s*26\s*--INSERTAR PEDIDO NUEVO/i, 0, d0.length, '@tipo=26').idx;
    const b = buscar(d0, /IF\s+@tipo\s*=\s*27\s*--INSERTAR DETALLE PEDIDO NUEVO/i, a, d0.length, '@tipo=27').idx;
    const conc = buscar(d0, /\r?\n[ \t]*set @NroConciliacion =isnull\(\(select top 1 id from Conciliaciones/i, a, b, '@26 set @NroConciliacion');
    const bloque = [
        '',
        `\t-- ===== ${MARCA}: nunca la cabecera sola =====================================================`,
        '\t-- Todos los productos (con cantidad) que mando el celular deben estar en VentasDetalles. Si el pedido llego',
        '\t-- vacio o falta alguno: error -> CATCH -> ROLLBACK (no se graba nada) y el celular lo vuelve a intentar.',
        '\tIF NOT EXISTS (SELECT 1 FROM VentasDetalles AS vdx WHERE vdx.VentaId = @oanumi)',
        "\t\tRAISERROR('PEDIDO SIN DETALLE: el pedido llego sin productos. No se grabo.', 16, 1)",
        '\tIF EXISTS (SELECT 1 FROM @ventas1.nodes(\'/row/row\') AS x(v)',
        "\t\t\tWHERE x.v.value('obcprod[1]','int') > 0 AND x.v.value('obpcant[1]','numeric(18,5)') > 0",
        '\t\t\tAND NOT EXISTS (SELECT 1 FROM VentasDetalles AS vdx',
        "\t\t\t                WHERE vdx.VentaId = @oanumi AND vdx.ProductoId = x.v.value('obcprod[1]','int')))",
        "\t\tRAISERROR('PEDIDO INCOMPLETO: no se grabaron todos los productos del pedido. No se grabo.', 16, 1)",
        '\t-- ==========================================================================================',
    ].join(NL);
    const d = d0.slice(0, conc.idx) + bloque + d0.slice(conc.idx);

    const hoy = new Date().toISOString().slice(0, 10);
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const yaEsta = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const cab = [
        '/* =====================================================================',
        '   08 - PEDIDO DEL CELULAR SIEMPRE CON SU DETALLE (nunca la cabecera sola en Ventas)',
        `   Cambia SOLO ${SP} @26 (pedido nuevo del celular): antes del COMMIT verifica que todos los productos`,
        '   que mando el celular quedaron en VentasDetalles. Si llego sin productos o falta alguno: ROLLBACK, no se',
        '   graba nada y el celular recibe el error (lo vuelve a intentar). Todo lo demas del SP queda IDENTICO.',
        '',
        `   Generado desde ${bd} (${hoy}), con el 07 instalado. PROTECCION: solo se aplica si el SP es esa version`,
        '   (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da',
        '   error y no modifica nada.',
        '   REQUISITO: 07_UbicacionMovil.sql instalado.',
        '   VOLVER ATRAS: revertir\\REVERTIR_08_PedidoConDetalle.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${yaEsta}`,
        'BEGIN',
        "\tRAISERROR('08 PedidoConDetalle YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('08 PedidoConDetalle: ${SP} es OTRA version (no la de ${bd} ${hoy} con el 07). Instalar antes 01..07. No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = ['SET NOEXEC OFF', 'GO', `IF ${yaEsta}`, "\tRAISERROR('08 PedidoConDetalle INSTALADO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r1 = escribir(bd, '08_PedidoConDetalle.sql', `${cab}${NL}${aAlter(d)}${NL}GO${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 08_PedidoConDetalle.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version con el 07 (sin la verificacion del detalle).`,
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${yaEsta})`,
        'BEGIN',
        "\tRAISERROR('08 PedidoConDetalle NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        `${aAlter(d0)}${NL}GO`,
        'SET NOEXEC OFF', 'GO',
        "RAISERROR('08 PedidoConDetalle DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_08_PedidoConDetalle.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
