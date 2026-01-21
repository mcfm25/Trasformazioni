using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la creazione di una nuova assegnazione mezzo
    /// </summary>
    public class AssegnazioneMezzoCreateViewModel
    {
        [Required(ErrorMessage = "Il mezzo è obbligatorio")]
        public Guid MezzoId { get; set; }

        // Info display mezzo (readonly, per mostrare nel form)
        public string? MezzoTarga { get; set; }
        public string? MezzoDescrizioneCompleta { get; set; }

        [Required(ErrorMessage = "L'utente è obbligatorio")]
        [Display(Name = "Utente")]
        public string UtenteId { get; set; } = string.Empty;

        [Required(ErrorMessage = "La data di inizio è obbligatoria")]
        [Display(Name = "Data Inizio")]
        [DataType(DataType.Date)]
        public DateTime DataInizio { get; set; } = DateTime.Today;

        /// <summary>
        /// Data e ora di fine assegnazione (OPZIONALE)
        /// - Se NULL: assegnazione a tempo indeterminato (blocca completamente il mezzo)
        /// - Se POPOLATA: assegnazione temporanea (permette altre prenotazioni non sovrapposte)
        /// </summary>
        [Display(Name = "Data e Ora Fine")]
        [DataType(DataType.DateTime)]
        public DateTime? DataFine { get; set; }

        [Required(ErrorMessage = "Il motivo dell'assegnazione è obbligatorio")]
        [Display(Name = "Motivo Assegnazione")]
        public MotivoAssegnazione MotivoAssegnazione { get; set; }

        [Display(Name = "Chilometraggio Iniziale (km)")]
        [Range(0, 9999999.99, ErrorMessage = "Chilometraggio non valido")]
        public decimal? ChilometraggioInizio { get; set; }

        [Display(Name = "Note")]
        [StringLength(1000, ErrorMessage = "Le note non possono superare i 1000 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        /// <summary>
        /// Indica se è una prenotazione futura (non ancora iniziata)
        /// </summary>
        public bool IsPrenotazione => DataInizio > DateTime.Now;

        /// <summary>
        /// Indica se è un'assegnazione a tempo indeterminato
        /// </summary>
        public bool IsTempoIndeterminato => !DataFine.HasValue;

        /// <summary>
        /// Durata in giorni (solo se DataFine popolata)
        /// </summary>
        public int? DurataGiorni
        {
            get
            {
                if (!DataFine.HasValue)
                    return null;

                return (DataFine.Value.Date - DataInizio.Date).Days;
            }
        }

        /// <summary>
        /// Durata in ore (solo se DataFine popolata)
        /// </summary>
        public double? DurataOre
        {
            get
            {
                if (!DataFine.HasValue)
                    return null;

                return (DataFine.Value - DataInizio).TotalHours;
            }
        }

        // Proprietà calcolate

        /// <summary>
        /// Validazione personalizzata del ViewModel
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validazione: DataFine deve essere successiva a DataInizio
            if (DataFine.HasValue && DataFine.Value <= DataInizio)
            {
                yield return new ValidationResult(
                    "La data e ora di fine deve essere successiva alla data e ora di inizio",
                    new[] { nameof(DataFine) });
            }

            // Validazione: DataInizio non troppo nel passato (max 7 giorni)
            if (DataInizio.Date < DateTime.Today.AddDays(-7))
            {
                yield return new ValidationResult(
                    "La data di inizio non può essere più vecchia di 7 giorni",
                    new[] { nameof(DataInizio) });
            }

            // Validazione: DataFine non troppo nel futuro (max 1 anno)
            if (DataFine.HasValue && DataFine.Value > DateTime.Today.AddYears(1))
            {
                yield return new ValidationResult(
                    "La data di fine non può essere oltre 1 anno nel futuro",
                    new[] { nameof(DataFine) });
            }

            // Validazione: Se DataFine è popolata, durata minima 1 ora
            if (DataFine.HasValue && DurataOre < 1)
            {
                yield return new ValidationResult(
                    "L'assegnazione deve durare almeno 1 ora",
                    new[] { nameof(DataFine) });
            }
        }
    }
}