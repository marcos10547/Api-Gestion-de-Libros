using System.ComponentModel.DataAnnotations;
using System;

public class ListaDeseos
{
    public ListaDeseos() { }

    public int ListaId { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public decimal CostoEstimadoTotal { get; set; }
    public bool EsPublica { get; set; }
    public int NumLibros { get; set; }

    public int ClienteId { get; set; }
}