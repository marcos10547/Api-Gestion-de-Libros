using System.Collections.Generic;

public interface IAutorService
{
    AutorResponseDTO GetAutorById(int id);
    IEnumerable<AutorResponseDTO> GetAllAutores();
    AutorResponseDTO CreateAutor(AutorCreateDTO dto);
    bool UpdateAutor(int id, AutorCreateDTO dto);
    bool DeleteAutor(int id);
}