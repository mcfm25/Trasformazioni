using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione dei mezzi in lista
    /// </summary>
    public class MezzoListViewModel
    {
        public Guid Id { get; set; }
        public string Targa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modello { get; set; } = string.Empty;
        public TipoMezzo Tipo { get; set; }
        public string TipoDescrizione { get; set; } = string.Empty;
        public StatoMezzo Stato { get; set; }
        public string StatoDescrizione { get; set; } = string.Empty;
        public TipoProprietaMezzo TipoProprieta { get; set; }
        public string TipoProprietaDescrizione { get; set; } = string.Empty;
        public decimal? Chilometraggio { get; set; }

        /// <summary>
        /// Indica se l'assicurazione è in scadenza (entro 30 giorni)
        /// </summary>
        public bool IsAssicurazioneInScadenza { get; set; }

        /// <summary>
        /// Indica se la revisione è in scadenza (entro 30 giorni)
        /// </summary>
        public bool IsRevisioneInScadenza { get; set; }

        /// <summary>
        /// Assegnazione attualmente attiva (se presente)
        /// </summary>
        public AssegnazioneMezzoDetailsViewModel? AssegnazioneAttiva { get; set; }


        /// <summary>
        /// Numero di prenotazioni future attive (non ancora iniziate)
        /// </summary>
        public int NumeroPrenotazioniFuture { get; set; }

        /// <summary>
        /// Numero totale di assegnazioni/prenotazioni attive (in corso + future)
        /// </summary>
        public int NumeroAssegnazioniTotali { get; set; }

        /// <summary>
        /// Indica se il mezzo ha prenotazioni in coda
        /// </summary>
        public bool HasPrenotazioniInCoda => NumeroPrenotazioniFuture > 0;

        /// <summary>
        /// Indica se il mezzo ha assegnazioni multiple
        /// </summary>
        public bool HasAssegnazioniMultiple => NumeroAssegnazioniTotali > 1;

        public string DescrizioneCompleta => $"{Marca} {Modello} - {Targa}";
    }
}