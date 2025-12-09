public interface IOpinionService
{
    IEnumerable<OpinionResponseDTO> GetAllReseñas();
    OpinionResponseDTO GetReseñaById(int id);
    
    IEnumerable<OpinionResponseDTO> GetReseñasFiltradas(string isbn, DateTime? minFecha);
    
    OpinionStatsDTO GetEstadisticas(string isbn);

    OpinionResponseDTO CreateReseña(OpinionCreateDTO dto);
    bool UpdateReseña(int id, OpinionCreateDTO dto);
    bool DeleteReseña(int id);
}