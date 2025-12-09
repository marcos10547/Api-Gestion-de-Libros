using System.Data;
using Microsoft.Data.SqlClient;

public class OpinionRepository : IOpinionRepository
{
    private readonly string _connectionString;

    public OpinionRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    private Opiniones MapFromReader(SqlDataReader reader)
    {
        return new Opiniones
        {
            ReseñaId = (int)reader["ReseñaId"],
            TituloReseña = reader["TituloReseña"] != DBNull.Value ? reader["TituloReseña"].ToString() : null,
            Comentario = reader["Comentario"].ToString(),
            Puntuacion = (int)reader["Puntuacion"],
            FechaReseña = (DateTime)reader["FechaReseña"],
            NombreCliente = reader["TituloReseña"] != DBNull.Value ? reader["TituloReseña"].ToString() : null,
            ClienteId = (int)reader["ClienteId"],
            LibroISBN = reader["LibroISBN"].ToString()
        };
    }

    public Opiniones GetById(int id) 
    {
        using var db = GetConnection(); db.Open();
        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Opiniones WHERE ReseñaId = @Id";
        cmd.Parameters.Add(new SqlParameter("@Id", id));
        using var r = (SqlDataReader)cmd.ExecuteReader();
        return r.Read() ? MapFromReader(r) : null;
    }
    
    public IEnumerable<Opiniones> GetAll() 
    {
        var l = new List<Opiniones>();
        using var db = GetConnection(); db.Open();
        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Opiniones";
        using var r = (SqlDataReader)cmd.ExecuteReader();
        while(r.Read()) l.Add(MapFromReader(r));
        return l;
    }

    public Opiniones Create(Opiniones reseña)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                INSERT INTO Opiniones (TituloReseña, Comentario, Puntuacion, FechaReseña, ClienteId, LibroISBN)
                OUTPUT INSERTED.ReseñaId
                VALUES (@Titulo, @Comentario, @Puntuacion, @Fecha, @ClienteId, @ISBN);
            ";
            
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@Titulo", (object)reseña.TituloReseña! ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Comentario", reseña.Comentario));
                command.Parameters.Add(new SqlParameter("@Puntuacion", reseña.Puntuacion));
                command.Parameters.Add(new SqlParameter("@Fecha", reseña.FechaReseña));
                command.Parameters.Add(new SqlParameter("@ClienteId", reseña.ClienteId));
                command.Parameters.Add(new SqlParameter("@ISBN", reseña.LibroISBN));

                reseña.ReseñaId = (int)command.ExecuteScalar(); 
                return reseña;
            }
        }
    }


    public bool Update(Opiniones r)
    {
        using var db = GetConnection(); db.Open();
        var cmd = db.CreateCommand();
        cmd.CommandText = "UPDATE Opiniones SET TituloReseña=@T, Comentario=@C, Puntuacion=@P WHERE ReseñaId=@Id";
        cmd.Parameters.Add(new SqlParameter("@T", r.TituloReseña ?? (object)DBNull.Value));
        cmd.Parameters.Add(new SqlParameter("@C", r.Comentario));
        cmd.Parameters.Add(new SqlParameter("@P", r.Puntuacion));
        cmd.Parameters.Add(new SqlParameter("@Id", r.ReseñaId));
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using var db = GetConnection(); db.Open();
        var cmd = db.CreateCommand();
        cmd.CommandText = "DELETE FROM Opiniones WHERE ReseñaId=@Id";
        cmd.Parameters.Add(new SqlParameter("@Id", id));
        return cmd.ExecuteNonQuery() > 0;
    }

    public string GetNombreCliente(int clienteId)
    {
        using var db = GetConnection(); db.Open();
        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT NombreUsuario FROM Cliente WHERE ClienteId = @Id";
        cmd.Parameters.Add(new SqlParameter("@Id", clienteId));
        var result = cmd.ExecuteScalar();
        return result != null ? result.ToString() : "Usuario Eliminado";
    }

     public IEnumerable<Opiniones> GetFiltradas(string isbn, DateTime? minFecha)
    {
        var lista = new List<Opiniones>();
        using (var db = GetConnection())
        {
            db.Open();
            var cmd = db.CreateCommand();
            
            string sql = "SELECT * FROM Opiniones WHERE LibroISBN = @ISBN";
            cmd.Parameters.Add(new SqlParameter("@ISBN", isbn));

            if (minFecha.HasValue)
            {
                sql += " AND FechaReseña >= @MinFecha";
                cmd.Parameters.Add(new SqlParameter("@MinFecha", minFecha.Value));
            }
            cmd.CommandText = sql;

            using (var reader = (SqlDataReader)cmd.ExecuteReader())
            {
                while (reader.Read()) lista.Add(MapFromReader(reader));
            }
        }
        return lista;
    }

    public OpinionStatsDTO GetEstadisticas(string isbn)
    {
        var stats = new OpinionStatsDTO();
        using (var db = GetConnection())
        {
            db.Open();
            var cmd = db.CreateCommand();
            cmd.CommandText = @"
                SELECT 
                    COUNT(*) as Total,
                    ISNULL(AVG(CAST(Puntuacion AS FLOAT)), 0) as Media,
                    ISNULL(MIN(Puntuacion), 0) as Minimo,
                    ISNULL(MAX(Puntuacion), 0) as Maximo
                FROM Opiniones 
                WHERE LibroISBN = @ISBN";
            
            cmd.Parameters.Add(new SqlParameter("@ISBN", isbn));

            using (var reader = (SqlDataReader)cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    stats.TotalOpiniones = (int)reader["Total"];
                    stats.PuntuacionMedia = (double)reader["Media"];
                    stats.PuntuacionMinima = (int)reader["Minimo"];
                    stats.PuntuacionMaxima = (int)reader["Maximo"];
                }
            }
        }
        return stats;
    }

}