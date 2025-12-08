public class GeneroService : IGeneroService
{
    private readonly IGeneroRepository _generoRepository;

    public GeneroService(IGeneroRepository generoRepository)
    {
        _generoRepository = generoRepository;
    }
    
    private GeneroResponseDTO MapToResponseDTO(Genero genero)
    {
        if (genero == null) return null!;
        
        return new GeneroResponseDTO
        {
            GeneroId = genero.GeneroId,
            Nombre = genero.Nombre,
            Descripcion = genero.Descripcion,
            EsFiccion = genero.EsFiccion,
            CodigoClasificacion = genero.CodigoClasificacion,
            Popularidad = genero.Popularidad,
            FechaCreacion = genero.FechaCreacion
        };
    }

    public IEnumerable<GeneroResponseDTO> GetAllGeneros()
    {
        var generos = _generoRepository.GetAll();
        return generos.Select(MapToResponseDTO);
    }

    public GeneroResponseDTO GetGeneroById(int id)
    {
        var genero = _generoRepository.GetById(id);
        return MapToResponseDTO(genero);
    }

    public GeneroResponseDTO CreateGenero(GeneroCreateDTO dto)
    {
        var nuevoGenero = new Genero
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            EsFiccion = dto.EsFiccion,
            CodigoClasificacion = dto.CodigoClasificacion,
            Popularidad = dto.Popularidad,
            FechaCreacion = DateTime.UtcNow
        };

        var generoGuardado = _generoRepository.Create(nuevoGenero);
        return MapToResponseDTO(generoGuardado);
    }
    
    public bool UpdateGenero(int id, GeneroCreateDTO dto)
    {
        var generoExistente = _generoRepository.GetById(id);
        if (generoExistente == null) return false;

        generoExistente.Nombre = dto.Nombre;
        generoExistente.Descripcion = dto.Descripcion;
        generoExistente.EsFiccion = dto.EsFiccion;
        generoExistente.CodigoClasificacion = dto.CodigoClasificacion;
        generoExistente.Popularidad = dto.Popularidad;
        
        return _generoRepository.Update(generoExistente);
    }

    public bool DeleteGenero(int id)
    {
        return _generoRepository.Delete(id);
    }
}