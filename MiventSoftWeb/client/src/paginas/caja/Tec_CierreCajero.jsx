// =====================================================================
//  KAILY LA PAZ: Tec_CierreCajaCajero.vb -> Cierre Caja Cajero   (SP: MAM_CierreCajero)
//   Nuevo     = abrir la caja (monto inicial, tipo de cambio, detalle)
//   Modificar = cerrar la caja: se cargan las ventas, cobranzas e ingresos/egresos pendientes
//               y se cuentan los billetes (Bs y $us). Una caja cerrada no se modifica.
//  Total caja = Monto inicial + Ventas + Cobranzas + Ingresos − Egresos
//  Efectivo   = Σ billetes Bs + Σ billetes $us × tipo de cambio ; Diferencia = Efectivo − Total caja
//  El navegador calcula para mostrar; al grabar el servidor recalcula con los mismos SP.
// =====================================================================
import { useEffect, useRef, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, SwitchButton, fechaCorta } from '../../componentes/Controles';
import TablaReporte from '../../componentes/TablaReporte';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/caja/cierre-cajero';
const dos = (n) => String(n).padStart(2, '0');
const hoyLocal = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const c = (f, n) => { const k = Object.keys(f).find((x) => x.toLowerCase() === n.toLowerCase()); return k === undefined ? undefined : f[k]; };

const VACIO = { Id: '', Fecha: '', PersonalId: 0, NombrePersonal: '', SucursalId: '', MontoInicial: 0, TipoCambio: 0, EstadoCaja: 1, Observacion: '' };
const SIN_DETALLE = { ventas: [], cobranzas: [], movimientos: [], cortes: [] };

// Columnas de las grillas del escritorio
const COL_VENTAS = [
  { campo: 'Id', titulo: 'Cod Venta', tipo: 'entero' }, { campo: 'contado', titulo: 'Contado Bs', tipo: 'bs', total: true },
  { campo: 'contado$', titulo: 'Contado $us', tipo: 'bs', total: true }, { campo: 'tarjeta', titulo: 'Tarjeta', tipo: 'bs', total: true },
  { campo: 'TransferenciaBancaria', titulo: 'Transferencia', tipo: 'bs', total: true }, { campo: 'credito', titulo: 'Crédito', tipo: 'bs', total: true },
  { campo: 'totalbs', titulo: 'Total Venta Bs', tipo: 'bs', total: true },
];
const COL_COBROS = [
  { campo: 'FechaPago', titulo: 'Fecha Pago', tipo: 'fecha' }, { campo: 'NroComprobanteVenta', titulo: 'Comprobante Venta' },
  { campo: 'Monto', titulo: 'Monto Pagado', tipo: 'bs', total: true },
];
const COL_MOV = [
  { campo: 'Fecha', titulo: 'Fecha', tipo: 'fecha' }, { campo: 'Movimiento', titulo: 'Movimiento' }, { campo: 'NombreTipoMovimiento', titulo: 'Tipo Movimiento' },
  { campo: 'Descripcion', titulo: 'Descripción' }, { campo: 'Monto', titulo: 'Monto', tipo: 'bs', total: true },
];
// Las grillas usan el nombre de la columna en cualquier mayuscula
const normal = (filas, cols) => filas.map((f, i) => ({ __no: i, ...Object.fromEntries(cols.map((x) => [x.campo, c(f, x.campo)])), IngresoEgreso: c(f, 'IngresoEgreso') }));

// Calculartotales + _prCalcular
function totales(montoInicial, tipoCambio, d) {
  const s = (dt, k) => dt.reduce((a, f) => a + num(f[k]), 0);
  const ingresos = d.movimientos.filter((f) => num(f.IngresoEgreso) === 1).reduce((a, f) => a + num(f.Monto), 0);
  const egresos = d.movimientos.filter((f) => num(f.IngresoEgreso) !== 1).reduce((a, f) => a + num(f.Monto), 0);
  const ventas = s(d.ventas, 'totalbs');
  const cobros = s(d.cobranzas, 'Monto');
  const caja = num(montoInicial) + ventas + cobros + ingresos - egresos;
  const efectivo = d.cortes.reduce((a, x) => a + num(x.CorteBs) * num(x.CantidadBs), 0) + d.cortes.reduce((a, x) => a + num(x.CorteDolares) * num(x.CantidadDolares), 0) * num(tipoCambio);
  return { ventas, cobros, ingresos, egresos, caja, tarjeta: s(d.ventas, 'tarjeta'), transferencia: s(d.ventas, 'TransferenciaBancaria'), efectivo, diferencia: efectivo - caja };
}

