// =====================================================================
//  Tec_AdministrarAsignacionesPedidos.vb -> Asignación de pedidos  (SP: MAM_Ventas)
//  Pestañas como el escritorio: Pendientes · Asignados · Entregados · Anulados
//  Cada pestaña tiene su casilla "Seleccionar" (columna Asignar de VB) y sus acciones.
//  Permisos del rol: Asignar = Insertar; Quitar / Entregar / Revertir = Modificar; Anular = Eliminar.
// =====================================================================
import { useCallback, useEffect, useState } from 'react';
import { createPortal } from 'react-dom';
import { Link, useLocation } from 'react-router-dom';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti } from '../../componentes/Controles';
import { EfectoConfirmar } from '../../componentes/Efecto';
import { useToast } from '../../componentes/Toast';
import { usePermisos } from '../../global/Sesion';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { columnasDe } from './Reportes';

const URL = '/api/ventas/asignaciones';
const BASE = '/ventas/asignacion-pedidos';
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

const TABS = [
  { k: 'pendientes', t: 'Pendientes', i: 'alert' },
  { k: 'asignados', t: 'Asignados', i: 'truck' },
  { k: 'entregados', t: 'Entregados', i: 'check' },
  { k: 'anulados', t: 'Anulados', i: 'x' },
];

const COLS = [
  { campo: 'Id', titulo: 'Nro Pedido' },
  { campo: 'NombreCliente', titulo: 'Cliente', ancho: 180, render: (f) => tit(f.NombreCliente) },
  { campo: 'FechaPedido', titulo: 'Fecha Pedido', tipo: 'fecha' },
  { campo: 'NombrePersonal', titulo: 'Vendedor', render: (f) => tit(f.NombrePersonal) },
  { campo: 'totalPedido', titulo: 'Total', tipo: 'bs', total: true },
];
const COLS_ENT = [...COLS.slice(0, 4), {
  campo: 'conciliacion', titulo: 'Conciliación',
  render: (f) => <span>{f.conciliacion}{Number(f.EstadoConciliacion) === 0 && <span className="chip-mora mora-alta chip-cerrada">Cerrada</span>}</span>,
}, COLS[4]];
const COLS_ANU = [...COLS.slice(0, 3), { campo: 'FechaAnulacion', titulo: 'Fecha Anulada', tipo: 'fecha' }, ...COLS.slice(3)];

// Confirmaciones del escritorio (Efecto tipo 3)
const CONFIRMA = {
  anular: { titulo: 'Confirmación de Anulación', texto: '¿Esta Seguro de Anular Todos Los Pedidos Seleccionados ?' },
  'revertir-entrega': { titulo: 'Confirmación de Reversión', texto: '¿Esta Seguro de Revertir El Estado Del Pedido ?' },
  'revertir-anulacion': { titulo: 'Confirmación de Reversión', texto: '¿Esta Seguro de Revertir Estado Del Pedido a Estado Pendiente?' },
};

