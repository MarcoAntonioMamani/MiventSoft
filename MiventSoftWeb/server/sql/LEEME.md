# Scripts de base de datos (server\sql)

Se ejecutan **a mano en SSMS**: abrir el archivo y ejecutar completo (F5). `iniciar.bat` NO los ejecuta.
Antes: **backup** de la BD y hacerlo **fuera del horario de atención**.

**El número del nombre es el orden.** Se ejecutan de menor a mayor, en la BD elegida en SSMS.
La carpeta `revertir\` NO es parte de la instalación (ver más abajo).

## Orden de instalación

| # | Archivo | Qué hace | ¿Sirve para cualquier BD? | Requiere |
|---|---|---|---|---|
| 01 | `01_IX_VentasPanel.sql` | Índices del panel y la lista de ventas | Sí, tal cual | — |
| 02 | `02_IX_ProductosPrecios.sql` | Índices de precios y stock | Sí, tal cual | — |
| 03 | `03_MAM_PanelControl.sql` | SP propio del panel de la web (el escritorio no lo usa) | Sí, tal cual | — |
| 04 | `04_sp_Mam_Precios_tipo5.sql` | Lentitud de la pantalla Precios (cambia 1 línea) | **No**: generar para esa BD | — |
| 05 | `05_StockSeguro.sql` | Validación de stock en ventas y salidas, y triggers de saldo | **No**: generar para esa BD | 02 |
| 06 | `06_EliminarRegistros.sql` | Permiso de usuario "Eliminar registros" (columna `Usuarios.EliminarRegistros` + `MAM_Usuarios`) | **No**: generar para esa BD | — |
| 07 | `07_Auditoria.sql` | Tabla `AuditoriaRegistros`, SP `MAM_Auditoria` y programa web "Auditoría De Registros" | Sí, tal cual | — |
| 08 | `08_Auditoria_SPs.sql` | `MAM_Ventas`, `MAM_Compras` y `MAM_Movimientos` registran lo modificado y lo eliminado | **No**: generar para esa BD | 05 y 07 |
| 09 | `09_CompraEliminarStock.sql` | No deja eliminar una compra si el stock quedaría negativo (lo comprado ya se vendió o salió) | **No**: generar para esa BD | 08 |
| 10 | `10_AuditoriaEquipo.sql` | Auditoría: desde dónde se modificó o eliminó (equipo, IP, navegador, ubicación GPS) | Sí, tal cual | 07 |
| 11 | `11_AuditoriaUsuario.sql` | Auditoría: nombre de usuario **completo** en compras y movimientos (antes salía cortado a 10 letras) | Sí, tal cual | 10 |
| 12 | `12_AuditoriaUbicacionPrecisa.sql` | Auditoría: latitud y longitud con **18 decimales** (antes 6) | Sí, tal cual | 11 |

Después del paso 05, revisar si ya hay stock negativo (esos productos no se podrán vender hasta corregirlos con un ingreso):

```sql
SELECT s.ProductoId, p.NombreProducto, s.DepositoId, s.Cantidad
FROM ProductosStock s INNER JOIN Productos p ON p.Id = s.ProductoId
WHERE s.Cantidad < 0
```

### Pasos 06 a 12 (permiso de eliminar, auditoría, stock al eliminar, equipo, usuario y ubicación)

- **06** — todos los usuarios (los que ya existen y los nuevos) quedan **sin** el permiso. Después, en *Gestión De Usuarios*, se le marca a quien sí debe eliminar (también al administrador). Hasta entonces **nadie** puede eliminar. Sin el permiso no se pueden eliminar ventas, compras, movimientos (ingresos, salidas y traspasos) ni pagos de cobranzas o de cuentas por pagar, ni desde la web ni desde el escritorio nuevo.
- **El escritorio nuevo (TeVendo.exe) necesita el 06 instalado**: el login lee la columna `EliminarRegistros`. Un escritorio anterior sigue funcionando con el 06 instalado (no controla el permiso y, al modificar un usuario, conserva el valor).
- **07** — la tabla de auditoría es de **solo lectura**: no se puede modificar ni borrar. Si la auditoría falla por algo, **nunca impide grabar**: el error queda en la `Bitacora`.
- **08** — se registra quién, cuándo, desde qué equipo y si fue desde la web o desde el escritorio, con la foto **antes** y **después** (cabecera y detalle). Guardar sin cambiar nada no se registra. Lo que hacen los SP queda igual (ventas sigue borrando su detalle al eliminar).
- **09** — al eliminar una compra, si lo que ingresó ya se vendió o salió y el stock quedaría **negativo**, no se elimina nada y la web muestra los productos (quedar en 0 sí se permite). Eliminar un **ingreso de inventario** ya lo controla el 05. Eliminar la compra ahora es todo o nada.
- **10** — la auditoría guarda también **desde dónde**: en el escritorio, el nombre de la PC y su IP; en la web, el nombre que se le dio al equipo (menú del usuario → *Nombre de este equipo*) con su código, la IP, el navegador y el sistema, y la **ubicación GPS** solo desde la app del celular (por http el navegador no entrega la ubicación). Los registros anteriores al 10 quedan sin esos datos. Si después se vuelve a ejecutar el 07, conserva la versión del 10.
  - **Nombre de la PC (web)**: lo averigua el servidor por la IP (DNS / LLMNR / NetBIOS, como `ping -a`). Solo funciona si el servidor está en la **misma red** que las PCs; por internet no se puede y queda el nombre dado en *Nombre de este equipo* (o su código).
  - **IP**: se guarda "IP local | IP pública". La pública de las PCs de la red es la de internet de la oficina (el servidor la consulta a `api.ipify.org` una vez por hora). `AUDITORIA_IP_PUBLICA=0` en `server\.env` lo desactiva.
  - **Ubicación**: GPS de la app del celular, o del navegador solo con **https**. Si no hay GPS se guarda la **aproximada por IP pública** (`ipapi.co`, a nivel ciudad, puede errar por kilómetros; se muestra como "Aproximada por IP"). `AUDITORIA_UBICACION_IP=0` la desactiva.
- **11** — `MAM_Compras` y `MAM_Movimientos` reciben el usuario cortado a 10 letras ("MARCO ANTO"); esos SP **no se tocan**. La web deja el nombre completo para la auditoría; desde el escritorio, si el nombre vino cortado y hay **un solo** usuario (o su personal) cuyo nombre empieza así, se graba ese nombre. Los registros anteriores cortados se **muestran** con el nombre completo (lo grabado no cambia: la tabla es de solo lectura). Si después se vuelve a ejecutar el 10, conserva la versión del 11.
- **12** — `AuditoriaRegistros` y `AuditoriaContexto`: `Latitud` y `Longitud` pasan de `decimal(9,6)` a `decimal(21,18)`. La web envía la coordenada como texto, tal como la da el GPS, sin redondear. `MAM_Auditoria` devuelve además `LatitudTexto` / `LongitudTexto` con el valor exacto. Lo ya grabado se conserva. Para deshacer el 11 hay que deshacer primero el 12.
- El programa "Auditoría De Registros" queda en el módulo de *Gestión De Usuarios* (solo web), asignado al rol 1 (Administrador). A otros roles se asigna en *Roles de Usuario*.

Los archivos 04, 05, 06, 08 y 09 que están en esta carpeta son los de **KailiIndustrialLaPaz**. El 08 se generó con el 05 ya instalado y el 09 con el 08 ya instalado.

## Si algo sale mal: carpeta `revertir\` (NO es parte de la instalación)

Solo para deshacer, **en orden inverso** (primero el número más alto):

| Deshace | Archivo |
|---|---|
| 11 | `revertir\REVERTIR_11_AuditoriaUsuario.sql` (vuelve `MAM_Auditoria` a la versión del 10) |
| 12 | `revertir\REVERTIR_12_AuditoriaUbicacionPrecisa.sql` (vuelve `MAM_Auditoria` a la versión del 11 y las columnas a 6 decimales) |
| 10 | `revertir\REVERTIR_10_AuditoriaEquipo.sql` (las columnas nuevas y su historial se conservan) |
| 09 | `revertir\REVERTIR_09_CompraEliminarStock.sql` |
| 08 | `revertir\REVERTIR_08_Auditoria_SPs.sql` |
| 07 | `revertir\REVERTIR_07_Auditoria.sql` (la tabla `AuditoriaRegistros` y su historial se conservan) |
| 06 | `revertir\REVERTIR_06_EliminarRegistros.sql` (antes volver a la versión anterior de la web y del escritorio) |
| 05 | `revertir\REVERTIR_05_StockSeguro.sql` (si el 08 está instalado, primero deshacer el 08) |

Si el script no está instalado, avisa "NO esta instalado… no hay nada que deshacer" y no toca nada.

## Otra base de datos (pasos 04, 05, 06, 08 y 09)

Esos pasos llevan el SP **completo** de la BD de la que salieron. Si en la otra BD el SP es distinto, el script avisa "…es OTRA versión… No se modificó nada" y hay que generarlos desde esa base:

```
cd server
node sql\generar\precios_tipo5.js       --bd=NombreDeLaBD
node sql\generar\stockseguro.js         --bd=NombreDeLaBD
node sql\generar\eliminar_registros.js  --bd=NombreDeLaBD
node sql\generar\auditoria_sps.js       --bd=NombreDeLaBD   (despues de instalar el 05 en esa BD)
node sql\generar\compra_eliminar_stock.js --bd=NombreDeLaBD (despues de instalar el 08 en esa BD)
```

- Usa la conexión de `server\.env`. Con `--bd=` se elige otra BD del **mismo servidor**. Si es otro servidor, primero hay que apuntar `server\.env` a ese servidor (`configurar.ps1 -Forzar`).
- Los scripts salen en `server\sql\generados\NombreDeLaBD\` (y los de deshacer en su subcarpeta `revertir\`). Se ejecutan en SSMS **en esa BD**, con el mismo orden de números.
- Si el SP de esa BD tiene otra estructura, el generador se detiene con "NO SE GENERO NADA: …" y no escribe nada. Eso hay que revisarlo a mano. Pasa con `DistribucionDistralKCP2023`, cuyo `MAM_Ventas` es distinto.
- El generador solo lee el código de los SP; no cambia nada en la BD.

## Protecciones de los scripts

- Si ya están instalados, avisan **"YA ESTA INSTALADO… No se modificó nada"**. Ejecutarlos dos veces no hace daño.
- Si el SP de la BD es otra versión que la que se usó para generarlos, avisan **"…es OTRA versión… No se modificó nada"** y no tocan nada.
- Si falta un paso anterior (por ejemplo el 08 sin el 07), avisan **"falta ejecutar primero…"** y no tocan nada.
- 06 y 08 van en una transacción: o queda todo instalado o no queda nada.
- La comparación ignora mayúsculas y espacios, porque SQL Server reescribe `CREATE` al hacer `ALTER`.

## Probar sin dejar nada grabado

```
cd server
node test\probar_inventario_bd.js KailiIndustrialLaPaz --aplicar=sql\05_StockSeguro.sql --rechazo=venta
node test\probar_auditoria_bd.js  KailiIndustrialLaPaz
```

- `probar_inventario_bd.js`: ventas, compras, movimientos y traspasos con las rutas reales de la web. Está armada con los datos de Kaily (sucursales 1 y 2, categoría 3).
- `probar_auditoria_bd.js`: aplica 06 a 12 y los vuelve a ejecutar. Prueba que una compra ya vendida no se pueda eliminar, que se grabe desde qué equipo (nombre, IP, navegador, GPS con 18 decimales) y que el usuario quede completo (web y escritorio). Modifica y elimina una venta, una compra y un ingreso, y revisa la auditoría y la pantalla. Prueba que un usuario sin permiso no pueda eliminar. Al final ejecuta los REVERTIR.
- Las dos corren dentro de una transacción que al final se revierte.
