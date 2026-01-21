using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la creazione di una nuova Elaborazione Lotto
    /// Include validazioni business logic per prezzi e motivazione
    /// </summary>
    public class ElaborazioneLottoCreateViewModel : IValidatableObject
    {
        // ===== RELAZIONE OBBLIGATORIA =====
        [Required(ErrorMessage = "Il lotto è obbligatorio")]
        [Display(Name = "Lotto")]
        public Guid LottoId { get; set; }

        // ===== PREZZI =====
        [Display(Name = "Prezzo Desiderato")]
        [DataType(DataType.Currency)]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Il prezzo desiderato deve essere maggiore di 0")]
        public decimal? PrezzoDesiderato { get; set; }

        [Display(Name = "Prezzo Reale Uscita")]
        [DataType(DataType.Currency)]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Il prezzo reale deve essere maggiore di 0")]
        public decimal? PrezzoRealeUscita { get; set; }

        // ===== MOTIVAZIONE =====
        /// <summary>
        /// Obbligatoria se PrezzoDesiderato != PrezzoRealeUscita
        /// Validazione gestita in IValidatableObject
        /// </summary>
        [Display(Name = "Motivazione Adattamento")]
        [StringLength(2000, ErrorMessage = "La motivazione non può superare i 2000 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? MotivazioneAdattamento { get; set; }

        // ===== NOTE =====
        [Display(Name = "Note")]
        [StringLength(2000, ErrorMessage = "Le note non possono superare i 2000 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // ===== VALIDAZIONI CUSTOM =====
        /// <summary>
        /// Implementa validazioni business logic:
        /// - MotivazioneAdattamento obbligatoria se PrezzoDesiderato != PrezzoRealeUscita
        /// - Almeno uno dei due prezzi deve essere valorizzato
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validazione 1: Almeno uno dei due prezzi deve essere valorizzato
            if (!PrezzoDesiderato.HasValue && !PrezzoRealeUscita.HasValue)
            {
                yield return new ValidationResult(
                    "Almeno uno tra Prezzo Desiderato e Prezzo Reale deve essere valorizzato",
                    new[] { nameof(PrezzoDesiderato), nameof(PrezzoRealeUscita) }
                );
            }

            // Validazione 2: Se entrambi i prezzi sono valorizzati e diversi → MotivazioneAdattamento obbligatoria
            if (PrezzoDesiderato.HasValue &&
                PrezzoRealeUscita.HasValue &&
                PrezzoDesiderato.Value != PrezzoRealeUscita.Value &&
                string.IsNullOrWhiteSpace(MotivazioneAdattamento))
            {
                yield return new ValidationResult(
                    "La Motivazione Adattamento è obbligatoria quando il Prezzo Desiderato è diverso dal Prezzo Reale",
                    new[] { nameof(MotivazioneAdattamento) }
                );
            }

            // Validazione 3: Se prezzi sono uguali, la motivazione non serve
            if (PrezzoDesiderato.HasValue &&
                PrezzoRealeUscita.HasValue &&
                PrezzoDesiderato.Value == PrezzoRealeUscita.Value &&
                !string.IsNullOrWhiteSpace(MotivazioneAdattamento))
            {
                yield return new ValidationResult(
                    "La Motivazione Adattamento non è necessaria quando i prezzi sono uguali",
                    new[] { nameof(MotivazioneAdattamento) }
                );
            }

            // Validazione 4: Prezzi devono essere > 0 se valorizzati
            if (PrezzoDesiderato.HasValue && PrezzoDesiderato.Value <= 0)
            {
                yield return new ValidationResult(
                    "Il Prezzo Desiderato deve essere maggiore di 0",
                    new[] { nameof(PrezzoDesiderato) }
                );
            }

            if (PrezzoRealeUscita.HasValue && PrezzoRealeUscita.Value <= 0)
            {
                yield return new ValidationResult(
                    "Il Prezzo Reale deve essere maggiore di 0",
                    new[] { nameof(PrezzoRealeUscita) }
                );
            }
        }
    }
}