using System.Collections.Generic;

public interface IAutorRepository
{
    Autor GetById(int id);
    IEnumerable<Autor> GetAll();
    Autor Create(Autor autor);
    bool Update(Autor autor);
    bool Delete(int id);
}