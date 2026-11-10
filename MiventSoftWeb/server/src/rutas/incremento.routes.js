// =====================================================================
//  Tec_IncrementoVenta.vb + Rep_HistorialIncrementoVenta.vb  (boton de la barra superior)
//  SP: MAM_VentasIncremento @tipo 1 (vigente), 2 (guardar), 3 (historial)
//  Igual que el escritorio: cualquier usuario con sesion puede verlo y cambiarlo;
//  el SP registra quien lo cambio (historial).
// =====================================================================
const express = require('express');
const L = require('../negocio/AccesoLogica');
const { asyncHandler, ErrorNegocio } = require('../middleware/errores');
const v = require('../utils/validar');

const router = express.Router();
const dos = (n) => String(n).padStart(2, '0');

router.get('/', asyncHandler(async (req, res) => {
    const dt = await L.L_fnObtenerIncrementoVentaHoy();
    const k = dt[0] ? Object.keys(dt[0]).find((x) => x.toLowerCase() === 'monto') : null;
    res.json({ ok: true, data: { monto: k ? Number(dt[0][k]) : 2.0 } }); // sin configurar: 2.00 como VB
}));

router.put('/', asyncHandler(async (req, res) => {
    const monto = Number.parseFloat(req.body?.monto);
    if (!Number.isFinite(monto) || monto <= 0) throw new ErrorNegocio('Ingrese Un Monto Valido Mayor A Cero'.toUpperCase());
    if (monto > 100000) throw new ErrorNegocio('Monto demasiado alto');
    await L.L_prGuardarIncrementoVentaHoy(monto.toFixed(2)); // VB: monto.ToString("0.00")
    res.json({ ok: true, data: { monto: Number(monto.toFixed(2)) }, mensaje: 'Incremento Del Dia Actualizado Correctamente'.toUpperCase() });
}));

router.get('/historial', asyncHandler(async (req, res) => {
    const f = (t) => { const s = v.texto(t, 10); if (!/^\d{4}-\d{2}-\d{2}$/.test(s)) throw new ErrorNegocio('Fecha no valida'); return s.replace(/-/g, '/'); };
    const dt = await L.L_prListarHistorialIncrementoVenta(f(req.query.desde), f(req.query.hasta));
    const fmt = (x) => (x instanceof Date ? `${x.getUTCFullYear()}-${dos(x.getUTCMonth() + 1)}-${dos(x.getUTCDate())} ${dos(x.getUTCHours())}:${dos(x.getUTCMinutes())}` : x);
    res.json({ ok: true, data: v.proyectar(dt, ['Id', 'Fecha', 'Monto', 'Usuario', 'FechaHoraModificacion']).map((r) => ({ ...r, Fecha: fmt(r.Fecha)?.slice?.(0, 10) ?? r.Fecha, FechaHoraModificacion: fmt(r.FechaHoraModificacion) })) });
}));

module.exports = router;
