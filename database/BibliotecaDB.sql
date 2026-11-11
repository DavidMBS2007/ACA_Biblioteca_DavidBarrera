/* =====================================================================
   Sistema de Gestión de Biblioteca - Script SQL Server
   Base de datos: BibliotecaDB
   Contenido: creación de BD, tablas, claves, restricciones, vistas
              y datos de prueba.
   Ejecutar completo en SSMS (o con sqlcmd -i BibliotecaDB.sql).
   ===================================================================== */

IF DB_ID(N'BibliotecaDB') IS NULL
    CREATE DATABASE BibliotecaDB;
GO

USE BibliotecaDB;
GO

/* ---------- Limpieza (permite re-ejecutar el script) ---------- */
IF OBJECT_ID(N'vw_PrestamosDetalle', N'V') IS NOT NULL DROP VIEW vw_PrestamosDetalle;
GO
IF OBJECT_ID(N'Prestamos',   N'U') IS NOT NULL DROP TABLE Prestamos;
IF OBJECT_ID(N'Libros',      N'U') IS NOT NULL DROP TABLE Libros;
IF OBJECT_ID(N'Usuarios',    N'U') IS NOT NULL DROP TABLE Usuarios;
IF OBJECT_ID(N'Editoriales', N'U') IS NOT NULL DROP TABLE Editoriales;
IF OBJECT_ID(N'Autores',     N'U') IS NOT NULL DROP TABLE Autores;
GO

/* ---------- Tablas ---------- */
CREATE TABLE Autores (
    IdAutor      INT IDENTITY(1,1) NOT NULL,
    Nombre       NVARCHAR(60) NOT NULL,
    Apellido     NVARCHAR(60) NOT NULL,
    Nacionalidad NVARCHAR(60) NULL,
    CONSTRAINT PK_Autores PRIMARY KEY (IdAutor),
    CONSTRAINT UQ_Autores_NombreCompleto UNIQUE (Nombre, Apellido)
);

CREATE TABLE Editoriales (
    IdEditorial INT IDENTITY(1,1) NOT NULL,
    Nombre      NVARCHAR(100) NOT NULL,
    Pais        NVARCHAR(60) NULL,
    CONSTRAINT PK_Editoriales PRIMARY KEY (IdEditorial),
    CONSTRAINT UQ_Editoriales_Nombre UNIQUE (Nombre)
);

CREATE TABLE Usuarios (
    IdUsuario INT IDENTITY(1,1) NOT NULL,
    Nombre    NVARCHAR(60)  NOT NULL,
    Apellido  NVARCHAR(60)  NOT NULL,
    Documento NVARCHAR(20)  NOT NULL,
    Telefono  NVARCHAR(20)  NULL,
    Correo    NVARCHAR(100) NULL,
    CONSTRAINT PK_Usuarios PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_Usuarios_Documento UNIQUE (Documento)
);

CREATE TABLE Libros (
    IdLibro         INT IDENTITY(1,1) NOT NULL,
    ISBN            NVARCHAR(20)  NOT NULL,
    Titulo          NVARCHAR(200) NOT NULL,
    IdAutor         INT NOT NULL,
    IdEditorial     INT NOT NULL,
    AnioPublicacion SMALLINT NOT NULL,
    Existencias     INT NOT NULL CONSTRAINT DF_Libros_Existencias DEFAULT (1),
    CONSTRAINT PK_Libros PRIMARY KEY (IdLibro),
    CONSTRAINT UQ_Libros_ISBN UNIQUE (ISBN),
    CONSTRAINT FK_Libros_Autores     FOREIGN KEY (IdAutor)     REFERENCES Autores(IdAutor),
    CONSTRAINT FK_Libros_Editoriales FOREIGN KEY (IdEditorial) REFERENCES Editoriales(IdEditorial),
    CONSTRAINT CK_Libros_Anio        CHECK (AnioPublicacion BETWEEN 1000 AND 2100),
    CONSTRAINT CK_Libros_Existencias CHECK (Existencias >= 0)
);

CREATE TABLE Prestamos (
    IdPrestamo      INT IDENTITY(1,1) NOT NULL,
    IdUsuario       INT NOT NULL,
    IdLibro         INT NOT NULL,
    FechaPrestamo   DATE NOT NULL CONSTRAINT DF_Prestamos_Fecha DEFAULT (CAST(GETDATE() AS DATE)),
    FechaDevolucion DATE NULL,
    Estado          NVARCHAR(10) NOT NULL CONSTRAINT DF_Prestamos_Estado DEFAULT (N'Prestado'),
    CONSTRAINT PK_Prestamos PRIMARY KEY (IdPrestamo),
    CONSTRAINT FK_Prestamos_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario),
    CONSTRAINT FK_Prestamos_Libros   FOREIGN KEY (IdLibro)   REFERENCES Libros(IdLibro),
    CONSTRAINT CK_Prestamos_Estado   CHECK (Estado IN (N'Prestado', N'Devuelto')),
    -- Coherencia entre estado y fecha de devolución
    CONSTRAINT CK_Prestamos_Coherencia CHECK (
        (Estado = N'Prestado' AND FechaDevolucion IS NULL)
        OR
        (Estado = N'Devuelto' AND FechaDevolucion IS NOT NULL AND FechaDevolucion >= FechaPrestamo)
    )
);

