using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La password corrente è obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Password Corrente")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nuova password è obbligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La password deve essere tra 8 e 100 caratteri")]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La conferma password è obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Nuova Password")]
        [Compare("NewPassword", ErrorMessage = "La nuova password e la conferma non coincidono")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}