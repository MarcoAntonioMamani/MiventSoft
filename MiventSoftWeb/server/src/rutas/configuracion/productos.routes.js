// =====================================================================
//  Tec_Productos.vb  (programa: btConfProducto)   SP: MAM_Productos (TVP @TCL0064 y @PrecioT)
//
//  @PrecioT (Kaily): la tabla de ListPreciosDetalles (@tipo 10) con el precio de cada
//    categoria. El navegador envia solo { idCategoria: precio }.
//
//  TablaImagenes en VB: filas (?, ProductoId, lhima, img, estado)
//    estado 0 = nueva, 1 = existente, -1 = eliminar
//  El navegador envia solo: archivos nuevos + nombres a eliminar.
//  La tabla completa se arma aqui con L_prCargarImagenesRecepcion, igual que VB.
//  Kaily: codigo externo unico (MAM_Productos @tipo 11) y reporte de precios (@tipo 8, sp_Mam_Precios 4/9/10).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const { rutaNuevaLibreria, libreria } = require('./_libreria');
const img = require('../../utils/imagenes');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btConfProducto'));

const CAMPOS = ['Id', 'CodigoExterno', 'CodigoBarras', 'NombreProducto', 'DescripcionProducto', 'StockMinimo', 'estado',
    'CategoriaId', 'NombreCategoria', 'EmpresaId', 'Empresa', 'ProveedorId', 'MarcaId', 'AttributoId', 'FamiliaId',
    'UnidadVentaId', 'UnidadMaximaId', 'Conversion'];

router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_Productos'), CAMPOS) });
}));

router.get('/combos', asyncHandler(async (req, res) => {
    const [empresas, categorias, proveedores, marcas, atributos, familias, unidadesVenta, unidadesMaxima] = await Promise.all([
        L.L_prListaEmpresasUsuarios(), L.L_prListaCategorias(), L.L_prListarProveedores(),
        libreria(3), libreria(4), libreria(5), libreria(6), libreria(7),
    ]);
    res.json({
        ok: true,
        data: {
            empresas: v.proyectar(empresas, ['Id', 'Nombre']),
            categorias: v.proyectar(categorias, ['Id', 'NombreCategoria']),
            proveedores: v.proyectar(proveedores, ['Id', 'NombreProveedor']),
            marcas, atributos, familias, unidadesVenta, unidadesMaxima,
        },
    });
}));

// ---------------------------------------------------------------------
//  Kaily: "Reporte Precios" (ButtonX4, sin imagenes) y "Reporte Precios Imagenes" (btnReporteConImagenes)
//  AyudaMultiCategoria: Sucursal (L_fnGeneralSucursales), Categoria Precio (L_fnListarCategoriaPrecioFilter)
//  y las categorias de producto a incluir (L_fnListarCategoriaProductos, al menos una).
//  GenerarReporte: GenerarReportePrecios(CategoriaPrecio, Sucursal) y solo las filas de las categorias
//  elegidas (ExisteCategoria). Cabecera: ObtenerImagenEmpresa (imagen, Nombre, Direccion).
//  Las imagenes no se incrustan: el navegador las pide a /api/imagenes/producto.
// ---------------------------------------------------------------------
router.get('/reporte-precios/combos', asyncHandler(async (req, res) => {
    const [sucursales, preciosCat, categorias] = await Promise.all([L.L_fnGeneralSucursales(), L.L_fnListarCategoriaPrecioFilter(), L.L_fnListarCategoriaProductos()]);
    res.json({
        ok: true,
        data: {
            sucursales: v.proyectar(sucursales, ['aanumi', 'aabdes']),
            categoriasPrecio: v.proyectar(preciosCat, ['ygnumi', 'Nombre']),
            categorias: v.proyectar(categorias, ['id', 'Nombre']),
        },
    });
}));

