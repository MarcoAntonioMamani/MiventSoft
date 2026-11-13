# Santit Web — Configuración y Ventas (Gestión Pedidos)

Santit es la versión web de MiventSoft / TeVendo.

Migración del sistema de distribución MiventSoft (VB.NET) a la web, **usando los mismos SP y la misma estructura de capas**.

| VB.NET (escritorio) | Web |
|---|---|
| `Datos/Configuracion.vb` + `dbConnect.TXT` | `server/src/datos/Configuracion.js` + `server/.env` |
| `Datos/MetodoDatos.vb`, `AccesoDatos.vb`, `DParametro.vb` | `server/src/datos/*.js` (mismos nombres: `D_ProcedimientoConParam`, `new DParametro(...)`) |
| `Negocio/AccesoLogica.vb` | `server/src/negocio/AccesoLogica.js` (mismas funciones `L_pr...`, mismos `@tipo`) |
| `Modelo/ModeloF1.vb` (`_PMNuevo`, `_PMOGrabarRegistro`...) | `client/src/componentes/ModeloF1.jsx` (`useModeloF1` con los mismos `_PMO*`) |
| `Celda`, `JGrM_Buscador`, `Efecto`, `ToastNotification` | `Celda.js`, `GridBuscador.jsx`, `Efecto.jsx`, `Toast.jsx` |
| `Tec_Login`, `Tec_Principal` (`_PCargarPrivilegios`) | `Tec_Login.jsx`, `Tec_Principal.jsx` + `middleware/permisos.js` |
| `TecCategorias`, `Tec_Empresas`, `Tec_Users`, `Tec_Roles` | `paginas/configuracion/*.jsx` + `rutas/configuracion/*.routes.js` |
| `Tec_Clientes`, `Tec_Zonas`, `Tec_Productos`, `Tec_Precios` | (paso 2) mismas carpetas |

SP utilizados (sin cambios): `MAM_Categorias`, `MAM_Empresas`, `MAM_Usuarios`, `MAM_Roles` (TVP `RolDetalleType`), `MAM_Clasificadores`, `MAM_Clientes`, `sp_Mam_Zonas` (TVP `@ZonasPuntos`), `MAM_Productos` (TVP `@TCL0064`), `sp_Mam_Precios` (TVP `@TY007`).
Tablas leídas directo (igual que en VB, pero con parámetros): `Usuarios` (login), `RolesDetalles` + `Programas` (privilegios).

## Rama KailyLaPaz (punto de venta)
La web se trajo de la rama de distribución (Distral) y se está ajustando programa por programa al escritorio de Kaily (`Negocio/AccesoLogica.vb` de esta rama). Los SP no se modifican.
- **Menú**: los paneles y botones de `Tec_Principal.Designer.vb` de Kaily (`server/src/negocio/menuEscritorio.js`).
- **Login**: lee las mismas columnas que `L_Validar_Usuario` de Kaily (permisos de venta, tipo de cambio, sucursal).
- **Usuarios**: sucursal (con "Todos" = -1) y los 5 permisos de venta (`@ModificarPrecio`, `@ModificarDescuento`, `@VentasTodasVentas`, `@VerTodasVentasCierre`, `@ModificarRegistroAntiguos`). Ya no se envía `@PedidoSinConciliacion`.
- **Clientes**: sin `@Referencia` ni `@TipoNegocio`.
- **Productos**: precios por categoría (`ListPreciosDetalles`, `MAM_Productos` @tipo 10) enviados en `@PrecioT`.
- **Ventas** (`ventasPuntoVenta.routes.js` + `Tec_VentasPuntoVenta.jsx`): lista por sucursal del usuario, productos y kits (`@tipo 5` con `@CategoriaVentaId`, kits `@tipo 10`), cobro en contado en `@VentaPagos` (`@tipo 8`), `@Facturado`, Caja Chica abierta obligatoria y los permisos de precio, descuento y registros anteriores.
- **Proformas** (`btnProforma`, `proformas.routes.js` + `Proforma.jsx`): la misma pantalla de Ventas de Kaily con `MAM_Proforma`. Sin cobro, sin `@Facturado` y sin campos de distribución; TipoVenta 1 y vencimiento = hoy, como `ProformaInsertar`. Cotiza sin stock y no pide Caja Chica.
- `test/probar_ventas_bd.js` ya prueba la venta de Kaily (con `@VentaPagos`) contra la BD, dentro de una transacción que se revierte.
- **Administrar Caja** (módulo Ventas):
  - Registrar gastos o ingresos (`btnGastos`): el mismo formulario y SP que ingresos/egresos (`MAM_CajaIngresoEgreso`), en `/api/caja/ingresos-egresos`.
  - Cierre de caja del cajero (`btnCierreCajero`, `rutas/caja/cierreCajero.routes.js` + `paginas/caja/Tec_CierreCajero.jsx`): `MAM_CierreCajero` 1–11. Nuevo = abrir caja; Modificar = cerrar con ventas, cobranzas e ingresos/egresos pendientes y el conteo de billetes (`@CorteBilletes`, `@ModuloId`). Los totales se recalculan en el servidor.
  - Caja general (`btnCajaGeneral`, `MAM_CajaIngresoEgreso` 6 + 7 con saldo inicial y acumulado) y Reporte caja detallado (`btnReporteCajaDetallado`, @tipo 13).
  - Ingresos/egresos mensuales y detallado (`btnIngresoEgresosMensuales`, `btnIngresoEgresoDetallado`): los reportes de distribución, que usan los mismos SP. `requierePrograma` ahora acepta varios Id.
