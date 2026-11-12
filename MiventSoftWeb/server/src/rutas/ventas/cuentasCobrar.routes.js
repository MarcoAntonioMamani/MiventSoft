// =====================================================================
//  KAILY LA PAZ: Tec_AdministrarCuentasPorCobrar.vb  (programa: btnCredPago)  SP: MAM_CreditosVentas
//   Cobrar Credito       @tipo 3 (deudas) + @tipo 5 (pagos) + @tipo 4 (grabar pago)
//   Creditos Pendientes  @tipo 1      Creditos Cobrados @tipo 2
//   Administrar Cobros   @tipo 6 (todas) + @tipo 5 + @tipo 7 (eliminar pago) + @tipo 8 (estado de cuenta)
//   Personal que cobra   ListarPersonalCredito (MAM_Compras @tipo 8)
//   Reportes (cabecera)  MAM_Empresas (L_prListarGeneral) + logo MAM_ReporteVentas @tipo 3, igual que la nota de venta
//  Creditos Cobrados por fecha de venta: @tipo 2 no trae la fecha de la venta; se cruza (solo lectura)
//   con MAM_Ventas @tipo 3 del rango (L_prListarVentasGeneralFiltroFecha, la misma lista de Ventas) por el numero de venta.
//  Reglas de Kaily:
//   * Sin rol 1 ni "Ver todas las ventas" (Global_VentasTodos) solo se ven los creditos de su personal (PersonalId).
//   * El cobro exige una Caja Chica abierta (MAM_CierreCajero, EstadoCaja = 1) con la fecha del pago.
//   * Un pago no se elimina si es de dias anteriores (sin ModificarRegistroAnteriores) o si ya esta en un cierre (CierreModulo > 0).
//  Mejora: el servidor valida que el pago no supere el saldo (VB solo lo hacia en pantalla).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btnCredPago'));

const dos = (n) => String(n).padStart(2, '0');
const iso = (x) => {
    if (x instanceof Date) return `${x.getUTCFullYear()}-${dos(x.getUTCMonth() + 1)}-${dos(x.getUTCDate())}`;
    const m = /^(\d{4})[-/](\d{2})[-/](\d{2})/.exec(String(x ?? ''));
    return m ? `${m[1]}-${m[2]}-${m[3]}` : (x ?? null);
};
const conFechas = (filas, campos) => filas.map((f) => { const o = { ...f }; campos.forEach((c) => { if (c in o) o[c] = iso(o[c]); }); return o; });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };

const CAMPOS_DEUDA = ['Id', 'Credito', 'Venta', 'Nombre', 'Monto', 'abonado', 'Restante', 'FechaVencimientoCredito', 'DiasMora'];
const CAMPOS_PAGO = ['Id', 'FechaPago', 'Monto', 'Glosa', 'NroComprobante', 'NombrePersonal', 'CierreModulo'];
const dmy = (s) => s.split('-').reverse().join('/');
const hoyBolivia = () => iso(new Date(Date.now() - 4 * 3600 * 1000)); // Global_FechaActual

// gi_userRol <> 1 y Global_VentasTodos <> 1: solo las filas con PersonalId = Global_IdPersonal
function delPersonal(req, dt) {
    const s = req.sesion;
    if (Number(s.gi_userRol) === 1 || s.Global_VentasTodos === 1) return dt;
    const k = Object.keys(dt[0] || {}).find((x) => x.toLowerCase() === 'personalid');
    return k ? dt.filter((f) => Number(f[k]) === Number(s.Global_IdPersonal)) : dt;
}
const deudas = async (req) => v.proyectar(delPersonal(req, await L.L_prListarPagosPendientesFiltrosClientes()), CAMPOS_DEUDA);
const todas = async (req) => v.proyectar(delPersonal(req, await L.L_prListarPagosTodosCuentasPorCobrar()), CAMPOS_DEUDA);
const pagosDe = async (id) => conFechas(v.proyectar(await L.L_prListarPagosCreditoVenta(id), CAMPOS_PAGO), ['FechaPago']).map((p) => ({ ...p, CierreModulo: num(p.CierreModulo) }));

