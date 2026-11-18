// =====================================================================
//  Panel principal: entregas por distribuidor y mapa del lugar donde se entrego cada pedido (app movil)
//  Datos: /api/dashboard/entregas-mapa (MAM_VentasRegistroMovil @tipo 2, script 07). Numeradas en el orden
//  en que se entregaron (hora del celular). Mismo diseño que "Pedidos por vendedor".
// =====================================================================
import { useEffect, useMemo, useRef, useState } from 'react';
import { Marker, Polyline, Tooltip } from 'react-leaflet';
import { Mapa, Encuadrar, CENTRO_DEFECTO } from './Mapa';
import { api } from '../api/http';
import { titulo } from '../global/programas';
import { fmtBs, fmtN } from './Graficos';
import Icono from './Icono';
import Ampliable, { BotonAmpliar } from './Ampliable';
import { fmtKm, fmtMin, txtTramo, claseEstado, iconoOrden, IrA } from './PedidosVendedor';

const detalle = (p) => [
  p.hora || '--:--',
  `N° ${p.id}`,
  titulo(p.estado),
  p.punto ? (p.precision !== null ? `±${fmtN(p.precision)} m` : '') : 'sin ubicación',
  p.sinInternet ? `llegó ${fmtMin(p.sinInternet)} después (sin internet)` : '',
].filter(Boolean).join(' · ');

