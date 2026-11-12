// =====================================================================
//  KAILY LA PAZ: FrmDespacho.vb (btnDespachoVenta) -> Despacho Ventas   (SP: MAM_Ventas)
//  Lista de ventas con su estado de despacho. "Editar" habilita solo el despacho:
//  Despachador (ListarPersonal), Entregado y Observacion -> VentaInsertarDespacho (@tipo 11).
//  Sin Nuevo ni Eliminar (en el escritorio estan ocultos). Nota de despacho solo si esta entregado.
// =====================================================================
import { useEffect, useRef, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, SwitchButton, fechaCorta } from '../../componentes/Controles';
import { EfectoAyuda } from '../../componentes/Efecto';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { useAlActivar } from '../../global/vistaActiva';
import { NotaVenta, CFG_VENTA_PV } from './Tec_VentasPuntoVenta';

const URL = '/api/ventas/despacho';
const CFG_NOTA = { ...CFG_VENTA_PV, nota: 'Nota de Despacho' }; // P_GenerarReporte: TipoReporte "NOTA DE DESPACHO"
const MAX_OBSERVACION = 400;
const dos = (n) => String(n).padStart(2, '0');
const hoyLocal = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const fechaTxt = (iso) => (iso ? fechaCorta(`${iso}T00:00:00Z`) : '');

const VACIO = {
  Id: '', FechaVenta: '', Personal: '', NombreCliente: '', NombreAlmacen: '', TVenta: '', Glosa: '', Descuento: 0,
  EstadoDespacho: false, FechaDespacho: '', PersonalDespachadorId: 0, Despachante: '', ObservacionDespacho: '',
};
const COLS_PERSONAL = [new Celda('Nombre', true, 'NOMBRE', 300), new Celda('Direccion', true, 'DIRECCION', 180), new Celda('Telefono01', true, 'TELEFONO', 140)];

