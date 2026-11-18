// =====================================================================
//  ModeloF1 (equivale a Modelo/ModeloF1.vb)
//
//  En VB cada formulario heredaba de ModeloF1 y sobreescribia los metodos _PMO*.
//  Aqui cada pantalla llama a useModeloF1({...}) pasando esos mismos metodos:
//
//    _PMOGetTablaBuscador()            -> Promise<filas>
//    _PMOGetListEstructuraBuscador     -> Celda[]
//    _PMOLimpiar()                     -> limpia el formulario
//    _PMOMostrarRegistro(fila)         -> carga la fila en el formulario
//    _PMOValidarCampos()               -> true/false (marca errores)
//    _PMOLimpiarErrores()
//    _PMOGrabarRegistro()              -> Promise<{id, mensaje}>
//    _PMOModificarRegistro(fila)       -> Promise<{id, mensaje}>
//    _PMOEliminarRegistro(fila)        -> Promise<{mensaje}>
//    _PMODescripcionEliminar(fila)     -> texto de la confirmacion
//  Opcionales:
//    _PMOBloqueoModificar(fila)        -> texto si NO se puede modificar (ej. conciliacion cerrada)
//    _PMOBloqueoEliminar(fila)         -> texto si NO se puede eliminar
//    filtroExtra(fila) / filtroExtraClave -> filtro adicional de la lista (ej. por estado)
//    sinNuevo / sinEliminar            -> la pantalla no registra / no elimina aunque el rol pueda
//  Props del componente:
//    menuNuevo=[{texto, icono, accion}] -> varias formas de crear (ej. Pedido / Venta directa)
//    accionesRegistro                   -> botones extra al ver un registro (ej. Imprimir)
//    accionesFila(f)                    -> [{texto, icono, onClick}] extra en cada fila de la lista y en su menu (clic derecho)
//
//  RUTAS (cada programa tiene su propia URL, funciona el boton "atras"):
//    /modulo/programa               -> lista (buscador)
//    /modulo/programa/nuevo         -> nuevo registro
//    /modulo/programa/:id           -> ver registro
//    /modulo/programa/:id/editar    -> modificar registro
//  Al Grabar, Cancelar o Eliminar se vuelve a la lista con el registro resaltado.
// =====================================================================
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useAlActivar, useRefrescar } from '../global/vistaActiva';
import { createPortal } from 'react-dom';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import GridBuscador, { valorCelda } from './GridBuscador';
import { EfectoConfirmar, EfectoStockNegativo } from './Efecto';
import { useToast } from './Toast';
import Icono from './Icono';
import { usePermisos } from '../global/Sesion';
import { coincide } from '../global/buscar';

// Separa la URL en base del programa (/configuracion/clientes) y sub-ruta
function _leerRuta(pathname) {
  const p = pathname.replace(/\/+$/, '').split('/').filter(Boolean);
  const base = `/${p.slice(0, 2).join('/')}`;
  const resto = p.slice(2);
  if (!resto.length) return { base, tipo: 'lista', clave: 'lista' };
  if (resto[0] === 'nuevo') return { base, tipo: 'nuevo', clave: 'nuevo' };
  const editar = resto[1] === 'editar';
  return { base, tipo: editar ? 'editar' : 'ver', id: decodeURIComponent(resto[0]), clave: `${resto[0]}|${editar}` };
}

