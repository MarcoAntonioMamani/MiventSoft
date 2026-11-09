// =====================================================================
//  Rep_VentasRealizadas (btVentReporteRealizada): datos del SP MAM_ReporteVentas @tipo 1 / 2 / 13 / 14
//   - "Reporte Datos"  = Reporte_VentasRealizadas.rpt: agrupado por Sucursal > Personal,
//     Id cliente, Cliente, Comprobante (Id - año), Fecha venta, Total venta, con subtotales.
//   - "Grafico"        = ReporteGraficoVentasRealizadas.rpt: "Ventas Por Vendedores" (dona con el total al centro,
//     valores con lineas guia y leyenda con total) y "Ventas Por Sucursales" (barras con eje y leyenda).
//     El mismo SVG se usa en pantalla y en la impresion.
//  Rep_VentasVsCostos (btVentReporteVentaVsCosto): MAM_ReporteVentas @tipo 4 / 5, mismo esquema
//   - Datos   = VentasVsCostos.rpt: agrupado por Personal; Comprobante, Sucursal, Cliente, Fecha venta,
//     Total costo, Total venta, Utilidad.
//   - Grafico = ReporteGraficoUtilidades.rpt: "Ventas Por Personal y Utilidades" (torta) y "Utilidades Por Sucursales".
//  Reporte_ProductoMasVendido (btVentProdMasVendidos): MAM_ReporteVentas @tipo 6
//   (IdProducto, NombreProducto, Familia, SucursalId, NombreAlmacen, Cantidad, Total)
//   - Datos   = Reporte_ProductosMasVendidos.rpt: todas las filas, sin grupos.
//   - Grafico = ReporteGraficoProductosMasVendidos.rpt con los primeros N productos (tbCantidad, 10):
//     "Productos Mas Vendidos" (barras por producto, eje Cantidad / Productos, leyenda en 2 columnas) y
//     "Productos Mas Vendidos Por Sucursal" (dona por sucursal con el total al centro y leyenda con total).
//  Reporte_HistoricoCobro (btnReporteCobranza): MAM_ReporteVentas @tipo 17 (todos) / 18 (un vendedor)
//   - Datos = Reporte_Cobros.rpt: agrupado por Personal; Cod Venta, Fecha Venta, Fecha Pago, Glosa (cliente - glosa), Monto.
//     Solo el monto cobrado (el SP puede traer tambien el monto de la venta: no se repite). Sin grafico, como el escritorio.
//  Los nombres de columna se buscan sin importar mayusculas (lo que devuelva el SP).
// =====================================================================
import { esc } from '../../global/imprimir';
import { titulo as tit } from '../../global/programas';

const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
export const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const fechaDe = (x) => String(x ?? '').slice(0, 10);

// Busca la columna por nombres posibles (sin importar mayusculas)
function buscar(fila, ...nombres) {
  const ks = Object.keys(fila || {});
  for (const n of nombres) { const k = ks.find((x) => x.toLowerCase() === n.toLowerCase()); if (k) return k; }
  return null;
}

// TipoVenta: 0/1 (0 = credito, como Tec_Ventas) o texto ("CREDITO", "Contado"...)
const tipoTxt = (t) => {
  if (t === null || t === undefined || t === '') return '';
  if (typeof t === 'number' || /^\d+$/.test(String(t))) return Number(t) === 0 ? 'Crédito' : 'Contado';
  if (/^cr[eé]dito$/i.test(String(t).trim())) return 'Crédito';
  if (/^contado$/i.test(String(t).trim())) return 'Contado';
  return tit(String(t));
};

/** Normaliza las filas del SP a campos conocidos (__*) sin perder las originales */
export function normalizar(filas) {
  const f0 = filas[0] || {};
  const k = {
    id: buscar(f0, 'Id', 'VentaId', 'IdVenta', 'Comprobante'),
    fecha: buscar(f0, 'FechaVenta', 'Fecha'),
    total: buscar(f0, 'TotalVenta', 'Total', 'Monto'),
    cliId: buscar(f0, 'ClienteId', 'IdCliente', 'CodigoCliente'),
    cli: buscar(f0, 'NombreCliente', 'Cliente', 'Nombrecliente'),
    perId: buscar(f0, 'PersonalId', 'IdPersonal'),
    per: buscar(f0, 'Personal', 'NombrePersonal', 'Vendedor'),
    sucId: buscar(f0, 'SucursalId', 'IdSucursal'),
    suc: buscar(f0, 'NombreSucursal', 'Sucursal', 'DescripcionSucursal'),
    tipo: buscar(f0, 'TipoVenta', 'TVenta'),
    costo: buscar(f0, 'TotalCosto', 'Costo', 'CostoTotal'),
    util: buscar(f0, 'Utilidad', 'TotalUtilidad', 'Ganancia'),
  };
  return filas.map((f, i) => {
    const fecha = k.fecha ? fechaDe(f[k.fecha]) : '';
    const id = k.id ? f[k.id] : i + 1;
    const tipo = k.tipo ? f[k.tipo] : null;
    return {
      ...f,
      __n: i,
      __id: id,
      __comprobante: `${id}${fecha ? ` - ${fecha.slice(0, 4)}` : ''}`,
      __fecha: fecha,
      __total: num(k.total ? f[k.total] : 0),
      __costo: num(k.costo ? f[k.costo] : 0),
      // si el SP no trae Utilidad, es Total venta - Total costo
      __utilidad: k.util ? num(f[k.util]) : num(k.total ? f[k.total] : 0) - num(k.costo ? f[k.costo] : 0),
      __cliId: k.cliId ? f[k.cliId] : '',
      __cli: k.cli ? String(f[k.cli] ?? '') : '',
      __perId: k.perId ? f[k.perId] : '',
      __per: k.per ? String(f[k.per] ?? '') : '',
      __sucId: k.sucId ? f[k.sucId] : '',
      __suc: k.suc ? String(f[k.suc] ?? '') : '',
      // TipoVenta puede venir como 0/1 (0 = credito, como Tec_Ventas) o como texto
      __tipo: tipoTxt(tipo),
    };
  });
}

