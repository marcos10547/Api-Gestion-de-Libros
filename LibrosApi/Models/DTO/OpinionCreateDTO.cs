using System.ComponentModel.DataAnnotations;

public class OpinionCreateDTO
{
    public string? TituloReseña { get; set; }
    public string Comentario { get; set; } = default!;
    
    [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
    public int Puntuacion { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Required]
    public DateTime FechaReseña { get; set; } 

    [Required]
    public string NombreCliente { get; set; }
    
    [Required]
    public string LibroISBN { get; set; } = default!;

}