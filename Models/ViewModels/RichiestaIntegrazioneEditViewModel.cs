using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la modifica di una richiesta di integrazione esistente
    /// Include l'ID e permette di gestire sia la richiesta che la risposta
    /// </summary>
    public class RichiestaIntegrazioneEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Il lotto di riferimento è obbligatorio")]
        public Guid LottoId { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Display(Name = "Numero Progressivo")]
        public int NumeroProgressivo { get; set; }

        [Display(Name = "Chiusa")]
        public bool IsChiusa { get; set; }

        // ===== RICHIESTA ENTE =====

        [Required(ErrorMessage = "La data richiesta ente è obbligatoria")]
        [Display(Name = "Data Richiesta Ente")]
        [DataType(DataType.Date)]
        public DateTime DataRichiestaEnte { get; set; }

        [Required(ErrorMessage = "Il testo della richiesta è obbligatorio")]
        [StringLength(5000, ErrorMessage = "Il testo della richiesta non può superare i 5000 caratteri")]
        [Display(Name = "Testo Richiesta Ente")]
        [DataType(DataType.MultilineText)]
        public string TestoRichiestaEnte { get; set; } = string.Empty;

        [Display(Name = "Documento Richiesta Corrente")]
        public string? DocumentoRichiestaPathCorrente { get; set; }

        [Display(Name = "Nome File Richiesta Corrente")]
        public string? NomeFileRichiestaCorrente { get; set; }

        [Display(Name = "Nuovo Documento Richiesta (opzionale)")]
        public IFormFile? NuovoDocumentoRichiestaFile { get; set; }

        [Display(Name = "Mantieni Documento Richiesta Esistente")]
        public bool MantieniDocumentoRichiestaEsistente { get; set; } = true;

        // ===== RISPOSTA AZIENDA =====

        [Display(Name = "Data Risposta Azienda")]
        [DataType(DataType.Date)]
        public DateTime? DataRispostaAzienda { get; set; }

        [StringLength(5000, ErrorMessage = "Il testo della risposta non può superare i 5000 caratteri")]
        [Display(Name = "Testo Risposta Azienda")]
        [DataType(DataType.MultilineText)]
        public string? TestoRispostaAzienda { get; set; }

        [Display(Name = "Documento Risposta Corrente")]
        public string? DocumentoRispostaPathCorrente { get; set; }

        [Display(Name = "Nome File Risposta Corrente")]
        public string? NomeFileRispostaCorrente { get; set; }

        [Display(Name = "Nuovo Documento Risposta (opzionale)")]
        public IFormFile? NuovoDocumentoRispostaFile { get; set; }

        [Display(Name = "Mantieni Documento Risposta Esistente")]
        public bool MantieniDocumentoRispostaEsistente { get; set; } = true;

        [Display(Name = "Risposto Da (User ID)")]
        public string? RispostaDaUserId { get; set; }

        // ===== INFORMAZIONI DI CONTESTO (READ ONLY PER L'UI) =====

        /// <summary>
        /// Nome dell'utente che ha risposto (per visualizzazione)
        /// </summary>
        [Display(Name = "Risposto Da")]
        public string? RispostoDaNome { get; set; }

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
        /// Titolo della gara (per visualizzazione)
        /// </summary>
        [Display(Name = "Titolo Gara")]
        public string? TitoloGara { get; set; }

        /// <summary>
        /// Ente appaltante (per visualizzazione)
        /// </summary>
        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        /// <summary>
        /// Data scadenza offerte dalla gara (per riferimento)
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
        /// Indica se ha documento richiesta allegato
        /// </summary>
        public bool HasDocumentoRichiesta => !string.IsNullOrEmpty(DocumentoRichiestaPathCorrente);

        /// <summary>
        /// Indica se ha documento risposta allegato
        /// </summary>
        public bool HasDocumentoRisposta => !string.IsNullOrEmpty(DocumentoRispostaPathCorrente);

        /// <summary>
        /// Indica se ha una risposta
        /// </summary>
        public bool HasRisposta => DataRispostaAzienda.HasValue;

        // ===== VALIDAZIONI CUSTOM =====

        /// <summary>
        /// Validazione custom per le date, documenti e stato
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validazione data richiesta
            if (DataRichiestaEnte > DateTime.Today)
            {
                yield return new ValidationResult(
                    "La data richiesta ente non può essere nel futuro",
                    new[] { nameof(DataRichiestaEnte) });
            }

            // Validazione data richiesta non troppo vecchia
            if (DataRichiestaEnte < DateTime.Today.AddMonths(-6))
            {
                yield return new ValidationResult(
                    "La data richiesta ente non può essere più vecchia di 6 mesi",
                    new[] { nameof(DataRichiestaEnte) });
            }

            // Validazione coerenza con scadenza gara
            if (DataTerminePresentazioneOfferte.HasValue &&
                DataRichiestaEnte < DataTerminePresentazioneOfferte.Value.Date)
            {
                yield return new ValidationResult(
                    $"La data richiesta ente non può essere precedente alla scadenza offerte della gara ({DataTerminePresentazioneOfferte:dd/MM/yyyy})",
                    new[] { nameof(DataRichiestaEnte) });
            }

            // Validazione data risposta
            if (DataRispostaAzienda.HasValue && DataRispostaAzienda.Value < DataRichiestaEnte)
            {
                yield return new ValidationResult(
                    "La data risposta azienda deve essere successiva o uguale alla data richiesta ente",
                    new[] { nameof(DataRispostaAzienda) });
            }

            // Validazione data risposta non nel futuro
            if (DataRispostaAzienda.HasValue && DataRispostaAzienda.Value > DateTime.Today)
            {
                yield return new ValidationResult(
                    "La data risposta azienda non può essere nel futuro",
                    new[] { nameof(DataRispostaAzienda) });
            }

            // Validazione testo risposta se data risposta presente
            if (DataRispostaAzienda.HasValue && string.IsNullOrWhiteSpace(TestoRispostaAzienda))
            {
                yield return new ValidationResult(
                    "Se si specifica una data risposta, è necessario inserire il testo della risposta",
                    new[] { nameof(TestoRispostaAzienda) });
            }

            // Validazione chiusura richiesta
            if (IsChiusa && !DataRispostaAzienda.HasValue)
            {
                yield return new ValidationResult(
                    "Non è possibile chiudere una richiesta che non ha ancora una risposta",
                    new[] { nameof(IsChiusa) });
            }

            // Validazione dimensione nuovo file richiesta
            if (NuovoDocumentoRichiestaFile != null && NuovoDocumentoRichiestaFile.Length > 10 * 1024 * 1024) // 10 MB
            {
                yield return new ValidationResult(
                    "Il file richiesta non può superare i 10 MB",
                    new[] { nameof(NuovoDocumentoRichiestaFile) });
            }

            // Validazione estensione nuovo file richiesta
            if (NuovoDocumentoRichiestaFile != null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip", ".rar" };
                var extension = Path.GetExtension(NuovoDocumentoRichiestaFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    yield return new ValidationResult(
                        "Sono ammessi solo file PDF, Word, Excel o archivi compressi",
                        new[] { nameof(NuovoDocumentoRichiestaFile) });
                }
            }

            // Validazione dimensione nuovo file risposta
            if (NuovoDocumentoRispostaFile != null && NuovoDocumentoRispostaFile.Length > 10 * 1024 * 1024) // 10 MB
            {
                yield return new ValidationResult(
                    "Il file risposta non può superare i 10 MB",
                    new[] { nameof(NuovoDocumentoRispostaFile) });
            }

            // Validazione estensione nuovo file risposta
            if (NuovoDocumentoRispostaFile != null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip", ".rar" };
                var extension = Path.GetExtension(NuovoDocumentoRispostaFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    yield return new ValidationResult(
                        "Sono ammessi solo file PDF, Word, Excel o archivi compressi",
                        new[] { nameof(NuovoDocumentoRispostaFile) });
                }
            }

            // Validazione coerenza file richiesta
            if (!MantieniDocumentoRichiestaEsistente &&
                NuovoDocumentoRichiestaFile == null &&
                HasDocumentoRichiesta)
            {
                yield return new ValidationResult(
                    "Se si sceglie di non mantenere il documento richiesta esistente, è necessario caricare un nuovo documento",
                    new[] { nameof(NuovoDocumentoRichiestaFile) });
            }

            // Validazione coerenza file risposta
            if (!MantieniDocumentoRispostaEsistente &&
                NuovoDocumentoRispostaFile == null &&
                HasDocumentoRisposta)
            {
                yield return new ValidationResult(
                    "Se si sceglie di non mantenere il documento risposta esistente, è necessario caricare un nuovo documento",
                    new[] { nameof(NuovoDocumentoRispostaFile) });
            }

            // Validazione lunghezza testo minima
            if (TestoRichiestaEnte.Length < 10)
            {
                yield return new ValidationResult(
                    "Il testo della richiesta deve contenere almeno 10 caratteri",
                    new[] { nameof(TestoRichiestaEnte) });
            }

            // Validazione lunghezza testo risposta minima (se presente)
            if (!string.IsNullOrEmpty(TestoRispostaAzienda) && TestoRispostaAzienda.Length < 10)
            {
                yield return new ValidationResult(
                    "Il testo della risposta deve contenere almeno 10 caratteri",
                    new[] { nameof(TestoRispostaAzienda) });
            }
        }
    }
}