// =====================================================================
//  Tec_Empresas.vb  (programa: btConfEmpresa)
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const { subirImagen, GuardarImagen, IMAGEN_DEFECTO } = require('../../utils/imagenes');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btConfEmpresa'));

// _PMOGetTablaBuscador -> L_prListarGeneral("MAM_Empresas")
router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_Empresas'), ['Id', 'Nombre', 'Descripcion', 'Latitud', 'Longitud', 'Imagen', 'Propietario', 'Telefono', 'Nit', 'Ciudad', 'Direccion']) });
}));

function _leerEmpresa(body) {
    return {
        Nombre: v.requerido(body.Nombre, 'Nombre Empresa', 150),
        Descripcion: v.texto(body.Descripcion, 300),
        Latitud: v.decimal(body.Latitud),
        Longitud: v.decimal(body.Longitud),
        Propietario: v.texto(body.Propietario, 150),
        Telefono: v.texto(body.Telefono, 50),
        Nit: v.texto(body.Nit, 50),
        Ciudad: v.texto(body.Ciudad, 100),
        Direccion: v.texto(body.Direccion, 300),
        ImagenActual: v.texto(body.ImagenActual, 200) || IMAGEN_DEFECTO,
    };
}

router.post('/', subirImagen, asyncHandler(async (req, res) => {
    const e = _leerEmpresa(req.body);
    const Imagen = (await GuardarImagen('empresa', req.file)) || IMAGEN_DEFECTO;
    const r = await L.InsertarEmpresa({ _Id: '', ...e, Imagen });
    if (!r.resultado) throw new ErrorNegocio('Existe Problema al Registrar la Empresa'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `Empresa con Id = ${r._numi} fue Registrada Correctamente.`.toUpperCase() });
}));

router.put('/:id', subirImagen, asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const e = _leerEmpresa(req.body);
    const Imagen = (await GuardarImagen('empresa', req.file)) || e.ImagenActual;
    const r = await L.ModificarEmpresa({ _Id: id, ...e, Imagen });
    if (!r.resultado) throw new ErrorNegocio('Existe Problemas al modificar datos de la empresa'.toUpperCase());
    res.json({ ok: true, id, mensaje: `Empresa con Codigo = ${id} modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const ok = await L.L_prBorrarRegistro(id, 'MAM_Empresas');
    if (!ok) throw new ErrorNegocio('No se pudo eliminar la empresa (puede tener registros relacionados).');
    res.json({ ok: true, mensaje: `Empresa con Codigo ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
