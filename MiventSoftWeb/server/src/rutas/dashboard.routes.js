// =====================================================================
//  Panel principal (dashboard) - SOLO LECTURA
//  Fuentes (mismos SP que el escritorio):
//    MAM_Ventas @tipo 3 (L_prListarVentasGeneralFiltroFecha, sucursal del usuario) -> ventas, montos, vendedores, horas, tendencia
//    MAM_CreditosVentas @tipo 1                      -> cuentas por cobrar
//    MAM_CreditosCompras @tipo 1                     -> cuentas por pagar
//    MAM_ReporteVentas @tipo 6 / 10 / 11             -> productos mas vendidos, stock 0, stock minimo
//  Kaily (punto de venta): sin pedidos pendientes de asignar ni mapa de pedidos (MAM_Ventas 7 / 23 no existen).
//    Sin NombreEstado en MAM_Ventas @tipo 3: no hay estados ni anulados. Detalle de cada venta: MAM_Ventas @tipo 4.
//  Destacados del mes: mismo MAM_Ventas @tipo 3 (desde el 1ro del mes) y MAM_ReporteVentas @tipo 6 del mes.
//  MAM_PanelControl (SP propio de la web, server/sql/03_MAM_PanelControl.sql):
//    @tipo 1 hora de cada venta (MAM_Ventas @tipo 3 no trae HoraRegistro) -> ventas por hora
//    @tipo 2 compras por cliente y dia (12 meses) -> clientes que dejaron de comprar, clientes rentables (GET /analisis)
//    @tipo 3 productos con stock sin ventas en 60 dias, con su ultima venta (GET /analisis)
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

// Sucursal del usuario (Global_Sucursal); -1 = todas
const sucursalDe = (sesion) => { const s = Number(sesion?.gi_userSuc); return Number.isFinite(s) ? s : -1; };

// Ventas como Tec_Ventas de Kaily: L_prListarVentasGeneralFiltroFecha("MAM_Ventas", desde, hasta, Global_Sucursal)
function ventasRango(sesion, desde, hasta) {
    return L.L_prListarVentasGeneralFiltroFecha('MAM_Ventas', fmtVB(desde), fmtVB(hasta), sucursalDe(sesion));
}

// Hora de las ventas del mismo rango y sucursal: MAM_PanelControl @tipo 1 (Id, HoraRegistro)
function horasRango(sesion, desde, hasta) {
    return L.L_prPanelHorasVentas(fmtVB(desde), fmtVB(hasta), sucursalDe(sesion));
}

