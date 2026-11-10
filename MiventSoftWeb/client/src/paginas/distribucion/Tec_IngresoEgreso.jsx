// =====================================================================
//  Tec_ProgramaIngresoEgresoCaja.vb -> Registrar gastos / ingresos generales  (SP: MAM_CajaIngresoEgreso)
//  El motivo define si es Ingreso o Egreso. Motivo 9 = traspaso entre cajas (pide caja destino).
//  Caja chica exige cierre de cajero abierto en la fecha. Bloqueado si pertenece a un cierre.
// =====================================================================
import { useEffect, useState } from 'react';
import { createPortal } from 'react-dom';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, fechaCorta } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { columnasDe } from '../ventas/Reportes';
import { imprimirReporte } from '../../componentes/ReporteImpreso';
import { useSesion } from '../../global/Sesion';

// Columnas para la hoja impresa (mismas que en pantalla; los textos en MAYUSCULAS se muestran como Titulo)
const colsImpresion = (filas) => columnasDe(filas).map((c) => (c.render ? { ...c, valor: c.render } : c));

const URL = '/api/distribucion/ingresos-egresos';
const TRASPASO = 9;
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const MSJ_CIERRE = 'No Es Posible Modificar El Movimiento ya Que Pertenece a Un cierre de Caja Con Estado Cerrado'.toUpperCase();

