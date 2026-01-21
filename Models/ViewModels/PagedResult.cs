namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// Classe generica per risultati paginati
    /// </summary>
    /// <typeparam name="T">Tipo degli elementi</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Lista degli elementi della pagina corrente
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Numero della pagina corrente (1-based)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Numero di elementi per pagina
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Numero totale di elementi (su tutte le pagine)
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Numero totale di pagine
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        /// <summary>
        /// Indica se esiste una pagina precedente
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indica se esiste una pagina successiva
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Numero della prima riga visualizzata (per "Mostra 1-20 di 100")
        /// </summary>
        public int FirstItemOnPage => TotalItems == 0 ? 0 : (PageNumber - 1) * PageSize + 1;

        /// <summary>
        /// Numero dell'ultima riga visualizzata
        /// </summary>
        public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalItems);
    }
}