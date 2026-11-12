// =====================================================================
//  Panel principal (dashboard) - SOLO LECTURA
//  Fuentes (mismos SP que el escritorio):
//    MAM_Ventas @tipo 3 (L_prListarVentasGeneralFiltroFecha, sucursal del usuario) -> ventas, montos, vendedores, horas, tendencia
//    MAM_CreditosVentas @tipo 1                      -> cuentas por cobrar
//    MAM_CreditosCompras @tipo 1                     -> cuentas por pagar
//    MAM_ReporteVentas @tipo 6 / 10 / 11             -> productos mas vendidos, stock 0, stock minimo
//  Kaily (punto de venta): sin pedidos pendientes de asignar ni mapa de pedidos (MAM_Ventas 7 / 23 no existen).
//  Destacados del mes: mismo MAM_Ventas @tipo 3 (desde el 1ro del mes) y MAM_ReporteVentas @tipo 6 del mes.
//  Al navegador se envian SOLO agregados (totales, rankings), nunca las listas completas.
// =====================================================================
const express = require('express');
const L = require('../negocio/AccesoLogica');
const { asyncHandler, ErrorNegocio } = require('../middleware/errores');

const router = express.Router();
const DIAS_TENDENCIA = 30;

// Opcional: limitar el panel a ciertos roles (DASHBOARD_ROLES=1,3 en .env)
router.use((req, res, next) => {
    const roles = String(process.env.DASHBOARD_ROLES || '').split(',').map((x) => x.trim()).filter(Boolean);
    if (roles.length && !roles.includes(String(req.sesion.gi_userRol))) {
        return res.status(403).json({ ok: false, mensaje: 'Su rol no tiene acceso al panel de indicadores.' });
    }
    next();
});

// ---------- utilidades ----------
const dos = (n) => String(n).padStart(2, '0');
const iso = (d) => `${d.getUTCFullYear()}-${dos(d.getUTCMonth() + 1)}-${dos(d.getUTCDate())}`;
const fmtVB = (d) => `${d.getUTCFullYear()}/${dos(d.getUTCMonth() + 1)}/${dos(d.getUTCDate())}`;
const sumar = (d, n) => new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate() + n));
const inicioMes = (d) => new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), 1));
const num = (v) => { const n = Number(v); return Number.isFinite(n) ? n : 0; };
const r2 = (n) => Math.round(n * 100) / 100;

// Lee una columna sin distinguir mayusculas (los SP mezclan "Nombrecliente" / "NombreCliente")
function lector(filas) {
    const mapa = {};
    for (const k of Object.keys(filas[0] || {})) mapa[k.toLowerCase()] = k;
    return (f, ...nombres) => {
        for (const n of nombres) {
            const k = mapa[n.toLowerCase()];
            if (k !== undefined && f[k] !== null && f[k] !== undefined) return f[k];
        }
        return null;
    };
}

// Ventas como Tec_Ventas de Kaily: L_prListarVentasGeneralFiltroFecha("MAM_Ventas", desde, hasta, Global_Sucursal)
function ventasRango(sesion, desde, hasta) {
    const suc = Number(sesion?.gi_userSuc);
    return L.L_prListarVentasGeneralFiltroFecha('MAM_Ventas', fmtVB(desde), fmtVB(hasta), Number.isFinite(suc) ? suc : -1);
}

// Hoy en Bolivia (UTC-4) aunque el servidor tenga otra zona
function hoyBolivia() {
    const ahora = new Date(Date.now() - 4 * 3600 * 1000);
    return new Date(Date.UTC(ahora.getUTCFullYear(), ahora.getUTCMonth(), ahora.getUTCDate()));
}

function leerFecha(txt) {
    if (!txt) return hoyBolivia();
    const m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(String(txt));
    if (!m) throw new ErrorNegocio('Fecha no valida (use AAAA-MM-DD)');
    const d = new Date(Date.UTC(+m[1], +m[2] - 1, +m[3]));
    if (Number.isNaN(d.getTime())) throw new ErrorNegocio('Fecha no valida');
    return d;
}

// FechaVenta -> 'AAAA-MM-DD' (mssql entrega DATE como medianoche UTC)
function diaDe(v) {
    if (!v) return null;
    if (v instanceof Date) return iso(v);
    const m = /^(\d{4})[-/](\d{2})[-/](\d{2})/.exec(String(v));
    if (m) return `${m[1]}-${m[2]}-${m[3]}`;
    const m2 = /^(\d{2})\/(\d{2})\/(\d{4})/.exec(String(v));
    return m2 ? `${m2[3]}-${m2[2]}-${m2[1]}` : null;
}

