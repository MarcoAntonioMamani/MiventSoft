// Equivalente a Modelo/Celda.vb:  New Celda("Id", True, "ID", 40)
// tipo opcional: 'bool' | 'num' | 'texto'  (en VB se deducia del DataColumn)
export class Celda {
  constructor(campo, visible, titulo = '', tamano = 100, formato = '', tipo = '') {
    this.campo = campo;
    this.visible = visible;
    this.titulo = titulo;
    this.tamano = tamano;
    this.formato = formato;
    this.tipo = tipo;
  }
}
