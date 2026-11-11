// =====================================================================
//  Tec_Clientes.vb  (programa: btConfCliente)   SP: MAM_Clientes
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const { rutaNuevaLibreria, libreria } = require('./_libreria');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btConfCliente'));

// Solo columnas que usa la pantalla (se omiten FechaUltimaVenta, ImagenCliente, imgEstado...)
const CAMPOS = ['Id', 'CodigoExterno', 'NombreCliente', 'DireccionCliente', 'Telefono', 'TipoDocumento', 'NroDocumento',
    'RazonSocial', 'Nit', 'Estado', 'FechaIngreso', 'Latitud', 'Longitud', 'PrecioCategoriaId', 'ZonaId'];

router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_prListarGeneral('MAM_Clientes'), CAMPOS) });
}));

router.get('/combos', asyncHandler(async (req, res) => {
    const [tiposDocumento, precios, zonas] = await Promise.all([
        libreria(8),
        L.L_prListaCategoriasPrecios(),
        L.L_prListarZonas(),
    ]);
    res.json({
        ok: true,
        data: {
            tiposDocumento,
            precios: v.proyectar(precios, ['Id', 'Descripcion']),
            zonas: v.proyectar(zonas, ['Id', 'NombreZona']),
        },
    });
}));

// btnTipoDocumento (8)
router.post('/libreria/:idClasificador', rutaNuevaLibreria([8]));

function _leerCliente(b, fechaIngreso) {
    const lat = v.decimal(b.Latitud);
    const lng = v.decimal(b.Longitud);
    if (Math.abs(lat) > 90 || Math.abs(lng) > 180) throw new ErrorNegocio('Ubicacion no valida');
    return {
        IdZona: v.entero(b.ZonaId, 'una Zona', { requeridoPositivo: true }),
        IdPrecio: v.entero(b.PrecioCategoriaId, 'una Categoria de Precio', { requeridoPositivo: true }),
        CodigoExterno: v.texto(b.CodigoExterno, 50),
        NombreCliente: v.requerido(b.NombreCliente, 'Nombre Cliente', 200),
        Direccion: v.texto(b.DireccionCliente, 300),
        Telefono: v.texto(b.Telefono, 50),
        TipoDocumento: v.entero(b.TipoDocumento, 'Tipo de Documento', { requeridoPositivo: true }),
        NroDocumento: v.texto(b.NroDocumento, 50),
        RazonSocial: v.texto(b.RazonSocial, 200),
        nit: v.texto(b.Nit, 50),
        estado: v.bit(b.Estado),
        FechaIngreso: fechaIngreso,
        Latitud: lat,
        Longitud: lng,
    };
}

router.post('/', asyncHandler(async (req, res) => {
    // Igual que VB: Now.Date.ToString("yyyy/MM/dd")
    const c = _leerCliente(req.body, v.fechaVB());
    const r = await L.InsertarCliente({ _Id: '', ...c });
    if (!r.resultado) throw new ErrorNegocio('Existe Problema al Registrar el Cliente'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `Cliente con Id = ${r._numi} fue Registrado Correctamente.`.toUpperCase() });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    // Correccion: el escritorio reescribia FechaIngreso con la fecha de hoy al modificar.
    // Aqui se conserva la fecha original del cliente.
    const actual = v.buscarId(await L.L_prListarGeneral('MAM_Clientes'), id);
    if (!actual) throw new ErrorNegocio('El cliente no existe', 404);
    const c = _leerCliente(req.body, v.fechaVB(v.campo(actual, 'FechaIngreso')));
    const r = await L.ModificarCliente({ _Id: id, ...c });
    if (!r.resultado) throw new ErrorNegocio('Existe Problemas al modificar el Cliente'.toUpperCase());
    res.json({ ok: true, id, mensaje: `Cliente con Codigo = ${id} modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await L.L_prClienteBorrar(id))) throw new ErrorNegocio('No se pudo eliminar el cliente (puede tener ventas registradas).');
    res.json({ ok: true, mensaje: `Cliente con Codigo ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
