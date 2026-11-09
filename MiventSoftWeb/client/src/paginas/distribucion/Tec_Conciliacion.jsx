// =====================================================================
//  Tec_Conciliacion.vb -> Conciliaciones De Productos  (SP: MAM_Conciliacion)
//  Se crean con el Despacho: aqui solo se ven, se ajustan devoluciones y se abren / cierran.
//  Matriz: una columna por salida (despacho) + Total salida / Devolución / Entregado / Diferencia.
//  No se modifica si ya tiene Cierre de Caja.
// =====================================================================
import { useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, SwitchButton, fechaCorta } from '../../componentes/Controles';
import NotaImpresa, { bsNota } from '../../componentes/NotaImpresa';
import { useToast } from '../../componentes/Toast';
import { PermisoContext, usePermisos } from '../../global/Sesion';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';

const URL = '/api/distribucion/conciliaciones';
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };

// Igual que el escritorio: sin Nuevo ni Eliminar
export default function Tec_Conciliacion() {
  const puede = usePermisos();
  return <PermisoContext.Provider value={{ ...puede, insertar: false, eliminar: false }}><Pantalla /></PermisoContext.Provider>;
}

function Pantalla() {
  const toast = useToast();
  const [filtro, setFiltro] = useState(() => { const d = new Date(); d.setDate(d.getDate() - 30); return { desde: `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`, hasta: hoy() }; });
  const [form, setForm] = useState({ Id: '', Personal: '', Fecha: '', Observacion: '', Estado: true });
  const [matriz, setMatriz] = useState({ salidas: [], filas: [] });
  const [cargandoDet, setCargandoDet] = useState(false);
  const [reporte, setReporte] = useState(false);
  const [qDet, setQDet] = useState('');

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(`${URL}?${new URLSearchParams(filtro)}`)).data.map((f) => ({
      ...f, FechaTxt: fechaCorta(`${f.Fecha}T00:00:00Z`), EstadoTxt: f.Estado === 1 ? 'ABIERTA' : 'CERRADA', CierreTxt: f.CierreCaja > 0 ? `#${f.CierreCaja}` : '',
    })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'Cod. Conciliación', 90, '', 'num'),
      new Celda('FechaTxt', true, 'Fecha Conciliación', 110),
      new Celda('NombrePersonal', true, 'Nombre Personal', 220),
      new Celda('Observacion', true, 'Detalle Conciliación', 200),
      new Celda('EstadoTxt', true, 'Estado Conciliación', 120),
      new Celda('CierreTxt', true, 'Cierre de caja', 100),
    ],
    _PMOLimpiar: () => { setForm({ Id: '', Personal: '', Fecha: hoy(), Observacion: '', Estado: true }); setMatriz({ salidas: [], filas: [] }); },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, Personal: f.NombrePersonal ?? '', Fecha: f.Fecha ?? '', Observacion: f.Observacion ?? '', Estado: f.Estado === 1 });
      setQDet('');
      setCargandoDet(true);
      api.get(`${URL}/${f.Id}/detalle?abierta=${f.Estado === 1 ? 1 : 0}`)
        .then((r) => setMatriz({ ...r.data, filas: r.data.filas.map((x) => ({ ...x, real: x.TotalDevoluciones })) }))
        .catch((e) => toast(e.message, 'error')).finally(() => setCargandoDet(false));
    },
    _PMOLimpiarErrores: () => {},
    _PMOValidarCampos: () => { if (!form.Fecha) { toast('Los Siguientes Campos Son Requeridos: Fecha', 'error'); return false; } return true; },
    _PMOModificarRegistro: (f) => api.put(`${URL}/${form.Id}`, {
      fechaOriginal: f?.Fecha ?? m.filaActual?.Fecha, Fecha: form.Fecha, Estado: form.Estado ? 1 : 0, Observacion: form.Observacion,
      devoluciones: Object.fromEntries(matriz.filas.map((x) => [x.ProductoId, x.real])),
    }),
    _PMOBloqueoModificar: (f) => (f?.CierreCaja > 0 ? 'No se puede Modificar la Conciliación por que ya tiene un Cierre de Caja Asociado'.toUpperCase() : null),
  });
  useEffect(() => { m._PMFiltrar(); }, [filtro.desde, filtro.hasta]); // eslint-disable-line react-hooks/exhaustive-deps
  const ro = !m.habilitado;

  // grDetalle_CellValueChanged: Diferencia = devolucion real - devolucion calculada (vacio = calculada)
  // Se llama en cada cambio de valor (como CellValueChanged del escritorio), no solo al salir de la celda
  const cambiarDev = (pid, txt) => {
    const vacio = txt === '' || !Number.isFinite(Number(txt));
    if (!vacio && Number(txt) < 0) { toast('La devolución no puede ser negativa', 'error'); return; }
    setMatriz((mt) => ({
      ...mt,
      filas: mt.filas.map((x) => {
        if (x.ProductoId !== pid) return x;
        const real = vacio ? x.TotalDevolucionesCalculado : Number(txt);
        return { ...x, real, Diferencia: Math.round((real - x.TotalDevolucionesCalculado) * 1e6) / 1e6 };
      }),
    }));
  };
  const tot = (k) => matriz.filas.reduce((s, x) => s + num(x[k]), 0);
  // Buscador del detalle: filtra por codigo o nombre (los totales siguen siendo de toda la conciliacion)
  const qn = qDet.trim().toLowerCase();
  const filasVis = qn ? matriz.filas.filter((x) => String(x.ProductoId).includes(qn) || String(x.NombreProducto ?? '').toLowerCase().includes(qn)) : matriz.filas;

  return (
    <ModeloF1 m={m} titulo="Conciliaciones" subtitulo="Conciliaciones De Productos" icono="check"
      accionesRegistro={<button className="btn btn-sec" onClick={() => setReporte(true)} disabled={!m.filaActual || m.habilitado || !matriz.filas.length}><Icono n="printer" size={16} /> <span className="txt-btn">Comprobante</span></button>}
      extraBuscador={(
        <div className="filtros-lista">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={filtro.desde} max={filtro.hasta} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={filtro.hasta} min={filtro.desde} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, hasta: e.target.value }))} /></label>
        </div>
      )}>
      <div className="conc-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos de la conciliación <span className={`chip-mora ${form.Estado ? 'al-dia' : 'mora-alta'}`}>{form.Estado ? 'Abierta' : 'Cerrada'}</span></h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly /></Campo>
            <Campo etiqueta="Fecha conciliación" requerido>
              <input type="date" className="control" value={form.Fecha} onChange={(e) => setForm((x) => ({ ...x, Fecha: e.target.value }))} readOnly={ro} disabled={ro} />
            </Campo>
            <Campo etiqueta="Distribuidor" ancho={2}><input className="control" value={tit(form.Personal)} readOnly /></Campo>
            <Campo etiqueta="Detalle" ancho={2}><input className="control" value={form.Observacion} onChange={(e) => setForm((x) => ({ ...x, Observacion: e.target.value }))} readOnly={ro} maxLength={500} /></Campo>
            <Campo etiqueta="Estado"><SwitchButton valor={form.Estado} onChange={(x) => setForm((f) => ({ ...f, Estado: x }))} deshabilitado={ro} textoOn="ABIERTA" textoOff="CERRADA" /></Campo>
          </div>
          {!ro && <p className="ayuda-tip conc-ayuda">Al <b>cerrar</b> la conciliación se guardan las devoluciones; el distribuidor ya no podrá hacer ventas directas con ella.</p>}
        </div>

        <div className="tarjeta">
          <h2 className="tarjeta-tit">Productos <span className="contador">{matriz.filas.length}</span> <small className="sub-rango">{matriz.salidas.length} salida{matriz.salidas.length === 1 ? '' : 's'}</small></h2>
          {cargandoDet ? <div className="cargando-bloque"><span className="spinner" /> Cargando detalle...</div> : (
            <>
            {matriz.filas.length > 0 && (
              <div className="buscar cc-buscar"><Icono n="search" size={16} /><input className="control" placeholder="Buscar producto por código o nombre..." value={qDet} onChange={(e) => setQDet(e.target.value)} aria-label="Buscar producto en la conciliación" /></div>
            )}
            <div className="tabla-scroll cc-scroll">
              {/* como el escritorio: Tot. salidas y Entregado en verde, Devoluciones en amarillo; las filas con diferencia se marcan */}
              <table className="grid grid-tarjetas tabla-detalle tabla-conc">
                <thead>
                  <tr>
                    <th className="num">Item</th><th>Producto</th>
                    {matriz.salidas.map((s, i) => <th key={s} className="num" title={`Despacho #${s}`}>Sal. #{String(i + 1).padStart(2, '0')}</th>)}
                    <th className="num cc-sal">Tot. salidas</th><th className="num cc-dev">Devoluciones</th><th className="num cc-ent">Entregado</th><th className="num cc-dif">Diferencia</th>
                  </tr>
                </thead>
                <tbody>
                  {filasVis.map((x) => {
                    const dif = num(x.Diferencia);
                    return (
                      <tr key={x.ProductoId} className={dif < 0 ? 'cc-con-dif cc-dif-neg' : dif ? 'cc-con-dif' : ''}>
                        <td data-label="Item" className="num">{x.ProductoId}</td>
                        <td data-label="Producto" className="det-producto"><b>{tit(x.NombreProducto)}</b></td>
                        {x.salidas.map((c, i) => <td key={matriz.salidas[i]} data-label={`Sal. #${String(i + 1).padStart(2, '0')}`} className="num">{c === null ? '' : n2(c)}</td>)}
                        <td data-label="Tot. salidas" className="num cc-sal"><b>{n2(x.TotalSalida)}</b></td>
                        <td data-label="Devoluciones" className="num cc-dev">
                          {ro ? <b>{n2(x.real)}</b> : <CeldaDevolucion valor={num(x.real)} onCambio={(t) => cambiarDev(x.ProductoId, t)} etiqueta={`Devolución de ${x.NombreProducto}`} />}
                          {num(x.real) !== num(x.TotalDevolucionesCalculado) && <small className="cc-calc">calc. {n2(x.TotalDevolucionesCalculado)}</small>}
                        </td>
                        <td data-label="Entregado" className="num cc-ent"><b>{n2(x.TotalEntregado)}</b></td>
                        <td data-label="Diferencia" className={`num cc-dif ${dif < 0 ? 'neg' : dif > 0 ? 'pos' : ''}`}><b>{n2(x.Diferencia)}</b></td>
                      </tr>
                    );
                  })}
                  {!filasVis.length && <tr><td colSpan={6 + matriz.salidas.length} className="vacio">{matriz.filas.length ? 'Ningún producto coincide con la búsqueda' : 'Sin productos'}</td></tr>}
                </tbody>
                {matriz.filas.length > 0 && (
                  <tfoot><tr className="tr-total">
                    <td /><td><b>TOTAL</b></td>{matriz.salidas.map((s, i) => <td key={s} className="num">{n2(matriz.filas.reduce((t, x) => t + num(x.salidas[i]), 0))}</td>)}
                    <td className="num cc-sal"><b>{n2(tot('TotalSalida'))}</b></td><td className="num cc-dev"><b>{n2(tot('real'))}</b></td>
                    <td className="num cc-ent"><b>{n2(tot('TotalEntregado'))}</b></td><td className="num cc-dif"><b>{n2(tot('Diferencia'))}</b></td>
                  </tr></tfoot>
                )}
              </table>
            </div>
            {matriz.filas.some((x) => num(x.Diferencia)) && <p className="cc-aviso"><Icono n="alert" size={15} /> {matriz.filas.filter((x) => num(x.Diferencia)).length} producto(s) con diferencia: la devolución registrada no coincide con la calculada.</p>}
            </>
          )}
        </div>
      </div>
      {reporte && <ReporteConciliacion form={form} matriz={matriz} onCerrar={() => setReporte(false)} />}
    </ModeloF1>
  );
}

