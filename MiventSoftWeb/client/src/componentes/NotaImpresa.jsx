// =====================================================================
//  Hoja de nota imprimible comun (Nota de venta, Proforma, Nota de compra...): mismo diseno que Recibo.rpt
//  del escritorio, en una vista previa con Imprimir / Cerrar (o "No imprimir" al ofrecerla despues de grabar).
//  Cada programa solo arma sus datos: cabecera, columnas, filas, pie y totales.
// =====================================================================
import { useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { urlImagen } from '../api/http';
import { imprimirHtml } from '../global/imprimir';
import { montoLiteral } from '../global/literal';
import { titulo as tit } from '../global/programas';
import Icono from './Icono';

// ---------- P_GenerarReporte: Nota de venta imprimible (mismo esquema que Recibo.rpt del escritorio) ----------
// Formato compacto como el escritorio: ocupa ~media hoja carta y se imprime desde el borde superior.
// Se imprime en un iframe aparte con solo la hoja, asi no quedan espacios en blanco de la pagina detras.
export const CSS_NOTA = `
.nota-papel { --nv-borde: #1f2937; --nv-gris: #f1f3f5; box-sizing: border-box; width: 100%; background: #fff; color: #111;
  font: 11px/1.3 Arial, Helvetica, sans-serif; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
.nota-papel * { box-sizing: border-box; }
.nota-papel table { font: inherit; }
.nota-papel .nv-titulo { background: var(--nv-gris); text-align: center; padding: 5px 8px 4px; margin-bottom: 6px; }
.nota-papel .nv-titulo h1 { margin: 0; font-size: 16px; font-weight: 700; line-height: 1.2; color: #1c2f8f; text-transform: uppercase; }
.nota-papel .nv-titulo b { display: block; font-size: 14px; letter-spacing: 1px; font-variant-numeric: tabular-nums; }
.nota-papel .nv-cab { display: grid; grid-template-columns: minmax(0, 1fr) minmax(0, 1fr); gap: 16px; align-items: stretch; margin-bottom: 6px; }
.nota-papel .nv-empresa { display: flex; flex-direction: column; gap: 1px; border: 1px solid var(--nv-borde); padding: 4px 8px; }
.nota-papel .nv-empresa b { font-size: 12px; margin-bottom: 2px; text-transform: uppercase; }
.nota-papel .nv-empresa small { font-size: 10px; }
.nota-papel .nv-logo { display: flex; align-items: center; justify-content: center; }
.nota-papel .nv-logo img { max-width: 100%; max-height: 70px; object-fit: contain; }
.nota-papel .nv-caja { border: 1px solid #b8bec6; border-radius: 6px; padding: 4px 8px; }
.nota-papel .nv-cliente { margin-bottom: 8px; }
.nota-papel .nv-cliente dl { display: grid; grid-template-columns: minmax(0, 1.6fr) minmax(0, 1fr); gap: 1px 16px; margin: 0; }
.nota-papel .nv-caja dl div { display: grid; grid-template-columns: minmax(92px, max-content) minmax(0, 1fr); gap: 4px 6px; min-width: 0; }
.nota-papel .nv-cliente dl div:nth-child(even) { grid-template-columns: 118px minmax(0, 1fr); }
.nota-papel .nv-cliente dl div:nth-child(even) dt { text-align: right; }
.nota-papel .nv-caja dt { font-weight: 700; }
.nota-papel .nv-caja dd { margin: 0; overflow-wrap: anywhere; }
.nota-papel .nota-tabla { width: 100%; border-collapse: collapse; border: 1px solid var(--nv-borde); margin-bottom: 6px; }
.nota-papel .nota-tabla thead th { background: #1c2f8f; color: #fff; font-weight: 700; padding: 4px 6px; border-bottom: 1px solid #14226b; text-align: left; white-space: nowrap; }
.nota-papel .nota-tabla { border-color: #14226b; }
.nota-papel .nv-caja dt { color: #1c2f8f; }
.nota-papel .nota-total span { color: #1c2f8f; }
.nota-papel .nota-tabla td { padding: 2px 6px; border-bottom: 1px solid #9ca3af; vertical-align: top; }
.nota-papel .nota-tabla tbody tr:last-child td { border-bottom: 0; }
.nota-papel .nota-tabla .num { text-align: right; font-variant-numeric: tabular-nums; }
.nota-papel .nota-tabla tr { break-inside: avoid; }
.nota-papel .nv-pie { display: grid; grid-template-columns: minmax(0, 1fr) minmax(190px, 250px); gap: 2px 20px; background: var(--nv-gris); break-inside: avoid; }
.nota-papel .nv-pie-izq { margin: 0; display: flex; flex-direction: column; gap: 1px; }
.nota-papel .nv-pie-izq div { grid-template-columns: 66px minmax(0, 1fr); }
.nota-papel .nota-totales { display: flex; flex-direction: column; gap: 1px; }
.nota-papel .nota-totales div { display: flex; justify-content: space-between; gap: 10px; font-variant-numeric: tabular-nums; }
.nota-papel .nota-totales span { font-weight: 700; }
.nota-papel .nota-totales b { font-weight: 400; }
.nota-papel .nota-total { margin-top: 3px; padding-top: 3px; border-top: 1px solid var(--nv-borde); font-size: 13px; }
.nota-papel .nota-total b { font-weight: 700; }
.nota-papel .nota-literal { grid-column: 1 / -1; margin: 1px 0 0; font-size: 10px; text-align: right; }
.nota-papel .nota-literal span { color: #555; margin-right: 6px; }
.nota-papel .nv-firmas { display: grid; grid-template-columns: repeat(auto-fit, minmax(0, 1fr)); gap: 40px; margin-top: 46px; break-inside: avoid; }
.nota-papel .nv-firmas div { display: flex; flex-direction: column; align-items: center; gap: 4px; }
.nota-papel .nv-firmas span { width: 85%; border-top: 1px dotted #111; }
.nota-papel .nv-firmas b { font-size: 12px; }
`;
// Margen de la hoja (el @page va sin margen para que el navegador no imprima su encabezado: ver global/imprimir.js)
export const CSS_HOJA = 'body { padding: 8mm 10mm; }';


const MESES = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
export const bsNota = (n, d = 2) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: d, maximumFractionDigits: d });

