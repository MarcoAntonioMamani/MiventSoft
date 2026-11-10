// =====================================================================
//  Datos/DParametro.js   (equivalente a Datos/DParametro.vb)
//
//  new DParametro("@tipo", 3)
//  new DParametro("@RolDetalleType", "", detalle)      <- tabla (TVP)
//
//  "detalle" equivale al DataTable que pasabas como SqlDbType.Structured:
//      { columnas: ['Id','RolId',...], filas: [ {Id:1, RolId:2, ...}, ... ] }
//  Igual que ADO.NET, las columnas se envian POR POSICION (ordinal) al tipo tabla,
//  asi que el orden de "columnas" debe ser el mismo que usabas en DefaultView.ToTable(...)
// =====================================================================
class DParametro {
    constructor(nom, val, _detalle = null) {
        this.nombre = nom;
        this.valor = val;
        this.detalle = _detalle;
    }
}

module.exports = DParametro;
