// =====================================================================
//  Panel principal: pedidos por vendedor y mapa del lugar donde se registro cada pedido (app movil)
//  Datos: /api/dashboard/pedidos-mapa (MAM_Ventas @tipo 3 + @tipo 23). Iconos numerados en orden de creacion.
// =====================================================================
import { useEffect, useMemo, useRef, useState } from 'react';
import { Marker, Polyline, Tooltip, useMap } from 'react-leaflet';
import L from 'leaflet';
import { Mapa, Encuadrar, CENTRO_DEFECTO } from './Mapa';
import { api } from '../api/http';
import { titulo } from '../global/programas';
import { fmtBs, fmtN } from './Graficos';
import Icono from './Icono';

// 0.85 -> "850 m", 12.4 -> "12,4 km" ; 95 -> "1 h 35 min"
const fmtKm = (n) => (n < 1 ? `${Math.round(n * 1000)} m` : `${Number(n).toLocaleString('es-BO', { maximumFractionDigits: 1 })} km`);
const fmtMin = (m) => (m < 60 ? `${m} min` : `${Math.floor(m / 60)} h${m % 60 ? ` ${m % 60} min` : ''}`);
const txtTramo = (p) => (p.tramoKm === null || p.tramoKm === undefined ? '' : `+${fmtKm(p.tramoKm)}${p.tramoMin !== null ? ` · ${fmtMin(p.tramoMin)}` : ''} desde el anterior`);

const claseEstado =(e) => (/ANUL/.test(e) ? 'ped-anulado' : /ENTREG/.test(e) ? 'ped-entregado' : /PEND/.test(e) ? 'ped-pendiente' : 'ped-asignado');

// Icono numerado (se cachea por numero/estado/seleccion)
const iconos = new Map();
function iconoOrden(n, estado, activo) {
  const k = `${n}|${estado}|${activo ? 1 : 0}`;
  if (iconos.has(k)) return iconos.get(k);
  const t = activo ? 44 : 32;
  const ic = L.divIcon({
    className: `pin-orden ${claseEstado(estado)}${activo ? ' activo' : ''}`,
    // el seleccionado lleva un anillo que late para llamar la atencion
    html: `${activo ? '<i class="pin-orden-pulso"></i>' : ''}<span>${n}</span>`,
    iconSize: [t, t], iconAnchor: [t / 2, t / 2], tooltipAnchor: [0, -t / 2 - 4],
  });
  iconos.set(k, ic);
  return ic;
}

function IrA({ punto }) {
  const map = useMap();
  useEffect(() => { if (punto) map.setView(punto, Math.max(map.getZoom(), 17)); }, [punto]); // eslint-disable-line react-hooks/exhaustive-deps
  return null;
}

