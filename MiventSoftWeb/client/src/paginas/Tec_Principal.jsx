// =====================================================================
//  Tec_Principal.vb: menu de modulos + programas segun privilegios del rol
//  Cada modulo y programa tiene su ruta (URL) y migas de pan navegables.
//  Menu lateral en acordeon: un solo modulo desplegado a la vez; al abrir otro, el anterior se pliega.
//  Al entrar a un programa (o al modulo) se despliega su modulo y se pliegan los demas.
//  Programas abiertos (como el escritorio): cada programa queda en una pestaña y SIGUE VIVO al cambiar
//  a otro (filtros, formulario a medio llenar, pagina de la lista). Volver a abrirlo desde el menu
//  lleva a la pestaña tal como estaba. Cada pestaña tiene su propia ruta (<Routes location>).
//  Como el escritorio: la barra de pestañas se ve solo mientras se trabaja en un programa; en el Inicio
//  o en el menu de un modulo se oculta. El item "Programas abiertos" del menu lateral se selecciona solo
//  al abrir un programa y lleva de vuelta al ultimo que se estaba usando.
//  Pestañas en 2 filas como maximo: si no entran, se reparten en 2 filas y la barra se desplaza a la derecha.
//  Maximizar: el programa ocupa toda la pantalla (sin menu lateral ni cabecera); Restaurar vuelve al menu.
// =====================================================================
import { createContext, useContext, useEffect, useLayoutEffect, useMemo, useRef, useState } from 'react';
import { Link, NavLink, Outlet, Routes, useLocation, useNavigate } from 'react-router-dom';
import { useSesion } from '../global/Sesion';
import { PROGRAMAS_WEB, imgPrograma, iconoModulo, slug, titulo, ubicar } from '../global/programas';
import TipoCambio from '../componentes/TipoCambio';
import Reloj from '../componentes/Reloj';
import { ProveedorVista } from '../global/vistaActiva';
import Icono from '../componentes/Icono';
import { MarcaSistema, NOMBRE_SISTEMA, LEMA_SISTEMA } from '../componentes/Logo';
import { useToast } from '../componentes/Toast';
import NombreEquipo from '../componentes/NombreEquipo';
import { idEquipo, nombreEquipo, ubicacionAuditoria, ubicacionDisponible } from '../global/equipo';
import { esApp } from '../global/appMovil';

const MAX_PESTANAS = 10;
const ANCHO_PESTANA = 176; const ESPACIO_PESTANA = 6;
const CLAVE_MAX = 'santit.maximizado';
const leerMax = () => { try { return localStorage.getItem(CLAVE_MAX) === '1'; } catch { return false; } };
const CLAVE_GUARDADO = 'santit.pestanas';
const leerGuardadas = () => { try { const x = JSON.parse(sessionStorage.getItem(CLAVE_GUARDADO) || '[]'); return Array.isArray(x) ? x.filter((p) => p && p.ruta && p.url && p.id) : []; } catch { return []; } };
const guardar = (ps) => { try { sessionStorage.setItem(CLAVE_GUARDADO, JSON.stringify(ps)); } catch { /* sin almacenamiento */ } };
const enEdicion = (url) => /\/(nuevo|[^/]+\/editar)(\?|$)/.test(url);

// Para los enlaces a programas (menu, mosaicos, accesos): si ya esta abierto, volver a como estaba
const PestanasContext = createContext({ urlDe: (ruta) => ruta });
export const usePestanas = () => useContext(PestanasContext);

