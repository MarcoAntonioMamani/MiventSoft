// =====================================================================
//  Sesion (equivale a gs_user / gi_userNumi / gi_userRol / gi_userSuc de P_Global.vb)
//  JWT en cookie httpOnly + SameSite=Strict (no accesible desde JavaScript).
// =====================================================================
const jwt = require('jsonwebtoken');
const { L_prConUsuario } = require('../negocio/AccesoLogica');

const NOMBRE_COOKIE = 'mivent_sesion';

function _secreto() {
    const s = process.env.JWT_SECRET;
    if (!s || s.length < 32) throw new Error('JWT_SECRET no configurado o demasiado corto (minimo 32 caracteres)');
    return s;
}

function crearSesion(res, datos) {
    const horas = Number(process.env.JWT_HORAS || 10);
    const token = jwt.sign(datos, _secreto(), { expiresIn: `${horas}h` });
    res.cookie(NOMBRE_COOKIE, token, {
        httpOnly: true,
        sameSite: 'strict',
        // Secure solo si se sirve por HTTPS (COOKIE_SECURE=true); en LAN por http debe ser false
        secure: process.env.COOKIE_SECURE === 'true',
        maxAge: horas * 3600 * 1000,
        path: '/',
    });
}

function cerrarSesion(res) {
    res.clearCookie(NOMBRE_COOKIE, { path: '/' });
}

// Requiere sesion valida y ejecuta el resto de la peticion con L_Usuario = gs_user
function requiereSesion(req, res, next) {
    const token = req.cookies?.[NOMBRE_COOKIE];
    if (!token) return res.status(401).json({ ok: false, mensaje: 'Sesion no iniciada.' });
    try {
        req.sesion = jwt.verify(token, _secreto());
    } catch {
        cerrarSesion(res);
        return res.status(401).json({ ok: false, mensaje: 'La sesion expiro. Ingrese nuevamente.' });
    }
    L_prConUsuario(req.sesion.gs_user, () => next(), req.clienteAud);
}

module.exports = { crearSesion, cerrarSesion, requiereSesion };