export default function Tec_CierreCajero() {
  const toast = useToast();
  const [form, setForm] = useState(VACIO);
  const [ini, setIni] = useState(null);
  const [det, setDet] = useState(SIN_DETALLE);
  const [cargandoDet, setCargandoDet] = useState(false);
  const [filtro, setFiltro] = useState({ desde: hoyLocal(), hasta: hoyLocal() });
  const pedido = useRef(0);
  const set = (k) => (x) => setForm((f) => ({ ...f, [k]: x?.target ? x.target.value : x }));

  useEffect(() => { api.get(`${URL}/inicio`).then((r) => setIni(r.data)).catch((e) => toast(e.message, 'error')); }, []); // eslint-disable-line react-hooks/exhaustive-deps
  useAlActivar(() => {
    if (m.modo !== 'ver') return; // un cierre a medio registrar no se toca (cambiar ini lo limpiaria)
    api.get(`${URL}/inicio`).then((r) => setIni(r.data)).catch((e) => toast(e.message, 'error'));
  }, { alMontar: false });

  const cargarDetalle = async (f, pendiente) => {
    const n = ++pedido.current;
    setDet(SIN_DETALLE);
    setCargandoDet(true);
    try {
      const r = await api.get(`${URL}/${f.Id}/detalle?fecha=${encodeURIComponent(f.Fecha)}${pendiente ? '&pendiente=1' : ''}`);
      if (n !== pedido.current) return;
      setDet({
        ventas: normal(r.data.ventas, COL_VENTAS), cobranzas: normal(r.data.cobranzas, COL_COBROS), movimientos: normal(r.data.movimientos, COL_MOV),
        cortes: r.data.cortes.map((x) => ({ CorteBs: num(c(x, 'CorteBs')), CantidadBs: num(c(x, 'CantidadBs')), CorteDolares: num(c(x, 'CorteDolares')), CantidadDolares: num(c(x, 'CantidadDolares')) })),
      });
    } catch (e) { toast(e.message, 'error'); } finally { if (n === pedido.current) setCargandoDet(false); }
  };

  // _PMOLimpiar + btnNuevo_Click: personal del usuario, sucursal del usuario, tipo de cambio vigente
  const limpiar = (d = ini) => {
    setDet(SIN_DETALLE);
    const fija = d && Number(d.sucursalUsuario) > 0;
    setForm({ ...VACIO, Fecha: d?.hoy || hoyLocal(), PersonalId: d?.personal?.Id ?? 0, NombrePersonal: d?.personal?.Nombre ?? '',
      SucursalId: fija ? d.sucursalUsuario : (d?.sucursales?.[0]?.aanumi ?? ''), TipoCambio: num(d?.tipoCambio) });
  };

  const bloqueo = (accion) => (f) => {
    if (!f) return null;
    if (accion === 'Modificar' && Number(f.EstadoCaja) === 0) return 'No Se Puede Modificar La Caja Por que Ya Se Encuentra con Estado Cerrada. Elimine la Caja Y Vuelva a Realizar El Cierre';
    if (!ini?.modificarAnteriores && f.Fecha < (ini?.hoy || hoyLocal())) return `No puede ${accion} Cierre Anteriores a la fecha de Hoy, Consulte con su Administrador`.toUpperCase();
    return null;
  };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => {
      const q = new URLSearchParams(filtro);
      return (await api.get(`${URL}?${q}`)).data.map((f) => ({ ...f, FechaTxt: fechaCorta(`${f.Fecha}T00:00:00Z`) }));
    },
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'Cierre', 70, '', 'num'),
      new Celda('FechaTxt', true, 'Fecha', 100),
      new Celda('NombrePersonal', true, 'Personal', 170),
      new Celda('Sucursal', true, 'Sucursal', 130),
      new Celda('Caja', true, 'Estado Caja', 110),
      new Celda('TotalEfectivoRecibido', true, 'E. Recibido', 110, '0.00', 'num'),
      new Celda('Observacion', true, 'Detalle', 180),
    ],
    _PMOLimpiar: () => limpiar(),
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, Fecha: f.Fecha, PersonalId: f.PersonalId, NombrePersonal: f.NombrePersonal ?? '', SucursalId: f.SucursalId ?? '', MontoInicial: num(f.MontoInicial),
        TipoCambio: num(f.TipoCambio), EstadoCaja: Number(f.EstadoCaja), Observacion: f.Observacion ?? '' });
      cargarDetalle(f, false);
    },
    _PMOLimpiarErrores: () => {},
    _PMOBloqueoModificar: bloqueo('Modificar'),
    _PMOBloqueoEliminar: bloqueo('Eliminar'),
    _PMOValidarCampos: () => {
      if (!form.SucursalId) { toast('Los Siguientes Campos Son Requeridos: Sucursal', 'error'); return false; }
      return true;
    },
    _PMOGrabarRegistro: () => api.post(URL, { SucursalId: form.SucursalId, MontoInicial: num(form.MontoInicial), TipoCambio: num(form.TipoCambio), Observacion: form.Observacion }),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, {
      fechaOriginal: form.Fecha, SucursalId: form.SucursalId, MontoInicial: num(form.MontoInicial), TipoCambio: num(form.TipoCambio), Observacion: form.Observacion,
      cortes: det.cortes.map((x) => ({ CantidadBs: x.CantidadBs, CantidadDolares: x.CantidadDolares })),
    }),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}?fecha=${encodeURIComponent(f.Fecha)}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar Cierre Caja #${f.Id} ?`,
  });
  const ro = !m.habilitado;
  const cerrando = m.modo === 'modificar';

  const primera = useRef(true);
  useEffect(() => { if (primera.current) { primera.current = false; return; } m._PMFiltrar(); }, [filtro.desde, filtro.hasta]); // eslint-disable-line react-hooks/exhaustive-deps
  useEffect(() => { if (m.modo === 'nuevo' && ini) limpiar(ini); }, [ini]); // eslint-disable-line react-hooks/exhaustive-deps
  // Modificar = cerrar: btnCargarDatos (ventas, cobranzas y movimientos pendientes + billetes en 0)
  useEffect(() => { if (cerrando && form.Id) cargarDetalle({ Id: form.Id, Fecha: form.Fecha }, true); }, [cerrando]); // eslint-disable-line react-hooks/exhaustive-deps

  const t = totales(form.MontoInicial, form.TipoCambio, det);
  const cambiarCorte = (i, k, x) => setDet((d) => ({ ...d, cortes: d.cortes.map((y, j) => (j === i ? { ...y, [k]: Math.max(0, Math.floor(num(x))) } : y)) }));
  const cortesVista = cerrando ? det.cortes : det.cortes.filter((x) => num(x.CantidadBs) > 0 || num(x.CantidadDolares) > 0);
  const sucursalFija = ini && Number(ini.sucursalUsuario) > 0;
  const nuevo = m.modo === 'nuevo';

  return (
    <ModeloF1 m={m} titulo="Cierre de caja" subtitulo="Cierre Caja Cajero" icono="calc"
      menuNuevo={m.puede.insertar ? [{ texto: 'Abrir caja', icono: 'calc', accion: () => m._PMNuevo() }] : []}
      extraBuscador={(
        <div className="filtros-lista">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={filtro.desde} max={filtro.hasta} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={filtro.hasta} min={filtro.desde} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, hasta: e.target.value }))} /></label>
          <span className="ayuda-tip">Nuevo = abrir caja · Modificar = cerrar caja</span>
        </div>
      )}>
      <div className="registro-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">{nuevo ? 'Abrir caja' : cerrando ? 'Cerrar caja' : 'Datos del cierre'}</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Fecha"><input className="control" value={form.Fecha ? fechaCorta(`${form.Fecha}T00:00:00Z`) : ''} readOnly /></Campo>
            <Campo etiqueta="Estado caja"><SwitchButton valor={nuevo || (Number(form.EstadoCaja) === 1 && !cerrando)} onChange={() => {}} deshabilitado textoOn="ABIERTA" textoOff="CERRADA" /></Campo>
            <Campo etiqueta="Personal" ancho={2}><input className="control" value={tit(form.NombrePersonal)} readOnly /></Campo>
            <Campo etiqueta="Sucursal" requerido>
              <ComboMulti dt={ini?.sucursales || []} valor={form.SucursalId} onChange={set('SucursalId')} id="aanumi" nameId="Código" descripcionId="aabdes" descripcion="Sucursal" deshabilitado={ro || sucursalFija} />
            </Campo>
            <Campo etiqueta="Monto inicial"><input className="control num" type="number" min="0" step="0.01" value={form.MontoInicial} onChange={set('MontoInicial')} readOnly={ro} /></Campo>
            <Campo etiqueta="Tipo de cambio"><input className="control num" type="number" min="0" step="0.01" value={form.TipoCambio} onChange={set('TipoCambio')} readOnly={ro} /></Campo>
            <Campo etiqueta="Detalle" ancho={3}><input className="control" value={form.Observacion} onChange={set('Observacion')} readOnly={ro} maxLength={500} /></Campo>
          </div>
        </div>

        {!nuevo && (
          <div className="tarjeta">
            <h2 className="tarjeta-tit">Totales</h2>
            <div className="venta-totales">
              <div><span>Monto inicial</span><b>{bs(form.MontoInicial)}</b></div>
              <div><span>Ventas (contado y cobranza)</span><b>{bs(t.ventas)}</b></div>
              <div><span>Cobranzas</span><b>{bs(t.cobros)}</b></div>
              <div><span>Ingresos</span><b>{bs(t.ingresos)}</b></div>
              <div><span>Gastos</span><b>− {bs(t.egresos)}</b></div>
              <div className="venta-total"><span>Total caja</span><b>Bs {bs(t.caja)}</b></div>
              <div><span>Tarjeta</span><b>{bs(t.tarjeta)}</b></div>
              <div><span>Transferencia</span><b>{bs(t.transferencia)}</b></div>
              <div><span>Efectivo recibido</span><b>{bs(t.efectivo)}</b></div>
              <div className="venta-total"><span>Diferencia</span><b className={t.diferencia < 0 ? 'texto-peligro' : ''}>Bs {bs(t.diferencia)}</b></div>
            </div>
          </div>
        )}
      </div>

      {!nuevo && (cargandoDet ? <div className="cargando-bloque"><span className="spinner" /> Cargando datos de la caja...</div> : (
        <>
          <div className="tarjeta">
            <h2 className="tarjeta-tit">Conteo de efectivo</h2>
            <div className="tabla-scroll">
              <table className="grid tabla-detalle">
                <thead><tr><th className="num">Billete Bs</th><th className="num">Cantidad Bs</th><th className="num">SubTotal Bs</th><th className="num">Billete $us</th><th className="num">Cantidad $us</th><th className="num">SubTotal $us</th></tr></thead>
                <tbody>
                  {cortesVista.map((x) => {
                    const i = det.cortes.indexOf(x);
                    return (
                      <tr key={i}>
                        <td className="num">{bs(x.CorteBs)}</td>
                        <td className="num">{cerrando ? <input className="control control-celda num" type="number" min="0" step="1" value={x.CantidadBs} onChange={(e) => cambiarCorte(i, 'CantidadBs', e.target.value)} aria-label={`Cantidad de billetes de ${x.CorteBs} Bs`} /> : x.CantidadBs}</td>
                        <td className="num">{bs(x.CorteBs * x.CantidadBs)}</td>
                        <td className="num">{x.CorteDolares ? bs(x.CorteDolares) : '—'}</td>
                        <td className="num">{cerrando && x.CorteDolares ? <input className="control control-celda num" type="number" min="0" step="1" value={x.CantidadDolares} onChange={(e) => cambiarCorte(i, 'CantidadDolares', e.target.value)} aria-label={`Cantidad de billetes de ${x.CorteDolares} dolares`} /> : (x.CorteDolares ? x.CantidadDolares : '')}</td>
                        <td className="num">{x.CorteDolares ? bs(x.CorteDolares * x.CantidadDolares) : ''}</td>
                      </tr>
                    );
                  })}
                  {!cortesVista.length && <tr><td colSpan={6} className="vacio">{Number(form.EstadoCaja) === 1 ? 'La caja está abierta: use Modificar para cerrarla.' : 'Sin efectivo registrado'}</td></tr>}
                </tbody>
              </table>
            </div>
          </div>
          <div className="tarjeta"><h2 className="tarjeta-tit">Ventas <span className="contador">{det.ventas.length}</span></h2>
            <TablaReporte titulo="VENTAS DEL CIERRE" columnas={COL_VENTAS} filas={det.ventas} claveFila="__no" vacio="Sin ventas" /></div>
          <div className="tarjeta"><h2 className="tarjeta-tit">Cobranzas <span className="contador">{det.cobranzas.length}</span></h2>
            <TablaReporte titulo="COBRANZAS DEL CIERRE" columnas={COL_COBROS} filas={det.cobranzas} claveFila="__no" vacio="Sin cobranzas" /></div>
          <div className="tarjeta"><h2 className="tarjeta-tit">Ingresos y egresos <span className="contador">{det.movimientos.length}</span></h2>
            <TablaReporte titulo="INGRESOS Y EGRESOS DEL CIERRE" columnas={COL_MOV} filas={det.movimientos} claveFila="__no" vacio="Sin movimientos" /></div>
        </>
      ))}
    </ModeloF1>
  );
}
