namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione dei periodi occupati di un mezzo
    /// Utilizzato per il calendario FullCalendar
    /// </summary>
    public class PeriodoOccupatoViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Data e ora di inizio del periodo occupato
        /// </summary>
        public DateTime DataInizio { get; set; }

        /// <summary>
        /// Data e ora di fine del periodo occupato
        /// Null = assegnazione a tempo indeterminato (in corso)
        /// </summary>
        public DateTime? DataFine { get; set; }

        /// <summary>
        /// Nome completo dell'utente che ha il mezzo assegnato
        /// </summary>
        public string UtenteNomeCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Indica se l'assegnazione è attualmente in corso
        /// </summary>
        public bool IsInCorso { get; set; }

        /// <summary>
        /// Indica se è una prenotazione futura (non ancora iniziata)
        /// </summary>
        public bool IsPrenotazione { get; set; }

        // Proprietà per FullCalendar (formato JSON compatibile)

        /// <summary>
        /// Titolo dell'evento nel calendario
        /// </summary>
        public string Title => $"{UtenteNomeCompleto}";

        /// <summary>
        /// Data/ora di inizio in formato ISO 8601 per FullCalendar
        /// </summary>
        public string Start => DataInizio.ToString("yyyy-MM-ddTHH:mm:ss");

        /// <summary>
        /// Data/ora di fine in formato ISO 8601 per FullCalendar
        /// Null se assegnazione a tempo indeterminato
        /// </summary>
        public string? End => DataFine?.ToString("yyyy-MM-ddTHH:mm:ss");

        /// <summary>
        /// Colore dell'evento nel calendario
        /// Rosso = in corso, Giallo = prenotazione futura, Grigio = completata
        /// </summary>
        public string Color
        {
            get
            {
                if (IsInCorso)
                    return "#dc3545"; // Rosso (Bootstrap danger)
                else if (IsPrenotazione)
                    return "#ffc107"; // Giallo (Bootstrap warning)
                else
                    return "#6c757d"; // Grigio (Bootstrap secondary)
            }
        }

        /// <summary>
        /// Colore del bordo dell'evento
        /// </summary>
        public string BorderColor => Color;

        /// <summary>
        /// Tooltip descrittivo per l'evento
        /// </summary>
        public string Description
        {
            get
            {
                if (IsInCorso)
                    return $"In corso - {UtenteNomeCompleto}";
                else if (IsPrenotazione)
                    return $"Prenotazione - {UtenteNomeCompleto}";
                else
                    return $"Completata - {UtenteNomeCompleto}";
            }
        }
    }
}