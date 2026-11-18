// =====================================================================
//  PDF y Compartir de cualquier hoja (reportes y notas): la vista previa de global/imprimir.js lo usa.
//   - El PDF lo arma el servidor (POST /api/pdf, server/src/utils/pdf.js): sale igual que la hoja impresa.
//   - Compartir = el ARCHIVO PDF adjunto (nunca un enlace):
//       App Android : menu Compartir del telefono (WhatsApp, Gmail, Telegram...) o directo al chat del cliente.
//       Navegador   : menu Compartir del sistema (celular; PC con https). Si el navegador no puede compartir
//                     archivos (PC por http), se descarga el PDF y se abre WhatsApp para adjuntarlo.
// =====================================================================
const app = typeof window !== 'undefined' ? window.SantitApp : undefined;

/** Codigo de pais para los celulares sin el (Bolivia). */
export const CODIGO_PAIS = '591';

/** Celular en formato internacional para WhatsApp (solo digitos) o '' si no parece un celular. */
export function celularWhatsApp(telefono) {
  const partes = String(telefono || '').split(/[\/,;|]| y /i).map((p) => p.replace(/\D/g, ''));
  for (const d0 of partes) {
    let d = d0.replace(/^00/, '');
    if (/^[67]\d{7}$/.test(d)) d = CODIGO_PAIS + d; // celular boliviano de 8 digitos
    if (d.startsWith(CODIGO_PAIS) ? /^591[67]\d{7}$/.test(d) : d.length >= 10 && d.length <= 15) return d;
  }
  return '';
}

/** Opcion whatsapp de la vista previa para enviar el documento al cliente, o null si no tiene celular. */
export function whatsappCliente({ telefono, cliente, documento, empresa }) {
  const celular = celularWhatsApp(telefono);
  if (!celular) return null;
  const nombre = String(cliente || '').trim();
  const saludo = nombre ? `Hola ${nombre}` : 'Hola';
  return { celular, nombre: nombre || 'el cliente', texto: `${saludo}, le enviamos su ${documento}.${empresa ? ` ${empresa}` : ''}` };
}

export const nombrePdf = (titulo) => `${String(titulo || 'Reporte').normalize('NFD').replace(/[̀-ͯ]/g, '')
  .replace(/[^\w\- ]+/g, ' ').trim().replace(/\s+/g, '_').slice(0, 80) || 'Reporte'}.pdf`;

const aDataUrl = (blob) => new Promise((ok, error) => { const r = new FileReader(); r.onload = () => ok(r.result); r.onerror = error; r.readAsDataURL(blob); });

// Las imagenes (logo) van dentro del HTML: el navegador que arma el PDF no tiene red.
async function conImagenesIncrustadas(html) {
  const urls = [...new Set([...html.matchAll(/\ssrc="([^"]+)"/g)].map((m) => m[1]).filter((u) => !/^data:/i.test(u)))];
  const datos = await Promise.all(urls.map(async (u) => {
    try {
      const url = new URL(u.replace(/&amp;/g, '&'), window.location.origin);
      if (url.origin !== window.location.origin) return '';
      const r = await fetch(url.href, { credentials: 'include' });
      return r.ok ? await aDataUrl(await r.blob()) : '';
    } catch { return ''; }
  }));
  let res = html.replace(/<base [^>]*>/i, '');
  urls.forEach((u, i) => { res = res.split(`src="${u}"`).join(datos[i] ? `src="${datos[i]}"` : 'src="" hidden'); });
  return res;
}

/** Arma el PDF de un documento HTML completo. Devuelve un File. */
export async function generarPdf(html, titulo) {
  const nombre = nombrePdf(titulo);
  const cuerpo = JSON.stringify({ html: await conImagenesIncrustadas(html), nombre });
  let r;
  try {
    r = await fetch('/api/pdf', { method: 'POST', credentials: 'include', cache: 'no-store', headers: { 'Content-Type': 'application/json' }, body: cuerpo });
  } catch {
    throw new Error('No se pudo conectar con el servidor para generar el PDF.');
  }
  if (!r.ok) {
    const j = await r.json().catch(() => ({}));
    throw new Error(r.status === 413 ? 'El reporte es demasiado grande para un PDF. Filtre menos registros.' : j.mensaje || `No se pudo generar el PDF (error ${r.status}).`);
  }
  return new File([await r.blob()], nombre, { type: 'application/pdf' });
}

const base64De = async (file) => String(await aDataUrl(file)).split(',')[1];
const appPuede = (m) => !!app && typeof app[m] === 'function';
// app instalada antes de esta funcion: no puede recibir archivos de la web
const appVieja = (m) => { if (app && !appPuede(m)) throw new Error('Para guardar o compartir el PDF desde la app instale su version nueva. Mientras tanto use Imprimir y elija "Guardar como PDF".'); };

/** Descarga / guarda el PDF. */
export async function descargarPdf(file) {
  appVieja('guardarBinario');
  if (appPuede('guardarBinario')) { app.guardarBinario(file.name, await base64De(file), file.type); return; }
  const a = document.createElement('a');
  a.href = URL.createObjectURL(file);
  a.download = file.name;
  document.body.appendChild(a); a.click(); a.remove();
  setTimeout(() => URL.revokeObjectURL(a.href), 60000);
}

const puedeCompartirNavegador = (file) => { try { return !!navigator.canShare?.({ files: [file] }); } catch { return false; } };

/** ¿Este equipo puede compartir el archivo con el menu del sistema? (para el texto del boton) */
export function hayMenuCompartir() {
  if (appPuede('compartirArchivo')) return true;
  try { return !!navigator.canShare?.({ files: [new File(['x'], 'x.pdf', { type: 'application/pdf' })] }); } catch { return false; }
}

/**
 * Comparte el PDF con el menu del sistema (WhatsApp, correo...).
 * @returns {'compartido'|'cancelado'|'sin-gesto'|'descargado'} 'descargado': este navegador no comparte archivos
 *   (se descargo y se abrio WhatsApp Web); 'sin-gesto': hay que tocar otra vez el boton (el navegador lo exige)
 */
export async function compartirPdf(file, { titulo = '', texto = '' } = {}) {
  appVieja('compartirArchivo');
  if (appPuede('compartirArchivo')) { app.compartirArchivo(file.name, await base64De(file), file.type, titulo || file.name, texto); return 'compartido'; }
  if (puedeCompartirNavegador(file)) {
    try {
      await navigator.share({ files: [file], title: titulo || file.name, ...(texto ? { text: texto } : {}) });
      return 'compartido';
    } catch (e) {
      if (e?.name === 'AbortError') return 'cancelado';
      if (e?.name === 'NotAllowedError') return 'sin-gesto';
      throw e;
    }
  }
  await descargarPdf(file);
  window.open('https://web.whatsapp.com/', 'whatsapp');
  return 'descargado';
}

/**
 * Envia el PDF al WhatsApp de un numero (ej. el cliente de la venta).
 * App: abre el chat de ese numero con el PDF adjunto. Navegador: abre el chat y descarga el PDF para adjuntarlo.
 * @returns {'enviado'|'descargado'}
 */
export async function enviarPdfWhatsApp(file, celular, texto = '') {
  appVieja('enviarWhatsApp');
  if (appPuede('enviarWhatsApp')) { app.enviarWhatsApp(file.name, await base64De(file), file.type, celular, texto); return 'enviado'; }
  await descargarPdf(file);
  window.open(`https://wa.me/${celular}${texto ? `?text=${encodeURIComponent(texto)}` : ''}`, 'whatsapp');
  return 'descargado';
}
