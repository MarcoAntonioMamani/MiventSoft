// =====================================================================
//  Reportes de ingresos y egresos (mismos endpoints que el panel Distribucion de Distral)
//   ingresos-egresos-anual (btnIngresoEgresosMensuales) · ingresos-egresos-detallado (btnIngresoEgresoDetallado)
//  Kaily: sin Rep_Gastos, Rep_Modificaciones, FProductosPorClientes, Rep_AuditoriaVentas ni Rep_HistorialIncrementoVenta.
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboLibreria } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { columnasDe } from '../ventas/Reportes';
import { imprimirReporte } from '../../componentes/ReporteImpreso';
import { esc } from '../../global/imprimir';
import { useSesion } from '../../global/Sesion';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/distribucion/reportes';
const dos = (n) => String(n).padStart(2, '0');
const isoLocal = (d) => `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`;
const hoy = () => isoLocal(new Date());
const compacto = (n) => Number(n || 0).toLocaleString('es-BO', { notation: 'compact', maximumFractionDigits: 1 });
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

export const CONFIG_DIST = {
  'ingresos-egresos-anual': { titulo: 'Ingresos y egresos anual', sub: 'Reporte Ingresos Egresos Anual', icono: 'chart', filtros: ['anio'] },
  'ingresos-egresos-detallado': { titulo: 'Ingresos y egresos detallado', sub: 'Reporte Ingresos Egresos Detallado', icono: 'bars', filtros: ['fechas'] },
};

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
  const [f, setF] = useState({ desde: hoy(), hasta: hoy(), anio: '' });
  const [aux, setAux] = useState({});
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(false);
  const set = (k) => (x) => setF((o) => ({ ...o, [k]: x }));
  const tiene = (x) => cfg.filtros.includes(x);

  useEffect(() => {
    setDatos(null);
    if (!tiene('anio')) return;
    api.get(`${URL}/ingresos-egresos-anual/anios`).then((r) => {
      setAux({ anios: r.data });
      if (r.data?.length) setF((o) => ({ ...o, anio: r.data[r.data.length - 1].cnnum })); // cbAnio: ultimo año
    }).catch((e) => toast(e.message, 'error'));
  }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps
  useAlActivar(() => { if (tiene('anio')) api.get(`${URL}/ingresos-egresos-anual/anios`).then((r) => setAux({ anios: r.data })).catch((e) => toast(e.message, 'error')); }, { alMontar: false });

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
    if (clave === 'ingresos-egresos-anual') {
      return columnasDe(filas).map((c) => (c.campo === 'Concepto' ? { ...c, render: (x) => <b>{x.Concepto}</b> }
        : { ...c, tipo: 'bs', total: false, render: (x) => (x.Concepto === 'SALDO' ? <b className={Number(x[c.campo]) < 0 ? 'txt-neg' : 'txt-pos'}>{bs(x[c.campo])}</b> : bs(x[c.campo])) }));
    }
    return columnasDe(filas);
  }, [filas, clave]);
  const rango = tiene('fechas') ? `Del ${f.desde.split('-').reverse().join('/')} al ${f.hasta.split('-').reverse().join('/')}` : `Gestión ${anioTxt}`;

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
          <button className="btn btn-pri filtro-boton" onClick={generar} disabled={cargando || (tiene('anio') && !f.anio)}>{cargando ? <span className="spinner" /> : <Icono n="search" size={16} />} Generar</button>
        </div>

        {datos && clave === 'ingresos-egresos-anual' && datos.grafico && <GraficoAnual datos={datos.grafico} anio={anioTxt} />}
        {datos && clave === 'ingresos-egresos-detallado' && (
          <DetalleIngresosEgresos filas={filas} rango={rango} cabecera={datos.cabecera} usuario={tit(sesion?.gs_user || '')} />
        )}
        {datos && clave === 'ingresos-egresos-anual' && (
          <TablaReporte titulo={cfg.sub.toUpperCase()} subtitulo={rango} columnas={columnas} filas={filas} cargando={cargando} claveFila="__no"
            vacio="No Existen Datos Para Mostrar"
            onImprimir={datos.grafico ? () => imprimirReporte({
              titulo: 'Reporte de ingresos y egresos anual', subtitulo: `Gestión ${anioTxt}`, usuario: tit(sesion?.gs_user || ''),
              empresa: datos.cabecera?.empresa, imagenEmpresa: datos.cabecera?.imagenEmpresa, datos: [['Gestión:', anioTxt]],
              horizontal: true, sinTabla: true, filas, unidad: 'concepto', antes: htmlAnual(filas, datos.grafico), css: CSS_ANUAL,
            }) : undefined} />
        )}
        {!datos && !cargando && <div className="reporte-vacio"><Icono n="search" size={28} /><span>Elija los filtros y presione <b>Generar</b></span></div>}
      </div>
    </section>
  );
}

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
