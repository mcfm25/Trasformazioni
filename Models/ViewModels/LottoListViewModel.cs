using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare un lotto nelle liste paginate
    /// Contiene solo le informazioni essenziali per la visualizzazione in elenco
    /// Utilizzato in combinazione con LottoFilterViewModel e PagedResult
    /// </summary>
    public class LottoListViewModel
    {
        public Guid Id { get; set; }
        public Guid GaraId { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [Display(Name = "Tipologia")]
        public TipologiaLotto Tipologia { get; set; }

        [Display(Name = "Stato")]
        public StatoLotto Stato { get; set; }

        // ===== INFO GARA =====

        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        [Display(Name = "Titolo Gara")]
        public string TitoloGara { get; set; } = string.Empty;

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        // ===== INFO ECONOMICHE =====

        [Display(Name = "Importo Base Asta")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoBaseAsta { get; set; }

        [Display(Name = "Quotazione")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? Quotazione { get; set; }

        // ===== INFO OPERATORE =====

        [Display(Name = "Operatore Assegnato")]
        public string? OperatoreAssegnatoNome { get; set; }

        [Display(Name = "Operatore Assegnato")]
        public string? OperatoreAssegnatoId { get; set; }

        // ===== DATE CRITICHE =====

        [Display(Name = "Data Scadenza Offerte")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        [Display(Name = "Data Inizio Esame Ente")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataInizioEsameEnte { get; set; }

        // ===== STATISTICHE =====

        [Display(Name = "Numero Preventivi")]
        public int NumeroPreventivi { get; set; }

        [Display(Name = "Numero Richieste Integrazione")]
        public int NumeroRichiesteIntegrazione { get; set; }

        [Display(Name = "Richieste Aperte")]
        public int RichiesteIntegrazioneAperte { get; set; }

        [Display(Name = "Numero Partecipanti")]
        public int NumeroPartecipanti { get; set; }

        // ===== RIFIUTO =====

        [Display(Name = "Motivo Rifiuto")]
        public string? MotivoRifiuto { get; set; }

        // ===== AUDIT =====

        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? ModifiedAt { get; set; }

        // ===== PROPRIETÀ COMPUTATE (per UI) =====

        /// <summary>
        /// Verifica se la scadenza offerte è imminente (entro 7 giorni)
        /// </summary>
        public bool IsInScadenza => DataTerminePresentazioneOfferte.HasValue &&
                                    DataTerminePresentazioneOfferte.Value > DateTime.Now &&
                                    DataTerminePresentazioneOfferte.Value <= DateTime.Now.AddDays(7);

        /// <summary>
        /// Verifica se la scadenza offerte è passata
        /// </summary>
        public bool IsScaduta => DataTerminePresentazioneOfferte.HasValue &&
                                 DataTerminePresentazioneOfferte.Value < DateTime.Now;

        /// <summary>
        /// Indica se il lotto ha un operatore assegnato
        /// </summary>
        public bool HasOperatore => !string.IsNullOrEmpty(OperatoreAssegnatoId);

        /// <summary>
        /// Indica se il lotto è in fase attiva (non concluso)
        /// </summary>
        public bool IsAttivo => Stato != StatoLotto.Vinto &&
                                Stato != StatoLotto.Perso &&
                                Stato != StatoLotto.Scartato &&
                                Stato != StatoLotto.Rifiutato;

        /// <summary>
        /// Indica se il lotto è concluso (vinto/perso/scartato/rifiutato)
        /// </summary>
        public bool IsConcluso => !IsAttivo;

        /// <summary>
        /// Indica se il lotto è stato vinto
        /// </summary>
        public bool IsVinto => Stato == StatoLotto.Vinto;

        /// <summary>
        /// Indica se ci sono richieste di integrazione aperte
        /// </summary>
        public bool HasRichiesteAperte => RichiesteIntegrazioneAperte > 0;

        /// <summary>
        /// Badge CSS class basato sullo stato
        /// </summary>
        public string StatoBadgeClass => Stato switch
        {
            StatoLotto.Bozza => "badge bg-secondary",
            StatoLotto.InValutazioneTecnica => "badge bg-info",
            StatoLotto.InValutazioneEconomica => "badge bg-info",
            StatoLotto.Approvato => "badge bg-success",
            StatoLotto.Rifiutato => "badge bg-danger",
            StatoLotto.InElaborazione => "badge bg-primary",
            StatoLotto.Presentato => "badge bg-warning text-dark",
            StatoLotto.InEsame => "badge bg-warning text-dark",
            StatoLotto.RichiestaIntegrazione => "badge bg-warning text-dark",
            StatoLotto.Vinto => "badge bg-success",
            StatoLotto.Perso => "badge bg-danger",
            StatoLotto.Scartato => "badge bg-dark",
            _ => "badge bg-light text-dark"
        };

        /// <summary>
        /// Icona FontAwesome basata sullo stato
        /// </summary>
        public string StatoIcon => Stato switch
        {
            StatoLotto.Bozza => "fa-solid fa-file",
            StatoLotto.InValutazioneTecnica => "fa-solid fa-magnifying-glass",
            StatoLotto.InValutazioneEconomica => "fa-solid fa-euro-sign",
            StatoLotto.Approvato => "fa-solid fa-check-circle",
            StatoLotto.Rifiutato => "fa-solid fa-times-circle",
            StatoLotto.InElaborazione => "fa-solid fa-edit",
            StatoLotto.Presentato => "fa-solid fa-paper-plane",
            StatoLotto.InEsame => "fa-solid fa-hourglass-half",
            StatoLotto.RichiestaIntegrazione => "fa-solid fa-exclamation-triangle",
            StatoLotto.Vinto => "fa-solid fa-trophy",
            StatoLotto.Perso => "fa-solid fa-thumbs-down",
            StatoLotto.Scartato => "fa-solid fa-ban",
            _ => "fa-solid fa-question"
        };

        /// <summary>
        /// Icona FontAwesome basata sulla tipologia
        /// </summary>
        public string TipologiaIcon => Tipologia switch
        {
            TipologiaLotto.FornituraHardware => "fa-solid fa-computer",
            TipologiaLotto.Servizi => "fa-solid fa-briefcase",
            TipologiaLotto.Manutenzione => "fa-solid fa-wrench",
            TipologiaLotto.Lavori => "fa-solid fa-hammer",
            TipologiaLotto.Misto => "fa-solid fa-layer-group",
            TipologiaLotto.Altro => "fa-solid fa-ellipsis",
            _ => "fa-solid fa-box"
        };

        /// <summary>
        /// Tooltip per la tipologia
        /// </summary>
        public string TipologiaTooltip => Tipologia switch
        {
            TipologiaLotto.FornituraHardware => "Fornitura Hardware",
            TipologiaLotto.Servizi => "Prestazione Servizi",
            TipologiaLotto.Manutenzione => "Servizi di Manutenzione",
            TipologiaLotto.Lavori => "Lavori",
            TipologiaLotto.Misto => "Lotto Misto",
            TipologiaLotto.Altro => "Altra Tipologia",
            _ => Tipologia.ToString()
        };

        /// <summary>
        /// Giorni rimanenti alla scadenza offerte (null se non applicabile)
        /// </summary>
        public int? GiorniRimanenti
        {
            get
            {
                if (!DataTerminePresentazioneOfferte.HasValue)
                    return null;

                var diff = (DataTerminePresentazioneOfferte.Value - DateTime.Now).TotalDays;
                return diff > 0 ? (int)Math.Ceiling(diff) : 0;
            }
        }

        /// <summary>
        /// Testo descrittivo dello stato della scadenza
        /// </summary>
        public string ScadenzaText
        {
            get
            {
                if (!DataTerminePresentazioneOfferte.HasValue)
                    return "Nessuna scadenza";

                if (IsScaduta)
                    return "Scaduta";

                if (IsInScadenza)
                {
                    var giorni = GiorniRimanenti ?? 0;
                    return giorni == 0 ? "Scade oggi" : $"Scade tra {giorni} giorni";
                }

                return "Scadenza futura";
            }
        }

        /// <summary>
        /// CSS class per la scadenza
        /// </summary>
        public string ScadenzaClass => IsScaduta ? "text-danger" :
                                       IsInScadenza ? "text-warning fw-bold" :
                                       "text-muted";

        /// <summary>
        /// Tooltip per l'operatore assegnato
        /// </summary>
        public string OperatoreTooltip => HasOperatore
            ? $"Assegnato a: {OperatoreAssegnatoNome}"
            : "Nessun operatore assegnato";

        /// <summary>
        /// CSS class per indicare se ha operatore assegnato
        /// </summary>
        public string OperatoreClass => HasOperatore ? "text-success" : "text-muted";

        /// <summary>
        /// Icona per l'operatore
        /// </summary>
        public string OperatoreIcon => HasOperatore ? "fa-solid fa-user-check" : "fa-solid fa-user-slash";
    }
}