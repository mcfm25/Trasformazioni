using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la modifica di una scadenza esistente
    /// Include l'ID e permette la modifica dello stato di completamento
    /// Le scadenze automatiche hanno limitazioni sulla modifica
    /// </summary>
    public class ScadenzaEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        // ===== RELAZIONI (OPZIONALI) =====

        [Display(Name = "Gara")]
        public Guid? GaraId { get; set; }

        [Display(Name = "Lotto")]
        public Guid? LottoId { get; set; }

        [Display(Name = "Preventivo")]
        public Guid? PreventivoId { get; set; }

        // ===== IDENTIFICAZIONE =====

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

        // ===== STATO =====

        [Display(Name = "Automatica")]
        public bool IsAutomatica { get; set; }

        [Display(Name = "Completata")]
        public bool IsCompletata { get; set; }

        [Display(Name = "Data Completamento")]
        [DataType(DataType.Date)]
        public DateTime? DataCompletamento { get; set; }

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
        /// Indica se la scadenza può essere modificata liberamente
        /// Le scadenze automatiche hanno limitazioni
        /// </summary>
        public bool CanEditFully => !IsAutomatica;

        /// <summary>
        /// Indica se può modificare solo lo stato di completamento
        /// </summary>
        public bool CanEditOnlyCompletion => IsAutomatica;

        // ===== VALIDAZIONI CUSTOM =====

        /// <summary>
        /// Validazione custom per le relazioni, date e stato
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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

            // Validazione: data scadenza non può essere troppo nel passato (max 30 giorni) per scadenze non completate
            if (!IsCompletata && DataScadenza < DateTime.Today.AddDays(-30))
            {
                yield return new ValidationResult(
                    "La data di scadenza di una scadenza attiva non può essere più vecchia di 30 giorni",
                    new[] { nameof(DataScadenza) });
            }

            // Validazione: data completamento deve essere valorizzata se completata
            if (IsCompletata && !DataCompletamento.HasValue)
            {
                yield return new ValidationResult(
                    "Se la scadenza è completata, è necessario specificare la data di completamento",
                    new[] { nameof(DataCompletamento) });
            }

            // Validazione: data completamento non può essere precedente alla creazione
            if (DataCompletamento.HasValue && DataCompletamento.Value < CreatedAt)
            {
                yield return new ValidationResult(
                    "La data di completamento non può essere precedente alla data di creazione della scadenza",
                    new[] { nameof(DataCompletamento) });
            }

            // Validazione: data completamento non può essere nel futuro
            if (DataCompletamento.HasValue && DataCompletamento.Value > DateTime.Today)
            {
                yield return new ValidationResult(
                    "La data di completamento non può essere nel futuro",
                    new[] { nameof(DataCompletamento) });
            }

            // Validazione: se non completata, data completamento deve essere null
            if (!IsCompletata && DataCompletamento.HasValue)
            {
                yield return new ValidationResult(
                    "Se la scadenza non è completata, la data di completamento deve essere vuota",
                    new[] { nameof(DataCompletamento) });
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

            // Validazione: coerenza con data scadenza gara per scadenze non automatiche
            if (!IsAutomatica &&
                Tipo == TipoScadenza.PresentazioneOfferta &&
                DataTerminePresentazioneOfferte.HasValue &&
                DataScadenza.Date != DataTerminePresentazioneOfferte.Value.Date)
            {
                yield return new ValidationResult(
                    $"Per il tipo 'Presentazione Offerta' la data dovrebbe coincidere con la scadenza della gara ({DataTerminePresentazioneOfferte:dd/MM/yyyy})",
                    new[] { nameof(DataScadenza) });
            }

            // Warning: modifica di scadenza automatica (non bloccante, solo informativo)
            if (IsAutomatica)
            {
                // Le scadenze automatiche dovrebbero essere modificate con cautela
                // Il sistema potrebbe rigenerarle automaticamente
            }
        }
    }
}