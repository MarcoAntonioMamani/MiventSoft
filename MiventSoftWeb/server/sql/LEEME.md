# Scripts de base de datos — Distral (server\sql)

Se ejecutan **a mano en SSMS**: abrir el archivo y ejecutar completo (F5), en la BD de Distral.
`iniciar.bat` NO los ejecuta. Antes: **backup** de la BD y hacerlo **fuera del horario de atención**.

**El número del nombre es el orden.** Se ejecutan de menor a mayor. La carpeta `revertir\` NO es parte de la instalación.

## Orden de instalación

| # | Archivo | Qué hace |
|---|---|---|
| 01 | `01_IX_Indices.sql` | Índices: lista de pedidos 11 s → 0,15 s; grabar/eliminar pedidos, compras y movimientos más rápido |
| 02 | `02_MAM_PanelControl.sql` | SP propio del panel de Inicio de la web: clientes que dejaron de comprar, clientes rentables, productos sin ventas |
| 03 | `03_sp_Mam_Precios_tipo5.sql` | Pantalla de Precios: corrige una línea de `sp_Mam_Precios @5` que podía tardar decenas de segundos |
| 04 | `04_StockSeguro.sql` | No se vende ni se saca más de lo que hay: pedidos de la web/escritorio (`MAM_Ventas`), **pedidos del celular** (`sp_go_TC004_appMovil`), movimientos de inventario (salidas y eliminar ingresos) y triggers de saldo |
| 05 | `05_CompraStock.sql` | No se elimina ni se modifica una compra si el stock quedaría negativo (lo comprado ya se vendió o salió) |
| 06 | `06_IncrementoMovil.sql` | **Incremento del día en los pedidos del celular** (app nueva, `/apiV4`): TotalVenta y crédito con el incremento, fila en `VentasIncrementoAplicado` (igual que la web); al modificar desde el celular se conserva |
| 07 | `07_UbicacionMovil.sql` | **GPS, fecha/hora y modelo del celular** al levantar el pedido y al entregarlo/anularlo: tabla `VentasRegistroMovil`, SP `MAM_VentasRegistroMovil` y programa web *Ubicación de pedidos y entregas* (`btnRegistroCelular`, rol Administrador) |
| 08 | `08_PedidoConDetalle.sql` | Un pedido del celular **nunca queda solo con la cabecera**: si llega sin productos o falta alguno, no se graba nada (ROLLBACK) |
| 09 | `09_PedidosObservados.sql` | **Pedidos del celular que no pueden entrar** (sin stock, rechazados, sin productos, fallas repetidas): tabla `PedidosMovilObservados`, SP `MAM_PedidosMovilObservados` y programa web *Pedidos Observados Celular* (`btnPedidosObservados`, rol Administrador) con notificación. **No modifica** `sp_go_TC004_appMovil`; se puede ejecutar más de una vez |
| 10 | `10_RolMovil.sql` | **Rol del celular en el login** (`sp_go_TC004_appMovil` @20): agrega la columna `rol` (1 preventista, 2 distribuidor) para el inicio de la app 3.4; la app vieja la ignora. Requiere el 08. Después reiniciar `WebServicesDynasys` |
| 11 | `11_NoDesasignarDespachado.sql` | **No se desasigna un pedido que ya está en un despacho automático de una conciliación abierta** (`MAM_Ventas` @12 Quitar asignación, @13 Anular, @-1 Eliminar): avisa los pedidos y su despacho; para hacerlo hay que **eliminar primero el despacho**. Vale para web, escritorio y cualquier programa que use el SP. Requiere el 04. No hace falta reiniciar nada. |
| 12 | `12_DespachoSeguro.sql` | **Despachos sin datos sueltos** (`MAM_DespachoProductos`): no se elimina un despacho con pedidos **entregados** ni de conciliación cerrada; si era el **último** de su conciliación, **la conciliación se elimina con él** (si ya tiene entregas, ventas directas, detalle o cierre de caja, no se permite). Al grabar uno automático, un pedido no entra a dos despachos ni al de otro personal. Al modificar: no en conciliación cerrada, no el automático, no cambiar el personal. No depende de los anteriores. Va con la web nueva (mensaje de conciliación eliminada) |
| 13 | `13_EntregaSinCambios.sql` | **Entregar pedidos de otro día sin error** (`sp_go_TC004_appMovil` @29): al entregar, la app reenvía el detalle completo aunque no lo cambie y el @29 lo rechazaba con *"No se puede modificar pedido de otra fecha diferente a la actual"* (el celular lo reintentaba sin fin: quedaba *por subir*). Ahora, si el detalle es igual al de la venta (productos, cantidades y precios), no toca nada y responde OK. Un cambio real sigue validando fecha y stock como antes. Requiere el 10. No hace falta actualizar la app ni reiniciar la API |
| 14 | `14_EntregadosAlVolver.sql` | **"Entregados hoy" no se pierde al cerrar sesión en el celular** (`sp_go_TC004_appMovil` @24): la lista de pedidos que baja el celular trae ahora `fechaHoraEntrega` (hora del celular de la última entrega/anulación, de `VentasRegistroMovil` del 07). Antes, al salir y volver a entrar, los entregados del día llegaban sin esa fecha y desaparecían de *Entregados hoy* y *Cobrado hoy*. Requiere el 13. No hace falta actualizar la app (3.2+) ni reiniciar la API |

Cada script avisa al final con un mensaje: `INSTALADO`, `YA ESTA INSTALADO` (no hace nada) o, si el SP de esa BD es
**otra versión**, da error y **no modifica nada**. Los 03, 04 y 05 se generaron desde `DistribucionDistralKCP2023`
(2026-11-15): sirven para cualquier BD de Distral con esos mismos SP (la protección lo comprueba).

Los 06, 07 y 08 (2026-11-16) se probaron en una copia fresca de `DistribucionDistralKCP2023`: instalan en orden, al repetirlos
avisan `YA ESTA INSTALADO`, y los `REVERTIR` dejan el SP idéntico al original. Llevan BOM UTF-8 (los acentos de los
comentarios del SP se conservan; sin BOM, `sqlcmd` los cambia y la protección del script siguiente lo rechaza).

**06, 07, 08 y 09 van junto con la API (`WebServicesDynasys`) y la app nueva (3.2).** La app vieja (`/apiV3`) sigue funcionando igual.

Antes del 04, revisar si ya hay stock negativo (esos productos no se podrán vender hasta corregirlos con un ingreso):

```sql
SELECT s.ProductoId, p.DescripcionProducto, s.DepositoId, s.Cantidad
FROM ProductosStock s INNER JOIN Productos p ON p.Id = s.ProductoId
WHERE s.Cantidad < 0
```

### Qué cambia para los usuarios (04 y 05)

- Un pedido (web, escritorio o celular) con más cantidad que el stock **no se graba** y se avisa:
  `STOCK INSUFICIENTE: Producto (stock actual X, necesita Y)`. Quedar en 0 sí se permite.
- Si dos preventistas piden el mismo producto al mismo tiempo, el segundo espera un instante y ve el stock real.
- Un pedido del celular que ya llegó (mismo código de sincronización) se devuelve igual, sin volver a validar.
- La app del celular recibe el mensaje como error al sincronizar: el pedido queda en el celular hasta corregir la cantidad.
- Eliminar un ingreso de inventario o una compra cuya mercadería ya se vendió **no se permite** (la web muestra la tabla de productos).
- Compras se graban sin límite (son ingreso).

### Qué cambia para los usuarios (06, 07 y 08, con la app nueva 3.1)

- El celular baja el incremento del día al abrir *Nuevo Pedido*, al sincronizar y cada ~30 s; si la web lo cambia, los pedidos
  nuevos usan el nuevo. Los ya grabados conservan el suyo.
- Para **grabar un pedido / venta directa** y para **entregar o anular** la app exige la ubicación (lectura de menos de 2 min;
  si no hay, espera hasta 25 s y si el GPS está apagado pide activarlo). Se guarda en el celular con la hora: llega aunque se
  sincronice más tarde sin internet.
- La web, en Distribución → *Ubicación Pedidos y Entregas*: tabla por fecha/personal y mapa con el punto del pedido y el de la
  entrega, distancia entre ambos, modelo del celular y si llegó tarde (sin internet).

### Qué cambia para los usuarios (09, con la app 3.2)

- La sincronización ya no se traba con un pedido: sube hasta 20 por ciclo en el orden en que se grabaron; el que falla espera
  su turno (1, 2, 5, 10 y luego cada 15 min) y los demás siguen. Las **entregas y cobranzas ya no esperan** a los pedidos nuevos.
  Al volver internet sincroniza enseguida.
- La API distingue el error pasajero (503: reintenta), el definitivo (code 6: el pedido queda *observado* con el motivo) y el
  desconocido (500: reintenta sin trabar). A la 5ª falla seguida el celular avisa a la web y lo marca *NO SE PUDO ENVIAR*.
- La web avisa al administrador (barra superior) cuando hay pedidos observados. En Distribución → *Pedidos Observados Celular*
  se corrigen las cantidades y se graba (mismo SP que el celular, con su GPS e incremento) o se descarta con un motivo.
- Lo que corrige o descarta la web **llega solo al celular** en la siguiente sincronización (toma el número de venta y el detalle
  grabado, o lo marca *Descartado en web*). Si el celular lo corrige, la web lo ve resuelto solo. Nunca se duplica: el primero que
  graba gana (`Ventas.IdSincronizacion`).
- Venta directa sin internet: la sincronización ahora manda `VentaDirecta` (antes llegaba como pedido normal).

### Qué cambia para los usuarios (11)

- En *Asignación de pedidos* (web o escritorio), **Quitar** o **Anular** un pedido que ya entró a un despacho automático
  de una conciliación **abierta** no hace nada y avisa: `NO SE PUEDE QUITAR LA ASIGNACION: los pedidos #48389 (despacho 1241) ... Elimine primero ese despacho.`
  Lo mismo al **eliminar** el pedido. Si en la lista hay uno despachado, no se toca ninguno de los elegidos.