export default function Tec_DespachoVentas() {
  const toast = useToast();
  const [form, setForm] = useState(VACIO);
  const [detalle, setDetalle] = useState([]);
  const [cargandoDet, setCargandoDet] = useState(false);
  const [errores, setErrores] = useState({});
  const [ini, setIni] = useState(null);
  const [filtro, setFiltro] = useState({ desde: hoyLocal(), hasta: hoyLocal() });
  const [personal, setPersonal] = useState(null); // ayuda "Seleccione Despachador" (null = cerrada)
  const [recibo, setRecibo] = useState(null);
  const pedidoDet = useRef(0);
  const hoy = ini?.hoy || hoyLocal();

  const cargarIni = () => api.get(`${URL}/inicio`).then((r) => setIni(r.data)).catch((e) => toast(e.message, 'error'));
  useEffect(() => { cargarIni(); }, []); // eslint-disable-line react-hooks/exhaustive-deps
  useAlActivar(cargarIni, { alMontar: false });

  // _PMOMostrarRegistro
  const mostrar = (f) => {
    setForm({
      Id: f.Id, FechaVenta: f.FechaVenta ?? '', Personal: f.Personal ?? '', NombreCliente: f.NombreCliente ?? '', NombreAlmacen: f.NombreAlmacen ?? '',
      TVenta: f.TVenta ?? '', Glosa: f.Glosa ?? '', Descuento: num(f.Descuento),
      EstadoDespacho: Number(f.EstadoDespacho) === 1, FechaDespacho: f.FechaDespacho ?? '',
      PersonalDespachadorId: num(f.PersonalDespachadorId), Despachante: f.Despachante ?? '', ObservacionDespacho: f.ObservacionDespacho ?? '',
    });
    const n = ++pedidoDet.current;
    setDetalle([]);
    setCargandoDet(true);
    api.get(`${URL}/${f.Id}/detalle`).then((r) => { if (n === pedidoDet.current) setDetalle(r.data); })
      .catch((e) => toast(e.message, 'error')).finally(() => n === pedidoDet.current && setCargandoDet(false));
  };

  const m = useModeloF1({
    sinNuevo: true, sinEliminar: true, // FrmDespacho._PMInhabilitar: btnNuevo / btnEliminar ocultos
    _PMOGetTablaBuscador: async () => {
      const q = new URLSearchParams({ desde: filtro.desde, hasta: filtro.hasta });
      return (await api.get(`${URL}?${q}`)).data.map((f) => ({ ...f, Fecha: fechaTxt(f.FechaVenta), FechaDesp: fechaTxt(f.FechaDespacho) }));
    },
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 70, '', 'num'),
      new Celda('Fecha', true, 'Fecha Venta', 100),
      new Celda('Personal', true, 'Personal', 150),
      new Celda('NombreCliente', true, 'Cliente', 180),
      new Celda('TVenta', true, 'Tipo Venta', 90),
      new Celda('Glosa', true, 'Glosa', 160),
      new Celda('NombreAlmacen', true, 'Sucursal', 120),
      new Celda('TotalVenta', true, 'Total Venta', 110, '0.00', 'num'),
      new Celda('TDespachado', true, 'Estado Despacho', 140),
      new Celda('FechaDesp', true, 'Fecha Despacho', 110),
      new Celda('Despachante', true, 'Despachante', 160),
    ],
    _PMOLimpiar: () => { setForm(VACIO); setDetalle([]); },
    _PMOMostrarRegistro: mostrar,
    _PMOLimpiarErrores: () => setErrores({}),
    // EditarToolStripMenuItem_Click
    _PMOBloqueoModificar: (f) => (f && ini && !ini.modificarAnteriores && f.FechaVenta < hoy
      ? 'No puede Modificar Despachos Anteriores a la fecha de Hoy, Consulte con su Administrador'.toUpperCase() : null),
    _PMOValidarCampos: () => {
      const e = {};
      if (!(form.PersonalDespachadorId > 0)) e.Despachador = 'Debe Seleccionar un Despachador';
      setErrores(e);
      if (e.Despachador) toast(e.Despachador.toUpperCase(), 'error');
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => Promise.reject(new Error('En Despacho no se registran ventas nuevas.')),
    // btnModificarEntregado_Click
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, {
      fecha: form.FechaVenta, PersonalId: form.PersonalDespachadorId, Despachado: form.EstadoDespacho ? 1 : 0, Observacion: form.ObservacionDespacho,
    }),
    _PMOEliminarRegistro: () => Promise.reject(new Error('En Despacho no se eliminan ventas.')),
  });
  const ro = !m.habilitado;
  // el SP (@tipo 11) no cambia despachador ni observacion de una venta ya despachada: solo se puede quitar el despacho
  const yaDespachado = Number(m.filaActual?.EstadoDespacho) === 1;
  const editaDatos = !ro && !yaDespachado;

  const primera = useRef(true);
  useEffect(() => { if (primera.current) { primera.current = false; return; } m._PMFiltrar(); }, [filtro.desde, filtro.hasta]); // eslint-disable-line react-hooks/exhaustive-deps

  // ButtonX1: "Seleccione Despachador"
  const elegirDespachador = async () => {
    try { setPersonal((await api.get(`${URL}/personal`)).data); } catch (e) { toast(e.message, 'error'); }
  };

  // BtnImprimir: solo con el despacho entregado
  const verNota = async (f) => {
    if (!f) return;
    if (Number(f.EstadoDespacho) !== 1) { toast('No se puede imprimir por que el despacho no esta entregado'.toUpperCase(), 'error'); return; }
    try { setRecibo((await api.get(`${URL}/${f.Id}/recibo?fecha=${encodeURIComponent(f.FechaVenta)}`)).data); } catch (e) { toast(e.message, 'error'); }
  };

  const suma = detalle.reduce((s, f) => s + num(f.Total), 0);

  return (
    <>
    <ModeloF1 m={m} titulo="Despacho Ventas" subtitulo="Despacho de las ventas" icono="truck"
      accionesFila={(f) => (Number(f.EstadoDespacho) === 1 ? [{ texto: 'Nota de Despacho', icono: 'printer', onClick: () => verNota(f) }] : [])}
      accionesRegistro={(
        <button className="btn btn-sec" onClick={() => verNota(m.filaActual)} disabled={!yaDespachado} title={yaDespachado ? 'Nota de Despacho' : 'El despacho no esta entregado'}>
          <Icono n="printer" size={16} /> <span className="txt-btn">Nota de Despacho</span>
        </button>
      )}
      extraBuscador={(
        <div className="filtros-lista">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={filtro.desde} max={filtro.hasta} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={filtro.hasta} min={filtro.desde} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, hasta: e.target.value }))} /></label>
        </div>
      )}>
      <div className="venta-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Despacho</h2>
          <div className="campos">
            <Campo etiqueta="Despachador" ancho={2} error={errores.Despachador} requerido={editaDatos}>
              <div className="control-con-btn">
                <input className={`control ${editaDatos ? 'control-elegir' : ''}`} value={tit(form.Despachante)} readOnly placeholder={editaDatos ? 'Toque para elegir el despachador...' : ''}
                  onClick={() => editaDatos && elegirDespachador()} onKeyDown={(e) => editaDatos && (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), elegirDespachador())} />
                {editaDatos && <button type="button" className="btn btn-sec" onClick={elegirDespachador}><Icono n="search" size={15} /> Buscar</button>}
              </div>
            </Campo>
            <Campo etiqueta="Entregado">
              <SwitchButton valor={form.EstadoDespacho} onChange={(x) => setForm((f) => ({ ...f, EstadoDespacho: x }))} deshabilitado={ro} textoOn="SI" textoOff="NO" />
            </Campo>
            {yaDespachado && <Campo etiqueta="Fecha despacho"><input className="control" value={fechaTxt(form.FechaDespacho)} readOnly /></Campo>}
            <Campo etiqueta="Observación" ancho={2}>
              <input className="control" value={form.ObservacionDespacho} maxLength={MAX_OBSERVACION} readOnly={!editaDatos}
                onChange={(e) => setForm((f) => ({ ...f, ObservacionDespacho: e.target.value }))} />
            </Campo>
          </div>
          {!ro && yaDespachado && (
            <p className="ayuda-tip">Esta venta ya está despachada: solo puede marcarla como NO entregada. Para cambiar el despachador o la observación, quite el despacho, grabe y vuelva a despacharla.</p>
          )}

          <div className="sep" />
          <h2 className="tarjeta-tit">Datos de la venta</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly /></Campo>
            <Campo etiqueta="Fecha venta"><input className="control" value={fechaTxt(form.FechaVenta)} readOnly /></Campo>
            <Campo etiqueta="Sucursal"><input className="control" value={tit(form.NombreAlmacen)} readOnly /></Campo>
            <Campo etiqueta="Tipo de venta"><input className="control" value={form.TVenta} readOnly /></Campo>
            <Campo etiqueta="Vendedor" ancho={2}><input className="control" value={tit(form.Personal)} readOnly /></Campo>
            <Campo etiqueta="Cliente" ancho={2}><input className="control" value={tit(form.NombreCliente)} readOnly /></Campo>
            <Campo etiqueta="Glosa" ancho={2}><input className="control" value={form.Glosa} readOnly /></Campo>
          </div>
        </div>

        <div className="tarjeta venta-detalle">
          <div className="detalle-cab-venta">
            <h2 className="tarjeta-tit">Detalle productos <span className="contador">{detalle.length}</span></h2>
          </div>
          {cargandoDet ? <div className="cargando-bloque"><span className="spinner" /> Cargando detalle...</div> : (
            <div className="tabla-scroll">
              <table className="grid grid-tarjetas tabla-detalle">
                <thead>
                  <tr><th>Producto</th><th>Kit</th><th className="num">Cantidad</th><th className="num">Precio</th><th className="num">M. Desc.</th><th className="num">Total</th></tr>
                </thead>
                <tbody>
                  {detalle.map((f, i) => (
                    <tr key={`${f.Id}-${i}`}>
                      <td data-label="Producto" className="det-producto"><b>{tit(f.Producto)}</b><small>Cód. {f.ProductoId}</small></td>
                      <td data-label="Kit">{Number(f.Tipo) === 2 ? `${tit(f.KitNombre)} × ${n2(f.CantidadKit)}` : '—'}</td>
                      <td data-label="Cantidad" className="num">{n2(f.Cantidad)}</td>
                      <td data-label="Precio" className="num">{bs(f.Precio)}</td>
                      <td data-label="M. Desc." className="num">{bs(f.MontoDescuento)}</td>
                      <td data-label="Total" className="num det-total"><b>{bs(f.Total)}</b></td>
                    </tr>
                  ))}
                  {!detalle.length && <tr><td colSpan={6} className="vacio">Sin productos</td></tr>}
                </tbody>
              </table>
            </div>
          )}
          <div className="venta-totales">
            <div><span>SubTotal</span><b>{bs(suma)}</b></div>
            <div><span>Descuento</span><b>{bs(form.Descuento)}</b></div>
            <div className="venta-total"><span>Total venta</span><b>Bs {bs(suma - num(form.Descuento))}</b></div>
          </div>
        </div>
      </div>

      <EfectoAyuda abierto={!!personal} titulo="SELECCIONE DESPACHADOR" dt={personal || []} columnas={COLS_PERSONAL}
        onCancelar={() => setPersonal(null)}
        onSeleccionar={(r) => { setErrores({}); setForm((f) => ({ ...f, PersonalDespachadorId: r.Id, Despachante: r.Nombre })); setPersonal(null); }} />
    </ModeloF1>
    {recibo && <NotaVenta datos={recibo} cfg={CFG_NOTA} onCerrar={() => setRecibo(null)} />}
    </>
  );
}
