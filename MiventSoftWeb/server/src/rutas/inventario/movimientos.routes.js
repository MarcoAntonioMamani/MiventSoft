// =====================================================================
//  Tec_Movimientos.vb + Tec_MovimientoDetalle.vb (programa: btInvMovimiento)  SP: MAM_Movimientos
//   Lista L_prListarGeneral (@tipo 3) · detalle @tipo 4 · tipos @tipo 5 · productos con stock @tipo 7
//   Grabar @tipo 1 / Modificar @tipo 2 (@detalle con estados 0/1/2/-1/-2, Estado = 1) · Eliminar @tipo -1
//   Depositos: MAM_Almacenes @tipo 4
//  Reglas: tipo 4 = INGRESO (sin limite de stock); los demas (3 = salida) no pueden superar el stock.
//  Igual que Ventas: si la configuracion usa LOTES se avisa (la web aun no maneja lotes).
//  Mejora: el servidor arma el detalle desde los SP y valida stock y cantidades.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { fechaReq, aVB, col, num, fechaIso } = require('../distribucion/comun');

const router = express.Router();
router.use(requierePrograma('btInvMovimiento'));
const INGRESO = 4;
const SALIDA = 3;
const CAMPOS = ['Id', 'fdoc', 'concep', 'nconcep', 'obs', 'est', 'alm', 'NombreDeposito'];
const fila = (f) => ({ ...f, fdoc: fechaIso(f.fdoc) });
const r6 = (x) => Math.round(x * 1e6) / 1e6;
const nuevaFila = (columnas, valores) => {
    const o = Object.fromEntries(columnas.map((c) => [c, null]));
    for (const [k, x] of Object.entries(valores)) { const c = columnas.find((y) => y.toLowerCase() === k.toLowerCase()); if (c) o[c] = x; }
    return o;
};

async function usaLote() {
    const c = await L.L_prLeerConfiguracion();
    const x = c[0] ? col(c[0], 'Lote') : false;
    return x === true || Number(x) === 1;
}
async function combos() {
    const [dep, tipos] = await Promise.all([L.L_prListarDepositos(), L.L_prListarTiposMovimientos()]);
    return { depositos: v.proyectar(dep, ['Id', 'NombreDeposito']), tipos: v.proyectar(tipos, ['Id', 'Descripcion']) };
}
const productosCampos = (dt) => dt.map((p) => ({ Id: num(col(p, 'Id')), CodigoExterno: col(p, 'CodigoExterno') ?? '', NombreProducto: col(p, 'NombreProducto') ?? col(p, 'DescripcionProducto') ?? '',
    DescripcionProducto: col(p, 'DescripcionProducto') ?? '', NombreCategoria: col(p, 'NombreCategoria') ?? '', stock: num(col(p, 'stockUnidad') ?? col(p, 'stock')), conversion: num(col(p, 'conversion')) || 1 }));
function cantidadValida(c, stock, tipo, nombre) {
    const n = Number(c);
    if (!Number.isFinite(n) || n <= 0) throw new ErrorNegocio(`La Cantidad de ${nombre} debe ser Mayor a 0`);
    if (tipo !== INGRESO && n > (stock === Infinity ? Infinity : num(stock)) + 1e-9) throw new ErrorNegocio(`La cantidad que se quiere sacar es mayor a la que existe en el stock solo puede Sacar : ${num(stock)} (${nombre})`.toUpperCase());
    return n;
}

async function leerCabecera(b) {
    const c = await combos();
    const ConceptoId = Number(b.ConceptoId);
    if (!c.tipos.some((t) => Number(t.Id) === ConceptoId)) throw new ErrorNegocio('Seleccione Tipo Movimiento'.toUpperCase());
    const DepositoId = Number(b.DepositoId);
    if (!c.depositos.some((d) => Number(d.Id) === DepositoId)) throw new ErrorNegocio('Seleccion Deposito'.toUpperCase());
    return { ConceptoId, DepositoId, Observacion: v.texto(b.Observacion, 500), FechaDocumento: aVB(fechaReq(b.FechaDocumento, 'Fecha Transaccion')) };
}
function filasNuevas(columnas, pedidos, productos, tipo, siguienteId, vigentes) {
    const vistos = new Set(vigentes);
    let id = siguienteId;
    return pedidos.map((d) => {
        const pid = v.entero(d.ProductoId, 'Producto', { requeridoPositivo: true });
        if (vistos.has(pid)) throw new ErrorNegocio('El producto ya existe en el detalle'.toUpperCase());
        vistos.add(pid);
        const p = productos.find((x) => x.Id === pid);
        if (!p) throw new ErrorNegocio('Producto no disponible en el deposito');
        const cant = cantidadValida(d.Cantidad, p.stock, tipo, p.NombreProducto);
        id += 1;
        return nuevaFila(columnas, { Id: id, MovimientoId: 0, ProductoId: pid, Producto: p.DescripcionProducto || p.NombreProducto, Cantidad: cant, CantidadCajas: r6(cant / p.conversion),
            Lote: '20200101', FechaVencimiento: new Date(Date.UTC(2020, 0, 1)), img: null, estado: 0, stock: p.stock, stockFinal: tipo === SALIDA ? p.stock - cant : p.stock + cant, conversion: p.conversion });
    });
}

