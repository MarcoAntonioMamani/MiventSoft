// =====================================================================
//  Tec_Conciliacion.vb (programa: btnConciliaciones)  SP: MAM_Conciliacion
//   Lista        L_prListarGeneralFechas (@tipo 3)
//   Detalle      @tipo 4 (productos) + @tipo 5 (salidas) + @tipo 6 (entregado, si esta ABIERTA)
//                                                       o @tipo 7 (detalle guardado, si esta CERRADA)
//   Modificar    @tipo 2 con @DetalleConciliacion (ProductoId, TotalSalida, TotalDevoluciones, TotalEntregado, Diferencia)
//  Igual que el escritorio no hay Nuevo ni Eliminar (las crea el Despacho) y no se modifica si
//  ya tiene Cierre de Caja. Mejora: la matriz se calcula en el servidor; del navegador solo se
//  aceptan las devoluciones reales, y Diferencia = devolucion real - devolucion calculada.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { fechaReq, rango, aVB, col, num, fechaIso } = require('./comun');

const router = express.Router();
router.use(requierePrograma('btnConciliaciones'));

const CAMPOS = ['Id', 'Fecha', 'Estado', 'CierreCaja', 'PersonalId', 'NombrePersonal', 'SucursalId', 'Observacion'];
const fila = (f) => ({ ...f, Fecha: fechaIso(f.Fecha), Estado: f.Estado === true || Number(f.Estado) === 1 ? 1 : 0, CierreCaja: num(f.CierreCaja) });

async function buscar(id, fecha) {
    const f = v.proyectar(await L.L_prListarGeneralFechas('MAM_Conciliacion', aVB(fecha), aVB(fecha)), CAMPOS).find((x) => Number(x.Id) === id);
    if (!f) throw new ErrorNegocio(`No se encontro la Conciliacion ${id}. Actualice la lista e intente de nuevo.`, 404);
    return fila(f);
}

/** _prCargarTablaDetalle de VB: matriz producto x salida + totales */
async function armarDetalle(id, abierta) {
    const [productos, salidas] = await Promise.all([L.ListarProductosSalidasConciliacion(id), L.ListarTodasSalidas(id)]);
    const ids = [...new Set(salidas.map((s) => num(col(s, 'DespachoProductosId'))))].sort((a, b) => a - b);
    const n = salidas.length ? Math.min(num(col(salidas[0], 'CantidadSalidas')) || ids.length, ids.length) : 0;
    const usadas = ids.slice(0, n);
    const fuente = abierta ? await L.ListarProductosEntregadoConciliaciones(id) : await L.ListarProductosDetalleConciliacion(id);
    const filas = productos.map((p) => {
        const pid = num(col(p, 'ProductoId'));
        const deEste = salidas.filter((s) => num(col(s, 'ProductoId')) === pid);
        const cant = usadas.map((sid) => { const r = deEste.filter((s) => num(col(s, 'DespachoProductosId')) === sid); return r.length === 1 ? num(col(r[0], 'Cantidad')) : null; });
        const TotalSalida = deEste.reduce((t, s) => t + num(col(s, 'Cantidad')), 0);
        const ent = fuente.filter((e) => num(col(e, 'ProductoId')) === pid);
        let r;
        if (!ent.length) r = { TotalEntregado: 0, TotalDevoluciones: TotalSalida, TotalDevolucionesCalculado: TotalSalida, Diferencia: 0 };
        else if (abierta) {
            const TotalEntregado = ent.reduce((t, e) => t + num(col(e, 'Cantidad')), 0);
            r = { TotalEntregado, TotalDevoluciones: TotalSalida - TotalEntregado, TotalDevolucionesCalculado: TotalSalida - TotalEntregado, Diferencia: 0 };
        } else {
            const e = ent[0];
            r = { TotalEntregado: num(col(e, 'TotalEntregado')), TotalDevoluciones: num(col(e, 'TotalDevoluciones')), TotalDevolucionesCalculado: num(col(e, 'TotalDevolucionesCalculado')), Diferencia: num(col(e, 'Diferencia')) };
        }
        return { ProductoId: pid, NombreProducto: col(p, 'NombreProducto') ?? '', salidas: cant, TotalSalida, ...r };
    });
    return { salidas: usadas, filas };
}

router.get('/', asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneralFechas('MAM_Conciliacion', d, h), CAMPOS).map(fila) });
}));

// Empresa y logo para el comprobante de conciliacion (mismos SP que la nota de venta, solo lectura)
router.get('/aux/cabecera', asyncHandler(async (req, res) => {
    const [empresas, imagen] = await Promise.all([L.L_prListarGeneral('MAM_Empresas').catch(() => []), L.ObtenerImagenEmpresa().catch(() => [])]);
    res.json({ ok: true, data: { empresa: v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Nit', 'Ciudad'])[0] || null, imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '' } });
}));
router.get('/:id/detalle', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const abierta = v.bit(req.query.abierta);
    res.json({ ok: true, data: await armarDetalle(id, abierta) });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const b = req.body || {};
    const actual = await buscar(id, fechaReq(b.fechaOriginal, 'Fecha original'));
    if (actual.CierreCaja > 0) throw new ErrorNegocio('No se puede Modificar la Conciliación por que ya tiene un Cierre de Caja Asociado'.toUpperCase());
    const Fecha = fechaReq(b.Fecha, 'Fecha Conciliación');
    const Estado = v.bit(b.Estado) ? 1 : 0;
    const Observacion = v.texto(b.Observacion, 500);
    const dev = b.devoluciones && typeof b.devoluciones === 'object' ? b.devoluciones : {};

    // la matriz se calcula con el estado GUARDADO (igual que VB al mostrar el registro)
    const { filas } = await armarDetalle(id, actual.Estado === 1);
    const detalle = filas.map((f) => {
        const txt = dev[f.ProductoId];
        let TotalDevoluciones = f.TotalDevoluciones;
        let Diferencia = f.Diferencia;
        if (txt !== undefined && txt !== null && String(txt).trim() !== '') {
            const n = Number(txt);
            if (!Number.isFinite(n) || n < 0) throw new ErrorNegocio(`Devolucion no valida para ${f.NombreProducto}`);
            TotalDevoluciones = n;
            Diferencia = Math.round((n - f.TotalDevolucionesCalculado) * 1e6) / 1e6;
        }
        return { ProductoId: f.ProductoId, TotalSalida: f.TotalSalida, TotalDevoluciones, TotalEntregado: f.TotalEntregado, Diferencia };
    });
    const ok = await L.ModificarConciliacion(String(id), aVB(Fecha), Estado, Observacion,
        { columnas: ['ProductoId', 'TotalSalida', 'TotalDevoluciones', 'TotalEntregado', 'Diferencia'], filas: detalle });
    if (!ok) throw new ErrorNegocio('Error al guardar la conciliación'.toUpperCase());
    res.json({ ok: true, id, mensaje: `CONCILIACIÓN # ${id} MODIFICADO CON EXITO.` });
}));

module.exports = router;
module.exports._interno = { armarDetalle };
