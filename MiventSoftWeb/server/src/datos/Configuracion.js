// =====================================================================
//  Datos/Configuracion.js   (equivalente a Datos/Configuracion.vb)
//  En VB la cadena de conexion venia de dbConnect.TXT (o estaba hardcodeada).
//  Aqui sale de variables de entorno (.env) -> nunca credenciales en el codigo.
// =====================================================================
require('dotenv').config();

function _requerido(nombre) {
    const v = process.env[nombre];
    if (v === undefined || v === '') {
        throw new Error(`Falta la variable de entorno ${nombre} (revisa server/.env)`);
    }
    return v;
}

const Configuracion = {
    get CadenaConexion() {
        const cfg = {
            server: _requerido('DB_SERVER'),
            user: _requerido('DB_USER'),
            password: _requerido('DB_PASSWORD'),
            database: _requerido('DB_NAME'),
            port: Number(process.env.DB_PORT || 1433),
            options: {
                encrypt: process.env.DB_ENCRYPT === 'true',
                trustServerCertificate: process.env.DB_TRUST_CERT !== 'false',
                enableArithAbort: true,
            },
            pool: { max: 20, min: 0, idleTimeoutMillis: 30000 },
            requestTimeout: 60000,
        };
        if (process.env.DB_INSTANCE) {
            cfg.options.instanceName = process.env.DB_INSTANCE;
            delete cfg.port; // con instancia nombrada se resuelve via SQL Browser
        }
        return cfg;
    },

    // gs_CarpetaRaiz
    get gs_CarpetaRaiz() {
        return process.env.CARPETA_RAIZ || 'C:\\BD';
    },
};

module.exports = Configuracion;
