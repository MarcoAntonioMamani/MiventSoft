// =====================================================================
//  Equivalente al formulario "Efecto" del escritorio:
//   tipo 3 -> Confirmacion (EfectoConfirmar)
//   tipo 6 -> Ayuda / seleccion de un registro en grilla (EfectoAyuda)
// =====================================================================
import { useEffect, useMemo, useRef, useState } from 'react';
import Icono from './Icono';

export function EfectoConfirmar({ abierto, titulo, descripcion, onAceptar, onCancelar, peligro = true }) {
  const btn = useRef(null);
  useEffect(() => {
    if (abierto) btn.current?.focus();
  }, [abierto]);
  if (!abierto) return null;
  return (
    <div className="modal-fondo" onMouseDown={onCancelar}>
      <div className="modal modal-sm" role="alertdialog" aria-modal="true" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => e.key === 'Escape' && onCancelar()}>
        <div className={`modal-icono ${peligro ? 'peligro' : ''}`}><Icono n="alert" size={26} /></div>
        <h3>{titulo}</h3>
        <p>{descripcion}</p>
        <div className="modal-acciones">
          <button className="btn btn-sec" onClick={onCancelar}>Cancelar</button>
          <button ref={btn} className={`btn ${peligro ? 'btn-peligro' : 'btn-pri'}`} onClick={onAceptar}>Aceptar</button>
        </div>
      </div>
    </div>
  );
}

export function EfectoAyuda({ abierto, titulo, dt = [], columnas, onSeleccionar, onCancelar }) {
  const [filtro, setFiltro] = useState('');
  const [pos, setPos] = useState(0);
  const input = useRef(null);

  useEffect(() => {
    if (abierto) {
      setFiltro('');
      setPos(0);
      setTimeout(() => input.current?.focus(), 0);
    }
  }, [abierto]);

  const filas = useMemo(() => {
    const f = filtro.trim().toLowerCase();
    if (!f) return dt;
    return dt.filter((r) => columnas.some((c) => String(r[c.campo] ?? '').toLowerCase().includes(f)));
  }, [dt, filtro, columnas]);

  if (!abierto) return null;

  const teclas = (e) => {
    if (e.key === 'ArrowDown') { e.preventDefault(); setPos((p) => Math.min(p + 1, filas.length - 1)); }
    if (e.key === 'ArrowUp') { e.preventDefault(); setPos((p) => Math.max(p - 1, 0)); }
    if (e.key === 'Enter' && filas[pos]) onSeleccionar(filas[pos]);
    if (e.key === 'Escape') onCancelar();
  };

  return (
    <div className="modal-fondo" onMouseDown={onCancelar}>
      <div className="modal modal-lg" role="dialog" aria-modal="true" aria-label={titulo} onMouseDown={(e) => e.stopPropagation()} onKeyDown={teclas}>
        <div className="modal-cab">
          <h3>{titulo}</h3>
          <button className="btn-icono" onClick={onCancelar} aria-label="Cerrar"><Icono n="x" /></button>
        </div>
        <div className="buscar">
          <Icono n="search" size={16} />
          <input ref={input} value={filtro} onChange={(e) => { setFiltro(e.target.value); setPos(0); }} placeholder="Buscar..." />
        </div>
        <div className="tabla-scroll alto-ayuda">
          <table className="grid">
            <thead><tr>{columnas.map((c) => <th key={c.campo} style={{ width: c.tamano }}>{c.titulo}</th>)}</tr></thead>
            <tbody>
              {filas.map((r, i) => (
                <tr key={i} className={i === pos ? 'sel' : ''} onClick={() => setPos(i)} onDoubleClick={() => onSeleccionar(r)}>
                  {columnas.map((c) => <td key={c.campo}>{r[c.campo]}</td>)}
                </tr>
              ))}
              {!filas.length && <tr><td colSpan={columnas.length} className="vacio">Sin resultados</td></tr>}
            </tbody>
          </table>
        </div>
        <div className="modal-acciones">
          <span className="ayuda-tip">Doble clic o Enter para seleccionar</span>
          <button className="btn btn-sec" onClick={onCancelar}>Cancelar</button>
          <button className="btn btn-pri" disabled={!filas[pos]} onClick={() => onSeleccionar(filas[pos])}>Seleccionar</button>
        </div>
      </div>
    </div>
  );
}
