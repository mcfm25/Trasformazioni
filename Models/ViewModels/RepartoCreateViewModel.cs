using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    public class RepartoCreateViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Inserire un indirizzo email valido")]
        [StringLength(255, ErrorMessage = "L'email non può superare i 255 caratteri")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }
    }
}