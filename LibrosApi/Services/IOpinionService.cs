public interface IOpinionService
{
    IEnumerable<OpinionResponseDTO> GetAllReseñas();
    OpinionResponseDTO GetReseñaById(int id);
    OpinionResponseDTO CreateReseña(OpinionCreateDTO dto);
    bool UpdateReseña(int id, OpinionCreateDTO dto);
    bool DeleteReseña(int id);
}