// =====================================================================
//  F_AsignacionZona.vb (programa: btnAsignacionZona)  SP: sp_Mam_Zonas
//   Personal: L_prListarGeneral("MAM_Personal")   Zonas del personal: @tipo 5 (@idRepartidor)
//   Guardar: @tipo 6 con @Asignaciontype = la grilla COMPLETA (columna "asignada" marcada), como VB.
//   Mejora: el servidor relee la grilla y marca solo las zonas elegidas.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { col, num } = require('./comun');

const router = express.Router();
router.use(requierePrograma('btnAsignacionZona'));
const marcada = (x) => x === true || Number(x) === 1;

router.get('/personal', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_Personal'), ['Id', 'NombrePersonal', 'Tipo', 'Estado']) });
}));
router.get('/:personal', asyncHandler(async (req, res) => {
    const dt = await L.L_fnListarZonasAsignadas(v.idRuta(req.params.personal));
    res.json({ ok: true, data: dt.map((f) => ({ idZona: num(col(f, 'idZona')), NombreZona: col(f, 'NombreZona') ?? '', asignada: marcada(col(f, 'asignada')) })) });
}));
router.put('/:personal', asyncHandler(async (req, res) => {
    const personal = v.idRuta(req.params.personal);
    if (!(await L.L_prListarGeneral('MAM_Personal')).some((p) => num(col(p, 'Id')) === personal)) throw new ErrorNegocio('Seleccione un Personal');
    const sel = new Set((Array.isArray(req.body?.zonas) ? req.body.zonas : []).map(Number));
    const dt = await L.L_fnListarZonasAsignadas(personal);
    const columnas = L._columnasDe(dt);
    const kAsig = columnas.find((c) => c.toLowerCase() === 'asignada');
    const kZona = columnas.find((c) => c.toLowerCase() === 'idzona');
    if (!kAsig || !kZona) throw new ErrorNegocio('sp_Mam_Zonas @tipo 5 no devolvio las columnas idZona / asignada');
    const filas = dt.map((f) => ({ ...f, [kAsig]: sel.has(num(f[kZona])) }));
    if (!(await L.L_prRegistrarAsignacion({ columnas, filas }, personal))) throw new ErrorNegocio('Error al Guardar Asignacion'.toUpperCase());
    res.json({ ok: true, mensaje: 'Asignacion Registrado con Exito' });
}));

module.exports = router;
