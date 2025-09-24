using System.ComponentModel.DataAnnotations.Schema;

namespace PRUEBA1.Models
{
    public class Pedido
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public int IdLibro { get; set; }
        public int? IdProducto { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaReserva { get; set; } = DateTime.Now;
        public DateTime? FechaEntrega { get; set; }
        public string Estado { get; set; } = "Pendiente";

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        [ForeignKey("IdLibro")]
        public Libro Libro { get; set; }

        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }
    }
}
