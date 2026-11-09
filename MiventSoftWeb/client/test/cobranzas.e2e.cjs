// Reporte de cobranzas (btnReporteCobranza), como Reporte_Cobros.rpt:
//  agrupado por Personal; Cod venta, Fecha venta, Fecha pago, Glosa (cliente - glosa), Monto (un solo monto: el cobrado)
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/cobranzas.e2e.cjs [carpetaCapturas]
const { chromium, devices } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';
const hace = (d) => new Date(Date.now() - 4 * 3600e3 - d * 864e5).toISOString().slice(0, 10);

(async () => {
  const b = await chromium.launch();
  let fallos = 0;
  const errs = [];
  const ok = (c, m) => { if (!c) { fallos++; console.log('  FALLA:', m); } else console.log('  ok:', m); };
  const login = async (p) => {
    await p.goto(`${BASE}/login`);
    await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter');
    await p.waitForSelector('.dash-cab, .kpi', { timeout: 15000 });
  };
  const n = (t) => Number(String(t).replace(/[^\d,-]/g, '').replace(',', '.'));

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);
  await p.goto(`${BASE}/ventas/reporte-cobranza`); await p.waitForSelector('.filtros-reporte');
  await p.locator('.filtros-reporte input[type=date]').first().fill(hace(30));
  await p.locator('.filtro-boton').click();
  await p.waitForSelector('.tr-tabla tbody tr td');

  console.log('PANTALLA');
  const th = (await p.locator('.tr-tabla thead th').allInnerTexts()).map((x) => x.replace(/[▲▼]/g, '').trim().toUpperCase());
  ok(JSON.stringify(th) === JSON.stringify(['COD VENTA', 'FECHA VENTA', 'FECHA PAGO', 'PERSONAL', 'GLOSA', 'MONTO']), `columnas como el escritorio (${th.join(', ')})`);
  ok(th.filter((x) => x.includes('MONTO')).length === 1, 'un solo monto (no se repite)');
  ok(await p.locator('.vista-reporte[role=tablist]').count() === 0, 'sin pestaña de grafico (el escritorio no tiene)');
  const glosa = await p.locator('.tr-tabla tbody tr').first().locator('td[data-label="Glosa"]').innerText();
  ok(/ - /.test(glosa), `glosa con el cliente (${glosa})`);
  const totalPant = n((await p.locator('.tr-total td').last().innerText()));

  console.log('IMPRESION');
  await p.evaluate(() => { window.__h = null; window.print = () => {}; new MutationObserver((ms) => ms.forEach((m) => m.addedNodes.forEach((x) => {
      // impresion directa (iframe oculto) o vista previa (.vp-fondo): se simula Imprimir y luego Cerrar
      const ifr = x.tagName === 'IFRAME' ? x : x.querySelector?.('iframe.vp-hoja');
      if (!ifr) return;
      const vp = ifr.closest('.vp-fondo');
      ifr.contentWindow.print = () => { window.__h = '<!doctype html>' + ifr.contentDocument.documentElement.outerHTML;; if (vp) vp.querySelector('.vp-cerrar').click(); };
      if (vp) setTimeout(() => ifr.contentWindow.print(), 30); // captura la hoja de la vista previa
    }))).observe(document.body, { childList: true }); });
  await p.locator('.tr-acciones button', { hasText: 'Imprimir' }).click();
  await p.waitForFunction(() => window.__h, null, { timeout: 8000 });
  const h = await p.evaluate(() => window.__h);
  ok(h.includes('Reporte de cobranzas'), 'titulo');
  ok(/Movimiento desde:<\/b>\d{2}\/\d{2}\/\d{4}/.test(h) && /Hasta:<\/b>/.test(h), '"Movimiento desde" / "Hasta" como el escritorio');
  const cols = [...h.matchAll(/<th(?: class="num")?>([^<]+)<\/th>/g)].map((x) => x[1]);
  ok(JSON.stringify(cols) === JSON.stringify(['Cod venta', 'Fecha venta', 'Fecha pago', 'Glosa', 'Monto']), `columnas impresas (${cols.join(', ')})`);
  ok((h.match(/class="rp-grupo rp-n0"/g) || []).length >= 2 && !h.includes('class="rp-grupo rp-n1"'), 'agrupado solo por Personal');
  ok(/Personal: \d+ · /.test(h), 'personal con su codigo');
  const tg = h.match(/TOTAL GENERAL \(\d+ cobros\)<\/td>(?:<td[^>]*><\/td>)*<td class="num">([^<]+)/);
  ok(tg && Math.abs(n(tg[1]) - totalPant) < 0.01, `total impreso = total en pantalla (${tg && tg[1]})`);
  const hp = await b.newPage(); await hp.goto(`${BASE}/login`); await hp.setContent(h, { waitUntil: 'load' }); await hp.emulateMedia({ media: 'print' });
  const corte = await hp.evaluate(() => [...document.querySelectorAll('.rp-t td, .rp-t th')].filter((c) => c.scrollWidth > c.clientWidth + 1).length);
  ok(corte === 0, 'sin texto cortado (la glosa baja de linea)');
  await hp.setViewportSize({ width: 816, height: 1056 }); await hp.screenshot({ path: `${OUT}/cobranzas_impreso.png` });

  console.log('UN VENDEDOR (@tipo 18)');
  await p.locator('.filtros-reporte .filtro-ancho').first().click();
  await p.keyboard.type('LUIS'); await p.waitForTimeout(300); await p.keyboard.press('Enter'); await p.waitForTimeout(200);
  await p.locator('.filtro-boton').click(); await p.waitForTimeout(800);
  const pers = new Set(await p.locator('.tr-tabla tbody td[data-label="Personal"]').allInnerTexts());
  ok(pers.size === 1 && [...pers][0].toUpperCase().includes('LUIS'), `solo ese vendedor (${[...pers]})`);

  console.log('CELULAR');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  await login(m);
  await m.goto(`${BASE}/ventas/reporte-cobranza`); await m.waitForSelector('.filtros-reporte');
  await m.locator('.filtro-boton').click(); await m.waitForTimeout(800);
  ok(await m.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'sin scroll horizontal');

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
