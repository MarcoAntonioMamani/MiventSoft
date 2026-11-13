// =====================================================================
//  KAILY LA PAZ: Tec_Compras.vb + Tec_ComprasDetalle.vb (programa: btnCompras)  SP: MAM_Compras
//   Lista L_prListarGeneralComprasFiltro (@tipo 3 con @desde / @hasta) · detalle @tipo 4 · productos @tipo 5
//   proveedores @tipo 6 · comprobante @tipo 7 · Grabar @tipo 1 / Modificar @tipo 2 · Eliminar @tipo -1
//   Sucursales: L_fnGeneralSucursales
//  Reglas VB de Kaily:
//   * Sin Nro. Nota de Compra / Recepcion. Detalle en unidades (sin cajas ni conversion):
//     Cantidad = CantidadCompra + CantidadIncremento (bonificacion, CantidadCompra x % / 100); SubTotal = CantidadCompra x P.Costo.
//   * Sin ModificarRegistroAnteriores no se modifica ni elimina una compra de dias anteriores.
//   * No se elimina si tiene pagos de credito (transaccion = 1). TipoVenta 1 = contado, 0 = credito (usa vencimiento).
//  Mejora: el servidor arma el detalle desde los SP y recalcula subtotales y total.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { fechaReq, rango, aVB, col, num, fechaIso, limpiar, hoyBolivia } = require('../distribucion/comun');

const router = express.Router();
router.use(requierePrograma('btnCompras'));
const CAMPOS = ['Id', 'AlmacenId', 'ProveedorId', 'FechaTransaccion', 'NombreProveedor', 'TipoVenta', 'FechaVencimientoCredito',
    'TituloMoneda', 'Glosa', 'TotalCompra', 'descuento', 'transaccion'];
const r2 = (x) => Math.round(x * 100) / 100;
const r6 = (x) => Math.round(x * 1e6) / 1e6;
const nuevaFila = (columnas, valores) => {
    const o = Object.fromEntries(columnas.map((c) => [c, null]));
    for (const [k, x] of Object.entries(valores)) { const c = columnas.find((y) => y.toLowerCase() === k.toLowerCase()); if (c) o[c] = x; }
    return o;
};
const fila = (f) => ({ ...f, FechaTransaccion: fechaIso(f.FechaTransaccion), FechaVencimientoCredito: fechaIso(f.FechaVencimientoCredito), TipoVenta: v.bit(f.TipoVenta) ? 1 : 0,
    descuento: num(f.descuento), TotalCompra: num(f.TotalCompra), transaccion: num(f.transaccion) });
// _PMOGetTablaBuscador (tbDesde / tbHasta)
const lista = async (desde, hasta) => v.proyectar(await L.L_prListarGeneralComprasFiltro('MAM_Compras', desde, hasta), CAMPOS).map(fila);
async function buscarCompra(id, fechaTxt) {
    const fecha = fechaReq(fechaTxt, 'Fecha Compra');
    const c = (await lista(aVB(fecha), aVB(fecha))).find((x) => Number(x.Id) === id);
    if (!c) throw new ErrorNegocio('No se encontro la compra. Actualice la lista.', 404);
    return c;
}
// btnModificar / btnEliminar: Global_ModificarRegistro = 0 y FechaTransaccion < Global_FechaActual
function validarEditable(req, c, accion) {
    if (req.sesion.Global_ModificarRegistro !== 1 && c.FechaTransaccion < hoyBolivia()) {
        throw new ErrorNegocio(`No puede ${accion} Compras Anteriores a la fecha de Hoy, Consulte con su Administrador`.toUpperCase());
    }
}

