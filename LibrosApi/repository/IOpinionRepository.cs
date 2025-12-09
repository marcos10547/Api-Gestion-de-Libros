public interface IOpinionRepository
{
    IEnumerable<Opiniones> GetAll();
    Opiniones GetById(int id);
    
    Opiniones Create(Opiniones resena);
    bool Update(Opiniones resena);
    bool Delete(int id);
    
    IEnumerable<Opiniones> GetFiltradas(string isbn, DateTime? minFecha);
    
    OpinionStatsDTO GetEstadisticas(string isbn);

    string GetNombreCliente(int clienteId);
}