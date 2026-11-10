// =====================================================================
//  Tec_Despachos.vb + Tec_DespachoDetalle.vb (programa: btnDespacho)  SP: MAM_DespachoProductos
//   Lista            L_prListarGeneralFechas (@tipo 3)       Detalle  @tipo 4
//   Productos        @tipo 5 (ListarProductosSeleccionables, sucursal 1)
//   Despacho automatico (btnAgregarGastos): @tipo 7 productos de las ventas + @tipo 8 referencia de ventas
//   Grabar @tipo 1 (@DetalleDespacho + @ReferenciaVenta)   Modificar @tipo 2   Eliminar @tipo -1
//   Nota de despacho @tipo 6
//  Reglas del escritorio: no se edita ni elimina si la conciliacion esta cerrada; un despacho
//  automatico solo se elimina. TipoMovimiento = 5 (salida), Sucursal = 1.
//  Mejoras: el servidor arma el detalle (estados 0/1/2/-1/-2) desde los SP y valida stock y
//  cantidades; del navegador solo recibe ProductoId / Id y Cantidad.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { fechaReq, rango, aVB, col, num, fechaIso, limpiar } = require('./comun');

const router = express.Router();
router.use(requierePrograma('btnDespacho'));

const SUCURSAL = 1;          // Dim SucursalId As Integer = 1 / ListarProductosSeleccionables(1)
const MOV_SALIDA = 5;        // Dim MovimientoSalidId As Integer = 5
const CAMPOS = ['Id', 'PersonalId', 'NombrePersonal', 'ConciliacionId', 'SucursalId', 'Fecha', 'NroNota', 'Detalle', 'TipoMovimientoId', 'EstadoConciliacion', 'DespachoAutomatico'];
const fila = (f) => ({ ...f, Fecha: fechaIso(f.Fecha), EstadoConciliacion: f.EstadoConciliacion === null || f.EstadoConciliacion === undefined ? null : num(f.EstadoConciliacion), DespachoAutomatico: num(f.DespachoAutomatico) });
const CAMPOS_DET = ['Id', 'ProductoId', 'NombreProducto', 'Cantidad', 'CantidadCaja', 'conversion', 'Stock'];
const CAMPOS_PROD = ['Id', 'CodigoExterno', 'NombreProducto', 'DescripcionProducto', 'NombreCategoria', 'stock', 'conversion'];
const cerrada = (f) => f.EstadoConciliacion !== null && Number(f.EstadoConciliacion) === 0;
const MSJ_CERRADA = 'La Conciliacion Ya Esta Cerrada No Es Posible Editar Este Despacho. Primero Debe Revertir El Estado De La Conciliacion';
const r6 = (x) => Math.round(x * 1e6) / 1e6;

async function buscar(id, fecha) {
    const f = v.proyectar(await L.L_prListarGeneralFechas('MAM_DespachoProductos', aVB(fecha), aVB(fecha)), CAMPOS).find((x) => Number(x.Id) === id);
    if (!f) throw new ErrorNegocio(`No se encontro el Despacho ${id}. Actualice la lista e intente de nuevo.`, 404);
    return fila(f);
}
async function personalValido(id) {
    const n = v.entero(id, 'Personal', { requeridoPositivo: true });
    if (!(await L.ListarPersonal()).some((p) => Number(col(p, 'Id')) === n)) throw new ErrorNegocio('Seleccione Personal');
    return n;
}
function cantidadValida(c, stock, nombre) {
    const n = Number(c);
    if (!Number.isFinite(n) || n <= 0) throw new ErrorNegocio(`La Cantidad de ${nombre} debe ser Mayor a 0`);
    if (n > num(stock) + 1e-9) throw new ErrorNegocio(`La Cantidad = ${n} es mayor al Stock del Producto ${nombre} = ${num(stock)}`);
    return n;
}

/** Fila del tipo tabla con TODAS las columnas del SELECT (por nombre, sin distinguir mayusculas) */
function nuevaFila(columnas, valores) {
    const o = Object.fromEntries(columnas.map((c) => [c, null]));
    for (const [k, x] of Object.entries(valores)) { const c = columnas.find((y) => y.toLowerCase() === k.toLowerCase()); if (c) o[c] = x; }
    return o;
}
// Tec_DespachoDetalle.seleccionarProducto: Rows.Add(Id, 0, ProductoId, Nombre, cant, cant/conv, stock, 0, conv, img)
function filasNuevas(columnas, pedidos, productos, siguienteId, yaEnDetalle) {
    const vistos = new Set(yaEnDetalle);
    let id = siguienteId;
    return pedidos.map((d) => {
        const pid = v.entero(d.ProductoId, 'Producto', { requeridoPositivo: true });
        if (vistos.has(pid)) throw new ErrorNegocio('El producto ya existe en el detalle'.toUpperCase());
        vistos.add(pid);
        const p = productos.find((x) => Number(col(x, 'Id')) === pid);
        if (!p) throw new ErrorNegocio('Producto no disponible para despacho');
        const nombre = col(p, 'NombreProducto');
        if (!(num(col(p, 'stock')) > 0)) throw new ErrorNegocio(`El Producto ${nombre} no Cuenta con Stock Disponible`);
        const cant = cantidadValida(d.Cantidad, col(p, 'stock'), nombre);
        const conv = num(col(p, 'conversion')) || 1;
        id += 1;
        return nuevaFila(columnas, { Id: id, DespachoProductosId: 0, ProductoId: pid, NombreProducto: nombre, Cantidad: cant, CantidadCaja: r6(cant / conv), Stock: col(p, 'stock'), estado: 0, conversion: conv, img: null });
    });
}

