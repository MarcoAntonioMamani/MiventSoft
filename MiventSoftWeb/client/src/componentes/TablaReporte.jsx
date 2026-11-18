// =====================================================================
//  TablaReporte: tabla de solo lectura para listados y reportes
//  (equivale a las grillas Janus con fila de totales + "Imprimir Reporte" de VB)
//   - busqueda en todas las columnas, paginacion de 15, totales del filtro
//   - en celular cada fila es una tarjeta
//   - Imprimir (todas las filas filtradas) y exportar a Excel (CSV)
//  columnas: [{ campo, titulo, tipo: 'texto'|'num'|'bs'|'fecha'|'entero', total, render(f), clase(f) }]
//  seleccion (opcional): { ids: Set, onCambio(Set) } -> casilla por fila (como la columna Asignar de Janus);
//    la casilla del encabezado marca / desmarca todas las filas del filtro actual
//  claseFila(f) (opcional): clase CSS extra por fila (color de toda la fila, como GridEXFormatCondition)
//  onImprimir(filasFiltradas) (opcional): impresion propia (ej. ReporteImpreso con grupos) en vez de la tabla simple
// =====================================================================
import { useMemo, useState } from 'react';
import { Paginador, FILAS_POR_PAGINA } from './GridBuscador';
import { useSesion } from '../global/Sesion';
import Icono from './Icono';
import { imprimirReporte } from './ReporteImpreso';
import { palabrasDe, coincide } from '../global/buscar';
import { descargarTexto } from '../global/appMovil';

const bs = (n, d = 2) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: d, maximumFractionDigits: d });
export function fechaTxt(v) {
  if (!v) return '';
  const m = /^(\d{4})-(\d{2})-(\d{2})(.*)$/.exec(String(v));
  return m ? `${m[3]}/${m[2]}/${m[1]}${m[4] ? m[4].replace('T', ' ').slice(0, 6) : ''}` : String(v);
}
export function valorTexto(c, v) {
  if (v === null || v === undefined || v === '') return '';
  if (c.tipo === 'bs' || c.tipo === 'num') return bs(v, c.decimales ?? 2);
  if (c.tipo === 'entero') return Number(v).toLocaleString('es-BO', { maximumFractionDigits: 0 });
  if (c.tipo === 'fecha') return fechaTxt(v);
  return String(v);
}
const esNum = (c) => ['bs', 'num', 'entero'].includes(c.tipo);

