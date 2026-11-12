// =====================================================================
//  KAILY: Tec_Productos -> "Reporte Precios" (ButtonX4) y "Reporte Precios Imagenes" (btnReporteConImagenes)
//  Dialogo = AyudaMultiCategoria (Efecto tipo 23): Sucursal, Categoria Precio y categorias a incluir.
//  Hoja = Reporte_ProductosPrecios.rpt / Reporte_ProductoImagenes.rpt: agrupado por categoria (A-Z),
//  columnas Cod. Producto | Descripcion | Unidad Venta | Industria | Precio (o Imagen en lugar del codigo).
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import { createPortal } from 'react-dom';
import { api, urlImagen } from '../../api/http';
import { useSesion } from '../../global/Sesion';
import { esc } from '../../global/imprimir';
import { imprimirReporte } from '../../componentes/ReporteImpreso';
import { useToast } from '../../componentes/Toast';
import Icono from '../../componentes/Icono';

const URL = '/api/configuracion/productos/reporte-precios';
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const tit = (s) => String(s || '').toLowerCase().replace(/(^|\s)\S/g, (c) => c.toUpperCase());

const CSS_PRECIOS = `
.rp td.rp-img { width: 40mm; padding: 3px 5px; }
.rp td.rp-img img { display: block; width: 38mm; height: 28mm; object-fit: contain; }
.rp td.rp-img .rp-sinimg { display: flex; align-items: center; justify-content: center; width: 38mm; height: 28mm; border: 1px dashed #d1d5db; color: #9ca3af; font-size: 9px; }
.rp td.rp-precio { font-weight: 700; }
`;

// GroupHeaderSection1 "Categoria Producto:" + Section3 (detalle); sin subtotales (GroupFooter vacio)
function cuerpoReporte(filas, conImagenes) {
  const grupos = new Map();
  filas.forEach((f) => { const k = String(f.NombreCategoria || '').trim(); if (!grupos.has(k)) grupos.set(k, []); grupos.get(k).push(f); });
  return [...grupos.keys()].sort((a, b) => a.localeCompare(b, 'es')).map((k, i) => {
    const g = grupos.get(k);
    const det = g.map((f) => {
      const src = conImagenes && f.Rutaimg ? urlImagen('producto', f.Rutaimg) : null;
      const primera = conImagenes
        ? `<td class="rp-img">${src ? `<img src="${esc(src)}" alt="" onerror="this.outerHTML='<span class=&quot;rp-sinimg&quot;>Sin imagen</span>'">` : '<span class="rp-sinimg">Sin imagen</span>'}</td>`
        : `<td class="nw">${esc(f.CodigoExterno)}</td>`;
      return `<tr>${primera}<td>${esc(f.DescripcionProducto)}</td><td>${esc(f.unidad)}</td><td>${esc(f.industria)}</td><td class="num rp-precio">${esc(bs(f.Precio))}</td></tr>`;
    }).join('');
    return `${i ? '<tr class="rp-espacio"><td colspan="5"></td></tr>' : ''}<tr class="rp-grupo rp-n0"><td colspan="5"><b>Categoria Producto: ${esc(k)}</b><small>${g.length} producto${g.length === 1 ? '' : 's'}</small></td></tr>${det}`;
  }).join('');
}