// ButtonX4: Caja Chica con EstadoCaja = 1 y Fecha = fecha del pago
async function validarCaja(fecha) {
    const dt = await L.L_prListarGeneral('MAM_CierreCajero');
    const k = (f, n) => f[Object.keys(f).find((x) => x.toLowerCase() === n.toLowerCase())];
    if (!dt.some((f) => Number(k(f, 'EstadoCaja')) === 1 && iso(k(f, 'Fecha')) === fecha)) {
        throw new ErrorNegocio(`No Es Posible Hacer EL Cobro Por que no Existe Caja Chica con Estado Abierto Para Esta Fecha =${dmy(fecha)}`);
    }
}

// Cobrar Credito: deudas pendientes (@tipo 3)
router.get('/deudas', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: conFechas(await deudas(req), ['FechaVencimientoCredito']) });
}));
// Administrar Cobros: todas las cuentas (@tipo 6)
router.get('/todas', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: conFechas(await todas(req), ['FechaVencimientoCredito']) });
}));
// Creditos Pendientes (@tipo 1): Vendedor = NombrePersonal
router.get('/pendientes', asyncHandler(async (req, res) => {
    const c = ['Id', 'Credito', 'venta', 'NombrePersonal', 'Nombrecliente', 'Monto', 'abonado', 'Restante', 'FechaVencimientoCredito', 'DiasMora'];
    res.json({ ok: true, data: conFechas(v.proyectar(delPersonal(req, await L.L_prListarPagosPendientesClientes()), c), ['FechaVencimientoCredito']) });
}));
// Creditos Cobrados (@tipo 2), opcional ?desde=yyyy-mm-dd&hasta=yyyy-mm-dd por fecha de venta
const fechaIso = (x, nombre) => {
    const t = v.texto(x, 10);
    if (!/^\d{4}-\d{2}-\d{2}$/.test(t) || Number.isNaN(Date.parse(t))) throw new ErrorNegocio(`Fecha no valida: ${nombre}`);
    return t;
};
const nroVenta = (x) => { const m = String(x ?? '').match(/\d+/g); return m ? Number(m.join('')) : null; }; // "venta00047041" -> 47041
router.get('/cobrados', asyncHandler(async (req, res) => {
    const c = ['Id', 'Credito', 'venta', 'NombreCliente', 'Monto', 'Pagado', 'FechaVencimientoCredito', 'FechaUltimaPago'];
    const dt = delPersonal(req, await L.L_prListarCreditosPagadosCliente());
    let filas = conFechas(v.proyectar(dt, c), ['FechaVencimientoCredito', 'FechaUltimaPago']);
    if (req.query.desde || req.query.hasta) {
        const desde = fechaIso(req.query.desde, 'Desde');
        const hasta = fechaIso(req.query.hasta, 'Hasta');
        if (desde > hasta) throw new ErrorNegocio('La fecha Desde no puede ser mayor que Hasta');
        // si algun dia el SP devuelve la fecha de la venta, se usa directamente
        const campoFV = Object.keys(dt[0] || {}).find((k) => k.toLowerCase() === 'fechaventa');
        let fechaDe;
        if (campoFV) {
            const mapa = new Map(dt.map((f) => [String(f.Id), iso(f[campoFV])]));
            fechaDe = (f) => mapa.get(String(f.Id)) || null;
        } else {
            // Kaily: la lista de Ventas es L_prListarVentasGeneralFiltroFecha (-1 = todas las sucursales)
            const ventas = await L.L_prListarVentasGeneralFiltroFecha('MAM_Ventas', desde.replace(/-/g, '/'), hasta.replace(/-/g, '/'), -1);
            const mapa = new Map(v.proyectar(ventas, ['Id', 'FechaVenta']).map((x) => [Number(x.Id), iso(x.FechaVenta)]));
            fechaDe = (f) => mapa.get(nroVenta(f.venta)) || null;
        }
        filas = filas.map((f) => ({ ...f, FechaVenta: fechaDe(f) })).filter((f) => f.FechaVenta && f.FechaVenta >= desde && f.FechaVenta <= hasta);
    }
    res.json({ ok: true, data: filas });
}));
// Cabecera de los reportes impresos: empresa y logo (los mismos SP que la nota de venta)
router.get('/cabecera-reporte', asyncHandler(async (req, res) => {
    const [empresas, imagen] = await Promise.all([L.L_prListarGeneral('MAM_Empresas'), L.ObtenerImagenEmpresa().catch(() => [])]);
    res.json({
        ok: true,
        data: {
            empresa: v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Nit', 'Ciudad'])[0] || null,
            imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '',
        },
    });
}));
// Personal que registra el cobro (por defecto el del usuario)
router.get('/personal', asyncHandler(async (req, res) => {
    const [lista, propio] = await Promise.all([L.ListarPersonalCredito(), L.ListarPersonalById(req.sesion.Global_IdPersonal ?? 0)]);
    res.json({
        ok: true,
        data: {
            lista: v.proyectar(lista, ['Id', 'Nombre', 'Direccion', 'Telefono01']),
            defecto: propio.length ? { Id: Number(req.sesion.Global_IdPersonal), Nombre: String(propio[0].Nombre ?? '') } : null,
        },
    });
}));
// Pagos de un credito (@tipo 5)
router.get('/:id/pagos', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    res.json({ ok: true, data: await pagosDe(id) });
}));
// Estado de cuenta (@tipo 8): columnas del reporte, sin imagenes
router.get('/:id/estado', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const dt = await L.L_EstadoDeCuentasPorCobrar(id);
    const limpio = dt.map((f) => Object.fromEntries(Object.entries(f).filter(([, x]) => !Buffer.isBuffer(x)).map(([k, x]) => [k, x instanceof Date ? iso(x) : x])));
    res.json({ ok: true, data: limpio });
}));

