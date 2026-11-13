// =====================================================================
//  Impresion de Auditoria (vista previa comun de global/imprimir.js: Imprimir, PDF y Compartir)
//   - imprimirAuditoria: nota de UN registro (documento ELIMINADO o MODIFICADO) con toda su informacion.
//   - imprimirReporteAuditoria: reporte de los registros de la lista (filtros + busqueda), cada uno con su detalle.
//  Cada registro: sello, quien / cuando / desde donde, datos del documento (antes -> despues) y sus lineas.
// =====================================================================
import { esc, imprimirHtml } from '../../global/imprimir';
import { valorTexto, fechaTxt } from '../../componentes/TablaReporte';
import { urlImagen } from '../../api/http';

const CSS = `
body { padding: 9mm 11mm; }
.au-n { font: 10.5px/1.35 Arial, Helvetica, sans-serif; color: #111; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
.au-n * { box-sizing: border-box; }
.au-n .cab { display: grid; grid-template-columns: minmax(0, 1fr) auto; gap: 12px; align-items: center; padding-bottom: 6px; border-bottom: 2px solid #1c2f8f; }
.au-n .emp { font-size: 9.5px; color: #374151; }
.au-n .emp b { display: block; font-size: 12px; color: #111; text-transform: uppercase; }
.au-n .logo img { display: block; max-height: 46px; max-width: 170px; object-fit: contain; }
.au-n .rep-tit { margin: 10px 0 4px; font-size: 17px; color: #1c2f8f; }
.au-n .barra { display: flex; flex-wrap: wrap; gap: 2px 18px; margin: 0 0 10px; padding: 5px 9px; background: #f1f3f5; border: 1px solid #d1d5db; border-radius: 4px; font-size: 9.5px; }
.au-n .barra b { margin-right: 4px; }
.au-n .reg + .reg { margin-top: 12px; padding-top: 10px; border-top: 2px dashed #9ca3af; }
.au-n .tit { display: grid; grid-template-columns: minmax(0, 1fr) auto; gap: 12px; align-items: center; margin: 10px 0 8px; break-after: avoid; }
.au-n .reg .tit { margin-top: 0; }
.au-n h1 { margin: 0; font-size: 17px; color: #1c2f8f; }
.au-n .reg h1 { font-size: 14px; }
.au-n .sub { font-size: 10.5px; color: #4b5563; margin-top: 2px; }
.au-n .sello { border: 3px solid currentColor; border-radius: 6px; padding: 4px 14px; font-size: 18px; font-weight: 800; letter-spacing: 2px; transform: rotate(-4deg); text-transform: uppercase; }
.au-n .reg .sello { font-size: 12px; padding: 2px 9px; border-width: 2px; }
.au-n .sello.elim { color: #b91c1c; }
.au-n .sello.modif { color: #b45309; }
.au-n .caja { border: 1px solid #c7ccd4; border-radius: 6px; padding: 6px 10px; margin-bottom: 8px; break-inside: avoid; }
.au-n h2 { margin: 0 0 5px; font-size: 11px; color: #1c2f8f; text-transform: uppercase; letter-spacing: .4px; }
.au-n dl { display: grid; grid-template-columns: 1fr 1fr; gap: 2px 18px; margin: 0; }
.au-n dl div { display: grid; grid-template-columns: 108px minmax(0, 1fr); gap: 6px; min-width: 0; }
.au-n dt { font-weight: 700; color: #374151; }
.au-n dd { margin: 0; overflow-wrap: anywhere; }
.au-n .cambio dd { background: #fef3c7; border-radius: 3px; padding: 0 3px; }
.au-n s { color: #9ca3af; }
.au-n .flecha { color: #6b7280; padding: 0 4px; }
.au-n table { width: 100%; border-collapse: collapse; margin-top: 2px; }
.au-n th { background: #1c2f8f; color: #fff; padding: 4px 6px; text-align: left; white-space: nowrap; font-weight: 700; }
.au-n td { padding: 3px 6px; border-bottom: 1px solid #e5e7eb; vertical-align: top; }
.au-n .num { text-align: right; white-space: nowrap; font-variant-numeric: tabular-nums; }
.au-n tr { break-inside: avoid; }
.au-n tr.quitado td, .au-n tr.eliminado td { color: #991b1b; }
.au-n tr.quitado td:not(:first-child) { text-decoration: line-through; }
.au-n tr.agregado td { color: #166534; }
.au-n .est { font-size: 9px; font-weight: 700; text-transform: uppercase; }
.au-n .total { display: flex; justify-content: flex-end; gap: 14px; align-items: baseline; margin: 6px 0; font-size: 13px; }
.au-n .total b { font-size: 15px; }
.au-n .pie { margin-top: 14px; padding-top: 5px; border-top: 1px solid #c7ccd4; font-size: 9px; color: #6b7280; display: flex; justify-content: space-between; gap: 12px; }
`;

