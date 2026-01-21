using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità che rappresenta l'assegnazione di un mezzo a un utente
    /// Traccia lo storico completo delle assegnazioni e prenotazioni
    /// </summary>
    public class AssegnazioneMezzo : BaseEntity
    {
        /// <summary>
        /// Identificatore univoco dell'assegnazione
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID del mezzo assegnato
        /// </summary>
        public Guid MezzoId { get; set; }

        /// <summary>
        /// ID dell'utente a cui è assegnato il mezzo
        /// </summary>
        public string UtenteId { get; set; } = string.Empty;

        /// <summary>
        /// Data e ora di inizio assegnazione (può essere futura per prenotazioni)
        /// </summary>
        public DateTime DataInizio { get; set; }

        /// <summary>
        /// Data e ora di fine assegnazione (null se ancora assegnato/prenotato)
        /// </summary>
        public DateTime? DataFine { get; set; }

        /// <summary>
        /// Motivo dell'assegnazione
        /// </summary>
        public MotivoAssegnazione MotivoAssegnazione { get; set; }

        /// <summary>
        /// Chilometraggio del mezzo al momento dell'assegnazione
        /// </summary>
        public decimal? ChilometraggioInizio { get; set; }

        /// <summary>
        /// Chilometraggio del mezzo al momento della riconsegna
        /// </summary>
        public decimal? ChilometraggioFine { get; set; }

        /// <summary>
        /// Note aggiuntive sull'assegnazione
        /// </summary>
        public string? Note { get; set; }

        // Navigation Properties

        /// <summary>
        /// Mezzo assegnato
        /// </summary>
        public virtual Mezzo Mezzo { get; set; } = null!;

        /// <summary>
        /// Utente assegnatario
        /// </summary>
        public virtual ApplicationUser Utente { get; set; } = null!;

        // Proprietà calcolate

        /// <summary>
        /// Indica se l'assegnazione è ancora attiva (non riconsegnata)
        /// LOGICA: è attiva se DataFine è null OPPURE se DataFine è nel futuro
        /// </summary>
        public bool IsAttiva => DataFine == null || DataFine > DateTime.Now;

        /// <summary>
        /// Indica se è una prenotazione futura (non ancora iniziata)
        /// </summary>
        public bool IsPrenotazione => DataInizio > DateTime.Now;

        /// <summary>
        /// Indica se l'assegnazione è attualmente in corso
        /// LOGICA: è iniziata E (senza fine OPPURE fine nel futuro)
        /// </summary>
        public bool IsInCorso => DataInizio <= DateTime.Now && (DataFine == null || DataFine > DateTime.Now);

        /// <summary>
        /// Indica se l'assegnazione è completata (chiusa)
        /// </summary>
        public bool IsCompletata => DataFine.HasValue && DataFine.Value <= DateTime.Now;

        /// <summary>
        /// Durata dell'assegnazione in giorni (se chiusa)
        /// </summary>
        public int? DurataGiorni
        {
            get
            {
                if (DataFine.HasValue)
                {
                    return (DataFine.Value.Date - DataInizio.Date).Days;
                }
                return null;
            }
        }

        /// <summary>
        /// Durata dell'assegnazione in ore (se chiusa)
        /// </summary>
        public double? DurataOre
        {
            get
            {
                if (!DataFine.HasValue)
                    return null;

                return (DataFine.Value - DataInizio).TotalHours;
            }
        }

        /// <summary>
        /// Chilometri percorsi durante l'assegnazione
        /// </summary>
        public decimal? ChilometriPercorsi
        {
            get
            {
                if (ChilometraggioInizio.HasValue && ChilometraggioFine.HasValue)
                {
                    return ChilometraggioFine.Value - ChilometraggioInizio.Value;
                }
                return null;
            }
        }
    }
}