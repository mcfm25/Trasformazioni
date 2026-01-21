using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per filtrare e paginare le scadenze
    /// Utilizzato nella pagina Scadenzario con liste e ricerche
    /// </summary>
    public class ScadenzaFilterViewModel
    {
        // ===================================
        // PARAMETRI DI RICERCA
        // ===================================

        /// <summary>
        /// Termine di ricerca libera (descrizione, note)
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
        /// Filtro per lotto specifico
        /// </summary>
        public Guid? LottoId { get; set; }

        /// <summary>
        /// Filtro per preventivo specifico
        /// </summary>
        public Guid? PreventivoId { get; set; }

        /// <summary>
        /// Filtro per tipo di scadenza
        /// </summary>
        public TipoScadenza? Tipo { get; set; }

        /// <summary>
        /// Filtro per scadenze completate (null = tutte, true = solo completate, false = solo attive)
        /// </summary>
        public bool? IsCompletata { get; set; }

        /// <summary>
        /// Filtro per scadenze automatiche (null = tutte, true = solo automatiche, false = solo manuali)
        /// </summary>
        public bool? IsAutomatica { get; set; }

        /// <summary>
        /// Mostra solo scadenze scadute (data passata e non completate)
        /// </summary>
        public bool SoloScadute { get; set; } = false;

        /// <summary>
        /// Mostra solo scadenze in scadenza (entro X giorni e non completate)
        /// </summary>
        public bool SoloInScadenza { get; set; } = false;

        /// <summary>
        /// Numero di giorni per considerare una scadenza "in scadenza"
        /// Usato solo se SoloInScadenza = true
        /// </summary>
        public int GiorniScadenza { get; set; } = 7;

        /// <summary>
        /// Mostra solo scadenze di oggi
        /// </summary>
        public bool SoloOggi { get; set; } = false;

        /// <summary>
        /// Filtro data scadenza da
        /// </summary>
        public DateTime? DataScadenzaDa { get; set; }

        /// <summary>
        /// Filtro data scadenza a
        /// </summary>
        public DateTime? DataScadenzaA { get; set; }

        /// <summary>
        /// Filtro data completamento da
        /// </summary>
        public DateTime? DataCompletamentoDa { get; set; }

        /// <summary>
        /// Filtro data completamento a
        /// </summary>
        public DateTime? DataCompletamentoA { get; set; }

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
        /// Valori: "data", "tipo", "descrizione", "stato", "completamento"
        /// Default: "data" (DataScadenza)
        /// </summary>
        public string? OrderBy { get; set; } = "data";

        /// <summary>
        /// Ordinamento decrescente
        /// </summary>
        public bool OrderDescending { get; set; } = false; // Default ASC per scadenze (prossime prima)

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

            if (GiorniScadenza < 1)
                GiorniScadenza = 7;

            if (GiorniScadenza > 365)
                GiorniScadenza = 365; // Limite massimo sensato
        }

        // ===================================
        // UTILITY METHODS
        // ===================================

        /// <summary>
        /// Verifica se ci sono filtri attivi
        /// </summary>
        public bool HasFilters => !string.IsNullOrWhiteSpace(SearchTerm)
            || GaraId.HasValue
            || LottoId.HasValue
            || PreventivoId.HasValue
            || Tipo.HasValue
            || IsCompletata.HasValue
            || IsAutomatica.HasValue
            || SoloScadute
            || SoloInScadenza
            || SoloOggi
            || DataScadenzaDa.HasValue
            || DataScadenzaA.HasValue
            || DataCompletamentoDa.HasValue
            || DataCompletamentoA.HasValue;

        /// <summary>
        /// Crea una copia dei filtri per una nuova pagina
        /// </summary>
        public ScadenzaFilterViewModel ForPage(int pageNumber)
        {
            return new ScadenzaFilterViewModel
            {
                PageNumber = pageNumber,
                PageSize = this.PageSize,
                SearchTerm = this.SearchTerm,
                GaraId = this.GaraId,
                LottoId = this.LottoId,
                PreventivoId = this.PreventivoId,
                Tipo = this.Tipo,
                IsCompletata = this.IsCompletata,
                IsAutomatica = this.IsAutomatica,
                SoloScadute = this.SoloScadute,
                SoloInScadenza = this.SoloInScadenza,
                GiorniScadenza = this.GiorniScadenza,
                SoloOggi = this.SoloOggi,
                DataScadenzaDa = this.DataScadenzaDa,
                DataScadenzaA = this.DataScadenzaA,
                DataCompletamentoDa = this.DataCompletamentoDa,
                DataCompletamentoA = this.DataCompletamentoA,
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
            GaraId = null;
            LottoId = null;
            PreventivoId = null;
            Tipo = null;
            IsCompletata = null;
            IsAutomatica = null;
            SoloScadute = false;
            SoloInScadenza = false;
            SoloOggi = false;
            GiorniScadenza = 7;
            DataScadenzaDa = null;
            DataScadenzaA = null;
            DataCompletamentoDa = null;
            DataCompletamentoA = null;
            PageNumber = 1; // Torna alla prima pagina quando resetti i filtri
        }

        /// <summary>
        /// Imposta filtri per vedere scadenze di oggi
        /// </summary>
        public void SetFiltroOggi()
        {
            ResetFilters();
            SoloOggi = true;
            IsCompletata = false; // Solo attive
            OrderBy = "data";
            OrderDescending = false;
        }

        /// <summary>
        /// Imposta filtri per vedere scadenze in scadenza (prossimi 7 giorni)
        /// </summary>
        public void SetFiltroInScadenza(int giorni = 7)
        {
            ResetFilters();
            SoloInScadenza = true;
            GiorniScadenza = giorni;
            IsCompletata = false; // Solo attive
            OrderBy = "data";
            OrderDescending = false;
        }

        /// <summary>
        /// Imposta filtri per vedere solo scadenze scadute
        /// </summary>
        public void SetFiltroScadute()
        {
            ResetFilters();
            SoloScadute = true;
            IsCompletata = false; // Solo attive (quelle completate non sono "scadute")
            OrderBy = "data";
            OrderDescending = false;
        }

        /// <summary>
        /// Imposta filtri per vedere solo scadenze attive (non completate)
        /// </summary>
        public void SetFiltroAttive()
        {
            ResetFilters();
            IsCompletata = false;
            OrderBy = "data";
            OrderDescending = false;
        }

        /// <summary>
        /// Imposta filtri per vedere solo scadenze completate
        /// </summary>
        public void SetFiltroCompletate()
        {
            ResetFilters();
            IsCompletata = true;
            OrderBy = "completamento";
            OrderDescending = true; // Le più recenti prima
        }
    }
}