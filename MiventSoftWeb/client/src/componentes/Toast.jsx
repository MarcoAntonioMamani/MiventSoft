// Equivalente a ToastNotification.Show(Me, mensaje, ..., eToastGlowColor.Green/Red, TopCenter)
// Tarjeta con titulo por tipo, mensaje, boton cerrar y barra de tiempo (se pausa con el mouse).
// Los mensajes en MAYUSCULAS heredados del escritorio se muestran en forma de oracion.
import { createContext, useCallback, useContext, useEffect, useRef, useState } from 'react';
import Icono from './Icono';

const ToastContext = createContext(() => {});
const MAX = 4;
const TIPOS = {
  ok: { titulo: 'Operación exitosa', icono: 'check' },
  error: { titulo: 'No se pudo completar', icono: 'alert' },
  aviso: { titulo: 'Atención', icono: 'alert' },
  info: { titulo: 'Información', icono: 'info' },
};
// Palabras frecuentes del escritorio sin tilde
const TILDES = { exito: 'éxito', codigo: 'código', modificacion: 'modificación', eliminacion: 'eliminación', informacion: 'información', operacion: 'operación',
  credito: 'crédito', creditos: 'créditos', deposito: 'depósito', maximo: 'máximo', minimo: 'mínimo', numero: 'número', valido: 'válido', seleccion: 'selección' };

const conTildes = (s) => s.replace(/[A-Za-zñÑ]+/g, (w) => {
  const t = TILDES[w.toLowerCase()];
  if (!t) return w;
  if (w === w.toUpperCase()) return t.toUpperCase();
  return w[0] === w[0].toUpperCase() ? t[0].toUpperCase() + t.slice(1) : t;
});

export function oracion(txt) {
  const s = String(txt ?? '');
  const letras = s.replace(/[^A-Za-zÁÉÍÓÚÑáéíóúñ]/g, '');
  if (letras.length < 4 || letras !== letras.toUpperCase()) return conTildes(s); // ya viene en minusculas
  return conTildes(s.toLowerCase())
    .replace(/(^\s*|[.!?:]\s+)([a-záéíóúñ¿¡])/g, (m, a, b) => a + b.toUpperCase());
}

function Toast({ t, onCerrar }) {
  const tipo = TIPOS[t.tipo] || TIPOS.info;
  const [pausa, setPausa] = useState(false);
  const resto = useRef(t.ms);
  const inicio = useRef(Date.now());
  useEffect(() => {
    if (pausa) return undefined;
    inicio.current = Date.now();
    const h = setTimeout(() => onCerrar(t.id), resto.current);
    return () => { clearTimeout(h); resto.current -= Date.now() - inicio.current; };
  }, [pausa, t.id, onCerrar]);
  return (
    <div className={`toast toast-${t.tipo}`} role={t.tipo === 'error' ? 'alert' : 'status'} onMouseEnter={() => setPausa(true)} onMouseLeave={() => setPausa(false)}>
      <span className="toast-ic"><Icono n={tipo.icono} size={18} /></span>
      <div className="toast-txt">
        <b>{tipo.titulo}</b>
        <span>{oracion(t.mensaje)}</span>
      </div>
      <button type="button" className="toast-x" onClick={() => onCerrar(t.id)} aria-label="Cerrar notificación"><Icono n="x" size={16} /></button>
      <i className="toast-barra" style={{ animationDuration: `${t.ms}ms`, animationPlayState: pausa ? 'paused' : 'running' }} />
    </div>
  );
}

export function ToastProvider({ children }) {
  const [toasts, setToasts] = useState([]);
  const cerrar = useCallback((id) => setToasts((t) => t.filter((x) => x.id !== id)), []);
  const mostrar = useCallback((mensaje, tipo = 'ok', ms) => {
    mostrar.ultimo = Date.now(); // ModeloF1 lo usa para no repetir el aviso generico
    const id = Math.random().toString(36).slice(2);
    const dur = ms || (tipo === 'error' ? 6500 : 4500);
    // mismo mensaje repetido: no se apila
    setToasts((t) => [...t.filter((x) => !(x.mensaje === mensaje && x.tipo === tipo)), { id, mensaje, tipo, ms: dur }].slice(-MAX));
  }, []);

  return (
    <ToastContext.Provider value={mostrar}>
      {children}
      <div className="toasts" aria-live="polite">
        {toasts.map((t) => <Toast key={t.id} t={t} onCerrar={cerrar} />)}
      </div>
    </ToastContext.Provider>
  );
}

export const useToast = () => useContext(ToastContext);