- **Dosificación** (`btConfDosificacion`, `MAM_Dosificacion` 1/2/3/-1) y **Tipo de cambio** (botón de la cabecera, `ActualizarTipoCambio` → `MAM_Ventas` @tipo 7). La Llave no se muestra en la lista ni va a los logs.
- **Productos**: código externo único (`MAM_Productos` @tipo 11) y reporte "Listado de precios" con y sin imágenes (@tipo 8 + categorías elegidas).
- **Ventas / Proformas**: alta rápida de cliente (`FormularioCliente`, `MAM_Clientes` @tipo 8 con `@Telefono`).
- **Precios**: Cod. Sistema visible; al agregar una categoría de precio se graban los precios pendientes (`btnGrabar.PerformClick`).
- **Inventario**: traspasos con `@DepositoIdDestino` / `@IdMovimientoDestino`, borrar con `L_prBorrarRegistroMovimiento` (@ConceptoId), comprobante (@tipo 17), FormularioStock sin proveedor y categoría de precio = `sp_Mam_Precios` @tipo 8.
- **Compras**: sin nro. de nota de compra/recepción, lista por fechas (`L_prListarGeneralComprasFiltro`). **CxC**: sin preventista, cobro con Caja Chica abierta.
- **`@tipo` que cambiaron respecto a Distral** (mismos parámetros, el contrato no los detecta): `InsertarTipoMovimiento` 21→9, `L_fnListarCategoriaPrecio` 9→8; el @tipo 7 de `MAM_Ventas` ahora es `InsertarTipoCambio` (el panel ya no lo llama).
- **Despacho De Productos de Ventas** (`btnDespachoVenta`, `FrmDespacho`): sí está, en el grupo Ventas del menú (`/ventas/despacho`, `MAM_Ventas` @tipo 11).
- **Panel de control: SP propio `MAM_PanelControl`** (`server/sql/03_MAM_PanelControl.sql`, solo lectura, el escritorio no lo usa). **Instalarlo en cada BD** ejecutando el script completo; solo crea o reemplaza ese SP (`npm run verificar` avisa si falta). Sin él, el panel funciona, pero sin ventas por hora ni análisis, y avisa qué no pudo leer.
  - `@tipo 1`: hora de cada venta. `MAM_Ventas` @tipo 3 no trae `HoraRegistro`; se une por Id. La hora se graba como `'9:5'`, sin ceros.
  - `@tipo 2`: compras por cliente y día (12 meses, sucursal del usuario). Sirve para **Clientes que dejaron de comprar** (compraron en el año y no volvieron en 60 días) y **Clientes rentables** (más monto o más días con compra, en 30, 90 o 365 días, con gráfico frecuencia vs. monto). No cuenta el cliente de mostrador (Id 1).
  - `@tipo 3`: **productos con stock sin ventas en 60 días**, de todas las sucursales, con su última venta, valorados a precio de venta (categoría 1, almacén 1, como `MAM_Movimientos` @tipo 18).
  - Ventas: el "Total en dólares" del formulario usa el tipo de cambio con que se registró la venta (`MAM_Ventas` @tipo 3; 0 si es a crédito). Solo una venta nueva usa el último tipo de cambio. El cobro al modificar sigue con el vigente, como VB.
- **Fuera de esta rama** (punto de venta, no distribución): módulo Distribución (despachos, conciliaciones, cierre de chofer), asignaciones de pedidos y de zonas, incremento de ventas, histórico/compras por producto y los reportes de Distral. Se quitaron de la web, del menú y de las pruebas.
- **Contrato**: `node test/generar_contrato.js` regenera `test/contrato_vb.json` desde el `Negocio/AccesoLogica.vb` de la rama (`--comparar` muestra las diferencias). Hacerlo al traer la web a otra rama de cliente.
- Pendiente: `test/probar_ventas_bd.js` y algunas pruebas de pantalla (`client/test/*.e2e.cjs`) siguen escritas para Distral; el comprobante de movimientos (@tipo 17) falta validarlo contra la BD real.

## Arranque rápido contra tu BD (como el sistema normal)
1. Instala **Node.js LTS** desde https://nodejs.org (una sola vez).
2. Doble clic en **`iniciar.bat`**. Lo que hace:
   - Crea `server\.env` leyendo `..\TeVendo\bin\Debug\dbConnect.TXT` (igual que `Tec_Login`). Las credenciales no salen de tu PC.
   - Instala dependencias (solo la primera vez) y compila la web.
   - **Verifica tu BD en solo lectura**: conexión, que existan los SP y las tablas, y la estructura de los tipos tabla.
   - Levanta el sistema y abre `http://localhost:3001`. Entras con tu usuario del escritorio.
3. Desde otra PC o un celular de la misma red: usa la dirección `http://IP-de-tu-PC:3001` que muestra la consola. Si no carga, permite el puerto 3001 en el Firewall de Windows.

Si `dbConnect.TXT` está en otra ruta: `powershell -ExecutionPolicy Bypass -File configurar.ps1 -Archivo "C:\ruta\dbConnect.TXT" -Forzar`.
Para volver a verificar la BD: `cd server && npm run verificar`.

> El driver de Node se conecta por **TCP/IP**. Si la verificación no conecta: SQL Server Configuration Manager > Protocolos > **TCP/IP = Habilitado** (y reinicia el servicio). Con instancia con nombre (`EQUIPO\SQLEXPRESS`) también debe estar iniciado **SQL Server Browser**.

## Requisitos
- Node.js 18 o superior
- Acceso al SQL Server de MiventSoft (el mismo de `dbConnect.TXT`)

## Instalación
```bash
cd server
npm install
copy .env.example .env      # completar DB_*, CARPETA_RAIZ y JWT_SECRET
cd ../client
npm install
```

## Desarrollo
```bash
# terminal 1
cd server && npm run dev        # API en http://localhost:3001
# terminal 2
cd client && npm run dev        # Web en http://localhost:5173
```
**Sin base de datos** (solo para ver las pantallas): `cd server && npm run mock` → usuario `ADMIN` / `123`.

## Pruebas automáticas
```bash
cd server && npm test
```
Pruebas de pantalla (con el servidor en modo simulador `node src/app.js --mock`):
```bash
cd client
node test/combos.e2e.cjs      # combos en 4 anchos de pantalla
node test/dashboard.e2e.cjs   # panel: filtros, detalle al clic, sin cortes, de 320 px a 1440 px
node test/destacados.e2e.cjs  # destacados del mes
node test/alertas.e2e.cjs     # alertas sin texto encimado
node test/ventas.e2e.cjs      # modulo Ventas completo + responsivo
node test/cxc.e2e.cjs         # cuentas por cobrar, incremento y permisos por accion
node test/reportes.e2e.cjs    # los 7 reportes de Gestion Pedidos
node test/proforma.e2e.cjs    # proformas
node test/inventario.e2e.cjs  # ingreso y salida de productos, kardex, saldos y monitoreo
node test/mapa.e2e.cjs        # mapa de clientes + iconos a color en los menus
node test/nota_venta.e2e.cjs  # nota de venta compacta: 1 pagina, empieza arriba, ~media hoja
node test/menu_fila.e2e.cjs   # menu Ver/Editar/Eliminar: clic derecho (PC), doble toque y mantener presionado (celular)
node test/cxc_reportes.e2e.cjs # reportes impresos de creditos pendientes (por preventista) y cobrados (por fecha de venta)
node test/ventas_realizadas.e2e.cjs # reporte de ventas realizadas: datos Sucursal > Personal y grafico
node test/ventas_costos.e2e.cjs # ventas vs costos: datos por Personal y grafico de utilidades
node test/productos_vendidos.e2e.cjs # productos mas vendidos: datos y graficos (primeros N)
node test/menu_acordeon.e2e.cjs # menu lateral en acordeon (un solo modulo desplegado)
node test/rendimiento.e2e.cjs # rendimiento del personal: lineas por mes, sin textos encimados
node test/cobranzas.e2e.cjs   # reporte de cobranzas agrupado por personal (un solo monto)
node test/previa_pestanas.e2e.cjs # vista previa al imprimir y programas abiertos en pestañas
```
Inserta, modifica y elimina en las 8 pantallas usando un simulador de los SP. El simulador **compara cada llamada con `test/contrato_vb.json`**, extraído de `Negocio/AccesoLogica.vb`: si la web envía a un SP + `@tipo` un parámetro distinto al escritorio, la prueba falla. No reemplaza la prueba contra tu SQL Server real (ver *Primera prueba con tu BD*).

