// =====================================================================
//  Tec_CierreCaja.vb (programa: btnCierreCaja)  SP: MAM_CierreCaja
//   Lista L_prListarGeneralFechas (@tipo 3) · Gastos @tipo 4 · Conciliaciones cerradas sin cierre @tipo 5
//   Ventas de la conciliacion @tipo 6 · Cobranzas sin cierre @tipo 7 / con cierre @tipo 8 · Reporte @tipo 9
//   Grabar @tipo 1 / Modificar @tipo 2 (@DetalleGastos estados 0/1/-1/-2 + @Cobranzas) · Eliminar @tipo -1
//  Calculo (igual que VB): EfectivoCalculado = Contado + Cobranzas - Gastos ; Diferencia = Recibido - Calculado.
//  Al modificar se conservan ventas y cobranzas del cierre (VB no las recalcula); se recalculan gastos.
//  Mejora: todos los totales se calculan en el servidor con los mismos SP.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { libreria } = require('../configuracion/_libreria');
const { fechaReq, rango, aVB, col, num, fechaIso, limpiar } = require('./comun');

const router = express.Router();
router.use(requierePrograma('btnCierreCaja'));

const r2 = (x) => Math.round(x * 100) / 100;
const CAMPOS = ['Id', 'NroConciliacion', 'PersonalId', 'NombrePersonal', 'Detalle', 'FechaCierre', 'TotalGastos', 'VentasCreditos', 'VentasContado', 'TotalCobros', 'EfectivoCalculado', 'EfectivoRecibido', 'Diferencia'];
const fila = (f) => ({ ...f, FechaCierre: fechaIso(f.FechaCierre) });
const sinBin = (dt) => limpiar(dt).map((f) => { const o = { ...f }; delete o.img; delete o.Img; return o; });
const nuevaFila = (columnas, valores) => {
    const o = Object.fromEntries(columnas.map((c) => [c, null]));
    for (const [k, x] of Object.entries(valores)) { const c = columnas.find((y) => y.toLowerCase() === k.toLowerCase()); if (c) o[c] = x; }
    return o;
};
const sumaVentas = (dt) => ({ VentasContado: r2(dt.reduce((s, f) => s + num(col(f, 'Contado')), 0)), VentasCredito: r2(dt.reduce((s, f) => s + num(col(f, 'Credito')), 0)) });
const sumaCobros = (dt) => r2(dt.reduce((s, f) => s + num(col(f, 'Monto')), 0));

async function buscar(id, fecha) {
    const f = v.proyectar(await L.L_prListarGeneralFechas('MAM_CierreCaja', aVB(fecha), aVB(fecha)), CAMPOS).find((x) => Number(x.Id) === id);
    if (!f) throw new ErrorNegocio(`No se encontro el Cierre ${id}. Actualice la lista e intente de nuevo.`, 404);
    return fila(f);
}

/** Gastos: existentes (estado 1 / -1 si se quitaron) + nuevos (0). Concepto = libreria 14, monto > 0 */
async function armarGastos(cierreId, enviados) {
    const [existentes, conceptos] = await Promise.all([L.ListaDetalleGastosCierreId(cierreId || -1), libreria(14)]);
    const columnas = L._columnasDe(existentes);
    const quitar = new Set(enviados.filter((g) => g.Id && !g.nuevo && g.eliminado).map((g) => Number(g.Id)));
    const filas = (cierreId ? existentes : []).map((f) => nuevaFila(columnas, { ...f, estado: quitar.has(num(col(f, 'Id'))) ? -1 : 1 }));
    let id = filas.reduce((m, f) => Math.max(m, num(col(f, 'Id'))), 0);
    for (const g of enviados.filter((x) => x.nuevo && !x.eliminado)) {
        const c = conceptos.find((x) => Number(x.cnnum) === Number(g.ConceptoGastoId));
        if (!c) throw new ErrorNegocio('Debe Seleccionar un Concepto Valido'.toUpperCase());
        const monto = Number(g.Monto);
        if (!Number.isFinite(monto) || monto <= 0) throw new ErrorNegocio('Debe colocar Un Monto Valido'.toUpperCase());
        id += 1;
        filas.push(nuevaFila(columnas, { Id: id, CierreCajaId: 0, ConceptoGastoId: c.cnnum, Detalle: c.cndesc1, Descripcion: v.texto(g.Descripcion, 300), Monto: monto, estado: 0, img: null }));
    }
    const total = r2(filas.filter((f) => num(col(f, 'estado')) >= 0).reduce((s, f) => s + num(col(f, 'Monto')), 0));
    return { tabla: { columnas, filas }, total };
}
function recibido(x) {
    const n = Number(x ?? 0);
    if (!Number.isFinite(n) || n < 0) throw new ErrorNegocio('Ingrese un Efectivo Recibido valido');
    return r2(n);
}

