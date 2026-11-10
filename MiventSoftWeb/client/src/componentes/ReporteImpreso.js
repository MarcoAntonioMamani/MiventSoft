// =====================================================================
//  ReporteImpreso: hoja de reporte para imprimir (estandar de la web, basado en los .rpt del escritorio)
//   Cabecera: empresa + titulo + logo; barra con usuario, fecha de impresion, filtros y resumen.
//   Cuerpo: una sola tabla; opcionalmente agrupada (fila de grupo + subtotal) y total general.
//   - El texto no se corta: las columnas de texto hacen salto de linea, los montos no.
//   - La cabecera y los titulos de columna se repiten en cada hoja (como el encabezado de pagina del .rpt).
//   - Filas espaciadoras en thead/tfoot dan el margen de cada hoja (el @page va sin margen).
//  columnas: [{ campo, titulo, tipo: 'texto'|'bs'|'num'|'entero'|'fecha', total, ancho, valor(f), clase(f) }]
// =====================================================================
import { esc, imprimirHtml } from '../global/imprimir';
import { valorTexto } from './TablaReporte';
import { urlImagen } from '../api/http';

const esNum = (c) => ['bs', 'num', 'entero'].includes(c.tipo);
const n = (x) => { const v = Number(x); return Number.isFinite(v) ? v : 0; };

export const CSS_REPORTE = `
.rp { font: 10px/1.3 Arial, Helvetica, sans-serif; color: #111; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
.rp * { box-sizing: border-box; }
.rp table.rp-t { width: 100%; border-collapse: collapse; }
.rp .rp-margen td { height: 8mm; padding: 0; border: 0; background: none; }
.rp th, .rp td { text-align: left; }
.rp .rp-cab { display: grid; grid-template-columns: minmax(0, 1fr) auto; gap: 12px; align-items: center; }
.rp .rp-emp { font-size: 9.5px; color: #374151; }
.rp .rp-emp b { display: block; font-size: 11px; color: #111; text-transform: uppercase; }
.rp h1 { margin: 2px 0 0; font-size: 15px; line-height: 1.2; color: #1c2f8f; }
.rp .rp-subt { font-size: 10.5px; color: #374151; }
.rp .rp-logo img { display: block; max-height: 42px; max-width: 160px; object-fit: contain; }
.rp .rp-barra { display: flex; flex-wrap: wrap; gap: 2px 18px; margin: 5px 0 4px; padding: 4px 8px; background: #f1f3f5; border: 1px solid #d1d5db; border-radius: 4px; font-size: 9.5px; }
.rp .rp-barra b { font-weight: 700; margin-right: 4px; }
.rp .rp-barra .rp-der { margin-left: auto; }
.rp .rp-col th { padding: 4px 5px; background: #1c2f8f; color: #fff; border-top: 1px solid #14226b; border-bottom: 1px solid #14226b; font-weight: 700; white-space: nowrap; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
.rp td { padding: 2px 5px; border-bottom: 1px solid #e5e7eb; vertical-align: top; overflow-wrap: anywhere; }
.rp .num { text-align: right; white-space: nowrap; font-variant-numeric: tabular-nums; }
.rp .nw { white-space: nowrap; }
.rp .rp-t > tbody > tr:nth-child(even) > td { background: #fafafa; }
.rp tr { break-inside: avoid; }
.rp tr.rp-grupo td { background: #fff !important; padding: 7px 5px 3px; border-bottom: 1.5px solid #111; font-size: 11px; break-after: avoid; }
.rp tr.rp-grupo td small { font-size: 9.5px; color: #4b5563; margin-left: 8px; font-weight: 400; }
.rp tr.rp-sub td { background: #f3f4f6 !important; font-weight: 700; border-top: 1px solid #9ca3af; border-bottom: 0; break-before: avoid; }
.rp tr.rp-total td { background: #e3e8fb !important; color: #14226b; font-weight: 700; font-size: 11px; border-top: 1.5px solid #111; border-bottom: 1.5px solid #111; padding: 4px 5px; }
.rp tr.rp-espacio td { height: 4px; padding: 0; border: 0; background: #fff !important; }
.rp .rp-alerta { color: #b91c1c; font-weight: 700; }
.rp .rp-vacio { text-align: center; padding: 20px; color: #6b7280; }
.rp tr.rp-grupo.rp-n0 td { font-size: 12px; border-bottom: 2px solid #111; }
.rp tr.rp-grupo.rp-n1 td { padding-left: 12px; font-size: 11px; border-bottom: 1px solid #6b7280; }
.rp tr.rp-sub.rp-n0 td { background: #e5e7eb !important; border-top: 1.5px solid #111; }
.rp tr.rp-sub.rp-n1 td { padding-left: 12px; }
.rp tr.rp-antes > td { background: #fff !important; border: 0; padding: 0 0 6px; }
.rp .rp-kpis { display: grid; grid-template-columns: repeat(auto-fit, minmax(110px, 1fr)); gap: 6px; margin: 2px 0 8px; }
.rp .rp-kpis div { border: 1px solid #d1d5db; border-radius: 6px; padding: 5px 8px; }
.rp .rp-kpis span { display: block; font-size: 9px; color: #4b5563; text-transform: uppercase; letter-spacing: .3px; }
.rp .rp-kpis b { font-size: 13px; font-variant-numeric: tabular-nums; }
.rp .rp-graf { border: 1px solid #d1d5db; border-radius: 6px; padding: 6px 10px 8px; margin-bottom: 8px; break-inside: avoid; }
.rp .rp-graf h2 { margin: 0 0 6px; font-size: 11.5px; }
.rp .rp-bar { display: grid; grid-template-columns: minmax(90px, 32%) minmax(0, 1fr) 118px; align-items: center; gap: 8px; padding: 2px 0; }
.rp .rp-bar-n { overflow-wrap: anywhere; }
.rp .rp-bar-c { height: 11px; background: #f3f4f6; border-radius: 3px; overflow: hidden; }
.rp .rp-bar-c i { display: block; height: 100%; border-radius: 3px; }
.rp .rp-bar-v { text-align: right; font-weight: 700; white-space: nowrap; font-variant-numeric: tabular-nums; }
.rp .rp-bar-v small { font-weight: 400; color: #6b7280; margin-left: 6px; display: inline-block; min-width: 26px; }
.rp .rp-cols { display: flex; align-items: flex-end; gap: 3px; height: 150px; }
.rp .rp-colu { flex: 1 1 0; min-width: 0; display: flex; flex-direction: column; align-items: center; height: 100%; }
.rp .rp-colu-v { font-size: 7.5px; color: #374151; white-space: nowrap; transform: rotate(-60deg); transform-origin: center; height: 26px; display: flex; align-items: center; }
.rp .rp-colu-c { flex: 1; width: 78%; display: flex; align-items: flex-end; background: #f9fafb; }
.rp .rp-colu-c i { display: block; width: 100%; border-radius: 2px 2px 0 0; }
.rp .rp-colu-n { font-size: 7.5px; color: #374151; margin-top: 2px; white-space: nowrap; }
`;

