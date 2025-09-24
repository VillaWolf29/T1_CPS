namespace PRUEBA1.Models
{
    public class Libro
    {
        public int IdLibro { get; set; }
        public string Autor { get; set; }
        public string Titulo { get; set; }        
        public string Genero { get; set; }
        public DateOnly FechaPublicacion { get; set; }

        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