const ESTADO = { agregado: 'Agregado', quitado: 'Quitado', modificado: 'Modificado', igual: 'Sin cambio', eliminado: 'Eliminado' };
const val = (c, v) => (v === null || v === undefined || v === '' ? '—' : valorTexto(c, c.tipo === 'num' ? Number(v) : v));
const deA = (c, a, b) => `<s>${esc(val(c, a))}</s><span class="flecha">→</span><b>${esc(val(c, b))}</b>`;
const monto = (r, v) => (v == null ? '' : `${Number(r.Modulo) === 3 ? '' : 'Bs '}${valorTexto({ tipo: 'num' }, v)}`);
const par = (k, v, cambio) => `<div${cambio ? ' class="cambio"' : ''}><dt>${esc(k)}</dt><dd>${v}</dd></div>`;
const ahoraTxt = () => {
  const a = new Date(); const dos = (x) => String(x).padStart(2, '0');
  return `${dos(a.getDate())}/${dos(a.getMonth() + 1)}/${a.getFullYear()} ${dos(a.getHours())}:${dos(a.getMinutes())}`;
};

function cabeceraEmpresa(empresa, imagenEmpresa) {
  const e = empresa || {};
  const logo = imagenEmpresa ? urlImagen('empresa', imagenEmpresa) : null;
  return `<header class="cab"><div class="emp"><b>${esc(e.Nombre || '')}</b>${esc([e.Direccion, e.Ciudad].filter(Boolean).join(' · '))}${e.Telefono ? ` · Tel. ${esc(e.Telefono)}` : ''}</div>`
    + `<div class="logo">${logo ? `<img src="${esc(logo)}" alt="">` : ''}</div></header>`;
}

/** Un registro completo (d = { registro, cabecera, columnas, detalle }) */
function seccion(d, titulo) {
  const r = d.registro;
  const elim = Number(r.Operacion) === 2;
  const u = r.Ubicacion;
  const donde = [
    par('Usuario:', esc(r.Usuario || '—')),
    par(elim ? 'Eliminado el:' : 'Modificado el:', esc(fechaTxt(r.Fecha))),
    par('Origen:', esc(r.Origen === 'Web' ? `Web · ${r.Navegador || ''}` : r.Navegador || 'Escritorio')),
    par('Equipo:', esc(r.Equipo || '—')),
    par('IP:', esc([r.IpLocal && `Local ${r.IpLocal}`, r.IpPublica && `Pública ${r.IpPublica}`].filter(Boolean).join(' · ') || '—')),
    par('Ubicación:', u ? `${esc(`${u.latTexto ?? u.lat}, ${u.lngTexto ?? u.lng}`)} <small>(${u.aproximada ? 'aproximada por IP' : `GPS${u.precision != null ? ` ± ${u.precision} m` : ''}`})</small>` : '—'),
  ].join('');
  const datos = d.cabecera.map((c) => par(`${c.titulo}:`, c.cambio ? deA(c, c.antes, c.despues) : esc(val(c, c.antes ?? c.despues)), c.cambio)).join('');
  const cols = d.columnas;
  const filas = d.detalle.map((l) => {
    const base = l.despues || l.antes;
    return `<tr class="${l.estado}">${elim ? '' : `<td class="est">${esc(ESTADO[l.estado] || l.estado)}</td>`}${cols.map((c) => `<td class="${c.tipo === 'num' ? 'num' : ''}">${
      l.cambios.includes(c.campo) ? deA(c, l.antes[c.campo], l.despues[c.campo]) : esc(val(c, base[c.campo]))}</td>`).join('')}</tr>`;
  }).join('');
  const tabla = d.detalle.length
    ? `<table><thead><tr>${elim ? '' : '<th>Estado</th>'}${cols.map((c) => `<th class="${c.tipo === 'num' ? 'num' : ''}">${esc(c.titulo)}</th>`).join('')}</tr></thead><tbody>${filas}</tbody></table>`
    : '<p>Sin detalle.</p>';
  const total = r.TotalAntes == null ? '' : `<div class="total"><span>${elim ? 'Total eliminado:' : 'Total:'}</span>${
    elim || r.TotalDespues == null || Number(r.TotalAntes) === Number(r.TotalDespues) ? `<b>${esc(monto(r, elim ? r.TotalAntes : r.TotalDespues))}</b>`
      : `<s>${esc(monto(r, r.TotalAntes))}</s><span class="flecha">→</span><b>${esc(monto(r, r.TotalDespues))}</b>`}</div>`;
  return `<section class="tit"><div><h1>${esc(titulo)}</h1><div class="sub">${esc(r.Descripcion || '')}${r.FechaDocumento ? ` · Documento del ${esc(fechaTxt(r.FechaDocumento))}` : ''}</div></div>
<div class="sello ${elim ? 'elim' : 'modif'}">${elim ? 'Eliminado' : 'Modificado'}</div></section>
<section class="caja"><h2>Quién y desde dónde</h2><dl>${donde}</dl></section>
<section class="caja"><h2>${elim ? 'Datos del documento eliminado' : 'Datos del documento (antes → después)'}</h2><dl>${datos || par('', 'Sin datos')}</dl></section>
<section class="caja"><h2>Detalle${elim ? ` (${d.detalle.length} línea${d.detalle.length === 1 ? '' : 's'})` : ''}</h2>${tabla}</section>
${total}`;
}