// HoraRegistro -> 0..23 (puede venir 'HH:mm', 'HH:mm:ss' o un TIME/DATETIME)
function horaDe(v) {
    if (v === null || v === undefined || v === '') return null;
    if (v instanceof Date) return v.getUTCHours();
    const m = /(\d{1,2}):(\d{2})/.exec(String(v));
    return m ? Math.min(23, Number(m[1])) : null;
}

async function seguro(promesa, nombre, errores) {
    try {
        return await promesa;
    } catch (e) {
        console.error(`[dashboard] ${nombre}:`, e.message);
        errores.push(nombre);
        return null;
    }
}

// ---------- ventas ----------
function resumenVentas(filas, fechaSel, idVendedor) {
    const c = lector(filas);
    const dia = iso(fechaSel);
    const inicioVentana = iso(sumar(fechaSel, -(DIAS_TENDENCIA - 1)));
    const ayer = iso(sumar(fechaSel, -1));
    const pedidos = filas.map((f) => ({
        dia: diaDe(c(f, 'FechaVenta')),
        hora: horaDe(c(f, 'HoraRegistro')),
        total: num(c(f, 'TotalVenta')),
        vendedorId: c(f, 'PersonalId'),
        vendedor: String(c(f, 'Personal', 'NombrePersonal') ?? 'SIN VENDEDOR').trim(),
        clienteId: c(f, 'ClienteId'),
        cliente: String(c(f, 'NombreCliente', 'Cliente') ?? '').trim(),
        tipo: String(c(f, 'TVenta') ?? (num(c(f, 'TipoVenta')) === 1 ? 'CONTADO' : 'CREDITO')).trim().toUpperCase(),
        estado: String(c(f, 'NombreEstado') ?? 'SIN ESTADO').trim().toUpperCase(),
    })).filter((p) => p.dia);

    // Lista de vendedores (de los ultimos 30 dias) para el filtro
    const vend = new Map();
    for (const p of pedidos) if (p.vendedorId !== null && p.vendedorId !== undefined) vend.set(String(p.vendedorId), p.vendedor);
    const vendedores = [...vend].map(([id, nombre]) => ({ id, nombre })).sort((a, b) => a.nombre.localeCompare(b.nombre, 'es'));

    const anulado = (p) => /ANUL/.test(p.estado);
    const validos = pedidos.filter((p) => !anulado(p));
    const delVendedor = idVendedor ? validos.filter((p) => String(p.vendedorId) === String(idVendedor)) : validos;

    const deDia = delVendedor.filter((p) => p.dia === dia);
    const deAyer = delVendedor.filter((p) => p.dia === ayer);
    const monto = (arr) => r2(arr.reduce((s, p) => s + p.total, 0));

    // Tendencia diaria
    const tendencia = [];
    for (let i = DIAS_TENDENCIA - 1; i >= 0; i--) {
        const d = iso(sumar(fechaSel, -i));
        const arr = delVendedor.filter((p) => p.dia === d);
        tendencia.push({ fecha: d, pedidos: arr.length, monto: monto(arr) });
    }
    const previos = tendencia.slice(0, -1);
    const diasConVenta = previos.filter((t) => t.pedidos > 0).length || 1;
    const promMonto = r2(previos.reduce((s, t) => s + t.monto, 0) / diasConVenta);
    const promPedidos = r2(previos.reduce((s, t) => s + t.pedidos, 0) / diasConVenta);

    // Por hora: dia seleccionado vs promedio de los dias anteriores con ventas
    const porHora = Array.from({ length: 24 }, (_, h) => ({ hora: h, pedidos: 0, monto: 0, promedio: 0 }));
    for (const p of deDia) if (p.hora !== null) { porHora[p.hora].pedidos++; porHora[p.hora].monto += p.total; }
    const anteriores = delVendedor.filter((p) => p.dia !== dia && p.dia >= inicioVentana && p.dia <= dia && p.hora !== null);
    for (const p of anteriores) porHora[p.hora].promedio += p.total;
    porHora.forEach((h) => { h.monto = r2(h.monto); h.promedio = r2(h.promedio / diasConVenta); });

    // Ranking de vendedores del dia (siempre todos, para comparar)
    const rk = new Map();
    for (const p of validos.filter((x) => x.dia === dia)) {
        const k = String(p.vendedorId);
        const r = rk.get(k) || { id: k, nombre: p.vendedor, pedidos: 0, monto: 0 };
        r.pedidos++; r.monto += p.total;
        rk.set(k, r);
    }
    const ranking = [...rk.values()].map((r) => ({ ...r, monto: r2(r.monto) })).sort((a, b) => b.monto - a.monto);
    const masPedidos = [...ranking].sort((a, b) => b.pedidos - a.pedidos)[0] || null;

    const agrupar = (arr, k) => {
        const m = new Map();
        for (const p of arr) { const x = m.get(p[k]) || { nombre: p[k], pedidos: 0, monto: 0 }; x.pedidos++; x.monto += p.total; m.set(p[k], x); }
        return [...m.values()].map((x) => ({ ...x, monto: r2(x.monto) })).sort((a, b) => b.pedidos - a.pedidos);
    };
    const todosDia = (idVendedor ? pedidos.filter((p) => String(p.vendedorId) === String(idVendedor)) : pedidos).filter((p) => p.dia === dia);

    return {
        vendedores,
        kpis: {
            pedidos: deDia.length,
            monto: monto(deDia),
            ticket: deDia.length ? r2(monto(deDia) / deDia.length) : 0,
            clientes: new Set(deDia.map((p) => p.clienteId)).size,
            pedidosAyer: deAyer.length,
            montoAyer: monto(deAyer),
            promMonto,
            promPedidos,
            anulados: todosDia.filter(anulado).length,
        },
        porHora,
        tendencia,
        ranking: ranking.slice(0, 8),
        masPedidos,
        estados: agrupar(todosDia, 'estado'),
        tipos: agrupar(deDia, 'tipo'),
        mes: resumenMes(validos, delVendedor, fechaSel),
    };
}

