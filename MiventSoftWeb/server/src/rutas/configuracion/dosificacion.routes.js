// =====================================================================
//  KAILY LA PAZ: Tec_Dosificacion.vb (programa: btConfDosificacion)   SP: MAM_Dosificacion
//   * Lista: L_prListarGeneral("MAM_Dosificacion") (@tipo 3)
//   * Sucursal: L_fnGeneralSucursales (sp_Mam_Precios @tipo 4)
//   * Grabar / Modificar: @tipo 1 / 2 · Eliminar: L_prBorrarRegistro (@tipo -1)
//   * Fechas en "yyyy/MM/dd"; Estado y TipoComputarizado con IIf(sw.Value, 1, 0)
//  La Llave no se escribe en logs (es el dato sensible de la dosificacion).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { fechaReq, aVB, fechaIso } = require('../distribucion/comun');

const router = express.Router();
router.use(requierePrograma('btConfDosificacion'));

const CAMPOS = ['Id', 'SucursalId', 'FechaInicio', 'FechaLimite', 'NombreAlmacen', 'UltimoNroFactura', 'NumeroAutorizacion', 'Llave', 'Nota01', 'Nota02', 'Estado', 'TipoComputarizado'];
const bool = (x) => x === true || Number(x) === 1;

// _PMOGetTablaBuscador
router.get('/', asyncHandler(async (req, res) => {
    const dt = v.proyectar(await L.L_prListarGeneral('MAM_Dosificacion'), CAMPOS)
        .map((f) => ({ ...f, FechaInicio: fechaIso(f.FechaInicio), FechaLimite: fechaIso(f.FechaLimite), Estado: bool(f.Estado), TipoComputarizado: bool(f.TipoComputarizado) }));
    res.json({ ok: true, data: dt });
}));

// _prIniciarTodo -> _prCargarComboGenerico(cbSucursal, L_fnGeneralSucursales(), "aanumi", "Codigo", "aabdes", "Sucursal")
router.get('/combos', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: { sucursales: v.proyectar(await L.L_fnGeneralSucursales(), ['aanumi', 'aabdes']) } });
}));

// _PMOValidarCampos: Ultimo Nro Factura >= 0 y Sucursal seleccionada
function _leer(body) {
    const UltimoNroFactura = v.entero(body.UltimoNroFactura, 'Ultimo Nro Factura');
    if (UltimoNroFactura < 0) throw new ErrorNegocio('Los Siguientes Campos Son Requeridos: Monto Valido Mayor a 0');
    return {
        SucursalId: v.entero(body.SucursalId, 'una Sucursal', { requeridoPositivo: true }),
        NumeroAutorizacion: v.texto(body.NumeroAutorizacion, 100),
        UltimoNroFactura,
        Llave: v.texto(body.Llave, 500),
        FechaInicio: aVB(fechaReq(body.FechaInicio, 'Fecha Inicio')),
        FechaLimite: aVB(fechaReq(body.FechaLimite, 'Fecha Limite')),
        Nota01: v.texto(body.Nota01, 500),
        Nota02: v.texto(body.Nota02, 500),
        Estado: v.bit(body.Estado),
        TipoComputarizado: v.bit(body.TipoComputarizado),
    };
}

// _PMOGrabarRegistro
router.post('/', asyncHandler(async (req, res) => {
    const r = await L.L_prInsertarDosificacion({ _numi: '', ..._leer(req.body) });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar Dosificación'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `Dosificación #${r._numi} Grabado con Exito.`.toUpperCase() });
}));

// _PMOModificarRegistro
router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const r = await L.L_prModificarDosificacion({ _numi: id, ..._leer(req.body) });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar Dosificación'.toUpperCase());
    res.json({ ok: true, id, mensaje: `Codigo de Dosificación ${id} modificado con Exito.`.toUpperCase() });
}));

// _PMOEliminarRegistro
router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await L.L_prBorrarRegistro(id, 'MAM_Dosificacion'))) throw new ErrorNegocio('Error al eliminar Dosificación'.toUpperCase());
    res.json({ ok: true, mensaje: `Codigo de Dosificación ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
