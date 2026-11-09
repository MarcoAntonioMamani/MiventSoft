// =====================================================================
//  Tec_RegistrarPersonalAPlanilla.vb (btnRegistrarPlanilla) -> Registrar personal a planilla  (SP: MAM_Planilla @tipo 5 / 1)
//  Año (lib. 13, se puede crear) y Mes (lib. 12): lista los contratos sin planilla del periodo y los registra todos
//  con sus conceptos fijos (Monto = Sueldo x %). El servidor arma la planilla de cada trabajador.
// =====================================================================
import { useEffect, useState } from 'react';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti, ComboLibreria } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { usePermisos } from '../../global/Sesion';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';

const URL = '/api/planilla/planilla';
const MESES = ['ENERO', 'FEBRERO', 'MARZO', 'ABRIL', 'MAYO', 'JUNIO', 'JULIO', 'AGOSTO', 'SEPTIEMBRE', 'OCTUBRE', 'NOVIEMBRE', 'DICIEMBRE'];

// Año = ultimo de la libreria; Mes = el mes actual (ObtenerFilaMes de VB)
export function periodoInicial(anios, meses) {
  const actual = MESES[new Date().getMonth()];
  return { anio: anios[anios.length - 1]?.cnnum ?? '', mes: (meses.find((m) => actual.includes(String(m.cndesc1).toUpperCase())) || meses[0])?.cnnum ?? '' };
}

export default function Tec_RegistrarPlanilla() {
  const toast = useToast();
  const puede = usePermisos();
  const [c, setC] = useState({ anios: [], meses: [] });
  const [p, setP] = useState({ anio: '', mes: '' });
  const [filas, setFilas] = useState([]);
  const [cargando, setCargando] = useState(false);
  const [grabando, setGrabando] = useState(false);
  useEffect(() => { api.get(`${URL}/combos`).then((r) => { setC(r.data); setP(periodoInicial(r.data.anios, r.data.meses)); }).catch((e) => toast(e.message, 'error')); }, [toast]);
  const cargar = () => {
    if (!p.anio || !p.mes) return;
    setCargando(true);
    api.get(`${URL}/registrar/sin-planilla?mes=${p.mes}&anio=${p.anio}`).then((r) => setFilas(r.data)).catch((e) => toast(e.message, 'error')).finally(() => setCargando(false));
  };
  useEffect(cargar, [p.anio, p.mes]); // eslint-disable-line react-hooks/exhaustive-deps
  const nuevoAnio = async (descripcion) => {
    const r = await api.post(`${URL}/registrar/libreria/13`, { descripcion });
    setC((o) => ({ ...o, anios: r.data }));
    toast(r.mensaje, 'ok');
    return r;
  };
  const registrar = async () => {
    setGrabando(true);
    try { const r = await api.post(`${URL}/registrar`, { mes: p.mes, anio: p.anio }); toast(r.mensaje, 'ok'); cargar(); } catch (e) { toast(e.message, 'error', 7000); } finally { setGrabando(false); }
  };
  const nombreMes = c.meses.find((m) => String(m.cnnum) === String(p.mes))?.cndesc1;
  const nombreAnio = c.anios.find((m) => String(m.cnnum) === String(p.anio))?.cndesc1;

  return (
    <section className="modelo entrada">
      <header className="modelo-cab">
        <div className="modelo-titulo"><span className="modelo-icono"><Icono n="users" size={22} /></span><div><h1>Registrar personal a planilla</h1><p>Contratos sin planilla en el periodo</p></div></div>
      </header>
      <div className="modelo-cuerpo">
        <div className="filtros-lista filtros-reporte">
          <div className="filtro filtro-ancho"><span>Año</span><ComboLibreria dt={c.anios} valor={p.anio} onChange={(x) => setP((o) => ({ ...o, anio: x }))} titulo="Gestión (Año)" onNuevo={puede.insertar ? nuevoAnio : undefined} /></div>
          <div className="filtro filtro-ancho"><span>Mes</span><ComboMulti dt={c.meses} valor={p.mes} onChange={(x) => setP((o) => ({ ...o, mes: x }))} id="cnnum" nameId="Cód." descripcionId="cndesc1" descripcion="Mes" /></div>
          {puede.insertar && filas.length > 0 && (
            <button className="btn btn-pri filtro-boton" onClick={registrar} disabled={grabando}>{grabando ? <span className="spinner" /> : <Icono n="check" size={16} />} Registrar planilla ({filas.length})</button>
          )}
        </div>
        <TablaReporte titulo="PERSONAL SIN PLANILLA" subtitulo={[nombreMes && tit(nombreMes), nombreAnio].filter(Boolean).join(' ')} cargando={cargando} claveFila="ContratoId" vacio="Todo el personal con contrato ya está en la planilla de este periodo"
          columnas={[
            { campo: 'Personal', titulo: 'Trabajador', ancho: 200, render: (x) => <b>{tit(x.Personal ?? '')}</b> },
            { campo: 'Cargo', titulo: 'Cargo' },
            { campo: 'InicioContrato', titulo: 'Inicio contrato', tipo: 'fecha' },
            { campo: 'FinContrato', titulo: 'Fin contrato', tipo: 'fecha' },
            { campo: 'SueldoBase', titulo: 'Sueldo base', tipo: 'bs', total: true },
            { campo: 'SueldoBruto', titulo: 'Sueldo bruto', tipo: 'bs', total: true },
          ]} filas={filas} />
      </div>
    </section>
  );
}
