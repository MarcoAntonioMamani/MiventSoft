// =====================================================================
//  Pruebas automaticas del modulo Configuracion  ->  npm test
//  Corre la API con el simulador de SP (MetodoDatos.mock.js), que RECHAZA
//  cualquier llamada cuyo SP/@tipo/parametros no coincida con AccesoLogica.vb.
// =====================================================================
const { test, before, after } = require('node:test');
const assert = require('node:assert/strict');
const os = require('node:os');
const fs = require('node:fs');
const path = require('node:path');

const PUERTO = 3199;
const URL = `http://127.0.0.1:${PUERTO}`;
const carpeta = fs.mkdtempSync(path.join(os.tmpdir(), 'mivent-'));
Object.assign(process.env, {
    MIVENT_MOCK: '1', NODE_ENV: 'test', PORT: String(PUERTO), CARPETA_RAIZ: carpeta,
    JWT_SECRET: 'prueba_prueba_prueba_prueba_prueba_1234567890',
});

let cookie = '';
const mock = require('../src/datos/MetodoDatos.mock');
const JPG = Buffer.from('ffd8ffe000104a46494600010100000100010000ffdb004300', 'hex');

async function api(metodo, ruta, cuerpo, { form } = {}) {
    const op = { method: metodo, headers: { cookie } };
    if (form) op.body = form;
    else if (cuerpo !== undefined) { op.body = JSON.stringify(cuerpo); op.headers['Content-Type'] = 'application/json'; }
    const r = await fetch(URL + ruta, op);
    const sc = r.headers.get('set-cookie');
    if (sc) cookie = sc.split(';')[0];
    const json = await r.json();
    return { status: r.status, ...json };
}
const ok = async (...a) => {
    const r = await api(...a);
    assert.equal(r.ok, true, `${a[0]} ${a[1]} -> ${r.status} ${r.mensaje}`);
    return r;
};
const ultimaLlamada = (sp, tipo) => [...mock.__llamadas].reverse().find((l) => l.sp === sp && l.tipo === tipo);

before(async () => {
    require('../src/app');
    await new Promise((r) => setTimeout(r, 300));
});
after(() => {
    fs.rmSync(carpeta, { recursive: true, force: true });
    setTimeout(() => process.exit(0), 50);
});

test('login: rechaza credenciales malas y acepta ADMIN/123', async () => {
    assert.equal((await api('GET', '/api/configuracion/clientes')).status, 401);
    assert.equal((await api('POST', '/api/auth/login', { usuario: 'admin', password: 'x' })).status, 401);
    await ok('POST', '/api/auth/login', { usuario: 'admin', password: '123' });
    const s = await ok('GET', '/api/auth/sesion');
    const progs = s.data.modulos.flatMap((m) => m.programas.map((p) => p.IdPrograma));
    for (const p of ['BTCONFCLIENTE', 'BTZONA', 'BTCONFPRODUCTO', 'BTCONFPRECIO']) assert.ok(progs.includes(p), p);
});

test('no se envian datos innecesarios', async () => {
    const u = await ok('GET', '/api/configuracion/usuarios');
    assert.ok(u.data.every((f) => !('Contrasena' in f)), 'la contrasena no debe viajar');
    const c = await ok('GET', '/api/configuracion/clientes');
    assert.ok(c.data.every((f) => !('FechaUltimaVenta' in f) && !('ImagenCliente' in f) && !('imgEstado' in f)));
    const p = await ok('GET', '/api/configuracion/productos');
    assert.ok(p.data.every((f) => !('FechaRegistro' in f) && !('UsuarioRegistro' in f)));
    const r = await ok('GET', '/api/configuracion/roles');
    assert.deepEqual(Object.keys(r.data[0]), ['Id', 'NombreRol']);
});

test('CLIENTES: insertar, modificar (conserva FechaIngreso), validar y eliminar', async () => {
    const combos = await ok('GET', '/api/configuracion/clientes/combos');
    assert.ok(combos.data.zonas.length && combos.data.precios.length && combos.data.tiposDocumento.length);

    const malo = await api('POST', '/api/configuracion/clientes', { NombreCliente: '', ZonaId: 1, PrecioCategoriaId: 1, TipoDocumento: 1, TipoNegocio: 1 });
    assert.equal(malo.status, 400);

    const nuevo = { NombreCliente: 'TIENDA PRUEBA', DireccionCliente: 'Av. Siempre Viva', Telefono: '70011122', ZonaId: 2, PrecioCategoriaId: 2,
        TipoDocumento: 1, NroDocumento: '1234567', RazonSocial: 'PRUEBA SRL', Nit: '99887766', Estado: true, Latitud: -17.4, Longitud: -66.15, TipoNegocio: 1, Referencia: 'Esquina' };
    const r = await ok('POST', '/api/configuracion/clientes', nuevo);
    const llam = ultimaLlamada('MAM_Clientes', 1);
    assert.equal(llam.params['@NombreCliente'], 'TIENDA PRUEBA');
    assert.match(llam.params['@FechaIngreso'], /^\d{4}\/\d{2}\/\d{2}$/);

    let lista = (await ok('GET', '/api/configuracion/clientes')).data;
    const creado = lista.find((x) => Number(x.Id) === Number(r.id));
    assert.equal(creado.NombreCliente, 'TIENDA PRUEBA');
    assert.equal(Number(creado.ZonaId), 2);

    await ok('PUT', '/api/configuracion/clientes/1', { ...nuevo, NombreCliente: 'TIENDA DONA ROSA (MOD)' });
    assert.equal(ultimaLlamada('MAM_Clientes', 2).params['@FechaIngreso'], '2026/01/15', 'debe conservar la fecha original');

    const lib = await ok('POST', '/api/configuracion/clientes/libreria/8', { descripcion: 'pasaporte' });
    assert.ok(lib.data.some((x) => x.cndesc1 === 'PASAPORTE'));
    assert.equal((await api('POST', '/api/configuracion/clientes/libreria/3', { descripcion: 'x' })).status, 403);

    await ok('DELETE', `/api/configuracion/clientes/${r.id}`);
    lista = (await ok('GET', '/api/configuracion/clientes')).data;
    assert.ok(!lista.some((x) => Number(x.Id) === Number(r.id)));
});

test('ZONAS: insertar con poligono, modificar sin y con redibujo, eliminar', async () => {
    assert.equal((await api('POST', '/api/configuracion/zonas', { NombreZona: 'SUR', puntos: [] })).status, 400);
    assert.equal((await api('POST', '/api/configuracion/zonas', { NombreZona: 'SUR', puntos: [[-17.4, -66.1], [-17.41, -66.1]] })).status, 400);

    const pts = [[-17.41, -66.16], [-17.41, -66.14], [-17.42, -66.14], [-17.42, -66.16]];
    const r = await ok('POST', '/api/configuracion/zonas', { NombreZona: 'SUR', DescripcionZona: 'Zona sur', Color: '#ff0000', puntos: pts });
    const tvp = ultimaLlamada('sp_Mam_Zonas', 1).params['@ZonasPuntos'].filas;
    assert.deepEqual(tvp[0], [1, 1, -17.41, -66.16, 0], 'fila igual a _TablePoint.Rows.Add(1,1,lat,lng,0)');
    assert.equal((await ok('GET', `/api/configuracion/zonas/${r.id}/puntos`)).data.length, 4);

    // modificar SIN redibujar: los puntos existentes van con estado 1
    await ok('PUT', `/api/configuracion/zonas/${r.id}`, { NombreZona: 'SUR 2', Color: '#00FF00' });
    assert.ok(ultimaLlamada('sp_Mam_Zonas', 2).params['@ZonasPuntos'].filas.every((f) => f[4] === 1));
    assert.equal((await ok('GET', `/api/configuracion/zonas/${r.id}/puntos`)).data.length, 4);

    // redibujar: existentes -> -1, nuevos -> 0
    await ok('PUT', `/api/configuracion/zonas/${r.id}`, { NombreZona: 'SUR 2', Color: '#00FF00', puntos: pts.slice(0, 3) });
    const estados = ultimaLlamada('sp_Mam_Zonas', 2).params['@ZonasPuntos'].filas.map((f) => f[4]);
    assert.deepEqual(estados, [-1, -1, -1, -1, 0, 0, 0]);
    assert.equal((await ok('GET', `/api/configuracion/zonas/${r.id}/puntos`)).data.length, 3);

    const z = (await ok('GET', '/api/configuracion/zonas')).data.find((x) => Number(x.Id) === Number(r.id));
    assert.equal(z.Color, '#00FF00');
    await ok('DELETE', `/api/configuracion/zonas/${r.id}`);
});

