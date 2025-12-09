public interface IOpinionRepository
{
    IEnumerable<Opiniones> GetAll();
    Opiniones GetById(int id);
    
    Opiniones Create(Opiniones resena);
    bool Update(Opiniones resena);
    bool Delete(int id);
    string GetNombreCliente(int clienteId);
}