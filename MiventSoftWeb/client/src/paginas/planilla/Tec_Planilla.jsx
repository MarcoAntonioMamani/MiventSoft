// =====================================================================
//  Tec_Listarplanilla.vb (btnPlanilla) -> Planilla de sueldos  (SP: MAM_Planilla @tipo 3 / 9 / 8)
//  Por periodo (Año lib. 13 / Mes lib. 12) o todas. Por trabajador:
//   C.Fijos    -> Tec_VisualizarConceptoFijosPlanilla (solo ver)
//   C.Variable -> Tec_VisualizarConceptosVariablesPlanilla (agregar / quitar, @tipo 7)
//   Reporte    -> boleta de sueldo (@tipo 8) · Reporte planilla (@tipo 9)
// =====================================================================
import { useCallback, useEffect, useState } from 'react';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { usePermisos } from '../../global/Sesion';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { columnasDe } from '../ventas/Reportes';
import { periodoInicial } from './Tec_RegistrarPlanilla';

const URL = '/api/planilla/planilla';
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

export default function Tec_Planilla() {
  const toast = useToast();
  const puede = usePermisos();
  const [c, setC] = useState({ anios: [], meses: [] });
  const [p, setP] = useState({ anio: '', mes: '', todos: false });
  const [filas, setFilas] = useState([]);
  const [cargando, setCargando] = useState(false);
  const [dialogo, setDialogo] = useState(null); // { tipo: 'fijos'|'variables'|'boleta'|'reporte', fila }
  useEffect(() => { api.get(`${URL}/combos`).then((r) => { setC(r.data); setP((o) => ({ ...o, ...periodoInicial(r.data.anios, r.data.meses) })); }).catch((e) => toast(e.message, 'error')); }, [toast]);
  const q = `mes=${p.mes}&anio=${p.anio}${p.todos ? '&todos=1' : ''}`;
  const cargar = useCallback(() => {
    if (!p.todos && (!p.anio || !p.mes)) return;
    setCargando(true);
    api.get(`${URL}/listar?${q}`).then((r) => setFilas(r.data)).catch((e) => toast(e.message, 'error')).finally(() => setCargando(false));
  }, [q, p.todos, p.anio, p.mes, toast]);
  useEffect(cargar, [cargar]);
  const btn = (f, tipo, icono, texto) => <button type="button" className="btn-icono btn-planilla" onClick={(e) => { e.stopPropagation(); setDialogo({ tipo, fila: f }); }} title={texto} aria-label={`${texto} de ${f.Trabajador}`}><Icono n={icono} size={17} /></button>;

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo"><span className="modelo-icono"><Icono n="wallet" size={22} /></span><div><h1>Planilla de sueldos</h1><p>Listar Planilla</p></div></div>
        <div className="barra"><button className="btn-icono" title="Actualizar" aria-label="Actualizar" onClick={cargar}><Icono n="refresh" size={18} /></button></div>
      </header>
      <div className="modelo-cuerpo">
        <div className="filtros-lista filtros-reporte">
          <div className="filtro filtro-ancho"><span>Año</span><ComboMulti dt={c.anios} valor={p.anio} onChange={(x) => setP((o) => ({ ...o, anio: x }))} id="cnnum" nameId="Cód." descripcionId="cndesc1" descripcion="Año" deshabilitado={p.todos} /></div>
          <div className="filtro filtro-ancho"><span>Mes</span><ComboMulti dt={c.meses} valor={p.mes} onChange={(x) => setP((o) => ({ ...o, mes: x }))} id="cnnum" nameId="Cód." descripcionId="cndesc1" descripcion="Mes" deshabilitado={p.todos} /></div>
          <div className="filtro"><span>Periodo</span><div className="chips-estado">
            <button type="button" className={`chip-filtro ${!p.todos ? 'activo' : ''}`} onClick={() => setP((o) => ({ ...o, todos: false }))}>Del mes</button>
            <button type="button" className={`chip-filtro ${p.todos ? 'activo' : ''}`} onClick={() => setP((o) => ({ ...o, todos: true }))}>Todas</button>
          </div></div>
          <button className="btn btn-pri filtro-boton" disabled={!filas.length} onClick={() => setDialogo({ tipo: 'reporte' })}><Icono n="list" size={16} /> Reporte planilla</button>
        </div>
        <TablaReporte titulo="PLANILLA DE SUELDOS" cargando={cargando} claveFila="PlanillaId" vacio="No hay planillas en este periodo" filas={filas}
          columnas={[
            { campo: 'fila', titulo: 'Nº', render: (x) => x.fila },
            { campo: 'Anio', titulo: 'Año' }, { campo: 'Mes', titulo: 'Mes', render: (x) => tit(x.Mes ?? '') },
            { campo: 'Trabajador', titulo: 'Trabajador', ancho: 190, render: (x) => <b>{tit(x.Trabajador ?? '')}</b> },
            { campo: 'Cargo', titulo: 'Cargo' },
            { campo: 'Sueldo', titulo: 'Sueldo', tipo: 'bs', total: true },
            { campo: 'SueldoBruto', titulo: 'Sueldo bruto', tipo: 'bs', total: true },
            { campo: 'SueldoNeto', titulo: 'Sueldo neto', tipo: 'bs', total: true, render: (x) => <b>{n2(x.SueldoNeto)}</b> },
          ]}
          accion={(f) => <span className="acciones-planilla">{btn(f, 'fijos', 'list', 'Conceptos fijos')}{btn(f, 'variables', 'calc', 'Conceptos variables')}{btn(f, 'boleta', 'receipt', 'Boleta de sueldo')}</span>} />
      </div>
      {dialogo?.tipo === 'fijos' && <ConceptosFijos fila={dialogo.fila} onCerrar={() => setDialogo(null)} />}
      {dialogo?.tipo === 'variables' && <ConceptosVariables fila={dialogo.fila} puede={puede.modificar} onCerrar={(cambio) => { setDialogo(null); if (cambio) cargar(); }} />}
      {dialogo?.tipo === 'boleta' && <Documento titulo={`Boleta de sueldo · ${tit(dialogo.fila.Trabajador ?? '')}`} url={`${URL}/listar/${dialogo.fila.PlanillaId}/boleta`} onCerrar={() => setDialogo(null)} />}
      {dialogo?.tipo === 'reporte' && <Documento titulo="Reporte de planilla" url={`${URL}/listar/reporte?${q}`} onCerrar={() => setDialogo(null)} />}
    </section>
  );
}

