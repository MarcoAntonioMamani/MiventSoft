// =====================================================================
//  Tec_Ventas.vb -> Gestion De Ventas   (SP: MAM_Ventas)
//  + Tec_VentasDetalle.vb (agregar productos) + frm_CantidadCajas.vb (cantidad cajas/unidades)
//
//  El navegador calcula para MOSTRAR; al grabar el servidor vuelve a tomar precios y
//  stock de los mismos SP y recalcula todo (no se confia en montos del navegador).
// =====================================================================
import { useEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { useLocation } from 'react-router-dom';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, SwitchButton, fechaCorta } from '../../componentes/Controles';
import { EfectoAyuda } from '../../componentes/Efecto';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import NotaImpresa from '../../componentes/NotaImpresa';
import { useListaTeclado } from '../../componentes/useListaTeclado';
import { palabrasDe, coincide } from '../../global/buscar';

const URL = '/api/ventas/ventas';

// Configuracion: Tec_Ventas (MAM_Ventas) y Proforma (MAM_Proforma) comparten la pantalla
export const CFG_VENTAS = { url: URL, titulo: 'Ventas', subtitulo: 'Gestion De Ventas', icono: 'cart', nombre: 'Venta', incremento: true, conciliacion: true };
export const CFG_PROFORMA = {
  url: '/api/ventas/proformas', titulo: 'Proformas', subtitulo: 'Gestion De Proformas', icono: 'list', nombre: 'Proforma', incremento: false, conciliacion: false,
  // ProformaDetalle: todos los productos (sin stock en rojo) y se puede elegir cualquiera: no es una venta
  sinStock: true,
  tipos: { proforma: { texto: 'Nueva Proforma', icono: 'list', detalle: 'Cotización para el cliente (no descuenta stock)' } },
};
const dos = (n) => String(n).padStart(2, '0');
const hoy = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const bs = (n, d = 2) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: d, maximumFractionDigits: d });
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };

// Las tres formas de crear del escritorio (botones de la lista)
const TIPOS = {
  pedido: { texto: 'Nuevo Pedido', icono: 'cart', detalle: 'Queda PENDIENTE para asignar a un chofer' },
  directa: { texto: 'Nueva Venta Directa', icono: 'truck', detalle: 'Se entrega en el momento · requiere conciliación abierta' },
  sinconciliacion: { texto: 'Venta Directa Sin Conciliación', icono: 'store', detalle: 'Solo usuarios autorizados' },
};

const VACIO = {
  Id: '', SucursalId: '', FechaVenta: '', PersonalId: 0, Personal: '', ClienteId: 0, NombreCliente: '', TipoVenta: true,
  FechaVencimientoCredito: '', FechaEntregar: '', Glosa: '', Descuento: 0, PorcDescuento: 0, EstadoPedido: '', NombreEstado: '',
  Distribuidor: '', Incremento: 0, VentaDirecta: 0, SinConciliacion: 0,
};

// ---------- calculos de una fila (P_PonerTotal / CellValueChanged) ----------
// Un descuento invalido se conserva tal cual (la celda queda en rojo y no deja grabar) y el total va sin descuento
function recalcular(f) {
  const sub = num(f.CantidadUnitaria) * num(f.Precio);
  let porc = num(f.ProcentajeDescuento);
  let monto = num(f.MontoDescuento);
  if (f._origenDesc === 'monto') {
    if (monto < 0 || monto > sub + 1e-9) return { ...f, SubTotal: sub, Total: sub };
    porc = sub ? (monto * 100) / sub : 0;
  } else {
    if (porc < 0 || porc > 100) return { ...f, SubTotal: sub, Total: sub };
    monto = sub * (porc / 100);
  }
  return { ...f, SubTotal: sub, ProcentajeDescuento: porc, MontoDescuento: monto, Total: sub - monto };
}

// Validacion de una fila editada (CellValueChanged de VB): { campo: mensaje }
function errorFilaVenta(f, sinStock) {
  const e = {};
  const cant = num(f.CantidadUnitaria);
  if (!(cant > 0)) e.Cantidad = 'La Cantidad debe ser Mayor a 0';
  else if (!sinStock && cant > num(f.stock) + 1e-9) e.Cantidad = `La Cantidad = ${n2(cant)} es mayor al Stock del Producto = ${n2(f.stock)}`;
  const sub = cant * num(f.Precio);
  if (f._origenDesc === 'monto') {
    if (num(f.MontoDescuento) < 0 || num(f.MontoDescuento) > sub + 1e-9) e.Desc = `El Monto de Descuento no puede ser mayor al SubTotal = ${n2(sub)}`;
  } else if (num(f.ProcentajeDescuento) < 0 || num(f.ProcentajeDescuento) > 100) e.Desc = 'El Porcentaje de Descuento debe estar entre 0 y 100';
  return e;
}

export default function Tec_Ventas() { return <PantallaVentas cfg={CFG_VENTAS} />; }

