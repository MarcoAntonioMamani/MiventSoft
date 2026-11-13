// =====================================================================
//  PRUEBA INTEGRAL: ELIMINAR REGISTROS + AUDITORIA CONTRA LA BD REAL (SIN DEJAR RASTRO)
//    node test/probar_auditoria_bd.js            (pide el nombre de la base)
//    node test/probar_auditoria_bd.js <NombreBD> (sin preguntar)
//    --scripts=carpeta   donde estan 06_ y 08_ (por defecto sql/generados/<BD>; el 07 sale de sql/)
//
//  Todo dentro de UNA transaccion que al final se REVIERTE (ROLLBACK):
//    1. Aplica 06_EliminarRegistros, 07_Auditoria y 08_Auditoria_SPs, y los vuelve a ejecutar ("YA ESTA INSTALADO")
//    2. MAM_Usuarios: sin @EliminarRegistros (escritorio anterior) conserva el valor; insertar sin el = 0
//    3. Con las RUTAS REALES de la web: venta, compra e ingreso -> grabar, modificar, guardar sin cambios y eliminar
//       -> AuditoriaRegistros: una fila por modificacion y por eliminacion, con antes / despues
//    4. Pantalla de auditoria: lista, filtros y comparacion antes / despues
//    5. Usuario sin "Eliminar registros": la web rechaza el DELETE (403)
//    6. Scripts REVERTIR (08, 07, 06) dejan todo como estaba
//  Solo imprime Id, cantidades y totales. NO ejecutar en horario de atencion (bloquea los SP mientras corre).
// =====================================================================
require('dotenv').config({ path: require('node:path').join(__dirname, '..', '.env') });
const fs = require('node:fs');
const path = require('node:path');
const readline = require('node:readline/promises');
const express = require('express');
const sql = require('mssql');
const MetodoDatos = require('../src/datos/MetodoDatos');
const L = require('../src/negocio/AccesoLogica');
const { manejadorErrores, productosStock } = require('../src/middleware/errores');
const { LimpiarCachePermisos } = require('../src/middleware/permisos');
const { hoyBolivia } = require('../src/rutas/distribucion/comun');

const SUC = 1; const CAT = 3;
let fallas = 0;
const ok = (c, m) => { console.log(`${c ? '  [OK]   ' : '  [FALLA]'} ${m}`); if (!c) fallas++; return c; };
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const SQLDIR = path.join(__dirname, '..', 'sql');

