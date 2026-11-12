// =====================================================================
//  TecCategorias.vb -> Gestion De Categorias   (SP: MAM_Categorias)
// =====================================================================
import { useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, ImagenSelector, SwitchButton } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { api, urlImagen } from '../../api/http';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/configuracion/categorias';
const VACIO = { Id: '', NombreCategoria: '', DescripcionCategoria: '', Estado: true, VisibleApp: true, EmpresaId: '', Imagen: 'Default.jpg' };

export default function TecCategorias() {
  const [form, setForm] = useState(VACIO);
  const [archivo, setArchivo] = useState(null);
  const [errores, setErrores] = useState({});
  const [empresas, setEmpresas] = useState([]);
  // Al corregir un campo se quita su mensaje de error
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };

  // _prCargarComboGenerico(cbEmpresa, L_prListaEmpresasUsuarios(), "Id", "Codigo", "Nombre", "Empresa")
  useAlActivar(() => { api.get(`${URL}/combos`).then((r) => setEmpresas(r.data.empresas)).catch(() => {}); });

  const _datos = () => {
    const fd = new FormData();
    fd.append('NombreCategoria', form.NombreCategoria);
    fd.append('DescripcionCategoria', form.DescripcionCategoria);
    fd.append('Estado', form.Estado ? 1 : 0);
    fd.append('VisibleApp', form.VisibleApp ? 1 : 0);
    fd.append('EmpresaId', form.EmpresaId);
    fd.append('ImagenActual', form.Imagen || 'Default.jpg');
    if (archivo) fd.append('imagen', archivo);
    return fd;
  };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data,
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombreCategoria', true, 'Categoria', 200),
      new Celda('Estado', true, 'Estado', 90, 'ACTIVO', 'bool'),
      new Celda('DescripcionCategoria', true, 'Descripcion', 260),
      new Celda('Imagen', false),
      new Celda('EmpresaId', false),
      new Celda('App', true, 'App', 80, 'VISIBLE', 'bool'),
      new Celda('Empresa', true, 'Empresa', 180),
    ],
    _PMOLimpiar: () => {
      setForm({ ...VACIO, EmpresaId: empresas[0]?.Id ?? '' });
      setArchivo(null);
    },
    _PMOMostrarRegistro: (f) => {
      setForm({
        Id: f.Id, NombreCategoria: f.NombreCategoria ?? '', DescripcionCategoria: f.DescripcionCategoria ?? '',
        Estado: !!f.Estado, VisibleApp: !!f.App, EmpresaId: f.EmpresaId ?? '', Imagen: f.Imagen,
      });
      setArchivo(null);
    },
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombreCategoria.trim()) e.NombreCategoria = 'Ingrese Nombre de Categoria';
      if (!form.EmpresaId) e.EmpresaId = 'Seleccione una Empresa';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, _datos()),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, _datos()),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar la Categoria ${f.NombreCategoria} ?`,
  });
  const ro = !m.habilitado;

  return (
    <ModeloF1 m={m} titulo="Categorías" subtitulo="Gestion De Categorias de productos" icono="tag">
      <div className="registro-grid con-lateral">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos de la categoría</h2>
          <div className="campos">
            <Campo etiqueta="Código" ancho={1}><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Empresa" ancho={2} error={errores.EmpresaId} requerido>
              <ComboMulti dt={empresas} valor={form.EmpresaId} onChange={set('EmpresaId')} id="Id" nameId="Código" descripcionId="Nombre" descripcion="Empresa" deshabilitado={ro} error={errores.EmpresaId} />
            </Campo>
            <Campo etiqueta="Nombre categoría" ancho={3} error={errores.NombreCategoria} requerido>
              <input className="control" value={form.NombreCategoria} onChange={set('NombreCategoria')} readOnly={ro} maxLength={100} autoFocus={!ro} />
            </Campo>
            <Campo etiqueta="Descripción" ancho={3}>
              <textarea className="control" rows={3} value={form.DescripcionCategoria} onChange={set('DescripcionCategoria')} readOnly={ro} maxLength={200} />
            </Campo>
            <Campo etiqueta="Estado"><SwitchButton valor={form.Estado} onChange={set('Estado')} deshabilitado={ro} /></Campo>
            <Campo etiqueta="Visible en App"><SwitchButton valor={form.VisibleApp} onChange={set('VisibleApp')} deshabilitado={ro} textoOn="SI" textoOff="NO" /></Campo>
          </div>
        </div>
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Imagen</h2>
          <ImagenSelector key={`${form.Id}-${m.modo}`} src={urlImagen('categoria', form.Imagen)} deshabilitado={ro} onArchivo={setArchivo} />
        </div>
      </div>
    </ModeloF1>
  );
}
