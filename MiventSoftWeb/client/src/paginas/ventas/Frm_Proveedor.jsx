// =====================================================================
//  Frm_Proveedor.vb -> Gestion De Proveedores   (SP: MAM_Proveedores)
//  Mejora: se agrega "Descripción" (VB la enviaba vacia y al modificar la borraba).
// =====================================================================
import { useEffect, useRef, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboLibreria, SwitchButton } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';

const URL = '/api/ventas/proveedores';
const VACIO = { Id: '', NombreProveedor: '', Direccion: '', Telefono01: '', Telefono02: '', Descripcion: '', TipoDocumento: '', NroDocumento: '', Estado: true };

export default function Frm_Proveedor() {
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [combos, setCombos] = useState({ tiposDocumento: [] });
  const combosRef = useRef(combos);
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };
  const setMay = (k) => (e) => { quitarError(k); setForm((f) => ({ ...f, [k]: e.target.value.toUpperCase() })); };

  useEffect(() => { api.get(`${URL}/combos`).then((r) => { combosRef.current = r.data; setCombos(r.data); }).catch(() => {}); }, []);

  const nuevaLibreria = (idClasif, clave) => async (descripcion) => {
    const r = await api.post(`${URL}/libreria/${idClasif}`, { descripcion });
    combosRef.current = { ...combosRef.current, [clave]: r.data };
    setCombos(combosRef.current);
    return r;
  };

  const _datos = () => ({
    NombreProveedor: form.NombreProveedor, Direccion: form.Direccion, Telefono01: form.Telefono01, Telefono02: form.Telefono02,
    Descripcion: form.Descripcion, TipoDocumento: form.TipoDocumento, NroDocumento: form.NroDocumento, Estado: form.Estado,
  });

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data,
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombreProveedor', true, 'Nombre Proveedor', 240),
      new Celda('Telefono01', true, 'Teléfono', 110),
      new Celda('NroDocumento', true, 'Nro Documento', 130),
      new Celda('Estado', true, 'Estado', 90, 'ACTIVO', 'bool'),
    ],
    _PMOLimpiar: () => setForm({ ...VACIO, TipoDocumento: combos.tiposDocumento[0]?.cnnum ?? '' }),
    _PMOMostrarRegistro: (f) => setForm({
      Id: f.Id, NombreProveedor: f.NombreProveedor ?? '', Direccion: f.Direccion ?? '', Telefono01: f.Telefono01 ?? '', Telefono02: f.Telefono02 ?? '',
      Descripcion: f.Descripcion ?? '', TipoDocumento: f.TipoDocumento ?? '', NroDocumento: f.NroDocumento ?? '', Estado: f.Estado === true || Number(f.Estado) === 1,
    }),
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombreProveedor.trim()) e.NombreProveedor = 'Ingrese Nombre de Proveedor';
      if (!form.TipoDocumento) e.TipoDocumento = 'Seleccione un Tipo Documento';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, _datos()),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, _datos()),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Proveedor ${f.NombreProveedor} ?`,
  });
  const ro = !m.habilitado;

  return (
    <ModeloF1 m={m} titulo="Proveedores" subtitulo="Gestion De Proveedores" icono="bag">
      <div className="registro-grid una">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del proveedor</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Nombre" ancho={2} error={errores.NombreProveedor} requerido>
              <input className="control mayus" value={form.NombreProveedor} onChange={setMay('NombreProveedor')} readOnly={ro} maxLength={150} />
            </Campo>
            <Campo etiqueta="Dirección" ancho={2}><input className="control" value={form.Direccion} onChange={set('Direccion')} readOnly={ro} maxLength={250} /></Campo>
            <Campo etiqueta="Teléfono 1"><input className="control" inputMode="tel" value={form.Telefono01} onChange={set('Telefono01')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="Teléfono 2"><input className="control" inputMode="tel" value={form.Telefono02} onChange={set('Telefono02')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="Tipo de documento" error={errores.TipoDocumento} requerido>
              <ComboLibreria dt={combos.tiposDocumento} valor={form.TipoDocumento} onChange={set('TipoDocumento')} titulo="Tipo De Documento" deshabilitado={ro} error={errores.TipoDocumento} onNuevo={nuevaLibreria(8, 'tiposDocumento')} />
            </Campo>
            <Campo etiqueta="Nro. documento"><input className="control" value={form.NroDocumento} onChange={set('NroDocumento')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="Descripción" ancho={2}><input className="control" value={form.Descripcion} onChange={set('Descripcion')} readOnly={ro} maxLength={300} /></Campo>
            <Campo etiqueta="Estado"><SwitchButton valor={form.Estado} onChange={set('Estado')} deshabilitado={ro} /></Campo>
          </div>
        </div>
      </div>
    </ModeloF1>
  );
}
