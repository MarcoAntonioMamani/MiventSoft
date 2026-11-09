// =====================================================================
//  Detalle del panel: dialogo que lista los registros detras de un indicador.
//  Escritorio: ventana centrada. Celular: hoja inferior a pantalla casi completa
//  con filas en tarjetas. Busqueda local + paginacion de 15 (igual que las listas).
// =====================================================================
import { useEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { api } from '../api/http';
import { Paginador, FILAS_POR_PAGINA } from './GridBuscador';
import { fmtBs, fmtN } from './Graficos';
import { titulo } from '../global/programas';
import Icono from './Icono';

const COLOR_CHIP = (n) => (/ENTREG/.test(n) ? 'ok' : /ANUL/.test(n) ? 'mal' : /PEND/.test(n) ? 'aviso' : 'info');

function celda(c, v) {
  if (v === null || v === undefined || v === '') return '—';
  if (c.tipo === 'bs') return fmtBs(v);
  if (c.tipo === 'fecha') { const [a, m, d] = String(v).split('-'); return d ? `${d}/${m}/${a}` : v; }
  if (c.tipo === 'num') return fmtN(v);
  if (c.tipo === 'mora') return v > 0 ? <span className={`chip-det ${v > 60 ? 'grave' : v > 30 ? 'mal' : 'aviso'}`}>{v} días</span> : <span className="chip-det ok">Al día</span>;
  if (c.chip) return <span className={`chip-det ${COLOR_CHIP(String(v))}`}>{titulo(String(v))}</span>;
  return typeof v === 'string' && v === v.toUpperCase() && v.length > 3 ? titulo(v) : v;
}

/**
 * consulta = { tipo: 'pedidos'|'cobrar'|'pagar'|'stock0'|'stockmin'|'asignar', titulo, sub, params, accion? }
 */
export default function DetalleDashboard({ consulta, onCerrar }) {
  const [estado, setEstado] = useState({ cargando: true, error: '', datos: null });
  const [texto, setTexto] = useState('');
  const [pagina, setPagina] = useState(0);
  const caja = useRef(null);

  useEffect(() => {
    let vivo = true;
    setEstado({ cargando: true, error: '', datos: null });
    setTexto(''); setPagina(0);
    const q = new URLSearchParams(Object.entries(consulta.params || {}).filter(([, v]) => v !== '' && v !== null && v !== undefined));
    api.get(`/api/dashboard/detalle/${consulta.tipo}?${q}`)
      .then((r) => vivo && setEstado({ cargando: false, error: '', datos: r.data }))
      .catch((e) => vivo && setEstado({ cargando: false, error: e.message, datos: null }));
    return () => { vivo = false; };
  }, [consulta]);

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
    const t = texto.trim().toUpperCase();
    if (!t) return d.filas;
    return d.filas.filter((f) => Object.values(f).some((v) => String(v ?? '').toUpperCase().includes(t)));
  }, [d, texto]);
  const totalPaginas = Math.max(1, Math.ceil(filtradas.length / FILAS_POR_PAGINA));
  const pag = Math.min(pagina, totalPaginas - 1);
  const visibles = filtradas.slice(pag * FILAS_POR_PAGINA, (pag + 1) * FILAS_POR_PAGINA);
  const colBs = d?.columnas.find((c) => c.tipo === 'bs' && (c.k === 'saldo' || c.k === 'total'));
  const sumaFiltrada = colBs ? filtradas.reduce((s, f) => s + Number(f[colBs.k] || 0), 0) : null;

  return createPortal(
    <div className="modal-fondo detalle-fondo" onMouseDown={onCerrar}>
      <div ref={caja} tabIndex={-1} className="modal detalle" role="dialog" aria-modal="true" aria-labelledby="det-tit" onMouseDown={(e) => e.stopPropagation()}>
        <header className="detalle-cab">
          <div>
            <h3 id="det-tit">{consulta.titulo}</h3>
            {consulta.sub && <p>{consulta.sub}</p>}
          </div>
          <button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" size={18} /></button>
        </header>

        {d && (
          <div className="detalle-resumen">
            <span><b>{fmtN(filtradas.length)}</b> {filtradas.length === 1 ? 'registro' : 'registros'}{texto && d.cantidad !== filtradas.length ? ` de ${fmtN(d.cantidad)}` : ''}</span>
            {sumaFiltrada !== null && <span>{colBs.k === 'saldo' ? 'Saldo' : 'Total'}: <b>{fmtBs(sumaFiltrada)}</b></span>}
            {consulta.accion && <button className="btn btn-txt btn-sm" onClick={() => { consulta.accion.fn(); onCerrar(); }}>{consulta.accion.texto}</button>}
          </div>
        )}
        {d && d.filas.length > 8 && (
          <div className="buscar detalle-buscar">
            <Icono n="search" size={16} />
            <input className="control" placeholder="Buscar en el detalle..." value={texto} onChange={(e) => { setTexto(e.target.value); setPagina(0); }} aria-label="Buscar en el detalle" />
          </div>
        )}

        <div className="detalle-cuerpo">
          {estado.cargando && <div className="detalle-cargando"><span className="spinner" /> Cargando...</div>}
          {estado.error && <div className="alerta"><Icono n="alert" size={16} /> {estado.error}</div>}
          {d && !filtradas.length && <div className="panel-vacio"><Icono n="grid" size={26} /><span>Sin registros</span></div>}
          {d && filtradas.length > 0 && (
            <table className="grid grid-tarjetas detalle-tabla">
              <thead><tr>{d.columnas.map((c) => <th key={c.k} className={c.tipo === 'bs' || c.tipo === 'num' ? 'num' : ''}>{c.t}</th>)}</tr></thead>
              <tbody>
                {visibles.map((f, i) => (
                  <tr key={i} style={{ '--i': i }}>
                    {d.columnas.map((c) => (
                      <td key={c.k} data-label={c.t} className={`${c.tipo === 'bs' || c.tipo === 'num' ? 'num' : ''} ${c.ancho ? 'ancha' : ''}`}>{celda(c, f[c.k])}</td>
                    ))}
                  </tr>
                ))}
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
