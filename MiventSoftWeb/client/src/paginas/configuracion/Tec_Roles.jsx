// =====================================================================
//  Tec_Roles.vb -> Gestion De Roles   (SP: MAM_Roles, TVP RolDetalleType)
//  estado del detalle: igual que el escritorio (si estaba en 1 y se edita -> 2)
// =====================================================================
import { useMemo, useRef, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';
import { useToast } from '../../componentes/Toast';
import Icono from '../../componentes/Icono';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/configuracion/roles';

export default function Tec_Roles() {
  const toast = useToast();
  const [form, setForm] = useState({ Id: '', NombreRol: '' });
  const [detalle, setDetalle] = useState([]);
  const [modulos, setModulos] = useState([]);
  const [modSel, setModSel] = useState(null);
  const [errores, setErrores] = useState({});
  const pedido = useRef(0);

  useAlActivar(() => {
    api.get(`${URL}/modulos`).then((r) => { setModulos(r.data); setModSel((s) => s ?? r.data[0]?.cnnum ?? null); }).catch((e) => toast(e.message, 'error'));
  });

  // _prCargarGridDetalle(numi)
  const cargarDetalle = async (numi) => {
    const n = ++pedido.current;
    try {
      const r = await api.get(`${URL}/${numi}/detalle`);
      if (n === pedido.current) setDetalle(r.data);
    } catch (e) { toast(e.message, 'error'); }
  };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data,
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 80, '', 'num'),
      new Celda('NombreRol', true, 'ROL', 300),
      new Celda('FechaRegistro', false), new Celda('HoraRegistro', false), new Celda('UsuarioRegistro', false),
    ],
    _PMOLimpiar: () => { setForm({ Id: '', NombreRol: '' }); setDetalle([]); },
    _PMONuevo: () => cargarDetalle(-1),
    _PMOMostrarRegistro: (f) => { setForm({ Id: f.Id, NombreRol: f.NombreRol ?? '' }); cargarDetalle(f.Id); },
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombreRol.trim()) e.NombreRol = 'INGRESE LA DESCRIPCION DEL ROL!';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, { NombreRol: form.NombreRol, detalle }),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, { NombreRol: form.NombreRol, detalle }),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Rol ${f.NombreRol} ?`,
  });
  const ro = !m.habilitado;

  const visibles = useMemo(() => detalle.filter((d) => String(d.Modulo) === String(modSel)), [detalle, modSel]);
  const asignados = (cnnum) => detalle.filter((d) => String(d.Modulo) === String(cnnum) && Number(d.Ver) === 1).length;
  const esVer = (d) => Number(d.Ver) === 1 || d.Ver === true;

  // grDetalle_CellEdited  /  _prSeleccionarTodos("Ver")
  const marcar = (filtro, valor) =>
    setDetalle((dt) => dt.map((d) => (filtro(d) ? { ...d, Ver: valor ? 1 : 0, estado: Number(d.estado) === 1 ? 2 : d.estado } : d)));

  const modActual = modulos.find((x) => String(x.cnnum) === String(modSel));

  return (
    <ModeloF1 m={m} titulo="Roles" subtitulo="Gestion De Roles y privilegios" icono="shield">
      <div className="tarjeta">
        <div className="campos">
          <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
          <Campo etiqueta="Nombre del rol" ancho={2} error={errores.NombreRol} requerido>
            <input className="control mayus" value={form.NombreRol} onChange={(e) => { setErrores((x) => ({ ...x, NombreRol: undefined })); setForm({ ...form, NombreRol: e.target.value.toUpperCase() }); }} readOnly={ro} maxLength={30} />
          </Campo>
        </div>
      </div>
      <div className="roles-layout">
        <div className="tarjeta sin-pad">
          <h2 className="tarjeta-tit pad">Módulos</h2>
          <ul className="lista-modulos">
            {modulos.map((x) => (
              <li key={x.cnnum}>
                <button className={String(x.cnnum) === String(modSel) ? 'activo' : ''} onClick={() => setModSel(x.cnnum)}>
                  <span>{x.cndesc1}</span>
                  <span className="contador">{asignados(x.cnnum)}</span>
                </button>
              </li>
            ))}
          </ul>
        </div>
        <div className="tarjeta sin-pad">
          <div className="detalle-cab pad">
            <h2 className="tarjeta-tit">PRIVILEGIOS DEL MODULO {modActual?.cndesc1 ?? ''}</h2>
            {!ro && (
              <div className="barra">
                <button className="btn btn-sec btn-sm" onClick={() => marcar((d) => String(d.Modulo) === String(modSel), true)}><Icono n="check" size={14} /> Seleccionar todos</button>
                <button className="btn btn-sec btn-sm" onClick={() => marcar((d) => String(d.Modulo) === String(modSel), false)}><Icono n="x" size={14} /> Quitar todos</button>
              </div>
            )}
          </div>
          <div className="tabla-scroll">
            <table className="grid">
              <thead><tr><th style={{ width: 90 }}>CODIGO</th><th>PROGRAMAS</th><th style={{ width: 120 }} className="centro">ASIGNAR</th></tr></thead>
              <tbody>
                {visibles.map((d) => (
                  <tr key={d.ProgramaId} className={esVer(d) ? 'marcada' : ''}>
                    <td className="num">{d.ProgramaId}</td>
                    <td>{d.DescripcionPrograma}</td>
                    <td className="centro">
                      <input type="checkbox" className="chk" checked={esVer(d)} disabled={ro} aria-label={`Asignar ${d.DescripcionPrograma}`}
                        onChange={(e) => marcar((x) => x.ProgramaId === d.ProgramaId, e.target.checked)} />
                    </td>
                  </tr>
                ))}
                {!visibles.length && <tr><td colSpan={3} className="vacio">{m.dt.length || m.modo === 'nuevo' ? 'Sin programas en este módulo' : 'Seleccione un rol'}</td></tr>}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </ModeloF1>
  );
}
