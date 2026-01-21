using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la modifica di una gara esistente
    /// Include l'ID e permette la modifica dello stato
    /// </summary>
    public class GaraEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Required(ErrorMessage = "Il codice gara è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il codice gara non può superare i 50 caratteri")]
        [Display(Name = "Codice Gara")]
        //[Remote(action: "CheckCodiceGara", controller: "Gare",
        //AdditionalFields = nameof(Id),
        //ErrorMessage = "Questo codice gara è già utilizzato")]
        public string CodiceGara { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il titolo è obbligatorio")]
        [StringLength(500, ErrorMessage = "Il titolo non può superare i 500 caratteri")]
        [Display(Name = "Titolo")]
        public string Titolo { get; set; } = string.Empty;

        [Display(Name = "PNRR")]
        public bool PNRR { get; set; } = false;

        [StringLength(2000, ErrorMessage = "La descrizione non può superare i 2000 caratteri")]
        [Display(Name = "Descrizione")]
        [DataType(DataType.MultilineText)]
        public string? Descrizione { get; set; }

        [Required(ErrorMessage = "La tipologia è obbligatoria")]
        [Display(Name = "Tipologia")]
        public TipologiaGara Tipologia { get; set; }

        [Required(ErrorMessage = "Lo stato è obbligatorio")]
        [Display(Name = "Stato")]
        public StatoGara Stato { get; set; }

        // ===== INFO AMMINISTRAZIONE =====

        [StringLength(200, ErrorMessage = "Il nome dell'ente appaltante non può superare i 200 caratteri")]
        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        [StringLength(100, ErrorMessage = "La regione non può superare i 100 caratteri")]
        [Display(Name = "Regione")]
        public string? Regione { get; set; }

        [StringLength(200, ErrorMessage = "Il nome del punto ordinante non può superare i 200 caratteri")]
        [Display(Name = "Nome Punto Ordinante")]
        public string? NomePuntoOrdinante { get; set; }

        [StringLength(50, ErrorMessage = "Il telefono non può superare i 50 caratteri")]
        [Display(Name = "Telefono Punto Ordinante")]
        [Phone(ErrorMessage = "Inserire un numero di telefono valido")]
        public string? TelefonoPuntoOrdinante { get; set; }

        // ===== CODICI GARA =====

        [StringLength(20, ErrorMessage = "Il CIG non può superare i 20 caratteri")]
        [Display(Name = "CIG")]
        public string? CIG { get; set; }

        [StringLength(20, ErrorMessage = "Il CUP non può superare i 20 caratteri")]
        [Display(Name = "CUP")]
        public string? CUP { get; set; }

        [StringLength(50, ErrorMessage = "L'RDO non può superare i 50 caratteri")]
        [Display(Name = "RDO")]
        public string? RDO { get; set; }

        [StringLength(100, ErrorMessage = "Il bando non può superare i 100 caratteri")]
        [Display(Name = "Bando")]
        public string? Bando { get; set; }

        [StringLength(500, ErrorMessage = "La denominazione iniziativa non può superare i 500 caratteri")]
        [Display(Name = "Denominazione Iniziativa")]
        public string? DenominazioneIniziativa { get; set; }

        [StringLength(200, ErrorMessage = "La procedura non può superare i 200 caratteri")]
        [Display(Name = "Procedura")]
        public string? Procedura { get; set; }

        [StringLength(200, ErrorMessage = "Il criterio di aggiudicazione non può superare i 200 caratteri")]
        [Display(Name = "Criterio Aggiudicazione")]
        public string? CriterioAggiudicazione { get; set; }

        // ===== DATE CRITICHE =====

        [Display(Name = "Data Pubblicazione")]
        [DataType(DataType.Date)]
        public DateTime? DataPubblicazione { get; set; }

        [Display(Name = "Data Inizio Presentazione Offerte")]
        [DataType(DataType.DateTime)]
        public DateTime? DataInizioPresentazioneOfferte { get; set; }

        [Display(Name = "Data Termine Richiesta Chiarimenti")]
        [DataType(DataType.DateTime)]
        public DateTime? DataTermineRichiestaChiarimenti { get; set; }

        [Display(Name = "Data Termine Presentazione Offerte")]
        [DataType(DataType.DateTime)]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== INFO ECONOMICHE =====

        [Display(Name = "Importo Totale Stimato")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "L'importo deve essere un valore positivo")]
        public decimal? ImportoTotaleStimato { get; set; }

        // ===== LINK =====

        [StringLength(500, ErrorMessage = "Il link non può superare i 500 caratteri")]
        [Display(Name = "Link Piattaforma")]
        [Url(ErrorMessage = "Inserire un URL valido")]
        public string? LinkPiattaforma { get; set; }

        // ===== CHIUSURA MANUALE =====

        [Display(Name = "Chiusa Manualmente")]
        public bool IsChiusaManualmente { get; set; }

        [Display(Name = "Data Chiusura Manuale")]
        [DataType(DataType.Date)]
        public DateTime? DataChiusuraManuale { get; set; }

        [StringLength(1000, ErrorMessage = "Il motivo di chiusura non può superare i 1000 caratteri")]
        [Display(Name = "Motivo Chiusura Manuale")]
        [DataType(DataType.MultilineText)]
        public string? MotivoChiusuraManuale { get; set; }

        // ===== INFORMAZIONI DI SISTEMA (READ ONLY) =====

        [Display(Name = "Numero Lotti")]
        public int NumeroLotti { get; set; }

        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Creata Da")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Modificata Da")]
        public string? ModifiedBy { get; set; }

        // ===== CHECKLIST DOCUMENTI RICHIESTI =====

        [Display(Name = "Documenti Richiesti")]
        public List<Guid> DocumentiRichiestiIds { get; set; } = new();

        // ===== VALIDAZIONI CUSTOM =====

        /// <summary>
        /// Validazione custom: DataTerminePresentazioneOfferte deve essere dopo DataInizioPresentazioneOfferte
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validazione date presentazione offerte
            if (DataInizioPresentazioneOfferte.HasValue &&
                DataTerminePresentazioneOfferte.HasValue &&
                DataTerminePresentazioneOfferte.Value <= DataInizioPresentazioneOfferte.Value)
            {
                yield return new ValidationResult(
                    "La data termine presentazione offerte deve essere successiva alla data inizio",
                    new[] { nameof(DataTerminePresentazioneOfferte) });
            }

            // Validazione data termine chiarimenti
            if (DataTermineRichiestaChiarimenti.HasValue &&
                DataTerminePresentazioneOfferte.HasValue &&
                DataTermineRichiestaChiarimenti.Value >= DataTerminePresentazioneOfferte.Value)
            {
                yield return new ValidationResult(
                    "La data termine richiesta chiarimenti deve essere precedente alla scadenza offerte",
                    new[] { nameof(DataTermineRichiestaChiarimenti) });
            }

            // Validazione data pubblicazione
            if (DataPubblicazione.HasValue &&
                DataInizioPresentazioneOfferte.HasValue &&
                DataPubblicazione.Value > DataInizioPresentazioneOfferte.Value)
            {
                yield return new ValidationResult(
                    "La data di pubblicazione deve essere precedente all'inizio della presentazione offerte",
                    new[] { nameof(DataPubblicazione) });
            }

            // Validazione chiusura manuale
            if (IsChiusaManualmente && !DataChiusuraManuale.HasValue)
            {
                yield return new ValidationResult(
                    "Se la gara è chiusa manualmente, è necessario specificare la data di chiusura",
                    new[] { nameof(DataChiusuraManuale) });
            }

            // Validazione motivo chiusura
            if (IsChiusaManualmente && string.IsNullOrWhiteSpace(MotivoChiusuraManuale))
            {
                yield return new ValidationResult(
                    "Se la gara è chiusa manualmente, è necessario specificare il motivo",
                    new[] { nameof(MotivoChiusuraManuale) });
            }
        }
    }
}