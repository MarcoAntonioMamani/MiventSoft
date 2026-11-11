// =====================================================================
//  KAILY LA PAZ: Tec_CierreCajaCajero.vb (programa: btnCierreCajero)   SP: MAM_CierreCajero
//
//  Igual que el escritorio:
//   * Lista: @tipo 3 (@desde, @hasta). Si el rol no es 1 y el usuario no tiene "Ver todos los cierres",
//     solo sus cierres (PersonalId = Global_IdPersonal).
//   * Nuevo = ABRIR caja: L_prCierreCajeroInsertar (@tipo 1) solo cabecera, EstadoCaja 1, fecha de hoy,
//     personal del usuario, sucursal del usuario (si tiene una), monto inicial, tipo de cambio y detalle.
//   * Modificar = CERRAR caja (solo si esta abierta): "Cargar datos" trae lo pendiente
//       ventas @tipo 5 · cobranzas @tipo 7 · ingresos/egresos @tipo 9 (con @Global_CierreTodos)
//     y el conteo de billetes. L_prCierreCajeroModificar (@tipo 2) con EstadoCaja 0,
//     @CorteBilletes (estructura @tipo 10) y @ModuloId (estructura @tipo 11: Id, 0, 1/2/3, Id del registro).
//   * Totales (Calculartotales):
//       TotalVentas = Σ totalbs · TotalTarjeta = Σ tarjeta · TotalTransferencia = Σ TransferenciaBancaria
//       TotalCobranzas = Σ Monto · Ingresos / Egresos por IngresoEgreso
//       TotalCaja = MontoInicial + Ventas + Cobranzas + Ingresos − Egresos
//       TotalEfectivo = Σ (CorteBs × CantidadBs) + Σ (CorteDolares × CantidadDolares) × TipoCambio
//       EfectivoRecibido = TotalEfectivo · Diferencia = EfectivoRecibido − TotalCaja
//   * No se modifica ni elimina un cierre de dias anteriores sin "Modificar registros anteriores".
//  Los totales los calcula el servidor con los mismos SP (el navegador solo envia las cantidades de billetes).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { rango, aVB, col, num, fechaIso, limpiar, hoyBolivia } = require('../distribucion/comun');

const router = express.Router();
router.use(requierePrograma('btnCierreCajero'));

// btnCargarDatos: cortes de billetes (Bs, $us) en el orden del escritorio
const CORTES = [[200, 100], [100, 50], [50, 20], [20, 10], [10, 5], [5, 1], [2, 0], [1, 0], [0.5, 0], [0.2, 0], [0.1, 0]];
const MODULO = { venta: 1, cobranza: 2, movimiento: 3 };
const r2 = (x) => Math.round(num(x) * 100) / 100;

function globales(s) {
    const suc = Number(s.gi_userSuc);
    return {
        rol: Number(s.gi_userRol),
        sucursal: Number.isFinite(suc) ? suc : -1,
        personal: Number(s.Global_IdPersonal) || 0,
        cierreTodos: s.Global_CierreTodos === 1 ? 1 : 0,
        modificarAnteriores: s.Global_ModificarRegistro === 1,
    };
}

const CAMPOS = ['Id', 'Fecha', 'PersonalId', 'NombrePersonal', 'SucursalId', 'Sucursal', 'MontoInicial', 'EstadoCaja', 'Caja', 'TipoCambio', 'TotalVentas',
    'TotalCobranza', 'TotalIngreso', 'TotalEgresos', 'TotalCaja', 'TotalEfectivo', 'TotalTransferencia', 'TotalTarjeta', 'TotalEfectivoRecibido', 'Diferencia', 'Observacion'];
const fila = (f) => ({ ...f, Fecha: fechaIso(f.Fecha), EstadoCaja: f.EstadoCaja === true || Number(f.EstadoCaja) === 1 ? 1 : 0 });

