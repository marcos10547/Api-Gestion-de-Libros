using System.ComponentModel.DataAnnotations;
using System;

public class AutorCreateDTO
{
    [Required]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string NombreCompleto { get; set; }

    [Required]
    public string Nacionalidad { get; set; }

    [Required]
    public DateTime FechaNacimiento { get; set; }
    
    [Range(0.00, 100.00, ErrorMessage = "El porcentaje de Royalty debe estar entre 0 y 100.")]
    public decimal RoyaltyPorcentaje { get; set; }

    [Required]
    public bool EsActivo { get; set; }
}