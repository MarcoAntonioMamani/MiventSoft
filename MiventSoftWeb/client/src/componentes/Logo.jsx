// =====================================================================
//  Logo de Sanbit (Punto de Ventas): terminal POS con ticket saliendo.
//  SVG en linea (sin archivos ni internet). El mismo dibujo esta en public/favicon.svg.
// =====================================================================
import { useId } from 'react';

export const NOMBRE_SISTEMA = 'Sanbit';
export const LEMA_SISTEMA = 'Punto de Ventas';

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
      {/* ticket */}
      <path d="M16.5 22V7.4l2.5 1.5 2.5-1.5 2.5 1.5 2.5-1.5 2.5 1.5 2.5-1.5V22Z" fill="#fff" />
      <path d="M19.8 12.6h8.4M19.8 16.2h5.2" stroke="#f59e0b" strokeWidth="1.9" strokeLinecap="round" />
      {/* terminal */}
      <rect x="9.5" y="20" width="29" height="21" rx="5" fill="#fff" />
      <rect x="15" y="20" width="18" height="1.8" fill="#1e3a8a" opacity=".2" />
      <rect x="13.5" y="24" width="21" height="5.6" rx="1.8" fill={`url(#${id}p)`} />
      <rect x="15.6" y="26.1" width="7" height="1.4" rx=".7" fill="#93c5fd" />
      <g fill="#bfdbfe">
        <rect x="13.5" y="32.3" width="4.4" height="2.6" rx=".9" />
        <rect x="19.2" y="32.3" width="4.4" height="2.6" rx=".9" />
        <rect x="24.9" y="32.3" width="4.4" height="2.6" rx=".9" />
        <rect x="13.5" y="35.9" width="4.4" height="2.6" rx=".9" />
        <rect x="19.2" y="35.9" width="4.4" height="2.6" rx=".9" />
        <rect x="24.9" y="35.9" width="4.4" height="2.6" rx=".9" />
      </g>
      <rect x="30.6" y="32.3" width="3.9" height="6.2" rx=".9" fill="#f59e0b" />
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