export function useModeloF1(opciones) {
  const op = useRef(opciones);
  op.current = opciones;
  const idCampo = opciones.idCampo || 'Id';
  const toast = useToast();
  const permisos = usePermisos();
  // sinNuevo / sinEliminar: la pantalla no tiene esas acciones (ej. FrmDespacho: botones ocultos en el escritorio)
  const puede = useMemo(() => ({
    ...permisos,
    insertar: !!permisos?.insertar && !opciones.sinNuevo,
    eliminar: !!permisos?.eliminar && !opciones.sinEliminar,
  }), [permisos, opciones.sinNuevo, opciones.sinEliminar]);
  const navigate = useNavigate();
  const location = useLocation();
  const ruta = _leerRuta(location.pathname);

  const [dt, setDt] = useState([]);
  const [filtros, setFiltros] = useState({});
  const [orden, setOrden] = useState({ campo: null, asc: true });
  const [idSel, setIdSel] = useState(null);
  const [resaltado, setResaltado] = useState(null);
  const [modo, setModo] = useState('ver'); // 'ver' | 'nuevo' | 'modificar'  (_MNuevo / _MModificar)
  const [cargando, setCargando] = useState(true);
  const [grabando, setGrabando] = useState(false);
  const [confirmar, setConfirmar] = useState(null);
  const [avisoStock, setAvisoStock] = useState(null); // { mensaje, productos } al no poder eliminar por stock
  const tab = ruta.tipo === 'lista' ? 'buscador' : 'registro';
  const habilitado = modo !== 'ver';

  // filas visibles = dt filtrado (Contains) + ordenado
  const filas = useMemo(() => {
    const est = op.current._PMOGetListEstructuraBuscador;
    let r = dt.filter((f) =>
      Object.entries(filtros).every(([campo, txt]) => {
        if (txt === '' || txt === undefined) return true;
        if (campo === '__todo') {
          return coincide(est.filter((c) => c.visible && c.tipo !== 'bool').map((c) => f[c.campo] ?? '').join(' '), txt);
        }
        const c = est.find((x) => x.campo === campo);
        if (c?.tipo === 'bool') return (valorCelda(f, c) ? '1' : '0') === txt;
        return coincide(f[campo], txt);
      })
    );
    if (op.current.filtroExtra) r = r.filter((f) => op.current.filtroExtra(f));
    if (orden.campo) {
      const k = orden.campo;
      r = [...r].sort((a, b) => {
        const x = a[k] ?? '';
        const y = b[k] ?? '';
        const cmp = typeof x === 'number' && typeof y === 'number' ? x - y : String(x).localeCompare(String(y), 'es', { numeric: true });
        return orden.asc ? cmp : -cmp;
      });
    }
    return r;
  }, [dt, filtros, orden, opciones.filtroExtraClave]); // eslint-disable-line react-hooks/exhaustive-deps

  const _MPos = filas.findIndex((f) => f.__id === idSel);
  const filaActual = dt.find((f) => f.__id === idSel) || null;

  // Id del registro cargado en el formulario (distinto de la fila seleccionada en la lista)
  const enFormulario = useRef(null);
  const _mostrar = useCallback((fila) => {
    if (fila) {
      setIdSel(fila.__id);
      enFormulario.current = fila.__id;
      op.current._PMOMostrarRegistro(fila);
    } else {
      setIdSel(null);
      enFormulario.current = null;
      op.current._PMOLimpiar();
    }
  }, []);

  const _resaltar = (id) => {
    setResaltado(id);
    setTimeout(() => setResaltado(null), 2200);
  };

  // _PMFiltrar: recarga el buscador
  const _PMFiltrar = useCallback(async (idMostrar) => {
    setCargando(true);
    try {
      const datos = await op.current._PMOGetTablaBuscador();
      const conId = datos.map((f) => ({ ...f, __id: String(f[idCampo]) }));
      setDt(conId);
      const objetivo = idMostrar !== undefined && idMostrar !== null ? conId.find((f) => f.__id === String(idMostrar)) : null;
      if (objetivo) _mostrar(objetivo);
      return conId;
    } catch (e) {
      toast(e.message, 'error', 8000);
      return [];
    } finally {
      setCargando(false);
    }
  }, [idCampo, _mostrar, toast]);

  // _PMIniciarTodo
  useEffect(() => {
    _PMFiltrar();
  }, [_PMFiltrar]);

  // Al volver a la pestaña: el buscador se trae otra vez de la BD (sin spinner).
  // Un registro a medio registrar/editar no se toca; el que solo se esta viendo se muestra actualizado.
  const rutaRef = useRef(ruta);
  rutaRef.current = ruta;
  useAlActivar(async () => {
    try {
      const datos = await op.current._PMOGetTablaBuscador();
      const conId = datos.map((f) => ({ ...f, __id: String(f[idCampo]) }));
      setDt(conId);
      const r = rutaRef.current;
      if (r.tipo === 'ver') {
        const fila = conId.find((f) => f.__id === r.id);
        if (fila) _mostrar(fila);
      }
    } catch (e) {
      toast(e.message, 'error', 8000);
    }
  }, { alMontar: false, alRefrescar: false }); // con "Actualizar" el buscador lo recarga _PMFiltrar (con spinner)

  // La URL manda: aplica lista / nuevo / ver / editar
  useEffect(() => {
    if (cargando) return;
    if (ruta.tipo === 'lista') {
      setModo('ver');
      return;
    }
    if ((ruta.tipo === 'nuevo' && !puede.insertar) || (ruta.tipo === 'editar' && !puede.modificar)) {
      toast(SIN_PERMISO(ruta.tipo === 'nuevo' ? 'registrar' : 'modificar'), 'error');
      navigate(ruta.base, { replace: true });
      return;
    }
    if (ruta.tipo === 'nuevo') {
      if (modo !== 'nuevo') {
        op.current._PMOLimpiarErrores?.();
        setIdSel(null);
        enFormulario.current = null;
        op.current._PMOLimpiar();
        op.current._PMONuevo?.();
        setModo('nuevo');
      }
      return;
    }
    const fila = dt.find((f) => f.__id === ruta.id);
    if (!fila) {
      toast(`El registro ${ruta.id} no existe.`, 'error');
      navigate(ruta.base, { replace: true });
      return;
    }
    if (fila.__id !== enFormulario.current || modo === 'nuevo') _mostrar(fila);
    op.current._PMOLimpiarErrores?.();
    setModo(ruta.tipo === 'editar' ? 'modificar' : 'ver');
  }, [ruta.clave, cargando, dt]); // eslint-disable-line react-hooks/exhaustive-deps

  const irLista = (idResaltar) => {
    if (idResaltar !== undefined && idResaltar !== null) _resaltar(String(idResaltar));
    // Desde editar/nuevo se reemplaza la entrada: "atras" no vuelve a un formulario ya cerrado
    navigate(ruta.base, { replace: ruta.tipo === 'editar' || ruta.tipo === 'nuevo' });
  };
  const _irA = (i) => !habilitado && filas[i] && navigate(`${ruta.base}/${encodeURIComponent(filas[i].__id)}`, { replace: true });
  const _PMPrimerRegistro = () => _irA(0);
  const _PMAnteriorRegistro = () => _MPos > 0 && _irA(_MPos - 1);
  const _PMSiguienteRegistro = () => _MPos < filas.length - 1 && _irA(_MPos + 1);
  const _PMUltimoRegistro = () => _irA(filas.length - 1);

  const SIN_PERMISO = (a) => `Su rol no tiene permiso para ${a} en este programa.`;
  const _PMNuevo = (consulta = '') => (puede.insertar ? navigate(`${ruta.base}/nuevo${consulta}`) : toast(SIN_PERMISO('registrar'), 'error'));
  const abrir = (fila) => navigate(`${ruta.base}/${encodeURIComponent(fila.__id)}`);
  const _PMModificar = (fila = filaActual) => {
    if (!fila) return;
    if (!puede.modificar) { toast(SIN_PERMISO('modificar'), 'error'); return; }
    const bloqueo = op.current._PMOBloqueoModificar?.(fila);
    if (bloqueo) { toast(bloqueo, 'error', 6000); return; }
    navigate(`${ruta.base}/${encodeURIComponent(fila.__id)}/editar`);
  };

  const _PMEliminar = (fila = filaActual) => {
    if (!fila) return;
    if (!puede.eliminar) { toast(SIN_PERMISO('eliminar'), 'error'); return; }
    const bloqueo = op.current._PMOBloqueoEliminar?.(fila);
    if (bloqueo) { toast(bloqueo, 'error', 6000); return; }
    setConfirmar({
      titulo: 'Confirmación de Eliminación',
      descripcion: op.current._PMODescripcionEliminar?.(fila) || '¿Esta seguro de eliminar el registro?',
      fila,
    });
  };

  const _confirmarEliminar = async () => {
    const { fila } = confirmar;
    setConfirmar(null);
    try {
      const r = await op.current._PMOEliminarRegistro(fila);
      toast(r?.mensaje || 'REGISTRO ELIMINADO CON EXITO.', 'ok');
      setIdSel(null);
      await _PMFiltrar();
      irLista();
    } catch (e) {
      // stock quedaria negativo: popup con los productos (en vez de un mensaje que desaparece)
      if (e.datos?.stockNegativo?.length) setAvisoStock({ mensaje: e.message, productos: e.datos.stockNegativo });
      else toast(e.message, 'error', 8000);
    }
  };

  const _PMGuardar = async () => {
    if (grabando) return;
    const t0 = Date.now();
    if (op.current._PMOValidarCampos() === false) {
      // solo si la pantalla no mostro ya su propio mensaje
      if (!(toast.ultimo >= t0)) toast('Complete los campos requeridos marcados en rojo.', 'error', 6000);
      return;
    }
    setGrabando(true);
    try {
      const r = modo === 'nuevo' ? await op.current._PMOGrabarRegistro() : await op.current._PMOModificarRegistro(filaActual);
      toast(r?.mensaje || 'GRABADO CON EXITO.', 'ok');
      op.current._PMOLimpiarErrores?.();
      const id = r?.id ?? filaActual?.[idCampo];
      setModo('ver');
      await _PMFiltrar(id);
      irLista(id);
    } catch (e) {
      toast(e.message, 'error', 8000);
    } finally {
      setGrabando(false);
    }
  };

  // _PMSalir: cancela (descarta cambios) y vuelve a la lista
  const _PMSalir = () => {
    op.current._PMOLimpiarErrores?.();
    if (filaActual) _mostrar(filaActual);
    irLista(filaActual?.__id);
  };

  return {
    dt, filas, filtros, setFiltros, orden, setOrden, idSel, resaltado, filaActual, _MPos, ruta,
    modo, habilitado, tab, cargando, grabando, confirmar, setConfirmar, _confirmarEliminar, avisoStock, setAvisoStock,
    _PMFiltrar, _PMNuevo, _PMModificar, _PMEliminar, _PMGuardar, _PMSalir, abrir, seleccionar: (f) => setIdSel(f.__id), irLista,
    _PMPrimerRegistro, _PMAnteriorRegistro, _PMSiguienteRegistro, _PMUltimoRegistro,
    estructura: opciones._PMOGetListEstructuraBuscador,
    puede,
  };
}

