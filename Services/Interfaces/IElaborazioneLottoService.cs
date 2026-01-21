using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interface per il service delle Elaborazioni Lotto
    /// Gestisce la logica di business per la fase di elaborazione con prezzi desiderati e reali
    /// </summary>
    public interface IElaborazioneLottoService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutte le elaborazioni
        /// </summary>
        Task<IEnumerable<ElaborazioneLottoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene un'elaborazione per ID con tutte le relazioni
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        Task<ElaborazioneLottoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene l'elaborazione di un lotto specifico
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<ElaborazioneLottoDetailsViewModel?> GetByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene un'elaborazione per modifica (EditViewModel)
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        Task<ElaborazioneLottoEditViewModel?> GetForEditAsync(Guid id);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea una nuova elaborazione
        /// Validazioni:
        /// - Il lotto deve esistere
        /// - Il lotto non deve avere già un'elaborazione
        /// - Se PrezzoDesiderato != PrezzoRealeUscita → MotivazioneAdattamento obbligatoria
        /// </summary>
        /// <param name="model">Dati per la creazione</param>
        /// <param name="currentUserId">ID dell'utente corrente (per audit)</param>
        /// <returns>Tupla (Success, ErrorMessage, Id)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? Id)> CreateAsync(
            ElaborazioneLottoCreateViewModel model,
            string currentUserId
        );

        /// <summary>
        /// Aggiorna un'elaborazione esistente
        /// Validazioni:
        /// - L'elaborazione deve esistere
        /// - Se PrezzoDesiderato != PrezzoRealeUscita → MotivazioneAdattamento obbligatoria
        /// </summary>
        /// <param name="model">Dati per l'aggiornamento</param>
        /// <param name="currentUserId">ID dell'utente corrente (per audit)</param>
        /// <returns>Tupla (Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            ElaborazioneLottoEditViewModel model,
            string currentUserId
        );

        /// <summary>
        /// Elimina un'elaborazione (soft delete)
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        /// <param name="currentUserId">ID dell'utente corrente (per audit)</param>
        /// <returns>Tupla (Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId);

        // ===================================
        // QUERY SPECIFICHE - PREZZI
        // ===================================

        /// <summary>
        /// Ottiene elaborazioni con scostamento tra prezzo desiderato e reale
        /// (dove entrambi i prezzi sono valorizzati e diversi)
        /// </summary>
        Task<IEnumerable<ElaborazioneLottoListViewModel>> GetWithScostamentoAsync();

        /// <summary>
        /// Ottiene elaborazioni dove il prezzo reale supera il desiderato
        /// </summary>
        Task<IEnumerable<ElaborazioneLottoListViewModel>> GetConPrezzoRealeSuperioreAsync();

        /// <summary>
        /// Ottiene elaborazioni dove il prezzo reale è inferiore al desiderato
        /// </summary>
        Task<IEnumerable<ElaborazioneLottoListViewModel>> GetConPrezzoRealeInferioreAsync();

        /// <summary>
        /// Ottiene elaborazioni senza prezzi valorizzati
        /// </summary>
        Task<IEnumerable<ElaborazioneLottoListViewModel>> GetSenzaPrezziAsync();

        // ===================================
        // CALCOLI
        // ===================================

        /// <summary>
        /// Calcola lo scostamento percentuale per un'elaborazione specifica
        /// Formula: |PrezzoReale - PrezzoDesiderato| / PrezzoDesiderato * 100
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        /// <returns>Scostamento percentuale o null se non calcolabile</returns>
        Task<decimal?> CalcolaScostamentoAsync(Guid id);

        /// <summary>
        /// Ottiene lo scostamento medio di tutte le elaborazioni
        /// </summary>
        Task<decimal?> GetScostamentoMedioAsync();

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se un lotto ha già un'elaborazione
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> LottoHasElaborazioneAsync(Guid lottoId, Guid? excludeId = null);

        /// <summary>
        /// Verifica se esiste un'elaborazione con l'ID specificato
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene statistiche sui prezzi
        /// </summary>
        /// <returns>
        /// Dictionary con chiavi:
        /// - TotaleElaborazioni
        /// - ConPrezzoDesiderato
        /// - ConPrezzoReale
        /// - ConEntrambiPrezzi
        /// - ConScostamento
        /// - PrezzoRealeSuperiore
        /// - PrezzoRealeInferiore
        /// - SenzaPrezzi
        /// </returns>
        Task<Dictionary<string, int>> GetStatistichePrezziAsync();
    }
}