// =====================================================================
//  btnIncrementoVenta (barra superior de Tec_Principal) -> Tec_IncrementoVenta + Rep_HistorialIncrementoVenta
// =====================================================================
import { useEffect, useState } from 'react';
import { createPortal } from 'react-dom';
import { api } from '../api/http';
import { useToast } from './Toast';
import TablaReporte from './TablaReporte';
import Icono from './Icono';

const dos = (n) => String(n).padStart(2, '0');
const isoLocal = (d) => `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`;
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

export default function IncrementoVenta() {
  const toast = useToast();
  const [monto, setMonto] = useState(null);
  const [abierto, setAbierto] = useState(false);
  const cargar = () => api.get('/api/incremento').then((r) => setMonto(r.data.monto)).catch(() => {});
  useEffect(() => { cargar(); }, []);
  return (
    <>
      <button type="button" className="chip-incremento" onClick={() => setAbierto(true)} title="Incremento diario que se suma a cada venta nueva">
        <Icono n="dollar" size={15} /><span className="ci-txt">Incremento</span><b>Bs {monto === null ? '…' : bs(monto)}</b>
      </button>
      {abierto && <Dialogo actual={monto} onCerrar={(nuevo) => { setAbierto(false); if (nuevo !== undefined) { setMonto(nuevo); toast('Incremento Del Dia Actualizado Correctamente'.toUpperCase(), 'ok'); } }} />}
    </>
  );
}

function Dialogo({ actual, onCerrar }) {
  const toast = useToast();
  const [txt, setTxt] = useState(actual !== null ? Number(actual).toFixed(2) : '2.00');
  const [historial, setHistorial] = useState(false);
  const [rango, setRango] = useState(() => { const h = new Date(); const d = new Date(); d.setDate(d.getDate() - 30); return { desde: isoLocal(d), hasta: isoLocal(h) }; });
  const [filas, setFilas] = useState([]);
  const [cargando, setCargando] = useState(false);
  const [guardando, setGuardando] = useState(false);

  const guardar = async () => {
    const m = Number.parseFloat(txt);
    if (!Number.isFinite(m) || m <= 0) { toast('Ingrese Un Monto Valido Mayor A Cero'.toUpperCase(), 'error'); return; }
    setGuardando(true);
    try { const r = await api.put('/api/incremento', { monto: m }); onCerrar(r.data.monto); } catch (e) { toast(e.message, 'error'); } finally { setGuardando(false); }
  };
  const buscar = async () => {
    setCargando(true);
    try {
      const r = await api.get(`/api/incremento/historial?desde=${rango.desde}&hasta=${rango.hasta}`);
      setFilas(r.data);
      if (!r.data.length) toast('No Existen Cambios Registrados En El Rango Seleccionado'.toUpperCase(), 'error');
    } catch (e) { toast(e.message, 'error'); } finally { setCargando(false); }
  };
  useEffect(() => { if (historial) buscar(); }, [historial]); // eslint-disable-line react-hooks/exhaustive-deps

  return createPortal(
    <div className="modal-fondo" onMouseDown={() => onCerrar()}>
      <div className={`modal ${historial ? 'modal-lg' : 'modal-sm'} dialogo-incremento`} role="dialog" aria-modal="true" aria-label="Incremento de venta" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') onCerrar(); }}>
        <div className="modal-cab"><h3>Incremento de venta del día</h3><button className="btn-icono" onClick={() => onCerrar()} aria-label="Cerrar"><Icono n="x" /></button></div>
        <p className="inc-ayuda">Se suma al total de cada venta <b>nueva</b>. Las ventas ya registradas conservan su incremento.</p>
        <div className="inc-monto">
          <span>Bs</span>
          <input autoFocus className="control num" type="text" inputMode="decimal" value={txt} aria-label="Monto del incremento"
            onChange={(e) => /^\d*[.,]?\d{0,2}$/.test(e.target.value) && setTxt(e.target.value.replace(',', '.'))}
            onKeyDown={(e) => e.key === 'Enter' && guardar()} onFocus={(e) => e.target.select()} />
        </div>
        <div className="modal-acciones">
          <button className="btn btn-txt" onClick={() => setHistorial((h) => !h)}><Icono n="list" size={15} /> {historial ? 'Ocultar historial' : 'Ver historial'}</button>
          <button className="btn btn-sec" onClick={() => onCerrar()}>Cancelar</button>
          <button className="btn btn-pri" onClick={guardar} disabled={guardando}>{guardando ? <span className="spinner" /> : <Icono n="save" size={15} />} Guardar</button>
        </div>
        {historial && (
          <div className="inc-historial">
            <div className="filtros-lista compacto">
              <label className="filtro"><span>Desde</span><input type="date" className="control" value={rango.desde} max={rango.hasta} onChange={(e) => e.target.value && setRango((r) => ({ ...r, desde: e.target.value }))} /></label>
              <label className="filtro"><span>Hasta</span><input type="date" className="control" value={rango.hasta} min={rango.desde} onChange={(e) => e.target.value && setRango((r) => ({ ...r, hasta: e.target.value }))} /></label>
              <button className="btn btn-sec" onClick={buscar}><Icono n="search" size={15} /> Buscar</button>
            </div>
            <TablaReporte titulo="Historial de incremento de venta" subtitulo={`Del ${rango.desde} al ${rango.hasta}`} cargando={cargando} filas={filas} vacio="Sin cambios en el rango"
              columnas={[{ campo: 'Fecha', titulo: 'Día', tipo: 'fecha' }, { campo: 'Monto', titulo: 'Monto (Bs)', tipo: 'bs' }, { campo: 'Usuario', titulo: 'Modificado por' }, { campo: 'FechaHoraModificacion', titulo: 'Fecha y hora del cambio' }]} />
          </div>
        )}
      </div>
    </div>,
    document.body,
  );
}
