// Cuentas por pagar: sin "Null" en preventista, tabla que entra en pantalla, Vencimiento/mora en una columna y detalle de la compra (MAM_Compras @tipo 4)
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/cuentas_pagar.e2e.cjs
const { chromium, devices } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
(async () => { const b = await chromium.launch(); let f = 0; const ok = (c, m) => { if (!c) f++; console.log(c ? '  ok:' : '  FALLA:', m); };
for (const [n, o] of [['1440', { viewport: { width: 1440, height: 900 } }], ['1280', { viewport: { width: 1280, height: 800 } }], ['movil', { ...devices['Pixel 7'] }]]) {
const ctx = await b.newContext(o); const p = await ctx.newPage(); const errs = []; p.on('pageerror', (e) => errs.push(e.message));
await p.goto('http://localhost:3001/login'); await p.fill('input >> nth=0','admin'); await p.fill('input[type="password"]','123'); await p.keyboard.press('Enter'); await p.waitForSelector('.dash-cab, .kpi');
await p.goto('http://localhost:3001/compras/cuentas-pagar'); await p.waitForSelector('.tr-tabla tbody tr td:not(.vacio)');
const th = await p.locator('.tr-tabla thead').innerText();
ok(!/Preventista/i.test(th) && !(await p.locator('.tr-tabla tbody').innerText()).includes('Null'), `${n} sin "Null" (preventista vacio no se muestra)`);
ok(/Vencimiento \/ mora/i.test(th) && !/^Mora$/im.test(th) && await p.locator('.venc-mora .chip-mora').count() > 0, `${n} vencimiento y mora en una columna`);
if (n !== 'movil') ok(await p.evaluate(() => { const t = document.querySelector('.tr-tabla'); const c = t.closest('.tabla-scroll') || t.parentElement; return c.scrollWidth <= c.clientWidth + 1; }), `${n} la tabla entra sin scroll horizontal`);
else ok(await p.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'movil sin scroll horizontal de pagina');
await p.screenshot({ path: `${OUT}/cxp_${n}.png` });
await p.locator('button[title="Ver detalle de la compra"]').first().click();
await p.waitForSelector('[aria-label="Detalle de la compra"] tbody tr td:not(.vacio)', { timeout: 8000 }).catch(() => {});
ok(await p.locator('[aria-label="Detalle de la compra"] tbody tr td:not(.vacio)').count() > 0 && /Total compra/.test(await p.locator('[aria-label="Detalle de la compra"]').innerText()), `${n} ver detalle de la compra`);
await p.screenshot({ path: `${OUT}/cxp_det_${n}.png` });
await p.keyboard.press('Escape'); await p.waitForTimeout(200);
ok(await p.locator('[aria-label="Detalle de la compra"]').count() === 0, `${n} Esc cierra el detalle`);
ok(!errs.length, `${n} sin errores ${errs.join('|')}`); await ctx.close(); }
await b.close(); console.log(f ? `${f} FALLA(S)` : 'TODO OK'); process.exit(f ? 1 : 0); })();
