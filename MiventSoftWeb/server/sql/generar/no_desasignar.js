// =====================================================================
//  Genera 11_NoDesasignarDespachado.sql y revertir/REVERTIR_11_NoDesasignarDespachado.sql (Distral)
//    node sql/generar/no_desasignar.js                 (BD de server/.env)
//    node sql/generar/no_desasignar.js --bd=OtraBD     (otra BD del mismo servidor)
//  Lee MAM_Ventas de ESA base (con el 04 instalado). Un pedido que ya entro a un despacho automatico
//  (ConciliacionAutomaticoPedido) NO se puede desasignar, anular desde Asignaciones ni eliminar mientras ese
//  despacho exista. Para hacerlo: eliminar primero el despacho (MAM_DespachoProductos @-1 borra el enlace).
//  Motivo: al desasignar, el pedido quedaba en el camion del repartidor; el celular lo bajaba (sp @24 lee
//  ConciliacionAutomaticoPedido) y lo marcaba "por subir" como si fuera suyo.
//  Lo valida el SP: aplica igual a la web, al escritorio (VB) y a cualquier otro que use MAM_Ventas.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'MAM_Ventas';
const MARCA = 'Sanbit: no desasignar despachados';

// La validacion va ANTES del BEGIN TRY de cada @tipo: sus CATCH solo anotan en Bitacora y no avisan al usuario
// Solo cuentan los despachos de conciliaciones ABIERTAS (las que baja el celular, sp @24): un pedido que volvio sin
// entregar de una conciliacion ya cerrada se puede desasignar / reasignar normalmente
const ABIERTA = 'INNER JOIN DespachoProductos AS dp ON dp.Id = cap.DespachoId INNER JOIN Conciliaciones AS cc ON cc.Id = dp.ConciliacionId AND cc.Estado = 1';
function bloqueLote(tipo, accion) {
    const v = `@Desp_T${tipo}`;
    const join = `FROM @Asignacion AS a INNER JOIN ConciliacionAutomaticoPedido AS cap ON cap.ventaId = a.Id ${ABIERTA} WHERE a.Asignar = 1`;
    return [
        `\t\t-- ${MARCA} (11): un pedido que ya esta en un despacho automatico no se toca hasta eliminar ese despacho`,
        `\t\tIF EXISTS (SELECT 1 ${join})`,
        '\t\tBEGIN',
        `\t\t\tDECLARE ${v} nvarchar(1500) = STUFF((SELECT TOP 10 CONCAT(', #', cap.ventaId, ' (despacho ', cap.DespachoId, ')')`,
        `\t\t\t\t${join} ORDER BY cap.ventaId FOR XML PATH(''), TYPE).value('.', 'nvarchar(1500)'), 1, 2, '')`,
        `\t\t\tRAISERROR('NO SE PUEDE ${accion}: los pedidos %s ya estan en un despacho de una conciliacion abierta. Elimine primero ese despacho.', 16, 1, ${v})`,
        '\t\t\tRETURN',
        '\t\tEND',
        '',
    ].join(NL);
}
function bloqueEliminar() {
    return [
        `\t\t-- ${MARCA} (11): un pedido que ya esta en un despacho automatico no se elimina hasta eliminar ese despacho`,
        `\t\tIF EXISTS (SELECT 1 FROM ConciliacionAutomaticoPedido AS cap ${ABIERTA} WHERE cap.ventaId = @Id)`,
        '\t\tBEGIN',
        `\t\t\tDECLARE @Desp_TM1 int = (SELECT TOP 1 cap.DespachoId FROM ConciliacionAutomaticoPedido AS cap ${ABIERTA} WHERE cap.ventaId = @Id ORDER BY cap.DespachoId DESC)`,
        "\t\t\tRAISERROR('NO SE PUEDE ELIMINAR EL PEDIDO #%d: ya esta en el despacho %d de una conciliacion abierta. Elimine primero ese despacho.', 16, 1, @Id, @Desp_TM1)",
        '\t\t\tRETURN',
        '\t\tEND',
        '',
    ].join(NL);
}

