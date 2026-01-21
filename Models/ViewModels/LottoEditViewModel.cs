using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la modifica di un lotto esistente
    /// Include l'ID e permette la modifica dello stato
    /// </summary>
    public class LottoEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "La gara di riferimento è obbligatoria")]
        public Guid GaraId { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Required(ErrorMessage = "Il codice lotto è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il codice lotto non può superare i 50 caratteri")]
        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        [StringLength(1000, ErrorMessage = "La descrizione non può superare i 1000 caratteri")]
        [Display(Name = "Descrizione")]
        [DataType(DataType.MultilineText)]
        public string Descrizione { get; set; } = string.Empty;

        [Required(ErrorMessage = "La tipologia è obbligatoria")]
        [Display(Name = "Tipologia")]
        public TipologiaLotto Tipologia { get; set; }

        [Required(ErrorMessage = "Lo stato è obbligatorio")]
        [Display(Name = "Stato")]
        public StatoLotto Stato { get; set; }

        // ===== RIFIUTO =====

        [StringLength(2000, ErrorMessage = "Il motivo di rifiuto non può superare i 2000 caratteri")]
        [Display(Name = "Motivo Rifiuto")]
        [DataType(DataType.MultilineText)]
        public string? MotivoRifiuto { get; set; }

        // ===== INFO GENERALI =====

        [StringLength(500, ErrorMessage = "Il link non può superare i 500 caratteri")]
        [Display(Name = "Link Piattaforma")]
        [Url(ErrorMessage = "Inserire un URL valido")]
        public string? LinkPiattaforma { get; set; }

        [Display(Name = "Operatore Assegnato")]
        public string? OperatoreAssegnatoId { get; set; }

        [Display(Name = "Giorni Fornitura")]
        [Range(1, 3650, ErrorMessage = "I giorni di fornitura devono essere compresi tra 1 e 3650")]
        public int? GiorniFornitura { get; set; }

        // ===== INFO ECONOMICHE =====

        [Display(Name = "Importo Base Asta")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "L'importo base asta deve essere un valore positivo")]
        public decimal? ImportoBaseAsta { get; set; }

        [Display(Name = "Quotazione")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "La quotazione deve essere un valore positivo")]
        public decimal? Quotazione { get; set; }

        // ===== INFO CONTRATTO =====

        [StringLength(200, ErrorMessage = "La durata del contratto non può superare i 200 caratteri")]
        [Display(Name = "Durata Contratto")]
        public string? DurataContratto { get; set; }

        [Display(Name = "Data Stipula Contratto")]
        [DataType(DataType.Date)]
        public DateTime? DataStipulaContratto { get; set; }

        [Display(Name = "Data Scadenza Contratto")]
        [DataType(DataType.Date)]
        public DateTime? DataScadenzaContratto { get; set; }

        [StringLength(500, ErrorMessage = "Le modalità di fatturazione non possono superare i 500 caratteri")]
        [Display(Name = "Fatturazione")]
        [DataType(DataType.MultilineText)]
        public string? Fatturazione { get; set; }

        // ===== INFO PARTECIPAZIONE =====

        [Display(Name = "Richiede Fideiussione")]
        public bool RichiedeFideiussione { get; set; } = false;

        // ===== CHECKLIST DOCUMENTI RICHIESTI =====

        /// <summary>
        /// Lista degli ID dei tipi documento richiesti per questo lotto
        /// </summary>
        [Display(Name = "Documenti Richiesti")]
        public List<Guid> DocumentiRichiestiIds { get; set; } = new List<Guid>();

        // ===== ESAME ENTE =====

        [Display(Name = "Data Inizio Esame Ente")]
        [DataType(DataType.Date)]
        public DateTime? DataInizioEsameEnte { get; set; }

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
        /// Ente appaltante (per visualizzazione)
        /// </summary>
        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        /// <summary>
        /// Data scadenza offerte dalla gara (per visualizzazione)
        /// </summary>
        [Display(Name = "Scadenza Offerte Gara")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== INFORMAZIONI DI SISTEMA (READ ONLY) =====

        [Display(Name = "Numero Preventivi")]
        public int NumeroPreventivi { get; set; }

        [Display(Name = "Numero Richieste Integrazione")]
        public int NumeroRichiesteIntegrazione { get; set; }

        [Display(Name = "Numero Partecipanti")]
        public int NumeroPartecipanti { get; set; }

        [Display(Name = "Numero Documenti")]
        public int NumeroDocumenti { get; set; }

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
        /// Validazione custom per le date del contratto e altri vincoli
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validazione date contratto
            if (DataStipulaContratto.HasValue &&
                DataScadenzaContratto.HasValue &&
                DataScadenzaContratto.Value <= DataStipulaContratto.Value)
            {
                yield return new ValidationResult(
                    "La data scadenza contratto deve essere successiva alla data di stipula",
                    new[] { nameof(DataScadenzaContratto) });
            }

            // Validazione importi
            if (ImportoBaseAsta.HasValue &&
                Quotazione.HasValue &&
                Quotazione.Value > ImportoBaseAsta.Value)
            {
                yield return new ValidationResult(
                    "La quotazione non può essere superiore all'importo base d'asta",
                    new[] { nameof(Quotazione) });
            }

            // Validazione data inizio esame
            if (DataInizioEsameEnte.HasValue &&
                DataTerminePresentazioneOfferte.HasValue &&
                DataInizioEsameEnte.Value < DataTerminePresentazioneOfferte.Value)
            {
                yield return new ValidationResult(
                    "La data inizio esame ente deve essere successiva alla scadenza presentazione offerte",
                    new[] { nameof(DataInizioEsameEnte) });
            }

            // Validazione motivo rifiuto
            if (Stato == StatoLotto.Rifiutato && string.IsNullOrWhiteSpace(MotivoRifiuto))
            {
                yield return new ValidationResult(
                    "Se il lotto è rifiutato, è necessario specificare il motivo",
                    new[] { nameof(MotivoRifiuto) });
            }

            // Validazione data stipula contratto per lotti vinti
            if (Stato == StatoLotto.Vinto && !DataStipulaContratto.HasValue)
            {
                yield return new ValidationResult(
                    "Per un lotto vinto è consigliabile specificare la data di stipula del contratto",
                    new[] { nameof(DataStipulaContratto) });
            }
        }
    }
}