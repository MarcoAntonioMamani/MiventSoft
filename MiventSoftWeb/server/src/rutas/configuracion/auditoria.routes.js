// =====================================================================
//  SOLO WEB: Auditoria De Registros (programa: btnAuditoriaRegistros)   SP: MAM_Auditoria (sql/07_Auditoria.sql)
//  Lo MODIFICADO y ELIMINADO de ventas, compras y movimientos de inventario, hecho desde la web o el escritorio
//  (los SP lo registran, script 08): quien, cuando, desde que equipo, y la comparacion antes / despues.
//   * Lista: @tipo 3 por fecha de la operacion, modulo, operacion (todo / modificados / eliminados), usuario y documento.
//   * Detalle: @tipo 4. Las fotos vienen en XML (<foto><cab .../><det .../></foto>) y aqui se comparan campo por campo;
//     el detalle se empareja por producto + lote: igual / modificado / agregado / quitado.
//  Solo lectura: la tabla AuditoriaRegistros no se puede modificar ni borrar.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { PRECISION_IP } = require('../../middleware/clienteAuditoria');
const { rango, num, fechaIso, limpiar } = require('../distribucion/comun');

const router = express.Router();
router.use(requierePrograma('btnAuditoriaRegistros'));

// empresa y logo para la nota impresa / PDF del registro (igual que los reportes de caja)
async function cabeceraImpresa() {
    const [empresas, imagen] = await Promise.all([L.L_prListarGeneral('MAM_Empresas').catch(() => []), L.ObtenerImagenEmpresa().catch(() => [])]);
    return { empresa: v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Ciudad'])[0] || null, imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '' };
}

const MODULOS = { 1: 'Venta', 2: 'Compra', 3: 'Movimiento' };
const OPERACIONES = { 1: 'Modificado', 2: 'Eliminado' };
// APP_NAME() de la conexion: la web se conecta como MiventSoftWeb (datos/Configuracion.js)
const origen = (app) => (/MiventSoftWeb/i.test(String(app || '')) ? 'Web' : 'Escritorio');

// Campos que se muestran de cada foto: [atributo, titulo, tipo]  (tipo: texto | num | fecha | sino)
const CAMPOS = {
    1: {
        cab: [['Fecha', 'Fecha', 'fecha'], ['Sucursal', 'Sucursal'], ['Cliente', 'Cliente'], ['Personal', 'Vendedor'], ['TipoVenta', 'Tipo de venta'],
            ['VencimientoCredito', 'Vence credito', 'fecha'], ['Glosa', 'Glosa'], ['Facturado', 'Facturado', 'sino'], ['Descuento', 'Descuento', 'num'], ['Total', 'Total', 'num']],
        det: [['Producto', 'Producto'], ['Cantidad', 'Cantidad', 'num'], ['Precio', 'Precio', 'num'], ['Descuento', 'Descuento', 'num'], ['Total', 'Total', 'num']],
    },
    2: {
        cab: [['Fecha', 'Fecha', 'fecha'], ['Almacen', 'Almacen'], ['Proveedor', 'Proveedor'], ['TipoCompra', 'Tipo de compra'],
            ['VencimientoCredito', 'Vence credito', 'fecha'], ['Glosa', 'Glosa'], ['Descuento', 'Descuento', 'num'], ['Total', 'Total', 'num']],
        det: [['Producto', 'Producto'], ['Cantidad', 'Cantidad', 'num'], ['PrecioCosto', 'P. Costo', 'num'],
            ['PrecioVenta', 'P. Venta', 'num'], ['Total', 'Total', 'num']],
    },
    3: {
        cab: [['Fecha', 'Fecha', 'fecha'], ['Concepto', 'Concepto'], ['Deposito', 'Deposito'], ['DepositoDestino', 'Deposito destino'],
            ['IdMovimientoDestino', 'Movimiento destino'], ['Observacion', 'Observacion'], ['Total', 'Cantidad total', 'num']],
        det: [['Producto', 'Producto'], ['Cantidad', 'Cantidad', 'num']],
    },
};

// ---------- fotos XML (FOR XML RAW: atributos con &amp; &lt; &gt; &quot; &#xD; ...) ----------
const ENT = { amp: '&', lt: '<', gt: '>', quot: '"', apos: "'" };
const decodificar = (s) => s.replace(/&(#x[0-9a-f]+|#\d+|amp|lt|gt|quot|apos);/gi, (m, e) => {
    if (e[0] !== '#') return ENT[e.toLowerCase()];
    return String.fromCodePoint(e[1] === 'x' || e[1] === 'X' ? parseInt(e.slice(2), 16) : parseInt(e.slice(1), 10));
});
function leerFoto(xml) {
    if (!xml) return null;
    const foto = { cab: null, det: [] };
    for (const m of String(xml).matchAll(/<(cab|det)\b([^>]*?)\/?>/g)) {
        const fila = {};
        for (const a of m[2].matchAll(/([\w.-]+)="([^"]*)"/g)) fila[a[1]] = decodificar(a[2]);
        if (m[1] === 'cab') foto.cab = fila; else foto.det.push(fila);
    }
    return foto;
}

// Valor comparable segun el tipo (los decimales vienen como "12.50" y "12.5" segun la columna)
function normal(x, tipo) {
    if (x === undefined || x === null || x === '') return '';
    if (tipo === 'num') return String(num(x));
    if (tipo === 'fecha') return fechaIso(x) || String(x);
    if (tipo === 'sino') return num(x) === 1 ? 'Si' : 'No';
    return String(x).trim();
}
const proyectarFila = (f, campos) => (f ? Object.fromEntries(campos.map(([k, , t]) => [k, normal(f[k], t)])) : null);

/** Empareja el detalle por producto + lote (en orden) y marca cada linea */
function compararDetalle(antes, despues, campos) {
    const clave = (f) => `${f.ProductoId ?? ''}|${f.Lote ?? ''}`;
    const libres = antes.map((f, i) => ({ f, i, usado: false }));
    const filas = [];
    for (const d of despues) {
        const a = libres.find((x) => !x.usado && clave(x.f) === clave(d));
        if (a) a.usado = true;
        const pa = proyectarFila(a?.f, campos); const pd = proyectarFila(d, campos);
        const cambios = pa ? campos.map(([k]) => k).filter((k) => pa[k] !== pd[k]) : [];
        filas.push({ estado: !pa ? 'agregado' : cambios.length ? 'modificado' : 'igual', antes: pa, despues: pd, cambios });
    }
    for (const a of libres.filter((x) => !x.usado)) filas.push({ estado: 'quitado', antes: proyectarFila(a.f, campos), despues: null, cambios: [] });
    return filas;
}

// Ubicacion (sql/10): solo si vino del GPS. Mapa: enlace a OpenStreetMap (no se carga nada externo en la pantalla)
const ubicacion = (f) => {
    if (f.Latitud == null || f.Longitud == null) return null;
    const lat = num(f.Latitud); const lng = num(f.Longitud);
    const precision = f.PrecisionMetros == null ? null : num(f.PrecisionMetros);
    const aproximada = precision != null && precision >= PRECISION_IP; // por IP publica (no GPS)
    // texto exacto con todos los decimales guardados (sql/12: LatitudTexto / LongitudTexto); sin el 12, el numero
    const txt = (t, n) => (t != null ? String(t).replace(/(\.\d*?)0+$/, '$1').replace(/\.$/, '') : String(n));
    return { lat, lng, latTexto: txt(f.LatitudTexto, lat), lngTexto: txt(f.LongitudTexto, lng), precision, aproximada,
        mapa: `https://www.openstreetmap.org/?mlat=${lat}&mlon=${lng}#map=${aproximada ? 12 : 18}/${lat}/${lng}` };
};
// Ip guardada como "local | publica" (middleware/clienteAuditoria.js)
const ips = (v) => { const [a, b] = String(v || '').split('|').map((x) => x.trim()); return { IpLocal: b ? a : (a && /^(10\.|192\.168\.|172\.(1[6-9]|2\d|3[01])\.|127\.)/.test(a) ? a : ''), IpPublica: b || (a && !/^(10\.|192\.168\.|172\.(1[6-9]|2\d|3[01])\.|127\.)/.test(a) ? a : '') }; };

const fila = (f) => {
    const o = limpiar([f])[0];
    const { Latitud, Longitud, PrecisionMetros, LatitudTexto, LongitudTexto, ...resto } = o;
    return {
        ...resto, FechaDocumento: fechaIso(f.FechaDocumento), NombreModulo: MODULOS[num(f.Modulo)] || '', NombreOperacion: OPERACIONES[num(f.Operacion)] || '',
        Origen: origen(f.Aplicacion), TotalAntes: f.TotalAntes == null ? null : num(f.TotalAntes), TotalDespues: f.TotalDespues == null ? null : num(f.TotalDespues),
        Ip: f.Ip || '', ...ips(f.Ip), Navegador: f.Navegador || '', Ubicacion: ubicacion(f),
    };
};

// Lista + resumen. modulo / operacion: 0 = todos ; usuario: '' = todos ; documento: Id (opcional)
router.get('/', asyncHandler(async (req, res) => {
    const [desde, hasta] = rango(req.query);
    const modulo = [1, 2, 3].includes(Number(req.query.modulo)) ? Number(req.query.modulo) : -1;
    const operacion = [1, 2].includes(Number(req.query.operacion)) ? Number(req.query.operacion) : -1;
    const documento = Number.parseInt(req.query.documento, 10) > 0 ? Number.parseInt(req.query.documento, 10) : -1;
    const usuario = v.texto(req.query.usuario, 100).trim();
    const filas = (await L.L_prAuditoriaListar(desde, hasta, modulo, operacion, usuario, documento)).map(fila);
    const cuenta = (k, x) => filas.filter((f) => num(f[k]) === x).length;
    res.json({
        ok: true,
        data: {
            filas,
            resumen: {
                total: filas.length, modificados: cuenta('Operacion', 1), eliminados: cuenta('Operacion', 2),
                ventas: cuenta('Modulo', 1), compras: cuenta('Modulo', 2), movimientos: cuenta('Modulo', 3),
                usuarios: new Set(filas.map((f) => f.Usuario)).size,
            },
        },
    });
}));

router.get('/usuarios', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: (await L.L_prAuditoriaUsuarios()).map((f) => String(f.Usuario)) });
}));

