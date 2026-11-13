// =====================================================================
//  Privilegios por programa (equivale a _PCargarPrivilegios de Tec_Principal.vb)
//  Programas.IdPrograma = nombre del boton en el escritorio (btConfRoles, btnConfCategoria...)
//  Regla del escritorio: el programa es visible si Ver/Insertar/Modificar/Eliminar = 1
// =====================================================================
const { L_RolDetalle_General, L_prUsuarioGeneral } = require('../negocio/AccesoLogica');

const _cache = new Map(); // rol -> { expira, programas:Set }
const TTL_MS = 60 * 1000;

// Programas donde se eliminan transacciones (ventas, compras, movimientos de inventario, pagos de cobranzas y de
// cuentas por pagar): ademas del "Eliminar" del rol, el usuario debe tener "Eliminar registros" (Usuarios.EliminarRegistros)
const PROGRAMAS_ELIMINAR_USUARIO = new Set(['BTVENTVENTA', 'BTNDESPACHOVENTA', 'BTNCOMPRAS', 'BTINVMOVIMIENTO', 'BTNCREDPAGO', 'BTNCOMPRACREDITO']);
const MENSAJE_ELIMINAR_USUARIO = 'Su usuario no tiene permiso para eliminar registros, consulte con su Administrador.';

// Se lee de la BD (no del inicio de sesion): si el administrador lo quita, aplica sin volver a ingresar
let _cacheUsuarios = { expira: 0, eliminar: new Map() };
let _avisoSinColumna = false;
async function UsuarioPuedeEliminar(idUsuario) {
    const id = Number(idUsuario);
    if (_cacheUsuarios.expira <= Date.now() || !_cacheUsuarios.eliminar.has(id)) {
        const dt = await L_prUsuarioGeneral();
        // BD sin EliminarRegistros.sql: se permite como antes y se avisa una vez
        if (dt.length && !('EliminarRegistros' in dt[0]) && !_avisoSinColumna) {
            _avisoSinColumna = true;
            console.warn('[permisos] Falta ejecutar EliminarRegistros.sql en la BD: no se controla "Eliminar registros" del usuario.');
        }
        _cacheUsuarios = { expira: Date.now() + TTL_MS, eliminar: new Map(dt.map((u) => [Number(u.Id), _bit(u.EliminarRegistros ?? 1)])) };
    }
    return _cacheUsuarios.eliminar.get(id) === true;
}

/** Quita "eliminar" si el programa es de transacciones y el usuario no tiene "Eliminar registros" */
async function _conPermisoUsuario(sesion, idPrograma, p) {
    if (!p.eliminar || !PROGRAMAS_ELIMINAR_USUARIO.has(String(idPrograma).toUpperCase())) return p;
    return (await UsuarioPuedeEliminar(sesion.gi_userNumi)) ? p : { ...p, eliminar: false, sinEliminarUsuario: true };
}

// Programa asignado al rol = "Ver" marcado (es la casilla "Asignar" de Roles de usuario). Si en la base quedo
// Insertar/Modificar/Eliminar en 1 con Ver en 0 (programa desasignado), NO se muestra ni se puede abrir:
// asi el menu coincide con lo que se ve en Roles (un modulo sin programas asignados no aparece).
function _tienePermiso(f) {
    return Number(f.Ver) === 1 || f.Ver === true;
}

const _bit = (v) => Number(v) === 1 || v === true;
/**
 * Acciones del rol en un programa.
 * Si el rol solo tiene "Ver" (no marco Insertar / Modificar / Eliminar) se permite todo,
 * igual que el escritorio (que solo decide si el programa se ve o no).
 * Si marco alguna de las tres, se respetan las que marco.
 */
function _acciones(f) {
    const a = { insertar: _bit(f.Insertar), modificar: _bit(f.Modificar), eliminar: _bit(f.Eliminar) };
    return a.insertar || a.modificar || a.eliminar ? a : { insertar: true, modificar: true, eliminar: true };
}

