// =====================================================================
//  Tec_Sucursales.vb (btInvAmacen) -> Almacenes / Sucursales  (SP: MAM_Almacenes 1 / 2 / -1)
// =====================================================================
import { useEffect, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, SwitchButton } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';
import { useToast } from '../../componentes/Toast';

const URL = '/api/inventario/almacenes';
const VACIO = { Id: '', NombreAlmacen: '', DepositoId: '', Direccion: '', Descripcion: '', Telefono: '', estado: 1 };

export default function Tec_Almacenes() {
  const toast = useToast();
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [deps, setDeps] = useState([]);
  useEffect(() => { api.get(`${URL}/depositos`).then((r) => setDeps(r.data)).catch((e) => toast(e.message, 'error')); }, [toast]);
  // si se abrio 'Nuevo' antes de que cargue la lista de depositos
  useEffect(() => { if (deps.length) setForm((f) => (f.Id === '' && !f.DepositoId ? { ...f, DepositoId: deps[0].Id } : f)); }, [deps]);
  const set = (k) => (e) => { setErrores((x) => ({ ...x, [k]: undefined })); setForm((f) => ({ ...f, [k]: e.target.value })); };
  const datos = () => ({ ...form, Id: undefined, estado: form.estado ? 1 : 0 });

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data.map((f) => ({ ...f, EstadoTxt: Number(f.estado) ? 'ACTIVO' : 'INACTIVO' })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombreAlmacen', true, 'Sucursal', 200),
      new Celda('NombreDeposito', true, 'Depósito', 160),
      new Celda('Direccion', true, 'Dirección', 220),
      new Celda('Telefono', true, 'Teléfono', 110),
      new Celda('EstadoTxt', true, 'Estado', 90),
    ],
    _PMOLimpiar: () => setForm({ ...VACIO, DepositoId: deps[0]?.Id ?? '' }),
    _PMOMostrarRegistro: (f) => setForm({ Id: f.Id, NombreAlmacen: f.NombreAlmacen ?? '', DepositoId: f.DepositoId ?? '', Direccion: f.Direccion ?? '', Descripcion: f.Descripcion ?? '', Telefono: f.Telefono ?? '', estado: Number(f.estado) ? 1 : 0 }),
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombreAlmacen.trim()) e.NombreAlmacen = 'Ingrese el Nombre de la Sucursal';
      if (!form.DepositoId) e.DepositoId = 'Seleccione Un Deposito';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, datos()),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, datos()),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar la Sucursal ${f.NombreAlmacen} ?`,
  });
  const ro = !m.habilitado;

  return (
    <ModeloF1 m={m} titulo="Almacenes" subtitulo="Gestion De Sucursales" icono="building">
      <div className="tarjeta">
        <h2 className="tarjeta-tit">Datos de la sucursal</h2>
        <div className="campos campos-2">
          <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
          <Campo etiqueta="Estado"><SwitchButton valor={!!form.estado} onChange={(x) => setForm((f) => ({ ...f, estado: x ? 1 : 0 }))} deshabilitado={ro} /></Campo>
          <Campo etiqueta="Nombre" ancho={2} error={errores.NombreAlmacen} requerido>
            <input className="control" value={form.NombreAlmacen} onChange={set('NombreAlmacen')} readOnly={ro} maxLength={150} />
          </Campo>
          <Campo etiqueta="Depósito" error={errores.DepositoId} requerido>
            <ComboMulti dt={deps} valor={form.DepositoId} onChange={(x) => { setErrores((e) => ({ ...e, DepositoId: undefined })); setForm((f) => ({ ...f, DepositoId: x })); }} id="Id" nameId="Cód." descripcionId="NombreDeposito" descripcion="Depósito" deshabilitado={ro} />
          </Campo>
          <Campo etiqueta="Teléfono"><input className="control" value={form.Telefono} onChange={set('Telefono')} readOnly={ro} maxLength={50} inputMode="tel" /></Campo>
          <Campo etiqueta="Dirección" ancho={2}><input className="control" value={form.Direccion} onChange={set('Direccion')} readOnly={ro} maxLength={250} /></Campo>
          <Campo etiqueta="Descripción" ancho={2}><textarea className="control" rows={3} value={form.Descripcion} onChange={set('Descripcion')} readOnly={ro} maxLength={250} /></Campo>
        </div>
      </div>
    </ModeloF1>
  );
}
