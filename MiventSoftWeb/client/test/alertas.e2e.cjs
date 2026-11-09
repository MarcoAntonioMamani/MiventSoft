const { chromium } = require('playwright');
(async () => { const b = await chromium.launch(); let mal = 0;
 for (const w of [1440, 1280, 1100, 1024, 900, 768, 390, 320]) { const p = await b.newPage({ viewport:{width:w,height:900} });
  await p.goto('http://localhost:3001/login'); await p.fill('input >> nth=0','admin'); await p.fill('input[type="password"]','123'); await p.keyboard.press('Enter');
  await p.waitForSelector('.alertas'); await p.waitForTimeout(900);
  // numeros grandes como en la BD real
  await p.evaluate(() => document.querySelectorAll('.alertas li b').forEach((b, i) => b.textContent = ['2','307','1.625','32','489'][i]));
  const r = await p.evaluate(() => [...document.querySelectorAll('.alertas li > button')].map(bt => {
    const [ic, tx, n] = bt.children; const a = tx.getBoundingClientRect(), c = n.getBoundingClientRect(), i = ic.getBoundingClientRect();
    return { solapa: a.right > c.left + 0.5 || i.right > a.left + 0.5, ancho: Math.round(a.width), lineas: Math.round(a.height / 17) }; }));
  const peor = r.reduce((m, x) => Math.max(m, x.lineas), 0); const sol = r.some((x) => x.solapa); if (sol || peor > 2) mal++;
  console.log(w, 'solapa', sol, 'max lineas', peor, 'ancho texto min', Math.min(...r.map(x=>x.ancho)));
  const el = p.locator('.alertas').first(); await el.scrollIntoViewIfNeeded(); await el.screenshot({ path: `${process.argv[2] || require('os').tmpdir()}/al_${w}.png` }); await p.close(); }
 console.log('mal', mal); await b.close(); })();
