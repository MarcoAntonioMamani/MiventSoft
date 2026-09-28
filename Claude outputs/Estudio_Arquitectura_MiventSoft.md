# Estudio de arquitectura — MiventSoft / BD KailiIndustrialLaPaz

Base: script de BD subido hoy (`KailyLaPazScript.sql`, fecha de script 24/09/2026 — se asume el más actual) +
código en `D:\Sistemas\MivenSoft2025\MiventSoft` (WinForms VB.NET). Todo lo que sigue está verificado contra
el script real o el código real, no asumido. Donde algo queda como duda explícita, es porque el script no lo
resuelve (p. ej. no incluye triggers).

## 1. Arquitectura general (confirmada en sesiones previas + hoy)

```
UI (WinForms, Janus GridEX + DevComponents DotNetBar)
   Tec_XXX.vb / Tec_XXX.Designer.vb
        |
Negocio/AccesoLogica.vb   <-- UN SOLO ARCHIVO, ~158 KB, funciones Shared
        |  arma List(Of Datos.DParametro) y llama D_ProcedimientoConParam
        v
Datos/AccesoDatos.vb + MetodoDatos.vb + DParametro.vb + Configuracion.vb
        |
        v
SQL Server: 30 SPs "mega-SP" (MAM_XXX / sp_Mam_XXX), cada uno despachado
por un parámetro @tipo con decenas de ramas IF @tipo=N ... BEGIN...END,
todas con BEGIN TRY/CATCH -> INSERT INTO Bitacora en error.
```

`Negocio/AccesoLogica.vb` es un único archivo monolítico. Es el punto de mayor concentración de lógica de
negocio del lado .NET (fuera del propio SQL) y el mayor candidato a cuello de botella de mantenibilidad si
se sigue creciendo ahí — cualquier reescritura debería como mínimo particionarlo por dominio.

**Importante para el proyecto web (ya charlado):** no toda la lógica de negocio vive en los SP. Cosas como
deduplicación de filas en grillas, generación manual de Id (`Max(Id)+1` en memoria), cálculos de conversión
cantidad↔caja, habilitación condicional de controles según combos, y validaciones de stock, viven en el
code-behind VB.NET de cada formulario. Portar un módulo a web exige auditar ESE código, no solo el SP.

## 2. Inventario de código (por carpeta, sin bin/obj/packages)

| Carpeta | Contenido |
|---|---|
| `Datos/` | 4 archivos: `AccesoDatos.vb`, `Configuracion.vb`, `DParametro.vb`, `MetodoDatos.vb`. Capa de acceso a datos pura. |
| `Negocio/` | 1 archivo: `AccesoLogica.vb` (158 KB, monolito). |
| `Modelo/` | 21 archivos, 8 formularios base (`ModeloF0/F1/F2/F00/FR/R0`, `ModeloAyuda`, `ModeloConTab`, `ModelUnico`) + `Celda.vb`/`MGlobal.vb`. Varios (`ModeloConTab`, `ModeloF00`, `ModeloF2`) parecen stubs casi vacíos — verificar si están en uso real. |
| `Facturacion/` | 5 archivos, **sin formularios** (librería pura): `AllegedRC4.vb`, `Base64SIN.vb`, `ControlCode.vb`, `ConvertirLiteral.vb`, `Verhoeff.vb`. Es la librería de facturación electrónica SIN (Bolivia): cifrado, dígito verificador Verhoeff (típico de CUF/CAFC), literal de montos. **Riesgo a señalar:** `AllegedRC4.vb` sugiere una implementación RC4 "alegada"/no estándar — si cifra algo del comprobante fiscal, amerita revisión de seguridad antes de asumirla apta para producción. |
| `TeVendo/Caja` | 7 formularios: `Tec_CajaGeneral`, `Tec_CierreCajaCajero`, `Tec_Kits`, `Tec_ProgramaIngresoEgresoCaja`, reportes de caja. |
| `TeVendo/Control` | 3 clases de UserControl (`Imageus`, `UCImg`, `UCLavadero`). |
| `TeVendo/Distribucion` | 3 formularios: `Tec_Conciliacion`, `Tec_DespachoDetalle`, `Tec_Despachos`. |
| `TeVendo/Inventarios` | 13 formularios/reportes (ya trabajados: `Tec_Movimientos`, `Tec_MovimientoDetalle`, `FormularioStock`, kardex, reportes de stock). |
| `TeVendo/Modulo Configuracion` | 14 formularios: Productos, Categorías, Clientes, Empresas, Precios, Roles, Usuarios, Zonas, Mapa/Mapaclientes, popups de cantidad. |
| `TeVendo/Modulo RecusosHumanos` | 6 formularios (RRHH/nómina, **no explorado antes**): `Tec_ConceptosFijos`, `Tec_Contratos`, `Tec_Listarplanilla`, `Tec_RegistrarPersonalAPlanilla`, `Tec_VisualizarConceptoFijosPlanilla`, `Tec_VisualizarConceptosVariablesPlanilla`. |
| `TeVendo/ModuloFacturas` | 1 formulario: `Tec_Dosificacion` (dosificación de facturas ante el SIN). |
| `TeVendo/Modulos Compras Ventas` | 10 formularios: Ventas, Compras, Proformas, Cuentas por cobrar/pagar, Proveedores, Personal. `Tec_Ventas.vb` es el archivo más grande del proyecto (94 KB). |
| `TeVendo/Programas Principales` | `Tec_Login`, `Tec_Principal` (`.resx` de ~2.9 MB — revisar qué recursos embebidos son). |
| `TeVendo/ReporteSmartApps` | ~35 pares `.rpt`/`.vb` — reportes Crystal Reports, no son formularios. |
| `TeVendo/Utils` | 11 formularios de utilidad (popups de ayuda, crear categoría/marca/proveedor, `Efecto.vb` como dispatcher central). |

