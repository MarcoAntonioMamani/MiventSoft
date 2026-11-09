// Buscador de productos solo con teclado (Proforma / Ventas y Compras, Despachos, Movimientos):
//  escribir -> Flecha abajo -> mover -> Enter -> cantidad unitaria -> Enter -> de vuelta al buscador
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/teclado_productos.e2e.cjs [carpetaCapturas]
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
  await p.goto(`${BASE}/login`); await p.fill('input >> nth=0', 'admin'); await p.fill('input[type="password"]', '123'); await p.keyboard.press('Enter');
  await p.waitForSelector('.dash-cab, .kpi', { timeout: 15000 });
  const foco = () => p.evaluate(() => { const a = document.activeElement; return a?.closest('.cantidad-cajas') ? `cant:${a.closest('label')?.textContent.trim().split(/\s/)[0]}` : a?.matches('input[aria-label="Buscar producto"]') ? 'buscar' : a?.tagName === 'TR' ? `fila:${a.dataset.fila}:${a.innerText.split('\t')[1]}` : a?.tagName; });
  const marcada = () => p.locator('.selector-productos tbody tr.marcada').innerText().then((t) => t.split('\t')[1]).catch(() => '');

  console.log('PROFORMA (selector de ventas)');
  await p.goto(`${BASE}/ventas/proformas`); await p.waitForSelector('.filtros-lista');
  await p.locator('.filtros-nuevo button', { hasText: 'Nueva Proforma' }).click();
  await p.waitForFunction(() => document.querySelector('.campo input[placeholder*="cliente"]')?.value);
  await p.locator('button', { hasText: 'Agregar productos' }).click();
  await p.waitForSelector('.tabla-productos tbody tr.fila-clic');
  ok(await foco() === 'buscar', 'al abrir, el foco esta en el buscador');
  await p.keyboard.type('l');
  await p.waitForTimeout(150);
  const nFil = await p.locator('.tabla-productos tbody tr.fila-clic').count();
  ok(nFil >= 2, `filtra al escribir (${nFil} productos)`);
  await p.keyboard.press('ArrowDown');
  ok((await foco()).startsWith('fila:0'), `Flecha abajo baja a la primera fila (${await foco()})`);
  await p.keyboard.press('ArrowDown');
  const f1 = await foco();
  ok(f1.startsWith('fila:1') && (await marcada()) === f1.split(':')[2], `Flecha abajo marca la siguiente (${f1})`);
  await p.screenshot({ path: `${OUT}/teclado_fila.png` });
  await p.keyboard.press('ArrowUp'); await p.keyboard.press('ArrowUp');
  ok(await foco() === 'buscar', 'Flecha arriba en la primera fila vuelve al buscador');
  await p.keyboard.press('ArrowDown'); await p.keyboard.press('ArrowDown');
  const elegido = (await foco()).split(':')[2];
  await p.keyboard.press('Enter');
  await p.waitForSelector('.cantidad-cajas');
  ok(await foco() === 'cant:Unidades', `Enter abre la cantidad con el foco en Unidades (${await foco()})`);
  ok((await p.locator('.cantidad-cajas h3').innerText()).toUpperCase() === elegido.toUpperCase(), `es el producto marcado (${elegido})`);
  await p.keyboard.type('12');
  ok(await p.locator('.cantidad-cajas input').first().inputValue() === '2', 'las cajas se calculan solas (12 u = 2 cajas)');
  await p.keyboard.press('Enter');
  await p.waitForTimeout(200);
  ok(await p.locator('.cantidad-cajas').count() === 0 && await p.locator('.selector-productos .chip-ok').count() === 1, 'Enter agrega el producto');
  ok(await foco() === 'buscar' && await p.locator('input[aria-label="Buscar producto"]').inputValue() === '', 'despues de agregar, el foco vuelve al buscador vacio');
  // otra busqueda solo con teclado; Escape en la cantidad vuelve a la fila
  await p.keyboard.type('coca'); await p.waitForTimeout(150);
  await p.keyboard.press('Enter');
  await p.waitForSelector('.cantidad-cajas');
  await p.keyboard.press('Escape'); await p.waitForTimeout(150);
  ok(await p.locator('.selector-productos').count() === 1 && (await foco()).startsWith('fila:0'), `Escape en la cantidad vuelve a la fila (${await foco()})`);
  await p.keyboard.type('x'); await p.waitForTimeout(100);
  ok(await foco() === 'buscar', 'escribir estando en la tabla sigue la busqueda');
  await p.keyboard.press('Escape'); await p.waitForTimeout(150);
  const detN = await p.locator('.venta-detalle td.det-producto').count();
  ok(await p.locator('.selector-productos').count() === 0 && detN >= 1, `Escape cierra el buscador con el producto agregado (${await p.locator('.selector-productos').count()} / ${detN})`);

  console.log('DESPACHOS / COMPRAS (otro selector)');
  await p.goto(`${BASE}/compras/compras/nuevo`); await p.waitForSelector('.venta-detalle');
  await p.locator('.campo', { hasText: 'Proveedor' }).locator('button').first().click();
  await p.locator('.combo-lista tr').nth(1).click();
  await p.locator('button', { hasText: 'Agregar producto' }).click();
  await p.waitForSelector('.selector-productos tbody tr.fila-clic');
  ok(await foco() === 'buscar', 'foco en el buscador');
  await p.keyboard.press('ArrowDown'); await p.keyboard.press('ArrowDown'); await p.keyboard.press('ArrowDown');
  ok((await foco()).startsWith('fila:2'), 'flechas recorren la tabla');
  await p.keyboard.press('End');
  const ult = await p.locator('.selector-productos tbody tr.fila-clic').count();
  ok((await foco()).startsWith(`fila:${ult - 1}`), 'Fin va a la ultima fila');
  await p.keyboard.press('Enter'); await p.waitForSelector('.cantidad-cajas');
  ok(await foco() === 'cant:Unidades', 'foco en Unidades');
  await p.keyboard.type('6'); await p.keyboard.press('Enter'); await p.waitForTimeout(200);
  ok(await foco() === 'buscar', 'vuelve al buscador despues de agregar');

  ok(errs.length === 0, `sin errores de pagina ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