// ---------------- lecturas ----------------
router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_Movimientos'), CAMPOS).map(fila) });
}));
router.get('/combos', asyncHandler(async (req, res) => res.json({ ok: true, data: { ...(await combos()), lote: await usaLote() } })));
router.get('/productos/:deposito', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: productosCampos(await L.L_prListarProductosLote(v.idRuta(req.params.deposito))) });
}));
router.get('/:id/detalle', asyncHandler(async (req, res) => {
    const dt = await L.L_prListarDetalleMovimiento(v.idRuta(req.params.id));
    res.json({ ok: true, data: dt.map((f) => ({ Id: num(col(f, 'Id')), ProductoId: num(col(f, 'ProductoId')), Producto: col(f, 'Producto') ?? '', Cantidad: num(col(f, 'Cantidad')),
        CantidadCajas: num(col(f, 'CantidadCajas')), conversion: num(col(f, 'conversion')) || 1, stock: num(col(f, 'stock')) })) });
}));

// ---------------- grabar ----------------
router.post('/', asyncHandler(async (req, res) => {
    if (await usaLote()) throw new ErrorNegocio('El sistema esta configurado con LOTES: registre este movimiento desde el escritorio.');
    const b = req.body || {};
    const cab = await leerCabecera(b);
    const columnas = L._columnasDe(await L.L_prListarDetalleMovimiento(-1));
    const productos = productosCampos(await L.L_prListarProductosLote(cab.DepositoId));
    const filas = filasNuevas(columnas, (Array.isArray(b.detalle) ? b.detalle : []).filter((d) => !d.eliminado), productos, cab.ConceptoId, 0, []);
    if (!filas.length) throw new ErrorNegocio('Por Favor Inserte un Detalle'.toUpperCase());
    const r = await L.L_prMovimientoInsertar({ ...cab, _numi: '' }, { columnas, filas });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Movimiento'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `CODIGO DE MOVIMIENTO ${r._numi} GRABADO CON EXITO.` });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    if (await usaLote()) throw new ErrorNegocio('El sistema esta configurado con LOTES: modifique este movimiento desde el escritorio.');
    const id = v.idRuta(req.params.id);
    if (!v.proyectar(await L.L_prListarGeneral('MAM_Movimientos'), ['Id']).some((x) => Number(x.Id) === id)) throw new ErrorNegocio('No se encontro el movimiento. Actualice la lista.', 404);
    const b = req.body || {};
    const cab = await leerCabecera(b);
    const existentes = await L.L_prListarDetalleMovimiento(id);
    const columnas = L._columnasDe(existentes);
    const enviados = Array.isArray(b.detalle) ? b.detalle : [];
    const porId = new Map(enviados.filter((d) => !d.nuevo && d.Id !== undefined).map((d) => [Number(d.Id), d]));
    const filas = existentes.map((f) => {
        const d = porId.get(num(col(f, 'Id')));
        const o = nuevaFila(columnas, { ...f, estado: 1 });
        if (!d) return o;
        if (d.eliminado) return nuevaFila(columnas, { ...o, estado: -1 });
        const st = col(f, 'stock');
        const cant = cantidadValida(d.Cantidad, st === undefined || st === null ? Infinity : st, cab.ConceptoId, col(f, 'Producto'));
        if (Math.abs(cant - num(col(f, 'Cantidad'))) < 1e-9) return o;
        const conv = num(col(f, 'conversion')) || 1;
        return nuevaFila(columnas, { ...o, Cantidad: cant, CantidadCajas: r6(cant / conv), estado: 2 });
    });
    const vigentes = filas.filter((f) => num(col(f, 'estado')) >= 0).map((f) => num(col(f, 'ProductoId')));
    const nuevos = enviados.filter((d) => d.nuevo && !d.eliminado);
    if (nuevos.length) {
        const productos = productosCampos(await L.L_prListarProductosLote(cab.DepositoId));
        filas.push(...filasNuevas(columnas, nuevos, productos, cab.ConceptoId, filas.reduce((m, f) => Math.max(m, num(col(f, 'Id'))), 0), vigentes));
    }
    if (!filas.some((f) => num(col(f, 'estado')) >= 0)) throw new ErrorNegocio('Por Favor Inserte un Detalle'.toUpperCase());
    if (!(await L.L_prMovimientoActualizar({ ...cab, _numi: String(id) }, { columnas, filas }))) throw new ErrorNegocio('Error al guardar el Movimiento'.toUpperCase());
    res.json({ ok: true, id, mensaje: `CODIGO DE MOVIMIENTO ${id} MODIFICADO CON EXITO.` });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await L.L_prBorrarRegistro(id, 'MAM_Movimientos'))) throw new ErrorNegocio('Error al eliminar el Movimiento'.toUpperCase());
    res.json({ ok: true, mensaje: `CODIGO DE MOVIMIENTO ${id} ELIMINADO CON EXITO.` });
}));

module.exports = router;