export default function Tec_IngresoEgreso() {
  const toast = useToast();
  const { sesion } = useSesion();
  const [nuevoMotivo, setNuevoMotivo] = useState(false);
  const [c, setC] = useState({ cajas: [], tipos: [], sucursales: [], personal: null });
  const [form, setForm] = useState({});
  const [errores, setErrores] = useState({});
  useEffect(() => { api.get(`${URL}/combos`).then((r) => setC(r.data)).catch((e) => toast(e.message, 'error')); }, [toast]);
  const tipo = c.tipos.find((t) => Number(t.Id) === Number(form.CajaTipoMovimientoId));
  const esIngreso = tipo ? Number(tipo.Tipo) === 1 : true;
  const esTraspaso = Number(form.CajaTipoMovimientoId) === TRASPASO;
  const set = (k) => (x) => { setErrores((e) => ({ ...e, [k]: undefined })); setForm((f) => ({ ...f, [k]: x?.target ? x.target.value : x })); };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data.map((f) => ({ ...f, FechaTxt: fechaCorta(`${f.Fecha}T00:00:00Z`) })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'Movimiento', 80, '', 'num'),
      new Celda('FechaTxt', true, 'Fecha', 100),
      new Celda('Descripcion', true, 'Descripción', 200),
      new Celda('Monto', true, 'Monto', 100, '0.00', 'num'),
      new Celda('NombreCaja', true, 'Caja', 110),
      new Celda('Movimiento', true, 'Movimiento', 90),
      new Celda('NombreTipoMovimiento', true, 'Motivo', 140),
      new Celda('NombrePersonal', true, 'Personal', 140),
      new Celda('Sucursal', true, 'Sucursal', 100),
    ],
    // _PMOLimpiar: primer item de cada combo
    _PMOLimpiar: () => setForm({ Id: '', Fecha: hoy(), Descripcion: '', Monto: '', CajaId: c.cajas[0]?.Id ?? '', CajaTipoMovimientoId: c.tipos[0]?.Id ?? '', SucursalId: c.sucursales[0]?.aanumi ?? '', CajaIdDestino: c.cajas[c.cajas.length - 1]?.Id ?? '', Personal: c.personal?.Nombre ?? '' }),
    _PMOMostrarRegistro: (f) => setForm({ Id: f.Id, Fecha: f.Fecha, Descripcion: f.Descripcion ?? '', Monto: f.Monto, CajaId: f.CajaId, CajaTipoMovimientoId: f.CajatipoMovimientoId, SucursalId: f.SucursalId, CajaIdDestino: f.CajaIdDestino, Personal: f.NombrePersonal ?? '' }),
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!(Number(form.Monto) > 0)) e.Monto = 'Ingrese un Monto Valido';
      if (!form.CajaTipoMovimientoId) e.CajaTipoMovimientoId = 'Seleccione un Motivo Movimiento Valido';
      if (!form.CajaId) e.CajaId = 'Seleccione una Caja';
      if (!form.SucursalId) e.SucursalId = 'Seleccione una Sucursal';
      if (esTraspaso && m.modo === 'nuevo') {
        if (!form.CajaIdDestino) e.CajaIdDestino = 'Seleccione una Caja Destino';
        else if (Number(form.CajaIdDestino) === Number(form.CajaId)) e.CajaIdDestino = 'Seleccione una Caja diferente Al Origen';
      }
      setErrores(e);
      if (Object.keys(e).length) toast(Object.values(e)[0], 'error');
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, form),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, form),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar El Ingreso / Egreso ${tit(f.NombreTipoMovimiento || '')} ?`,
    _PMOBloqueoModificar: (f) => (Number(f?.CajatipoMovimientoId) === TRASPASO ? 'No Es Posible Modificar Un Traspaso. Elimine Este registro y Vuelva a Registrarlo'.toUpperCase() : f?.CierreCajeroId > 0 ? MSJ_CIERRE : null),
    _PMOBloqueoEliminar: (f) => (f?.CierreCajeroId > 0 ? MSJ_CIERRE : null),
  });
  const ro = !m.habilitado;

  const comprobante = async (fila) => {
    const f = fila || m.filaActual; if (!f) return;
    try {
      const r = await api.get(`${URL}/${f.Id}/reporte`);
      if (!r.data.filas.length) { toast('No Existen Datos Para Mostrar', 'error'); return; }
      imprimirReporte({
        titulo: `Comprobante ingreso / egreso #${f.Id}`, subtitulo: tit(f.NombreTipoMovimiento || ''), usuario: tit(sesion?.gs_user || ''),
        empresa: { Nombre: r.data.empresa.nombre, Direccion: r.data.empresa.direccion }, imagenEmpresa: r.data.imagenEmpresa,
        columnas: colsImpresion(r.data.filas), filas: r.data.filas,
      });
    } catch (e) { toast(e.message, 'error'); }
  };

  return (
    <>
    <ModeloF1 m={m} titulo="Ingresos y egresos" subtitulo="Ingreso/Egreso Caja" icono="wallet"
      accionesRegistro={<button className="btn btn-sec" onClick={() => comprobante()} disabled={!m.filaActual || m.habilitado}><Icono n="receipt" size={16} /> <span className="txt-btn">Comprobante</span></button>}
      accionesFila={(f) => [{ texto: 'Comprobante', icono: 'printer', onClick: () => comprobante(f) }]}>
      <div className="registro-grid una">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Movimiento de caja <span className={`chip-mora ${esIngreso ? 'al-dia' : 'mora-alta'}`}>{esTraspaso ? 'Traspaso' : esIngreso ? 'Ingreso' : 'Egreso'}</span></h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id ?? ''} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Fecha" requerido><input type="date" className="control" value={form.Fecha ?? ''} onChange={set('Fecha')} readOnly={ro} disabled={ro} /></Campo>
            <Campo etiqueta="Motivo" ancho={2} error={errores.CajaTipoMovimientoId} requerido>
              <div className="control-con-btn">
                <ComboMulti dt={c.tipos} valor={form.CajaTipoMovimientoId} onChange={set('CajaTipoMovimientoId')} id="Id" nameId="Cód." descripcionId="Descripcion" descripcion="Motivo Movimiento" deshabilitado={ro} error={errores.CajaTipoMovimientoId} />
                {!ro && <button type="button" className="btn btn-sec btn-cuadrado" title="Nuevo motivo" aria-label="Nuevo motivo" onClick={() => setNuevoMotivo(true)}><Icono n="plus" size={16} /></button>}
              </div>
            </Campo>
            <Campo etiqueta={esTraspaso ? 'Caja origen' : 'Caja'} error={errores.CajaId} requerido>
              <ComboMulti dt={c.cajas} valor={form.CajaId} onChange={set('CajaId')} id="Id" nameId="Cód." descripcionId="NombreCaja" descripcion="Caja" deshabilitado={ro} error={errores.CajaId} />
            </Campo>
            {esTraspaso && (
              <Campo etiqueta="Caja destino" error={errores.CajaIdDestino} requerido>
                <ComboMulti dt={c.cajas} valor={form.CajaIdDestino} onChange={set('CajaIdDestino')} id="Id" nameId="Cód." descripcionId="NombreCaja" descripcion="Caja destino" deshabilitado={ro || m.modo !== 'nuevo'} error={errores.CajaIdDestino} />
              </Campo>
            )}
            <Campo etiqueta="Monto (Bs)" error={errores.Monto} requerido>
              <input className="control num" type="number" inputMode="decimal" min="0" step="0.01" value={form.Monto ?? ''} onChange={set('Monto')} readOnly={ro} />
            </Campo>
            <Campo etiqueta="Sucursal" error={errores.SucursalId} requerido>
              <ComboMulti dt={c.sucursales} valor={form.SucursalId} onChange={set('SucursalId')} id="aanumi" nameId="Cód." descripcionId="aabdes" descripcion="Sucursal" deshabilitado={ro} error={errores.SucursalId} />
            </Campo>
            <Campo etiqueta="Personal"><input className="control" value={tit(form.Personal ?? '')} readOnly /></Campo>
            <Campo etiqueta="Descripción" ancho={2}><input className="control" value={form.Descripcion ?? ''} onChange={set('Descripcion')} readOnly={ro} maxLength={500} /></Campo>
          </div>
        </div>
      </div>
    </ModeloF1>
    {nuevoMotivo && <DialogoMotivo onCerrar={() => setNuevoMotivo(false)} onCreado={(r) => { setC((o) => ({ ...o, tipos: r.data })); set('CajaTipoMovimientoId')(r.id); setNuevoMotivo(false); toast(r.mensaje, 'ok'); }} />}
    </>
  );
}