function celda(c, f) {
  const v = c.valor ? c.valor(f) : valorTexto(c, f[c.campo]);
  const cl = [esNum(c) ? 'num' : '', c.tipo === 'fecha' || c.nowrap ? 'nw' : '', c.clase ? c.clase(f) : ''].filter(Boolean).join(' ');
  return `<td${cl ? ` class="${cl}"` : ''}>${esc(v)}</td>`;
}

function filaTotales(columnas, filas, etiqueta, clase) {
  const tot = (c) => filas.reduce((s, f) => s + n(f[c.campo]), 0);
  const primera = columnas.findIndex((c) => c.total);
  const span = primera > 0 ? primera : 1;
  const celdas = columnas.slice(span).map((c) => (c.total ? `<td class="num">${esc(valorTexto(c, tot(c)))}</td>` : '<td></td>')).join('');
  return `<tr class="${clase}"><td colspan="${span}">${esc(etiqueta)}</td>${celdas}</tr>`;
}

/**
 * Arma el HTML del reporte.
 * @param {object} o
 * @param {string} o.titulo
 * @param {string} [o.subtitulo]  ej. 'Expresado en Bolivianos'
 * @param {{ Nombre, Direccion, Telefono, Ciudad }} [o.empresa]
 * @param {string} [o.imagenEmpresa]  nombre de la imagen de la empresa (como la nota de venta)
 * @param {string} [o.usuario]
 * @param {Array<[string,string]>} [o.datos]   pares etiqueta/valor para la barra (filtros, resumen)
 * @param {Array} o.columnas
 * @param {Array} o.filas
 * @param {Array<{ clave:(f)=>any, etiqueta?:(f)=>string, titulo:string, orden?:(f)=>any, subtotal?:string }>} [o.niveles]
 *        grupos anidados (ej. Sucursal > Personal), cada uno con su subtotal
 * @param {(f)=>string} [o.agruparPor]  atajo para un solo nivel (ej. preventista) con o.tituloGrupo
 * @param {string} [o.unidad]           ej. 'crédito' -> "4 créditos"
 * @param {string} [o.antes]            HTML (ya escapado) antes de la tabla, ej. graficos/resumen
 * @param {boolean} [o.sinTabla]        solo cabecera + o.antes (reporte grafico)
 */
