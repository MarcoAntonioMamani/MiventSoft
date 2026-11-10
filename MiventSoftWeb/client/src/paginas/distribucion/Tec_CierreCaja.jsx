// =====================================================================
//  Tec_CierreCaja.vb -> Cierre de caja del distribuidor  (SP: MAM_CierreCaja)
//  1) Elegir una conciliación cerrada  2) ventas + cobranzas  3) gastos  4) efectivo recibido
//  Efectivo calculado = Contado + Cobranzas − Gastos ; Diferencia = Recibido − Calculado (lo recalcula el servidor)
// =====================================================================
import { useEffect, useState } from 'react';
import { createPortal } from 'react-dom';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, fechaCorta } from '../../componentes/Controles';
import { EfectoAyuda } from '../../componentes/Efecto';
import TablaReporte from '../../componentes/TablaReporte';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import Icono from '../../componentes/Icono';
import { columnasDe } from '../ventas/Reportes';
import { api, urlImagen } from '../../api/http';
import { esc, imprimirHtml } from '../../global/imprimir';

const URL = '/api/distribucion/cierre-caja';
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const bs = (n) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const r2 = (x) => Math.round(x * 100) / 100;
// en el cierre 'Credito' es un monto (no el numero de credito)
const colsVentas = (filas) => columnasDe(filas).map((c) => (c.campo === 'Credito' ? { ...c, tipo: 'bs', total: true, render: undefined } : c));
const VACIO = { Id: '', ConciliacionId: '', PersonalId: 0, Personal: '', FechaCierre: '', VentasContado: 0, VentasCredito: 0, TotalCobros: 0, EfectivoRecibido: '' };
let _k = 0;

