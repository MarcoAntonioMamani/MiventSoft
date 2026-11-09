// =====================================================================
//  Frm_Proveedor.vb  (programa: btnProveedores)
//  Buscador: L_prListarGeneral("MAM_Proveedores")  ·  Grabar: InsertarProveedor / ModificarProveedor
//  Mejora: VB enviaba Descripcion = "" siempre (al modificar borraba la descripcion
//  guardada). Aqui se edita y se conserva.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const { rutaNuevaLibreria, libreria } = require('../configuracion/_libreria');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btnProveedores'));

const CAMPOS = ['Id', 'NombreProveedor', 'Direccion', 'Telefono01', 'Telefono02', 'Descripcion', 'TipoDocumento', 'NroDocumento', 'Estado'];

router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_Proveedores'), CAMPOS) });
}));

router.get('/combos', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: { tiposDocumento: await libreria(8) } });
}));

router.post('/libreria/:idClasificador', rutaNuevaLibreria([8]));

function _leer(body) {
    return {
        NombreProveedor: v.requerido(body.NombreProveedor, 'Nombre Proveedor', 150).toUpperCase(),
        Direccion: v.texto(body.Direccion, 250),
        Telefono01: v.texto(body.Telefono01, 50),
        Telefono02: v.texto(body.Telefono02, 50),
        Descripcion: v.texto(body.Descripcion, 300),
        TipoDocumento: v.entero(body.TipoDocumento, 'un Tipo Documento', { requeridoPositivo: true }),
        NroDocumento: v.texto(body.NroDocumento, 50),
        Estado: v.bit(body.Estado),
    };
}

router.post('/', asyncHandler(async (req, res) => {
    const r = await L.InsertarProveedor({ _Id: '', ..._leer(req.body) });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Proveedor'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `Codigo de Proveedor ${r._numi} Grabado con Exito.`.toUpperCase() });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const r = await L.ModificarProveedor({ _Id: id, ..._leer(req.body) });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Proveedor'.toUpperCase());
    res.json({ ok: true, id, mensaje: `Codigo de Proveedor ${id} modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const ok = await L.L_prBorrarRegistro(id, 'MAM_Proveedores');
    if (!ok) throw new ErrorNegocio('No se pudo eliminar el Proveedor (puede tener compras o productos relacionados).');
    res.json({ ok: true, mensaje: `Codigo de Proveedor ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
