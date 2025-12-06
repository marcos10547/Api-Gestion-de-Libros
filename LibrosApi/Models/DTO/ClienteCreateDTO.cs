using System.ComponentModel.DataAnnotations;
using System;

public class ClienteCreateDTO
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 4)]
    public string NombreUsuario { get; set; }

    [Required]
    public bool EsPremium { get; set; }
}