export default function TablaReporte({ columnas, filas = [], cargando, titulo, subtitulo, onFila, accion, vacio = 'Sin registros', claveFila = 'Id', herramientas, filaSel, seleccion, onImprimir, claseFila }) {
  const { sesion } = useSesion();
  const [texto, setTexto] = useState('');
  const [pagina, setPagina] = useState(0);
  const [orden, setOrden] = useState({ campo: null, asc: true });

  const filtradas = useMemo(() => {
    const palabras = palabrasDe(texto);
    let r = filas.filter((f) => {
      if (!palabras.length) return true;
      return coincide(columnas.map((c) => valorTexto(c, f[c.campo])).join(' '), palabras);
    });
    if (orden.campo) {
      const c = columnas.find((x) => x.campo === orden.campo);
      r = [...r].sort((a, b) => {
        const x = a[orden.campo]; const y = b[orden.campo];
        const cmp = esNum(c) ? Number(x || 0) - Number(y || 0) : String(x ?? '').localeCompare(String(y ?? ''), 'es', { numeric: true });
        return orden.asc ? cmp : -cmp;
      });
    }
    return r;
  }, [filas, texto, orden, columnas]);

  const totales = useMemo(() => Object.fromEntries(columnas.filter((c) => c.total).map((c) => [c.campo, filtradas.reduce((s, f) => s + Number(f[c.campo] || 0), 0)])), [filtradas, columnas]);
  const totalPaginas = Math.max(1, Math.ceil(filtradas.length / FILAS_POR_PAGINA));
  const pag = Math.min(pagina, totalPaginas - 1);
  const visibles = filtradas.slice(pag * FILAS_POR_PAGINA, (pag + 1) * FILAS_POR_PAGINA);
  const hayTotales = Object.keys(totales).length > 0;
  const nCols = columnas.length + (accion ? 1 : 0) + (seleccion ? 1 : 0);

  // ---- seleccion ----
  const marcadas = seleccion ? filtradas.filter((f) => seleccion.ids.has(f[claveFila])).length : 0;
  const todas = seleccion && filtradas.length > 0 && marcadas === filtradas.length;
  const alternar = (f) => {
    const n = new Set(seleccion.ids);
    if (n.has(f[claveFila])) n.delete(f[claveFila]); else n.add(f[claveFila]);
    seleccion.onCambio(n);
  };
  const alternarTodas = () => {
    const n = new Set(seleccion.ids);
    filtradas.forEach((f) => (todas ? n.delete(f[claveFila]) : n.add(f[claveFila])));
    seleccion.onCambio(n);
  };

  const exportar = () => {
    const esc = (s) => `"${String(s ?? '').replace(/"/g, '""')}"`;
    const lineas = [columnas.map((c) => esc(c.titulo)).join(';'),
      ...filtradas.map((f) => columnas.map((c) => (esNum(c) ? String(Number(f[c.campo] || 0)).replace('.', ',') : esc(valorTexto(c, f[c.campo])))).join(';'))];
    descargarTexto(`${(titulo || 'reporte').replace(/[^\w\- ]+/g, '').trim().replace(/\s+/g, '_')}.csv`, `﻿${lineas.join('\r\n')}`);
  };
  const imprimir = () => {
    if (onImprimir) { onImprimir(filtradas); return; }
    // reporte estandar (cabecera, totales, texto sin cortar) en la vista previa
    imprimirReporte({
      titulo: titulo || 'Reporte', usuario: sesion?.gs_user || '', datos: subtitulo ? [['', subtitulo]] : [],
      columnas: columnas.map((c) => ({ campo: c.campo, titulo: c.titulo, tipo: c.tipo, total: c.total, decimales: c.decimales })), filas: filtradas,
    });
  };

  return (
    <div className="tabla-reporte">
      <div className="tr-herramientas">
        <div className="buscar"><Icono n="search" size={16} />
          <input className="control" placeholder="Buscar..." value={texto} onChange={(e) => { setTexto(e.target.value); setPagina(0); }} aria-label={`Buscar en ${titulo || 'la tabla'}`} />
        </div>
        {herramientas}
        <div className="tr-acciones">
          <button className="btn btn-sec btn-sm" onClick={exportar} disabled={!filtradas.length} title="Exportar a Excel"><Icono n="save" size={15} /> Excel</button>
          <button className="btn btn-sec btn-sm" onClick={imprimir} disabled={!filtradas.length} title="Imprimir reporte"><Icono n="list" size={15} /> Imprimir</button>
        </div>
      </div>
      <div className="tabla-scroll">
        <table className={`grid grid-tarjetas tr-tabla ${seleccion ? 'con-sel' : ''}`}>
          <thead>
            <tr>
              {seleccion && (
                <th className="col-sel">
                  <input type="checkbox" checked={!!todas} ref={(el) => { if (el) el.indeterminate = marcadas > 0 && !todas; }}
                    onChange={alternarTodas} disabled={!filtradas.length} aria-label="Seleccionar todos" title="Seleccionar todos" />
                </th>
              )}
              {columnas.map((c) => (
                <th key={c.campo} className={esNum(c) ? 'num' : ''} style={c.ancho ? { minWidth: c.ancho } : undefined}>
                  <button type="button" className="th-orden" onClick={() => setOrden((o) => ({ campo: c.campo, asc: o.campo === c.campo ? !o.asc : true }))}>
                    {c.titulo}{orden.campo === c.campo ? (orden.asc ? ' ▲' : ' ▼') : ''}
                  </button>
                </th>
              ))}
              {accion && <th aria-label="Acciones" />}
            </tr>
          </thead>
          <tbody>
            {cargando && !filas.length && Array.from({ length: 6 }, (_, i) => (
              <tr key={`sk${i}`} className="esqueleto" aria-hidden="true">
                {Array.from({ length: nCols }, (_, j) => <td key={j}><span className="esq-barra" style={{ width: `${45 + ((i * 7 + j * 13) % 45)}%` }} /></td>)}
              </tr>
            ))}
            {visibles.map((f, i) => (
              <tr key={f[claveFila] ?? i} className={`${onFila ? 'fila-clic' : ''} ${(filaSel !== undefined && filaSel === f[claveFila]) || seleccion?.ids.has(f[claveFila]) ? 'sel' : ''} ${claseFila ? claseFila(f) || '' : ''}`} onClick={onFila ? () => onFila(f) : undefined}
                tabIndex={onFila ? 0 : undefined} onKeyDown={onFila ? (e) => e.key === 'Enter' && e.target === e.currentTarget && onFila(f) : undefined}>
                {seleccion && (
                  <td className="col-sel" data-label="Seleccionar" onClick={(e) => e.stopPropagation()}>
                    <input type="checkbox" checked={seleccion.ids.has(f[claveFila])} onChange={() => alternar(f)} aria-label={`Seleccionar ${f[claveFila]}`} />
                  </td>
                )}
                {columnas.map((c) => (
                  <td key={c.campo} data-label={c.titulo} className={`${esNum(c) ? 'num' : ''} ${c.clase ? c.clase(f) : ''}`}>{c.render ? c.render(f) : valorTexto(c, f[c.campo])}</td>
                ))}
                {accion && <td className="col-acc" onClick={(e) => e.stopPropagation()}>{accion(f)}</td>}
              </tr>
            ))}
            {!cargando && !filtradas.length && <tr><td colSpan={nCols} className="vacio">{vacio}</td></tr>}
          </tbody>
          {hayTotales && filtradas.length > 0 && (
            <tfoot>
              <tr className="tr-total">
                {seleccion && <td className="col-sel" />}
                {columnas.map((c, i) => <td key={c.campo} className={`${esNum(c) ? 'num' : ''} ${!c.total && i === 0 ? 'tf-tit' : ''}`} data-label={c.total ? `Total ${c.titulo}` : ''}>{c.total ? <b>{valorTexto(c, totales[c.campo])}</b> : i === 0 ? <b>TOTAL ({filtradas.length})</b> : ''}</td>)}
                {accion && <td />}
              </tr>
            </tfoot>
          )}
        </table>
      </div>
      <div className="grid-pie"><Paginador pagina={pag} totalPaginas={totalPaginas} desde={pag * FILAS_POR_PAGINA + 1} hasta={Math.min(filtradas.length, (pag + 1) * FILAS_POR_PAGINA)} total={filtradas.length} onPagina={setPagina} /></div>

    </div>
  );
}
