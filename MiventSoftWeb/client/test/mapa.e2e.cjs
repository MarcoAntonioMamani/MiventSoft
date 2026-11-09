// Mapa de clientes + iconos a color del escritorio en el menu (simulador)
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';
(async () => {
  const b = await chromium.launch();
  let fallos = 0; const errs = [];
  const ok = (c, m) => { if (!c) { fallos++; console.log('  FALLA:', m); } else console.log('  ok:', m); };
  const sinX = (p) => p.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth);
  const login = async (p) => { await p.goto(`${BASE}/login`); await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter'); await p.waitForSelector('.cabecera', { timeout: 15000 }); };
  for (const [w, h] of [[1440, 900], [390, 844]]) {
    const p = await b.newPage({ viewport: { width: w, height: h } });
    p.on('pageerror', (e) => errs.push(`${w}: ${e.message}`));
    await login(p);
    await p.goto(`${BASE}/mapa-clientes`); await p.waitForSelector('.mos-tile'); await p.waitForTimeout(1200);
    const imgs = await p.locator('.mos-ic.con-img img').evaluateAll((xs) => xs.map((x) => x.naturalWidth));
    ok(imgs.length === 2 && imgs.every((n) => n > 0), `${w} menu con iconos a color del sistema (${imgs.length})`);
    await p.screenshot({ path: `${OUT}/mapa_menu_${w}.png` });
    await p.locator('a.mos-tile', { hasText: 'Ubicacion' }).click();
    await p.waitForSelector('.mc-items button'); await p.waitForTimeout(500);
    ok(await p.locator('.leaflet-interactive').count() > 0, `${w} puntos en el mapa`);
    await p.locator('.mc-items button:not(.sin-punto)').first().click();
    ok(await p.locator('.mc-ficha').count() === 1, `${w} ficha del cliente`);
    await p.locator('.mc-items button.sin-punto').first().click().catch(() => {});
    ok(await sinX(p), `${w} sin scroll horizontal`);
    await p.screenshot({ path: `${OUT}/mapa_${w}.png` });
    for (const m of ['configuracion', 'ventas', 'distribucion', 'inventario']) {
      await p.goto(`${BASE}/${m}`); await p.waitForSelector('.mos-tile'); await p.waitForTimeout(700);
      ok(await p.locator('.mos-ic.con-img img').count() > 0, `${w} ${m} con iconos del sistema`);
      if (w === 1440) await p.screenshot({ path: `${OUT}/menu2_${m}.png` });
    }
    await p.close();
  }
  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs);
  await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