// Celda de devolucion: recalcula en cada tecla (CellValueChanged) y al enfocar selecciona todo el valor.
// Mientras tiene el foco no se pisa lo que el usuario escribe; al salir muestra el valor aplicado
// (vacio -> devolucion calculada, igual que el escritorio).
const fmtCelda = (v) => String(Math.round(num(v) * 100000) / 100000);
function CeldaDevolucion({ valor, onCambio, etiqueta }) {
  const [txt, setTxt] = useState(fmtCelda(valor));
  const [foco, setFoco] = useState(false);
  const recienFoco = useRef(false); // el primer mouseup tras enfocar no debe quitar la seleccion
  useEffect(() => { if (!foco) setTxt(fmtCelda(valor)); }, [valor, foco]);
  return (
    <input className="control control-celda num" type="number" inputMode="decimal" step="any" min="0" value={txt} aria-label={etiqueta}
      onFocus={(e) => { setFoco(true); recienFoco.current = true; const el = e.target; el.select(); setTimeout(() => { if (document.activeElement === el) el.select(); }, 0); }}
      onMouseUp={(e) => { if (recienFoco.current) { e.preventDefault(); recienFoco.current = false; } }}
      onChange={(e) => { setTxt(e.target.value); onCambio(e.target.value); }}
      onBlur={() => setFoco(false)}
      onKeyDown={(e) => { if (e.key === 'Enter') { e.preventDefault(); e.currentTarget.blur(); } }} />
  );
}

