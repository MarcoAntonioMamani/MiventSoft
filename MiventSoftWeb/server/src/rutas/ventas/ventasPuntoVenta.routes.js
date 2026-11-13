// =====================================================================
//  KAILY LA PAZ (punto de venta): Tec_Ventas.vb + Tec_VentasDetalle.vb + EfectivoCobranza.vb
//  programa: btVentVenta   SP: MAM_Ventas (los mismos @tipo y parametros que Negocio/AccesoLogica.vb)
//
//   Lista      L_prListarVentasGeneralFiltroFecha (@tipo 3, @Desde, @Hasta, @SucursalId = Global_Sucursal)
//              Si el rol no es 1 y el usuario no tiene VerTodasVentas, solo sus ventas (PersonalId = Global_IdPersonal)
//   Productos  L_prListarProductosVentas (@tipo 5, @SucursalId, @CategoriaVentaId). Tipo 1 = producto, Tipo 2 = kit
//   Kits       L_prListarProductosKitsVenta (@tipo 10, @Id): cada kit se agrega como sus productos (Tipo 2)
//   Detalle    ListaVentasDetalles (@tipo 4) -> @VentaDetalleType por posicion, estados 0/1/2/-1
//   Pagos      ListaVentasDetallePago (@tipo 8) -> @VentaPagos: contado = 1 fila
//              (0, 0, MontoBs, MontoDolares, Tarjeta, Transferencia, TipoCambio, 0); credito = vacia
//   Grabar     VentaInsertar @tipo 1 / VentaModificar @tipo 2 (+ @Facturado)   Eliminar L_prBorrarRegistro @tipo -1
//
//  Reglas del escritorio:
//   * Vendedor = personal del usuario (btnVendedor esta oculto). Sucursal fija si el usuario tiene una (Global_Sucursal >= 0).
//   * Debe existir Caja Chica abierta (MAM_CierreCajero) para la sucursal y la fecha de la venta.
//   * Cantidad <= stock. Precio editable solo con ModificarPrecioVenta y nunca menor al costo.
//     Descuento por linea solo con AplicarDescuentoVenta. Los productos de un kit no se editan.
//   * Contado: lo pagado (Bs + $ x TC + tarjeta + transferencia) debe cubrir el total.
//   * No se modifica ni elimina una venta de dias anteriores sin ModificarRegistroAnteriores,
//     ni una que pertenece a un cierre de caja (CierreModulo > 0).
//   * Total = Suma(Total detalle) - Descuento.
//
//  Seguridad (el resultado en BD es el mismo): precios, costos, stock y totales los toma y calcula el servidor.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

/**
 * Mismo programa para Tec_Ventas (MAM_Ventas) y Tec_Proformas (MAM_Proforma) de Kaily.
 * C = { programa, nombre, sp, listaDetalles, insertar, modificar, recibo,
 *       conPagos, exigeCaja, validarEditable, filtrarPorVendedor, sinStock, soloContado,
 *       tcNota: 'venta' (tipo de cambio cobrado en la venta) | 'vigente' (ultimo de la tabla TipoCambio) }
 */
function crearRouterPV(C) {
const router = express.Router();
router.use(requierePrograma(C.programa));

const SP = C.sp;
const MAX_DIAS_LISTA = 370;
const MAX_FILAS_DETALLE = 300;
const CATEGORIA_DEFECTO = 3; // _PMOLimpiar: cbPrecios.Value = 3
const _esProducto = (f) => (Number(f.Tipo ?? f.tipo) || 1) === 1; // en la lista de venta, Tipo 2 = kit (su Id no es de producto)

// ---------- utilidades ----------
const dos = (n) => String(n).padStart(2, '0');
const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };
const r2 = (x) => Math.round(num(x) * 100) / 100;
// Total en dolares: Bs / tipo de cambio; 0 si no hay tipo de cambio (ej. venta a credito, sin VentasMonedaCobrada)
const aDolares = (bs, tc) => (num(tc) > 0 ? r2(num(bs) / num(tc)) : 0);
function isoDe(x) {
    if (!x) return null;
    if (x instanceof Date) return `${x.getUTCFullYear()}-${dos(x.getUTCMonth() + 1)}-${dos(x.getUTCDate())}`;
    const m = /^(\d{4})[-/](\d{2})[-/](\d{2})/.exec(String(x));
    return m ? `${m[1]}-${m[2]}-${m[3]}` : null;
}
function fechaReq(txt, campo) {
    const s = v.texto(txt, 10);
    const m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(s);
    const d = m ? new Date(Date.UTC(+m[1], +m[2] - 1, +m[3])) : null;
    if (!d || Number.isNaN(d.getTime()) || isoDe(d) !== s) throw new ErrorNegocio(`Ingrese una Fecha Valida: ${campo}`);
    return s;
}
const aVB = (iso) => iso.replace(/-/g, '/'); // .ToString("yyyy/MM/dd")
const ddmmyyyy = (iso) => { const [a, m, d] = iso.split('-'); return `${d}/${m}/${a}`; };
function hoyBolivia() { // Global_FechaActual (FechaBolivia = UTC-4)
    const a = new Date(Date.now() - 4 * 3600 * 1000);
    return `${a.getUTCFullYear()}-${dos(a.getUTCMonth() + 1)}-${dos(a.getUTCDate())}`;
}
function lector(filas) {
    const mapa = {};
    for (const k of Object.keys(filas[0] || {})) mapa[k.toLowerCase()] = k;
    return (f, n) => { const k = mapa[n.toLowerCase()]; if (k === undefined) return undefined; const x = f[k]; return Array.isArray(x) ? x[0] : x; };
}
const bit1 = (x) => x === true || Number(x) === 1;

