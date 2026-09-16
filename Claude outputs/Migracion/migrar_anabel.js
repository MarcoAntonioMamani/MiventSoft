/*
 * migrar_anabel.js
 * ----------------------------------------------------------------------
 * Migra el Excel de productos (columnas CODIGO, DESCRIPCION, GRUPO, MARCA,
 * VENTA, COSTO) a la base de datos de MiventSoft, en el orden que pediste:
 *
 *   1) Categorias      (GRUPO del Excel -> tabla Categorias)
 *   2) Clasificadores  (MARCA del Excel -> ClasificadorDetalle, IdClasificador=3)
 *   3) Productos       (usa los Id de Categoria/Marca ya resueltos)
 *   4) Precios         (2 filas por producto: Costo y Venta, tabla Precios)
 *   5) ProductosCodigoBarras (solo cuando el producto SI trae codigo y es
 *      el primero que lo usa)
 *
 * NO llama al SP MAM_Productos: ese SP esta pensado para el formulario
 * (recibe TVPs de imagenes/precios/codigos de barra armados por la grilla
 * de Tec_Productos.vb) y no para carga masiva. Se decidio con el usuario
 * insertar directo en las tablas (ver conversacion). El esquema de tablas
 * fue verificado contra bd_schema_utf8_v2.sql real del proyecto, no
 * inventado.
 *
 * REGLAS DE NEGOCIO ACORDADAS (no cambiar sin volver a confirmar):
 *   - Si el producto no trae codigo de barra, o su codigo ya fue usado por
 *     otra fila (del Excel o YA EXISTENTE en la BD), el producto igual se
 *     crea, pero SIN codigo de barra (no se inserta en ProductosCodigoBarras
 *     ni se llena Productos.CodigoBarras). Se reporta en el CSV de conflictos.
 *   - Si falta el precio de Costo o el de Venta, igual se inserta la fila en
 *     Precios, con 0.
 *   - Si el producto no trae GRUPO (categoria), se usa una categoria por
 *     defecto compartida "SIN CATEGORIA" (se crea una sola vez y se reusa).
 *   - Si el producto no trae MARCA, se usa una marca por defecto compartida
 *     "SIN MARCA" (mismo criterio, por consistencia con lo anterior).
 *   - EmpresaId, ProveedorId, AttributoId, FamiliaId, UnidadVentaId,
 *     UnidadMaximaId quedan NULL (se completan despues a mano si hace falta).
 *   - Conversion=0, StockMinimo=0, Estado=1 (activo).
 *   - Reintentable: si ya existe un producto con el mismo NOMBRE y la misma
 *     MARCA, no se vuelve a crear (se reporta como OMITIDO). No se usa el
 *     codigo de barra para esta decision -- este Excel tiene productos
 *     DISTINTOS que comparten texto de descripcion pero son de otra marca
 *     (ej. "PAPEL HIGIENICO UNIDAD" existe en 3 marcas distintas: son 3
 *     productos reales, deben crearse los 3). Esto permite correr el
 *     script mas de una vez en dev/test sin duplicar todo de nuevo.
 *
 * IMPORTANTE (seguridad / operacion):
 *   - La cadena de conexion sale de variables de entorno (.env), nunca
 *     hardcodeada en este archivo. Copia .env.example a .env y completa los
 *     datos de tu servidor de DESARROLLO/PRUEBAS primero.
 *   - NO corras esto contra produccion sin haberlo probado antes en
 *     dev/test y revisado los reportes CSV que genera.
 *   - Los Id de Categorias y Productos NO son IDENTITY en tu esquema (se
 *     calculan como MAX(Id)+1, igual que ya lo hace la app). Este script
 *     asume que es el UNICO proceso escribiendo en esas tablas mientras
 *     corre: no lo corras con el sistema en uso simultaneo (corre fuera de
 *     horario / con la app cerrada) para evitar colisiones de Id.
 *
 * Uso:
 *   npm install
 *   cp .env.example .env   (completar con datos de dev/test)
 *   npm run migrar
 */