export default function ModeloF1({ m, titulo, subtitulo, icono = 'grid', children, extraBuscador, menuNuevo, accionesRegistro, accionesFila }) {
  const [menuAbierto, setMenuAbierto] = useState(null); // 'barra' | 'fab' | null
  const refrescar = useRefrescar();
  const nuevo =(origen) => (menuNuevo?.length ? setMenuAbierto(origen) : m._PMNuevo());
  // Atajos: Ctrl+S grabar, Esc cancelar
  useEffect(() => {
    const t = (e) => {
      if (!m.habilitado) return;
      if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 's') { e.preventDefault(); m._PMGuardar(); }
      // Esc dentro de una ventana (buscador de productos, cantidad, ayudas) cierra solo esa ventana, no el registro
      if (e.key === 'Escape' && !m.confirmar && !e.defaultPrevented && !e.target?.closest?.('[role=dialog], [role=alertdialog], .modal-fondo') && !document.querySelector('.modal-fondo, [role=dialog], [role=alertdialog]')) m._PMSalir();
    };
    window.addEventListener('keydown', t);
    return () => window.removeEventListener('keydown', t);
  });

  const enLista = m.tab === 'buscador';
  const total = m.filas.length;
  const pag = m._MPos >= 0 ? `${m._MPos + 1} de ${total}` : `— de ${total}`;
  const etiquetaRegistro = m.modo === 'nuevo' ? 'Nuevo registro' : m.modo === 'modificar' ? `Editando #${m.filaActual?.__id ?? ''}` : `Registro #${m.filaActual?.__id ?? ''}`;

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo">
          <span className="modelo-icono"><Icono n={icono} size={22} /></span>
          <div>
            <h1>{titulo}</h1>
            {subtitulo && <p>{subtitulo}</p>}
          </div>
        </div>
        <div className="barra">
          {enLista ? (
            <>
              {/* Actualizar: buscador + combos de la pantalla, todo de la BD */}
              <button className="btn-icono" title="Actualizar" aria-label="Actualizar" disabled={m.cargando} onClick={() => { m._PMFiltrar(); refrescar(); }}>
                {m.cargando ? <span className="spinner" /> : <Icono n="refresh" size={18} />}
              </button>
              {m.puede.insertar && <button className="btn btn-pri ocultar-movil" onClick={() => nuevo('barra')} aria-haspopup={menuNuevo?.length ? 'menu' : undefined} aria-expanded={menuAbierto === 'barra'}>
                <Icono n="plus" size={17} /> Nuevo{menuNuevo?.length ? <Icono n="chevronDown" size={14} /> : null}
              </button>}
            </>
          ) : !m.habilitado ? (
            <>
              <button className="btn btn-txt" onClick={() => m.irLista(m.filaActual?.__id)}><Icono n="list" size={16} /> <span className="txt-btn">Lista</span></button>
              {accionesRegistro}
              {m.puede.eliminar && <button className="btn btn-sec btn-txt-peligro" onClick={() => m._PMEliminar()} disabled={!m.filaActual}><Icono n="trash" size={16} /> <span className="txt-btn">Eliminar</span></button>}
              {m.puede.modificar && <button className="btn btn-pri" onClick={() => m._PMModificar()} disabled={!m.filaActual}><Icono n="edit" size={16} /> <span className="txt-btn">Modificar</span></button>}
            </>
          ) : (
            <>
              <button className="btn btn-sec" onClick={m._PMSalir} disabled={m.grabando}><Icono n="x" size={16} /> <span className="txt-btn">Cancelar</span></button>
              <button className="btn btn-ok" onClick={m._PMGuardar} disabled={m.grabando}>
                {m.grabando ? <span className="spinner" aria-hidden="true" /> : <Icono n="save" size={16} />} <span className="txt-btn">{m.grabando ? 'Grabando...' : 'Grabar'}</span>
              </button>
            </>
          )}
        </div>
      </header>

      <div className="modelo-tabs">
        <div role="tablist" className={`tabs tabs-${enLista ? 'izq' : 'der'}`}>
          <Link role="tab" aria-selected={enLista} className={enLista ? 'activa' : ''} to={m.ruta.base}
            onClick={(e) => { if (m.habilitado) { e.preventDefault(); m._PMSalir(); } }}>
            <Icono n="list" size={16} /> Lista <span className="contador">{m.dt.length}</span>
          </Link>
          <span role="tab" aria-selected={!enLista} className={`tab-reg ${!enLista ? 'activa' : ''}`}>
            <Icono n="form" size={16} /> {enLista ? 'Registro' : etiquetaRegistro}
            {m.habilitado && <span className={`modo-chip ${m.modo}`}>{m.modo === 'nuevo' ? 'Nuevo' : 'Editando'}</span>}
          </span>
          <span className="tabs-indicador" aria-hidden="true" />
        </div>
        {!enLista && m.modo === 'ver' && (
          <nav className="navegacion" aria-label="Navegacion de registros">
            <button className="btn-icono" onClick={m._PMPrimerRegistro} aria-label="Primero"><Icono n="first" size={16} /></button>
            <button className="btn-icono" onClick={m._PMAnteriorRegistro} aria-label="Anterior"><Icono n="prev" size={16} /></button>
            <span className="paginacion">{pag}</span>
            <button className="btn-icono" onClick={m._PMSiguienteRegistro} aria-label="Siguiente"><Icono n="next" size={16} /></button>
            <button className="btn-icono" onClick={m._PMUltimoRegistro} aria-label="Ultimo"><Icono n="last" size={16} /></button>
          </nav>
        )}
      </div>

      <div className="modelo-cuerpo">
        {!enLista ? (
          <div key={`${m.ruta.clave}`} className={`registro entrada ${m.habilitado ? 'editando' : 'solo-lectura'}`}>
            {m.cargando && !m.dt.length && m.modo !== 'nuevo' ? <div className="cargando-bloque"><span className="spinner grande" /> Cargando...</div> : children}
          </div>
        ) : (
          <div className="entrada">
            {extraBuscador}
            <GridBuscador
              estructura={m.estructura}
              filas={m.filas}
              totalFilas={m.dt.length}
              filtros={m.filtros}
              setFiltros={m.setFiltros}
              orden={m.orden}
              setOrden={m.setOrden}
              idSel={m.idSel}
              resaltado={m.resaltado}
              cargando={m.cargando}
              onSeleccionar={m.seleccionar}
              onAbrir={m.abrir}
              menuFila={(f) => [
                { texto: 'Ver', icono: 'eye', tono: 'ver', onClick: () => m.abrir(f) },
                m.puede.modificar && { texto: 'Editar', icono: 'edit', tono: 'editar', onClick: () => m._PMModificar(f) },
                ...(accionesFila ? accionesFila(f) : []),
                m.puede.eliminar && { texto: 'Eliminar', icono: 'trash', peligro: true, onClick: () => m._PMEliminar(f) },
              ].filter(Boolean)}
              acciones={(f) => (
                <span className="acciones-fila">
                  <button className="btn-icono" title="Ver" aria-label="Ver" onClick={(e) => { e.stopPropagation(); m.abrir(f); }}><Icono n="eye" size={16} /></button>
                  {m.puede.modificar && <button className="btn-icono" title="Editar" aria-label="Editar" onClick={(e) => { e.stopPropagation(); m._PMModificar(f); }}><Icono n="edit" size={16} /></button>}
                  {accionesFila && accionesFila(f).map((x) => <button key={x.texto} className="btn-icono" title={x.texto} aria-label={x.texto} onClick={(e) => { e.stopPropagation(); x.onClick(); }}><Icono n={x.icono} size={16} /></button>)}
                  {m.puede.eliminar && <button className="btn-icono peligro" title="Eliminar" aria-label="Eliminar" onClick={(e) => { e.stopPropagation(); m._PMEliminar(f); }}><Icono n="trash" size={16} /></button>}
                </span>
              )}
            />
          </div>
        )}
      </div>

      {enLista && m.puede.insertar && createPortal(
        <button className="fab" onClick={() => nuevo('fab')} aria-label={`Nuevo ${titulo}`}><Icono n="plus" size={24} /></button>,
        document.body
      )}

      {menuAbierto && menuNuevo?.length > 0 && createPortal(
        <div className="menu-nuevo-fondo" onMouseDown={() => setMenuAbierto(null)}>
          <div className={`menu-nuevo ${menuAbierto === 'fab' ? 'desde-fab' : 'desde-barra'}`} role="menu" onMouseDown={(e) => e.stopPropagation()}
            onKeyDown={(e) => e.key === 'Escape' && setMenuAbierto(null)}>
            <div className="menu-nuevo-tit">Nuevo</div>
            {menuNuevo.map((x, i) => (
              <button key={x.texto} role="menuitem" autoFocus={i === 0} onClick={() => { setMenuAbierto(null); x.accion(); }}>
                <span className="menu-nuevo-ic"><Icono n={x.icono || 'plus'} size={18} /></span>
                <span><b>{x.texto}</b>{x.detalle && <small>{x.detalle}</small>}</span>
              </button>
            ))}
          </div>
        </div>,
        document.body
      )}

      <EfectoConfirmar
        abierto={!!m.confirmar}
        titulo={m.confirmar?.titulo}
        descripcion={m.confirmar?.descripcion}
        onAceptar={m._confirmarEliminar}
        onCancelar={() => m.setConfirmar(null)}
      />
      <EfectoStockNegativo
        abierto={!!m.avisoStock}
        mensaje={m.avisoStock?.mensaje}
        productos={m.avisoStock?.productos}
        onCerrar={() => m.setAvisoStock(null)}
      />
    </section>
  );
}
