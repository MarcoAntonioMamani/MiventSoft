// Asignacion de pedidos (simulador): asignar, quitar, entregar, revertir, anular, detalle, lista de carga, responsivo
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';
const RUTA = `${BASE}/ventas/asignacion-pedidos`;

(async () => {
  const b = await chromium.launch();
  let fallos = 0; const errs = [];
  const ok = (c, m) => { if (!c) { fallos++; console.log('  FALLA:', m); } else console.log('  ok:', m); };
  const espera = async (p, t) => { try { await p.locator('.toast', { hasText: t }).last().waitFor({ timeout: 7000 }); return true; } catch { console.log('   toasts:', await p.locator('.toast').allInnerTexts()); return false; } };
  const sinX = (p) => p.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth);
  const login = async (p, u = 'admin', c = '123') => {
    await p.goto(`${BASE}/login`); await p.fill('input >> nth=0', u); await p.fill('input[type="password"]', c); await p.keyboard.press('Enter');
    await p.waitForSelector('.cabecera', { timeout: 15000 });
  };
  const filas = (p) => p.locator('.tr-tabla tbody tr:has(td.col-sel)');
  const accion = (p, t) => p.locator('.asig-acciones button', { hasText: t });
  const tabla = (p) => p.waitForSelector('.tr-tabla tbody tr td.col-sel', { timeout: 8000 });

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);

  console.log('PENDIENTES');
  await p.goto(RUTA); await tabla(p);
  const nPend = await filas(p).count();
  ok(nPend >= 2, `lista pendientes (${nPend})`);
  await accion(p, 'Asignar').click();
  ok(await espera(p, 'No Existe Ningun Pedido Seleccionado'), 'sin seleccion avisa');
  const id1 = (await filas(p).nth(0).locator('td').nth(1).innerText()).trim();
  const id2 = (await filas(p).nth(1).locator('td').nth(1).innerText()).trim();
  await filas(p).nth(0).locator('td.col-sel input').check();
  await filas(p).nth(1).click(); // clic en la fila tambien selecciona
  ok((await p.locator('.asig-sel').innerText()).includes('2 seleccionados'), 'resumen de seleccion');
  await p.locator('th.col-sel input').check();
  ok((await p.locator('.asig-sel b').innerText()) === String(nPend), 'seleccionar todos');
  await p.locator('th.col-sel input').uncheck();
  ok(await p.locator('.asig-sel').count() === 0, 'quitar todos');
  await filas(p).nth(0).locator('td.col-sel input').check();
  await filas(p).nth(1).locator('td.col-sel input').check();
  await filas(p).nth(0).locator('button', { hasText: 'Detalle' }).click();
  await p.waitForSelector('.dialogo-asig');
  ok((await p.locator('.dialogo-asig h3').innerText()).includes(`Pedido # ${id1}`), 'detalle del pedido');
  await p.keyboard.press('Escape');
  ok(await p.locator('.dialogo-asig').count() === 0, 'cierra detalle con Esc');
  ok(await p.locator('.asig-sel b').innerText() === '2', 'detalle no cambia la seleccion');
  await accion(p, 'Asignar').click();
  ok(await espera(p, 'Asignacion Realizada Correctamente'), 'asigna al chofer');
  await p.waitForTimeout(400);
  ok(await filas(p).count() === nPend - 2, 'salen de pendientes');
  await p.screenshot({ path: `${OUT}/asig_pendientes_1440.png` });

  console.log('ASIGNADOS');
  await p.locator('.tabs-cxc a', { hasText: 'Asignados' }).click();
  await p.waitForURL(/asignados$/); await tabla(p);
  const txt = await p.locator('.tr-tabla tbody').innerText();
  ok(txt.includes(id1) && txt.includes(id2), 'aparecen en asignados del chofer');
  await accion(p, 'Lista de productos').click();
  await p.waitForSelector('.dialogo-asig .tr-tabla tbody tr td');
  ok((await p.locator('.dialogo-asig').innerText()).toUpperCase().includes('CARLOS VARGAS'), 'lista de carga con chofer');
  await p.keyboard.press('Escape');
  const filaDe = (id) => p.locator('.tr-tabla tbody tr', { has: p.locator('td:nth-child(2)', { hasText: new RegExp(`^${id}$`) }) });
  await filaDe(id2).locator('td.col-sel input').check();
  await accion(p, 'Quitar asignación').click();
  ok(await espera(p, 'Quitado Las Asignaciones'), 'quita asignacion');
  await p.waitForTimeout(400);
  await filaDe(id1).locator('td.col-sel input').check();
  await accion(p, 'Entregar').click();
  ok(await espera(p, 'Estado Entregado'), 'entrega pedido');

  console.log('ENTREGADOS');
  await p.locator('.tabs-cxc a', { hasText: 'Entregados' }).click();
  await p.waitForURL(/entregados$/);
  // conciliacion abierta (CONC-13)
  await p.locator('.filtro', { hasText: 'Conciliación' }).locator('button').first().click();
  await p.locator('.combo-lista, [role="listbox"]').locator('text=CONC-13').first().click();
  await tabla(p);
  ok((await p.locator('.tr-tabla tbody').innerText()).includes(id1), 'entregado en conciliacion abierta');
  await filaDe(id1).locator('td.col-sel input').check();
  await accion(p, 'Revertir a asignado').click();
  await p.locator('.modal .btn-peligro').click();
  ok(await espera(p, 'Revertido Al Estado Asignado'), 'revierte a asignado');
  await p.locator('.filtro', { hasText: 'Conciliación' }).locator('button').first().click();
  await p.locator('.combo-lista, [role="listbox"]').locator('text=CONC-12').first().click();
  await tabla(p);
  ok(await p.locator('.chip-cerrada').count() > 0, 'marca conciliacion cerrada');
  await filas(p).nth(0).locator('td.col-sel input').check();
  await accion(p, 'Revertir a asignado').click();
  ok(await espera(p, 'Conciliacion Cerrada'), 'no revierte conciliacion cerrada');

  console.log('ANULAR / REVERTIR ANULACION');
  await p.locator('.tabs-cxc a', { hasText: 'Asignados' }).click(); await tabla(p);
  await filaDe(id1).locator('td.col-sel input').check();
  await accion(p, 'Anular').click();
  ok((await p.locator('.modal').innerText()).includes('Anular Todos Los Pedidos'), 'confirmacion de anulacion');
  await p.locator('.modal .btn-peligro').click();
  ok(await espera(p, 'Anulados Correctamente'), 'anula');
  await p.locator('.tabs-cxc a', { hasText: 'Anulados' }).click(); await tabla(p);
  await p.locator('.tr-herramientas input').fill(id1); // muchos anulados: se busca (paginado de 15)
  await filaDe(id1).locator('td.col-sel input').check();
  await accion(p, 'Revertir a pendiente').click();
  await p.locator('.modal .btn-peligro').click();
  ok(await espera(p, 'Reversion Exitosa'), 'revierte anulacion');
  await p.locator('.tabs-cxc a', { hasText: 'Pendientes' }).click(); await tabla(p);
  ok((await p.locator('.tr-tabla tbody').innerText()).includes(id1), 'vuelve a pendientes');
  await p.close();

  console.log('PERMISOS (rol VENDEDOR sin el programa)');
  const q = await b.newPage({ viewport: { width: 1280, height: 800 } });
  q.on('pageerror', (e) => errs.push(e.message));
  await login(q, 'lquispe', 'abc');
  await q.goto(RUTA);
  await q.waitForSelector('.sin-permiso', { timeout: 8000 }).catch(() => {});
  ok(await q.locator('.sin-permiso').count() === 1, 'sin acceso');
  await q.close();

  console.log('RESPONSIVO');
  for (const [w, h] of [[1024, 768], [768, 1024], [390, 844], [320, 640]]) {
    const r = await b.newPage({ viewport: { width: w, height: h } });
    r.on('pageerror', (e) => errs.push(`${w}: ${e.message}`));
    await login(r);
    for (const t of ['', '/asignados', '/entregados', '/anulados']) {
      await r.goto(`${RUTA}${t}`); await r.waitForSelector('.tr-tabla tbody tr'); await r.waitForTimeout(400);
      ok(await sinX(r), `${w} asignacion${t} sin scroll horizontal`);
    }
    await r.goto(RUTA); await tabla(r);
    await filas(r).nth(0).locator('td.col-sel input').check();
    const bx = await filas(r).nth(0).locator('td.col-sel').boundingBox();
    ok(bx && bx.x + bx.width <= w + 1, `${w} casilla visible`);
    await r.screenshot({ path: `${OUT}/asig_${w}.png` });
    await filas(r).nth(0).locator('button', { hasText: 'Detalle' }).click();
    await r.waitForSelector('.dialogo-asig .tr-tabla'); await r.waitForTimeout(300);
    const d = await r.locator('.dialogo-asig').boundingBox();
    ok(d.x >= -1 && d.x + d.width <= w + 1, `${w} detalle dentro de la pantalla`);
    await r.keyboard.press('Escape');
    await r.close();
  }
  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs);
  await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