export default function ReportePreciosProductos({ conImagenes, onCerrar }) {
  const toast = useToast();
  const { sesion } = useSesion();
  const [combos, setCombos] = useState(null);
  const [sucursal, setSucursal] = useState('');
  const [precio, setPrecio] = useState('');
  const [marcadas, setMarcadas] = useState(() => new Set());
  const [q, setQ] = useState('');
  const [cargando, setCargando] = useState(false);

  // FormularioAyuda_Load: primer item de cada combo
  useEffect(() => {
    api.get(`${URL}/combos`).then((r) => {
      setCombos(r.data);
      setSucursal(r.data.sucursales[0]?.aanumi ?? '');
      setPrecio(r.data.categoriasPrecio[0]?.ygnumi ?? '');
    }).catch((e) => { toast(e.message, 'error'); onCerrar(); });
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const visibles = useMemo(() => (combos?.categorias || []).filter((c) => String(c.Nombre || '').toUpperCase().includes(q.toUpperCase())), [combos, q]);
  const todas = combos?.categorias?.length > 0 && combos.categorias.every((c) => marcadas.has(c.id));
  const alternar = (id) => setMarcadas((s) => { const n = new Set(s); if (n.has(id)) n.delete(id); else n.add(id); return n; });
  // ButtonX2: marca todas o, si ya estaban todas, ninguna
  const alternarTodas = () => setMarcadas(todas ? new Set() : new Set(combos.categorias.map((c) => c.id)));

  // ButtonX1: al menos una categoria -> GenerarReporte
  const aceptar = async () => {
    if (!marcadas.size) { toast('Debe Seleccionar Al Menos una Categoria', 'error'); return; }
    setCargando(true);
    try {
      const qs = new URLSearchParams({ categoriaPrecio: precio, sucursal, categorias: [...marcadas].join(','), imagenes: conImagenes ? '1' : '0' });
      const { filas, cabecera } = (await api.get(`${URL}?${qs}`)).data;
      const nombreSuc = combos.sucursales.find((s) => String(s.aanumi) === String(sucursal))?.aabdes ?? '';
      const nombrePrecio = combos.categoriasPrecio.find((c) => String(c.ygnumi) === String(precio))?.Nombre ?? '';
      imprimirReporte({
        titulo: conImagenes ? 'LISTADO DE PRODUCTOS' : 'LISTADO DE PRECIOS DE PRODUCTOS',
        usuario: tit(sesion?.gs_user), empresa: { Nombre: cabecera.Nombre, Direccion: cabecera.Direccion }, imagenEmpresa: cabecera.imagenEmpresa,
        datos: [['Sucursal:', nombreSuc], ['Categoría precio:', nombrePrecio]],
        columnas: [
          conImagenes ? { campo: 'Rutaimg', titulo: 'Imagen Producto', ancho: '42mm' } : { campo: 'CodigoExterno', titulo: 'Cod. Producto', ancho: '14%' },
          { campo: 'DescripcionProducto', titulo: 'Descripcion Producto' },
          { campo: 'unidad', titulo: 'Unidad Venta', ancho: '14%' },
          { campo: 'industria', titulo: 'Industria', ancho: '16%' },
          { campo: 'Precio', titulo: 'Precio', tipo: 'bs', ancho: '11%' },
        ],
        filas, unidad: 'producto', cuerpoHtml: filas.length ? cuerpoReporte(filas, conImagenes) : '', css: CSS_PRECIOS,
      });
      onCerrar();
    } catch (e) { toast(e.message, 'error'); } finally { setCargando(false); }
  };

  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-lg" role="dialog" aria-modal="true" aria-label="Reporte de precios" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') onCerrar(); }}>
        <div className="modal-cab"><h3>{conImagenes ? 'SELECCIONE CATEGORIA PRECIO' : 'SELECCIONE DATOS'}</h3><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        {!combos ? <div className="vacio"><span className="spinner" /> Cargando...</div> : (
          <>
            <div className="filtros-lista">
              <label className="filtro"><span>Sucursal</span>
                <select className="control" value={sucursal} onChange={(e) => setSucursal(e.target.value)}>
                  {combos.sucursales.map((s) => <option key={s.aanumi} value={s.aanumi}>{s.aabdes}</option>)}
                </select>
              </label>
              <label className="filtro"><span>Categoría precio</span>
                <select className="control" value={precio} onChange={(e) => setPrecio(e.target.value)}>
                  {combos.categoriasPrecio.map((c) => <option key={c.ygnumi} value={c.ygnumi}>{c.Nombre}</option>)}
                </select>
              </label>
            </div>
            <div className="buscar"><Icono n="search" size={16} /><input autoFocus className="control" placeholder="Buscar categoría..." value={q} onChange={(e) => setQ(e.target.value)} aria-label="Buscar categoría" /></div>
            <div className="tabla-scroll alto-ayuda">
              <table className="grid">
                <thead><tr><th>Categoría</th><th className="centro" style={{ width: 130 }}>Seleccionar</th></tr></thead>
                <tbody>
                  {visibles.map((c) => (
                    <tr key={c.id} className={marcadas.has(c.id) ? 'sel' : ''} onClick={() => alternar(c.id)}>
                      <td>{c.Nombre}</td>
                      <td className="centro"><input type="checkbox" checked={marcadas.has(c.id)} onChange={() => alternar(c.id)} onClick={(e) => e.stopPropagation()} aria-label={`Seleccionar ${c.Nombre}`} /></td>
                    </tr>
                  ))}
                  {!visibles.length && <tr><td colSpan={2} className="vacio">Sin categorías</td></tr>}
                </tbody>
              </table>
            </div>
          </>
        )}
        <div className="modal-acciones">
          <button className="btn btn-txt" onClick={alternarTodas} disabled={!combos}><Icono n="list" size={15} /> {todas ? 'Quitar todas' : 'Seleccionar todas'}</button>
          <span className="ayuda-tip">{marcadas.size} seleccionada(s)</span>
          <button className="btn btn-sec" onClick={onCerrar}>Salir</button>
          <button className="btn btn-pri" onClick={aceptar} disabled={!combos || cargando}>{cargando ? <span className="spinner" /> : <Icono n="printer" size={15} />} Aceptar</button>
        </div>
      </div>
    </div>,
    document.body,
  );
}
