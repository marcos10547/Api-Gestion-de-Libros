public class OpinionService : IOpinionService
{
    private readonly IOpinionRepository _repository;

    public OpinionService(IOpinionRepository repository)
    {
        _repository = repository;
    }

    private OpinionResponseDTO MapToDTO(Opiniones r)
    {
        if (r == null) return null;
        
    
        string nombreCliente = _repository.GetNombreCliente(r.ClienteId);

        return new OpinionResponseDTO
        {
            ReseñaId = r.ReseñaId,
            TituloReseña = r.TituloReseña,
            Comentario = r.Comentario,
            Puntuacion = r.Puntuacion,
            FechaReseña = r.FechaReseña,
            ClienteId = r.ClienteId,
            LibroISBN = r.LibroISBN,
            NombreCliente = nombreCliente 
        };
    }

    public IEnumerable<OpinionResponseDTO> GetAllReseñas()
    {
        return _repository.GetAll().Select(MapToDTO);
    }

    public OpinionResponseDTO GetReseñaById(int id)
    {
        return MapToDTO(_repository.GetById(id));
    }

   


    public OpinionResponseDTO CreateReseña(OpinionCreateDTO dto)
    {
        var nueva = new Opiniones
        {
            TituloReseña = dto.TituloReseña,
            Comentario = dto.Comentario,
            Puntuacion = dto.Puntuacion,
            ClienteId = dto.ClienteId,
            FechaReseña = DateTime.UtcNow,
            NombreCliente = dto.NombreCliente, 
            LibroISBN = dto.LibroISBN
            
        };
        return MapToDTO(_repository.Create(nueva));
    }
      
    public bool UpdateReseña(int id, OpinionCreateDTO dto)
    {
        var existente = _repository.GetById(id);
        if (existente == null) return false;

        existente.TituloReseña = dto.TituloReseña;
        existente.Comentario = dto.Comentario;
        existente.Puntuacion = dto.Puntuacion;
        
        return _repository.Update(existente);
    }

    public bool DeleteReseña(int id)
    {
        return _repository.Delete(id);
    }

}