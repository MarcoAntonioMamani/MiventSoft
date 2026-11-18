// Controles de formulario: Campo (label + error como el ErrorProvider MEP),
// SwitchButton (swEstado) y ComboMulti (MultiColumnCombo de Janus)
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import Icono from './Icono';
import { palabrasDe, coincide } from '../global/buscar';

export function Campo({ etiqueta, error, children, ancho, requerido }) {
  return (
    <div className={`campo ${error ? 'con-error' : ''}`} style={ancho ? { gridColumn: `span ${ancho}` } : undefined}>
      <span className="campo-etq">{etiqueta}{requerido && <b className="req">*</b>}</span>
      {children}
      {error && <span className="campo-error" role="alert">{error}</span>}
    </div>
  );
}

export function SwitchButton({ valor, onChange, deshabilitado, textoOn = 'ACTIVO', textoOff = 'INACTIVO' }) {
  return (
    <button
      type="button"
      role="switch"
      aria-checked={!!valor}
      disabled={deshabilitado}
      className={`switch ${valor ? 'on' : ''}`}
      onClick={() => onChange(!valor)}
    >
      <span className="switch-bola" />
      <span className="switch-txt">{valor ? textoOn : textoOff}</span>
    </button>
  );
}

/**
 * ComboMulti: equivale a _prCargarComboGenerico(mCombo, dt, id, nameId, descripcionId, descripcion)
 * La lista se dibuja en un "portal" (encima de todo) para que ninguna tarjeta la recorte.
 * PC: se abre hacia abajo o hacia arriba segun el espacio. Celular: hoja inferior.
 */