export function PantallaVentas({ cfg }) {
  const URL = cfg.url; // eslint-disable-line no-shadow
  const TIPOS_CFG = cfg.tipos || TIPOS;
  const tipoInicial = Object.keys(TIPOS_CFG)[0];
  const toast = useToast();
  const location = useLocation();
  const [form, setForm] = useState(VACIO);
  const [detalle, setDetalle] = useState([]); // filas con { key, Id, nuevo, eliminado, ... }
  const [cargandoDet, setCargandoDet] = useState(false);
  const [errores, setErrores] = useState({});
  const [ini, setIni] = useState(null);
  const [filtro, setFiltro] = useState({ desde: hoy(), hasta: hoy(), estado: 5 });
  const [ayudaVend, setAyudaVend] = useState(false);
  const [personal, setPersonal] = useState([]);
  const [ayudaCli, setAyudaCli] = useState(false);
  const [selector, setSelector] = useState(false);
  const [recibo, setRecibo] = useState(null);
  const tipoNuevo = useRef(tipoInicial);
  const pedidoDet = useRef(0);

  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };

  useEffect(() => { api.get(`${URL}/inicio`).then((r) => setIni(r.data)).catch((e) => toast(e.message, 'error')); }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const vigentes = detalle.filter((f) => !f.eliminado);
  const errFila = (f) => (f._editado ? errorFilaVenta(f, cfg.sinStock) : {});
  const suma = vigentes.reduce((s, f) => s + num(f.Total), 0);

  // _PMOLimpiar (venta NUEVA): vendedor = personal del usuario, cliente Id 1, sucursal 1ra, fechas hoy
  const limpiar = (datosIni = ini) => {
    const t = tipoNuevo.current;
    setDetalle([]);
    setForm({
      ...VACIO,
      SucursalId: datosIni?.sucursales?.[0]?.aanumi ?? '',
      FechaVenta: hoy(), FechaVencimientoCredito: hoy(), FechaEntregar: hoy(),
      PersonalId: datosIni?.vendedorDefecto?.Id ?? 0, Personal: datosIni?.vendedorDefecto?.Nombre ?? '',
      ClienteId: datosIni?.clienteDefecto?.Id ?? 0, NombreCliente: datosIni?.clienteDefecto?.NombreCliente ?? '',
      EstadoPedido: t === 'pedido' ? 1 : 4, // proforma y ventas directas: 4
      NombreEstado: datosIni?.estados?.find((e) => Number(e.id) === (t === 'pedido' ? 1 : 4))?.NombreEstado ?? '',
      Incremento: cfg.incremento ? (datosIni?.incrementoHoy ?? 0) : 0,
      VentaDirecta: t === 'pedido' ? 0 : 1, SinConciliacion: t === 'sinconciliacion' || t === 'proforma' ? 1 : 0, // proforma: VentaDirecta=1, SinConciliacion=1
    });
  };

  const mostrar = (f) => {
    setForm({
      Id: f.Id, SucursalId: f.SucursalId ?? '', FechaVenta: f.FechaVenta ?? '', PersonalId: f.PersonalId ?? 0, Personal: f.Personal ?? '',
      ClienteId: f.ClienteId ?? 0, NombreCliente: f.NombreCliente ?? '', TipoVenta: Number(f.TipoVenta) === 1,
      FechaVencimientoCredito: f.FechaVencimientoCredito ?? f.FechaVenta ?? '', FechaEntregar: f.FechaEntrega ?? f.FechaVenta ?? '',
      Glosa: f.Glosa ?? '', Descuento: num(f.Descuento), PorcDescuento: 0, EstadoPedido: f.EstadoPedido ?? '', NombreEstado: f.NombreEstado ?? '',
      Distribuidor: f.NombrePersonal ?? '', Incremento: num(f.IncrementoVenta), VentaDirecta: num(f.VentaDirecta), SinConciliacion: num(f.VentaDirectaSinConciliacion),
    });
    // _prCargarDetalleVenta(Id)
    const n = ++pedidoDet.current;
    setDetalle([]);
    setCargandoDet(true);
    api.get(`${URL}/${f.Id}/detalle`).then((r) => {
      if (n !== pedidoDet.current) return;
      setDetalle(r.data.map((d) => ({ ...d, key: `e${d.Id}`, nuevo: false, eliminado: false })));
    }).catch((e) => toast(e.message, 'error')).finally(() => n === pedidoDet.current && setCargandoDet(false));
  };

  const _datos = () => ({
    modo: tipoNuevo.current,
    fechaOriginal: m.filaActual?.FechaVenta,
    SucursalId: form.SucursalId, FechaVenta: form.FechaVenta, PersonalId: form.PersonalId, ClienteId: form.ClienteId,
    TipoVenta: form.TipoVenta ? 1 : 0, FechaVencimientoCredito: form.FechaVencimientoCredito, FechaEntregar: form.FechaEntregar,
    Glosa: form.Glosa, Descuento: num(form.Descuento),
    detalle: detalle.map((f) => (f.nuevo
      ? { nuevo: true, eliminado: !!f.eliminado, ProductoId: f.ProductoId, PrecioId: f.PrecioId, CantidadUnitaria: f.CantidadUnitaria, ProcentajeDescuento: f.ProcentajeDescuento, MontoDescuento: f.MontoDescuento }
      : { Id: f.Id, eliminado: !!f.eliminado, CantidadUnitaria: f.CantidadUnitaria, ProcentajeDescuento: f.ProcentajeDescuento, MontoDescuento: f.MontoDescuento })),
  });

  const bloqueo = (accion) => (f) => (f && f.EstadoConciliacion !== null && f.EstadoConciliacion !== undefined && Number(f.EstadoConciliacion) === 0
    ? `Esta Venta no puede ser ${accion} por que pertenece a una conciliacion cerrada = ${f.conciliacionId}` : null);

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => {
      const q = new URLSearchParams({ desde: filtro.desde, hasta: filtro.hasta });
      return (await api.get(`${URL}?${q}`)).data.map((f) => ({ ...f, Fecha: fechaCorta(`${f.FechaVenta}T00:00:00Z`) }));
    },
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 70, '', 'num'),
      new Celda('Fecha', true, 'Fecha Venta', 100),
      new Celda('HoraRegistro', true, 'Hora', 70),
      new Celda('Personal', true, 'Personal', 160),
      new Celda('NombreCliente', true, 'Cliente', 200),
      new Celda('TVenta', true, 'Tipo Venta', 90),
      new Celda('TotalVenta', true, 'Total Venta', 110, '0.00', 'num'),
      new Celda('NombreEstado', true, 'Estado Pedido', 110),
      new Celda('conciliacionId', true, 'Nro Conciliación', 90),
    ],
    // cbFiltroEstado: 5 = Todos
    filtroExtra: (f) => Number(filtro.estado) === 5 || Number(f.EstadoPedido) === Number(filtro.estado),
    filtroExtraClave: filtro.estado,
    _PMOLimpiar: () => limpiar(),
    _PMOMostrarRegistro: mostrar,
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOBloqueoModificar: bloqueo('Modificada'),
    _PMOBloqueoEliminar: bloqueo('Eliminada'),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.SucursalId) e.SucursalId = 'Seleccione una Sucursal';
      if (!(form.PersonalId > 0)) e.Personal = 'Seleccione un Personal';
      if (!(form.ClienteId > 0)) e.Cliente = 'Seleccione un Cliente';
      if (!form.FechaVenta) e.FechaVenta = 'Ingrese una Fecha Valida';
      if (!vigentes.length) e.detalle = 'Por Favor Inserte un Detalle';
      const ed = errorDetalle(vigentes, errFila, (f) => f.Producto);
      if (ed) { e.detalle = ed; toast(ed, 'error', 6000); }
      if (num(form.Descuento) > suma + 1e-9) e.Descuento = 'El descuento no puede ser mayor al total';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: async () => {
      const r = await api.post(URL, _datos());
      // la venta nueva puede quedar fuera del rango de fechas de la lista
      if (r.fecha && (r.fecha < filtro.desde || r.fecha > filtro.hasta)) setFiltro((x) => ({ ...x, desde: r.fecha < x.desde ? r.fecha : x.desde, hasta: r.fecha > x.hasta ? r.fecha : x.hasta }));
      // como el escritorio: al grabar se ofrece la nota para imprimir (Imprimir / Cerrar)
      verNota(r.id, r.fecha || form.FechaVenta, 'nueva');
      return r;
    },
    _PMOModificarRegistro: async () => {
      const r = await api.put(`${URL}/${form.Id}`, _datos());
      verNota(r.id || form.Id, r.fecha || form.FechaVenta, 'modificada');
      return r;
    },
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}?fecha=${encodeURIComponent(f.FechaVenta)}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar la ${cfg.nombre} ${f.Id} ?`,
  });
  const ro = !m.habilitado;

  // al cambiar el rango de fechas se recarga (btnConfirmarSalir "Actualizar")
  const primera = useRef(true);
  useEffect(() => { if (primera.current) { primera.current = false; return; } m._PMFiltrar(); }, [filtro.desde, filtro.hasta]); // eslint-disable-line react-hooks/exhaustive-deps

  // tipo de venta segun la URL (/ventas/ventas/nuevo?tipo=directa)
  useEffect(() => {
    if (m.ruta.tipo !== 'nuevo') return;
    const t = new URLSearchParams(location.search).get('tipo');
    tipoNuevo.current = TIPOS_CFG[t] ? t : tipoInicial;
    if (ini) limpiar(ini);
  }, [m.ruta.tipo, location.search, ini]); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => { if (m.ruta.tipo !== 'nuevo') tipoNuevo.current = tipoInicial; }, [m.ruta.tipo]); // eslint-disable-line react-hooks/exhaustive-deps

  // VentaDirectaVariable: en "sin conciliacion" se listan productos como pedido normal
  // ProformaDetalle siempre lista con @tipo 5 (nunca los productos de la conciliacion del chofer)
  const directaProductos = cfg.sinStock || form.SinConciliacion === 1 ? 0 : form.VentaDirecta;

  const menuNuevo = ini && m.puede.insertar ? Object.entries(TIPOS_CFG)
    .filter(([k]) => k !== 'sinconciliacion' || ini.permiteSinConciliacion)
    .map(([k, x]) => ({ ...x, accion: () => m._PMNuevo(`?tipo=${k}`) })) : [];

  // ---------- detalle ----------
  const cambiarFila = (key, cambio) => {
    setDetalle((d) => d.map((f) => {
      if (f.key !== key) return f;
      // el valor se aplica aunque sea invalido: la celda lo marca y Grabar lo bloquea (errorFilaVenta)
      let nf = { ...f, ...cambio, _editado: true };
      if ('Cantidad' in cambio) nf.CantidadUnitaria = num(cambio.Cantidad) * num(f.Conversion);
      if ('CantidadUnitaria' in cambio || 'Cantidad' in cambio) {
        nf.CantidadUnitaria = num(nf.CantidadUnitaria);
        nf.Cantidad = nf.CantidadUnitaria / (num(f.Conversion) || 1);
        nf._origenDesc = 'porc'; // al cambiar la cantidad se conserva el % (CellValueChanged de VB)
      }
      nf = recalcular(nf);
      return nf;
    }));
    quitarError('detalle');
  };
  const quitarFila = (key) => setDetalle((d) => d.map((f) => (f.key === key ? { ...f, eliminado: true } : f)));

  // Si cambia sucursal o cliente, los productos nuevos se vuelven a elegir (el precio depende de ambos)
  const cambiarContexto = (k, v, extra = {}) => {
    const hayNuevos = detalle.some((f) => f.nuevo && !f.eliminado);
    setForm((x) => ({ ...x, [k]: v, ...extra }));
    if (hayNuevos) {
      setDetalle((d) => d.filter((f) => !f.nuevo));
      toast('Se quitaron los productos agregados: vuelva a agregarlos (el precio depende de la sucursal y del cliente).', 'aviso', 7000);
    }
  };

  const abrirSelector = async () => {
    if (!(form.ClienteId > 0)) { toast('Seleccione Cliente'.toUpperCase(), 'error'); return; }
    if (!form.SucursalId) { toast('Seleccion Sucursal'.toUpperCase(), 'error'); return; }
    if (!(form.PersonalId > 0)) { toast('Seleccione Vendedor'.toUpperCase(), 'error'); return; }
    // btnSeleccionarProducto_Click: venta directa (con conciliacion) exige conciliacion abierta
    if (cfg.conciliacion && form.VentaDirecta === 1 && form.SinConciliacion === 0) {
      try {
        const r = await api.get(`${URL}/conciliacion/${form.PersonalId}`);
        if (!r.data.abierta) { toast(`No es posible Crear ventas para El Vendedor ${form.Personal} No tiene conciliación abierta`, 'error', 8000); return; }
      } catch (e) { toast(e.message, 'error'); return; }
    }
    setSelector(true);
  };

  const agregarProducto = (p, precioId, cantUni) => {
    const conv = num(p.Conversion) || 1;
    const fila = recalcular({
      key: `n${Date.now()}${p.Id}`, Id: null, nuevo: true, eliminado: false, ProductoId: p.Id, Producto: p.NombreProducto, PrecioId: precioId,
      Conversion: conv, Precio: num(p.PrecioVenta) / conv, stock: num(p.stock), CantidadUnitaria: cantUni, Cantidad: cantUni / conv,
      ProcentajeDescuento: 0, MontoDescuento: 0,
    });
    setDetalle((d) => [...d, fila]);
    quitarError('detalle');
  };

  // descuento general (tbPdesc / tbMdesc)
  const setDescMonto = (v) => {
    quitarError('Descuento');
    const mo = num(v);
    if (mo < 0 || mo > suma) { setForm((f) => ({ ...f, Descuento: 0, PorcDescuento: 0 })); if (mo > suma) toast('El descuento no puede ser mayor al total', 'error'); return; }
    setForm((f) => ({ ...f, Descuento: v, PorcDescuento: suma ? (mo * 100) / suma : 0 }));
  };
  const setDescPorc = (v) => {
    quitarError('Descuento');
    const p = num(v);
    if (p <= 0 || p > 100) { setForm((f) => ({ ...f, Descuento: 0, PorcDescuento: p > 100 ? 0 : v })); return; }
    setForm((f) => ({ ...f, PorcDescuento: v, Descuento: Math.round(suma * (p / 100) * 100) / 100 }));
  };
  const porcMostrado = form.PorcDescuento || (suma ? (num(form.Descuento) * 100) / suma : 0);
  const total = suma - num(form.Descuento) + num(form.Incremento);

  // P_GenerarReporte: nota (vista previa) desde el registro, desde la lista o al grabar (ReporteVenta del escritorio)
  const verNota = async (id, fecha, grabada = false) => {
    if (!id || !fecha) return;
    try {
      const r = await api.get(`${URL}/${id}/recibo?fecha=${encodeURIComponent(fecha)}`);
      setRecibo({ ...r.data, grabada });
    } catch (e) { toast(e.message, 'error'); }
  };
  const imprimir = () => m.filaActual && verNota(m.filaActual.Id, m.filaActual.FechaVenta);

  const tipoTexto = !cfg.conciliacion ? cfg.nombre : m.modo === 'nuevo' ? TIPOS_CFG[tipoNuevo.current]?.texto
    : form.SinConciliacion === 1 ? 'Venta directa sin conciliación' : form.VentaDirecta === 1 ? 'Venta directa' : 'Pedido';

  return (
    <>
    <ModeloF1 m={m} titulo={cfg.titulo} subtitulo={cfg.subtitulo} icono={cfg.icono} menuNuevo={menuNuevo}
      accionesFila={(f) => [{ texto: cfg.nombre === 'Venta' ? 'Nota de venta' : 'Imprimir', icono: 'printer', onClick: () => verNota(f.Id, f.FechaVenta) }]}
      accionesRegistro={<button className="btn btn-sec" onClick={imprimir} disabled={!m.filaActual}><Icono n="printer" size={16} /> <span className="txt-btn">{cfg.nombre === 'Venta' ? 'Nota de venta' : 'Imprimir'}</span></button>}
      extraBuscador={(
        <div className="filtros-lista">
          {cfg.nombre === 'Venta' && (
            <button type="button" className="btn btn-sec btn-nota-lista" onClick={() => (m.filaActual ? imprimir() : toast('Seleccione una venta de la lista', 'aviso'))}
              title={m.filaActual ? `Imprimir nota de venta #${m.filaActual.Id}` : 'Seleccione una venta de la lista'}>
              <Icono n="receipt" size={16} /> Imprimir nota de venta{m.filaActual ? ` #${m.filaActual.Id}` : ''}
            </button>
          )}
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={filtro.desde} max={filtro.hasta} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={filtro.hasta} min={filtro.desde} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, hasta: e.target.value }))} /></label>
          <div className="filtro filtro-estado">
            <span>Estado</span>
            <div className="chips-estado" role="group" aria-label="Filtrar por estado">
              {[{ id: 5, NombreEstado: 'TODOS' }, ...(ini?.estados || [])].map((e) => (
                <button key={e.id} type="button" className={`chip-filtro ${Number(filtro.estado) === Number(e.id) ? 'activo' : ''}`} aria-pressed={Number(filtro.estado) === Number(e.id)}
                  onClick={() => setFiltro((x) => ({ ...x, estado: e.id }))}>{tit(e.NombreEstado)}</button>
              ))}
            </div>
          </div>
          <div className="filtros-nuevo">
            {menuNuevo.map((x) => <button key={x.texto} className="btn btn-sec btn-sm" onClick={x.accion}><Icono n={x.icono} size={15} /> {x.texto}</button>)}
          </div>
        </div>
      )}>
      <div className="venta-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos de la {cfg.nombre.toLowerCase()} <span className={`chip-tipo ${form.VentaDirecta ? 'directa' : ''}`}>{tipoTexto}</span></h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Sucursal" error={errores.SucursalId} requerido>
              <ComboMulti dt={ini?.sucursales || []} valor={form.SucursalId} onChange={(v) => { quitarError('SucursalId'); cambiarContexto('SucursalId', v); }} id="aanumi" nameId="Código" descripcionId="aabdes" descripcion="Sucursal" deshabilitado={ro} error={errores.SucursalId} />
            </Campo>
            <Campo etiqueta="Fecha transacción" error={errores.FechaVenta} requerido>
              <input type="date" className="control" value={form.FechaVenta} onChange={set('FechaVenta')} readOnly={ro} disabled={ro} />
            </Campo>
            <Campo etiqueta="Vendedor" ancho={2} error={errores.Personal} requerido>
              <div className="control-con-btn">
                <input className={`control ${!ro ? 'control-elegir' : ''}`} value={tit(form.Personal)} readOnly placeholder="Toque para buscar el vendedor..."
                  onClick={() => !ro && abrirVend()} onKeyDown={(e) => !ro && (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), abrirVend())} />
                {!ro && <button type="button" className="btn btn-sec" onClick={abrirVend}><Icono n="search" size={15} /> Buscar</button>}
              </div>
            </Campo>
            <Campo etiqueta="Cliente" ancho={2} error={errores.Cliente} requerido>
              <div className="control-con-btn">
                <input className={`control ${!ro ? 'control-elegir' : ''}`} value={tit(form.NombreCliente)} readOnly placeholder="Toque para buscar el cliente..."
                  onClick={() => !ro && setAyudaCli(true)} onKeyDown={(e) => !ro && (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), setAyudaCli(true))} />
                {!ro && <button type="button" className="btn btn-sec" onClick={() => setAyudaCli(true)}><Icono n="search" size={15} /> Buscar</button>}
              </div>
            </Campo>
            <Campo etiqueta="Tipo de venta">
              <SwitchButton valor={form.TipoVenta} onChange={set('TipoVenta')} deshabilitado={ro} textoOn="CONTADO" textoOff="CREDITO" />
            </Campo>
            {!form.TipoVenta && (
              <Campo etiqueta="Venc. crédito">
                <input type="date" className="control" value={form.FechaVencimientoCredito} min={form.FechaVenta} onChange={set('FechaVencimientoCredito')} readOnly={ro} disabled={ro} />
              </Campo>
            )}
            <Campo etiqueta="Fecha a entregar / entregado">
              <input type="date" className="control" value={form.FechaEntregar} min={m.modo === 'nuevo' ? hoy() : undefined} onChange={set('FechaEntregar')} readOnly={ro} disabled={ro} />
            </Campo>
            <Campo etiqueta="Estado pedido"><input className="control" value={tit(form.NombreEstado)} readOnly /></Campo>
            {m.modo === 'ver' && <Campo etiqueta="Distribuidor asignado"><input className="control" value={tit(form.Distribuidor) || '—'} readOnly /></Campo>}
            <Campo etiqueta="Glosa" ancho={2}><input className="control" value={form.Glosa} onChange={set('Glosa')} readOnly={ro} maxLength={500} /></Campo>
          </div>
        </div>

        <div className="tarjeta venta-detalle">
          <div className="detalle-cab-venta">
            <h2 className="tarjeta-tit">Detalle productos <span className="contador">{vigentes.length}</span></h2>
            {!ro && <button className="btn btn-pri" onClick={abrirSelector}><Icono n="plus" size={16} /> Agregar productos</button>}
          </div>
          {errores.detalle && <p className="campo-error" role="alert">{errores.detalle}</p>}
          {cargandoDet ? <div className="cargando-bloque"><span className="spinner" /> Cargando detalle...</div> : (
            <div className="tabla-scroll">
              <table className="grid grid-tarjetas tabla-detalle">
                <thead>
                  <tr>
                    <th>Producto</th><th className="num">Cant. caja</th><th className="num">Cant. unid.</th><th className="num">Precio</th>
                    <th className="num">SubTotal</th><th className="num">% Desc.</th><th className="num">M. Desc.</th><th className="num">Total</th>{!ro && <th aria-label="Quitar" />}
                  </tr>
                </thead>
                <tbody>
                  {vigentes.map((f) => (
                    <tr key={f.key} className={f.nuevo ? 'fila-nueva' : ''}>
                      <td data-label="Producto" className="det-producto"><b>{tit(f.Producto)}</b><small>Cód. {f.ProductoId} · stock {n2(f.stock)} u · {n2(f.Conversion)} u/caja</small></td>
                      <td data-label="Cant. caja" className="num">{ro ? n2(f.Cantidad) : <NumeroCelda valor={f.Cantidad} onCambio={(x) => cambiarFila(f.key, { Cantidad: x })} etiqueta={`Cajas de ${f.Producto}`} error={errFila(f).Cantidad} />}</td>
                      <td data-label="Cant. unid." className="num">{ro ? n2(f.CantidadUnitaria) : <NumeroCelda valor={f.CantidadUnitaria} onCambio={(x) => cambiarFila(f.key, { CantidadUnitaria: x })} etiqueta={`Unidades de ${f.Producto}`} error={errFila(f).Cantidad} />}</td>
                      <td data-label="Precio" className="num">{bs(f.Precio, 4)}</td>
                      <td data-label="SubTotal" className="num">{bs(f.SubTotal)}</td>
                      <td data-label="% Desc." className="num">{ro ? n2(f.ProcentajeDescuento) : <NumeroCelda valor={Math.round(num(f.ProcentajeDescuento) * 100) / 100} onCambio={(x) => cambiarFila(f.key, { ProcentajeDescuento: num(x), _origenDesc: 'porc' })} etiqueta="Porcentaje de descuento" error={f._origenDesc !== 'monto' ? errFila(f).Desc : undefined} />}</td>
                      <td data-label="M. Desc." className="num">{ro ? bs(f.MontoDescuento) : <NumeroCelda valor={Math.round(num(f.MontoDescuento) * 100) / 100} onCambio={(x) => cambiarFila(f.key, { MontoDescuento: num(x), _origenDesc: 'monto' })} etiqueta="Monto de descuento" error={f._origenDesc === 'monto' ? errFila(f).Desc : undefined} />}</td>
                      <td data-label="Total" className="num det-total"><b>{bs(f.Total)}</b></td>
                      {!ro && <td className="col-acc"><button className="btn-icono peligro" onClick={() => quitarFila(f.key)} aria-label={`Quitar ${f.Producto}`} title="Quitar"><Icono n="trash" size={16} /></button></td>}
                    </tr>
                  ))}
                  {!vigentes.length && <tr><td colSpan={9} className="vacio">{ro ? 'Sin productos' : 'Agregue productos con el botón "Agregar productos"'}</td></tr>}
                </tbody>
              </table>
            </div>
          )}
          <div className="venta-totales">
            <div><span>Suma detalle</span><b>{bs(suma)}</b></div>
            <div className="venta-desc">
              <span>Descuento</span>
              {ro ? <b>{bs(form.Descuento)} <small>({n2(porcMostrado)}%)</small></b> : (
                <span className="desc-inputs">
                  <label><input className="control num" type="number" min="0" max="100" step="0.01" value={Math.round(num(porcMostrado) * 100) / 100 || ''} placeholder="0" onChange={(e) => setDescPorc(e.target.value)} aria-label="Porcentaje descuento" />%</label>
                  <label>Bs<input className="control num" type="number" min="0" step="0.01" value={form.Descuento || ''} placeholder="0" onChange={(e) => setDescMonto(e.target.value)} aria-label="Monto descuento" /></label>
                </span>
              )}
            </div>
            {errores.Descuento && <p className="campo-error">{errores.Descuento}</p>}
            {cfg.incremento && <div><span>Incremento adicional</span><b>{bs(form.Incremento)}</b></div>}
            <div className="venta-total"><span>Total {cfg.nombre.toLowerCase()}</span><b>Bs {bs(total)}</b></div>
          </div>
        </div>
      </div>

      <EfectoAyuda
        abierto={ayudaVend}
        titulo="SELECCIONE PERSONAL"
        dt={personal}
        columnas={[new Celda('Nombre', true, 'NOMBRE', 300), new Celda('Direccion', true, 'DIRECCION', 180), new Celda('Telefono01', true, 'TELEFONO', 140)]}
        onCancelar={() => setAyudaVend(false)}
        onSeleccionar={(r) => { quitarError('Personal'); setForm((f) => ({ ...f, PersonalId: r.Id, Personal: r.Nombre })); setAyudaVend(false); }}
      />
      {ayudaCli && (
        <BuscarCliente url={`${URL}/clientes`} onCancelar={() => setAyudaCli(false)}
          onSeleccionar={(r) => { quitarError('Cliente'); setAyudaCli(false); if (Number(r.Id) !== Number(form.ClienteId)) cambiarContexto('ClienteId', r.Id, { NombreCliente: r.NombreCliente }); }} />
      )}
      {selector && (
        <SelectorProductos
          url={URL}
          parametros={{ sucursal: form.SucursalId, cliente: form.ClienteId, directa: directaProductos, vendedor: form.PersonalId }}
          categorias={ini?.categoriasPrecio || []}
          enDetalle={new Set(vigentes.map((f) => Number(f.ProductoId)))}
          onAgregar={agregarProducto}
          sinStock={!!cfg.sinStock}
          onCerrar={() => setSelector(false)}
        />
      )}
    </ModeloF1>
    {recibo && <NotaVenta datos={recibo} nombre={cfg.nombre} onCerrar={() => setRecibo(null)} />}
    </>
  );

  function abrirVend() {
    if (!personal.length) api.get(`${URL}/personal`).then((r) => setPersonal(r.data)).catch((e) => toast(e.message, 'error'));
    setAyudaVend(true);
  }
}

