using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione soggetti
    /// </summary>
    public interface ISoggettoService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti i soggetti
        /// </summary>
        Task<IEnumerable<SoggettoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene il dettaglio di un soggetto
        /// </summary>
        Task<SoggettoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene tutti i clienti
        /// </summary>
        Task<IEnumerable<SoggettoListViewModel>> GetClientiAsync();

        /// <summary>
        /// Ottiene tutti i fornitori
        /// </summary>
        Task<IEnumerable<SoggettoListViewModel>> GetFornitoriAsync();

        /// <summary>
        /// Ottiene soggetti filtrati
        /// </summary>
        Task<IEnumerable<SoggettoListViewModel>> GetFilteredAsync(
            TipoSoggetto? tipo = null,
            NaturaGiuridica? natura = null,
            bool? isCliente = null,
            bool? isFornitore = null,
            string? searchTerm = null);

        /// <summary>
        /// Cerca soggetti per testo
        /// </summary>
        Task<IEnumerable<SoggettoListViewModel>> SearchAsync(string searchTerm);

        /// <summary>
        /// Ottiene soggetti paginati con filtri e ordinamento ⭐ NUOVO
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Risultato paginato con ViewModels</returns>
        Task<PagedResult<SoggettoListViewModel>> GetPagedAsync(SoggettoFilterViewModel filters);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea un nuovo soggetto
        /// Gestisce validazioni business (unicità, campi obbligatori per tipo, ecc.)
        /// </summary>
        /// <returns>(Success, ErrorMessage, SoggettoId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? SoggettoId)> CreateAsync(
            SoggettoCreateViewModel model,
            string currentUserId);

        /// <summary>
        /// Aggiorna un soggetto esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            SoggettoEditViewModel model,
            string currentUserId);

        /// <summary>
        /// Elimina un soggetto (soft delete)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Valida i dati di un soggetto
        /// Controlla campi obbligatori per tipo, formati, unicità
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateAsync(
            SoggettoCreateViewModel model,
            Guid? excludeId = null);

        /// <summary>
        /// Verifica se un codice interno è già utilizzato
        /// </summary>
        Task<bool> IsCodiceInternoUniqueAsync(string codiceInterno, Guid? excludeId = null);

        /// <summary>
        /// Verifica se una partita IVA è già utilizzata
        /// </summary>
        Task<bool> IsPartitaIVAUniqueAsync(string partitaIva, Guid? excludeId = null);

        /// <summary>
        /// Verifica se un codice fiscale è già utilizzato
        /// </summary>
        Task<bool> IsCodiceFiscaleUniqueAsync(string codiceFiscale, Guid? excludeId = null);

        /// <summary>
        /// Verifica se un'email è già utilizzata
        /// </summary>
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);

        // ===================================
        // UTILITY / STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene statistiche sui soggetti
        /// </summary>
        Task<SoggettoStatisticheViewModel> GetStatisticheAsync();

        /// <summary>
        /// Verifica se un soggetto esiste
        /// </summary>
        Task<bool> ExistsAsync(Guid id);
    }
}