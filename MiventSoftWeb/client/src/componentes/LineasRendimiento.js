// =====================================================================
//  Grafico de lineas de rendimiento mensual por vendedor (SVG puro, sin librerias).
//  El mismo dibujo se usa en pantalla (con interaccion encima) y en la impresion.
//  Para que nada se tape:
//   - cada vendedor: color + marcador de forma distinta (circulo, cuadrado, triangulo, rombo...)
//   - nombres al final de cada linea, separados verticalmente si quedan cerca (con linea guia)
//   - meses del eje X: se omiten etiquetas intermedias si no hay espacio
//   - valores sobre los puntos solo con un vendedor y si hay espacio (alternando arriba/abajo)
// =====================================================================
import { esc } from '../global/imprimir';

export const COLORES = ['#2563eb', '#e11d48', '#059669', '#d97706', '#7c3aed', '#0891b2', '#be185d', '#65a30d', '#ea580c', '#475569', '#0d9488', '#9333ea'];
const FORMAS = ['circulo', 'cuadrado', 'triangulo', 'rombo', 'tri-inv', 'circulo-h', 'cuadrado-h', 'rombo-h'];
export const MESES = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'];
export const MESES_LARGO = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
export const mesCorto = (clave) => `${MESES[Number(clave.slice(5, 7)) - 1]} ${clave.slice(2, 4)}`;
export const mesLargo = (clave) => `${MESES_LARGO[Number(clave.slice(5, 7)) - 1]} ${clave.slice(0, 4)}`;
export const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
export const corto = (n) => {
  const v = Math.abs(Number(n || 0));
  if (v >= 1e6) return `${(n / 1e6).toLocaleString('es-BO', { maximumFractionDigits: 1 })} M`;
  if (v >= 1e3) return `${(n / 1e3).toLocaleString('es-BO', { maximumFractionDigits: 1 })} k`;
  return Number(n).toLocaleString('es-BO', { maximumFractionDigits: 0 });
};
export const color = (i) => COLORES[i % COLORES.length];
// nombre corto para etiquetas: "JOEL CARLOS OQUENDO CRUZ" -> "Joel Oquendo"
export const nombreCorto = (n) => {
  const p = String(n || '').trim().toLowerCase().split(/\s+/).filter(Boolean).map((x) => x.charAt(0).toUpperCase() + x.slice(1));
  if (p.length <= 2) return p.join(' ');
  return `${p[0]} ${p.length >= 4 ? p[2] : p[1]}`;
};

function escala(max) {
  if (!(max > 0)) return { tope: 100, paso: 25 };
  const crudo = max / 5; const p = 10 ** Math.floor(Math.log10(crudo));
  const paso = [1, 2, 2.5, 5, 10].map((m) => m * p).find((x) => x >= crudo) || 10 * p;
  return { tope: Math.ceil((max * 1.04) / paso) * paso, paso };
}

/** Separa etiquetas verticalmente (minimo `gap` px) dentro de [min, max], manteniendo el orden */
export function repartir(ys, gap, min, max) {
  const it = ys.map((y, i) => ({ y, i })).sort((a, b) => a.y - b.y);
  for (let k = 1; k < it.length; k++) if (it[k].y - it[k - 1].y < gap) it[k].y = it[k - 1].y + gap;
  const exceso = it.length ? it[it.length - 1].y - max : 0;
  if (exceso > 0) it.forEach((x) => { x.y -= exceso; });
  for (let k = it.length - 2; k >= 0; k--) if (it[k + 1].y - it[k].y < gap) it[k].y = it[k + 1].y - gap;
  if (it.length && it[0].y < min) { const d = min - it[0].y; it.forEach((x) => { x.y += d; }); }
  const r = []; it.forEach((x) => { r[x.i] = x.y; });
  return r;
}

