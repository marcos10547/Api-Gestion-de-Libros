using System.ComponentModel.DataAnnotations;

public class ReseñaCreateDTO
{
    [StringLength(100)]
    public string? TituloReseña { get; set; }

    [Required]
    [StringLength(1000)]
    public string Comentario { get; set; } = default!;

    [Required]
    [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
    public int Puntuacion { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Required]
    public string LibroISBN { get; set; } = default!;
}