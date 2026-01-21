using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per filtrare e paginare i lotti
    /// Utilizzato nelle liste e ricerche
    /// </summary>
    public class LottoFilterViewModel
    {
        // ===================================
        // PARAMETRI DI RICERCA
        // ===================================

        /// <summary>
        /// Termine di ricerca libera (codice lotto, descrizione, codice gara)
        /// </summary>
        public string? SearchTerm { get; set; }

        // ===================================
        // FILTRI SPECIFICI
        // ===================================

        /// <summary>
        /// Filtro per gara specifica
        /// </summary>
        public Guid? GaraId { get; set; }

        /// <summary>
        /// Filtro per stato del lotto
        /// </summary>
        public StatoLotto? Stato { get; set; }

        /// <summary>
        /// Filtro per tipologia del lotto
        /// </summary>
        public TipologiaLotto? Tipologia { get; set; }

        /// <summary>
        /// Filtro per operatore assegnato
        /// </summary>
        public string? OperatoreAssegnatoId { get; set; }

        /// <summary>
        /// Mostra solo lotti con operatore assegnato
        /// </summary>
        public bool SoloConOperatore { get; set; } = false;

        // Range importo
        public decimal? ImportoMin { get; set; }
        public decimal? ImportoMax { get; set; }

        // Checkbox filtri rapidi
        public bool SoloAttivi { get; set; }
        public bool SoloVinti { get; set; }
        public bool SoloPersi { get; set; }

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
        /// Valori: "codice", "descrizione", "stato", "importo", "gara"
        /// </summary>
        public string? OrderBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; }

        /// <summary>
        /// Ordinamento decrescente
        /// </summary>
        public bool OrderDescending { get; set; } = true;

        // Computed
        public bool HasFilters =>
            !string.IsNullOrWhiteSpace(SearchTerm) ||
            Stato.HasValue ||
            Tipologia.HasValue ||
            GaraId.HasValue ||
            ImportoMin.HasValue ||
            ImportoMax.HasValue ||
            SoloAttivi ||
            SoloVinti ||
            SoloPersi;

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
    }
}