using System;
using System.ComponentModel.DataAnnotations;

public class Libro
{
    public Libro() { }

    public string ISBN { get; set; }
    public string Titulo { get; set; }
    public decimal PrecioVenta { get; set; }
    public int NumPaginas { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public bool EsBestSeller { get; set; }
    public decimal CostoProduccion { get; set; }
    
    public int AutorId { get; set; }
    public int GeneroId { get; set; }
}