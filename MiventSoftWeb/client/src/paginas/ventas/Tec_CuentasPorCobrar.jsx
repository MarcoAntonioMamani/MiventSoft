// =====================================================================
//  Tec_AdministrarCuentasPorCobrar.vb -> Administrar Cuentas por Cobrar  (SP: MAM_CreditosVentas)
//  Pestañas como el escritorio: Cobrar Credito · Creditos Pendientes · Creditos Cobrados · Administrar Cobros
//  Reportes impresos (ReporteImpreso):
//   - Creditos pendientes: agrupado por preventista con subtotales (Reporte_CobrosPendientes.rpt)
//   - Creditos cobrados: sin agrupar, todos o filtrados por fecha de venta (Reporte_CreditoVentaPagados.rpt)
// =====================================================================
import { useCallback, useEffect, useState } from 'react';
import { createPortal } from 'react-dom';
import { Link, useLocation } from 'react-router-dom';
import TablaReporte, { fechaTxt, valorTexto } from '../../componentes/TablaReporte';
import { Campo } from '../../componentes/Controles';
import { EfectoAyuda, EfectoConfirmar } from '../../componentes/Efecto';
import { useToast } from '../../componentes/Toast';
import { usePermisos, useSesion } from '../../global/Sesion';
import { imprimirReporte } from '../../componentes/ReporteImpreso';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';

const URL = '/api/ventas/cuentas-cobrar';
const BASE = '/ventas/cuentas-cobrar';
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };

const TABS = [
  { k: 'cobrar', t: 'Cobrar crédito', i: 'wallet' },
  { k: 'pendientes', t: 'Créditos pendientes', i: 'alert' },
  { k: 'cobrados', t: 'Créditos cobrados', i: 'check' },
  { k: 'administrar', t: 'Administrar cobros', i: 'settings' },
];