// Celda numerica: aplica el valor en cada tecla (CellValueChanged inmediato: totales y cajas<->unidades al instante).
// Si el valor queda invalido (prop error) la celda se marca en rojo y la alerta sale al salir de la celda o con Enter;
// la pantalla bloquea el grabado con errorDetalle().
export function NumeroCelda({ valor, onCambio, etiqueta, error }) {
  const toast = useToast();
  const fmt = (v) => String(Math.round(num(v) * 100000) / 100000);
  const [txt, setTxt] = useState(fmt(valor));
  const foco = useRef(false);
  // mientras se escribe no se pisa lo tecleado ("1." / "") con el valor normalizado
  useEffect(() => { if (!foco.current) setTxt(fmt(valor)); }, [valor]); // eslint-disable-line react-hooks/exhaustive-deps
  const avisar = () => { if (error) toast(error, 'error', 6000); };
  return (
    <input className={`control control-celda num${error ? ' invalido' : ''}`} type="number" inputMode="decimal" step="any" min="0" value={txt}
      aria-label={etiqueta} aria-invalid={!!error} title={error || undefined}
      onFocus={(e) => { foco.current = true; e.target.select(); }}
      onChange={(e) => { setTxt(e.target.value); onCambio(e.target.value); }}
      onBlur={() => { foco.current = false; setTxt(fmt(valor)); avisar(); }}
      onKeyDown={(e) => { if (e.key === 'Enter') { e.preventDefault(); avisar(); } }} />
  );
}