// Global_* de la sesion (Tec_Login.vb)
function globales(s) {
    const suc = Number(s.gi_userSuc);
    return {
        rol: Number(s.gi_userRol),
        sucursal: Number.isFinite(suc) ? suc : -1,               // Global_Sucursal (-1 = todas)
        personal: Number(s.Global_IdPersonal) || 0,               // Global_IdPersonal
        modificarPrecio: s.Global_ModificarPrecio === 1,
        modificarDescuento: s.Global_ModificarDescuento === 1,
        verTodas: Number(s.gi_userRol) === 1 || s.Global_VentasTodos === 1,
        modificarAnteriores: s.Global_ModificarRegistro === 1,
    };
}

// _PMOGetListEstructuraBuscador (solo lo que usa la pantalla)
const CAMPOS_LISTA = ['Id', 'SucursalId', 'FechaVenta', 'HoraRegistro', 'PersonalId', 'Personal', 'TipoVenta', 'TVenta', 'FechaVencimientoCredito',
    'ClienteId', 'NombreCliente', 'Glosa', 'NombreAlmacen', 'TotalVenta', 'Descuento', 'Facturado', 'MontoBs', 'MontoDolares', 'TarjetaBancaria',
    'TransferenciaBancaria', 'TipoCambio', 'TDespachado', 'Despachante', 'CierreModulo',
    // FrmDespacho (btnDespachoVenta) usa la misma lista
    'EstadoDespacho', 'FechaDespacho', 'PersonalDespachadorId', 'ObservacionDespacho'];

function _filaLista(f) {
    return {
        ...f,
        FechaVenta: isoDe(f.FechaVenta),
        FechaVencimientoCredito: isoDe(f.FechaVencimientoCredito),
        HoraRegistro: f.HoraRegistro instanceof Date ? `${dos(f.HoraRegistro.getUTCHours())}:${dos(f.HoraRegistro.getUTCMinutes())}` : (f.HoraRegistro ?? ''),
        // MAM_Ventas @tipo 3 trae el TipoCambio cobrado (isnull(pago.TipoCambio,0)); MAM_Proforma @tipo 3 no lo trae (null)
        TotalDolares: f.TipoCambio === null || f.TipoCambio === undefined ? null : aDolares(f.TotalVenta, f.TipoCambio),
    };
}

// _PMOGetTablaBuscador
async function _lista(g, desde, hasta) {
    const dt = v.proyectar(await L.L_prListarVentasGeneralFiltroFecha(SP, aVB(desde), aVB(hasta), g.sucursal), CAMPOS_LISTA).map(_filaLista);
    return g.verTodas || !C.filtrarPorVendedor ? dt : dt.filter((f) => Number(f.PersonalId) === g.personal);
}

async function _buscarVenta(g, id, fechaIso) {
    const f = (await _lista(g, fechaIso, fechaIso)).find((x) => Number(x.Id) === Number(id));
    if (!f) throw new ErrorNegocio(`No se encontro la Venta ${id}. Actualice la lista e intente de nuevo.`, 404);
    return f;
}

// btnModificar / btnEliminar / menu de la fila
function _validarEditable(g, f, accion) {
    if (!C.validarEditable) return; // Tec_Proformas: btnModificar / btnEliminar sin estas reglas
    if (!g.modificarAnteriores && f.FechaVenta < hoyBolivia()) {
        throw new ErrorNegocio(`No puede ${accion} Ventas Anteriores a la fecha de Hoy, Consulte con su Administrador`.toUpperCase());
    }
    if (num(f.CierreModulo) > 0) {
        throw new ErrorNegocio(`No Es Posible ${accion} La Venta Ya que Pertenece A un cierre De Caja Cerrado # ${num(f.CierreModulo)}`);
    }
}

// _PMOValidarCampos: Caja Chica abierta (MAM_CierreCajero) para la sucursal y la fecha de la venta
async function _validarCaja(sucursal, fechaIso) {
    if (!C.exigeCaja) return; // Tec_Proformas no exige caja
    const dt = await L.L_prListarGeneral('MAM_CierreCajero');
    const c = lector(dt);
    const ok = dt.some((f) => Number(c(f, 'SucursalId')) === Number(sucursal) && Number(c(f, 'EstadoCaja')) === 1 && isoDe(c(f, 'Fecha')) === fechaIso);
    if (!ok) throw new ErrorNegocio(`No Es Posible Hacer EL Movimiento Por que no Existe Caja Chica con Estado Abierto Para Esta Fecha =${ddmmyyyy(fechaIso)}`);
}

async function _usaLote() {
    const dt = await L.L_prLeerConfiguracion();
    if (!dt.length) return false;
    return bit1(lector(dt)(dt[0], 'Lote'));
}

