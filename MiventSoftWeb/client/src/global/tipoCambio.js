// =====================================================================
//  Tipo de cambio vigente: el ULTIMO guardado en la tabla TipoCambio
//  (misma consulta que L_Validar_Usuario / Global_TipoCambio del escritorio).
//  Sin cache: se vuelve a leer de la BD al abrir, al volver a la ventana/app
//  y cuando alguien lo actualiza desde el boton "Tipo Cambio" de la cabecera.
// =====================================================================
import { useCallback, useEffect, useState } from 'react';
import { api } from '../api/http';

const EVENTO = 'sanbit:tipo-cambio';

// el boton de la cabecera avisa a las pantallas abiertas (ventas, proformas) el nuevo monto
export const avisarTipoCambio = (monto) => window.dispatchEvent(new CustomEvent(EVENTO, { detail: Number(monto) || 0 }));

// [tc, leer]: tc = null mientras carga; 0 = no hay tipo de cambio valido guardado
export function useTipoCambio() {
  const [tc, setTc] = useState(null);
  const leer = useCallback(() => api.get('/api/tipo-cambio').then((r) => setTc(Number(r.data?.monto) || 0)).catch(() => {}), []);
  useEffect(() => {
    leer();
    const alCambiar = (e) => setTc(Number(e.detail) || 0);
    const alVolver = () => { if (document.visibilityState === 'visible') leer(); };
    window.addEventListener(EVENTO, alCambiar);
    window.addEventListener('focus', leer);
    document.addEventListener('visibilitychange', alVolver);
    return () => {
      window.removeEventListener(EVENTO, alCambiar);
      window.removeEventListener('focus', leer);
      document.removeEventListener('visibilitychange', alVolver);
    };
  }, [leer]);
  return [tc, leer];
}

// Total en dolares con el tipo de cambio vigente (null si no hay tipo de cambio valido)
export const aDolares = (bs, tc) => (Number(tc) > 0 ? Number(bs || 0) / Number(tc) : null);