export default function Tec_AsignacionPedidos() {
  const toast = useToast();
  const puede = usePermisos();
  const location = useLocation();
  const tab = TABS.find((t) => location.pathname.endsWith(`/${t.k}`))?.k || 'pendientes';
  const [combos, setCombos] = useState({ choferes: [], conciliaciones: [] });
  // cbPersonal (asignar) · cbPersonalAsignado · cbChofer + cbConciliacion: como VB, por defecto el primero
  const [sel, setSel] = useState({ choferAsignar: '', choferAsignados: '', choferEntregados: '', conciliacion: '' });
  const [datos, setDatos] = useState({});
  const [marcados, setMarcados] = useState({ pendientes: new Set(), asignados: new Set(), entregados: new Set(), anulados: new Set() });
  const [cargando, setCargando] = useState(false);
  const [ocupado, setOcupado] = useState(false);
  const [confirmar, setConfirmar] = useState(null); // { accion, cuerpo, tab }
  const [detalle, setDetalle] = useState(null);
  const [carga, setCarga] = useState(null);

  useEffect(() => {
    api.get(`${URL}/inicio`).then((r) => {
      setCombos(r.data);
      const ch = r.data.choferes[0]?.Id ?? '';
      setSel({ choferAsignar: ch, choferAsignados: ch, choferEntregados: ch, conciliacion: r.data.conciliaciones[0]?.id ?? '' });
    }).catch((e) => toast(e.message, 'error'));
  }, [toast]);

  const rutaDe = useCallback((t) => {
    if (t === 'pendientes' || t === 'anulados') return `${URL}/${t}`;
    if (t === 'asignados') return sel.choferAsignados ? `${URL}/asignados/${sel.choferAsignados}` : null;
    return sel.choferEntregados ? `${URL}/entregados/${sel.choferEntregados}?conciliacion=${encodeURIComponent(sel.conciliacion ?? '')}` : null;
  }, [sel]);

  const cargar = useCallback(async (t = tab) => {
    const ruta = rutaDe(t);
    setMarcados((m) => ({ ...m, [t]: new Set() }));
    if (!ruta) { setDatos((d) => ({ ...d, [t]: [] })); return; }
    setCargando(true);
    try { const r = await api.get(ruta); setDatos((d) => ({ ...d, [t]: r.data })); } catch (e) { toast(e.message, 'error'); } finally { setCargando(false); }
  }, [tab, rutaDe, toast]);
  useEffect(() => { cargar(tab); }, [tab, sel.choferAsignados, sel.choferEntregados, sel.conciliacion]); // eslint-disable-line react-hooks/exhaustive-deps

  const filas = datos[tab] || [];
  const ids = marcados[tab];
  const elegidas = filas.filter((f) => ids.has(f.Id));
  const totalSel = elegidas.reduce((s, f) => s + Number(f.totalPedido || 0), 0);

  // ---------- acciones ----------
  const ejecutar = async (accion, cuerpo, refrescar) => {
    setOcupado(true);
    try {
      const r = await api.post(`${URL}/${accion}`, cuerpo);
      toast(r.mensaje, 'ok');
      setDatos({});
      refrescar.forEach((t) => cargar(t));
    } catch (e) { toast(e.message, 'error'); } finally { setOcupado(false); setConfirmar(null); }
  };
  const pedir = (accion, cuerpo, refrescar) => {
    if (!ids.size) { toast('No Existe Ningun Pedido Seleccionado', 'error'); return; }
    if (accion === 'revertir-entrega' && elegidas.some((f) => Number(f.EstadoConciliacion) === 0)) {
      toast('No Es Posible Revertir Estado Por Que Existen Pedidos Que Pertenece A una Conciliacion Cerrada', 'error'); return;
    }
    const c = { accion, cuerpo: { ...cuerpo, ids: [...ids] }, refrescar };
    if (CONFIRMA[accion]) setConfirmar(c); else ejecutar(c.accion, c.cuerpo, c.refrescar);
  };
  const asignar = () => {
    if (!ids.size) { toast('No Existe Ningun Pedido Seleccionado', 'error'); return; }
    if (!sel.choferAsignar) { toast('Seleccione un Chofer', 'error'); return; }
    pedir('asignar', { chofer: sel.choferAsignar }, ['pendientes']);
  };
  const verCarga = async (de) => {
    try {
      const r = await api.get(`${URL}/carga/${sel.choferAsignados}?de=${de}`);
      if (!r.data.filas.length) { toast('No Existen Datos Para Mostrar', 'error'); return; }
      setCarga({ ...r.data, de });
    } catch (e) { toast(e.message, 'error'); }
  };

  const cmbChofer = (k) => (
    <div className="filtro filtro-ancho"><span>Chofer</span>
      <ComboMulti dt={combos.choferes} valor={sel[k]} onChange={(x) => setSel((s) => ({ ...s, [k]: x }))} id="Id" nameId="Cód." descripcionId="Nombre" descripcion="Chofer" />
    </div>
  );
  const resumenSel = ids.size > 0 && <span className="asig-sel" aria-live="polite"><b>{ids.size}</b> seleccionado{ids.size === 1 ? '' : 's'} · Bs {bs(totalSel)}</span>;
  const btn = (texto, icono, onClick, clase = 'btn-sec', deshab = false) => (
    <button className={`btn ${clase}`} onClick={onClick} disabled={ocupado || deshab}><Icono n={icono} size={16} /> {texto}</button>
  );

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo">
          <span className="modelo-icono"><Icono n="truck" size={22} /></span>
          <div><h1>Asignación de pedidos</h1><p>Administrar Asignaciones Pedidos</p></div>
        </div>
        <div className="barra"><button className="btn-icono" title="Actualizar" aria-label="Actualizar" onClick={() => cargar()}><Icono n="refresh" size={18} /></button></div>
      </header>
      <nav className="tabs-cxc" aria-label="Secciones">
        {TABS.map((t) => (
          <Link key={t.k} to={t.k === 'pendientes' ? BASE : `${BASE}/${t.k}`} replace className={tab === t.k ? 'activa' : ''} aria-current={tab === t.k ? 'page' : undefined}>
            <Icono n={t.i} size={16} /> <span>{t.t}</span>
          </Link>
        ))}
      </nav>
      <div className="modelo-cuerpo">
        <div className="filtros-lista filtros-reporte asig-barra">
          {tab === 'pendientes' && (
            <>
              {puede.insertar && cmbChofer('choferAsignar')}
              <div className="asig-acciones">
                {resumenSel}
                {puede.insertar && btn('Asignar', 'truck', asignar, 'btn-pri')}
                {puede.eliminar && btn('Anular', 'x', () => pedir('anular', { desde: 'pendientes' }, ['pendientes', 'anulados']), 'btn-peligro-sec')}
              </div>
            </>
          )}
          {tab === 'asignados' && (
            <>
              {cmbChofer('choferAsignados')}
              <div className="asig-acciones">
                {resumenSel}
                {puede.modificar && btn('Entregar', 'check', () => pedir('entregar', { chofer: sel.choferAsignados }, ['asignados']), 'btn-pri')}
                {puede.modificar && btn('Quitar asignación', 'undo', () => pedir('quitar', { chofer: sel.choferAsignados }, ['asignados']))}
                {puede.eliminar && btn('Anular', 'x', () => pedir('anular', { desde: 'asignados', chofer: sel.choferAsignados }, ['asignados']), 'btn-peligro-sec')}
                {btn('Lista de productos', 'box', () => verCarga('productos'), 'btn-sec', !sel.choferAsignados || !filas.length)}
                {btn('Lista de clientes', 'store', () => verCarga('clientes'), 'btn-sec', !sel.choferAsignados || !filas.length)}
              </div>
            </>
          )}
          {tab === 'entregados' && (
            <>
              {cmbChofer('choferEntregados')}
              <div className="filtro filtro-ancho"><span>Conciliación</span>
                <ComboMulti dt={combos.conciliaciones} valor={sel.conciliacion} onChange={(x) => setSel((s) => ({ ...s, conciliacion: x }))} id="id" nameId="Cód." descripcionId="conciliacion" descripcion="Conciliación" />
              </div>
              <div className="asig-acciones">
                {resumenSel}
                {puede.modificar && btn('Revertir a asignado', 'undo', () => pedir('revertir-entrega', { chofer: sel.choferEntregados, conciliacion: sel.conciliacion }, ['entregados']))}
              </div>
            </>
          )}
          {tab === 'anulados' && (
            <div className="asig-acciones">
              {resumenSel}
              {puede.modificar && btn('Revertir a pendiente', 'undo', () => pedir('revertir-anulacion', {}, ['anulados']))}
            </div>
          )}
        </div>

        <TablaReporte
          titulo={{ pendientes: 'Pedidos pendientes de asignación', asignados: 'Pedidos asignados', entregados: 'Pedidos entregados', anulados: 'Pedidos anulados' }[tab]}
          subtitulo={tab === 'asignados' || tab === 'entregados' ? `Chofer: ${tit(combos.choferes.find((c) => String(c.Id) === String(sel[tab === 'asignados' ? 'choferAsignados' : 'choferEntregados']))?.Nombre || '')}` : undefined}
          columnas={tab === 'entregados' ? COLS_ENT : tab === 'anulados' ? COLS_ANU : COLS}
          filas={filas} cargando={cargando} vacio="No hay pedidos"
          seleccion={{ ids, onCambio: (n) => setMarcados((m) => ({ ...m, [tab]: n })) }}
          onFila={(f) => setMarcados((m) => { const n = new Set(m[tab]); if (n.has(f.Id)) n.delete(f.Id); else n.add(f.Id); return { ...m, [tab]: n }; })}
          accion={(f) => <button className="btn btn-txt btn-sm" onClick={() => setDetalle(f)} aria-label={`Detalle del pedido ${f.Id}`}><Icono n="eye" size={15} /> Detalle</button>} />
      </div>

      <EfectoConfirmar abierto={!!confirmar} titulo={confirmar ? CONFIRMA[confirmar.accion].titulo : ''} descripcion={confirmar ? CONFIRMA[confirmar.accion].texto : ''}
        onAceptar={() => !ocupado && ejecutar(confirmar.accion, confirmar.cuerpo, confirmar.refrescar)} onCancelar={() => setConfirmar(null)} />
      {detalle && <DetallePedido fila={detalle} onCerrar={() => setDetalle(null)} />}
      {carga && <ListaCarga datos={carga} onCerrar={() => setCarga(null)} />}
    </section>
  );
}