router.get('/reporte-precios', asyncHandler(async (req, res) => {
    const categoriaPrecio = v.entero(req.query.categoriaPrecio, 'una Categoria Precio', { requeridoPositivo: true });
    const sucursal = v.entero(req.query.sucursal, 'una Sucursal');
    const elegidas = new Set(String(req.query.categorias || '').split(',').map((x) => Number.parseInt(x, 10)).filter(Number.isFinite));
    if (!elegidas.size) throw new ErrorNegocio('Debe Seleccionar Al Menos una Categoria');
    const conImagenes = req.query.imagenes === '1';
    const [dt, imagen] = await Promise.all([L.GenerarReportePrecios(categoriaPrecio, sucursal), L.ObtenerImagenEmpresa().catch(() => [])]);
    const campos = ['Id', 'CodigoExterno', 'NombreProducto', 'DescripcionProducto', 'industria', 'unidad', 'NombreCategoria', 'CategoriaId', 'Precio', ...(conImagenes ? ['Rutaimg'] : [])];
    const filas = v.proyectar(dt, campos)
        .filter((f) => elegidas.has(Number(f.CategoriaId)))
        .map((f) => (conImagenes ? { ...f, Rutaimg: f.Rutaimg ? String(f.Rutaimg).replace(/\\/g, '/').split('/').pop() : '' } : f));
    const e = imagen[0] || {};
    res.json({
        ok: true,
        data: {
            filas,
            cabecera: { imagenEmpresa: imagen.length ? String(Object.values(e)[0] ?? '') : '', Nombre: String(v.campo(e, 'Nombre') ?? ''), Direccion: String(v.campo(e, 'Direccion') ?? '') },
        },
    });
}));

// Solo los nombres de imagen activas (no los bytes)
router.get('/:id/imagenes', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const dt = v.proyectar(await L.L_prCargarImagenesRecepcion(id), ['lhima']);
    res.json({ ok: true, data: dt.map((f) => f.lhima).filter(Boolean) });
}));

// Kaily: grPrecios = ListPreciosDetalles(ProductoId)  (-1 = producto nuevo)
router.get('/:id/precios', asyncHandler(async (req, res) => {
    const id = req.params.id === '-1' ? -1 : v.idRuta(req.params.id);
    res.json({ ok: true, data: v.proyectar(await L.ListPreciosDetalles(id), ['id', 'Descripcion', 'precio']) });
}));

// btMarca(3) btAtributo(4) btFamilia(5) btUniVenta(6) btUniMaxima(7)
router.post('/libreria/:idClasificador', rutaNuevaLibreria([3, 4, 5, 6, 7]));

function _leerProducto(b) {
    const stock = Number.parseFloat(b.StockMinimo);
    const conv = Number.parseFloat(b.Conversion);
    if (!Number.isFinite(stock) || stock < 0) throw new ErrorNegocio('Ingrese valor Stock Minimo');
    if (!Number.isFinite(conv) || conv <= 0) throw new ErrorNegocio('Ingrese Valor en Conversion');
    const opc = (x) => v.entero(x, '');
    return {
        _CodigoExterno: v.texto(b.CodigoExterno, 50),
        _CodigoBarra: v.texto(b.CodigoBarras, 50),
        _NombreProducto: v.requerido(b.NombreProducto, 'Nombre Producto', 200),
        _Descripcion: v.texto(b.DescripcionProducto, 300),
        _stockMinimo: stock,
        _estado: v.bit(b.estado),
        _CategoriaId: v.entero(b.CategoriaId, 'una Categoria', { requeridoPositivo: true }),
        _EmpresaId: v.entero(b.EmpresaId, 'una Empresa', { requeridoPositivo: true }),
        _ProveedorId: opc(b.ProveedorId),
        _MarcaId: opc(b.MarcaId),
        _AttributoId: opc(b.AttributoId),
        _FamiliaId: opc(b.FamiliaId),
        _UnidadVentaId: opc(b.UnidadVentaId),
        _UnidadMaximaId: opc(b.UnidadMaximaId),
        _conversion: conv,
    };
}

function _leerEliminar(b) {
    let x = b.eliminar;
    if (x === undefined) return [];
    if (typeof x === 'string') { try { x = JSON.parse(x); } catch { x = [x]; } }
    return Array.isArray(x) ? x.map(String).slice(0, 50) : [];
}

// Arma TablaImagenes: existentes (estado 1 o -1) + nuevas (estado 0)
async function _armarTablaImagenes(idProducto, archivos, eliminar) {
    (archivos || []).forEach(img.ValidarImagen);
    const dt = await L.L_prCargarImagenesRecepcion(idProducto > 0 ? idProducto : -1);
    const cols = L._columnasDe(dt);
    const iNombre = cols.findIndex((c) => c.toLowerCase() === 'lhima');
    const iCol = cols.findIndex((c) => c.toLowerCase() === 'estado');
    const iEstado = iCol >= 0 ? iCol : 4;
    const aEliminar = new Set(eliminar);
    const borrar = [];
    const filas = dt.map((f) => {
        const fila = cols.map((c) => f[c]);
        if (iNombre >= 0 && aEliminar.has(String(fila[iNombre]))) {
            fila[iEstado] = -1;
            borrar.push(fila[iNombre]);
        }
        return fila;
    });
    const nuevas = (archivos || []).map((a, i) => ({ nombre: img.NombreImagenProducto(i), buffer: a.buffer }));
    // TablaImagenes.Rows.Add(0, ProductoId, nombre, bytes, 0)
    nuevas.forEach((n) => filas.push([0, idProducto > 0 ? idProducto : 0, n.nombre, n.buffer, 0]));
    return { tabla: { columnas: [], filas }, nuevas, borrar };
}

