// =====================================================================
//  Reportes de "Ingreso y Salida De Productos" (cada uno con su programa)
//   kardex             Tec_KardexProductos       btInvKardex           MAM_Movimientos 13 (productos) / 14 / 15
//   saldos             Rep_SaldoProductos        btInvSaldo            MAM_Movimientos 9 / 10 / 11 / 12
//   saldos-valorados   Rep_SaldosValoradosP      btInvSaldosValorados  MAM_ReporteVentas 9
//   general            Rep_MovimientosProductos  btInvKardexReporte    MAM_Movimientos 16 + 14 + 15 por producto
//   monitoreo          F_MonitoreoProductos      btnInvMonitoreo       MAM_ReporteVentas 10 / 11 / 12
//  SP MAM_Movimientos 14/15: concep 1 = tipo con Factor 1 (ingreso/compra) -> columna 'entrada' (+)
//                            concep 2 = tipo con Factor -1 (venta/salida)  -> columna 'salida'  (-)
//  Igual que la grilla VB: INGRESOS = entrada, SALIDA = salida. Saldo inicial = movimientos anteriores
//  a "Desde"; saldo acumulado fila por fila (concep 1/3 suma entrada, 2/4 suma salida).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma, ProgramasPermitidos } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { rango, col, num, limpiar } = require('../distribucion/comun');

const router = express.Router();
const sinImg = (dt) => limpiar(dt).map((f) => { const o = { ...f }; delete o.img; delete o.Img; return o; });
const r6 = (x) => Math.round(x * 1e6) / 1e6;
const concep = (f) => String(col(f, 'concep') ?? '').trim();
const ING = ['1', '3'];
const SAL = ['2', '4'];
const mov = (dt, cs) => dt.filter((f) => cs.includes(concep(f))).reduce((s, f) => s + num(col(f, 'entrada')) + num(col(f, 'salida')), 0);

router.use('/aux', asyncHandler(async (req, res, next) => {
    const p = await ProgramasPermitidos(req.sesion.gi_userRol);
    if (!['BTINVKARDEX', 'BTINVSALDO', 'BTINVKARDEXREPORTE'].some((x) => p.has(x))) return res.status(403).json({ ok: false, mensaje: 'No tiene privilegios para este programa.' });
    next();
}));
router.get('/aux/depositos', asyncHandler(async (req, res) => res.json({ ok: true, data: v.proyectar(await L.L_prListarDepositos(), ['Id', 'NombreDeposito']) })));

// ---------- Tec_KardexProductos ----------
router.get('/kardex/productos/:deposito', requierePrograma('btInvKardex'), asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarProductosKardex(v.idRuta(req.params.deposito)), ['Id', 'Nombre', 'Stock']) });
}));
router.get('/kardex', requierePrograma('btInvKardex'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const prod = v.entero(req.query.producto, 'Producto', { requeridoPositivo: true });
    const dep = v.entero(req.query.deposito, 'Deposito', { requeridoPositivo: true });
    const [total, periodo] = await Promise.all([L.L_fnObtenerHistorialProductoGeneral(prod, d, dep), L.L_fnObtenerHistorialProducto(prod, d, h, dep)]);
    if (!periodo.length) return res.json({ ok: true, data: { filas: [], resumen: null } });
    const inicial = mov(total, ING) + mov(total, SAL);
    let saldo = inicial;
    const filas = sinImg(periodo).map((f) => {
        const c = concep(f);
        const ent = ING.includes(c) ? num(f.entrada) : 0;
        const sal = SAL.includes(c) ? num(f.salida) : 0; // viene negativo del SP
        saldo += ent + sal;
        return { fecha: f.fdoc, concepto: f.descConcep, observacion: f.obs, ingreso: r6(ent), salida: r6(Math.abs(sal)), saldo: r6(saldo), documento: f.fechaDocumento ?? '' };
    });
    filas.unshift({ fecha: req.query.desde, concepto: 'Saldo Inicial', observacion: `Saldo A la Fecha ${String(req.query.desde).split('-').reverse().join('/')}`, ingreso: 0, salida: 0, saldo: r6(inicial), documento: '' });
    res.json({ ok: true, data: { filas, resumen: { inicial: r6(inicial), ingresos: r6(mov(periodo, ING)), salidas: r6(Math.abs(mov(periodo, SAL))), final: r6(saldo) } } });
}));

// ---------- Rep_SaldoProductos ----------
router.get('/saldos', requierePrograma('btInvSaldo'), asyncHandler(async (req, res) => {
    const dep = Number.parseInt(req.query.deposito, 10);
    const mayor0 = req.query.mayor0 === '1';
    const todos = !(dep > 0);
    const dt = todos ? (mayor0 ? await L.ReporteSaldosTodosAlmacenesMayorA0() : await L.ReporteSaldosTodosAlmacenesTodos())
        : (mayor0 ? await L.ReporteSaldosUnAlmacenCantidadMayor0(dep) : await L.ReporteSaldosUnAlmacenTodosCantidad(dep));
    res.json({ ok: true, data: sinImg(dt) });
}));

router.get('/saldos-valorados', requierePrograma('btInvSaldosValorados'), asyncHandler(async (req, res) => {
    res.json({ ok: true, data: sinImg(await L.ReporteSaldosValorados()) });
}));

// ---------- Rep_MovimientosProductos (kardex general) ----------
router.get('/general', requierePrograma('btInvKardexReporte'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const dep = v.entero(req.query.deposito, 'Deposito', { requeridoPositivo: true });
    const prods = await L.L_fnListarProductosQueTienenMovimientos(d, h, dep);
    if (prods.length > 1500) throw new ErrorNegocio('Demasiados productos con movimiento: reduzca el rango de fechas');
    const filas = [];
    for (let i = 0; i < prods.length; i += 8) { // de a 8 productos en paralelo
        const lote = prods.slice(i, i + 8);
        const calc = await Promise.all(lote.map(async (p) => {
            const id = num(col(p, 'Id'));
            const [total, periodo] = await Promise.all([L.L_fnObtenerHistorialProductoGeneral(id, d, dep), L.L_fnObtenerHistorialProducto(id, d, h, dep)]);
            const o = sinImg([p])[0];
            if (periodo.length) {
                const saldo = mov(total, ING) + mov(total, SAL);
                const ing = mov(periodo, ING); const sal = mov(periodo, SAL); // sal negativo
                Object.assign(o, { SaldoAnterior: r6(saldo), Entradas: r6(ing), Salidas: r6(Math.abs(sal)), SaldoFinal: r6(Math.abs(ing + saldo + sal)) });
            }
            return o;
        }));
        filas.push(...calc);
    }
    res.json({ ok: true, data: filas });
}));

// ---------- F_MonitoreoProductos ----------
router.get('/monitoreo', requierePrograma('btnInvMonitoreo'), asyncHandler(async (req, res) => {
    const [s0, min, venc] = await Promise.all([L.L_prListarProductosConStock0(), L.L_prListarProductosConStockMinimo(), L.L_prListarProductosStockVencidos()]);
    res.json({ ok: true, data: { sinStock: sinImg(s0), stockMinimo: sinImg(min), vencidos: sinImg(venc) } });
}));

module.exports = router;