async function usaLote() {
    const c = await L.L_prLeerConfiguracion();
    const x = c[0] ? col(c[0], 'Lote') : false;
    return x === true || Number(x) === 1;
}
async function sucursales() { return (await L.L_fnGeneralSucursales()).map((s) => ({ Id: num(col(s, 'aanumi')), Sucursal: String(col(s, 'aabdes') ?? '') })); }
async function proveedores() { return v.proyectar(await L.ListarProveedores(), ['Id', 'Nombre', 'Direccion', 'Telefono01']); }
// Tec_ComprasDetalle: Cod Externo, Descripcion, Categoria, Industria, Stock
const productosCampos = (dt) => dt.map((p) => ({ Id: num(col(p, 'Id')), CodigoExterno: col(p, 'CodigoExterno') ?? '', NombreProducto: col(p, 'NombreProducto') ?? '',
    DescripcionProducto: col(p, 'DescripcionProducto') ?? '', NombreCategoria: col(p, 'NombreCategoria') ?? '', industria: col(p, 'industria') ?? '',
    PrecioCosto: num(col(p, 'PrecioCosto')), PrecioVenta: num(col(p, 'PrecioVenta')), stock: num(col(p, 'stock')) }));
const detalleCampos = (f) => ({ Id: num(col(f, 'Id')), ProductoId: num(col(f, 'ProductoId')), Producto: col(f, 'Producto') ?? '',
    CantidadCompra: num(col(f, 'CantidadCompra')), PorcentajeIncremento: num(col(f, 'PorcentajeIncremento')), CantidadIncremento: num(col(f, 'CantidadIncremento')),
    Cantidad: num(col(f, 'Cantidad')), PrecioCosto: num(col(f, 'PrecioCosto')), PrecioVenta: num(col(f, 'PrecioVenta')), TotalCompra: num(col(f, 'TotalCompra')) });

function valores(d, nombre) {
    const cant = Number(d.CantidadCompra);
    if (!Number.isFinite(cant) || cant <= 0) throw new ErrorNegocio(`La Cantidad de ${nombre} debe ser Mayor a 0`);
    const costo = Number(d.PrecioCosto);
    if (!Number.isFinite(costo) || costo <= 0) throw new ErrorNegocio(`El Precio Costo de ${nombre} debe ser Mayor a 0`);
    const venta = Number(d.PrecioVenta);
    if (!Number.isFinite(venta) || venta < 0) throw new ErrorNegocio(`El Precio Venta de ${nombre} no puede ser negativo`);
    return { cant: r6(cant), costo: r6(costo), venta: r6(venta) };
}
// grdetalle_CellValueChanged (CantidadCompra) + P_PonerTotal
function cantidades(cant, porcentaje, costo) {
    const incremento = r6(cant * (porcentaje / 100));
    return { CantidadCompra: cant, PorcentajeIncremento: porcentaje, CantidadIncremento: incremento, Cantidad: r6(cant + incremento), TotalCompra: r2(cant * costo) };
}
function filasNuevas(columnas, pedidos, productos, siguienteId, vigentes) {
    const vistos = new Set(vigentes);
    let id = siguienteId;
    return pedidos.map((d) => {
        const pid = v.entero(d.ProductoId, 'Producto', { requeridoPositivo: true });
        if (vistos.has(pid)) throw new ErrorNegocio('El producto ya existe en el detalle'.toUpperCase());
        vistos.add(pid);
        const p = productos.find((x) => x.Id === pid);
        if (!p) throw new ErrorNegocio('Producto no disponible en la sucursal');
        const x = valores({ PrecioCosto: p.PrecioCosto, PrecioVenta: p.PrecioVenta, ...d }, p.NombreProducto);
        id += 1;
        // InsertarProductosSinLote: Lote "20200101", vencimiento 01/01/2020, costo / venta = precios del producto
        return nuevaFila(columnas, { Id: id, CompraId: 0, ProductoId: pid, Producto: p.NombreProducto, ...cantidades(x.cant, 0, x.costo), Lote: '20200101',
            FechaVencimiento: new Date(Date.UTC(2020, 0, 1)), PrecioCosto: x.costo, PrecioVenta: x.venta, img: null, estado: 0, costo: p.PrecioCosto, venta: p.PrecioVenta });
    });
}
async function leerCabecera(b) {
    const AlmacenId = Number(b.AlmacenId);
    const ProveedorId = Number(b.ProveedorId);
    const faltan = [];
    if (!(await sucursales()).some((s) => s.Id === AlmacenId)) faltan.push('Sucursal');
    if (!(await proveedores()).some((p) => Number(p.Id) === ProveedorId)) faltan.push('Proveedor');
    if (faltan.length) throw new ErrorNegocio(`Los Siguientes Campos Son Requeridos: ${faltan.join(', ')}`);
    const TipoVenta = v.bit(b.TipoVenta) ? 1 : 0;
    const fecha = fechaReq(b.FechaTransaccion, 'Fecha Transaccion');
    const venc = TipoVenta ? fecha : fechaReq(b.FechaVencimientoCredito, 'Vencimiento Credito');
    if (!TipoVenta && venc < fecha) throw new ErrorNegocio('La fecha de vencimiento del credito no puede ser menor a la fecha de compra');
    const Descuento = r2(Math.max(0, v.decimal(b.Descuento)));
    return { AlmacenId, ProveedorId, TipoVenta, FechaTransaccion: aVB(fecha), FechaVencimientoCredito: aVB(venc), Glosa: v.texto(b.Glosa, 500), Descuento, fecha };
}
// _prCalcularPrecioTotal: suma de las filas con estado >= 0 menos el descuento
function totales(cab, filas) {
    const suma = r2(filas.filter((f) => num(col(f, 'estado')) >= 0).reduce((s, f) => s + num(col(f, 'TotalCompra')), 0));
    if (cab.Descuento > suma) throw new ErrorNegocio('El descuento no puede ser mayor al total de la compra');
    return { ...cab, TotalCompra: r2(suma - cab.Descuento) };
}

