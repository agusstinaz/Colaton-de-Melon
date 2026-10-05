using System.ComponentModel.DataAnnotations;

namespace ColatonDeMelon.Models
{
    public class CambiarPasswordViewModel
    {
        [Required(ErrorMessage = "Ingresá tu contraseña actual.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string PasswordActual { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá una nueva contraseña.")]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmá tu nueva contraseña.")]
        [DataType(DataType.Password)]
        [Compare(
            "NuevaPassword",
            ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar nueva contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}