using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia service per la gestione dei TipiDocumento.
    /// Fornisce la business logic per le operazioni CRUD e query.
    /// </summary>
    public interface ITipoDocumentoService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti i tipi documento
        /// </summary>
        Task<IEnumerable<TipoDocumentoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene un tipo documento per ID (dettaglio)
        /// </summary>
        Task<TipoDocumentoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene un tipo documento per modifica
        /// </summary>
        Task<TipoDocumentoEditViewModel?> GetForEditAsync(Guid id);

        /// <summary>
        /// Ottiene tutti i tipi documento per una specifica area
        /// </summary>
        Task<IEnumerable<TipoDocumentoListViewModel>> GetByAreaAsync(AreaDocumento area);

        /// <summary>
        /// Ottiene i tipi documento per dropdown (SelectList)
        /// </summary>
        /// <param name="area">Filtra per area (opzionale)</param>
        Task<IEnumerable<TipoDocumentoDropdownViewModel>> GetForDropdownAsync(AreaDocumento? area = null);

        /// <summary>
        /// Verifica se un nome è disponibile per una specifica area
        /// </summary>
        Task<bool> IsNomeDisponibileAsync(string nome, AreaDocumento area, Guid? excludeId = null);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea un nuovo tipo documento
        /// </summary>
        /// <returns>Tuple con successo, messaggio errore e ID creato</returns>
        Task<(bool Success, string? ErrorMessage, Guid? Id)> CreateAsync(TipoDocumentoCreateViewModel model, string userId);

        /// <summary>
        /// Aggiorna un tipo documento esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(TipoDocumentoEditViewModel model, string userId);

        /// <summary>
        /// Elimina un tipo documento (soft delete)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string userId);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se un tipo documento può essere eliminato
        /// </summary>
        /// <returns>Tuple con possibilità di eliminazione e motivo</returns>
        Task<(bool CanDelete, string? Reason)> CanDeleteAsync(Guid id);

        /// <summary>
        /// Verifica se un tipo documento può essere modificato
        /// </summary>
        Task<(bool CanEdit, string? Reason)> CanEditAsync(Guid id);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene statistiche sui tipi documento
        /// </summary>
        Task<Dictionary<string, int>> GetStatisticheAsync();

        /// <summary>
        /// Ottiene il conteggio dei tipi per area
        /// </summary>
        Task<Dictionary<AreaDocumento, int>> GetCountByAreaAsync();
    }
}
