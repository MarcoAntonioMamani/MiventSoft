const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
(async () => {
  const b = await chromium.launch(); const errs = []; let fallos = 0;
  const ok = (c, msg) => { if (!c) { fallos++; console.log('  FALLA:', msg); } };
  for (const [w,h] of [[1440,900],[1024,768],[768,1024],[390,844],[320,640]]) {
    const p = await b.newPage({ viewport:{width:w,height:h}, hasTouch: w < 800 });
    p.on('pageerror', e => errs.push(w+': '+e.message));
    p.on('response', r => { if (r.url().includes('/api/dashboard') && r.status() >= 400) errs.push(w+' HTTP '+r.status()+' '+r.url()); });
    await p.goto('http://localhost:3001/login');
    await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter');
    await p.waitForSelector('.kpi'); await p.waitForTimeout(1200);
    const sx = () => p.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
    ok(await sx() === 0, w+' overflow inicial');
    // detalle: abrir, contar filas, sin overflow, cerrar con Escape
    const probar = async (loc, nombre) => {
      await loc.scrollIntoViewIfNeeded(); await loc.click(); 
      await p.waitForSelector('.modal.detalle', { timeout: 5000 });
      await p.waitForFunction(() => !document.querySelector('.detalle-cargando'), null, { timeout: 5000 });
      await p.waitForTimeout(450); const filas = await p.locator('.detalle-tabla tbody tr').count();
      const err = await p.locator('.modal.detalle .alerta').count();
      const box = await p.locator('.modal.detalle').boundingBox();
      const dentro = box.x >= -1 && box.x + box.width <= w + 1 && box.y + box.height <= h + 1;
      const hscroll = await p.evaluate(() => { const c = document.querySelector('.detalle-cuerpo'); return c.scrollWidth - c.clientWidth; });
      ok(filas > 0 && !err, `${w} ${nombre}: filas=${filas} err=${err}`);
      ok(dentro, `${w} ${nombre}: dialogo fuera de pantalla ${JSON.stringify(box)}`);
      if (w < 800) ok(hscroll <= 1, `${w} ${nombre}: scroll horizontal en detalle ${hscroll}`);
      if (nombre === 'kpi-pedidos') await p.screenshot({ path: `${OUT}/det_${w}.png` });
      await p.keyboard.press('Escape'); await p.waitForTimeout(250);
      ok(await p.locator('.modal.detalle').count() === 0, `${w} ${nombre}: no cerro`);
      return filas;
    };
    await probar(p.locator('.kpi.clic').nth(0), 'kpi-pedidos');
    await probar(p.locator('.kpi.clic').nth(3), 'kpi-vendedor');
    await probar(p.locator('.barras-h li.clic').first(), 'ranking');
    await probar(p.locator('.seg-leyenda li.clic').first(), 'estado');
    await probar(p.locator('.credito button.credito-vencido').first(), 'cxc-vencido');
    await probar(p.locator('.panel-cab .btn-txt').nth(1), 'cxp-todo');
    await probar(p.locator('.alertas li > button:not([disabled])').nth(2), 'alerta-stock0');
    // linea por hora: clic en el pico
    const pico = p.locator('.viz-pico circle'); await pico.scrollIntoViewIfNeeded();
    const bb = await pico.boundingBox(); await p.mouse.click(bb.x + bb.width/2, bb.y + bb.height/2);
    await p.waitForSelector('.modal.detalle', { timeout: 5000 }); await p.waitForTimeout(600);
    ok(await p.locator('.detalle-tabla tbody tr').count() > 0, w+' hora sin filas');
    await p.locator('.detalle-cab .btn-icono').click(); await p.waitForTimeout(250);
    // filtro vendedor
    const antes = await p.locator('.kpi').first().innerText();
    await p.locator('.filtro-vend .combo-btn').click(); await p.waitForTimeout(400);
    await p.locator('.combo-lista tbody tr').nth(2).click(); await p.waitForTimeout(1200);
    ok(await p.locator('.combo-lista').count() === 0, w+' combo no cerro');
    const desp = await p.locator('.kpi').first().innerText();
    ok(antes !== desp, w+' filtro vendedor no cambia KPI');
    // fecha
    await p.fill('input[type="date"]', '2026-10-20'); await p.waitForTimeout(1200);
    ok(desp !== await p.locator('.kpi').first().innerText(), w+' fecha no cambia KPI');
    ok(await sx() === 0, w+' overflow final');
    await p.screenshot({ path: `${OUT}/d3_${w}.png` });
    console.log(w, 'ok');
    await p.close();
  }
  console.log('fallos', fallos, 'errores', errs); await b.close();
})();
