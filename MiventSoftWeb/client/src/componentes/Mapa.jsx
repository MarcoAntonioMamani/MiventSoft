// Mapa (reemplaza GMap.NET del escritorio): Leaflet
//  - Centro por defecto: Potosí (Plaza 10 de Noviembre) cuando no hay punto GPS.
//  - Capas sin clave de API: Calles (OpenStreetMap), Detallado (OSM Humanitario), Satélite (Esri) + escala.
//  - Pin: mismo diseño y colores que los iconos del escritorio (iconmarker / iconselected /
//    iconunselected), redibujado en vector para que no se pixele ni se deforme.
import { useEffect, useMemo, useRef } from 'react';
import { LayersControl, MapContainer, Marker, ScaleControl, TileLayer, useMap, useMapEvents } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

export const CENTRO_DEFECTO = [-19.5888, -65.7534]; // Potosí

// Todas sin clave de API (CARTO ahora la exige y muestra "API KEY REQUIRED")
const CAPAS = [
  { nombre: 'Calles', url: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png', attribution: '&copy; OpenStreetMap', maxZoom: 19 },
  { nombre: 'Detallado', url: 'https://{s}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png', attribution: '&copy; OpenStreetMap &copy; HOT', maxZoom: 19 },
  { nombre: 'Satélite', url: 'https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', attribution: '&copy; Esri', maxZoom: 19 },
];
// Capa inicial: client/.env VITE_MAPA=calles|detallado|satelite (por defecto calles)
const INICIAL = { calles: 0, osm: 0, detallado: 1, satelite: 2 }[String(import.meta.env.VITE_MAPA || 'calles').toLowerCase()] ?? 0;

export function ClicMapa({ activo, onPunto }) {
  const map = useMapEvents({ click: (e) => activo && onPunto([e.latlng.lat, e.latlng.lng]) });
  // cursor de mira cuando se puede marcar
  useEffect(() => { map.getContainer().classList.toggle('mapa-marcando', !!activo); }, [activo, map]);
  return null;
}

// Centra en un punto o ajusta a varios puntos
export function Encuadrar({ puntos, zoom = 15 }) {
  const map = useMap();
  const clave = JSON.stringify(puntos);
  useEffect(() => {
    if (!puntos?.length) return;
    // sin animacion: si el pin cambia de posicion a mitad de una animacion, Leaflet lo deja desplazado
    map.stop();
    if (puntos.length === 1) map.setView(puntos[0], Math.max(map.getZoom(), zoom), { animate: false });
    else map.fitBounds(puntos, { padding: [30, 30], maxZoom: 17, animate: false });
  }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps
  // El contenedor puede cambiar de tamano (responsive) -> recalcular
  useEffect(() => {
    const t = setTimeout(() => map.invalidateSize(), 150);
    return () => clearTimeout(t);
  });
  return null;
}

// ---------- Pin (iconmarker / iconselected / iconunselected del escritorio) ----------
const COLORES = { marcado: ['#e53935', '#b71c1c'], seleccionado: ['#e53935', '#b71c1c'], normal: ['#00897b', '#00695c'], empresa: ['#1e63d6', '#0f3f99'] };
const GLIFOS = {
  // tienda (cliente)
  tienda: '<path d="M13.2 15.2h11.6l-1-3.4H14.2Z" fill="currentColor"/><path d="M13.6 16.6h10.8v7.2H13.6Z" fill="none" stroke="currentColor" stroke-width="1.6"/><path d="M17.3 23.8v-3.6h3.4v3.6" fill="none" stroke="currentColor" stroke-width="1.6"/>',
  // edificio (empresa)
  empresa: '<path d="M14 25V13.4h7v11.6M21 17h3v8M16.2 16h2.6M16.2 19h2.6M16.2 22h2.6" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/>',
};
const iconos = new Map();
export function iconoPin(tipo = 'marcado', glifo = 'tienda') {
  const k = `${tipo}|${glifo}`;
  if (iconos.has(k)) return iconos.get(k);
  const [c1, c2] = COLORES[tipo] || COLORES.marcado;
  const grande = tipo === 'seleccionado' || tipo === 'marcado' || tipo === 'empresa';
  const [w, h] = grande ? [38, 50] : [30, 40];
  const id = `g${k.replace(/\W/g, '')}`;
  const svg = `<svg viewBox="0 0 38 50" width="${w}" height="${h}" aria-hidden="true">
    <defs><linearGradient id="${id}" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="${c1}"/><stop offset="1" stop-color="${c2}"/></linearGradient></defs>
    <path d="M19 1.5C9.3 1.5 1.5 9.2 1.5 18.8 1.5 31.6 19 48 19 48s17.5-16.4 17.5-29.2C36.5 9.2 28.7 1.5 19 1.5Z" fill="url(#${id})" stroke="#fff" stroke-width="2"/>
    <circle cx="19" cy="18.8" r="10.5" fill="#fff"/>
    <g style="color:${c2}">${GLIFOS[glifo] || GLIFOS.tienda}</g>
  </svg>`;
  const ic = L.divIcon({
    className: `pin-mapa pin-${tipo}`,
    html: `${tipo === 'seleccionado' ? '<span class="pin-pulso"></span>' : ''}<span class="pin-sombra"></span>${svg}`,
    iconSize: [w, h], iconAnchor: [w / 2, h - 1], tooltipAnchor: [0, -h + 6], popupAnchor: [0, -h + 6],
  });
  iconos.set(k, ic);
  return ic;
}
export function Pin({ posicion, tipo = 'marcado', glifo = 'tienda', onClick, arrastrable, onMover, children, zIndexOffset }) {
  const icon = useMemo(() => iconoPin(tipo, glifo), [tipo, glifo]);
  const handlers = useMemo(() => ({
    ...(onClick ? { click: onClick } : {}),
    ...(onMover ? { dragend: (e) => { const p = e.target.getLatLng(); onMover([p.lat, p.lng]); } } : {}),
  }), [onClick, onMover]);
  const ref = useRef(null);
  const map = useMap();
  // Al cambiar de registro (Siguiente / Anterior) se fija la posicion y se recoloca al terminar cualquier movimiento del mapa
  useEffect(() => {
    const mk = ref.current;
    if (!mk) return undefined;
    mk.setLatLng(posicion);
    const recolocar = () => { mk.setLatLng(posicion); mk.update(); };
    map.on('moveend zoomend viewreset resize', recolocar);
    const t = setTimeout(recolocar, 300);
    return () => { clearTimeout(t); map.off('moveend zoomend viewreset resize', recolocar); };
  }, [posicion[0], posicion[1], map]); // eslint-disable-line react-hooks/exhaustive-deps
  return <Marker ref={ref} position={posicion} icon={icon} draggable={!!arrastrable} eventHandlers={handlers} zIndexOffset={zIndexOffset ?? (tipo === 'normal' ? 0 : 1000)} keyboard={false}>{children}</Marker>;
}

export function Mapa({ children, className = 'mapa', zoom = 14 }) {
  return (
    <MapContainer center={CENTRO_DEFECTO} zoom={zoom} className={`${className} mapa-estilo`} scrollWheelZoom preferCanvas>
      <LayersControl position="topright">
        {CAPAS.map((c, i) => (
          <LayersControl.BaseLayer key={c.nombre} name={c.nombre} checked={i === INICIAL}>
            <TileLayer attribution={c.attribution} url={c.url} subdomains={c.subdomains || 'abc'} maxZoom={c.maxZoom} />
          </LayersControl.BaseLayer>
        ))}
      </LayersControl>
      <ScaleControl position="bottomright" imperial={false} />
      {children}
    </MapContainer>
  );
}