// =====================================================================
//  LISTA Y DATOS DE LA PANTALLA
// =====================================================================
router.get('/', asyncHandler(async (req, res) => {
    const desde = fechaReq(req.query.desde || hoyBolivia(), 'Desde');
    const hasta = fechaReq(req.query.hasta || hoyBolivia(), 'Hasta');
    if (desde > hasta) throw new ErrorNegocio('La Fecha Desde Debe Ser Menor Que la Fecha Hasta');
    if ((Date.parse(hasta) - Date.parse(desde)) / 86400000 > MAX_DIAS_LISTA) throw new ErrorNegocio('El rango maximo es de un año');
    res.json({ ok: true, data: await _lista(globales(req.sesion), desde, hasta) });
}));

// _prIniciarTodo + btnNuevo_Click
router.get('/inicio', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    const [sucursales, categoriasPrecio, lote, vendedor, clientes, tipoCambio] = await Promise.all([
        L.L_fnGeneralSucursales(),
        L.L_prListaCategoriasPrecios(),
        _usaLote(),
        L.ListarPersonalById(g.personal),
        L.L_prListarGeneral('MAM_Clientes'),
        L.L_ObtenerTipoCambio(),
    ]);
    const cats = v.proyectar(categoriasPrecio, ['Id', 'Descripcion']);
    res.json({
        ok: true,
        data: {
            sucursales: v.proyectar(sucursales, ['aanumi', 'aabdes']),
            sucursalUsuario: g.sucursal, // >= 0: fija (cbSucursal.ReadOnly)
            categoriasPrecio: cats,
            categoriaDefecto: cats.some((x) => Number(x.Id) === CATEGORIA_DEFECTO) ? CATEGORIA_DEFECTO : (cats[0]?.Id ?? ''),
            lote,
            tipoCambio,
            hoy: hoyBolivia(),
            exigeCaja: !!C.exigeCaja,
            sinStock: !!C.sinStock,
            conPagos: !!C.conPagos,
            vendedorDefecto: vendedor.length ? { Id: g.personal, Nombre: String(vendedor[0].Nombre ?? '') } : null,
            clienteDefecto: v.proyectar(clientes, ['Id', 'NombreCliente']).find((x) => Number(x.Id) === 1) || null,
            permisos: {
                modificarPrecio: g.modificarPrecio,
                modificarDescuento: g.modificarDescuento,
                modificarAnteriores: g.modificarAnteriores || !C.validarEditable,
            },
        },
    });
}));

// ButtonX1 (nueva venta): debe haber alguna Caja Chica abierta (de la sucursal del usuario si tiene una)
router.get('/caja-abierta', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    const dt = await L.L_prListarGeneral('MAM_CierreCajero');
    const c = lector(dt);
    const abierta = dt.some((f) => Number(c(f, 'EstadoCaja')) === 1 && (g.sucursal <= 0 || Number(c(f, 'SucursalId')) === g.sucursal));
    res.json({ ok: true, data: { abierta } });
}));

// btnCliente -> ListarCliente (busqueda en el servidor; maximo 50)
router.get('/clientes', asyncHandler(async (req, res) => {
    const palabras = v.texto(req.query.q, 60).toUpperCase().split(/\s+/).filter(Boolean);
    const dt = await L.ListarCliente();
    const c = lector(dt);
    const lista = [];
    for (const f of dt) {
        const fila = {
            Id: c(f, 'Id'),
            NombreCliente: String(c(f, 'NombreCliente') ?? c(f, 'NombreProveedor') ?? ''),
            DireccionCliente: String(c(f, 'DireccionCliente') ?? ''),
            Telefono: String(c(f, 'Telefono') ?? ''),
        };
        if (palabras.every((p) => `${fila.Id} ${fila.NombreCliente} ${fila.DireccionCliente} ${fila.Telefono}`.toUpperCase().includes(p))) lista.push(fila);
        if (lista.length >= 50) break;
    }
    res.json({ ok: true, data: lista });
}));

// Kaily FormularioCliente (Efecto tipo 7): crear cliente sin salir de la venta/proforma
//  Combos: Tipo Documento = libreria 8, Precios = L_prListaCategoriasPrecios (primer item por defecto)
router.get('/clientes/combos', asyncHandler(async (req, res) => {
    const [tipos, precios] = await Promise.all([L.L_prLibreriaDetalleGeneral(8), L.L_prListaCategoriasPrecios()]);
    res.json({ ok: true, data: { tiposDocumento: v.proyectar(tipos, ['cnnum', 'cndesc1']), categoriasPrecio: v.proyectar(precios, ['Id', 'Descripcion']) } });
}));

// ButtonX1_Click: _PMOValidarCampos (Nombre Cliente, Precios, Tipo Documento) e InsertarClienteFormularioExterno
//  como el escritorio: RazonSocial = Nombre y Nit = Nro Documento
router.post('/clientes', asyncHandler(async (req, res) => {
    const b = req.body || {};
    const nombre = v.requerido(b.NombreCliente, 'Nombre Cliente', 200);
    const precio = v.entero(b.PrecioCategoriaId, 'Precios', { requeridoPositivo: true });
    const tipoDoc = v.entero(b.TipoDocumento, 'Tipo Documento', { requeridoPositivo: true });
    const nro = v.texto(b.NroDocumento, 50);
    const telefono = v.texto(b.Telefono, 50);
    const dt = await L.InsertarClienteFormularioExterno('', nombre, tipoDoc, nro, nombre, nro, precio, telefono);
    if (!dt.length) throw new ErrorNegocio('No se pudo registrar el cliente'.toUpperCase());
    const c = lector(dt);
    const Id = c(dt[0], 'Id');
    res.json({ ok: true, data: { Id, NombreCliente: String(c(dt[0], 'NombreCliente') ?? nombre) }, mensaje: `Cliente ${Id} registrado con Exito.`.toUpperCase() });
}));

