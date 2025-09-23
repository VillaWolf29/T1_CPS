namespace PRUEBA1.Models
{
    public class Reseña
    {
        public int IdReseña { get; set; }
        public string Contenido { get; set; }
        public string Calificacion { get; set; }
        public int IdAutor { get; set; }
        public Autor Autor { get; set; }

    }
}