/**
 * @param {object} p
 * @param {string} p.titulo        'Nota de Venta' | 'Proforma' | 'Nota de Compra'
 * @param {string|number} p.id     numero del documento (se muestra con 10 digitos)
 * @param {string} p.fecha         'aaaa-mm-dd' -> "Ciudad, 09 De Noviembre Del 2026"
 * @param {object} [p.empresa]     { Nombre, Direccion, Telefono, Nit, Ciudad }
 * @param {string} [p.imagenEmpresa]
 * @param {Array<[string, any]>} p.cabecera   pares etiqueta/valor del recuadro (2 columnas)
 * @param {Array<{titulo, num?, valor:(f)=>any}>} p.columnas
 * @param {Array} p.filas
 * @param {Array<[string, any]>} [p.pie]      pares del pie izquierdo (vendedor, glosa...)
 * @param {Array<[string, any]>} [p.totales]  pares antes del total
 * @param {string} p.etiquetaTotal  ej. 'Total Venta: Bs'
 * @param {number} p.total
 * @param {string|null} [p.grabada] texto del aviso cuando se ofrece al grabar
 * @param {boolean} [p.literal]     "Son: ... Bolivianos" (no en documentos sin montos, ej. despacho)
 * @param {(n:number)=>string} [p.formatoTotal]
 * @param {string[]} [p.firmas]     lineas de firma al pie (ej. ['Encargado Almacen', 'Distribuidor'])
 */
