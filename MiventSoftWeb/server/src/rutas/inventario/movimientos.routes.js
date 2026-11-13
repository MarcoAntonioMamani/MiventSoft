// =====================================================================
//  KAILY LA PAZ: Tec_Movimientos.vb + Tec_MovimientoDetalle.vb (programa: btInvMovimiento)  SP: MAM_Movimientos
//   Lista L_prListarGeneral (@tipo 3) · detalle @tipo 4 · tipos @tipo 5 · productos con stock @tipo 7
//   Grabar @tipo 1 (con @DepositoIdDestino / @IdMovimientoDestino) / Modificar @tipo 2 (@detalle con estados 0/1/2/-1)
//   Eliminar L_prBorrarRegistroMovimiento (@tipo -1 + @ConceptoId) · comprobante ListarReporteMovimientoProductos (@tipo 17)
//   Depositos: MAM_Almacenes @tipo 4
//  Reglas del escritorio de Kaily:
//   * 4 = INGRESO: sin limite de stock y acepta cantidades negativas (reseteo de inventario a cero).
//   * 8 = TRASPASO: deposito destino distinto al origen; graba la salida (8) y luego el ingreso (7) en el
//     destino con @IdMovimientoDestino = Id de la salida (_prGuardarTraspaso). No se modifica: se elimina y se vuelve a crear.
//   * Los demas no superan el stock. Las filas con Cantidad 0 no se envian (_fnDetalleParaGrabar).
//   * Sin ModificarRegistroAnteriores no se modifica ni elimina un movimiento de dias anteriores.
//  Igual que Ventas: si la configuracion usa LOTES se avisa (la web aun no maneja lotes).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { fechaReq, aVB, col, num, fechaIso, hoyBolivia } = require('../distribucion/comun');

const router = express.Router();
router.use(requierePrograma('btInvMovimiento'));
const INGRESO = 4;
const TRASPASO = 8;            // Traspaso salida (origen)
const TRASPASO_INGRESO = 7;    // Traspaso ingreso (destino), lo graba el sistema
const CAMPOS = ['Id', 'fdoc', 'concep', 'nconcep', 'obs', 'est', 'alm', 'NombreDeposito'];
const fila = (f) => ({ ...f, fdoc: fechaIso(f.fdoc) });
const nuevaFila = (columnas, valores) => {
    const o = Object.fromEntries(columnas.map((c) => [c, null]));
    for (const [k, x] of Object.entries(valores)) { const c = columnas.find((y) => y.toLowerCase() === k.toLowerCase()); if (c) o[c] = x; }
    return o;
};
// Global_ModificarRegistro de la sesion (Tec_Login.vb)
const modificarAnteriores = (req) => req.sesion?.Global_ModificarRegistro === 1;

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
// Cantidad de una fila: 0 = no se graba (null); ingreso acepta negativos; los demas no superan el stock
function cantidadValida(c, stock, tipo, nombre) {
    const n = Number(c);
    if (!Number.isFinite(n)) throw new ErrorNegocio(`La Cantidad de ${nombre} no es valida`);
    if (n === 0) return null;
    if (tipo !== INGRESO && n < 0) throw new ErrorNegocio(`La Cantidad de ${nombre} debe ser Mayor a 0`);
    if (tipo !== INGRESO && n > (stock === Infinity ? Infinity : num(stock)) + 1e-9) throw new ErrorNegocio(`La cantidad que se quiere sacar es mayor a la que existe en el stock solo puede Sacar : ${num(stock)} (${nombre})`.toUpperCase());
    return n;
}