// Detalle de un registro (fila de @tipo 4): cabecera campo por campo y detalle linea por linea (antes / despues)
function armarDetalle(r) {
    const def = CAMPOS[num(r.Modulo)];
    if (!def) throw new ErrorNegocio('Modulo de auditoria desconocido');
    const antes = leerFoto(r.Antes); const despues = leerFoto(r.Despues);
    const ca = proyectarFila(antes?.cab, def.cab); const cd = proyectarFila(despues?.cab, def.cab);
    const cabecera = def.cab.map(([k, titulo, tipo = 'texto']) => ({
        campo: k, titulo, tipo, antes: ca ? ca[k] : null, despues: cd ? cd[k] : null, cambio: !!(ca && cd && ca[k] !== cd[k]),
    })).filter((c) => c.antes || c.despues); // sin campos vacios en los dos lados
    const detalle = despues ? compararDetalle(antes?.det || [], despues.det, def.det)
        : (antes?.det || []).map((x) => ({ estado: 'eliminado', antes: proyectarFila(x, def.det), despues: null, cambios: [] }));
    const { Antes, Despues, ...resto } = r;
    return {
        registro: fila(resto),
        cabecera,
        columnas: def.det.map(([k, titulo, tipo = 'texto']) => ({ campo: k, titulo, tipo })),
        detalle,
        resumenDetalle: {
            agregados: detalle.filter((d) => d.estado === 'agregado').length,
            quitados: detalle.filter((d) => d.estado === 'quitado').length,
            modificados: detalle.filter((d) => d.estado === 'modificado').length,
        },
    };
}

