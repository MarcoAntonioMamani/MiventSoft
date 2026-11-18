// =====================================================================
//  Genera 06_IncrementoMovil.sql y revertir/REVERTIR_06_IncrementoMovil.sql para la BD conectada (Distral)
//    node sql/generar/incremento_movil.js                 (BD de server/.env)
//    node sql/generar/incremento_movil.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee sp_go_TC004_appMovil de ESA base (YA con el 04 instalado) y le agrega el incremento del dia, igual que
//  MAM_Ventas (web/escritorio): fila en VentasIncrementoAplicado y TotalVenta (-> credito) con el incremento.
//  Si el codigo de esa base es distinto, se detiene sin generar nada.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'sp_go_TC004_appMovil';
const MARCA = 'Sanbit: incremento del dia';
const MARCA_04 = 'Sanbit: validacion de stock';

/** Reemplaza "viejo" por "nuevo" entre desde..hasta; tiene que aparecer exactamente "veces" veces */
function cambiar(s, viejo, nuevo, desde, hasta, veces, nombre) {
    const tramo = s.slice(desde, hasta);
    const n = tramo.split(viejo).length - 1;
    if (n !== veces) falla(`${nombre}: se esperaba ${veces} vez/veces y aparece ${n}. El SP de esta base es distinto, revisarlo a mano`);
    return s.slice(0, desde) + tramo.split(viejo).join(nuevo) + s.slice(hasta);
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    const d0 = def[SP];
    if (d0.includes(MARCA)) falla(`${SP} de ${bd} ya tiene el incremento: no hace falta generar nada`);
    if (!d0.includes(MARCA_04)) falla(`${SP} de ${bd} NO tiene el 04 (StockSeguro): instalar primero 01..05 y generar despues`);
    if (/@incremento\b/i.test(d0)) falla(`${SP} de ${bd} ya tiene un parametro @incremento: revisarlo a mano`);
    const existeTabla = (await MetodoDatos.EjecutarConsulta("SELECT OBJECT_ID('dbo.VentasIncrementoAplicado') t"))[0].t;
    if (!existeTabla) falla(`${bd} no tiene la tabla VentasIncrementoAplicado (incremento de la web)`);

    let d = d0;
    // 1. Parametro nuevo (opcional): -1 = la app vieja no lo manda -> no se registra incremento
    const pIni = buscar(d, /@oaap int=-1,@ventaDirecta int=-1,/i, 0, d.length, 'parametro @ventaDirecta');
    d = cambiar(d, '@oaap int=-1,@ventaDirecta int=-1,', `@oaap int=-1,@ventaDirecta int=-1,@incremento decimal(18,2)=-1,`, pIni.idx, pIni.fin, 1, 'parametros');

    // 2. @26 pedido nuevo: TotalVenta con el incremento + fila en VentasIncrementoAplicado (misma transaccion)
    let a = buscar(d, /IF\s+@tipo\s*=\s*26\s*--INSERTAR PEDIDO NUEVO/i, 0, d.length, '@tipo=26').idx;
    let b = buscar(d, /IF\s+@tipo\s*=\s*27\s*--INSERTAR DETALLE PEDIDO NUEVO/i, a, d.length, '@tipo=27').idx;
    d = cambiar(d, "@oaobs,0,@total,GETDATE (),@oahora,'APP',@oaest,", "@oaobs,0,@total + IIF(@incremento > 0, @incremento, 0),GETDATE (),@oahora,'APP',@oaest,", a, b, 1, '@26 insert Ventas');
    b = buscar(d, /IF\s+@tipo\s*=\s*27\s*--INSERTAR DETALLE PEDIDO NUEVO/i, a, d.length, '@tipo=27').idx;
    const finIns = buscar(d, /0,@oafdoc,@codigogenerado,'Movil',@latitud,@longitud ,@ventaDirecta,0,0\)[^\n]*\n/i, a, b, '@26 fin del insert Ventas');
    const bloque26 = [
        `\t\t-- ===== ${MARCA} (app movil nueva, /apiV4) ======================================================`,
        '\t\t-- La app manda el incremento que le mostro al cliente (0 o mas). La app vieja no lo manda (-1): no se registra.',
        '\t\t-- Igual que MAM_Ventas: una fila en VentasIncrementoAplicado; TotalVenta (y el credito) ya lo incluyen.',
        '\t\tIF (@incremento >= 0)',
        '\t\t\tINSERT INTO VentasIncrementoAplicado (VentaId, Monto, FechaHoraRegistro) VALUES (@oanumi, @incremento, SYSDATETIME())',
        '\t\t-- ==========================================================================================',
        '',
    ].join(NL);
    d = d.slice(0, finIns.fin) + bloque26 + d.slice(finIns.fin);

    // 3. @29 modificar pedido: el total recalculado conserva el incremento registrado de ESA venta (como la web)
    a = buscar(d, /IF\s+@tipo\s*=\s*29\s*--\s*MODIFICAR PEDIDO/i, 0, d.length, '@tipo=29').idx;
    const setTot = buscar(d, /SET TotalVenta =\s*\(\s*SELECT ISNULL\(SUM\(vd\.Total\), 0\)\s*FROM VentasDetalles AS vd\s*WHERE vd\.VentaId = @oanumi\s*\)/i, a, d.length, '@29 SET TotalVenta');
    d = d.slice(0, setTot.fin)
        + `${NL}        + ISNULL((SELECT TOP 1 inc.Monto FROM VentasIncrementoAplicado AS inc WHERE inc.VentaId = @oanumi ORDER BY inc.Id DESC), 0) -- ${MARCA}: el registrado`
        + d.slice(setTot.fin);

    // 4. @24 lista de pedidos para el celular: devuelve el incremento registrado de cada pedido
    a = buscar(d, /IF\s+@tipo\s*=\s*24\s*--ListPedidos/i, 0, d.length, '@tipo=24').idx;
    b = buscar(d, /IF\s+@tipo\s*=\s*26\s*--INSERTAR PEDIDO NUEVO/i, a, d.length, '@tipo=26').idx;
    const INC = ',isnull((select top 1 inc.Monto from VentasIncrementoAplicado as inc where inc.VentaId=a.Id order by inc.Id desc),0) as incremento';
    d = cambiar(d, ',a.Estado as oaap,a.VentaDirecta as ventaDirecta', `,a.Estado as oaap,a.VentaDirecta as ventaDirecta${INC}`, a, b, 1, '@24 select (ventaDirecta)');
    b = buscar(d, /IF\s+@tipo\s*=\s*26\s*--INSERTAR PEDIDO NUEVO/i, a, d.length, '@tipo=26').idx;
    d = cambiar(d, ",1 as estadoStock,a.Estado as oaap,a.VentaDirecta\r\n", `,1 as estadoStock,a.Estado as oaap,a.VentaDirecta${INC}\r\n`, a, b, 3, '@24 selects');

    const hoy = new Date().toISOString().slice(0, 10);
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const yaEsta = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const cab = [
        '/* =====================================================================',
        '   06 - INCREMENTO DEL DIA EN LOS PEDIDOS DEL CELULAR (igual que la web / escritorio)',
        `   Cambia SOLO ${SP}:`,
        '   1. Parametro nuevo @incremento (opcional, -1 = no vino). Lo manda la app nueva (/apiV4);',
        '      la app vieja (/apiV3) no lo manda y sus pedidos quedan exactamente como hoy.',
        '   2. @26 pedido nuevo: TotalVenta = total de la app + incremento (el credito sale con ese total, por el',
        '      trigger de Ventas) y una fila en VentasIncrementoAplicado, igual que MAM_Ventas.',
        '      Un pedido que ya llego (mismo codigo de sincronizacion) no se toca: no se suma dos veces.',
        '   3. @29 modificar pedido: el total recalculado conserva el incremento REGISTRADO de esa venta',
        '      (antes se perdia al modificar desde el celular un pedido con incremento).',
        '   4. @24 lista de pedidos del celular: devuelve tambien "incremento" (la app vieja lo ignora).',
        '   Todo lo demas del SP queda IDENTICO.',
        '',
        `   Generado desde ${bd} (${hoy}), con el 04 instalado. PROTECCION: solo se aplica si el SP es esa version`,
        '   (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da',
        '   error y no modifica nada.',
        '   REQUISITO: 04_StockSeguro.sql instalado y la tabla VentasIncrementoAplicado (incremento de la web).',
        '   VOLVER ATRAS: revertir\\REVERTIR_06_IncrementoMovil.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${yaEsta}`,
        'BEGIN',
        "\tRAISERROR('06 IncrementoMovil YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        "ELSE IF OBJECT_ID('dbo.VentasIncrementoAplicado') IS NULL",
        'BEGIN',
        "\tRAISERROR('06 IncrementoMovil: falta la tabla VentasIncrementoAplicado (incremento de la web). No se modifico nada.', 16, 1)",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('06 IncrementoMovil: ${SP} es OTRA version (no la de ${bd} ${hoy} con el 04). Instalar antes 01..05. No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = ['SET NOEXEC OFF', 'GO', `IF ${yaEsta}`, "\tRAISERROR('06 IncrementoMovil INSTALADO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r1 = escribir(bd, '06_IncrementoMovil.sql', `${cab}${NL}${aAlter(d)}${NL}GO${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 06_IncrementoMovil.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version con el 04 y sin incremento. Los pedidos ya grabados con`,
        '   incremento conservan su TotalVenta y su fila en VentasIncrementoAplicado.',
        '   ANTES: volver a la app que usa /apiV3 (la /apiV4 manda @incremento, que sin el 06 no existe).',
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${yaEsta})`,
        'BEGIN',
        "\tRAISERROR('06 IncrementoMovil NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        `${aAlter(d0)}${NL}GO`,
        'SET NOEXEC OFF', 'GO',
        "RAISERROR('06 IncrementoMovil DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_06_IncrementoMovil.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
