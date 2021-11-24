using EngineAPI.Resources;
using System.ComponentModel.DataAnnotations;

namespace EngineAPI.DTOs
{
    public class ChangePasswordDTO
    {
        [Display(Name = nameof(Resource.ActualPass))]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "El campo {0} debe tener una longitud mínima de {1} carácteres.")]
        public string OldPassword { get; set; }

        [Display(Name = "Nueva contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "El campo {0} debe tener una longitud mínima de {1} carácteres.")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirmación de contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "El campo {0} debe tener una longitud mínima de {1} carácteres.")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no son iguales.")]
        public string Confirm { get; set; }
    }
}