'use strict';

require('dotenv').config();
const fs = require('fs');
const path = require('path');
const sql = require('mssql');
const XLSX = require('xlsx');

// ---------------------------------------------------------------------
// Configuracion (todo desde .env, nada hardcodeado)
// ---------------------------------------------------------------------
const CONFIG = {
  server: requireEnv('DB_SERVER'),
  port: parseInt(process.env.DB_PORT || '1433', 10),
  database: requireEnv('DB_NAME'),
  user: requireEnv('DB_USER'),
  password: requireEnv('DB_PASSWORD'),
  encrypt: (process.env.DB_ENCRYPT || 'false').toLowerCase() === 'true',
  trustServerCertificate: (process.env.DB_TRUST_SERVER_CERT || 'true').toLowerCase() === 'true',
};

const EXCEL_PATH = process.env.EXCEL_PATH || './ANABEL.xlsx';
const EXCEL_SHEET = process.env.EXCEL_SHEET || 'Hoja1';
const USUARIO_MIGRACION = process.env.USUARIO_MIGRACION || 'MIGRACION_ANABEL';
const ALMACEN_ID = process.env.ALMACEN_ID ? parseInt(process.env.ALMACEN_ID, 10) : null;
const CATEGORIA_PRECIO_COSTO_ID_ENV = process.env.CATEGORIA_PRECIO_COSTO_ID
  ? parseInt(process.env.CATEGORIA_PRECIO_COSTO_ID, 10)
  : null;
const CATEGORIA_PRECIO_VENTA_ID_ENV = process.env.CATEGORIA_PRECIO_VENTA_ID
  ? parseInt(process.env.CATEGORIA_PRECIO_VENTA_ID, 10)
  : null;

const NOMBRE_CATEGORIA_DEFECTO = 'SIN CATEGORIA';
const NOMBRE_MARCA_DEFECTO = 'SIN MARCA';
const ID_CLASIFICADOR_MARCA = 3; // confirmado en Tec_Productos.vb: L_prLibreriaDetalleGeneral(3)

function requireEnv(name) {
  const v = process.env[name];
  if (v === undefined || v === '') {
    console.error(`Falta la variable de entorno ${name}. Copia .env.example a .env y completala.`);
    process.exit(1);
  }
  return v;
}

// ---------------------------------------------------------------------
// Utilidades
// ---------------------------------------------------------------------

// Mismo formato que usa el resto del sistema (CONCAT(DATEPART(HOUR,...),':',DATEPART(MINUTE,...)))
// -> sin ceros a la izquierda, ej. "9:5". Se replica tal cual para no
// introducir un formato distinto al del resto de la BD.
function horaActualComoLaApp() {
  const d = new Date();
  return `${d.getHours()}:${d.getMinutes()}`;
}

function normalizarTexto(v) {
  if (v === null || v === undefined) return '';
  return String(v).trim();
}

function claveComparacion(v) {
  return normalizarTexto(v).toUpperCase();
}

// El Excel puede traer el codigo de barra como numero (la mayoria) o como
// texto (24 filas, codigos internos tipo "esc'3598'000180"). Siempre lo
// dejamos como string, sin notacion cientifica ni decimales.
function normalizarCodigoBarra(v) {
  if (v === null || v === undefined) return '';
  if (typeof v === 'number') {
    return String(Math.trunc(v));
  }
  return String(v).trim();
}

function parsearPrecio(v) {
  if (v === null || v === undefined || v === '') return 0;
  const n = typeof v === 'number' ? v : parseFloat(String(v).replace(',', '.'));
  return Number.isFinite(n) ? n : 0;
}

