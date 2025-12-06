using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; 
using System.Text;
using System;
using System.Linq;

public class LibroRepository : ILibroRepository
{
    private readonly string _connectionString;

    public LibroRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString); 
    }

    private Libro MapFromReader(SqlDataReader reader)
    {
        return new Libro
        {
            ISBN = reader["ISBN"].ToString()!,
            Titulo = reader["Titulo"].ToString()!,
            PrecioVenta = (decimal)reader["PrecioVenta"],
            NumPaginas = (int)reader["NumPaginas"],
            FechaPublicacion = (DateTime)reader["FechaPublicacion"],
            EsBestSeller = (bool)reader["EsBestSeller"],
            CostoProduccion = (decimal)reader["CostoProduccion"],
            AutorId = (int)reader["AutorId"],
            GeneroId = (int)reader["GeneroId"]
        };
    }

    public IEnumerable<Libro> GetLibros(LibroQueryParams parametros)
    {
        var libros = new List<Libro>();
        var sql = new StringBuilder("SELECT * FROM Libro WHERE 1 = 1");

        if (!string.IsNullOrWhiteSpace(parametros.Titulo))
        {
            sql.Append($" AND Titulo LIKE '%{parametros.Titulo}%' ");
        }

        if (parametros.GeneroId.HasValue)
        {
            sql.Append($" AND GeneroId = {parametros.GeneroId.Value} ");
        }

        if (parametros.PrecioMax.HasValue)
        {
            sql.Append($" AND PrecioVenta <= {parametros.PrecioMax.Value} ");
        }

        var columna = parametros.OrdenarPor ?? "Titulo";
        var dir = parametros.Direccion?.ToLower() == "desc" ? "DESC" : "ASC";
        
        // Validación básica para evitar inyección en ORDER BY
        if (!new[] { "Titulo", "PrecioVenta", "FechaPublicacion" }.Contains(columna))
        {
            columna = "Titulo";
        }

        sql.Append($" ORDER BY {columna} {dir}");
        
        using (var db = GetConnection())
        {
            db.Open();
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql.ToString();
                using (var reader = (SqlDataReader)command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        libros.Add(MapFromReader(reader));
                    }
                }
            }
        }
        return libros;
    }

    public Libro GetByISBN(string isbn)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM Libro WHERE ISBN = @ISBN";
            command.Parameters.Add(new SqlParameter("@ISBN", isbn));
            
            using (var reader = (SqlDataReader)command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return MapFromReader(reader);
                }
            }
        }
        return null!;
    }

    public IEnumerable<Libro> GetAll()
    {
        var libros = new List<Libro>();
        using (var db = GetConnection())
        {
            db.Open();
            using (var command = db.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Libro";
                using (var reader = (SqlDataReader)command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        libros.Add(MapFromReader(reader));
                    }
                }
            }
        }
        return libros;
    }

    public Libro Create(Libro libro)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                INSERT INTO Libro (ISBN, Titulo, PrecioVenta, NumPaginas, FechaPublicacion, EsBestSeller, CostoProduccion, AutorId, GeneroId)
                VALUES (@ISBN, @Titulo, @PrecioVenta, @NumPaginas, @FechaPublicacion, @EsBestSeller, @CostoProduccion, @AutorId, @GeneroId)
            ";
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@ISBN", libro.ISBN));
                command.Parameters.Add(new SqlParameter("@Titulo", libro.Titulo));
                command.Parameters.Add(new SqlParameter("@PrecioVenta", libro.PrecioVenta));
                command.Parameters.Add(new SqlParameter("@NumPaginas", libro.NumPaginas));
                command.Parameters.Add(new SqlParameter("@FechaPublicacion", libro.FechaPublicacion));
                command.Parameters.Add(new SqlParameter("@EsBestSeller", libro.EsBestSeller));
                command.Parameters.Add(new SqlParameter("@CostoProduccion", libro.CostoProduccion));
                command.Parameters.Add(new SqlParameter("@AutorId", libro.AutorId));
                command.Parameters.Add(new SqlParameter("@GeneroId", libro.GeneroId));
                
                command.ExecuteNonQuery();
                return libro;
            }
        }
    }

    public bool Update(Libro libro)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                UPDATE Libro SET 
                    Titulo = @Titulo, 
                    PrecioVenta = @PrecioVenta, 
                    NumPaginas = @NumPaginas, 
                    FechaPublicacion = @FechaPublicacion, 
                    EsBestSeller = @EsBestSeller,
                    CostoProduccion = @CostoProduccion,
                    AutorId = @AutorId, 
                    GeneroId = @GeneroId
                WHERE ISBN = @ISBN
            ";
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@ISBN", libro.ISBN));
                command.Parameters.Add(new SqlParameter("@Titulo", libro.Titulo));
                command.Parameters.Add(new SqlParameter("@PrecioVenta", libro.PrecioVenta));
                command.Parameters.Add(new SqlParameter("@NumPaginas", libro.NumPaginas));
                command.Parameters.Add(new SqlParameter("@FechaPublicacion", libro.FechaPublicacion));
                command.Parameters.Add(new SqlParameter("@EsBestSeller", libro.EsBestSeller));
                command.Parameters.Add(new SqlParameter("@CostoProduccion", libro.CostoProduccion));
                command.Parameters.Add(new SqlParameter("@AutorId", libro.AutorId));
                command.Parameters.Add(new SqlParameter("@GeneroId", libro.GeneroId));
                
                return command.ExecuteNonQuery() > 0;
            }
        }
    }

    public bool Delete(string isbn)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "DELETE FROM Libro WHERE ISBN = @ISBN";
            command.Parameters.Add(new SqlParameter("@ISBN", isbn));
            return command.ExecuteNonQuery() > 0;
        }
    }
}