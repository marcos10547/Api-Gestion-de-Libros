using System.ComponentModel.DataAnnotations;

public class ListaDeseosAddLibroDTO
{
    [Required]
    public string LibroISBN { get; set; } = default!;
}