// Reportes impresos de Cuentas por cobrar:
//  - Creditos pendientes: agrupado por preventista con subtotales y total general, sin texto cortado
//  - Creditos cobrados: sin agrupar; todas las fechas o filtrado por fecha de venta
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/cxc_reportes.e2e.cjs [carpetaCapturas]
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
  // captura lo que se manda a imprimir (sin abrir el dialogo)
  const espiarImpresion = (p) => p.evaluate(() => {
    window.__impreso = null; window.__printPrincipal = 0;
    window.print = () => { window.__printPrincipal++; };
    new MutationObserver((ms) => ms.forEach((m) => m.addedNodes.forEach((n) => {
      // impresion directa (iframe oculto) o vista previa (.vp-fondo): se simula Imprimir y luego Cerrar
      const ifr = n.tagName === 'IFRAME' ? n : n.querySelector?.('iframe.vp-hoja');
      if (!ifr) return;
      const vp = ifr.closest('.vp-fondo');
      ifr.contentWindow.print = () => { window.__impreso = '<!doctype html>' + ifr.contentDocument.documentElement.outerHTML;; if (vp) vp.querySelector('.vp-cerrar').click(); };
      if (vp) setTimeout(() => ifr.contentWindow.print(), 30); // captura la hoja de la vista previa
    }))).observe(document.body, { childList: true });
  });
  const imprimir = async (p) => {
    await p.evaluate(() => { window.__impreso = null; });
    await p.locator('.tr-acciones button', { hasText: 'Imprimir' }).click();
    await p.waitForFunction(() => window.__impreso, null, { timeout: 8000 });
    return p.evaluate(() => window.__impreso);
  };
  // renderiza lo impreso como PDF carta y mide
  const medir = async (html, nombre) => {
    const h = await b.newPage();
    await h.goto(`${BASE}/login`);
    await h.setContent(html, { waitUntil: 'load' });
    await h.emulateMedia({ media: 'print' });
    const m = await h.evaluate(() => {
      const recorte = [...document.querySelectorAll('.rp td, .rp th')].filter((c) => c.scrollWidth > c.clientWidth + 1).length;
      const t = document.querySelector('.rp-t').getBoundingClientRect();
      return { recorte, ancho: t.width, vw: document.documentElement.clientWidth, top: document.querySelector('.rp-cab').getBoundingClientRect().top };
    });
    const pdf = await h.pdf({ format: 'Letter', preferCSSPageSize: true });
    require('fs').writeFileSync(`${OUT}/${nombre}.pdf`, pdf);
    m.paginas = (pdf.toString('latin1').match(/\/Type\s*\/Page[^s]/g) || []).length;
    await h.setViewportSize({ width: 816, height: 1056 });
    await h.screenshot({ path: `${OUT}/${nombre}.png` });
    await h.close();
    return m;
  };

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);

  console.log('CREDITOS PENDIENTES - reporte agrupado');
  await p.goto(`${BASE}/ventas/cuentas-cobrar/pendientes`); await p.waitForSelector('.tr-tabla tbody tr');
  await espiarImpresion(p);
  const nPend = Number((await p.locator('.tr-total td').first().innerText()).match(/\d+/)[0]);
  const html1 = await imprimir(p);
  ok(await p.evaluate(() => window.__printPrincipal === 0), 'imprime solo el reporte (iframe)');
  ok(/@page\s*\{[^}]*margin:\s*0;/.test(html1), '@page sin margen (sin encabezado del navegador)');
  ok(html1.includes('Cobros pendientes'), 'titulo del reporte');
  ok(html1.includes('DISTRIBUIDORA CENTRAL'), 'empresa en la cabecera');
  const cab = await p.evaluate(async () => (await (await fetch('/api/ventas/cuentas-cobrar/cabecera-reporte')).json()).data);
  const conLogo = cab.imagenEmpresa && !/^default\.jpg$/i.test(cab.imagenEmpresa);
  ok(conLogo ? /<img[^>]+empresa/.test(html1) : !/<img/.test(html1), `logo de la empresa ${conLogo ? 'incluido' : '(sin logo propio: no deja imagen rota)'}`);
  const grupos = (html1.match(/class="rp-grupo[ "]/g) || []).length;
  const subs = (html1.match(/class="rp-sub[ "]/g) || []).length;
  ok(grupos >= 2 && grupos === subs, `agrupado por preventista con subtotal (${grupos} grupos)`);
  ok(html1.includes('Preventista: ANA FLORES') && html1.includes('Preventista: MARCO MAMANI'), 'grupos con el nombre del preventista');
  ok(/TOTAL GENERAL \(\d+ créditos\)/.test(html1), 'total general');
  ok((html1.match(/<tbody>[\s\S]*<\/tbody>/)[0].match(/<tr>/g) || []).length === nPend, `todas las filas (${nPend})`);
  const m1 = await medir(html1, 'cxc_pendientes');
  ok(m1.recorte === 0, `ningun texto cortado (${m1.recorte} celdas)`);
  ok(m1.ancho <= m1.vw, 'cabe en el ancho de la hoja');
  ok(m1.top <= 40, `empieza arriba (top=${m1.top.toFixed(0)}px)`);
  ok(m1.paginas === 1, `entra en ${m1.paginas} hoja(s)`);

  console.log('CREDITOS PENDIENTES - filtro de busqueda se respeta');
  await p.locator('.tr-herramientas input').fill('ANA FLORES');
  const html1b = await imprimir(p);
  ok((html1b.match(/class="rp-grupo[ "]/g) || []).length === 1, 'imprime solo lo filtrado');
  await p.locator('.tr-herramientas input').fill('');

  console.log('CREDITOS COBRADOS - todas las fechas, sin agrupar');
  await p.locator('.tabs-cxc a', { hasText: 'cobrados' }).click();
  await p.waitForSelector('.cxc-filtro-fecha');
  ok(await p.locator('.cxc-todas input').isChecked(), 'por defecto todas las fechas');
  await p.waitForSelector('.tr-tabla tbody tr');
  const nTodas = await p.locator('.tr-tabla tbody tr').count();
  ok(nTodas >= 8, `muestra todos los creditos cobrados (${nTodas})`);
  ok(await p.locator('.cxc-filtro-fecha input[type="date"]').first().isDisabled(), 'fechas deshabilitadas con "Todas"');
  const html2 = await imprimir(p);
  ok(!html2.includes('class="rp-grupo"') && !html2.includes('class="rp-sub"'), 'reporte sin agrupar');
  ok(html2.includes('Créditos cobrados') && /Fecha de venta:<\/b>Todas/.test(html2), 'indica "Fecha de venta: Todas"');
  ok(/TOTAL \(\d+ créditos\)/.test(html2), 'total del reporte');
  const m2 = await medir(html2, 'cxc_cobrados_todas');
  ok(m2.recorte === 0 && m2.ancho <= m2.vw, 'sin texto cortado y cabe en la hoja');

  console.log('CREDITOS COBRADOS - filtrado por fecha de venta');
  await Promise.all([p.waitForResponse((r) => r.url().includes('/cobrados?desde=')), p.locator('.cxc-todas input').uncheck()]);
  await p.waitForTimeout(300);
  ok(await p.locator('.tr-tabla thead th', { hasText: 'Fecha venta' }).count() === 1, 'columna Fecha venta');
  const nMes = await p.locator('.tr-tabla tbody tr:not(:has(.vacio))').count();
  ok(nMes >= 1 && nMes < nTodas, `filtra por fecha de venta del mes (${nMes} de ${nTodas})`);
  const fechas = await p.locator('.tr-tabla tbody td[data-label="Fecha venta"]').allInnerTexts();
  const [d, h] = await p.locator('.cxc-filtro-fecha input[type="date"]').evaluateAll((xs) => xs.map((x) => x.value));
  const isoDe = (t) => t.split('/').reverse().join('-');
  ok(fechas.every((t) => isoDe(t) >= d && isoDe(t) <= h), `fechas de venta dentro del rango ${d} a ${h}`);
  const html3 = await imprimir(p);
  ok(html3.includes('Fecha de venta:</b>') && !/Fecha de venta:<\/b>Todas/.test(html3), 'el reporte indica el rango');
  ok(html3.includes('>Fecha venta<'), 'columna Fecha venta en el reporte');
  await medir(html3, 'cxc_cobrados_rango');

  // rango amplio: trae los de meses anteriores
  await Promise.all([p.waitForResponse((r) => r.url().includes('desde=2026-01-01')), p.locator('.cxc-filtro-fecha input[type="date"]').first().fill('2026-01-01')]);
  await p.waitForTimeout(300);
  ok(await p.locator('.tr-tabla tbody tr').count() === nTodas, 'rango amplio = todos');
  // rango invertido: avisa y no consulta
  await p.locator('.cxc-filtro-fecha input[type="date"]').first().fill('2030-01-01');
  ok(await p.locator('.toast', { hasText: 'Desde no puede ser mayor' }).count() >= 1, 'avisa si Desde > Hasta');

  console.log('API - validacion de fechas');
  const r = await p.evaluate(async () => (await fetch('/api/ventas/cuentas-cobrar/cobrados?desde=xx&hasta=2026-01-01')).status);
  ok(r >= 400 && r < 500, `fecha invalida rechazada (${r})`);

  console.log('CELULAR - filtro usable');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  m.on('pageerror', (e) => errs.push(e.message));
  await login(m);
  await m.goto(`${BASE}/ventas/cuentas-cobrar/cobrados`); await m.waitForSelector('.cxc-filtro-fecha');
  await m.locator('.cxc-todas input').uncheck();
  await m.waitForTimeout(600);
  ok(await m.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'sin scroll horizontal');
  const alto = await m.locator('.cxc-filtro-fecha input[type="date"]').first().boundingBox();
  ok(alto.height >= 40, 'campos de fecha de buen tamaño para el dedo');
  await m.screenshot({ path: `${OUT}/cxc_cobrados_movil.png` });

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
