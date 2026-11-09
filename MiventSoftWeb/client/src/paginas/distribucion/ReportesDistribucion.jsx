// =====================================================================
//  Reportes del panel Distribución (cada uno con su programa y su URL)
//   gastos-chofer · ingresos-egresos-anual · ingresos-egresos-detallado · historico-clientes
//   productos-por-cliente · auditoria-ventas · historial-incremento
//  Columnas tal como las devuelve cada SP (TablaReporte: buscar, totales, Excel, imprimir).
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import { createPortal } from 'react-dom';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { columnasDe } from '../ventas/Reportes';

const URL = '/api/distribucion/reportes';
const dos = (n) => String(n).padStart(2, '0');
const isoLocal = (d) => `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`;
const hoy = () => isoLocal(new Date());
const haceDias = (n) => { const d = new Date(); d.setDate(d.getDate() - n); return isoLocal(d); };
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const TODOS = (lista, campo = 'Nombre') => [{ Id: -1, [campo]: 'TODOS' }, ...lista];

export const CONFIG_DIST = {
  'gastos-chofer': { titulo: 'Gastos del chofer', sub: 'Reporte Gastos Chofer', icono: 'receipt', filtros: ['fechas', 'gasto', 'personal'] },
  'ingresos-egresos-anual': { titulo: 'Ingresos y egresos anual', sub: 'Reporte Ingresos Egresos Anual', icono: 'chart', filtros: ['anio'] },
  'ingresos-egresos-detallado': { titulo: 'Ingresos y egresos detallado', sub: 'Reporte Ingresos Egresos Detallado', icono: 'bars', filtros: ['fechas'] },
  'historico-clientes': { titulo: 'Histórico de registro de clientes', sub: 'Historico Registro Clientes', icono: 'globe', filtros: ['fechas', 'personal'] },
  'productos-por-cliente': { titulo: 'Ventas de productos por cliente', sub: 'Ventas Productos Por Cliente', icono: 'area', filtros: ['fechas', 'proveedor', 'categoria', 'cliente', 'producto'] },
  'auditoria-ventas': { titulo: 'Auditoría de ventas', sub: 'Auditoria Ventas', icono: 'clipboard', filtros: ['fechas', 'evento'] },
  'historial-incremento': { titulo: 'Historial del incremento de ventas', sub: 'Historial Incremento Ventas', icono: 'history', filtros: ['fechas'], desde: 30 },
};
const EVENTOS = [{ Id: 'TODOS', Nombre: 'TODOS' }, { Id: 'ANULADO', Nombre: 'ANULADOS' }, { Id: 'ELIMINADO', Nombre: 'ELIMINADOS' }, { Id: 'MODIFICADO', Nombre: 'MODIFICADOS' }];
const CHIP_EVENTO = { ANULADO: 'mora-alta', ELIMINADO: 'mora-grave', MODIFICADO: 'mora-baja' };

