using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la modifica di un registro contratti
    /// </summary>
    public class RegistroContrattiEditViewModel
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        [Required]
        public Guid Id { get; set; }

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
        public DateTime DataDocumento { get; set; }

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

        /// <summary>
        /// Data di invio al cliente
        /// </summary>
        [Display(Name = "Data Invio")]
        [DataType(DataType.Date)]
        public DateTime? DataInvio { get; set; }

        /// <summary>
        /// Data di accettazione da parte del cliente
        /// </summary>
        [Display(Name = "Data Accettazione")]
        [DataType(DataType.Date)]
        public DateTime? DataAccettazione { get; set; }

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
        public int GiorniAlertScadenza { get; set; }

        /// <summary>
        /// Indica se il contratto si rinnova automaticamente
        /// </summary>
        [Display(Name = "Rinnovo Automatico")]
        public bool IsRinnovoAutomatico { get; set; }

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
        // STATO
        // ===================================

        /// <summary>
        /// Stato corrente del documento
        /// </summary>
        [Display(Name = "Stato")]
        [Required(ErrorMessage = "Lo stato è obbligatorio")]
        public StatoRegistro Stato { get; set; }

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
        /// Lista stati per dropdown
        /// </summary>
        public SelectList? StatiSelectList { get; set; }

        // ===================================
        // INFO AGGIUNTIVE (READONLY)
        // ===================================

        /// <summary>
        /// Numero protocollo del documento padre (per display)
        /// </summary>
        public string? ParentNumeroProtocollo { get; set; }

        /// <summary>
        /// Oggetto del documento padre (per display)
        /// </summary>
        public string? ParentOggetto { get; set; }

        /// <summary>
        /// Indica se ha documenti figli (per warning)
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// Numero di allegati
        /// </summary>
        public int NumeroAllegati { get; set; }

        // ===================================
        // PROPRIETÀ CALCOLATE
        // ===================================

        /// <summary>
        /// Indica se lo stato può essere modificato
        /// </summary>
        public bool CanChangeStato => Stato != StatoRegistro.Annullato && Stato != StatoRegistro.Rinnovato;

        /// <summary>
        /// Indica se il tipo può essere modificato
        /// </summary>
        public bool CanChangeTipo => Stato == StatoRegistro.Bozza || Stato == StatoRegistro.InRevisione;

        /// <summary>
        /// Lista degli stati selezionabili in base allo stato corrente
        /// </summary>
        public IEnumerable<StatoRegistro> StatiDisponibili
        {
            get
            {
                return Stato switch
                {
                    StatoRegistro.Bozza => new[]
                    {
                        StatoRegistro.Bozza,
                        StatoRegistro.InRevisione,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.InRevisione => new[]
                    {
                        StatoRegistro.InRevisione,
                        StatoRegistro.Bozza,
                        StatoRegistro.Inviato,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.Inviato => new[]
                    {
                        StatoRegistro.Inviato,
                        StatoRegistro.InRevisione,
                        StatoRegistro.Attivo,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.Attivo => new[]
                    {
                        StatoRegistro.Attivo,
                        StatoRegistro.InScadenza,
                        StatoRegistro.Sospeso,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.InScadenza => new[]
                    {
                        StatoRegistro.InScadenza,
                        StatoRegistro.InScadenzaPropostoRinnovo,
                        StatoRegistro.Attivo,
                        StatoRegistro.Scaduto,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.InScadenzaPropostoRinnovo => new[]
                    {
                        StatoRegistro.InScadenzaPropostoRinnovo,
                        StatoRegistro.InScadenza,
                        StatoRegistro.Rinnovato,
                        StatoRegistro.Scaduto,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.Scaduto => new[]
                    {
                        StatoRegistro.Scaduto,
                        StatoRegistro.Rinnovato
                    },
                    StatoRegistro.Sospeso => new[]
                    {
                        StatoRegistro.Sospeso,
                        StatoRegistro.Attivo,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.Rinnovato => new[]
                    {
                        StatoRegistro.Rinnovato
                    },
                    StatoRegistro.Annullato => new[]
                    {
                        StatoRegistro.Annullato
                    },
                    _ => new[] { Stato }
                };
            }
        }

        // ===================================
        // CONTENUTO AGGIUNTIVO
        // ===================================

        ///// <summary>
        ///// Descrizione dettagliata del contratto/preventivo
        ///// </summary>
        //[Display(Name = "Descrizione")]
        //[StringLength(2000, ErrorMessage = "La descrizione non può superare i 2000 caratteri")]
        //public string? Descrizione { get; set; }

        ///// <summary>
        ///// Note interne
        ///// </summary>
        //[Display(Name = "Note")]
        //[StringLength(2000, ErrorMessage = "Le note non possono superare i 2000 caratteri")]
        //public string? Note { get; set; }

        // ===================================
        // IMPORTI AGGIUNTIVI
        // ===================================

        /// <summary>
        /// Importo totale del contratto
        /// </summary>
        [Display(Name = "Importo Totale")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "L'importo non può essere negativo")]
        public decimal ImportoTotale => (ImportoCanoneAnnuo ?? 0) + (ImportoUnatantum ?? 0);

        // ===================================
        // PROPRIETÀ CALCOLATE PER DISPLAY
        // ===================================

        /// <summary>
        /// Descrizione del tipo registro
        /// </summary>
        public string TipoRegistroDescrizione => TipoRegistro == TipoRegistro.Preventivo ? "Preventivo" : "Contratto";

        /// <summary>
        /// Classe CSS badge per il tipo
        /// </summary>
        public string TipoBadgeClass => TipoRegistro == TipoRegistro.Preventivo ? "bg-info" : "bg-primary";

        /// <summary>
        /// Descrizione dello stato
        /// </summary>
        public string StatoDescrizione => Stato switch
        {
            StatoRegistro.Bozza => "Bozza",
            StatoRegistro.InRevisione => "In Revisione",
            StatoRegistro.Inviato => "Inviato",
            StatoRegistro.Attivo => "Attivo",
            StatoRegistro.InScadenza => "In Scadenza",
            StatoRegistro.InScadenzaPropostoRinnovo => "Proposto Rinnovo",
            StatoRegistro.Scaduto => "Scaduto",
            StatoRegistro.Sospeso => "Sospeso",
            StatoRegistro.Rinnovato => "Rinnovato",
            StatoRegistro.Annullato => "Annullato",
            _ => Stato.ToString()
        };

        /// <summary>
        /// Classe CSS badge per lo stato
        /// </summary>
        public string StatoBadgeClass => Stato switch
        {
            StatoRegistro.Bozza => "bg-secondary",
            StatoRegistro.InRevisione => "bg-info",
            StatoRegistro.Inviato => "bg-primary",
            StatoRegistro.Attivo => "bg-success",
            StatoRegistro.InScadenza => "bg-warning text-dark",
            StatoRegistro.InScadenzaPropostoRinnovo => "bg-warning text-dark",
            StatoRegistro.Scaduto => "bg-danger",
            StatoRegistro.Sospeso => "bg-dark",
            StatoRegistro.Rinnovato => "bg-info",
            StatoRegistro.Annullato => "bg-secondary",
            _ => "bg-secondary"
        };

        /// <summary>
        /// Icona Bootstrap per lo stato
        /// </summary>
        public string StatoIcon => Stato switch
        {
            StatoRegistro.Bozza => "bi bi-pencil",
            StatoRegistro.InRevisione => "bi bi-arrow-repeat",
            StatoRegistro.Inviato => "bi bi-send",
            StatoRegistro.Attivo => "bi bi-check-circle",
            StatoRegistro.InScadenza => "bi bi-clock",
            StatoRegistro.InScadenzaPropostoRinnovo => "bi bi-arrow-clockwise",
            StatoRegistro.Scaduto => "bi bi-x-circle",
            StatoRegistro.Sospeso => "bi bi-pause-circle",
            StatoRegistro.Rinnovato => "bi bi-arrow-repeat",
            StatoRegistro.Annullato => "bi bi-slash-circle",
            _ => "bi bi-question-circle"
        };
    }
}