// =====================================================================
//  Reportes de "Ingreso y Salida De Productos"
//   kardex (Historial de productos) · saldos · saldos-valorados · general (Reporte general historial) · monitoreo
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { columnasDe } from '../ventas/Reportes';
import { useAlActivar, useRefrescar } from '../../global/vistaActiva';

const URL = '/api/inventario/reportes';
const dos = (n) => String(n).padStart(2, '0');
const iso = (d) => `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`;
const hoy = () => iso(new Date());
const primeroMes = () => { const d = new Date(); return iso(new Date(d.getFullYear(), d.getMonth(), 1)); };
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });

export const CONFIG_INV = {
  kardex: { titulo: 'Historial de productos', sub: 'Kardex De Productos', icono: 'history', filtros: ['fechas', 'deposito', 'producto'] },
  saldos: { titulo: 'Reporte de saldos de productos', sub: 'Saldos De Productos', icono: 'bars', filtros: ['depositoTodos', 'mayor0'] },
  'saldos-valorados': { titulo: 'Saldos valorados', sub: 'Saldos Valorados De Productos', icono: 'dollar', filtros: [] },
  general: { titulo: 'Reporte general del historial', sub: 'Kardex General De Productos', icono: 'chart', filtros: ['fechas', 'deposito'] },
};
const TABS_MON = [
  { k: 'sinStock', t: 'Productos sin stock', i: 'alert', c: 'mora-grave' },
  { k: 'stockMinimo', t: 'Stock mínimo', i: 'bars', c: 'mora-baja' },
  { k: 'vencidos', t: 'Productos vencidos', i: 'history', c: 'mora-alta' },
];

export default function ReporteInventario({ clave }) {
  if (clave === 'monitoreo') return <Monitoreo />;
  return <Reporte clave={clave} />;
}

