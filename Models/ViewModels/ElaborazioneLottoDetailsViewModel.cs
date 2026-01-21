using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione dettagliata di un'Elaborazione Lotto
    /// Include tutte le proprietà, relazioni con Lotto/Gara e calcoli derivati
    /// </summary>
    public class ElaborazioneLottoDetailsViewModel
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }

        // ===== PREZZI =====
        [Display(Name = "Prezzo Desiderato")]
        [DisplayFormat(DataFormatString = "{0:C2}", NullDisplayText = "Non definito")]
        public decimal? PrezzoDesiderato { get; set; }

        [Display(Name = "Prezzo Reale Uscita")]
        [DisplayFormat(DataFormatString = "{0:C2}", NullDisplayText = "Non definito")]
        public decimal? PrezzoRealeUscita { get; set; }

        // ===== SCOSTAMENTO CALCOLATO =====
        /// <summary>
        /// Scostamento percentuale tra PrezzoDesiderato e PrezzoRealeUscita
        /// Calcolato come: |PrezzoReale - PrezzoDesiderato| / PrezzoDesiderato * 100
        /// </summary>
        [Display(Name = "Scostamento Percentuale")]
        [DisplayFormat(DataFormatString = "{0:F2}%", NullDisplayText = "N/A")]
        public decimal? ScostamentoPercentuale { get; set; }

        /// <summary>
        /// Differenza assoluta tra i due prezzi (PrezzoReale - PrezzoDesiderato)
        /// </summary>
        [Display(Name = "Differenza Assoluta")]
        [DisplayFormat(DataFormatString = "{0:C2}", NullDisplayText = "N/A")]
        public decimal? DifferenzaAssoluta { get; set; }

        /// <summary>
        /// Indica se il prezzo reale è superiore al desiderato
        /// </summary>
        [Display(Name = "Prezzo Reale Superiore")]
        public bool IsPrezzoRealeSuperiore { get; set; }

        /// <summary>
        /// Indica se il prezzo reale è inferiore al desiderato
        /// </summary>
        [Display(Name = "Prezzo Reale Inferiore")]
        public bool IsPrezzoRealeInferiore { get; set; }

        // ===== MOTIVAZIONE E NOTE =====
        [Display(Name = "Motivazione Adattamento")]
        public string? MotivazioneAdattamento { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        // ===== STATO CALCOLATO =====
        /// <summary>
        /// Stato dell'elaborazione calcolato in base ai prezzi valorizzati
        /// Valori possibili: "Da iniziare", "Prezzo desiderato definito", "Solo prezzo reale", "Completata"
        /// </summary>
        [Display(Name = "Stato Elaborazione")]
        public string StatoElaborazione { get; set; } = string.Empty;

        // ===== INFO LOTTO =====
        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Display(Name = "Descrizione Lotto")]
        public string DescrizioneLotto { get; set; } = string.Empty;

        [Display(Name = "Tipologia Lotto")]
        public TipologiaLotto? TipologiaLotto { get; set; }

        [Display(Name = "Stato Lotto")]
        public StatoLotto? StatoLotto { get; set; }

        [Display(Name = "Importo Base d'Asta")]
        [DisplayFormat(DataFormatString = "{0:C2}", NullDisplayText = "Non definito")]
        public decimal? ImportoBaseAstaLotto { get; set; }

        [Display(Name = "Quotazione")]
        [DisplayFormat(DataFormatString = "{0:C2}", NullDisplayText = "Non definita")]
        public decimal? QuotazioneLotto { get; set; }

        [Display(Name = "Operatore Assegnato")]
        public string? OperatoreAssegnatoNome { get; set; }

        // ===== INFO GARA =====
        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        [Display(Name = "Titolo Gara")]
        public string TitoloGara { get; set; } = string.Empty;

        [Display(Name = "Tipologia Gara")]
        public TipologiaGara? TipologiaGara { get; set; }

        [Display(Name = "Stato Gara")]
        public StatoGara? StatoGara { get; set; }

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        [Display(Name = "Regione")]
        public string? Regione { get; set; }

        [Display(Name = "Data Termine Presentazione Offerte")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", NullDisplayText = "Non definita")]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== AUDIT - CREAZIONE =====
        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Creato Da")]
        public string CreatedBy { get; set; } = string.Empty;

        // ===== AUDIT - MODIFICA =====
        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", NullDisplayText = "Mai modificata")]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Modificato Da")]
        public string? ModifiedBy { get; set; }
    }
}