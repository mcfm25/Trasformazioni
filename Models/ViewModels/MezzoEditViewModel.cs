using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la modifica di un mezzo esistente
    /// </summary>
    public class MezzoEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "La targa è obbligatoria")]
        [Display(Name = "Targa")]
        [StringLength(10, ErrorMessage = "La targa non può superare i 10 caratteri")]
        public string Targa { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca è obbligatoria")]
        [Display(Name = "Marca")]
        [StringLength(50, ErrorMessage = "La marca non può superare i 50 caratteri")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il modello è obbligatorio")]
        [Display(Name = "Modello")]
        [StringLength(50, ErrorMessage = "Il modello non può superare i 50 caratteri")]
        public string Modello { get; set; } = string.Empty;

        [Display(Name = "Anno")]
        [Range(1900, 2100, ErrorMessage = "Anno non valido")]
        public int? Anno { get; set; }

        [Required(ErrorMessage = "Il tipo di mezzo è obbligatorio")]
        [Display(Name = "Tipo Mezzo")]
        public TipoMezzo Tipo { get; set; }

        [Required(ErrorMessage = "Lo stato è obbligatorio")]
        [Display(Name = "Stato")]
        public StatoMezzo Stato { get; set; }

        [Required(ErrorMessage = "Il tipo di proprietà è obbligatorio")]
        [Display(Name = "Tipo Proprietà")]
        public TipoProprietaMezzo TipoProprieta { get; set; }

        [Display(Name = "Chilometraggio (km)")]
        [Range(0, 9999999.99, ErrorMessage = "Chilometraggio non valido")]
        public decimal? Chilometraggio { get; set; }

        [Display(Name = "Data Immatricolazione")]
        [DataType(DataType.Date)]
        public DateTime? DataImmatricolazione { get; set; }

        [Display(Name = "Data Acquisto")]
        [DataType(DataType.Date)]
        public DateTime? DataAcquisto { get; set; }

        [Display(Name = "Data Inizio Noleggio")]
        [DataType(DataType.Date)]
        public DateTime? DataInizioNoleggio { get; set; }

        [Display(Name = "Data Fine Noleggio")]
        [DataType(DataType.Date)]
        public DateTime? DataFineNoleggio { get; set; }

        [Display(Name = "Società Noleggio")]
        [StringLength(100, ErrorMessage = "La società di noleggio non può superare i 100 caratteri")]
        public string? SocietaNoleggio { get; set; }

        [Display(Name = "Scadenza Assicurazione")]
        [DataType(DataType.Date)]
        public DateTime? DataScadenzaAssicurazione { get; set; }

        [Display(Name = "Scadenza Revisione")]
        [DataType(DataType.Date)]
        public DateTime? DataScadenzaRevisione { get; set; }

        [Display(Name = "Note")]
        [StringLength(1000, ErrorMessage = "Le note non possono superare i 1000 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        [Display(Name = "Device IOT")]
        [StringLength(30, ErrorMessage = "Le note non possono superare i 30 caratteri")]
        //[DataType(DataType.MultilineText)]
        public string? DeviceIMEI { get; set; }

        [Display(Name = "Telefono SIM")]
        [StringLength(30, ErrorMessage = "Le note non possono superare i 30 caratteri")]
        //[DataType(DataType.MultilineText)]
        public string? DevicePhoneNumber { get; set; }
    }
}