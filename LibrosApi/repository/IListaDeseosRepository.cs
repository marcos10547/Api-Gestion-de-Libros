public interface IListaDeseosRepository
{
    ListaDeseos GetById(int id);
    IEnumerable<ListaDeseos> GetAll();
    IEnumerable<ListaDeseos> GetByClienteId(int clienteId);
    ListaDeseos Create(ListaDeseos lista);
    bool Delete(int id);
    
    // Métodos para la relación N:M (Libros en la lista)
    bool AddLibro(int listaId, string isbn);
    bool RemoveLibro(int listaId, string isbn);
    IEnumerable<string> GetLibrosInLista(int listaId); // Devuelve los ISBNs
    void RecalcularTotales(int listaId); // Actualiza NumLibros y CostoEstimado
}