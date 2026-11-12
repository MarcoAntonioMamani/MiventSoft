// =====================================================================
//  KAILY LA PAZ (punto de venta): Tec_Ventas.vb -> Gestion De Ventas   (SP: MAM_Ventas)
//  + Tec_VentasDetalle.vb (productos y kits) + EfectivoCobranza.vb (cobro en contado)
//
//  Igual que el escritorio:
//   * Nueva venta solo si hay Caja Chica abierta. Vendedor = personal del usuario; cliente por defecto Id 1.
//   * Cantidad en unidades; precio editable solo con "Modificar precio"; descuentos solo con "Aplicar descuento".
//     El total se puede escribir y el descuento sale de la diferencia (tbTotal_ValueChanged).
//   * Un kit se agrega como sus productos (no editables) y se quita completo.
//   * Contado: al grabar se pide el cobro (Bs, $, tarjeta, transferencia) y debe cubrir el total.
//  El navegador calcula para MOSTRAR; el servidor vuelve a tomar precios, costos y stock de los SP.
// =====================================================================
import { useDeferredValue, useEffect, useMemo, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import ModeloF1, { useModeloF1 } from '../../componentes/ModeloF1';
import { Campo, ComboMulti, SwitchButton, fechaCorta } from '../../componentes/Controles';
import { useToast } from '../../componentes/Toast';
import { Celda } from '../../global/Celda';
import { titulo as tit } from '../../global/programas';
import { api } from '../../api/http';
import Icono from '../../componentes/Icono';
import NotaImpresa from '../../componentes/NotaImpresa';
import { useListaTeclado, FilaMas, AYUDA_ELEGIR } from '../../componentes/useListaTeclado';
import { palabrasDe, indexar, filtrarIndice } from '../../global/buscar';
import { NumeroCelda, BuscarCliente, errorDetalle } from './Tec_Ventas';
import FormularioCliente from './FormularioCliente';
import { useAlActivar } from '../../global/vistaActiva';
import { useTipoCambio, aDolares } from '../../global/tipoCambio';

// Tec_Ventas.vb y Tec_Proformas.vb de Kaily comparten la pantalla
export const CFG_VENTA_PV = { url: '/api/ventas/ventas', titulo: 'Ventas', subtitulo: 'Gestion De Ventas', icono: 'cart', nombre: 'Venta', cobro: true, caja: true, tipoVenta: true, facturado: true, sinStock: false, nota: 'Nota de Venta' };
export const CFG_PROFORMA_PV = { url: '/api/ventas/proformas', titulo: 'Proformas', subtitulo: 'Gestion De Proformas', icono: 'list', nombre: 'Proforma', cobro: false, caja: false, tipoVenta: false, facturado: false, sinStock: true, nota: 'Proforma' };
const dos = (n) => String(n).padStart(2, '0');
const hoyLocal = () => { const d = new Date(); return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`; };
const bs = (n, d = 2) => Number(n || 0).toLocaleString('es-BO', { minimumFractionDigits: d, maximumFractionDigits: d });
const n2 = (n) => Number(n || 0).toLocaleString('es-BO', { maximumFractionDigits: 2 });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const r2 = (x) => Math.round(num(x) * 100) / 100;

const VACIO = {
  Id: '', SucursalId: '', FechaVenta: '', PersonalId: 0, Personal: '', ClienteId: 0, NombreCliente: '', TipoVenta: true,
  FechaVencimientoCredito: '', Glosa: '', Descuento: 0, Facturado: false,
  MontoBs: 0, MontoDolares: 0, TarjetaBancaria: 0, TransferenciaBancaria: 0, TipoCambio: 0,
};

// P_PonerTotal / CellValueChanged: SubTotal = Cantidad * Precio; el descuento por % o por monto
function recalcular(f) {
  const sub = num(f.Cantidad) * num(f.Precio);
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

function errorFila(f, sinStock) {
  const e = {};
  const cant = num(f.Cantidad);
  if (!(cant > 0)) e.Cantidad = 'La Cantidad debe ser Mayor a 0';
  else if (!sinStock && Number(f.Tipo) !== 2 && cant > num(f.stock) + 1e-9) e.Cantidad = `La Cantidad = ${n2(cant)} es mayor al Stock del Producto = ${n2(f.stock)}`;
  if (num(f.Precio) < 0) e.Precio = 'Precio no valido';
  const sub = cant * num(f.Precio);
  if (f._origenDesc === 'monto') {
    if (num(f.MontoDescuento) < 0 || num(f.MontoDescuento) > sub + 1e-9) e.Desc = `El Monto de Descuento no puede ser mayor al SubTotal = ${n2(sub)}`;
  } else if (num(f.ProcentajeDescuento) < 0 || num(f.ProcentajeDescuento) > 100) e.Desc = 'El Porcentaje de Descuento debe estar entre 0 y 100';
  return e;
}

export default function Tec_VentasPuntoVenta() { return <PantallaPV cfg={CFG_VENTA_PV} />; }

export function PantallaPV({ cfg }) {
  const URL = cfg.url; // eslint-disable-line no-shadow
  const toast = useToast();
  const [form, setForm] = useState(VACIO);
  const [detalle, setDetalle] = useState([]);
  const [cargandoDet, setCargandoDet] = useState(false);
  const [errores, setErrores] = useState({});
  const [ini, setIni] = useState(null);
  // tipo de cambio vigente (ultimo guardado en la BD): total en $us y cobro en dolares. Se relee al volver a la pestaña
  const [tc, leerTc] = useTipoCambio();
  useAlActivar(leerTc, { alMontar: false });
  const [filtro, setFiltro] = useState({ desde: hoyLocal(), hasta: hoyLocal() });
  const [ayudaCli, setAyudaCli] = useState(false);
  const [nuevoCli, setNuevoCli] = useState(false); // Kaily FormularioCliente: alta rapida
  const [selector, setSelector] = useState(false);
  const [categoria, setCategoria] = useState(''); // cbPrecios (se conserva entre aperturas, como el escritorio)
  const [cobro, setCobro] = useState(null); // { total, resolver }
  const [recibo, setRecibo] = useState(null);
  const pedidoDet = useRef(0);

  const permisos = ini?.permisos || {};
  const hoy = ini?.hoy || hoyLocal();
  const quitarError = (k) => setErrores((e) => (e[k] ? { ...e, [k]: undefined } : e));
  const set = (k) => (v) => { quitarError(k); setForm((f) => ({ ...f, [k]: v?.target ? v.target.value : v })); };

  useEffect(() => {
    api.get(`${URL}/inicio`).then((r) => { setIni(r.data); setCategoria(r.data.categoriaDefecto); }).catch((e) => toast(e.message, 'error'));
  }, []); // eslint-disable-line react-hooks/exhaustive-deps
  // al volver a la pestaña se recargan los combos, pero NO con una venta a medio registrar/editar (cambiar ini la limpiaria)
  useAlActivar(() => {
    if (m.ruta.tipo === 'nuevo' || m.ruta.tipo === 'editar') return;
    api.get(`${URL}/inicio`).then((r) => setIni(r.data)).catch((e) => toast(e.message, 'error'));
  }, { alMontar: false });

  const vigentes = detalle.filter((f) => !f.eliminado);
  const errDe = (f) => (f._editado ? errorFila(f, cfg.sinStock) : {});
  const suma = vigentes.reduce((s, f) => s + num(f.Total), 0);
  const total = suma - num(form.Descuento);
  const sucursalFija = ini && Number(ini.sucursalUsuario) >= 0;

  // _PMOLimpiar + btnNuevo_Click
  const limpiar = (d = ini) => {
    setDetalle([]);
    const fija = d && Number(d.sucursalUsuario) >= 0 && d.sucursales.some((s) => Number(s.aanumi) === Number(d.sucursalUsuario));
    setForm({
      ...VACIO,
      SucursalId: fija ? d.sucursalUsuario : (d?.sucursales?.[0]?.aanumi ?? ''),
      FechaVenta: d?.hoy || hoyLocal(), FechaVencimientoCredito: d?.hoy || hoyLocal(),
      PersonalId: d?.vendedorDefecto?.Id ?? 0, Personal: d?.vendedorDefecto?.Nombre ?? '',
      ClienteId: d?.clienteDefecto?.Id ?? 0, NombreCliente: d?.clienteDefecto?.NombreCliente ?? '',
    });
    if (d) setCategoria(d.categoriaDefecto);
  };

  const mostrar = (f) => {
    setForm({
      Id: f.Id, SucursalId: f.SucursalId ?? '', FechaVenta: f.FechaVenta ?? '', PersonalId: f.PersonalId ?? 0, Personal: f.Personal ?? '',
      ClienteId: f.ClienteId ?? 0, NombreCliente: f.NombreCliente ?? '', TipoVenta: Number(f.TipoVenta) === 1,
      FechaVencimientoCredito: f.FechaVencimientoCredito ?? f.FechaVenta ?? '', Glosa: f.Glosa ?? '', Descuento: num(f.Descuento), Facturado: !!Number(f.Facturado),
      MontoBs: num(f.MontoBs), MontoDolares: num(f.MontoDolares), TarjetaBancaria: num(f.TarjetaBancaria), TransferenciaBancaria: num(f.TransferenciaBancaria), TipoCambio: num(f.TipoCambio),
    });
    const n = ++pedidoDet.current;
    setDetalle([]);
    setCargandoDet(true);
    api.get(`${URL}/${f.Id}/detalle`).then((r) => {
      if (n !== pedidoDet.current) return;
      setDetalle(r.data.map((d) => ({ ...d, key: `e${d.Id}`, nuevo: false, eliminado: false })));
    }).catch((e) => toast(e.message, 'error')).finally(() => n === pedidoDet.current && setCargandoDet(false));
  };

  // Lo que se envia: filas existentes por Id; productos nuevos; cada kit nuevo una sola vez
  const _datos = (pago) => {
    const kits = new Set();
    const filas = [];
    for (const f of detalle) {
      if (!f.nuevo) {
        filas.push({ Id: f.Id, eliminado: !!f.eliminado, Cantidad: f.Cantidad, ProcentajeDescuento: f.ProcentajeDescuento, MontoDescuento: f.MontoDescuento,
          ...(permisos.modificarPrecio ? { Precio: f.Precio } : {}) });
      } else if (!f.eliminado && Number(f.Tipo) === 2) {
        if (!kits.has(f.KitId)) { kits.add(f.KitId); filas.push({ nuevo: true, Tipo: 2, KitId: f.KitId, CantidadKit: f.CantidadKit, PrecioId: f.PrecioId }); }
      } else if (!f.eliminado) {
        filas.push({ nuevo: true, Tipo: 1, ProductoId: f.ProductoId, PrecioId: f.PrecioId, Cantidad: f.Cantidad, ProcentajeDescuento: f.ProcentajeDescuento,
          MontoDescuento: f.MontoDescuento, ...(permisos.modificarPrecio ? { Precio: f.Precio } : {}) });
      }
    }
    return {
      fechaOriginal: m.filaActual?.FechaVenta,
      SucursalId: form.SucursalId, FechaVenta: form.FechaVenta, ClienteId: form.ClienteId, TipoVenta: form.TipoVenta ? 1 : 0,
      FechaVencimientoCredito: form.FechaVencimientoCredito, Glosa: form.Glosa, Descuento: r2(form.Descuento), Facturado: form.Facturado ? 1 : 0,
      PrecioId: categoria, detalle: filas, pago,
    };
  };

  // Efecto tipo 20 (EfectivoCobranza): solo en contado. Cancelar = no se graba
  const conCobro = cfg.cobro && form.TipoVenta;
  const pedirCobro = () => (conCobro
    ? new Promise((resolver) => setCobro({ total: r2(total), resolver }))
    : Promise.resolve(null));
  const grabar = async (enviar) => {
    const pago = await pedirCobro();
    if (conCobro && !pago) throw new Error('Cobro cancelado: la venta no se grabo.');
    return enviar(_datos(pago));
  };

  // btnModificar / btnEliminar / menu de la fila
  const bloqueo = (accion) => (f) => {
    if (!f) return null;
    if (!permisos.modificarAnteriores && f.FechaVenta < hoy) return `No puede ${accion} Ventas Anteriores a la fecha de Hoy, Consulte con su Administrador`.toUpperCase();
    if (num(f.CierreModulo) > 0) return `No Es Posible ${accion} La Venta Ya que Pertenece A un cierre De Caja Cerrado # ${num(f.CierreModulo)}`;
    return null;
  };

  const m = useModeloF1({
    _PMOGetTablaBuscador: async () => {
      const q = new URLSearchParams({ desde: filtro.desde, hasta: filtro.hasta });
      return (await api.get(`${URL}?${q}`)).data.map((f) => ({ ...f, Fecha: fechaCorta(`${f.FechaVenta}T00:00:00Z`) }));
    },
    _PMOGetListEstructuraBuscador: [
      new Celda('Id', true, 'ID', 70, '', 'num'),
      new Celda('Fecha', true, 'Fecha Venta', 100),
      new Celda('HoraRegistro', true, 'Hora', 70),
      new Celda('Personal', true, 'Personal', 150),
      new Celda('NombreCliente', true, 'Cliente', 190),
      new Celda('TVenta', true, 'Tipo Venta', 90),
      new Celda('Glosa', true, 'Glosa', 160),
      new Celda('NombreAlmacen', true, 'Sucursal', 120),
      new Celda('TotalVenta', true, 'Total Venta', 110, '0.00', 'num'),
      new Celda('TDespachado', true, 'Estado Despacho', 130),
      new Celda('Despachante', true, 'Despachante', 140),
    ],
    _PMOLimpiar: () => limpiar(),
    _PMOMostrarRegistro: mostrar,
    _PMOLimpiarErrores: () => setErrores({}),
    _PMOBloqueoModificar: bloqueo('Modificar'),
    _PMOBloqueoEliminar: bloqueo('Eliminar'),
    _PMOValidarCampos: () => {
      const e = {};
      if (!form.SucursalId) e.SucursalId = 'Seleccione una Sucursal';
      if (!(form.PersonalId > 0)) e.Personal = 'Su usuario no tiene un personal asignado';
      if (!(form.ClienteId > 0)) e.Cliente = 'Seleccione un Cliente';
      if (!form.FechaVenta) e.FechaVenta = 'Ingrese una Fecha Valida';
      if (!vigentes.length) e.detalle = 'Por Favor Inserte un Detalle';
      const ed = errorDetalle(vigentes, errDe, (f) => f.Producto);
      if (ed) { e.detalle = ed; toast(ed, 'error', 6000); }
      if (num(form.Descuento) > suma + 1e-9 || num(form.Descuento) < 0) e.Descuento = 'El total Ingresado es Superior al monto total de la venta';
      setErrores(e);
      return !Object.keys(e).length;
    },
    _PMOGrabarRegistro: () => grabar(async (datos) => {
      const r = await api.post(URL, datos);
      if (r.fecha && (r.fecha < filtro.desde || r.fecha > filtro.hasta)) setFiltro((x) => ({ desde: r.fecha < x.desde ? r.fecha : x.desde, hasta: r.fecha > x.hasta ? r.fecha : x.hasta }));
      verNota(r.id, r.fecha || form.FechaVenta, 'nueva'); // ReporteVenta: "¿Desea Generar el Reporte de la Venta?"
      return r;
    }),
    _PMOModificarRegistro: () => grabar(async (datos) => {
      const r = await api.put(`${URL}/${form.Id}`, datos);
      verNota(r.id || form.Id, r.fecha || form.FechaVenta, 'modificada');
      return r;
    }),
    _PMOEliminarRegistro: (f) => api.del(`${URL}/${f.Id}?fecha=${encodeURIComponent(f.FechaVenta)}`),
    _PMODescripcionEliminar: (f) => `¿Esta Seguro de Eliminar la ${cfg.nombre} ${f.Id} ?`,
  });
  const ro = !m.habilitado;

  const primera = useRef(true);
  useEffect(() => { if (primera.current) { primera.current = false; return; } m._PMFiltrar(); }, [filtro.desde, filtro.hasta]); // eslint-disable-line react-hooks/exhaustive-deps
  useEffect(() => { if (m.ruta.tipo === 'nuevo' && ini) limpiar(ini); }, [m.ruta.tipo, ini]); // eslint-disable-line react-hooks/exhaustive-deps

  // ButtonX1: no se vende sin Caja Chica abierta
  const nuevaVenta = async () => {
    if (!cfg.caja) { m._PMNuevo(); return; }
    try {
      const r = await api.get(`${URL}/caja-abierta`);
      if (!r.data.abierta) { toast('No Es Posible Hacer La Venta Por que no Existe Caja Chica con Estado Abierta', 'error', 8000); return; }
      m._PMNuevo();
    } catch (e) { toast(e.message, 'error'); }
  };
  const menuNuevo = m.puede.insertar ? [{ texto: `Nueva ${cfg.nombre}`, icono: cfg.icono, accion: nuevaVenta }] : [];

  // ---------- detalle ----------
  const cambiarFila = (key, cambio) => {
    setDetalle((d) => d.map((f) => {
      if (f.key !== key) return f;
      const nf = { ...f, ...cambio, _editado: true };
      if ('Cantidad' in cambio || 'Precio' in cambio) nf._origenDesc = 'porc'; // se conserva el % (CellValueChanged)
      return recalcular(nf);
    }));
    quitarError('detalle');
  };
  // _prEliminarFila: un kit se quita completo
  const quitarFila = (f) => setDetalle((d) => d.map((x) => {
    const mismo = Number(f.Tipo) === 2 ? Number(x.Tipo) === 2 && Number(x.KitId) === Number(f.KitId) && x.nuevo === f.nuevo : x.key === f.key;
    return mismo ? { ...x, eliminado: true } : x;
  }));

  const cambiarSucursal = (v) => {
    quitarError('SucursalId');
    const hayNuevos = detalle.some((f) => f.nuevo && !f.eliminado);
    setForm((x) => ({ ...x, SucursalId: v }));
    if (hayNuevos) {
      setDetalle((d) => d.filter((f) => !f.nuevo));
      toast('Se quitaron los productos agregados: vuelva a agregarlos (el stock depende de la sucursal).', 'aviso', 7000);
    }
  };

  const abrirSelector = () => {
    if (!(form.ClienteId > 0)) { toast('Seleccione Cliente'.toUpperCase(), 'error'); return; }
    if (!form.SucursalId) { toast('Seleccion Sucursal'.toUpperCase(), 'error'); return; }
    setSelector(true);
  };

  const enDetalle = useMemo(() => ({
    productos: new Set(vigentes.filter((f) => Number(f.Tipo) !== 2).map((f) => Number(f.ProductoId))),
    kits: new Set(vigentes.filter((f) => Number(f.Tipo) === 2).map((f) => Number(f.KitId))),
  }), [detalle]); // eslint-disable-line react-hooks/exhaustive-deps

  const agregar = async (p, precioId, cant) => {
    const base = { Id: null, nuevo: true, eliminado: false, PrecioId: precioId, ProcentajeDescuento: 0, MontoDescuento: 0 };
    if (Number(p.Tipo) === 2) {
      try {
        const comp = (await api.get(`${URL}/kits/${p.Id}`)).data;
        if (!comp.length) { toast(`El kit ${p.NombreProducto} no tiene productos`, 'error'); return false; }
        const filas = comp.map((c, i) => recalcular({
          ...base, key: `k${Date.now()}${p.Id}_${i}`, Tipo: 2, TipoNombre: 'Kits', KitId: p.Id, KitNombre: p.NombreProducto, CantidadKit: cant,
          ProductoId: c.ProductoId, Producto: c.NombreProducto, Cantidad: num(c.Cantidad) * cant, Precio: num(c.PrecioVenta), stock: num(c.Cantidad),
        }));
        setDetalle((d) => [...d, ...filas]);
      } catch (e) { toast(e.message, 'error'); return false; }
    } else {
      setDetalle((d) => [...d, recalcular({
        ...base, key: `n${Date.now()}${p.Id}`, Tipo: 1, TipoNombre: 'Productos', KitId: 0, KitNombre: '', CantidadKit: 0,
        ProductoId: p.Id, Producto: p.NombreProducto, Cantidad: cant, Precio: num(p.PrecioVenta), stock: num(p.stock),
      })]);
    }
    quitarError('detalle');
    return true;
  };

  // descuento general: tbPdesc / tbMdesc (con permiso) y tbTotal (siempre)
  const setDescMonto = (v) => {
    quitarError('Descuento');
    const mo = num(v);
    if (mo < 0 || mo > suma) { setForm((f) => ({ ...f, Descuento: 0 })); if (mo > suma) toast('El descuento no puede ser mayor al total', 'error'); return; }
    setForm((f) => ({ ...f, Descuento: v }));
  };
  const setDescPorc = (v) => {
    const p = num(v);
    setDescMonto(p <= 0 || p > 100 ? 0 : r2(suma * (p / 100)));
  };
  const setTotal = (v) => {
    quitarError('Descuento');
    const t = num(v);
    if (t > suma + 1e-9 || t < 0) { setForm((f) => ({ ...f, Descuento: 0 })); toast('El total Ingresado es Superior al monto total de la venta', 'error'); return; }
    setForm((f) => ({ ...f, Descuento: r2(suma - t) }));
  };
  const porcDesc = suma ? (num(form.Descuento) * 100) / suma : 0;

  // tab_Cobro (al ver una venta): lo pagado y el cambio
  const pagado = form.MontoBs + form.MontoDolares * form.TipoCambio + form.TarjetaBancaria + form.TransferenciaBancaria;

  const verNota = async (id, fecha, grabada = false) => {
    if (!id || !fecha) return;
    try {
      const r = await api.get(`${URL}/${id}/recibo?fecha=${encodeURIComponent(fecha)}`);
      setRecibo({ ...r.data, grabada });
    } catch (e) { toast(e.message, 'error'); }
  };
  const imprimir = () => m.filaActual && verNota(m.filaActual.Id, m.filaActual.FechaVenta);

  return (
    <>
    <ModeloF1 m={m} titulo={cfg.titulo} subtitulo={cfg.subtitulo} icono={cfg.icono} menuNuevo={menuNuevo}
      accionesFila={(f) => [{ texto: cfg.nota, icono: 'printer', onClick: () => verNota(f.Id, f.FechaVenta) }]}
      accionesRegistro={<button className="btn btn-sec" onClick={imprimir} disabled={!m.filaActual}><Icono n="printer" size={16} /> <span className="txt-btn">{cfg.nota}</span></button>}
      extraBuscador={(
        <div className="filtros-lista">
          <label className="filtro"><span>Desde</span><input type="date" className="control" value={filtro.desde} max={filtro.hasta} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, desde: e.target.value }))} /></label>
          <label className="filtro"><span>Hasta</span><input type="date" className="control" value={filtro.hasta} min={filtro.desde} onChange={(e) => e.target.value && setFiltro((x) => ({ ...x, hasta: e.target.value }))} /></label>
          {tc !== null && <span className="ayuda-tip">Tipo de cambio: <b>{n2(tc)}</b></span>}
        </div>
      )}>
      <div className="venta-grid">
        <div className="tarjeta">
          <h2 className="tarjeta-tit">Datos de la {cfg.nombre.toLowerCase()}</h2>
          <div className="campos">
            <Campo etiqueta="Código"><input className="control" value={form.Id} readOnly placeholder="Automático" /></Campo>
            <Campo etiqueta="Sucursal" error={errores.SucursalId} requerido>
              <ComboMulti dt={ini?.sucursales || []} valor={form.SucursalId} onChange={cambiarSucursal} id="aanumi" nameId="Código" descripcionId="aabdes" descripcion="Sucursal" deshabilitado={ro || sucursalFija} error={errores.SucursalId} />
            </Campo>
            <Campo etiqueta="Fecha venta" error={errores.FechaVenta} requerido>
              <input type="date" className="control" value={form.FechaVenta} onChange={set('FechaVenta')} readOnly={ro} disabled={ro} />
            </Campo>
            <Campo etiqueta="Vendedor" ancho={2} error={errores.Personal}><input className="control" value={tit(form.Personal)} readOnly /></Campo>
            <Campo etiqueta="Cliente" ancho={2} error={errores.Cliente} requerido>
              <div className="control-con-btn">
                <input className={`control ${!ro ? 'control-elegir' : ''}`} value={tit(form.NombreCliente)} readOnly placeholder="Toque para buscar el cliente..."
                  onClick={() => !ro && setAyudaCli(true)} onKeyDown={(e) => !ro && (e.key === 'Enter' || e.key === ' ') && (e.preventDefault(), setAyudaCli(true))} />
                {!ro && <button type="button" className="btn btn-sec" onClick={() => setAyudaCli(true)}><Icono n="search" size={15} /> Buscar</button>}
              </div>
            </Campo>
            {cfg.tipoVenta && (
              <Campo etiqueta="Tipo de venta">
                <SwitchButton valor={form.TipoVenta} onChange={set('TipoVenta')} deshabilitado={ro} textoOn="CONTADO" textoOff="CREDITO" />
              </Campo>
            )}
            {cfg.tipoVenta && !form.TipoVenta && (
              <Campo etiqueta="Venc. crédito">
                <input type="date" className="control" value={form.FechaVencimientoCredito} min={form.FechaVenta} onChange={set('FechaVencimientoCredito')} readOnly={ro} disabled={ro} />
              </Campo>
            )}
            {cfg.facturado && <Campo etiqueta="Facturado"><SwitchButton valor={form.Facturado} onChange={set('Facturado')} deshabilitado={ro} textoOn="SI" textoOff="NO" /></Campo>}
            <Campo etiqueta="Glosa" ancho={2}><input className="control" value={form.Glosa} onChange={set('Glosa')} readOnly={ro} maxLength={500} /></Campo>
          </div>
          {cfg.cobro && ro && form.Id && form.TipoVenta && (
            <>
              <div className="sep" />
              <h2 className="tarjeta-tit">Cobro</h2>
              <div className="campos">
                <Campo etiqueta="Efectivo Bs"><input className="control num" value={bs(form.MontoBs)} readOnly /></Campo>
                <Campo etiqueta="Dólares"><input className="control num" value={bs(form.MontoDolares)} readOnly /></Campo>
                <Campo etiqueta="Tarjeta"><input className="control num" value={bs(form.TarjetaBancaria)} readOnly /></Campo>
                <Campo etiqueta="Transferencia"><input className="control num" value={bs(form.TransferenciaBancaria)} readOnly /></Campo>
                <Campo etiqueta={`Total pagado (TC ${n2(form.TipoCambio)})`}><input className="control num" value={bs(pagado)} readOnly /></Campo>
                <Campo etiqueta="Cambio"><input className="control num" value={bs(pagado - total)} readOnly /></Campo>
              </div>
            </>
          )}
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
                    <th>Producto</th><th>Kit</th><th className="num">Cantidad</th><th className="num">Precio</th>
                    <th className="num">SubTotal</th><th className="num">% Desc.</th><th className="num">M. Desc.</th><th className="num">Total</th>{!ro && <th aria-label="Quitar" />}
                  </tr>
                </thead>
                <tbody>
                  {vigentes.map((f) => {
                    const esKit = Number(f.Tipo) === 2;
                    const editable = !ro && !esKit;
                    return (
                      <tr key={f.key} className={f.nuevo ? 'fila-nueva' : ''}>
                        <td data-label="Producto" className="det-producto"><b>{tit(f.Producto)}</b><small>Cód. {f.ProductoId}{!esKit ? ` · stock ${n2(f.stock)}` : ''}</small></td>
                        <td data-label="Kit">{esKit ? `${tit(f.KitNombre)} × ${n2(f.CantidadKit)}` : '—'}</td>
                        <td data-label="Cantidad" className="num">{editable ? <NumeroCelda valor={f.Cantidad} onCambio={(x) => cambiarFila(f.key, { Cantidad: x })} etiqueta={`Cantidad de ${f.Producto}`} error={errDe(f).Cantidad} /> : n2(f.Cantidad)}</td>
                        <td data-label="Precio" className="num">{editable && permisos.modificarPrecio ? <NumeroCelda valor={f.Precio} onCambio={(x) => cambiarFila(f.key, { Precio: x })} etiqueta={`Precio de ${f.Producto}`} error={errDe(f).Precio} /> : bs(f.Precio)}</td>
                        <td data-label="SubTotal" className="num">{bs(f.SubTotal)}</td>
                        <td data-label="% Desc." className="num">{editable && permisos.modificarDescuento ? <NumeroCelda valor={r2(f.ProcentajeDescuento)} onCambio={(x) => cambiarFila(f.key, { ProcentajeDescuento: num(x), _origenDesc: 'porc' })} etiqueta="Porcentaje de descuento" error={f._origenDesc !== 'monto' ? errDe(f).Desc : undefined} /> : n2(f.ProcentajeDescuento)}</td>
                        <td data-label="M. Desc." className="num">{editable && permisos.modificarDescuento ? <NumeroCelda valor={r2(f.MontoDescuento)} onCambio={(x) => cambiarFila(f.key, { MontoDescuento: num(x), _origenDesc: 'monto' })} etiqueta="Monto de descuento" error={f._origenDesc === 'monto' ? errDe(f).Desc : undefined} /> : bs(f.MontoDescuento)}</td>
                        <td data-label="Total" className="num det-total"><b>{bs(f.Total)}</b></td>
                        {!ro && <td className="col-acc"><button className="btn-icono peligro" onClick={() => quitarFila(f)} aria-label={`Quitar ${esKit ? f.KitNombre : f.Producto}`} title={esKit ? 'Quitar el kit completo' : 'Quitar'}><Icono n="trash" size={16} /></button></td>}
                      </tr>
                    );
                  })}
                  {!vigentes.length && <tr><td colSpan={9} className="vacio">{ro ? 'Sin productos' : 'Agregue productos con el botón "Agregar productos"'}</td></tr>}
                </tbody>
              </table>
            </div>
          )}
          <div className="venta-totales">
            <div><span>SubTotal</span><b>{bs(suma)}</b></div>
            <div className="venta-desc">
              <span>Descuento</span>
              {ro || !permisos.modificarDescuento ? <b>{bs(form.Descuento)} <small>({n2(porcDesc)}%)</small></b> : (
                <span className="desc-inputs">
                  <label><input className="control num" type="number" min="0" max="100" step="0.01" value={r2(porcDesc) || ''} placeholder="0" onChange={(e) => setDescPorc(e.target.value)} aria-label="Porcentaje descuento" />%</label>
                  <label>Bs<input className="control num" type="number" min="0" step="0.01" value={form.Descuento || ''} placeholder="0" onChange={(e) => setDescMonto(e.target.value)} aria-label="Monto descuento" /></label>
                </span>
              )}
            </div>
            {errores.Descuento && <p className="campo-error">{errores.Descuento}</p>}
            <div className="venta-total">
              <span>Total {cfg.nombre.toLowerCase()}</span>
              {ro ? <b>Bs {bs(total)}</b> : (
                <label className="desc-inputs">Bs<input className="control num" type="number" min="0" step="0.01" value={r2(total)} onChange={(e) => setTotal(e.target.value)} aria-label="Total de la venta" title="Si escribe un total menor, la diferencia se aplica como descuento" /></label>
              )}
            </div>
            {/* total en dolares con el ultimo tipo de cambio guardado (tabla TipoCambio) */}
            <div className="venta-total-dolar">
              <span>Total en dólares <small>(TC {tc === null ? '…' : n2(tc)})</small></span>
              {tc === null ? <b>…</b> : tc > 0 ? <b>$us {bs(aDolares(total, tc))}</b> : <b className="neg" title="Actualice el tipo de cambio con el botón de la cabecera">Sin tipo de cambio</b>}
            </div>
          </div>
        </div>
      </div>

      {ayudaCli && (
        <BuscarCliente url={`${URL}/clientes`} onCancelar={() => setAyudaCli(false)}
          onSeleccionar={(r) => { quitarError('Cliente'); setAyudaCli(false); setForm((f) => ({ ...f, ClienteId: r.Id, NombreCliente: r.NombreCliente })); }}
          pie={<button type="button" className="btn btn-sec" onClick={() => setNuevoCli(true)}><Icono n="user" size={15} /> Nuevo cliente</button>} />
      )}
      {nuevoCli && (
        <FormularioCliente url={`${URL}/clientes`} onCerrar={() => setNuevoCli(false)}
          onCreado={(r) => { quitarError('Cliente'); setNuevoCli(false); setAyudaCli(false); setForm((f) => ({ ...f, ClienteId: r.Id, NombreCliente: r.NombreCliente })); }} />
      )}
      {selector && (
        <SelectorProductos url={URL} sinStock={!!cfg.sinStock} sucursal={form.SucursalId} categorias={ini?.categoriasPrecio || []} categoria={categoria} onCategoria={setCategoria}
          enDetalle={enDetalle} onAgregar={agregar} onCerrar={() => setSelector(false)} />
      )}
      {cobro && (
        <CobroVenta total={cobro.total} tipoCambio={num(tc)}
          onAceptar={(p) => { cobro.resolver(p); setCobro(null); }} onCancelar={() => { cobro.resolver(null); setCobro(null); }} />
      )}
    </ModeloF1>
    {recibo && <NotaVenta datos={recibo} cfg={cfg} onCerrar={() => setRecibo(null)} />}
    </>
  );
}