// ---------------------------------------------------------------------
// Lectura del Excel
// ---------------------------------------------------------------------
function leerFilasExcel() {
  const wb = XLSX.readFile(EXCEL_PATH, { cellDates: false });
  const hoja = wb.Sheets[EXCEL_SHEET];
  if (!hoja) {
    throw new Error(`No se encontro la hoja "${EXCEL_SHEET}" en ${EXCEL_PATH}. Hojas disponibles: ${wb.SheetNames.join(', ')}`);
  }
  // header:1 -> arreglos por fila, sin asumir nombres de columna exactos
  const filas = XLSX.utils.sheet_to_json(hoja, { header: 1, blankrows: false, defval: null });

  const datos = [];
  // fila 0 = encabezado (CODIGO, DESCRIPCION, GRUPO, MARCA, VENTA, COSTO)
  for (let i = 1; i < filas.length; i++) {
    const [codigo, descripcion, grupo, marca, venta, costo] = filas[i];
    const nroFilaExcel = i + 1; // +1 porque Excel es 1-based y ya saltamos el header
    const desc = normalizarTexto(descripcion);
    if (!desc) {
      // Fila sin descripcion: no hay nada que crear como producto.
      continue;
    }
    datos.push({
      nroFilaExcel,
      codigoBarraOriginal: codigo,
      codigoBarra: normalizarCodigoBarra(codigo),
      descripcion: desc,
      grupo: normalizarTexto(grupo),
      marca: normalizarTexto(marca),
      venta: parsearPrecio(venta),
      costo: parsearPrecio(costo),
    });
  }
  return datos;
}

// ---------------------------------------------------------------------
// Resolucion de PreciosCategorias (Costo=Tipo 0, Venta=Tipo 1)
// ---------------------------------------------------------------------
async function resolverCategoriasPrecio(pool) {
  if (CATEGORIA_PRECIO_COSTO_ID_ENV && CATEGORIA_PRECIO_VENTA_ID_ENV) {
    return { costoId: CATEGORIA_PRECIO_COSTO_ID_ENV, ventaId: CATEGORIA_PRECIO_VENTA_ID_ENV };
  }

  const result = await pool.request().query(
    'SELECT Id, Tipo, Descripcion FROM PreciosCategorias WHERE Tipo IN (0, 1)'
  );
  const costos = result.recordset.filter((r) => r.Tipo === 0);
  const ventas = result.recordset.filter((r) => r.Tipo === 1);

  if (costos.length !== 1 || ventas.length !== 1) {
    console.error('No pude resolver automaticamente la categoria de precio de Costo/Venta.');
    console.error('Filas de PreciosCategorias con Tipo=0 (costo):', costos);
    console.error('Filas de PreciosCategorias con Tipo=1 (venta):', ventas);
    console.error('Fija CATEGORIA_PRECIO_COSTO_ID y CATEGORIA_PRECIO_VENTA_ID en tu .env con el Id correcto y volve a correr.');
    process.exit(1);
  }

  return { costoId: costos[0].Id, ventaId: ventas[0].Id };
}

// ---------------------------------------------------------------------
// Categorias (GRUPO)
// ---------------------------------------------------------------------
async function cargarCategoriasExistentes(pool) {
  const result = await pool.request().query('SELECT Id, NombreCategoria FROM Categorias');
  const mapa = new Map();
  let maxId = 0;
  for (const row of result.recordset) {
    mapa.set(claveComparacion(row.NombreCategoria), row.Id);
    if (row.Id > maxId) maxId = row.Id;
  }
  return { mapa, siguienteId: maxId + 1 };
}

