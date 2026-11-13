// =====================================================================
//  Cliente HTTP. La sesion viaja en cookie httpOnly (credentials: 'include').
// =====================================================================
import { cabecerasEquipo } from '../global/equipo';
let _alExpirarSesion = () => {};
export function alExpirarSesion(fn) {
  _alExpirarSesion = fn;
}

// Barra de progreso global: mientras haya peticiones, <html class="red-activa"> (la dibuja el CSS).
// Sin estado de React (no re-renderiza nada) y solo si tarda mas de 150 ms (las rapidas no parpadean).
let _enCurso = 0;
let _espera = null;
function _actividad(delta) {
  _enCurso = Math.max(0, _enCurso + delta);
  const raiz = typeof document === 'undefined' ? null : document.documentElement;
  if (!raiz) return;
  if (_enCurso > 0) {
    if (!_espera && !raiz.classList.contains('red-activa')) {
      _espera = setTimeout(() => { _espera = null; if (_enCurso > 0) raiz.classList.add('red-activa'); }, 150);
    }
  } else {
    clearTimeout(_espera);
    _espera = null;
    raiz.classList.remove('red-activa');
  }
}

async function _peticion(metodo, url, cuerpo) {
  _actividad(1);
  try {
    return await _enviar(metodo, url, cuerpo);
  } finally {
    _actividad(-1);
  }
}

async function _enviar(metodo, url, cuerpo) {
  // no-store: siempre a la BD, nunca una respuesta guardada por el navegador
  const opciones = { method: metodo, credentials: 'include', headers: {}, cache: 'no-store' };
  // Auditoria: desde que equipo se modifica o elimina (global/equipo.js)
  if (metodo === 'PUT' || metodo === 'DELETE') Object.assign(opciones.headers, await cabecerasEquipo());
  if (cuerpo instanceof FormData) {
    opciones.body = cuerpo;
  } else if (cuerpo !== undefined) {
    opciones.headers['Content-Type'] = 'application/json';
    opciones.body = JSON.stringify(cuerpo);
  }
  let resp;
  try {
    resp = await fetch(url, opciones);
  } catch {
    throw new Error('No se pudo conectar con el servidor.');
  }
  const json = await resp.json().catch(() => ({ ok: false, mensaje: `Error ${resp.status}` }));
  if (resp.status === 401 && !url.endsWith('/login')) _alExpirarSesion(json.mensaje);
  if (!resp.ok || json.ok === false) {
    const err = new Error(json.mensaje || 'Error inesperado');
    err.traza = json.traza;
    err.datos = json.datos; // detalle de negocio (ej. productos sin stock suficiente)
    throw err;
  }
  return json;
}

export const api = {
  get: (url) => _peticion('GET', url),
  post: (url, cuerpo) => _peticion('POST', url, cuerpo),
  put: (url, cuerpo) => _peticion('PUT', url, cuerpo),
  del: (url) => _peticion('DELETE', url),
};

export const urlImagen = (tipo, nombre) =>
  nombre && !/^default\.jpg$/i.test(nombre) ? `/api/imagenes/${tipo}?nombre=${encodeURIComponent(nombre)}` : null;
