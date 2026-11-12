// =====================================================================
//  Verificacion contra TU base de datos (solo lectura, no modifica nada)
//    node test/verificar.js
//  Revisa: conexion, que existan los SP y tablas que usa la web, y la
//  estructura de los tipos tabla (TVP) que reciben.
// =====================================================================
require('dotenv').config({ path: require('node:path').join(__dirname, '..', '.env') });
const sql = require('mssql');
const Configuracion = require('../src/datos/Configuracion');

const SPS = ['MAM_Clasificadores', 'MAM_Categorias', 'MAM_Empresas', 'MAM_Usuarios', 'MAM_Roles', 'MAM_Clientes', 'sp_Mam_Zonas', 'MAM_Productos', 'sp_Mam_Precios',
    // Paso 3: modulo Ventas
    'MAM_Personal', 'MAM_Proveedores', 'MAM_Ventas', 'MAM_Movimientos', 'MAM_VentasIncremento', 'MAM_ReporteVentas', 'MAM_CreditosVentas', 'MAM_CreditosCompras',
    // Kaily: Ventas exige Caja Chica abierta
    'MAM_CierreCajero'];
// Parametros que la web envia a cada SP (los mismos que AccesoLogica.vb, ver test/contrato_vb.json)
const CONTRATO = require('./contrato_vb.json');
const SP_PARAMETROS = ['MAM_Personal', 'MAM_Proveedores', 'MAM_Ventas', 'MAM_DespachoProductos', 'MAM_Conciliacion', 'MAM_CajaIngresoEgreso', 'MAM_CierreCaja', 'MAM_AuditoriaVentas', 'MAM_Movimientos', 'MAM_Compras', 'MAM_CreditosCompras', 'MAM_Almacenes', 'MAM_ConceptosFijos', 'MAM_Contratos', 'MAM_Planilla'];
const TABLAS = ['Usuarios', 'RolesDetalles', 'Programas'];
const TVP = { MAM_Roles: ['@RolDetalleType', 10], sp_Mam_Zonas: ['@ZonasPuntos', 5], MAM_Productos: ['@TCL0064', 5], sp_Mam_Precios: ['@TY007', null] };

const OK = '  [OK]   ';
const MAL = '  [FALLA]';
let fallas = 0;

