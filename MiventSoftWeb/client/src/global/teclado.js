// =====================================================================
//  Teclado en celular (para todos los campos de la app, sin tocar cada pantalla)
//   1) Buscadores y campos de texto: sin autocompletado del navegador. Chrome en Android
//      mostraba sobre el teclado la barra negra de tarjetas / direcciones / claves.
//      Solo se aplica a campos que no definieron autoComplete (el login conserva username / password).
//   2) Alto real visible: con el teclado abierto se publican --vv-alto (alto visible) y
//      --teclado (lo que tapa el teclado) para que hojas y modales queden encima del teclado
//      (iOS y navegadores que no achican la pagina; en Android lo hace interactive-widget del index.html).
//   3) El campo con foco se lleva a la vista cuando aparece el teclado.
// =====================================================================
const TIPOS_TEXTO = new Set(['text', 'search', 'number', 'tel', 'url', 'email', '']);

function sinAutocompletar(el) {
  if (!(el instanceof HTMLInputElement || el instanceof HTMLTextAreaElement)) return;
  if (el instanceof HTMLInputElement && !TIPOS_TEXTO.has(el.type)) return; // password, date, checkbox...
  if (el.hasAttribute('autocomplete')) return; // definido a proposito (login, usuarios)
  el.setAttribute('autocomplete', 'off');
  el.setAttribute('autocorrect', 'off');
  el.setAttribute('autocapitalize', 'off');
  el.setAttribute('spellcheck', 'false');
  // nombre no reconocible: Chrome no lo asocia a tarjeta / direccion
  if (!el.name) el.setAttribute('name', `campo-${Math.random().toString(36).slice(2, 8)}`);
  if (el instanceof HTMLInputElement && (el.type === 'search' || /buscar/i.test(el.placeholder || el.getAttribute('aria-label') || ''))) {
    el.setAttribute('enterkeyhint', 'search');
  }
}

export function activarTeclado() {
  if (typeof window === 'undefined') return;
  // al enfocar (antes de que el navegador ofrezca autocompletar) y al aparecer campos nuevos
  document.addEventListener('focusin', (e) => sinAutocompletar(e.target), true);
  const marcar = (raiz) => raiz.querySelectorAll?.('input, textarea').forEach(sinAutocompletar);
  marcar(document);
  new MutationObserver((ms) => ms.forEach((m) => m.addedNodes.forEach((n) => {
    if (n.nodeType !== 1) return;
    sinAutocompletar(n);
    marcar(n);
  }))).observe(document.body, { childList: true, subtree: true });

  const vv = window.visualViewport;
  if (!vv) return;
  const raiz = document.documentElement.style;
  let previo = 0;
  const ajustar = () => {
    const teclado = Math.max(0, Math.round(window.innerHeight - vv.height - vv.offsetTop));
    raiz.setProperty('--vv-alto', `${Math.round(vv.height)}px`);
    raiz.setProperty('--teclado', `${teclado}px`);
    document.documentElement.classList.toggle('con-teclado', teclado > 80 || vv.height < previo - 120);
    // el teclado aparecio: llevar el campo con foco a la vista (sin saltos si ya se ve)
    const el = document.activeElement;
    if (vv.height < previo - 80 && el && /^(INPUT|TEXTAREA)$/.test(el.tagName)) {
      setTimeout(() => el.scrollIntoView({ block: 'nearest', behavior: 'smooth' }), 60);
    }
    previo = vv.height;
  };
  previo = vv.height;
  vv.addEventListener('resize', ajustar);
  vv.addEventListener('scroll', ajustar);
  ajustar();
}
