// =====================================================================
//  Tec_Login.vb + _PCargarPrivilegios de Tec_Principal.vb
// =====================================================================
const express = require('express');
const rateLimit = require('express-rate-limit');
const {
    L_prConUsuario,
    L_Validar_Usuario,
    ListarPersonalById,
    L_prLibreriaDetalleGeneral,
    L_RolDetalle_General,
} = require('../negocio/AccesoLogica');
const { D_InfoConexion } = require('../datos/AccesoDatos');
const { comoEscritorio } = require('../negocio/menuEscritorio');
const { crearSesion, cerrarSesion, requiereSesion } = require('../middleware/autenticacion');
const { _tienePermiso, _acciones, UsuarioPuedeEliminar, PROGRAMAS_ELIMINAR_USUARIO } = require('../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../middleware/errores');

const router = express.Router();

// Anti fuerza bruta: 10 intentos cada 15 minutos por IP
const limiteLogin = rateLimit({
    windowMs: 15 * 60 * 1000,
    limit: 10,
    standardHeaders: true,
    legacyHeaders: false,
    message: { ok: false, mensaje: 'Demasiados intentos. Espere unos minutos e intente de nuevo.' },
});

router.post(
    '/login',
    limiteLogin,
    asyncHandler(async (req, res) => {
        const tbUsuario = String(req.body?.usuario ?? '').trim();
        const tbpassword = String(req.body?.password ?? '');

        if (tbUsuario === '') throw new ErrorNegocio('Error: Debe ingresar un usuario correcto!!!');
        if (tbpassword === '') throw new ErrorNegocio('Error: Debe ingresar una contraseña correctamente..!!!');

        // Igual que VB: L_Validar_Usuario(tbUsuario.Text.ToUpper(), tbpassword.Text.ToUpper())
        const dtUsuario = await L_Validar_Usuario(tbUsuario.toUpperCase(), tbpassword.toUpperCase());
        if (dtUsuario.length === 0) {
            throw new ErrorNegocio(
                'Error: Los datos de Usuario y Contraseña son incorrectas. Por Favor ingrese con credenciales validos !!!',
                401
            );
        }

        const fila = dtUsuario[0];
        const idPersonal = fila.IdPersonal ?? 0;
        const dt = await ListarPersonalById(idPersonal);
        const gs_user = dt.length > 0 ? dt[0].Nombre : tbUsuario;

        // Kaily (Tec_Login.vb): Global_ModificarPrecio, Global_ModificarDescuento, Global_VentasTodos,
        // Global_CierreTodos, Global_ModificarRegistro y Global_NombreSucursal
        const bit = (x) => (x === true || Number(x) === 1 ? 1 : 0);
        crearSesion(res, {
            gs_user,
            gs_login: tbUsuario.toUpperCase(),
            gi_userNumi: fila.Id,
            gi_userRol: fila.RolId,
            gi_userSuc: fila.SucursalId,
            Global_IdPersonal: idPersonal,
            Global_ModificarPrecio: bit(fila.ModificarPrecioVenta),
            Global_ModificarDescuento: bit(fila.AplicarDescuentoVenta),
            Global_VentasTodos: bit(fila.VerTodasVentas),
            Global_CierreTodos: bit(fila.VerTodosVentasCierre),
            Global_ModificarRegistro: bit(fila.ModificarRegistroAnteriores),
            Global_NombreSucursal: String(fila.NombreSucursal ?? ''),
        });
        res.json({ ok: true });
    })
);

router.post('/logout', (req, res) => {
    cerrarSesion(res);
    res.json({ ok: true });
});

// Datos de sesion + menu de modulos/programas segun el rol (_PCargarPrivilegios)
router.get(
    '/sesion',
    requiereSesion,
    asyncHandler(async (req, res) => {
        const s = req.sesion;
        // Usuarios.EliminarRegistros: sin el permiso se ocultan los botones de eliminar de ventas, compras, movimientos y pagos
        const eliminarRegistros = await L_prConUsuario(s.gs_user, () => UsuarioPuedeEliminar(s.gi_userNumi));
        const modulos = await L_prConUsuario(s.gs_user, async () => {
            const dtModulos = await L_prLibreriaDetalleGeneral(1); // id=1 los modulos del sistema
            const resultado = [];
            for (const m of dtModulos) {
                const dtDetRol = await L_RolDetalle_General(-1, s.gi_userRol, m.cnnum);
                const programas = dtDetRol.filter(_tienePermiso).map((f) => {
                    // permisos por accion para ocultar botones (solo "Ver" = todo, como el escritorio)
                    const a = _acciones(f);
                    const id = String(f.IdPrograma).toUpperCase();
                    if (!eliminarRegistros && PROGRAMAS_ELIMINAR_USUARIO.has(id)) a.eliminar = false;
                    return {
                        IdPrograma: id,
                        DescripcionPrograma: String(f.DescripcionPrograma || '').toUpperCase(),
                        I: a.insertar ? 1 : 0, M: a.modificar ? 1 : 0, E: a.eliminar ? 1 : 0,
                    };
                });
                if (programas.length) resultado.push({ cnnum: m.cnnum, cndesc1: m.cndesc1, programas });
            }
            // solo los botones que tiene cada panel del escritorio, en su orden y con su texto
            return comoEscritorio(resultado);
        });

        res.json({
            ok: true,
            data: {
                gs_user: s.gs_user,
                gs_login: s.gs_login,
                gi_userNumi: s.gi_userNumi,
                gi_userRol: s.gi_userRol,
                gi_userSuc: s.gi_userSuc,
                // Kaily: permisos de venta del usuario (Tec_Login.vb -> Global_*)
                permisosVenta: {
                    modificarPrecio: s.Global_ModificarPrecio === 1,
                    modificarDescuento: s.Global_ModificarDescuento === 1,
                    verTodasVentas: s.Global_VentasTodos === 1,
                    verTodosCierres: s.Global_CierreTodos === 1,
                    modificarRegistrosAnteriores: s.Global_ModificarRegistro === 1,
                    eliminarRegistros,
                },
                nombreSucursal: s.Global_NombreSucursal || '',
                modulos,
                conexion: await D_InfoConexion(),
            },
        });
    })
);

module.exports = router;