## Primera prueba con tu BD (en este orden)
1. Grabar un **Rol** (TVP `RolDetalleType`).
2. Crear una **Zona** dibujando 3+ puntos y luego modificarla sin redibujar (TVP `@ZonasPuntos`).
3. Crear un **Producto** con 1 imagen y luego quitarla (TVP `@TCL0064`, carpeta `Imagenes Productos\ProductosTodos`).
4. Cambiar 1 **Precio** y verificar en el escritorio (TVP `@TY007`).
5. Modificar un **Cliente** y verificar que `FechaIngreso` no cambió.

## Producción (un solo proceso)
```bash
cd client && npm run build
cd ../server && set NODE_ENV=production && npm start   # sirve API + web en el puerto PORT
```
Detrás de IIS (ARR/URL Rewrite) o como servicio de Windows (NSSM / pm2). Usar HTTPS: la cookie de sesión es `Secure` en producción.

## Cosas a revisar
1. **Tipo tabla `RolDetalleType`**: la estructura se lee automáticamente desde SQL Server (`sys.table_types`) y las columnas se envían **por posición**, igual que el `DataTable` de VB (`Id, RolId, ProgramaId, DescripcionPrograma, Modulo, Ver, Insertar, Modificar, Eliminar, estado`).
2. **Usuario de BD**: crear un login SQL exclusivo para la web con `EXECUTE` sobre los SP y `SELECT` en `Usuarios`, `RolesDetalles` y `Programas`; no usar `sa`.
3. **Imágenes**: se guardan en `CARPETA_RAIZ\Imagenes\Imagenes Categoria|Empresa` con el mismo nombre `\Imagen_HHmmss_ddMMyyyy.jpg`, compartidas con el escritorio.

## Diferencias respecto al escritorio
- Login y privilegios con **consultas parametrizadas** (el `D_Datos_Tabla` concatenado era vulnerable a SQL Injection).
- La **contraseña no viaja al navegador**; al modificar un usuario, el campo vacío conserva la actual.
- `L_Usuario` se toma del usuario autenticado en **cada petición**, no de una variable compartida.
- `Tec_Users`: en VB `VentaDirecta` se enviaba con el valor de `swEstado`; aquí se envía el de `swVentasDirectas`.
- `TecCategorias`: en VB `cbEmpresa.Value = .GetValue("Empresa")` asignaba el nombre; aquí se usa `EmpresaId`.
- Errores de SQL: los `RAISERROR` de los SP (>= 50000) se muestran al usuario; los demás se registran en el servidor con un código de traza.

### Paso 2
- **Datos mínimos**: cada pantalla recibe solo sus columnas (`proyectar`); Precios envía solo las celdas cambiadas; Zonas solo los puntos si se redibujó; Productos solo las imágenes nuevas y los nombres a quitar. Las tablas completas para los TVP se arman en el servidor con los mismos estados que VB (0/1/-1/-2 y 0/1/2/3).
- `Tec_Clientes`: el escritorio sobrescribía `FechaIngreso` con la fecha de hoy al modificar; aquí se conserva la original (con corrección de zona horaria UTC-4).
- `Tec_Clientes`: se omitió `L_prCargarImagenesRecepcion(Id cliente)`, que consultaba imágenes de **productos** con el Id del cliente y no se usaba.
- Responsive: en celular las grillas se muestran como tarjetas con un buscador único, Precios como una tarjeta por producto, y Clientes tiene "Mi ubicación" (GPS).

### v0.2 (diseño y navegación)
- Diseño estilo Material, con tipografía Roboto incluida (funciona sin internet), elevaciones, efecto ripple, animaciones de entrada, filas de carga y avisos animados. Se respeta `prefers-reduced-motion`.
- Rutas por módulo y programa: `/configuracion`, `/configuracion/clientes`, `/configuracion/clientes/nuevo`, `/configuracion/clientes/15`, `/configuracion/clientes/15/editar`. Hay migas de pan clicables y el botón atrás del navegador funciona.
- Las listas se paginan de a 15 registros. Después de Grabar, Cancelar o Eliminar se vuelve a la lista, en la página del registro y con la fila resaltada.
- En celular, botón flotante "+" para Nuevo y ventanas que suben desde abajo.

### v0.3 (Panel de control)
- Pantalla principal con indicadores: pedidos y monto del día (con variación contra ayer y contra el promedio), ticket promedio, vendedor con más pedidos, ventas por hora, rendimiento de 30 días, ranking de vendedores, pedidos por estado y forma de pago, cuentas por cobrar y por pagar con antigüedad de mora, alertas y productos más vendidos.
- Filtro por fecha y por vendedor. Se actualiza solo cada 2 minutos cuando se mira el día de hoy.
- **Destacados del mes** (del 1ro del mes a la fecha elegida): cliente que más compró, producto más vendido y vendedor del mes, cada uno con su top 5 y detalle al clic. Con filtro de vendedor, el cliente del mes es de la cartera de ese vendedor; el producto (`MAM_ReporteVentas` @tipo 6) y el vendedor del mes siempre son de toda la empresa.
- **Detalle al hacer clic** en indicadores, barras del ranking, estados, hora del gráfico, tramos de mora, mayores saldos y alertas. Tiene búsqueda y paginación de 15; en celular se muestra como tarjetas.
- SP usados (los mismos del escritorio, solo lectura): `MAM_Ventas` @tipo 3 y 7, `MAM_CreditosVentas` @tipo 1, `MAM_CreditosCompras` @tipo 1, `MAM_ReporteVentas` @tipo 6, 10 y 11.
- Opcional: `DASHBOARD_ROLES=1,3` en `server/.env` limita el panel (incluye montos de cartera) a esos roles.
- **Verificar con tu BD:** los nombres de columna se tomaron de las pantallas VB (FechaVenta, HoraRegistro, TotalVenta, PersonalId, Personal, NombreEstado, TVenta, Restante, DiasMora, Nombrecliente, NombreProveedor, NombrePersonal). Si un bloque sale vacío o con aviso "No se pudo leer", revisar la consola del servidor.

### v0.4 (Paso 3: módulo Ventas)
- **Personal** (`Tec_Personal`, programa `btnPersonal`), **Proveedores** (`Frm_Proveedor`, `btnProveedores`) y **Ventas** (`Tec_Ventas` + `Tec_VentasDetalle`, `btVentVenta`) con los mismos SP, `@tipo` y parámetros que el escritorio (validado contra `test/contrato_vb.json`).
- Ventas, igual que el escritorio:
  - Tres formas de crear: Nuevo Pedido (estado 1), Venta Directa (estado 4, exige conciliación abierta del vendedor) y Venta Directa Sin Conciliación (solo usuarios con `pedido = 1`).
  - Productos de `MAM_Ventas` @tipo 5 o @tipo 20, con búsqueda por varias palabras. Cantidad en cajas ⇄ unidades y control de stock.
  - Descuento por línea (% o monto), descuento general e incremento diario (solo en ventas nuevas).
  - Detalle enviado a `@VentaDetalleType` por posición, con estados 0/1/2/-1/-2.
  - No se modifica ni elimina una venta de una conciliación cerrada.
  - Nota de venta imprimible con el monto en letras.