## 3. Modelo de datos real (script de hoy)

**73 tablas, 30 SPs, 18 Table Types (TVP), 28 vistas.** Lista completa de SPs:

`MAM_Almacenes, MAM_App, MAM_AppMovil, MAM_CajaIngresoEgreso, MAM_Categorias, MAM_CierreCajero,
MAM_Clasificadores, MAM_Clientes, MAM_Compras, MAM_ConceptosFijos, MAM_Conciliacion, MAM_Contratos,
MAM_CreditosCompras, MAM_CreditosVentas, MAM_DespachoProductos, MAM_Dosificacion, MAM_Empresas, MAM_Kits,
MAM_Movimientos, MAM_Personal, MAM_Planilla, MAM_Productos, MAM_Proveedores, MAM_ReporteVentas, MAM_Roles,
MAM_Usuarios, MAM_Ventas, MAM_VentasIncremento, sp_Mam_Precios, sp_Mam_Zonas`

`MAM_App` y `MAM_AppMovil` no tienen un formulario `Tec_` correspondiente evidente en el inventario — sugiere
que hay (o hubo) un canal adicional (app móvil / API) que consume estos mismos SPs. Vale confirmarlo: si es
así, el proyecto web tendría que convivir con ese tercer consumidor, no solo con el desktop.

### 3.1 Mapeo módulo ↔ SP ↔ tablas principales