// Tec_VentasDetalle: productos y kits de la sucursal con el precio de la categoria (sin PrecioCosto)
const CAMPOS_PRODUCTO = ['Id', 'Tipo', 'CodigoExterno', 'NombreProducto', 'DescripcionProducto', 'NombreTipo', 'NombreCategoria', 'industria', 'stock', 'PrecioVenta'];
router.get('/productos', asyncHandler(async (req, res) => {
    const sucursal = v.entero(req.query.sucursal, 'una Sucursal', { requeridoPositivo: true });
    const precio = v.entero(req.query.precio, 'una Categoria de Precio', { requeridoPositivo: true });
    const dt = v.proyectar(await L.L_prSoloActivos(L.L_prListarProductosVentas(sucursal, precio), 'Id', _esProducto), CAMPOS_PRODUCTO); // sin desactivados
    res.json({ ok: true, data: dt.map((p) => ({ ...p, Tipo: num(p.Tipo) || 1 })) });
}));

// Productos de un kit (para mostrarlos al agregarlo)
router.get('/kits/:id', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    res.json({ ok: true, data: v.proyectar(await L.L_prListarProductosKitsVenta(id), ['ProductoId', 'NombreProducto', 'Cantidad', 'PrecioVenta']) });
}));

const CAMPOS_DETALLE = ['Id', 'ProductoId', 'Producto', 'Cantidad', 'Precio', 'SubTotal', 'ProcentajeDescuento', 'MontoDescuento', 'Total', 'stock',
    'Tipo', 'TipoNombre', 'KitId', 'KitNombre', 'CantidadKit'];
router.get('/:id/detalle', asyncHandler(async (req, res) => {
    const id = v.idRuta(req.params.id);
    res.json({ ok: true, data: v.proyectar(await C.listaDetalles(id), CAMPOS_DETALLE).map((f) => ({ ...f, Tipo: num(f.Tipo) || 1 })) });
}));

// =====================================================================
//  GRABAR / MODIFICAR
// =====================================================================
function _leerCabecera(b) {
    const FechaVenta = fechaReq(b.FechaVenta, 'Fecha Venta');
    // Tec_Proformas: ProformaInsertar(..., 1, Now.Date, ...) -> TipoVenta 1 y vencimiento = hoy
    if (C.soloContado) {
        return {
            AlmacenId: v.entero(b.SucursalId, 'una Sucursal', { requeridoPositivo: true }),
            FechaVenta, ClienteId: v.entero(b.ClienteId, 'un Cliente', { requeridoPositivo: true }), TipoVenta: 1, FechaVencCredito: hoyBolivia(),
            glosa: v.texto(b.Glosa, 500), Descuento: Math.max(0, v.decimal(b.Descuento)), Facturado: 0,
            PrecioId: v.entero(b.PrecioId ?? CATEGORIA_DEFECTO, 'una Categoria de Precio'),
        };
    }
    const TipoVenta = v.bit(b.TipoVenta); // swTipoVenta: CONTADO = 1, CREDITO = 0
    return {
        AlmacenId: v.entero(b.SucursalId, 'una Sucursal', { requeridoPositivo: true }),
        FechaVenta,
        ClienteId: v.entero(b.ClienteId, 'un Cliente', { requeridoPositivo: true }),
        TipoVenta,
        // VB envia siempre la fecha del control (tambien en contado)
        FechaVencCredito: b.FechaVencimientoCredito ? fechaReq(b.FechaVencimientoCredito, 'Venc. Credito') : FechaVenta,
        glosa: v.texto(b.Glosa, 500),
        Descuento: Math.max(0, v.decimal(b.Descuento)),
        Facturado: v.bit(b.Facturado),
        PrecioId: v.entero(b.PrecioId ?? CATEGORIA_DEFECTO, 'una Categoria de Precio'),
    };
}

/**
 * P_PonerTotal + CellValueChanged (Kaily: la cantidad es en unidades y el precio es por unidad)
 *   SubTotal = Cantidad * Precio
 *   % (0..100] -> MontoDescuento = SubTotal * % / 100 ; si no, monto (0..SubTotal] -> % = monto * 100 / SubTotal
 *   Total = SubTotal - MontoDescuento
 */
function _calcular(f, cantidad, porc, monto) {
    if (!(cantidad > 0)) throw new ErrorNegocio(`La Cantidad de ${f.Producto} debe ser Mayor a 0`);
    const sub = cantidad * num(f.Precio);
    let p = num(porc);
    let m = num(monto);
    if (p > 0 && p <= 100) m = sub * (p / 100);
    else if (m > 0 && m <= sub) p = sub ? (m * 100) / sub : 0;
    else { p = 0; m = 0; }
    return { ...f, Cantidad: cantidad, SubTotal: sub, ProcentajeDescuento: p, MontoDescuento: m, Total: sub - m };
}

