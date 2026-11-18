// =====================================================================
//  Pestañas vivas: cada programa abierto sigue montado aunque no se vea (Tec_Principal).
//  Por eso los datos que una pantalla carga "al abrir" (combos, buscador, stock) quedarian viejos.
//  useAlActivar(fn): ejecuta fn al montar, CADA VEZ que la pestaña vuelve a ser la visible y cada vez
//  que se pulsa "Actualizar" (refrescar) en esa pestaña: siempre se trae lo actual de la BD.
// =====================================================================
import { createContext, createElement, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';

// activa = la pestaña es la visible. Fuera de las pestañas (Inicio, modales sueltos): siempre activa, sin refrescar.
export const VistaActiva = createContext({ activa: true, version: 0, refrescar: null });

/** Envuelve una pestaña: le da su estado activa/oculta y su boton "Actualizar" (refrescar). */
export function ProveedorVista({ activa, children }) {
  const [version, setVersion] = useState(0);
  const refrescar = useCallback(() => setVersion((v) => v + 1), []);
  const valor = useMemo(() => ({ activa, version, refrescar }), [activa, version, refrescar]);
  return createElement(VistaActiva.Provider, { value: valor }, children);
}

/** Funcion del boton "Actualizar": vuelve a ejecutar todas las cargas (useAlActivar) de la pestaña.
 *  respaldo: lo que se ejecuta si la pantalla no esta dentro de una pestaña. */
export function useRefrescar(respaldo) {
  const { refrescar } = useContext(VistaActiva);
  return refrescar || respaldo || (() => {});
}

/**
 * @param {() => void} fn   carga a ejecutar (siempre se usa la version del ultimo render)
 * @param {{ alMontar?: boolean, alRefrescar?: boolean }} [opciones]
 *   alMontar=false: no se ejecuta al montar (la carga inicial la hace otro efecto)
 *   alRefrescar=false: no se ejecuta con el boton "Actualizar" (quien lo pulsa ya recarga eso)
 */
export function useAlActivar(fn, { alMontar = true, alRefrescar = true } = {}) {
  const { activa, version } = useContext(VistaActiva);
  const ref = useRef(fn);
  ref.current = fn;
  const antes = useRef(null); // null = aun no montado
  useEffect(() => {
    const montando = antes.current === null;
    if (activa && antes.current !== true && (alMontar || !montando)) ref.current();
    antes.current = activa;
  }, [activa]); // eslint-disable-line react-hooks/exhaustive-deps
  const versionAntes = useRef(version);
  useEffect(() => {
    if (versionAntes.current === version) return;
    versionAntes.current = version;
    if (alRefrescar) ref.current();
  }, [version]); // eslint-disable-line react-hooks/exhaustive-deps
}
