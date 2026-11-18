// =====================================================================
//  Genera 10_RolMovil.sql y revertir/REVERTIR_10_RolMovil.sql para la BD conectada (Distral)
//    node sql/generar/rol_movil.js                 (BD de server/.env)
//    node sql/generar/rol_movil.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee sp_go_TC004_appMovil de ESA base (YA con el 08 instalado). En @tipo=20 (login del celular) agrega la
//  columna "rol" segun Personal.TipoPersonal (ClasificadorDetalle): 1 = vendedor / preventista,
//  2 = repartidor / distribuidor, 0 = otro. La app 3.4 arma el inicio segun el rol (no quita permisos).
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'sp_go_TC004_appMovil';
const MARCA = 'Sanbit: rol del celular';
const MARCA_08 = 'Sanbit: pedido con detalle';

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    const d0 = def[SP];
    if (d0.includes(MARCA)) falla(`${SP} de ${bd} ya tiene el 10: no hace falta generar nada`);
    if (!d0.includes(MARCA_08)) falla(`${SP} de ${bd} NO tiene el 08 (PedidoConDetalle): instalar primero 01..08 y generar despues`);

    // Solo dentro de @tipo=20 (login): despues de la columna "categorias", antes del FROM Personal
    const a = buscar(d0, /IF\s*@tipo\s*=\s*20\b/i, 0, d0.length, '@tipo=20').idx;
    const b = buscar(d0, /IF\s*@tipo\s*=\s*21\b/i, a, d0.length, '@tipo=21').idx;
    const col = buscar(d0, /\)\s*as\s+categorias[ \t]*(?=\r?\n\s*from\s+Personal\s+a\b)/i, a, b, '@20 columna categorias antes de FROM Personal');
    const nueva = [
        ',',
        `\t\t\t -- ${MARCA}: 1 vendedor/preventista, 2 repartidor/distribuidor, 0 otro (la app arma el inicio segun esto)`,
        "\t\t\t isnull((SELECT TOP 1 CASE WHEN cd.Descripcion LIKE '%repart%' OR cd.Descripcion LIKE '%distrib%' OR cd.Descripcion LIKE '%chofer%' THEN 2",
        "\t\t\t\tWHEN cd.Descripcion LIKE '%vend%' OR cd.Descripcion LIKE '%prevent%' THEN 1 ELSE 0 END",
        '\t\t\t\tFROM ClasificadorDetalle AS cd WHERE cd.Id = a.TipoPersonal), 0) as rol',
    ].join(NL);
    const d = d0.slice(0, col.fin) + nueva + d0.slice(col.fin);

    const hoy = new Date().toISOString().slice(0, 10);
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const yaEsta = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const cab = [
        '/* =====================================================================',
        '   10 - ROL DEL CELULAR EN EL LOGIN (preventista / distribuidor)',
        `   Cambia SOLO ${SP} @20 (login del celular): agrega la columna "rol" segun Personal.TipoPersonal`,
        '   (1 vendedor/preventista, 2 repartidor/distribuidor, 0 otro). La app 3.4 muestra en el inicio lo que le',
        '   sirve a cada uno; no quita permisos. La app vieja ignora la columna. Todo lo demas del SP queda IDENTICO.',
        '',
        `   Generado desde ${bd} (${hoy}), con el 08 instalado. PROTECCION: solo se aplica si el SP es esa version`,
        '   (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da',
        '   error y no modifica nada.',
        '   REQUISITO: 08_PedidoConDetalle.sql instalado. Despues: reiniciar WebServicesDynasys (access_data.js con "rol").',
        '   VOLVER ATRAS: revertir\\REVERTIR_10_RolMovil.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${yaEsta}`,
        'BEGIN',
        "\tRAISERROR('10 RolMovil YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('10 RolMovil: ${SP} es OTRA version (no la de ${bd} ${hoy} con el 08). Instalar antes 01..08. No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = ['SET NOEXEC OFF', 'GO', `IF ${yaEsta}`, "\tRAISERROR('10 RolMovil INSTALADO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r1 = escribir(bd, '10_RolMovil.sql', `${cab}${NL}${aAlter(d)}${NL}GO${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 10_RolMovil.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version con el 08 (login sin la columna rol).`,
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${yaEsta})`,
        'BEGIN',
        "\tRAISERROR('10 RolMovil NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        `${aAlter(d0)}${NL}GO`,
        'SET NOEXEC OFF', 'GO',
        "RAISERROR('10 RolMovil DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_10_RolMovil.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