- Motivo: al desasignarlo quedaba en el camión del repartidor; el celular lo seguía bajando como suyo y lo marcaba
  *por subir*, y la web lo mostraba sin asignar.
- Para corregir un pedido despachado: eliminar el despacho, hacer el cambio y volver a despachar.
- El repartidor puede seguir anulando desde el celular (no pasa por estos @tipo).
- Un pedido que volvió sin entregar de una conciliación **ya cerrada** se desasigna / reasigna normalmente.

### Qué cambia para los usuarios (12)

- **Eliminar despacho** (web o escritorio):
  - Conciliación cerrada: no se permite (la web ya lo bloqueaba; ahora también el escritorio).
  - Algún pedido del despacho **ya entregado**: no se permite y lista los pedidos (`#48352, #48353`). Revertir antes la entrega.
  - Era el **último** despacho de la conciliación: se elimina también la conciliación (antes quedaba abierta y vacía).
    Los pedidos siguen asignados al mismo personal, listos para otro despacho. La web lo dice en el mensaje.
    Si esa conciliación ya tiene entregas, ventas directas, detalle o cierre de caja: no se permite (cerrarla desde Conciliación).
  - Borra también la copia del detalle de ventas del despacho (`VentasDetallesDespacho`), que antes quedaba suelta y,
    como el número del despacho es el último + 1, se pegaba al siguiente despacho.
  - Todo en una transacción: o se borra todo o nada.