// GenerarReporte (Reporte_Conciliacion01..05): "Comprobante de Conciliacion de Productos" en la hoja comun
function ReporteConciliacion({ form, matriz, onCerrar }) {
  const toast = useToast();
  const [cab, setCab] = useState(null);
  useEffect(() => { api.get(`${URL}/aux/cabecera`).then((r) => setCab(r.data)).catch((e) => { toast(e.message, 'error'); setCab({}); }); }, []); // eslint-disable-line react-hooks/exhaustive-deps
  if (!cab) return null;
  const ahora = new Date(); const dd = (x) => String(x).padStart(2, '0');
  return (
    <NotaImpresa titulo="Comprobante de Conciliacion de Productos" id={form.Id} fecha={form.Fecha} empresa={cab.empresa} imagenEmpresa={cab.imagenEmpresa} onCerrar={onCerrar}
      cabecera={[
        ['Fecha Conciliación:', form.Fecha ? fechaCorta(`${form.Fecha}T00:00:00Z`) : ''], ['Nro Conciliación:', Number(form.Id).toLocaleString('en-US')],
        ['Distribuidor:', tit(form.Personal)], ['Fecha Impresión:', `${dd(ahora.getDate())}/${dd(ahora.getMonth() + 1)}/${ahora.getFullYear()}`],
        ['Detalle:', form.Observacion || ''], ['Hora Impresión:', `${dd(ahora.getHours())}:${dd(ahora.getMinutes())}:${dd(ahora.getSeconds())}`],
      ]}
      columnas={[
        { titulo: 'Producto', valor: (x) => x.NombreProducto },
        ...matriz.salidas.map((s, i) => ({ titulo: `Salida #${String(i + 1).padStart(2, '0')}`, num: true, valor: (x) => (x.salidas[i] === null ? '' : bsNota(x.salidas[i])) })),
        { titulo: 'Total Salida', num: true, valor: (x) => n2(x.TotalSalida) }, { titulo: 'Devolucion', num: true, valor: (x) => n2(x.real) },
        { titulo: 'Entregado', num: true, valor: (x) => n2(x.TotalEntregado) }, { titulo: 'Diferencia', num: true, valor: (x) => n2(x.Diferencia) },
      ]}
      filas={matriz.filas} claveFila={(x) => x.ProductoId}
      pie={[['Productos:', String(matriz.filas.length)], ['Salidas:', String(matriz.salidas.length)]]}
      totales={[['Total Salida:', n2(matriz.filas.reduce((a, x) => a + num(x.TotalSalida), 0))], ['Devolución:', n2(matriz.filas.reduce((a, x) => a + num(x.real), 0))]]}
      etiquetaTotal="Total Entregado:" total={matriz.filas.reduce((a, x) => a + num(x.TotalEntregado), 0)} formatoTotal={n2} literal={false}
      firmas={['Encargado Almacen', 'Distribuidor']} />
  );
}