- Mejoras de seguridad:
  - Los precios y totales los calcula el servidor con los mismos SP; lo que mande el navegador se ignora.
  - El costo nunca sale al navegador.
  - El stock se revalida al grabar.
  - La descripción del proveedor ya no se borra al modificar.
- Pendiente: venta **por lotes** (si la configuración usa lotes, la web avisa y se usa el escritorio).

### v0.5 (Gestión Pedidos completo)
Todo el menú *Gestión Pedidos* del escritorio, cada opción con su propio programa (se habilita por rol):

| Escritorio | Programa | Web |
|---|---|---|
| Ventas | `btVentVenta` | `/ventas/ventas` |
| Asignación de pedidos (`Tec_AdministrarAsignacionesPedidos`) | `btnAsignacionPedidos` | `/ventas/asignacion-pedidos` |
| Cuentas por cobrar a crédito | `btnCredPago` | `/ventas/cuentas-cobrar` |
| Crear proformas | `btVentProforma` | `/ventas/proformas` |
| Reporte de ventas / Ventas vs costos / Productos más vendidos / Rendimiento personal / Ventas por productos / Reporte cobranza / Ventas por categoría | `btVentReporteRealizada`, `btVentReporteVentaVsCosto`, `btVentProdMasVendidos`, `btVentRendimiento`, `btnVentaProductos`, `btnReporteCobranza`, `btnReporteProductoCategoria` | `/ventas/reporte-ventas`, `/ventas/ventas-vs-costos`, `/ventas/productos-mas-vendidos`, `/ventas/rendimiento-personal`, `/ventas/ventas-por-productos`, `/ventas/reporte-cobranza`, `/ventas/ventas-por-categoria` |
| Incremento de venta (barra superior) | (cualquier usuario, igual que el escritorio) | chip "Incremento" arriba |

`btVentGrafica` y `btInvUtilidad` no tienen código en el escritorio; no se migraron.

- **Permisos por acción** (`RolesDetalles`): si el rol solo tiene **Ver** marcado en un programa, tiene acceso completo (igual que el escritorio). Si además marcó alguna de Insertar / Modificar / Eliminar, solo se permiten las marcadas (se ocultan los botones y el servidor también lo valida).
  - Asignación de pedidos: Asignar = Insertar; Quitar / Entregar / Revertir = Modificar; Anular = Eliminar.
- **Asignación de pedidos**: mismos SP (`MAM_Ventas` 7/8/9/10/11/12/13/14/15/16/17/18/22, `MAM_Clientes` 12). Igual que VB se envía la grilla completa con la columna `Asignar`; el servidor vuelve a leer la lista y marca solo los pedidos elegidos (si alguno ya cambió de estado, avisa y no graba).
- **Proformas**: el escritorio solo crea con `btnVentaSinConciliacion` (VD 1, SC 1, estado 4) y lo permite si el usuario tiene `pedido = 1`. La web graba lo mismo, pero lo habilita por el permiso *Insertar* del rol en `btVentProforma`.
- **Reportes**: las columnas salen tal cual las devuelve cada SP (sin imágenes). Imprimir y exportar a Excel (CSV).
- Arriba se muestra el servidor y la base a la que apunta la web.

### v0.6 (Distribución: Despachos y Conciliaciones)
- **Despachos** (`Tec_Despachos` + `Tec_DespachoDetalle`, programa `btnDespacho`, `/distribucion/despachos`): mismos SP (`MAM_DespachoProductos` 1/2/4/5/6/7/8/-1). Productos con stock (cajas ⇄ unidades), *Cargar productos de ventas* = despacho automático (envía `@ReferenciaVenta`), nota de despacho imprimible.
  - Igual que el escritorio: no se edita ni elimina si la conciliación está cerrada; el automático solo se elimina. Movimiento 5, sucursal 1.
  - El servidor arma el detalle con estados 0/1/2/-1/-2 y valida stock y cantidades.
  - **A revisar:** al grabar un despacho nuevo la web envía `@ConciliacionId = 0`; el escritorio envía el de la última conciliación que se estaba viendo (el SP debería ignorarlo y usar/crear la conciliación abierta del personal). Confirmar con el SP.
- **Conciliaciones** (`Tec_Conciliacion`, programa `btnConciliaciones`, `/distribucion/conciliaciones`): `MAM_Conciliacion` 2/4/5/6/7. Sin Nuevo ni Eliminar (las crea el despacho). Matriz con una columna por salida, devolución editable, abrir / cerrar, reporte imprimible. Bloqueada si tiene cierre de caja.
  - Mejora: la diferencia siempre es *devolución real − devolución calculada* (en el escritorio no se recalculaba al escribir 0).

### v0.7 (Distribución completa + menú de mosaico)
- **Menú de cada módulo** en mosaico de colores (como el escritorio, mejorado): tarjetas con degradado, ícono propio por programa, animación al pasar el mouse; 2 columnas en celular. Los programas aún no migrados se ven en gris como "Próximamente".
- **Cierre de caja** (`btnCierreCaja`, `/distribucion/cierre-caja`): `MAM_CierreCaja` 1/2/4/5/6/7/8/9/-1. Se elige una conciliación cerrada sin cierre, se cargan sus ventas y las cobranzas del personal, se agregan gastos (libreria 14) y el efectivo recibido. Calculado = contado + cobranzas − gastos; diferencia = recibido − calculado (todo recalculado en el servidor). Al modificar se conservan ventas y cobranzas del cierre, como en VB.
- **Registrar gastos / ingresos generales** (`btnCierreRegistroIngresoEgresoGenerales`, `/distribucion/ingresos-egresos`): `MAM_CajaIngresoEgreso`. El motivo define ingreso/egreso; motivo 9 = traspaso entre cajas (2 movimientos enlazados); caja chica exige cierre de cajero abierto en la fecha; bloqueado si pertenece a un cierre. *Pendiente:* el botón "nuevo motivo" del escritorio (Efecto 21).
- **Asignación de zonas** (`btnAsignacionZona`): `sp_Mam_Zonas` 5/6, envía la grilla completa con `asignada`.
- **Reportes**: gastos del chofer, ingresos/egresos anual (con gráfico) y detallado, histórico de registro de clientes (campos cambiados), ventas de productos por cliente, auditoría de ventas (con detalle) e historial del incremento.

