// =====================================================================
//  Tec_Productos.vb -> Gestion De Productos   (SP: MAM_Productos, TVP TCL0064)
//  Imagenes: se suben solo las nuevas y se indica cuales quitar.
// =====================================================================
import { useEffect, useRef, useState } from 'react';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, ComboLibreria, SwitchButton } from '../../componentes/Controles';
import { Celda } from '../../global/Celda';
import { api, urlImagen } from '../../api/http';
import { useToast } from '../../componentes/Toast';
import Icono from '../../componentes/Icono';

const URL = '/api/configuracion/productos';
const VACIO = { Id: '', CodigoExterno: '', CodigoBarras: '', NombreProducto: '', DescripcionProducto: '', StockMinimo: '0', estado: true,
  CategoriaId: '', EmpresaId: '', ProveedorId: '', MarcaId: '', AttributoId: '', FamiliaId: '', UnidadVentaId: '', UnidadMaximaId: '', Conversion: '1' };
const LIBRERIAS = [
  ['MarcaId', 'marcas', 3, 'Marca'], ['AttributoId', 'atributos', 4, 'Atributo'], ['FamiliaId', 'familias', 5, 'Familia'],
  ['UnidadVentaId', 'unidadesVenta', 6, 'Unidad de venta'], ['UnidadMaximaId', 'unidadesMaxima', 7, 'Unidad máxima'],
];

function Galeria({ existentes, eliminar, nuevas, deshabilitado, onAgregar, onQuitarExistente, onQuitarNueva }) {
  const input = useRef(null);
  const [previews, setPreviews] = useState([]);
  useEffect(() => {
    const urls = nuevas.map((f) => window.URL.createObjectURL(f));
    setPreviews(urls);
    return () => urls.forEach((u) => window.URL.revokeObjectURL(u));
  }, [nuevas]);
  const activas = existentes.filter((n) => !eliminar.includes(n));
  return (
    <div className="galeria">
      {activas.map((n) => (
        <figure key={n} className="galeria-item">
          <img src={urlImagen('producto', n)} alt="" loading="lazy" />
          {!deshabilitado && <button type="button" className="galeria-quitar" aria-label="Quitar imagen" onClick={() => onQuitarExistente(n)}><Icono n="x" size={14} /></button>}
        </figure>
      ))}
      {previews.map((u, i) => (
        <figure key={u} className="galeria-item nueva">
          <img src={u} alt="" />
          <span className="galeria-etq">Nueva</span>
          <button type="button" className="galeria-quitar" aria-label="Quitar imagen" onClick={() => onQuitarNueva(i)}><Icono n="x" size={14} /></button>
        </figure>
      ))}
      {!deshabilitado && (
        <button type="button" className="galeria-item galeria-agregar" onClick={() => input.current?.click()}>
          <Icono n="plus" size={22} /><span>Agregar</span>
        </button>
      )}
      {deshabilitado && !activas.length && <div className="galeria-vacia"><Icono n="image" size={30} /><span>Sin imágenes</span></div>}
      <input ref={input} type="file" multiple hidden accept="image/png,image/jpeg,image/gif,image/bmp"
        onChange={(e) => { onAgregar([...e.target.files]); e.target.value = ''; }} />
    </div>
  );
}

