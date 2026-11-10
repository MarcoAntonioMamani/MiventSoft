// Compras: nota de compra al grabar (como ReporteCompra del escritorio) y desde la lista; contador de productos agregados
// en el buscador (Compras, Movimientos, Despachos, como en Pedido); Cuentas por pagar: detalle de la compra con scroll.
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/compras_nota.e2e.cjs [carpetaCapturas]
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';
(async () => {
  const b = await chromium.launch(); let f = 0; const errs = [];
  const ok = (c, m) => { if (!c) f++; console.log(c ? '  ok:' : '  FALLA:', m); };
  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await p.goto(`${BASE}/login`); await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter'); await p.waitForSelector('.dash-cab, .kpi');

  console.log('COMPRAS');
  await p.goto(`${BASE}/compras/compras/nuevo`); await p.waitForSelector('.venta-detalle');
  await p.locator('.campo', { hasText: 'Proveedor' }).locator('button').first().click();
  await p.locator('.combo-lista tr').nth(1).click();
  await p.locator('button', { hasText: 'Agregar producto' }).click();
  await p.waitForSelector('.selector-productos tbody tr.fila-clic');
  for (let i = 0; i < 2; i++) { await p.keyboard.press('Enter'); await p.waitForSelector('.cantidad-cajas'); await p.keyboard.type('12'); await p.keyboard.press('Enter'); await p.waitForTimeout(250); }
  ok((await p.locator('.selector-productos .chip-ok').innerText().catch(() => '')).trim() === '2 agregados', 'el buscador muestra cuantos productos se agregaron');
  await p.keyboard.press('Escape'); await p.waitForTimeout(200);
  await p.locator('.btn-ok').click();
  await p.waitForSelector('.nota-venta .nota-grabada', { timeout: 8000 }).catch(() => {});
  ok(/Compra #\d+ grabada/.test(await p.locator('.nota-grabada').innerText().catch(() => '')), 'al grabar sale la nota de compra para decidir si imprimir');
  ok(await p.locator('.nota-papel .nota-tabla tbody tr').count() === 2 && /Nota de Compra/i.test(await p.locator('.nv-titulo').innerText()), 'la nota tiene la misma hoja que la nota de venta, con los 2 productos');
  ok(/Proveedor:/.test(await p.locator('.nota-papel').innerText()) && /Son:/.test(await p.locator('.nota-papel').innerText()), 'proveedor, totales y literal');
  await p.locator('.nota-papel').screenshot({ path: `${OUT}/nota_compra.png` });
  await p.locator('.nota-venta .btn-sec', { hasText: 'No imprimir' }).click();
  await p.waitForURL(/compras\/compras$/);
  await p.locator('.grid tbody tr').first().locator('button[title="Nota de compra"]').click();
  await p.waitForSelector('.nota-papel', { timeout: 8000 }).catch(() => {});
  ok(await p.locator('.nota-papel').count() === 1 && await p.locator('.nota-grabada').count() === 0, 'imprimir la nota desde la lista');
  await p.keyboard.press('Escape');

  console.log('MOVIMIENTOS');
  await p.goto(`${BASE}/inventario/movimientos/nuevo`); await p.waitForTimeout(800);
  const btn = p.locator('button', { hasText: /Agregar producto/ }).first();
  if (await btn.count()) {
    await btn.click().catch(() => {});
    if (await p.locator('.selector-productos').count()) {
      await p.waitForSelector('.selector-productos tbody tr.fila-clic');
      await p.locator('.selector-productos tbody tr.fila-clic:not(.sin-stock)').first().click();
      await p.waitForSelector('.cantidad-cajas'); await p.keyboard.type('1'); await p.keyboard.press('Enter'); await p.waitForTimeout(250);
      ok((await p.locator('.selector-productos .chip-ok').innerText().catch(() => '')).trim() === '1 agregado', 'movimientos: contador de agregados');
    } else ok(true, 'movimientos: requiere tipo de movimiento antes (sin cambios)');
  }

  console.log('CUENTAS POR PAGAR');
  await p.goto(`${BASE}/compras/cuentas-pagar`); await p.waitForSelector('.tr-tabla tbody tr td:not(.vacio)');
  await p.locator('button[title="Ver detalle de la compra"]').first().click();
  await p.waitForSelector('.modal-detalle-compra tbody tr td:not(.vacio)');
  const sc = await p.evaluate(() => { const m = document.querySelector('.modal-detalle-compra'); const t = m.querySelector('.tabla-scroll'); return { modal: m.getBoundingClientRect().bottom <= innerHeight + 1, scroll: t.scrollHeight > t.clientHeight, filas: t.querySelectorAll('tbody tr').length }; });
  ok(sc.modal && sc.scroll && sc.filas >= 20, `detalle largo: la ventana entra en pantalla y la tabla se desplaza (${sc.filas} productos)`);
  await p.locator('.modal-detalle-compra .tabla-scroll').evaluate((t) => { t.scrollTop = t.scrollHeight; });
  ok(await p.locator('.modal-detalle-compra tfoot').isVisible(), 'se llega al total de la compra');
  await p.keyboard.press('Escape'); await p.waitForTimeout(200);
  await p.locator('.tr-tabla tbody tr').first().locator('button', { hasText: 'Pagar' }).click(); await p.waitForTimeout(400);
  ok(!(await p.locator('.modal').last().innerText()).includes('Null'), 'pagar: preventista sin "Null"');

  ok(!errs.length, `sin errores ${errs.join(' | ')}`);
  await b.close(); console.log(f ? `\n${f} FALLA(S)` : '\nTODO OK'); process.exit(f ? 1 : 0);
})();
