// =====================================================================
//  Genera sp_Mam_Precios_tipo5.sql para la BD conectada (lentitud de la pantalla Precios)
//    node sql/generar/precios_tipo5.js                 (BD de server/.env)
//    node sql/generar/precios_tipo5.js --bd=OtraBD     (otra BD del mismo servidor)
//  Cambia UNA linea de sp_Mam_Precios @tipo 5: el "inner join Productos as e" sin condicion sobre e
//  cruzaba cada precio con todos los productos (43 s -> 0.2 s, mismo resultado).
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, escribir } = require('./comun');

const MALO = 'select b.PrecioCategoriaId  from Precios  as b inner join Productos  as e on b.ProductoId  =c.Id and b.AlmacenId  =@almacen )';
const BUENO = 'select b.PrecioCategoriaId  from Precios  as b where b.ProductoId  =c.Id and b.AlmacenId  =@almacen )';

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos(['sp_Mam_Precios']);
    const d = def.sp_Mam_Precios;
    if (d.includes(BUENO) && !d.includes(MALO)) falla(`sp_Mam_Precios de ${bd} ya tiene la correccion: no hace falta generar nada`);
    if (d.split(MALO).length !== 2) falla(`La linea a corregir no aparece exactamente una vez en sp_Mam_Precios de ${bd}: revisarlo a mano`);
    const hoy = new Date().toISOString().slice(0, 10);
    const texto = [
        '/* =====================================================================',
        '   sp_Mam_Precios @tipo 5 (Tec_Precios: matriz producto x categoria)',
        '   CAMBIA UNA SOLA LINEA (resto del SP identico al de la BD):',
        `     antes:  ${MALO}`,
        `     ahora:  ${BUENO}`,
        '   El "inner join Productos as e" no tenia condicion sobre e: cruzaba cada precio con todos los',
        '   productos. Mismo resultado (en Kaily: 43 s -> 0.2 s cuando el SQL arma un mal plan).',
        '',
        `   Generado desde ${bd} (${hoy}). Si ya esta corregido avisa y no hace nada; si el SP es otra version,`,
        '   da error y no modifica nada.',
        '   VOLVER ATRAS: revertir\\REVERTIR_03_sp_Mam_Precios_tipo5.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF CHARINDEX(N'${BUENO.replace(/'/g, "''")}', OBJECT_DEFINITION(OBJECT_ID('dbo.sp_Mam_Precios'))) > 0`,
        'BEGIN',
        "\tRAISERROR('sp_Mam_Precios YA ESTA CORREGIDO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA('sp_Mam_Precios')}, 0x) <> ${hash.sp_Mam_Precios}`,
        'BEGIN',
        `\tRAISERROR('sp_Mam_Precios es OTRA version (no la de ${bd} ${hoy}). No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        aAlter(d).replace(MALO, BUENO),
        'GO',
        'SET NOEXEC OFF',
        'GO',
        `IF CHARINDEX(N'${BUENO.replace(/'/g, "''")}', OBJECT_DEFINITION(OBJECT_ID('dbo.sp_Mam_Precios'))) > 0`,
        "	RAISERROR('03 sp_Mam_Precios CORREGIDO.', 10, 1) WITH NOWAIT",
        'GO',
    ].join(NL);
    console.log(`OK (${bd}):\n  ${escribir(bd, '03_sp_Mam_Precios_tipo5.sql', `${texto}${NL}`)}`);
    const rev = [
        '/* SOLO PARA DESHACER 03_sp_Mam_Precios_tipo5.sql: vuelve sp_Mam_Precios a la version anterior. NO es parte de la instalacion. */',
        'SET NOEXEC OFF', 'GO',
        `IF CHARINDEX(N'${BUENO.replace(/'/g, "''")}', OBJECT_DEFINITION(OBJECT_ID('dbo.sp_Mam_Precios'))) = 0`,
        'BEGIN', "\tRAISERROR('03 NO esta instalado: no hay nada que deshacer.', 10, 1) WITH NOWAIT", '\tSET NOEXEC ON', 'END', 'GO',
        'SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO', aAlter(d), 'GO', 'SET NOEXEC OFF', 'GO',
        "RAISERROR('03 DESHECHO.', 10, 1) WITH NOWAIT", 'GO',
    ].join(NL);
    console.log(`  ${escribir(bd, 'revertir/REVERTIR_03_sp_Mam_Precios_tipo5.sql', `${rev}${NL}`)}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
