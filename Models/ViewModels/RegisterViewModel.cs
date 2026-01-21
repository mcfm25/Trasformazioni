using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il cognome non può superare i 100 caratteri")]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        //[StringLength(100, ErrorMessage = "Il reparto non può superare i 100 caratteri")]
        //[Display(Name = "Reparto")]
        //public string? Reparto { get; set; }
        [Display(Name = "Reparto")]
        public Guid? RepartoId { get; set; }

        [Required(ErrorMessage = "La data di assunzione è obbligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Data Assunzione")]
        public DateTime DataAssunzione { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La password deve essere tra 8 e 100 caratteri")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La conferma password è obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        [Compare("Password", ErrorMessage = "La password e la conferma non coincidono")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public SelectList? RepartiSelectList { get; set; }
    }
}