(async () => {
    let pool;
    try {
        const cfg = Configuracion.CadenaConexion;
        console.log(`\nConectando a ${cfg.server}${cfg.options.instanceName ? '\\' + cfg.options.instanceName : ''} / BD ${cfg.database} ...`);
        pool = await new sql.ConnectionPool(cfg).connect();
        const v = await pool.request().query('SELECT @@VERSION AS v, DB_NAME() AS bd');
        console.log(`${OK} Conexion correcta a ${v.recordset[0].bd}`);
        console.log(`         ${String(v.recordset[0].v).split('\n')[0]}`);
    } catch (e) {
        console.log(`${MAL} No se pudo conectar: ${e.message}`);
        console.log('\n  Revise:');
        console.log('   - Que SQL Server acepte conexiones TCP/IP (SQL Server Configuration Manager > Protocolos > TCP/IP = Habilitado).');
        console.log('   - Si usa instancia con nombre (EQUIPO\\SQLEXPRESS), que el servicio "SQL Server Browser" este iniciado.');
        console.log('   - Usuario/clave/BD en server\\.env (generado desde dbConnect.TXT).');
        process.exit(1);
    }

    console.log('\nProcedimientos almacenados:');
    for (const sp of SPS) {
        const r = await pool.request().input('n', sp).query('SELECT OBJECT_ID(@n) AS id');
        if (r.recordset[0].id) console.log(`${OK} ${sp}`);
        else { console.log(`${MAL} ${sp} no existe`); fallas++; }
    }

    console.log('\nTablas leidas directamente (login y privilegios):');
    for (const t of TABLAS) {
        const r = await pool.request().input('n', t).query("SELECT OBJECT_ID(@n, 'U') AS id");
        if (r.recordset[0].id) console.log(`${OK} ${t}`);
        else { console.log(`${MAL} ${t} no existe`); fallas++; }
    }

    console.log('\nTipos tabla (TVP) que la web envia a los SP:');
    for (const [sp, [param, esperado]] of Object.entries(TVP)) {
        // Compatible con SQL Server 2008+ (sin STRING_AGG)
        const r = await pool.request().input('sp', sp).input('p', param).query(`
            SELECT tt.name AS tipo, c.name AS col
            FROM sys.parameters p
            JOIN sys.table_types tt ON tt.user_type_id = p.user_type_id
            JOIN sys.columns c ON c.object_id = tt.type_table_object_id
            WHERE p.object_id = OBJECT_ID(@sp) AND p.name = @p
            ORDER BY c.column_id`).catch((e) => ({ recordset: [], error: e }));
        const filas = r.recordset;
        if (!filas.length) { console.log(`${MAL} ${sp} ${param}: no encontrado ${r.error ? '(' + r.error.message + ')' : ''}`); fallas++; continue; }
        const f = { tipo: filas[0].tipo, columnas: filas.length, nombres: filas.map((x) => x.col).join(', ') };
        const aviso = esperado && f.columnas !== esperado ? `  <- el escritorio envia ${esperado} columnas` : '';
        if (aviso) fallas++;
        console.log(`${aviso ? MAL : OK} ${sp} ${param} = ${f.tipo} (${f.columnas} col: ${f.nombres})${aviso}`);
    }

    console.log('\nParametros de los SP del modulo Ventas (los que envia la web):');
    for (const sp of SP_PARAMETROS) {
        const r = await pool.request().input('sp', sp).query('SELECT LOWER(name) AS n FROM sys.parameters WHERE object_id = OBJECT_ID(@sp)');
        const enSp = new Set(r.recordset.map((x) => x.n));
        const usados = new Set();
        for (const [k, lista] of Object.entries(CONTRATO)) if (k.startsWith(`${sp}|`)) lista.forEach((c) => c.params.forEach((x) => usados.add(x)));
        ['@tipo', '@id', '@usuario'].forEach((x) => usados.add(x)); // L_prListarGeneral / L_prBorrarRegistro
        const faltan = [...usados].filter((x) => !enSp.has(x));
        if (!enSp.size) { console.log(`${MAL} ${sp}: no existe`); fallas++; continue; }
        if (faltan.length) { console.log(`${MAL} ${sp}: el SP no tiene ${faltan.join(', ')}`); fallas++; }
        else console.log(`${OK} ${sp}: acepta los ${usados.size} parametros que envia la web`);
    }

    // VentaDetalleType: VB envia el DataTable del grid (columnas de MAM_Ventas @tipo 4) POR POSICION
    console.log('\nDetalle de ventas (@VentaDetalleType vs columnas de MAM_Ventas @tipo 4):');
    try {
        const t = await pool.request().input('sp', 'MAM_Ventas').query(`
            SELECT c.name AS col FROM sys.parameters p
            JOIN sys.table_types tt ON tt.user_type_id = p.user_type_id
            JOIN sys.columns c ON c.object_id = tt.type_table_object_id
            WHERE p.object_id = OBJECT_ID(@sp) AND p.name = '@VentaDetalleType' ORDER BY c.column_id`);
        const det = await pool.request().input('tipo', 4).input('usuario', 'VERIFICAR').input('id', -1).execute('MAM_Ventas'); // ListaVentasDetalles(-1): solo lectura
        const cols = Object.values(det.recordset.columns).sort((a, b) => a.index - b.index).map((c) => c.name);
        const tvp = t.recordset.map((x) => x.col);
        if (!tvp.length) { console.log(`${MAL} MAM_Ventas no tiene el parametro @VentaDetalleType`); fallas++; }
        else if (tvp.length !== cols.length) {
            console.log(`${MAL} el tipo tiene ${tvp.length} columnas y el detalle (@tipo 4) devuelve ${cols.length}: grabar una venta fallaria`);
            console.log(`         tipo:    ${tvp.join(', ')}`);
            console.log(`         detalle: ${cols.join(', ')}`);
            fallas++;
        } else {
            const distintas = tvp.map((c, i) => (c.toLowerCase() !== String(cols[i]).toLowerCase() ? `${i + 1}:${cols[i]}->${c}` : null)).filter(Boolean);
            console.log(`${OK} ${tvp.length} columnas en el mismo orden${distintas.length ? ` (nombres distintos, se envian por posicion igual que VB: ${distintas.join(' ')})` : ''}`);
        }
    } catch (e) { console.log(`${MAL} No se pudo comparar el detalle: ${e.message}`); fallas++; }

    // KAILY: SIN "Asignacion de pedidos" (era de Distral: MAM_Ventas 7 / 15 / 17).
    // En Kaily MAM_Ventas @tipo 7 es InsertarTipoCambio: ejecutarlo aqui GRABABA un tipo de cambio en 0
    // (usuario VERIFICAR) en cada arranque de iniciar.bat. Este script solo debe ejecutar @tipo de LECTURA.

    // Distribucion: tipos tabla vs las grillas que envia el escritorio (solo lectura)
    console.log('\nDistribucion (tipos tabla vs grillas, solo lectura):');
    const colsDe = async (sp, param) => (await pool.request().input('sp', sp).input('p', param).query(`
            SELECT c.name AS col FROM sys.parameters p
            JOIN sys.table_types tt ON tt.user_type_id = p.user_type_id
            JOIN sys.columns c ON c.object_id = tt.type_table_object_id
            WHERE p.object_id = OBJECT_ID(@sp) AND p.name = @p ORDER BY c.column_id`)).recordset.map((x) => x.col);
    const comparar = (nombre, tvp, cols) => {
        const porNombre = tvp.every((c) => cols.some((g) => g.toLowerCase() === c.toLowerCase()));
        if (!tvp.length) { console.log(`${MAL} ${nombre}: el SP no tiene ese parametro`); fallas++; }
        else if (tvp.length === cols.length || porNombre) console.log(`${OK} ${nombre}: ${tvp.length} columnas${tvp.length !== cols.length ? ` (la grilla trae ${cols.length}; se envian por nombre)` : ''}`);
        else { console.log(`${MAL} ${nombre}: tipo ${tvp.length} col (${tvp.join(', ')}) / grilla ${cols.length} col (${cols.join(', ')})`); fallas++; }
    };
    const colsSp = async (sp, tipo, extra) => {
        const rq = pool.request().input('tipo', tipo).input('usuario', 'VERIFICAR');
        Object.entries(extra).forEach(([k, x]) => rq.input(k, x));
        const r = await rq.execute(sp);
        return Object.values(r.recordset.columns).sort((a, b) => a.index - b.index).map((c) => c.name);
    };
    for (const [nombre, sp, param, tipo, extra] of [
        ['@DetalleDespacho vs detalle (@tipo 4)', 'MAM_DespachoProductos', '@DetalleDespacho', 4, { id: -1 }],
        ['@DetalleDespacho vs automatico (@tipo 7)', 'MAM_DespachoProductos', '@DetalleDespacho', 7, { PersonalId: -1, SucursalId: 1 }],
        ['@ReferenciaVenta vs referencia (@tipo 8)', 'MAM_DespachoProductos', '@ReferenciaVenta', 8, { PersonalId: -1 }],
        ['@DetalleGastos vs gastos del cierre (@tipo 4)', 'MAM_CierreCaja', '@DetalleGastos', 4, { id: -1 }],
        ['@Cobranzas vs cobranzas sin cierre (@tipo 7)', 'MAM_CierreCaja', '@Cobranzas', 7, { PersonalId: -1, FechaCierre: '1900/01/01' }],
        ['@Cobranzas vs cobranzas con cierre (@tipo 8)', 'MAM_CierreCaja', '@Cobranzas', 8, { id: -1 }],
        ['@detalle (movimientos) vs detalle (@tipo 4)', 'MAM_Movimientos', '@detalle', 4, { id: -1 }],
        ['@detalle (compras) vs detalle (@tipo 4)', 'MAM_Compras', '@detalle', 4, { id: -1 }],
        ['@DetalleContratoConcepto vs conceptos del contrato (@tipo 4)', 'MAM_Contratos', '@DetalleContratoConcepto', 4, { id: -1 }],
        ['@PlanillaConcepto vs conceptos de planilla (@tipo 4)', 'MAM_Planilla', '@PlanillaConcepto', 4, { Id: -1, TipoConcepto: -1 }],
        ['@Asignaciontype vs zonas del personal (@tipo 5)', 'sp_Mam_Zonas', '@Asignaciontype', 5, { idRepartidor: -1 }],
        // Kaily: formas de pago de la venta (ListaVentasDetallePago)
        ['@VentaPagos vs pagos de la venta (@tipo 8)', 'MAM_Ventas', '@VentaPagos', 8, { id: -1 }],
    ]) {
        try { comparar(nombre, await colsDe(sp, param), await colsSp(sp, tipo, extra)); } catch (e) { console.log(`${MAL} ${nombre}: ${e.message}`); fallas++; }
    }
    try {
        const t = await colsDe('MAM_Conciliacion', '@DetalleConciliacion');
        if (t.length === 5) console.log(`${OK} @DetalleConciliacion: 5 columnas (${t.join(', ')})`);
        else { console.log(`${MAL} @DetalleConciliacion: tiene ${t.length} columnas y el escritorio envia 5 (ProductoId, TotalSalida, TotalDevoluciones, TotalEntregado, Diferencia)`); fallas++; }
    } catch (e) { console.log(`${MAL} @DetalleConciliacion: ${e.message}`); fallas++; }

    console.log('\nConfiguracion de ventas (solo lectura):');
    try {
        const c = await pool.request().input('tipo', 6).input('usuario', 'VERIFICAR').execute('MAM_Movimientos'); // L_prLeerConfiguracion
        const fila = c.recordset[0] || {};
        const k = Object.keys(fila).find((x) => x.toLowerCase() === 'lote');
        const lote = k ? (fila[k] === true || Number(fila[k]) === 1) : false;
        console.log(lote ? `${MAL} El sistema usa LOTES: la venta por lotes aun no esta en la web (se avisa al grabar).` : `${OK} Ventas sin lotes (igual que la web)`);
        const inc = await pool.request().input('tipo', 1).input('usuario', 'VERIFICAR').execute('MAM_VentasIncremento').catch(() => null);
        console.log(inc ? `${OK} Incremento de venta de hoy: ${inc.recordset[0] ? Object.values(inc.recordset[0]).join(' / ') : 'no configurado (se usa 2.00 como el escritorio)'}` : `${MAL} MAM_VentasIncremento no responde`);
    } catch (e) { console.log(`${MAL} ${e.message}`); fallas++; }

    console.log('\nLectura de prueba (solo SELECT via SP):');
    try {
        const r = await pool.request().input('tipo', 3).input('IdClasificador', 1).input('usuario', 'VERIFICAR').execute('MAM_Clasificadores');
        console.log(`${OK} MAM_Clasificadores @tipo=3 -> ${r.recordset.length} modulos (${r.recordset.map((x) => x.cndesc1).join(', ')})`);
    } catch (e) { console.log(`${MAL} MAM_Clasificadores: ${e.message}`); fallas++; }

    await pool.close();
    console.log(fallas ? `\n${fallas} problema(s) encontrado(s). Revise lo marcado como [FALLA].\n` : '\nTodo correcto. La web puede usar su base de datos.\n');
    process.exit(fallas ? 2 : 0);
})();
