public interface IGeneroService
{
    GeneroResponseDTO GetGeneroById(int id);
    IEnumerable<GeneroResponseDTO> GetAllGeneros();
    GeneroResponseDTO CreateGenero(GeneroCreateDTO dto);
    bool UpdateGenero(int id, GeneroCreateDTO dto);
    bool DeleteGenero(int id);
}