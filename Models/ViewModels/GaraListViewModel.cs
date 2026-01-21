using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare una gara nelle liste paginate
    /// Contiene solo le informazioni essenziali per la visualizzazione in elenco
    /// Utilizzato in combinazione con GaraFilterViewModel e PagedResult
    /// </summary>
    public class GaraListViewModel
    {
        public Guid Id { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        [Display(Name = "Titolo")]
        public string Titolo { get; set; } = string.Empty;

        [Display(Name = "Tipologia")]
        public TipologiaGara Tipologia { get; set; }

        [Display(Name = "Stato")]
        public StatoGara Stato { get; set; }

        // ===== INFO AMMINISTRAZIONE =====

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        [Display(Name = "Regione")]
        public string? Regione { get; set; }

        // ===== CODICI GARA =====

        [Display(Name = "CIG")]
        public string? CIG { get; set; }

        [Display(Name = "CUP")]
        public string? CUP { get; set; }

        // ===== DATE CRITICHE =====

        [Display(Name = "Data Pubblicazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataPubblicazione { get; set; }

        [Display(Name = "Scadenza Offerte")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== INFO ECONOMICHE =====

        [Display(Name = "Importo Totale Stimato")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoTotaleStimato { get; set; }

        // ===== STATISTICHE LOTTI =====

        [Display(Name = "Numero Lotti")]
        public int NumeroLotti { get; set; }

        [Display(Name = "Lotti Vinti")]
        public int LottiVinti { get; set; }

        [Display(Name = "Lotti Persi")]
        public int LottiPersi { get; set; }

        [Display(Name = "Lotti In Lavorazione")]
        public int LottiInLavorazione { get; set; }

        // ===== CHIUSURA MANUALE =====

        [Display(Name = "Chiusa Manualmente")]
        public bool IsChiusaManualmente { get; set; }

        [Display(Name = "Data Chiusura")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataChiusuraManuale { get; set; }

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
        /// Indica se ci sono lotti attivi (in lavorazione)
        /// </summary>
        public bool HasLottiAttivi => LottiInLavorazione > 0;

        /// <summary>
        /// Percentuale di successo (lotti vinti / totale lotti completati)
        /// Restituisce null se non ci sono lotti completati
        /// </summary>
        public decimal? PercentualeSuccesso
        {
            get
            {
                var completati = LottiVinti + LottiPersi;
                if (completati == 0)
                    return null;

                return Math.Round((decimal)LottiVinti / completati * 100, 1);
            }
        }

        /// <summary>
        /// Badge CSS class basato sullo stato
        /// </summary>
        public string StatoBadgeClass => Stato switch
        {
            StatoGara.Bozza => "badge bg-secondary",
            StatoGara.InLavorazione => "badge bg-primary",
            StatoGara.Conclusa => "badge bg-success",
            StatoGara.ChiusaManualmente => "badge bg-dark",
            _ => "badge bg-light text-dark"
        };

        /// <summary>
        /// Icona FontAwesome basata sulla tipologia
        /// </summary>
        public string TipologiaIcon => Tipologia switch
        {
            TipologiaGara.AppaltoPubblico => "fa-solid fa-building-columns",
            TipologiaGara.RDO => "fa-solid fa-file-invoice",
            TipologiaGara.GaraTelematica => "fa-solid fa-laptop",
            TipologiaGara.AccordoQuadro => "fa-solid fa-file-contract",
            TipologiaGara.SistemaDinamico => "fa-solid fa-arrows-spin",
            TipologiaGara.ProceduraNegoziata => "fa-solid fa-handshake",
            TipologiaGara.Altra => "fa-solid fa-ellipsis",
            _ => "fa-solid fa-file-invoice"
        };

        /// <summary>
        /// Tooltip per la tipologia
        /// </summary>
        public string TipologiaTooltip => Tipologia switch
        {
            TipologiaGara.AppaltoPubblico => "Appalto Pubblico",
            TipologiaGara.RDO => "RDO - Richiesta di Offerta",
            TipologiaGara.GaraTelematica => "Gara Telematica",
            TipologiaGara.AccordoQuadro => "Accordo Quadro",
            TipologiaGara.SistemaDinamico => "Sistema Dinamico di Acquisizione",
            TipologiaGara.ProceduraNegoziata => "Procedura Negoziata",
            TipologiaGara.Altra => "Altra Tipologia",
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
    }
}