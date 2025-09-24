using System.ComponentModel.DataAnnotations;

namespace PRUEBA1.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Formato de correo electrónico inválido")]
        public string Correo { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "La contraseña no puede exceder los 100 caracteres")]
        public string Contraseña { get; set; }

        [Display(Name = "Recordar mis datos")]
        public bool Recordarme { get; set; }
    }
}