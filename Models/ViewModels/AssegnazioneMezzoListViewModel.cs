using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione delle assegnazioni mezzi in lista (storico)
    /// </summary>
    public class AssegnazioneMezzoListViewModel
    {
        public Guid Id { get; set; }

        // Dati Mezzo
        public Guid MezzoId { get; set; }
        public string MezzoTarga { get; set; } = string.Empty;
        public string MezzoDescrizioneCompleta { get; set; } = string.Empty;

        // Dati Utente
        public string UtenteId { get; set; } = string.Empty;
        public string UtenteNomeCompleto { get; set; } = string.Empty;

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

        // Proprietà calcolate
        public bool IsAttiva { get; set; }
        public bool IsPrenotazione { get; set; }
        public bool IsInCorso { get; set; }
        public int? DurataGiorni { get; set; }

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
    }
}