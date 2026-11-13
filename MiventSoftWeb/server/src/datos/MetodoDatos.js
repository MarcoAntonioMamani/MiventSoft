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
const { clienteActual, usuarioActual } = require('./ContextoPeticion');

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

const _TIPOS_TEXTO = new Set(['char', 'varchar', 'nchar', 'nvarchar', 'text', 'ntext']);

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
            if (v === undefined) v = null;
            // '' -> NULL salvo en columnas de texto NOT NULL del tipo tabla (ADO.NET envia la cadena vacia tal cual;
            // p.ej. PlanillaConcetoType.Descripcion NOT NULL fallaba con "Cannot insert the value NULL")
            if (v === '' && (c.is_nullable || !_TIPOS_TEXTO.has(c.tipoColumna))) v = null;
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

// ---------------------------------------------------------------------
//  Auditoria (sql/10_AuditoriaEquipo.sql): al modificar (@tipo 2) o eliminar (@tipo -1) en estos SP, los datos
//  del equipo del cliente se dejan en AuditoriaContexto en el MISMO lote que ejecuta el SP (misma sesion SQL,
//  @@SPID), y MAM_Auditoria los lee al registrar. Si el 10 no esta instalado se ejecuta el SP como siempre.
//  Con el 11 tambien va el nombre completo del usuario (MAM_Compras y MAM_Movimientos lo reciben cortado a 10).
// ---------------------------------------------------------------------
const _SP_AUDITADOS = new Set(['mam_ventas', 'mam_compras', 'mam_movimientos']);
let _hayContexto = { valor: false, usuario: false, vence: 0 };
async function _ContextoInstalado(pool) {
    if ((_hayContexto.valor && _hayContexto.usuario) || _hayContexto.vence > Date.now()) return _hayContexto;
    try {
        const r = await pool.request().query("SELECT CASE WHEN OBJECT_ID('dbo.AuditoriaContexto', 'U') IS NULL THEN 0 ELSE 1 END AS x, CASE WHEN COL_LENGTH('dbo.AuditoriaContexto', 'Usuario') IS NULL THEN 0 ELSE 1 END AS u");
        // todo instalado (10 y 11): para siempre ; si falta algo se vuelve a mirar en 1 minuto (por si se ejecuta sin reiniciar)
        _hayContexto = { valor: r.recordset[0].x === 1, usuario: r.recordset[0].u === 1, vence: Date.now() + 60 * 1000 };
    } catch {
        _hayContexto = { valor: false, usuario: false, vence: Date.now() + 60 * 1000 };
    }
    return _hayContexto;
}
const _nombreSeguro = (n) => /^[A-Za-z_][A-Za-z0-9_]*$/.test(n);

// =====================================================================
//  EjecutarProcedimiento  -> devuelve el primer recordset (como el DataTable de VB)
// =====================================================================
async function EjecutarProcedimiento(_Nombre, _listaParam) {
    const pool = await CrearConexion();
    const request = pool.request();

    const hayDetalle = _listaParam.some((p) => p.detalle);
    const defTVP = hayDetalle ? await _ObtenerTiposTabla(pool, _Nombre) : {};

    const nombres = [];
    for (const p of _listaParam) {
        const nombre = p.nombre.replace(/^@/, '');
        nombres.push(nombre);
        if (p.detalle) {
            const def = defTVP[p.nombre.toLowerCase()];
            if (!def) throw new Error(`El SP ${_Nombre} no tiene un parametro tipo tabla ${p.nombre}`);
            request.input(nombre, _ConstruirTabla(def, p.detalle, p.nombre));
        } else {
            request.input(nombre, _NormalizarValor(p.valor));
        }
    }

    const cliente = clienteActual();
    const tipo = Number(_listaParam.find((p) => /^@?tipo$/i.test(p.nombre))?.valor);
    if (cliente && _SP_AUDITADOS.has(_Nombre.toLowerCase()) && (tipo === 2 || tipo === -1)
        && nombres.every(_nombreSeguro) && (await _ContextoInstalado(pool)).valor) {
        const conUsuario = _hayContexto.usuario;
        if (conUsuario) request.input('__audUsuario', sql.NVarChar(100), usuarioActual());
        request.input('__audIp', sql.NVarChar(64), cliente.ip || null);
        request.input('__audNav', sql.NVarChar(200), cliente.navegador || null);
        request.input('__audEquipo', sql.NVarChar(128), cliente.equipo || null);
        // texto exacto ("-16.495521234567891"): SQL lo convierte a la columna (decimal(21,18) con el 12; decimal(9,6) antes)
        request.input('__audLat', sql.VarChar(40), cliente.latitud ?? null);
        request.input('__audLng', sql.VarChar(40), cliente.longitud ?? null);
        request.input('__audPrec', sql.Int, cliente.precision ?? null);
        const resultado = await request.query(`SET NOCOUNT ON;
DELETE dbo.AuditoriaContexto WHERE Spid = @@SPID OR Fecha < DATEADD(DAY, -1, GETDATE());
INSERT dbo.AuditoriaContexto (Spid, Fecha, Ip, Navegador, Equipo, Latitud, Longitud, PrecisionMetros${conUsuario ? ', Usuario' : ''})
VALUES (@@SPID, GETDATE(), @__audIp, @__audNav, @__audEquipo, @__audLat, @__audLng, @__audPrec${conUsuario ? ', @__audUsuario' : ''});
EXEC dbo.[${_Nombre}] ${nombres.map((n) => `@${n}=@${n}`).join(', ')};
DELETE dbo.AuditoriaContexto WHERE Spid = @@SPID;`);
        return resultado.recordset || [];
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
