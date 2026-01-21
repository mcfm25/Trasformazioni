using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la modifica di un Partecipante Lotto esistente
    /// Include ID e validazioni business logic
    /// </summary>
    public class PartecipanteLottoEditViewModel : IValidatableObject
    {
        // ===== IDENTIFICAZIONE =====
        [Required]
        public Guid Id { get; set; }

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
        public bool IsAggiudicatario { get; set; }

        [Display(Name = "È stato scartato dall'ente")]
        public bool IsScartatoDallEnte { get; set; }

        // ===== NOTE =====
        [Display(Name = "Note")]
        [StringLength(2000, ErrorMessage = "Le note non possono superare i 2000 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // ===== INFO VISUALIZZAZIONE (readonly) =====
        /// <summary>
        /// Codice del lotto per visualizzazione (non modificabile)
        /// </summary>
        [Display(Name = "Codice Lotto")]
        public string? CodiceLotto { get; set; }

        /// <summary>
        /// Descrizione del lotto per visualizzazione (non modificabile)
        /// </summary>
        [Display(Name = "Descrizione Lotto")]
        public string? DescrizioneLotto { get; set; }

        /// <summary>
        /// Nome del soggetto collegato per visualizzazione (non modificabile)
        /// </summary>
        [Display(Name = "Soggetto Collegato")]
        public string? NomeSoggetto { get; set; }

        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Creato Da")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Modificato Da")]
        public string? ModifiedBy { get; set; }

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

            // Validazione 2: IsAggiudicatario e IsScartatoDallEnte non possono essere entrambi true
            if (IsAggiudicatario && IsScartatoDallEnte)
            {
                yield return new ValidationResult(
                    "Un partecipante non può essere contemporaneamente Aggiudicatario e Scartato dall'Ente",
                    new[] { nameof(IsAggiudicatario), nameof(IsScartatoDallEnte) }
                );
            }

            // Validazione 3: RagioneSociale non deve essere vuota se specificata
            if (!string.IsNullOrWhiteSpace(RagioneSociale) && RagioneSociale.Trim().Length < 2)
            {
                yield return new ValidationResult(
                    "La Ragione Sociale deve contenere almeno 2 caratteri",
                    new[] { nameof(RagioneSociale) }
                );
            }

            // Validazione 4: OffertaEconomica >= 0 se valorizzata
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