### v0.8 (Ingreso y salida de productos)
- **Ingreso y salida de productos** (`btInvMovimiento`, `/inventario/movimientos`): `MAM_Movimientos` 1/2/4/5/7/-1, depósitos `MAM_Almacenes` 4. Tipo 4 = ingreso (sin límite), los demás no superan el stock; stock y stock final por producto; al cambiar tipo o depósito en uno nuevo se limpia el detalle (como VB). Con LOTES la web avisa y no graba (igual que Ventas).
- **Historial de productos** (kardex, `btInvKardex`): saldo inicial + movimientos con saldo acumulado (mismo cálculo de VB: conceptos 2/4 ingresos, 1/3 salidas) y tarjetas de resumen.
- **Reporte de saldos** (`btInvSaldo`, @tipo 9/10/11/12), **Saldos valorados** (`btInvSaldosValorados`, `MAM_ReporteVentas` 9), **Reporte general historial** (`btInvKardexReporte`, @tipo 16 + 14 + 15 por producto) y **Monitoreo** (`btnInvMonitoreo`: sin stock, stock mínimo y vencidos).

### v0.9 (Mapa de clientes + iconos del sistema)
- **Ubicación de clientes** (`btZonaMapaCliente`, `/mapa-clientes/ubicacion`): `MAM_Clientes` 6 (todos) / 7 (por zona), con lista buscable, contadores (en el mapa / sin ubicación) y ficha del cliente. **Gestión de zonas** (`btZona`) ya estaba migrada y aparece en el módulo donde la tenga el rol.
- **Iconos a color del escritorio**: se extrajeron de `Tec_Principal.resx` y `Resources` a `client/public/iconos/<IdPrograma>.png` y se usan en el mosaico, el menú lateral y los accesos del inicio. Los programas sin icono propio usan el ícono de línea.

### v1.0 (Almacenes, Compras y Planilla: todos los programas migrados)
- **Almacenes** (`btInvAmacen`, `/inventario/almacenes`): `MAM_Almacenes` 1 / 2 / -1 (depósitos @tipo 4). Igual que VB: no se elimina la primera sucursal.
- **Compras** (`btnCompras`, `/compras/compras`): `MAM_Compras` 1 / 2 / -1, detalle @tipo 4, productos @tipo 5, proveedores @tipo 6 y comprobante @tipo 7.
  - Contado / Crédito con fecha de vencimiento. SubTotal = P.Costo × Cajas; Total = suma − descuento, recalculado en el servidor.
  - No se elimina una compra con pagos de crédito. Con LOTES activo, igual que Ventas, se pide registrar desde el escritorio.
- **Cuentas por pagar** (`btnCompraCredito`): `MAM_CreditosCompras` 1–8, con las mismas pestañas que Cuentas por cobrar.
- **Histórico de compras** (`btnHistorico`): `MAM_Compras` 9 / 10 / 11, con una columna de cantidad y otra de P.Costo por producto (ArmarHistorico).
- **Compras por producto** (`btnComprasPorProducto`): `MAM_ReporteVentas` 16, para todos los productos o uno.
- **Conceptos fijos** (`btnConceptoFijos`): `MAM_ConceptosFijos` 1 / 2 / -1 / 4. Fijo/Variable; si es Fijo, el porcentaje debe ser mayor que 0.
- **Contratos** (`btnContratos`): `MAM_Contratos` 1 / 2 / -1 / 4 / 5.
  - Tipo de contrato (lib. 10) y Cargo (lib. 11) se pueden crear desde la pantalla.
  - Un contrato indefinido nuevo se graba con fin = hoy + 50 años.
  - No se registra si el personal ya tiene un contrato vigente.
- **Registrar personal a planilla** (`btnRegistrarPlanilla`): `MAM_Planilla` 5 → 1 por contrato, con sus conceptos fijos (Monto = Sueldo × %).
- **Planilla** (`btnPlanilla`): `MAM_Planilla` 3 / 9 / 8, con conceptos fijos (ver), conceptos variables (agregar/quitar con @tipo 7), boleta y reporte.
  - Año (lib. 13) y Mes (lib. 12) se envían como el **código de la librería**, igual que el escritorio.
- `verificar.js` ahora también compara `@detalle` (Compras), `@DetalleContratoConcepto` y `@PlanillaConcepto` con sus grillas. Todo es solo lectura.
- El programa abierto queda resaltado en el menú lateral en todos los módulos nuevos (lo comprueba `compras_planilla.e2e.cjs`).

### v1.1 (Nota de venta compacta)
- La nota de venta tiene el tamaño del `Recibo.rpt` del escritorio: letra 11 px, filas y cajas más bajas, logo de 70 px como máximo. Ocupa cerca de media hoja carta.
- **Imprimir** manda solo la hoja a un iframe aparte (márgenes de 8 mm), así ya no quedan espacios en blanco arriba por la página que está detrás. Los estilos de la hoja están en `CSS_NOTA` (`Tec_Ventas.jsx`) y se usan igual en la vista previa y al imprimir.

- La hoja se imprime con `@page { margin: 0 }`: así el navegador no agrega su encabezado y pie (fecha, título, URL). El margen de 8 mm lo pone el `body`.

### v1.2 (Menú de la fila, como el escritorio)
- En todas las listas (`GridBuscador`, usado por `ModeloF1`), **clic derecho** sobre una fila abre el menú **Ver / Editar / Eliminar** en el lugar del clic, igual que el escritorio. Así no hace falta desplazarse a la derecha hasta los botones.
- En **celular**: **doble toque** o **mantener presionado** sobre la tarjeta abre el mismo menú como hoja desde abajo, con el registro arriba. Un toque simple o deslizar no lo abren.
- Teclado: tecla Menú o Shift+F10 sobre la fila elegida; flechas para moverse y Escape para cerrar.
- Las opciones ejecutan lo mismo que los botones de la fila (`m.abrir`, `_PMModificar`, `_PMEliminar`, con las mismas reglas) y respetan los permisos del rol: si no puede modificar o eliminar, la opción no aparece.

### v1.3 (Reportes de créditos de Cuentas por cobrar)
- **Estándar de reporte impreso** (`componentes/ReporteImpreso.js` + `global/imprimir.js`):
  - Cabecera con empresa, título y logo (los mismos SP de la nota de venta), y una barra con usuario, fecha de impresión, filtros y resumen.
  - Una sola tabla compacta (10 px). El texto baja de línea en vez de cortarse; los montos y las fechas no.
  - La cabecera y los títulos de columna se repiten en cada hoja.
  - Se imprime en un iframe sin márgenes de página, así no salen el encabezado del navegador ni espacios en blanco.
  - La nota de venta usa la misma impresión.
- **Créditos pendientes** (`MAM_CreditosVentas` @tipo 1): agrupado por preventista como `Reporte_CobrosPendientes.rpt`, con subtotal de Monto / Abonado / Restante, total general y, en la barra, por cobrar, vencido y cantidad de preventistas. La mora de más de 30 días sale resaltada. Se imprime lo que está filtrado en la búsqueda.
- **Créditos cobrados** (`MAM_CreditosVentas` @tipo 2): **sin agrupar**. Por defecto muestra todas las fechas; al desmarcar "Todas las fechas" se filtra por **fecha de venta** (Desde / Hasta) y se agrega la columna Fecha venta.
  - El @tipo 2 no devuelve la fecha de la venta. El servidor la obtiene de `MAM_Ventas` @tipo 3 del mismo rango (la lista de Ventas, solo lectura), uniendo por el número de venta (`venta00047041` → 47041). Si algún día el SP devuelve `FechaVenta`, se usa directamente.
  - **Verificar con tu BD:** que el número dentro de `venta` sea el Id de la venta (en el escritorio se ve así: `venta0004704…`).
