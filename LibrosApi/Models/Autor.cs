using System;
using System.ComponentModel.DataAnnotations;

public class Autor
{
    public Autor() { }

    public int AutorId { get; set; }
    public string NombreCompleto { get; set; }
    public string Nacionalidad { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public decimal RoyaltyPorcentaje { get; set; }
    public int NumObras { get; set; }
    public bool EsActivo { get; set; }
}