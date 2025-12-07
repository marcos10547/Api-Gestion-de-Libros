public class ReseñaResponseDTO
{
    public int ReseñaId { get; set; }
    public string? TituloReseña { get; set; }
    public string Comentario { get; set; } = default!;
    public int Puntuacion { get; set; }
    public DateTime FechaReseña { get; set; }
    public bool EsAprobada { get; set; }
    public int Longitud { get; set; }
    public int ClienteId { get; set; }
    public string LibroISBN { get; set; } = default!;
}