// Nota: se inserta fuera de la transaccion del producto a proposito -- una
// categoria/marca nueva debe quedar guardada aunque el producto que la
// disparo falle despues (asi la siguiente fila que use la misma categoria
// no la vuelve a intentar crear).
async function crearCategoria(pool, siguienteIdRef, nombre) {
  const id = siguienteIdRef.valor++;
  const request = new sql.Request(pool);
  await request
    .input('Id', sql.Int, id)
    .input('NombreCategoria', sql.NVarChar(150), nombre)
    .input('Estado', sql.Int, 1)
    .input('FechaRegistro', sql.Date, new Date())
    .input('HoraRegistro', sql.NVarChar(15), horaActualComoLaApp())
    .input('UsuarioRegistro', sql.NVarChar(50), USUARIO_MIGRACION)
    .query(`INSERT INTO Categorias (Id, NombreCategoria, Estado, FechaRegistro, HoraRegistro, UsuarioRegistro)
            VALUES (@Id, @NombreCategoria, @Estado, @FechaRegistro, @HoraRegistro, @UsuarioRegistro)`);
  return id;
}

// ---------------------------------------------------------------------
// Marcas (Clasificadores / ClasificadorDetalle, IdClasificador=3)
// ---------------------------------------------------------------------
async function cargarMarcasExistentes(pool) {
  const result = await pool
    .request()
    .input('IdClasificador', sql.Int, ID_CLASIFICADOR_MARCA)
    .query('SELECT Id, Descripcion FROM ClasificadorDetalle WHERE IdClasificador = @IdClasificador');
  const mapa = new Map();
  for (const row of result.recordset) {
    mapa.set(claveComparacion(row.Descripcion), row.Id);
  }
  return mapa;
}

async function crearMarca(pool, nombre) {
  const request = new sql.Request(pool);
  const result = await request
    .input('IdClasificador', sql.Int, ID_CLASIFICADOR_MARCA)
    .input('Descripcion', sql.NVarChar(200), nombre)
    .query(`INSERT INTO ClasificadorDetalle (IdClasificador, Descripcion)
            OUTPUT INSERTED.Id
            VALUES (@IdClasificador, @Descripcion)`);
  return result.recordset[0].Id;
}

// ---------------------------------------------------------------------
// Codigos de barra ya usados (en la BD, para no chocar con productos que
// ya existen) -- ademas de los que se van marcando fila a fila del Excel.
// ---------------------------------------------------------------------
async function cargarCodigosBarraExistentes(pool) {
  const result = await pool.request().query(`
    SELECT CodigoBarras FROM ProductosCodigoBarras WHERE CodigoBarras IS NOT NULL
    UNION
    SELECT CodigoBarras FROM Productos WHERE CodigoBarras IS NOT NULL
  `);
  const set = new Set();
  for (const row of result.recordset) {
    const c = normalizarTexto(row.CodigoBarras);
    if (c) set.add(c);
  }
  return set;
}

// Para el "reintentable": clave = NombreProducto + Marca. NO alcanza con el
// nombre solo -- este Excel tiene productos DISTINTOS con el mismo texto de
// descripcion (ej. "PAPEL HIGIENICO UNIDAD" existe con marca SUPER JUNIOR,
// NACIONAL BALANCE y HOGAR LILA: son 3 productos reales, no 1 duplicado).
// Con nombre+marca esos 3 se crean igual, y solo se omite una fila cuando
// de verdad se repite nombre Y marca (esos si son duplicados genuinos del
// archivo, ej. "GELATINA AROMAMR SABOR MARACUYA 250G" / AROMAMR dos veces).
async function cargarProductosExistentesPorNombreYMarca(pool) {
  const result = await pool.request().query(`
    SELECT p.NombreProducto, cd.Descripcion AS Marca
    FROM Productos AS p
    LEFT JOIN ClasificadorDetalle AS cd ON cd.Id = p.MarcaId
    WHERE p.NombreProducto IS NOT NULL
  `);
  const set = new Set();
  for (const row of result.recordset) {
    set.add(claveComparacion(row.NombreProducto) + '|' + claveComparacion(row.Marca));
  }
  return set;
}

async function obtenerSiguienteIdProducto(pool) {
  const result = await pool.request().query('SELECT ISNULL(MAX(Id), 0) AS MaxId FROM Productos');
  return result.recordset[0].MaxId + 1;
}

