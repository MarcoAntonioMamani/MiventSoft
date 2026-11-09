// Cuentas por cobrar + Incremento de venta + permisos por accion (simulador)
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';

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

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);

  console.log('CUENTAS POR COBRAR');
  await p.goto(`${BASE}/ventas/cuentas-cobrar`); await p.waitForSelector('.tr-tabla tbody tr.fila-clic');
  ok(await p.locator('.cxc-resumen').count() === 1, 'resumen por cobrar / vencido');
  const fila = p.locator('.tr-tabla tbody tr', { hasText: 'Abarrotes La Paz' });
  await fila.locator('button', { hasText: 'Cobrar' }).click();
  await p.waitForSelector('.dialogo-credito .credito-form');
  ok((await p.locator('.credito-form input.control-elegir').inputValue()).length > 0, 'personal por defecto = usuario');
  const saldoAntes = await p.locator('.credito-cifras .saldo b').innerText();
  await p.locator('.monto-pago').fill('99999');
  ok(await espera(p, 'MÁXIMO PARA PAGAR'), 'no permite pagar mas que el saldo');
  await p.locator('.monto-pago').fill('100');
  await p.locator('.credito-form input').nth(2).fill('R-001');
  await p.locator('.dialogo-credito button', { hasText: 'Pagar' }).click();
  ok(await espera(p, 'Registrado con Éxito'), 'registra el pago');
  await p.waitForFunction(() => document.querySelectorAll('.credito-pagos tbody tr td.num').length > 0);
  const saldoDesp = await p.locator('.credito-cifras .saldo b').innerText();
  ok(saldoAntes !== saldoDesp, `saldo baja (${saldoAntes} -> ${saldoDesp})`);
  await p.locator('.dialogo-credito .credito-form .btn', { hasText: 'Todo' }).click();
  ok(Number(await p.locator('.monto-pago').inputValue()) > 0, 'boton Todo = saldo');
  await p.screenshot({ path: `${OUT}/cxc_cobrar_1440.png` });
  await p.keyboard.press('Escape');

  console.log('ADMINISTRAR COBROS');
  await p.locator('.tabs-cxc a', { hasText: 'Administrar' }).click();
  await p.waitForURL(/administrar$/);
  await p.locator('.tr-tabla tbody tr', { hasText: 'Abarrotes La Paz' }).locator('button', { hasText: 'Pagos' }).click();
  await p.waitForSelector('.credito-pagos tbody tr td.num');
  await p.locator('.credito-pagos button[aria-label="Eliminar pago"]').first().click();
  await p.locator('.modal .btn-peligro').click();
  ok(await espera(p, 'ELIMINADO'), 'elimina pago');
  await p.locator('.credito-pagos button', { hasText: 'Estado de cuenta' }).click();
  ok(await espera(p, 'NO EXISTEN DATOS'), 'estado de cuenta sin pagos avisa');
  await p.keyboard.press('Escape');

  console.log('PENDIENTES / COBRADOS');
  await p.locator('.tabs-cxc a', { hasText: 'pendientes' }).click();
  await p.waitForSelector('.tr-tabla tfoot');
  ok(await p.locator('.chip-mora.mora-grave').count() > 0, 'mora coloreada');
  await p.locator('.tr-herramientas input').fill('rosa');
  ok(await p.locator('.tr-tabla tbody tr').count() >= 1, 'busqueda en reporte');
  await p.locator('.tabs-cxc a', { hasText: 'cobrados' }).click();
  await p.waitForSelector('.tr-tabla tbody tr td');
  ok(await p.locator('.tr-tabla tbody tr').count() >= 2, 'creditos cobrados');
  const [descarga] = await Promise.all([p.waitForEvent('download'), p.locator('.tr-acciones button', { hasText: 'Excel' }).click()]);
  ok(descarga.suggestedFilename().endsWith('.csv'), 'exporta a Excel (CSV)');

  console.log('INCREMENTO');
  await p.locator('.chip-incremento').click();
  await p.locator('.inc-monto input').fill('1.75');
  await p.locator('.dialogo-incremento .btn-pri').click();
  ok(await espera(p, 'ACTUALIZADO'), 'guarda incremento');
  ok((await p.locator('.chip-incremento b').innerText()).includes('1,75'), 'barra muestra el nuevo monto');
  await p.locator('.chip-incremento').click();
  await p.locator('.dialogo-incremento .btn-txt').click();
  await p.waitForSelector('.inc-historial .tr-tabla tbody tr td:not(.vacio)');
  ok(await p.locator('.inc-historial .tr-tabla tbody tr').count() >= 2, 'historial de cambios');
  await p.keyboard.press('Escape');
  await p.goto(`${BASE}/ventas/ventas/nuevo?tipo=pedido`);
  await p.waitForSelector('.venta-totales');
  ok((await p.locator('.venta-totales').innerText()).includes('1,75'), 'venta nueva usa el incremento nuevo');
  await p.close();

  console.log('PERMISOS POR ACCION (rol VENDEDOR: Clientes Ver + Modificar, Productos solo Ver)');
  const q = await b.newPage({ viewport: { width: 1280, height: 800 } });
  q.on('pageerror', (e) => errs.push(e.message));
  await login(q, 'lquispe', 'abc');
  await q.goto(`${BASE}/configuracion/clientes`); await q.waitForSelector('.grid tbody tr');
  ok(await q.locator('.modelo-cab .btn-pri', { hasText: 'Nuevo' }).count() === 0, 'sin boton Nuevo');
  ok(await q.locator('.grid button[title="Editar"]').count() > 0 && await q.locator('.grid button[title="Eliminar"]').count() === 0, 'con Editar, sin Eliminar');
  await q.goto(`${BASE}/configuracion/clientes/nuevo`);
  ok(await espera(q, 'no tiene permiso'), 'URL /nuevo directa bloqueada');
  await q.goto(`${BASE}/configuracion/productos`); await q.waitForSelector('.grid tbody tr');
  await q.locator('.modelo-cab .btn-pri', { hasText: 'Nuevo' }).waitFor({ timeout: 5000 }).catch(() => {});
  ok(await q.locator('.modelo-cab .btn-pri', { hasText: 'Nuevo' }).count() === 1, 'solo Ver marcado = acceso completo (como el escritorio)');
  await q.goto(`${BASE}/ventas/ventas`);
  ok(await q.locator('.sin-permiso').count() === 1, 'programa sin acceso');
  await q.close();

  console.log('RESPONSIVO');
  for (const [w, h] of [[1024, 768], [768, 1024], [390, 844], [320, 640]]) {
    const r = await b.newPage({ viewport: { width: w, height: h } });
    r.on('pageerror', (e) => errs.push(`${w}: ${e.message}`));
    await login(r);
    for (const t of ['', '/pendientes', '/cobrados', '/administrar']) {
      await r.goto(`${BASE}/ventas/cuentas-cobrar${t}`); await r.waitForSelector('.tr-tabla tbody tr'); await r.waitForTimeout(300);
      ok(await sinX(r), `${w} cuentas-cobrar${t} sin scroll horizontal`);
    }
    await r.goto(`${BASE}/ventas/cuentas-cobrar`); await r.waitForSelector('.tr-tabla tbody tr.fila-clic');
    await r.locator('.tr-tabla tbody tr.fila-clic').first().click();
    await r.waitForSelector('.credito-form'); await r.waitForTimeout(400);
    const bx = await r.locator('.dialogo-credito').boundingBox();
    ok(bx.x >= -1 && bx.x + bx.width <= w + 1, `${w} dialogo de cobro dentro de la pantalla`);
    await r.screenshot({ path: `${OUT}/cxc_dialogo_${w}.png` });
    await r.keyboard.press('Escape');
    const cab = await r.evaluate(() => { const c = document.querySelector('.cabecera'); return c.scrollWidth <= c.clientWidth + 1; });
    ok(cab, `${w} cabecera (incremento + servidor + usuario) sin desbordar`);
    await r.screenshot({ path: `${OUT}/cxc_lista_${w}.png` });
    await r.close();
  }
  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs);
  await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
