using System.ComponentModel.DataAnnotations;
using System;

public class LibroCreateDTO
{
    [Required]
    [StringLength(13, MinimumLength = 10)]
    public string ISBN { get; set; } = default!;

    [Required]
    public string Titulo { get; set; } = default!;

    [Required]
    [Range(0.01, 1000.00)]
    public decimal PrecioVenta { get; set; }
    
    [Required]
    public int AutorId { get; set; }

    [Required]
    public int GeneroId { get; set; }
    
    [Range(1, 10000)]
    public int NumPaginas { get; set; }
    
    public DateTime FechaPublicacion { get; set; }
    
    public bool EsBestSeller { get; set; }
    
    public decimal CostoProduccion { get; set; }
}