test('PRODUCTOS: insertar con imagenes, quitar imagen, validar y eliminar', async () => {
    const combos = await ok('GET', '/api/configuracion/productos/combos');
    assert.ok(combos.data.categorias.length && combos.data.unidadesVenta.length);

    const form = (campos, archivos = [], extra = {}) => {
        const f = new FormData();
        Object.entries({ ...campos, ...extra }).forEach(([k, val]) => f.append(k, typeof val === 'object' ? JSON.stringify(val) : String(val)));
        archivos.forEach((b, i) => f.append('imagenes', new Blob([b], { type: 'image/jpeg' }), `f${i}.jpg`));
        return f;
    };
    const base = { NombreProducto: 'FANTA 2L', CodigoExterno: 'P-900', CodigoBarras: '7770001', StockMinimo: 12, Conversion: 6, estado: 1,
        CategoriaId: 1, EmpresaId: 1, ProveedorId: 1, MarcaId: 1, AttributoId: 1, FamiliaId: 1, UnidadVentaId: 1, UnidadMaximaId: 1 };

    assert.equal((await api('POST', '/api/configuracion/productos', null, { form: form({ ...base, Conversion: 0 }) })).status, 400);
    assert.equal((await api('POST', '/api/configuracion/productos', null, { form: form(base, [Buffer.from('no soy imagen')]) })).status, 400);

    const r = await ok('POST', '/api/configuracion/productos', null, { form: form(base, [JPG, JPG]) });
    const tvp = ultimaLlamada('MAM_Productos', 1).params['@TCL0064'].filas;
    assert.equal(tvp.length, 2);
    assert.equal(tvp[0][4], 0);
    let imgs = (await ok('GET', `/api/configuracion/productos/${r.id}/imagenes`)).data;
    assert.equal(imgs.length, 2);
    const dir = path.join(carpeta, 'Imagenes', 'Imagenes Productos', 'ProductosTodos');
    assert.equal(fs.readdirSync(dir).length, 2, 'archivos guardados en la carpeta del escritorio');

    await ok('PUT', `/api/configuracion/productos/${r.id}`, null, { form: form({ ...base, NombreProducto: 'FANTA 3L' }, [], { eliminar: [imgs[0]] }) });
    const estados = ultimaLlamada('MAM_Productos', 2).params['@TCL0064'].filas.map((f) => f[4]);
    assert.deepEqual(estados.sort(), [-1, 1]);
    imgs = (await ok('GET', `/api/configuracion/productos/${r.id}/imagenes`)).data;
    assert.equal(imgs.length, 1);
    assert.equal(fs.readdirSync(dir).length, 1, 'el archivo eliminado se borra');

    const prod = (await ok('GET', '/api/configuracion/productos')).data.find((x) => Number(x.Id) === Number(r.id));
    assert.equal(prod.NombreProducto, 'FANTA 3L');

    // Kaily: precios por categoria (@PrecioT, tabla completa de ListPreciosDetalles por posicion)
    assert.equal((await ok('GET', '/api/configuracion/productos/-1/precios')).data.length, 2, 'producto nuevo: ListPreciosDetalles(-1)');
    await ok('PUT', `/api/configuracion/productos/${r.id}`, null, { form: form(base, [], { precios: JSON.stringify({ 1: 4.5, 2: -3 }) }) });
    assert.deepEqual(ultimaLlamada('MAM_Productos', 2).params['@PrecioT'].filas, [[1, 'PRECIO COSTO', 4.5], [2, 'PRECIO VENTA', 0]], 'precio <= 0 se graba 0');
    assert.deepEqual((await ok('GET', `/api/configuracion/productos/${r.id}/precios`)).data.map((x) => x.precio), [4.5, 0]);

    const bloqueado = await api('DELETE', '/api/configuracion/productos/1');
    assert.equal(bloqueado.status, 400);
    assert.match(bloqueado.mensaje, /MOVIMIENTOS/, 'muestra el mensaje que devuelve el SP');
    await ok('DELETE', `/api/configuracion/productos/${r.id}`);
});

test('PRECIOS: matriz, solo cambios, estados 2/3 y categorias', async () => {
    const alm = (await ok('GET', '/api/configuracion/precios/almacenes')).data[0].aanumi;
    const m = (await ok('GET', `/api/configuracion/precios/matriz/${alm}`)).data;
    assert.ok(m.productos.length && m.categorias.length);
    assert.ok(m.precios.every((p) => p.length === 3), 'solo [producto, categoria, precio]');

    const existente = m.precios[0];
    const vacio = m.productos.map((p) => p.yfnumi).find((pid) => !m.precios.some((x) => x[0] === pid && x[1] === 3));
    await ok('PUT', `/api/configuracion/precios/matriz/${alm}`, { cambios: [[existente[0], existente[1], 99.5], [vacio, 3, 12.25]] });
    const filas = ultimaLlamada('sp_Mam_Precios', 2).params['@TY007'].filas;
    const est = (pid, cid) => filas.find((f) => f[1] === pid && f[2] === cid)[5];
    assert.equal(est(existente[0], existente[1]), 2);
    assert.equal(est(vacio, 3), 3);

    const m2 = (await ok('GET', `/api/configuracion/precios/matriz/${alm}`)).data;
    assert.equal(m2.precios.find((x) => x[0] === existente[0] && x[1] === existente[1])[2], 99.5);
    assert.equal(m2.precios.find((x) => x[0] === vacio && x[1] === 3)[2], 12.25);

    assert.equal((await api('PUT', `/api/configuracion/precios/matriz/${alm}`, { cambios: [[9999, 1, 5]] })).status, 400);
    assert.equal((await api('PUT', `/api/configuracion/precios/matriz/${alm}`, { cambios: [[existente[0], existente[1], -1]] })).status, 400);

    const c = await ok('POST', '/api/configuracion/precios/categorias', { descripcion: 'DISTRIBUIDOR', tipo: 1 });
    assert.equal(ultimaLlamada('sp_Mam_Precios', 1).params['@Codigo'], 'D', 'siguiente letra despues de C');
    await ok('POST', '/api/configuracion/precios/categorias', { descripcion: 'ESPECIAL', tipo: 0 });
    assert.equal(ultimaLlamada('sp_Mam_Precios', 1).params['@Codigo'], '1');
    const cats = (await ok('GET', '/api/configuracion/precios/categorias')).data;
    assert.equal((await api('DELETE', `/api/configuracion/precios/categorias/${cats[0].ygnumi}`)).status, 400, 'categoria del sistema');
    await ok('DELETE', `/api/configuracion/precios/categorias/${c.id}`);
});

test('PASO 1 sigue funcionando: categoria, empresa, rol y usuario', async () => {
    const f = new FormData();
    f.append('NombreCategoria', 'SNACKS'); f.append('EmpresaId', '1'); f.append('Estado', '1'); f.append('VisibleApp', '1');
    const cat = await ok('POST', '/api/configuracion/categorias', null, { form: f });
    await ok('DELETE', `/api/configuracion/categorias/${cat.id}`);

    const det = (await ok('GET', '/api/configuracion/roles/-1/detalle')).data;
    const rol = await ok('POST', '/api/configuracion/roles', { NombreRol: 'SUPERVISOR', detalle: det.map((d) => ({ ...d, Ver: d.ProgramaId === 5 ? 1 : 0 })) });
    const usu = await ok('POST', '/api/configuracion/usuarios', { NombreUsuario: 'sup1', Contrasena: 'abc', IdEmpresa: 1, RolId: rol.id, IdPersonal: 3, estado: true, pedido: false });
    assert.equal(ultimaLlamada('MAM_Usuarios', 1).params['@NombreUsuario'], 'SUP1');
    await ok('PUT', `/api/configuracion/usuarios/${usu.id}`, { NombreUsuario: 'sup1', Contrasena: '', IdEmpresa: 1, RolId: rol.id, IdPersonal: 3, estado: true,
        SucursalId: -1, ModificarPrecioVenta: true, AplicarDescuentoVenta: false, VerTodasVentas: true, VerTodosVentasCierre: false, ModificarRegistroAnteriores: true });
    const llU = ultimaLlamada('MAM_Usuarios', 2).params;
    assert.equal(llU['@Contrasena'], 'ABC', 'contrasena vacia = conserva la actual');
    assert.deepEqual([llU['@SucursalId'], llU['@ModificarPrecio'], llU['@ModificarDescuento'], llU['@VentasTodasVentas'], llU['@VerTodasVentasCierre'], llU['@ModificarRegistroAntiguos']],
        [-1, 1, 0, 1, 0, 1], 'permisos de Kaily (Tec_Users.vb)');
    await ok('DELETE', `/api/configuracion/usuarios/${usu.id}`);
    await ok('DELETE', `/api/configuracion/roles/${rol.id}`);
});

test('PASO 3 PERSONAL y PROVEEDORES: insertar, modificar, validar y eliminar', async () => {
    const malo = await api('POST', '/api/ventas/personal', { NombrePersonal: '', EmpresaId: 1, TipoDocumento: 1 });
    assert.equal(malo.status, 400);
    const r = await ok('POST', '/api/ventas/personal', { NombrePersonal: 'juan', EmpresaId: 1, TipoDocumento: 1, TipoPersonal: 2, Estado: true, FechaNacimiento: '1990-03-04' });
    const ins = ultimaLlamada('MAM_Personal', 1).params;
    assert.equal(ins['@NombrePersonal'], 'JUAN');
    assert.equal(ins['@FechaNacimiento'], '1990/03/04', 'fecha como VB: yyyy/MM/dd');
    assert.equal(ins['@estado'], 1);
    await ok('PUT', `/api/ventas/personal/${r.id}`, { NombrePersonal: 'juan 2', EmpresaId: 1, TipoDocumento: 2, Estado: false });
    assert.equal(ultimaLlamada('MAM_Personal', 2).params['@estado'], 0);
    await ok('DELETE', `/api/ventas/personal/${r.id}`);
    const pr = await ok('POST', '/api/ventas/proveedores', { NombreProveedor: 'prov', TipoDocumento: 2, Descripcion: 'abc', Estado: 1 });
    assert.equal(ultimaLlamada('MAM_Proveedores', 1).params['@Descripcion'], 'abc', 'la descripcion ya no se pierde');
    await ok('DELETE', `/api/ventas/proveedores/${pr.id}`);
});