// grdetalle_CellEdited: el precio no puede ser menor al costo
function _precio(g, entrada, actual, costo, nombre) {
    if (!g.modificarPrecio || entrada.Precio === undefined || entrada.Precio === null || entrada.Precio === '') return actual;
    const p = num(entrada.Precio);
    if (p < costo - 1e-9) throw new ErrorNegocio(`${nombre}: El Precio Es Menor Al Precio De Costo Del Producto que Es = ${r2(costo)}`);
    return p;
}

const _base = (id) => ({
    Id: id, VentaId: 0, ProductoId: 0, Producto: '', Cantidad: 0, Precio: 0, SubTotal: 0, ProcentajeDescuento: 0, MontoDescuento: 0, Total: 0,
    Detalle: '', PrecioCosto: 0, Lote: '20200101', FechaVencimiento: new Date(Date.UTC(2020, 0, 1)), estado: 0, img: null, stock: 0,
    Tipo: 1, TipoNombre: 'Productos', KitId: 0, KitNombre: '', CantidadKit: 0,
});

/**
 * Arma el DataTable del grid como lo envia VB a @VentaDetalleType:
 *  columnas en el orden del SELECT de ListaVentasDetalles (@tipo 4) y TODAS las filas existentes (las quitadas con -1).
 *  entradas: [{ Id, eliminado, Cantidad, Precio, ProcentajeDescuento, MontoDescuento }]                  filas existentes
 *            [{ nuevo, Tipo: 1, ProductoId, PrecioId, Cantidad, Precio, ProcentajeDescuento, MontoDescuento }] productos
 *            [{ nuevo, Tipo: 2, KitId, PrecioId, CantidadKit }]                                              kits
 */
