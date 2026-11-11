// =====================================================================
//  Tec_Clientes.vb -> Gestion De Clientes   (SP: MAM_Clientes)
// =====================================================================
import { useRef, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, ComboLibreria, SwitchButton, fechaCorta } from '../../componentes/Controles';
import { Mapa, ClicMapa, Encuadrar, Pin, CENTRO_DEFECTO } from '../../componentes/Mapa';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';

const URL = '/api/configuracion/clientes';
const VACIO = { Id: '', CodigoExterno: '', NombreCliente: '', DireccionCliente: '', Telefono: '', TipoDocumento: '', NroDocumento: '',
  RazonSocial: '', Nit: '', Estado: true, Latitud: 0, Longitud: 0, PrecioCategoriaId: '', ZonaId: '' };

export default function Tec_Clientes() {
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [combos, setCombos] = useState({ tiposDocumento: [], precios: [], zonas: [] });
  const combosRef = useRef(null);
  // Al corregir un campo se quita su mensaje de error
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };

  // Los combos se cargan una vez y sirven tambien para mostrar el nombre de la zona en el buscador
  const cargarCombos = async () => {
    if (!combosRef.current) {
      combosRef.current = (await api.get(`${URL}/combos`)).data;
      setCombos(combosRef.current);
    }
    return combosRef.current;
  };

  const nuevaLibreria = (idClasif, clave) => async (descripcion) => {
    const r = await api.post(`${URL}/libreria/${idClasif}`, { descripcion });
    combosRef.current = { ...combosRef.current, [clave]: r.data };
    setCombos(combosRef.current);
    return r;
  };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => {
      const [c, r] = await Promise.all([cargarCombos(), api.get(URL)]);
      const zonas = Object.fromEntries(c.zonas.map((z) => [String(z.Id), z.NombreZona]));
      return r.data.map((f) => ({ ...f, Zona: zonas[String(f.ZonaId)] || '', Fecha: fechaCorta(f.FechaIngreso) }));
    },
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombreCliente', true, 'Cliente', 220),
      new Celda('DireccionCliente', true, 'Dirección', 200),
      new Celda('Telefono', true, 'Teléfono', 110),
      new Celda('NroDocumento', true, 'Nro. Doc.', 110),
      new Celda('Zona', true, 'Zona', 120),
      new Celda('Estado', true, 'Estado', 90, 'ACTIVO', 'bool'),
      new Celda('Fecha', true, 'Ingreso', 100),
    ],
    _PMOLimpiar: () => {
      const c = combosRef.current || combos;
      setForm({ ...VACIO, TipoDocumento: c.tiposDocumento[0]?.cnnum ?? '',
        PrecioCategoriaId: c.precios[0]?.Id ?? '', ZonaId: c.zonas[0]?.Id ?? '' });
    },
    _PMOMostrarRegistro: (f) => setForm(Object.fromEntries(Object.keys(VACIO).map((k) => [k, f[k] ?? VACIO[k]]))),
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombreCliente.trim()) e.NombreCliente = 'Ingrese Nombre del Cliente';
      if (!form.ZonaId) e.ZonaId = 'Seleccione una Zona';
      if (!form.PrecioCategoriaId) e.PrecioCategoriaId = 'Seleccione Categoria de Precio';
      if (!form.TipoDocumento) e.TipoDocumento = 'Seleccione Tipo de Documento';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, { ...form, Id: undefined }),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, { ...form, Id: undefined }),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el cliente ${f.NombreCliente} ?`,
  });
  const ro = !m.habilitado;
  const tienePunto = Number(form.Latitud) !== 0 && Number(form.Longitud) !== 0;
  const pos = [Number(form.Latitud), Number(form.Longitud)];

  const miUbicacion = () => navigator.geolocation?.getCurrentPosition(
    (p) => setForm((f) => ({ ...f, Latitud: p.coords.latitude, Longitud: p.coords.longitude })),
    () => {}, { enableHighAccuracy: true, timeout: 10000 });

  return (
    <ModeloF1 m={m} titulo="Clientes" subtitulo="Gestion De Clientes" icono="store">
      <div className="registro-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del cliente</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Código externo"><input className="control" value={form.CodigoExterno} onChange={set('CodigoExterno')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="Estado"><SwitchButton valor={!!form.Estado} onChange={set('Estado')} deshabilitado={ro} /></Campo>
            <Campo etiqueta="Nombre del cliente" ancho={3} error={errores.NombreCliente} requerido>
              <input className="control" value={form.NombreCliente} onChange={set('NombreCliente')} readOnly={ro} maxLength={200} />
            </Campo>
            <Campo etiqueta="Dirección" ancho={2}><input className="control" value={form.DireccionCliente} onChange={set('DireccionCliente')} readOnly={ro} maxLength={300} /></Campo>
            <Campo etiqueta="Teléfono"><input className="control" type="tel" value={form.Telefono} onChange={set('Telefono')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="Zona" error={errores.ZonaId} requerido>
              <ComboMulti dt={combos.zonas} valor={form.ZonaId} onChange={set('ZonaId')} id="Id" nameId="Código" descripcionId="NombreZona" descripcion="Zona" deshabilitado={ro} error={errores.ZonaId} />
            </Campo>
            <Campo etiqueta="Categoría de precio" error={errores.PrecioCategoriaId} requerido>
              <ComboMulti dt={combos.precios} valor={form.PrecioCategoriaId} onChange={set('PrecioCategoriaId')} id="Id" nameId="Código" descripcionId="Descripcion" descripcion="Precio" deshabilitado={ro} error={errores.PrecioCategoriaId} />
            </Campo>
          </div>
          <div className="sep" />
          <h2 className="tarjeta-tit">Facturación</h2>
          <div className="campos">
            <Campo etiqueta="Tipo de documento" error={errores.TipoDocumento} requerido>
              <ComboLibreria dt={combos.tiposDocumento} valor={form.TipoDocumento} onChange={set('TipoDocumento')} titulo="Tipo de Documento" deshabilitado={ro} error={errores.TipoDocumento} onNuevo={nuevaLibreria(8, 'tiposDocumento')} />
            </Campo>
            <Campo etiqueta="Nro. documento"><input className="control" value={form.NroDocumento} onChange={set('NroDocumento')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="NIT"><input className="control" value={form.Nit} onChange={set('Nit')} readOnly={ro} maxLength={50} inputMode="numeric" /></Campo>
            <Campo etiqueta="Razón social" ancho={3}><input className="control" value={form.RazonSocial} onChange={set('RazonSocial')} readOnly={ro} maxLength={200} /></Campo>
          </div>
        </div>
        <div className="tarjeta tarjeta-mapa">
          <div className="mapa-cab">
            <h2 className="tarjeta-tit"><Icono n="pin" size={16} /> Ubicación</h2>
            <span className="mapa-coord">{tienePunto ? `${pos[0].toFixed(6)}, ${pos[1].toFixed(6)}` : 'Sin ubicación'}</span>
          </div>
          {!ro && (
            <div className="barra mapa-barra">
              <span className="ayuda-tip">Toque el mapa o arrastre el pin para marcar la ubicación.</span>
              <button type="button" className="btn btn-sec btn-sm" onClick={miUbicacion}><Icono n="pin" size={14} /> Mi ubicación</button>
            </div>
          )}
          <Mapa>
            <ClicMapa activo={!ro} onPunto={([la, ln]) => setForm((f) => ({ ...f, Latitud: la, Longitud: ln }))} />
            <Encuadrar puntos={[tienePunto ? pos : CENTRO_DEFECTO]} zoom={tienePunto ? 15 : 14} />
            {tienePunto && <Pin posicion={pos} tipo="marcado" arrastrable={!ro} onMover={(p) => setForm((f) => ({ ...f, Latitud: p[0], Longitud: p[1] }))} />}
          </Mapa>
        </div>
      </div>
    </ModeloF1>
  );
}