// Columnas en pantalla
export const COLS_PANTALLA = [
  { campo: '__comprobante', titulo: 'Comprobante' },
  { campo: '__fecha', titulo: 'Fecha venta', tipo: 'fecha' },
  { campo: '__per', titulo: 'Personal', render: (f) => tit(f.__per) },
  { campo: '__cliId', titulo: 'Id cliente' },
  { campo: '__cli', titulo: 'Cliente', ancho: 200, render: (f) => tit(f.__cli) },
  { campo: '__tipo', titulo: 'Tipo venta' },
  { campo: '__total', titulo: 'Total venta (Bs)', tipo: 'bs', total: true },
];
// Columnas del reporte impreso (las del .rpt + tipo de venta si el SP lo trae)
export const colsImpresion = (conTipo) => [
  { campo: '__cliId', titulo: 'Id cliente', nowrap: true, clase: () => 'num' },
  { campo: '__cli', titulo: 'Cliente', ancho: '42%' },
  { campo: '__comprobante', titulo: 'Comprobante', nowrap: true },
  { campo: '__fecha', titulo: 'Fecha venta', tipo: 'fecha' },
  ...(conTipo ? [{ campo: '__tipo', titulo: 'Tipo venta', nowrap: true }] : []),
  { campo: '__total', titulo: 'Total venta (Bs)', tipo: 'bs', total: true },
];
export const nivelesImpresion = (filas) => {
  const conSuc = filas.some((f) => f.__suc || f.__sucId !== '');
  return [
    ...(conSuc ? [{ clave: (f) => `${f.__sucId}|${f.__suc}`, etiqueta: (f) => [f.__sucId, f.__suc].filter((x) => x !== '').join(' · '), orden: (f) => num(f.__sucId), titulo: 'Sucursal', subtotal: 'Total sucursal' }] : []),
    { clave: (f) => `${f.__perId}|${f.__per}`, etiqueta: (f) => [f.__perId, f.__per].filter((x) => x !== '').join(' · '), orden: (f) => (f.__perId !== '' ? num(f.__perId) : f.__per), titulo: 'Personal', subtotal: 'Subtotal' },
  ];
};

/** Resumen para los graficos (pantalla e impresion) */
export function resumir(filas, desde, hasta) {
  const total = filas.reduce((a, f) => a + f.__total, 0);
  const clientes = new Set(filas.map((f) => f.__cliId !== '' ? f.__cliId : f.__cli)).size;
  const agrupar = (clave, nombre) => {
    const m = new Map();
    filas.forEach((f) => { const k = clave(f); const x = m.get(k) || { nombre: nombre(f), v: 0, n: 0 }; x.v += f.__total; x.n += 1; m.set(k, x); });
    return [...m.values()].sort((a, b) => b.v - a.v);
  };
  const vendedores = agrupar((f) => `${f.__perId}|${f.__per}`, (f) => tit(f.__per) || 'Sin personal');
  const top = agrupar((f) => `${f.__cliId}|${f.__cli}`, (f) => tit(f.__cli) || 'Sin cliente').slice(0, 10);
  const tipos = filas.some((f) => f.__tipo) ? agrupar((f) => f.__tipo || 'Sin tipo', (f) => f.__tipo || 'Sin tipo') : [];
  // todos los dias del rango (incluye los dias sin ventas) si son 2 a 62 dias
  const dias = [];
  const d0 = new Date(`${desde}T00:00:00Z`); const d1 = new Date(`${hasta}T00:00:00Z`);
  const nDias = Math.round((d1 - d0) / 864e5) + 1;
  if (nDias >= 2 && nDias <= 62) {
    const m = new Map(); filas.forEach((f) => { const x = m.get(f.__fecha) || { monto: 0, pedidos: 0 }; x.monto += f.__total; x.pedidos += 1; m.set(f.__fecha, x); });
    for (let i = 0; i < nDias; i++) { const k = new Date(d0.getTime() + i * 864e5).toISOString().slice(0, 10); dias.push({ fecha: k, ...(m.get(k) || { monto: 0, pedidos: 0 }) }); }
  }
  return { total, n: filas.length, ticket: filas.length ? total / filas.length : 0, clientes, vendedores, top, tipos, dias };
}

// ---------- Grafico (igual al ReporteGraficoVentasRealizadas.rpt) ----------
// Colores de la paleta de Crystal Reports, en el mismo orden
const PALETA = ['#3b6496', '#f0a453', '#12946b', '#e05a1e', '#a0285e', '#fcd060', '#2a7a96', '#e0745e', '#4b7a17', '#cc2a5a', '#7f6db0', '#3fa7c4', '#8c564b', '#c9b037', '#5b6770'];
const f2 = (n) => bs(n);

/** Datos de los dos graficos a partir de las filas normalizadas (vendedores y sucursales en orden alfabetico, como el .rpt) */
export function datosGrafico(filas, campo = '__total') {
  const agrupar = (clave, nombre) => {
    const m = new Map();
    filas.forEach((f) => { const k = clave(f); const x = m.get(k) || { nombre: nombre(f), v: 0 }; x.v += f[campo]; m.set(k, x); });
    return [...m.values()].sort((a, b) => a.nombre.localeCompare(b.nombre, 'es'));
  };
  return {
    total: filas.reduce((a, f) => a + f[campo], 0),
    vendedores: agrupar((f) => `${f.__perId}|${f.__per}`, (f) => f.__per || 'Sin personal'),
    sucursales: agrupar((f) => `${f.__sucId}|${f.__suc}`, (f) => tit(f.__suc) || 'Sucursal'),
  };
}

const leyenda = (items, total, conValor = true) => `<table class="vg-ley"><tbody>${items.map((x, i) => `<tr><td><i style="background:${PALETA[i % PALETA.length]}"></i></td><td>${esc(x.nombre)}</td>${conValor ? `<td class="vg-num">${f2(x.v)}</td>` : ''}</tr>`).join('')}`
  + (total !== undefined ? `<tr class="vg-ley-tot"><td></td><td>Total:</td><td class="vg-num">${f2(total)}</td></tr>` : '') + '</tbody></table>';

