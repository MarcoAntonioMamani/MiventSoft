// =====================================================================
//  KAILY LA PAZ: reportes de Administrar Caja
//   caja-general    Tec_CajaGeneral: historial de la caja general con saldo inicial y saldo acumulado
//   caja-detallado  Tec_ReporteCajaDetallado: movimientos de una caja (o Todas) en un rango
// =====================================================================
import { useEffect, useState } from 'react';
import TablaReporte from '../../componentes/TablaReporte';
import { ComboMulti } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import { columnasDe } from '../ventas/Reportes';

const URL = '/api/caja/reportes';
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

const CONFIG = {
  'caja-general': { titulo: 'Caja general', sub: 'Historial De Movimientos Caja General', icono: 'wallet' },
  'caja-detallado': { titulo: 'Reporte caja detallado', sub: 'Movimientos De Caja Detallado', icono: 'history' },
};
// Columnas de grMovimientos (Tec_CajaGeneral); el saldo negativo se resalta en rojo como el escritorio
const COL_GENERAL = [
  { campo: 'Id', titulo: 'ID', tipo: 'entero' }, { campo: 'Fecha', titulo: 'Fecha Transacción', tipo: 'fecha' },
  { campo: 'TipoMovimiento', titulo: 'Tipo Movimiento' }, { campo: 'Descripcion', titulo: 'Descripción Movimiento' },
  { campo: 'Ingreso', titulo: 'Ingresos', tipo: 'bs', total: true }, { campo: 'Egreso', titulo: 'Egresos', tipo: 'bs', total: true },
  { campo: 'Saldo', titulo: 'Saldo', tipo: 'bs', clase: (f) => (Number(f.Saldo) < 0 ? 'saldo-negativo' : '') },
];

export default function ReporteCaja({ clave }) {
  const cfg = CONFIG[clave];
  const toast = useToast();
  const [f, setF] = useState({ desde: hoy(), hasta: hoy(), caja: -1 });
  const [cajas, setCajas] = useState([]);
  const [datos, setDatos] = useState(null);
  const [cargando, setCargando] = useState(false);

  useEffect(() => { if (clave === 'caja-detallado') api.get(`${URL}/caja-detallado/cajas`).then((r) => setCajas(r.data)).catch((e) => toast(e.message, 'error')); }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps

  const generar = async () => {
    if (f.desde > f.hasta) { toast('La Fecha Desde Debe Ser Menor Que la Fecha Hasta', 'error'); return; }
    setCargando(true);
    try {
      const q = new URLSearchParams({ desde: f.desde, hasta: f.hasta, ...(clave === 'caja-detallado' ? { caja: f.caja } : {}) });
      const r = (await api.get(`${URL}/${clave}?${q}`)).data;
      setDatos(r);
      if (clave === 'caja-general' && r.sinMovimientos) toast('No Existe Movimiento Para Los Filtros Establecidos'.toUpperCase(), 'aviso');
      if (clave === 'caja-detallado' && !r.filas.length) toast('No Existen Datos Para Mostrar con Los Filtros Seleccionados'.toUpperCase(), 'aviso');
    } catch (e) { toast(e.message, 'error'); } finally { setCargando(false); }
  };
  useEffect(() => { if (clave === 'caja-general') generar(); }, [clave]); // eslint-disable-line react-hooks/exhaustive-deps

  const filas = (datos?.filas || []).map((x, i) => ({ __no: i, ...x }));
  const columnas = clave === 'caja-general' ? COL_GENERAL : columnasDe(datos?.filas || []);

  return (
    <div className="entrada">
      <div className="cab-pagina"><h1><Icono n={cfg.icono} size={22} /> {cfg.titulo}</h1><p>{cfg.sub}</p></div>
      <div className="tarjeta">
        <div className="filtros-lista">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={f.desde} max={f.hasta} onChange={(e) => setF((x) => ({ ...x, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={f.hasta} min={f.desde} onChange={(e) => setF((x) => ({ ...x, hasta: e.target.value }))} /></label>
          {clave === 'caja-detallado' && (
            <div className="filtro"><span>Caja</span>
              <ComboMulti dt={cajas} valor={f.caja} onChange={(x) => setF((y) => ({ ...y, caja: x }))} id="Id" nameId="Código" descripcionId="NombreCaja" descripcion="Caja" /></div>
          )}
          <button className="btn btn-pri" onClick={generar} disabled={cargando}><Icono n="search" size={16} /> Generar</button>
          {clave === 'caja-general' && datos?.saldoCajaGeneral !== null && datos?.saldoCajaGeneral !== undefined && (
            <span className="ayuda-tip">Saldo caja general: <b>Bs {bs(datos.saldoCajaGeneral)}</b></span>
          )}
        </div>
        <TablaReporte titulo={cfg.titulo.toUpperCase()} subtitulo={`Del ${f.desde.split('-').reverse().join('/')} al ${f.hasta.split('-').reverse().join('/')}`}
          columnas={columnas} filas={filas} cargando={cargando} claveFila="__no" vacio={datos ? 'Sin movimientos' : 'Elija el rango y pulse Generar'} />
      </div>
    </div>
  );
}
