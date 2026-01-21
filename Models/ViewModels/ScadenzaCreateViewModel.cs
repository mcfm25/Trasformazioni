using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la creazione di una nuova scadenza
    /// Contiene solo i campi necessari per la creazione iniziale
    /// Le scadenze manuali possono essere create dall'utente
    /// Le scadenze automatiche sono create dal sistema
    /// </summary>
    public class ScadenzaCreateViewModel
    {
        // ===== RELAZIONI (OPZIONALI) =====

        [Display(Name = "Gara")]
        public Guid? GaraId { get; set; }

        [Display(Name = "Lotto")]
        public Guid? LottoId { get; set; }

        [Display(Name = "Preventivo")]
        public Guid? PreventivoId { get; set; }

        // ===== IDENTIFICAZIONE (OBBLIGATORI) =====

        [Required(ErrorMessage = "Il tipo di scadenza è obbligatorio")]
        [Display(Name = "Tipo")]
        public TipoScadenza Tipo { get; set; }

        [Required(ErrorMessage = "La data scadenza è obbligatoria")]
        [Display(Name = "Data Scadenza")]
        [DataType(DataType.Date)]
        public DateTime DataScadenza { get; set; }

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri")]
        [Display(Name = "Descrizione")]
        [DataType(DataType.MultilineText)]
        public string Descrizione { get; set; } = string.Empty;

        // ===== CONFIGURAZIONE =====

        [Required(ErrorMessage = "I giorni di preavviso sono obbligatori")]
        [Range(0, 365, ErrorMessage = "I giorni di preavviso devono essere compresi tra 0 e 365")]
        [Display(Name = "Giorni Preavviso")]
        public int GiorniPreavviso { get; set; } = 7;

        // ===== NOTE =====

        [StringLength(2000, ErrorMessage = "Le note non possono superare i 2000 caratteri")]
        [Display(Name = "Note")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // ===== INFORMAZIONI DI CONTESTO (READ ONLY PER L'UI) =====

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
        /// Fornitore del preventivo (per visualizzazione)
        /// </summary>
        [Display(Name = "Fornitore")]
        public string? FornitorePreventivo { get; set; }

        /// <summary>
        /// Data scadenza offerte dalla gara (per riferimento)
        /// </summary>
        [Display(Name = "Scadenza Offerte Gara")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== VALIDAZIONI CUSTOM =====

        /// <summary>
        /// Validazione custom per le relazioni e date
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validazione: almeno una relazione o nessuna (scadenza generica)
            // Non è un errore non avere relazioni

            // Validazione: se ha lotto, deve avere anche gara
            if (LottoId.HasValue && !GaraId.HasValue)
            {
                yield return new ValidationResult(
                    "Se si specifica un lotto, è necessario specificare anche la gara di appartenenza",
                    new[] { nameof(LottoId) });
            }

            // Validazione: se ha preventivo, deve avere anche lotto
            if (PreventivoId.HasValue && !LottoId.HasValue)
            {
                yield return new ValidationResult(
                    "Se si specifica un preventivo, è necessario specificare anche il lotto di appartenenza",
                    new[] { nameof(PreventivoId) });
            }

            // Validazione: data scadenza non può essere troppo nel passato (max 30 giorni)
            if (DataScadenza < DateTime.Today.AddDays(-30))
            {
                yield return new ValidationResult(
                    "La data di scadenza non può essere più vecchia di 30 giorni",
                    new[] { nameof(DataScadenza) });
            }

            // Warning: data scadenza nel passato
            if (DataScadenza < DateTime.Today)
            {
                // Questo è un warning, non un errore bloccante
                // L'utente potrebbe voler creare una scadenza già passata per tracking storico
            }

            // Validazione: coerenza con tipo di scadenza
            if (Tipo == TipoScadenza.PresentazioneOfferta && !GaraId.HasValue)
            {
                yield return new ValidationResult(
                    "Per il tipo 'Presentazione Offerta' è necessario specificare una gara",
                    new[] { nameof(Tipo) });
            }

            if (Tipo == TipoScadenza.RichiestaChiarimenti && !GaraId.HasValue)
            {
                yield return new ValidationResult(
                    "Per il tipo 'Richiesta Chiarimenti' è necessario specificare una gara",
                    new[] { nameof(Tipo) });
            }

            if (Tipo == TipoScadenza.ScadenzaPreventivo && !PreventivoId.HasValue)
            {
                yield return new ValidationResult(
                    "Per il tipo 'Scadenza Preventivo' è necessario specificare un preventivo",
                    new[] { nameof(Tipo) });
            }

            if ((Tipo == TipoScadenza.StipulaContratto || Tipo == TipoScadenza.ScadenzaContratto) &&
                !LottoId.HasValue)
            {
                yield return new ValidationResult(
                    $"Per il tipo '{Tipo}' è necessario specificare un lotto",
                    new[] { nameof(Tipo) });
            }

            // Validazione: coerenza con data scadenza gara
            if (Tipo == TipoScadenza.PresentazioneOfferta &&
                DataTerminePresentazioneOfferte.HasValue &&
                DataScadenza.Date != DataTerminePresentazioneOfferte.Value.Date)
            {
                yield return new ValidationResult(
                    $"Per il tipo 'Presentazione Offerta' la data dovrebbe coincidere con la scadenza della gara ({DataTerminePresentazioneOfferte:dd/MM/yyyy})",
                    new[] { nameof(DataScadenza) });
            }
        }
    }
}