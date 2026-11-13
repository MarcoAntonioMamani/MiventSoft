// =====================================================================
//  Detalle del panel: dialogo que lista los registros detras de un indicador.
//  Escritorio: ventana centrada. Celular: hoja inferior a pantalla casi completa
//  con filas en tarjetas. Busqueda local + paginacion de 15 (igual que las listas).
//  Rapido: lo ya abierto en el ultimo minuto se muestra al instante (cache en memoria)
//  y se actualiza por detras; mientras carga la primera vez se ve un esqueleto de la tabla.
// =====================================================================
import { useEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { api } from '../api/http';
import { Paginador, FILAS_POR_PAGINA } from './GridBuscador';
import { fmtBs, fmtN } from './Graficos';
import { titulo } from '../global/programas';
import Icono from './Icono';
import { palabrasDe, coincide } from '../global/buscar';

const COLOR_CHIP = (n) => (/ENTREG/.test(n) ? 'ok' : /ANUL/.test(n) ? 'mal' : /PEND/.test(n) ? 'aviso' : 'info');
const ICONO_TIPO = { pedidos: 'cart', cobrar: 'wallet', pagar: 'bag', stock0: 'box', stockmin: 'alert', productosMes: 'box', sinCompra: 'users', clientesTop: 'users', sinVenta: 'box' };

// ---------- cache en memoria (solo lectura, 1 minuto) ----------
const VIGENCIA_MS = 60000;
const REVALIDAR_MS = 8000; // si lo guardado tiene mas de 8 s, se pide de nuevo por detras
const cache = new Map(); // url -> { t, datos }
function deCache(url) {
  const c = cache.get(url);
  return c && Date.now() - c.t < VIGENCIA_MS ? c : null;
}
function aCache(url, datos) {
  cache.delete(url);
  cache.set(url, { t: Date.now(), datos });
  if (cache.size > 60) cache.delete(cache.keys().next().value); // la mas antigua
}
async function pedir(url) {
  const r = await api.get(url);
  aCache(url, r.data);
  return r.data;
}

function celda(c, v) {
  if (v === null || v === undefined || v === '') return <span className="det-nulo">—</span>;
  if (c.tipo === 'bs') return fmtBs(v);
  if (c.tipo === 'fecha') { const [a, m, d] = String(v).split('-'); return d ? `${d}/${m}/${a}` : v; }
  if (c.tipo === 'num') return fmtN(v);
  if (c.tipo === 'mora') return v > 0 ? <span className={`chip-det ${v > 60 ? 'grave' : v > 30 ? 'mal' : 'aviso'}`}>{v} días</span> : <span className="chip-det ok">Al día</span>;
  if (c.chip) return <span className={`chip-det ${COLOR_CHIP(String(v))}`}>{titulo(String(v))}</span>;
  return typeof v === 'string' && v === v.toUpperCase() && v.length > 3 ? titulo(v) : v;
}

// Esqueleto de la tabla mientras llega la primera respuesta
function EsqueletoTabla({ filas = 8, columnas = 5 }) {
  const anchos = [72, 40, 56, 30, 48, 64];
  return (
    <div className="det-esq" aria-hidden="true">
      {Array.from({ length: filas }, (_, i) => (
        <div key={i} className="det-esq-fila" style={{ '--i': i, '--cols': columnas }}>
          {Array.from({ length: columnas }, (_, j) => <span key={j} className="esq-barra" style={{ width: `${anchos[(i + j * 2) % anchos.length]}%` }} />)}
        </div>
      ))}
    </div>
  );
}

// Productos de una venta (MAM_Ventas @tipo 4), debajo de su fila.
// Con div (no tabla): las reglas de .grid-tarjetas del celular no deben tocar esta lista.
function ProductosVenta({ info }) {
  if (!info || info.cargando) {
    return (
      <div className="det-venta det-venta-esq" aria-busy="true" aria-label="Cargando productos">
        {[60, 44, 52].map((w, i) => <div key={i} className="det-venta-linea"><span className="esq-barra" style={{ width: `${w}%` }} /><span className="esq-barra" /><span className="esq-barra" /><span className="esq-barra dv-desc" /><span className="esq-barra" /></div>)}
      </div>
    );
  }
  if (info.error) return <div className="alerta"><Icono n="alert" size={16} /> {info.error}</div>;
  const { filas, suma, descuento, total, glosa } = info.datos;
  if (!filas.length) return <div className="det-venta-cargando">Sin productos</div>;
  return (
    <div className="det-venta" role="table" aria-label="Productos de la venta">
      <div className="det-venta-linea det-venta-cab" role="row">
        <span role="columnheader">Producto</span><span role="columnheader" className="num">Cant.</span><span role="columnheader" className="num">Precio</span>
        <span role="columnheader" className="num dv-desc">Desc.</span><span role="columnheader" className="num">Total</span>
      </div>
      {filas.map((p, i) => (
        <div key={i} className="det-venta-linea" role="row" style={{ '--i': i }}>
          <span role="cell" className="dv-prod">{titulo(p.producto)}{p.kit && <small> · kit {titulo(p.kit)}</small>}</span>
          <span role="cell" className="num dv-cant">{fmtN(p.cantidad)}</span>
          <span role="cell" className="num dv-precio">{fmtBs(p.precio)}</span>
          <span role="cell" className="num dv-desc">{p.descuento ? fmtBs(p.descuento) : '—'}</span>
          <span role="cell" className="num dv-total"><b>{fmtBs(p.total)}</b></span>
        </div>
      ))}
      <div className="det-venta-pie">
        {glosa && <span className="det-venta-glosa">Glosa: {glosa}</span>}
        {descuento > 0 && <span>Subtotal {fmtBs(suma)} · Descuento {fmtBs(descuento)}</span>}
        <b>Total {fmtBs(total)}</b>
      </div>
    </div>
  );
}

/**
 * consulta = { tipo: 'pedidos'|'cobrar'|'pagar'|'stock0'|'stockmin'|'productosMes'|'sinCompra'|'clientesTop'|'sinVenta', titulo, sub, params, accion? }
 * Si la respuesta trae detalleVenta, cada fila (id + fecha) se despliega con sus productos.
 */
export default function DetalleDashboard({ consulta, onCerrar }) {
  const url = useMemo(() => {
    const q = new URLSearchParams(Object.entries(consulta.params || {}).filter(([, v]) => v !== '' && v !== null && v !== undefined));
    return `/api/dashboard/detalle/${consulta.tipo}?${q}`;
  }, [consulta]);
  const [estado, setEstado] = useState(() => { const c = deCache(url); return c ? { cargando: false, error: '', datos: c.datos } : { cargando: true, error: '', datos: null }; });
  const [texto, setTexto] = useState('');
  const [pagina, setPagina] = useState(0);
  const [abierta, setAbierta] = useState(null); // id de la venta desplegada
  const [ventas, setVentas] = useState({}); // id -> { cargando, error, datos }
  const caja = useRef(null);

  const alternar = (f) => {
    const id = f.id;
    if (abierta === id) { setAbierta(null); return; }
    setAbierta(id);
    if (ventas[id]?.datos) return;
    const fecha = f.fecha || consulta.params?.fecha || '';
    const u = `/api/dashboard/venta/${encodeURIComponent(id)}?${new URLSearchParams(fecha ? { fecha } : {})}`;
    const c = deCache(u);
    if (c) { setVentas((v) => ({ ...v, [id]: { datos: c.datos } })); return; }
    setVentas((v) => ({ ...v, [id]: { cargando: true } }));
    pedir(u)
      .then((datos) => setVentas((v) => ({ ...v, [id]: { datos } })))
      .catch((e) => setVentas((v) => ({ ...v, [id]: { error: e.message } })));
  };

  useEffect(() => {
    let vivo = true;
    const c = deCache(url);
    setTexto(''); setPagina(0); setAbierta(null); setVentas({});
    if (c) {
      setEstado({ cargando: false, error: '', datos: c.datos });
      if (Date.now() - c.t < REVALIDAR_MS) return () => { vivo = false; };
    } else {
      setEstado({ cargando: true, error: '', datos: null });
    }
    // primera vez: con esqueleto; desde cache: se actualiza por detras sin parpadeo
    pedir(url)
      .then((datos) => vivo && setEstado({ cargando: false, error: '', datos }))
      .catch((e) => vivo && setEstado((s) => (s.datos ? s : { cargando: false, error: e.message, datos: null })));
    return () => { vivo = false; };
  }, [url]);

  // Escape cierra, foco al abrir, bloquear scroll del fondo
  useEffect(() => {
    const tecla = (e) => e.key === 'Escape' && onCerrar();
    document.addEventListener('keydown', tecla);
    const prev = document.body.style.overflow;
    document.body.style.overflow = 'hidden';
    caja.current?.focus();
    return () => { document.removeEventListener('keydown', tecla); document.body.style.overflow = prev; };
  }, [onCerrar]);

  const d = estado.datos;
  const filtradas = useMemo(() => {
    if (!d) return [];
    const w = palabrasDe(texto);
    if (!w.length) return d.filas;
    return d.filas.filter((f) => coincide(Object.values(f).map((v) => v ?? '').join(' '), w));
  }, [d, texto]);
  const totalPaginas = Math.max(1, Math.ceil(filtradas.length / FILAS_POR_PAGINA));
  const pag = Math.min(pagina, totalPaginas - 1);
  const visibles = filtradas.slice(pag * FILAS_POR_PAGINA, (pag + 1) * FILAS_POR_PAGINA);
  const colBs = d?.columnas.find((c) => c.tipo === 'bs' && (c.k === 'saldo' || c.k === 'total'));
  const sumaFiltrada = colBs ? filtradas.reduce((s, f) => s + Number(f[colBs.k] || 0), 0) : null;

  return createPortal(
    <div className="modal-fondo detalle-fondo" onMouseDown={onCerrar}>
      <div ref={caja} tabIndex={-1} className="modal detalle" role="dialog" aria-modal="true" aria-labelledby="det-tit" aria-busy={estado.cargando} onMouseDown={(e) => e.stopPropagation()}>
        <header className="detalle-cab">
          <span className="detalle-ic" aria-hidden="true"><Icono n={ICONO_TIPO[consulta.tipo] || 'grid'} size={19} /></span>
          <div className="detalle-tit">
            <h3 id="det-tit">{consulta.titulo}</h3>
            {consulta.sub && <p>{consulta.sub}</p>}
          </div>
          <button className="btn-icono" onClick={onCerrar} aria-label="Cerrar" title="Cerrar (Esc)"><Icono n="x" size={18} /></button>
        </header>

        <div className="detalle-resumen">
          {d ? (
            <>
              <div className="det-stat">
                <span>Registros</span>
                <b>{fmtN(filtradas.length)}{texto && d.cantidad !== filtradas.length ? <small> de {fmtN(d.cantidad)}</small> : null}</b>
              </div>
              {sumaFiltrada !== null && <div className="det-stat det-stat-total"><span>{colBs.k === 'saldo' ? 'Saldo' : 'Total'}</span><b>{fmtBs(sumaFiltrada)}</b></div>}
            </>
          ) : estado.cargando ? (
            <><div className="det-stat esq" aria-hidden="true"><span className="esq-barra" /><span className="esq-barra alta" /></div><div className="det-stat esq" aria-hidden="true"><span className="esq-barra" /><span className="esq-barra alta" /></div></>
          ) : null}
          {consulta.accion && <button className="btn btn-sec btn-sm det-accion" onClick={() => { consulta.accion.fn(); onCerrar(); }}>{consulta.accion.texto}</button>}
        </div>
        {d && d.filas.length > 8 && (
          <div className="buscar detalle-buscar">
            <Icono n="search" size={16} />
            <input className="control" placeholder="Buscar en el detalle..." value={texto} onChange={(e) => { setTexto(e.target.value); setPagina(0); }} aria-label="Buscar en el detalle" />
          </div>
        )}

        <div className="detalle-cuerpo">
          {estado.cargando && !d && <EsqueletoTabla columnas={5} />}
          {estado.error && <div className="alerta det-error"><Icono n="alert" size={16} /> {estado.error}</div>}
          {d && !filtradas.length && <div className="panel-vacio"><Icono n="grid" size={26} /><span>{texto ? 'Sin coincidencias para la búsqueda' : 'Sin registros'}</span></div>}
          {d && filtradas.length > 0 && (
            <table className="grid grid-tarjetas detalle-tabla">
              <thead><tr>{d.columnas.map((c) => <th key={c.k} className={c.tipo === 'bs' || c.tipo === 'num' ? 'num' : ''}>{c.t}</th>)}</tr></thead>
              <tbody key={`${pag}-${texto}`}>
                {visibles.map((f, i) => {
                  const conDet = !!d.detalleVenta && f.id !== null && f.id !== undefined;
                  const abre = conDet && abierta === f.id;
                  const clic = conDet ? {
                    className: `fila-clic ${abre ? 'det-abierta' : ''}`, tabIndex: 0, 'aria-expanded': abre, title: abre ? 'Ocultar productos' : 'Ver productos de la venta',
                    onClick: () => alternar(f), onKeyDown: (e) => (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), alternar(f)),
                  } : {};
                  return [
                    <tr key={`f${i}`} style={{ '--i': i }} {...clic}>
                      {d.columnas.map((c, j) => (
                        <td key={c.k} data-label={c.t} className={`${c.tipo === 'bs' || c.tipo === 'num' ? 'num' : ''} ${c.ancho ? 'ancha' : ''}`}>
                          {conDet && j === 0 && <Icono n="next" size={13} className={`det-flecha ${abre ? 'abierta' : ''}`} />}
                          {celda(c, f[c.k])}
                        </td>
                      ))}
                    </tr>,
                    abre && <tr key={`d${i}`} className="det-venta-fila"><td colSpan={d.columnas.length}><ProductosVenta info={ventas[f.id]} /></td></tr>,
                  ];
                })}
              </tbody>
            </table>
          )}
        </div>
        {d?.truncado && <p className="detalle-nota">Se muestran los primeros {fmtN(d.filas.length)} de {fmtN(d.cantidad)} registros.</p>}
        {d && filtradas.length > FILAS_POR_PAGINA && (
          <div className="detalle-pie">
            <Paginador pagina={pag} totalPaginas={totalPaginas} desde={pag * FILAS_POR_PAGINA + 1} hasta={Math.min(filtradas.length, (pag + 1) * FILAS_POR_PAGINA)} total={filtradas.length} onPagina={setPagina} />
          </div>
        )}
      </div>
    </div>,
    document.body,
  );
}
