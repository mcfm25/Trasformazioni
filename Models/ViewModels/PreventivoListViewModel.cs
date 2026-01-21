using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare un preventivo nelle liste paginate
    /// Contiene solo le informazioni essenziali per la visualizzazione in elenco
    /// Utilizzato in combinazione con PreventivoFilterViewModel e PagedResult
    /// </summary>
    public class PreventivoListViewModel
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }
        public Guid SoggettoId { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [Display(Name = "Stato")]
        public StatoPreventivo Stato { get; set; }

        // ===== INFO LOTTO =====

        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Display(Name = "Descrizione Lotto")]
        public string DescrizioneLotto { get; set; } = string.Empty;

        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        // ===== INFO FORNITORE =====

        [Display(Name = "Fornitore")]
        public string NomeFornitore { get; set; } = string.Empty;

        [Display(Name = "Partita IVA")]
        public string? PartitaIVAFornitore { get; set; }

        // ===== INFO ECONOMICHE =====

        [Display(Name = "Importo Offerto")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoOfferto { get; set; }

        // ===== DATE =====

        [Display(Name = "Data Richiesta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DataRichiesta { get; set; }

        [Display(Name = "Data Ricezione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataRicezione { get; set; }

        [Display(Name = "Data Scadenza")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DataScadenza { get; set; }

        // ===== DOCUMENTO =====

        [Display(Name = "Nome File")]
        public string NomeFile { get; set; } = string.Empty;

        [Display(Name = "Percorso Documento")]
        public string DocumentPath { get; set; } = string.Empty;

        // ===== SELEZIONE =====

        [Display(Name = "Selezionato")]
        public bool IsSelezionato { get; set; }

        // ===== AUTO-RINNOVO =====

        [Display(Name = "Giorni Auto-Rinnovo")]
        public int? GiorniAutoRinnovo { get; set; }

        // ===== AUDIT =====

        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? ModifiedAt { get; set; }

        // ===== PROPRIETÀ COMPUTATE (per UI) =====

        /// <summary>
        /// Verifica se il preventivo è in scadenza (entro 7 giorni)
        /// </summary>
        public bool IsInScadenza => DataScadenza > DateTime.Now &&
                                    DataScadenza <= DateTime.Now.AddDays(7);

        /// <summary>
        /// Verifica se il preventivo è scaduto
        /// </summary>
        public bool IsScaduto => DataScadenza < DateTime.Now;

        /// <summary>
        /// Verifica se il preventivo è stato ricevuto
        /// </summary>
        public bool IsRicevuto => DataRicezione.HasValue;

        /// <summary>
        /// Verifica se ha auto-rinnovo attivo
        /// </summary>
        public bool HasAutoRinnovo => GiorniAutoRinnovo.HasValue && GiorniAutoRinnovo.Value > 0;

        /// <summary>
        /// Indica se il preventivo è valido e utilizzabile
        /// </summary>
        public bool IsValido => Stato == StatoPreventivo.Valido || Stato == StatoPreventivo.Ricevuto;

        /// <summary>
        /// Indica se ha un documento allegato
        /// </summary>
        public bool HasDocumento => !string.IsNullOrEmpty(DocumentPath);

        /// <summary>
        /// Badge CSS class basato sullo stato
        /// </summary>
        public string StatoBadgeClass => Stato switch
        {
            StatoPreventivo.InAttesa => "badge bg-secondary",
            StatoPreventivo.Ricevuto => "badge bg-info",
            StatoPreventivo.Valido => "badge bg-success",
            StatoPreventivo.InScadenza => "badge bg-warning text-dark",
            StatoPreventivo.Scaduto => "badge bg-danger",
            StatoPreventivo.Rinnovato => "badge bg-primary",
            StatoPreventivo.Annullato => "badge bg-dark",
            _ => "badge bg-light text-dark"
        };

        /// <summary>
        /// Icona FontAwesome basata sullo stato
        /// </summary>
        public string StatoIcon => Stato switch
        {
            StatoPreventivo.InAttesa => "fa-solid fa-clock",
            StatoPreventivo.Ricevuto => "fa-solid fa-envelope-open",
            StatoPreventivo.Valido => "fa-solid fa-check-circle",
            StatoPreventivo.InScadenza => "fa-solid fa-exclamation-triangle",
            StatoPreventivo.Scaduto => "fa-solid fa-times-circle",
            StatoPreventivo.Rinnovato => "fa-solid fa-rotate",
            StatoPreventivo.Annullato => "fa-solid fa-ban",
            _ => "fa-solid fa-question"
        };

        /// <summary>
        /// Giorni rimanenti alla scadenza (null se scaduto)
        /// </summary>
        public int? GiorniRimanenti
        {
            get
            {
                if (IsScaduto)
                    return null;

                var diff = (DataScadenza - DateTime.Now).TotalDays;
                return diff > 0 ? (int)Math.Ceiling(diff) : 0;
            }
        }

        /// <summary>
        /// Giorni trascorsi dalla richiesta
        /// </summary>
        public int GiorniDallaRichiesta
        {
            get
            {
                var diff = (DateTime.Now - DataRichiesta).TotalDays;
                return (int)Math.Floor(diff);
            }
        }

        /// <summary>
        /// Testo descrittivo dello stato della scadenza
        /// </summary>
        public string ScadenzaText
        {
            get
            {
                if (IsScaduto)
                    return "Scaduto";

                if (IsInScadenza)
                {
                    var giorni = GiorniRimanenti ?? 0;
                    return giorni == 0 ? "Scade oggi" : $"Scade tra {giorni} giorni";
                }

                return "Valido";
            }
        }

        /// <summary>
        /// CSS class per la scadenza
        /// </summary>
        public string ScadenzaClass => IsScaduto ? "text-danger fw-bold" :
                                       IsInScadenza ? "text-warning fw-bold" :
                                       "text-success";

        /// <summary>
        /// Badge per il preventivo selezionato
        /// </summary>
        public string SelezionatoBadge => IsSelezionato ? "badge bg-success" : "";

        /// <summary>
        /// Icona per il preventivo selezionato
        /// </summary>
        public string SelezionatoIcon => IsSelezionato ? "fa-solid fa-star" : "fa-regular fa-star";

        /// <summary>
        /// Tooltip per il preventivo selezionato
        /// </summary>
        public string SelezionatoTooltip => IsSelezionato ? "Preventivo selezionato" : "Non selezionato";

        /// <summary>
        /// Badge per auto-rinnovo
        /// </summary>
        public string AutoRinnovoBadge => HasAutoRinnovo ? "badge bg-primary" : "";

        /// <summary>
        /// Icona per auto-rinnovo
        /// </summary>
        public string AutoRinnovoIcon => HasAutoRinnovo ? "fa-solid fa-rotate" : "";

        /// <summary>
        /// Tooltip per auto-rinnovo
        /// </summary>
        public string AutoRinnovoTooltip => HasAutoRinnovo
            ? $"Auto-rinnovo ogni {GiorniAutoRinnovo} giorni"
            : "Nessun auto-rinnovo";

        /// <summary>
        /// Icona per il documento
        /// </summary>
        public string DocumentoIcon => HasDocumento ? "fa-solid fa-file-pdf text-danger" : "fa-solid fa-file-circle-xmark text-muted";

        /// <summary>
        /// Tooltip per il documento
        /// </summary>
        public string DocumentoTooltip => HasDocumento ? $"File: {NomeFile}" : "Nessun documento allegato";
    }
}