// ---------- destacados del mes (del 1ro del mes a la fecha elegida) ----------
function resumenMes(validos, delVendedor, fechaSel) {
    const desde = iso(inicioMes(fechaSel));
    const hasta = iso(fechaSel);
    const enMes = (p) => p.dia >= desde && p.dia <= hasta;
    const top = (arr, clave, nombre) => {
        const m = new Map();
        for (const p of arr) {
            const k = String(p[clave] ?? p[nombre]);
            const x = m.get(k) || { id: p[clave] === null || p[clave] === undefined ? null : String(p[clave]), nombre: p[nombre] || 'SIN NOMBRE', pedidos: 0, monto: 0 };
            x.pedidos++; x.monto += p.total;
            m.set(k, x);
        }
        return [...m.values()].map((x) => ({ ...x, monto: r2(x.monto) })).sort((a, b) => b.monto - a.monto || b.pedidos - a.pedidos);
    };
    const mesTodos = validos.filter(enMes);
    const mesFiltro = delVendedor.filter(enMes);
    const clientes = top(mesFiltro, 'clienteId', 'cliente');
    const vendedores = top(mesTodos, 'vendedorId', 'vendedor');
    return {
        desde, hasta,
        monto: r2(mesFiltro.reduce((s, p) => s + p.total, 0)),
        montoTodos: r2(mesTodos.reduce((s, p) => s + p.total, 0)),
        pedidos: mesFiltro.length,
        clientesAtendidos: clientes.length,
        clientes: clientes.slice(0, 5),
        vendedores: vendedores.slice(0, 5),
        masPedidos: [...vendedores].sort((a, b) => b.pedidos - a.pedidos)[0] || null,
    };
}