async function _armarDetalle(g, { ventaId, cab, entradas, existentes }) {
    if (!Array.isArray(entradas)) throw new ErrorNegocio('Por Favor Inserte un Detalle'.toUpperCase());
    if (entradas.length > MAX_FILAS_DETALLE) throw new ErrorNegocio(`El detalle no puede tener mas de ${MAX_FILAS_DETALLE} filas`);

    const columnas = L._columnasDe(await C.listaDetalles(ventaId ?? -1));
    const cE = lector(existentes);
    const cache = new Map();
    const productosDe = async (precioId) => {
        if (!cache.has(precioId)) {
            const dt = await L.L_prSoloActivos(L.L_prListarProductosVentas(cab.AlmacenId, precioId), 'Id', _esProducto); // sin desactivados
            cache.set(precioId, { dt, c: lector(dt) });
        }
        return cache.get(precioId);
    };

    // Stock insuficiente: se juntan TODOS los productos para marcarlos en pantalla (no solo el primero)
    const faltan = [];
    const faltaStock = (f, solicitado) => faltan.push({ ProductoId: num(f.ProductoId), Producto: String(f.Producto ?? '').trim(), stock: num(f.stock), solicitado: num(solicitado) });

    const filas = [];
    const productosUsados = new Set();
    const kitsUsados = new Set();
    let mayorId = existentes.reduce((m, f) => Math.max(m, num(cE(f, 'Id'))), 0); // _GenerarId

    // 1) filas existentes: sin cambios (1), modificadas (2) o quitadas (-1)
    for (const ex of existentes) {
        const fila = { ..._base(num(cE(ex, 'Id'))) };
        for (const col of columnas) fila[col] = cE(ex, col);
        Object.assign(fila, {
            Id: num(cE(ex, 'Id')), ProductoId: num(cE(ex, 'ProductoId')), Producto: String(cE(ex, 'Producto') ?? ''), Precio: num(cE(ex, 'Precio')),
            PrecioCosto: num(cE(ex, 'PrecioCosto')), stock: num(cE(ex, 'stock')), Tipo: num(cE(ex, 'Tipo')) || 1, KitId: num(cE(ex, 'KitId')),
        });
        const e = entradas.find((x) => x && !x.nuevo && x.Id !== undefined && x.Id !== null && num(x.Id) === fila.Id);
        if (!e || e.eliminado) { filas.push({ ...fila, estado: -1 }); continue; }
        if (fila.Tipo === 2) { // productos de un kit: no se editan (grdetalle_EditingCell)
            kitsUsados.add(fila.KitId);
            filas.push({ ...fila, estado: 1 });
            continue;
        }
        if (productosUsados.has(fila.ProductoId)) throw new ErrorNegocio(`El producto ${fila.Producto} esta repetido en el detalle`.toUpperCase());
        productosUsados.add(fila.ProductoId);
        const precio = _precio(g, e, fila.Precio, fila.PrecioCosto, fila.Producto);
        const cant = num(e.Cantidad);
        const porc = g.modificarDescuento ? e.ProcentajeDescuento : num(cE(ex, 'ProcentajeDescuento'));
        const monto = g.modificarDescuento ? e.MontoDescuento : 0;
        const calc = _calcular({ ...fila, Precio: precio }, cant, porc, monto);
        const cambio = Math.abs(cant - num(cE(ex, 'Cantidad'))) > 1e-6 || Math.abs(precio - num(cE(ex, 'Precio'))) > 1e-6
            || Math.abs(calc.MontoDescuento - num(cE(ex, 'MontoDescuento'))) > 1e-6;
        if (cambio && !C.sinStock && cant > fila.stock + 1e-9) faltaStock(fila, cant);
        filas.push({ ...calc, estado: cambio ? 2 : 1 });
    }

    // 2) productos y kits nuevos (estado 0). Los agregados y luego quitados no se envian.
    for (const e of entradas) {
        if (!e || !e.nuevo || e.eliminado) continue;
        const precioId = v.entero(e.PrecioId ?? cab.PrecioId, 'una Categoria de Precio', { requeridoPositivo: true });
        const { dt, c } = await productosDe(precioId);
        if (num(e.Tipo) === 2) {
            const kitId = v.entero(e.KitId, 'un Kit', { requeridoPositivo: true });
            const kit = dt.find((p) => num(c(p, 'Id')) === kitId && num(c(p, 'Tipo')) === 2);
            if (!kit) throw new ErrorNegocio(`El kit ${kitId} no esta disponible para esta sucursal`);
            const nombreKit = String(c(kit, 'NombreProducto') ?? '');
            if (kitsUsados.has(kitId)) throw new ErrorNegocio('El Kit Ya Existe En el Detalle'.toUpperCase());
            kitsUsados.add(kitId);
            const cantKit = num(e.CantidadKit);
            if (!(cantKit > 0)) throw new ErrorNegocio(`La Cantidad del kit ${nombreKit} debe ser Mayor a 0`);
            if (!C.sinStock && !(num(c(kit, 'stock')) > 0)) throw new ErrorNegocio('No Existe Stock del Producto Para Proceder con la Venta'.toUpperCase());
            if (!C.sinStock && cantKit > num(c(kit, 'stock')) + 1e-9) throw new ErrorNegocio(`La cantidad de ${nombreKit} es Superior Al Stock Disponible = ${num(c(kit, 'stock'))}`);
            const comp = await L.L_prListarProductosKitsVenta(kitId);
            const k = lector(comp);
            if (!comp.length) throw new ErrorNegocio(`El kit ${nombreKit} no tiene productos`);
            for (const p of comp) {
                mayorId += 1;
                const cant = num(k(p, 'Cantidad')) * cantKit;
                const precio = num(k(p, 'PrecioVenta'));
                filas.push({
                    ..._base(mayorId), ProductoId: num(k(p, 'ProductoId')), Producto: String(k(p, 'NombreProducto') ?? ''), Cantidad: cant, Precio: precio,
                    SubTotal: precio * cant, Total: precio * cant, PrecioCosto: num(k(p, 'PrecioCosto')), stock: num(k(p, 'Cantidad')),
                    Tipo: 2, TipoNombre: 'Kits', KitId: kitId, KitNombre: nombreKit, CantidadKit: cantKit, estado: 0,
                });
            }
            continue;
        }
        const productoId = v.entero(e.ProductoId, 'un Producto', { requeridoPositivo: true });
        const prod = dt.find((p) => num(c(p, 'Id')) === productoId && (num(c(p, 'Tipo')) || 1) === 1);
        if (!prod) throw new ErrorNegocio(`El producto ${productoId} no esta disponible para esta sucursal`);
        if (productosUsados.has(productoId)) throw new ErrorNegocio('El producto ya existe en el detalle'.toUpperCase());
        productosUsados.add(productoId);
        mayorId += 1;
        const fila = {
            ..._base(mayorId), ProductoId: productoId, Producto: String(c(prod, 'NombreProducto') ?? ''), Precio: num(c(prod, 'PrecioVenta')),
            PrecioCosto: num(c(prod, 'PrecioCosto')), stock: num(c(prod, 'stock')),
        };
        const precio = _precio(g, e, fila.Precio, fila.PrecioCosto, fila.Producto);
        const calc = _calcular({ ...fila, Precio: precio }, num(e.Cantidad), g.modificarDescuento ? e.ProcentajeDescuento : 0, g.modificarDescuento ? e.MontoDescuento : 0);
        if (!C.sinStock && calc.Cantidad > fila.stock + 1e-9) faltaStock(fila, calc.Cantidad); // incluye stock 0
        filas.push({ ...calc, estado: 0 });
    }

    if (faltan.length) throw L.L_prErrorSinStock(faltan, `grabar la ${C.nombre.toLowerCase()}`);

    // Kaily: un producto con precio 0 se puede elegir, pero no se graba hasta que tenga precio (filas nuevas o modificadas;
    // las de un kit toman el precio del kit). El stock ya se valida arriba con el stock vigente de la BD.
    const sinPrecio = filas.filter((f) => (f.estado === 0 || f.estado === 2) && num(f.Tipo) !== 2 && !(num(f.Precio) > 0)).map((f) => f.Producto);
    if (sinPrecio.length) {
        const nombres = sinPrecio.length > 5 ? `${sinPrecio.slice(0, 5).join(', ')} y ${sinPrecio.length - 5} mas` : sinPrecio.join(', ');
        throw new ErrorNegocio(`No se puede grabar: ${nombres} sin precio de venta. Coloque el precio en Precios.`);
    }

    const vigentes = filas.filter((f) => f.estado >= 0);
    if (!vigentes.length) throw new ErrorNegocio('Por Favor Inserte un Detalle'.toUpperCase());

    // solo las columnas del tipo, en el orden del SELECT (por posicion, como ADO.NET)
    const filasTVP = filas.map((f) => {
        const o = {};
        for (const col of columnas) {
            const k = Object.keys(f).find((x) => x.toLowerCase() === col.toLowerCase());
            o[col] = k === undefined ? null : f[k];
        }
        return o;
    });
    return { detalle: { columnas, filas: filasTVP }, suma: vigentes.reduce((s, f) => s + f.Total, 0) };
}

