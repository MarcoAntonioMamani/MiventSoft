// =====================================================================
//  Pedidos del celular observados (solo web, script 09 - MAM_PedidosMovilObservados)   btnPedidosObservados
//  Pedidos que la app no pudo subir: sin stock, rechazados por el servidor, sin productos o con fallas repetidas.
//   GET  /resumen            notificacion de la barra superior (cualquier usuario; sin el programa -> permitido:false)
//   GET  /                   listado (?desde&hasta&estado): pendientes siempre + resueltos del rango
//   GET  /:id                la observacion con el pedido (JSON del celular) y el stock actual de sus productos
//   POST /:id/grabar         corrige cantidades y graba el pedido con sp_go_TC004_appMovil @26 (como la API del celular)
//   POST /:id/descartar      descarta el pedido (motivo obligatorio). El celular lo ve en su proxima sincronizacion.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma, PermisosPrograma, accionPermiso } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio, productosStock } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { rango, col, num } = require('./comun');

const router = express.Router();
const PROGRAMA = 'btnPedidosObservados';

const TIPOS = { 1: 'SIN STOCK', 2: 'RECHAZADO', 3: 'SIN PRODUCTOS', 4: 'NO SE PUDO ENVIAR' };
const ESTADOS = { 0: 'PENDIENTE', 1: 'CORREGIDO EN WEB', 2: 'ENTRO DESDE EL CELULAR', 3: 'DESCARTADO' };

const dos = (n) => String(n).padStart(2, '0');
// datetime2 / date de SQL (mssql los entrega en UTC con la hora de la base): texto tal cual
const fechaHoraTxt = (x) => (x instanceof Date
    ? `${x.getUTCFullYear()}-${dos(x.getUTCMonth() + 1)}-${dos(x.getUTCDate())} ${dos(x.getUTCHours())}:${dos(x.getUTCMinutes())}`
    : (x ?? null));
const fechaTxt = (x) => (x instanceof Date ? `${x.getUTCFullYear()}-${dos(x.getUTCMonth() + 1)}-${dos(x.getUTCDate())}` : (x ?? null));
const redondear = (n, d = 2) => Math.round((Number(n) + Number.EPSILON) * 10 ** d) / 10 ** d;

function observacion(f) {
    const tipo = num(col(f, 'TipoObservacion'));
    const estado = num(col(f, 'Estado'));
    const incremento = col(f, 'Incremento');
    return {
        Id: col(f, 'Id'),
        CodigoGenerado: String(col(f, 'CodigoGenerado') ?? ''),
        TipoObservacion: tipo,
        Tipo: TIPOS[tipo] || `TIPO ${tipo}`,
        Motivo: String(col(f, 'Motivo') ?? ''),
        PersonalId: col(f, 'PersonalId'),
        Preventista: String(col(f, 'NombrePersonal') ?? '').trim(),
        ClienteId: col(f, 'ClienteId'),
        Cliente: String(col(f, 'NombreCliente') ?? '').trim(),
        Total: num(col(f, 'Total')),
        Incremento: incremento === null || incremento === undefined ? null : num(incremento),
        FechaPedido: fechaTxt(col(f, 'FechaPedido')),
        Estado: estado,
        EstadoTxt: ESTADOS[estado] || `ESTADO ${estado}`,
        VentaId: col(f, 'VentaId') ?? null,
        Intentos: num(col(f, 'Intentos')),
        FechaRegistro: fechaHoraTxt(col(f, 'FechaRegistro')),
        FechaActualizacion: fechaHoraTxt(col(f, 'FechaActualizacion')),
        UsuarioResuelve: String(col(f, 'UsuarioResuelve') ?? ''),
        FechaResuelto: fechaHoraTxt(col(f, 'FechaResuelto')),
        ObsResuelve: String(col(f, 'ObsResuelve') ?? ''),
        ModeloCelular: String(col(f, 'ModeloCelular') ?? ''),
    };
}

/** JSON del pedido tal como lo mando el celular (null si no se puede leer) */
function leerPedido(txt) {
    if (!txt) return null;
    try {
        const p = JSON.parse(txt);
        return p && typeof p === 'object' && !Array.isArray(p) ? p : null;
    } catch {
        return null;
    }
}
const detalleDe = (p) => (p && Array.isArray(p.detalle) ? p.detalle.filter((d) => d && typeof d === 'object') : []);