- Endpoint nuevo `GET /api/ventas/cuentas-cobrar/cabecera-reporte` (empresa + logo) y `GET /cobrados?desde=&hasta=`.

### v1.4 (Reporte de ventas realizadas)
- **Datos** (`Reporte_VentasRealizadas.rpt`, `MAM_ReporteVentas` @tipo 1 / 2 / 13 / 14): mismo diseño que el escritorio.
  - Agrupado por **Sucursal → Personal** (con su código), con columnas Id cliente, Cliente, Comprobante (`Id - año`), Fecha venta y Total venta.
  - Subtotal por personal, total por sucursal y total general.
  - Mejoras: columna Tipo venta (si el SP la trae), total vendido y ticket promedio en la barra, y texto sin cortar.
- `ReporteImpreso` admite grupos anidados (`niveles`) y gráficos en la hoja (`htmlBarras`, `htmlColumnas`, `htmlIndicadores`).
- Endpoint `GET /api/ventas/reportes/aux/cabecera` (empresa y logo para cualquier reporte de Gestión Pedidos).
- **Verificar con tu BD:** los nombres de columna se buscan sin importar mayúsculas (SucursalId/NombreSucursal, PersonalId/Personal, ClienteId/NombreCliente, Id, FechaVenta, TotalVenta, TipoVenta). Si la Sucursal no sale, revisar qué nombre devuelve el SP.
- **Gráfico** (`ReporteGraficoVentasRealizadas.rpt`): igual que el escritorio, con el mismo dibujo SVG en pantalla y en la impresión (`htmlGraficoVR` + `CSS_GRAFICO_VR`).
  - **Ventas Por Vendedores**: subtítulo "Expresado en Bolivianos"; dona con el total al centro, el valor de cada vendedor con línea guía y una leyenda con colores, montos y total. Los vendedores van en orden alfabético.
  - **Ventas Por Sucursales**: barras con eje "Totales Ventas" (0K, 20K…) y "Sucursales", el valor dentro de la barra y una leyenda.

### v1.5 (Ventas vs costos como el escritorio)
- **Datos** (`VentasVsCostos.rpt`, `MAM_ReporteVentas` @tipo 4 / 5):
  - Título "Reporte de ventas vs costos", con "Expresado en Bolivianos".
  - Agrupado por **Personal** (con su código), con columnas Comprobante, Sucursal, Cliente, Fecha venta, Total costo, Total venta y Utilidad.
  - Subtotales por personal y total general.
- **Gráfico** (`ReporteGraficoUtilidades.rpt`):
  - Título "Reporte gráfico de utilidades de ventas".
  - **Ventas Por Personal y Utilidades**: torta con línea guía para cada valor y leyenda con total.
  - **Utilidades Por Sucursales**: barras con ejes "Utilidad" / "Sucursal".
  - Igual en pantalla y en la impresión.
- `VentasRealizadas.js` ahora tiene `MODOS` (configuración por reporte: columnas, grupos, títulos y gráficos), así cada reporte nuevo es solo configuración.
- `ReporteImpreso` acepta `subtitulo` (ej. "Expresado en Bolivianos").

### v1.6 (Productos más vendidos como el escritorio)
- **Datos** (`Reporte_ProductosMasVendidos.rpt`, `MAM_ReporteVentas` @tipo 6): todas las filas sin grupos, con columnas Código, Producto, Familia, Almacén, Cantidad y Total. Lleva totales.
- **Gráfico** (`ReporteGraficoProductosMasVendidos.rpt`): usa los **primeros N** productos del SP ("Mostrar", 10 por defecto, como `tbCantidad`).
  - **Productos Mas Vendidos**: barras por producto con ejes Cantidad / Productos, nombres inclinados, el valor en la barra y una leyenda en 2 columnas.
  - **Productos Mas Vendidos Por Sucursal**: una dona por sucursal, con el total al centro, el nombre de la sucursal y una leyenda con total.
  - Los productos van en orden alfabético y tienen el mismo color en los dos gráficos, con la paleta de Crystal. Pantalla e impresión son iguales y entran en una hoja.
- Cada reporte en `MODOS` define `normalizar` y `htmlGrafico`; `Reportes.jsx` es genérico.

### v1.7 (Menú lateral en acordeón)
- Solo **un módulo desplegado** a la vez: al abrir otro, el anterior se pliega. Si se vuelve a tocar el abierto, se pliega.
- Al entrar a un programa, o al hacer clic en el nombre del módulo, se despliega su módulo y se pliegan los demás. Funciona igual en PC y en celular.
- Los programas plegados no reciben foco con Tab (`inert`).

### v1.8 (Rendimiento del personal: gráfico de líneas por mes)
- **Gráfico** de "Rendimiento del personal" (`btVentRendimiento`):
  - **Con un vendedor**: una línea de sus ventas por mes con el valor en cada punto y un área suave. Se ve si sube o baja.
  - **Con varios**: una línea por vendedor para comparar quién vendió más.
- **Para que nada se tape:**
  - Cada vendedor tiene su color y un **marcador de forma distinta** (círculo, cuadrado, triángulo, rombo…).
  - Los nombres van al final de cada línea y se separan solos, con una línea guía.
  - Los valores sobre los puntos (con un solo vendedor) se ubican según la pendiente y buscan un lugar libre si chocan.
  - Las etiquetas de meses se omiten si no hay espacio.
  - Al pasar el mouse o tocar, se ve el detalle del mes (cada vendedor ordenado, ▲/▼ respecto al mes anterior y el total). Al pasar por una tarjeta se resalta esa línea.
  - Vista **"Separados"**: un gráfico por vendedor con la misma escala, sin cruces.
- **Tarjetas por vendedor:** total, promedio mensual, pedidos, tendencia (Sube / Baja / Estable, por regresión lineal) y mejor mes. Un clic oculta o muestra la línea.
- **¿En qué mes se vende más?:** columnas con el total de cada mes y el mejor mes resaltado. El **detalle por mes** es una tabla vendedor × mes con el que más vendió cada mes en verde.
- Si el rango es de un solo mes, se ofrece **"Ver últimos 12 meses"**, que también actualiza el filtro.
- **Imprimir gráfico:** hoja horizontal con las tarjetas, las líneas (o los separados), el mejor mes y el detalle.
- **Datos:** `GET /api/ventas/reportes/rendimiento/mensual`. Como `MAM_ReporteVentas` @tipo 8 devuelve el total del rango, se consulta una vez por mes (mismo SP y @tipo, con la misma tabla `@PersonalType`, solo lectura, de a uno). El rango máximo es de un año.
  - **Verificar con tu BD:** que @tipo 8 devuelva el nombre o Id del personal y el total (se buscan `Personal`/`NombrePersonal`, `Total`/`TotalVenta`, `Pedidos`/`Cantidad`).

