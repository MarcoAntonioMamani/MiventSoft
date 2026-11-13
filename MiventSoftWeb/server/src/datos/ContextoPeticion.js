// =====================================================================
//  Contexto de cada peticion (AsyncLocalStorage): usuario autenticado y, en lo que se modifica o
//  elimina, los datos del equipo del cliente para la auditoria (middleware/clienteAuditoria.js).
//  Lo usan negocio/AccesoLogica.js (L_Usuario) y datos/MetodoDatos.js (AuditoriaContexto, sql/10).
// =====================================================================
const { AsyncLocalStorage } = require('node:async_hooks');

const contexto = new AsyncLocalStorage();

/** { ip, navegador, equipo, latitud, longitud, precision } o null */
const clienteActual = () => contexto.getStore()?.cliente || null;

/** Usuario autenticado (nombre completo) o null */
const usuarioActual = () => contexto.getStore()?.usuario || null;

module.exports = { contexto, clienteActual, usuarioActual };