/** Dona "Ventas Por Vendedores" con el total al centro y valores con lineas guia */
function svgDona(items, total, { torta = false, pie = '' } = {}) {
  const W = 580, H = 330, cx = 290, cy = 172, R = 118, r = torta ? 0 : 82; // espacio a los lados para los valores
  const tot = total || 1;
  let ang = -Math.PI / 2; // desde arriba, sentido horario
  const pt = (a, rad) => [cx + rad * Math.cos(a), cy + rad * Math.sin(a)];
  const arcos = []; const etq = [];
  items.forEach((x, i) => {
    const frac = Math.max(0, x.v) / tot;
    const a0 = ang; const a1 = ang + frac * Math.PI * 2; ang = a1;
    if (frac <= 0) return;
    const col = PALETA[i % PALETA.length];
    if (frac >= 0.9999) {
      arcos.push(torta ? `<circle cx="${cx}" cy="${cy}" r="${R}" fill="${col}"/>` : `<circle cx="${cx}" cy="${cy}" r="${(R + r) / 2}" fill="none" stroke="${col}" stroke-width="${R - r}"/>`);
    } else {
      const g = a1 - a0 > Math.PI ? 1 : 0;
      const [x0, y0] = pt(a0, R); const [x1, y1] = pt(a1, R); const [x2, y2] = pt(a1, r); const [x3, y3] = pt(a0, r);
      arcos.push(torta
        ? `<path d="M${cx},${cy}L${x0.toFixed(2)},${y0.toFixed(2)}A${R},${R} 0 ${g} 1 ${x1.toFixed(2)},${y1.toFixed(2)}Z" fill="${col}" stroke="#fff" stroke-width="1"/>`
        : `<path d="M${x0.toFixed(2)},${y0.toFixed(2)}A${R},${R} 0 ${g} 1 ${x1.toFixed(2)},${y1.toFixed(2)}L${x2.toFixed(2)},${y2.toFixed(2)}A${r},${r} 0 ${g} 0 ${x3.toFixed(2)},${y3.toFixed(2)}Z" fill="${col}" stroke="#fff" stroke-width="1"/>`);
    }
    const am = (a0 + a1) / 2;
    etq.push({ am, texto: f2(x.v), der: Math.cos(am) >= 0 });
  });
  // etiquetas a cada lado, separadas para que no se encimen
  const lineas = [];
  [true, false].forEach((der) => {
    const lado = etq.filter((e) => e.der === der).map((e) => ({ ...e, y: cy + (R + 22) * Math.sin(e.am) })).sort((a, b) => a.y - b.y);
    for (let i = 1; i < lado.length; i++) if (lado[i].y - lado[i - 1].y < 14) lado[i].y = lado[i - 1].y + 14;
    for (let i = lado.length - 2; i >= 0; i--) if (lado[i + 1].y > H - 6 && lado[i + 1].y - lado[i].y < 14) lado[i].y = lado[i + 1].y - 14;
    lado.forEach((e) => {
      const [px, py] = pt(e.am, R); const y = Math.max(10, Math.min(H - 6, e.y));
      const xCodo = der ? Math.max(px + 8, cx + R * 0.55) : Math.min(px - 8, cx - R * 0.55);
      const xFin = der ? cx + R + 52 : cx - R - 52;
      lineas.push(`<polyline points="${px.toFixed(1)},${py.toFixed(1)} ${xCodo.toFixed(1)},${y.toFixed(1)} ${xFin},${y.toFixed(1)}" fill="none" stroke="#111" stroke-width="0.8"/>`
        + `<text x="${der ? xFin + 3 : xFin - 3}" y="${y + 4}" text-anchor="${der ? 'start' : 'end'}" class="vg-etq">${esc(e.texto)}</text>`);
    });
  });
  return `<svg class="vg-dona" viewBox="0 0 ${W} ${H}" role="img" aria-label="Ventas por vendedores">${arcos.join('')}${lineas.join('')}`
    + (torta ? '' : `<text x="${cx}" y="${cy + 6}" text-anchor="middle" class="vg-centro">${esc(f2(total))}</text>`)
    + (pie ? `<text x="${cx}" y="${cy + R + 16}" text-anchor="middle" class="vg-tit-eje">${esc(pie)}</text>` : '') + '</svg>';
}

/** Escala "bonita" para el eje Y (0, 4K, 8K...) */
function escala(max) {
  if (max <= 0) return { tope: 1, paso: 0.25 };
  const crudo = max / 7; const p = 10 ** Math.floor(Math.log10(crudo));
  const paso = [1, 2, 2.5, 4, 5, 10].map((m) => m * p).find((x) => x >= crudo) || 10 * p;
  return { tope: Math.ceil(max / paso) * paso, paso };
}
const kfmt = (n) => (n >= 1e6 ? `${+(n / 1e6).toFixed(1)}M` : n >= 1e3 ? `${+(n / 1e3).toFixed(1)}K` : `${+n.toFixed(2)}K`.replace(/K$/, n ? '' : 'K'));

