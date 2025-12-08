public interface IGeneroRepository
{
    Genero GetById(int id);
    IEnumerable<Genero> GetAll();
    Genero Create(Genero genero);
    bool Update(Genero genero);
    bool Delete(int id);
}