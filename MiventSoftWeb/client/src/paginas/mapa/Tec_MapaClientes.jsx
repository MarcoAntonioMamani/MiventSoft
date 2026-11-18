// =====================================================================
//  Tec_Mapaclientes.vb -> Ubicación de clientes  (SP: MAM_Clientes @tipo 6 / 7)
//  Mapa con todos los clientes (o de una zona); clic en el punto o en la lista = seleccionar y centrar.
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import { CircleMarker, Tooltip, useMap } from 'react-leaflet';
import { Mapa, Encuadrar, Pin, CENTRO_DEFECTO } from '../../componentes/Mapa';
import { ComboMulti } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { palabrasDe, coincide } from '../../global/buscar';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/mapa/clientes';
// Con muchos clientes se dibujan puntos livianos (canvas) y solo el seleccionado lleva pin
const MAX_PINES = 800;

function IrA({ punto }) {
  const map = useMap();
  useEffect(() => { if (punto) map.setView(punto, Math.max(map.getZoom(), 17)); }, [punto]); // eslint-disable-line react-hooks/exhaustive-deps
  return null;
}

export default function Tec_MapaClientes() {
  const toast = useToast();
  const [zonas, setZonas] = useState([]);
  const [zona, setZona] = useState(-1);
  const [clientes, setClientes] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [sel, setSel] = useState(null);
  const [q, setQ] = useState('');

  useAlActivar(() => { api.get(`${URL}/zonas`).then((r) => setZonas([{ Id: -1, NombreZona: 'TODAS LAS ZONAS' }, ...r.data])).catch((e) => toast(e.message, 'error')); });
  useEffect(() => {
    setCargando(true); setSel(null);
    api.get(`${URL}${Number(zona) > 0 ? `?zona=${zona}` : ''}`).then((r) => setClientes(r.data)).catch((e) => toast(e.message, 'error')).finally(() => setCargando(false));
  }, [zona, toast]);
  // al volver a la pestaña: clientes actualizados de la BD (sin perder la zona elegida)
  useAlActivar(() => { api.get(`${URL}${Number(zona) > 0 ? `?zona=${zona}` : ''}`).then((r) => setClientes(r.data)).catch((e) => toast(e.message, 'error')); }, { alMontar: false });

  const conPunto = useMemo(() => clientes.filter((c) => c.Latitud && c.Longitud), [clientes]);
  const puntos = useMemo(() => conPunto.map((c) => [c.Latitud, c.Longitud]), [conPunto]);
  const filtrados = useMemo(() => { const w = palabrasDe(q); return clientes.filter((c) => coincide(`${c.Id} ${c.NombreCliente} ${c.Telefono}`, w)); }, [clientes, q]);
  const elegir = (c) => {
    if (!c.Latitud || !c.Longitud) { toast('NO EXISTE DATOS PARA ESTE CLIENTE', 'error'); return; }
    setSel(c);
  };

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo">
          <span className="modelo-icono"><Icono n="globe" size={22} /></span>
          <div><h1>Ubicación de clientes</h1><p>Mapa Clientes</p></div>
        </div>
      </header>
      <div className="modelo-cuerpo mapa-cli">
        <aside className="tarjeta mc-lista">
          <div className="filtro"><span className="campo-etq">Zona</span>
            <ComboMulti dt={zonas} valor={zona} onChange={setZona} id="Id" nameId="Cód." descripcionId="NombreZona" descripcion="Zona" />
          </div>
          <div className="mc-stats">
            <div><b>{clientes.length}</b><span>clientes</span></div>
            <div className="ok"><b>{conPunto.length}</b><span>en el mapa</span></div>
            <div className="sin"><b>{clientes.length - conPunto.length}</b><span>sin ubicación</span></div>
          </div>
          <div className="buscar"><Icono n="search" size={16} /><input className="control" placeholder="Buscar cliente..." value={q} onChange={(e) => setQ(e.target.value)} aria-label="Buscar cliente" /></div>
          <ul className="mc-items">
            {cargando && <li className="vacio"><span className="spinner" /> Cargando...</li>}
            {!cargando && filtrados.slice(0, 300).map((c) => (
              <li key={c.Id}><button type="button" className={`${sel?.Id === c.Id ? 'activo' : ''} ${!c.Latitud || !c.Longitud ? 'sin-punto' : ''}`} onClick={() => elegir(c)}>
                <Icono n="pin" size={16} /><span><b>{tit(c.NombreCliente)}</b><small>Cód. {c.Id}{c.Telefono ? ` · ${c.Telefono}` : ''}{!c.Latitud || !c.Longitud ? ' · sin ubicación' : ''}</small></span>
              </button></li>
            ))}
            {!cargando && !filtrados.length && <li className="vacio">Sin clientes</li>}
          </ul>
        </aside>
        <div className="tarjeta mc-mapa">
          <Mapa className="mapa mapa-clientes">
            <Encuadrar puntos={puntos.length ? puntos : [CENTRO_DEFECTO]} zoom={13} />
            <IrA punto={sel ? [sel.Latitud, sel.Longitud] : null} />
            {conPunto.filter((c) => c.Id !== sel?.Id).map((c) => (conPunto.length <= MAX_PINES ? (
              <Pin key={c.Id} posicion={[c.Latitud, c.Longitud]} tipo="normal" onClick={() => setSel(c)}>
                <Tooltip direction="top" className="tip-mapa"><b>{tit(c.NombreCliente)}</b>{c.Telefono ? <span>{c.Telefono}</span> : null}</Tooltip>
              </Pin>
            ) : (
              <CircleMarker key={c.Id} center={[c.Latitud, c.Longitud]} radius={6} eventHandlers={{ click: () => setSel(c) }}
                pathOptions={{ color: '#fff', weight: 2, fillColor: '#00897b', fillOpacity: 0.95 }}>
                <Tooltip direction="top" offset={[0, -6]} className="tip-mapa"><b>{tit(c.NombreCliente)}</b>{c.Telefono ? <span>{c.Telefono}</span> : null}</Tooltip>
              </CircleMarker>
            )))}
            {sel && (
              <Pin posicion={[sel.Latitud, sel.Longitud]} tipo="seleccionado" zIndexOffset={2000}>
                <Tooltip direction="top" className="tip-mapa"><b>{tit(sel.NombreCliente)}</b>{sel.Telefono ? <span>{sel.Telefono}</span> : null}</Tooltip>
              </Pin>
            )}
          </Mapa>
          {sel && (
            <div className="mc-ficha">
              <b>{tit(sel.NombreCliente)}</b>
              <span>Cód. {sel.Id}{sel.Telefono ? ` · Tel. ${sel.Telefono}` : ''}</span>
              {sel.Direccion && <span>{sel.Direccion}</span>}
              <small>{sel.Latitud.toFixed(6)}, {sel.Longitud.toFixed(6)}</small>
              <button className="btn-icono" onClick={() => setSel(null)} aria-label="Cerrar"><Icono n="x" size={16} /></button>
            </div>
          )}
        </div>
      </div>
    </section>
  );
}
