using System.ComponentModel.DataAnnotations.Schema;

namespace PRUEBA1.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public int IdRol { get; set; }
        [ForeignKey("IdRol")]
        public Rol Rol { get; set; }

    }
}