async function cargar(id) {
    const dt = await L.L_prPedidoObservado(id);
    if (!dt.length) throw new ErrorNegocio('La observacion no existe', 404);
    return { fila: dt[0], obs: observacion(dt[0]), pedido: leerPedido(col(dt[0], 'Pedido')) };
}

// ---------- notificacion: cantidad de pendientes ----------
let _avisoSinSp = false;
router.get('/resumen', asyncHandler(async (req, res) => {
    const p = await PermisosPrograma(req.sesion.gi_userRol, PROGRAMA);
    if (!p) return res.json({ ok: true, data: { permitido: false } });
    try {
        const f = (await L.L_prPedidosObservadosResumen())[0] || {};
        return res.json({ ok: true, data: {
            permitido: true, pendientes: num(col(f, 'Pendientes')), ultima: fechaHoraTxt(col(f, 'Ultima')),
            sinStock: num(col(f, 'SinStock')), otros: num(col(f, 'Otros')),
        } });
    } catch (e) {
        // script 09 sin instalar o base caida: la barra superior no muestra nada (no es un error para el usuario)
        if (!_avisoSinSp) { _avisoSinSp = true; console.error('[pedidos-observados] resumen no disponible:', e.message); }
        return res.json({ ok: true, data: { permitido: true, pendientes: 0, disponible: false } });
    }
}));

// ---------- listado ----------
router.get('/', requierePrograma(PROGRAMA), asyncHandler(async (req, res) => {
    const [d, h] = rango(req.query);
    const e = Number.parseInt(req.query.estado, 10);
    const estado = Number.isFinite(e) && e >= 0 && e <= 3 ? e : -1;
    res.json({ ok: true, data: (await L.L_prPedidosObservadosListar(d, h, estado)).map(observacion) });
}));

// ---------- una observacion con su pedido y el stock actual ----------
router.get('/:id', requierePrograma(PROGRAMA), asyncHandler(async (req, res) => {
    const { obs, pedido } = await cargar(v.idRuta(req.params.id));
    const det = detalleDe(pedido);
    const ids = [...new Set(det.map((x) => Number.parseInt(x.obcprod, 10)).filter((n) => n > 0))];
    const stock = new Map((ids.length ? await L.L_prPedidosObservadosStock(ids) : []).map((f) => [num(col(f, 'ProductoId')), num(col(f, 'Stock'))]));
    res.json({ ok: true, data: {
        ...obs,
        pedidoLegible: !!pedido,
        pedido: pedido ? {
            cliente: String(pedido.cliente ?? obs.Cliente ?? ''),
            oaobs: String(pedido.oaobs ?? ''),
            oahora: String(pedido.oahora ?? ''),
            ventaDirecta: num(pedido.VentaDirecta ?? pedido.ventaDirecta),
            tipocobro: num(pedido.tipocobro),
            incremento: num(pedido.incremento),
            total: num(pedido.total),
            fechaHoraPedido: String(pedido.fechaHoraPedido ?? ''),
        } : null,
        detalle: det.map((x) => {
            const id = Number.parseInt(x.obcprod, 10);
            return {
                obcprod: id, cadesc: String(x.cadesc ?? ''), obpcant: num(x.obpcant), obpbase: num(x.obpbase),
                descuento: num(x.descuento), total: num(x.total), stock: stock.has(id) ? stock.get(id) : null,
            };
        }),
    } });
}));

// ---------- grabar el pedido corregido ----------
// Escapa texto para XML (el detalle va a @ventas1 como <row><row>..</row></row>, igual que js2xmlparser en la API)
const escXml = (s) => String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&apos;');
function xmlDetalle(detalle) {
    const fila = (d) => Object.entries(d)
        .filter(([k, x]) => /^[A-Za-z_][\w.-]*$/.test(k) && (x === null || ['string', 'number', 'boolean'].includes(typeof x)))
        .map(([k, x]) => `<${k}>${x === null ? '' : escXml(x)}</${k}>`).join('');
    return `<row>${detalle.map((d) => `<row>${fila(d)}</row>`).join('')}</row>`;
}
const nOpc = (x, def) => { const n = Number(x); return (x === null || x === undefined || x === '' || !Number.isFinite(n)) ? def : n; };
const txt = (x, max) => String(x === null || x === undefined ? '' : x).slice(0, max);
const coordenada = (x, limite) => { const n = nOpc(x, 0); return Math.abs(n) <= limite ? n : 0; };
const fechaHoraCel = (x) => (/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$/.test(String(x || '')) ? String(x) : '');
// oafdoc del celular: Gson ("Nov 16, 2026 10:00:00 AM") o ISO. Si no se entiende, la fecha registrada de la observacion.
function fechaPedido(p, obs) {
    const s = String(p.oafdoc ?? '');
    const iso = /^(\d{4})-(\d{2})-(\d{2})/.exec(s);
    if (iso) return `${iso[1]}-${iso[2]}-${iso[3]}`;
    const d = new Date(s.replace(/ /g, ' '));
    if (s && !Number.isNaN(d.getTime())) return `${d.getFullYear()}-${dos(d.getMonth() + 1)}-${dos(d.getDate())}`;
    return obs.FechaPedido || null;
}

