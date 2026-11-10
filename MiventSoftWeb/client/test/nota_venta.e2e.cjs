// Nota de venta compacta: se imprime en un iframe aparte, empieza arriba de la hoja y ocupa ~media carta.
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/nota_venta.e2e.cjs [carpetaCapturas]
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';

(async () => {
  const b = await chromium.launch();
  let fallos = 0;
  const errs = [];
  const ok = (c, m) => { if (!c) { fallos++; console.log('  FALLA:', m); } else console.log('  ok:', m); };
  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await p.goto(`${BASE}/login`);
  await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter');
  await p.waitForSelector('.dash-cab, .kpi', { timeout: 15000 });

  await p.goto(`${BASE}/ventas/ventas`); await p.waitForSelector('.grid tbody tr');
  console.log('NOTA DE VENTA');
  // Captura lo que se manda a imprimir (sin abrir el dialogo del navegador)
  await p.evaluate(() => {
    window.__impreso = null; window.__printPrincipal = 0;
    window.print = () => { window.__printPrincipal++; };
    new MutationObserver((ms) => ms.forEach((m) => m.addedNodes.forEach((n) => {
      // impresion directa (iframe oculto) o vista previa (.vp-fondo): se simula Imprimir y luego Cerrar
      const ifr = n.tagName === 'IFRAME' ? n : n.querySelector?.('iframe.vp-hoja');
      if (!ifr) return;
      const vp = ifr.closest('.vp-fondo');
      ifr.contentWindow.print = () => { window.__impreso = ifr.contentDocument.documentElement.outerHTML;; if (vp) vp.querySelector('.vp-cerrar').click(); };
      if (vp) setTimeout(() => ifr.contentWindow.print(), 30); // captura la hoja de la vista previa
    }))).observe(document.body, { childList: true });
  });

  await p.locator('.grid tbody tr').first().click();
  await p.locator('.btn-nota-lista').click();
  await p.waitForSelector('.nota-papel');
  ok((await p.locator('.nota-literal').innerText()).includes('Bolivianos'), 'vista previa con monto literal');
  await p.screenshot({ path: `${OUT}/nota_vista.png` });

  await p.locator('.nota-venta .btn-pri', { hasText: 'Imprimir' }).click();
  await p.waitForFunction(() => window.__impreso, null, { timeout: 8000 });
  const html = await p.evaluate(() => window.__impreso);
  ok(await p.evaluate(() => window.__printPrincipal === 0), 'no imprime la pagina completa');
  ok(html.includes('nota-papel') && !html.includes('modal-cab') && !html.includes('id="root"'), 'el iframe solo lleva la hoja');
  await p.waitForFunction(() => !document.querySelector('body > iframe'), null, { timeout: 70000 }).catch(() => {});

  // Renderiza lo impreso como PDF carta para medir
  const h = await b.newPage();
  await h.goto(`${BASE}/login`); // mismo origen para el logo
  await h.setContent(html, { waitUntil: 'load' });
  await h.emulateMedia({ media: 'print' });
  const med = await h.evaluate(() => { const r = document.querySelector('.nota-papel').getBoundingClientRect(); return { top: r.top, alto: r.height }; });
  ok(med.top <= 40, `la hoja empieza arriba, solo con el margen de 8 mm (top=${med.top.toFixed(1)}px)`);
  ok(/@page\s*\{[^}]*margin:\s*0;/.test(html), '@page sin margen: el navegador no imprime fecha/titulo/URL');
  // carta = 11in = 1056 px CSS; margenes 8mm => ~995 px utiles
  ok(med.alto < 995 * 0.6, `ocupa poco de la hoja (${med.alto.toFixed(0)} px de ~995 = ${(med.alto / 995 * 100).toFixed(0)}%)`);
  const pdf = await h.pdf({ format: 'Letter', preferCSSPageSize: true });
  const paginas = (pdf.toString('latin1').match(/\/Type\s*\/Page[^s]/g) || []).length;
  ok(paginas === 1, `una sola pagina (${paginas})`);
  require('fs').writeFileSync(`${OUT}/nota_impresa.pdf`, pdf);
  await h.setViewportSize({ width: 816, height: 1056 });
  await h.screenshot({ path: `${OUT}/nota_impresa.png` });

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