// ---------------- lecturas ----------------
router.get('/', asyncHandler(async (req, res) => {
    const [desde, hasta] = rango(req.query);
    res.json({ ok: true, data: await lista(desde, hasta) });
}));
router.get('/combos', asyncHandler(async (req, res) => res.json({ ok: true, data: {
    sucursales: await sucursales(), proveedores: await proveedores(), lote: await usaLote(),
    hoy: hoyBolivia(), modificarAnteriores: req.sesion.Global_ModificarRegistro === 1,
} })));
router.get('/productos/:sucursal', asyncHandler(async (req, res) => res.json({ ok: true, data: productosCampos(await L.L_prSoloActivos(L.L_prListarProductosCompras(v.idRuta(req.params.sucursal)))) })));
router.get('/:id/detalle', asyncHandler(async (req, res) => res.json({ ok: true, data: (await L.ListaComprasDetalles(v.idRuta(req.params.id))).map(detalleCampos) })));
// Comprobante de compra (@tipo 7)
router.get('/:id/comprobante', asyncHandler(async (req, res) => res.json({ ok: true, data: limpiar(await L.ReporteCompras(v.idRuta(req.params.id))) })));

// Nota de compra (P_GenerarReporte -> Reporte_Compras con MAM_Compras @tipo 7): cabecera de la lista,
// detalle @tipo 4, codigos de @tipo 7 si los trae, empresa y logo como la nota de venta. Solo lectura.
router.get('/:id/nota', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const compra = await buscarCompra(id, req.query.fecha);
    const [det, rep, empresas, imagen, provs, sucs] = await Promise.all([
        L.ListaComprasDetalles(id), L.ReporteCompras(id).catch(() => []), L.L_prListarGeneral('MAM_Empresas'),
        L.ObtenerImagenEmpresa().catch(() => []), proveedores().catch(() => []), sucursales().catch(() => []),
    ]);
    const detalle = det.map(detalleCampos).filter((d, i) => num(col(det[i], 'estado') ?? 1) >= 0);
    const codigos = Object.fromEntries(rep.map((f) => [String(col(f, 'Producto') ?? col(f, 'NombreProducto') ?? ''), col(f, 'CodigoExterno') ?? col(f, 'Codigo') ?? null]).filter(([k, x]) => k && x !== null));
    const suma = r2(detalle.reduce((a, d) => a + d.TotalCompra, 0));
    res.json({ ok: true, data: {
        compra, detalle, codigos, subtotal: suma, total: r2(suma - num(compra.descuento)),
        proveedor: provs.find((p) => Number(p.Id) === Number(compra.ProveedorId)) || null,
        sucursal: (sucs.find((x) => x.Id === Number(compra.AlmacenId)) || {}).Sucursal || '',
        empresa: v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Nit', 'Ciudad'])[0] || null,
        imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '',
    } });
}));