### v1.9 (Reporte de cobranzas como el escritorio)
- `Reporte_Cobros.rpt` (`MAM_ReporteVentas` @tipo 17 todos / @tipo 18 un vendedor):
  - Agrupado por **Personal** (con su código), con columnas Cod venta, Fecha venta, Fecha pago, Glosa y **Monto**.
  - La glosa se muestra como "CLIENTE - glosa", igual que el escritorio.
  - La cabecera lleva "Movimiento desde / Hasta", el total cobrado y la cantidad de ventas cobradas. Hay subtotal por personal y total general.
- Antes la web mostraba todas las columnas del SP y el monto salía **repetido** (monto de la venta y monto del pago). Ahora se muestra un solo **Monto**: el cobrado (`Monto`, `MontoPago`, `Pago`…).
- Sin pestaña de gráfico, porque el escritorio no tiene.
- **Verificar con tu BD:** que la columna del pago se llame `Monto`. Si el SP usa otro nombre para el pago y `Monto` es el de la venta, avisar para ajustar el orden de búsqueda.

### v2.0 (Vista previa al imprimir y programas abiertos en pestañas, como el escritorio)
- **Vista previa** (`global/imprimir.js`): en **todos** los reportes, "Imprimir" abre una ventana con la hoja tal cual saldrá impresa (carta vertical u horizontal) y los botones **Imprimir** y **Cerrar**.
  - Esc, Cerrar o un clic fuera la cierran, y se sigue en el mismo programa con todo como estaba. El foco vuelve al botón Imprimir.
  - Ctrl+P dentro del visor imprime la hoja, no la aplicación.
  - En celular se abre a pantalla completa y la hoja se achica para verse entera.
  - El estado de cuenta de Cuentas por cobrar y por pagar y las tablas sin reporte propio ahora usan el reporte estándar con vista previa. La nota de venta ya era una vista previa: imprime directo.
- **Programas abiertos** (`Tec_Principal.jsx`): cada programa que se abre queda en una **pestaña** arriba, como "Programas Abiertos" del escritorio.
  - Cada programa **sigue cargado** al cambiar a otro: conserva filtros, búsquedas, reportes generados, la página de la lista y formularios a medio llenar. Cada pestaña tiene su propia ruta (`<Routes location>`) y recuerda hasta dónde se había bajado en la página.
  - Abrir un programa que ya está abierto (desde el menú, los mosaicos o los accesos del inicio) lleva a su pestaña tal como estaba.
  - Las pestañas con un registro a medio editar muestran un punto y, al cerrarlas, piden confirmación ("Cerrar sin grabar").
  - Cerrar con la X o con clic de la rueda del mouse; botón "Cerrar todas". Máximo 10: se cierra la más antigua que no esté en edición.
  - Al recargar la página se recuerdan los programas abiertos (`sessionStorage`), pero solo se carga el visible; los demás se cargan al tocarlos.

### v2.10 (Conciliación con colores y reportes en azul)
- **Conciliación de productos** (pantalla) como el escritorio: Item, Producto, Sal. #01…, **Tot. salidas** y **Entregado** en verde, **Devoluciones** en amarillo y Diferencia; la fila con diferencia se marca en rojo, debajo de la devolución se ve la calculada cuando no coincide y un aviso cuenta los productos con diferencia.
- **Comprobante de Conciliación de Productos** (botón "Comprobante"): hoja común con fecha y nro de conciliación, distribuidor, detalle, fecha/hora de impresión, Producto, Salida #01…, Total Salida, Devolución, Entregado, Diferencia, totales y firmas Encargado Almacen / Distribuidor.
- **Reportes con color:** la cabecera de las tablas y los títulos de todos los reportes impresos (reportes y notas) van en azul como los .rpt del escritorio, para que no se vean opacos.

### v2.9 (Comprobante de despacho)
- **Despachos:** al grabar o modificar sale el **Comprobante de Despacho de Productos** (`Reporte_Despacho`, `MAM_DespachoProductos @tipo 6`) en la misma vista previa estándar que la nota de venta: empresa y logo, fecha de despacho, código de despacho, distribuidor, nro de conciliación, detalle, productos (item, producto, cantidad), total general, fecha y hora de impresión y firmas **Encargado Almacen** / **Distribuidor**. Imprimir o No imprimir.
- Reimpresión desde la lista (botón de la fila y clic derecho) y desde el registro.
- `NotaImpresa` ahora admite documentos sin montos (sin "Son: … Bolivianos"), firmas al pie y ancho por columna.

### v2.8 (Tablas en pantalla, nota estándar y menú según el rol)
- **Listas que entran en la pantalla** (todas las de `GridBuscador`: Compras, Ventas, Proformas, Productos, Clientes, Movimientos, Despachos, Usuarios…): el ancho de cada columna es una referencia, los textos largos y los títulos pasan a otra línea, números y fechas no se cortan, la cabecera con filtros queda fija y en laptops la tabla es un poco más compacta. Sin barra horizontal a 1280 y 1440 px.
- **Nota imprimible estándar** (`componentes/NotaImpresa.jsx`): Nota de venta, Proforma y Nota de compra usan la misma hoja y la misma vista previa (Imprimir / Cerrar, o No imprimir al ofrecerla después de grabar). Cada programa solo arma sus datos.
- **Menú según el rol:** un programa se muestra (y se puede abrir) solo si tiene **Ver** marcado (la casilla "Asignar" de Roles de usuario). Un módulo sin programas asignados ya no aparece, aunque en la base hayan quedado Insertar/Modificar/Eliminar en 1. Al recargar, las pestañas de programas desasignados se cierran.

### v2.7 (Nota de compra y buscador con contador)
- **Compras:** al grabar o modificar sale la **Nota de Compra** en vista previa (como `ReporteCompra` del escritorio: "¿Desea Generar el Reporte de la Compra #?"), con **Imprimir** / **No imprimir**. Es la misma hoja que la nota de venta: empresa y logo, proveedor (dirección, teléfono), sucursal, tipo contado/crédito y vencimiento, notas de compra/recepción, detalle (código, producto, cajas, cantidad, costo, total), subtotal, descuento, total y literal. Datos: lista + `MAM_Compras @tipo 4` y `@tipo 7` (solo lectura). También desde la lista (botón de la fila y clic derecho) y desde el registro.
- **Buscador de productos de Compras, Movimientos y Despachos:** muestra "N agregados" como el de Pedido y Proforma.
- **Cuentas por pagar:** el detalle de la compra se desplaza (encabezado y total fijos) aunque tenga muchos productos; en Pagar, el preventista ya no sale "Null".

