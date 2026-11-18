// =====================================================================
//  PDF de cualquier reporte o nota: la web manda la misma hoja que imprime (HTML completo, imagenes en data:)
//  y recibe el PDF para descargarlo o compartirlo (WhatsApp) como ARCHIVO adjunto. No se guarda nada en el servidor.
// =====================================================================
const express = require('express');
const rateLimit = require('express-rate-limit');
const { asyncHandler, ErrorNegocio } = require('../middleware/errores');
const { htmlAPdf, ErrorPdf } = require('../utils/pdf');

const router = express.Router();

// limite propio: un reporte grande con logo puede pasar de 1 MB (el resto de la API sigue en 1 MB)
router.use(express.json({ limit: '15mb' }));
router.use(rateLimit({
    windowMs: 60 * 1000, limit: 30, standardHeaders: true, legacyHeaders: false,
    keyGenerator: (req) => String(req.sesion?.gi_userNumi ?? req.ip),
    message: { ok: false, mensaje: 'Demasiados PDF en poco tiempo. Espere un minuto.' },
}));

const nombreArchivo = (s) => (String(s || 'reporte').normalize('NFD').replace(/[̀-ͯ]/g, '')
    .replace(/[^\w\- ]+/g, ' ').trim().replace(/\s+/g, '_').slice(0, 80) || 'reporte') + '.pdf';

router.post('/', asyncHandler(async (req, res) => {
    const html = req.body?.html;
    if (typeof html !== 'string' || !/^\s*<!doctype html>/i.test(html)) throw new ErrorNegocio('No se recibio la hoja para el PDF.');
    // la hoja no puede pedir archivos del servidor (ademas el navegador del PDF no tiene red)
    if (/file:/i.test(html)) throw new ErrorNegocio('La hoja contiene rutas no permitidas.');
    let pdf;
    try {
        pdf = await htmlAPdf(html);
    } catch (e) {
        if (e instanceof ErrorPdf) throw new ErrorNegocio(e.message, 503);
        throw e;
    }
    const nombre = nombreArchivo(req.body?.nombre);
    res.set({
        'Content-Type': 'application/pdf',
        'Content-Length': pdf.length,
        'Content-Disposition': `attachment; filename="${nombre}"`,
    });
    res.end(pdf);
}));

module.exports = router;