export default function NotaImpresa({ titulo, id, fecha, empresa, imagenEmpresa, cabecera = [], columnas = [], filas = [], claveFila = (f, i) => i, pie = [], totales = [], etiquetaTotal = 'Total: Bs', total = 0, grabada = null, literal = true, formatoTotal = bsNota, firmas = [], onCerrar }) {
  const papel = useRef(null);
  const [logoOk, setLogoOk] = useState(true);
  const [a, mm, d] = String(fecha || '').slice(0, 10).split('-');
  const ciudad = tit(empresa?.Ciudad || 'Potosí');
  const fechaTxt = a ? `${ciudad}, ${String(Number(d)).padStart(2, '0')} De ${MESES[Number(mm) - 1]} Del ${a}` : '';
  const logo = imagenEmpresa ? urlImagen('empresa', imagenEmpresa) : null;
  const imprimir = () => papel.current && imprimirHtml({ cuerpo: papel.current.outerHTML, css: CSS_HOJA + CSS_NOTA, directo: true }); // ya es la vista previa
  useEffect(() => { const k = (e) => e.key === 'Escape' && onCerrar(); window.addEventListener('keydown', k); return () => window.removeEventListener('keydown', k); }, [onCerrar]);
  return createPortal(
    <div className="modal-fondo nota-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-lg nota-venta" role="dialog" aria-modal="true" aria-label={titulo} onMouseDown={(e) => e.stopPropagation()}>
        <style>{CSS_NOTA}</style>
        <div className="modal-cab no-imprimir"><h3>{titulo} #{id}</h3>
          {grabada && <p className="nota-grabada" role="status"><Icono n="check" size={16} /> {grabada} ¿Desea imprimir el comprobante?</p>}
          <button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="nota-vista">
          <article className="nota-papel" ref={papel}>
            <header className="nv-titulo"><h1>{titulo}</h1><b>{String(id).padStart(10, '0')}</b></header>
            <section className="nv-cab">
              <div className="nv-empresa">
                <b>{empresa?.Nombre || ''}</b>
                <span>{ciudad} - Bolivia</span>
                {empresa?.Direccion && <span>{empresa.Direccion}</span>}
                {(empresa?.Telefono || empresa?.Nit) && <span>{empresa?.Telefono ? `Tel. ${empresa.Telefono}` : ''}{empresa?.Telefono && empresa?.Nit ? ' · ' : ''}{empresa?.Nit ? `NIT ${empresa.Nit}` : ''}</span>}
                <small>{fechaTxt}</small>
              </div>
              <div className="nv-logo">{logo && logoOk ? <img src={logo} alt="" onError={() => setLogoOk(false)} /> : null}</div>
            </section>
            <section className="nv-caja nv-cliente">
              <dl>{cabecera.map(([k, x]) => <div key={k}><dt>{k}</dt><dd>{x ?? ''}</dd></div>)}</dl>
            </section>
            <table className="nota-tabla">
              <thead><tr>{columnas.map((c) => <th key={c.titulo} className={c.num ? 'num' : ''} style={c.ancho ? { width: c.ancho } : undefined}>{c.titulo}</th>)}</tr></thead>
              <tbody>{filas.map((f, i) => <tr key={claveFila(f, i)}>{columnas.map((c) => <td key={c.titulo} className={c.num ? 'num' : ''}>{c.valor(f)}</td>)}</tr>)}</tbody>
            </table>
            <section className="nv-caja nv-pie">
              <dl className="nv-pie-izq">{pie.map(([k, x]) => <div key={k}><dt>{k}</dt><dd>{x ?? ''}</dd></div>)}</dl>
              <div className="nota-totales">
                {totales.map(([k, x]) => <div key={k}><span>{k}</span><b>{x}</b></div>)}
                <div className="nota-total"><span>{etiquetaTotal}</span><b>{formatoTotal(total)}</b></div>
              </div>
              {literal && <p className="nota-literal"><span>Son:</span> {montoLiteral(total)}</p>}
            </section>
            {firmas.length > 0 && <section className="nv-firmas">{firmas.map((x) => <div key={x}><span /><b>{x}</b></div>)}</section>}
          </article>
        </div>
        <div className="modal-acciones no-imprimir">
          <button className="btn btn-sec" onClick={onCerrar}>{grabada ? 'No imprimir' : 'Cerrar'}</button>
          <button className="btn btn-pri" autoFocus onClick={imprimir}><Icono n="printer" size={16} /> Imprimir</button>
        </div>
      </div>
    </div>,
    document.body,
  );
}