function Modal({ titulo, children, onCerrar, ancho = 'modal-lg' }) {
  return (
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className={`modal ${ancho}`} role="dialog" aria-modal="true" aria-label={titulo} onMouseDown={(e) => e.stopPropagation()} onKeyDown={(e) => e.key === 'Escape' && onCerrar()}>
        <div className="modal-cab"><h3>{titulo}</h3><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        {children}
      </div>
    </div>
  );
}

// Tec_VisualizarConceptoFijosPlanilla
function ConceptosFijos({ fila, onCerrar }) {
  const toast = useToast();
  const [filas, setFilas] = useState(null);
  useEffect(() => { api.get(`${URL}/listar/${fila.PlanillaId}/conceptos?tipo=1`).then((r) => setFilas(r.data)).catch((e) => toast(e.message, 'error')); }, [fila.PlanillaId, toast]);
  return (
    <Modal titulo={`Conceptos fijos · ${tit(fila.Trabajador ?? '')}`} onCerrar={onCerrar}>
      <TablaReporte titulo="CONCEPTOS FIJOS" cargando={!filas} filas={filas || []} vacio="Sin conceptos fijos"
        columnas={[{ campo: 'NombreConcepto', titulo: 'Concepto fijo', render: (x) => <b>{tit(x.NombreConcepto)}</b> }, { campo: 'Porcentaje', titulo: 'Porcentaje %', tipo: 'num', total: true }, { campo: 'Monto', titulo: 'Monto', tipo: 'bs', total: true }]} />
    </Modal>
  );
}

