using System.Collections.Generic;

public interface IListaDeseosRepository
{
    ListaDeseos GetById(int id);
    IEnumerable<ListaDeseos> GetAll();
    IEnumerable<ListaDeseos> GetByClienteId(int clienteId);
    ListaDeseos Create(ListaDeseos lista);
    bool Delete(int id);
    
    bool AddLibro(int listaId, string isbn);
    bool RemoveLibro(int listaId, string isbn);
    
    IEnumerable<Libro> GetLibrosInLista(int listaId); 
    
    void RecalcularTotales(int listaId);
}