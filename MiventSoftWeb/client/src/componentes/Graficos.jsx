// =====================================================================
//  Graficos SVG propios (sin librerias): linea por hora, barras de tendencia,
//  barras horizontales y barra segmentada. Todos con tooltip al pasar el mouse
//  o tocar, y responsivos (se redibujan al ancho real del contenedor).
// =====================================================================
import { useEffect, useRef, useState } from 'react';

export const fmtBs = (n, dec = 2) => `Bs ${Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: dec, maximumFractionDigits: dec })}`;
export const fmtN = (n) => Number(n || 0).toLocaleString('es-BO');
export const fmtCorto = (n) => {
  const v = Number(n || 0);
  if (Math.abs(v) >= 1e6) return `${(v / 1e6).toLocaleString('es-BO', { maximumFractionDigits: 1 })} M`;
  if (Math.abs(v) >= 1e3) return `${(v / 1e3).toLocaleString('es-BO', { maximumFractionDigits: 1 })} k`;
  return v.toLocaleString('es-BO', { maximumFractionDigits: 0 });
};

function useAncho() {
  const ref = useRef(null);
  const [ancho, setAncho] = useState(600);
  useEffect(() => {
    if (!ref.current) return undefined;
    const ro = new ResizeObserver(([e]) => setAncho(Math.max(240, Math.round(e.contentRect.width))));
    ro.observe(ref.current);
    return () => ro.disconnect();
  }, []);
  return [ref, ancho];
}

// Escala "bonita" para el eje Y
function escala(max, ticks = 4) {
  if (max <= 0) return { tope: 1, paso: 0.25 };
  const crudo = max / ticks;
  const mag = 10 ** Math.floor(Math.log10(crudo));
  const paso = [1, 2, 2.5, 5, 10].map((m) => m * mag).find((p) => p >= crudo);
  return { tope: paso * ticks, paso };
}

// Indices de etiquetas del eje X: cada N, mas la ultima/seleccionada, sin que choquen (minPx de separacion)
function etiquetasVisibles(n, xDe, cada, minPx, forzar = []) {
  const base = [];
  for (let i = 0; i < n; i += 1) if (i % cada === 0 || i === n - 1 || forzar.includes(i)) base.push(i);
  const prioridad = new Set([...forzar, n - 1]);
  const out = [];
  base.forEach((i) => {
    const prev = out[out.length - 1];
    if (prev === undefined || xDe(i) - xDe(prev) >= minPx) out.push(i);
    else if (prioridad.has(i) && !prioridad.has(prev)) { out.pop(); out.push(i); }
  });
  return out;
}

function Tooltip({ x, y, ancho, children }) {
  if (x === null) return null;
  const izq = x > ancho * 0.6;
  return (
    <div className="viz-tooltip" style={{ left: x, top: y, transform: `translate(${izq ? 'calc(-100% - 12px)' : '12px'}, -50%)` }}>{children}</div>
  );
}

