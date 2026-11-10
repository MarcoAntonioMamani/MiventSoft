// Sirve las imagenes de la carpeta compartida con el escritorio (solo con sesion)
const express = require('express');
const { RutaImagen } = require('../utils/imagenes');

const router = express.Router();

router.get('/:tipo', (req, res) => {
    let ruta = null;
    try {
        ruta = RutaImagen(req.params.tipo, req.query.nombre);
    } catch {
        ruta = null;
    }
    if (!ruta) return res.status(404).end();
    res.set('Cache-Control', 'private, max-age=300');
    res.sendFile(ruta);
});

module.exports = router;
