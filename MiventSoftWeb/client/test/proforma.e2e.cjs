// Proformas (simulador): crear, modificar, imprimir, eliminar + responsivo
const { chromium } = require('playwright');
const OUT = process.argv[2] || require('os').tmpdir();
const BASE = 'http://localhost:3001';
(async () => {
  const b = await chromium.launch(); let fallos = 0; const errs = [];
  const ok = (c, m) => { if (!c) { fallos++; console.log('  FALLA:', m); } else console.log('  ok:', m); };
  const espera = async (p, t) => { try { await p.locator('.toast', { hasText: t }).last().waitFor({ timeout: 7000 }); return true; } catch { console.log('   toasts:', await p.locator('.toast').allInnerTexts()); return false; } };
  for (const [w, h] of [[1440, 900], [390, 844]]) {
    const p = await b.newPage({ viewport: { width: w, height: h } });
    p.on('pageerror', (e) => errs.push(`${w}: ${e.message}`));
    await p.goto(`${BASE}/login`); await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter');
    await p.waitForSelector('.cabecera');
    await p.goto(`${BASE}/ventas/proformas`); await p.waitForSelector('.filtros-lista');
    await p.locator('.filtros-nuevo button', { hasText: 'Nueva Proforma' }).waitFor({ timeout: 8000 }).catch(() => {});
    ok(await p.locator('.filtros-nuevo button', { hasText: 'Nueva Proforma' }).count() === 1, `${w} boton Nueva Proforma`);
    await p.locator('.filtros-nuevo button', { hasText: 'Nueva Proforma' }).click();
    await p.waitForFunction(() => document.querySelector('.campo input[placeholder*="cliente"]')?.value);
    ok(!(await p.locator('.venta-totales').innerText()).includes('Incremento'), `${w} sin incremento`);
    let urlProd = '';
    p.on('request', (r) => { if (/\/proformas\/productos/.test(r.url())) urlProd = r.url(); });
    await p.locator('button', { hasText: 'Agregar productos' }).click();
    await p.waitForSelector('.tabla-productos tbody tr.fila-clic');
    ok(/directa=0/.test(urlProd), `${w} productos de @tipo 5 (no de la conciliacion del chofer)`);
    const sinSt = p.locator('.tabla-productos tbody tr.sin-stock-elegible');
    ok(await sinSt.count() >= 1, `${w} muestra tambien los productos sin stock`);
    const fondo = await sinSt.first().locator('td').nth(1).evaluate((td) => getComputedStyle(td.closest('tr')).backgroundColor + getComputedStyle(td).backgroundColor);
    ok(/220, 38, 38/.test(fondo), `${w} los sin stock se ven en rojo`);
    await p.screenshot({ path: `${OUT}/proforma_selector_${w}.png` });
    await p.locator('.tabla-productos tbody tr.fila-clic').first().click();
    await p.locator('.cantidad-cajas input').first().fill('2');
    await p.locator('.cantidad-cajas .btn-pri').click();
    await sinSt.first().click();
    ok(await p.locator('.cantidad-cajas').count() === 1, `${w} deja elegir un producto sin stock`);
    await p.locator('.cantidad-cajas input').first().fill('3');
    await p.locator('.cantidad-cajas .btn-pri').click();
    ok(await p.locator('.cantidad-cajas').count() === 0, `${w} sin limite de stock en la cantidad`);
    await p.locator('.selector-productos .modal-acciones .btn-pri').click();
    const total = await p.locator('.venta-total b').innerText();
    await p.locator('.btn-ok').click();
    ok(await espera(p, 'PROFORMA'), `${w} graba proforma (${total})`);
    await p.waitForSelector('.nota-venta .nota-grabada', { timeout: 8000 }).catch(() => {});
    ok(/Proforma #\d+ grabada/.test(await p.locator('.nota-grabada').innerText().catch(() => '')) && await p.locator('.nota-venta .nota-papel').count() === 1, `${w} al grabar sale la proforma para decidir si imprimir`);
    await p.locator('.nota-venta .btn-sec', { hasText: 'No imprimir' }).click();
    await p.waitForURL(/proformas$/);
    const fila = p.locator('.grid tbody tr').first();
    await fila.waitFor();
    ok((await fila.innerText()).includes(total.replace('Bs ', '')), `${w} total en la lista = ${total}`);
    // imprimir desde la lista: boton de la fila y clic derecho
    await fila.locator('button[title="Imprimir"]').click();
    await p.waitForSelector('.nota-papel');
    ok(await p.locator('.nota-grabada').count() === 0 && (await p.locator('.nota-venta .modal-acciones').innerText()).includes('Cerrar'), `${w} imprimir desde la fila de la lista`);
    await p.locator('.nota-venta .btn-sec').click();
    if (w > 900) {
      await fila.click({ button: 'right' });
      ok(await p.locator('[role=menuitem]', { hasText: 'Imprimir' }).count() === 1, `${w} clic derecho con Imprimir`);
      await p.keyboard.press('Escape');
    }
    await fila.locator('button[title="Ver"]').click();
    await p.locator('.modelo-cab button', { hasText: 'Imprimir' }).first().click();
    await p.waitForSelector('.nota-papel');
    ok((await p.locator('.nv-titulo').innerText()).toUpperCase().includes('PROFORMA'), `${w} impresion de proforma`);
    const nota = await p.locator('.nota-papel').innerText();
    ok(/Nit\/Ci:\s*\S+/.test(nota) && /Hora Registro:/.test(nota) && (await p.locator('.nota-tabla tbody tr').count()) === 2, `${w} nota con los mismos datos que la nota de venta (cliente, hora, codigos)`);
    const secc = await p.locator('.nota-papel > *').evaluateAll((xs) => xs.map((x) => x.className).join('|'));
    ok(secc === 'nv-titulo|nv-cab|nv-caja nv-cliente|nota-tabla|nv-caja nv-pie', `${w} misma hoja que la nota de venta (${secc})`);
    await p.locator('.nota-papel').screenshot({ path: `${OUT}/proforma_nota_${w}.png` });
    await p.locator('.nota-venta .btn-sec').click();
    ok(await p.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), `${w} sin scroll horizontal`);
    await p.screenshot({ path: `${OUT}/proforma_${w}.png`, fullPage: true });
    if (await p.locator('.btn-txt-peligro').count()) { await p.locator('.btn-txt-peligro').click(); await p.locator('.modal .btn-peligro').click(); ok(await espera(p, 'ELIMINADO'), `${w} elimina`); }
    await p.close();
  }
  console.log(`\nFALLOS: ${fallos}  ERRORES JS: ${errs.length}`, errs); await b.close(); process.exit(fallos || errs.length ? 1 : 0);
})();
