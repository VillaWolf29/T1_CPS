using System.ComponentModel.DataAnnotations.Schema;

namespace PRUEBA1.Models
{
    public class DetalleVenta
    {
        public int IdDetalle { get; set; }
        public int IdVenta { get; set; }
        public int? IdLibro { get; set; }
        public int? IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        [ForeignKey("IdVenta")]
        public Venta Venta { get; set; }

        [ForeignKey("IdLibro")]
        public Libro Libro { get; set; }

        [ForeignKey("IdProducto")]
        public Producto Producto { get; set; }
    }
}
