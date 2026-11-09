// Productos mas vendidos (btVentProdMasVendidos), como el escritorio:
//  Datos   = todas las filas de MAM_ReporteVentas @tipo 6, sin grupos
//  Grafico = primeros N: "Productos Mas Vendidos" (barras) y "Productos Mas Vendidos Por Sucursal" (dona)
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/productos_vendidos.e2e.cjs [carpetaCapturas]
const { chromium, devices } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';

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
  const espiar = (p) => p.evaluate(() => {
    window.__impreso = null; window.print = () => { window.__principal = true; };
    new MutationObserver((ms) => ms.forEach((m) => m.addedNodes.forEach((n) => {
      // impresion directa (iframe oculto) o vista previa (.vp-fondo): se simula Imprimir y luego Cerrar
      const ifr = n.tagName === 'IFRAME' ? n : n.querySelector?.('iframe.vp-hoja');
      if (!ifr) return;
      const vp = ifr.closest('.vp-fondo');
      ifr.contentWindow.print = () => { window.__impreso = '<!doctype html>' + ifr.contentDocument.documentElement.outerHTML;; if (vp) vp.querySelector('.vp-cerrar').click(); };
      if (vp) setTimeout(() => ifr.contentWindow.print(), 30); // captura la hoja de la vista previa
    }))).observe(document.body, { childList: true });
  });
  const capturar = async (p, boton) => {
    await p.evaluate(() => { window.__impreso = null; });
    await boton.click();
    await p.waitForFunction(() => window.__impreso, null, { timeout: 8000 });
    return p.evaluate(() => window.__impreso);
  };
  const medir = async (html, nombre) => {
    const h = await b.newPage();
    await h.goto(`${BASE}/login`); await h.setContent(html, { waitUntil: 'load' }); await h.emulateMedia({ media: 'print' });
    const m = await h.evaluate(() => ({ recorte: [...document.querySelectorAll('.rp-t > tbody > tr > td, .rp th')].filter((c) => c.scrollWidth > c.clientWidth + 1).length }));
    const pdf = await h.pdf({ format: 'Letter', preferCSSPageSize: true });
    m.paginas = (pdf.toString('latin1').match(/\/Type\s*\/Page[^s]/g) || []).length;
    await h.setViewportSize({ width: 816, height: 1056 }); await h.screenshot({ path: `${OUT}/${nombre}.png`, fullPage: true });
    await h.close();
    return m;
  };

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);
  await p.goto(`${BASE}/ventas/productos-mas-vendidos`); await p.waitForSelector('.filtros-reporte');
  await espiar(p);
  await p.locator('.filtro-boton').click();
  await p.waitForSelector('.tr-tabla tbody tr td');

  console.log('DATOS');
  const th = await p.locator('.tr-tabla thead').innerText();
  ok(['Código', 'Producto', 'Familia', 'Almacén', 'Cantidad', 'Total'].every((x) => th.toUpperCase().includes(x.toUpperCase())), 'columnas del SP @tipo 6');
  const nTotal = Number((await p.locator('.tr-total td').first().innerText()).match(/\d+/)[0]);
  const h1 = await capturar(p, p.locator('.tr-acciones button', { hasText: 'Imprimir' }));
  ok(h1.includes('Reporte de productos más vendidos'), 'titulo');
  ok(!h1.includes('class="rp-grupo'), 'sin grupos (como el .rpt)');
  ok(new RegExp(`TOTAL \\(${nTotal} productos\\)`).test(h1), `todas las filas (${nTotal})`);
  const m1 = await medir(h1, 'pmv_datos');
  ok(m1.recorte === 0, `sin texto cortado (${m1.paginas} hoja/s)`);

  console.log('GRAFICO (primeros 10)');
  await p.locator('.vista-reporte button', { hasText: 'Gráfico' }).click();
  await p.waitForSelector('.rv-grafico .vg');
  ok(JSON.stringify(await p.locator('.vg-sec h2').allInnerTexts()) === JSON.stringify(['Productos Mas Vendidos', 'Productos Mas Vendidos Por Sucursal']), 'los dos graficos del escritorio');
  const barras = await p.locator('.vg-barras rect[fill]:not([fill="none"])').count();
  ok(barras === 10, `10 barras por defecto (${barras})`);
  ok(await p.locator('.vg-ley2 > div').count() === 10, 'leyenda en 2 columnas con los 10 productos');
  const ejes = await p.locator('.vg-barras').textContent();
  ok(ejes.includes('Cantidad') && ejes.includes('Productos'), 'ejes Cantidad / Productos');
  const ley = await p.locator('.vg-ley2 span').allInnerTexts();
  ok(ley.join('|') === [...ley].sort((x, y) => x.localeCompare(y, 'es')).join('|'), 'productos en orden alfabetico');
  const colBarra = await p.locator('.vg-barras rect[fill]:not([fill="none"])').first().getAttribute('fill');
  const colDona = await p.locator('.vg-dona path').first().getAttribute('fill');
  ok(colBarra === colDona, 'el mismo color por producto en los dos graficos');
  ok((await p.locator('.vg-dona').textContent()).includes('Sucursal Principal'), 'nombre de la sucursal debajo de la dona');
  const centro = (await p.locator('.vg-centro').textContent()).trim();
  const totLey = (await p.locator('.vg-sec').nth(1).locator('.vg-ley-tot .vg-num').innerText()).trim();
  ok(centro === totLey, `total al centro = total de la leyenda (${totLey})`);
  await p.locator('.top-n input').fill('5'); await p.waitForTimeout(300);
  ok(await p.locator('.vg-barras rect[fill]:not([fill="none"])').count() === 5, 'Mostrar 5 productos');
  await p.locator('.top-n input').fill('10'); await p.waitForTimeout(300);
  await p.screenshot({ path: `${OUT}/pmv_grafico_1440.png`, fullPage: true });
  const h2 = await capturar(p, p.locator('.rv-imprimir'));
  ok(h2.includes('Reporte gráfico de productos más vendidos') && h2.includes('Productos Mas Vendidos Por Sucursal'), 'impresion con los dos graficos');
  ok(/Mostrar:<\/b>10 productos/.test(h2), 'la impresion indica cuantos productos');
  const m2 = await medir(h2, 'pmv_grafico');
  ok(m2.paginas === 1, `grafico impreso en ${m2.paginas} hoja`);

  console.log('CELULAR');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  m.on('pageerror', (e) => errs.push(e.message));
  await login(m);
  await m.goto(`${BASE}/ventas/productos-mas-vendidos`); await m.waitForSelector('.filtros-reporte');
  await m.locator('.filtro-boton').click(); await m.waitForSelector('.tr-tabla tbody tr td');
  await m.locator('.vista-reporte button', { hasText: 'Gráfico' }).click(); await m.waitForSelector('.rv-grafico .vg');
  ok(await m.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'grafico sin scroll horizontal');

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
