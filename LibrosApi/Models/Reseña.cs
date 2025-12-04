using System.ComponentModel.DataAnnotations;
using System;

public class Reseña
{
    public Reseña() { }

    public int ReseñaId { get; set; }
    public string TituloReseña { get; set; }
    public string Comentario { get; set; }
    public int Puntuacion { get; set; }
    public DateTime FechaReseña { get; set; }
    public bool EsAprobada { get; set; }
    public string IpOrigen { get; set; }
    public int Longitud { get; set; }
    
    public int ClienteId { get; set; }
    public string LibroISBN { get; set; }
}
