// =====================================================================
//  Tec_Sucursales.vb (programa: btInvAmacen)  SP: MAM_Almacenes 1 / 2 / -1, lista L_prListarGeneral, depositos @tipo 4
//  Reglas VB: nombre y deposito requeridos; no se elimina la primera sucursal de la lista.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btInvAmacen'));
const CAMPOS = ['Id', 'DepositoId', 'NombreAlmacen', 'NombreDeposito', 'Direccion', 'Descripcion', 'Telefono', 'estado'];

async function leer(b) {
    const NombreAlmacen = v.texto(b.NombreAlmacen, 150).toUpperCase();
    if (!NombreAlmacen) throw new ErrorNegocio('Ingrese el Nombre de la Sucursal');
    const DepositoId = Number(b.DepositoId);
    if (!v.proyectar(await L.L_prListarDepositos(), ['Id']).some((d) => Number(d.Id) === DepositoId)) throw new ErrorNegocio('Seleccione Un Deposito');
    return { NombreAlmacen, DepositoId, Direccion: v.texto(b.Direccion, 250), Descripcion: v.texto(b.Descripcion, 250), Telefono: v.texto(b.Telefono, 50), estado: v.bit(b.estado) ? 1 : 0 };
}
router.get('/', asyncHandler(async (req, res) => res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_Almacenes'), CAMPOS) })));
router.get('/depositos', asyncHandler(async (req, res) => res.json({ ok: true, data: v.proyectar(await L.L_prListarDepositos(), ['Id', 'NombreDeposito']) })));
router.post('/', asyncHandler(async (req, res) => {
    const r = await L.L_prSucursalInsertar({ ...(await leer(req.body || {})), _numi: '' });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar la Sucursal'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `CODIGO DE SUCURSAL ${r._numi} GRABADO CON EXITO.` });
}));
router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await L.L_prSucursalActualizar({ ...(await leer(req.body || {})), _numi: String(id) }))) throw new ErrorNegocio('Error al modificar la Sucursal'.toUpperCase());
    res.json({ ok: true, id, mensaje: `CODIGO DE SUCURSAL ${id} MODIFICADO CON EXITO.` });
}));
router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const lista = v.proyectar(await L.L_prListarGeneral('MAM_Almacenes'), ['Id']);
    if (Number(lista[0]?.Id) === id) throw new ErrorNegocio('No se Puede Eliminar Todas Las Sucursales');
    if (!(await L.L_prSucursalBorrar(id))) throw new ErrorNegocio('No se pudo eliminar la Sucursal (puede tener movimientos registrados)');
    res.json({ ok: true, mensaje: `CODIGO DE SUCURSAL ${id} ELIMINADO CON EXITO.` });
}));
module.exports = router;
