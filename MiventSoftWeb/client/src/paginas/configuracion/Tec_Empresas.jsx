// =====================================================================
//  Tec_Empresas.vb -> Gestion De Empresas   (SP: MAM_Empresas)
//  El GMap del escritorio se reemplaza por Leaflet + OpenStreetMap.
// =====================================================================
import { useState } from 'react';
import { Mapa, ClicMapa, Encuadrar, Pin, CENTRO_DEFECTO } from '../../componentes/Mapa';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ImagenSelector } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { api, urlImagen } from '../../api/http';
import Icono from '../../componentes/Icono';

const URL = '/api/configuracion/empresas';
const CENTRO = CENTRO_DEFECTO; // P_IniciarMap
const VACIO = { Id: '', Nombre: '', Descripcion: '', Propietario: '', Telefono: '', Nit: '', Ciudad: '', Direccion: '', Latitud: 0, Longitud: 0, Imagen: 'Default.jpg' };

export default function Tec_Empresas() {
  const [form, setForm] = useState(VACIO);
  const [archivo, setArchivo] = useState(null);
  const [errores, setErrores] = useState({});
  // Al corregir un campo se quita su mensaje de error
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (e) => { quitarError(k); setForm((f) => ({ ...f, [k]: e.target.value })); };

  const _datos = () => {
    const fd = new FormData();
    ['Nombre', 'Descripcion', 'Propietario', 'Telefono', 'Nit', 'Ciudad', 'Direccion', 'Latitud', 'Longitud'].forEach((k) => fd.append(k, form[k] ?? ''));
    fd.append('ImagenActual', form.Imagen || 'Default.jpg');
    if (archivo) fd.append('imagen', archivo);
    return fd;
  };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data,
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('Nombre', true, 'Nombre Empresa', 240),
      new Celda('Descripcion', true, 'Descripcion', 200),
      new Celda('Latitud', false), new Celda('Longitud', false), new Celda('Imagen', false),
      new Celda('Propietario', true, 'Propietario', 160),
      new Celda('Telefono', false), new Celda('Nit', false), new Celda('Ciudad', false),
      new Celda('Direccion', true, 'Direccion', 200),
    ],
    _PMOLimpiar: () => { setForm(VACIO); setArchivo(null); },
    _PMOMostrarRegistro: (f) => {
      setForm({ ...VACIO, ...Object.fromEntries(Object.keys(VACIO).map((k) => [k, f[k] ?? VACIO[k]])) });
      setArchivo(null);
    },
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.Nombre.trim()) e.Nombre = 'Ingrese Nombre de Empresa';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, _datos()),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, _datos()),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar la Empresa ${f.Nombre} ?`,
  });
  const ro = !m.habilitado;
  const tienePunto = Number(form.Latitud) !== 0 && Number(form.Longitud) !== 0;
  const pos = tienePunto ? [Number(form.Latitud), Number(form.Longitud)] : CENTRO;

  return (
    <ModeloF1 m={m} titulo="Empresas" subtitulo="Gestion De Empresas" icono="building">
      <div className="registro-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos de la empresa</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Nombre empresa" ancho={2} error={errores.Nombre} requerido>
              <input className="control" value={form.Nombre} onChange={set('Nombre')} readOnly={ro} maxLength={150} />
            </Campo>
            <Campo etiqueta="Descripción" ancho={3}><input className="control" value={form.Descripcion} onChange={set('Descripcion')} readOnly={ro} maxLength={300} /></Campo>
            <Campo etiqueta="Propietario" ancho={2}><input className="control" value={form.Propietario} onChange={set('Propietario')} readOnly={ro} /></Campo>
            <Campo etiqueta="NIT"><input className="control" value={form.Nit} onChange={set('Nit')} readOnly={ro} inputMode="numeric" /></Campo>
            <Campo etiqueta="Teléfono"><input className="control" value={form.Telefono} onChange={set('Telefono')} readOnly={ro} /></Campo>
            <Campo etiqueta="Ciudad" ancho={2}><input className="control" value={form.Ciudad} onChange={set('Ciudad')} readOnly={ro} /></Campo>
            <Campo etiqueta="Dirección" ancho={3}><input className="control" value={form.Direccion} onChange={set('Direccion')} readOnly={ro} /></Campo>
          </div>
          <div className="sep" />
          <h2 className="tarjeta-tit">Logo</h2>
          <ImagenSelector key={`${form.Id}-${m.modo}`} src={urlImagen('empresa', form.Imagen)} deshabilitado={ro} onArchivo={setArchivo} />
        </div>
        <div className="tarjeta tarjeta-mapa">
          <div className="mapa-cab">
            <h2 className="tarjeta-tit"><Icono n="pin" size={16} /> Ubicación</h2>
            <span className="mapa-coord">{tienePunto ? `${Number(form.Latitud).toFixed(6)}, ${Number(form.Longitud).toFixed(6)}` : 'Sin ubicación'}</span>
          </div>
          {!ro && <p className="ayuda-tip">Haga clic en el mapa o arrastre el pin para marcar la ubicación.</p>}
          <Mapa>
            <ClicMapa activo={!ro} onPunto={([la, ln]) => setForm((f) => ({ ...f, Latitud: la, Longitud: ln }))} />
            <Encuadrar puntos={[pos]} />
            {tienePunto && <Pin posicion={pos} tipo="empresa" glifo="empresa" arrastrable={!ro} onMover={(p) => setForm((f) => ({ ...f, Latitud: p[0], Longitud: p[1] }))} />}
          </Mapa>
        </div>
      </div>
    </ModeloF1>
  );
}
