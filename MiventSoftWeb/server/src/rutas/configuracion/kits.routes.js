// =====================================================================
//  KAILY LA PAZ: Tec_Kits.vb (programa: btnKits)   SP: MAM_Kits
//   Lista L_prListarGeneral (@tipo 3) · productos del kit ListaProductosKits (@tipo 4)
//   productos elegibles ListarProductosKits (@tipo 5: ProductoId, NombreProducto, NombreCategoria, PrecioVenta, PrecioCosto)
//   Grabar InsertarKit @tipo 1 / ModificarKit @tipo 2 con @KitsProductos:
//     DefaultView.ToTable(False, Id, KitId, ProductoId, NombreProducto, Precio, Cantidad, SubTotal, estado, PrecioCosto, PrecioVentaBackup)
//     estado 0 nuevo · 1 sin cambios · 2 modificado · -1 quitado
//   Eliminar L_prBorrarRegistro (@tipo -1)
//  Reglas del escritorio:
//   * Nombre del kit requerido. Total = Σ SubTotal (Cantidad × Precio) de los productos vigentes.
//   * Cantidad no valida o <= 0 -> 1. Precio vacio -> precio de venta (PrecioVentaBackup).
//   * El precio no puede ser menor al costo del producto.
//  En la web el precio de costo no sale al navegador y el servidor arma la tabla con los datos del SP.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btnKits'));

const COLUMNAS = ['Id', 'KitId', 'ProductoId', 'NombreProducto', 'Precio', 'Cantidad', 'SubTotal', 'estado', 'PrecioCosto', 'PrecioVentaBackup'];
const MAX_FILAS = 200;
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };

router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_Kits'), ['Id', 'NombreKit', 'DescripcionKit', 'Estado', 'Total']) });
}));

// btnAgregarProducto -> ListarProductosKits (sin PrecioCosto)
router.get('/productos', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.ListarProductosKits(), ['ProductoId', 'NombreProducto', 'NombreCategoria', 'PrecioVenta']) });
}));

router.get('/:id/productos', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    res.json({ ok: true, data: v.proyectar(await L.ListaProductosKits(id), ['Id', 'ProductoId', 'NombreProducto', 'Precio', 'Cantidad', 'SubTotal', 'PrecioVentaBackup']) });
}));

function _leerCabecera(b) {
    return {
        NombreKit: v.requerido(b.NombreKit, 'Nombre Kit', 200),
        DescripcionKit: v.texto(b.DescripcionKit, 500),
        Estado: v.bit(b.Estado),
    };
}

// grProductos_CellValueChanged / CellEdited
function _cantidad(x) { const n = num(x); return n > 0 ? n : 1; }
function _precio(x, backup, costo, nombre) {
    if (x === undefined || x === null || x === '' || !Number.isFinite(Number(x))) return backup;
    const p = Number(x);
    if (p < costo - 1e-9) throw new ErrorNegocio(`${nombre}: El Precio Es Menor Al Precio De Costo Del Producto que Es = ${Math.round(costo * 100) / 100}`);
    return p;
}

/**
 * entradas: [{ Id, eliminado, Cantidad, Precio }]   filas que ya estaban en el kit
 *           [{ nuevo, ProductoId, Cantidad, Precio }] productos agregados
 */
async function _armarDetalle(kitId, entradas) {
    if (!Array.isArray(entradas)) entradas = [];
    if (entradas.length > MAX_FILAS) throw new ErrorNegocio(`El kit no puede tener mas de ${MAX_FILAS} productos`);
    const [existentes, catalogo] = await Promise.all([L.ListaProductosKits(kitId > 0 ? kitId : -1), L.ListarProductosKits()]);
    const c = (f, n) => v.campo(f, n);
    const filas = [];
    const usados = new Set();
    let mayorId = existentes.reduce((m, f) => Math.max(m, num(c(f, 'Id'))), 0); // _GenerarId

    for (const ex of existentes) {
        const fila = Object.fromEntries(COLUMNAS.map((k) => [k, c(ex, k)]));
        const id = num(fila.Id);
        const e = entradas.find((x) => x && !x.nuevo && num(x.Id) === id);
        if (!e || e.eliminado) { filas.push({ ...fila, estado: -1 }); continue; }
        if (usados.has(num(fila.ProductoId))) throw new ErrorNegocio(`El producto ${fila.NombreProducto} esta repetido en el kit`);
        usados.add(num(fila.ProductoId));
        const cant = _cantidad(e.Cantidad);
        const precio = _precio(e.Precio, num(fila.PrecioVentaBackup) || num(fila.Precio), num(fila.PrecioCosto), fila.NombreProducto);
        const cambio = Math.abs(cant - num(fila.Cantidad)) > 1e-9 || Math.abs(precio - num(fila.Precio)) > 1e-9;
        filas.push({ ...fila, Cantidad: cant, Precio: precio, SubTotal: cant * precio, estado: cambio ? 2 : 1 });
    }
    for (const e of entradas) {
        if (!e || !e.nuevo || e.eliminado) continue;
        const pid = v.entero(e.ProductoId, 'un Producto', { requeridoPositivo: true });
        const p = catalogo.find((x) => num(c(x, 'ProductoId')) === pid);
        if (!p) throw new ErrorNegocio(`El producto ${pid} no esta disponible para kits`);
        const nombre = String(c(p, 'NombreProducto') ?? '');
        if (usados.has(pid)) throw new ErrorNegocio(`El producto ${nombre} ya esta en el kit`);
        usados.add(pid);
        const venta = num(c(p, 'PrecioVenta'));
        const cant = _cantidad(e.Cantidad);
        const precio = _precio(e.Precio, venta, num(c(p, 'PrecioCosto')), nombre);
        mayorId += 1;
        // Rows.Add(_GenerarId() + 1, 0, ProductoId, NombreProducto, PrecioVenta, 1, PrecioVenta, 0, img, PrecioCosto, PrecioVenta)
        filas.push({ Id: mayorId, KitId: 0, ProductoId: pid, NombreProducto: nombre, Precio: precio, Cantidad: cant, SubTotal: cant * precio, estado: 0,
            PrecioCosto: num(c(p, 'PrecioCosto')), PrecioVentaBackup: venta });
    }
    const vigentes = filas.filter((f) => f.estado >= 0);
    return { detalle: { columnas: COLUMNAS, filas: filas.map((f) => Object.fromEntries(COLUMNAS.map((k) => [k, f[k]]))) }, total: vigentes.reduce((s, f) => s + num(f.SubTotal), 0) };
}

router.post('/', asyncHandler(async (req, res) => {
    const k = _leerCabecera(req.body || {});
    const { detalle, total } = await _armarDetalle(0, req.body.productos);
    const r = await L.InsertarKit({ _Id: '', ...k, Total: total }, detalle);
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Kit'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `Codigo de Kit ${r._numi} Grabado con Exito.`.toUpperCase() });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const k = _leerCabecera(req.body || {});
    const { detalle, total } = await _armarDetalle(id, req.body.productos);
    const r = await L.ModificarKit({ _Id: id, ...k, Total: total }, detalle);
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Kits'.toUpperCase());
    res.json({ ok: true, id, mensaje: `Codigo de Kits ${id} modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await L.L_prBorrarRegistro(id, 'MAM_Kits'))) throw new ErrorNegocio('Error al eliminar el Kits'.toUpperCase());
    res.json({ ok: true, mensaje: `Codigo de Kits ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
