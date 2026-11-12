// =====================================================================
//  KAILY LA PAZ: Tec_Movimientos.vb + Tec_MovimientoDetalle.vb -> Ingreso y salida de productos  (SP: MAM_Movimientos)
//  Tipo 4 = ingreso (sin límite de stock, acepta negativos) · 8 = traspaso (depósito origen -> destino) · los demás no superan el stock.
//  Solo "Cantidad" (sin cajas). Stock actual / Stock final solo en un movimiento nuevo.
//  En un ingreso nuevo: "Agregar todos" (cantidad 0) y "Resetear a cero" (cantidad = -stock). Las filas en 0 no se graban.
//  Al cambiar el depósito en un movimiento nuevo se limpia el detalle (cbDepositos_ValueChanged).
//  Un traspaso no se modifica; sin permiso no se modifica ni elimina un movimiento de días anteriores.
//  Al grabar se ofrece el comprobante (ReporteVenta -> Reporte_Movimiento, @tipo 17).
// =====================================================================
import { useMemo, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, fechaCorta } from '../../componentes/Controles';
import { EfectoConfirmar } from '../../componentes/Efecto';
import NotaImpresa, { bsNota } from '../../componentes/NotaImpresa';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { NumeroCelda, errorDetalle } from '../ventas/Tec_Ventas';
import { SelectorProductos } from '../../componentes/SelectorProductos';
import { useAlActivar } from '../../global/vistaActiva';

const URL = '/api/inventario/movimientos';
const INGRESO = 4;
const TRASPASO = 8;
const OBS_RESETEO = 'RESETEO DE INVENTARIO A CERO';
const dos = (n) => String(n).padStart(2, '0');
const hoyLocal = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
let _k = 0;

