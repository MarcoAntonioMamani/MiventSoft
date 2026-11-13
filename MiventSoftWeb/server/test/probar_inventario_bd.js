// =====================================================================
//  PRUEBA INTEGRAL DE STOCK Y PRECIOS CONTRA LA BD REAL (SIN DEJAR RASTRO)
//    node test/probar_inventario_bd.js            (pide el nombre de la base)
//    node test/probar_inventario_bd.js <NombreBD> (sin preguntar)
//
//  Levanta las RUTAS REALES de la web (ventas, compras, movimientos, precios, inventario)
//  y hace las operaciones como la pantalla, todo dentro de UNA transaccion que al final
//  se REVIERTE (ROLLBACK). Despues del rollback comprueba que el stock quedo como estaba.
//    1. Integridad: ProductosStock / Precios sin filas duplicadas; listas sin productos repetidos
//    2. Precios: cambiar precio -> se ve en Ventas, Compras e Inventario
//    3. Venta: grabar / modificar (cambiar cantidad, quitar item) / eliminar -> stock en BD y en las 4 listas
//    4. Compra: grabar / modificar / eliminar -> stock
//    5. Movimientos: ingreso, salida, traspaso (origen y destino) y su eliminacion -> stock
//  Solo imprime Id, cantidades y precios (ningun dato de clientes).
//  NO ejecutar en horario de atencion: mientras corre bloquea las filas que toca.
// =====================================================================
require('dotenv').config({ path: require('node:path').join(__dirname, '..', '.env') });
const readline = require('node:readline/promises');
const express = require('express');
const sql = require('mssql');
const MetodoDatos = require('../src/datos/MetodoDatos');
const L = require('../src/negocio/AccesoLogica');
const { manejadorErrores } = require('../src/middleware/errores');
const { hoyBolivia } = require('../src/rutas/distribucion/comun');

const SUC = 1;          // sucursal / deposito de la prueba
const DEP2 = 2;         // deposito destino del traspaso
const CAT = 3;          // categoria de precio por defecto de la venta (_PMOLimpiar)
const CAT_VENTA_COMPRAS = 1; // MAM_Compras @5: PrecioVenta = Min(Id) de categorias Tipo 1

let fallas = 0;
const avisos = [];
const ok = (c, m) => { console.log(`${c ? '  [OK]   ' : '  [FALLA]'} ${m}`); if (!c) fallas++; return c; };
const aviso = (m) => { console.log(`  [AVISO] ${m}`); avisos.push(m); };
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const igual = (a, b) => Math.abs(num(a) - num(b)) < 1e-6;