// ---------------------------------------------------------------------
// Insercion de un producto completo (Productos + Precios x2 + CodigoBarras)
// dentro de una transaccion propia.
// ---------------------------------------------------------------------
async function insertarProductoCompleto(pool, opciones) {
  const {
    id,
    fila,
    categoriaId,
    marcaId,
    categoriaPrecioCostoId,
    categoriaPrecioVentaId,
    asignarCodigoBarra,
  } = opciones;

  const transaction = new sql.Transaction(pool);
  await transaction.begin();
  try {
    const reqProducto = new sql.Request(transaction);
    await reqProducto
      .input('Id', sql.Int, id)
      .input('CodigoBarras', sql.NVarChar(200), asignarCodigoBarra ? fila.codigoBarra : null)
      .input('NombreProducto', sql.NVarChar(150), fila.descripcion)
      .input('StockMinimo', sql.Decimal(18, 2), 0)
      .input('Estado', sql.Int, 1)
      .input('CategoriaId', sql.Int, categoriaId)
      .input('MarcaId', sql.Int, marcaId)
      .input('Conversion', sql.Decimal(18, 2), 0)
      .input('FechaRegistro', sql.Date, new Date())
      .input('HoraRegistro', sql.NVarChar(15), horaActualComoLaApp())
      .input('UsuarioRegistro', sql.NVarChar(50), USUARIO_MIGRACION)
      .query(`INSERT INTO Productos
                (Id, CodigoBarras, NombreProducto, StockMinimo, Estado, CategoriaId, MarcaId,
                 Conversion, FechaRegistro, HoraRegistro, UsuarioRegistro)
              VALUES
                (@Id, @CodigoBarras, @NombreProducto, @StockMinimo, @Estado, @CategoriaId, @MarcaId,
                 @Conversion, @FechaRegistro, @HoraRegistro, @UsuarioRegistro)`);

    // Precios: siempre se insertan las 2 filas (Costo y Venta), 0 si faltan.
    for (const { categoriaPrecioId, monto } of [
      { categoriaPrecioId: categoriaPrecioCostoId, monto: fila.costo },
      { categoriaPrecioId: categoriaPrecioVentaId, monto: fila.venta },
    ]) {
      const reqPrecio = new sql.Request(transaction);
      await reqPrecio
        .input('AlmacenId', sql.Int, ALMACEN_ID)
        .input('ProductoId', sql.Int, id)
        .input('PrecioCategoriaId', sql.Int, categoriaPrecioId)
        .input('Precio', sql.Decimal(18, 2), monto)
        .input('FechaRegistro', sql.Date, new Date())
        .input('HoraRegistro', sql.NVarChar(15), horaActualComoLaApp())
        .input('UsuarioRegistro', sql.NVarChar(50), USUARIO_MIGRACION)
        .query(`INSERT INTO Precios (AlmacenId, ProductoId, PrecioCategoriaId, Precio, FechaRegistro, HoraRegistro, UsuarioRegistro)
                VALUES (@AlmacenId, @ProductoId, @PrecioCategoriaId, @Precio, @FechaRegistro, @HoraRegistro, @UsuarioRegistro)`);
    }

    if (asignarCodigoBarra) {
      const reqCodigo = new sql.Request(transaction);
      await reqCodigo
        .input('ProductoId', sql.Int, id)
        .input('CodigoBarras', sql.NVarChar(50), fila.codigoBarra)
        .query('INSERT INTO ProductosCodigoBarras (ProductoId, CodigoBarras) VALUES (@ProductoId, @CodigoBarras)');
    }

    await transaction.commit();
  } catch (err) {
    await transaction.rollback();
    throw err;
  }
}

