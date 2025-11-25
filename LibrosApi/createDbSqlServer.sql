CREATE DATABASE MyLibraryDB;

SELECT name, database_id, create_date
FROM sys.databases
WHERE name = 'MyLibraryDB';

USE MyLibraryDB;

-- 2. CREACIÓN DE TABLAS GENERALES

-- TABLA AUTOR (1/6)
CREATE TABLE Autor (
    AutorId INT PRIMARY KEY IDENTITY(1,1), -- PK
    NombreCompleto NVARCHAR(100) NOT NULL,
    Nacionalidad NVARCHAR(50),
    FechaNacimiento DATETIME NOT NULL,
    RoyaltyPorcentaje DECIMAL(5, 2) NOT NULL,
    NumObras INT NOT NULL DEFAULT 0,
    EsActivo BIT NOT NULL DEFAULT 1
);

-- TABLA GENERO (2/6)
CREATE TABLE Genero (
    GeneroId INT PRIMARY KEY IDENTITY(1,1), -- PK
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(500),
    EsFiccion BIT NOT NULL DEFAULT 1,
    CodigoClasificacion NVARCHAR(10),
    Popularidad INT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME NOT NULL
);

-- TABLA CLIENTE (USUARIO) (3/6)
CREATE TABLE Cliente (
    ClienteId INT PRIMARY KEY IDENTITY(1,1), -- PK (Identificador de Usuario)
    Email NVARCHAR(100) NOT NULL UNIQUE,
    NombreUsuario NVARCHAR(50) NOT NULL,
    GastoTotal DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    FechaRegistro DATETIME NOT NULL,
    EsPremium BIT NOT NULL DEFAULT 0,
    Nivel INT NOT NULL DEFAULT 1
);

-- TABLA LIBRO (4/6) - Contiene FKs a Autor y Genero
CREATE TABLE Libro (
    ISBN NVARCHAR(13) PRIMARY KEY, -- PK (string)
    Titulo NVARCHAR(200) NOT NULL,
    PrecioVenta DECIMAL(8, 2) NOT NULL,
    NumPaginas INT NOT NULL,
    FechaPublicacion DATETIME NOT NULL,
    EsBestSeller BIT NOT NULL DEFAULT 0,
    CostoProduccion DECIMAL(8, 2) NOT NULL,
    
    -- FKs
    AutorId INT NOT NULL,
    GeneroId INT NOT NULL,
    
    CONSTRAINT FK_Libro_Autor FOREIGN KEY (AutorId) REFERENCES Autor(AutorId),
    CONSTRAINT FK_Libro_Genero FOREIGN KEY (GeneroId) REFERENCES Genero(GeneroId)
);

-- 3. CREACIÓN DE TABLAS ASOCIADAS A CLIENTE (RECURSOS ASOCIADOS)

-- TABLA RESEÑA (5/6) - FKs a Libro y Cliente
CREATE TABLE Reseña (
    ReseñaId INT PRIMARY KEY IDENTITY(1,1), -- PK
    TituloReseña NVARCHAR(100),
    Comentario NVARCHAR(1000) NOT NULL,
    Puntuacion INT NOT NULL, -- Implementar validación 1-5 en la API
    FechaReseña DATETIME NOT NULL,
    EsAprobada BIT NOT NULL DEFAULT 0,
    IpOrigen NVARCHAR(45),
    Longitud INT NOT NULL,
    
    -- FKs
    ClienteId INT NOT NULL,
    LibroISBN NVARCHAR(13) NOT NULL,
    
    CONSTRAINT FK_Resena_Cliente FOREIGN KEY (ClienteId) REFERENCES Cliente(ClienteId),
    CONSTRAINT FK_Resena_Libro FOREIGN KEY (LibroISBN) REFERENCES Libro(ISBN)
);

-- TABLA LISTA DESEOS (6/6) - FK a Cliente
CREATE TABLE ListaDeseos (
    ListaId INT PRIMARY KEY IDENTITY(1,1), -- PK
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500),
    FechaCreacion DATETIME NOT NULL,
    CostoEstimadoTotal DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    EsPublica BIT NOT NULL DEFAULT 0,
    NumLibros INT NOT NULL DEFAULT 0,
    
    -- FK
    ClienteId INT NOT NULL,
    
    CONSTRAINT FK_ListaDeseos_Cliente FOREIGN KEY (ClienteId) REFERENCES Cliente(ClienteId)
);

-- 4. TABLA DE UNIÓN (RELACIÓN MUCHOS A MUCHOS)

-- TABLA ListaDeseos_Libro (N:M)
CREATE TABLE ListaDeseos_Libro (
    ListaId INT NOT NULL,
    LibroISBN NVARCHAR(13) NOT NULL,
    
    CONSTRAINT PK_ListaDeseos_Libro PRIMARY KEY (ListaId, LibroISBN),
    
    CONSTRAINT FK_LdL_Lista FOREIGN KEY (ListaId) REFERENCES ListaDeseos(ListaId),
    CONSTRAINT FK_LdL_Libro FOREIGN KEY (LibroISBN) REFERENCES Libro(ISBN)
);
GO