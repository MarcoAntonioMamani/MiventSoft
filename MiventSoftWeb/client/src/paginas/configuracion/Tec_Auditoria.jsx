// =====================================================================
//  SOLO WEB: Auditoria De Registros (programa btnAuditoriaRegistros)   SP: MAM_Auditoria (server/sql/07, 08 y 10)
//  Que se MODIFICO o ELIMINO en ventas, compras y movimientos de inventario (desde la web o el escritorio),
//  quien lo hizo, cuando y desde donde (equipo, IP, navegador, ubicacion).
//   PC:      filtros en una fila, tabla de 7 columnas, detalle en ventana con panel lateral "Quien y desde donde".
//   Celular: filtros plegables, tarjetas, detalle en hoja a pantalla completa.
//   Detalle: por defecto solo lo que CAMBIO (campos y lineas); "Ver todo" muestra el resto.
// =====================================================================
import { useCallback, useEffect, useMemo, useState } from 'react';
import { createPortal } from 'react-dom';
import TablaReporte, { valorTexto, fechaTxt } from '../../componentes/TablaReporte';
import { useToast } from '../../componentes/Toast';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { useAlActivar } from '../../global/vistaActiva';
import { palabrasDe, coincide } from '../../global/buscar';
import { useSesion } from '../../global/Sesion';
import { imprimirAuditoria, imprimirReporteAuditoria } from './AuditoriaImpresa';

const URL = '/api/configuracion/auditoria';
const dos = (n) => String(n).padStart(2, '0');
const isoDe = (d) => `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`;
const hoy = () => isoDe(new Date());
const haceDias = (n) => { const d = new Date(); d.setDate(d.getDate() - n); return isoDe(d); };
const POR_PAGINA_MOVIL = 25;

const OPERACIONES = [
  { id: 0, texto: 'Todo', icono: 'list' },
  { id: 2, texto: 'Eliminados', icono: 'trash' },
  { id: 1, texto: 'Modificados', icono: 'pencil' },
];
const MODULOS = [{ id: 0, texto: 'Todos' }, { id: 1, texto: 'Ventas' }, { id: 2, texto: 'Compras' }, { id: 3, texto: 'Movimientos' }];
const ESTADO_LINEA = {
  agregado: { texto: 'Agregado', clase: 'aud-ok' },
  quitado: { texto: 'Quitado', clase: 'aud-mal' },
  modificado: { texto: 'Modificado', clase: 'aud-aviso' },
  igual: { texto: 'Sin cambio', clase: 'aud-neutro' },
  eliminado: { texto: 'Eliminado', clase: 'aud-mal' },
};

/** true en pantallas angostas (celular) */
function useEsMovil(consulta = '(max-width: 760px)') {
  const [m, setM] = useState(() => typeof window !== 'undefined' && !!window.matchMedia?.(consulta).matches);
  useEffect(() => {
    const mq = window.matchMedia?.(consulta);
    if (!mq) return undefined;
    const fn = () => setM(mq.matches);
    mq.addEventListener?.('change', fn);
    return () => mq.removeEventListener?.('change', fn);
  }, [consulta]);
  return m;
}

const BadgeOperacion = ({ op }) => (Number(op) === 2
  ? <span className="badge aud-mal">Eliminado</span>
  : <span className="badge aud-aviso">Modificado</span>);

// Movimientos: el total es cantidad de productos, no Bs
const montoTxt = (f, v) => (v === null || v === undefined ? '' : `${Number(f.Modulo) === 3 ? '' : 'Bs '}${valorTexto({ tipo: 'num' }, v)}`);
const horaTxt = (v) => { const m = /[T ](\d{2}:\d{2})/.exec(String(v || '')); return m ? m[1] : ''; };
const diaTxt = (v) => fechaTxt(String(v || '').slice(0, 10));
const plural = (n, s, p = `${s}s`) => `${n} ${n === 1 ? s : p}`;