// ---------- creditos (cobrar / pagar) ----------
function resumenCreditos(filas, campoNombre, filtroVendedor) {
    if (!filas) return null;
    const c = lector(filas);
    let arr = filas.map((f) => ({
        nombre: String(c(f, campoNombre) ?? '').trim() || 'SIN NOMBRE',
        saldo: num(c(f, 'Restante')),
        monto: num(c(f, 'Monto')),
        mora: num(c(f, 'DiasMora')),
        preventista: String(c(f, 'NombrePersonal') ?? '').trim(),
    })).filter((x) => x.saldo > 0.009);
    if (filtroVendedor) arr = arr.filter((x) => x.preventista.toUpperCase() === filtroVendedor.toUpperCase());
    const porNombre = new Map();
    for (const x of arr) {
        const r = porNombre.get(x.nombre) || { nombre: x.nombre, saldo: 0, mora: 0, documentos: 0 };
        r.saldo += x.saldo; r.mora = Math.max(r.mora, x.mora); r.documentos++;
        porNombre.set(x.nombre, r);
    }
    const tramos = [
        { tramo: 'Al día', saldo: 0 }, { tramo: '1–30 días', saldo: 0 }, { tramo: '31–60 días', saldo: 0 }, { tramo: '+60 días', saldo: 0 },
    ];
    for (const x of arr) tramos[x.mora <= 0 ? 0 : x.mora <= 30 ? 1 : x.mora <= 60 ? 2 : 3].saldo += x.saldo;
    return {
        total: r2(arr.reduce((s, x) => s + x.saldo, 0)),
        vencido: r2(arr.filter((x) => x.mora > 0).reduce((s, x) => s + x.saldo, 0)),
        documentos: arr.length,
        documentosVencidos: arr.filter((x) => x.mora > 0).length,
        cuentas: porNombre.size,
        tramos: tramos.map((t) => ({ ...t, saldo: r2(t.saldo) })),
        top: [...porNombre.values()].sort((a, b) => b.saldo - a.saldo).slice(0, 5).map((x) => ({ ...x, saldo: r2(x.saldo) })),
    };
}

function resumenProductos(filas, limite = 6) {
    if (!filas) return null;
    const c = lector(filas);
    const m = new Map();
    for (const f of filas) {
        const k = String(c(f, 'IdProducto', 'ProductoId', 'NombreProducto'));
        const x = m.get(k) || { nombre: String(c(f, 'NombreProducto', 'Producto') ?? '').trim(), cantidad: 0, total: 0 };
        x.cantidad += num(c(f, 'Cantidad')); x.total += num(c(f, 'Total'));
        m.set(k, x);
    }
    return [...m.values()].sort((a, b) => b.total - a.total).slice(0, limite).map((x) => ({ ...x, cantidad: r2(x.cantidad), total: r2(x.total) }));
}

router.get('/', asyncHandler(async (req, res) => {
    const fecha = leerFecha(req.query.fecha);
    const idVendedor = req.query.vendedor ? String(Number.parseInt(req.query.vendedor, 10)) : '';
    if (idVendedor === 'NaN') throw new ErrorNegocio('Vendedor no valido');
    const desde30 = sumar(fecha, -(DIAS_TENDENCIA - 1));
    const desdeMes = inicioMes(fecha);
    const desde = desdeMes < desde30 ? desdeMes : desde30; // una sola lectura cubre tendencia y mes
    const errores = [];

    const [ventas, cxc, cxp, productos, productosMes, stock0, stockMin] = await Promise.all([
        seguro(ventasRango(req.sesion, desde, fecha), 'ventas', errores),
        seguro(L.L_prListarPagosPendientesClientes(), 'cuentas por cobrar', errores),
        seguro(L.L_prListarPagosPendientes(), 'cuentas por pagar', errores),
        seguro(L.ReporteVentasProductosMasVendido(fmtVB(fecha), fmtVB(fecha)), 'productos', errores),
        seguro(L.ReporteVentasProductosMasVendido(fmtVB(desdeMes), fmtVB(fecha)), 'productos del mes', errores),
        seguro(L.L_prListarProductosConStock0(), 'stock en cero', errores),
        seguro(L.L_prListarProductosConStockMinimo(), 'stock minimo', errores),
    ]);

    const v = ventas ? resumenVentas(ventas, fecha, idVendedor) : null;
    const nombreVend = idVendedor && v ? (v.vendedores.find((x) => x.id === idVendedor)?.nombre || '') : '';

    res.set('Cache-Control', 'no-store');
    res.json({
        ok: true,
        data: {
            fecha: iso(fecha),
            vendedor: idVendedor || null,
            generado: new Date().toISOString(),
            ventas: v,
            cobrar: resumenCreditos(cxc, 'Nombrecliente', nombreVend),
            pagar: resumenCreditos(cxp, 'NombreProveedor'),
            productos: resumenProductos(productos),
            productosMes: resumenProductos(productosMes, 5),
            alertas: {
                stock0: stock0 ? stock0.length : null,
                stockMinimo: stockMin ? stockMin.length : null,
            },
            errores,
        },
    });
}));

// ---------- detalle al hacer clic (SOLO LECTURA, columnas minimas) ----------
const LIMITE_DETALLE = 500;
const TRAMOS = [[-Infinity, 0], [1, 30], [31, 60], [61, Infinity]];