// Primer error de las filas editadas del detalle (para bloquear Grabar/Modificar). errorFila(f) -> { campo: mensaje }
export function errorDetalle(filas, errorFila, nombre) {
  for (const f of filas) {
    if (f.eliminado || !f._editado) continue;
    const msg = Object.values(errorFila(f) || {}).find(Boolean);
    if (msg) return `${tit(nombre(f))}: ${msg}`;
  }
  return null;
}

// ---------- Buscar cliente (btnCliente -> ListarCliente) con busqueda en el servidor ----------
export function BuscarCliente({ onSeleccionar, onCancelar, url = `${URL}/clientes` }) {
  const [q, setQ] = useState('');
  const [filas, setFilas] = useState([]);
  const [pos, setPos] = useState(0);
  const [cargando, setCargando] = useState(true);
  const toast = useToast();
  useEffect(() => {
    const t = setTimeout(() => {
      setCargando(true);
      api.get(`${url}?q=${encodeURIComponent(q)}`).then((r) => { setFilas(r.data); setPos(0); }).catch((e) => toast(e.message, 'error')).finally(() => setCargando(false));
    }, 250);
    return () => clearTimeout(t);
  }, [q]); // eslint-disable-line react-hooks/exhaustive-deps
  const teclas = (e) => {
    if (e.key === 'Escape') onCancelar();
    if (e.key === 'ArrowDown') { e.preventDefault(); setPos((p) => Math.min(filas.length - 1, p + 1)); }
    if (e.key === 'ArrowUp') { e.preventDefault(); setPos((p) => Math.max(0, p - 1)); }
    if (e.key === 'Enter' && filas[pos]) { e.preventDefault(); onSeleccionar(filas[pos]); }
  };
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCancelar}>
      <div className="modal modal-lg" role="dialog" aria-modal="true" aria-label="Seleccione cliente" onMouseDown={(e) => e.stopPropagation()} onKeyDown={teclas}>
        <div className="modal-cab"><h3>SELECCIONE CLIENTE</h3><button className="btn-icono" onClick={onCancelar} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="buscar"><Icono n="search" size={16} /><input autoFocus className="control" placeholder="Nombre, dirección, teléfono o código..." value={q} onChange={(e) => setQ(e.target.value)} aria-label="Buscar cliente" /></div>
        <div className="tabla-scroll alto-ayuda">
          <table className="grid grid-tarjetas">
            <thead><tr><th className="num">ID</th><th>Nombre</th><th>Dirección</th><th>Teléfono</th></tr></thead>
            <tbody>
              {filas.map((r, i) => (
                <tr key={r.Id} className={i === pos ? 'sel' : ''} onClick={() => onSeleccionar(r)} onMouseEnter={() => setPos(i)}>
                  <td data-label="ID" className="num">{r.Id}</td><td data-label="Nombre">{tit(r.NombreCliente)}</td><td data-label="Dirección">{r.DireccionCliente}</td><td data-label="Teléfono">{r.Telefono}</td>
                </tr>
              ))}
              {!filas.length && <tr><td colSpan={4} className="vacio">{cargando ? 'Buscando...' : 'Sin resultados'}</td></tr>}
            </tbody>
          </table>
        </div>
        <div className="modal-acciones"><span className="ayuda-tip">Se muestran hasta 50 · toque un cliente para elegirlo</span><button className="btn btn-sec" onClick={onCancelar}>Cancelar</button></div>
      </div>
    </div>,
    document.body,
  );
}

