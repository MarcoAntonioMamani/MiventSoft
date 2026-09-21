/* ============================================================================
   AUDITORÍA DE VENTAS - Creación de tablas
   Base de datos: DistribucionDistralKCP2023
   Generado con Claude para Marco Mamani (BANCOSOL)

   Objetivo: reemplazar la reconstrucción manual/indirecta desde
   VentasDetallesBackup (que descubrimos que se llena distinto desde
   MAM_Ventas y desde sp_go_TC004_appMovil, con textos de Accion
   inconsistentes) por dos tablas nuevas, limpias, pensadas para que un
   programa las lea directo sin tener que adivinar nada.

   Sí, la estructura es la misma que te propuse antes — lo único que
   cambió en el camino fue el CONJUNTO DE VALORES que puede tomar
   TipoCambio en VentasAuditoriaDetalle (eso no es un cambio de columnas,
   es solo el catálogo de texto que se guarda ahí). Quedó así:

     - 'AGREGADO'            -> producto que no estaba y ahora sí (solo Modificado)
     - 'ELIMINADO'           -> producto que estaba y ya no (solo Modificado)
     - 'CANTIDAD_MODIFICADA' -> mismo producto, cambió la cantidad (solo Modificado)
     - 'SIN_CAMBIO'          -> mismo producto, sin cambios (solo Modificado,
                                 se guarda para tener el pedido completo, ver
                                 conversación anterior)
     - 'VIGENTE'             -> foto completa del detalle al momento de
                                 Anular o Eliminar (no hay "antes/después" ahí,
                                 solo "esto es lo que tenía el pedido")
   ============================================================================ */

IF OBJECT_ID('dbo.VentasAuditoriaDetalle', 'U') IS NOT NULL DROP TABLE dbo.VentasAuditoriaDetalle;
IF OBJECT_ID('dbo.VentasAuditoriaEventos', 'U') IS NOT NULL DROP TABLE dbo.VentasAuditoriaEventos;
GO

-- ============================================================================
-- Tabla 1: un registro por evento (Anulado / Eliminado / Modificado)
-- ============================================================================
CREATE TABLE dbo.VentasAuditoriaEventos (
    Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    VentaId         INT NOT NULL,
    TipoEvento      NVARCHAR(20) NOT NULL,   -- 'ANULADO' | 'ELIMINADO' | 'MODIFICADO'
    Origen          NVARCHAR(20) NOT NULL,   -- 'DESKTOP' (MAM_Ventas) | 'MOVIL' (sp_go_TC004_appMovil)
    Fecha           DATE NOT NULL,
    Hora            NVARCHAR(5)  NOT NULL,   -- 'HH:MM', igual convención que el resto del sistema
    Usuario         NVARCHAR(150) NULL,
    EstadoAnterior  INT NULL,
    EstadoNuevo     INT NULL,
    AnuladoAnterior INT NULL,
    AnuladoNuevo    INT NULL,
    Observacion     NVARCHAR(450) NULL,
    CONSTRAINT FK_VentasAuditoriaEventos_Ventas
        FOREIGN KEY (VentaId) REFERENCES dbo.Ventas(Id)
);
GO

CREATE INDEX IX_VentasAuditoriaEventos_Fecha_Tipo
    ON dbo.VentasAuditoriaEventos (Fecha, TipoEvento);
GO

CREATE INDEX IX_VentasAuditoriaEventos_VentaId
    ON dbo.VentasAuditoriaEventos (VentaId);
GO

-- ============================================================================
-- Tabla 2: detalle de productos ligado a cada evento (pedido completo)
-- ============================================================================
CREATE TABLE dbo.VentasAuditoriaDetalle (
    Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    EventoId        INT NOT NULL,
    VentaId         INT NOT NULL,
    ProductoId      INT NOT NULL,
    TipoCambio      NVARCHAR(20) NOT NULL,   -- ver catálogo arriba
    CantidadAntes   DECIMAL(18,5) NULL,
    CantidadDespues DECIMAL(18,5) NULL,
    PrecioAntes     DECIMAL(18,4) NULL,
    PrecioDespues   DECIMAL(18,4) NULL,
    CONSTRAINT FK_VentasAuditoriaDetalle_Evento
        FOREIGN KEY (EventoId) REFERENCES dbo.VentasAuditoriaEventos(Id),
    CONSTRAINT FK_VentasAuditoriaDetalle_Producto
        FOREIGN KEY (ProductoId) REFERENCES dbo.Productos(Id)
);
GO

CREATE INDEX IX_VentasAuditoriaDetalle_EventoId
    ON dbo.VentasAuditoriaDetalle (EventoId);
GO

/* ============================================================================
   Prueba rápida después de crear las tablas (deben devolver 0 filas):
   SELECT * FROM dbo.VentasAuditoriaEventos;
   SELECT * FROM dbo.VentasAuditoriaDetalle;

   RECOMENDACIÓN (separación de ambientes): correr esto primero en un
   ambiente de desarrollo/pruebas, no directo en producción, antes de que
   parchemos MAM_Ventas y sp_go_TC004_appMovil para que empiecen a
   escribir aquí.
   ============================================================================ */
