// =====================================================================
//  F_AsignacionZona.vb -> Asignación de zonas al personal  (SP: sp_Mam_Zonas @tipo 5 / 6)
//  Izquierda el personal; derecha sus zonas con casilla "asignada". Guardar = @tipo 6.
// =====================================================================
import { useEffect, useMemo, useState } from 'react';
import { useToast } from '../../componentes/Toast';
import { usePermisos } from '../../global/Sesion';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { palabrasDe, coincide } from '../../global/buscar';

const URL = '/api/distribucion/zonas';

export default function Tec_AsignacionZona() {
  const toast = useToast();
  const puede = usePermisos();
  const [personal, setPersonal] = useState([]);
  const [q, setQ] = useState('');
  const [sel, setSel] = useState(null);
  const [zonas, setZonas] = useState(null);
  const [marcadas, setMarcadas] = useState(new Set());
  const [original, setOriginal] = useState(new Set());
  const [guardando, setGuardando] = useState(false);

  useEffect(() => { api.get(`${URL}/personal`).then((r) => setPersonal(r.data)).catch((e) => toast(e.message, 'error')); }, [toast]);
  const elegir = async (p) => {
    setSel(p); setZonas(null);
    try {
      const r = await api.get(`${URL}/${p.Id}`);
      const s = new Set(r.data.filter((z) => z.asignada).map((z) => z.idZona));
      setZonas(r.data); setMarcadas(s); setOriginal(new Set(s));
    } catch (e) { toast(e.message, 'error'); }
  };
  const filtrado = useMemo(() => { const w = palabrasDe(q); return personal.filter((p) => coincide(`${p.Id} ${p.NombrePersonal} ${p.Tipo ?? ''}`, w)); }, [personal, q]);
  const cambio = zonas && (marcadas.size !== original.size || [...marcadas].some((z) => !original.has(z)));
  const alternar = (id) => setMarcadas((s) => { const n = new Set(s); if (n.has(id)) n.delete(id); else n.add(id); return n; });
  const guardar = async () => {
    setGuardando(true);
    try { const r = await api.put(`${URL}/${sel.Id}`, { zonas: [...marcadas] }); toast(r.mensaje, 'ok'); setOriginal(new Set(marcadas)); } catch (e) { toast(e.message, 'error'); } finally { setGuardando(false); }
  };
  const editable = puede.modificar || puede.insertar;

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo">
          <span className="modelo-icono"><Icono n="target" size={22} /></span>
          <div><h1>Asignación de zonas</h1><p>Zonas de reparto por personal</p></div>
        </div>
      </header>
      <div className="modelo-cuerpo zona-asig">
        <div className="tarjeta za-personal">
          <h2 className="tarjeta-tit">Personal <span className="contador">{filtrado.length}</span></h2>
          <div className="buscar"><Icono n="search" size={16} /><input className="control" placeholder="Buscar personal..." value={q} onChange={(e) => setQ(e.target.value)} aria-label="Buscar personal" /></div>
          <ul className="za-lista">
            {filtrado.map((p) => (
              <li key={p.Id}><button type="button" className={sel?.Id === p.Id ? 'activo' : ''} onClick={() => elegir(p)}>
                <span className="za-av">{tit(p.NombrePersonal).split(' ').map((x) => x[0]).slice(0, 2).join('')}</span>
                <span><b>{tit(p.NombrePersonal)}</b><small>{tit(p.Tipo ?? '')} · Cód. {p.Id}</small></span>
              </button></li>
            ))}
          </ul>
        </div>
        <div className="tarjeta za-zonas">
          {!sel ? <div className="reporte-vacio"><Icono n="target" size={28} /><span>Elija un personal para ver sus zonas</span></div> : (
            <>
              <div className="za-cab">
                <h2 className="tarjeta-tit">Zonas de {tit(sel.NombrePersonal)} <span className="contador">{marcadas.size}</span></h2>
                {editable && <button className="btn btn-pri" onClick={guardar} disabled={!cambio || guardando}><Icono n="save" size={16} /> Guardar</button>}
              </div>
              {!zonas ? <div className="cargando-bloque"><span className="spinner" /> Cargando...</div> : (
                <div className="za-grid">
                  {zonas.map((z) => (
                    <label key={z.idZona} className={`za-zona ${marcadas.has(z.idZona) ? 'on' : ''}`}>
                      <input type="checkbox" checked={marcadas.has(z.idZona)} onChange={() => alternar(z.idZona)} disabled={!editable} />
                      <Icono n="map" size={16} /><span>{tit(z.NombreZona)}</span>
                    </label>
                  ))}
                  {!zonas.length && <p className="txt-suave">No hay zonas registradas</p>}
                </div>
              )}
            </>
          )}
        </div>
      </div>
    </section>
  );
}
