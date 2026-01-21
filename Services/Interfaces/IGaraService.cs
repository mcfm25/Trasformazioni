using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione Gare
    /// </summary>
    public interface IGaraService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutte le gare
        /// </summary>
        Task<IEnumerable<GaraListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene il dettaglio di una gara
        /// </summary>
        Task<GaraDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene una gara per la modifica (include DocumentiRichiesti)
        /// </summary>
        Task<GaraEditViewModel?> GetForEditAsync(Guid id);

        /// <summary>
        /// Ottiene gare filtrate per stato
        /// </summary>
        Task<IEnumerable<GaraListViewModel>> GetByStatoAsync(StatoGara stato);

        /// <summary>
        /// Ottiene gare filtrate per tipologia
        /// </summary>
        Task<IEnumerable<GaraListViewModel>> GetByTipologiaAsync(TipologiaGara tipologia);

        /// <summary>
        /// Ottiene gare attive (non chiuse manualmente)
        /// </summary>
        Task<IEnumerable<GaraListViewModel>> GetActiveGareAsync();

        /// <summary>
        /// Ottiene gare con scadenza offerte imminente
        /// </summary>
        Task<IEnumerable<GaraListViewModel>> GetGareInScadenzaAsync(int giorniProssimi = 7);

        /// <summary>
        /// Cerca gare per testo
        /// </summary>
        Task<IEnumerable<GaraListViewModel>> SearchAsync(string searchTerm);

        /// <summary>
        /// Ottiene gare paginate con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Risultato paginato con ViewModels</returns>
        Task<PagedResult<GaraListViewModel>> GetPagedAsync(GaraFilterViewModel filters);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea una nuova gara
        /// Gestisce validazioni business (unicità CIG, date, campi obbligatori, ecc.)
        /// </summary>
        /// <returns>(Success, ErrorMessage, GaraId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? GaraId)> CreateAsync(GaraCreateViewModel model);

        /// <summary>
        /// Aggiorna una gara esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(GaraEditViewModel model);

        /// <summary>
        /// Aggiorna solo lo stato di una gara esistente
        /// Metodo ottimizzato per cambi di stato senza modificare altri campi
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <param name="nuovoStato">Nuovo stato da impostare</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> UpdateStatoAsync(Guid garaId, StatoGara nuovoStato);

        /// <summary>
        /// Elimina una gara (soft delete)
        /// Verifica che non abbia lotti con preventivi/documenti
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id);

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE
        // ===================================

        /// <summary>
        /// Conclude una gara verificando che tutti i lotti siano in stato terminale
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> ConcludiGaraAsync(Guid garaId);

        /// <summary>
        /// Riattiva una gara chiusa manualmente
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> RiattivaGaraAsync(Guid garaId);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se un codice gara è unico
        /// </summary>
        Task<bool> IsCodiceGaraUniqueAsync(string codiceGara, Guid? excludeId = null);

        /// <summary>
        /// Verifica se un CIG è unico
        /// </summary>
        Task<bool> IsCIGUniqueAsync(string cig, Guid? excludeId = null);

        /// <summary>
        /// Verifica se tutti i lotti di una gara sono in stato terminale
        /// </summary>
        Task<bool> AreAllLottiInStatoTerminaleAsync(Guid garaId);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio delle gare per stato
        /// </summary>
        Task<Dictionary<StatoGara, int>> GetCountByStatoAsync();

        /// <summary>
        /// Ottiene l'importo totale delle gare per stato
        /// </summary>
        Task<Dictionary<StatoGara, decimal>> GetImportoTotaleByStatoAsync();
    }
}