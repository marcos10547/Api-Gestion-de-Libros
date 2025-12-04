using System.Collections.Generic;
using System.Linq;

public class AutorService : IAutorService
{
    private readonly IAutorRepository _autorRepository;

    public AutorService(IAutorRepository autorRepository)
    {
        _autorRepository = autorRepository;
    }
    
    private AutorResponseDTO MapToResponseDTO(Autor autor)
    {
        if (autor == null) return null;
        
        return new AutorResponseDTO
        {
            AutorId = autor.AutorId,
            NombreCompleto = autor.NombreCompleto,
            Nacionalidad = autor.Nacionalidad,
            FechaNacimiento = autor.FechaNacimiento,
            RoyaltyPorcentaje = autor.RoyaltyPorcentaje,
            NumObras = autor.NumObras
        };
    }

    public IEnumerable<AutorResponseDTO> GetAllAutores()
    {
        var autores = _autorRepository.GetAll();
        
        return autores.Select(MapToResponseDTO);
    }

    public AutorResponseDTO GetAutorById(int id)
    {
        var autor = _autorRepository.GetById(id);
        
        return MapToResponseDTO(autor);
    }

    public AutorResponseDTO CreateAutor(AutorCreateDTO dto)
    {
        var nuevoAutor = new Autor
        {
            NombreCompleto = dto.NombreCompleto,
            Nacionalidad = dto.Nacionalidad,
            FechaNacimiento = dto.FechaNacimiento,
            RoyaltyPorcentaje = dto.RoyaltyPorcentaje,
            EsActivo = dto.EsActivo,
            NumObras = 0 
        };

        var autorGuardado = _autorRepository.Create(nuevoAutor);
        
        return MapToResponseDTO(autorGuardado);
    }
    
    public bool UpdateAutor(int id, AutorCreateDTO dto)
    {
        var autorExistente = _autorRepository.GetById(id);
        if (autorExistente == null) return false;

        autorExistente.NombreCompleto = dto.NombreCompleto;
        autorExistente.Nacionalidad = dto.Nacionalidad;
        autorExistente.FechaNacimiento = dto.FechaNacimiento;
        autorExistente.RoyaltyPorcentaje = dto.RoyaltyPorcentaje;
        autorExistente.EsActivo = dto.EsActivo;
        
        return _autorRepository.Update(autorExistente);
    }

    public bool DeleteAutor(int id)
    {
        return _autorRepository.Delete(id);
    }
}