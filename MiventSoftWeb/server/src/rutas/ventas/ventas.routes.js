// =====================================================================
//  Tec_Ventas.vb + Tec_VentasDetalle.vb + frm_CantidadCajas.vb  (programa: btVentVenta)
//
//  Mismo flujo que el escritorio:
//   * Buscador por rango de fechas: L_prListarGeneralFechas("MAM_Ventas", desde, hasta)
//   * Tres formas de crear (botones de la lista):
//       "Nuevo Pedido"                    -> VentaDirecta=0, EstadoPedido=1
//       "Nueva Venta Directa"             -> VentaDirecta=1, EstadoPedido=4 (exige conciliacion abierta del vendedor)
//       "Venta Directa Sin Conciliacion"  -> VentaDirecta=1 + SinConciliacion=1 (solo usuarios con Usuarios.pedido=1)
//   * Detalle: productos de L_prListarProductosVentas (@tipo 5) o, en venta directa,
//     L_prListarProductosVentasConciliaciones (@tipo 20). Precio unitario = PrecioVenta / Conversion.
//   * Estados de fila del detalle (igual que el DataTable del grid de VB):
//       0 nuevo · 1 sin cambios · 2 modificado · -1 eliminado (existia) · -2 eliminado (era nuevo)
//   * Total = Suma(Total detalle) - Descuento + Incremento (incremento de HOY solo en ventas nuevas;
//     en modificacion el incremento REGISTRADO de esa venta).
//   * No se modifica ni elimina una venta de una conciliacion cerrada (EstadoConciliacion = 0).
//
//  Mejoras de seguridad respecto al escritorio (el resultado en BD es el mismo):
//   * El navegador NO envia precios ni totales: el servidor los toma de los mismos SP y los calcula.
//   * El precio de costo nunca sale al navegador.
//   * El stock se vuelve a validar en el servidor al grabar.
// =====================================================================
const express = require('express');
const L = require('../../negocio/AccesoLogica');
const { requierePrograma } = require('../../middleware/permisos');
const { asyncHandler, ErrorNegocio } = require('../../middleware/errores');
const v = require('../../utils/validar');

/**
 * Mismo programa para Tec_Ventas (MAM_Ventas) y Proforma (MAM_Proforma): el escritorio
 * tiene ambos formularios casi identicos. C = configuracion de cada uno.
 */
