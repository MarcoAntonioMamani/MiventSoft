// =====================================================================
//  Reportes del panel Distribución (cada uno con su programa y su URL)
//   gastos-chofer · ingresos-egresos-anual · ingresos-egresos-detallado · historico-clientes
//   productos-por-cliente · auditoria-ventas · historial-incremento
//  Columnas tal como las devuelve cada SP (TablaReporte: buscar, totales, Excel, imprimir).
// =====================================================================
import { useEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import TablaReporte, { fechaTxt } from '../../componentes/TablaReporte';
import { Mapa, Encuadrar, Pin } from '../../componentes/Mapa';
import { ComboLibreria, ComboMulti } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { columnasDe } from '../ventas/Reportes';
import { imprimirReporte } from '../../componentes/ReporteImpreso';
import { esc } from '../../global/imprimir';
import { useSesion } from '../../global/Sesion';
import { palabrasDe, coincide } from '../../global/buscar';
import { descargarTexto } from '../../global/appMovil';

const URL = '/api/distribucion/reportes';
const dos = (n) => String(n).padStart(2, '0');
const isoLocal = (d) => `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`;
const hoy = () => isoLocal(new Date());
const haceDias = (n) => { const d = new Date(); d.setDate(d.getDate() - n); return isoLocal(d); };
const compacto = (n) => Number(n || 0).toLocaleString('es-BO', { notation: 'compact', maximumFractionDigits: 1 });
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
// Rep_AuditoriaVentas (grMaestro): color de toda la fila segun la accion (rojo=Eliminado, naranja=Anulado, azul=Modificado)
const CLASE_EVENTO = { ELIMINADO: 'aud-eliminado', ANULADO: 'aud-anulado', MODIFICADO: 'aud-modificado' };
const CLASE_CAMBIO = { AGREGADO: 'aud-agregado', ELIMINADO: 'aud-eliminado', CANTIDAD_MODIFICADA: 'aud-anulado' };
const COLS_AUDITORIA = [
  { campo: 'VentaId', titulo: 'Venta', render: (x) => <b>{x.VentaId}</b> },
  { campo: 'TipoEvento', titulo: 'Acción', render: (x) => <b>{x.TipoEvento}</b> },
  { campo: 'Origen', titulo: 'Origen', clase: (x) => (String(x.Origen).toUpperCase() === 'MOVIL' ? 'aud-movil' : '') },
  { campo: 'FechaHora', titulo: 'Fecha y hora', tipo: 'fecha' },
  { campo: 'Usuario', titulo: 'Usuario' },
  { campo: 'Cliente', titulo: 'Cliente' },
  { campo: 'Personal', titulo: 'Vendedor' },
  { campo: 'TotalVenta', titulo: 'Total venta', tipo: 'bs' },
];
// detalle simplificado: antes -> despues en una sola celda (si no cambio, un solo valor)
const vacioN = (v) => v === null || v === undefined || v === '';
function AntesDespues({ a, d }) {
  if (vacioN(a) && vacioN(d)) return null;
  if (vacioN(a)) return <b>{bs(d)}</b>;
  if (vacioN(d)) return <s>{bs(a)}</s>;
  if (Number(a) === Number(d)) return <>{bs(d)}</>;
  return <span className="aud-flecha">{bs(a)} <i>→</i> <b>{bs(d)}</b></span>;
}
const COLS_AUDITORIA_DET = [
  { campo: 'Producto', titulo: 'Producto', render: (x) => <span className="aud-prod">{x.Producto}<small>Cód. {x.ProductoId}</small></span> },
  { campo: 'TipoCambio', titulo: 'Cambio', render: (x) => <b>{String(x.TipoCambio ?? '').replace('_', ' ')}</b> },
  { campo: 'CantidadDespues', titulo: 'Cantidad', tipo: 'num', render: (x) => <AntesDespues a={x.CantidadAntes} d={x.CantidadDespues} /> },
  { campo: 'PrecioDespues', titulo: 'Precio', tipo: 'num', render: (x) => <AntesDespues a={x.PrecioAntes} d={x.PrecioDespues} /> },
];

// ---------- Hoja impresa del reporte anual: carta horizontal (14 columnas + grafico entran en el ancho) ----------
const CSS_ANUAL = `
.rp .ia-kpis { display: flex; gap: 8px; margin: 4px 0 8px; }
.rp .ia-kpis div { flex: 1; border: 1px solid #d1d5db; border-radius: 4px; padding: 4px 8px; }
.rp .ia-kpis span { display: block; font-size: 8.5px; color: #4b5563; text-transform: uppercase; }
.rp .ia-kpis b { font-size: 12.5px; font-variant-numeric: tabular-nums; }
.rp table.ia-t { width: 100%; border-collapse: collapse; table-layout: fixed; font-size: 8.4px; font-variant-numeric: tabular-nums; }
.rp table.ia-t th { background: #1c2f8f; color: #fff; padding: 3px 2px; font-size: 8.4px; text-align: right; }
.rp table.ia-t th:first-child, .rp table.ia-t td:first-child { text-align: left; width: 7.5%; padding-left: 4px; }
.rp table.ia-t th:last-child { width: 8.5%; }
.rp table.ia-t td { padding: 3px 2px; text-align: right; border-bottom: 1px solid #e5e7eb; white-space: nowrap; overflow: hidden; }
.rp table.ia-t td:last-child { font-weight: 700; }
.rp table.ia-t tr.saldo td { font-weight: 700; background: #e3e8fb; border-top: 1.5px solid #111; border-bottom: 1.5px solid #111; }
.rp .ia-neg { color: #b91c1c; } .rp .ia-pos { color: #047857; }
.rp .ia-graf { margin-top: 10px; break-inside: avoid; }
.rp .ia-graf h2 { margin: 0 0 4px; font-size: 11px; display: flex; gap: 14px; align-items: center; }
.rp .ia-graf h2 small { font-size: 8.5px; font-weight: 400; display: inline-flex; align-items: center; gap: 4px; }
.rp .ia-graf h2 small i { width: 9px; height: 9px; display: inline-block; border-radius: 2px; }
.rp .ia-barras { display: flex; align-items: stretch; gap: 4px; height: 165px; border-bottom: 1px solid #9ca3af; }
.rp .ia-mes { flex: 1 1 0; min-width: 0; display: flex; gap: 2px; justify-content: center; align-items: flex-end; }
.rp .ia-par { width: 42%; height: 100%; display: flex; flex-direction: column; justify-content: flex-end; align-items: center; }
.rp .ia-par em { font-style: normal; font-size: 7px; white-space: nowrap; margin-bottom: 1px; }
.rp .ia-par i { display: block; width: 100%; border-radius: 2px 2px 0 0; }
.rp .ia-meses { display: flex; gap: 4px; }
.rp .ia-meses span { flex: 1 1 0; text-align: center; font-size: 8px; color: #374151; padding-top: 2px; }
`;
const C_ING = '#047857';
const C_EGR = '#dc2626';
function htmlAnual(filas, grafico) {
  const meses = grafico.map((d) => d.mes);
  const g = (Concepto) => filas.find((x) => x.Concepto === Concepto) || {};
  const ti = Number(g('INGRESOS').Total || 0); const te = Number(g('EGRESOS').Total || 0);
  const kpis = `<div class="ia-kpis"><div><span>Total ingresos</span><b class="ia-pos">Bs ${esc(bs(ti))}</b></div><div><span>Total egresos</span><b class="ia-neg">Bs ${esc(bs(te))}</b></div>`
    + `<div><span>Saldo (ingresos − egresos)</span><b class="${ti - te < 0 ? 'ia-neg' : 'ia-pos'}">Bs ${esc(bs(ti - te))}</b></div></div>`;
  const celda = (f, k) => { const n = Number(f[k] || 0); return `<td${f.Concepto === 'SALDO' ? ` class="${n < 0 ? 'ia-neg' : 'ia-pos'}"` : ''}>${esc(bs(n))}</td>`; };
  const tabla = `<table class="ia-t"><thead><tr><th>Concepto</th>${meses.map((m) => `<th>${esc(m.slice(0, 3))}</th>`).join('')}<th>Total</th></tr></thead><tbody>`
    + filas.map((f) => `<tr${f.Concepto === 'SALDO' ? ' class="saldo"' : ''}><td><b>${esc(f.Concepto)}</b></td>${meses.map((m) => celda(f, m)).join('')}${celda(f, 'Total')}</tr>`).join('')
    + '</tbody></table>';
  const max = Math.max(1, ...grafico.map((d) => Math.max(d.ingreso, d.egreso)));
  const barra = (v, c) => `<div class="ia-par"><em style="color:${c}">${v ? esc(compacto(v)) : ''}</em><i style="height:${((v / max) * 85).toFixed(2)}%;background:${c}"></i></div>`;
  const graf = `<section class="ia-graf"><h2>Ingresos vs egresos por mes <small><i style="background:${C_ING}"></i>Ingresos</small><small><i style="background:${C_EGR}"></i>Egresos</small></h2>`
    + `<div class="ia-barras">${grafico.map((d) => `<div class="ia-mes">${barra(d.ingreso, C_ING)}${barra(d.egreso, C_EGR)}</div>`).join('')}</div>`
    + `<div class="ia-meses">${meses.map((m) => `<span>${esc(m.slice(0, 3))}</span>`).join('')}</div></section>`;
  return kpis + tabla + graf;
}

export default function ReporteDistribucion({ clave }) {
  const cfg = CONFIG_DIST[clave];
  const toast = useToast();
  const { sesion } = useSesion();
  const [f, setF] = useState({ desde: haceDias(cfg.desde || 0), hasta: hoy(), gasto: -1, personal: -1, anio: '', evento: 'TODOS', proveedor: -1, categoria: -1, cliente: -1, producto: -1 });
  const [aux, setAux] = useState({});
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(false);
  const [evento, setEvento] = useState(null);
  const [selHist, setSelHist] = useState(null); // historico de clientes: registro cuyas ubicaciones se ven en los mapas
  const set = (k) => (x) => setF((o) => ({ ...o, [k]: x }));
  const tiene = (x) => cfg.filtros.includes(x);

  useEffect(() => {
    setDatos(null);
    const carga = [];
    if (tiene('personal')) carga.push(api.get(`${URL}/aux/personal`).then((r) => ({ personal: TODOS(r.data) })));
    if (tiene('gasto')) carga.push(api.get(`${URL}/gastos-chofer/combos`).then((r) => ({ gastos: TODOS(r.data.tiposGasto.map((x) => ({ Id: x.cnnum, Nombre: x.cndesc1 }))) })));
    if (tiene('anio')) carga.push(api.get(`${URL}/ingresos-egresos-anual/anios`).then((r) => ({ anios: r.data })));
    if (tiene('proveedor')) carga.push(api.get(`${URL}/productos-por-cliente/combos`).then((r) => ({ prov: TODOS(r.data.proveedores), cat: TODOS(r.data.categorias), cli: TODOS(r.data.clientes, 'NombreCliente'), prod: TODOS(r.data.productos) })));
    Promise.all(carga).then((xs) => {
      const a = Object.assign({}, ...xs);
      setAux(a);
      if (a.anios?.length) setF((o) => ({ ...o, anio: a.anios[a.anios.length - 1].cnnum })); // cbAnio: ultimo año
    }).catch((e) => toast(e.message, 'error'));
  }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps

  // Gestion (libreria 13): el combo guarda el cnnum, el SP recibe el año (cndesc1)
  const anioTxt = String(aux.anios?.find((x) => String(x.cnnum) === String(f.anio))?.cndesc1 ?? '').trim();
  const nuevaGestion = async (descripcion) => {
    const r = await api.post(`${URL}/ingresos-egresos-anual/anios`, { descripcion });
    setAux((o) => ({ ...o, anios: r.data }));
    toast(r.mensaje, 'ok');
    return r;
  };
  const generar = async () => {
    const q = new URLSearchParams();
    if (tiene('fechas')) { q.set('desde', f.desde); q.set('hasta', f.hasta); }
    if (tiene('anio')) q.set('anio', anioTxt);
    ['gasto', 'personal', 'evento', 'proveedor', 'categoria', 'cliente', 'producto'].forEach((k) => tiene(k) && q.set(k, f[k]));
    setCargando(true);
    try {
      const r = await api.get(`${URL}/${clave}?${q}`);
      setDatos(r.data);
      // historico: se elige de entrada el primer registro con ubicacion (o el primero)
      if (clave === 'historico-clientes' && Array.isArray(r.data)) setSelHist((r.data.find((x) => x.ubicacion || x.ubicacionAntes) || r.data[0])?.Id ?? null);
      const n = Array.isArray(r.data) ? r.data.length : r.data.filas.length;
      if (!n) toast('No Existen Datos Para Mostrar con Los Filtros Seleccionados'.toUpperCase(), 'error');
    } catch (e) { toast(e.message, 'error', 7000); } finally { setCargando(false); }
  };

  // cbTipoEvento_ValueChanged: con datos ya cargados, cambiar el tipo recarga solo
  useEffect(() => { if (clave === 'auditoria-ventas' && datos) generar(); }, [f.evento]); // eslint-disable-line react-hooks/exhaustive-deps

  const filas = Array.isArray(datos) ? datos : datos?.filas || [];
  const columnas = useMemo(() => {
    if (clave === 'historico-clientes') {
      return [{ campo: 'Fecha', titulo: 'Fecha', tipo: 'fecha' }, { campo: 'Accion', titulo: 'Acción' }, { campo: 'NombrePersonal', titulo: 'Personal', render: (x) => tit(x.NombrePersonal) },
        { campo: 'diferencia', titulo: 'Campos modificados o creados', render: (x) => <Cambios cambios={x.cambios} ubicacion={x.ubicacion} /> }];
    }
    if (clave === 'ingresos-egresos-anual') {
      return columnasDe(filas).map((c) => (c.campo === 'Concepto' ? { ...c, render: (x) => <b>{x.Concepto}</b> }
        : { ...c, tipo: 'bs', total: false, render: (x) => (x.Concepto === 'SALDO' ? <b className={Number(x[c.campo]) < 0 ? 'txt-neg' : 'txt-pos'}>{bs(x[c.campo])}</b> : bs(x[c.campo])) }));
    }
    if (clave === 'auditoria-ventas') return COLS_AUDITORIA;
    return columnasDe(filas);
  }, [filas, clave]);
  const rango = tiene('fechas') ? `Del ${f.desde.split('-').reverse().join('/')} al ${f.hasta.split('-').reverse().join('/')}` : `Gestión ${anioTxt}`;
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
          {tiene('anio') && <div className="filtro filtro-ancho"><span>Gestión</span><ComboLibreria dt={aux.anios || []} valor={f.anio} onChange={set('anio')} titulo="Gestión (Año)" onNuevo={nuevaGestion} /></div>}
          {tiene('gasto') && combo('gasto', 'Tipo de gasto', aux.gastos)}
          {tiene('personal') && combo('personal', 'Personal', aux.personal)}
          {tiene('evento') && combo('evento', 'Acción', EVENTOS)}
          {tiene('proveedor') && combo('proveedor', 'Proveedor', aux.prov)}
          {tiene('categoria') && combo('categoria', 'Categoría', aux.cat)}
          {tiene('cliente') && combo('cliente', 'Cliente', aux.cli, 'NombreCliente')}
          {tiene('producto') && combo('producto', 'Producto', aux.prod)}
          <button className="btn btn-pri filtro-boton" onClick={generar} disabled={cargando || (tiene('anio') && !f.anio)}>{cargando ? <span className="spinner" /> : <Icono n="search" size={16} />} Generar</button>
        </div>

        {datos && clave === 'ingresos-egresos-anual' && datos.grafico && <GraficoAnual datos={datos.grafico} anio={anioTxt} />}
        {datos && clave === 'ingresos-egresos-detallado' && (
          <DetalleIngresosEgresos filas={filas} rango={rango} cabecera={datos.cabecera} usuario={tit(sesion?.gs_user || '')} />
        )}
        {datos && clave === 'productos-por-cliente' && (
          <VentasPorCliente filas={filas} rango={rango} cabecera={datos.cabecera} usuario={tit(sesion?.gs_user || '')} />
        )}
        {datos && clave !== 'ingresos-egresos-detallado' && clave !== 'productos-por-cliente' && (() => {
          const hist = clave === 'historico-clientes';
          const aud = clave === 'auditoria-ventas';
          const tabla = (
          <TablaReporte titulo={aud ? `EVENTOS (${filas.length})` : cfg.sub.toUpperCase()} subtitulo={rango} columnas={columnas} filas={filas} cargando={cargando} claveFila={aud || hist ? 'Id' : '__no'}
            vacio="No Existen Datos Para Mostrar"
            onFila={aud ? setEvento : hist ? (x) => setSelHist(x.Id) : undefined}
            filaSel={hist ? selHist : aud ? evento?.Id : undefined}
            claseFila={aud ? (x) => CLASE_EVENTO[x.TipoEvento] : undefined}
            onImprimir={clave === 'ingresos-egresos-anual' && datos.grafico ? () => imprimirReporte({
              titulo: 'Reporte de ingresos y egresos anual', subtitulo: `Gestión ${anioTxt}`, usuario: tit(sesion?.gs_user || ''),
              empresa: datos.cabecera?.empresa, imagenEmpresa: datos.cabecera?.imagenEmpresa, datos: [['Gestión:', anioTxt]],
              horizontal: true, sinTabla: true, filas, unidad: 'concepto', antes: htmlAnual(filas, datos.grafico), css: CSS_ANUAL,
            }) : undefined} />
          );
          // Rep_AuditoriaVentas: tabla de eventos a todo el ancho; clic en una fila abre el detalle en un popup
          if (aud) return <>{tabla}{evento && <DetalleEvento ev={evento} onCerrar={() => setEvento(null)} />}</>;
          // Rep_Modificaciones: tabla a la izquierda y los mapas Antes / Despues del registro elegido a la derecha
          if (!hist) return tabla;
          return <div className="hist-layout"><div className="hist-tabla">{tabla}</div><MapasHistorico fila={filas.find((x) => x.Id === selHist)} /></div>;
        })()}
        {!datos && !cargando && <div className="reporte-vacio"><Icono n="search" size={28} /><span>Elija los filtros y presione <b>Generar</b></span></div>}
      </div>
    </section>
  );
}

// ---------- Ventas de productos por cliente (MAM_Ventas 31) ----------
// Tres niveles: Cliente > Categoria > Producto, cada uno con sus cantidades y total vendido.
// Pantalla: clientes plegables (orden por venta o nombre, buscador, de a 50); impresion compacta
// (cliente y categoria en una sola linea con su subtotal: sin filas de "Subtotal" aparte).
const PPC_PASO = 50;
const F_CANT = new Intl.NumberFormat('es-BO', { maximumFractionDigits: 2 }); // reutilizados: miles de lineas al imprimir
const F_BS = new Intl.NumberFormat('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const cant = (n) => F_CANT.format(Number(n || 0));
const cantTxt = (cj, u) => [cj ? `${cant(cj)} cj` : '', u ? `${cant(u)} u` : ''].filter(Boolean).join(' · ') || '—';
const txt = (x, def) => String(x ?? '').trim() || def;
const plur = (n, s, p) => `${n.toLocaleString('es-BO')} ${n === 1 ? s : p}`;

function agruparPPC(filas, orden) {
  const m = new Map();
  const sumar = (o, f) => { o.total += Number(f.TotalVenta || 0); o.cajas += Number(f.CantidadCaja || 0); o.uni += Number(f.CantidadUni || 0); };
  filas.forEach((f) => {
    let c = m.get(f.ClienteId);
    if (!c) { c = { id: f.ClienteId, nombre: txt(f.NombreCliente, '(Sin nombre)'), cats: new Map(), total: 0, cajas: 0, uni: 0, n: 0 }; m.set(f.ClienteId, c); }
    const kc = txt(f.Categoria, '(Sin categoría)');
    let g = c.cats.get(kc);
    if (!g) { g = { nombre: kc, prods: [], total: 0, cajas: 0, uni: 0 }; c.cats.set(kc, g); }
    g.prods.push(f); sumar(g, f); sumar(c, f); c.n += 1;
  });
  const porNombre = (a, b) => a.localeCompare(b, 'es', { numeric: true });
  const cmp = orden === 'nombre' ? (a, b) => porNombre(a.nombre, b.nombre) : (a, b) => b.total - a.total || porNombre(a.nombre, b.nombre);
  const clientes = [...m.values()].map((c) => ({
    ...c,
    cats: [...c.cats.values()].sort(cmp).map((g) => ({
      ...g, prods: [...g.prods].sort(orden === 'nombre' ? (a, b) => porNombre(txt(a.NombreProducto, ''), txt(b.NombreProducto, '')) : (a, b) => Number(b.TotalVenta || 0) - Number(a.TotalVenta || 0)),
    })),
  })).sort(cmp);
  return clientes;
}

const COLS_PPC = [
  { campo: 'CodigoExterno', titulo: 'Código', ancho: '12%' },
  { campo: 'NombreProducto', titulo: 'Producto' },
  { campo: 'Unidad', titulo: 'Und.', ancho: '8%' },
  { campo: 'CantidadCaja', titulo: 'Cajas', tipo: 'num', ancho: '9%' },
  { campo: 'CantidadUni', titulo: 'Unid.', tipo: 'num', ancho: '9%' },
  { campo: 'TotalVenta', titulo: 'Total Bs', tipo: 'bs', ancho: '13%' },
];
const CSS_PPC = `
.rp .rp-t > tbody > tr.pc-p > td { padding: 1px 5px; font-size: 9px; background: #fff; border-bottom: 1px dotted #e5e7eb; }
.rp tr.pc-p td:first-child { padding-left: 22px; color: #4b5563; }
.rp tr.pc-c td { padding: 7px 5px 3px; font-size: 11px; font-weight: 700; background: #fff !important; border-bottom: 1.5px solid #111; break-after: avoid; }
.rp tr.pc-c td small, .rp tr.pc-g td small { font-weight: 400; color: #6b7280; margin-left: 8px; font-size: 9px; }
.rp tr.pc-g td { padding: 3px 5px 2px 12px; font-size: 9.5px; font-weight: 700; background: #f3f4f6 !important; color: #1c2f8f; border-bottom: 1px solid #d1d5db; break-after: avoid; }
.rp tr.pc-g td.num, .rp tr.pc-c td.num { padding-left: 5px; }
`;
const numTd = (v, bsf) => `<td class="num">${v ? esc(bsf ? F_BS.format(v) : cant(v)) : ''}</td>`;
function htmlPPC(clientes) {
  return clientes.map((c) => `<tr class="pc-c"><td colspan="3">${esc(c.nombre)}<small>${esc(plur(c.cats.length, 'categoría', 'categorías'))} · ${esc(plur(c.n, 'producto', 'productos'))}</small></td>${numTd(c.cajas)}${numTd(c.uni)}${numTd(c.total, true)}</tr>`
    + c.cats.map((g) => `<tr class="pc-g"><td colspan="3">${esc(g.nombre)}<small>${esc(plur(g.prods.length, 'producto', 'productos'))}</small></td>${numTd(g.cajas)}${numTd(g.uni)}${numTd(g.total, true)}</tr>`
      + g.prods.map((p) => `<tr class="pc-p"><td>${esc(txt(p.CodigoExterno, ''))}</td><td>${esc(txt(p.NombreProducto, ''))}</td><td>${esc(txt(p.Unidad, ''))}</td>${numTd(p.CantidadCaja)}${numTd(p.CantidadUni)}${numTd(p.TotalVenta, true)}</tr>`).join('')).join('')).join('');
}

function VentasPorCliente({ filas, rango, cabecera, usuario }) {
  const [orden, setOrden] = useState('venta');
  const [buscar, setBuscar] = useState('');
  const [abiertos, setAbiertos] = useState(() => new Set());
  const [ver, setVer] = useState(PPC_PASO);
  const todos = useMemo(() => agruparPPC(filas, orden), [filas, orden]);
  // buscador: por cliente (muestra todo lo que compro) o por producto / codigo / categoria (solo esas lineas)
  const lista = useMemo(() => {
    const w = palabrasDe(buscar);
    if (!w.length) return todos;
    const porNombre = new Set(todos.filter((c) => coincide(c.nombre, w)).map((c) => c.id));
    // linea: palabras en cualquier orden entre cliente + producto + codigo + categoria (ej. "coca juan")
    const fs = filas.filter((f) => !porNombre.has(f.ClienteId) && coincide(`${f.NombreCliente ?? ''} ${f.NombreProducto ?? ''} ${f.CodigoExterno ?? ''} ${f.Categoria ?? ''}`, w));
    const parciales = new Map(agruparPPC(fs, orden).map((c) => [c.id, c]));
    return todos.map((c) => (porNombre.has(c.id) ? c : parciales.get(c.id))).filter(Boolean);
  }, [todos, buscar, filas, orden]);
  useEffect(() => { setVer(PPC_PASO); }, [buscar, orden, filas]);
  const kpi = useMemo(() => ({
    total: todos.reduce((a, c) => a + c.total, 0), cajas: todos.reduce((a, c) => a + c.cajas, 0), uni: todos.reduce((a, c) => a + c.uni, 0),
    productos: new Set(filas.map((f) => f.ProductoId)).size, max: Math.max(1, ...todos.map((c) => c.total)),
  }), [todos, filas]);
  const visibles = lista.slice(0, ver);
  const alternar = (k) => setAbiertos((o) => { const n = new Set(o); if (n.has(k)) n.delete(k); else n.add(k); return n; });
  const todosAbiertos = visibles.length > 0 && visibles.every((c) => abiertos.has(c.id));
  const plegar = () => setAbiertos(todosAbiertos ? new Set() : new Set(visibles.map((c) => c.id)));
  const lineas = lista.flatMap((c) => c.cats.flatMap((g) => g.prods));
  const totalLista = lista.reduce((a, c) => a + c.total, 0);

  const imprimir = () => imprimirReporte({
    titulo: 'Ventas de productos por cliente', subtitulo: rango, usuario, empresa: cabecera?.empresa, imagenEmpresa: cabecera?.imagenEmpresa,
    datos: [['Clientes:', lista.length.toLocaleString('es-BO')], ['Total vendido:', `Bs ${bs(totalLista)}`], ...(buscar.trim() ? [['Búsqueda:', buscar.trim()]] : [])],
    columnas: COLS_PPC, filas: lineas, unidad: 'línea', css: CSS_PPC,
    cuerpoHtml: `${htmlPPC(lista)}<tr class="rp-espacio"><td colspan="6"></td></tr><tr class="rp-total"><td colspan="3">TOTAL GENERAL (${esc(plur(lista.length, 'cliente', 'clientes'))})</td>`
      + `${numTd(lista.reduce((a, c) => a + c.cajas, 0))}${numTd(lista.reduce((a, c) => a + c.uni, 0))}${numTd(totalLista, true)}</tr>`,
  });
  const excel = () => {
    const q = (s) => `"${String(s ?? '').replace(/"/g, '""')}"`;
    const n = (v) => String(Number(v || 0)).replace('.', ',');
    const ls = ['Cliente;Categoría;Código;Producto;Unidad;Cajas;Unidades;Total Bs',
      ...lista.flatMap((c) => c.cats.flatMap((g) => g.prods.map((p) => [q(c.nombre), q(g.nombre), q(txt(p.CodigoExterno, '')), q(txt(p.NombreProducto, '')), q(txt(p.Unidad, '')), n(p.CantidadCaja), n(p.CantidadUni), n(p.TotalVenta)].join(';'))))];
    descargarTexto('Ventas_productos_por_cliente.csv', `﻿${ls.join('\r\n')}`);
  };

  if (!filas.length) return <div className="reporte-vacio"><Icono n="search" size={28} /><span>No Existen Datos Para Mostrar</span></div>;
  return (
    <div className="ppc">
      <div className="ppc-kpis">
        <div className="ied-kpi"><span>Clientes</span><b>{todos.length.toLocaleString('es-BO')}</b><small>{rango}</small></div>
        <div className="ied-kpi"><span>Productos distintos</span><b>{kpi.productos.toLocaleString('es-BO')}</b><small>{plur(filas.length, 'línea', 'líneas')}</small></div>
        <div className="ied-kpi"><span>Cantidad vendida</span><b className="ppc-kpi-cant">{cantTxt(kpi.cajas, kpi.uni)}</b><small>cajas · unidades</small></div>
        <div className="ied-kpi ing"><span>Total vendido</span><b>Bs {bs(kpi.total)}</b><small>prom. Bs {bs(kpi.total / Math.max(1, todos.length))} / cliente</small></div>
      </div>
      <div className="ppc-barra">
        <label className="ppc-buscar"><Icono n="search" size={16} /><input type="search" className="control" placeholder="Buscar cliente, producto, código o categoría" value={buscar} onChange={(e) => setBuscar(e.target.value)} /></label>
        <div className="ppc-orden" role="group" aria-label="Ordenar">
          <button type="button" className={orden === 'venta' ? 'activo' : ''} onClick={() => setOrden('venta')}>Mayor venta</button>
          <button type="button" className={orden === 'nombre' ? 'activo' : ''} onClick={() => setOrden('nombre')}>A–Z</button>
        </div>
        <button type="button" className="btn btn-sec btn-sm" onClick={plegar} disabled={!visibles.length}><Icono n="chevronDown" size={15} /> {todosAbiertos ? 'Plegar' : 'Desplegar'}</button>
        <button type="button" className="btn btn-sec btn-sm" onClick={excel} disabled={!lista.length}><Icono n="save" size={15} /> Excel</button>
        <button type="button" className="btn btn-sec btn-sm" onClick={imprimir} disabled={!lista.length}><Icono n="printer" size={15} /> Imprimir</button>
      </div>
      {buscar.trim() && <p className="ppc-info">{plur(lista.length, 'cliente', 'clientes')} · Bs {bs(totalLista)}</p>}
      {!lista.length && <p className="ied-vacio">Sin coincidencias para «{buscar}».</p>}
      <div className="ppc-lista">
        {visibles.map((c) => {
          const ab = abiertos.has(c.id);
          return (
            <div key={c.id} className={`ppc-cli ${ab ? 'abierto' : ''}`}>
              <button type="button" className="ppc-cli-cab" aria-expanded={ab} onClick={() => alternar(c.id)}>
                <Icono n="chevronDown" size={16} />
                <span className="ppc-nom"><b>{c.nombre}</b><small>{plur(c.cats.length, 'categoría', 'categorías')} · {plur(c.n, 'producto', 'productos')}</small></span>
                <span className="ppc-cant">{cantTxt(c.cajas, c.uni)}</span>
                <b className="ppc-tot">Bs {bs(c.total)}</b>
                <i className="ppc-parte" style={{ '--p': `${Math.max(1, (c.total / kpi.max) * 100)}%` }} aria-hidden="true" />
              </button>
              {ab && (
                <div className="ppc-cats">
                  {c.cats.map((g) => (
                    <div key={g.nombre} className="ppc-cat">
                      <div className="ppc-cat-cab"><Icono n="tag" size={14} /><b>{g.nombre}</b><small>{plur(g.prods.length, 'producto', 'productos')}</small><span className="ppc-cant">{cantTxt(g.cajas, g.uni)}</span><b className="ppc-tot">Bs {bs(g.total)}</b></div>
                      <ul className="ppc-prods">
                        {g.prods.map((p) => (
                          <li key={p.ProductoId}>
                            <code>{txt(p.CodigoExterno, '—')}</code>
                            <span className="ppc-pnom">{txt(p.NombreProducto, '')}{p.Unidad ? <small>{txt(p.Unidad, '')}</small> : null}</span>
                            <span className="ppc-cant">{cantTxt(p.CantidadCaja, p.CantidadUni)}</span>
                            <span className="ppc-tot">Bs {bs(p.TotalVenta)}</span>
                          </li>
                        ))}
                      </ul>
                    </div>
                  ))}
                </div>
              )}
            </div>
          );
        })}
      </div>
      {lista.length > ver && (
        <button type="button" className="btn btn-sec ppc-mas" onClick={() => setVer((x) => x + PPC_PASO)}>
          Mostrar {Math.min(PPC_PASO, lista.length - ver)} más <small>({visibles.length.toLocaleString('es-BO')} de {lista.length.toLocaleString('es-BO')})</small>
        </button>
      )}
    </div>
  );
}

// ---------- Ingresos / egresos detallado ----------
// Primero todos los ingresos y luego todos los egresos; dentro de cada uno, agrupado por tipo (Modulo)
// con su subtotal; el detalle (Nro. comprobante = Id del registro, fecha y monto) se abre bajo cada tipo.
// Al final: balance = ingresos - egresos.
const COLS_IED = [
  { campo: 'NroComprobante', titulo: 'Nro. comprobante' },
  { campo: 'Fecha', titulo: 'Fecha', tipo: 'fecha' },
  { campo: 'Monto', titulo: 'Monto (Bs)', tipo: 'bs', total: true },
];
const CSS_IED = `
.rp .ied-bal { display: flex; justify-content: space-between; align-items: center; margin-top: 10px; padding: 8px 10px; border: 2px solid #111; border-radius: 4px; background: #f1f3f5; break-inside: avoid; }
.rp .ied-bal div { display: flex; gap: 22px; font-size: 10.5px; }
.rp .ied-bal b { font-variant-numeric: tabular-nums; }
.rp .ied-bal .tot { font-size: 13px; }
.rp .ied-neg { color: #b91c1c; } .rp .ied-pos { color: #047857; }
`;
function agruparIE(filas) {
  const secciones = [{ clave: 'INGRESOS', ingreso: true }, { clave: 'EGRESOS', ingreso: false }].map((s) => {
    const fs = filas.filter((f) => !!f.Ingreso === s.ingreso);
    const m = new Map();
    fs.forEach((f) => { if (!m.has(f.Modulo)) m.set(f.Modulo, []); m.get(f.Modulo).push(f); });
    const grupos = [...m.entries()].map(([nombre, g]) => ({ nombre, filas: g, total: g.reduce((a, f) => a + Number(f.Monto || 0), 0) }));
    return { ...s, grupos, n: fs.length, total: grupos.reduce((a, g) => a + g.total, 0) };
  });
  return { secciones, ingresos: secciones[0].total, egresos: secciones[1].total };
}
function DetalleIngresosEgresos({ filas, rango, cabecera, usuario }) {
  const r = useMemo(() => agruparIE(filas), [filas]);
  const [abiertos, setAbiertos] = useState(() => new Set());
  const balance = r.ingresos - r.egresos;
  const alternar = (k) => setAbiertos((o) => { const n = new Set(o); if (n.has(k)) n.delete(k); else n.add(k); return n; });
  const plural = (n) => `${n.toLocaleString('es-BO')} ${n === 1 ? 'comprobante' : 'comprobantes'}`;
  const imprimir = () => imprimirReporte({
    titulo: 'Reporte de ingresos y egresos detallado', subtitulo: rango, usuario, empresa: cabecera?.empresa, imagenEmpresa: cabecera?.imagenEmpresa,
    datos: [['Ingresos:', `Bs ${bs(r.ingresos)}`], ['Egresos:', `Bs ${bs(r.egresos)}`], ['Balance:', `Bs ${bs(balance)}`]],
    columnas: COLS_IED, filas, unidad: 'comprobante', totalGeneral: false, css: CSS_IED,
    niveles: [
      { clave: (f) => f.Grupo, titulo: 'Movimiento', orden: (f) => (f.Ingreso ? 0 : 1), subtotal: 'Total' },
      { clave: (f) => f.Modulo, titulo: 'Tipo', subtotal: 'Subtotal' },
    ],
    despues: `<div class="ied-bal"><div><span>Total ingresos: <b class="ied-pos">Bs ${esc(bs(r.ingresos))}</b></span><span>Total egresos: <b class="ied-neg">Bs ${esc(bs(r.egresos))}</b></span></div>`
      + `<span class="tot">BALANCE (ingresos − egresos): <b class="${balance < 0 ? 'ied-neg' : 'ied-pos'}">Bs ${esc(bs(balance))}</b></span></div>`,
  });
  if (!filas.length) return <div className="reporte-vacio"><Icono n="search" size={28} /><span>No Existen Datos Para Mostrar</span></div>;
  return (
    <div className="ied">
      <div className="ied-resumen">
        <div className="ied-kpi ing"><span>Total ingresos</span><b>Bs {bs(r.ingresos)}</b><small>{plural(r.secciones[0].n)}</small></div>
        <div className="ied-kpi egr"><span>Total egresos</span><b>Bs {bs(r.egresos)}</b><small>{plural(r.secciones[1].n)}</small></div>
        <div className={`ied-kpi ${balance < 0 ? 'egr' : 'ing'}`}><span>Balance (ingresos − egresos)</span><b>Bs {bs(balance)}</b><small>{rango}</small></div>
        <button className="btn btn-sec ied-imprimir" onClick={imprimir}><Icono n="list" size={16} /> Imprimir</button>
      </div>
      {r.secciones.map((s) => (
        <section key={s.clave} className={`ied-sec ${s.ingreso ? 'ing' : 'egr'}`} aria-label={s.clave}>
          <header className="ied-sec-cab"><h2>{s.clave}</h2><small>{plural(s.n)}</small><b>Bs {bs(s.total)}</b></header>
          {!s.grupos.length && <p className="ied-vacio">Sin {s.ingreso ? 'ingresos' : 'egresos'} en el rango.</p>}
          {s.grupos.map((g) => {
            const k = `${s.clave}|${g.nombre}`;
            const ab = abiertos.has(k);
            return (
              <div key={k} className={`ied-grupo ${ab ? 'abierto' : ''}`}>
                <button type="button" className="ied-grupo-cab" aria-expanded={ab} onClick={() => alternar(k)}>
                  <Icono n="chevronDown" size={16} /><b>{g.nombre}</b><small>{plural(g.filas.length)}</small><span>Bs {bs(g.total)}</span>
                </button>
                {ab && <div className="ied-detalle"><TablaReporte titulo={`${s.clave} - ${g.nombre}`.toUpperCase()} subtitulo={rango} columnas={COLS_IED} filas={g.filas} claveFila="__no" vacio="Sin comprobantes" /></div>}
              </div>
            );
          })}
        </section>
      ))}
      <div className={`ied-balance ${balance < 0 ? 'egr' : 'ing'}`}>
        <span>Ingresos <b>Bs {bs(r.ingresos)}</b> − Egresos <b>Bs {bs(r.egresos)}</b></span>
        <strong>Balance: Bs {bs(balance)}</strong>
      </div>
    </div>
  );
}

// Rep_Modificaciones (Gmc_ClienteAnterior / Gmc_ClienteActual): ubicacion antes y despues del registro elegido.
// Nuevo: "Antes" vacio y "Después" con su ubicacion. Modificacion: donde estaba y donde esta hoy.
// Solo se pinta si la latitud y longitud son validas (el servidor envia null si no).
function MapaPunto({ titulo, punto, vacio, tipo }) {
  const pos = punto ? [punto.lat, punto.lon] : null;
  return (
    <div className="hist-mapa">
      <div className="hist-mapa-cab"><b>{titulo}</b>{pos && <small>{punto.lat.toFixed(6)}, {punto.lon.toFixed(6)}</small>}</div>
      <div className="hist-mapa-caja">
        <Mapa className="hist-mapa-leaflet" zoom={16}>
          {pos && <><Encuadrar puntos={[pos]} zoom={16} /><Pin posicion={pos} tipo={tipo} /></>}
        </Mapa>
        {!pos && <div className="hist-mapa-vacio"><Icono n="pin" size={20} /><span>{vacio}</span></div>}
      </div>
    </div>
  );
}
function MapasHistorico({ fila }) {
  const nuevo = !!fila?.nuevo || /NUEV|CREA|INSERT/i.test(String(fila?.Accion || ''));
  const mismo = fila?.ubicacion && fila?.ubicacionAntes && fila.ubicacion.lat === fila.ubicacionAntes.lat && fila.ubicacion.lon === fila.ubicacionAntes.lon;
  // pantalla angosta (mapas arriba de la tabla): al elegir otro registro se sube a los mapas si no se ven.
  // En pantalla ancha los mapas quedan fijos al costado (sticky) y no hace falta.
  const ref = useRef(null);
  const previo = useRef(fila?.Id);
  useEffect(() => {
    if (previo.current === fila?.Id) return;
    previo.current = fila?.Id;
    const el = ref.current;
    if (!el || window.innerWidth > 1100) return;
    const r = el.getBoundingClientRect();
    if (r.top < 64 || r.top > window.innerHeight * 0.5) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }, [fila?.Id]);
  return (
    <aside ref={ref} className="hist-mapas" aria-label="Ubicación del cliente">
      {!fila ? <div className="hist-mapa-aviso"><Icono n="pin" size={18} /> Elija un registro de la tabla para ver su ubicación</div> : (
        <>
          <div className="hist-mapa-aviso"><b>{fila.Accion}</b> · {fechaTxt(fila.Fecha)}{mismo ? ' · la ubicación no cambió' : ''}</div>
          <MapaPunto titulo="Antes" punto={nuevo ? null : fila.ubicacionAntes} tipo="normal"
            vacio={nuevo ? 'Cliente nuevo: no tiene ubicación anterior' : 'Sin latitud / longitud válida antes'} />
          <MapaPunto titulo={nuevo ? 'Ubicación registrada' : 'Después (hoy)'} punto={fila.ubicacion} tipo="marcado" vacio="Sin latitud / longitud válida" />
        </>
      )}
    </aside>
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

// Grafico de ingresos vs egresos por mes (barras agrupadas) con el monto de cada barra y los totales del año
function GraficoAnual({ datos, anio }) {
  const max = Math.max(1, ...datos.map((d) => Math.max(d.ingreso, d.egreso)));
  const ti = datos.reduce((s, d) => s + d.ingreso, 0);
  const te = datos.reduce((s, d) => s + d.egreso, 0);
  return (
    <div className="tarjeta graf-anual">
      <div className="graf-anual-cab"><h2 className="tarjeta-tit">Ingresos vs egresos {anio}</h2>
        <div className="leyenda"><span><i className="dot ing" /> Ingresos</span><span><i className="dot egr" /> Egresos</span></div></div>
      <div className="ga-resumen">
        <div><span>Total ingresos</span><b className="txt-pos">Bs {bs(ti)}</b></div>
        <div><span>Total egresos</span><b className="txt-neg">Bs {bs(te)}</b></div>
        <div><span>Saldo (ingresos − egresos)</span><b className={ti - te < 0 ? 'txt-neg' : 'txt-pos'}>Bs {bs(ti - te)}</b></div>
      </div>
      <div className="graf-anual-barras" role="img" aria-label={`Ingresos y egresos por mes de ${anio}`}>
        {datos.map((d) => (
          <div key={d.mes} className="ga-mes" title={`${d.mes}: ingresos Bs ${bs(d.ingreso)} · egresos Bs ${bs(d.egreso)}`}>
            <div className="ga-col">
              <div className="ga-par"><em className="ing">{d.ingreso ? compacto(d.ingreso) : ''}</em><span className="ga-barra ing" style={{ height: `${(d.ingreso / max) * 82}%` }} /></div>
              <div className="ga-par"><em className="egr">{d.egreso ? compacto(d.egreso) : ''}</em><span className="ga-barra egr" style={{ height: `${(d.egreso / max) * 82}%` }} /></div>
            </div>
            <small>{d.mes.slice(0, 3)}</small>
          </div>
        ))}
      </div>
    </div>
  );
}

// Rep_AuditoriaVentas: detalle del evento (productos agregados / eliminados / cantidad modificada)
// Popup (grDetalle): colores por tipo de cambio y totales de cantidad antes / despues (fila dorada del VB)
function DetalleEvento({ ev, onCerrar }) {
  const toast = useToast();
  const [filas, setFilas] = useState(null);
  useEffect(() => {
    let vivo = true; // si cierran o eligen otro evento antes de que responda, se descarta la respuesta vieja
    setFilas(null);
    api.get(`${URL}/auditoria-ventas/${ev.Id}/detalle`).then((r) => vivo && setFilas(r.data)).catch((e) => { if (vivo) { toast(e.message, 'error'); setFilas([]); } });
    return () => { vivo = false; };
  }, [ev.Id]); // eslint-disable-line react-hooks/exhaustive-deps
  useEffect(() => { const k = (e) => e.key === 'Escape' && onCerrar(); window.addEventListener('keydown', k); return () => window.removeEventListener('keydown', k); }, [onCerrar]);
  const sum = (k) => (filas || []).reduce((s, x) => s + Number(x[k] || 0), 0);
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-lg aud-modal" role="dialog" aria-modal="true" aria-label="Detalle del evento" onMouseDown={(e) => e.stopPropagation()}>
        <div className={`modal-cab aud-detalle-cab ${CLASE_EVENTO[ev.TipoEvento] || ''}`}>
          <div><h3>Venta {ev.VentaId} <b>({ev.TipoEvento})</b></h3><small>{tit(ev.Cliente || '')} · {ev.Usuario} · {fechaTxt(ev.FechaHora)} · Bs {bs(ev.TotalVenta)}</small></div>
          <button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button>
        </div>
        {!filas ? <div className="cargando-bloque"><span className="spinner" /> Cargando...</div>
          : <TablaReporte titulo={`AUDITORIA VENTA #${ev.VentaId} (${ev.TipoEvento})`} subtitulo={fechaTxt(ev.FechaHora)} columnas={COLS_AUDITORIA_DET} filas={filas} claveFila="__no"
              claseFila={(x) => CLASE_CAMBIO[x.TipoCambio]} vacio="Sin cambios de productos" />}
        {filas?.length > 0 && <div className="aud-totales"><span>Cantidad antes <b>{bs(sum('CantidadAntes'))}</b></span><span>Cantidad después <b>{bs(sum('CantidadDespues'))}</b></span></div>}
        <div className="modal-acciones"><button className="btn btn-sec" onClick={onCerrar}>Cerrar</button></div>
      </div>
    </div>,
    document.body,
  );
}
