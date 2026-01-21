using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione Preventivi
    /// Include gestione scadenze e auto-rinnovo (monitorati da background job)
    /// </summary>
    public interface IPreventivoService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti i preventivi
        /// </summary>
        Task<IEnumerable<PreventivoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene il dettaglio di un preventivo
        /// </summary>
        Task<PreventivoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene preventivi di un lotto specifico
        /// </summary>
        Task<IEnumerable<PreventivoListViewModel>> GetByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene preventivi di un fornitore specifico
        /// </summary>
        Task<IEnumerable<PreventivoListViewModel>> GetByFornitoreAsync(Guid soggettoId);

        /// <summary>
        /// Ottiene preventivi filtrati per stato
        /// </summary>
        Task<IEnumerable<PreventivoListViewModel>> GetByStatoAsync(StatoPreventivo stato);

        /// <summary>
        /// Ottiene preventivi in attesa
        /// </summary>
        Task<IEnumerable<PreventivoListViewModel>> GetInAttesaAsync();

        /// <summary>
        /// Ottiene preventivi ricevuti
        /// </summary>
        Task<IEnumerable<PreventivoListViewModel>> GetRicevutiAsync();

        /// <summary>
        /// Ottiene preventivi validi
        /// </summary>
        Task<IEnumerable<PreventivoListViewModel>> GetValidiAsync();

        /// <summary>
        /// Ottiene preventivi selezionati
        /// </summary>
        Task<IEnumerable<PreventivoListViewModel>> GetSelezionatiAsync();

        /// <summary>
        /// Ottiene preventivi in scadenza (per notifiche)
        /// CRITICO: usato per dashboard e notifiche
        /// </summary>
        /// <param name="giorniPreavviso">Giorni di preavviso (default 7)</param>
        Task<IEnumerable<PreventivoListViewModel>> GetInScadenzaAsync(int giorniPreavviso = 7);

        /// <summary>
        /// Ottiene preventivi scaduti
        /// CRITICO: usato dal background job per aggiornare stati
        /// </summary>
        Task<IEnumerable<PreventivoListViewModel>> GetScadutiAsync();

        /// <summary>
        /// Ottiene preventivi paginati con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Risultato paginato con ViewModels</returns>
        Task<PagedResult<PreventivoListViewModel>> GetPagedAsync(PreventivoFilterViewModel filters);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea un nuovo preventivo
        /// Gestisce validazioni business (esistenza lotto/fornitore, date valide, ecc.)
        /// </summary>
        /// <returns>(Success, ErrorMessage, PreventivoId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? PreventivoId)> CreateAsync(PreventivoCreateViewModel model, string? documentPath = null);

        /// <summary>
        /// Aggiorna un preventivo esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(PreventivoEditViewModel model, string? newDocumentPath = null);

        /// <summary>
        /// Elimina un preventivo (soft delete)
        /// Verifica che non sia selezionato
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id);

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE
        // ===================================

        /// <summary>
        /// Toggle flag IsSelezionato del preventivo
        /// Permette di marcare/smarcare N preventivi come selezionati (nessun limite)
        /// Diverso da SelezionaPreventivoAsync che deseleziona gli altri
        /// </summary>
        /// <param name="preventivoId">ID del preventivo</param>
        /// <returns>(Success, ErrorMessage, NuovoStatoSelezionato)</returns>
        Task<(bool Success, string? ErrorMessage, bool NuovoStato)> ToggleSelezionatoAsync(Guid preventivoId);

        /// <summary>
        /// Seleziona un preventivo per un lotto
        /// Deseleziona automaticamente gli altri preventivi dello stesso lotto
        /// </summary>
        /// <param name="preventivoId">ID del preventivo da selezionare</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> SelezionaPreventivoAsync(Guid preventivoId);

        /// <summary>
        /// Conferma ricezione preventivo e cambia stato
        /// </summary>
        /// <param name="preventivoId">ID del preventivo</param>
        /// <param name="dataRicezione">Data ricezione</param>
        /// <param name="importoOfferto">Importo offerto dal fornitore</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> ConfermaRicezioneAsync(Guid preventivoId, DateTime dataRicezione, decimal? importoOfferto);

        /// <summary>
        /// Valida un preventivo ricevuto
        /// Cambia lo stato da Ricevuto a Valido
        /// </summary>
        /// <param name="preventivoId">ID del preventivo</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> ValidaPreventivoAsync(Guid preventivoId);

        /// <summary>
        /// Rinnova un preventivo scaduto (se ha auto-rinnovo)
        /// CRITICO: chiamato dal background job
        /// </summary>
        /// <param name="preventivoId">ID del preventivo da rinnovare</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> RinnovaPreventivoAsync(Guid preventivoId);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se la data di scadenza è valida
        /// </summary>
        Task<bool> ValidaScadenzaAsync(DateTime dataScadenza, DateTime dataRichiesta);

        /// <summary>
        /// Verifica se un preventivo può essere selezionato
        /// </summary>
        Task<bool> CanSelectPreventivoAsync(Guid preventivoId);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio dei preventivi per stato
        /// </summary>
        Task<Dictionary<StatoPreventivo, int>> GetCountByStatoAsync();

        /// <summary>
        /// Ottiene l'importo medio dei preventivi per un lotto
        /// </summary>
        Task<decimal?> GetImportoMedioByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene l'importo minimo tra i preventivi validi di un lotto
        /// </summary>
        Task<decimal?> GetImportoMinimoValidiByLottoIdAsync(Guid lottoId);
    }
}