export function htmlReporte({ titulo, subtitulo = '', empresa, imagenEmpresa, usuario, datos = [], columnas = [], filas = [], niveles, agruparPor, tituloGrupo = '', unidad = 'registro', antes = '', sinTabla = false, totalGeneral = true, despues = '', cuerpoHtml = '' }) {
  const nc = Math.max(1, sinTabla ? 1 : columnas.length);
  const plural = (k) => `${k} ${unidad}${k === 1 ? '' : (/[aeiouáéíóú]$/i.test(unidad) ? 's' : 'es')}`;
  const ahora = new Date();
  const dos = (x) => String(x).padStart(2, '0');
  const fechaImp = `${dos(ahora.getDate())}/${dos(ahora.getMonth() + 1)}/${ahora.getFullYear()} ${dos(ahora.getHours())}:${dos(ahora.getMinutes())}`;
  const urlLogo = imagenEmpresa ? urlImagen('empresa', imagenEmpresa) : null; // null para Default.jpg
  const logo = urlLogo ? `<div class="rp-logo"><img src="${esc(urlLogo)}" alt="" onerror="this.remove()"></div>` : '<div></div>';
  const emp = empresa ? `<div class="rp-emp"><b>${esc(empresa.Nombre)}</b>${esc([empresa.Direccion, empresa.Ciudad].filter(Boolean).join(' · '))}${empresa.Telefono ? ` · Tel. ${esc(empresa.Telefono)}` : ''}</div>` : '';
  const barra = [['Usuario:', usuario || ''], ['Impreso:', fechaImp], ...datos].map(([k, v]) => `<span><b>${esc(k)}</b>${esc(v)}</span>`).join('')
    + `<span class="rp-der"><b>Total:</b>${esc(plural(filas.length))}</span>`;

  const lv = niveles || (agruparPor ? [{ clave: agruparPor, titulo: tituloGrupo }] : []);
  const filasHtml = (fs) => fs.map((f) => `<tr>${columnas.map((c) => celda(c, f)).join('')}</tr>`).join('');
  // grupos anidados: encabezado, filas (o subgrupos) y subtotal de cada nivel
  const grupos = (fs, i) => {
    if (i >= lv.length) return filasHtml(fs);
    const L = lv[i];
    const m = new Map();
    fs.forEach((f) => { const k = String(L.clave(f) ?? '').trim() || '(Sin asignar)'; if (!m.has(k)) m.set(k, []); m.get(k).push(f); });
    const ord = L.orden || ((f) => String(L.etiqueta ? L.etiqueta(f) : L.clave(f) ?? ''));
    const cmp = (a, b) => { const x = ord(m.get(a)[0]); const y = ord(m.get(b)[0]); return typeof x === 'number' && typeof y === 'number' ? x - y : String(x).localeCompare(String(y), 'es', { numeric: true }); };
    return [...m.keys()].sort(cmp).map((k, j) => {
      const g = m.get(k);
      const nombre = L.etiqueta ? L.etiqueta(g[0]) : k;
      return `${j ? `<tr class="rp-espacio"><td colspan="${nc}"></td></tr>` : ''}<tr class="rp-grupo rp-n${i}"><td colspan="${nc}"><b>${esc(L.titulo)}: ${esc(nombre)}</b><small>${esc(plural(g.length))}</small></td></tr>`
        + grupos(g, i + 1)
        + filaTotales(columnas, g, `${L.subtotal || 'Subtotal'} ${nombre}`, `rp-sub rp-n${i}`);
    }).join('');
  };

  let cuerpo = '';
  if (sinTabla) cuerpo = '';
  else if (cuerpoHtml) cuerpo = cuerpoHtml; // filas ya armadas (agrupaciones compactas propias del reporte)
  else if (!filas.length) cuerpo = `<tr><td colspan="${nc}" class="rp-vacio">Sin registros</td></tr>`;
  else if (lv.length) {
    // totalGeneral = false: sin TOTAL GENERAL (ej. ingresos y egresos no se suman entre si)
    cuerpo = grupos(filas, 0) + (totalGeneral ? `<tr class="rp-espacio"><td colspan="${nc}"></td></tr>` + filaTotales(columnas, filas, `TOTAL GENERAL (${plural(filas.length)})`, 'rp-total') : '');
  } else {
    cuerpo = filasHtml(filas);
    if (columnas.some((c) => c.total)) cuerpo += filaTotales(columnas, filas, `TOTAL (${plural(filas.length)})`, 'rp-total');
  }
  if (despues) cuerpo += `<tr class="rp-antes"><td colspan="${nc}">${despues}</td></tr>`; // HTML ya escapado al final (ej. balance)
  if (antes) cuerpo =`<tr class="rp-antes"><td colspan="${nc}">${antes}</td></tr>` + cuerpo;

  const anchos = sinTabla ? '' : `<colgroup>${columnas.map((c) => `<col${c.ancho ? ` style="width:${c.ancho}"` : ''}>`).join('')}</colgroup>`;
  const titulosCol = sinTabla || !filas.length && antes ? '' : `<tr class="rp-col">${columnas.map((c) => `<th${esNum(c) ? ' class="num"' : ''}>${esc(c.titulo)}</th>`).join('')}</tr>`;
  return `<div class="rp"><table class="rp-t">${anchos}
<thead>
<tr class="rp-margen"><td colspan="${nc}"></td></tr>
<tr><th colspan="${nc}" style="padding:0;font-weight:400;background:none;border:0"><div class="rp-cab"><div>${emp}<h1>${esc(titulo)}</h1>${subtitulo ? `<div class="rp-subt">${esc(subtitulo)}</div>` : ''}</div>${logo}</div><div class="rp-barra">${barra}</div></th></tr>
${titulosCol}
</thead>
<tfoot><tr class="rp-margen"><td colspan="${nc}"></td></tr></tfoot>
<tbody>${cuerpo}</tbody></table></div>`;
}

