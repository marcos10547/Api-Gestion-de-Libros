using System;

public class ClienteResponseDTO
{
    public int ClienteId { get; set; }
    public string Email { get; set; }
    public string NombreUsuario { get; set; }
    public decimal GastoTotal { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool EsPremium { get; set; }
    public int Nivel { get; set; }
}