// =====================================================================
//  Panel de control (pantalla principal de Sanbit)
//  Datos de /api/dashboard (solo agregados, calculados con los SP del escritorio)
// =====================================================================
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useSesion } from '../global/Sesion';
import { titulo } from '../global/programas';
import { api } from '../api/http';
import { ComboMulti } from '../componentes/Controles';
import { LineaHoras, BarrasDias, BarrasH, BarraSegmentada, Dispersion, fmtBs, fmtN } from '../componentes/Graficos';
import Icono from '../componentes/Icono';
import DetalleDashboard from '../componentes/DetalleDashboard';

const REFRESCO_MS = 120000; // 2 minutos
const COLOR_ESTADO = (n) => (/ENTREG/.test(n) ? 'var(--estado-ok)' : /ANUL/.test(n) ? 'var(--estado-mal)' : /PEND/.test(n) ? 'var(--estado-aviso)' : /ASIG|RUTA|DESP/.test(n) ? 'var(--serie-1)' : 'var(--serie-7)');
const COLOR_TIPO = (n) => (/CONT/.test(n) ? 'var(--serie-1)' : 'var(--serie-2)');

function hoyLocal() {
  const d = new Date();
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function Variacion({ actual, base, etiqueta, invertir }) {
  if (!base) return <span className="chip-var neutro">— {etiqueta}</span>;
  const p = ((actual - base) / base) * 100;
  const sube = p >= 0;
  const bueno = invertir ? !sube : sube;
  return (
    <span className={`chip-var ${bueno ? 'sube' : 'baja'}`} title={`${etiqueta}: ${base.toLocaleString('es-BO')}`}>
      {sube ? '▲' : '▼'} {Math.abs(p).toLocaleString('es-BO', { maximumFractionDigits: 1 })}% <em>{etiqueta}</em>
    </span>
  );
}

function Kpi({ titulo: t, valor, pie, color, children, icono, onClick }) {
  const clic = onClick ? { role: 'button', tabIndex: 0, onClick, onKeyDown: (e) => (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), onClick()), title: 'Ver detalle' } : {};
  return (
    <article className={`kpi ${onClick ? 'clic' : ''}`} style={{ '--kpi': color }} {...clic}>
      <header><span>{t}</span><Icono n={icono} size={17} /></header>
      <div className="kpi-valor">{valor}</div>
      <div className="kpi-chips">{children}</div>
      {pie && <footer>{pie}</footer>}
    </article>
  );
}

function Panel({ titulo: t, sub, children, accion, clase = '' }) {
  return (
    <section className={`panel ${clase}`}>
      <header className="panel-cab">
        <div><h2>{t}</h2>{sub && <p>{sub}</p>}</div>
        {accion}
      </header>
      <div className="panel-cuerpo">{children}</div>
    </section>
  );
}

const MESES = ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio', 'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'];

/** Tarjeta de "destacado del mes": ganador grande + puestos 2..5 */
function Destacado({ etiqueta, icono, color, lista, valor = 'monto', formato = fmtBs, pie, vacio, onElegir, onVerTodo }) {
  const [primero, ...resto] = lista || [];
  const clic = (x) => (onElegir ? { role: 'button', tabIndex: 0, onClick: () => onElegir(x), onKeyDown: (e) => (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), onElegir(x)), title: 'Ver detalle' } : {});
  return (
    <article className="destacado" style={{ '--dest': color }}>
      <header className="destacado-cab">
        <span className="destacado-ic"><Icono n={icono} size={18} /></span>
        <span className="destacado-etq">{etiqueta}</span>
        {onVerTodo && <button className="btn btn-txt btn-sm" onClick={onVerTodo}>Ver todo</button>}
      </header>
      {!primero ? <SinDatos texto={vacio} /> : (
        <>
          <div className={`destacado-ganador ${onElegir ? 'clic' : ''}`} {...clic(primero)}>
            <span className="destacado-trofeo" aria-hidden="true">1</span>
            <div className="destacado-datos">
              <b className="destacado-nombre" title={titulo(primero.nombre)}>{titulo(primero.nombre)}</b>
              <span className="destacado-valor">{formato(primero[valor])}</span>
              <span className="destacado-pie">{pie(primero)}</span>
            </div>
          </div>
          {resto.length > 0 && (
            <ol className="destacado-resto" start={2}>
              {resto.map((x, i) => (
                <li key={(x.id ?? x.nombre) + i} className={onElegir ? 'clic' : ''} {...clic(x)}>
                  <span className="pos">{i + 2}</span>
                  <span className="nom" title={titulo(x.nombre)}>{titulo(x.nombre)}</span>
                  <b>{formato(x[valor])}</b>
                </li>
              ))}
            </ol>
          )}
        </>
      )}
    </article>
  );
}

function SinDatos({ texto = 'Sin datos para este filtro' }) {
  return <div className="panel-vacio"><Icono n="grid" size={26} /><span>{texto}</span></div>;
}

