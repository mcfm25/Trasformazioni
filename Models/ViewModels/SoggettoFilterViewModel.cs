using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per filtri e paginazione della lista soggetti
    /// </summary>
    public class SoggettoFilterViewModel
    {
        // ===================================
        // PAGINAZIONE
        // ===================================

        /// <summary>
        /// Numero pagina corrente (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Numero di elementi per pagina
        /// </summary>
        public int PageSize { get; set; } = 20;

        // ===================================
        // FILTRI
        // ===================================

        /// <summary>
        /// Filtro per tipo soggetto
        /// </summary>
        public TipoSoggetto? TipoSoggetto { get; set; }

        /// <summary>
        /// Filtro per natura giuridica
        /// </summary>
        public NaturaGiuridica? NaturaGiuridica { get; set; }

        /// <summary>
        /// Filtro per clienti (null = tutti, true = solo clienti, false = solo non clienti)
        /// </summary>
        public bool? IsCliente { get; set; }

        /// <summary>
        /// Filtro per fornitori (null = tutti, true = solo fornitori, false = solo non fornitori)
        /// </summary>
        public bool? IsFornitore { get; set; }

        /// <summary>
        /// Termine di ricerca (cerca in denominazione, nome, cognome, email, P.IVA, CF, codice interno)
        /// </summary>
        public string? SearchTerm { get; set; }

        // ===================================
        // ORDINAMENTO
        // ===================================

        /// <summary>
        /// Campo per ordinamento
        /// Valori possibili: "nome", "tipo", "natura", "citta", "email", "ruolo"
        /// Default: "nome"
        /// </summary>
        public string OrderBy { get; set; } = "nome";

        /// <summary>
        /// Direzione ordinamento: "asc" o "desc"
        /// Default: "asc"
        /// </summary>
        public string OrderDirection { get; set; } = "asc";

        // ===================================
        // UTILITY METHODS
        // ===================================

        /// <summary>
        /// Verifica se ci sono filtri attivi
        /// </summary>
        public bool HasFilters => TipoSoggetto.HasValue
            || NaturaGiuridica.HasValue
            || IsCliente.HasValue
            || IsFornitore.HasValue
            || !string.IsNullOrWhiteSpace(SearchTerm);

        /// <summary>
        /// Crea una copia dei filtri per una nuova pagina
        /// </summary>
        public SoggettoFilterViewModel ForPage(int pageNumber)
        {
            return new SoggettoFilterViewModel
            {
                PageNumber = pageNumber,
                PageSize = this.PageSize,
                TipoSoggetto = this.TipoSoggetto,
                NaturaGiuridica = this.NaturaGiuridica,
                IsCliente = this.IsCliente,
                IsFornitore = this.IsFornitore,
                SearchTerm = this.SearchTerm,
                OrderBy = this.OrderBy,
                OrderDirection = this.OrderDirection
            };
        }

        /// <summary>
        /// Resetta tutti i filtri mantenendo solo paginazione e ordinamento
        /// </summary>
        public void ResetFilters()
        {
            TipoSoggetto = null;
            NaturaGiuridica = null;
            IsCliente = null;
            IsFornitore = null;
            SearchTerm = null;
            PageNumber = 1; // Torna alla prima pagina quando resetti i filtri
        }
    }
}