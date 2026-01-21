using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la modifica di un preventivo esistente
    /// Include l'ID e permette la modifica dello stato
    /// </summary>
    public class PreventivoEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Il lotto di riferimento è obbligatorio")]
        public Guid LottoId { get; set; }

        [Required(ErrorMessage = "Il fornitore è obbligatorio")]
        [Display(Name = "Fornitore")]
        public Guid SoggettoId { get; set; }

        // ===== IDENTIFICAZIONE =====

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

        // ===== DATE =====

        [Required(ErrorMessage = "La data richiesta è obbligatoria")]
        [Display(Name = "Data Richiesta")]
        [DataType(DataType.Date)]
        public DateTime DataRichiesta { get; set; }

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

        // ===== DOCUMENTO =====

        [Display(Name = "Documento Corrente")]
        public string? DocumentPathCorrente { get; set; }

        [Display(Name = "Nome File Corrente")]
        public string? NomeFileCorrente { get; set; }

        [Display(Name = "Nuovo Documento (opzionale)")]
        public IFormFile? NuovoDocumentoFile { get; set; }

        [Display(Name = "Mantieni Documento Esistente")]
        public bool MantieniFIleEsistente { get; set; } = true;

        // ===== SELEZIONE =====

        [Display(Name = "Selezionato")]
        public bool IsSelezionato { get; set; }

        // ===== NOTE =====

        [StringLength(2000, ErrorMessage = "Le note non possono superare i 2000 caratteri")]
        [Display(Name = "Note")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // ===== INFORMAZIONI DI CONTESTO (READ ONLY PER L'UI) =====

        /// <summary>
        /// Nome del fornitore (per visualizzazione)
        /// </summary>
        [Display(Name = "Fornitore")]
        public string? NomeFornitore { get; set; }

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

        // ===== INFORMAZIONI DI SISTEMA (READ ONLY) =====

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

        // ===== PROPRIETÀ COMPUTATE =====

        /// <summary>
        /// Indica se ha un documento allegato
        /// </summary>
        public bool HasDocumento => !string.IsNullOrEmpty(DocumentPathCorrente);

        // ===== VALIDAZIONI CUSTOM =====

        /// <summary>
        /// Validazione custom per le date e altri vincoli
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

            // Validazione stato ricevuto richiede data ricezione
            if ((Stato == StatoPreventivo.Ricevuto ||
                 Stato == StatoPreventivo.Valido ||
                 Stato == StatoPreventivo.InScadenza) &&
                !DataRicezione.HasValue)
            {
                yield return new ValidationResult(
                    "Per lo stato selezionato è necessario specificare la data di ricezione",
                    new[] { nameof(DataRicezione) });
            }

            // Validazione stato ricevuto richiede importo
            if ((Stato == StatoPreventivo.Ricevuto ||
                 Stato == StatoPreventivo.Valido ||
                 Stato == StatoPreventivo.InScadenza) &&
                !ImportoOfferto.HasValue)
            {
                yield return new ValidationResult(
                    "Per lo stato selezionato è necessario specificare l'importo offerto",
                    new[] { nameof(ImportoOfferto) });
            }

            // Validazione selezione richiede stato valido
            if (IsSelezionato &&
                Stato != StatoPreventivo.Valido &&
                Stato != StatoPreventivo.Ricevuto)
            {
                yield return new ValidationResult(
                    "È possibile selezionare solo preventivi con stato 'Valido' o 'Ricevuto'",
                    new[] { nameof(IsSelezionato) });
            }

            // Validazione dimensione nuovo file
            if (NuovoDocumentoFile != null && NuovoDocumentoFile.Length > 10 * 1024 * 1024) // 10 MB
            {
                yield return new ValidationResult(
                    "Il file non può superare i 10 MB",
                    new[] { nameof(NuovoDocumentoFile) });
            }

            // Validazione estensione nuovo file
            if (NuovoDocumentoFile != null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
                var extension = Path.GetExtension(NuovoDocumentoFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    yield return new ValidationResult(
                        "Sono ammessi solo file PDF, Word o Excel",
                        new[] { nameof(NuovoDocumentoFile) });
                }
            }

            // Validazione coerenza file
            if (!MantieniFIleEsistente && NuovoDocumentoFile == null && HasDocumento)
            {
                yield return new ValidationResult(
                    "Se si sceglie di non mantenere il file esistente, è necessario caricare un nuovo documento",
                    new[] { nameof(NuovoDocumentoFile) });
            }
        }
    }
}