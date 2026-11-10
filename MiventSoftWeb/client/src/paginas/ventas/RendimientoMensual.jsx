// =====================================================================
//  Rendimiento del personal - grafico mes a mes (btVentRendimiento)
//   - Un vendedor: su linea de ventas por mes (sube / baja), con los valores en cada punto.
//   - Varios: una linea por vendedor para comparar, y en que mes se vende mas.
//   - "Juntos" (todas las lineas) o "Separados" (un grafico por vendedor, misma escala: sin cruces).
//   - Al pasar el mouse o tocar: detalle del mes; al pasar por un vendedor se resalta su linea.
//  Datos: GET /api/ventas/reportes/rendimiento/mensual (MAM_ReporteVentas @tipo 8 por mes, solo lectura).
// =====================================================================
import { useEffect, useMemo, useRef, useState } from 'react';
import { api } from '../../api/http';
import { titulo as tit } from '../../global/programas';
import { esc } from '../../global/imprimir';
import { imprimirReporte } from '../../componentes/ReporteImpreso';
import Icono from '../../componentes/Icono';
import {
  svgLineas, geometria, estadisticas, tendenciaTxt, svgMarcador, color, bs, corto, mesCorto, mesLargo, nombreCorto, CSS_LINEAS,
} from '../../componentes/LineasRendimiento';

const pct = (t) => `${t > 0 ? '+' : ''}${Math.round(t * 100)}%`;

function useAncho() {
  const ref = useRef(null);
  const [w, setW] = useState(800);
  useEffect(() => {
    if (!ref.current) return undefined;
    const ro = new ResizeObserver(([e]) => setW(Math.max(280, Math.round(e.contentRect.width))));
    ro.observe(ref.current);
    return () => ro.disconnect();
  }, []);
  return [ref, w];
}

// Columnas: total vendido por mes (vendedores visibles); el mejor mes resaltado
function svgMeses(meses, totales, W = 760, H = 190) {
  const m = { l: 52, r: 8, t: 22, b: 26 };
  const iw = W - m.l - m.r; const ih = H - m.t - m.b;
  const max = Math.max(1, ...totales);
  const crudo = max / 4; const p = 10 ** Math.floor(Math.log10(crudo || 1));
  const paso = [1, 2, 2.5, 5, 10].map((k) => k * p).find((x) => x >= crudo) || 10 * p;
  const tope = Math.ceil((max * 1.08) / paso) * paso;
  const y = (v) => m.t + ih - (v / tope) * ih;
  const bw = iw / meses.length; const iMax = totales.indexOf(Math.max(...totales));
  const cada = Math.max(1, Math.ceil(54 / bw));
  let s = `<svg class="vl" viewBox="0 0 ${W} ${H}" role="img" aria-label="Total vendido por mes">`;
  for (let v = 0; v <= tope + 1e-9; v += paso) s += `<line x1="${m.l}" x2="${m.l + iw}" y1="${y(v).toFixed(1)}" y2="${y(v).toFixed(1)}" class="vl-grid"/><text x="${m.l - 8}" y="${(y(v) + 4).toFixed(1)}" text-anchor="end" class="vl-eje">${esc(corto(v))}</text>`;
  s += meses.map((mm, k) => {
    const x0 = m.l + k * bw + bw * 0.18; const w = bw * 0.64; const yy = y(totales[k]); const mejor = k === iMax && totales[k] > 0;
    const etq = mejor || bw >= 58 ? `<text x="${(x0 + w / 2).toFixed(1)}" y="${(yy - 6).toFixed(1)}" text-anchor="middle" class="vl-val">${esc(corto(totales[k]))}</text>` : '';
    return `<rect x="${x0.toFixed(1)}" y="${yy.toFixed(1)}" width="${w.toFixed(1)}" height="${(m.t + ih - yy).toFixed(1)}" rx="3" fill="${mejor ? '#059669' : '#93c5fd'}"><title>${esc(mesLargo(mm.clave))}: Bs ${bs(totales[k])}</title></rect>${etq}`
      + (k % cada === 0 || k === meses.length - 1 ? `<text x="${(x0 + w / 2).toFixed(1)}" y="${H - 8}" text-anchor="middle" class="vl-eje">${esc(mesCorto(mm.clave))}</text>` : '');
  }).join('');
  return s + `<line x1="${m.l}" x2="${m.l + iw}" y1="${m.t + ih}" y2="${m.t + ih}" class="vl-base"/></svg>`;
}