(async () => {
    const pool = await MetodoDatos.CrearConexion();
    const info = (await pool.request().query('SELECT @@SERVERNAME AS s, DB_NAME() AS bd')).recordset[0];
    console.log(`\nServidor: ${info.s}   Base de datos: ${info.bd}`);
    // --aplicar=archivo.sql: lo ejecuta DENTRO de la transaccion antes de probar (validar un script sin dejarlo instalado)
    const aplicar = (process.argv.find((a) => a.startsWith('--aplicar=')) || '').slice(10);
    // --rechazo=venta|salida: al final llama al SP directo (sin la validacion de la web) para ver que el SP rechaza
    const pasoRechazo = (process.argv.find((a) => a.startsWith('--rechazo=')) || '').slice(10);
    let resp = process.argv.slice(2).find((a) => !a.startsWith('--'));
    if (!resp) {
        const rl = readline.createInterface({ input: process.stdin, output: process.stdout });
        resp = await rl.question(`Todo se revierte al final. Para continuar escriba el nombre de la base (${info.bd}): `);
        rl.close();
    }
    if (String(resp).trim().toLowerCase() !== String(info.bd).toLowerCase()) { console.log('Cancelado.'); process.exit(1); }

    // Lectura directa FUERA de la transaccion (para comparar despues del rollback)
    const stockFuera = async (p, d) => num((await pool.request().input('p', sql.Int, p).input('d', sql.Int, d)
        .query('SELECT ISNULL(SUM(Cantidad),0) c FROM ProductosStock WHERE ProductoId=@p AND DepositoId=@d')).recordset[0].c);

    // Todas las llamadas de la web van por la transaccion, una a la vez (las rutas usan Promise.all)
    const tx = new sql.Transaction(pool);
    await tx.begin();
    let cola = Promise.resolve();
    const enCola = (fn) => { const p = cola.then(fn, fn); cola = p.catch(() => {}); return p; };
    class RequestTx extends sql.Request {
        constructor() { super(tx); }
        execute(...a) { return enCola(() => super.execute(...a)); }
        query(...a) { return enCola(() => super.query(...a)); }
    }
    const requestOriginal = pool.request.bind(pool);
    pool.request = () => new RequestTx();
    const q = async (s) => (await new RequestTx().query(s)).recordset;
    if (aplicar) {
        const lotes = require('node:fs').readFileSync(aplicar, 'utf8').split(/^\s*GO\s*$/im).map((x) => x.trim()).filter(Boolean);
        for (const l of lotes) await new RequestTx().query(l);
        console.log(`\nScript aplicado DENTRO de la transaccion (se revierte al final): ${aplicar} (${lotes.length} lotes)`);
    }

    // Mini servidor con las rutas reales; la sesion es la de un administrador
    const personal = num((await L.ListarPersonal())[0]?.Id);
    const sesion = { gs_user: 'PRUEBA_WEB', gi_userNumi: 0, gi_userRol: 1, gi_userSuc: -1, Global_IdPersonal: personal,
        Global_ModificarPrecio: 0, Global_ModificarDescuento: 0, Global_VentasTodos: 1, Global_CierreTodos: 1, Global_ModificarRegistro: 1, Global_NombreSucursal: '' };
    const app = express();
    app.use(express.json({ limit: '5mb' }));
    app.use((req, res, next) => { req.sesion = sesion; L.L_prConUsuario(sesion.gs_user, () => next()); });
    app.use('/ventas', require('../src/rutas/ventas/ventasPuntoVenta.routes'));
    app.use('/compras', require('../src/rutas/compras/compras.routes'));
    app.use('/mov', require('../src/rutas/inventario/movimientos.routes'));
    app.use('/precios', require('../src/rutas/configuracion/precios.routes'));
    app.use('/stock', require('../src/rutas/inventario/stock.routes'));
    app.use('/caja', require('../src/rutas/caja/cierreCajero.routes'));
    app.use(manejadorErrores);
    const srv = app.listen(0);
    const URL = `http://127.0.0.1:${srv.address().port}`;
    const api = async (metodo, ruta, cuerpo) => {
        const r = await fetch(URL + ruta, { method: metodo, headers: { 'Content-Type': 'application/json' }, body: cuerpo === undefined ? undefined : JSON.stringify(cuerpo) });
        const j = await r.json();
        if (!j.ok) throw new Error(`${metodo} ${ruta} -> ${r.status} ${j.mensaje}`);
        return j;
    };

    // ---------- lectores ----------
    const listas = async () => {
        const [ven, com, mov, inv] = [(await api('GET', `/ventas/productos?sucursal=${SUC}&precio=${CAT}`)).data,
            (await api('GET', `/compras/productos/${SUC}`)).data, (await api('GET', `/mov/productos/${SUC}`)).data,
            (await api('GET', `/stock?categoria=${CAT}`)).data];
        return { ven: ven.filter((p) => p.Tipo === 1), com, mov, inv };
    };
    const stockBD = async (p, d = SUC) => (await q(`SELECT ISNULL(SUM(Cantidad),0) c, COUNT(*) n FROM ProductosStock WHERE ProductoId=${p} AND DepositoId=${d}`))[0];
    async function verStock(etiqueta, esperado) {
        const l = await listas();
        for (const [p, cant] of Object.entries(esperado)) {
            const b = await stockBD(p);
            const vista = {
                BD: num(b.c), Ventas: num(l.ven.find((x) => x.Id === +p)?.stock), Compras: num(l.com.find((x) => x.Id === +p)?.stock),
                Movimientos: num(l.mov.find((x) => x.Id === +p)?.stock), Inventario: num(l.inv.find((x) => x.Id === +p)?.stockGeneral),
            };
            const malas = Object.entries(vista).filter(([k, x]) => k !== 'Inventario' && !igual(x, cant));
            ok(!malas.length && b.n === 1, `${etiqueta} prod ${p}: esperado ${cant} | ${Object.entries(vista).map(([k, x]) => `${k} ${x}`).join(', ')} | filas stock ${b.n}`);
        }
        return l;
    }
    async function duplicados(etiqueta, l) {
        const r = (await q(`SELECT
            (SELECT COUNT(*) FROM (SELECT 1 x FROM ProductosStock GROUP BY ProductoId, DepositoId, Lote, FechaVencimiento HAVING COUNT(*) > 1) a) stockLote,
            (SELECT COUNT(*) FROM (SELECT 1 x FROM ProductosStock GROUP BY ProductoId, DepositoId HAVING COUNT(*) > 1) a) stockProd,
            (SELECT COUNT(*) FROM (SELECT 1 x FROM Precios GROUP BY ProductoId, AlmacenId, PrecioCategoriaId HAVING COUNT(*) > 1) a) precios,
            (SELECT COUNT(*) FROM ProductosStock WHERE Cantidad < 0) negativos`))[0];
        const rep = (arr) => arr.length - new Set(arr.map((x) => x.Id)).size;
        const lis = { Ventas: rep(l.ven), Compras: rep(l.com), Movimientos: rep(l.mov), Inventario: rep(l.inv) };
        ok(r.stockLote === 0 && r.stockProd === 0, `${etiqueta}: ProductosStock sin duplicados (producto+deposito+lote: ${r.stockLote}, producto+deposito: ${r.stockProd})`);
        ok(r.precios === 0, `${etiqueta}: Precios sin duplicados (producto+almacen+categoria: ${r.precios})`);
        ok(Object.values(lis).every((x) => x === 0), `${etiqueta}: listas sin productos repetidos (${Object.entries(lis).map(([k, x]) => `${k} ${x}`).join(', ')})`);
        if (r.negativos) aviso(`${etiqueta}: ${r.negativos} fila(s) de ProductosStock con stock negativo`);
    }

    let A; let B; const inicial = {};
    try {
        await L.L_prConUsuario('PRUEBA_WEB', async () => {
            const hoy = hoyBolivia();
            console.log(`\nFecha de la prueba: ${hoy}   sucursal/deposito ${SUC}   categoria ${CAT}`);

            // ---------------- 1. integridad inicial ----------------
            console.log('\n1. Integridad del inventario (antes de tocar nada)');
            let l = await listas();
            console.log(`   productos: Ventas ${l.ven.length}, Compras ${l.com.length}, Movimientos ${l.mov.length}, Inventario ${l.inv.length}`);
            await duplicados('inicio', l);
            const inactivos = await L.L_prProductosInactivos();
            const conInactivos = Object.entries({ Ventas: l.ven, Compras: l.com, Movimientos: l.mov, Inventario: l.inv }).map(([k, x]) => `${k} ${x.filter((p) => inactivos.has(p.Id)).length}`);
            ok(conInactivos.every((x) => x.endsWith(' 0')), `productos desactivados (${inactivos.size}) no aparecen para vender/comprar/mover (${conInactivos.join(', ')})`);
            const conStock = l.ven.filter((p) => p.stock >= 5 && l.com.some((c) => c.Id === p.Id) && l.mov.some((m) => m.Id === p.Id));
            if (conStock.length < 2) throw new Error('Se necesitan 2 productos con stock >= 5 en la sucursal 1');
            [A, B] = [conStock[0].Id, conStock[1].Id];
            for (const p of [A, B]) { inicial[p] = num((await stockBD(p)).c); inicial[`${p}@2`] = num((await stockBD(p, DEP2)).c); }
            console.log(`   productos de prueba: ${A} (stock ${inicial[A]}) y ${B} (stock ${inicial[B]})`);
            await verStock('inicio', { [A]: inicial[A], [B]: inicial[B] });

            // ---------------- 2. precios ----------------
            console.log('\n2. Precios: cambiar en Tec_Precios y verlo en Ventas, Compras e Inventario');
            const precio = async (p, alm, cat) => num((await q(`SELECT Precio FROM Precios WHERE ProductoId=${p} AND AlmacenId=${alm} AND PrecioCategoriaId=${cat}`))[0]?.Precio);
            const filasPrecios = async () => num((await q('SELECT COUNT(*) n FROM Precios'))[0].n);
            const nPrecios = await filasPrecios();
            const p3 = Math.round((await precio(A, SUC, CAT) + 1.11) * 100) / 100;
            const p1 = Math.round((await precio(A, SUC, CAT_VENTA_COMPRAS) + 2.22) * 100) / 100;
            await api('PUT', `/precios/matriz/${SUC}`, { cambios: [[A, CAT, p3], [A, CAT_VENTA_COMPRAS, p1]] });
            ok(igual(await precio(A, SUC, CAT), p3) && igual(await precio(A, SUC, CAT_VENTA_COMPRAS), p1), `grabado en Precios: cat ${CAT} = ${p3}, cat ${CAT_VENTA_COMPRAS} = ${p1}`);
            ok(await filasPrecios() === nPrecios, `Precios no crecio al grabar (${nPrecios} filas)`);
            l = await listas();
            ok(igual(l.ven.find((x) => x.Id === A)?.PrecioVenta, p3), `Ventas (cat ${CAT}) muestra ${l.ven.find((x) => x.Id === A)?.PrecioVenta}, esperado ${p3}`);
            ok(igual(l.com.find((x) => x.Id === A)?.PrecioVenta, p1), `Compras (P.Venta = cat ${CAT_VENTA_COMPRAS}) muestra ${l.com.find((x) => x.Id === A)?.PrecioVenta}, esperado ${p1}`);
            ok(igual(l.inv.find((x) => x.Id === A)?.precio, p3), `Inventario (cat ${CAT}) muestra ${l.inv.find((x) => x.Id === A)?.precio}, esperado ${p3}`);
            // Sucursal 2: el precio se graba por almacen; MAM_Ventas @5 lee siempre el almacen 1
            const p3s2 = Math.round((await precio(A, DEP2, CAT) + 3.33) * 100) / 100;
            await api('PUT', `/precios/matriz/${DEP2}`, { cambios: [[A, CAT, p3s2]] });
            const ven2 = (await api('GET', `/ventas/productos?sucursal=${DEP2}&precio=${CAT}`)).data.find((x) => x.Id === A && x.Tipo === 1);
            const com2 = (await api('GET', `/compras/productos/${DEP2}`)).data.find((x) => x.Id === A);
            if (!igual(ven2?.PrecioVenta, p3s2)) aviso(`Sucursal ${DEP2}: precio cambiado a ${p3s2} en Tec_Precios, pero Ventas de la sucursal ${DEP2} muestra ${ven2?.PrecioVenta} (MAM_Ventas @5 usa siempre Precios del almacen 1)`);
            else ok(true, `Ventas sucursal ${DEP2} muestra el precio de su almacen (${p3s2})`);
            ok(igual(com2?.PrecioVenta, await precio(A, DEP2, CAT_VENTA_COMPRAS)), `Compras sucursal ${DEP2} usa el precio de su almacen`);
            await duplicados('despues de grabar precios', l);

            // ---------------- 3. venta ----------------
            console.log('\n3. Venta: grabar, modificar (cambiar cantidad y quitar un item) y eliminar');
            const caja = await L.L_prListarGeneral('MAM_CierreCajero');
            const abierta = caja.some((f) => num(f.SucursalId) === SUC && num(f.EstadoCaja) === 1 && new Date(f.Fecha).toISOString().slice(0, 10) === hoy);
            if (!abierta) { await api('POST', '/caja', { SucursalId: SUC, MontoInicial: 0, TipoCambio: 6.96, Observacion: 'PRUEBA (se revierte)' }); console.log('   (se abrio una caja chica de prueba para hoy, tambien se revierte)'); }
            const cliente = num((await L.ListarCliente())[0]?.Id);
            const cabV = { SucursalId: SUC, FechaVenta: hoy, ClienteId: cliente, TipoVenta: 1, Glosa: 'PRUEBA WEB (se revierte)', PrecioId: CAT, Descuento: 0 };
            // Se puede elegir con precio 0 o sin stock, pero no grabar
            const rechazo = async (cuerpo) => { try { await api('POST', '/ventas', cuerpo); return null; } catch (e) { return e.message; } };
            const cero = l.ven.find((p) => !(num(p.PrecioVenta) > 0) && p.stock >= 1);
            if (cero) {
                const r = await rechazo({ ...cabV, detalle: [{ nuevo: true, Tipo: 1, ProductoId: cero.Id, PrecioId: CAT, Cantidad: 1 }], pago: { MontoBs: 999999 } });
                ok(r && /sin precio/i.test(r), `venta de un producto con precio 0 (prod ${cero.Id}) rechazada: ${r ? 'si' : 'NO, se grabo'}`);
            }
            const vacio = l.ven.find((p) => num(p.PrecioVenta) > 0 && !(p.stock > 0));
            if (vacio) {
                const r = await rechazo({ ...cabV, detalle: [{ nuevo: true, Tipo: 1, ProductoId: vacio.Id, PrecioId: CAT, Cantidad: 1 }], pago: { MontoBs: 999999 } });
                ok(r && /stock/i.test(r), `venta de un producto sin stock (prod ${vacio.Id}) rechazada: ${r ? 'si' : 'NO, se grabo'}`);
            }
            const r3 = await rechazo({ ...cabV, detalle: [{ nuevo: true, Tipo: 1, ProductoId: B, PrecioId: CAT, Cantidad: inicial[B] + 1 }], pago: { MontoBs: 999999 } });
            ok(r3 && /stock/i.test(r3), `venta de mas cantidad que el stock (prod ${B}: ${inicial[B] + 1}) rechazada: ${r3 ? 'si' : 'NO, se grabo'}`);
            const v1 = await api('POST', '/ventas', { ...cabV, detalle: [{ nuevo: true, Tipo: 1, ProductoId: A, PrecioId: CAT, Cantidad: 2 }, { nuevo: true, Tipo: 1, ProductoId: B, PrecioId: CAT, Cantidad: 1 }], pago: { MontoBs: 999999 } });
            ok(true, `venta ${v1.id} grabada (A x2, B x1)`);
            const det = (await api('GET', `/ventas/${v1.id}/detalle`)).data;
            const filas = Array.isArray(det) ? det : det.detalle || det.filas || [];
            const dA = filas.find((f) => num(f.ProductoId) === A);
            ok(dA && igual(dA.Precio, p3), `la venta uso el precio nuevo (${dA?.Precio} = ${p3})`);
            await verStock('venta grabada', { [A]: inicial[A] - 2, [B]: inicial[B] - 1 });
            await api('PUT', `/ventas/${v1.id}`, { ...cabV, fechaOriginal: hoy, detalle: [{ Id: num(dA.Id), Cantidad: 1 }], pago: { MontoBs: 999999 } });
            const det2 = (await api('GET', `/ventas/${v1.id}/detalle`)).data;
            const filas2 = Array.isArray(det2) ? det2 : det2.detalle || det2.filas || [];
            ok(filas2.length === 1, `venta modificada: ${filas2.length} item(s) en el detalle (esperado 1)`);
            l = await verStock('venta modificada (A 2->1, B quitado)', { [A]: inicial[A] - 1, [B]: inicial[B] });
            await api('DELETE', `/ventas/${v1.id}?fecha=${hoy}`);
            l = await verStock('venta eliminada', { [A]: inicial[A], [B]: inicial[B] });
            await duplicados('despues de la venta', l);

            // ---------------- 4. compra ----------------
            console.log('\n4. Compra: grabar, modificar y eliminar');
            const prov = num((await L.ListarProveedores())[0]?.Id);
            const cabC = { AlmacenId: SUC, ProveedorId: prov, TipoVenta: 1, FechaTransaccion: hoy, Glosa: 'PRUEBA WEB (se revierte)', Descuento: 0 };
            const c1 = await api('POST', '/compras', { ...cabC, detalle: [{ ProductoId: A, CantidadCompra: 5 }] });
            await verStock(`compra ${c1.id} grabada (+5)`, { [A]: inicial[A] + 5 });
            const dc = (await api('GET', `/compras/${c1.id}/detalle`)).data[0];
            await api('PUT', `/compras/${c1.id}`, { ...cabC, fechaOriginal: hoy, detalle: [{ Id: dc.Id, CantidadCompra: 3, PrecioCosto: dc.PrecioCosto, PrecioVenta: dc.PrecioVenta }] });
            await verStock('compra modificada (5->3)', { [A]: inicial[A] + 3 });
            ok(igual(await precio(A, SUC, CAT), p3), `la compra no altero el precio de venta cat ${CAT}`);
            await api('DELETE', `/compras/${c1.id}?fecha=${hoy}`);
            l = await verStock('compra eliminada', { [A]: inicial[A] });
            await duplicados('despues de la compra', l);

            // ---------------- 5. movimientos ----------------
            console.log('\n5. Movimientos: ingreso, salida y traspaso');
            const tipos = (await api('GET', '/mov/combos')).data.tipos;
            const salida = tipos.find((t) => ![4, 7, 8].includes(num(t.Id)) && /SALIDA|EGRESO|BAJA/i.test(t.Descripcion));
            const cabM = { DepositoId: SUC, FechaDocumento: hoy, Observacion: 'PRUEBA WEB (se revierte)' };
            const m1 = await api('POST', '/mov', { ...cabM, ConceptoId: 4, detalle: [{ ProductoId: A, Cantidad: 4 }] });
            await verStock(`ingreso ${m1.id} (+4)`, { [A]: inicial[A] + 4 });
            const dm = (await api('GET', `/mov/${m1.id}/detalle`)).data[0];
            await api('PUT', `/mov/${m1.id}`, { ...cabM, ConceptoId: 4, detalle: [{ Id: dm.Id, Cantidad: 2 }] });
            await verStock('ingreso modificado (4->2)', { [A]: inicial[A] + 2 });
            await api('DELETE', `/mov/${m1.id}`);
            await verStock('ingreso eliminado', { [A]: inicial[A] });
            if (salida) {
                const m2 = await api('POST', '/mov', { ...cabM, ConceptoId: num(salida.Id), detalle: [{ ProductoId: A, Cantidad: 1 }] });
                await verStock(`salida ${m2.id} "${salida.Descripcion}" (-1)`, { [A]: inicial[A] - 1 });
                await api('DELETE', `/mov/${m2.id}`);
                await verStock('salida eliminada', { [A]: inicial[A] });
            } else aviso('no se encontro un tipo de movimiento de salida para probar');
            const t1 = await api('POST', '/mov', { ...cabM, ConceptoId: 8, DepositoIdDestino: DEP2, detalle: [{ ProductoId: A, Cantidad: 1 }] });
            const s2 = await stockBD(A, DEP2);
            await verStock(`traspaso ${t1.id} -> ${t1.idDestino} (origen -1)`, { [A]: inicial[A] - 1 });
            ok(igual(s2.c, inicial[`${A}@2`] + 1) && s2.n === 1, `traspaso destino deposito ${DEP2}: ${s2.c} (esperado ${inicial[`${A}@2`] + 1}), filas stock ${s2.n}`);
            await api('DELETE', `/mov/${t1.id}`);
            const s2b = await stockBD(A, DEP2);
            await verStock('traspaso eliminado (origen)', { [A]: inicial[A] });
            ok(igual(s2b.c, inicial[`${A}@2`]), `traspaso eliminado: destino deposito ${DEP2} vuelve a ${inicial[`${A}@2`]} (tiene ${s2b.c})`);
            if (!igual(s2b.c, inicial[`${A}@2`])) {
                const quedo = (await q(`SELECT COUNT(*) n FROM Movimientos WHERE Id = ${num(t1.idDestino)}`))[0].n;
                aviso(`al eliminar la salida del traspaso el ingreso ${t1.idDestino} del destino ${quedo ? 'SIGUE grabado' : 'se borro pero el stock no volvio'}`);
            }
            l = await listas();
            await duplicados('final', l);

            // ---------------- 6. el SP rechaza aunque la web no valide (escritorio / dos usuarios a la vez) ----------------
            // Va al final: al rechazar, el SP hace ROLLBACK de TODO (tambien de esta prueba)
            if (pasoRechazo) {
                const definicion = (await q("SELECT OBJECT_DEFINITION(OBJECT_ID('dbo.MAM_Ventas')) d"))[0].d;
                if (!/STOCK INSUFICIENTE/.test(definicion)) aviso('el SP aun no tiene la validacion de stock (05_StockSeguro.sql): se omite el paso 6');
                else {
                    console.log(`\n6. SP directo (sin la validacion de la web): ${pasoRechazo} de mas que el stock`);
                    const pide = inicial[A] + 5;
                    const fila = (columnas, valores) => Object.fromEntries(columnas.map((c) => [c, valores[Object.keys(valores).find((k) => k.toLowerCase() === c.toLowerCase())] ?? null]));
                    const comun = { Id: 1, ProductoId: A, Producto: 'PRUEBA', Cantidad: pide, Lote: '20200101', FechaVencimiento: new Date(Date.UTC(2020, 0, 1)), estado: 0 };
                    let error = null;
                    try {
                        if (pasoRechazo === 'venta') {
                            const columnas = L._columnasDe(await L.ListaVentasDetalles(-1));
                            const det = { columnas, filas: [fila(columnas, { ...comun, VentaId: 0, Precio: 1, SubTotal: pide, ProcentajeDescuento: 0, MontoDescuento: 0, Total: pide, PrecioCosto: 0, Tipo: 1, KitId: 0, CantidadKit: 0 })] };
                            const pagos = { columnas: L._columnasDe(await L.ListaVentasDetallePago(-1)), filas: [] };
                            await L.VentaInsertar({ _numi: '0', AlmacenId: SUC, FechaTransacccion: hoy.replace(/-/g, '/'), PersonalId: personal, ClienteId: cliente, TipoVenta: 0,
                                FechaVencCredito: hoy.replace(/-/g, '/'), glosa: 'PRUEBA', TotalCompra: pide, Descuento: 0, Facturado: 0 }, det, pagos);
                        } else {
                            const columnas = L._columnasDe(await L.L_prListarDetalleMovimiento(-1));
                            await L.L_prMovimientoInsertar({ ConceptoId: num(salida.Id), DepositoId: SUC, Observacion: 'PRUEBA', FechaDocumento: hoy.replace(/-/g, '/'), DepositoIdDestino: 0, IdMovimientoDestino: 0, _numi: '' },
                                { columnas, filas: [fila(columnas, { ...comun, MovimientoId: 0, stock: 0 })] });
                        }
                    } catch (e) { error = e.message || 'el SP revirtio la transaccion (dentro de esta prueba el texto no llega; en uso normal llega STOCK INSUFICIENTE: ...)'; }
                    ok(error !== null, `el SP rechazo: ${error ?? 'NO, se grabo'}`);
                }
            }
        });
    } catch (e) {
        ok(false, `Error: ${e.message}`);
    } finally {
        srv.close();
        await cola.catch(() => {});
        try { await tx.rollback(); console.log('\nTransaccion REVERTIDA.'); } catch (e) { console.log(`\nAviso al revertir: ${e.message}`); }
        pool.request = requestOriginal;
    }
    if (A) {
        const fin = [await stockFuera(A, SUC), await stockFuera(B, SUC), await stockFuera(A, DEP2)];
        ok(igual(fin[0], inicial[A]) && igual(fin[1], inicial[B]) && igual(fin[2], inicial[`${A}@2`]), `despues del rollback el stock real quedo igual (${A}: ${fin[0]}, ${B}: ${fin[1]}, ${A} dep ${DEP2}: ${fin[2]})`);
    }
    await MetodoDatos.CerrarConexion();
    console.log(`\n${fallas ? `${fallas} FALLA(S)` : 'Sin fallas'}${avisos.length ? `, ${avisos.length} aviso(s)` : ''}.\n`);
    process.exit(fallas ? 2 : 0);
})().catch((e) => { console.error(e.message); process.exit(1); });