// Arma dtPrecio igual que CType(grPrecios.DataSource, DataTable): la tabla de ListPreciosDetalles
// (todas sus columnas, por posicion) con el precio que escribio el usuario.
// VB grPrecios_CellEdited: precio no numerico o <= 0 -> 0
async function _armarTablaPrecios(idProducto, precios) {
    const dt = await L.ListPreciosDetalles(idProducto > 0 ? idProducto : -1);
    const cols = L._columnasDe(dt);
    const iId = cols.findIndex((c) => c.toLowerCase() === 'id');
    const iPrecio = cols.findIndex((c) => c.toLowerCase() === 'precio');
    let mapa = {};
    if (typeof precios === 'string') { try { mapa = JSON.parse(precios) || {}; } catch { mapa = {}; } }
    else if (precios && typeof precios === 'object') mapa = precios;
    const filas = dt.map((f) => {
        const fila = cols.map((c) => f[c]);
        const k = iId >= 0 ? String(fila[iId]) : null;
        if (iPrecio >= 0 && k !== null && Object.prototype.hasOwnProperty.call(mapa, k)) {
            const n = Number.parseFloat(mapa[k]);
            fila[iPrecio] = Number.isFinite(n) && n > 0 ? n : 0;
        }
        return fila;
    });
    return { columnas: [], filas };
}

// Kaily _PMOValidarCampos: codigo externo repetido (ObtenerCoincidenciasCodigoExterno, MAM_Productos @tipo 11)
//  Nuevo: con 1 coincidencia ya existe · Modificar: el propio producto cuenta, error con mas de 1 (igual que el escritorio)
async function _validarCodigoExterno(codigo, nuevo) {
    if (!codigo) return;
    const dt = await L.ObtenerCoincidenciasCodigoExterno(codigo);
    if (nuevo && dt.length > 0) {
        throw new ErrorNegocio(`Ya existe un Producto con un mismo codigo Externo= ${codigo} En el Producto ${v.campo(dt[0], 'id') ?? ''}  ${v.campo(dt[0], 'DescripcionProducto') ?? ''}`.trim());
    }
    if (!nuevo && dt.length > 1) throw new ErrorNegocio('Ya existe un Producto con un mismo codigo Externo');
}

// _prGuardarImagenes: despues de grabar en BD, escribe/borra archivos
async function _guardarArchivos({ nuevas, borrar }) {
    for (const n of nuevas) await img.GuardarImagenConNombre('producto', n.nombre, n.buffer);
    for (const b of borrar) await img.EliminarImagen('producto', b);
}

router.post('/', img.subirImagenes, asyncHandler(async (req, res) => {
    const p = _leerProducto(req.body);
    await _validarCodigoExterno(p._CodigoExterno, true);
    const t = await _armarTablaImagenes(0, req.files, []);
    const precios = await _armarTablaPrecios(0, req.body.precios);
    const r = await L.L_prProductoInsertar({ _numi: '', ...p }, t.tabla, precios);
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el producto'.toUpperCase());
    await _guardarArchivos(t);
    res.json({ ok: true, id: r._numi, mensaje: `Codigo de Producto ${r._numi} Grabado con Exito.`.toUpperCase() });
}));

router.put('/:id', img.subirImagenes, asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const p = _leerProducto(req.body);
    await _validarCodigoExterno(p._CodigoExterno, false);
    const t = await _armarTablaImagenes(id, req.files, _leerEliminar(req.body));
    const precios = await _armarTablaPrecios(id, req.body.precios);
    const r = await L.L_prProductoModificar({ _numi: id, ...p }, t.tabla, precios);
    if (!r.resultado) throw new ErrorNegocio('Error al modificar el producto'.toUpperCase());
    await _guardarArchivos(t);
    res.json({ ok: true, id, mensaje: `Codigo de Producto ${id} modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const r = await L.L_prProductoBorrar(id);
    if (!r.resultado) throw new ErrorNegocio(r._mensaje);
    res.json({ ok: true, mensaje: `Codigo de Producto ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
