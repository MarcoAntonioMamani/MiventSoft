// =====================================================================
//  Notificacion de la barra superior: pedidos del celular observados que no pueden sincronizar (script 09)
//  Solo para quien tiene el programa btnPedidosObservados (Administrador). Consulta cada 60 s, al volver a
//  la pestaña del navegador y cuando la pantalla de pedidos observados cambia algo.
//  Si sube la cantidad o hay un pedido observado nuevo (fecha "ultima" distinta), avisa con un toast.
//  Si la consulta falla no muestra nada (no es un error del usuario).
// =====================================================================
import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSesion } from '../global/Sesion';
import { PROGRAMAS_WEB } from '../global/programas';
import { useToast } from './Toast';
import Icono from './Icono';
import { EVENTO_CAMBIO } from '../paginas/distribucion/Tec_PedidosObservados';

const PROGRAMA = 'BTNPEDIDOSOBSERVADOS';
const CADA_MS = 60 * 1000;
const CLAVE_VISTO = 'mivent_pedidos_observados_visto';

function leerVisto() {
  try { return JSON.parse(localStorage.getItem(CLAVE_VISTO) || 'null'); } catch { return null; }
}
function guardarVisto(v) {
  try { localStorage.setItem(CLAVE_VISTO, JSON.stringify(v)); } catch { /* navegador sin almacenamiento: solo no recuerda lo visto */ }
}

export default function AvisoObservados() {
  const { sesion, tienePrograma } = useSesion();
  const permitido = tienePrograma(PROGRAMA);
  const toast = useToast();
  const navegar = useNavigate();
  const [res, setRes] = useState(null);
  const enCurso = useRef(false);

  useEffect(() => {
    if (!permitido) { setRes(null); return undefined; }
    let vivo = true;
    const consultar = async () => {
      if (enCurso.current || document.visibilityState === 'hidden') return;
      enCurso.current = true;
      try {
        // fetch directo: sin la barra de progreso global ni el aviso de sesion vencida (es una consulta de fondo)
        const r = await fetch('/api/distribucion/pedidos-observados/resumen', { credentials: 'include', cache: 'no-store' });
        if (!r.ok) return;
        const j = await r.json();
        const d = j?.data;
        if (!vivo || !d || d.permitido === false) { if (vivo) setRes(null); return; }
        setRes(d);
        const visto = leerVisto();
        const usuario = sesion?.gs_login || '';
        const nuevo = d.pendientes > 0 && (!visto || visto.usuario !== usuario || d.pendientes > visto.pendientes || (d.ultima && d.ultima !== visto.ultima));
        if (nuevo) {
          toast(`Hay ${d.pendientes} pedido${d.pendientes === 1 ? '' : 's'} del celular observado${d.pendientes === 1 ? '' : 's'} que no ${d.pendientes === 1 ? 'puede' : 'pueden'} sincronizar.`, 'aviso', 9000);
        }
        guardarVisto({ usuario, pendientes: d.pendientes, ultima: d.ultima || null });
      } catch {
        // sin conexion: se reintenta en la siguiente vuelta
      } finally {
        enCurso.current = false;
      }
    };
    consultar();
    const h = setInterval(consultar, CADA_MS);
    const alVolver = () => { if (document.visibilityState === 'visible') consultar(); };
    document.addEventListener('visibilitychange', alVolver);
    window.addEventListener('focus', alVolver);
    window.addEventListener(EVENTO_CAMBIO, consultar);
    return () => {
      vivo = false;
      clearInterval(h);
      document.removeEventListener('visibilitychange', alVolver);
      window.removeEventListener('focus', alVolver);
      window.removeEventListener(EVENTO_CAMBIO, consultar);
    };
  }, [permitido, sesion?.gs_login]); // eslint-disable-line react-hooks/exhaustive-deps

  if (!permitido || !res || !(res.pendientes > 0)) return null;
  const n = res.pendientes;
  const detalle = [res.sinStock ? `${res.sinStock} sin stock` : '', res.otros ? `${res.otros} con otro problema` : ''].filter(Boolean).join(', ');
  return (
    <button type="button" className="aviso-observados" onClick={() => navegar(PROGRAMAS_WEB[PROGRAMA].ruta)}
      title={`${n} pedido${n === 1 ? '' : 's'} del celular observado${n === 1 ? '' : 's'}${detalle ? ` (${detalle})` : ''}. Clic para revisarlos.`}
      aria-label={`${n} pedidos del celular observados`}>
      <Icono n="alert" size={16} /><span className="ao-txt">Observados</span><b>{n}</b>
    </button>
  );
}
