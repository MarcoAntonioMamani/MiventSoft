// =====================================================================
//  Utilidades de los generadores de scripts (server/sql/generar)
//  Conexion: la de server/.env. Con --bd=Nombre se usa otra BD del MISMO servidor
//  (otro servidor: cambiar server/.env, p.ej. con configurar.ps1 -Forzar).
//  Los scripts salen en server/sql/generados/<BD>/ (uno por base de datos).
// =====================================================================
const fs = require('node:fs');
const path = require('node:path');

const bdArg = (process.argv.find((a) => a.startsWith('--bd=')) || '').slice(5);
if (bdArg) process.env.DB_NAME = bdArg;
require('dotenv').config({ path: path.join(__dirname, '..', '..', '.env') });
const MetodoDatos = require('../../src/datos/MetodoDatos');

const NL = '\r\n';
const falla = (m) => { throw new Error(m); };

// Huella del codigo sin mayusculas ni espacios: al hacer ALTER, SQL Server guarda "CREATE" y SSMS puede
// agregar saltos de linea; eso no debe impedir aplicar un script, pero cualquier cambio real en el codigo si.
const HUELLA = (o) => `HASHBYTES('SHA2_256', REPLACE(REPLACE(REPLACE(REPLACE(UPPER(OBJECT_DEFINITION(OBJECT_ID('dbo.${o}'))), CHAR(13), ''), CHAR(10), ''), CHAR(9), ''), ' ', ''))`;

/** Definicion actual y huella de cada objeto en la BD conectada */
async function leerObjetos(nombres) {
    const def = {}; const hash = {};
    for (const o of nombres) {
        const r = (await MetodoDatos.EjecutarConsulta(`SELECT OBJECT_DEFINITION(OBJECT_ID('dbo.${o}')) d, CONVERT(varchar(80), ${HUELLA(o)}, 1) h`))[0];
        if (!r.d) falla(`No existe dbo.${o} en esta base de datos`);
        def[o] = r.d; hash[o] = r.h;
    }
    return { def, hash };
}
async function nombreBD() { return (await MetodoDatos.EjecutarConsulta('SELECT DB_NAME() bd'))[0].bd; }

/** Primer CREATE PROCEDURE / TRIGGER -> ALTER (el resto del texto queda igual) */
function aAlter(s) {
    const re = /CREATE\s+(PROCEDURE|TRIGGER)/i;
    if (!re.test(s)) falla('La definicion no tiene CREATE PROCEDURE / TRIGGER');
    return s.replace(re, (m, t) => `ALTER ${t.toUpperCase()}`).replace(/^\s+/, '');
}
function buscar(s, re, desde = 0, hasta = s.length, nombre = String(re)) {
    const m = re.exec(s.slice(desde, hasta));
    if (!m) falla(`No se encontro ${nombre}: el SP de esta base es distinto, hay que revisarlo a mano`);
    return { idx: desde + m.index, fin: desde + m.index + m[0].length };
}
function insertarDespuesDeLinea(s, idx, texto) { const fin = s.indexOf('\n', idx); return s.slice(0, fin + 1) + texto + s.slice(fin + 1); }

/** Escribe server/sql/generados/<BD>/<archivo> (el codigo de los SP va tal cual, sin tocar sus saltos de linea) */
function escribir(bd, archivo, texto) {
    const ruta = path.join(__dirname, '..', 'generados', bd, archivo); // archivo puede ir en subcarpeta (revertir/...)
    fs.mkdirSync(path.dirname(ruta), { recursive: true });
    fs.writeFileSync(ruta, texto);
    return ruta;
}

module.exports = { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, insertarDespuesDeLinea, escribir };