const primeroDelMes = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-01`; };

// Columnas de los reportes impresos (el texto no se corta; los montos y fechas no saltan de linea)
const IMP_PENDIENTES = [
  { campo: 'Credito', titulo: 'Crédito', nowrap: true }, { campo: 'venta', titulo: 'Venta', nowrap: true },
  { campo: 'Nombrecliente', titulo: 'Cliente', ancho: '34%' },
  { campo: 'Monto', titulo: 'Monto', tipo: 'bs', total: true }, { campo: 'abonado', titulo: 'Abonado', tipo: 'bs', total: true },
  { campo: 'Restante', titulo: 'Restante', tipo: 'bs', total: true },
  { campo: 'FechaVencimientoCredito', titulo: 'Venc. crédito', tipo: 'fecha' },
  { campo: 'DiasMora', titulo: 'Días mora', tipo: 'entero', clase: (f) => (num(f.DiasMora) > 30 ? 'rp-alerta' : '') },
];
const IMP_COBRADOS = [
  { campo: 'Credito', titulo: 'Crédito', nowrap: true }, { campo: 'venta', titulo: 'Venta', nowrap: true },
  { campo: 'FechaVenta', titulo: 'Fecha venta', tipo: 'fecha' },
  { campo: 'NombreCliente', titulo: 'Cliente', ancho: '34%' },
  { campo: 'Monto', titulo: 'Monto', tipo: 'bs', total: true }, { campo: 'Pagado', titulo: 'Pagado', tipo: 'bs', total: true },
  { campo: 'FechaVencimientoCredito', titulo: 'Venc. crédito', tipo: 'fecha' }, { campo: 'FechaUltimaPago', titulo: 'Último pago', tipo: 'fecha' },
];

const mora = (f) => (num(f.DiasMora) > 60 ? 'mora-grave' : num(f.DiasMora) > 30 ? 'mora-alta' : num(f.DiasMora) > 0 ? 'mora-baja' : '');
const chipMora = (f) => (num(f.DiasMora) > 0 ? <span className={`chip-mora ${mora(f)}`}>{num(f.DiasMora)} días</span> : <span className="chip-mora al-dia">Al día</span>);

const COLS_DEUDA = [
  { campo: 'Credito', titulo: 'Crédito' },
  { campo: 'Venta', titulo: 'Venta' },
  { campo: 'Nombre', titulo: 'Cliente', ancho: 180, render: (f) => tit(f.Nombre) },
  { campo: 'NombrePersonal', titulo: 'Preventista', render: (f) => tit(f.NombrePersonal) },
  { campo: 'Monto', titulo: 'Monto', tipo: 'bs', total: true },
  { campo: 'abonado', titulo: 'Abonado', tipo: 'bs', total: true },
  { campo: 'Restante', titulo: 'Restante', tipo: 'bs', total: true, render: (f) => <b>{bs(f.Restante)}</b> },
  { campo: 'FechaVencimientoCredito', titulo: 'Venc. crédito', tipo: 'fecha' },
  { campo: 'DiasMora', titulo: 'Mora', tipo: 'entero', render: chipMora },
];

export default function Tec_CuentasPorCobrar() {
  const toast = useToast();
  const puede = usePermisos();
  const location = useLocation();
  const tab = TABS.find((t) => location.pathname.endsWith(`/${t.k}`))?.k || 'cobrar';
  const [datos, setDatos] = useState({});
  const [cargando, setCargando] = useState(false);
  const [credito, setCredito] = useState(null); // { fila, modo: 'cobrar'|'administrar' }
  const { sesion } = useSesion();
  // Creditos cobrados: filtro por fecha de venta (todas = sin filtro)
  const [rango, setRango] = useState({ desde: primeroDelMes(), hasta: hoy(), todas: true });
  const [cabecera, setCabecera] = useState(null); // empresa + logo para los reportes impresos

  const cargar = useCallback(async (t = tab, r = rango) => {
    const ruta = { cobrar: 'deudas', pendientes: 'pendientes', cobrados: 'cobrados', administrar: 'todas' }[t];
    const qs = t === 'cobrados' && !r.todas ? `?desde=${r.desde}&hasta=${r.hasta}` : '';
    setCargando(true);
    try { const res = await api.get(`${URL}/${ruta}${qs}`); setDatos((d) => ({ ...d, [t]: res.data })); } catch (e) { toast(e.message, 'error'); } finally { setCargando(false); }
  }, [tab, rango, toast]);
  useEffect(() => { cargar(tab); }, [tab]); // eslint-disable-line react-hooks/exhaustive-deps

  const cambiarRango = (cambio) => {
    const r = { ...rango, ...cambio };
    if (!r.todas && (!r.desde || !r.hasta)) { setRango(r); return; }
    if (!r.todas && r.desde > r.hasta) { toast('La fecha Desde no puede ser mayor que Hasta', 'aviso'); setRango(r); return; }
    setRango(r);
    cargar('cobrados', r);
  };

  const imprimir = async (filasImp, opciones) => {
    if (!filasImp.length) { toast('No Existen Datos Para Generar el Reporte'.toUpperCase(), 'error'); return; }
    let cab = cabecera;
    if (!cab) {
      try { cab = (await api.get(`${URL}/cabecera-reporte`)).data; setCabecera(cab); } catch { cab = {}; }
    }
    imprimirReporte({ ...cab, usuario: tit(sesion?.gs_user || ''), filas: filasImp, ...opciones });
  };
  const imprimirPendientes = (fs) => {
    const vencido = fs.reduce((a, f) => a + (num(f.DiasMora) > 0 ? num(f.Restante) : 0), 0);
    const restante = fs.reduce((a, f) => a + num(f.Restante), 0);
    const preventistas = new Set(fs.map((f) => String(f.NombrePersonal || '').trim())).size;
    imprimir(fs, {
      titulo: 'Cobros pendientes (Cuentas por cobrar)', columnas: IMP_PENDIENTES, unidad: 'crédito',
      agruparPor: (f) => f.NombrePersonal, tituloGrupo: 'Preventista',
      datos: [['Preventistas:', String(preventistas)], ['Por cobrar:', `Bs ${bs(restante)}`], ['Vencido:', `Bs ${bs(vencido)}`]],
    });
  };
  const imprimirCobrados = (fs) => imprimir(fs, {
    titulo: 'Créditos cobrados (Cuentas por cobrar)', columnas: rango.todas ? IMP_COBRADOS.filter((c) => c.campo !== 'FechaVenta') : IMP_COBRADOS, unidad: 'crédito',
    datos: [['Fecha de venta:', rango.todas ? 'Todas' : `${fechaTxt(rango.desde)} al ${fechaTxt(rango.hasta)}`], ['Cobrado:', `Bs ${bs(fs.reduce((a, f) => a + num(f.Pagado), 0))}`]],
  });

  const filas = datos[tab] || [];
  const resumen = (datos.cobrar || datos.pendientes || []).reduce((a, f) => ({ total: a.total + num(f.Restante), vencido: a.vencido + (num(f.DiasMora) > 0 ? num(f.Restante) : 0), n: a.n + 1 }), { total: 0, vencido: 0, n: 0 });

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo">
          <span className="modelo-icono"><Icono n="wallet" size={22} /></span>
          <div><h1>Cuentas por cobrar</h1><p>Administrar Cuentas por Cobrar</p></div>
        </div>
        <div className="barra"><button className="btn-icono" title="Actualizar" aria-label="Actualizar" onClick={() => cargar()}><Icono n="refresh" size={18} /></button></div>
      </header>
      <nav className="tabs-cxc" aria-label="Secciones">
        {TABS.map((t) => (
          <Link key={t.k} to={t.k === 'cobrar' ? BASE : `${BASE}/${t.k}`} replace className={tab === t.k ? 'activa' : ''} aria-current={tab === t.k ? 'page' : undefined}>
            <Icono n={t.i} size={16} /> <span>{t.t}</span>
          </Link>
        ))}
      </nav>
      <div className="modelo-cuerpo">
        {(tab === 'cobrar' || tab === 'pendientes') && resumen.n > 0 && (
          <div className="cxc-resumen">
            <div><span>Por cobrar</span><b>Bs {bs(resumen.total)}</b><small>{resumen.n} créditos</small></div>
            <div className="vencido"><span>Vencido</span><b>Bs {bs(resumen.vencido)}</b><small>{resumen.total ? Math.round((resumen.vencido / resumen.total) * 100) : 0}% del total</small></div>
          </div>
        )}
        {tab === 'cobrar' && (
          <TablaReporte titulo="Deudas por cobrar" columnas={COLS_DEUDA} filas={filas} cargando={cargando} vacio="No hay créditos pendientes"
            onFila={(f) => setCredito({ fila: f, modo: 'cobrar' })}
            accion={(f) => <button className="btn btn-pri btn-sm" onClick={() => setCredito({ fila: f, modo: 'cobrar' })}><Icono n="dollar" size={15} /> {puede.insertar ? 'Cobrar' : 'Ver'}</button>} />
        )}
        {tab === 'pendientes' && (
          <TablaReporte titulo="Reporte de créditos pendientes" columnas={[
            { campo: 'Credito', titulo: 'Crédito' }, { campo: 'venta', titulo: 'Venta' },
            { campo: 'NombrePersonal', titulo: 'Preventista', render: (f) => tit(f.NombrePersonal) },
            { campo: 'Nombrecliente', titulo: 'Cliente', ancho: 180, render: (f) => tit(f.Nombrecliente) },
            { campo: 'Monto', titulo: 'Monto', tipo: 'bs', total: true }, { campo: 'abonado', titulo: 'Abonado', tipo: 'bs', total: true },
            { campo: 'Restante', titulo: 'Restante', tipo: 'bs', total: true }, { campo: 'FechaVencimientoCredito', titulo: 'Venc. crédito', tipo: 'fecha' },
            { campo: 'DiasMora', titulo: 'Mora', tipo: 'entero', render: chipMora },
          ]} filas={filas} cargando={cargando} vacio="No hay créditos pendientes" onImprimir={imprimirPendientes} />
        )}
        {tab === 'cobrados' && (
          <>
            <div className="cxc-filtro-fecha" role="group" aria-label="Filtrar por fecha de venta">
              <span className="cxc-filtro-tit"><Icono n="history" size={16} /> Fecha de venta</span>
              <label className="campo-fecha"><span>Desde</span>
                <input type="date" className="control" value={rango.desde} max={rango.hasta || undefined} disabled={rango.todas} onChange={(e) => cambiarRango({ desde: e.target.value })} aria-label="Fecha de venta desde" />
              </label>
              <label className="campo-fecha"><span>Hasta</span>
                <input type="date" className="control" value={rango.hasta} min={rango.desde || undefined} disabled={rango.todas} onChange={(e) => cambiarRango({ hasta: e.target.value })} aria-label="Fecha de venta hasta" />
              </label>
              <label className="cxc-todas"><input type="checkbox" checked={rango.todas} onChange={(e) => cambiarRango({ todas: e.target.checked })} /> Todas las fechas</label>
            </div>
            <TablaReporte titulo="Reporte de créditos cobrados" columnas={[
              { campo: 'Credito', titulo: 'Crédito' }, { campo: 'venta', titulo: 'Venta' },
              ...(rango.todas ? [] : [{ campo: 'FechaVenta', titulo: 'Fecha venta', tipo: 'fecha' }]),
              { campo: 'NombreCliente', titulo: 'Cliente', ancho: 180, render: (f) => tit(f.NombreCliente) },
              { campo: 'Monto', titulo: 'Monto', tipo: 'bs', total: true }, { campo: 'Pagado', titulo: 'Pagado', tipo: 'bs', total: true },
              { campo: 'FechaVencimientoCredito', titulo: 'Venc. crédito', tipo: 'fecha' }, { campo: 'FechaUltimaPago', titulo: 'Último pago', tipo: 'fecha' },
            ]} filas={filas} cargando={cargando} vacio={rango.todas ? 'No hay créditos cobrados' : 'No hay créditos cobrados de ventas en esas fechas'}
              onImprimir={imprimirCobrados} />
          </>
        )}
        {tab === 'administrar' && (
          <TablaReporte titulo="Administrar cobros" columnas={COLS_DEUDA} filas={filas} cargando={cargando}
            onFila={(f) => setCredito({ fila: f, modo: 'administrar' })}
            accion={(f) => <button className="btn btn-sec btn-sm" onClick={() => setCredito({ fila: f, modo: 'administrar' })}><Icono n="list" size={15} /> Pagos</button>} />
        )}
      </div>
      {credito && <DialogoCredito {...credito} puede={puede} onCerrar={(cambio) => { setCredito(null); if (cambio) { setDatos({}); cargar(); } }} />}
    </section>
  );
}

// ---------- Cobrar un credito / administrar sus pagos ----------
function DialogoCredito({ fila, modo, puede, onCerrar }) {
  const toast = useToast();
  const [pagos, setPagos] = useState([]);
  const [saldo, setSaldo] = useState(num(fila.Restante));
  const [personal, setPersonal] = useState({ lista: [], defecto: null });
  const [form, setForm] = useState({ PersonalId: 0, Personal: '', FechaPago: hoy(), NroComprobante: '', Glosa: '', Monto: '' });
  const [err, setErr] = useState({});
  const [ayuda, setAyuda] = useState(false);
  const [enviando, setEnviando] = useState(false);
  const [cambio, setCambio] = useState(false);
  const [borrar, setBorrar] = useState(null);
  const [estado, setEstado] = useState(null);
  const cobrar = modo === 'cobrar' && puede.insertar && saldo > 0;

  const cargarPagos = useCallback(() => api.get(`${URL}/${fila.Id}/pagos`).then((r) => setPagos(r.data)).catch((e) => toast(e.message, 'error')), [fila.Id, toast]);
  useEffect(() => { cargarPagos(); }, [cargarPagos]);
  useEffect(() => {
    if (!cobrar) return;
    api.get(`${URL}/personal`).then((r) => {
      setPersonal(r.data);
      if (r.data.defecto) setForm((x) => ({ ...x, PersonalId: r.data.defecto.Id, Personal: r.data.defecto.Nombre }));
    }).catch(() => {});
  }, [cobrar]);

  const set = (k) => (e) => { setErr((x) => ({ ...x, [k]: undefined })); setForm((x) => ({ ...x, [k]: e.target.value })); };
  const setMonto = (e) => {
    setErr((x) => ({ ...x, Monto: undefined }));
    const v = e.target.value;
    // tbMontoAPagar_ValueChanged: no puede superar el saldo
    if (num(v) > saldo) { toast(`El monto Maximo para Pagar es ${bs(saldo)}`.toUpperCase(), 'error'); setForm((x) => ({ ...x, Monto: String(saldo) })); return; }
    setForm((x) => ({ ...x, Monto: v }));
  };
  const pagar = async () => {
    const e = {};
    if (!(num(form.Monto) > 0)) e.Monto = 'El monto a Pagar Debe ser Mayor a 0';
    if (!form.FechaPago) e.FechaPago = 'Debe Seleccionar Fecha';
    setErr(e);
    if (Object.keys(e).length) { toast('Los Siguientes Campos Son Requerido', 'error'); return; }
    setEnviando(true);
    try {
      const r = await api.post(`${URL}/${fila.Id}/pagos`, { Monto: num(form.Monto), FechaPago: form.FechaPago, PersonalId: form.PersonalId, Glosa: form.Glosa, NroComprobante: form.NroComprobante });
      toast(r.mensaje, 'ok');
      setSaldo(r.saldo);
      setCambio(true);
      setForm((x) => ({ ...x, Glosa: '', NroComprobante: '', Monto: '' })); // Limpiar()
      cargarPagos();
    } catch (er) { toast(er.message, 'error', 8000); } finally { setEnviando(false); }
  };
  const confirmarBorrar = async () => {
    const p = borrar; setBorrar(null);
    try {
      const r = await api.del(`${URL}/pagos/${p.Id}`);
      toast(r.mensaje, 'ok'); setSaldo((s) => s + num(p.Monto)); setCambio(true); cargarPagos();
    } catch (er) { toast(er.message, 'error'); }
  };
  const verEstado = async () => {
    try { const r = await api.get(`${URL}/${fila.Id}/estado`); if (!r.data.length) { toast('No Existen Datos Para Generar el Reporte'.toUpperCase(), 'error'); return; } setEstado(r.data); } catch (er) { toast(er.message, 'error'); }
  };
  const totalPagado = pagos.reduce((s, p) => s + num(p.Monto), 0);

  return createPortal(
    <div className="modal-fondo" onMouseDown={() => onCerrar(cambio)}>
      <div className="modal modal-xl dialogo-credito" role="dialog" aria-modal="true" aria-label="Crédito" onMouseDown={(e) => e.stopPropagation()} onKeyDown={(e) => e.key === 'Escape' && !ayuda && !borrar && onCerrar(cambio)}>
        <div className="modal-cab">
          <h3>Venta {fila.Venta} · {tit(fila.Nombre)}</h3>
          <button className="btn-icono" onClick={() => onCerrar(cambio)} aria-label="Cerrar"><Icono n="x" /></button>
        </div>
        <div className="credito-cifras">
          <div><span>Monto venta</span><b>Bs {bs(fila.Monto)}</b></div>
          <div><span>Pagado</span><b>Bs {bs(num(fila.Monto) - saldo)}</b></div>
          <div className={saldo > 0 ? 'saldo' : 'saldo cero'}><span>Saldo adeudado</span><b>Bs {bs(saldo)}</b></div>
          <div><span>Preventista</span><b className="texto">{tit(fila.NombrePersonal) || '—'}</b></div>
        </div>
        <div className={`credito-cuerpo ${cobrar ? 'con-form' : ''}`}>
          {cobrar && (
            <div className="credito-form">
              <h4>Registrar pago</h4>
              <div className="campos campos-1">
                <Campo etiqueta="Personal">
                  <div className="control-con-btn">
                    <input className="control control-elegir" value={tit(form.Personal)} readOnly placeholder="Seleccione..." onClick={() => setAyuda(true)} />
                    <button type="button" className="btn btn-sec" onClick={() => setAyuda(true)} aria-label="Buscar personal"><Icono n="search" size={15} /></button>
                  </div>
                </Campo>
                <Campo etiqueta="Fecha de pago" error={err.FechaPago} requerido><input type="date" className="control" value={form.FechaPago} max={hoy()} onChange={set('FechaPago')} /></Campo>
                <Campo etiqueta="Nro. comprobante"><input className="control" value={form.NroComprobante} onChange={set('NroComprobante')} maxLength={50} /></Campo>
                <Campo etiqueta="Glosa"><input className="control" value={form.Glosa} onChange={set('Glosa')} maxLength={300} /></Campo>
                <Campo etiqueta="Monto a pagar (Bs)" error={err.Monto} requerido>
                  <div className="control-con-btn">
                    <input className="control num monto-pago" type="number" inputMode="decimal" min="0" step="0.01" max={saldo} value={form.Monto} onChange={setMonto} placeholder="0,00" />
                    <button type="button" className="btn btn-sec" onClick={() => setForm((x) => ({ ...x, Monto: String(Math.round(saldo * 100) / 100) }))}>Todo</button>
                  </div>
                </Campo>
              </div>
              <button className="btn btn-ok btn-bloque" onClick={pagar} disabled={enviando}>{enviando ? <span className="spinner" /> : <Icono n="check" size={17} />} Pagar</button>
            </div>
          )}
          <div className="credito-pagos">
            <div className="credito-pagos-cab">
              <h4>Listado de pagos <span className="contador">{pagos.length}</span></h4>
              {modo === 'administrar' && <button className="btn btn-sec btn-sm" onClick={verEstado}><Icono n="list" size={15} /> Estado de cuenta</button>}
            </div>
            <div className="tabla-scroll">
              <table className="grid grid-tarjetas">
                <thead><tr><th>Fecha</th><th className="num">Monto</th><th>Comprobante</th><th>Glosa</th><th>Personal</th>{modo === 'administrar' && puede.eliminar && <th aria-label="Eliminar" />}</tr></thead>
                <tbody>
                  {pagos.map((p) => (
                    <tr key={p.Id}>
                      <td data-label="Fecha">{fechaTxt(p.FechaPago)}</td>
                      <td data-label="Monto" className="num"><b>{bs(p.Monto)}</b></td>
                      <td data-label="Comprobante">{p.NroComprobante || '—'}</td>
                      <td data-label="Glosa">{p.Glosa || '—'}</td>
                      <td data-label="Personal">{tit(p.NombrePersonal)}</td>
                      {modo === 'administrar' && puede.eliminar && <td className="col-acc"><button className="btn-icono peligro" onClick={() => setBorrar(p)} aria-label="Eliminar pago" title="Eliminar pago"><Icono n="trash" size={16} /></button></td>}
                    </tr>
                  ))}
                  {!pagos.length && <tr><td colSpan={6} className="vacio">Sin pagos registrados</td></tr>}
                </tbody>
                {pagos.length > 0 && <tfoot><tr className="tr-total"><td className="tf-tit"><b>TOTAL ({pagos.length} {pagos.length === 1 ? 'pago' : 'pagos'})</b></td><td className="num" data-label="Total pagado"><b>{bs(totalPagado)}</b></td><td colSpan={4} /></tr></tfoot>}
              </table>
            </div>
          </div>
        </div>
      </div>
      <EfectoAyuda abierto={ayuda} titulo="SELECCIONE PERSONAL" dt={personal.lista}
        columnas={[new Celda('Nombre', true, 'NOMBRE', 300), new Celda('Direccion', true, 'DIRECCION', 180), new Celda('Telefono01', true, 'TELEFONO', 140)]}
        onCancelar={() => setAyuda(false)} onSeleccionar={(r) => { setForm((x) => ({ ...x, PersonalId: r.Id, Personal: r.Nombre })); setAyuda(false); }} />
      <EfectoConfirmar abierto={!!borrar} titulo="Eliminación De Pagos"
        descripcion={borrar ? `¿Esta Seguro de Eliminar el Pago de Fecha ${fechaTxt(borrar.FechaPago)} Con Monto de ${bs(borrar.Monto)} ?` : ''}
        onAceptar={confirmarBorrar} onCancelar={() => setBorrar(null)} />
      {estado && <EstadoCuenta filas={estado} fila={fila} onCerrar={() => setEstado(null)} />}
    </div>,
    document.body,
  );
}

// Reporte_VentaEstadoCuentas: columnas tal como las devuelve MAM_CreditosVentas @tipo 8
function EstadoCuenta({ filas, fila, onCerrar }) {
  const cols = Object.keys(filas[0] || {}).map((k) => ({ campo: k, titulo: k, tipo: typeof filas[0][k] === 'number' ? 'num' : /fecha/i.test(k) ? 'fecha' : 'texto' }));
  return (
    <div className="modal-fondo modal-encima" onMouseDown={(e) => { e.stopPropagation(); onCerrar(); }}>
      <div className="modal modal-lg nota-venta" role="dialog" aria-modal="true" aria-label="Estado de cuenta" onMouseDown={(e) => e.stopPropagation()}>
        <div className="modal-cab no-imprimir"><h3>Estado de cuenta</h3><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="nota-papel">
          <header className="nota-cab"><div><b className="nota-empresa">ESTADO DE CUENTA</b><span>Venta {fila.Venta} · {fila.Nombre}</span></div><div className="nota-num"><span>CRÉDITO</span><b>N° {fila.Credito}</b></div></header>
          <div className="tabla-scroll">
            <table className="nota-tabla">
              <thead><tr>{cols.map((c) => <th key={c.campo} className={c.tipo === 'num' ? 'num' : ''}>{c.titulo}</th>)}</tr></thead>
              <tbody>{filas.map((f, i) => <tr key={i}>{cols.map((c) => <td key={c.campo} className={c.tipo === 'num' ? 'num' : ''}>{valorTexto(c, f[c.campo])}</td>)}</tr>)}</tbody>
            </table>
          </div>
        </div>
        <div className="modal-acciones no-imprimir"><button className="btn btn-sec" onClick={onCerrar}>Cerrar</button><button className="btn btn-pri" onClick={() => imprimirReporte({
          titulo: 'Estado de cuenta', columnas: cols.map((c) => ({ ...c, tipo: c.tipo === 'num' ? 'bs' : c.tipo })), filas, unidad: 'movimiento',
          datos: [['Crédito:', `N° ${fila.Credito}`], ['Venta:', String(fila.Venta ?? '')], ['Nombre:', String(fila.Nombre ?? '')]],
        })}><Icono n="list" size={16} /> Imprimir</button></div>
      </div>
    </div>
  );
}