// ---------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------
async function main() {
  console.log(`Leyendo ${EXCEL_PATH} (hoja "${EXCEL_SHEET}")...`);
  const filas = leerFilasExcel();
  console.log(`  ${filas.length} filas con producto valido (con descripcion).`);

  console.log(`Conectando a ${CONFIG.server}:${CONFIG.port}/${CONFIG.database} como ${CONFIG.user}...`);
  const pool = await sql.connect(CONFIG);

  try {
    const { costoId: categoriaPrecioCostoId, ventaId: categoriaPrecioVentaId } = await resolverCategoriasPrecio(pool);
    console.log(`  PreciosCategorias: Costo=${categoriaPrecioCostoId}  Venta=${categoriaPrecioVentaId}`);
    if (ALMACEN_ID) {
      console.log(`  Precios.AlmacenId fijo = ${ALMACEN_ID}`);
    } else {
      console.log('  Precios.AlmacenId = NULL (no se fijo ALMACEN_ID en .env)');
    }

    const { mapa: categorias, siguienteId: siguienteIdCategoriaInicial } = await cargarCategoriasExistentes(pool);
    const siguienteIdCategoria = { valor: siguienteIdCategoriaInicial };
    const marcas = await cargarMarcasExistentes(pool);
    const codigosBarraUsados = await cargarCodigosBarraExistentes(pool);
    const productosExistentes = await cargarProductosExistentesPorNombreYMarca(pool);
    let siguienteIdProducto = await obtenerSiguienteIdProducto(pool);

    console.log(`  Categorias existentes: ${categorias.size}. Marcas existentes: ${marcas.size}.`);
    console.log(`  Codigos de barra ya usados en la BD: ${codigosBarraUsados.size}.`);
    console.log(`  Productos ya existentes (por nombre+marca): ${productosExistentes.size}.`);
    console.log(`  Siguiente Id de Producto a usar: ${siguienteIdProducto}.`);
    console.log('');

    const reporte = [];
    const conflictosCodigoBarra = [];

    let creados = 0;
    let omitidos = 0;
    let errores = 0;
    let sinCodigoBarra = 0;
    let categoriasCreadas = 0;
    let marcasCreadas = 0;

    for (const fila of filas) {
      try {
        const nombreGrupo = fila.grupo || NOMBRE_CATEGORIA_DEFECTO;
        const nombreMarca = fila.marca || NOMBRE_MARCA_DEFECTO;

        // Reintentable: la clave de "ya migrado" es NOMBRE + MARCA (no solo
        // el nombre: este Excel tiene productos DISTINTOS con el mismo texto
        // de descripcion pero distinta marca -- ver comentario en
        // cargarProductosExistentesPorNombreYMarca). Tampoco se usa el
        // codigo de barra como clave: eso es aparte, mas abajo, y solo
        // decide si el producto se queda CON o SIN codigo, nunca si se crea.
        const claveProducto = claveComparacion(fila.descripcion) + '|' + claveComparacion(nombreMarca);
        if (productosExistentes.has(claveProducto)) {
          omitidos++;
          reporte.push({ ...fila, estado: 'OMITIDO', motivo: 'ya existe un producto con ese nombre y esa marca', productoId: '', categoriaId: '', marcaId: '' });
          continue;
        }

        // Resolver categoria (GRUPO), con default compartido si viene vacio.
        const claveGrupo = claveComparacion(nombreGrupo);
        let categoriaId = categorias.get(claveGrupo);
        if (!categoriaId) {
          categoriaId = await crearCategoria(pool, siguienteIdCategoria, nombreGrupo);
          categorias.set(claveGrupo, categoriaId);
          categoriasCreadas++;
        }

        // Resolver marca (MARCA), con default compartido si viene vacio.
        const claveMarca = claveComparacion(nombreMarca);
        let marcaId = marcas.get(claveMarca);
        if (!marcaId) {
          marcaId = await crearMarca(pool, nombreMarca);
          marcas.set(claveMarca, marcaId);
          marcasCreadas++;
        }

        // El codigo de barra SOLO decide si este producto se queda con
        // codigo o no -- nunca decide si el producto se crea (esa decision
        // ya se tomo arriba, por nombre). "Primer codigo gana": si ya lo
        // uso otro producto (de la BD o de una fila anterior de este mismo
        // Excel), este producto se crea igual, pero sin codigo de barra.
        const asignarCodigoBarra = Boolean(fila.codigoBarra) && !codigosBarraUsados.has(fila.codigoBarra);
        const id = siguienteIdProducto++;

        await insertarProductoCompleto(pool, {
          id,
          fila,
          categoriaId,
          marcaId,
          categoriaPrecioCostoId,
          categoriaPrecioVentaId,
          asignarCodigoBarra,
        });

        if (asignarCodigoBarra) {
          codigosBarraUsados.add(fila.codigoBarra);
        } else {
          sinCodigoBarra++;
          conflictosCodigoBarra.push({
            fila: fila.nroFilaExcel,
            codigo: fila.codigoBarraOriginal,
            descripcion: fila.descripcion,
            motivo: fila.codigoBarra ? 'codigo repetido, ya usado por otro producto' : 'sin codigo de barra en el Excel',
          });
        }
        productosExistentes.add(claveProducto);

        creados++;
        reporte.push({ ...fila, estado: 'OK', motivo: '', productoId: id, categoriaId, marcaId });
      } catch (err) {
        errores++;
        reporte.push({ ...fila, estado: 'ERROR', motivo: err.message, productoId: '', categoriaId: '', marcaId: '' });
        console.error(`Fila ${fila.nroFilaExcel} (${fila.descripcion}): ERROR - ${err.message}`);
      }
    }

    escribirCsvReporte(reporte);
    escribirCsvConflictos(conflictosCodigoBarra);

    console.log('');
    console.log('--- Resumen ---');
    console.log(`Productos creados:        ${creados}`);
    console.log(`  - sin codigo de barra:  ${sinCodigoBarra} (ver reporte_conflictos_codigo_barras.csv)`);
    console.log(`Productos omitidos (ya existian): ${omitidos}`);
    console.log(`Errores:                  ${errores}`);
    console.log(`Categorias nuevas creadas: ${categoriasCreadas}`);
    console.log(`Marcas nuevas creadas:     ${marcasCreadas}`);
    console.log('Reporte completo: reporte_migracion.csv');
  } finally {
    await pool.close();
  }
}