test('VENTAS (Kaily): precios del servidor, kits, pagos, caja abierta, permisos y bloqueos', async () => {
    // KAILY (punto de venta): Tec_Ventas.vb + Tec_VentasDetalle.vb + EfectivoCobranza.vb
    const hoy = (() => { const d = new Date(Date.now() - 4 * 3600e3); return d.toISOString().slice(0, 10); })();
    const ayer = new Date(Date.now() - 28 * 3600e3).toISOString().slice(0, 10);
    const cerca = (a, b) => Math.abs(a - b) < 1e-6;
    const prods = await ok('GET', '/api/ventas/ventas/productos?sucursal=1&precio=2');
    assert.ok(prods.data.every((p) => !('PrecioCosto' in p)), 'el costo no viaja al navegador');
    assert.ok(prods.data.some((p) => p.Tipo === 2), 'los kits vienen en la misma lista (Tipo 2)');
    assert.deepEqual(Object.keys(ultimaLlamada('MAM_Ventas', 5).params).sort(), ['@CategoriaVentaId', '@SucursalId', '@tipo', '@usuario'].sort());
    assert.equal((await ok('GET', '/api/ventas/ventas/caja-abierta')).data.abierta, true);

    // el navegador manda vendedor 2 y un total falso: el vendedor es el del usuario y los montos los calcula el servidor
    const cab = { SucursalId: 1, FechaVenta: hoy, PersonalId: 2, ClienteId: 1, TipoVenta: 1, PrecioId: 2, Glosa: 'test', TotalVenta: 1 };
    const det1 = [
        { nuevo: true, Tipo: 1, ProductoId: 1, Cantidad: 12 },
        { nuevo: true, Tipo: 1, ProductoId: 3, Cantidad: 3, ProcentajeDescuento: 10 },
        { nuevo: true, Tipo: 2, KitId: 90, CantidadKit: 2 },
        { nuevo: true, Tipo: 1, ProductoId: 2, Cantidad: 6, eliminado: true },
    ];
    assert.equal((await api('POST', '/api/ventas/ventas', { ...cab, Descuento: 1, detalle: det1, pago: { MontoBs: 100 } })).mensaje, 'El Monto Pagado Es Menor Al Monto Total De La Venta');
    // $ x tipo de cambio (6.96) tambien cuenta
    const v = await ok('POST', '/api/ventas/ventas', { ...cab, Descuento: 1, detalle: det1, pago: { MontoBs: 50, MontoDolares: 20, Tarjeta: 0, Transferencia: 0 } });
    let ll = ultimaLlamada('MAM_Ventas', 1).params;
    const det = ll['@VentaDetalleType'];
    assert.equal(det.columnas.length, 22, 'columnas en el orden de MAM_Ventas @tipo 4 de Kaily');
    assert.ok(det.filas.every((f) => f.estado === 0), 'nuevas = 0; la agregada y quitada no se envia');
    assert.equal(det.filas.find((f) => f.ProductoId === 1 && f.Tipo === 1).Precio, 8.5, 'precio por unidad del SP, no del navegador');
    const kit = det.filas.filter((f) => f.Tipo === 2);
    assert.deepEqual(kit.map((f) => [f.ProductoId, f.Cantidad, f.KitId, f.CantidadKit]), [[1, 4, 90, 2], [3, 2, 90, 2]], 'el kit se agrega como sus productos');
    assert.ok(cerca(ll['@TotalVenta'], 102 + 31.05 + 50 - 1), 'Total = Suma - Descuento (sin incremento)');
    assert.equal(ll['@PersonalId'], 1, 'vendedor = personal del usuario');
    assert.equal(ll['@Facturado'], 0);
    assert.deepEqual(ll['@VentaPagos'].filas, [[0, 0, 50, 20, 0, 0, 6.96, 0]], 'fila de pago como dt.Rows.Add de VB');
    assert.ok(!('@EstadoPedido' in ll) && !('@IncrementoVenta' in ll), 'sin campos de distribucion');

    // modificar: cambia cantidad (2), quita (-1); los productos del kit quedan sin cambios (1)
    const detBD = (await ok('GET', `/api/ventas/ventas/${v.id}/detalle`)).data;
    const idCoca = detBD.find((f) => f.ProductoId === 1 && f.Tipo === 1).Id;
    const idLeche = detBD.find((f) => f.ProductoId === 3 && f.Tipo === 1).Id;
    await ok('PUT', `/api/ventas/ventas/${v.id}`, { ...cab, TipoVenta: 0, Facturado: true, fechaOriginal: hoy, Descuento: 0, detalle: [
        { Id: idCoca, Cantidad: 18 }, { Id: idLeche, eliminado: true }, ...detBD.filter((f) => f.Tipo === 2).map((f) => ({ Id: f.Id })),
    ] });
    ll = ultimaLlamada('MAM_Ventas', 2).params;
    assert.deepEqual(ll['@VentaDetalleType'].filas.map((f) => [f.Id, f.estado]).sort((a, b) => a[0] - b[0]),
        detBD.map((f) => [f.Id, f.Id === idCoca ? 2 : f.Id === idLeche ? -1 : 1]).sort((a, b) => a[0] - b[0]));
    assert.equal(ll['@VentaPagos'].filas.length, 0, 'credito: sin pagos');
    assert.equal(ll['@Facturado'], 1);

    // reglas
    assert.equal((await api('POST', '/api/ventas/ventas', { ...cab, detalle: [{ nuevo: true, ProductoId: 4, Cantidad: 9999 }], pago: { MontoBs: 1e6 } })).status, 400, 'stock');
    assert.match((await api('POST', '/api/ventas/ventas', { ...cab, detalle: [{ nuevo: true, ProductoId: 1, Cantidad: 1, Precio: 0.01 }], pago: { MontoBs: 100 } })).mensaje, /Precio De Costo/, 'precio < costo');
    assert.match((await api('POST', '/api/ventas/ventas', { ...cab, FechaVenta: ayer, detalle: [{ nuevo: true, ProductoId: 1, Cantidad: 1 }], pago: { MontoBs: 100 } })).mensaje, /Caja Chica/, 'sin caja abierta');
    const viejas = (await ok('GET', `/api/ventas/ventas?desde=${new Date(Date.now() - 25 * 864e5).toISOString().slice(0, 10)}&hasta=${ayer}`)).data;
    assert.match((await api('DELETE', `/api/ventas/ventas/${viejas[0].Id}?fecha=${viejas[0].FechaVenta}`)).mensaje, /ANTERIORES/, 'sin ModificarRegistroAnteriores');
    await ok('DELETE', `/api/ventas/ventas/${v.id}?fecha=${hoy}`);
});

test('PROFORMAS (Kaily): MAM_Proforma sin pagos, cotiza sin stock y sin caja', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    const ayer = new Date(Date.now() - 28 * 3600e3).toISOString().slice(0, 10);
    // sin caja abierta ayer y producto 5 sin stock: igual se graba (TipoPrograma 2)
    const p = await ok('POST', '/api/ventas/proformas', { SucursalId: 1, FechaVenta: ayer, ClienteId: 1, TipoVenta: 0, PrecioId: 2, Glosa: 'cotizacion',
        detalle: [{ nuevo: true, Tipo: 1, ProductoId: 5, Cantidad: 50 }, { nuevo: true, Tipo: 1, ProductoId: 1, Cantidad: 9999 }] });
    let ll = ultimaLlamada('MAM_Proforma', 1).params;
    assert.equal(ll['@TipoVenta'], 1, 'ProformaInsertar envia TipoVenta 1');
    assert.equal(ll['@FechaVencimientoCredito'], hoy.replace(/-/g, '/'), 'vencimiento = hoy (Now.Date)');
    assert.ok(!('@VentaPagos' in ll) && !('@Facturado' in ll) && !('@EstadoPedido' in ll), 'sin pagos ni campos de distribucion');
    assert.equal(ll['@VentaDetalleType'].filas.length, 2);
    const lista = (await ok('GET', `/api/ventas/proformas?desde=${ayer}&hasta=${ayer}`)).data;
    assert.ok(lista.some((x) => Number(x.Id) === Number(p.id)));
    const det = (await ok('GET', `/api/ventas/proformas/${p.id}/detalle`)).data;
    await ok('PUT', `/api/ventas/proformas/${p.id}`, { SucursalId: 1, FechaVenta: ayer, ClienteId: 1, PrecioId: 2, fechaOriginal: ayer,
        detalle: [{ Id: det[0].Id, Cantidad: 60 }, { Id: det[1].Id, eliminado: true }] });
    ll = ultimaLlamada('MAM_Proforma', 2).params;
    assert.deepEqual(ll['@VentaDetalleType'].filas.map((f) => f.estado).sort(), [-1, 2]);
    assert.ok((await ok('GET', `/api/ventas/proformas/${p.id}/recibo?fecha=${ayer}`)).data.detalle.length >= 1);
    await ok('DELETE', `/api/ventas/proformas/${p.id}?fecha=${ayer}`);
});