/** Inserta el bloque al inicio del @tipo (antes de su primer BEGIN TRY) */
function antesDelTry(s, reTipo, nombre, bloque) {
    const t = buscar(s, reTipo, 0, s.length, nombre);
    const tr = buscar(s, /BEGIN\s+TRY/i, t.fin, Math.min(s.length, t.fin + 400), `BEGIN TRY de ${nombre}`);
    const ini = s.lastIndexOf('\n', tr.idx) + 1; // inicio de la linea del BEGIN TRY
    return s.slice(0, ini) + bloque + s.slice(ini);
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    const d0 = def[SP];
    if (d0.includes(MARCA)) falla(`${SP} de ${bd} ya tiene el 11: no hace falta generar nada`);
    if (!d0.includes('STOCK INSUFICIENTE')) falla(`${SP} de ${bd} NO tiene el 04 (StockSeguro): instalar primero 01..05 y generar despues`);

    // De atras hacia adelante para que los indices no se muevan
    let d = d0;
    d = antesDelTry(d, /IF\s*@tipo\s*=\s*13\s*--\s*Anular\s+Pedidos/i, '@tipo=13 Anular Pedidos', bloqueLote(13, 'ANULAR'));
    d = antesDelTry(d, /IF\s*@tipo\s*=\s*12\s*--\s*Eliminar\s+Asignados/i, '@tipo=12 Eliminar Asignados', bloqueLote(12, 'QUITAR LA ASIGNACION'));
    d = antesDelTry(d, /IF\s*@tipo\s*=\s*-1\s*--\s*ELIMINAR\s+REGISTRO/i, '@tipo=-1 Eliminar', bloqueEliminar());
    if ((d.match(new RegExp(MARCA, 'g')) || []).length !== 3) falla('No se pudieron ubicar los 3 lugares (@-1, @12, @13)');

    const hoy = new Date().toISOString().slice(0, 10);
    const OPCIONES = ['SET QUOTED_IDENTIFIER ON', 'GO', 'SET ANSI_NULLS ON', 'GO'].join(NL);
    const yaEsta = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const cab = [
        '/* =====================================================================',
        '   11 - NO DESASIGNAR PEDIDOS QUE YA ESTAN EN UN DESPACHO AUTOMATICO',
        `   Cambia SOLO ${SP}: al inicio de @12 (Quitar asignacion), @13 (Anular pedidos) y @-1 (Eliminar pedido)`,
        '   valida que el pedido NO este en un despacho automatico de una conciliacion ABIERTA. Si esta, no hace nada y avisa:',
        '     "NO SE PUEDE QUITAR LA ASIGNACION: los pedidos #48389 (despacho 1241) ... Elimine primero ese despacho."',
        '   Los de conciliaciones cerradas (volvieron sin entregar) se desasignan normalmente.',
        '   Para desasignar hay que eliminar antes el despacho (eso borra el enlace). Vale para web, escritorio y',
        '   cualquier programa que use el SP. Todo lo demas del SP queda IDENTICO.',
        '',
        `   Generado desde ${bd} (${hoy}), con el 04 instalado. PROTECCION: solo se aplica si el SP es esa version`,
        '   (sin contar mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da',
        '   error y no modifica nada (regenerar con: node sql/generar/no_desasignar.js).',
        '   REQUISITO: 04_StockSeguro.sql instalado. No hace falta reiniciar la web ni la API.',
        '   VOLVER ATRAS: revertir\\REVERTIR_11_NoDesasignarDespachado.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${yaEsta}`,
        'BEGIN',
        "\tRAISERROR('11 NoDesasignarDespachado YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('11 NoDesasignarDespachado: ${SP} es OTRA version (no la de ${bd} ${hoy} con el 04). No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
    ].join(NL);
    const fin = ['SET NOEXEC OFF', 'GO', `IF ${yaEsta}`, "\tRAISERROR('11 NoDesasignarDespachado INSTALADO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r1 = escribir(bd, '11_NoDesasignarDespachado.sql', `${cab}${NL}${aAlter(d)}${NL}GO${NL}${fin}${NL}`);
    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 11_NoDesasignarDespachado.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version con el 04 (se puede desasignar aunque el pedido este despachado).`,
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${yaEsta})`,
        'BEGIN',
        "\tRAISERROR('11 NoDesasignarDespachado NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        OPCIONES,
        `${aAlter(d0)}${NL}GO`,
        'SET NOEXEC OFF', 'GO',
        "RAISERROR('11 NoDesasignarDespachado DESHECHO.', 10, 1) WITH NOWAIT", 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_11_NoDesasignarDespachado.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