export function ComboMulti({ dt = [], valor, onChange, id = 'Id', nameId = 'Codigo', descripcionId = 'Nombre', descripcion = 'Nombre', deshabilitado, error, onAbrir }) {
  const [abierto, setAbierto] = useState(false);
  const [filtro, setFiltro] = useState('');
  const [pos, setPos] = useState(null);
  const [activo, setActivo] = useState(0);
  const btn = useRef(null);
  const lista = useRef(null);

  const actual = dt.find((r) => String(r[id]) === String(valor));
  const filas = useMemo(() => {
    const w = palabrasDe(filtro); // palabras en cualquier orden, sin tildes
    return w.length ? dt.filter((r) => coincide(`${r[id]} ${r[descripcionId]}`, w)) : dt;
  }, [dt, filtro, id, descripcionId]);

  const calcular = useCallback(() => {
    const r = btn.current?.getBoundingClientRect();
    if (!r) return;
    const vw = window.innerWidth;
    const vh = window.innerHeight;
    if (vw <= 700) return setPos({ hoja: true });
    const abajo = vh - r.bottom - 10;
    const arriba = r.top - 10;
    const haciaArriba = abajo < 260 && arriba > abajo;
    const ancho = Math.min(Math.max(r.width, 300), vw - 16);
    const left = Math.min(Math.max(r.left, 8), vw - ancho - 8);
    const maxH = Math.min(360, haciaArriba ? arriba : abajo);
    setPos(haciaArriba ? { left, width: ancho, bottom: vh - r.top + 4, maxHeight: maxH } : { left, width: ancho, top: r.bottom + 4, maxHeight: maxH });
  }, []);

  const abrir = () => {
    if (deshabilitado) return;
    setFiltro('');
    setActivo(Math.max(0, dt.findIndex((r) => String(r[id]) === String(valor))));
    calcular();
    setAbierto(true);
    onAbrir?.(); // opcional: recargar la lista (la pestaña queda montada y los datos pueden estar viejos)
  };
  const cerrar = () => { setAbierto(false); btn.current?.focus(); };
  const elegir = (r) => { onChange(r[id], r); setAbierto(false); btn.current?.focus(); };

  useEffect(() => {
    if (!abierto) return undefined;
    const fuera = (e) => { if (!btn.current?.contains(e.target) && !lista.current?.contains(e.target)) setAbierto(false); };
    document.addEventListener('mousedown', fuera);
    window.addEventListener('resize', calcular);
    window.addEventListener('scroll', calcular, true);
    return () => {
      document.removeEventListener('mousedown', fuera);
      window.removeEventListener('resize', calcular);
      window.removeEventListener('scroll', calcular, true);
    };
  }, [abierto, calcular]);

  useEffect(() => {
    lista.current?.querySelector('tr.activa')?.scrollIntoView({ block: 'nearest' });
  }, [activo, abierto]);

  const teclas = (e) => {
    if (e.key === 'ArrowDown') { e.preventDefault(); setActivo((a) => Math.min(a + 1, filas.length - 1)); }
    if (e.key === 'ArrowUp') { e.preventDefault(); setActivo((a) => Math.max(a - 1, 0)); }
    if (e.key === 'Enter') { e.preventDefault(); if (filas[activo]) elegir(filas[activo]); }
    if (e.key === 'Escape') { e.preventDefault(); e.stopPropagation(); cerrar(); }
    if (e.key === 'Tab') setAbierto(false);
  };

  const contenido = abierto && pos && (
    <>
      {pos.hoja && <div className="combo-velo" onMouseDown={() => setAbierto(false)} />}
      <div ref={lista} className={`combo-lista ${pos.hoja ? 'combo-hoja' : pos.bottom !== undefined ? 'hacia-arriba' : ''}`}
        style={pos.hoja ? undefined : { left: pos.left, width: pos.width, top: pos.top, bottom: pos.bottom, maxHeight: pos.maxHeight }}
        role="dialog" aria-label={descripcion} onMouseDown={(e) => e.stopPropagation()}>
        {pos.hoja && <div className="combo-hoja-cab"><b>{descripcion}</b><button type="button" className="btn-icono" onClick={cerrar} aria-label="Cerrar"><Icono n="x" /></button></div>}
        <div className="buscar combo-buscar">
          <Icono n="search" size={16} />
          <input autoFocus placeholder="Buscar..." value={filtro} onKeyDown={teclas} onChange={(e) => { setFiltro(e.target.value); setActivo(0); }} aria-label={`Buscar ${descripcion}`} />
        </div>
        <div className="combo-filas">
          <table className="grid grid-compacta">
            <thead><tr><th style={{ width: 76 }}>{nameId}</th><th>{descripcion}</th></tr></thead>
            <tbody>
              {filas.map((r, i) => (
                <tr key={r[id]} className={`${String(r[id]) === String(valor) ? 'sel' : ''} ${i === activo ? 'activa' : ''}`}
                  onMouseEnter={() => setActivo(i)} onClick={() => elegir(r)}>
                  <td className="num">{r[id]}</td><td>{r[descripcionId]}</td>
                </tr>
              ))}
              {!filas.length && <tr><td colSpan={2} className="vacio">Sin resultados</td></tr>}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );

  return (
    <div className={`combo ${error ? 'con-error' : ''} ${abierto ? 'abierto' : ''}`}>
      <button ref={btn} type="button" className="control combo-btn" disabled={deshabilitado} aria-haspopup="listbox" aria-expanded={abierto}
        onClick={(e) => { e.preventDefault(); abierto ? setAbierto(false) : abrir(); }}
        onKeyDown={(e) => { if (!abierto && (e.key === 'ArrowDown' || e.key === 'Enter' || e.key === ' ')) { e.preventDefault(); abrir(); } }}>
        <span className={actual ? '' : 'placeholder'} title={actual ? actual[descripcionId] : undefined}>{actual ? actual[descripcionId] : 'Seleccione...'}</span>
        <Icono n="chevronDown" size={16} className={`flecha ${abierto ? 'rot' : ''}`} />
      </button>
      {contenido && createPortal(contenido, document.body)}
    </div>
  );
}

