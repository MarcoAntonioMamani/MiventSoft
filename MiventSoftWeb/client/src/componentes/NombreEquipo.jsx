// =====================================================================
//  Nombre de este equipo (menu del usuario). Se guarda en este navegador y se envia en lo que se
//  modifica o elimina: la Auditoria De Registros muestra "CAJA 1 #7F3A9C" en vez de solo una IP.
// =====================================================================
import { useEffect, useRef, useState } from 'react';
import Icono from './Icono';
import { idEquipo, nombreEquipo, guardarNombreEquipo, ubicacionDisponible } from '../global/equipo';
import { esApp } from '../global/appMovil';

export default function NombreEquipo({ onCerrar, toast }) {
  const [nombre, setNombre] = useState(nombreEquipo);
  const input = useRef(null);
  useEffect(() => { input.current?.focus(); input.current?.select(); }, []);
  const grabar = (e) => {
    e.preventDefault();
    guardarNombreEquipo(nombre);
    toast?.(nombre.trim() ? `Este equipo ahora se llama "${nombre.trim()}".` : 'Se quito el nombre de este equipo.', 'ok');
    onCerrar();
  };
  return (
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <form className="modal modal-sm equipo-modal" role="dialog" aria-modal="true" aria-labelledby="equipo-tit" onSubmit={grabar}
        onMouseDown={(e) => e.stopPropagation()} onKeyDown={(e) => e.key === 'Escape' && onCerrar()}>
        <div className="modal-icono"><Icono n={esApp ? 'phone' : 'monitor'} size={26} /></div>
        <h3 id="equipo-tit">Nombre de este equipo</h3>
        <p>Identifica a esta {esApp ? 'app' : 'computadora o navegador'} en la Auditoría de registros cuando se modifica o elimina algo.</p>
        <label className="equipo-campo">
          <span>Nombre</span>
          <input ref={input} className="control" value={nombre} maxLength={60} placeholder="Ej.: CAJA 1, ALMACÉN, CELULAR JUAN"
            onChange={(e) => setNombre(e.target.value.toUpperCase())} />
        </label>
        <dl className="equipo-datos">
          <div><dt>Código de este equipo</dt><dd>#{idEquipo()}</dd></div>
          <div><dt>Ubicación</dt><dd>{ubicacionDisponible() ? 'Se registra (GPS)' : 'No disponible en este navegador'}</dd></div>
        </dl>
        <div className="modal-acciones">
          <button type="button" className="btn btn-sec" onClick={onCerrar}>Cancelar</button>
          <button type="submit" className="btn btn-pri">Guardar</button>
        </div>
      </form>
    </div>
  );
}