test('CIERRE CAJERO (Kaily): abrir caja, cerrar con pendientes y billetes, totales del servidor', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    const ini = (await ok('GET', '/api/caja/cierre-cajero/inicio')).data;
    assert.equal(ini.cortes.length, 11, 'las 11 denominaciones del escritorio');
    const a = await ok('POST', '/api/caja/cierre-cajero', { SucursalId: 1, MontoInicial: 100, TipoCambio: 6.96, Observacion: 'apertura' });
    let ll = ultimaLlamada('MAM_CierreCajero', 1).params;
    assert.deepEqual([ll['@EstadoCaja'], ll['@MontoInicial'], ll['@PersonalId']], [1, 100, 1], 'abre la caja con el personal del usuario');
    const pend = (await ok('GET', `/api/caja/cierre-cajero/${a.id}/detalle?fecha=${hoy}&pendiente=1`)).data;
    assert.ok(pend.ventas.length && pend.cobranzas.length && pend.movimientos.length);
    // 1 billete de 200 Bs + 1 de 10 $us
    const cortes = ini.cortes.map(([b, d]) => ({ CantidadBs: b === 200 ? 1 : 0, CantidadDolares: d === 10 ? 1 : 0 }));
    await ok('PUT', `/api/caja/cierre-cajero/${a.id}`, { fechaOriginal: hoy, SucursalId: 1, MontoInicial: 100, TipoCambio: 6.96, Observacion: 'cierre', cortes, TotalCaja: 1 });
    ll = ultimaLlamada('MAM_CierreCajero', 2).params;
    assert.equal(ll['@EstadoCaja'], 0, 'cierra la caja');
    assert.equal(ll['@TotalCaja'], 100 + 150 + 40 - 10, 'Inicial + Ventas + Cobranzas + Ingresos - Egresos');
    assert.equal(ll['@TotalEfectivo'], Math.round((200 + 10 * 6.96) * 100) / 100);
    assert.equal(ll['@Diferencia'], Math.round((200 + 69.6 - 280) * 100) / 100);
    assert.equal(ll['@CorteBilletes'].filas.length, 11);
    assert.deepEqual(ll['@ModuloId'].filas.map((f) => f[2]), [1, 2, 3], 'ventas = 1, cobranzas = 2, movimientos = 3');
    assert.match((await api('PUT', `/api/caja/cierre-cajero/${a.id}`, { fechaOriginal: hoy, SucursalId: 1, cortes })).mensaje, /Cerrada/, 'una caja cerrada no se modifica');
    await ok('DELETE', `/api/caja/cierre-cajero/${a.id}?fecha=${hoy}`);
});

test('ADMINISTRAR CAJA (Kaily): gastos por btnGastos, caja general con saldo inicial y caja detallado', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    const hace60 = new Date(Date.now() - 60 * 864e5).toISOString().slice(0, 10);
    assert.ok((await ok('GET', '/api/caja/ingresos-egresos')).data.length, 'mismo formulario de ingresos y egresos con btnGastos');
    const g = (await ok('GET', `/api/caja/reportes/caja-general?desde=${hoy}&hasta=${hoy}`)).data;
    assert.equal(g.filas[0].TipoMovimiento, 'Saldo Inicial');
    assert.equal(g.filas[0].Saldo, -120, 'saldo anterior: el egreso de 120 de hace 40 dias');
    assert.equal(g.filas.at(-1).Saldo, -120 + 500, 'saldo acumulado');
    assert.equal(g.saldoCajaGeneral, 1250.5, 'TotalCajaGeneral de Configuracion');
    const cajas = (await ok('GET', '/api/caja/reportes/caja-detallado/cajas')).data;
    assert.ok(cajas.some((c) => c.Id === -1), 'opcion Todos');
    assert.ok((await ok('GET', `/api/caja/reportes/caja-detallado?desde=${hace60}&hasta=${hoy}&caja=-1`)).data.filas.length >= 2);
    assert.deepEqual(Object.keys(ultimaLlamada('MAM_CajaIngresoEgreso', 13).params).sort(), ['@CajaId', '@FechaF', '@FechaI', '@tipo'].sort(), 'sin @usuario, como VB');
});

test('PERMISOS: solo Ver = acceso completo; si marco alguna accion se respetan', () => {
    const { _acciones } = require('../src/middleware/permisos');
    assert.deepEqual(_acciones({ Ver: 1, Insertar: 0, Modificar: 0, Eliminar: 0 }), { insertar: true, modificar: true, eliminar: true }, 'como el escritorio');
    assert.deepEqual(_acciones({ Ver: 1, Insertar: false, Modificar: true, Eliminar: false }), { insertar: false, modificar: true, eliminar: false });
    assert.deepEqual(_acciones({ Ver: 1, Insertar: 1, Modificar: 1, Eliminar: 1 }), { insertar: true, modificar: true, eliminar: true });
});

test('INGRESOS/EGRESOS (traspaso, caja chica, bloqueos) y sus reportes; sin lo exclusivo de distribucion', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    const ayer = new Date(Date.now() - 28 * 3600e3).toISOString().slice(0, 10);
    const c = (await ok('GET', '/api/distribucion/ingresos-egresos/combos')).data;
    assert.ok(c.cajas.length && c.tipos.length && c.personal);
    const base = { Fecha: hoy, Descripcion: 'x', Monto: 50, CajaId: 1, SucursalId: 1, CajaTipoMovimientoId: 2 };
    assert.match((await api('POST', '/api/distribucion/ingresos-egresos', { ...base, Monto: 0 })).mensaje, /Mayor a 0/);
    const n = await ok('POST', '/api/distribucion/ingresos-egresos', base);
    let ll = ultimaLlamada('MAM_CajaIngresoEgreso', 1).params;
    assert.equal(ll['@IngresoEgreso'], 0, 'el motivo define ingreso/egreso'); assert.equal(ll['@PersonalId'], 1, 'personal de la sesion');
    // caja chica: solo si hay cierre de cajero abierto en esa fecha
    assert.match((await api('POST', '/api/distribucion/ingresos-egresos', { ...base, CajaId: 2, Fecha: ayer })).mensaje, /Caja Chica/);
    await ok('POST', '/api/distribucion/ingresos-egresos', { ...base, CajaId: 2 });
    // traspaso: 2 movimientos enlazados
    assert.match((await api('POST', '/api/distribucion/ingresos-egresos', { ...base, CajaTipoMovimientoId: 9, CajaIdDestino: 1 })).mensaje, /diferente Al Origen/);
    const t = await ok('POST', '/api/distribucion/ingresos-egresos', { ...base, CajaTipoMovimientoId: 9, CajaIdDestino: 3 });
    ll = ultimaLlamada('MAM_CajaIngresoEgreso', 1).params;
    assert.deepEqual([ll['@CajaId'], ll['@IngresoEgreso'], ll['@CajaTipoMovimientoId'], ll['@CajaIdDestino'], String(ll['@CajaIngresoEgresoIdDestino'])], [3, 1, 10, 1, String(t.id)]);
    assert.match((await api('PUT', `/api/distribucion/ingresos-egresos/${t.id}`, base)).mensaje, /TRASPASO/);
    assert.match((await api('DELETE', '/api/distribucion/ingresos-egresos/2')).mensaje, /CIERRE DE CAJA/);
    await ok('PUT', `/api/distribucion/ingresos-egresos/${n.id}`, { ...base, Monto: 75 });
    await ok('DELETE', `/api/distribucion/ingresos-egresos/${n.id}`);
    // reportes de ingresos/egresos (Kaily: Administrar Caja). Los de distribucion (gastos chofer, historico,
    // auditoria, productos por cliente, incremento) y la asignacion de zonas ya no existen
    const anual = (await ok('GET', `/api/distribucion/reportes/ingresos-egresos-anual?anio=${hoy.slice(0, 4)}`)).data;
    assert.deepEqual(anual.filas.map((f) => f.Concepto), ['INGRESOS', 'EGRESOS', 'SALDO']);
    assert.equal(ultimaLlamada('MAM_CajaIngresoEgreso', 9) !== undefined, true);
    await ok('GET', `/api/distribucion/reportes/ingresos-egresos-detallado?desde=${hoy}&hasta=${hoy}`);
    for (const r of ['gastos-chofer', 'historico-clientes', 'auditoria-ventas', 'productos-por-cliente', 'historial-incremento']) {
        assert.equal((await api('GET', `/api/distribucion/reportes/${r}?desde=${hoy}&hasta=${hoy}`)).status, 404, `${r} no existe en Kaily`);
    }
    for (const u of ['/api/distribucion/zonas/4', '/api/distribucion/despachos', '/api/distribucion/conciliaciones', '/api/distribucion/cierre-caja/conciliaciones', '/api/ventas/asignaciones/pendientes', '/api/incremento']) {
        assert.equal((await api('GET', u)).status, 404, `${u} no existe en Kaily`);
    }
});

