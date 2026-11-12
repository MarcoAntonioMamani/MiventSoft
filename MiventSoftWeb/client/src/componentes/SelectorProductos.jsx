// =====================================================================
//  Buscador de productos para agregar al detalle (antes en Tec_Despachos / Tec_DespachoDetalle).
//  Busca por varias palabras (codigo + nombre + descripcion + categoria). Cantidad en unidades.
//  Lo usa Tec_Movimientos (sinLimite = ingreso sin tope de stock).
// =====================================================================
import { useDeferredValue, useEffect, useMemo, useState } from 'react';
import { createPortal } from 'react-dom';
import { useToast } from './Toast';
import Icono from './Icono';
import { useListaTeclado, FilaMas, AYUDA_ELEGIR } from './useListaTeclado';
import { titulo as tit } from '../global/programas';
import { api } from '../api/http';
import { palabrasDe, indexar, filtrarIndice } from '../global/buscar';

const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };

export function SelectorProductos({ url, enDetalle, onAgregar, onCerrar, sinLimite = false }) {
  const toast = useToast();
  const [productos, setProductos] = useState(null);
  const [q, setQ] = useState('');
  const [elegido, setElegido] = useState(null);
  const [agregados, setAgregados] = useState(0); // cuantos se agregaron en esta ventana
  useEffect(() => { api.get(url).then((r) => setProductos(r.data)).catch((e) => { toast(e.message, 'error'); onCerrar(); }); }, []); // eslint-disable-line react-hooks/exhaustive-deps
  // texto de busqueda normalizado una sola vez por carga; la lista se filtra sin trabar lo que se escribe
  const indice = useMemo(() => indexar(productos, (p) => `${p.CodigoExterno ?? ''} ${p.NombreProducto ?? ''} ${p.DescripcionProducto ?? ''} ${p.NombreCategoria ?? ''}`), [productos]);
  const qBusca = useDeferredValue(q);
  const filtrados = useMemo(() => filtrarIndice(indice, palabrasDe(qBusca)).filter((p) => !enDetalle.has(Number(p.Id))), [indice, qBusca, enDetalle]);
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
            <thead><tr><th>Código</th><th>Producto</th><th>Categoría</th><th className="num">Stock</th></tr></thead>
            <tbody ref={nav.tbodyRef}>
              {nav.visibles.map((p, i) => (
                <tr key={p.Id} className={`fila-clic ${i === nav.marcada ? 'marcada' : ''} ${num(p.stock) <= 0 ? 'sin-stock' : ''}`} {...nav.propsFila(i)}>
                  <td data-label="Código">{p.CodigoExterno}</td>
                  <td data-label="Producto"><b>{tit(p.NombreProducto)}</b></td>
                  <td data-label="Categoría">{tit(p.NombreCategoria)}</td>
                  <td data-label="Stock" className="num">{num(p.stock) <= 0 ? <span className="badge badge-mal">Sin stock</span> : n2(p.stock)}</td>
                </tr>
              ))}
              <FilaMas nav={nav} columnas={4} />
              {!filtrados.length && <tr><td colSpan={4} className="vacio">{productos ? 'Sin productos' : 'Cargando productos...'}</td></tr>}
            </tbody>
          </table>
        </div>
        <div className="modal-acciones"><span className="ayuda-tip"><span className="solo-teclado"><kbd>↓</kbd><kbd>↑</kbd> elegir · <kbd>Enter</kbd> cantidad · </span>{AYUDA_ELEGIR}</span><button className="btn btn-pri" onClick={onCerrar}>Listo</button></div>
      </div>
      {elegido && <CantidadProducto sinLimite={sinLimite} producto={elegido} onCancelar={() => { setElegido(null); nav.volver(false); }}
        onAceptar={(cant) => { onAgregar(elegido, cant); setAgregados((x) => x + 1); setElegido(null); setQ(''); nav.volver(true); }} />}
    </div>,
    document.body,
  );
}

// Efecto tipo 5: cantidad > 0 y <= stock (salvo sinLimite)
function CantidadProducto({ producto, onAceptar, onCancelar, sinLimite = false }) {
  const stock = num(producto.stock);
  const [cant, setCant] = useState('1');
  const [err, setErr] = useState('');
  const validar = () => {
    const c = num(cant);
    if (!(c > 0)) { setErr('La Cantidad debe ser Mayor a 0'); return; }
    if (!sinLimite && c > stock + 1e-9) { setErr(`La cantidad es Superior Al Stock Disponible = ${n2(stock)}`); return; }
    onAceptar(c);
  };
  return (
    <div className="modal-fondo modal-encima" onMouseDown={(e) => { e.stopPropagation(); onCancelar(); }}>
      <div className="modal modal-sm cantidad-cajas" role="dialog" aria-modal="true" aria-label="Cantidad" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') { e.stopPropagation(); onCancelar(); } if (e.key === 'Enter') { e.preventDefault(); validar(); } }}>
        <h3>{tit(producto.DescripcionProducto || producto.NombreProducto)}</h3>
        <p className="cc-stock">Stock: <b>{n2(stock)}</b></p>
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

export default SelectorProductos;
