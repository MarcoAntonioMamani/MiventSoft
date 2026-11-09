// =====================================================================
//  Cliente HTTP. La sesion viaja en cookie httpOnly (credentials: 'include').
// =====================================================================
let _alExpirarSesion = () => {};
export function alExpirarSesion(fn) {
  _alExpirarSesion = fn;
}

async function _peticion(metodo, url, cuerpo) {
  const opciones = { method: metodo, credentials: 'include', headers: {} };
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