// ---------- Tec_VentasDetalle: productos (Tipo 1) y kits (Tipo 2) ----------
function SelectorProductos({ url, sinStock, sucursal, categorias, categoria, onCategoria, enDetalle, onAgregar, onCerrar }) {
  const toast = useToast();
  const [productos, setProductos] = useState([]);
  const [cargando, setCargando] = useState(false);
  const [q, setQ] = useState('');
  const [elegido, setElegido] = useState(null);
  const [agregados, setAgregados] = useState(0);

  // stock siempre de la BD (sin cache): se pide cada vez que se abre o se cambia la categoria;
  // si se cambia de categoria antes de que llegue la respuesta anterior, esa se descarta
  useEffect(() => {
    if (!categoria) return undefined;
    let vigente = true;
    setCargando(true);
    setProductos([]);
    api.get(`${url}/productos?${new URLSearchParams({ sucursal, precio: categoria })}`).then((r) => { if (vigente) setProductos(r.data); })
      .catch((e) => { if (vigente) toast(e.message, 'error'); }).finally(() => { if (vigente) setCargando(false); });
    return () => { vigente = false; };
  }, [categoria]); // eslint-disable-line react-hooks/exhaustive-deps

  // tbProducto_TextChanged: todas las palabras en codigo + nombre + descripcion + categoria + industria
  // (texto normalizado una sola vez por carga; la lista se filtra sin trabar lo que se escribe)
  const indice = useMemo(() => indexar(productos, (p) => `${p.CodigoExterno ?? ''} ${p.NombreProducto ?? ''} ${p.DescripcionProducto ?? ''} ${p.NombreCategoria ?? ''} ${p.industria ?? ''}`), [productos]);
  const qBusca = useDeferredValue(q);
  const filtrados = useMemo(() => filtrarIndice(indice, palabrasDe(qBusca))
    .filter((p) => (Number(p.Tipo) === 2 ? !enDetalle.kits.has(Number(p.Id)) : !enDetalle.productos.has(Number(p.Id)))), [indice, qBusca, enDetalle]);

  const elegir = (p) => {
    if (!sinStock && !(num(p.stock) > 0)) { toast(`El Producto: ${p.NombreProducto} NO CUENTA CON STOCK DISPONIBLE`.toUpperCase(), 'error'); return; }
    setElegido(p);
  };
  const nav = useListaTeclado(filtrados, elegir, { onCerrar, pausado: !!elegido });

  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal modal-xl selector-productos" role="dialog" aria-modal="true" aria-label="Agregar productos" onMouseDown={(e) => e.stopPropagation()}>
        <div className="modal-cab">
          <h3>Agregar productos {agregados > 0 && <span className="chip-ok">{agregados} agregado{agregados > 1 ? 's' : ''}</span>}</h3>
          <button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button>
        </div>
        <div className="selector-filtros">
          <div className="buscar"><Icono n="search" size={16} /><input autoFocus ref={nav.buscarRef} className="control" placeholder="Buscar por código, descripción, categoría o industria..." value={q} onChange={(e) => setQ(e.target.value)} aria-label="Buscar producto" onKeyDown={nav.teclaBuscar} /></div>
          <div className="selector-precio">
            <ComboMulti dt={categorias} valor={categoria} onChange={onCategoria} id="Id" nameId="Código" descripcionId="Descripcion" descripcion="Categoría de precio" />
          </div>
        </div>
        <div className="tabla-scroll alto-selector">
          <table className="grid grid-tarjetas tabla-productos">
            <thead><tr><th>Código</th><th>Descripción</th><th>Tipo</th><th>Categoría</th><th>Industria</th><th className="num">Stock</th><th className="num">Precio</th></tr></thead>
            <tbody ref={nav.tbodyRef}>
              {nav.visibles.map((p, i) => (
                <tr key={`${p.Tipo}-${p.Id}`} className={`fila-clic ${i === nav.marcada ? 'marcada' : ''} ${num(p.stock) <= 0 ? 'sin-stock-elegible' : ''}`} {...nav.propsFila(i)}>
                  <td data-label="Código">{p.CodigoExterno}</td>
                  <td data-label="Descripción"><b>{tit(p.DescripcionProducto || p.NombreProducto)}</b></td>
                  <td data-label="Tipo">{Number(p.Tipo) === 2 ? <span className="badge">Kit</span> : tit(p.NombreTipo || 'Producto')}</td>
                  <td data-label="Categoría">{tit(p.NombreCategoria)}</td>
                  <td data-label="Industria">{tit(p.industria)}</td>
                  <td data-label="Stock" className="num">{num(p.stock) <= 0 ? <span className="badge badge-mal">Sin stock</span> : n2(p.stock)}</td>
                  <td data-label="Precio" className="num"><b>{bs(p.PrecioVenta)}</b></td>
                </tr>
              ))}
              <FilaMas nav={nav} columnas={7} />
              {!filtrados.length && <tr><td colSpan={7} className="vacio">{cargando ? 'Cargando productos...' : 'Sin productos'}</td></tr>}
            </tbody>
          </table>
        </div>
        <div className="modal-acciones"><span className="ayuda-tip"><span className="solo-teclado"><kbd>↓</kbd><kbd>↑</kbd> elegir · <kbd>Enter</kbd> cantidad · </span>{AYUDA_ELEGIR} · <span className="leyenda-sin-stock">en rojo</span>: sin stock</span><button className="btn btn-pri" onClick={onCerrar}>Listo</button></div>
      </div>
      {elegido && (
        <CantidadVenta producto={elegido} sinLimite={sinStock} onCancelar={() => { setElegido(null); nav.volver(false); }}
          onAceptar={async (cant) => {
            const ok = await onAgregar(elegido, categoria, cant);
            setElegido(null);
            if (ok !== false) { setAgregados((a) => a + 1); setQ(''); }
            nav.volver(ok !== false);
          }} />
      )}
    </div>,
    document.body,
  );
}

