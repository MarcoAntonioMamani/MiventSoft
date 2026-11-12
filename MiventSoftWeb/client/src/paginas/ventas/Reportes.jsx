// =====================================================================
//  KAILY LA PAZ: reportes del Panel_Ventas (Rep_VentasRealizadas, Reporte_VentasVsCostos, Reporte_ProductoMasVendido,
//  Reporte_RendimientoPersonal). Kaily quito el filtro de cliente de Ventas realizadas y Ventas vs costos.
//  Mismos filtros que el escritorio; las columnas son las que devuelve el SP.
//  "Datos" = tabla (Imprimir / Excel) · "Grafico" = barras (swTipoReporte del escritorio)
//  Ventas realizadas, Ventas vs costos y Productos mas vendidos: impresion y graficos como el escritorio (VentasRealizadas.js, MODOS)
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti } from '../../componentes/Controles';
import { BarrasH } from '../../componentes/Graficos';
import { imprimirReporte } from '../../componentes/ReporteImpreso';
import { useSesion } from '../../global/Sesion';
import { MODOS, CSS_GRAFICO_VR } from './VentasRealizadas';
import RendimientoMensual from './RendimientoMensual';
import { useToast } from '../../componentes/Toast';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/ventas/reportes';
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

// "NombreCliente" -> "Nombre cliente"
const etiqueta = (k) => {
  const t = String(k).replace(/_/g, ' ').replace(/([a-záéíóú])([A-Z])/g, '$1 $2').trim();
  return t.charAt(0).toUpperCase() + t.slice(1).toLowerCase();
};
const NO_SUMAR = /(^id$|id$|^id|codigo|cod$|chk|dias|mora|conversion|porcentaje|precio|^nro|numero|año|anio|mes$|hora|sucursal)/i;

// Columnas a partir de lo que devolvio el SP
export function columnasDe(filas) {
  const f = filas[0] || {};
  // claves internas (ClienteId, IdProducto, SucursalId...) no se muestran; "Id" si (es el codigo)
  const interna = (k) => /^.+Id$/.test(k) || /^Id[A-Z]/.test(k) || /^.+ID$/.test(k);
  return Object.keys(f).filter((k) => !/^(chk|img|imagen|estado)$/i.test(k) && !interna(k)).map((k) => {
    const x = filas.find((r) => r[k] !== null && r[k] !== undefined)?.[k];
    const esNum = typeof x === 'number';
    const fecha = typeof x === 'string' && /^\d{4}-\d{2}-\d{2}/.test(x);
    const idLike = /^id$/i.test(k) || /^(nro|numero|codigo|credito|venta)$/i.test(k) || /^(nro|numero|cod)[A-Z_]/.test(k);
    return {
      campo: k, titulo: etiqueta(k),
      tipo: esNum ? (idLike ? 'texto' : /cantidad|pedidos|unid/i.test(k) ? 'num' : 'bs') : fecha ? 'fecha' : 'texto',
      total: esNum && !idLike && !NO_SUMAR.test(k),
      render: !esNum && !fecha && typeof x === 'string' && x === x.toUpperCase() && x.length > 3 ? (r) => tit(r[k]) : undefined,
    };
  });
}

const CONFIG = {
  'ventas-realizadas': { titulo: 'Reporte de ventas', sub: 'Reporte De Ventas Realizadas', icono: 'list', filtros: ['vendedor'] },
  'ventas-costos': { titulo: 'Ventas vs costos', sub: 'Reporte De Utilidades Ventas', icono: 'dollar', filtros: ['vendedor'] },
  'productos-mas-vendidos': { titulo: 'Productos más vendidos', sub: 'Reporte De Productos Mas Vendidos', icono: 'box', filtros: [], topN: true },
  rendimiento: { titulo: 'Rendimiento del personal', sub: 'Reporte Rendimiento De Personal', icono: 'users', filtros: ['personal'] },
};

