// Menu contextual de la fila (Ver / Editar / Eliminar) en las listas de ModeloF1.
// PC: clic derecho y Shift+F10. Celular: mantener presionado (sin abrirse al deslizar).
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/menu_fila.e2e.cjs [carpetaCapturas]
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
  const opciones = (p) => p.locator('.menu-fila button').allInnerTexts().then((t) => t.map((x) => x.trim()));

  // ================= PC =================
  console.log('PC - clic derecho');
  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);
  await p.goto(`${BASE}/ventas/ventas`); await p.waitForSelector('.grid tbody tr.fila');
  const fila = p.locator('.grid tbody tr.fila').nth(2);
  const id = (await fila.locator('td').first().innerText()).trim();
  await fila.locator('td').nth(3).click({ button: 'right' });
  await p.waitForSelector('.menu-fila');
  ok(JSON.stringify(await opciones(p)) === JSON.stringify(['Ver', 'Editar', 'Nota de venta', 'Eliminar']), `opciones Ver/Editar/Nota de venta/Eliminar (${await opciones(p)})`);
  ok(await fila.evaluate((tr) => tr.classList.contains('sel')), 'selecciona la fila del clic derecho');
  const caja = await p.locator('.menu-fila').boundingBox();
  const celda = await fila.locator('td').nth(3).boundingBox();
  // donde se hizo clic; si no entra hacia abajo se sube lo necesario para quedar dentro de la pantalla
  const cy = celda.y + celda.height / 2;
  ok(Math.abs(caja.x - (celda.x + celda.width / 2)) < 30 && (Math.abs(caja.y - cy) < 30 || (caja.y < cy && caja.y + caja.height <= 900 + 1 && caja.y + caja.height >= cy)), 'aparece donde se hizo clic (dentro de la pantalla)');
  ok(await p.evaluate(() => document.activeElement?.textContent.trim() === 'Ver'), 'foco en la primera opcion');
  await p.screenshot({ path: `${OUT}/menu_fila_pc.png` });
  await p.keyboard.press('Escape');
  ok(await p.locator('.menu-fila').count() === 0, 'Escape cierra');

  console.log('PC - Editar desde el menu');
  await fila.locator('td').nth(3).click({ button: 'right' });
  await p.locator('.menu-fila button', { hasText: 'Editar' }).click();
  await p.waitForURL(new RegExp(`/ventas/ventas/${id}/editar$`));
  ok(true, `abre /ventas/ventas/${id}/editar`);
  await p.goto(`${BASE}/ventas/ventas`); await p.waitForSelector('.grid tbody tr.fila');

  console.log('PC - Eliminar pide confirmacion');
  await p.locator('.grid tbody tr.fila').first().click({ button: 'right' });
  await p.locator('.menu-fila button', { hasText: 'Eliminar' }).click();
  await p.waitForSelector('.modal .btn-peligro');
  ok(true, 'muestra la confirmacion de eliminar');
  await p.keyboard.press('Escape');
  await p.locator('.modal .btn-sec').click().catch(() => {});

  console.log('PC - Ver y clic fuera');
  await p.goto(`${BASE}/ventas/ventas`); await p.waitForSelector('.grid tbody tr.fila');
  await p.locator('.grid tbody tr.fila').first().click({ button: 'right' });
  await p.mouse.click(5, 890);
  ok(await p.locator('.menu-fila').count() === 0, 'clic fuera cierra');
  await p.locator('.grid tbody tr.fila').first().click({ button: 'right' });
  await p.locator('.menu-fila button', { hasText: 'Ver' }).click();
  await p.waitForURL(/\/ventas\/ventas\/\d+$/);
  ok(true, 'Ver abre el registro');

  console.log('PC - teclado Shift+F10');
  await p.goto(`${BASE}/ventas/ventas`); await p.waitForSelector('.grid tbody tr.fila');
  await p.locator('.grid tbody tr.fila').first().click();
  await p.keyboard.press('Shift+F10');
  ok(await p.locator('.menu-fila').count() === 1, 'Shift+F10 abre el menu de la fila elegida');
  await p.keyboard.press('ArrowDown');
  ok(await p.evaluate(() => document.activeElement?.textContent.trim() === 'Editar'), 'flechas recorren las opciones');
  await p.keyboard.press('Escape');

  console.log('PC - otra pantalla (Clientes)');
  await p.goto(`${BASE}/configuracion/clientes`); await p.waitForSelector('.grid tbody tr.fila');
  await p.locator('.grid tbody tr.fila').first().click({ button: 'right' });
  ok((await opciones(p)).includes('Editar'), 'funciona en Clientes');
  await p.keyboard.press('Escape');

  // ================= CELULAR =================
  console.log('CELULAR - mantener presionado');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  m.on('pageerror', (e) => errs.push(e.message));
  await login(m);
  await m.goto(`${BASE}/ventas/ventas`); await m.waitForSelector('.grid tbody tr.fila');
  const cdp = await ctx.newCDPSession(m);
  const tarjeta = m.locator('.grid tbody tr.fila').nth(1);
  await tarjeta.scrollIntoViewIfNeeded();
  const t = await tarjeta.boundingBox();
  const x = t.x + t.width / 2; const y = t.y + 30;
  const toque = (type, px = x, py = y) => cdp.send('Input.dispatchTouchEvent', { type, touchPoints: type === 'touchEnd' ? [] : [{ x: px, y: py }] });

  // toque corto: no abre
  await toque('touchStart'); await m.waitForTimeout(120); await toque('touchEnd');
  await m.waitForTimeout(700);
  ok(await m.locator('.menu-fila').count() === 0, 'toque corto no abre el menu');

  // deslizar (scroll): no abre
  await toque('touchStart'); await m.waitForTimeout(80);
  for (let i = 1; i <= 6; i++) { await toque('touchMove', x, y - i * 12); await m.waitForTimeout(40); }
  await m.waitForTimeout(600); await toque('touchEnd');
  ok(await m.locator('.menu-fila').count() === 0, 'deslizar no abre el menu');

  // doble toque: abre el menu (no abre el registro)
  await tarjeta.scrollIntoViewIfNeeded();
  const t1 = await tarjeta.boundingBox();
  const dx = t1.x + t1.width / 2; const dy = t1.y + 30;
  await toque('touchStart', dx, dy); await m.waitForTimeout(60); await toque('touchEnd', dx, dy);
  await m.waitForTimeout(120);
  await toque('touchStart', dx, dy); await m.waitForTimeout(60); await toque('touchEnd', dx, dy);
  await m.waitForTimeout(600);
  ok(await m.locator('.menu-fila').count() === 1, 'doble toque abre el menu');
  ok(/\/ventas\/ventas$/.test(m.url()), 'doble toque no abre el registro');
  await m.screenshot({ path: `${OUT}/menu_fila_doble_toque.png` });
  await m.locator('.menu-fila-fondo').tap({ position: { x: 20, y: 20 } });
  await m.waitForTimeout(300);
  ok(await m.locator('.menu-fila').count() === 0, 'tocar fuera cierra');

  // dos toques lentos: no abre
  await toque('touchStart', dx, dy); await m.waitForTimeout(60); await toque('touchEnd', dx, dy);
  await m.waitForTimeout(600);
  await toque('touchStart', dx, dy); await m.waitForTimeout(60); await toque('touchEnd', dx, dy);
  await m.waitForTimeout(500);
  ok(await m.locator('.menu-fila').count() === 0, 'dos toques lentos no abren el menu');

  // presion larga: abre como hoja desde abajo
  await tarjeta.scrollIntoViewIfNeeded();
  const t2 = await tarjeta.boundingBox();
  await toque('touchStart', t2.x + t2.width / 2, t2.y + 30); await m.waitForTimeout(750);
  ok(await m.locator('.menu-fila').count() === 1, 'mantener presionado abre el menu');
  await toque('touchEnd', t2.x + t2.width / 2, t2.y + 30);
  await m.waitForTimeout(400);
  ok(await m.locator('.menu-fila').count() === 1, 'sigue abierto al soltar el dedo');
  const hoja = await m.locator('.menu-fila').boundingBox();
  const vp = m.viewportSize();
  ok(Math.abs(hoja.y + hoja.height - vp.height) < 2 && hoja.width >= vp.width - 1, 'en celular sale como hoja desde abajo');
  ok((await m.locator('.menu-fila-tit').innerText()).trim().length > 0, 'muestra de que registro es');
  ok(JSON.stringify(await opciones(m)) === JSON.stringify(['Ver', 'Editar', 'Nota de venta', 'Eliminar']), 'opciones en celular');
  ok(await m.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'sin scroll horizontal');
  await m.screenshot({ path: `${OUT}/menu_fila_movil.png` });
  await m.locator('.menu-fila button', { hasText: 'Editar' }).tap();
  await m.waitForURL(/\/ventas\/ventas\/\d+\/editar$/);
  ok(true, 'Editar desde el menu en celular');

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