// Tec_VisualizarDetallePedido (Efecto 17): "Pedido # N  Cliente = X"
function DetallePedido({ fila, onCerrar }) {
  const toast = useToast();
  const [det, setDet] = useState(null);
  useEffect(() => {
    api.get(`${URL}/pedido/${fila.Id}/detalle`).then((r) => setDet(r.data)).catch((e) => { toast(e.message, 'error'); onCerrar(); });
  }, [fila.Id]); // eslint-disable-line react-hooks/exhaustive-deps
  useEffect(() => { const k = (e) => e.key === 'Escape' && onCerrar(); window.addEventListener('keydown', k); return () => window.removeEventListener('keydown', k); }, [onCerrar]);
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-lg dialogo-asig" role="dialog" aria-modal="true" aria-label={`Pedido ${fila.Id}`} onMouseDown={(e) => e.stopPropagation()}>
        <div className="modal-cab"><h3>Pedido # {fila.Id} · {tit(fila.NombreCliente)}</h3><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        {!det ? <div className="cargando-bloque"><span className="spinner" /> Cargando detalle...</div> : (
          <TablaReporte titulo={`Pedido ${fila.Id}`} subtitulo={tit(fila.NombreCliente)} filas={det} claveFila="__no" vacio="Sin detalle"
            columnas={[
              { campo: 'ProductoId', titulo: 'Cod Producto' },
              { campo: 'Producto', titulo: 'Producto', ancho: 180 },
              { campo: 'Cantidad', titulo: 'Cantidad', tipo: 'num', total: true },
              { campo: 'Precio', titulo: 'Precio', tipo: 'bs' },
              { campo: 'SubTotal', titulo: 'SubTotal', tipo: 'bs', total: true },
              { campo: 'MontoDescuento', titulo: 'M.Descuento', tipo: 'bs', total: true },
              { campo: 'Total', titulo: 'Total', tipo: 'bs', total: true },
            ]} />
        )}
        <div className="modal-acciones"><button className="btn btn-sec" onClick={onCerrar}>Cerrar</button></div>
      </div>
    </div>,
    document.body,
  );
}

