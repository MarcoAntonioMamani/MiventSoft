// =====================================================================
//  Pedidos del celular observados (solo web, script 09)   btnPedidosObservados
//  Pedidos que la app no pudo subir (sin stock, rechazados, sin productos, fallas repetidas).
//  El administrador corrige las cantidades y lo graba (mismo SP que la API del celular) o lo descarta;
//  el celular ve el cambio en su proxima sincronizacion.
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import { createPortal } from 'react-dom';
import { api } from '../../api/http';
import { useToast } from '../../componentes/Toast';
import TablaReporte, { fechaTxt } from '../../componentes/TablaReporte';
import Icono from '../../componentes/Icono';
import { usePermisos } from '../../global/Sesion';
import { titulo as tit } from '../../global/programas';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/distribucion/pedidos-observados';
export const EVENTO_CAMBIO = 'pedidos-observados:cambio'; // la notificacion de la barra superior se actualiza
const dos = (n) => String(n).padStart(2, '0');
const isoLocal = (d) => `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`;
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const cant = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });
const red2 = (n) => Math.round((Number(n) + Number.EPSILON) * 100) / 100;

const ESTADOS = [
  { v: '-1', t: 'Todos' }, { v: '0', t: 'Pendientes' }, { v: '1', t: 'Corregidos en web' }, { v: '2', t: 'Entraron desde el celular' }, { v: '3', t: 'Descartados' },
];
const CLASE_ESTADO = { 0: 'obs-pendiente', 1: 'obs-web', 2: 'obs-celular', 3: 'obs-descartado' };
const TXT_ESTADO = { 0: 'Pendiente', 1: 'Corregido en web', 2: 'Entró desde el celular', 3: 'Descartado' };
const TXT_TIPO = { 1: 'Sin stock', 2: 'Rechazado', 3: 'Sin productos', 4: 'No se pudo enviar' };

const COLUMNAS = [
  { campo: 'EstadoTxt', titulo: 'Estado', render: (x) => <span className={`obs-estado ${CLASE_ESTADO[x.Estado] || ''}`}>{TXT_ESTADO[x.Estado] || x.EstadoTxt}</span> },
  { campo: 'Tipo', titulo: 'Observación', render: (x) => <span className={`obs-tipo obs-tipo-${x.TipoObservacion}`}>{TXT_TIPO[x.TipoObservacion] || x.Tipo}</span> },
  { campo: 'FechaActualizacion', titulo: 'Último intento', render: (x) => <span className="cel-fh">{String(x.FechaActualizacion || '').slice(11)}<small>{fechaTxt(String(x.FechaActualizacion || '').slice(0, 10))}</small></span> },
  { campo: 'Preventista', titulo: 'Preventista', render: (x) => tit(x.Preventista) },
  { campo: 'Cliente', titulo: 'Cliente', render: (x) => tit(x.Cliente) },
  { campo: 'Total', titulo: 'Total (Bs)', tipo: 'bs' },
  { campo: 'Motivo', titulo: 'Motivo', render: (x) => <span className="obs-motivo" title={x.Motivo}>{x.Motivo}</span> },
  { campo: 'Intentos', titulo: 'Intentos', tipo: 'entero' },
  { campo: 'ModeloCelular', titulo: 'Celular' },
  { campo: 'VentaId', titulo: 'Venta', tipo: 'entero' },
];