async function leerCabecera(b) {
    const c = await combos();
    const ConceptoId = Number(b.ConceptoId);
    if (!c.tipos.some((t) => Number(t.Id) === ConceptoId)) throw new ErrorNegocio('Seleccione Tipo Movimiento'.toUpperCase());
    const DepositoId = Number(b.DepositoId);
    if (!c.depositos.some((d) => Number(d.Id) === DepositoId)) throw new ErrorNegocio('Seleccion Deposito'.toUpperCase());
    const cab = { ConceptoId, DepositoId, Observacion: v.texto(b.Observacion, 500), FechaDocumento: aVB(fechaReq(b.FechaDocumento, 'Fecha Transaccion')) };
    if (ConceptoId === TRASPASO) {
        const destino = Number(b.DepositoIdDestino);
        if (!c.depositos.some((d) => Number(d.Id) === destino)) throw new ErrorNegocio('Seleccione el Deposito Destino'.toUpperCase());
        if (destino === DepositoId) throw new ErrorNegocio('El deposito Destino debe ser Diferente al Origen'.toUpperCase());
        cab.DepositoIdDestino = destino;
    }
    return cab;
}
function filasNuevas(columnas, pedidos, productos, tipo, siguienteId, vigentes) {
    const vistos = new Set(vigentes);
    let id = siguienteId;
    const filas = [];
    for (const d of pedidos) {
        const pid = v.entero(d.ProductoId, 'Producto', { requeridoPositivo: true });
        if (vistos.has(pid)) throw new ErrorNegocio('El producto ya existe en el detalle'.toUpperCase());
        vistos.add(pid);
        const p = productos.find((x) => x.Id === pid);
        if (!p) throw new ErrorNegocio('Producto no disponible en el deposito');
        const cant = cantidadValida(d.Cantidad, p.stock, tipo, p.NombreProducto);
        if (cant === null) continue; // Agregar todos: las filas en 0 no se graban
        id += 1;
        filas.push(nuevaFila(columnas, { Id: id, MovimientoId: 0, ProductoId: pid, Producto: p.DescripcionProducto || p.NombreProducto, Cantidad: cant,
            Lote: '20200101', FechaVencimiento: new Date(Date.UTC(2020, 0, 1)), img: null, estado: 0, stock: p.stock }));
    }
    return filas;
}
const ERROR_SIN_CANTIDAD = 'Ingrese la Cantidad de al menos un Producto'.toUpperCase();

// Salidas (y un ingreso con cantidad negativa): se juntan TODOS los productos sin stock suficiente y se avisan juntos.
// El SP (MAM_Movimientos) vuelve a validar con las filas de stock bloqueadas: si otro usuario saco el producto
// recien, el SP lo rechaza igual (web y escritorio).
function validarStock(pedidos, productos, tipo) {
    const faltan = [];
    for (const d of pedidos) {
        const p = productos.find((x) => x.Id === Number(d.ProductoId));
        const n = Number(d.Cantidad);
        if (!p || !Number.isFinite(n) || n === 0) continue;
        const sale = tipo === INGRESO ? -n : n; // lo que descuenta del deposito
        if (sale > 0 && sale > num(p.stock) + 1e-9) {
            faltan.push({ ProductoId: p.Id, Producto: String(p.DescripcionProducto || p.NombreProducto).trim(), stock: num(p.stock), solicitado: sale });
        }
    }
    if (faltan.length) throw L.L_prErrorSinStock(faltan, 'grabar el movimiento');
}

async function buscarMovimiento(id) {
    const f = v.proyectar(await L.L_prListarGeneral('MAM_Movimientos'), CAMPOS).map(fila).find((x) => Number(x.Id) === id);
    if (!f) throw new ErrorNegocio('No se encontro el movimiento. Actualice la lista.', 404);
    return f;
}
// EditarToolStripMenuItem / EliminarToolStripMenuItem: Global_ModificarRegistro = 0 y fdoc < Global_FechaActual
function validarFecha(req, f, accion) {
    if (!modificarAnteriores(req) && f.fdoc < hoyBolivia()) {
        throw new ErrorNegocio(`No puede ${accion} Movimientos Anteriores a la fecha de Hoy, Consulte con su Administrador`.toUpperCase());
    }
}

// Traspaso (8): deposito destino para mostrarlo al visualizar. Si el SP @tipo 3 trae DepositoIdDestino o
// IdMovimientoDestino se usan; si no, el destino es el deposito del ingreso (7) que _prGuardarTraspaso graba
// justo despues de la salida: el siguiente 7 del mismo dia, con la misma descripcion y en otro deposito.
function conDestinoTraspaso(crudas) {
    const filas = v.proyectar(crudas, CAMPOS).map(fila);
    const dato = (i, c) => num(col(crudas[i], c));
    const porId = (a, b) => Number(filas[a].Id) - Number(filas[b].Id);
    const de = (concep) => filas.map((_, i) => i).filter((i) => Number(filas[i].concep) === concep).sort(porId);
    const ingresos = de(TRASPASO_INGRESO);
    const libres = new Set(ingresos);
    for (const s of de(TRASPASO)) {
        const S = filas[s];
        const j = ingresos.find((k) => libres.has(k) && dato(k, 'IdMovimientoDestino') === Number(S.Id))
            ?? ingresos.find((k) => libres.has(k) && !dato(k, 'IdMovimientoDestino') && Number(filas[k].Id) > Number(S.Id)
                && filas[k].fdoc === S.fdoc && Number(filas[k].alm) !== Number(S.alm) && (filas[k].obs ?? '') === (S.obs ?? ''));
        if (j !== undefined) libres.delete(j);
        S.almDestino = dato(s, 'DepositoIdDestino') || (j !== undefined ? num(filas[j].alm) : null) || null;
    }
    return filas;
}

