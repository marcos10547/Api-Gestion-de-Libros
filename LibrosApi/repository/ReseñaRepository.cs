using System.Data;
using Microsoft.Data.SqlClient; 

public class ReseñaRepository : IReseñaRepository
{
    private readonly string _connectionString;

    public ReseñaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString); 
    }

    private Reseña MapFromReader(SqlDataReader reader)
    {
        return new Reseña
        {
            ReseñaId = (int)reader["ReseñaId"],
            TituloReseña = reader["TituloReseña"] != DBNull.Value ? reader["TituloReseña"].ToString() : null,
            Comentario = reader["Comentario"].ToString()!,
            Puntuacion = (int)reader["Puntuacion"],
            FechaReseña = (DateTime)reader["FechaReseña"],
            EsAprobada = (bool)reader["EsAprobada"],
            IpOrigen = reader["IpOrigen"] != DBNull.Value ? reader["IpOrigen"].ToString() : null,
            Longitud = (int)reader["Longitud"],
            ClienteId = (int)reader["ClienteId"],
            LibroISBN = reader["LibroISBN"].ToString()!
        };
    }

    public Reseña GetById(int id)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM Reseña WHERE ReseñaId = @Id";
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

    public IEnumerable<Reseña> GetAll()
    {
        var lista = new List<Reseña>();
        using (var db = GetConnection())
        {
            db.Open();
            using (var command = db.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Reseña";
                using (var reader = (SqlDataReader)command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapFromReader(reader));
                    }
                }
            }
        }
        return lista;
    }

    public IEnumerable<Reseña> GetByClienteId(int clienteId)
    {
        var lista = new List<Reseña>();
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM Reseña WHERE ClienteId = @ClienteId";
            command.Parameters.Add(new SqlParameter("@ClienteId", clienteId));

            using (var reader = (SqlDataReader)command.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(MapFromReader(reader));
                }
            }
        }
        return lista;
    }

    public IEnumerable<Reseña> GetByLibroISBN(string isbn)
    {
        var lista = new List<Reseña>();
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM Reseña WHERE LibroISBN = @LibroISBN";
            command.Parameters.Add(new SqlParameter("@LibroISBN", isbn));

            using (var reader = (SqlDataReader)command.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(MapFromReader(reader));
                }
            }
        }
        return lista;
    }

    public Reseña Create(Reseña reseña)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                INSERT INTO Reseña (TituloReseña, Comentario, Puntuacion, FechaReseña, EsAprobada, IpOrigen, Longitud, ClienteId, LibroISBN)
                OUTPUT INSERTED.ReseñaId
                VALUES (@Titulo, @Comentario, @Puntuacion, @Fecha, @EsAprobada, @Ip, @Longitud, @ClienteId, @ISBN);
            ";
            
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@Titulo", (object)reseña.TituloReseña! ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Comentario", reseña.Comentario));
                command.Parameters.Add(new SqlParameter("@Puntuacion", reseña.Puntuacion));
                command.Parameters.Add(new SqlParameter("@Fecha", reseña.FechaReseña));
                command.Parameters.Add(new SqlParameter("@EsAprobada", reseña.EsAprobada));
                command.Parameters.Add(new SqlParameter("@Ip", (object)reseña.IpOrigen! ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Longitud", reseña.Longitud));
                command.Parameters.Add(new SqlParameter("@ClienteId", reseña.ClienteId));
                command.Parameters.Add(new SqlParameter("@ISBN", reseña.LibroISBN));

                reseña.ReseñaId = (int)command.ExecuteScalar(); 
                return reseña;
            }
        }
    }

    public bool Delete(int id)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "DELETE FROM Reseña WHERE ReseñaId = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            
            return command.ExecuteNonQuery() > 0;
        }
    }
}