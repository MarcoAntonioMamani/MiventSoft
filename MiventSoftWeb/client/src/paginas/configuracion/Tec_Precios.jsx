// =====================================================================
//  Tec_Precios.vb -> Gestionar Precios   (SP: sp_Mam_Precios)
//  Matriz producto x categoria de precio por almacen. Solo se envian las
//  celdas modificadas; el servidor arma la tabla TY007 completa como VB.
// =====================================================================
import { useCallback, useEffect, useMemo, useState } from 'react';
import { ComboMulti, SwitchButton } from '../../componentes/Controles';
import { EfectoConfirmar } from '../../componentes/Efecto';
import { Paginador, FILAS_POR_PAGINA } from '../../componentes/GridBuscador';
import { api } from '../../api/http';
import { useToast } from '../../componentes/Toast';
import Icono from '../../componentes/Icono';
import { palabrasDe, coincide } from '../../global/buscar';
import { useAlActivar, useRefrescar } from '../../global/vistaActiva';

const URL = '/api/configuracion/precios';
const clave = (p, c) => `${p}|${c}`;

function PanelCategorias({ abierto, onCerrar, onCambio, onAgregado }) {
  const toast = useToast();
  const [cats, setCats] = useState([]);
  const [desc, setDesc] = useState('');
  const [letra, setLetra] = useState(true);
  const [borrar, setBorrar] = useState(null);

  const cargar = useCallback(() => api.get(`${URL}/categorias`).then((r) => setCats(r.data)).catch((e) => toast(e.message, 'error')), [toast]);
  useEffect(() => { if (abierto) cargar(); }, [abierto, cargar]);
  if (!abierto) return null;

  const agregar = async () => {
    if (!desc.trim()) return toast('Ingrese una Descripcion!', 'error');
    try {
      const r = await api.post(`${URL}/categorias`, { descripcion: desc.trim(), tipo: letra ? 1 : 0 });
      toast(r.mensaje, 'ok');
      setDesc('');
      cargar();
      onAgregado(); // Kaily btnAgregar_Click: _prGrabarCategorias() + btnGrabar.PerformClick()
    } catch (e) { toast(e.message, 'error'); }
  };
  const eliminar = async () => {
    const c = borrar;
    setBorrar(null);
    try {
      const r = await api.del(`${URL}/categorias/${c.ygnumi}`);
      toast(r.mensaje, 'ok');
      cargar();
      onCambio();
    } catch (e) { toast(e.message, 'error'); }
  };

  return (
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-lg" role="dialog" aria-modal="true" aria-label="Categorías de precio" onMouseDown={(e) => e.stopPropagation()}>
        <div className="modal-cab">
          <h3>Categorías de precio</h3>
          <button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button>
        </div>
        <div className="cat-nueva">
          <input className="control mayus" placeholder="Descripción de la nueva categoría" value={desc} maxLength={100}
            onChange={(e) => setDesc(e.target.value.toUpperCase())} onKeyDown={(e) => e.key === 'Enter' && agregar()} />
          <SwitchButton valor={letra} onChange={setLetra} textoOn="LETRA" textoOff="NÚMERO" />
          <button className="btn btn-pri" onClick={agregar}><Icono n="plus" size={16} /> Agregar</button>
        </div>
        <div className="tabla-scroll alto-ayuda">
          <table className="grid">
            <thead><tr><th style={{ width: 70 }}>Cod</th><th>Descripción</th><th style={{ width: 110 }}>Tipo</th><th style={{ width: 60 }} /></tr></thead>
            <tbody>
              {cats.map((c, i) => (
                <tr key={c.ygnumi}>
                  <td className="centro"><b>{c.ygcod}</b></td>
                  <td>{c.ygdesc}</td>
                  <td>{c.estado}</td>
                  <td className="centro">
                    {i > 1 ? (
                      <button className="btn-icono peligro" aria-label={`Eliminar ${c.ygdesc}`} onClick={() => setBorrar(c)}><Icono n="trash" size={15} /></button>
                    ) : <span className="badge badge-off" title="Categoría del sistema">SIS</span>}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
      <EfectoConfirmar abierto={!!borrar} titulo="Confirmación de Eliminación" descripcion={`¿Eliminar la categoría ${borrar?.ygdesc}?`}
        onAceptar={eliminar} onCancelar={() => setBorrar(null)} />
    </div>
  );
}

export default function Tec_Precios() {
  const toast = useToast();
  const [almacenes, setAlmacenes] = useState([]);
  const [almacen, setAlmacen] = useState('');
  const [datos, setDatos] = useState({ productos: [], categorias: [], precios: [] });
  const [cambios, setCambios] = useState({}); // clave -> valor (texto)
  const [editando, setEditando] = useState(false);
  const [cargando, setCargando] = useState(false);
  const [grabando, setGrabando] = useState(false);
  const [filtro, setFiltro] = useState('');
  const [panelCat, setPanelCat] = useState(false);
  const [pagina, setPagina] = useState(0);

  useAlActivar(() => {
    api.get(`${URL}/almacenes`).then((r) => { setAlmacenes(r.data); setAlmacen((a) => (a !== '' ? a : r.data[0]?.aanumi ?? '')); }).catch((e) => toast(e.message, 'error'));
  });

  const cargar = useCallback(async () => {
    if (!almacen) return;
    setCargando(true);
    try {
      const r = await api.get(`${URL}/matriz/${almacen}`);
      setDatos(r.data);
      setCambios({});
    } catch (e) { toast(e.message, 'error'); } finally { setCargando(false); }
  }, [almacen, toast]);
  useEffect(() => { cargar(); }, [cargar]);
  // al volver a la pestaña: precios actuales de la BD, salvo que haya cambios sin grabar
  useAlActivar(() => { if (!editando && !Object.keys(cambios).length) cargar(); }, { alMontar: false });
  const refrescar = useRefrescar(cargar); // boton Actualizar: almacenes + matriz de la BD

  const precios = useMemo(() => new Map(datos.precios.map(([p, c, v]) => [clave(p, c), v])), [datos.precios]);
  const filas = useMemo(() => {
    const w = palabrasDe(filtro);
    if (!w.length) return datos.productos;
    return datos.productos.filter((p) => coincide(`${p.yfnumi} ${p.yfcprod} ${p.yfcdprod1} ${p.NombreCategoria} ${p.proveedor}`, w));
  }, [datos.productos, filtro]);
  useEffect(() => setPagina(0), [filtro, almacen]);
  const totalPaginas = Math.max(1, Math.ceil(filas.length / FILAS_POR_PAGINA));
  const pag = Math.min(pagina, totalPaginas - 1);
  const visibles = filas.slice(pag * FILAS_POR_PAGINA, (pag + 1) * FILAS_POR_PAGINA);

  const nCambios = Object.keys(cambios).length;
  const valor = (p, c) => {
    const k = clave(p, c);
    if (k in cambios) return cambios[k];
    const v = precios.get(k);
    return v === undefined ? '' : String(v);
  };
  const cambiar = (p, c, texto) => {
    const k = clave(p, c);
    const original = precios.get(k);
    setCambios((x) => {
      const n = { ...x };
      if (original !== undefined && Number(texto) === original && texto !== '') delete n[k];
      else if (original === undefined && texto === '') delete n[k];
      else n[k] = texto;
      return n;
    });
  };

  const grabar = async () => {
    if (!nCambios) return toast('No hay precios modificados.', 'error');
    const lista = Object.entries(cambios).map(([k, v]) => { const [p, c] = k.split('|').map(Number); return [p, c, v === '' ? 0 : Number(v)]; });
    if (lista.some((x) => !Number.isFinite(x[2]) || x[2] < 0)) return toast('Revise los precios: deben ser números positivos.', 'error');
    setGrabando(true);
    try {
      const r = await api.put(`${URL}/matriz/${almacen}`, { cambios: lista });
      toast(r.mensaje, 'ok');
      setEditando(false);
      await cargar();
    } catch (e) { toast(e.message, 'error', 8000); } finally { setGrabando(false); }
  };
  const cancelar = () => { setCambios({}); setEditando(false); };

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo">
          <span className="modelo-icono"><Icono n="dollar" size={22} /></span>
          <div><h1>Precios</h1><p>Gestionar precios por almacén y categoría</p></div>
        </div>
        <div className="barra">
          {!editando ? (
            <>
              <button className="btn btn-pri" onClick={() => setEditando(true)} disabled={!datos.productos.length}><Icono n="edit" size={16} /> <span className="txt-btn">Modificar</span></button>
              <button className="btn btn-sec" onClick={() => setPanelCat(true)}><Icono n="tag" size={16} /> <span className="txt-btn">Categorías</span></button>
              <button className="btn-icono" title="Actualizar" aria-label="Actualizar" onClick={refrescar} disabled={cargando}>{cargando ? <span className="spinner" /> : <Icono n="refresh" size={17} />}</button>
            </>
          ) : (
            <>
              <span className="modo-chip modificar">{nCambios} cambio(s)</span>
              <button className="btn btn-sec" onClick={cancelar} disabled={grabando}><Icono n="x" size={16} /> <span className="txt-btn">Cancelar</span></button>
              <button className="btn btn-ok" onClick={grabar} disabled={grabando || !nCambios}><Icono n="save" size={16} /> <span className="txt-btn">{grabando ? 'Grabando...' : 'Grabar'}</span></button>
            </>
          )}
        </div>
      </header>
      <div className="precios-filtros">
        <label className="campo">
          <span className="campo-etq">Almacén</span>
          <ComboMulti dt={almacenes} valor={almacen} id="aanumi" nameId="COD" descripcionId="aabdes" descripcion="Descripción" deshabilitado={editando}
            onChange={(v) => setAlmacen(v)} />
        </label>
        <div className="buscar">
          <Icono n="search" size={16} />
          <input value={filtro} onChange={(e) => setFiltro(e.target.value)} placeholder="Buscar producto, categoría o proveedor..." aria-label="Buscar producto" />
        </div>
      </div>
      <div className="modelo-cuerpo">
        <div className="grid-buscador">
          <div className="tabla-scroll">
            <table className="grid grid-precios">
              <thead>
                <tr>
                  <th className="fija" style={{ minWidth: 230 }}>Producto</th>
                  <th className="sec">Categoría</th>
                  <th className="sec">Proveedor</th>
                  {datos.categorias.map((c) => <th key={c.ygnumi} className="num" title={c.ygdesc}><span className="cod-cat">{c.ygcod}</span> {c.ygdesc}</th>)}
                </tr>
              </thead>
              <tbody>
                {visibles.map((p, i) => (
                  <tr key={p.yfnumi} className="fila" style={{ '--i': i }}>
                    <td className="fija" data-label="Producto">
                      {/* Kaily: columnas CodExterno (yfcprod) y Cod.Sistema (yfnumi) visibles */}
                      <b>{p.yfcdprod1}</b><span className="sub-celda">{p.yfcprod ? `${p.yfcprod} · ` : ''}Cod. Sistema {p.yfnumi}</span>
                    </td>
                    <td className="sec" data-label="Categoría">{p.NombreCategoria}</td>
                    <td className="sec" data-label="Proveedor">{p.proveedor}</td>
                    {datos.categorias.map((c) => {
                      const k = clave(p.yfnumi, c.ygnumi);
                      const mod = k in cambios;
                      return (
                        <td key={c.ygnumi} className={`num celda-precio ${mod ? 'modificada' : ''}`} data-label={`${c.ygcod} · ${c.ygdesc}`}>
                          {editando ? (
                            <input type="number" min="0" step="0.01" inputMode="decimal" className="input-precio" value={valor(p.yfnumi, c.ygnumi)}
                              aria-label={`Precio ${c.ygdesc} de ${p.yfcdprod1}`} onChange={(e) => cambiar(p.yfnumi, c.ygnumi, e.target.value)} />
                          ) : (
                            valor(p.yfnumi, c.ygnumi) === '' ? <span className="sin-precio">—</span> : Number(valor(p.yfnumi, c.ygnumi)).toFixed(2)
                          )}
                        </td>
                      );
                    })}
                  </tr>
                ))}
                {!filas.length && <tr><td colSpan={3 + datos.categorias.length} className="vacio">{cargando ? 'Cargando...' : 'Sin productos'}</td></tr>}
              </tbody>
            </table>
          </div>
          <div className="grid-pie">
            <span className="grid-total"><Icono n="list" size={14} /> {filas.length} de {datos.productos.length} productos{nCambios ? ` · ${nCambios} cambio(s) sin grabar` : ''}</span>
            <Paginador pagina={pag} totalPaginas={totalPaginas} total={filas.length} desde={filas.length ? pag * FILAS_POR_PAGINA + 1 : 0}
              hasta={Math.min((pag + 1) * FILAS_POR_PAGINA, filas.length)} onPagina={setPagina} />
          </div>
        </div>
      </div>
      <PanelCategorias abierto={panelCat} onCerrar={() => setPanelCat(false)} onCambio={cargar}
        onAgregado={() => (nCambios ? grabar() : cargar())} />
    </section>
  );
}