// ---------- graficos para la hoja impresa (HTML/CSS puro: se imprime en color y nitido) ----------
/** Barras horizontales: [{ nombre, v, extra }] */
export function htmlBarras(titulo, filas, formato, { color = '#2563eb' } = {}) {
  const max = Math.max(...filas.map((f) => Number(f.v) || 0), 0) || 1;
  const total = filas.reduce((a, f) => a + (Number(f.v) || 0), 0) || 1;
  return `<section class="rp-graf"><h2>${esc(titulo)}</h2>${filas.map((f) => `<div class="rp-bar"><span class="rp-bar-n">${esc(f.nombre)}</span>`
    + `<span class="rp-bar-c"><i style="width:${Math.max(0.5, (Number(f.v) / max) * 100).toFixed(2)}%;background:${color}"></i></span>`
    + `<span class="rp-bar-v">${esc(formato(f.v))}<small>${esc(f.extra ?? `${Math.round((Number(f.v) / total) * 100)}%`)}</small></span></div>`).join('')}</section>`;
}
/** Columnas verticales (ej. por dia): [{ nombre, v }] */
export function htmlColumnas(titulo, filas, formato, { color = '#0d9488' } = {}) {
  const max = Math.max(...filas.map((f) => Number(f.v) || 0), 0) || 1;
  return `<section class="rp-graf"><h2>${esc(titulo)}</h2><div class="rp-cols">${filas.map((f) => `<div class="rp-colu" title="${esc(formato(f.v))}">`
    + `<span class="rp-colu-v">${esc(formato(f.v))}</span><span class="rp-colu-c"><i style="height:${Math.max(1, (Number(f.v) / max) * 100).toFixed(2)}%;background:${color}"></i></span>`
    + `<span class="rp-colu-n">${esc(f.nombre)}</span></div>`).join('')}</div></section>`;
}
/** Tarjetas de indicadores: [[etiqueta, valor]] */
export function htmlIndicadores(items) {
  return `<div class="rp-kpis">${items.map(([k, v]) => `<div><span>${esc(k)}</span><b>${esc(v)}</b></div>`).join('')}</div>`;
}

// Margen lateral de la hoja: la tabla va dentro de un body con padding lateral; arriba/abajo lo dan las filas rp-margen
const CSS_HOJA = 'body { padding: 0 9mm; }';

export function imprimirReporte(opciones) {
  return imprimirHtml({ cuerpo: htmlReporte(opciones), css: CSS_HOJA + CSS_REPORTE + (opciones.css || ''), horizontal: opciones.horizontal, titulo: opciones.titulo, alCerrar: opciones.alCerrar });
}