/** Barras "Ventas Por Sucursales" con eje Totales Ventas / Sucursales */
function svgBarras(items, { ejeY = 'Totales Ventas', ejeX = 'Sucursales', miles = true } = {}) {
  const W = 560, H = 330, m = { l: 62, r: 10, t: 12, b: 46 };
  const { tope, paso } = escala(Math.max(...items.map((x) => x.v), 0));
  const ih = H - m.t - m.b; const iw = W - m.l - m.r;
  const y = (v) => m.t + ih - (v / tope) * ih;
  const ticks = []; for (let v = 0; v <= tope + 1e-9; v += paso) ticks.push(v);
  const bw = iw / Math.max(1, items.length);
  const barras = items.map((x, i) => {
    const x0 = m.l + i * bw + bw * 0.1; const w = bw * 0.8; const yy = y(Math.max(0, x.v));
    return `<rect x="${x0.toFixed(1)}" y="${yy.toFixed(1)}" width="${w.toFixed(1)}" height="${(m.t + ih - yy).toFixed(1)}" fill="${PALETA[i % PALETA.length]}"/>`
      + `<text x="${(x0 + w / 2).toFixed(1)}" y="${((yy + m.t + ih) / 2 + 4).toFixed(1)}" text-anchor="middle" class="vg-val">${esc(f2(x.v))}</text>`
      + `<text x="${(x0 + w / 2).toFixed(1)}" y="${H - m.b + 14}" text-anchor="middle" class="vg-eje">${esc(x.nombre)}</text>`;
  }).join('');
  return `<svg class="vg-barras" viewBox="0 0 ${W} ${H}" role="img" aria-label="Ventas por sucursales">`
    + ticks.map((v) => `<line x1="${m.l}" x2="${W - m.r}" y1="${y(v).toFixed(1)}" y2="${y(v).toFixed(1)}" stroke="#111" stroke-width="0.6"/><text x="${m.l - 6}" y="${(y(v) + 4).toFixed(1)}" text-anchor="end" class="vg-eje">${miles ? kfmt(v) : +v.toFixed(2)}</text>`).join('')
    + `<rect x="${m.l}" y="${m.t}" width="${iw}" height="${ih}" fill="none" stroke="#111" stroke-width="0.8"/>`
    + barras
    + `<text x="${m.l + iw / 2}" y="${H - 8}" text-anchor="middle" class="vg-tit-eje">${esc(ejeX)}</text>`
    + `<text transform="translate(14 ${m.t + ih / 2}) rotate(-90)" text-anchor="middle" class="vg-tit-eje">${esc(ejeY)}</text></svg>`;
}

/** HTML de los dos graficos (pantalla e impresion), segun el reporte (MODOS[clave].grafico) */
export function htmlGraficoVR(g, gr = MODOS['ventas-realizadas'].grafico) {
  const [a, b] = gr;
  return `<div class="vg">
<section class="vg-sec"><h2>${esc(a.titulo)}</h2>${a.sub ? `<p class="vg-sub">${esc(a.sub)}</p>` : ''}
<div class="vg-cuerpo"><div class="vg-svg">${svgDona(g.vendedores, g.total, { torta: a.torta })}</div>${leyenda(g.vendedores, g.total)}</div></section>
<section class="vg-sec"><h2>${esc(b.titulo)}</h2>
<div class="vg-cuerpo"><div class="vg-svg vg-svg-barras">${svgBarras(g.sucursales, b)}</div>${leyenda(g.sucursales, undefined, false)}</div></section>
</div>`;
}

// Estilos del grafico: los mismos en pantalla (inyectados) y en la hoja impresa
export const CSS_GRAFICO_VR = `
.vg { font-family: Arial, Helvetica, sans-serif; color: #111; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
.vg-sec { break-inside: avoid; padding: 6px 0 14px; }
.vg-sec + .vg-sec { border-top: 1px solid #e5e7eb; padding-top: 14px; }
.vg h2 { margin: 0; text-align: center; font-size: 19px; font-weight: 700; color: #111; }
.vg-sub { margin: 2px 0 0; text-align: center; font-size: 14px; color: #6b7280; }
.vg-cuerpo { display: flex; flex-wrap: wrap; align-items: center; justify-content: center; gap: 10px 22px; margin-top: 6px; }
.vg-svg { flex: 1 1 380px; max-width: 580px; min-width: 0; }
.vg-svg-barras { max-width: 560px; flex-basis: 420px; }
.vg svg { display: block; width: 100%; height: auto; }
.vg-etq { font-size: 12px; fill: #111; }
.vg-centro { font-size: 17px; font-weight: 700; fill: #111; }
.vg-val { font-size: 11px; fill: #111; }
.vg-eje { font-size: 10.5px; fill: #111; }
.vg-tit-eje { font-size: 11.5px; font-weight: 700; fill: #111; }
.vg .vg-ley { width: auto; border: 1px solid #111; border-collapse: collapse; font-size: 11px; background: #fff; flex: 0 1 auto; max-width: 100%; }
.vg .vg-ley td { padding: 2px 6px; vertical-align: middle; overflow-wrap: normal; border: 0; background: #fff !important; color: #111; }
.vg .vg-ley td:first-child { padding-right: 0; width: 14px; }
.vg .vg-ley i { display: block; width: 10px; height: 10px; }
.vg .vg-ley .vg-num { text-align: right; white-space: nowrap; font-variant-numeric: tabular-nums; padding-left: 18px; }
.vg .vg-ley-tot td { padding-top: 5px; }
.vg .vg-ley2 { display: grid; grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 8px 14px; border: 1px solid #111; padding: 8px 10px; font-size: 9.5px; max-width: 330px; flex: 0 1 330px; }
.vg .vg-ley2 div { display: flex; gap: 6px; align-items: flex-start; }
.vg .vg-ley2 i { flex: none; width: 9px; height: 9px; margin-top: 2px; }
.vg-prod { font-size: 9px; fill: #111; }
/* productos: leyendas al costado y chicas, como el .rpt (entra en una hoja) */
.vg-p .vg-cuerpo { flex-wrap: nowrap; align-items: center; gap: 12px; }
.vg-p .vg-svg-barras { flex: 1 1 460px; max-width: 640px; }
.vg-p .vg-svg { flex: 1 1 400px; max-width: 520px; }
.vg-p .vg-ley2 { flex: 0 1 250px; font-size: 8.5px; gap: 6px 10px; padding: 8px; }
.vg-p .vg-ley { flex: 0 1 290px; font-size: 8.5px; }
.vg-p .vg-ley td { padding: 1px 4px; }
@media (max-width: 700px) { .vg-p .vg-cuerpo { flex-wrap: wrap; } .vg-p .vg-ley2, .vg-p .vg-ley { flex-basis: 100%; max-width: 100%; } }
.vg-val-c { font-size: 11px; fill: #111; }
`;