// ---------------- lecturas ----------------
router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: conDestinoTraspaso(await L.L_prListarGeneral('MAM_Movimientos')) });
}));
router.get('/combos', asyncHandler(async (req, res) => res.json({ ok: true, data: {
    ...(await combos()), lote: await usaLote(), modificarAnteriores: modificarAnteriores(req), hoy: hoyBolivia(), traspaso: TRASPASO, ingreso: INGRESO,
} })));
router.get('/productos/:deposito', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: productosCampos(await L.L_prSoloActivos(L.L_prListarProductosLote(v.idRuta(req.params.deposito)))) });
}));
router.get('/:id/detalle', asyncHandler(async (req, res) => {
    const dt = await L.L_prListarDetalleMovimiento(v.idRuta(req.params.id));
    res.json({ ok: true, data: dt.map((f) => ({ Id: num(col(f, 'Id')), ProductoId: num(col(f, 'ProductoId')), Producto: col(f, 'Producto') ?? '', Cantidad: num(col(f, 'Cantidad')), stock: num(col(f, 'stock')) })) });
}));

// P_GenerarReporte: Reporte_Movimiento con ListarReporteMovimientoProductos (columnas por nombre, sin importar mayusculas)
router.get('/:id/comprobante', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const [dt, imagen, empresas] = await Promise.all([L.ListarReporteMovimientoProductos(id), L.ObtenerImagenEmpresa().catch(() => []), L.L_prListarGeneral('MAM_Empresas').catch(() => [])]);
    const de = (f, ...n) => { for (const x of n) { const k = Object.keys(f).find((y) => y.toLowerCase() === x.toLowerCase()); if (k !== undefined && f[k] !== null && f[k] !== '') return f[k]; } return null; };
    const r0 = dt[0] || {};
    const e0 = v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Nit', 'Ciudad'])[0] || {};
    res.json({ ok: true, data: {
        filas: dt.map((f) => ({
            Item: de(f, 'CodigoExterno', 'ProductoId', 'Item') ?? '',
            Producto: String(de(f, 'Producto', 'NombreProducto', 'DescripcionProducto') ?? ''),
            Cantidad: num(de(f, 'Cantidad')),
        })),
        cab: { Id: id, Fecha: fechaIso(de(r0, 'FechaDocumento', 'fdoc', 'Fecha')), Tipo: de(r0, 'TipoMovimiento', 'nconcep', 'Concepto', 'Descripcion'),
            Deposito: de(r0, 'NombreDeposito', 'Deposito'), DepositoDestino: de(r0, 'DepositoDestino', 'NombreDepositoDestino'), Observacion: de(r0, 'Observacion', 'obs') },
        empresa: { Nombre: e0.Nombre || '', Direccion: e0.Direccion || '', Telefono: e0.Telefono || '', Nit: e0.Nit || '', Ciudad: e0.Ciudad || '' },
        imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '',
    } });
}));