// Reporte_ProductosAsignados / clientes asignados al chofer (columnas tal como las devuelve el SP)
function ListaCarga({ datos, onCerrar }) {
  useEffect(() => { const k = (e) => e.key === 'Escape' && onCerrar(); window.addEventListener('keydown', k); return () => window.removeEventListener('keydown', k); }, [onCerrar]);
  const titulo = datos.de === 'clientes' ? 'Clientes asignados' : 'Productos asignados';
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-lg dialogo-asig" role="dialog" aria-modal="true" aria-label={titulo} onMouseDown={(e) => e.stopPropagation()}>
        <div className="modal-cab">
          <div><h3>{titulo}</h3><small className="sub-rango">Chofer: {tit(datos.chofer)}{datos.empresa.nombre ? ` · ${datos.empresa.nombre}` : ''}</small></div>
          <button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button>
        </div>
        <TablaReporte titulo={`${titulo.toUpperCase()} - ${datos.chofer}`} subtitulo={[datos.empresa.nombre, datos.empresa.direccion].filter(Boolean).join(' · ')}
          columnas={columnasDe(datos.filas)} filas={datos.filas} claveFila="__no" vacio="No Existen Datos Para Mostrar" />
        <div className="modal-acciones"><button className="btn btn-sec" onClick={onCerrar}>Cerrar</button></div>
      </div>
    </div>,
    document.body,
  );
}