// ---------------- lecturas ----------------
router.get('/', asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneralFechas('MAM_CierreCaja', d, h), CAMPOS).map(fila) });
}));
router.get('/conceptos', asyncHandler(async (req, res) => res.json({ ok: true, data: await libreria(14) })));
// btnSearchPersonal: conciliaciones cerradas pendientes de cierre
router.get('/conciliaciones', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: sinBin(await L.ListarConciliacionesCaja()).map((f) => ({ NroConciliacion: col(f, 'NroConciliacion'), Fecha: col(f, 'Fecha'), PersonalId: col(f, 'PersonalID'), NombrePersonal: col(f, 'NombrePersonal') })) });
}));
// Al elegir la conciliacion: ventas + cobranzas sin cierre (para mostrar y calcular)
router.get('/preparar', asyncHandler(async (req, res) => {
    const conc = v.entero(req.query.conciliacion, 'Conciliacion', { requeridoPositivo: true });
    const personal = v.entero(req.query.personal, 'Personal', { requeridoPositivo: true });
    const fecha = fechaReq(req.query.fecha, 'Fecha Cierre');
    const [ventas, cobros] = await Promise.all([L.ListaDetallePedidosconciliacion(conc), L.ListaCobranzasPorPersonalSinCierre(personal, aVB(fecha))]);
    res.json({ ok: true, data: { ventas: sinBin(ventas), cobranzas: sinBin(cobros), ...sumaVentas(ventas), TotalCobros: sumaCobros(cobros) } });
}));
router.get('/:id/detalle', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const conc = v.entero(req.query.conciliacion, 'Conciliacion');
    const [gastos, ventas, cobros] = await Promise.all([L.ListaDetalleGastosCierreId(id), L.ListaDetallePedidosconciliacion(conc || -1), L.ListaCobranzasPorPersonalconCierre(id)]);
    res.json({ ok: true, data: { gastos: sinBin(gastos), ventas: sinBin(ventas), cobranzas: sinBin(cobros) } });
}));
router.get('/:id/reporte', asyncHandler(async (req, res) => {
    const [dt, emp] = await Promise.all([L.DatosreporteCierreCaja(v.idRuta(req.params.id)), L.ObtenerImagenEmpresa()]);
    const e = emp[0] || {};
    res.json({ ok: true, data: { filas: sinBin(dt), empresa: { nombre: e.Nombre ?? '', direccion: e.Direccion ?? '' }, imagenEmpresa: emp.length ? String(Object.values(e)[0] ?? '') : '' } });
}));

// ---------------- grabar ----------------
router.post('/', asyncHandler(async (req, res) => {
    const b = req.body || {};
    const ConciliacionId = v.entero(b.ConciliacionId, 'Conciliacion', { requeridoPositivo: true });
    const conc = (await L.ListarConciliacionesCaja()).find((f) => num(col(f, 'NroConciliacion')) === ConciliacionId);
    if (!conc) throw new ErrorNegocio('Seleccione Una Conciliacion (cerrada y sin cierre de caja)');
    const PersonalId = num(col(conc, 'PersonalID'));
    const FechaCierre = fechaReq(b.FechaCierre, 'Fecha Cierre');
    const [ventas, cobros] = await Promise.all([L.ListaDetallePedidosconciliacion(ConciliacionId), L.ListaCobranzasPorPersonalSinCierre(PersonalId, aVB(FechaCierre))]);
    const g = await armarGastos(0, Array.isArray(b.gastos) ? b.gastos : []);
    const sv = sumaVentas(ventas);
    const TotalCobros = sumaCobros(cobros);
    const EfectivoCalculado = r2(sv.VentasContado + TotalCobros - g.total);
    const EfectivoRecibido = recibido(b.EfectivoRecibido);
    const r = await L.InsertarCierreCaja(
        { _Id: '', PersonalId, ConciliacionId, FechaCierre: aVB(FechaCierre), TotalGasto: g.total, ...sv, TotalCobros, EfectivoCalculado, EfectivoRecibido, Diferencia: r2(EfectivoRecibido - EfectivoCalculado) },
        g.tabla, { columnas: L._columnasDe(cobros), filas: cobros },
    );
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Cierre De Caja'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `CODIGO DE CIERRE DE CAJA ${r._numi} GRABADO CON EXITO.` });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const b = req.body || {};
    const actual = await buscar(id, fechaReq(b.fechaOriginal, 'Fecha original'));
    const FechaCierre = fechaReq(b.FechaCierre, 'Fecha Cierre');
    const cobros = await L.ListaCobranzasPorPersonalconCierre(id);
    const g = await armarGastos(id, Array.isArray(b.gastos) ? b.gastos : []);
    const VentasContado = num(actual.VentasContado);
    const TotalCobros = num(actual.TotalCobros);
    const EfectivoCalculado = r2(VentasContado + TotalCobros - g.total);
    const EfectivoRecibido = recibido(b.EfectivoRecibido);
    const ok = await L.ModificarCierreCaja(
        { _Id: String(id), PersonalId: num(actual.PersonalId), ConciliacionId: num(actual.NroConciliacion), FechaCierre: aVB(FechaCierre), TotalGasto: g.total, VentasCredito: num(actual.VentasCreditos), VentasContado, TotalCobros, EfectivoCalculado, EfectivoRecibido, Diferencia: r2(EfectivoRecibido - EfectivoCalculado) },
        g.tabla, { columnas: L._columnasDe(cobros), filas: cobros },
    );
    if (!ok) throw new ErrorNegocio('Error al guardar el Cierre De Caja'.toUpperCase());
    res.json({ ok: true, id, mensaje: `CODIGO DE CIERRE DE CAJA ${id} MODIFICADO CON EXITO.` });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    await buscar(id, fechaReq(req.query.fecha, 'Fecha'));
    if (!(await L.L_prBorrarRegistro(id, 'MAM_CierreCaja'))) throw new ErrorNegocio('Error al eliminar el Cierre De Caja'.toUpperCase());
    res.json({ ok: true, mensaje: `CODIGO DE CIERRE DE CAJA ${id} ELIMINADO CON EXITO.` });
}));

module.exports = router;
