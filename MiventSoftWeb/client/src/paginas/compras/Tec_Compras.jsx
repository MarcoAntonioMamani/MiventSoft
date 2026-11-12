// =====================================================================
//  KAILY LA PAZ: Tec_Compras.vb + Tec_ComprasDetalle.vb -> Gestion De Compras  (SP: MAM_Compras)
//  Buscador por rango de fechas (tbDesde / tbHasta) · Contado / Crédito (con fecha de vencimiento)
//  Detalle en unidades: SubTotal = Cantidad x P.Costo · Total = suma - descuento (sin cajas ni notas de compra/recepción).
//  Sin "Modificar registros anteriores" no se modifica ni elimina una compra de días anteriores.
//  Al cambiar la sucursal en una compra nueva se limpia el detalle (los productos dependen de ella).
//  El servidor recalcula subtotales y total.
// =====================================================================
import { useDeferredValue, useEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, SwitchButton, fechaCorta } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import NotaImpresa from '../../componentes/NotaImpresa';
import Icono from '../../componentes/Icono';
import { useListaTeclado, FilaMas, AYUDA_ELEGIR } from '../../componentes/useListaTeclado';
import { palabrasDe, indexar, filtrarIndice } from '../../global/buscar';
import { NumeroCelda, errorDetalle } from '../ventas/Tec_Ventas';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/compras/compras';
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const r2 = (x) => Math.round(x * 100) / 100;
const VACIO = { Id: '', AlmacenId: '', ProveedorId: '', FechaTransaccion: '', TipoVenta: 1, FechaVencimientoCredito: '', Glosa: '', Descuento: 0, transaccion: 0, fechaOriginal: '' };
let _k = 0;

