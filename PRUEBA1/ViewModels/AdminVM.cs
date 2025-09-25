using PRUEBA1.Models;

namespace PRUEBA1.ViewModels
{
    public class AdminVM
    {
        public List<Usuario> Usuarios { get; set; }
        public List<Libro> Libros { get; set; }
        public List<Inventario> Inventarios { get; set; }
    }
}
