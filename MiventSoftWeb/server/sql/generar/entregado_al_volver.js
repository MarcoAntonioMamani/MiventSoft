// =====================================================================
//  Genera 14_EntregadosAlVolver.sql y revertir/REVERTIR_14_EntregadosAlVolver.sql (Distral)
//    node sql/generar/entregado_al_volver.js                 (BD de server/.env)
//    node sql/generar/entregado_al_volver.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee sp_go_TC004_appMovil de ESA base (con el 13 instalado).
//  Problema: la app cuenta "Entregados hoy" con los pedidos entregados (oaest 3) cuya fechaHoraEntrega es de hoy. Ese
//  dato lo pone el celular al entregar. Al cerrar sesion y volver a entrar la app borra sus pedidos y los baja de nuevo
//  con el @24, que no traia esa fecha: los entregados del dia quedaban sin fecha y desaparecian de "Entregados hoy" y
//  de "Cobrado hoy", aunque siguen en la conciliacion.
//  Arreglo: el @24 devuelve la columna fechaHoraEntrega (fecha/hora del celular del ultimo evento de entrega o
//  anulacion en VentasRegistroMovil, script 07, mismo formato que guarda la app: yyyy-MM-dd HH:mm:ss).
//  La app (3.2 en adelante) ya tiene ese campo: no hace falta actualizarla ni tocar la API (pasa las columnas tal cual).
//  La app vieja ignora la columna.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'sp_go_TC004_appMovil';
const MARCA = 'Sanbit: entregados al volver';

const COLUMNA = [
    `\t\t\t-- ===== ${MARCA} (14): fecha/hora del celular de la entrega (o anulacion) =====`,
    "\t\t\t,(SELECT TOP 1 CONVERT(varchar(19), ISNULL(m.FechaHoraCelular, m.FechaHoraServidor), 120) FROM VentasRegistroMovil AS m",
    "\t\t\t  WHERE m.VentaId = a.Id AND m.Evento <> 'PEDIDO' ORDER BY m.Id DESC) AS fechaHoraEntrega",
].join(NL);

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]); // hash del SP tal como esta ahora (con el 13)
    const d0 = def[SP];
    if (d0.includes(MARCA)) falla(`${SP} de ${bd} ya tiene el 14: no hace falta generar nada`);
    if (!d0.includes('Sanbit: entrega sin cambios')) falla(`${SP} de ${bd} NO tiene el 13: instalar primero 01..13 y generar despues`);
    const tabla = await MetodoDatos.EjecutarConsulta("SELECT OBJECT_ID('dbo.VentasRegistroMovil') AS id");
    if (!tabla?.[0]?.id) falla(`${bd} no tiene la tabla VentasRegistroMovil (script 07)`);

    // solo dentro del @24 (lista de pedidos del celular): cada SELECT termina su lista de columnas en "as incremento"
    const t = buscar(d0, /IF\s*@tipo\s*=\s*24\s*--\s*ListPedidos/i, 0, d0.length, '@tipo=24 ListPedidos');
    const f = buscar(d0, /IF\s*@tipo\s*=\s*25\b/i, t.fin, d0.length, '@tipo=25 (fin del @24)');
    const bloque = d0.slice(t.fin, f.idx);
    const re = /\bas\s+incremento\b/gi;
    const n = (bloque.match(re) || []).length;
    if (n !== 4) falla(`Se esperaban 4 SELECT con "as incremento" en el @24 y hay ${n}: revisar el SP a mano`);
    const nuevo = bloque.replace(re, (m) => `${m}${NL}${COLUMNA}`);
    const d = d0.slice(0, t.fin) + nuevo + d0.slice(f.idx);

    const hoy = new Date().toISOString().slice(0, 10);
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const yaEsta = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const cab = [
        '/* =====================================================================',
        '   14 - ENTREGADOS AL VOLVER: "ENTREGADOS HOY" NO SE PIERDE AL CERRAR SESION EN EL CELULAR',
        `   Cambia SOLO ${SP} @24 (lista de pedidos que baja el celular): agrega la columna fechaHoraEntrega`,
        '   (fecha/hora del celular de la ultima entrega o anulacion, tabla VentasRegistroMovil del script 07).',
        '   Antes, al salir y volver a entrar, el celular bajaba los entregados sin esa fecha y desaparecian de',
        '   "Entregados hoy" y "Cobrado hoy" aunque siguen en la conciliacion.',
        '   Todo lo demas del SP queda IDENTICO. La app 3.2+ ya lee el campo; la vieja lo ignora.',
        '',
        `   Generado desde ${bd} (${hoy}), con el 13 instalado. PROTECCION: solo se aplica si el SP es esa version`,
        '   (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da',
        '   error y no modifica nada (regenerar con: node sql/generar/entregado_al_volver.js).',
        '   REQUISITO: 01..13 instalados. No hace falta reiniciar la API ni actualizar la app.',
        '   VOLVER ATRAS: revertir\\REVERTIR_14_EntregadosAlVolver.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${yaEsta}`,
        'BEGIN',
        "\tRAISERROR('14 EntregadosAlVolver YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('14 EntregadosAlVolver: ${SP} es OTRA version (no la de ${bd} ${hoy} con el 13). Instalar antes 01..13. No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = ['SET NOEXEC OFF', 'GO', `IF ${yaEsta}`, "\tRAISERROR('14 EntregadosAlVolver INSTALADO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r1 = escribir(bd, '14_EntregadosAlVolver.sql', `${cab}${NL}${aAlter(d)}${NL}GO${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 14_EntregadosAlVolver.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version con el 13 (el @24 deja de mandar fechaHoraEntrega).`,
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${yaEsta})`,
        'BEGIN',
        "\tRAISERROR('14 EntregadosAlVolver NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        `${aAlter(d0)}${NL}GO`,
        'SET NOEXEC OFF', 'GO',
        "RAISERROR('14 EntregadosAlVolver DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_14_EntregadosAlVolver.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
