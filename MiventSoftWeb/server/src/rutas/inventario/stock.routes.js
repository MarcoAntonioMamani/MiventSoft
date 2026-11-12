// =====================================================================
//  KAILY LA PAZ: FormularioStock.vb (pestaña fija "Inventario" de Tec_Principal) -> Inventario Productos. SOLO LECTURA.
//   combo: sp_Mam_Precios @tipo 8 (L_fnListarCategoriaPrecio, Categoria Precio). Kaily ya no filtra por proveedor.
//   grilla: MAM_Movimientos @tipo 18 (L_prListarProductosTodosInventario, solo @CategoriaPrecio)
//   Columnas visibles: Cod. Externo, Categoria, Unidad Venta, Descripcion, Stock Minimo, Precio, Stock, Stock General
//  En el escritorio la pestaña no depende de RolesDetalles (la ve todo usuario con sesion): aqui igual.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { asyncHandler } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { col, num } = require('../distribucion/comun');

const router = express.Router();

router.get('/combos', asyncHandler(async (req, res) => {
    const cats = await L.L_fnListarCategoriaPrecio();
    res.json({ ok: true, data: {
        categorias: cats.map((f) => ({ Id: num(col(f, 'ygnumi')), Codigo: col(f, 'ygcod') ?? '', Nombre: String(col(f, 'ygdesc') ?? '').trim() })),
    } });
}));

// "stock" puede venir como numero o como texto (stock por deposito, MaxLines 5 en la grilla): se envia tal cual
const valorStock = (x) => (typeof x === 'number' ? x : x === null || x === undefined ? '' : String(x).trim());

router.get('/', asyncHandler(async (req, res) => {
    const cat = v.entero(req.query.categoria, 'Categoria Precio', { requeridoPositivo: true });
    const dt = await L.L_prListarProductosTodosInventario(cat);
    res.set('Cache-Control', 'no-store');
    res.json({ ok: true, data: dt.map((f) => ({
        Id: num(col(f, 'Id')),
        CodigoExterno: String(col(f, 'CodigoExterno') ?? ''),
        DescripcionProducto: String(col(f, 'DescripcionProducto') ?? '').trim(),
        Categoria: String(col(f, 'Categoria') ?? '').trim(),
        unidad: String(col(f, 'unidad') ?? '').trim(),
        precio: num(col(f, 'precio')),
        stockMinimo: num(col(f, 'stockMinimo')),
        stock: valorStock(col(f, 'stock')),
        stockGeneral: num(col(f, 'stockGeneral')),
    })) });
}));

module.exports = router;