// ---------- Ventas vs costos ----------
const COLS_VC_PANTALLA = [
  { campo: '__comprobante', titulo: 'Comprobante' },
  { campo: '__fecha', titulo: 'Fecha venta', tipo: 'fecha' },
  { campo: '__per', titulo: 'Personal', render: (f) => tit(f.__per) },
  { campo: '__suc', titulo: 'Sucursal', render: (f) => tit(f.__suc) },
  { campo: '__cli', titulo: 'Cliente', ancho: 200, render: (f) => tit(f.__cli) },
  { campo: '__costo', titulo: 'Total costo', tipo: 'bs', total: true },
  { campo: '__total', titulo: 'Total venta', tipo: 'bs', total: true },
  { campo: '__utilidad', titulo: 'Utilidad', tipo: 'bs', total: true },
];
const colsVcImpresion = () => [
  { campo: '__comprobante', titulo: 'Comprobante', nowrap: true },
  { campo: '__suc', titulo: 'Sucursal', valor: (f) => tit(f.__suc) },
  { campo: '__cli', titulo: 'Cliente', ancho: '34%' },
  { campo: '__fecha', titulo: 'Fecha venta', tipo: 'fecha' },
  { campo: '__costo', titulo: 'Total costo', tipo: 'bs', total: true },
  { campo: '__total', titulo: 'Total venta', tipo: 'bs', total: true },
  { campo: '__utilidad', titulo: 'Utilidad', tipo: 'bs', total: true },
];
// solo Personal (la sucursal va como columna, igual que VentasVsCostos.rpt)
const nivelesVc = (filas) => nivelesImpresion(filas).filter((n) => n.titulo === 'Personal');

// ---------- Productos mas vendidos ----------
export function normalizarProductos(filas) {
  const f0 = filas[0] || {};
  const k = {
    id: buscar(f0, 'IdProducto', 'ProductoId', 'Id'), prod: buscar(f0, 'NombreProducto', 'Producto'),
    fam: buscar(f0, 'Familia', 'Categoria'), sucId: buscar(f0, 'SucursalId', 'IdSucursal'),
    alm: buscar(f0, 'NombreAlmacen', 'Almacen', 'NombreSucursal', 'Sucursal'),
    cant: buscar(f0, 'Cantidad', 'CantidadVendida'), total: buscar(f0, 'Total', 'TotalVenta', 'Monto'),
  };
  return filas.map((f, i) => ({
    ...f, __n: i,
    __prodId: k.id ? f[k.id] : '', __prod: k.prod ? String(f[k.prod] ?? '') : '', __fam: k.fam ? String(f[k.fam] ?? '') : '',
    __sucId: k.sucId ? f[k.sucId] : '', __alm: k.alm ? String(f[k.alm] ?? '') : '',
    __cant: num(k.cant ? f[k.cant] : 0), __total: num(k.total ? f[k.total] : 0),
  }));
}
const COLS_PROD_PANTALLA = [
  { campo: '__prodId', titulo: 'Código' },
  { campo: '__prod', titulo: 'Producto', ancho: 260 },
  { campo: '__fam', titulo: 'Familia', render: (f) => tit(f.__fam) },
  { campo: '__alm', titulo: 'Almacén', render: (f) => tit(f.__alm) },
  { campo: '__cant', titulo: 'Cantidad', tipo: 'num', total: true },
  { campo: '__total', titulo: 'Total (Bs)', tipo: 'bs', total: true },
];
const colsProdImpresion = (fs) => [
  { campo: '__prodId', titulo: 'Código', nowrap: true, clase: () => 'num' },
  { campo: '__prod', titulo: 'Producto', ancho: '44%' },
  ...(fs.some((f) => f.__fam) ? [{ campo: '__fam', titulo: 'Familia', valor: (f) => tit(f.__fam) }] : []),
  { campo: '__alm', titulo: 'Almacén', valor: (f) => tit(f.__alm) },
  { campo: '__cant', titulo: 'Cantidad', tipo: 'num', total: true },
  { campo: '__total', titulo: 'Total (Bs)', tipo: 'bs', total: true },
];

// Barras verticales por producto (eje Cantidad / Productos), etiquetas inclinadas en 2 lineas como el .rpt
function svgBarrasProductos(items) {
  const W = 700, H = 430, m = { l: 58, r: 10, t: 12, b: 150 };
  const { tope, paso } = escala(Math.max(...items.map((x) => x.v), 0));
  const ih = H - m.t - m.b; const iw = W - m.l - m.r;
  const y = (v) => m.t + ih - (v / tope) * ih;
  const ticks = []; for (let v = 0; v <= tope + 1e-9; v += paso) ticks.push(v);
  const bw = iw / Math.max(1, items.length);
  const partir = (t) => { // 2 lineas de ~30 caracteres
    const ps = String(t).split(/\s+/); const l = ['', ''];
    ps.forEach((w) => { if (l[1] || (l[0] + ' ' + w).trim().length > 30) l[1] = (l[1] + ' ' + w).trim(); else l[0] = (l[0] + ' ' + w).trim(); });
    if (l[1].length > 34) l[1] = l[1].slice(0, 33) + '…';
    return l.filter(Boolean);
  };
  const barras = items.map((x, i) => {
    const x0 = m.l + i * bw + bw * 0.1; const w = bw * 0.8; const yy = y(Math.max(0, x.v)); const xc = x0 + w / 2;
    const ls = partir(x.nombre);
    return `<rect x="${x0.toFixed(1)}" y="${yy.toFixed(1)}" width="${w.toFixed(1)}" height="${(m.t + ih - yy).toFixed(1)}" fill="${PALETA[i % PALETA.length]}"/>`
      + `<text x="${xc.toFixed(1)}" y="${Math.min(m.t + ih - 4, (yy + m.t + ih) / 2 + 4).toFixed(1)}" text-anchor="middle" class="vg-val-c">${esc(f2(x.v))}</text>`
      + `<text transform="translate(${(xc + 4).toFixed(1)} ${m.t + ih + 8}) rotate(-40)" text-anchor="end" class="vg-prod">${ls.map((t, j) => `<tspan x="0" dy="${j ? 11 : 0}">${esc(t)}</tspan>`).join('')}</text>`;
  }).join('');
  return `<svg class="vg-barras" viewBox="0 0 ${W} ${H}" role="img" aria-label="Productos mas vendidos">`
    + ticks.map((v) => `<line x1="${m.l}" x2="${W - m.r}" y1="${y(v).toFixed(1)}" y2="${y(v).toFixed(1)}" stroke="#111" stroke-width="0.6"/><text x="${m.l - 6}" y="${(y(v) + 4).toFixed(1)}" text-anchor="end" class="vg-eje">${+v.toFixed(2)}</text>`).join('')
    + `<rect x="${m.l}" y="${m.t}" width="${iw}" height="${ih}" fill="none" stroke="#111" stroke-width="0.8"/>`
    + barras
    + `<text x="${m.l + iw / 2}" y="${H - 4}" text-anchor="middle" class="vg-tit-eje">Productos</text>`
    + `<text transform="translate(14 ${m.t + ih / 2}) rotate(-90)" text-anchor="middle" class="vg-tit-eje">Cantidad</text></svg>`;
}