// Tabla vendedor x mes: el que mas vendio cada mes resaltado
function htmlMatriz(meses, series) {
  const lideres = meses.map((_, k) => { let mx = 0; let id = null; series.forEach((s) => { if (s.valores[k].total > mx) { mx = s.valores[k].total; id = s.id; } }); return id; });
  const totMes = meses.map((_, k) => series.reduce((a, s) => a + s.valores[k].total, 0));
  const iMejor = totMes.indexOf(Math.max(...totMes));
  return `<table class="rm-matriz"><thead><tr><th>Vendedor</th>${meses.map((mm, k) => `<th class="num${k === iMejor ? ' rm-mejor' : ''}">${esc(mesCorto(mm.clave))}</th>`).join('')}<th class="num">Total</th></tr></thead><tbody>`
    + series.map((s) => `<tr><td><span class="rm-nom">${svgMarcador(s.i)}${esc(tit(s.nombre))}</span></td>${s.valores.map((v, k) => `<td class="num${lideres[k] === s.id && series.length > 1 ? ' rm-lider' : ''}">${v.total ? esc(bs(v.total)) : '—'}</td>`).join('')}<td class="num"><b>${esc(bs(s.valores.reduce((a, v) => a + v.total, 0)))}</b></td></tr>`).join('')
    + `</tbody><tfoot><tr><td>Total mes</td>${totMes.map((t, k) => `<td class="num${k === iMejor ? ' rm-mejor' : ''}">${esc(bs(t))}</td>`).join('')}<td class="num">${esc(bs(totMes.reduce((a, b) => a + b, 0)))}</td></tr></tfoot></table>`;
}

export const CSS_RENDIMIENTO = `${CSS_LINEAS}
.rm-matriz { width: 100%; border-collapse: collapse; font-size: 12px; font-variant-numeric: tabular-nums; }
.rm-matriz th, .rm-matriz td { padding: 5px 8px; border-bottom: 1px solid #e5e7eb; text-align: left; white-space: nowrap; }
.rm-matriz th { font-size: 11px; color: #4b5563; font-weight: 700; background: #f9fafb; }
.rm-matriz .num { text-align: right; }
.rm-matriz tfoot td { font-weight: 700; border-top: 1.5px solid #9ca3af; background: #f9fafb; }
.rm-matriz .rm-lider { color: #047857; font-weight: 700; background: #ecfdf5; }
.rm-matriz th.rm-mejor, .rm-matriz td.rm-mejor { color: #047857; }
.rm-matriz th.rm-mejor::after { content: ' ★'; }
.rm-nom { display: inline-flex; align-items: center; gap: 6px; white-space: normal; }
.rm-nom svg { flex: none; }
`;

