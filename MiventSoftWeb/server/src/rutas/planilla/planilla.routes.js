// =====================================================================
//  Planilla de sueldos  SP: MAM_Planilla
//   /registrar  Tec_RegistrarPersonalAPlanilla (btnRegistrarPlanilla)  sin planilla @tipo 5 -> por contrato @tipo 1
//               (conceptos fijos del contrato: Monto = Sueldo x %; SueldoNeto = SueldoBruto, igual que VB)
//   /listar     Tec_Listarplanilla (btnPlanilla)  @tipo 3 · reporte @tipo 9 · boleta @tipo 8
//               conceptos fijos (Tec_VisualizarConceptoFijosPlanilla, @tipo 4 TipoConcepto 1)
//               conceptos variables (Tec_VisualizarConceptosVariablesPlanilla, @tipo 4 TipoConcepto 0, lista @tipo 6, grabar @tipo 7)
//  Año = libreria 13 y Mes = libreria 12: igual que VB se envian los codigos (cnnum) de la libreria.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma, accionPermiso, ProgramasPermitidos } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { col, num, limpiar } = require('../distribucion/comun');
const { rutaNuevaLibreria, libreria } = require('../configuracion/_libreria');

const router = express.Router();
const REG = requierePrograma('btnRegistrarPlanilla');
const LIS = requierePrograma('btnPlanilla');
const r2 = (x) => Math.round(x * 100) / 100;
const nuevaFila = (columnas, valores) => {
    const o = Object.fromEntries(columnas.map((c) => [c, null]));
    for (const [k, x] of Object.entries(valores)) { const c = columnas.find((y) => y.toLowerCase() === k.toLowerCase()); if (c) o[c] = x; }
    return o;
};
const hoyUTC = () => { const d = new Date(Date.now() - 4 * 3600e3); return new Date(Date.UTC(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate())); };
async function periodo(q) {
    const Mes = Number.parseInt(q.mes, 10);
    const Anio = Number.parseInt(q.anio, 10);
    if (!(await libreria(12)).some((x) => Number(x.cnnum) === Mes)) throw new ErrorNegocio('Seleccione un Mes');
    if (!(await libreria(13)).some((x) => Number(x.cnnum) === Anio)) throw new ErrorNegocio('Seleccione un Año');
    return { Mes, Anio };
}
const sinBinarios = (dt) => limpiar(dt).map((f) => { const o = { ...f }; ['ConceptoFijos', 'ConceptoVariable', 'Reporte', 'eliminar', 'img'].forEach((k) => delete o[k]); return o; });
const conceptos = async (id, tipo) => (await L.ListaConceptosPlanilla(id, tipo)).map((f) => ({ Id: num(col(f, 'Id')), ConceptoId: num(col(f, 'ConceptoId')), NombreConcepto: col(f, 'NombreConcepto') ?? '',
    Porcentaje: num(col(f, 'Porcentaje')), Monto: num(col(f, 'Monto')), Descripcion: col(f, 'Descripcion') ?? '' }));

// combos: año / mes (con cualquiera de los dos programas)
router.get('/combos', asyncHandler(async (req, res) => {
    const p = await ProgramasPermitidos(req.sesion.gi_userRol);
    if (!p.has('BTNREGISTRARPLANILLA') && !p.has('BTNPLANILLA')) return res.status(403).json({ ok: false, mensaje: 'No tiene privilegios para este programa.' });
    res.json({ ok: true, data: { anios: await libreria(13), meses: await libreria(12) } });
}));

// ---------------- Registrar personal a planilla ----------------
router.post('/registrar/libreria/:idClasificador', REG, rutaNuevaLibreria([13]));
router.get('/registrar/sin-planilla', REG, asyncHandler(async (req, res) => {
    const { Mes, Anio } = await periodo(req.query);
    res.json({ ok: true, data: sinBinarios(await L.ListarPersonalSinPlanilla(Mes, Anio)) });
}));
router.post('/registrar', REG, asyncHandler(async (req, res) => {
    const { Mes, Anio } = await periodo(req.body || {});
    const pendientes = await L.ListarPersonalSinPlanilla(Mes, Anio);
    if (!pendientes.length) throw new ErrorNegocio('No hay personal pendiente para este periodo');
    const columnas = L._columnasDe(await L.ListaConceptosPlanilla(-1, -1));
    let ok = 0; const errores = [];
    for (const p of pendientes) {
        const ContratoId = num(col(p, 'ContratoId'));
        const Sueldo = num(col(p, 'SueldoBase'));
        const SueldoBruto = num(col(p, 'SueldoBruto'));
        const filas = (await L.ListaConceptosContratos(ContratoId)).filter((c) => num(col(c, 'estado')) >= 0).map((c, j) => {
            const Porcentaje = num(col(c, 'PorcentajeConcepto'));
            return nuevaFila(columnas, { Id: j, PlanillaId: j, ConceptoId: num(col(c, 'ConceptoId')), NombreConcepto: col(c, 'NombreConcepto'), Porcentaje,
                Monto: r2(Sueldo * (Porcentaje / 100)), TipoConcepto: 1, Descripcion: '', Fecha: hoyUTC(), estado: 0 });
        });
        const r = await L.InsertarPlanilla({ _numi: '', ContratoId, Anio, Mes, Sueldo, SueldoBruto, SueldoNeto: SueldoBruto }, { columnas, filas });
        if (r.resultado) ok += 1; else errores.push(String(col(p, 'Personal') ?? ContratoId));
    }
    if (!ok) throw new ErrorNegocio('No se pudo registrar la planilla');
    res.json({ ok: true, registrados: ok, mensaje: `${ok} TRABAJADOR(ES) REGISTRADO(S) EN PLANILLA.${errores.length ? ` Con error: ${errores.join(', ')}` : ''}` });
}));