/** Graficos de productos mas vendidos (pantalla e impresion), con los primeros N del SP */
export function htmlGraficoProductos(filas, { topN = 10 } = {}) {
  const top = filas.slice(0, Math.max(1, topN));
  // orden alfabetico (como el .rpt); el mismo color por producto en los dos graficos
  const prods = [...new Map(top.map((f) => [f.__prod, f.__prod])).keys()].sort((a, b) => a.localeCompare(b, 'es'));
  const color = new Map(prods.map((p, i) => [p, i]));
  const porProd = prods.map((p) => ({ nombre: p, v: top.filter((f) => f.__prod === p).reduce((a, f) => a + f.__cant, 0) }));
  const sucursales = [...new Map(top.map((f) => [`${f.__sucId}|${f.__alm}`, f])).values()];
  const ley2 = `<div class="vg-ley2">${porProd.map((x) => `<div><i style="background:${PALETA[color.get(x.nombre) % PALETA.length]}"></i><span>${esc(x.nombre)}</span></div>`).join('')}</div>`;
  const donas = sucursales.map((sf) => {
    const fs = top.filter((f) => `${f.__sucId}|${f.__alm}` === `${sf.__sucId}|${sf.__alm}`);
    // items en el orden de los productos para respetar los colores
    const items = prods.map((p) => ({ nombre: p, v: fs.filter((f) => f.__prod === p).reduce((a, f) => a + f.__cant, 0) }));
    const tot = items.reduce((a, x) => a + x.v, 0);
    return `<div class="vg-cuerpo"><div class="vg-svg">${svgDona(items, tot, { pie: tit(sf.__alm) })}</div>${leyenda(items, tot)}</div>`;
  }).join('');
  return `<div class="vg vg-p">
<section class="vg-sec"><h2>Productos Mas Vendidos</h2>
<div class="vg-cuerpo"><div class="vg-svg vg-svg-barras">${svgBarrasProductos(porProd)}</div>${ley2}</div></section>
<section class="vg-sec"><h2>Productos Mas Vendidos Por Sucursal</h2>${donas}</section>
</div>`;
}

// ---------- Reporte de cobranzas ----------
export function normalizarCobros(filas) {
  const f0 = filas[0] || {};
  const k = {
    venta: buscar(f0, 'VentaId', 'IdVenta', 'CodVenta', 'CodigoVenta', 'Venta', 'NroVenta'),
    fVenta: buscar(f0, 'FechaVenta'), fPago: buscar(f0, 'FechaPago', 'Fecha'),
    cli: buscar(f0, 'NombreCliente', 'Cliente', 'Nombrecliente'), glosa: buscar(f0, 'Glosa', 'Observacion', 'Detalle'),
    // monto COBRADO (no el de la venta)
    monto: buscar(f0, 'Monto', 'MontoPago', 'Pago', 'MontoCobrado', 'Abono', 'Importe'),
    perId: buscar(f0, 'PersonalId', 'IdPersonal'), per: buscar(f0, 'NombrePersonal', 'Personal', 'Vendedor'),
  };
  return filas.map((f, i) => {
    const cli = k.cli ? String(f[k.cli] ?? '').trim() : ''; const gl = k.glosa ? String(f[k.glosa] ?? '').trim() : '';
    return {
      ...f, __n: i,
      __venta: k.venta ? String(f[k.venta] ?? '').replace(/^venta0*/i, '') : '',
      __fVenta: k.fVenta ? fechaDe(f[k.fVenta]) : '', __fPago: k.fPago ? fechaDe(f[k.fPago]) : '',
      // como el .rpt: "CLIENTE - glosa" (sin repetir el cliente si la glosa ya lo trae)
      __glosa: cli && gl && !gl.toUpperCase().startsWith(cli.toUpperCase()) ? `${cli} - ${gl}` : (gl || cli),
      __total: num(k.monto ? f[k.monto] : 0),
      __perId: k.perId ? f[k.perId] : '', __per: k.per ? String(f[k.per] ?? '') : '',
    };
  });
}
const COLS_COBROS = [
  { campo: '__venta', titulo: 'Cod venta' },
  { campo: '__fVenta', titulo: 'Fecha venta', tipo: 'fecha' },
  { campo: '__fPago', titulo: 'Fecha pago', tipo: 'fecha' },
  { campo: '__per', titulo: 'Personal', render: (f) => tit(f.__per) },
  { campo: '__glosa', titulo: 'Glosa', ancho: 280 },
  { campo: '__total', titulo: 'Monto', tipo: 'bs', total: true },
];
const colsCobrosImpresion = () => [
  { campo: '__venta', titulo: 'Cod venta', nowrap: true, clase: () => 'num' },
  { campo: '__fVenta', titulo: 'Fecha venta', tipo: 'fecha' },
  { campo: '__fPago', titulo: 'Fecha pago', tipo: 'fecha' },
  { campo: '__glosa', titulo: 'Glosa', ancho: '52%' },
  { campo: '__total', titulo: 'Monto', tipo: 'bs', total: true },
];


