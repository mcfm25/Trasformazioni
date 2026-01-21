using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per filtrare e paginare le gare
    /// Utilizzato nelle liste e ricerche
    /// </summary>
    public class GaraFilterViewModel
    {
        // ===================================
        // PARAMETRI DI RICERCA
        // ===================================

        /// <summary>
        /// Termine di ricerca libera (codice, titolo, descrizione, ente)
        /// </summary>
        public string? SearchTerm { get; set; }

        // ===================================
        // FILTRI SPECIFICI
        // ===================================

        /// <summary>
        /// Filtro per stato
        /// </summary>
        public StatoGara? Stato { get; set; }

        /// <summary>
        /// Filtro per tipologia
        /// </summary>
        public TipologiaGara? Tipologia { get; set; }

        /// <summary>
        /// Filtro per regione
        /// </summary>
        public string? Regione { get; set; }

        /// <summary>
        /// Filtro data pubblicazione da
        /// </summary>
        public DateTime? DataPubblicazioneDa { get; set; }

        /// <summary>
        /// Filtro data pubblicazione a
        /// </summary>
        public DateTime? DataPubblicazioneA { get; set; }

        /// <summary>
        /// Mostra solo gare attive (non chiuse manualmente)
        /// </summary>
        public bool SoloAttive { get; set; } = false;

        // ===================================
        // PARAMETRI DI PAGINAZIONE
        // ===================================

        /// <summary>
        /// Numero di pagina corrente (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Numero di elementi per pagina
        /// </summary>
        public int PageSize { get; set; } = 20;

        // ===================================
        // PARAMETRI DI ORDINAMENTO
        // ===================================

        /// <summary>
        /// Campo per ordinamento
        /// Valori: "codice", "titolo", "stato", "datapubblicazione", "importo"
        /// </summary>
        public string? OrderBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Ordinamento decrescente
        /// </summary>
        public bool OrderDescending { get; set; } = true;

        // ===================================
        // METODI DI VALIDAZIONE
        // ===================================

        /// <summary>
        /// Assicura che i parametri di paginazione siano validi
        /// </summary>
        public void Validate()
        {
            if (PageNumber < 1)
                PageNumber = 1;

            if (PageSize < 1)
                PageSize = 20;

            if (PageSize > 100)
                PageSize = 100; // Limite massimo per evitare problemi di performance
        }

        // ===================================
        // UTILITY METHODS
        // ===================================

        /// <summary>
        /// Verifica se ci sono filtri attivi
        /// Esclude i parametri di paginazione e ordinamento
        /// </summary>
        public bool HasFilters =>
            !string.IsNullOrWhiteSpace(SearchTerm) ||
            Stato.HasValue ||
            Tipologia.HasValue ||
            !string.IsNullOrWhiteSpace(Regione) ||
            DataPubblicazioneDa.HasValue ||
            DataPubblicazioneA.HasValue ||
            SoloAttive;

        /// <summary>
        /// Crea una copia dei filtri per una nuova pagina
        /// Mantiene tutti i filtri ma cambia il numero di pagina
        /// </summary>
        public GaraFilterViewModel ForPage(int pageNumber)
        {
            return new GaraFilterViewModel
            {
                PageNumber = pageNumber,
                PageSize = this.PageSize,
                SearchTerm = this.SearchTerm,
                Stato = this.Stato,
                Tipologia = this.Tipologia,
                Regione = this.Regione,
                DataPubblicazioneDa = this.DataPubblicazioneDa,
                DataPubblicazioneA = this.DataPubblicazioneA,
                SoloAttive = this.SoloAttive,
                OrderBy = this.OrderBy,
                OrderDescending = this.OrderDescending
            };
        }

        /// <summary>
        /// Resetta tutti i filtri mantenendo solo paginazione e ordinamento
        /// </summary>
        public void ResetFilters()
        {
            SearchTerm = null;
            Stato = null;
            Tipologia = null;
            Regione = null;
            DataPubblicazioneDa = null;
            DataPubblicazioneA = null;
            SoloAttive = false;
            PageNumber = 1; // Torna alla prima pagina quando resetti i filtri
        }
    }
}