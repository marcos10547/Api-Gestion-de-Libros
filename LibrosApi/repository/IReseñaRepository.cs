public interface IReseñaRepository
{
    Reseña GetById(int id);
    IEnumerable<Reseña> GetAll();
    IEnumerable<Reseña> GetByClienteId(int clienteId); // Requisito Clave
    IEnumerable<Reseña> GetByLibroISBN(string isbn);   // Extra útil
    Reseña Create(Reseña reseña);
    bool Delete(int id);
}