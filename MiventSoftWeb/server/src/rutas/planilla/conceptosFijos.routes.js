// =====================================================================
//  Tec_ConceptosFijos.vb (programa: btnConceptoFijos)  SP: MAM_ConceptosFijos 1 / 2 / -1, operaciones @tipo 4
//  Reglas VB: nombre y operacion requeridos; si el concepto es FIJO el porcentaje debe ser > 0.
//  swTipoConcepto: Fijo (1) / Variable (0)   swEstado: ACTIVO (1) / INACTIVO (0)
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btnConceptoFijos'));
const CAMPOS = ['Id', 'NombreConcepto', 'Porcentaje', 'OperacionId', 'Tipo', 'NombreOperacion', 'TipoConcepto', 'Estado'];
const operaciones = async () => v.proyectar(await L.L_prListarOperacionesConceptosFijos(), ['Id', 'NombreOperacion']);

async function leer(b) {
    const NombreConcepto = v.texto(b.NombreConcepto, 150).toUpperCase();
    const faltan = [];
    if (!NombreConcepto) faltan.push('Nombre Concepto');
    const OperacionId = Number(b.OperacionId);
    if (!(await operaciones()).some((o) => Number(o.Id) === OperacionId)) faltan.push('Operación');
    const TipoConcepto = v.bit(b.TipoConcepto) ? 1 : 0;
    const Porcentaje = Math.round(v.decimal(b.Porcentaje) * 100) / 100;
    if ((TipoConcepto && !(Porcentaje > 0)) || Porcentaje < 0 || Porcentaje > 100) faltan.push('Porcentaje');
    if (faltan.length) throw new ErrorNegocio(`Los Siguientes Campos Son Requeridos: ${faltan.join(', ')}`);
    return { NombreConcepto, OperacionId, TipoConcepto, Porcentaje, Estado: v.bit(b.Estado) ? 1 : 0 };
}
router.get('/', asyncHandler(async (req, res) => res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_ConceptosFijos'), CAMPOS) })));
router.get('/operaciones', asyncHandler(async (req, res) => res.json({ ok: true, data: await operaciones() })));
router.post('/', asyncHandler(async (req, res) => {
    const r = await L.L_prConceptoFijoInsertar({ ...(await leer(req.body || {})), _numi: '' });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Concepto Fijo'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `CONCEPTO FIJO ${r._numi} GRABADO CON EXITO.` });
}));
router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await L.L_prConceptoFijoModificar({ ...(await leer(req.body || {})), _numi: String(id) }))) throw new ErrorNegocio('Error al modificar el Concepto Fijo'.toUpperCase());
    res.json({ ok: true, id, mensaje: `CODIGO DE CONCEPTO FIJO ${id} MODIFICADO CON EXITO.` });
}));
router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await L.L_prBorrarRegistro(id, 'MAM_ConceptosFijos'))) throw new ErrorNegocio('No se pudo eliminar el Concepto (puede estar usado en contratos o planillas)');
    res.json({ ok: true, mensaje: `CODIGO DE CONCEPTO FIJO ${id} ELIMINADO CON EXITO.` });
}));
module.exports = router;