function crearRouter(C) {
    const router = express.Router();
    router.use(requierePrograma(C.programa));

    const MAX_DIAS_LISTA = 370;
    const MAX_FILAS_DETALLE = 300;
    const INCREMENTO_DEFECTO = 2.0; // _prCargarIncrementoVenta: si nunca se configuro, 2.00

    // ---------- utilidades ----------
    const dos = (n) => String(n).padStart(2, '0');
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
    function hoyBolivia() {
        const a = new Date(Date.now() - 4 * 3600 * 1000);
        return `${a.getUTCFullYear()}-${dos(a.getUTCMonth() + 1)}-${dos(a.getUTCDate())}`;
    }
    const num = (x) => { const n = Number(x); return Number.isFinite(n) ? n : 0; };

    // Lector de columnas sin distinguir mayusculas (Conversion/conversion, Stock/stock...)
    function lector(filas) {
        const mapa = {};
        for (const k of Object.keys(filas[0] || {})) mapa[k.toLowerCase()] = k;
        return (f, n) => { const k = mapa[n.toLowerCase()]; return k === undefined ? undefined : f[k]; };
    }

    // _PMOGetListEstructuraBuscador (solo lo que usa la pantalla)
    const CAMPOS_LISTA = ['Id', 'SucursalId', 'FechaVenta', 'HoraRegistro', 'PersonalId', 'Personal', 'TipoVenta', 'TVenta', 'FechaVencimientoCredito',
        'ClienteId', 'NombreCliente', 'Glosa', 'TotalVenta', 'Descuento', 'EstadoPedido', 'NombreEstado', 'VentaDirecta', 'VentaDirectaSinConciliacion',
        'FechaEntrega', 'NombrePersonal', 'EstadoConciliacion', 'conciliacionId', 'IncrementoVenta'];

    function _filaLista(f) {
        return {
            ...f,
            FechaVenta: isoDe(f.FechaVenta),
            FechaVencimientoCredito: isoDe(f.FechaVencimientoCredito),
            FechaEntrega: isoDe(f.FechaEntrega),
            HoraRegistro: f.HoraRegistro instanceof Date
                ? `${dos(f.HoraRegistro.getUTCHours())}:${dos(f.HoraRegistro.getUTCMinutes())}`
                : (f.HoraRegistro ?? ''),
        };
    }

    // Venta tal como la ve el buscador (para validar conciliacion y tomar los datos originales)
    async function _buscarVenta(id, fechaIso) {
        const dt = await L.L_prListarGeneralFechas(C.sp, aVB(fechaIso), aVB(fechaIso));
        const f = v.proyectar(dt, CAMPOS_LISTA).find((x) => Number(x.Id) === Number(id));
        if (!f) throw new ErrorNegocio(`No se encontro la Venta ${id}. Actualice la lista e intente de nuevo.`, 404);
        return _filaLista(f);
    }

    const _conciliacionCerrada = (f) => f.EstadoConciliacion !== null && f.EstadoConciliacion !== undefined && Number(f.EstadoConciliacion) === 0;

    async function _usuarioPuedeSinConciliacion(gi_userNumi) {
        const dt = await L.L_prUsuarioGeneral();
        const c = lector(dt);
        return dt.some((u) => Number(c(u, 'Id')) === Number(gi_userNumi) && (Number(c(u, 'pedido')) === 1 || c(u, 'pedido') === true));
    }

    async function _incrementoHoy() {
        if (!C.conIncremento) return 0; // Proforma: Total = Suma - Descuento (sin incremento)
        const dt = await L.L_fnObtenerIncrementoVentaHoy();
        if (dt.length > 0) { const c = lector(dt); return num(c(dt[0], 'Monto')); }
        return INCREMENTO_DEFECTO;
    }

    async function _usaLote() {
        const dt = await L.L_prLeerConfiguracion();
        if (!dt.length) return false;
        const c = lector(dt);
        const x = c(dt[0], 'Lote');
        return x === true || Number(x) === 1;
    }

    // Productos disponibles: mismo SP que Tec_VentasDetalle._prCargarProductos
    // ProformaDetalle._prCargarProductos: siempre @tipo 5 (todos los productos, tambien sin stock)
    async function _productos(sucursal, cliente, precioId, ventaDirectaVariable, distribuidorId) {
        return ventaDirectaVariable === 1 && !C.sinStock
            ? L.L_prListarProductosVentasConciliaciones(sucursal, cliente, distribuidorId, precioId)
            : L.L_prListarProductosVentas(sucursal, cliente, precioId);
    }

    // =====================================================================
    //  LISTA
    // =====================================================================
    router.get('/', asyncHandler(async (req, res) => {
        const desde = fechaReq(req.query.desde || hoyBolivia(), 'Desde');
        const hasta = fechaReq(req.query.hasta || hoyBolivia(), 'Hasta');
        if (desde > hasta) throw new ErrorNegocio('La fecha Desde no puede ser mayor que Hasta');
        if ((Date.parse(hasta) - Date.parse(desde)) / 86400000 > MAX_DIAS_LISTA) throw new ErrorNegocio('El rango maximo es de un año');
        const dt = await L.L_prListarGeneralFechas(C.sp, aVB(desde), aVB(hasta));
        res.json({ ok: true, data: v.proyectar(dt, CAMPOS_LISTA).map(_filaLista) });
    }));

    // Datos de arranque de la pantalla (_prIniciarTodo)
    router.get('/inicio', asyncHandler(async (req, res) => {
        const [sucursales, estados, categoriasPrecio, lote, incremento, sinConciliacion, vendedor, clientes] = await Promise.all([
            L.L_fnGeneralSucursales(),
            L.L_fnPedidosEstados(),
            L.L_prListaCategoriasPrecios(),
            _usaLote(),
            _incrementoHoy(),
            _usuarioPuedeSinConciliacion(req.sesion.gi_userNumi),
            L.ListarPersonalById(req.sesion.Global_IdPersonal ?? 0),
            L.L_prListarGeneral('MAM_Clientes'),
        ]);
        // btnNuevo_Click: vendedor = personal del usuario; cliente por defecto = Id 1
        const cliDef = v.proyectar(clientes, ['Id', 'NombreCliente']).find((x) => Number(x.Id) === 1) || null;
        res.json({
            ok: true,
            data: {
                sucursales: v.proyectar(sucursales, ['aanumi', 'aabdes']),
                estados: v.proyectar(estados, { id: 'id', NombreEstado: 'NombreEstado' }),
                categoriasPrecio: v.proyectar(categoriasPrecio, ['Id', 'Descripcion']),
                lote,
                incrementoHoy: incremento,
                permiteSinConciliacion: sinConciliacion,
                vendedorDefecto: vendedor.length ? { Id: Number(req.sesion.Global_IdPersonal), Nombre: String(vendedor[0].Nombre ?? '') } : null,
                clienteDefecto: cliDef,
            },
        });
    }));

    // btnVendedor -> ListarPersonal
    router.get('/personal', asyncHandler(async (req, res) => {
        res.json({ ok: true, data: v.proyectar(await L.ListarPersonal(), ['Id', 'Nombre', 'Direccion', 'Telefono01']) });
    }));

    // btnCliente -> ListarCliente (busqueda en el servidor; se envian como maximo 50)
    router.get('/clientes', asyncHandler(async (req, res) => {
        const q = v.texto(req.query.q, 60).toUpperCase();
        const dt = await L.ListarCliente();
        const c = lector(dt);
        const palabras = q.split(/\s+/).filter(Boolean);
        const lista = [];
        for (const f of dt) {
            const fila = {
                Id: c(f, 'Id'),
                NombreCliente: String(c(f, 'NombreCliente') ?? c(f, 'NombreProveedor') ?? ''),
                DireccionCliente: String(c(f, 'DireccionCliente') ?? ''),
                Telefono: String(c(f, 'Telefono') ?? ''),
            };
            const texto = `${fila.Id} ${fila.NombreCliente} ${fila.DireccionCliente} ${fila.Telefono}`.toUpperCase();
            if (palabras.every((p) => texto.includes(p))) lista.push(fila);
            if (lista.length >= 50) break;
        }
        res.json({ ok: true, data: lista });
    }));

    // Tec_VentasDetalle: productos con stock y precio para la categoria elegida
    router.get('/productos', asyncHandler(async (req, res) => {
        const sucursal = v.entero(req.query.sucursal, 'Sucursal', { requeridoPositivo: true });
        const cliente = v.entero(req.query.cliente, 'Cliente', { requeridoPositivo: true });
        const precio = v.entero(req.query.precio, 'Categoria de Precio', { requeridoPositivo: true });
        const directa = v.bit(req.query.directa) ? 1 : 0;
        const vendedor = directa ? v.entero(req.query.vendedor, 'Vendedor', { requeridoPositivo: true }) : 0;
        const dt = await _productos(sucursal, cliente, precio, directa, vendedor);
        // PrecioCosto NO se envia al navegador
        res.json({ ok: true, data: v.proyectar(dt, ['Id', 'CodigoExterno', 'NombreProducto', 'DescripcionProducto', 'NombreCategoria', 'Conversion', 'stock', 'stockCajas', 'PrecioVenta']) });
    }));

    // Venta directa: L_prVerificarConcicliacionAbierta (se consulta al pulsar "Agregar Productos")
    router.get('/conciliacion/:vendedor', asyncHandler(async (req, res) => {
        const id = v.idRuta(req.params.vendedor);
        const dt = await L.L_prVerificarConcicliacionAbierta(id);
        res.json({ ok: true, data: { abierta: dt.length > 0 && num(Object.values(dt[0])[0]) !== 0 } });
    }));

    const CAMPOS_DETALLE = ['Id', 'ProductoId', 'Producto', 'Cantidad', 'CantidadUnitaria', 'Precio', 'SubTotal', 'ProcentajeDescuento', 'MontoDescuento', 'Total', 'Conversion', 'stock'];

    router.get('/:id/detalle', asyncHandler(async (req, res) => {
        const id = v.idRuta(req.params.id);
        res.json({ ok: true, data: v.proyectar(await C.listaDetalles(id), CAMPOS_DETALLE) });
    }));

    // =====================================================================
    //  GRABAR / MODIFICAR
    // =====================================================================

    // Cabecera (_PMOValidarCampos + _ValidarCampos)
    function _leerCabecera(b) {
        const TipoVenta = v.bit(b.TipoVenta); // swTipoVenta: CONTADO = 1, CREDITO = 0
        const FechaVenta = fechaReq(b.FechaVenta, 'Fecha Venta');
        return {
            AlmacenId: v.entero(b.SucursalId, 'Sucursal', { requeridoPositivo: true }),
            FechaVenta,
            PersonalId: v.entero(b.PersonalId, 'Vendedor', { requeridoPositivo: true }),
            ClienteId: v.entero(b.ClienteId, 'Cliente', { requeridoPositivo: true }),
            TipoVenta,
            // VB siempre envia la fecha del control (aunque sea contado)
            FechaVencCredito: b.FechaVencimientoCredito ? fechaReq(b.FechaVencimientoCredito, 'Venc. Credito') : FechaVenta,
            FechaEntregar: fechaReq(b.FechaEntregar || FechaVenta, 'Fecha a Entregar'),
            glosa: v.texto(b.Glosa, 500),
            Descuento: Math.max(0, v.decimal(b.Descuento)),
            PrecioId: v.entero(b.PrecioId, 'Categoria de Precio'),
        };
    }

    /**
     * Calcula una fila igual que P_PonerTotal + CellValueChanged del grid:
     *   CantidadUnitaria = Cantidad * Conversion
     *   SubTotal = CantidadUnitaria * Precio
     *   % descuento (0..100] -> MontoDescuento = SubTotal * % / 100 ; si no, monto (0..SubTotal] -> % = monto*100/SubTotal
     *   Total = SubTotal - MontoDescuento
     */
    function _calcularFila(f, entrada) {
        const conv = num(f.Conversion) || 1;
        let cantUni = entrada.CantidadUnitaria !== undefined && entrada.CantidadUnitaria !== null && entrada.CantidadUnitaria !== ''
            ? num(entrada.CantidadUnitaria)
            : num(entrada.Cantidad) * conv;
        if (!(cantUni > 0)) throw new ErrorNegocio(`La Cantidad de ${f.Producto} debe ser Mayor a 0`);
        cantUni = Math.round(cantUni * 100000) / 100000;
        const subTotal = cantUni * num(f.Precio);
        let porc = num(entrada.ProcentajeDescuento);
        let monto = num(entrada.MontoDescuento);
        if (porc > 0 && porc <= 100) monto = subTotal * (porc / 100);
        else if (monto > 0 && monto <= subTotal) porc = subTotal ? (monto * 100) / subTotal : 0;
        else { porc = 0; monto = 0; }
        return {
            ...f,
            CantidadUnitaria: cantUni,
            Cantidad: cantUni / conv,
            SubTotal: subTotal,
            ProcentajeDescuento: porc,
            MontoDescuento: monto,
            Total: subTotal - monto,
        };
    }

    /** Fila nueva con los datos del producto (InsertarProductosSinLote) */
    function _filaNueva(prod, c, nuevoId) {
        const conv = num(c(prod, 'Conversion')) || 1;
        return {
            Id: nuevoId,
            VentaId: 0,
            ProductoId: num(c(prod, 'Id')),
            Producto: String(c(prod, 'NombreProducto') ?? ''),
            Conversion: conv,
            Precio: num(c(prod, 'PrecioVenta')) / conv,
            PrecioCosto: num(c(prod, 'PrecioCosto')) / conv,
            stock: num(c(prod, 'stock')),
            Detalle: '',
            Lote: '20200101', // valores por defecto de _prAddDetalleVenta (sin lote)
            FechaVencimiento: new Date(Date.UTC(2020, 0, 1)),
            estado: 0,
            img: null,
        };
    }

    /**
     * Arma el DataTable del detalle como lo envia VB a @VentaDetalleType:
     *  - columnas en el orden del SELECT de ListaVentasDetalles (MAM_Ventas @tipo 4)
     *  - TODAS las filas, incluidas las eliminadas (-1/-2), porque el SP decide por "estado"
     */
    async function _armarDetalle({ ventaId, cab, entradas, ventaDirectaVariable, existentes }) {
        if (!Array.isArray(entradas)) throw new ErrorNegocio('Por Favor Inserte un Detalle'.toUpperCase());
        if (entradas.length > MAX_FILAS_DETALLE) throw new ErrorNegocio(`El detalle no puede tener mas de ${MAX_FILAS_DETALLE} productos`);

        const plantilla = await C.listaDetalles(ventaId ?? -1);
        const columnas = L._columnasDe(plantilla);
        const cE = lector(existentes);

        // Productos (precio / stock vigentes) por categoria de precio usada en cada fila nueva
        const cache = new Map();
        const productosDe = async (precioId) => {
            if (!cache.has(precioId)) {
                const dt = await _productos(cab.AlmacenId, cab.ClienteId, precioId, ventaDirectaVariable, cab.PersonalId);
                cache.set(precioId, { dt, c: lector(dt) });
            }
            return cache.get(precioId);
        };

        const filas = [];
        const usados = new Set();
        let mayorId = existentes.reduce((m, f) => Math.max(m, num(cE(f, 'Id'))), 0); // _GenerarId

        // 1) Filas que ya existian: sin cambios (1), modificadas (2) o eliminadas (-1)
        for (const ex of existentes) {
            const idDet = num(cE(ex, 'Id'));
            const base = {};
            for (const col of columnas) base[col] = cE(ex, col);
            const fila = {
                ...base,
                Id: idDet,
                ProductoId: num(cE(ex, 'ProductoId')),
                Producto: String(cE(ex, 'Producto') ?? ''),
                Conversion: num(cE(ex, 'Conversion')) || 1,
                Precio: num(cE(ex, 'Precio')),
                stock: num(cE(ex, 'stock')),
            };
            const entrada = entradas.find((e) => e && e.Id !== undefined && e.Id !== null && num(e.Id) === idDet && !e.nuevo);
            if (!entrada || entrada.eliminado) {
                filas.push({ ...fila, estado: -1 });
                continue;
            }
            if (usados.has(fila.ProductoId)) throw new ErrorNegocio(`El producto ${fila.Producto} esta repetido en el detalle`.toUpperCase());
            usados.add(fila.ProductoId);
            const calc = _calcularFila(fila, entrada);
            const cambioCant = Math.abs(calc.CantidadUnitaria - num(cE(ex, 'CantidadUnitaria'))) > 1e-6;
            const cambioDesc = Math.abs(calc.MontoDescuento - num(cE(ex, 'MontoDescuento'))) > 1e-6;
            // CellValueChanged: la cantidad nueva no puede superar el stock de la fila
            if (cambioCant && !C.sinStock && calc.CantidadUnitaria > fila.stock) {
                throw new ErrorNegocio(`La Cantidad = ${calc.CantidadUnitaria} de ${fila.Producto} es mayor al Stock del Producto = ${fila.stock}`);
            }
            filas.push({ ...calc, estado: cambioCant || cambioDesc ? 2 : 1 });
        }

        // 2) Filas nuevas (estado 0); las que se agregaron y luego se quitaron van con -2 (como VB)
        for (const e of entradas) {
            if (!e || !e.nuevo) continue;
            const productoId = v.entero(e.ProductoId, 'Producto', { requeridoPositivo: true });
            const precioId = v.entero(e.PrecioId ?? cab.PrecioId, 'Categoria de Precio', { requeridoPositivo: true });
            const { dt, c } = await productosDe(precioId);
            const prod = dt.find((p) => num(c(p, 'Id')) === productoId);
            if (!prod) throw new ErrorNegocio(`El producto ${productoId} no esta disponible para esta sucursal / cliente`);
            mayorId += 1;
            const nueva = _filaNueva(prod, c, mayorId);
            if (e.eliminado) { filas.push({ ...nueva, Cantidad: 0, CantidadUnitaria: 0, SubTotal: 0, ProcentajeDescuento: 0, MontoDescuento: 0, Total: 0, estado: -2 }); continue; }
            if (usados.has(productoId)) throw new ErrorNegocio('El producto ya existe en el detalle'.toUpperCase());
            usados.add(productoId);
            // Proforma (frm_CantidadCajas TipoMovimiento=4): no es venta, no se limita por stock
            if (!C.sinStock && !(nueva.stock > 0)) throw new ErrorNegocio(`No Existe Stock del Producto ${nueva.Producto} Para Proceder con la Venta`.toUpperCase());
            const calc = _calcularFila(nueva, e);
            if (!C.sinStock && calc.CantidadUnitaria > nueva.stock) {
                throw new ErrorNegocio(`La cantidad de ${nueva.Producto} es Superior Al Stock Disponible = ${nueva.stock}`);
            }
            filas.push({ ...calc, estado: 0 });
        }

        const vigentes = filas.filter((f) => f.estado >= 0);
        if (!vigentes.length) throw new ErrorNegocio('Por Favor Inserte un Detalle'.toUpperCase());

        // Solo las columnas del tipo, en el orden del SELECT (por posicion, como ADO.NET)
        const filasTVP = filas.map((f) => {
            const o = {};
            for (const col of columnas) {
                const k = Object.keys(f).find((x) => x.toLowerCase() === col.toLowerCase());
                o[col] = k === undefined ? null : f[k];
            }
            return o;
        });
        const sumaTotal = vigentes.reduce((s, f) => s + f.Total, 0);
        return { detalle: { columnas, filas: filasTVP }, sumaTotal, vigentes };
    }

    function _validarDescuentoGeneral(descuento, suma) {
        // tbMdesc_ValueChanged: el descuento no puede superar el total
        if (descuento > suma + 1e-9) throw new ErrorNegocio('El Monto de Descuento no puede ser mayor al Total de la Venta');
    }

    const MODOS = C.modos;

    router.post('/', asyncHandler(async (req, res) => {
        const modo = MODOS[String(req.body?.modo || '')];
        if (!modo) throw new ErrorNegocio('Tipo de venta no valido');
        const cab = _leerCabecera(req.body);

        if (await _usaLote()) throw new ErrorNegocio('La configuracion del sistema usa LOTES: la venta por lotes aun no esta disponible en la web. Use el escritorio.');
        if (modo.SinConciliacion === 1 && C.exigePermisoSinConciliacion && !(await _usuarioPuedeSinConciliacion(req.sesion.gi_userNumi))) {
            throw new ErrorNegocio('Su usuario no tiene permiso para Ventas Directas Sin Conciliacion', 403);
        }
        // _PMOGrabarRegistro: venta directa (con conciliacion) exige conciliacion abierta del vendedor
        if (modo.VentaDirecta === 1 && modo.SinConciliacion === 0) {
            const dt = await L.L_prVerificarConcicliacionAbierta(cab.PersonalId);
            if (!dt.length || num(Object.values(dt[0])[0]) === 0) {
                throw new ErrorNegocio('No es posible guardar la venta por que El Vendedor No tiene conciliación abierta');
            }
        }
        const ventaDirectaVariable = modo.SinConciliacion === 1 ? 0 : modo.VentaDirecta;
        const { detalle, sumaTotal } = await _armarDetalle({ ventaId: -1, cab, entradas: req.body.detalle, ventaDirectaVariable, existentes: [] });
        _validarDescuentoGeneral(cab.Descuento, sumaTotal);
        const incremento = await _incrementoHoy();

        const r = await C.insertar({
            _numi: '0',
            AlmacenId: cab.AlmacenId,
            FechaTransacccion: aVB(cab.FechaVenta),
            PersonalId: cab.PersonalId,
            ClienteId: cab.ClienteId,
            TipoVenta: cab.TipoVenta,
            FechaVencCredito: aVB(cab.FechaVencCredito),
            estado: 1,
            glosa: cab.glosa,
            TotalCompra: sumaTotal - cab.Descuento + incremento,
            Descuento: cab.Descuento,
            EstadoPedido: modo.EstadoPedido,
            FechaEntregar: aVB(cab.FechaEntregar),
            VentaDirecta: modo.SinConciliacion === 1 ? 0 : modo.VentaDirecta, // IIf(VentaDirectaSinConciliacion = 1, 0, VentaDirecta)
            VentaDirectaSinConciliacion: modo.SinConciliacion,
            IncrementoVenta: incremento,
        }, detalle);
        if (!r.resultado) throw new ErrorNegocio('Error al guardar la Venta'.toUpperCase());
        res.json({ ok: true, id: r._numi, fecha: cab.FechaVenta, mensaje: `Codigo de ${C.nombre} ${r._numi} Grabado con Exito.`.toUpperCase() });
    }));

    router.put('/:id', asyncHandler(async (req, res) => {
        const id = v.idRuta(req.params.id);
        const original = await _buscarVenta(id, fechaReq(req.body?.fechaOriginal, 'Fecha original'));
        if (_conciliacionCerrada(original)) {
            throw new ErrorNegocio(`Esta Venta no puede ser Modificada por que pertenece a una conciliacion cerrada = ${original.conciliacionId}`);
        }
        if (await _usaLote()) throw new ErrorNegocio('La configuracion del sistema usa LOTES: modifique esta venta desde el escritorio.');
        const cab = _leerCabecera(req.body);
        const ventaDirecta = num(original.VentaDirecta);
        const sinConc = num(original.VentaDirectaSinConciliacion);
        const ventaDirectaVariable = sinConc === 1 ? 0 : ventaDirecta;

        const existentes = await C.listaDetalles(id);
        const { detalle, sumaTotal } = await _armarDetalle({ ventaId: id, cab, entradas: req.body.detalle, ventaDirectaVariable, existentes });
        _validarDescuentoGeneral(cab.Descuento, sumaTotal);
        const incrementoRegistrado = num(original.IncrementoVenta); // _IncrementoVentaRegistrado

        const r = await C.modificar({
            _numi: id,
            AlmacenId: cab.AlmacenId,
            FechaTransacccion: aVB(cab.FechaVenta),
            PersonalId: cab.PersonalId,
            ClienteId: cab.ClienteId,
            TipoVenta: cab.TipoVenta,
            FechaVencCredito: aVB(cab.FechaVencCredito),
            Moneda: 1,
            estado: 1,
            glosa: cab.glosa,
            TotalCompra: sumaTotal - cab.Descuento + incrementoRegistrado,
            Descuento: cab.Descuento,
            EstadoPedido: num(original.EstadoPedido), // cbEstadoPedido es de solo lectura
            FechaEntregar: aVB(cab.FechaEntregar),
            VentaDirecta: sinConc === 1 ? 0 : ventaDirecta,
            VentaDirectaSinConciliacion: sinConc,
        }, detalle);
        if (!r.resultado) throw new ErrorNegocio('Error al guardar La Venta'.toUpperCase());
        res.json({ ok: true, id, fecha: cab.FechaVenta, mensaje: `Codigo de ${C.nombre} ${id} modificado con Exito.`.toUpperCase() });
    }));

    router.delete('/:id', asyncHandler(async (req, res) => {
        const id = v.idRuta(req.params.id);
        const original = await _buscarVenta(id, fechaReq(req.query.fecha, 'Fecha'));
        if (_conciliacionCerrada(original)) {
            throw new ErrorNegocio(`Esta Venta no puede ser Eliminada por que pertenece a una conciliacion cerrada = ${original.conciliacionId}`);
        }
        const ok = await L.L_prBorrarRegistro(id, C.sp);
        if (!ok) throw new ErrorNegocio('No se pudo eliminar la Venta.');
        res.json({ ok: true, mensaje: `Codigo de ${C.nombre} ${id} eliminado con Exito.`.toUpperCase() });
    }));

    // =====================================================================
    //  NOTA DE VENTA (P_GenerarReporte): cabecera + detalle; el navegador la imprime
    //  Total = Suma(Total) - Descuento + Incremento registrado (igual que el recibo)
    // =====================================================================
    router.get('/:id/recibo', asyncHandler(async (req, res) => {
        const id = v.idRuta(req.params.id);
        const venta = await _buscarVenta(id, fechaReq(req.query.fecha, 'Fecha'));
        const [detalle, empresas, imagen, rec] = await Promise.all([
            C.listaDetalles(id),
            L.L_prListarGeneral('MAM_Empresas'),
            L.ObtenerImagenEmpresa().catch(() => []),
            (C.recibo || L.ListarVentaRecibo)(id).catch(() => []), // MAM_Ventas / MAM_Proforma @tipo 6
        ]);
        // Datos del recibo del escritorio (MAM_Ventas @tipo 6): cliente (NIT, telefono, direccion), hora y codigo de producto
        const r0 = rec[0] || {};
        const de = (f, ...n) => { for (const x of n) { const k = Object.keys(f).find((y) => y.toLowerCase() === x.toLowerCase()); if (k !== undefined && f[k] !== null && f[k] !== '') return Array.isArray(f[k]) ? f[k][0] : f[k]; } return null; };
        const hora = (x) => (x instanceof Date ? `${String(x.getUTCHours()).padStart(2, '0')}:${String(x.getUTCMinutes()).padStart(2, '0')}` : (String(x ?? '').match(/\d{1,2}:\d{2}/) || [''])[0]);
        const cliente = {
            Nit: de(r0, 'Nit', 'NitCi', 'Ci', 'NroDocumento', 'NitCliente'),
            Telefono: de(r0, 'Telefono', 'TelefonoCliente', 'Telefono01', 'Celular'),
            Direccion: de(r0, 'Direccion', 'DireccionCliente'),
            Hora: hora(de(r0, 'HoraRegistro', 'Hora', 'HoraVenta', 'FechaRegistro')),
        };
        const det = v.proyectar(detalle, ['ProductoId', 'Producto', 'Cantidad', 'CantidadUnitaria', 'Precio', 'SubTotal', 'MontoDescuento', 'Total']);
        const suma = det.reduce((s, f) => s + num(f.Total), 0);
        const emp = v.proyectar(empresas, ['Nombre', 'Direccion', 'Telefono', 'Nit', 'Ciudad'])[0] || null;
        res.json({
            ok: true,
            data: {
                venta,
                detalle: det,
                subtotal: suma,
                total: suma - num(venta.Descuento) + num(venta.IncrementoVenta),
                empresa: emp,
                cliente,
                codigos: Object.fromEntries(rec.map((f) => [String(de(f, 'Producto', 'NombreProducto', 'Descripcion') ?? ''), de(f, 'CodigoExterno', 'Codigo', 'ProductoId', 'IdProducto')]).filter(([k, x]) => k && x !== null)),
                imagenEmpresa: imagen.length ? String(Object.values(imagen[0])[0] ?? '') : '',
            },
        });
    }));


    router._interno = { _armarDetalle, _leerCabecera, _incrementoHoy, _buscarVenta, MODOS };
    return router;
}

const VENTAS = {
    recibo: (id) => L.ListarVentaRecibo(id),
    programa: 'btVentVenta',
    nombre: 'Venta',
    sp: 'MAM_Ventas',
    listaDetalles: (id) => L.ListaVentasDetalles(id),
    insertar: (v, d) => L.VentaInsertar(v, d),
    modificar: (v, d) => L.VentaModificar(v, d),
    conIncremento: true,
    exigePermisoSinConciliacion: true,
    modos: {
        pedido: { VentaDirecta: 0, SinConciliacion: 0, EstadoPedido: 1 },
        directa: { VentaDirecta: 1, SinConciliacion: 0, EstadoPedido: 4 },
        sinconciliacion: { VentaDirecta: 1, SinConciliacion: 1, EstadoPedido: 4 },
    },
};
const routerVentas = crearRouter(VENTAS);
module.exports = routerVentas;
module.exports.crearRouter = crearRouter;
// Para test/probar_ventas_bd.js (misma logica que usa la web al grabar)
module.exports._interno = routerVentas._interno;
