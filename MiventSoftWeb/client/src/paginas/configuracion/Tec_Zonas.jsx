// =====================================================================
//  Tec_Zonas.vb -> Gestion De Zonas   (SP: sp_Mam_Zonas)
//  "Ingresar Puntos" / "Dibujar Marcado" del escritorio -> modo dibujo en el mapa.
//  Solo se envian los puntos si se redibujo el poligono.
// =====================================================================
import { useRef, useState } from 'react';
import { CircleMarker, Polygon, Polyline } from 'react-leaflet';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo } from '../../componentes/Controles';
import { Mapa, ClicMapa, Encuadrar } from '../../componentes/Mapa';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';
import { useToast } from '../../componentes/Toast';
import Icono from '../../componentes/Icono';

const URL = '/api/configuracion/zonas';
const VACIO = { Id: '', NombreZona: '', DescripcionZona: '', Color: '#2C6AA8' };

export default function Tec_Zonas() {
  const toast = useToast();
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [puntos, setPuntos] = useState([]);
  const [dibujando, setDibujando] = useState(false);
  const [redibujado, setRedibujado] = useState(false);
  const pedido = useRef(0);
  // Al corregir un campo se quita su mensaje de error
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (e) => { quitarError(k); setForm((f) => ({ ...f, [k]: e.target.value })); };

  const cargarPuntos = async (id) => {
    const n = ++pedido.current;
    try {
      const r = await api.get(`${URL}/${id}/puntos`);
      if (n === pedido.current) setPuntos(r.data);
    } catch (e) { toast(e.message, 'error'); }
  };
  const reiniciarDibujo = () => { setDibujando(false); setRedibujado(false); };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data,
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('Color', false),
      new Celda('NombreZona', true, 'Zona', 220),
      new Celda('DescripcionZona', true, 'Descripción', 320),
    ],
    _PMOLimpiar: () => { setForm(VACIO); setPuntos([]); reiniciarDibujo(); },
    _PMONuevo: () => { setDibujando(true); setRedibujado(true); },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, NombreZona: f.NombreZona ?? '', DescripcionZona: f.DescripcionZona ?? '', Color: /^#[0-9a-f]{6}$/i.test(f.Color) ? f.Color : '#2C6AA8' });
      reiniciarDibujo();
      cargarPuntos(f.Id);
    },
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombreZona.trim()) e.NombreZona = 'Ingrese Nombre de Zona';
      if (redibujado && puntos.length < 3) e.puntos = 'Marque al menos 3 puntos en el mapa';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, { ...form, Id: undefined, puntos }),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, { ...form, Id: undefined, ...(redibujado ? { puntos } : {}) }),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar la Zona ${f.NombreZona} ?`,
  });
  const ro = !m.habilitado;

  const empezarDibujo = () => { setPuntos([]); setDibujando(true); setRedibujado(true); };
  const color = form.Color;

  return (
    <ModeloF1 m={m} titulo="Zonas" subtitulo="Gestion De Zonas de distribución" icono="map">
      <div className="registro-grid zonas-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos de la zona</h2>
          <div className="campos campos-2">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Color">
              <div className="control-con-btn">
                <input type="color" className="control control-color" value={color} onChange={set('Color')} disabled={ro} aria-label="Color de la zona" />
                <input className="control" value={color.toUpperCase()} readOnly />
              </div>
            </Campo>
            <Campo etiqueta="Nombre de la zona" ancho={2} error={errores.NombreZona} requerido>
              <input className="control" value={form.NombreZona} onChange={set('NombreZona')} readOnly={ro} maxLength={100} />
            </Campo>
            <Campo etiqueta="Descripción" ancho={2}>
              <textarea className="control" rows={3} value={form.DescripcionZona} onChange={set('DescripcionZona')} readOnly={ro} maxLength={300} />
            </Campo>
          </div>
          <div className="sep" />
          <div className="resumen-zona">
            <span className="muestra-color" style={{ background: color }} />
            <div><b>{puntos.length}</b> puntos en el polígono{redibujado && !ro && <em className="etq-cambio">redibujado</em>}</div>
          </div>
          {errores.puntos && <p className="campo-error">{errores.puntos}</p>}
        </div>
        <div className="tarjeta tarjeta-mapa">
          <div className="mapa-cab"><h2 className="tarjeta-tit"><Icono n="map" size={16} /> Polígono</h2></div>
          {!ro && (
            <div className="barra mapa-barra">
              {!dibujando ? (
                <button type="button" className="btn btn-pri btn-sm" onClick={empezarDibujo}><Icono n="pencil" size={14} /> {puntos.length ? 'Redibujar zona' : 'Dibujar zona'}</button>
              ) : (
                <>
                  <span className="ayuda-tip">Toque el mapa para agregar puntos.</span>
                  <button type="button" className="btn btn-sec btn-sm" disabled={!puntos.length} onClick={() => setPuntos((p) => p.slice(0, -1))}><Icono n="undo" size={14} /> Deshacer</button>
                  <button type="button" className="btn btn-sec btn-sm" disabled={!puntos.length} onClick={() => setPuntos([])}><Icono n="trash" size={14} /> Limpiar</button>
                  <button type="button" className="btn btn-ok btn-sm" disabled={puntos.length < 3} onClick={() => setDibujando(false)}><Icono n="check" size={14} /> Terminar</button>
                </>
              )}
            </div>
          )}
          <Mapa className="mapa mapa-alto">
            <ClicMapa activo={!ro && dibujando} onPunto={(p) => { quitarError('puntos'); setPuntos((x) => [...x, p]); }} />
            {puntos.length > 0 && !dibujando && <Encuadrar puntos={puntos} />}
            {dibujando ? (
              <>
                {puntos.length > 1 && <Polyline positions={puntos} pathOptions={{ color, dashArray: '6 6' }} />}
                {puntos.map((p, i) => <CircleMarker key={i} center={p} radius={6} pathOptions={{ color: '#fff', weight: 2, fillColor: i === 0 ? '#15803d' : color, fillOpacity: 1 }} />)}
              </>
            ) : (
              puntos.length >= 3 && <Polygon positions={puntos} pathOptions={{ color: '#d31c70', weight: 1.5, fillColor: color, fillOpacity: 0.3 }} />
            )}
          </Mapa>
        </div>
      </div>
    </ModeloF1>
  );
}