test('INVENTARIO: movimientos (stock, estados, ingreso sin limite), kardex, saldos y monitoreo', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    const hace30 = new Date(Date.now() - 30 * 864e5).toISOString().slice(0, 10);
    const c = (await ok('GET', '/api/inventario/movimientos/combos')).data;
    assert.equal(c.lote, false);
    const prods = (await ok('GET', '/api/inventario/movimientos/productos/1')).data;
    const p1 = prods.find((x) => x.stock > 5);
    const cab = { ConceptoId: 3, DepositoId: 1, FechaDocumento: hoy, Observacion: 'prueba' };
    const p2 = prods.find((x) => x.Id !== p1.Id);
    assert.match((await api('POST', '/api/inventario/movimientos', { ...cab, detalle: [{ ProductoId: p1.Id, Cantidad: p1.stock + 1 }] })).mensaje, /MAYOR A LA QUE EXISTE/);
    assert.match((await api('POST', '/api/inventario/movimientos', { ...cab, detalle: [] })).mensaje, /DETALLE/);
    // Kaily: las filas en 0 no se graban (Agregar todos); al menos una con cantidad
    assert.match((await api('POST', '/api/inventario/movimientos', { ...cab, detalle: [{ ProductoId: p1.Id, Cantidad: 0 }] })).mensaje, /AL MENOS UN PRODUCTO/);
    assert.match((await api('POST', '/api/inventario/movimientos', { ...cab, detalle: [{ ProductoId: p1.Id, Cantidad: -1 }] })).mensaje, /MAYOR A 0/i, 'salida no acepta negativos');
    await ok('POST', '/api/inventario/movimientos', { ...cab, ConceptoId: 4, detalle: [{ ProductoId: p1.Id, Cantidad: p1.stock + 1000 }] }); // ingreso: sin limite
    let ll = ultimaLlamada('MAM_Movimientos', 1).params;
    assert.deepEqual([ll['@DepositoIdDestino'], ll['@IdMovimientoDestino']], [0, 0], 'movimiento normal: destino 0 / 0');
    // ingreso: reseteo a cero con cantidades negativas; la fila en 0 se descarta
    const rs = await ok('POST', '/api/inventario/movimientos', { ...cab, ConceptoId: 4, Observacion: 'RESETEO DE INVENTARIO A CERO', detalle: [{ ProductoId: p1.Id, Cantidad: -p1.stock }, { ProductoId: p2.Id, Cantidad: 0 }] });
    ll = ultimaLlamada('MAM_Movimientos', 1).params;
    assert.deepEqual(ll['@detalle'].filas.map((f) => f.Cantidad), [-p1.stock]);
    await ok('DELETE', `/api/inventario/movimientos/${rs.id}`);
    const n = await ok('POST', '/api/inventario/movimientos', { ...cab, detalle: [{ ProductoId: p1.Id, Cantidad: 2 }] });
    ll = ultimaLlamada('MAM_Movimientos', 1).params;
    assert.equal(ll['@Estado'], 1); assert.equal(ll['@detalle'].filas[0].estado, 0);
    assert.equal(ll['@detalle'].columnas.length, 10, 'detalle de Kaily: sin CantidadCajas, stockFinal ni conversion');
    const det = (await ok('GET', `/api/inventario/movimientos/${n.id}/detalle`)).data;
    assert.ok(!('CantidadCajas' in det[0]));
    await ok('PUT', `/api/inventario/movimientos/${n.id}`, { ...cab, detalle: [{ Id: det[0].Id, Cantidad: 3 }] });
    assert.deepEqual(ultimaLlamada('MAM_Movimientos', 2).params['@detalle'].filas.map((f) => f.estado), [2]);
    assert.ok(!('@DepositoIdDestino' in ultimaLlamada('MAM_Movimientos', 2).params), 'L_prMovimientoActualizar no envia destino');
    // comprobante (ReporteVenta -> Reporte_Movimiento, @tipo 17)
    const comp = (await ok('GET', `/api/inventario/movimientos/${n.id}/comprobante`)).data;
    assert.equal(ultimaLlamada('MAM_Movimientos', 17).params['@id'], n.id);
    assert.ok(comp.filas.length && comp.filas[0].Cantidad === 3 && comp.cab.Id === n.id);
    await ok('DELETE', `/api/inventario/movimientos/${n.id}`);
    assert.equal(ultimaLlamada('MAM_Movimientos', -1).params['@ConceptoId'], 3, 'L_prBorrarRegistroMovimiento envia el ConceptoId');
    // TRASPASO (8): destino distinto; salida en el origen + ingreso (7) en el destino enlazado
    assert.match((await api('POST', '/api/inventario/movimientos', { ...cab, ConceptoId: 8, DepositoIdDestino: 1, detalle: [{ ProductoId: p1.Id, Cantidad: 1 }] })).mensaje, /DIFERENTE AL ORIGEN/);
    const t = await ok('POST', '/api/inventario/movimientos', { ...cab, ConceptoId: 8, DepositoIdDestino: 2, detalle: [{ ProductoId: p1.Id, Cantidad: 1 }] });
    const ins = mock.__llamadas.filter((x) => x.sp === 'MAM_Movimientos' && x.tipo === 1).slice(-2).map((x) => x.params);
    assert.deepEqual([ins[0]['@ConceptoId'], ins[0]['@DepositoId'], ins[0]['@DepositoIdDestino'], ins[0]['@IdMovimientoDestino']], [8, 1, 2, 0]);
    assert.deepEqual([ins[1]['@ConceptoId'], ins[1]['@DepositoId'], ins[1]['@DepositoIdDestino'], Number(ins[1]['@IdMovimientoDestino'])], [7, 2, 1, Number(t.id)]);
    assert.match((await api('PUT', `/api/inventario/movimientos/${t.id}`, { ...cab, ConceptoId: 8, detalle: [] })).mensaje, /TRASPASO/, 'un traspaso no se modifica');
    await ok('DELETE', `/api/inventario/movimientos/${t.id}`);
    assert.equal(ultimaLlamada('MAM_Movimientos', -1).params['@ConceptoId'], 8);
    assert.ok(!(await ok('GET', '/api/inventario/movimientos')).data.some((x) => Number(x.Id) === Number(t.idDestino)), 'se borra tambien el ingreso del destino');
    // sin ModificarRegistroAnteriores no se modifica ni elimina un movimiento de dias anteriores
    const viejo = (await ok('GET', '/api/inventario/movimientos')).data.find((x) => x.fdoc < hoy);
    if (!c.modificarAnteriores) assert.match((await api('DELETE', `/api/inventario/movimientos/${viejo.Id}`)).mensaje, /ANTERIORES/);
    // kardex: saldo inicial + acumulado
    const k = (await ok('GET', `/api/inventario/reportes/kardex?desde=${hace30}&hasta=${hoy}&producto=1&deposito=1`)).data;
    assert.equal(k.filas[0].concepto, 'Saldo Inicial');
    assert.equal(k.resumen.final, k.filas[k.filas.length - 1].saldo);
    assert.equal(k.resumen.final, k.resumen.inicial + k.resumen.ingresos - k.resumen.salidas);
    // ventas/mermas son salida, compras/ingresos son ingreso (no invertido)
    assert.ok(k.filas.filter((f) => f.concepto === 'SALIDA').every((f) => f.salida > 0 && f.ingreso === 0));
    assert.ok(k.filas.filter((f) => f.concepto === 'INGRESO').every((f) => f.ingreso > 0 && f.salida === 0));
    await ok('GET', '/api/inventario/reportes/saldos?mayor0=1');
    await ok('GET', '/api/inventario/reportes/saldos?deposito=1');
    assert.equal(ultimaLlamada('MAM_Movimientos', 11).params['@DepositoId'], 1);
    await ok('GET', '/api/inventario/reportes/saldos-valorados');
    const g = (await ok('GET', `/api/inventario/reportes/general?desde=${hace30}&hasta=${hoy}&deposito=1`)).data;
    assert.ok(g.length && g[0].SaldoFinal !== undefined);
    const mon = (await ok('GET', '/api/inventario/reportes/monitoreo')).data;
    assert.ok(Array.isArray(mon.sinStock) && Array.isArray(mon.stockMinimo) && mon.vencidos.length === 1);
});

test('MAPA DE CLIENTES: todos (@tipo 6) y por zona (@tipo 7)', async () => {
    const z = (await ok('GET', '/api/mapa/clientes/zonas')).data;
    assert.ok(z.length);
    const todos = (await ok('GET', '/api/mapa/clientes')).data;
    assert.ok(todos.length && 'Latitud' in todos[0]);
    await ok('GET', `/api/mapa/clientes?zona=${z[0].Id}`);
    assert.equal(ultimaLlamada('MAM_Clientes', 7).params['@ZonaId'], z[0].Id);
});

test('ALMACENES: alta, modificacion, validaciones y no eliminar la primera', async () => {
    const lista = (await ok('GET', '/api/inventario/almacenes')).data;
    assert.ok(lista.length);
    const dep = (await ok('GET', '/api/inventario/almacenes/depositos')).data[0].Id;
    assert.match((await api('POST', '/api/inventario/almacenes', { NombreAlmacen: '', DepositoId: dep })).mensaje, /Nombre/i);
    assert.match((await api('POST', '/api/inventario/almacenes', { NombreAlmacen: 'X', DepositoId: 9999 })).mensaje, /Deposito/i);
    const n = await ok('POST', '/api/inventario/almacenes', { NombreAlmacen: 'sucursal prueba', DepositoId: dep, estado: 1 });
    assert.equal(ultimaLlamada('MAM_Almacenes', 1).params['@NombreAlmacen'], 'SUCURSAL PRUEBA');
    await ok('PUT', `/api/inventario/almacenes/${n.id}`, { NombreAlmacen: 'sucursal 2', DepositoId: dep, estado: 0 });
    assert.equal(ultimaLlamada('MAM_Almacenes', 2).params['@estado'], 0);
    assert.match((await api('DELETE', `/api/inventario/almacenes/${lista[0].Id}`)).mensaje, /Todas Las Sucursales/);
    await ok('DELETE', `/api/inventario/almacenes/${n.id}`);
});

