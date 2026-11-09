// =====================================================================
//  Tec_Mapaclientes.vb (programa: btZonaMapaCliente)  SP: MAM_Clientes @tipo 6 (todos) / @tipo 7 (@ZonaId)
//  Zonas: L_prListarZonas (MAM_Clientes @tipo 4). Solo lectura. Clientes sin coordenadas no se dibujan (como VB).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { col, num } = require('../distribucion/comun');

const router = express.Router();
router.use(requierePrograma('btZonaMapaCliente'));

router.get('/zonas', asyncHandler(async (req, res) => res.json({ ok: true, data: v.proyectar(await L.L_prListarZonas(), ['Id', 'NombreZona']) })));
router.get('/', asyncHandler(async (req, res) => {
    const zona = Number.parseInt(req.query.zona, 10);
    const dt = zona > 0 ? await L.L_prMapaCLienteGeneralPorZona(zona) : await L.L_prMapaCLienteGeneral();
    res.json({ ok: true, data: dt.map((f) => ({ Id: num(col(f, 'Id')), NombreCliente: col(f, 'NombreCliente') ?? '', Telefono: col(f, 'Telefono') ?? '',
        Direccion: col(f, 'DireccionCliente') ?? col(f, 'Direccion') ?? '', Latitud: num(col(f, 'Latitud')), Longitud: num(col(f, 'Longitud')) })) });
}));

module.exports = router;
