// =====================================================================
//  Manejo de errores centralizado (reemplaza los MsgBox(ex.Message) de MetodoDatos.vb)
//  - Errores de negocio lanzados desde los SP con RAISERROR/THROW (numero >= 50000)
//    se muestran tal cual al usuario (como el ToastNotification rojo).
//  - Cualquier otro error se registra en el servidor y al navegador va un mensaje
//    generico + un codigo de traza (no se exponen detalles internos de SQL).
// =====================================================================
const crypto = require('node:crypto');

class ErrorNegocio extends Error {
    constructor(mensaje, status = 400) {
        super(mensaje);
        this.status = status;
        this.esNegocio = true;
    }
}

// Envuelve handlers async: errores -> manejadorErrores, y re-establece L_Usuario
// (por si un middleware basado en streams, como multer, perdio el contexto async)
const asyncHandler = (fn) => (req, res, next) => {
    const { L_prConUsuario } = require('../negocio/AccesoLogica');
    const ejecutar = () => Promise.resolve(fn(req, res, next)).catch(next);
    return req.sesion ? L_prConUsuario(req.sesion.gs_user, ejecutar) : ejecutar();
};

// eslint-disable-next-line no-unused-vars
function manejadorErrores(err, req, res, next) {
    if (err.esNegocio) {
        return res.status(err.status).json({ ok: false, mensaje: err.message });
    }
    const numeroSql = err.number || err.originalError?.info?.number;
    if (numeroSql && numeroSql >= 50000) {
        return res.status(400).json({ ok: false, mensaje: err.message });
    }
    if (err.code === 'LIMIT_FILE_SIZE') {
        return res.status(400).json({ ok: false, mensaje: 'La imagen supera el tamano maximo permitido (3 MB).' });
    }
    const traza = crypto.randomUUID().slice(0, 8);
    // Se registra el error pero NO el cuerpo de la peticion (puede contener contrasenas)
    console.error(`[${new Date().toISOString()}] [traza ${traza}] ${req.method} ${req.originalUrl} ->`, err.message);
    if (process.env.NODE_ENV !== 'production') console.error(err.stack);
    const mensaje =
        err.code === 'ESOCKET' || err.code === 'ELOGIN' || err.code === 'ETIMEOUT'
            ? 'No se pudo conectar con la base de datos. Verifique el servidor SQL.'
            : 'Ocurrio un error inesperado al procesar la solicitud.';
    res.status(500).json({ ok: false, mensaje, traza });
}

module.exports = { ErrorNegocio, asyncHandler, manejadorErrores };
