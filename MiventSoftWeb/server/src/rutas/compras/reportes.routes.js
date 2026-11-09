// =====================================================================
//  Reportes de Compras (cada uno con SU programa)
//   historico            Tec_HistoricoPreciosCompra  btnHistorico           MAM_Compras 9 (compras) + 11 (productos) + 10 (totales)
//   compras-por-producto FReporteCompraProducto      btnComprasPorProducto  MAM_ReporteVentas 16 (+ MAM_Movimientos 13 = productos)
//  Historico: igual que ArmarHistorico() de VB, una fila por compra y por cada producto
//  una columna de Cantidad y otra de Precio Costo, mas TotalesCantidad / TotalesBs.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { rango, col, num, limpiar, fechaIso } = require('../distribucion/comun');

const router = express.Router();

router.get('/historico', requierePrograma('btnHistorico'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const compras = await L.ListaComprasHistorico(d, h);
    if (!compras.length) return res.json({ ok: true, data: { filas: [], productos: [] } });
    const [det, tot] = await Promise.all([L.ListaComprasProductosHistorico(d, h), L.ListaComprasTotalesHistorico(d, h)]);
    // ObtenerProductos(): productos unicos en el orden en que aparecen
    const productos = [];
    for (const f of det) {
        const id = num(col(f, 'ProductoId'));
        if (!productos.some((p) => p.Id === id)) productos.push({ Id: id, Nombre: String(col(f, 'NombreProducto') ?? id) });
    }
    const filas = compras.map((c) => {
        const id = num(col(c, 'Id'));
        const o = { Id: id, FechaCompra: fechaIso(col(c, 'FechaCompra')), NombreProveedor: col(c, 'NombreProveedor') ?? '', NroNotaCompra: col(c, 'NroNotaCompra') ?? '',
            NroNotaRecepcion: col(c, 'NroNotaRecepcion') ?? '' };
        for (const f of det.filter((x) => num(col(x, 'Id')) === id)) {
            const pid = num(col(f, 'ProductoId'));
            o[`c${pid}`] = num(col(f, 'Cantidad'));
            o[`p${pid}`] = num(col(f, 'PrecioCosto'));
        }
        const t = tot.find((x) => num(col(x, 'Id')) === id);
        o.TotalesCantidad = t ? num(col(t, 'Cantidad')) : null;
        o.TotalesBs = t ? num(col(t, 'TotalCompra')) : null;
        return o;
    });
    res.json({ ok: true, data: { filas, productos } });
}));

router.get('/compras-por-producto/productos', requierePrograma('btnComprasPorProducto'), asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarProductosKardex(1), ['Id', 'Nombre', 'stock']) });
}));
router.get('/compras-por-producto', requierePrograma('btnComprasPorProducto'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const p = req.query.producto ? Number.parseInt(req.query.producto, 10) : 0; // 0 = todos (chkTodos)
    if (!Number.isFinite(p) || p < 0) throw new ErrorNegocio('Seleccione un Producto');
    res.json({ ok: true, data: limpiar(await L.ReporteComprasPorProductos(d, h, p)) });
}));

module.exports = router;
