// =====================================================================
//  Tec_Compras.vb + Tec_ComprasDetalle.vb -> Gestion De Compras  (SP: MAM_Compras)
//  Contado / Crédito (con fecha de vencimiento) · SubTotal = P.Costo x Cajas · Total = suma - descuento.
//  Al cambiar la sucursal en una compra nueva se limpia el detalle (los productos dependen de ella).
//  El servidor recalcula subtotales y total.
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, SwitchButton, fechaCorta } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import NotaImpresa from '../../componentes/NotaImpresa';
import Icono from '../../componentes/Icono';
import { NumeroCelda, errorDetalle } from '../ventas/Tec_Ventas';
import { SelectorProductos } from '../distribucion/Tec_Despachos';

const URL = '/api/compras/compras';
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const r2 = (x) => Math.round(x * 100) / 100;
let _k = 0;

export default function Tec_Compras() {
  const toast = useToast();
  const [c, setC] = useState({ sucursales: [], proveedores: [], lote: false });
  const [form, setForm] = useState({ Id: '', AlmacenId: '', ProveedorId: '', FechaTransaccion: '', TipoVenta: 1, FechaVencimientoCredito: '', Glosa: '', NroNotaCompra: '', NroNotaRecepcion: '', Descuento: 0, transaccion: 0 });
  const [errores, setErrores] = useState({});
  const [detalle, setDetalle] = useState([]);
  const [selector, setSelector] = useState(false);
  const [nota, setNota] = useState(null);
  // ReporteCompra del escritorio: "¿Desea Generar el Reporte de la Compra #?" -> vista previa con Imprimir / No imprimir
  const verNota = async (id, grabada = false) => {
    if (!id) return;
    try { const r = await api.get(`${URL}/${id}/nota`); setNota({ ...r.data, grabada }); } catch (e) { toast(e.message, 'error'); }
  };
  const [cargandoDet, setCargandoDet] = useState(false);
  useEffect(() => { api.get(`${URL}/combos`).then((r) => setC(r.data)).catch((e) => toast(e.message, 'error')); }, [toast]);
  useEffect(() => { if (c.sucursales.length) setForm((f) => (f.Id === '' && !f.AlmacenId ? { ...f, AlmacenId: c.sucursales[0].Id } : f)); }, [c.sucursales]);
  const vig = detalle.filter((d) => !d.eliminado);
  const suma = r2(vig.reduce((a, d) => a + num(d.TotalCompra), 0));
  const total = r2(suma - num(form.Descuento));
  const set = (k) => (e) => { const x = e?.target ? e.target.value : e; setErrores((o) => ({ ...o, [k]: undefined })); setForm((f) => ({ ...f, [k]: x })); };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data.map((f) => ({ ...f, FechaTxt: fechaCorta(`${f.FechaTransaccion}T00:00:00Z`), TipoTxt: f.TipoVenta ? 'CONTADO' : 'CRÉDITO' })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NroNotaCompra', true, 'Nota Compra', 100),
      new Celda('NroNotaRecepcion', true, 'Nota Recepción', 100),
      new Celda('FechaTxt', true, 'Fecha Compra', 100),
      new Celda('NombreProveedor', true, 'Proveedor', 220),
      new Celda('TipoTxt', true, 'Tipo', 90),
      new Celda('Glosa', true, 'Glosa', 220),
      new Celda('TotalCompra', true, 'Total Compra', 110, '0.00', 'num'),
    ],
    _PMOLimpiar: () => {
      setForm({ Id: '', AlmacenId: c.sucursales[0]?.Id ?? '', ProveedorId: '', FechaTransaccion: hoy(), TipoVenta: 1, FechaVencimientoCredito: hoy(), Glosa: '', NroNotaCompra: '', NroNotaRecepcion: '', Descuento: 0, transaccion: 0 });
      setDetalle([]);
    },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, AlmacenId: f.AlmacenId, ProveedorId: f.ProveedorId, FechaTransaccion: f.FechaTransaccion, TipoVenta: f.TipoVenta, FechaVencimientoCredito: f.FechaVencimientoCredito || f.FechaTransaccion,
        Glosa: f.Glosa ?? '', NroNotaCompra: f.NroNotaCompra ?? '', NroNotaRecepcion: f.NroNotaRecepcion ?? '', Descuento: num(f.descuento), transaccion: f.transaccion });
      setCargandoDet(true);
      api.get(`${URL}/${f.Id}/detalle`).then((r) => setDetalle(r.data.map((d) => ({ ...d, key: ++_k })))).catch((e) => toast(e.message, 'error')).finally(() => setCargandoDet(false));
    },
    _PMOLimpiarErrores: () => setErrores({}),
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
      const r = await api.post(URL, { ...form, detalle: vig.map((d) => ({ ProductoId: d.ProductoId, Cantidad: d.Cantidad, PrecioCosto: d.PrecioCosto, PrecioVenta: d.PrecioVenta })) });
      verNota(r.id, 'nueva');
      return r;
    },
    _PMOModificarRegistro: async () => {
      const r = await api.put(`${URL}/${form.Id}`, { ...form, detalle: detalle.filter((d) => !(d.nuevo && d.eliminado)).map((d) => (d.nuevo
        ? { nuevo: true, ProductoId: d.ProductoId, Cantidad: d.Cantidad, PrecioCosto: d.PrecioCosto, PrecioVenta: d.PrecioVenta }
        : { Id: d.Id, Cantidad: d.Cantidad, PrecioCosto: d.PrecioCosto, PrecioVenta: d.PrecioVenta, eliminado: !!d.eliminado })) });
      verNota(form.Id, 'modificada');
      return r;
    },
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMOBloqueoEliminar: (f) => (f.transaccion === 1 ? 'No Es Posible Eliminar la Compra por que Tiene Pagos de Creditos Registrado'.toUpperCase() : null),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar la Compra ${f.Id} ?`,
  });
  const ro = !m.habilitado;
  const cambiarSucursal = (x) => { set('AlmacenId')(x); if (m.modo === 'nuevo' && detalle.length) { setDetalle([]); toast('Se limpió el detalle por el cambio de sucursal', 'ok', 2500); } };
  // Cantidad (unidades) / Cajas / precios -> SubTotal = P.Costo x Cajas (P_PonerTotal)
  // el valor se aplica en cada tecla aunque sea invalido: la celda lo marca (errFila) y Grabar lo bloquea
  const cambiar = (key, cambios) => setDetalle((d) => d.map((x) => {
    if (x.key !== key) return x;
    const y = { ...x, ...cambios, _editado: true };
    const cajas = num(y.Cantidad) / (num(y.conversion) || 1);
    return { ...y, CantidadCajas: cajas, TotalCompra: r2(num(y.PrecioCosto) * cajas) };
  }));
  const errFila = (d) => {
    if (!d._editado) return {};
    const e = {};
    if (!(num(d.Cantidad) > 0)) e.Cantidad = 'La Cantidad debe ser Mayor a 0';
    if (!(num(d.PrecioCosto) > 0)) e.PrecioCosto = 'El Precio Costo debe ser Mayor a 0';
    if (num(d.PrecioVenta) < 0) e.PrecioVenta = 'El Precio Venta no puede ser negativo';
    return e;
  };
  const enDetalle = useMemo(() => new Set(vig.map((d) => Number(d.ProductoId))), [vig]);
  const porc = suma > 0 ? (num(form.Descuento) * 100) / suma : 0;

  return (
    <>
    <ModeloF1 m={m} titulo="Compras" subtitulo="Gestion De Compras" icono="bag"
      accionesRegistro={<button className="btn btn-sec" onClick={() => verNota(m.filaActual?.Id)} disabled={!m.filaActual}><Icono n="printer" size={16} /> <span className="txt-btn">Nota de compra</span></button>}
      accionesFila={(f) => [{ texto: 'Nota de compra', icono: 'printer', onClick: () => verNota(f.Id) }]}>
      {c.lote && !ro && <p className="aviso-lote"><Icono n="alert" size={16} /> El sistema usa lotes: registre las compras desde el escritorio.</p>}
      <div className="venta-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos de la compra <span className={`chip-mora ${form.TipoVenta ? 'al-dia' : 'mora-alta'}`}>{form.TipoVenta ? 'Contado' : 'Crédito'}</span></h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Fecha compra" requerido error={errores.FechaTransaccion}><input type="date" className="control" value={form.FechaTransaccion} onChange={set('FechaTransaccion')} readOnly={ro} disabled={ro} /></Campo>
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
            <Campo etiqueta="Nota de compra"><input className="control" value={form.NroNotaCompra} onChange={set('NroNotaCompra')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="Nota de recepción"><input className="control" value={form.NroNotaRecepcion} onChange={set('NroNotaRecepcion')} readOnly={ro} maxLength={50} /></Campo>
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
                <thead><tr><th>Producto</th><th className="num">Cajas</th><th className="num">Unidades</th><th className="num">P. Costo</th><th className="num">P. Venta</th><th className="num">SubTotal</th>{!ro && <th aria-label="Quitar" />}</tr></thead>
                <tbody>
                  {vig.map((d) => (
                    <tr key={d.key} className={d.nuevo ? 'fila-nueva' : ''}>
                      <td data-label="Producto" className="det-producto"><b>{tit(d.Producto)}</b><small>Cód. {d.ProductoId} · {n2(d.conversion)} u/caja</small></td>
                      <td data-label="Cajas" className="num">{ro ? n2(d.CantidadCajas) : <NumeroCelda valor={num(d.CantidadCajas)} onCambio={(x) => cambiar(d.key, { Cantidad: num(x) * (num(d.conversion) || 1) })} etiqueta={`Cajas de ${d.Producto}`} error={errFila(d).Cantidad} />}</td>
                      <td data-label="Unidades" className="num">{ro ? n2(d.Cantidad) : <NumeroCelda valor={num(d.Cantidad)} onCambio={(x) => cambiar(d.key, { Cantidad: num(x) })} etiqueta={`Unidades de ${d.Producto}`} error={errFila(d).Cantidad} />}</td>
                      <td data-label="P. Costo" className="num">{ro ? n2(d.PrecioCosto) : <NumeroCelda valor={num(d.PrecioCosto)} onCambio={(x) => cambiar(d.key, { PrecioCosto: num(x) })} etiqueta={`Precio costo de ${d.Producto}`} error={errFila(d).PrecioCosto} />}</td>
                      <td data-label="P. Venta" className="num">{ro ? n2(d.PrecioVenta) : <NumeroCelda valor={num(d.PrecioVenta)} onCambio={(x) => cambiar(d.key, { PrecioVenta: num(x) })} etiqueta={`Precio venta de ${d.Producto}`} error={errFila(d).PrecioVenta} />}</td>
                      <td data-label="SubTotal" className="num"><b>{n2(d.TotalCompra)}</b></td>
                      {!ro && <td className="col-acc"><button className="btn-icono peligro" onClick={() => setDetalle((x) => x.map((y) => (y.key === d.key ? { ...y, eliminado: true } : y)))} aria-label={`Quitar ${d.Producto}`} title="Quitar"><Icono n="trash" size={16} /></button></td>}
                    </tr>
                  ))}
                  {!vig.length && <tr><td colSpan={7} className="vacio">{ro ? 'Sin productos' : 'Agregue productos con "Agregar producto"'}</td></tr>}
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
      {selector && <SelectorProductos url={`${URL}/productos/${form.AlmacenId}`} sinLimite enDetalle={enDetalle} onCerrar={() => setSelector(false)}
        onAgregar={(p, cant) => setDetalle((d) => [...d, { key: ++_k, nuevo: true, ProductoId: p.Id, Producto: p.NombreProducto, Cantidad: cant, CantidadCajas: cant / (num(p.conversion) || 1), conversion: p.conversion,
          PrecioCosto: num(p.PrecioCosto), PrecioVenta: num(p.PrecioVenta), TotalCompra: r2(num(p.PrecioCosto) * (cant / (num(p.conversion) || 1))) }])} />}
    </ModeloF1>
    {nota && <NotaCompra datos={nota} onCerrar={() => setNota(null)} />}
    </>
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
        ['Nota de Compra:', compra.NroNotaCompra || ''], ['Nota de Recepción:', compra.NroNotaRecepcion || ''],
      ]}
      columnas={[
        { titulo: 'Código', num: true, valor: (f) => codigos[f.Producto] ?? f.ProductoId }, { titulo: 'Producto', valor: (f) => f.Producto },
        { titulo: 'Cajas', num: true, valor: (f) => n2(f.CantidadCajas) }, { titulo: 'Cantidad', num: true, valor: (f) => n2(f.Cantidad) },
        { titulo: 'P. Costo', num: true, valor: (f) => n2(f.PrecioCosto) }, { titulo: 'Total', num: true, valor: (f) => n2(f.TotalCompra) },
      ]}
      filas={detalle} claveFila={(f, i) => f.Id || i}
      pie={[['Moneda:', tit(compra.TituloMoneda || 'Bolivianos')], ['Glosa:', compra.Glosa || '']]}
      totales={[['Sub Total:', n2(datos.subtotal)], ['Descuento:', n2(compra.descuento)]]}
      etiquetaTotal="Total Compra: Bs" total={datos.total} />
  );
}
