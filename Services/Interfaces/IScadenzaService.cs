using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione Scadenze
    /// Gestisce lo scadenzario con supporto per scadenze automatiche e manuali
    /// CRITICO: utilizzato da background job giornaliero
    /// </summary>
    public interface IScadenzaService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutte le scadenze
        /// </summary>
        Task<IEnumerable<ScadenzaListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene scadenze in un range di date (per calendario)
        /// </summary>
        Task<IEnumerable<ScadenzaListViewModel>> GetByDataRangeAsync(DateTime start, DateTime end);

        /// <summary>
        /// Ottiene il dettaglio di una scadenza
        /// </summary>
        Task<ScadenzaDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene scadenze di una gara specifica
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <param name="soloAutomatiche">Se true, filtra solo scadenze automatiche; se false, solo manuali; se null, tutte</param>
        Task<IEnumerable<ScadenzaListViewModel>> GetByGaraIdAsync(Guid garaId, bool? soloAutomatiche = null);

        /// <summary>
        /// Ottiene scadenze di un lotto specifico
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="soloAutomatiche">Se true, filtra solo scadenze automatiche; se false, solo manuali; se null, tutte</param>
        Task<IEnumerable<ScadenzaListViewModel>> GetByLottoIdAsync(Guid lottoId, bool? soloAutomatiche = null);

        /// <summary>
        /// Ottiene scadenze di un preventivo specifico
        /// </summary>
        /// <param name="preventivoId">ID del preventivo</param>
        /// <param name="soloAutomatiche">Se true, filtra solo scadenze automatiche; se false, solo manuali; se null, tutte</param>
        Task<IEnumerable<ScadenzaListViewModel>> GetByPreventivoIdAsync(Guid preventivoId, bool? soloAutomatiche = null);

        /// <summary>
        /// Ottiene una scadenza automatica specifica per gara e tipo
        /// Usato per sincronizzazione scadenze automatiche nell'Helper
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <param name="tipo">Tipo di scadenza</param>
        Task<ScadenzaDetailsViewModel?> GetScadenzaAutomaticaByGaraAndTipoAsync(Guid garaId, TipoScadenza tipo);

        /// <summary>
        /// Ottiene una scadenza automatica specifica per lotto e tipo
        /// Usato per sincronizzazione scadenze automatiche nell'Helper
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="tipo">Tipo di scadenza</param>
        Task<ScadenzaDetailsViewModel?> GetScadenzaAutomaticaByLottoAndTipoAsync(Guid lottoId, TipoScadenza tipo);

        /// <summary>
        /// Ottiene scadenze filtrate per tipo
        /// </summary>
        Task<IEnumerable<ScadenzaListViewModel>> GetByTipoAsync(TipoScadenza tipo);

        /// <summary>
        /// Ottiene scadenze attive (non completate)
        /// </summary>
        Task<IEnumerable<ScadenzaListViewModel>> GetAttiveAsync();

        /// <summary>
        /// Ottiene scadenze completate
        /// </summary>
        Task<IEnumerable<ScadenzaListViewModel>> GetCompletateAsync();

        /// <summary>
        /// Ottiene scadenze in scadenza (entro X giorni)
        /// CRITICO: usato per dashboard e notifiche
        /// </summary>
        /// <param name="giorniPreavviso">Giorni di preavviso (default 7)</param>
        Task<IEnumerable<ScadenzaListViewModel>> GetInScadenzaAsync(int giorniPreavviso = 7);

        /// <summary>
        /// Ottiene scadenze scadute (passate e non completate)
        /// CRITICO: usato dal background job
        /// </summary>
        Task<IEnumerable<ScadenzaListViewModel>> GetScaduteAsync();

        /// <summary>
        /// Ottiene scadenze di oggi
        /// </summary>
        Task<IEnumerable<ScadenzaListViewModel>> GetOggiAsync();

        /// <summary>
        /// Ottiene scadenze automatiche
        /// </summary>
        Task<IEnumerable<ScadenzaListViewModel>> GetAutomaticheAsync();

        /// <summary>
        /// Ottiene scadenze manuali
        /// </summary>
        Task<IEnumerable<ScadenzaListViewModel>> GetManualiAsync();

        /// <summary>
        /// Ottiene scadenze paginate con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Risultato paginato con ViewModels</returns>
        Task<PagedResult<ScadenzaListViewModel>> GetPagedAsync(ScadenzaFilterViewModel filters);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea una nuova scadenza
        /// Gestisce validazioni business (relazioni, date, tipo, ecc.)
        /// </summary>
        /// <returns>(Success, ErrorMessage, ScadenzaId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? ScadenzaId)> CreateAsync(ScadenzaCreateViewModel model);

        /// <summary>
        /// Aggiorna una scadenza esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(ScadenzaEditViewModel model);

        /// <summary>
        /// Elimina una scadenza (soft delete)
        /// Le scadenze automatiche non possono essere eliminate manualmente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id);

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE
        // ===================================

        /// <summary>
        /// Completa una scadenza
        /// </summary>
        /// <param name="scadenzaId">ID della scadenza</param>
        /// <param name="dataCompletamento">Data completamento (default oggi)</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> CompletaAsync(Guid scadenzaId, DateTime? dataCompletamento = null);

        /// <summary>
        /// Riattiva una scadenza completata
        /// </summary>
        /// <param name="scadenzaId">ID della scadenza</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> RiattivaAsync(Guid scadenzaId);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Valida la coerenza tra tipo di scadenza e relazioni
        /// </summary>
        Task<(bool IsValid, string? ErrorMessage)> ValidaTipoERelazioneAsync(
            TipoScadenza tipo,
            Guid? garaId,
            Guid? lottoId,
            Guid? preventivoId);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene statistiche complete sulle scadenze
        /// </summary>
        Task<Dictionary<string, int>> GetStatisticheAsync();

        /// <summary>
        /// Ottiene il conteggio delle scadenze per tipo
        /// </summary>
        Task<Dictionary<TipoScadenza, int>> GetCountByTipoAsync();

        /// <summary>
        /// Ottiene il conteggio delle scadenze attive
        /// </summary>
        Task<int> GetCountAttiveAsync();

        /// <summary>
        /// Ottiene il conteggio delle scadenze in scadenza
        /// </summary>
        Task<int> GetCountInScadenzaAsync(int giorniPreavviso = 7);

        /// <summary>
        /// Ottiene il conteggio delle scadenze scadute
        /// </summary>
        Task<int> GetCountScaduteAsync();
    }
}