async function _lista(g, desde, hasta) {
    const dt = v.proyectar(await L.L_prListarGeneralCierreCajero(desde, hasta), CAMPOS).map(fila);
    return g.rol === 1 || g.cierreTodos === 1 ? dt : dt.filter((f) => Number(f.PersonalId) === g.personal);
}
async function _buscar(g, id, fecha) {
    const f = (await _lista(g, aVB(fecha), aVB(fecha))).find((x) => Number(x.Id) === id);
    if (!f) throw new ErrorNegocio(`No se encontro el cierre de caja ${id}. Actualice la lista.`, 404);
    return f;
}
function _validarEditable(g, f, accion) {
    if (!g.modificarAnteriores && f.Fecha < hoyBolivia()) {
        throw new ErrorNegocio(`No puede ${accion} Cierre Anteriores a la fecha de Hoy, Consulte con su Administrador`.toUpperCase());
    }
}

// Lo pendiente de cerrar (btnCargarDatos) o lo que ya pertenece al cierre
async function _datos(g, cierre, pendiente) {
    if (pendiente) {
        const fecha = aVB(cierre.Fecha);
        const [ventas, cobranzas, movimientos] = await Promise.all([
            L.L_prListarVentaCierresCajaPendiente(num(cierre.PersonalId), num(cierre.SucursalId), fecha, g.cierreTodos),
            L.L_prListarCobrosCajaPendiente(fecha, num(cierre.PersonalId), g.cierreTodos),
            L.L_prListarMovimientosIngresoEgresoCierrePendiente(num(cierre.SucursalId), fecha, num(cierre.PersonalId), g.cierreTodos),
        ]);
        return { ventas, cobranzas, movimientos, cortes: [] };
    }
    const id = num(cierre.Id);
    const [ventas, cobranzas, movimientos, cortes] = await Promise.all([
        L.L_prListarVentaCierresCaja(id), L.L_prListarCobranzaCierresCaja(id), L.L_prListarMovimientosIngresoEgresoCierre(id), L.L_prListarEfectivoCortesCierre(id),
    ]);
    return { ventas, cobranzas, movimientos, cortes };
}

// Calculartotales + _prCalcular
function _totales(montoInicial, tipoCambio, d, cortes) {
    const s = (dt, c) => dt.reduce((a, f) => a + num(col(f, c)), 0);
    const ingresos = d.movimientos.filter((f) => num(col(f, 'IngresoEgreso')) === 1).reduce((a, f) => a + num(col(f, 'Monto')), 0);
    const egresos = d.movimientos.filter((f) => num(col(f, 'IngresoEgreso')) !== 1).reduce((a, f) => a + num(col(f, 'Monto')), 0);
    const TotalVentas = s(d.ventas, 'totalbs');
    const TotalCobranzas = s(d.cobranzas, 'Monto');
    const TotalCaja = montoInicial + TotalVentas + TotalCobranzas + ingresos - egresos;
    const efectivoBs = cortes.reduce((a, c) => a + num(c.CorteBs) * num(c.CantidadBs), 0);
    const efectivoDol = cortes.reduce((a, c) => a + num(c.CorteDolares) * num(c.CantidadDolares), 0);
    const TotalEfectivo = efectivoBs + efectivoDol * tipoCambio;
    return {
        TotalVentas, TotalCobranzas, TotalIngresos: ingresos, TotalEgresos: egresos, TotalCaja,
        TotalTarjeta: s(d.ventas, 'tarjeta'), TotalTransferencia: s(d.ventas, 'TransferenciaBancaria'),
        TotalEfectivo, TotalEfectivoRecibido: TotalEfectivo, Diferencia: TotalEfectivo - TotalCaja,
    };
}

