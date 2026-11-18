// =====================================================================
//  Tec_ConceptosFijos.vb (btnConceptoFijos) -> Conceptos de planilla  (SP: MAM_ConceptosFijos)
//  Tipo: Fijo (porcentaje del sueldo, > 0) / Variable (monto que se carga en la planilla).
// =====================================================================
import { useEffect, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, SwitchButton } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';
import { useToast } from '../../componentes/Toast';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/planilla/conceptos-fijos';
const VACIO = { Id: '', NombreConcepto: '', Porcentaje: 0, OperacionId: '', TipoConcepto: 1, Estado: 1 };

export default function Tec_ConceptosFijos() {
  const toast = useToast();
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [ops, setOps] = useState([]);
  useAlActivar(() => { api.get(`${URL}/operaciones`).then((r) => setOps(r.data)).catch((e) => toast(e.message, 'error')); });
  useEffect(() => { if (ops.length) setForm((f) => (f.Id === '' && !f.OperacionId ? { ...f, OperacionId: ops[0].Id } : f)); }, [ops]);
  const set = (k) => (e) => { const x = e?.target ? e.target.value : e; setErrores((o) => ({ ...o, [k]: undefined })); setForm((f) => ({ ...f, [k]: x })); };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data.map((f) => ({ ...f, EstadoTxt: Number(f.Estado) ? 'ACTIVO' : 'INACTIVO', TipoTxt: Number(f.Tipo) ? 'FIJO' : 'VARIABLE' })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombreConcepto', true, 'Nombre Concepto', 240),
      new Celda('Porcentaje', true, 'Porcentaje', 100, '0.00', 'num'),
      new Celda('NombreOperacion', true, 'Operación', 110),
      new Celda('TipoTxt', true, 'Concepto', 100),
      new Celda('EstadoTxt', true, 'Estado', 100),
    ],
    _PMOLimpiar: () => setForm({ ...VACIO, OperacionId: ops[0]?.Id ?? '' }),
    _PMOMostrarRegistro: (f) => setForm({ Id: f.Id, NombreConcepto: f.NombreConcepto ?? '', Porcentaje: Number(f.Porcentaje) || 0, OperacionId: f.OperacionId ?? '', TipoConcepto: Number(f.Tipo) ? 1 : 0, Estado: Number(f.Estado) ? 1 : 0 }),
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombreConcepto.trim()) e.NombreConcepto = 'Ingrese Nombre de Concepto';
      if (!form.OperacionId) e.OperacionId = 'Seleccione una Operación';
      if ((form.TipoConcepto && !(Number(form.Porcentaje) > 0)) || Number(form.Porcentaje) < 0 || Number(form.Porcentaje) > 100) e.Porcentaje = 'Ingrese valor Porcentaje Valido';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, { ...form, Id: undefined }),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, { ...form, Id: undefined }),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Concepto ${f.NombreConcepto} ?`,
  });
  const ro = !m.habilitado;

  return (
    <ModeloF1 m={m} titulo="Conceptos fijos" subtitulo="Gestion De Conceptos De Planilla" icono="clipboard">
      <div className="tarjeta">
        <h2 className="tarjeta-tit">Datos del concepto</h2>
        <div className="campos campos-2">
          <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
          <Campo etiqueta="Estado"><SwitchButton valor={!!form.Estado} onChange={(x) => setForm((f) => ({ ...f, Estado: x ? 1 : 0 }))} deshabilitado={ro} /></Campo>
          <Campo etiqueta="Nombre del concepto" ancho={2} error={errores.NombreConcepto} requerido>
            <input className="control" value={form.NombreConcepto} onChange={set('NombreConcepto')} readOnly={ro} maxLength={150} />
          </Campo>
          <Campo etiqueta="Tipo de concepto"><SwitchButton valor={!!form.TipoConcepto} onChange={(x) => setForm((f) => ({ ...f, TipoConcepto: x ? 1 : 0 }))} deshabilitado={ro} textoOn="FIJO" textoOff="VARIABLE" /></Campo>
          <Campo etiqueta="Operación" error={errores.OperacionId} requerido>
            <ComboMulti dt={ops} valor={form.OperacionId} onChange={set('OperacionId')} id="Id" nameId="Cód." descripcionId="NombreOperacion" descripcion="Operaciones" deshabilitado={ro} />
          </Campo>
          <Campo etiqueta="Porcentaje (%)" error={errores.Porcentaje} requerido={!!form.TipoConcepto}>
            <input className="control num" type="number" min="0" max="100" step="0.01" value={form.Porcentaje} onChange={set('Porcentaje')} readOnly={ro} />
          </Campo>
        </div>
        <p className="ayuda-tip">{form.TipoConcepto ? 'Concepto fijo: se calcula como porcentaje del sueldo base en cada planilla.' : 'Concepto variable: el monto se registra en la planilla de cada trabajador.'}</p>
      </div>
    </ModeloF1>
  );
}
