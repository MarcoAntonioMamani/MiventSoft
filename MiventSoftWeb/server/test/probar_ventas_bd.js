// =====================================================================
//  PRUEBA DE GRABACION CONTRA LA BD DE PRUEBAS (SIN DEJAR RASTRO)
//    node test/probar_ventas_bd.js
//
//  Ejecuta con SUS SP reales, dentro de UNA transaccion que al final se
//  REVIERTE (ROLLBACK): nada queda grabado.
//    1. Personal:    InsertarPersonal -> ModificarPersonal -> L_prBorrarRegistro
//    2. Proveedores: InsertarProveedor -> ModificarProveedor -> L_prBorrarRegistro
//    3. Ventas:      VentaInsertar (pedido) -> verifica detalle y total
//                    VentaModificar (cambia cantidad, agrega/quita producto) -> verifica
//                    L_prBorrarRegistro -> verifica que ya no existe
//  Usa la MISMA logica que la web (rutas/ventas/ventas.routes.js).
//
//  Seguridad: pide escribir el nombre de la base antes de empezar.
//  NO lo ejecute contra produccion (aunque se revierta, bloquea tablas mientras corre).
// =====================================================================
require('dotenv').config({ path: require('node:path').join(__dirname, '..', '.env') });
const readline = require('node:readline/promises');
const sql = require('mssql');
const MetodoDatos = require('../src/datos/MetodoDatos');
const L = require('../src/negocio/AccesoLogica');
const { _interno: V } = require('../src/rutas/ventas/ventas.routes');

let fallas = 0;
const ok = (c, m) => { console.log(`${c ? '  [OK]   ' : '  [FALLA]'} ${m}`); if (!c) fallas++; };
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const col = (f, n) => { const k = Object.keys(f).find((x) => x.toLowerCase() === n.toLowerCase()); return k === undefined ? undefined : f[k]; };
const dos = (n) => String(n).padStart(2, '0');
const hoyIso = () => { const d = new Date(Date.now() - 4 * 3600 * 1000); return `${d.getUTCFullYear()}-${dos(d.getUTCMonth() + 1)}-${dos(d.getUTCDate())}`; };

