// Menu de mosaico + Distribucion (2): ingresos/egresos, zonas y reportes (simulador), 1440 y 390
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';
const REPORTES = ['gastos-chofer', 'ingresos-egresos-anual', 'ingresos-egresos-detallado', 'historico-clientes', 'productos-por-cliente', 'auditoria-ventas', 'historial-incremento'];

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

  console.log('MENU MOSAICO');
  for (const m of ['distribucion', 'ventas', 'configuracion']) {
    await p.goto(`${BASE}/${m}`); await p.waitForSelector('.mos-tile');
    ok(await p.locator('a.mos-tile').count() > 0, `${m}: tarjetas de colores`);
    await p.screenshot({ path: `${OUT}/menu_${m}_1440.png` });
  }
  ok(await p.locator('.mos-tile.pendiente').count() === 0, 'configuracion sin pendientes');
  await p.goto(`${BASE}/distribucion`); await p.waitForSelector('.mos-tile');
  ok(await p.locator('.mos-tile.pendiente').count() === 0, 'distribucion completa (sin pendientes)');
  await p.locator('a.mos-tile', { hasText: 'Asignacion De Zonas' }).click();
  await p.waitForURL(/asignacion-zonas/);

  console.log('ASIGNACION DE ZONAS');
  await p.locator('.za-lista button', { hasText: 'Carlos Vargas' }).click();
  await p.waitForSelector('.za-zona');
  await p.locator('.za-zona').last().click();
  await p.locator('.za-cab .btn-pri').click();
  ok(await espera(p, 'Asignacion Registrado'), 'guarda asignacion');

  console.log('INGRESOS / EGRESOS');
  await p.goto(`${BASE}/distribucion/ingresos-egresos/nuevo`); await p.waitForSelector('.campos');
  await p.locator('input[type="number"]').fill('35');
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'GRABADO CON ÉXITO'), 'graba movimiento');
  await p.goto(`${BASE}/distribucion/ingresos-egresos`); await p.waitForSelector('.grid tbody tr');
  await p.locator('.grid tbody tr', { hasText: 'LUZ' }).first().locator('button[title="Editar"]').click();
  ok(await espera(p, 'CIERRE DE CAJA'), 'bloquea movimiento con cierre');

  console.log('CIERRE DE CAJA');
  await p.goto(`${BASE}/distribucion/cierre-caja/nuevo`); await p.waitForSelector('.cierre-resumen');
  await p.locator('button', { hasText: 'Buscar' }).first().click();
  await p.locator('.modal tr', { hasText: 'Carlos Vargas' }).first().dblclick();
  await p.waitForTimeout(500);
  ok((await p.locator('.cierre-resumen').innerText()).includes('Cobranzas'), 'carga ventas y cobranzas de la conciliacion');
  await p.locator('button', { hasText: 'Agregar gasto' }).click();
  await p.locator('.modal input[type="number"]').fill('25');
  await p.locator('.modal .btn-pri', { hasText: 'Agregar' }).click();
  await p.locator('.cierre-resumen input').fill('500');
  ok((await p.locator('.cierre-resumen .dif dd').innerText()).length > 0, 'calcula diferencia');
  await p.screenshot({ path: `${OUT}/cierre_1440.png` });
  await p.locator('button', { hasText: 'Grabar' }).first().click();
  ok(await espera(p, 'GRABADO CON ÉXITO'), 'graba cierre');

  console.log('REPORTES');
  for (const r of REPORTES) {
    await p.goto(`${BASE}/distribucion/${r}`); await p.waitForSelector('.filtro-boton');
    await p.waitForTimeout(300);
    await p.locator('.filtro-boton').click();
    await p.waitForSelector('.tr-tabla tbody tr td', { timeout: 8000 });
    ok(await p.locator('.tr-tabla tbody tr td:not(.vacio)').count() > 0 || r === 'historial-incremento', `${r} genera`);
  }
  await p.goto(`${BASE}/distribucion/ingresos-egresos-anual`); await p.waitForSelector('.filtro-boton'); await p.waitForTimeout(300);
  await p.locator('.filtro-boton').click(); await p.waitForSelector('.graf-anual');
  ok(await p.locator('.ga-mes').count() === 12, 'grafico anual 12 meses');
  await p.screenshot({ path: `${OUT}/rep_anual_1440.png` });
  await p.goto(`${BASE}/distribucion/auditoria-ventas`); await p.waitForSelector('.filtro-boton'); await p.locator('.filtro-boton').click();
  await p.locator('.tr-tabla tbody tr.fila-clic').first().click();
  await p.waitForSelector('.dialogo-asig .tr-tabla tbody tr td');
  ok(true, 'detalle de auditoria');
  await p.close();

  console.log('RESPONSIVO 390');
  const r = await b.newPage({ viewport: { width: 390, height: 844 } });
  r.on('pageerror', (e) => errs.push(`390: ${e.message}`));
  await login(r);
  for (const t of ['distribucion', 'ventas', 'distribucion/asignacion-zonas', 'distribucion/ingresos-egresos/nuevo', 'distribucion/ingresos-egresos-anual', 'distribucion/cierre-caja/nuevo']) {
    await r.goto(`${BASE}/${t}`); await r.waitForTimeout(800);
    ok(await sinX(r), `390 ${t} sin scroll horizontal`);
    await r.screenshot({ path: `${OUT}/d2_${t.replace(/\//g, '_')}_390.png` });
  }
  await r.close();
  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs);
  await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
