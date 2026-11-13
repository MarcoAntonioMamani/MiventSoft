// =====================================================================
//  Genera EliminarRegistros.sql y EliminarRegistros_REVERTIR.sql para la BD conectada
//    node sql/generar/eliminar_registros.js                 (BD de server/.env)
//    node sql/generar/eliminar_registros.js --bd=OtraBD     (otra BD del mismo servidor)
//  Agrega Usuarios.EliminarRegistros ("Puede eliminar registros") y lo graba / lista en MAM_Usuarios.
//  Lee MAM_Usuarios de ESA base y verifica cada punto de cambio: si el SP es distinto, no genera nada.
// =====================================================================
const { MetodoDatos, NL, falla, HUELLA, leerObjetos, nombreBD, aAlter, buscar, escribir } = require('./comun');

const SP = 'MAM_Usuarios';
const MARCA = 'Kaily: eliminar registros';
const COLUMNAS = 'RolId, NombreUsuario, Contrasena, Estado, SucursalId, FechaRegistro, HoraRegistro, UsuarioRegistro, IdEmpresa, IdPersonal, '
    + 'ModificarPrecioVenta, AplicarDescuentoVenta, VerTodasVentas, VerTodosVentasCierre, ModificarRegistroAnteriores';

function reemplazar(s, re, nombre, f) {
    const m = buscar(s, re, 0, s.length, nombre);
    return s.slice(0, m.idx) + f(s.slice(m.idx, m.fin)) + s.slice(m.fin);
}

function aplicarCambios(s) {
    if (s.includes(MARCA)) falla(`${SP} ya tiene "${MARCA}": esta BD ya esta instalada`);
    // 1. parametro nuevo (-1 = no enviado: el escritorio anterior no lo envia)
    s = reemplazar(s, /@ModificarRegistroAntiguos\s+int\s*=\s*-1/i, 'el parametro @ModificarRegistroAntiguos',
        (x) => `${x}, @EliminarRegistros int=-1 /* ${MARCA} */`);
    // 2. INSERT con lista de columnas (el VALUES posicional se rompe al agregar una columna). No enviado = no puede eliminar
    s = reemplazar(s, /INSERT\s+INTO\s+Usuarios\s+VALUES\s*\(\s*@RolId[\s\S]*?@ModificarRegistroAntiguos\s*\)/i, 'el INSERT INTO Usuarios',
        (x) => {
            const valores = x.slice(x.indexOf('(') + 1, x.lastIndexOf(')')).trim();
            return `-- ${MARCA}: lista de columnas; si no se envia, no puede eliminar${NL}\t\t\tINSERT INTO Usuarios (${COLUMNAS}, EliminarRegistros)${NL}`
                + `\t\t\tVALUES(${valores}, IIF(@EliminarRegistros = 1, 1, 0))`;
        });
    // 3. UPDATE: si no se envia (-1) conserva el valor actual
    s = reemplazar(s, /ModificarRegistroAnteriores\s*=\s*@ModificarRegistroAntiguos/i, 'el UPDATE de ModificarRegistroAnteriores',
        (x) => `${x},${NL}\t\t\t\t\t\t\t EliminarRegistros = IIF(@EliminarRegistros = -1, EliminarRegistros, @EliminarRegistros) /* ${MARCA} */`);
    // 4. @tipo 3 (lista de usuarios)
    s = reemplazar(s, /isnull\s*\(\s*u\.ModificarRegistroAnteriores\s*,\s*0\s*\)\s+as\s+ModificarRegistroAnteriores/i, 'la columna ModificarRegistroAnteriores de @tipo 3',
        (x) => `${x},${NL}\t\tisnull(u.EliminarRegistros,0) as EliminarRegistros /* ${MARCA} */`);
    return s;
}

