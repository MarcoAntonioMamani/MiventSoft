// =====================================================================
//  Respuestas JSON grandes de la API comprimidas con gzip (zlib de Node, sin dependencias).
//  Las listas de productos con stock pesan cientos de KB: comprimidas viajan ~5-8 veces mas livianas
//  por WiFi al celular. No guarda nada: cada pedido sigue yendo a la BD (Cache-Control: no-store).
//  Nivel 1 = el mas rapido; corre en el pool de hilos (no bloquea al resto de pedidos).
// =====================================================================
const zlib = require('node:zlib');

const MINIMO = 8 * 1024; // las respuestas chicas no ganan nada

function comprimirJson(req, res, next) {
    if (!/\bgzip\b/i.test(req.headers['accept-encoding'] || '')) return next();
    const enviarJson = res.json.bind(res);
    res.json = (cuerpo) => {
        let texto;
        try { texto = JSON.stringify(cuerpo); } catch { return enviarJson(cuerpo); }
        if (texto === undefined || texto.length < MINIMO || res.headersSent) return enviarJson(cuerpo);
        zlib.gzip(texto, { level: 1 }, (err, buf) => {
            if (res.headersSent) return;
            if (err) { enviarJson(cuerpo); return; }
            res.set('Content-Type', 'application/json; charset=utf-8');
            res.set('Content-Encoding', 'gzip');
            res.set('Content-Length', String(buf.length));
            res.vary('Accept-Encoding');
            res.end(buf);
        });
        return res;
    };
    next();
}

module.exports = { comprimirJson };
