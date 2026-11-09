// =====================================================================
//  Tec_Contratos.vb (programa: btnContratos)  SP: MAM_Contratos 1 / 2 / -1, conceptos del contrato @tipo 4, conceptos fijos @tipo 5
//   Tipo de contrato = libreria 10, Cargo = libreria 11 (se pueden crear, Efecto tipo 10) · Personal = ListarPersonal
//  Reglas VB: personal, tipo y cargo requeridos; si no es indefinido la fecha fin debe ser mayor al inicio;
//   al grabar uno nuevo indefinido la fecha fin es hoy + 50 años; no se registra si el personal ya tiene contrato vigente.
//  @DetalleContratoConcepto: Id, ContratoId, ConceptoId, NombreConcepto, PorcentajeConcepto, estado (0 nuevo / -1 quitado).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { fechaReq, aVB, col, num, fechaIso } = require('../distribucion/comun');
const { rutaNuevaLibreria, libreria } = require('../configuracion/_libreria');

const router = express.Router();
router.use(requierePrograma('btnContratos'));
const CAMPOS = ['Id', 'PersonalId', 'NombrePersonal', 'TipoContratoID', 'TipoContratoDescripcion', 'Cargo', 'DescripcionCargo', 'Antigüedad', 'SueldoBase', 'InicioContrato', 'FinContrato', 'FechaFin', 'Estado', 'Indefinido'];
const COLS_TVP = ['Id', 'ContratoId', 'ConceptoId', 'NombreConcepto', 'PorcentajeConcepto', 'estado'];
const lista = async () => v.proyectar(await L.L_prListarGeneral('MAM_Contratos'), CAMPOS).map((f) => ({ ...f, Antiguedad: f['Antigüedad'], InicioContrato: fechaIso(f.InicioContrato),
    FinContrato: fechaIso(f.FinContrato), FechaFin: f.FechaFin instanceof Date ? fechaIso(f.FechaFin) : f.FechaFin, Indefinido: v.bit(f.Indefinido) ? 1 : 0, SueldoBase: num(f.SueldoBase) }));
const conceptosFijos = async () => (await L.ListaConceptosFijosContrato()).map((f) => ({ Id: num(col(f, 'id')), NombreConcepto: col(f, 'NombreConcepto') ?? '', Porcentaje: num(col(f, 'Porcentaje')) }));
const conceptosDe = async (id) => (await L.ListaConceptosContratos(id)).map((f) => ({ Id: num(col(f, 'Id')), ContratoId: num(col(f, 'ContratoId')), ConceptoId: num(col(f, 'ConceptoId')),
    NombreConcepto: col(f, 'NombreConcepto') ?? '', PorcentajeConcepto: num(col(f, 'PorcentajeConcepto')), estado: num(col(f, 'estado')) }));
const hoyMas50 = () => { const d = new Date(Date.now() - 4 * 3600e3); return `${d.getUTCFullYear() + 50}-${String(d.getUTCMonth() + 1).padStart(2, '0')}-${String(d.getUTCDate()).padStart(2, '0')}`; };