// ---------- Tec_VentasDetalle: elegir productos ----------
function SelectorProductos({ url, parametros, categorias, enDetalle, onAgregar, onCerrar, sinStock = false }) {
  const toast = useToast();
  const [precioId, setPrecioId] = useState(categorias[0]?.Id ?? ''); // cbPrecios: primer item
  const [productos, setProductos] = useState([]);
  const [cargando, setCargando] = useState(false);
  const [q, setQ] = useState('');
  const [elegido, setElegido] = useState(null);
  const [agregados, setAgregados] = useState(0);

  useEffect(() => {
    if (!precioId) return;
    setCargando(true);
    const p = new URLSearchParams({ sucursal: parametros.sucursal, cliente: parametros.cliente, precio: precioId, directa: parametros.directa, vendedor: parametros.vendedor });
    api.get(`${url}/productos?${p}`).then((r) => setProductos(r.data)).catch((e) => toast(e.message, 'error')).finally(() => setCargando(false));
  }, [precioId]); // eslint-disable-line react-hooks/exhaustive-deps

  // tbProducto_TextChanged: todas las palabras deben estar en codigo + nombre + descripcion + categoria
  const filtrados = useMemo(() => {
    const palabras = palabrasDe(q);
    return productos.filter((p) => !enDetalle.has(Number(p.Id))).filter((p) => {
      return coincide(`${p.CodigoExterno ?? ''} ${p.NombreProducto ?? ''} ${p.DescripcionProducto ?? ''} ${p.NombreCategoria ?? ''}`, palabras);
    });
  }, [productos, q, enDetalle]);

  const elegir = (p) => {
    if (!sinStock && !(num(p.stock) > 0)) { toast('No Existe Stock del Producto Para Proceder con la Venta'.toUpperCase(), 'error'); return; }
    setElegido(p);
  };
  const nav = useListaTeclado(filtrados, elegir, { onCerrar, pausado: !!elegido });

  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-xl selector-productos" role="dialog" aria-modal="true" aria-label="Agregar productos" onMouseDown={(e) => e.stopPropagation()}
        >
        <div className="modal-cab">
          <h3>Agregar productos {agregados > 0 && <span className="chip-ok">{agregados} agregado{agregados > 1 ? 's' : ''}</span>}</h3>
          <button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button>
        </div>
        <div className="selector-filtros">
          <div className="buscar"><Icono n="search" size={16} /><input autoFocus ref={nav.buscarRef} className="control" placeholder="Buscar por código, nombre o categoría..." value={q} onChange={(e) => setQ(e.target.value)} aria-label="Buscar producto"
            onKeyDown={nav.teclaBuscar} /></div>
          <div className="selector-precio">
            <ComboMulti dt={categorias} valor={precioId} onChange={(v) => setPrecioId(v)} id="Id" nameId="Código" descripcionId="Descripcion" descripcion="Categoría de precio" />
          </div>
        </div>
        <div className="tabla-scroll alto-selector">
          <table className="grid grid-tarjetas tabla-productos">
            <thead><tr><th>Código</th><th>Producto</th><th>Categoría</th><th className="num">Conv.</th><th className="num">Stock unid.</th><th className="num">Stock cajas</th><th className="num">Precio caja</th></tr></thead>
            <tbody ref={nav.tbodyRef}>
              {filtrados.map((p, i) => (
                <tr key={p.Id} className={`fila-clic ${i === nav.marcada ? 'marcada' : ''} ${num(p.stock) <= 0 ? `sin-stock${sinStock ? ' sin-stock-elegible' : ''}` : ''}`} title={num(p.stock) <= 0 && sinStock ? 'Sin stock: se puede cotizar igual' : undefined} onClick={() => elegir(p)} {...nav.propsFila(i)}>
                  <td data-label="Código">{p.CodigoExterno}</td>
                  <td data-label="Producto"><b>{tit(p.NombreProducto)}</b></td>
                  <td data-label="Categoría">{tit(p.NombreCategoria)}</td>
                  <td data-label="Conv." className="num">{n2(p.Conversion)}</td>
                  <td data-label="Stock unid." className="num">{num(p.stock) <= 0 ? <span className="badge badge-mal">Sin stock</span> : n2(p.stock)}</td>
                  <td data-label="Stock cajas" className="num">{n2(p.stockCajas)}</td>
                  <td data-label="Precio caja" className="num"><b>{bs(p.PrecioVenta)}</b></td>
                </tr>
              ))}
              {!filtrados.length && <tr><td colSpan={7} className="vacio">{cargando ? 'Cargando productos...' : 'Sin productos'}</td></tr>}
            </tbody>
          </table>
        </div>
        <div className="modal-acciones"><span className="ayuda-tip"><span className="solo-teclado"><kbd>↓</kbd><kbd>↑</kbd> elegir · <kbd>Enter</kbd> cantidad · </span>{sinStock ? <>Toque un producto para indicar la cantidad · <span className="leyenda-sin-stock">en rojo</span>: sin stock (se puede cotizar)</> : 'Toque un producto para indicar la cantidad'}</span><button className="btn btn-pri" onClick={onCerrar}>Listo</button></div>
      </div>
      {elegido && (
        <CantidadCajas producto={elegido} sinLimite={sinStock} onCancelar={() => { setElegido(null); nav.volver(false); }}
          onAceptar={(cantUni) => { onAgregar(elegido, precioId, cantUni); setAgregados((a) => a + 1); setElegido(null); setQ(''); nav.volver(true); }} />
      )}
    </div>,
    document.body,
  );
}