// ---------------- Listar planilla ----------------
router.get('/listar', LIS, asyncHandler(async (req, res) => {
    const todos = req.query.todos === '1' ? 1 : 0;
    const { Mes, Anio } = todos ? { Mes: Number(req.query.mes) || 0, Anio: Number(req.query.anio) || 0 } : await periodo(req.query);
    res.json({ ok: true, data: sinBinarios(await L.ListarPlanilla(Mes, Anio, todos)) });
}));
router.get('/listar/reporte', LIS, asyncHandler(async (req, res) => {
    const todos = req.query.todos === '1' ? 1 : 0;
    const { Mes, Anio } = todos ? { Mes: Number(req.query.mes) || 0, Anio: Number(req.query.anio) || 0 } : await periodo(req.query);
    res.json({ ok: true, data: sinBinarios(await L.ListarPlanillaReporte(Mes, Anio, todos)) });
}));
router.get('/listar/variables', LIS, asyncHandler(async (req, res) => {
    res.json({ ok: true, data: (await L.ListarConceptoVariablePlanilla()).map((f) => ({ Id: num(col(f, 'id')), NombreConcepto: col(f, 'NombreConcepto') ?? '', Operacion: num(col(f, 'Operacion')) })) });
}));
router.get('/listar/:id/boleta', LIS, asyncHandler(async (req, res) => res.json({ ok: true, data: sinBinarios(await L.ReporteBoletaSalarioIndividual(v.idRuta(req.params.id))) })));
router.get('/listar/:id/conceptos', LIS, asyncHandler(async (req, res) => {
    res.json({ ok: true, data: await conceptos(v.idRuta(req.params.id), req.query.tipo === '0' ? 0 : 1) });
}));

// Conceptos variables: agregar / quitar -> ModificarPlanillaSoloDetalle (@tipo 7) con toda la grilla (estado 0 nuevo, -1 quitado)
async function grabarVariables(id, cambiar) {
    const actuales = await L.ListaConceptosPlanilla(id, 0);
    const columnas = L._columnasDe(actuales);
    const filas = cambiar(actuales.map((f) => nuevaFila(columnas, f)), columnas);
    if (!(await L.ModificarPlanillaSoloDetalle(id, { columnas, filas }))) throw new ErrorNegocio('No se pudo guardar el concepto variable');
}
router.post('/listar/:id/variables', accionPermiso('modificar'), LIS, asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const b = req.body || {};
    const conceptosVar = (await L.ListarConceptoVariablePlanilla()).map((f) => ({ Id: num(col(f, 'id')), NombreConcepto: col(f, 'NombreConcepto') ?? '', Operacion: num(col(f, 'Operacion')) }));
    const c = conceptosVar.find((x) => x.Id === Number(b.ConceptoId));
    if (!c) throw new ErrorNegocio('Seleccione un Concepto Variable');
    const monto = r2(v.decimal(b.Monto));
    if (!(monto > 0)) throw new ErrorNegocio('El Monto debe ser Mayor a 0');
    const pl = (await L.ListarPlanilla(0, 0, 1)).find((x) => num(col(x, 'PlanillaId')) === id);
    if (!pl) throw new ErrorNegocio('No se encontro la planilla. Actualice la lista.', 404);
    if (c.Operacion === -1 && monto > num(col(pl, 'SueldoNeto'))) throw new ErrorNegocio(`El monto no puede ser mayor al Sueldo Neto (${num(col(pl, 'SueldoNeto')).toFixed(2)})`);
    await grabarVariables(id, (filas, columnas) => [...filas, nuevaFila(columnas, { Id: filas.reduce((m, f) => Math.max(m, num(col(f, 'Id'))), 0) + 1, PlanillaId: id, ConceptoId: c.Id,
        NombreConcepto: c.NombreConcepto, Porcentaje: 0, Monto: r2(monto * c.Operacion), TipoConcepto: 0, Descripcion: v.texto(b.Descripcion, 200), Fecha: hoyUTC(), estado: 0 })]);
    res.json({ ok: true, mensaje: 'CONCEPTO VARIABLE REGISTRADO CON EXITO.' });
}));
router.delete('/listar/:id/variables/:det', accionPermiso('modificar'), LIS, asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const det = v.idRuta(req.params.det);
    let hay = false;
    await grabarVariables(id, (filas) => filas.map((f) => { if (num(col(f, 'Id')) !== det) return f; hay = true; const k = Object.keys(f).find((x) => x.toLowerCase() === 'estado'); return { ...f, [k]: -1 }; }));
    if (!hay) throw new ErrorNegocio('El concepto ya no existe. Actualice la lista.');
    res.json({ ok: true, mensaje: 'CONCEPTO VARIABLE ELIMINADO CON EXITO.' });
}));
module.exports = router;