// ---------------- grabar ----------------
router.post('/', asyncHandler(async (req, res) => {
    if (await usaLote()) throw new ErrorNegocio('El sistema esta configurado con LOTES: registre esta compra desde el escritorio.');
    const b = req.body || {};
    const cab = await leerCabecera(b);
    const columnas = L._columnasDe(await L.ListaComprasDetalles(-1));
    const productos = productosCampos(await L.L_prSoloActivos(L.L_prListarProductosCompras(cab.AlmacenId)));
    const filas = filasNuevas(columnas, (Array.isArray(b.detalle) ? b.detalle : []).filter((d) => !d.eliminado), productos, 0, []);
    if (!filas.length) throw new ErrorNegocio('Por Favor Inserte un Detalle'.toUpperCase());
    const r = await L.ComprasInsertar({ ...totales(cab, filas), _numi: '0' }, { columnas, filas });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar la Compra'.toUpperCase());
    res.json({ ok: true, id: r._numi, fecha: cab.fecha, mensaje: `CODIGO DE COMPRA ${r._numi} GRABADO CON EXITO.` });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    if (await usaLote()) throw new ErrorNegocio('El sistema esta configurado con LOTES: modifique esta compra desde el escritorio.');
    const id = v.idRuta(req.params.id);
    const b = req.body || {};
    validarEditable(req, await buscarCompra(id, b.fechaOriginal), 'Modificar');
    const cab = await leerCabecera(b);
    const existentes = await L.ListaComprasDetalles(id);
    const columnas = L._columnasDe(existentes);
    const enviados = Array.isArray(b.detalle) ? b.detalle : [];
    const porId = new Map(enviados.filter((d) => !d.nuevo && d.Id !== undefined).map((d) => [Number(d.Id), d]));
    const filas = existentes.map((f) => {
        const d = porId.get(num(col(f, 'Id')));
        const o = nuevaFila(columnas, { ...f, estado: 1 });
        if (!d) return o;
        if (d.eliminado) return nuevaFila(columnas, { ...o, estado: -1 });
        const a = detalleCampos(f);
        const x = valores(d, a.Producto);
        if (Math.abs(x.cant - a.CantidadCompra) < 1e-9 && Math.abs(x.costo - a.PrecioCosto) < 1e-9 && Math.abs(x.venta - a.PrecioVenta) < 1e-9) return o;
        // conserva el % de bonificacion de la fila (columna oculta en el escritorio)
        return nuevaFila(columnas, { ...o, ...cantidades(x.cant, a.PorcentajeIncremento, x.costo), PrecioCosto: x.costo, PrecioVenta: x.venta, estado: 2 });
    });
    const vigentes = filas.filter((f) => num(col(f, 'estado')) >= 0).map((f) => num(col(f, 'ProductoId')));
    const nuevos = enviados.filter((d) => d.nuevo && !d.eliminado);
    if (nuevos.length) {
        const productos = productosCampos(await L.L_prSoloActivos(L.L_prListarProductosCompras(cab.AlmacenId)));
        filas.push(...filasNuevas(columnas, nuevos, productos, filas.reduce((m, f) => Math.max(m, num(col(f, 'Id'))), 0), vigentes));
    }
    if (!filas.some((f) => num(col(f, 'estado')) >= 0)) throw new ErrorNegocio('Por Favor Inserte un Detalle'.toUpperCase());
    if (!(await L.ComprasModificar({ ...totales(cab, filas), _numi: String(id) }, { columnas, filas }))) throw new ErrorNegocio('Error al guardar la Compra'.toUpperCase());
    res.json({ ok: true, id, fecha: cab.fecha, mensaje: `CODIGO DE COMPRA ${id} MODIFICADO CON EXITO.` });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const c = await buscarCompra(id, req.query.fecha);
    validarEditable(req, c, 'Eliminar');
    if (c.transaccion === 1) throw new ErrorNegocio('No Es Posible Eliminar la Compra por que Tiene Pagos de Creditos Registrado'.toUpperCase());
    if (!(await L.L_prBorrarRegistro(id, 'MAM_Compras'))) throw new ErrorNegocio('Error al eliminar la Compra'.toUpperCase());
    res.json({ ok: true, mensaje: `CODIGO DE COMPRA ${id} ELIMINADO CON EXITO.` });
}));

module.exports = router;
