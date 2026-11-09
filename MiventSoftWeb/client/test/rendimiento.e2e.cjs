// Rendimiento del personal: grafico de lineas por mes (uno o varios vendedores), sin textos encimados.
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/rendimiento.e2e.cjs [carpetaCapturas]
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
  // textos del grafico que se encimen (etiquetas, ejes, valores)
  const encimados = (p, sel = '.rm-graf svg.vl') => p.evaluate((s) => {
    let n = 0;
    document.querySelectorAll(s).forEach((svg) => {
      const r = [...svg.querySelectorAll('text')].filter((t) => getComputedStyle(t.closest('.vl-s') || t).opacity !== '0').map((t) => t.getBoundingClientRect()).filter((x) => x.width);
      r.forEach((a, i) => r.slice(i + 1).forEach((c) => { if (a.left < c.right - 1 && c.left < a.right - 1 && a.top < c.bottom - 1 && c.top < a.bottom - 1) n++; }));
    });
    return n;
  }, sel);
  const fueraSvg = (p) => p.evaluate(() => { const svg = document.querySelector('.rm-lienzo svg.vl'); const r = svg.getBoundingClientRect(); return [...svg.querySelectorAll('text')].filter((t) => { const x = t.getBoundingClientRect(); return x.left < r.left - 1 || x.right > r.right + 1 || x.top < r.top - 1 || x.bottom > r.bottom + 1; }).length; });
  const elegir = async (p, n) => {
    await p.waitForSelector('.selec-item');
    const marcados = await p.locator('.selec-item input:checked').count();
    for (let i = 0; i < marcados; i++) await p.locator('.selec-item input:checked').first().uncheck();
    for (let i = 0; i < n; i++) await p.locator('.selec-item input').nth(i).check();
  };

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);
  await p.goto(`${BASE}/ventas/rendimiento-personal`); await p.waitForSelector('.filtros-reporte');

  console.log('UN VENDEDOR - ultimos 12 meses');
  await elegir(p, 1);
  await p.locator('.filtro-boton').click(); await p.waitForSelector('.tr-tabla tbody tr td');
  await p.locator('.vista-reporte button', { hasText: 'Gráfico' }).click();
  await p.waitForSelector('.rm');
  ok(await p.locator('.rm-aviso').count() === 1, 'rango de un mes: aviso para elegir varios meses');
  await p.locator('.rm-aviso button').click();
  await p.waitForFunction(() => document.querySelectorAll('.rm-lienzo svg.vl .vl-s path').length > 0 && !document.querySelector('.rm-aviso'), null, { timeout: 15000 });
  const nMeses = await p.locator('.rm-matriz thead th.num').count() - 1;
  ok(nMeses >= 12 && nMeses <= 13, `ultimos 12 meses (${nMeses} meses)`);
  const desde = await p.locator('.filtros-reporte input[type=date]').first().inputValue();
  ok(/^\d{4}-\d{2}-01$/.test(desde), `el filtro Desde se actualiza (${desde})`);
  ok(await p.locator('.rm-lienzo .vl-s').count() >= 1 && await p.locator('.rm-lienzo .vl-area').count() === 1, 'linea con area suave (un vendedor)');
  ok(await p.locator('.rm-lienzo .vl-val').count() === nMeses, 'valor en cada mes');
  ok(await p.locator('.rm-kpi .rm-tend').count() === 1, 'tendencia (sube / baja / estable)');
  ok(await encimados(p) === 0, 'ningun texto encimado');
  ok(await fueraSvg(p) === 0, 'ningun texto fuera del grafico');
  // detalle al pasar el mouse
  const sobre = async (fx) => { await p.locator('.rm-lienzo').scrollIntoViewIfNeeded(); const bb = await p.locator('.rm-lienzo').boundingBox(); await p.mouse.move(bb.x + bb.width * fx, bb.y + bb.height * 0.5); await p.waitForTimeout(150); };
  await sobre(0.5);
  ok(await p.locator('.rm-tip').count() === 1 && /Bs/.test(await p.locator('.rm-tip').innerText()), 'detalle del mes al pasar el mouse');
  ok(/▲|▼/.test(await p.locator('.rm-tip').innerText()), 'muestra si subio o bajo respecto al mes anterior');
  await p.screenshot({ path: `${OUT}/rend_1vend.png`, fullPage: true });

  console.log('VARIOS VENDEDORES');
  await elegir(p, 5);
  await p.locator('.filtro-boton').click(); await p.waitForTimeout(400);
  await p.locator('.vista-reporte[role=tablist]').first().locator('button', { hasText: 'Gráfico' }).click();
  await p.waitForFunction(() => document.querySelectorAll('.rm-kpi').length === 5 && document.querySelectorAll('.rm-lienzo .vl-etq').length === 5, null, { timeout: 15000 });
  ok(true, '5 vendedores: 5 lineas con su nombre al final');
  const formas = await p.evaluate(() => [...document.querySelectorAll('.rm-lienzo g.vl-s:not(.vl-etq)')].map((g) => { const m = g.querySelector('circle, rect, path:nth-of-type(3)'); return m ? m.tagName + (m.getAttribute('fill') === '#fff' ? 'h' : '') : ''; }));
  ok(new Set(await p.evaluate(() => [...document.querySelectorAll('.rm-lienzo g.vl-s:not(.vl-etq) > path:nth-of-type(2)')].map((x) => x.getAttribute('stroke')))).size === 5, 'un color distinto por vendedor');
  ok(formas.length === 5, `marcadores por vendedor (${formas.join(',')})`);
  ok(await encimados(p) === 0, 'ningun texto encimado con 5 lineas (nombres separados)');
  ok(await fueraSvg(p) === 0, 'ningun texto fuera del grafico');
  ok(await p.locator('.rm-lienzo .vl-val').count() === 0, 'con varias lineas no se ponen valores sobre los puntos (no se tapan)');
  await sobre(0.4);
  ok(await p.locator('.rm-tip .rm-tip-fila').count() === 5 && /Total del mes/.test(await p.locator('.rm-tip').innerText()), 'detalle del mes con los 5 vendedores ordenados y el total');
  // resaltar un vendedor
  await p.locator('.rm-kpi').nth(2).hover(); await p.waitForTimeout(350);
  const op = await p.evaluate(() => [...document.querySelectorAll('.rm-lienzo g.vl-s:not(.vl-etq)')].map((g) => getComputedStyle(g).opacity));
  ok(op.filter((x) => x === '1').length === 1, 'al pasar por un vendedor se resalta su linea y se atenuan las demas');
  await p.mouse.move(5, 5);
  // ocultar / mostrar
  await p.locator('.rm-kpi').nth(0).click(); await p.waitForTimeout(200);
  ok(await p.locator('.rm-lienzo .vl-etq').count() === 4, 'clic en la tarjeta oculta la linea');
  await p.locator('.rm-kpi').nth(0).click(); await p.waitForTimeout(200);
  ok(await p.locator('.rm-lienzo .vl-etq').count() === 5, 'otro clic la vuelve a mostrar');
  // mejor mes y lider por mes
  ok(await p.locator('.rm-matriz th.rm-mejor').count() === 1, 'mejor mes marcado');
  ok(await p.locator('.rm-matriz td.rm-lider').count() >= nMeses - 1, 'vendedor que mas vendio en cada mes resaltado');
  ok(/Mejor mes:/.test(await p.locator('.rm-sub').first().innerText()), '¿En qué mes se vende más? con el mejor mes');
  await p.screenshot({ path: `${OUT}/rend_5vend.png`, fullPage: true });

  console.log('SEPARADOS');
  await p.locator('.rm-vista button', { hasText: 'Separados' }).click();
  ok(await p.locator('.rm-sep-item').count() === 5, 'un grafico por vendedor (sin cruces)');
  const topes = await p.evaluate(() => [...document.querySelectorAll('.rm-sep-item svg.vl')].map((s) => [...s.querySelectorAll('.vl-eje')].map((t) => t.textContent).filter((t) => /\d/.test(t) && !/[A-Za-z]{3}/.test(t)).pop()));
  ok(new Set(topes).size === 1, `misma escala en todos (${topes[0]})`);
  ok(await encimados(p, '.rm-sep-item svg.vl') === 0, 'ningun texto encimado en los separados');
  await p.screenshot({ path: `${OUT}/rend_separados.png`, fullPage: true });

  console.log('IMPRIMIR');
  await p.evaluate(() => { window.__h = null; window.print = () => {}; new MutationObserver((ms) => ms.forEach((m) => m.addedNodes.forEach((n) => {
      // impresion directa (iframe oculto) o vista previa (.vp-fondo): se simula Imprimir y luego Cerrar
      const ifr = n.tagName === 'IFRAME' ? n : n.querySelector?.('iframe.vp-hoja');
      if (!ifr) return;
      const vp = ifr.closest('.vp-fondo');
      ifr.contentWindow.print = () => { window.__h = '<!doctype html>' + ifr.contentDocument.documentElement.outerHTML;; if (vp) vp.querySelector('.vp-cerrar').click(); };
      if (vp) setTimeout(() => ifr.contentWindow.print(), 30); // captura la hoja de la vista previa
    }))).observe(document.body, { childList: true }); });
  await p.locator('.rm-vista button', { hasText: 'Juntos' }).click();
  await p.locator('.rm-cab button', { hasText: 'Imprimir' }).click();
  await p.waitForFunction(() => window.__h, null, { timeout: 8000 });
  const h = await p.evaluate(() => window.__h);
  ok(h.includes('Rendimiento del personal por mes') && h.includes('class="vl"') && h.includes('rm-matriz'), 'impresion con lineas, mejor mes y detalle');
  const hp = await b.newPage(); await hp.goto(`${BASE}/login`); await hp.setContent(h, { waitUntil: 'load' }); await hp.emulateMedia({ media: 'print' });
  const pdf = await hp.pdf({ format: 'Letter', preferCSSPageSize: true });
  const pags = (pdf.toString('latin1').match(/\/Type\s*\/Page[^s]/g) || []).length;
  ok(pags <= 2, `impreso en ${pags} hoja(s)`);
  ok(await encimados(hp, 'svg.vl') === 0, 'ningun texto encimado en la impresion');
  await hp.setViewportSize({ width: 1056, height: 816 }); ok(await hp.evaluate(() => document.documentElement.scrollWidth <= 1056), 'la hoja impresa no se desborda a lo ancho'); await hp.screenshot({ path: `${OUT}/rend_impreso.png`, fullPage: true });

  console.log('CELULAR');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  m.on('pageerror', (e) => errs.push(e.message));
  await login(m);
  await m.goto(`${BASE}/ventas/rendimiento-personal`); await m.waitForSelector('.filtros-reporte');
  await elegir(m, 3);
  await m.locator('.filtro-boton').click(); await m.waitForSelector('.tr-tabla tbody tr td');
  await m.locator('.vista-reporte button', { hasText: 'Gráfico' }).click(); await m.waitForSelector('.rm-aviso');
  await m.locator('.rm-aviso button').click();
  await m.waitForFunction(() => document.querySelectorAll('.rm-lienzo .vl-etq').length === 3, null, { timeout: 15000 });
  ok(await m.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'sin scroll horizontal');
  ok(await encimados(m) === 0, 'ningun texto encimado en celular');
  const bx = await m.locator('.rm-lienzo').boundingBox();
  await m.locator('.rm-lienzo').tap({ position: { x: bx.width * 0.7, y: bx.height * 0.5 } });
  ok(await m.locator('.rm-tip').count() === 1, 'al tocar se ve el detalle del mes');
  const t = await m.locator('.rm-tip').boundingBox();
  ok(t.x >= 0 && t.x + t.width <= 412 + 1, 'el detalle queda dentro de la pantalla');
  await m.screenshot({ path: `${OUT}/rend_movil.png`, fullPage: true });

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