export default function RendimientoMensual({ desde, hasta, personal, onUltimos12, cabecera, usuario, filtrosTxt }) {
  const [datos, setDatos] = useState(null);
  const [error, setError] = useState('');
  const [cargando, setCargando] = useState(false);
  const [ocultos, setOcultos] = useState(() => new Set());
  const [vista, setVista] = useState('juntos');
  const [foco, setFoco] = useState(null);   // indice de color del vendedor resaltado
  const [hover, setHover] = useState(null); // indice de mes
  const [ref, W] = useAncho();
  const clave = `${desde}|${hasta}|${personal.join(',')}`;

  useEffect(() => {
    let vivo = true;
    setCargando(true); setError(''); setHover(null);
    api.get(`/api/ventas/reportes/rendimiento/mensual?${new URLSearchParams({ desde, hasta, personal: personal.join(',') })}`)
      .then((r) => { if (vivo) { setDatos({ ...r.data, series: r.data.series.map((s, i) => ({ ...s, i })) }); setOcultos(new Set()); if (r.data.aviso) setError(r.data.aviso); } })
      .catch((e) => { if (vivo) setError(e.message); })
      .finally(() => { if (vivo) setCargando(false); });
    return () => { vivo = false; };
  }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps

  const stats = useMemo(() => (datos ? estadisticas(datos.meses, datos.series) : []), [datos]);
  const visibles = useMemo(() => (datos ? datos.series.filter((s) => !ocultos.has(s.id)) : []), [datos, ocultos]);
  const H = W < 560 ? 280 : 360;
  const geo = useMemo(() => (datos ? geometria({ meses: datos.meses, series: visibles, W, H }) : null), [datos, visibles, W, H]);
  const svg = useMemo(() => (datos && visibles.length ? svgLineas({ meses: datos.meses, series: visibles, W, H }) : ''), [datos, visibles, W, H]);
  const totMes = useMemo(() => (datos ? datos.meses.map((_, k) => visibles.reduce((a, s) => a + s.valores[k].total, 0)) : []), [datos, visibles]);

  if (cargando && !datos) return <div className="tarjeta rm"><div className="reporte-vacio"><span className="spinner" /> Calculando ventas por mes...</div></div>;
  if (error) return <div className="tarjeta rm"><div className="reporte-vacio"><Icono n="alert" size={24} /><span>{error}</span></div></div>;
  if (!datos) return null;
  const { meses } = datos;
  const iMejor = totMes.indexOf(Math.max(...totMes));
  const alternar = (id) => setOcultos((o) => { const n = new Set(o); if (n.has(id)) n.delete(id); else if (datos.series.length - n.size > 1) n.add(id); return n; });

  // capa interactiva: mes mas cercano al puntero
  const mover = (e) => {
    if (!geo || meses.length === 0) return;
    const r = e.currentTarget.getBoundingClientRect();
    const px = (e.clientX - r.left) * (W / r.width);
    let k = 0; let dmin = Infinity;
    meses.forEach((_, j) => { const d = Math.abs(geo.x(j) - px); if (d < dmin) { dmin = d; k = j; } });
    setHover(k);
  };
  const tip = hover !== null && geo ? (() => {
    const filas = visibles.map((s) => ({ s, v: s.valores[hover].total, prev: hover ? s.valores[hover - 1].total : null })).sort((a, b) => b.v - a.v);
    const x = geo.x(hover);
    return { x, izq: x > W * 0.6, filas, total: filas.reduce((a, f) => a + f.v, 0) };
  })() : null;

  const imprimir = () => {
    const sts = stats.filter((s) => !ocultos.has(s.id));
    const kpis = `<div class="rp-kpis">${sts.map((s) => `<div><span>${svgMarcador(s.i, 10)} ${esc(tit(s.nombre))}</span><b>Bs ${esc(bs(s.total))}</b><span>Prom. Bs ${esc(bs(s.prom))}/mes · ${esc(tendenciaTxt(s.tend))} ${esc(pct(s.tend))}</span><span>Mejor: ${esc(s.mejor ? mesLargo(s.mejor) : '—')}</span></div>`).join('')}</div>`;
    const ancho = 980; // hoja horizontal
    const lineas = vista === 'separados' && visibles.length > 1
      ? `<div class="rm-sep-imp">${visibles.map((s) => `<section class="rp-graf"><h2>${esc(tit(s.nombre))}</h2>${svgLineas({ meses, series: [s], W: 480, H: 200, tope: Math.max(...visibles.flatMap((x) => x.valores.map((v) => v.total))), conEtiquetas: false })}</section>`).join('')}</div>`
      : `<section class="rp-graf"><h2>Ventas por mes${visibles.length > 1 ? ' por vendedor' : ''}</h2>${svgLineas({ meses, series: visibles, W: ancho, H: 340 })}</section>`;
    imprimirReporte({
      ...cabecera, usuario, filas: visibles, unidad: 'vendedor', titulo: 'Rendimiento del personal por mes', subtitulo: 'Expresado en Bolivianos',
      datos: filtrosTxt, sinTabla: true,
      antes: kpis + lineas + `<section class="rp-graf"><h2>¿En qué mes se vende más?</h2>${svgMeses(meses, totMes, ancho, 180)}</section>`
        + `<section class="rp-graf"><h2>Detalle por mes</h2>${htmlMatriz(meses, visibles)}</section>`,
      horizontal: true,
      css: CSS_RENDIMIENTO + `.rm-sep-imp { display: grid; grid-template-columns: 1fr 1fr; gap: 6px; } .rm-sep-imp .rp-graf { margin: 0; }
.rp-kpis span svg { vertical-align: -1px; } .rp-kpis div span:not(:first-child) { text-transform: none; letter-spacing: 0; font-size: 9px; }
.rp .rm-matriz, .rp .rm-matriz th { font-size: 9px; } .rp .rm-matriz th, .rp .rm-matriz td { padding: 3px 4px; overflow-wrap: normal; border-bottom: 1px solid #e5e7eb; }
.rp .rm-matriz td:first-child { white-space: normal; min-width: 90px; } .rp .rm-matriz .rm-lider { background: #ecfdf5 !important; }`,
    });
  };

  const topeComun = Math.max(0, ...visibles.flatMap((s) => s.valores.map((v) => v.total)));
  const anchoSep = W >= 760 ? Math.floor((W - 16) / 2) : W;

  return (
    <div className="tarjeta rm" data-foco={foco ?? undefined}>
      <style>{CSS_RENDIMIENTO + (foco !== null ? `.rm-graf .vl-s{opacity:.12}.rm-graf .vl-s[data-s="${foco}"]{opacity:1}` : '')}</style>
      <div className="rm-cab">
        <h2 className="tarjeta-tit">Rendimiento de ventas por mes <small className="sub-rango">{meses.length} {meses.length === 1 ? 'mes' : 'meses'}</small></h2>
        <div className="rm-acciones">
          {visibles.length > 1 && (
            <div className="vista-reporte rm-vista" role="tablist" aria-label="Forma del grafico">
              <button role="tab" aria-selected={vista === 'juntos'} className={vista === 'juntos' ? 'activa' : ''} onClick={() => setVista('juntos')}>Juntos</button>
              <button role="tab" aria-selected={vista === 'separados'} className={vista === 'separados' ? 'activa' : ''} onClick={() => setVista('separados')}>Separados</button>
            </div>
          )}
          <button className="btn btn-sec btn-sm" onClick={imprimir}><Icono n="list" size={15} /> Imprimir gráfico</button>
        </div>
      </div>

      {meses.length < 2 && (
        <div className="rm-aviso">
          <Icono n="info" size={18} />
          <span>El rango elegido abarca un solo mes. Para ver si las ventas suben o bajan, elija varios meses.</span>
          <button className="btn btn-pri btn-sm" onClick={onUltimos12}>Ver últimos 12 meses</button>
        </div>
      )}

      {/* tarjetas por vendedor: clic = mostrar / ocultar su linea; pasar el mouse = resaltarla */}
      <div className="rm-kpis">
        {stats.map((s) => {
          const t = tendenciaTxt(s.tend); const oculto = ocultos.has(s.id);
          return (
            <button key={s.id} type="button" className={`rm-kpi ${oculto ? 'oculto' : ''}`} style={{ '--c': color(s.i) }}
              onClick={() => { alternar(s.id); setFoco(null); }} onMouseEnter={() => !oculto && setFoco(s.i)} onMouseLeave={() => setFoco(null)} onFocus={(e) => !oculto && e.currentTarget.matches(':focus-visible') && setFoco(s.i)} onBlur={() => setFoco(null)}
              aria-pressed={!oculto} title={oculto ? 'Mostrar en el gráfico' : 'Ocultar del gráfico'}>
              <span className="rm-kpi-nom"><span dangerouslySetInnerHTML={{ __html: svgMarcador(s.i, 13) }} />{tit(s.nombre)}</span>
              <b>Bs {bs(s.total)}</b>
              <span className="rm-kpi-det">Prom. Bs {bs(s.prom)}/mes · {s.pedidos.toLocaleString('es-BO')} pedidos</span>
              {meses.length > 1 && (
                <span className="rm-kpi-pie">
                  <em className={`rm-tend ${t}`}>{t === 'sube' ? '▲' : t === 'baja' ? '▼' : '►'} {t === 'estable' ? 'Estable' : `${t === 'sube' ? 'Sube' : 'Baja'} ${pct(s.tend)}`}</em>
                  <span>Mejor: {s.mejor ? mesCorto(s.mejor) : '—'}</span>
                </span>
              )}
            </button>
          );
        })}
      </div>

      <div ref={ref} className="rm-ancho">
        {vista === 'separados' && visibles.length > 1 ? (
          <div className="rm-sep">
            {visibles.map((s) => (
              <div key={s.id} className="rm-sep-item rm-graf">
                <h3><span dangerouslySetInnerHTML={{ __html: svgMarcador(s.i, 12) }} /> {tit(s.nombre)}</h3>
                <div dangerouslySetInnerHTML={{ __html: svgLineas({ meses, series: [s], W: anchoSep, H: 220, tope: topeComun, conEtiquetas: false }) }} />
              </div>
            ))}
          </div>
        ) : (
          <div className="rm-graf rm-lienzo" onPointerMove={mover} onPointerDown={mover} onPointerLeave={(e) => e.pointerType === 'mouse' && setHover(null)}>
            <div dangerouslySetInnerHTML={{ __html: svg }} />
            {tip && (
              <>
                <div className="rm-cruz" style={{ left: `${(tip.x / W) * 100}%`, top: `${(geo.m.t / H) * 100}%`, height: `${(geo.ih / H) * 100}%` }} />
                <div className={`rm-tip ${tip.izq ? 'izq' : ''}`} style={{ left: `${(tip.x / W) * 100}%` }} role="status">
                  <b>{mesLargo(meses[hover].clave)}</b>
                  {tip.filas.map((f) => (
                    <span key={f.s.id} className="rm-tip-fila">
                      <i dangerouslySetInnerHTML={{ __html: svgMarcador(f.s.i, 11) }} />
                      <span>{nombreCorto(f.s.nombre)}</span>
                      <b>Bs {bs(f.v)}</b>
                      {f.prev !== null && f.prev > 0 && <em className={f.v >= f.prev ? 'sube' : 'baja'}>{f.v >= f.prev ? '▲' : '▼'} {pct((f.v - f.prev) / f.prev)}</em>}
                    </span>
                  ))}
                  {tip.filas.length > 1 && <span className="rm-tip-tot">Total del mes <b>Bs {bs(tip.total)}</b></span>}
                </div>
              </>
            )}
          </div>
        )}
      </div>

      {meses.length > 1 && (
        <div className="rm-bloque">
          <h3 className="rm-sub">¿En qué mes se vende más? {totMes[iMejor] > 0 && <small>Mejor mes: <b>{mesLargo(meses[iMejor].clave)}</b> · Bs {bs(totMes[iMejor])}</small>}</h3>
          <div className="rm-meses" dangerouslySetInnerHTML={{ __html: svgMeses(meses, totMes, W, W < 560 ? 170 : 190) }} />
        </div>
      )}
      <div className="rm-bloque">
        <h3 className="rm-sub">Detalle por mes {visibles.length > 1 && <small>En verde, el vendedor que más vendió cada mes</small>}</h3>
        <div className="tabla-scroll" dangerouslySetInnerHTML={{ __html: htmlMatriz(meses, visibles) }} />
      </div>
    </div>
  );
}