// ---------------- grabar ----------------
router.post('/', asyncHandler(async (req, res) => {
    if (await usaLote()) throw new ErrorNegocio('El sistema esta configurado con LOTES: registre este movimiento desde el escritorio.');
    const b = req.body || {};
    const cab = await leerCabecera(b);
    const columnas = L._columnasDe(await L.L_prListarDetalleMovimiento(-1));
    const pedidos = (Array.isArray(b.detalle) ? b.detalle : []).filter((d) => !d.eliminado);
    if (!pedidos.length) throw new ErrorNegocio('Por Favor Inserte un Detalle'.toUpperCase());
    const productos = productosCampos(await L.L_prSoloActivos(L.L_prListarProductosLote(cab.DepositoId)));
    validarStock(pedidos, productos, cab.ConceptoId);
    const filas = filasNuevas(columnas, pedidos, productos, cab.ConceptoId, 0, []);
    if (!filas.length) throw new ErrorNegocio(ERROR_SIN_CANTIDAD);
    const dt = { columnas, filas };

    // _PMOGrabarRegistro: movimiento normal con destino 0 / 0
    if (cab.ConceptoId !== TRASPASO) {
        const r = await L.L_prMovimientoInsertar({ ...cab, DepositoIdDestino: 0, IdMovimientoDestino: 0, _numi: '' }, dt);
        if (!r.resultado) throw new ErrorNegocio('Error al guardar el Movimiento'.toUpperCase());
        return res.json({ ok: true, id: r._numi, mensaje: `CODIGO DE MOVIMIENTO ${r._numi} GRABADO CON EXITO.` });
    }
    // _prGuardarTraspaso: salida del origen y luego el ingreso (7) en el destino enlazado a la salida
    const r = await L.L_prMovimientoInsertar({ ...cab, IdMovimientoDestino: 0, _numi: '' }, dt);
    if (!r.resultado) throw new ErrorNegocio('Error al Insertar Movimiento'.toUpperCase());
    const rd = await L.L_prMovimientoInsertar({ ...cab, ConceptoId: TRASPASO_INGRESO, DepositoId: cab.DepositoIdDestino, DepositoIdDestino: cab.DepositoId, IdMovimientoDestino: r._numi, _numi: '' }, dt);
    if (!rd.resultado) throw new ErrorNegocio(`Se grabo la salida ${r._numi} pero no el ingreso en el deposito destino. Elimine el movimiento ${r._numi} y vuelva a crearlo.`.toUpperCase());
    res.json({ ok: true, id: r._numi, idDestino: rd._numi, mensaje: `CODIGO DE MOVIMIENTO ${r._numi} GRABADO CON EXITO.` });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    if (await usaLote()) throw new ErrorNegocio('El sistema esta configurado con LOTES: modifique este movimiento desde el escritorio.');
    const id = v.idRuta(req.params.id);
    const actual = await buscarMovimiento(id);
    validarFecha(req, actual, 'Modificar');
    if (Number(actual.concep) === TRASPASO) throw new ErrorNegocio('No Se puede Modificar un Movimiento de Traspaso. Elimine el Movimiento y Vuelva a Crearlo'.toUpperCase());
    const b = req.body || {};
    const cab = await leerCabecera({ ...b, ConceptoId: b.ConceptoId ?? actual.concep });
    if (cab.ConceptoId === TRASPASO) throw new ErrorNegocio('No Se puede Modificar un Movimiento de Traspaso. Elimine el Movimiento y Vuelva a Crearlo'.toUpperCase());
    const existentes = await L.L_prListarDetalleMovimiento(id);
    const columnas = L._columnasDe(existentes);
    const enviados = Array.isArray(b.detalle) ? b.detalle : [];
    const porId = new Map(enviados.filter((d) => !d.nuevo && d.Id !== undefined).map((d) => [Number(d.Id), d]));
    const filas = [];
    for (const f of existentes) {
        const d = porId.get(num(col(f, 'Id')));
        const o = nuevaFila(columnas, { ...f, estado: 1 });
        if (!d) { filas.push(o); continue; }
        if (d.eliminado) { filas.push(nuevaFila(columnas, { ...o, estado: -1 })); continue; }
        const st = col(f, 'stock');
        const cant = cantidadValida(d.Cantidad, st === undefined || st === null ? Infinity : st, cab.ConceptoId, col(f, 'Producto'));
        if (cant === null) continue; // _fnDetalleParaGrabar quita las filas en 0
        filas.push(Math.abs(cant - num(col(f, 'Cantidad'))) < 1e-9 ? o : nuevaFila(columnas, { ...o, Cantidad: cant, estado: 2 }));
    }
    const vigentes = filas.filter((f) => num(col(f, 'estado')) >= 0).map((f) => num(col(f, 'ProductoId')));
    const nuevos = enviados.filter((d) => d.nuevo && !d.eliminado);
    if (nuevos.length) {
        const productos = productosCampos(await L.L_prSoloActivos(L.L_prListarProductosLote(cab.DepositoId)));
        filas.push(...filasNuevas(columnas, nuevos, productos, cab.ConceptoId, existentes.reduce((m, f) => Math.max(m, num(col(f, 'Id'))), 0), vigentes));
    }
    if (!filas.some((f) => num(col(f, 'estado')) >= 0 && num(col(f, 'ProductoId')) > 0)) throw new ErrorNegocio(ERROR_SIN_CANTIDAD);
    if (!(await L.L_prMovimientoActualizar({ ...cab, _numi: String(id) }, { columnas, filas }))) throw new ErrorNegocio('Error al guardar el Movimiento'.toUpperCase());
    res.json({ ok: true, id, mensaje: `CODIGO DE MOVIMIENTO ${id} MODIFICADO CON EXITO.` });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const actual = await buscarMovimiento(id);
    validarFecha(req, actual, 'Eliminar');
    if (!(await L.L_prBorrarRegistroMovimiento(id, 'MAM_Movimientos', Number(actual.concep)))) throw new ErrorNegocio('Error al eliminar el Movimiento'.toUpperCase());
    res.json({ ok: true, mensaje: `CODIGO DE MOVIMIENTO ${id} ELIMINADO CON EXITO.` });
}));

module.exports = router;
