// Almacenes, Compras y Planilla (simulador): CRUD, dialogos, menu resaltado + 390 px
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
  // el programa abierto queda resaltado en el menu lateral (y visible)
  const resaltado = async (p, texto) => {
    const a = p.locator('.menu-item.sub.active');
    if (await a.count() !== 1) return false;
    return (await a.innerText()).toLowerCase().includes(texto.toLowerCase()) && a.isVisible();
  };
  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);

  console.log('MENUS');
  for (const [m, n] of [['compras', 5], ['planillas', 5]]) {
    await p.goto(`${BASE}/${m}`); await p.waitForSelector('.mos-tile'); await p.waitForTimeout(700);
    ok(await p.locator('a.mos-tile').count() === n && await p.locator('.mos-tile.pendiente').count() === 0, `${m}: ${n} programas, sin pendientes`);
    await p.screenshot({ path: `${OUT}/cp_menu_${m}.png` });
  }

  console.log('ALMACENES');
  await p.goto(`${BASE}/inventario/almacenes/nuevo`); await p.waitForSelector('text=Datos de la sucursal');
  ok(await resaltado(p, 'Sucursales'), 'menu resalta Gestion De Sucursales');
  await p.locator('.campo', { hasText: 'Nombre' }).locator('input').first().fill('sucursal e2e');
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'GRABADO CON ÉXITO'), 'graba sucursal');

  console.log('COMPRAS');
  await p.goto(`${BASE}/compras/compras/nuevo`); await p.waitForSelector('.venta-detalle');
  ok(await resaltado(p, 'Compras'), 'menu resalta Compras');
  await p.locator('.campo', { hasText: 'Proveedor' }).locator('button').first().click();
  await p.locator('.combo-lista tr').nth(1).click();
  await p.locator('button', { hasText: 'Agregar producto' }).click();
  await p.waitForSelector('.selector-productos tbody tr.fila-clic');
  await p.locator('.selector-productos tbody tr.fila-clic').first().click();
  await p.locator('.cantidad-cajas input').first().fill('2');
  await p.locator('.cantidad-cajas .btn-pri').click();
  await p.locator('.selector-productos .modal-acciones .btn-pri').click();
  ok(await p.locator('.tabla-detalle tbody tr.fila-nueva').count() === 1, 'producto agregado');
  ok(!(await p.locator('.venta-total b').innerText()).includes(' 0,00'), 'total calculado');
  await p.locator('[role="switch"]').first().click();
  ok(await p.locator('.campo', { hasText: 'Vencimiento crédito' }).count() === 1, 'credito muestra vencimiento');
  await p.screenshot({ path: `${OUT}/cp_compra_1440.png` });
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'GRABADO CON ÉXITO'), 'graba compra');

  console.log('CUENTAS POR PAGAR');
  await p.goto(`${BASE}/compras/cuentas-pagar`); await p.waitForSelector('.tr-tabla tbody tr td:not(.vacio)');
  ok(await resaltado(p, 'Cuentas'), 'menu resalta Cuentas por pagar');
  await p.locator('.tr-tabla tbody tr button', { hasText: 'Pagar' }).first().click();
  await p.waitForSelector('.credito-cuerpo');
  await p.screenshot({ path: `${OUT}/cp_cxp_1440.png` });
  await p.keyboard.press('Escape');

  console.log('REPORTES COMPRAS');
  await p.goto(`${BASE}/compras/historico`); await p.waitForSelector('.filtro-boton');
  const d = new Date(Date.now() - 40 * 864e5).toISOString().slice(0, 10);
  await p.locator('input[type="date"]').first().fill(d);
  await p.locator('.filtro-boton').click();
  await p.waitForSelector('.tr-tabla tbody tr td:not(.vacio)', { timeout: 8000 }).catch(() => {});
  ok(await p.locator('.tr-tabla thead th', { hasText: 'Cant.' }).count() > 0, 'historico con columnas por producto');
  ok(await resaltado(p, 'Historico'), 'menu resalta Historico');
  await p.screenshot({ path: `${OUT}/cp_historico_1440.png` });
  await p.goto(`${BASE}/compras/compras-por-producto`); await p.waitForSelector('.filtro-boton');
  await p.locator('input[type="date"]').first().fill(d);
  await p.locator('.filtro-boton').click();
  await p.waitForSelector('.tr-tabla tbody tr td:not(.vacio)', { timeout: 8000 }).catch(() => {});
  ok(await p.locator('.tr-tabla tbody tr td:not(.vacio)').count() > 0, 'compras por producto genera');

  console.log('PLANILLA');
  await p.goto(`${BASE}/planilla/conceptos-fijos`); await p.waitForTimeout(800);
  ok(await resaltado(p, 'Conceptos'), 'menu resalta Conceptos fijos');
  await p.goto(`${BASE}/planilla/contratos/nuevo`); await p.waitForSelector('text=Datos del contrato');
  ok(await resaltado(p, 'Contratos'), 'menu resalta Contratos');
  await p.screenshot({ path: `${OUT}/cp_contrato_1440.png` });
  await p.goto(`${BASE}/planilla/registrar`); await p.waitForSelector('.tr-tabla tbody tr td:not(.vacio)');
  await p.locator('button', { hasText: 'Registrar planilla' }).click();
  ok(await espera(p, 'REGISTRADO'), 'registra planilla');
  await p.goto(`${BASE}/planilla/planilla`); await p.waitForSelector('.tr-tabla tbody tr td:not(.vacio)');
  ok(await resaltado(p, 'Planilla'), 'menu resalta Planilla');
  await p.locator('button[title="Conceptos variables"]').first().click();
  await p.waitForSelector('.modal .planilla-neto');
  await p.locator('.modal .filtro', { hasText: 'Concepto variable' }).locator('button').first().click();
  await p.locator('.combo-lista tr').nth(1).click();
  await p.locator('.modal input[type="number"]').fill('50');
  await p.locator('.modal button', { hasText: 'Registrar' }).click();
  ok(await espera(p, 'CONCEPTO VARIABLE REGISTRADO'), 'agrega concepto variable');
  await p.screenshot({ path: `${OUT}/cp_planilla_var_1440.png` });
  await p.keyboard.press('Escape');
  await p.close();

  console.log('RESPONSIVO 390');
  const r = await b.newPage({ viewport: { width: 390, height: 844 } });
  r.on('pageerror', (e) => errs.push(`390: ${e.message}`));
  await login(r);
  for (const t of ['compras', 'compras/compras/nuevo', 'compras/cuentas-pagar', 'compras/historico', 'inventario/almacenes/nuevo', 'planillas', 'planilla/contratos/nuevo', 'planilla/conceptos-fijos/nuevo', 'planilla/registrar', 'planilla/planilla']) {
    await r.goto(`${BASE}/${t}`); await r.waitForTimeout(900);
    ok(await sinX(r), `390 ${t} sin scroll horizontal`);
    await r.screenshot({ path: `${OUT}/cp_${t.replace(/\//g, '_')}_390.png` });
  }
  await r.close();
  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs);
  await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
