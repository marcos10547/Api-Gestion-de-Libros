public class ListaDeseosResponseDTO
{
    public int ListaId { get; set; }
    public string Nombre { get; set; } = default!;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public decimal CostoEstimadoTotal { get; set; }
    public bool EsPublica { get; set; }
    public int NumLibros { get; set; }
    public int ClienteId { get; set; }
    
    public List<string> LibrosISBN { get; set; } = new List<string>();
}