// ---------------- lecturas ----------------
router.get('/', asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneralFechas('MAM_DespachoProductos', d, h), CAMPOS).map(fila) });
}));
router.get('/personal', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.ListarPersonal(), ['Id', 'Nombre', 'Direccion', 'Telefono01']) });
}));
router.get('/productos', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.ListarProductosSeleccionables(SUCURSAL), CAMPOS_PROD) });
}));
// btnAgregarGastos: productos de las ventas del personal (despacho automatico)
router.get('/automatico/:personal', asyncHandler(async (req, res) => {
    const dt = await L.CargarProductosAutomatico(v.idRuta(req.params.personal), SUCURSAL);
    if (!dt.length) throw new ErrorNegocio('No hay Productos Para Agregar'.toUpperCase());
    res.json({ ok: true, data: v.proyectar(dt, CAMPOS_DET) });
}));
router.get('/:id/detalle', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.ListaDetalleDespachoProductos(v.idRuta(req.params.id)), CAMPOS_DET) });
}));
// ImprimirNotaSalida (Reporte_Despacho): MAM_DespachoProductos @tipo 6 (solo lectura) + empresa y logo.
// Columnas por nombre sin importar mayusculas (lo que devuelva el SP): Item/ProductoId, NombreProducto, Cajas, Cantidad
// y la cabecera (fecha, distribuidor, conciliacion, detalle) si el SP la trae en cada fila.
router.get('/:id/reporte', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const [dt, imagen, empresas] = await Promise.all([GenerarDatos(id), L.ObtenerImagenEmpresa().catch(() => []), L.L_prListarGeneral('MAM_Empresas').catch(() => [])]);
    const de = (f, ...n) => { for (const x of n) { const k = Object.keys(f).find((y) => y.toLowerCase() === x.toLowerCase()); if (k !== undefined && f[k] !== null && f[k] !== '') return f[k]; } return null; };
    const r0 = dt[0] || {};
    const filas = dt.map((f) => ({
        Item: de(f, 'Item', 'ProductoId', 'IdProducto', 'CodigoExterno') ?? '',
        Producto: String(de(f, 'NombreProducto', 'Producto', 'Descripcion') ?? ''),
        Cajas: de(f, 'Cajas', 'CantidadCaja', 'CantidadCajas'),
        Cantidad: num(de(f, 'Cantidad', 'CantidadUnitaria')),
    }));
    const e0 = v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Nit', 'Ciudad'])[0] || {};
    const imgEmp = imagen[0] || {};
    res.json({ ok: true, data: {
        filas,
        cab: { Id: num(de(r0, 'Id', 'DespachoId', 'DespachoProductosId')) || id, Fecha: fechaIso(de(r0, 'Fecha', 'FechaDespacho')), NombrePersonal: de(r0, 'NombrePersonal', 'Distribuidor', 'Personal'),
            ConciliacionId: de(r0, 'ConciliacionId', 'NroConciliacion'), Detalle: de(r0, 'Detalle', 'Glosa') },
        empresa: { Nombre: e0.Nombre || imgEmp.Nombre || '', Direccion: e0.Direccion || imgEmp.Direccion || '', Telefono: e0.Telefono || '', Nit: e0.Nit || '', Ciudad: e0.Ciudad || '' },
        imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '',
    } });
}));
const GenerarDatos = (id) => L.GenerarDatosDespachoReporte(id);

