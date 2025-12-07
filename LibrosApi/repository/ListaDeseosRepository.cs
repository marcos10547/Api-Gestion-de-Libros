using System.Data;
using Microsoft.Data.SqlClient; 

public class ListaDeseosRepository : IListaDeseosRepository
{
    private readonly string _connectionString;

    public ListaDeseosRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString); 
    }

    private ListaDeseos MapFromReader(SqlDataReader reader)
    {
        return new ListaDeseos
        {
            ListaId = (int)reader["ListaId"],
            Nombre = reader["Nombre"].ToString()!,
            Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : null,
            FechaCreacion = (DateTime)reader["FechaCreacion"],
            CostoEstimadoTotal = (decimal)reader["CostoEstimadoTotal"],
            EsPublica = (bool)reader["EsPublica"],
            NumLibros = (int)reader["NumLibros"],
            ClienteId = (int)reader["ClienteId"]
        };
    }

    public ListaDeseos GetById(int id)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM ListaDeseos WHERE ListaId = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            
            using (var reader = (SqlDataReader)command.ExecuteReader())
            {
                if (reader.Read()) return MapFromReader(reader);
            }
        }
        return null!;
    }

    public IEnumerable<ListaDeseos> GetAll()
    {
        var listas = new List<ListaDeseos>();
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM ListaDeseos";
            using (var reader = (SqlDataReader)command.ExecuteReader())
            {
                while (reader.Read()) listas.Add(MapFromReader(reader));
            }
        }
        return listas;
    }

    public IEnumerable<ListaDeseos> GetByClienteId(int clienteId)
    {
        var listas = new List<ListaDeseos>();
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT * FROM ListaDeseos WHERE ClienteId = @ClienteId";
            command.Parameters.Add(new SqlParameter("@ClienteId", clienteId));
            using (var reader = (SqlDataReader)command.ExecuteReader())
            {
                while (reader.Read()) listas.Add(MapFromReader(reader));
            }
        }
        return listas;
    }

    public ListaDeseos Create(ListaDeseos lista)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                INSERT INTO ListaDeseos (Nombre, Descripcion, FechaCreacion, CostoEstimadoTotal, EsPublica, NumLibros, ClienteId)
                OUTPUT INSERTED.ListaId
                VALUES (@Nombre, @Descripcion, @Fecha, @Costo, @EsPublica, @NumLibros, @ClienteId);
            ";
            
            using (var command = db.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SqlParameter("@Nombre", lista.Nombre));
                command.Parameters.Add(new SqlParameter("@Descripcion", (object)lista.Descripcion! ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Fecha", lista.FechaCreacion));
                command.Parameters.Add(new SqlParameter("@Costo", lista.CostoEstimadoTotal));
                command.Parameters.Add(new SqlParameter("@EsPublica", lista.EsPublica));
                command.Parameters.Add(new SqlParameter("@NumLibros", lista.NumLibros));
                command.Parameters.Add(new SqlParameter("@ClienteId", lista.ClienteId));

                lista.ListaId = (int)command.ExecuteScalar(); 
                return lista;
            }
        }
    }

    public bool Delete(int id)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var cmdRel = db.CreateCommand();
            cmdRel.CommandText = "DELETE FROM ListaDeseos_Libro WHERE ListaId = @Id";
            cmdRel.Parameters.Add(new SqlParameter("@Id", id));
            cmdRel.ExecuteNonQuery();

            var command = db.CreateCommand();
            command.CommandText = "DELETE FROM ListaDeseos WHERE ListaId = @Id";
            command.Parameters.Add(new SqlParameter("@Id", id));
            return command.ExecuteNonQuery() > 0;
        }
    }


    public bool AddLibro(int listaId, string isbn)
    {
        using (var db = GetConnection())
        {
            db.Open();
            try 
            {
                var command = db.CreateCommand();
                command.CommandText = "INSERT INTO ListaDeseos_Libro (ListaId, LibroISBN) VALUES (@ListaId, @ISBN)";
                command.Parameters.Add(new SqlParameter("@ListaId", listaId));
                command.Parameters.Add(new SqlParameter("@ISBN", isbn));
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException) 
            {
                return false; 
            }
        }
    }

    public bool RemoveLibro(int listaId, string isbn)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "DELETE FROM ListaDeseos_Libro WHERE ListaId = @ListaId AND LibroISBN = @ISBN";
            command.Parameters.Add(new SqlParameter("@ListaId", listaId));
            command.Parameters.Add(new SqlParameter("@ISBN", isbn));
            return command.ExecuteNonQuery() > 0;
        }
    }

    public IEnumerable<string> GetLibrosInLista(int listaId)
    {
        var isbns = new List<string>();
        using (var db = GetConnection())
        {
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "SELECT LibroISBN FROM ListaDeseos_Libro WHERE ListaId = @ListaId";
            command.Parameters.Add(new SqlParameter("@ListaId", listaId));
            using (var reader = (SqlDataReader)command.ExecuteReader())
            {
                while (reader.Read()) isbns.Add(reader["LibroISBN"].ToString()!);
            }
        }
        return isbns;
    }

    public void RecalcularTotales(int listaId)
    {
        using (var db = GetConnection())
        {
            db.Open();
            var sql = @"
                UPDATE ListaDeseos
                SET 
                    NumLibros = (SELECT COUNT(*) FROM ListaDeseos_Libro WHERE ListaId = @Id),
                    CostoEstimadoTotal = ISNULL((
                        SELECT SUM(L.PrecioVenta) 
                        FROM ListaDeseos_Libro LDL
                        JOIN Libro L ON LDL.LibroISBN = L.ISBN
                        WHERE LDL.ListaId = @Id
                    ), 0)
                WHERE ListaId = @Id
            ";
            var command = db.CreateCommand();
            command.CommandText = sql;
            command.Parameters.Add(new SqlParameter("@Id", listaId));
            command.ExecuteNonQuery();
        }
    }
}