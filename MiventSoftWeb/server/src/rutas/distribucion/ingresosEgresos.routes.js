// =====================================================================
//  Tec_ProgramaIngresoEgresoCaja.vb (programa: btnCierreRegistroIngresoEgresoGenerales)
//  SP: MAM_CajaIngresoEgreso  lista (L_prListarGeneral) / 1 insertar / 2 modificar / -1 eliminar
//      4 cajas, 5 motivos, 8 comprobante.  Nuevo motivo: MAM_Ventas @tipo 21 (Crear_TipoMovimientoCaja)  Caja chica: MAM_CierreCajero (L_prListarGeneral)
//  Reglas del escritorio:
//   - Monto > 0, motivo, caja y sucursal requeridos. Ingreso/Egreso lo define el motivo (columna Tipo).
//   - Motivo 9 = traspaso entre cajas: exige caja destino distinta; graba 2 movimientos
//     (el segundo en la caja destino como ingreso, motivo 10, enlazado al primero). No se modifica.
//   - Caja 2 (caja chica): debe existir un cierre de cajero ABIERTO de esa sucursal en esa fecha.
//   - No se modifica ni elimina si pertenece a un cierre de cajero (CierreCajeroId > 0).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { fechaReq, aVB, col, num, fechaIso, limpiar } = require('./comun');

// Kaily usa el mismo formulario con otro boton (btnGastos, "Registrar Gastos o Ingresos" de Administrar Caja)
function crearRouterIE(programa) {
const router = express.Router();
router.use(requierePrograma(programa));

const TRASPASO = 9;
const TRASPASO_DESTINO = 10;
const CAJA_CHICA = 2;
const CAMPOS = ['Id', 'CierreCajeroId', 'Fecha', 'Descripcion', 'Monto', 'CajaId', 'NombreCaja', 'IngresoEgreso', 'Movimiento', 'CajatipoMovimientoId',
    'NombreTipoMovimiento', 'PersonalId', 'NombrePersonal', 'SucursalId', 'Sucursal', 'CajaIngresoEgresoIdDestino', 'CajaIdDestino'];
const fila = (f) => ({ ...f, Fecha: fechaIso(f.Fecha), IngresoEgreso: f.IngresoEgreso === true || Number(f.IngresoEgreso) === 1 ? 1 : 0, CierreCajeroId: num(f.CierreCajeroId) });
const MSJ_CIERRE = 'No Es Posible Modificar El Movimiento ya Que Pertenece a Un cierre de Caja Con Estado Cerrado'.toUpperCase();

async function combos() {
    const [cajas, tipos, suc] = await Promise.all([L.L_prListarCaja(), L.L_prListarTipoMovimientoCaja(), L.L_fnGeneralSucursales()]);
    return { cajas: v.proyectar(cajas, ['Id', 'NombreCaja']), tipos: v.proyectar(tipos, ['Id', 'Descripcion', 'Tipo']), sucursales: v.proyectar(suc, ['aanumi', 'aabdes']) };
}
async function buscar(id) {
    const f = v.proyectar(await L.L_prListarGeneral('MAM_CajaIngresoEgreso'), CAMPOS).find((x) => Number(x.Id) === id);
    if (!f) throw new ErrorNegocio(`No se encontro el movimiento ${id}. Actualice la lista.`, 404);
    return fila(f);
}
// VB: L_prListarGeneral("MAM_CierreCajero").Select("SucursalId=.. and EstadoCaja=1") y Fecha = fecha del movimiento
async function cajaChicaAbierta(sucursal, fecha) {
    const dt = await L.L_prListarGeneral('MAM_CierreCajero');
    const ok = dt.some((r) => num(col(r, 'SucursalId')) === sucursal && num(col(r, 'EstadoCaja')) === 1 && fechaIso(col(r, 'Fecha')) === fecha);
    if (!ok) throw new ErrorNegocio(`No Es Posible Hacer EL Movimiento Por que no Existe Caja Chica con Estado Abierto Para Esta Fecha =${fecha.split('-').reverse().join('/')}`);
}

async function leer(b, esNuevo) {
    const c = await combos();
    const Fecha = fechaReq(b.Fecha, 'Fecha');
    const Monto = Number(b.Monto);
    if (!Number.isFinite(Monto) || Monto <= 0) throw new ErrorNegocio('Los Siguientes Campos Son Requeridos: Monto Valido Mayor a 0');
    const tipo = c.tipos.find((t) => Number(t.Id) === Number(b.CajaTipoMovimientoId));
    if (!tipo) throw new ErrorNegocio('Los Siguientes Campos Son Requeridos: Motivo Movimiento');
    const CajaId = Number(b.CajaId);
    if (!c.cajas.some((x) => Number(x.Id) === CajaId)) throw new ErrorNegocio('Los Siguientes Campos Son Requeridos: Caja');
    const SucursalId = Number(b.SucursalId);
    if (!c.sucursales.some((x) => Number(x.aanumi) === SucursalId)) throw new ErrorNegocio('Los Siguientes Campos Son Requeridos: Sucursal');
    const m = { Fecha, Descripcion: v.texto(b.Descripcion, 500), Monto, CajaId, SucursalId, CajaTipoMovimientoId: Number(tipo.Id), IngresoEgreso: Number(tipo.Tipo) === 1 ? 1 : 0, CajaIdDestino: 0 };
    if (esNuevo && m.CajaTipoMovimientoId === TRASPASO) {
        m.CajaIdDestino = Number(b.CajaIdDestino);
        if (!c.cajas.some((x) => Number(x.Id) === m.CajaIdDestino)) throw new ErrorNegocio('Seleccione una Caja Destino');
        if (m.CajaIdDestino === CajaId) throw new ErrorNegocio('en el Campo Caja Destino Seleccione una Caja diferente Al Origen');
    }
    if (CajaId === CAJA_CHICA || m.CajaIdDestino === CAJA_CHICA) await cajaChicaAbierta(SucursalId, Fecha);
    return m;
}

router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_CajaIngresoEgreso'), CAMPOS).map(fila) });
}));
router.get('/combos', asyncHandler(async (req, res) => {
    const c = await combos();
    const p = req.sesion.Global_IdPersonal ? await L.ListarPersonalById(req.sesion.Global_IdPersonal) : [];
    res.json({ ok: true, data: { ...c, personal: p[0] ? { Id: req.sesion.Global_IdPersonal, Nombre: col(p[0], 'Nombre') ?? '' } : null } });
}));
// btnAgregarMotivoMovimiento (Efecto 21 -> Crear_TipoMovimientoCaja): descripcion de mas de 3 letras + ingreso/egreso
router.post('/motivos', asyncHandler(async (req, res) => {
    const desc = v.requerido(req.body?.descripcion, 'Tipo Movimiento', 100).toUpperCase();
    if (desc.length <= 3) throw new ErrorNegocio('Debe Ingresar un Texto Valido en Tipo Movimiento');
    const tipos = await L.L_prListarTipoMovimientoCaja();
    if (tipos.some((t) => String(col(t, 'Descripcion') ?? '').trim().toUpperCase() === desc)) throw new ErrorNegocio(`El motivo "${desc}" ya existe.`);
    const r = await L.InsertarTipoMovimiento(desc, v.bit(req.body?.ingreso) ? 1 : 0);
    if (!r.resultado) throw new ErrorNegocio('Error Al guardar Tipo Movimiento');
    res.json({ ok: true, id: Number(String(r._numi).trim()), data: (await combos()).tipos, mensaje: `MOTIVO ${desc} REGISTRADO CON EXITO.` });
}));
router.get('/:id/reporte', asyncHandler(async (req, res) => {
    const [dt, emp] = await Promise.all([L.ListarReporteIngresoEgreso(v.idRuta(req.params.id)), L.ObtenerImagenEmpresa()]);
    const e = emp[0] || {};
    res.json({ ok: true, data: { filas: limpiar(dt).map((f) => { const o = { ...f }; delete o.img; return o; }), empresa: { nombre: e.Nombre ?? '', direccion: e.Direccion ?? '' }, imagenEmpresa: emp.length ? String(Object.values(e)[0] ?? '') : '' } });
}));

