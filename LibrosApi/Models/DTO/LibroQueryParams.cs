using System;

public class LibroQueryParams
{
    public string? Titulo { get; set; } 
    public int? GeneroId { get; set; } 
    public decimal? PrecioMax { get; set; } 

    public string OrdenarPor { get; set; } = "Titulo"; 
    public string Direccion { get; set; } = "asc"; 
}