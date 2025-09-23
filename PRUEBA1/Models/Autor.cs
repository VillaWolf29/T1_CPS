namespace PRUEBA1.Models
{
    public class Autor
    {
        public int IdAutor { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Nacionalidad { get; set; }        
        public List<Reseña> Reseñas { get; set; } = new List<Reseña>();        

    }
}