export default function PedidosVendedor({ fecha, vendedor, recarga }) {
  const [elegido, setElegido] = useState(vendedor || '');
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');
  const [sel, setSel] = useState(null);
  const pedido = useRef(0);
  const lista = useRef(null);

  // Al elegir un punto en el mapa, la lista baja hasta ese pedido
  useEffect(() => {
    if (!sel || !lista.current) return;
    lista.current.querySelector(`[data-id="${sel.id}"]`)?.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
  }, [sel]);

  // El filtro de vendedor del panel manda
  useEffect(() => { setElegido(vendedor || ''); }, [vendedor]);

  useEffect(() => {
    const n = ++pedido.current;
    setCargando(true);
    const q = new URLSearchParams({ fecha, ...(elegido ? { vendedor: elegido } : {}) });
    api.get(`/api/dashboard/pedidos-mapa?${q}`)
      .then((r) => { if (n === pedido.current) { setDatos(r.data); setError(''); } })
      .catch((e) => { if (n === pedido.current) setError(e.message); })
      .finally(() => { if (n === pedido.current) setCargando(false); });
  }, [fecha, elegido, recarga]);

  useEffect(() => { setSel(null); }, [fecha, datos?.vendedor]);

  const pedidos = datos?.pedidos || [];
  const conPunto = useMemo(() => pedidos.filter((p) => p.punto), [pedidos]);
  const ruta = useMemo(() => conPunto.map((p) => p.punto), [conPunto]);
  const actual = datos?.vendedores?.find((x) => x.id === datos.vendedor);
  const rec = datos?.recorrido;
  const elegir = (p) => p.punto && setSel(p.id === sel?.id ? null : p);

  return (
    <section className="panel span-12 pv-panel">
      <header className="panel-cab">
        <div>
          <h2>Pedidos por vendedor</h2>
          <p>Ubicación donde se registró cada pedido · numerados en el orden en que se crearon</p>
        </div>
        {actual && (
          <div className="pv-resumen">
            <span><b>{fmtN(actual.pedidos)}</b> pedidos</span>
            <span><b>{fmtBs(actual.monto, 0)}</b> válido</span>
            <span><b>{fmtN(conPunto.length)}</b> con ubicación</span>
          </div>
        )}
      </header>
      <div className="panel-cuerpo">
        {error && <div className="alerta"><Icono n="alert" size={16} /> {error}</div>}
        {datos && !datos.vendedores.length ? (
          <div className="panel-vacio"><Icono n="map" size={26} /><span>Sin pedidos en el día</span></div>
        ) : (
          <>
            <div className="pv-vendedores" role="tablist" aria-label="Vendedores">
              {(datos?.vendedores || []).map((x) => (
                <button key={x.id} type="button" role="tab" aria-selected={x.id === datos.vendedor}
                  className={`pv-vend ${x.id === datos.vendedor ? 'activo' : ''}`} onClick={() => setElegido(x.id)}>
                  <Icono n="user" size={15} /><span>{titulo(x.nombre)}</span><b>{fmtN(x.pedidos)}</b>
                </button>
              ))}
            </div>
            {/* recorrido aproximado: suma de distancias en linea recta entre pedidos consecutivos con GPS */}
            {rec && (
              <div className="pv-recorrido" title="Aproximado: distancia en línea recta entre un pedido y el siguiente. Por calles suele ser mayor.">
                <div><Icono n="map" size={18} /><span>Recorrido aprox.</span><b>{fmtKm(rec.km)}</b><small>en línea recta</small></div>
                <div><Icono n="history" size={18} /><span>Tiempo</span><b>{rec.minutos !== null ? fmtMin(rec.minutos) : '--'}</b><small>{rec.desde || '--:--'} → {rec.hasta || '--:--'}</small></div>
                <div><Icono n="target" size={18} /><span>Tramos</span><b>{fmtN(rec.tramos)}</b><small>del punto 1 al último</small></div>
                <div><Icono n="truck" size={18} /><span>Velocidad prom.</span><b>{rec.kmh !== null ? `${fmtN(rec.kmh)} km/h` : '--'}</b></div>
              </div>
            )}
            <div className={`pv-cuerpo ${cargando ? 'recargando' : ''}`}>
              <ol className="pv-lista" ref={lista}>
                {!datos && <li className="vacio"><span className="spinner" /> Cargando...</li>}
                {pedidos.map((p) => (
                  <li key={p.id} data-id={p.id}>
                    <button type="button" className={`${sel?.id === p.id ? 'activo' : ''} ${p.punto ? '' : 'sin-punto'}`} onClick={() => elegir(p)}
                      title={p.punto ? 'Ver en el mapa' : 'El pedido no tiene ubicación registrada'}>
                      <span className={`pv-num ${claseEstado(p.estado)}`}>{p.orden}</span>
                      <span className="pv-txt"><b>{titulo(p.cliente)}</b>
                        <small>{p.hora || '--:--'} · N° {p.id} · {titulo(p.estado)}{p.punto ? (p.distanciaKm !== null ? ` · a ${fmtN(p.distanciaKm)} km del cliente` : '') : ' · sin ubicación'}</small>
                        {txtTramo(p) && <small className="pv-tramo">↳ {txtTramo(p)}</small>}
                      </span>
                      <em>{fmtBs(p.total)}</em>
                    </button>
                  </li>
                ))}
                {datos && !pedidos.length && <li className="vacio">Sin pedidos</li>}
              </ol>
              <div className="pv-mapa">
                <Mapa className="mapa mapa-pedidos">
                  <Encuadrar puntos={ruta.length ? ruta : [CENTRO_DEFECTO]} zoom={14} />
                  <IrA punto={sel?.punto || null} />
                  {/* recorrido: linea con borde blanco para que se vea sobre cualquier capa */}
                  {ruta.length > 1 && <Polyline positions={ruta} pathOptions={{ color: '#ffffff', weight: 7, opacity: 0.85 }} />}
                  {ruta.length > 1 && <Polyline positions={ruta} pathOptions={{ color: '#1d4ed8', weight: 3.5, opacity: 0.95, dashArray: '8 7' }} />}
                  {conPunto.map((p) => {
                    const activo = sel?.id === p.id;
                    return (
                      <Marker key={p.id} position={p.punto} icon={iconoOrden(p.orden, p.estado, activo)} zIndexOffset={activo ? 3000 : p.orden}
                        eventHandlers={{ click: () => setSel(p) }} keyboard={false}>
                        {/* seleccionado: nombre del cliente fijo arriba del punto; los demas al pasar el mouse */}
                        {activo
                          ? <Tooltip key="fijo" direction="top" permanent className={`pv-etiqueta activa ${claseEstado(p.estado)}`}><b>{p.orden}. {titulo(p.cliente)}</b><small>{p.hora} · {fmtBs(p.total)}</small>{txtTramo(p) && <small>{txtTramo(p)}</small>}</Tooltip>
                          :<Tooltip key="hover" direction="top" className="pv-etiqueta"><b>{p.orden}. {titulo(p.cliente)}</b><small>{p.hora} · {fmtBs(p.total)}</small></Tooltip>}
                      </Marker>
                    );
                  })}
                </Mapa>
                <div className="pv-leyenda">
                  <span><i className="ped-pendiente" /> Pendiente</span><span><i className="ped-asignado" /> Asignado</span>
                  <span><i className="ped-entregado" /> Entregado</span><span><i className="ped-anulado" /> Anulado</span>
                </div>
              </div>
            </div>
          </>
        )}
      </div>
    </section>
  );
}
