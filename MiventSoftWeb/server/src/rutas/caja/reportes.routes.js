// =====================================================================
//  KAILY LA PAZ: reportes de Administrar Caja (cada uno con su programa)
//   caja-general     Tec_CajaGeneral          btnCajaGeneral          MAM_CajaIngresoEgreso 6 (saldo anterior) + 7 (movimientos)
//                    y TotalCajaGeneral de la tabla Configuracion (L_ObtenerConfiguraciones)
//   caja-detallado   Tec_ReporteCajaDetallado btnReporteCajaDetallado MAM_CajaIngresoEgreso 13 (caja -1 = Todos) + cajas @tipo 4
//  (Ingresos/egresos mensuales y detallado usan los reportes de distribucion/reportes.routes.js, mismos SP)
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { rango, col, num, limpiar, fechaIso } = require('../distribucion/comun');

const router = express.Router();

async function cabeceraImpresa() {
    const [empresas, imagen] = await Promise.all([L.L_prListarGeneral('MAM_Empresas').catch(() => []), L.ObtenerImagenEmpresa().catch(() => [])]);
    return { empresa: v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Ciudad'])[0] || null, imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '' };
}

// P_CalcularTotalizador: fila "Saldo Inicial" (Σ Ingreso + Σ Egreso anteriores; los egresos vienen en negativo)
// y saldo acumulado fila por fila
router.get('/caja-general', requierePrograma('btnCajaGeneral'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const [anterior, movs, conf, cabecera] = await Promise.all([
        L.L_fnObtenerHistorialCajaGeneral(d), L.L_fnObtenerHistorialCajaGeneralDesdeHasta(d, h), L.L_ObtenerConfiguraciones().catch(() => []), cabeceraImpresa(),
    ]);
    let saldo = anterior.reduce((s, f) => s + num(col(f, 'Ingreso')) + num(col(f, 'Egreso')), 0);
    const filas = [{ Id: 0, Fecha: d.replace(/\//g, '-'), TipoMovimiento: 'Saldo Inicial', Descripcion: `Saldo A la Fecha ${d.split('/').reverse().join('/')}`, Ingreso: 0, Egreso: 0, Saldo: saldo }];
    for (const f of movs) {
        const ingreso = num(col(f, 'IngresoEgreso')) === 1;
        saldo += ingreso ? num(col(f, 'Ingreso')) : num(col(f, 'Egreso'));
        filas.push({ Id: num(col(f, 'Id')), Fecha: fechaIso(col(f, 'Fecha')), TipoMovimiento: String(col(f, 'TipoMovimiento') ?? ''), Descripcion: String(col(f, 'Descripcion') ?? ''),
            Ingreso: num(col(f, 'Ingreso')), Egreso: num(col(f, 'Egreso')), Saldo: saldo });
    }
    res.json({ ok: true, data: { filas, sinMovimientos: !movs.length, saldoCajaGeneral: conf.length ? num(col(conf[0], 'TotalCajaGeneral')) : null, cabecera } });
}));

router.get('/caja-detallado/cajas', requierePrograma('btnReporteCajaDetallado'), asyncHandler(async (req, res) => {
    const cajas = v.proyectar(await L.L_prListarCaja(), ['Id', 'NombreCaja']);
    cajas.push({ Id: -1, NombreCaja: 'Todos' }); // dt.Rows.Add(-1, "Todos")
    res.json({ ok: true, data: cajas });
}));
router.get('/caja-detallado', requierePrograma('btnReporteCajaDetallado'), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const caja = Number.parseInt(req.query.caja, 10);
    const [dt, cabecera] = await Promise.all([L.ReporteIngresoEgresoDetalladoCaja(d, h, Number.isFinite(caja) ? caja : -1), cabeceraImpresa()]);
    res.json({ ok: true, data: { filas: limpiar(dt).map((f) => { const o = { ...f }; delete o.img; return o; }), cabecera } });
}));

module.exports = router;