### v2.6 (Menú como el escritorio y Cuentas por pagar)
- **Menú de módulos igual al escritorio** (`server/src/negocio/menuEscritorio.js`, sacado de `Tec_Principal.Designer.vb`): como `_PCargarPrivilegios`, cada módulo muestra solo los botones que existen en su panel del escritorio, en el mismo orden y con el mismo texto. Lo que el rol tenga registrado pero no esté en el panel ya no aparece (ni como "Próximamente"). Ej. Inventario: Ingreso y Salida De Productos, Historial De Productos, Reporte De Saldos Productos, Saldos Valorados, Reporte General Historial y Monitoreo De Productos. El panel se elige por la posición del módulo (como `listaTabs.Item(i)`); si la base tuviera otro orden, por los botones en común.
- **Cuentas por pagar:** sin "Null" en Preventista (la columna se oculta si ninguna compra lo tiene); **Crédito/Compra** y **Vencimiento/Mora** (fecha y días de mora) en una sola columna cada una, para que la tabla entre en la pantalla; botón **Ver detalle de la compra** (productos, cajas, unidades, precio costo y total; `MAM_Compras @tipo 4`, solo lectura).

### v2.5 (Ventas por categoría como el escritorio)
- **Detallado** (`ProductosXCategoriaRep.rpt`, `MAM_Productos @tipo 9`): agrupado por **Categoría** y dentro por **Vendedor**, con FechaVenta, **Cod. Venta** (número de la nota de venta), Producto, Cantidad, Precio, Descuento y Total; totales por vendedor, por categoría y general.
- **General** (`ReporteGeneralPorCategoria.rpt`, `MAM_Productos @tipo 12`): agrupado por **Vendedor** con Categoría, Cantidad y Total, y "Total: <vendedor>" al pie de cada uno.
- Los dos salen con el título "Reporte de ventas realizadas", Desde/Hasta, usuario y los filtros elegidos. Al cambiar entre Detallado y General se limpia el resultado anterior.

### v2.4 (Buscador de productos con teclado e impresión de proformas/ventas)
- **Buscador de productos** (Ventas, Proformas, Compras, Despachos, Movimientos) manejable solo con teclado: se escribe para filtrar, **Flecha abajo** baja a la tabla, las flechas / Inicio / Fin / RePág / AvPág mueven la fila marcada y **Enter** la elige. La cantidad se abre con el foco en **Unidades** (las cajas se calculan solas); **Enter** agrega y el foco vuelve al buscador vacío para la siguiente búsqueda. Flecha arriba en la primera fila vuelve al buscador y escribir estando en la tabla sigue la búsqueda. Esc en la cantidad vuelve a la fila; Esc en el buscador lo cierra.
- Corregido: **Esc dentro de una ventana** (buscador, cantidad, ayudas) ya no sale del registro que se estaba editando.
- **Al grabar o modificar** una venta o proforma se muestra su nota/proforma lista para imprimir (como `ReporteVenta` del escritorio: "¿Desea Generar el Reporte…?"): **Imprimir** (con el foco, basta Enter) o **No imprimir**.
- En la **lista** de Ventas y Proformas cada fila tiene **Imprimir** (botón de la fila y clic derecho / toque largo). `ModeloF1` acepta `accionesFila` para agregar acciones estándar a cualquier programa.

### v2.3 (Proformas como el escritorio)
- **Agregar productos** en una proforma lista **todos** los productos con `MAM_Ventas @tipo 5` (igual que `ProformaDetalle`), nunca los de la conciliación del chofer (`@tipo 20`, que dejaba el buscador vacío). Los **sin stock se ven en rojo** y **se pueden elegir**, sin límite de cantidad por stock (como `frm_CantidadCajas` con TipoMovimiento = 4): es una cotización, no una venta. El servidor tampoco valida stock al grabar una proforma.
- La **nota de la proforma** es la misma hoja que la nota de venta del pedido: ahora usa `ListarProformaRecibo` (`MAM_Proforma @tipo 6`) para Nit/Ci, teléfono, dirección, hora de registro y código de cada producto.

### v2.2 (Pestañas en 2 filas y programa maximizado)
- **Pestañas:** si caben, van en una fila; si no, se reparten en **2 filas como máximo** y las que sobran quedan a la derecha con **scroll horizontal**. La pestaña activa siempre queda a la vista.
- Botón **Maximizar** (al lado de "Cerrar todas"): oculta el menú lateral y la cabecera para que el programa use toda la pantalla. **Restaurar** vuelve al menú con el mismo programa, sin perder lo que se estaba haciendo.
- El modo maximizado se recuerda (también al cambiar de programa o al volver por "Programas abiertos"); en el Inicio y en los menús de módulo el menú lateral siempre se ve.

### v2.1 (Impresión en ventana aparte, pestañas en filas y "Programas abiertos" en el menú)
- **Imprimir** (desde la vista previa o con Ctrl+P) abre la hoja en una **ventana aparte**. El cuadro de impresión del navegador aparece en esa ventana, no sobre la aplicación, y la ventana se cierra sola al imprimir o cancelar. Si el navegador bloquea las ventanas emergentes, se imprime desde la hoja de la vista previa.
- **Pestañas en filas:** pasan a la fila siguiente (tamaño fijo, sin achicarse ni barra horizontal), con **3 filas visibles como máximo** (en v2.2: 2 filas y scroll horizontal). El contenido se acomoda debajo.
- Igual que en el escritorio, la **barra de pestañas se ve solo mientras se trabaja en un programa**. En el Inicio o en el menú de un módulo se oculta.
- En el menú lateral, el ítem **"Programas abiertos"** (con la cantidad) se selecciona solo al abrir un programa y lleva de vuelta al último que se estaba usando, tal como estaba. Ese último se recuerda al recargar.

### Diseño de los .rpt del escritorio (herramientas/volcar_rpt.bat)
- Lee **todos** los `.rpt` de `TeVendo\ReporteSmartApps` con el motor de Crystal instalado en la PC y guarda su diseño en `herramientas\rpt\<Reporte>.json`: secciones, textos, campos con su posición y fuente, grupos, orden, fórmulas, totales, parámetros y gráficos (tipo, títulos, ejes y campos).
- Solo lectura: no se conecta a la base ni modifica los `.rpt`. Si falla con PowerShell de 64 bits, se reintenta con el de 32 bits.
- Con esos JSON se replican en la web los reportes que faltan, igual que el escritorio.

### Verificar contra la BD de pruebas
- `iniciar.bat` ejecuta `verificar.js` (solo lectura). Revisa que los SP de Ventas acepten todos los parámetros que envía la web, que `VentaDetalleType` tenga las mismas columnas que el detalle y que los tipos de Asignación de pedidos (`@Asignacion`, `@AsignacionEntregado`, `@AsignacionAnulacion`) coincidan con sus grillas.
- `probar_ventas_bd.bat` hace altas, modificaciones y bajas de Personal, Proveedores y Ventas con tus SP reales, dentro de una transacción que **se revierte** al final.
  - Pide escribir el nombre de la base antes de empezar.
  - **No lo uses en producción.**

## Pendiente (próximos pasos)
- Lotes (Ventas, Movimientos y Compras) siguen solo en el escritorio.
- Las contraseñas siguen en texto plano porque el escritorio las compara así. Migrar a hash (bcrypt) cuando el escritorio ya no se use para ingresar.