async function leer(b, nuevo) {
    const faltan = [];
    const PersonalId = Number(b.PersonalId);
    if (!v.proyectar(await L.ListarPersonal(), ['Id']).some((p) => Number(p.Id) === PersonalId)) faltan.push('Personal');
    const TipoContratoId = Number(b.TipoContratoId);
    if (!(await libreria(10)).some((x) => Number(x.cnnum) === TipoContratoId)) faltan.push('Tipo Contrato');
    const Cargo = Number(b.Cargo);
    if (!(await libreria(11)).some((x) => Number(x.cnnum) === Cargo)) faltan.push('Cargo');
    if (faltan.length) throw new ErrorNegocio(`Los Siguientes Campos Son Requeridos: ${faltan.join(', ')}`);
    const Indefinido = v.bit(b.Indefinido) ? 1 : 0;
    const inicio = fechaReq(b.InicioContrato, 'Inicio Contrato');
    let fin = Indefinido && nuevo ? hoyMas50() : fechaReq(b.FinContrato, 'Fin Contrato');
    if (Indefinido && fin <= inicio) fin = hoyMas50();
    if (!Indefinido && inicio >= fin) throw new ErrorNegocio('Fecha Finalizacion Debe Ser Mayor a la Fecha de Inicio de Vigencia');
    const SueldoBase = Math.round(v.decimal(b.SueldoBase) * 100) / 100;
    if (SueldoBase < 0) throw new ErrorNegocio('El Sueldo no puede ser negativo');
    return { PersonalId, TipoContratoId, Cargo, Indefinido, SueldoBase, InicioContrato: aVB(inicio), FinContrato: aVB(fin), _inicio: inicio };
}
function filasConceptos(existentes, enviados, fijos) {
    const quitar = new Set(enviados.filter((d) => !d.nuevo && d.eliminado).map((d) => Number(d.Id)));
    const filas = existentes.map((f) => ({ ...f, estado: quitar.has(f.Id) ? -1 : f.estado }));
    const vigentes = new Set(filas.filter((f) => f.estado >= 0).map((f) => f.ConceptoId));
    let id = filas.reduce((m, f) => Math.max(m, f.Id), 0);
    for (const d of enviados.filter((x) => x.nuevo && !x.eliminado)) {
        const c = fijos.find((x) => x.Id === Number(d.ConceptoId));
        if (!c) throw new ErrorNegocio('Concepto fijo no valido');
        if (vigentes.has(c.Id)) throw new ErrorNegocio(`El concepto ${c.NombreConcepto} ya esta en el contrato`);
        vigentes.add(c.Id);
        filas.push({ Id: ++id, ContratoId: 0, ConceptoId: c.Id, NombreConcepto: c.NombreConcepto, PorcentajeConcepto: c.Porcentaje, estado: 0 });
    }
    return { columnas: COLS_TVP, filas: filas.map((f) => Object.fromEntries(COLS_TVP.map((k) => [k, f[k]]))) };
}

router.get('/', asyncHandler(async (req, res) => res.json({ ok: true, data: await lista() })));
router.get('/combos', asyncHandler(async (req, res) => res.json({ ok: true, data: {
    tipos: await libreria(10), cargos: await libreria(11), personal: v.proyectar(await L.ListarPersonal(), ['Id', 'Nombre', 'Direccion', 'Telefono01']), conceptos: await conceptosFijos() } })));
router.get('/:id/conceptos', asyncHandler(async (req, res) => res.json({ ok: true, data: await conceptosDe(v.idRuta(req.params.id)) })));
router.post('/libreria/:idClasificador', rutaNuevaLibreria([10, 11]));

router.post('/', asyncHandler(async (req, res) => {
    const b = req.body || {};
    const c = await leer(b, true);
    // VerificarExisteContratoVigente: mismo personal con contrato cuyo fin es >= al inicio del nuevo
    if ((await lista()).some((x) => Number(x.PersonalId) === c.PersonalId && x.FinContrato && c._inicio <= x.FinContrato)) {
        throw new ErrorNegocio(`El Personal Ya tiene un Contrato Vigente`);
    }
    const dt = filasConceptos([], Array.isArray(b.conceptos) ? b.conceptos : [], await conceptosFijos());
    const r = await L.InsertarContratos({ ...c, _numi: '' }, dt);
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Contrato'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `CODIGO DE CONTRATO ${r._numi} GRABADO CON EXITO.` });
}));
router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await lista()).some((x) => Number(x.Id) === id)) throw new ErrorNegocio('No se encontro el contrato. Actualice la lista.', 404);
    const b = req.body || {};
    const c = await leer(b, false);
    const dt = filasConceptos(await conceptosDe(id), Array.isArray(b.conceptos) ? b.conceptos : [], await conceptosFijos());
    if (!(await L.ModificarContratos({ ...c, _numi: String(id) }, dt))) throw new ErrorNegocio('Error al guardar el Contrato'.toUpperCase());
    res.json({ ok: true, id, mensaje: `CODIGO DE CONTRATO ${id} MODIFICADO CON EXITO.` });
}));
router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await L.L_prBorrarRegistro(id, 'MAM_Contratos'))) throw new ErrorNegocio('Error al eliminar el Contrato (puede tener planillas registradas)'.toUpperCase());
    res.json({ ok: true, mensaje: `CODIGO DE CONTRATO ${id} ELIMINADO CON EXITO.` });
}));
module.exports = router;
