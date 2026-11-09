// Menu lateral en acordeon: un solo modulo desplegado a la vez.
// Uso: con el servidor en modo simulador (node src/app.js --mock):  node test/menu_acordeon.e2e.cjs [carpetaCapturas]
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
  const abiertos = (p) => p.locator('.lateral .menu-sub.abierto').count();
  const nombreAbierto = (p) => p.locator('.lateral .menu-grupo:has(.menu-sub.abierto) .menu-mod-link').innerText().catch(() => '');

  const p = await b.newPage({ viewport: { width: 1440, height: 900 } });
  p.on('pageerror', (e) => errs.push(e.message));
  p.on('console', (m) => { if (m.type() === 'error' && !/Failed to load resource/.test(m.text())) errs.push(m.text()); }); // avisos de React (ej. atributo inert)
  await login(p);

  console.log('PC');
  ok(await abiertos(p) <= 1, 'al entrar, a lo sumo un modulo desplegado');
  const flechas = p.locator('.lateral .menu-mod-flecha');
  const n = await flechas.count();
  ok(n >= 3, `${n} modulos en el menu`);
  await flechas.nth(0).click(); await p.waitForTimeout(350);
  const primero = await nombreAbierto(p);
  ok(await abiertos(p) === 1, `abre el 1er modulo (${primero.trim()})`);
  await flechas.nth(1).click(); await p.waitForTimeout(350);
  ok(await abiertos(p) === 1 && (await nombreAbierto(p)) !== primero, 'al abrir el 2do, el 1ro se pliega');
  await flechas.nth(1).click(); await p.waitForTimeout(350);
  ok(await abiertos(p) === 0, 'tocar el abierto lo pliega');
  ok(await p.locator('.lateral .menu-sub:not(.abierto) a').first().evaluate((a) => a.closest('[inert]') !== null), 'los programas plegados no reciben foco con Tab (inert)');

  // entrar a un programa de otro modulo: se despliega su modulo y se pliega el resto
  await flechas.nth(0).click(); await p.waitForTimeout(300);
  await p.goto(`${BASE}/ventas/ventas`); await p.waitForSelector('.grid tbody tr');
  await p.waitForTimeout(350);
  ok(await abiertos(p) === 1 && /ventas|pedidos/i.test(await nombreAbierto(p)), `al abrir Ventas se despliega solo su modulo (${(await nombreAbierto(p)).trim()})`);
  ok(await p.locator('.lateral .menu-sub.abierto .menu-item.sub.active').count() === 1, 'programa activo resaltado');
  // navegar con el menu a un programa de otro modulo
  const idxConf = await p.locator('.lateral .menu-grupo .menu-mod-link').evaluateAll((xs) => xs.findIndex((x) => /configuraci/i.test(x.textContent)));
  await flechas.nth(idxConf).click(); await p.waitForTimeout(350);
  ok(await abiertos(p) === 1 && /configuraci/i.test(await nombreAbierto(p)), 'abrir Configuración pliega Ventas');
  await p.locator('.lateral .menu-sub.abierto a.menu-item.sub').first().click();
  await p.waitForTimeout(500);
  ok(await abiertos(p) === 1 && /configuraci/i.test(await nombreAbierto(p)), 'al entrar al programa queda solo Configuración');
  // clic en el nombre del modulo
  const idxVen = await p.locator('.lateral .menu-grupo .menu-mod-link').evaluateAll((xs) => xs.findIndex((x) => /ventas|pedidos/i.test(x.textContent)));
  await p.locator('.lateral .menu-grupo .menu-mod-link').nth(idxVen).click(); await p.waitForTimeout(500);
  ok(await abiertos(p) === 1 && /ventas|pedidos/i.test(await nombreAbierto(p)), 'clic en el nombre del modulo: lo despliega y pliega el resto');
  await p.screenshot({ path: `${OUT}/menu_acordeon.png` });

  console.log('CELULAR');
  const ctx = await b.newContext({ ...devices['Pixel 7'] });
  const m = await ctx.newPage();
  m.on('pageerror', (e) => errs.push(e.message));
  await login(m);
  await m.locator('.cabecera .solo-movil').click(); await m.waitForTimeout(300);
  const fm = m.locator('.lateral .menu-mod-flecha');
  await fm.nth(0).click(); await m.waitForTimeout(300);
  await fm.nth(2).click(); await m.waitForTimeout(300);
  ok(await abiertos(m) === 1, 'en celular tambien un solo modulo desplegado');

  ok(errs.length === 0, `sin errores ${errs.join(' | ')}`);
  await b.close();
  console.log(fallos ? `\n${fallos} FALLA(S)` : '\nTODO OK');
  process.exit(fallos ? 1 : 0);
})();