export default function Tec_Movimientos() {
  const toast = useToast();
  const [c, setC] = useState({ depositos: [], tipos: [], lote: false, modificarAnteriores: false, hoy: '' });
  const [form, setForm] = useState({ Id: '', FechaDocumento: '', ConceptoId: '', DepositoId: '', DepositoIdDestino: '', Observacion: '' });
  const [detalle, setDetalle] = useState([]);
  const [selector, setSelector] = useState(false);
  const [cargandoDet, setCargandoDet] = useState(false);
  const [reseteo, setReseteo] = useState(false);
  const [nota, setNota] = useState(null);
  useAlActivar(() => { api.get(`${URL}/combos`).then((r) => setC(r.data)).catch((e) => toast(e.message, 'error')); });
  const vig = detalle.filter((d) => !d.eliminado);
  const tipo = Number(form.ConceptoId);
  const esIngreso = tipo === INGRESO;
  const esTraspaso = tipo === TRASPASO && c.depositos.length > 1;

  // ReporteVenta / ReporteToolStripMenuItem: comprobante del movimiento
  const verNota = async (id, grabada = null) => {
    try {
      const r = await api.get(`${URL}/${id}/comprobante`);
      if (!r.data.filas.length) { toast('No Existen Datos Para Mostrar', 'error'); return; }
      setNota({ ...r.data, grabada });
    } catch (e) { toast(e.message, 'error'); }
  };
  // EditarToolStripMenuItem / EliminarToolStripMenuItem
  const bloqueo = (accion) => (f) => {
    if (!f) return null;
    if (!c.modificarAnteriores && f.fdoc < (c.hoy || hoyLocal())) return `No puede ${accion} Movimientos Anteriores a la fecha de Hoy, Consulte con su Administrador`.toUpperCase();
    if (accion === 'Modificar' && Number(f.concep) === TRASPASO) return 'No Se puede Modificar un Movimiento de Traspaso. Elimine el Movimiento y Vuelva a Crearlo'.toUpperCase();
    return null;
  };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data.map((f) => ({ ...f, FechaTxt: fechaCorta(`${f.fdoc}T00:00:00Z`) })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 70, '', 'num'),
      new Celda('FechaTxt', true, 'Fecha Transacción', 110),
      new Celda('nconcep', true, 'Tipo Movimiento', 130),
      new Celda('obs', true, 'Descripción Movimiento', 250),
      new Celda('NombreDeposito', true, 'Depósito', 150),
    ],
    _PMOLimpiar: () => {
      setForm({ Id: '', FechaDocumento: c.hoy || hoyLocal(), ConceptoId: c.tipos[0]?.Id ?? '', DepositoId: c.depositos[0]?.Id ?? '', DepositoIdDestino: c.depositos[1]?.Id ?? '', Observacion: '' });
      setDetalle([]);
    },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, FechaDocumento: f.fdoc, ConceptoId: f.concep, DepositoId: f.alm, DepositoIdDestino: f.almDestino ?? '', Observacion: f.obs ?? '' });
      setCargandoDet(true);
      api.get(`${URL}/${f.Id}/detalle`).then((r) => setDetalle(r.data.map((d) => ({ ...d, key: ++_k })))).catch((e) => toast(e.message, 'error')).finally(() => setCargandoDet(false));
    },
    _PMOLimpiarErrores: () => {},
    _PMOBloqueoModificar: bloqueo('Modificar'),
    _PMOBloqueoEliminar: bloqueo('Eliminar'),
    _PMOValidarCampos: () => {
      if (!form.ConceptoId) { toast('Seleccione Tipo Movimiento'.toUpperCase(), 'error'); return false; }
      if (!form.DepositoId) { toast('Seleccion Deposito'.toUpperCase(), 'error'); return false; }
      if (esTraspaso && String(form.DepositoId) === String(form.DepositoIdDestino)) { toast('El deposito Destino debe ser Diferente al Origen'.toUpperCase(), 'error'); return false; }
      if (!vig.length) { toast('Por Favor Inserte un Detalle'.toUpperCase(), 'error'); return false; }
      const ed = errorDetalle(vig, errFila, (d) => d.Producto);
      if (ed) { toast(ed, 'error', 6000); return false; }
      if (!vig.some((d) => !d.nuevo || num(d.Cantidad) !== 0)) { toast('Ingrese la Cantidad de al menos un Producto'.toUpperCase(), 'error'); return false; }
      return true;
    },
    _PMOGrabarRegistro: async () => {
      const r = await api.post(URL, { ...form, detalle: vig.map((d) => ({ ProductoId: d.ProductoId, Cantidad: d.Cantidad })) });
      verNota(r.id, 'nueva');
      return r;
    },
    _PMOModificarRegistro: async () => {
      const r = await api.put(`${URL}/${form.Id}`, { ...form, detalle: detalle.filter((d) => !(d.nuevo && d.eliminado)).map((d) => (d.nuevo ? { nuevo: true, ProductoId: d.ProductoId, Cantidad: d.Cantidad } : { Id: d.Id, Cantidad: d.Cantidad, eliminado: !!d.eliminado })) });
      verNota(form.Id, 'modificada');
      return r;
    },
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Movimiento ${f.obs || f.Id} ?`,
  });
  const ro = !m.habilitado;
  const nuevo = m.modo === 'nuevo';
  // cbDepositos_ValueChanged: en un movimiento nuevo con productos se limpia el detalle
  const cambiarDeposito = (x) => {
    const nombre = c.depositos.find((d) => String(d.Id) === String(x))?.NombreDeposito ?? '';
    setForm((f) => ({ ...f, DepositoId: x }));
    if (nuevo && vig.length) { setDetalle([]); toast(`Cambio de deposito: se limpio el detalle. Vuelva a agregar los productos de ${nombre}`.toUpperCase(), 'ok', 4000); }
  };
  // el valor se aplica en cada tecla aunque sea invalido: la celda lo marca (errFila) y Grabar lo bloquea
  const cambiarCant = (key, txt) => setDetalle((d) => d.map((x) => (x.key !== key ? x : { ...x, Cantidad: num(txt), _editado: true })));
  const errFila = (x) => {
    if (!x._editado) return {};
    const n = num(x.Cantidad);
    if (!esIngreso && n < 0) return { Cantidad: 'La Cantidad debe ser Mayor a 0' };
    if (!esIngreso && x.stock !== undefined && n > num(x.stock) + 1e-9) return { Cantidad: `La cantidad que se quiere sacar es mayor a la que existe en el stock solo puede Sacar : ${n2(x.stock)}`.toUpperCase() };
    return {};
  };
  const enDetalle = useMemo(() => new Set(vig.map((d) => Number(d.ProductoId))), [vig]);
  // _prCalcularStockFinal: filas ya grabadas = stock actual; ingreso suma; los demas restan
  const final = (d) => (!d.nuevo ? num(d.stock) : esIngreso ? num(d.stock) + num(d.Cantidad) : num(d.stock) - num(d.Cantidad));
  const verStock = !ro && nuevo;

  // btnAgregarTodos: todos los productos del deposito con cantidad 0 (solo se escriben las cantidades)
  const agregarTodos = async () => {
    if (!form.DepositoId) { toast('Seleccione un Deposito'.toUpperCase(), 'error'); return; }
    try {
      const prods = (await api.get(`${URL}/productos/${form.DepositoId}`)).data;
      if (!prods.length) { toast('No hay productos en el deposito'.toUpperCase(), 'error'); return; }
      const nuevos = prods.filter((p) => !enDetalle.has(Number(p.Id)))
        .map((p) => ({ key: ++_k, nuevo: true, ProductoId: p.Id, Producto: p.DescripcionProducto || p.NombreProducto, Cantidad: 0, stock: p.stock }));
      setDetalle((d) => [...d.filter((x) => Number(x.ProductoId) > 0), ...nuevos]);
      toast(`${nuevos.length} productos agregados con cantidad 0. Escriba solo las cantidades.`.toUpperCase(), 'ok', 4000);
    } catch (e) { toast(e.message, 'error'); }
  };
  // ButtonX2 (reseteo): reemplaza el detalle con la cantidad para dejar cada stock en 0 (stock -5 => +5 ; 10 => -10)
  const resetear = async () => {
    setReseteo(false);
    try {
      const prods = (await api.get(`${URL}/productos/${form.DepositoId}`)).data.filter((p) => num(p.stock) !== 0);
      if (!prods.length) { toast('Todos los productos del deposito ya tienen stock 0'.toUpperCase(), 'error'); return; }
      setDetalle(prods.map((p) => ({ key: ++_k, nuevo: true, ProductoId: p.Id, Producto: p.DescripcionProducto || p.NombreProducto, Cantidad: -num(p.stock), stock: p.stock })));
      setForm((f) => ({ ...f, Observacion: f.Observacion.trim() ? f.Observacion : OBS_RESETEO }));
      toast(`${prods.length} lineas cargadas para resetear el stock a 0. Revise y Grabe.`.toUpperCase(), 'ok', 4000);
    } catch (e) { toast(e.message, 'error'); }
  };
  const pedirReseteo = () => {
    if (c.lote) { toast('El Reseteo no esta disponible con control por Lote activo'.toUpperCase(), 'error'); return; }
    if (!form.DepositoId) { toast('Seleccione un Deposito'.toUpperCase(), 'error'); return; }
    setReseteo(true);
  };
  const depNombre = c.depositos.find((d) => String(d.Id) === String(form.DepositoId))?.NombreDeposito ?? '';

  return (
    <>
    <ModeloF1 m={m} titulo="Ingreso y salida de productos" subtitulo="Gestion De Movimientos" icono="box"
      accionesRegistro={<button className="btn btn-sec" onClick={() => verNota(m.filaActual?.Id)} disabled={!m.filaActual || m.habilitado}><Icono n="printer" size={16} /> <span className="txt-btn">Comprobante</span></button>}
      accionesFila={(f) => [{ texto: 'Reporte', icono: 'printer', onClick: () => verNota(f.Id) }]}>
      {c.lote && !ro && <p className="aviso-lote"><Icono n="alert" size={16} /> El sistema usa lotes: registre los movimientos desde el escritorio.</p>}
      <div className="venta-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del movimiento <span className={`chip-mora ${esIngreso ? 'al-dia' : 'mora-alta'}`}>{esIngreso ? 'Ingreso' : tipo === TRASPASO ? 'Traspaso' : 'Salida'}</span></h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Fecha transacción" requerido><input type="date" className="control" value={form.FechaDocumento} onChange={(e) => setForm((f) => ({ ...f, FechaDocumento: e.target.value }))} readOnly={ro} disabled={ro} /></Campo>
            <Campo etiqueta="Tipo de movimiento" requerido>
              <ComboMulti dt={c.tipos} valor={form.ConceptoId} onChange={(x) => setForm((f) => ({ ...f, ConceptoId: x }))} id="Id" nameId="Cód." descripcionId="Descripcion" descripcion="Tipo Movimiento" deshabilitado={ro} />
            </Campo>
            <Campo etiqueta={esTraspaso ? 'Depósito origen' : 'Depósito'} requerido>
              <ComboMulti dt={c.depositos} valor={form.DepositoId} onChange={cambiarDeposito} id="Id" nameId="Cód." descripcionId="NombreDeposito" descripcion="Depósito" deshabilitado={ro} />
            </Campo>
            {/* solo en traspasos: al registrar se elige; al visualizar muestra el destino grabado (deshabilitado) */}
            {esTraspaso && (
              <Campo etiqueta="Depósito destino" requerido={!ro}>
                <ComboMulti dt={c.depositos} valor={form.DepositoIdDestino} onChange={(x) => setForm((f) => ({ ...f, DepositoIdDestino: x }))} id="Id" nameId="Cód." descripcionId="NombreDeposito" descripcion="Depósito destino" deshabilitado={ro} />
              </Campo>
            )}
            <Campo etiqueta="Descripción" ancho={2}><input className="control" value={form.Observacion} onChange={(e) => setForm((f) => ({ ...f, Observacion: e.target.value }))} readOnly={ro} maxLength={500} /></Campo>
          </div>
        </div>
        <div className="tarjeta venta-detalle">
          <div className="detalle-cab-venta">
            <h2 className="tarjeta-tit">Productos <span className="contador">{vig.length}</span></h2>
            {!ro && (
              <span className="barra">
                {nuevo && esIngreso && <button className="btn btn-sec" onClick={agregarTodos}><Icono n="list" size={16} /> Agregar todos</button>}
                {nuevo && esIngreso && <button className="btn btn-sec" onClick={pedirReseteo}><Icono n="refresh" size={16} /> Resetear a cero</button>}
                <button className="btn btn-pri" onClick={() => (form.DepositoId ? setSelector(true) : toast('Seleccion Deposito'.toUpperCase(), 'error'))}><Icono n="plus" size={16} /> Agregar producto</button>
              </span>
            )}
          </div>
          {cargandoDet ? <div className="cargando-bloque"><span className="spinner" /> Cargando detalle...</div> : (
            <div className="tabla-scroll">
              <table className="grid grid-tarjetas tabla-detalle">
                <thead><tr><th>Producto</th>{verStock && <th className="num">Stock actual</th>}<th className="num">Cantidad</th>{verStock && <th className="num">Stock final</th>}{!ro && <th aria-label="Quitar" />}</tr></thead>
                <tbody>
                  {vig.map((d) => (
                    <tr key={d.key} className={d.nuevo ? 'fila-nueva' : ''}>
                      <td data-label="Producto" className="det-producto"><b>{tit(d.Producto)}</b><small>Cód. {d.ProductoId}</small></td>
                      {verStock && <td data-label="Stock actual" className="num">{n2(d.stock)}</td>}
                      <td data-label="Cantidad" className="num">{ro ? n2(d.Cantidad) : <NumeroCelda valor={num(d.Cantidad)} onCambio={(x) => cambiarCant(d.key, x)} etiqueta={`Cantidad de ${d.Producto}`} error={errFila(d).Cantidad} />}</td>
                      {verStock && <td data-label="Stock final" className={`num ${final(d) < 0 ? 'neg' : ''}`}><b>{n2(final(d))}</b></td>}
                      {!ro && <td className="col-acc"><button className="btn-icono peligro" onClick={() => setDetalle((x) => x.map((y) => (y.key === d.key ? { ...y, eliminado: true } : y)))} aria-label={`Quitar ${d.Producto}`} title="Quitar"><Icono n="trash" size={16} /></button></td>}
                    </tr>
                  ))}
                  {!vig.length && <tr><td colSpan={5} className="vacio">{ro ? 'Sin productos' : 'Agregue productos con "Agregar producto"'}</td></tr>}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
      {selector && <SelectorProductos url={`${URL}/productos/${form.DepositoId}`} sinLimite={esIngreso} enDetalle={enDetalle} onCerrar={() => setSelector(false)}
        onAgregar={(p, cant) => setDetalle((d) => [...d, { key: ++_k, nuevo: true, ProductoId: p.Id, Producto: p.DescripcionProducto || p.NombreProducto, Cantidad: cant, stock: p.stock }])} />}
      <EfectoConfirmar abierto={reseteo} titulo="Confirmación de Reseteo" onCancelar={() => setReseteo(false)} onAceptar={resetear}
        descripcion={`¿Cargar TODOS los productos del deposito ${depNombre} con la cantidad necesaria para dejar su stock en 0? Se reemplazara el detalle actual.`} />
    </ModeloF1>
    {/* fuera del registro: tambien se abre desde la lista y despues de grabar */}
    {nota && <NotaMovimiento datos={nota} onCerrar={() => setNota(null)} />}
    </>
  );
}

// P_GenerarReporte (Reporte_Movimiento): columnas de MAM_Movimientos @tipo 17
function NotaMovimiento({ datos, onCerrar }) {
  const { filas, cab = {} } = datos;
  const total = filas.reduce((a, f) => a + num(f.Cantidad), 0);
  return (
    <NotaImpresa titulo="Comprobante de Movimiento" id={cab.Id} fecha={cab.Fecha} empresa={datos.empresa} imagenEmpresa={datos.imagenEmpresa} onCerrar={onCerrar}
      grabada={datos.grabada ? `Movimiento #${cab.Id} ${datos.grabada === 'nueva' ? 'grabado' : 'modificado'} con éxito.` : null}
      cabecera={[
        ['Fecha:', cab.Fecha ? fechaCorta(`${String(cab.Fecha).slice(0, 10)}T00:00:00Z`) : ''], ['Cod. Movimiento:', String(cab.Id ?? '')],
        ['Tipo Movimiento:', tit(cab.Tipo || '')], ['Depósito:', tit(cab.Deposito || '')],
        ...(cab.DepositoDestino ? [['Depósito Destino:', tit(cab.DepositoDestino)]] : []),
        ['Descripción:', cab.Observacion ?? ''],
      ]}
      columnas={[{ titulo: 'Item', ancho: '12%', valor: (f) => f.Item }, { titulo: 'Producto', valor: (f) => f.Producto }, { titulo: 'Cantidad', num: true, valor: (f) => bsNota(f.Cantidad) }]}
      filas={filas}
      pie={[['Productos:', String(filas.length)]]}
      etiquetaTotal="Total General:" total={total} formatoTotal={bsNota} literal={false}
      firmas={['Encargado Almacen', 'Recibido']} />
  );
}
