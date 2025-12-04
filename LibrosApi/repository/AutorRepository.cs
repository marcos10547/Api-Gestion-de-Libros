using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; 
using System;

public class AutorRepository : IAutorRepository
{
    private readonly string _connectionString;

    public AutorRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString); 
    }

    private Autor MapFromReader(SqlDataReader reader)
    {
        return new Autor
        {
            AutorId = (int)reader["AutorId"],
            NombreCompleto = reader["NombreCompleto"].ToString(),
            Nacionalidad = reader["Nacionalidad"].ToString(),
            FechaNacimiento = (DateTime)reader["FechaNacimiento"],
            RoyaltyPorcentaje = (decimal)reader["RoyaltyPorcentaje"],
            NumObras = (int)reader["NumObras"],
            EsActivo = (bool)reader["EsActivo"]
        };
    }

    public IEnumerable<Autor> GetAll()
    {
        var autores = new List<Autor>();
        var sql = "SELECT AutorId, NombreCompleto, Nacionalidad, FechaNacimiento, RoyaltyPorcentaje, NumObras, EsActivo FROM Autor";

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
                        autores.Add(MapFromReader(reader));
                    }
                }
            }
        }
        return autores;
    }

    public Autor GetById(int id)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT AutorId, NombreCompleto, Nacionalidad, FechaNacimiento, RoyaltyPorcentaje, NumObras, EsActivo FROM Autor WHERE AutorId = @Id";
            
            var param = command.CreateParameter();
            param.ParameterName = "@Id";
            param.Value = id;
            command.Parameters.Add(param);
            
            using (var reader = (SqlDataReader)command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return MapFromReader(reader);
                }
            }
        }
        return null;
    }
    
    public Autor Create(Autor autor)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                INSERT INTO Autor (NombreCompleto, Nacionalidad, FechaNacimiento, RoyaltyPorcentaje, NumObras, EsActivo)
                OUTPUT INSERTED.AutorId
                VALUES (@NombreCompleto, @Nacionalidad, @FechaNacimiento, @RoyaltyPorcentaje, @NumObras, @EsActivo);
            ";
            
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@NombreCompleto", autor.NombreCompleto));
                command.Parameters.Add(new SqlParameter("@Nacionalidad", autor.Nacionalidad));
                command.Parameters.Add(new SqlParameter("@FechaNacimiento", autor.FechaNacimiento));
                command.Parameters.Add(new SqlParameter("@RoyaltyPorcentaje", autor.RoyaltyPorcentaje));
                command.Parameters.Add(new SqlParameter("@NumObras", autor.NumObras));
                command.Parameters.Add(new SqlParameter("@EsActivo", autor.EsActivo));

                autor.AutorId = (int)command.ExecuteScalar(); 
                return autor;
            }
        }
    }

    public bool Update(Autor autor)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                UPDATE Autor SET 
                    NombreCompleto = @NombreCompleto, 
                    Nacionalidad = @Nacionalidad, 
                    FechaNacimiento = @FechaNacimiento,
                    RoyaltyPorcentaje = @RoyaltyPorcentaje,
                    NumObras = @NumObras,
                    EsActivo = @EsActivo
                WHERE AutorId = @AutorId
            ";
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@AutorId", autor.AutorId));
                command.Parameters.Add(new SqlParameter("@NombreCompleto", autor.NombreCompleto));
                command.Parameters.Add(new SqlParameter("@Nacionalidad", autor.Nacionalidad));
                command.Parameters.Add(new SqlParameter("@FechaNacimiento", autor.FechaNacimiento));
                command.Parameters.Add(new SqlParameter("@RoyaltyPorcentaje", autor.RoyaltyPorcentaje));
                command.Parameters.Add(new SqlParameter("@NumObras", autor.NumObras));
                command.Parameters.Add(new SqlParameter("@EsActivo", autor.EsActivo));
                
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
            command.CommandText = "DELETE FROM Autor WHERE AutorId = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            
            return command.ExecuteNonQuery() > 0;
        }
    }
}