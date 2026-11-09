// Efecto "ripple" de Material Design en botones, items de menu y tarjetas
const SELECTOR = '.btn, .btn-icono, .menu-item, .menu-mod-link, .tile, .fab, .pag-num, .tabs a, .lista-modulos button, .usuario-btn';

export function activarRipple() {
  if (window.matchMedia?.('(prefers-reduced-motion: reduce)').matches) return;
  document.addEventListener('pointerdown', (e) => {
    const el = e.target.closest(SELECTOR);
    if (!el || el.disabled || el.classList.contains('pendiente')) return;
    const r = el.getBoundingClientRect();
    const d = Math.max(r.width, r.height) * 2;
    const onda = document.createElement('span');
    onda.className = 'ripple';
    onda.style.cssText = `width:${d}px;height:${d}px;left:${e.clientX - r.left - d / 2}px;top:${e.clientY - r.top - d / 2}px`;
    el.appendChild(onda);
    onda.addEventListener('animationend', () => onda.remove());
  }, { passive: true });
}