// ---------- frm_CantidadCajas: cajas <-> unidades, no supera el stock ----------
export function CantidadCajas({ producto, onAceptar, onCancelar, sinLimite = false }) {
  const conv = num(producto.Conversion) || 1;
  const stock = num(producto.stock);
  const [cajas, setCajas] = useState('');
  const [unid, setUnid] = useState('');
  const [err, setErr] = useState('');
  const refCajas = useRef(null);
  const refUnid = useRef(null);
  // Flecha izquierda en Unidades -> Cajas; flecha derecha en Cajas -> Unidades (con todo seleccionado)
  const saltar = (destino) => (e) => {
    if (e.altKey || e.ctrlKey || e.shiftKey || e.metaKey) return;
    e.preventDefault();
    destino.current?.focus();
    destino.current?.select();
  };
  const seleccionar = (e) => e.target.select();
  const validar = () => {
    const u = num(unid);
    if (!(num(cajas) > 0) || !(u > 0)) { setErr('La Cantidad debe ser Mayor a 0'); return; }
    if (!sinLimite && u > stock + 1e-9) { setErr(`La cantidad es Superior Al Stock Disponible = ${n2(stock)}`); return; }
    onAceptar(u);
  };
  return (
    <div className="modal-fondo modal-encima" onMouseDown={(e) => { e.stopPropagation(); onCancelar(); }}>
      <div className="modal modal-sm cantidad-cajas" role="dialog" aria-modal="true" aria-label="Cantidad" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') { e.stopPropagation(); onCancelar(); } if (e.key === 'Enter') { e.preventDefault(); validar(); } }}>
        <h3>{tit(producto.NombreProducto)}</h3>
        <p className="cc-stock">Stock: <b>{n2(stock)}</b> unidades · <b>{n2(stock / conv)}</b> cajas · {n2(conv)} u/caja</p>
        <div className="cc-campos">
          <label>Cajas<input ref={refCajas} onFocus={seleccionar} onKeyDown={(e) => { if (e.key === 'ArrowRight') saltar(refUnid)(e); }}
            className="control num" type="number" inputMode="decimal" min="0" step="any" value={cajas}
            onChange={(e) => { setErr(''); setCajas(e.target.value); setUnid(e.target.value === '' ? '' : String(Math.round(num(e.target.value) * conv * 100000) / 100000)); }} /></label>
          <span className="cc-igual" aria-hidden="true">=</span>
          <label>Unidades<input ref={refUnid} autoFocus onFocus={seleccionar} onKeyDown={(e) => { if (e.key === 'ArrowLeft') saltar(refCajas)(e); }}
            className="control num" type="number" inputMode="decimal" min="0" step="any" value={unid}
            onChange={(e) => { setErr(''); setUnid(e.target.value); setCajas(e.target.value === '' ? '' : String(Math.round((num(e.target.value) / conv) * 100000) / 100000)); }} /></label>
        </div>
        {err && <p className="campo-error" role="alert">{err}</p>}
        <div className="modal-acciones">
          <button className="btn btn-sec" onClick={onCancelar}>Cancelar</button>
          <button className="btn btn-pri" onClick={validar}>Agregar</button>
        </div>
      </div>
    </div>
  );
}

