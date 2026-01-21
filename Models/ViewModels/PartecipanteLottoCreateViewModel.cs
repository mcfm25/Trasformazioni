using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la creazione di un nuovo Partecipante Lotto
    /// Include validazioni business logic per flags e campi obbligatori condizionali
    /// </summary>
    public class PartecipanteLottoCreateViewModel : IValidatableObject
    {
        // ===== RELAZIONE OBBLIGATORIA =====
        [Required(ErrorMessage = "Il lotto è obbligatorio")]
        [Display(Name = "Lotto")]
        public Guid LottoId { get; set; }

        // ===== SOGGETTO (opzionale) =====
        /// <summary>
        /// ID del Soggetto esistente (opzionale)
        /// Se null, RagioneSociale diventa obbligatoria
        /// </summary>
        [Display(Name = "Soggetto")]
        public Guid? SoggettoId { get; set; }

        // ===== DATI PARTECIPANTE =====
        /// <summary>
        /// Obbligatoria se SoggettoId è null
        /// Validazione gestita in IValidatableObject
        /// </summary>
        [Display(Name = "Ragione Sociale")]
        [StringLength(200, ErrorMessage = "La ragione sociale non può superare i 200 caratteri")]
        public string? RagioneSociale { get; set; }

        [Display(Name = "Offerta Economica")]
        [DataType(DataType.Currency)]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "L'offerta economica deve essere maggiore o uguale a 0")]
        public decimal? OffertaEconomica { get; set; }

        // ===== FLAGS STATO =====
        [Display(Name = "È l'aggiudicatario")]
        public bool IsAggiudicatario { get; set; } = false;

        [Display(Name = "È stato scartato dall'ente")]
        public bool IsScartatoDallEnte { get; set; } = false;

        // ===== NOTE =====
        [Display(Name = "Note")]
        [StringLength(2000, ErrorMessage = "Le note non possono superare i 2000 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // ===== VALIDAZIONI CUSTOM =====
        /// <summary>
        /// Implementa validazioni business logic:
        /// - RagioneSociale obbligatoria se SoggettoId è null
        /// - IsAggiudicatario e IsScartatoDallEnte non possono essere entrambi true
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validazione 1: Se SoggettoId è null → RagioneSociale obbligatoria
            if (!SoggettoId.HasValue && string.IsNullOrWhiteSpace(RagioneSociale))
            {
                yield return new ValidationResult(
                    "La Ragione Sociale è obbligatoria se non si seleziona un Soggetto esistente",
                    new[] { nameof(RagioneSociale) }
                );
            }

            // Validazione 2: Se SoggettoId presente → RagioneSociale non necessaria (verrà ignorata)
            // Questa è solo informativa, non blocca

            // Validazione 3: IsAggiudicatario e IsScartatoDallEnte non possono essere entrambi true
            if (IsAggiudicatario && IsScartatoDallEnte)
            {
                yield return new ValidationResult(
                    "Un partecipante non può essere contemporaneamente Aggiudicatario e Scartato dall'Ente",
                    new[] { nameof(IsAggiudicatario), nameof(IsScartatoDallEnte) }
                );
            }

            // Validazione 4: RagioneSociale non deve essere vuota se specificata
            if (!string.IsNullOrWhiteSpace(RagioneSociale) && RagioneSociale.Trim().Length < 2)
            {
                yield return new ValidationResult(
                    "La Ragione Sociale deve contenere almeno 2 caratteri",
                    new[] { nameof(RagioneSociale) }
                );
            }

            // Validazione 5: OffertaEconomica >= 0 se valorizzata
            if (OffertaEconomica.HasValue && OffertaEconomica.Value < 0)
            {
                yield return new ValidationResult(
                    "L'Offerta Economica deve essere maggiore o uguale a 0",
                    new[] { nameof(OffertaEconomica) }
                );
            }
        }
    }
}