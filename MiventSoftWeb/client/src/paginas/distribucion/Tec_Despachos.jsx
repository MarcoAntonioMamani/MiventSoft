// =====================================================================
//  Tec_Despachos.vb + Tec_DespachoDetalle.vb -> Despacho de productos  (SP: MAM_DespachoProductos)
//  Salida de productos al distribuidor. "Cargar productos de ventas" = despacho automatico.
//  No se edita ni elimina si la conciliacion esta cerrada; el automatico solo se elimina.
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import { createPortal } from 'react-dom';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, fechaCorta } from '../../componentes/Controles';
import { EfectoAyuda } from '../../componentes/Efecto';
import TablaReporte from '../../componentes/TablaReporte';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { CantidadCajas, NumeroCelda, errorDetalle } from '../ventas/Tec_Ventas';
import { useListaTeclado } from '../../componentes/useListaTeclado';
import NotaImpresa, { bsNota } from '../../componentes/NotaImpresa';
import { palabrasDe, coincide } from '../../global/buscar';

const URL = '/api/distribucion/despachos';
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const VACIO = { Id: '', PersonalId: 0, Personal: '', Fecha: '', Detalle: '' };
const MSJ_CERRADA = 'La Conciliacion Ya Esta Cerrada No Es Posible Editar Este Despacho. Primero Debe Revertir El Estado De La Conciliacion';
let _k = 0;