function marcador(forma, x, y, col, r = 4) {
  const h = forma.endsWith('-h'); const f = forma.replace('-h', '');
  const relleno = h ? '#fff' : col; const sw = h ? 2 : 1.5; const borde = h ? col : '#fff';
  const a = `fill="${relleno}" stroke="${borde}" stroke-width="${sw}"`;
  if (f === 'cuadrado') return `<rect x="${(x - r).toFixed(1)}" y="${(y - r).toFixed(1)}" width="${2 * r}" height="${2 * r}" ${a}/>`;
  if (f === 'triangulo') return `<path d="M${x.toFixed(1)},${(y - r - 1).toFixed(1)}L${(x + r + 0.5).toFixed(1)},${(y + r).toFixed(1)}L${(x - r - 0.5).toFixed(1)},${(y + r).toFixed(1)}Z" ${a}/>`;
  if (f === 'tri-inv') return `<path d="M${x.toFixed(1)},${(y + r + 1).toFixed(1)}L${(x + r + 0.5).toFixed(1)},${(y - r).toFixed(1)}L${(x - r - 0.5).toFixed(1)},${(y - r).toFixed(1)}Z" ${a}/>`;
  if (f === 'rombo') return `<path d="M${x.toFixed(1)},${(y - r - 1).toFixed(1)}L${(x + r + 1).toFixed(1)},${y.toFixed(1)}L${x.toFixed(1)},${(y + r + 1).toFixed(1)}L${(x - r - 1).toFixed(1)},${y.toFixed(1)}Z" ${a}/>`;
  return `<circle cx="${x.toFixed(1)}" cy="${y.toFixed(1)}" r="${r}" ${a}/>`;
}
export const forma = (i) => FORMAS[i % FORMAS.length];
export const svgMarcador = (i, tam = 12) => `<svg width="${tam}" height="${tam}" viewBox="0 0 12 12" aria-hidden="true">${marcador(forma(i), 6, 6, color(i), 4)}</svg>`;

/**
 * Geometria del grafico (la usan el SVG y la capa interactiva de la pantalla).
 * series: [{ id, nombre, valores: [{ total }] , i (indice de color) }]
 */
export function geometria({ meses, series, W = 900, H = 360, tope: topeFijo, conEtiquetas = true }) {
  const etqW = conEtiquetas ? Math.min(150, Math.max(90, W * 0.16)) : 12;
  const m = { l: 58, r: etqW, t: 16, b: 34 };
  const iw = Math.max(60, W - m.l - m.r); const ih = H - m.t - m.b;
  const max = Math.max(0, ...series.flatMap((s) => s.valores.map((v) => v.total)));
  const { tope, paso } = topeFijo ? escala(topeFijo) : escala(max);
  const n = meses.length;
  const x = (k) => m.l + (n <= 1 ? iw / 2 : (k * iw) / (n - 1));
  const y = (v) => m.t + ih - (Math.max(0, v) / tope) * ih;
  const ticks = []; for (let v = 0; v <= tope + 1e-9; v += paso) ticks.push(v);
  // etiquetas de meses: como maximo una cada ~54 px
  const cada = Math.max(1, Math.ceil(54 / Math.max(1, n <= 1 ? iw : iw / (n - 1))));
  // siempre el ultimo mes; las intermedias que quedan muy cerca del ultimo se omiten
  const visibles = meses.map((_, k) => k === n - 1 || (k % cada === 0 && n - 1 - k >= cada));
  return { m, iw, ih, W, H, x, y, tope, paso, ticks, n, visibles };
}

