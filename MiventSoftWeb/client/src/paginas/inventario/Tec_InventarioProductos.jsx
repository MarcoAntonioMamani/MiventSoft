// =====================================================================
//  FormularioStock.vb -> Inventario productos (pestaña fija "Inventario" del escritorio). Solo lectura.
//  KAILY: SP MAM_Movimientos @tipo 18 (@CategoriaPrecio) · combo sp_Mam_Precios @tipo 8 (sin filtro de proveedor)
//  Buscar (codigo, descripcion, categoria), Actualizar stock, Imprimir reporte y Exportar Excel.
//  Stock general resaltado; en rojo cuando es <= 0 (GridEXFormatCondition de VB).
// =====================================================================
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { useAlActivar, useRefrescar } from '../../global/vistaActiva';

const URL = '/api/inventario/stock';
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

export default function Tec_InventarioProductos() {
  const toast = useToast();
  const [cats, setCats] = useState([]);
  const [cat, setCat] = useState('');
  const [filas, setFilas] = useState(null);
  const [cargando, setCargando] = useState(false);
  const pedido = useRef(0);

  useAlActivar(() => {
    api.get(`${URL}/combos`).then((r) => {
      setCats(r.data.categorias);
      setCat((c) => (c !== '' ? c : r.data.categorias[0]?.Id ?? ''));
    }).catch((e) => toast(e.message, 'error'));
  });

  const cargar = useCallback(async () => {
    if (!cat) return;
    const n = ++pedido.current;
    setCargando(true);
    try {
      const r = await api.get(`${URL}?${new URLSearchParams({ categoria: cat })}`);
      if (n !== pedido.current) return;
      setFilas(r.data);
      if (!r.data.length) toast('No Existen Datos Para Mostrar con Los Filtros Seleccionados'.toUpperCase(), 'error');
    } catch (e) {
      if (n === pedido.current) toast(e.message, 'error');
    } finally {
      if (n === pedido.current) setCargando(false);
    }
  }, [cat, toast]);

  useEffect(() => { cargar(); }, [cargar]);
  useAlActivar(cargar, { alMontar: false }); // stock actualizado al volver a la pestaña y con Actualizar
  const refrescar = useRefrescar(cargar); // boton Actualizar stock: categorias + stock de la BD

  const resumen = useMemo(() => {
    const f = filas || [];
    return { total: f.length, sinStock: f.filter((x) => x.stockGeneral <= 0).length, bajoMin: f.filter((x) => x.stockGeneral > 0 && x.stockGeneral <= x.stockMinimo).length };
  }, [filas]);

  const columnas = useMemo(() => [
    { campo: 'CodigoExterno', titulo: 'Cod. Externo' },
    { campo: 'Categoria', titulo: 'Categoría', render: (x) => tit(x.Categoria) },
    { campo: 'unidad', titulo: 'Unidad venta', render: (x) => tit(x.unidad) },
    { campo: 'DescripcionProducto', titulo: 'Descripción', ancho: 240, render: (x) => <b>{x.DescripcionProducto}</b> },
    { campo: 'stockMinimo', titulo: 'Stock mínimo', tipo: 'num' },
    { campo: 'precio', titulo: 'Precio', tipo: 'num' },
    // stock por depósito (texto de varias líneas en el escritorio) o un número
    { campo: 'stock', titulo: 'Stock', ...(typeof filas?.[0]?.stock === 'number' ? { tipo: 'num' } : { ancho: 200, render: (x) => <span style={{ whiteSpace: 'pre-line' }}>{x.stock}</span> }) },
    { campo: 'stockGeneral', titulo: 'Stock general', tipo: 'num', total: true, render: (x) => <b>{n2(x.stockGeneral)}</b>,
      clase: (x) => `stock-gen ${x.stockGeneral <= 0 ? 'neg' : ''}` },
  ], [filas]);

  const catNombre = cats.find((c) => String(c.Id) === String(cat))?.Nombre;
  const sub = `Categoría precio: ${tit(catNombre || '')}`;

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo"><span className="modelo-icono"><Icono n="box" size={22} /></span><div><h1>Inventario productos</h1><p>Stock general de productos</p></div></div>
        <div className="barra">
          <button className="btn btn-pri" onClick={refrescar} disabled={cargando || !cat}>{cargando ? <span className="spinner" /> : <Icono n="refresh" size={16} />} Actualizar stock</button>
        </div>
      </header>
      <div className="modelo-cuerpo">
        <div className="filtros-lista filtros-reporte">
          <div className="filtro filtro-ancho"><span>Categoría precio</span>
            <ComboMulti dt={cats} valor={cat} onChange={(x) => x && setCat(x)} id="Id" nameId="Cód." descripcionId="Nombre" descripcion="Cat. Precio" />
          </div>
        </div>

        {filas && (
          <div className="inv-resumen">
            <div><span>Productos</span><b>{resumen.total}</b></div>
            <div className="aviso"><span>Bajo stock mínimo</span><b>{resumen.bajoMin}</b></div>
            <div className="mal"><span>Sin stock</span><b>{resumen.sinStock}</b></div>
          </div>
        )}
        {filas
          ? <TablaReporte titulo="INVENTARIO PRODUCTOS - STOCK GENERAL" subtitulo={sub} columnas={columnas} filas={filas} cargando={cargando} claveFila="Id" vacio="No Existen Datos Para Mostrar" />
          : <div className="cargando-bloque"><span className="spinner" /> Cargando...</div>}
      </div>
    </section>
  );
}
