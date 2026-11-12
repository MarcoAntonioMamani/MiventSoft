// =====================================================================
//  KAILY LA PAZ: reportes del Panel_Ventas (cada uno con SU programa, como el menu del escritorio)
//   ventas-realizadas      Rep_VentasRealizadas      btVentReporteRealizada     MAM_ReporteVentas 1 / 2 (sin filtro de cliente)
//   ventas-costos          Reporte_VentasVsCostos    btVentReporteVentaVsCosto  MAM_ReporteVentas 4 / 5 (sin filtro de cliente)
//   productos-mas-vendidos Reporte_ProductoMasVendido btVentProdMasVendidos     MAM_ReporteVentas 6
//   rendimiento            Reporte_RendimientoPersonal btVentRendimiento        MAM_ReporteVentas 7 + 8 (@PersonalType)
//  Kaily no tiene Rep_VentasPorProductos, Reporte_HistoricoCobro ni FrmVentasPorCategoria (sus SP @tipo ya no estan en AccesoLogica.vb).
//  Se devuelven las columnas tal cual las entrega el SP (sin imagenes), fechas como AAAA-MM-DD.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma, ProgramasPermitidos } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
const MAX_DIAS = 370;
const dos = (n) => String(n).padStart(2, '0');

function fechas(q) {
    const leer = (t, campo) => {
        const s = v.texto(t, 10);
        const m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(s);
        if (!m || Number.isNaN(Date.parse(s))) throw new ErrorNegocio(`Fecha ${campo} no valida`);
        return s;
    };
    const d = leer(q.desde, 'Desde');
    const h = leer(q.hasta, 'Hasta');
    if (d > h) throw new ErrorNegocio('La fecha Desde no puede ser mayor que Hasta');
    if ((Date.parse(h) - Date.parse(d)) / 864e5 > MAX_DIAS) throw new ErrorNegocio('El rango maximo es de un año');
    return [d.replace(/-/g, '/'), h.replace(/-/g, '/')]; // .ToString("yyyy/MM/dd")
}
const idOpc = (x) => { const n = Number.parseInt(x, 10); return Number.isFinite(n) && n > 0 ? n : 0; };

// Filas del SP para el navegador: sin columnas binarias (logo/imagenes), fechas legibles
function limpiar(dt) {
    return dt.map((f) => {
        const o = {};
        for (const [k, x] of Object.entries(f)) {
            if (Buffer.isBuffer(x)) continue;
            o[k] = x instanceof Date
                ? (x.getUTCHours() || x.getUTCMinutes()
                    ? `${x.getUTCFullYear()}-${dos(x.getUTCMonth() + 1)}-${dos(x.getUTCDate())} ${dos(x.getUTCHours())}:${dos(x.getUTCMinutes())}`
                    : `${x.getUTCFullYear()}-${dos(x.getUTCMonth() + 1)}-${dos(x.getUTCDate())}`)
                : x;
        }
        return o;
    });
}
const enviar = (res, dt) => res.json({ ok: true, data: limpiar(dt) });

// ---------- auxiliares (combos de los reportes): solo con algun programa de reportes ----------
const REPORTES = ['BTVENTREPORTEREALIZADA', 'BTVENTREPORTEVENTAVSCOSTO', 'BTVENTPRODMASVENDIDOS', 'BTVENTRENDIMIENTO'];
router.use('/aux', asyncHandler(async (req, res, next) => {
    const p = await ProgramasPermitidos(req.sesion.gi_userRol);
    if (!REPORTES.some((x) => p.has(x))) return res.status(403).json({ ok: false, mensaje: 'No tiene privilegios para este programa.' });
    next();
}));
router.get('/aux/personal', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.ListarPersonal(), ['Id', 'Nombre']) });
}));
// Cabecera de los reportes impresos: empresa y logo (mismos SP que la nota de venta, solo lectura)
router.get('/aux/cabecera', asyncHandler(async (req, res) => {
    const [empresas, imagen] = await Promise.all([L.L_prListarGeneral('MAM_Empresas'), L.ObtenerImagenEmpresa().catch(() => [])]);
    res.json({
        ok: true,
        data: {
            empresa: v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Nit', 'Ciudad'])[0] || null,
            imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '',
        },
    });
}));
// ---------- reportes ----------
router.get('/ventas-realizadas', requierePrograma('btVentReporteRealizada'), asyncHandler(async (req, res) => {
    const [d, h] = fechas(req.query);
    const vend = idOpc(req.query.vendedor); // chkTodos = sin vendedor
    enviar(res, vend ? await L.ReporteVentasAtendidasTodosUnVendedor(d, h, vend) : await L.ReporteVentasAtendidasTodos(d, h));
}));