function escribirCsvReporte(filas) {
  const encabezado = 'fila_excel;codigo_excel;codigo_normalizado;descripcion;grupo;marca;venta;costo;estado;motivo;producto_id;categoria_id;marca_id\n';
  const lineas = filas.map((f) =>
    [
      f.nroFilaExcel,
      csvEscape(f.codigoBarraOriginal),
      csvEscape(f.codigoBarra),
      csvEscape(f.descripcion),
      csvEscape(f.grupo),
      csvEscape(f.marca),
      f.venta,
      f.costo,
      f.estado,
      csvEscape(f.motivo),
      f.productoId,
      f.categoriaId,
      f.marcaId,
    ].join(';')
  );
  fs.writeFileSync(path.join(__dirname, 'reporte_migracion.csv'), encabezado + lineas.join('\n'), 'utf8');
}

function escribirCsvConflictos(conflictos) {
  const encabezado = 'fila_excel;codigo;descripcion;motivo\n';
  const lineas = conflictos.map((c) => [c.fila, csvEscape(c.codigo), csvEscape(c.descripcion), csvEscape(c.motivo)].join(';'));
  fs.writeFileSync(path.join(__dirname, 'reporte_conflictos_codigo_barras.csv'), encabezado + lineas.join('\n'), 'utf8');
}

function csvEscape(v) {
  if (v === null || v === undefined) return '';
  const s = String(v);
  if (s.includes(';') || s.includes('"') || s.includes('\n')) {
    return '"' + s.replace(/"/g, '""') + '"';
  }
  return s;
}

main()
  .then(() => process.exit(0))
  .catch((err) => {
    console.error('Fallo la migracion:', err);
    process.exit(1);
  });