function horaTxt(v) {
    if (v === null || v === undefined || v === '') return '';
    if (v instanceof Date) return `${dos(v.getUTCHours())}:${dos(v.getUTCMinutes())}`;
    const m = /(\d{1,2}):(\d{2})/.exec(String(v));
    return m ? `${dos(m[1])}:${m[2]}` : '';
}

const DETALLES = {
    async pedidos(q, sesion) {
        const fecha = leerFecha(q.fecha);
        const ini = q.mes === '1' ? inicioMes(fecha) : fecha;
        const filas = await ventasRango(sesion, ini, fecha);
        const c = lector(filas);
        const dia = iso(fecha);
        const diaIni = iso(ini);
        const hora = q.hora !== undefined && q.hora !== '' ? Number.parseInt(q.hora, 10) : null;
        let arr = filas.map((f) => ({
            id: c(f, 'Id', 'VentaId'),
            fecha: diaDe(c(f, 'FechaVenta')),
            hora: horaTxt(c(f, 'HoraRegistro')),
            h: horaDe(c(f, 'HoraRegistro')),
            cliente: String(c(f, 'NombreCliente', 'Cliente') ?? '').trim(),
            clienteId: c(f, 'ClienteId'),
            vendedorId: c(f, 'PersonalId'),
            vendedor: String(c(f, 'Personal', 'NombrePersonal') ?? '').trim(),
            tipo: String(c(f, 'TVenta') ?? (num(c(f, 'TipoVenta')) === 1 ? 'CONTADO' : 'CREDITO')).trim().toUpperCase(),
            estado: String(c(f, 'NombreEstado') ?? '').trim().toUpperCase(),
            total: r2(num(c(f, 'TotalVenta'))),
            _dia: diaDe(c(f, 'FechaVenta')),
        })).filter((p) => p._dia >= diaIni && p._dia <= dia);
        if (q.cliente) arr = arr.filter((p) => String(p.clienteId) === String(q.cliente));
        if (q.vendedor) arr = arr.filter((p) => String(p.vendedorId) === String(q.vendedor));
        if (q.estado) arr = arr.filter((p) => p.estado === String(q.estado).toUpperCase());
        else if (q.validos === '1') arr = arr.filter((p) => !/ANUL/.test(p.estado));
        if (q.tipo) arr = arr.filter((p) => p.tipo === String(q.tipo).toUpperCase());
        if (hora !== null && Number.isFinite(hora)) arr = arr.filter((p) => p.h === hora);
        arr.sort((a, b) => (b.fecha + b.hora).localeCompare(a.fecha + a.hora));
        const conFecha = q.mes === '1';
        return {
            columnas: [
                { k: 'id', t: 'N°', tipo: 'num' }, ...(conFecha ? [{ k: 'fecha', t: 'Fecha', tipo: 'fecha' }] : []), { k: 'hora', t: 'Hora' }, { k: 'cliente', t: 'Cliente', ancho: true },
                { k: 'vendedor', t: 'Vendedor' }, { k: 'tipo', t: 'Pago' }, { k: 'estado', t: 'Estado', chip: true }, { k: 'total', t: 'Total', tipo: 'bs' },
            ],
            filas: arr.map(({ id, fecha: fe, hora: hh, cliente, vendedor, tipo, estado, total }) => ({ id, ...(conFecha ? { fecha: fe } : {}), hora: hh, cliente, vendedor, tipo, estado, total })),
            total: r2(arr.reduce((s, p) => s + p.total, 0)),
        };
    },
    async cobrar(q) { return creditosDetalle(await L.L_prListarPagosPendientesClientes(), 'Nombrecliente', q, true); },
    async pagar(q) { return creditosDetalle(await L.L_prListarPagosPendientes(), 'NombreProveedor', q, false); },
    async productosMes(q) {
        const fecha = leerFecha(q.fecha);
        const arr = resumenProductos(await L.ReporteVentasProductosMasVendido(fmtVB(inicioMes(fecha)), fmtVB(fecha)), LIMITE_DETALLE + 1);
        return {
            columnas: [{ k: 'puesto', t: 'N°', tipo: 'num' }, { k: 'nombre', t: 'Producto', ancho: true }, { k: 'cantidad', t: 'Cantidad', tipo: 'num' }, { k: 'total', t: 'Total', tipo: 'bs' }],
            filas: arr.map((p, i) => ({ puesto: i + 1, ...p })),
            total: r2(arr.reduce((s, p) => s + p.total, 0)),
        };
    },
    async stock0() { return productosDetalle(await L.L_prListarProductosConStock0()); },
    async stockmin() { return productosDetalle(await L.L_prListarProductosConStockMinimo()); },
};