router.get('/ventas-costos', requierePrograma('btVentReporteVentaVsCosto'), asyncHandler(async (req, res) => {
    const [d, h] = fechas(req.query);
    const vend = idOpc(req.query.vendedor);
    enviar(res, vend ? await L.ReporteVentasUtilidadesTodosUnVendedor(d, h, vend) : await L.ReporteVentasUtilidadesTodos(d, h));
}));

router.get('/productos-mas-vendidos', requierePrograma('btVentProdMasVendidos'), asyncHandler(async (req, res) => {
    const [d, h] = fechas(req.query);
    enviar(res, await L.ReporteVentasProductosMasVendido(d, h));
}));

router.get('/rendimiento/personal', requierePrograma('btVentRendimiento'), asyncHandler(async (req, res) => {
    enviar(res, await L.ReporteListarPersonal());
}));
// Personal elegido -> tabla @PersonalType completa con chk marcado (igual que VB). todos=true: marca a todo el personal
async function personalMarcado(query, todos = false) {
    const dtPer = await L.ReporteListarPersonal();
    const columnas = L._columnasDe(dtPer);
    const kId = columnas.find((c) => c.toLowerCase() === 'id');
    const kChk = columnas.find((c) => c.toLowerCase() === 'chk');
    if (!kId || !kChk) throw new ErrorNegocio('MAM_ReporteVentas @tipo 7 no devolvio las columnas Id / chk');
    const sel = new Set(todos
        ? dtPer.map((f) => Number(f[kId])).filter((x) => x > 0)
        : String(query.personal || '').split(',').map((x) => Number.parseInt(x, 10)).filter((x) => x > 0));
    if (!sel.size) throw new ErrorNegocio(todos ? 'No hay personal registrado' : 'Seleccione al menos un Personal');
    const kNom = columnas.find((c) => /^(nombrepersonal|nombre|personal)$/i.test(c));
    const filas = dtPer.map((f) => ({ ...f, [kChk]: sel.has(Number(f[kId])) ? 1 : 0 }));
    const elegidos = dtPer.filter((f) => sel.has(Number(f[kId]))).map((f) => ({ id: Number(f[kId]), nombre: String(kNom ? f[kNom] ?? '' : f[kId]) }));
    return { tvp: { columnas, filas }, elegidos };
}

router.get('/rendimiento', requierePrograma('btVentRendimiento'), asyncHandler(async (req, res) => {
    const [d, h] = fechas(req.query);
    const { tvp } = await personalMarcado(req.query);
    enviar(res, await L.ReporteVentasRendimientoPersonal(d, h, tvp));
}));

