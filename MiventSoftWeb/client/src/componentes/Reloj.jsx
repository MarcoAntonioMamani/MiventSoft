// =====================================================================
//  Reloj de la barra superior: fecha, hora, minutos y segundos (hora de Bolivia).
//  Solo este componente se actualiza cada segundo (alineado al cambio de segundo);
//  el resto de la aplicacion no se vuelve a dibujar.
// =====================================================================
import { useEffect, useState } from 'react';

const ZONA = 'America/La_Paz';
const fHora = new Intl.DateTimeFormat('es-BO', { timeZone: ZONA, hour: '2-digit', minute: '2-digit', second: '2-digit', hourCycle: 'h23' });
const fFecha = new Intl.DateTimeFormat('es-BO', { timeZone: ZONA, weekday: 'short', day: 'numeric', month: 'short', year: 'numeric' });
const fCorta = new Intl.DateTimeFormat('es-BO', { timeZone: ZONA, day: '2-digit', month: '2-digit', year: 'numeric' });
const fLarga = new Intl.DateTimeFormat('es-BO', { timeZone: ZONA, weekday: 'long', day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit', second: '2-digit', hourCycle: 'h23' });

function partes(d) {
  const p = Object.fromEntries(fHora.formatToParts(d).map((x) => [x.type, x.value]));
  return { h: p.hour, m: p.minute, s: p.second };
}
const capital = (t) => t.charAt(0).toUpperCase() + t.slice(1);
const MARCAS = Array.from({ length: 12 }, (_, i) => i * 30);

export default function Reloj() {
  const [ahora, setAhora] = useState(() => new Date());
  useEffect(() => {
    let t;
    const tic = () => { setAhora(new Date()); t = setTimeout(tic, 1005 - (Date.now() % 1000)); };
    t = setTimeout(tic, 1005 - (Date.now() % 1000));
    return () => clearTimeout(t);
  }, []);
  const { h, m, s } = partes(ahora);
  const H = Number(h), M = Number(m), S = Number(s);
  const giro = (g) => `rotate(${g} 16 16)`;
  return (
    <div className="reloj" title={capital(fLarga.format(ahora))} aria-label={`Hora ${h}:${m}:${s}, ${fLarga.format(ahora)}`} role="timer">
      <svg className="reloj-ic" viewBox="0 0 32 32" aria-hidden="true">
        <circle cx="16" cy="16" r="14.6" className="reloj-cara" />
        {MARCAS.map((g) => <line key={g} x1="16" y1={g % 90 ? 3.6 : 3.2} x2="16" y2={g % 90 ? 5.2 : 6.2} className={`reloj-marca ${g % 90 ? '' : 'fuerte'}`} transform={giro(g)} />)}
        <line x1="16" y1="17.5" x2="16" y2="9.4" className="reloj-h" transform={giro((H % 12) * 30 + M * 0.5)} />
        <line x1="16" y1="18" x2="16" y2="6.4" className="reloj-m" transform={giro(M * 6 + S * 0.1)} />
        <line x1="16" y1="19.5" x2="16" y2="4.6" className="reloj-s" transform={giro(S * 6)} />
        <circle cx="16" cy="16" r="1.7" className="reloj-eje" />
      </svg>
      <span className="reloj-txt">
        <b className="reloj-hora">{h}:{m}<span className="reloj-seg">:{s}</span></b>
        <span className="reloj-fecha">{capital(fFecha.format(ahora).replace(/\./g, ''))}</span>
        <span className="reloj-fecha-corta">{fCorta.format(ahora)}</span>
      </span>
    </div>
  );
}