export default function Reporte({ clave }) {
  const cfg = CONFIG[clave];
  const toast = useToast();
  const [f, setF] = useState({ desde: hoy(), hasta: hoy(), vendedor: '', personal: [] });
  const [aux, setAux] = useState({ personal: [], rendimiento: [] });
  const [filas, setFilas] = useState(null);
  const [cargando, setCargando] = useState(false);
  const [vista, setVista] = useState('datos');
  const [topN, setTopN] = useState(10);
  const set = (k) => (v) => setF((x) => ({ ...x, [k]: v?.target ? v.target.value : v }));
  const { sesion } = useSesion();
  const [cabecera, setCabecera] = useState(null);
  const [consulta, setConsulta] = useState(null); // filtros con los que se genero (para el reporte impreso)
  const modo = MODOS[clave];
  const esVR = !!modo;

  useEffect(() => {
    setFilas(null);
    if (cfg.filtros.includes('vendedor')) api.get(`${URL}/aux/personal`).then((r) => setAux((a) => ({ ...a, personal: [{ Id: '', Nombre: 'TODOS' }, ...r.data] }))).catch(() => {});
    if (cfg.filtros.includes('personal')) api.get(`${URL}/rendimiento/personal`).then((r) => setAux((a) => ({ ...a, rendimiento: r.data }))).catch(() => {});
  }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps
  // al volver a la pestaña: personal actualizado de la BD (sin borrar el reporte generado)
  useAlActivar(() => {
    if (cfg.filtros.includes('vendedor')) api.get(`${URL}/aux/personal`).then((r) => setAux((a) => ({ ...a, personal: [{ Id: '', Nombre: 'TODOS' }, ...r.data] }))).catch(() => {});
    if (cfg.filtros.includes('personal')) api.get(`${URL}/rendimiento/personal`).then((r) => setAux((a) => ({ ...a, rendimiento: r.data }))).catch(() => {});
  }, { alMontar: false });

  // Rendimiento: ventas del rango por persona para ordenar la lista (con ventas primero, sin ventas al final).
  // Se recalcula al cambiar Desde/Hasta, con espera de 400 ms para no consultar en cada tecla.
  const [actividad, setActividad] = useState(null); // Map id -> { total, pedidos }
  const [cargandoAct, setCargandoAct] = useState(false);
  useEffect(() => {
    if (clave !== 'rendimiento' || f.desde > f.hasta) return undefined;
    let vivo = true;
    const t = setTimeout(() => {
      setCargandoAct(true);
      api.get(`${URL}/rendimiento/actividad?${new URLSearchParams({ desde: f.desde, hasta: f.hasta })}`)
        .then((r) => {
          if (!vivo) return;
          setActividad(new Map(r.data.personal.map((x) => [x.id, x])));
          if (r.data.aviso) toast(r.data.aviso, 'error', 7000);
        })
        .catch(() => { if (vivo) setActividad(null); }) // sin datos: se deja el orden original
        .finally(() => { if (vivo) setCargandoAct(false); });
    }, 400);
    return () => { vivo = false; clearTimeout(t); };
  }, [clave, f.desde, f.hasta]); // eslint-disable-line react-hooks/exhaustive-deps
  const personalOrden = useMemo(() => {
    const l = aux.rendimiento.map((p, i) => { const a = actividad?.get(Number(p.Id)); return { p, i, a, con: !!a && a.total > 0 }; });
    if (!actividad) return l;
    return l.sort((x, y) => (y.con - x.con) || ((y.a?.total || 0) - (x.a?.total || 0)) || (x.i - y.i));
  }, [aux.rendimiento, actividad]);
  const nConVentas = personalOrden.filter((x) => x.con).length;

  // empresa y logo para imprimir el grafico de rendimiento
  useEffect(() => {
    if (clave === 'rendimiento' && vista === 'grafico' && !cabecera) api.get(`${URL}/aux/cabecera`).then((r) => setCabecera(r.data)).catch(() => setCabecera({}));
  }, [clave, vista, cabecera]);
  const fBase = f;
  // over: filtros que se cambian y generan en el mismo paso (ej. "Ver últimos 12 meses")
  const generar = async (over = {}) => {
    const f = { ...fBase, ...over };
    if (Object.keys(over).length) setF(f);
    if (f.desde > f.hasta) { toast('La fecha Desde no puede ser mayor que Hasta', 'error'); return; }
    if (clave === 'rendimiento' && !f.personal.length) { toast('Seleccione al menos un Personal', 'error'); return; }
    const q = new URLSearchParams({ desde: f.desde, hasta: f.hasta });
    if (f.vendedor) q.set('vendedor', f.vendedor);
    if (clave === 'rendimiento') q.set('personal', f.personal.join(','));
    setCargando(true);
    try {
      const r = await api.get(`${URL}/${clave}?${q}`);
      setFilas(r.data);
      setConsulta({ desde: f.desde, hasta: f.hasta, vendedor: f.vendedor ? (aux.personal.find((p) => String(p.Id) === String(f.vendedor))?.Nombre || f.vendedor) : 'TODOS', personal: [...f.personal] });
      if (!r.data.length) toast('No Existen Datos Para Mostrar con Los Filtros Seleccionados'.toUpperCase(), 'error');
    } catch (e) { toast(e.message, 'error', 7000); } finally { setCargando(false); }
  };

  const columnas = useMemo(() => columnasDe(filas || []), [filas]);
  // Grafico: primer texto (descriptivo) vs la columna numerica principal
  const grafico = useMemo(() => {
    if (!filas?.length) return null;
    const etq = columnas.find((c) => c.tipo === 'texto' && !/id$/i.test(c.campo) && /nombre|producto|personal|cliente|categoria/i.test(c.campo)) || columnas.find((c) => c.tipo === 'texto');
    const val = [...columnas].reverse().find((c) => c.total && /total|monto|utilidad|importe/i.test(c.campo)) || columnas.find((c) => c.total);
    if (!etq || !val) return null;
    const m = new Map();
    for (const r of filas) { const k = String(r[etq.campo] ?? '—'); m.set(k, (m.get(k) || 0) + Number(r[val.campo] || 0)); }
    return { val, etq, datos: [...m].map(([nombre, v]) => ({ nombre: tit(nombre), v })).sort((a, b) => b.v - a.v).slice(0, cfg.topN ? topN : 15) };
  }, [filas, columnas, topN, cfg.topN]);

  const rango = `Del ${f.desde.split('-').reverse().join('/')} al ${f.hasta.split('-').reverse().join('/')}`;

  // ---- Ventas realizadas ----
  const filasVR = useMemo(() => (esVR && filas ? modo.normalizar(filas) : []), [esVR, modo, filas]);
  const graficoVR = useMemo(() => (esVR && filasVR.length && modo.htmlGrafico ? modo.htmlGrafico(filasVR, { topN }) : null), [esVR, filasVR, modo, topN]);
  const imprimirVR = async (fs, grafico = false) => {
    if (!fs.length) { toast('No Existen Datos Para Generar el Reporte'.toUpperCase(), 'error'); return; }
    let cab = cabecera;
    if (!cab) { try { cab = (await api.get(`${URL}/aux/cabecera`)).data; setCabecera(cab); } catch { cab = {}; } }
    const c = consulta;
    const filtros = [[modo.etiquetaDesde || 'Desde:', c.desde.split('-').reverse().join('/')], ['Hasta:', c.hasta.split('-').reverse().join('/')],
      ...(c.vendedor !== 'TODOS' ? [['Personal:', tit(c.vendedor)]] : [])];
    const base = { ...cab, usuario: tit(sesion?.gs_user || ''), filas: fs, unidad: modo.unidad };
    if (grafico) {
      const n = modo.topN ? Math.min(topN, fs.length) : fs.length;
      imprimirReporte({ ...base, filas: fs.slice(0, n), titulo: modo.tituloGrafico, subtitulo: modo.subtituloGrafico,
        datos: [...filtros, ...(modo.topN ? [['Mostrar:', `${n} productos`]] : [])], sinTabla: true, antes: modo.htmlGrafico(fs, { topN }), css: CSS_GRAFICO_VR });
    } else {
      imprimirReporte({ ...base, titulo: modo.titulo, subtitulo: modo.subtituloFn ? modo.subtituloFn(fs) : modo.subtitulo, columnas: modo.colsImpresion(fs), niveles: modo.niveles(fs), datos: [...filtros, ...modo.resumen(fs)] });
    }
  };

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
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={f.desde} max={f.hasta} onChange={(e) => e.target.value && set('desde')(e)} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={f.hasta} min={f.desde} onChange={(e) => e.target.value && set('hasta')(e)} /></label>
          {cfg.filtros.includes('vendedor') && (
            <div className="filtro filtro-ancho"><span>Vendedor</span>
              <ComboMulti dt={aux.personal} valor={f.vendedor} onChange={set('vendedor')} id="Id" nameId="Cód." descripcionId="Nombre" descripcion="Vendedor" />
            </div>
          )}
          <button className="btn btn-pri filtro-boton" onClick={() => generar()} disabled={cargando}>{cargando ? <span className="spinner" /> : <Icono n="search" size={16} />} Generar</button>
        </div>

        {cfg.filtros.includes('personal') && (
          <div className="tarjeta selec-personal">
            <div className="selec-cab">
              <b>Personal ({f.personal.length} de {aux.rendimiento.length})
                {(actividad || cargandoAct) && <small className="sub-rango">{cargandoAct ? 'actualizando ventas del rango…' : `${nConVentas} con ventas en el rango`}</small>}
              </b>
              <button className="btn btn-txt btn-sm" onClick={() => setF((x) => ({ ...x, personal: x.personal.length === aux.rendimiento.length ? [] : aux.rendimiento.map((p) => p.Id) }))}>
                {f.personal.length === aux.rendimiento.length ? 'Quitar todos' : 'Todos'}
              </button>
            </div>
            <div className="selec-lista">
              {personalOrden.map(({ p, a, con }, k) => {
                const on = f.personal.includes(p.Id);
                const sin = !!actividad && !con;
                const detalle = [p.Cargo && tit(p.Cargo), actividad && (con ? `Bs ${bs(a.total)} · ${a.pedidos} ped.` : 'Sin ventas')].filter(Boolean).join(' · ');
                return [
                  sin && k === nConVentas && (
                    <div key="sep" className="sub-rango" style={{ gridColumn: '1 / -1', margin: '6px 0 0' }}>Sin ventas en el rango ({personalOrden.length - nConVentas})</div>
                  ),
                  <label key={p.Id} className={`selec-item ${on ? 'on' : ''}`} style={sin && !on ? { opacity: 0.55 } : undefined}>
                    <input type="checkbox" checked={on} onChange={() => setF((x) => ({ ...x, personal: on ? x.personal.filter((i) => i !== p.Id) : [...x.personal, p.Id] }))} />
                    <span><b>{tit(p.NombrePersonal)}</b>{detalle && <small>{detalle}</small>}</span>
                  </label>,
                ];
              })}
            </div>
          </div>
        )}

        {filas && (
          <>
            {!modo?.sinGrafico && <div className="vista-reporte" role="tablist" aria-label="Vista">
              <button role="tab" aria-selected={vista === 'datos'} className={vista === 'datos' ? 'activa' : ''} onClick={() => setVista('datos')}><Icono n="list" size={15} /> Datos</button>
              <button role="tab" aria-selected={vista === 'grafico'} className={vista === 'grafico' ? 'activa' : ''} onClick={() => setVista('grafico')} disabled={esVR ? !filasVR.length : clave === 'rendimiento' ? !filas?.length : !grafico}><Icono n="grid" size={15} /> Gráfico</button>
              {cfg.topN && vista === 'grafico' && <label className="top-n">Mostrar <input className="control num" type="number" min="1" max="50" value={topN} onChange={(e) => setTopN(Math.max(1, Math.min(50, Number(e.target.value) || 10)))} /> productos</label>}
            </div>}
            {esVR && (vista === 'datos' || modo.sinGrafico) && (
              <TablaReporte titulo={cfg.sub.toUpperCase()} subtitulo={rango} columnas={typeof modo.colsPantalla === 'function' ? modo.colsPantalla(filasVR) : modo.colsPantalla} filas={filasVR} cargando={cargando} claveFila="__n" vacio="No Existen Datos Para Mostrar"
                onImprimir={(fs) => imprimirVR(fs)} />
            )}
            {esVR && !modo.sinGrafico && vista === 'grafico' && graficoVR && <GraficoVentas html={graficoVR} modo={modo} rango={rango} onImprimir={() => imprimirVR(filasVR, true)} />}
            {clave === 'rendimiento' && vista === 'grafico' && consulta && (
              <RendimientoMensual desde={consulta.desde} hasta={consulta.hasta} personal={consulta.personal}
                usuario={tit(sesion?.gs_user || '')} cabecera={cabecera || {}}
                filtrosTxt={[['Desde:', consulta.desde.split('-').reverse().join('/')], ['Hasta:', consulta.hasta.split('-').reverse().join('/')]]}
                onUltimos12={() => { const d = new Date(); const ini = new Date(d.getFullYear(), d.getMonth() - 11, 1); generar({ desde: `${ini.getFullYear()}-${dos(ini.getMonth() + 1)}-01`, hasta: hoy() }); }} />
            )}
            {esVR || (clave === 'rendimiento' && vista === 'grafico') ? null : vista === 'datos' || !grafico ? (
              <TablaReporte titulo={cfg.sub.toUpperCase()} subtitulo={rango} columnas={columnas} filas={filas} cargando={cargando} claveFila="__no" vacio="No Existen Datos Para Mostrar" />
            ) : (
              <div className="tarjeta">
                <h2 className="tarjeta-tit">{grafico.val.titulo} por {grafico.etq.titulo.toLowerCase()} <small className="sub-rango">{rango}</small></h2>
                <BarrasH filas={grafico.datos} valor="v" etiqueta="nombre" formato={(n) => (grafico.val.tipo === 'bs' ? `Bs ${bs(n)}` : Number(n).toLocaleString('es-BO'))} />
              </div>
            )}
          </>
        )}
        {!filas && !cargando && <div className="reporte-vacio"><Icono n="search" size={28} /><span>Elija los filtros y presione <b>Generar</b></span></div>}
      </div>
    </section>
  );
}

// ReporteGraficoVentasRealizadas / ReporteGraficoUtilidades: los dos graficos del escritorio (mismo dibujo que la hoja impresa)
function GraficoVentas({ html, modo, rango, onImprimir }) {
  return (
    <div className="tarjeta rv-grafico">
      <style>{CSS_GRAFICO_VR}</style>
      <div className="rv-cab">
        <h2 className="tarjeta-tit">{modo.tituloGrafico} <small className="sub-rango">{modo.subtituloGrafico ? `${modo.subtituloGrafico} · ` : ''}{rango}</small></h2>
        <button className="btn btn-sec btn-sm rv-imprimir" onClick={onImprimir}><Icono n="list" size={15} /> Imprimir gráfico</button>
      </div>
      {/* HTML armado con texto escapado (esc) */}
      <div dangerouslySetInnerHTML={{ __html: html }} />
    </div>
  );
}

export const REPORTES = Object.keys(CONFIG);
