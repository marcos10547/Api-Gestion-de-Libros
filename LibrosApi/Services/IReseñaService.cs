public interface IReseñaService
{
    ReseñaResponseDTO GetReseñaById(int id);
    IEnumerable<ReseñaResponseDTO> GetAllReseñas();
    IEnumerable<ReseñaResponseDTO> GetReseñasByCliente(int clienteId);
    IEnumerable<ReseñaResponseDTO> GetReseñasByLibro(string isbn);
    ReseñaResponseDTO CreateReseña(ReseñaCreateDTO dto);
    bool DeleteReseña(int id);
}