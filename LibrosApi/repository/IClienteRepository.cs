using System.Collections.Generic;

public interface IClienteRepository
{
    Cliente GetById(int id);
    IEnumerable<Cliente> GetAll();
    Cliente Create(Cliente cliente);
    bool Update(Cliente cliente);
    bool Delete(int id);
}