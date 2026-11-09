// =====================================================================
//  Tec_Roles.vb  (programa: btConfRoles)
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma, LimpiarCachePermisos } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btConfRoles'));

router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prRolGeneral(), ['Id', 'NombreRol']) });
}));

// _prCargarGridModulos -> L_prLibreriaDetalleGeneral(1)  (cnnum, cndesc1)
router.get('/modulos', asyncHandler(async (req, res) => {
    const dt = await L.L_prLibreriaDetalleGeneral(1);
    dt.sort((a, b) => Number(a.cnnum) - Number(b.cnnum));
    res.json({ ok: true, data: v.proyectar(dt, ['cnnum', 'cndesc1']) });
}));

// _prCargarGridDetalle(numi)  -> para "Nuevo" se pide con -1
router.get('/:id/detalle', asyncHandler(async (req, res) => {
    const id = Number.parseInt(req.params.id, 10);
    if (Number.isNaN(id)) throw new ErrorNegocio('Codigo de rol no valido');
    res.json({ ok: true, data: v.proyectar(await L.L_prRolDetalleGeneral(id), L.COLUMNAS_ROL_DETALLE) });
}));

function _leerRol(body) {
    const NombreRol = v.requerido(body.NombreRol, 'INGRESE LA DESCRIPCION DEL ROL!', 30);
    const detalle = Array.isArray(body.detalle) ? body.detalle : [];
    // Solo se aceptan las columnas del tipo tabla, con valores normalizados
    const filas = detalle.map((f) => ({
        Id: Number.parseInt(f.Id, 10) || 0,
        RolId: Number.parseInt(f.RolId, 10) || 0,
        ProgramaId: Number.parseInt(f.ProgramaId, 10) || 0,
        DescripcionPrograma: v.texto(f.DescripcionPrograma, 200),
        Modulo: Number.parseInt(f.Modulo, 10) || 0,
        Ver: v.bit(f.Ver),
        Insertar: v.bit(f.Insertar),
        Modificar: v.bit(f.Modificar),
        Eliminar: v.bit(f.Eliminar),
        estado: Number.parseInt(f.estado, 10) || 0,
    }));
    return { NombreRol, filas };
}

router.post('/', asyncHandler(async (req, res) => {
    const { NombreRol, filas } = _leerRol(req.body);
    const r = await L.L_prRolGrabar('', NombreRol, filas);
    if (!r.resultado) throw new ErrorNegocio('No se pudo grabar el rol');
    LimpiarCachePermisos();
    res.json({ ok: true, id: r._numi, mensaje: `Codigo de Rol ${r._numi} Grabado con Exito.`.toUpperCase() });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const { NombreRol, filas } = _leerRol(req.body);
    const r = await L.L_prRolModificar(id, NombreRol, filas);
    if (!r.resultado) throw new ErrorNegocio('No se pudo modificar el rol');
    LimpiarCachePermisos();
    res.json({ ok: true, id, mensaje: `Codigo de Rol ${id} modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (id === Number(req.sesion.gi_userRol)) throw new ErrorNegocio('No puede eliminar el rol con el que inicio sesion.');
    const ok = await L.L_prRolBorrar(id);
    if (!ok) throw new ErrorNegocio('No se pudo eliminar el rol (puede tener usuarios asignados).');
    LimpiarCachePermisos();
    res.json({ ok: true, mensaje: `Codigo de Rol ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
