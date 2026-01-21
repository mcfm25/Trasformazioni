using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione dettagliata di un'assegnazione mezzo
    /// </summary>
    public class AssegnazioneMezzoDetailsViewModel
    {
        public Guid Id { get; set; }

        // Dati Mezzo
        public Guid MezzoId { get; set; }
        public string MezzoTarga { get; set; } = string.Empty;
        public string MezzoTargaFormattata { get; set; } = string.Empty;
        public string MezzoMarca { get; set; } = string.Empty;
        public string MezzoModello { get; set; } = string.Empty;
        public string MezzoDescrizioneCompleta { get; set; } = string.Empty;

        // Dati Utente
        public string UtenteId { get; set; } = string.Empty;
        public string UtenteNomeCompleto { get; set; } = string.Empty;
        public string UtenteEmail { get; set; } = string.Empty;
        public string? UtenteReparto { get; set; }

        // Dati Assegnazione
        public DateTime DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        public MotivoAssegnazione MotivoAssegnazione { get; set; }
        public string MotivoAssegnazioneDescrizione { get; set; } = string.Empty;

        // Chilometraggio
        public decimal? ChilometraggioInizio { get; set; }
        public decimal? ChilometraggioFine { get; set; }
        public decimal? ChilometriPercorsi { get; set; }

        // Note
        public string? Note { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }

        // Proprietà calcolate
        public bool IsAttiva { get; set; }
        public bool IsPrenotazione { get; set; }
        public bool IsInCorso { get; set; }
        public int? DurataGiorni { get; set; }

        /// <summary>
        /// Giorni rimanenti alla data inizio (per prenotazioni)
        /// </summary>
        public int? GiorniAllaDataInizio
        {
            get
            {
                if (IsPrenotazione)
                {
                    return (DataInizio.Date - DateTime.Now.Date).Days;
                }
                return null;
            }
        }

        /// <summary>
        /// Giorni dall'inizio dell'assegnazione (per assegnazioni in corso)
        /// </summary>
        public int? GiorniDallInizio
        {
            get
            {
                if (IsInCorso)
                {
                    return (DateTime.Now.Date - DataInizio.Date).Days;
                }
                return null;
            }
        }

        /// <summary>
        /// Badge descrittivo dello stato dell'assegnazione
        /// </summary>
        public string StatoDescrizione
        {
            get
            {
                if (IsPrenotazione)
                    return "Prenotazione";
                else if (IsInCorso)
                    return "In Corso";
                else
                    return "Completata";
            }
        }

        /// <summary>
        /// Classe CSS Bootstrap per il badge dello stato
        /// </summary>
        public string StatoBadgeClass
        {
            get
            {
                if (IsPrenotazione)
                    return "bg-info";
                else if (IsInCorso)
                    return "bg-primary";
                else
                    return "bg-secondary";
            }
        }
    }
}