/** Selector de opciones en pastilla (periodo, criterio) */
function Segmentos({ opciones, valor, onChange, etiqueta }) {
  return (
    <div className="segmentos" role="radiogroup" aria-label={etiqueta}>
      {opciones.map(([k, t]) => (
        <button key={k} type="button" role="radio" aria-checked={valor === k} className={valor === k ? 'activo' : ''} onClick={() => onChange(k)}>{t}</button>
      ))}
    </div>
  );
}

// Horas a mostrar en "Ventas por hora": del horario real (dia o promedio) +-1 h, minimo 8 horas
function rangoHoras(ph) {
  const con = (ph || []).filter((h) => h.monto > 0 || h.promedio > 0).map((h) => h.hora);
  if (!con.length) return [8, 20];
  let a = Math.max(0, Math.min(...con) - 1);
  let b = Math.min(23, Math.max(...con) + 1);
  while (b - a < 8 && (a > 0 || b < 23)) { if (b < 23) b += 1; else a -= 1; }
  return [a, b];
}

const PERIODOS_TXT = { 30: 'Últimos 30 días', 90: 'Últimos 90 días', 365: 'Últimos 12 meses' };
const fechaTxtCorta = (d) => { if (!d) return '—'; const [a, m, dd] = String(d).split('-'); return `${dd}/${m}/${a}`; };
const diasTxt = (n) => `${fmtN(n)} ${n === 1 ? 'día' : 'días'}`;
const COLOR_TRAMO = ['var(--estado-aviso)', 'var(--estado-mal)', 'var(--mora-grave)', 'var(--txt-4)'];

function Esqueleto() {
  return (
    <div className="dash-grid esqueleto-dash" aria-hidden="true">
      {[1, 2, 3, 4].map((i) => <div key={i} className="kpi"><span className="esq-barra" style={{ width: '40%' }} /><span className="esq-barra alta" style={{ width: '70%' }} /></div>)}
      <div className="panel span-8"><span className="esq-barra" style={{ width: '30%' }} /><div className="esq-bloque" /></div>
      <div className="panel span-4"><span className="esq-barra" style={{ width: '50%' }} /><div className="esq-bloque" /></div>
    </div>
  );
}

