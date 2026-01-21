using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione categorie contratto
    /// </summary>
    public interface ICategoriaContrattoService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutte le categorie
        /// </summary>
        Task<IEnumerable<CategoriaContrattoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene tutte le categorie attive (per dropdown)
        /// </summary>
        Task<IEnumerable<CategoriaContrattoListViewModel>> GetAttiveAsync();

        /// <summary>
        /// Ottiene il dettaglio di una categoria
        /// </summary>
        Task<CategoriaContrattoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene una categoria per modifica
        /// </summary>
        Task<CategoriaContrattoEditViewModel?> GetForEditAsync(Guid id);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea una nuova categoria
        /// </summary>
        /// <returns>(Success, ErrorMessage, CategoriaId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? CategoriaId)> CreateAsync(
            CategoriaContrattoCreateViewModel model);

        /// <summary>
        /// Aggiorna una categoria esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            CategoriaContrattoEditViewModel model);

        /// <summary>
        /// Elimina una categoria (soft delete)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id);

        /// <summary>
        /// Attiva una categoria
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> AttivaAsync(Guid id);

        /// <summary>
        /// Disattiva una categoria
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DisattivaAsync(Guid id);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se il nome è univoco
        /// </summary>
        Task<bool> IsNomeUniqueAsync(string nome, Guid? excludeId = null);

        /// <summary>
        /// Verifica se la categoria è utilizzata
        /// </summary>
        Task<bool> IsUsedAsync(Guid id);

        /// <summary>
        /// Verifica se la categoria esiste
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // UTILITY
        // ===================================

        /// <summary>
        /// Ottiene il prossimo ordine disponibile
        /// </summary>
        Task<int> GetNextOrdineAsync();

        /// <summary>
        /// Conta quanti registri utilizzano la categoria
        /// </summary>
        Task<int> CountUsageAsync(Guid id);

        /// <summary>
        /// Ottiene statistiche sulle categorie
        /// </summary>
        Task<(int Totale, int Attive, int Inattive)> GetStatisticheAsync();
    }
}