export default function ReporteDistribucion({ clave }) {
  const cfg = CONFIG_DIST[clave];
  const toast = useToast();
  const [f, setF] = useState({ desde: haceDias(cfg.desde || 0), hasta: hoy(), gasto: -1, personal: -1, anio: '', evento: 'TODOS', proveedor: -1, categoria: -1, cliente: -1, producto: -1 });
  const [aux, setAux] = useState({});
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(false);
  const [evento, setEvento] = useState(null);
  const set = (k) => (x) => setF((o) => ({ ...o, [k]: x }));
  const tiene = (x) => cfg.filtros.includes(x);

  useEffect(() => {
    setDatos(null);
    const carga = [];
    if (tiene('personal')) carga.push(api.get(`${URL}/aux/personal`).then((r) => ({ personal: TODOS(r.data) })));
    if (tiene('gasto')) carga.push(api.get(`${URL}/gastos-chofer/combos`).then((r) => ({ gastos: TODOS(r.data.tiposGasto.map((x) => ({ Id: x.cnnum, Nombre: x.cndesc1 }))) })));
    if (tiene('anio')) carga.push(api.get(`${URL}/ingresos-egresos-anual/anios`).then((r) => ({ anios: r.data.map((x) => ({ Id: x.cndesc1, Nombre: x.cndesc1 })) })));
    if (tiene('proveedor')) carga.push(api.get(`${URL}/productos-por-cliente/combos`).then((r) => ({ prov: TODOS(r.data.proveedores), cat: TODOS(r.data.categorias), cli: TODOS(r.data.clientes, 'NombreCliente'), prod: TODOS(r.data.productos) })));
    Promise.all(carga).then((xs) => {
      const a = Object.assign({}, ...xs);
      setAux(a);
      if (a.anios?.length) setF((o) => ({ ...o, anio: a.anios[a.anios.length - 1].Id })); // cbAnio: ultimo año
    }).catch((e) => toast(e.message, 'error'));
  }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps

  const generar = async () => {
    const q = new URLSearchParams();
    if (tiene('fechas')) { q.set('desde', f.desde); q.set('hasta', f.hasta); }
    ['gasto', 'personal', 'anio', 'evento', 'proveedor', 'categoria', 'cliente', 'producto'].forEach((k) => tiene(k) && q.set(k, f[k]));
    setCargando(true);
    try {
      const r = await api.get(`${URL}/${clave}?${q}`);
      setDatos(r.data);
      const n = Array.isArray(r.data) ? r.data.length : r.data.filas.length;
      if (!n) toast('No Existen Datos Para Mostrar con Los Filtros Seleccionados'.toUpperCase(), 'error');
    } catch (e) { toast(e.message, 'error', 7000); } finally { setCargando(false); }
  };

  const filas = Array.isArray(datos) ? datos : datos?.filas || [];
  const columnas = useMemo(() => {
    if (clave === 'historico-clientes') {
      return [{ campo: 'Fecha', titulo: 'Fecha', tipo: 'fecha' }, { campo: 'Accion', titulo: 'Acción' }, { campo: 'NombrePersonal', titulo: 'Personal', render: (x) => tit(x.NombrePersonal) },
        { campo: 'diferencia', titulo: 'Campos modificados o creados', render: (x) => <Cambios cambios={x.cambios} ubicacion={x.ubicacion} /> }];
    }
    if (clave === 'ingresos-egresos-anual') return columnasDe(filas).map((c) => (c.campo === 'Concepto' ? { ...c, render: (x) => <b>{x.Concepto}</b> } : { ...c, tipo: 'bs', total: false }));
    const cols = columnasDe(filas);
    if (clave === 'auditoria-ventas') return cols.map((c) => (c.campo === 'TipoEvento' ? { ...c, render: (x) => <span className={`chip-mora ${CHIP_EVENTO[x.TipoEvento] || 'al-dia'}`}>{x.TipoEvento}</span> } : c));
    return cols;
  }, [filas, clave]);
  const rango = tiene('fechas') ? `Del ${f.desde.split('-').reverse().join('/')} al ${f.hasta.split('-').reverse().join('/')}` : `Año ${f.anio}`;
  const combo = (k, etq, dt, desc = 'Nombre') => (
    <div className="filtro filtro-ancho"><span>{etq}</span><ComboMulti dt={dt || []} valor={f[k]} onChange={set(k)} id="Id" nameId="Cód." descripcionId={desc} descripcion={etq} /></div>
  );

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo">
          <span className="modelo-icono"><Icono n={cfg.icono} size={22} /></span>
          <div><h1>{cfg.titulo}</h1><p>{cfg.sub}</p></div>
        </div>
      </header>
      <div className="modelo-cuerpo">
        <div className="filtros-lista filtros-reporte">
          {tiene('fechas') && (
            <>
              <label className="filtro"><span>Desde</span><input type="date" className="control" value={f.desde} max={f.hasta} onChange={(e) => e.target.value && set('desde')(e.target.value)} /></label>
              <label className="filtro"><span>Hasta</span><input type="date" className="control" value={f.hasta} min={f.desde} onChange={(e) => e.target.value && set('hasta')(e.target.value)} /></label>
            </>
          )}
          {tiene('anio') && combo('anio', 'Año', aux.anios)}
          {tiene('gasto') && combo('gasto', 'Tipo de gasto', aux.gastos)}
          {tiene('personal') && combo('personal', 'Personal', aux.personal)}
          {tiene('evento') && combo('evento', 'Acción', EVENTOS)}
          {tiene('proveedor') && combo('proveedor', 'Proveedor', aux.prov)}
          {tiene('categoria') && combo('categoria', 'Categoría', aux.cat)}
          {tiene('cliente') && combo('cliente', 'Cliente', aux.cli, 'NombreCliente')}
          {tiene('producto') && combo('producto', 'Producto', aux.prod)}
          <button className="btn btn-pri filtro-boton" onClick={generar} disabled={cargando || (tiene('anio') && !f.anio)}>{cargando ? <span className="spinner" /> : <Icono n="search" size={16} />} Generar</button>
        </div>

        {datos && clave === 'ingresos-egresos-anual' && datos.grafico && <GraficoAnual datos={datos.grafico} anio={f.anio} />}
        {datos && (
          <TablaReporte titulo={cfg.sub.toUpperCase()} subtitulo={rango} columnas={columnas} filas={filas} cargando={cargando} claveFila={clave === 'auditoria-ventas' ? 'Id' : '__no'}
            vacio="No Existen Datos Para Mostrar"
            onFila={clave === 'auditoria-ventas' ? setEvento : undefined}
            accion={clave === 'auditoria-ventas' ? (x) => <button className="btn btn-txt btn-sm" onClick={() => setEvento(x)}><Icono n="eye" size={15} /> Detalle</button> : undefined} />
        )}
        {!datos && !cargando && <div className="reporte-vacio"><Icono n="search" size={28} /><span>Elija los filtros y presione <b>Generar</b></span></div>}
      </div>
      {evento && <DetalleEvento ev={evento} onCerrar={() => setEvento(null)} />}
    </section>
  );
}