const conLogoSeguro = (html) => html.replace(/<img /g, '<img onerror="this.remove()" ');

/** Nota de UN registro (d = respuesta de GET /auditoria/:id). */
export function imprimirAuditoria(d, usuarioImprime = '') {
  const r = d.registro;
  const elim = Number(r.Operacion) === 2;
  const doc = `${r.NombreModulo} N° ${r.DocumentoId}`;
  const cuerpo = `<article class="au-n">${cabeceraEmpresa(d.empresa, d.imagenEmpresa)}${seccion(d, `Auditoría · ${doc}`)}
<footer class="pie"><span>Registro de auditoría N° ${esc(r.Id)} · solo lectura (no se puede modificar ni borrar)</span><span>Impreso: ${esc(ahoraTxt())}${usuarioImprime ? ` · ${esc(usuarioImprime)}` : ''}</span></footer></article>`;
  return imprimirHtml({ cuerpo: conLogoSeguro(cuerpo), css: CSS, titulo: `Auditoria ${doc} ${elim ? 'eliminado' : 'modificado'}` });
}

/**
 * Reporte detallado de la lista (d = respuesta de GET /auditoria/reporte).
 * @param {{ titulo: string, filtros: Array<[string,string]>, usuario?: string }} o
 */
export function imprimirReporteAuditoria(d, { titulo, filtros = [], usuario = '' }) {
  const regs = d.registros || [];
  const elim = regs.filter((x) => Number(x.registro.Operacion) === 2);
  const totalElim = elim.filter((x) => Number(x.registro.Modulo) !== 3).reduce((s, x) => s + Number(x.registro.TotalAntes || 0), 0);
  const barra = [['Usuario:', usuario], ['Impreso:', ahoraTxt()], ...filtros,
    ['Registros:', String(regs.length)], ['Eliminados:', String(elim.length)], ['Modificados:', String(regs.length - elim.length)],
    ...(elim.length ? [['Total eliminado (ventas y compras):', `Bs ${valorTexto({ tipo: 'num' }, totalElim)}`]] : [])]
    .filter(([, v]) => v).map(([k, v]) => `<span><b>${esc(k)}</b>${esc(v)}</span>`).join('');
  const cuerpo = `<article class="au-n">${cabeceraEmpresa(d.empresa, d.imagenEmpresa)}
<h1 class="rep-tit">${esc(titulo)}</h1><div class="barra">${barra}</div>
${regs.map((x) => `<div class="reg">${seccion(x, `${x.registro.NombreModulo} N° ${x.registro.DocumentoId}`)}</div>`).join('')}
<footer class="pie"><span>Auditoría de registros · solo lectura (no se puede modificar ni borrar)</span><span>${esc(regs.length)} registro${regs.length === 1 ? '' : 's'}</span></footer></article>`;
  return imprimirHtml({ cuerpo: conLogoSeguro(cuerpo), css: CSS, titulo });
}