function creditosDetalle(filas, campoNombre, q, conPreventista) {
    const c = lector(filas);
    let arr = filas.map((f) => ({
        nombre: String(c(f, campoNombre) ?? '').trim() || 'SIN NOMBRE',
        preventista: String(c(f, 'NombrePersonal') ?? '').trim(),
        monto: r2(num(c(f, 'Monto'))),
        saldo: r2(num(c(f, 'Restante'))),
        mora: num(c(f, 'DiasMora')),
    })).filter((x) => x.saldo > 0.009);
    if (conPreventista && q.vendedorNombre) arr = arr.filter((x) => x.preventista.toUpperCase() === String(q.vendedorNombre).toUpperCase());
    const t = q.tramo !== undefined && q.tramo !== '' ? Number(q.tramo) : null;
    if (t !== null && TRAMOS[t]) arr = arr.filter((x) => Math.max(0, x.mora) >= Math.max(0, TRAMOS[t][0]) && Math.max(0, x.mora) <= TRAMOS[t][1]);
    if (q.vencido === '1') arr = arr.filter((x) => x.mora > 0);
    if (q.nombre) arr = arr.filter((x) => x.nombre.toUpperCase() === String(q.nombre).toUpperCase());
    arr.sort((a, b) => b.mora - a.mora || b.saldo - a.saldo);
    const columnas = [{ k: 'nombre', t: conPreventista ? 'Cliente' : 'Proveedor', ancho: true }];
    if (conPreventista) columnas.push({ k: 'preventista', t: 'Preventista' });
    columnas.push({ k: 'monto', t: 'Monto', tipo: 'bs' }, { k: 'saldo', t: 'Saldo', tipo: 'bs' }, { k: 'mora', t: 'Mora', tipo: 'mora' });
    return {
        columnas,
        filas: arr.map((x) => (conPreventista ? x : { nombre: x.nombre, monto: x.monto, saldo: x.saldo, mora: x.mora })),
        total: r2(arr.reduce((s, x) => s + x.saldo, 0)),
    };
}

function productosDetalle(filas) {
    const c = lector(filas);
    const arr = filas.map((f) => {
        const stock = c(f, 'Stock', 'Saldo', 'StockActual', 'Cantidad');
        const minimo = c(f, 'StockMinimo');
        return {
            codigo: String(c(f, 'CodigoExterno', 'yfcprod', 'Id', 'IdProducto') ?? ''),
            producto: String(c(f, 'NombreProducto', 'Producto', 'yfcdprod1') ?? '').trim(),
            stock: stock === null ? null : r2(num(stock)),
            minimo: minimo === null ? null : r2(num(minimo)),
        };
    });
    const columnas = [{ k: 'codigo', t: 'Código' }, { k: 'producto', t: 'Producto', ancho: true }];
    if (arr.some((x) => x.stock !== null)) columnas.push({ k: 'stock', t: 'Stock', tipo: 'num' });
    if (arr.some((x) => x.minimo !== null)) columnas.push({ k: 'minimo', t: 'Mínimo', tipo: 'num' });
    return { columnas, filas: arr.map((x) => Object.fromEntries(columnas.map((k) => [k.k, x[k.k]]))) };
}

router.get('/detalle/:tipo', asyncHandler(async (req, res) => {
    const fn = DETALLES[req.params.tipo];
    if (!fn) throw new ErrorNegocio('Detalle no disponible');
    const q = {};
    for (const k of ['fecha', 'vendedor', 'vendedorNombre', 'estado', 'tipo', 'hora', 'tramo', 'vencido', 'nombre', 'validos', 'mes', 'cliente']) {
        if (req.query[k] !== undefined) q[k] = String(req.query[k]).slice(0, 80);
    }
    if (q.vendedor && !/^\d+$/.test(q.vendedor)) throw new ErrorNegocio('Vendedor no valido');
    if (q.hora && !/^\d{1,2}$/.test(q.hora)) throw new ErrorNegocio('Hora no valida');
    if (q.cliente && !/^\d+$/.test(q.cliente)) throw new ErrorNegocio('Cliente no valido');
    const r = await fn(q, req.sesion);
    const truncado = r.filas.length > LIMITE_DETALLE;
    res.set('Cache-Control', 'no-store');
    res.json({ ok: true, data: { ...r, cantidad: r.filas.length, filas: r.filas.slice(0, LIMITE_DETALLE), truncado } });
}));

module.exports = router;
