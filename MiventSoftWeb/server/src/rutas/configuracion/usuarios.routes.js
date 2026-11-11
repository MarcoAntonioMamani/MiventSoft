// =====================================================================
//  Tec_Users.vb  (programa: btConfUsuarios)
//  Cambio de seguridad respecto al escritorio: la contrasena NO se envia al
//  navegador. Al modificar, si se deja vacia se conserva la actual.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma, LimpiarCachePermisos } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btConfUsuarios'));

// Solo lo que usa la pantalla (la Contrasena NUNCA se envia)
const CAMPOS = ['Id', 'NombreUsuario', 'estado', 'RolId', 'NombreRol', 'SucursalId', 'IdEmpresa', 'Empresa', 'IdPersonal', 'Personal',
    'ModificarPrecioVenta', 'AplicarDescuentoVenta', 'VerTodasVentas', 'VerTodosVentasCierre', 'ModificarRegistroAnteriores'];

router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prUsuarioGeneral(), CAMPOS) });
}));

// cbRol, cbEmpresa, cbSucursal y la ayuda de Personal (btnVendedor -> ListarPersonal)
router.get('/combos', asyncHandler(async (req, res) => {
    const [roles, empresas, personal, sucursales] = await Promise.all([
        L.L_prListaRolesUsuarios(),
        L.L_prListaEmpresasUsuarios(),
        L.ListarPersonal(),
        L.L_fnGeneralSucursales(),
    ]);
    // Igual que VB: dt.Rows.Add(-1, "Todos")
    const suc = v.proyectar(sucursales, ['aanumi', 'aabdes']);
    suc.push({ aanumi: -1, aabdes: 'Todos' });
    res.json({
        ok: true,
        data: {
            roles: v.proyectar(roles, ['Id', 'NombreRol']),
            empresas: v.proyectar(empresas, ['Id', 'Nombre']),
            personal: v.proyectar(personal, ['Id', 'Nombre', 'Direccion', 'Telefono01']),
            sucursales: suc,
        },
    });
}));

function _leerUsuario(body, esNuevo) {
    const u = {
        IdPersonal: v.entero(body.IdPersonal, 'Un Personal', { requeridoPositivo: true }),
        // El login compara en MAYUSCULAS (ToUpper) -> se graba en mayusculas
        _NombreUsuario: v.requerido(body.NombreUsuario, 'Nombre de Usuario', 50).toUpperCase(),
        _Contrasena: v.texto(body.Contrasena, 50).toUpperCase(),
        _IdEmpresa: v.entero(body.IdEmpresa, 'Una Empresa', { requeridoPositivo: true }),
        _RolId: v.entero(body.RolId, 'Una Rol', { requeridoPositivo: true }),
        _estado: v.bit(body.estado),
        // cbSucursal (-1 = Todos), igual que Kaily
        _sucursalId: v.entero(body.SucursalId ?? 1, 'Una Sucursal'),
        _ModificarPrecio: v.bit(body.ModificarPrecioVenta),
        _ModificarDescuento: v.bit(body.AplicarDescuentoVenta),
        _VerTodasVentas: v.bit(body.VerTodasVentas),
        _VerTodosCierres: v.bit(body.VerTodosVentasCierre),
        _ModificarRegistrosAntiguos: v.bit(body.ModificarRegistroAnteriores),
    };
    if (esNuevo && u._Contrasena === '') throw new ErrorNegocio('Ingrese una Contraseña');
    return u;
}

router.post('/', asyncHandler(async (req, res) => {
    const u = _leerUsuario(req.body, true);
    const r = await L.L_prUsuarioInsertar({ _numi: '', ...u });
    if (!r.resultado) throw new ErrorNegocio('No se pudo grabar el usuario');
    res.json({ ok: true, id: r._numi, mensaje: `Codigo de Usuario ${r._numi} Grabado con Exito.`.toUpperCase() });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const u = _leerUsuario(req.body, false);
    if (u._Contrasena === '') {
        const actual = v.buscarId(await L.L_prUsuarioGeneral(), id);
        if (!actual) throw new ErrorNegocio('El usuario no existe', 404);
        u._Contrasena = v.campo(actual, 'Contrasena');
    }
    const r = await L.L_prUsuarioModificar({ _numi: id, ...u });
    if (!r.resultado) throw new ErrorNegocio('No se pudo modificar el usuario');
    LimpiarCachePermisos();
    res.json({ ok: true, id, mensaje: `Codigo de Usuario ${id} modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (id === Number(req.sesion.gi_userNumi)) throw new ErrorNegocio('No puede eliminar su propio usuario.');
    const ok = await L.L_prUsuarioBorrar(id);
    if (!ok) throw new ErrorNegocio('No se pudo eliminar el usuario.');
    res.json({ ok: true, mensaje: `Codigo de Usuario ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