/** Linea: valores por hora del dia vs promedio */
export function LineaHoras({ datos, desde = 7, hasta = 20, onElegir }) {
  const [ref, W] = useAncho();
  const [hover, setHover] = useState(null);
  const H = 250, m = { t: 16, r: 14, b: 28, l: 52 };
  const filas = datos.filter((d) => d.hora >= desde && d.hora <= hasta);
  const { tope, paso } = escala(Math.max(...filas.map((d) => Math.max(d.monto, d.promedio)), 0));
  const x = (i) => m.l + (i * (W - m.l - m.r)) / Math.max(1, filas.length - 1);
  const y = (v) => H - m.b - (v / tope) * (H - m.t - m.b);
  const linea = (k) => filas.map((d, i) => `${i ? 'L' : 'M'}${x(i).toFixed(1)},${y(d[k]).toFixed(1)}`).join('');
  const area = `${linea('monto')}L${x(filas.length - 1)},${y(0)}L${x(0)},${y(0)}Z`;
  const marcar = (e) => {
    const r = e.currentTarget.getBoundingClientRect();
    const px = (e.touches ? e.touches[0].clientX : e.clientX) - r.left;
    const i = Math.round(((px - m.l) / (W - m.l - m.r)) * (filas.length - 1));
    setHover(Math.max(0, Math.min(filas.length - 1, i)));
  };
  const max = filas.reduce((a, d, i) => (d.monto > (filas[a]?.monto ?? -1) ? i : a), 0);
  const h = hover !== null ? filas[hover] : null;
  return (
    <div className="viz" ref={ref}>
      <svg width={W} height={H} role="img" aria-label="Ventas por hora del día comparadas con el promedio" onMouseMove={marcar} onTouchStart={marcar} onTouchMove={marcar} onMouseLeave={() => setHover(null)}
        onClick={(e) => { if (!onElegir) return; marcar(e); const r = e.currentTarget.getBoundingClientRect(); const i = Math.max(0, Math.min(filas.length - 1, Math.round(((e.clientX - r.left - m.l) / (W - m.l - m.r)) * (filas.length - 1)))); if (filas[i]?.pedidos > 0) onElegir(filas[i].hora); }}
        style={{ cursor: onElegir && h?.pedidos > 0 ? 'pointer' : 'default' }}>
        <defs>
          <linearGradient id="gradHora" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0" stopColor="var(--serie-1)" stopOpacity=".22" />
            <stop offset="1" stopColor="var(--serie-1)" stopOpacity="0" />
          </linearGradient>
        </defs>
        {Array.from({ length: Math.round(tope / paso) + 1 }, (_, i) => i * paso).map((v) => (
          <g key={v}>
            <line x1={m.l} x2={W - m.r} y1={y(v)} y2={y(v)} className="viz-grid" />
            <text x={m.l - 8} y={y(v)} className="viz-eje" textAnchor="end" dominantBaseline="middle">{fmtCorto(v)}</text>
          </g>
        ))}
        {etiquetasVisibles(filas.length, x, W < 420 ? 3 : 2, 44).map((i) => { const d = filas[i]; return (
          <text key={d.hora} x={x(i)} y={H - 8} className="viz-eje" textAnchor="middle">{String(d.hora).padStart(2, '0')}:00</text>
        ); })}
        <path d={linea('promedio')} className="viz-linea-prom" />
        <path d={area} fill="url(#gradHora)" className="viz-area" />
        <path d={linea('monto')} className="viz-linea" />
        {filas[max]?.monto > 0 && (
          <g className="viz-pico">
            <circle cx={x(max)} cy={y(filas[max].monto)} r="5" />
            <text x={x(max)} y={y(filas[max].monto) - 12} textAnchor="middle">{fmtCorto(filas[max].monto)}</text>
          </g>
        )}
        {h && (
          <g>
            <line x1={x(hover)} x2={x(hover)} y1={m.t} y2={H - m.b} className="viz-cruz" />
            <circle cx={x(hover)} cy={y(h.promedio)} r="4" className="viz-punto-prom" />
            <circle cx={x(hover)} cy={y(h.monto)} r="5" className="viz-punto" />
          </g>
        )}
      </svg>
      {h && (
        <Tooltip x={x(hover)} y={y(h.monto)} ancho={W}>
          <b>{String(h.hora).padStart(2, '0')}:00 – {String(h.hora).padStart(2, '0')}:59</b>
          <span><i className="dot s1" /> Día: {fmtBs(h.monto)} · {h.pedidos} ped.</span>
          <span><i className="dot prom" /> Promedio: {fmtBs(h.promedio)}</span>
          {onElegir && h.pedidos > 0 && <small>Clic para ver los pedidos</small>}
        </Tooltip>
      )}
    </div>
  );
}