// ---- Lectura de MAM_ReporteVentas @tipo 8 (sirve con filas de resumen o de detalle por venta) ----
const numero = (x) => { const n = Number(typeof x === 'string' ? x.replace(/,/g, '') : x); return Number.isFinite(n) ? n : 0; };
const norm = (k) => String(k).toLowerCase().replace(/[^a-z0-9]/g, ''); // "Total Venta" == "totalventa"
// Resuelve columnas por PRIORIDAD de nombre (no por orden de columnas del SP): con filas de detalle
// (Id de la venta + PersonalId) se debe tomar PersonalId, nunca el Id de la venta.
const resolver = (cols, exactos, patron, excluir) => {
    for (const n of exactos) { const k = cols.find((c) => norm(c) === norm(n)); if (k) return k; }
    return patron ? cols.find((c) => patron.test(norm(c)) && !(excluir && excluir.test(norm(c)))) : undefined;
};
const columnasRendimiento = (cols) => ({
    id: resolver(cols, ['PersonalId', 'IdPersonal', 'VendedorId', 'IdVendedor']),
    nom: resolver(cols, ['Personal', 'NombrePersonal', 'Vendedor', 'NombreVendedor'], /personal|vendedor/, /id$|^id/),
    tot: resolver(cols, ['Total', 'TotalVenta', 'TotalVentas', 'MontoTotal', 'Monto', 'Importe', 'Ventas'], /total|monto|importe|venta/, /^id|id$|cantidad|pedido|nro|fecha|costo|utilidad/),
    ped: resolver(cols, ['Pedidos', 'NroVentas', 'CantidadVentas', 'NroPedidos', 'CantidadPedidos']),
});
/** Acumulador de totales por persona. elegidos: [{ id, nombre }]. agregar(dt, i) suma las filas del SP en la posicion i. */
function acumuladorRendimiento(elegidos, posiciones) {
    const porId = new Map(elegidos.map((e) => [e.id, e]));
    const porNombre = new Map(elegidos.map((e) => [e.nombre.trim().toUpperCase(), e]));
    const tot = new Map(elegidos.map((e) => [e.id, Array.from({ length: posiciones }, () => ({ total: 0, pedidos: 0 }))]));
    let cols = null; let k = {}; let filasLeidas = 0; let filasSinVendedor = 0;
    return {
        tot,
        agregar(dt, i) {
            for (const f of dt) {
                if (!cols) { cols = Object.keys(f); k = columnasRendimiento(cols); }
                filasLeidas++;
                const e = (k.id && porId.get(Number(f[k.id]))) || (k.nom && porNombre.get(String(f[k.nom] ?? '').trim().toUpperCase()));
                if (!e) { filasSinVendedor++; continue; }
                const x = tot.get(e.id)[i];
                x.total += numero(k.tot ? f[k.tot] : 0);
                x.pedidos += k.ped ? numero(f[k.ped]) : 1; // sin columna de pedidos: una fila por venta
            }
        },
        // Diagnostico: solo nombres de columnas y conteos, nunca valores (sin datos de clientes en logs)
        aviso(origen) {
            if (!filasLeidas || (k.tot && (k.id || k.nom) && filasSinVendedor < filasLeidas)) return null;
            const msg = `No se pudo interpretar MAM_ReporteVentas @tipo 8. Columnas: ${cols.join(', ')}`;
            console.warn(`[${origen}]`, msg, JSON.stringify({ k, filasLeidas, filasSinVendedor }));
            return msg;
        },
    };
}
const r2 = (n) => Math.round(n * 100) / 100;

// Actividad del rango (para ordenar la lista de personal antes de generar): una sola consulta con TODO el
// personal marcado. Solo se devuelven agregados por persona (monto y pedidos), nunca el detalle de ventas.
router.get('/rendimiento/actividad', requierePrograma('btVentRendimiento'), asyncHandler(async (req, res) => {
    const [d, h] = fechas(req.query);
    const { tvp, elegidos } = await personalMarcado(req.query, true);
    const acc = acumuladorRendimiento(elegidos, 1);
    acc.agregar(await L.ReporteVentasRendimientoPersonal(d, h, tvp), 0);
    const aviso = acc.aviso('rendimiento/actividad');
    const data = elegidos.map((e) => { const x = acc.tot.get(e.id)[0]; return { id: e.id, total: r2(x.total), pedidos: x.pedidos }; });
    res.json({ ok: true, data: { personal: data, aviso } });
}));

// Rendimiento mes a mes (grafico de lineas): el SP @tipo 8 devuelve el total del rango por personal,
// asi que se consulta una vez por cada mes del rango (mismo SP y @tipo, solo lectura) y se arma la serie.
router.get('/rendimiento/mensual', requierePrograma('btVentRendimiento'), asyncHandler(async (req, res) => {
    const [d, h] = fechas(req.query).map((x) => x.replace(/\//g, '-'));
    const { tvp, elegidos } = await personalMarcado(req.query);
    const meses = [];
    for (let a = Number(d.slice(0, 4)), m = Number(d.slice(5, 7)); `${a}-${String(m).padStart(2, '0')}` <= h.slice(0, 7); m === 12 ? (a++, m = 1) : m++) {
        const clave = `${a}-${String(m).padStart(2, '0')}`;
        const fin = new Date(Date.UTC(a, m, 0)).toISOString().slice(0, 10);
        meses.push({ clave, desde: clave + '-01' < d ? d : clave + '-01', hasta: fin > h ? h : fin });
    }
    const acc = acumuladorRendimiento(elegidos, meses.length);
    for (let i = 0; i < meses.length; i++) { // de a uno: no sobrecargar la BD
        acc.agregar(await L.ReporteVentasRendimientoPersonal(meses[i].desde.replace(/-/g, '/'), meses[i].hasta.replace(/-/g, '/'), tvp), i);
    }
    const series = elegidos.map((e) => ({ ...e, valores: acc.tot.get(e.id).map((x) => ({ total: r2(x.total), pedidos: x.pedidos })) }));
    res.json({ ok: true, data: { meses, series, aviso: acc.aviso('rendimiento/mensual') } });
}));

module.exports = router;