- **Nuevo despacho automático**: si un pedido ya está en otro despacho abierto o ya no está asignado a ese personal
  (dos usuarios a la vez, pantalla vieja), no se graba y avisa.
- **Modificar despacho**: no en conciliación cerrada, no el automático (eliminar y crear), no cambiar el personal
  (la conciliación es de esa persona: eliminar y crear para el nuevo).
- El repartidor cuya conciliación se eliminó debe **cerrar sesión y volver a entrar** en el celular (guarda el número al ingresar).

Los 11 y 12 se probaron juntos en `DistralPruebaSanbit` dentro de una transacción con ROLLBACK (11 casos: bloqueos, despacho
manual en la misma conciliación, eliminar el último y que se vaya la conciliación, quitar después, pedido de conciliación
cerrada). Los `REVERTIR` dejan los dos SP idénticos.

## Si algo sale mal: carpeta `revertir\` (NO es parte de la instalación)

Solo para deshacer, **en orden inverso** (primero el número más alto):
`REVERTIR_14_EntregadosAlVolver.sql` → `REVERTIR_13_EntregaSinCambios.sql` → `REVERTIR_12_DespachoSeguro.sql` → `REVERTIR_11_NoDesasignarDespachado.sql` → `REVERTIR_10_RolMovil.sql` → `REVERTIR_09_PedidosObservados.sql` (borra solo el SP; conserva la tabla y el programa) → `REVERTIR_08_PedidoConDetalle.sql` → `REVERTIR_07_UbicacionMovil.sql` → `REVERTIR_06_IncrementoMovil.sql` →
`REVERTIR_05_CompraStock.sql` → `REVERTIR_04_StockSeguro.sql` → `REVERTIR_03_sp_Mam_Precios_tipo5.sql` →
`REVERTIR_02_MAM_PanelControl.sql` → `REVERTIR_01_IX_Indices.sql`. Dejan los SP exactamente como estaban.

## Generadores (`generar\`)

Si la BD de otro servidor tiene otra versión de algún SP, el script correspondiente no se aplica. En ese caso se
regenera desde esa BD (con `server\.env` apuntando a ella):

```
node sql/generar/precios_tipo5.js      -> 03
node sql/generar/stockseguro.js        -> 04
node sql/generar/compra_stock.js       -> 05
node sql/generar/incremento_movil.js   -> 06   (con 01..05 instalados)
node sql/generar/ubicacion_movil.js    -> 07   (con 06 instalado)
node sql/generar/pedido_detalle.js     -> 08   (con 07 instalado)
node sql/generar/no_desasignar.js      -> 11   (con 04 instalado)
node sql/generar/despacho_seguro.js    -> 12
node sql/generar/entrega_sin_cambios.js -> 13  (con 10 instalado)
node sql/generar/entregado_al_volver.js -> 14  (con 13 instalado)
```

Salen en `sql\generados\<BD>\` (con su `revertir\`). La BD debe estar SIN esos cambios al generar.
