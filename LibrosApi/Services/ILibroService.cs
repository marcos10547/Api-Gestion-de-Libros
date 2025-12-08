public interface ILibroService
{
    LibroResponseDTO GetLibroByISBN(string isbn);
    IEnumerable<LibroResponseDTO> GetAllLibros();
    IEnumerable<LibroResponseDTO> GetLibros(LibroQueryParams parametros);
    LibroResponseDTO CreateLibro(LibroCreateDTO dto);
    bool UpdateLibro(string isbn, LibroCreateDTO dto);
    bool DeleteLibro(string isbn);
}