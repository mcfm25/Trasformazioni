using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la creazione di un nuovo preventivo
    /// Contiene solo i campi necessari per la creazione iniziale
    /// Lo stato viene impostato automaticamente a InAttesa
    /// Il preventivo deve essere sempre associato a un lotto e a un fornitore
    /// </summary>
    public class PreventivoCreateViewModel
    {
        // ===== RELAZIONI (OBBLIGATORIE) =====

        [Required(ErrorMessage = "Il lotto di riferimento è obbligatorio")]
        public Guid LottoId { get; set; }

        [Required(ErrorMessage = "Il fornitore è obbligatorio")]
        [Display(Name = "Fornitore")]
        public Guid SoggettoId { get; set; }

        // ===== IDENTIFICAZIONE (OBBLIGATORI) =====

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        [StringLength(1000, ErrorMessage = "La descrizione non può superare i 1000 caratteri")]
        [Display(Name = "Descrizione")]
        [DataType(DataType.MultilineText)]
        public string Descrizione { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lo stato è obbligatorio")]
        [Display(Name = "Stato")]
        public StatoPreventivo Stato { get; set; }

        // ===== INFO ECONOMICHE =====

        [Display(Name = "Importo Offerto")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "L'importo offerto deve essere un valore positivo")]
        public decimal? ImportoOfferto { get; set; }

        // ===== DATE (OBBLIGATORIE) =====

        [Required(ErrorMessage = "La data richiesta è obbligatoria")]
        [Display(Name = "Data Richiesta")]
        [DataType(DataType.Date)]
        public DateTime DataRichiesta { get; set; } = DateTime.Today;

        [Display(Name = "Data Ricezione")]
        [DataType(DataType.Date)]
        public DateTime? DataRicezione { get; set; }

        [Required(ErrorMessage = "La data scadenza è obbligatoria")]
        [Display(Name = "Data Scadenza")]
        [DataType(DataType.Date)]
        public DateTime DataScadenza { get; set; }

        // ===== AUTO-RINNOVO =====

        [Display(Name = "Giorni Auto-Rinnovo")]
        [Range(1, 365, ErrorMessage = "I giorni di auto-rinnovo devono essere compresi tra 1 e 365")]
        public int? GiorniAutoRinnovo { get; set; }

        // ===== SELEZIONE =====

        [Display(Name = "Selezionato")]
        public bool IsSelezionato { get; set; }

        // ===== DOCUMENTO =====

        [Display(Name = "Documento Preventivo")]
        public IFormFile? DocumentoFile { get; set; }

        // ===== NOTE =====

        [StringLength(2000, ErrorMessage = "Le note non possono superare i 2000 caratteri")]
        [Display(Name = "Note")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // ===== INFORMAZIONI DI CONTESTO (READ ONLY PER L'UI) =====

        /// <summary>
        /// Codice del lotto (per visualizzazione)
        /// </summary>
        [Display(Name = "Codice Lotto")]
        public string? CodiceLotto { get; set; }

        /// <summary>
        /// Descrizione del lotto (per visualizzazione)
        /// </summary>
        [Display(Name = "Descrizione Lotto")]
        public string? DescrizioneLotto { get; set; }

        /// <summary>
        /// Codice della gara (per visualizzazione)
        /// </summary>
        [Display(Name = "Codice Gara")]
        public string? CodiceGara { get; set; }

        /// <summary>
        /// Importo base asta del lotto (per riferimento)
        /// </summary>
        [Display(Name = "Importo Base Asta")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal? ImportoBaseAstaLotto { get; set; }

        /// <summary>
        /// Data scadenza offerte dalla gara (per visualizzazione)
        /// </summary>
        [Display(Name = "Scadenza Offerte Gara")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== VALIDAZIONI CUSTOM =====

        /// <summary>
        /// Validazione custom per le date
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validazione data ricezione
            if (DataRicezione.HasValue && DataRicezione.Value < DataRichiesta)
            {
                yield return new ValidationResult(
                    "La data di ricezione deve essere successiva o uguale alla data di richiesta",
                    new[] { nameof(DataRicezione) });
            }

            // Validazione data scadenza
            if (DataScadenza <= DataRichiesta)
            {
                yield return new ValidationResult(
                    "La data di scadenza deve essere successiva alla data di richiesta",
                    new[] { nameof(DataScadenza) });
            }

            // Validazione data scadenza vs data ricezione
            if (DataRicezione.HasValue && DataScadenza <= DataRicezione.Value)
            {
                yield return new ValidationResult(
                    "La data di scadenza deve essere successiva alla data di ricezione",
                    new[] { nameof(DataScadenza) });
            }

            // Validazione importo vs importo base asta
            if (ImportoOfferto.HasValue &&
                ImportoBaseAstaLotto.HasValue &&
                ImportoOfferto.Value > ImportoBaseAstaLotto.Value * 1.5m)
            {
                yield return new ValidationResult(
                    $"L'importo offerto ({ImportoOfferto:C2}) è significativamente superiore all'importo base d'asta ({ImportoBaseAstaLotto:C2}). Verificare.",
                    new[] { nameof(ImportoOfferto) });
            }

            // Validazione scadenza preventivo vs scadenza offerte
            if (DataTerminePresentazioneOfferte.HasValue &&
                DataScadenza < DataTerminePresentazioneOfferte.Value)
            {
                yield return new ValidationResult(
                    "La data di scadenza del preventivo dovrebbe essere successiva alla scadenza presentazione offerte della gara",
                    new[] { nameof(DataScadenza) });
            }

            // Validazione dimensione file
            if (DocumentoFile != null && DocumentoFile.Length > 10 * 1024 * 1024) // 10 MB
            {
                yield return new ValidationResult(
                    "Il file non può superare i 10 MB",
                    new[] { nameof(DocumentoFile) });
            }

            // Validazione estensione file
            if (DocumentoFile != null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
                var extension = Path.GetExtension(DocumentoFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    yield return new ValidationResult(
                        "Sono ammessi solo file PDF, Word o Excel",
                        new[] { nameof(DocumentoFile) });
                }
            }
        }
    }
}