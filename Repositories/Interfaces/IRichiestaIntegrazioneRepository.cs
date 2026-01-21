using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per il repository delle Richieste Integrazione
    /// Definisce le operazioni di accesso ai dati per l'entità RichiestaIntegrazione
    /// Gestisce il ping-pong di richieste/risposte con l'ente
    /// </summary>
    public interface IRichiestaIntegrazioneRepository
    {
        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        /// <summary>
        /// Ottiene tutte le richieste di integrazione
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazione>> GetAllAsync();

        /// <summary>
        /// Ottiene una richiesta per ID
        /// </summary>
        /// <param name="id">ID della richiesta</param>
        Task<RichiestaIntegrazione?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge una nuova richiesta di integrazione
        /// </summary>
        /// <param name="richiesta">Entità richiesta da aggiungere</param>
        Task<RichiestaIntegrazione> AddAsync(RichiestaIntegrazione richiesta);

        /// <summary>
        /// Aggiorna una richiesta esistente
        /// </summary>
        /// <param name="richiesta">Entità richiesta da aggiornare</param>
        Task UpdateAsync(RichiestaIntegrazione richiesta);

        /// <summary>
        /// Elimina una richiesta (soft delete)
        /// </summary>
        /// <param name="id">ID della richiesta</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Verifica se esiste una richiesta con l'ID specificato
        /// </summary>
        /// <param name="id">ID della richiesta</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        /// <summary>
        /// Ottiene una richiesta con il lotto incluso
        /// </summary>
        /// <param name="id">ID della richiesta</param>
        Task<RichiestaIntegrazione?> GetWithLottoAsync(Guid id);

        /// <summary>
        /// Ottiene una richiesta con l'utente che ha risposto incluso
        /// </summary>
        /// <param name="id">ID della richiesta</param>
        Task<RichiestaIntegrazione?> GetWithRispostaDaAsync(Guid id);

        /// <summary>
        /// Ottiene una richiesta con tutte le relazioni (lotto, gara, utente risposta)
        /// </summary>
        /// <param name="id">ID della richiesta</param>
        Task<RichiestaIntegrazione?> GetCompleteAsync(Guid id);

        // ===================================
        // RICERCHE PER LOTTO
        // ===================================

        /// <summary>
        /// Ottiene tutte le richieste di un lotto specifico
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<RichiestaIntegrazione>> GetByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene tutte le richieste di un lotto ordinate per NumeroProgressivo
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<RichiestaIntegrazione>> GetByLottoIdOrderedAsync(Guid lottoId);

        /// <summary>
        /// Ottiene solo le richieste aperte di un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<RichiestaIntegrazione>> GetByLottoIdAperteAsync(Guid lottoId);

        /// <summary>
        /// Ottiene solo le richieste chiuse di un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<RichiestaIntegrazione>> GetByLottoIdChiuseAsync(Guid lottoId);

        // ===================================
        // RICERCHE PER STATO
        // ===================================

        /// <summary>
        /// Ottiene richieste di integrazione paginate con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Tupla con (Items, TotalCount)</returns>
        Task<(IEnumerable<RichiestaIntegrazione> Items, int TotalCount)> GetPagedAsync(RichiestaIntegrazioneFilterViewModel filters);

        /// <summary>
        /// Ottiene tutte le richieste aperte (IsChiusa = false)
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazione>> GetAperteAsync();

        /// <summary>
        /// Ottiene tutte le richieste chiuse (IsChiusa = true)
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazione>> GetChiuseAsync();

        /// <summary>
        /// Ottiene le richieste non ancora risposte (DataRispostaAzienda = null)
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazione>> GetNonRisposteAsync();

        /// <summary>
        /// Ottiene le richieste risposte ma non ancora chiuse
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazione>> GetRisposteNonChiuseAsync();

        /// <summary>
        /// Ottiene le richieste scadute (più di X giorni senza risposta)
        /// </summary>
        /// <param name="giorniScadenza">Numero di giorni per considerare la richiesta scaduta</param>
        Task<IEnumerable<RichiestaIntegrazione>> GetScaduteAsync(int giorniScadenza = 7);

        // ===================================
        // RICERCHE PER UTENTE
        // ===================================

        /// <summary>
        /// Ottiene tutte le richieste risposte da un utente specifico
        /// </summary>
        /// <param name="userId">ID dell'utente</param>
        Task<IEnumerable<RichiestaIntegrazione>> GetByRispostaDaUserIdAsync(string userId);

        // ===================================
        // RICERCHE PER DATA
        // ===================================

        /// <summary>
        /// Ottiene richieste in un range di date (data richiesta ente)
        /// </summary>
        /// <param name="dataInizio">Data inizio</param>
        /// <param name="dataFine">Data fine</param>
        Task<IEnumerable<RichiestaIntegrazione>> GetByDataRichiestaRangeAsync(DateTime dataInizio, DateTime dataFine);

        // ===================================
        // VALIDAZIONI / UTILITY
        // ===================================

        /// <summary>
        /// Ottiene il prossimo numero progressivo disponibile per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<int> GetNextNumeroProgressivoAsync(Guid lottoId);

        /// <summary>
        /// Verifica se esiste già una richiesta con il numero progressivo specificato per il lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="numeroProgressivo">Numero progressivo</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByLottoAndNumeroAsync(Guid lottoId, int numeroProgressivo, Guid? excludeId = null);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio delle richieste aperte
        /// </summary>
        Task<int> GetCountAperteAsync();

        /// <summary>
        /// Ottiene il conteggio delle richieste per lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<int> GetCountByLottoAsync(Guid lottoId);

        /// <summary>
        /// Ottiene il conteggio delle richieste aperte per lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<int> GetCountAperteByLottoAsync(Guid lottoId);

        /// <summary>
        /// Ottiene statistiche sulle richieste
        /// </summary>
        /// <returns>
        /// Dictionary con chiavi:
        /// - TotaleRichieste
        /// - Aperte
        /// - Chiuse
        /// - NonRisposte
        /// - RisposteNonChiuse
        /// - TempoMedioRisposta (in giorni)
        /// </returns>
        Task<Dictionary<string, object>> GetStatisticheAsync();
    }
}