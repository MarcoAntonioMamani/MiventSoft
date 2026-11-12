// =====================================================================
//  KAILY: btnTipoCambio (barra superior de Tec_Principal) -> ActualizarTipoCambio.vb
//  "Tipo Cambio: 6.96"; al guardar: InsertarTipoCambio (MAM_Ventas @tipo 7)
// =====================================================================
import { useState } from 'react';
import { createPortal } from 'react-dom';
import { api } from '../api/http';
import { useToast } from './Toast';
import Icono from './Icono';
import { useTipoCambio, avisarTipoCambio } from '../global/tipoCambio';

const URL = '/api/tipo-cambio';

export default function TipoCambio() {
  const toast = useToast();
  const [monto, leer] = useTipoCambio(); // siempre el ultimo guardado en la BD (tambien si lo cambian desde el escritorio)
  const [abierto, setAbierto] = useState(false);
  const sinTc = monto !== null && !(monto > 0);
  return (
    <>
      <button type="button" className={`chip-incremento ${sinTc ? 'chip-alerta' : ''}`} onClick={() => { leer(); setAbierto(true); }}
        title={sinTc ? 'El último tipo de cambio guardado es 0: actualícelo' : 'Actualizar Tipo De Cambio'}>
        <Icono n="dollar" size={15} /><span className="ci-txt">Tipo Cambio:</span><b>{monto === null ? '…' : Number(monto).toString()}</b>
      </button>
      {abierto && <Dialogo actual={monto} onCerrar={(nuevo) => {
        setAbierto(false);
        if (nuevo !== undefined) { avisarTipoCambio(nuevo); toast('Tipo De Cambio Actualizado Correctamente', 'ok'); }
      }} />}
    </>
  );
}

function Dialogo({ actual, onCerrar }) {
  const toast = useToast();
  const [txt, setTxt] = useState(actual !== null ? String(actual) : '');
  const [guardando, setGuardando] = useState(false);

  // btnGuardar_Click
  const guardar = async () => {
    const m = Number.parseFloat(txt);
    if (!txt.trim() || !Number.isFinite(m) || m <= 0) { toast('Debe Ingresar un Valor Correcto en Tipo De Cambio', 'error'); return; }
    setGuardando(true);
    try { const r = await api.post(URL, { monto: m }); onCerrar(r.data.monto); } catch (e) { toast(e.message, 'error'); } finally { setGuardando(false); }
  };

  return createPortal(
    <div className="modal-fondo" onMouseDown={() => onCerrar()}>
      <div className="modal modal-sm dialogo-incremento" role="dialog" aria-modal="true" aria-label="Actualizar tipo de cambio" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') onCerrar(); }}>
        <div className="modal-cab"><h3>Actualizar Tipo De Cambio</h3><button className="btn-icono" onClick={() => onCerrar()} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="inc-monto">
          <span>Tipo Cambio</span>
          <input autoFocus className="control num" type="text" inputMode="decimal" value={txt} aria-label="Tipo de cambio"
            onChange={(e) => /^\d*[.,]?\d{0,4}$/.test(e.target.value) && setTxt(e.target.value.replace(',', '.'))}
            onKeyDown={(e) => e.key === 'Enter' && guardar()} onFocus={(e) => e.target.select()} />
        </div>
        <div className="modal-acciones">
          <button className="btn btn-sec" onClick={() => onCerrar()}>Salir</button>
          <button className="btn btn-pri" onClick={guardar} disabled={guardando}>{guardando ? <span className="spinner" /> : <Icono n="save" size={15} />} Guardar</button>
        </div>
      </div>
    </div>,
    document.body,
  );
}
