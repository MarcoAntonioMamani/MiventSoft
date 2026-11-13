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

/**
 * DB_SERVER como se escribe en SSMS / dbConnect.TXT del escritorio:
 *   "host", "host,puerto", "tcp:host,puerto", "EQUIPO\INSTANCIA", "EQUIPO\INSTANCIA,puerto", ".", "(local)".
 * El puerto escrito junto al host manda sobre DB_PORT; la instancia, sobre DB_INSTANCE vacio.
 */
function _servidor() {
    let host = _requerido('DB_SERVER').trim().replace(/^tcp:/i, '');
    let puerto = (process.env.DB_PORT || '').trim();
    let instancia = (process.env.DB_INSTANCE || '').trim();
    const conPuerto = /^(.*),\s*(\d+)$/.exec(host);
    if (conPuerto) { host = conPuerto[1].trim(); puerto = conPuerto[2]; }
    const conInstancia = /^(.*)\\(.+)$/.exec(host);
    if (conInstancia) { host = conInstancia[1]; if (!instancia) instancia = conInstancia[2]; }
    if (['.', '(local)', '(localdb)', ''].includes(host.toLowerCase())) host = 'localhost';
    const nPuerto = Number(puerto || 1433);
    if (!Number.isInteger(nPuerto) || nPuerto < 1 || nPuerto > 65535) {
        throw new Error(`Puerto de SQL Server no valido: "${puerto}" (revisa DB_SERVER / DB_PORT en server/.env)`);
    }
    // con puerto explicito (host,puerto) se conecta directo; sin el, la instancia nombrada se resuelve via SQL Browser
    return { host, puerto: nPuerto, instancia: conPuerto ? '' : instancia };
}

const Configuracion = {
    get CadenaConexion() {
        const srv = _servidor();
        const cfg = {
            server: srv.host,
            user: _requerido('DB_USER'),
            password: _requerido('DB_PASSWORD'),
            database: _requerido('DB_NAME'),
            port: srv.puerto,
            options: {
                encrypt: process.env.DB_ENCRYPT === 'true',
                trustServerCertificate: process.env.DB_TRUST_CERT !== 'false',
                enableArithAbort: true,
                // APP_NAME() en SQL: la auditoria (MAM_Auditoria) distingue la web del escritorio
                appName: 'MiventSoftWeb',
            },
            pool: { max: 20, min: 0, idleTimeoutMillis: 30000 },
            requestTimeout: 60000,
        };
        if (srv.instancia) {
            cfg.options.instanceName = srv.instancia;
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
