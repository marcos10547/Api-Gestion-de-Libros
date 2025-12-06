using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; 
using System;

public class ClienteRepository : IClienteRepository
{
    private readonly string _connectionString;

    public ClienteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString); 
    }
    
    private Cliente MapFromReader(SqlDataReader reader)
    {
        return new Cliente
        {
            ClienteId = (int)reader["ClienteId"],
            Email = reader["Email"].ToString(),
            NombreUsuario = reader["NombreUsuario"].ToString(),
            GastoTotal = (decimal)reader["GastoTotal"],
            FechaRegistro = (DateTime)reader["FechaRegistro"],
            EsPremium = (bool)reader["EsPremium"],
            Nivel = (int)reader["Nivel"]
        };
    }

    public IEnumerable<Cliente> GetAll()
    {
        var clientes = new List<Cliente>();
        var sql = "SELECT ClienteId, Email, NombreUsuario, GastoTotal, FechaRegistro, EsPremium, Nivel FROM Cliente";

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
                        clientes.Add(MapFromReader(reader));
                    }
                }
            }
        }
        return clientes;
    }

    public Cliente GetById(int id)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM Cliente WHERE ClienteId = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            
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
    
    public Cliente Create(Cliente cliente)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                INSERT INTO Cliente (Email, NombreUsuario, GastoTotal, FechaRegistro, EsPremium, Nivel)
                OUTPUT INSERTED.ClienteId
                VALUES (@Email, @NombreUsuario, @GastoTotal, @FechaRegistro, @EsPremium, @Nivel);
            ";
            
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@Email", cliente.Email));
                command.Parameters.Add(new SqlParameter("@NombreUsuario", cliente.NombreUsuario));
                command.Parameters.Add(new SqlParameter("@GastoTotal", cliente.GastoTotal));
                command.Parameters.Add(new SqlParameter("@FechaRegistro", cliente.FechaRegistro));
                command.Parameters.Add(new SqlParameter("@EsPremium", cliente.EsPremium));
                command.Parameters.Add(new SqlParameter("@Nivel", cliente.Nivel));


                cliente.ClienteId = (int)command.ExecuteScalar(); 
                return cliente;
            }
        }
    }

    public bool Update(Cliente cliente)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                UPDATE Cliente SET 
                    Email = @Email, 
                    NombreUsuario = @NombreUsuario, 
                    GastoTotal = @GastoTotal,
                    EsPremium = @EsPremium,
                    Nivel = @Nivel
                WHERE ClienteId = @ClienteId
            ";
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@ClienteId", cliente.ClienteId));
                command.Parameters.Add(new SqlParameter("@Email", cliente.Email));
                command.Parameters.Add(new SqlParameter("@NombreUsuario", cliente.NombreUsuario));
                command.Parameters.Add(new SqlParameter("@GastoTotal", cliente.GastoTotal));
                command.Parameters.Add(new SqlParameter("@EsPremium", cliente.EsPremium));
                command.Parameters.Add(new SqlParameter("@Nivel", cliente.Nivel));
                
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
            command.CommandText = "DELETE FROM Cliente WHERE ClienteId = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            
            return command.ExecuteNonQuery() > 0;
        }
    }
}