export default function Tec_PedidosObservados() {
  const toast = useToast();
  const [f, setF] = useState(() => { const h = new Date(); const d = new Date(); d.setDate(d.getDate() - 7); return { desde: isoLocal(d), hasta: isoLocal(h), estado: '-1' }; });
  const [filas, setFilas] = useState([]);
  const [cargando, setCargando] = useState(false);
  const [sel, setSel] = useState(null);

  const buscar = async (silencioso = false) => {
    setCargando(true);
    try {
      const r = await api.get(`${URL}?desde=${f.desde}&hasta=${f.hasta}&estado=${f.estado}`);
      setFilas(r.data);
      if (!silencioso && !r.data.length) toast('No hay pedidos observados con los filtros elegidos', 'info');
      // se elige el primer pendiente (o se mantiene el elegido si sigue en la lista)
      setSel((s) => (s && r.data.some((x) => x.Id === s) ? s : (r.data.find((x) => x.Estado === 0) || r.data[0])?.Id ?? null));
    } catch (e) { toast(e.message, 'error'); } finally { setCargando(false); }
  };
  useAlActivar(() => buscar(true));
  const pendientes = filas.filter((x) => x.Estado === 0).length;
  const alCambiar = () => { buscar(true); window.dispatchEvent(new Event(EVENTO_CAMBIO)); };

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo">
          <span className="modelo-icono"><Icono n="alert" size={22} /></span>
          <div><h1>Pedidos observados del celular</h1><p>Pedidos que no pudieron sincronizar: corregir y grabar, o descartar</p></div>
        </div>
      </header>
      <div className="modelo-cuerpo">
        <div className="filtros-lista filtros-reporte">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={f.desde} max={f.hasta} onChange={(e) => e.target.value && setF((o) => ({ ...o, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={f.hasta} min={f.desde} onChange={(e) => e.target.value && setF((o) => ({ ...o, hasta: e.target.value }))} /></label>
          <label className="filtro"><span>Estado</span>
            <select className="control" value={f.estado} onChange={(e) => setF((o) => ({ ...o, estado: e.target.value }))}>
              {ESTADOS.map((x) => <option key={x.v} value={x.v}>{x.t}</option>)}
            </select>
          </label>
          <button className="btn btn-pri filtro-boton" onClick={() => buscar()} disabled={cargando}>{cargando ? <span className="spinner" /> : <Icono n="search" size={16} />} Buscar</button>
        </div>
        <p className="obs-ayuda">Los <b>pendientes</b> se muestran siempre; los resueltos, del rango de fechas. Si el pedido entra desde el celular, se marca solo.</p>
        <div className="obs-layout">
          <div className="obs-tabla">
            <TablaReporte titulo={`PEDIDOS OBSERVADOS (${pendientes} PENDIENTE${pendientes === 1 ? '' : 'S'})`} subtitulo={`Del ${fechaTxt(f.desde)} al ${fechaTxt(f.hasta)}`}
              columnas={COLUMNAS} filas={filas} cargando={cargando} claveFila="Id" vacio="No hay pedidos observados"
              onFila={(x) => setSel(x.Id)} filaSel={sel} claseFila={(x) => (x.Estado === 0 ? 'obs-fila-pendiente' : '')} />
          </div>
          {sel && <Detalle key={sel} id={sel} onCambio={alCambiar} />}
        </div>
      </div>
    </section>
  );
}

// ---------- Panel de una observacion: correccion (pendiente) o resumen (resuelta) ----------
function Detalle({ id, onCambio }) {
  const toast = useToast();
  const permisos = usePermisos();
  const [d, setD] = useState(null);
  const [error, setError] = useState('');
  const [cants, setCants] = useState({});
  const [grabando, setGrabando] = useState(false);
  const [resultado, setResultado] = useState(null); // { tipo: 'ok'|'error', mensaje, productos }
  const [descartar, setDescartar] = useState(false);
  const [nota, setNota] = useState('');

  const cargar = () => api.get(`${URL}/${id}`).then((r) => {
    setD(r.data);
    setCants(Object.fromEntries(r.data.detalle.map((x) => [x.obcprod, String(x.obpcant)])));
    setError('');
  }).catch((e) => setError(e.message));
  useEffect(() => { cargar(); }, [id]); // eslint-disable-line react-hooks/exhaustive-deps

  const lineas = useMemo(() => (d?.detalle || []).map((x) => {
    const c = Number(String(cants[x.obcprod] ?? '').replace(',', '.'));
    const valida = Number.isFinite(c) && c >= 0 && String(cants[x.obcprod] ?? '').trim() !== '';
    const n = valida ? c : 0;
    const obptot = red2(x.obpbase * n);
    const descuento = x.obpcant > 0 ? red2(x.descuento * n / x.obpcant) : 0;
    return { ...x, nueva: n, valida, total: red2(obptot - descuento), descuentoNuevo: descuento, falta: x.stock !== null && n > x.stock };
  }), [d, cants]);

  if (error) return <aside className="obs-panel"><p className="txt-neg">{error}</p></aside>;
  if (!d) return <aside className="obs-panel"><span className="spinner" /></aside>;

  const pendiente = d.Estado === 0;
  const total = red2(lineas.reduce((s, x) => s + x.total, 0));
  const incremento = Math.max(0, Number(d.Incremento ?? d.pedido?.incremento ?? 0));
  const invalidas = lineas.some((x) => !x.valida);
  const vacio = lineas.every((x) => x.nueva <= 0);
  const faltan = lineas.filter((x) => x.falta).length;
  const puedeGrabar = pendiente && d.pedidoLegible && d.TipoObservacion !== 3 && lineas.length > 0;

  const grabar = async () => {
    setGrabando(true);
    setResultado(null);
    try {
      const r = await api.post(`${URL}/${id}/grabar`, { detalle: lineas.map((x) => ({ obcprod: x.obcprod, obpcant: x.nueva })), observacion: nota });
      setResultado({ tipo: 'ok', mensaje: r.mensaje + (r.aviso ? ` ${r.aviso}` : '') });
      toast(r.mensaje, 'ok', 6000);
      await cargar();
      onCambio();
    } catch (e) {
      setResultado({ tipo: 'error', mensaje: e.message, productos: e.datos?.stockNegativo });
      cargar(); // stock actualizado
    } finally { setGrabando(false); }
  };

  return (
    <aside className="obs-panel">
      <div className="obs-panel-cab">
        <div>
          <span className={`obs-estado ${CLASE_ESTADO[d.Estado] || ''}`}>{TXT_ESTADO[d.Estado]}</span>{' '}
          <span className={`obs-tipo obs-tipo-${d.TipoObservacion}`}>{TXT_TIPO[d.TipoObservacion] || d.Tipo}</span>
          <h3>{tit(d.Cliente) || 'Cliente sin nombre'}</h3>
          <small>Preventista {tit(d.Preventista) || `#${d.PersonalId}`} · Pedido del {fechaTxt(d.FechaPedido)}{d.pedido?.oahora ? ` ${d.pedido.oahora}` : ''} · {d.ModeloCelular || 'celular sin modelo'}</small>
          <small className="obs-codigo">Código del celular: {d.CodigoGenerado} · {d.Intentos} intento{d.Intentos === 1 ? '' : 's'}</small>
        </div>
      </div>
      <div className="obs-motivo-caja"><Icono n="alert" size={15} /><span>{d.Motivo || 'Sin detalle del motivo'}</span></div>
      {d.pedido?.oaobs && <p className="obs-nota-pedido"><b>Obs. del pedido:</b> {d.pedido.oaobs}</p>}

      {!pendiente && (
        <div className="obs-resuelto">
          {d.Estado === 1 && <p><Icono n="check" size={15} /> Corregido y grabado en la web como <b>venta {d.VentaId}</b> por {tit(d.UsuarioResuelve)} el {fechaTxt(d.FechaResuelto)}.</p>}
          {d.Estado === 2 && <p><Icono n="phone" size={15} /> Entró desde el celular como <b>venta {d.VentaId}</b> ({fechaTxt(d.FechaResuelto)}).</p>}
          {d.Estado === 3 && <p><Icono n="trash" size={15} /> Descartado por {tit(d.UsuarioResuelve)} el {fechaTxt(d.FechaResuelto)}.</p>}
          {d.ObsResuelve && <p><b>Nota:</b> {d.ObsResuelve}</p>}
          <p className="obs-ayuda">El celular verá este cambio en su próxima sincronización.</p>
        </div>
      )}

      {!d.pedidoLegible && <p className="txt-neg">No se pudo leer el pedido que mandó el celular. Solo se puede descartar.</p>}
      {lineas.length > 0 && (
        <div className="obs-productos">
          <table className="grid detalle-tabla obs-tabla-prod">
            <thead><tr><th>Producto</th><th className="num">Pidió</th><th className="num">Stock</th><th className="num">{pendiente ? 'Cantidad' : 'Cant.'}</th><th className="num">Precio</th><th className="num">Total</th></tr></thead>
            <tbody>
              {lineas.map((x) => (
                <tr key={x.obcprod} className={pendiente && x.falta ? 'obs-falta' : ''}>
                  <td>{x.cadesc || `Producto ${x.obcprod}`}{x.descuento > 0 && <small className="obs-desc">Desc. Bs {bs(x.descuentoNuevo)}</small>}</td>
                  <td className="num">{cant(x.obpcant)}</td>
                  <td className="num">{x.stock === null ? '—' : cant(x.stock)}{pendiente && x.falta && <small className="txt-neg">no alcanza</small>}</td>
                  <td className="num">
                    {pendiente && puedeGrabar ? (
                      <input className={`control num obs-cant ${x.valida ? '' : 'invalida'}`} type="text" inputMode="decimal" value={cants[x.obcprod] ?? ''} aria-label={`Cantidad de ${x.cadesc}`}
                        onChange={(e) => /^\d*[.,]?\d{0,2}$/.test(e.target.value) && setCants((c) => ({ ...c, [x.obcprod]: e.target.value }))} onFocus={(e) => e.target.select()} />
                    ) : cant(x.nueva)}
                  </td>
                  <td className="num">{bs(x.obpbase)}</td>
                  <td className="num">{bs(x.total)}</td>
                </tr>
              ))}
            </tbody>
          </table>
          <div className="obs-totales">
            <span>Total <b>Bs {bs(total)}</b></span>
            <span>Incremento <b>Bs {bs(incremento)}</b></span>
            <span className="obs-total-final">Total con incremento <b>Bs {bs(total + incremento)}</b></span>
          </div>
          {pendiente && puedeGrabar && <p className="obs-ayuda">Cantidad 0 = quitar el producto. Precio y descuento quedan como los tomó el preventista.</p>}
        </div>
      )}

      {resultado && (
        <div className={`obs-resultado ${resultado.tipo}`} role={resultado.tipo === 'error' ? 'alert' : 'status'}>
          <b>{resultado.tipo === 'ok' ? 'Pedido grabado' : 'No se grabó'}</b>
          <span>{resultado.mensaje}</span>
          {resultado.productos?.length > 0 && (
            <ul>{resultado.productos.map((p) => <li key={p.Producto}>{p.Producto}: hay {cant(p.stock)}, se pidió {cant(p.cantidad)}</li>)}</ul>
          )}
        </div>
      )}

      {pendiente && (
        <>
          {puedeGrabar && <label className="obs-nota"><span>Nota (opcional)</span><input className="control" maxLength={300} value={nota} onChange={(e) => setNota(e.target.value)} placeholder="Ej. se ajustó al stock disponible" /></label>}
          <div className="obs-acciones">
            <button className="btn btn-sec" onClick={() => setDescartar(true)} disabled={grabando || !permisos.modificar}><Icono n="trash" size={15} /> Descartar</button>
            {puedeGrabar && (
              <button className="btn btn-pri" onClick={grabar} disabled={grabando || invalidas || vacio || !permisos.modificar}
                title={faltan ? 'Hay productos sin stock suficiente: el servidor lo rechazará' : ''}>
                {grabando ? <span className="spinner" /> : <Icono n="save" size={15} />} Grabar pedido corregido
              </button>
            )}
          </div>
          {puedeGrabar && faltan > 0 && <p className="obs-ayuda txt-neg">{faltan} producto{faltan === 1 ? '' : 's'} sin stock suficiente: baje la cantidad o póngala en 0.</p>}
          {!permisos.modificar && <p className="obs-ayuda">Su rol solo puede ver este programa.</p>}
        </>
      )}
      {descartar && <DialogoDescartar id={id} onCerrar={(hecho) => { setDescartar(false); if (hecho) { toast('Pedido descartado. El celular lo verá en su próxima sincronización.', 'ok'); cargar(); onCambio(); } }} />}
    </aside>
  );
}

function DialogoDescartar({ id, onCerrar }) {
  const toast = useToast();
  const [motivo, setMotivo] = useState('');
  const [enviando, setEnviando] = useState(false);
  const enviar = async () => {
    if (!motivo.trim()) { toast('Escriba el motivo del descarte', 'error'); return; }
    setEnviando(true);
    try { await api.post(`${URL}/${id}/descartar`, { motivo: motivo.trim() }); onCerrar(true); } catch (e) { toast(e.message, 'error'); setEnviando(false); }
  };
  return createPortal(
    <div className="modal-fondo" onMouseDown={() => onCerrar(false)}>
      <div className="modal modal-sm" role="dialog" aria-modal="true" aria-label="Descartar pedido" onMouseDown={(e) => e.stopPropagation()} onKeyDown={(e) => e.key === 'Escape' && onCerrar(false)}>
        <div className="modal-cab"><h3>Descartar pedido</h3><button className="btn-icono" onClick={() => onCerrar(false)} aria-label="Cerrar"><Icono n="x" /></button></div>
        <p className="obs-ayuda" style={{ textAlign: 'left' }}>El pedido no se grabará. En el celular quedará como descartado, con este motivo.</p>
        <textarea autoFocus className="control obs-motivo-txt" rows={3} maxLength={300} value={motivo} onChange={(e) => setMotivo(e.target.value)} placeholder="Motivo (obligatorio)" />
        <div className="modal-acciones">
          <button className="btn btn-sec" onClick={() => onCerrar(false)}>Cancelar</button>
          <button className="btn btn-peligro" onClick={enviar} disabled={enviando || !motivo.trim()}>{enviando ? <span className="spinner" /> : <Icono n="trash" size={15} />} Descartar</button>
        </div>
      </div>
    </div>,
    document.body,
  );
}