function TotalCambio({ f }) {
  if (f.TotalAntes == null) return null;
  if (Number(f.Operacion) === 2 || f.TotalDespues == null) return <span className="au-total elim">{montoTxt(f, f.TotalAntes)}</span>;
  if (Number(f.TotalAntes) === Number(f.TotalDespues)) return <span className="au-total">{montoTxt(f, f.TotalDespues)}</span>;
  return <span className="au-total"><s>{montoTxt(f, f.TotalAntes)}</s><Icono n="next" size={11} /><b>{montoTxt(f, f.TotalDespues)}</b></span>;
}
const ubicTxt = (u) => (u ? (u.aproximada ? 'Ubic. aprox.' : 'GPS') : '');

const COLUMNAS = [
  { campo: 'Fecha', titulo: 'Fecha', tipo: 'fecha', render: (f) => <span className="au-2l"><b>{diaTxt(f.Fecha)}</b><small>{horaTxt(f.Fecha)}</small></span> },
  { campo: 'NombreOperacion', titulo: 'Operación', render: (f) => <BadgeOperacion op={f.Operacion} /> },
  { campo: 'DocumentoId', titulo: 'Documento', render: (f) => <span className="au-2l"><b>{f.NombreModulo} N° {f.DocumentoId}</b><small>{f.FechaDocumento ? `del ${fechaTxt(f.FechaDocumento)}` : ''}</small></span> },
  { campo: 'Descripcion', titulo: 'Descripción', render: (f) => <span className="au-desc" title={f.Descripcion}>{f.Descripcion}</span> },
  { campo: 'TotalAntes', titulo: 'Total', tipo: 'num', render: (f) => <TotalCambio f={f} /> },
  { campo: 'Usuario', titulo: 'Usuario', render: (f) => <span className="au-desc au-usuario" title={f.Usuario}>{f.Usuario}</span> },
  { campo: 'Equipo', titulo: 'Desde', render: (f) => (
    <span className="au-2l au-desde">
      <b><Icono n={f.Origen === 'Web' ? 'globe' : 'monitor'} size={13} /> {f.Equipo || f.Origen}</b>
      <small>{[f.IpLocal || f.IpPublica, ubicTxt(f.Ubicacion)].filter(Boolean).join(' · ') || f.Origen}</small>
    </span>
  ) },
];

const celda = (c, v) => (v === null || v === undefined || v === '' ? <span className="aud-vacio">—</span> : valorTexto(c, c.tipo === 'num' ? Number(v) : v));
const DeA = ({ c, a, b }) => <span className="au-de-a"><s>{celda(c, a)}</s><Icono n="next" size={11} /><b>{celda(c, b)}</b></span>;

// ---------- Detalle ----------
function Dato({ icono, titulo, children }) {
  return (
    <div className="au-dato">
      <span className="au-dato-ic"><Icono n={icono} size={15} /></span>
      <div><span className="au-dato-tit">{titulo}</span><div className="au-dato-val">{children}</div></div>
    </div>
  );
}

function PanelDonde({ r }) {
  const u = r.Ubicacion;
  return (
    <section className="au-panel">
      <h4 className="au-tit">Quién y desde dónde</h4>
      <Dato icono="user" titulo="Usuario">{r.Usuario || '—'}</Dato>
      <Dato icono="history" titulo="Fecha y hora">{fechaTxt(r.Fecha)}</Dato>
      <Dato icono={r.Origen === 'Web' ? 'globe' : 'monitor'} titulo={r.Origen === 'Web' ? 'Web' : 'Escritorio'}>
        {r.Navegador || <span className="aud-vacio">—</span>}
      </Dato>
      <Dato icono="monitor" titulo="Equipo">{r.Equipo || <span className="aud-vacio">—</span>}</Dato>
      <Dato icono="globe" titulo="Dirección IP">
        {!r.IpLocal && !r.IpPublica && <span className="aud-vacio">—</span>}
        {r.IpLocal && <span className="au-ip"><em>Local</em>{r.IpLocal}</span>}
        {r.IpPublica && <span className="au-ip"><em>Pública</em>{r.IpPublica}</span>}
      </Dato>
      <Dato icono="pin" titulo="Ubicación">
        {u ? (
          <a className="au-mapa" href={u.mapa} target="_blank" rel="noopener noreferrer">
            <span className={`au-chip ${u.aproximada ? 'aprox' : 'gps'}`}>{u.aproximada ? 'Aproximada por IP' : `GPS${u.precision != null ? ` ± ${u.precision} m` : ''}`}</span>
            <span className="au-coord">{u.latTexto ?? u.lat}, {u.lngTexto ?? u.lng}</span>
            <span className="au-mapa-ver">Ver en el mapa <Icono n="next" size={12} /></span>
          </a>
        ) : <span className="aud-vacio">{r.Origen === 'Web' ? 'No registrada' : 'No aplica'}</span>}
      </Dato>
    </section>
  );
}