// ---------- ReporteCierreCajaChofer.rpt: Reporte de arqueo dinero en efectivo (SP MAM_CierreCaja @tipo 9) ----------
// Cada fila del SP es una venta (cliente / nota / contado / credito) y repite los totales del cierre.
const n2 = (x) => num(x).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const fechaHora = (s) => {
  const [a, mm, d] = String(s || '').slice(0, 10).split('-');
  if (!a) return '';
  const h = String(s).slice(11, 16);
  return `${d}/${mm}/${a}${h ? `&nbsp;&nbsp; ${h}` : ''}`;
};
const CSS_ARQUEO = `
body { padding: 8mm 10mm; }
.arq { box-sizing: border-box; width: 100%; color: #111; font: 11px/1.35 Arial, Helvetica, sans-serif; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
.arq * { box-sizing: border-box; }
.arq .a-cab { display: grid; grid-template-columns: 1fr 1.3fr 1fr; gap: 12px; align-items: start; margin-bottom: 16px; }
.arq .a-logo { border: 1px solid #f0b37e; border-radius: 6px; height: 72px; display: flex; align-items: center; justify-content: center; padding: 4px; }
.arq .a-logo img { max-width: 100%; max-height: 62px; object-fit: contain; }
.arq h1 { margin: 2px 0 0; text-align: center; color: #1c2f8f; font-size: 16px; line-height: 1.35; text-transform: uppercase; }
.arq .a-emp { text-align: right; text-transform: uppercase; font-size: 11px; letter-spacing: .3px; }
.arq .a-datos { border: 1px dotted #1c2f8f; background: #fafbfc; padding: 8px 12px; margin-bottom: 18px; min-height: 60px; }
.arq .a-datos div { display: grid; grid-template-columns: 110px 1fr; gap: 8px; margin-bottom: 6px; }
.arq .a-et { color: #1c2f8f; font-weight: 700; }
.arq .a-sec { display: inline-block; min-width: 45%; background: #6b6b6b; color: #fff; font-weight: 700; padding: 2px 8px; margin: 0 0 18px 16px; }
.arq table { width: 100%; border-collapse: collapse; margin-bottom: 30px; }
.arq thead th { background: #1c2f8f; color: #fff; font-weight: 700; text-align: left; padding: 4px 16px; border: 1px solid #14226b; }
.arq td { padding: 3px 16px; border-bottom: 1px dotted #9aa1a9; font-variant-numeric: tabular-nums; }
.arq .num { text-align: right; }
.arq .vacio { text-align: center; color: #555; }
.arq .a-tot { display: grid; grid-template-columns: 1fr 1fr; gap: 0 24px; margin: 0 16px 90px; break-inside: avoid; }
.arq .a-tot dl { margin: 0; }
.arq .a-tot dl div { display: grid; gap: 10px; padding: 4px 6px; align-items: baseline; }
.arq .a-izq div { grid-template-columns: 190px 1fr; }
.arq .a-izq dt { text-align: right; }
.arq .a-der div { grid-template-columns: 130px 1fr; max-width: 290px; }
.arq .a-tot dt { color: #1c2f8f; font-weight: 700; }
.arq .a-tot dd { margin: 0; font-variant-numeric: tabular-nums; }
.arq .a-tot .a-rec { background: #f1f1f1; }
.arq .a-firmas { display: grid; grid-template-columns: 1fr 1fr; gap: 30px; margin: 0 16px; text-align: center; break-inside: avoid; }
.arq .a-firmas span { display: block; border-top: 1px dotted #555; margin-bottom: 8px; }
.arq .a-firmas b { display: block; color: #1c2f8f; font-size: 13px; }
.arq .a-firmas small { display: block; margin-top: 10px; font-size: 11px; text-transform: uppercase; }
`;
function hojaArqueo({ filas, f, empresa, imagenEmpresa }) {
  const c = filas[0] || {};
  const v = (campo, alterno) => (c[campo] ?? f?.[alterno ?? campo] ?? 0);
  const personal = c.NombrePersonal ?? f?.NombrePersonal ?? '';
  const logo = imagenEmpresa ? urlImagen('empresa', imagenEmpresa) : '';
  const cuerpo = filas.length
    ? filas.map((x) => `<tr><td>${esc(x.NombreCliente)}</td><td class="num">${esc(Number(x.NroNotaVenta || 0).toLocaleString('en-US'))}</td><td class="num">${n2(x.Contado)}</td><td class="num">${n2(x.Credito)}</td></tr>`).join('')
    : '<tr><td colspan="4" class="vacio">Sin ventas</td></tr>';
  return `<article class="arq">
  <header class="a-cab">
    <div class="a-logo">${logo ? `<img src="${esc(logo)}" alt="" onerror="this.remove()">` : ''}</div>
    <h1>Reporte de arqueo dinero<br>en efectivo (Bs)</h1>
    <div class="a-emp">${esc(empresa?.nombre)}<br>${esc(empresa?.direccion)}</div>
  </header>
  <section class="a-datos">
    <div><span class="a-et">Distribuidor:</span><span>${esc(personal)}</span></div>
    <div><span class="a-et">Fecha Arqueo:</span><span>${fechaHora(c.FechaArqueo ?? f?.FechaCierre)}</span></div>
  </section>
  <div class="a-sec">Resumen De Venta Realizadas</div>
  <table>
    <thead><tr><th>Cliente</th><th class="num">Nro De Nota Ventas</th><th class="num">Contado</th><th class="num">Credito</th></tr></thead>
    <tbody>${cuerpo}</tbody>
  </table>
  <section class="a-tot">
    <dl class="a-izq">
      <div><dt>Dinero De Ventas Al Credito:</dt><dd>${n2(v('VentasCreditos'))}</dd></div>
      <div><dt>Dinero De Ventas Al Contado:</dt><dd>${n2(v('VentasContado'))}</dd></div>
      <div><dt>Amortizaciones:</dt><dd>${n2(v('TotalCobros'))}</dd></div>
    </dl>
    <dl class="a-der">
      <div><dt>Total Gastos:</dt><dd>${n2(v('TotalGastos'))}</dd></div>
      <div><dt>Efectivo Calculado:</dt><dd>${n2(v('EfectivoCalculado'))}</dd></div>
      <div class="a-rec"><dt>Efectivo Recibido:</dt><dd>${n2(v('EfectivoRecibido'))}</dd></div>
      <div><dt>Diferencia (+/-):</dt><dd>${n2(v('Diferencia'))}</dd></div>
    </dl>
  </section>
  <section class="a-firmas">
    <div><span></span><b>Entregado Conforme</b><small>${esc(personal)}</small></div>
    <div><span></span><b>Revisado y Recibido Conforme</b></div>
  </section>
</article>`;
}