/** SVG del grafico de lineas (string). opciones: { foco (indice de serie resaltada), valores (mostrar valores) } */
export function svgLineas({ meses, series, W = 900, H = 360, tope, conEtiquetas = true, valores = true, titulo = '' }) {
  const g = geometria({ meses, series, W, H, tope, conEtiquetas });
  const { m, iw, ih, x, y } = g;
  let s = `<svg class="vl" viewBox="0 0 ${W} ${H}" role="img" aria-label="${esc(titulo || 'Rendimiento de ventas por mes')}">`;
  // grilla y eje Y
  s += g.ticks.map((v) => `<line x1="${m.l}" x2="${m.l + iw}" y1="${y(v).toFixed(1)}" y2="${y(v).toFixed(1)}" class="vl-grid"/><text x="${m.l - 8}" y="${(y(v) + 4).toFixed(1)}" text-anchor="end" class="vl-eje">${esc(corto(v))}</text>`).join('');
  s += `<line x1="${m.l}" x2="${m.l + iw}" y1="${m.t + ih}" y2="${m.t + ih}" class="vl-base"/>`;
  // eje X
  s += meses.map((mm, k) => (g.visibles[k] ? `<text x="${x(k).toFixed(1)}" y="${m.t + ih + 18}" text-anchor="middle" class="vl-eje">${esc(mesCorto(mm.clave))}</text>` : '')
    + `<line x1="${x(k).toFixed(1)}" x2="${x(k).toFixed(1)}" y1="${m.t + ih}" y2="${m.t + ih + 4}" class="vl-base"/>`).join('');
  // area suave cuando hay un solo vendedor
  if (series.length === 1 && g.n > 1) {
    const sr = series[0]; const c = color(sr.i ?? 0);
    s += `<path class="vl-area" d="M${x(0).toFixed(1)},${(m.t + ih).toFixed(1)}${sr.valores.map((v, k) => `L${x(k).toFixed(1)},${y(v.total).toFixed(1)}`).join('')}L${x(g.n - 1).toFixed(1)},${(m.t + ih).toFixed(1)}Z" fill="${c}" opacity="0.08"/>`;
  }
  // lineas (las de mayor total al final: quedan encima)
  const orden = series.map((sr, k) => ({ sr, k, t: sr.valores.reduce((a, v) => a + v.total, 0) })).sort((a, b) => a.t - b.t);
  orden.forEach(({ sr }) => {
    const i = sr.i ?? 0; const c = color(i);
    const d = sr.valores.map((v, k) => `${k ? 'L' : 'M'}${x(k).toFixed(1)},${y(v.total).toFixed(1)}`).join('');
    s += `<g class="vl-s" data-s="${i}"><path d="${d}" fill="none" stroke="#fff" stroke-width="5" stroke-linejoin="round" stroke-linecap="round" opacity="0.9"/>`
      + `<path d="${d}" fill="none" stroke="${c}" stroke-width="2.4" stroke-linejoin="round" stroke-linecap="round"${i >= COLORES.length ? ' stroke-dasharray="6 4"' : ''}/>`
      + sr.valores.map((v, k) => marcador(forma(i), x(k), y(v.total), c)).join('') + '</g>';
  });
  // valores sobre los puntos: solo con un vendedor y si hay espacio
  if (valores && series.length === 1 && g.n > 0 && (g.n === 1 || iw / (g.n - 1) >= 46)) {
    const sr = series[0];
    const vs = sr.valores.map((v) => v.total);
    const bot = m.t + ih - 3; const top = m.t + 9;
    const caja = (c) => { const w = c.txt.length * 6.3; const x0 = c.anc === 'middle' ? c.tx - w / 2 : c.anc === 'end' ? c.tx - w : c.tx; return { l: x0, r: x0 + w, t: c.ty - 10, b: c.ty + 2 }; };
    const choca = (p, q) => p.l < q.r && q.l < p.r && p.t < q.b && q.t < p.b;
    const puestas = [];
    vs.forEach((v, k) => {
      // ubicacion segun la pendiente para no tapar la linea: pico arriba, valle abajo, subida a la izquierda, bajada a la derecha
      const prev = k ? vs[k - 1] : v; const sig = k < vs.length - 1 ? vs[k + 1] : v;
      const px = x(k); const py = y(v); const txt = corto(v);
      const arriba = { tx: px, ty: py - 10, anc: 'middle' };
      const abajo = { tx: px, ty: py + 17, anc: 'middle' };
      const izq = { tx: px - 7, ty: py - 7, anc: 'end' };
      const der = { tx: px + 7, ty: py - 7, anc: 'start' };
      const valle = v <= prev && v <= sig && (v < prev || v < sig);
      const pico = v >= prev && v >= sig;
      let opciones;
      if (k === vs.length - 1 && vs.length > 1) opciones = [{ ...izq, ty: py - 9 }, arriba, abajo];
      else if (k === 0 && vs.length > 1) opciones = sig >= v ? [{ ...der, ty: py + 15 }, arriba, der] : [{ ...der, ty: py - 9 }, arriba, abajo];
      else if (pico) opciones = [arriba, sig > prev ? izq : der, abajo];
      else if (valle) opciones = [abajo, sig > prev ? izq : der, arriba, sig > prev ? der : izq];
      else opciones = sig > v ? [izq, arriba, abajo, der] : [der, arriba, abajo, izq];
      opciones = opciones.map((o) => ({ ...o, txt, ty: Math.max(top, Math.min(bot, o.ty)) })).filter((o) => o.ty <= bot);
      const elegida = opciones.find((o) => !puestas.some((q) => choca(caja(o), caja(q)))) || { ...arriba, txt, ty: Math.max(top, py - 22) };
      puestas.push(elegida);
    });
    s += puestas.map((c) => `<text x="${c.tx.toFixed(1)}" y="${c.ty.toFixed(1)}" text-anchor="${c.anc}" class="vl-val">${esc(c.txt)}</text>`).join('');
  }
  // nombres al final de cada linea, separados para no encimarse
  if (conEtiquetas && series.length) {
    const ult = g.n - 1;
    const ys0 = series.map((sr) => y(sr.valores[ult]?.total ?? 0));
    const ys = repartir(ys0, 15, m.t + 4, m.t + ih);
    s += series.map((sr, k) => {
      const i = sr.i ?? 0; const c = color(i); const x0 = x(ult) + 6; const xt = m.l + iw + 14;
      const guia = `<path d="M${x0.toFixed(1)},${ys0[k].toFixed(1)}L${(xt - 4).toFixed(1)},${ys[k].toFixed(1)}" stroke="${c}" stroke-width="1" fill="none" opacity="0.6"/>`;
      return `<g class="vl-s vl-etq" data-s="${i}">${guia}<text x="${xt.toFixed(1)}" y="${(ys[k] + 4).toFixed(1)}" class="vl-nom" fill="${c}">${esc(nombreCorto(sr.nombre))}</text></g>`;
    }).join('');
  }
  return s + '</svg>';
}