// Pone HoraRegistro (por Id) en las filas de MAM_Ventas @tipo 3. Si el SP ya la trae, la deja como esta.
function unirHoras(filas, horas) {
    if (!filas?.length || !horas?.length) return filas;
    const c = lector(filas);
    if (filas.some((f) => c(f, 'HoraRegistro') !== null)) return filas;
    const ch = lector(horas);
    const m = new Map(horas.map((h) => [String(ch(h, 'Id')), ch(h, 'HoraRegistro')]));
    return filas.map((f) => ({ ...f, HoraRegistro: m.get(String(c(f, 'Id', 'VentaId'))) ?? null }));
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

// HoraRegistro -> 0..23 (puede venir 'HH:mm', 'HH:mm:ss', un TIME/DATETIME o sin ceros: '9:5', '23:0' como lo graba el SP)
function horaDe(v) {
    if (v === null || v === undefined || v === '') return null;
    if (v instanceof Date) return v.getUTCHours();
    const m = /(\d{1,2}):(\d{1,2})/.exec(String(v));
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
    // Kaily: MAM_Ventas @tipo 3 no trae NombreEstado (sin estados de pedido ni anulados)
    const conEstado = filas.some((f) => c(f, 'NombreEstado') !== null);

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
        conHora: pedidos.some((p) => p.hora !== null), // false: el SP de horas no respondio o las ventas no tienen hora
        kpis: {
            pedidos: deDia.length,
            monto: monto(deDia),
            ticket: deDia.length ? r2(monto(deDia) / deDia.length) : 0,
            clientes: new Set(deDia.map((p) => p.clienteId)).size,
            pedidosAyer: deAyer.length,
            montoAyer: monto(deAyer),
            promMonto,
            promPedidos,
            anulados: conEstado ? todosDia.filter(anulado).length : null,
        },
        porHora,
        tendencia,
        ranking: ranking.slice(0, 8),
        masPedidos,
        estados: conEstado ? agrupar(todosDia, 'estado') : [],
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

    const [ventasSP, horas, cxc, cxp, productos, productosMes, stock0, stockMin] = await Promise.all([
        seguro(ventasRango(req.sesion, desde, fecha), 'ventas', errores),
        seguro(horasRango(req.sesion, desde, fecha), 'hora de las ventas (MAM_PanelControl)', errores),
        seguro(L.L_prListarPagosPendientesClientes(), 'cuentas por cobrar', errores),
        seguro(L.L_prListarPagosPendientes(), 'cuentas por pagar', errores),
        seguro(L.ReporteVentasProductosMasVendido(fmtVB(fecha), fmtVB(fecha)), 'productos', errores),
        seguro(L.ReporteVentasProductosMasVendido(fmtVB(desdeMes), fmtVB(fecha)), 'productos del mes', errores),
        seguro(L.L_prListarProductosConStock0(), 'stock en cero', errores),
        seguro(L.L_prListarProductosConStockMinimo(), 'stock minimo', errores),
    ]);

    const ventas = unirHoras(ventasSP, horas);
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
    const m = /(\d{1,2}):(\d{1,2})/.exec(String(v));
    return m ? `${dos(m[1])}:${dos(m[2])}` : '';
}

const DETALLES = {
    async pedidos(q, sesion) {
        const fecha = leerFecha(q.fecha);
        // dias = ultimos N dias (compras de un cliente desde el analisis); mes = desde el 1ro del mes; si no, solo el dia
        const dias = q.dias ? Number(q.dias) : 0;
        const ini = dias ? sumar(fecha, -(dias - 1)) : q.mes === '1' ? inicioMes(fecha) : fecha;
        const [ventas, horas] = await Promise.all([
            ventasRango(sesion, ini, fecha),
            horasRango(sesion, ini, fecha).catch((e) => { console.error('[dashboard] hora de las ventas:', e.message); return null; }),
        ]);
        const filas = unirHoras(ventas, horas);
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
        const conFecha = q.mes === '1' || dias > 1;
        const conEstado = filas.some((f) => c(f, 'NombreEstado') !== null); // Kaily: sin NombreEstado
        return {
            columnas: [
                { k: 'id', t: 'N°', tipo: 'num' }, ...(conFecha ? [{ k: 'fecha', t: 'Fecha', tipo: 'fecha' }] : []), { k: 'hora', t: 'Hora' }, { k: 'cliente', t: 'Cliente', ancho: true },
                { k: 'vendedor', t: 'Vendedor' }, { k: 'tipo', t: 'Pago' }, ...(conEstado ? [{ k: 'estado', t: 'Estado', chip: true }] : []), { k: 'total', t: 'Total', tipo: 'bs' },
            ],
            // la fecha va siempre: con id + fecha se pide el detalle de la venta (GET /venta/:id)
            filas: arr.map(({ id, fecha: fe, hora: hh, cliente, vendedor, tipo, estado, total }) => ({ id, fecha: fe, hora: hh, cliente, vendedor, tipo, ...(conEstado ? { estado } : {}), total })),
            total: r2(arr.reduce((s, p) => s + p.total, 0)),
            detalleVenta: true,
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
    // Clientes que compraron en los ultimos 12 meses y no volvieron en 60 dias (tramo opcional)
    async sinCompra(q, sesion) {
        const fecha = leerFecha(q.fecha);
        let arr = clientesSinCompra(await comprasClientes(sesion, fecha)).arr;
        const t = q.tramo !== undefined && q.tramo !== '' ? Number(q.tramo) : null;
        if (t !== null && TRAMOS_SIN_COMPRA[t]) arr = arr.filter((x) => enTramo(x.sinComprar, TRAMOS_SIN_COMPRA[t]));
        return {
            columnas: [{ k: 'nombre', t: 'Cliente', ancho: true }, { k: 'ultima', t: 'Última compra', tipo: 'fecha' }, { k: 'sinComprar', t: 'Días sin comprar', tipo: 'num' },
                { k: 'compras', t: 'Compras', tipo: 'num' }, { k: 'total', t: 'Compró (12 meses)', tipo: 'bs' }],
            filas: arr.map((x) => ({ nombre: x.nombre, ultima: x.ultima, sinComprar: x.sinComprar, compras: x.compras, total: x.monto })),
            total: r2(arr.reduce((s, x) => s + x.monto, 0)),
        };
    },
    // Ranking completo de clientes del periodo (30 / 90 / 365 dias) por monto o por frecuencia
    async clientesTop(q, sesion) {
        const fecha = leerFecha(q.fecha);
        const dias = PERIODOS.includes(Number(q.dias)) ? Number(q.dias) : PERIODOS[1];
        const arr = periodoClientes(await comprasClientes(sesion, fecha), dias).arr.sort(ORDEN_CLIENTES[q.orden === 'frecuencia' ? 'frecuencia' : 'monto']);
        return {
            columnas: [{ k: 'puesto', t: 'N°', tipo: 'num' }, { k: 'nombre', t: 'Cliente', ancho: true }, { k: 'dias', t: 'Días con compra', tipo: 'num' }, { k: 'compras', t: 'Compras', tipo: 'num' },
                { k: 'ticket', t: 'Ticket prom.', tipo: 'bs' }, { k: 'total', t: 'Total', tipo: 'bs' }, { k: 'ultima', t: 'Última compra', tipo: 'fecha' }],
            filas: arr.map((x, i) => ({ puesto: i + 1, nombre: x.nombre, dias: x.dias, compras: x.compras, ticket: x.ticket, total: x.monto, ultima: x.ultima })),
            total: r2(arr.reduce((s, x) => s + x.monto, 0)),
        };
    },
    // Productos con stock y sin ventas en 60 dias (tramo de dias sin venta o categoria opcional)
    async sinVenta(q) {
        const fecha = leerFecha(q.fecha);
        let arr = await productosSinVenta(fecha);
        const t = q.tramo !== undefined && q.tramo !== '' ? Number(q.tramo) : null;
        if (t !== null && TRAMOS_SIN_VENTA[t]) arr = arr.filter((x) => x.tramo === t);
        if (q.categoria) arr = arr.filter((x) => x.categoria.toUpperCase() === String(q.categoria).toUpperCase());
        return {
            columnas: [{ k: 'codigo', t: 'Código' }, { k: 'producto', t: 'Producto', ancho: true }, { k: 'categoria', t: 'Categoría' }, { k: 'stock', t: 'Stock', tipo: 'num' },
                { k: 'precio', t: 'Precio', tipo: 'bs' }, { k: 'total', t: 'Valor en stock', tipo: 'bs' }, { k: 'ultima', t: 'Última venta', tipo: 'fecha' }],
            filas: arr.map((x) => ({ codigo: x.codigo, producto: x.producto, categoria: x.categoria, stock: x.stock, precio: x.precio, total: x.valor, ultima: x.ultima })),
            total: r2(arr.reduce((s, x) => s + x.valor, 0)),
            limite: 5000, // todos (para buscar cualquier producto); son filas cortas
        };
    },
};

// =====================================================================
//  ANALISIS (GET /analisis): clientes que dejaron de comprar, clientes rentables y productos sin venta.
//  Clientes: MAM_PanelControl @tipo 2 (compras por cliente y dia, 12 meses, sucursal del usuario),
//            sin el cliente por defecto de Tec_Ventas (Id 1, mostrador): no es un cliente identificado.
//  Productos: MAM_PanelControl @tipo 3 (stock > 0 y sin ventas en 60 dias, toda la empresa), valor a precio de venta.
//  No depende del filtro de vendedor: un cliente "deja de comprar" a la tienda, no a un vendedor.
// =====================================================================
const DIAS_SIN_COMPRA = 60;
const DIAS_ANALISIS = 365;
const CLIENTE_MOSTRADOR = 1;
const PERIODOS = [30, 90, 365];
const TRAMOS_SIN_COMPRA = [[60, 90, '60 a 90 días'], [91, 180, '91 a 180 días'], [181, Infinity, 'Más de 180 días']];
const TRAMOS_SIN_VENTA = [[60, 180, '60 a 180 días'], [181, 365, '181 a 365 días'], [366, Infinity, 'Más de 1 año'], [null, null, 'Nunca se vendió']];
const enTramo = (d, [a, b]) => d >= a && d <= b;
const diasEntre = (a, b) => Math.round((Date.parse(`${b}T00:00:00Z`) - Date.parse(`${a}T00:00:00Z`)) / 864e5);
const ORDEN_CLIENTES = {
    monto: (a, b) => b.monto - a.monto || b.dias - a.dias,
    frecuencia: (a, b) => b.dias - a.dias || b.compras - a.compras || b.monto - a.monto,
};

// { fecha, clientes: [{ id, nombre, dias: [{ dia, compras, monto }] }], mostrador: [{ dia, monto }] }
async function comprasClientes(sesion, fecha) {
    const filas = await L.L_prPanelComprasClientes(fmtVB(sumar(fecha, -(DIAS_ANALISIS - 1))), fmtVB(fecha), sucursalDe(sesion));
    const c = lector(filas);
    const m = new Map();
    const mostrador = [];
    for (const f of filas) {
        const id = c(f, 'ClienteId');
        const dia = diaDe(c(f, 'FechaVenta'));
        if (id === null || !dia) continue;
        const d = { dia, compras: num(c(f, 'Compras')), monto: num(c(f, 'Monto')) };
        if (Number(id) === CLIENTE_MOSTRADOR) { mostrador.push(d); continue; }
        const k = String(id);
        if (!m.has(k)) m.set(k, { id: k, nombre: String(c(f, 'NombreCliente') ?? '').trim() || 'SIN NOMBRE', dias: [] });
        m.get(k).dias.push(d);
    }
    return { fecha, clientes: [...m.values()], mostrador };
}

// Compras de un cliente desde un dia: monto, compras, dias distintos con compra (frecuencia), ticket, ultima compra
function comprasDesde(x, desde) {
    const ds = x.dias.filter((d) => d.dia >= desde);
    if (!ds.length) return null;
    const monto = r2(ds.reduce((s, d) => s + d.monto, 0));
    const compras = ds.reduce((s, d) => s + d.compras, 0);
    return { id: x.id, nombre: x.nombre, monto, compras, dias: ds.length, ticket: compras ? r2(monto / compras) : 0, ultima: ds.reduce((a, d) => (d.dia > a ? d.dia : a), '') };
}

function periodoClientes(base, dias) {
    const desde = iso(sumar(base.fecha, -(dias - 1)));
    return {
        desde,
        arr: base.clientes.map((x) => comprasDesde(x, desde)).filter(Boolean),
        mostrador: r2(base.mostrador.filter((d) => d.dia >= desde).reduce((s, d) => s + d.monto, 0)),
    };
}

function clientesSinCompra(base) {
    const hasta = iso(base.fecha);
    const todos = base.clientes.map((x) => comprasDesde(x, '0000-00-00')).filter(Boolean).map((x) => ({ ...x, sinComprar: diasEntre(x.ultima, hasta) }));
    return { total: todos.length, arr: todos.filter((x) => x.sinComprar >= DIAS_SIN_COMPRA).sort(ORDEN_CLIENTES.monto) };
}

const corto = (x) => ({ id: x.id, nombre: x.nombre, monto: x.monto, compras: x.compras, dias: x.dias, ticket: x.ticket, ultima: x.ultima });

function resumenClientes(base) {
    const { total, arr } = clientesSinCompra(base);
    const sinCompra = {
        clientes: arr.length,
        deTotal: total,
        monto: r2(arr.reduce((s, x) => s + x.monto, 0)),
        frecuentes: arr.filter((x) => x.dias >= 3).length, // compraron en 3 o mas dias distintos
        tramos: TRAMOS_SIN_COMPRA.map((t, i) => {
            const xs = arr.filter((x) => enTramo(x.sinComprar, t));
            return { i, tramo: t[2], clientes: xs.length, monto: r2(xs.reduce((s, x) => s + x.monto, 0)) };
        }),
        top: arr.slice(0, 5).map((x) => ({ ...corto(x), sinComprar: x.sinComprar })),
    };
    const rentables = {};
    for (const p of PERIODOS) {
        const { desde, arr: xs, mostrador } = periodoClientes(base, p);
        const porMonto = [...xs].sort(ORDEN_CLIENTES.monto);
        const porFrec = [...xs].sort(ORDEN_CLIENTES.frecuencia);
        // nube frecuencia vs monto: los 40 primeros por monto y los 40 por frecuencia
        const ids = new Set([...porMonto.slice(0, 40), ...porFrec.slice(0, 40)].map((x) => x.id));
        rentables[p] = {
            desde,
            clientes: xs.length,
            monto: r2(xs.reduce((s, x) => s + x.monto, 0)),
            montoTop: r2(porMonto.slice(0, 10).reduce((s, x) => s + x.monto, 0)),
            mostrador,
            topMonto: porMonto.slice(0, 10).map(corto),
            topFrecuencia: porFrec.slice(0, 10).map(corto),
            puntos: xs.filter((x) => ids.has(x.id)).map(corto),
        };
    }
    return { sinCompra, rentables };
}

// Productos con stock y sin ventas en 60 dias (MAM_PanelControl @tipo 3), con tramo segun su ultima venta
async function productosSinVenta(fecha) {
    const filas = await L.L_prPanelProductosSinVenta(fmtVB(sumar(fecha, -(DIAS_SIN_COMPRA - 1))), fmtVB(fecha));
    const c = lector(filas);
    const hasta = iso(fecha);
    return filas.map((f) => {
        const ultima = diaDe(c(f, 'UltimaVenta'));
        const sin = ultima ? diasEntre(ultima, hasta) : null;
        return {
            id: String(c(f, 'Id')),
            codigo: String(c(f, 'CodigoExterno') ?? '').trim(),
            producto: String(c(f, 'NombreProducto') ?? '').trim(),
            categoria: String(c(f, 'Categoria') ?? '').trim() || 'SIN CATEGORIA',
            stock: r2(num(c(f, 'Stock'))),
            precio: r2(num(c(f, 'PrecioVenta'))),
            valor: r2(num(c(f, 'Valor'))),
            ultima,
            sinVender: sin,
            tramo: sin === null ? 3 : Math.max(0, TRAMOS_SIN_VENTA.findIndex((t) => t[0] !== null && enTramo(sin, t))),
        };
    }).sort((a, b) => b.valor - a.valor);
}

function resumenSinVenta(arr) {
    const grupos = new Map();
    for (const x of arr) {
        const g = grupos.get(x.categoria) || { nombre: x.categoria, productos: 0, valor: 0 };
        g.productos++; g.valor += x.valor;
        grupos.set(x.categoria, g);
    }
    return {
        productos: arr.length,
        unidades: r2(arr.reduce((s, x) => s + x.stock, 0)),
        valor: r2(arr.reduce((s, x) => s + x.valor, 0)),
        tramos: TRAMOS_SIN_VENTA.map((t, i) => {
            const xs = arr.filter((x) => x.tramo === i);
            return { i, tramo: t[2], productos: xs.length, valor: r2(xs.reduce((s, x) => s + x.valor, 0)) };
        }),
        categorias: [...grupos.values()].map((g) => ({ ...g, valor: r2(g.valor) })).sort((a, b) => b.valor - a.valor).slice(0, 6),
        top: arr.slice(0, 5).map(({ id, producto, categoria, stock, valor, ultima, sinVender }) => ({ id, producto, categoria, stock, valor, ultima, sinVender })),
    };
}

router.get('/analisis', asyncHandler(async (req, res) => {
    const fecha = leerFecha(req.query.fecha);
    const errores = [];
    const [base, sinVenta] = await Promise.all([
        seguro(comprasClientes(req.sesion, fecha), 'clientes (MAM_PanelControl)', errores),
        seguro(productosSinVenta(fecha), 'productos sin venta (MAM_PanelControl)', errores),
    ]);
    res.set('Cache-Control', 'no-store');
    res.json({
        ok: true,
        data: {
            fecha: iso(fecha),
            diasSinCompra: DIAS_SIN_COMPRA,
            clientes: base ? resumenClientes(base) : null,
            productos: sinVenta ? resumenSinVenta(sinVenta) : null,
            errores,
        },
    });
}));

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
    for (const k of ['fecha', 'vendedor', 'vendedorNombre', 'estado', 'tipo', 'hora', 'tramo', 'vencido', 'nombre', 'validos', 'mes', 'cliente', 'dias', 'orden', 'categoria']) {
        if (req.query[k] !== undefined) q[k] = String(req.query[k]).slice(0, 80);
    }
    if (q.vendedor && !/^\d+$/.test(q.vendedor)) throw new ErrorNegocio('Vendedor no valido');
    if (q.hora && !/^\d{1,2}$/.test(q.hora)) throw new ErrorNegocio('Hora no valida');
    if (q.cliente && !/^\d+$/.test(q.cliente)) throw new ErrorNegocio('Cliente no valido');
    if (q.dias && (!/^\d{1,3}$/.test(q.dias) || Number(q.dias) < 1 || Number(q.dias) > DIAS_ANALISIS)) throw new ErrorNegocio('Periodo no valido');
    const { limite = LIMITE_DETALLE, ...r } = await fn(q, req.sesion);
    const truncado = r.filas.length > limite;
    res.set('Cache-Control', 'no-store');
    res.json({ ok: true, data: { ...r, cantidad: r.filas.length, filas: r.filas.slice(0, limite), truncado } });
}));