// Selector de imagen (btnImage / UsImg). Devuelve el File y muestra preview.
export function ImagenSelector({ src, deshabilitado, onArchivo }) {
  const [preview, setPreview] = useState(null);
  const input = useRef(null);
  useEffect(() => setPreview(null), [src]);
  useEffect(() => () => preview && URL.revokeObjectURL(preview), [preview]);
  const mostrar = preview || src;
  return (
    <div className="imagen-sel">
      <div className="imagen-marco">
        {mostrar ? <img src={mostrar} alt="Imagen" onError={(e) => (e.currentTarget.style.display = 'none')} /> : <Icono n="image" size={42} />}
      </div>
      {!deshabilitado && (
        <>
          <button type="button" className="btn btn-sec btn-sm" onClick={() => input.current?.click()}>
            <Icono n="image" size={15} /> Cargar imagen
          </button>
          <input ref={input} type="file" accept="image/png,image/jpeg,image/gif,image/bmp" hidden
            onChange={(e) => {
              const f = e.target.files?.[0];
              if (!f) return;
              setPreview(URL.createObjectURL(f));
              onArchivo(f);
              e.target.value = '';
            }} />
        </>
      )}
    </div>
  );
}

/**
 * ComboLibreria: ComboMulti de MAM_Clasificadores (cnnum / cndesc1) con boton "+"
 * para crear un nuevo item (equivale a Efecto tipo 10 del escritorio).
 * onNuevo(descripcion) -> Promise<{ id, data }>
 */
export function ComboLibreria({ dt, valor, onChange, titulo, deshabilitado, error, onNuevo }) {
  const [abierto, setAbierto] = useState(false);
  const [texto, setTexto] = useState('');
  const [enviando, setEnviando] = useState(false);
  const [err, setErr] = useState('');

  const guardar = async () => {
    if (!texto.trim()) return setErr('Ingrese una descripcion');
    setEnviando(true);
    setErr('');
    try {
      const r = await onNuevo(texto.trim());
      onChange(r.id);
      setAbierto(false);
      setTexto('');
    } catch (e) {
      setErr(e.message);
    } finally {
      setEnviando(false);
    }
  };

  return (
    <div className="control-con-btn">
      <ComboMulti dt={dt} valor={valor} onChange={onChange} id="cnnum" nameId="Código" descripcionId="cndesc1" descripcion={titulo} deshabilitado={deshabilitado} error={error} />
      {!deshabilitado && onNuevo && (
        <button type="button" className="btn btn-sec btn-cuadrado" title={`Nuevo ${titulo}`} aria-label={`Nuevo ${titulo}`} onClick={() => { setAbierto(true); setErr(''); }}>
          <Icono n="plus" size={16} />
        </button>
      )}
      {abierto && (
        <div className="modal-fondo" onMouseDown={() => setAbierto(false)}>
          <div className="modal modal-sm" role="dialog" aria-modal="true" onMouseDown={(e) => e.stopPropagation()}
            onKeyDown={(e) => { if (e.key === 'Escape') setAbierto(false); if (e.key === 'Enter') { e.preventDefault(); guardar(); } }}>
            <h3>Nuevo {titulo}</h3>
            <input autoFocus className="control mayus" value={texto} maxLength={100} onChange={(e) => setTexto(e.target.value.toUpperCase())} style={{ marginTop: 14 }} />
            {err && <p className="campo-error">{err}</p>}
            <div className="modal-acciones">
              <button className="btn btn-sec" onClick={() => setAbierto(false)}>Cancelar</button>
              <button className="btn btn-pri" disabled={enviando} onClick={guardar}>{enviando ? 'Guardando...' : 'Guardar'}</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

// Fecha que viene de SQL (UTC) -> dd/mm/yyyy
export function fechaCorta(v) {
  if (!v) return '';
  const d = new Date(v);
  if (Number.isNaN(d.getTime())) return String(v);
  return `${String(d.getUTCDate()).padStart(2, '0')}/${String(d.getUTCMonth() + 1).padStart(2, '0')}/${d.getUTCFullYear()}`;
}
