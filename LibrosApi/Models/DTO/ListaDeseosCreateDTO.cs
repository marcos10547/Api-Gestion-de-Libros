using System.ComponentModel.DataAnnotations;

public class ListaDeseosCreateDTO
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = default!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Required]
    public bool EsPublica { get; set; }

    [Required]
    public int ClienteId { get; set; }
}