| Módulo (UI) | SP | Tablas núcleo |
|---|---|---|
| Ventas / VentasDetalle | `MAM_Ventas` | Ventas, VentasDetalles, VentasMonedaCobrada, VentasDetallesBackup |
| Compras / ComprasDetalle | `MAM_Compras` | Compras, CompraDetalle, Proveedor |
| Movimientos / MovimientoDetalle | `MAM_Movimientos` | Movimientos, MovimientosDetalle, MovimientosLog, MovimientosDetalleLog |
| Productos / Categorías | `MAM_Productos`, `MAM_Categorias` | Productos, Categorias, ProductosStock, ProductosImagenes, ProductosCodigoBarras* |
| Precios | `sp_Mam_Precios` | Precios, PreciosCategorias, PreciosBackup, Precios_Log |
| Clasificadores (Marca/Familia/etc.) | `MAM_Clasificadores` | Clasificadores, ClasificadorDetalle |
| Clientes / Zonas | `MAM_Clientes`, `sp_Mam_Zonas` | Clientes, Zonas, ZonasPuntos |
| Caja | `MAM_CajaIngresoEgreso`, `MAM_CierreCajero` | CajaIngresoEgreso, Cajas, CierreCaja*, CierreCajero* |
| Créditos | `MAM_CreditosCompras`, `MAM_CreditosVentas` | CreditosCompras/Ventas, Transaccion*Credito*, Transacciones*CreditoDetalle |
| Distribución | `MAM_DespachoProductos`, `MAM_Conciliacion` | Despacho, DespachoProductos(Detalle), Conciliaciones(Detalle) |
| RRHH / Planilla | `MAM_Contratos`, `MAM_ConceptosFijos`, `MAM_Planilla`, `MAM_Personal` | Personal, Contratos, ContratosConceptos, ConceptosFijos, PlanillaSalarios(Conceptos) |
| Facturación electrónica | `MAM_Dosificacion` + librería `Facturacion/` | Factura, FacturaDetalle, FacturacionNit, Dosificacion |
| Kits | `MAM_Kits` | Kits, KitsProductos |
| Usuarios / Roles | `MAM_Usuarios`, `MAM_Roles` | Usuarios, Roles, RolesDetalles |
| Reportes de ventas | `MAM_ReporteVentas`, `MAM_VentasIncremento` | Ventas*, VentasIncrementoDiario |

*(`ProductosCodigoBarras` no apareció en el listado de 73 tablas de este script — confirmar si sigue
existiendo con otro nombre o si fue absorbida; se usó activamente en la migración ANABEL de hace unos días.)*

### 3.2 Hallazgo importante: ya existe infraestructura de auditoría, no capturada en sesiones anteriores

El script trae tres tablas de log/auditoría que **no conocíamos** cuando armamos `Tec_AuditoriaVentas` hace
unos días:

- `MovimientosLog` / `MovimientosDetalleLog`: columnas `Accion`, `FechaLog`, `Usuario`, `Procedimiento`,
  `ObservacionLog` — pinta de log aplicativo (poblado desde el SP o desde la app).
- `Precios_Log`: mucho más sofisticada — columnas `_Old`/`_New` por campo, más `UsuarioEjecutor`,
  `FechaHoraUtc`, `HostName`, `AppName`, `Spid`, `TxGuid`. Este patrón (Spid, TxGuid, HostName) es típico de
  un **trigger de SQL Server**, no de código de aplicación.

**El script no incluye ningún `CREATE TRIGGER`**, así que no puedo confirmar si estas tablas están siendo
alimentadas activamente ahora mismo o si son infraestructura preparada y aún no conectada. Esto es una
pregunta directa para vos, no algo que deba asumir: ¿hay triggers en la BD real que no se incluyeron al
generar este script (revisá la opción "Include triggers" al exportar), o el llenado de `Precios_Log` lo hace
`sp_Mam_Precios` explícitamente? Si ya existe auditoría real para Movimientos/Precios, el trabajo pendiente de
`Tec_AuditoriaCompras`/`Tec_AuditoriaMovimientos` que quedó pausado podría simplificarse mucho (leer de estas
tablas en vez de construir todo el mecanismo desde cero como hicimos para Ventas).

### 3.3 Integridad referencial: solo 69 FK para 73 tablas

Se parsearon 69 `FOREIGN KEY` reales del script. Es poca cobertura para 73 tablas — confirma lo que ya
sabíamos por el patrón de Id manual (`Max(Id)+1`): buena parte de la integridad referencial (Ventas→Clientes,
VentasDetalles→Productos, etc.) se sostiene por convención en el código de aplicación, no por constraint en
la base. Esto es relevante para el proyecto web: un backend nuevo tiene que replicar esas validaciones "a
mano", la base no las va a rechazar por sí sola.

Dos relaciones llaman la atención y convendría confirmar que no sean un artefacto de la generación del script
(no las tomo como bug, solo como algo a verificar contra la BD real):

- `TransaccionComprasCredito.TransaccionCompraId -> TransaccionComprasCredito.Id` (autoreferencia sobre la
  misma tabla y mismo campo lógico).
- `RolesDetalles.Id -> RolesDetalles.Id` (ídem).

Y dos que sí parecen un patrón de diseño deliberado (reutilizar el `Clasificador` genérico también para
valores de calendario), no un error:

- `PlanillaSalarios.Anio -> ClasificadorDetalle.Id`
- `PlanillaSalarios.Mes -> ClasificadorDetalle.Id`

## 4. Convenciones de codificación observadas (confirmadas en código real, no supuestas)

- **Generación de Id:** manual, `Max(Id)+1` calculado en VB o en el propio SP — no `IDENTITY` en las tablas
  transaccionales núcleo (Productos, Categorias, PreciosCategorias, filas de detalle en grillas). Las tablas
  de log sí usan `IDENTITY` (`MovimientosLog`, `Precios_Log`, etc.) — la inconsistencia entre "tablas de
  negocio sin identity" y "tablas de log con identity" es la firma de que el patrón de Id manual fue una
  decisión de diseño explícita en su momento, no un descuido.
- **Patrón "Clasificador genérico":** `Clasificadores(Id, Grupo)` + `ClasificadorDetalle(Id identity,
  IdClasificador, orden, Descripcion)` — usado para Marca, Familia, Atributo, UnidadVenta/Maxima, y también
  (según el punto anterior) para Año/Mes de planilla. Es el mecanismo de "enum configurable" de todo el
  sistema.
- **Manejo de errores:** `BEGIN TRY/CATCH` en cada rama de cada SP, con `INSERT INTO Bitacora` on error
  (número, procedimiento, línea, mensaje). Centralizado y consistente en los 30 SPs.
- **UI:** Janus GridEX (grillas) + DevComponents DotNetBar (botones, tabs, labels) en todos los formularios.
  Boilerplate de Designer.vb muy repetitivo entre formularios (mismo patrón de `BeginInit`/`SuspendLayout`).
- **Capa Negocio:** funciones `Shared` en un único archivo, nombradas `L_prXxx`/`L_fnXxx`, arman
  `List(Of Datos.DParametro)` y delegan a `D_ProcedimientoConParam`.
- **Dispatcher de popups:** `Efecto.vb` es un formulario único que, según un parámetro `tipo`, decide qué
  sub-formulario mostrar (confirmaciones, selección de producto, cantidad, etc.) — punto único de navegación
  para diálogos secundarios.

## 5. Riesgos/SPOFs a tener presentes para cualquier trabajo futuro (desktop o web)

1. **Concurrencia de Id manual** (`Max(Id)+1`): condición de carrera real bajo más de un usuario concurrente
   escribiendo la misma tabla. Ya señalado en sesiones anteriores; se vuelve más probable, no menos, en un
   escenario web con más usuarios simultáneos.
2. **`Negocio/AccesoLogica.vb` como monolito de 158 KB**: punto único de fallo de mantenibilidad — un cambio
   mal aislado ahí puede afectar módulos no relacionados.
3. **Integridad referencial débil** (69 FK / 73 tablas): cualquier capa nueva (web u otra) tiene que
   reimplementar validaciones que hoy la base no garantiza.
4. **Posible canal adicional (`MAM_App`/`MAM_AppMovil`)** consumiendo los mismos SPs sin un formulario
   Desktop asociado — confirmar antes de asumir que el desktop WinForms es el único cliente.
5. **`AllegedRC4.vb`** en la librería de facturación electrónica — verificar qué cifra y si es aceptable para
   datos fiscales antes de asumirlo apto para producción.
6. **Tablas de auditoría sin trigger visible en el script** (`MovimientosLog`, `MovimientosDetalleLog`,
   `Precios_Log`) — confirmar si están activas hoy; si lo están, cambia el plan de trabajo pendiente de
   auditoría para Compras/Movimientos.

## 6. Preguntas abiertas para vos (no asumidas, necesito el dato)

- ¿`Precios_Log`/`MovimientosLog` se llenan hoy vía trigger (no incluido en el script) o vía código? Si es
  trigger, ¿podés exportarlo o decirme el nombre para leerlo?
- ¿`MAM_App`/`MAM_AppMovil` corresponden a una app móvil/API real en uso, o son legado sin cliente activo?
- ¿`ProductosCodigoBarras` (usada en la migración ANABEL) sigue existiendo? No apareció en este script de 73
  tablas — puede ser que el script no incluya todo el esquema o que la tabla tenga otro nombre ahora.