/**
 * EfectivoCobranza (Efecto tipo 20): solo en contado.
 *   TotalPagado = MontoBs + MontoDolares * TipoCambio + Tarjeta + Transferencia >= TotalVenta
 *   dt.Rows.Add(0, 0, MontoBs, MontoDolares, Tarjeta, Transferencia, Global_TipoCambio, 0)
 */
async function _armarPagos(tipoVenta, pago, total) {
    if (!C.conPagos) return null; // Tec_Proformas: sin @VentaPagos
    const columnas = L._columnasDe(await L.ListaVentasDetallePago(-1));
    if (tipoVenta !== 1) return { columnas, filas: [] };
    const p = pago || {};
    const montos = [p.MontoBs, p.MontoDolares, p.Tarjeta, p.Transferencia].map((x) => Math.max(0, v.decimal(x)));
    const tc = await L.L_ObtenerTipoCambio();
    const pagado = montos[0] + montos[1] * tc + montos[2] + montos[3];
    if (r2(pagado) < r2(total)) throw new ErrorNegocio('El Monto Pagado Es Menor Al Monto Total De La Venta');
    return { columnas, filas: [[0, 0, montos[0], montos[1], montos[2], montos[3], tc, 0]] };
}

/**
 * El SP (MAM_Ventas @1/@2) valida el stock dentro de su transaccion, con las filas bloqueadas: es la ultima palabra
 * (web y escritorio). Si otro usuario vendio el mismo producto entre que se armo el detalle y se grabo, el SP
 * responde "STOCK INSUFICIENTE: ..."; aqui se vuelve a armar el detalle con el stock que quedo, para devolver
 * la lista de productos y su stock actual y que la pantalla los marque.
 */
async function _grabarConStock(grabar, rearmar) {
    try {
        return await grabar();
    } catch (e) {
        if (!/STOCK INSUFICIENTE/i.test(e.message) || C.sinStock) throw e;
        await rearmar(); // lanza el error con la lista (stock actual de cada producto)
        throw new ErrorNegocio(e.message);
    }
}

function _validarDescuento(descuento, suma) {
    if (descuento > suma + 1e-9) throw new ErrorNegocio('El total Ingresado es Superior al monto total de la venta');
}

router.post('/', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    if (await _usaLote()) throw new ErrorNegocio('La configuracion del sistema usa LOTES: la venta por lotes aun no esta disponible en la web. Use el escritorio.');
    const cab = _leerCabecera(req.body || {});
    if (g.sucursal >= 0) cab.AlmacenId = g.sucursal; // cbSucursal fijo en la sucursal del usuario
    if (!(g.personal > 0)) throw new ErrorNegocio('Seleccione un Personal: su usuario no tiene personal asignado.');
    await _validarCaja(cab.AlmacenId, cab.FechaVenta);
    const { detalle, suma } = await _armarDetalle(g, { ventaId: -1, cab, entradas: req.body.detalle, existentes: [] });
    _validarDescuento(cab.Descuento, suma);
    const total = suma - cab.Descuento;
    const pagos = await _armarPagos(cab.TipoVenta, req.body.pago, total);
    const r = await _grabarConStock(() => C.insertar({
        _numi: '0', AlmacenId: cab.AlmacenId, FechaTransacccion: aVB(cab.FechaVenta), PersonalId: g.personal, ClienteId: cab.ClienteId,
        TipoVenta: cab.TipoVenta, FechaVencCredito: aVB(cab.FechaVencCredito), glosa: cab.glosa, TotalCompra: total, Descuento: cab.Descuento, Facturado: cab.Facturado,
    }, detalle, pagos), () => _armarDetalle(g, { ventaId: -1, cab, entradas: req.body.detalle, existentes: [] }));
    if (!r.resultado) throw new ErrorNegocio(`Error al guardar la ${C.nombre}`.toUpperCase());
    res.json({ ok: true, id: r._numi, fecha: cab.FechaVenta, mensaje: `Codigo de ${C.nombre} ${r._numi} Grabado con Exito.`.toUpperCase() });
}));

