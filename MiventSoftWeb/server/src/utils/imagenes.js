// =====================================================================
//  Imagenes (equivale a _fnCopiarImagenRutaDefinida / _fnMoverImagenRuta)
//  Se guardan en la MISMA carpeta que usa el escritorio, con el MISMO nombre:
//     <gs_CarpetaRaiz>\Imagenes\Imagenes Categoria\Imagen_HHmmss_ddMMyyyy.jpg
//  y en BD se graba "\Imagen_....jpg" igual que en VB, asi web y escritorio
//  comparten las imagenes.
// =====================================================================
const fs = require('node:fs');
const path = require('node:path');
const multer = require('multer');
const Configuracion = require('../datos/Configuracion');
const { ErrorNegocio } = require('../middleware/errores');

const CARPETAS = {
    categoria: 'Imagenes Categoria',
    empresa: 'Imagenes Empresa',
    producto: path.join('Imagenes Productos', 'ProductosTodos'),
};

const IMAGEN_DEFECTO = 'Default.jpg';

const subirImagen = multer({
    storage: multer.memoryStorage(),
    limits: { fileSize: 3 * 1024 * 1024, files: 1 },
}).single('imagen');

// Productos: varias imagenes (TCL0064)
const subirImagenes = multer({
    storage: multer.memoryStorage(),
    limits: { fileSize: 3 * 1024 * 1024, files: 10 },
}).array('imagenes', 10);

// Valida por "magic bytes", no por la extension que manda el navegador
function _esImagenValida(buf) {
    if (!buf || buf.length < 4) return false;
    const jpg = buf[0] === 0xff && buf[1] === 0xd8 && buf[2] === 0xff;
    const png = buf[0] === 0x89 && buf[1] === 0x50 && buf[2] === 0x4e && buf[3] === 0x47;
    const gif = buf[0] === 0x47 && buf[1] === 0x49 && buf[2] === 0x46;
    const bmp = buf[0] === 0x42 && buf[1] === 0x4d;
    return jpg || png || gif || bmp;
}

function _dos(n) {
    return String(n).padStart(2, '0');
}

// Igual que P_fnObtenerID() de VB: HHmmss_ddMMyyyy
function P_fnObtenerID() {
    const d = new Date();
    return `${_dos(d.getHours())}${_dos(d.getMinutes())}${_dos(d.getSeconds())}_${_dos(d.getDate())}${_dos(d.getMonth() + 1)}${d.getFullYear()}`;
}

function RutaCarpeta(tipo) {
    const sub = CARPETAS[tipo];
    if (!sub) throw new ErrorNegocio('Tipo de imagen no valido');
    return path.join(Configuracion.gs_CarpetaRaiz, 'Imagenes', sub);
}

/** Guarda el archivo subido y devuelve el nombre a grabar en BD ("\Imagen_xxx.jpg") */
async function GuardarImagen(tipo, archivo) {
    if (!archivo) return null;
    if (!_esImagenValida(archivo.buffer)) {
        throw new ErrorNegocio('El archivo no es una imagen valida (JPG, PNG, GIF o BMP).');
    }
    const carpeta = RutaCarpeta(tipo);
    await fs.promises.mkdir(carpeta, { recursive: true });

    let base = `Imagen_${P_fnObtenerID()}`;
    let n = 0;
    while (fs.existsSync(path.join(carpeta, `${base}${n ? '_' + n : ''}.jpg`))) n++;
    if (n) base = `${base}_${n}`;

    await fs.promises.writeFile(path.join(carpeta, `${base}.jpg`), archivo.buffer);
    return `\\${base}.jpg`;
}

// Productos: igual que Tec_Productos.vb -> "\Imagen_" + dia+mes+anio+hora+min+seg (sin ceros)
function NombreImagenProducto(indice = 0) {
    const d = new Date();
    const base = `Imagen_${d.getDate()}${d.getMonth() + 1}${d.getFullYear()}${d.getHours()}${d.getMinutes()}${d.getSeconds()}`;
    return `\\${base}${indice ? '_' + indice : ''}.jpg`;
}

function ValidarImagen(archivo) {
    if (!_esImagenValida(archivo?.buffer)) {
        throw new ErrorNegocio('El archivo no es una imagen valida (JPG, PNG, GIF o BMP).');
    }
}

async function GuardarImagenConNombre(tipo, nombreBD, buffer) {
    const carpeta = RutaCarpeta(tipo);
    await fs.promises.mkdir(carpeta, { recursive: true });
    const archivo = path.join(carpeta, path.basename(String(nombreBD).replace(/\\/g, '/')));
    if (!archivo.startsWith(carpeta)) throw new ErrorNegocio('Nombre de imagen no valido');
    await fs.promises.writeFile(archivo, buffer);
}

async function EliminarImagen(tipo, nombreBD) {
    const ruta = RutaImagen(tipo, nombreBD);
    if (ruta) await fs.promises.unlink(ruta).catch(() => {});
}

/** Resuelve la ruta fisica de una imagen guardada en BD, bloqueando path traversal */
function RutaImagen(tipo, nombreBD) {
    const limpio = path.basename(String(nombreBD || '').replace(/\\/g, '/'));
    if (!limpio || limpio.toLowerCase() === IMAGEN_DEFECTO.toLowerCase()) return null;
    const carpeta = RutaCarpeta(tipo);
    const ruta = path.join(carpeta, limpio);
    if (!ruta.startsWith(carpeta)) return null;
    return fs.existsSync(ruta) ? ruta : null;
}

module.exports = { subirImagen, subirImagenes, NombreImagenProducto, ValidarImagen, GuardarImagenConNombre, EliminarImagen, GuardarImagen, RutaImagen, P_fnObtenerID, IMAGEN_DEFECTO };
