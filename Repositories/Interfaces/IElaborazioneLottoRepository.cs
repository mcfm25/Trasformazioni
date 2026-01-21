using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per il repository delle Elaborazioni Lotto
    /// Definisce le operazioni di accesso ai dati per l'entità ElaborazioneLotto
    /// Gestisce la fase di elaborazione con prezzi desiderati e reali
    /// </summary>
    public interface IElaborazioneLottoRepository
    {
        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        /// <summary>
        /// Ottiene tutte le elaborazioni
        /// </summary>
        Task<IEnumerable<ElaborazioneLotto>> GetAllAsync();

        /// <summary>
        /// Ottiene un'elaborazione per ID
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        Task<ElaborazioneLotto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge una nuova elaborazione
        /// </summary>
        /// <param name="elaborazione">Entità elaborazione da aggiungere</param>
        Task<ElaborazioneLotto> AddAsync(ElaborazioneLotto elaborazione);

        /// <summary>
        /// Aggiorna un'elaborazione esistente
        /// </summary>
        /// <param name="elaborazione">Entità elaborazione da aggiornare</param>
        Task UpdateAsync(ElaborazioneLotto elaborazione);

        /// <summary>
        /// Elimina un'elaborazione (soft delete)
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Verifica se esiste un'elaborazione con l'ID specificato
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        /// <summary>
        /// Ottiene un'elaborazione con il lotto incluso
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        Task<ElaborazioneLotto?> GetWithLottoAsync(Guid id);

        /// <summary>
        /// Ottiene un'elaborazione con tutte le relazioni (lotto, gara)
        /// </summary>
        /// <param name="id">ID dell'elaborazione</param>
        Task<ElaborazioneLotto?> GetCompleteAsync(Guid id);

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        /// <summary>
        /// Ottiene l'elaborazione di un lotto specifico
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<ElaborazioneLotto?> GetByLottoIdAsync(Guid lottoId);

        // ===================================
        // RICERCHE PER PREZZO
        // ===================================

        /// <summary>
        /// Ottiene elaborazioni con prezzo desiderato in un determinato range
        /// </summary>
        /// <param name="min">Prezzo minimo</param>
        /// <param name="max">Prezzo massimo</param>
        Task<IEnumerable<ElaborazioneLotto>> GetByPrezzoDesideratoRangeAsync(decimal min, decimal max);

        /// <summary>
        /// Ottiene elaborazioni con prezzo reale in un determinato range
        /// </summary>
        /// <param name="min">Prezzo minimo</param>
        /// <param name="max">Prezzo massimo</param>
        Task<IEnumerable<ElaborazioneLotto>> GetByPrezzoRealeRangeAsync(decimal min, decimal max);

        /// <summary>
        /// Ottiene elaborazioni con scostamento tra prezzo desiderato e reale
        /// (dove entrambi i prezzi sono valorizzati e diversi)
        /// </summary>
        Task<IEnumerable<ElaborazioneLotto>> GetWithScostamentoAsync();

        /// <summary>
        /// Ottiene elaborazioni dove il prezzo reale supera il desiderato
        /// </summary>
        Task<IEnumerable<ElaborazioneLotto>> GetConPrezzoRealeSuperioreAsync();

        /// <summary>
        /// Ottiene elaborazioni dove il prezzo reale è inferiore al desiderato
        /// </summary>
        Task<IEnumerable<ElaborazioneLotto>> GetConPrezzoRealeInferioreAsync();

        /// <summary>
        /// Ottiene elaborazioni senza prezzi valorizzati
        /// </summary>
        Task<IEnumerable<ElaborazioneLotto>> GetSenzaPrezziAsync();

        // ===================================
        // VALIDAZIONI / ESISTENZA
        // ===================================

        /// <summary>
        /// Verifica se esiste già un'elaborazione per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByLottoIdAsync(Guid lottoId, Guid? excludeId = null);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene lo scostamento medio tra prezzo desiderato e reale
        /// </summary>
        /// <returns>Scostamento medio percentuale</returns>
        Task<decimal?> GetScostamentoMedioAsync();

        /// <summary>
        /// Ottiene il conteggio delle elaborazioni con scostamento
        /// </summary>
        Task<int> GetCountWithScostamentoAsync();

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
        /// </returns>
        Task<Dictionary<string, int>> GetStatistichePrezziAsync();

        Task<int> CountByLottoIdAsync(Guid lottoId);
    }
}