function Cabecera({ cabecera, eliminado }) {
  const [todos, setTodos] = useState(false);
  const cambios = cabecera.filter((c) => c.cambio);
  const lista = eliminado || todos ? cabecera : cambios;
  return (
    <section className="au-sec">
      <div className="au-sec-cab">
        <h4 className="au-tit">{eliminado ? 'Datos del documento' : 'Cabecera'}
          {!eliminado && <span className={`badge ${cambios.length ? 'aud-aviso' : 'aud-neutro'}`}>{cambios.length ? plural(cambios.length, 'campo cambiado', 'campos cambiados') : 'Sin cambios'}</span>}
        </h4>
        {!eliminado && cambios.length < cabecera.length && (
          <button type="button" className="au-link" onClick={() => setTodos((t) => !t)}>{todos ? 'Ver solo los cambios' : `Ver todos los campos (${cabecera.length})`}</button>
        )}
      </div>
      {lista.length > 0 && (
        <div className={`au-campos ${eliminado ? 'dos' : ''}`}>
          {lista.map((c) => (
            <div key={c.campo} className={`au-campo ${c.cambio ? 'cambio' : ''}`}>
              <span className="au-campo-tit">{c.titulo}</span>
              <span className="au-campo-val">{c.cambio ? <DeA c={c} a={c.antes} b={c.despues} /> : celda(c, c.antes ?? c.despues)}</span>
            </div>
          ))}
        </div>
      )}
    </section>
  );
}

