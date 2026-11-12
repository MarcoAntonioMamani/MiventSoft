// =====================================================================
//  Reportes de ingresos y egresos (Kaily: se abren desde Administrar Caja)
//   ingresos-egresos-anual  Tec_ReporteIngresosEgresosMensuales btnIngresoEgresosMensuales MAM_CajaIngresoEgreso 9 (x 12 meses, libreria 13 = años)
//   ingresos-egresos-det    Tec_ReporteIngresosEgresosDetallado btnIngresoEgresoDetallado  MAM_CajaIngresoEgreso 12
//  Rep_Gastos, Rep_Modificaciones, FProductosPorClientes, Rep_AuditoriaVentas y Rep_HistorialIncrementoVenta eran de
//  distribucion: sus SP (MAM_CierreCaja 10, MAM_Clientes 13, MAM_Ventas 31, MAM_AuditoriaVentas, MAM_VentasIncremento)
//  no estan en Negocio/AccesoLogica.vb de Kaily.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { rango, col, num, limpiar } = require('./comun');
const { libreria } = require('../configuracion/_libreria');

const router = express.Router();
const MESES = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];

// ---------- Ingresos / Egresos anual: Ingreso y Egreso por mes ----------
// Kaily: los mismos reportes se abren desde Administrar Caja (btnIngresoEgresosMensuales / btnIngresoEgresoDetallado)
const P_ANUAL = ['btnCierreReporteAnual', 'btnIngresoEgresosMensuales'];
const P_DETALLADO = ['btnCierreReporteDetallado', 'btnIngresoEgresoDetallado'];
router.get('/ingresos-egresos-anual/anios', requierePrograma(P_ANUAL), asyncHandler(async (req, res) => {
    res.json({ ok: true, data: await libreria(13) });
}));
// btnAnio (Efecto 10, libreria 13 "Crear Gestion(Año)"): solo años de 4 digitos, sin repetir
router.post('/ingresos-egresos-anual/anios', requierePrograma(P_ANUAL), asyncHandler(async (req, res) => {
    const anio = String(req.body?.descripcion ?? '').trim();
    if (!/^\d{4}$/.test(anio) || +anio < 2000 || +anio > 2100) throw new ErrorNegocio('Ingrese una gestión válida (año de 4 dígitos, ej. 2026)');
    if ((await libreria(13)).some((x) => String(x.cndesc1).trim() === anio)) throw new ErrorNegocio(`La gestión ${anio} ya existe.`);
    const r = await L.L_prClasificadorGrabar('', 13, anio);
    if (!r.resultado) throw new ErrorNegocio('No se pudo registrar la gestión.');
    res.json({ ok: true, id: r._numi, data: await libreria(13), mensaje: `GESTION ${anio} REGISTRADA CON EXITO.` });
}));
// cabecera de la hoja impresa (empresa y logo, mismos SP de solo lectura que los demas reportes)
async function cabeceraImpresa() {
    const [empresas, imagen] = await Promise.all([L.L_prListarGeneral('MAM_Empresas').catch(() => []), L.ObtenerImagenEmpresa().catch(() => [])]);
    return { empresa: v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Ciudad'])[0] || null, imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '' };
}
router.get('/ingresos-egresos-anual', requierePrograma(P_ANUAL), asyncHandler(async (req, res) => {
    const anio = Number.parseInt(req.query.anio, 10);
    if (!(anio >= 2000 && anio <= 2100)) throw new ErrorNegocio('Seleccione un Año');
    const [meses, cabecera] = await Promise.all([Promise.all(MESES.map((_, i) => L.ReporteIngresosEgresosMes(anio, i + 1))), cabeceraImpresa()]);
    const suma = (dt, ie) => dt.filter((f) => num(col(f, 'IngresoEgreso')) === ie).reduce((s, f) => s + num(col(f, 'Monto')), 0);
    const ing = meses.map((dt) => suma(dt, 1));
    const egr = meses.map((dt) => Math.abs(suma(dt, 0))); // MAM_CajaIngresoEgreso 9 trae los egresos en negativo (VB: Egreso * -1)
    const fila = (Concepto, vals) => ({ Concepto, ...Object.fromEntries(MESES.map((m, i) => [m, Math.round(vals[i] * 100) / 100])), Total: Math.round(vals.reduce((a, b) => a + b, 0) * 100) / 100 });
    res.json({ ok: true, data: { filas: [fila('INGRESOS', ing), fila('EGRESOS', egr), fila('SALDO', ing.map((x, i) => x - egr[i]))], grafico: MESES.map((m, i) => ({ mes: m, ingreso: ing[i], egreso: egr[i] })), cabecera } });
}));

router.get('/ingresos-egresos-detallado', requierePrograma(P_DETALLADO), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    // MAM_CajaIngresoEgreso 12: Modulo (tipo de ingreso/egreso), NroComprobante (Id del registro), Fecha, Monto, IngresoEgreso (1/0).
    // Los egresos vienen en negativo: se envian en positivo y el balance es ingresos - egresos.
    // Orden: primero ingresos, luego egresos; dentro, por tipo, fecha y comprobante.
    const [dt, cabecera] = await Promise.all([L.ReporteIngresoEgresoDetallado(d, h), cabeceraImpresa()]);
    const filas = limpiar(dt).map((f) => {
        const ing = num(col(f, 'IngresoEgreso')) === 1;
        return { Grupo: ing ? 'INGRESOS' : 'EGRESOS', Ingreso: ing, Modulo: String(col(f, 'Modulo') ?? '').trim() || '(Sin tipo)', NroComprobante: col(f, 'NroComprobante'), Fecha: col(f, 'Fecha'), Monto: Math.abs(num(col(f, 'Monto'))) };
    });
    const k = (f) => String(f.Fecha ?? '');
    filas.sort((a, b) => (b.Ingreso - a.Ingreso) || a.Modulo.localeCompare(b.Modulo, 'es') || k(a).localeCompare(k(b)) || num(a.NroComprobante) - num(b.NroComprobante));
    res.json({ ok: true, data: { filas, cabecera } });
}));

module.exports = router;