router.put('/:id', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    const id = v.idRuta(req.params.id);
    const original = await _buscarVenta(g, id, fechaReq(req.body?.fechaOriginal, 'Fecha original'));
    _validarEditable(g, original, 'Modificar');
    if (await _usaLote()) throw new ErrorNegocio('La configuracion del sistema usa LOTES: modifique esta venta desde el escritorio.');
    const cab = _leerCabecera(req.body || {});
    if (g.sucursal >= 0) cab.AlmacenId = g.sucursal;
    await _validarCaja(cab.AlmacenId, cab.FechaVenta);
    const existentes = await C.listaDetalles(id);
    const { detalle, suma } = await _armarDetalle(g, { ventaId: id, cab, entradas: req.body.detalle, existentes });
    _validarDescuento(cab.Descuento, suma);
    const total = suma - cab.Descuento;
    const pagos = await _armarPagos(cab.TipoVenta, req.body.pago, total);
    const r = await _grabarConStock(() => C.modificar({
        _numi: id, AlmacenId: cab.AlmacenId, FechaTransacccion: aVB(cab.FechaVenta), PersonalId: num(original.PersonalId), ClienteId: cab.ClienteId,
        TipoVenta: cab.TipoVenta, FechaVencCredito: aVB(cab.FechaVencCredito), glosa: cab.glosa, TotalCompra: total, Descuento: cab.Descuento, Facturado: cab.Facturado,
    }, detalle, pagos), async () => _armarDetalle(g, { ventaId: id, cab, entradas: req.body.detalle, existentes: await C.listaDetalles(id) }));
    if (!r.resultado) throw new ErrorNegocio(`Error al Modificar la ${C.nombre}`.toUpperCase());
    res.json({ ok: true, id, fecha: cab.FechaVenta, mensaje: `Codigo de ${C.nombre} ${id} Modificado con Exito.`.toUpperCase() });
}));

router.delete('/:id', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    const id = v.idRuta(req.params.id);
    const original = await _buscarVenta(g, id, fechaReq(req.query.fecha, 'Fecha'));
    _validarEditable(g, original, 'Eliminar');
    if (!(await L.L_prBorrarRegistro(id, SP))) throw new ErrorNegocio(`No se pudo eliminar la ${C.nombre}.`);
    res.json({ ok: true, mensaje: `Codigo de ${C.nombre} ${id} eliminado con Exito.`.toUpperCase() });
}));

// =====================================================================
//  NOTA DE VENTA (P_GenerarReporte): ListarVentaRecibo (@tipo 6). Total = Suma(Total) - DescuentoVenta
// =====================================================================
router.get('/:id/recibo', asyncHandler(async (req, res) => {
    const g = globales(req.sesion);
    const id = v.idRuta(req.params.id);
    const venta = await _buscarVenta(g, id, fechaReq(req.query.fecha, 'Fecha'));
    res.json({ ok: true, data: await _recibo(id, venta) });
}));

// datos de la nota (tambien la "NOTA DE DESPACHO" de FrmDespacho)
async function _recibo(id, venta) {
    const [detalle, empresas, imagen, rec, tcVigente] = await Promise.all([
        C.listaDetalles(id),
        L.L_prListarGeneral('MAM_Empresas'),
        L.ObtenerImagenEmpresa().catch(() => []),
        C.recibo(id).catch(() => []),
        C.tcNota === 'vigente' ? L.L_ObtenerTipoCambio() : null,
    ]);
    // total en dolares: venta = el tipo de cambio cobrado (el mismo de la lista, 0 en credito);
    // proforma = el vigente (MAM_Proforma @tipo 6 lo busca en VentasMonedaCobrada con el Id de la proforma, que es otra venta)
    const tipoCambio = C.tcNota === 'vigente' ? num(tcVigente) : num(venta.TipoCambio);
    const r0 = rec[0] || {};
    const c0 = lector(rec);
    const de = (...n) => { for (const x of n) { const y = c0(r0, x); if (y !== undefined && y !== null && y !== '') return y; } return null; };
    const det = v.proyectar(detalle, ['ProductoId', 'Producto', 'Cantidad', 'Precio', 'SubTotal', 'MontoDescuento', 'Total', 'KitNombre']);
    const suma = det.reduce((s, f) => s + num(f.Total), 0);
    const total = suma - num(venta.Descuento);
    return {
        venta,
        detalle: det,
        subtotal: suma,
        total,
        tipoCambio,
        totalDolares: aDolares(total, tipoCambio),
        empresa: v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Nit', 'Ciudad'])[0] || null,
        cliente: { Nit: de('Nit', 'NitCi', 'NroDocumento'), Telefono: de('Telefono', 'Telefono01'), Direccion: de('Direccion', 'DireccionCliente') },
        codigos: Object.fromEntries(rec.map((f) => [String(c0(f, 'Producto') ?? c0(f, 'NombreProducto') ?? ''), c0(f, 'CodigoExterno') ?? null]).filter(([k, x]) => k && x !== null)),
        imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '',
    };
}

router._interno = { _armarDetalle, _armarPagos, _leerCabecera, _calcular, _buscarVenta, _lista, _recibo, globales, fechaReq, hoyBolivia, isoDe };
return router;
}

// Tec_Ventas.vb (btVentVenta)
const routerVentas = crearRouterPV({
    programa: 'btVentVenta', nombre: 'Venta', sp: 'MAM_Ventas',
    listaDetalles: (id) => L.ListaVentasDetalles(id),
    insertar: (x, d, p) => L.VentaInsertar(x, d, p),
    modificar: (x, d, p) => L.VentaModificar(x, d, p),
    recibo: (id) => L.ListarVentaRecibo(id),
    conPagos: true, exigeCaja: true, validarEditable: true, filtrarPorVendedor: true, sinStock: false, soloContado: false, tcNota: 'venta',
});

module.exports = routerVentas;
module.exports.crearRouterPV = crearRouterPV;
module.exports._interno = routerVentas._interno;
