// =====================================================================
//  Tec_Personal.vb  (programa: btnPersonal)
//  Buscador: L_prListarGeneral("MAM_Personal")  ·  Grabar: InsertarPersonal / ModificarPersonal
//  Combos: cbTipoDocumento = libreria 8, cbTipoPersonal = libreria 9, cbEmpresa = L_prListaEmpresasUsuarios
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const { rutaNuevaLibreria, libreria } = require('../configuracion/_libreria');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btnPersonal'));

// _PMOGetListEstructuraBuscador (solo las columnas que usa la pantalla)
const CAMPOS = ['Id', 'NombrePersonal', 'Direccion', 'Telefono01', 'TipoDocumento', 'NroDocumento', 'TipoPersonal', 'Tipo', 'Estado', 'EmpresaId', 'FechaNacimiento'];

router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_Personal'), CAMPOS) });
}));

router.get('/combos', asyncHandler(async (req, res) => {
    const [tiposDocumento, tiposPersonal, empresas] = await Promise.all([libreria(8), libreria(9), L.L_prListaEmpresasUsuarios()]);
    res.json({ ok: true, data: { tiposDocumento, tiposPersonal, empresas: v.proyectar(empresas, ['Id', 'Nombre']) } });
}));

// btnTipoDocumento -> Efecto tipo 10, ModuloLibreria = 8
router.post('/libreria/:idClasificador', rutaNuevaLibreria([8]));

// _PMOValidarCampos: Nombre, Empresa y Tipo Documento requeridos
function _leer(body) {
    const fecha = v.texto(body.FechaNacimiento, 10);
    if (fecha && !/^\d{4}-\d{2}-\d{2}$/.test(fecha)) throw new ErrorNegocio('Fecha de nacimiento no valida');
    return {
        NombrePersonal: v.requerido(body.NombrePersonal, 'Nombre Personal', 150).toUpperCase(),
        Direccion: v.texto(body.Direccion, 250),
        Telefono01: v.texto(body.Telefono01, 50),
        TipoDocumento: v.entero(body.TipoDocumento, 'un Tipo Documento', { requeridoPositivo: true }),
        NroDocumento: v.texto(body.NroDocumento, 50),
        TipoPersonal: v.entero(body.TipoPersonal, 'un Tipo Personal'),
        Estado: v.bit(body.Estado),
        EmpresaId: v.entero(body.EmpresaId, 'una Empresa', { requeridoPositivo: true }),
        FechaNacimiento: v.fechaVB(fecha || null), // tbFechaNacimiento.Value.ToString("yyyy/MM/dd")
    };
}

router.post('/', asyncHandler(async (req, res) => {
    const r = await L.InsertarPersonal({ _Id: '', ..._leer(req.body) });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Personal'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `Codigo de Personal ${r._numi} Grabado con Exito.`.toUpperCase() });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const r = await L.ModificarPersonal({ _Id: id, ..._leer(req.body) });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar el Personal'.toUpperCase());
    res.json({ ok: true, id, mensaje: `Codigo de Personal ${id} modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const ok = await L.L_prBorrarRegistro(id, 'MAM_Personal');
    if (!ok) throw new ErrorNegocio('No se pudo eliminar el Personal (puede tener ventas o usuarios relacionados).');
    res.json({ ok: true, mensaje: `Codigo de Personal ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