// btnAgregarMotivoMovimiento -> Crear_TipoMovimientoCaja: descripcion (mas de 3 letras) + Ingreso / Egreso
function DialogoMotivo({ onCreado, onCerrar }) {
  const [desc, setDesc] = useState('');
  const [ingreso, setIngreso] = useState(true);
  const [err, setErr] = useState('');
  const [enviando, setEnviando] = useState(false);
  const guardar = async () => {
    if (enviando) return;
    if (desc.trim().length <= 3) { setErr('Debe Ingresar un Texto Valido en Tipo Movimiento (mas de 3 letras)'); return; }
    setEnviando(true); setErr('');
    try { onCreado(await api.post(`${URL}/motivos`, { descripcion: desc.trim(), ingreso })); } catch (e) { setErr(e.message); setEnviando(false); }
  };
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-sm" role="dialog" aria-modal="true" aria-label="Nuevo motivo de movimiento" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') onCerrar(); if (e.key === 'Enter') { e.preventDefault(); guardar(); } }}>
        <div className="modal-cab"><h3>Nuevo motivo de movimiento</h3><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="campos una-col">
          <Campo etiqueta="Tipo movimiento" requerido error={err}><input autoFocus className="control mayus" value={desc} maxLength={100} onChange={(e) => { setDesc(e.target.value.toUpperCase()); setErr(''); }} /></Campo>
          <Campo etiqueta="Tipo">
            <div className="seg" role="radiogroup" aria-label="Ingreso o egreso">
              <button type="button" role="radio" aria-checked={ingreso} className={`btn ${ingreso ? 'btn-pri' : 'btn-sec'}`} onClick={() => setIngreso(true)}>Ingreso</button>
              <button type="button" role="radio" aria-checked={!ingreso} className={`btn ${!ingreso ? 'btn-pri' : 'btn-sec'}`} onClick={() => setIngreso(false)}>Egreso</button>
            </div>
          </Campo>
        </div>
        <div className="modal-acciones"><button className="btn btn-sec" onClick={onCerrar}>Cancelar</button><button className="btn btn-pri" disabled={enviando} onClick={guardar}>{enviando ? 'Guardando...' : 'Guardar'}</button></div>
      </div>
    </div>,
    document.body,
  );
}
