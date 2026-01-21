using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare i dettagli completi di una scadenza
    /// Include tutte le informazioni della scadenza e le relazioni associate
    /// Utilizzato nella pagina Details
    /// </summary>
    public class ScadenzaDetailsViewModel
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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataScadenza { get; set; }

        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        // ===== STATO =====

        [Display(Name = "Automatica")]
        public bool IsAutomatica { get; set; }

        [Display(Name = "Completata")]
        public bool IsCompletata { get; set; }

        [Display(Name = "Data Completamento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataCompletamento { get; set; }

        [Display(Name = "Giorni Preavviso")]
        public int GiorniPreavviso { get; set; }

        // ===== NOTE =====

        [Display(Name = "Note")]
        public string? Note { get; set; }

        // ===== INFO GARA (se presente) =====

        [Display(Name = "Codice Gara")]
        public string? CodiceGara { get; set; }

        [Display(Name = "Titolo Gara")]
        public string? TitoloGara { get; set; }

        [Display(Name = "Tipologia Gara")]
        public TipologiaGara? TipologiaGara { get; set; }

        [Display(Name = "Stato Gara")]
        public StatoGara? StatoGara { get; set; }

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        [Display(Name = "Regione")]
        public string? Regione { get; set; }

        // ===== INFO LOTTO (se presente) =====

        [Display(Name = "Codice Lotto")]
        public string? CodiceLotto { get; set; }

        [Display(Name = "Descrizione Lotto")]
        public string? DescrizioneLotto { get; set; }

        [Display(Name = "Tipologia Lotto")]
        public TipologiaLotto? TipologiaLotto { get; set; }

        [Display(Name = "Stato Lotto")]
        public StatoLotto? StatoLotto { get; set; }

        [Display(Name = "Importo Base Asta")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoBaseAstaLotto { get; set; }

        // ===== INFO PREVENTIVO (se presente) =====

        [Display(Name = "Fornitore")]
        public string? FornitorePreventivo { get; set; }

        [Display(Name = "Importo Offerto")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoOffertoPreventivo { get; set; }

        [Display(Name = "Stato Preventivo")]
        public StatoPreventivo? StatoPreventivo { get; set; }

        [Display(Name = "Data Richiesta Preventivo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataRichiestaPreventivo { get; set; }

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
        /// Indica se può essere modificata
        /// </summary>
        public bool CanEdit => !IsAutomatica || !IsCompletata;

        /// <summary>
        /// Indica se può essere completata
        /// </summary>
        public bool CanComplete => !IsCompletata;

        /// <summary>
        /// Indica se può essere riaperta
        /// </summary>
        public bool CanReopen => IsCompletata && !IsAutomatica;

        /// <summary>
        /// Indica se può essere eliminata
        /// </summary>
        public bool CanDelete => !IsAutomatica;

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
        /// Tempo intercorso tra creazione e completamento (null se non completata)
        /// </summary>
        public TimeSpan? TempoPerCompletamento
        {
            get
            {
                if (!DataCompletamento.HasValue)
                    return null;

                return DataCompletamento.Value - CreatedAt;
            }
        }

        /// <summary>
        /// Tempo intercorso tra creazione e completamento in giorni (null se non completata)
        /// </summary>
        public int? GiorniPerCompletamento
        {
            get
            {
                if (!TempoPerCompletamento.HasValue)
                    return null;

                return (int)Math.Floor(TempoPerCompletamento.Value.TotalDays);
            }
        }

        /// <summary>
        /// Indica se il completamento è avvenuto in tempo (prima della scadenza)
        /// Null se non completata
        /// </summary>
        public bool? CompletataInTempo
        {
            get
            {
                if (!DataCompletamento.HasValue)
                    return null;

                return DataCompletamento.Value <= DataScadenza;
            }
        }

        /// <summary>
        /// Giorni di anticipo/ritardo sul completamento
        /// Positivo = completata in anticipo, Negativo = completata in ritardo
        /// Null se non completata
        /// </summary>
        public int? GiorniAnticipoRitardo
        {
            get
            {
                if (!DataCompletamento.HasValue)
                    return null;

                var diff = (DataScadenza - DataCompletamento.Value).TotalDays;
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
                {
                    if (CompletataInTempo == true)
                    {
                        var giorni = GiorniAnticipoRitardo ?? 0;
                        return giorni == 0 ? "Completata in tempo" :
                               giorni == 1 ? "Completata con 1 giorno di anticipo" :
                               $"Completata con {giorni} giorni di anticipo";
                    }
                    else
                    {
                        var giorni = Math.Abs(GiorniAnticipoRitardo ?? 0);
                        return giorni == 0 ? "Completata il giorno della scadenza" :
                               giorni == 1 ? "Completata con 1 giorno di ritardo" :
                               $"Completata con {giorni} giorni di ritardo";
                    }
                }

                var giorniRim = GiorniRimanenti;

                if (giorniRim < 0)
                    return $"Scaduta da {Math.Abs(giorniRim)} giorni";

                if (giorniRim == 0)
                    return "Scade oggi";

                if (giorniRim == 1)
                    return "Scade domani";

                return $"Tra {giorniRim} giorni";
            }
        }

        /// <summary>
        /// CSS class per lo stato di completamento
        /// </summary>
        public string CompletamentoClass
        {
            get
            {
                if (!IsCompletata)
                    return "text-muted";

                return CompletataInTempo == true ? "text-success" : "text-danger";
            }
        }

        /// <summary>
        /// Icona per lo stato di completamento
        /// </summary>
        public string CompletamentoIcon
        {
            get
            {
                if (!IsCompletata)
                    return "fa-solid fa-clock";

                return CompletataInTempo == true ? "fa-solid fa-check-circle" : "fa-solid fa-exclamation-circle";
            }
        }

        /// <summary>
        /// Contesto completo della scadenza
        /// </summary>
        public string ContestoCompleto
        {
            get
            {
                var parts = new List<string>();

                if (HasGara && !string.IsNullOrEmpty(CodiceGara))
                    parts.Add($"Gara: {CodiceGara} - {TitoloGara}");

                if (HasLotto && !string.IsNullOrEmpty(CodiceLotto))
                    parts.Add($"Lotto: {CodiceLotto} - {DescrizioneLotto}");

                if (HasPreventivo && !string.IsNullOrEmpty(FornitorePreventivo))
                    parts.Add($"Preventivo: {FornitorePreventivo}");

                return parts.Count > 0 ? string.Join(" | ", parts) : "Scadenza generica";
            }
        }
    }
}