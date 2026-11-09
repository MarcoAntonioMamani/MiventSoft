// Buscadores de productos manejables solo con teclado (como las grillas Janus del escritorio):
//  * En el buscador: Flecha abajo baja a la tabla; Enter elige la fila marcada (la primera si no se movio).
//  * En la tabla: flechas / Inicio / Fin / RePag / AvPag mueven la seleccion, Enter elige,
//    Flecha arriba en la primera fila vuelve al buscador y escribir una letra sigue la busqueda.
//  * Despues de agregar (o cancelar) el foco vuelve al buscador / a la fila.
import { useCallback, useEffect, useRef, useState } from 'react';

const PAGINA = 8;

export function useListaTeclado(filas, onElegir, { onCerrar, pausado = false } = {}) {
  const [marcada, setMarcada] = useState(0);
  const buscarRef = useRef(null);
  const tbodyRef = useRef(null);
  const total = filas.length;

  // al cambiar el filtro se vuelve a marcar la primera fila
  const firma = filas.map((f) => f?.Id ?? '').join(',');
  useEffect(() => { setMarcada(0); }, [firma]);

  // Esc cierra el buscador aunque el foco se haya perdido (p. ej. justo al cerrar la ventana de cantidad)
  useEffect(() => {
    if (!onCerrar || pausado) return undefined;
    const k = (e) => { if (e.key === 'Escape' && !e.defaultPrevented) { e.preventDefault(); onCerrar(); } };
    window.addEventListener('keydown', k);
    return () => window.removeEventListener('keydown', k);
  }, [onCerrar, pausado]);

  const filaEl = (i) => tbodyRef.current?.querySelectorAll('tr[data-fila]')[i];
  const irA = useCallback((i, enfocar = true) => {
    if (!total) return;
    const n = Math.max(0, Math.min(total - 1, i));
    setMarcada(n);
    const el = filaEl(n);
    if (el) { if (enfocar) el.focus({ preventScroll: true }); el.scrollIntoView({ block: 'nearest' }); }
  }, [total]);

  const enfocarBuscar = useCallback((seleccionar = true) => {
    const el = buscarRef.current;
    if (!el) return;
    el.focus();
    if (seleccionar) el.select();
  }, []);

  const teclaBuscar = (e) => {
    if (e.key === 'ArrowDown' && total) { e.preventDefault(); irA(marcada); }
    else if (e.key === 'Enter' && total) { e.preventDefault(); onElegir(filas[Math.min(marcada, total - 1)], marcada); }
  };

  const teclaFila = (i) => (e) => {
    const k = e.key;
    if (k === 'ArrowDown') { e.preventDefault(); irA(i + 1); }
    else if (k === 'ArrowUp') { e.preventDefault(); if (i === 0) { setMarcada(0); enfocarBuscar(false); } else irA(i - 1); }
    else if (k === 'PageDown') { e.preventDefault(); irA(i + PAGINA); }
    else if (k === 'PageUp') { e.preventDefault(); irA(i - PAGINA); }
    else if (k === 'Home') { e.preventDefault(); irA(0); }
    else if (k === 'End') { e.preventDefault(); irA(total - 1); }
    else if (k === 'Enter' || k === ' ') { e.preventDefault(); onElegir(filas[i], i); }
    else if ((k.length === 1 || k === 'Backspace') && !e.ctrlKey && !e.altKey && !e.metaKey) {
      // seguir escribiendo en el buscador sin tener que volver con el mouse
      enfocarBuscar(false);
    }
  };

  const propsFila = (i) => ({
    'data-fila': i,
    tabIndex: i === marcada ? 0 : -1,
    'aria-selected': i === marcada,
    onKeyDown: teclaFila(i),
    onFocus: () => setMarcada(i),
  });

  // volver despues del dialogo de cantidad: al buscador (agrego) o a la fila (cancelo)
  const volver = useCallback((agrego, i = marcada) => {
    setTimeout(() => (agrego ? enfocarBuscar() : irA(i)), 0);
  }, [enfocarBuscar, irA, marcada]);

  return { marcada, buscarRef, tbodyRef, teclaBuscar, propsFila, volver, enfocarBuscar };
}
