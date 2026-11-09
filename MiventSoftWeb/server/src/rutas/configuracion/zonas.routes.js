// =====================================================================
//  Tec_Zonas.vb  (programa: btZona)   SP: sp_Mam_Zonas  (TVP @ZonasPuntos)
//
//  _TablePoint en VB: filas (?, ?, zblat, zblongi, estado). Estados:
//    0 = nuevo, 1 = existente, -1 = existente eliminado, -2 = nuevo eliminado
//  El navegador solo envia la lista [lat,lng] si redibujo el poligono;
//  la tabla completa (con los estados) se arma aqui en el servidor, igual que VB.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btZona'));

const MAX_PUNTOS = 500;

router.get('/', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.GeneralZona(), ['Id', 'NombreZona', 'DescripcionZona', 'Color']) });
}));

// Solo lat/lng del poligono
router.get('/:id/puntos', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const dt = v.proyectar(await L.L_fnListarPuntosPorZona(id), ['zblat', 'zblongi']);
    res.json({ ok: true, data: dt.map((p) => [Number(p.zblat), Number(p.zblongi)]) });
}));

function _leerPuntos(b) {
    if (b.puntos === undefined || b.puntos === null) return null; // no redibujo
    if (!Array.isArray(b.puntos) || b.puntos.length > MAX_PUNTOS) throw new ErrorNegocio('Puntos de zona no validos');
    const puntos = b.puntos.map((p) => [Number(p?.[0]), Number(p?.[1])]);
    if (puntos.some(([la, ln]) => !Number.isFinite(la) || !Number.isFinite(ln) || Math.abs(la) > 90 || Math.abs(ln) > 180)) {
        throw new ErrorNegocio('Coordenadas no validas');
    }
    if (puntos.length > 0 && puntos.length < 3) throw new ErrorNegocio('La zona necesita al menos 3 puntos');
    return puntos;
}

function _leerZona(b) {
    const color = v.texto(b.Color, 9) || '#2C6AA8';
    if (!/^#[0-9A-Fa-f]{6}$/.test(color)) throw new ErrorNegocio('Color no valido');
    return {
        NombreZona: v.requerido(b.NombreZona, 'Nombre Zona', 100),
        DescripcionZona: v.texto(b.DescripcionZona, 300),
        Color: color.toUpperCase(),
        puntos: _leerPuntos(b),
    };
}

// Arma _TablePoint igual que el escritorio y lo envia por posicion
async function _armarTablePoint(idZona, puntosNuevos) {
    const dt = await L.L_fnListarPuntosPorZona(idZona);
    const cols = L._columnasDe(dt);
    const iCol = cols.findIndex((c) => c.toLowerCase() === 'estado');
    const iEstado = iCol >= 0 ? iCol : 4;
    const filas = dt.map((f) => cols.map((c) => f[c]));
    if (puntosNuevos) {
        // _prCambiarEstadoPuntos(1, -1) / (0, -2)
        filas.forEach((f) => {
            if (Number(f[iEstado]) === 1) f[iEstado] = -1;
            else if (Number(f[iEstado]) === 0) f[iEstado] = -2;
        });
        // _TablePoint.Rows.Add(1, 1, plg.Lat, plg.Lng, 0)
        puntosNuevos.forEach(([lat, lng]) => filas.push([1, 1, lat, lng, 0]));
    }
    return { columnas: [], filas };
}

router.post('/', asyncHandler(async (req, res) => {
    const z = _leerZona(req.body);
    if (!z.puntos || !z.puntos.length) throw new ErrorNegocio('Dibuje la zona en el mapa (minimo 3 puntos).');
    const point = await _armarTablePoint(-1, z.puntos);
    const r = await L.InsertarZona('', z.NombreZona, z.DescripcionZona, z.Color, point);
    if (!r.resultado) throw new ErrorNegocio('La Zona no pudo ser insertado'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `Codigo de Zona ${r._numi} Grabado con Exito.`.toUpperCase() });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const z = _leerZona(req.body);
    const point = await _armarTablePoint(id, z.puntos);
    const r = await L.ActualizarZona(id, z.NombreZona, z.DescripcionZona, z.Color, point);
    if (!r.resultado) throw new ErrorNegocio('La Zona no pudo ser modificado'.toUpperCase());
    res.json({ ok: true, id, mensaje: `Codigo de Zona ${id} modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    if (!(await L.EliminarZona(id))) throw new ErrorNegocio('No se pudo eliminar la zona (puede tener clientes asignados).');
    res.json({ ok: true, mensaje: `Codigo de Zona ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
