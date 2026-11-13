// =====================================================================
//  Manejo de errores centralizado (reemplaza los MsgBox(ex.Message) de MetodoDatos.vb)
//  - Errores de negocio lanzados desde los SP con RAISERROR/THROW (numero >= 50000)
//    se muestran tal cual al usuario (como el ToastNotification rojo).
//  - Cualquier otro error se registra en el servidor y al navegador va un mensaje
//    generico + un codigo de traza (no se exponen detalles internos de SQL).
// =====================================================================
const crypto = require('node:crypto');

class ErrorNegocio extends Error {
    constructor(mensaje, status = 400) {
        super(mensaje);
        this.status = status;
        this.esNegocio = true;
    }
}

// Envuelve handlers async: errores -> manejadorErrores, y re-establece L_Usuario
// (por si un middleware basado en streams, como multer, perdio el contexto async)
const asyncHandler = (fn) => (req, res, next) => {
    const { L_prConUsuario } = require('../negocio/AccesoLogica');
    const ejecutar = () => Promise.resolve(fn(req, res, next)).catch(next);
    return req.sesion ? L_prConUsuario(req.sesion.gs_user, ejecutar, req.clienteAud) : ejecutar();
};

// Mensajes de stock de los SP (05 StockSeguro y 09 CompraEliminarStock):
//   "STOCK INSUFICIENTE: Prod (stock actual 0.00, necesita 20.00); ..."
//   "NO SE PUEDE ELIMINAR LA COMPRA, EL STOCK QUEDARIA NEGATIVO: Prod (stock actual 0.00, la compra ingreso 20.00); ..."
// -> [{ Producto, stock, cantidad, quedaria }] para mostrarlos en una tabla (null si el mensaje es otro)
const RE_STOCK = /^(STOCK INSUFICIENTE|NO SE PUEDE ELIMINAR[^:]*STOCK[^:]*):\s*(.+)$/is;
const RE_ITEM = /^(.*) \(stock actual (-?[\d.]+), (?:necesita|la compra ingreso) (-?[\d.]+)\)$/is;
function productosStock(mensaje) {
    const m = RE_STOCK.exec(String(mensaje || '').trim());
    if (!m) return null;
    const productos = m[2].split('; ').map((x) => RE_ITEM.exec(x.trim())).filter(Boolean)
        .map(([, Producto, s, c]) => ({ Producto: Producto.trim(), stock: Number(s), cantidad: Number(c), quedaria: Math.round((Number(s) - Number(c)) * 100) / 100 }));
    return productos.length ? productos : null;
}

// eslint-disable-next-line no-unused-vars
function manejadorErrores(err, req, res, next) {
    if (err.esNegocio) {
        // datos: detalle para la pantalla (ej. { sinStock: [{ ProductoId, Producto, stock, solicitado }] })
        return res.status(err.status).json({ ok: false, mensaje: err.message, ...(err.datos ? { datos: err.datos } : {}) });
    }
    const numeroSql = err.number || err.originalError?.info?.number;
    if (numeroSql && numeroSql >= 50000) {
        const productos = productosStock(err.message);
        if (productos && req.method === 'DELETE') {
            // eliminar un ingreso / compra cuyo stock ya se vendio o salio: mensaje claro + la tabla de productos
            return res.status(409).json({ ok: false, mensaje: 'No se puede eliminar: parte de lo que ingreso este documento ya se vendio o salio del inventario. Si se elimina, el stock quedaria en negativo.',
                datos: { stockNegativo: productos } });
        }
        return res.status(400).json({ ok: false, mensaje: err.message, ...(productos ? { datos: { stockNegativo: productos } } : {}) });
    }
    if (err.code === 'LIMIT_FILE_SIZE') {
        return res.status(400).json({ ok: false, mensaje: 'La imagen supera el tamano maximo permitido (3 MB).' });
    }
    const traza = crypto.randomUUID().slice(0, 8);
    // Se registra el error pero NO el cuerpo de la peticion (puede contener contrasenas)
    console.error(`[${new Date().toISOString()}] [traza ${traza}] ${req.method} ${req.originalUrl} ->`, err.message);
    if (process.env.NODE_ENV !== 'production') console.error(err.stack);
    const mensaje =
        err.code === 'ESOCKET' || err.code === 'ELOGIN' || err.code === 'ETIMEOUT'
            ? 'No se pudo conectar con la base de datos. Verifique el servidor SQL.'
            : 'Ocurrio un error inesperado al procesar la solicitud.';
    res.status(500).json({ ok: false, mensaje, traza });
}

module.exports = { ErrorNegocio, asyncHandler, manejadorErrores, productosStock };
