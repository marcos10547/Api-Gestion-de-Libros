public class Opiniones
{
    public Opiniones () {}

    public int ReseñaId { get; set; }
    public string TituloReseña { get; set; }
    public string Comentario { get; set; }
    public int Puntuacion { get; set; }
    public DateTime FechaReseña { get; set; }
    public int ClienteId { get; set; }
    public string LibroISBN { get; set; }
    public string NombreCliente { get; set; } 
}