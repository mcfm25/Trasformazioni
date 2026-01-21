using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare i dettagli completi di un preventivo
    /// Include tutte le informazioni del preventivo e le relazioni associate
    /// Utilizzato nella pagina Details
    /// </summary>
    public class PreventivoDetailsViewModel
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

        [Display(Name = "Tipologia Lotto")]
        public TipologiaLotto TipologiaLotto { get; set; }

        [Display(Name = "Stato Lotto")]
        public StatoLotto StatoLotto { get; set; }

        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        [Display(Name = "Titolo Gara")]
        public string TitoloGara { get; set; } = string.Empty;

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        [Display(Name = "Importo Base Asta")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoBaseAstaLotto { get; set; }

        // ===== INFO FORNITORE =====

        [Display(Name = "Fornitore")]
        public string NomeFornitore { get; set; } = string.Empty;

        [Display(Name = "Tipo Soggetto")]
        public TipoSoggetto TipoSoggetto { get; set; }

        [Display(Name = "Natura Giuridica")]
        public NaturaGiuridica? NaturaGiuridica { get; set; }

        [Display(Name = "Partita IVA")]
        public string? PartitaIVAFornitore { get; set; }

        [Display(Name = "Codice Fiscale")]
        public string? CodiceFiscaleFornitore { get; set; }

        [Display(Name = "Email")]
        public string? EmailFornitore { get; set; }

        [Display(Name = "Telefono")]
        public string? TelefonoFornitore { get; set; }

        [Display(Name = "Indirizzo")]
        public string? IndirizzoFornitore { get; set; }

        // ===== INFO ECONOMICHE =====

        [Display(Name = "Importo Offerto")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoOfferto { get; set; }

        // ===== DATE =====

        [Display(Name = "Data Richiesta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataRichiesta { get; set; }

        [Display(Name = "Data Ricezione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataRicezione { get; set; }

        [Display(Name = "Data Scadenza")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataScadenza { get; set; }

        // ===== AUTO-RINNOVO =====

        [Display(Name = "Giorni Auto-Rinnovo")]
        public int? GiorniAutoRinnovo { get; set; }

        // ===== DOCUMENTO =====

        [Display(Name = "Percorso Documento")]
        public string DocumentPath { get; set; } = string.Empty;

        [Display(Name = "Nome File")]
        public string NomeFile { get; set; } = string.Empty;

        // ===== SELEZIONE =====

        [Display(Name = "Selezionato")]
        public bool IsSelezionato { get; set; }

        // ===== NOTE =====

        [Display(Name = "Note")]
        public string? Note { get; set; }

        // ===== AUDIT =====

        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Creato Da")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Modificato Da")]
        public string? ModifiedBy { get; set; }

        // ===== PROPRIETÀ COMPUTATE =====

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
        /// Indica se il preventivo può essere modificato
        /// </summary>
        public bool CanEdit => Stato != StatoPreventivo.Annullato;

        /// <summary>
        /// Indica se il preventivo può essere selezionato
        /// </summary>
        public bool CanSelect => IsValido && !IsScaduto && !IsSelezionato;

        /// <summary>
        /// Indica se il preventivo può essere annullato
        /// </summary>
        public bool CanCancel => Stato != StatoPreventivo.Annullato;

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
        /// Giorni rimanenti alla scadenza (può essere negativo se scaduto)
        /// </summary>
        public int GiorniRimanenti
        {
            get
            {
                var diff = (DataScadenza - DateTime.Now).TotalDays;
                return (int)Math.Ceiling(diff);
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
        /// Giorni trascorsi dalla ricezione (null se non ricevuto)
        /// </summary>
        public int? GiorniDallaRicezione
        {
            get
            {
                if (!DataRicezione.HasValue)
                    return null;

                var diff = (DateTime.Now - DataRicezione.Value).TotalDays;
                return (int)Math.Floor(diff);
            }
        }

        /// <summary>
        /// Tempo di attesa per ricevere il preventivo (giorni tra richiesta e ricezione)
        /// Null se non ancora ricevuto
        /// </summary>
        public int? TempoAttesaGiorni
        {
            get
            {
                if (!DataRicezione.HasValue)
                    return null;

                var diff = (DataRicezione.Value - DataRichiesta).TotalDays;
                return (int)Math.Floor(diff);
            }
        }

        /// <summary>
        /// Percentuale di sconto rispetto all'importo base asta (null se non applicabile)
        /// </summary>
        public decimal? PercentualeSconto
        {
            get
            {
                if (!ImportoOfferto.HasValue || !ImportoBaseAstaLotto.HasValue || ImportoBaseAstaLotto.Value == 0)
                    return null;

                return Math.Round((ImportoBaseAstaLotto.Value - ImportoOfferto.Value) / ImportoBaseAstaLotto.Value * 100, 2);
            }
        }

        /// <summary>
        /// Differenza assoluta tra importo offerto e importo base asta
        /// </summary>
        public decimal? DifferenzaImporti
        {
            get
            {
                if (!ImportoOfferto.HasValue || !ImportoBaseAstaLotto.HasValue)
                    return null;

                return ImportoBaseAstaLotto.Value - ImportoOfferto.Value;
            }
        }

        /// <summary>
        /// Data prevista del prossimo rinnovo (null se non ha auto-rinnovo)
        /// </summary>
        public DateTime? DataProssimoRinnovo
        {
            get
            {
                if (!HasAutoRinnovo)
                    return null;

                return DataScadenza.AddDays(GiorniAutoRinnovo!.Value);
            }
        }

        /// <summary>
        /// Testo descrittivo per il tempo di attesa
        /// </summary>
        public string TempoAttesaText
        {
            get
            {
                if (!TempoAttesaGiorni.HasValue)
                    return "In attesa di ricezione";

                var giorni = TempoAttesaGiorni.Value;
                return giorni == 0 ? "Ricevuto in giornata" :
                       giorni == 1 ? "Ricevuto in 1 giorno" :
                       $"Ricevuto in {giorni} giorni";
            }
        }

        /// <summary>
        /// CSS class per la percentuale di sconto
        /// </summary>
        public string ScontoClass
        {
            get
            {
                if (!PercentualeSconto.HasValue)
                    return "text-muted";

                return PercentualeSconto.Value >= 10 ? "text-success fw-bold" :
                       PercentualeSconto.Value >= 5 ? "text-success" :
                       PercentualeSconto.Value > 0 ? "text-info" :
                       PercentualeSconto.Value == 0 ? "text-warning" :
                       "text-danger";
            }
        }
    }
}