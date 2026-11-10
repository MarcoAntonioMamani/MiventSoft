// Los 7 reportes de Gestion Pedidos (simulador): generar, filtros, grafico, Excel, responsivo
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';
const RUTAS = ['reporte-ventas', 'ventas-vs-costos', 'productos-mas-vendidos', 'rendimiento-personal', 'ventas-por-productos', 'reporte-cobranza', 'ventas-por-categoria'];
(async () => {
  const b = await chromium.launch(); let fallos = 0; const errs = [];
  const ok = (c, m) => { if (!c) { fallos++; console.log('  FALLA:', m); } else console.log('  ok:', m); };
  const hace = (d) => { const x = new Date(Date.now() - d * 864e5 - 4 * 3600e3); return x.toISOString().slice(0, 10); };
  for (const [w, h] of [[1440, 900], [390, 844]]) {
    const p = await b.newPage({ viewport: { width: w, height: h } });
    p.on('pageerror', (e) => errs.push(`${w}: ${e.message}`));
    await p.goto(`${BASE}/login`); await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter');
    await p.waitForSelector('.cabecera');
    for (const r of RUTAS) {
      await p.goto(`${BASE}/ventas/${r}`); await p.waitForSelector('.filtros-reporte');
      await p.locator('.filtros-reporte input[type=date]').first().fill(hace(10));
      if (r === 'rendimiento-personal') {
        await p.waitForSelector('.selec-item');
        await p.locator('.selec-cab button').click();
      }
      await p.locator('.filtro-boton').click();
      await p.waitForSelector('.tr-tabla tbody tr td, .toast', { timeout: 8000 });
      await p.waitForTimeout(300);
      const n = await p.locator('.tr-tabla tbody tr').count();
      ok(n > 0, `${w} ${r}: ${n} filas`);
      ok(await p.locator('.tr-tabla thead th').count() >= 2, `${w} ${r}: columnas del SP`);
      ok(!(await p.locator('.tr-tabla thead').innerText()).match(/image/i), `${w} ${r}: sin columnas de imagen`);
      if (await p.locator('.vista-reporte button:not([disabled])', { hasText: 'Gráfico' }).count()) {
        await p.locator('.vista-reporte button', { hasText: 'Gráfico' }).click();
        await p.waitForSelector('.barras-h li, .vg-dona path, .vg-dona circle, .rm .rm-kpi', { timeout: 10000 }).catch(() => {}); ok(await p.locator('.barras-h li, .vg-dona path, .vg-dona circle, .rm .rm-kpi').count() > 0, `${w} ${r}: grafico`);
        await p.locator('.vista-reporte button', { hasText: 'Datos' }).click();
      }
      ok(await p.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), `${w} ${r}: sin scroll horizontal`);
      if (w === 1440 && r === 'reporte-ventas') {
        await p.locator('.filtros-reporte .control-elegir').click();
        await p.locator('.modal input[aria-label="Buscar cliente"]').fill('abarrotes la paz');
        await p.locator('.modal tbody tr', { hasText: 'Abarrotes La Paz' }).first().waitFor();
        await p.locator('.modal tbody tr', { hasText: 'Abarrotes La Paz' }).first().click();
        await p.locator('.filtro-boton').click(); await p.waitForTimeout(600);
        const clientes = await p.locator('.tr-tabla tbody tr').allInnerTexts();
        ok(clientes.length > 0 && clientes.every((c) => /abarrotes la paz/i.test(c)), 'filtro por cliente');
        const [d] = await Promise.all([p.waitForEvent('download'), p.locator('.tr-acciones button', { hasText: 'Excel' }).click()]);
        ok(d.suggestedFilename().endsWith('.csv'), 'Excel');
      }
      await p.screenshot({ path: `${OUT}/rep_${r}_${w}.png` });
    }
    await p.close();
  }
  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs); await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
