// =====================================================================
//  Panel de control (pantalla principal de Santit)
//  Datos de /api/dashboard (solo agregados, calculados con los SP del escritorio)
// =====================================================================
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSesion } from '../global/Sesion';
import { titulo } from '../global/programas';
import { api } from '../api/http';
import { ComboMulti } from '../componentes/Controles';
import { LineaHoras, BarrasDias, BarrasH, BarraSegmentada, fmtBs, fmtN } from '../componentes/Graficos';
import Icono from '../componentes/Icono';
import DetalleDashboard from '../componentes/DetalleDashboard';
import PedidosVendedor from '../componentes/PedidosVendedor';

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
  const { sesion, tienePrograma } = useSesion();
  const navegar = useNavigate();
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
  const pedidosTxt = (n) => `${fmtN(n)} ${n === 1 ? 'pedido' : 'pedidos'}`;
  const TRAMO_TXT = ['al día', 'con 1 a 30 días de mora', 'con 31 a 60 días de mora', 'con más de 60 días de mora'];

  const alertas = datos ? [
    { n: datos.alertas.pendientesAsignar, t: 'Pedidos sin asignar a chofer', i: 'truck', nivel: 'aviso', ver: () => setDetalle({ tipo: 'asignar', titulo: 'Pedidos sin asignar a chofer', sub: 'Pendientes de asignación',
      accion: tienePrograma('btnAsignacionPedidos') ? { texto: 'Ir a Asignación de pedidos', fn: () => navegar('/ventas/asignacion-pedidos') } : undefined }) },
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
            <button className="btn-icono" onClick={cargar} title="Actualizar" aria-label="Actualizar">{cargando && datos ? <span className="spinner" /> : <Icono n="refresh" size={18} />}</button>
            {actualizado && <small>Actualizado {actualizado.toLocaleTimeString('es-BO', { hour: '2-digit', minute: '2-digit' })}</small>}
          </div>
        </div>
      </header>

      {error && <div className="alerta"><Icono n="alert" size={16} /> {error}</div>}
      {datos?.errores?.length > 0 && <div className="alerta aviso"><Icono n="alert" size={16} /> No se pudo leer: {datos.errores.join(', ')}. El resto del panel está actualizado.</div>}

      {!datos ? <Esqueleto /> : (
        <div className={`dash-grid ${cargando ? 'recargando' : ''}`}>
          {/* ===== KPIs ===== */}
          <Kpi titulo="Pedidos del día" icono="cart" color="var(--serie-1)" onClick={k?.pedidos || k?.anulados ? () => verPedidos('Pedidos del día') : undefined} valor={k ? fmtN(k.pedidos) : '—'}
            pie={k ? `${fmtN(k.anulados)} ${k.anulados === 1 ? 'anulado' : 'anulados'} · promedio ${fmtN(Math.round(k.promPedidos))}/día` : ''}>
            {k && <><Variacion actual={k.pedidos} base={k.pedidosAyer} etiqueta="vs ayer" /><Variacion actual={k.pedidos} base={k.promPedidos} etiqueta="vs prom." /></>}
          </Kpi>
          <Kpi titulo="Monto vendido" icono="dollar" color="var(--estado-ok)" onClick={k?.pedidos ? () => verPedidos('Monto vendido', { validos: '1' }) : undefined} valor={k ? fmtBs(k.monto) : '—'}
            pie={k ? `Promedio diario ${fmtBs(k.promMonto, 0)}` : ''}>
            {k && <><Variacion actual={k.monto} base={k.montoAyer} etiqueta="vs ayer" /><Variacion actual={k.monto} base={k.promMonto} etiqueta="vs prom." /></>}
          </Kpi>
          <Kpi titulo="Ticket promedio" icono="tag" color="var(--serie-7)" onClick={k?.pedidos ? () => verPedidos('Pedidos válidos del día', { validos: '1' }) : undefined} valor={k ? fmtBs(k.ticket) : '—'}
            pie={k ? `${fmtN(k.clientes)} ${k.clientes === 1 ? 'cliente atendido' : 'clientes atendidos'}` : ''}>
            {k && k.promPedidos > 0 && <Variacion actual={k.ticket} base={k.promMonto / k.promPedidos} etiqueta="vs prom." />}
          </Kpi>
          <Kpi titulo="Vendedor del día" icono="users" color="var(--serie-2)"
            onClick={v?.masPedidos ? () => verPedidos(`Pedidos de ${titulo(v.masPedidos.nombre)}`, { vendedor: v.masPedidos.id, validos: '1' }, v.masPedidos.id !== vendedor ? filtrarVend(v.masPedidos.id) : undefined) : undefined}
            valor={v?.masPedidos ? <span className="kpi-nombre">{titulo(v.masPedidos.nombre)}</span> : '—'}
            pie={v?.masPedidos ? `${fmtN(v.masPedidos.pedidos)} ${v.masPedidos.pedidos === 1 ? 'pedido' : 'pedidos'} · ${fmtBs(v.masPedidos.monto)}` : 'Sin pedidos en el día'}>
            {v?.ranking?.[0] && v.ranking[0].id !== v.masPedidos?.id && <span className="chip-var neutro">Mayor monto: {titulo(v.ranking[0].nombre)}</span>}
            {v?.masPedidos && <span className="chip-var trofeo">★ Más pedidos</span>}
          </Kpi>

          {/* ===== Fila 2 ===== */}
          <Panel clase="span-8" titulo="Ventas por hora" sub={`${esHoy ? 'Hoy' : 'Día seleccionado'} vs promedio de los últimos 30 días`}
            accion={<div className="leyenda"><span><i className="dot s1" /> {esHoy ? 'Hoy' : 'Día'}</span><span><i className="dot prom" /> Promedio 30 días</span></div>}>
            {v ? <LineaHoras datos={v.porHora} onElegir={(hh) => verPedidos(`Pedidos de ${String(hh).padStart(2, '0')}:00 a ${String(hh).padStart(2, '0')}:59`, { hora: String(hh), validos: '1' })} /> : <SinDatos />}
          </Panel>
          <Panel clase="span-4" titulo="Ranking de vendedores" sub="Monto vendido en el día">
            {v?.ranking?.length ? (
              <BarrasH filas={v.ranking.map((r) => ({ ...r, nombre: titulo(r.nombre) }))} valor="monto" etiqueta="nombre"
                destacar={(r) => vendedor && r.id === vendedor}
                onElegir={(r) => verPedidos(`Pedidos de ${r.nombre}`, { vendedor: r.id, validos: '1' }, r.id !== vendedor ? filtrarVend(r.id) : undefined)}
                extra={(r) => <>{fmtN(r.pedidos)} pedidos{r.id === v.masPedidos?.id ? <em className="estrella"> ★ más pedidos</em> : ''}</>} />
            ) : <SinDatos texto="Sin ventas en el día" />}
          </Panel>

          {/* ===== Fila 3 ===== */}
          <Panel clase="span-8" titulo="Rendimiento por día" sub="Monto vendido, últimos 30 días · toque una barra para ver ese día">
            {v ? <BarrasDias datos={v.tendencia} seleccion={fecha} onElegir={setFecha} /> : <SinDatos />}
          </Panel>
          <Panel clase="span-4" titulo="Pedidos del día" sub="Por estado y forma de pago">
            {v?.estados?.length ? (
              <div className="pila">
                <BarraSegmentada partes={v.estados.map((e) => ({ ...e, orig: e.nombre, nombre: titulo(e.nombre), color: COLOR_ESTADO(e.nombre) }))}
                  onElegir={(e) => verPedidos(`Pedidos en estado ${e.nombre.toLowerCase()}`, { estado: e.orig })} />
                <BarraSegmentada valor="monto" formato={(n) => fmtBs(n, 0)} partes={v.tipos.map((e) => ({ ...e, orig: e.nombre, nombre: titulo(e.nombre), color: COLOR_TIPO(e.nombre) }))}
                  onElegir={(e) => verPedidos(`Pedidos al ${e.nombre.toLowerCase()}`, { tipo: e.orig, validos: '1' })} />
              </div>
            ) : <SinDatos texto="Sin pedidos en el día" />}
          </Panel>

          {/* ===== Pedidos por vendedor y mapa de donde se registraron ===== */}
          <PedidosVendedor fecha={fecha} vendedor={vendedor} recarga={actualizado?.getTime()} />

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
                  pie={(x) => `${pedidosTxt(x.pedidos)} · ${pct(x.monto, m.montoTodos)} de las ventas${m.masPedidos && m.masPedidos.id === x.id ? ' · ★ más pedidos' : ''}`}
                  onElegir={(x) => verPedidos(`Ventas de ${titulo(x.nombre)} en ${mesTxt}`, { mes: '1', vendedor: x.id, validos: '1' }, x.id !== vendedor ? filtrarVend(x.id) : undefined)} />
              </div>
            ) : <SinDatos />}
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
