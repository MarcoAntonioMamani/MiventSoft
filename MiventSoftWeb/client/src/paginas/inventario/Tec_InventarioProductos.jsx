// =====================================================================
//  FormularioStock.vb -> Inventario productos (pestaña fija "Inventario" del escritorio). Solo lectura.
//  SP: MAM_Movimientos @tipo 18 (@CategoriaPrecio, @ProveedorId) · combos sp_Mam_Precios @tipo 9 / MAM_Compras @tipo 12
//  Buscar (codigo, proveedor, descripcion, categoria), Actualizar stock, Imprimir reporte y Exportar Excel.
//  Stock general resaltado; en rojo cuando es <= 0 (GridEXFormatCondition de VB).
// =====================================================================
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';

const URL = '/api/inventario/stock';
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

export default function Tec_InventarioProductos() {
  const toast = useToast();
  const [cats, setCats] = useState([]);
  const [provs, setProvs] = useState([]);
  const [cat, setCat] = useState('');
  const [prov, setProv] = useState(-1);
  const [filas, setFilas] = useState(null);
  const [cargando, setCargando] = useState(false);
  const pedido = useRef(0);

  useEffect(() => {
    api.get(`${URL}/combos`).then((r) => {
      setCats(r.data.categorias);
      setProvs(r.data.proveedores);
      setCat(r.data.categorias[0]?.Id ?? '');
    }).catch((e) => toast(e.message, 'error'));
  }, [toast]);

  const cargar = useCallback(async () => {
    if (!cat) return;
    const n = ++pedido.current;
    setCargando(true);
    try {
      const r = await api.get(`${URL}?${new URLSearchParams({ categoria: cat, proveedor: prov })}`);
      if (n !== pedido.current) return;
      setFilas(r.data);
      if (!r.data.length) toast('No Existen Datos Para Mostrar con Los Filtros Seleccionados'.toUpperCase(), 'error');
    } catch (e) {
      if (n === pedido.current) toast(e.message, 'error');
    } finally {
      if (n === pedido.current) setCargando(false);
    }
  }, [cat, prov, toast]);

  useEffect(() => { cargar(); }, [cargar]);

  const resumen = useMemo(() => {
    const f = filas || [];
    return { total: f.length, sinStock: f.filter((x) => x.stockGeneral <= 0).length, bajoMin: f.filter((x) => x.stockGeneral > 0 && x.stockGeneral <= x.stockMinimo).length };
  }, [filas]);

  const columnas = useMemo(() => [
    { campo: 'CodigoExterno', titulo: 'Cod. Externo' },
    { campo: 'NombreProveedor', titulo: 'Proveedor', render: (x) => tit(x.NombreProveedor) },
    { campo: 'DescripcionProducto', titulo: 'Descripción', ancho: 260, render: (x) => <b>{x.DescripcionProducto}</b> },
    { campo: 'Categoria', titulo: 'Categoría', render: (x) => tit(x.Categoria) },
    { campo: 'precio', titulo: 'Precio', tipo: 'num' },
    { campo: 'stockMinimo', titulo: 'Stock mínimo', tipo: 'num' },
    { campo: 'stockGeneral', titulo: 'Stock general', tipo: 'num', total: true, render: (x) => <b>{n2(x.stockGeneral)}</b>,
      clase: (x) => `stock-gen ${x.stockGeneral <= 0 ? 'neg' : ''}` },
  ], []);

  const catNombre = cats.find((c) => String(c.Id) === String(cat))?.Nombre;
  const provNombre = provs.find((p) => String(p.Id) === String(prov))?.Nombre;
  const sub = [`Categoría precio: ${tit(catNombre || '')}`, `Proveedor: ${tit(provNombre || 'TODOS')}`].join(' · ');

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo"><span className="modelo-icono"><Icono n="box" size={22} /></span><div><h1>Inventario productos</h1><p>Stock general de productos</p></div></div>
        <div className="barra">
          <button className="btn btn-pri" onClick={cargar} disabled={cargando || !cat}>{cargando ? <span className="spinner" /> : <Icono n="refresh" size={16} />} Actualizar stock</button>
        </div>
      </header>
      <div className="modelo-cuerpo">
        <div className="filtros-lista filtros-reporte">
          <div className="filtro filtro-ancho"><span>Proveedor</span>
            <ComboMulti dt={provs} valor={prov} onChange={(x) => setProv(x ?? -1)} id="Id" nameId="Cód." descripcionId="Nombre" descripcion="Proveedor" />
          </div>
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
