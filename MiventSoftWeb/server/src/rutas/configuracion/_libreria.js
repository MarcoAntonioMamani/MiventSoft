// Efecto tipo 10 del escritorio: "Crear nuevo ..." en un combo de libreria (MAM_Clasificadores)
// Cada pantalla solo puede crear en SUS clasificadores (lista blanca).
const L = require('../../negocio/AccesoLogica');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

function rutaNuevaLibreria(permitidos) {
    return asyncHandler(async (req, res) => {
        const id = Number.parseInt(req.params.idClasificador, 10);
        if (!permitidos.includes(id)) throw new ErrorNegocio('Libreria no permitida en esta pantalla', 403);
        const desc = v.requerido(req.body?.descripcion, 'Descripcion', 100).toUpperCase();
        const existentes = await L.L_prLibreriaDetalleGeneral(id);
        if (existentes.some((f) => String(f.cndesc1).trim().toUpperCase() === desc)) {
            throw new ErrorNegocio(`"${desc}" ya existe.`);
        }
        const r = await L.L_prClasificadorGrabar('', id, desc);
        if (!r.resultado) throw new ErrorNegocio('No se pudo registrar.');
        const lista = v.proyectar(await L.L_prLibreriaDetalleGeneral(id), ['cnnum', 'cndesc1']);
        res.json({ ok: true, id: r._numi, data: lista, mensaje: `${desc} REGISTRADO CON EXITO.` });
    });
}

const libreria = async (id) => v.proyectar(await L.L_prLibreriaDetalleGeneral(id), ['cnnum', 'cndesc1']);

module.exports = { rutaNuevaLibreria, libreria };
