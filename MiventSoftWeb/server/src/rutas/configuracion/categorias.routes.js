// =====================================================================
//  TecCategorias.vb  (programa: btnConfCategoria)
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const { subirImagen, GuardarImagen, IMAGEN_DEFECTO } = require('../../utils/imagenes');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btnConfCategoria'));

// _PMOGetTablaBuscador
router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prCategoriaGeneral(), ['Id', 'NombreCategoria', 'Estado', 'DescripcionCategoria', 'Imagen', 'EmpresaId', 'App', 'Empresa']) });
}));

// _prIniciarTodo -> _prCargarComboGenerico(cbEmpresa, L_prListaEmpresasUsuarios(), ...)
router.get('/combos', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: { empresas: v.proyectar(await L.L_prListaEmpresasUsuarios(), ['Id', 'Nombre']) } });
}));

function _leerCategoria(body) {
    return {
        NombreCategoria: v.requerido(body.NombreCategoria, 'Nombre Categoria', 100),
        DescripcionCategoria: v.texto(body.DescripcionCategoria, 200),
        Estado: v.bit(body.Estado),
        VisibleApp: v.bit(body.VisibleApp),
        EmpresaId: v.entero(body.EmpresaId, 'una Empresa', { requeridoPositivo: true }),
        ImagenActual: v.texto(body.ImagenActual, 200) || IMAGEN_DEFECTO,
    };
}

// _PMOGrabarRegistro
router.post('/', subirImagen, asyncHandler(async (req, res) => {
    const c = _leerCategoria(req.body);
    const nameImg = (await GuardarImagen('categoria', req.file)) || IMAGEN_DEFECTO;
    const r = await L.L_prCategoriaInsertar('', c.NombreCategoria, c.DescripcionCategoria, c.Estado, nameImg, c.VisibleApp, c.EmpresaId);
    if (!r.resultado) throw new ErrorNegocio('Existe Problema al Registrar la Categoria');
    res.json({ ok: true, id: r._numi, mensaje: `Codigo de Categoria ${r._numi} Grabado con Exito.`.toUpperCase() });
}));

// _PMOModificarRegistro  (si no se subio imagen nueva se conserva la actual)
router.put('/:id', subirImagen, asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const c = _leerCategoria(req.body);
    const nameImg = (await GuardarImagen('categoria', req.file)) || c.ImagenActual;
    const r = await L.L_prCategoriaModificar(id, c.NombreCategoria, c.DescripcionCategoria, c.Estado, nameImg, c.VisibleApp, c.EmpresaId);
    if (!r.resultado) throw new ErrorNegocio('Existe Problemas al modificar la Categoria');
    res.json({ ok: true, id, mensaje: `Codigo de Categoria ${id} modificado con Exito.`.toUpperCase() });
}));

// _PMOEliminarRegistro
router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const res_ = await L.L_prCategoriaBorrar(id);
    if (!res_) throw new ErrorNegocio('No se pudo eliminar la categoria (puede tener registros relacionados).');
    res.json({ ok: true, mensaje: `Codigo de Categoria ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
