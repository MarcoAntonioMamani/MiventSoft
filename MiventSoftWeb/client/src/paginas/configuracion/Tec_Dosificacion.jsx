// =====================================================================
//  KAILY: Tec_Dosificacion.vb -> Gestion De Dosificacion   (SP: MAM_Dosificacion)
// =====================================================================
import { useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, SwitchButton, fechaCorta } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/configuracion/dosificacion';
const dos = (n) => String(n).padStart(2, '0');
const hoyLocal = () => { const h = new Date(); return `${h.getFullYear()}-${dos(h.getMonth() + 1)}-${dos(h.getDate())}`; };
const VACIO = { Id: '', SucursalId: '', NumeroAutorizacion: '', UltimoNroFactura: 0, Llave: '', FechaInicio: '', FechaLimite: '', Nota01: '', Nota02: '', Estado: true, TipoComputarizado: true };

export default function Tec_Dosificacion() {
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [sucursales, setSucursales] = useState([]);
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };

  // _prCargarComboGenerico(cbSucursal, L_fnGeneralSucursales(), "aanumi", "Codigo", "aabdes", "Sucursal")
  useAlActivar(() => { api.get(`${URL}/combos`).then((r) => setSucursales(r.data.sucursales)).catch(() => {}); });

  const _datos = () => ({
    SucursalId: form.SucursalId, NumeroAutorizacion: form.NumeroAutorizacion, UltimoNroFactura: form.UltimoNroFactura, Llave: form.Llave,
    FechaInicio: form.FechaInicio, FechaLimite: form.FechaLimite, Nota01: form.Nota01, Nota02: form.Nota02,
    Estado: form.Estado ? 1 : 0, TipoComputarizado: form.TipoComputarizado ? 1 : 0,
  });

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data.map((f) => ({
      ...f, FechaInicioTxt: fechaCorta(`${f.FechaInicio}T00:00:00Z`), FechaLimiteTxt: fechaCorta(`${f.FechaLimite}T00:00:00Z`),
    })),
    // _PMOGetListEstructuraBuscador (la Llave y las notas no se muestran en la lista)
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'Código', 70, '', 'num'),
      new Celda('SucursalId', false),
      new Celda('FechaInicioTxt', true, 'Fecha Inicio', 110),
      new Celda('FechaLimiteTxt', true, 'Fecha Límite', 110),
      new Celda('NombreAlmacen', true, 'Sucursal', 180),
      new Celda('UltimoNroFactura', true, 'Ult. Nro fact.', 110, '', 'num'),
      new Celda('NumeroAutorizacion', true, 'Nro Autorización', 170),
      new Celda('Estado', true, 'Estado', 90, 'ACTIVO', 'bool'),
    ],
    _PMOLimpiar: () => setForm({ ...VACIO, FechaInicio: hoyLocal(), FechaLimite: hoyLocal(), SucursalId: sucursales[0]?.aanumi ?? '' }),
    _PMOMostrarRegistro: (f) => setForm({
      Id: f.Id, SucursalId: f.SucursalId ?? '', NumeroAutorizacion: f.NumeroAutorizacion ?? '', UltimoNroFactura: f.UltimoNroFactura ?? 0,
      Llave: f.Llave ?? '', FechaInicio: f.FechaInicio || '', FechaLimite: f.FechaLimite || '', Nota01: f.Nota01 ?? '', Nota02: f.Nota02 ?? '',
      Estado: !!f.Estado, TipoComputarizado: !!f.TipoComputarizado,
    }),
    _PMOLimpiarErrores: () => setErrores({}),
    // _PMOValidarCampos: numero de factura >= 0 y sucursal seleccionada
    _PMOValidarCampos: () => {
      const e = {};
      const n = Number(form.UltimoNroFactura);
      if (!Number.isInteger(n) || n < 0) e.UltimoNroFactura = 'Ingrese un Monto Valido';
      if (!form.SucursalId && form.SucursalId !== 0) e.SucursalId = 'Seleccione una Sucursal';
      if (!form.FechaInicio) e.FechaInicio = 'Ingrese la Fecha Inicio';
      if (!form.FechaLimite) e.FechaLimite = 'Ingrese la Fecha Limite';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, _datos()),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, _datos()),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar La Dosificación ${f.Id} ?`,
  });
  const ro = !m.habilitado;

  return (
    <ModeloF1 m={m} titulo="Dosificación" subtitulo="Gestion De Dosificacion" icono="receipt">
      <div className="registro-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos de la dosificación</h2>
          <div className="campos">
            <Campo etiqueta="Código" ancho={1}><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Sucursal" ancho={2} error={errores.SucursalId} requerido>
              <ComboMulti dt={sucursales} valor={form.SucursalId} onChange={set('SucursalId')} id="aanumi" nameId="Codigo" descripcionId="aabdes" descripcion="Sucursal" deshabilitado={ro} error={errores.SucursalId} />
            </Campo>
            <Campo etiqueta="Nro Autorización" ancho={2}>
              <input className="control" value={form.NumeroAutorizacion} onChange={set('NumeroAutorizacion')} readOnly={ro} maxLength={100} autoFocus={!ro} />
            </Campo>
            <Campo etiqueta="Último Nro Factura" ancho={1} error={errores.UltimoNroFactura}>
              <input className="control num" type="number" min={0} step={1} value={form.UltimoNroFactura} onChange={set('UltimoNroFactura')} readOnly={ro} />
            </Campo>
            <Campo etiqueta="Llave" ancho={3}>
              <input className="control" value={form.Llave} onChange={set('Llave')} readOnly={ro} maxLength={500} autoComplete="off" spellCheck={false} />
            </Campo>
            <Campo etiqueta="Fecha Inicio" ancho={1} error={errores.FechaInicio} requerido>
              <input className="control" type="date" value={form.FechaInicio} onChange={set('FechaInicio')} readOnly={ro} />
            </Campo>
            <Campo etiqueta="Fecha Límite" ancho={1} error={errores.FechaLimite} requerido>
              <input className="control" type="date" value={form.FechaLimite} onChange={set('FechaLimite')} readOnly={ro} />
            </Campo>
            <Campo etiqueta="Nota #01" ancho={3}>
              <textarea className="control" rows={2} value={form.Nota01} onChange={set('Nota01')} readOnly={ro} maxLength={500} />
            </Campo>
            <Campo etiqueta="Nota #02" ancho={3}>
              <textarea className="control" rows={2} value={form.Nota02} onChange={set('Nota02')} readOnly={ro} maxLength={500} />
            </Campo>
            <Campo etiqueta="Estado"><SwitchButton valor={form.Estado} onChange={set('Estado')} deshabilitado={ro} /></Campo>
            <Campo etiqueta="Tipo"><SwitchButton valor={form.TipoComputarizado} onChange={set('TipoComputarizado')} deshabilitado={ro} textoOn="COMPUTARIZADA" textoOff="MANUAL" /></Campo>
          </div>
        </div>
      </div>
    </ModeloF1>
  );
}