function Reporte({ clave }) {
  const cfg = CONFIG_INV[clave];
  const toast = useToast();
  const [f, setF] = useState({ desde: primeroMes(), hasta: hoy(), deposito: '', producto: '', mayor0: true });
  const [deps, setDeps] = useState([]);
  const [prods, setProds] = useState([]);
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(false);
  const tiene = (x) => cfg.filtros.includes(x);

  useEffect(() => {
    setDatos(null);
    if (tiene('deposito') || tiene('depositoTodos')) {
      api.get(`${URL}/aux/depositos`).then((r) => {
        setDeps(tiene('depositoTodos') ? [{ Id: -1, NombreDeposito: 'TODOS LOS ALMACENES' }, ...r.data] : r.data);
        setF((o) => ({ ...o, deposito: tiene('depositoTodos') ? -1 : r.data[0]?.Id ?? '' }));
      }).catch((e) => toast(e.message, 'error'));
    }
  }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps
  useEffect(() => {
    if (!tiene('producto') || !f.deposito) return;
    api.get(`${URL}/kardex/productos/${f.deposito}`).then((r) => { setProds(r.data); setF((o) => ({ ...o, producto: '' })); }).catch((e) => toast(e.message, 'error'));
  }, [f.deposito, clave]); // eslint-disable-line react-hooks/exhaustive-deps
  // al volver a la pestaña: almacenes y productos actualizados, sin perder lo elegido
  useAlActivar(() => {
    if (tiene('deposito') || tiene('depositoTodos')) api.get(`${URL}/aux/depositos`).then((r) => setDeps(tiene('depositoTodos') ? [{ Id: -1, NombreDeposito: 'TODOS LOS ALMACENES' }, ...r.data] : r.data)).catch((e) => toast(e.message, 'error'));
    if (tiene('producto') && f.deposito) api.get(`${URL}/kardex/productos/${f.deposito}`).then((r) => setProds(r.data)).catch((e) => toast(e.message, 'error'));
  }, { alMontar: false });

  const generar = async () => {
    if (tiene('producto') && !f.producto) { toast('Seleccione un Producto'.toUpperCase(), 'error'); return; }
    if (tiene('deposito') && !f.deposito) { toast('Debe Seleccionar un Deposito'.toUpperCase(), 'error'); return; }
    const q = new URLSearchParams();
    if (tiene('fechas')) { q.set('desde', f.desde); q.set('hasta', f.hasta); }
    if (tiene('deposito') || tiene('depositoTodos')) q.set('deposito', f.deposito);
    if (tiene('producto')) q.set('producto', f.producto);
    if (tiene('mayor0') && f.mayor0) q.set('mayor0', '1');
    setCargando(true);
    try {
      const r = await api.get(`${URL}/${clave}?${q}`);
      setDatos(r.data);
      if (!(Array.isArray(r.data) ? r.data : r.data.filas).length) toast(clave === 'kardex' ? 'No Existe Movimiento Para Los Filtros Establecidos'.toUpperCase() : 'No Existen Datos Para Mostrar con Los Filtros Seleccionados'.toUpperCase(), 'error');
    } catch (e) { toast(e.message, 'error', 7000); } finally { setCargando(false); }
  };

  const filas = Array.isArray(datos) ? datos : datos?.filas || [];
  const columnas = useMemo(() => {
    if (clave === 'kardex') {
      return [{ campo: 'fecha', titulo: 'Fecha', tipo: 'fecha' }, { campo: 'concepto', titulo: 'Concepto', render: (x) => <b>{tit(x.concepto ?? '')}</b> }, { campo: 'observacion', titulo: 'Observación', ancho: 200 },
        { campo: 'ingreso', titulo: 'Ingreso', tipo: 'num', total: true, clase: (x) => (x.ingreso ? 'pos' : '') }, { campo: 'salida', titulo: 'Salida', tipo: 'num', total: true, clase: (x) => (x.salida ? 'neg' : '') },
        { campo: 'saldo', titulo: 'Saldo', tipo: 'num', render: (x) => <b>{n2(x.saldo)}</b>, clase: (x) => (x.saldo < 0 ? 'neg' : '') }];
    }
    return columnasDe(filas).map((c) => (/saldo|entradas|salidas|cantidad|stock/i.test(c.campo) && c.tipo === 'bs' ? { ...c, tipo: 'num' } : c));
  }, [filas, clave]);
  const prodNombre = prods.find((p) => String(p.Id) === String(f.producto))?.Nombre;
  const depNombre = deps.find((d) => String(d.Id) === String(f.deposito))?.NombreDeposito;
  const sub = [tiene('fechas') && `Del ${f.desde.split('-').reverse().join('/')} al ${f.hasta.split('-').reverse().join('/')}`, depNombre, prodNombre].filter(Boolean).join(' · ');

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo"><span className="modelo-icono"><Icono n={cfg.icono} size={22} /></span><div><h1>{cfg.titulo}</h1><p>{cfg.sub}</p></div></div>
      </header>
      <div className="modelo-cuerpo">
        <div className="filtros-lista filtros-reporte">
          {tiene('fechas') && (
            <>
              <label className="filtro"><span>Desde</span><input type="date" className="control" value={f.desde} max={f.hasta} onChange={(e) => e.target.value && setF((o) => ({ ...o, desde: e.target.value }))} /></label>
              <label className="filtro"><span>Hasta</span><input type="date" className="control" value={f.hasta} min={f.desde} onChange={(e) => e.target.value && setF((o) => ({ ...o, hasta: e.target.value }))} /></label>
            </>
          )}
          {(tiene('deposito') || tiene('depositoTodos')) && (
            <div className="filtro filtro-ancho"><span>Depósito</span><ComboMulti dt={deps} valor={f.deposito} onChange={(x) => setF((o) => ({ ...o, deposito: x }))} id="Id" nameId="Cód." descripcionId="NombreDeposito" descripcion="Depósito" /></div>
          )}
          {tiene('producto') && (
            <div className="filtro filtro-ancho"><span>Producto</span><ComboMulti dt={prods} valor={f.producto} onChange={(x) => setF((o) => ({ ...o, producto: x }))} id="Id" nameId="Cód." descripcionId="Nombre" descripcion="Producto" /></div>
          )}
          {tiene('mayor0') && (
            <div className="filtro"><span>Productos</span><div className="chips-estado">
              <button type="button" className={`chip-filtro ${f.mayor0 ? 'activo' : ''}`} onClick={() => setF((o) => ({ ...o, mayor0: true }))}>Con stock</button>
              <button type="button" className={`chip-filtro ${!f.mayor0 ? 'activo' : ''}`} onClick={() => setF((o) => ({ ...o, mayor0: false }))}>Todos</button>
            </div></div>
          )}
          <button className="btn btn-pri filtro-boton" onClick={generar} disabled={cargando}>{cargando ? <span className="spinner" /> : <Icono n="search" size={16} />} Generar</button>
        </div>

        {clave === 'kardex' && datos?.resumen && (
          <div className="kardex-resumen">
            <div><span>Saldo inicial</span><b>{n2(datos.resumen.inicial)}</b></div>
            <div className="ing"><span>Ingresos</span><b>+ {n2(datos.resumen.ingresos)}</b></div>
            <div className="sal"><span>Salidas</span><b>− {n2(datos.resumen.salidas)}</b></div>
            <div className="fin"><span>Saldo final</span><b>{n2(datos.resumen.final)}</b></div>
          </div>
        )}
        {datos && <TablaReporte titulo={cfg.sub.toUpperCase()} subtitulo={sub} columnas={columnas} filas={filas} cargando={cargando} claveFila="__no" vacio="No Existen Datos Para Mostrar" />}
        {!datos && !cargando && <div className="reporte-vacio"><Icono n="search" size={28} /><span>Elija los filtros y presione <b>Generar</b></span></div>}
      </div>
    </section>
  );
}

