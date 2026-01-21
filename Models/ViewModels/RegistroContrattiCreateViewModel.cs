using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la creazione di un nuovo registro contratti
    /// </summary>
    public class RegistroContrattiCreateViewModel
    {
        // ===================================
        // IDENTIFICAZIONE
        // ===================================

        /// <summary>
        /// Codice di riferimento a programma esterno
        /// </summary>
        [Display(Name = "Riferimento Esterno")]
        [StringLength(50, ErrorMessage = "Il riferimento esterno non può superare i 50 caratteri")]
        public string? IdRiferimentoEsterno { get; set; }

        /// <summary>
        /// Numero protocollo interno aziendale
        /// </summary>
        [Display(Name = "Numero Protocollo")]
        [StringLength(50, ErrorMessage = "Il numero protocollo non può superare i 50 caratteri")]
        public string? NumeroProtocollo { get; set; }

        /// <summary>
        /// Tipo di documento (Preventivo o Contratto)
        /// </summary>
        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Il tipo è obbligatorio")]
        public TipoRegistro TipoRegistro { get; set; }

        // ===================================
        // CLIENTE
        // ===================================

        /// <summary>
        /// FK al cliente (Soggetto con IsCliente = true)
        /// </summary>
        [Display(Name = "Cliente")]
        public Guid? ClienteId { get; set; }

        /// <summary>
        /// Ragione sociale - se non selezionato cliente da anagrafica
        /// </summary>
        [Display(Name = "Ragione Sociale")]
        [StringLength(200, ErrorMessage = "La ragione sociale non può superare i 200 caratteri")]
        public string? RagioneSociale { get; set; }

        /// <summary>
        /// Natura giuridica controparte
        /// </summary>
        [Display(Name = "Tipo Controparte")]
        public NaturaGiuridica? TipoControparte { get; set; }

        // ===================================
        // CONTENUTO
        // ===================================

        /// <summary>
        /// Oggetto del contratto/preventivo
        /// </summary>
        [Display(Name = "Oggetto")]
        [Required(ErrorMessage = "L'oggetto è obbligatorio")]
        [StringLength(500, ErrorMessage = "L'oggetto non può superare i 500 caratteri")]
        public string Oggetto { get; set; } = string.Empty;

        /// <summary>
        /// FK alla categoria contratto
        /// </summary>
        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "La categoria è obbligatoria")]
        public Guid CategoriaContrattoId { get; set; }

        // ===================================
        // RESPONSABILE INTERNO
        // ===================================

        /// <summary>
        /// FK all'utente responsabile interno
        /// </summary>
        [Display(Name = "Responsabile")]
        public string? UtenteId { get; set; }

        // ===================================
        // DATE
        // ===================================

        /// <summary>
        /// Data del documento
        /// </summary>
        [Display(Name = "Data Documento")]
        [Required(ErrorMessage = "La data documento è obbligatoria")]
        [DataType(DataType.Date)]
        public DateTime DataDocumento { get; set; } = DateTime.Now.Date;

        /// <summary>
        /// Data di decorrenza/inizio validità
        /// </summary>
        [Display(Name = "Data Decorrenza")]
        [DataType(DataType.Date)]
        public DateTime? DataDecorrenza { get; set; }

        /// <summary>
        /// Data di fine o scadenza
        /// </summary>
        [Display(Name = "Data Fine/Scadenza")]
        [DataType(DataType.Date)]
        public DateTime? DataFineOScadenza { get; set; }

        // ===================================
        // SCADENZE E RINNOVI
        // ===================================

        /// <summary>
        /// Giorni di preavviso per disdetta
        /// </summary>
        [Display(Name = "Giorni Preavviso Disdetta")]
        [Range(0, 365, ErrorMessage = "I giorni devono essere compresi tra 0 e 365")]
        public int? GiorniPreavvisoDisdetta { get; set; }

        /// <summary>
        /// Giorni di anticipo per alert scadenza
        /// </summary>
        [Display(Name = "Giorni Alert Scadenza")]
        [Range(1, 365, ErrorMessage = "I giorni devono essere compresi tra 1 e 365")]
        public int GiorniAlertScadenza { get; set; } = 60;

        /// <summary>
        /// Indica se il contratto si rinnova automaticamente
        /// </summary>
        [Display(Name = "Rinnovo Automatico")]
        public bool IsRinnovoAutomatico { get; set; } = false;

        /// <summary>
        /// Durata del rinnovo automatico in giorni
        /// </summary>
        [Display(Name = "Giorni Rinnovo Automatico")]
        [Range(1, 3650, ErrorMessage = "I giorni devono essere compresi tra 1 e 3650")]
        public int? GiorniRinnovoAutomatico { get; set; }

        // ===================================
        // IMPORTI
        // ===================================

        /// <summary>
        /// Importo del canone annuo
        /// </summary>
        [Display(Name = "Importo Canone Annuo")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "L'importo non può essere negativo")]
        public decimal? ImportoCanoneAnnuo { get; set; }

        /// <summary>
        /// Importo una tantum (setup, attivazione, ecc.)
        /// </summary>
        [Display(Name = "Importo Una Tantum")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "L'importo non può essere negativo")]
        public decimal? ImportoUnatantum { get; set; }

        // ===================================
        // GERARCHIA / VERSIONAMENTO
        // ===================================

        /// <summary>
        /// FK al record padre (per versionamento e alberatura)
        /// </summary>
        [Display(Name = "Documento Padre")]
        public Guid? ParentId { get; set; }

        // ===================================
        // SELECT LIST PER DROPDOWN
        // ===================================

        /// <summary>
        /// Lista clienti per dropdown
        /// </summary>
        public SelectList? ClientiSelectList { get; set; }

        /// <summary>
        /// Lista categorie per dropdown
        /// </summary>
        public SelectList? CategorieSelectList { get; set; }

        /// <summary>
        /// Lista utenti per dropdown
        /// </summary>
        public SelectList? UtentiSelectList { get; set; }

        /// <summary>
        /// Lista tipi registro per dropdown
        /// </summary>
        public SelectList? TipiRegistroSelectList { get; set; }

        /// <summary>
        /// Lista nature giuridiche per dropdown
        /// </summary>
        public SelectList? NatureGiuridicheSelectList { get; set; }

        /// <summary>
        /// Lista documenti disponibili come parent (per dropdown)
        /// </summary>
        public SelectList? ParentSelectList { get; set; }

        // ===================================
        // INFO DOCUMENTO PADRE (READONLY)
        // ===================================

        /// <summary>
        /// Numero protocollo del documento padre (per display)
        /// </summary>
        public string? ParentNumeroProtocollo { get; set; }

        /// <summary>
        /// Oggetto del documento padre (per display)
        /// </summary>
        public string? ParentOggetto { get; set; }
    }
}