(async () => {
    const pool = await MetodoDatos.CrearConexion();
    const info = (await pool.request().query('SELECT @@SERVERNAME AS s, DB_NAME() AS bd')).recordset[0];
    console.log(`\nServidor: ${info.s}   Base de datos: ${info.bd}`);
    let resp = process.argv.slice(2).find((a) => !a.startsWith('--'));
    if (!resp) {
        const rl = readline.createInterface({ input: process.stdin, output: process.stdout });
        resp = await rl.question(`Todo se revierte al final. Para continuar escriba el nombre de la base (${info.bd}): `);
        rl.close();
    }
    if (String(resp).trim().toLowerCase() !== String(info.bd).toLowerCase()) { console.log('Cancelado.'); process.exit(1); }
    const dirGen = (process.argv.find((a) => a.startsWith('--scripts=')) || '').slice(10) || path.join(SQLDIR, 'generados', info.bd);
    const archivo = (n) => { const r = [path.join(dirGen, n), path.join(SQLDIR, n)].find((x) => fs.existsSync(x)); if (!r) throw new Error(`No existe ${n}`); return r; };
    const estadoFuera = async () => (await pool.request().query(`SELECT COL_LENGTH('dbo.Usuarios','EliminarRegistros') col, OBJECT_ID('dbo.AuditoriaRegistros') tabla,
        OBJECT_ID('dbo.MAM_Auditoria') sp, CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas')) LIKE '%Kaily: auditoria%' THEN 1 ELSE 0 END marca`)).recordset[0];
    const inicio = await estadoFuera();
    if (inicio.col || inicio.marca) console.log('  (la BD ya tiene instalado algo de esto: los scripts deben avisar "YA ESTA INSTALADO")');

    // Todas las llamadas por la transaccion, una a la vez
    const tx = new sql.Transaction(pool);
    await tx.begin();
    let cola = Promise.resolve();
    const enCola = (fn) => { const p = cola.then(fn, fn); cola = p.catch(() => {}); return p; };
    let mensajes = [];
    class RequestTx extends sql.Request {
        constructor() { super(tx); this.on('info', (m) => mensajes.push(m.message)); }
        execute(...a) { return enCola(() => super.execute(...a)); }
        query(...a) { return enCola(() => super.query(...a)); }
        batch(...a) { return enCola(() => super.batch(...a)); } // lote SQL como SSMS (no sp_executesql)
    }
    const requestOriginal = pool.request.bind(pool);
    pool.request = () => new RequestTx();
    const q = async (s) => (await new RequestTx().query(s)).recordset;
    const correr = async (n) => {
        mensajes = [];
        const lotes = fs.readFileSync(archivo(n), 'utf8').split(/^\s*GO\s*$/im).map((x) => x.trim()).filter(Boolean);
        for (const l of lotes) await new RequestTx().batch(l);
        return mensajes.join(' | ');
    };

    const personal = num((await L.ListarPersonal())[0]?.Id);
    const admin = (await q('SELECT TOP 1 Id FROM Usuarios WHERE RolId = 1 ORDER BY Id'))[0];
    const USR = 'PRUEBA WEB AUDITORIA'; // mas de 10 letras: prueba el 11
    const sesion = { gs_user: USR, gi_userNumi: num(admin?.Id), gi_userRol: 1, gi_userSuc: -1, Global_IdPersonal: personal,
        Global_ModificarPrecio: 0, Global_ModificarDescuento: 0, Global_VentasTodos: 1, Global_CierreTodos: 1, Global_ModificarRegistro: 1, Global_NombreSucursal: '' };
    const app = express();
    app.use(express.json({ limit: '5mb' }));
    app.use(require('../src/middleware/clienteAuditoria').clienteAuditoria);
    app.use((req, res, next) => { req.sesion = sesion; L.L_prConUsuario(sesion.gs_user, () => next(), req.clienteAud); });
    app.use('/ventas', require('../src/rutas/ventas/ventasPuntoVenta.routes'));
    app.use('/compras', require('../src/rutas/compras/compras.routes'));
    app.use('/mov', require('../src/rutas/inventario/movimientos.routes'));
    app.use('/caja', require('../src/rutas/caja/cierreCajero.routes'));
    app.use('/auditoria', require('../src/rutas/configuracion/auditoria.routes'));
    app.use(manejadorErrores);
    const srv = app.listen(0);
    const URL = `http://127.0.0.1:${srv.address().port}`;
    const api = async (metodo, ruta, cuerpo) => {
        // como la APK en un celular: nombre y codigo del equipo + GPS (sql/10)
        const headers = { 'Content-Type': 'application/json', 'X-Equipo-Id': 'A1B2C3', 'X-Equipo-Nombre': encodeURIComponent('CAJA PRUEBA'), 'X-Ubicacion': '-16.500123456789012345,-68.150456789012345678,12',
            'User-Agent': 'Mozilla/5.0 (Linux; Android 13; SM-A135F Build/TP1A; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/131.0 Mobile Safari/537.36' };
        const r = await fetch(URL + ruta, { method: metodo, headers, body: cuerpo === undefined ? undefined : JSON.stringify(cuerpo) });
        return { status: r.status, ...(await r.json()) };
    };
    const okApi = async (...a) => { const r = await api(...a); if (!r.ok) throw new Error(`${a[0]} ${a[1]} -> ${r.status} ${r.mensaje}`); return r; };
    const auditoria = async (modulo, doc) => q(`SELECT Id, Operacion, Usuario, Aplicacion, TotalAntes, TotalDespues, FechaDocumento,
        CAST(Antes AS nvarchar(max)) Antes, CAST(Despues AS nvarchar(max)) Despues FROM AuditoriaRegistros WHERE Modulo = ${modulo} AND DocumentoId = ${doc} ORDER BY Id`);
    const lineas = (xml) => (String(xml || '').match(/<det /g) || []).length;

    try {
        await L.L_prConUsuario(USR, async () => {
            const hoy = hoyBolivia();
            // ---------------- 1. scripts ----------------
            console.log('\n1. Scripts dentro de la transaccion');
            if (!inicio.sp) { // orden equivocado: el 08 sin el 07 no debe tocar nada
                let error = '';
                try { await correr('08_Auditoria_SPs.sql'); } catch (e) { error = e.message; }
                await new RequestTx().batch('SET NOEXEC OFF');
                const m = (await q("SELECT CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas')) LIKE '%Kaily: auditoria%' THEN 1 ELSE 0 END m"))[0].m;
                ok(/falta ejecutar primero 07/i.test(error) && m === 0, `08 antes que 07: "${error}" y no modifico nada`);
            }
            for (const n of ['06_EliminarRegistros.sql', '07_Auditoria.sql', '08_Auditoria_SPs.sql', '09_CompraEliminarStock.sql', '10_AuditoriaEquipo.sql', '11_AuditoriaUsuario.sql', '12_AuditoriaUbicacionPrecisa.sql']) {
                const m = await correr(n);
                ok(!/fallo|OTRA version|falta ejecutar/i.test(m), `${n}: ${m}`);
            }
            for (const n of ['06_EliminarRegistros.sql', '07_Auditoria.sql', '08_Auditoria_SPs.sql', '09_CompraEliminarStock.sql', '10_AuditoriaEquipo.sql', '11_AuditoriaUsuario.sql', '12_AuditoriaUbicacionPrecisa.sql']) {
                const m = await correr(n);
                ok(/YA ESTA INSTALADO|ya existia|10 Auditoria equipo INSTALADO|version del 11 \(se conserva\)/i.test(m) && !/fallo/i.test(m), `${n} otra vez: ${m}`);
            }
            const marcas = (await q(`SELECT SUM(CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.' + n)) LIKE '%Kaily: auditoria%' THEN 1 ELSE 0 END) m
                FROM (VALUES ('MAM_Ventas'), ('MAM_Compras'), ('MAM_Movimientos')) v(n)`))[0].m;
            ok(marcas === 3, `los 3 SP llaman a MAM_Auditoria (${marcas})`);
            const prog = (await q("SELECT p.Id, (SELECT COUNT(*) FROM RolesDetalles d WHERE d.ProgramaId = p.Id AND d.Ver = 1) ver FROM Programas p WHERE p.IdPrograma = 'btnAuditoriaRegistros'"))[0];
            ok(prog && prog.ver >= 1, `programa btnAuditoriaRegistros creado y asignado (${prog?.ver} rol/es)`);
            LimpiarCachePermisos();

            // ---------------- 2. MAM_Usuarios ----------------
            console.log('\n2. Usuarios.EliminarRegistros');
            const sinNull = (await q('SELECT COUNT(*) n FROM Usuarios WHERE EliminarRegistros IS NULL'))[0].n;
            ok(sinNull === 0, 'los usuarios que ya existian quedan con valor (0 = sin permiso)');
            await q(`UPDATE Usuarios SET EliminarRegistros = 1 WHERE Id = ${sesion.gi_userNumi}`); // el de la prueba puede eliminar
            LimpiarCachePermisos();
            const u = (await L.L_prUsuarioGeneral()).find((x) => num(x.Id) === sesion.gi_userNumi);
            ok(u && u.EliminarRegistros !== undefined, '@tipo 3 devuelve EliminarRegistros');
            const r2 = new RequestTx();
            for (const [k, x] of Object.entries({ tipo: 2, Id: u.Id, RolId: u.RolId, NombreUsuario: u.NombreUsuario, Contrasena: u.Contrasena, Estado: u.estado ? 1 : 0,
                SucursalId: u.SucursalId, IdEmpresa: u.IdEmpresa, usuario: 'PRUEBA', IdPersonal: u.IdPersonal, ModificarPrecio: u.ModificarPrecioVenta,
                ModificarDescuento: u.AplicarDescuentoVenta, VentasTodasVentas: u.VerTodasVentas, VerTodasVentasCierre: u.VerTodosVentasCierre,
                ModificarRegistroAntiguos: u.ModificarRegistroAnteriores })) r2.input(k, x);
            await r2.execute('MAM_Usuarios'); // como el escritorio anterior: sin @EliminarRegistros
            ok(num((await q(`SELECT EliminarRegistros e FROM Usuarios WHERE Id = ${u.Id}`))[0].e) === 1, 'modificar sin @EliminarRegistros (escritorio anterior) conserva el valor');

            // ---------------- 3. operaciones ----------------
            console.log('\n3. Venta, compra e ingreso: grabar, modificar, guardar sin cambios y eliminar');
            const caja = await L.L_prListarGeneral('MAM_CierreCajero');
            const abierta = caja.some((f) => num(f.SucursalId) === SUC && num(f.EstadoCaja) === 1 && new Date(f.Fecha).toISOString().slice(0, 10) === hoy);
            if (!abierta) await okApi('POST', '/caja', { SucursalId: SUC, MontoInicial: 0, TipoCambio: 6.96, Observacion: 'PRUEBA (se revierte)' });
            const prods = (await okApi('GET', `/ventas/productos?sucursal=${SUC}&precio=${CAT}`)).data.filter((p) => p.Tipo === 1 && p.stock >= 5 && num(p.PrecioVenta) > 0);
            if (prods.length < 2) throw new Error('Se necesitan 2 productos con stock >= 5 y precio en la sucursal 1');
            const [A, B] = [prods[0].Id, prods[1].Id];
            const cliente = num((await L.ListarCliente())[0]?.Id);
            const cabV = { SucursalId: SUC, FechaVenta: hoy, ClienteId: cliente, TipoVenta: 1, Glosa: 'PRUEBA AUDITORIA (se revierte)', PrecioId: CAT, Descuento: 0 };
            const v1 = await okApi('POST', '/ventas', { ...cabV, detalle: [{ nuevo: true, Tipo: 1, ProductoId: A, PrecioId: CAT, Cantidad: 2 }, { nuevo: true, Tipo: 1, ProductoId: B, PrecioId: CAT, Cantidad: 1 }], pago: { MontoBs: 999999 } });
            ok((await auditoria(1, v1.id)).length === 0, `venta ${v1.id} nueva: sin auditoria (solo se audita modificar y eliminar)`);
            const det = (await okApi('GET', `/ventas/${v1.id}/detalle`)).data;
            const filas = Array.isArray(det) ? det : det.detalle || det.filas || [];
            const dA = filas.find((f) => num(f.ProductoId) === A);
            await okApi('PUT', `/ventas/${v1.id}`, { ...cabV, fechaOriginal: hoy, detalle: [{ Id: num(dA.Id), Cantidad: 1 }], pago: { MontoBs: 999999 } });
            let a = await auditoria(1, v1.id);
            ok(a.length === 1 && a[0].Operacion === 1, 'venta modificada: 1 registro "Modificado"');
            ok(lineas(a[0]?.Antes) === 2 && lineas(a[0]?.Despues) === 1, `antes 2 lineas, despues 1 (${lineas(a[0]?.Antes)} / ${lineas(a[0]?.Despues)})`);
            ok(a[0]?.Usuario === USR && /MiventSoftWeb/.test(a[0]?.Aplicacion), `usuario ${a[0]?.Usuario}, aplicacion ${a[0]?.Aplicacion}`);
            ok(num(a[0]?.TotalAntes) > num(a[0]?.TotalDespues) && a[0]?.FechaDocumento, `total ${a[0]?.TotalAntes} -> ${a[0]?.TotalDespues}`);
            const det2 = (await okApi('GET', `/ventas/${v1.id}/detalle`)).data;
            const filas2 = Array.isArray(det2) ? det2 : det2.detalle || det2.filas || [];
            await okApi('PUT', `/ventas/${v1.id}`, { ...cabV, fechaOriginal: hoy, detalle: filas2.map((f) => ({ Id: num(f.Id), Cantidad: num(f.Cantidad) })), pago: { MontoBs: 999999 } });
            ok((await auditoria(1, v1.id)).length === 1, 'guardar la venta sin cambiar nada no agrega registro');
            const audVenta = a[0].Id;
            await okApi('DELETE', `/ventas/${v1.id}?fecha=${hoy}`);
            a = await auditoria(1, v1.id);
            ok(a.length === 2 && a[1].Operacion === 2 && lineas(a[1].Antes) === 1 && !a[1].Despues, 'venta eliminada: registro "Eliminado" con el detalle que tenia (1 linea)');

            const prov = num((await L.ListarProveedores())[0]?.Id);
            const cabC = { AlmacenId: SUC, ProveedorId: prov, TipoVenta: 1, FechaTransaccion: hoy, Glosa: 'PRUEBA AUDITORIA (se revierte)', Descuento: 0 };
            const c1 = await okApi('POST', '/compras', { ...cabC, detalle: [{ ProductoId: A, CantidadCompra: 5 }] });
            const dc = (await okApi('GET', `/compras/${c1.id}/detalle`)).data[0];
            await okApi('PUT', `/compras/${c1.id}`, { ...cabC, fechaOriginal: hoy, detalle: [{ Id: dc.Id, CantidadCompra: 3, PrecioCosto: dc.PrecioCosto, PrecioVenta: dc.PrecioVenta }] });
            await okApi('DELETE', `/compras/${c1.id}?fecha=${hoy}`);
            a = await auditoria(2, c1.id);
            ok(a.length === 2 && a[0].Operacion === 1 && a[1].Operacion === 2, `compra ${c1.id}: Modificado + Eliminado (${a.map((x) => x.Operacion).join(',')})`);
            ok(/Cantidad="5/.test(a[0]?.Antes) && /Cantidad="3/.test(a[0]?.Despues), 'compra: antes cantidad 5, despues 3');
            ok(a.every((x) => x.Usuario === USR), `11: compra con el usuario completo (${a.map((x) => x.Usuario).join(', ')})`);
            // 10: desde donde (equipo, IP, navegador, GPS)
            const dondeC = (await q(`SELECT Equipo, Ip, Navegador, CONVERT(varchar(40), Latitud) Latitud, CONVERT(varchar(40), Longitud) Longitud, PrecisionMetros FROM AuditoriaRegistros WHERE Id = ${a[1]?.Id}`))[0];
            // misma PC del servidor: nombre real de la PC; con el nombre dado: "CAJA PRUEBA (PC)"
            ok(dondeC?.Equipo === `CAJA PRUEBA (${require('node:os').hostname().toUpperCase()})` && /^127\.0\.0\.1( \| [\d.:a-f]+)?$/i.test(dondeC?.Ip) && /App Sanbit · Android 13 · SM-A135F/.test(dondeC?.Navegador)
                && dondeC?.Latitud === '-16.500123456789012345' && dondeC?.Longitud === '-68.150456789012345678' && num(dondeC?.PrecisionMetros) === 12,
                `compra eliminada desde: ${JSON.stringify(dondeC)}`);
            const ctx = (await q("SELECT COUNT(*) n FROM AuditoriaContexto WHERE Spid = @@SPID"))[0].n;
            ok(ctx === 0, 'AuditoriaContexto queda vacio despues de cada operacion');

            // 09: compra cuyo ingreso ya se vendio -> no se elimina (el stock quedaria negativo)
            const c2 = await okApi('POST', '/compras', { ...cabC, detalle: [{ ProductoId: A, CantidadCompra: 5 }] });
            const dep = num((await q(`SELECT DepositoId d FROM Movimientos WHERE ConceptoId = 2 AND TransaccionId = ${c2.id}`))[0]?.d);
            const stockA = async () => num((await q(`SELECT SUM(Cantidad) s FROM ProductosStock WHERE ProductoId = ${A} AND DepositoId = ${dep}`))[0]?.s);
            const stockOriginal = (await stockA()) - 5;
            const audC2 = (await auditoria(2, c2.id)).length; // el Id puede repetir el de la compra eliminada arriba
            await q(`UPDATE ProductosStock SET Cantidad = 0 WHERE ProductoId = ${A} AND DepositoId = ${dep}`); // como si se hubiera vendido todo
            let rc = await api('DELETE', `/compras/${c2.id}?fecha=${hoy}`);
            const sn = rc.datos?.stockNegativo || [];
            ok(rc.status === 409 && sn.length === 1 && sn[0].stock === 0 && sn[0].cantidad === 5 && sn[0].quedaria === -5,
                `DELETE compra ya vendida rechazado: ${rc.status} ${rc.mensaje} ${JSON.stringify(sn)}`);
            const sigue = (await q(`SELECT (SELECT COUNT(*) FROM Compras WHERE Id = ${c2.id}) c, (SELECT COUNT(*) FROM CompraDetalle WHERE CompraId = ${c2.id}) d,
                (SELECT COUNT(*) FROM Movimientos WHERE ConceptoId = 2 AND TransaccionId = ${c2.id}) m`))[0];
            ok(sigue.c === 1 && sigue.d === 1 && sigue.m === 1 && (await stockA()) === 0, 'la compra sigue completa y el stock no cambio (0)');
            ok((await auditoria(2, c2.id)).length === audC2, 'no se registro como eliminada');
            await q(`UPDATE TOP (1) ProductosStock SET Cantidad = 5 WHERE ProductoId = ${A} AND DepositoId = ${dep}`); // queda en 0 al eliminar: se permite
            rc = await api('DELETE', `/compras/${c2.id}?fecha=${hoy}`);
            ok(rc.ok && (await stockA()) === 0 && !(await q(`SELECT 1 x FROM Compras WHERE Id = ${c2.id}`)).length, `con stock 5 si se elimina y queda en 0 (${rc.status} ${rc.mensaje || ''})`);
            await q(`UPDATE TOP (1) ProductosStock SET Cantidad = ${stockOriginal} WHERE ProductoId = ${A} AND DepositoId = ${dep}`); // stock como estaba

            const cabM = { DepositoId: SUC, FechaDocumento: hoy, Observacion: 'PRUEBA AUDITORIA (se revierte)' };
            const m1 = await okApi('POST', '/mov', { ...cabM, ConceptoId: 4, detalle: [{ ProductoId: A, Cantidad: 4 }] });
            const dm = (await okApi('GET', `/mov/${m1.id}/detalle`)).data[0];
            await okApi('PUT', `/mov/${m1.id}`, { ...cabM, ConceptoId: 4, detalle: [{ Id: dm.Id, Cantidad: 2 }] });
            await okApi('DELETE', `/mov/${m1.id}`);
            a = await auditoria(3, m1.id);
            ok(a.length === 2 && num(a[0]?.TotalAntes) === 4 && num(a[0]?.TotalDespues) === 2 && a[1]?.Operacion === 2, `ingreso ${m1.id}: Modificado (4 -> 2) + Eliminado`);
            ok(a.every((x) => x.Usuario === USR), `11: ingreso con el usuario completo (${a.map((x) => x.Usuario).join(', ')})`);
            // 11 escritorio y registros anteriores: usuario cortado a 10 -> nombre completo si en Usuarios hay uno solo que empieza asi
            const largo = (await q(`SELECT TOP 1 k.n FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS k WHERE LEN(k.n) > 10 AND (SELECT COUNT(DISTINCT j.n) FROM (SELECT x.NombreUsuario AS n FROM Usuarios AS x UNION SELECT p.NombrePersonal FROM Usuarios AS x INNER JOIN Personal AS p ON p.Id = x.IdPersonal) AS j WHERE LEFT(j.n, 10) = LEFT(k.n, 10)) = 1 ORDER BY k.n`))[0]?.n;
            if (largo) {
                const corto = largo.slice(0, 10);
                const rq = new RequestTx(); rq.input('u', corto);
                await rq.query(`EXEC MAM_Auditoria @tipo=2, @Modulo=3, @Operacion=2, @DocumentoId=${m1.id}, @usuario=@u`);
                const g = (await q(`SELECT TOP 1 Id, Usuario FROM AuditoriaRegistros WHERE Modulo = 3 AND DocumentoId = ${m1.id} ORDER BY Id DESC`))[0];
                ok(g?.Usuario === largo, `escritorio con "${corto}" -> se graba "${g?.Usuario}"`);
                const rq2 = new RequestTx(); rq2.input('u', corto);
                await rq2.query(`INSERT AuditoriaRegistros (Fecha, Modulo, Operacion, DocumentoId, Usuario) VALUES (GETDATE(), 3, 2, ${m1.id}, @u)`);
                const viejo = (await q(`SELECT TOP 1 Id FROM AuditoriaRegistros WHERE Modulo = 3 AND DocumentoId = ${m1.id} ORDER BY Id DESC`))[0].Id;
                const rq3 = new RequestTx();
                const vista = (await rq3.query(`EXEC MAM_Auditoria @tipo=4, @Id=${viejo}`)).recordset[0];
                ok(vista?.Usuario === largo, `registro anterior "${corto}" se muestra "${vista?.Usuario}"`);
            } else console.log('   (sin usuario/personal de mas de 10 letras: no se prueba el escritorio)');

            // 05: eliminar un ingreso ya usado responde "STOCK INSUFICIENTE: ..." (no se prueba aqui: su ROLLBACK
            // revertiria la transaccion de esta prueba). Se verifica que la web arme la tabla con ese mensaje.
            const smn = productosStock('STOCK INSUFICIENTE: ACEITE 1L (stock actual 0.00, necesita 4.00); ARROZ (stock actual 1.50, necesita 2.00)');
            ok(smn?.length === 2 && smn[0].quedaria === -4 && smn[1].Producto === 'ARROZ' && smn[1].quedaria === -0.5, `mensaje del 05 -> tabla: ${JSON.stringify(smn)}`);

            // ---------------- 4. pantalla ----------------
            console.log('\n4. Pantalla Auditoria De Registros');
            const l = (await okApi('GET', `/auditoria?desde=${hoy}&hasta=${hoy}&usuario=${encodeURIComponent(USR)}`)).data;
            ok(l.filas.length === 7 && l.resumen.eliminados === 4 && l.resumen.modificados === 3, `lista del usuario ${USR}: ${l.filas.length} (4 eliminados, 3 modificados)`);
            ok(l.filas.every((f) => f.Origen === 'Web'), 'origen Web');
            const el = (await okApi('GET', `/auditoria?desde=${hoy}&hasta=${hoy}&usuario=${encodeURIComponent(USR)}&operacion=2&modulo=1`)).data;
            ok(el.filas.length === 1 && num(el.filas[0].DocumentoId) === v1.id, 'filtro eliminados de ventas');
            const d = (await okApi('GET', `/auditoria/${audVenta}`)).data;
            const total = d.cabecera.find((c) => c.campo === 'Total');
            ok(total?.cambio, `detalle: Total ${total?.antes} -> ${total?.despues}`);
            ok(d.resumenDetalle.quitados === 1 && d.resumenDetalle.modificados === 1, `detalle: 1 quitado y 1 modificado (${JSON.stringify(d.resumenDetalle)})`);

            // ---------------- 5. permiso ----------------
            console.log('\n5. Usuario sin "Eliminar registros"');
            await q(`UPDATE Usuarios SET EliminarRegistros = 0 WHERE Id = ${sesion.gi_userNumi}`);
            LimpiarCachePermisos();
            const v2 = await okApi('POST', '/ventas', { ...cabV, detalle: [{ nuevo: true, Tipo: 1, ProductoId: A, PrecioId: CAT, Cantidad: 1 }], pago: { MontoBs: 999999 } });
            const r = await api('DELETE', `/ventas/${v2.id}?fecha=${hoy}`);
            ok(r.status === 403 && /eliminar registros/i.test(r.mensaje), `DELETE venta rechazado: ${r.status} ${r.mensaje}`);
            ok(num((await q(`SELECT Estado FROM Ventas WHERE Id = ${v2.id}`))[0]?.Estado) !== -1, 'la venta sigue activa');
            await q(`UPDATE Usuarios SET EliminarRegistros = 1 WHERE Id = ${sesion.gi_userNumi}`);
            LimpiarCachePermisos();

            // ---------------- 6. revertir ----------------
            console.log('\n6. Scripts REVERTIR (12, 11, 10, 09, 08, 07, 06)');
            for (const n of ['revertir/REVERTIR_12_AuditoriaUbicacionPrecisa.sql', 'revertir/REVERTIR_11_AuditoriaUsuario.sql', 'revertir/REVERTIR_10_AuditoriaEquipo.sql', 'revertir/REVERTIR_09_CompraEliminarStock.sql', 'revertir/REVERTIR_08_Auditoria_SPs.sql', 'revertir/REVERTIR_07_Auditoria.sql', 'revertir/REVERTIR_06_EliminarRegistros.sql']) {
                const m = await correr(n);
                ok(/DESHECHO/i.test(m) && !/No se pudo/i.test(m), `${n}: ${m}`);
            }
            const fin = (await q(`SELECT COL_LENGTH('dbo.Usuarios','EliminarRegistros') col, OBJECT_ID('dbo.MAM_Auditoria') sp,
                (SELECT COUNT(*) FROM Programas WHERE IdPrograma = 'btnAuditoriaRegistros') prog,
                CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas')) LIKE '%Kaily: auditoria%' THEN 1 ELSE 0 END marca`))[0];
            ok(!fin.col && !fin.sp && !fin.prog && !fin.marca, 'revertido: sin columna, sin SP, sin programa y SP originales');
        });
    } catch (e) {
        ok(false, `ERROR: ${e.message}`);
    } finally {
        srv.close();
        try { await tx.rollback(); console.log('\nTransaccion REVERTIDA.'); } catch (e) { console.log(`\nAviso al revertir: ${e.message}`); }
        pool.request = requestOriginal;
        const fin = await estadoFuera();
        ok(fin.col === inicio.col && fin.tabla === inicio.tabla && fin.sp === inicio.sp && fin.marca === inicio.marca, 'despues del rollback la BD quedo como estaba');
    }
    console.log(`\n${fallas ? `${fallas} FALLA(S)` : 'TODO OK'}`);
    await MetodoDatos.CerrarConexion();
    process.exit(fallas ? 2 : 0);
})().catch((e) => { console.error(e.message); process.exit(1); });
