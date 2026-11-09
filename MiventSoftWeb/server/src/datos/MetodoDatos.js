// =====================================================================
//  Datos/MetodoDatos.js   (equivalente a Datos/MetodoDatos.vb)
//
//  Diferencias importantes con el VB:
//   * En VB habia UN SqlCommand compartido (Shared). En web atendemos muchos
//     usuarios a la vez, asi que se usa un POOL de conexiones (thread-safe).
//   * En VB los errores se mostraban con MsgBox y se devolvia un DataTable vacio.
//     Aqui se lanzan (throw) y el middleware de errores responde al navegador.
// =====================================================================
const sql = require('mssql');
const Configuracion = require('./Configuracion');

let _pool = null;
let _conectando = null;

async function CrearConexion() {
    if (_pool && _pool.connected) return _pool;
    if (_conectando) return _conectando;
    _conectando = new sql.ConnectionPool(Configuracion.CadenaConexion)
        .connect()
        .then((p) => {
            _pool = p;
            _conectando = null;
            p.on('error', (err) => console.error('[MetodoDatos] Error en pool SQL:', err.message));
            return p;
        })
        .catch((err) => {
            _conectando = null;
            throw err;
        });
    return _conectando;
}

// ---------------------------------------------------------------------
//  Tipos tabla (TVP): se lee la definicion REAL desde SQL Server
//  (sys.table_types) una sola vez por SP y se cachea. Asi no hay que
//  re-escribir en Node la estructura de RolDetalleType, TY007, etc.
// ---------------------------------------------------------------------
const _cacheTVP = new Map();

const _mapaTipos = {
    int: () => sql.Int,
    bigint: () => sql.BigInt,
    smallint: () => sql.SmallInt,
    tinyint: () => sql.TinyInt,
    bit: () => sql.Bit,
    decimal: (c) => sql.Decimal(c.precision, c.scale),
    numeric: (c) => sql.Numeric(c.precision, c.scale),
    money: () => sql.Money,
    smallmoney: () => sql.SmallMoney,
    float: () => sql.Float,
    real: () => sql.Real,
    date: () => sql.Date,
    datetime: () => sql.DateTime,
    datetime2: (c) => sql.DateTime2(c.scale),
    smalldatetime: () => sql.SmallDateTime,
    time: (c) => sql.Time(c.scale),
    char: (c) => sql.Char(c.max_length),
    varchar: (c) => sql.VarChar(c.max_length === -1 ? sql.MAX : c.max_length),
    nchar: (c) => sql.NChar(c.max_length / 2),
    nvarchar: (c) => sql.NVarChar(c.max_length === -1 ? sql.MAX : c.max_length / 2),
    text: () => sql.Text,
    ntext: () => sql.NText,
    uniqueidentifier: () => sql.UniqueIdentifier,
    varbinary: (c) => sql.VarBinary(c.max_length === -1 ? sql.MAX : c.max_length),
    binary: (c) => sql.Binary(c.max_length),
    image: () => sql.Image,
};

async function _ObtenerTiposTabla(pool, nombreSp) {
    if (_cacheTVP.has(nombreSp)) return _cacheTVP.get(nombreSp);

    const r = await pool.request()
        .input('sp', sql.NVarChar(256), nombreSp)
        .query(`
            SELECT p.name AS parametro, tt.name AS tipo, SCHEMA_NAME(tt.schema_id) AS esquema,
                   c.column_id, c.name AS columna, ty.name AS tipoColumna,
                   c.max_length, c.precision, c.scale, c.is_nullable
            FROM sys.parameters p
            JOIN sys.table_types tt ON tt.user_type_id = p.user_type_id
            JOIN sys.columns c      ON c.object_id = tt.type_table_object_id
            JOIN sys.types ty       ON ty.user_type_id = c.user_type_id
            WHERE p.object_id = OBJECT_ID(@sp)
            ORDER BY p.name, c.column_id`);

    const definicion = {};
    for (const f of r.recordset) {
        const clave = f.parametro.toLowerCase();
        if (!definicion[clave]) definicion[clave] = { tipo: `${f.esquema}.${f.tipo}`, columnas: [] };
        definicion[clave].columnas.push(f);
    }
    _cacheTVP.set(nombreSp, definicion);
    return definicion;
}

function _ConstruirTabla(defTVP, detalle, nombreParam) {
    const tabla = new sql.Table(defTVP.tipo);
    defTVP.columnas.forEach((c) => {
        const fabrica = _mapaTipos[c.tipoColumna] || (() => sql.NVarChar(sql.MAX));
        tabla.columns.add(c.columna, fabrica(c), { nullable: true });
    });

    const columnasOrigen = detalle.columnas || [];
    const largo = columnasOrigen.length || (Array.isArray(detalle.filas?.[0]) ? detalle.filas[0].length : 0);
    if (largo && largo !== defTVP.columnas.length) {
        throw new Error(
            `El detalle ${nombreParam} trae ${largo} columnas y el tipo ${defTVP.tipo} espera ${defTVP.columnas.length}`
        );
    }

    for (const fila of detalle.filas || []) {
        // Igual que ADO.NET: por posicion. La fila puede ser objeto o arreglo posicional.
        const valores = defTVP.columnas.map((c, i) => {
            let v;
            if (Array.isArray(fila)) v = fila[i];
            else v = fila[columnasOrigen[i] ?? c.columna];
            if (v === undefined || v === '') v = null;
            if (typeof v === 'boolean') v = v ? 1 : 0;
            return v;
        });
        tabla.rows.add(...valores);
    }
    return tabla;
}

function _NormalizarValor(v) {
    if (v === undefined) return null;
    if (typeof v === 'boolean') return v ? 1 : 0;
    return v;
}

// =====================================================================
//  EjecutarProcedimiento  -> devuelve el primer recordset (como el DataTable de VB)
// =====================================================================
async function EjecutarProcedimiento(_Nombre, _listaParam) {
    const pool = await CrearConexion();
    const request = pool.request();

    const hayDetalle = _listaParam.some((p) => p.detalle);
    const defTVP = hayDetalle ? await _ObtenerTiposTabla(pool, _Nombre) : {};

    for (const p of _listaParam) {
        const nombre = p.nombre.replace(/^@/, '');
        if (p.detalle) {
            const def = defTVP[p.nombre.toLowerCase()];
            if (!def) throw new Error(`El SP ${_Nombre} no tiene un parametro tipo tabla ${p.nombre}`);
            request.input(nombre, _ConstruirTabla(def, p.detalle, p.nombre));
        } else {
            request.input(nombre, _NormalizarValor(p.valor));
        }
    }

    const resultado = await request.execute(_Nombre);
    return resultado.recordset || [];
}

// =====================================================================
//  EjecutarConsulta -> SELECT parametrizado (reemplaza D_Datos_Tabla con
//  concatenacion de strings, que era vulnerable a SQL Injection)
// =====================================================================
async function EjecutarConsulta(_Sql, _listaParam = []) {
    const pool = await CrearConexion();
    const request = pool.request();
    for (const p of _listaParam) request.input(p.nombre.replace(/^@/, ''), _NormalizarValor(p.valor));
    const resultado = await request.query(_Sql);
    return resultado.recordset || [];
}

async function CerrarConexion() {
    if (_pool) await _pool.close();
    _pool = null;
}

module.exports = { CrearConexion, EjecutarProcedimiento, EjecutarConsulta, CerrarConexion };
