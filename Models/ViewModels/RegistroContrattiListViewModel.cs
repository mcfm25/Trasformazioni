using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione in lista dei registri contratti
    /// </summary>
    public class RegistroContrattiListViewModel
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        // ===== IDENTIFICAZIONE =====

        /// <summary>
        /// Riferimento esterno
        /// </summary>
        public string? IdRiferimentoEsterno { get; set; }

        /// <summary>
        /// Numero protocollo interno
        /// </summary>
        public string? NumeroProtocollo { get; set; }

        /// <summary>
        /// Tipo documento (Preventivo o Contratto)
        /// </summary>
        public TipoRegistro TipoRegistro { get; set; }

        /// <summary>
        /// Descrizione del tipo per display
        /// </summary>
        public string TipoRegistroDescrizione => TipoRegistro == TipoRegistro.Preventivo
            ? "Preventivo"
            : "Contratto";

        // ===== CLIENTE =====

        /// <summary>
        /// ID del cliente
        /// </summary>
        public Guid? ClienteId { get; set; }

        /// <summary>
        /// Ragione sociale del cliente
        /// </summary>
        public string? RagioneSociale { get; set; }

        // ===== CONTENUTO =====

        /// <summary>
        /// Oggetto del contratto/preventivo
        /// </summary>
        public string Oggetto { get; set; } = string.Empty;

        /// <summary>
        /// Nome della categoria
        /// </summary>
        public string CategoriaNome { get; set; } = string.Empty;

        // ===== RESPONSABILE =====

        /// <summary>
        /// Nome del responsabile interno
        /// </summary>
        public string? ResponsabileInterno { get; set; }

        // ===== DATE =====

        /// <summary>
        /// Data del documento
        /// </summary>
        public DateTime DataDocumento { get; set; }

        /// <summary>
        /// Data di decorrenza
        /// </summary>
        public DateTime? DataDecorrenza { get; set; }

        /// <summary>
        /// Data di fine o scadenza
        /// </summary>
        public DateTime? DataFineOScadenza { get; set; }

        // ===== IMPORTI =====

        /// <summary>
        /// Importo canone annuo
        /// </summary>
        public decimal? ImportoCanoneAnnuo { get; set; }

        /// <summary>
        /// Importo una tantum
        /// </summary>
        public decimal? ImportoUnatantum { get; set; }

        /// <summary>
        /// Importo totale
        /// </summary>
        public decimal ImportoTotale => (ImportoCanoneAnnuo ?? 0) + (ImportoUnatantum ?? 0);

        // ===== STATO =====

        /// <summary>
        /// Stato corrente
        /// </summary>
        public StatoRegistro Stato { get; set; }

        // ===== GERARCHIA =====

        /// <summary>
        /// Indica se ha un documento padre
        /// </summary>
        public bool HasParent { get; set; }

        /// <summary>
        /// Indica se ha documenti figli
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// Numero di allegati
        /// </summary>
        public int NumeroAllegati { get; set; }

        // ===== PROPRIETÀ CALCOLATE =====

        /// <summary>
        /// Data limite disdetta calcolata
        /// </summary>
        public DateTime? DataLimiteDisdetta { get; set; }

        /// <summary>
        /// Giorni rimanenti alla scadenza
        /// </summary>
        public int? GiorniAllaScadenza
        {
            get
            {
                if (!DataFineOScadenza.HasValue)
                    return null;

                var diff = (DataFineOScadenza.Value.Date - DateTime.Now.Date).Days;
                return diff;
            }
        }

        /// <summary>
        /// Indica se è in scadenza imminente (entro 30 giorni)
        /// </summary>
        public bool IsScadenzaImminente => GiorniAllaScadenza.HasValue &&
                                            GiorniAllaScadenza.Value >= 0 &&
                                            GiorniAllaScadenza.Value <= 30;

        /// <summary>
        /// Indica se è scaduto
        /// </summary>
        public bool IsScaduto => GiorniAllaScadenza.HasValue && GiorniAllaScadenza.Value < 0;

        /// <summary>
        /// Badge CSS per il tipo
        /// </summary>
        public string TipoBadgeClass => TipoRegistro == TipoRegistro.Preventivo
            ? "badge bg-info"
            : "badge bg-primary";

        /// <summary>
        /// Badge CSS per lo stato
        /// </summary>
        public string StatoBadgeClass => Stato switch
        {
            StatoRegistro.Bozza => "badge bg-secondary",
            StatoRegistro.InRevisione => "badge bg-warning text-dark",
            StatoRegistro.Inviato => "badge bg-info",
            StatoRegistro.Attivo => "badge bg-success",
            StatoRegistro.InScadenza => "badge bg-warning text-dark",
            StatoRegistro.InScadenzaPropostoRinnovo => "badge bg-warning text-dark",
            StatoRegistro.Scaduto => "badge bg-danger",
            StatoRegistro.Rinnovato => "badge bg-primary",
            StatoRegistro.Annullato => "badge bg-dark",
            StatoRegistro.Sospeso => "badge bg-secondary",
            _ => "badge bg-light text-dark"
        };

        /// <summary>
        /// Icona FontAwesome per lo stato
        /// </summary>
        public string StatoIcon => Stato switch
        {
            StatoRegistro.Bozza => "fa-solid fa-file-pen",
            StatoRegistro.InRevisione => "fa-solid fa-magnifying-glass",
            StatoRegistro.Inviato => "fa-solid fa-paper-plane",
            StatoRegistro.Attivo => "fa-solid fa-check-circle",
            StatoRegistro.InScadenza => "fa-solid fa-clock",
            StatoRegistro.InScadenzaPropostoRinnovo => "fa-solid fa-rotate",
            StatoRegistro.Scaduto => "fa-solid fa-calendar-xmark",
            StatoRegistro.Rinnovato => "fa-solid fa-arrows-rotate",
            StatoRegistro.Annullato => "fa-solid fa-ban",
            StatoRegistro.Sospeso => "fa-solid fa-pause",
            _ => "fa-solid fa-question"
        };

        /// <summary>
        /// Descrizione dello stato
        /// </summary>
        public string StatoDescrizione => Stato switch
        {
            StatoRegistro.Bozza => "In bozza",
            StatoRegistro.InRevisione => "In revisione",
            StatoRegistro.Inviato => "Inviato",
            StatoRegistro.Attivo => "Attivo",
            StatoRegistro.InScadenza => "In scadenza",
            StatoRegistro.InScadenzaPropostoRinnovo => "In scadenza - Proposto rinnovo",
            StatoRegistro.Scaduto => "Scaduto",
            StatoRegistro.Rinnovato => "Rinnovato",
            StatoRegistro.Annullato => "Annullato",
            StatoRegistro.Sospeso => "Sospeso",
            _ => "Sconosciuto"
        };
    }
}