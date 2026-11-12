// Modulo Ventas (simulador): Personal, Proveedores y Ventas de punta a punta + responsivo
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/ventas.e2e.cjs [carpetaCapturas]
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';

(async () => {
  const b = await chromium.launch();
  let fallos = 0;
  const errs = [];
  const ok = (c, m) => { if (!c) { fallos++; console.log('  FALLA:', m); } else console.log('  ok:', m); };
  const sinScrollX = (p) => p.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth <= 0);
  // espera un toast NUEVO que contenga el texto (los anteriores pueden seguir visibles)
  const esperaToast = async (p, texto) => {
    try { await p.locator('.toast', { hasText: texto }).last().waitFor({ timeout: 7000 }); return true; } catch {
      console.log('    toasts visibles:', await p.locator('.toast').allInnerTexts()); return false;
    }
  };
  const limpiarToasts = (p) => p.evaluate(() => document.querySelectorAll('.toast-x').forEach((b) => b.click()));
  const login = async (p) => {
    await p.goto(`${BASE}/login`);
    await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter');
    await p.waitForSelector('.dash-cab, .kpi', { timeout: 15000 });
  };

  // ================= ESCRITORIO: flujo completo =================
  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  await login(p);

  console.log('PERSONAL');
  await p.goto(`${BASE}/ventas/personal`); await p.waitForSelector('.grid tbody tr');
  await p.goto(`${BASE}/ventas/personal/nuevo`); await p.waitForSelector('.registro');
  await p.locator('.btn-ok').click(); // grabar vacio -> error
  ok(await esperaToast(p, 'Requeridos'), 'valida nombre requerido');
  await p.locator('.campo:has-text("Nombre") input').first().fill('cajero de prueba');
  await p.locator('.btn-ok').click();
  ok(await esperaToast(p, 'GRABADO'), 'graba personal');
  await p.waitForURL(/\/ventas\/personal$/);
  ok(await p.locator('.grid tbody tr', { hasText: 'CAJERO DE PRUEBA' }).count() === 1, 'aparece en la lista');
  await p.locator('.grid tbody tr', { hasText: 'CAJERO DE PRUEBA' }).locator('button[title="Editar"]').click();
  await p.locator('.campo:has-text("Teléfono") input').fill('77712345');
  await p.locator('.btn-ok').click();
  ok(await esperaToast(p, 'MODIFICADO'), 'modifica personal');
  await p.waitForURL(/\/ventas\/personal$/);
  await p.locator('.grid tbody tr', { hasText: 'CAJERO DE PRUEBA' }).locator('button[title="Eliminar"]').click();
  await p.locator('.modal .btn-peligro').click();
  ok(await esperaToast(p, 'ELIMINADO'), 'elimina personal');

  console.log('PROVEEDORES');
  await p.goto(`${BASE}/ventas/proveedores/nuevo`); await p.waitForSelector('.registro');
  await p.locator('.campo:has-text("Nombre") input').first().fill('proveedor e2e');
  await p.locator('.campo:has-text("Descripción") input').fill('Distribuidor de prueba');
  await p.locator('.btn-ok').click();
  ok(await esperaToast(p, 'GRABADO'), 'graba proveedor');

  console.log('VENTAS - nuevo pedido');
  await p.goto(`${BASE}/ventas/ventas`); await p.waitForSelector('.filtros-lista');
  await p.locator('.modelo-cab .btn-pri', { hasText: 'Nuevo' }).click();
  ok(await p.locator('.menu-nuevo button').count() >= 2, 'menu con tipos de venta');
  await p.locator('.menu-nuevo button', { hasText: 'Nuevo Pedido' }).click();
  await p.waitForURL(/nuevo\?tipo=pedido/);
  await p.waitForFunction(() => document.querySelector('.campo input[placeholder*="vendedor"]')?.value);
  ok((await p.locator('.campo input[placeholder*="vendedor"]').inputValue()).length > 0, 'vendedor por defecto (usuario)');
  ok((await p.locator('.campo input[placeholder*="cliente"]').inputValue()).length > 0, 'cliente por defecto (Id 1)');
  await p.locator('.btn-ok').click();
  ok(await esperaToast(p, 'Requeridos'), 'no graba sin detalle');

  await p.locator('button', { hasText: 'Agregar productos' }).click();
  await p.waitForSelector('.selector-productos .tabla-productos tbody tr');
  await p.locator('.selector-productos input[aria-label="Buscar producto"]').fill('coca 2l');
  ok(await p.locator('.tabla-productos tbody tr.fila-clic').count() === 1, 'busqueda multipalabra');
  await p.locator('.tabla-productos tbody tr.fila-clic').first().dblclick();
  await p.locator('.cantidad-cajas input').first().fill('2');
  ok(await p.locator('.cantidad-cajas input').nth(1).inputValue() === '12', 'cajas -> unidades (conv 6)');
  await p.locator('.cantidad-cajas .btn-pri').click();
  // producto sin stock
  await p.locator('.selector-productos input[aria-label="Buscar producto"]').fill('arroz');
  await p.locator('.tabla-productos tbody tr.fila-clic').first().dblclick();
  ok(await esperaToast(p, 'NO EXISTE STOCK'), 'bloquea producto sin stock');
  // mayor al stock
  await p.locator('.selector-productos input[aria-label="Buscar producto"]').fill('yogurt');
  await p.locator('.tabla-productos tbody tr.fila-clic').first().dblclick();
  await p.locator('.cantidad-cajas input').nth(1).fill('999');
  await p.locator('.cantidad-cajas .btn-pri').click();
  ok((await p.locator('.cantidad-cajas .campo-error').innerText()).includes('Superior Al Stock'), 'no supera el stock');
  await p.locator('.cantidad-cajas input').nth(1).fill('3');
  await p.locator('.cantidad-cajas .btn-pri').click();
  await p.screenshot({ path: `${OUT}/v_selector_1440.png` });
  await p.locator('.selector-productos .modal-acciones .btn-pri').click();
  ok(await p.locator('.tabla-detalle tbody tr').count() === 2, '2 productos en el detalle');
  // descuento 10% en la fila del yogurt
  const filaYog = p.locator('.tabla-detalle tbody tr', { hasText: 'Yogurt' });
  await filaYog.locator('input[aria-label="Porcentaje de descuento"]').fill('10');
  await filaYog.locator('input[aria-label="Porcentaje de descuento"]').press('Enter');
  // 2 cajas coca = 12*8.5 = 102 ; 3 u yogurt (precio caja 2: 14*6=? -> unitario) con 10%
  const totalTxt = await p.locator('.venta-total b').innerText();
  console.log('   total en pantalla:', totalTxt);
  await p.screenshot({ path: `${OUT}/v_form_1440.png`, fullPage: true });
  await limpiarToasts(p);
  await p.locator('.btn-ok').click();
  ok(await esperaToast(p, 'GRABADO'), 'graba pedido');
  await p.waitForSelector('.nota-venta .nota-grabada', { timeout: 8000 }).catch(() => {});
  ok(/grabada con éxito/.test(await p.locator('.nota-grabada').innerText().catch(() => '')) && await p.evaluate(() => document.activeElement?.textContent.includes('Imprimir')), 'al grabar ofrece la nota (Imprimir con el foco / No imprimir)');
  await p.locator('.nota-venta .btn-sec', { hasText: 'No imprimir' }).click();
  const t1 = await p.locator('.toast', { hasText: 'GRABADO' }).last().innerText();
  await p.waitForURL(/\/ventas\/ventas$/);
  const idNuevo = (t1.match(/VENTA (\d+)/i) || [])[1];
  const fila = p.locator('.grid tbody tr', { hasText: idNuevo });
  await fila.first().waitFor({ timeout: 8000 }).catch(() => {});
  ok(await fila.count() === 1, `pedido ${idNuevo} en la lista`);
  ok((await fila.innerText()).includes(totalTxt.replace('Bs ', '')), 'total de la lista = total del formulario (servidor recalcula igual)');

  console.log('VENTAS - modificar');
  await fila.locator('button[title="Editar"]').click();
  await p.waitForSelector('.tabla-detalle tbody tr input');
  const filaCoca = p.locator('.tabla-detalle tbody tr', { hasText: 'Coca' });
  await filaCoca.locator('input[aria-label^="Cajas"]').fill('3');
  await filaCoca.locator('input[aria-label^="Cajas"]').press('Enter');
  await p.waitForFunction(() => [...document.querySelectorAll('.tabla-detalle input[aria-label^="Unidades"]')].some((i) => i.value === '18'), null, { timeout: 4000 }).catch(() => {});
  ok(await filaCoca.locator('input[aria-label^="Unidades"]').inputValue() === '18', 'cambia cajas -> unidades');
  await p.locator('.tabla-detalle tbody tr', { hasText: 'Yogurt' }).locator('button[aria-label^="Quitar"]').click();
  ok(await p.locator('.tabla-detalle tbody tr').count() === 1, 'quita fila');
  const total2 = await p.locator('.venta-total b').innerText();
  await p.locator('.btn-ok').click();
  ok(await esperaToast(p, 'MODIFICADO'), 'modifica venta');
  await p.waitForSelector('.nota-venta .nota-grabada', { timeout: 8000 }).catch(() => {});
  ok(/modificada con éxito/.test(await p.locator('.nota-grabada').innerText().catch(() => '')), 'al modificar tambien ofrece la nota');
  await p.locator('.nota-venta .btn-sec').click();
  await p.waitForURL(/\/ventas\/ventas$/);
  ok((await p.locator('.grid tbody tr', { hasText: idNuevo }).innerText()).includes(total2.replace('Bs ', '')), `total modificado = ${total2}`);

  console.log('VENTAS - nota de venta');
  await p.locator('.grid tbody tr', { hasText: idNuevo }).locator('button[title="Ver"]').click();
  await p.locator('button', { hasText: 'Nota de venta' }).click();
  await p.waitForSelector('.nota-papel');
  ok((await p.locator('.nota-literal').innerText()).includes('Bolivianos'), 'nota con monto literal');
  await p.screenshot({ path: `${OUT}/v_nota_1440.png` });
  await p.locator('.nota-venta .btn-sec').click();

  console.log('VENTAS - eliminar');
  await p.locator('.btn-txt-peligro').click();
  await p.locator('.modal .btn-peligro').click();
  ok(await esperaToast(p, 'ELIMINADO'), 'elimina venta');

  console.log('VENTAS - reglas');
  // conciliacion cerrada: ventas de hace mas de 7 dias en el simulador
  const desde = new Date(Date.now() - 20 * 864e5).toISOString().slice(0, 10);
  await p.goto(`${BASE}/ventas/ventas`); await p.waitForSelector('.grid tbody tr');
  await p.locator('.filtros-lista input[type=date]').first().fill(desde);
  await p.waitForLoadState('networkidle'); await p.waitForTimeout(600);
  const cerrada = () => p.locator('.grid tbody tr').filter({ has: p.locator('td:nth-child(9)', { hasText: /^12$/ }) }).first();
  await cerrada().waitFor({ timeout: 8000 });
  await cerrada().locator('button[title="Editar"]').click();
  ok(await esperaToast(p, 'conciliacion cerrada'), 'bloquea modificar con conciliacion cerrada');
  await cerrada().locator('button[title="Eliminar"]').click();
  ok(await esperaToast(p, 'conciliacion cerrada'), 'bloquea eliminar con conciliacion cerrada');
  // filtro por estado
  await p.locator('.chip-filtro', { hasText: 'Anulado' }).click();
  const estados = await p.locator('.grid tbody tr td:nth-child(8)').allInnerTexts();
  ok(estados.length > 0 && estados.every((x) => /ANULADO/i.test(x)), 'filtro por estado');
  // venta directa: vendedor sin conciliacion abierta
  await p.goto(`${BASE}/ventas/ventas/nuevo?tipo=directa`);
  await p.waitForFunction(() => document.querySelector('.campo input[placeholder*="vendedor"]')?.value);
  await p.locator('button', { hasText: 'Buscar' }).first().click();
  await p.locator('.modal tbody tr', { hasText: 'LUIS QUISPE' }).dblclick();
  await p.locator('button', { hasText: 'Agregar productos' }).click();
  ok(await esperaToast(p, 'conciliación abierta'), 'venta directa exige conciliacion abierta');
  await p.locator('.btn', { hasText: 'Cancelar' }).click();
  await p.close();

  // ================= RESPONSIVO =================
  for (const [w, h] of [[1024, 768], [768, 1024], [390, 844], [320, 640]]) {
    const q = await b.newPage({ viewport: { width: w, height: h }, hasTouch: w < 800 });
    q.on('pageerror', (e) => errs.push(`${w}: ${e.message}`));
    await login(q);
    for (const ruta of ['/ventas/personal', '/ventas/proveedores', '/ventas/ventas', '/ventas/personal/nuevo', '/ventas/proveedores/nuevo']) {
      await q.goto(BASE + ruta); await q.waitForTimeout(700);
      ok(await sinScrollX(q), `${w} ${ruta} sin scroll horizontal`);
    }
    await q.goto(`${BASE}/ventas/ventas/nuevo?tipo=pedido`);
    await q.waitForFunction(() => document.querySelector('.campo input[placeholder*="cliente"]')?.value);
    await q.locator('button', { hasText: 'Agregar productos' }).click();
    await q.waitForSelector('.tabla-productos tbody tr.fila-clic');
    await q.waitForTimeout(400);
    const box = await q.locator('.selector-productos').boundingBox();
    ok(box.x >= -1 && box.x + box.width <= w + 1, `${w} selector dentro de la pantalla`);
    await q.locator('.tabla-productos tbody tr.fila-clic').first().dblclick();
    await q.waitForTimeout(400);
    const bc = await q.locator('.cantidad-cajas').boundingBox();
    ok(bc.x >= -1 && bc.x + bc.width <= w + 1 && bc.y + bc.height <= h + 1, `${w} dialogo cantidad visible`);
    await q.locator('.cantidad-cajas input').first().fill('1');
    await q.locator('.cantidad-cajas .btn-pri').click();
    await q.locator('.selector-productos .modal-acciones .btn-pri').click();
    await q.waitForTimeout(300);
    ok(await sinScrollX(q), `${w} formulario con detalle sin scroll horizontal`);
    await q.screenshot({ path: `${OUT}/v_form_${w}.png`, fullPage: true });
    await q.goto(`${BASE}/ventas/ventas`); await q.waitForTimeout(500);
    if (await q.locator('.fab').isVisible()) await q.locator('.fab').click();
    else await q.locator('.modelo-cab .btn-pri', { hasText: 'Nuevo' }).click();
    ok(await q.locator('.menu-nuevo button').count() >= 2, `${w} boton Nuevo abre menu de tipos`);
    const mb = await q.locator('.menu-nuevo').boundingBox();
    ok(mb.x >= -1 && mb.x + mb.width <= w + 1 && mb.y >= 0, `${w} menu dentro de la pantalla`);
    await q.screenshot({ path: `${OUT}/v_lista_${w}.png` });
    await q.close();
  }

  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs);
  await b.close();
  process.exit(fallos || errs.length ? 1 : 0);
})();
