// =====================================================================
//  Datos/AccesoDatos.js   (equivalente a Datos/AccesoDatos.vb)
//  Misma API: D_ProcedimientoConParam(nombreSp, listaParametros)
// =====================================================================
const _modoMock =
    (process.env.MIVENT_MOCK === '1' || process.argv.includes('--mock')) && process.env.NODE_ENV !== 'production';
const MetodoDatos = _modoMock ? require('./MetodoDatos.mock') : require('./MetodoDatos');

if (_modoMock) {
    console.warn('[AccesoDatos] *** MODO MOCK: datos de prueba en memoria, SIN SQL Server ***');
}

async function D_abrirConexion() {
    return MetodoDatos.CrearConexion();
}

async function D_ProcedimientoConParam(_Nombre, _listaParam) {
    return MetodoDatos.EjecutarProcedimiento(_Nombre, _listaParam);
}

async function D_Procedimiento(_Nombre) {
    return MetodoDatos.EjecutarProcedimiento(_Nombre, []);
}

// Consulta parametrizada (solo para los pocos metodos que en VB usaban D_Datos_Tabla)
async function D_ConsultaParam(_Sql, _listaParam) {
    return MetodoDatos.EjecutarConsulta(_Sql, _listaParam);
}

// Servidor y base a los que REALMENTE esta conectado el pool (no la configuracion).
// Se consulta una vez por proceso. Nunca devuelve usuario ni clave.
let _infoConexion = null;
async function D_InfoConexion() {
    if (_modoMock) return { servidor: 'SIMULADOR', bd: 'DATOS DE PRUEBA', mock: true };
    if (_infoConexion) return _infoConexion;
    const cfgServidor = [process.env.DB_SERVER, process.env.DB_INSTANCE].filter(Boolean).join('\\');
    try {
        const r = await MetodoDatos.EjecutarConsulta('SELECT @@SERVERNAME AS servidor, DB_NAME() AS bd', []);
        _infoConexion = { servidor: String(r[0]?.servidor || cfgServidor), bd: String(r[0]?.bd || process.env.DB_NAME || ''), configurado: cfgServidor };
    } catch (e) {
        console.error('[AccesoDatos] No se pudo leer el nombre del servidor:', e.message);
        return { servidor: cfgServidor, bd: process.env.DB_NAME || '', configurado: cfgServidor };
    }
    return _infoConexion;
}

module.exports = { D_abrirConexion, D_ProcedimientoConParam, D_Procedimiento, D_ConsultaParam, D_InfoConexion };
