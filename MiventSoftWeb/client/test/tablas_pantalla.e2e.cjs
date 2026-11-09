// Las listas (GridBuscador) entran en la pantalla sin scroll horizontal a 1280 y 1440 px
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/tablas_pantalla.e2e.cjs
const { chromium } = require('playwright');
(async () => { const b = await chromium.launch(); let f = 0; const ok = (c, m) => { if (!c) f++; console.log(c ? '  ok:' : '  FALLA:', m); };
for (const w of [1280, 1440]) {
const p = await b.newPage({ viewport: { width: w, height: 900 } }); const errs = []; p.on('pageerror', (e) => errs.push(e.message));
await p.goto('http://localhost:3001/login'); await p.fill('input >> nth=0','admin'); await p.fill('input[type="password"]','123'); await p.keyboard.press('Enter'); await p.waitForSelector('.dash-cab, .kpi');
for (const r of ['compras/compras', 'ventas/ventas', 'ventas/proformas', 'configuracion/productos', 'configuracion/clientes', 'inventario/movimientos', 'distribucion/despachos', 'configuracion/usuarios']) {
  await p.goto(`http://localhost:3001/${r}`); await p.waitForSelector('.grid tbody tr', { timeout: 10000 }).catch(() => {}); await p.waitForTimeout(500);
  const m = await p.evaluate(() => { const t = document.querySelector('.pestana-vista:not([hidden]) .tabla-scroll') || document.querySelector('.tabla-scroll'); return t ? { sw: t.scrollWidth, cw: t.clientWidth } : null; });
  ok(m && m.sw <= m.cw + 1, `${w} ${r}: la tabla entra (${m && m.sw}/${m && m.cw})`);
  if (r === 'compras/compras') { await p.screenshot({ path: `${process.argv[2] || require('os').tmpdir()}/fit_compras_${w}.png` }); ok(await p.locator('.grid tbody tr').first().locator('button[title="Nota de compra"]').count() === 1, `${w} compras: boton Nota de compra en la fila`); }
}
ok(!errs.length, `${w} sin errores ${errs.join('|')}`); await p.close(); }
await b.close(); console.log(f ? `${f} FALLA(S)` : 'TODO OK'); process.exit(f ? 1 : 0); })();
