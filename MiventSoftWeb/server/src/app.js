// =====================================================================
//  MiventSoft Web - API
//  Capas:  datos/ (AccesoDatos, MetodoDatos)  ->  negocio/ (AccesoLogica)  ->  rutas/
// =====================================================================
require('dotenv').config();
const path = require('node:path');
const fs = require('node:fs');
const express = require('express');
const helmet = require('helmet');
const cookieParser = require('cookie-parser');

const { requiereSesion } = require('./middleware/autenticacion');
const { manejadorErrores } = require('./middleware/errores');

const app = express();
app.disable('x-powered-by');
app.set('trust proxy', 1);

app.use(helmet({
    // Los servidores de mapas (OpenStreetMap) bloquean pedidos sin "Referer".
    // Se envia solo el origen (http://servidor:puerto), nunca la ruta ni datos.
    referrerPolicy: { policy: 'strict-origin-when-cross-origin' },
    contentSecurityPolicy: {
        directives: {
            defaultSrc: ["'self'"],
            imgSrc: ["'self'", 'data:', 'blob:', 'https://*.tile.openstreetmap.org', 'https://tile.openstreetmap.org', 'https://*.basemaps.cartocdn.com'],
            styleSrc: ["'self'", "'unsafe-inline'"],
            scriptSrc: ["'self'"],
            connectSrc: ["'self'"],
            // En la red local se usa http: no forzar https en los recursos
            upgradeInsecureRequests: process.env.COOKIE_SECURE === 'true' ? [] : null,
        },
    },
}));
app.use(express.json({ limit: '1mb' }));
app.use(cookieParser());

// CORS solo para el Vite de desarrollo (en produccion el front se sirve desde aqui mismo)
if (process.env.NODE_ENV !== 'production' && process.env.CORS_ORIGIN) {
    app.use((req, res, next) => {
        if (req.headers.origin === process.env.CORS_ORIGIN) {
            res.set('Access-Control-Allow-Origin', process.env.CORS_ORIGIN);
            res.set('Access-Control-Allow-Credentials', 'true');
            res.set('Access-Control-Allow-Headers', 'Content-Type');
            res.set('Access-Control-Allow-Methods', 'GET,POST,PUT,DELETE');
            if (req.method === 'OPTIONS') return res.sendStatus(204);
        }
        next();
    });
}

app.get('/api/salud', (req, res) => res.json({ ok: true, servicio: 'MiventSoft API' }));

// Datos siempre frescos de la BD: ninguna respuesta de la API se guarda en el navegador ni en proxies.
// (imagenes.routes lo reemplaza por su propio max-age: las imagenes no son datos de negocio)
app.use('/api', (req, res, next) => { res.set('Cache-Control', 'no-store'); next(); });
// JSON grandes (listas de productos con stock) comprimidos: llegan mas rapido al celular, sin cache
app.use('/api', require('./middleware/comprimir').comprimirJson);

app.use('/api/auth', require('./rutas/auth.routes'));
app.use('/api/imagenes', requiereSesion, require('./rutas/imagenes.routes'));
app.use('/api/dashboard', requiereSesion, require('./rutas/dashboard.routes'));