// F_MonitoreoProductos: sin stock / stock minimo / vencidos
function Monitoreo() {
  const toast = useToast();
  const [datos, setDatos] = useState(null);
  const [tab, setTab] = useState('sinStock');
  const cargar = () => api.get(`${URL}/monitoreo`).then((r) => setDatos(r.data)).catch((e) => toast(e.message, 'error'));
  useAlActivar(cargar); // stock actual al abrir, al volver a la pestaña y con Actualizar
  const refrescar = useRefrescar(cargar);
  const filas = datos?.[tab] || [];
  const cols = useMemo(() => columnasDe(filas).map((c) => (c.tipo === 'bs' ? { ...c, tipo: 'num' } : c)), [filas]);
  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo"><span className="modelo-icono"><Icono n="target" size={22} /></span><div><h1>Monitoreo de productos</h1><p>Stock en cero, stock mínimo y vencidos</p></div></div>
        <div className="barra"><button className="btn-icono" title="Actualizar" aria-label="Actualizar" onClick={refrescar}><Icono n="refresh" size={18} /></button></div>
      </header>
      <div className="modelo-cuerpo">
        <div className="mon-tarjetas">
          {TABS_MON.map((t) => (
            <button key={t.k} type="button" className={`mon-tarjeta ${t.k} ${tab === t.k ? 'activa' : ''}`} onClick={() => setTab(t.k)} aria-pressed={tab === t.k}>
              <Icono n={t.i} size={22} /><span>{t.t}</span><b>{datos ? datos[t.k].length : '…'}</b>
            </button>
          ))}
        </div>
        {!datos ? <div className="cargando-bloque"><span className="spinner" /> Cargando...</div>
          : <TablaReporte titulo={TABS_MON.find((t) => t.k === tab).t.toUpperCase()} columnas={cols} filas={filas} claveFila="__no" vacio="Sin productos en esta lista" />}
      </div>
    </section>
  );
}
