// Reporte Ventas vs costos (btVentReporteVentaVsCosto), como el escritorio:
//  Datos   = VentasVsCostos.rpt: agrupado por Personal; Comprobante, Sucursal, Cliente, Fecha venta, Total costo, Total venta, Utilidad
//  Grafico = ReporteGraficoUtilidades.rpt: "Ventas Por Personal y Utilidades" (torta) y "Utilidades Por Sucursales"
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/ventas_costos.e2e.cjs [carpetaCapturas]
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
  await p.goto(`${BASE}/ventas/ventas-vs-costos`); await p.waitForSelector('.filtros-reporte');
  await espiar(p);

  console.log('DATOS');
  await p.locator('.filtro-boton').click();
  await p.waitForSelector('.tr-tabla tbody tr td');
  const th = await p.locator('.tr-tabla thead').innerText();
  ok(['Comprobante', 'Sucursal', 'Cliente', 'Total costo', 'Total venta', 'Utilidad'].every((x) => th.toUpperCase().includes(x.toUpperCase())), 'columnas del .rpt');
  const tot = await p.locator('.tr-total td').allInnerTexts();
  const [tc, tv, tu] = tot.slice(-3).map(n);
  ok(Math.abs(tv - tc - tu) < 0.02, `utilidad = venta - costo en los totales (${tv} - ${tc} = ${tu})`);
  const h1 = await capturar(p, p.locator('.tr-acciones button', { hasText: 'Imprimir' }));
  ok(h1.includes('Reporte de ventas vs costos') && h1.includes('rp-subt">Expresado en Bolivianos'), 'titulo y "Expresado en Bolivianos"');
  ok(!h1.includes('class="rp-grupo rp-n1"') && (h1.match(/class="rp-grupo rp-n0"/g) || []).length >= 2, 'agrupado solo por Personal (como el .rpt)');
  ok(/Personal: \d+ · /.test(h1), 'personal con su codigo');
  ok(/<th>Sucursal<\/th>/.test(h1) && h1.includes('Sucursal Principal'), 'sucursal como columna');
  ok(/TOTAL GENERAL \(\d+ ventas\)/.test(h1) && /Utilidad:<\/b>Bs/.test(h1), 'total general y utilidad en la cabecera');
  const m1 = await medir(h1, 'vc_datos');
  ok(m1.recorte === 0, `sin texto cortado (${m1.paginas} hoja/s)`);

  console.log('GRAFICO');
  await p.locator('.filtros-reporte input[type=date]').first().fill(hace(10));
  await p.locator('.filtro-boton').click(); await p.waitForTimeout(700);
  const utilTotal = (await p.locator('.tr-total td').allInnerTexts()).slice(-1)[0].trim();
  await p.locator('.vista-reporte button', { hasText: 'Gráfico' }).click();
  await p.waitForSelector('.rv-grafico .vg');
  ok(JSON.stringify(await p.locator('.vg-sec h2').allInnerTexts()) === JSON.stringify(['Ventas Por Personal y Utilidades', 'Utilidades Por Sucursales']), 'los dos graficos del escritorio');
  ok(await p.locator('.vg-centro').count() === 0 && await p.locator('.vg-dona path').count() >= 2, 'torta (sin hueco), un sector por personal');
  const totLey = (await p.locator('.vg-sec').first().locator('.vg-ley-tot .vg-num').innerText()).trim();
  ok(totLey === utilTotal, `la torta suma la utilidad (${totLey} = ${utilTotal})`);
  const ejes = await p.locator('.vg-barras').textContent();
  ok(ejes.includes('Utilidad') && ejes.includes('Sucursal') && !/\dK/.test(ejes), 'barras con ejes "Utilidad" / "Sucursal" y numeros sin K');
  await p.waitForTimeout(400); await p.screenshot({ path: `${OUT}/vc_grafico_1440.png`, fullPage: true });
  const h2 = await capturar(p, p.locator('.rv-imprimir'));
  ok(h2.includes('Reporte gráfico de utilidades de ventas') && h2.includes('rp-subt">Expresado en Bolivianos'), 'titulo del grafico impreso');
  ok(h2.includes('Ventas Por Personal y Utilidades') && h2.includes('Utilidades Por Sucursales'), 'impresion con los dos graficos');
  const m2 = await medir(h2, 'vc_grafico');
  ok(m2.paginas === 1, `grafico en ${m2.paginas} hoja`);

  console.log('VENTAS REALIZADAS sigue igual');
  await p.goto(`${BASE}/ventas/reporte-ventas`); await p.waitForSelector('.filtros-reporte');
  await p.locator('.filtro-boton').click(); await p.waitForSelector('.tr-tabla tbody tr td');
  await p.locator('.vista-reporte button', { hasText: 'Gráfico' }).click(); await p.waitForSelector('.rv-grafico .vg');
  ok((await p.locator('.vg-sec h2').first().innerText()) === 'Ventas Por Vendedores' && await p.locator('.vg-centro').count() === 1, 'dona de ventas por vendedores');

  console.log('CELULAR');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  m.on('pageerror', (e) => errs.push(e.message));
  await login(m);
  await m.goto(`${BASE}/ventas/ventas-vs-costos`); await m.waitForSelector('.filtros-reporte');
  await m.locator('.filtro-boton').click(); await m.waitForSelector('.tr-tabla tbody tr td');
  await m.locator('.vista-reporte button', { hasText: 'Gráfico' }).click(); await m.waitForSelector('.rv-grafico .vg');
  ok(await m.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'grafico sin scroll horizontal');

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
