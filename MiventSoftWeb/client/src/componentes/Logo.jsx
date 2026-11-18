// =====================================================================
//  Logo de Sanbit (Distribucion): camion de reparto.
//  SVG en linea (sin archivos ni internet). El mismo dibujo esta en public/favicon.svg.
// =====================================================================
import { useId } from 'react';

export const NOMBRE_SISTEMA = 'Sanbit';
export const LEMA_SISTEMA = 'Distribución';

export default function Logo({ className = 'marca-logo' }) {
  const id = 'sb' + useId().replace(/[^a-zA-Z0-9]/g, ''); // ids unicos: puede haber mas de un logo en pantalla
  return (
    <svg className={className} viewBox="0 0 48 48" aria-hidden="true" focusable="false">
      <defs>
        <linearGradient id={`${id}f`} x1="0" y1="0" x2="1" y2="1">
          <stop offset="0" stopColor="#3b82f6" />
          <stop offset="1" stopColor="#1e3a8a" />
        </linearGradient>
        <linearGradient id={`${id}p`} x1="0" y1="0" x2="0" y2="1">
          <stop offset="0" stopColor="#2563eb" />
          <stop offset="1" stopColor="#1e40af" />
        </linearGradient>
      </defs>
      <rect width="48" height="48" rx="13" fill={`url(#${id}f)`} />
      <rect x=".75" y=".75" width="46.5" height="46.5" rx="12.25" fill="none" stroke="#fff" strokeOpacity=".16" strokeWidth="1.5" />
      {/* carga */}
      <rect x="6.5" y="14" width="21" height="16.5" rx="2.6" fill="#fff" />
      <rect x="9.5" y="19" width="13" height="2.4" rx="1.2" fill="#f59e0b" />
      <rect x="9.5" y="23.4" width="8.5" height="2" rx="1" fill="#1e3a8a" opacity=".55" />
      {/* cabina */}
      <path d="M28.6 18.2h6.1c.9 0 1.7.4 2.2 1.2l3.3 4.9c.3.5.5 1 .5 1.6v4.6H28.6Z" fill="#fff" />
      <path d="M30.4 19.9h3.9c.5 0 .9.2 1.2.6l2.2 3.4h-7.3Z" fill={`url(#${id}p)`} />
      {/* chasis y ruedas */}
      <rect x="6" y="30.2" width="35" height="2.2" rx="1.1" fill="#bfdbfe" />
      <circle cx="13.5" cy="33.6" r="3.9" fill="#1e3a8a" stroke="#fff" strokeWidth="1.7" />
      <circle cx="33.5" cy="33.6" r="3.9" fill="#1e3a8a" stroke="#fff" strokeWidth="1.7" />
      <circle cx="13.5" cy="33.6" r="1.2" fill="#f59e0b" />
      <circle cx="33.5" cy="33.6" r="1.2" fill="#f59e0b" />
    </svg>
  );
}

// Logo + nombre + lema (menu lateral y login). El contenedor (.marca) lo pone quien lo usa.
export function MarcaSistema() {
  return (
    <>
      <Logo />
      <span className="marca-txt"><span>San<b>bit</b></span><small>{LEMA_SISTEMA}</small></span>
    </>
  );
}