/** Barras verticales: tendencia diaria; la barra seleccionada se resalta */
export function BarrasDias({ datos, seleccion, onElegir, valor = 'monto' }) {
  const [ref, W] = useAncho();
  const [hover, setHover] = useState(null);
  const H = 220, m = { t: 14, r: 8, b: 26, l: 48 };
  const { tope, paso } = escala(Math.max(...datos.map((d) => d[valor]), 0));
  const bw = (W - m.l - m.r) / datos.length;
  const y = (v) => H - m.b - (v / tope) * (H - m.t - m.b);
  const d = hover !== null ? datos[hover] : null;
  const dia = (f) => { const [a, mm, dd] = f.split('-'); return new Date(Date.UTC(+a, +mm - 1, +dd)); };
  const iSel = datos.findIndex((x) => x.fecha === seleccion);
  const conEtiqueta = new Set(etiquetasVisibles(datos.length, (i) => m.l + i * bw + bw / 2, W < 500 ? 7 : 5, 34, iSel >= 0 ? [iSel] : []));
  return (
    <div className="viz" ref={ref}>
      <svg width={W} height={H} role="img" aria-label="Ventas por día de los últimos 30 días" onMouseLeave={() => setHover(null)}>
        {Array.from({ length: Math.round(tope / paso) + 1 }, (_, i) => i * paso).map((v) => (
          <g key={v}>
            <line x1={m.l} x2={W - m.r} y1={y(v)} y2={y(v)} className="viz-grid" />
            <text x={m.l - 8} y={y(v)} className="viz-eje" textAnchor="end" dominantBaseline="middle">{fmtCorto(v)}</text>
          </g>
        ))}
        {datos.map((x, i) => {
          const alto = Math.max(x[valor] > 0 ? 2 : 0, y(0) - y(x[valor]));
          const sel = x.fecha === seleccion;
          const f = dia(x.fecha);
          return (
            <g key={x.fecha} onMouseEnter={() => setHover(i)} onTouchStart={() => setHover(i)} onClick={() => onElegir?.(x.fecha)} style={{ cursor: onElegir ? 'pointer' : 'default' }}>
              <rect x={m.l + i * bw} y={m.t} width={bw} height={H - m.t - m.b} fill="transparent" />
              <path className={`viz-barra ${sel ? 'sel' : ''} ${hover === i ? 'hover' : ''}`} style={{ '--i': i }}
                d={alto ? `M${m.l + i * bw + 1},${y(0)}V${y(0) - alto + Math.min(4, alto)}q0,-${Math.min(4, alto)} ${Math.min(4, alto)},-${Math.min(4, alto)}H${m.l + (i + 1) * bw - 1 - Math.min(4, alto)}q${Math.min(4, alto)},0 ${Math.min(4, alto)},${Math.min(4, alto)}V${y(0)}Z` : ''} />
              {conEtiqueta.has(i) && (
                <text x={m.l + i * bw + bw / 2} y={H - 8} className={`viz-eje ${sel ? 'fuerte' : ''}`} textAnchor="middle">{f.getUTCDate()}/{f.getUTCMonth() + 1}</text>
              )}
            </g>
          );
        })}
      </svg>
      {d && (
        <Tooltip x={m.l + hover * bw + bw / 2} y={y(d[valor])} ancho={W}>
          <b>{dia(d.fecha).toLocaleDateString('es-BO', { weekday: 'short', day: 'numeric', month: 'short', timeZone: 'UTC' })}</b>
          <span>{fmtBs(d.monto)}</span>
          <span>{d.pedidos} pedidos</span>
        </Tooltip>
      )}
    </div>
  );
}

/** Barras horizontales con etiqueta y valor */
export function BarrasH({ filas, valor, etiqueta, formato = fmtBs, extra, destacar, onElegir }) {
  const max = Math.max(...filas.map((f) => f[valor]), 0) || 1;
  return (
    <ul className="barras-h">
      {filas.map((f, i) => (
        <li key={f[etiqueta] + i} className={`${destacar?.(f) ? 'destacada' : ''} ${onElegir ? 'clic' : ''}`} style={{ '--i': i }}
          {...(onElegir ? { role: 'button', tabIndex: 0, onClick: () => onElegir(f), onKeyDown: (e) => (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), onElegir(f)), title: 'Ver detalle' } : {})}>
          <div className="barras-h-txt"><span title={f[etiqueta]}>{f[etiqueta]}</span><b>{formato(f[valor])}</b></div>
          <div className="barras-h-pista"><div className="barras-h-barra" style={{ width: `${(f[valor] / max) * 100}%` }} /></div>
          {extra && <div className="barras-h-extra">{extra(f)}</div>}
        </li>
      ))}
    </ul>
  );
}

/** Barra segmentada (partes de un total) con leyenda */
export function BarraSegmentada({ partes, valor = 'pedidos', formato = fmtN, onElegir }) {
  const total = partes.reduce((s, p) => s + p[valor], 0) || 1;
  return (
    <div className="seg">
      <div className="seg-barra" role="img" aria-label={partes.map((p) => `${p.nombre}: ${formato(p[valor])}`).join(', ')}>
        {partes.map((p) => (
          <span key={p.nombre} className="seg-parte" title={`${p.nombre}: ${formato(p[valor])}`} style={{ width: `${(p[valor] / total) * 100}%`, background: p.color }} />
        ))}
      </div>
      <ul className="seg-leyenda">
        {partes.map((p) => (
          <li key={p.nombre} className={onElegir && p[valor] > 0 ? 'clic' : ''}
            {...(onElegir && p[valor] > 0 ? { role: 'button', tabIndex: 0, onClick: () => onElegir(p), onKeyDown: (e) => (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), onElegir(p)), title: 'Ver detalle' } : {})}>
            <i style={{ background: p.color }} /><span>{p.nombre}</span><b>{formato(p[valor])}</b><em>{Math.round((p[valor] / total) * 100)}%</em></li>
        ))}
      </ul>
    </div>
  );
}