// Cantidades del navegador -> filas de cortes (las 11 denominaciones del escritorio)
function _cortes(entrada) {
    const lista = Array.isArray(entrada) ? entrada : [];
    return CORTES.map(([bs, dol], i) => {
        const e = lista[i] || {};
        const cb = Math.max(0, Math.floor(num(e.CantidadBs)));
        const cd = Math.max(0, Math.floor(num(e.CantidadDolares)));
        return { Id: i + 1, CierreCajeroId: 0, CorteBs: bs, CantidadBs: cb, SubTotalBs: bs * cb, CorteDolares: dol, CantidadDolares: cd, SubtotalDolares: dol * cd };
    });
}

function _leerCabecera(b) {
    const MontoInicial = Number(b.MontoInicial ?? 0);
    if (!Number.isFinite(MontoInicial) || MontoInicial < 0) throw new ErrorNegocio('Ingrese un Monto Inicial Valido');
    const TipoCambio = Number(b.TipoCambio ?? 0);
    if (!Number.isFinite(TipoCambio) || TipoCambio < 0) throw new ErrorNegocio('Ingrese un Tipo de Cambio Valido');
    return { MontoInicial, TipoCambio, Observacion: v.texto(b.Observacion, 500), SucursalId: v.entero(b.SucursalId, 'una Sucursal', { requeridoPositivo: true }) };
}

// =====================================================================
router.get('/', asyncHandler(async (req, res) => {
    const [desde, hasta] = rango(req.query);
    res.json({ ok: true, data: await _lista(globales(req.sesion), desde, hasta) });
}));

router.get('/inicio', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    const [sucursales, personal, tipoCambio] = await Promise.all([L.L_fnGeneralSucursales(), L.ListarPersonalById(g.personal), L.L_ObtenerTipoCambio()]);
    res.json({
        ok: true,
        data: {
            sucursales: v.proyectar(sucursales, ['aanumi', 'aabdes']),
            sucursalUsuario: g.sucursal,
            personal: personal.length ? { Id: g.personal, Nombre: String(col(personal[0], 'Nombre') ?? '') } : null,
            tipoCambio, hoy: hoyBolivia(), cortes: CORTES, modificarAnteriores: g.modificarAnteriores,
        },
    });
}));

// Detalle de un cierre: lo grabado (cerrado) o, con ?pendiente=1, lo que se cerraria (btnCargarDatos)
router.get('/:id/detalle', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    const id = v.idRuta(req.params.id);
    const cierre = await _buscar(g, id, fechaIso(req.query.fecha) || hoyBolivia());
    const pendiente = req.query.pendiente === '1';
    if (pendiente && cierre.EstadoCaja !== 1) throw new ErrorNegocio('La caja ya esta cerrada.');
    const d = await _datos(g, cierre, pendiente);
    // Cerrado: los billetes grabados (@tipo 10). Pendiente: las 11 denominaciones en 0 (btnCargarDatos)
    const cortes = pendiente ? _cortes([]) : d.cortes.map((c) => ({
        Id: num(col(c, 'Id')), CierreCajeroId: num(col(c, 'CierreCajeroId')), CorteBs: num(col(c, 'CorteBs')), CantidadBs: num(col(c, 'CantidadBs')),
        SubTotalBs: num(col(c, 'SubTotalBs')), CorteDolares: num(col(c, 'CorteDolares')), CantidadDolares: num(col(c, 'CantidadDolares')), SubtotalDolares: num(col(c, 'SubtotalDolares')),
    }));
    const t = _totales(num(cierre.MontoInicial), num(cierre.TipoCambio), d, cortes);
    res.json({ ok: true, data: { ventas: limpiar(d.ventas), cobranzas: limpiar(d.cobranzas), movimientos: limpiar(d.movimientos), cortes: limpiar(cortes), totales: t } });
}));

