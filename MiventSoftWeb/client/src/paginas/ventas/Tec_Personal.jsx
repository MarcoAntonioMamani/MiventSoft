// =====================================================================
//  Tec_Personal.vb -> Gestion De Personal   (SP: MAM_Personal)
// =====================================================================
import { useRef, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, ComboLibreria, SwitchButton, fechaCorta } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/ventas/personal';
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`; };
const VACIO = { Id: '', NombrePersonal: '', Direccion: '', Telefono01: '', TipoDocumento: '', NroDocumento: '', TipoPersonal: '', EmpresaId: '', Estado: true, FechaNacimiento: '' };

export default function Tec_Personal() {
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [combos, setCombos] = useState({ tiposDocumento: [], tiposPersonal: [], empresas: [] });
  const combosRef = useRef(combos);
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };
  const setMay = (k) => (e) => { quitarError(k); setForm((f) => ({ ...f, [k]: e.target.value.toUpperCase() })); };

  useAlActivar(() => { api.get(`${URL}/combos`).then((r) => { combosRef.current = r.data; setCombos(r.data); }).catch(() => {}); });

  // btnTipoDocumento -> Efecto tipo 10 (libreria 8)
  const nuevaLibreria = (idClasif, clave) => async (descripcion) => {
    const r = await api.post(`${URL}/libreria/${idClasif}`, { descripcion });
    combosRef.current = { ...combosRef.current, [clave]: r.data };
    setCombos(combosRef.current);
    return r;
  };

  const _datos = () => ({
    NombrePersonal: form.NombrePersonal, Direccion: form.Direccion, Telefono01: form.Telefono01, TipoDocumento: form.TipoDocumento,
    NroDocumento: form.NroDocumento, TipoPersonal: form.TipoPersonal, EmpresaId: form.EmpresaId, Estado: form.Estado, FechaNacimiento: form.FechaNacimiento,
  });

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data.map((f) => ({ ...f, FNac: fechaCorta(f.FechaNacimiento) })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombrePersonal', true, 'Nombre Personal', 220),
      new Celda('NroDocumento', true, 'Nro Documento', 120),
      new Celda('Estado', true, 'Estado', 90, 'ACTIVO', 'bool'),
      new Celda('Tipo', true, 'Cargo', 120),
      new Celda('Telefono01', true, 'Teléfono', 110),
      new Celda('FNac', true, 'F.Nacimiento', 110),
    ],
    // _PMOLimpiar: primer item de cada combo y fecha de hoy
    _PMOLimpiar: () => setForm({
      ...VACIO, FechaNacimiento: hoy(),
      TipoDocumento: combos.tiposDocumento[0]?.cnnum ?? '', TipoPersonal: combos.tiposPersonal[0]?.cnnum ?? '', EmpresaId: combos.empresas[0]?.Id ?? '',
    }),
    _PMOMostrarRegistro: (f) => setForm({
      Id: f.Id, NombrePersonal: f.NombrePersonal ?? '', Direccion: f.Direccion ?? '', Telefono01: f.Telefono01 ?? '', TipoDocumento: f.TipoDocumento ?? '',
      NroDocumento: f.NroDocumento ?? '', TipoPersonal: f.TipoPersonal ?? '', EmpresaId: f.EmpresaId ?? '', Estado: f.Estado === true || Number(f.Estado) === 1,
      FechaNacimiento: f.FechaNacimiento ? String(f.FechaNacimiento).slice(0, 10) : '',
    }),
    _PMOLimpiarErrores: () => setErrores({}),
    // _PMOValidarCampos: Nombre, Empresa, Tipo Documento
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombrePersonal.trim()) e.NombrePersonal = 'Ingrese Nombre de Personal';
      if (!form.EmpresaId) e.EmpresaId = 'Seleccione una Empresa';
      if (!form.TipoDocumento) e.TipoDocumento = 'Seleccione un Tipo Documento';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, _datos()),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, _datos()),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Personal ${f.NombrePersonal} ?`,
  });
  const ro = !m.habilitado;

  return (
    <ModeloF1 m={m} titulo="Personal" subtitulo="Gestion De Personal" icono="users">
      <div className="registro-grid una">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del personal</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Nombre" ancho={2} error={errores.NombrePersonal} requerido>
              <input className="control mayus" value={form.NombrePersonal} onChange={setMay('NombrePersonal')} readOnly={ro} maxLength={150} />
            </Campo>
            <Campo etiqueta="Dirección" ancho={2}><input className="control" value={form.Direccion} onChange={set('Direccion')} readOnly={ro} maxLength={250} /></Campo>
            <Campo etiqueta="Teléfono"><input className="control" inputMode="tel" value={form.Telefono01} onChange={set('Telefono01')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="Tipo de documento" error={errores.TipoDocumento} requerido>
              <ComboLibreria dt={combos.tiposDocumento} valor={form.TipoDocumento} onChange={set('TipoDocumento')} titulo="Tipo De Documento" deshabilitado={ro} error={errores.TipoDocumento} onNuevo={nuevaLibreria(8, 'tiposDocumento')} />
            </Campo>
            <Campo etiqueta="Nro. documento"><input className="control" value={form.NroDocumento} onChange={set('NroDocumento')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="Fecha de nacimiento">
              <input type="date" className="control" value={form.FechaNacimiento} onChange={set('FechaNacimiento')} readOnly={ro} disabled={ro} max={hoy()} />
            </Campo>
            <Campo etiqueta="Cargo (tipo personal)">
              <ComboMulti dt={combos.tiposPersonal} valor={form.TipoPersonal} onChange={set('TipoPersonal')} id="cnnum" nameId="Código" descripcionId="cndesc1" descripcion="Tipo Personal" deshabilitado={ro} />
            </Campo>
            <Campo etiqueta="Empresa" error={errores.EmpresaId} requerido>
              <ComboMulti dt={combos.empresas} valor={form.EmpresaId} onChange={set('EmpresaId')} id="Id" nameId="Código" descripcionId="Nombre" descripcion="Empresa" deshabilitado={ro} error={errores.EmpresaId} />
            </Campo>
            <Campo etiqueta="Estado"><SwitchButton valor={form.Estado} onChange={set('Estado')} deshabilitado={ro} /></Campo>
          </div>
        </div>
      </div>
    </ModeloF1>
  );
}
