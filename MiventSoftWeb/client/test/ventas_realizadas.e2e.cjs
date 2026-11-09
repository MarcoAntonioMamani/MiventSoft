// Reporte de ventas realizadas (btVentReporteRealizada): Datos agrupado Sucursal > Personal y Grafico mejorado.
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/ventas_realizadas.e2e.cjs [carpetaCapturas]
const { chromium, devices } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';
const hace = (d) => { const x = new Date(Date.now() - 4 * 3600e3 - d * 864e5); return x.toISOString().slice(0, 10); };

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
    const m = await h.evaluate(() => ({
      recorte: [...document.querySelectorAll('.rp td, .rp th, .rp-bar-n')].filter((c) => c.scrollWidth > c.clientWidth + 1).length,
      ancho: document.querySelector('.rp-t').getBoundingClientRect().width, vw: document.documentElement.clientWidth,
    }));
    const pdf = await h.pdf({ format: 'Letter', preferCSSPageSize: true });
    require('fs').writeFileSync(`${OUT}/${nombre}.pdf`, pdf);
    m.paginas = (pdf.toString('latin1').match(/\/Type\s*\/Page[^s]/g) || []).length;
    await h.setViewportSize({ width: 816, height: 1056 }); await h.screenshot({ path: `${OUT}/${nombre}.png` });
    await h.close();
    return m;
  };

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);
  await p.goto(`${BASE}/ventas/reporte-ventas`); await p.waitForSelector('.filtros-reporte');
  await espiar(p);

  console.log('DATOS - un dia (como la captura del escritorio)');
  await p.locator('.filtro-boton').click();
  await p.waitForSelector('.tr-tabla tbody tr td');
  const tit = await p.locator('.tr-tabla thead').innerText();
  ok(/Comprobante/i.test(tit) && /Id cliente/i.test(tit) && /Total venta/i.test(tit), 'columnas del .rpt (Comprobante, Id cliente, Total venta)');
  ok(/^\d+ - \d{4}$/.test((await p.locator('.tr-tabla tbody tr').first().locator('td').first().innerText()).trim()), 'Comprobante = Id - año');
  const h1 = await capturar(p, p.locator('.tr-acciones button', { hasText: 'Imprimir' }));
  ok(!(await p.evaluate(() => window.__principal)), 'imprime solo el reporte');
  ok(h1.includes('Reporte de ventas realizadas'), 'titulo');
  ok((h1.match(/class="rp-grupo rp-n0"/g) || []).length === 1 && h1.includes('Sucursal: 1 · SUCURSAL PRINCIPAL'), 'grupo Sucursal');
  const nPer = (h1.match(/class="rp-grupo rp-n1"/g) || []).length;
  ok(nPer >= 2 && (h1.match(/class="rp-sub rp-n1"/g) || []).length === nPer, `grupos Personal con subtotal (${nPer})`);
  ok(/Personal: \d+ · /.test(h1), 'personal con su codigo, como el escritorio');
  ok(h1.includes('Total sucursal') && /TOTAL GENERAL \(\d+ ventas\)/.test(h1), 'total por sucursal y total general');
  ok(/Desde:<\/b>\d{2}\/\d{2}\/\d{4}/.test(h1) && /Total vendido:<\/b>Bs/.test(h1), 'barra con Desde/Hasta y total vendido');
  const m1 = await medir(h1, 'vr_datos_dia');
  ok(m1.recorte === 0 && m1.ancho <= m1.vw, `sin texto cortado y cabe en la hoja (${m1.paginas} hoja/s)`);

  console.log('DATOS - 10 dias, varias hojas');
  await p.locator('.filtros-reporte input[type=date]').first().fill(hace(10));
  await p.locator('.filtro-boton').click(); await p.waitForTimeout(700);
  const h2 = await capturar(p, p.locator('.tr-acciones button', { hasText: 'Imprimir' }));
  const m2 = await medir(h2, 'vr_datos_10dias');
  ok(m2.paginas >= 2 && m2.recorte === 0, `reporte largo: ${m2.paginas} hojas, sin texto cortado`);
  const filasTabla = Number((await p.locator('.tr-total td').first().innerText()).match(/\d+/)[0]);
  ok(new RegExp(`TOTAL GENERAL \\(${filasTabla} ventas\\)`).test(h2), `el reporte lleva todas las filas (${filasTabla})`);

  console.log('GRAFICO (como ReporteGraficoVentasRealizadas.rpt)');
  await p.locator('.vista-reporte button', { hasText: 'Gráfico' }).click();
  await p.waitForSelector('.rv-grafico .vg');
  const secc = await p.locator('.vg-sec h2').allInnerTexts();
  ok(JSON.stringify(secc) === JSON.stringify(['Ventas Por Vendedores', 'Ventas Por Sucursales']), `los dos graficos del escritorio (${secc})`);
  ok((await p.locator('.vg-sub').innerText()).includes('Expresado en Bolivianos'), 'subtitulo "Expresado en Bolivianos"');
  const nVend = await p.locator('.vg-sec').first().locator('.vg-ley tbody tr:not(.vg-ley-tot)').count();
  ok(nVend >= 2 && await p.locator('.vg-dona path, .vg-dona circle').count() === nVend, `dona con un sector por vendedor (${nVend})`);
  const centro = await p.locator('.vg-centro').textContent();
  const totLey = (await p.locator('.vg-sec').first().locator('.vg-ley-tot .vg-num').innerText()).trim();
  ok(centro.trim() === totLey && h2.includes(`Total vendido:</b>Bs ${totLey}`), `total al centro = total de la leyenda = total de los datos (${totLey})`);
  const nombres = await p.locator('.vg-sec').first().locator('.vg-ley tbody tr:not(.vg-ley-tot) td:nth-child(2)').allInnerTexts();
  ok(nombres.join('|') === [...nombres].sort((x, y) => x.localeCompare(y, 'es')).join('|'), 'leyenda en orden alfabetico, como el escritorio');
  ok(await p.locator('.vg-dona .vg-etq').count() === nVend, 'cada sector con su valor y linea guia');
  ok(await p.locator('.vg-barras rect[fill]:not([fill="none"])').count() === 1 && (await p.locator('.vg-sec').nth(1).locator('.vg-ley').innerText()).includes('Sucursal Principal'), 'barras por sucursal con leyenda');
  ok((await p.locator('.vg-barras').textContent()).includes('Totales Ventas') && (await p.locator('.vg-barras').textContent()).includes('Sucursales'), 'ejes "Totales Ventas" / "Sucursales"');
  const solapes = await p.evaluate(() => { const r = [...document.querySelectorAll('.vg-dona .vg-etq')].map((t) => t.getBoundingClientRect()); let n = 0; r.forEach((a, i) => r.slice(i + 1).forEach((b) => { if (a.left < b.right && b.left < a.right && a.top < b.bottom && b.top < a.bottom) n++; })); return n; });
  ok(solapes === 0, 'los valores de la dona no se enciman');
  const fuera = await p.evaluate(() => { const sv = document.querySelector('.vg-dona').getBoundingClientRect(); return [...document.querySelectorAll('.vg-dona .vg-etq')].filter((t) => { const r = t.getBoundingClientRect(); return r.left < sv.left - 0.5 || r.right > sv.right + 0.5; }).length; });
  ok(fuera === 0, 'los valores de la dona no se salen del dibujo');
  await p.waitForTimeout(400); await p.screenshot({ path: `${OUT}/vr_grafico_1440.png`, fullPage: true });
  const h3 = await capturar(p, p.locator('.rv-imprimir'));
  ok(h3.includes('Reporte gráfico de ventas realizadas') && h3.includes('Ventas Por Vendedores') && h3.includes('Ventas Por Sucursales'), 'impresion con los dos graficos');
  ok(h3.includes('class="vg-dona"') && h3.includes('.vg-ley'), 'la impresion usa el mismo dibujo y estilos que la pantalla');
  ok(/Desde:<\/b>\d{2}\/\d{2}\/\d{4}/.test(h3) && /Usuario:<\/b>/.test(h3), 'cabecera con Desde/Hasta y usuario');
  const m3 = await medir(h3, 'vr_grafico');
  ok(m3.recorte === 0 && m3.paginas === 1, `grafico impreso en ${m3.paginas} hoja`);

  console.log('CELULAR');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  m.on('pageerror', (e) => errs.push(e.message));
  await login(m);
  await m.goto(`${BASE}/ventas/reporte-ventas`); await m.waitForSelector('.filtros-reporte');
  await m.locator('.filtros-reporte input[type=date]').first().fill(hace(6));
  await m.locator('.filtro-boton').click(); await m.waitForSelector('.tr-tabla tbody tr td');
  await m.locator('.vista-reporte button', { hasText: 'Gráfico' }).click(); await m.waitForSelector('.rv-grafico .vg');
  ok(await m.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'grafico sin scroll horizontal');
  await m.waitForTimeout(1200); await m.screenshot({ path: `${OUT}/vr_grafico_movil.png`, fullPage: true });

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
