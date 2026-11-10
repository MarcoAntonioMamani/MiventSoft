// =====================================================================
//  Tec_Movimientos.vb + Tec_MovimientoDetalle.vb -> Ingreso y salida de productos  (SP: MAM_Movimientos)
//  Tipo 4 = ingreso (sin límite de stock) · los demás (3 = salida) no superan el stock.
//  Al cambiar el depósito o el tipo en un movimiento nuevo se limpia el detalle (como VB).
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, fechaCorta } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { NumeroCelda, errorDetalle } from '../ventas/Tec_Ventas';
import { SelectorProductos } from '../distribucion/Tec_Despachos';

const URL = '/api/inventario/movimientos';
const INGRESO = 4;
const SALIDA = 3;
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
let _k = 0;

export default function Tec_Movimientos() {
  const toast = useToast();
  const [c, setC] = useState({ depositos: [], tipos: [], lote: false });
  const [form, setForm] = useState({ Id: '', FechaDocumento: '', ConceptoId: '', DepositoId: '', Observacion: '' });
  const [detalle, setDetalle] = useState([]);
  const [selector, setSelector] = useState(false);
  const [cargandoDet, setCargandoDet] = useState(false);
  useEffect(() => { api.get(`${URL}/combos`).then((r) => setC(r.data)).catch((e) => toast(e.message, 'error')); }, [toast]);
  const vig = detalle.filter((d) => !d.eliminado);
  const tipo = Number(form.ConceptoId);
  const esIngreso = tipo === INGRESO;

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data.map((f) => ({ ...f, FechaTxt: fechaCorta(`${f.fdoc}T00:00:00Z`) })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 70, '', 'num'),
      new Celda('FechaTxt', true, 'Fecha Transacción', 110),
      new Celda('nconcep', true, 'Tipo Movimiento', 130),
      new Celda('obs', true, 'Descripción Movimiento', 250),
      new Celda('NombreDeposito', true, 'Depósito', 150),
    ],
    _PMOLimpiar: () => { setForm({ Id: '', FechaDocumento: hoy(), ConceptoId: c.tipos[0]?.Id ?? '', DepositoId: c.depositos[0]?.Id ?? '', Observacion: '' }); setDetalle([]); },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, FechaDocumento: f.fdoc, ConceptoId: f.concep, DepositoId: f.alm, Observacion: f.obs ?? '' });
      setCargandoDet(true);
      api.get(`${URL}/${f.Id}/detalle`).then((r) => setDetalle(r.data.map((d) => ({ ...d, key: ++_k })))).catch((e) => toast(e.message, 'error')).finally(() => setCargandoDet(false));
    },
    _PMOLimpiarErrores: () => {},
    _PMOValidarCampos: () => {
      if (!form.ConceptoId) { toast('Seleccione Tipo Movimiento'.toUpperCase(), 'error'); return false; }
      if (!form.DepositoId) { toast('Seleccion Deposito'.toUpperCase(), 'error'); return false; }
      if (!vig.length) { toast('Por Favor Inserte un Detalle'.toUpperCase(), 'error'); return false; }
      const ed = errorDetalle(vig, errFila, (d) => d.Producto);
      if (ed) { toast(ed, 'error', 6000); return false; }
      return true;
    },
    _PMOGrabarRegistro: () => api.post(URL, { ...form, detalle: vig.map((d) => ({ ProductoId: d.ProductoId, Cantidad: d.Cantidad })) }),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, { ...form, detalle: detalle.filter((d) => !(d.nuevo && d.eliminado)).map((d) => (d.nuevo ? { nuevo: true, ProductoId: d.ProductoId, Cantidad: d.Cantidad } : { Id: d.Id, Cantidad: d.Cantidad, eliminado: !!d.eliminado })) }),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Movimiento ${f.obs || f.Id} ?`,
  });
  const ro = !m.habilitado;
  // cbTipoMovimiento / cbDepositos cambian en un registro nuevo -> se limpia el detalle (VB)
  const cambiarCab = (k) => (x) => { setForm((f) => ({ ...f, [k]: x })); if (m.modo === 'nuevo' && detalle.length) { setDetalle([]); toast('Se limpió el detalle por el cambio', 'ok', 2500); } };
  // el valor se aplica en cada tecla aunque sea invalido: la celda lo marca (errFila) y Grabar lo bloquea
  const cambiarCant = (key, txt) => setDetalle((d) => d.map((x) => {
    if (x.key !== key) return x;
    const n = num(txt);
    return { ...x, Cantidad: n, CantidadCajas: n / (num(x.conversion) || 1), _editado: true };
  }));
  const errFila = (x) => {
    if (!x._editado) return {};
    const n = num(x.Cantidad);
    if (!(n > 0)) return { Cantidad: 'La Cantidad debe ser Mayor a 0' };
    if (!esIngreso && x.stock !== undefined && n > num(x.stock) + 1e-9) return { Cantidad: `La cantidad que se quiere sacar es mayor a la que existe en el stock solo puede Sacar : ${n2(x.stock)}`.toUpperCase() };
    return {};
  };
  const enDetalle = useMemo(() => new Set(vig.map((d) => Number(d.ProductoId))), [vig]);
  const final = (d) => (tipo === SALIDA ? num(d.stock) - num(d.Cantidad) : num(d.stock) + num(d.Cantidad));

  return (
    <ModeloF1 m={m} titulo="Ingreso y salida de productos" subtitulo="Gestion De Movimientos" icono="box">
      {c.lote && !ro && <p className="aviso-lote"><Icono n="alert" size={16} /> El sistema usa lotes: registre los movimientos desde el escritorio.</p>}
      <div className="venta-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del movimiento <span className={`chip-mora ${esIngreso ? 'al-dia' : 'mora-alta'}`}>{esIngreso ? 'Ingreso' : 'Salida'}</span></h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Fecha transacción" requerido><input type="date" className="control" value={form.FechaDocumento} onChange={(e) => setForm((f) => ({ ...f, FechaDocumento: e.target.value }))} readOnly={ro} disabled={ro} /></Campo>
            <Campo etiqueta="Tipo de movimiento" requerido>
              <ComboMulti dt={c.tipos} valor={form.ConceptoId} onChange={cambiarCab('ConceptoId')} id="Id" nameId="Cód." descripcionId="Descripcion" descripcion="Tipo Movimiento" deshabilitado={ro} />
            </Campo>
            <Campo etiqueta="Depósito" requerido>
              <ComboMulti dt={c.depositos} valor={form.DepositoId} onChange={cambiarCab('DepositoId')} id="Id" nameId="Cód." descripcionId="NombreDeposito" descripcion="Depósito" deshabilitado={ro} />
            </Campo>
            <Campo etiqueta="Descripción" ancho={2}><input className="control" value={form.Observacion} onChange={(e) => setForm((f) => ({ ...f, Observacion: e.target.value }))} readOnly={ro} maxLength={500} /></Campo>
          </div>
        </div>
        <div className="tarjeta venta-detalle">
          <div className="detalle-cab-venta">
            <h2 className="tarjeta-tit">Productos <span className="contador">{vig.length}</span></h2>
            {!ro && <button className="btn btn-pri" onClick={() => (form.DepositoId ? setSelector(true) : toast('Seleccion Deposito'.toUpperCase(), 'error'))}><Icono n="plus" size={16} /> Agregar producto</button>}
          </div>
          {cargandoDet ? <div className="cargando-bloque"><span className="spinner" /> Cargando detalle...</div> : (
            <div className="tabla-scroll">
              <table className="grid grid-tarjetas tabla-detalle">
                <thead><tr><th>Producto</th><th className="num">Cajas</th><th className="num">Unidades</th>{!ro && <th className="num">Stock</th>}{!ro && <th className="num">Stock final</th>}{!ro && <th aria-label="Quitar" />}</tr></thead>
                <tbody>
                  {vig.map((d) => (
                    <tr key={d.key} className={d.nuevo ? 'fila-nueva' : ''}>
                      <td data-label="Producto" className="det-producto"><b>{tit(d.Producto)}</b><small>Cód. {d.ProductoId} · {n2(d.conversion)} u/caja</small></td>
                      <td data-label="Cajas" className="num">{ro ? n2(d.CantidadCajas) : <NumeroCelda valor={num(d.CantidadCajas)} onCambio={(x) => cambiarCant(d.key, num(x) * (num(d.conversion) || 1))} etiqueta={`Cajas de ${d.Producto}`} error={errFila(d).Cantidad} />}</td>
                      <td data-label="Unidades" className="num">{ro ? n2(d.Cantidad) : <NumeroCelda valor={num(d.Cantidad)} onCambio={(x) => cambiarCant(d.key, x)} etiqueta={`Unidades de ${d.Producto}`} error={errFila(d).Cantidad} />}</td>
                      {!ro && <td data-label="Stock" className="num">{n2(d.stock)}</td>}
                      {!ro && <td data-label="Stock final" className={`num ${final(d) < 0 ? 'neg' : ''}`}><b>{n2(final(d))}</b></td>}
                      {!ro && <td className="col-acc"><button className="btn-icono peligro" onClick={() => setDetalle((x) => x.map((y) => (y.key === d.key ? { ...y, eliminado: true } : y)))} aria-label={`Quitar ${d.Producto}`} title="Quitar"><Icono n="trash" size={16} /></button></td>}
                    </tr>
                  ))}
                  {!vig.length && <tr><td colSpan={6} className="vacio">{ro ? 'Sin productos' : 'Agregue productos con "Agregar producto"'}</td></tr>}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
      {selector && <SelectorProductos url={`${URL}/productos/${form.DepositoId}`} sinLimite={esIngreso} enDetalle={enDetalle} onCerrar={() => setSelector(false)}
        onAgregar={(p, cant) => setDetalle((d) => [...d, { key: ++_k, nuevo: true, ProductoId: p.Id, Producto: p.DescripcionProducto || p.NombreProducto, Cantidad: cant, CantidadCajas: cant / (num(p.conversion) || 1), conversion: p.conversion, stock: p.stock }])} />}
    </ModeloF1>
  );
}