(async () => {
    const pool = await MetodoDatos.CrearConexion();
    const info = (await pool.request().query('SELECT @@SERVERNAME AS s, DB_NAME() AS bd')).recordset[0];
    console.log(`\nServidor: ${info.s}   Base de datos: ${info.bd}`);
    console.log('Se ejecutaran altas, modificaciones y bajas REALES dentro de una transaccion que se revierte al final.');
    const rl = readline.createInterface({ input: process.stdin, output: process.stdout });
    const resp = (await rl.question(`Para continuar escriba el nombre de la base (${info.bd}): `)).trim();
    rl.close();
    if (resp.toLowerCase() !== String(info.bd).toLowerCase()) { console.log('Cancelado.'); process.exit(1); }

    // Todas las llamadas de la web pasan por esta transaccion
    const tx = new sql.Transaction(pool);
    await tx.begin();
    const requestOriginal = pool.request.bind(pool);
    pool.request = () => new sql.Request(tx);

    try {
        await L.L_prConUsuario('PRUEBA_WEB', async () => {
            // ---------- Personal ----------
            console.log('\nPersonal:');
            const td = await L.L_prLibreriaDetalleGeneral(8);
            const emp = await L.L_prListaEmpresasUsuarios();
            const per = { NombrePersonal: 'ZZ PRUEBA WEB', Direccion: 'X', Telefono01: '1', TipoDocumento: td[0]?.cnnum ?? 1, NroDocumento: '1', TipoPersonal: 1, Estado: 1, EmpresaId: emp[0]?.Id ?? 1, FechaNacimiento: '1990/01/01' };
            const r1 = await L.InsertarPersonal({ _Id: '', ...per });
            ok(r1.resultado, `InsertarPersonal -> Id ${r1._numi}`);
            const r2 = await L.ModificarPersonal({ _Id: r1._numi, ...per, NombrePersonal: 'ZZ PRUEBA WEB 2' });
            ok(r2.resultado, 'ModificarPersonal');
            const lp = await L.L_prListarGeneral('MAM_Personal');
            ok(lp.some((f) => num(col(f, 'Id')) === num(r1._numi) && col(f, 'NombrePersonal') === 'ZZ PRUEBA WEB 2'), 'el buscador muestra el cambio');
            ok(await L.L_prBorrarRegistro(r1._numi, 'MAM_Personal'), 'L_prBorrarRegistro MAM_Personal');

            // ---------- Proveedores ----------
            console.log('\nProveedores:');
            const pr = { NombreProveedor: 'ZZ PRUEBA WEB', Direccion: 'X', Telefono01: '1', Telefono02: '', Descripcion: 'prueba', TipoDocumento: td[0]?.cnnum ?? 1, NroDocumento: '1', Estado: 1 };
            const p1 = await L.InsertarProveedor({ _Id: '', ...pr });
            ok(p1.resultado, `InsertarProveedor -> Id ${p1._numi}`);
            ok((await L.ModificarProveedor({ _Id: p1._numi, ...pr, Descripcion: 'prueba 2' })).resultado, 'ModificarProveedor');
            ok(await L.L_prBorrarRegistro(p1._numi, 'MAM_Proveedores'), 'L_prBorrarRegistro MAM_Proveedores');

            // ---------- Ventas ----------
            console.log('\nVentas (pedido):');
            const suc = (await L.L_fnGeneralSucursales())[0];
            const cli = (await L.ListarCliente())[0];
            const vend = (await L.ListarPersonal())[0];
            const cat = (await L.L_prListaCategoriasPrecios())[0];
            if (!suc || !cli || !vend || !cat) throw new Error('Faltan datos base (sucursal, cliente, personal o categoria de precio)');
            const hoy = hoyIso();
            const cab = V._leerCabecera({ SucursalId: col(suc, 'aanumi'), FechaVenta: hoy, PersonalId: col(vend, 'Id'), ClienteId: col(cli, 'Id'), TipoVenta: 1, FechaEntregar: hoy, Glosa: 'PRUEBA WEB (se revierte)', PrecioId: col(cat, 'Id') });
            const prods = (await L.L_prListarProductosVentas(cab.AlmacenId, cab.ClienteId, cab.PrecioId)).filter((p) => num(col(p, 'stock')) >= 2);
            if (prods.length < 2) throw new Error('Se necesitan al menos 2 productos con stock >= 2 en la sucursal para la prueba');
            const [a, b] = prods;
            const entradas = [{ nuevo: true, ProductoId: col(a, 'Id'), PrecioId: cab.PrecioId, CantidadUnitaria: 1 }, { nuevo: true, ProductoId: col(b, 'Id'), PrecioId: cab.PrecioId, CantidadUnitaria: 2, ProcentajeDescuento: 10 }];
            const d1 = await V._armarDetalle({ ventaId: -1, cab, entradas, ventaDirectaVariable: 0, existentes: [] });
            const inc = await V._incrementoHoy();
            const v1 = await L.VentaInsertar({ _numi: '0', AlmacenId: cab.AlmacenId, FechaTransacccion: hoy.replace(/-/g, '/'), PersonalId: cab.PersonalId, ClienteId: cab.ClienteId, TipoVenta: 1,
                FechaVencCredito: hoy.replace(/-/g, '/'), estado: 1, glosa: cab.glosa, TotalCompra: d1.sumaTotal + inc, Descuento: 0, EstadoPedido: 1, FechaEntregar: hoy.replace(/-/g, '/'),
                VentaDirecta: 0, VentaDirectaSinConciliacion: 0, IncrementoVenta: inc }, d1.detalle);
            ok(v1.resultado, `VentaInsertar -> Venta ${v1._numi} (detalle ${d1.detalle.columnas.length} columnas)`);
            const det1 = await L.ListaVentasDetalles(v1._numi);
            ok(det1.length === 2, `ListaVentasDetalles: ${det1.length} filas (esperado 2)`);
            const suma1 = det1.reduce((s, f) => s + num(col(f, 'Total')), 0);
            ok(Math.abs(suma1 - d1.sumaTotal) < 0.01, `total detalle en BD ${suma1.toFixed(2)} = calculado ${d1.sumaTotal.toFixed(2)}`);
            const vta1 = await V._buscarVenta(v1._numi, hoy);
            ok(Math.abs(num(vta1.TotalVenta) - (d1.sumaTotal + inc)) < 0.01, `TotalVenta ${num(vta1.TotalVenta).toFixed(2)} (incremento ${inc})`);

            console.log('\nVentas (modificar):');
            const filaA = det1.find((f) => num(col(f, 'ProductoId')) === num(col(a, 'Id')));
            const entradas2 = [{ Id: col(filaA, 'Id'), CantidadUnitaria: 2 }]; // A: 1 -> 2 ; B: se elimina
            const d2 = await V._armarDetalle({ ventaId: v1._numi, cab, entradas: entradas2, ventaDirectaVariable: 0, existentes: det1 });
            const v2 = await L.VentaModificar({ _numi: v1._numi, AlmacenId: cab.AlmacenId, FechaTransacccion: hoy.replace(/-/g, '/'), PersonalId: cab.PersonalId, ClienteId: cab.ClienteId, TipoVenta: 1,
                FechaVencCredito: hoy.replace(/-/g, '/'), Moneda: 1, estado: 1, glosa: cab.glosa, TotalCompra: d2.sumaTotal + num(vta1.IncrementoVenta), Descuento: 0, EstadoPedido: num(vta1.EstadoPedido),
                FechaEntregar: hoy.replace(/-/g, '/'), VentaDirecta: 0, VentaDirectaSinConciliacion: 0 }, d2.detalle);
            ok(v2.resultado, 'VentaModificar');
            const det2 = await L.ListaVentasDetalles(v1._numi);
            ok(det2.length === 1 && num(col(det2[0], 'CantidadUnitaria')) === 2, `detalle modificado: ${det2.length} fila(s), cantidad ${det2[0] ? col(det2[0], 'CantidadUnitaria') : '-'}`);

            console.log('\nVentas (eliminar):');
            ok(await L.L_prBorrarRegistro(v1._numi, 'MAM_Ventas'), 'L_prBorrarRegistro MAM_Ventas');
            const sigue = (await L.L_prListarGeneralFechas('MAM_Ventas', hoy.replace(/-/g, '/'), hoy.replace(/-/g, '/'))).some((f) => num(col(f, 'Id')) === num(v1._numi));
            ok(!sigue, 'la venta ya no aparece en el buscador');
        });
    } catch (e) {
        ok(false, `Error: ${e.message}`);
    } finally {
        try { await tx.rollback(); console.log('\nTransaccion REVERTIDA: no quedo nada grabado.'); } catch (e) { console.log(`\nAviso al revertir: ${e.message} (si el SP revirtio por su cuenta, tampoco quedo nada).`); }
        pool.request = requestOriginal;
        await MetodoDatos.CerrarConexion();
    }
    console.log(fallas ? `\n${fallas} problema(s). Revise lo marcado como [FALLA].\n` : '\nTodo correcto: la web graba, modifica y elimina igual que el escritorio.\n');
    process.exit(fallas ? 2 : 0);
})().catch((e) => { console.error(e.message); process.exit(1); });
