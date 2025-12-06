using System;

public class GeneroResponseDTO
{
    public int GeneroId { get; set; }
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public bool EsFiccion { get; set; }
    public string? CodigoClasificacion { get; set; }
    public int Popularidad { get; set; }
    public DateTime FechaCreacion { get; set; }
}