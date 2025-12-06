using System.Collections.Generic;
using System.Linq;

public class LibroService : ILibroService
{
    private readonly ILibroRepository _libroRepository;

    public LibroService(ILibroRepository libroRepository)
    {
        _libroRepository = libroRepository;
    }

    private LibroResponseDTO MapToDTO(Libro libro)
    {
        if (libro == null) return null!;
        return new LibroResponseDTO
        {
            ISBN = libro.ISBN,
            Titulo = libro.Titulo,
            PrecioVenta = libro.PrecioVenta,
            NumPaginas = libro.NumPaginas,
            FechaPublicacion = libro.FechaPublicacion,
            EsBestSeller = libro.EsBestSeller,
            AutorId = libro.AutorId,
            GeneroId = libro.GeneroId
        };
    }

    public IEnumerable<LibroResponseDTO> GetLibros(LibroQueryParams parametros)
    {
        var libros = _libroRepository.GetLibros(parametros);
        return libros.Select(MapToDTO);
    }

    public LibroResponseDTO GetLibroByISBN(string isbn)
    {
        var libro = _libroRepository.GetByISBN(isbn);
        return MapToDTO(libro);
    }

    public IEnumerable<LibroResponseDTO> GetAllLibros()
    {
        var libros = _libroRepository.GetAll();
        return libros.Select(MapToDTO);
    }

    public LibroResponseDTO CreateLibro(LibroCreateDTO dto)
    {
        var libro = new Libro
        {
            ISBN = dto.ISBN,
            Titulo = dto.Titulo,
            PrecioVenta = dto.PrecioVenta,
            NumPaginas = dto.NumPaginas,
            FechaPublicacion = dto.FechaPublicacion,
            EsBestSeller = dto.EsBestSeller,
            CostoProduccion = dto.CostoProduccion,
            AutorId = dto.AutorId,
            GeneroId = dto.GeneroId
        };

        var creado = _libroRepository.Create(libro);
        return MapToDTO(creado);
    }

    public bool UpdateLibro(string isbn, LibroCreateDTO dto)
    {
        var existente = _libroRepository.GetByISBN(isbn);
        if (existente == null) return false;

        existente.Titulo = dto.Titulo;
        existente.PrecioVenta = dto.PrecioVenta;
        existente.NumPaginas = dto.NumPaginas;
        existente.FechaPublicacion = dto.FechaPublicacion;
        existente.EsBestSeller = dto.EsBestSeller;
        existente.CostoProduccion = dto.CostoProduccion;
        existente.AutorId = dto.AutorId;
        existente.GeneroId = dto.GeneroId;

        return _libroRepository.Update(existente);
    }

    public bool DeleteLibro(string isbn)
    {
        return _libroRepository.Delete(isbn);
    }
}