// Nota de venta / proforma: hoja comun en componentes/NotaImpresa.jsx (mismo esquema que Recibo.rpt)
export { CSS_NOTA, CSS_HOJA } from '../../componentes/NotaImpresa';
function NotaVenta({ datos, nombre = 'Venta', onCerrar }) {
  const { venta, detalle, total, empresa, cliente = {}, codigos = {} } = datos;
  const credito = Number(venta.TipoVenta) === 0;
  return (
    <NotaImpresa titulo={nombre === 'Venta' ? 'Nota de Venta' : nombre} id={venta.Id} fecha={venta.FechaVenta} empresa={empresa} imagenEmpresa={datos.imagenEmpresa} onCerrar={onCerrar}
      grabada={datos.grabada ? `${nombre} #${venta.Id} ${datos.grabada === 'nueva' ? 'grabada' : 'modificada'} con éxito.` : null}
      cabecera={[
        ['Señor(es):', venta.NombreCliente], ['Nit/Ci:', cliente.Nit || 'SN'], ['Dirección:', cliente.Direccion || ''], ['Teléfono:', cliente.Telefono || ''],
        ['Tipo Venta:', credito ? 'Crédito' : 'Contado'], ['Fecha Venc. Crédito:', credito && venta.FechaVencimientoCredito ? fechaCorta(`${venta.FechaVencimientoCredito}T00:00:00Z`) : ''],
        ...(cliente.Hora ? [['Hora Registro:', cliente.Hora]] : []),
      ]}
      columnas={[
        { titulo: 'Código', num: true, valor: (f) => codigos[f.Producto] ?? f.ProductoId }, { titulo: 'Producto', valor: (f) => f.Producto },
        { titulo: 'Cantidad', num: true, valor: (f) => bs(f.CantidadUnitaria ?? f.Cantidad) }, { titulo: 'Precio', num: true, valor: (f) => bs(f.Precio) },
        { titulo: 'SubTotal', num: true, valor: (f) => bs(f.SubTotal ?? (num(f.Total) + num(f.MontoDescuento))) }, { titulo: 'Monto Desc.', num: true, valor: (f) => bs(f.MontoDescuento) },
        { titulo: 'Total', num: true, valor: (f) => bs(f.Total) },
      ]}
      filas={detalle} claveFila={(f) => f.ProductoId}
      pie={[['Vendedor:', tit(venta.Personal || '')], ['Glosa:', venta.Glosa || '']]}
      totales={[['Sub Total:', bs(datos.subtotal)], ['Descuento:', bs(venta.Descuento)], ...(num(venta.IncrementoVenta) > 0 ? [['+Incremento:', bs(venta.IncrementoVenta)]] : [])]}
      etiquetaTotal="Total Venta: Bs" total={total} />
  );
}