/** Estadisticas por vendedor: total, promedio, mejor/peor mes y tendencia (pendiente lineal relativa) */
export function estadisticas(meses, series) {
  return series.map((sr) => {
    const v = sr.valores.map((x) => x.total); const n = v.length;
    const total = v.reduce((a, b) => a + b, 0); const prom = n ? total / n : 0;
    const iMax = v.indexOf(Math.max(...v)); const iMin = v.indexOf(Math.min(...v));
    let tend = 0;
    if (n >= 2 && prom > 0) { // regresion lineal: cambio por mes sobre el promedio
      const mx = (n - 1) / 2; let num = 0; let den = 0;
      v.forEach((y, k) => { num += (k - mx) * (y - prom); den += (k - mx) ** 2; });
      tend = ((num / den) * (n - 1)) / prom; // variacion estimada en todo el periodo
    }
    const ult = n >= 2 && v[n - 2] > 0 ? (v[n - 1] - v[n - 2]) / v[n - 2] : null;
    return { ...sr, total, prom, pedidos: sr.valores.reduce((a, x) => a + x.pedidos, 0), mejor: meses[iMax]?.clave, mejorV: v[iMax] ?? 0, peor: meses[iMin]?.clave, peorV: v[iMin] ?? 0, tend, ult };
  });
}
export const tendenciaTxt = (t) => (t > 0.05 ? 'sube' : t < -0.05 ? 'baja' : 'estable');

// Estilos (pantalla e impresion)
export const CSS_LINEAS = `
.vl { display: block; width: 100%; height: auto; overflow: visible; font-family: Arial, Helvetica, sans-serif; }
.vl-grid { stroke: #e5e7eb; stroke-width: 1; }
.vl-base { stroke: #9ca3af; stroke-width: 1; }
.vl-eje { font-size: 11px; fill: #4b5563; }
.vl-val { font-size: 10.5px; fill: #111827; font-weight: 700; paint-order: stroke; stroke: #fff; stroke-width: 3px; }
.vl-nom { font-size: 11.5px; font-weight: 700; paint-order: stroke; stroke: #fff; stroke-width: 3px; }
.vl-s { transition: opacity .15s; }
`;
