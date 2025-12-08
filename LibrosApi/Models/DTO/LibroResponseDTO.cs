using System;

public class LibroResponseDTO
{
    public string ISBN { get; set; } = default!;
    public string Titulo { get; set; } = default!;
    public decimal PrecioVenta { get; set; }
    public int NumPaginas { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public bool EsBestSeller { get; set; }
    public int AutorId { get; set; }
    public int GeneroId { get; set; }
}