test('COMPRAS (Kaily): lista por fechas, unidades sin cajas ni notas, totales del servidor y bloqueos', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    const hace30 = new Date(Date.now() - 30 * 864e5).toISOString().slice(0, 10);
    // L_prListarGeneralComprasFiltro: @tipo 3 con @desde / @hasta
    const lista = (await ok('GET', `/api/compras/compras?desde=${hace30}&hasta=${hoy}`)).data;
    assert.ok(lista.length >= 3);
    let ll = ultimaLlamada('MAM_Compras', 3).params;
    assert.deepEqual([ll['@desde'], ll['@hasta']], [hace30.replace(/-/g, '/'), hoy.replace(/-/g, '/')]);
    assert.ok(!('NroNotaCompra' in lista[0]));
    assert.equal((await ok('GET', '/api/compras/compras')).data.length, 0, 'sin fechas = hoy (tbDesde / tbHasta = Now.Date)');
    const c = (await ok('GET', '/api/compras/compras/combos')).data;
    assert.equal(c.modificarAnteriores, false);
    const prods = (await ok('GET', `/api/compras/compras/productos/${c.sucursales[0].Id}`)).data;
    assert.ok('industria' in prods[0] && 'NombreCategoria' in prods[0] && !('conversion' in prods[0]));
    const cab = { AlmacenId: c.sucursales[0].Id, ProveedorId: c.proveedores[0].Id, FechaTransaccion: hoy, TipoVenta: 1, Glosa: 'prueba', Descuento: 10 };
    assert.match((await api('POST', '/api/compras/compras', { ...cab, ProveedorId: 999, detalle: [{ ProductoId: prods[0].Id, CantidadCompra: 6 }] })).mensaje, /Proveedor/);
    assert.match((await api('POST', '/api/compras/compras', { ...cab, detalle: [] })).mensaje, /DETALLE/);
    const p = prods[0];
    const n = await ok('POST', '/api/compras/compras', { ...cab, NroNotaCompra: 'X', detalle: [{ ProductoId: p.Id, CantidadCompra: 2, PrecioCosto: 50, PrecioVenta: 60, TotalCompra: 1 }] });
    ll = ultimaLlamada('MAM_Compras', 1).params;
    assert.ok(!('@NroNotaCompra' in ll) && !('@NroNotaRecepcion' in ll), 'ComprasInsertar de Kaily sin notas');
    assert.equal(ll['@TotalCompra'], 90); // 50 x 2 unidades - 10 (el total lo calcula el servidor)
    const f0 = ll['@detalle'].filas[0];
    assert.deepEqual([f0.estado, f0.CantidadCompra, f0.PorcentajeIncremento, f0.CantidadIncremento, f0.Cantidad, f0.TotalCompra], [0, 2, 0, 0, 2, 100]);
    assert.equal(ll['@Moneda'], 1); assert.equal(ll['@EmpresaId'], 1);
    assert.match((await api('POST', '/api/compras/compras', { ...cab, TipoVenta: 0, FechaVencimientoCredito: '2000-01-01', detalle: [{ ProductoId: p.Id, CantidadCompra: 1 }] })).mensaje, /vencimiento/i);
    const det = (await ok('GET', `/api/compras/compras/${n.id}/detalle`)).data;
    assert.equal(det[0].CantidadCompra, 2);
    await ok('PUT', `/api/compras/compras/${n.id}`, { ...cab, fechaOriginal: hoy, Descuento: 0, detalle: [{ Id: det[0].Id, CantidadCompra: 3, PrecioCosto: 40, PrecioVenta: 60 }, { nuevo: true, ProductoId: prods[1].Id, CantidadCompra: 1 }] });
    ll = ultimaLlamada('MAM_Compras', 2).params;
    assert.deepEqual(ll['@detalle'].filas.map((f) => f.estado), [2, 0]);
    assert.equal(ll['@TotalCompra'], 120 + prods[1].PrecioCosto);
    assert.match((await ok('GET', `/api/compras/compras/${n.id}/nota?fecha=${hoy}`)).data.compra.Glosa, /prueba/);
    await ok('GET', `/api/compras/compras/${n.id}/comprobante`);
    await ok('DELETE', `/api/compras/compras/${n.id}?fecha=${hoy}`);
    // Global_ModificarRegistro = 0: no se modifica ni elimina una compra de dias anteriores
    const vieja = lista.find((x) => x.transaccion === 0);
    assert.match((await api('PUT', `/api/compras/compras/${vieja.Id}`, { ...cab, fechaOriginal: vieja.FechaTransaccion, detalle: [] })).mensaje, /ANTERIORES/);
    const conPagos = lista.find((x) => x.transaccion === 1);
    assert.match((await api('DELETE', `/api/compras/compras/${conPagos.Id}?fecha=${conPagos.FechaTransaccion}`)).mensaje, /ANTERIORES/);
    assert.equal((await api('GET', '/api/compras/reportes/historico')).status, 404, 'Kaily no tiene reportes de compras');
});

test('CUENTAS POR PAGAR: deudas, pago con saldo maximo, estado de cuenta y eliminar pago', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    const deudas = (await ok('GET', '/api/compras/cuentas-pagar/deudas')).data;
    assert.ok(deudas.length && 'Compra' in deudas[0]);
    await ok('GET', '/api/compras/cuentas-pagar/pendientes');
    await ok('GET', '/api/compras/cuentas-pagar/pagados');
    await ok('GET', '/api/compras/cuentas-pagar/todas');
    const per = (await ok('GET', '/api/compras/cuentas-pagar/personal')).data.lista[0].Id;
    const d = deudas[0];
    assert.match((await api('POST', `/api/compras/cuentas-pagar/${d.Id}/pagos`, { Monto: d.Restante + 1, FechaPago: hoy, PersonalId: per })).mensaje, /MAXIMO/);
    const r = await ok('POST', `/api/compras/cuentas-pagar/${d.Id}/pagos`, { Monto: 100, FechaPago: hoy, PersonalId: per, Glosa: 'abono' });
    assert.equal(ultimaLlamada('MAM_CreditosCompras', 4).params['@CreditoCompraId'], d.Id);
    const pagos = (await ok('GET', `/api/compras/cuentas-pagar/${d.Id}/pagos`)).data;
    assert.equal(pagos.length, 1);
    await ok('GET', `/api/compras/cuentas-pagar/${d.Id}/estado`);
    await ok('DELETE', `/api/compras/cuentas-pagar/pagos/${r.id}`);
});

test('CUENTAS POR COBRAR (Kaily): sin preventista, cobro con Caja Chica abierta y bloqueos al eliminar pagos', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    const ayer = new Date(Date.now() - 28 * 3600e3).toISOString().slice(0, 10);
    const deudas = (await ok('GET', '/api/ventas/cuentas-cobrar/deudas')).data;
    assert.ok(deudas.length && !('NombrePersonal' in deudas[0]));
    assert.ok('NombrePersonal' in (await ok('GET', '/api/ventas/cuentas-cobrar/pendientes')).data[0], 'Vendedor en Creditos Pendientes');
    await ok('GET', '/api/ventas/cuentas-cobrar/cobrados');
    await ok('GET', `/api/ventas/cuentas-cobrar/cobrados?desde=${ayer}&hasta=${hoy}`);
    const todas = (await ok('GET', '/api/ventas/cuentas-cobrar/todas')).data;
    const per = (await ok('GET', '/api/ventas/cuentas-cobrar/personal')).data.lista[0].Id;
    const d = deudas[0];
    assert.match((await api('POST', `/api/ventas/cuentas-cobrar/${d.Id}/pagos`, { Monto: d.Restante + 1, FechaPago: hoy, PersonalId: per })).mensaje, /MAXIMO/);
    // ButtonX4: no hay Caja Chica abierta con la fecha de ayer
    assert.match((await api('POST', `/api/ventas/cuentas-cobrar/${d.Id}/pagos`, { Monto: 1, FechaPago: ayer, PersonalId: per })).mensaje, /Caja Chica/);
    const r = await ok('POST', `/api/ventas/cuentas-cobrar/${d.Id}/pagos`, { Monto: 10, FechaPago: hoy, PersonalId: per, Glosa: 'abono' });
    assert.equal(ultimaLlamada('MAM_CreditosVentas', 4).params['@CreditoVentaId'], d.Id);
    const pagos = (await ok('GET', `/api/ventas/cuentas-cobrar/${d.Id}/pagos`)).data;
    assert.ok(pagos.some((p) => Number(p.Id) === Number(r.id) && p.CierreModulo === 0));
    assert.ok(todas.some((x) => x.Id === d.Id));
    await ok('DELETE', `/api/ventas/cuentas-cobrar/${d.Id}/pagos/${r.id}`);
    assert.equal(ultimaLlamada('MAM_CreditosVentas', 7).params['@TransaccionVentaId'], Number(r.id));
});

test('REPORTES DE VENTAS (Kaily): sin filtro de cliente y sin los reportes de distribucion', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    await ok('GET', `/api/ventas/reportes/ventas-realizadas?desde=${hoy}&hasta=${hoy}&cliente=1`);
    assert.ok(ultimaLlamada('MAM_ReporteVentas', 1), 'con cliente igual usa @tipo 1 (Kaily no tiene 13 / 14)');
    await ok('GET', `/api/ventas/reportes/ventas-realizadas?desde=${hoy}&hasta=${hoy}&vendedor=1`);
    assert.equal(ultimaLlamada('MAM_ReporteVentas', 2).params['@IdPersonal'], 1);
    await ok('GET', `/api/ventas/reportes/ventas-costos?desde=${hoy}&hasta=${hoy}`);
    await ok('GET', `/api/ventas/reportes/ventas-costos?desde=${hoy}&hasta=${hoy}&vendedor=1`);
    await ok('GET', `/api/ventas/reportes/productos-mas-vendidos?desde=${hoy}&hasta=${hoy}`);
    for (const r of ['ventas-productos', 'historico-cobros', 'ventas-categoria', 'aux/clientes']) {
        assert.equal((await api('GET', `/api/ventas/reportes/${r}?desde=${hoy}&hasta=${hoy}`)).status, 404, r);
    }
});