// ---------------- grabar ----------------
router.post('/', asyncHandler(async (req, res) => {
    const b = req.body || {};
    const PersonalId = await personalValido(b.PersonalId);
    const Fecha = fechaReq(b.Fecha, 'Fecha Salida');
    const Detalle = v.texto(b.Detalle, 500);
    const automatico = v.bit(b.automatico) ? 1 : 0;
    const pedidos = Array.isArray(b.detalle) ? b.detalle.filter((d) => !d.eliminado) : [];
    const productos = await L.ListarProductosSeleccionables(SUCURSAL);

    let columnas;
    let filas;
    if (automatico) {
        // la grilla es la tabla de @tipo 7; se respetan cambios de cantidad / quitados (estado -2)
        const base = await L.CargarProductosAutomatico(PersonalId, SUCURSAL);
        if (!base.length) throw new ErrorNegocio('No hay Productos Para Agregar'.toUpperCase());
        columnas = L._columnasDe(base);
        const porProd = new Map((b.detalle || []).map((d) => [Number(d.ProductoId), d]));
        filas = base.map((f) => {
            const pid = num(col(f, 'ProductoId'));
            const d = porProd.get(pid);
            const o = nuevaFila(columnas, { ...f, estado: 0 });
            if (!d) return o;
            if (d.eliminado) return nuevaFila(columnas, { ...o, estado: -2 });
            const cant = cantidadValida(d.Cantidad, col(f, 'Stock'), col(f, 'NombreProducto'));
            const conv = num(col(f, 'conversion')) || 1;
            return nuevaFila(columnas, { ...o, Cantidad: cant, CantidadCaja: r6(cant / conv) });
        });
        const baseIds = new Set(filas.map((f) => num(col(f, 'ProductoId'))));
        const maxId = filas.reduce((m, f) => Math.max(m, num(col(f, 'Id'))), 0);
        filas.push(...filasNuevas(columnas, pedidos.filter((d) => !baseIds.has(Number(d.ProductoId))), productos, maxId, baseIds));
    } else {
        columnas = L._columnasDe(await L.ListaDetalleDespachoProductos(-1));
        filas = filasNuevas(columnas, pedidos, productos, 0, []);
    }
    if (!filas.some((f) => num(col(f, 'estado')) >= 0)) throw new ErrorNegocio('Por Favor Inserte un Detalle');
    const ref = await L.CargarReferenciaVentas(automatico ? PersonalId : -1);

    const r = await L.InsertarDespachoProductos(
        { _Id: '', PersonalId, ConciliacionId: 0, SucursalId: SUCURSAL, Fecha: aVB(Fecha), NroNota: '', Detalle, TipoMovimientoID: MOV_SALIDA, DespachoAutomatico: automatico },
        { columnas, filas }, { columnas: L._columnasDe(ref), filas: ref },
    );
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Despacho'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `CODIGO DE DESPACHO ${r._numi} GRABADO CON EXITO.` });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const b = req.body || {};
    const actual = await buscar(id, fechaReq(b.fechaOriginal, 'Fecha original'));
    if (cerrada(actual)) throw new ErrorNegocio(MSJ_CERRADA);
    if (actual.DespachoAutomatico === 1) throw new ErrorNegocio('Debe Eliminarse este despacho ya que es un Despacho Automatico');
    const PersonalId = await personalValido(b.PersonalId);
    const Fecha = fechaReq(b.Fecha, 'Fecha Salida');
    const Detalle = v.texto(b.Detalle, 500);

    const existentes = await L.ListaDetalleDespachoProductos(id);
    const columnas = L._columnasDe(existentes);
    const enviados = Array.isArray(b.detalle) ? b.detalle : [];
    const porId = new Map(enviados.filter((d) => d.Id !== undefined && d.Id !== null && !d.nuevo).map((d) => [Number(d.Id), d]));
    const filas = existentes.map((f) => {
        const d = porId.get(num(col(f, 'Id')));
        const o = nuevaFila(columnas, { ...f, estado: 1 });
        if (!d) return o; // sin cambios
        if (d.eliminado) return nuevaFila(columnas, { ...o, estado: -1 });
        const cant = cantidadValida(d.Cantidad, col(f, 'Stock'), col(f, 'NombreProducto'));
        if (Math.abs(cant - num(col(f, 'Cantidad'))) < 1e-9) return o;
        const conv = num(col(f, 'conversion')) || 1;
        return nuevaFila(columnas, { ...o, Cantidad: cant, CantidadCaja: r6(cant / conv), estado: 2 });
    });
    const vigentes = filas.filter((f) => num(col(f, 'estado')) >= 0).map((f) => num(col(f, 'ProductoId')));
    const maxId = filas.reduce((m, f) => Math.max(m, num(col(f, 'Id'))), 0);
    const nuevos = enviados.filter((d) => d.nuevo && !d.eliminado);
    if (nuevos.length) filas.push(...filasNuevas(columnas, nuevos, await L.ListarProductosSeleccionables(SUCURSAL), maxId, vigentes));
    if (!filas.some((f) => num(col(f, 'estado')) >= 0)) throw new ErrorNegocio('Por Favor Inserte un Detalle');

    const ok = await L.ModificarDespachoProductos(
        { _Id: String(id), PersonalId, ConciliacionId: num(actual.ConciliacionId), SucursalId: num(actual.SucursalId) || SUCURSAL, Fecha: aVB(Fecha), NroNota: String(id), Detalle, TipoMovimientoID: MOV_SALIDA },
        { columnas, filas },
    );
    if (!ok) throw new ErrorNegocio('Error al guardar el Despacho'.toUpperCase());
    res.json({ ok: true, id, mensaje: `CODIGO DE DESPACHO ${id} MODIFICADO CON EXITO.` });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const actual = await buscar(id, fechaReq(req.query.fecha, 'Fecha'));
    if (cerrada(actual)) throw new ErrorNegocio(MSJ_CERRADA);
    if (!(await L.L_prBorrarRegistro(id, 'MAM_DespachoProductos'))) throw new ErrorNegocio('Error al eliminar el Despacho'.toUpperCase());
    res.json({ ok: true, mensaje: `CODIGO DE DESPACHO ${id} ELIMINADO CON EXITO.` });
}));

module.exports = router;