// Nuevo: abrir caja
router.post('/', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    if (!(g.personal > 0)) throw new ErrorNegocio('Su usuario no tiene un personal asignado.');
    const cab = _leerCabecera(req.body || {});
    if (g.sucursal > 0) cab.SucursalId = g.sucursal; // cbSucursal fijo
    const r = await L.L_prCierreCajeroInsertar({
        _numi: '', Fecha: aVB(hoyBolivia()), PersonalId: g.personal, SucursalId: cab.SucursalId, MontoInicial: cab.MontoInicial, EstadoCaja: 1, TipoCambio: cab.TipoCambio,
        TotalVentas: 0, TotalCobranzas: 0, TotalIngresos: 0, TotalEgresos: 0, TotalCaja: cab.MontoInicial, TotalEfectivo: 0, TotalTransferencia: 0, TotalTarjeta: 0,
        TotalEfectivoRecibido: 0, Diferencia: -cab.MontoInicial, Observacion: cab.Observacion,
    });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar Cierre De Caja'.toUpperCase());
    res.json({ ok: true, id: r._numi, fecha: hoyBolivia(), mensaje: `CIERRE CAJA #${r._numi} GRABADO CON EXITO.` });
}));

// Modificar: cerrar caja con lo pendiente y el conteo de billetes
router.put('/:id', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    const id = v.idRuta(req.params.id);
    const cierre = await _buscar(g, id, fechaIso(req.body?.fechaOriginal) || hoyBolivia());
    if (cierre.EstadoCaja !== 1) throw new ErrorNegocio('No Se Puede Modificar La Caja Por que Ya Se Encuentra con Estado Cerrada. Elimine la Caja Y Vuelva a Realizar El Cierre');
    _validarEditable(g, cierre, 'Modificar');
    const cab = _leerCabecera(req.body || {});
    const d = await _datos(g, cierre, true);
    const cortes = _cortes(req.body.cortes);
    const t = _totales(cab.MontoInicial, cab.TipoCambio, d, cortes);
    // ArmarIdModulos: (id, 0, 1 venta / 2 cobranza / 3 movimiento, Id del registro)
    const modCols = L._columnasDe(await L.L_prListarIdModulosCierre(-1));
    const modFilas = [];
    const agregar = (dt, tipo) => dt.forEach((f) => modFilas.push([modFilas.length, 0, tipo, num(col(f, 'Id'))]));
    agregar(d.ventas, MODULO.venta); agregar(d.cobranzas, MODULO.cobranza); agregar(d.movimientos, MODULO.movimiento);
    const cortesCols = L._columnasDe(await L.L_prListarEfectivoCortesCierre(-1));
    const r = await L.L_prCierreCajeroModificar({
        _numi: id, Fecha: aVB(cierre.Fecha), PersonalId: num(cierre.PersonalId), SucursalId: cab.SucursalId, MontoInicial: cab.MontoInicial, EstadoCaja: 0,
        TipoCambio: cab.TipoCambio, ...Object.fromEntries(Object.entries(t).map(([k, x]) => [k, r2(x)])), Observacion: cab.Observacion,
    }, { columnas: cortesCols, filas: cortes.map((c) => [c.Id, c.CierreCajeroId, c.CorteBs, c.CantidadBs, c.SubTotalBs, c.CorteDolares, c.CantidadDolares, c.SubtotalDolares]) },
    { columnas: modCols, filas: modFilas });
    if (!r.resultado) throw new ErrorNegocio('Error al guardar Cierre Caja'.toUpperCase());
    res.json({ ok: true, id, fecha: cierre.Fecha, mensaje: `CODIGO DE CIERRE DE CAJA ${id} MODIFICADO CON EXITO.` });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    const id = v.idRuta(req.params.id);
    const cierre = await _buscar(g, id, fechaIso(req.query.fecha) || hoyBolivia());
    _validarEditable(g, cierre, 'Eliminar');
    if (!(await L.L_prBorrarRegistro(id, 'MAM_CierreCajero'))) throw new ErrorNegocio('Error al eliminar Cierre De Caja'.toUpperCase());
    res.json({ ok: true, mensaje: `CODIGO DE CIERRE CAJA ${id} ELIMINADO CON EXITO.` });
}));

module.exports = router;
module.exports._interno = { _totales, _cortes, CORTES };