export default function Tec_Despachos() {
  const toast = useToast();
  const [filtro, setFiltro] = useState({ desde: hoy(), hasta: hoy() });
  const [form, setForm] = useState(VACIO);
  const [detalle, setDetalle] = useState([]); // { key, Id?, nuevo, eliminado, ProductoId, NombreProducto, Cantidad, conversion, Stock }
  const [automatico, setAutomatico] = useState(false);
  const [errores, setErrores] = useState({});
  const [personal, setPersonal] = useState(null);
  const [selector, setSelector] = useState(false);
  const [cargandoDet, setCargandoDet] = useState(false);
  const [nota, setNota] = useState(null);
  const vigentes = detalle.filter((f) => !f.eliminado);
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));

  const cargarDetalle = (id) => {
    setCargandoDet(true);
    api.get(`${URL}/${id}/detalle`).then((r) => setDetalle(r.data.map((f) => ({ ...f, key: ++_k })))).catch((e) => toast(e.message, 'error')).finally(() => setCargandoDet(false));
  };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(`${URL}?${new URLSearchParams(filtro)}`)).data.map((f) => ({
      ...f, FechaTxt: fechaCorta(`${f.Fecha}T00:00:00Z`), EstadoTxt: f.EstadoConciliacion === 0 ? 'CERRADA' : 'ABIERTA', Tipo: f.DespachoAutomatico === 1 ? 'AUTOMÁTICO' : 'MANUAL',
    })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'Cod. Despacho', 80, '', 'num'),
      new Celda('NombrePersonal', true, 'Nombre Personal', 220),
      new Celda('FechaTxt', true, 'Fecha Despacho', 110),
      new Celda('Detalle', true, 'Detalle', 200),
      new Celda('Tipo', true, 'Tipo', 100),
      new Celda('ConciliacionId', true, 'Conciliación', 90),
      new Celda('EstadoTxt', true, 'Estado Conciliación', 120, 'ABIERTA', 'estado'),
    ],
    _PMOLimpiar: () => { setForm({ ...VACIO, Fecha: hoy() }); setDetalle([]); setAutomatico(false); },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, PersonalId: f.PersonalId, Personal: f.NombrePersonal ?? '', Fecha: f.Fecha ?? '', Detalle: f.Detalle ?? '' });
      setAutomatico(f.DespachoAutomatico === 1);
      cargarDetalle(f.Id);
    },
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!(form.PersonalId > 0)) e.Personal = 'Seleccione Personal';
      if (!form.Fecha) e.Fecha = 'Seleccione Fecha';
      if (!vigentes.length) e.detalle = 'Por Favor Inserte un Detalle';
      setErrores(e);
      if (Object.keys(e).length) toast(`Los Siguientes Campos Son Requeridos: ${Object.values(e).join(', ')}`, 'error');
      const ed = errorDetalle(vigentes, errFila, (f) => f.NombreProducto);
      if (ed) { setErrores({ ...e, detalle: ed }); toast(ed, 'error', 6000); return false; }
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: async () => {
      const r = await api.post(URL, {
        PersonalId: form.PersonalId, Fecha: form.Fecha, Detalle: form.Detalle, automatico,
        detalle: detalle.filter((f) => !(f.nuevo && f.eliminado) || automatico).map((f) => ({ ProductoId: f.ProductoId, Cantidad: f.Cantidad, eliminado: !!f.eliminado })),
      });
      verNota(r.id, { form: { ...form }, grabada: 'nueva' }); // como la nota de venta: se ofrece para imprimir
      return r;
    },
    _PMOModificarRegistro: async (f) => {
      const r = await api.put(`${URL}/${form.Id}`, {
        fechaOriginal: f?.Fecha ?? m.filaActual?.Fecha, PersonalId: form.PersonalId, Fecha: form.Fecha, Detalle: form.Detalle,
        detalle: detalle.filter((x) => !(x.nuevo && x.eliminado)).map((x) => (x.nuevo ? { nuevo: true, ProductoId: x.ProductoId, Cantidad: x.Cantidad } : { Id: x.Id, Cantidad: x.Cantidad, eliminado: !!x.eliminado })),
      });
      verNota(form.Id, { form: { ...form }, fila: { ...(m.filaActual || {}), ...form, NombrePersonal: form.Personal }, grabada: 'modificada' });
      return r;
    },
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}?fecha=${f.Fecha}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Despacho del Personal ${tit(f.NombrePersonal)} ?`,
    _PMOBloqueoModificar: (f) => (f?.EstadoConciliacion === 0 ? MSJ_CERRADA : f?.DespachoAutomatico === 1 ? 'Debe Eliminarse este despacho ya que es un Despacho Automatico' : null),
    _PMOBloqueoEliminar: (f) => (f?.EstadoConciliacion === 0 ? MSJ_CERRADA : null),
  });
  useEffect(() => { m._PMFiltrar(); }, [filtro.desde, filtro.hasta]); // eslint-disable-line react-hooks/exhaustive-deps
  const ro = !m.habilitado;

  const abrirPersonal = async () => {
    try { const r = await api.get(`${URL}/personal`); setPersonal(r.data); } catch (e) { toast(e.message, 'error'); }
  };
  // btnAgregarGastos: productos de las ventas asignadas al personal
  const cargarAutomatico = async () => {
    if (!(form.PersonalId > 0)) { toast('Seleccione un Personal'.toUpperCase(), 'error'); return; }
    try {
      const r = await api.get(`${URL}/automatico/${form.PersonalId}`);
      setDetalle(r.data.map((f) => ({ ...f, key: ++_k, nuevo: true })));
      setAutomatico(true);
      quitarError('detalle');
    } catch (e) { toast(e.message, 'error'); }
  };
  // el valor se aplica en cada tecla aunque sea invalido: la celda lo marca (errFila) y Grabar lo bloquea
  const cambiarFila = (key, txt) => setDetalle((d) => d.map((f) => {
    if (f.key !== key) return f;
    const c = num(txt);
    return { ...f, Cantidad: c, CantidadCaja: c / (num(f.conversion) || 1), _editado: true };
  }));
  const errFila = (f) => {
    if (!f._editado) return {};
    const c = num(f.Cantidad);
    if (!(c > 0)) return { Cantidad: 'La Cantidad debe ser Mayor a 0' };
    if (c > num(f.Stock) + 1e-9) return { Cantidad: `La Cantidad = ${n2(c)} es mayor al Stock del Producto = ${n2(f.Stock)}` };
    return {};
  };
  const quitarFila = (key) => setDetalle((d) => d.map((f) => (f.key === key ? { ...f, eliminado: true } : f)));
  const enDetalle = useMemo(() => new Set(vigentes.map((f) => Number(f.ProductoId))), [vigentes]);

  // ImprimirNotaSalida: desde el registro, desde la lista o al grabar (como la nota de venta)
  const verNota = async (id, extra = {}) => {
    if (!id) return;
    try { const r = await api.get(`${URL}/${id}/reporte`); if (!r.data.filas.length) { toast('No Existen Datos Para Mostrar', 'error'); return; } setNota({ ...r.data, ...extra }); } catch (e) { toast(e.message, 'error'); }
  };
  const imprimir = () => m.filaActual && verNota(m.filaActual.Id, { fila: m.filaActual });
  const totalU = vigentes.reduce((s, f) => s + num(f.Cantidad), 0);

  return (
    <>
    <ModeloF1 m={m} titulo="Despachos" subtitulo="Despacho De Productos" icono="truck"
      accionesRegistro={<button className="btn btn-sec" onClick={imprimir} disabled={!m.filaActual || m.habilitado}><Icono n="printer" size={16} /> <span className="txt-btn">Nota de despacho</span></button>}
      accionesFila={(f) => [{ texto: 'Nota de despacho', icono: 'printer', onClick: () => verNota(f.Id, { fila: f }) }]}
      extraBuscador={(
        <div className="filtros-lista">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={filtro.desde} max={filtro.hasta} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={filtro.hasta} min={filtro.desde} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, hasta: e.target.value }))} /></label>
        </div>
      )}>
      <div className="venta-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del despacho {automatico && <span className="chip-tipo directa">Automático</span>}</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Fecha salida" error={errores.Fecha} requerido>
              <input type="date" className="control" value={form.Fecha} onChange={(e) => { quitarError('Fecha'); setForm((x) => ({ ...x, Fecha: e.target.value })); }} readOnly={ro} disabled={ro} />
            </Campo>
            <Campo etiqueta="Personal" ancho={2} error={errores.Personal} requerido>
              <div className="control-con-btn">
                <input className={`control ${!ro ? 'control-elegir' : ''}`} value={tit(form.Personal)} readOnly placeholder="Toque para buscar el personal..." onClick={() => !ro && !automatico && abrirPersonal()} />
                {!ro && !automatico && <button type="button" className="btn btn-sec" onClick={abrirPersonal}><Icono n="search" size={15} /> Buscar</button>}
              </div>
            </Campo>
            <Campo etiqueta="Detalle" ancho={2}><input className="control" value={form.Detalle} onChange={(e) => setForm((x) => ({ ...x, Detalle: e.target.value }))} readOnly={ro} maxLength={500} /></Campo>
          </div>
        </div>

        <div className="tarjeta venta-detalle">
          <div className="detalle-cab-venta">
            <h2 className="tarjeta-tit">Productos <span className="contador">{vigentes.length}</span></h2>
            {!ro && (
              <div className="desp-botones">
                {m.modo === 'nuevo' && !automatico && <button className="btn btn-sec" onClick={cargarAutomatico}><Icono n="cart" size={16} /> Cargar productos de ventas</button>}
                <button className="btn btn-pri" onClick={() => setSelector(true)}><Icono n="plus" size={16} /> Agregar producto</button>
              </div>
            )}
          </div>
          {errores.detalle && <p className="campo-error" role="alert">{errores.detalle}</p>}
          {cargandoDet ? <div className="cargando-bloque"><span className="spinner" /> Cargando detalle...</div> : (
            <div className="tabla-scroll">
              <table className="grid grid-tarjetas tabla-detalle">
                <thead><tr><th>Producto</th><th className="num">Cajas</th><th className="num">Unidades</th>{!ro && <th className="num">Stock</th>}{!ro && <th aria-label="Quitar" />}</tr></thead>
                <tbody>
                  {vigentes.map((f) => (
                    <tr key={f.key} className={f.nuevo ? 'fila-nueva' : ''}>
                      <td data-label="Producto" className="det-producto"><b>{tit(f.NombreProducto)}</b><small>Cód. {f.ProductoId} · {n2(f.conversion)} u/caja</small></td>
                      <td data-label="Cajas" className="num">{ro ? n2(f.CantidadCaja) : <NumeroCelda valor={num(f.CantidadCaja)} onCambio={(x) => cambiarFila(f.key, num(x) * (num(f.conversion) || 1))} etiqueta={`Cajas de ${f.NombreProducto}`} error={errFila(f).Cantidad} />}</td>
                      <td data-label="Unidades" className="num">{ro ? n2(f.Cantidad) : <NumeroCelda valor={num(f.Cantidad)} onCambio={(x) => cambiarFila(f.key, x)} etiqueta={`Unidades de ${f.NombreProducto}`} error={errFila(f).Cantidad} />}</td>
                      {!ro && <td data-label="Stock" className="num">{n2(f.Stock)}</td>}
                      {!ro && <td className="col-acc"><button className="btn-icono peligro" onClick={() => quitarFila(f.key)} aria-label={`Quitar ${f.NombreProducto}`} title="Quitar"><Icono n="trash" size={16} /></button></td>}
                    </tr>
                  ))}
                  {!vigentes.length && <tr><td colSpan={5} className="vacio">{ro ? 'Sin productos' : 'Agregue productos o cárguelos de las ventas del personal'}</td></tr>}
                </tbody>
                {vigentes.length > 0 && <tfoot><tr className="tr-total"><td className="tf-tit"><b>TOTAL ({vigentes.length} {vigentes.length === 1 ? 'producto' : 'productos'})</b></td><td className="num" data-label="Total cajas"><b>{n2(vigentes.reduce((s, f) => s + num(f.CantidadCaja), 0))}</b></td><td className="num" data-label="Total unidades"><b>{n2(totalU)}</b></td>{!ro && <td />}{!ro && <td />}</tr></tfoot>}
              </table>
            </div>
          )}
        </div>
      </div>

      <EfectoAyuda abierto={!!personal} titulo="SELECCIONE PERSONAL" dt={personal || []}
        columnas={[new Celda('Nombre', true, 'NOMBRE', 300), new Celda('Direccion', true, 'DIRECCION', 180), new Celda('Telefono01', true, 'TELEFONO', 140)]}
        onCancelar={() => setPersonal(null)} onSeleccionar={(r) => { quitarError('Personal'); setForm((x) => ({ ...x, PersonalId: r.Id, Personal: r.Nombre })); setPersonal(null); }} />
      {selector && <SelectorProductos enDetalle={enDetalle} onCerrar={() => setSelector(false)}
        onAgregar={(p, cant) => { quitarError('detalle'); setDetalle((d) => [...d, { key: ++_k, nuevo: true, ProductoId: p.Id, NombreProducto: p.NombreProducto, Cantidad: cant, CantidadCaja: cant / (num(p.conversion) || 1), conversion: p.conversion, Stock: p.stock }]); }} />}
    </ModeloF1>
    {/* fuera del registro: tambien se abre desde la lista y despues de grabar */}
    {nota && <NotaDespacho datos={nota} onCerrar={() => setNota(null)} />}
    </>
  );
}

// Tec_DespachoDetalle: buscar por varias palabras (codigo + nombre + descripcion + categoria)
export function SelectorProductos({ enDetalle, onAgregar, onCerrar, url = `${URL}/productos`, sinLimite = false }) {
  const toast = useToast();
  const [productos, setProductos] = useState(null);
  const [q, setQ] = useState('');
  const [elegido, setElegido] = useState(null);
  const [agregados, setAgregados] = useState(0); // como el buscador de Pedido: cuantos se agregaron en esta ventana
  useEffect(() => { api.get(url).then((r) => setProductos(r.data)).catch((e) => { toast(e.message, 'error'); onCerrar(); }); }, []); // eslint-disable-line react-hooks/exhaustive-deps
  const filtrados = useMemo(() => {
    const w = palabrasDe(q);
    return (productos || []).filter((p) => !enDetalle.has(Number(p.Id)))
      .filter((p) => coincide(`${p.CodigoExterno ?? ''} ${p.NombreProducto ?? ''} ${p.DescripcionProducto ?? ''} ${p.NombreCategoria ?? ''}`, w));
  }, [productos, q, enDetalle]);
  const elegir = (p) => (sinLimite || num(p.stock) > 0 ? setElegido(p) : toast('El Producto no Cuenta con Stock Disponible', 'error'));
  const nav = useListaTeclado(filtrados, elegir, { onCerrar, pausado: !!elegido });
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-xl selector-productos" role="dialog" aria-modal="true" aria-label="Agregar productos" onMouseDown={(e) => e.stopPropagation()}>
        <div className="modal-cab"><h3>Agregar productos {agregados > 0 && <span className="chip-ok">{agregados} agregado{agregados > 1 ? 's' : ''}</span>}</h3><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="selector-filtros">
          <div className="buscar"><Icono n="search" size={16} /><input autoFocus ref={nav.buscarRef} className="control" placeholder="Buscar por código, nombre o categoría..." value={q} onChange={(e) => setQ(e.target.value)} aria-label="Buscar producto"
            onKeyDown={nav.teclaBuscar} /></div>
        </div>
        <div className="tabla-scroll alto-selector">
          <table className="grid grid-tarjetas tabla-productos">
            <thead><tr><th>Código</th><th>Producto</th><th>Categoría</th><th className="num">Conv.</th><th className="num">Stock unid.</th><th className="num">Stock cajas</th></tr></thead>
            <tbody ref={nav.tbodyRef}>
              {filtrados.map((p, i) => (
                <tr key={p.Id} className={`fila-clic ${i === nav.marcada ? 'marcada' : ''} ${num(p.stock) <= 0 ? 'sin-stock' : ''}`} onClick={() => elegir(p)} {...nav.propsFila(i)}>
                  <td data-label="Código">{p.CodigoExterno}</td>
                  <td data-label="Producto"><b>{tit(p.NombreProducto)}</b></td>
                  <td data-label="Categoría">{tit(p.NombreCategoria)}</td>
                  <td data-label="Conv." className="num">{n2(p.conversion)}</td>
                  <td data-label="Stock unid." className="num">{num(p.stock) <= 0 ? <span className="badge badge-mal">Sin stock</span> : n2(p.stock)}</td>
                  <td data-label="Stock cajas" className="num">{n2(num(p.stock) / (num(p.conversion) || 1))}</td>
                </tr>
              ))}
              {!filtrados.length && <tr><td colSpan={6} className="vacio">{productos ? 'Sin productos' : 'Cargando productos...'}</td></tr>}
            </tbody>
          </table>
        </div>
        <div className="modal-acciones"><span className="ayuda-tip"><span className="solo-teclado"><kbd>↓</kbd><kbd>↑</kbd> elegir · <kbd>Enter</kbd> cantidad · </span>Toque un producto para indicar la cantidad</span><button className="btn btn-pri" onClick={onCerrar}>Listo</button></div>
      </div>
      {elegido && <CantidadCajas sinLimite={sinLimite} producto={{ ...elegido, Conversion: elegido.conversion }} onCancelar={() => { setElegido(null); nav.volver(false); }}
        onAceptar={(cant) => { onAgregar(elegido, cant); setAgregados((x) => x + 1); setElegido(null); setQ(''); nav.volver(true); }} />}
    </div>,
    document.body,
  );
}

// ImprimirNotaSalida (Reporte_Despacho): columnas tal como las devuelve MAM_DespachoProductos @tipo 6
function NotaDespacho({ datos, onCerrar }) {
  const { filas, cab = {}, fila = {}, form = {} } = datos;
  const ahora = new Date(); const dd = (x) => String(x).padStart(2, '0');
  const id = fila.Id ?? cab.Id;
  const fecha = fila.Fecha || form.Fecha || cab.Fecha;
  const conc = fila.ConciliacionId ?? cab.ConciliacionId;
  const total = filas.reduce((a, f) => a + num(f.Cantidad), 0);
  return (
    <NotaImpresa titulo="Comprobante de Despacho de Productos" id={id} fecha={fecha} empresa={datos.empresa} imagenEmpresa={datos.imagenEmpresa} onCerrar={onCerrar}
      grabada={datos.grabada ? `Despacho #${id} ${datos.grabada === 'nueva' ? 'grabado' : 'modificado'} con éxito.` : null}
      cabecera={[
        ['Fecha Despacho:', fecha ? fechaCorta(`${String(fecha).slice(0, 10)}T00:00:00Z`) : ''], ['Cod. Despacho:', Number(id).toLocaleString('en-US')],
        ['Distribuidor:', tit(fila.NombrePersonal || form.Personal || cab.NombrePersonal || '')], ['Nro Conciliación:', conc ? Number(conc).toLocaleString('en-US') : ''],
        ['Detalle:', fila.Detalle ?? form.Detalle ?? cab.Detalle ?? ''],
      ]}
      columnas={[
        { titulo: 'Item', ancho: '9%', valor: (f) => f.Item }, { titulo: 'Producto', valor: (f) => f.Producto },
        ...(filas.some((f) => f.Cajas !== null && f.Cajas !== undefined) ? [{ titulo: 'Cajas', num: true, valor: (f) => bsNota(f.Cajas) }] : []),
        { titulo: 'Cantidad', num: true, valor: (f) => bsNota(f.Cantidad) },
      ]}
      filas={filas}
      pie={[['Impresión:', `${dd(ahora.getDate())}/${dd(ahora.getMonth() + 1)}/${ahora.getFullYear()} ${dd(ahora.getHours())}:${dd(ahora.getMinutes())}:${dd(ahora.getSeconds())}`], ['Productos:', String(filas.length)]]}
      etiquetaTotal="Total General:" total={total} formatoTotal={bsNota} literal={false}
      firmas={['Encargado Almacen', 'Distribuidor']} />
  );
}
