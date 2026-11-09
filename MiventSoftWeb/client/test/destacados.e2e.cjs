// Destacados del mes: carga, detalle al clic, sin cortes ni encimados de 320 a 1440 px
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
(async () => {
  const b = await chromium.launch(); let fallos = 0; const errs = [];
  const ok = (c, m) => { if (!c) { fallos++; console.log('  FALLA:', m); } };
  for (const [w, h] of [[1440, 900], [1100, 800], [1024, 768], [768, 1024], [390, 844], [320, 640]]) {
    const p = await b.newPage({ viewport: { width: w, height: h } });
    p.on('pageerror', (e) => errs.push(`${w}: ${e.message}`));
    await p.goto('http://localhost:3001/login');
    await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter');
    await p.waitForSelector('.destacado'); await p.waitForTimeout(1000);
    ok(await p.locator('.destacado').count() === 3, `${w} no hay 3 destacados`);
    ok(await p.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth) === 0, `${w} scroll horizontal`);
    // nada se sale de su tarjeta ni se encima
    const prob = await p.evaluate(() => {
      const out = [];
      document.querySelectorAll('.destacado').forEach((card, ci) => {
        const cr = card.getBoundingClientRect();
        card.querySelectorAll('.destacado-nombre, .destacado-valor, .destacado-resto li, .destacado-cab .btn').forEach((e) => {
          const r = e.getBoundingClientRect(); if (r.right > cr.right + 1 || r.left < cr.left - 1) out.push(`card${ci} ${e.className} sale`);
        });
        card.querySelectorAll('.destacado-resto li').forEach((li) => {
          const [, nom, val] = li.children; if (nom.getBoundingClientRect().right > val.getBoundingClientRect().left + 0.5) out.push(`card${ci} encimado`);
        });
      });
      return out;
    });
    ok(!prob.length, `${w} ${prob.join(', ')}`);
    const abrir = async (loc, n) => {
      await loc.scrollIntoViewIfNeeded(); await loc.click();
      await p.waitForSelector('.modal.detalle'); await p.waitForFunction(() => !document.querySelector('.detalle-cargando'));
      const filas = await p.locator('.detalle-tabla tbody tr').count();
      ok(filas > 0 && !(await p.locator('.modal.detalle .alerta').count()), `${w} ${n} sin filas`);
      if (w === 1440 && n === 'cliente') await p.screenshot({ path: `${OUT}/dest_det.png` });
      await p.keyboard.press('Escape'); await p.waitForTimeout(250);
    };
    await abrir(p.locator('.destacado').nth(0).locator('.destacado-ganador'), 'cliente');
    await abrir(p.locator('.destacado').nth(0).locator('.destacado-resto li').first(), 'cliente-2');
    await abrir(p.locator('.destacado').nth(1).locator('.destacado-cab .btn'), 'productos');
    await abrir(p.locator('.destacado').nth(2).locator('.destacado-ganador'), 'vendedor');
    const panel = p.locator('.panel:has(.destacados)'); await panel.scrollIntoViewIfNeeded();
    await panel.screenshot({ path: `${OUT}/dest_${w}.png` });
    console.log(w, 'ok'); await p.close();
  }
  console.log('fallos', fallos, 'errores', errs); await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
