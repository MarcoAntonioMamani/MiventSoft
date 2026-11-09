// =====================================================================
//  Tec_AdministrarCuentasPorPagar.vb  (programa: btnCompraCredito)  SP: MAM_CreditosCompras
//   Pagar Credito        @tipo 3 (deudas) + @tipo 5 (pagos) + @tipo 4 (grabar pago)
//   Creditos Pendientes  @tipo 1      Creditos Pagados @tipo 2
//   Administrar Pagos    @tipo 6 (todas) + @tipo 5 + @tipo 7 (eliminar pago) + @tipo 8 (estado de cuenta)
//   Personal que paga    ListarPersonalCredito (MAM_Compras @tipo 8)
//  Mejora: el servidor valida que el pago no supere el saldo (VB solo lo hacia en pantalla).
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btnCompraCredito'));

const dos = (n) => String(n).padStart(2, '0');
const iso = (x) => {
    if (x instanceof Date) return `${x.getUTCFullYear()}-${dos(x.getUTCMonth() + 1)}-${dos(x.getUTCDate())}`;
    const m = /^(\d{4})[-/](\d{2})[-/](\d{2})/.exec(String(x ?? ''));
    return m ? `${m[1]}-${m[2]}-${m[3]}` : (x ?? null);
};
const conFechas = (filas, campos) => filas.map((f) => { const o = { ...f }; campos.forEach((c) => { if (c in o) o[c] = iso(o[c]); }); return o; });
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };

const CAMPOS_DEUDA = ['Id', 'Credito', 'Compra', 'NombrePersonal', 'Nombre', 'Monto', 'abonado', 'Restante', 'FechaVencimientoCredito', 'DiasMora'];

// Pagar Credito: deudas pendientes (@tipo 3)
router.get('/deudas', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: conFechas(v.proyectar(await L.L_prListarPagosPendientesFiltros(), CAMPOS_DEUDA), ['FechaVencimientoCredito']) });
}));
// Administrar Pagos: todas las cuentas (@tipo 6)
router.get('/todas', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: conFechas(v.proyectar(await L.L_prListarPagosTodosCuentasPorPagar(), CAMPOS_DEUDA), ['FechaVencimientoCredito']) });
}));
// Detalle de la compra del credito (mismo SP que Tec_ComprasDetalle: MAM_Compras @tipo 4), solo lectura
//  :compra = numero de la compra ("Compra0001" -> 1)
router.get('/compra/:compra/detalle', asyncHandler(async (req, res) => {
    const id = v.idRuta(String(req.params.compra).replace(/^\D+/, ''));
    const dt = await L.ListaComprasDetalles(id);
    const c = (f, ...n) => { for (const x of n) { const k = Object.keys(f).find((y) => y.toLowerCase() === x.toLowerCase()); if (k !== undefined) return f[k]; } return null; };
    res.json({ ok: true, data: dt.filter((f) => num(c(f, 'estado')) >= 0 || c(f, 'estado') === null).map((f) => {
        const conv = num(c(f, 'conversion', 'Conversion')) || 1; const cant = num(c(f, 'Cantidad'));
        return { ProductoId: num(c(f, 'ProductoId')), Producto: String(c(f, 'Producto', 'NombreProducto') ?? ''), Cantidad: cant,
            Cajas: num(c(f, 'CantidadCajas')) || cant / conv, PrecioCosto: num(c(f, 'PrecioCosto')), Total: num(c(f, 'TotalCompra', 'Total')) };
    }) });
}));
// Creditos Pendientes (@tipo 1)
router.get('/pendientes', asyncHandler(async (req, res) => {
    const c = ['Id', 'Credito', 'Compra', 'NombrePersonal', 'NombreProveedor', 'Monto', 'abonado', 'Restante', 'FechaVencimientoCredito', 'DiasMora'];
    res.json({ ok: true, data: conFechas(v.proyectar(await L.L_prListarPagosPendientes(), c), ['FechaVencimientoCredito']) });
}));
// Creditos Pagados (@tipo 2)
router.get('/pagados', asyncHandler(async (req, res) => {
    const c = ['Id', 'Credito', 'Compra', 'NombreProveedor', 'Monto', 'Pagado', 'FechaVencimientoCredito', 'FechaUltimaPago'];
    res.json({ ok: true, data: conFechas(v.proyectar(await L.L_prListarCreditosPagados(), c), ['FechaVencimientoCredito', 'FechaUltimaPago']) });
}));
// Personal que registra el pago (por defecto el del usuario)
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
    const c = ['Id', 'FechaPago', 'Monto', 'Glosa', 'NroComprobante', 'NombrePersonal'];
    res.json({ ok: true, data: conFechas(v.proyectar(await L.L_prListarPagosCreditoCompra(id), c), ['FechaPago']) });
}));
// Estado de cuenta (@tipo 8): columnas del reporte, sin imagenes
router.get('/:id/estado', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const dt = await L.L_EstadoDeCuentasPorPagar(id);
    const limpio = dt.map((f) => Object.fromEntries(Object.entries(f).filter(([, x]) => !Buffer.isBuffer(x)).map(([k, x]) => [k, x instanceof Date ? iso(x) : x])));
    res.json({ ok: true, data: limpio });
}));

// ButtonX4 "Pagar": ValidarCamposaGrabar + L_prGrabarPagosCreditoCompras
router.post('/:id/pagos', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const b = req.body || {};
    const monto = Math.round(v.decimal(b.Monto) * 100) / 100;
    if (!(monto > 0)) throw new ErrorNegocio('Los Siguientes Campos Son Requerido : El monto a Pagar Debe ser Mayor a 0');
    const fecha = v.texto(b.FechaPago, 10);
    if (!/^\d{4}-\d{2}-\d{2}$/.test(fecha) || Number.isNaN(Date.parse(fecha))) throw new ErrorNegocio('Debe Seleccionar Fecha');
    const personal = v.entero(b.PersonalId, 'Personal');
    // La deuda debe seguir pendiente y el pago no puede superar el saldo
    const deuda = v.proyectar(await L.L_prListarPagosPendientesFiltros(), CAMPOS_DEUDA).find((x) => Number(x.Id) === id);
    if (!deuda) throw new ErrorNegocio('La deuda ya no esta pendiente. Actualice la lista.');
    if (monto > num(deuda.Restante) + 0.005) throw new ErrorNegocio(`El monto Maximo para Pagar es ${num(deuda.Restante).toFixed(2)}`.toUpperCase());
    const r = await L.L_prGrabarPagosCreditoCompras('', id, fecha.replace(/-/g, '/'), personal, v.texto(b.Glosa, 300), v.texto(b.NroComprobante, 50), monto);
    if (!r.resultado) throw new ErrorNegocio('Error al Grabar el Pago');
    res.json({ ok: true, id: r._numi, saldo: Math.round((num(deuda.Restante) - monto) * 100) / 100, mensaje: 'El Pago Ha sido Registrado con Exito' });
}));

// Administrar Pagos: eliminar un pago (@tipo 7)
router.delete('/pagos/:idPago', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.idPago);
    const ok = await L.L_prEliminarPagosCuentaPorPagar(id);
    if (!ok) throw new ErrorNegocio('No se pudo eliminar el pago.');
    res.json({ ok: true, mensaje: 'El pago ha sido Eliminado con Exito'.toUpperCase() });
}));

module.exports = router;