test('PLANILLA: conceptos fijos, contratos (vigente, conceptos por estado), registrar y conceptos variables', async () => {
    // Conceptos fijos
    const ops = (await ok('GET', '/api/planilla/conceptos-fijos/operaciones')).data;
    assert.match((await api('POST', '/api/planilla/conceptos-fijos', { NombreConcepto: 'x', OperacionId: ops[0].Id, TipoConcepto: 1, Porcentaje: 0 })).mensaje, /Porcentaje/);
    const cf = await ok('POST', '/api/planilla/conceptos-fijos', { NombreConcepto: 'rc-iva', OperacionId: ops[1].Id, TipoConcepto: 1, Porcentaje: 13, Estado: 1 });
    assert.equal(ultimaLlamada('MAM_ConceptosFijos', 1).params['@NombreConcepto'], 'RC-IVA');
    await ok('PUT', `/api/planilla/conceptos-fijos/${cf.id}`, { NombreConcepto: 'rc-iva', OperacionId: ops[1].Id, TipoConcepto: 0, Porcentaje: 0, Estado: 1 });
    await ok('DELETE', `/api/planilla/conceptos-fijos/${cf.id}`);
    // Contratos
    const c = (await ok('GET', '/api/planilla/contratos/combos')).data;
    const lista = (await ok('GET', '/api/planilla/contratos')).data;
    const base = { TipoContratoId: c.tipos[0].cnnum, Cargo: c.cargos[0].cnnum, SueldoBase: 4000, InicioContrato: '2026-01-01', FinContrato: '2026-12-31', Indefinido: 0 };
    assert.match((await api('POST', '/api/planilla/contratos', { ...base, PersonalId: lista[0].PersonalId })).mensaje, /Vigente/);
    assert.match((await api('POST', '/api/planilla/contratos', { ...base, PersonalId: 5, FinContrato: '2025-01-01' })).mensaje, /Finalizacion/);
    const n = await ok('POST', '/api/planilla/contratos', { ...base, PersonalId: 5, Indefinido: 1, conceptos: [{ nuevo: true, ConceptoId: c.conceptos[0].Id }] });
    let ll = ultimaLlamada('MAM_Contratos', 1).params;
    assert.ok(ll['@FinContrato'] > '2070'); // indefinido = hoy + 50 años
    assert.equal(ll['@DetalleContratoConcepto'].filas[0].estado, 0);
    const con = (await ok('GET', `/api/planilla/contratos/${n.id}/conceptos`)).data;
    await ok('PUT', `/api/planilla/contratos/${n.id}`, { ...base, PersonalId: 5, conceptos: [{ Id: con[0].Id, eliminado: true }, { nuevo: true, ConceptoId: c.conceptos[1].Id }] });
    assert.deepEqual(ultimaLlamada('MAM_Contratos', 2).params['@DetalleContratoConcepto'].filas.map((f) => f.estado), [-1, 0]);
    // Registrar planilla (año / mes = codigos de libreria)
    const per = (await ok('GET', '/api/planilla/planilla/combos')).data;
    const q = `mes=${per.meses[0].cnnum}&anio=${per.anios[0].cnnum}`;
    const sin = (await ok('GET', `/api/planilla/planilla/registrar/sin-planilla?${q}`)).data;
    assert.ok(sin.length >= 4);
    const r = await ok('POST', '/api/planilla/planilla/registrar', { mes: per.meses[0].cnnum, anio: per.anios[0].cnnum });
    assert.equal(r.registrados, sin.length);
    const pc = mock.__llamadas.filter((x) => x.sp === 'MAM_Planilla' && x.tipo === 1)[0].params;
    assert.equal(pc['@Mes'], per.meses[0].cnnum);
    assert.ok(pc['@PlanillaConcepto'].filas.every((f) => f.TipoConcepto === 1 && f.estado === 0));
    assert.equal((await ok('GET', `/api/planilla/planilla/registrar/sin-planilla?${q}`)).data.length, 0);
    // Listar + conceptos variables
    const pl = (await ok('GET', `/api/planilla/planilla/listar?${q}`)).data;
    assert.equal(pl.length, sin.length);
    const p1 = pl[0];
    assert.ok((await ok('GET', `/api/planilla/planilla/listar/${p1.PlanillaId}/conceptos?tipo=1`)).data.length);
    const vars = (await ok('GET', '/api/planilla/planilla/listar/variables')).data;
    assert.match((await api('POST', `/api/planilla/planilla/listar/${p1.PlanillaId}/variables`, { ConceptoId: vars[0].Id, Monto: p1.SueldoNeto + 1 })).mensaje, /Sueldo Neto/);
    await ok('POST', `/api/planilla/planilla/listar/${p1.PlanillaId}/variables`, { ConceptoId: vars[0].Id, Monto: 100, Descripcion: 'anticipo' });
    assert.equal(ultimaLlamada('MAM_Planilla', 7).params['@PlanillaConcepto'].filas.at(-1).Monto, -100);
    const v0 = (await ok('GET', `/api/planilla/planilla/listar/${p1.PlanillaId}/conceptos?tipo=0`)).data;
    assert.equal(v0.length, 1);
    await ok('DELETE', `/api/planilla/planilla/listar/${p1.PlanillaId}/variables/${v0[0].Id}`);
    const bol = (await ok('GET', `/api/planilla/planilla/listar/${p1.PlanillaId}/boleta`)).data;
    assert.ok(bol.filas.length > 0 && bol.filas[0].HaberBasicoBase !== undefined && bol.filas[0].img === undefined, 'boleta: columnas del SP sin binarios');
    assert.equal(bol.empresa.nombre, 'DISTRAL REGIONAL- POTOSI');
    assert.equal(bol.imagenEmpresa, 'Default.jpg');
    await ok('GET', '/api/planilla/planilla/listar/reporte?todos=1');
    await ok('DELETE', `/api/planilla/contratos/${n.id}`);
});

test('RENDIMIENTO PERSONAL: actividad del rango (orden de la lista) y grafico mensual con MAM_ReporteVentas 8', async () => {
    const hoy = new Date(Date.now() - 4 * 3600e3).toISOString().slice(0, 10);
    const hace30 = new Date(Date.now() - 30 * 864e5).toISOString().slice(0, 10);
    const per = (await ok('GET', '/api/ventas/reportes/rendimiento/personal')).data;
    // actividad: una sola llamada al @tipo 8 con TODO el personal marcado, y solo agregados por persona
    const n0 = mock.__llamadas.length;
    const act = (await ok('GET', `/api/ventas/reportes/rendimiento/actividad?desde=${hace30}&hasta=${hoy}`)).data;
    const del8 = mock.__llamadas.slice(n0).filter((l) => l.sp === 'MAM_ReporteVentas' && l.tipo === 8);
    assert.equal(del8.length, 1, 'una sola consulta al SP');
    const tvp = Object.entries(del8[0].params).find(([k]) => /personaltype/i.test(k))[1];
    assert.ok(tvp.filas.every((f) => f.chk), 'todo el personal marcado');
    assert.equal(act.personal.length, per.length);
    assert.ok(act.personal.every((x) => Object.keys(x).join() === 'id,total,pedidos'), 'sin detalle de ventas');
    assert.ok(act.personal.some((x) => x.total > 0), 'el simulador tiene ventas en el rango');
    assert.equal(act.aviso, null);
    // grafico mensual: mismos totales que la actividad para el mismo rango
    const ids = act.personal.map((x) => x.id);
    const men = (await ok('GET', `/api/ventas/reportes/rendimiento/mensual?desde=${hace30}&hasta=${hoy}&personal=${ids.join(',')}`)).data;
    assert.equal(men.aviso, null);
    for (const s of men.series) {
        const tot = Math.round(s.valores.reduce((a, v) => a + v.total, 0) * 100) / 100;
        assert.ok(Math.abs(tot - act.personal.find((x) => x.id === s.id).total) < 0.05, `totales de ${s.nombre}`);
    }
    assert.equal((await api('GET', `/api/ventas/reportes/rendimiento/actividad?desde=${hoy}&hasta=${hace30}`)).ok, false, 'rango invertido');
});

test('BUSCAR POR ID: columna en cualquier mayuscula o repetida por un JOIN (mssql la vuelve arreglo)', () => {
    const v = require('../src/utils/validar');
    assert.equal(v.buscarId([{ ID: 7, FechaIngreso: 'x' }, { ID: 3056, FechaIngreso: 'y' }], 3056).FechaIngreso, 'y');
    assert.equal(v.campo(v.buscarId([{ id: [3056, 12], fechaingreso: 'z' }], 3056), 'FechaIngreso'), 'z');
    assert.deepEqual(v.proyectar([{ Id: [5, 9], Nombre: 'A' }], ['Id', 'Nombre']), [{ Id: 5, Nombre: 'A' }]);
});

test('DOSIFICACION y TIPO DE CAMBIO (Kaily): MAM_Dosificacion 1/2/-1 y MAM_Ventas @tipo 7', async () => {
    const lista = (await ok('GET', '/api/configuracion/dosificacion')).data;
    assert.ok(lista.length && lista[0].FechaInicio === '2026-01-01', 'fechas como yyyy-mm-dd');
    assert.ok((await ok('GET', '/api/configuracion/dosificacion/combos')).data.sucursales.length);
    const d = { SucursalId: 1, NumeroAutorizacion: '123', UltimoNroFactura: 0, Llave: 'K', FechaInicio: '2026-02-01', FechaLimite: '2026-12-31', Nota01: 'a', Nota02: '', Estado: 1, TipoComputarizado: 0 };
    const r = await ok('POST', '/api/configuracion/dosificacion', d);
    let ll = ultimaLlamada('MAM_Dosificacion', 1).params;
    assert.deepEqual([ll['@FechaInicio'], ll['@FechaLimite'], ll['@Estado'], ll['@TipoComputarizado']], ['2026/02/01', '2026/12/31', 1, 0], 'fechas yyyy/MM/dd e IIf(sw, 1, 0)');
    assert.match((await api('POST', '/api/configuracion/dosificacion', { ...d, UltimoNroFactura: -1 })).mensaje, /Monto Valido/);
    assert.match((await api('POST', '/api/configuracion/dosificacion', { ...d, SucursalId: '' })).mensaje, /Sucursal/);
    await ok('PUT', `/api/configuracion/dosificacion/${r.id}`, { ...d, UltimoNroFactura: 50 });
    assert.equal(ultimaLlamada('MAM_Dosificacion', 2).params['@UltimoNroFactura'], 50);
    await ok('DELETE', `/api/configuracion/dosificacion/${r.id}`);
    // btnTipoCambio
    assert.equal((await ok('GET', '/api/tipo-cambio')).data.monto, 6.96);
    assert.match((await api('POST', '/api/tipo-cambio', { monto: 0 })).mensaje, /Valor Correcto/);
    await ok('POST', '/api/tipo-cambio', { monto: 6.97 });
    ll = ultimaLlamada('MAM_Ventas', 7).params;
    assert.equal(ll['@TipoCambio'], 6.97);
    assert.equal((await ok('GET', '/api/tipo-cambio')).data.monto, 6.97, 'L_Validar_Usuario / ventas leen el nuevo tipo de cambio');
    await ok('POST', '/api/tipo-cambio', { monto: 6.96 });
});

