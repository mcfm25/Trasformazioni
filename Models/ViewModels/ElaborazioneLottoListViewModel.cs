using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione in lista delle Elaborazioni Lotto
    /// Contiene proprietà essenziali per liste paginate con scostamento calcolato
    /// </summary>
    public class ElaborazioneLottoListViewModel
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }

        // ===== INFO LOTTO =====
        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Display(Name = "Descrizione Lotto")]
        public string DescrizioneLotto { get; set; } = string.Empty;

        // ===== INFO GARA =====
        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

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
        [Display(Name = "Scostamento %")]
        [DisplayFormat(DataFormatString = "{0:F2}%", NullDisplayText = "N/A")]
        public decimal? ScostamentoPercentuale { get; set; }

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

        // ===== STATO CALCOLATO =====
        /// <summary>
        /// Stato dell'elaborazione calcolato in base ai prezzi valorizzati
        /// Valori possibili: "Da iniziare", "Prezzo desiderato definito", "Solo prezzo reale", "Completata"
        /// </summary>
        [Display(Name = "Stato")]
        public string StatoElaborazione { get; set; } = string.Empty;

        // ===== MOTIVAZIONE =====
        /// <summary>
        /// Indica se è presente una motivazione di adattamento
        /// </summary>
        [Display(Name = "Ha Motivazione")]
        public bool HasMotivazione { get; set; }

        // ===== AUDIT =====
        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", NullDisplayText = "Mai modificata")]
        public DateTime? ModifiedAt { get; set; }
    }
}