export default function Tec_Principal({ rutas }) {
  const { sesion, salir } = useSesion();
  const loc = useLocation();
  const navegar = useNavigate();
  const { modulo, programa } = ubicar(sesion.modulos, loc.pathname);
  const activa = programa ? PROGRAMAS_WEB[programa.IdPrograma]?.ruta : null; // ruta base del programa visible
  // al recargar solo vuelven las pestañas de programas que el rol todavia tiene asignados
  const [pestanas, setPestanas] = useState(() => {
    const ids = new Set(sesion.modulos.flatMap((m) => m.programas.map((p) => String(p.IdPrograma).toUpperCase())));
    return leerGuardadas().filter((p) => ids.has(String(p.id).toUpperCase()));
  }); // [{ ruta, id, url }]
  const [cerrando, setCerrando] = useState(null); // pestaña con un registro a medio editar que se quiere cerrar
  const scrolls = useRef(new Map());
  // Maximizar (se recuerda: los programas siguientes tambien se abren maximizados hasta Restaurar)
  const [maximizar, setMaximizar] = useState(leerMax);
  const maximizado = maximizar && !!activa;
  const cambiarMax = (v) => { setMaximizar(v); try { localStorage.setItem(CLAVE_MAX, v ? '1' : '0'); } catch { /* sin almacenamiento */ } };
  // ancho disponible para las pestañas (2 filas como maximo)
  const barraRef = useRef(null);
  const [anchoBarra, setAnchoBarra] = useState(1000);
  useEffect(() => {
    const el = barraRef.current; if (!el) return undefined;
    const ro = new ResizeObserver(([e]) => setAnchoBarra(Math.round(e.contentRect.width)));
    ro.observe(el); return () => ro.disconnect();
  });
  // solo se montan los programas que se abrieron en esta sesion (las pestañas recordadas al recargar se cargan al tocarlas)
  const [montadas, setMontadas] = useState(() => new Set(activa ? [activa] : []));
  useEffect(() => { if (activa && !montadas.has(activa)) setMontadas((m) => new Set([...m, activa])); }, [activa]); // eslint-disable-line react-hooks/exhaustive-deps
  const previa = useRef(activa);

  // la pestaña del programa visible se crea o se actualiza (su ultima direccion)
  useEffect(() => {
    if (!activa) return;
    const url = loc.pathname + loc.search;
    setPestanas((ps) => {
      const i = ps.findIndex((p) => p.ruta === activa);
      if (i >= 0) { if (ps[i].url === url) return ps; const n = [...ps]; n[i] = { ...n[i], url }; return n; }
      let n = [...ps, { ruta: activa, id: programa.IdPrograma, url }];
      while (n.length > MAX_PESTANAS) { const j = n.findIndex((p) => p.ruta !== activa && !enEdicion(p.url)); n = n.filter((_, k) => k !== (j >= 0 ? j : 0)); }
      return n;
    });
  }, [activa, loc.pathname, loc.search]); // eslint-disable-line react-hooks/exhaustive-deps
  useEffect(() => { guardar(pestanas); }, [pestanas]);
  // cada pestaña recuerda hasta donde se habia bajado en la pagina
  useLayoutEffect(() => {
    const antes = previa.current;
    if (antes === activa) return;
    if (antes) scrolls.current.set(antes, window.scrollY);
    previa.current = activa;
    window.scrollTo(0, activa ? scrolls.current.get(activa) || 0 : 0);
  }, [activa]);

  // ultimo programa visible (para "Programas abiertos"); se recuerda al recargar
  const ultima = useRef((() => { try { return sessionStorage.getItem(`${CLAVE_GUARDADO}.ultima`); } catch { return null; } })());
  if (activa && ultima.current !== activa) { ultima.current = activa; try { sessionStorage.setItem(`${CLAVE_GUARDADO}.ultima`, activa); } catch { /* sin almacenamiento */ } }
  const urlDe = (ruta) => pestanas.find((p) => p.ruta === ruta)?.url || ruta;
  const contexto = useMemo(() => ({ urlDe }), [pestanas]); // eslint-disable-line react-hooks/exhaustive-deps
  const quitar = (ruta) => {
    const i = pestanas.findIndex((p) => p.ruta === ruta);
    const resto = pestanas.filter((p) => p.ruta !== ruta);
    setPestanas(resto);
    scrolls.current.delete(ruta);
    setMontadas((m) => { const n = new Set(m); n.delete(ruta); return n; });
    if (ruta === activa) { const sig = resto[Math.min(i, resto.length - 1)]; navegar(sig ? sig.url : '/'); }
  };
  const cerrarPestana = (p) => (enEdicion(p.url) ? setCerrando(p) : quitar(p.ruta));
  const cerrarTodas = () => {
    const editando = pestanas.filter((p) => enEdicion(p.url));
    if (editando.length) { setCerrando({ todas: true, n: editando.length }); return; }
    setPestanas([]); setMontadas(new Set()); scrolls.current.clear(); if (activa) navegar('/');
  };
  // pestañas a dibujar (la activa aunque el efecto aun no la haya agregado)
  const lista = activa && !pestanas.some((p) => p.ruta === activa) ? [...pestanas, { ruta: activa, id: programa.IdPrograma, url: loc.pathname + loc.search }] : pestanas;
  // 1 fila si entran; si no, 2 filas repartidas por igual y desplazamiento a la derecha
  const capacidad = Math.max(1, Math.floor((anchoBarra + ESPACIO_PESTANA) / (ANCHO_PESTANA + ESPACIO_PESTANA)));
  const unaFila = lista.length <= capacidad;
  const columnas = unaFila ? lista.length : Math.ceil(lista.length / 2);
  // la pestaña activa siempre a la vista dentro del scroll horizontal (sin mover la pagina)
  useEffect(() => {
    const l = barraRef.current; const a = l?.querySelector('.pestana.activa');
    if (!l || !a) return;
    const izq = a.getBoundingClientRect().left - l.getBoundingClientRect().left + l.scrollLeft; const der = izq + a.offsetWidth;
    if (izq < l.scrollLeft) l.scrollLeft = izq - 8;
    else if (der > l.scrollLeft + l.clientWidth) l.scrollLeft = der - l.clientWidth + 8;
  }, [activa, columnas, anchoBarra]);
  const nombreDe = (p) => {
    for (const m of sesion.modulos) { const x = m.programas.find((q) => q.IdPrograma === p.id); if (x) return titulo(x.DescripcionPrograma); }
    return p.ruta;
  };
  const [abierto, setAbierto] = useState(() => modulo?.cnnum ?? null); // modulo desplegado (uno solo)
  const [menuMovil, setMenuMovil] = useState(false);
  const [menuUsuario, setMenuUsuario] = useState(false);
  const [verEquipo, setVerEquipo] = useState(false); // Nombre de este equipo (auditoria)
  const toast = useToast();
  // Ubicacion para la auditoria: se pide al entrar (app Android o navegador con https / localhost), asi el
  // permiso se acepta una vez y al modificar o eliminar ya esta lista. Se renueva cada 2 minutos de uso.
  useEffect(() => {
    if (!esApp && !ubicacionDisponible()) return undefined;
    ubicacionAuditoria();
    const t = setInterval(() => ubicacionAuditoria(), 120000);
    return () => clearInterval(t);
  }, []);

  useEffect(() => {
    if (modulo) setAbierto(modulo.cnnum);
    setMenuMovil(false);
    // el programa activo siempre visible en el menu lateral
    const t = setTimeout(() => document.querySelector('.menu-item.sub.active')?.scrollIntoView({ block: 'nearest', behavior: 'smooth' }), 260);
    document.title = `${programa ? titulo(programa.DescripcionPrograma) + ' · ' : modulo ? titulo(modulo.cndesc1) + ' · ' : ''}${NOMBRE_SISTEMA}`;
    return () => clearTimeout(t);
  }, [loc.pathname]); // eslint-disable-line react-hooks/exhaustive-deps

  // abrir uno pliega el anterior; volver a tocar el abierto lo pliega
  const alternar = (n) => setAbierto((a) => (a === n ? null : n));

  // Migas: Inicio > Modulo > Programa > Registro
  const resto = loc.pathname.split('/').filter(Boolean).slice(2);
  const sub = resto[0] === 'nuevo' ? 'Nuevo' : resto[0] ? `#${decodeURIComponent(resto[0])}${resto[1] === 'editar' ? ' · Editar' : ''}` : null;
  const rutaProg = programa && PROGRAMAS_WEB[programa.IdPrograma]?.ruta;

  return (
    <PestanasContext.Provider value={contexto}>
    <div className={`app ${menuMovil ? 'menu-abierto' : ''} ${maximizado ? 'maximizado' : ''}`}>
      <aside className="lateral">
        <Link to="/" className="marca"><MarcaSistema /></Link>
        <nav className="menu" aria-label="Menu principal">
          <NavLink to="/" end className="menu-item"><Icono n="home" size={18} /> Inicio</NavLink>
          {sesion.modulos.map((m) => {
            const abiertoM = abierto === m.cnnum;
            return (
              <div key={m.cnnum} className={`menu-grupo ${modulo?.cnnum === m.cnnum ? 'actual' : ''}`}>
                <div className="menu-mod">
                  <NavLink to={`/${slug(m.cndesc1)}`} end className="menu-mod-link" onClick={() => setAbierto(m.cnnum)}><Icono n={iconoModulo(m.cndesc1)} size={18} /> <span>{titulo(m.cndesc1)}</span></NavLink>
                  <button className="menu-mod-flecha" onClick={() => alternar(m.cnnum)} aria-expanded={abiertoM} aria-label={`Mostrar programas de ${titulo(m.cndesc1)}`}>
                    <Icono n="chevronDown" size={16} className={`flecha ${abiertoM ? 'rot' : ''}`} />
                  </button>
                </div>
                <div className={`menu-sub ${abiertoM ? 'abierto' : ''}`} inert={abiertoM ? undefined : ''}>
                  <div>
                    {m.programas.map((p) => {
                      const w = PROGRAMAS_WEB[p.IdPrograma];
                      return w ? (
                        <NavLink key={p.IdPrograma} to={urlDe(w.ruta)} className={() => `menu-item sub ${activa === w.ruta ? 'active' : ''}`}>{imgPrograma(p.IdPrograma) ? <img className="menu-img" src={imgPrograma(p.IdPrograma)} alt="" /> : <Icono n={w.icono} size={16} />} {titulo(p.DescripcionPrograma)}</NavLink>
                      ) : (
                        <span key={p.IdPrograma} className="menu-item sub pendiente" title="Aún no migrado a la web">{titulo(p.DescripcionPrograma)} <em>pronto</em></span>
                      );
                    })}
                  </div>
                </div>
              </div>
            );
          })}
          {/* Programas abiertos (como el escritorio): seleccionado mientras se trabaja en un programa */}
          {(() => {
            const destino = lista.find((p) => p.ruta === ultima.current) || lista[lista.length - 1];
            return destino ? (
              <Link to={destino.ruta === activa ? loc.pathname + loc.search : destino.url} className={`menu-item menu-abiertos ${activa ? 'active' : ''}`} aria-current={activa ? 'page' : undefined}>
                <Icono n="list" size={18} /> <span>Programas abiertos</span><b className="menu-cuenta">{lista.length}</b>
              </Link>
            ) : (
              <span className="menu-item menu-abiertos vacio" title="Todavía no hay programas abiertos"><Icono n="list" size={18} /> <span>Programas abiertos</span><b className="menu-cuenta">0</b></span>
            );
          })()}
        </nav>
        <div className="lateral-pie">{NOMBRE_SISTEMA} · {LEMA_SISTEMA} · v1.0</div>
      </aside>
      <div className="principal">
        <header className="cabecera">
          <button className="btn-icono solo-movil" onClick={() => setMenuMovil(!menuMovil)} aria-label="Menu"><Icono n="menu" /></button>
          <nav className="miga" aria-label="Ubicacion">
            <Link to="/"><Icono n="home" size={15} /></Link>
            {modulo && (<><Icono n="next" size={13} className="miga-sep" /><Link to={`/${slug(modulo.cndesc1)}`}>{titulo(modulo.cndesc1)}</Link></>)}
            {programa && (<><Icono n="next" size={13} className="miga-sep" />{sub ? <Link to={rutaProg}>{titulo(programa.DescripcionPrograma)}</Link> : <b>{titulo(programa.DescripcionPrograma)}</b>}</>)}
            {sub && (<><Icono n="next" size={13} className="miga-sep" /><b>{sub}</b></>)}
            {!modulo && <><Icono n="next" size={13} className="miga-sep" /><b>Inicio</b></>}
          </nav>
          <Reloj />
          <TipoCambio />
          {sesion.conexion && (
            <div className={`conexion-bd ${sesion.conexion.mock ? 'mock' : ''}`}
              title={`Servidor: ${sesion.conexion.servidor}\nBase de datos: ${sesion.conexion.bd}${sesion.conexion.configurado && sesion.conexion.configurado.toUpperCase() !== sesion.conexion.servidor.toUpperCase() ? `\nConfigurado en dbConnect: ${sesion.conexion.configurado}` : ''}`}>
              <i className="conexion-punto" />
              <span className="conexion-txt"><b>{sesion.conexion.servidor}</b><span>{sesion.conexion.bd}</span></span>
            </div>
          )}
          <div className="usuario">
            <button className="usuario-btn" onClick={() => setMenuUsuario(!menuUsuario)} aria-expanded={menuUsuario}>
              <span className="usuario-av">{(sesion.gs_user || '?').slice(0, 1)}</span>
              <span className="usuario-txt"><b>{titulo(sesion.gs_user)}</b><span>{sesion.gs_login} · Suc. {sesion.nombreSucursal || sesion.gi_userSuc}</span></span>
              <Icono n="chevronDown" size={15} />
            </button>
            {menuUsuario && (
              <>
                <div className="capa-cerrar" onClick={() => setMenuUsuario(false)} />
                <div className="menu-flotante">
                  <div className="menu-flotante-cab"><b>{titulo(sesion.gs_user)}</b><span>Usuario {sesion.gs_login}</span>
                    {sesion.conexion && <span className="menu-bd">{sesion.conexion.servidor} · {sesion.conexion.bd}</span>}</div>
                  <button className="menu-neutro" onClick={() => { setMenuUsuario(false); setVerEquipo(true); }}>
                    <Icono n={esApp ? 'phone' : 'monitor'} size={16} />
                    <span className="menu-equipo">Nombre de este equipo<small>{nombreEquipo() || `Sin nombre · #${idEquipo()}`}</small></span>
                  </button>
                  <button onClick={salir}><Icono n="logout" size={16} /> Cerrar sesión</button>
                </div>
              </>
            )}
            {verEquipo && <NombreEquipo toast={toast} onCerrar={() => setVerEquipo(false)} />}
          </div>
        </header>
        {activa && lista.length > 0 && ( /* en Inicio o en el menu de un modulo no se muestran (como el escritorio) */
          <div className="pestanas" role="tablist" aria-label="Programas abiertos">
            <div className={`pestanas-lista ${unaFila ? 'una-fila' : 'dos-filas'}`} ref={barraRef}
              style={{ gridTemplateColumns: unaFila ? `repeat(${columnas}, minmax(150px, 240px))` : `repeat(${columnas}, ${ANCHO_PESTANA}px)` }}>
              {lista.map((p) => {
                const w = PROGRAMAS_WEB[p.id] || {};
                const act = p.ruta === activa;
                return (
                  <div key={p.ruta} className={`pestana ${act ? 'activa' : ''}`} role="presentation" onAuxClick={(e) => { if (e.button === 1) { e.preventDefault(); cerrarPestana(p); } }}>
                    <Link role="tab" aria-selected={act} to={act ? loc.pathname + loc.search : p.url} className="pestana-link" title={nombreDe(p)}>
                      {imgPrograma(p.id) ? <img src={imgPrograma(p.id)} alt="" /> : <Icono n={w.icono || 'list'} size={15} />}
                      <span>{nombreDe(p)}</span>
                      {enEdicion(act ? loc.pathname : p.url) && <i className="pestana-editando" title="Registro a medio editar" />}
                    </Link>
                    <button type="button" className="pestana-x" onClick={() => cerrarPestana(p)} aria-label={`Cerrar ${nombreDe(p)}`} title="Cerrar"><Icono n="x" size={14} /></button>
                  </div>
                );
              })}
            </div>
            <div className="pestanas-acc">
              {lista.length > 1 && <button type="button" className="pestanas-todas" onClick={cerrarTodas} title="Cerrar todos los programas">Cerrar todas</button>}
              <button type="button" className="pestanas-max" onClick={() => cambiarMax(!maximizado)} aria-pressed={maximizado}
                title={maximizado ? 'Restaurar: volver a mostrar el menú' : 'Maximizar: el programa ocupa toda la pantalla'}>
                {maximizado
                  ? <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true"><path d="M4 14h6v6M20 10h-6V4M14 10l7-7M3 21l7-7" /></svg>
                  : <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true"><path d="M15 3h6v6M9 21H3v-6M21 3l-7 7M3 21l7-7" /></svg>}
                <span>{maximizado ? 'Restaurar' : 'Maximizar'}</span>
              </button>
            </div>
          </div>
        )}
        <main className="contenido">
          {!activa && <div className="vista-outlet" key={loc.pathname}><Outlet /></div>}
          {/* cada programa abierto sigue montado (conserva lo que se estaba haciendo); solo se ve el activo */}
          {lista.filter((p) => p.ruta === activa || montadas.has(p.ruta)).map((p) => (
            <div key={p.ruta} className="pestana-vista" hidden={p.ruta !== activa}>
              {/* al volver a la pestaña o pulsar "Actualizar" cada pantalla recarga sus datos de la BD (useAlActivar) */}
              <ProveedorVista activa={p.ruta === activa}>
                <Routes location={p.ruta === activa ? loc : { pathname: p.url.split('?')[0], search: p.url.includes('?') ? `?${p.url.split('?')[1]}` : '', hash: '', state: null, key: p.ruta }}>
                  {rutas}
                </Routes>
              </ProveedorVista>
            </div>
          ))}
        </main>
      </div>
      <div className="velo" onClick={() => setMenuMovil(false)} />
      {cerrando && (
        <div className="modal-fondo" onMouseDown={() => setCerrando(null)}>
          <div className="modal modal-sm" role="alertdialog" aria-modal="true" aria-labelledby="cerrar-tit" onMouseDown={(e) => e.stopPropagation()} onKeyDown={(e) => e.key === 'Escape' && setCerrando(null)}>
            <div className="modal-icono peligro"><Icono n="alert" size={26} /></div>
            <h3 id="cerrar-tit">{cerrando.todas ? '¿Cerrar todos los programas?' : `¿Cerrar ${nombreDe(cerrando)}?`}</h3>
            <p>{cerrando.todas ? `Hay ${cerrando.n} registro${cerrando.n > 1 ? 's' : ''} a medio editar. Se perderán los cambios no grabados.` : 'Tiene un registro a medio editar. Se perderán los cambios no grabados.'}</p>
            <div className="modal-acciones">
              <button className="btn btn-sec" autoFocus onClick={() => setCerrando(null)}>Volver</button>
              <button className="btn btn-peligro" onClick={() => {
                const c = cerrando; setCerrando(null);
                if (c.todas) { setPestanas([]); setMontadas(new Set()); scrolls.current.clear(); if (activa) navegar('/'); } else quitar(c.ruta);
              }}>Cerrar sin grabar</button>
            </div>
          </div>
        </div>
      )}
    </div>
    </PestanasContext.Provider>
  );
}
