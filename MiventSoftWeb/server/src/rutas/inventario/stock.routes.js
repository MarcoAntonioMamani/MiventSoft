// =====================================================================
//  FormularioStock.vb (pestaña fija "Inventario" de Tec_Principal) -> Inventario Productos. SOLO LECTURA.
//   combos: sp_Mam_Precios @tipo 9 (Categoria Precio) · MAM_Compras @tipo 12 (Proveedor, -1 = TODOS)
//   grilla: MAM_Movimientos @tipo 18 (@CategoriaPrecio, @ProveedorId)
//  En el escritorio la pestaña no depende de RolesDetalles (la ve todo usuario con sesion): aqui igual.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { asyncHandler } = require('../../middleware/errores');
const v = require('../../utils/validar');
const { col, num } = require('../distribucion/comun');

const router = express.Router();

router.get('/combos', asyncHandler(async (req, res) => {
    const [cats, provs] = await Promise.all([L.L_fnListarCategoriaPrecio(), L.ListarProveedoresCombo()]);
    res.json({ ok: true, data: {
        categorias: cats.map((f) => ({ Id: num(col(f, 'ygnumi')), Codigo: col(f, 'ygcod') ?? '', Nombre: String(col(f, 'ygdesc') ?? '').trim() })),
        // dt.Rows.Add(-1, "TODOS") -> va al final, como en VB
        proveedores: [...provs.map((f) => ({ Id: num(col(f, 'Id')), Nombre: String(col(f, 'Proveedor') ?? '').trim() })), { Id: -1, Nombre: 'TODOS' }],
    } });
}));

router.get('/', asyncHandler(async (req, res) => {
    const cat = v.entero(req.query.categoria, 'Categoria Precio', { requeridoPositivo: true });
    const prov = req.query.proveedor === undefined || String(req.query.proveedor) === '-1' ? -1 : v.entero(req.query.proveedor, 'Proveedor', { requeridoPositivo: true });
    const dt = await L.L_prListarProductosTodosInventario(cat, prov);
    res.set('Cache-Control', 'no-store');
    res.json({ ok: true, data: dt.map((f) => ({
        Id: num(col(f, 'Id')),
        CodigoExterno: String(col(f, 'CodigoExterno') ?? ''),
        NombreProveedor: String(col(f, 'NombreProveedor') ?? '').trim(),
        DescripcionProducto: String(col(f, 'DescripcionProducto') ?? '').trim(),
        Categoria: String(col(f, 'Categoria') ?? '').trim(),
        industria: String(col(f, 'industria') ?? '').trim(),
        unidad: String(col(f, 'unidad') ?? '').trim(),
        precio: num(col(f, 'precio')),
        stockMinimo: num(col(f, 'stockMinimo')),
        stockGeneral: num(col(f, 'stockGeneral')),
    })) });
}));

module.exports = router;
