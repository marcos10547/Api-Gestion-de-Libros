using System.Collections.Generic;

public interface ILibroRepository
{
    Libro GetByISBN(string isbn);
    IEnumerable<Libro> GetAll();
    Libro Create(Libro libro);
    bool Update(Libro libro);
    bool Delete(string isbn);
    IEnumerable<Libro> GetLibros(LibroQueryParams parametros);
}