using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare una scadenza nelle liste paginate
    /// Contiene solo le informazioni essenziali per la visualizzazione in elenco
    /// Utilizzato in combinazione con ScadenzaFilterViewModel e PagedResult
    /// </summary>
    public class ScadenzaListViewModel
    {
        public Guid Id { get; set; }

        // ===== RELAZIONI (NULLABLE) =====

        public Guid? GaraId { get; set; }
        public Guid? LottoId { get; set; }
        public Guid? PreventivoId { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Display(Name = "Tipo")]
        public TipoScadenza Tipo { get; set; }

        [Display(Name = "Data Scadenza")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DataScadenza { get; set; }

        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        // ===== STATO =====

        [Display(Name = "Automatica")]
        public bool IsAutomatica { get; set; }

        [Display(Name = "Completata")]
        public bool IsCompletata { get; set; }

        [Display(Name = "Data Completamento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataCompletamento { get; set; }

        [Display(Name = "Giorni Preavviso")]
        public int GiorniPreavviso { get; set; }

        // ===== INFO CONTESTUALI =====

        [Display(Name = "Codice Gara")]
        public string? CodiceGara { get; set; }

        [Display(Name = "Codice Lotto")]
        public string? CodiceLotto { get; set; }

        [Display(Name = "Fornitore Preventivo")]
        public string? FornitorePreventivo { get; set; }

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        // ===== AUDIT =====

        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? ModifiedAt { get; set; }

        // ===== PROPRIETÀ COMPUTATE (per UI) =====

        /// <summary>
        /// Verifica se la scadenza è imminente (entro i giorni di preavviso)
        /// </summary>
        public bool IsInScadenza => !IsCompletata &&
                                    DataScadenza > DateTime.Now &&
                                    DataScadenza <= DateTime.Now.AddDays(GiorniPreavviso);

        /// <summary>
        /// Verifica se la scadenza è già passata e non completata
        /// </summary>
        public bool IsScaduta => !IsCompletata && DataScadenza < DateTime.Now;

        /// <summary>
        /// Verifica se la scadenza è oggi
        /// </summary>
        public bool IsOggi => !IsCompletata &&
                              DataScadenza.Date == DateTime.Today;

        /// <summary>
        /// Indica se la scadenza è attiva (non completata e futura)
        /// </summary>
        public bool IsAttiva => !IsCompletata && DataScadenza >= DateTime.Now;

        /// <summary>
        /// Indica se ha una gara associata
        /// </summary>
        public bool HasGara => GaraId.HasValue;

        /// <summary>
        /// Indica se ha un lotto associato
        /// </summary>
        public bool HasLotto => LottoId.HasValue;

        /// <summary>
        /// Indica se ha un preventivo associato
        /// </summary>
        public bool HasPreventivo => PreventivoId.HasValue;

        /// <summary>
        /// Badge CSS class basato sullo stato
        /// </summary>
        public string StatoBadgeClass
        {
            get
            {
                if (IsCompletata)
                    return "badge bg-success";

                if (IsScaduta)
                    return "badge bg-danger";

                if (IsOggi)
                    return "badge bg-warning text-dark";

                if (IsInScadenza)
                    return "badge bg-warning";

                return "badge bg-primary";
            }
        }

        /// <summary>
        /// Testo dello stato
        /// </summary>
        public string StatoText
        {
            get
            {
                if (IsCompletata)
                    return "Completata";

                if (IsScaduta)
                    return "Scaduta";

                if (IsOggi)
                    return "Oggi";

                if (IsInScadenza)
                    return "In Scadenza";

                return "Attiva";
            }
        }

        /// <summary>
        /// Icona FontAwesome basata sullo stato
        /// </summary>
        public string StatoIcon
        {
            get
            {
                if (IsCompletata)
                    return "fa-solid fa-check-circle";

                if (IsScaduta)
                    return "fa-solid fa-times-circle";

                if (IsOggi)
                    return "fa-solid fa-exclamation-circle";

                if (IsInScadenza)
                    return "fa-solid fa-exclamation-triangle";

                return "fa-solid fa-clock";
            }
        }

        /// <summary>
        /// Icona FontAwesome basata sul tipo
        /// </summary>
        public string TipoIcon => Tipo switch
        {
            TipoScadenza.PresentazioneOfferta => "fa-solid fa-paper-plane",
            TipoScadenza.RichiestaChiarimenti => "fa-solid fa-question-circle",
            TipoScadenza.ScadenzaPreventivo => "fa-solid fa-file-invoice",
            TipoScadenza.IntegrazioneDocumentazione => "fa-solid fa-file-upload",
            TipoScadenza.StipulaContratto => "fa-solid fa-file-signature",
            TipoScadenza.ScadenzaContratto => "fa-solid fa-calendar-times",
            TipoScadenza.Altro => "fa-solid fa-ellipsis",
            _ => "fa-solid fa-calendar"
        };

        /// <summary>
        /// Badge per scadenza automatica
        /// </summary>
        public string AutomaticaBadge => IsAutomatica ? "badge bg-info" : "";

        /// <summary>
        /// Icona per scadenza automatica
        /// </summary>
        public string AutomaticaIcon => IsAutomatica ? "fa-solid fa-robot" : "fa-solid fa-user";

        /// <summary>
        /// Tooltip per scadenza automatica
        /// </summary>
        public string AutomaticaTooltip => IsAutomatica ? "Scadenza automatica" : "Scadenza manuale";

        /// <summary>
        /// Giorni rimanenti alla scadenza (può essere negativo se scaduta)
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
        /// Giorni trascorsi dal completamento (null se non completata)
        /// </summary>
        public int? GiorniDalCompletamento
        {
            get
            {
                if (!DataCompletamento.HasValue)
                    return null;

                var diff = (DateTime.Now - DataCompletamento.Value).TotalDays;
                return (int)Math.Floor(diff);
            }
        }

        /// <summary>
        /// Testo descrittivo dei giorni rimanenti
        /// </summary>
        public string GiorniRimanentiText
        {
            get
            {
                if (IsCompletata)
                    return "Completata";

                var giorni = GiorniRimanenti;

                if (giorni < 0)
                    return $"Scaduta da {Math.Abs(giorni)} giorni";

                if (giorni == 0)
                    return "Scade oggi";

                if (giorni == 1)
                    return "Scade domani";

                return $"Tra {giorni} giorni";
            }
        }

        /// <summary>
        /// CSS class per i giorni rimanenti
        /// </summary>
        public string GiorniRimanentiClass
        {
            get
            {
                if (IsCompletata)
                    return "text-success";

                if (IsScaduta)
                    return "text-danger fw-bold";

                if (IsOggi)
                    return "text-warning fw-bold";

                if (IsInScadenza)
                    return "text-warning";

                return "text-primary";
            }
        }

        /// <summary>
        /// Contesto della scadenza (Gara/Lotto/Preventivo)
        /// </summary>
        public string Contesto
        {
            get
            {
                if (HasPreventivo && !string.IsNullOrEmpty(FornitorePreventivo))
                    return $"Preventivo: {FornitorePreventivo}";

                if (HasLotto && !string.IsNullOrEmpty(CodiceLotto))
                    return $"Lotto: {CodiceLotto}";

                if (HasGara && !string.IsNullOrEmpty(CodiceGara))
                    return $"Gara: {CodiceGara}";

                return "Scadenza generica";
            }
        }

        /// <summary>
        /// Icona del contesto
        /// </summary>
        public string ContestoIcon
        {
            get
            {
                if (HasPreventivo)
                    return "fa-solid fa-file-invoice";

                if (HasLotto)
                    return "fa-solid fa-box";

                if (HasGara)
                    return "fa-solid fa-building-columns";

                return "fa-solid fa-calendar";
            }
        }
    }
}