/** Los mismos parametros que arma WebServicesDynasys (controllers/client.js postPedidoMovil, rama /apiV4) */
function parametros26(p, detalle, total, obs) {
    const lista = [
        { nombre: 'tipo', valor: 26 },
        { nombre: 'oanumi', valor: txt(p.oanumi, 100) },
        { nombre: 'oafdoc', valor: fechaPedido(p, obs) },
        { nombre: 'oahora', valor: txt(p.oahora, 10) },
        { nombre: 'oaccli', valor: Number.parseInt(p.oaccli, 10) },
        { nombre: 'oarepa', valor: Number.parseInt(p.oarepa, 10) },
        { nombre: 'oaest', valor: Number.parseInt(p.oaest, 10) },
        { nombre: 'oaobs', valor: txt(p.oaobs, 200) },
        { nombre: 'total', valor: total },
        { nombre: 'latitud', valor: coordenada(p.latitud, 90) },
        { nombre: 'longitud', valor: coordenada(p.longitud, 99) },
        { nombre: 'tipocobro', valor: Number.parseInt(p.tipocobro, 10) },
        { nombre: 'codigogenerado', valor: txt(p.codigogenerado, 200) },
        { nombre: 'ventaDirecta', valor: Number.parseInt(p.VentaDirecta ?? p.ventaDirecta ?? 0, 10) || 0 },
        { nombre: 'Reclamo', valor: txt(p.reclamo, 200) },
        { nombre: 'ventas1', valor: xmlDetalle(detalle) },
        { nombre: 'body', valor: null },
    ];
    const incremento = Number(p.incremento);
    lista.push({ nombre: 'incremento', valor: Number.isFinite(incremento) && incremento >= 0 ? incremento : -1 });
    if (txt(p.modeloCelular, 120).trim()) { // agregarRegistroCelular(array, body, true)
        lista.push(
            { nombre: 'gpsPrecision', valor: Math.min(nOpc(p.precisionPedido, -1), 99999999) },
            { nombre: 'fechaHoraCelular', valor: fechaHoraCel(p.fechaHoraPedido) },
            { nombre: 'modeloCelular', valor: txt(p.modeloCelular, 120) },
            { nombre: 'versionAndroid', valor: txt(p.versionAndroid, 30) },
            { nombre: 'versionApp', valor: txt(p.versionApp, 30) },
            { nombre: 'latEntrega', valor: coordenada(p.latEntrega, 90) },
            { nombre: 'lngEntrega', valor: coordenada(p.lngEntrega, 99) },
            { nombre: 'precisionEntrega', valor: Math.min(nOpc(p.precisionEntrega, -1), 99999999) },
            { nombre: 'fechaHoraEntrega', valor: fechaHoraCel(p.fechaHoraEntrega) },
        );
    }
    return lista;
}

