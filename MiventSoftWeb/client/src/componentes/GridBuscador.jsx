// =====================================================================
//  GridBuscador: equivale a JGrM_Buscador (Janus GridEX) de ModeloF1
//   - Columnas segun _PMOGetListEstructuraBuscador (List(Of Celda))
//   - Filtro por columna tipo "Contains" + buscador general
//   - Orden por columna, paginacion (15 por pagina), doble clic / Enter abre
//   - Menu contextual de la fila (Ver / Editar / Eliminar): clic derecho en PC (como el escritorio),
//     doble toque o mantener presionado en celular, o tecla Menu / Shift+F10 con la fila elegida
// =====================================================================
import { useEffect, useLayoutEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import Icono from './Icono';

const PRESION_MS = 500;   // mantener presionado en pantalla tactil
const TOLERANCIA_PX = 10; // si el dedo se mueve mas, es scroll y no abre el menu
const DOBLE_TOQUE_MS = 350; // dos toques en la misma tarjeta dentro de este tiempo = doble toque
const GRACIA_MS = 450;    // al soltar el dedo el navegador manda un clic: no debe cerrar ni elegir una opcion

// Menu flotante en la posicion del clic. En celular se muestra como hoja desde abajo.
function MenuFila({ x, y, items, titulo, porToque, onCerrar }) {
  const caja = useRef(null);
  const abiertoEn = useRef(Date.now());
  const recien = () => porToque && Date.now() - abiertoEn.current < GRACIA_MS;
  const [pos, setPos] = useState({ left: x, top: y });
  useLayoutEffect(() => {
    const r = caja.current.getBoundingClientRect();
    const m = 8;
    setPos({
      left: Math.max(m, Math.min(x, window.innerWidth - r.width - m)),
      top: Math.max(m, y + r.height + m > window.innerHeight ? y - r.height : y),
    });
    caja.current.querySelector('button')?.focus({ preventScroll: true });
  }, [x, y]);
  useEffect(() => {
    // un desplazamiento que venia de antes de abrir (inercia, scroll suave) no debe cerrarlo
    const cerrar = () => { if (Date.now() - abiertoEn.current > 300) onCerrar(); };
    window.addEventListener('resize', cerrar);
    window.addEventListener('scroll', cerrar, true);
    return () => { window.removeEventListener('resize', cerrar); window.removeEventListener('scroll', cerrar, true); };
  }, [onCerrar]);
  const teclas = (e) => {
    const bs = Array.from(caja.current.querySelectorAll('button'));
    const i = bs.indexOf(document.activeElement);
    if (e.key === 'Escape' || e.key === 'Tab') { e.preventDefault(); onCerrar(); }
    if (e.key === 'ArrowDown') { e.preventDefault(); bs[(i + 1) % bs.length]?.focus(); }
    if (e.key === 'ArrowUp') { e.preventDefault(); bs[(i - 1 + bs.length) % bs.length]?.focus(); }
  };
  return createPortal(
    <div className="menu-fila-fondo" onMouseDown={() => !recien() && onCerrar()} onContextMenu={(e) => { e.preventDefault(); if (!recien()) onCerrar(); }}>
      <div className="menu-fila" role="menu" aria-label={titulo} ref={caja} style={pos} onMouseDown={(e) => e.stopPropagation()} onKeyDown={teclas}>
        <div className="menu-fila-tit">{titulo}</div>
        {items.map((it) => (
          <button key={it.texto} type="button" role="menuitem" className={`${it.peligro ? 'peligro' : ''} ${it.tono ? `tono-${it.tono}` : ''}`}
            onClick={(e) => { if (e.detail > 0 && recien()) return; onCerrar(); it.onClick(); }}>
            <Icono n={it.icono} size={18} /> {it.texto}
          </button>
        ))}
      </div>
    </div>,
    document.body,
  );
}

export const FILAS_POR_PAGINA = 15;

export function valorCelda(fila, c) {
  const v = fila[c.campo];
  if (c.tipo === 'bool') return v === true || v === 1 || v === '1';
  return v;
}

function Celdita({ fila, c }) {
  const v = valorCelda(fila, c);
  if (c.tipo === 'bool') {
    return <span className={`badge ${v ? 'badge-ok' : 'badge-off'}`}>{v ? c.formato || 'SI' : c.formatoNo || 'NO'}</span>;
  }
  // Codigos (Id, ...Id) se muestran tal cual: 3035, no 3.035
  if (c.tipo === 'num' && /(^id$|id$)/i.test(c.campo) && c.formato === '') return v ?? '';
  if (c.tipo === 'num' && v !== null && v !== undefined && v !== '') {
    return Number(v).toLocaleString('es-BO', { minimumFractionDigits: c.formato === '0.00' ? 2 : 0 });
  }
  return v ?? '';
}

// Numeros de pagina con "..." (1 … 4 5 6 … 20)
function paginasVisibles(actual, total) {
  if (total <= 7) return Array.from({ length: total }, (_, i) => i);
  const s = new Set([0, total - 1, actual - 1, actual, actual + 1].filter((p) => p >= 0 && p < total));
  const orden = [...s].sort((a, b) => a - b);
  const r = [];
  orden.forEach((p, i) => { if (i && p - orden[i - 1] > 1) r.push('…' + p); r.push(p); });
  return r;
}

export function Paginador({ pagina, totalPaginas, desde, hasta, total, onPagina }) {
  if (!total) return null;
  return (
    <nav className="paginador" aria-label="Paginacion">
      <span className="paginador-info">{desde}–{hasta} de {total}</span>
      <div className="paginador-btns">
        <button className="btn-icono" disabled={pagina === 0} onClick={() => onPagina(pagina - 1)} aria-label="Pagina anterior"><Icono n="prev" size={16} /></button>
        {paginasVisibles(pagina, totalPaginas).map((p) =>
          typeof p === 'string' ? <span key={p} className="pag-puntos">…</span> : (
            <button key={p} className={`pag-num ${p === pagina ? 'activa' : ''}`} aria-current={p === pagina ? 'page' : undefined} onClick={() => onPagina(p)}>{p + 1}</button>
          )
        )}
        <button className="btn-icono" disabled={pagina >= totalPaginas - 1} onClick={() => onPagina(pagina + 1)} aria-label="Pagina siguiente"><Icono n="next" size={16} /></button>
      </div>
    </nav>
  );
}

export default function GridBuscador({ estructura, filas, totalFilas, filtros, setFiltros, orden, setOrden, idSel, resaltado, onSeleccionar, onAbrir, acciones, menuFila, habilitado = true, cargando }) {
  const cols = estructura.filter((c) => c.visible);
  const cuerpo = useRef(null);
  const [pagina, setPagina] = useState(0);
  const totalPaginas = Math.max(1, Math.ceil(filas.length / FILAS_POR_PAGINA));
  const [menu, setMenu] = useState(null); // { x, y, fila }
  const presion = useRef(null);           // { t, x, y } mientras el dedo esta presionado
  const ultimoToque = useRef(null);       // { id, t, x, y } del toque anterior (doble toque)
  const tactil = useRef(false);           // el ultimo puntero fue un dedo

  const abrirMenu = (f, x, y, porToque = false) => {
    if (!habilitado || !menuFila || !menuFila(f).length) return false;
    onSeleccionar(f);
    setMenu({ x, y, fila: f, porToque });
    return true;
  };
  const cerrarMenu = useMemo(() => () => setMenu(null), []);
  const cancelarPresion = () => { if (presion.current) { clearTimeout(presion.current.t); presion.current = null; } };
  useEffect(() => cancelarPresion, []);

  const eventosFila = (f) => (!menuFila ? {} : {
    onContextMenu: (e) => {
      // en celular el menu ya lo abrio la presion larga: solo evitar el menu del navegador
      if (menu || abrirMenu(f, e.clientX, e.clientY)) e.preventDefault();
    },
    onPointerDown: (e) => {
      tactil.current = e.pointerType !== 'mouse';
      if (!tactil.current) return;
      cancelarPresion();
      const { clientX: x, clientY: y } = e;
      // doble toque: segundo toque en la misma fila, rapido y en el mismo lugar
      const u = ultimoToque.current;
      const ahora = Date.now();
      if (u && u.id === f.__id && ahora - u.t < DOBLE_TOQUE_MS && Math.hypot(x - u.x, y - u.y) < 30) {
        ultimoToque.current = null;
        if (abrirMenu(f, x, y, true)) { e.preventDefault(); navigator.vibrate?.(15); }
        return;
      }
      ultimoToque.current = { id: f.__id, t: ahora, x, y };
      presion.current = { x, y, t: setTimeout(() => { presion.current = null; if (abrirMenu(f, x, y, true)) navigator.vibrate?.(15); }, PRESION_MS) };
    },
    onPointerMove: (e) => {
      const p = presion.current;
      if (p && Math.hypot(e.clientX - p.x, e.clientY - p.y) > TOLERANCIA_PX) cancelarPresion();
    },
    onPointerUp: cancelarPresion,
    onPointerCancel: cancelarPresion,
  });

  // Al volver a la lista (o al grabar) mostrar la pagina del registro seleccionado
  useEffect(() => {
    const i = filas.findIndex((f) => f.__id === (resaltado || idSel));
    if (i >= 0) setPagina(Math.floor(i / FILAS_POR_PAGINA));
  }, [resaltado, filas.length]); // eslint-disable-line react-hooks/exhaustive-deps

  // Al filtrar u ordenar, volver a la primera pagina
  const firmaFiltro = JSON.stringify(filtros) + JSON.stringify(orden);
  const primera = useRef(true);
  useEffect(() => {
    if (primera.current) { primera.current = false; return; }
    setPagina(0);
  }, [firmaFiltro]);

  const pag = Math.min(pagina, totalPaginas - 1);
  const visibles = useMemo(() => filas.slice(pag * FILAS_POR_PAGINA, (pag + 1) * FILAS_POR_PAGINA), [filas, pag]);

  useEffect(() => {
    cuerpo.current?.querySelector('tr.resaltado, tr.sel')?.scrollIntoView({ block: 'nearest' });
  }, [resaltado, pag]);

  const teclas = (e) => {
    if (!habilitado || !visibles.length) return;
    const i = visibles.findIndex((f) => f.__id === idSel);
    if (e.key === 'ArrowDown') { e.preventDefault(); onSeleccionar(visibles[Math.min(i + 1, visibles.length - 1)]); }
    if (e.key === 'ArrowUp') { e.preventDefault(); onSeleccionar(visibles[Math.max(i - 1, 0)]); }
    if (e.key === 'PageDown') { e.preventDefault(); setPagina(Math.min(pag + 1, totalPaginas - 1)); }
    if (e.key === 'PageUp') { e.preventDefault(); setPagina(Math.max(pag - 1, 0)); }
    if (e.key === 'Enter' && i >= 0) onAbrir(visibles[i]);
    if ((e.key === 'ContextMenu' || (e.key === 'F10' && e.shiftKey)) && i >= 0) {
      e.preventDefault();
      const r = cuerpo.current.querySelector('tr.sel')?.getBoundingClientRect();
      if (r) abrirMenu(visibles[i], r.left + 40, r.bottom);
    }
  };

  const clicOrden = (campo) => {
    if (orden.campo !== campo) setOrden({ campo, asc: true });
    else if (orden.asc) setOrden({ campo, asc: false });
    else setOrden({ campo: null, asc: true });
  };

  const hayFiltro = Object.values(filtros).some(Boolean);
  const nCols = cols.length + (acciones ? 1 : 0);

  return (
    <div className={`grid-buscador ${habilitado ? '' : 'bloqueado'}`}>
      <div className="grid-herramientas">
        <div className="buscar">
          <Icono n="search" size={17} />
          <input type="search" value={filtros.__todo ?? ''} onChange={(e) => setFiltros({ ...filtros, __todo: e.target.value })} placeholder="Buscar en todas las columnas..." aria-label="Buscar en la lista" />
        </div>
        {hayFiltro && <button className="btn btn-txt btn-sm" onClick={() => setFiltros({})}><Icono n="x" size={14} /> Quitar filtros</button>}
      </div>
      <div className="tabla-scroll" tabIndex={0} onKeyDown={teclas} ref={cuerpo}>
        <table className="grid grid-tarjetas">
          <thead>
            <tr>
              {cols.map((c) => (
                <th key={c.campo} style={{ width: c.tamano }} onClick={() => clicOrden(c.campo)} className={`ordenable ${c.tipo === 'num' ? 'num' : ''} ${orden.campo === c.campo ? 'ordenada' : ''}`}
                  aria-sort={orden.campo === c.campo ? (orden.asc ? 'ascending' : 'descending') : 'none'}>
                  {c.titulo}
                  <span className="orden">{orden.campo === c.campo ? (orden.asc ? '↑' : '↓') : '↕'}</span>
                </th>
              ))}
              {acciones && <th className="col-acc" />}
            </tr>
            <tr className="fila-filtro">
              {cols.map((c) => (
                <th key={c.campo}>
                  {c.tipo === 'bool' ? (
                    <select value={filtros[c.campo] ?? ''} onChange={(e) => setFiltros({ ...filtros, [c.campo]: e.target.value })} aria-label={`Filtrar ${c.titulo}`}>
                      <option value="">Todos</option>
                      <option value="1">{c.formato || 'SI'}</option>
                      <option value="0">{c.formatoNo || 'NO'}</option>
                    </select>
                  ) : (
                    <input value={filtros[c.campo] ?? ''} onChange={(e) => setFiltros({ ...filtros, [c.campo]: e.target.value })} aria-label={`Filtrar ${c.titulo}`} placeholder="Filtrar" />
                  )}
                </th>
              ))}
              {acciones && <th />}
            </tr>
          </thead>
          <tbody key={pag}>
            {cargando && !filas.length && Array.from({ length: 6 }, (_, i) => (
              <tr key={`sk${i}`} className="esqueleto" aria-hidden="true">
                {Array.from({ length: nCols }, (_, j) => <td key={j}><span className="esq-barra" style={{ width: `${45 + ((i * 7 + j * 13) % 45)}%` }} /></td>)}
              </tr>
            ))}
            {visibles.map((f, i) => (
              <tr key={f.__id} style={{ '--i': i }}
                className={`fila ${f.__id === idSel ? 'sel' : ''} ${f.__id === resaltado ? 'resaltado' : ''}`}
                onClick={() => habilitado && onSeleccionar(f)} onDoubleClick={() => habilitado && !tactil.current && onAbrir(f)} {...eventosFila(f)}>
                {cols.map((c) => (
                  <td key={c.campo} data-label={c.titulo} className={c.tipo === 'num' ? 'num' : c.tipo === 'bool' ? 'centro' : /fecha|hora/i.test(c.campo) || c.tipo === 'fecha' ? 'nw' : ''}>
                    <Celdita fila={f} c={c} />
                  </td>
                ))}
                {acciones && <td className="col-acc" onClick={(e) => e.stopPropagation()}>{acciones(f)}</td>}
              </tr>
            ))}
            {!cargando && !filas.length && (
              <tr>
                <td colSpan={nCols} className="vacio">
                  <div className="vacio-caja">
                    <Icono n={totalFilas ? 'search' : 'box'} size={34} />
                    <b>{totalFilas ? 'Ningún registro coincide con el filtro' : 'Todavía no hay registros'}</b>
                    <span>{totalFilas ? 'Pruebe con otro texto o quite los filtros.' : 'Use el botón Nuevo para crear el primero.'}</span>
                  </div>
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      <div className="grid-pie">
        <span className="grid-total"><Icono n="list" size={14} /> {filas.length} {filas.length === 1 ? 'registro' : 'registros'}{hayFiltro ? ` (de ${totalFilas})` : ''}</span>
        <Paginador pagina={pag} totalPaginas={totalPaginas} total={filas.length}
          desde={filas.length ? pag * FILAS_POR_PAGINA + 1 : 0} hasta={Math.min((pag + 1) * FILAS_POR_PAGINA, filas.length)} onPagina={setPagina} />
      </div>
      {menu && <MenuFila x={menu.x} y={menu.y} porToque={menu.porToque} items={menuFila(menu.fila)} titulo={cols.slice(0, 2).map((c) => menu.fila[c.campo]).filter((v) => v !== null && v !== undefined && v !== '').join(' · ') || `Registro ${menu.fila.__id}`} onCerrar={cerrarMenu} />}
    </div>
  );
}
