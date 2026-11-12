// =====================================================================
//  KAILY LA PAZ: btnTipoCambio de Tec_Principal -> ActualizarTipoCambio.vb
//   * Monto actual: el mismo SELECT de L_Validar_Usuario (L_ObtenerTipoCambio)
//   * Guardar: InsertarTipoCambio (MAM_Ventas @tipo 7)
//  En el escritorio el boton no depende de permisos: lo usa todo usuario con sesion.
// =====================================================================
const express = require('express');
const L = require('../negocio/AccesoLogica');
const { asyncHandler, ErrorNegocio } = require('../middleware/errores');
const v = require('../utils/validar');

const router = express.Router();

// ActualizarTipoCambio_Load: tbTipoCambio.Value = Global_TipoCambio
router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: { monto: Number(await L.L_ObtenerTipoCambio()) || 0 } });
}));

// btnGuardar_Click
router.post('/', asyncHandler(async (req, res) => {
    const monto = v.decimal(req.body?.monto);
    if (!(monto > 0)) throw new ErrorNegocio('Debe Ingresar un Valor Correcto en Tipo De Cambio');
    const r = await L.InsertarTipoCambio(monto);
    if (!r.resultado) throw new ErrorNegocio('Error Al guardar Tipo De Cambio');
    res.json({ ok: true, data: { monto }, mensaje: 'Tipo De Cambio Actualizado Correctamente' });
}));

module.exports = router;