router.post('/:id/grabar', accionPermiso('modificar'), requierePrograma(PROGRAMA), asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const { obs, pedido } = await cargar(id);
    if (obs.Estado !== 0) throw new ErrorNegocio(`Este pedido ya no esta pendiente: ${obs.EstadoTxt.toLowerCase()}${obs.VentaId ? ` (venta ${obs.VentaId})` : ''}.`, 409);
    if (!pedido) throw new ErrorNegocio('No se pudo leer el pedido que mando el celular. Solo se puede descartar.', 409);
    const original = detalleDe(pedido);
    if (!original.length || obs.TipoObservacion === 3) throw new ErrorNegocio('El pedido no tiene productos: solo se puede descartar o corregir desde el celular.', 409);
    if (!pedido.codigogenerado || String(pedido.codigogenerado) !== obs.CodigoGenerado) throw new ErrorNegocio('El pedido guardado no corresponde a esta observacion.', 409);

    const yaEntro = await L.L_fnVentaPorCodigoCelular(obs.CodigoGenerado);
    if (yaEntro) throw new ErrorNegocio(`Ya entro desde el celular (venta ${yaEntro}).`, 409);

    // cantidades nuevas (0 = quitar). Lo que no venga queda como lo pidio el celular. No se agregan productos.
    const nuevas = new Map();
    for (const x of Array.isArray(req.body?.detalle) ? req.body.detalle : []) {
        const prod = Number.parseInt(x?.obcprod, 10);
        const cant = Number(x?.obpcant);
        if (!(prod > 0)) continue;
        if (!Number.isFinite(cant) || cant < 0 || cant > 999999) throw new ErrorNegocio('Ingrese cantidades validas (0 para quitar el producto).');
        nuevas.set(prod, redondear(cant, 2));
    }
    const detalle = original.map((d) => {
        const prod = Number.parseInt(d.obcprod, 10);
        const antes = num(d.obpcant);
        const cant = nuevas.has(prod) ? nuevas.get(prod) : antes;
        const precio = num(d.obpbase);
        const obptot = redondear(precio * cant, 2);
        const descuento = antes > 0 ? redondear(num(d.descuento) * cant / antes, 2) : 0;
        return { ...d, obpcant: cant, obptot, descuento, total: redondear(obptot - descuento, 2) };
    }).filter((d) => d.obpcant > 0);
    if (!detalle.length) throw new ErrorNegocio('El pedido debe quedar con al menos un producto. Para no grabarlo use Descartar.');
    const total = redondear(detalle.reduce((s, d) => s + d.total, 0), 2);
    const obsResuelve = v.texto(req.body?.observacion, 300);
    const corregido = { ...pedido, detalle, total, corregidoWeb: { usuario: L.L_Usuario(), fecha: new Date().toISOString() } };

    let ventaId;
    try {
        const r = await L.L_prPedidoCelularGrabar(parametros26(pedido, detalle, total, obs));
        ventaId = num(col(r[0] || {}, 'code_id'));
    } catch (e) {
        const productos = productosStock(e.message);
        if (productos) {
            const err = new ErrorNegocio(`No alcanza el stock, el pedido sigue observado. ${e.message}`, 409);
            err.datos = { stockNegativo: productos };
            throw err;
        }
        throw e; // RAISERROR del SP -> 400 con su mensaje; otros -> 500 con traza (manejadorErrores)
    }
    if (!(ventaId > 0)) throw new ErrorNegocio('El servidor no devolvio el numero de venta. Revise el listado antes de reintentar.', 500);

    let aviso = '';
    try {
        await L.L_prPedidoObservadoCorregido(id, ventaId, JSON.stringify(corregido), obsResuelve);
    } catch (e) {
        // la venta ya quedo grabada: la observacion se resuelve sola (Estado 2) en la proxima consulta
        console.error(`[pedidos-observados] venta ${ventaId} grabada, pero no se pudo marcar la observacion ${id}:`, e.message);
        aviso = 'La venta se grabo; la observacion se actualizara en la proxima consulta.';
    }
    console.log(`[pedidos-observados] observacion ${id} grabada desde la web como venta ${ventaId}`);
    res.json({ ok: true, ventaId, total, aviso, mensaje: `Pedido grabado como venta ${ventaId}. El celular lo vera corregido en su proxima sincronizacion.` });
}));

// ---------- descartar ----------
router.post('/:id/descartar', accionPermiso('modificar'), requierePrograma(PROGRAMA), asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const motivo = v.requerido(req.body?.motivo, 'Motivo', 300);
    const r = await L.L_prPedidoObservadoDescartar(id, motivo);
    if (!num(col(r[0] || {}, 'filas'))) throw new ErrorNegocio('No se pudo descartar: el pedido ya fue resuelto o ya entro desde el celular.', 409);
    console.log(`[pedidos-observados] observacion ${id} descartada desde la web`);
    res.json({ ok: true, mensaje: 'Pedido descartado. El celular lo vera en su proxima sincronizacion.' });
}));

module.exports = router;
