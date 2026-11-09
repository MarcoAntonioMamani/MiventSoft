// =====================================================================
//  Reportes de Compras
//   historico            Tec_HistoricoPreciosCompra  (una columna de cantidad y otra de precio costo por producto)
//   compras-por-producto FReporteCompraProducto      (todos los productos o uno)
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { columnasDe } from '../ventas/Reportes';

const URL = '/api/compras/reportes';
const dos = (n) => String(n).padStart(2, '0');
const iso = (d) => `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`;
const hoy = () => iso(new Date());
const primeroMes = () => { const d = new Date(); return iso(new Date(d.getFullYear(), d.getMonth(), 1)); };

const CONFIG = {
  historico: { titulo: 'Histórico de compras', sub: 'Historico De Precios De Compra', icono: 'history' },
  'compras-por-producto': { titulo: 'Compras por producto', sub: 'Reporte De Compras Por Productos', icono: 'bars' },
};

export default function ReporteCompras({ clave }) {
  const cfg = CONFIG[clave];
  const toast = useToast();
  const [f, setF] = useState({ desde: primeroMes(), hasta: hoy(), todos: true, producto: '' });
  const [prods, setProds] = useState([]);
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(false);

  useEffect(() => {
    setDatos(null);
    if (clave === 'compras-por-producto') api.get(`${URL}/compras-por-producto/productos`).then((r) => setProds(r.data)).catch((e) => toast(e.message, 'error'));
  }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps

  const generar = async () => {
    if (clave === 'compras-por-producto' && !f.todos && !f.producto) { toast('Seleccione un Producto'.toUpperCase(), 'error'); return; }
    const q = new URLSearchParams({ desde: f.desde, hasta: f.hasta });
    if (clave === 'compras-por-producto' && !f.todos) q.set('producto', f.producto);
    setCargando(true);
    try {
      const r = await api.get(`${URL}/${clave}?${q}`);
      setDatos(r.data);
      if (!(Array.isArray(r.data) ? r.data : r.data.filas).length) toast('No Existen Datos Para Mostrar con Los Filtros Seleccionados'.toUpperCase(), 'error');
    } catch (e) { toast(e.message, 'error', 7000); } finally { setCargando(false); }
  };

  const filas = Array.isArray(datos) ? datos : datos?.filas || [];
  const columnas = useMemo(() => {
    if (clave !== 'historico') return columnasDe(filas);
    const ps = datos?.productos || [];
    return [
      { campo: 'Id', titulo: 'Código', tipo: 'num' },
      { campo: 'FechaCompra', titulo: 'Fecha compra', tipo: 'fecha' },
      { campo: 'NombreProveedor', titulo: 'Proveedor', ancho: 180, render: (x) => <b>{tit(x.NombreProveedor ?? '')}</b> },
      { campo: 'NroNotaCompra', titulo: 'Nota compra' },
      { campo: 'NroNotaRecepcion', titulo: 'Nota recepción' },
      ...ps.map((p) => ({ campo: `c${p.Id}`, titulo: `Cant. ${tit(p.Nombre)}`, tipo: 'num', total: true, clase: () => 'col-cant' })),
      { campo: 'TotalesCantidad', titulo: 'Total cantidad', tipo: 'num', total: true, render: (x) => <b>{x.TotalesCantidad ?? ''}</b> },
      ...ps.map((p) => ({ campo: `p${p.Id}`, titulo: `P.Costo ${tit(p.Nombre)}`, tipo: 'bs', clase: () => 'col-precio' })),
      { campo: 'TotalesBs', titulo: 'Total Bs', tipo: 'bs', total: true },
    ];
  }, [filas, datos, clave]);
  const prodNombre = prods.find((p) => String(p.Id) === String(f.producto))?.Nombre;
  const sub = [`Del ${f.desde.split('-').reverse().join('/')} al ${f.hasta.split('-').reverse().join('/')}`, clave === 'compras-por-producto' && (f.todos ? 'Todos los productos' : prodNombre)].filter(Boolean).join(' · ');

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo"><span className="modelo-icono"><Icono n={cfg.icono} size={22} /></span><div><h1>{cfg.titulo}</h1><p>{cfg.sub}</p></div></div>
      </header>
      <div className="modelo-cuerpo">
        <div className="filtros-lista filtros-reporte">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={f.desde} max={f.hasta} onChange={(e) => e.target.value && setF((o) => ({ ...o, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={f.hasta} min={f.desde} onChange={(e) => e.target.value && setF((o) => ({ ...o, hasta: e.target.value }))} /></label>
          {clave === 'compras-por-producto' && (
            <>
              <div className="filtro"><span>Productos</span><div className="chips-estado">
                <button type="button" className={`chip-filtro ${f.todos ? 'activo' : ''}`} onClick={() => setF((o) => ({ ...o, todos: true }))}>Todos</button>
                <button type="button" className={`chip-filtro ${!f.todos ? 'activo' : ''}`} onClick={() => setF((o) => ({ ...o, todos: false }))}>Uno</button>
              </div></div>
              {!f.todos && <div className="filtro filtro-ancho"><span>Producto</span><ComboMulti dt={prods} valor={f.producto} onChange={(x) => setF((o) => ({ ...o, producto: x }))} id="Id" nameId="Cód." descripcionId="Nombre" descripcion="Producto" /></div>}
            </>
          )}
          <button className="btn btn-pri filtro-boton" onClick={generar} disabled={cargando}>{cargando ? <span className="spinner" /> : <Icono n="search" size={16} />} Generar</button>
        </div>
        {datos && <TablaReporte titulo={cfg.sub.toUpperCase()} subtitulo={sub} columnas={columnas} filas={filas} cargando={cargando} claveFila={clave === 'historico' ? 'Id' : '__no'} vacio="No Existen Datos Para Mostrar" />}
        {!datos && !cargando && <div className="reporte-vacio"><Icono n="search" size={28} /><span>Elija los filtros y presione <b>Generar</b></span></div>}
      </div>
    </section>
  );
}