export default function Tec_CierreCaja() {
  const toast = useToast();
  const [filtro, setFiltro] = useState({ desde: hoy(), hasta: hoy() });
  const [form, setForm] = useState(VACIO);
  const [ventas, setVentas] = useState([]);
  const [cobranzas, setCobranzas] = useState([]);
  const [gastos, setGastos] = useState([]);
  const [concs, setConcs] = useState(null);
  const [nuevoGasto, setNuevoGasto] = useState(false);
  const vig = gastos.filter((g) => !g.eliminado);
  const totalGastos = r2(vig.reduce((s, g) => s + num(g.Monto), 0));
  const calculado = r2(num(form.VentasContado) + num(form.TotalCobros) - totalGastos);
  const diferencia = r2(num(form.EfectivoRecibido) - calculado);

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => (await api.get(`${URL}?${new URLSearchParams(filtro)}`)).data.map((f) => ({ ...f, FechaTxt: fechaCorta(`${f.FechaCierre}T00:00:00Z`) })),
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'Cod. Cierre', 80, '', 'num'),
      new Celda('NroConciliacion', true, 'Conciliación', 90, '', 'num'),
      new Celda('NombrePersonal', true, 'Nombre Personal', 200),
      new Celda('FechaTxt', true, 'Fecha Cierre', 100),
      new Celda('EfectivoCalculado', true, 'Efectivo calculado', 120, '0.00', 'num'),
      new Celda('EfectivoRecibido', true, 'Efectivo Recibido', 120, '0.00', 'num'),
      new Celda('Diferencia', true, 'Diferencia', 100, '0.00', 'num'),
    ],
    _PMOLimpiar: () => { setForm({ ...VACIO, FechaCierre: hoy() }); setVentas([]); setCobranzas([]); setGastos([]); },
    _PMOMostrarRegistro: (f) => {
      setForm({ Id: f.Id, ConciliacionId: f.NroConciliacion, PersonalId: f.PersonalId, Personal: f.NombrePersonal ?? '', FechaCierre: f.FechaCierre, VentasContado: f.VentasContado, VentasCredito: f.VentasCreditos, TotalCobros: f.TotalCobros, EfectivoRecibido: f.EfectivoRecibido });
      api.get(`${URL}/${f.Id}/detalle?conciliacion=${f.NroConciliacion}`).then((r) => {
        setVentas(r.data.ventas); setCobranzas(r.data.cobranzas); setGastos(r.data.gastos.map((g) => ({ ...g, key: ++_k })));
      }).catch((e) => toast(e.message, 'error'));
    },
    _PMOLimpiarErrores: () => {},
    _PMOValidarCampos: () => {
      if (!form.ConciliacionId) { toast('Los Siguientes Campos Son Requeridos: Personal (Seleccione Una Conciliacion)', 'error'); return false; }
      if (!form.FechaCierre) { toast('Los Siguientes Campos Son Requeridos: Fecha', 'error'); return false; }
      return true;
    },
    _PMOGrabarRegistro: () => api.post(URL, { ConciliacionId: form.ConciliacionId, FechaCierre: form.FechaCierre, EfectivoRecibido: num(form.EfectivoRecibido), gastos: datosGastos() }),
    _PMOModificarRegistro: (f) => api.put(`${URL}/${form.Id}`, { fechaOriginal: f?.FechaCierre ?? m.filaActual?.FechaCierre, FechaCierre: form.FechaCierre, EfectivoRecibido: num(form.EfectivoRecibido), gastos: datosGastos() }),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}?fecha=${f.FechaCierre}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar el Cierre de Caja de ${tit(f.NombrePersonal)} ?`,
  });
  useEffect(() => { m._PMFiltrar(); }, [filtro.desde, filtro.hasta]); // eslint-disable-line react-hooks/exhaustive-deps
  const ro = !m.habilitado;
  function datosGastos() {
    return gastos.filter((g) => !(g.nuevo && g.eliminado)).map((g) => (g.nuevo ? { nuevo: true, ConceptoGastoId: g.ConceptoGastoId, Descripcion: g.Descripcion, Monto: g.Monto } : { Id: g.Id, eliminado: !!g.eliminado }));
  }

  const abrirConcs = async () => { try { setConcs((await api.get(`${URL}/conciliaciones`)).data); } catch (e) { toast(e.message, 'error'); } };
  const elegirConc = async (c) => {
    setConcs(null);
    try {
      const r = await api.get(`${URL}/preparar?${new URLSearchParams({ conciliacion: c.NroConciliacion, personal: c.PersonalId, fecha: form.FechaCierre || hoy() })}`);
      setForm((f) => ({ ...f, ConciliacionId: c.NroConciliacion, PersonalId: c.PersonalId, Personal: c.NombrePersonal, VentasContado: r.data.VentasContado, VentasCredito: r.data.VentasCredito, TotalCobros: r.data.TotalCobros }));
      setVentas(r.data.ventas); setCobranzas(r.data.cobranzas);
    } catch (e) { toast(e.message, 'error'); }
  };
  const imprimir = async (fila) => {
    const f = fila || m.filaActual; if (!f) return;
    try {
      const r = await api.get(`${URL}/${f.Id}/reporte`);
      imprimirHtml({ cuerpo: hojaArqueo({ ...r.data, f }), css: CSS_ARQUEO, titulo: `Arqueo de caja #${f.Id}` });
    } catch (e) { toast(e.message, 'error'); }
  };

  return (
    <>
    <ModeloF1 m={m} titulo="Cierre de caja" subtitulo="Cierre De Caja Del Distribuidor" icono="calc"
      accionesRegistro={<button className="btn btn-sec" onClick={() => imprimir()} disabled={!m.filaActual || m.habilitado}><Icono n="receipt" size={16} /> <span className="txt-btn">Reporte</span></button>}
      accionesFila={(f) => [{ texto: 'Reporte', icono: 'printer', onClick: () => imprimir(f) }]}
      extraBuscador={(
        <div className="filtros-lista">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={filtro.desde} max={filtro.hasta} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={filtro.hasta} min={filtro.desde} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, hasta: e.target.value }))} /></label>
        </div>
      )}>
      <div className="cierre-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos del cierre</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Fecha cierre" requerido><input type="date" className="control" value={form.FechaCierre} onChange={(e) => setForm((f) => ({ ...f, FechaCierre: e.target.value }))} readOnly={ro} disabled={ro} /></Campo>
            <Campo etiqueta="Distribuidor / conciliación" ancho={2} requerido>
              <div className="control-con-btn">
                <input className={`control ${!ro && m.modo === 'nuevo' ? 'control-elegir' : ''}`} readOnly value={form.Personal ? `${tit(form.Personal)} · Conciliación #${form.ConciliacionId}` : ''} placeholder="Seleccione una conciliación cerrada..."
                  onClick={() => !ro && m.modo === 'nuevo' && abrirConcs()} />
                {!ro && m.modo === 'nuevo' && <button type="button" className="btn btn-sec" onClick={abrirConcs}><Icono n="search" size={15} /> Buscar</button>}
              </div>
            </Campo>
          </div>
        </div>

        <div className="tarjeta cierre-resumen">
          <h2 className="tarjeta-tit">Resumen</h2>
          <dl>
            <div><dt>Ventas al contado</dt><dd>{bs(form.VentasContado)}</dd></div>
            <div><dt>Ventas al crédito</dt><dd className="suave">{bs(form.VentasCredito)}</dd></div>
            <div><dt>Cobranzas</dt><dd>+ {bs(form.TotalCobros)}</dd></div>
            <div><dt>Gastos</dt><dd className="neg">− {bs(totalGastos)}</dd></div>
            <div className="total"><dt>Efectivo calculado</dt><dd>{bs(calculado)}</dd></div>
            <div className="recibido"><dt>Efectivo recibido</dt><dd>{ro ? bs(form.EfectivoRecibido) : <input className="control num" type="number" min="0" step="0.01" inputMode="decimal" value={form.EfectivoRecibido} onChange={(e) => setForm((f) => ({ ...f, EfectivoRecibido: e.target.value }))} aria-label="Efectivo recibido" />}</dd></div>
            <div className={`dif ${diferencia < 0 ? 'neg' : diferencia > 0 ? 'pos' : 'cero'}`}><dt>Diferencia</dt><dd>{bs(diferencia)}</dd></div>
          </dl>
        </div>

        <div className="tarjeta cierre-gastos">
          <div className="detalle-cab-venta">
            <h2 className="tarjeta-tit">Gastos <span className="contador">{vig.length}</span></h2>
            {!ro && <button className="btn btn-pri" onClick={() => setNuevoGasto(true)}><Icono n="plus" size={16} /> Agregar gasto</button>}
          </div>
          <div className="tabla-scroll">
            <table className="grid grid-tarjetas tabla-detalle">
              <thead><tr><th>Concepto</th><th>Descripción</th><th className="num">Monto</th>{!ro && <th aria-label="Quitar" />}</tr></thead>
              <tbody>
                {vig.map((g) => (
                  <tr key={g.key} className={g.nuevo ? 'fila-nueva' : ''}>
                    <td data-label="Concepto"><b>{tit(g.Detalle ?? '')}</b></td><td data-label="Descripción">{g.Descripcion}</td><td data-label="Monto" className="num">{bs(g.Monto)}</td>
                    {!ro && <td className="col-acc"><button className="btn-icono peligro" onClick={() => setGastos((x) => x.map((y) => (y.key === g.key ? { ...y, eliminado: true } : y)))} aria-label="Quitar gasto" title="Quitar"><Icono n="trash" size={16} /></button></td>}
                  </tr>
                ))}
                {!vig.length && <tr><td colSpan={4} className="vacio">Sin gastos</td></tr>}
              </tbody>
            </table>
          </div>
        </div>
        <div className="tarjeta"><h2 className="tarjeta-tit">Ventas de la conciliación <span className="contador">{ventas.length}</span></h2>
          <TablaReporte titulo="VENTAS DE LA CONCILIACION" columnas={colsVentas(ventas)} filas={ventas} claveFila="__no" vacio="Sin ventas" /></div>
        <div className="tarjeta"><h2 className="tarjeta-tit">Cobranzas <span className="contador">{cobranzas.length}</span></h2>
          <TablaReporte titulo="COBRANZAS" columnas={columnasDe(cobranzas)} filas={cobranzas} claveFila="__no" vacio="Sin cobranzas" /></div>
      </div>

      <EfectoAyuda abierto={!!concs} titulo="SELECCIONE CONCILIACIÓN CERRADA" dt={(concs || []).map((c) => ({ ...c, Id: c.NroConciliacion, FechaTxt: fechaCorta(`${String(c.Fecha).slice(0, 10)}T00:00:00Z`) }))}
        columnas={[new Celda('NroConciliacion', true, 'NRO CONCILIACIÓN', 120), new Celda('FechaTxt', true, 'FECHA', 120), new Celda('NombrePersonal', true, 'CHOFER / DISTRIBUIDOR', 300)]}
        onCancelar={() => setConcs(null)} onSeleccionar={elegirConc} />
      {nuevoGasto && <DialogoGasto onCerrar={() => setNuevoGasto(false)} onAgregar={(g) => { setGastos((x) => [...x, { ...g, key: ++_k, nuevo: true }]); setNuevoGasto(false); }} />}
    </ModeloF1>
    </>
  );
}

