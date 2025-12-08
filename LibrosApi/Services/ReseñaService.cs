public class ReseñaService : IReseñaService
{
    private readonly IReseñaRepository _reseñaRepository;

    public ReseñaService(IReseñaRepository reseñaRepository)
    {
        _reseñaRepository = reseñaRepository;
    }
    
    private ReseñaResponseDTO MapToDTO(Reseña reseña)
    {
        if (reseña == null) return null!;
        return new ReseñaResponseDTO
        {
            ReseñaId = reseña.ReseñaId,
            TituloReseña = reseña.TituloReseña,
            Comentario = reseña.Comentario,
            Puntuacion = reseña.Puntuacion,
            FechaReseña = reseña.FechaReseña,
            EsAprobada = reseña.EsAprobada,
            Longitud = reseña.Longitud,
            ClienteId = reseña.ClienteId,
            LibroISBN = reseña.LibroISBN
        };
    }

    public ReseñaResponseDTO GetReseñaById(int id)
    {
        var reseña = _reseñaRepository.GetById(id);
        return MapToDTO(reseña);
    }

    public IEnumerable<ReseñaResponseDTO> GetAllReseñas()
    {
        var reseñas = _reseñaRepository.GetAll();
        return reseñas.Select(MapToDTO);
    }

    public IEnumerable<ReseñaResponseDTO> GetReseñasByCliente(int clienteId)
    {
        var reseñas = _reseñaRepository.GetByClienteId(clienteId);
        return reseñas.Select(MapToDTO);
    }

    public IEnumerable<ReseñaResponseDTO> GetReseñasByLibro(string isbn)
    {
        var reseñas = _reseñaRepository.GetByLibroISBN(isbn);
        return reseñas.Select(MapToDTO);
    }

    public ReseñaResponseDTO CreateReseña(ReseñaCreateDTO dto)
    {
        var nuevaReseña = new Reseña
        {
            TituloReseña = dto.TituloReseña,
            Comentario = dto.Comentario,
            Puntuacion = dto.Puntuacion,
            ClienteId = dto.ClienteId,
            LibroISBN = dto.LibroISBN,
            
            FechaReseña = DateTime.UtcNow,
            EsAprobada = true, 
            IpOrigen = "127.0.0.1", 
            Longitud = dto.Comentario.Length
        };

        var creada = _reseñaRepository.Create(nuevaReseña);
        return MapToDTO(creada);
    }

    public bool DeleteReseña(int id)
    {
        return _reseñaRepository.Delete(id);
    }
}