// =====================================================================
//  Tec_Users.vb -> Gestion De Usuarios   (SP: MAM_Usuarios)
//  Nota: la contraseña ya no viaja al navegador. Al modificar, dejarla vacia = sin cambios.
// =====================================================================
import { useEffect, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, SwitchButton } from '../../componentes/Controles';
import { EfectoAyuda } from '../../componentes/Efecto';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';

const URL = '/api/configuracion/usuarios';
const VACIO = { Id: '', NombreUsuario: '', Contrasena: '', estado: true, pedido: false, RolId: '', IdEmpresa: '', IdPersonal: 0, Personal: '' };

export default function Tec_Users() {
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [combos, setCombos] = useState({ roles: [], empresas: [], personal: [] });
  const [ayuda, setAyuda] = useState(false);
  // Al corregir un campo se quita su mensaje de error
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };
  const setMay = (k) => (e) => { quitarError(k); setForm((f) => ({ ...f, [k]: e.target.value.toUpperCase() })); };

  useEffect(() => { api.get(`${URL}/combos`).then((r) => setCombos(r.data)).catch(() => {}); }, []);

  const _datos = () => ({
    NombreUsuario: form.NombreUsuario, Contrasena: form.Contrasena, estado: form.estado, pedido: form.pedido,
    RolId: form.RolId, IdEmpresa: form.IdEmpresa, IdPersonal: form.IdPersonal,
  });

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data,
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombreUsuario', true, 'Usuario', 140),
      new Celda('estado', true, 'Estado', 90, 'ACTIVO', 'bool'),
      new Celda('RolId', false),
      new Celda('NombreRol', true, 'Rol', 150),
      new Celda('SucursalId', false), new Celda('IdEmpresa', false),
      new Celda('Empresa', true, 'Empresa', 180),
      new Celda('IdPersonal', false), new Celda('pedido', false),
      new Celda('Personal', true, 'Personal', 200),
    ],
    _PMOLimpiar: () => setForm({ ...VACIO, RolId: combos.roles[0]?.Id ?? '', IdEmpresa: combos.empresas[0]?.Id ?? '' }),
    _PMOMostrarRegistro: (f) => setForm({
      Id: f.Id, NombreUsuario: f.NombreUsuario ?? '', Contrasena: '', estado: !!f.estado, pedido: !!f.pedido,
      RolId: f.RolId ?? '', IdEmpresa: f.IdEmpresa ?? '', IdPersonal: f.IdPersonal ?? 0, Personal: f.Personal ?? '',
    }),
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!(form.IdPersonal > 0)) e.Personal = 'Seleccione Un Personal';
      if (!form.NombreUsuario.trim()) e.NombreUsuario = 'Ingrese Nombre de Usuario';
      if (m.modo === 'nuevo' && !form.Contrasena) e.Contrasena = 'Ingrese una Contraseña';
      if (!form.IdEmpresa) e.IdEmpresa = 'Seleccione Una Empresa';
      if (!form.RolId) e.RolId = 'Seleccione Un Rol';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, _datos()),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, _datos()),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Usuario ${f.NombreUsuario} ?`,
  });
  const ro = !m.habilitado;

  return (
    <ModeloF1 m={m} titulo="Usuarios" subtitulo="Gestion De Usuarios del sistema" icono="users">
      <div className="registro-grid una">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del usuario</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Personal" ancho={2} error={errores.Personal} requerido>
              <div className="control-con-btn">
                <input className={`control ${!ro ? 'control-elegir' : ''}`} value={form.Personal} readOnly placeholder="Toque para buscar el personal..."
                  onClick={() => !ro && setAyuda(true)} onKeyDown={(e) => !ro && (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), setAyuda(true))} />
                {!ro && <button type="button" className="btn btn-sec" onClick={() => setAyuda(true)}><Icono n="search" size={15} /> Buscar</button>}
              </div>
            </Campo>
            <Campo etiqueta="Nombre de usuario" error={errores.NombreUsuario} requerido>
              <input className="control mayus" value={form.NombreUsuario} onChange={setMay('NombreUsuario')} readOnly={ro} maxLength={50} autoComplete="off" />
            </Campo>
            <Campo etiqueta={m.modo === 'modificar' ? 'Nueva contraseña' : 'Contraseña'} error={errores.Contrasena} requerido={m.modo === 'nuevo'}>
              <input className="control mayus" type="password" value={form.Contrasena} onChange={setMay('Contrasena')} readOnly={ro} maxLength={50}
                autoComplete="new-password" placeholder={m.modo === 'modificar' ? 'Vacío = sin cambios' : ro ? '••••••' : ''} />
            </Campo>
            <Campo etiqueta="Estado"><SwitchButton valor={form.estado} onChange={set('estado')} deshabilitado={ro} /></Campo>
            <Campo etiqueta="Rol" error={errores.RolId} requerido>
              <ComboMulti dt={combos.roles} valor={form.RolId} onChange={set('RolId')} id="Id" nameId="Código" descripcionId="NombreRol" descripcion="Roles" deshabilitado={ro} error={errores.RolId} />
            </Campo>
            <Campo etiqueta="Empresa" error={errores.IdEmpresa} requerido>
              <ComboMulti dt={combos.empresas} valor={form.IdEmpresa} onChange={set('IdEmpresa')} id="Id" nameId="Código" descripcionId="Nombre" descripcion="Empresa" deshabilitado={ro} error={errores.IdEmpresa} />
            </Campo>
            <Campo etiqueta="Venta directa (pedido sin conciliación)"><SwitchButton valor={form.pedido} onChange={set('pedido')} deshabilitado={ro} textoOn="SI" textoOff="NO" /></Campo>
          </div>
        </div>
      </div>
      <EfectoAyuda
        abierto={ayuda}
        titulo="SELECCIONE PERSONAL"
        dt={combos.personal}
        columnas={[new Celda('Nombre', true, 'NOMBRE', 300), new Celda('Direccion', true, 'DIRECCION', 180), new Celda('Telefono01', true, 'TELEFONO', 140)]}
        onCancelar={() => setAyuda(false)}
        onSeleccionar={(r) => { quitarError('Personal'); setForm((f) => ({ ...f, IdPersonal: r.Id, Personal: r.Nombre })); setAyuda(false); }}
      />
    </ModeloF1>
  );
}