// Tec_Gastos: concepto (libreria 14), descripcion y monto > 0
function DialogoGasto({ onAgregar, onCerrar }) {
  const toast = useToast();
  const [conceptos, setConceptos] = useState([]);
  const [g, setG] = useState({ ConceptoGastoId: '', Descripcion: '', Monto: '' });
  useEffect(() => { api.get(`${URL}/conceptos`).then((r) => { setConceptos(r.data); setG((x) => ({ ...x, ConceptoGastoId: r.data[0]?.cnnum ?? '' })); }).catch((e) => toast(e.message, 'error')); }, []); // eslint-disable-line react-hooks/exhaustive-deps
  const aceptar = () => {
    if (!(num(g.Monto) > 0)) { toast('Debe colocar Un Monto Valido'.toUpperCase(), 'error'); return; }
    const c = conceptos.find((x) => String(x.cnnum) === String(g.ConceptoGastoId));
    if (!c) { toast('Debe Seleccionar un Concepto Valido'.toUpperCase(), 'error'); return; }
    onAgregar({ ConceptoGastoId: c.cnnum, Detalle: c.cndesc1, Descripcion: g.Descripcion, Monto: num(g.Monto) });
  };
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-sm" role="dialog" aria-modal="true" aria-label="Agregar gasto" onMouseDown={(e) => e.stopPropagation()} onKeyDown={(e) => { if (e.key === 'Escape') onCerrar(); if (e.key === 'Enter') aceptar(); }}>
        <div className="modal-cab"><h3>Agregar gasto</h3><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="campos una-col">
          <Campo etiqueta="Concepto" requerido><ComboMulti dt={conceptos} valor={g.ConceptoGastoId} onChange={(x) => setG((o) => ({ ...o, ConceptoGastoId: x }))} id="cnnum" nameId="Cód." descripcionId="cndesc1" descripcion="Concepto Gasto" /></Campo>
          <Campo etiqueta="Descripción"><input className="control" value={g.Descripcion} onChange={(e) => setG((o) => ({ ...o, Descripcion: e.target.value }))} maxLength={300} /></Campo>
          <Campo etiqueta="Monto (Bs)" requerido><input autoFocus className="control num" type="number" min="0" step="0.01" inputMode="decimal" value={g.Monto} onChange={(e) => setG((o) => ({ ...o, Monto: e.target.value }))} /></Campo>
        </div>
        <div className="modal-acciones"><button className="btn btn-sec" onClick={onCerrar}>Cancelar</button><button className="btn btn-pri" onClick={aceptar}>Agregar</button></div>
      </div>
    </div>,
    document.body,
  );
}
