// Ventas por categoria (btnReporteProductoCategoria), como el escritorio:
//  Detallado = ProductosXCategoriaRep.rpt (@tipo 9): Categoria > Vendedor; FechaVenta, Cod. Venta, Producto, Cantidad, Precio, Descuento, Total
//  General   = ReporteGeneralPorCategoria.rpt (@tipo 12): por Vendedor; Categoria, Cantidad, total y "Total: <vendedor>"
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/ventas_categoria.e2e.cjs [carpetaCapturas]
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
    const m = await h.evaluate(() => ({ recorte: [...document.querySelectorAll('.rp td, .rp th')].filter((c) => c.scrollWidth > c.clientWidth + 1).length }));
    const pdf = await h.pdf({ format: 'Letter', preferCSSPageSize: true });
    m.paginas = (pdf.toString('latin1').match(/\/Type\s*\/Page[^s]/g) || []).length;
    await h.setViewportSize({ width: 816, height: 1056 }); await h.screenshot({ path: `${OUT}/${nombre}.png` });
    await h.close();
    return m;
  };
  const n = (t) => Number(String(t).replace(/[^\d,-]/g, '').replace(',', '.'));

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);
  await p.goto(`${BASE}/ventas/ventas-por-categoria`); await p.waitForSelector('.filtros-reporte');
  await espiar(p);
  await p.locator('.filtros-reporte input[type=date]').first().fill(hace(20));

  console.log('DETALLADO (ProductosXCategoriaRep)');
  await p.locator('.chip-filtro', { hasText: 'Detallado' }).click();
  await p.locator('.filtro-boton').click();
  await p.waitForSelector('.tr-tabla tbody tr td');
  const th = await p.locator('.tr-tabla thead').innerText();
  ok(['Categoría', 'Vendedor', 'Fecha venta', 'Cod. venta', 'Producto', 'Cantidad', 'Precio', 'Descuento', 'Total'].every((x) => th.toUpperCase().includes(x.toUpperCase())), 'columnas en pantalla con la nota de venta');
  const h1 = await capturar(p, p.locator('.tr-acciones button', { hasText: 'Imprimir' }));
  ok(h1.includes('Reporte de ventas realizadas') && h1.includes('Detallado por categoría'), 'titulo como el escritorio');
  ok(/<th[^>]*>FechaVenta<\/th>/.test(h1) && /<th[^>]*>Cod\. Venta<\/th>/.test(h1) && /<th[^>]*>Descuento<\/th>/.test(h1), 'columnas FechaVenta, Cod. Venta, Producto, Cantidad, Precio, Descuento, Total');
  const nCat = (h1.match(/class="rp-grupo rp-n0"/g) || []).length; const nVen = (h1.match(/class="rp-grupo rp-n1"/g) || []).length;
  ok(nCat >= 2 && /Categoria: /.test(h1), `agrupado por Categoria (${nCat})`);
  ok(nVen >= nCat && /Vendedor: /.test(h1), `y dentro por Vendedor (${nVen})`);
  ok(/>\d{2},\d{3}</.test(h1), 'Cod. Venta con separador de miles (47,949)');
  ok(h1.includes('Total categoria') && /TOTAL GENERAL/.test(h1), 'totales por categoria y general');
  const m1 = await medir(h1, 'vcat_detallado');
  ok(m1.recorte === 0, `sin texto cortado (${m1.paginas} hoja/s)`);

  console.log('GENERAL (ReporteGeneralPorCategoria)');
  await p.locator('.chip-filtro', { hasText: 'General' }).click();
  ok(await p.locator('.tr-tabla').count() === 0, 'al cambiar de vista se limpia el resultado anterior');
  await p.locator('.filtro-boton').click();
  await p.waitForSelector('.tr-tabla tbody tr td');
  const th2 = await p.locator('.tr-tabla thead').innerText();
  ok(/Vendedor/i.test(th2) && /Categor/i.test(th2) && !/Producto/i.test(th2), 'pantalla: vendedor, categoria, cantidad, total');
  const totPant = (await p.locator('.tr-total td').allInnerTexts()).slice(-1)[0].trim();
  const h2 = await capturar(p, p.locator('.tr-acciones button', { hasText: 'Imprimir' }));
  ok(h2.includes('General por categoría') && !h2.includes('class="rp-grupo rp-n1"') && (h2.match(/class="rp-grupo rp-n0"/g) || []).length >= 2, 'agrupado solo por Vendedor');
  ok(/<th[^>]*>Categoria<\/th>/.test(h2) && /<th[^>]*>Cantidad<\/th>/.test(h2) && !/Cod\. Venta/.test(h2), 'columnas Categoria, Cantidad, Total');
  ok(/Total: [^<]+</.test(h2), 'subtotal "Total: <vendedor>" como el escritorio');
  ok(h2.includes(totPant), `total general = pantalla (${totPant})`);
  const m2 = await medir(h2, 'vcat_general');
  ok(m2.recorte === 0, `sin texto cortado (${m2.paginas} hoja/s)`);

  console.log('CELULAR');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  m.on('pageerror', (e) => errs.push(e.message));
  await login(m);
  await m.goto(`${BASE}/ventas/ventas-por-categoria`); await m.waitForSelector('.filtros-reporte');
  await m.locator('.filtro-boton').click(); await m.waitForSelector('.tr-tabla tbody tr td');
  ok(await m.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'sin scroll horizontal');

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
