using System;

public class AutorResponseDTO
{
    public int AutorId { get; set; }
    public string NombreCompleto { get; set; }
    public string Nacionalidad { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public decimal RoyaltyPorcentaje { get; set; }
    public int NumObras { get; set; }
}