CREATE INDEX IX_Libros_Titulo       ON Libros(Titulo);
CREATE INDEX IX_Libros_IdAutor      ON Libros(IdAutor);
CREATE INDEX IX_Libros_IdEditorial  ON Libros(IdEditorial);
CREATE INDEX IX_Prestamos_IdUsuario ON Prestamos(IdUsuario);
CREATE INDEX IX_Prestamos_IdLibro   ON Prestamos(IdLibro, Estado);
GO

/* ---------- Vista usada por el módulo de préstamos y reportes ---------- */
CREATE VIEW vw_PrestamosDetalle
AS
SELECT  p.IdPrestamo,
        p.IdUsuario,
        u.Nombre + N' ' + u.Apellido AS Usuario,
        u.Documento,
        p.IdLibro,
        l.Titulo AS Libro,
        l.ISBN,
        p.FechaPrestamo,
        p.FechaDevolucion,
        p.Estado
FROM    Prestamos p
        INNER JOIN Usuarios u ON u.IdUsuario = p.IdUsuario
        INNER JOIN Libros   l ON l.IdLibro   = p.IdLibro;
GO

/* ---------- Datos de prueba ---------- */
INSERT INTO Autores (Nombre, Apellido, Nacionalidad) VALUES
 (N'Gabriel',    N'García Márquez', N'Colombiana'),
 (N'Isabel',     N'Allende',        N'Chilena'),
 (N'Julio',      N'Cortázar',       N'Argentina'),
 (N'Mario',      N'Vargas Llosa',   N'Peruana'),
 (N'Jorge Luis', N'Borges',         N'Argentina');

INSERT INTO Editoriales (Nombre, Pais) VALUES
 (N'Editorial Sudamericana', N'Argentina'),
 (N'Penguin Random House',   N'España'),
 (N'Alfaguara',              N'España'),
 (N'Planeta',                N'España');

INSERT INTO Usuarios (Nombre, Apellido, Documento, Telefono, Correo) VALUES
 (N'Laura',  N'Gómez',    N'1012345678', N'3001234567', N'laura.gomez@correo.com'),
 (N'Andrés', N'Martínez', N'1023456789', N'3109876543', N'andres.martinez@correo.com'),
 (N'Camila', N'Rojas',    N'1034567890', NULL,          N'camila.rojas@correo.com'),
 (N'Daniel', N'Pérez',    N'1045678901', N'3205558899', NULL),
 (N'Sofía',  N'Ramírez',  N'1056789012', N'3157771122', N'sofia.ramirez@correo.com');

-- Existencias iniciales (antes de descontar los préstamos activos)
INSERT INTO Libros (ISBN, Titulo, IdAutor, IdEditorial, AnioPublicacion, Existencias) VALUES
 (N'978-0-0000-0001-1', N'Cien años de soledad',            1, 1, 1967, 3),
 (N'978-0-0000-0002-2', N'El amor en los tiempos del cólera',1, 2, 1985, 2),
 (N'978-0-0000-0003-3', N'La casa de los espíritus',         2, 2, 1982, 2),
 (N'978-0-0000-0004-4', N'Eva Luna',                         2, 3, 1987, 4),
 (N'978-0-0000-0005-5', N'Rayuela',                          3, 1, 1963, 3),
 (N'978-0-0000-0006-6', N'La ciudad y los perros',           4, 3, 1963, 2),
 (N'978-0-0000-0007-7', N'Ficciones',                        5, 4, 1944, 1),
 (N'978-0-0000-0008-8', N'El Aleph',                         5, 4, 1949, 3);

INSERT INTO Prestamos (IdUsuario, IdLibro, FechaPrestamo, FechaDevolucion, Estado) VALUES
 (1, 1, '2026-10-20', NULL,         N'Prestado'),
 (2, 3, '2026-10-25', NULL,         N'Prestado'),
 (3, 5, '2026-11-02', NULL,         N'Prestado'),
 (4, 2, '2026-09-10', '2026-09-24', N'Devuelto'),
 (1, 6, '2026-09-15', '2026-09-29', N'Devuelto');

-- Descontar de las existencias los libros que están actualmente prestados
UPDATE l
SET    l.Existencias = l.Existencias - x.Cantidad
FROM   Libros l
       INNER JOIN (SELECT IdLibro, COUNT(*) AS Cantidad
                   FROM Prestamos WHERE Estado = N'Prestado'
                   GROUP BY IdLibro) x ON x.IdLibro = l.IdLibro;
GO

PRINT N'BibliotecaDB creada correctamente.';
GO