// ---- Modulo Configuracion ----
app.use('/api/configuracion/categorias', requiereSesion, require('./rutas/configuracion/categorias.routes'));
app.use('/api/configuracion/empresas', requiereSesion, require('./rutas/configuracion/empresas.routes'));
app.use('/api/configuracion/usuarios', requiereSesion, require('./rutas/configuracion/usuarios.routes'));
app.use('/api/configuracion/roles', requiereSesion, require('./rutas/configuracion/roles.routes'));
app.use('/api/configuracion/clientes', requiereSesion, require('./rutas/configuracion/clientes.routes'));
app.use('/api/configuracion/zonas', requiereSesion, require('./rutas/configuracion/zonas.routes'));
app.use('/api/configuracion/productos', requiereSesion, require('./rutas/configuracion/productos.routes'));
app.use('/api/configuracion/kits', requiereSesion, require('./rutas/configuracion/kits.routes'));
app.use('/api/configuracion/precios', requiereSesion, require('./rutas/configuracion/precios.routes'));
app.use('/api/configuracion/dosificacion', requiereSesion, require('./rutas/configuracion/dosificacion.routes')); // Kaily: MAM_Dosificacion
app.use('/api/tipo-cambio', requiereSesion, require('./rutas/tipoCambio.routes')); // Kaily: btnTipoCambio (ActualizarTipoCambio)
// Paso 3: modulo Ventas
app.use('/api/ventas/personal', requiereSesion, require('./rutas/ventas/personal.routes'));
app.use('/api/ventas/proveedores', requiereSesion, require('./rutas/ventas/proveedores.routes'));
// Kaily La Paz (punto de venta): ventas y proformas con crearRouterPV (ventasPuntoVenta.routes.js)
app.use('/api/ventas/ventas', requiereSesion, require('./rutas/ventas/ventasPuntoVenta.routes'));
app.use('/api/ventas/cuentas-cobrar', requiereSesion, require('./rutas/ventas/cuentasCobrar.routes'));
app.use('/api/ventas/reportes', requiereSesion, require('./rutas/ventas/reportes.routes'));
app.use('/api/ventas/proformas', requiereSesion, require('./rutas/ventas/proformas.routes'));
app.use('/api/ventas/despacho', requiereSesion, require('./rutas/ventas/despachoVentas.routes')); // Kaily: btnDespachoVenta (FrmDespacho)
app.use('/api/distribucion/ingresos-egresos', requiereSesion, require('./rutas/distribucion/ingresosEgresos.routes'));
// Kaily: Administrar Caja > Caja general y Reporte caja detallado
app.use('/api/caja/reportes', requiereSesion, require('./rutas/caja/reportes.routes'));
// Kaily: Administrar Caja > Cierre de caja del cajero (btnCierreCajero, MAM_CierreCajero)
app.use('/api/caja/cierre-cajero', requiereSesion, require('./rutas/caja/cierreCajero.routes'));
// Kaily: Administrar Caja > Registrar Gastos o Ingresos (btnGastos), mismo formulario y SP
app.use('/api/caja/ingresos-egresos', requiereSesion, require('./rutas/distribucion/ingresosEgresos.routes').crearRouterIE('btnGastos'));
app.use('/api/distribucion/reportes', requiereSesion, require('./rutas/distribucion/reportes.routes'));
app.use('/api/mapa/clientes', requiereSesion, require('./rutas/mapa/clientes.routes'));
app.use('/api/inventario/almacenes', requiereSesion, require('./rutas/inventario/almacenes.routes'));
app.use('/api/compras/compras', requiereSesion, require('./rutas/compras/compras.routes'));
app.use('/api/compras/cuentas-pagar', requiereSesion, require('./rutas/compras/cuentasPagar.routes'));
app.use('/api/planilla/conceptos-fijos', requiereSesion, require('./rutas/planilla/conceptosFijos.routes'));
app.use('/api/planilla/contratos', requiereSesion, require('./rutas/planilla/contratos.routes'));
app.use('/api/planilla/planilla', requiereSesion, require('./rutas/planilla/planilla.routes'));
app.use('/api/inventario/movimientos', requiereSesion, require('./rutas/inventario/movimientos.routes'));
app.use('/api/inventario/reportes', requiereSesion, require('./rutas/inventario/reportes.routes'));
app.use('/api/inventario/stock', requiereSesion, require('./rutas/inventario/stock.routes')); // pestaña fija "Inventario" (FormularioStock)

app.use('/api', (req, res) => res.status(404).json({ ok: false, mensaje: 'Recurso no encontrado' }));

// Front compilado (npm run build en /client) servido por el mismo Node
const dist = path.join(__dirname, '..', '..', 'client', 'dist');
if (fs.existsSync(dist)) {
    app.use(express.static(dist, { index: false }));
    app.get('*', (req, res) => res.sendFile(path.join(dist, 'index.html')));
}

app.use(manejadorErrores);

const PORT = Number(process.env.PORT || 3001);
const servidor = app.listen(PORT, async () => {
    console.log('');
    console.log('  ==============================================');
    console.log(`   Sanbit (Punto de Ventas) listo en  http://localhost:${PORT}`);
    console.log('   Para usarla desde otra PC o celular de la red:');
    for (const ifs of Object.values(require('node:os').networkInterfaces())) {
        for (const i of ifs || []) if (i.family === 'IPv4' && !i.internal) console.log(`     http://${i.address}:${PORT}`);
    }
    try {
        const c = await require('./datos/AccesoDatos').D_InfoConexion();
        console.log(`   Base de datos:  ${c.servidor}  /  ${c.bd}`);
    } catch { /* el detalle ya se registro */ }
    console.log('   (Ctrl+C para detener)');
    console.log('  ==============================================');
});
servidor.on('error', (e) => {
    if (e.code === 'EADDRINUSE') {
        console.error('');
        console.error(`  *** El puerto ${PORT} ya esta en uso: hay OTRA copia de Sanbit abierta (con la configuracion anterior).`);
        console.error('  *** Cierre esa ventana (o ejecute detener.bat) y vuelva a abrir iniciar.bat.');
        console.error('');
        process.exit(1);
    }
    throw e;
});
