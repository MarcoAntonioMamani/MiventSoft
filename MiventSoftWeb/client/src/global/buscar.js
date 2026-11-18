// =====================================================================
//  Busqueda por palabras (todos los combos / buscadores)
//  - cada palabra escrita debe aparecer en el texto, en CUALQUIER orden
//  - sin distinguir mayusculas ni tildes ("cafe" encuentra "CAFÉ")
//  Uso: const w = palabrasDe(q); lista.filter((x) => coincide(`${x.Id} ${x.Nombre}`, w))
// =====================================================================
export const normalizar = (s) => String(s ?? '').normalize('NFD').replace(/[̀-ͯ]/g, '').toUpperCase();

export const palabrasDe = (q) => normalizar(q).split(/[\s,;]+/).filter(Boolean);

// palabras: arreglo de palabrasDe() (o el texto de busqueda directamente)
export function coincide(texto, palabras) {
  const w = Array.isArray(palabras) ? palabras : palabrasDe(palabras);
  if (!w.length) return true;
  const t = normalizar(texto);
  return w.every((p) => t.includes(p));
}

// Listas grandes (buscadores de productos): el texto de cada fila se normaliza UNA vez al cargar,
// no en cada tecla. indice = indexar(lista, (x) => `${x.Codigo} ${x.Nombre}`); filtrarIndice(indice, palabrasDe(q))
export const indexar = (lista, texto) => (lista || []).map((x) => [normalizar(texto(x)), x]);
export function filtrarIndice(indice, palabras) {
  if (!palabras.length) return indice.map((par) => par[1]);
  const r = [];
  for (const [t, x] of indice) if (palabras.every((p) => t.includes(p))) r.push(x);
  return r;
}
