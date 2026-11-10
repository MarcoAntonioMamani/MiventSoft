// =====================================================================
//  Impresion de documentos (nota de venta, reportes)
//   - Vista previa: ventana emergente con la hoja tal cual saldra impresa y los botones Imprimir / Cerrar
//     (como el visor de Crystal del escritorio). Al cerrarla se sigue en el programa donde se estaba.
//   - Imprimir abre la hoja en una ventana aparte: el cuadro de impresion del navegador sale ahi, no sobre la app.
//   - Se imprime solo el documento: sin la app detras ni espacios en blanco.
//   - @page sin margen: el navegador no tiene donde poner su encabezado/pie (fecha, titulo, URL).
//     El margen de la hoja lo da el documento (padding del body, o filas espaciadoras en los reportes).
// =====================================================================

import { imprimirEnApp } from './appMovil';

export const esc =(s) => String(s ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

const CSS_BASE = `@page { size: letter portrait; margin: 0; }
html, body { margin: 0; padding: 0; background: #fff; }`;

// Solo en pantalla (vista previa): la hoja carta sobre fondo gris. No afecta la impresion.
const cssPrevia = (horizontal) => `@media screen {
  html { background: #525659; }
  body { box-sizing: border-box; width: ${horizontal ? '11in' : '8.5in'}; min-height: ${horizontal ? '8.5in' : '11in'}; margin: 18px auto !important; background: #fff; box-shadow: 0 4px 18px rgba(0,0,0,.45); }
}`;

const documento = ({ cuerpo, css, horizontal, previa }) => {
  const pagina = horizontal ? CSS_BASE.replace('letter portrait', 'letter landscape') : CSS_BASE;
  // titulo en blanco: si el usuario activa encabezados del navegador, no repite el nombre
  return `<!doctype html><html><head><meta charset="utf-8"><base href="${window.location.origin}/"><title> </title><style>${pagina}${css}${previa ? cssPrevia(horizontal) : ''}</style></head><body>${cuerpo}</body></html>`;
};

const esperarImagenes = (doc) => Promise.race([
  Promise.all(Array.from(doc.images).map((i) => (i.complete ? null : new Promise((r) => { i.onload = r; i.onerror = r; })))),
  new Promise((r) => setTimeout(r, 3000)),
]);

// Impresion directa en un iframe oculto (sin vista previa)
function imprimirDirecto(html, { titulo, horizontal } = {}) {
  if (imprimirEnApp(html, titulo, horizontal)) return; // app Android: impresion nativa
  const ifr = document.createElement('iframe');
  ifr.setAttribute('aria-hidden', 'true');
  ifr.setAttribute('tabindex', '-1');
  Object.assign(ifr.style, { position: 'fixed', right: '0', bottom: '0', width: '0', height: '0', border: '0' });
  document.body.appendChild(ifr);
  let quitado = false;
  const quitar = () => { if (!quitado) { quitado = true; ifr.remove(); } };
  try {
    const doc = ifr.contentDocument;
    doc.open(); doc.write(html); doc.close();
    esperarImagenes(doc).then(() => {
      const w = ifr.contentWindow;
      w.addEventListener('afterprint', () => setTimeout(quitar, 300), { once: true });
      w.focus();
      w.print();
      setTimeout(quitar, 60000); // por si el navegador no emite afterprint
    });
  } catch (e) {
    quitar();
    window.print();
  }
}

// Vista previa en ventana emergente: hoja + Imprimir / Cerrar. Esc o clic fuera cierra.
function vistaPrevia(html, { titulo, horizontal, aviso = '', textoCerrar = 'Cerrar', alCerrar }) {
  const antes = document.activeElement;
  const fondo = document.createElement('div');
  fondo.className = 'vp-fondo';
  fondo.innerHTML = `<div class="vp" role="dialog" aria-modal="true" aria-label="${esc(`Vista previa: ${titulo}`)}">
  <div class="vp-cab">
    <div class="vp-tit"><b>${esc(titulo)}</b><span>Vista previa · hoja carta ${horizontal ? 'horizontal' : 'vertical'}</span>${aviso ? `<span class="vp-aviso" role="status">${esc(aviso)}</span>` : ''}</div>
    <div class="vp-acc">
      <button type="button" class="btn btn-sec vp-cerrar">${esc(textoCerrar)}</button>
      <button type="button" class="btn btn-pri vp-imprimir"><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M6 9V2h12v7"/><path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"/><rect x="6" y="14" width="12" height="8"/></svg> Imprimir</button>
    </div>
  </div>
  <div class="vp-cuerpo"><iframe class="vp-hoja" title="${esc(`Hoja: ${titulo}`)}"></iframe></div>
</div>`;
  document.body.appendChild(fondo);
  document.body.classList.add('vp-abierta');
  const ifr = fondo.querySelector('iframe');
  const doc = ifr.contentDocument;
  doc.open(); doc.write(html); doc.close();

  // en pantallas angostas la hoja se achica para verla completa
  const anchoHoja = (horizontal ? 11 : 8.5) * 96 + 36;
  const ajustar = () => { try { doc.documentElement.style.zoom = String(Math.min(1, (ifr.clientWidth - 8) / anchoHoja)); } catch { /* sin acceso */ } };
  ajustar();
  const ro = new ResizeObserver(ajustar); ro.observe(ifr);

  let cerrado = false;
  // silencioso = true: lo cierra el programa (no se avisa con alCerrar)
  const cerrar = (silencioso) => {
    if (cerrado) return; cerrado = true;
    ro.disconnect();
    document.removeEventListener('keydown', teclas, true);
    try { doc.removeEventListener('keydown', teclas, true); } catch { /* ya cerrado */ }
    fondo.remove();
    if (!document.querySelector('.vp-fondo')) document.body.classList.remove('vp-abierta');
    if (antes && document.contains(antes)) antes.focus({ preventScroll: true });
    if (silencioso !== true && alCerrar) alCerrar();
  };
  const btnImp = () => fondo.querySelector('.vp-imprimir');
  // al terminar, el foco vuelve al boton (Escape y Ctrl+P siguen funcionando)
  // Imprimir en una VENTANA APARTE: el cuadro de impresion del navegador aparece en esa ventana y no tapa la app.
  // Al imprimir o cancelar, la ventana se cierra sola. Si el navegador bloquea ventanas emergentes, se imprime desde la hoja.
  const imprimir = () => {
    if (imprimirEnApp(html.replace('<title> </title>', `<title>${esc(titulo)}</title>`), titulo, horizontal)) return; // app Android
    const w = window.screen?.availWidth || 1280; const h = window.screen?.availHeight || 900;
    const ancho = Math.min(w - 40, horizontal ? 1180 : 940); const alto = Math.min(h - 40, 1000);
    const win = window.open('', 'santit_imprimir', `popup=yes,width=${ancho},height=${alto},left=${Math.max(0, (w - ancho) / 2)},top=${Math.max(0, (h - alto) / 2)}`);
    if (!win) { esperarImagenes(doc).then(() => { ifr.contentWindow.focus(); ifr.contentWindow.print(); btnImp()?.focus({ preventScroll: true }); }); return; }
    win.document.open(); win.document.write(html.replace('<title> </title>', `<title>${esc(titulo)}</title>`)); win.document.close();
    let cerrada = false;
    const cerrarVentana = () => { if (!cerrada) { cerrada = true; try { win.close(); } catch { /* ya cerrada */ } } };
    win.addEventListener('afterprint', () => setTimeout(cerrarVentana, 200), { once: true });
    esperarImagenes(win.document).then(() => { win.focus(); win.print(); });
  };
  function teclas(e) {
    if (e.key === 'Escape') { e.preventDefault(); e.stopPropagation(); cerrar(); }
    if (e.key === 'Tab') { // el foco no sale de la ventana
      const bs = [...fondo.querySelectorAll('button')];
      const i = bs.indexOf(document.activeElement);
      if (e.shiftKey && i <= 0) { e.preventDefault(); bs[bs.length - 1].focus(); } else if (!e.shiftKey && i === bs.length - 1) { e.preventDefault(); bs[0].focus(); }
    }
    if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'p') { e.preventDefault(); imprimir(); } // Ctrl+P imprime la hoja, no la app
  }
  document.addEventListener('keydown', teclas, true);
  doc.addEventListener('keydown', teclas, true); // tambien con el foco dentro de la hoja
  fondo.addEventListener('mousedown', (e) => { if (e.target === fondo) cerrar(); });
  fondo.querySelector('.vp-cerrar').addEventListener('click', () => cerrar());
  fondo.querySelector('.vp-imprimir').addEventListener('click', imprimir);
  fondo.querySelector('.vp-imprimir').focus();
  return { cerrar, imprimir };
}

/**
 * Imprime un documento HTML: abre la vista previa (o imprime directo si directo = true).
 * @param {{ cuerpo: string, css?: string, horizontal?: boolean, titulo?: string, directo?: boolean, aviso?: string, textoCerrar?: string, alCerrar?: () => void }} o
 *   cuerpo: HTML ya escapado; css: estilos propios del documento
 *   aviso: texto bajo el titulo (ej. "Venta grabada. ¿Desea imprimir?"); alCerrar: al cerrar la vista previa
 */
export function imprimirHtml({ cuerpo, css = '', horizontal = false, titulo = 'Reporte', directo = false, aviso, textoCerrar, alCerrar }) {
  if (directo) { imprimirDirecto(documento({ cuerpo, css, horizontal, previa: false }), { titulo, horizontal }); return null; }
  return vistaPrevia(documento({ cuerpo, css, horizontal, previa: true }), { titulo, horizontal, aviso, textoCerrar, alCerrar });
}