// ---------- Efecto tipo 5 (TipoMovimiento 3): cantidad, no supera el stock ----------
// TipoMovimiento 4 (proforma): no limita por stock
function CantidadVenta({ producto, onAceptar, onCancelar, sinLimite = false }) {
  const stock = num(producto.stock);
  const [cant, setCant] = useState('1');
  const [err, setErr] = useState('');
  const validar = () => {
    const c = num(cant);
    if (!(c > 0)) { setErr('La Cantidad debe ser Mayor a 0'); return; }
    if (!sinLimite && c > stock + 1e-9) { setErr(`La cantidad es Superior Al Stock Disponible = ${n2(stock)}`); return; }
    onAceptar(c);
  };
  return (
    <div className="modal-fondo modal-encima" onMouseDown={(e) => { e.stopPropagation(); onCancelar(); }}>
      <div className="modal modal-sm cantidad-cajas" role="dialog" aria-modal="true" aria-label="Cantidad" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') { e.stopPropagation(); onCancelar(); } if (e.key === 'Enter') { e.preventDefault(); validar(); } }}>
        <h3>{tit(producto.DescripcionProducto || producto.NombreProducto)}{Number(producto.Tipo) === 2 ? ' (kit)' : ''}</h3>
        <p className="cc-stock">Stock: <b>{n2(stock)}</b> · Precio: <b>{bs(producto.PrecioVenta)}</b></p>
        <div className="cc-campos cc-unico">
          <label>Cantidad<input autoFocus onFocus={(e) => e.target.select()} className="control num" type="number" inputMode="decimal" min="0" step="any" value={cant}
            onChange={(e) => { setErr(''); setCant(e.target.value); }} /></label>
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

// ---------- EfectivoCobranza: cobro en contado ----------
function CobroVenta({ total, tipoCambio, onAceptar, onCancelar }) {
  const [p, setP] = useState({ MontoBs: '', MontoDolares: '', Tarjeta: '', Transferencia: '' });
  const [err, setErr] = useState('');
  const setv = (k) => (e) => { setErr(''); setP((x) => ({ ...x, [k]: e.target.value })); };
  // chkMontoBs / chkTarjeta / chkTransferencia: todo el total en esa forma de pago
  const todo = (k) => (e) => { setErr(''); setP((x) => ({ ...x, [k]: e.target.checked ? String(total) : '' })); };
  const pagado = num(p.MontoBs) + num(p.MontoDolares) * tipoCambio + num(p.Tarjeta) + num(p.Transferencia);
  const cambio = Math.max(0, pagado - total);
  const aceptar = () => {
    if (r2(pagado) < r2(total)) { setErr('El Monto Pagado Es Menor Al Monto Total De La Venta'); return; }
    onAceptar({ MontoBs: num(p.MontoBs), MontoDolares: num(p.MontoDolares), Tarjeta: num(p.Tarjeta), Transferencia: num(p.Transferencia) });
  };
  const fila = (k, etiqueta, conTodo = true) => (
    <label className="cobro-fila" key={k}>
      <span>{etiqueta}</span>
      <input className="control num" type="number" inputMode="decimal" min="0" step="0.01" value={p[k]} onChange={setv(k)} onFocus={(e) => e.target.select()} autoFocus={k === 'MontoBs'} />
      {conTodo ? <span className="cobro-todo"><input type="checkbox" checked={num(p[k]) === total && total > 0} onChange={todo(k)} aria-label={`Todo en ${etiqueta}`} /> Todo</span> : <small>{bs(num(p[k]) * tipoCambio)} Bs</small>}
    </label>
  );
  return createPortal(
    <div className="modal-fondo" onMouseDown={onCancelar}>
      <div className="modal modal-sm" role="dialog" aria-modal="true" aria-label="Cobro de la venta" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') onCancelar(); if (e.key === 'Enter') { e.preventDefault(); aceptar(); } }}>
        <div className="modal-cab"><h3>Cobro de la venta</h3><button className="btn-icono" onClick={onCancelar} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="venta-totales">
          <div className="venta-total"><span>Total venta</span><b>Bs {bs(total)}</b></div>
        </div>
        <div className="cobro-campos">
          {fila('MontoBs', 'Efectivo Bs')}
          {fila('MontoDolares', `Dólares (TC ${n2(tipoCambio)})`, false)}
          {fila('Tarjeta', 'Tarjeta')}
          {fila('Transferencia', 'Transferencia')}
        </div>
        <div className="venta-totales">
          <div><span>Total pagado</span><b>{bs(pagado)}</b></div>
          <div className="venta-total"><span>Cambio</span><b>Bs {bs(cambio)}</b></div>
        </div>
        {err && <p className="campo-error" role="alert">{err}</p>}
        <div className="modal-acciones">
          <button className="btn btn-sec" onClick={onCancelar}>Cancelar</button>
          <button className="btn btn-pri" onClick={aceptar}><Icono n="check" size={16} /> Cobrar y grabar</button>
        </div>
      </div>
    </div>,
    document.body,
  );
}

// ---------- P_GenerarReporte: Nota de venta (Recibo.rpt). Total = Suma - Descuento ----------
function NotaVenta({ datos, cfg, onCerrar }) {
  const { venta, detalle, total, empresa, cliente = {}, codigos = {} } = datos;
  const credito = cfg.tipoVenta && Number(venta.TipoVenta) === 0;
  const pagado = num(venta.MontoBs) + num(venta.MontoDolares) * num(venta.TipoCambio) + num(venta.TarjetaBancaria) + num(venta.TransferenciaBancaria);
  return (
    <NotaImpresa titulo={cfg.nota} id={venta.Id} fecha={venta.FechaVenta} empresa={empresa} imagenEmpresa={datos.imagenEmpresa} onCerrar={onCerrar}
      grabada={datos.grabada ? `${cfg.nombre} #${venta.Id} ${datos.grabada === 'nueva' ? 'grabada' : 'modificada'} con éxito.` : null}
      cabecera={[
        ['Señor(es):', venta.NombreCliente], ['Nit/Ci:', cliente.Nit || 'SN'], ['Dirección:', cliente.Direccion || ''], ['Teléfono:', cliente.Telefono || ''],
        ...(cfg.tipoVenta ? [['Tipo Venta:', credito ? 'Crédito' : 'Contado'], ['Fecha Venc. Crédito:', credito && venta.FechaVencimientoCredito ? fechaCorta(`${venta.FechaVencimientoCredito}T00:00:00Z`) : '']] : []),
        ...(venta.HoraRegistro ? [['Hora Registro:', venta.HoraRegistro]] : []), ['Sucursal:', venta.NombreAlmacen || ''],
      ]}
      columnas={[
        { titulo: 'Código', num: true, valor: (f) => codigos[f.Producto] ?? f.ProductoId },
        { titulo: 'Producto', valor: (f) => (f.KitNombre ? `${f.Producto} (${f.KitNombre})` : f.Producto) },
        { titulo: 'Cantidad', num: true, valor: (f) => bs(f.Cantidad) }, { titulo: 'Precio', num: true, valor: (f) => bs(f.Precio) },
        { titulo: 'SubTotal', num: true, valor: (f) => bs(f.SubTotal ?? (num(f.Total) + num(f.MontoDescuento))) }, { titulo: 'Monto Desc.', num: true, valor: (f) => bs(f.MontoDescuento) },
        { titulo: 'Total', num: true, valor: (f) => bs(f.Total) },
      ]}
      filas={detalle} claveFila={(f, i) => `${f.ProductoId}-${i}`}
      pie={[['Vendedor:', tit(venta.Personal || '')], ['Glosa:', venta.Glosa || ''], ...(cfg.cobro && !credito && pagado > 0 ? [['Pagado:', bs(pagado)], ['Cambio:', bs(pagado - total)]] : [])]}
      totales={[['Sub Total:', bs(datos.subtotal)], ['Descuento:', bs(venta.Descuento)]]}
      etiquetaTotal={`Total ${cfg.nombre}: Bs`} total={total} />
  );
}
