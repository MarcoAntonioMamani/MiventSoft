// =====================================================================
//  Tec_Precios.vb  (programa: btConfPrecio)   SP: sp_Mam_Precios (TVP @TY007)
//
//  Tabla "precio" en VB = L_fnListarProductosConPrecios(almacen). Estados:
//    0 = sin precio, 1 = existente, 2 = existente modificado, 3 = nuevo
//  El navegador envia SOLO las celdas cambiadas [producto, categoria, precio].
//  Aqui se recarga "precio", se aplican los cambios con las mismas reglas de
//  grprecio_CellEdited y se envia la tabla completa al SP, igual que VB.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

const router = express.Router();
router.use(requierePrograma('btConfPrecio'));

router.get('/almacenes', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_fnGeneralSucursales(), ['aanumi', 'aabdes']) });
}));

router.get('/categorias', asyncHandler(async (req, res) => {
    res.json({ ok: true, data: v.proyectar(await L.L_fnGeneralCategorias(), ['ygnumi', 'ygcod', 'ygdesc', 'estado']) });
}));

// Matriz producto x categoria para un almacen
router.get('/matriz/:almacen', asyncHandler(async (req, res) => {
    const almacen = v.idRuta(req.params.almacen);
    const [productos, categorias, precio] = await Promise.all([
        L.L_prSoloActivos(L.L_fnListarProductos(), 'yfnumi'), L.L_fnListarCategorias(), L.L_fnListarProductosConPrecios(almacen),
    ]);
    const p = v.proyectar(precio, ['yhprod', 'yhcatpre', 'yhprecio', 'estado']);
    res.json({
        ok: true,
        data: {
            productos: v.proyectar(productos, ['yfnumi', 'yfcprod', 'yfcdprod1', 'NombreCategoria', 'proveedor']),
            categorias: v.proyectar(categorias, ['ygnumi', 'ygcod', 'ygdesc']),
            // [producto, categoria, precio] solo de las celdas que tienen precio registrado
            precios: p.filter((x) => Number(x.estado) !== 0).map((x) => [x.yhprod, x.yhcatpre, Number(x.yhprecio)]),
        },
    });
}));

router.put('/matriz/:almacen', asyncHandler(async (req, res) => {
    const almacen = v.idRuta(req.params.almacen);
    const cambios = Array.isArray(req.body?.cambios) ? req.body.cambios : [];
    if (!cambios.length) throw new ErrorNegocio('No hay precios modificados.');
    if (cambios.length > 20000) throw new ErrorNegocio('Demasiados cambios en una sola grabacion.');

    const precio = await L.L_fnListarProductosConPrecios(almacen);
    const cols = L._columnasDe(precio);
    const k = (n) => cols.find((c) => c.toLowerCase() === n.toLowerCase());
    const [cProd, cCat, cPrecio, cEstado] = [k('yhprod'), k('yhcatpre'), k('yhprecio'), k('estado')];
    if (!cProd || !cCat || !cPrecio || !cEstado) throw new Error('sp_Mam_Precios tipo 5 no devolvio las columnas esperadas');

    const indice = new Map(precio.map((f) => [`${f[cProd]}|${f[cCat]}`, f]));
    for (const c of cambios) {
        const fila = indice.get(`${Number(c?.[0])}|${Number(c?.[1])}`);
        if (!fila) throw new ErrorNegocio('Producto o categoria de precio no valido.');
        let valor = Number.parseFloat(c[2]);
        if (!Number.isFinite(valor)) valor = 0; // VB: si no es numerico -> 0
        if (valor < 0) throw new ErrorNegocio('El precio no puede ser negativo.');
        const est = Number(fila[cEstado]);
        if (est === 1 || est === 2) fila[cEstado] = 2;
        else fila[cEstado] = 3; // 0 o 3
        fila[cPrecio] = Math.round(valor * 100) / 100;
    }

    const filas = precio.map((f) => cols.map((c) => f[c]));
    const r = await L.L_fnGrabarPrecios('', almacen, { columnas: [], filas });
    if (!r.resultado) throw new ErrorNegocio('Los precios no pudieron ser grabados'.toUpperCase());
    res.json({ ok: true, mensaje: `${cambios.length} PRECIO(S) GRABADO(S) CON EXITO.` });
}));

// ---- Categorias de precio (panel "Categorias") ----

// _FSiguienteLetra
function _siguienteLetra(palabra) {
    const A = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    if (palabra.length === 1) {
        const i = A.indexOf(palabra[0]);
        return i === 25 ? 'AA' : A[i + 1];
    }
    const i = A.indexOf(palabra[1]);
    return i === 25 ? '' : palabra[0] + A[i + 1];
}
const _esNumero = (x) => String(x).trim() !== '' && !Number.isNaN(Number(x));

// _fnObtenerSiguienteString / _fnObtenerSiguienteInt (busca desde el final de la lista)
function _siguienteCodigo(dt, esLetra) {
    for (let i = dt.length - 1; i >= 0; i--) {
        const cod = String(dt[i].ygcod).trim();
        if (esLetra && !_esNumero(cod)) return _siguienteLetra(cod);
        if (!esLetra && _esNumero(cod)) return String(Number(cod) + 1);
    }
    return esLetra ? _siguienteLetra(' ') : '1';
}

router.post('/categorias', asyncHandler(async (req, res) => {
    const desc = v.requerido(req.body?.descripcion, 'Ingrese una Descripcion!', 100);
    const esLetra = v.bit(req.body?.tipo) === 1; // swEstado = True -> letra
    const dt = await L.L_fnGeneralCategorias();
    const letra = _siguienteCodigo(dt, esLetra);
    if (!letra) throw new ErrorNegocio('Se alcanzo el limite de codigos de categoria.');
    const r = await L.L_fnGrabarCategorias('', letra, desc, esLetra ? 1 : 0);
    if (!r.resultado) throw new ErrorNegocio('La categoria no pudo ser insertado'.toUpperCase());
    res.json({ ok: true, id: r._numi, mensaje: `Categoria ${letra} Grabado con Exito.`.toUpperCase() });
}));

router.delete('/categorias/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    const dt = await L.L_fnGeneralCategorias();
    const pos = dt.findIndex((f) => Number(f.ygnumi) === id);
    if (pos < 0) throw new ErrorNegocio('La categoria no existe', 404);
    // VB: If grcategoria.Row > 1 -> las 2 primeras son del sistema
    if (pos <= 1) throw new ErrorNegocio('CODIGO DE CATEGORIA DEL SISTEMA NO PUEDE SER ELIMINADA');
    if (!(await L.L_fnEliminarCategoria(id))) throw new ErrorNegocio('No se pudo eliminar la categoria.');
    res.json({ ok: true, mensaje: `Codigo de Categoria ${id} eliminado con Exito.`.toUpperCase() });
}));

module.exports = router;
module.exports._siguienteCodigo = _siguienteCodigo;
