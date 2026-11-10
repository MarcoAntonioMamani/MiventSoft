// =====================================================================
//  Integracion con la app Android (Santit movil, carpeta /android)
//  La app expone window.SantitApp. En el navegador nada de esto se activa.
// =====================================================================
const app = typeof window !== 'undefined' ? window.SantitApp : undefined;

export const esApp = !!app;

export const servidorApp = () => { try { return app?.servidorActual() || ''; } catch { return ''; } };
export const configurarServidorApp = () => app?.configurarServidor();

/** Imprime con el servicio de impresion de Android. Devuelve false si no es la app. */
export function imprimirEnApp(html, titulo, horizontal) {
  if (!app) return false;
  app.imprimir(html, titulo || 'Santit', !!horizontal);
  return true;
}

/** Descarga un texto (CSV): en la app pide donde guardarlo; en el navegador descarga directo. */
export function descargarTexto(nombre, texto, mime = 'text/csv;charset=utf-8') {
  if (app) { app.guardarArchivo(nombre, texto, mime); return; }
  const a = document.createElement('a');
  a.href = URL.createObjectURL(new Blob([texto], { type: mime }));
  a.download = nombre;
  a.click();
  setTimeout(() => URL.revokeObjectURL(a.href), 1000);
}

/** En la app (http) el WebView bloquea navigator.geolocation: se reemplaza por el GPS nativo. */
export function instalarPuenteApp() {
  if (!app) return;
  const pendientes = new Map();
  let n = 0;
  window.__santitUbicacion = (id, codigo, lat, lng, precision, mensaje) => {
    const p = pendientes.get(id);
    if (!p) return;
    pendientes.delete(id);
    if (codigo === 0) p.ok({ coords: { latitude: lat, longitude: lng, accuracy: precision }, timestamp: Date.now() });
    else p.error?.({ code: codigo, message: mensaje });
  };
  const geo = {
    getCurrentPosition(ok, error) { const id = String(++n); pendientes.set(id, { ok, error }); app.obtenerUbicacion(id); },
    watchPosition(ok, error) { this.getCurrentPosition(ok, error); return 0; },
    clearWatch() {},
  };
  try { Object.defineProperty(navigator, 'geolocation', { value: geo, configurable: true }); } catch { /* navegador sin soporte */ }
}
