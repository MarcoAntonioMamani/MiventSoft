// Ingreso y salida de productos (simulador): menu, movimientos, kardex, saldos, valorados, general, monitoreo + 390 px
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

  console.log('MENU');
  await p.goto(`${BASE}/inventario`); await p.waitForSelector('.mos-tile'); await p.waitForTimeout(1200);
  ok(await p.locator('a.mos-tile').count() === 6 && await p.locator('.mos-tile.pendiente').count() === 0, '6 programas como el escritorio, sin pendientes');
  await p.screenshot({ path: `${OUT}/inv_menu_1440.png` });

  console.log('MOVIMIENTOS');
  await p.goto(`${BASE}/inventario/movimientos/nuevo`); await p.waitForSelector('.venta-detalle');
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'INSERTE UN DETALLE'), 'exige detalle');
  await p.locator('button', { hasText: 'Agregar producto' }).click();
  await p.waitForSelector('.selector-productos tbody tr.fila-clic');
  await p.locator('.selector-productos tbody tr.fila-clic:not(.sin-stock)').first().click();
  await p.locator('.cantidad-cajas input').first().fill('1');
  await p.locator('.cantidad-cajas .btn-pri').click();
  await p.locator('.selector-productos .modal-acciones .btn-pri').click();
  ok(await p.locator('.tabla-detalle tbody tr.fila-nueva').count() === 1, 'producto agregado con stock final');
  await p.screenshot({ path: `${OUT}/inv_mov_1440.png` });
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'GRABADO CON ÉXITO'), 'graba movimiento');

  console.log('KARDEX');
  await p.goto(`${BASE}/inventario/historial-productos`); await p.waitForSelector('.filtro-boton'); await p.waitForTimeout(500);
  await p.locator('.filtro', { hasText: 'Producto' }).locator('button').first().click();
  await p.locator('.combo-lista tr', { hasText: '1' }).first().click();
  const d = new Date(Date.now() - 40 * 864e5).toISOString().slice(0, 10);
  await p.locator('input[type="date"]').first().fill(d);
  await p.locator('.filtro-boton').click();
  await p.waitForSelector('.kardex-resumen');
  ok((await p.locator('.tr-tabla tbody tr').first().innerText()).includes('Saldo Inicial') || (await p.locator('.tr-tabla tbody').innerText()).includes('Saldo Inicial'), 'kardex con saldo inicial');
  await p.screenshot({ path: `${OUT}/inv_kardex_1440.png` });

  console.log('REPORTES');
  for (const r of ['saldos', 'saldos-valorados', 'reporte-general']) {
    await p.goto(`${BASE}/inventario/${r}`); await p.waitForSelector('.filtro-boton'); await p.waitForTimeout(500);
    if (r === 'reporte-general') await p.locator('input[type="date"]').first().fill(d);
    await p.locator('.filtro-boton').click();
    await p.waitForSelector('.tr-tabla tbody tr td:not(.vacio)', { timeout: 8000 }).catch(() => {});
    ok(await p.locator('.tr-tabla tbody tr td:not(.vacio)').count() > 0, `${r} genera`);
  }
  await p.goto(`${BASE}/inventario/monitoreo`); await p.waitForSelector('.mon-tarjeta b');
  await p.locator('.mon-tarjeta.vencidos').click();
  await p.waitForSelector('.tr-tabla tbody tr td:not(.vacio)');
  ok(true, 'monitoreo vencidos');
  await p.screenshot({ path: `${OUT}/inv_monitoreo_1440.png` });
  await p.close();

  console.log('RESPONSIVO 390');
  const r = await b.newPage({ viewport: { width: 390, height: 844 } });
  r.on('pageerror', (e) => errs.push(`390: ${e.message}`));
  await login(r);
  for (const t of ['inventario', 'inventario/movimientos', 'inventario/movimientos/nuevo', 'inventario/historial-productos', 'inventario/monitoreo']) {
    await r.goto(`${BASE}/${t}`); await r.waitForTimeout(900);
    ok(await sinX(r), `390 ${t} sin scroll horizontal`);
    await r.screenshot({ path: `${OUT}/inv_${t.replace(/\//g, '_')}_390.png` });
  }
  await r.close();
  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs);
  await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