// ---------- Ventas por categoria (FrmVentasPorCategoria) ----------
//  Detallado = ProductosXCategoriaRep.rpt   (MAM_Productos @tipo 9):  Categoria > Vendedor; FechaVenta, Cod. Venta, Producto, Cantidad, Precio, Descuento, Total
//  General   = ReporteGeneralPorCategoria.rpt (MAM_Productos @tipo 12): Vendedor; Categoria, Cantidad, total y "Total: <vendedor>"
export function normalizarCategoria(filas) {
  const f0 = filas[0] || {};
  const k = {
    cat: buscar(f0, 'NombreCategoria', 'Categoria', 'Categoría'), sub: buscar(f0, 'Subcategoria', 'NombreSubCategoria', 'SubCategoria'),
    per: buscar(f0, 'NombrePersonal', 'Personal', 'Vendedor'), perId: buscar(f0, 'PersonalId', 'IdPersonal'),
    fecha: buscar(f0, 'FechaVenta', 'Fecha'), venta: buscar(f0, 'VentaId', 'IdVenta', 'CodVenta', 'NroVenta'),
    prod: buscar(f0, 'NombreProducto', 'Producto', 'DescripcionProducto'), prodId: buscar(f0, 'ProductoId', 'CodigoExterno'),
    cant: buscar(f0, 'Cantidad', 'CantidadUnitaria'), precio: buscar(f0, 'Precio', 'PrecioVenta'),
    desc: buscar(f0, 'MontoDescuento', 'Descuento'), total: buscar(f0, 'Total', 'TotalVenta', 'Monto'),
  };
  const resumen = !k.venta && !k.prod; // @tipo 12: ya viene totalizado por categoria
  const fs = filas.map((f) => ({
    ...f, __resumen: resumen,
    __cat: k.cat ? String(f[k.cat] ?? '').trim() : '', __sub: k.sub ? String(f[k.sub] ?? '').trim() : '',
    __per: k.per ? String(f[k.per] ?? '').trim() : '', __perId: k.perId ? f[k.perId] : '',
    __fecha: k.fecha ? fechaDe(f[k.fecha]) : '', __venta: k.venta ? String(f[k.venta] ?? '') : '',
    __prod: k.prod ? String(f[k.prod] ?? '').trim() : '', __prodId: k.prodId ? f[k.prodId] : '',
    __cant: num(k.cant ? f[k.cant] : 0), __precio: num(k.precio ? f[k.precio] : 0), __desc: num(k.desc ? f[k.desc] : 0),
    __total: num(k.total ? f[k.total] : 0),
  }));
  // orden del .rpt: General = vendedor > categoria; Detallado = categoria > vendedor > fecha > venta
  const c = (x, y) => String(x).localeCompare(String(y), 'es', { numeric: true });
  fs.sort(resumen ? (x, y) => c(x.__per, y.__per) || c(x.__cat, y.__cat)
    : (x, y) => c(x.__cat, y.__cat) || c(x.__per, y.__per) || c(x.__fecha, y.__fecha) || c(x.__venta, y.__venta));
  return fs.map((f, i) => ({ ...f, __n: i }));
}
const esResumenCat = (fs) => !!fs[0]?.__resumen;
const ventaMiles = (f) => (f.__venta && /^\d+$/.test(f.__venta) ? Number(f.__venta).toLocaleString('en-US') : f.__venta); // 47,949 como el .rpt
const colsCatPantalla = (fs) => (esResumenCat(fs) ? [
  { campo: '__per', titulo: 'Vendedor', render: (f) => tit(f.__per) },
  { campo: '__cat', titulo: 'Categoría' },
  { campo: '__cant', titulo: 'Cantidad', tipo: 'num', total: true },
  { campo: '__total', titulo: 'Total', tipo: 'bs', total: true },
] : [
  { campo: '__cat', titulo: 'Categoría' },
  { campo: '__per', titulo: 'Vendedor', render: (f) => tit(f.__per) },
  { campo: '__fecha', titulo: 'Fecha venta', tipo: 'fecha' },
  { campo: '__venta', titulo: 'Cod. venta' },
  { campo: '__prod', titulo: 'Producto', ancho: 260 },
  { campo: '__cant', titulo: 'Cantidad', tipo: 'num', total: true },
  { campo: '__precio', titulo: 'Precio', tipo: 'bs' },
  { campo: '__desc', titulo: 'Descuento', tipo: 'bs', total: true },
  { campo: '__total', titulo: 'Total', tipo: 'bs', total: true },
]);
const colsCatImpresion = (fs) => (esResumenCat(fs) ? [
  { campo: '__cat', titulo: 'Categoria', ancho: '60%' },
  { campo: '__cant', titulo: 'Cantidad', tipo: 'num', total: true },
  { campo: '__total', titulo: 'Total', tipo: 'bs', total: true },
] : [
  { campo: '__fecha', titulo: 'FechaVenta', valor: (f) => f.__fecha, nowrap: true },
  { campo: '__venta', titulo: 'Cod. Venta', valor: ventaMiles, nowrap: true, clase: () => 'num' },
  { campo: '__prod', titulo: 'Producto', ancho: '40%' },
  { campo: '__cant', titulo: 'Cantidad', tipo: 'num', total: true },
  { campo: '__precio', titulo: 'Precio', tipo: 'bs' },
  { campo: '__desc', titulo: 'Descuento', tipo: 'bs', total: true },
  { campo: '__total', titulo: 'Total', tipo: 'bs', total: true },
]);
const nivelVendedorCat = (subtotal, conSub) => ({
  clave: (f) => `${f.__per}${conSub ? `|${f.__sub}` : ''}`,
  etiqueta: (f) => [tit(f.__per) || 'Sin vendedor', conSub && f.__sub && f.__sub.toUpperCase() !== f.__cat.toUpperCase() ? f.__sub : ''].filter(Boolean).join(' · '),
  orden: (f) => f.__per, titulo: 'Vendedor', subtotal,
});
const nivelesCat = (fs) => (esResumenCat(fs)
  ? [nivelVendedorCat('Total:', false)]
  : [{ clave: (f) => f.__cat, etiqueta: (f) => f.__cat || 'Sin categoría', titulo: 'Categoria', subtotal: 'Total categoria' }, nivelVendedorCat('Total', true)]);
