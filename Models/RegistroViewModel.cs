using System.ComponentModel.DataAnnotations;

namespace ColatonDeMelon.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresá un correo electrónico válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tenés que confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}