// =====================================================================
//  KAILY: Tec_Kits.vb -> Gestion De Kits (productos agrupados)   SP: MAM_Kits, TVP KitsProductos
//  El servidor arma la tabla KitsProductos (estado 0/1/2/-1), valida precio >= costo y calcula el Total.
//  Aqui solo se editan Cantidad y Precio; el Total que se muestra es referencial.
// =====================================================================
import { useRef, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, SwitchButton } from '../../componentes/Controles';
import { EfectoAyuda } from '../../componentes/Efecto';
import { Celda } from '../../global/Celda';
import { api } from '../../api/http';
import { useToast } from '../../componentes/Toast';
import Icono from '../../componentes/Icono';

const URL = '/api/configuracion/kits';
const VACIO = { Id: '', NombreKit: '', DescripcionKit: '', Estado: true };
const COLS_AYUDA = [
  { campo: 'ProductoId', titulo: 'Código', tamano: 80 },
  { campo: 'NombreProducto', titulo: 'Producto', tamano: 280 },
  { campo: 'NombreCategoria', titulo: 'Categoría', tamano: 160 },
  { campo: 'PrecioVenta', titulo: 'Precio venta', tamano: 100 },
];
const n2 = (x) => (Number(x) || 0).toFixed(2);

export default function Tec_Kits() {
  const toast = useToast();
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  // [{ key, Id?, nuevo?, ProductoId, NombreProducto, Cantidad, Precio, PrecioVentaBackup }]
  const [productos, setProductos] = useState([]);
  const [ayuda, setAyuda] = useState({ abierto: false, dt: [] });
  const pedido = useRef(0);
  const clave = useRef(0);

  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };

  const cargarProductos = async (id) => {
    const n = ++pedido.current;
    setProductos([]);
    try {
      const r = await api.get(`${URL}/${id}/productos`);
      if (n === pedido.current) {
        setProductos(r.data.map((p) => ({ key: `e${p.Id}`, Id: p.Id, ProductoId: p.ProductoId, NombreProducto: p.NombreProducto,
          Cantidad: String(p.Cantidad ?? 1), Precio: String(p.Precio ?? 0), PrecioVentaBackup: p.PrecioVentaBackup })));
      }
    } catch (e) { toast(e.message, 'error'); }
  };

  // btnAgregarProducto -> ListarProductosKits
  const abrirAyuda = async () => {
    try {
      const r = await api.get(`${URL}/productos`);
      const usados = new Set(productos.map((p) => Number(p.ProductoId)));
      setAyuda({ abierto: true, dt: r.data.filter((p) => !usados.has(Number(p.ProductoId))) });
    } catch (e) { toast(e.message, 'error'); }
  };
  const agregar = (p) => {
    setAyuda({ abierto: false, dt: [] });
    if (productos.some((x) => Number(x.ProductoId) === Number(p.ProductoId))) { toast(`El producto ${p.NombreProducto} ya esta en el kit`, 'error'); return; }
    clave.current += 1;
    setProductos((ps) => [...ps, { key: `n${clave.current}`, nuevo: true, ProductoId: p.ProductoId, NombreProducto: p.NombreProducto,
      Cantidad: '1', Precio: String(p.PrecioVenta ?? 0), PrecioVentaBackup: p.PrecioVenta }]);
    quitarError('productos');
  };
  const editar = (i, k, v) => setProductos((ps) => ps.map((p, j) => (j === i ? { ...p, [k]: v } : p)));
  // grProductos_CellEdited: cantidad no valida o <= 0 -> 1; precio vacio -> precio de venta
  const normalizar = (i) => setProductos((ps) => ps.map((p, j) => {
    if (j !== i) return p;
    const c = Number.parseFloat(p.Cantidad);
    const pr = Number.parseFloat(p.Precio);
    return { ...p, Cantidad: Number.isFinite(c) && c > 0 ? String(c) : '1', Precio: Number.isFinite(pr) ? String(pr) : String(p.PrecioVentaBackup ?? 0) };
  }));
  const quitar = (i) => setProductos((ps) => ps.filter((_, j) => j !== i));
  const total = productos.reduce((s, p) => s + (Number(p.Cantidad) || 0) * (Number(p.Precio) || 0), 0);

  const _datos = () => ({
    NombreKit: form.NombreKit, DescripcionKit: form.DescripcionKit, Estado: form.Estado ? 1 : 0,
    productos: productos.map((p) => (p.nuevo
      ? { nuevo: true, ProductoId: p.ProductoId, Cantidad: Number(p.Cantidad), Precio: Number(p.Precio) }
      : { Id: p.Id, Cantidad: Number(p.Cantidad), Precio: Number(p.Precio) })),
  });

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data,
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombreKit', true, 'Kit', 240),
      new Celda('DescripcionKit', true, 'Descripción', 260),
      new Celda('Total', true, 'Total', 100, '0.00', 'num'),
      new Celda('Estado', true, 'Estado', 90, 'ACTIVO', 'bool'),
    ],
    _PMOLimpiar: () => {
      setForm(VACIO);
      pedido.current++;
      setProductos([]);
    },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, NombreKit: f.NombreKit ?? '', DescripcionKit: f.DescripcionKit ?? '', Estado: !!f.Estado });
      cargarProductos(f.Id);
    },
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombreKit.trim()) e.NombreKit = 'Ingrese Nombre del Kit';
      if (!productos.length) e.productos = 'Agregue al menos un producto al kit';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, _datos()),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, _datos()),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Kit ${f.NombreKit} ?`,
  });
  const ro = !m.habilitado;

  return (
    <ModeloF1 m={m} titulo="Kits" subtitulo="Gestion De Kits" icono="box">
      <div className="tarjeta">
        <h2 className="tarjeta-tit">Datos del kit</h2>
        <div className="campos">
          <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
          <Campo etiqueta="Nombre del kit" ancho={2} error={errores.NombreKit} requerido>
            <input className="control" value={form.NombreKit} onChange={set('NombreKit')} readOnly={ro} maxLength={200} />
          </Campo>
          <Campo etiqueta="Descripción" ancho={3}>
            <input className="control" value={form.DescripcionKit} onChange={set('DescripcionKit')} readOnly={ro} maxLength={500} />
          </Campo>
          <Campo etiqueta="Total"><input className="control num" value={n2(total)} readOnly /></Campo>
          <Campo etiqueta="Estado"><SwitchButton valor={!!form.Estado} onChange={set('Estado')} deshabilitado={ro} /></Campo>
        </div>
      </div>
      <div className="tarjeta">
        <div className="detalle-cab-venta">
          <h2 className="tarjeta-tit">Productos del kit</h2>
          {!ro && <button type="button" className="btn btn-sec" onClick={abrirAyuda}><Icono n="plus" size={16} /> Agregar producto</button>}
        </div>
        {errores.productos && <p className="campo-error">{errores.productos}</p>}
        {productos.length ? (
          <div className="tabla-scroll">
            <table className="grid tabla-detalle">
              <thead><tr><th>Código</th><th>Producto</th><th className="num">Cantidad</th><th className="num">Precio</th><th className="num">SubTotal</th>{!ro && <th />}</tr></thead>
              <tbody>
                {productos.map((p, i) => (
                  <tr key={p.key}>
                    <td>{p.ProductoId}</td>
                    <td>{p.NombreProducto}</td>
                    <td className="num">
                      {ro ? n2(p.Cantidad) : (
                        <input className="control control-celda num" type="number" min="0" step="any" inputMode="decimal" aria-label={`Cantidad ${p.NombreProducto}`}
                          value={p.Cantidad} onChange={(e) => editar(i, 'Cantidad', e.target.value)} onBlur={() => normalizar(i)} />
                      )}
                    </td>
                    <td className="num">
                      {ro ? n2(p.Precio) : (
                        <input className="control control-celda num" type="number" min="0" step="any" inputMode="decimal" aria-label={`Precio ${p.NombreProducto}`}
                          value={p.Precio} onChange={(e) => editar(i, 'Precio', e.target.value)} onBlur={() => normalizar(i)} />
                      )}
                    </td>
                    <td className="num">{n2((Number(p.Cantidad) || 0) * (Number(p.Precio) || 0))}</td>
                    {!ro && (
                      <td><button type="button" className="btn-icono" aria-label={`Quitar ${p.NombreProducto}`} onClick={() => quitar(i)}><Icono n="x" size={16} /></button></td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : <p className="ayuda-tip">El kit no tiene productos.</p>}
      </div>
      <EfectoAyuda abierto={ayuda.abierto} titulo="Seleccione un producto" dt={ayuda.dt} columnas={COLS_AYUDA}
        onSeleccionar={agregar} onCancelar={() => setAyuda({ abierto: false, dt: [] })} />
    </ModeloF1>
  );
}