router.post('/', asyncHandler(async (req, res) => {
    const m = await leer(req.body || {}, true);
    const PersonalId = num(req.sesion.Global_IdPersonal);
    const base = { ...m, Fecha: aVB(m.Fecha), PersonalId, _numi: '' };
    if (m.CajaTipoMovimientoId === TRASPASO) {
        // _prGuardarTraspaso: salida de la caja origen + ingreso (motivo 10) en la caja destino
        const r1 = await L.L_prIngresoSalidaInsertar({ ...base, IdIngresoEgreso: 0 });
        if (!r1.resultado) throw new ErrorNegocio('El traspaso no pudo ser insertado'.toUpperCase());
        const r2 = await L.L_prIngresoSalidaInsertar({ ...base, CajaId: m.CajaIdDestino, IngresoEgreso: 1, CajaTipoMovimientoId: TRASPASO_DESTINO, CajaIdDestino: m.CajaId, IdIngresoEgreso: r1._numi });
        if (!r2.resultado) throw new ErrorNegocio(`Se registro la salida #${r1._numi} pero NO el ingreso en la caja destino. Revise y corrija el movimiento.`);
        return res.json({ ok: true, id: r1._numi, mensaje: 'TRASPASO DE DINERO ENTRE CAJAS GRABADO CON EXITO.' });
    }
    const r = await L.L_prIngresoSalidaInsertar(base);
    if (!r.resultado) throw new ErrorNegocio('Error al guardar Ingreso / Egreso'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `INGRESO / EGRESO #${r._numi} GRABADO CON EXITO.` });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const actual = await buscar(id);
    if (num(actual.CajatipoMovimientoId) === TRASPASO) throw new ErrorNegocio('No Es Posible Modificar Un Traspaso. Elimine Este registro y Vuelva a Registrarlo'.toUpperCase());
    if (actual.CierreCajeroId > 0) throw new ErrorNegocio(MSJ_CIERRE);
    const m = await leer(req.body || {}, false);
    if (m.CajaTipoMovimientoId === TRASPASO) throw new ErrorNegocio('Para un traspaso elimine este registro y registre uno nuevo');
    const ok = await L.L_prIngresoSalidaModificar({ ...m, Fecha: aVB(m.Fecha), PersonalId: num(actual.PersonalId), _numi: String(id), CajaIdDestino: 0 });
    if (!ok) throw new ErrorNegocio('Error al guardar Ingreso / Egreso'.toUpperCase());
    res.json({ ok: true, id, mensaje: `CODIGO DE INGRESO / EGRESO ${id} MODIFICADO CON EXITO.` });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const actual = await buscar(id);
    if (actual.CierreCajeroId > 0) throw new ErrorNegocio(MSJ_CIERRE);
    if (!(await L.L_prBorrarRegistro(id, 'MAM_CajaIngresoEgreso'))) throw new ErrorNegocio('Error al eliminar Caja Ingreso / Egreso'.toUpperCase());
    res.json({ ok: true, mensaje: `CODIGO DE CAJA INGRESO / EGRESO ${id} ELIMINADO CON EXITO.` });
}));

return router;
}

module.exports = crearRouterIE('btnCierreRegistroIngresoEgresoGenerales');
module.exports.crearRouterIE = crearRouterIE;