(async () => {
    const bd = await nombreBD();
    const { def, hash } = await leerObjetos([SP]);
    const tieneColumna = (await MetodoDatos.EjecutarConsulta("SELECT COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') c"))[0].c != null;
    if (tieneColumna) falla('Usuarios ya tiene la columna EliminarRegistros: esta BD ya esta instalada');
    const nuevo = aplicarCambios(def[SP]);
    const hoy = new Date().toISOString().slice(0, 10);
    const instalado = `OBJECT_DEFINITION(OBJECT_ID('dbo.${SP}')) LIKE '%${MARCA}%'`;
    const cab = [
        '/* =====================================================================',
        '   06 - PUEDE ELIMINAR REGISTROS (permiso por usuario; web y escritorio usan el mismo SP)',
        '',
        '   1. Usuarios.EliminarRegistros (int): 1 = puede eliminar ventas, compras, movimientos de inventario',
        '      (ingresos / salidas / traspasos) y pagos de cobranzas y de cuentas por pagar.',
        '   2. Los usuarios que YA existen quedan con 0 (SIN el permiso). Despues, en Usuarios,',
        '      se le habilita a quien si debe eliminar. Los usuarios nuevos tambien empiezan SIN el permiso.',
        `   3. ${SP}: @EliminarRegistros (-1 = no enviado). Al modificar desde un escritorio que todavia no`,
        '      tiene la casilla, conserva el valor actual (no lo borra). El INSERT ahora lleva lista de columnas.',
        '   Todo lo demas del SP queda IDENTICO a la version de la BD.',
        '',
        `   Generado desde ${bd} (${hoy}). PROTECCION: solo se aplica si ${SP} es esa version (sin contar`,
        '   mayusculas ni espacios). Si ya esta instalado, avisa y no hace nada. Si es otra version, da error',
        '   y no modifica nada. Columna y SP van en UNA transaccion: o queda todo o no queda nada.',
        '   VOLVER ATRAS: revertir\\REVERTIR_06_EliminarRegistros.sql (SOLO para deshacer).',
        '   ===================================================================== */',
        'SET NOEXEC OFF',
        'GO',
        `IF ${instalado}`,
        'BEGIN',
        "\tRAISERROR('EliminarRegistros YA ESTA INSTALADO en esta base. No se modifico nada.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        `ELSE IF ISNULL(${HUELLA(SP)}, 0x) <> ${hash[SP]}`,
        'BEGIN',
        `\tRAISERROR('EliminarRegistros: ${SP} es OTRA version (no la de ${bd} ${hoy}). No se modifico nada.', 16, 1)`,
        '\tSET NOEXEC ON',
        'END',
        'GO',
        'SET XACT_ABORT ON',
        'BEGIN TRAN',
        "IF COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NULL",
        '\tALTER TABLE dbo.Usuarios ADD EliminarRegistros int NULL',
        'GO',
        '-- los que ya existen quedan SIN el permiso (se habilita a mano en Usuarios)',
        "EXEC('UPDATE dbo.Usuarios SET EliminarRegistros = 0 WHERE EliminarRegistros IS NULL')",
        'GO',
    ].join(NL);
    const fin = [
        // si algo fallo (SP sin la marca o sin columna) se deshace todo: el SP anterior no puede convivir con la columna
        'IF @@TRANCOUNT > 0',
        'BEGIN',
        `\tIF ${instalado} AND COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NOT NULL`,
        "\tBEGIN COMMIT TRAN; RAISERROR('EliminarRegistros INSTALADO.', 10, 1) WITH NOWAIT END",
        '\tELSE',
        "\tBEGIN ROLLBACK TRAN; RAISERROR('EliminarRegistros: fallo la instalacion. Se deshizo todo, no se modifico nada.', 16, 1) END",
        'END',
        `ELSE IF NOT (${instalado}) OR COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NULL`,
        "\tRAISERROR('EliminarRegistros: fallo la instalacion (revisar los mensajes de arriba).', 16, 1)",
        'GO',
        'SET NOEXEC OFF',
        'GO',
    ].join(NL);
    const r1 = escribir(bd, '06_EliminarRegistros.sql', `${cab}${NL}${aAlter(nuevo)}${NL}GO${NL}${fin}${NL}`);

    const rev = [
        '/* =====================================================================',
        '   SOLO PARA DESHACER 06_EliminarRegistros.sql. NO es parte de la instalacion: NO ejecutar al instalar.',
        `   Vuelve ${SP} de ${bd} a la version anterior y QUITA la columna Usuarios.EliminarRegistros`,
        '   (el SP anterior inserta sin lista de columnas y no funciona con la columna de mas).',
        '   Antes: volver a la version anterior de la web y del escritorio. Si no esta instalado, avisa y no hace nada.',
        '   ===================================================================== */',
        'SET NOEXEC OFF', 'GO',
        `IF NOT (${instalado})`,
        'BEGIN',
        "\tRAISERROR('EliminarRegistros NO esta instalado en esta base: no hay nada que deshacer.', 10, 1) WITH NOWAIT",
        '\tSET NOEXEC ON',
        'END',
        'GO',
        'SET XACT_ABORT ON',
        'BEGIN TRAN',
        // primero la columna: el SP anterior (INSERT sin lista de columnas) no compila con la columna de mas
        "IF COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NOT NULL",
        "\tEXEC('ALTER TABLE dbo.Usuarios DROP COLUMN EliminarRegistros')",
        'GO',
        `${aAlter(def[SP])}${NL}GO`,
        'IF @@TRANCOUNT > 0',
        'BEGIN',
        `\tIF NOT (${instalado}) AND COL_LENGTH('dbo.Usuarios', 'EliminarRegistros') IS NULL`,
        "\tBEGIN COMMIT TRAN; RAISERROR('EliminarRegistros DESHECHO.', 10, 1) WITH NOWAIT END",
        '\tELSE',
        "\tBEGIN ROLLBACK TRAN; RAISERROR('No se pudo deshacer EliminarRegistros. No se modifico nada.', 16, 1) END",
        'END',
        'GO',
        'SET NOEXEC OFF', 'GO'].join(NL);
    const r2 = escribir(bd, 'revertir/REVERTIR_06_EliminarRegistros.sql', `${rev}${NL}`);
    console.log(`OK (${bd}):\n  ${r1}\n  ${r2}`);
    await MetodoDatos.CerrarConexion();
})().catch(async (e) => { console.error(`NO SE GENERO NADA: ${e.message}`); await MetodoDatos.CerrarConexion().catch(() => {}); process.exit(1); });
