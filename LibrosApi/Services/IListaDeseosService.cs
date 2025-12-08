public interface IListaDeseosService
{
    ListaDeseosResponseDTO GetById(int id);
    IEnumerable<ListaDeseosResponseDTO> GetByCliente(int clienteId);
    ListaDeseosResponseDTO Create(ListaDeseosCreateDTO dto);
    bool Delete(int id);
    bool AddLibroToLista(int listaId, string isbn);
    bool RemoveLibroFromLista(int listaId, string isbn);
}