// Reporte: los registros de la lista (mismos filtros; ids = los que quedaron tras la busqueda) con TODO su detalle
const REPORTE_MAX = 300;
router.get('/reporte', asyncHandler(async (req, res) => {
    const [desde, hasta] = rango(req.query);
    const modulo = [1, 2, 3].includes(Number(req.query.modulo)) ? Number(req.query.modulo) : -1;
    const operacion = [1, 2].includes(Number(req.query.operacion)) ? Number(req.query.operacion) : -1;
    const usuario = v.texto(req.query.usuario, 100).trim();
    const ids = String(req.query.ids || '').split(',').map((x) => Number.parseInt(x, 10)).filter((x) => x > 0);
    let lista = await L.L_prAuditoriaListar(desde, hasta, modulo, operacion, usuario, -1);
    if (ids.length) { const set = new Set(ids); lista = lista.filter((x) => set.has(num(x.Id))); }
    if (!lista.length) throw new ErrorNegocio('No hay registros para el reporte con los filtros actuales.');
    if (lista.length > REPORTE_MAX) throw new ErrorNegocio(`El reporte tiene ${lista.length} registros; el maximo es ${REPORTE_MAX}. Acorte el rango de fechas o use los filtros.`);
    // detalle de cada uno (@tipo 4), de a 5 a la vez para no cargar la BD
    const registros = new Array(lista.length);
    for (let i = 0; i < lista.length; i += 5) {
        const parte = await Promise.all(lista.slice(i, i + 5).map((x) => L.L_prAuditoriaDetalle(num(x.Id))));
        parte.forEach((dt, j) => { registros[i + j] = dt[0] ? armarDetalle(dt[0]) : null; });
    }
    res.json({ ok: true, data: { ...(await cabeceraImpresa()), desde, hasta, registros: registros.filter(Boolean) } });
}));

router.get('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const [dt, impresa] = await Promise.all([L.L_prAuditoriaDetalle(id), cabeceraImpresa()]);
    if (!dt[0]) throw new ErrorNegocio('No se encontro el registro de auditoria. Actualice la lista.', 404);
    res.json({ ok: true, data: { ...armarDetalle(dt[0]), ...impresa } });
}));

module.exports = router;
