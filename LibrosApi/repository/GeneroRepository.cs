using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; 
using System;

public class GeneroRepository : IGeneroRepository
{
    private readonly string _connectionString;

    public GeneroRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString); 
    }

    private Genero MapFromReader(SqlDataReader reader)
    {
        return new Genero
        {
            GeneroId = (int)reader["GeneroId"],
            Nombre = reader["Nombre"].ToString()!,
            Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : null,
            EsFiccion = (bool)reader["EsFiccion"],
            CodigoClasificacion = reader["CodigoClasificacion"] != DBNull.Value ? reader["CodigoClasificacion"].ToString() : null,
            Popularidad = (int)reader["Popularidad"],
            FechaCreacion = (DateTime)reader["FechaCreacion"]
        };
    }

    public IEnumerable<Genero> GetAll()
    {
        var generos = new List<Genero>();
        var sql = "SELECT * FROM Genero";

        using (var db = GetConnection())
        {
            db.Open();
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                using (var reader = (SqlDataReader)command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        generos.Add(MapFromReader(reader));
                    }
                }
            }
        }
        return generos;
    }

    public Genero GetById(int id)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM Genero WHERE GeneroId = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            
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
    
    public Genero Create(Genero genero)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                INSERT INTO Genero (Nombre, Descripcion, EsFiccion, CodigoClasificacion, Popularidad, FechaCreacion)
                OUTPUT INSERTED.GeneroId
                VALUES (@Nombre, @Descripcion, @EsFiccion, @CodigoClasificacion, @Popularidad, @FechaCreacion);
            ";
            
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@Nombre", genero.Nombre));
                command.Parameters.Add(new SqlParameter("@Descripcion", (object)genero.Descripcion! ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@EsFiccion", genero.EsFiccion));
                command.Parameters.Add(new SqlParameter("@CodigoClasificacion", (object)genero.CodigoClasificacion! ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Popularidad", genero.Popularidad));
                command.Parameters.Add(new SqlParameter("@FechaCreacion", genero.FechaCreacion));

                genero.GeneroId = (int)command.ExecuteScalar(); 
                return genero;
            }
        }
    }

    public bool Update(Genero genero)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                UPDATE Genero SET 
                    Nombre = @Nombre, 
                    Descripcion = @Descripcion, 
                    EsFiccion = @EsFiccion,
                    CodigoClasificacion = @CodigoClasificacion,
                    Popularidad = @Popularidad
                WHERE GeneroId = @GeneroId
            ";
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@GeneroId", genero.GeneroId));
                command.Parameters.Add(new SqlParameter("@Nombre", genero.Nombre));
                command.Parameters.Add(new SqlParameter("@Descripcion", (object)genero.Descripcion! ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@EsFiccion", genero.EsFiccion));
                command.Parameters.Add(new SqlParameter("@CodigoClasificacion", (object)genero.CodigoClasificacion! ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Popularidad", genero.Popularidad));
                
                return command.ExecuteNonQuery() > 0;
            }
        }
    }

    public bool Delete(int id)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "DELETE FROM Genero WHERE GeneroId = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            
            return command.ExecuteNonQuery() > 0;
        }
    }
}