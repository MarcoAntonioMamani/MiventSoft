// =====================================================================
//  Este equipo, para la Auditoria De Registros (server: middleware/clienteAuditoria.js, sql/10)
//  En cada modificacion o eliminacion (PUT / DELETE) se envian:
//   X-Equipo-Id     codigo fijo de este navegador (se crea una vez: "7F3A9C")
//   X-Equipo-Nombre nombre que se le dio a este equipo (menu del usuario > "Nombre de este equipo")
//   X-Ubicacion     "lat,lng,precision" si hay GPS: en la app Android siempre (permiso de ubicacion);
//                   en el navegador solo con https o localhost (por http el navegador no la entrega)
//  El navegador no puede leer el nombre de la PC: por eso se le da un nombre aqui.
// =====================================================================
import { esApp } from './appMovil';

const K_ID = 'mivent.equipo.id';
const K_NOMBRE = 'mivent.equipo.nombre';

const leer = (k) => { try { return localStorage.getItem(k) || ''; } catch { return ''; } };
const guardar = (k, v) => { try { if (v) localStorage.setItem(k, v); else localStorage.removeItem(k); } catch { /* sin almacenamiento */ } };

let _id = '';
export function idEquipo() {
  if (_id) return _id;
  _id = leer(K_ID);
  if (!/^[0-9A-F]{6}$/.test(_id)) {
    const b = new Uint8Array(3);
    (globalThis.crypto?.getRandomValues ? globalThis.crypto.getRandomValues(b) : b.forEach((_, i) => { b[i] = Math.floor(Math.random() * 256); }));
    _id = Array.from(b, (x) => x.toString(16).padStart(2, '0')).join('').toUpperCase();
    guardar(K_ID, _id);
  }
  return _id;
}

export const nombreEquipo = () => leer(K_NOMBRE);
export const guardarNombreEquipo = (nombre) => guardar(K_NOMBRE, String(nombre || '').trim().slice(0, 60));

/** Hay forma de obtener la ubicacion en este equipo? */
export const ubicacionDisponible = () => typeof navigator !== 'undefined' && !!navigator.geolocation && (esApp || !!window.isSecureContext);

// Ultima ubicacion (vale 2 minutos). Si el usuario la nego, no se vuelve a pedir en esta sesion.
let _ultima = null;
let _negada = false;
let _pidiendo = null;
function pedirUbicacion(espera) {
  if (_pidiendo) return _pidiendo;
  _pidiendo = new Promise((resolver) => {
    const fin = (v) => { clearTimeout(t); _pidiendo = null; resolver(v); };
    const t = setTimeout(() => fin(null), espera);
    try {
      navigator.geolocation.getCurrentPosition(
        (p) => { _ultima = { lat: p.coords.latitude, lng: p.coords.longitude, precision: p.coords.accuracy, t: Date.now() }; fin(_ultima); },
        (e) => { if (e?.code === 1) _negada = true; fin(null); },
        { enableHighAccuracy: true, timeout: espera, maximumAge: 120000 },
      );
    } catch { fin(null); }
  });
  return _pidiendo;
}

/** Ubicacion para la auditoria: la reciente al instante; si no hay, espera como maximo 3 s (nunca bloquea mas) */
export async function ubicacionAuditoria() {
  if (_negada || !ubicacionDisponible()) return null;
  if (_ultima && Date.now() - _ultima.t < 120000) return _ultima;
  return pedirUbicacion(3000);
}

/** Cabeceras para PUT / DELETE */
export async function cabecerasEquipo() {
  const h = { 'X-Equipo-Id': idEquipo() };
  const n = nombreEquipo();
  if (n) h['X-Equipo-Nombre'] = encodeURIComponent(n);
  const u = await ubicacionAuditoria().catch(() => null);
  // coordenadas con TODOS los decimales que da el GPS (el servidor guarda hasta 18: sql/12)
  const c = (n) => (/e/i.test(String(n)) ? n.toFixed(18) : String(n));
  if (u) h['X-Ubicacion'] = `${c(u.lat)},${c(u.lng)},${Math.round(u.precision || 0)}`;
  return h;
}
