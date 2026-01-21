using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per filtrare e paginare i preventivi
    /// Utilizzato nelle liste e ricerche
    /// </summary>
    public class PreventivoFilterViewModel
    {
        // ===================================
        // PARAMETRI DI RICERCA
        // ===================================

        /// <summary>
        /// Termine di ricerca libera (descrizione, nome file, codice fornitore)
        /// </summary>
        public string? SearchTerm { get; set; }

        // ===================================
        // FILTRI SPECIFICI
        // ===================================

        /// <summary>
        /// Filtro per lotto specifico
        /// </summary>
        public Guid? LottoId { get; set; }

        /// <summary>
        /// Filtro per fornitore specifico
        /// </summary>
        public Guid? SoggettoId { get; set; }

        /// <summary>
        /// Filtro per stato del preventivo
        /// </summary>
        public StatoPreventivo? Stato { get; set; }

        /// <summary>
        /// Filtro data richiesta da
        /// </summary>
        public DateTime? DataRichiestaDa { get; set; }

        /// <summary>
        /// Filtro data richiesta a
        /// </summary>
        public DateTime? DataRichiestaA { get; set; }

        /// <summary>
        /// Filtro data scadenza da
        /// </summary>
        public DateTime? DataScadenzaDa { get; set; }

        /// <summary>
        /// Filtro data scadenza a
        /// </summary>
        public DateTime? DataScadenzaA { get; set; }

        /// <summary>
        /// Filtro importo minimo offerto
        /// </summary>
        public decimal? ImportoMinimo { get; set; }

        /// <summary>
        /// Filtro importo massimo offerto
        /// </summary>
        public decimal? ImportoMassimo { get; set; }

        /// <summary>
        /// Mostra solo preventivi selezionati (vincitori)
        /// </summary>
        public bool SoloSelezionati { get; set; } = false;

        /// <summary>
        /// Mostra solo preventivi scaduti
        /// </summary>
        public bool SoloScaduti { get; set; } = false;

        /// <summary>
        /// Mostra solo preventivi in scadenza (entro 7 giorni)
        /// </summary>
        public bool SoloInScadenza { get; set; } = false;

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
        /// Valori: "descrizione", "stato", "datarichiesta", "datascadenza", "importo", "fornitore", "lotto"
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
    }
}