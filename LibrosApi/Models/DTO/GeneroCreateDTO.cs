using System.ComponentModel.DataAnnotations;

public class GeneroCreateDTO
{
    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = default!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Required]
    public bool EsFiccion { get; set; }

    [StringLength(10)]
    public string? CodigoClasificacion { get; set; }

    [Range(0, 100)]
    public int Popularidad { get; set; }
}