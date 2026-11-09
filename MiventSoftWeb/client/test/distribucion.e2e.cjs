// Distribucion (simulador): Despachos y Conciliaciones + responsivo (1440 / 390)
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';

(async () => {
  const b = await chromium.launch();
  let fallos = 0; const errs = [];
  const ok = (c, m) => { if (!c) { fallos++; console.log('  FALLA:', m); } else console.log('  ok:', m); };
  const espera = async (p, t) => { try { await p.locator('.toast', { hasText: t }).last().waitFor({ timeout: 7000 }); return true; } catch { console.log('   toasts:', await p.locator('.toast').allInnerTexts()); return false; } };
  const sinX = (p) => p.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth);
  const login = async (p) => {
    await p.goto(`${BASE}/login`); await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter');
    await p.waitForSelector('.cabecera', { timeout: 15000 });
  };
  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);

  console.log('DESPACHOS');
  await p.goto(`${BASE}/distribucion/despachos`); await p.waitForSelector('.grid tbody tr');
  ok(await p.locator('.grid tbody tr').count() >= 3, 'lista de despachos');
  await p.goto(`${BASE}/distribucion/despachos/nuevo`); await p.waitForSelector('.venta-detalle');
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'Requeridos'), 'valida personal y detalle');
  await p.locator('button', { hasText: 'Buscar' }).first().click();
  await p.locator('.modal tr', { hasText: 'CARLOS VARGAS' }).first().dblclick().catch(async () => { await p.locator('.modal tr', { hasText: 'CARLOS VARGAS' }).first().click(); await p.keyboard.press('Enter'); });
  await p.waitForTimeout(300);
  ok((await p.locator('.control-elegir').first().inputValue()).toUpperCase().includes('CARLOS'), 'elige personal');
  await p.locator('button', { hasText: 'Agregar producto' }).click();
  await p.waitForSelector('.selector-productos tbody tr.fila-clic');
  await p.locator('.selector-productos tbody tr.fila-clic:not(.sin-stock)').first().click();
  await p.locator('.cantidad-cajas input').first().fill('2');
  await p.locator('.cantidad-cajas .btn-pri').click();
  await p.locator('.selector-productos .modal-acciones .btn-pri').click();
  ok(await p.locator('.tabla-detalle tbody tr.fila-nueva').count() === 1, 'producto agregado');
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'GRABADO CON ÉXITO'), 'graba despacho');
  // como la nota de venta: al grabar sale el comprobante para imprimir (Reporte_Despacho)
  await p.waitForSelector('.nota-venta .nota-grabada', { timeout: 8000 }).catch(() => {});
  ok(/Despacho #\d+ grabado/.test(await p.locator('.nota-grabada').innerText().catch(() => '')), 'al grabar sale el comprobante de despacho');
  const hoja = await p.locator('.nota-papel').innerText().catch(() => '');
  ok(/Comprobante de Despacho de Productos/i.test(hoja) && /Distribuidor:\s*Carlos Vargas/i.test(hoja) && /Total General:/.test(hoja), 'comprobante con distribuidor, productos y total general');
  ok(/Encargado Almacen/.test(hoja) && !/Son:/.test(hoja), 'firmas Encargado Almacen / Distribuidor, sin literal de dinero');
  await p.locator('.nota-papel').screenshot({ path: `${OUT}/desp_comprobante.png` });
  await p.locator('.nota-venta .btn-sec', { hasText: 'No imprimir' }).click();
  await p.waitForURL(/despachos$/);
  await p.locator('.grid tbody tr').first().locator('button[title="Nota de despacho"]').click();
  await p.waitForSelector('.nota-papel', { timeout: 8000 }).catch(() => {});
  ok(await p.locator('.nota-papel').count() === 1 && await p.locator('.nota-grabada').count() === 0, 'reimprimir desde la lista');
  await p.keyboard.press('Escape');
  await p.screenshot({ path: `${OUT}/desp_1440.png` });

  console.log('AUTOMATICO');
  await p.goto(`${BASE}/distribucion/despachos/nuevo`); await p.waitForSelector('.venta-detalle');
  await p.locator('button', { hasText: 'Cargar productos de ventas' }).click();
  ok(await espera(p, 'SELECCIONE UN PERSONAL'), 'exige personal');
  await p.locator('button', { hasText: 'Buscar' }).first().click();
  await p.locator('.modal tr', { hasText: 'CARLOS VARGAS' }).first().dblclick();
  await p.locator('button', { hasText: 'Cargar productos de ventas' }).click();
  await p.waitForTimeout(400);
  ok(await p.locator('.tabla-detalle tbody tr.fila-nueva').count() === 2 && await p.locator('.chip-tipo', { hasText: 'Automático' }).count() === 1, 'carga productos de ventas');
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'GRABADO CON ÉXITO'), 'graba automatico');

  console.log('CONCILIACIONES');
  await p.goto(`${BASE}/distribucion/conciliaciones`); await p.waitForSelector('.grid tbody tr');
  ok(await p.locator('.modelo-cab .btn-pri', { hasText: 'Nuevo' }).count() === 0, 'sin boton Nuevo (como el escritorio)');
  await p.locator('.grid tbody tr', { hasText: 'ABIERTA' }).first().click();
  await p.locator('.grid tbody tr', { hasText: 'ABIERTA' }).first().dblclick().catch(() => {});
  await p.waitForSelector('.tabla-conc tbody tr td.num', { timeout: 8000 });
  ok(await p.locator('.tabla-conc thead th', { hasText: 'Sal. #' }).count() >= 2, 'una columna por salida');
  const col = (sel) => p.locator(`.tabla-conc tbody tr:first-child td.${sel}`).evaluate((td) => getComputedStyle(td).backgroundColor);
  ok(/184, 245, 176/.test(await col('cc-sal')) && /253, 230, 138/.test(await col('cc-dev')) && /184, 245, 176/.test(await col('cc-ent')), 'columnas con color: salidas y entregado verde, devoluciones amarillo');
  await p.screenshot({ path: `${OUT}/conc_1440.png` });
  await p.locator('button', { hasText: 'Comprobante' }).first().click();
  await p.waitForSelector('.nota-papel', { timeout: 8000 }).catch(() => {});
  const hc = await p.locator('.nota-papel').innerText().catch(() => '');
  ok(/Comprobante de Conciliacion de Productos/i.test(hc) && /Nro Conciliación:/.test(hc) && /Salida #01/i.test(hc) && /Encargado Almacen/.test(hc), 'comprobante de conciliacion como el escritorio');
  ok(/28, 47, 143/.test(await p.locator('.nota-papel .nota-tabla thead th').first().evaluate((th) => getComputedStyle(th).backgroundColor)), 'cabecera de la tabla en azul');
  await p.locator('.nota-papel').screenshot({ path: `${OUT}/conc_comprobante.png` });
  await p.keyboard.press('Escape'); await p.waitForTimeout(200);
  await p.locator('button', { hasText: 'Modificar' }).first().click();
  const celda = p.locator('.tabla-conc tbody tr').first().locator('input.control-celda');
  await celda.fill('1'); await celda.press('Enter');
  ok((await p.locator('.tabla-conc tbody tr').first().locator('td').last().innerText()).trim() !== '0' && await p.locator('.tabla-conc tr.cc-con-dif').count() >= 1, 'calcula diferencia y marca la fila');
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'MODIFICADO CON ÉXITO'), 'guarda conciliacion');
  await p.goto(`${BASE}/distribucion/conciliaciones`); await p.waitForSelector('.grid tbody tr');
  await p.locator('.grid tbody tr', { hasText: '#7' }).first().locator('button[title="Editar"]').click();
  ok(await espera(p, 'CIERRE DE CAJA'), 'bloquea si tiene cierre de caja');
  await p.close();

  console.log('RESPONSIVO');
  for (const [w, h] of [[390, 844], [1024, 768]]) {
    const r = await b.newPage({ viewport: { width: w, height: h } });
    r.on('pageerror', (e) => errs.push(`${w}: ${e.message}`));
    await login(r);
    for (const t of ['despachos', 'despachos/nuevo', 'conciliaciones']) {
      await r.goto(`${BASE}/distribucion/${t}`); await r.waitForTimeout(800);
      ok(await sinX(r), `${w} ${t} sin scroll horizontal`);
      await r.screenshot({ path: `${OUT}/dist_${t.replace('/', '_')}_${w}.png` });
    }
    await r.close();
  }
  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs);
  await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
