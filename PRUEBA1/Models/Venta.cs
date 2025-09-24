using System.ComponentModel.DataAnnotations.Schema;

namespace PRUEBA1.Models
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaVenta { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; } = "Completada";

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