// Rep_Modificaciones: lista de campos cambiados (antes -> despues) y la ubicacion registrada
function Cambios({ cambios = [], ubicacion }) {
  if (!cambios.length) return <span className="txt-suave">Sin cambios</span>;
  return (
    <ul className="lista-cambios">
      {cambios.map((c) => <li key={c.campo}><b>{c.campo}:</b> {c.antes !== '' && <><s>{c.antes}</s> → </>}{c.despues}</li>)}
      {ubicacion && <li className="txt-suave"><Icono n="pin" size={13} /> {ubicacion.lat.toFixed(5)}, {ubicacion.lon.toFixed(5)}</li>}
    </ul>
  );
}

// Grafico de ingresos vs egresos por mes (barras agrupadas)
function GraficoAnual({ datos, anio }) {
  const max = Math.max(1, ...datos.map((d) => Math.max(d.ingreso, d.egreso)));
  return (
    <div className="tarjeta graf-anual">
      <div className="graf-anual-cab"><h2 className="tarjeta-tit">Ingresos vs egresos {anio}</h2>
        <div className="leyenda"><span><i className="dot ing" /> Ingresos</span><span><i className="dot egr" /> Egresos</span></div></div>
      <div className="graf-anual-barras" role="img" aria-label={`Ingresos y egresos por mes de ${anio}`}>
        {datos.map((d) => (
          <div key={d.mes} className="ga-mes" title={`${d.mes}: ingresos Bs ${bs(d.ingreso)} · egresos Bs ${bs(d.egreso)}`}>
            <div className="ga-col">
              <span className="ga-barra ing" style={{ height: `${(d.ingreso / max) * 100}%` }} />
              <span className="ga-barra egr" style={{ height: `${(d.egreso / max) * 100}%` }} />
            </div>
            <small>{d.mes.slice(0, 3)}</small>
          </div>
        ))}
      </div>
    </div>
  );
}

// Rep_AuditoriaVentas: detalle del evento (productos agregados / eliminados / cantidad modificada)
function DetalleEvento({ ev, onCerrar }) {
  const toast = useToast();
  const [filas, setFilas] = useState(null);
  useEffect(() => { api.get(`${URL}/auditoria-ventas/${ev.Id}/detalle`).then((r) => setFilas(r.data)).catch((e) => { toast(e.message, 'error'); onCerrar(); }); }, [ev.Id]); // eslint-disable-line react-hooks/exhaustive-deps
  useEffect(() => { const k = (e) => e.key === 'Escape' && onCerrar(); window.addEventListener('keydown', k); return () => window.removeEventListener('keydown', k); }, [onCerrar]);
  const cols = useMemo(() => columnasDe(filas || []).map((c) => (c.campo === 'TipoCambio' ? { ...c, render: (x) => <span className={`chip-mora ${x.TipoCambio === 'AGREGADO' ? 'al-dia' : x.TipoCambio === 'ELIMINADO' ? 'mora-grave' : 'mora-baja'}`}>{String(x.TipoCambio).replace('_', ' ')}</span> } : c)), [filas]);
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-lg dialogo-asig" role="dialog" aria-modal="true" aria-label="Detalle del evento" onMouseDown={(e) => e.stopPropagation()}>
        <div className="modal-cab"><div><h3>Venta #{ev.VentaId} · {ev.TipoEvento}</h3><small className="sub-rango">{ev.Usuario} · {ev.FechaHora} · {tit(ev.Cliente || '')}</small></div><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        {!filas ? <div className="cargando-bloque"><span className="spinner" /> Cargando...</div>
          : <TablaReporte titulo={`AUDITORIA VENTA #${ev.VentaId}`} subtitulo={`${ev.TipoEvento} · ${ev.FechaHora}`} columnas={cols} filas={filas} claveFila="__no" vacio="Sin cambios de productos" />}
        <div className="modal-acciones"><button className="btn btn-sec" onClick={onCerrar}>Cerrar</button></div>
      </div>
    </div>,
    document.body,
  );
}
