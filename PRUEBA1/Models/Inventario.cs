using System.ComponentModel.DataAnnotations.Schema;

namespace PRUEBA1.Models
{
    public class Inventario
    {
        public int IdInventario { get; set; }
        public int IdLibro { get; set; }
        public int IdProducto { get; set; }
        public int Stock { get; set; }
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        [ForeignKey("IdLibro")]
        public Libro Libro { get; set; }

        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }
    }
}