test('PRODUCTOS (Kaily): codigo externo unico (@tipo 11) y reporte de precios (@tipo 8)', async () => {
    const form = (campos) => { const f = new FormData(); Object.entries(campos).forEach(([k, val]) => f.append(k, String(val))); return f; };
    const base = { NombreProducto: 'PEPSI 2L', CodigoExterno: 'P-200', CodigoBarras: '', StockMinimo: 1, Conversion: 1, estado: 1,
        CategoriaId: 1, EmpresaId: 1, ProveedorId: 1, MarcaId: 1, AttributoId: 1, FamiliaId: 1, UnidadVentaId: 1, UnidadMaximaId: 1 };
    // Nuevo con un codigo que ya tiene el producto 1
    const rep = await api('POST', '/api/configuracion/productos', null, { form: form(base) });
    assert.equal(rep.status, 400);
    assert.match(rep.mensaje, /mismo codigo Externo= P-200 En el Producto 1/);
    assert.equal(ultimaLlamada('MAM_Productos', 11).params['@CodigoExterno'], 'P-200');
    // Nuevo con codigo libre; al modificar, su propio codigo (1 coincidencia) no es error
    const r = await ok('POST', '/api/configuracion/productos', null, { form: form({ ...base, CodigoExterno: 'P-951' }) });
    await ok('PUT', `/api/configuracion/productos/${r.id}`, null, { form: form({ ...base, CodigoExterno: 'P-951', NombreProducto: 'PEPSI 3L' }) });
    // sin codigo externo no se consulta
    const antes = mock.__llamadas.filter((l) => l.sp === 'MAM_Productos' && l.tipo === 11).length;
    await ok('PUT', `/api/configuracion/productos/${r.id}`, null, { form: form({ ...base, CodigoExterno: '' }) });
    assert.equal(mock.__llamadas.filter((l) => l.sp === 'MAM_Productos' && l.tipo === 11).length, antes);
    await ok('DELETE', `/api/configuracion/productos/${r.id}`);

    // Reporte de precios: combos de AyudaMultiCategoria y filtro por categorias elegidas
    const c = (await ok('GET', '/api/configuracion/productos/reporte-precios/combos')).data;
    assert.ok(c.sucursales.length && c.categoriasPrecio[0].Nombre && c.categorias.length === 3);
    assert.equal(ultimaLlamada('sp_Mam_Precios', 9) !== undefined && ultimaLlamada('sp_Mam_Precios', 10) !== undefined, true, 'Filter = @tipo 9, categorias = @tipo 10');
    assert.match((await api('GET', '/api/configuracion/productos/reporte-precios?categoriaPrecio=2&sucursal=1&categorias=')).mensaje, /Al Menos una Categoria/);
    const sin = (await ok('GET', '/api/configuracion/productos/reporte-precios?categoriaPrecio=2&sucursal=1&categorias=2&imagenes=0')).data;
    const lr = ultimaLlamada('MAM_Productos', 8).params;
    assert.deepEqual([lr['@CategoriaPrecio'], lr['@SucursalId']], [2, 1]);
    assert.ok(sin.filas.length && sin.filas.every((f) => Number(f.CategoriaId) === 2 && !('Rutaimg' in f)), 'solo la categoria elegida (ExisteCategoria)');
    assert.ok(sin.cabecera.Nombre, 'nombre de la empresa (ObtenerImagenEmpresa)');
    const con = (await ok('GET', '/api/configuracion/productos/reporte-precios?categoriaPrecio=2&sucursal=1&categorias=1,2&imagenes=1')).data;
    assert.ok(con.filas.some((f) => f.Rutaimg === 'producto_1.jpg'), 'solo el nombre del archivo, el navegador lo pide a /api/imagenes');
});

test('CLIENTE RAPIDO (Kaily FormularioCliente): MAM_Clientes @tipo 8 desde ventas y proformas', async () => {
    const cb = (await ok('GET', '/api/ventas/ventas/clientes/combos')).data;
    assert.ok(cb.tiposDocumento.length && cb.categoriasPrecio.length);
    const d = { NombreCliente: 'CLIENTE MOSTRADOR NUEVO', TipoDocumento: cb.tiposDocumento[0].cnnum, NroDocumento: '1234567', Telefono: '70011122', PrecioCategoriaId: cb.categoriasPrecio[0].Id };
    assert.match((await api('POST', '/api/ventas/ventas/clientes', { ...d, NombreCliente: ' ' })).mensaje, /Nombre Cliente/);
    assert.match((await api('POST', '/api/ventas/ventas/clientes', { ...d, PrecioCategoriaId: '' })).mensaje, /Precios/);
    const r = (await ok('POST', '/api/ventas/ventas/clientes', d)).data;
    assert.ok(r.Id > 0 && r.NombreCliente === d.NombreCliente);
    const ll = ultimaLlamada('MAM_Clientes', 8).params;
    assert.deepEqual([ll['@RazonSocial'], ll['@nit'], ll['@Telefono'], ll['@Observacion']], [d.NombreCliente, d.NroDocumento, d.Telefono, ''], 'RazonSocial = Nombre, Nit = Nro Documento');
    assert.ok(!('@usuario' in ll), 'el escritorio no envia @usuario');
    assert.ok((await ok('GET', `/api/ventas/ventas/clientes?q=${encodeURIComponent('MOSTRADOR NUEVO')}`)).data.some((x) => Number(x.Id) === Number(r.Id)));
    assert.ok((await ok('POST', '/api/ventas/proformas/clientes', { ...d, NombreCliente: 'CLIENTE PROFORMA NUEVO' })).data.Id > 0, 'Tec_Proformas usa el mismo formulario');
});

test('PANEL y MENU (Kaily): ventas por sucursal, sin pedidos por asignar ni mapa, sin modulo Distribucion', async () => {
    const d = (await ok('GET', '/api/dashboard')).data;
    assert.ok(d.ventas && !('pendientesAsignar' in d.alertas), 'sin MAM_Ventas @tipo 7 de distribucion');
    const ll = [...mock.__llamadas].reverse().find((x) => x.sp === 'MAM_Ventas' && x.tipo === 3).params;
    assert.ok('@SucursalId' in ll, 'L_prListarVentasGeneralFiltroFecha con la sucursal del usuario');
    assert.ok(Array.isArray((await ok('GET', '/api/dashboard/detalle/pedidos')).data.filas));
    assert.equal((await api('GET', '/api/dashboard/detalle/asignar')).status, 400);
    assert.equal((await api('GET', '/api/dashboard/pedidos-mapa')).status, 404);
    const s = await ok('GET', '/api/auth/sesion');
    const ids = s.data.modulos.flatMap((m) => m.programas.map((p) => p.IdPrograma.toUpperCase()));
    for (const p of ['BTNDESPACHO', 'BTNCONCILIACIONES', 'BTNCIERRECAJA', 'BTNASIGNACIONPEDIDOS', 'BTNASIGNACIONZONA']) assert.ok(!ids.includes(p), `${p} no aparece`);
});

test('ningun llamado rompio el contrato de los SP del escritorio', () => {
    assert.ok(mock.__llamadas.length > 40, `se validaron ${mock.__llamadas.length} llamadas`);
});

test('Inventario (FormularioStock): modulo fijo, menu "Ingreso y Salida De Productos" y stock general (MAM_Movimientos @tipo 18)', async () => {
    const s = await ok('GET', '/api/auth/sesion');
    const nombres = s.data.modulos.map((m) => m.cndesc1);
    assert.equal(nombres[nombres.length - 1], 'INVENTARIO');
    assert.ok(s.data.modulos.at(-1).programas.some((p) => p.IdPrograma === 'FORMULARIOSTOCK'));
    const ing = s.data.modulos.find((m) => m.programas.some((p) => p.IdPrograma === 'BTINVMOVIMIENTO'));
    assert.equal(ing.cndesc1, 'INGRESO Y SALIDA DE PRODUCTOS');
    const c = (await ok('GET', '/api/inventario/stock/combos')).data;
    // Kaily: L_fnListarCategoriaPrecio = sp_Mam_Precios @tipo 8 (el contrato no lo distingue de 9 y 10: mismos parametros)
    assert.equal([...mock.__llamadas].reverse().find((x) => x.sp === 'sp_Mam_Precios').tipo, 8, 'combo Categoria Precio con @tipo 8');
    assert.ok(!mock.__llamadas.slice(-3).some((x) => x.sp === 'MAM_Compras' && x.tipo === 12), 'Kaily ya no carga proveedores');
    assert.ok(c.categorias.length && !('proveedores' in c));
    const todos = (await ok('GET', `/api/inventario/stock?categoria=${c.categorias[0].Id}`)).data;
    const ll = ultimaLlamada('MAM_Movimientos', 18);
    assert.ok(!('@ProveedorId' in ll.params), 'L_prListarProductosTodosInventario solo con @CategoriaPrecio');
    assert.equal(ll.params['@CategoriaPrecio'], c.categorias[0].Id);
    assert.ok(todos.length > 0 && 'stockGeneral' in todos[0] && 'precio' in todos[0] && 'stock' in todos[0] && 'unidad' in todos[0]);
    assert.equal((await api('GET', '/api/inventario/stock?categoria=x')).status, 400);
});