export default function Tec_Productos() {
  const toast = useToast();
  const [form, setForm] = useState(VACIO);
  const [errores, setErrores] = useState({});
  const [combos, setCombos] = useState({ empresas: [], categorias: [], proveedores: [], marcas: [], atributos: [], familias: [], unidadesVenta: [], unidadesMaxima: [] });
  const [imgs, setImgs] = useState({ existentes: [], eliminar: [], nuevas: [] });
  const combosRef = useRef(null);
  const pedido = useRef(0);
  // Al corregir un campo se quita su mensaje de error
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };

  useEffect(() => {
    api.get(`${URL}/combos`).then((r) => { combosRef.current = r.data; setCombos(r.data); }).catch((e) => toast(e.message, 'error'));
  }, [toast]);

  const cargarImagenes = async (id) => {
    const n = ++pedido.current;
    setImgs({ existentes: [], eliminar: [], nuevas: [] });
    try {
      const r = await api.get(`${URL}/${id}/imagenes`);
      if (n === pedido.current) setImgs({ existentes: r.data, eliminar: [], nuevas: [] });
    } catch (e) { toast(e.message, 'error'); }
  };

  const nuevaLibreria = (idClasif, clave) => async (descripcion) => {
    const r = await api.post(`${URL}/libreria/${idClasif}`, { descripcion });
    setCombos((c) => ({ ...c, [clave]: r.data }));
    return r;
  };

  const _datos = () => {
    const fd = new FormData();
    Object.keys(VACIO).filter((k) => k !== 'Id').forEach((k) => fd.append(k, k === 'estado' ? (form.estado ? 1 : 0) : form[k] ?? ''));
    if (imgs.eliminar.length) fd.append('eliminar', JSON.stringify(imgs.eliminar));
    imgs.nuevas.forEach((f) => fd.append('imagenes', f));
    return fd;
  };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(URL)).data,
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 60, '', 'num'),
      new Celda('NombreProducto', true, 'Producto', 240),
      new Celda('CodigoExterno', true, 'Código', 90),
      new Celda('NombreCategoria', true, 'Categoría', 130),
      new Celda('Empresa', true, 'Empresa', 160),
      new Celda('StockMinimo', true, 'Stock mín.', 90, '0.00', 'num'),
      new Celda('estado', true, 'Estado', 90, 'ACTIVO', 'bool'),
    ],
    _PMOLimpiar: () => {
      const c = combosRef.current || combos;
      const p = (arr, k = 'Id') => arr?.[0]?.[k] ?? '';
      setForm({ ...VACIO, EmpresaId: p(c.empresas), CategoriaId: p(c.categorias), ProveedorId: p(c.proveedores),
        MarcaId: p(c.marcas, 'cnnum'), AttributoId: p(c.atributos, 'cnnum'), FamiliaId: p(c.familias, 'cnnum'),
        UnidadVentaId: p(c.unidadesVenta, 'cnnum'), UnidadMaximaId: p(c.unidadesMaxima, 'cnnum') });
      pedido.current++;
      setImgs({ existentes: [], eliminar: [], nuevas: [] });
    },
    _PMOMostrarRegistro: (f) => {
      setForm(Object.fromEntries(Object.keys(VACIO).map((k) => [k, k === 'estado' ? !!f.estado : f[k] ?? VACIO[k]])));
      cargarImagenes(f.Id);
    },
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.NombreProducto.trim()) e.NombreProducto = 'Ingrese Nombre de Producto';
      if (!form.EmpresaId) e.EmpresaId = 'Seleccione una Empresa';
      if (!form.CategoriaId) e.CategoriaId = 'Seleccione una Categoria';
      if (String(form.StockMinimo).trim() === '' || Number(form.StockMinimo) < 0) e.StockMinimo = 'Ingrese valor Stock Minimo';
      if (!(Number(form.Conversion) > 0)) e.Conversion = 'Ingrese Valor en Conversion';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => api.post(URL, _datos()),
    _PMOModificarRegistro: () => api.put(`${URL}/${form.Id}`, _datos()),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Producto ${f.NombreProducto} ?`,
  });
  const ro = !m.habilitado;

  const agregarImagenes = (files) => {
    const validas = files.filter((f) => f.size <= 3 * 1024 * 1024);
    if (validas.length < files.length) toast('Algunas imágenes superan 3 MB y no se agregaron.', 'error');
    setImgs((x) => ({ ...x, nuevas: [...x.nuevas, ...validas].slice(0, 10) }));
  };

  return (
    <ModeloF1 m={m} titulo="Productos" subtitulo="Gestion De Productos" icono="box">
      <div className="registro-grid con-lateral-ancho">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del producto</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Código externo"><input className="control" value={form.CodigoExterno} onChange={set('CodigoExterno')} readOnly={ro} maxLength={50} /></Campo>
            <Campo etiqueta="Código de barras"><input className="control" value={form.CodigoBarras} onChange={set('CodigoBarras')} readOnly={ro} maxLength={50} inputMode="numeric" /></Campo>
            <Campo etiqueta="Nombre del producto" ancho={3} error={errores.NombreProducto} requerido>
              <input className="control" value={form.NombreProducto} onChange={set('NombreProducto')} readOnly={ro} maxLength={200} />
            </Campo>
            <Campo etiqueta="Descripción" ancho={3}><input className="control" value={form.DescripcionProducto} onChange={set('DescripcionProducto')} readOnly={ro} maxLength={300} /></Campo>
            <Campo etiqueta="Empresa" error={errores.EmpresaId} requerido>
              <ComboMulti dt={combos.empresas} valor={form.EmpresaId} onChange={set('EmpresaId')} id="Id" nameId="Código" descripcionId="Nombre" descripcion="Empresa" deshabilitado={ro} error={errores.EmpresaId} />
            </Campo>
            <Campo etiqueta="Categoría" error={errores.CategoriaId} requerido>
              <ComboMulti dt={combos.categorias} valor={form.CategoriaId} onChange={set('CategoriaId')} id="Id" nameId="Código" descripcionId="NombreCategoria" descripcion="Categoría" deshabilitado={ro} error={errores.CategoriaId} />
            </Campo>
            <Campo etiqueta="Proveedor">
              <ComboMulti dt={combos.proveedores} valor={form.ProveedorId} onChange={set('ProveedorId')} id="Id" nameId="Código" descripcionId="NombreProveedor" descripcion="Proveedor" deshabilitado={ro} />
            </Campo>
            {LIBRERIAS.map(([campo, clave, idc, titulo]) => (
              <Campo key={campo} etiqueta={titulo}>
                <ComboLibreria dt={combos[clave]} valor={form[campo]} onChange={set(campo)} titulo={titulo} deshabilitado={ro} onNuevo={nuevaLibreria(idc, clave)} />
              </Campo>
            ))}
            <Campo etiqueta="Conversión (unid. por máxima)" error={errores.Conversion} requerido>
              <input className="control num" type="number" min="0" step="any" inputMode="decimal" value={form.Conversion} onChange={set('Conversion')} readOnly={ro} />
            </Campo>
            <Campo etiqueta="Stock mínimo" error={errores.StockMinimo} requerido>
              <input className="control num" type="number" min="0" step="any" inputMode="decimal" value={form.StockMinimo} onChange={set('StockMinimo')} readOnly={ro} />
            </Campo>
            <Campo etiqueta="Estado"><SwitchButton valor={!!form.estado} onChange={set('estado')} deshabilitado={ro} /></Campo>
          </div>
        </div>
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Imágenes</h2>
          <Galeria
            existentes={imgs.existentes} eliminar={imgs.eliminar} nuevas={imgs.nuevas} deshabilitado={ro}
            onAgregar={agregarImagenes}
            onQuitarExistente={(n) => setImgs((x) => ({ ...x, eliminar: [...x.eliminar, n] }))}
            onQuitarNueva={(i) => setImgs((x) => ({ ...x, nuevas: x.nuevas.filter((_, j) => j !== i) }))}
          />
        </div>
      </div>
    </ModeloF1>
  );
}
