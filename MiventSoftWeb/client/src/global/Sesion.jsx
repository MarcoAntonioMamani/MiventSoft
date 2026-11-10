// =====================================================================
//  Sesion global (equivale a gs_user, gi_userNumi, gi_userRol, gi_userSuc de P_Global.vb)
// =====================================================================
import { createContext, useCallback, useContext, useEffect, useState } from 'react';
import { api, alExpirarSesion } from '../api/http';

const SesionContext = createContext(null);

export function SesionProvider({ children }) {
  const [sesion, setSesion] = useState(null);
  const [cargando, setCargando] = useState(true);
  const [aviso, setAviso] = useState('');

  const cargarSesion = useCallback(async () => {
    try {
      const r = await api.get('/api/auth/sesion');
      setSesion(r.data);
    } catch {
      setSesion(null);
    } finally {
      setCargando(false);
    }
  }, []);

  useEffect(() => {
    alExpirarSesion((msg) => {
      setAviso(msg || 'La sesion expiro.');
      setSesion(null);
    });
    cargarSesion();
  }, [cargarSesion]);

  const ingresar = async (usuario, password) => {
    await api.post('/api/auth/login', { usuario, password });
    setAviso('');
    await cargarSesion();
  };

  const salir = async () => {
    await api.post('/api/auth/logout').catch(() => {});
    setSesion(null);
  };

  const tienePrograma = (idPrograma) =>
    !!sesion?.modulos?.some((m) => m.programas.some((p) => p.IdPrograma === idPrograma.toUpperCase()));

  // { insertar, modificar, eliminar } del rol para el programa (RolesDetalles)
  const permisos = (idPrograma) => {
    const id = String(idPrograma).toUpperCase();
    for (const m of sesion?.modulos || []) {
      const p = m.programas.find((x) => x.IdPrograma === id);
      if (p) return { insertar: p.I !== 0, modificar: p.M !== 0, eliminar: p.E !== 0 };
    }
    return { insertar: false, modificar: false, eliminar: false };
  };

  return (
    <SesionContext.Provider value={{ sesion, cargando, aviso, ingresar, salir, tienePrograma, permisos }}>
      {children}
    </SesionContext.Provider>
  );
}

export const useSesion = () => useContext(SesionContext);

// Permisos del programa abierto (lo provee App.jsx); por defecto todo permitido
export const PermisoContext = createContext({ insertar: true, modificar: true, eliminar: true });
export const usePermisos = () => useContext(PermisoContext);