function htmlGraficoCategoria(fs) {
  const agrupar = (clave) => { const m = new Map(); fs.forEach((f) => { const n = clave(f) || '—'; m.set(n, (m.get(n) || 0) + f.__total); }); return [...m].map(([nombre, v]) => ({ nombre, v })).sort((a, b) => b.v - a.v); };
  const cats = agrupar((f) => f.__cat); const pers = agrupar((f) => tit(f.__per)).sort((a, b) => a.nombre.localeCompare(b.nombre, 'es'));
  const tot = fs.reduce((a, f) => a + f.__total, 0);
  return `<div class="vg">
<section class="vg-sec"><h2>Ventas Por Categoria</h2><p class="vg-sub">Expresado en Bolivianos</p>
<div class="vg-cuerpo"><div class="vg-svg">${svgDona(cats, tot)}</div>${leyenda(cats, tot)}</div></section>
<section class="vg-sec"><h2>Ventas Por Vendedor</h2>
<div class="vg-cuerpo"><div class="vg-svg vg-svg-barras">${svgBarras(pers, { ejeY: 'Total Ventas', ejeX: 'Vendedores' })}</div>${leyenda(pers, undefined, false)}</div></section>
</div>`;
}

/**
 * Configuracion por reporte: columnas, grupos, titulos y graficos del escritorio.
 * resumen(fs) -> pares para la barra del reporte impreso.
 */
export const MODOS = {
  'ventas-realizadas': {
    colsPantalla: COLS_PANTALLA,
    colsImpresion: (fs) => colsImpresion(fs.some((x) => x.__tipo)),
    niveles: nivelesImpresion,
    titulo: 'Reporte de ventas realizadas', subtitulo: '',
    tituloGrafico: 'Reporte gráfico de ventas realizadas', subtituloGrafico: '',
    campoGrafico: '__total',
    resumen: (fs) => { const t = fs.reduce((a, x) => a + x.__total, 0); return [['Total vendido:', `Bs ${bs(t)}`], ['Ticket promedio:', `Bs ${bs(t / fs.length)}`]]; },
    grafico: [
      { titulo: 'Ventas Por Vendedores', sub: 'Expresado en Bolivianos', torta: false },
      { titulo: 'Ventas Por Sucursales', ejeY: 'Totales Ventas', ejeX: 'Sucursales', miles: true },
    ],
  },
  'productos-mas-vendidos': {
    normalizar: normalizarProductos,
    colsPantalla: COLS_PROD_PANTALLA,
    colsImpresion: colsProdImpresion,
    niveles: () => [],
    unidad: 'producto',
    titulo: 'Reporte de productos más vendidos', subtitulo: '',
    tituloGrafico: 'Reporte gráfico de productos más vendidos', subtituloGrafico: '',
    topN: true,
    resumen: (fs) => [['Cantidad:', fs.reduce((a, x) => a + x.__cant, 0).toLocaleString('es-BO')], ['Total:', `Bs ${bs(fs.reduce((a, x) => a + x.__total, 0))}`]],
    htmlGrafico: (fs, o) => htmlGraficoProductos(fs, o),
  },
  'historico-cobros': {
    normalizar: normalizarCobros,
    colsPantalla: COLS_COBROS,
    colsImpresion: colsCobrosImpresion,
    niveles: (fs) => nivelesImpresion(fs).filter((n) => n.titulo === 'Personal'),
    unidad: 'cobro',
    titulo: 'Reporte de cobranzas', subtitulo: 'Expresado en Bolivianos',
    sinGrafico: true,
    etiquetaDesde: 'Movimiento desde:',
    resumen: (fs) => [['Total cobrado:', `Bs ${bs(fs.reduce((a, x) => a + x.__total, 0))}`], ['Ventas cobradas:', String(new Set(fs.map((x) => x.__venta)).size)]],
  },
  'ventas-categoria': {
    normalizar: normalizarCategoria,
    colsPantalla: colsCatPantalla, // funcion: depende de si es detallado o general
    colsImpresion: colsCatImpresion,
    niveles: nivelesCat,
    unidad: 'registro',
    titulo: 'Reporte de ventas realizadas', subtitulo: '',
    subtituloFn: (fs) => (esResumenCat(fs) ? 'General por categoría' : 'Detallado por categoría'),
    tituloGrafico: 'Reporte gráfico de ventas por categoría', subtituloGrafico: 'Expresado en Bolivianos',
    resumen: (fs) => [['Cantidad:', bs(fs.reduce((a, x) => a + x.__cant, 0))], ['Total vendido:', `Bs ${bs(fs.reduce((a, x) => a + x.__total, 0))}`]],
    htmlGrafico: (fs) => htmlGraficoCategoria(fs),
  },
  'ventas-costos': {
    colsPantalla: COLS_VC_PANTALLA,
    colsImpresion: colsVcImpresion,
    niveles: nivelesVc,
    titulo: 'Reporte de ventas vs costos', subtitulo: 'Expresado en Bolivianos',
    tituloGrafico: 'Reporte gráfico de utilidades de ventas', subtituloGrafico: 'Expresado en Bolivianos',
    campoGrafico: '__utilidad',
    resumen: (fs) => {
      const v = fs.reduce((a, x) => a + x.__total, 0); const u = fs.reduce((a, x) => a + x.__utilidad, 0);
      return [['Total venta:', `Bs ${bs(v)}`], ['Utilidad:', `Bs ${bs(u)}`]];
    },
    grafico: [
      { titulo: 'Ventas Por Personal y Utilidades', torta: true },
      { titulo: 'Utilidades Por Sucursales', ejeY: 'Utilidad', ejeX: 'Sucursal', miles: false },
    ],
  },
};
// Reportes de ventas: normalizar y graficos comunes
['ventas-realizadas', 'ventas-costos'].forEach((k) => {
  const m = MODOS[k];
  m.normalizar = normalizar;
  m.unidad = 'venta';
  m.htmlGrafico = (fs) => htmlGraficoVR(datosGrafico(fs, m.campoGrafico), m.grafico);
});