async function _cargar(rolId) {
    const c = _cache.get(rolId);
    if (c && c.expira > Date.now()) return c;
    const dt = await L_RolDetalle_General(-1, rolId);
    const permisos = new Map();
    for (const f of dt.filter(_tienePermiso)) {
        permisos.set(String(f.IdPrograma).toUpperCase(), { ver: true, ..._acciones(f) });
    }
    const r = { expira: Date.now() + TTL_MS, programas: new Set(permisos.keys()), permisos };
    _cache.set(rolId, r);
    return r;
}

async function ProgramasPermitidos(rolId) {
    return (await _cargar(rolId)).programas;
}

/** { ver, insertar, modificar, eliminar } del rol para un programa (null si no lo tiene) */
async function PermisosPrograma(rolId, idPrograma) {
    return (await _cargar(rolId)).permisos.get(String(idPrograma).toUpperCase()) || null;
}

function LimpiarCachePermisos() {
    _cache.clear();
    _cacheUsuarios = { expira: 0, eliminar: new Map() };
}

// Accion que exige cada peticion (RolesDetalles: Insertar / Modificar / Eliminar)
//   GET = ver ; POST = insertar ; PUT = modificar ; DELETE = eliminar
//   POST /libreria/... (crear item de combo dentro del formulario) = insertar o modificar
//   Pantallas con acciones que no son CRUD (ej. Asignacion de pedidos) fijan la accion con accionPermiso('modificar')
function _accionPermitida(req, p) {
    if (req.accionPermiso) return !!p[req.accionPermiso];
    switch (req.method) {
        case 'GET': case 'HEAD': return true;
        case 'POST': return req.path.startsWith('/libreria') ? p.insertar || p.modificar : p.insertar;
        case 'PUT': case 'PATCH': return p.modificar;
        case 'DELETE': return p.eliminar;
        default: return false;
    }
}
const NOMBRE_ACCION = { POST: 'registrar', PUT: 'modificar', PATCH: 'modificar', DELETE: 'eliminar' };
const NOMBRE_PERMISO = { insertar: 'registrar', modificar: 'modificar', eliminar: 'eliminar' };
// Uso: router.post('/anular', accionPermiso('eliminar'), requierePrograma('btnX'), ...)
const accionPermiso = (accion) => (req, res, next) => { req.accionPermiso = accion; next(); };

// Uso: router.use(requierePrograma('btConfRoles'))
// Tambien acepta varios Id (el mismo formulario con botones distintos segun la rama): requierePrograma(['btnA', 'btnB'])
function requierePrograma(idPrograma) {
    const claves = (Array.isArray(idPrograma) ? idPrograma : [idPrograma]).map((x) => x.toUpperCase());
    return async (req, res, next) => {
        try {
            let p = null;
            let programa = null;
            for (const clave of claves) { p = await PermisosPrograma(req.sesion.gi_userRol, clave); if (p) { programa = clave; break; } }
            if (!p) return res.status(403).json({ ok: false, mensaje: 'No tiene privilegios para este programa.' });
            p = await _conPermisoUsuario(req.sesion, programa, p);
            if (!_accionPermitida(req, p)) {
                // lo bloquea solo el permiso del usuario (el rol si lo permite)
                if (p.sinEliminarUsuario && _accionPermitida(req, { ...p, eliminar: true })) {
                    return res.status(403).json({ ok: false, mensaje: MENSAJE_ELIMINAR_USUARIO });
                }
                return res.status(403).json({ ok: false, mensaje: `Su rol no tiene permiso para ${NOMBRE_PERMISO[req.accionPermiso] || NOMBRE_ACCION[req.method] || 'esta accion'} en este programa.` });
            }
            req.permisos = p;
            next();
        } catch (e) {
            next(e);
        }
    };
}

module.exports = {
    requierePrograma, accionPermiso, _acciones, ProgramasPermitidos, PermisosPrograma, LimpiarCachePermisos, _tienePermiso,
    UsuarioPuedeEliminar, PROGRAMAS_ELIMINAR_USUARIO,
};