function Lineas({ d, eliminado, movil }) {
  const [todas, setTodas] = useState(false);
  const rd = d.resumenDetalle;
  const conCambio = d.detalle.filter((l) => l.estado !== 'igual');
  const iguales = d.detalle.length - conCambio.length;
  const lista = eliminado || todas || !iguales ? d.detalle : conCambio;
  const prod = d.columnas.find((c) => c.campo === 'Producto');
  const otras = d.columnas.filter((c) => c.campo !== 'Producto');
  return (
    <section className="au-sec">
      <div className="au-sec-cab">
        <h4 className="au-tit">{eliminado ? `Detalle (${plural(d.detalle.length, 'línea')})` : 'Detalle'}
          {!eliminado && (
            <span className="au-chips">
              {rd.agregados > 0 && <span className="badge aud-ok">{rd.agregados} agregado{rd.agregados > 1 ? 's' : ''}</span>}
              {rd.quitados > 0 && <span className="badge aud-mal">{rd.quitados} quitado{rd.quitados > 1 ? 's' : ''}</span>}
              {rd.modificados > 0 && <span className="badge aud-aviso">{rd.modificados} modificado{rd.modificados > 1 ? 's' : ''}</span>}
              {!conCambio.length && <span className="badge aud-neutro">Sin cambios</span>}
            </span>
          )}
        </h4>
        {!eliminado && iguales > 0 && (
          <button type="button" className="au-link" onClick={() => setTodas((t) => !t)}>{todas ? 'Ocultar líneas sin cambio' : `Ver también ${plural(iguales, 'línea')} sin cambio`}</button>
        )}
      </div>
      {!lista.length && <p className="au-nota">{d.detalle.length ? 'Las líneas del detalle no cambiaron.' : 'Sin detalle.'}</p>}
      {lista.length > 0 && (movil ? (
        <div className="au-lineas">
          {lista.map((l, i) => {
            const e = ESTADO_LINEA[l.estado];
            const base = l.despues || l.antes;
            return (
              <article key={i} className={`au-linea ${l.estado}`}>
                <header>
                  <b>{prod ? celda(prod, base.Producto) : `Línea ${i + 1}`}</b>
                  {!eliminado && <span className={`badge ${e.clase}`}>{e.texto}</span>}
                </header>
                <dl>
                  {otras.filter((c) => (base[c.campo] !== '' && base[c.campo] != null) || l.cambios.includes(c.campo)).map((c) => (
                    <div key={c.campo} className={l.cambios.includes(c.campo) ? 'cambio' : ''}>
                      <dt>{c.titulo}</dt>
                      <dd>{l.cambios.includes(c.campo) ? <DeA c={c} a={l.antes[c.campo]} b={l.despues[c.campo]} /> : celda(c, base[c.campo])}</dd>
                    </div>
                  ))}
                </dl>
              </article>
            );
          })}
        </div>
      ) : (
        <div className="au-tabla-env">
          <table className="au-tabla">
            <thead><tr>{!eliminado && <th>Estado</th>}{d.columnas.map((c) => <th key={c.campo} className={c.tipo === 'num' ? 'num' : ''}>{c.titulo}</th>)}</tr></thead>
            <tbody>
              {lista.map((l, i) => {
                const e = ESTADO_LINEA[l.estado];
                const base = l.despues || l.antes;
                return (
                  <tr key={i} className={`au-fila-${l.estado}`}>
                    {!eliminado && <td><span className={`badge ${e.clase}`}>{e.texto}</span></td>}
                    {d.columnas.map((c) => (
                      <td key={c.campo} className={c.tipo === 'num' ? 'num' : ''}>
                        {l.cambios.includes(c.campo) ? <DeA c={c} a={l.antes[c.campo]} b={l.despues[c.campo]} /> : celda(c, base[c.campo])}
                      </td>
                    ))}
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      ))}
    </section>
  );
}

function Detalle({ id, onCerrar, movil }) {
  const toast = useToast();
  const { sesion } = useSesion();
  const [d, setD] = useState(null);
  useAlActivar(() => { api.get(`${URL}/${id}`).then((r) => setD(r.data)).catch((e) => { toast(e.message, 'error'); onCerrar(); }); });
  useEffect(() => {
    // Escape cierra el detalle, salvo que este abierta la vista previa de la nota (ella se cierra primero)
    const t = (e) => e.key === 'Escape' && !document.querySelector('.vp-fondo') && onCerrar();
    window.addEventListener('keydown', t);
    const ov = document.body.style.overflow;
    document.body.style.overflow = 'hidden'; // sin scroll de fondo
    return () => { window.removeEventListener('keydown', t); document.body.style.overflow = ov; };
  }, [onCerrar]);
  const r = d?.registro;
  const eliminado = Number(r?.Operacion) === 2;

  // en el body (portal): ninguna animacion de la pagina desplaza la ventana
  return createPortal(
    <div className={`au-fondo ${movil ? 'movil' : ''}`} onMouseDown={onCerrar}>
      <div className={`au-ventana ${movil ? 'movil' : ''} ${eliminado ? 'elim' : 'modif'}`} role="dialog" aria-modal="true" aria-label="Detalle de auditoría" onMouseDown={(e) => e.stopPropagation()}>
        <header className="au-ventana-cab">
          {movil && <button className="btn-icono" onClick={onCerrar} aria-label="Volver"><Icono n="prev" /></button>}
          <div className="au-ventana-tit">
            <h3>{r ? `${r.NombreModulo} N° ${r.DocumentoId}` : 'Cargando…'} {r && <BadgeOperacion op={r.Operacion} />}</h3>
            {r && <p>{r.Descripcion || ''}{r.FechaDocumento ? ` · Documento del ${fechaTxt(r.FechaDocumento)}` : ''}</p>}
          </div>
          {!movil && <button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button>}
        </header>
        {!d ? (
          <div className="au-cargando"><span className="esq-barra" style={{ width: '60%' }} /><span className="esq-barra" style={{ width: '85%' }} /><span className="esq-barra" style={{ width: '40%' }} /></div>
        ) : (
          <div className="au-ventana-cuerpo">
            <aside className="au-lado"><PanelDonde r={r} /></aside>
            <div className="au-principal">
              {r.TotalAntes != null && (
                <div className={`au-resumen ${eliminado ? 'elim' : ''}`}>
                  <span>{eliminado ? 'Total eliminado' : 'Total'}</span>
                  <TotalCambio f={r} />
                </div>
              )}
              <Cabecera cabecera={d.cabecera} eliminado={eliminado} />
              <Lineas d={d} eliminado={eliminado} movil={movil} />
            </div>
          </div>
        )}
        <footer className="au-ventana-pie">
          <span><Icono n="lock" size={13} /> Solo lectura: la auditoría no se puede modificar ni borrar.</span>
          <div className="au-pie-acc">
            {d && <button className="btn btn-sec" onClick={() => imprimirAuditoria(d, sesion?.gs_user || '')} title="Nota del documento con toda su información: imprimir, PDF o compartir"><Icono n="printer" size={15} /> Imprimir / PDF</button>}
            <button className="btn btn-pri" onClick={onCerrar}>Cerrar</button>
          </div>
        </footer>
      </div>
    </div>,
    document.body
  );
}

// ---------- Lista en tarjetas (celular) ----------
function ListaMovil({ filas, cargando, vacio, onAbrir, onImprimir, imprimiendo }) {
  const [texto, setTexto] = useState('');
  const [cuantos, setCuantos] = useState(POR_PAGINA_MOVIL);
  useEffect(() => setCuantos(POR_PAGINA_MOVIL), [filas, texto]);
  const filtradas = useMemo(() => {
    const w = palabrasDe(texto);
    if (!w.length) return filas;
    return filas.filter((f) => coincide([f.NombreModulo, f.DocumentoId, f.Descripcion, f.Usuario, f.Equipo, f.Ip, f.NombreOperacion].join(' '), w));
  }, [filas, texto]);
  return (
    <>
      <div className="buscar au-buscar">
        <Icono n="search" size={16} />
        <input value={texto} onChange={(e) => setTexto(e.target.value)} placeholder="Buscar documento, usuario, equipo…" aria-label="Buscar" />
        {texto && <button type="button" className="btn-icono" onClick={() => setTexto('')} aria-label="Limpiar"><Icono n="x" size={15} /></button>}
      </div>
      <button type="button" className="btn btn-sec au-imprimir-movil" onClick={() => onImprimir(filtradas)} disabled={!filtradas.length || imprimiendo}>
        <Icono n="printer" size={16} /> {imprimiendo ? 'Armando reporte…' : `Imprimir / PDF con detalle (${filtradas.length})`}
      </button>
      {cargando && !filas.length && Array.from({ length: 4 }, (_, i) => <div key={i} className="au-tarjeta esq" aria-hidden="true"><span className="esq-barra" style={{ width: '70%' }} /><span className="esq-barra" style={{ width: '45%' }} /></div>)}
      {!cargando && !filtradas.length && <p className="au-vacio-lista"><Icono n="info" size={16} /> {texto ? 'Ningún registro coincide con la búsqueda' : vacio}</p>}
      <ul className="au-tarjetas">
        {filtradas.slice(0, cuantos).map((f) => {
          const elim = Number(f.Operacion) === 2;
          return (
            <li key={f.Id}>
              <button type="button" className={`au-tarjeta ${elim ? 'elim' : 'modif'}`} onClick={() => onAbrir(f.Id)}>
                <span className="au-tarjeta-ic"><Icono n={elim ? 'trash' : 'pencil'} size={16} /></span>
                <span className="au-tarjeta-cuerpo">
                  <span className="au-tarjeta-l1"><b>{f.NombreModulo} N° {f.DocumentoId}</b><time>{diaTxt(f.Fecha)} · {horaTxt(f.Fecha)}</time></span>
                  {f.Descripcion && <span className="au-tarjeta-desc">{f.Descripcion}</span>}
                  <span className="au-tarjeta-l3">
                    <span><Icono n="user" size={12} /> {f.Usuario || '—'}</span>
                    <span><Icono n={f.Origen === 'Web' ? 'globe' : 'monitor'} size={12} /> {f.Equipo || f.Origen}</span>
                  </span>
                  <span className="au-tarjeta-l4"><TotalCambio f={f} />{f.Ubicacion && <span className={`au-chip ${f.Ubicacion.aproximada ? 'aprox' : 'gps'}`}><Icono n="pin" size={11} /> {ubicTxt(f.Ubicacion)}</span>}</span>
                </span>
              </button>
            </li>
          );
        })}
      </ul>
      {filtradas.length > cuantos && (
        <button type="button" className="btn btn-sec au-mas" onClick={() => setCuantos((c) => c + POR_PAGINA_MOVIL)}>Ver más ({filtradas.length - cuantos})</button>
      )}
    </>
  );
}

export default function Tec_Auditoria() {
  const toast = useToast();
  const movil = useEsMovil();
  const [f, setF] = useState({ desde: haceDias(7), hasta: hoy(), modulo: 0, usuario: '', documento: '' });
  const [operacion, setOperacion] = useState(0);
  const [usuarios, setUsuarios] = useState([]);
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(false);
  const [abierto, setAbierto] = useState(null);
  const [verFiltros, setVerFiltros] = useState(false);
  const [imprimiendo, setImprimiendo] = useState(false);
  const { sesion } = useSesion();
  const cerrar = useCallback(() => setAbierto(null), []);
  const set = (k) => (e) => setF((x) => ({ ...x, [k]: e.target.value }));

  const generar = async (filtros = f) => {
    if (filtros.desde > filtros.hasta) { toast('La Fecha Desde Debe Ser Menor Que la Fecha Hasta', 'error'); return; }
    setCargando(true);
    try {
      const q = new URLSearchParams({ desde: filtros.desde, hasta: filtros.hasta, modulo: filtros.modulo, usuario: filtros.usuario, documento: filtros.documento || '' });
      setDatos((await api.get(`${URL}?${q}`)).data);
      setVerFiltros(false);
    } catch (e) { toast(e.message, 'error'); } finally { setCargando(false); }
  };
  useAlActivar(() => {
    api.get(`${URL}/usuarios`).then((r) => setUsuarios(r.data)).catch(() => {});
    generar();
  });

  const filas = useMemo(() => (datos?.filas || []).filter((x) => !operacion || Number(x.Operacion) === operacion), [datos, operacion]);
  const r = datos?.resumen;
  const conteo = { 0: r?.total ?? 0, 1: r?.modificados ?? 0, 2: r?.eliminados ?? 0 };
  const rangoTxt = `${f.desde.split('-').reverse().join('/')} – ${f.hasta.split('-').reverse().join('/')}`;
  const extras = [Number(f.modulo) ? MODULOS.find((m) => m.id === Number(f.modulo))?.texto : '', f.usuario, f.documento ? `N° ${f.documento}` : ''].filter(Boolean);
  const vacio = datos ? 'Sin registros para los filtros elegidos' : 'Elija el rango y pulse Generar';

  // Reporte de los registros que se ven (filtros + operacion + busqueda), cada uno con todo su detalle
  const imprimirDetallado = async (lista) => {
    if (!lista.length || imprimiendo) return;
    setImprimiendo(true);
    try {
      const q = new URLSearchParams({ desde: f.desde, hasta: f.hasta, modulo: f.modulo, usuario: f.usuario, operacion, ids: lista.map((x) => x.Id).join(',') });
      const d = (await api.get(`${URL}/reporte?${q}`)).data;
      const que = operacion === 2 ? 'documentos eliminados' : operacion === 1 ? 'documentos modificados' : 'documentos eliminados y modificados';
      imprimirReporteAuditoria(d, {
        titulo: `Auditoría: ${que}`, usuario: sesion?.gs_user || '',
        filtros: [['Del:', rangoTxt], ['Módulo:', MODULOS.find((m) => m.id === Number(f.modulo))?.texto || 'Todos'], ['Usuario:', f.usuario || 'Todos']],
      });
    } catch (e) { toast(e.message, 'error'); } finally { setImprimiendo(false); }
  };

  const segmento = (
    <div className="aud-seg au-seg" role="tablist" aria-label="Operación">
      {OPERACIONES.map((o) => (
        <button key={o.id} type="button" role="tab" aria-selected={operacion === o.id} className={operacion === o.id ? 'activo' : ''} onClick={() => setOperacion(o.id)}>
          {!movil && <Icono n={o.icono} size={14} />} {o.texto} <em>{conteo[o.id]}</em>
        </button>
      ))}
    </div>
  );

  const filtros = (
    <form className={`au-filtros ${movil ? 'movil' : ''}`} onSubmit={(e) => { e.preventDefault(); generar(); }}>
      <label><span>Desde</span><input type="date" className="control" value={f.desde} max={f.hasta} onChange={set('desde')} /></label>
      <label><span>Hasta</span><input type="date" className="control" value={f.hasta} min={f.desde} onChange={set('hasta')} /></label>
      <label><span>Módulo</span>
        <select className="control" value={f.modulo} onChange={set('modulo')}>{MODULOS.map((m) => <option key={m.id} value={m.id}>{m.texto}</option>)}</select>
      </label>
      <label className="au-f-usuario"><span>Usuario</span>
        <select className="control" value={f.usuario} onChange={set('usuario')}>
          <option value="">Todos</option>
          {usuarios.map((u) => <option key={u} value={u}>{u}</option>)}
        </select>
      </label>
      <label><span>N° documento</span><input type="number" inputMode="numeric" min="1" className="control" placeholder="Todos" value={f.documento} onChange={set('documento')} /></label>
      <button type="submit" className="btn btn-pri" disabled={cargando}><Icono n="search" size={16} /> Generar</button>
    </form>
  );

  return (
    <div className={`entrada au-pagina ${movil ? 'movil' : ''}`}>
      <header className="au-cab">
        <span className="au-cab-ic"><Icono n="shield" size={20} /></span>
        <div>
          <h1>Auditoría de registros</h1>
          <p>Ventas, compras y movimientos modificados o eliminados: quién, cuándo y desde dónde.</p>
        </div>
      </header>

      {movil ? (
        <>
          <button type="button" className="au-filtros-res" onClick={() => setVerFiltros((v) => !v)} aria-expanded={verFiltros}>
            <Icono n="history" size={16} />
            <span><b>{rangoTxt}</b><small>{extras.length ? extras.join(' · ') : 'Todos los módulos y usuarios'}</small></span>
            <span className="au-filtros-btn">Filtros <Icono n="chevronDown" size={14} /></span>
          </button>
          {verFiltros && filtros}
          <div className="au-seg-movil">{segmento}</div>
          <ListaMovil filas={filas} cargando={cargando} vacio={vacio} onAbrir={setAbierto} onImprimir={imprimirDetallado} imprimiendo={imprimiendo} />
        </>
      ) : (
        <>
          <div className="tarjeta au-barra">{filtros}</div>
          <div className="tarjeta au-lista">
            <TablaReporte
              titulo="AUDITORÍA DE REGISTROS"
              subtitulo={`Del ${rangoTxt} · ${OPERACIONES.find((o) => o.id === operacion).texto} · ${plural(r?.usuarios ?? 0, 'usuario')}`}
              herramientas={segmento}
              columnas={COLUMNAS} filas={filas} cargando={cargando}
              claseFila={(x) => (Number(x.Operacion) === 2 ? 'aud-fila-eliminado' : '')}
              onFila={(x) => setAbierto(x.Id)}
              onImprimir={imprimirDetallado}
              vacio={vacio}
            />
          </div>
        </>
      )}

      {abierto && <Detalle id={abierto} movil={movil} onCerrar={cerrar} />}
    </div>
  );
}