// Tec_VisualizarConceptosVariablesPlanilla
function ConceptosVariables({ fila, puede, onCerrar }) {
  const toast = useToast();
  const [filas, setFilas] = useState(null);
  const [tipos, setTipos] = useState([]);
  const [f, setF] = useState({ ConceptoId: '', Monto: '', Descripcion: '' });
  const [neto, setNeto] = useState(Number(fila.SueldoNeto) || 0);
  const [cambio, setCambio] = useState(false);
  const [ocupado, setOcupado] = useState(false);
  const cargar = useCallback(() => api.get(`${URL}/listar/${fila.PlanillaId}/conceptos?tipo=0`).then((r) => setFilas(r.data)).catch((e) => toast(e.message, 'error')), [fila.PlanillaId, toast]);
  useEffect(() => { cargar(); api.get(`${URL}/listar/variables`).then((r) => setTipos(r.data)).catch((e) => toast(e.message, 'error')); }, [cargar, toast]);
  const op = tipos.find((t) => String(t.Id) === String(f.ConceptoId))?.Operacion ?? 0;
  const agregar = async () => {
    const monto = Number(f.Monto);
    if (!f.ConceptoId) { toast('Seleccione un Concepto Variable', 'error'); return; }
    if (!(monto > 0)) { toast('El Monto debe ser Mayor a 0', 'error'); return; }
    if (op === -1 && monto > neto) { toast(`El monto no puede ser mayor al Sueldo Neto (${n2(neto)})`, 'error'); return; }
    setOcupado(true);
    try {
      const r = await api.post(`${URL}/listar/${fila.PlanillaId}/variables`, f);
      toast(r.mensaje, 'ok'); setNeto((x) => x + monto * op); setF({ ConceptoId: '', Monto: '', Descripcion: '' }); setCambio(true); cargar();
    } catch (e) { toast(e.message, 'error', 7000); } finally { setOcupado(false); }
  };
  const quitar = async (x) => {
    if (!window.confirm(`¿Esta Seguro de Eliminar el concepto Variable ${x.NombreConcepto} Con Monto = ${n2(x.Monto)} ?`)) return;
    setOcupado(true);
    try { const r = await api.del(`${URL}/listar/${fila.PlanillaId}/variables/${x.Id}`); toast(r.mensaje, 'ok'); setNeto((n) => n - Number(x.Monto)); setCambio(true); cargar(); } catch (e) { toast(e.message, 'error'); } finally { setOcupado(false); }
  };
  return (
    <Modal titulo={`Conceptos variables · ${tit(fila.Trabajador ?? '')}`} onCerrar={() => onCerrar(cambio)}>
      <p className="planilla-neto"><span>Sueldo neto</span><b>Bs {n2(neto)}</b></p>
      {puede && (
        <div className="filtros-lista filtros-reporte">
          <div className="filtro filtro-ancho"><span>Concepto variable</span><ComboMulti dt={tipos} valor={f.ConceptoId} onChange={(x) => setF((o) => ({ ...o, ConceptoId: x }))} id="Id" nameId="Cód." descripcionId="NombreConcepto" descripcion="Concepto Variable" /></div>
          <label className="filtro"><span>Monto {op === -1 ? '(descuenta)' : op === 1 ? '(suma)' : ''}</span><input className="control num" type="number" min="0" step="0.01" value={f.Monto} onChange={(e) => setF((o) => ({ ...o, Monto: e.target.value }))} /></label>
          <label className="filtro filtro-ancho"><span>Descripción</span><input className="control" maxLength={200} value={f.Descripcion} onChange={(e) => setF((o) => ({ ...o, Descripcion: e.target.value }))} /></label>
          <button className="btn btn-pri filtro-boton" onClick={agregar} disabled={ocupado}><Icono n="plus" size={16} /> Registrar</button>
        </div>
      )}
      <TablaReporte titulo="CONCEPTOS VARIABLES" cargando={!filas} filas={filas || []} vacio="Sin conceptos variables"
        columnas={[{ campo: 'NombreConcepto', titulo: 'Concepto variable', render: (x) => <b>{tit(x.NombreConcepto)}</b> }, { campo: 'Descripcion', titulo: 'Descripción' },
          { campo: 'Monto', titulo: 'Monto', tipo: 'bs', total: true, clase: (x) => (x.Monto < 0 ? 'neg' : 'pos') }]}
        accion={puede ? (x) => <button className="btn-icono peligro" disabled={ocupado} onClick={() => quitar(x)} aria-label={`Quitar ${x.NombreConcepto}`} title="Quitar"><Icono n="trash" size={16} /></button> : undefined} />
    </Modal>
  );
}

// Boleta (Reporte_BoletaSueldo) / Reporte_Planilla: columnas tal como las devuelve el SP
function Documento({ titulo, url, onCerrar }) {
  const toast = useToast();
  const [filas, setFilas] = useState(null);
  useEffect(() => { api.get(url).then((r) => { if (!r.data.length) toast('No Existen Datos Para Generar el Reporte'.toUpperCase(), 'error'); setFilas(r.data); }).catch((e) => { toast(e.message, 'error'); onCerrar(); }); }, [url]); // eslint-disable-line react-hooks/exhaustive-deps
  return (
    <Modal titulo={titulo} onCerrar={onCerrar}>
      <TablaReporte titulo={titulo.toUpperCase()} cargando={!filas} filas={filas || []} columnas={columnasDe(filas || [])} claveFila="__no" vacio="Sin datos" />
    </Modal>
  );
}
