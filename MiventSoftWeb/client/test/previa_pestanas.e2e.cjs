// 1) Vista previa al imprimir: ventana con la hoja + Imprimir / Cerrar; al cerrar se sigue en el programa.
// 2) Programas abiertos en pestañas: cada programa conserva lo que se estaba haciendo.
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/previa_pestanas.e2e.cjs [carpetaCapturas]
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
  const visible = (p) => p.locator('.pestana-vista:not([hidden])');
  const menu = async (p, texto) => {
    const grupo = p.locator('.lateral .menu-grupo', { has: p.locator('.menu-item.sub', { hasText: texto }) }).first();
    if (!(await grupo.locator('.menu-sub.abierto').count())) await grupo.locator('.menu-mod-flecha').click();
    await grupo.locator('.menu-item.sub', { hasText: texto }).first().click();
  };

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  p.on('console', (m) => { if (m.type() === 'error' && !/Failed to load resource/.test(m.text())) errs.push(m.text()); });
  await login(p);
  await p.evaluate(() => { try { sessionStorage.removeItem('santit.pestanas'); } catch {} });

  console.log('VISTA PREVIA AL IMPRIMIR');
  await p.goto(`${BASE}/ventas/reporte-cobranza`); await p.waitForSelector('.filtros-reporte');
  await p.locator('.filtro-boton').click(); await visible(p).locator('.tr-tabla tbody tr td').first().waitFor();
  await p.evaluate(() => { window.__printApp = 0; window.print = () => { window.__printApp++; }; });
  const btnImp = visible(p).locator('.tr-acciones button', { hasText: 'Imprimir' });
  await btnImp.click();
  await p.waitForSelector('.vp-fondo .vp-hoja');
  ok((await p.locator('.vp-tit b').innerText()).includes('Reporte de cobranzas'), 'se abre la vista previa con el titulo del reporte');
  const fr = p.frameLocator('.vp-hoja');
  await fr.locator('.rp').waitFor();
  ok((await fr.locator('h1').innerText()).includes('Reporte de cobranzas'), 'la hoja se ve dentro de la ventana');
  ok(await p.locator('.vp-imprimir').evaluate((x) => x === document.activeElement), 'el foco queda en Imprimir');
  // Imprimir: abre la hoja en una VENTANA APARTE y la imprime ahi (el cuadro del navegador no tapa la app)
  await p.evaluate(() => {
    window.__printPopup = 0; window.__popups = [];
    const abrir = window.open.bind(window);
    window.open = (...a) => { const w = abrir(...a); if (w) { w.print = () => { window.__printPopup++; }; window.__popups.push(w); } return w; };
  });
  const [pop] = await Promise.all([p.waitForEvent('popup'), p.locator('.vp-imprimir').click()]);
  await p.waitForFunction(() => window.__printPopup === 1, null, { timeout: 5000 }).catch(() => {});
  ok(await p.evaluate(() => window.__printPopup === 1 && window.__printApp === 0), 'Imprimir abre una ventana aparte y le manda imprimir solo a ella');
  ok((await pop.title()) === 'Reporte de cobranzas' && (await pop.locator('h1').innerText()).includes('Reporte de cobranzas'), 'la ventana de impresion tiene la hoja del reporte');
  ok(await p.locator('.vp-fondo').count() === 1, 'la vista previa sigue abierta en la app (como el visor del escritorio)');
  await pop.close();
  await p.locator('.vp-imprimir').focus();
  const [pop2] = await Promise.all([p.waitForEvent('popup'), p.keyboard.press('Control+p')]);
  await p.waitForTimeout(300);
  ok(await p.evaluate(() => window.__printPopup === 2 && window.__printApp === 0), 'Ctrl+P en la vista previa imprime la hoja (en su ventana), no la app');
  await pop2.close();
  await p.keyboard.press('Escape');
  ok(await p.locator('.vp-fondo').count() === 0, 'Escape cierra la vista previa');
  ok(/\/ventas\/reporte-cobranza$/.test(p.url()) && await visible(p).locator('.tr-tabla tbody tr td').count() > 0, 'al cerrar sigue en el mismo programa con el reporte generado');
  ok(await btnImp.evaluate((x) => x === document.activeElement), 'el foco vuelve al boton Imprimir');
  await btnImp.click(); await p.waitForSelector('.vp-fondo');
  await p.locator('.vp-cerrar').click();
  ok(await p.locator('.vp-fondo').count() === 0, 'boton Cerrar');
  await btnImp.click(); await p.waitForSelector('.vp-fondo');
  await p.mouse.click(5, 5);
  ok(await p.locator('.vp-fondo').count() === 0, 'clic fuera de la ventana cierra');
  await btnImp.click(); await p.waitForSelector('.vp-fondo');
  await p.screenshot({ path: `${OUT}/vista_previa.png` });
  await p.keyboard.press('Escape');

  console.log('PESTAÑAS: CONSERVAN LO QUE SE ESTABA HACIENDO');
  ok(await p.locator('.pestana').count() === 1, 'el reporte abierto queda como pestaña');
  // Ventas: escribir una busqueda
  await menu(p, 'Crear Nuevos Pedidos'); await visible(p).locator('.grid tbody tr').first().waitFor();
  await visible(p).locator('.grid-herramientas input').fill('TIENDA');
  await p.waitForTimeout(250);
  const filasVentas = await visible(p).locator('.grid tbody tr').count();
  // Clientes: empezar un registro nuevo y escribir
  await menu(p, 'Gestion De Clientes'); await visible(p).locator('.grid tbody tr').first().waitFor();
  await visible(p).locator('.modelo-cab .btn-pri', { hasText: 'Nuevo' }).click();
  await visible(p).locator('.registro').waitFor();
  const campoNombre = visible(p).locator('.campo:has-text("Nombre") input').first();
  await campoNombre.fill('CLIENTE A MEDIO LLENAR');
  ok(await p.locator('.pestana').count() === 3, '3 programas abiertos: 3 pestañas');
  ok(await p.locator('.pestana.activa .pestana-editando').count() === 1, 'la pestaña marca el registro a medio editar');
  // volver a Ventas por la pestaña
  await p.locator('.pestana', { hasText: 'Crear Nuevos Pedidos' }).first().locator('.pestana-link').click();
  await p.waitForTimeout(200);
  ok(await visible(p).locator('.grid-herramientas input').inputValue() === 'TIENDA' && await visible(p).locator('.grid tbody tr').count() === filasVentas, 'Ventas conserva la busqueda escrita');
  // volver a Clientes desde el MENU: debe quedar el formulario como estaba
  await menu(p, 'Gestion De Clientes'); await p.waitForTimeout(200);
  ok(/\/configuracion\/clientes\/nuevo/.test(p.url()), 'desde el menu vuelve al formulario abierto (no a la lista)');
  ok(await visible(p).locator('.campo:has-text("Nombre") input').first().inputValue() === 'CLIENTE A MEDIO LLENAR', 'el formulario conserva lo escrito');
  // el reporte de cobranzas sigue generado sin volver a consultar
  let consultas = 0; p.on('request', (r) => { if (/\/historico-cobros\?/.test(r.url())) consultas++; });
  await p.locator('.pestana', { hasText: 'Cobranza' }).first().locator('.pestana-link').click(); await p.waitForTimeout(400);
  ok(await visible(p).locator('.tr-tabla tbody tr td').count() > 0 && consultas === 0, 'el reporte sigue generado, sin volver a consultar');
  // Inicio: las pestañas siguen abiertas
  await p.locator('.lateral .menu-item', { hasText: 'Inicio' }).click(); await p.waitForSelector('.dash-cab, .kpi');
  ok(await p.locator('.pestanas').count() === 0, 'en Inicio no se ve la barra de pestañas (como el escritorio)');
  ok((await p.locator('.menu-abiertos .menu-cuenta').innerText()).trim() === '3' && await p.locator('.menu-abiertos.active').count() === 0, '"Programas abiertos" del menu cuenta 3 y no esta seleccionado');
  // menu de un modulo: tampoco se ven las pestañas
  await p.goto(`${BASE}/ventas`); await p.waitForSelector('.mos-tile');
  ok(await p.locator('.pestanas').count() === 0, 'en el menu de un modulo no se ven las pestañas');
  // "Programas abiertos" vuelve al ultimo programa usado, con todo como estaba
  await p.locator('.menu-abiertos').click(); await p.waitForTimeout(400);
  ok(await p.locator('.pestanas').count() === 1 && await p.locator('.pestana').count() === 3, '"Programas abiertos" muestra las pestañas');
  ok(/\/ventas\/reporte-cobranza/.test(p.url()) && await p.locator('.menu-abiertos.active').count() === 1, 'vuelve al ultimo programa usado y queda seleccionado en el menu');
  await p.screenshot({ path: `${OUT}/pestanas.png` });

  console.log('PESTAÑAS EN FILAS (sin achicarse)');
  for (const r of ['/configuracion/roles', '/configuracion/usuarios', '/configuracion/productos', '/configuracion/precios', '/ventas/personal', '/ventas/proveedores', '/ventas/reporte-ventas']) {
    await p.goto(BASE + r); await p.waitForSelector('.pestana-vista:not([hidden]) .modelo, .pestana-vista:not([hidden]) .filtros-reporte', { timeout: 15000 });
  }
  const geo = await p.evaluate(() => {
    const ts = [...document.querySelectorAll('.pestana')].map((t) => t.getBoundingClientRect());
    const l = document.querySelector('.pestanas-lista');
    return { n: ts.length, filas: new Set(ts.map((r) => Math.round(r.top))).size, anchoMin: Math.min(...ts.map((r) => r.width)), scrollX: l.scrollWidth > l.clientWidth + 1, altos: new Set(ts.map((r) => Math.round(r.height))).size };
  });
  ok(geo.n === 10 && geo.filas === 2, `10 pestañas en ${geo.filas} filas`);
  ok(geo.anchoMin >= 150 && geo.altos === 1 && !geo.scrollX, `pestañas sin achicarse; si caben en 2 filas no hay scroll (ancho min ${Math.round(geo.anchoMin)} px)`);
  const tapa = await p.evaluate(() => { const b = document.querySelector('.pestanas').getBoundingClientRect(); const c = document.querySelector('.pestana-vista:not([hidden])').getBoundingClientRect(); return c.top >= b.bottom - 1; });
  ok(tapa, 'el contenido empieza debajo de las pestañas (no queda tapado)');
  await p.screenshot({ path: `${OUT}/pestanas_filas.png` });
  await p.setViewportSize({ width: 820, height: 900 }); await p.waitForTimeout(200);
  const g2 = await p.evaluate(() => { const l = document.querySelector('.pestanas-lista'); const ts = [...document.querySelectorAll('.pestana')].map((t) => t.getBoundingClientRect()); return { filas: new Set(ts.map((r) => Math.round(r.top))).size, alto: l.clientHeight, scrollX: l.scrollWidth > l.clientWidth + 1 }; });
  ok(g2.filas === 2 && g2.scrollX, `en pantalla angosta: 2 filas y el resto con scroll a la derecha (${g2.filas} filas, ${g2.alto} px)`);
  const activaVisible = await p.evaluate(() => { const l = document.querySelector('.pestanas-lista').getBoundingClientRect(); const a = document.querySelector('.pestana.activa').getBoundingClientRect(); return a.left >= l.left - 1 && a.right <= l.right + 1 ? true : JSON.stringify({ l: [l.left, l.right], a: [a.left, a.right], sl: document.querySelector('.pestanas-lista').scrollLeft }); });
  ok(activaVisible === true, `la pestaña activa queda a la vista dentro del scroll ${activaVisible === true ? '' : activaVisible}`);
  await p.screenshot({ path: `${OUT}/pestanas_angosto.png` });
  await p.setViewportSize({ width: 1440, height: 900 });

  console.log('MAXIMIZAR EL PROGRAMA');
  const urlMax = p.url();
  await visible(p).locator('.filtros-reporte input[type=date]').first().fill('2026-01-01');
  await p.locator('.pestanas-max').click(); await p.waitForTimeout(250);
  const vis = (sel) => p.evaluate((s) => { const e = document.querySelector(s); return !!e && getComputedStyle(e).display !== 'none'; }, sel);
  ok(!(await vis('.lateral')) && !(await vis('.cabecera')) && await vis('.pestanas'), 'maximizado: sin menu ni cabecera, con las pestañas');
  const anchoVista = await p.evaluate(() => document.querySelector('.pestana-vista:not([hidden])').getBoundingClientRect().width);
  ok(anchoVista > 1300, `el programa usa todo el ancho (${Math.round(anchoVista)} px)`);
  ok(await visible(p).locator('.filtros-reporte input[type=date]').first().inputValue() === '2026-01-01' && p.url() === urlMax, 'el programa conserva lo que tenia');
  await p.screenshot({ path: `${OUT}/pestanas_maximizado.png` });
  await p.locator('.pestana', { hasText: 'Roles' }).first().locator('.pestana-link').click(); await p.waitForTimeout(250);
  ok(!(await vis('.lateral')), 'al cambiar de programa sigue maximizado');
  await p.locator('.pestanas-max').click(); await p.waitForTimeout(250);
  ok(await vis('.lateral') && await vis('.cabecera') && await p.locator('.menu-abiertos.active').count() === 1, 'Restaurar vuelve al menu con el programa');
  await p.locator('.pestanas-max').click(); await p.waitForTimeout(150);
  await p.locator('.pestana', { hasText: 'Roles' }).first().locator('.pestana-x').click(); await p.waitForTimeout(150);
  await p.goto(`${BASE}/`); await p.waitForSelector('.dash-cab, .kpi');
  ok(await vis('.lateral'), 'en Inicio el menu siempre se ve aunque este maximizado');
  await p.locator('.menu-abiertos').click(); await p.waitForTimeout(300);
  ok(!(await vis('.lateral')), 'al volver a un programa se recuerda maximizado');
  await p.locator('.pestanas-max').click(); await p.waitForTimeout(150);
  await p.locator('.pestanas-todas').click(); await p.waitForTimeout(200);
  ok(await p.locator('[role=alertdialog]').count() === 1, '"Cerrar todas" avisa que hay un registro a medio editar');
  await p.locator('[role=alertdialog] .btn-peligro').click(); await p.waitForTimeout(200);
  ok(await p.locator('.pestana').count() === 0, 'confirmado: se cierran todas');
  await p.goto(`${BASE}/ventas/reporte-cobranza`); await p.waitForSelector('.filtros-reporte');
  await menu(p, 'Crear Nuevos Pedidos'); await visible(p).locator('.grid tbody tr').first().waitFor();
  await visible(p).locator('.grid-herramientas input').fill('TIENDA');
  await menu(p, 'Gestion De Clientes'); await visible(p).locator('.grid tbody tr').first().waitFor();
  await visible(p).locator('.modelo-cab .btn-pri', { hasText: 'Nuevo' }).click(); await visible(p).locator('.registro').waitFor();
  await visible(p).locator('.campo:has-text("Nombre") input').first().fill('CLIENTE A MEDIO LLENAR');

  console.log('CERRAR PESTAÑAS');
  await p.locator('.pestana', { hasText: 'Gestion De Clientes' }).first().locator('.pestana-link').click(); await p.waitForTimeout(200);
  await p.locator('.pestana.activa .pestana-x').click();
  ok(await p.locator('[role=alertdialog]').count() === 1, 'cerrar un registro a medio editar pide confirmacion');
  await p.locator('[role=alertdialog] .btn-sec').click();
  ok(await p.locator('.pestana').count() === 3 && /clientes\/nuevo/.test(p.url()), '"Volver" no cierra nada');
  await p.locator('.pestana.activa .pestana-x').click();
  await p.locator('[role=alertdialog] .btn-peligro').click(); await p.waitForTimeout(250);
  ok(await p.locator('.pestana').count() === 2 && !/clientes/.test(p.url()), '"Cerrar sin grabar" cierra y pasa a otra pestaña');
  // clic con la rueda del mouse
  await p.locator('.pestana', { hasText: 'Crear Nuevos Pedidos' }).first().click({ button: 'middle' }); await p.waitForTimeout(200);
  ok(await p.locator('.pestana').count() === 1, 'clic con la rueda cierra la pestaña');

  console.log('RECARGAR LA PAGINA');
  await menu(p, 'Crear Nuevos Pedidos'); await visible(p).locator('.grid tbody tr').first().waitFor();
  await p.reload(); await p.waitForSelector('.pestana');
  ok(await p.locator('.pestana').count() === 2, 'al recargar se recuerdan los programas abiertos');
  ok(await p.locator('.pestana-vista').count() === 1, 'al recargar solo se carga el programa visible (los demas al tocarlos)');
  await p.locator('.pestanas-todas').click(); await p.waitForTimeout(200);
  ok(await p.locator('.pestana').count() === 0 && new URL(p.url()).pathname === '/', 'Cerrar todas');

  console.log('CELULAR');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  m.on('pageerror', (e) => errs.push(e.message));
  await login(m);
  await m.goto(`${BASE}/ventas/reporte-cobranza`); await m.waitForSelector('.filtros-reporte');
  await m.locator('.filtro-boton').click(); await m.locator('.pestana-vista:not([hidden]) .tr-tabla tbody tr td').first().waitFor();
  await m.locator('.pestana-vista:not([hidden]) .tr-acciones button', { hasText: 'Imprimir' }).click();
  await m.waitForSelector('.vp-fondo .vp-hoja'); await m.waitForTimeout(300);
  const vp = await m.locator('.vp').boundingBox();
  ok(vp.width >= 410 && vp.x <= 1, 'vista previa a pantalla completa');
  const zoom = await m.evaluate(() => Number(document.querySelector('.vp-hoja').contentDocument.documentElement.style.zoom || 1));
  ok(zoom < 1 && zoom > 0.3, `la hoja se achica para verse completa (zoom ${zoom.toFixed(2)})`);
  await m.screenshot({ path: `${OUT}/vista_previa_movil.png` });
  await m.locator('.vp-cerrar').tap();
  ok(await m.locator('.vp-fondo').count() === 0, 'cerrar en celular');
  ok(await m.evaluate(() => document.documentElement.scrollWidth <= document.documentElement.clientWidth), 'sin scroll horizontal con pestañas');

  ok(errs.length === 0, `sin errores ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