export default function Inicio() {
  const { sesion } = useSesion();
  const [fecha, setFecha] = useState(hoyLocal());
  const [vendedor, setVendedor] = useState('');
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState('');
  const [actualizado, setActualizado] = useState(null);
  const [detalle, setDetalle] = useState(null);
  const pedido = useRef(0);
  const esHoy = fecha === hoyLocal();

  const cargar = useCallback(async () => {
    const n = ++pedido.current;
    setCargando(true);
    try {
      const q = new URLSearchParams({ fecha, ...(vendedor ? { vendedor } : {}) });
      const r = await api.get(`/api/dashboard?${q}`);
      if (n !== pedido.current) return;
      setDatos(r.data);
      setError('');
      setActualizado(new Date());
    } catch (e) {
      if (n === pedido.current) setError(e.message);
    } finally {
      if (n === pedido.current) setCargando(false);
    }
  }, [fecha, vendedor]);

  useEffect(() => { cargar(); }, [cargar]);

  // Analisis de clientes y productos (MAM_PanelControl): aparte, para no demorar los indicadores; solo cambia con la fecha
  const [analisis, setAnalisis] = useState(null);
  const [cargandoAn, setCargandoAn] = useState(true);
  const [periodo, setPeriodo] = useState('90');
  const [criterio, setCriterio] = useState('monto');
  const pedidoAn = useRef(0);
  const cargarAnalisis = useCallback(async () => {
    const n = ++pedidoAn.current;
    setCargandoAn(true);
    try {
      const r = await api.get(`/api/dashboard/analisis?${new URLSearchParams({ fecha })}`);
      if (n === pedidoAn.current) setAnalisis(r.data);
    } catch (e) {
      if (n === pedidoAn.current) setAnalisis({ error: e.message });
    } finally {
      if (n === pedidoAn.current) setCargandoAn(false);
    }
  }, [fecha]);
  useEffect(() => { cargarAnalisis(); }, [cargarAnalisis]);
  const actualizar = () => { cargar(); cargarAnalisis(); };

  // Actualizacion automatica solo para el dia de hoy
  useEffect(() => {
    if (!esHoy) return undefined;
    const t = setInterval(() => document.visibilityState === 'visible' && cargar(), REFRESCO_MS);
    return () => clearInterval(t);
  }, [esHoy, cargar]);

  const v = datos?.ventas;
  const k = v?.kpis;
  const vendedores = useMemo(() => [{ id: '', nombre: 'TODOS LOS VENDEDORES' }, ...(v?.vendedores || [])], [v?.vendedores]);
  const nomVend = vendedores.find((x) => x.id === vendedor)?.nombre;
  const fechaTxt = new Date(`${fecha}T12:00:00`).toLocaleDateString('es-BO', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
  const h = new Date().getHours();
  const saludo = h < 12 ? 'Buenos días' : h < 19 ? 'Buenas tardes' : 'Buenas noches';

  // ---------- detalle al hacer clic ----------
  const subDia = `${fechaTxt.charAt(0).toUpperCase()}${fechaTxt.slice(1)}${vendedor && nomVend ? ` · ${titulo(nomVend)}` : ''}`;
  const verPedidos = (t, extra = {}, accion) => setDetalle({
    tipo: 'pedidos', titulo: t, sub: extra.mes ? subMesTxt() : extra.vendedor && extra.vendedor !== vendedor ? `${fechaTxt} · ${titulo(vendedores.find((x) => x.id === extra.vendedor)?.nombre || '')}` : subDia,
    params: { fecha, vendedor, ...extra }, accion,
  });
  function subMesTxt() { const f = new Date(`${fecha}T12:00:00`); return `Del 1 al ${f.getDate()} de ${MESES[f.getMonth()]} ${f.getFullYear()}`; }
  const filtrarVend = (id) => ({ texto: 'Filtrar panel por este vendedor', fn: () => setVendedor(String(id)) });
  const subCobrar = vendedor && nomVend ? `Clientes de ${titulo(nomVend)}` : 'Todos los clientes';
  const verCobrar = (t, extra = {}) => setDetalle({ tipo: 'cobrar', titulo: t, sub: subCobrar, params: { vendedorNombre: vendedor ? nomVend : '', ...extra } });
  const verPagar = (t, extra = {}) => setDetalle({ tipo: 'pagar', titulo: t, sub: 'Proveedores', params: extra });
  const m = v?.mes;
  const fMes = new Date(`${fecha}T12:00:00`);
  const mesTxt = `${MESES[fMes.getMonth()]} ${fMes.getFullYear()}`;
  const subMes = `${mesTxt.charAt(0).toUpperCase()}${mesTxt.slice(1)} · del 1 al ${fMes.getDate()}`;
  const pct = (x, total) => (total ? `${Math.round((x / total) * 100)}%` : '');
  // Kaily (punto de venta): los textos hablan de ventas (los datos siguen llamandose "pedidos")
  const pedidosTxt = (n) => `${fmtN(n)} ${n === 1 ? 'venta' : 'ventas'}`;
  const TRAMO_TXT = ['al día', 'con 1 a 30 días de mora', 'con 31 a 60 días de mora', 'con más de 60 días de mora'];
  const horas = rangoHoras(v?.porHora);

  // ---------- analisis: clientes y productos ----------
  const an = analisis && !analisis.error ? analisis : null;
  const rent = an?.clientes?.rentables?.[periodo];
  const sc = an?.clientes?.sinCompra;
  const ps = an?.productos;
  const topRent = rent ? (criterio === 'monto' ? rent.topMonto : rent.topFrecuencia) : [];
  const idsTop = new Set(topRent.map((x) => x.id));
  const subFecha = esHoy ? 'al día de hoy' : `al ${fechaTxtCorta(fecha)}`;
  // compras de un cliente (todas, sin filtro de vendedor) en los ultimos N dias
  const verCompras = (x, dias) => setDetalle({
    tipo: 'pedidos', titulo: `Compras de ${titulo(x.nombre)}`, sub: `${PERIODOS_TXT[dias] || `Últimos ${dias} días`} · todos los vendedores`, params: { fecha, dias: String(dias), cliente: x.id },
  });
  const verSinCompra = (t, extra = {}) => setDetalle({ tipo: 'sinCompra', titulo: t, sub: `Compraron en los últimos 12 meses · ${subFecha}`, params: { fecha, ...extra } });
  const verSinVenta = (t, extra = {}) => setDetalle({ tipo: 'sinVenta', titulo: t, sub: `Con stock y sin ventas en ${an?.diasSinCompra ?? 60} días · ${subFecha}`, params: { fecha, ...extra } });
  const verRanking = () => setDetalle({
    tipo: 'clientesTop', titulo: `Clientes que ${criterio === 'monto' ? 'más compran' : 'compran más seguido'}`, sub: `${PERIODOS_TXT[periodo]} · sin el cliente de mostrador`, params: { fecha, dias: periodo, orden: criterio },
  });
  const esqAnalisis = <div className="esq-panel" aria-hidden="true"><span className="esq-barra" style={{ width: '45%' }} /><span className="esq-barra alta" style={{ width: '30%' }} /><div className="esq-bloque bajo" /></div>;

  const alertas = datos ? [
    { n: datos.cobrar?.documentosVencidos, t: 'Créditos de clientes vencidos', i: 'wallet', nivel: 'mal', ver: () => verCobrar('Créditos de clientes vencidos', { vencido: '1' }) },
    { n: datos.alertas.stock0, t: 'Productos con stock en cero', i: 'box', nivel: 'mal', ver: () => setDetalle({ tipo: 'stock0', titulo: 'Productos con stock en cero', sub: 'Inventario actual' }) },
    { n: datos.alertas.stockMinimo, t: 'Productos bajo stock mínimo', i: 'alert', nivel: 'aviso', ver: () => setDetalle({ tipo: 'stockmin', titulo: 'Productos bajo stock mínimo', sub: 'Inventario actual' }) },
    { n: datos.pagar?.documentosVencidos, t: 'Pagos a proveedores vencidos', i: 'bag', nivel: 'aviso', ver: () => verPagar('Pagos a proveedores vencidos', { vencido: '1' }) },
  ].filter((a) => a.n !== null && a.n !== undefined) : [];

  return (
    <div className="dashboard entrada">
      <header className="dash-cab">
        <div className="dash-titulo">
          <span className="eyebrow-oscuro">{saludo}, {titulo(sesion.gs_user)}</span>
          <h1>Panel de control</h1>
          <p className="dash-fecha">{fechaTxt}{nomVend && vendedor ? ` · ${titulo(nomVend)}` : ''}</p>
        </div>
        <div className="dash-filtros">
          <label className="filtro">
            <span>Fecha</span>
            <input type="date" className="control" value={fecha} max={hoyLocal()} onChange={(e) => e.target.value && setFecha(e.target.value)} />
          </label>
          <div className="filtro filtro-vend">
            <span>Vendedor</span>
            <ComboMulti dt={vendedores} valor={vendedor} onChange={(x) => setVendedor(String(x ?? ''))} id="id" nameId="Cód." descripcionId="nombre" descripcion="Vendedor" />
          </div>
          <div className="dash-estado">
            {esHoy ? <span className="en-vivo"><i /> EN VIVO</span> : <button className="btn btn-txt btn-sm" onClick={() => setFecha(hoyLocal())}>Ir a hoy</button>}
            <button className="btn-icono" onClick={actualizar} title="Actualizar" aria-label="Actualizar">{(cargando && datos) || (cargandoAn && analisis) ? <span className="spinner" /> : <Icono n="refresh" size={18} />}</button>
            {actualizado && <small>Actualizado {actualizado.toLocaleTimeString('es-BO', { hour: '2-digit', minute: '2-digit' })}</small>}
          </div>
        </div>
      </header>

      {error && <div className="alerta"><Icono n="alert" size={16} /> {error}</div>}
      {datos?.errores?.length > 0 && <div className="alerta aviso"><Icono n="alert" size={16} /> No se pudo leer: {datos.errores.join(', ')}. El resto del panel está actualizado.</div>}

      {!datos ? <Esqueleto /> : (
        <div className={`dash-grid ${cargando ? 'recargando' : ''}`}>
          {/* ===== KPIs ===== */}
          <Kpi titulo="Ventas del día" icono="cart" color="var(--serie-1)" onClick={k?.pedidos || k?.anulados ? () => verPedidos('Ventas del día') : undefined} valor={k ? fmtN(k.pedidos) : '—'}
            pie={k ? `${k.anulados !== null ? `${fmtN(k.anulados)} ${k.anulados === 1 ? 'anulada' : 'anuladas'} · ` : ''}promedio ${fmtN(Math.round(k.promPedidos))}/día` : ''}>
            {k && <><Variacion actual={k.pedidos} base={k.pedidosAyer} etiqueta="vs ayer" /><Variacion actual={k.pedidos} base={k.promPedidos} etiqueta="vs prom." /></>}
          </Kpi>
          <Kpi titulo="Monto vendido" icono="dollar" color="var(--estado-ok)" onClick={k?.pedidos ? () => verPedidos('Monto vendido', { validos: '1' }) : undefined} valor={k ? fmtBs(k.monto) : '—'}
            pie={k ? `Promedio diario ${fmtBs(k.promMonto, 0)}` : ''}>
            {k && <><Variacion actual={k.monto} base={k.montoAyer} etiqueta="vs ayer" /><Variacion actual={k.monto} base={k.promMonto} etiqueta="vs prom." /></>}
          </Kpi>
          <Kpi titulo="Ticket promedio" icono="tag" color="var(--serie-7)" onClick={k?.pedidos ? () => verPedidos('Ventas del día', { validos: '1' }) : undefined} valor={k ? fmtBs(k.ticket) : '—'}
            pie={k ? `${fmtN(k.clientes)} ${k.clientes === 1 ? 'cliente atendido' : 'clientes atendidos'}` : ''}>
            {k && k.promPedidos > 0 && <Variacion actual={k.ticket} base={k.promMonto / k.promPedidos} etiqueta="vs prom." />}
          </Kpi>
          <Kpi titulo="Vendedor del día" icono="users" color="var(--serie-2)"
            onClick={v?.masPedidos ? () => verPedidos(`Ventas de ${titulo(v.masPedidos.nombre)}`, { vendedor: v.masPedidos.id, validos: '1' }, v.masPedidos.id !== vendedor ? filtrarVend(v.masPedidos.id) : undefined) : undefined}
            valor={v?.masPedidos ? <span className="kpi-nombre">{titulo(v.masPedidos.nombre)}</span> : '—'}
            pie={v?.masPedidos ? `${pedidosTxt(v.masPedidos.pedidos)} · ${fmtBs(v.masPedidos.monto)}` : 'Sin ventas en el día'}>
            {v?.ranking?.[0] && v.ranking[0].id !== v.masPedidos?.id && <span className="chip-var neutro">Mayor monto: {titulo(v.ranking[0].nombre)}</span>}
            {v?.masPedidos && <span className="chip-var trofeo">★ Más ventas</span>}
          </Kpi>

          {/* ===== Fila 2 ===== */}
          <Panel clase="span-8" titulo="Ventas por hora" sub={`${esHoy ? 'Hoy' : 'Día seleccionado'} vs promedio de los últimos 30 días`}
            accion={<div className="leyenda"><span><i className="dot s1" /> {esHoy ? 'Hoy' : 'Día'}</span><span><i className="dot prom" /> Promedio 30 días</span></div>}>
            {!v ? <SinDatos /> : !v.conHora ? <SinDatos texto="No se pudo leer la hora de las ventas" /> : (
              <LineaHoras datos={v.porHora} desde={horas[0]} hasta={horas[1]}
                onElegir={(hh) => verPedidos(`Ventas de ${String(hh).padStart(2, '0')}:00 a ${String(hh).padStart(2, '0')}:59`, { hora: String(hh), validos: '1' })} />
            )}
          </Panel>
          <Panel clase="span-4" titulo="Ranking de vendedores" sub="Monto vendido en el día">
            {v?.ranking?.length ? (
              <BarrasH filas={v.ranking.map((r) => ({ ...r, nombre: titulo(r.nombre) }))} valor="monto" etiqueta="nombre"
                destacar={(r) => vendedor && r.id === vendedor}
                onElegir={(r) => verPedidos(`Ventas de ${r.nombre}`, { vendedor: r.id, validos: '1' }, r.id !== vendedor ? filtrarVend(r.id) : undefined)}
                extra={(r) => <>{pedidosTxt(r.pedidos)}{r.id === v.masPedidos?.id ? <em className="estrella"> ★ más ventas</em> : ''}</>} />
            ) : <SinDatos texto="Sin ventas en el día" />}
          </Panel>

          {/* ===== Fila 3 ===== */}
          <Panel clase="span-8" titulo="Rendimiento por día" sub="Monto vendido, últimos 30 días · toque una barra para ver ese día">
            {v ? <BarrasDias datos={v.tendencia} seleccion={fecha} onElegir={setFecha} /> : <SinDatos />}
          </Panel>
          {/* Kaily: MAM_Ventas @tipo 3 no trae estados (estados = []): solo la forma de pago */}
          <Panel clase="span-4" titulo="Ventas del día" sub={v?.estados?.length ? 'Por estado y forma de pago' : 'Por forma de pago'}>
            {v?.estados?.length || v?.tipos?.length ? (
              <div className="pila">
                {v.estados.length > 0 && (
                  <BarraSegmentada partes={v.estados.map((e) => ({ ...e, orig: e.nombre, nombre: titulo(e.nombre), color: COLOR_ESTADO(e.nombre) }))}
                    onElegir={(e) => verPedidos(`Ventas en estado ${e.nombre.toLowerCase()}`, { estado: e.orig })} />
                )}
                <BarraSegmentada valor="monto" formato={(n) => fmtBs(n, 0)} partes={v.tipos.map((e) => ({ ...e, orig: e.nombre, nombre: titulo(e.nombre), color: COLOR_TIPO(e.nombre) }))}
                  onElegir={(e) => verPedidos(`Ventas al ${e.nombre.toLowerCase()}`, { tipo: e.orig, validos: '1' })} />
              </div>
            ) : <SinDatos texto="Sin ventas en el día" />}
          </Panel>

          {/* ===== Destacados del mes ===== */}
          <Panel clase="span-12" titulo="Destacados del mes" sub={`${subMes}${vendedor && nomVend ? ` · clientes de ${titulo(nomVend)}` : ''}`}
            accion={m ? <div className="mes-total"><span>Ventas del mes</span><b>{fmtBs(m.monto, 0)}</b><small>{pedidosTxt(m.pedidos)} · {fmtN(m.clientesAtendidos)} clientes</small></div> : null}>
            {m ? (
              <div className="destacados">
                <Destacado etiqueta="Cliente del mes" icono="store" color="var(--serie-1)" lista={m.clientes} vacio="Sin ventas en el mes"
                  pie={(x) => `${pedidosTxt(x.pedidos)} · ${pct(x.monto, m.monto)} de las ventas`}
                  onElegir={(x) => x.id && verPedidos(`Compras de ${titulo(x.nombre)} en ${mesTxt}`, { mes: '1', cliente: x.id, validos: '1' })} />
                <Destacado etiqueta="Producto del mes" icono="box" color="var(--serie-2)" lista={datos.productosMes} valor="total" vacio="Sin ventas de productos"
                  pie={(x) => `${fmtN(x.cantidad)} unidades vendidas${vendedor ? ' · todos los vendedores' : ''}`}
                  onVerTodo={datos.productosMes?.length ? () => setDetalle({ tipo: 'productosMes', titulo: `Productos más vendidos de ${mesTxt}`, sub: 'Todos los vendedores', params: { fecha } }) : undefined} />
                <Destacado etiqueta="Vendedor del mes" icono="users" color="var(--serie-7)" lista={m.vendedores} vacio="Sin ventas en el mes"
                  pie={(x) => `${pedidosTxt(x.pedidos)} · ${pct(x.monto, m.montoTodos)} de las ventas${m.masPedidos && m.masPedidos.id === x.id ? ' · ★ más ventas' : ''}`}
                  onElegir={(x) => verPedidos(`Ventas de ${titulo(x.nombre)} en ${mesTxt}`, { mes: '1', vendedor: x.id, validos: '1' }, x.id !== vendedor ? filtrarVend(x.id) : undefined)} />
              </div>
            ) : <SinDatos />}
          </Panel>

          {/* ===== Clientes rentables (MAM_PanelControl @tipo 2) ===== */}
          {analisis?.error && <div className="alerta aviso span-12"><Icono n="alert" size={16} /> Análisis de clientes y productos: {analisis.error}</div>}
          {an?.errores?.length > 0 && <div className="alerta aviso span-12"><Icono n="alert" size={16} /> No se pudo leer: {an.errores.join(', ')}.</div>}
          <Panel clase="span-12" titulo="Clientes rentables" sub={`Los que más compran y los que compran más seguido · ${PERIODOS_TXT[periodo].toLowerCase()} · sin el cliente de mostrador`}
            accion={(
              <div className="dash-toggles">
                <Segmentos etiqueta="Ordenar por" valor={criterio} onChange={setCriterio} opciones={[['monto', 'Más compran'], ['frecuencia', 'Más seguido']]} />
                <Segmentos etiqueta="Período" valor={periodo} onChange={setPeriodo} opciones={[['30', '30 días'], ['90', '90 días'], ['365', '12 meses']]} />
              </div>
            )}>
            {!analisis ? esqAnalisis : !rent ? <SinDatos texto="No disponible" /> : !rent.clientes ? <SinDatos texto="Sin compras de clientes en el período" /> : (
              <div className={`rent-grid ${cargandoAn ? 'recargando' : ''}`}>
                <div className="rent-col">
                  <div className="mini-kpis">
                    <div><span>Clientes</span><b>{fmtN(rent.clientes)}</b><small>con compras</small></div>
                    <div><span>Vendido a clientes</span><b>{fmtBs(rent.monto, 0)}</b></div>
                    <div><span>Los 10 primeros</span><b>{pct(rent.montoTop, rent.monto) || '—'}</b><small>de lo vendido</small></div>
                  </div>
                  <BarrasH key={`${periodo}-${criterio}`} color="var(--serie-2)" filas={topRent.map((x) => ({ ...x, nombre: titulo(x.nombre) }))} etiqueta="nombre"
                    valor={criterio === 'monto' ? 'monto' : 'dias'} formato={criterio === 'monto' ? (n) => fmtBs(n, 0) : diasTxt}
                    onElegir={(x) => verCompras(x, Number(periodo))}
                    extra={(x) => (criterio === 'monto'
                      ? `${pedidosTxt(x.compras)} en ${diasTxt(x.dias)} · ticket ${fmtBs(x.ticket, 0)}`
                      : `${fmtBs(x.monto, 0)} · ${pedidosTxt(x.compras)} · última ${fechaTxtCorta(x.ultima)}`)} />
                  <button type="button" className="btn btn-txt btn-sm rent-ver" onClick={verRanking}>Ver ranking completo <Icono n="next" size={14} /></button>
                </div>
                <div className="rent-col">
                  <h3 className="sub-tit">Frecuencia vs monto</h3>
                  <Dispersion key={periodo} puntos={rent.puntos.map((x) => ({ ...x, nombre: titulo(x.nombre) }))} destacar={(x) => idsTop.has(x.id)} onElegir={(x) => verCompras(x, Number(periodo))} />
                  <div className="leyenda leyenda-viz"><span><i className="dot s2" /> Los 10 primeros</span><span><i className="dot s1 suave" /> Otros clientes</span></div>
                  <h3 className="sub-tit">Concentración de las ventas</h3>
                  <BarraSegmentada valor="monto" formato={(n) => fmtBs(n, 0)} partes={[
                    { nombre: 'Los 10 primeros clientes', monto: rent.montoTop, color: 'var(--serie-2)' },
                    { nombre: 'Otros clientes', monto: Math.max(0, Math.round((rent.monto - rent.montoTop) * 100) / 100), color: 'var(--serie-1)' },
                    { nombre: 'Mostrador (cliente por defecto)', monto: rent.mostrador, color: 'var(--serie-7)' },
                  ]} />
                </div>
              </div>
            )}
          </Panel>

          {/* ===== Clientes que dejaron de comprar / Productos sin ventas ===== */}
          <Panel clase="span-6" titulo="Clientes que dejaron de comprar" sub={`Compraron en los últimos 12 meses y no volvieron en ${an?.diasSinCompra ?? 60} días`}
            accion={sc?.clientes ? <button className="btn btn-txt btn-sm" onClick={() => verSinCompra('Clientes que dejaron de comprar')}>Ver todo <Icono n="next" size={14} /></button> : null}>
            {!analisis ? esqAnalisis : !sc ? <SinDatos texto="No disponible" /> : !sc.clientes ? <SinDatos texto="Todos los clientes del año volvieron a comprar" /> : (
              <div className={`credito ${cargandoAn ? 'recargando' : ''}`}>
                <div className="credito-total">
                  <b>{fmtN(sc.clientes)} {sc.clientes === 1 ? 'cliente' : 'clientes'}</b>
                  <span>{pct(sc.clientes, sc.deTotal)} de los {fmtN(sc.deTotal)} clientes del año · compraban {fmtBs(sc.monto, 0)} · {fmtN(sc.frecuentes)} {sc.frecuentes === 1 ? 'era frecuente' : 'eran frecuentes'} (3+ días de compra)</span>
                </div>
                <BarraSegmentada valor="clientes" partes={sc.tramos.map((t) => ({ nombre: `Sin comprar ${t.tramo.toLowerCase()}`, i: t.i, clientes: t.clientes, color: COLOR_TRAMO[t.i] }))}
                  onElegir={(t) => verSinCompra(`Clientes ${t.nombre.toLowerCase()}`, { tramo: String(t.i) })} />
                <h3 className="sub-tit">Los que más compraban</h3>
                <BarrasH filas={sc.top.map((x) => ({ ...x, nombre: titulo(x.nombre) }))} valor="monto" etiqueta="nombre" formato={(n) => fmtBs(n, 0)}
                  onElegir={(x) => verCompras(x, 365)}
                  extra={(x) => <><em className="mora">Hace {fmtN(x.sinComprar)} días</em> · {pedidosTxt(x.compras)} · última {fechaTxtCorta(x.ultima)}</>} />
              </div>
            )}
          </Panel>
          <Panel clase="span-6" titulo="Productos sin ventas" sub={`Con stock y sin ventas en los últimos ${an?.diasSinCompra ?? 60} días · todas las sucursales`}
            accion={ps?.productos ? <button className="btn btn-txt btn-sm" onClick={() => verSinVenta('Productos sin ventas')}>Ver todo <Icono n="next" size={14} /></button> : null}>
            {!analisis ? esqAnalisis : !ps ? <SinDatos texto="No disponible" /> : !ps.productos ? <SinDatos texto="Todos los productos con stock tuvieron ventas" /> : (
              <div className={`credito ${cargandoAn ? 'recargando' : ''}`}>
                <div className="credito-total">
                  <b>{fmtN(ps.productos)} {ps.productos === 1 ? 'producto' : 'productos'}</b>
                  <span>{fmtN(ps.unidades)} unidades en stock · {fmtBs(ps.valor, 0)} a precio de venta</span>
                </div>
                <BarraSegmentada valor="productos" partes={ps.tramos.map((t) => ({ nombre: t.i === 3 ? t.tramo : `Última venta: ${t.tramo.toLowerCase()}`, i: t.i, productos: t.productos, color: COLOR_TRAMO[t.i] }))}
                  onElegir={(t) => verSinVenta(t.i === 3 ? 'Productos con stock que nunca se vendieron' : `Productos sin ventas · ${t.nombre.toLowerCase()}`, { tramo: String(t.i) })} />
                <h3 className="sub-tit">Por categoría · valor en stock</h3>
                <BarrasH filas={ps.categorias.map((c) => ({ ...c, orig: c.nombre, nombre: titulo(c.nombre) }))} valor="valor" etiqueta="nombre" formato={(n) => fmtBs(n, 0)}
                  onElegir={(c) => verSinVenta(`Productos sin ventas · ${c.nombre}`, { categoria: c.orig })}
                  extra={(c) => `${fmtN(c.productos)} ${c.productos === 1 ? 'producto' : 'productos'}`} />
              </div>
            )}
          </Panel>

          {/* ===== Fila 4: finanzas, productos, alertas ===== */}
          <Panel clase="span-4" titulo="Cuentas por cobrar" sub={vendedor ? `Clientes de ${titulo(nomVend || '')}` : 'Créditos de clientes pendientes'}
            accion={datos.cobrar?.documentos ? <button className="btn btn-txt btn-sm" onClick={() => verCobrar('Cuentas por cobrar')}>Ver todo <Icono n="next" size={14} /></button> : null}>
            {datos.cobrar ? (
              <div className="credito">
                <div className="credito-total"><b>{fmtBs(datos.cobrar.total, 0)}</b><span>{fmtN(datos.cobrar.documentos)} créditos · {fmtN(datos.cobrar.cuentas)} clientes</span></div>
                <button type="button" className="credito-vencido" disabled={!datos.cobrar.documentosVencidos} onClick={() => verCobrar('Créditos de clientes vencidos', { vencido: '1' })}><Icono n="alert" size={14} /> Vencido: <b>{fmtBs(datos.cobrar.vencido, 0)}</b> ({datos.cobrar.total ? Math.round((datos.cobrar.vencido / datos.cobrar.total) * 100) : 0}%)</button>
                <BarraSegmentada valor="saldo" formato={(n) => fmtBs(n, 0)} partes={datos.cobrar.tramos.map((t, i) => ({ nombre: t.tramo, i, saldo: t.saldo, color: ['var(--estado-ok)', 'var(--estado-aviso)', 'var(--estado-mal)', 'var(--mora-grave)'][i] }))}
                  onElegir={(t) => verCobrar(`Créditos de clientes ${TRAMO_TXT[t.i]}`, { tramo: String(t.i) })} />
                <h3 className="sub-tit">Mayores saldos</h3>
                <BarrasH filas={datos.cobrar.top.map((x) => ({ ...x, orig: x.nombre, nombre: titulo(x.nombre) }))} valor="saldo" etiqueta="nombre" formato={(n) => fmtBs(n, 0)}
                  onElegir={(x) => verCobrar(`Créditos de ${x.nombre}`, { nombre: x.orig })}
                  extra={(x) => (x.mora > 0 ? <em className="mora">{x.mora} días de mora</em> : 'Al día')} />
              </div>
            ) : <SinDatos texto="No disponible" />}
          </Panel>
          <Panel clase="span-4" titulo="Cuentas por pagar" sub="Créditos con proveedores"
            accion={datos.pagar?.documentos ? <button className="btn btn-txt btn-sm" onClick={() => verPagar('Cuentas por pagar')}>Ver todo <Icono n="next" size={14} /></button> : null}>
            {datos.pagar ? (
              <div className="credito">
                <div className="credito-total"><b>{fmtBs(datos.pagar.total, 0)}</b><span>{fmtN(datos.pagar.documentos)} créditos · {fmtN(datos.pagar.cuentas)} proveedores</span></div>
                <button type="button" className="credito-vencido" disabled={!datos.pagar.documentosVencidos} onClick={() => verPagar('Pagos a proveedores vencidos', { vencido: '1' })}><Icono n="alert" size={14} /> Vencido: <b>{fmtBs(datos.pagar.vencido, 0)}</b></button>
                <BarraSegmentada valor="saldo" formato={(n) => fmtBs(n, 0)} partes={datos.pagar.tramos.map((t, i) => ({ nombre: t.tramo, i, saldo: t.saldo, color: ['var(--estado-ok)', 'var(--estado-aviso)', 'var(--estado-mal)', 'var(--mora-grave)'][i] }))}
                  onElegir={(t) => verPagar(`Créditos con proveedores ${TRAMO_TXT[t.i]}`, { tramo: String(t.i) })} />
                <h3 className="sub-tit">Mayores saldos</h3>
                <BarrasH filas={datos.pagar.top.map((x) => ({ ...x, orig: x.nombre, nombre: titulo(x.nombre) }))} valor="saldo" etiqueta="nombre" formato={(n) => fmtBs(n, 0)}
                  onElegir={(x) => verPagar(`Créditos con ${x.nombre}`, { nombre: x.orig })}
                  extra={(x) => (x.mora > 0 ? <em className="mora">{x.mora} días de mora</em> : 'Al día')} />
              </div>
            ) : <SinDatos texto="No disponible" />}
          </Panel>
          <div className="span-4 pila-paneles">
            <Panel titulo="Alertas" sub="Requieren atención">
              <ul className="alertas">
                {alertas.map((a) => (
                  <li key={a.t} className={a.n > 0 ? a.nivel : 'ok'}>
                    <button type="button" disabled={!a.n} onClick={a.ver} title={a.n ? 'Ver detalle' : undefined}>
                      <span className="alerta-ic"><Icono n={a.n > 0 ? a.i : 'check'} size={16} /></span>
                      <span>{a.t}</span><b>{fmtN(a.n)}</b>
                    </button>
                  </li>
                ))}
              </ul>
            </Panel>
            <Panel titulo="Productos más vendidos" sub="En el día (todos los vendedores)">
              {datos.productos?.length ? (
                <BarrasH filas={datos.productos.map((p) => ({ ...p, nombre: titulo(p.nombre) }))} valor="total" etiqueta="nombre" formato={(n) => fmtBs(n, 0)}
                  extra={(p) => `${fmtN(p.cantidad)} unidades`} />
              ) : <SinDatos texto="Sin ventas de productos" />}
            </Panel>
          </div>
        </div>
      )}

      {detalle && <DetalleDashboard consulta={detalle} onCerrar={() => setDetalle(null)} />}
    </div>
  );
}
