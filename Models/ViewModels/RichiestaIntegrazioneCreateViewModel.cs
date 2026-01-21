using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la creazione di una nuova richiesta di integrazione
    /// Le richieste sono create quando l'ente richiede integrazioni documentali
    /// Il numero progressivo viene assegnato automaticamente
    /// </summary>
    public class RichiestaIntegrazioneCreateViewModel
    {
        // ===== RELAZIONE (OBBLIGATORIA) =====

        [Required(ErrorMessage = "Il lotto di riferimento è obbligatorio")]
        public Guid LottoId { get; set; }

        // ===== RICHIESTA ENTE (OBBLIGATORIA) =====

        [Required(ErrorMessage = "La data richiesta ente è obbligatoria")]
        [Display(Name = "Data Richiesta Ente")]
        [DataType(DataType.Date)]
        public DateTime DataRichiestaEnte { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Il testo della richiesta è obbligatorio")]
        [StringLength(5000, ErrorMessage = "Il testo della richiesta non può superare i 5000 caratteri")]
        [Display(Name = "Testo Richiesta Ente")]
        [DataType(DataType.MultilineText)]
        public string TestoRichiestaEnte { get; set; } = string.Empty;

        [Display(Name = "Documento Richiesta Ente (opzionale)")]
        public IFormFile? DocumentoRichiestaFile { get; set; }

        // ===== INFORMAZIONI DI CONTESTO (READ ONLY PER L'UI) =====

        /// <summary>
        /// Numero progressivo (assegnato automaticamente)
        /// </summary>
        [Display(Name = "Numero Progressivo")]
        public int? NumeroProgressivoSuggerito { get; set; }

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

        /// <summary>
        /// Numero di richieste già esistenti per questo lotto (per contesto)
        /// </summary>
        [Display(Name = "Richieste Esistenti")]
        public int RichiesteEsistentiLotto { get; set; }

        // ===== VALIDAZIONI CUSTOM =====

        /// <summary>
        /// Validazione custom per le date e documenti
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

            // Validazione dimensione file
            if (DocumentoRichiestaFile != null && DocumentoRichiestaFile.Length > 10 * 1024 * 1024) // 10 MB
            {
                yield return new ValidationResult(
                    "Il file non può superare i 10 MB",
                    new[] { nameof(DocumentoRichiestaFile) });
            }

            // Validazione estensione file
            if (DocumentoRichiestaFile != null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip", ".rar" };
                var extension = Path.GetExtension(DocumentoRichiestaFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    yield return new ValidationResult(
                        "Sono ammessi solo file PDF, Word, Excel o archivi compressi",
                        new[] { nameof(DocumentoRichiestaFile) });
                }
            }

            // Validazione lunghezza testo minima
            if (TestoRichiestaEnte.Length < 10)
            {
                yield return new ValidationResult(
                    "Il testo della richiesta deve contenere almeno 10 caratteri",
                    new[] { nameof(TestoRichiestaEnte) });
            }
        }
    }
}