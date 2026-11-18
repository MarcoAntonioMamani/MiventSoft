// =====================================================================
//  "Mostrar todo": muestra un panel del inicio en pantalla completa y, al cerrar, lo devuelve a su lugar
//  con la pagina en la misma posicion en la que estaba.
//    <Ampliable>{(completo, alternar) => <section ...><BotonAmpliar completo={completo} onClick={alternar} /></section>}</Ampliable>
//  - El panel se lleva a document.body (portal): asi queda encima de la cabecera y del menu aunque algun
//    contenedor del inicio tenga opacidad o animacion (que atraparian un position: fixed).
//  - En su lugar queda un hueco del mismo alto para que la pagina no "salte" mientras esta abierto.
//  - Se cierra con el boton, con Esc o con el boton Atras del navegador (no sale del inicio).
//  - El estado del panel (vendedor elegido, punto seleccionado) se conserva: vive en el componente padre.
// =====================================================================
import { useCallback, useEffect, useLayoutEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import Icono from './Icono';

// Contenedores con scroll entre el panel y la pagina (+ la ventana), para devolverlos a su posicion
function posiciones(el) {
  const lista = [];
  for (let p = el?.parentElement; p && p !== document.body; p = p.parentElement) {
    const o = getComputedStyle(p).overflowY;
    if ((o === 'auto' || o === 'scroll') && p.scrollHeight > p.clientHeight) lista.push([p, p.scrollTop]);
  }
  return { lista, x: window.scrollX, y: window.scrollY };
}

export default function Ampliable({ children }) {
  const [completo, setCompleto] = useState(false);
  const [hueco, setHueco] = useState(null);
  const caja = useRef(null);
  const guardado = useRef(null);
  const conHistoria = useRef(false);

  const abrir = useCallback(() => {
    const el = caja.current?.firstElementChild;
    if (el) {
      guardado.current = posiciones(el);
      // ocupa el mismo lugar en la grilla del inicio (span-12, span-8, ...)
      setHueco({ alto: el.getBoundingClientRect().height, clase: [...el.classList].filter((c) => /^span-/.test(c)).join(' ') });
    }
    setCompleto(true);
  }, []);

  const cerrar = useCallback(() => {
    // si se abrio con una entrada en el historial, se consume para que Atras siga funcionando normal
    if (conHistoria.current) { conHistoria.current = false; window.history.back(); return; }
    setCompleto(false);
  }, []);

  const alternar = useCallback(() => (completo ? cerrar() : abrir()), [completo, abrir, cerrar]);

  // Abierto: sin scroll de fondo, Esc cierra y Atras cierra (en vez de salir del inicio)
  useEffect(() => {
    if (!completo) return undefined;
    document.body.classList.add('sin-scroll-fondo');
    const tecla = (e) => { if (e.key === 'Escape') cerrar(); };
    const atras = () => { if (conHistoria.current) { conHistoria.current = false; setCompleto(false); } };
    try {
      window.history.pushState({ ...(window.history.state || {}), ampliado: true }, '');
      conHistoria.current = true;
    } catch { conHistoria.current = false; }
    window.addEventListener('keydown', tecla);
    window.addEventListener('popstate', atras);
    return () => {
      document.body.classList.remove('sin-scroll-fondo');
      window.removeEventListener('keydown', tecla);
      window.removeEventListener('popstate', atras);
    };
  }, [completo, cerrar]);

  // Cerrado: el panel vuelve a su lugar y la pagina a la posicion en la que estaba
  useLayoutEffect(() => {
    if (completo || !guardado.current) return;
    const { lista, x, y } = guardado.current;
    guardado.current = null;
    setHueco(null);
    const volver = () => { lista.forEach(([p, top]) => { p.scrollTop = top; }); window.scrollTo(x, y); };
    volver();
    requestAnimationFrame(volver); // por si el mapa termina de acomodarse un cuadro despues
  }, [completo]);

  // Si el panel se desmonta abierto (cambio de pagina), el fondo no queda sin scroll
  useEffect(() => () => { document.body.classList.remove('sin-scroll-fondo'); }, []);

  const contenido = children(completo, alternar);
  if (!completo) return <div ref={caja} className="ampliable">{contenido}</div>;
  return (
    <>
      <div className={`ampliable-hueco ${hueco?.clase || ''}`} style={{ height: hueco?.alto || 0 }} aria-hidden="true" />
      {createPortal(
        <div className="ampliable-completo" role="dialog" aria-modal="true">{contenido}</div>,
        document.body,
      )}
    </>
  );
}

export function BotonAmpliar({ completo, onClick }) {
  return (
    <button type="button" className={`btn-ampliar ${completo ? 'activo' : ''}`} onClick={onClick}
      title={completo ? 'Volver al inicio (Esc)' : 'Ver el panel en pantalla completa'}>
      <Icono n={completo ? 'reducir' : 'ampliar'} size={15} />
      <span>{completo ? 'Cerrar' : 'Mostrar todo'}</span>
    </button>
  );
}
