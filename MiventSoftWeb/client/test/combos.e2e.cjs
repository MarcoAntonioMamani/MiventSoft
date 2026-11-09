// =====================================================================
//  Prueba de interfaz (Playwright): todos los combos de todas las pantallas
//  en 4 anchos. Requiere el servidor en modo prueba:  cd server && npm run mock
//  Ejecutar:  node test/combos.e2e.cjs
// =====================================================================
const { chromium } = require('playwright');
const URL = process.env.URL || 'http://localhost:3001';
const ANCHOS = [[1440, 900, 'PC'], [1024, 768, 'Laptop'], [768, 1024, 'Tablet'], [390, 844, 'Celular']];
const PANTALLAS = ['categorias', 'usuarios', 'clientes', 'productos', 'empresas', 'zonas', 'roles', 'ventas/personal', 'ventas/proveedores'];
const fallas = [];
const falla = (m) => { fallas.push(m); console.log('   FALLA:', m); };

(async () => {
  const b = await chromium.launch();
  for (const [w, h, nombre] of ANCHOS) {
    console.log(`\n== ${nombre} ${w}x${h} ==`);
    const ctx = await b.newContext({ viewport: { width: w, height: h }, isMobile: w < 500, hasTouch: w < 500 });
    const p = await ctx.newPage();
    p.on('pageerror', (e) => falla(`[${nombre}] error JS: ${e.message}`));
    await p.goto(URL);
    await p.fill('input[autocomplete=username]', 'admin'); await p.fill('input[type=password]', '123'); await p.click('button.btn-pri');
    await p.waitForSelector('.kpi, .dash-cab');
    for (const pant of PANTALLAS) {
      await p.goto(`${URL}/${pant.includes('/') ? pant : `configuracion/${pant}`}/nuevo`);
      await p.waitForSelector('.registro.editando', { timeout: 8000 }).catch(() => falla(`[${nombre}] ${pant}: no abrio el formulario nuevo`));
      await p.waitForTimeout(400);
      const sw = await p.evaluate(() => document.documentElement.scrollWidth - window.innerWidth);
      if (sw > 1) falla(`[${nombre}] ${pant}: desplazamiento horizontal de ${sw}px`);
      // Controles que se salen de su tarjeta
      const salidos = await p.evaluate(() => [...document.querySelectorAll('.registro .control, .registro .btn')].filter((el) => {
        const t = el.closest('.tarjeta'); if (!t) return false;
        const a = el.getBoundingClientRect(), c = t.getBoundingClientRect();
        return a.width > 0 && (a.right > c.right + 1 || a.left < c.left - 1);
      }).length);
      if (salidos) falla(`[${nombre}] ${pant}: ${salidos} control(es) se salen de su tarjeta`);
      const n = await p.locator('.registro .combo-btn').count();
      let ok = 0;
      for (let i = 0; i < n; i++) {
        const btnC = p.locator('.registro .combo-btn').nth(i);
        await btnC.scrollIntoViewIfNeeded();
        await btnC.click();
        const lista = p.locator('.combo-lista');
        if (!(await lista.isVisible().catch(() => false))) { falla(`[${nombre}] ${pant} combo ${i + 1}: no se abrio`); continue; }
        await p.waitForTimeout(350); // esperar fin de la animacion
        const caja = await lista.boundingBox();
        if (caja.x < 0 || caja.y < 0 || caja.x + caja.width > w + 1 || caja.y + caja.height > h + 1) falla(`[${nombre}] ${pant} combo ${i + 1}: la lista queda fuera de la pantalla`);
        const filas = lista.locator('tbody tr');
        const nf = await filas.count();
        const fila = filas.nth(nf > 1 ? nf - 1 : 0);
        const esperado = (await fila.locator('td').nth(1).innerText()).trim();
        await fila.click();
        await p.waitForTimeout(250);
        if (await lista.isVisible().catch(() => false)) { falla(`[${nombre}] ${pant} combo ${i + 1}: NO SE CERRO al elegir`); await p.keyboard.press('Escape'); continue; }
        const texto = (await btnC.innerText()).trim();
        if (texto !== esperado) { falla(`[${nombre}] ${pant} combo ${i + 1}: eligio "${esperado}" pero muestra "${texto}"`); continue; }
        // teclado: abrir, flecha arriba, Enter
        await btnC.focus(); await p.keyboard.press('Enter'); await p.keyboard.press('ArrowUp'); await p.keyboard.press('Enter'); await p.waitForTimeout(200);
        if (await lista.isVisible().catch(() => false)) { falla(`[${nombre}] ${pant} combo ${i + 1}: con teclado no se cerro`); await p.keyboard.press('Escape'); continue; }
        ok++;
      }
      // Personal (ayuda) en usuarios
      if (pant === 'usuarios') {
        await p.click('.btn:has-text("Buscar"), .control-elegir');
        await p.locator('.modal tbody tr').nth(1).dblclick();
        await p.waitForTimeout(250);
        if (await p.locator('.modal').isVisible().catch(() => false)) falla(`[${nombre}] usuarios: la ayuda de Personal no se cerro`);
        const per = await p.locator('.control-elegir').inputValue().catch(() => '');
        if (!per) falla(`[${nombre}] usuarios: no se cargo el Personal elegido`);
      }
      console.log(`   ${pant}: ${ok}/${n} combos OK`);
      if (nombre === 'Celular' || nombre === 'Tablet' || pant === 'usuarios') await p.screenshot({ path: `/tmp/claude-0/shots/c_${nombre}_${pant}.png`, fullPage: true });
    }
    await ctx.close();
  }
  await b.close();
  console.log(fallas.length ? `\n${fallas.length} FALLA(S)` : '\nTODO OK');
  process.exit(fallas.length ? 1 : 0);
})();