export default function Tec_Compras() {
  const toast = useToast();
  const [c, setC] = useState({ sucursales: [], proveedores: [], lote: false, hoy: hoy(), modificarAnteriores: false });
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [detalle, setDetalle] = useState([]);
  const [selector, setSelector] = useState(false);
  const [nota, setNota] = useState(null);
  const [filtro, setFiltro] = useState({ desde: hoy(), hasta: hoy() });
  // ReporteCompra del escritorio: "¿Desea Generar el Reporte de la Compra #?" -> vista previa con Imprimir / No imprimir
  const verNota = async (id, fecha, grabada = false) => {
    if (!id) return;
    try { const r = await api.get(`${URL}/${id}/nota?fecha=${encodeURIComponent(fecha)}`); setNota({ ...r.data, grabada }); } catch (e) { toast(e.message, 'error'); }
  };
  const [cargandoDet, setCargandoDet] = useState(false);
  useAlActivar(() => { api.get(`${URL}/combos`).then((r) => setC(r.data)).catch((e) => toast(e.message, 'error')); });
  useEffect(() => { if (c.sucursales.length) setForm((f) => (f.Id === '' && !f.AlmacenId ? { ...f, AlmacenId: c.sucursales[0].Id } : f)); }, [c.sucursales]);
  const vig = detalle.filter((d) => !d.eliminado);
  const suma = r2(vig.reduce((a, d) => a + num(d.TotalCompra), 0));
  const total = r2(suma - num(form.Descuento));
  const set = (k) => (e) => { const x = e?.target ? e.target.value : e; setErrores((o) => ({ ...o, [k]: undefined })); setForm((f) => ({ ...f, [k]: x })); };

  // btnModificar / btnEliminar: Global_ModificarRegistro = 0 y FechaTransaccion < hoy
  const bloqueo = (accion) => (f) => {
    if (!f) return null;
    if (!c.modificarAnteriores && f.FechaTransaccion < c.hoy) return `No puede ${accion} Compras Anteriores a la fecha de Hoy, Consulte con su Administrador`.toUpperCase();
    if (accion === 'Eliminar' && f.transaccion === 1) return 'No Es Posible Eliminar la Compra por que Tiene Pagos de Creditos Registrado'.toUpperCase();
    return null;
  };
  const verFecha = (fecha) => { if (fecha && (fecha < filtro.desde || fecha > filtro.hasta)) setFiltro((x) => ({ desde: fecha < x.desde ? fecha : x.desde, hasta: fecha > x.hasta ? fecha : x.hasta })); };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => {
      const q = new URLSearchParams({ desde: filtro.desde, hasta: filtro.hasta });
      return (await api.get(`${URL}?${q}`)).data.map((f) => ({ ...f, FechaTxt: fechaCorta(`${f.FechaTransaccion}T00:00:00Z`), TipoTxt: f.TipoVenta ? 'CONTADO' : 'CRÉDITO' }));
    },
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('FechaTxt', true, 'Fecha Transaccion', 100),
      new Celda('NombreProveedor', true, 'Proveedor', 220),
      new Celda('TipoTxt', true, 'Tipo', 90),
      new Celda('TituloMoneda', true, 'Moneda', 90),
      new Celda('Glosa', true, 'Glosa', 220),
      new Celda('TotalCompra', true, 'Total Compra', 110, '0.00', 'num'),
    ],
    _PMOLimpiar: () => {
      setForm({ ...VACIO, AlmacenId: c.sucursales[0]?.Id ?? '', FechaTransaccion: hoy(), FechaVencimientoCredito: hoy() });
      setDetalle([]);
    },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, AlmacenId: f.AlmacenId, ProveedorId: f.ProveedorId, FechaTransaccion: f.FechaTransaccion, TipoVenta: f.TipoVenta, FechaVencimientoCredito: f.FechaVencimientoCredito || f.FechaTransaccion,
        Glosa: f.Glosa ?? '', Descuento: num(f.descuento), transaccion: f.transaccion, fechaOriginal: f.FechaTransaccion });
      setCargandoDet(true);
      api.get(`${URL}/${f.Id}/detalle`).then((r) => setDetalle(r.data.map((d) => ({ ...d, key: ++_k })))).catch((e) => toast(e.message, 'error')).finally(() => setCargandoDet(false));
    },
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOBloqueoModificar: bloqueo('Modificar'),
    _PMOBloqueoEliminar: bloqueo('Eliminar'),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.AlmacenId) e.AlmacenId = 'Seleccione una Sucursal';
      if (!form.ProveedorId) e.ProveedorId = 'Seleccione un Proveedor';
      if (!form.FechaTransaccion) e.FechaTransaccion = 'Ingrese una Fecha Valida';
      if (!form.TipoVenta && form.FechaVencimientoCredito < form.FechaTransaccion) e.FechaVencimientoCredito = 'No puede ser menor a la fecha de compra';
      if (num(form.Descuento) > suma) e.Descuento = 'El descuento no puede ser mayor al total';
      setErrores(e);
      if (Object.keys(e).length) { toast('Los Siguientes Campos Son Requeridos'.toUpperCase(), 'error'); return false; }
      if (!vig.length) { toast('Por Favor Inserte un Detalle'.toUpperCase(), 'error'); return false; }
      const ed = errorDetalle(vig, errFila, (d) => d.Producto);
      if (ed) { toast(ed, 'error', 6000); return false; }
      return true;
    },
    _PMOGrabarRegistro: async () => {
      const r = await api.post(URL, { ...form, detalle: vig.map((d) => ({ ProductoId: d.ProductoId, CantidadCompra: d.CantidadCompra, PrecioCosto: d.PrecioCosto, PrecioVenta: d.PrecioVenta })) });
      verFecha(r.fecha);
      verNota(r.id, r.fecha || form.FechaTransaccion, 'nueva');
      return r;
    },
    _PMOModificarRegistro: async () => {
      const r = await api.put(`${URL}/${form.Id}`, { ...form, detalle: detalle.filter((d) => !(d.nuevo && d.eliminado)).map((d) => (d.nuevo
        ? { nuevo: true, ProductoId: d.ProductoId, CantidadCompra: d.CantidadCompra, PrecioCosto: d.PrecioCosto, PrecioVenta: d.PrecioVenta }
        : { Id: d.Id, CantidadCompra: d.CantidadCompra, PrecioCosto: d.PrecioCosto, PrecioVenta: d.PrecioVenta, eliminado: !!d.eliminado })) });
      verFecha(r.fecha);
      verNota(form.Id, r.fecha || form.FechaTransaccion, 'modificada');
      return r;
    },
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}?fecha=${encodeURIComponent(f.FechaTransaccion)}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar la Compra ${f.Id} ?`,
  });
  const ro = !m.habilitado;
  const primera = useRef(true);
  useEffect(() => { if (primera.current) { primera.current = false; return; } m._PMFiltrar(); }, [filtro.desde, filtro.hasta]); // eslint-disable-line react-hooks/exhaustive-deps

  const cambiarSucursal = (x) => { set('AlmacenId')(x); if (m.modo === 'nuevo' && detalle.length) { setDetalle([]); toast('Se limpió el detalle por el cambio de sucursal', 'ok', 2500); } };
  // CantidadCompra / precios -> SubTotal = CantidadCompra x P.Costo (P_PonerTotal); Cantidad = CantidadCompra + bonificación
  // el valor se aplica en cada tecla aunque sea invalido: la celda lo marca (errFila) y Grabar lo bloquea
  const cambiar = (key, cambios) => setDetalle((d) => d.map((x) => {
    if (x.key !== key) return x;
    const y = { ...x, ...cambios, _editado: true };
    const incremento = num(y.CantidadCompra) * (num(y.PorcentajeIncremento) / 100);
    return { ...y, CantidadIncremento: incremento, Cantidad: num(y.CantidadCompra) + incremento, TotalCompra: r2(num(y.PrecioCosto) * num(y.CantidadCompra)) };
  }));
  const errFila = (d) => {
    if (!d._editado) return {};
    const e = {};
    if (!(num(d.CantidadCompra) > 0)) e.CantidadCompra = 'La Cantidad debe ser Mayor a 0';
    if (!(num(d.PrecioCosto) > 0)) e.PrecioCosto = 'El Precio Costo debe ser Mayor a 0';
    if (num(d.PrecioVenta) < 0) e.PrecioVenta = 'El Precio Venta no puede ser negativo';
    return e;
  };
  const enDetalle = useMemo(() => new Set(vig.map((d) => Number(d.ProductoId))), [vig]);
  const porc = suma > 0 ? (num(form.Descuento) * 100) / suma : 0;

  return (
    <>
    <ModeloF1 m={m} titulo="Compras" subtitulo="Gestion De Compras" icono="bag"
      accionesRegistro={<button className="btn btn-sec" onClick={() => m.filaActual && verNota(m.filaActual.Id, m.filaActual.FechaTransaccion)} disabled={!m.filaActual}><Icono n="printer" size={16} /> <span className="txt-btn">Nota de compra</span></button>}
      accionesFila={(f) => [{ texto: 'Nota de compra', icono: 'printer', onClick: () => verNota(f.Id, f.FechaTransaccion) }]}
      extraBuscador={(
        <div className="filtros-lista">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={filtro.desde} max={filtro.hasta} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={filtro.hasta} min={filtro.desde} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, hasta: e.target.value }))} /></label>
        </div>
      )}>
      {c.lote && !ro && <p className="aviso-lote"><Icono n="alert" size={16} /> El sistema usa lotes: registre las compras desde el escritorio.</p>}
      <div className="venta-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos de la compra <span className={`chip-mora ${form.TipoVenta ? 'al-dia' : 'mora-alta'}`}>{form.TipoVenta ? 'Contado' : 'Crédito'}</span></h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Fecha transacción" requerido error={errores.FechaTransaccion}><input type="date" className="control" value={form.FechaTransaccion} onChange={set('FechaTransaccion')} readOnly={ro} disabled={ro} /></Campo>
            <Campo etiqueta="Sucursal" requerido error={errores.AlmacenId}>
              <ComboMulti dt={c.sucursales} valor={form.AlmacenId} onChange={cambiarSucursal} id="Id" nameId="Cód." descripcionId="Sucursal" descripcion="Sucursal" deshabilitado={ro} />
            </Campo>
            <Campo etiqueta="Proveedor" requerido error={errores.ProveedorId}>
              <ComboMulti dt={c.proveedores} valor={form.ProveedorId} onChange={set('ProveedorId')} id="Id" nameId="Cód." descripcionId="Nombre" descripcion="Proveedor" deshabilitado={ro} />
            </Campo>
            <Campo etiqueta="Tipo"><SwitchButton valor={!!form.TipoVenta} onChange={(x) => setForm((f) => ({ ...f, TipoVenta: x ? 1 : 0 }))} deshabilitado={ro} textoOn="CONTADO" textoOff="CRÉDITO" /></Campo>
            {!form.TipoVenta && (
              <Campo etiqueta="Vencimiento crédito" requerido error={errores.FechaVencimientoCredito}><input type="date" className="control" value={form.FechaVencimientoCredito} min={form.FechaTransaccion} onChange={set('FechaVencimientoCredito')} readOnly={ro} disabled={ro} /></Campo>
            )}
            <Campo etiqueta="Glosa" ancho={2}><input className="control" value={form.Glosa} onChange={set('Glosa')} readOnly={ro} maxLength={500} /></Campo>
          </div>
        </div>
        <div className="tarjeta venta-detalle">
          <div className="detalle-cab-venta">
            <h2 className="tarjeta-tit">Productos <span className="contador">{vig.length}</span></h2>
            {!ro && <button className="btn btn-pri" onClick={() => (form.AlmacenId ? setSelector(true) : toast('Seleccione una Sucursal'.toUpperCase(), 'error'))}><Icono n="plus" size={16} /> Agregar producto</button>}
          </div>
          {cargandoDet ? <div className="cargando-bloque"><span className="spinner" /> Cargando detalle...</div> : (
            <div className="tabla-scroll">
              <table className="grid grid-tarjetas tabla-detalle">
                <thead><tr><th>Producto</th><th className="num">Cantidad</th><th className="num">P. Costo</th><th className="num">P. Venta</th><th className="num">SubTotal</th>{!ro && <th aria-label="Quitar" />}</tr></thead>
                <tbody>
                  {vig.map((d) => (
                    <tr key={d.key} className={d.nuevo ? 'fila-nueva' : ''}>
                      <td data-label="Producto" className="det-producto"><b>{tit(d.Producto)}</b><small>Cód. {d.ProductoId}{num(d.CantidadIncremento) > 0 ? ` · Bonif. ${n2(d.CantidadIncremento)} · Total ${n2(d.Cantidad)}` : ''}</small></td>
                      <td data-label="Cantidad" className="num">{ro ? n2(d.CantidadCompra) : <NumeroCelda valor={num(d.CantidadCompra)} onCambio={(x) => cambiar(d.key, { CantidadCompra: num(x) })} etiqueta={`Cantidad de ${d.Producto}`} error={errFila(d).CantidadCompra} />}</td>
                      <td data-label="P. Costo" className="num">{ro ? n2(d.PrecioCosto) : <NumeroCelda valor={num(d.PrecioCosto)} onCambio={(x) => cambiar(d.key, { PrecioCosto: num(x) })} etiqueta={`Precio costo de ${d.Producto}`} error={errFila(d).PrecioCosto} />}</td>
                      <td data-label="P. Venta" className="num">{ro ? n2(d.PrecioVenta) : <NumeroCelda valor={num(d.PrecioVenta)} onCambio={(x) => cambiar(d.key, { PrecioVenta: num(x) })} etiqueta={`Precio venta de ${d.Producto}`} error={errFila(d).PrecioVenta} />}</td>
                      <td data-label="SubTotal" className="num"><b>{n2(d.TotalCompra)}</b></td>
                      {!ro && <td className="col-acc"><button className="btn-icono peligro" onClick={() => setDetalle((x) => x.map((y) => (y.key === d.key ? { ...y, eliminado: true } : y)))} aria-label={`Quitar ${d.Producto}`} title="Quitar"><Icono n="trash" size={16} /></button></td>}
                    </tr>
                  ))}
                  {!vig.length && <tr><td colSpan={6} className="vacio">{ro ? 'Sin productos' : 'Agregue productos con "Agregar producto"'}</td></tr>}
                </tbody>
              </table>
            </div>
          )}
          <div className="venta-totales">
            <div><span>Suma detalle</span><b>{n2(suma)}</b></div>
            <div className="venta-desc">
              <span>Descuento</span>
              {ro ? <b>{n2(form.Descuento)} <small>({n2(porc)}%)</small></b> : (
                <span className="desc-inputs">
                  <label><input className="control num" type="number" min="0" max="100" step="0.01" value={Math.round(porc * 100) / 100 || ''} placeholder="0" onChange={(e) => setForm((f) => ({ ...f, Descuento: r2((suma * Math.min(100, num(e.target.value))) / 100) }))} aria-label="Porcentaje descuento" />%</label>
                  <label>Bs<input className="control num" type="number" min="0" step="0.01" value={form.Descuento || ''} placeholder="0" onChange={(e) => setForm((f) => ({ ...f, Descuento: Math.max(0, num(e.target.value)) }))} aria-label="Monto descuento" /></label>
                </span>
              )}
            </div>
            {errores.Descuento && <p className="campo-error">{errores.Descuento}</p>}
            <div className="venta-total"><span>Total compra</span><b>Bs {n2(total)}</b></div>
          </div>
        </div>
      </div>
      {selector && <SelectorCompras url={`${URL}/productos/${form.AlmacenId}`} enDetalle={enDetalle} onCerrar={() => setSelector(false)}
        onAgregar={(p, cant) => setDetalle((d) => [...d, { key: ++_k, nuevo: true, ProductoId: p.Id, Producto: p.NombreProducto, CantidadCompra: cant, PorcentajeIncremento: 0, CantidadIncremento: 0, Cantidad: cant,
          PrecioCosto: num(p.PrecioCosto), PrecioVenta: num(p.PrecioVenta), TotalCompra: r2(num(p.PrecioCosto) * cant) }])} />}
    </ModeloF1>
    {nota && <NotaCompra datos={nota} onCerrar={() => setNota(null)} />}
    </>
  );
}

// ---------- Tec_ComprasDetalle: Cod Externo, Descripción, Categoría, Industria y Stock; cantidad sin límite de stock ----------
function SelectorCompras({ url, enDetalle, onAgregar, onCerrar }) {
  const toast = useToast();
  const [productos, setProductos] = useState(null);
  const [q, setQ] = useState('');
  const [elegido, setElegido] = useState(null);
  const [agregados, setAgregados] = useState(0);
  useEffect(() => { api.get(url).then((r) => setProductos(r.data)).catch((e) => { toast(e.message, 'error'); onCerrar(); }); }, []); // eslint-disable-line react-hooks/exhaustive-deps
  // tbProducto_TextChanged: Id + codigo + descripcion + categoria + industria
  // (texto normalizado una sola vez por carga; la lista se filtra sin trabar lo que se escribe)
  const indice = useMemo(() => indexar(productos, (p) => `${p.Id} ${p.CodigoExterno ?? ''} ${p.DescripcionProducto ?? ''} ${p.NombreProducto ?? ''} ${p.NombreCategoria ?? ''} ${p.industria ?? ''}`), [productos]);
  const qBusca = useDeferredValue(q);
  const filtrados = useMemo(() => filtrarIndice(indice, palabrasDe(qBusca)).filter((p) => !enDetalle.has(Number(p.Id))), [indice, qBusca, enDetalle]);
  const nav = useListaTeclado(filtrados, setElegido, { onCerrar, pausado: !!elegido });
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-xl selector-productos" role="dialog" aria-modal="true" aria-label="Agregar productos" onMouseDown={(e) => e.stopPropagation()}>
        <div className="modal-cab"><h3>Agregar productos {agregados > 0 && <span className="chip-ok">{agregados} agregado{agregados > 1 ? 's' : ''}</span>}</h3><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="selector-filtros">
          <div className="buscar"><Icono n="search" size={16} /><input autoFocus ref={nav.buscarRef} className="control" placeholder="Buscar por código, descripción, categoría o industria..." value={q} onChange={(e) => setQ(e.target.value)} aria-label="Buscar producto"
            onKeyDown={nav.teclaBuscar} /></div>
        </div>
        <div className="tabla-scroll alto-selector">
          <table className="grid grid-tarjetas tabla-productos">
            <thead><tr><th>Cód. externo</th><th>Descripción</th><th>Categoría</th><th>Industria</th><th className="num">Stock</th></tr></thead>
            <tbody ref={nav.tbodyRef}>
              {nav.visibles.map((p, i) => (
                <tr key={p.Id} className={`fila-clic ${i === nav.marcada ? 'marcada' : ''}`} {...nav.propsFila(i)}>
                  <td data-label="Cód. externo">{p.CodigoExterno}</td>
                  <td data-label="Descripción"><b>{tit(p.DescripcionProducto || p.NombreProducto)}</b></td>
                  <td data-label="Categoría">{tit(p.NombreCategoria)}</td>
                  <td data-label="Industria">{tit(p.industria)}</td>
                  <td data-label="Stock" className="num">{n2(p.stock)}</td>
                </tr>
              ))}
              <FilaMas nav={nav} columnas={5} />
              {!filtrados.length && <tr><td colSpan={5} className="vacio">{productos ? 'Sin productos' : 'Cargando productos...'}</td></tr>}
            </tbody>
          </table>
        </div>
        <div className="modal-acciones"><span className="ayuda-tip"><span className="solo-teclado"><kbd>↓</kbd><kbd>↑</kbd> elegir · <kbd>Enter</kbd> cantidad · </span>{AYUDA_ELEGIR}</span><button className="btn btn-pri" onClick={onCerrar}>Listo</button></div>
      </div>
      {elegido && <CantidadCompra producto={elegido} onCancelar={() => { setElegido(null); nav.volver(false); }}
        onAceptar={(cant) => { onAgregar(elegido, cant); setAgregados((x) => x + 1); setElegido(null); setQ(''); nav.volver(true); }} />}
    </div>,
    document.body,
  );
}

// FormularioCantidadProductos: cantidad mayor a 0 (compras no se limitan por stock)
function CantidadCompra({ producto, onAceptar, onCancelar }) {
  const [cant, setCant] = useState('1');
  const [err, setErr] = useState('');
  const validar = () => { const x = num(cant); if (!(x > 0)) { setErr('La Cantidad debe ser Mayor a 0'); return; } onAceptar(x); };
  return (
    <div className="modal-fondo modal-encima" onMouseDown={(e) => { e.stopPropagation(); onCancelar(); }}>
      <div className="modal modal-sm cantidad-cajas" role="dialog" aria-modal="true" aria-label="Cantidad" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') { e.stopPropagation(); onCancelar(); } if (e.key === 'Enter') { e.preventDefault(); validar(); } }}>
        <h3>{tit(producto.DescripcionProducto || producto.NombreProducto)}</h3>
        <p className="cc-stock">Stock: <b>{n2(producto.stock)}</b> · P. Costo: <b>{n2(producto.PrecioCosto)}</b></p>
        <div className="cc-campos cc-unico">
          <label>Cantidad<input autoFocus onFocus={(e) => e.target.select()} className="control num" type="number" inputMode="decimal" min="0" step="any" value={cant}
            onChange={(e) => { setErr(''); setCant(e.target.value); }} /></label>
        </div>
        {err && <p className="campo-error" role="alert">{err}</p>}
        <div className="modal-acciones">
          <button className="btn btn-sec" onClick={onCancelar}>Cancelar</button>
          <button className="btn btn-pri" onClick={validar}>Agregar</button>
        </div>
      </div>
    </div>
  );
}

// ---------- Nota de compra (Reporte_Compras): la hoja comun de componentes/NotaImpresa ----------
function NotaCompra({ datos, onCerrar }) {
  const { compra, detalle, proveedor, codigos = {} } = datos;
  const credito = Number(compra.TipoVenta) === 0;
  return (
    <NotaImpresa titulo="Nota de Compra" id={compra.Id} fecha={compra.FechaTransaccion} empresa={datos.empresa} imagenEmpresa={datos.imagenEmpresa} onCerrar={onCerrar}
      grabada={datos.grabada ? `Compra #${compra.Id} ${datos.grabada === 'nueva' ? 'grabada' : 'modificada'} con éxito.` : null}
      cabecera={[
        ['Proveedor:', tit(compra.NombreProveedor || proveedor?.Nombre || '')], ['Sucursal:', tit(datos.sucursal || '')],
        ['Dirección:', proveedor?.Direccion || ''], ['Teléfono:', proveedor?.Telefono01 || ''],
        ['Tipo Compra:', credito ? 'Crédito' : 'Contado'], ['Fecha Venc. Crédito:', credito && compra.FechaVencimientoCredito ? fechaCorta(`${compra.FechaVencimientoCredito}T00:00:00Z`) : ''],
      ]}
      columnas={[
        { titulo: 'Código', num: true, valor: (f) => codigos[f.Producto] ?? f.ProductoId }, { titulo: 'Producto', valor: (f) => f.Producto },
        { titulo: 'Cantidad', num: true, valor: (f) => n2(f.CantidadCompra) },
        { titulo: 'P. Costo', num: true, valor: (f) => n2(f.PrecioCosto) }, { titulo: 'Total', num: true, valor: (f) => n2(f.TotalCompra) },
      ]}
      filas={detalle} claveFila={(f, i) => f.Id || i}
      pie={[['Moneda:', tit(compra.TituloMoneda || 'Bolivianos')], ['Glosa:', compra.Glosa || '']]}
      totales={[['Sub Total:', n2(datos.subtotal)], ['Descuento:', n2(compra.descuento)]]}
      etiquetaTotal="Total Compra: Bs" total={datos.total} />
  );
}