// ButtonX4 "Pagar": ValidarCamposaGrabar + L_prGrabarPagosCreditoVentas
router.post('/:id/pagos', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const b = req.body || {};
    const monto = Math.round(v.decimal(b.Monto) * 100) / 100;
    if (!(monto > 0)) throw new ErrorNegocio('Los Siguientes Campos Son Requerido : El monto a Pagar Debe ser Mayor a 0');
    const fecha = v.texto(b.FechaPago, 10);
    if (!/^\d{4}-\d{2}-\d{2}$/.test(fecha) || Number.isNaN(Date.parse(fecha))) throw new ErrorNegocio('Debe Seleccionar Fecha');
    const personal = v.entero(b.PersonalId, 'Personal');
    // La deuda debe seguir pendiente y el pago no puede superar el saldo
    const deuda = (await deudas(req)).find((x) => Number(x.Id) === id);
    if (!deuda) throw new ErrorNegocio('La deuda ya no esta pendiente. Actualice la lista.');
    if (monto > num(deuda.Restante) + 0.005) throw new ErrorNegocio(`El monto Maximo para Pagar es ${num(deuda.Restante).toFixed(2)}`.toUpperCase());
    await validarCaja(fecha);
    const r = await L.L_prGrabarPagosCreditoVentas('', id, fecha.replace(/-/g, '/'), personal, v.texto(b.Glosa, 300), v.texto(b.NroComprobante, 50), monto);
    if (!r.resultado) throw new ErrorNegocio('Error al Grabar el Pago');
    res.json({ ok: true, id: r._numi, saldo: Math.round((num(deuda.Restante) - monto) * 100) / 100, mensaje: 'El Pago Ha sido Registrado con Exito' });
}));

// Administrar Cobros: eliminar un pago (@tipo 7) del credito :id
router.delete('/:id/pagos/:idPago', asyncHandler(async (req, res) => {
    const credito = v.idRuta(req.params.id);
    const id = v.idRuta(req.params.idPago);
    if (!(await todas(req)).some((x) => Number(x.Id) === credito)) throw new ErrorNegocio('No se encontro el credito. Actualice la lista.', 404);
    const pago = (await pagosDe(credito)).find((p) => Number(p.Id) === id);
    if (!pago) throw new ErrorNegocio('No se encontro el pago. Actualice la lista.', 404);
    if (req.sesion.Global_ModificarRegistro !== 1 && pago.FechaPago < hoyBolivia()) {
        throw new ErrorNegocio('No puede Eliminar Pagos Anteriores a la fecha de Hoy, Consulte con su Administrador'.toUpperCase());
    }
    if (pago.CierreModulo > 0) throw new ErrorNegocio(`No Es Posible Eliminar El Cobro por Que ya Pertenece a un Cierre De Caja # ${pago.CierreModulo}`);
    const ok = await L.L_prEliminarPagosCuentaPorCobrar(id);
    if (!ok) throw new ErrorNegocio('No se pudo eliminar el pago.');
    res.json({ ok: true, mensaje: 'El pago ha sido Eliminado con Exito'.toUpperCase() });
}));

module.exports = router;