// Productos de una venta del detalle (al tocar la fila): ListaVentasDetalles = MAM_Ventas @tipo 4.
// Solo si la venta esta en la lista de esa fecha para la sucursal del usuario (lo mismo que ve en el panel).
router.get('/venta/:id', asyncHandler(async (req, res) => {
    if (!/^\d{1,10}$/.test(String(req.params.id))) throw new ErrorNegocio('Venta no valida');
    const id = Number(req.params.id);
    const fecha = leerFecha(req.query.fecha);
    const ventas = await ventasRango(req.sesion, fecha, fecha);
    const cv = lector(ventas);
    const venta = ventas.find((f) => Number(cv(f, 'Id', 'VentaId')) === id);
    if (!venta) throw new ErrorNegocio('No se encontro la venta. Actualice el panel e intente de nuevo.', 404);
    const det = await L.ListaVentasDetalles(id);
    const c = lector(det);
    const filas = det.map((f) => {
        const kit = String(c(f, 'KitNombre') ?? '').trim();
        return {
            producto: String(c(f, 'Producto', 'NombreProducto') ?? '').trim(),
            kit: kit || null,
            cantidad: r2(num(c(f, 'Cantidad'))),
            precio: r2(num(c(f, 'Precio'))),
            descuento: r2(num(c(f, 'MontoDescuento'))),
            total: r2(num(c(f, 'Total'))),
        };
    });
    const suma = r2(filas.reduce((s, f) => s + f.total, 0));
    const descuento = r2(num(cv(venta, 'Descuento')));
    res.set('Cache-Control', 'no-store');
    res.json({ ok: true, data: { filas, suma, descuento, total: r2(suma - descuento), glosa: String(cv(venta, 'Glosa') ?? '').trim() } });
}));

module.exports = router;
