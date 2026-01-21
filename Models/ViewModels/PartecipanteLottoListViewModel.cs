using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione in lista dei Partecipanti Lotto
    /// Contiene proprietà essenziali per liste paginate con badge stato
    /// </summary>
    public class PartecipanteLottoListViewModel
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }
        public Guid? SoggettoId { get; set; }

        // ===== DATI PARTECIPANTE =====
        [Display(Name = "Ragione Sociale")]
        public string RagioneSociale { get; set; } = string.Empty;

        [Display(Name = "Offerta Economica")]
        [DisplayFormat(DataFormatString = "{0:C2}", NullDisplayText = "Non specificata")]
        public decimal? OffertaEconomica { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        // ===== FLAGS STATO =====
        [Display(Name = "Aggiudicatario")]
        public bool IsAggiudicatario { get; set; }

        [Display(Name = "Scartato dall'Ente")]
        public bool IsScartatoDallEnte { get; set; }

        /// <summary>
        /// Indica se il partecipante è collegato a un Soggetto esistente
        /// </summary>
        [Display(Name = "Soggetto Censito")]
        public bool HasSoggetto { get; set; }

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

        // ===== INFO SOGGETTO (se collegato) =====
        /// <summary>
        /// Nome completo o denominazione del Soggetto collegato
        /// </summary>
        [Display(Name = "Soggetto")]
        public string? NomeSoggetto { get; set; }

        // ===== STATO CALCOLATO =====
        /// <summary>
        /// Stato del partecipante calcolato in base ai flag
        /// Valori possibili: "Aggiudicatario", "Scartato", "Partecipante"
        /// </summary>
        [Display(Name = "Stato")]
        public string StatoPartecipante { get; set; } = string.Empty;

        // ===== AUDIT =====
        [Display(Name = "Data Censimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", NullDisplayText = "Mai modificato")]
        public DateTime? ModifiedAt { get; set; }
    }
}