export default function EntregasRepartidor({ fecha, recarga }) {
  const [elegido, setElegido] = useState('');
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');
  const [sel, setSel] = useState(null);
  const pedido = useRef(0);
  const lista = useRef(null);

  useEffect(() => {
    if (!sel || !lista.current) return;
    lista.current.querySelector(`[data-id="${sel.id}"]`)?.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
  }, [sel]);

  useEffect(() => {
    const n = ++pedido.current;
    setCargando(true);
    const q = new URLSearchParams({ fecha, ...(elegido ? { repartidor: elegido } : {}) });
    api.get(`/api/dashboard/entregas-mapa?${q}`)
      .then((r) => { if (n === pedido.current) { setDatos(r.data); setError(''); } })
      .catch((e) => { if (n === pedido.current) setError(e.message); })
      .finally(() => { if (n === pedido.current) setCargando(false); });
  }, [fecha, elegido, recarga]);

  useEffect(() => { setSel(null); }, [fecha, datos?.repartidor]);

  const entregas = datos?.entregas || [];
  const conPunto = useMemo(() => entregas.filter((p) => p.punto), [entregas]);
  const ruta = useMemo(() => conPunto.map((p) => p.punto), [conPunto]);
  const actual = datos?.repartidores?.find((x) => x.id === datos.repartidor);
  const rec = datos?.recorrido;
  const elegir = (p) => p.punto && setSel(p.id === sel?.id ? null : p);

  // Sin el script 07 no hay de donde sacar la ubicacion: el panel no se muestra
  if (datos && datos.instalado === false) return null;

  return (
    <Ampliable>{(completo, alternar) => (
    <section className="panel span-12 pv-panel">
      <header className="panel-cab">
        <div>
          <h2>Entregas por distribuidor</h2>
          <p>Ubicación donde se entregó cada pedido (app del celular) · numeradas en el orden en que se entregaron</p>
        </div>
        <div className="pv-cab-der">
          {actual && (
            <div className="pv-resumen">
              <span><b>{fmtN(actual.entregas)}</b> entregas</span>
              <span><b>{fmtBs(actual.monto, 0)}</b> entregado</span>
              {actual.anuladas > 0 && <span><b>{fmtN(actual.anuladas)}</b> anuladas</span>}
              <span><b>{fmtN(conPunto.length)}</b> con ubicación</span>
            </div>
          )}
          <BotonAmpliar completo={completo} onClick={alternar} />
        </div>
      </header>
      <div className="panel-cuerpo">
        {error && <div className="alerta"><Icono n="alert" size={16} /> {error}</div>}
        {datos && !datos.repartidores.length ? (
          <div className="panel-vacio"><Icono n="truck" size={26} /><span>Sin entregas registradas desde el celular en el día</span></div>
        ) : (
          <>
            <div className="pv-vendedores" role="tablist" aria-label="Distribuidores">
              {(datos?.repartidores || []).map((x) => (
                <button key={x.id} type="button" role="tab" aria-selected={x.id === datos.repartidor}
                  className={`pv-vend ${x.id === datos.repartidor ? 'activo' : ''}`} onClick={() => setElegido(x.id)}>
                  <Icono n="truck" size={15} /><span>{titulo(x.nombre)}</span><b>{fmtN(x.entregas)}</b>
                </button>
              ))}
            </div>
            {/* recorrido aproximado: suma de distancias en linea recta entre entregas consecutivas con GPS */}
            {rec && (
              <div className="pv-recorrido" title="Aproximado: distancia en línea recta entre una entrega y la siguiente. Por calles suele ser mayor.">
                <div><Icono n="map" size={18} /><span>Recorrido aprox.</span><b>{fmtKm(rec.km)}</b><small>en línea recta</small></div>
                <div><Icono n="history" size={18} /><span>Tiempo</span><b>{rec.minutos !== null ? fmtMin(rec.minutos) : '--'}</b><small>{rec.desde || '--:--'} → {rec.hasta || '--:--'}</small></div>
                <div><Icono n="target" size={18} /><span>Tramos</span><b>{fmtN(rec.tramos)}</b><small>de la entrega 1 a la última</small></div>
                <div><Icono n="truck" size={18} /><span>Velocidad prom.</span><b>{rec.kmh !== null ? `${fmtN(rec.kmh)} km/h` : '--'}</b></div>
              </div>
            )}
            <div className={`pv-cuerpo ${cargando ? 'recargando' : ''}`}>
              <ol className="pv-lista" ref={lista}>
                {!datos && <li className="vacio"><span className="spinner" /> Cargando...</li>}
                {entregas.map((p) => (
                  <li key={p.id} data-id={p.id}>
                    <button type="button" className={`${sel?.id === p.id ? 'activo' : ''} ${p.punto ? '' : 'sin-punto'}`} onClick={() => elegir(p)}
                      title={p.punto ? 'Ver en el mapa' : 'La entrega no tiene ubicación registrada'}>
                      <span className={`pv-num ${claseEstado(p.estado)}`}>{p.orden}</span>
                      <span className="pv-txt"><b>{titulo(p.cliente)}</b>
                        <small>{detalle(p)}</small>
                        {txtTramo(p) && <small className="pv-tramo">↳ {txtTramo(p)}</small>}
                      </span>
                      <em>{fmtBs(p.total)}</em>
                    </button>
                  </li>
                ))}
                {datos && !entregas.length && <li className="vacio">Sin entregas</li>}
              </ol>
              <div className="pv-mapa">
                <Mapa className="mapa mapa-pedidos">
                  <Encuadrar puntos={ruta.length ? ruta : [CENTRO_DEFECTO]} zoom={14} />
                  <IrA punto={sel?.punto || null} />
                  {ruta.length > 1 && <Polyline positions={ruta} pathOptions={{ color: '#ffffff', weight: 7, opacity: 0.85 }} />}
                  {ruta.length > 1 && <Polyline positions={ruta} pathOptions={{ color: '#047857', weight: 3.5, opacity: 0.95, dashArray: '8 7' }} />}
                  {conPunto.map((p) => {
                    const activo = sel?.id === p.id;
                    const etiqueta = <><b>{p.orden}. {titulo(p.cliente)}</b><small>{p.hora} · {fmtBs(p.total)}{p.estado === 'ANULADO' ? ' · anulado' : ''}</small></>;
                    return (
                      <Marker key={p.id} position={p.punto} icon={iconoOrden(p.orden, p.estado, activo)} zIndexOffset={activo ? 3000 : p.orden}
                        eventHandlers={{ click: () => setSel(p) }} keyboard={false}>
                        {activo
                          ? <Tooltip key="fijo" direction="top" permanent className={`pv-etiqueta activa ${claseEstado(p.estado)}`}>{etiqueta}{txtTramo(p) && <small>{txtTramo(p)}</small>}</Tooltip>
                          : <Tooltip key="hover" direction="top" className="pv-etiqueta">{etiqueta}</Tooltip>}
                      </Marker>
                    );
                  })}
                </Mapa>
                <div className="pv-leyenda">
                  <span><i className="ped-entregado" /> Entregado</span><span><i className="ped-anulado" /> Anulado</span>
                </div>
              </div>
            </div>
          </>
        )}
      </div>
    </section>
    )}</Ampliable>
  );
}
