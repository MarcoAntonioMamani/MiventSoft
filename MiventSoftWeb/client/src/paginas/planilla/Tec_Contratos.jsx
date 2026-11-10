// =====================================================================
//  Tec_Contratos.vb (btnContratos) -> Gestion De Contratos  (SP: MAM_Contratos)
//  Personal, tipo de contrato (lib. 10) y cargo (lib. 11) · indefinido (sin fecha fin) · conceptos fijos del contrato.
//  No se registra un contrato nuevo si el personal ya tiene uno vigente (lo valida el servidor).
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, ComboLibreria, SwitchButton, fechaCorta } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import { useToast } from '../../componentes/Toast';
import Icono from '../../componentes/Icono';

const URL = '/api/planilla/contratos';
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const VACIO = { Id: '', PersonalId: '', TipoContratoId: '', Cargo: '', SueldoBase: 0, InicioContrato: '', FinContrato: '', Indefinido: 0 };
let _k = 0;

export default function Tec_Contratos() {
  const toast = useToast();
  const [c, setC] = useState({ tipos: [], cargos: [], personal: [], conceptos: [] });
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [conceptos, setConceptos] = useState([]);
  const [agregar, setAgregar] = useState('');
  useEffect(() => { api.get(`${URL}/combos`).then((r) => setC(r.data)).catch((e) => toast(e.message, 'error')); }, [toast]);
  useEffect(() => { setForm((f) => (f.Id === '' ? { ...f, TipoContratoId: f.TipoContratoId || (c.tipos[0]?.cnnum ?? ''), Cargo: f.Cargo || (c.cargos[0]?.cnnum ?? '') } : f)); }, [c.tipos, c.cargos]);
  const set = (k) => (e) => { const x = e?.target ? e.target.value : e; setErrores((o) => ({ ...o, [k]: undefined })); setForm((f) => ({ ...f, [k]: x })); };
  const nuevaLibreria = (idClasif, clave) => async (descripcion) => {
    const r = await api.post(`${URL}/libreria/${idClasif}`, { descripcion });
    setC((o) => ({ ...o, [clave]: r.data }));
    toast(r.mensaje, 'ok');
    return r;
  };
  const vig = conceptos.filter((x) => !x.eliminado);

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data.map((f) => ({ ...f, InicioTxt: fechaCorta(`${f.InicioContrato}T00:00:00Z`), FinTxt: f.Indefinido ? 'INDEFINIDO' : fechaCorta(`${f.FinContrato}T00:00:00Z`) })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombrePersonal', true, 'Nombre Personal', 200),
      new Celda('TipoContratoDescripcion', true, 'Tipo Contrato', 120),
      new Celda('DescripcionCargo', true, 'Cargo', 120),
      new Celda('Antiguedad', true, 'Antigüedad', 130),
      new Celda('SueldoBase', true, 'Sueldo', 90, '0.00', 'num'),
      new Celda('InicioTxt', true, 'Inicio', 95),
      new Celda('FinTxt', true, 'Fin', 95),
    ],
    _PMOLimpiar: () => { setForm({ ...VACIO, TipoContratoId: c.tipos[0]?.cnnum ?? '', Cargo: c.cargos[0]?.cnnum ?? '', InicioContrato: hoy(), FinContrato: hoy() }); setConceptos([]); },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, PersonalId: f.PersonalId, TipoContratoId: f.TipoContratoID, Cargo: f.Cargo, SueldoBase: Number(f.SueldoBase) || 0, InicioContrato: f.InicioContrato, FinContrato: f.FinContrato, Indefinido: f.Indefinido });
      api.get(`${URL}/${f.Id}/conceptos`).then((r) => setConceptos(r.data.filter((x) => x.estado >= 0).map((x) => ({ ...x, key: ++_k })))).catch((e) => toast(e.message, 'error'));
    },
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.PersonalId) e.PersonalId = 'Seleccione Personal';
      if (!form.TipoContratoId) e.TipoContratoId = 'Seleccione un Tipo de Contrato';
      if (!form.Cargo) e.Cargo = 'Seleccione un Cargo';
      if (!form.Indefinido && form.InicioContrato >= form.FinContrato) e.FinContrato = 'Fecha Finalizacion Debe Ser Mayor a la Fecha de Inicio de Vigencia';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, { ...form, Id: undefined, conceptos: vig.map((x) => ({ nuevo: true, ConceptoId: x.ConceptoId })) }),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, { ...form, Id: undefined, conceptos: conceptos.filter((x) => !(x.nuevo && x.eliminado)).map((x) => (x.nuevo ? { nuevo: true, ConceptoId: x.ConceptoId } : { Id: x.Id, eliminado: !!x.eliminado })) }),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Contrato del Personal ${f.NombrePersonal} ?`,
  });
  const ro = !m.habilitado;
  const disponibles = useMemo(() => c.conceptos.filter((x) => !vig.some((y) => y.ConceptoId === x.Id)), [c.conceptos, vig]);
  const agregarConcepto = () => {
    const x = c.conceptos.find((y) => String(y.Id) === String(agregar));
    if (!x) { toast('Seleccione Conceptos Fijos'.toUpperCase(), 'error'); return; }
    setConceptos((o) => [...o, { key: ++_k, nuevo: true, ConceptoId: x.Id, NombreConcepto: x.NombreConcepto, PorcentajeConcepto: x.Porcentaje }]);
    setAgregar('');
  };
  const totalPorc = vig.reduce((a, x) => a + Number(x.PorcentajeConcepto || 0), 0);

  return (
    <ModeloF1 m={m} titulo="Contratos" subtitulo="Gestion De Contratos" icono="clipboard">
      <div className="venta-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del contrato {form.Indefinido ? <span className="chip-mora al-dia">Indefinido</span> : null}</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Personal" requerido error={errores.PersonalId}>
              <ComboMulti dt={c.personal} valor={form.PersonalId} onChange={set('PersonalId')} id="Id" nameId="Cód." descripcionId="Nombre" descripcion="Personal" deshabilitado={ro} />
            </Campo>
            <Campo etiqueta="Tipo de contrato" requerido error={errores.TipoContratoId}>
              <ComboLibreria dt={c.tipos} valor={form.TipoContratoId} onChange={set('TipoContratoId')} titulo="Tipo De Contrato" deshabilitado={ro} onNuevo={nuevaLibreria(10, 'tipos')} />
            </Campo>
            <Campo etiqueta="Cargo" requerido error={errores.Cargo}>
              <ComboLibreria dt={c.cargos} valor={form.Cargo} onChange={set('Cargo')} titulo="Cargo" deshabilitado={ro} onNuevo={nuevaLibreria(11, 'cargos')} />
            </Campo>
            <Campo etiqueta="Sueldo base (Bs)"><input className="control num" type="number" min="0" step="0.01" value={form.SueldoBase} onChange={set('SueldoBase')} readOnly={ro} /></Campo>
            <Campo etiqueta="Indefinido"><SwitchButton valor={!!form.Indefinido} onChange={(x) => setForm((f) => ({ ...f, Indefinido: x ? 1 : 0 }))} deshabilitado={ro} textoOn="SI" textoOff="NO" /></Campo>
            <Campo etiqueta="Inicio de contrato" requerido><input type="date" className="control" value={form.InicioContrato} onChange={set('InicioContrato')} readOnly={ro} disabled={ro} /></Campo>
            {!form.Indefinido && <Campo etiqueta="Fin de contrato" requerido error={errores.FinContrato}><input type="date" className="control" value={form.FinContrato} min={form.InicioContrato} onChange={set('FinContrato')} readOnly={ro} disabled={ro} /></Campo>}
          </div>
        </div>
        <div className="tarjeta venta-detalle">
          <div className="detalle-cab-venta">
            <h2 className="tarjeta-tit">Conceptos fijos <span className="contador">{vig.length}</span></h2>
            {!ro && (
              <div className="control-con-btn concepto-agregar">
                <ComboMulti dt={disponibles} valor={agregar} onChange={setAgregar} id="Id" nameId="Cód." descripcionId="NombreConcepto" descripcion="Concepto" />
                <button type="button" className="btn btn-pri" onClick={agregarConcepto}><Icono n="plus" size={16} /> Agregar</button>
              </div>
            )}
          </div>
          <div className="tabla-scroll">
            <table className="grid grid-tarjetas tabla-detalle">
              <thead><tr><th>Concepto</th><th className="num">Porcentaje</th><th className="num">Monto aprox.</th>{!ro && <th aria-label="Quitar" />}</tr></thead>
              <tbody>
                {vig.map((x) => (
                  <tr key={x.key} className={x.nuevo ? 'fila-nueva' : ''}>
                    <td data-label="Concepto"><b>{tit(x.NombreConcepto)}</b></td>
                    <td data-label="Porcentaje" className="num">{n2(x.PorcentajeConcepto)} %</td>
                    <td data-label="Monto aprox." className="num">{n2((Number(form.SueldoBase) || 0) * Number(x.PorcentajeConcepto || 0) / 100)}</td>
                    {!ro && <td className="col-acc"><button className="btn-icono peligro" onClick={() => setConceptos((o) => o.map((y) => (y.key === x.key ? { ...y, eliminado: true } : y)))} aria-label={`Quitar ${x.NombreConcepto}`} title="Quitar"><Icono n="trash" size={16} /></button></td>}
                  </tr>
                ))}
                {!vig.length && <tr><td colSpan={4} className="vacio">Sin conceptos fijos</td></tr>}
              </tbody>
              {vig.length > 0 && <tfoot><tr className="tr-total"><td className="tf-tit"><b>Total</b></td><td className="num" data-label="Total porcentaje"><b>{n2(totalPorc)} %</b></td><td className="num" data-label="Total monto aprox."><b>{n2((Number(form.SueldoBase) || 0) * totalPorc / 100)}</b></td>{!ro && <td />}</tr></tfoot>}
            </table>
          </div>
        </div>
      </div>
    </ModeloF1>
  );
}
