// Monto en letras para la Nota de Venta (equivale a Facturacion.ConvertirLiteral.A_fnConvertirLiteral)
// 1234.5 -> "Mil Doscientos Treinta Y Cuatro con 50/100 Bolivianos"
const U = ['', 'Uno', 'Dos', 'Tres', 'Cuatro', 'Cinco', 'Seis', 'Siete', 'Ocho', 'Nueve', 'Diez', 'Once', 'Doce', 'Trece', 'Catorce', 'Quince',
  'Dieciseis', 'Diecisiete', 'Dieciocho', 'Diecinueve', 'Veinte', 'Veintiuno', 'Veintidos', 'Veintitres', 'Veinticuatro', 'Veinticinco', 'Veintiseis',
  'Veintisiete', 'Veintiocho', 'Veintinueve'];
const D = ['', '', '', 'Treinta', 'Cuarenta', 'Cincuenta', 'Sesenta', 'Setenta', 'Ochenta', 'Noventa'];
const C = ['', 'Ciento', 'Doscientos', 'Trescientos', 'Cuatrocientos', 'Quinientos', 'Seiscientos', 'Setecientos', 'Ochocientos', 'Novecientos'];

function menorMil(n) {
  if (n === 0) return '';
  if (n === 100) return 'Cien';
  const c = Math.floor(n / 100);
  const r = n % 100;
  let t = C[c];
  if (r) {
    const dec = r < 30 ? U[r] : `${D[Math.floor(r / 10)]}${r % 10 ? ` Y ${U[r % 10]}` : ''}`;
    t = t ? `${t} ${dec}` : dec;
  }
  return t;
}

export function enteroALetras(n) {
  n = Math.floor(Math.abs(Number(n) || 0));
  if (n === 0) return 'Cero';
  const millones = Math.floor(n / 1e6);
  const miles = Math.floor((n % 1e6) / 1000);
  const resto = n % 1000;
  const partes = [];
  if (millones) partes.push(millones === 1 ? 'Un Millon' : `${enteroALetras(millones)} Millones`);
  if (miles) partes.push(miles === 1 ? 'Mil' : `${menorMil(miles)} Mil`);
  if (resto) partes.push(menorMil(resto));
  return partes.join(' ').replace(/Uno Mil/g, 'Un Mil').replace(/Uno Millones/g, 'Un Millones');
}

export function montoLiteral(total) {
  const t = Math.round((Number(total) || 0) * 100) / 100;
  const entero = Math.floor(t);
  const cent = Math.round((t - entero) * 100);
  return `${enteroALetras(entero)} con ${String(cent).padStart(2, '0')}/100 Bolivianos`;
}
